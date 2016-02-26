Imports System

'
' * Copyright (c) 1999, 2008, Oracle and/or its affiliates. All rights reserved.
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
	''' Runtime exceptions emitted by JMX implementations.
	''' 
	''' @since 1.5
	''' </summary>
	Public Class JMRuntimeException
		Inherits Exception

		' Serial version 
		Private Const serialVersionUID As Long = 6573344628407841861L

		''' <summary>
		''' Default constructor.
		''' </summary>
		Public Sub New()
			MyBase.New()
		End Sub

		''' <summary>
		''' Constructor that allows a specific error message to be specified.
		''' </summary>
		''' <param name="message"> the detail message. </param>
		Public Sub New(ByVal message As String)
			MyBase.New(message)
		End Sub

		''' <summary>
		''' Constructor with a nested exception.  This constructor is
		''' package-private because it arrived too late for the JMX 1.2
		''' specification.  A later version may make it public.
		''' </summary>
		Friend Sub New(ByVal message As String, ByVal cause As Exception)
			MyBase.New(message, cause)
		End Sub
	End Class

End Namespace