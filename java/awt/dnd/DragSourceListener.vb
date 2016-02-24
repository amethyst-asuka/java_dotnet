'
' * Copyright (c) 1997, 2003, Oracle and/or its affiliates. All rights reserved.
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
	''' The <code>DragSourceListener</code> defines the
	''' event interface for originators of
	''' Drag and Drop operations to track the state of the user's gesture, and to
	''' provide appropriate &quot;drag over&quot;
	''' feedback to the user throughout the
	''' Drag and Drop operation.
	''' <p>
	''' The drop site is <i>associated with the previous <code>dragEnter()</code>
	''' invocation</i> if the latest invocation of <code>dragEnter()</code> on this
	''' listener:
	''' <ul>
	''' <li>corresponds to that drop site and
	''' <li> is not followed by a <code>dragExit()</code> invocation on this listener.
	''' </ul>
	''' 
	''' @since 1.2
	''' </summary>

	Public Interface DragSourceListener
		Inherits java.util.EventListener

		''' <summary>
		''' Called as the cursor's hotspot enters a platform-dependent drop site.
		''' This method is invoked when all the following conditions are true:
		''' <UL>
		''' <LI>The cursor's hotspot enters the operable part of a platform-
		''' dependent drop site.
		''' <LI>The drop site is active.
		''' <LI>The drop site accepts the drag.
		''' </UL>
		''' </summary>
		''' <param name="dsde"> the <code>DragSourceDragEvent</code> </param>
		Sub dragEnter(ByVal dsde As DragSourceDragEvent)

		''' <summary>
		''' Called as the cursor's hotspot moves over a platform-dependent drop site.
		''' This method is invoked when all the following conditions are true:
		''' <UL>
		''' <LI>The cursor's hotspot has moved, but still intersects the
		''' operable part of the drop site associated with the previous
		''' dragEnter() invocation.
		''' <LI>The drop site is still active.
		''' <LI>The drop site accepts the drag.
		''' </UL>
		''' </summary>
		''' <param name="dsde"> the <code>DragSourceDragEvent</code> </param>
		Sub dragOver(ByVal dsde As DragSourceDragEvent)

		''' <summary>
		''' Called when the user has modified the drop gesture.
		''' This method is invoked when the state of the input
		''' device(s) that the user is interacting with changes.
		''' Such devices are typically the mouse buttons or keyboard
		''' modifiers that the user is interacting with.
		''' </summary>
		''' <param name="dsde"> the <code>DragSourceDragEvent</code> </param>
		Sub dropActionChanged(ByVal dsde As DragSourceDragEvent)

		''' <summary>
		''' Called as the cursor's hotspot exits a platform-dependent drop site.
		''' This method is invoked when any of the following conditions are true:
		''' <UL>
		''' <LI>The cursor's hotspot no longer intersects the operable part
		''' of the drop site associated with the previous dragEnter() invocation.
		''' </UL>
		''' OR
		''' <UL>
		''' <LI>The drop site associated with the previous dragEnter() invocation
		''' is no longer active.
		''' </UL>
		''' OR
		''' <UL>
		''' <LI> The drop site associated with the previous dragEnter() invocation
		''' has rejected the drag.
		''' </UL>
		''' </summary>
		''' <param name="dse"> the <code>DragSourceEvent</code> </param>
		Sub dragExit(ByVal dse As DragSourceEvent)

		''' <summary>
		''' This method is invoked to signify that the Drag and Drop
		''' operation is complete. The getDropSuccess() method of
		''' the <code>DragSourceDropEvent</code> can be used to
		''' determine the termination state. The getDropAction() method
		''' returns the operation that the drop site selected
		''' to apply to the Drop operation. Once this method is complete, the
		''' current <code>DragSourceContext</code> and
		''' associated resources become invalid.
		''' </summary>
		''' <param name="dsde"> the <code>DragSourceDropEvent</code> </param>
		Sub dragDropEnd(ByVal dsde As DragSourceDropEvent)
	End Interface

End Namespace