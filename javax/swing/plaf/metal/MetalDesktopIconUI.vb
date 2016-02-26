Imports javax.swing
Imports javax.swing.border
Imports javax.swing.plaf

'
' * Copyright (c) 1998, 2003, Oracle and/or its affiliates. All rights reserved.
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
	''' Metal desktop icon.
	''' 
	''' @author Steve Wilson
	''' </summary>
	Public Class MetalDesktopIconUI
		Inherits javax.swing.plaf.basic.BasicDesktopIconUI

		Friend button As JButton
		Friend label As JLabel
		Friend titleListener As TitleListener
		Private width As Integer

		Public Shared Function createUI(ByVal c As JComponent) As ComponentUI
			Return New MetalDesktopIconUI
		End Function

		Public Sub New()
		End Sub

		Protected Friend Overrides Sub installDefaults()
			MyBase.installDefaults()
			LookAndFeel.installColorsAndFont(desktopIcon, "DesktopIcon.background", "DesktopIcon.foreground", "DesktopIcon.font")
			width = UIManager.getInt("DesktopIcon.width")
		End Sub

		Protected Friend Overrides Sub installComponents()
			frame = desktopIcon.internalFrame
			Dim icon As Icon = frame.frameIcon
			Dim title As String = frame.title

			button = New JButton(title, icon)
'JAVA TO VB CONVERTER TODO TASK: Anonymous inner classes are not converted to VB if the base type is not defined in the code being converted:
'			button.addActionListener(New ActionListener()
	'		{
	'								  public void actionPerformed(ActionEvent e)
	'								  {
	'			 deiconize();
	'								  }
	'		});
			button.font = desktopIcon.font
			button.background = desktopIcon.background
			button.foreground = desktopIcon.foreground

			Dim buttonH As Integer = button.preferredSize.height

			Dim drag As Icon = New MetalBumps((buttonH\3), buttonH, MetalLookAndFeel.controlHighlight, MetalLookAndFeel.controlDarkShadow, MetalLookAndFeel.control)
			label = New JLabel(drag)

			label.border = New MatteBorder(0, 2, 0, 1, desktopIcon.background)
			desktopIcon.layout = New BorderLayout(2, 0)
			desktopIcon.add(button, BorderLayout.CENTER)
			desktopIcon.add(label, BorderLayout.WEST)
		End Sub

		Protected Friend Overrides Sub uninstallComponents()
			desktopIcon.layout = Nothing
			desktopIcon.remove(label)
			desktopIcon.remove(button)
			button = Nothing
			frame = Nothing
		End Sub

		Protected Friend Overrides Sub installListeners()
			MyBase.installListeners()
'JAVA TO VB CONVERTER TODO TASK: Assignments within expressions are not supported in VB
			desktopIcon.internalFrame.addPropertyChangeListener(titleListener = New TitleListener(Me))
		End Sub

		Protected Friend Overrides Sub uninstallListeners()
			desktopIcon.internalFrame.removePropertyChangeListener(titleListener)
			titleListener = Nothing
			MyBase.uninstallListeners()
		End Sub


		Public Overrides Function getPreferredSize(ByVal c As JComponent) As Dimension
			' Metal desktop icons can not be resized.  Their dimensions should
			' always be the minimum size.  See getMinimumSize(JComponent c).
			Return getMinimumSize(c)
		End Function

		Public Overrides Function getMinimumSize(ByVal c As JComponent) As Dimension
			' For the metal desktop icon we will use the layout maanger to
			' determine the correct height of the component, but we want to keep
			' the width consistent according to the jlf spec.
			Return New Dimension(width, desktopIcon.layout.minimumLayoutSize(desktopIcon).height)
		End Function

		Public Overrides Function getMaximumSize(ByVal c As JComponent) As Dimension
			' Metal desktop icons can not be resized.  Their dimensions should
			' always be the minimum size.  See getMinimumSize(JComponent c).
			Return getMinimumSize(c)
		End Function

		Friend Class TitleListener
			Implements PropertyChangeListener

			Private ReadOnly outerInstance As MetalDesktopIconUI

			Public Sub New(ByVal outerInstance As MetalDesktopIconUI)
				Me.outerInstance = outerInstance
			End Sub

			Public Overridable Sub propertyChange(ByVal e As PropertyChangeEvent)
			  If e.propertyName.Equals("title") Then outerInstance.button.text = CStr(e.newValue)

			  If e.propertyName.Equals("frameIcon") Then outerInstance.button.icon = CType(e.newValue, Icon)
			End Sub
		End Class
	End Class

End Namespace