Imports System.Text

'
' * Copyright (c) 2007, 2013, Oracle and/or its affiliates. All rights reserved.
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

Namespace javax.xml.bind

	''' <summary>
	''' Processes white space normalization.
	''' 
	''' @since 1.0
	''' </summary>
	Friend MustInherit Class WhiteSpaceProcessor

	' benchmarking (see test/src/ReplaceTest.java in the CVS Attic)
	' showed that this code is slower than the current code.
	'
	'    public static String replace(String text) {
	'        final int len = text.length();
	'        StringBuffer result = new StringBuffer(len);
	'
	'        for (int i = 0; i < len; i++) {
	'            char ch = text.charAt(i);
	'            if (isWhiteSpace(ch))
	'                result.append(' ');
	'            else
	'                result.append(ch);
	'        }
	'
	'        return result.toString();
	'    }

		Public Shared Function replace(ByVal text As String) As String
			Return replace(CType(text, CharSequence)).ToString()
		End Function

		''' <summary>
		''' @since 2.0
		''' </summary>
		Public Shared Function replace(ByVal text As CharSequence) As CharSequence
			Dim i As Integer=text.length()-1

			' look for the first whitespace char.
			Do While i>=0 AndAlso Not isWhiteSpaceExceptSpace(text.Chars(i))
				i -= 1
			Loop

			If i<0 Then Return text

			' we now know that we need to modify the text.
			' allocate a char array to do it.
			Dim buf As New StringBuilder(text)

			buf(i) = " "c
			i -= 1
			Do While i>=0
				If isWhiteSpaceExceptSpace(buf.Chars(i)) Then buf(i) = " "c
				i -= 1
			Loop

			Return New String(buf)
		End Function

		''' <summary>
		''' Equivalent of <seealso cref="String#trim()"/>.
		''' @since 2.0
		''' </summary>
		Public Shared Function trim(ByVal text As CharSequence) As CharSequence
			Dim len As Integer = text.length()
			Dim start As Integer = 0

			Do While start<len AndAlso isWhiteSpace(text.Chars(start))
				start += 1
			Loop

			Dim [end] As Integer = len-1

			Do While [end]>start AndAlso isWhiteSpace(text.Chars([end]))
				[end] -= 1
			Loop

			If start=0 AndAlso [end]=len-1 Then
				Return text ' no change
			Else
				Return text.subSequence(start,[end]+1)
			End If
		End Function

		Public Shared Function collapse(ByVal text As String) As String
			Return collapse(CType(text, CharSequence)).ToString()
		End Function

		''' <summary>
		''' This is usually the biggest processing bottleneck.
		''' 
		''' @since 2.0
		''' </summary>
		Public Shared Function collapse(ByVal text As CharSequence) As CharSequence
			Dim len As Integer = text.length()

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

			Return result
		End Function

		''' <summary>
		''' Returns true if the specified string is all whitespace.
		''' </summary>
		Public Shared Function isWhiteSpace(ByVal s As CharSequence) As Boolean
			For i As Integer = s.length()-1 To 0 Step -1
				If Not isWhiteSpace(s.Chars(i)) Then Return False
			Next i
			Return True
		End Function

		''' <summary>
		''' returns true if the specified char is a white space character. </summary>
		Public Shared Function isWhiteSpace(ByVal ch As Char) As Boolean
			' most of the characters are non-control characters.
			' so check that first to quickly return false for most of the cases.
			If AscW(ch)>&H20 Then Return False

			' other than we have to do four comparisons.
			Return AscW(ch) = &H9 OrElse AscW(ch) = &HA OrElse AscW(ch) = &HD OrElse AscW(ch) = &H20
		End Function

		''' <summary>
		''' Returns true if the specified char is a white space character
		''' but not 0x20.
		''' </summary>
		Protected Friend Shared Function isWhiteSpaceExceptSpace(ByVal ch As Char) As Boolean
			' most of the characters are non-control characters.
			' so check that first to quickly return false for most of the cases.
			If ch>=&H20 Then Return False

			' other than we have to do four comparisons.
			Return AscW(ch) = &H9 OrElse AscW(ch) = &HA OrElse AscW(ch) = &HD
		End Function
	End Class

End Namespace