Imports System

'
' * Copyright (c) 1999, 2005, Oracle and/or its affiliates. All rights reserved.
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


	' java import


	''' <summary>
	''' Represents an MBean attribute by associating its name with its value.
	''' The MBean server and other objects use this class to get and set attributes values.
	''' 
	''' @since 1.5
	''' </summary>
	<Serializable> _
	Public Class Attribute

		' Serial version 
		Private Const serialVersionUID As Long = 2484220110589082382L

		''' <summary>
		''' @serial Attribute name.
		''' </summary>
		Private name As String

		''' <summary>
		''' @serial Attribute value
		''' </summary>
		Private value As Object= Nothing


		''' <summary>
		''' Constructs an Attribute object which associates the given attribute name with the given value.
		''' </summary>
		''' <param name="name"> A String containing the name of the attribute to be created. Cannot be null. </param>
		''' <param name="value"> The Object which is assigned to the attribute. This object must be of the same type as the attribute.
		'''  </param>
		Public Sub New(ByVal name As String, ByVal value As Object)

			If name Is Nothing Then Throw New RuntimeOperationsException(New System.ArgumentException("Attribute name cannot be null "))

			Me.name = name
			Me.value = value
		End Sub


		''' <summary>
		''' Returns a String containing the  name of the attribute.
		''' </summary>
		''' <returns> the name of the attribute. </returns>
		Public Overridable Property name As String
			Get
				Return name
			End Get
		End Property

		''' <summary>
		''' Returns an Object that is the value of this attribute.
		''' </summary>
		''' <returns> the value of the attribute. </returns>
		Public Overridable Property value As Object
			Get
				Return value
			End Get
		End Property

		''' <summary>
		''' Compares the current Attribute Object with another Attribute Object.
		''' </summary>
		''' <param name="object">  The Attribute that the current Attribute is to be compared with.
		''' </param>
		''' <returns>  True if the two Attribute objects are equal, otherwise false. </returns>


		Public Overrides Function Equals(ByVal [object] As Object) As Boolean
			If Not(TypeOf [object] Is Attribute) Then Return False
			Dim val As Attribute = CType([object], Attribute)

			If value Is Nothing Then
				If val.value Is Nothing Then
					Return name.Equals(val.name)
				Else
					Return False
				End If
			End If

			Return ((name.Equals(val.name)) AndAlso (value.Equals(val.value)))
		End Function

		''' <summary>
		''' Returns a hash code value for this attribute.
		''' </summary>
		''' <returns> a hash code value for this attribute. </returns>
		Public Overrides Function GetHashCode() As Integer
			Return name.GetHashCode() Xor (If(value Is Nothing, 0, value.GetHashCode()))
		End Function

		''' <summary>
		''' Returns a String object representing this Attribute's value. The format of this
		''' string is not specified, but users can expect that two Attributes return the
		''' same string if and only if they are equal.
		''' </summary>
		Public Overrides Function ToString() As String
			Return name & " = " & value
		End Function
	End Class

End Namespace