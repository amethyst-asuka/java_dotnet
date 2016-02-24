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
	''' The <code>DropTargetDropEvent</code> is delivered
	''' via the <code>DropTargetListener</code> drop() method.
	''' <p>
	''' The <code>DropTargetDropEvent</code> reports the <i>source drop actions</i>
	''' and the <i>user drop action</i> that reflect the current state of the
	''' drag-and-drop operation.
	''' <p>
	''' <i>Source drop actions</i> is a bitwise mask of <code>DnDConstants</code>
	''' that represents the set of drop actions supported by the drag source for
	''' this drag-and-drop operation.
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

	Public Class DropTargetDropEvent
		Inherits DropTargetEvent

		Private Const serialVersionUID As Long = -1721911170440459322L

		''' <summary>
		''' Construct a <code>DropTargetDropEvent</code> given
		''' the <code>DropTargetContext</code> for this operation,
		''' the location of the drag <code>Cursor</code>'s
		''' hotspot in the <code>Component</code>'s coordinates,
		''' the currently
		''' selected user drop action, and the current set of
		''' actions supported by the source.
		''' By default, this constructor
		''' assumes that the target is not in the same virtual machine as
		''' the source; that is, <seealso cref="#isLocalTransfer()"/> will
		''' return <code>false</code>.
		''' <P> </summary>
		''' <param name="dtc">        The <code>DropTargetContext</code> for this operation </param>
		''' <param name="cursorLocn"> The location of the "Drag" Cursor's
		''' hotspot in <code>Component</code> coordinates </param>
		''' <param name="dropAction"> the user drop action. </param>
		''' <param name="srcActions"> the source drop actions.
		''' </param>
		''' <exception cref="NullPointerException">
		''' if cursorLocn is <code>null</code> </exception>
		''' <exception cref="IllegalArgumentException">
		'''         if dropAction is not one of  <code>DnDConstants</code>. </exception>
		''' <exception cref="IllegalArgumentException">
		'''         if srcActions is not a bitwise mask of <code>DnDConstants</code>. </exception>
		''' <exception cref="IllegalArgumentException"> if dtc is <code>null</code>. </exception>

		Public Sub New(ByVal dtc As DropTargetContext, ByVal cursorLocn As java.awt.Point, ByVal dropAction As Integer, ByVal srcActions As Integer)
			MyBase.New(dtc)

			If cursorLocn Is Nothing Then Throw New NullPointerException("cursorLocn")

			If dropAction <> DnDConstants.ACTION_NONE AndAlso dropAction <> DnDConstants.ACTION_COPY AndAlso dropAction <> DnDConstants.ACTION_MOVE AndAlso dropAction <> DnDConstants.ACTION_LINK Then Throw New IllegalArgumentException("dropAction = " & dropAction)

			If (srcActions And Not(DnDConstants.ACTION_COPY_OR_MOVE Or DnDConstants.ACTION_LINK)) <> 0 Then Throw New IllegalArgumentException("srcActions")

			location = cursorLocn
			actions = srcActions
			Me.dropAction = dropAction
		End Sub

		''' <summary>
		''' Construct a <code>DropTargetEvent</code> given the
		''' <code>DropTargetContext</code> for this operation,
		''' the location of the drag <code>Cursor</code>'s hotspot
		''' in the <code>Component</code>'s
		''' coordinates, the currently selected user drop action,
		''' the current set of actions supported by the source,
		''' and a <code>boolean</code> indicating if the source is in the same JVM
		''' as the target.
		''' <P> </summary>
		''' <param name="dtc">        The DropTargetContext for this operation </param>
		''' <param name="cursorLocn"> The location of the "Drag" Cursor's
		''' hotspot in Component's coordinates </param>
		''' <param name="dropAction"> the user drop action. </param>
		''' <param name="srcActions"> the source drop actions. </param>
		''' <param name="isLocal">  True if the source is in the same JVM as the target
		''' </param>
		''' <exception cref="NullPointerException">
		'''         if cursorLocn is  <code>null</code> </exception>
		''' <exception cref="IllegalArgumentException">
		'''         if dropAction is not one of <code>DnDConstants</code>. </exception>
		''' <exception cref="IllegalArgumentException"> if srcActions is not a bitwise mask of <code>DnDConstants</code>. </exception>
		''' <exception cref="IllegalArgumentException">  if dtc is <code>null</code>. </exception>

		Public Sub New(ByVal dtc As DropTargetContext, ByVal cursorLocn As java.awt.Point, ByVal dropAction As Integer, ByVal srcActions As Integer, ByVal isLocal As Boolean)
			Me.New(dtc, cursorLocn, dropAction, srcActions)

			isLocalTx = isLocal
		End Sub

		''' <summary>
		''' This method returns a <code>Point</code>
		''' indicating the <code>Cursor</code>'s current
		''' location in the <code>Component</code>'s coordinates.
		''' <P> </summary>
		''' <returns> the current <code>Cursor</code> location in Component's coords. </returns>

		Public Overridable Property location As java.awt.Point
			Get
				Return location
			End Get
		End Property


		''' <summary>
		''' This method returns the current DataFlavors.
		''' <P> </summary>
		''' <returns> current DataFlavors </returns>

		Public Overridable Property currentDataFlavors As java.awt.datatransfer.DataFlavor()
			Get
				Return dropTargetContext.currentDataFlavors
			End Get
		End Property

		''' <summary>
		''' This method returns the currently available
		''' <code>DataFlavor</code>s as a <code>java.util.List</code>.
		''' <P> </summary>
		''' <returns> the currently available DataFlavors as a java.util.List </returns>

		Public Overridable Property currentDataFlavorsAsList As IList(Of java.awt.datatransfer.DataFlavor)
			Get
				Return dropTargetContext.currentDataFlavorsAsList
			End Get
		End Property

		''' <summary>
		''' This method returns a <code>boolean</code> indicating if the
		''' specified <code>DataFlavor</code> is available
		''' from the source.
		''' <P> </summary>
		''' <param name="df"> the <code>DataFlavor</code> to test
		''' <P> </param>
		''' <returns> if the DataFlavor specified is available from the source </returns>

		Public Overridable Function isDataFlavorSupported(ByVal df As java.awt.datatransfer.DataFlavor) As Boolean
			Return dropTargetContext.isDataFlavorSupported(df)
		End Function

		''' <summary>
		''' This method returns the source drop actions.
		''' </summary>
		''' <returns> the source drop actions. </returns>
		Public Overridable Property sourceActions As Integer
			Get
				Return actions
			End Get
		End Property

		''' <summary>
		''' This method returns the user drop action.
		''' </summary>
		''' <returns> the user drop actions. </returns>
		Public Overridable Property dropAction As Integer
			Get
				Return dropAction
			End Get
		End Property

		''' <summary>
		''' This method returns the <code>Transferable</code> object
		''' associated with the drop.
		''' <P> </summary>
		''' <returns> the <code>Transferable</code> associated with the drop </returns>

		Public Overridable Property transferable As java.awt.datatransfer.Transferable
			Get
				Return dropTargetContext.transferable
			End Get
		End Property

		''' <summary>
		''' accept the drop, using the specified action.
		''' <P> </summary>
		''' <param name="dropAction"> the specified action </param>

		Public Overridable Sub acceptDrop(ByVal dropAction As Integer)
			dropTargetContext.acceptDrop(dropAction)
		End Sub

		''' <summary>
		''' reject the Drop.
		''' </summary>

		Public Overridable Sub rejectDrop()
			dropTargetContext.rejectDrop()
		End Sub

		''' <summary>
		''' This method notifies the <code>DragSource</code>
		''' that the drop transfer(s) are completed.
		''' <P> </summary>
		''' <param name="success"> a <code>boolean</code> indicating that the drop transfer(s) are completed. </param>

		Public Overridable Sub dropComplete(ByVal success As Boolean)
			dropTargetContext.dropComplete(success)
		End Sub

		''' <summary>
		''' This method returns an <code>int</code> indicating if
		''' the source is in the same JVM as the target.
		''' <P> </summary>
		''' <returns> if the Source is in the same JVM </returns>

		Public Overridable Property localTransfer As Boolean
			Get
				Return isLocalTx
			End Get
		End Property

	'    
	'     * fields
	'     

		Private Shared ReadOnly zero As New java.awt.Point(0,0)

		''' <summary>
		''' The location of the drag cursor's hotspot in Component coordinates.
		''' 
		''' @serial
		''' </summary>
		Private location As java.awt.Point = zero

		''' <summary>
		''' The source drop actions.
		''' 
		''' @serial
		''' </summary>
		Private actions As Integer = DnDConstants.ACTION_NONE

		''' <summary>
		''' The user drop action.
		''' 
		''' @serial
		''' </summary>
		Private dropAction As Integer = DnDConstants.ACTION_NONE

		''' <summary>
		''' <code>true</code> if the source is in the same JVM as the target.
		''' 
		''' @serial
		''' </summary>
		Private isLocalTx As Boolean = False
	End Class

End Namespace