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
	''' Unchecked exception thrown when an attempt is made to construct a channel in 
	''' a group that is shutdown or the completion handler for an I/O operation 
	''' cannot be invoked because the channel group has terminated.
	''' 
	''' @since 1.7
	''' </summary>

	Public Class ShutdownChannelGroupException
		Inherits IllegalStateException

		Private Shadows Const serialVersionUID As Long = -3903801676350154157L

		''' <summary>
		''' Constructs an instance of this class.
		''' </summary>
		Public Sub New()
		End Sub

	End Class

End Namespace