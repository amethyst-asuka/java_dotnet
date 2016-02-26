Imports javax.swing
Imports javax.swing.text
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
	''' Basis of a look and feel for a JTextField.
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
	''' @author  Steve Wilson
	''' </summary>
	Public Class MetalTextFieldUI
		Inherits BasicTextFieldUI

		Public Shared Function createUI(ByVal c As JComponent) As ComponentUI
			Return New MetalTextFieldUI
		End Function

		''' <summary>
		''' This method gets called when a bound property is changed
		''' on the associated JTextComponent.  This is a hook
		''' which UI implementations may change to reflect how the
		''' UI displays bound properties of JTextComponent subclasses.
		''' </summary>
		''' <param name="evt"> the property change event </param>
		Public Overrides Sub propertyChange(ByVal evt As PropertyChangeEvent)
			MyBase.propertyChange(evt)
		End Sub

	End Class

End Namespace