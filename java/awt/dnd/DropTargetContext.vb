Imports System
Imports System.Collections.Generic

'
' * Copyright (c) 1997, 2004, Oracle and/or its affiliates. All rights reserved.
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
	''' A <code>DropTargetContext</code> is created
	''' whenever the logical cursor associated
	''' with a Drag and Drop operation coincides with the visible geometry of
	''' a <code>Component</code> associated with a <code>DropTarget</code>.
	''' The <code>DropTargetContext</code> provides
	''' the mechanism for a potential receiver
	''' of a drop operation to both provide the end user with the appropriate
	''' drag under feedback, but also to effect the subsequent data transfer
	''' if appropriate.
	''' 
	''' @since 1.2
	''' </summary>

	<Serializable> _
	Public Class DropTargetContext

		Private Const serialVersionUID As Long = -634158968993743371L

		''' <summary>
		''' Construct a <code>DropTargetContext</code>
		''' given a specified <code>DropTarget</code>.
		''' <P> </summary>
		''' <param name="dt"> the DropTarget to associate with </param>

		Friend Sub New(ByVal dt As DropTarget)
			MyBase.New()

			dropTarget = dt
		End Sub

		''' <summary>
		''' This method returns the <code>DropTarget</code> associated with this
		''' <code>DropTargetContext</code>.
		''' <P> </summary>
		''' <returns> the <code>DropTarget</code> associated with this <code>DropTargetContext</code> </returns>

		Public Overridable Property dropTarget As DropTarget
			Get
				Return dropTarget
			End Get
		End Property

		''' <summary>
		''' This method returns the <code>Component</code> associated with
		''' this <code>DropTargetContext</code>.
		''' <P> </summary>
		''' <returns> the Component associated with this Context </returns>

		Public Overridable Property component As java.awt.Component
			Get
				Return dropTarget.component
			End Get
		End Property

		''' <summary>
		''' Called when associated with the <code>DropTargetContextPeer</code>.
		''' <P> </summary>
		''' <param name="dtcp"> the <code>DropTargetContextPeer</code> </param>

		Public Overridable Sub addNotify(ByVal dtcp As java.awt.dnd.peer.DropTargetContextPeer)
			dropTargetContextPeer = dtcp
		End Sub

		''' <summary>
		''' Called when disassociated with the <code>DropTargetContextPeer</code>.
		''' </summary>

		Public Overridable Sub removeNotify()
			dropTargetContextPeer = Nothing
			transferable = Nothing
		End Sub

		''' <summary>
		''' This method sets the current actions acceptable to
		''' this <code>DropTarget</code>.
		''' <P> </summary>
		''' <param name="actions"> an <code>int</code> representing the supported action(s) </param>

		Protected Friend Overridable Property targetActions As Integer
			Set(ByVal actions As Integer)
				Dim peer As java.awt.dnd.peer.DropTargetContextPeer = dropTargetContextPeer
				If peer IsNot Nothing Then
					SyncLock peer
						peer.targetActions = actions
						dropTarget.doSetDefaultActions(actions)
					End SyncLock
				Else
					dropTarget.doSetDefaultActions(actions)
				End If
			End Set
			Get
				Dim peer As java.awt.dnd.peer.DropTargetContextPeer = dropTargetContextPeer
				Return (If(peer IsNot Nothing, peer.targetActions, dropTarget.defaultActions))
			End Get
		End Property



		''' <summary>
		''' This method signals that the drop is completed and
		''' if it was successful or not.
		''' <P> </summary>
		''' <param name="success"> true for success, false if not
		''' <P> </param>
		''' <exception cref="InvalidDnDOperationException"> if a drop is not outstanding/extant </exception>

		Public Overridable Sub dropComplete(ByVal success As Boolean)
			Dim peer As java.awt.dnd.peer.DropTargetContextPeer = dropTargetContextPeer
			If peer IsNot Nothing Then peer.dropComplete(success)
		End Sub

		''' <summary>
		''' accept the Drag.
		''' <P> </summary>
		''' <param name="dragOperation"> the supported action(s) </param>

		Protected Friend Overridable Sub acceptDrag(ByVal dragOperation As Integer)
			Dim peer As java.awt.dnd.peer.DropTargetContextPeer = dropTargetContextPeer
			If peer IsNot Nothing Then peer.acceptDrag(dragOperation)
		End Sub

		''' <summary>
		''' reject the Drag.
		''' </summary>

		Protected Friend Overridable Sub rejectDrag()
			Dim peer As java.awt.dnd.peer.DropTargetContextPeer = dropTargetContextPeer
			If peer IsNot Nothing Then peer.rejectDrag()
		End Sub

		''' <summary>
		''' called to signal that the drop is acceptable
		''' using the specified operation.
		''' must be called during DropTargetListener.drop method invocation.
		''' <P> </summary>
		''' <param name="dropOperation"> the supported action(s) </param>

		Protected Friend Overridable Sub acceptDrop(ByVal dropOperation As Integer)
			Dim peer As java.awt.dnd.peer.DropTargetContextPeer = dropTargetContextPeer
			If peer IsNot Nothing Then peer.acceptDrop(dropOperation)
		End Sub

		''' <summary>
		''' called to signal that the drop is unacceptable.
		''' must be called during DropTargetListener.drop method invocation.
		''' </summary>

		Protected Friend Overridable Sub rejectDrop()
			Dim peer As java.awt.dnd.peer.DropTargetContextPeer = dropTargetContextPeer
			If peer IsNot Nothing Then peer.rejectDrop()
		End Sub

		''' <summary>
		''' get the available DataFlavors of the
		''' <code>Transferable</code> operand of this operation.
		''' <P> </summary>
		''' <returns> a <code>DataFlavor[]</code> containing the
		''' supported <code>DataFlavor</code>s of the
		''' <code>Transferable</code> operand. </returns>

		Protected Friend Overridable Property currentDataFlavors As java.awt.datatransfer.DataFlavor()
			Get
				Dim peer As java.awt.dnd.peer.DropTargetContextPeer = dropTargetContextPeer
				Return If(peer IsNot Nothing, peer.transferDataFlavors, New java.awt.datatransfer.DataFlavor(){})
			End Get
		End Property

		''' <summary>
		''' This method returns a the currently available DataFlavors
		''' of the <code>Transferable</code> operand
		''' as a <code>java.util.List</code>.
		''' <P> </summary>
		''' <returns> the currently available
		''' DataFlavors as a <code>java.util.List</code> </returns>

		Protected Friend Overridable Property currentDataFlavorsAsList As IList(Of java.awt.datatransfer.DataFlavor)
			Get
				Return java.util.Arrays.asList(currentDataFlavors)
			End Get
		End Property

		''' <summary>
		''' This method returns a <code>boolean</code>
		''' indicating if the given <code>DataFlavor</code> is
		''' supported by this <code>DropTargetContext</code>.
		''' <P> </summary>
		''' <param name="df"> the <code>DataFlavor</code>
		''' <P> </param>
		''' <returns> if the <code>DataFlavor</code> specified is supported </returns>

		Protected Friend Overridable Function isDataFlavorSupported(ByVal df As java.awt.datatransfer.DataFlavor) As Boolean
			Return currentDataFlavorsAsList.Contains(df)
		End Function

		''' <summary>
		''' get the Transferable (proxy) operand of this operation
		''' <P> </summary>
		''' <exception cref="InvalidDnDOperationException"> if a drag is not outstanding/extant
		''' <P> </exception>
		''' <returns> the <code>Transferable</code> </returns>

		Protected Friend Overridable Property transferable As java.awt.datatransfer.Transferable
			Get
				Dim peer As java.awt.dnd.peer.DropTargetContextPeer = dropTargetContextPeer
				If peer Is Nothing Then
					Throw New InvalidDnDOperationException
				Else
					If transferable Is Nothing Then
						Dim t As java.awt.datatransfer.Transferable = peer.transferable
						Dim isLocal As Boolean = peer.transferableJVMLocal
						SyncLock Me
							If transferable Is Nothing Then transferable = createTransferableProxy(t, isLocal)
						End SyncLock
					End If
    
					Return transferable
				End If
			End Get
		End Property

		''' <summary>
		''' Get the <code>DropTargetContextPeer</code>
		''' <P> </summary>
		''' <returns> the platform peer </returns>

		Friend Overridable Property dropTargetContextPeer As java.awt.dnd.peer.DropTargetContextPeer
			Get
				Return dropTargetContextPeer
			End Get
		End Property

		''' <summary>
		''' Creates a TransferableProxy to proxy for the specified
		''' Transferable.
		''' </summary>
		''' <param name="t"> the <tt>Transferable</tt> to be proxied </param>
		''' <param name="local"> <tt>true</tt> if <tt>t</tt> represents
		'''        the result of a local drag-n-drop operation. </param>
		''' <returns> the new <tt>TransferableProxy</tt> instance. </returns>
		Protected Friend Overridable Function createTransferableProxy(ByVal t As java.awt.datatransfer.Transferable, ByVal local As Boolean) As java.awt.datatransfer.Transferable
			Return New TransferableProxy(Me, t, local)
		End Function

	''' <summary>
	'''************************************************************************* </summary>


		''' <summary>
		''' <code>TransferableProxy</code> is a helper inner class that implements
		''' <code>Transferable</code> interface and serves as a proxy for another
		''' <code>Transferable</code> object which represents data transfer for
		''' a particular drag-n-drop operation.
		''' <p>
		''' The proxy forwards all requests to the encapsulated transferable
		''' and automatically performs additional conversion on the data
		''' returned by the encapsulated transferable in case of local transfer.
		''' </summary>

		Protected Friend Class TransferableProxy
			Implements java.awt.datatransfer.Transferable

			Private ReadOnly outerInstance As DropTargetContext


			''' <summary>
			''' Constructs a <code>TransferableProxy</code> given
			''' a specified <code>Transferable</code> object representing
			''' data transfer for a particular drag-n-drop operation and
			''' a <code>boolean</code> which indicates whether the
			''' drag-n-drop operation is local (within the same JVM).
			''' <p> </summary>
			''' <param name="t"> the <code>Transferable</code> object </param>
			''' <param name="local"> <code>true</code>, if <code>t</code> represents
			'''        the result of local drag-n-drop operation </param>
			Friend Sub New(ByVal outerInstance As DropTargetContext, ByVal t As java.awt.datatransfer.Transferable, ByVal local As Boolean)
					Me.outerInstance = outerInstance
				proxy = New sun.awt.datatransfer.TransferableProxy(t, local)
				transferable = t
				isLocal = local
			End Sub

			''' <summary>
			''' Returns an array of DataFlavor objects indicating the flavors
			''' the data can be provided in by the encapsulated transferable.
			''' <p> </summary>
			''' <returns> an array of data flavors in which the data can be
			'''         provided by the encapsulated transferable </returns>
			Public Overridable Property transferDataFlavors As java.awt.datatransfer.DataFlavor()
				Get
					Return proxy.transferDataFlavors
				End Get
			End Property

			''' <summary>
			''' Returns whether or not the specified data flavor is supported by
			''' the encapsulated transferable. </summary>
			''' <param name="flavor"> the requested flavor for the data </param>
			''' <returns> <code>true</code> if the data flavor is supported,
			'''         <code>false</code> otherwise </returns>
			Public Overridable Function isDataFlavorSupported(ByVal flavor As java.awt.datatransfer.DataFlavor) As Boolean
				Return proxy.isDataFlavorSupported(flavor)
			End Function

			''' <summary>
			''' Returns an object which represents the data provided by
			''' the encapsulated transferable for the requested data flavor.
			''' <p>
			''' In case of local transfer a serialized copy of the object
			''' returned by the encapsulated transferable is provided when
			''' the data is requested in application/x-java-serialized-object
			''' data flavor.
			''' </summary>
			''' <param name="df"> the requested flavor for the data </param>
			''' <exception cref="IOException"> if the data is no longer available
			'''              in the requested flavor. </exception>
			''' <exception cref="UnsupportedFlavorException"> if the requested data flavor is
			'''              not supported. </exception>
			Public Overridable Function getTransferData(ByVal df As java.awt.datatransfer.DataFlavor) As Object
				Return proxy.getTransferData(df)
			End Function

	'        
	'         * fields
	'         

			' We don't need to worry about client code changing the values of
			' these variables. Since TransferableProxy is a protected [Class], only
			' subclasses of DropTargetContext can access it. And DropTargetContext
			' cannot be subclassed by client code because it does not have a
			' public constructor.

			''' <summary>
			''' The encapsulated <code>Transferable</code> object.
			''' </summary>
			Protected Friend transferable As java.awt.datatransfer.Transferable

			''' <summary>
			''' A <code>boolean</code> indicating if the encapsulated
			''' <code>Transferable</code> object represents the result
			''' of local drag-n-drop operation (within the same JVM).
			''' </summary>
			Protected Friend isLocal As Boolean

			Private proxy As sun.awt.datatransfer.TransferableProxy
		End Class

	''' <summary>
	'''************************************************************************* </summary>

	'    
	'     * fields
	'     

		''' <summary>
		''' The DropTarget associated with this DropTargetContext.
		''' 
		''' @serial
		''' </summary>
		Private dropTarget As DropTarget

		<NonSerialized> _
		Private dropTargetContextPeer As java.awt.dnd.peer.DropTargetContextPeer

		<NonSerialized> _
		Private transferable As java.awt.datatransfer.Transferable
	End Class

End Namespace