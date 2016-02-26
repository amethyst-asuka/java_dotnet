Imports Microsoft.VisualBasic
Imports System
Imports System.Diagnostics
Imports javax.swing.event
Imports sun.swing

'
' * Copyright (c) 2000, 2013, Oracle and/or its affiliates. All rights reserved.
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
Namespace javax.swing






	''' <summary>
	''' This class is used to handle the transfer of a <code>Transferable</code>
	''' to and from Swing components.  The <code>Transferable</code> is used to
	''' represent data that is exchanged via a cut, copy, or paste
	''' to/from a clipboard.  It is also used in drag-and-drop operations
	''' to represent a drag from a component, and a drop to a component.
	''' Swing provides functionality that automatically supports cut, copy,
	''' and paste keyboard bindings that use the functionality provided by
	''' an implementation of this class.  Swing also provides functionality
	''' that automatically supports drag and drop that uses the functionality
	''' provided by an implementation of this class.  The Swing developer can
	''' concentrate on specifying the semantics of a transfer primarily by setting
	''' the <code>transferHandler</code> property on a Swing component.
	''' <p>
	''' This class is implemented to provide a default behavior of transferring
	''' a component property simply by specifying the name of the property in
	''' the constructor.  For example, to transfer the foreground color from
	''' one component to another either via the clipboard or a drag and drop operation
	''' a <code>TransferHandler</code> can be constructed with the string "foreground".  The
	''' built in support will use the color returned by <code>getForeground</code> as the source
	''' of the transfer, and <code>setForeground</code> for the target of a transfer.
	''' <p>
	''' Please see
	''' <a href="https://docs.oracle.com/javase/tutorial/uiswing/dnd/index.html">
	''' How to Use Drag and Drop and Data Transfer</a>,
	''' a section in <em>The Java Tutorial</em>, for more information.
	''' 
	''' 
	''' @author Timothy Prinzing
	''' @author Shannon Hickey
	''' @since 1.4
	''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
	<Serializable> _
	Public Class TransferHandler

		''' <summary>
		''' An <code>int</code> representing no transfer action.
		''' </summary>
		Public Shared ReadOnly NONE As Integer = DnDConstants.ACTION_NONE

		''' <summary>
		''' An <code>int</code> representing a &quot;copy&quot; transfer action.
		''' This value is used when data is copied to a clipboard
		''' or copied elsewhere in a drag and drop operation.
		''' </summary>
		Public Shared ReadOnly COPY As Integer = DnDConstants.ACTION_COPY

		''' <summary>
		''' An <code>int</code> representing a &quot;move&quot; transfer action.
		''' This value is used when data is moved to a clipboard (i.e. a cut)
		''' or moved elsewhere in a drag and drop operation.
		''' </summary>
		Public Shared ReadOnly MOVE As Integer = DnDConstants.ACTION_MOVE

		''' <summary>
		''' An <code>int</code> representing a source action capability of either
		''' &quot;copy&quot; or &quot;move&quot;.
		''' </summary>
		Public Shared ReadOnly COPY_OR_MOVE As Integer = DnDConstants.ACTION_COPY_OR_MOVE

		''' <summary>
		''' An <code>int</code> representing a &quot;link&quot; transfer action.
		''' This value is used to specify that data should be linked in a drag
		''' and drop operation.
		''' </summary>
		''' <seealso cref= java.awt.dnd.DnDConstants#ACTION_LINK
		''' @since 1.6 </seealso>
		Public Shared ReadOnly LINK As Integer = DnDConstants.ACTION_LINK

		''' <summary>
		''' An interface to tag things with a {@code getTransferHandler} method.
		''' </summary>
		Friend Interface HasGetTransferHandler

			''' <summary>
			''' Returns the {@code TransferHandler}.
			''' </summary>
			''' <returns> The {@code TransferHandler} or {@code null} </returns>
			ReadOnly Property transferHandler As TransferHandler
		End Interface

		''' <summary>
		''' Represents a location where dropped data should be inserted.
		''' This is a base class that only encapsulates a point.
		''' Components supporting drop may provide subclasses of this
		''' containing more information.
		''' <p>
		''' Developers typically shouldn't create instances of, or extend, this
		''' class. Instead, these are something provided by the DnD
		''' implementation by <code>TransferSupport</code> instances and by
		''' components with a <code>getDropLocation()</code> method.
		''' </summary>
		''' <seealso cref= javax.swing.TransferHandler.TransferSupport#getDropLocation
		''' @since 1.6 </seealso>
		Public Class DropLocation
			Private ReadOnly dropPoint As Point

			''' <summary>
			''' Constructs a drop location for the given point.
			''' </summary>
			''' <param name="dropPoint"> the drop point, representing the mouse's
			'''        current location within the component. </param>
			''' <exception cref="IllegalArgumentException"> if the point
			'''         is <code>null</code> </exception>
			Protected Friend Sub New(ByVal dropPoint As Point)
				If dropPoint Is Nothing Then Throw New System.ArgumentException("Point cannot be null")

				Me.dropPoint = New Point(dropPoint)
			End Sub

			''' <summary>
			''' Returns the drop point, representing the mouse's
			''' current location within the component.
			''' </summary>
			''' <returns> the drop point. </returns>
			Public Property dropPoint As Point
				Get
					Return New Point(dropPoint)
				End Get
			End Property

			''' <summary>
			''' Returns a string representation of this drop location.
			''' This method is intended to be used for debugging purposes,
			''' and the content and format of the returned string may vary
			''' between implementations.
			''' </summary>
			''' <returns> a string representation of this drop location </returns>
			Public Overrides Function ToString() As String
				Return Me.GetType().name & "[dropPoint=" & dropPoint & "]"
			End Function
		End Class

		''' <summary>
		''' This class encapsulates all relevant details of a clipboard
		''' or drag and drop transfer, and also allows for customizing
		''' aspects of the drag and drop experience.
		''' <p>
		''' The main purpose of this class is to provide the information
		''' needed by a developer to determine the suitability of a
		''' transfer or to import the data contained within. But it also
		''' doubles as a controller for customizing properties during drag
		''' and drop, such as whether or not to show the drop location,
		''' and which drop action to use.
		''' <p>
		''' Developers typically need not create instances of this
		''' class. Instead, they are something provided by the DnD
		''' implementation to certain methods in <code>TransferHandler</code>.
		''' </summary>
		''' <seealso cref= #canImport(TransferHandler.TransferSupport) </seealso>
		''' <seealso cref= #importData(TransferHandler.TransferSupport)
		''' @since 1.6 </seealso>
		Public NotInheritable Class TransferSupport
			Private ___isDrop As Boolean
			Private component As Component

			Private showDropLocationIsSet As Boolean
			Private showDropLocation As Boolean

			Private dropAction As Integer = -1

			''' <summary>
			''' The source is a {@code DropTargetDragEvent} or
			''' {@code DropTargetDropEvent} for drops,
			''' and a {@code Transferable} otherwise
			''' </summary>
			Private source As Object

			Private dropLocation As DropLocation

			''' <summary>
			''' Create a <code>TransferSupport</code> with <code>isDrop()</code>
			''' <code>true</code> for the given component, event, and index.
			''' </summary>
			''' <param name="component"> the target component </param>
			''' <param name="event"> a <code>DropTargetEvent</code> </param>
			Private Sub New(ByVal component As Component, ByVal [event] As DropTargetEvent)

				___isDrop = True
				dNDVariablesles(component, [event])
			End Sub

			''' <summary>
			''' Create a <code>TransferSupport</code> with <code>isDrop()</code>
			''' <code>false</code> for the given component and
			''' <code>Transferable</code>.
			''' </summary>
			''' <param name="component"> the target component </param>
			''' <param name="transferable"> the transferable </param>
			''' <exception cref="NullPointerException"> if either parameter
			'''         is <code>null</code> </exception>
			Public Sub New(ByVal component As Component, ByVal transferable As Transferable)
				If component Is Nothing Then Throw New NullPointerException("component is null")

				If transferable Is Nothing Then Throw New NullPointerException("transferable is null")

				___isDrop = False
				Me.component = component
				Me.source = transferable
			End Sub

			''' <summary>
			''' Allows for a single instance to be reused during DnD.
			''' </summary>
			''' <param name="component"> the target component </param>
			''' <param name="event"> a <code>DropTargetEvent</code> </param>
			Private Sub setDNDVariables(ByVal component As Component, ByVal [event] As DropTargetEvent)

				Debug.Assert(___isDrop)

				Me.component = component
				Me.source = [event]
				dropLocation = Nothing
				dropAction = -1
				showDropLocationIsSet = False

				If source Is Nothing Then Return

				Debug.Assert(TypeOf source Is DropTargetDragEvent OrElse TypeOf source Is DropTargetDropEvent)

				Dim p As Point = If(TypeOf source Is DropTargetDragEvent, CType(source, DropTargetDragEvent).location, CType(source, DropTargetDropEvent).location)

				If sun.awt.SunToolkit.isInstanceOf(component, "javax.swing.text.JTextComponent") Then
					dropLocation = SwingAccessor.jTextComponentAccessor.dropLocationForPoint(CType(component, javax.swing.text.JTextComponent), p)
				ElseIf TypeOf component Is JComponent Then
					dropLocation = CType(component, JComponent).dropLocationForPoint(p)
				End If

	'            
	'             * The drop location may be null at this point if the component
	'             * doesn't return custom drop locations. In this case, a point-only
	'             * drop location will be created lazily when requested.
	'             
			End Sub

			''' <summary>
			''' Returns whether or not this <code>TransferSupport</code>
			''' represents a drop operation.
			''' </summary>
			''' <returns> <code>true</code> if this is a drop operation,
			'''         <code>false</code> otherwise. </returns>
			Public Property drop As Boolean
				Get
					Return ___isDrop
				End Get
			End Property

			''' <summary>
			''' Returns the target component of this transfer.
			''' </summary>
			''' <returns> the target component </returns>
			Public Property component As Component
				Get
					Return component
				End Get
			End Property

			''' <summary>
			''' Checks that this is a drop and throws an
			''' {@code IllegalStateException} if it isn't.
			''' </summary>
			''' <exception cref="IllegalStateException"> if {@code isDrop} is false. </exception>
			Private Sub assureIsDrop()
				If Not ___isDrop Then Throw New IllegalStateException("Not a drop")
			End Sub

			''' <summary>
			''' Returns the current (non-{@code null}) drop location for the component,
			''' when this {@code TransferSupport} represents a drop.
			''' <p>
			''' Note: For components with built-in drop support, this location
			''' will be a subclass of {@code DropLocation} of the same type
			''' returned by that component's {@code getDropLocation} method.
			''' <p>
			''' This method is only for use with drag and drop transfers.
			''' Calling it when {@code isDrop()} is {@code false} results
			''' in an {@code IllegalStateException}.
			''' </summary>
			''' <returns> the drop location </returns>
			''' <exception cref="IllegalStateException"> if this is not a drop </exception>
			''' <seealso cref= #isDrop() </seealso>
			Public Property dropLocation As DropLocation
				Get
					assureIsDrop()
    
					If dropLocation Is Nothing Then
		'                
		'                 * component didn't give us a custom drop location,
		'                 * so lazily create a point-only location
		'                 
						Dim p As Point = If(TypeOf source Is DropTargetDragEvent, CType(source, DropTargetDragEvent).location, CType(source, DropTargetDropEvent).location)
    
						dropLocation = New DropLocation(p)
					End If
    
					Return dropLocation
				End Get
			End Property

			''' <summary>
			''' Sets whether or not the drop location should be visually indicated
			''' for the transfer - which must represent a drop. This is applicable to
			''' those components that automatically
			''' show the drop location when appropriate during a drag and drop
			''' operation). By default, the drop location is shown only when the
			''' {@code TransferHandler} has said it can accept the import represented
			''' by this {@code TransferSupport}. With this method you can force the
			''' drop location to always be shown, or always not be shown.
			''' <p>
			''' This method is only for use with drag and drop transfers.
			''' Calling it when {@code isDrop()} is {@code false} results
			''' in an {@code IllegalStateException}.
			''' </summary>
			''' <param name="showDropLocation"> whether or not to indicate the drop location </param>
			''' <exception cref="IllegalStateException"> if this is not a drop </exception>
			''' <seealso cref= #isDrop() </seealso>
			Public Property showDropLocation As Boolean
				Set(ByVal showDropLocation As Boolean)
					assureIsDrop()
    
					Me.showDropLocation = showDropLocation
					Me.showDropLocationIsSet = True
				End Set
			End Property

			''' <summary>
			''' Sets the drop action for the transfer - which must represent a drop
			''' - to the given action,
			''' instead of the default user drop action. The action must be
			''' supported by the source's drop actions, and must be one
			''' of {@code COPY}, {@code MOVE} or {@code LINK}.
			''' <p>
			''' This method is only for use with drag and drop transfers.
			''' Calling it when {@code isDrop()} is {@code false} results
			''' in an {@code IllegalStateException}.
			''' </summary>
			''' <param name="dropAction"> the drop action </param>
			''' <exception cref="IllegalStateException"> if this is not a drop </exception>
			''' <exception cref="IllegalArgumentException"> if an invalid action is specified </exception>
			''' <seealso cref= #getDropAction </seealso>
			''' <seealso cref= #getUserDropAction </seealso>
			''' <seealso cref= #getSourceDropActions </seealso>
			''' <seealso cref= #isDrop() </seealso>
			Public Property dropAction As Integer
				Set(ByVal dropAction As Integer)
					assureIsDrop()
    
					Dim action As Integer = dropAction And sourceDropActions
    
					If Not(action = COPY OrElse action = MOVE OrElse action = LINK) Then Throw New System.ArgumentException("unsupported drop action: " & dropAction)
    
					Me.dropAction = dropAction
				End Set
				Get
					Return If(dropAction = -1, userDropAction, dropAction)
				End Get
			End Property


			''' <summary>
			''' Returns the user drop action for the drop, when this
			''' {@code TransferSupport} represents a drop.
			''' <p>
			''' The user drop action is chosen for a drop as described in the
			''' documentation for <seealso cref="java.awt.dnd.DropTargetDragEvent"/> and
			''' <seealso cref="java.awt.dnd.DropTargetDropEvent"/>. A different action
			''' may be chosen as the drop action by way of the {@code setDropAction}
			''' method.
			''' <p>
			''' You may wish to query this in {@code TransferHandler}'s
			''' {@code canImport} method when determining the suitability of a
			''' drop or when deciding on a drop action to explicitly choose.
			''' <p>
			''' This method is only for use with drag and drop transfers.
			''' Calling it when {@code isDrop()} is {@code false} results
			''' in an {@code IllegalStateException}.
			''' </summary>
			''' <returns> the user drop action </returns>
			''' <exception cref="IllegalStateException"> if this is not a drop </exception>
			''' <seealso cref= #setDropAction </seealso>
			''' <seealso cref= #getDropAction </seealso>
			''' <seealso cref= #isDrop() </seealso>
			Public Property userDropAction As Integer
				Get
					assureIsDrop()
    
					Return If(TypeOf source Is DropTargetDragEvent, CType(source, DropTargetDragEvent).dropAction, CType(source, DropTargetDropEvent).dropAction)
				End Get
			End Property

			''' <summary>
			''' Returns the drag source's supported drop actions, when this
			''' {@code TransferSupport} represents a drop.
			''' <p>
			''' The source actions represent the set of actions supported by the
			''' source of this transfer, and are represented as some bitwise-OR
			''' combination of {@code COPY}, {@code MOVE} and {@code LINK}.
			''' You may wish to query this in {@code TransferHandler}'s
			''' {@code canImport} method when determining the suitability of a drop
			''' or when deciding on a drop action to explicitly choose. To determine
			''' if a particular action is supported by the source, bitwise-AND
			''' the action with the source drop actions, and then compare the result
			''' against the original action. For example:
			''' <pre>
			''' boolean copySupported = (COPY &amp; getSourceDropActions()) == COPY;
			''' </pre>
			''' <p>
			''' This method is only for use with drag and drop transfers.
			''' Calling it when {@code isDrop()} is {@code false} results
			''' in an {@code IllegalStateException}.
			''' </summary>
			''' <returns> the drag source's supported drop actions </returns>
			''' <exception cref="IllegalStateException"> if this is not a drop </exception>
			''' <seealso cref= #isDrop() </seealso>
			Public Property sourceDropActions As Integer
				Get
					assureIsDrop()
    
					Return If(TypeOf source Is DropTargetDragEvent, CType(source, DropTargetDragEvent).sourceActions, CType(source, DropTargetDropEvent).sourceActions)
				End Get
			End Property

			''' <summary>
			''' Returns the data flavors for this transfer.
			''' </summary>
			''' <returns> the data flavors for this transfer </returns>
			Public Property dataFlavors As DataFlavor()
				Get
					If ___isDrop Then
						If TypeOf source Is DropTargetDragEvent Then
							Return CType(source, DropTargetDragEvent).currentDataFlavors
						Else
							Return CType(source, DropTargetDropEvent).currentDataFlavors
						End If
					End If
    
					Return CType(source, Transferable).transferDataFlavors
				End Get
			End Property

			''' <summary>
			''' Returns whether or not the given data flavor is supported.
			''' </summary>
			''' <param name="df"> the <code>DataFlavor</code> to test </param>
			''' <returns> whether or not the given flavor is supported. </returns>
			Public Function isDataFlavorSupported(ByVal df As DataFlavor) As Boolean
				If ___isDrop Then
					If TypeOf source Is DropTargetDragEvent Then
						Return CType(source, DropTargetDragEvent).isDataFlavorSupported(df)
					Else
						Return CType(source, DropTargetDropEvent).isDataFlavorSupported(df)
					End If
				End If

				Return CType(source, Transferable).isDataFlavorSupported(df)
			End Function

			''' <summary>
			''' Returns the <code>Transferable</code> associated with this transfer.
			''' <p>
			''' Note: Unless it is necessary to fetch the <code>Transferable</code>
			''' directly, use one of the other methods on this class to inquire about
			''' the transfer. This may perform better than fetching the
			''' <code>Transferable</code> and asking it directly.
			''' </summary>
			''' <returns> the <code>Transferable</code> associated with this transfer </returns>
			Public Property transferable As Transferable
				Get
					If ___isDrop Then
						If TypeOf source Is DropTargetDragEvent Then
							Return CType(source, DropTargetDragEvent).transferable
						Else
							Return CType(source, DropTargetDropEvent).transferable
						End If
					End If
    
					Return CType(source, Transferable)
				End Get
			End Property
		End Class


		''' <summary>
		''' Returns an {@code Action} that performs cut operations to the
		''' clipboard. When performed, this action operates on the {@code JComponent}
		''' source of the {@code ActionEvent} by invoking {@code exportToClipboard},
		''' with a {@code MOVE} action, on the component's {@code TransferHandler}.
		''' </summary>
		''' <returns> an {@code Action} for performing cuts to the clipboard </returns>
		Public Property Shared cutAction As Action
			Get
				Return cutAction
			End Get
		End Property

		''' <summary>
		''' Returns an {@code Action} that performs copy operations to the
		''' clipboard. When performed, this action operates on the {@code JComponent}
		''' source of the {@code ActionEvent} by invoking {@code exportToClipboard},
		''' with a {@code COPY} action, on the component's {@code TransferHandler}.
		''' </summary>
		''' <returns> an {@code Action} for performing copies to the clipboard </returns>
		Public Property Shared copyAction As Action
			Get
				Return copyAction
			End Get
		End Property

		''' <summary>
		''' Returns an {@code Action} that performs paste operations from the
		''' clipboard. When performed, this action operates on the {@code JComponent}
		''' source of the {@code ActionEvent} by invoking {@code importData},
		''' with the clipboard contents, on the component's {@code TransferHandler}.
		''' </summary>
		''' <returns> an {@code Action} for performing pastes from the clipboard </returns>
		Public Property Shared pasteAction As Action
			Get
				Return pasteAction
			End Get
		End Property


		''' <summary>
		''' Constructs a transfer handler that can transfer a Java Bean property
		''' from one component to another via the clipboard or a drag and drop
		''' operation.
		''' </summary>
		''' <param name="property">  the name of the property to transfer; this can
		'''  be <code>null</code> if there is no property associated with the transfer
		'''  handler (a subclass that performs some other kind of transfer, for example) </param>
		Public Sub New(ByVal [property] As String)
			propertyName = [property]
		End Sub

		''' <summary>
		''' Convenience constructor for subclasses.
		''' </summary>
		Protected Friend Sub New()
			Me.New(Nothing)
		End Sub


		''' <summary>
		''' image for the {@code startDrag} method
		''' </summary>
		''' <seealso cref= java.awt.dnd.DragGestureEvent#startDrag(Cursor dragCursor, Image dragImage, Point imageOffset, Transferable transferable, DragSourceListener dsl) </seealso>
		Private dragImage As Image

		''' <summary>
		''' anchor offset for the {@code startDrag} method
		''' </summary>
		''' <seealso cref= java.awt.dnd.DragGestureEvent#startDrag(Cursor dragCursor, Image dragImage, Point imageOffset, Transferable transferable, DragSourceListener dsl) </seealso>
		Private dragImageOffset As Point

		''' <summary>
		''' Sets the drag image parameter. The image has to be prepared
		''' for rendering by the moment of the call. The image is stored
		''' by reference because of some performance reasons.
		''' </summary>
		''' <param name="img"> an image to drag </param>
		Public Overridable Property dragImage As Image
			Set(ByVal img As Image)
				dragImage = img
			End Set
			Get
				Return dragImage
			End Get
		End Property


		''' <summary>
		''' Sets an anchor offset for the image to drag.
		''' It can not be {@code null}.
		''' </summary>
		''' <param name="p"> a {@code Point} object that corresponds
		''' to coordinates of an anchor offset of the image
		''' relative to the upper left corner of the image </param>
		Public Overridable Property dragImageOffset As Point
			Set(ByVal p As Point)
				dragImageOffset = New Point(p)
			End Set
			Get
				If dragImageOffset Is Nothing Then Return New Point(0,0)
				Return New Point(dragImageOffset)
			End Get
		End Property


		''' <summary>
		''' Causes the Swing drag support to be initiated.  This is called by
		''' the various UI implementations in the <code>javax.swing.plaf.basic</code>
		''' package if the dragEnabled property is set on the component.
		''' This can be called by custom UI
		''' implementations to use the Swing drag support.  This method can also be called
		''' by a Swing extension written as a subclass of <code>JComponent</code>
		''' to take advantage of the Swing drag support.
		''' <p>
		''' The transfer <em>will not necessarily</em> have been completed at the
		''' return of this call (i.e. the call does not block waiting for the drop).
		''' The transfer will take place through the Swing implementation of the
		''' <code>java.awt.dnd</code> mechanism, requiring no further effort
		''' from the developer. The <code>exportDone</code> method will be called
		''' when the transfer has completed.
		''' </summary>
		''' <param name="comp">  the component holding the data to be transferred;
		'''              provided to enable sharing of <code>TransferHandler</code>s </param>
		''' <param name="e">     the event that triggered the transfer </param>
		''' <param name="action"> the transfer action initially requested;
		'''               either {@code COPY}, {@code MOVE} or {@code LINK};
		'''               the DnD system may change the action used during the
		'''               course of the drag operation </param>
		Public Overridable Sub exportAsDrag(ByVal comp As JComponent, ByVal e As InputEvent, ByVal action As Integer)
			Dim srcActions As Integer = getSourceActions(comp)

			' only mouse events supported for drag operations
			If Not(TypeOf e Is MouseEvent) OrElse Not(action = COPY OrElse action = MOVE OrElse action = LINK) OrElse (srcActions And action) = 0 Then action = NONE

			If action <> NONE AndAlso (Not GraphicsEnvironment.headless) Then
				If recognizer Is Nothing Then recognizer = New SwingDragGestureRecognizer(New DragHandler)
				recognizer.gestured(comp, CType(e, MouseEvent), srcActions, action)
			Else
				exportDone(comp, Nothing, NONE)
			End If
		End Sub

		''' <summary>
		''' Causes a transfer from the given component to the
		''' given clipboard.  This method is called by the default cut and
		''' copy actions registered in a component's action map.
		''' <p>
		''' The transfer will take place using the <code>java.awt.datatransfer</code>
		''' mechanism, requiring no further effort from the developer. Any data
		''' transfer <em>will</em> be complete and the <code>exportDone</code>
		''' method will be called with the action that occurred, before this method
		''' returns. Should the clipboard be unavailable when attempting to place
		''' data on it, the <code>IllegalStateException</code> thrown by
		''' <seealso cref="Clipboard#setContents(Transferable, ClipboardOwner)"/> will
		''' be propagated through this method. However,
		''' <code>exportDone</code> will first be called with an action
		''' of <code>NONE</code> for consistency.
		''' </summary>
		''' <param name="comp">  the component holding the data to be transferred;
		'''              provided to enable sharing of <code>TransferHandler</code>s </param>
		''' <param name="clip">  the clipboard to transfer the data into </param>
		''' <param name="action"> the transfer action requested; this should
		'''  be a value of either <code>COPY</code> or <code>MOVE</code>;
		'''  the operation performed is the intersection  of the transfer
		'''  capabilities given by getSourceActions and the requested action;
		'''  the intersection may result in an action of <code>NONE</code>
		'''  if the requested action isn't supported </param>
		''' <exception cref="IllegalStateException"> if the clipboard is currently unavailable </exception>
		''' <seealso cref= Clipboard#setContents(Transferable, ClipboardOwner) </seealso>
		Public Overridable Sub exportToClipboard(ByVal comp As JComponent, ByVal clip As Clipboard, ByVal action As Integer)

			If (action = COPY OrElse action = MOVE) AndAlso (getSourceActions(comp) And action) <> 0 Then

				Dim t As Transferable = createTransferable(comp)
				If t IsNot Nothing Then
					Try
						clip.contentsnts(t, Nothing)
						exportDone(comp, t, action)
						Return
					Catch ise As IllegalStateException
						exportDone(comp, t, NONE)
						Throw ise
					End Try
				End If
			End If

			exportDone(comp, Nothing, NONE)
		End Sub

		''' <summary>
		''' Causes a transfer to occur from a clipboard or a drag and
		''' drop operation. The <code>Transferable</code> to be
		''' imported and the component to transfer to are contained
		''' within the <code>TransferSupport</code>.
		''' <p>
		''' While the drag and drop implementation calls {@code canImport}
		''' to determine the suitability of a transfer before calling this
		''' method, the implementation of paste does not. As such, it cannot
		''' be assumed that the transfer is acceptable upon a call to
		''' this method for paste. It is recommended that {@code canImport} be
		''' explicitly called to cover this case.
		''' <p>
		''' Note: The <code>TransferSupport</code> object passed to this method
		''' is only valid for the duration of the method call. It is undefined
		''' what values it may contain after this method returns.
		''' </summary>
		''' <param name="support"> the object containing the details of
		'''        the transfer, not <code>null</code>. </param>
		''' <returns> true if the data was inserted into the component,
		'''         false otherwise </returns>
		''' <exception cref="NullPointerException"> if <code>support</code> is {@code null} </exception>
		''' <seealso cref= #canImport(TransferHandler.TransferSupport)
		''' @since 1.6 </seealso>
		Public Overridable Function importData(ByVal support As TransferSupport) As Boolean
			Return If(TypeOf support.component Is JComponent, importData(CType(support.component, JComponent), support.transferable), False)
		End Function

		''' <summary>
		''' Causes a transfer to a component from a clipboard or a
		''' DND drop operation.  The <code>Transferable</code> represents
		''' the data to be imported into the component.
		''' <p>
		''' Note: Swing now calls the newer version of <code>importData</code>
		''' that takes a <code>TransferSupport</code>, which in turn calls this
		''' method (if the component in the {@code TransferSupport} is a
		''' {@code JComponent}). Developers are encouraged to call and override the
		''' newer version as it provides more information (and is the only
		''' version that supports use with a {@code TransferHandler} set directly
		''' on a {@code JFrame} or other non-{@code JComponent}).
		''' </summary>
		''' <param name="comp">  the component to receive the transfer;
		'''              provided to enable sharing of <code>TransferHandler</code>s </param>
		''' <param name="t">     the data to import </param>
		''' <returns>  true if the data was inserted into the component, false otherwise </returns>
		''' <seealso cref= #importData(TransferHandler.TransferSupport) </seealso>
		Public Overridable Function importData(ByVal comp As JComponent, ByVal t As Transferable) As Boolean
			Dim prop As PropertyDescriptor = getPropertyDescriptor(comp)
			If prop IsNot Nothing Then
				Dim writer As Method = prop.writeMethod
				If writer Is Nothing Then Return False
				Dim params As Type() = writer.parameterTypes
				If params.Length <> 1 Then Return False
				Dim flavor As DataFlavor = getPropertyDataFlavor(params(0), t.transferDataFlavors)
				If flavor IsNot Nothing Then
					Try
						Dim value As Object = t.getTransferData(flavor)
						Dim args As Object() = { value }
						sun.reflect.misc.MethodUtil.invoke(writer, comp, args)
						Return True
					Catch ex As Exception
						Console.Error.WriteLine("Invocation failed")
						' invocation code
					End Try
				End If
			End If
			Return False
		End Function

		''' <summary>
		''' This method is called repeatedly during a drag and drop operation
		''' to allow the developer to configure properties of, and to return
		''' the acceptability of transfers; with a return value of {@code true}
		''' indicating that the transfer represented by the given
		''' {@code TransferSupport} (which contains all of the details of the
		''' transfer) is acceptable at the current time, and a value of {@code false}
		''' rejecting the transfer.
		''' <p>
		''' For those components that automatically display a drop location during
		''' drag and drop, accepting the transfer, by default, tells them to show
		''' the drop location. This can be changed by calling
		''' {@code setShowDropLocation} on the {@code TransferSupport}.
		''' <p>
		''' By default, when the transfer is accepted, the chosen drop action is that
		''' picked by the user via their drag gesture. The developer can override
		''' this and choose a different action, from the supported source
		''' actions, by calling {@code setDropAction} on the {@code TransferSupport}.
		''' <p>
		''' On every call to {@code canImport}, the {@code TransferSupport} contains
		''' fresh state. As such, any properties set on it must be set on every
		''' call. Upon a drop, {@code canImport} is called one final time before
		''' calling into {@code importData}. Any state set on the
		''' {@code TransferSupport} during that last call will be available in
		''' {@code importData}.
		''' <p>
		''' This method is not called internally in response to paste operations.
		''' As such, it is recommended that implementations of {@code importData}
		''' explicitly call this method for such cases and that this method
		''' be prepared to return the suitability of paste operations as well.
		''' <p>
		''' Note: The <code>TransferSupport</code> object passed to this method
		''' is only valid for the duration of the method call. It is undefined
		''' what values it may contain after this method returns.
		''' </summary>
		''' <param name="support"> the object containing the details of
		'''        the transfer, not <code>null</code>. </param>
		''' <returns> <code>true</code> if the import can happen,
		'''         <code>false</code> otherwise </returns>
		''' <exception cref="NullPointerException"> if <code>support</code> is {@code null} </exception>
		''' <seealso cref= #importData(TransferHandler.TransferSupport) </seealso>
		''' <seealso cref= javax.swing.TransferHandler.TransferSupport#setShowDropLocation </seealso>
		''' <seealso cref= javax.swing.TransferHandler.TransferSupport#setDropAction
		''' @since 1.6 </seealso>
		Public Overridable Function canImport(ByVal support As TransferSupport) As Boolean
			Return If(TypeOf support.component Is JComponent, canImport(CType(support.component, JComponent), support.dataFlavors), False)
		End Function

		''' <summary>
		''' Indicates whether a component will accept an import of the given
		''' set of data flavors prior to actually attempting to import it.
		''' <p>
		''' Note: Swing now calls the newer version of <code>canImport</code>
		''' that takes a <code>TransferSupport</code>, which in turn calls this
		''' method (only if the component in the {@code TransferSupport} is a
		''' {@code JComponent}). Developers are encouraged to call and override the
		''' newer version as it provides more information (and is the only
		''' version that supports use with a {@code TransferHandler} set directly
		''' on a {@code JFrame} or other non-{@code JComponent}).
		''' </summary>
		''' <param name="comp">  the component to receive the transfer;
		'''              provided to enable sharing of <code>TransferHandler</code>s </param>
		''' <param name="transferFlavors">  the data formats available </param>
		''' <returns>  true if the data can be inserted into the component, false otherwise </returns>
		''' <seealso cref= #canImport(TransferHandler.TransferSupport) </seealso>
		Public Overridable Function canImport(ByVal comp As JComponent, ByVal transferFlavors As DataFlavor()) As Boolean
			Dim prop As PropertyDescriptor = getPropertyDescriptor(comp)
			If prop IsNot Nothing Then
				Dim writer As Method = prop.writeMethod
				If writer Is Nothing Then Return False
				Dim params As Type() = writer.parameterTypes
				If params.Length <> 1 Then Return False
				Dim flavor As DataFlavor = getPropertyDataFlavor(params(0), transferFlavors)
				If flavor IsNot Nothing Then Return True
			End If
			Return False
		End Function

		''' <summary>
		''' Returns the type of transfer actions supported by the source;
		''' any bitwise-OR combination of {@code COPY}, {@code MOVE}
		''' and {@code LINK}.
		''' <p>
		''' Some models are not mutable, so a transfer operation of {@code MOVE}
		''' should not be advertised in that case. Returning {@code NONE}
		''' disables transfers from the component.
		''' </summary>
		''' <param name="c">  the component holding the data to be transferred;
		'''           provided to enable sharing of <code>TransferHandler</code>s </param>
		''' <returns> {@code COPY} if the transfer property can be found,
		'''          otherwise returns <code>NONE</code> </returns>
		Public Overridable Function getSourceActions(ByVal c As JComponent) As Integer
			Dim prop As PropertyDescriptor = getPropertyDescriptor(c)
			If prop IsNot Nothing Then Return COPY
			Return NONE
		End Function

		''' <summary>
		''' Returns an object that establishes the look of a transfer.  This is
		''' useful for both providing feedback while performing a drag operation and for
		''' representing the transfer in a clipboard implementation that has a visual
		''' appearance.  The implementation of the <code>Icon</code> interface should
		''' not alter the graphics clip or alpha level.
		''' The icon implementation need not be rectangular or paint all of the
		''' bounding rectangle and logic that calls the icons paint method should
		''' not assume the all bits are painted. <code>null</code> is a valid return value
		''' for this method and indicates there is no visual representation provided.
		''' In that case, the calling logic is free to represent the
		''' transferable however it wants.
		''' <p>
		''' The default Swing logic will not do an alpha blended drag animation if
		''' the return is <code>null</code>.
		''' </summary>
		''' <param name="t">  the data to be transferred; this value is expected to have been
		'''  created by the <code>createTransferable</code> method </param>
		''' <returns>  <code>null</code>, indicating
		'''    there is no default visual representation </returns>
		Public Overridable Function getVisualRepresentation(ByVal t As Transferable) As Icon
			Return Nothing
		End Function

		''' <summary>
		''' Creates a <code>Transferable</code> to use as the source for
		''' a data transfer. Returns the representation of the data to
		''' be transferred, or <code>null</code> if the component's
		''' property is <code>null</code>
		''' </summary>
		''' <param name="c">  the component holding the data to be transferred;
		'''              provided to enable sharing of <code>TransferHandler</code>s </param>
		''' <returns>  the representation of the data to be transferred, or
		'''  <code>null</code> if the property associated with <code>c</code>
		'''  is <code>null</code>
		'''  </returns>
		Protected Friend Overridable Function createTransferable(ByVal c As JComponent) As Transferable
			Dim [property] As PropertyDescriptor = getPropertyDescriptor(c)
			If [property] IsNot Nothing Then Return New PropertyTransferable([property], c)
			Return Nothing
		End Function

		''' <summary>
		''' Invoked after data has been exported.  This method should remove
		''' the data that was transferred if the action was <code>MOVE</code>.
		''' <p>
		''' This method is implemented to do nothing since <code>MOVE</code>
		''' is not a supported action of this implementation
		''' (<code>getSourceActions</code> does not include <code>MOVE</code>).
		''' </summary>
		''' <param name="source"> the component that was the source of the data </param>
		''' <param name="data">   The data that was transferred or possibly null
		'''               if the action is <code>NONE</code>. </param>
		''' <param name="action"> the actual action that was performed </param>
		Protected Friend Overridable Sub exportDone(ByVal source As JComponent, ByVal data As Transferable, ByVal action As Integer)
		End Sub

		''' <summary>
		''' Fetches the property descriptor for the property assigned to this transfer
		''' handler on the given component (transfer handler may be shared).  This
		''' returns <code>null</code> if the property descriptor can't be found
		''' or there is an error attempting to fetch the property descriptor.
		''' </summary>
		Private Function getPropertyDescriptor(ByVal comp As JComponent) As PropertyDescriptor
			If propertyName Is Nothing Then Return Nothing
			Dim k As Type = comp.GetType()
			Dim bi As BeanInfo
			Try
				bi = Introspector.getBeanInfo(k)
			Catch ex As IntrospectionException
				Return Nothing
			End Try
			Dim props As PropertyDescriptor() = bi.propertyDescriptors
			For i As Integer = 0 To props.Length - 1
				If propertyName.Equals(props(i).name) Then
					Dim reader As Method = props(i).readMethod

					If reader IsNot Nothing Then
						Dim params As Type() = reader.parameterTypes

						If params Is Nothing OrElse params.Length = 0 Then Return props(i)
					End If
				End If
			Next i
			Return Nothing
		End Function

		''' <summary>
		''' Fetches the data flavor from the array of possible flavors that
		''' has data of the type represented by property type.  Null is
		''' returned if there is no match.
		''' </summary>
		Private Function getPropertyDataFlavor(ByVal k As Type, ByVal flavors As DataFlavor()) As DataFlavor
			For i As Integer = 0 To flavors.Length - 1
				Dim flavor As DataFlavor = flavors(i)
				If "application".Equals(flavor.primaryType) AndAlso "x-java-jvm-local-objectref".Equals(flavor.subType) AndAlso k.IsAssignableFrom(flavor.representationClass) Then Return flavor
			Next i
			Return Nothing
		End Function


		Private propertyName As String
		Private Shared recognizer As SwingDragGestureRecognizer = Nothing

		Private Property Shared dropTargetListener As DropTargetListener
			Get
				SyncLock GetType(DropHandler)
					Dim handler As DropHandler = CType(sun.awt.AppContext.appContext.get(GetType(DropHandler)), DropHandler)
    
					If handler Is Nothing Then
						handler = New DropHandler
						sun.awt.AppContext.appContext.put(GetType(DropHandler), handler)
					End If
    
					Return handler
				End SyncLock
			End Get
		End Property

		Friend Class PropertyTransferable
			Implements Transferable

			Friend Sub New(ByVal p As PropertyDescriptor, ByVal c As JComponent)
				[property] = p
				component = c
			End Sub

			' --- Transferable methods ----------------------------------------------

			''' <summary>
			''' Returns an array of <code>DataFlavor</code> objects indicating the flavors the data
			''' can be provided in.  The array should be ordered according to preference
			''' for providing the data (from most richly descriptive to least descriptive). </summary>
			''' <returns> an array of data flavors in which this data can be transferred </returns>
			Public Overridable Property transferDataFlavors As DataFlavor()
				Get
					Dim flavors As DataFlavor() = New DataFlavor(0){}
					Dim propertyType As Type = [property].propertyType
					Dim mimeType As String = DataFlavor.javaJVMLocalObjectMimeType & ";class=" & propertyType.name
					Try
						flavors(0) = New DataFlavor(mimeType)
					Catch cnfe As ClassNotFoundException
						flavors = New DataFlavor(){}
					End Try
					Return flavors
				End Get
			End Property

			''' <summary>
			''' Returns whether the specified data flavor is supported for
			''' this object. </summary>
			''' <param name="flavor"> the requested flavor for the data </param>
			''' <returns> true if this <code>DataFlavor</code> is supported,
			'''   otherwise false </returns>
			Public Overridable Function isDataFlavorSupported(ByVal flavor As DataFlavor) As Boolean
				Dim propertyType As Type = [property].propertyType
				If "application".Equals(flavor.primaryType) AndAlso "x-java-jvm-local-objectref".Equals(flavor.subType) AndAlso propertyType.IsSubclassOf(flavor.representationClass) Then Return True
				Return False
			End Function

			''' <summary>
			''' Returns an object which represents the data to be transferred.  The class
			''' of the object returned is defined by the representation class of the flavor.
			''' </summary>
			''' <param name="flavor"> the requested flavor for the data </param>
			''' <seealso cref= DataFlavor#getRepresentationClass </seealso>
			''' <exception cref="IOException">                if the data is no longer available
			'''              in the requested flavor. </exception>
			''' <exception cref="UnsupportedFlavorException"> if the requested data flavor is
			'''              not supported. </exception>
			Public Overridable Function getTransferData(ByVal flavor As DataFlavor) As Object
				If Not isDataFlavorSupported(flavor) Then Throw New UnsupportedFlavorException(flavor)
				Dim reader As Method = [property].readMethod
				Dim value As Object = Nothing
				Try
					value = sun.reflect.misc.MethodUtil.invoke(reader, component, CType(Nothing, Object()))
				Catch ex As Exception
					Throw New IOException("Property read failed: " & [property].name)
				End Try
				Return value
			End Function

			Friend component As JComponent
			Friend [property] As PropertyDescriptor
		End Class

		''' <summary>
		''' This is the default drop target for drag and drop operations if
		''' one isn't provided by the developer.  <code>DropTarget</code>
		''' only supports one <code>DropTargetListener</code> and doesn't
		''' function properly if it isn't set.
		''' This class sets the one listener as the linkage of drop handling
		''' to the <code>TransferHandler</code>, and adds support for
		''' additional listeners which some of the <code>ComponentUI</code>
		''' implementations install to manipulate a drop insertion location.
		''' </summary>
		Friend Class SwingDropTarget
			Inherits DropTarget
			Implements javax.swing.plaf.UIResource

			Friend Sub New(ByVal c As Component)
				MyBase.New(c, COPY_OR_MOVE Or LINK, Nothing)
				Try
					' addDropTargetListener is overridden
					' we specifically need to add to the superclass
					MyBase.addDropTargetListener(dropTargetListener)
				Catch tmle As java.util.TooManyListenersException
				End Try
			End Sub

			Public Overridable Sub addDropTargetListener(ByVal dtl As DropTargetListener)
				' Since the super class only supports one DropTargetListener,
				' and we add one from the constructor, we always add to the
				' extended list.
				If listenerList Is Nothing Then listenerList = New EventListenerList
				listenerList.add(GetType(DropTargetListener), dtl)
			End Sub

			Public Overridable Sub removeDropTargetListener(ByVal dtl As DropTargetListener)
				If listenerList IsNot Nothing Then listenerList.remove(GetType(DropTargetListener), dtl)
			End Sub

			' --- DropTargetListener methods (multicast) --------------------------

			Public Overridable Sub dragEnter(ByVal e As DropTargetDragEvent)
				MyBase.dragEnter(e)
				If listenerList IsNot Nothing Then
					Dim listeners As Object() = listenerList.listenerList
					For i As Integer = listeners.Length-2 To 0 Step -2
						If listeners(i) Is GetType(DropTargetListener) Then CType(listeners(i+1), DropTargetListener).dragEnter(e)
					Next i
				End If
			End Sub

			Public Overridable Sub dragOver(ByVal e As DropTargetDragEvent)
				MyBase.dragOver(e)
				If listenerList IsNot Nothing Then
					Dim listeners As Object() = listenerList.listenerList
					For i As Integer = listeners.Length-2 To 0 Step -2
						If listeners(i) Is GetType(DropTargetListener) Then CType(listeners(i+1), DropTargetListener).dragOver(e)
					Next i
				End If
			End Sub

			Public Overridable Sub dragExit(ByVal e As DropTargetEvent)
				MyBase.dragExit(e)
				If listenerList IsNot Nothing Then
					Dim listeners As Object() = listenerList.listenerList
					For i As Integer = listeners.Length-2 To 0 Step -2
						If listeners(i) Is GetType(DropTargetListener) Then CType(listeners(i+1), DropTargetListener).dragExit(e)
					Next i
				End If
				If Not active Then
					' If the Drop target is inactive the dragExit will not be dispatched to the dtListener,
					' so make sure that we clean up the dtListener anyway.
					Dim dtListener As DropTargetListener = dropTargetListener
						If dtListener IsNot Nothing AndAlso TypeOf dtListener Is DropHandler Then CType(dtListener, DropHandler).cleanup(False)
				End If
			End Sub

			Public Overridable Sub drop(ByVal e As DropTargetDropEvent)
				MyBase.drop(e)
				If listenerList IsNot Nothing Then
					Dim listeners As Object() = listenerList.listenerList
					For i As Integer = listeners.Length-2 To 0 Step -2
						If listeners(i) Is GetType(DropTargetListener) Then CType(listeners(i+1), DropTargetListener).drop(e)
					Next i
				End If
			End Sub

			Public Overridable Sub dropActionChanged(ByVal e As DropTargetDragEvent)
				MyBase.dropActionChanged(e)
				If listenerList IsNot Nothing Then
					Dim listeners As Object() = listenerList.listenerList
					For i As Integer = listeners.Length-2 To 0 Step -2
						If listeners(i) Is GetType(DropTargetListener) Then CType(listeners(i+1), DropTargetListener).dropActionChanged(e)
					Next i
				End If
			End Sub

			Private listenerList As EventListenerList
		End Class

		<Serializable> _
		Private Class DropHandler
			Implements DropTargetListener, ActionListener

			Private ___timer As Timer
			Private lastPosition As Point
			Private outer As New Rectangle
			Private inner As New Rectangle
			Private hysteresis As Integer = 10

			Private component As Component
			Private state As Object
			Private support As New TransferSupport(Nothing, CType(Nothing, DropTargetEvent))

			Private Const AUTOSCROLL_INSET As Integer = 10

			''' <summary>
			''' Update the geometry of the autoscroll region.  The geometry is
			''' maintained as a pair of rectangles.  The region can cause
			''' a scroll if the pointer sits inside it for the duration of the
			''' timer.  The region that causes the timer countdown is the area
			''' between the two rectangles.
			''' <p>
			''' This is implemented to use the visible area of the component
			''' as the outer rectangle, and the insets are fixed at 10. Should
			''' the component be smaller than a total of 20 in any direction,
			''' autoscroll will not occur in that direction.
			''' </summary>
			Private Sub updateAutoscrollRegion(ByVal c As JComponent)
				' compute the outer
				Dim visible As Rectangle = c.visibleRect
				outer.boundsnds(visible.x, visible.y, visible.width, visible.height)

				' compute the insets
				Dim i As New Insets(0, 0, 0, 0)
				If TypeOf c Is Scrollable Then
					Dim minSize As Integer = 2 * AUTOSCROLL_INSET

					If visible.width >= minSize Then
							i.right = AUTOSCROLL_INSET
							i.left = i.right
					End If

					If visible.height >= minSize Then
							i.bottom = AUTOSCROLL_INSET
							i.top = i.bottom
					End If
				End If

				' set the inner from the insets
				inner.boundsnds(visible.x + i.left, visible.y + i.top, visible.width - (i.left + i.right), visible.height - (i.top + i.bottom))
			End Sub

			''' <summary>
			''' Perform an autoscroll operation.  This is implemented to scroll by the
			''' unit increment of the Scrollable using scrollRectToVisible.  If the
			''' cursor is in a corner of the autoscroll region, more than one axis will
			''' scroll.
			''' </summary>
			Private Sub autoscroll(ByVal c As JComponent, ByVal pos As Point)
				If TypeOf c Is Scrollable Then
					Dim s As Scrollable = CType(c, Scrollable)
					If pos.y < inner.y Then
						' scroll upward
						Dim dy As Integer = s.getScrollableUnitIncrement(outer, SwingConstants.VERTICAL, -1)
						Dim r As New Rectangle(inner.x, outer.y - dy, inner.width, dy)
						c.scrollRectToVisible(r)
					ElseIf pos.y > (inner.y + inner.height) Then
						' scroll downard
						Dim dy As Integer = s.getScrollableUnitIncrement(outer, SwingConstants.VERTICAL, 1)
						Dim r As New Rectangle(inner.x, outer.y + outer.height, inner.width, dy)
						c.scrollRectToVisible(r)
					End If

					If pos.x < inner.x Then
						' scroll left
						Dim dx As Integer = s.getScrollableUnitIncrement(outer, SwingConstants.HORIZONTAL, -1)
						Dim r As New Rectangle(outer.x - dx, inner.y, dx, inner.height)
						c.scrollRectToVisible(r)
					ElseIf pos.x > (inner.x + inner.width) Then
						' scroll right
						Dim dx As Integer = s.getScrollableUnitIncrement(outer, SwingConstants.HORIZONTAL, 1)
						Dim r As New Rectangle(outer.x + outer.width, inner.y, dx, inner.height)
						c.scrollRectToVisible(r)
					End If
				End If
			End Sub

			''' <summary>
			''' Initializes the internal properties if they haven't been already
			''' inited. This is done lazily to avoid loading of desktop properties.
			''' </summary>
			Private Sub initPropertiesIfNecessary()
				If ___timer Is Nothing Then
					Dim t As Toolkit = Toolkit.defaultToolkit
					Dim prop As Integer?

					prop = CInt(Fix(t.getDesktopProperty("DnD.Autoscroll.interval")))

					___timer = New Timer(If(prop Is Nothing, 100, prop), Me)

					prop = CInt(Fix(t.getDesktopProperty("DnD.Autoscroll.initialDelay")))

					___timer.initialDelay = If(prop Is Nothing, 100, prop)

					prop = CInt(Fix(t.getDesktopProperty("DnD.Autoscroll.cursorHysteresis")))

					If prop IsNot Nothing Then hysteresis = prop
				End If
			End Sub

			''' <summary>
			''' The timer fired, perform autoscroll if the pointer is within the
			''' autoscroll region.
			''' <P> </summary>
			''' <param name="e"> the <code>ActionEvent</code> </param>
			Public Overridable Sub actionPerformed(ByVal e As ActionEvent)
				updateAutoscrollRegion(CType(component, JComponent))
				If outer.contains(lastPosition) AndAlso (Not inner.contains(lastPosition)) Then autoscroll(CType(component, JComponent), lastPosition)
			End Sub

			' --- DropTargetListener methods -----------------------------------

			Private Sub setComponentDropLocation(ByVal support As TransferSupport, ByVal forDrop As Boolean)

				Dim dropLocation As DropLocation = If(support Is Nothing, Nothing, support.dropLocation)

				If sun.awt.SunToolkit.isInstanceOf(component, "javax.swing.text.JTextComponent") Then
					state = SwingAccessor.jTextComponentAccessor.dropLocationion(CType(component, javax.swing.text.JTextComponent), dropLocation, state, forDrop)
				ElseIf TypeOf component Is JComponent Then
					state = CType(component, JComponent).dropLocationion(dropLocation, state, forDrop)
				End If
			End Sub

			Private Sub handleDrag(ByVal e As DropTargetDragEvent)
				Dim importer As TransferHandler = CType(component, HasGetTransferHandler).transferHandler

				If importer Is Nothing Then
					e.rejectDrag()
					componentDropLocationion(Nothing, False)
					Return
				End If

				support.dNDVariablesles(component, e)
				Dim canImport As Boolean = importer.canImport(support)

				If canImport Then
					e.acceptDrag(support.dropAction)
				Else
					e.rejectDrag()
				End If

				Dim showLocation As Boolean = If(support.showDropLocationIsSet, support.showDropLocation, canImport)

				componentDropLocationion(If(showLocation, support, Nothing), False)
			End Sub

			Public Overridable Sub dragEnter(ByVal e As DropTargetDragEvent)
				state = Nothing
				component = e.dropTargetContext.component

				handleDrag(e)

				If TypeOf component Is JComponent Then
					lastPosition = e.location
					updateAutoscrollRegion(CType(component, JComponent))
					initPropertiesIfNecessary()
				End If
			End Sub

			Public Overridable Sub dragOver(ByVal e As DropTargetDragEvent)
				handleDrag(e)

				If Not(TypeOf component Is JComponent) Then Return

				Dim p As Point = e.location

				If Math.Abs(p.x - lastPosition.x) > hysteresis OrElse Math.Abs(p.y - lastPosition.y) > hysteresis Then
					' no autoscroll
					If ___timer.running Then ___timer.stop()
				Else
					If Not ___timer.running Then ___timer.start()
				End If

				lastPosition = p
			End Sub

			Public Overridable Sub dragExit(ByVal e As DropTargetEvent)
				cleanup(False)
			End Sub

			Public Overridable Sub drop(ByVal e As DropTargetDropEvent)
				Dim importer As TransferHandler = CType(component, HasGetTransferHandler).transferHandler

				If importer Is Nothing Then
					e.rejectDrop()
					cleanup(False)
					Return
				End If

				support.dNDVariablesles(component, e)
				Dim canImport As Boolean = importer.canImport(support)

				If canImport Then
					e.acceptDrop(support.dropAction)

					Dim showLocation As Boolean = If(support.showDropLocationIsSet, support.showDropLocation, canImport)

					componentDropLocationion(If(showLocation, support, Nothing), False)

					Dim success As Boolean

					Try
						success = importer.importData(support)
					Catch re As Exception
						success = False
					End Try

					e.dropComplete(success)
					cleanup(success)
				Else
					e.rejectDrop()
					cleanup(False)
				End If
			End Sub

			Public Overridable Sub dropActionChanged(ByVal e As DropTargetDragEvent)
	'            
	'             * Work-around for Linux bug where dropActionChanged
	'             * is called before dragEnter.
	'             
				If component Is Nothing Then Return

				handleDrag(e)
			End Sub

			Private Sub cleanup(ByVal forDrop As Boolean)
				componentDropLocationion(Nothing, forDrop)
				If TypeOf component Is JComponent Then CType(component, JComponent).dndDone()

				If ___timer IsNot Nothing Then ___timer.stop()

				state = Nothing
				component = Nothing
				lastPosition = Nothing
			End Sub
		End Class

		''' <summary>
		''' This is the default drag handler for drag and drop operations that
		''' use the <code>TransferHandler</code>.
		''' </summary>
		Private Class DragHandler
			Implements DragGestureListener, DragSourceListener

			Private scrolls As Boolean

			' --- DragGestureListener methods -----------------------------------

			''' <summary>
			''' a Drag gesture has been recognized
			''' </summary>
			Public Overridable Sub dragGestureRecognized(ByVal dge As DragGestureEvent)
				Dim c As JComponent = CType(dge.component, JComponent)
				Dim th As TransferHandler = c.transferHandler
				Dim t As Transferable = th.createTransferable(c)
				If t IsNot Nothing Then
					scrolls = c.autoscrolls
					c.autoscrolls = False
					Try
						Dim im As Image = th.dragImage
						If im Is Nothing Then
							dge.startDrag(Nothing, t, Me)
						Else
							dge.startDrag(Nothing, im, th.dragImageOffset, t, Me)
						End If
						Return
					Catch re As Exception
						c.autoscrolls = scrolls
					End Try
				End If

				th.exportDone(c, t, NONE)
			End Sub

			' --- DragSourceListener methods -----------------------------------

			''' <summary>
			''' as the hotspot enters a platform dependent drop site
			''' </summary>
			Public Overridable Sub dragEnter(ByVal dsde As DragSourceDragEvent)
			End Sub

			''' <summary>
			''' as the hotspot moves over a platform dependent drop site
			''' </summary>
			Public Overridable Sub dragOver(ByVal dsde As DragSourceDragEvent)
			End Sub

			''' <summary>
			''' as the hotspot exits a platform dependent drop site
			''' </summary>
			Public Overridable Sub dragExit(ByVal dsde As DragSourceEvent)
			End Sub

			''' <summary>
			''' as the operation completes
			''' </summary>
			Public Overridable Sub dragDropEnd(ByVal dsde As DragSourceDropEvent)
				Dim dsc As DragSourceContext = dsde.dragSourceContext
				Dim c As JComponent = CType(dsc.component, JComponent)
				If dsde.dropSuccess Then
					c.transferHandler.exportDone(c, dsc.transferable, dsde.dropAction)
				Else
					c.transferHandler.exportDone(c, dsc.transferable, NONE)
				End If
				c.autoscrolls = scrolls
			End Sub

			Public Overridable Sub dropActionChanged(ByVal dsde As DragSourceDragEvent)
			End Sub
		End Class

		Private Class SwingDragGestureRecognizer
			Inherits DragGestureRecognizer

			Friend Sub New(ByVal dgl As DragGestureListener)
				MyBase.New(DragSource.defaultDragSource, Nothing, NONE, dgl)
			End Sub

			Friend Overridable Sub gestured(ByVal c As JComponent, ByVal e As MouseEvent, ByVal srcActions As Integer, ByVal action As Integer)
				component = c
				sourceActions = srcActions
				appendEvent(e)
				fireDragGestureRecognized(action, e.point)
			End Sub

			''' <summary>
			''' register this DragGestureRecognizer's Listeners with the Component
			''' </summary>
			Protected Friend Overridable Sub registerListeners()
			End Sub

			''' <summary>
			''' unregister this DragGestureRecognizer's Listeners with the Component
			''' 
			''' subclasses must override this method
			''' </summary>
			Protected Friend Overridable Sub unregisterListeners()
			End Sub

		End Class

		Friend Shared ReadOnly cutAction As Action = New TransferAction("cut")
		Friend Shared ReadOnly copyAction As Action = New TransferAction("copy")
		Friend Shared ReadOnly pasteAction As Action = New TransferAction("paste")

		Friend Class TransferAction
			Inherits UIAction
			Implements javax.swing.plaf.UIResource

			Friend Sub New(ByVal name As String)
				MyBase.New(name)
			End Sub

			Public Overridable Function isEnabled(ByVal sender As Object) As Boolean
				If TypeOf sender Is JComponent AndAlso CType(sender, JComponent).transferHandler Is Nothing Then Return False

				Return True
			End Function

			Private Shared ReadOnly javaSecurityAccess As sun.misc.JavaSecurityAccess = sun.misc.SharedSecrets.javaSecurityAccess

			Public Overridable Sub actionPerformed(ByVal e As ActionEvent)
				Dim src As Object = e.source

'JAVA TO VB CONVERTER TODO TASK: Anonymous inner classes are not converted to VB if the base type is not defined in the code being converted:
'				final java.security.PrivilegedAction<Void> action = New java.security.PrivilegedAction<Void>()
	'			{
	'				public Void run()
	'				{
	'					actionPerformedImpl(e);
	'					Return Nothing;
	'				}
	'			};

				Dim stack As java.security.AccessControlContext = java.security.AccessController.context
				Dim srcAcc As java.security.AccessControlContext = sun.awt.AWTAccessor.componentAccessor.getAccessControlContext(CType(src, Component))
				Dim eventAcc As java.security.AccessControlContext = sun.awt.AWTAccessor.aWTEventAccessor.getAccessControlContext(e)

					If srcAcc Is Nothing Then
						javaSecurityAccess.doIntersectionPrivilege(action, stack, eventAcc)
					Else
'JAVA TO VB CONVERTER TODO TASK: Anonymous inner classes are not converted to VB if the base type is not defined in the code being converted:
'						javaSecurityAccess.doIntersectionPrivilege(New java.security.PrivilegedAction<Void>()
	'					{
	'							public Void run()
	'							{
	'								javaSecurityAccess.doIntersectionPrivilege(action, eventAcc);
	'								Return Nothing;
	'							 }
	'					}, stack, srcAcc);
					End If
			End Sub

			Private Sub actionPerformedImpl(ByVal e As ActionEvent)
				Dim src As Object = e.source
				If TypeOf src Is JComponent Then
					Dim c As JComponent = CType(src, JComponent)
					Dim th As TransferHandler = c.transferHandler
					Dim ___clipboard As Clipboard = getClipboard(c)
					Dim name As String = CStr(getValue(Action.NAME))

					Dim trans As Transferable = Nothing

					' any of these calls may throw IllegalStateException
					Try
						If (___clipboard IsNot Nothing) AndAlso (th IsNot Nothing) AndAlso (name IsNot Nothing) Then
							If "cut".Equals(name) Then
								th.exportToClipboard(c, ___clipboard, MOVE)
							ElseIf "copy".Equals(name) Then
								th.exportToClipboard(c, ___clipboard, COPY)
							ElseIf "paste".Equals(name) Then
								trans = ___clipboard.getContents(Nothing)
							End If
						End If
					Catch ise As IllegalStateException
						' clipboard was unavailable
						UIManager.lookAndFeel.provideErrorFeedback(c)
						Return
					End Try

					' this is a paste action, import data into the component
					If trans IsNot Nothing Then th.importData(New TransferSupport(c, trans))
				End If
			End Sub

			''' <summary>
			''' Returns the clipboard to use for cut/copy/paste.
			''' </summary>
			Private Function getClipboard(ByVal c As JComponent) As Clipboard
				If sun.swing.SwingUtilities2.canAccessSystemClipboard() Then Return c.toolkit.systemClipboard
				Dim ___clipboard As Clipboard = CType(sun.awt.AppContext.appContext.get(SandboxClipboardKey), Clipboard)
				If ___clipboard Is Nothing Then
					___clipboard = New Clipboard("Sandboxed Component Clipboard")
					sun.awt.AppContext.appContext.put(SandboxClipboardKey, ___clipboard)
				End If
				Return ___clipboard
			End Function

			''' <summary>
			''' Key used in app context to lookup Clipboard to use if access to
			''' System clipboard is denied.
			''' </summary>
			Private Shared SandboxClipboardKey As New Object

		End Class

	End Class

End Namespace