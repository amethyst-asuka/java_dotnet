'
' * Copyright (c) 2002, 2013, Oracle and/or its affiliates. All rights reserved.
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

Namespace javax.swing.plaf.synth


	''' <summary>
	''' {@inheritDoc}
	''' 
	''' @author Georges Saab
	''' </summary>

	Friend Class SynthMenuLayout
		Inherits javax.swing.plaf.basic.DefaultMenuLayout

		Public Sub New(ByVal target As java.awt.Container, ByVal axis As Integer)
			MyBase.New(target, axis)
		End Sub

		Public Overrides Function preferredLayoutSize(ByVal target As java.awt.Container) As java.awt.Dimension
			If TypeOf target Is javax.swing.JPopupMenu Then
				Dim popupMenu As javax.swing.JPopupMenu = CType(target, javax.swing.JPopupMenu)
				popupMenu.putClientProperty(SynthMenuItemLayoutHelper.MAX_ACC_OR_ARROW_WIDTH, Nothing)
			End If

			Return MyBase.preferredLayoutSize(target)
		End Function
	End Class

End Namespace