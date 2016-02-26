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

Namespace javax.swing.plaf


	''' <summary>
	''' An Icon wrapper class which implements UIResource.  UI
	''' classes which set icon properties should use this class
	''' to wrap any icons specified as defaults.
	''' 
	''' This class delegates all method invocations to the
	''' Icon "delegate" object specified at construction.
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
	''' @author Amy Fowler
	'''  </seealso>
	<Serializable> _
	Public Class IconUIResource
		Implements javax.swing.Icon, javax.swing.plaf.UIResource

		Private [delegate] As javax.swing.Icon

		''' <summary>
		''' Creates a UIResource icon object which wraps
		''' an existing Icon instance. </summary>
		''' <param name="delegate"> the icon being wrapped </param>
		Public Sub New(ByVal [delegate] As javax.swing.Icon)
			If [delegate] Is Nothing Then Throw New System.ArgumentException("null delegate icon argument")
			Me.delegate = [delegate]
		End Sub

		Public Overridable Sub paintIcon(ByVal c As java.awt.Component, ByVal g As java.awt.Graphics, ByVal x As Integer, ByVal y As Integer)
			[delegate].paintIcon(c, g, x, y)
		End Sub

		Public Overridable Property iconWidth As Integer
			Get
				Return [delegate].iconWidth
			End Get
		End Property

		Public Overridable Property iconHeight As Integer
			Get
				Return [delegate].iconHeight
			End Get
		End Property

	End Class

End Namespace