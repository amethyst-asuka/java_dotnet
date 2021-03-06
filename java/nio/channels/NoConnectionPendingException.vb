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
	''' Unchecked exception thrown when the {@link SocketChannel#finishConnect
	''' finishConnect} method of a <seealso cref="SocketChannel"/> is invoked without first
	''' successfully invoking its <seealso cref="SocketChannel#connect connect"/> method.
	''' 
	''' @since 1.4
	''' </summary>

	Public Class NoConnectionPendingException
		Inherits IllegalStateException

		Private Shadows Const serialVersionUID As Long = -8296561183633134743L

		''' <summary>
		''' Constructs an instance of this class.
		''' </summary>
		Public Sub New()
		End Sub

	End Class

End Namespace