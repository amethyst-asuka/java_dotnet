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
	''' Represents "user defined" exceptions thrown by MBean methods
	''' in the agent. It "wraps" the actual "user defined" exception thrown.
	''' This exception will be built by the MBeanServer when a call to an
	''' MBean method results in an unknown exception.
	''' 
	''' @since 1.5
	''' </summary>
	Public Class MBeanException
		Inherits JMException


		' Serial version 
		Private Const serialVersionUID As Long = 4066342430588744142L

		''' <summary>
		''' @serial Encapsulated <seealso cref="Exception"/>
		''' </summary>
		Private exception As Exception


		''' <summary>
		''' Creates an <CODE>MBeanException</CODE> that wraps the actual <CODE>java.lang.Exception</CODE>.
		''' </summary>
		''' <param name="e"> the wrapped exception. </param>
		Public Sub New(ByVal e As Exception)
			MyBase.New()
			exception = e
		End Sub

		''' <summary>
		''' Creates an <CODE>MBeanException</CODE> that wraps the actual <CODE>java.lang.Exception</CODE> with
		''' a detail message.
		''' </summary>
		''' <param name="e"> the wrapped exception. </param>
		''' <param name="message"> the detail message. </param>
		Public Sub New(ByVal e As Exception, ByVal message As String)
			MyBase.New(message)
			exception = e
		End Sub


		''' <summary>
		''' Return the actual <seealso cref="Exception"/> thrown.
		''' </summary>
		''' <returns> the wrapped exception. </returns>
		Public Overridable Property targetException As Exception
			Get
				Return exception
			End Get
		End Property

		''' <summary>
		''' Return the actual <seealso cref="Exception"/> thrown.
		''' </summary>
		''' <returns> the wrapped exception. </returns>
		Public Overridable Property cause As Exception
			Get
				Return exception
			End Get
		End Property
	End Class

End Namespace