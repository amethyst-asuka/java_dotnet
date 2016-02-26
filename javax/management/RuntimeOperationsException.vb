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
	''' Represents runtime exceptions thrown in the agent when performing operations on MBeans.
	''' It wraps the actual <CODE>java.lang.RuntimeException</CODE> thrown.
	''' 
	''' @since 1.5
	''' </summary>
	Public Class RuntimeOperationsException
		Inherits JMRuntimeException

		' Serial version 
		Private Const serialVersionUID As Long = -8408923047489133588L

		''' <summary>
		''' @serial The encapsulated <seealso cref="RuntimeException"/>
		''' </summary>
		Private runtimeException As Exception


		''' <summary>
		''' Creates a <CODE>RuntimeOperationsException</CODE> that wraps the actual <CODE>java.lang.RuntimeException</CODE>.
		''' </summary>
		''' <param name="e"> the wrapped exception. </param>
		Public Sub New(ByVal e As Exception)
			MyBase.New()
			runtimeException = e
		End Sub

		''' <summary>
		''' Creates a <CODE>RuntimeOperationsException</CODE> that wraps the actual <CODE>java.lang.RuntimeException</CODE>
		''' with a detailed message.
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