Imports javax.swing
Imports javax.swing.plaf

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
	''' A Metal L&amp;F extension of BasicToolTipUI.
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
	Public Class MetalToolTipUI
		Inherits javax.swing.plaf.basic.BasicToolTipUI

		Friend Shared Shadows sharedInstance As New MetalToolTipUI
		Private smallFont As Font
		' Refer to note in getAcceleratorString about this field.
		Private tip As JToolTip
		Public Const padSpaceBetweenStrings As Integer = 12
		Private acceleratorDelimiter As String

		Public Sub New()
			MyBase.New()
		End Sub

		Public Shared Function createUI(ByVal c As JComponent) As ComponentUI
			Return sharedInstance
		End Function

		Public Overrides Sub installUI(ByVal c As JComponent)
			MyBase.installUI(c)
			tip = CType(c, JToolTip)
			Dim f As Font = c.font
			smallFont = New Font(f.name, f.style, f.size - 2)
			acceleratorDelimiter = UIManager.getString("MenuItem.acceleratorDelimiter")
			If acceleratorDelimiter Is Nothing Then acceleratorDelimiter = "-"
		End Sub

		Public Overrides Sub uninstallUI(ByVal c As JComponent)
			MyBase.uninstallUI(c)
			tip = Nothing
		End Sub

		Public Overrides Sub paint(ByVal g As Graphics, ByVal c As JComponent)
			Dim tip As JToolTip = CType(c, JToolTip)
			Dim font As Font = c.font
			Dim metrics As FontMetrics = sun.swing.SwingUtilities2.getFontMetrics(c, g, font)
			Dim size As Dimension = c.size
			Dim accelBL As Integer

			g.color = c.foreground
			' fix for bug 4153892
			Dim tipText As String = tip.tipText
			If tipText Is Nothing Then tipText = ""

			Dim accelString As String = getAcceleratorString(tip)
			Dim accelMetrics As FontMetrics = sun.swing.SwingUtilities2.getFontMetrics(c, g, smallFont)
			Dim accelSpacing As Integer = calcAccelSpacing(c, accelMetrics, accelString)

			Dim insets As Insets = tip.insets
			Dim paintTextR As New Rectangle(insets.left + 3, insets.top, size.width - (insets.left + insets.right) - 6 - accelSpacing, size.height - (insets.top + insets.bottom))
			Dim v As javax.swing.text.View = CType(c.getClientProperty(javax.swing.plaf.basic.BasicHTML.propertyKey), javax.swing.text.View)
			If v IsNot Nothing Then
				v.paint(g, paintTextR)
				accelBL = javax.swing.plaf.basic.BasicHTML.getHTMLBaseline(v, paintTextR.width, paintTextR.height)
			Else
				g.font = font
				sun.swing.SwingUtilities2.drawString(tip, g, tipText, paintTextR.x, paintTextR.y + metrics.ascent)
				accelBL = metrics.ascent
			End If

			If Not accelString.Equals("") Then
				g.font = smallFont
				g.color = MetalLookAndFeel.primaryControlDarkShadow
				sun.swing.SwingUtilities2.drawString(tip, g, accelString, tip.width - 1 - insets.right - accelSpacing + padSpaceBetweenStrings - 3, paintTextR.y + accelBL)
			End If
		End Sub

		Private Function calcAccelSpacing(ByVal c As JComponent, ByVal fm As FontMetrics, ByVal accel As String) As Integer
			Return If(accel.Equals(""), 0, padSpaceBetweenStrings + sun.swing.SwingUtilities2.stringWidth(c, fm, accel))
		End Function

		Public Overrides Function getPreferredSize(ByVal c As JComponent) As Dimension
			Dim d As Dimension = MyBase.getPreferredSize(c)

			Dim key As String = getAcceleratorString(CType(c, JToolTip))
			If Not(key.Equals("")) Then d.width += calcAccelSpacing(c, c.getFontMetrics(smallFont), key)
			Return d
		End Function

		Protected Friend Overridable Property acceleratorHidden As Boolean
			Get
				Dim b As Boolean? = CBool(UIManager.get("ToolTip.hideAccelerator"))
				Return b IsNot Nothing AndAlso b
			End Get
		End Property

		Private Function getAcceleratorString(ByVal tip As JToolTip) As String
			Me.tip = tip

			Dim retValue As String = acceleratorString

			Me.tip = Nothing
			Return retValue
		End Function

		' NOTE: This requires the tip field to be set before this is invoked.
		' As MetalToolTipUI is shared between all JToolTips the tip field is
		' set appropriately before this is invoked. Unfortunately this means
		' that subclasses that randomly invoke this method will see varying
		' results. If this becomes an issue, MetalToolTipUI should no longer be
		' shared.
		Public Overridable Property acceleratorString As String
			Get
				If tip Is Nothing OrElse acceleratorHidden Then Return ""
				Dim comp As JComponent = tip.component
				If Not(TypeOf comp Is AbstractButton) Then Return ""
    
				Dim keys As KeyStroke() = comp.getInputMap(JComponent.WHEN_IN_FOCUSED_WINDOW).keys()
				If keys Is Nothing Then Return ""
    
				Dim controlKeyStr As String = ""
    
				For i As Integer = 0 To keys.Length - 1
					Dim [mod] As Integer = keys(i).modifiers
					controlKeyStr = KeyEvent.getKeyModifiersText([mod]) + acceleratorDelimiter + KeyEvent.getKeyText(keys(i).keyCode)
					Exit For
				Next i
    
				Return controlKeyStr
			End Get
		End Property

	End Class

End Namespace