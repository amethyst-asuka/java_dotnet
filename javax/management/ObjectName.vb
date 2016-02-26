Imports Microsoft.VisualBasic
Imports System
Imports System.Collections
Imports System.Collections.Generic
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

Namespace javax.management


	''' <summary>
	''' <p>Represents the object name of an MBean, or a pattern that can
	''' match the names of several MBeans.  Instances of this class are
	''' immutable.</p>
	''' 
	''' <p>An instance of this class can be used to represent:</p>
	''' <ul>
	''' <li>An object name</li>
	''' <li>An object name pattern, within the context of a query</li>
	''' </ul>
	''' 
	''' <p>An object name consists of two parts, the domain and the key
	''' properties.</p>
	''' 
	''' <p>The <em>domain</em> is a string of characters not including
	''' the character colon (<code>:</code>).  It is recommended that the domain
	''' should not contain the string "{@code //}", which is reserved for future use.
	''' 
	''' <p>If the domain includes at least one occurrence of the wildcard
	''' characters asterisk (<code>*</code>) or question mark
	''' (<code>?</code>), then the object name is a pattern.  The asterisk
	''' matches any sequence of zero or more characters, while the question
	''' mark matches any single character.</p>
	''' 
	''' <p>If the domain is empty, it will be replaced in certain contexts
	''' by the <em>default domain</em> of the MBean server in which the
	''' ObjectName is used.</p>
	''' 
	''' <p>The <em>key properties</em> are an unordered set of keys and
	''' associated values.</p>
	''' 
	''' <p>Each <em>key</em> is a nonempty string of characters which may
	''' not contain any of the characters comma (<code>,</code>), equals
	''' (<code>=</code>), colon, asterisk, or question mark.  The same key
	''' may not occur twice in a given ObjectName.</p>
	''' 
	''' <p>Each <em>value</em> associated with a key is a string of
	''' characters that is either unquoted or quoted.</p>
	''' 
	''' <p>An <em>unquoted value</em> is a possibly empty string of
	''' characters which may not contain any of the characters comma,
	''' equals, colon, or quote.</p>
	''' 
	''' <p>If the <em>unquoted value</em> contains at least one occurrence
	''' of the wildcard characters asterisk or question mark, then the object
	''' name is a <em>property value pattern</em>. The asterisk matches any
	''' sequence of zero or more characters, while the question mark matches
	''' any single character.</p>
	''' 
	''' <p>A <em>quoted value</em> consists of a quote (<code>"</code>),
	''' followed by a possibly empty string of characters, followed by
	''' another quote.  Within the string of characters, the backslash
	''' (<code>\</code>) has a special meaning.  It must be followed by
	''' one of the following characters:</p>
	''' 
	''' <ul>
	''' <li>Another backslash.  The second backslash has no special
	''' meaning and the two characters represent a single backslash.</li>
	''' 
	''' <li>The character 'n'.  The two characters represent a newline
	''' ('\n' in Java).</li>
	''' 
	''' <li>A quote.  The two characters represent a quote, and that quote
	''' is not considered to terminate the quoted value. An ending closing
	''' quote must be present for the quoted value to be valid.</li>
	''' 
	''' <li>A question mark (?) or asterisk (*).  The two characters represent
	''' a question mark or asterisk respectively.</li>
	''' </ul>
	''' 
	''' <p>A quote may not appear inside a quoted value except immediately
	''' after an odd number of consecutive backslashes.</p>
	''' 
	''' <p>The quotes surrounding a quoted value, and any backslashes
	''' within that value, are considered to be part of the value.</p>
	''' 
	''' <p>If the <em>quoted value</em> contains at least one occurrence of
	''' the characters asterisk or question mark and they are not preceded
	''' by a backslash, then they are considered as wildcard characters and
	''' the object name is a <em>property value pattern</em>. The asterisk
	''' matches any sequence of zero or more characters, while the question
	''' mark matches any single character.</p>
	''' 
	''' <p>An ObjectName may be a <em>property list pattern</em>. In this
	''' case it may have zero or more keys and associated values. It matches
	''' a nonpattern ObjectName whose domain matches and that contains the
	''' same keys and associated values, as well as possibly other keys and
	''' values.</p>
	''' 
	''' <p>An ObjectName is a <em>property value pattern</em> when at least
	''' one of its <em>quoted</em> or <em>unquoted</em> key property values
	''' contains the wildcard characters asterisk or question mark as described
	''' above. In this case it has one or more keys and associated values, with
	''' at least one of the values containing wildcard characters. It matches a
	''' nonpattern ObjectName whose domain matches and that contains the same
	''' keys whose values match; if the property value pattern is also a
	''' property list pattern then the nonpattern ObjectName can contain
	''' other keys and values.</p>
	''' 
	''' <p>An ObjectName is a <em>property pattern</em> if it is either a
	''' <em>property list pattern</em> or a <em>property value pattern</em>
	''' or both.</p>
	''' 
	''' <p>An ObjectName is a pattern if its domain contains a wildcard or
	''' if the ObjectName is a property pattern.</p>
	''' 
	''' <p>If an ObjectName is not a pattern, it must contain at least one
	''' key with its associated value.</p>
	''' 
	''' <p>Examples of ObjectName patterns are:</p>
	''' 
	''' <ul>
	''' <li>{@code *:type=Foo,name=Bar} to match names in any domain whose
	'''     exact set of keys is {@code type=Foo,name=Bar}.</li>
	''' <li>{@code d:type=Foo,name=Bar,*} to match names in the domain
	'''     {@code d} that have the keys {@code type=Foo,name=Bar} plus
	'''     zero or more other keys.</li>
	''' <li>{@code *:type=Foo,name=Bar,*} to match names in any domain
	'''     that has the keys {@code type=Foo,name=Bar} plus zero or
	'''     more other keys.</li>
	''' <li>{@code d:type=F?o,name=Bar} will match e.g.
	'''     {@code d:type=Foo,name=Bar} and {@code d:type=Fro,name=Bar}.</li>
	''' <li>{@code d:type=F*o,name=Bar} will match e.g.
	'''     {@code d:type=Fo,name=Bar} and {@code d:type=Frodo,name=Bar}.</li>
	''' <li>{@code d:type=Foo,name="B*"} will match e.g.
	'''     {@code d:type=Foo,name="Bling"}. Wildcards are recognized even
	'''     inside quotes, and like other special characters can be escaped
	'''     with {@code \}.</li>
	''' </ul>
	''' 
	''' <p>An ObjectName can be written as a String with the following
	''' elements in order:</p>
	''' 
	''' <ul>
	''' <li>The domain.
	''' <li>A colon (<code>:</code>).
	''' <li>A key property list as defined below.
	''' </ul>
	''' 
	''' <p>A key property list written as a String is a comma-separated
	''' list of elements.  Each element is either an asterisk or a key
	''' property.  A key property consists of a key, an equals
	''' (<code>=</code>), and the associated value.</p>
	''' 
	''' <p>At most one element of a key property list may be an asterisk.
	''' If the key property list contains an asterisk element, the
	''' ObjectName is a property list pattern.</p>
	''' 
	''' <p>Spaces have no special significance in a String representing an
	''' ObjectName.  For example, the String:
	''' <pre>
	''' domain: key1 = value1 , key2 = value2
	''' </pre>
	''' represents an ObjectName with two keys.  The name of each key
	''' contains six characters, of which the first and last are spaces.
	''' The value associated with the key <code>"&nbsp;key1&nbsp;"</code>
	''' also begins and ends with a space.
	''' 
	''' <p>In addition to the restrictions on characters spelt out above,
	''' no part of an ObjectName may contain a newline character
	''' (<code>'\n'</code>), whether the domain, a key, or a value, whether
	''' quoted or unquoted.  The newline character can be represented in a
	''' quoted value with the sequence <code>\n</code>.
	''' 
	''' <p>The rules on special characters and quoting apply regardless of
	''' which constructor is used to make an ObjectName.</p>
	''' 
	''' <p>To avoid collisions between MBeans supplied by different
	''' vendors, a useful convention is to begin the domain name with the
	''' reverse DNS name of the organization that specifies the MBeans,
	''' followed by a period and a string whose interpretation is
	''' determined by that organization.  For example, MBeans specified by
	''' <code>example.com</code>  would have
	''' domains such as <code>com.example.MyDomain</code>.  This is essentially
	''' the same convention as for Java-language package names.</p>
	''' 
	''' <p>The <b>serialVersionUID</b> of this class is <code>1081892073854801359L</code>.
	''' 
	''' @since 1.5
	''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
	Public Class ObjectName
		Implements IComparable(Of ObjectName), QueryExp ' don't complain serialVersionUID not constant

		''' <summary>
		''' A structure recording property structure and
		''' proposing minimal services
		''' </summary>
		Private Class [Property]

			Friend _key_index As Integer
			Friend _key_length As Integer
			Friend _value_length As Integer

			''' <summary>
			''' Constructor.
			''' </summary>
			Friend Sub New(ByVal key_index As Integer, ByVal key_length As Integer, ByVal value_length As Integer)
				_key_index = key_index
				_key_length = key_length
				_value_length = value_length
			End Sub

			''' <summary>
			''' Assigns the key index of property
			''' </summary>
			Friend Overridable Property keyIndex As Integer
				Set(ByVal key_index As Integer)
					_key_index = key_index
				End Set
			End Property

			''' <summary>
			''' Returns a key string for receiver key
			''' </summary>
			Friend Overridable Function getKeyString(ByVal name As String) As String
				Return name.Substring(_key_index, _key_length)
			End Function

			''' <summary>
			''' Returns a value string for receiver key
			''' </summary>
			Friend Overridable Function getValueString(ByVal name As String) As String
				Dim in_begin As Integer = _key_index + _key_length + 1
				Dim out_end As Integer = in_begin + _value_length
				Return name.Substring(in_begin, out_end - in_begin)
			End Function
		End Class

		''' <summary>
		''' Marker class for value pattern property.
		''' </summary>
		Private Class PatternProperty
			Inherits [Property]

			''' <summary>
			''' Constructor.
			''' </summary>
			Friend Sub New(ByVal key_index As Integer, ByVal key_length As Integer, ByVal value_length As Integer)
				MyBase.New(key_index, key_length, value_length)
			End Sub
		End Class

		' Inner classes <========================================



		' Private fields ---------------------------------------->


		' Serialization compatibility stuff -------------------->

		' Two serial forms are supported in this class. The selected form depends
		' on system property "jmx.serial.form":
		'  - "1.0" for JMX 1.0
		'  - any other value for JMX 1.1 and higher
		'
		' Serial version for old serial form
		Private Const oldSerialVersionUID As Long = -5467795090068647408L
		'
		' Serial version for new serial form
		Private Const newSerialVersionUID As Long = 1081892073854801359L
		'
		' Serializable fields in old serial form
		Private Shared ReadOnly oldSerialPersistentFields As java.io.ObjectStreamField() = { New java.io.ObjectStreamField("domain", GetType(String)), New java.io.ObjectStreamField("propertyList", GetType(Hashtable)), New java.io.ObjectStreamField("propertyListString", GetType(String)), New java.io.ObjectStreamField("canonicalName", GetType(String)), New java.io.ObjectStreamField("pattern", Boolean.TYPE), New java.io.ObjectStreamField("propertyPattern", Boolean.TYPE) }
		'
		' Serializable fields in new serial form
		Private Shared ReadOnly newSerialPersistentFields As java.io.ObjectStreamField() = { }
		'
		' Actual serial version and serial form
		Private Shared ReadOnly serialVersionUID As Long
		Private Shared ReadOnly serialPersistentFields As java.io.ObjectStreamField()
		Private Shared compat As Boolean = False
		Shared Sub New()
			Try
				Dim act As New com.sun.jmx.mbeanserver.GetPropertyAction("jmx.serial.form")
				Dim form As String = java.security.AccessController.doPrivileged(act)
				compat = (form IsNot Nothing AndAlso form.Equals("1.0"))
			Catch e As Exception
				' OK: exception means no compat with 1.0, too bad
			End Try
			If compat Then
				serialPersistentFields = oldSerialPersistentFields
				serialVersionUID = oldSerialVersionUID
			Else
				serialPersistentFields = newSerialPersistentFields
				serialVersionUID = newSerialVersionUID
			End If
		End Sub

		'
		' Serialization compatibility stuff <==============================

		' Class private fields ----------------------------------->

		''' <summary>
		''' a shared empty array for empty property lists
		''' </summary>
		Private Shared ReadOnly _Empty_property_array As [Property]() = New [Property](){}


		' Class private fields <==============================

		' Instance private fields ----------------------------------->

		''' <summary>
		''' a String containing the canonical name
		''' </summary>
		<NonSerialized> _
		Private _canonicalName As String


		''' <summary>
		''' An array of properties in the same seq order as time creation
		''' </summary>
		<NonSerialized> _
		Private _kp_array As [Property]()

		''' <summary>
		''' An array of properties in the same seq order as canonical order
		''' </summary>
		<NonSerialized> _
		Private _ca_array As [Property]()


		''' <summary>
		''' The length of the domain part of built objectname
		''' </summary>
		<NonSerialized> _
		Private _domain_length As Integer = 0


		''' <summary>
		''' The propertyList of built object name. Initialized lazily.
		''' Table that contains all the pairs (key,value) for this ObjectName.
		''' </summary>
		<NonSerialized> _
		Private _propertyList As IDictionary(Of String, String)

		''' <summary>
		''' boolean that declares if this ObjectName domain part is a pattern
		''' </summary>
		<NonSerialized> _
		Private _domain_pattern As Boolean = False

		''' <summary>
		''' boolean that declares if this ObjectName contains a pattern on the
		''' key property list
		''' </summary>
		<NonSerialized> _
		Private _property_list_pattern As Boolean = False

		''' <summary>
		''' boolean that declares if this ObjectName contains a pattern on the
		''' value of at least one key property
		''' </summary>
		<NonSerialized> _
		Private _property_value_pattern As Boolean = False

		' Instance private fields <=======================================

		' Private fields <========================================


		'  Private methods ---------------------------------------->

		' Category : Instance construction ------------------------->

		''' <summary>
		''' Initializes this <seealso cref="ObjectName"/> from the given string
		''' representation.
		''' </summary>
		''' <param name="name"> A string representation of the <seealso cref="ObjectName"/>
		''' </param>
		''' <exception cref="MalformedObjectNameException"> The string passed as a
		''' parameter does not have the right format. </exception>
		''' <exception cref="NullPointerException"> The <code>name</code> parameter
		''' is null. </exception>
		Private Sub construct(ByVal name As String)

			' The name cannot be null
			If name Is Nothing Then Throw New NullPointerException("name cannot be null")

			' Test if the name is empty
			If name.Length = 0 Then
				' this is equivalent to the whole word query object name.
				_canonicalName = "*:*"
				_kp_array = _Empty_property_array
				_ca_array = _Empty_property_array
				_domain_length = 1
				_propertyList = Nothing
				_domain_pattern = True
				_property_list_pattern = True
				_property_value_pattern = False
				Return
			End If

			' initialize parsing of the string
			Dim name_chars As Char() = name.ToCharArray()
			Dim len As Integer = name_chars.Length
			Dim canonical_chars As Char() = New Char(len - 1){} ' canonical form will
														  ' be same length at most
			Dim cname_index As Integer = 0
			Dim index As Integer = 0
			Dim c, c1 As Char

			' parses domain part
		domain_parsing:
			Do While index < len
				Select Case name_chars(index)
					Case ":"c
						_domain_length = index
						index += 1
						GoTo domain_parsing
					Case "="c
						' ":" omission check.
						'
						' Although "=" is a valid character in the domain part
						' it is true that it is rarely used in the real world.
						' So check straight away if the ":" has been omitted
						' from the ObjectName. This allows us to provide a more
						' accurate exception message.
						index += 1
						Dim i As Integer = index
						Dim tempVar As Boolean = CType(AndAlso (name_chars(i) <> ":"c), i < len)
						i += 1
						Do While tempVar
							If i = len Then Throw New MalformedObjectNameException("Domain part must be specified")
							tempVar = CType(AndAlso (name_chars(i) <> ":"c), i < len)
							i += 1
						Loop
					Case ControlChars.Lf
						Throw New MalformedObjectNameException("Invalid character '\n' in domain name")
					Case "*"c , "?"c
						_domain_pattern = True
						index += 1
					Case Else
						index += 1
				End Select
			Loop

			' check for non-empty properties
			If index = len Then Throw New MalformedObjectNameException("Key properties cannot be empty")

			' we have got the domain part, begins building of _canonicalName
			Array.Copy(name_chars, 0, canonical_chars, 0, _domain_length)
			canonical_chars(_domain_length) = ":"c
			cname_index = _domain_length + 1

			' parses property list
			Dim prop As [Property]
			Dim keys_map As IDictionary(Of String, Property) = New Dictionary(Of String, Property)
			Dim keys As String()
			Dim key_name As String
			Dim quoted_value As Boolean
			Dim property_index As Integer = 0
			Dim in_index As Integer
			Dim key_index, key_length, value_index, value_length As Integer

			keys = New String(9){}
			_kp_array = New [Property](9){}
			_property_list_pattern = False
			_property_value_pattern = False

			Do While index < len
				c = name_chars(index)

				' case of pattern properties
				If c = "*"c Then
					If _property_list_pattern Then
						Throw New MalformedObjectNameException("Cannot have several '*' characters in pattern " & "property list")
					Else
						_property_list_pattern = True
						index += 1
						If (index < len) AndAlso (name_chars(index) <> ","c) Then
							Throw New MalformedObjectNameException("Invalid character found after '*': end of " & "name or ',' expected")
						ElseIf index = len Then
							If property_index = 0 Then
								' empty properties case
								_kp_array = _Empty_property_array
								_ca_array = _Empty_property_array
								_propertyList = java.util.Collections.emptyMap()
							End If
							Exit Do
						Else
							' correct pattern spec in props, continue
							index += 1
							Continue Do
						End If
					End If
				End If

				' standard property case, key part
				in_index = index
				key_index = in_index
				If name_chars(in_index) = "="c Then Throw New MalformedObjectNameException("Invalid key (empty)")
'JAVA TO VB CONVERTER TODO TASK: Assignments within expressions are not supported in VB
				Dim tempVar2 As Boolean = CType(AndAlso ((c1 = name_chars(in_index)) <> "="c), in_index < len)
				in_index += 1
				Do While tempVar2
					Select Case c1
						' '=' considered to introduce value part
						Case "*"c , "?"c , ","c , ":"c , ControlChars.Lf
							Dim ichar As String = (If(c1=ControlChars.Lf, "\n", "" & AscW(c1)))
							Throw New MalformedObjectNameException("Invalid character '" & ichar & "' in key part of property")
					End Select
					tempVar2 = CType(AndAlso ((c1 = name_chars(in_index)) <> "="c), in_index < len)
					in_index += 1
				Loop
				If name_chars(in_index - 1) <> "="c Then Throw New MalformedObjectNameException("Unterminated key property part")
				value_index = in_index ' in_index pointing after '=' char
				key_length = value_index - key_index - 1 ' found end of key

				' standard property case, value part
				Dim value_pattern As Boolean = False
				If in_index < len AndAlso name_chars(in_index) = """"c Then
					quoted_value = True
					' the case of quoted value part
				quoted_value_parsing:
					in_index += 1
					c1 = name_chars(in_index)
					Do While (in_index < len) AndAlso (c1 <> """"c)
						' the case of an escaped character
						If c1 = "\"c Then
							in_index += 1
							If in_index = len Then Throw New MalformedObjectNameException("Unterminated quoted value")
'JAVA TO VB CONVERTER TODO TASK: Assignments within expressions are not supported in VB
							Select Case c1 = name_chars(in_index)
								Case "\"c , """"c , "?"c , "*"c , "n"c
'JAVA TO VB CONVERTER TODO TASK: VB does not allow fall-through from a non-empty 'case':
								Case Else
									Throw New MalformedObjectNameException("Invalid escape sequence '\" & AscW(c1) & "' in quoted value")
							End Select
						ElseIf c1 = ControlChars.Lf Then
							Throw New MalformedObjectNameException("Newline in quoted value")
						Else
							Select Case c1
								Case "?"c , "*"c
									value_pattern = True
							End Select
						End If
						in_index += 1
						c1 = name_chars(in_index)
					Loop
					If in_index = len Then
						Throw New MalformedObjectNameException("Unterminated quoted value")
					Else
							in_index += 1
					End If
							value_length = in_index - value_index
				Else
					' the case of standard value part
					quoted_value = False
					c1 = name_chars(in_index)
					Do While (in_index < len) AndAlso (c1 <> ","c)
					Select Case c1
						' ',' considered to be the value separator
						Case "*"c , "?"c
							value_pattern = True
							in_index += 1
						Case "="c , ":"c , """"c , ControlChars.Lf
							Dim ichar As String = (If(c1=ControlChars.Lf, "\n", "" & AscW(c1)))
							Throw New MalformedObjectNameException("Invalid character '" & ichar & "' in value part of property")
						Case Else
							in_index += 1
					End Select
						c1 = name_chars(in_index)
					Loop
					value_length = in_index - value_index
				End If

				' Parsed property, checks the end of name
				If in_index = len - 1 Then
					If quoted_value Then
						Throw New MalformedObjectNameException("Invalid ending character `" & AscW(name_chars(in_index)) & "'")
					Else
						Throw New MalformedObjectNameException("Invalid ending comma")
					End If
				Else
					in_index += 1
				End If

				' we got the key and value part, prepare a property for this
				If Not value_pattern Then
					prop = New [Property](key_index, key_length, value_length)
				Else
					_property_value_pattern = True
					prop = New PatternProperty(key_index, key_length, value_length)
				End If
				key_name = name.Substring(key_index, key_length)

				If property_index = keys.Length Then
					Dim tmp_string_array As String() = New String(property_index + 10 - 1){}
					Array.Copy(keys, 0, tmp_string_array, 0, property_index)
					keys = tmp_string_array
				End If
				keys(property_index) = key_name

				addProperty(prop, property_index, keys_map, key_name)
				property_index += 1
				index = in_index
			Loop

			' computes and set canonical name
			canonicalNameame(name_chars, canonical_chars, keys, keys_map, cname_index, property_index)
		End Sub

		''' <summary>
		''' Construct an ObjectName from a domain and a Hashtable.
		''' </summary>
		''' <param name="domain"> Domain of the ObjectName. </param>
		''' <param name="props">  Map containing couples <i>key</i> {@literal ->} <i>value</i>.
		''' </param>
		''' <exception cref="MalformedObjectNameException"> The <code>domain</code>
		''' contains an illegal character, or one of the keys or values in
		''' <code>table</code> contains an illegal character, or one of the
		''' values in <code>table</code> does not follow the rules for quoting. </exception>
		''' <exception cref="NullPointerException"> One of the parameters is null. </exception>
		Private Sub construct(ByVal domain As String, ByVal props As IDictionary(Of String, String))

			' The domain cannot be null
			If domain Is Nothing Then Throw New NullPointerException("domain cannot be null")

			' The key property list cannot be null
			If props Is Nothing Then Throw New NullPointerException("key property list cannot be null")

			' The key property list cannot be empty
			If props.Count = 0 Then Throw New MalformedObjectNameException("key property list cannot be empty")

			' checks domain validity
			If Not isDomain(domain) Then Throw New MalformedObjectNameException("Invalid domain: " & domain)

			' init canonicalname
			Dim sb As New StringBuilder
			sb.Append(domain).append(":"c)
			_domain_length = domain.Length

			' allocates the property array
			Dim nb_props As Integer = props.Count
			_kp_array = New [Property](nb_props - 1){}

			Dim keys As String() = New String(nb_props - 1){}
			Dim keys_map As IDictionary(Of String, Property) = New Dictionary(Of String, Property)
			Dim prop As [Property]
			Dim key_index As Integer
			Dim i As Integer = 0
			For Each entry As KeyValuePair(Of String, String) In props
				If sb.Length > 0 Then sb.Append(",")
				Dim key As String = entry.Key
				Dim value As String
				Try
					value = entry.Value
				Catch e As ClassCastException
					Throw New MalformedObjectNameException(e.Message)
				End Try
				key_index = sb.Length
				checkKey(key)
				sb.Append(key)
				keys(i) = key
				sb.Append("=")
				Dim value_pattern As Boolean = checkValue(value)
				sb.Append(value)
				If Not value_pattern Then
					prop = New [Property](key_index, key.Length, value.Length)
				Else
					_property_value_pattern = True
					prop = New PatternProperty(key_index, key.Length, value.Length)
				End If
				addProperty(prop, i, keys_map, key)
				i += 1
			Next entry

			' initialize canonical name and data structure
			Dim len As Integer = sb.Length
			Dim initial_chars As Char() = New Char(len - 1){}
			sb.getChars(0, len, initial_chars, 0)
			Dim canonical_chars As Char() = New Char(len - 1){}
			Array.Copy(initial_chars, 0, canonical_chars, 0, _domain_length + 1)
			canonicalNameame(initial_chars, canonical_chars, keys, keys_map, _domain_length + 1, _kp_array.Length)
		End Sub
		' Category : Instance construction <==============================

		' Category : Internal utilities ------------------------------>

		''' <summary>
		''' Add passed property to the list at the given index
		''' for the passed key name
		''' </summary>
		Private Sub addProperty(ByVal prop As [Property], ByVal index As Integer, ByVal keys_map As IDictionary(Of String, Property), ByVal key_name As String)

			If keys_map.ContainsKey(key_name) Then Throw New MalformedObjectNameException("key `" & key_name & "' already defined")

			' if no more space for property arrays, have to increase it
			If index = _kp_array.Length Then
				Dim tmp_prop_array As [Property]() = New [Property](index + 10 - 1){}
				Array.Copy(_kp_array, 0, tmp_prop_array, 0, index)
				_kp_array = tmp_prop_array
			End If
			_kp_array(index) = prop
			keys_map(key_name) = prop
		End Sub

		''' <summary>
		''' Sets the canonical name of receiver from input 'specified_chars'
		''' array, by filling 'canonical_chars' array with found 'nb-props'
		''' properties starting at position 'prop_index'.
		''' </summary>
		Private Sub setCanonicalName(ByVal specified_chars As Char(), ByVal canonical_chars As Char(), ByVal keys As String(), ByVal keys_map As IDictionary(Of String, Property), ByVal prop_index As Integer, ByVal nb_props As Integer)

			' Sort the list of found properties
			If _kp_array <> _Empty_property_array Then
				Dim tmp_keys As String() = New String(nb_props - 1){}
				Dim tmp_props As [Property]() = New [Property](nb_props - 1){}

				Array.Copy(keys, 0, tmp_keys, 0, nb_props)
				java.util.Arrays.sort(tmp_keys)
				keys = tmp_keys
				Array.Copy(_kp_array, 0, tmp_props, 0, nb_props)
				_kp_array = tmp_props
				_ca_array = New [Property](nb_props - 1){}

				' now assigns _ca_array to the sorted list of keys
				' (there cannot be two identical keys in an objectname.
				For i As Integer = 0 To nb_props - 1
					_ca_array(i) = keys_map(keys(i))
				Next i

				' now we build the canonical name and set begin indexes of
				' properties to reflect canonical form
				Dim last_index As Integer = nb_props - 1
				Dim prop_len As Integer
				Dim prop As [Property]
				For i As Integer = 0 To last_index
					prop = _ca_array(i)
					' length of prop including '=' char
					prop_len = prop._key_length + prop._value_length + 1
					Array.Copy(specified_chars, prop._key_index, canonical_chars, prop_index, prop_len)
					prop.keyIndex = prop_index
					prop_index += prop_len
					If i <> last_index Then
						canonical_chars(prop_index) = ","c
						prop_index += 1
					End If
				Next i
			End If

			' terminate canonicalname with '*' in case of pattern
			If _property_list_pattern Then
				If _kp_array <> _Empty_property_array Then
					canonical_chars(prop_index) = ","c
					prop_index += 1
				End If
				canonical_chars(prop_index) = "*"c
				prop_index += 1
			End If

			' we now build the canonicalname string
			_canonicalName = (New String(canonical_chars, 0, prop_index)).intern()
		End Sub

		''' <summary>
		''' Parse a key.
		''' <pre>final int endKey=parseKey(s,startKey);</pre>
		''' <p>key starts at startKey (included), and ends at endKey (excluded).
		''' If (startKey == endKey), then the key is empty.
		''' </summary>
		''' <param name="s"> The char array of the original string. </param>
		''' <param name="startKey"> index at which to begin parsing. </param>
		''' <returns> The index following the last character of the key.
		'''  </returns>
		Private Shared Function parseKey(ByVal s As Char(), ByVal startKey As Integer) As Integer
			Dim [next] As Integer = startKey
			Dim endKey As Integer = startKey
			Dim len As Integer = s.Length
			Do While [next] < len
				Dim k As Char = s([next])
				[next] += 1
				Select Case k
				Case "*"c, "?"c, ","c, ":"c, ControlChars.Lf
					Dim ichar As String = (If(k=ControlChars.Lf, "\n", "" & AscW(k)))
					Throw New MalformedObjectNameException("Invalid character in key: `" & ichar & "'")
				Case "="c
					' we got the key.
					endKey = [next]-1
				Case Else
					If [next] < len Then
						Continue Do
					Else
						endKey=[next]
					End If
				End Select
				Exit Do
			Loop
			Return endKey
		End Function

		''' <summary>
		''' Parse a value.
		''' <pre>final int endVal=parseValue(s,startVal);</pre>
		''' <p>value starts at startVal (included), and ends at endVal (excluded).
		''' If (startVal == endVal), then the key is empty.
		''' </summary>
		''' <param name="s"> The char array of the original string. </param>
		''' <param name="startValue"> index at which to begin parsing. </param>
		''' <returns> The first element of the int array indicates the index
		'''         following the last character of the value. The second
		'''         element of the int array indicates that the value is
		'''         a pattern when its value equals 1.
		'''  </returns>
		Private Shared Function parseValue(ByVal s As Char(), ByVal startValue As Integer) As Integer()

			Dim value_pattern As Boolean = False

			Dim [next] As Integer = startValue
			Dim endValue As Integer = startValue

			Dim len As Integer = s.Length
			Dim q As Char=s(startValue)

			If q = """"c Then
				' quoted value
				[next] += 1
				If [next] = len Then Throw New MalformedObjectNameException("Invalid quote")
				Do While [next] < len
					Dim last As Char = s([next])
					If last = "\"c Then
						[next] += 1
						If [next] = len Then Throw New MalformedObjectNameException("Invalid unterminated quoted character sequence")
						last = s([next])
						Select Case last
							Case "\"c , "?"c , "*"c , "n"c
							Case """"c
								' We have an escaped quote. If this escaped
								' quote is the last character, it does not
								' qualify as a valid termination quote.
								'
								If [next]+1 = len Then Throw New MalformedObjectNameException("Missing termination quote")
							Case Else
								Throw New MalformedObjectNameException("Invalid quoted character sequence '\" & AscW(last) & "'")
						End Select
					ElseIf last = ControlChars.Lf Then
						Throw New MalformedObjectNameException("Newline in quoted value")
					ElseIf last = """"c Then
						[next] += 1
						Exit Do
					Else
						Select Case last
							Case "?"c , "*"c
								value_pattern = True
						End Select
					End If
					[next] += 1

					' Check that last character is a termination quote.
					' We have already handled the case were the last
					' character is an escaped quote earlier.
					'
					If ([next] >= len) AndAlso (last <> """"c) Then Throw New MalformedObjectNameException("Missing termination quote")
				Loop
				endValue = [next]
				If [next] < len Then
					Dim tempVar As Boolean = s([next]) <> ","c
					[next] += 1
					If tempVar Then Throw New MalformedObjectNameException("Invalid quote")
				End If
			Else
				' Non quoted value.
				Do While [next] < len
					Dim v As Char=s([next])
					[next] += 1
					Select Case v
						Case "*"c, "?"c
							value_pattern = True
							If [next] < len Then
								Continue Do
							Else
								endValue=[next]
							End If
						Case "="c, ":"c, ControlChars.Lf
							Dim ichar As String = (If(v=ControlChars.Lf, "\n", "" & AscW(v)))
							Throw New MalformedObjectNameException("Invalid character `" & ichar & "' in value")
						Case ","c
							endValue = [next]-1
						Case Else
							If [next] < len Then
								Continue Do
							Else
								endValue=[next]
							End If
					End Select
					Exit Do
				Loop
			End If
			Return New Integer() { endValue,If(value_pattern, 1, 0)}
		End Function

		''' <summary>
		''' Check if the supplied value is a valid value.
		''' </summary>
		''' <returns> true if the value is a pattern, otherwise false. </returns>
		Private Shared Function checkValue(ByVal val As String) As Boolean

			If val Is Nothing Then Throw New NullPointerException("Invalid value (null)")

			Dim len As Integer = val.Length
			If len = 0 Then Return False

			Dim s As Char() = val.ToCharArray()
			Dim result As Integer() = parseValue(s,0)
			Dim endValue As Integer = result(0)
			Dim value_pattern As Boolean = result(1) = 1
			If endValue < len Then Throw New MalformedObjectNameException("Invalid character in value: `" & AscW(s(endValue)) & "'")
			Return value_pattern
		End Function

		''' <summary>
		''' Check if the supplied key is a valid key.
		''' </summary>
		Private Shared Sub checkKey(ByVal key As String)

			If key Is Nothing Then Throw New NullPointerException("Invalid key (null)")

			Dim len As Integer = key.Length
			If len = 0 Then Throw New MalformedObjectNameException("Invalid key (empty)")
			Dim k As Char()=key.ToCharArray()
			Dim endKey As Integer = parseKey(k,0)
			If endKey < len Then Throw New MalformedObjectNameException("Invalid character in value: `" & AscW(k(endKey)) & "'")
		End Sub


		' Category : Internal utilities <==============================

		' Category : Internal accessors ------------------------------>

		''' <summary>
		''' Check if domain is a valid domain.  Set _domain_pattern if appropriate.
		''' </summary>
		Private Function isDomain(ByVal domain As String) As Boolean
			If domain Is Nothing Then Return True
			Dim len As Integer = domain.Length
			Dim [next] As Integer = 0
			Do While [next] < len
				Dim c As Char = domain.Chars([next])
				[next] += 1
				Select Case c
					Case ":"c , ControlChars.Lf
						Return False
					Case "*"c , "?"c
						_domain_pattern = True
				End Select
			Loop
			Return True
		End Function

		' Category : Internal accessors <==============================

		' Category : Serialization ----------------------------------->

		''' <summary>
		''' Deserializes an <seealso cref="ObjectName"/> from an <seealso cref="ObjectInputStream"/>.
		''' @serialData <ul>
		'''               <li>In the current serial form (value of property
		'''                   <code>jmx.serial.form</code> differs from
		'''                   <code>1.0</code>): the string
		'''                   &quot;&lt;domain&gt;:&lt;properties&gt;&lt;wild&gt;&quot;,
		'''                   where: <ul>
		'''                            <li>&lt;domain&gt; represents the domain part
		'''                                of the <seealso cref="ObjectName"/></li>
		'''                            <li>&lt;properties&gt; represents the list of
		'''                                properties, as returned by
		'''                                <seealso cref="#getKeyPropertyListString"/>
		'''                            <li>&lt;wild&gt; is empty if not
		'''                                <code>isPropertyPattern</code>, or
		'''                                is the character "<code>*</code>" if
		'''                                <code>isPropertyPattern</code>
		'''                                and &lt;properties&gt; is empty, or
		'''                                is "<code>,*</code>" if
		'''                                <code>isPropertyPattern</code> and
		'''                                &lt;properties&gt; is not empty.
		'''                            </li>
		'''                          </ul>
		'''                   The intent is that this string could be supplied
		'''                   to the <seealso cref="#ObjectName(String)"/> constructor to
		'''                   produce an equivalent <seealso cref="ObjectName"/>.
		'''               </li>
		'''               <li>In the old serial form (value of property
		'''                   <code>jmx.serial.form</code> is
		'''                   <code>1.0</code>): &lt;domain&gt; &lt;propertyList&gt;
		'''                   &lt;propertyListString&gt; &lt;canonicalName&gt;
		'''                   &lt;pattern&gt; &lt;propertyPattern&gt;,
		'''                   where: <ul>
		'''                            <li>&lt;domain&gt; represents the domain part
		'''                                of the <seealso cref="ObjectName"/></li>
		'''                            <li>&lt;propertyList&gt; is the
		'''                                <seealso cref="Hashtable"/> that contains all the
		'''                                pairs (key,value) for this
		'''                                <seealso cref="ObjectName"/></li>
		'''                            <li>&lt;propertyListString&gt; is the
		'''                                <seealso cref="String"/> representation of the
		'''                                list of properties in any order (not
		'''                                mandatorily a canonical representation)
		'''                                </li>
		'''                            <li>&lt;canonicalName&gt; is the
		'''                                <seealso cref="String"/> containing this
		'''                                <seealso cref="ObjectName"/>'s canonical name</li>
		'''                            <li>&lt;pattern&gt; is a boolean which is
		'''                                <code>true</code> if this
		'''                                <seealso cref="ObjectName"/> contains a pattern</li>
		'''                            <li>&lt;propertyPattern&gt; is a boolean which
		'''                                is <code>true</code> if this
		'''                                <seealso cref="ObjectName"/> contains a pattern in
		'''                                the list of properties</li>
		'''                          </ul>
		'''               </li>
		'''             </ul>
		''' </summary>
		Private Sub readObject(ByVal [in] As java.io.ObjectInputStream)

			Dim cn As String
			If compat Then
				' Read an object serialized in the old serial form
				'
				'in.defaultReadObject();
				Dim fields As java.io.ObjectInputStream.GetField = [in].readFields()
				Dim propListString As String = CStr(fields.get("propertyListString", ""))

				' 6616825: take care of property patterns
				Dim propPattern As Boolean = fields.get("propertyPattern", False)
				If propPattern Then propListString = (If(propListString.Length=0, "*", (propListString & ",*")))

				cn = CStr(fields.get("domain", "default")) & ":" & propListString
			Else
				' Read an object serialized in the new serial form
				'
				[in].defaultReadObject()
				cn = CStr([in].readObject())
			End If

			Try
				construct(cn)
			Catch e As NullPointerException
				Throw New java.io.InvalidObjectException(e.ToString())
			Catch e As MalformedObjectNameException
				Throw New java.io.InvalidObjectException(e.ToString())
			End Try
		End Sub


		''' <summary>
		''' Serializes an <seealso cref="ObjectName"/> to an <seealso cref="ObjectOutputStream"/>.
		''' @serialData <ul>
		'''               <li>In the current serial form (value of property
		'''                   <code>jmx.serial.form</code> differs from
		'''                   <code>1.0</code>): the string
		'''                   &quot;&lt;domain&gt;:&lt;properties&gt;&lt;wild&gt;&quot;,
		'''                   where: <ul>
		'''                            <li>&lt;domain&gt; represents the domain part
		'''                                of the <seealso cref="ObjectName"/></li>
		'''                            <li>&lt;properties&gt; represents the list of
		'''                                properties, as returned by
		'''                                <seealso cref="#getKeyPropertyListString"/>
		'''                            <li>&lt;wild&gt; is empty if not
		'''                                <code>isPropertyPattern</code>, or
		'''                                is the character "<code>*</code>" if
		'''                                this <code>isPropertyPattern</code>
		'''                                and &lt;properties&gt; is empty, or
		'''                                is "<code>,*</code>" if
		'''                                <code>isPropertyPattern</code> and
		'''                                &lt;properties&gt; is not empty.
		'''                            </li>
		'''                          </ul>
		'''                   The intent is that this string could be supplied
		'''                   to the <seealso cref="#ObjectName(String)"/> constructor to
		'''                   produce an equivalent <seealso cref="ObjectName"/>.
		'''               </li>
		'''               <li>In the old serial form (value of property
		'''                   <code>jmx.serial.form</code> is
		'''                   <code>1.0</code>): &lt;domain&gt; &lt;propertyList&gt;
		'''                   &lt;propertyListString&gt; &lt;canonicalName&gt;
		'''                   &lt;pattern&gt; &lt;propertyPattern&gt;,
		'''                   where: <ul>
		'''                            <li>&lt;domain&gt; represents the domain part
		'''                                of the <seealso cref="ObjectName"/></li>
		'''                            <li>&lt;propertyList&gt; is the
		'''                                <seealso cref="Hashtable"/> that contains all the
		'''                                pairs (key,value) for this
		'''                                <seealso cref="ObjectName"/></li>
		'''                            <li>&lt;propertyListString&gt; is the
		'''                                <seealso cref="String"/> representation of the
		'''                                list of properties in any order (not
		'''                                mandatorily a canonical representation)
		'''                                </li>
		'''                            <li>&lt;canonicalName&gt; is the
		'''                                <seealso cref="String"/> containing this
		'''                                <seealso cref="ObjectName"/>'s canonical name</li>
		'''                            <li>&lt;pattern&gt; is a boolean which is
		'''                                <code>true</code> if this
		'''                                <seealso cref="ObjectName"/> contains a pattern</li>
		'''                            <li>&lt;propertyPattern&gt; is a boolean which
		'''                                is <code>true</code> if this
		'''                                <seealso cref="ObjectName"/> contains a pattern in
		'''                                the list of properties</li>
		'''                          </ul>
		'''               </li>
		'''             </ul>
		''' </summary>
		Private Sub writeObject(ByVal out As java.io.ObjectOutputStream)

		  If compat Then
			' Serializes this instance in the old serial form
			' Read CR 6441274 before making any changes to this code
			Dim fields As java.io.ObjectOutputStream.PutField = out.putFields()
			fields.put("domain", _canonicalName.Substring(0, _domain_length))
			fields.put("propertyList", keyPropertyList)
			fields.put("propertyListString", keyPropertyListString)
			fields.put("canonicalName", _canonicalName)
			fields.put("pattern", (_domain_pattern OrElse _property_list_pattern))
			fields.put("propertyPattern", _property_list_pattern)
			out.writeFields()
		  Else
			' Serializes this instance in the new serial form
			'
			out.defaultWriteObject()
			out.writeObject(serializedNameString)
		  End If
		End Sub

		'  Category : Serialization <===================================

		' Private methods <========================================

		' Public methods ---------------------------------------->

		' Category : ObjectName Construction ------------------------------>

		''' <summary>
		''' <p>Return an instance of ObjectName that can be used anywhere
		''' an object obtained with {@link #ObjectName(String) new
		''' ObjectName(name)} can be used.  The returned object may be of
		''' a subclass of ObjectName.  Calling this method twice with the
		''' same parameters may return the same object or two equal but
		''' not identical objects.</p>
		''' </summary>
		''' <param name="name">  A string representation of the object name.
		''' </param>
		''' <returns> an ObjectName corresponding to the given String.
		''' </returns>
		''' <exception cref="MalformedObjectNameException"> The string passed as a
		''' parameter does not have the right format. </exception>
		''' <exception cref="NullPointerException"> The <code>name</code> parameter
		''' is null.
		'''  </exception>
		Public Shared Function getInstance(ByVal name As String) As ObjectName
			Return New ObjectName(name)
		End Function

		''' <summary>
		''' <p>Return an instance of ObjectName that can be used anywhere
		''' an object obtained with {@link #ObjectName(String, String,
		''' String) new ObjectName(domain, key, value)} can be used.  The
		''' returned object may be of a subclass of ObjectName.  Calling
		''' this method twice with the same parameters may return the same
		''' object or two equal but not identical objects.</p>
		''' </summary>
		''' <param name="domain">  The domain part of the object name. </param>
		''' <param name="key">  The attribute in the key property of the object name. </param>
		''' <param name="value"> The value in the key property of the object name.
		''' </param>
		''' <returns> an ObjectName corresponding to the given domain,
		''' key, and value.
		''' </returns>
		''' <exception cref="MalformedObjectNameException"> The
		''' <code>domain</code>, <code>key</code>, or <code>value</code>
		''' contains an illegal character, or <code>value</code> does not
		''' follow the rules for quoting. </exception>
		''' <exception cref="NullPointerException"> One of the parameters is null.
		'''  </exception>
		Public Shared Function getInstance(ByVal domain As String, ByVal key As String, ByVal value As String) As ObjectName
			Return New ObjectName(domain, key, value)
		End Function

		''' <summary>
		''' <p>Return an instance of ObjectName that can be used anywhere
		''' an object obtained with {@link #ObjectName(String, Hashtable)
		''' new ObjectName(domain, table)} can be used.  The returned
		''' object may be of a subclass of ObjectName.  Calling this method
		''' twice with the same parameters may return the same object or
		''' two equal but not identical objects.</p>
		''' </summary>
		''' <param name="domain">  The domain part of the object name. </param>
		''' <param name="table"> A hash table containing one or more key
		''' properties.  The key of each entry in the table is the key of a
		''' key property in the object name.  The associated value in the
		''' table is the associated value in the object name.
		''' </param>
		''' <returns> an ObjectName corresponding to the given domain and
		''' key mappings.
		''' </returns>
		''' <exception cref="MalformedObjectNameException"> The <code>domain</code>
		''' contains an illegal character, or one of the keys or values in
		''' <code>table</code> contains an illegal character, or one of the
		''' values in <code>table</code> does not follow the rules for
		''' quoting. </exception>
		''' <exception cref="NullPointerException"> One of the parameters is null.
		'''  </exception>
		Public Shared Function getInstance(ByVal domain As String, ByVal table As Dictionary(Of String, String)) As ObjectName
			Return New ObjectName(domain, table)
		End Function

		''' <summary>
		''' <p>Return an instance of ObjectName that can be used anywhere
		''' the given object can be used.  The returned object may be of a
		''' subclass of ObjectName.  If <code>name</code> is of a subclass
		''' of ObjectName, it is not guaranteed that the returned object
		''' will be of the same class.</p>
		''' 
		''' <p>The returned value may or may not be identical to
		''' <code>name</code>.  Calling this method twice with the same
		''' parameters may return the same object or two equal but not
		''' identical objects.</p>
		''' 
		''' <p>Since ObjectName is immutable, it is not usually useful to
		''' make a copy of an ObjectName.  The principal use of this method
		''' is to guard against a malicious caller who might pass an
		''' instance of a subclass with surprising behavior to sensitive
		''' code.  Such code can call this method to obtain an ObjectName
		''' that is known not to have surprising behavior.</p>
		''' </summary>
		''' <param name="name"> an instance of the ObjectName class or of a subclass
		''' </param>
		''' <returns> an instance of ObjectName or a subclass that is known to
		''' have the same semantics.  If <code>name</code> respects the
		''' semantics of ObjectName, then the returned object is equal
		''' (though not necessarily identical) to <code>name</code>.
		''' </returns>
		''' <exception cref="NullPointerException"> The <code>name</code> is null.
		'''  </exception>
		Public Shared Function getInstance(ByVal name As ObjectName) As ObjectName
			If name.GetType().Equals(GetType(ObjectName)) Then Return name
			Return com.sun.jmx.mbeanserver.Util.newObjectName(name.serializedNameString)
		End Function

		''' <summary>
		''' Construct an object name from the given string.
		''' </summary>
		''' <param name="name">  A string representation of the object name.
		''' </param>
		''' <exception cref="MalformedObjectNameException"> The string passed as a
		''' parameter does not have the right format. </exception>
		''' <exception cref="NullPointerException"> The <code>name</code> parameter
		''' is null. </exception>
		Public Sub New(ByVal name As String)
			construct(name)
		End Sub

		''' <summary>
		''' Construct an object name with exactly one key property.
		''' </summary>
		''' <param name="domain">  The domain part of the object name. </param>
		''' <param name="key">  The attribute in the key property of the object name. </param>
		''' <param name="value"> The value in the key property of the object name.
		''' </param>
		''' <exception cref="MalformedObjectNameException"> The
		''' <code>domain</code>, <code>key</code>, or <code>value</code>
		''' contains an illegal character, or <code>value</code> does not
		''' follow the rules for quoting. </exception>
		''' <exception cref="NullPointerException"> One of the parameters is null. </exception>
		Public Sub New(ByVal domain As String, ByVal key As String, ByVal value As String)
			' If key or value are null a NullPointerException
			' will be thrown by the put method in Hashtable.
			'
			Dim table As IDictionary(Of String, String) = java.util.Collections.singletonMap(key, value)
			construct(domain, table)
		End Sub

		''' <summary>
		''' Construct an object name with several key properties from a Hashtable.
		''' </summary>
		''' <param name="domain">  The domain part of the object name. </param>
		''' <param name="table"> A hash table containing one or more key
		''' properties.  The key of each entry in the table is the key of a
		''' key property in the object name.  The associated value in the
		''' table is the associated value in the object name.
		''' </param>
		''' <exception cref="MalformedObjectNameException"> The <code>domain</code>
		''' contains an illegal character, or one of the keys or values in
		''' <code>table</code> contains an illegal character, or one of the
		''' values in <code>table</code> does not follow the rules for
		''' quoting. </exception>
		''' <exception cref="NullPointerException"> One of the parameters is null. </exception>
		Public Sub New(ByVal domain As String, ByVal table As Dictionary(Of String, String))
			construct(domain, table)
	'         The exception for when a key or value in the table is not a
	'           String is now ClassCastException rather than
	'           MalformedObjectNameException.  This was not previously
	'           specified.  
		End Sub

		' Category : ObjectName Construction <==============================


		' Category : Getter methods ------------------------------>

		''' <summary>
		''' Checks whether the object name is a pattern.
		''' <p>
		''' An object name is a pattern if its domain contains a
		''' wildcard or if the object name is a property pattern.
		''' </summary>
		''' <returns>  True if the name is a pattern, otherwise false. </returns>
		Public Overridable Property pattern As Boolean
			Get
				Return (_domain_pattern OrElse _property_list_pattern OrElse _property_value_pattern)
			End Get
		End Property

		''' <summary>
		''' Checks whether the object name is a pattern on the domain part.
		''' </summary>
		''' <returns>  True if the name is a domain pattern, otherwise false.
		'''  </returns>
		Public Overridable Property domainPattern As Boolean
			Get
				Return _domain_pattern
			End Get
		End Property

		''' <summary>
		''' Checks whether the object name is a pattern on the key properties.
		''' <p>
		''' An object name is a pattern on the key properties if it is a
		''' pattern on the key property list (e.g. "d:k=v,*") or on the
		''' property values (e.g. "d:k=*") or on both (e.g. "d:k=*,*").
		''' </summary>
		''' <returns>  True if the name is a property pattern, otherwise false. </returns>
		Public Overridable Property propertyPattern As Boolean
			Get
				Return _property_list_pattern OrElse _property_value_pattern
			End Get
		End Property

		''' <summary>
		''' Checks whether the object name is a pattern on the key property list.
		''' <p>
		''' For example, "d:k=v,*" and "d:k=*,*" are key property list patterns
		''' whereas "d:k=*" is not.
		''' </summary>
		''' <returns>  True if the name is a property list pattern, otherwise false.
		''' 
		''' @since 1.6 </returns>
		Public Overridable Property propertyListPattern As Boolean
			Get
				Return _property_list_pattern
			End Get
		End Property

		''' <summary>
		''' Checks whether the object name is a pattern on the value part
		''' of at least one of the key properties.
		''' <p>
		''' For example, "d:k=*" and "d:k=*,*" are property value patterns
		''' whereas "d:k=v,*" is not.
		''' </summary>
		''' <returns>  True if the name is a property value pattern, otherwise false.
		''' 
		''' @since 1.6 </returns>
		Public Overridable Property propertyValuePattern As Boolean
			Get
				Return _property_value_pattern
			End Get
		End Property

		''' <summary>
		''' Checks whether the value associated with a key in a key
		''' property is a pattern.
		''' </summary>
		''' <param name="property"> The property whose value is to be checked.
		''' </param>
		''' <returns> True if the value associated with the given key property
		''' is a pattern, otherwise false.
		''' </returns>
		''' <exception cref="NullPointerException"> If <code>property</code> is null. </exception>
		''' <exception cref="IllegalArgumentException"> If <code>property</code> is not
		''' a valid key property for this ObjectName.
		''' 
		''' @since 1.6 </exception>
		Public Overridable Function isPropertyValuePattern(ByVal [property] As String) As Boolean
			If [property] Is Nothing Then Throw New NullPointerException("key property can't be null")
			For i As Integer = 0 To _ca_array.Length - 1
				Dim prop As [Property] = _ca_array(i)
				Dim key As String = prop.getKeyString(_canonicalName)
				If key.Equals([property]) Then Return (TypeOf prop Is PatternProperty)
			Next i
			Throw New System.ArgumentException("key property not found")
		End Function

		''' <summary>
		''' <p>Returns the canonical form of the name; that is, a string
		''' representation where the properties are sorted in lexical
		''' order.</p>
		''' 
		''' <p>More precisely, the canonical form of the name is a String
		''' consisting of the <em>domain part</em>, a colon
		''' (<code>:</code>), the <em>canonical key property list</em>, and
		''' a <em>pattern indication</em>.</p>
		''' 
		''' <p>The <em>canonical key property list</em> is the same string
		''' as described for <seealso cref="#getCanonicalKeyPropertyListString()"/>.</p>
		''' 
		''' <p>The <em>pattern indication</em> is:
		''' <ul>
		''' <li>empty for an ObjectName
		''' that is not a property list pattern;
		''' <li>an asterisk for an ObjectName
		''' that is a property list pattern with no keys; or
		''' <li>a comma and an
		''' asterisk (<code>,*</code>) for an ObjectName that is a property
		''' list pattern with at least one key.
		''' </ul>
		''' </summary>
		''' <returns> The canonical form of the name. </returns>
		Public Overridable Property canonicalName As String
			Get
				Return _canonicalName
			End Get
		End Property

		''' <summary>
		''' Returns the domain part.
		''' </summary>
		''' <returns> The domain. </returns>
		Public Overridable Property domain As String
			Get
				Return _canonicalName.Substring(0, _domain_length)
			End Get
		End Property

		''' <summary>
		''' Obtains the value associated with a key in a key property.
		''' </summary>
		''' <param name="property"> The property whose value is to be obtained.
		''' </param>
		''' <returns> The value of the property, or null if there is no such
		''' property in this ObjectName.
		''' </returns>
		''' <exception cref="NullPointerException"> If <code>property</code> is null. </exception>
		Public Overridable Function getKeyProperty(ByVal [property] As String) As String
			Return _getKeyPropertyList()([property])
		End Function

		''' <summary>
		''' <p>Returns the key properties as a Map.  The returned
		''' value is a Map in which each key is a key in the
		''' ObjectName's key property list and each value is the associated
		''' value.</p>
		''' 
		''' <p>The returned value must not be modified.</p>
		''' </summary>
		''' <returns> The table of key properties. </returns>
		Private Function _getKeyPropertyList() As IDictionary(Of String, String)
			SyncLock Me
				If _propertyList Is Nothing Then
					' build (lazy eval) the property list from the canonical
					' properties array
					_propertyList = New Dictionary(Of String, String)
					Dim len As Integer = _ca_array.Length
					Dim prop As [Property]
					For i As Integer = len - 1 To 0 Step -1
						prop = _ca_array(i)
						_propertyList(prop.getKeyString(_canonicalName)) = prop.getValueString(_canonicalName)
					Next i
				End If
			End SyncLock
			Return _propertyList
		End Function

		''' <summary>
		''' <p>Returns the key properties as a Hashtable.  The returned
		''' value is a Hashtable in which each key is a key in the
		''' ObjectName's key property list and each value is the associated
		''' value.</p>
		''' 
		''' <p>The returned value may be unmodifiable.  If it is
		''' modifiable, changing it has no effect on this ObjectName.</p>
		''' </summary>
		''' <returns> The table of key properties. </returns>
		' CR 6441274 depends on the modification property defined above
		Public Overridable Property keyPropertyList As Dictionary(Of String, String)
			Get
				Return New Dictionary(Of String, String)(_getKeyPropertyList())
			End Get
		End Property

		''' <summary>
		''' <p>Returns a string representation of the list of key
		''' properties specified at creation time.  If this ObjectName was
		''' constructed with the constructor <seealso cref="#ObjectName(String)"/>,
		''' the key properties in the returned String will be in the same
		''' order as in the argument to the constructor.</p>
		''' </summary>
		''' <returns> The key property list string.  This string is
		''' independent of whether the ObjectName is a pattern. </returns>
		Public Overridable Property keyPropertyListString As String
			Get
				' BEWARE : we rebuild the propertyliststring at each call !!
				If _kp_array.Length = 0 Then Return ""
    
				' the size of the string is the canonical one minus domain
				' part and pattern part
				Dim total_size As Integer = _canonicalName.Length - _domain_length - 1 - (If(_property_list_pattern, 2, 0))
    
				Dim dest_chars As Char() = New Char(total_size - 1){}
				Dim value As Char() = _canonicalName.ToCharArray()
				writeKeyPropertyListString(value,dest_chars,0)
				Return New String(dest_chars)
			End Get
		End Property

		''' <summary>
		''' <p>Returns the serialized string of the ObjectName.
		''' properties specified at creation time.  If this ObjectName was
		''' constructed with the constructor <seealso cref="#ObjectName(String)"/>,
		''' the key properties in the returned String will be in the same
		''' order as in the argument to the constructor.</p>
		''' </summary>
		''' <returns> The key property list string.  This string is
		''' independent of whether the ObjectName is a pattern. </returns>
		Private Property serializedNameString As String
			Get
    
				' the size of the string is the canonical one
				Dim total_size As Integer = _canonicalName.Length
				Dim dest_chars As Char() = New Char(total_size - 1){}
				Dim value As Char() = _canonicalName.ToCharArray()
				Dim offset As Integer = _domain_length+1
    
				' copy "domain:" into dest_chars
				'
				Array.Copy(value, 0, dest_chars, 0, offset)
    
				' Add property list string
				Dim [end] As Integer = writeKeyPropertyListString(value,dest_chars,offset)
    
				' Add ",*" if necessary
				If _property_list_pattern Then
					If [end] = offset Then
						' Property list string is empty.
						dest_chars([end]) = "*"c
					Else
						' Property list string is not empty.
						dest_chars([end]) = ","c
						dest_chars([end]+1) = "*"c
					End If
				End If
    
				Return New String(dest_chars)
			End Get
		End Property

		''' <summary>
		''' <p>Write a string representation of the list of key
		''' properties specified at creation time in the given array, starting
		''' at the specified offset.  If this ObjectName was
		''' constructed with the constructor <seealso cref="#ObjectName(String)"/>,
		''' the key properties in the returned String will be in the same
		''' order as in the argument to the constructor.</p>
		''' </summary>
		''' <returns> offset + #of chars written </returns>
		Private Function writeKeyPropertyListString(ByVal canonicalChars As Char(), ByVal data As Char(), ByVal offset As Integer) As Integer
			If _kp_array.Length = 0 Then Return offset

			Dim dest_chars As Char() = data
			Dim value As Char() = canonicalChars

			Dim index As Integer = offset
			Dim len As Integer = _kp_array.Length
			Dim last As Integer = len - 1
			For i As Integer = 0 To len - 1
				Dim prop As [Property] = _kp_array(i)
				Dim prop_len As Integer = prop._key_length + prop._value_length + 1
				Array.Copy(value, prop._key_index, dest_chars, index, prop_len)
				index += prop_len
				If i < last Then
					dest_chars(index) = ","c
					index += 1
				End If
			Next i
			Return index
		End Function



		''' <summary>
		''' Returns a string representation of the list of key properties,
		''' in which the key properties are sorted in lexical order. This
		''' is used in lexicographic comparisons performed in order to
		''' select MBeans based on their key property list.  Lexical order
		''' is the order implied by {@link String#compareTo(String)
		''' String.compareTo(String)}.
		''' </summary>
		''' <returns> The canonical key property list string.  This string is
		''' independent of whether the ObjectName is a pattern. </returns>
		Public Overridable Property canonicalKeyPropertyListString As String
			Get
				If _ca_array.Length = 0 Then Return ""
    
				Dim len As Integer = _canonicalName.Length
				If _property_list_pattern Then len -= 2
				Return _canonicalName.Substring(_domain_length +1, len - (_domain_length +1))
			End Get
		End Property
		' Category : Getter methods <===================================

		' Category : Utilities ---------------------------------------->

		''' <summary>
		''' <p>Returns a string representation of the object name.  The
		''' format of this string is not specified, but users can expect
		''' that two ObjectNames return the same string if and only if they
		''' are equal.</p>
		''' </summary>
		''' <returns> a string representation of this object name. </returns>
		Public Overrides Function ToString() As String
			Return serializedNameString
		End Function

		''' <summary>
		''' Compares the current object name with another object name.  Two
		''' ObjectName instances are equal if and only if their canonical
		''' forms are equal.  The canonical form is the string described
		''' for <seealso cref="#getCanonicalName()"/>.
		''' </summary>
		''' <param name="object">  The object name that the current object name is to be
		'''        compared with.
		''' </param>
		''' <returns> True if <code>object</code> is an ObjectName whose
		''' canonical form is equal to that of this ObjectName. </returns>
		Public Overrides Function Equals(ByVal [object] As Object) As Boolean

			' same object case
			If Me Is [object] Then Return True

			' object is not an object name case
			If Not(TypeOf [object] Is ObjectName) Then Return False

			' equality when canonical names are the same
			' (because usage of intern())
			Dim [on] As ObjectName = CType([object], ObjectName)
			Dim on_string As String = [on]._canonicalName
			If _canonicalName = on_string Then ' ES: OK Return True

			' Because we are sharing canonical form between object names,
			' we have finished the comparison at this stage ==> unequal
			Return False
		End Function

		''' <summary>
		''' Returns a hash code for this object name.
		''' 
		''' </summary>
		Public Overrides Function GetHashCode() As Integer
			Return _canonicalName.GetHashCode()
		End Function

		''' <summary>
		''' <p>Returns a quoted form of the given String, suitable for
		''' inclusion in an ObjectName.  The returned value can be used as
		''' the value associated with a key in an ObjectName.  The String
		''' <code>s</code> may contain any character.  Appropriate quoting
		''' ensures that the returned value is legal in an ObjectName.</p>
		''' 
		''' <p>The returned value consists of a quote ('"'), a sequence of
		''' characters corresponding to the characters of <code>s</code>,
		''' and another quote.  Characters in <code>s</code> appear
		''' unchanged within the returned value except:</p>
		''' 
		''' <ul>
		''' <li>A quote ('"') is replaced by a backslash (\) followed by a quote.</li>
		''' <li>An asterisk ('*') is replaced by a backslash (\) followed by an
		''' asterisk.</li>
		''' <li>A question mark ('?') is replaced by a backslash (\) followed by
		''' a question mark.</li>
		''' <li>A backslash ('\') is replaced by two backslashes.</li>
		''' <li>A newline character (the character '\n' in Java) is replaced
		''' by a backslash followed by the character '\n'.</li>
		''' </ul>
		''' </summary>
		''' <param name="s"> the String to be quoted.
		''' </param>
		''' <returns> the quoted String.
		''' </returns>
		''' <exception cref="NullPointerException"> if <code>s</code> is null.
		'''  </exception>
		Public Shared Function quote(ByVal s As String) As String
			Dim buf As New StringBuilder("""")
			Dim len As Integer = s.Length
			For i As Integer = 0 To len - 1
				Dim c As Char = s.Chars(i)
				Select Case c
				Case ControlChars.Lf
					c = "n"c
					buf.Append("\"c)
				Case "\"c, """"c, "*"c, "?"c
					buf.Append("\"c)
				End Select
				buf.Append(c)
			Next i
			buf.Append(""""c)
			Return buf.ToString()
		End Function

		''' <summary>
		''' <p>Returns an unquoted form of the given String.  If
		''' <code>q</code> is a String returned by <seealso cref="#quote quote(s)"/>,
		''' then <code>unquote(q).equals(s)</code>.  If there is no String
		''' <code>s</code> for which <code>quote(s).equals(q)</code>, then
		''' unquote(q) throws an IllegalArgumentException.</p>
		''' 
		''' <p>These rules imply that there is a one-to-one mapping between
		''' quoted and unquoted forms.</p>
		''' </summary>
		''' <param name="q"> the String to be unquoted.
		''' </param>
		''' <returns> the unquoted String.
		''' </returns>
		''' <exception cref="IllegalArgumentException"> if <code>q</code> could not
		''' have been returned by the <seealso cref="#quote"/> method, for instance
		''' if it does not begin and end with a quote (").
		''' </exception>
		''' <exception cref="NullPointerException"> if <code>q</code> is null.
		'''  </exception>
		Public Shared Function unquote(ByVal q As String) As String
			Dim buf As New StringBuilder
			Dim len As Integer = q.Length
			If len < 2 OrElse q.Chars(0) <> """"c OrElse q.Chars(len - 1) <> """"c Then Throw New System.ArgumentException("Argument not quoted")
			For i As Integer = 1 To len - 2
				Dim c As Char = q.Chars(i)
				If c = "\"c Then
					If i = len - 2 Then Throw New System.ArgumentException("Trailing backslash")
					i += 1
					c = q.Chars(i)
					Select Case c
					Case "n"c
						c = ControlChars.Lf
					Case "\"c, """"c, "*"c, "?"c
					Case Else
					  Throw New System.ArgumentException("Bad character '" & AscW(c) & "' after backslash")
					End Select
				Else
					Select Case c
						Case "*"c , "?"c , """"c, ControlChars.Lf
							 Throw New System.ArgumentException("Invalid unescaped character '" & AscW(c) & "' in the string to unquote")
					End Select
				End If
				buf.Append(c)
			Next i
			Return buf.ToString()
		End Function

		''' <summary>
		''' Defines the wildcard "*:*" ObjectName.
		''' 
		''' @since 1.6
		''' </summary>
		Public Shared ReadOnly WILDCARD As ObjectName = com.sun.jmx.mbeanserver.Util.newObjectName("*:*")

		' Category : Utilities <===================================

		' Category : QueryExp Interface ---------------------------------------->

		''' <summary>
		''' <p>Test whether this ObjectName, which may be a pattern,
		''' matches another ObjectName.  If <code>name</code> is a pattern,
		''' the result is false.  If this ObjectName is a pattern, the
		''' result is true if and only if <code>name</code> matches the
		''' pattern.  If neither this ObjectName nor <code>name</code> is
		''' a pattern, the result is true if and only if the two
		''' ObjectNames are equal as described for the {@link
		''' #equals(Object)} method.</p>
		''' </summary>
		''' <param name="name"> The name of the MBean to compare to.
		''' </param>
		''' <returns> True if <code>name</code> matches this ObjectName.
		''' </returns>
		''' <exception cref="NullPointerException"> if <code>name</code> is null.
		'''  </exception>
		Public Overridable Function apply(ByVal name As ObjectName) As Boolean Implements QueryExp.apply

			If name Is Nothing Then Throw New NullPointerException

			If name._domain_pattern OrElse name._property_list_pattern OrElse name._property_value_pattern Then Return False

			' No pattern
			If (Not _domain_pattern) AndAlso (Not _property_list_pattern) AndAlso (Not _property_value_pattern) Then Return _canonicalName.Equals(name._canonicalName)

			Return matchDomains(name) AndAlso matchKeys(name)
		End Function

		Private Function matchDomains(ByVal name As ObjectName) As Boolean
			If _domain_pattern Then Return com.sun.jmx.mbeanserver.Util.wildmatch(name.domain,domain)
			Return domain.Equals(name.domain)
		End Function

		Private Function matchKeys(ByVal name As ObjectName) As Boolean
			' If key property value pattern but not key property list
			' pattern, then the number of key properties must be equal
			'
			If _property_value_pattern AndAlso (Not _property_list_pattern) AndAlso (name._ca_array.Length <> _ca_array.Length) Then Return False

			' If key property value pattern or key property list pattern,
			' then every property inside pattern should exist in name
			'
			If _property_value_pattern OrElse _property_list_pattern Then
				Dim nameProps As IDictionary(Of String, String) = name._getKeyPropertyList()
				Dim props As [Property]() = _ca_array
				Dim cn As String = _canonicalName
				For i As Integer = props.Length - 1 To 0 Step -1
					' Find value in given object name for key at current
					' index in receiver
					'
					Dim p As [Property] = props(i)
					Dim k As String = p.getKeyString(cn)
					Dim v As String = nameProps(k)
					' Did we find a value for this key ?
					'
					If v Is Nothing Then Return False
					' If this property is ok (same key, same value), go to next
					'
					If _property_value_pattern AndAlso (TypeOf p Is PatternProperty) Then
						' wildmatch key property values
						' p is the property pattern, v is the string
						If com.sun.jmx.mbeanserver.Util.wildmatch(v,p.getValueString(cn)) Then
							Continue For
						Else
							Return False
						End If
					End If
					If v.Equals(p.getValueString(cn)) Then Continue For
					Return False
				Next i
				Return True
			End If

			' If no pattern, then canonical names must be equal
			'
			Dim p1 As String = name.canonicalKeyPropertyListString
			Dim p2 As String = canonicalKeyPropertyListString
			Return (p1.Equals(p2))
		End Function

	'     Method inherited from QueryExp, no implementation needed here
	'       because ObjectName is not relative to an MBeanServer and does
	'       not contain a subquery.
	'    
		Public Overridable Property mBeanServer Implements QueryExp.setMBeanServer As MBeanServer
			Set(ByVal mbs As MBeanServer)
			End Set
		End Property

		' Category : QueryExp Interface <=========================

		' Category : Comparable Interface ---------------------------------------->

		''' <summary>
		''' <p>Compares two ObjectName instances. The ordering relation between
		''' ObjectNames is not completely specified but is intended to be such
		''' that a sorted list of ObjectNames will appear in an order that is
		''' convenient for a person to read.</p>
		''' 
		''' <p>In particular, if the two ObjectName instances have different
		''' domains then their order is the lexicographical order of the domains.
		''' The ordering of the key property list remains unspecified.</p>
		''' 
		''' <p>For example, the ObjectName instances below:</p>
		''' <ul>
		''' <li>Shapes:type=Square,name=3</li>
		''' <li>Colors:type=Red,name=2</li>
		''' <li>Shapes:type=Triangle,side=isosceles,name=2</li>
		''' <li>Colors:type=Red,name=1</li>
		''' <li>Shapes:type=Square,name=1</li>
		''' <li>Colors:type=Blue,name=1</li>
		''' <li>Shapes:type=Square,name=2</li>
		''' <li>JMImplementation:type=MBeanServerDelegate</li>
		''' <li>Shapes:type=Triangle,side=scalene,name=1</li>
		''' </ul>
		''' <p>could be ordered as follows:</p>
		''' <ul>
		''' <li>Colors:type=Blue,name=1</li>
		''' <li>Colors:type=Red,name=1</li>
		''' <li>Colors:type=Red,name=2</li>
		''' <li>JMImplementation:type=MBeanServerDelegate</li>
		''' <li>Shapes:type=Square,name=1</li>
		''' <li>Shapes:type=Square,name=2</li>
		''' <li>Shapes:type=Square,name=3</li>
		''' <li>Shapes:type=Triangle,side=scalene,name=1</li>
		''' <li>Shapes:type=Triangle,side=isosceles,name=2</li>
		''' </ul>
		''' </summary>
		''' <param name="name"> the ObjectName to be compared.
		''' </param>
		''' <returns> a negative integer, zero, or a positive integer as this
		'''         ObjectName is less than, equal to, or greater than the
		'''         specified ObjectName.
		''' 
		''' @since 1.6 </returns>
		Public Overridable Function compareTo(ByVal name As ObjectName) As Integer
			' Quick optimization:
			'
			If name Is Me Then Return 0

			' (1) Compare domains
			'
			Dim domainValue As Integer = Me.domain.CompareTo(name.domain)
			If domainValue <> 0 Then Return domainValue

			' (2) Compare "type=" keys
			'
			' Within a given domain, all names with missing or empty "type="
			' come before all names with non-empty type.
			'
			' When both types are missing or empty, canonical-name ordering
			' applies which is a total order.
			'
			Dim thisTypeKey As String = Me.getKeyProperty("type")
			Dim anotherTypeKey As String = name.getKeyProperty("type")
			If thisTypeKey Is Nothing Then thisTypeKey = ""
			If anotherTypeKey Is Nothing Then anotherTypeKey = ""
			Dim typeKeyValue As Integer = thisTypeKey.CompareTo(anotherTypeKey)
			If typeKeyValue <> 0 Then Return typeKeyValue

			' (3) Compare canonical names
			'
			Return Me.canonicalName.CompareTo(name.canonicalName)
		End Function

		' Category : Comparable Interface <=========================

		' Public methods <========================================

	End Class

End Namespace