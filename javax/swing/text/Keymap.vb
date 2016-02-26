'
' * Copyright (c) 1997, 2011, Oracle and/or its affiliates. All rights reserved.
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
Namespace javax.swing.text


	''' <summary>
	''' A collection of bindings of KeyStrokes to actions.  The
	''' bindings are basically name-value pairs that potentially
	''' resolve in a hierarchy.
	''' 
	''' @author  Timothy Prinzing
	''' </summary>
	Public Interface Keymap

		''' <summary>
		''' Fetches the name of the set of key-bindings.
		''' </summary>
		''' <returns> the name </returns>
		ReadOnly Property name As String

		''' <summary>
		''' Fetches the default action to fire if a
		''' key is typed (i.e. a KEY_TYPED KeyEvent is received)
		''' and there is no binding for it.  Typically this
		''' would be some action that inserts text so that
		''' the keymap doesn't require an action for each
		''' possible key.
		''' </summary>
		''' <returns> the default action </returns>
		Property defaultAction As javax.swing.Action


		''' <summary>
		''' Fetches the action appropriate for the given symbolic
		''' event sequence.  This is used by JTextController to
		''' determine how to interpret key sequences.  If the
		''' binding is not resolved locally, an attempt is made
		''' to resolve through the parent keymap, if one is set.
		''' </summary>
		''' <param name="key"> the key sequence </param>
		''' <returns>  the action associated with the key
		'''  sequence if one is defined, otherwise <code>null</code> </returns>
		Function getAction(ByVal key As javax.swing.KeyStroke) As javax.swing.Action

		''' <summary>
		''' Fetches all of the keystrokes in this map that
		''' are bound to some action.
		''' </summary>
		''' <returns> the list of keystrokes </returns>
		ReadOnly Property boundKeyStrokes As javax.swing.KeyStroke()

		''' <summary>
		''' Fetches all of the actions defined in this keymap.
		''' </summary>
		''' <returns> the list of actions </returns>
		ReadOnly Property boundActions As javax.swing.Action()

		''' <summary>
		''' Fetches the keystrokes that will result in
		''' the given action.
		''' </summary>
		''' <param name="a"> the action </param>
		''' <returns> the list of keystrokes </returns>
		Function getKeyStrokesForAction(ByVal a As javax.swing.Action) As javax.swing.KeyStroke()

		''' <summary>
		''' Determines if the given key sequence is locally defined.
		''' </summary>
		''' <param name="key"> the key sequence </param>
		''' <returns> true if the key sequence is locally defined else false </returns>
		Function isLocallyDefined(ByVal key As javax.swing.KeyStroke) As Boolean

		''' <summary>
		''' Adds a binding to the keymap.
		''' </summary>
		''' <param name="key"> the key sequence </param>
		''' <param name="a"> the action </param>
		Sub addActionForKeyStroke(ByVal key As javax.swing.KeyStroke, ByVal a As javax.swing.Action)

		''' <summary>
		''' Removes a binding from the keymap.
		''' </summary>
		''' <param name="keys"> the key sequence </param>
		Sub removeKeyStrokeBinding(ByVal keys As javax.swing.KeyStroke)

		''' <summary>
		''' Removes all bindings from the keymap.
		''' </summary>
		Sub removeBindings()

		''' <summary>
		''' Fetches the parent keymap used to resolve key-bindings.
		''' </summary>
		''' <returns> the keymap </returns>
		Property resolveParent As Keymap


	End Interface

End Namespace