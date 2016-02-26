Imports System
Imports System.Collections.Generic
Imports System.Text

'
' * Copyright (c) 1999, 2011, Oracle and/or its affiliates. All rights reserved.
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

Namespace javax.naming


	''' <summary>
	''' The implementation class for CompoundName and CompositeName.
	''' This class is package private.
	'''  
	''' @author Rosanna Lee
	''' @author Scott Seligman
	''' @author Aravindan Ranganathan
	''' @since 1.3
	''' </summary>

	Friend Class NameImpl
		Private Const LEFT_TO_RIGHT As SByte = 1
		Private Const RIGHT_TO_LEFT As SByte = 2
		Private Const FLAT As SByte = 0

		Private components As List(Of String)

		Private syntaxDirection As SByte = LEFT_TO_RIGHT
		Private syntaxSeparator As String = "/"
		Private syntaxSeparator2 As String = Nothing
		Private syntaxCaseInsensitive As Boolean = False
		Private syntaxTrimBlanks As Boolean = False
		Private syntaxEscape As String = "\"
		Private syntaxBeginQuote1 As String = """"
		Private syntaxEndQuote1 As String = """"
		Private syntaxBeginQuote2 As String = "'"
		Private syntaxEndQuote2 As String = "'"
		Private syntaxAvaSeparator As String = Nothing
		Private syntaxTypevalSeparator As String = Nothing

		' escapingStyle gives the method used at creation time for
		' quoting or escaping characters in the name.  It is set to the
		' first style of quote or escape encountered if and when the name
		' is parsed.
		Private Const STYLE_NONE As Integer = 0
		Private Const STYLE_QUOTE1 As Integer = 1
		Private Const STYLE_QUOTE2 As Integer = 2
		Private Const STYLE_ESCAPE As Integer = 3
		Private escapingStyle As Integer = STYLE_NONE

		' Returns true if "match" is not null, and n contains "match" at
		' position i.
		Private Function isA(ByVal n As String, ByVal i As Integer, ByVal match As String) As Boolean
			Return (match IsNot Nothing AndAlso n.StartsWith(match, i))
		End Function

		Private Function isMeta(ByVal n As String, ByVal i As Integer) As Boolean
			Return (isA(n, i, syntaxEscape) OrElse isA(n, i, syntaxBeginQuote1) OrElse isA(n, i, syntaxBeginQuote2) OrElse isSeparator(n, i))
		End Function

		Private Function isSeparator(ByVal n As String, ByVal i As Integer) As Boolean
			Return (isA(n, i, syntaxSeparator) OrElse isA(n, i, syntaxSeparator2))
		End Function

		Private Function skipSeparator(ByVal name As String, ByVal i As Integer) As Integer
			If isA(name, i, syntaxSeparator) Then
				i += syntaxSeparator.Length
			ElseIf isA(name, i, syntaxSeparator2) Then
				i += syntaxSeparator2.Length
			End If
			Return (i)
		End Function

		Private Function extractComp(ByVal name As String, ByVal i As Integer, ByVal len As Integer, ByVal comps As List(Of String)) As Integer
			Dim beginQuote As String
			Dim endQuote As String
			Dim start As Boolean = True
			Dim one As Boolean = False
			Dim answer As New StringBuilder(len)

			Do While i < len
				' handle quoted strings
				one = isA(name, i, syntaxBeginQuote1)
				If start AndAlso (one OrElse isA(name, i, syntaxBeginQuote2)) Then

					' record choice of quote chars being used
					beginQuote = If(one, syntaxBeginQuote1, syntaxBeginQuote2)
					endQuote = If(one, syntaxEndQuote1, syntaxEndQuote2)
					If escapingStyle = STYLE_NONE Then escapingStyle = If(one, STYLE_QUOTE1, STYLE_QUOTE2)

					' consume string until matching quote
					i += beginQuote.Length
					Do While ((i < len) AndAlso (Not name.StartsWith(endQuote, i)))
						' skip escape character if it is escaping ending quote
						' otherwise leave as is.
						If isA(name, i, syntaxEscape) AndAlso isA(name, i + syntaxEscape.Length, endQuote) Then i += syntaxEscape.Length
						answer.Append(name.Chars(i)) ' copy char
						i += 1
					Loop

					' no ending quote found
					If i >= len Then Throw New InvalidNameException(name & ": no close quote")
	'                      new Exception("no close quote");

					i += endQuote.Length

					' verify that end-quote occurs at separator or end of string
					If i = len OrElse isSeparator(name, i) Then Exit Do
	'              throw (new Exception(
					Throw (New InvalidNameException(name & ": close quote appears before end of component"))

				ElseIf isSeparator(name, i) Then
					Exit Do

				ElseIf isA(name, i, syntaxEscape) Then
					If isMeta(name, i + syntaxEscape.Length) Then
						' if escape precedes meta, consume escape and let
						' meta through
						i += syntaxEscape.Length
						If escapingStyle = STYLE_NONE Then escapingStyle = STYLE_ESCAPE
					ElseIf i + syntaxEscape.Length >= len Then
						Throw (New InvalidNameException(name & ": unescaped " & syntaxEscape & " at end of component"))
					End If
				Else
					one = isA(name, i+syntaxTypevalSeparator.Length, syntaxBeginQuote1)
					If isA(name, i, syntaxTypevalSeparator) AndAlso (one OrElse isA(name, i+syntaxTypevalSeparator.Length, syntaxBeginQuote2)) Then
						' Handle quote occurring after typeval separator
						beginQuote = If(one, syntaxBeginQuote1, syntaxBeginQuote2)
						endQuote = If(one, syntaxEndQuote1, syntaxEndQuote2)
        
						i += syntaxTypevalSeparator.Length
						answer.Append(syntaxTypevalSeparator+beginQuote) ' add back
        
						' consume string until matching quote
						i += beginQuote.Length
						Do While ((i < len) AndAlso (Not name.StartsWith(endQuote, i)))
							' skip escape character if it is escaping ending quote
							' otherwise leave as is.
							If isA(name, i, syntaxEscape) AndAlso isA(name, i + syntaxEscape.Length, endQuote) Then i += syntaxEscape.Length
							answer.Append(name.Chars(i)) ' copy char
							i += 1
						Loop
        
						' no ending quote found
						If i >= len Then Throw New InvalidNameException(name & ": typeval no close quote")
        
						i += endQuote.Length
						answer.Append(endQuote) ' add back
        
						' verify that end-quote occurs at separator or end of string
						If i = len OrElse isSeparator(name, i) Then Exit Do
						Throw (New InvalidNameException(name.Substring(i) & ": typeval close quote appears before end of component"))
					End If
					End If

				answer.Append(name.Chars(i))
				i += 1
				start = False
			Loop

			If syntaxDirection = RIGHT_TO_LEFT Then
				comps.Insert(0, answer.ToString())
			Else
				comps.Add(answer.ToString())
			End If
			Return i
		End Function

		Private Shared Function getBoolean(ByVal p As java.util.Properties, ByVal name As String) As Boolean
			Return toBoolean(p.getProperty(name))
		End Function

		Private Shared Function toBoolean(ByVal name As String) As Boolean
			Return ((name IsNot Nothing) AndAlso name.ToLower(java.util.Locale.ENGLISH).Equals("true"))
		End Function

		Private Sub recordNamingConvention(ByVal p As java.util.Properties)
			Dim syntaxDirectionStr As String = p.getProperty("jndi.syntax.direction", "flat")
			If syntaxDirectionStr.Equals("left_to_right") Then
				syntaxDirection = LEFT_TO_RIGHT
			ElseIf syntaxDirectionStr.Equals("right_to_left") Then
				syntaxDirection = RIGHT_TO_LEFT
			ElseIf syntaxDirectionStr.Equals("flat") Then
				syntaxDirection = FLAT
			Else
				Throw New System.ArgumentException(syntaxDirectionStr & "is not a valid value for the jndi.syntax.direction property")
			End If

			If syntaxDirection <> FLAT Then
				syntaxSeparator = p.getProperty("jndi.syntax.separator")
				syntaxSeparator2 = p.getProperty("jndi.syntax.separator2")
				If syntaxSeparator Is Nothing Then Throw New System.ArgumentException("jndi.syntax.separator property required for non-flat syntax")
			Else
				syntaxSeparator = Nothing
			End If
			syntaxEscape = p.getProperty("jndi.syntax.escape")

			syntaxCaseInsensitive = getBoolean(p, "jndi.syntax.ignorecase")
			syntaxTrimBlanks = getBoolean(p, "jndi.syntax.trimblanks")

			syntaxBeginQuote1 = p.getProperty("jndi.syntax.beginquote")
			syntaxEndQuote1 = p.getProperty("jndi.syntax.endquote")
			If syntaxEndQuote1 Is Nothing AndAlso syntaxBeginQuote1 IsNot Nothing Then
				syntaxEndQuote1 = syntaxBeginQuote1
			ElseIf syntaxBeginQuote1 Is Nothing AndAlso syntaxEndQuote1 IsNot Nothing Then
				syntaxBeginQuote1 = syntaxEndQuote1
			End If
			syntaxBeginQuote2 = p.getProperty("jndi.syntax.beginquote2")
			syntaxEndQuote2 = p.getProperty("jndi.syntax.endquote2")
			If syntaxEndQuote2 Is Nothing AndAlso syntaxBeginQuote2 IsNot Nothing Then
				syntaxEndQuote2 = syntaxBeginQuote2
			ElseIf syntaxBeginQuote2 Is Nothing AndAlso syntaxEndQuote2 IsNot Nothing Then
				syntaxBeginQuote2 = syntaxEndQuote2
			End If

			syntaxAvaSeparator = p.getProperty("jndi.syntax.separator.ava")
			syntaxTypevalSeparator = p.getProperty("jndi.syntax.separator.typeval")
		End Sub

		Friend Sub New(ByVal syntax As java.util.Properties)
			If syntax IsNot Nothing Then recordNamingConvention(syntax)
			components = New List(Of )
		End Sub

		Friend Sub New(ByVal syntax As java.util.Properties, ByVal n As String)
			Me.New(syntax)

			Dim rToL As Boolean = (syntaxDirection = RIGHT_TO_LEFT)
			Dim compsAllEmpty As Boolean = True
			Dim len As Integer = n.Length

			Dim i As Integer = 0
			Do While i < len
				i = extractComp(n, i, len, components)

				Dim comp As String = If(rToL, components(0), components(components.Count - 1))
				If comp.Length >= 1 Then compsAllEmpty = False

				If i < len Then
					i = skipSeparator(n, i)
					If (i = len) AndAlso (Not compsAllEmpty) Then
						' Trailing separator found.  Add an empty component.
						If rToL Then
							components.Insert(0, "")
						Else
							components.Add("")
						End If
					End If
				End If
			Loop
		End Sub

		Friend Sub New(ByVal syntax As java.util.Properties, ByVal comps As System.Collections.IEnumerator(Of String))
			Me.New(syntax)

			' %% comps could shrink in the middle.
'JAVA TO VB CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
			Do While comps.hasMoreElements()
'JAVA TO VB CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
				components.Add(comps.nextElement())
			Loop
		End Sub
	'
	'    // Determines whether this component needs any escaping.
	'    private final boolean escapingNeeded(String comp) {
	'        int len = comp.length();
	'        for (int i = 0; i < len; i++) {
	'            if (i == 0) {
	'                if (isA(comp, 0, syntaxBeginQuote1) ||
	'                    isA(comp, 0, syntaxBeginQuote2)) {
	'                    return (true);
	'                }
	'            }
	'            if (isSeparator(comp, i)) {
	'                return (true);
	'            }
	'            if (isA(comp, i, syntaxEscape)) {
	'                i += syntaxEscape.length();
	'                if (i >= len || isMeta(comp, i)) {
	'                    return (true);
	'                }
	'            }
	'        }
	'        return (false);
	'    }
	'
		Private Function stringifyComp(ByVal comp As String) As String
			Dim len As Integer = comp.Length
			Dim escapeSeparator As Boolean = False, escapeSeparator2 As Boolean = False
			Dim beginQuote As String = Nothing, endQuote As String = Nothing
			Dim strbuf As New StringBuilder(len)

			' determine whether there are any separators; if so escape
			' or quote them
			If syntaxSeparator IsNot Nothing AndAlso comp.IndexOf(syntaxSeparator) >= 0 Then
				If syntaxBeginQuote1 IsNot Nothing Then
					beginQuote = syntaxBeginQuote1
					endQuote = syntaxEndQuote1
				ElseIf syntaxBeginQuote2 IsNot Nothing Then
					beginQuote = syntaxBeginQuote2
					endQuote = syntaxEndQuote2
				ElseIf syntaxEscape IsNot Nothing Then
					escapeSeparator = True
				End If
			End If
			If syntaxSeparator2 IsNot Nothing AndAlso comp.IndexOf(syntaxSeparator2) >= 0 Then
				If syntaxBeginQuote1 IsNot Nothing Then
					If beginQuote Is Nothing Then
						beginQuote = syntaxBeginQuote1
						endQuote = syntaxEndQuote1
					End If
				ElseIf syntaxBeginQuote2 IsNot Nothing Then
					If beginQuote Is Nothing Then
						beginQuote = syntaxBeginQuote2
						endQuote = syntaxEndQuote2
					End If
				ElseIf syntaxEscape IsNot Nothing Then
					escapeSeparator2 = True
				End If
			End If

			' if quoting component,
			If beginQuote IsNot Nothing Then

				' start string off with opening quote
				strbuf = strbuf.Append(beginQuote)

				' component is being quoted, so we only need to worry about
				' escaping end quotes that occur in component
				Dim i As Integer = 0
				Do While i < len
					If comp.StartsWith(endQuote, i) Then
						' end-quotes must be escaped when inside a quoted string
						strbuf.Append(syntaxEscape).append(endQuote)
						i += endQuote.Length
					Else
						' no special treatment required
						strbuf.Append(comp.Chars(i))
						i += 1
					End If
				Loop

				' end with closing quote
				strbuf.Append(endQuote)

			Else

				' When component is not quoted, add escape for:
				' 1. leading quote
				' 2. an escape preceding any meta char
				' 3. an escape at the end of a component
				' 4. separator

				' go through characters in component and escape where necessary
				Dim start As Boolean = True
				Dim i As Integer = 0
				Do While i < len
					' leading quote must be escaped
					If start AndAlso isA(comp, i, syntaxBeginQuote1) Then
						strbuf.Append(syntaxEscape).append(syntaxBeginQuote1)
						i += syntaxBeginQuote1.Length
					ElseIf start AndAlso isA(comp, i, syntaxBeginQuote2) Then
						strbuf.Append(syntaxEscape).append(syntaxBeginQuote2)
						i += syntaxBeginQuote2.Length
					Else

					' Escape an escape preceding meta characters, or at end.
					' Other escapes pass through.
					If isA(comp, i, syntaxEscape) Then
						If i + syntaxEscape.Length >= len Then
							' escape an ending escape
							strbuf.Append(syntaxEscape)
						ElseIf isMeta(comp, i + syntaxEscape.Length) Then
							' escape meta strings
							strbuf.Append(syntaxEscape)
						End If
						strbuf.Append(syntaxEscape)
						i += syntaxEscape.Length
					Else

					' escape unescaped separator
					If escapeSeparator AndAlso comp.StartsWith(syntaxSeparator, i) Then
						' escape separator
						strbuf.Append(syntaxEscape).append(syntaxSeparator)
						i += syntaxSeparator.Length
					ElseIf escapeSeparator2 AndAlso comp.StartsWith(syntaxSeparator2, i) Then
						' escape separator2
						strbuf.Append(syntaxEscape).append(syntaxSeparator2)
						i += syntaxSeparator2.Length
					Else
						' no special treatment required
						strbuf.Append(comp.Chars(i))
						i += 1
					End If
					End If
					End If
					start = False
				Loop
			End If
			Return (strbuf.ToString())
		End Function

		Public Overrides Function ToString() As String
			Dim answer As New StringBuilder
			Dim comp As String
			Dim compsAllEmpty As Boolean = True
			Dim size As Integer = components.Count

			For i As Integer = 0 To size - 1
				If syntaxDirection = RIGHT_TO_LEFT Then
					comp = stringifyComp(components(size - 1 - i))
				Else
					comp = stringifyComp(components(i))
				End If
				If (i <> 0) AndAlso (syntaxSeparator IsNot Nothing) Then answer.Append(syntaxSeparator)
				If comp.Length >= 1 Then compsAllEmpty = False
				answer = answer.Append(comp)
			Next i
			If compsAllEmpty AndAlso (size >= 1) AndAlso (syntaxSeparator IsNot Nothing) Then answer = answer.Append(syntaxSeparator)
			Return (answer.ToString())
		End Function

		Public Overrides Function Equals(ByVal obj As Object) As Boolean
			If (obj IsNot Nothing) AndAlso (TypeOf obj Is NameImpl) Then
				Dim target As NameImpl = CType(obj, NameImpl)
				If target.size() = Me.size() Then
					Dim mycomps As System.Collections.IEnumerator(Of String) = all
					Dim comps As System.Collections.IEnumerator(Of String) = target.all
'JAVA TO VB CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
					Do While mycomps.hasMoreElements()
						' %% comps could shrink in the middle.
'JAVA TO VB CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
						Dim my As String = mycomps.nextElement()
'JAVA TO VB CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
						Dim his As String = comps.nextElement()
						If syntaxTrimBlanks Then
							my = my.Trim()
							his = his.Trim()
						End If
						If syntaxCaseInsensitive Then
							If Not(my.ToUpper() = his.ToUpper()) Then Return False
						Else
							If Not(my.Equals(his)) Then Return False
						End If
					Loop
					Return True
				End If
			End If
			Return False
		End Function

		''' <summary>
		''' Compares obj to this NameImpl to determine ordering.
		''' Takes into account syntactic properties such as
		''' elimination of blanks, case-ignore, etc, if relevant.
		'''  
		''' Note: using syntax of this NameImpl and ignoring
		''' that of comparison target.
		''' </summary>
		Public Overridable Function compareTo(ByVal obj As NameImpl) As Integer
			If Me Is obj Then Return 0

			Dim len1 As Integer = size()
			Dim len2 As Integer = obj.size()
			Dim n As Integer = Math.Min(len1, len2)

			Dim index1 As Integer = 0, index2 As Integer = 0

			Dim tempVar As Boolean = n <> 0
			n -= 1
			Do While tempVar
				Dim comp1 As String = [get](index1)
				index1 += 1
				Dim comp2 As String = obj.get(index2)
				index2 += 1

				' normalize according to syntax
				If syntaxTrimBlanks Then
					comp1 = comp1.Trim()
					comp2 = comp2.Trim()
				End If

				Dim local As Integer
				If syntaxCaseInsensitive Then
					local = comp1.compareToIgnoreCase(comp2)
				Else
					local = comp1.CompareTo(comp2)
				End If

				If local <> 0 Then Return local
				tempVar = n <> 0
				n -= 1
			Loop

			Return len1 - len2
		End Function

		Public Overridable Function size() As Integer
			Return (components.Count)
		End Function

		Public Overridable Property all As System.Collections.IEnumerator(Of String)
			Get
				Return components.elements()
			End Get
		End Property

		Public Overridable Function [get](ByVal posn As Integer) As String
			Return components(posn)
		End Function

		Public Overridable Function getPrefix(ByVal posn As Integer) As System.Collections.IEnumerator(Of String)
			If posn < 0 OrElse posn > size() Then Throw New System.IndexOutOfRangeException(posn)
			Return New NameImplEnumerator(components, 0, posn)
		End Function

		Public Overridable Function getSuffix(ByVal posn As Integer) As System.Collections.IEnumerator(Of String)
			Dim cnt As Integer = size()
			If posn < 0 OrElse posn > cnt Then Throw New System.IndexOutOfRangeException(posn)
			Return New NameImplEnumerator(components, posn, cnt)
		End Function

		Public Overridable Property empty As Boolean
			Get
				Return (components.Count = 0)
			End Get
		End Property

		Public Overridable Function startsWith(ByVal posn As Integer, ByVal prefix As System.Collections.IEnumerator(Of String)) As Boolean
			If posn < 0 OrElse posn > size() Then Return False
			Try
				Dim mycomps As System.Collections.IEnumerator(Of String) = getPrefix(posn)
'JAVA TO VB CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
				Do While mycomps.hasMoreElements()
'JAVA TO VB CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
					Dim my As String = mycomps.nextElement()
'JAVA TO VB CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
					Dim his As String = prefix.nextElement()
					If syntaxTrimBlanks Then
						my = my.Trim()
						his = his.Trim()
					End If
					If syntaxCaseInsensitive Then
						If Not(my.ToUpper() = his.ToUpper()) Then Return False
					Else
						If Not(my.Equals(his)) Then Return False
					End If
				Loop
			Catch e As java.util.NoSuchElementException
				Return False
			End Try
			Return True
		End Function

		Public Overridable Function endsWith(ByVal posn As Integer, ByVal suffix As System.Collections.IEnumerator(Of String)) As Boolean
			' posn is number of elements in suffix
			' startIndex is the starting position in this name
			' at which to start the comparison. It is calculated by
			' subtracting 'posn' from size()
			Dim startIndex As Integer = size() - posn
			If startIndex < 0 OrElse startIndex > size() Then Return False
			Try
				Dim mycomps As System.Collections.IEnumerator(Of String) = getSuffix(startIndex)
'JAVA TO VB CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
				Do While mycomps.hasMoreElements()
'JAVA TO VB CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
					Dim my As String = mycomps.nextElement()
'JAVA TO VB CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
					Dim his As String = suffix.nextElement()
					If syntaxTrimBlanks Then
						my = my.Trim()
						his = his.Trim()
					End If
					If syntaxCaseInsensitive Then
						If Not(my.ToUpper() = his.ToUpper()) Then Return False
					Else
						If Not(my.Equals(his)) Then Return False
					End If
				Loop
			Catch e As java.util.NoSuchElementException
				Return False
			End Try
			Return True
		End Function

		Public Overridable Function addAll(ByVal comps As System.Collections.IEnumerator(Of String)) As Boolean
			Dim added As Boolean = False
'JAVA TO VB CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
			Do While comps.hasMoreElements()
				Try
'JAVA TO VB CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
					Dim comp As String = comps.nextElement()
					If size() > 0 AndAlso syntaxDirection = FLAT Then Throw New InvalidNameException("A flat name can only have a single component")
					components.Add(comp)
					added = True
				Catch e As java.util.NoSuchElementException
					Exit Do ' "comps" has shrunk.
				End Try
			Loop
			Return added
		End Function

		Public Overridable Function addAll(ByVal posn As Integer, ByVal comps As System.Collections.IEnumerator(Of String)) As Boolean
			Dim added As Boolean = False
			Dim i As Integer = posn
'JAVA TO VB CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
			Do While comps.hasMoreElements()
				Try
'JAVA TO VB CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
					Dim comp As String = comps.nextElement()
					If size() > 0 AndAlso syntaxDirection = FLAT Then Throw New InvalidNameException("A flat name can only have a single component")
					components.Insert(i, comp)
					added = True
				Catch e As java.util.NoSuchElementException
					Exit Do ' "comps" has shrunk.
				End Try
				i += 1
			Loop
			Return added
		End Function

		Public Overridable Sub add(ByVal comp As String)
			If size() > 0 AndAlso syntaxDirection = FLAT Then Throw New InvalidNameException("A flat name can only have a single component")
			components.Add(comp)
		End Sub

		Public Overridable Sub add(ByVal posn As Integer, ByVal comp As String)
			If size() > 0 AndAlso syntaxDirection = FLAT Then Throw New InvalidNameException("A flat name can only zero or one component")
			components.Insert(posn, comp)
		End Sub

		Public Overridable Function remove(ByVal posn As Integer) As Object
			Dim r As Object = components(posn)
			components.RemoveAt(posn)
			Return r
		End Function

		Public Overrides Function GetHashCode() As Integer
			Dim hash As Integer = 0
			Dim e As System.Collections.IEnumerator(Of String) = all
'JAVA TO VB CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
			Do While e.hasMoreElements()
'JAVA TO VB CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
				Dim comp As String = e.nextElement()
				If syntaxTrimBlanks Then comp = comp.Trim()
				If syntaxCaseInsensitive Then comp = comp.ToLower(java.util.Locale.ENGLISH)

				hash += comp.GetHashCode()
			Loop
			Return hash
		End Function
	End Class

	Friend NotInheritable Class NameImplEnumerator
		Implements System.Collections.IEnumerator(Of String)

		Friend vector As List(Of String)
		Friend count As Integer
		Friend limit As Integer

		Friend Sub New(ByVal v As List(Of String), ByVal start As Integer, ByVal lim As Integer)
			vector = v
			count = start
			limit = lim
		End Sub

		Public Function hasMoreElements() As Boolean
			Return count < limit
		End Function

		Public Function nextElement() As String
			If count < limit Then
					Dim tempVar As Integer = count
					count += 1
					Return vector(tempVar)
			End If
			Throw New java.util.NoSuchElementException("NameImplEnumerator")
		End Function
	End Class

End Namespace