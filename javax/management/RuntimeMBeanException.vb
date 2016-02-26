Imports System

'
' * Copyright (c) 1999, 2003, Oracle and/or its affiliates. All rights reserved.
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
	''' Represents runtime exceptions thrown by MBean methods in
	''' the agent. It "wraps" the actual <CODE>java.lang.RuntimeException</CODE> exception thrown.
	''' This exception will be built by the MBeanServer when a call to an
	''' MBean method throws a runtime exception.
	''' 
	''' @since 1.5
	''' </summary>
	Public Class RuntimeMBeanException
		Inherits JMRuntimeException

		' Serial version 
		Private Const serialVersionUID As Long = 5274912751982730171L

		''' <summary>
		''' @serial The encapsulated <seealso cref="RuntimeException"/>
		''' </summary>
		Private runtimeException As Exception


		''' <summary>
		''' Creates a <CODE>RuntimeMBeanException</CODE> that wraps the actual <CODE>java.lang.RuntimeException</CODE>.
		''' </summary>
		''' <param name="e"> the wrapped exception. </param>
		Public Sub New(ByVal e As Exception)
			MyBase.New()
			runtimeException = e
		End Sub

		''' <summary>
		''' Creates a <CODE>RuntimeMBeanException</CODE> that wraps the actual <CODE>java.lang.RuntimeException</CODE> with
		''' a detailed message.
		''' </summary>
		''' <param name="e"> the wrapped exception. </param>
		''' <param name="message"> the detail message. </param>
		Public Sub New(ByVal e As Exception, ByVal message As String)
			MyBase.New(message)
			runtimeException = e
		End Sub

		''' <summary>
		''' Returns the actual <seealso cref="RuntimeException"/> thrown.
		''' </summary>
		''' <returns> the wrapped <seealso cref="RuntimeException"/>. </returns>
		Public Overridable Property targetException As Exception
			Get
				Return runtimeException
			End Get
		End Property

		''' <summary>
		''' Returns the actual <seealso cref="RuntimeException"/> thrown.
		''' </summary>
		''' <returns> the wrapped <seealso cref="RuntimeException"/>. </returns>
		Public Overridable Property cause As Exception
			Get
				Return runtimeException
			End Get
		End Property
	End Class

End Namespace