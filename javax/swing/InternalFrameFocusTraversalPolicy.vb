'
' * Copyright (c) 2000, Oracle and/or its affiliates. All rights reserved.
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
	''' A FocusTraversalPolicy which can optionally provide an algorithm for
	''' determining a JInternalFrame's initial Component. The initial Component is
	''' the first to receive focus when the JInternalFrame is first selected. By
	''' default, this is the same as the JInternalFrame's default Component to
	''' focus.
	''' 
	''' @author David Mendenhall
	''' 
	''' @since 1.4
	''' </summary>
	Public MustInherit Class InternalFrameFocusTraversalPolicy
		Inherits java.awt.FocusTraversalPolicy

		''' <summary>
		''' Returns the Component that should receive the focus when a
		''' JInternalFrame is selected for the first time. Once the JInternalFrame
		''' has been selected by a call to <code>setSelected(true)</code>, the
		''' initial Component will not be used again. Instead, if the JInternalFrame
		''' loses and subsequently regains selection, or is made invisible or
		''' undisplayable and subsequently made visible and displayable, the
		''' JInternalFrame's most recently focused Component will become the focus
		''' owner. The default implementation of this method returns the
		''' JInternalFrame's default Component to focus.
		''' </summary>
		''' <param name="frame"> the JInternalFrame whose initial Component is to be
		'''        returned </param>
		''' <returns> the Component that should receive the focus when frame is
		'''         selected for the first time, or null if no suitable Component
		'''         can be found </returns>
		''' <seealso cref= JInternalFrame#getMostRecentFocusOwner </seealso>
		''' <exception cref="IllegalArgumentException"> if window is null </exception>
		Public Overridable Function getInitialComponent(ByVal frame As JInternalFrame) As java.awt.Component
			Return getDefaultComponent(frame)
		End Function
	End Class

End Namespace