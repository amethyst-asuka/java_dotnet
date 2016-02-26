Imports System

'
' * Copyright (c) 2000, 2005, Oracle and/or its affiliates. All rights reserved.
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

Namespace javax.xml.transform

	''' <summary>
	''' Thrown when a problem with configuration with the Transformer Factories
	''' exists. This error will typically be thrown when the class of a
	''' transformation factory specified in the system properties cannot be found
	''' or instantiated.
	''' </summary>
	Public Class TransformerFactoryConfigurationError
		Inherits Exception

		Private Const serialVersionUID As Long = -6527718720676281516L

		''' <summary>
		''' <code>Exception</code> for the
		'''  <code>TransformerFactoryConfigurationError</code>.
		''' </summary>
		Private exception As Exception

		''' <summary>
		''' Create a new <code>TransformerFactoryConfigurationError</code> with no
		''' detail mesage.
		''' </summary>
		Public Sub New()

			MyBase.New()

			Me.exception = Nothing
		End Sub

		''' <summary>
		''' Create a new <code>TransformerFactoryConfigurationError</code> with
		''' the <code>String</code> specified as an error message.
		''' </summary>
		''' <param name="msg"> The error message for the exception. </param>
		Public Sub New(ByVal msg As String)

			MyBase.New(msg)

			Me.exception = Nothing
		End Sub

		''' <summary>
		''' Create a new <code>TransformerFactoryConfigurationError</code> with a
		''' given <code>Exception</code> base cause of the error.
		''' </summary>
		''' <param name="e"> The exception to be encapsulated in a
		''' TransformerFactoryConfigurationError. </param>
		Public Sub New(ByVal e As Exception)

			MyBase.New(e.ToString())

			Me.exception = e
		End Sub

		''' <summary>
		''' Create a new <code>TransformerFactoryConfigurationError</code> with the
		''' given <code>Exception</code> base cause and detail message.
		''' </summary>
		''' <param name="e"> The exception to be encapsulated in a
		''' TransformerFactoryConfigurationError </param>
		''' <param name="msg"> The detail message. </param>
		Public Sub New(ByVal e As Exception, ByVal msg As String)

			MyBase.New(msg)

			Me.exception = e
		End Sub

		''' <summary>
		''' Return the message (if any) for this error . If there is no
		''' message for the exception and there is an encapsulated
		''' exception then the message of that exception will be returned.
		''' </summary>
		''' <returns> The error message. </returns>
		Public Overridable Property message As String
			Get
    
				Dim ___message As String = MyBase.message
    
				If (___message Is Nothing) AndAlso (exception IsNot Nothing) Then Return exception.Message
    
				Return ___message
			End Get
		End Property

		''' <summary>
		''' Return the actual exception (if any) that caused this exception to
		''' be raised.
		''' </summary>
		''' <returns> The encapsulated exception, or null if there is none. </returns>
		Public Overridable Property exception As Exception
			Get
				Return exception
			End Get
		End Property
		''' <summary>
		''' use the exception chaining mechanism of JDK1.4
		''' </summary>
		Public Property Overrides cause As Exception
			Get
				Return exception
			End Get
		End Property
	End Class

End Namespace