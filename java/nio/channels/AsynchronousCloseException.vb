'
' * Copyright (c) 2000, 2007, Oracle and/or its affiliates. All rights reserved.
' *
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
' *
' 

' -- This file was mechanically generated: Do not edit! -- //

Namespace java.nio.channels


	''' <summary>
	''' Checked exception received by a thread when another thread closes the
	''' channel or the part of the channel upon which it is blocked in an I/O
	''' operation.
	''' 
	''' @since 1.4
	''' </summary>

	Public Class AsynchronousCloseException
		Inherits ClosedChannelException

		Private Shadows Const serialVersionUID As Long = 6891178312432313966L

		''' <summary>
		''' Constructs an instance of this class.
		''' </summary>
		Public Sub New()
		End Sub

	End Class

End Namespace