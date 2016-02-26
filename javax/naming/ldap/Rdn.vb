Imports Microsoft.VisualBasic
Imports System
Imports System.Collections.Generic
Imports System.Text

'
' * Copyright (c) 2003, 2013, Oracle and/or its affiliates. All rights reserved.
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




	''' <summary>
	''' This class represents a relative distinguished name, or RDN, which is a
	''' component of a distinguished name as specified by
	''' <a href="http://www.ietf.org/rfc/rfc2253.txt">RFC 2253</a>.
	''' An example of an RDN is "OU=Sales+CN=J.Smith". In this example,
	''' the RDN consist of multiple attribute type/value pairs. The
	''' RDN is parsed as described in the class description for
	''' <seealso cref="javax.naming.ldap.LdapName <tt>LdapName</tt>"/>.
	''' <p>
	''' The Rdn class represents an RDN as attribute type/value mappings,
	''' which can be viewed using
	''' <seealso cref="javax.naming.directory.Attributes Attributes"/>.
	''' In addition, it contains convenience methods that allow easy retrieval
	''' of type and value when the Rdn consist of a single type/value pair,
	''' which is how it appears in a typical usage.
	''' It also contains helper methods that allow escaping of the unformatted
	''' attribute value and unescaping of the value formatted according to the
	''' escaping syntax defined in RFC2253. For methods that take or return
	''' attribute value as an Object, the value is either a String
	''' (in unescaped form) or a byte array.
	''' <p>
	''' <code>Rdn</code> will properly parse all valid RDNs, but
	''' does not attempt to detect all possible violations when parsing
	''' invalid RDNs. It is "generous" in accepting invalid RDNs.
	''' The "validity" of a name is determined ultimately when it
	''' is supplied to an LDAP server, which may accept or
	''' reject the name based on factors such as its schema information
	''' and interoperability considerations.
	''' 
	''' <p>
	''' The following code example shows how to construct an Rdn using the
	''' constructor that takes type and value as arguments:
	''' <pre>
	'''      Rdn rdn = new Rdn("cn", "Juicy, Fruit");
	'''      System.out.println(rdn.toString());
	''' </pre>
	''' The last line will print <tt>cn=Juicy\, Fruit</tt>. The
	''' <seealso cref="#unescapeValue(String) <tt>unescapeValue()</tt>"/> method can be
	''' used to unescape the escaped comma resulting in the original
	''' value <tt>"Juicy, Fruit"</tt>. The {@link #escapeValue(Object)
	''' <tt>escapeValue()</tt>} method adds the escape back preceding the comma.
	''' <p>
	''' This class can be instantiated by a string representation
	''' of the RDN defined in RFC 2253 as shown in the following code example:
	''' <pre>
	'''      Rdn rdn = new Rdn("cn=Juicy\\, Fruit");
	'''      System.out.println(rdn.toString());
	''' </pre>
	''' The last line will print <tt>cn=Juicy\, Fruit</tt>.
	''' <p>
	''' Concurrent multithreaded read-only access of an instance of
	''' <tt>Rdn</tt> need not be synchronized.
	''' <p>
	''' Unless otherwise noted, the behavior of passing a null argument
	''' to a constructor or method in this class will cause NullPointerException
	''' to be thrown.
	''' 
	''' @since 1.5
	''' </summary>

	<Serializable> _
	Public Class Rdn
		Implements IComparable(Of Object)

		<NonSerialized> _
		Private entries As List(Of RdnEntry)

		' The common case.
		Private Const DEFAULT_SIZE As Integer = 1

		Private Const serialVersionUID As Long = -5994465067210009656L

		''' <summary>
		''' Constructs an Rdn from the given attribute set. See
		''' <seealso cref="javax.naming.directory.Attributes Attributes"/>.
		''' <p>
		''' The string attribute values are not interpreted as
		''' <a href="http://www.ietf.org/rfc/rfc2253.txt">RFC 2253</a>
		''' formatted RDN strings. That is, the values are used
		''' literally (not parsed) and assumed to be unescaped.
		''' </summary>
		''' <param name="attrSet"> The non-null and non-empty attributes containing
		''' type/value mappings. </param>
		''' <exception cref="InvalidNameException"> If contents of <tt>attrSet</tt> cannot
		'''          be used to construct a valid RDN. </exception>
		Public Sub New(ByVal attrSet As javax.naming.directory.Attributes)
			If attrSet.size() = 0 Then Throw New javax.naming.InvalidNameException("Attributes cannot be empty")
			entries = New List(Of )(attrSet.size())
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
			Dim attrs As javax.naming.NamingEnumeration(Of ? As javax.naming.directory.Attribute) = attrSet.all
			Try
				Dim nEntries As Integer = 0
				Do While attrs.hasMore()
					Dim entry As New RdnEntry
					Dim attr As javax.naming.directory.Attribute = attrs.next()
					entry.type = attr.iD
					entry.value = attr.get()
					entries.Insert(nEntries, entry)
					nEntries += 1
				Loop
			Catch e As javax.naming.NamingException
				Dim e2 As New javax.naming.InvalidNameException(e.Message)
				e2.initCause(e)
				Throw e2
			End Try
			sort() ' arrange entries for comparison
		End Sub

		''' <summary>
		''' Constructs an Rdn from the given string.
		''' This constructor takes a string formatted according to the rules
		''' defined in <a href="http://www.ietf.org/rfc/rfc2253.txt">RFC 2253</a>
		''' and described in the class description for
		''' <seealso cref="javax.naming.ldap.LdapName"/>.
		''' </summary>
		''' <param name="rdnString"> The non-null and non-empty RFC2253 formatted string. </param>
		''' <exception cref="InvalidNameException"> If a syntax error occurs during
		'''                  parsing of the rdnString. </exception>
		Public Sub New(ByVal rdnString As String)
			entries = New List(Of )(DEFAULT_SIZE)
			CType(New Rfc2253Parser(rdnString), Rfc2253Parser).parseRdn(Me)
		End Sub

		''' <summary>
		''' Constructs an Rdn from the given <tt>rdn</tt>.
		''' The contents of the <tt>rdn</tt> are simply copied into the newly
		''' created Rdn. </summary>
		''' <param name="rdn"> The non-null Rdn to be copied. </param>
		Public Sub New(ByVal ___rdn As Rdn)
			entries = New List(Of )(___rdn.entries.Count)
			entries.AddRange(___rdn.entries)
		End Sub

		''' <summary>
		''' Constructs an Rdn from the given attribute type and
		''' value.
		''' The string attribute values are not interpreted as
		''' <a href="http://www.ietf.org/rfc/rfc2253.txt">RFC 2253</a>
		''' formatted RDN strings. That is, the values are used
		''' literally (not parsed) and assumed to be unescaped.
		''' </summary>
		''' <param name="type"> The non-null and non-empty string attribute type. </param>
		''' <param name="value"> The non-null and non-empty attribute value. </param>
		''' <exception cref="InvalidNameException"> If type/value cannot be used to
		'''                  construct a valid RDN. </exception>
		''' <seealso cref= #toString() </seealso>
		Public Sub New(ByVal type As String, ByVal value As Object)
			If value Is Nothing Then Throw New NullPointerException("Cannot set value to null")
			If type.Equals("") OrElse isEmptyValue(value) Then Throw New javax.naming.InvalidNameException("type or value cannot be empty, type:" & type & " value:" & value)
			entries = New List(Of )(DEFAULT_SIZE)
			put(type, value)
		End Sub

		Private Function isEmptyValue(ByVal val As Object) As Boolean
			Return ((TypeOf val Is String) AndAlso val.Equals("")) OrElse ((TypeOf val Is SByte()) AndAlso (CType(val, SByte()).Length = 0))
		End Function

		' An empty constructor used by the parser
		Friend Sub New()
			entries = New List(Of )(DEFAULT_SIZE)
		End Sub

	'    
	'     * Adds the given attribute type and value to this Rdn.
	'     * The string attribute values are not interpreted as
	'     * <a href="http://www.ietf.org/rfc/rfc2253.txt">RFC 2253</a>
	'     * formatted RDN strings. That is the values are used
	'     * literally (not parsed) and assumed to be unescaped.
	'     *
	'     * @param type The non-null and non-empty string attribute type.
	'     * @param value The non-null and non-empty attribute value.
	'     * @return The updated Rdn, not a new one. Cannot be null.
	'     * @see #toString()
	'     
		Friend Overridable Function put(ByVal type As String, ByVal value As Object) As Rdn

			' create new Entry
			Dim newEntry As New RdnEntry
			newEntry.type = type
			If TypeOf value Is SByte() Then ' clone the byte array
				newEntry.value = CType(value, SByte()).clone()
			Else
				newEntry.value = value
			End If
			entries.Add(newEntry)
			Return Me
		End Function

		Friend Overridable Sub sort()
			If entries.Count > 1 Then java.util.Collections.sort(entries)
		End Sub

		''' <summary>
		''' Retrieves one of this Rdn's value.
		''' This is a convenience method for obtaining the value,
		''' when the RDN contains a single type and value mapping,
		''' which is the common RDN usage.
		''' <p>
		''' For a multi-valued RDN, this method returns value corresponding
		''' to the type returned by <seealso cref="#getType() getType()"/> method.
		''' </summary>
		''' <returns> The non-null attribute value. </returns>
		Public Overridable Property value As Object
			Get
				Return entries(0).value
			End Get
		End Property

		''' <summary>
		''' Retrieves one of this Rdn's type.
		''' This is a convenience method for obtaining the type,
		''' when the RDN contains a single type and value mapping,
		''' which is the common RDN usage.
		''' <p>
		''' For a multi-valued RDN, the type/value pairs have
		''' no specific order defined on them. In that case, this method
		''' returns type of one of the type/value pairs.
		''' The <seealso cref="#getValue() getValue()"/> method returns the
		''' value corresponding to the type returned by this method.
		''' </summary>
		''' <returns> The non-null attribute type. </returns>
		Public Overridable Property type As String
			Get
				Return entries(0).type
			End Get
		End Property

		''' <summary>
		''' Returns this Rdn as a string represented in a format defined by
		''' <a href="http://www.ietf.org/rfc/rfc2253.txt">RFC 2253</a> and described
		''' in the class description for <seealso cref="javax.naming.ldap.LdapName LdapName"/>.
		''' </summary>
		''' <returns> The string representation of the Rdn. </returns>
		Public Overrides Function ToString() As String
			Dim builder As New StringBuilder
			Dim size As Integer = entries.Count
			If size > 0 Then builder.Append(entries(0))
			For [next] As Integer = 1 To size - 1
				builder.Append("+"c)
				builder.Append(entries([next]))
			Next [next]
			Return builder.ToString()
		End Function

		''' <summary>
		''' Compares this Rdn with the specified Object for order.
		''' Returns a negative integer, zero, or a positive integer as this
		''' Rdn is less than, equal to, or greater than the given Object.
		''' <p>
		''' If obj is null or not an instance of Rdn, ClassCastException
		''' is thrown.
		''' <p>
		''' The attribute type and value pairs of the RDNs are lined up
		''' against each other and compared lexicographically. The order of
		''' components in multi-valued Rdns (such as "ou=Sales+cn=Bob") is not
		''' significant.
		''' </summary>
		''' <param name="obj"> The non-null object to compare against. </param>
		''' <returns>  A negative integer, zero, or a positive integer as this Rdn
		'''          is less than, equal to, or greater than the given Object. </returns>
		''' <exception cref="ClassCastException"> if obj is null or not a Rdn. </exception>
		Public Overridable Function compareTo(ByVal obj As Object) As Integer
			If Not(TypeOf obj Is Rdn) Then Throw New ClassCastException("The obj is not a Rdn")
			If obj Is Me Then Return 0
			Dim that As Rdn = CType(obj, Rdn)
			Dim minSize As Integer = Math.Min(entries.Count, that.entries.Count)
			For i As Integer = 0 To minSize - 1

				' Compare a single pair of type/value pairs.
				Dim diff As Integer = entries(i).CompareTo(that.entries(i))
				If diff <> 0 Then Return diff
			Next i
			Return (entries.Count - that.entries.Count) ' longer RDN wins
		End Function

		''' <summary>
		''' Compares the specified Object with this Rdn for equality.
		''' Returns true if the given object is also a Rdn and the two Rdns
		''' represent the same attribute type and value mappings. The order of
		''' components in multi-valued Rdns (such as "ou=Sales+cn=Bob") is not
		''' significant.
		''' <p>
		''' Type and value equality matching is done as below:
		''' <ul>
		''' <li> The types are compared for equality with their case ignored.
		''' <li> String values with different but equivalent usage of quoting,
		''' escaping, or UTF8-hex-encoding are considered equal.
		''' The case of the values is ignored during the comparison.
		''' </ul>
		''' <p>
		''' If obj is null or not an instance of Rdn, false is returned.
		''' <p> </summary>
		''' <param name="obj"> object to be compared for equality with this Rdn. </param>
		''' <returns> true if the specified object is equal to this Rdn. </returns>
		''' <seealso cref= #hashCode() </seealso>
		Public Overrides Function Equals(ByVal obj As Object) As Boolean
			If obj Is Me Then Return True
			If Not(TypeOf obj Is Rdn) Then Return False
			Dim that As Rdn = CType(obj, Rdn)
			If entries.Count <> that.size() Then Return False
			For i As Integer = 0 To entries.Count - 1
				If Not entries(i).Equals(that.entries(i)) Then Return False
			Next i
			Return True
		End Function

		''' <summary>
		''' Returns the hash code of this RDN. Two RDNs that are
		''' equal (according to the equals method) will have the same
		''' hash code.
		''' </summary>
		''' <returns> An int representing the hash code of this Rdn. </returns>
		''' <seealso cref= #equals </seealso>
		Public Overrides Function GetHashCode() As Integer

			' Sum up the hash codes of the components.
			Dim hash As Integer = 0

			' For each type/value pair...
			For i As Integer = 0 To entries.Count - 1
				hash += entries(i).GetHashCode()
			Next i
			Return hash
		End Function

		''' <summary>
		''' Retrieves the <seealso cref="javax.naming.directory.Attributes Attributes"/>
		''' view of the type/value mappings contained in this Rdn.
		''' </summary>
		''' <returns>  The non-null attributes containing the type/value
		'''          mappings of this Rdn. </returns>
		Public Overridable Function toAttributes() As javax.naming.directory.Attributes
			Dim attrs As javax.naming.directory.Attributes = New javax.naming.directory.BasicAttributes(True)
			For i As Integer = 0 To entries.Count - 1
				Dim entry As RdnEntry = entries(i)
				Dim attr As javax.naming.directory.Attribute = attrs.put(entry.type, entry.value)
				If attr IsNot Nothing Then
					attr.add(entry.value)
					attrs.put(attr)
				End If
			Next i
			Return attrs
		End Function


		Private Class RdnEntry
			Implements IComparable(Of RdnEntry)

			Private type As String
			Private value As Object

			' If non-null, a cannonical representation of the value suitable
			' for comparison using String.compareTo()
			Private comparable As String = Nothing

			Friend Overridable Property type As String
				Get
					Return type
				End Get
			End Property

			Friend Overridable Property value As Object
				Get
					Return value
				End Get
			End Property

			Public Overridable Function compareTo(ByVal that As RdnEntry) As Integer
				Dim diff As Integer = type.compareToIgnoreCase(that.type)
				If diff <> 0 Then Return diff
				If value.Equals(that.value) Then ' try shortcut Return 0
				Return valueComparable.CompareTo(that.valueComparable)
			End Function

			Public Overrides Function Equals(ByVal obj As Object) As Boolean
				If obj Is Me Then Return True
				If Not(TypeOf obj Is RdnEntry) Then Return False

				' Any change here must be reflected in hashCode()
				Dim that As RdnEntry = CType(obj, RdnEntry)
				Return (type.ToUpper() = that.type.ToUpper()) AndAlso (valueComparable.Equals(that.valueComparable))
			End Function

			Public Overrides Function GetHashCode() As Integer
				Return (type.ToUpper(java.util.Locale.ENGLISH).GetHashCode() + valueComparable.GetHashCode())
			End Function

			Public Overrides Function ToString() As String
				Return type & "=" & escapeValue(value)
			End Function

			Private Property valueComparable As String
				Get
					If comparable IsNot Nothing Then Return comparable ' return cached result
    
					' cache result
					If TypeOf value Is SByte() Then
						comparable = escapeBinaryValue(CType(value, SByte()))
					Else
						comparable = CStr(value).ToUpper(java.util.Locale.ENGLISH)
					End If
					Return comparable
				End Get
			End Property
		End Class

		''' <summary>
		''' Retrieves the number of attribute type/value pairs in this Rdn. </summary>
		''' <returns> The non-negative number of type/value pairs in this Rdn. </returns>
		Public Overridable Function size() As Integer
			Return entries.Count
		End Function

		''' <summary>
		''' Given the value of an attribute, returns a string escaped according
		''' to the rules specified in
		''' <a href="http://www.ietf.org/rfc/rfc2253.txt">RFC 2253</a>.
		''' <p>
		''' For example, if the val is "Sue, Grabbit and Runn", the escaped
		''' value returned by this method is "Sue\, Grabbit and Runn".
		''' <p>
		''' A string value is represented as a String and binary value
		''' as a byte array.
		''' </summary>
		''' <param name="val"> The non-null object to be escaped. </param>
		''' <returns> Escaped string value. </returns>
		''' <exception cref="ClassCastException"> if val is is not a String or byte array. </exception>
		Public Shared Function escapeValue(ByVal val As Object) As String
			Return If(TypeOf val Is SByte(), escapeBinaryValue(CType(val, SByte())), escapeStringValue(CStr(val)))
		End Function

	'    
	'     * Given the value of a string-valued attribute, returns a
	'     * string suitable for inclusion in a DN.  This is accomplished by
	'     * using backslash (\) to escape the following characters:
	'     *  leading and trailing whitespace
	'     *  , = + < > # ; " \
	'     
		Private Shared ReadOnly escapees As String = ",=+<>#;""\"

		Private Shared Function escapeStringValue(ByVal val As String) As String

				Dim chars As Char() = val.ToCharArray()
				Dim builder As New StringBuilder(2 * val.Length)

				' Find leading and trailing whitespace.
				Dim lead As Integer ' index of first char that is not leading whitespace
				For lead = 0 To chars.Length - 1
					If Not isWhitespace(chars(lead)) Then Exit For
				Next lead
				Dim trail As Integer ' index of last char that is not trailing whitespace
				For trail = chars.Length - 1 To 0 Step -1
					If Not isWhitespace(chars(trail)) Then Exit For
				Next trail

				For i As Integer = 0 To chars.Length - 1
					Dim c As Char = chars(i)
					If (i < lead) OrElse (i > trail) OrElse (escapees.IndexOf(c) >= 0) Then builder.Append("\"c)
					builder.Append(c)
				Next i
				Return builder.ToString()
		End Function

	'    
	'     * Given the value of a binary attribute, returns a string
	'     * suitable for inclusion in a DN (such as "#CEB1DF80").
	'     * TBD: This method should actually generate the ber encoding
	'     * of the binary value
	'     
		Private Shared Function escapeBinaryValue(ByVal val As SByte()) As String

			Dim builder As New StringBuilder(1 + 2 * val.Length)
			builder.Append("#")

			For i As Integer = 0 To val.Length - 1
				Dim b As SByte = val(i)
				builder.Append(Char.forDigit(&HF And (CInt(CUInt(b) >> 4)), 16))
				builder.Append(Char.forDigit(&HF And b, 16))
			Next i
			Return builder.ToString()
		End Function

		''' <summary>
		''' Given an attribute value string formated according to the rules
		''' specified in
		''' <a href="http://www.ietf.org/rfc/rfc2253.txt">RFC 2253</a>,
		''' returns the unformated value.  Escapes and quotes are
		''' stripped away, and hex-encoded UTF-8 is converted to equivalent
		''' UTF-16 characters. Returns a string value as a String, and a
		''' binary value as a byte array.
		''' <p>
		''' Legal and illegal values are defined in RFC 2253.
		''' This method is generous in accepting the values and does not
		''' catch all illegal values.
		''' Therefore, passing in an illegal value might not necessarily
		''' trigger an <tt>IllegalArgumentException</tt>.
		''' </summary>
		''' <param name="val">     The non-null string to be unescaped. </param>
		''' <returns>          Unescaped value. </returns>
		''' <exception cref="IllegalArgumentException"> When an Illegal value
		'''                  is provided. </exception>
		Public Shared Function unescapeValue(ByVal val As String) As Object

				Dim chars As Char() = val.ToCharArray()
				Dim beg As Integer = 0
				Dim [end] As Integer = chars.Length

				' Trim off leading and trailing whitespace.
				Do While (beg < [end]) AndAlso isWhitespace(chars(beg))
					beg += 1
				Loop

				Do While (beg < [end]) AndAlso isWhitespace(chars([end] - 1))
					[end] -= 1
				Loop

				' Add back the trailing whitespace with a preceding '\'
				' (escaped or unescaped) that was taken off in the above
				' loop. Whether or not to retain this whitespace is decided below.
				If [end] <> chars.Length AndAlso (beg < [end]) AndAlso chars([end] - 1) = "\"c Then [end] += 1
				If beg >= [end] Then Return ""

				If chars(beg) = "#"c Then
					' Value is binary (eg: "#CEB1DF80").
					beg += 1
					Return decodeHexPairs(chars, beg, [end])
				End If

				' Trim off quotes.
				If (chars(beg) = """"c) AndAlso (chars([end] - 1) = """"c) Then
					beg += 1
					[end] -= 1
				End If

				Dim builder As New StringBuilder([end] - beg)
				Dim esc As Integer = -1 ' index of the last escaped character

				For i As Integer = beg To [end] - 1
					If (chars(i) = "\"c) AndAlso (i + 1 < [end]) Then
						If Not Char.IsLetterOrDigit(chars(i + 1)) Then
							i += 1 ' skip backslash
							builder.Append(chars(i)) ' snarf escaped char
							esc = i
						Else

							' Convert hex-encoded UTF-8 to 16-bit chars.
							Dim utf8 As SByte() = getUtf8Octets(chars, i, [end])
							If utf8.Length > 0 Then
								Try
									builder.Append(New String(utf8, "UTF8"))
								Catch e As java.io.UnsupportedEncodingException
									' shouldn't happen
								End Try
								i += utf8.Length * 3 - 1 ' no utf8 bytes available, invalid DN
							Else

								' '/' has no meaning, throw exception
								Throw New System.ArgumentException("Not a valid attribute string value:" & val & ",improper usage of backslash")
							End If
						End If
					Else
						builder.Append(chars(i)) ' snarf unescaped char
					End If
				Next i

				' Get rid of the unescaped trailing whitespace with the
				' preceding '\' character that was previously added back.
				Dim len As Integer = builder.Length
				If isWhitespace(builder.Chars(len - 1)) AndAlso esc <> ([end] - 1) Then builder.Length = len - 1
				Return builder.ToString()
		End Function


	'        
	'         * Given an array of chars (with starting and ending indexes into it)
	'         * representing bytes encoded as hex-pairs (such as "CEB1DF80"),
	'         * returns a byte array containing the decoded bytes.
	'         
			Private Shared Function decodeHexPairs(ByVal chars As Char(), ByVal beg As Integer, ByVal [end] As Integer) As SByte()
				Dim bytes As SByte() = New SByte(([end] - beg) \ 2 - 1){}
				Dim i As Integer = 0
				Do While beg + 1 < [end]
					Dim hi As Integer = Char.digit(chars(beg), 16)
					Dim lo As Integer = Char.digit(chars(beg + 1), 16)
					If hi < 0 OrElse lo < 0 Then Exit Do
					bytes(i) = CByte((hi<<4) + lo)
					beg += 2
					i += 1
				Loop
				If beg <> [end] Then Throw New System.ArgumentException("Illegal attribute value: " & New String(chars))
				Return bytes
			End Function

	'        
	'         * Given an array of chars (with starting and ending indexes into it),
	'         * finds the largest prefix consisting of hex-encoded UTF-8 octets,
	'         * and returns a byte array containing the corresponding UTF-8 octets.
	'         *
	'         * Hex-encoded UTF-8 octets look like this:
	'         *      \03\B1\DF\80
	'         
			Private Shared Function getUtf8Octets(ByVal chars As Char(), ByVal beg As Integer, ByVal [end] As Integer) As SByte()
				Dim utf8 As SByte() = New SByte(([end] - beg) \ 3 - 1){} ' allow enough room
				Dim len As Integer = 0 ' index of first unused byte in utf8

				Dim tempVar As Boolean = (beg + 2 < [end]) AndAlso (chars(beg) = "\"c)
				beg += 1
				Do While tempVar
					Dim hi As Integer = Char.digit(chars(beg), 16)
					beg += 1
					Dim lo As Integer = Char.digit(chars(beg), 16)
					beg += 1
					If hi < 0 OrElse lo < 0 Then Exit Do
					utf8(len) = CByte((hi<<4) + lo)
					len += 1
					tempVar = (beg + 2 < [end]) AndAlso (chars(beg) = "\"c)
					beg += 1
				Loop
				If len = utf8.Length Then
					Return utf8
				Else
					Dim res As SByte() = New SByte(len - 1){}
					Array.Copy(utf8, 0, res, 0, len)
					Return res
				End If
			End Function

	'    
	'     * Best guess as to what RFC 2253 means by "whitespace".
	'     
		Private Shared Function isWhitespace(ByVal c As Char) As Boolean
			Return (c = " "c OrElse c = ControlChars.Cr)
		End Function

		''' <summary>
		''' Serializes only the unparsed RDN, for compactness and to avoid
		''' any implementation dependency.
		''' 
		''' @serialData      The RDN string
		''' </summary>
		Private Sub writeObject(ByVal s As java.io.ObjectOutputStream)
			s.defaultWriteObject()
			s.writeObject(ToString())
		End Sub

		Private Sub readObject(ByVal s As java.io.ObjectInputStream)
			s.defaultReadObject()
			entries = New List(Of )(DEFAULT_SIZE)
			Dim unparsed As String = CStr(s.readObject())
			Try
				CType(New Rfc2253Parser(unparsed), Rfc2253Parser).parseRdn(Me)
			Catch e As javax.naming.InvalidNameException
				' shouldn't happen
				Throw New java.io.StreamCorruptedException("Invalid name: " & unparsed)
			End Try
		End Sub
	End Class

End Namespace