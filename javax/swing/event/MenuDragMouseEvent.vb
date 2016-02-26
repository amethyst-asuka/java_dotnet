'
' * Copyright (c) 1998, 2013, Oracle and/or its affiliates. All rights reserved.
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
Namespace javax.swing.event



	''' <summary>
	''' MenuDragMouseEvent is used to notify interested parties that
	''' the menu element has received a MouseEvent forwarded to it
	''' under drag conditions.
	''' <p>
	''' <strong>Warning:</strong>
	''' Serialized objects of this class will not be compatible with
	''' future Swing releases. The current serialization support is
	''' appropriate for short term storage or RMI between applications running
	''' the same version of Swing.  As of 1.4, support for long term storage
	''' of all JavaBeans&trade;
	''' has been added to the <code>java.beans</code> package.
	''' Please see <seealso cref="java.beans.XMLEncoder"/>.
	''' 
	''' @author Georges Saab
	''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
	Public Class MenuDragMouseEvent
		Inherits java.awt.event.MouseEvent

		Private path As javax.swing.MenuElement()
		Private manager As javax.swing.MenuSelectionManager

		''' <summary>
		''' Constructs a MenuDragMouseEvent object.
		''' <p>Absolute coordinates xAbs and yAbs are set to source's location on screen plus
		''' relative coordinates x and y. xAbs and yAbs are set to zero if the source is not showing.
		''' </summary>
		''' <param name="source">        the Component that originated the event
		'''                      (typically <code>this</code>) </param>
		''' <param name="id">            an int specifying the type of event, as defined
		'''                      in <seealso cref="java.awt.event.MouseEvent"/> </param>
		''' <param name="when">          a long identifying the time the event occurred </param>
		''' <param name="modifiers">     an int specifying any modifier keys held down,
		'''                      as specified in <seealso cref="java.awt.event.InputEvent"/> </param>
		''' <param name="x">             an int specifying the horizontal position at which
		'''                      the event occurred, in pixels </param>
		''' <param name="y">             an int specifying the vertical position at which
		'''                      the event occurred, in pixels </param>
		''' <param name="clickCount">    an int specifying the number of mouse-clicks </param>
		''' <param name="popupTrigger">  a boolean -- true if the event {should?/did?}
		'''                      trigger a popup </param>
		''' <param name="p">             an array of MenuElement objects specifying a path
		'''                        to a menu item affected by the drag </param>
		''' <param name="m">             a MenuSelectionManager object that handles selections </param>
		''' <seealso cref= MouseEvent#MouseEvent(java.awt.Component, int, long, int, int, int, int, int, int, boolean, int) </seealso>
'JAVA TO VB CONVERTER TODO TASK: The following line could not be converted:
		public MenuDragMouseEvent(java.awt.Component source, int id, long when, int modifiers, int x, int y, int clickCount, boolean popupTrigger, javax.swing.MenuElement p() , javax.swing.MenuSelectionManager m)
			MyBase(source, id, [when], modifiers, x, y, clickCount, popupTrigger)
			path = p
			manager = m

		''' <summary>
		''' Constructs a MenuDragMouseEvent object.
		''' <p>Even if inconsistent values for relative and absolute coordinates are
		''' passed to the constructor, the MenuDragMouseEvent instance is still
		''' created. </summary>
		''' <param name="source">        the Component that originated the event
		'''                      (typically <code>this</code>) </param>
		''' <param name="id">            an int specifying the type of event, as defined
		'''                      in <seealso cref="java.awt.event.MouseEvent"/> </param>
		''' <param name="when">          a long identifying the time the event occurred </param>
		''' <param name="modifiers">     an int specifying any modifier keys held down,
		'''                      as specified in <seealso cref="java.awt.event.InputEvent"/> </param>
		''' <param name="x">             an int specifying the horizontal position at which
		'''                      the event occurred, in pixels </param>
		''' <param name="y">             an int specifying the vertical position at which
		'''                      the event occurred, in pixels </param>
		''' <param name="xAbs">          an int specifying the horizontal absolute position at which
		'''                      the event occurred, in pixels </param>
		''' <param name="yAbs">          an int specifying the vertical absolute position at which
		'''                      the event occurred, in pixels </param>
		''' <param name="clickCount">    an int specifying the number of mouse-clicks </param>
		''' <param name="popupTrigger">  a boolean -- true if the event {should?/did?}
		'''                      trigger a popup </param>
		''' <param name="p">             an array of MenuElement objects specifying a path
		'''                        to a menu item affected by the drag </param>
		''' <param name="m">             a MenuSelectionManager object that handles selections </param>
		''' <seealso cref= MouseEvent#MouseEvent(java.awt.Component, int, long, int, int, int, int, int, int, boolean, int)
		''' @since 1.6 </seealso>
		public MenuDragMouseEvent(java.awt.Component source, Integer id, Long [when], Integer modifiers, Integer x, Integer y, Integer xAbs, Integer yAbs, Integer clickCount, Boolean popupTrigger, javax.swing.MenuElement p() , javax.swing.MenuSelectionManager m)
			MyBase(source, id, [when], modifiers, x, y, xAbs, yAbs, clickCount, popupTrigger, java.awt.event.MouseEvent.NOBUTTON)
			path = p
			manager = m

		''' <summary>
		''' Returns the path to the selected menu item.
		''' </summary>
		''' <returns> an array of MenuElement objects representing the path value </returns>
		public javax.swing.MenuElement() path
			Return path

		''' <summary>
		''' Returns the current menu selection manager.
		''' </summary>
		''' <returns> a MenuSelectionManager object </returns>
		public javax.swing.MenuSelectionManager menuSelectionManager
			Return manager
	End Class

End Namespace