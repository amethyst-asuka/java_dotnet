Imports System

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
Namespace javax.swing

	''' <summary>
	''' An exception that indicates the requested look &amp; feel
	''' management classes are not present on the user's system.
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
	''' @author unattributed
	''' </summary>
	Public Class UnsupportedLookAndFeelException
		Inherits Exception

		''' <summary>
		''' Constructs an UnsupportedLookAndFeelException object. </summary>
		''' <param name="s"> a message String </param>
		Public Sub New(ByVal s As String)
			MyBase.New(s)
		End Sub
	End Class

End Namespace