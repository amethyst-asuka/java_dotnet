Imports Microsoft.VisualBasic
Imports javax.swing

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

Namespace javax.swing.plaf.basic




	''' <summary>
	''' Standard tool tip L&amp;F.
	''' <p>
	''' 
	''' @author Dave Moore
	''' </summary>
	Public Class BasicToolTipUI
		Inherits javax.swing.plaf.ToolTipUI

		Friend Shared sharedInstance As New BasicToolTipUI
		''' <summary>
		''' Global <code>PropertyChangeListener</code> that
		''' <code>createPropertyChangeListener</code> returns.
		''' </summary>
		Private Shared sharedPropertyChangedListener As java.beans.PropertyChangeListener

		Private propertyChangeListener As java.beans.PropertyChangeListener

		Public Shared Function createUI(ByVal c As JComponent) As javax.swing.plaf.ComponentUI
			Return sharedInstance
		End Function

		Public Sub New()
			MyBase.New()
		End Sub

		Public Overridable Sub installUI(ByVal c As JComponent)
			installDefaults(c)
			installComponents(c)
			installListeners(c)
		End Sub

		Public Overridable Sub uninstallUI(ByVal c As JComponent)
			' REMIND: this is NOT getting called
			uninstallDefaults(c)
			uninstallComponents(c)
			uninstallListeners(c)
		End Sub

		Protected Friend Overridable Sub installDefaults(ByVal c As JComponent)
			LookAndFeel.installColorsAndFont(c, "ToolTip.background", "ToolTip.foreground", "ToolTip.font")
			LookAndFeel.installProperty(c, "opaque", Boolean.TRUE)
			componentChanged(c)
		End Sub

	   Protected Friend Overridable Sub uninstallDefaults(ByVal c As JComponent)
			LookAndFeel.uninstallBorder(c)
	   End Sub

	'     Unfortunately this has to remain private until we can make API additions.
	'     
		Private Sub installComponents(ByVal c As JComponent)
			BasicHTML.updateRenderer(c, CType(c, JToolTip).tipText)
		End Sub

	'     Unfortunately this has to remain private until we can make API additions.
	'     
		Private Sub uninstallComponents(ByVal c As JComponent)
			BasicHTML.updateRenderer(c, "")
		End Sub

		Protected Friend Overridable Sub installListeners(ByVal c As JComponent)
			propertyChangeListener = createPropertyChangeListener(c)

			c.addPropertyChangeListener(propertyChangeListener)
		End Sub

		Protected Friend Overridable Sub uninstallListeners(ByVal c As JComponent)
			c.removePropertyChangeListener(propertyChangeListener)

			propertyChangeListener = Nothing
		End Sub

	'     Unfortunately this has to remain private until we can make API additions.
	'     
		Private Function createPropertyChangeListener(ByVal c As JComponent) As java.beans.PropertyChangeListener
			If sharedPropertyChangedListener Is Nothing Then sharedPropertyChangedListener = New PropertyChangeHandler
			Return sharedPropertyChangedListener
		End Function

		Public Overridable Sub paint(ByVal g As Graphics, ByVal c As JComponent)
			Dim font As Font = c.font
			Dim metrics As FontMetrics = sun.swing.SwingUtilities2.getFontMetrics(c, g, font)
			Dim size As Dimension = c.size

			g.color = c.foreground
			' fix for bug 4153892
			Dim tipText As String = CType(c, JToolTip).tipText
			If tipText Is Nothing Then tipText = ""

			Dim insets As Insets = c.insets
			Dim paintTextR As New Rectangle(insets.left + 3, insets.top, size.width - (insets.left + insets.right) - 6, size.height - (insets.top + insets.bottom))
			Dim v As javax.swing.text.View = CType(c.getClientProperty(BasicHTML.propertyKey), javax.swing.text.View)
			If v IsNot Nothing Then
				v.paint(g, paintTextR)
			Else
				g.font = font
				sun.swing.SwingUtilities2.drawString(c, g, tipText, paintTextR.x, paintTextR.y + metrics.ascent)
			End If
		End Sub

		Public Overridable Function getPreferredSize(ByVal c As JComponent) As Dimension
			Dim font As Font = c.font
			Dim fm As FontMetrics = c.getFontMetrics(font)
			Dim insets As Insets = c.insets

			Dim prefSize As New Dimension(insets.left+insets.right, insets.top+insets.bottom)
			Dim text As String = CType(c, JToolTip).tipText

			If (text Is Nothing) OrElse text.Equals("") Then
				text = ""
			Else
				Dim v As javax.swing.text.View = If(c IsNot Nothing, CType(c.getClientProperty("html"), javax.swing.text.View), Nothing)
				If v IsNot Nothing Then
					prefSize.width += CInt(Fix(v.getPreferredSpan(javax.swing.text.View.X_AXIS))) + 6
					prefSize.height += CInt(Fix(v.getPreferredSpan(javax.swing.text.View.Y_AXIS)))
				Else
					prefSize.width += sun.swing.SwingUtilities2.stringWidth(c,fm,text) + 6
					prefSize.height += fm.height
				End If
			End If
			Return prefSize
		End Function

		Public Overridable Function getMinimumSize(ByVal c As JComponent) As Dimension
			Dim d As Dimension = getPreferredSize(c)
			Dim v As javax.swing.text.View = CType(c.getClientProperty(BasicHTML.propertyKey), javax.swing.text.View)
			If v IsNot Nothing Then d.width -= v.getPreferredSpan(javax.swing.text.View.X_AXIS) - v.getMinimumSpan(javax.swing.text.View.X_AXIS)
			Return d
		End Function

		Public Overridable Function getMaximumSize(ByVal c As JComponent) As Dimension
			Dim d As Dimension = getPreferredSize(c)
			Dim v As javax.swing.text.View = CType(c.getClientProperty(BasicHTML.propertyKey), javax.swing.text.View)
			If v IsNot Nothing Then d.width += v.getMaximumSpan(javax.swing.text.View.X_AXIS) - v.getPreferredSpan(javax.swing.text.View.X_AXIS)
			Return d
		End Function

		''' <summary>
		''' Invoked when the <code>JCompoment</code> associated with the
		''' <code>JToolTip</code> has changed, or at initialization time. This
		''' should update any state dependant upon the <code>JComponent</code>.
		''' </summary>
		''' <param name="c"> the JToolTip the JComponent has changed on. </param>
		Private Sub componentChanged(ByVal c As JComponent)
			Dim comp As JComponent = CType(c, JToolTip).component

			If comp IsNot Nothing AndAlso Not(comp.enabled) Then
				' For better backward compatibility, only install inactive
				' properties if they are defined.
				If UIManager.getBorder("ToolTip.borderInactive") IsNot Nothing Then
					LookAndFeel.installBorder(c, "ToolTip.borderInactive")
				Else
					LookAndFeel.installBorder(c, "ToolTip.border")
				End If
				If UIManager.getColor("ToolTip.backgroundInactive") IsNot Nothing Then
					LookAndFeel.installColors(c,"ToolTip.backgroundInactive", "ToolTip.foregroundInactive")
				Else
					LookAndFeel.installColors(c,"ToolTip.background", "ToolTip.foreground")
				End If
			Else
				LookAndFeel.installBorder(c, "ToolTip.border")
				LookAndFeel.installColors(c, "ToolTip.background", "ToolTip.foreground")
			End If
		End Sub


		Private Class PropertyChangeHandler
			Implements java.beans.PropertyChangeListener

			Public Overridable Sub propertyChange(ByVal e As java.beans.PropertyChangeEvent)
				Dim name As String = e.propertyName
				If name.Equals("tiptext") OrElse "font".Equals(name) OrElse "foreground".Equals(name) Then
					' remove the old html view client property if one
					' existed, and install a new one if the text installed
					' into the JLabel is html source.
					Dim tip As JToolTip = (CType(e.source, JToolTip))
					Dim text As String = tip.tipText
					BasicHTML.updateRenderer(tip, text)
				ElseIf "component".Equals(name) Then
					Dim tip As JToolTip = (CType(e.source, JToolTip))

					If TypeOf tip.uI Is BasicToolTipUI Then CType(tip.uI, BasicToolTipUI).componentChanged(tip)
				End If
			End Sub
		End Class
	End Class

End Namespace