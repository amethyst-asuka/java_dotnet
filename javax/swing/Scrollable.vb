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

Namespace javax.swing



	''' <summary>
	''' An interface that provides information to a scrolling container
	''' like JScrollPane.  A complex component that's likely to be used
	''' as a viewing a JScrollPane viewport (or other scrolling container)
	''' should implement this interface.
	''' </summary>
	''' <seealso cref= JViewport </seealso>
	''' <seealso cref= JScrollPane </seealso>
	''' <seealso cref= JScrollBar
	''' @author Hans Muller </seealso>
	Public Interface Scrollable
		''' <summary>
		''' Returns the preferred size of the viewport for a view component.
		''' For example, the preferred size of a <code>JList</code> component
		''' is the size required to accommodate all of the cells in its list.
		''' However, the value of <code>preferredScrollableViewportSize</code>
		''' is the size required for <code>JList.getVisibleRowCount</code> rows.
		''' A component without any properties that would affect the viewport
		''' size should just return <code>getPreferredSize</code> here.
		''' </summary>
		''' <returns> the preferredSize of a <code>JViewport</code> whose view
		'''    is this <code>Scrollable</code> </returns>
		''' <seealso cref= JViewport#getPreferredSize </seealso>
		ReadOnly Property preferredScrollableViewportSize As java.awt.Dimension


		''' <summary>
		''' Components that display logical rows or columns should compute
		''' the scroll increment that will completely expose one new row
		''' or column, depending on the value of orientation.  Ideally,
		''' components should handle a partially exposed row or column by
		''' returning the distance required to completely expose the item.
		''' <p>
		''' Scrolling containers, like JScrollPane, will use this method
		''' each time the user requests a unit scroll.
		''' </summary>
		''' <param name="visibleRect"> The view area visible within the viewport </param>
		''' <param name="orientation"> Either SwingConstants.VERTICAL or SwingConstants.HORIZONTAL. </param>
		''' <param name="direction"> Less than zero to scroll up/left, greater than zero for down/right. </param>
		''' <returns> The "unit" increment for scrolling in the specified direction.
		'''         This value should always be positive. </returns>
		''' <seealso cref= JScrollBar#setUnitIncrement </seealso>
		Function getScrollableUnitIncrement(ByVal visibleRect As java.awt.Rectangle, ByVal orientation As Integer, ByVal direction As Integer) As Integer


		''' <summary>
		''' Components that display logical rows or columns should compute
		''' the scroll increment that will completely expose one block
		''' of rows or columns, depending on the value of orientation.
		''' <p>
		''' Scrolling containers, like JScrollPane, will use this method
		''' each time the user requests a block scroll.
		''' </summary>
		''' <param name="visibleRect"> The view area visible within the viewport </param>
		''' <param name="orientation"> Either SwingConstants.VERTICAL or SwingConstants.HORIZONTAL. </param>
		''' <param name="direction"> Less than zero to scroll up/left, greater than zero for down/right. </param>
		''' <returns> The "block" increment for scrolling in the specified direction.
		'''         This value should always be positive. </returns>
		''' <seealso cref= JScrollBar#setBlockIncrement </seealso>
		Function getScrollableBlockIncrement(ByVal visibleRect As java.awt.Rectangle, ByVal orientation As Integer, ByVal direction As Integer) As Integer


		''' <summary>
		''' Return true if a viewport should always force the width of this
		''' <code>Scrollable</code> to match the width of the viewport.
		''' For example a normal
		''' text view that supported line wrapping would return true here, since it
		''' would be undesirable for wrapped lines to disappear beyond the right
		''' edge of the viewport.  Note that returning true for a Scrollable
		''' whose ancestor is a JScrollPane effectively disables horizontal
		''' scrolling.
		''' <p>
		''' Scrolling containers, like JViewport, will use this method each
		''' time they are validated.
		''' </summary>
		''' <returns> True if a viewport should force the Scrollables width to match its own. </returns>
		ReadOnly Property scrollableTracksViewportWidth As Boolean

		''' <summary>
		''' Return true if a viewport should always force the height of this
		''' Scrollable to match the height of the viewport.  For example a
		''' columnar text view that flowed text in left to right columns
		''' could effectively disable vertical scrolling by returning
		''' true here.
		''' <p>
		''' Scrolling containers, like JViewport, will use this method each
		''' time they are validated.
		''' </summary>
		''' <returns> True if a viewport should force the Scrollables height to match its own. </returns>
		ReadOnly Property scrollableTracksViewportHeight As Boolean
	End Interface

End Namespace