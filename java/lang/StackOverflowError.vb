'
' * Copyright (c) 1994, 2008, Oracle and/or its affiliates. All rights reserved.
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

Namespace java.lang

	''' <summary>
	''' Thrown when a stack overflow occurs because an application
	''' recurses too deeply.
	''' 
	''' @author unascribed
	''' @since   JDK1.0
	''' </summary>
	Public Class StackOverflowError
		Inherits VirtualMachineError

		Private Shadows Const serialVersionUID As Long = 8609175038441759607L

		''' <summary>
		''' Constructs a <code>StackOverflowError</code> with no detail message.
		''' </summary>
		Public Sub New()
			MyBase.New()
		End Sub

		''' <summary>
		''' Constructs a <code>StackOverflowError</code> with the specified
		''' detail message.
		''' </summary>
		''' <param name="s">   the detail message. </param>
		Public Sub New(  s As String)
			MyBase.New(s)
		End Sub
	End Class

End Namespace