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
	''' Unchecked exception thrown when an attempt is made to register a channel
	''' with a selector that was not created by the provider that created the
	''' channel.
	''' 
	''' @since 1.4
	''' </summary>

	Public Class IllegalSelectorException
		Inherits IllegalArgumentException

		Private Shadows Const serialVersionUID As Long = -8406323347253320987L

		''' <summary>
		''' Constructs an instance of this class.
		''' </summary>
		Public Sub New()
		End Sub

	End Class

End Namespace