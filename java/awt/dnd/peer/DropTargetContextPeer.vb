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
	''' This interface is exposed by the underlying window system platform to
	''' enable control of platform DnD operations
	''' </p>
	''' 
	''' @since 1.2
	''' 
	''' </summary>

	Public Interface DropTargetContextPeer

		''' <summary>
		''' update the peer's notion of the Target's actions
		''' </summary>

		Property targetActions As Integer



		''' <summary>
		''' get the DropTarget associated with this peer
		''' </summary>

		ReadOnly Property dropTarget As java.awt.dnd.DropTarget

		''' <summary>
		''' get the (remote) DataFlavors from the peer
		''' </summary>

		ReadOnly Property transferDataFlavors As java.awt.datatransfer.DataFlavor()

		''' <summary>
		''' get an input stream to the remote data
		''' </summary>

		ReadOnly Property transferable As java.awt.datatransfer.Transferable

		''' <returns> if the DragSource Transferable is in the same JVM as the Target </returns>

		ReadOnly Property transferableJVMLocal As Boolean

		''' <summary>
		''' accept the Drag
		''' </summary>

		Sub acceptDrag(ByVal dragAction As Integer)

		''' <summary>
		''' reject the Drag
		''' </summary>

		Sub rejectDrag()

		''' <summary>
		''' accept the Drop
		''' </summary>

		Sub acceptDrop(ByVal dropAction As Integer)

		''' <summary>
		''' reject the Drop
		''' </summary>

		Sub rejectDrop()

		''' <summary>
		''' signal complete
		''' </summary>

		Sub dropComplete(ByVal success As Boolean)

	End Interface

End Namespace