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
	''' <seealso cref="XmlAdapter"/> to handle <tt>xs:normalizedString</tt>.
	''' 
	''' <p>
	''' Replaces any tab, CR, and LF by a whitespace character ' ',
	''' as specified in <a href="http://www.w3.org/TR/xmlschema-2/#rf-whiteSpace">the whitespace facet 'replace'</a>
	''' 
	''' @author Kohsuke Kawaguchi, Martin Grebac
	''' @since JAXB 2.0
	''' </summary>
	Public NotInheritable Class NormalizedStringAdapter
		Inherits XmlAdapter(Of String, String)

		''' <summary>
		''' Replace any tab, CR, and LF by a whitespace character ' ',
		''' as specified in <a href="http://www.w3.org/TR/xmlschema-2/#rf-whiteSpace">the whitespace facet 'replace'</a>
		''' </summary>
		Public Function unmarshal(ByVal text As String) As String
			If text Is Nothing Then ' be defensive Return Nothing

			Dim i As Integer=text.Length-1

			' look for the first whitespace char.
			Do While i>=0 AndAlso Not isWhiteSpaceExceptSpace(text.Chars(i))
				i -= 1
			Loop

			If i<0 Then Return text

			' we now know that we need to modify the text.
			' allocate a char array to do it.
			Dim buf As Char() = text.ToCharArray()

			buf(i) = " "c
			i -= 1
			Do While i>=0
				If isWhiteSpaceExceptSpace(buf(i)) Then buf(i) = " "c
				i -= 1
			Loop

			Return New String(buf)
		End Function

		''' <summary>
		''' No-op.
		''' 
		''' Just return the same string given as the parameter.
		''' </summary>
			Public Function marshal(ByVal s As String) As String
				Return s
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