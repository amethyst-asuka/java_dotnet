Imports System.Collections.Generic

'
' * Copyright (c) 1997, 2013, Oracle and/or its affiliates. All rights reserved.
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

Namespace java.awt.dnd




	''' <summary>
	''' The <code>DropTargetDragEvent</code> is delivered to a
	''' <code>DropTargetListener</code> via its
	''' dragEnter() and dragOver() methods.
	''' <p>
	''' The <code>DropTargetDragEvent</code> reports the <i>source drop actions</i>
	''' and the <i>user drop action</i> that reflect the current state of
	''' the drag operation.
	''' <p>
	''' <i>Source drop actions</i> is a bitwise mask of <code>DnDConstants</code>
	''' that represents the set of drop actions supported by the drag source for
	''' this drag operation.
	''' <p>
	''' <i>User drop action</i> depends on the drop actions supported by the drag
	''' source and the drop action selected by the user. The user can select a drop
	''' action by pressing modifier keys during the drag operation:
	''' <pre>
	'''   Ctrl + Shift -&gt; ACTION_LINK
	'''   Ctrl         -&gt; ACTION_COPY
	'''   Shift        -&gt; ACTION_MOVE
	''' </pre>
	''' If the user selects a drop action, the <i>user drop action</i> is one of
	''' <code>DnDConstants</code> that represents the selected drop action if this
	''' drop action is supported by the drag source or
	''' <code>DnDConstants.ACTION_NONE</code> if this drop action is not supported
	''' by the drag source.
	''' <p>
	''' If the user doesn't select a drop action, the set of
	''' <code>DnDConstants</code> that represents the set of drop actions supported
	''' by the drag source is searched for <code>DnDConstants.ACTION_MOVE</code>,
	''' then for <code>DnDConstants.ACTION_COPY</code>, then for
	''' <code>DnDConstants.ACTION_LINK</code> and the <i>user drop action</i> is the
	''' first constant found. If no constant is found the <i>user drop action</i>
	''' is <code>DnDConstants.ACTION_NONE</code>.
	''' 
	''' @since 1.2
	''' </summary>

	Public Class DropTargetDragEvent
		Inherits DropTargetEvent

		Private Const serialVersionUID As Long = -8422265619058953682L

		''' <summary>
		''' Construct a <code>DropTargetDragEvent</code> given the
		''' <code>DropTargetContext</code> for this operation,
		''' the location of the "Drag" <code>Cursor</code>'s hotspot
		''' in the <code>Component</code>'s coordinates, the
		''' user drop action, and the source drop actions.
		''' <P> </summary>
		''' <param name="dtc">        The DropTargetContext for this operation </param>
		''' <param name="cursorLocn"> The location of the "Drag" Cursor's
		''' hotspot in Component coordinates </param>
		''' <param name="dropAction"> The user drop action </param>
		''' <param name="srcActions"> The source drop actions
		''' </param>
		''' <exception cref="NullPointerException"> if cursorLocn is null </exception>
		''' <exception cref="IllegalArgumentException"> if dropAction is not one of
		'''         <code>DnDConstants</code>. </exception>
		''' <exception cref="IllegalArgumentException"> if srcActions is not
		'''         a bitwise mask of <code>DnDConstants</code>. </exception>
		''' <exception cref="IllegalArgumentException"> if dtc is <code>null</code>. </exception>

		Public Sub New(ByVal dtc As DropTargetContext, ByVal cursorLocn As java.awt.Point, ByVal dropAction As Integer, ByVal srcActions As Integer)
			MyBase.New(dtc)

			If cursorLocn Is Nothing Then Throw New NullPointerException("cursorLocn")

			If dropAction <> DnDConstants.ACTION_NONE AndAlso dropAction <> DnDConstants.ACTION_COPY AndAlso dropAction <> DnDConstants.ACTION_MOVE AndAlso dropAction <> DnDConstants.ACTION_LINK Then Throw New IllegalArgumentException("dropAction" & dropAction)

			If (srcActions And Not(DnDConstants.ACTION_COPY_OR_MOVE Or DnDConstants.ACTION_LINK)) <> 0 Then Throw New IllegalArgumentException("srcActions")

			location = cursorLocn
			actions = srcActions
			Me.dropAction = dropAction
		End Sub

		''' <summary>
		''' This method returns a <code>Point</code>
		''' indicating the <code>Cursor</code>'s current
		''' location within the <code>Component'</code>s
		''' coordinates.
		''' <P> </summary>
		''' <returns> the current cursor location in
		''' <code>Component</code>'s coords. </returns>

		Public Overridable Property location As java.awt.Point
			Get
				Return location
			End Get
		End Property


		''' <summary>
		''' This method returns the current <code>DataFlavor</code>s from the
		''' <code>DropTargetContext</code>.
		''' <P> </summary>
		''' <returns> current DataFlavors from the DropTargetContext </returns>

		Public Overridable Property currentDataFlavors As java.awt.datatransfer.DataFlavor()
			Get
				Return dropTargetContext.currentDataFlavors
			End Get
		End Property

		''' <summary>
		''' This method returns the current <code>DataFlavor</code>s
		''' as a <code>java.util.List</code>
		''' <P> </summary>
		''' <returns> a <code>java.util.List</code> of the Current <code>DataFlavor</code>s </returns>

		Public Overridable Property currentDataFlavorsAsList As IList(Of java.awt.datatransfer.DataFlavor)
			Get
				Return dropTargetContext.currentDataFlavorsAsList
			End Get
		End Property

		''' <summary>
		''' This method returns a <code>boolean</code> indicating
		''' if the specified <code>DataFlavor</code> is supported.
		''' <P> </summary>
		''' <param name="df"> the <code>DataFlavor</code> to test
		''' <P> </param>
		''' <returns> if a particular DataFlavor is supported </returns>

		Public Overridable Function isDataFlavorSupported(ByVal df As java.awt.datatransfer.DataFlavor) As Boolean
			Return dropTargetContext.isDataFlavorSupported(df)
		End Function

		''' <summary>
		''' This method returns the source drop actions.
		''' </summary>
		''' <returns> the source drop actions </returns>
		Public Overridable Property sourceActions As Integer
			Get
				Return actions
			End Get
		End Property

		''' <summary>
		''' This method returns the user drop action.
		''' </summary>
		''' <returns> the user drop action </returns>
		Public Overridable Property dropAction As Integer
			Get
				Return dropAction
			End Get
		End Property

		''' <summary>
		''' This method returns the Transferable object that represents
		''' the data associated with the current drag operation.
		''' </summary>
		''' <returns> the Transferable associated with the drag operation </returns>
		''' <exception cref="InvalidDnDOperationException"> if the data associated with the drag
		'''         operation is not available
		''' 
		''' @since 1.5 </exception>
		Public Overridable Property transferable As java.awt.datatransfer.Transferable
			Get
				Return dropTargetContext.transferable
			End Get
		End Property

		''' <summary>
		''' Accepts the drag.
		''' 
		''' This method should be called from a
		''' <code>DropTargetListeners</code> <code>dragEnter</code>,
		''' <code>dragOver</code>, and <code>dropActionChanged</code>
		''' methods if the implementation wishes to accept an operation
		''' from the srcActions other than the one selected by
		''' the user as represented by the <code>dropAction</code>.
		''' </summary>
		''' <param name="dragOperation"> the operation accepted by the target </param>
		Public Overridable Sub acceptDrag(ByVal dragOperation As Integer)
			dropTargetContext.acceptDrag(dragOperation)
		End Sub

		''' <summary>
		''' Rejects the drag as a result of examining either the
		''' <code>dropAction</code> or the available <code>DataFlavor</code>
		''' types.
		''' </summary>
		Public Overridable Sub rejectDrag()
			dropTargetContext.rejectDrag()
		End Sub

	'    
	'     * fields
	'     

		''' <summary>
		''' The location of the drag cursor's hotspot in Component coordinates.
		''' 
		''' @serial
		''' </summary>
		Private location As java.awt.Point

		''' <summary>
		''' The source drop actions.
		''' 
		''' @serial
		''' </summary>
		Private actions As Integer

		''' <summary>
		''' The user drop action.
		''' 
		''' @serial
		''' </summary>
		Private dropAction As Integer
	End Class

End Namespace