Imports System

'
' * Copyright (c) 1996, 2009, Oracle and/or its affiliates. All rights reserved.
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

Namespace java.beans

	''' <summary>
	''' Thrown when an exception happens during Introspection.
	''' <p>
	''' Typical causes include not being able to map a string class name
	''' to a Class object, not being able to resolve a string method name,
	''' or specifying a method name that has the wrong type signature for
	''' its intended use.
	''' </summary>

	Public Class IntrospectionException
		Inherits Exception

		Private Shadows Const serialVersionUID As Long = -3728150539969542619L

		''' <summary>
		''' Constructs an <code>IntrospectionException</code> with a
		''' detailed message.
		''' </summary>
		''' <param name="mess"> Descriptive message </param>
		Public Sub New(  mess As String)
			MyBase.New(mess)
		End Sub
	End Class

End Namespace