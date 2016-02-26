Imports javax.swing

'
' * Copyright (c) 1997, 2008, Oracle and/or its affiliates. All rights reserved.
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
	''' Pluggable look and feel interface for <code>JFileChooser</code>.
	''' 
	''' @author Jeff Dinkins
	''' </summary>

	Public MustInherit Class FileChooserUI
		Inherits ComponentUI

		Public MustOverride Function getAcceptAllFileFilter(ByVal fc As JFileChooser) As javax.swing.filechooser.FileFilter
		Public MustOverride Function getFileView(ByVal fc As JFileChooser) As javax.swing.filechooser.FileView

		Public MustOverride Function getApproveButtonText(ByVal fc As JFileChooser) As String
		Public MustOverride Function getDialogTitle(ByVal fc As JFileChooser) As String

		Public MustOverride Sub rescanCurrentDirectory(ByVal fc As JFileChooser)
		Public MustOverride Sub ensureFileIsVisible(ByVal fc As JFileChooser, ByVal f As java.io.File)

		''' <summary>
		''' Returns default button for current <code>LookAndFeel</code>.
		''' <code>JFileChooser</code> will use this button as default button
		''' for dialog windows.
		''' 
		''' @since 1.7
		''' </summary>
		Public Overridable Function getDefaultButton(ByVal fc As JFileChooser) As JButton
			Return Nothing
		End Function
	End Class

End Namespace