Imports javax.swing
Imports javax.swing.border
Imports javax.swing.plaf

'
' * Copyright (c) 1998, 2010, Oracle and/or its affiliates. All rights reserved.
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
	''' BasicViewport implementation
	''' 
	''' @author Rich Schiavi
	''' </summary>
	Public Class BasicViewportUI
		Inherits ViewportUI

		' Shared UI object
		Private Shared viewportUI As ViewportUI

		Public Shared Function createUI(ByVal c As JComponent) As ComponentUI
			If viewportUI Is Nothing Then viewportUI = New BasicViewportUI
			Return viewportUI
		End Function

		Public Overridable Sub installUI(ByVal c As JComponent)
			MyBase.installUI(c)
			installDefaults(c)
		End Sub

		Public Overridable Sub uninstallUI(ByVal c As JComponent)
			uninstallDefaults(c)
			MyBase.uninstallUI(c)
		End Sub

		Protected Friend Overridable Sub installDefaults(ByVal c As JComponent)
			LookAndFeel.installColorsAndFont(c, "Viewport.background", "Viewport.foreground", "Viewport.font")
			LookAndFeel.installProperty(c, "opaque", Boolean.TRUE)
		End Sub

		Protected Friend Overridable Sub uninstallDefaults(ByVal c As JComponent)
		End Sub
	End Class

End Namespace