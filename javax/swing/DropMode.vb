'
' * Copyright (c) 2005, Oracle and/or its affiliates. All rights reserved.
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
	''' Drop modes, used to determine the method by which a component
	''' tracks and indicates a drop location during drag and drop.
	''' 
	''' @author Shannon Hickey </summary>
	''' <seealso cref= JTable#setDropMode </seealso>
	''' <seealso cref= JList#setDropMode </seealso>
	''' <seealso cref= JTree#setDropMode </seealso>
	''' <seealso cref= javax.swing.text.JTextComponent#setDropMode
	''' @since 1.6 </seealso>
	Public Enum DropMode

		''' <summary>
		''' A component's own internal selection mechanism (or caret for text
		''' components) should be used to track the drop location.
		''' </summary>
		USE_SELECTION

		''' <summary>
		''' The drop location should be tracked in terms of the index of
		''' existing items. Useful for dropping on items in tables, lists,
		''' and trees.
		''' </summary>
		[ON]

		''' <summary>
		''' The drop location should be tracked in terms of the position
		''' where new data should be inserted. For components that manage
		''' a list of items (list and tree for example), the drop location
		''' should indicate the index where new data should be inserted.
		''' For text components the location should represent a position
		''' between characters. For components that manage tabular data
		''' (table for example), the drop location should indicate
		''' where to insert new rows, columns, or both, to accommodate
		''' the dropped data.
		''' </summary>
		INSERT

		''' <summary>
		''' The drop location should be tracked in terms of the row index
		''' where new rows should be inserted to accommodate the dropped
		''' data. This is useful for components that manage tabular data.
		''' </summary>
		INSERT_ROWS

		''' <summary>
		''' The drop location should be tracked in terms of the column index
		''' where new columns should be inserted to accommodate the dropped
		''' data. This is useful for components that manage tabular data.
		''' </summary>
		INSERT_COLS

		''' <summary>
		''' This mode is a combination of <code>ON</code>
		''' and <code>INSERT</code>, specifying that data can be
		''' dropped on existing items, or in insert locations
		''' as specified by <code>INSERT</code>.
		''' </summary>
		ON_OR_INSERT

		''' <summary>
		''' This mode is a combination of <code>ON</code>
		''' and <code>INSERT_ROWS</code>, specifying that data can be
		''' dropped on existing items, or as insert rows
		''' as specified by <code>INSERT_ROWS</code>.
		''' </summary>
		ON_OR_INSERT_ROWS

		''' <summary>
		''' This mode is a combination of <code>ON</code>
		''' and <code>INSERT_COLS</code>, specifying that data can be
		''' dropped on existing items, or as insert columns
		''' as specified by <code>INSERT_COLS</code>.
		''' </summary>
		ON_OR_INSERT_COLS
	End Enum

End Namespace