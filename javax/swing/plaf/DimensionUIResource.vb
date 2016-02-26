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
	' * A subclass of <code>Dimension</code> that implements
	' * <code>UIResource</code>.  UI classes that use
	' * <code>Dimension</code> values for default properties
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
	Public Class DimensionUIResource
		Inherits java.awt.Dimension
		Implements javax.swing.plaf.UIResource

		Public Sub New(ByVal width As Integer, ByVal height As Integer)
			MyBase.New(width, height)
		End Sub
	End Class

End Namespace