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
	''' Represents exceptions thrown in the MBean server when using the
	''' java.lang.reflect classes to invoke methods on MBeans. It "wraps" the
	''' actual java.lang.Exception thrown.
	''' 
	''' @since 1.5
	''' </summary>
	Public Class ReflectionException
		Inherits JMException

		' Serial version 
		Private Const serialVersionUID As Long = 9170809325636915553L

		''' <summary>
		''' @serial The wrapped <seealso cref="Exception"/>
		''' </summary>
		Private exception As Exception


		''' <summary>
		''' Creates a <CODE>ReflectionException</CODE> that wraps the actual <CODE>java.lang.Exception</CODE>.
		''' </summary>
		''' <param name="e"> the wrapped exception. </param>
		Public Sub New(ByVal e As Exception)
			MyBase.New()
			exception = e
		End Sub

		''' <summary>
		''' Creates a <CODE>ReflectionException</CODE> that wraps the actual <CODE>java.lang.Exception</CODE> with
		''' a detail message.
		''' </summary>
		''' <param name="e"> the wrapped exception. </param>
		''' <param name="message"> the detail message. </param>
		Public Sub New(ByVal e As Exception, ByVal message As String)
			MyBase.New(message)
			exception = e
		End Sub

		''' <summary>
		''' Returns the actual <seealso cref="Exception"/> thrown.
		''' </summary>
		''' <returns> the wrapped <seealso cref="Exception"/>. </returns>
		Public Overridable Property targetException As Exception
			Get
				Return exception
			End Get
		End Property

		''' <summary>
		''' Returns the actual <seealso cref="Exception"/> thrown.
		''' </summary>
		''' <returns> the wrapped <seealso cref="Exception"/>. </returns>
		Public Overridable Property cause As Exception
			Get
				Return exception
			End Get
		End Property
	End Class

End Namespace