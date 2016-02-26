Imports System.Text

'
' * Copyright (c) 2004, 2013, Oracle and/or its affiliates. All rights reserved.
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

Namespace javax.xml.bind.annotation.adapters



	''' <summary>
	''' Built-in <seealso cref="XmlAdapter"/> to handle <tt>xs:token</tt> and its derived types.
	''' 
	''' <p>
	''' This adapter removes leading and trailing whitespaces, then truncate any
	''' sequnce of tab, CR, LF, and SP by a single whitespace character ' '.
	''' 
	''' @author Kohsuke Kawaguchi
	''' @since JAXB 2.0
	''' </summary>
	Public Class CollapsedStringAdapter
		Inherits XmlAdapter(Of String, String)

		''' <summary>
		''' Removes leading and trailing whitespaces of the string
		''' given as the parameter, then truncate any
		''' sequnce of tab, CR, LF, and SP by a single whitespace character ' '.
		''' </summary>
		Public Overridable Function unmarshal(ByVal text As String) As String
			If text Is Nothing Then ' be defensive Return Nothing

			Dim len As Integer = text.Length

			' most of the texts are already in the collapsed form.
			' so look for the first whitespace in the hope that we will
			' never see it.
			Dim s As Integer=0
			Do While s<len
				If isWhiteSpace(text.Chars(s)) Then Exit Do
				s += 1
			Loop
			If s=len Then Return text

			' we now know that the input contains spaces.
			' let's sit down and do the collapsing normally.

			Dim result As New StringBuilder(len) 'allocate enough size to avoid re-allocation

			If s<>0 Then
				For i As Integer = 0 To s - 1
					result.Append(text.Chars(i))
				Next i
				result.Append(" "c)
			End If

			Dim inStripMode As Boolean = True
			For i As Integer = s+1 To len - 1
				Dim ch As Char = text.Chars(i)
				Dim b As Boolean = isWhiteSpace(ch)
				If inStripMode AndAlso b Then Continue For ' skip this character

				inStripMode = b
				If inStripMode Then
					result.Append(" "c)
				Else
					result.Append(ch)
				End If
			Next i

			' remove trailing whitespaces
			len = result.Length
			If len > 0 AndAlso result.Chars(len - 1) = " "c Then result.Length = len - 1
			' whitespaces are already collapsed,
			' so all we have to do is to remove the last one character
			' if it's a whitespace.

			Return result.ToString()
		End Function

		''' <summary>
		''' No-op.
		''' 
		''' Just return the same string given as the parameter.
		''' </summary>
		Public Overridable Function marshal(ByVal s As String) As String
			Return s
		End Function


		''' <summary>
		''' returns true if the specified char is a white space character. </summary>
		Protected Friend Shared Function isWhiteSpace(ByVal ch As Char) As Boolean
			' most of the characters are non-control characters.
			' so check that first to quickly return false for most of the cases.
			If AscW(ch)>&H20 Then Return False

			' other than we have to do four comparisons.
			Return AscW(ch) = &H9 OrElse AscW(ch) = &HA OrElse AscW(ch) = &HD OrElse AscW(ch) = &H20
		End Function
	End Class

End Namespace