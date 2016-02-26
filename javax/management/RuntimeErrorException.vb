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
	''' When a <CODE>java.lang.Error</CODE> occurs in the agent it should be caught and
	''' re-thrown as a <CODE>RuntimeErrorException</CODE>.
	''' 
	''' @since 1.5
	''' </summary>
	Public Class RuntimeErrorException
		Inherits JMRuntimeException

		' Serial version 
		Private Const serialVersionUID As Long = 704338937753949796L

		''' <summary>
		''' @serial The encapsulated <seealso cref="Error"/>
		''' </summary>
		Private [error] As Exception

		''' <summary>
		''' Default constructor.
		''' </summary>
		''' <param name="e"> the wrapped error. </param>
		Public Sub New(ByVal e As Exception)
		  MyBase.New()
		  [error] = e
		End Sub

		''' <summary>
		''' Constructor that allows a specific error message to be specified.
		''' </summary>
		''' <param name="e"> the wrapped error. </param>
		''' <param name="message"> the detail message. </param>
		Public Sub New(ByVal e As Exception, ByVal message As String)
		   MyBase.New(message)
		   [error] = e
		End Sub

		''' <summary>
		''' Returns the actual <seealso cref="Error"/> thrown.
		''' </summary>
		''' <returns> the wrapped <seealso cref="Error"/>. </returns>
		Public Overridable Property targetError As Exception
			Get
				Return [error]
			End Get
		End Property

		''' <summary>
		''' Returns the actual <seealso cref="Error"/> thrown.
		''' </summary>
		''' <returns> the wrapped <seealso cref="Error"/>. </returns>
		Public Overridable Property cause As Exception
			Get
				Return [error]
			End Get
		End Property
	End Class

End Namespace