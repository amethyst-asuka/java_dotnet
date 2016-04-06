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
	''' Thrown when a particular method cannot be found.
	''' 
	''' @author     unascribed
	''' @since      JDK1.0
	''' </summary>
	Public Class NoSuchMethodException
		Inherits ReflectiveOperationException

		Private Shadows Const serialVersionUID As Long = 5034388446362600923L

		''' <summary>
		''' Constructs a <code>NoSuchMethodException</code> without a detail message.
		''' </summary>
		Public Sub New()
			MyBase.New()
		End Sub

		''' <summary>
		''' Constructs a <code>NoSuchMethodException</code> with a detail message.
		''' </summary>
		''' <param name="s">   the detail message. </param>
		Public Sub New(  s As String)
			MyBase.New(s)
		End Sub
	End Class

End Namespace