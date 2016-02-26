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
Namespace javax.swing.border


	''' <summary>
	''' A class which provides an empty, transparent border which
	''' takes up space but does no drawing.
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
	''' @author David Kloba
	''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
	<Serializable> _
	Public Class EmptyBorder
		Inherits AbstractBorder

		Protected Friend left, right, top, bottom As Integer

		''' <summary>
		''' Creates an empty border with the specified insets. </summary>
		''' <param name="top"> the top inset of the border </param>
		''' <param name="left"> the left inset of the border </param>
		''' <param name="bottom"> the bottom inset of the border </param>
		''' <param name="right"> the right inset of the border </param>
		Public Sub New(ByVal top As Integer, ByVal left As Integer, ByVal bottom As Integer, ByVal right As Integer)
			Me.top = top
			Me.right = right
			Me.bottom = bottom
			Me.left = left
		End Sub

		''' <summary>
		''' Creates an empty border with the specified insets. </summary>
		''' <param name="borderInsets"> the insets of the border </param>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
		Public Sub New(ByVal borderInsets As java.awt.Insets)
			Me.top = borderInsets.top
			Me.right = borderInsets.right
			Me.bottom = borderInsets.bottom
			Me.left = borderInsets.left
		End Sub

		''' <summary>
		''' Does no drawing by default.
		''' </summary>
		Public Overrides Sub paintBorder(ByVal c As java.awt.Component, ByVal g As java.awt.Graphics, ByVal x As Integer, ByVal y As Integer, ByVal width As Integer, ByVal height As Integer)
		End Sub

		''' <summary>
		''' Reinitialize the insets parameter with this Border's current Insets. </summary>
		''' <param name="c"> the component for which this border insets value applies </param>
		''' <param name="insets"> the object to be reinitialized </param>
		Public Overrides Function getBorderInsets(ByVal c As java.awt.Component, ByVal insets As java.awt.Insets) As java.awt.Insets
			insets.left = left
			insets.top = top
			insets.right = right
			insets.bottom = bottom
			Return insets
		End Function

		''' <summary>
		''' Returns the insets of the border.
		''' @since 1.3
		''' </summary>
		Public Overridable Property borderInsets As java.awt.Insets
			Get
				Return New java.awt.Insets(top, left, bottom, right)
			End Get
		End Property

		''' <summary>
		''' Returns whether or not the border is opaque.
		''' Returns false by default.
		''' </summary>
		Public Property Overrides borderOpaque As Boolean
			Get
				Return False
			End Get
		End Property

	End Class

End Namespace