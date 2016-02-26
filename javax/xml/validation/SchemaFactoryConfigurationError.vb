Imports System

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

Namespace javax.xml.validation

	''' <summary>
	''' Thrown when a problem with configuration with the Schema Factories
	''' exists. This error will typically be thrown when the class of a
	''' schema factory specified in the system properties cannot be found
	''' or instantiated.
	''' @since 1.8
	''' </summary>
	Public NotInheritable Class SchemaFactoryConfigurationError
		Inherits Exception

		Friend Const serialVersionUID As Long = 3531438703147750126L

		''' <summary>
		''' Create a new <code>SchemaFactoryConfigurationError</code> with no
		''' detail message.
		''' </summary>
		Public Sub New()
		End Sub


		''' <summary>
		''' Create a new <code>SchemaFactoryConfigurationError</code> with
		''' the <code>String</code> specified as an error message.
		''' </summary>
		''' <param name="message"> The error message for the exception. </param>
		Public Sub New(ByVal message As String)
			MyBase.New(message)
		End Sub

		''' <summary>
		''' Create a new <code>SchemaFactoryConfigurationError</code> with the
		''' given <code>Throwable</code> base cause.
		''' </summary>
		''' <param name="cause"> The exception or error to be encapsulated in a
		''' SchemaFactoryConfigurationError. </param>
		Public Sub New(ByVal cause As Exception)
			MyBase.New(cause)
		End Sub

		''' <summary>
		''' Create a new <code>SchemaFactoryConfigurationError</code> with the
		''' given <code>Throwable</code> base cause and detail message.
		''' </summary>
		''' <param name="cause"> The exception or error to be encapsulated in a
		''' SchemaFactoryConfigurationError. </param>
		''' <param name="message"> The detail message. </param>
		Public Sub New(ByVal message As String, ByVal cause As Exception)
			MyBase.New(message, cause)
		End Sub

	End Class

End Namespace