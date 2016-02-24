'
' * Copyright (c) 1995, 2013, Oracle and/or its affiliates. All rights reserved.
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
	''' The peer interface for <seealso cref="TextComponent"/>.
	''' 
	''' The peer interfaces are intended only for use in porting
	''' the AWT. They are not intended for use by application
	''' developers, and developers should not implement peers
	''' nor invoke any of the peer methods directly on the peer
	''' instances.
	''' </summary>
	Public Interface TextComponentPeer
		Inherits ComponentPeer

		''' <summary>
		''' Sets if the text component should be editable or not.
		''' </summary>
		''' <param name="editable"> {@code true} for editable text components,
		'''        {@code false} for non-editable text components
		''' </param>
		''' <seealso cref= TextComponent#setEditable(boolean) </seealso>
		WriteOnly Property editable As Boolean

		''' <summary>
		''' Returns the current content of the text component.
		''' </summary>
		''' <returns> the current content of the text component
		''' </returns>
		''' <seealso cref= TextComponent#getText() </seealso>
		Property text As String


		''' <summary>
		''' Returns the start index of the current selection.
		''' </summary>
		''' <returns> the start index of the current selection
		''' </returns>
		''' <seealso cref= TextComponent#getSelectionStart() </seealso>
		ReadOnly Property selectionStart As Integer

		''' <summary>
		''' Returns the end index of the current selection.
		''' </summary>
		''' <returns> the end index of the current selection
		''' </returns>
		''' <seealso cref= TextComponent#getSelectionEnd() </seealso>
		ReadOnly Property selectionEnd As Integer

		''' <summary>
		''' Selects an area of the text component.
		''' </summary>
		''' <param name="selStart"> the start index of the new selection </param>
		''' <param name="selEnd"> the end index of the new selection
		''' </param>
		''' <seealso cref= TextComponent#select(int, int) </seealso>
		Sub [select](ByVal selStart As Integer, ByVal selEnd As Integer)

		''' <summary>
		''' Sets the caret position of the text component.
		''' </summary>
		''' <param name="pos"> the caret position to set
		''' </param>
		''' <seealso cref= TextComponent#setCaretPosition(int) </seealso>
		Property caretPosition As Integer


		''' <summary>
		''' Returns the input method requests.
		''' </summary>
		''' <returns> the input method requests </returns>
		ReadOnly Property inputMethodRequests As java.awt.im.InputMethodRequests
	End Interface

End Namespace