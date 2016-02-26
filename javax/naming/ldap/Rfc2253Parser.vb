Imports Microsoft.VisualBasic
Imports System.Collections.Generic

'
' * Copyright (c) 2003, 2011, Oracle and/or its affiliates. All rights reserved.
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

Namespace javax.naming.ldap



	'
	' * RFC2253Parser implements a recursive descent parser for a single DN.
	' 
	Friend NotInheritable Class Rfc2253Parser

			Private ReadOnly name As String ' DN being parsed
			Private ReadOnly chars As Char() ' characters in LDAP name being parsed
			Private ReadOnly len As Integer ' length of "chars"
			Private cur As Integer = 0 ' index of first unconsumed char in "chars"

	'        
	'         * Given an LDAP DN in string form, returns a parser for it.
	'         
			Friend Sub New(ByVal name As String)
				Me.name = name
				len = name.Length
				chars = name.ToCharArray()
			End Sub

	'        
	'         * Parses the DN, returning a List of its RDNs.
	'         
			' public List<Rdn> getDN() throws InvalidNameException {

			Friend Function parseDn() As IList(Of Rdn)
				cur = 0

				' ArrayList<Rdn> rdns =
				'  new ArrayList<Rdn>(len / 3 + 10);  // leave room for growth

				Dim rdns As New List(Of Rdn)(len \ 3 + 10) ' leave room for growth

				If len = 0 Then Return rdns

				rdns.Add(doParse(New Rdn))
				Do While cur < len
					If chars(cur) = ","c OrElse chars(cur) = ";"c Then
						cur += 1
						rdns.Insert(0, doParse(New Rdn))
					Else
						Throw New javax.naming.InvalidNameException("Invalid name: " & name)
					End If
				Loop
				Return rdns
			End Function

	'        
	'         * Parses the DN, if it is known to contain a single RDN.
	'         
			Friend Function parseRdn() As Rdn
				Return parseRdn(New Rdn)
			End Function

	'        
	'         * Parses the DN, if it is known to contain a single RDN.
	'         
			Friend Function parseRdn(ByVal ___rdn As Rdn) As Rdn
				___rdn = doParse(___rdn)
				If cur < len Then Throw New javax.naming.InvalidNameException("Invalid RDN: " & name)
				Return ___rdn
			End Function

	'        
	'         * Parses the next RDN and returns it.  Throws an exception if
	'         * none is found.  Leading and trailing whitespace is consumed.
	'         
			 Private Function doParse(ByVal ___rdn As Rdn) As Rdn

				Do While cur < len
					consumeWhitespace()
					Dim attrType As String = parseAttrType()
					consumeWhitespace()
					If cur >= len OrElse chars(cur) <> "="c Then Throw New javax.naming.InvalidNameException("Invalid name: " & name)
					cur += 1 ' consume '='
					consumeWhitespace()
					Dim value As String = parseAttrValue()
					consumeWhitespace()

					___rdn.put(attrType, Rdn.unescapeValue(value))
					If cur >= len OrElse chars(cur) <> "+"c Then Exit Do
					cur += 1 ' consume '+'
				Loop
				___rdn.sort()
				Return ___rdn
			 End Function

	'        
	'         * Returns the attribute type that begins at the next unconsumed
	'         * char.  No leading whitespace is expected.
	'         * This routine is more generous than RFC 2253.  It accepts
	'         * attribute types composed of any nonempty combination of Unicode
	'         * letters, Unicode digits, '.', '-', and internal space characters.
	'         
			Private Function parseAttrType() As String

				Dim beg As Integer = cur
				Do While cur < len
					Dim c As Char = chars(cur)
					If Char.IsLetterOrDigit(c) OrElse c = "."c OrElse c = "-"c OrElse c = " "c Then
						cur += 1
					Else
						Exit Do
					End If
				Loop
				' Back out any trailing spaces.
				Do While (cur > beg) AndAlso (chars(cur - 1) = " "c)
					cur -= 1
				Loop

				If beg = cur Then Throw New javax.naming.InvalidNameException("Invalid name: " & name)
				Return New String(chars, beg, cur - beg)
			End Function

	'        
	'         * Returns the attribute value that begins at the next unconsumed
	'         * char.  No leading whitespace is expected.
	'         
			Private Function parseAttrValue() As String

				If cur < len AndAlso chars(cur) = "#"c Then
					Return parseBinaryAttrValue()
				ElseIf cur < len AndAlso chars(cur) = """"c Then
					Return parseQuotedAttrValue()
				Else
					Return parseStringAttrValue()
				End If
			End Function

			Private Function parseBinaryAttrValue() As String
				Dim beg As Integer = cur
				cur += 1 ' consume '#'
				Do While (cur < len) AndAlso Char.IsLetterOrDigit(chars(cur))
					cur += 1
				Loop
				Return New String(chars, beg, cur - beg)
			End Function

			Private Function parseQuotedAttrValue() As String

				Dim beg As Integer = cur
				cur += 1 ' consume '"'

				Do While (cur < len) AndAlso chars(cur) <> """"c
					If chars(cur) = "\"c Then cur += 1 ' consume backslash, then what follows
					cur += 1
				Loop
				If cur >= len Then ' no closing quote Throw New javax.naming.InvalidNameException("Invalid name: " & name)
				cur += 1 ' consume closing quote

				Return New String(chars, beg, cur - beg)
			End Function

			Private Function parseStringAttrValue() As String

				Dim beg As Integer = cur
				Dim esc As Integer = -1 ' index of the most recently escaped character

				Do While (cur < len) AndAlso Not atTerminator()
					If chars(cur) = "\"c Then
						cur += 1 ' consume backslash, then what follows
						esc = cur
					End If
					cur += 1
				Loop
				If cur > len Then ' 'twas backslash followed by nothing Throw New javax.naming.InvalidNameException("Invalid name: " & name)

				' Trim off (unescaped) trailing whitespace.
				Dim [end] As Integer
				For [end] = cur To beg + 1 Step -1
					If (Not isWhitespace(chars([end] - 1))) OrElse (esc = [end] - 1) Then Exit For
				Next [end]
				Return New String(chars, beg, [end] - beg)
			End Function

			Private Sub consumeWhitespace()
				Do While (cur < len) AndAlso isWhitespace(chars(cur))
					cur += 1
				Loop
			End Sub

	'        
	'         * Returns true if next unconsumed character is one that terminates
	'         * a string attribute value.
	'         
			Private Function atTerminator() As Boolean
				Return (cur < len AndAlso (chars(cur) = ","c OrElse chars(cur) = ";"c OrElse chars(cur) = "+"c))
			End Function

	'        
	'         * Best guess as to what RFC 2253 means by "whitespace".
	'         
			Private Shared Function isWhitespace(ByVal c As Char) As Boolean
				Return (c = " "c OrElse c = ControlChars.Cr)
			End Function
	End Class

End Namespace