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
	''' Pluggable look and feel interface for JTabbedPane.
	''' 
	''' @author Dave Moore
	''' @author Amy Fowler
	''' </summary>
	Public MustInherit Class TabbedPaneUI
		Inherits ComponentUI

		Public MustOverride Function tabForCoordinate(ByVal pane As javax.swing.JTabbedPane, ByVal x As Integer, ByVal y As Integer) As Integer
		Public MustOverride Function getTabBounds(ByVal pane As javax.swing.JTabbedPane, ByVal index As Integer) As java.awt.Rectangle
		Public MustOverride Function getTabRunCount(ByVal pane As javax.swing.JTabbedPane) As Integer
	End Class

End Namespace