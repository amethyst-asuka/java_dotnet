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
	''' MenuKeyEvent is used to notify interested parties that
	''' the menu element has received a KeyEvent forwarded to it
	''' in a menu tree.
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
	Public Class MenuKeyEvent
		Inherits java.awt.event.KeyEvent

		Private path As javax.swing.MenuElement()
		Private manager As javax.swing.MenuSelectionManager

		''' <summary>
		''' Constructs a MenuKeyEvent object.
		''' </summary>
		''' <param name="source">     the Component that originated the event
		'''                     (typically <code>this</code>) </param>
		''' <param name="id">         an int specifying the type of event, as defined
		'''                     in <seealso cref="java.awt.event.KeyEvent"/> </param>
		''' <param name="when">       a long identifying the time the event occurred </param>
		''' <param name="modifiers">     an int specifying any modifier keys held down,
		'''                      as specified in <seealso cref="java.awt.event.InputEvent"/> </param>
		''' <param name="keyCode">    an int specifying the specific key that was pressed </param>
		''' <param name="keyChar">    a char specifying the key's character value, if any
		'''                   -- null if the key has no character value </param>
		''' <param name="p">          an array of MenuElement objects specifying a path
		'''                     to a menu item affected by the drag </param>
		''' <param name="m">          a MenuSelectionManager object that handles selections </param>
'JAVA TO VB CONVERTER TODO TASK: The following line could not be converted:
		public MenuKeyEvent(java.awt.Component source, int id, long when, int modifiers, int keyCode, char keyChar, javax.swing.MenuElement p() , javax.swing.MenuSelectionManager m)
			MyBase(source, id, [when], modifiers, keyCode, keyChar)
			path = p
			manager = m

		''' <summary>
		''' Returns the path to the menu item referenced by this event.
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