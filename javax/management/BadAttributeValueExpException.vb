Imports System

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
	''' Thrown when an invalid MBean attribute is passed to a query
	''' constructing method.  This exception is used internally by JMX
	''' during the evaluation of a query.  User code does not usually
	''' see it.
	''' 
	''' @since 1.5
	''' </summary>
	Public Class BadAttributeValueExpException
		Inherits Exception


		' Serial version 
		Private Const serialVersionUID As Long = -3105272988410493376L

		''' <summary>
		''' @serial A string representation of the attribute that originated this exception.
		''' for example, the string value can be the return of {@code attribute.toString()}
		''' </summary>
		Private val As Object

		''' <summary>
		''' Constructs a BadAttributeValueExpException using the specified Object to
		''' create the toString() value.
		''' </summary>
		''' <param name="val"> the inappropriate value. </param>
		Public Sub New(ByVal val As Object)
			Me.val = If(val Is Nothing, Nothing, val.ToString())
		End Sub


		''' <summary>
		''' Returns the string representing the object.
		''' </summary>
		Public Overrides Function ToString() As String
			Return "BadAttributeValueException: " & val
		End Function

		Private Sub readObject(ByVal ois As java.io.ObjectInputStream)
			Dim gf As java.io.ObjectInputStream.GetField = ois.readFields()
			Dim valObj As Object = gf.get("val", Nothing)

			If valObj Is Nothing Then
				val = Nothing
			ElseIf TypeOf valObj Is String Then
				val= valObj
			ElseIf System.securityManager Is Nothing OrElse TypeOf valObj Is Long? OrElse TypeOf valObj Is Integer? OrElse TypeOf valObj Is Single? OrElse TypeOf valObj Is Double? OrElse TypeOf valObj Is SByte? OrElse TypeOf valObj Is Short? OrElse TypeOf valObj Is Boolean? Then
				val = valObj.ToString() ' the serialized object is from a version without JDK-8019292 fix
			Else
				val = System.identityHashCode(valObj) & "@" & valObj.GetType().name
			End If
		End Sub
	End Class

End Namespace