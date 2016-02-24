'
' * Copyright (c) 1995, 1998, Oracle and/or its affiliates. All rights reserved.
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
Namespace java.awt.peer


	''' <summary>
	''' The peer interface for <seealso cref="Scrollbar"/>.
	''' 
	''' The peer interfaces are intended only for use in porting
	''' the AWT. They are not intended for use by application
	''' developers, and developers should not implement peers
	''' nor invoke any of the peer methods directly on the peer
	''' instances.
	''' </summary>
	Public Interface ScrollbarPeer
		Inherits ComponentPeer

		''' <summary>
		''' Sets the parameters for the scrollbar.
		''' </summary>
		''' <param name="value"> the current value </param>
		''' <param name="visible"> how much of the whole scale is visible </param>
		''' <param name="minimum"> the minimum value </param>
		''' <param name="maximum"> the maximum value
		''' </param>
		''' <seealso cref= Scrollbar#setValues(int, int, int, int) </seealso>
		Sub setValues(ByVal value As Integer, ByVal visible As Integer, ByVal minimum As Integer, ByVal maximum As Integer)

		''' <summary>
		''' Sets the line increment of the scrollbar.
		''' </summary>
		''' <param name="l"> the line increment
		''' </param>
		''' <seealso cref= Scrollbar#setLineIncrement(int) </seealso>
		WriteOnly Property lineIncrement As Integer

		''' <summary>
		''' Sets the page increment of the scrollbar.
		''' </summary>
		''' <param name="l"> the page increment
		''' </param>
		''' <seealso cref= Scrollbar#setPageIncrement(int) </seealso>
		WriteOnly Property pageIncrement As Integer
	End Interface

End Namespace