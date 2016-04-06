'
' * Copyright (c) 1996, 1999, Oracle and/or its affiliates. All rights reserved.
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

Namespace java.awt.event


	''' <summary>
	''' The listener interface for receiving adjustment events.
	''' 
	''' @author Amy Fowler
	''' @since 1.1
	''' </summary>
	Public Interface AdjustmentListener
		Inherits java.util.EventListener

		''' <summary>
		''' Invoked when the value of the adjustable has changed.
		''' </summary>
		Sub adjustmentValueChanged(  e As AdjustmentEvent)

	End Interface

End Namespace