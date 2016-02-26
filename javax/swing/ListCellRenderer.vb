'
' * Copyright (c) 1997, 2005, Oracle and/or its affiliates. All rights reserved.
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
	''' Identifies components that can be used as "rubber stamps" to paint
	''' the cells in a JList.  For example, to use a JLabel as a
	''' ListCellRenderer, you would write something like this:
	''' <pre>
	''' {@code
	''' class MyCellRenderer extends JLabel implements ListCellRenderer<Object> {
	'''     public MyCellRenderer() {
	'''         setOpaque(true);
	'''     }
	''' 
	'''     public Component getListCellRendererComponent(JList<?> list,
	'''                                                   Object value,
	'''                                                   int index,
	'''                                                   boolean isSelected,
	'''                                                   boolean cellHasFocus) {
	''' 
	'''         setText(value.toString());
	''' 
	'''         Color background;
	'''         Color foreground;
	''' 
	'''         // check if this cell represents the current DnD drop location
	'''         JList.DropLocation dropLocation = list.getDropLocation();
	'''         if (dropLocation != null
	'''                 && !dropLocation.isInsert()
	'''                 && dropLocation.getIndex() == index) {
	''' 
	'''             background = Color.BLUE;
	'''             foreground = Color.WHITE;
	''' 
	'''         // check if this cell is selected
	'''         } else if (isSelected) {
	'''             background = Color.RED;
	'''             foreground = Color.WHITE;
	''' 
	'''         // unselected, and not the DnD drop location
	'''         } else {
	'''             background = Color.WHITE;
	'''             foreground = Color.BLACK;
	'''         };
	''' 
	'''         setBackground(background);
	'''         setForeground(foreground);
	''' 
	'''         return this;
	'''     }
	''' }
	''' }
	''' </pre>
	''' </summary>
	''' @param <E> the type of values this renderer can be used for
	''' </param>
	''' <seealso cref= JList </seealso>
	''' <seealso cref= DefaultListCellRenderer
	''' 
	''' @author Hans Muller </seealso>
	Public Interface ListCellRenderer(Of E)
		''' <summary>
		''' Return a component that has been configured to display the specified
		''' value. That component's <code>paint</code> method is then called to
		''' "render" the cell.  If it is necessary to compute the dimensions
		''' of a list because the list cells do not have a fixed size, this method
		''' is called to generate a component on which <code>getPreferredSize</code>
		''' can be invoked.
		''' </summary>
		''' <param name="list"> The JList we're painting. </param>
		''' <param name="value"> The value returned by list.getModel().getElementAt(index). </param>
		''' <param name="index"> The cells index. </param>
		''' <param name="isSelected"> True if the specified cell was selected. </param>
		''' <param name="cellHasFocus"> True if the specified cell has the focus. </param>
		''' <returns> A component whose paint() method will render the specified value.
		''' </returns>
		''' <seealso cref= JList </seealso>
		''' <seealso cref= ListSelectionModel </seealso>
		''' <seealso cref= ListModel </seealso>
		Function getListCellRendererComponent(Of T1 As E)(ByVal list As JList(Of T1), ByVal value As E, ByVal index As Integer, ByVal isSelected As Boolean, ByVal cellHasFocus As Boolean) As java.awt.Component
	End Interface

End Namespace