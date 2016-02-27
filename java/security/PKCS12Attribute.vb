Imports System
Imports sun.security.util

'
' * Copyright (c) 2013, Oracle and/or its affiliates. All rights reserved.
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

Namespace java.security

	''' <summary>
	''' An attribute associated with a PKCS12 keystore entry.
	''' The attribute name is an ASN.1 Object Identifier and the attribute
	''' value is a set of ASN.1 types.
	''' 
	''' @since 1.8
	''' </summary>
	Public NotInheritable Class PKCS12Attribute
		Implements KeyStore.Entry.Attribute

		Private Shared ReadOnly COLON_SEPARATED_HEX_PAIRS As java.util.regex.Pattern = java.util.regex.Pattern.compile("^[0-9a-fA-F]{2}(:[0-9a-fA-F]{2})+$")
		Private name As String
		Private value As String
		Private encoded As SByte()
		Private hashValue As Integer = -1

		''' <summary>
		''' Constructs a PKCS12 attribute from its name and value.
		''' The name is an ASN.1 Object Identifier represented as a list of
		''' dot-separated integers.
		''' A string value is represented as the string itself.
		''' A binary value is represented as a string of colon-separated
		''' pairs of hexadecimal digits.
		''' Multi-valued attributes are represented as a comma-separated
		''' list of values, enclosed in square brackets. See
		''' <seealso cref="Arrays#toString(java.lang.Object[])"/>.
		''' <p>
		''' A string value will be DER-encoded as an ASN.1 UTF8String and a
		''' binary value will be DER-encoded as an ASN.1 Octet String.
		''' </summary>
		''' <param name="name"> the attribute's identifier </param>
		''' <param name="value"> the attribute's value
		''' </param>
		''' <exception cref="NullPointerException"> if {@code name} or {@code value}
		'''     is {@code null} </exception>
		''' <exception cref="IllegalArgumentException"> if {@code name} or
		'''     {@code value} is incorrectly formatted </exception>
		Public Sub New(ByVal name As String, ByVal value As String)
			If name Is Nothing OrElse value Is Nothing Then Throw New NullPointerException
			' Validate name
			Dim type As ObjectIdentifier
			Try
				type = New ObjectIdentifier(name)
			Catch e As java.io.IOException
				Throw New IllegalArgumentException("Incorrect format: name", e)
			End Try
			Me.name = name

			' Validate value
			Dim length As Integer = value.length()
			Dim values As String()
			If value.Chars(0) = "["c AndAlso value.Chars(length - 1) = "]"c Then
				values = value.Substring(1, length - 1 - 1).Split(", ")
			Else
				values = New String(){ value }
			End If
			Me.value = value

			Try
				Me.encoded = encode(type, values)
			Catch e As java.io.IOException
				Throw New IllegalArgumentException("Incorrect format: value", e)
			End Try
		End Sub

		''' <summary>
		''' Constructs a PKCS12 attribute from its ASN.1 DER encoding.
		''' The DER encoding is specified by the following ASN.1 definition:
		''' <pre>
		''' 
		''' Attribute ::= SEQUENCE {
		'''     type   AttributeType,
		'''     values SET OF AttributeValue
		''' }
		''' AttributeType ::= OBJECT IDENTIFIER
		''' AttributeValue ::= ANY defined by type
		''' 
		''' </pre>
		''' </summary>
		''' <param name="encoded"> the attribute's ASN.1 DER encoding. It is cloned
		'''     to prevent subsequent modificaion.
		''' </param>
		''' <exception cref="NullPointerException"> if {@code encoded} is
		'''     {@code null} </exception>
		''' <exception cref="IllegalArgumentException"> if {@code encoded} is
		'''     incorrectly formatted </exception>
		Public Sub New(ByVal encoded As SByte())
			If encoded Is Nothing Then Throw New NullPointerException
			Me.encoded = encoded.clone()

			Try
				parse(encoded)
			Catch e As java.io.IOException
				Throw New IllegalArgumentException("Incorrect format: encoded", e)
			End Try
		End Sub

		''' <summary>
		''' Returns the attribute's ASN.1 Object Identifier represented as a
		''' list of dot-separated integers.
		''' </summary>
		''' <returns> the attribute's identifier </returns>
		Public Property Overrides name As String
			Get
				Return name
			End Get
		End Property

		''' <summary>
		''' Returns the attribute's ASN.1 DER-encoded value as a string.
		''' An ASN.1 DER-encoded value is returned in one of the following
		''' {@code String} formats:
		''' <ul>
		''' <li> the DER encoding of a basic ASN.1 type that has a natural
		'''      string representation is returned as the string itself.
		'''      Such types are currently limited to BOOLEAN, INTEGER,
		'''      OBJECT IDENTIFIER, UTCTime, GeneralizedTime and the
		'''      following six ASN.1 string types: UTF8String,
		'''      PrintableString, T61String, IA5String, BMPString and
		'''      GeneralString.
		''' <li> the DER encoding of any other ASN.1 type is not decoded but
		'''      returned as a binary string of colon-separated pairs of
		'''      hexadecimal digits.
		''' </ul>
		''' Multi-valued attributes are represented as a comma-separated
		''' list of values, enclosed in square brackets. See
		''' <seealso cref="Arrays#toString(java.lang.Object[])"/>.
		''' </summary>
		''' <returns> the attribute value's string encoding </returns>
		Public Property Overrides value As String
			Get
				Return value
			End Get
		End Property

		''' <summary>
		''' Returns the attribute's ASN.1 DER encoding.
		''' </summary>
		''' <returns> a clone of the attribute's DER encoding </returns>
		Public Property encoded As SByte()
			Get
				Return encoded.clone()
			End Get
		End Property

		''' <summary>
		''' Compares this {@code PKCS12Attribute} and a specified object for
		''' equality.
		''' </summary>
		''' <param name="obj"> the comparison object
		''' </param>
		''' <returns> true if {@code obj} is a {@code PKCS12Attribute} and
		''' their DER encodings are equal. </returns>
		Public Overrides Function Equals(ByVal obj As Object) As Boolean
			If Me Is obj Then Return True
			If Not(TypeOf obj Is PKCS12Attribute) Then Return False
			Return java.util.Arrays.Equals(encoded, CType(obj, PKCS12Attribute).encoded)
		End Function

		''' <summary>
		''' Returns the hashcode for this {@code PKCS12Attribute}.
		''' The hash code is computed from its DER encoding.
		''' </summary>
		''' <returns> the hash code </returns>
		Public Overrides Function GetHashCode() As Integer
			If hashValue = -1 Then java.util.Arrays.hashCode(encoded)
			Return hashValue
		End Function

		''' <summary>
		''' Returns a string representation of this {@code PKCS12Attribute}.
		''' </summary>
		''' <returns> a name/value pair separated by an 'equals' symbol </returns>
		Public Overrides Function ToString() As String
			Return (name & "=" & value)
		End Function

		Private Function encode(ByVal type As ObjectIdentifier, ByVal values As String()) As SByte()
			Dim attribute As New DerOutputStream
			attribute.putOID(type)
			Dim attrContent As New DerOutputStream
			For Each value_Renamed As String In values
				If COLON_SEPARATED_HEX_PAIRS.matcher(value_Renamed).matches() Then
					Dim bytes As SByte() = (New System.Numerics.BigInteger(value_Renamed.replace(":", ""), 16)).toByteArray()
					If bytes(0) = 0 Then bytes = java.util.Arrays.copyOfRange(bytes, 1, bytes.Length)
					attrContent.putOctetString(bytes)
				Else
					attrContent.putUTF8String(value_Renamed)
				End If
			Next value_Renamed
			attribute.write(DerValue.tag_Set, attrContent)
			Dim attributeValue As New DerOutputStream
			attributeValue.write(DerValue.tag_Sequence, attribute)

			Return attributeValue.toByteArray()
		End Function

		Private Sub parse(ByVal encoded As SByte())
			Dim attributeValue As New DerInputStream(encoded)
			Dim attrSeq As DerValue() = attributeValue.getSequence(2)
			Dim type As ObjectIdentifier = attrSeq(0).oID
			Dim attrContent As New DerInputStream(attrSeq(1).toByteArray())
			Dim attrValueSet As DerValue() = attrContent.getSet(1)
			Dim values As String() = New String(attrValueSet.Length - 1){}
			Dim printableString As String
			For i As Integer = 0 To attrValueSet.Length - 1
				If attrValueSet(i).tag = DerValue.tag_OctetString Then
					values(i) = Debug.ToString(attrValueSet(i).octetString)
				Else
					printableString = attrValueSet(i).asString
					If printableString IsNot Nothing Then
						values(i) = printableString
					ElseIf attrValueSet(i).tag = DerValue.tag_ObjectId Then
						values(i) = attrValueSet(i).oID.ToString()
					ElseIf attrValueSet(i).tag = DerValue.tag_GeneralizedTime Then
						values(i) = attrValueSet(i).generalizedTime.ToString()
					ElseIf attrValueSet(i).tag = DerValue.tag_UtcTime Then
						values(i) = attrValueSet(i).uTCTime.ToString()
					ElseIf attrValueSet(i).tag = DerValue.tag_Integer Then
						values(i) = attrValueSet(i).big java.lang.[Integer].ToString()
					ElseIf attrValueSet(i).tag = DerValue.tag_Boolean Then
						values(i) = Convert.ToString(attrValueSet(i).boolean)
					Else
						values(i) = Debug.ToString(attrValueSet(i).dataBytes)
					End If
					End If
			Next i

			Me.name = type.ToString()
			Me.value = If(values.Length = 1, values(0), java.util.Arrays.ToString(values))
		End Sub
	End Class

End Namespace