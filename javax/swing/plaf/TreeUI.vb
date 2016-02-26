'
' * Copyright (c) 1997, 1998, Oracle and/or its affiliates. All rights reserved.
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

Namespace javax.swing.plaf


	''' <summary>
	''' Pluggable look and feel interface for JTree.
	''' 
	''' @author Rob Davis
	''' @author Scott Violet
	''' </summary>
	Public MustInherit Class TreeUI
		Inherits ComponentUI

		''' <summary>
		''' Returns the Rectangle enclosing the label portion that the
		''' last item in path will be drawn into.  Will return null if
		''' any component in path is currently valid.
		''' </summary>
		Public MustOverride Function getPathBounds(ByVal tree As javax.swing.JTree, ByVal path As javax.swing.tree.TreePath) As java.awt.Rectangle

		''' <summary>
		''' Returns the path for passed in row.  If row is not visible
		''' null is returned.
		''' </summary>
		Public MustOverride Function getPathForRow(ByVal tree As javax.swing.JTree, ByVal row As Integer) As javax.swing.tree.TreePath

		''' <summary>
		''' Returns the row that the last item identified in path is visible
		''' at.  Will return -1 if any of the elements in path are not
		''' currently visible.
		''' </summary>
		Public MustOverride Function getRowForPath(ByVal tree As javax.swing.JTree, ByVal path As javax.swing.tree.TreePath) As Integer

		''' <summary>
		''' Returns the number of rows that are being displayed.
		''' </summary>
		Public MustOverride Function getRowCount(ByVal tree As javax.swing.JTree) As Integer

		''' <summary>
		''' Returns the path to the node that is closest to x,y.  If
		''' there is nothing currently visible this will return null, otherwise
		''' it'll always return a valid path.  If you need to test if the
		''' returned object is exactly at x, y you should get the bounds for
		''' the returned path and test x, y against that.
		''' </summary>
		Public MustOverride Function getClosestPathForLocation(ByVal tree As javax.swing.JTree, ByVal x As Integer, ByVal y As Integer) As javax.swing.tree.TreePath

		''' <summary>
		''' Returns true if the tree is being edited.  The item that is being
		''' edited can be returned by getEditingPath().
		''' </summary>
		Public MustOverride Function isEditing(ByVal tree As javax.swing.JTree) As Boolean

		''' <summary>
		''' Stops the current editing session.  This has no effect if the
		''' tree isn't being edited.  Returns true if the editor allows the
		''' editing session to stop.
		''' </summary>
		Public MustOverride Function stopEditing(ByVal tree As javax.swing.JTree) As Boolean

		''' <summary>
		''' Cancels the current editing session. This has no effect if the
		''' tree isn't being edited.  Returns true if the editor allows the
		''' editing session to stop.
		''' </summary>
		Public MustOverride Sub cancelEditing(ByVal tree As javax.swing.JTree)

		''' <summary>
		''' Selects the last item in path and tries to edit it.  Editing will
		''' fail if the CellEditor won't allow it for the selected item.
		''' </summary>
		Public MustOverride Sub startEditingAtPath(ByVal tree As javax.swing.JTree, ByVal path As javax.swing.tree.TreePath)

		''' <summary>
		''' Returns the path to the element that is being edited.
		''' </summary>
		Public MustOverride Function getEditingPath(ByVal tree As javax.swing.JTree) As javax.swing.tree.TreePath
	End Class

End Namespace