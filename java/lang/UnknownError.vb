'
' * Copyright (c) 1995, 2008, Oracle and/or its affiliates. All rights reserved.
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
	''' Thrown when an unknown but serious exception has occurred in the
	''' Java Virtual Machine.
	''' 
	''' @author unascribed
	''' @since   JDK1.0
	''' </summary>
	Public Class UnknownError
		Inherits VirtualMachineError

		Private Shadows Const serialVersionUID As Long = 2524784860676771849L

		''' <summary>
		''' Constructs an <code>UnknownError</code> with no detail message.
		''' </summary>
		Public Sub New()
			MyBase.New()
		End Sub

		''' <summary>
		''' Constructs an <code>UnknownError</code> with the specified detail
		''' message.
		''' </summary>
		''' <param name="s">   the detail message. </param>
		Public Sub New(  s As String)
			MyBase.New(s)
		End Sub
	End Class

End Namespace