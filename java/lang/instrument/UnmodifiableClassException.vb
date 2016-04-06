Imports System

'
' * Copyright (c) 2004, 2008, Oracle and/or its affiliates. All rights reserved.
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

Namespace java.lang.instrument

	''' <summary>
	''' Thrown by an implementation of
	''' <seealso cref="java.lang.instrument.Instrumentation#redefineClasses Instrumentation.redefineClasses"/>
	''' when one of the specified classes cannot be modified.
	''' </summary>
	''' <seealso cref=     java.lang.instrument.Instrumentation#redefineClasses
	''' @since   1.5 </seealso>
	Public Class UnmodifiableClassException
		Inherits Exception

		Private Shadows Const serialVersionUID As Long = 1716652643585309178L

		''' <summary>
		''' Constructs an <code>UnmodifiableClassException</code> with no
		''' detail message.
		''' </summary>
		Public Sub New()
			MyBase.New()
		End Sub

		''' <summary>
		''' Constructs an <code>UnmodifiableClassException</code> with the
		''' specified detail message.
		''' </summary>
		''' <param name="s">   the detail message. </param>
		Public Sub New(  s As String)
			MyBase.New(s)
		End Sub
	End Class

End Namespace