'
' * Copyright (c) 1997, 2006, Oracle and/or its affiliates. All rights reserved.
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

Namespace javax.swing.tree


	''' <summary>
	''' Adds to CellEditor the extensions necessary to configure an editor
	''' in a tree.
	''' </summary>
	''' <seealso cref= javax.swing.JTree
	'''  
	''' @author Scott Violet </seealso>

	Public Interface TreeCellEditor
		Inherits javax.swing.CellEditor

		''' <summary>
		''' Sets an initial <I>value</I> for the editor.  This will cause
		''' the editor to stopEditing and lose any partially edited value
		''' if the editor is editing when this method is called. <p>
		''' 
		''' Returns the component that should be added to the client's
		''' Component hierarchy.  Once installed in the client's hierarchy
		''' this component will then be able to draw and receive user input.
		''' </summary>
		''' <param name="tree">            the JTree that is asking the editor to edit;
		'''                          this parameter can be null </param>
		''' <param name="value">           the value of the cell to be edited </param>
		''' <param name="isSelected">      true if the cell is to be rendered with
		'''                          selection highlighting </param>
		''' <param name="expanded">        true if the node is expanded </param>
		''' <param name="leaf">            true if the node is a leaf node </param>
		''' <param name="row">             the row index of the node being edited </param>
		''' <returns>  the component for editing </returns>
		Function getTreeCellEditorComponent(ByVal tree As javax.swing.JTree, ByVal value As Object, ByVal isSelected As Boolean, ByVal expanded As Boolean, ByVal leaf As Boolean, ByVal row As Integer) As java.awt.Component
	End Interface

End Namespace