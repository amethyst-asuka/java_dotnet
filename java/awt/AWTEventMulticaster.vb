Imports System

'
' * Copyright (c) 1996, 2013, Oracle and/or its affiliates. All rights reserved.
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
Namespace java.awt



	''' <summary>
	''' {@code AWTEventMulticaster} implements efficient and thread-safe multi-cast
	''' event dispatching for the AWT events defined in the {@code java.awt.event}
	''' package.
	''' <p>
	''' The following example illustrates how to use this class:
	''' 
	''' <pre><code>
	''' public myComponent extends Component {
	'''     ActionListener actionListener = null;
	''' 
	'''     public synchronized void addActionListener(ActionListener l) {
	'''         actionListener = AWTEventMulticaster.add(actionListener, l);
	'''     }
	'''     public synchronized void removeActionListener(ActionListener l) {
	'''         actionListener = AWTEventMulticaster.remove(actionListener, l);
	'''     }
	'''     public void processEvent(AWTEvent e) {
	'''         // when event occurs which causes "action" semantic
	'''         ActionListener listener = actionListener;
	'''         if (listener != null) {
	'''             listener.actionPerformed(new ActionEvent());
	'''         }
	'''     }
	''' }
	''' </code></pre>
	''' The important point to note is the first argument to the {@code
	''' add} and {@code remove} methods is the field maintaining the
	''' listeners. In addition you must assign the result of the {@code add}
	''' and {@code remove} methods to the field maintaining the listeners.
	''' <p>
	''' {@code AWTEventMulticaster} is implemented as a pair of {@code
	''' EventListeners} that are set at construction time. {@code
	''' AWTEventMulticaster} is immutable. The {@code add} and {@code
	''' remove} methods do not alter {@code AWTEventMulticaster} in
	''' anyway. If necessary, a new {@code AWTEventMulticaster} is
	''' created. In this way it is safe to add and remove listeners during
	''' the process of an event dispatching.  However, event listeners
	''' added during the process of an event dispatch operation are not
	''' notified of the event currently being dispatched.
	''' <p>
	''' All of the {@code add} methods allow {@code null} arguments. If the
	''' first argument is {@code null}, the second argument is returned. If
	''' the first argument is not {@code null} and the second argument is
	''' {@code null}, the first argument is returned. If both arguments are
	''' {@code non-null}, a new {@code AWTEventMulticaster} is created using
	''' the two arguments and returned.
	''' <p>
	''' For the {@code remove} methods that take two arguments, the following is
	''' returned:
	''' <ul>
	'''   <li>{@code null}, if the first argument is {@code null}, or
	'''       the arguments are equal, by way of {@code ==}.
	'''   <li>the first argument, if the first argument is not an instance of
	'''       {@code AWTEventMulticaster}.
	'''   <li>result of invoking {@code remove(EventListener)} on the
	'''       first argument, supplying the second argument to the
	'''       {@code remove(EventListener)} method.
	''' </ul>
	''' <p>Swing makes use of
	''' <seealso cref="javax.swing.event.EventListenerList EventListenerList"/> for
	''' similar logic. Refer to it for details.
	''' </summary>
	''' <seealso cref= javax.swing.event.EventListenerList
	''' 
	''' @author      John Rose
	''' @author      Amy Fowler
	''' @since       1.1 </seealso>

	Public Class AWTEventMulticaster
		Implements ComponentListener, ContainerListener, FocusListener, KeyListener, MouseListener, MouseMotionListener, WindowListener, WindowFocusListener, WindowStateListener, ActionListener, ItemListener, AdjustmentListener, TextListener, InputMethodListener, HierarchyListener, HierarchyBoundsListener, MouseWheelListener

		Protected Friend ReadOnly a, b As java.util.EventListener

		''' <summary>
		''' Creates an event multicaster instance which chains listener-a
		''' with listener-b. Input parameters <code>a</code> and <code>b</code>
		''' should not be <code>null</code>, though implementations may vary in
		''' choosing whether or not to throw <code>NullPointerException</code>
		''' in that case. </summary>
		''' <param name="a"> listener-a </param>
		''' <param name="b"> listener-b </param>
		Protected Friend Sub New(ByVal a As java.util.EventListener, ByVal b As java.util.EventListener)
			Me.a = a
			Me.b = b
		End Sub

		''' <summary>
		''' Removes a listener from this multicaster.
		''' <p>
		''' The returned multicaster contains all the listeners in this
		''' multicaster with the exception of all occurrences of {@code oldl}.
		''' If the resulting multicaster contains only one regular listener
		''' the regular listener may be returned.  If the resulting multicaster
		''' is empty, then {@code null} may be returned instead.
		''' <p>
		''' No exception is thrown if {@code oldl} is {@code null}.
		''' </summary>
		''' <param name="oldl"> the listener to be removed </param>
		''' <returns> resulting listener </returns>
		Protected Friend Overridable Function remove(ByVal oldl As java.util.EventListener) As java.util.EventListener
			If oldl Is a Then Return b
			If oldl Is b Then Return a
			Dim a2 As java.util.EventListener = removeInternal(a, oldl)
			Dim b2 As java.util.EventListener = removeInternal(b, oldl)
			If a2 Is a AndAlso b2 Is b Then Return Me ' it's not here
			Return addInternal(a2, b2)
		End Function

		''' <summary>
		''' Handles the componentResized event by invoking the
		''' componentResized methods on listener-a and listener-b. </summary>
		''' <param name="e"> the component event </param>
		Public Overridable Sub componentResized(ByVal e As ComponentEvent) Implements ComponentListener.componentResized
			CType(a, ComponentListener).componentResized(e)
			CType(b, ComponentListener).componentResized(e)
		End Sub

		''' <summary>
		''' Handles the componentMoved event by invoking the
		''' componentMoved methods on listener-a and listener-b. </summary>
		''' <param name="e"> the component event </param>
		Public Overridable Sub componentMoved(ByVal e As ComponentEvent) Implements ComponentListener.componentMoved
			CType(a, ComponentListener).componentMoved(e)
			CType(b, ComponentListener).componentMoved(e)
		End Sub

		''' <summary>
		''' Handles the componentShown event by invoking the
		''' componentShown methods on listener-a and listener-b. </summary>
		''' <param name="e"> the component event </param>
		Public Overridable Sub componentShown(ByVal e As ComponentEvent) Implements ComponentListener.componentShown
			CType(a, ComponentListener).componentShown(e)
			CType(b, ComponentListener).componentShown(e)
		End Sub

		''' <summary>
		''' Handles the componentHidden event by invoking the
		''' componentHidden methods on listener-a and listener-b. </summary>
		''' <param name="e"> the component event </param>
		Public Overridable Sub componentHidden(ByVal e As ComponentEvent) Implements ComponentListener.componentHidden
			CType(a, ComponentListener).componentHidden(e)
			CType(b, ComponentListener).componentHidden(e)
		End Sub

		''' <summary>
		''' Handles the componentAdded container event by invoking the
		''' componentAdded methods on listener-a and listener-b. </summary>
		''' <param name="e"> the component event </param>
		Public Overridable Sub componentAdded(ByVal e As ContainerEvent) Implements ContainerListener.componentAdded
			CType(a, ContainerListener).componentAdded(e)
			CType(b, ContainerListener).componentAdded(e)
		End Sub

		''' <summary>
		''' Handles the componentRemoved container event by invoking the
		''' componentRemoved methods on listener-a and listener-b. </summary>
		''' <param name="e"> the component event </param>
		Public Overridable Sub componentRemoved(ByVal e As ContainerEvent) Implements ContainerListener.componentRemoved
			CType(a, ContainerListener).componentRemoved(e)
			CType(b, ContainerListener).componentRemoved(e)
		End Sub

		''' <summary>
		''' Handles the focusGained event by invoking the
		''' focusGained methods on listener-a and listener-b. </summary>
		''' <param name="e"> the focus event </param>
		Public Overridable Sub focusGained(ByVal e As FocusEvent) Implements FocusListener.focusGained
			CType(a, FocusListener).focusGained(e)
			CType(b, FocusListener).focusGained(e)
		End Sub

		''' <summary>
		''' Handles the focusLost event by invoking the
		''' focusLost methods on listener-a and listener-b. </summary>
		''' <param name="e"> the focus event </param>
		Public Overridable Sub focusLost(ByVal e As FocusEvent) Implements FocusListener.focusLost
			CType(a, FocusListener).focusLost(e)
			CType(b, FocusListener).focusLost(e)
		End Sub

		''' <summary>
		''' Handles the keyTyped event by invoking the
		''' keyTyped methods on listener-a and listener-b. </summary>
		''' <param name="e"> the key event </param>
		Public Overridable Sub keyTyped(ByVal e As KeyEvent) Implements KeyListener.keyTyped
			CType(a, KeyListener).keyTyped(e)
			CType(b, KeyListener).keyTyped(e)
		End Sub

		''' <summary>
		''' Handles the keyPressed event by invoking the
		''' keyPressed methods on listener-a and listener-b. </summary>
		''' <param name="e"> the key event </param>
		Public Overridable Sub keyPressed(ByVal e As KeyEvent) Implements KeyListener.keyPressed
			CType(a, KeyListener).keyPressed(e)
			CType(b, KeyListener).keyPressed(e)
		End Sub

		''' <summary>
		''' Handles the keyReleased event by invoking the
		''' keyReleased methods on listener-a and listener-b. </summary>
		''' <param name="e"> the key event </param>
		Public Overridable Sub keyReleased(ByVal e As KeyEvent) Implements KeyListener.keyReleased
			CType(a, KeyListener).keyReleased(e)
			CType(b, KeyListener).keyReleased(e)
		End Sub

		''' <summary>
		''' Handles the mouseClicked event by invoking the
		''' mouseClicked methods on listener-a and listener-b. </summary>
		''' <param name="e"> the mouse event </param>
		Public Overridable Sub mouseClicked(ByVal e As MouseEvent) Implements MouseListener.mouseClicked
			CType(a, MouseListener).mouseClicked(e)
			CType(b, MouseListener).mouseClicked(e)
		End Sub

		''' <summary>
		''' Handles the mousePressed event by invoking the
		''' mousePressed methods on listener-a and listener-b. </summary>
		''' <param name="e"> the mouse event </param>
		Public Overridable Sub mousePressed(ByVal e As MouseEvent) Implements MouseListener.mousePressed
			CType(a, MouseListener).mousePressed(e)
			CType(b, MouseListener).mousePressed(e)
		End Sub

		''' <summary>
		''' Handles the mouseReleased event by invoking the
		''' mouseReleased methods on listener-a and listener-b. </summary>
		''' <param name="e"> the mouse event </param>
		Public Overridable Sub mouseReleased(ByVal e As MouseEvent) Implements MouseListener.mouseReleased
			CType(a, MouseListener).mouseReleased(e)
			CType(b, MouseListener).mouseReleased(e)
		End Sub

		''' <summary>
		''' Handles the mouseEntered event by invoking the
		''' mouseEntered methods on listener-a and listener-b. </summary>
		''' <param name="e"> the mouse event </param>
		Public Overridable Sub mouseEntered(ByVal e As MouseEvent) Implements MouseListener.mouseEntered
			CType(a, MouseListener).mouseEntered(e)
			CType(b, MouseListener).mouseEntered(e)
		End Sub

		''' <summary>
		''' Handles the mouseExited event by invoking the
		''' mouseExited methods on listener-a and listener-b. </summary>
		''' <param name="e"> the mouse event </param>
		Public Overridable Sub mouseExited(ByVal e As MouseEvent) Implements MouseListener.mouseExited
			CType(a, MouseListener).mouseExited(e)
			CType(b, MouseListener).mouseExited(e)
		End Sub

		''' <summary>
		''' Handles the mouseDragged event by invoking the
		''' mouseDragged methods on listener-a and listener-b. </summary>
		''' <param name="e"> the mouse event </param>
		Public Overridable Sub mouseDragged(ByVal e As MouseEvent) Implements MouseMotionListener.mouseDragged
			CType(a, MouseMotionListener).mouseDragged(e)
			CType(b, MouseMotionListener).mouseDragged(e)
		End Sub

		''' <summary>
		''' Handles the mouseMoved event by invoking the
		''' mouseMoved methods on listener-a and listener-b. </summary>
		''' <param name="e"> the mouse event </param>
		Public Overridable Sub mouseMoved(ByVal e As MouseEvent) Implements MouseMotionListener.mouseMoved
			CType(a, MouseMotionListener).mouseMoved(e)
			CType(b, MouseMotionListener).mouseMoved(e)
		End Sub

		''' <summary>
		''' Handles the windowOpened event by invoking the
		''' windowOpened methods on listener-a and listener-b. </summary>
		''' <param name="e"> the window event </param>
		Public Overridable Sub windowOpened(ByVal e As WindowEvent) Implements WindowListener.windowOpened
			CType(a, WindowListener).windowOpened(e)
			CType(b, WindowListener).windowOpened(e)
		End Sub

		''' <summary>
		''' Handles the windowClosing event by invoking the
		''' windowClosing methods on listener-a and listener-b. </summary>
		''' <param name="e"> the window event </param>
		Public Overridable Sub windowClosing(ByVal e As WindowEvent) Implements WindowListener.windowClosing
			CType(a, WindowListener).windowClosing(e)
			CType(b, WindowListener).windowClosing(e)
		End Sub

		''' <summary>
		''' Handles the windowClosed event by invoking the
		''' windowClosed methods on listener-a and listener-b. </summary>
		''' <param name="e"> the window event </param>
		Public Overridable Sub windowClosed(ByVal e As WindowEvent) Implements WindowListener.windowClosed
			CType(a, WindowListener).windowClosed(e)
			CType(b, WindowListener).windowClosed(e)
		End Sub

		''' <summary>
		''' Handles the windowIconified event by invoking the
		''' windowIconified methods on listener-a and listener-b. </summary>
		''' <param name="e"> the window event </param>
		Public Overridable Sub windowIconified(ByVal e As WindowEvent) Implements WindowListener.windowIconified
			CType(a, WindowListener).windowIconified(e)
			CType(b, WindowListener).windowIconified(e)
		End Sub

		''' <summary>
		''' Handles the windowDeiconfied event by invoking the
		''' windowDeiconified methods on listener-a and listener-b. </summary>
		''' <param name="e"> the window event </param>
		Public Overridable Sub windowDeiconified(ByVal e As WindowEvent) Implements WindowListener.windowDeiconified
			CType(a, WindowListener).windowDeiconified(e)
			CType(b, WindowListener).windowDeiconified(e)
		End Sub

		''' <summary>
		''' Handles the windowActivated event by invoking the
		''' windowActivated methods on listener-a and listener-b. </summary>
		''' <param name="e"> the window event </param>
		Public Overridable Sub windowActivated(ByVal e As WindowEvent) Implements WindowListener.windowActivated
			CType(a, WindowListener).windowActivated(e)
			CType(b, WindowListener).windowActivated(e)
		End Sub

		''' <summary>
		''' Handles the windowDeactivated event by invoking the
		''' windowDeactivated methods on listener-a and listener-b. </summary>
		''' <param name="e"> the window event </param>
		Public Overridable Sub windowDeactivated(ByVal e As WindowEvent) Implements WindowListener.windowDeactivated
			CType(a, WindowListener).windowDeactivated(e)
			CType(b, WindowListener).windowDeactivated(e)
		End Sub

		''' <summary>
		''' Handles the windowStateChanged event by invoking the
		''' windowStateChanged methods on listener-a and listener-b. </summary>
		''' <param name="e"> the window event
		''' @since 1.4 </param>
		Public Overridable Sub windowStateChanged(ByVal e As WindowEvent) Implements WindowStateListener.windowStateChanged
			CType(a, WindowStateListener).windowStateChanged(e)
			CType(b, WindowStateListener).windowStateChanged(e)
		End Sub


		''' <summary>
		''' Handles the windowGainedFocus event by invoking the windowGainedFocus
		''' methods on listener-a and listener-b. </summary>
		''' <param name="e"> the window event
		''' @since 1.4 </param>
		Public Overridable Sub windowGainedFocus(ByVal e As WindowEvent) Implements WindowFocusListener.windowGainedFocus
			CType(a, WindowFocusListener).windowGainedFocus(e)
			CType(b, WindowFocusListener).windowGainedFocus(e)
		End Sub

		''' <summary>
		''' Handles the windowLostFocus event by invoking the windowLostFocus
		''' methods on listener-a and listener-b. </summary>
		''' <param name="e"> the window event
		''' @since 1.4 </param>
		Public Overridable Sub windowLostFocus(ByVal e As WindowEvent) Implements WindowFocusListener.windowLostFocus
			CType(a, WindowFocusListener).windowLostFocus(e)
			CType(b, WindowFocusListener).windowLostFocus(e)
		End Sub

		''' <summary>
		''' Handles the actionPerformed event by invoking the
		''' actionPerformed methods on listener-a and listener-b. </summary>
		''' <param name="e"> the action event </param>
		Public Overridable Sub actionPerformed(ByVal e As ActionEvent) Implements ActionListener.actionPerformed
			CType(a, ActionListener).actionPerformed(e)
			CType(b, ActionListener).actionPerformed(e)
		End Sub

		''' <summary>
		''' Handles the itemStateChanged event by invoking the
		''' itemStateChanged methods on listener-a and listener-b. </summary>
		''' <param name="e"> the item event </param>
		Public Overridable Sub itemStateChanged(ByVal e As ItemEvent) Implements ItemListener.itemStateChanged
			CType(a, ItemListener).itemStateChanged(e)
			CType(b, ItemListener).itemStateChanged(e)
		End Sub

		''' <summary>
		''' Handles the adjustmentValueChanged event by invoking the
		''' adjustmentValueChanged methods on listener-a and listener-b. </summary>
		''' <param name="e"> the adjustment event </param>
		Public Overridable Sub adjustmentValueChanged(ByVal e As AdjustmentEvent) Implements AdjustmentListener.adjustmentValueChanged
			CType(a, AdjustmentListener).adjustmentValueChanged(e)
			CType(b, AdjustmentListener).adjustmentValueChanged(e)
		End Sub
		Public Overridable Sub textValueChanged(ByVal e As TextEvent) Implements TextListener.textValueChanged
			CType(a, TextListener).textValueChanged(e)
			CType(b, TextListener).textValueChanged(e)
		End Sub

		''' <summary>
		''' Handles the inputMethodTextChanged event by invoking the
		''' inputMethodTextChanged methods on listener-a and listener-b. </summary>
		''' <param name="e"> the item event </param>
		Public Overridable Sub inputMethodTextChanged(ByVal e As InputMethodEvent) Implements InputMethodListener.inputMethodTextChanged
		   CType(a, InputMethodListener).inputMethodTextChanged(e)
		   CType(b, InputMethodListener).inputMethodTextChanged(e)
		End Sub

		''' <summary>
		''' Handles the caretPositionChanged event by invoking the
		''' caretPositionChanged methods on listener-a and listener-b. </summary>
		''' <param name="e"> the item event </param>
		Public Overridable Sub caretPositionChanged(ByVal e As InputMethodEvent) Implements InputMethodListener.caretPositionChanged
		   CType(a, InputMethodListener).caretPositionChanged(e)
		   CType(b, InputMethodListener).caretPositionChanged(e)
		End Sub

		''' <summary>
		''' Handles the hierarchyChanged event by invoking the
		''' hierarchyChanged methods on listener-a and listener-b. </summary>
		''' <param name="e"> the item event
		''' @since 1.3 </param>
		Public Overridable Sub hierarchyChanged(ByVal e As HierarchyEvent) Implements HierarchyListener.hierarchyChanged
			CType(a, HierarchyListener).hierarchyChanged(e)
			CType(b, HierarchyListener).hierarchyChanged(e)
		End Sub

		''' <summary>
		''' Handles the ancestorMoved event by invoking the
		''' ancestorMoved methods on listener-a and listener-b. </summary>
		''' <param name="e"> the item event
		''' @since 1.3 </param>
		Public Overridable Sub ancestorMoved(ByVal e As HierarchyEvent) Implements HierarchyBoundsListener.ancestorMoved
			CType(a, HierarchyBoundsListener).ancestorMoved(e)
			CType(b, HierarchyBoundsListener).ancestorMoved(e)
		End Sub

		''' <summary>
		''' Handles the ancestorResized event by invoking the
		''' ancestorResized methods on listener-a and listener-b. </summary>
		''' <param name="e"> the item event
		''' @since 1.3 </param>
		Public Overridable Sub ancestorResized(ByVal e As HierarchyEvent) Implements HierarchyBoundsListener.ancestorResized
			CType(a, HierarchyBoundsListener).ancestorResized(e)
			CType(b, HierarchyBoundsListener).ancestorResized(e)
		End Sub

		''' <summary>
		''' Handles the mouseWheelMoved event by invoking the
		''' mouseWheelMoved methods on listener-a and listener-b. </summary>
		''' <param name="e"> the mouse event
		''' @since 1.4 </param>
		Public Overridable Sub mouseWheelMoved(ByVal e As MouseWheelEvent) Implements MouseWheelListener.mouseWheelMoved
			CType(a, MouseWheelListener).mouseWheelMoved(e)
			CType(b, MouseWheelListener).mouseWheelMoved(e)
		End Sub

		''' <summary>
		''' Adds component-listener-a with component-listener-b and
		''' returns the resulting multicast listener. </summary>
		''' <param name="a"> component-listener-a </param>
		''' <param name="b"> component-listener-b </param>
		Public Shared Function add(ByVal a As ComponentListener, ByVal b As ComponentListener) As ComponentListener
			Return CType(addInternal(a, b), ComponentListener)
		End Function

		''' <summary>
		''' Adds container-listener-a with container-listener-b and
		''' returns the resulting multicast listener. </summary>
		''' <param name="a"> container-listener-a </param>
		''' <param name="b"> container-listener-b </param>
		Public Shared Function add(ByVal a As ContainerListener, ByVal b As ContainerListener) As ContainerListener
			Return CType(addInternal(a, b), ContainerListener)
		End Function

		''' <summary>
		''' Adds focus-listener-a with focus-listener-b and
		''' returns the resulting multicast listener. </summary>
		''' <param name="a"> focus-listener-a </param>
		''' <param name="b"> focus-listener-b </param>
		Public Shared Function add(ByVal a As FocusListener, ByVal b As FocusListener) As FocusListener
			Return CType(addInternal(a, b), FocusListener)
		End Function

		''' <summary>
		''' Adds key-listener-a with key-listener-b and
		''' returns the resulting multicast listener. </summary>
		''' <param name="a"> key-listener-a </param>
		''' <param name="b"> key-listener-b </param>
		Public Shared Function add(ByVal a As KeyListener, ByVal b As KeyListener) As KeyListener
			Return CType(addInternal(a, b), KeyListener)
		End Function

		''' <summary>
		''' Adds mouse-listener-a with mouse-listener-b and
		''' returns the resulting multicast listener. </summary>
		''' <param name="a"> mouse-listener-a </param>
		''' <param name="b"> mouse-listener-b </param>
		Public Shared Function add(ByVal a As MouseListener, ByVal b As MouseListener) As MouseListener
			Return CType(addInternal(a, b), MouseListener)
		End Function

		''' <summary>
		''' Adds mouse-motion-listener-a with mouse-motion-listener-b and
		''' returns the resulting multicast listener. </summary>
		''' <param name="a"> mouse-motion-listener-a </param>
		''' <param name="b"> mouse-motion-listener-b </param>
		Public Shared Function add(ByVal a As MouseMotionListener, ByVal b As MouseMotionListener) As MouseMotionListener
			Return CType(addInternal(a, b), MouseMotionListener)
		End Function

		''' <summary>
		''' Adds window-listener-a with window-listener-b and
		''' returns the resulting multicast listener. </summary>
		''' <param name="a"> window-listener-a </param>
		''' <param name="b"> window-listener-b </param>
		Public Shared Function add(ByVal a As WindowListener, ByVal b As WindowListener) As WindowListener
			Return CType(addInternal(a, b), WindowListener)
		End Function

		''' <summary>
		''' Adds window-state-listener-a with window-state-listener-b
		''' and returns the resulting multicast listener. </summary>
		''' <param name="a"> window-state-listener-a </param>
		''' <param name="b"> window-state-listener-b
		''' @since 1.4 </param>
		Public Shared Function add(ByVal a As WindowStateListener, ByVal b As WindowStateListener) As WindowStateListener
			Return CType(addInternal(a, b), WindowStateListener)
		End Function

		''' <summary>
		''' Adds window-focus-listener-a with window-focus-listener-b
		''' and returns the resulting multicast listener. </summary>
		''' <param name="a"> window-focus-listener-a </param>
		''' <param name="b"> window-focus-listener-b
		''' @since 1.4 </param>
		Public Shared Function add(ByVal a As WindowFocusListener, ByVal b As WindowFocusListener) As WindowFocusListener
			Return CType(addInternal(a, b), WindowFocusListener)
		End Function

		''' <summary>
		''' Adds action-listener-a with action-listener-b and
		''' returns the resulting multicast listener. </summary>
		''' <param name="a"> action-listener-a </param>
		''' <param name="b"> action-listener-b </param>
		Public Shared Function add(ByVal a As ActionListener, ByVal b As ActionListener) As ActionListener
			Return CType(addInternal(a, b), ActionListener)
		End Function

		''' <summary>
		''' Adds item-listener-a with item-listener-b and
		''' returns the resulting multicast listener. </summary>
		''' <param name="a"> item-listener-a </param>
		''' <param name="b"> item-listener-b </param>
		Public Shared Function add(ByVal a As ItemListener, ByVal b As ItemListener) As ItemListener
			Return CType(addInternal(a, b), ItemListener)
		End Function

		''' <summary>
		''' Adds adjustment-listener-a with adjustment-listener-b and
		''' returns the resulting multicast listener. </summary>
		''' <param name="a"> adjustment-listener-a </param>
		''' <param name="b"> adjustment-listener-b </param>
		Public Shared Function add(ByVal a As AdjustmentListener, ByVal b As AdjustmentListener) As AdjustmentListener
			Return CType(addInternal(a, b), AdjustmentListener)
		End Function
		Public Shared Function add(ByVal a As TextListener, ByVal b As TextListener) As TextListener
			Return CType(addInternal(a, b), TextListener)
		End Function

		''' <summary>
		''' Adds input-method-listener-a with input-method-listener-b and
		''' returns the resulting multicast listener. </summary>
		''' <param name="a"> input-method-listener-a </param>
		''' <param name="b"> input-method-listener-b </param>
		 Public Shared Function add(ByVal a As InputMethodListener, ByVal b As InputMethodListener) As InputMethodListener
			Return CType(addInternal(a, b), InputMethodListener)
		 End Function

		''' <summary>
		''' Adds hierarchy-listener-a with hierarchy-listener-b and
		''' returns the resulting multicast listener. </summary>
		''' <param name="a"> hierarchy-listener-a </param>
		''' <param name="b"> hierarchy-listener-b
		''' @since 1.3 </param>
		 Public Shared Function add(ByVal a As HierarchyListener, ByVal b As HierarchyListener) As HierarchyListener
			Return CType(addInternal(a, b), HierarchyListener)
		 End Function

		''' <summary>
		''' Adds hierarchy-bounds-listener-a with hierarchy-bounds-listener-b and
		''' returns the resulting multicast listener. </summary>
		''' <param name="a"> hierarchy-bounds-listener-a </param>
		''' <param name="b"> hierarchy-bounds-listener-b
		''' @since 1.3 </param>
		 Public Shared Function add(ByVal a As HierarchyBoundsListener, ByVal b As HierarchyBoundsListener) As HierarchyBoundsListener
			Return CType(addInternal(a, b), HierarchyBoundsListener)
		 End Function

		''' <summary>
		''' Adds mouse-wheel-listener-a with mouse-wheel-listener-b and
		''' returns the resulting multicast listener. </summary>
		''' <param name="a"> mouse-wheel-listener-a </param>
		''' <param name="b"> mouse-wheel-listener-b
		''' @since 1.4 </param>
		Public Shared Function add(ByVal a As MouseWheelListener, ByVal b As MouseWheelListener) As MouseWheelListener
			Return CType(addInternal(a, b), MouseWheelListener)
		End Function

		''' <summary>
		''' Removes the old component-listener from component-listener-l and
		''' returns the resulting multicast listener. </summary>
		''' <param name="l"> component-listener-l </param>
		''' <param name="oldl"> the component-listener being removed </param>
		Public Shared Function remove(ByVal l As ComponentListener, ByVal oldl As ComponentListener) As ComponentListener
			Return CType(removeInternal(l, oldl), ComponentListener)
		End Function

		''' <summary>
		''' Removes the old container-listener from container-listener-l and
		''' returns the resulting multicast listener. </summary>
		''' <param name="l"> container-listener-l </param>
		''' <param name="oldl"> the container-listener being removed </param>
		Public Shared Function remove(ByVal l As ContainerListener, ByVal oldl As ContainerListener) As ContainerListener
			Return CType(removeInternal(l, oldl), ContainerListener)
		End Function

		''' <summary>
		''' Removes the old focus-listener from focus-listener-l and
		''' returns the resulting multicast listener. </summary>
		''' <param name="l"> focus-listener-l </param>
		''' <param name="oldl"> the focus-listener being removed </param>
		Public Shared Function remove(ByVal l As FocusListener, ByVal oldl As FocusListener) As FocusListener
			Return CType(removeInternal(l, oldl), FocusListener)
		End Function

		''' <summary>
		''' Removes the old key-listener from key-listener-l and
		''' returns the resulting multicast listener. </summary>
		''' <param name="l"> key-listener-l </param>
		''' <param name="oldl"> the key-listener being removed </param>
		Public Shared Function remove(ByVal l As KeyListener, ByVal oldl As KeyListener) As KeyListener
			Return CType(removeInternal(l, oldl), KeyListener)
		End Function

		''' <summary>
		''' Removes the old mouse-listener from mouse-listener-l and
		''' returns the resulting multicast listener. </summary>
		''' <param name="l"> mouse-listener-l </param>
		''' <param name="oldl"> the mouse-listener being removed </param>
		Public Shared Function remove(ByVal l As MouseListener, ByVal oldl As MouseListener) As MouseListener
			Return CType(removeInternal(l, oldl), MouseListener)
		End Function

		''' <summary>
		''' Removes the old mouse-motion-listener from mouse-motion-listener-l
		''' and returns the resulting multicast listener. </summary>
		''' <param name="l"> mouse-motion-listener-l </param>
		''' <param name="oldl"> the mouse-motion-listener being removed </param>
		Public Shared Function remove(ByVal l As MouseMotionListener, ByVal oldl As MouseMotionListener) As MouseMotionListener
			Return CType(removeInternal(l, oldl), MouseMotionListener)
		End Function

		''' <summary>
		''' Removes the old window-listener from window-listener-l and
		''' returns the resulting multicast listener. </summary>
		''' <param name="l"> window-listener-l </param>
		''' <param name="oldl"> the window-listener being removed </param>
		Public Shared Function remove(ByVal l As WindowListener, ByVal oldl As WindowListener) As WindowListener
			Return CType(removeInternal(l, oldl), WindowListener)
		End Function

		''' <summary>
		''' Removes the old window-state-listener from window-state-listener-l
		''' and returns the resulting multicast listener. </summary>
		''' <param name="l"> window-state-listener-l </param>
		''' <param name="oldl"> the window-state-listener being removed
		''' @since 1.4 </param>
		Public Shared Function remove(ByVal l As WindowStateListener, ByVal oldl As WindowStateListener) As WindowStateListener
			Return CType(removeInternal(l, oldl), WindowStateListener)
		End Function

		''' <summary>
		''' Removes the old window-focus-listener from window-focus-listener-l
		''' and returns the resulting multicast listener. </summary>
		''' <param name="l"> window-focus-listener-l </param>
		''' <param name="oldl"> the window-focus-listener being removed
		''' @since 1.4 </param>
		Public Shared Function remove(ByVal l As WindowFocusListener, ByVal oldl As WindowFocusListener) As WindowFocusListener
			Return CType(removeInternal(l, oldl), WindowFocusListener)
		End Function

		''' <summary>
		''' Removes the old action-listener from action-listener-l and
		''' returns the resulting multicast listener. </summary>
		''' <param name="l"> action-listener-l </param>
		''' <param name="oldl"> the action-listener being removed </param>
		Public Shared Function remove(ByVal l As ActionListener, ByVal oldl As ActionListener) As ActionListener
			Return CType(removeInternal(l, oldl), ActionListener)
		End Function

		''' <summary>
		''' Removes the old item-listener from item-listener-l and
		''' returns the resulting multicast listener. </summary>
		''' <param name="l"> item-listener-l </param>
		''' <param name="oldl"> the item-listener being removed </param>
		Public Shared Function remove(ByVal l As ItemListener, ByVal oldl As ItemListener) As ItemListener
			Return CType(removeInternal(l, oldl), ItemListener)
		End Function

		''' <summary>
		''' Removes the old adjustment-listener from adjustment-listener-l and
		''' returns the resulting multicast listener. </summary>
		''' <param name="l"> adjustment-listener-l </param>
		''' <param name="oldl"> the adjustment-listener being removed </param>
		Public Shared Function remove(ByVal l As AdjustmentListener, ByVal oldl As AdjustmentListener) As AdjustmentListener
			Return CType(removeInternal(l, oldl), AdjustmentListener)
		End Function
		Public Shared Function remove(ByVal l As TextListener, ByVal oldl As TextListener) As TextListener
			Return CType(removeInternal(l, oldl), TextListener)
		End Function

		''' <summary>
		''' Removes the old input-method-listener from input-method-listener-l and
		''' returns the resulting multicast listener. </summary>
		''' <param name="l"> input-method-listener-l </param>
		''' <param name="oldl"> the input-method-listener being removed </param>
		Public Shared Function remove(ByVal l As InputMethodListener, ByVal oldl As InputMethodListener) As InputMethodListener
			Return CType(removeInternal(l, oldl), InputMethodListener)
		End Function

		''' <summary>
		''' Removes the old hierarchy-listener from hierarchy-listener-l and
		''' returns the resulting multicast listener. </summary>
		''' <param name="l"> hierarchy-listener-l </param>
		''' <param name="oldl"> the hierarchy-listener being removed
		''' @since 1.3 </param>
		Public Shared Function remove(ByVal l As HierarchyListener, ByVal oldl As HierarchyListener) As HierarchyListener
			Return CType(removeInternal(l, oldl), HierarchyListener)
		End Function

		''' <summary>
		''' Removes the old hierarchy-bounds-listener from
		''' hierarchy-bounds-listener-l and returns the resulting multicast
		''' listener. </summary>
		''' <param name="l"> hierarchy-bounds-listener-l </param>
		''' <param name="oldl"> the hierarchy-bounds-listener being removed
		''' @since 1.3 </param>
		Public Shared Function remove(ByVal l As HierarchyBoundsListener, ByVal oldl As HierarchyBoundsListener) As HierarchyBoundsListener
			Return CType(removeInternal(l, oldl), HierarchyBoundsListener)
		End Function

		''' <summary>
		''' Removes the old mouse-wheel-listener from mouse-wheel-listener-l
		''' and returns the resulting multicast listener. </summary>
		''' <param name="l"> mouse-wheel-listener-l </param>
		''' <param name="oldl"> the mouse-wheel-listener being removed
		''' @since 1.4 </param>
		Public Shared Function remove(ByVal l As MouseWheelListener, ByVal oldl As MouseWheelListener) As MouseWheelListener
		  Return CType(removeInternal(l, oldl), MouseWheelListener)
		End Function

		''' <summary>
		''' Returns the resulting multicast listener from adding listener-a
		''' and listener-b together.
		''' If listener-a is null, it returns listener-b;
		''' If listener-b is null, it returns listener-a
		''' If neither are null, then it creates and returns
		''' a new AWTEventMulticaster instance which chains a with b. </summary>
		''' <param name="a"> event listener-a </param>
		''' <param name="b"> event listener-b </param>
		Protected Friend Shared Function addInternal(ByVal a As java.util.EventListener, ByVal b As java.util.EventListener) As java.util.EventListener
			If a Is Nothing Then Return b
			If b Is Nothing Then Return a
			Return New AWTEventMulticaster(a, b)
		End Function

		''' <summary>
		''' Returns the resulting multicast listener after removing the
		''' old listener from listener-l.
		''' If listener-l equals the old listener OR listener-l is null,
		''' returns null.
		''' Else if listener-l is an instance of AWTEventMulticaster,
		''' then it removes the old listener from it.
		''' Else, returns listener l. </summary>
		''' <param name="l"> the listener being removed from </param>
		''' <param name="oldl"> the listener being removed </param>
		Protected Friend Shared Function removeInternal(ByVal l As java.util.EventListener, ByVal oldl As java.util.EventListener) As java.util.EventListener
			If l Is oldl OrElse l Is Nothing Then
				Return Nothing
			ElseIf TypeOf l Is AWTEventMulticaster Then
				Return CType(l, AWTEventMulticaster).remove(oldl)
			Else
				Return l ' it's not here
			End If
		End Function


	'     Serialization support.
	'     

		Protected Friend Overridable Sub saveInternal(ByVal s As java.io.ObjectOutputStream, ByVal k As String)
			If TypeOf a Is AWTEventMulticaster Then
				CType(a, AWTEventMulticaster).saveInternal(s, k)
			ElseIf TypeOf a Is java.io.Serializable Then
				s.writeObject(k)
				s.writeObject(a)
			End If

			If TypeOf b Is AWTEventMulticaster Then
				CType(b, AWTEventMulticaster).saveInternal(s, k)
			ElseIf TypeOf b Is java.io.Serializable Then
				s.writeObject(k)
				s.writeObject(b)
			End If
		End Sub

		Protected Friend Shared Sub save(ByVal s As java.io.ObjectOutputStream, ByVal k As String, ByVal l As java.util.EventListener)
		  If l Is Nothing Then
			  Return
		  ElseIf TypeOf l Is AWTEventMulticaster Then
			  CType(l, AWTEventMulticaster).saveInternal(s, k)
		  ElseIf TypeOf l Is java.io.Serializable Then
			   s.writeObject(k)
			   s.writeObject(l)
		  End If
		End Sub

        '    
        '     * Recursive method which returns a count of the number of listeners in
        '     * EventListener, handling the (common) case of l actually being an
        '     * AWTEventMulticaster.  Additionally, only listeners of type listenerType
        '     * are counted.  Method modified to fix bug 4513402.  -bchristi
        '     
        Private Shared Function getListenerCount(ByVal l As java.util.EventListener, ByVal listenerType As [Class]) As Integer
            If TypeOf l Is AWTEventMulticaster Then
                Dim mc As AWTEventMulticaster = CType(l, AWTEventMulticaster)
                Return getListenerCount(mc.a, listenerType) + getListenerCount(mc.b, listenerType)
            Else
                ' Only count listeners of correct type
                Return If(listenerType.isInstance(l), 1, 0)
            End If
        End Function

        '    
        '     * Recusive method which populates EventListener array a with EventListeners
        '     * from l.  l is usually an AWTEventMulticaster.  Bug 4513402 revealed that
        '     * if l differed in type from the element type of a, an ArrayStoreException
        '     * would occur.  Now l is only inserted into a if it's of the appropriate
        '     * type.  -bchristi
        '     
        Private Shared Function populateListenerArray(ByVal a As java.util.EventListener(), ByVal l As java.util.EventListener, ByVal index As Integer) As Integer
			If TypeOf l Is AWTEventMulticaster Then
				Dim mc As AWTEventMulticaster = CType(l, AWTEventMulticaster)
				Dim lhs As Integer = populateListenerArray(a, mc.a, index)
				Return populateListenerArray(a, mc.b, lhs)
			ElseIf a.GetType().GetElementType().isInstance(l) Then
				a(index) = l
				Return index + 1
			' Skip nulls, instances of wrong class
			Else
				Return index
			End If
		End Function

		''' <summary>
		''' Returns an array of all the objects chained as
		''' <code><em>Foo</em>Listener</code>s by the specified
		''' <code>java.util.EventListener</code>.
		''' <code><em>Foo</em>Listener</code>s are chained by the
		''' <code>AWTEventMulticaster</code> using the
		''' <code>add<em>Foo</em>Listener</code> method.
		''' If a <code>null</code> listener is specified, this method returns an
		''' empty array. If the specified listener is not an instance of
		''' <code>AWTEventMulticaster</code>, this method returns an array which
		''' contains only the specified listener. If no such listeners are chained,
		''' this method returns an empty array.
		''' </summary>
		''' <param name="l"> the specified <code>java.util.EventListener</code> </param>
		''' <param name="listenerType"> the type of listeners requested; this parameter
		'''          should specify an interface that descends from
		'''          <code>java.util.EventListener</code> </param>
		''' <returns> an array of all objects chained as
		'''          <code><em>Foo</em>Listener</code>s by the specified multicast
		'''          listener, or an empty array if no such listeners have been
		'''          chained by the specified multicast listener </returns>
		''' <exception cref="NullPointerException"> if the specified
		'''             {@code listenertype} parameter is {@code null} </exception>
		''' <exception cref="ClassCastException"> if <code>listenerType</code>
		'''          doesn't specify a class or interface that implements
		'''          <code>java.util.EventListener</code>
		''' 
		''' @since 1.4 </exception>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
		Public Shared Function getListeners(Of T As java.util.EventListener)(ByVal l As java.util.EventListener, ByVal listenerType As [Class]) As T()
			If listenerType Is Nothing Then Throw New NullPointerException("Listener type should not be null")

			Dim n As Integer = getListenerCount(l, listenerType)
			Dim result As T() = CType(Array.newInstance(listenerType, n), T())
			populateListenerArray(result, l, 0)
			Return result
		End Function
	End Class

End Namespace