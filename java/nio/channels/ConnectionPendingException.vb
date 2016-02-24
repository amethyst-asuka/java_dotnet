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
	''' Unchecked exception thrown when an attempt is made to connect a {@link
	''' SocketChannel} for which a non-blocking connection operation is already in
	''' progress.
	''' 
	''' @since 1.4
	''' </summary>

	Public Class ConnectionPendingException
		Inherits IllegalStateException

		Private Shadows Const serialVersionUID As Long = 2008393366501760879L

		''' <summary>
		''' Constructs an instance of this class.
		''' </summary>
		Public Sub New()
		End Sub

	End Class

End Namespace