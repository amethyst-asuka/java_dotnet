Imports System

'
' * Copyright (c) 1996, 2009, Oracle and/or its affiliates. All rights reserved.
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
	''' The <code>MenuShortcut</code>class represents a keyboard accelerator
	''' for a MenuItem.
	''' <p>
	''' Menu shortcuts are created using virtual keycodes, not characters.
	''' For example, a menu shortcut for Ctrl-a (assuming that Control is
	''' the accelerator key) would be created with code like the following:
	''' <p>
	''' <code>MenuShortcut ms = new MenuShortcut(KeyEvent.VK_A, false);</code>
	''' <p> or alternatively
	''' <p>
	''' <code>MenuShortcut ms = new MenuShortcut(KeyEvent.getExtendedKeyCodeForChar('A'), false);</code>
	''' <p>
	''' Menu shortcuts may also be constructed for a wider set of keycodes
	''' using the <code>java.awt.event.KeyEvent.getExtendedKeyCodeForChar</code> call.
	''' For example, a menu shortcut for "Ctrl+cyrillic ef" is created by
	''' <p>
	''' <code>MenuShortcut ms = new MenuShortcut(KeyEvent.getExtendedKeyCodeForChar('\u0444'), false);</code>
	''' <p>
	''' Note that shortcuts created with a keycode or an extended keycode defined as a constant in <code>KeyEvent</code>
	''' work regardless of the current keyboard layout. However, a shortcut made of
	''' an extended keycode not listed in <code>KeyEvent</code>
	''' only work if the current keyboard layout produces a corresponding letter.
	''' <p>
	''' The accelerator key is platform-dependent and may be obtained
	''' via <seealso cref="Toolkit#getMenuShortcutKeyMask"/>.
	''' 
	''' @author Thomas Ball
	''' @since JDK1.1
	''' </summary>
	<Serializable> _
	Public Class MenuShortcut
		''' <summary>
		''' The virtual keycode for the menu shortcut.
		''' This is the keycode with which the menu shortcut will be created.
		''' Note that it is a virtual keycode, not a character,
		''' e.g. KeyEvent.VK_A, not 'a'.
		''' Note: in 1.1.x you must use setActionCommand() on a menu item
		''' in order for its shortcut to work, otherwise it will fire a null
		''' action command.
		''' 
		''' @serial </summary>
		''' <seealso cref= #getKey() </seealso>
		''' <seealso cref= #usesShiftModifier() </seealso>
		''' <seealso cref= java.awt.event.KeyEvent
		''' @since JDK1.1 </seealso>
		Friend key As Integer

		''' <summary>
		''' Indicates whether the shft key was pressed.
		''' If true, the shift key was pressed.
		''' If false, the shift key was not pressed
		''' 
		''' @serial </summary>
		''' <seealso cref= #usesShiftModifier()
		''' @since JDK1.1 </seealso>
		Friend usesShift As Boolean

	'    
	'     * JDK 1.1 serialVersionUID
	'     
		 Private Const serialVersionUID As Long = 143448358473180225L

		''' <summary>
		''' Constructs a new MenuShortcut for the specified virtual keycode. </summary>
		''' <param name="key"> the raw keycode for this MenuShortcut, as would be returned
		''' in the keyCode field of a <seealso cref="java.awt.event.KeyEvent KeyEvent"/> if
		''' this key were pressed. </param>
		''' <seealso cref= java.awt.event.KeyEvent
		'''  </seealso>
		Public Sub New(ByVal key As Integer)
			Me.New(key, False)
		End Sub

		''' <summary>
		''' Constructs a new MenuShortcut for the specified virtual keycode. </summary>
		''' <param name="key"> the raw keycode for this MenuShortcut, as would be returned
		''' in the keyCode field of a <seealso cref="java.awt.event.KeyEvent KeyEvent"/> if
		''' this key were pressed. </param>
		''' <param name="useShiftModifier"> indicates whether this MenuShortcut is invoked
		''' with the SHIFT key down. </param>
		''' <seealso cref= java.awt.event.KeyEvent
		'''  </seealso>
		Public Sub New(ByVal key As Integer, ByVal useShiftModifier As Boolean)
			Me.key = key
			Me.usesShift = useShiftModifier
		End Sub

		''' <summary>
		''' Returns the raw keycode of this MenuShortcut. </summary>
		''' <returns> the raw keycode of this MenuShortcut. </returns>
		''' <seealso cref= java.awt.event.KeyEvent
		''' @since JDK1.1 </seealso>
		Public Overridable Property key As Integer
			Get
				Return key
			End Get
		End Property

		''' <summary>
		''' Returns whether this MenuShortcut must be invoked using the SHIFT key. </summary>
		''' <returns> <code>true</code> if this MenuShortcut must be invoked using the
		''' SHIFT key, <code>false</code> otherwise.
		''' @since JDK1.1 </returns>
		Public Overridable Function usesShiftModifier() As Boolean
			Return usesShift
		End Function

		''' <summary>
		''' Returns whether this MenuShortcut is the same as another:
		''' equality is defined to mean that both MenuShortcuts use the same key
		''' and both either use or don't use the SHIFT key. </summary>
		''' <param name="s"> the MenuShortcut to compare with this. </param>
		''' <returns> <code>true</code> if this MenuShortcut is the same as another,
		''' <code>false</code> otherwise.
		''' @since JDK1.1 </returns>
		Public Overrides Function Equals(ByVal s As MenuShortcut) As Boolean
			Return (s IsNot Nothing AndAlso (s.key = key) AndAlso (s.usesShiftModifier() = usesShift))
		End Function

		''' <summary>
		''' Returns whether this MenuShortcut is the same as another:
		''' equality is defined to mean that both MenuShortcuts use the same key
		''' and both either use or don't use the SHIFT key. </summary>
		''' <param name="obj"> the Object to compare with this. </param>
		''' <returns> <code>true</code> if this MenuShortcut is the same as another,
		''' <code>false</code> otherwise.
		''' @since 1.2 </returns>
		Public Overrides Function Equals(ByVal obj As Object) As Boolean
			If TypeOf obj Is MenuShortcut Then Return Equals(CType(obj, MenuShortcut))
			Return False
		End Function

		''' <summary>
		''' Returns the hashcode for this MenuShortcut. </summary>
		''' <returns> the hashcode for this MenuShortcut.
		''' @since 1.2 </returns>
		Public Overrides Function GetHashCode() As Integer
			Return If(usesShift, ((Not key)), key)
		End Function

		''' <summary>
		''' Returns an internationalized description of the MenuShortcut. </summary>
		''' <returns> a string representation of this MenuShortcut.
		''' @since JDK1.1 </returns>
		Public Overrides Function ToString() As String
			Dim modifiers As Integer = 0
			If Not GraphicsEnvironment.headless Then modifiers = Toolkit.defaultToolkit.menuShortcutKeyMask
			If usesShiftModifier() Then modifiers = modifiers Or Event.SHIFT_MASK
			Return java.awt.event.KeyEvent.getKeyModifiersText(modifiers) & "+" & java.awt.event.KeyEvent.getKeyText(key)
		End Function

		''' <summary>
		''' Returns the parameter string representing the state of this
		''' MenuShortcut. This string is useful for debugging. </summary>
		''' <returns>    the parameter string of this MenuShortcut.
		''' @since JDK1.1 </returns>
		Protected Friend Overridable Function paramString() As String
			Dim str As String = "key=" & key
			If usesShiftModifier() Then str &= ",usesShiftModifier"
			Return str
		End Function
	End Class

End Namespace