'
' * Copyright (c) 1996, 2005, Oracle and/or its affiliates. All rights reserved.
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

Namespace java.io

	''' <summary>
	''' Thrown when control information that was read from an object stream
	''' violates internal consistency checks.
	''' 
	''' @author  unascribed
	''' @since   JDK1.1
	''' </summary>
	Public Class StreamCorruptedException
		Inherits ObjectStreamException

		Private Shadows Const serialVersionUID As Long = 8983558202217591746L

		''' <summary>
		''' Create a StreamCorruptedException and list a reason why thrown.
		''' </summary>
		''' <param name="reason">  String describing the reason for the exception. </param>
		Public Sub New(  reason As String)
			MyBase.New(reason)
		End Sub

		''' <summary>
		''' Create a StreamCorruptedException and list no reason why thrown.
		''' </summary>
		Public Sub New()
			MyBase.New()
		End Sub
	End Class

End Namespace