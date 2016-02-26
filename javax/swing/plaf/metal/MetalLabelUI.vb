Imports javax.swing
Imports javax.swing.plaf
Imports javax.swing.plaf.basic

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
	''' A Windows L&amp;F implementation of LabelUI.  This implementation
	''' is completely static, i.e. there's only one UIView implementation
	''' that's shared by all JLabel objects.
	''' 
	''' @author Hans Muller
	''' </summary>

	Public Class MetalLabelUI
		Inherits BasicLabelUI

	   ''' <summary>
	   ''' The default <code>MetalLabelUI</code> instance. This field might
	   ''' not be used. To change the default instance use a subclass which
	   ''' overrides the <code>createUI</code> method, and place that class
	   ''' name in defaults table under the key "LabelUI".
	   ''' </summary>
		Protected Friend Shared metalLabelUI As New MetalLabelUI

		Private Shared ReadOnly METAL_LABEL_UI_KEY As New Object

		Public Shared Function createUI(ByVal c As JComponent) As ComponentUI
			If System.securityManager IsNot Nothing Then
				Dim appContext As sun.awt.AppContext = sun.awt.AppContext.appContext
				Dim safeMetalLabelUI As MetalLabelUI = CType(appContext.get(METAL_LABEL_UI_KEY), MetalLabelUI)
				If safeMetalLabelUI Is Nothing Then
					safeMetalLabelUI = New MetalLabelUI
					appContext.put(METAL_LABEL_UI_KEY, safeMetalLabelUI)
				End If
				Return safeMetalLabelUI
			End If
			Return metalLabelUI
		End Function

		''' <summary>
		''' Just paint the text gray (Label.disabledForeground) rather than
		''' in the labels foreground color.
		''' </summary>
		''' <seealso cref= #paint </seealso>
		''' <seealso cref= #paintEnabledText </seealso>
		Protected Friend Overridable Sub paintDisabledText(ByVal l As JLabel, ByVal g As Graphics, ByVal s As String, ByVal textX As Integer, ByVal textY As Integer)
			Dim mnemIndex As Integer = l.displayedMnemonicIndex
			g.color = UIManager.getColor("Label.disabledForeground")
			sun.swing.SwingUtilities2.drawStringUnderlineCharAt(l, g, s, mnemIndex, textX, textY)
		End Sub
	End Class

End Namespace