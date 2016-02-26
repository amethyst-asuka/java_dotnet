Imports System
Imports javax.swing.border

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
	' * A Border wrapper class which implements UIResource.  UI
	' * classes which set border properties should use this class
	' * to wrap any borders specified as defaults.
	' *
	' * This class delegates all method invocations to the
	' * Border "delegate" object specified at construction.
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
	<Serializable> _
	Public Class BorderUIResource
		Implements Border, javax.swing.plaf.UIResource

		Friend Shared etched As Border
		Friend Shared loweredBevel As Border
		Friend Shared raisedBevel As Border
		Friend Shared blackLine As Border

		Public Property Shared etchedBorderUIResource As Border
			Get
				If etched Is Nothing Then etched = New EtchedBorderUIResource
				Return etched
			End Get
		End Property

		Public Property Shared loweredBevelBorderUIResource As Border
			Get
				If loweredBevel Is Nothing Then loweredBevel = New BevelBorderUIResource(BevelBorder.LOWERED)
				Return loweredBevel
			End Get
		End Property

		Public Property Shared raisedBevelBorderUIResource As Border
			Get
				If raisedBevel Is Nothing Then raisedBevel = New BevelBorderUIResource(BevelBorder.RAISED)
				Return raisedBevel
			End Get
		End Property

		Public Property Shared blackLineBorderUIResource As Border
			Get
				If blackLine Is Nothing Then blackLine = New LineBorderUIResource(java.awt.Color.black)
				Return blackLine
			End Get
		End Property

		Private [delegate] As Border

		''' <summary>
		''' Creates a UIResource border object which wraps
		''' an existing Border instance. </summary>
		''' <param name="delegate"> the border being wrapped </param>
		Public Sub New(ByVal [delegate] As Border)
			If [delegate] Is Nothing Then Throw New System.ArgumentException("null border delegate argument")
			Me.delegate = [delegate]
		End Sub

		Public Overridable Sub paintBorder(ByVal c As java.awt.Component, ByVal g As java.awt.Graphics, ByVal x As Integer, ByVal y As Integer, ByVal width As Integer, ByVal height As Integer) Implements Border.paintBorder
			[delegate].paintBorder(c, g, x, y, width, height)
		End Sub

		Public Overridable Function getBorderInsets(ByVal c As java.awt.Component) As java.awt.Insets Implements Border.getBorderInsets
			Return [delegate].getBorderInsets(c)
		End Function

		Public Overridable Property borderOpaque As Boolean Implements Border.isBorderOpaque
			Get
				Return [delegate].borderOpaque
			End Get
		End Property

		Public Class CompoundBorderUIResource
			Inherits CompoundBorder
			Implements javax.swing.plaf.UIResource

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
			Public Sub New(ByVal outsideBorder As Border, ByVal insideBorder As Border)
				MyBase.New(outsideBorder, insideBorder)
			End Sub

		End Class

		Public Class EmptyBorderUIResource
			Inherits EmptyBorder
			Implements javax.swing.plaf.UIResource

			Public Sub New(ByVal top As Integer, ByVal left As Integer, ByVal bottom As Integer, ByVal right As Integer)
				MyBase.New(top, left, bottom, right)
			End Sub
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
			Public Sub New(ByVal insets As java.awt.Insets)
				MyBase.New(insets)
			End Sub
		End Class

		Public Class LineBorderUIResource
			Inherits LineBorder
			Implements javax.swing.plaf.UIResource

			Public Sub New(ByVal color As java.awt.Color)
				MyBase.New(color)
			End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
			Public Sub New(ByVal color As java.awt.Color, ByVal thickness As Integer)
				MyBase.New(color, thickness)
			End Sub
		End Class


		Public Class BevelBorderUIResource
			Inherits BevelBorder
			Implements javax.swing.plaf.UIResource

			Public Sub New(ByVal bevelType As Integer)
				MyBase.New(bevelType)
			End Sub

			Public Sub New(ByVal bevelType As Integer, ByVal highlight As java.awt.Color, ByVal shadow As java.awt.Color)
				MyBase.New(bevelType, highlight, shadow)
			End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
			Public Sub New(ByVal bevelType As Integer, ByVal highlightOuter As java.awt.Color, ByVal highlightInner As java.awt.Color, ByVal shadowOuter As java.awt.Color, ByVal shadowInner As java.awt.Color)
				MyBase.New(bevelType, highlightOuter, highlightInner, shadowOuter, shadowInner)
			End Sub
		End Class

		Public Class EtchedBorderUIResource
			Inherits EtchedBorder
			Implements javax.swing.plaf.UIResource

			Public Sub New()
				MyBase.New()
			End Sub

			Public Sub New(ByVal etchType As Integer)
				MyBase.New(etchType)
			End Sub

			Public Sub New(ByVal highlight As java.awt.Color, ByVal shadow As java.awt.Color)
				MyBase.New(highlight, shadow)
			End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
			Public Sub New(ByVal etchType As Integer, ByVal highlight As java.awt.Color, ByVal shadow As java.awt.Color)
				MyBase.New(etchType, highlight, shadow)
			End Sub
		End Class

		Public Class MatteBorderUIResource
			Inherits MatteBorder
			Implements javax.swing.plaf.UIResource

			Public Sub New(ByVal top As Integer, ByVal left As Integer, ByVal bottom As Integer, ByVal right As Integer, ByVal color As java.awt.Color)
				MyBase.New(top, left, bottom, right, color)
			End Sub

			Public Sub New(ByVal top As Integer, ByVal left As Integer, ByVal bottom As Integer, ByVal right As Integer, ByVal tileIcon As javax.swing.Icon)
				MyBase.New(top, left, bottom, right, tileIcon)
			End Sub

			Public Sub New(ByVal tileIcon As javax.swing.Icon)
				MyBase.New(tileIcon)
			End Sub
		End Class

		Public Class TitledBorderUIResource
			Inherits TitledBorder
			Implements javax.swing.plaf.UIResource

			Public Sub New(ByVal title As String)
				MyBase.New(title)
			End Sub

			Public Sub New(ByVal border As Border)
				MyBase.New(border)
			End Sub

			Public Sub New(ByVal border As Border, ByVal title As String)
				MyBase.New(border, title)
			End Sub

			Public Sub New(ByVal border As Border, ByVal title As String, ByVal titleJustification As Integer, ByVal titlePosition As Integer)
				MyBase.New(border, title, titleJustification, titlePosition)
			End Sub

			Public Sub New(ByVal border As Border, ByVal title As String, ByVal titleJustification As Integer, ByVal titlePosition As Integer, ByVal titleFont As java.awt.Font)
				MyBase.New(border, title, titleJustification, titlePosition, titleFont)
			End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
			Public Sub New(ByVal border As Border, ByVal title As String, ByVal titleJustification As Integer, ByVal titlePosition As Integer, ByVal titleFont As java.awt.Font, ByVal titleColor As java.awt.Color)
				MyBase.New(border, title, titleJustification, titlePosition, titleFont, titleColor)
			End Sub
		End Class

	End Class

End Namespace