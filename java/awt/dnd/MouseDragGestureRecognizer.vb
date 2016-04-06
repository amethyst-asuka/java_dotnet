'
' * Copyright (c) 1998, 2003, Oracle and/or its affiliates. All rights reserved.
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
	''' This abstract subclass of <code>DragGestureRecognizer</code>
	''' defines a <code>DragGestureRecognizer</code>
	''' for mouse-based gestures.
	''' 
	''' Each platform implements its own concrete subclass of this [Class],
	''' available via the Toolkit.createDragGestureRecognizer() method,
	''' to encapsulate
	''' the recognition of the platform dependent mouse gesture(s) that initiate
	''' a Drag and Drop operation.
	''' <p>
	''' Mouse drag gesture recognizers should honor the
	''' drag gesture motion threshold, available through
	''' <seealso cref="DragSource#getDragThreshold"/>.
	''' A drag gesture should be recognized only when the distance
	''' in either the horizontal or vertical direction between
	''' the location of the latest mouse dragged event and the
	''' location of the corresponding mouse button pressed event
	''' is greater than the drag gesture motion threshold.
	''' <p>
	''' Drag gesture recognizers created with
	''' <seealso cref="DragSource#createDefaultDragGestureRecognizer"/>
	''' follow this convention.
	''' 
	''' @author Laurence P. G. Cable
	''' </summary>
	''' <seealso cref= java.awt.dnd.DragGestureListener </seealso>
	''' <seealso cref= java.awt.dnd.DragGestureEvent </seealso>
	''' <seealso cref= java.awt.dnd.DragSource </seealso>

	Public MustInherit Class MouseDragGestureRecognizer
		Inherits DragGestureRecognizer
		Implements java.awt.event.MouseListener, java.awt.event.MouseMotionListener

			Public MustOverride Sub mouseMoved(  e As java.awt.event.MouseEvent)
			Public MustOverride Sub mouseDragged(  e As java.awt.event.MouseEvent)
			Public MustOverride Sub mouseExited(  e As java.awt.event.MouseEvent)
			Public MustOverride Sub mouseEntered(  e As java.awt.event.MouseEvent)
			Public MustOverride Sub mouseReleased(  e As java.awt.event.MouseEvent)
			Public MustOverride Sub mousePressed(  e As java.awt.event.MouseEvent)
			Public MustOverride Sub mouseClicked(  e As java.awt.event.MouseEvent)

		Private Const serialVersionUID As Long = 6220099344182281120L

		''' <summary>
		''' Construct a new <code>MouseDragGestureRecognizer</code>
		''' given the <code>DragSource</code> for the
		''' <code>Component</code> c, the <code>Component</code>
		''' to observe, the action(s)
		''' permitted for this drag operation, and
		''' the <code>DragGestureListener</code> to
		''' notify when a drag gesture is detected.
		''' <P> </summary>
		''' <param name="ds">  The DragSource for the Component c </param>
		''' <param name="c">   The Component to observe </param>
		''' <param name="act"> The actions permitted for this Drag </param>
		''' <param name="dgl"> The DragGestureListener to notify when a gesture is detected
		'''  </param>

		Protected Friend Sub New(  ds As DragSource,   c As java.awt.Component,   act As Integer,   dgl As DragGestureListener)
			MyBase.New(ds, c, act, dgl)
		End Sub

		''' <summary>
		''' Construct a new <code>MouseDragGestureRecognizer</code>
		''' given the <code>DragSource</code> for
		''' the <code>Component</code> c,
		''' the <code>Component</code> to observe, and the action(s)
		''' permitted for this drag operation.
		''' <P> </summary>
		''' <param name="ds">  The DragSource for the Component c </param>
		''' <param name="c">   The Component to observe </param>
		''' <param name="act"> The actions permitted for this drag </param>

		Protected Friend Sub New(  ds As DragSource,   c As java.awt.Component,   act As Integer)
			Me.New(ds, c, act, Nothing)
		End Sub

		''' <summary>
		''' Construct a new <code>MouseDragGestureRecognizer</code>
		''' given the <code>DragSource</code> for the
		''' <code>Component</code> c, and the
		''' <code>Component</code> to observe.
		''' <P> </summary>
		''' <param name="ds">  The DragSource for the Component c </param>
		''' <param name="c">   The Component to observe </param>

		Protected Friend Sub New(  ds As DragSource,   c As java.awt.Component)
			Me.New(ds, c, DnDConstants.ACTION_NONE)
		End Sub

		''' <summary>
		''' Construct a new <code>MouseDragGestureRecognizer</code>
		''' given the <code>DragSource</code> for the <code>Component</code>.
		''' <P> </summary>
		''' <param name="ds">  The DragSource for the Component </param>

		Protected Friend Sub New(  ds As DragSource)
			Me.New(ds, Nothing)
		End Sub

		''' <summary>
		''' register this DragGestureRecognizer's Listeners with the Component
		''' </summary>

		Protected Friend Overrides Sub registerListeners()
			component_Renamed.addMouseListener(Me)
			component_Renamed.addMouseMotionListener(Me)
		End Sub

		''' <summary>
		''' unregister this DragGestureRecognizer's Listeners with the Component
		''' 
		''' subclasses must override this method
		''' </summary>


		Protected Friend Overrides Sub unregisterListeners()
			component_Renamed.removeMouseListener(Me)
			component_Renamed.removeMouseMotionListener(Me)
		End Sub

		''' <summary>
		''' Invoked when the mouse has been clicked on a component.
		''' <P> </summary>
		''' <param name="e"> the <code>MouseEvent</code> </param>

		Public Overridable Sub mouseClicked(  e As java.awt.event.MouseEvent)
		End Sub

		''' <summary>
		''' Invoked when a mouse button has been
		''' pressed on a <code>Component</code>.
		''' <P> </summary>
		''' <param name="e"> the <code>MouseEvent</code> </param>

		Public Overridable Sub mousePressed(  e As java.awt.event.MouseEvent)
		End Sub

		''' <summary>
		''' Invoked when a mouse button has been released on a component.
		''' <P> </summary>
		''' <param name="e"> the <code>MouseEvent</code> </param>

		Public Overridable Sub mouseReleased(  e As java.awt.event.MouseEvent)
		End Sub

		''' <summary>
		''' Invoked when the mouse enters a component.
		''' <P> </summary>
		''' <param name="e"> the <code>MouseEvent</code> </param>

		Public Overridable Sub mouseEntered(  e As java.awt.event.MouseEvent)
		End Sub

		''' <summary>
		''' Invoked when the mouse exits a component.
		''' <P> </summary>
		''' <param name="e"> the <code>MouseEvent</code> </param>

		Public Overridable Sub mouseExited(  e As java.awt.event.MouseEvent)
		End Sub

		''' <summary>
		''' Invoked when a mouse button is pressed on a component.
		''' <P> </summary>
		''' <param name="e"> the <code>MouseEvent</code> </param>

		Public Overridable Sub mouseDragged(  e As java.awt.event.MouseEvent)
		End Sub

		''' <summary>
		''' Invoked when the mouse button has been moved on a component
		''' (with no buttons no down).
		''' <P> </summary>
		''' <param name="e"> the <code>MouseEvent</code> </param>

		Public Overridable Sub mouseMoved(  e As java.awt.event.MouseEvent)
		End Sub
	End Class

End Namespace