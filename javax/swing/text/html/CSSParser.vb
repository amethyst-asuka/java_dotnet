Imports Microsoft.VisualBasic
Imports System
Imports System.Text

'
' * Copyright (c) 1999, 2013, Oracle and/or its affiliates. All rights reserved.
' * ORACLE PROPRIETARY/CONFIDENTIAL. Use is subject to license terms.
' *
' *
' *
' *
' *
' *
' *
' *
' *
' *
' *
' *
' *
' *
' *
' *
' *
' *
' *
' *
' 
Namespace javax.swing.text.html


	''' <summary>
	''' A CSS parser. This works by way of a delegate that implements the
	''' CSSParserCallback interface. The delegate is notified of the following
	''' events:
	''' <ul>
	'''   <li>Import statement: <code>handleImport</code>
	'''   <li>Selectors <code>handleSelector</code>. This is invoked for each
	'''       string. For example if the Reader contained p, bar , a {}, the delegate
	'''       would be notified 4 times, for 'p,' 'bar' ',' and 'a'.
	'''   <li>When a rule starts, <code>startRule</code>
	'''   <li>Properties in the rule via the <code>handleProperty</code>. This
	'''       is invoked one per property/value key, eg font size: foo;, would
	'''       cause the delegate to be notified once with a value of 'font size'.
	'''   <li>Values in the rule via the <code>handleValue</code>, this is notified
	'''       for the total value.
	'''   <li>When a rule ends, <code>endRule</code>
	''' </ul>
	''' This will parse much more than CSS 1, and loosely implements the
	''' recommendation for <i>Forward-compatible parsing</i> in section
	''' 7.1 of the CSS spec found at:
	''' <a href=http://www.w3.org/TR/REC-CSS1>http://www.w3.org/TR/REC-CSS1</a>.
	''' If an error results in parsing, a RuntimeException will be thrown.
	''' <p>
	''' This will preserve case. If the callback wishes to treat certain poritions
	''' case insensitively (such as selectors), it should use toLowerCase, or
	''' something similar.
	''' 
	''' @author Scott Violet
	''' </summary>
	Friend Class CSSParser
		' Parsing something like the following:
		' (@rule | ruleset | block)*
		'
		' @rule       (block | identifier)*; (block with {} ends @rule)
		' block       matching [] () {} (that is, [()] is a block, [(){}{[]}]
		'                                is a block, ()[] is two blocks)
		' identifier  "*" | '*' | anything but a [](){} and whitespace
		'
		' ruleset     selector decblock
		' selector    (identifier | (block, except block '{}') )*
		' declblock   declaration* block*
		' declaration (identifier* stopping when identifier ends with :)
		'             (identifier* stopping when identifier ends with ;)
		'
		' comments /* */ can appear any where, and are stripped.


		' identifier - letters, digits, dashes and escaped characters
		' block starts with { ends with matching }, () [] and {} always occur
		'   in matching pairs, '' and "" also occur in pairs, except " may be


		' Indicates the type of token being parsed.
		Private Const IDENTIFIER As Integer = 1
		Private Const BRACKET_OPEN As Integer = 2
		Private Const BRACKET_CLOSE As Integer = 3
		Private Const BRACE_OPEN As Integer = 4
		Private Const BRACE_CLOSE As Integer = 5
		Private Const PAREN_OPEN As Integer = 6
		Private Const PAREN_CLOSE As Integer = 7
		Private Const [END] As Integer = -1

		Private Shared ReadOnly charMapping As Char() = { 0, 0, "["c, "]"c, "{"c, "}"c, "("c, ")"c, 0}


		''' <summary>
		''' Set to true if one character has been read ahead. </summary>
		Private didPushChar As Boolean
		''' <summary>
		''' The read ahead character. </summary>
		Private pushedChar As Integer
		''' <summary>
		''' Temporary place to hold identifiers. </summary>
		Private unitBuffer As StringBuilder
		''' <summary>
		''' Used to indicate blocks. </summary>
		Private unitStack As Integer()
		''' <summary>
		''' Number of valid blocks. </summary>
		Private stackCount As Integer
		''' <summary>
		''' Holds the incoming CSS rules. </summary>
		Private reader As Reader
		''' <summary>
		''' Set to true when the first non @ rule is encountered. </summary>
		Private encounteredRuleSet As Boolean
		''' <summary>
		''' Notified of state. </summary>
		Private callback As CSSParserCallback
		''' <summary>
		''' nextToken() inserts the string here. </summary>
		Private tokenBuffer As Char()
		''' <summary>
		''' Current number of chars in tokenBufferLength. </summary>
		Private tokenBufferLength As Integer
		''' <summary>
		''' Set to true if any whitespace is read. </summary>
		Private ___readWS As Boolean


		' The delegate interface.
		Friend Interface CSSParserCallback
			''' <summary>
			''' Called when an @import is encountered. </summary>
			Sub handleImport(ByVal importString As String)
			' There is currently no way to distinguish between '"foo,"' and
			' 'foo,'. But this generally isn't valid CSS. If it becomes
			' a problem, handleSelector will have to be told if the string is
			' quoted.
			Sub handleSelector(ByVal selector As String)
			Sub startRule()
			' Property names are mapped to lower case before being passed to
			' the delegate.
			Sub handleProperty(ByVal [property] As String)
			Sub handleValue(ByVal value As String)
			Sub endRule()
		End Interface

		Friend Sub New()
			unitStack = New Integer(1){}
			tokenBuffer = New Char(79){}
			unitBuffer = New StringBuilder
		End Sub

		Friend Overridable Sub parse(ByVal reader As Reader, ByVal callback As CSSParserCallback, ByVal inRule As Boolean)
			Me.callback = callback
				tokenBufferLength = 0
				stackCount = tokenBufferLength
			Me.reader = reader
			encounteredRuleSet = False
			Try
				If inRule Then
					parseDeclarationBlock()
				Else
					Do While nextStatement

					Loop
				End If
			Finally
				callback = Nothing
				reader = Nothing
			End Try
		End Sub

		''' <summary>
		''' Gets the next statement, returning false if the end is reached. A
		''' statement is either an @rule, or a ruleset.
		''' </summary>
		Private Property nextStatement As Boolean
			Get
				unitBuffer.Length = 0
    
				Dim token As Integer = nextToken(ChrW(0))
    
				Select Case token
				Case IDENTIFIER
					If tokenBufferLength > 0 Then
						If tokenBuffer(0) = "@"c Then
							parseAtRule()
						Else
							encounteredRuleSet = True
							parseRuleSet()
						End If
					End If
					Return True
				Case BRACKET_OPEN, BRACE_OPEN, PAREN_OPEN
					parseTillClosed(token)
					Return True
    
				Case BRACKET_CLOSE, BRACE_CLOSE, PAREN_CLOSE
					' Shouldn't happen...
					Throw New Exception("Unexpected top level block close")
    
				Case [END]
					Return False
				End Select
				Return True
			End Get
		End Property

		''' <summary>
		''' Parses an @ rule, stopping at a matching brace pair, or ;.
		''' </summary>
		Private Sub parseAtRule()
			' PENDING: make this more effecient.
			Dim done As Boolean = False
			Dim isImport As Boolean = (tokenBufferLength = 7 AndAlso tokenBuffer(0) = "@"c AndAlso tokenBuffer(1) = "i"c AndAlso tokenBuffer(2) = "m"c AndAlso tokenBuffer(3) = "p"c AndAlso tokenBuffer(4) = "o"c AndAlso tokenBuffer(5) = "r"c AndAlso tokenBuffer(6) = "t"c)

			unitBuffer.Length = 0
			Do While Not done
				Dim nextToken As Integer = nextToken(";"c)

				Select Case nextToken
				Case IDENTIFIER
					If tokenBufferLength > 0 AndAlso tokenBuffer(tokenBufferLength - 1) = ";"c Then
						tokenBufferLength -= 1
						done = True
					End If
					If tokenBufferLength > 0 Then
						If unitBuffer.Length > 0 AndAlso ___readWS Then unitBuffer.Append(" "c)
						unitBuffer.Append(tokenBuffer, 0, tokenBufferLength)
					End If

				Case BRACE_OPEN
					If unitBuffer.Length > 0 AndAlso ___readWS Then unitBuffer.Append(" "c)
					unitBuffer.Append(charMapping(nextToken))
					parseTillClosed(nextToken)
					done = True
					' Skip a tailing ';', not really to spec.
						Dim nextChar As Integer = readWS()
						If nextChar <> -1 AndAlso nextChar <> AscW(";"c) Then pushChar(nextChar)

				Case BRACKET_OPEN, PAREN_OPEN
					unitBuffer.Append(charMapping(nextToken))
					parseTillClosed(nextToken)

				Case BRACKET_CLOSE, BRACE_CLOSE, PAREN_CLOSE
					Throw New Exception("Unexpected close in @ rule")

				Case [END]
					done = True
				End Select
			Loop
			If isImport AndAlso (Not encounteredRuleSet) Then callback.handleImport(unitBuffer.ToString())
		End Sub

		''' <summary>
		''' Parses the next rule set, which is a selector followed by a
		''' declaration block.
		''' </summary>
		Private Sub parseRuleSet()
			If parseSelectors() Then
				callback.startRule()
				parseDeclarationBlock()
				callback.endRule()
			End If
		End Sub

		''' <summary>
		''' Parses a set of selectors, returning false if the end of the stream
		''' is reached.
		''' </summary>
		Private Function parseSelectors() As Boolean
			' Parse the selectors
			Dim nextToken As Integer

			If tokenBufferLength > 0 Then callback.handleSelector(New String(tokenBuffer, 0, tokenBufferLength))

			unitBuffer.Length = 0
			Do
				nextToken = nextToken(ChrW(0))
				Do While nextToken = IDENTIFIER
					If tokenBufferLength > 0 Then callback.handleSelector(New String(tokenBuffer, 0, tokenBufferLength))
					nextToken = nextToken(ChrW(0))
				Loop
				Select Case nextToken
				Case BRACE_OPEN
					Return True

				Case BRACKET_OPEN, PAREN_OPEN
					parseTillClosed(nextToken)
					' Not too sure about this, how we handle this isn't very
					' well spec'd.
					unitBuffer.Length = 0

				Case BRACKET_CLOSE, BRACE_CLOSE, PAREN_CLOSE
					Throw New Exception("Unexpected block close in selector")

				Case [END]
					' Prematurely hit end.
					Return False
				End Select
			Loop
		End Function

		''' <summary>
		''' Parses a declaration block. Which a number of declarations followed
		''' by a })].
		''' </summary>
		Private Sub parseDeclarationBlock()
			Do
				Dim token As Integer = parseDeclaration()
				Select Case token
				Case [END], BRACE_CLOSE
					Return

				Case BRACKET_CLOSE, PAREN_CLOSE
					' Bail
					Throw New Exception("Unexpected close in declaration block")
				Case IDENTIFIER
				End Select
			Loop
		End Sub

		''' <summary>
		''' Parses a single declaration, which is an identifier a : and another
		''' identifier. This returns the last token seen.
		''' </summary>
		' identifier+: identifier* ;|}
		Private Function parseDeclaration() As Integer
			Dim token As Integer

			token = parseIdentifiers(":"c, False)
			If token <> IDENTIFIER Then Return token
			' Make the property name to lowercase
			For counter As Integer = unitBuffer.Length - 1 To 0 Step -1
				unitBuffer(counter) = Char.ToLower(unitBuffer.Chars(counter))
			Next counter
			callback.handleProperty(unitBuffer.ToString())

			token = parseIdentifiers(";"c, True)
			callback.handleValue(unitBuffer.ToString())
			Return token
		End Function

		''' <summary>
		''' Parses identifiers until <code>extraChar</code> is encountered,
		''' returning the ending token, which will be IDENTIFIER if extraChar
		''' is found.
		''' </summary>
		Private Function parseIdentifiers(ByVal extraChar As Char, ByVal wantsBlocks As Boolean) As Integer
			Dim nextToken As Integer
			Dim ubl As Integer

			unitBuffer.Length = 0
			Do
				nextToken = nextToken(extraChar)

				Select Case nextToken
				Case IDENTIFIER
					If tokenBufferLength > 0 Then
						If tokenBuffer(tokenBufferLength - 1) = extraChar Then
							tokenBufferLength -= 1
							If tokenBufferLength > 0 Then
								If ___readWS AndAlso unitBuffer.Length > 0 Then unitBuffer.Append(" "c)
								unitBuffer.Append(tokenBuffer, 0, tokenBufferLength)
							End If
							Return IDENTIFIER
						End If
						If ___readWS AndAlso unitBuffer.Length > 0 Then unitBuffer.Append(" "c)
						unitBuffer.Append(tokenBuffer, 0, tokenBufferLength)
					End If

				Case BRACKET_OPEN, BRACE_OPEN, PAREN_OPEN
					ubl = unitBuffer.Length
					If wantsBlocks Then unitBuffer.Append(charMapping(nextToken))
					parseTillClosed(nextToken)
					If Not wantsBlocks Then unitBuffer.Length = ubl

				Case BRACE_CLOSE, BRACKET_CLOSE, PAREN_CLOSE, [END]
					' No need to throw for these two, we return token and
					' caller can do whatever.
					' Hit the end
					Return nextToken
				End Select
			Loop
		End Function

		''' <summary>
		''' Parses till a matching block close is encountered. This is only
		''' appropriate to be called at the top level (no nesting).
		''' </summary>
		Private Sub parseTillClosed(ByVal openToken As Integer)
			Dim nextToken As Integer
			Dim done As Boolean = False

			startBlock(openToken)
			Do While Not done
				nextToken = nextToken(ChrW(0))
				Select Case nextToken
				Case IDENTIFIER
					If unitBuffer.Length > 0 AndAlso ___readWS Then unitBuffer.Append(" "c)
					If tokenBufferLength > 0 Then unitBuffer.Append(tokenBuffer, 0, tokenBufferLength)

				Case BRACKET_OPEN, BRACE_OPEN, PAREN_OPEN
					If unitBuffer.Length > 0 AndAlso ___readWS Then unitBuffer.Append(" "c)
					unitBuffer.Append(charMapping(nextToken))
					startBlock(nextToken)

				Case BRACKET_CLOSE, BRACE_CLOSE, PAREN_CLOSE
					If unitBuffer.Length > 0 AndAlso ___readWS Then unitBuffer.Append(" "c)
					unitBuffer.Append(charMapping(nextToken))
					endBlock(nextToken)
					If Not inBlock() Then done = True

				Case [END]
					' Prematurely hit end.
					Throw New Exception("Unclosed block")
				End Select
			Loop
		End Sub

		''' <summary>
		''' Fetches the next token.
		''' </summary>
		Private Function nextToken(ByVal idChar As Char) As Integer
			___readWS = False

			Dim nextChar As Integer = readWS()

			Select Case nextChar
			Case "'"c
				readTill("'"c)
				If tokenBufferLength > 0 Then tokenBufferLength -= 1
				Return IDENTIFIER
			Case """"c
				readTill(""""c)
				If tokenBufferLength > 0 Then tokenBufferLength -= 1
				Return IDENTIFIER
			Case "["c
				Return BRACKET_OPEN
			Case "]"c
				Return BRACKET_CLOSE
			Case "{"c
				Return BRACE_OPEN
			Case "}"c
				Return BRACE_CLOSE
			Case "("c
				Return PAREN_OPEN
			Case ")"c
				Return PAREN_CLOSE
			Case -1
				Return [END]
			Case Else
				pushChar(nextChar)
				getIdentifier(idChar)
				Return IDENTIFIER
			End Select
		End Function

		''' <summary>
		''' Gets an identifier, returning true if the length of the string is greater than 0,
		''' stopping when <code>stopChar</code>, whitespace, or one of {}()[] is
		''' hit.
		''' </summary>
		' NOTE: this could be combined with readTill, as they contain somewhat
		' similar functionality.
		Private Function getIdentifier(ByVal stopChar As Char) As Boolean
			Dim lastWasEscape As Boolean = False
			Dim done As Boolean = False
			Dim escapeCount As Integer = 0
			Dim escapeChar As Integer = 0
			Dim nextChar As Integer
			Dim intStopChar As Integer = AscW(stopChar)
			' 1 for '\', 2 for valid escape char [0-9a-fA-F], 3 for
			' stop character (white space, ()[]{}) 0 otherwise
			Dim type As Short
			Dim escapeOffset As Integer = 0

			tokenBufferLength = 0
			Do While Not done
				nextChar = readChar()
				Select Case nextChar
				Case "\"c
					type = 1

				Case "0"c, "1"c, "2"c, "3"c, "4"c, "5"c, "6"c, "7"c, "8"c, "9"c
					type = 2
					escapeOffset = nextChar - AscW("0"c)

				Case "a"c, "b"c, "c"c, "d"c, "e"c, "f"c
					type = 2
					escapeOffset = nextChar - AscW("a"c) + 10

				Case "A"c, "B"c, "C"c, "D"c, "E"c, "F"c
					type = 2
					escapeOffset = nextChar - AscW("A"c) + 10

				Case "'"c, """"c, "["c, "]"c, "{"c, "}"c, "("c, ")"c, " "c, ControlChars.Lf, ControlChars.Tab, ControlChars.Cr
					type = 3

				Case "/"c
					type = 4

				Case -1
					' Reached the end
					done = True
					type = 0

				Case Else
					type = 0
				End Select
				If lastWasEscape Then
					If type = 2 Then
						' Continue with escape.
						escapeChar = escapeChar * 16 + escapeOffset
						escapeCount += 1
						If escapeCount = 4 Then
							lastWasEscape = False
							append(ChrW(escapeChar))
						End If
					Else
						' no longer escaped
						lastWasEscape = False
						If escapeCount > 0 Then
							append(ChrW(escapeChar))
							' Make this simpler, reprocess the character.
							pushChar(nextChar)
						ElseIf Not done Then
							append(ChrW(nextChar))
						End If
					End If
				ElseIf Not done Then
					If type = 1 Then
						lastWasEscape = True
							escapeCount = 0
							escapeChar = escapeCount
					ElseIf type = 3 Then
						done = True
						pushChar(nextChar)
					ElseIf type = 4 Then
						' Potential comment
						nextChar = readChar()
						If nextChar = AscW("*"c) Then
							done = True
							readComment()
							___readWS = True
						Else
							append("/"c)
							If nextChar = -1 Then
								done = True
							Else
								pushChar(nextChar)
							End If
						End If
					Else
						append(ChrW(nextChar))
						If nextChar = intStopChar Then done = True
					End If
				End If
			Loop
			Return (tokenBufferLength > 0)
		End Function

		''' <summary>
		''' Reads till a <code>stopChar</code> is encountered, escaping characters
		''' as necessary.
		''' </summary>
		Private Sub readTill(ByVal stopChar As Char)
			Dim lastWasEscape As Boolean = False
			Dim escapeCount As Integer = 0
			Dim escapeChar As Integer = 0
			Dim nextChar As Integer
			Dim done As Boolean = False
			Dim intStopChar As Integer = AscW(stopChar)
			' 1 for '\', 2 for valid escape char [0-9a-fA-F], 0 otherwise
			Dim type As Short
			Dim escapeOffset As Integer = 0

			tokenBufferLength = 0
			Do While Not done
				nextChar = readChar()
				Select Case nextChar
				Case "\"c
					type = 1

				Case "0"c, "1"c, "2"c, "3"c, "4"c, "5"c, "6"c, "7"c, "8"c, "9"c
					type = 2
					escapeOffset = nextChar - AscW("0"c)

				Case "a"c, "b"c, "c"c, "d"c, "e"c, "f"c
					type = 2
					escapeOffset = nextChar - AscW("a"c) + 10

				Case "A"c, "B"c, "C"c, "D"c, "E"c, "F"c
					type = 2
					escapeOffset = nextChar - AscW("A"c) + 10

				Case -1
					' Prematurely reached the end!
					Throw New Exception("Unclosed " & AscW(stopChar))

				Case Else
					type = 0
				End Select
				If lastWasEscape Then
					If type = 2 Then
						' Continue with escape.
						escapeChar = escapeChar * 16 + escapeOffset
						escapeCount += 1
						If escapeCount = 4 Then
							lastWasEscape = False
							append(ChrW(escapeChar))
						End If
					Else
						' no longer escaped
						If escapeCount > 0 Then
							append(ChrW(escapeChar))
							If type = 1 Then
								lastWasEscape = True
									escapeCount = 0
									escapeChar = escapeCount
							Else
								If nextChar = intStopChar Then done = True
								append(ChrW(nextChar))
								lastWasEscape = False
							End If
						Else
							append(ChrW(nextChar))
							lastWasEscape = False
						End If
					End If
				ElseIf type = 1 Then
					lastWasEscape = True
						escapeCount = 0
						escapeChar = escapeCount
				Else
					If nextChar = intStopChar Then done = True
					append(ChrW(nextChar))
				End If
			Loop
		End Sub

		Private Sub append(ByVal character As Char)
			If tokenBufferLength = tokenBuffer.Length Then
				Dim newBuffer As Char() = New Char(tokenBuffer.Length * 2 - 1){}
				Array.Copy(tokenBuffer, 0, newBuffer, 0, tokenBuffer.Length)
				tokenBuffer = newBuffer
			End If
			tokenBuffer(tokenBufferLength) = character
			tokenBufferLength += 1
		End Sub

		''' <summary>
		''' Parses a comment block.
		''' </summary>
		Private Sub readComment()
			Dim nextChar As Integer

			Do
				nextChar = readChar()
				Select Case nextChar
				Case -1
					Throw New Exception("Unclosed comment")
				Case "*"c
					nextChar = readChar()
					If nextChar = AscW("/"c) Then
						Return
					ElseIf nextChar = -1 Then
						Throw New Exception("Unclosed comment")
					Else
						pushChar(nextChar)
					End If
				Case Else
				End Select
			Loop
		End Sub

		''' <summary>
		''' Called when a block start is encountered ({[.
		''' </summary>
		Private Sub startBlock(ByVal startToken As Integer)
			If stackCount = unitStack.Length Then
				Dim newUS As Integer() = New Integer(stackCount * 2 - 1){}

				Array.Copy(unitStack, 0, newUS, 0, stackCount)
				unitStack = newUS
			End If
			unitStack(stackCount) = startToken
			stackCount += 1
		End Sub

		''' <summary>
		''' Called when an end block is encountered )]}
		''' </summary>
		Private Sub endBlock(ByVal endToken As Integer)
			Dim startToken As Integer

			Select Case endToken
			Case BRACKET_CLOSE
				startToken = BRACKET_OPEN
			Case BRACE_CLOSE
				startToken = BRACE_OPEN
			Case PAREN_CLOSE
				startToken = PAREN_OPEN
			Case Else
				' Will never happen.
				startToken = -1
			End Select
			If stackCount > 0 AndAlso unitStack(stackCount - 1) = startToken Then
				stackCount -= 1
			Else
				' Invalid state, should do something.
				Throw New Exception("Unmatched block")
			End If
		End Sub

		''' <returns> true if currently in a block. </returns>
		Private Function inBlock() As Boolean
			Return (stackCount > 0)
		End Function

		''' <summary>
		''' Skips any white space, returning the character after the white space.
		''' </summary>
		Private Function readWS() As Integer
			Dim nextChar As Integer
			nextChar = readChar()
			Do While nextChar <> -1 AndAlso Char.IsWhiteSpace(ChrW(nextChar))
				___readWS = True
				nextChar = readChar()
			Loop
			Return nextChar
		End Function

		''' <summary>
		''' Reads a character from the stream.
		''' </summary>
		Private Function readChar() As Integer
			If didPushChar Then
				didPushChar = False
				Return pushedChar
			End If
			Return reader.read()
			' Uncomment the following to do case insensitive parsing.
	'        
	'        if (retValue != -1) {
	'            return (int)Character.toLowerCase((char)retValue);
	'        }
	'        return retValue;
	'        
		End Function

		''' <summary>
		''' Supports one character look ahead, this will throw if called twice
		''' in a row.
		''' </summary>
		Private Sub pushChar(ByVal tempChar As Integer)
			If didPushChar Then Throw New Exception("Can not handle look ahead of more than one character")
			didPushChar = True
			pushedChar = tempChar
		End Sub
	End Class

End Namespace