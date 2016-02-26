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

Namespace javax.swing.plaf



	'
	' * A subclass of Insets that implements UIResource.  UI
	' * classes that use Insets values for default properties
	' * should use this class.
	' * <p>
	' * <strong>Warning:</strong>
	' * Serialized objects of this class will not be compatible with
	' * future Swing releases. The current serialization support is
	' * appropriate for short term storage or RMI between applications running
	' * the same version of Swing.  As of 1.4, support for long term storage
	' * of all JavaBeans&trade;
	' * has been added to the <code>java.beans</code> package.
	' * Please see {@link java.beans.XMLEncoder}.
	' *
	' * @see javax.swing.plaf.UIResource
	' * @author Amy Fowler
	' *
	' 
	Public Class InsetsUIResource
		Inherits java.awt.Insets
		Implements javax.swing.plaf.UIResource

		Public Sub New(ByVal top As Integer, ByVal left As Integer, ByVal bottom As Integer, ByVal right As Integer)
			MyBase.New(top, left, bottom, right)
		End Sub
	End Class

End Namespace