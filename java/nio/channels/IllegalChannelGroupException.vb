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
	''' Unchecked exception thrown when an attempt is made to open a channel
	''' in a group that was not created by the same provider. 
	''' 
	''' @since 1.7
	''' </summary>

	Public Class IllegalChannelGroupException
		Inherits IllegalArgumentException

		Private Shadows Const serialVersionUID As Long = -2495041211157744253L

		''' <summary>
		''' Constructs an instance of this class.
		''' </summary>
		Public Sub New()
		End Sub

	End Class

End Namespace