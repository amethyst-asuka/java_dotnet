'
' * Copyright (c) 1997, 1999, Oracle and/or its affiliates. All rights reserved.
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
	''' The DropTargetPeer class is the interface to the platform dependent
	''' DnD facilities. Since the DnD system is based on the native platform's
	''' facilities, a DropTargetPeer will be associated with a ComponentPeer
	''' of the nearsest enclosing native Container (in the case of lightweights)
	''' </p>
	''' 
	''' @since 1.2
	''' 
	''' </summary>

	Public Interface DropTargetPeer

		''' <summary>
		''' Add the DropTarget to the System
		''' </summary>
		''' <param name="dt"> The DropTarget effected </param>

		Sub addDropTarget(ByVal dt As java.awt.dnd.DropTarget)

		''' <summary>
		''' Remove the DropTarget from the system
		''' </summary>
		''' <param name="dt"> The DropTarget effected </param>

		Sub removeDropTarget(ByVal dt As java.awt.dnd.DropTarget)
	End Interface

End Namespace