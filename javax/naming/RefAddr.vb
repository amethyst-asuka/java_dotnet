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
	''' This class represents the address of a communications end-point.
	''' It consists of a type that describes the communication mechanism
	''' and an address contents determined by an RefAddr subclass.
	''' <p>
	''' For example, an address type could be "BSD Printer Address",
	''' which specifies that it is an address to be used with the BSD printing
	''' protocol. Its contents could be the machine name identifying the
	''' location of the printer server that understands this protocol.
	''' <p>
	''' A RefAddr is contained within a Reference.
	''' <p>
	''' RefAddr is an abstract class. Concrete implementations of it
	''' determine its synchronization properties.
	'''  
	''' @author Rosanna Lee
	''' @author Scott Seligman
	''' </summary>
	''' <seealso cref= Reference </seealso>
	''' <seealso cref= LinkRef </seealso>
	''' <seealso cref= StringRefAddr </seealso>
	''' <seealso cref= BinaryRefAddr
	''' @since 1.3 </seealso>

	'  <p>
	'  * The serialized form of a RefAddr object consists of only its type name
	'  * String.
	'  

	<Serializable> _
	Public MustInherit Class RefAddr
		''' <summary>
		''' Contains the type of this address.
		''' @serial
		''' </summary>
		Protected Friend addrType As String

		''' <summary>
		''' Constructs a new instance of RefAddr using its address type.
		''' </summary>
		''' <param name="addrType"> A non-null string describing the type of the address. </param>
		Protected Friend Sub New(ByVal addrType As String)
			Me.addrType = addrType
		End Sub

		''' <summary>
		''' Retrieves the address type of this address.
		''' </summary>
		''' <returns> The non-null address type of this address. </returns>
		Public Overridable Property type As String
			Get
				Return addrType
			End Get
		End Property

		''' <summary>
		''' Retrieves the contents of this address.
		''' </summary>
		''' <returns> The possibly null address contents. </returns>
		Public MustOverride ReadOnly Property content As Object

		''' <summary>
		''' Determines whether obj is equal to this RefAddr.
		''' <p>
		''' obj is equal to this RefAddr all of these conditions are true
		''' <ul>
		''' <li> non-null
		''' <li> instance of RefAddr
		''' <li> obj has the same address type as this RefAddr (using String.compareTo())
		''' <li> both obj and this RefAddr's contents are null or they are equal
		'''         (using the equals() test).
		''' </ul> </summary>
		''' <param name="obj"> possibly null obj to check. </param>
		''' <returns> true if obj is equal to this refaddr; false otherwise. </returns>
		''' <seealso cref= #getContent </seealso>
		''' <seealso cref= #getType </seealso>
		Public Overrides Function Equals(ByVal obj As Object) As Boolean
			If (obj IsNot Nothing) AndAlso (TypeOf obj Is RefAddr) Then
				Dim target As RefAddr = CType(obj, RefAddr)
				If addrType.CompareTo(target.addrType) = 0 Then
					Dim thisobj As Object = Me.content
					Dim thatobj As Object = target.content
					If thisobj Is thatobj Then Return True
					If thisobj IsNot Nothing Then Return thisobj.Equals(thatobj)
				End If
			End If
			Return False
		End Function

		''' <summary>
		''' Computes the hash code of this address using its address type and contents.
		''' The hash code is the sum of the hash code of the address type and
		''' the hash code of the address contents.
		''' </summary>
		''' <returns> The hash code of this address as an int. </returns>
		''' <seealso cref= java.lang.Object#hashCode </seealso>
		Public Overrides Function GetHashCode() As Integer
			Return If(content Is Nothing, addrType.GetHashCode(), addrType.GetHashCode() + content.GetHashCode())
		End Function

		''' <summary>
		''' Generates the string representation of this address.
		''' The string consists of the address's type and contents with labels.
		''' This representation is intended for display only and not to be parsed. </summary>
		''' <returns> The non-null string representation of this address. </returns>
		Public Overrides Function ToString() As String
			Dim str As New StringBuilder("Type: " & addrType & vbLf)

			str.Append("Content: " & content & vbLf)
			Return (str.ToString())
		End Function

		''' <summary>
		''' Use serialVersionUID from JNDI 1.1.1 for interoperability
		''' </summary>
		Private Const serialVersionUID As Long = -1468165120479154358L
	End Class

End Namespace