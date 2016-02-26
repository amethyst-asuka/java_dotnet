Imports System

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
Namespace javax.swing


	''' <summary>
	''' A KeyStroke represents a key action on the keyboard, or equivalent input
	''' device. KeyStrokes can correspond to only a press or release of a particular
	''' key, just as KEY_PRESSED and KEY_RELEASED KeyEvents do; alternately, they
	''' can correspond to typing a specific Java character, just as KEY_TYPED
	''' KeyEvents do. In all cases, KeyStrokes can specify modifiers (alt, shift,
	''' control, meta, altGraph, or a combination thereof) which must be present during the
	''' action for an exact match.
	''' <p>
	''' KeyStrokes are used to define high-level (semantic) action events. Instead
	''' of trapping every keystroke and throwing away the ones you are not
	''' interested in, those keystrokes you care about automatically initiate
	''' actions on the Components with which they are registered.
	''' <p>
	''' KeyStrokes are immutable, and are intended to be unique. Client code cannot
	''' create a KeyStroke; a variant of <code>getKeyStroke</code> must be used
	''' instead. These factory methods allow the KeyStroke implementation to cache
	''' and share instances efficiently.
	''' <p>
	''' <strong>Warning:</strong>
	''' Serialized objects of this class will not be compatible with
	''' future Swing releases. The current serialization support is
	''' appropriate for short term storage or RMI between applications running
	''' the same version of Swing.  As of 1.4, support for long term storage
	''' of all JavaBeans&trade;
	''' has been added to the <code>java.beans</code> package.
	''' Please see <seealso cref="java.beans.XMLEncoder"/>.
	''' </summary>
	''' <seealso cref= javax.swing.text.Keymap </seealso>
	''' <seealso cref= #getKeyStroke
	''' 
	''' @author Arnaud Weber
	''' @author David Mendenhall </seealso>
	Public Class KeyStroke
		Inherits java.awt.AWTKeyStroke

		''' <summary>
		''' Serial Version ID.
		''' </summary>
		Private Const serialVersionUID As Long = -9060180771037902530L

		Private Sub New()
		End Sub
		Private Sub New(ByVal keyChar As Char, ByVal keyCode As Integer, ByVal modifiers As Integer, ByVal onKeyRelease As Boolean)
			MyBase.New(keyChar, keyCode, modifiers, onKeyRelease)
		End Sub

		''' <summary>
		''' Returns a shared instance of a <code>KeyStroke</code>
		''' that represents a <code>KEY_TYPED</code> event for the
		''' specified character.
		''' </summary>
		''' <param name="keyChar"> the character value for a keyboard key </param>
		''' <returns> a KeyStroke object for that key </returns>
		Public Shared Function getKeyStroke(ByVal keyChar As Char) As KeyStroke
			SyncLock GetType(java.awt.AWTKeyStroke)
				registerSubclass(GetType(KeyStroke))
				Return CType(getAWTKeyStroke(keyChar), KeyStroke)
			End SyncLock
		End Function

		''' <summary>
		''' Returns an instance of a KeyStroke, specifying whether the key is
		''' considered to be activated when it is pressed or released. Unlike all
		''' other factory methods in this class, the instances returned by this
		''' method are not necessarily cached or shared.
		''' </summary>
		''' <param name="keyChar"> the character value for a keyboard key </param>
		''' <param name="onKeyRelease"> <code>true</code> if this KeyStroke corresponds to a
		'''        key release; <code>false</code> otherwise. </param>
		''' <returns> a KeyStroke object for that key </returns>
		''' @deprecated use getKeyStroke(char) 
		<Obsolete("use getKeyStroke(char)")> _
		Public Shared Function getKeyStroke(ByVal keyChar As Char, ByVal onKeyRelease As Boolean) As KeyStroke
			Return New KeyStroke(keyChar, java.awt.event.KeyEvent.VK_UNDEFINED, 0, onKeyRelease)
		End Function

		''' <summary>
		''' Returns a shared instance of a {@code KeyStroke}
		''' that represents a {@code KEY_TYPED} event for the
		''' specified Character object and a
		''' set of modifiers. Note that the first parameter is of type Character
		''' rather than char. This is to avoid inadvertent clashes with calls to
		''' <code>getKeyStroke(int keyCode, int modifiers)</code>.
		''' 
		''' The modifiers consist of any combination of following:<ul>
		''' <li>java.awt.event.InputEvent.SHIFT_DOWN_MASK
		''' <li>java.awt.event.InputEvent.CTRL_DOWN_MASK
		''' <li>java.awt.event.InputEvent.META_DOWN_MASK
		''' <li>java.awt.event.InputEvent.ALT_DOWN_MASK
		''' <li>java.awt.event.InputEvent.ALT_GRAPH_DOWN_MASK
		''' </ul>
		''' The old modifiers listed below also can be used, but they are
		''' mapped to _DOWN_ modifiers. <ul>
		''' <li>java.awt.event.InputEvent.SHIFT_MASK
		''' <li>java.awt.event.InputEvent.CTRL_MASK
		''' <li>java.awt.event.InputEvent.META_MASK
		''' <li>java.awt.event.InputEvent.ALT_MASK
		''' <li>java.awt.event.InputEvent.ALT_GRAPH_MASK
		''' </ul>
		''' also can be used, but they are mapped to _DOWN_ modifiers.
		''' 
		''' Since these numbers are all different powers of two, any combination of
		''' them is an integer in which each bit represents a different modifier
		''' key. Use 0 to specify no modifiers.
		''' </summary>
		''' <param name="keyChar"> the Character object for a keyboard character </param>
		''' <param name="modifiers"> a bitwise-ored combination of any modifiers </param>
		''' <returns> an KeyStroke object for that key </returns>
		''' <exception cref="IllegalArgumentException"> if keyChar is null
		''' </exception>
		''' <seealso cref= java.awt.event.InputEvent
		''' @since 1.3 </seealso>
		Public Shared Function getKeyStroke(ByVal keyChar As Char?, ByVal modifiers As Integer) As KeyStroke
			SyncLock GetType(java.awt.AWTKeyStroke)
				registerSubclass(GetType(KeyStroke))
				Return CType(getAWTKeyStroke(keyChar, modifiers), KeyStroke)
			End SyncLock
		End Function

		''' <summary>
		''' Returns a shared instance of a KeyStroke, given a numeric key code and a
		''' set of modifiers, specifying whether the key is activated when it is
		''' pressed or released.
		''' <p>
		''' The "virtual key" constants defined in java.awt.event.KeyEvent can be
		''' used to specify the key code. For example:<ul>
		''' <li>java.awt.event.KeyEvent.VK_ENTER
		''' <li>java.awt.event.KeyEvent.VK_TAB
		''' <li>java.awt.event.KeyEvent.VK_SPACE
		''' </ul>
		''' Alternatively, the key code may be obtained by calling
		''' <code>java.awt.event.KeyEvent.getExtendedKeyCodeForChar</code>.
		''' 
		''' The modifiers consist of any combination of:<ul>
		''' <li>java.awt.event.InputEvent.SHIFT_DOWN_MASK
		''' <li>java.awt.event.InputEvent.CTRL_DOWN_MASK
		''' <li>java.awt.event.InputEvent.META_DOWN_MASK
		''' <li>java.awt.event.InputEvent.ALT_DOWN_MASK
		''' <li>java.awt.event.InputEvent.ALT_GRAPH_DOWN_MASK
		''' </ul>
		''' The old modifiers <ul>
		''' <li>java.awt.event.InputEvent.SHIFT_MASK
		''' <li>java.awt.event.InputEvent.CTRL_MASK
		''' <li>java.awt.event.InputEvent.META_MASK
		''' <li>java.awt.event.InputEvent.ALT_MASK
		''' <li>java.awt.event.InputEvent.ALT_GRAPH_MASK
		''' </ul>
		''' also can be used, but they are mapped to _DOWN_ modifiers.
		''' 
		''' Since these numbers are all different powers of two, any combination of
		''' them is an integer in which each bit represents a different modifier
		''' key. Use 0 to specify no modifiers.
		''' </summary>
		''' <param name="keyCode"> an int specifying the numeric code for a keyboard key </param>
		''' <param name="modifiers"> a bitwise-ored combination of any modifiers </param>
		''' <param name="onKeyRelease"> <code>true</code> if the KeyStroke should represent
		'''        a key release; <code>false</code> otherwise. </param>
		''' <returns> a KeyStroke object for that key
		''' </returns>
		''' <seealso cref= java.awt.event.KeyEvent </seealso>
		''' <seealso cref= java.awt.event.InputEvent </seealso>
		Public Shared Function getKeyStroke(ByVal keyCode As Integer, ByVal modifiers As Integer, ByVal onKeyRelease As Boolean) As KeyStroke
			SyncLock GetType(java.awt.AWTKeyStroke)
				registerSubclass(GetType(KeyStroke))
				Return CType(getAWTKeyStroke(keyCode, modifiers, onKeyRelease), KeyStroke)
			End SyncLock
		End Function

		''' <summary>
		''' Returns a shared instance of a KeyStroke, given a numeric key code and a
		''' set of modifiers. The returned KeyStroke will correspond to a key press.
		''' <p>
		''' The "virtual key" constants defined in java.awt.event.KeyEvent can be
		''' used to specify the key code. For example:<ul>
		''' <li>java.awt.event.KeyEvent.VK_ENTER
		''' <li>java.awt.event.KeyEvent.VK_TAB
		''' <li>java.awt.event.KeyEvent.VK_SPACE
		''' </ul>
		''' Alternatively, the key code may be obtained by calling
		''' <code>java.awt.event.KeyEvent.getExtendedKeyCodeForChar</code>.
		''' 
		''' The modifiers consist of any combination of:<ul>
		''' <li>java.awt.event.InputEvent.SHIFT_DOWN_MASK
		''' <li>java.awt.event.InputEvent.CTRL_DOWN_MASK
		''' <li>java.awt.event.InputEvent.META_DOWN_MASK
		''' <li>java.awt.event.InputEvent.ALT_DOWN_MASK
		''' <li>java.awt.event.InputEvent.ALT_GRAPH_DOWN_MASK
		''' </ul>
		''' The old modifiers <ul>
		''' <li>java.awt.event.InputEvent.SHIFT_MASK
		''' <li>java.awt.event.InputEvent.CTRL_MASK
		''' <li>java.awt.event.InputEvent.META_MASK
		''' <li>java.awt.event.InputEvent.ALT_MASK
		''' <li>java.awt.event.InputEvent.ALT_GRAPH_MASK
		''' </ul>
		''' also can be used, but they are mapped to _DOWN_ modifiers.
		''' 
		''' Since these numbers are all different powers of two, any combination of
		''' them is an integer in which each bit represents a different modifier
		''' key. Use 0 to specify no modifiers.
		''' </summary>
		''' <param name="keyCode"> an int specifying the numeric code for a keyboard key </param>
		''' <param name="modifiers"> a bitwise-ored combination of any modifiers </param>
		''' <returns> a KeyStroke object for that key
		''' </returns>
		''' <seealso cref= java.awt.event.KeyEvent </seealso>
		''' <seealso cref= java.awt.event.InputEvent </seealso>
		Public Shared Function getKeyStroke(ByVal keyCode As Integer, ByVal modifiers As Integer) As KeyStroke
			SyncLock GetType(java.awt.AWTKeyStroke)
				registerSubclass(GetType(KeyStroke))
				Return CType(getAWTKeyStroke(keyCode, modifiers), KeyStroke)
			End SyncLock
		End Function

		''' <summary>
		''' Returns a KeyStroke which represents the stroke which generated a given
		''' KeyEvent.
		''' <p>
		''' This method obtains the keyChar from a KeyTyped event, and the keyCode
		''' from a KeyPressed or KeyReleased event. The KeyEvent modifiers are
		''' obtained for all three types of KeyEvent.
		''' </summary>
		''' <param name="anEvent"> the KeyEvent from which to obtain the KeyStroke </param>
		''' <exception cref="NullPointerException"> if <code>anEvent</code> is null </exception>
		''' <returns> the KeyStroke that precipitated the event </returns>
		Public Shared Function getKeyStrokeForEvent(ByVal anEvent As java.awt.event.KeyEvent) As KeyStroke
			SyncLock GetType(java.awt.AWTKeyStroke)
				registerSubclass(GetType(KeyStroke))
				Return CType(getAWTKeyStrokeForEvent(anEvent), KeyStroke)
			End SyncLock
		End Function

		''' <summary>
		''' Parses a string and returns a <code>KeyStroke</code>.
		''' The string must have the following syntax:
		''' <pre>
		'''    &lt;modifiers&gt;* (&lt;typedID&gt; | &lt;pressedReleasedID&gt;)
		''' 
		'''    modifiers := shift | control | ctrl | meta | alt | altGraph
		'''    typedID := typed &lt;typedKey&gt;
		'''    typedKey := string of length 1 giving Unicode character.
		'''    pressedReleasedID := (pressed | released) key
		'''    key := KeyEvent key code name, i.e. the name following "VK_".
		''' </pre>
		''' If typed, pressed or released is not specified, pressed is assumed. Here
		''' are some examples:
		''' <pre>
		'''     "INSERT" =&gt; getKeyStroke(KeyEvent.VK_INSERT, 0);
		'''     "control DELETE" =&gt; getKeyStroke(KeyEvent.VK_DELETE, InputEvent.CTRL_MASK);
		'''     "alt shift X" =&gt; getKeyStroke(KeyEvent.VK_X, InputEvent.ALT_MASK | InputEvent.SHIFT_MASK);
		'''     "alt shift released X" =&gt; getKeyStroke(KeyEvent.VK_X, InputEvent.ALT_MASK | InputEvent.SHIFT_MASK, true);
		'''     "typed a" =&gt; getKeyStroke('a');
		''' </pre>
		''' 
		''' In order to maintain backward-compatibility, specifying a null String,
		''' or a String which is formatted incorrectly, returns null.
		''' </summary>
		''' <param name="s"> a String formatted as described above </param>
		''' <returns> a KeyStroke object for that String, or null if the specified
		'''         String is null, or is formatted incorrectly
		''' </returns>
		''' <seealso cref= java.awt.event.KeyEvent </seealso>
		Public Shared Function getKeyStroke(ByVal s As String) As KeyStroke
			If s Is Nothing OrElse s.Length = 0 Then Return Nothing
			SyncLock GetType(java.awt.AWTKeyStroke)
				registerSubclass(GetType(KeyStroke))
				Try
					Return CType(getAWTKeyStroke(s), KeyStroke)
				Catch e As System.ArgumentException
					Return Nothing
				End Try
			End SyncLock
		End Function
	End Class

End Namespace