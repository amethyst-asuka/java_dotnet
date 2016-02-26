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
	''' Indicates a serious configuration error.
	''' </summary>
	Public Class TransformerConfigurationException
		Inherits TransformerException

		''' <summary>
		''' Create a new <code>TransformerConfigurationException</code> with no
		''' detail mesage.
		''' </summary>
		Public Sub New()
			MyBase.New("Configuration Error")
		End Sub

		''' <summary>
		''' Create a new <code>TransformerConfigurationException</code> with
		''' the <code>String </code> specified as an error message.
		''' </summary>
		''' <param name="msg"> The error message for the exception. </param>
		Public Sub New(ByVal msg As String)
			MyBase.New(msg)
		End Sub

		''' <summary>
		''' Create a new <code>TransformerConfigurationException</code> with a
		''' given <code>Exception</code> base cause of the error.
		''' </summary>
		''' <param name="e"> The exception to be encapsulated in a
		''' TransformerConfigurationException. </param>
		Public Sub New(ByVal e As Exception)
			MyBase.New(e)
		End Sub

		''' <summary>
		''' Create a new <code>TransformerConfigurationException</code> with the
		''' given <code>Exception</code> base cause and detail message.
		''' </summary>
		''' <param name="e"> The exception to be encapsulated in a
		'''      TransformerConfigurationException </param>
		''' <param name="msg"> The detail message. </param>
		Public Sub New(ByVal msg As String, ByVal e As Exception)
			MyBase.New(msg, e)
		End Sub

		''' <summary>
		''' Create a new TransformerConfigurationException from a message and a Locator.
		''' 
		''' <p>This constructor is especially useful when an application is
		''' creating its own exception from within a DocumentHandler
		''' callback.</p>
		''' </summary>
		''' <param name="message"> The error or warning message. </param>
		''' <param name="locator"> The locator object for the error or warning. </param>
		Public Sub New(ByVal message As String, ByVal locator As SourceLocator)
			MyBase.New(message, locator)
		End Sub

		''' <summary>
		''' Wrap an existing exception in a TransformerConfigurationException.
		''' </summary>
		''' <param name="message"> The error or warning message, or null to
		'''                use the message from the embedded exception. </param>
		''' <param name="locator"> The locator object for the error or warning. </param>
		''' <param name="e"> Any exception. </param>
		Public Sub New(ByVal message As String, ByVal locator As SourceLocator, ByVal e As Exception)
			MyBase.New(message, locator, e)
		End Sub
	End Class

End Namespace