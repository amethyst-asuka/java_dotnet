Imports javax.swing
Imports javax.swing.plaf
Imports javax.swing.plaf.basic

'
' * Copyright (c) 1998, 2013, Oracle and/or its affiliates. All rights reserved.
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

Namespace javax.swing.plaf.metal

	''' <summary>
	''' Metal split pane.
	''' <p>
	''' <strong>Warning:</strong>
	''' Serialized objects of this class will not be compatible with
	''' future Swing releases. The current serialization support is
	''' appropriate for short term storage or RMI between applications running
	''' the same version of Swing.  As of 1.4, support for long term storage
	''' of all JavaBeans&trade;
	''' has been added to the <code>java.beans</code> package.
	''' Please see <seealso cref="java.beans.XMLEncoder"/>.
	''' 
	''' @author Steve Wilson
	''' </summary>
	Public Class MetalSplitPaneUI
		Inherits BasicSplitPaneUI


		''' <summary>
		''' Creates a new MetalSplitPaneUI instance
		''' </summary>
		Public Shared Function createUI(ByVal x As JComponent) As ComponentUI
			Return New MetalSplitPaneUI
		End Function

		''' <summary>
		''' Creates the default divider.
		''' </summary>
		Public Overrides Function createDefaultDivider() As BasicSplitPaneDivider
			Return New MetalSplitPaneDivider(Me)
		End Function
	End Class

End Namespace