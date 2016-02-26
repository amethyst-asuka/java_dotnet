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
	' * A subclass of Color that implements UIResource.  UI
	' * classes that create colors should use this class.
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
	' * @author Hans Muller
	' *
	' 
	Public Class ColorUIResource
		Inherits java.awt.Color
		Implements UIResource

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
		Public Sub New(ByVal r As Integer, ByVal g As Integer, ByVal b As Integer)
			MyBase.New(r, g, b)
		End Sub

		Public Sub New(ByVal rgb As Integer)
			MyBase.New(rgb)
		End Sub

		Public Sub New(ByVal r As Single, ByVal g As Single, ByVal b As Single)
			MyBase.New(r, g, b)
		End Sub

		Public Sub New(ByVal c As java.awt.Color)
			MyBase.New(c.rGB, (c.rGB And &HFF000000L) <> &HFF000000L)
		End Sub
	End Class

End Namespace