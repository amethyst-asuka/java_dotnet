'
' * Copyright (c) 1997, 2007, Oracle and/or its affiliates. All rights reserved.
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

Namespace java.awt.dnd.peer



	''' <summary>
	''' <p>
	''' This interface is supplied by the underlying window system platform to
	''' expose the behaviors of the Drag and Drop system to an originator of
	''' the same
	''' </p>
	''' 
	''' @since 1.2
	''' 
	''' </summary>

	Public Interface DragSourceContextPeer

		''' <summary>
		''' start a drag
		''' </summary>

		Sub startDrag(  dsc As java.awt.dnd.DragSourceContext,   c As java.awt.Cursor,   dragImage As java.awt.Image,   imageOffset As java.awt.Point)

		''' <summary>
		''' return the current drag cursor
		''' </summary>

		Property cursor As java.awt.Cursor



		''' <summary>
		''' notify the peer that the Transferables DataFlavors have changed
		''' </summary>

		Sub transferablesFlavorsChanged()
	End Interface

End Namespace