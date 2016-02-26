Imports Microsoft.VisualBasic
Imports System
Imports System.Text

'
' * Copyright (c) 1997, 2008, Oracle and/or its affiliates. All rights reserved.
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
Namespace javax.swing.text.rtf


	''' <summary>
	''' <b>RTFParser</b> is a subclass of <b>AbstractFilter</b> which understands basic RTF syntax
	''' and passes a stream of control words, text, and begin/end group
	''' indications to its subclass.
	''' 
	''' Normally programmers will only use <b>RTFFilter</b>, a subclass of this class that knows what to
	''' do with the tokens this class parses.
	''' </summary>
	''' <seealso cref= AbstractFilter </seealso>
	''' <seealso cref= RTFFilter </seealso>
	Friend MustInherit Class RTFParser
		Inherits AbstractFilter

	  ''' <summary>
	  ''' The current RTF group nesting level. </summary>
	  Public level As Integer

	  Private state As Integer
	  Private currentCharacters As StringBuilder
	  Private pendingKeyword As String ' where keywords go while we
													' read their parameters
	  Private pendingCharacter As Integer ' for the \'xx construct

	  Private binaryBytesLeft As Long ' in a \bin blob?
	  Friend binaryBuf As ByteArrayOutputStream
	  Private savedSpecials As Boolean()

	  ''' <summary>
	  ''' A stream to which to write warnings and debugging information
	  '''  while parsing. This is set to <code>System.out</code> to log
	  '''  any anomalous information to stdout. 
	  ''' </summary>
	  Protected Friend warnings As PrintStream

	  ' value for the 'state' variable
	  Private ReadOnly S_text As Integer = 0 ' reading random text
	  Private ReadOnly S_backslashed As Integer = 1 ' read a backslash, waiting for next
	  Private ReadOnly S_token As Integer = 2 ' reading a multicharacter token
	  Private ReadOnly S_parameter As Integer = 3 ' reading a token's parameter

	  Private ReadOnly S_aftertick As Integer = 4 ' after reading \'
	  Private ReadOnly S_aftertickc As Integer = 5 ' after reading \'x

	  Private ReadOnly S_inblob As Integer = 6 ' in a \bin blob

	  ''' <summary>
	  ''' Implemented by subclasses to interpret a parameter-less RTF keyword.
	  '''  The keyword is passed without the leading '/' or any delimiting
	  '''  whitespace. 
	  ''' </summary>
	  Public MustOverride Function handleKeyword(ByVal keyword As String) As Boolean
	  ''' <summary>
	  ''' Implemented by subclasses to interpret a keyword with a parameter. </summary>
	  '''  <param name="keyword">   The keyword, as with <code>handleKeyword(String)</code>. </param>
	  '''  <param name="parameter"> The parameter following the keyword.  </param>
	  Public MustOverride Function handleKeyword(ByVal keyword As String, ByVal parameter As Integer) As Boolean
	  ''' <summary>
	  ''' Implemented by subclasses to interpret text from the RTF stream. </summary>
	  Public MustOverride Sub handleText(ByVal text As String)
	  Public Overridable Sub handleText(ByVal ch As Char)
		  handleText(Convert.ToString(ch))
	  End Sub
	  ''' <summary>
	  ''' Implemented by subclasses to handle the contents of the \bin keyword. </summary>
	  Public MustOverride Sub handleBinaryBlob(ByVal data As SByte())
	  ''' <summary>
	  ''' Implemented by subclasses to react to an increase
	  '''  in the nesting level. 
	  ''' </summary>
	  Public MustOverride Sub begingroup()
	  ''' <summary>
	  ''' Implemented by subclasses to react to the end of a group. </summary>
	  Public MustOverride Sub endgroup()

	  ' table of non-text characters in rtf
	  Friend Shared ReadOnly rtfSpecialsTable As Boolean()
	  Shared Sub New()
		rtfSpecialsTable = noSpecialsTable.clone()
		rtfSpecialsTable(ControlChars.Lf) = True
		rtfSpecialsTable(ControlChars.Cr) = True
		rtfSpecialsTable(AscW("{"c)) = True
		rtfSpecialsTable(AscW("}"c)) = True
		rtfSpecialsTable(AscW("\"c)) = True
	  End Sub

	  Public Sub New()
		currentCharacters = New StringBuilder
		state = S_text
		pendingKeyword = Nothing
		level = 0
		'warnings = System.out;

		specialsTable = rtfSpecialsTable
	  End Sub

	  ' TODO: Handle wrapup at end of file correctly.

	  Public Overrides Sub writeSpecial(ByVal b As Integer)
		write(ChrW(b))
	  End Sub

		Protected Friend Overridable Sub warning(ByVal s As String)
			If warnings IsNot Nothing Then warnings.println(s)
		End Sub

	  Public Overrides Sub write(ByVal s As String)
		If state <> S_text Then
		  Dim index As Integer = 0
		  Dim length As Integer = s.Length
		  Do While index < length AndAlso state <> S_text
			write(s.Chars(index))
			index += 1
		  Loop

		  If index >= length Then Return

		  s = s.Substring(index)
		End If

		If currentCharacters.Length > 0 Then
		  currentCharacters.Append(s)
		Else
		  handleText(s)
		End If
	  End Sub

	  Public Overrides Sub write(ByVal ch As Char)
		Dim ok As Boolean

		Select Case state
		  Case S_text
			If ch = ControlChars.Lf OrElse ch = ControlChars.Cr Then
			  Exit Select ' unadorned newlines are ignored
			ElseIf ch = "{"c Then
			  If currentCharacters.Length > 0 Then
				handleText(currentCharacters.ToString())
				currentCharacters = New StringBuilder
			  End If
			  level += 1
			  begingroup()
			ElseIf ch = "}"c Then
			  If currentCharacters.Length > 0 Then
				handleText(currentCharacters.ToString())
				currentCharacters = New StringBuilder
			  End If
			  If level = 0 Then Throw New IOException("Too many close-groups in RTF text")
			  endgroup()
			  level -= 1
			ElseIf ch = "\"c Then
			  If currentCharacters.Length > 0 Then
				handleText(currentCharacters.ToString())
				currentCharacters = New StringBuilder
			  End If
			  state = S_backslashed
			Else
			  currentCharacters.Append(ch)
			End If
		  Case S_backslashed
			If ch = "'"c Then
			  state = S_aftertick
			  Exit Select
			End If
			If Not Char.IsLetter(ch) Then
			  Dim newstring As Char() = New Char(0){}
			  newstring(0) = ch
			  If Not handleKeyword(New String(newstring)) Then warning("Unknown keyword: " & newstring & " (" & AscW(ch) & ")")
			  state = S_text
			  pendingKeyword = Nothing
			  ' currentCharacters is already an empty stringBuffer 
			  Exit Select
			End If

			state = S_token
			' FALL THROUGH 
'JAVA TO VB CONVERTER TODO TASK: VB does not allow fall-through from a non-empty 'case':
		  Case S_token
			If Char.IsLetter(ch) Then
			  currentCharacters.Append(ch)
			Else
			  pendingKeyword = currentCharacters.ToString()
			  currentCharacters = New StringBuilder

			  ' Parameter following?
			  If Char.IsDigit(ch) OrElse (ch = "-"c) Then
				state = S_parameter
				currentCharacters.Append(ch)
			  Else
				ok = handleKeyword(pendingKeyword)
				If Not ok Then warning("Unknown keyword: " & pendingKeyword)
				pendingKeyword = Nothing
				state = S_text

				' Non-space delimiters get included in the text
				If Not Char.IsWhiteSpace(ch) Then write(ch)
			  End If
			End If
		  Case S_parameter
			If Char.IsDigit(ch) Then
			  currentCharacters.Append(ch)
			Else
			  ' TODO: Test correct behavior of \bin keyword 
			  If pendingKeyword.Equals("bin") Then ' magic layer-breaking kwd
				Dim parameter As Long = Convert.ToInt64(currentCharacters.ToString())
				pendingKeyword = Nothing
				state = S_inblob
				binaryBytesLeft = parameter
				If binaryBytesLeft > Integer.MaxValue Then
					binaryBuf = New ByteArrayOutputStream(Integer.MaxValue)
				Else
					binaryBuf = New ByteArrayOutputStream(CInt(binaryBytesLeft))
				End If
				savedSpecials = specialsTable
				specialsTable = allSpecialsTable
				Exit Select
			  End If

			  Dim parameter As Integer = Convert.ToInt32(currentCharacters.ToString())
			  ok = handleKeyword(pendingKeyword, parameter)
			  If Not ok Then warning("Unknown keyword: " & pendingKeyword & " (param " & currentCharacters & ")")
			  pendingKeyword = Nothing
			  currentCharacters = New StringBuilder
			  state = S_text

			  ' Delimiters here are interpreted as text too
			  If Not Char.IsWhiteSpace(ch) Then write(ch)
			End If
		  Case S_aftertick
			If Char.digit(ch, 16) = -1 Then
			  state = S_text
			Else
			  pendingCharacter = Char.digit(ch, 16)
			  state = S_aftertickc
			End If
		  Case S_aftertickc
			state = S_text
			If Char.digit(ch, 16) <> -1 Then
			  pendingCharacter = pendingCharacter * 16 + Char.digit(ch, 16)
			  ch = translationTable(pendingCharacter)
			  If AscW(ch) <> 0 Then handleText(ch)
			End If
		  Case S_inblob
			binaryBuf.write(ch)
			binaryBytesLeft -= 1
			If binaryBytesLeft = 0 Then
				state = S_text
				specialsTable = savedSpecials
				savedSpecials = Nothing
				handleBinaryBlob(binaryBuf.toByteArray())
				binaryBuf = Nothing
			End If
		End Select
	  End Sub

	  ''' <summary>
	  ''' Flushes any buffered but not yet written characters.
	  '''  Subclasses which override this method should call this
	  '''  method <em>before</em> flushing
	  '''  any of their own buffers. 
	  ''' </summary>
	  Public Overridable Sub flush()
		MyBase.flush()

		If state = S_text AndAlso currentCharacters.Length > 0 Then
		  handleText(currentCharacters.ToString())
		  currentCharacters = New StringBuilder
		End If
	  End Sub

	  ''' <summary>
	  ''' Closes the parser. Currently, this simply does a <code>flush()</code>,
	  '''  followed by some minimal consistency checks. 
	  ''' </summary>
	  Public Overridable Sub close()
		flush()

		If state <> S_text OrElse level > 0 Then
		  warning("Truncated RTF file.")

		  ' TODO: any sane way to handle termination in a non-S_text state? 
		  ' probably not 

	'       this will cause subclasses to behave more reasonably
	'         some of the time 
		  Do While level > 0
			  endgroup()
			  level -= 1
		  Loop
		End If

		MyBase.close()
	  End Sub

	End Class

End Namespace