'
' * Copyright (c) 2003, 2012, Oracle and/or its affiliates. All rights reserved.
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

Namespace java.awt.peer


	''' <summary>
	''' The native peer interface for <seealso cref="KeyboardFocusManager"/>.
	''' </summary>
	Public Interface KeyboardFocusManagerPeer

		''' <summary>
		''' Sets the window that should become the focused window.
		''' </summary>
		''' <param name="win"> the window that should become the focused window
		'''  </param>
		Property currentFocusedWindow As java.awt.Window


		''' <summary>
		''' Sets the component that should become the focus owner.
		''' </summary>
		''' <param name="comp"> the component to become the focus owner
		''' </param>
		''' <seealso cref= KeyboardFocusManager#setNativeFocusOwner(Component) </seealso>
		Property currentFocusOwner As java.awt.Component


		''' <summary>
		''' Clears the current global focus owner.
		''' </summary>
		''' <param name="activeWindow">
		''' </param>
		''' <seealso cref= KeyboardFocusManager#clearGlobalFocusOwner() </seealso>
		Sub clearGlobalFocusOwner(  activeWindow As java.awt.Window)

	End Interface

End Namespace