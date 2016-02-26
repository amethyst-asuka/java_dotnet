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
	''' Pluggable look and feel interface for JSplitPane.
	''' 
	''' @author Scott Violet
	''' </summary>
	Public MustInherit Class SplitPaneUI
		Inherits ComponentUI

		''' <summary>
		''' Messaged to relayout the JSplitPane based on the preferred size
		''' of the children components.
		''' </summary>
		Public MustOverride Sub resetToPreferredSizes(ByVal jc As javax.swing.JSplitPane)

		''' <summary>
		''' Sets the location of the divider to location.
		''' </summary>
		Public MustOverride Sub setDividerLocation(ByVal jc As javax.swing.JSplitPane, ByVal location As Integer)

		''' <summary>
		''' Returns the location of the divider.
		''' </summary>
		Public MustOverride Function getDividerLocation(ByVal jc As javax.swing.JSplitPane) As Integer

		''' <summary>
		''' Returns the minimum possible location of the divider.
		''' </summary>
		Public MustOverride Function getMinimumDividerLocation(ByVal jc As javax.swing.JSplitPane) As Integer

		''' <summary>
		''' Returns the maximum possible location of the divider.
		''' </summary>
		Public MustOverride Function getMaximumDividerLocation(ByVal jc As javax.swing.JSplitPane) As Integer

		''' <summary>
		''' Messaged after the JSplitPane the receiver is providing the look
		''' and feel for paints its children.
		''' </summary>
		Public MustOverride Sub finishedPaintingChildren(ByVal jc As javax.swing.JSplitPane, ByVal g As java.awt.Graphics)
	End Class

End Namespace