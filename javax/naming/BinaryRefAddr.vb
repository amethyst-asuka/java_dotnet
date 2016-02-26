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

Namespace javax.naming

	''' <summary>
	''' This class represents the binary form of the address of
	''' a communications end-point.
	''' <p>
	''' A BinaryRefAddr consists of a type that describes the communication mechanism
	''' and an opaque buffer containing the address description
	''' specific to that communication mechanism. The format and interpretation of
	''' the address type and the contents of the opaque buffer are based on
	''' the agreement of three parties: the client that uses the address,
	''' the object/server that can be reached using the address,
	''' and the administrator or program that creates the address.
	''' <p>
	''' An example of a binary reference address is an BER X.500 presentation address.
	''' Another example of a binary reference address is a serialized form of
	''' a service's object handle.
	''' <p>
	''' A binary reference address is immutable in the sense that its fields
	''' once created, cannot be replaced. However, it is possible to access
	''' the byte array used to hold the opaque buffer. Programs are strongly
	''' recommended against changing this byte array. Changes to this
	''' byte array need to be explicitly synchronized.
	'''  
	''' @author Rosanna Lee
	''' @author Scott Seligman
	''' </summary>
	''' <seealso cref= RefAddr </seealso>
	''' <seealso cref= StringRefAddr
	''' @since 1.3 </seealso>

	'  
	'  * The serialized form of a BinaryRefAddr object consists of its type
	'  * name String and a byte array containing its "contents".
	'  

	Public Class BinaryRefAddr
		Inherits RefAddr

		''' <summary>
		''' Contains the bytes of the address.
		''' This field is initialized by the constructor and returned
		''' using getAddressBytes() and getAddressContents().
		''' @serial
		''' </summary>
		Private buf As SByte() = Nothing

		''' <summary>
		''' Constructs a new instance of BinaryRefAddr using its address type and a byte
		''' array for contents.
		''' </summary>
		''' <param name="addrType"> A non-null string describing the type of the address. </param>
		''' <param name="src">      The non-null contents of the address as a byte array.
		'''                 The contents of src is copied into the new BinaryRefAddr. </param>
		Public Sub New(ByVal addrType As String, ByVal src As SByte())
			Me.New(addrType, src, 0, src.Length)
		End Sub

		''' <summary>
		''' Constructs a new instance of BinaryRefAddr using its address type and
		''' a region of a byte array for contents.
		''' </summary>
		''' <param name="addrType"> A non-null string describing the type of the address. </param>
		''' <param name="src">      The non-null contents of the address as a byte array.
		'''                 The contents of src is copied into the new BinaryRefAddr. </param>
		''' <param name="offset">   The starting index in src to get the bytes.
		'''                 {@code 0 <= offset <= src.length}. </param>
		''' <param name="count">    The number of bytes to extract from src.
		'''                 {@code 0 <= count <= src.length-offset}. </param>
		Public Sub New(ByVal addrType As String, ByVal src As SByte(), ByVal offset As Integer, ByVal count As Integer)
			MyBase.New(addrType)
			buf = New SByte(count - 1){}
			Array.Copy(src, offset, buf, 0, count)
		End Sub

		''' <summary>
		''' Retrieves the contents of this address as an Object.
		''' The result is a byte array.
		''' Changes to this array will affect this BinaryRefAddr's contents.
		''' Programs are recommended against changing this array's contents
		''' and to lock the buffer if they need to change it.
		''' </summary>
		''' <returns> The non-null buffer containing this address's contents. </returns>
		Public Property Overrides content As Object
			Get
				Return buf
			End Get
		End Property


		''' <summary>
		''' Determines whether obj is equal to this address.  It is equal if
		''' it contains the same address type and their contents are byte-wise
		''' equivalent. </summary>
		''' <param name="obj">      The possibly null object to check. </param>
		''' <returns> true if the object is equal; false otherwise. </returns>
		Public Overrides Function Equals(ByVal obj As Object) As Boolean
			If (obj IsNot Nothing) AndAlso (TypeOf obj Is BinaryRefAddr) Then
				Dim target As BinaryRefAddr = CType(obj, BinaryRefAddr)
				If addrType.CompareTo(target.addrType) = 0 Then
					If buf Is Nothing AndAlso target.buf Is Nothing Then Return True
					If buf Is Nothing OrElse target.buf Is Nothing OrElse buf.Length <> target.buf.Length Then Return False
					For i As Integer = 0 To buf.Length - 1
						If buf(i) <> target.buf(i) Then Return False
					Next i
					Return True
				End If
			End If
			Return False
		End Function

		''' <summary>
		''' Computes the hash code of this address using its address type and contents.
		''' Two BinaryRefAddrs have the same hash code if they have
		''' the same address type and the same contents.
		''' It is also possible for different BinaryRefAddrs to have
		''' the same hash code.
		''' </summary>
		''' <returns> The hash code of this address as an int. </returns>
		Public Overrides Function GetHashCode() As Integer
			Dim hash As Integer = addrType.GetHashCode()
			For i As Integer = 0 To buf.Length - 1
				hash += buf(i) ' %%% improve later
			Next i
			Return hash
		End Function

		''' <summary>
		''' Generates the string representation of this address.
		''' The string consists of the address's type and contents with labels.
		''' The first 32 bytes of contents are displayed (in hexadecimal).
		''' If there are more than 32 bytes, "..." is used to indicate more.
		''' This string is meant to used for debugging purposes and not
		''' meant to be interpreted programmatically. </summary>
		''' <returns> The non-null string representation of this address. </returns>
		Public Overrides Function ToString() As String
			Dim str As New StringBuilder("Address Type: " & addrType & vbLf)

			str.Append("AddressContents: ")
			Dim i As Integer = 0
			Do While i<buf.Length AndAlso i < 32
				str.Append(Integer.toHexString(buf(i)) & " ")
				i += 1
			Loop
			If buf.Length >= 32 Then str.Append(" ..." & vbLf)
			Return (str.ToString())
		End Function

		''' <summary>
		''' Use serialVersionUID from JNDI 1.1.1 for interoperability
		''' </summary>
		Private Const serialVersionUID As Long = -3415254970957330361L
	End Class

End Namespace