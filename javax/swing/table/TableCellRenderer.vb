Imports javax.swing

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

Namespace javax.swing.table

	''' <summary>
	''' This interface defines the method required by any object that
	''' would like to be a renderer for cells in a <code>JTable</code>.
	''' 
	''' @author Alan Chung
	''' </summary>

	Public Interface TableCellRenderer

		''' <summary>
		''' Returns the component used for drawing the cell.  This method is
		''' used to configure the renderer appropriately before drawing.
		''' <p>
		''' The <code>TableCellRenderer</code> is also responsible for rendering the
		''' the cell representing the table's current DnD drop location if
		''' it has one. If this renderer cares about rendering
		''' the DnD drop location, it should query the table directly to
		''' see if the given row and column represent the drop location:
		''' <pre>
		'''     JTable.DropLocation dropLocation = table.getDropLocation();
		'''     if (dropLocation != null
		'''             &amp;&amp; !dropLocation.isInsertRow()
		'''             &amp;&amp; !dropLocation.isInsertColumn()
		'''             &amp;&amp; dropLocation.getRow() == row
		'''             &amp;&amp; dropLocation.getColumn() == column) {
		''' 
		'''         // this cell represents the current drop location
		'''         // so render it specially, perhaps with a different color
		'''     }
		''' </pre>
		''' <p>
		''' During a printing operation, this method will be called with
		''' <code>isSelected</code> and <code>hasFocus</code> values of
		''' <code>false</code> to prevent selection and focus from appearing
		''' in the printed output. To do other customization based on whether
		''' or not the table is being printed, check the return value from
		''' <seealso cref="javax.swing.JComponent#isPaintingForPrint()"/>.
		''' </summary>
		''' <param name="table">           the <code>JTable</code> that is asking the
		'''                          renderer to draw; can be <code>null</code> </param>
		''' <param name="value">           the value of the cell to be rendered.  It is
		'''                          up to the specific renderer to interpret
		'''                          and draw the value.  For example, if
		'''                          <code>value</code>
		'''                          is the string "true", it could be rendered as a
		'''                          string or it could be rendered as a check
		'''                          box that is checked.  <code>null</code> is a
		'''                          valid value </param>
		''' <param name="isSelected">      true if the cell is to be rendered with the
		'''                          selection highlighted; otherwise false </param>
		''' <param name="hasFocus">        if true, render cell appropriately.  For
		'''                          example, put a special border on the cell, if
		'''                          the cell can be edited, render in the color used
		'''                          to indicate editing </param>
		''' <param name="row">             the row index of the cell being drawn.  When
		'''                          drawing the header, the value of
		'''                          <code>row</code> is -1 </param>
		''' <param name="column">          the column index of the cell being drawn </param>
		''' <seealso cref= javax.swing.JComponent#isPaintingForPrint() </seealso>
		Function getTableCellRendererComponent(ByVal table As JTable, ByVal value As Object, ByVal isSelected As Boolean, ByVal hasFocus As Boolean, ByVal row As Integer, ByVal column As Integer) As java.awt.Component
	End Interface

End Namespace