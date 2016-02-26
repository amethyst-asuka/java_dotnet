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



	''' <summary>
	''' A subclass of java.awt.Font that implements UIResource.
	''' UI classes which set default font properties should use
	''' this class.
	''' <p>
	''' <strong>Warning:</strong>
	''' Serialized objects of this class will not be compatible with
	''' future Swing releases. The current serialization support is
	''' appropriate for short term storage or RMI between applications running
	''' the same version of Swing.  As of 1.4, support for long term storage
	''' of all JavaBeans&trade;
	''' has been added to the <code>java.beans</code> package.
	''' Please see <seealso cref="java.beans.XMLEncoder"/>.
	''' </summary>
	''' <seealso cref= javax.swing.plaf.UIResource
	''' @author Hans Muller
	'''  </seealso>
	Public Class FontUIResource
		Inherits java.awt.Font
		Implements javax.swing.plaf.UIResource

		Public Sub New(ByVal name As String, ByVal style As Integer, ByVal size As Integer)
			MyBase.New(name, style, size)
		End Sub

		Public Sub New(ByVal font As java.awt.Font)
			MyBase.New(font)
		End Sub
	End Class

End Namespace