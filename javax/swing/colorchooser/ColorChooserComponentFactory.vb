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

Namespace javax.swing.colorchooser


	''' <summary>
	''' A class designed to produce preconfigured "accessory" objects to
	''' insert into color choosers.
	''' 
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
	Public Class ColorChooserComponentFactory

		Private Sub New() ' can't instantiate
		End Sub

		Public Property Shared defaultChooserPanels As AbstractColorChooserPanel()
			Get
				Return New AbstractColorChooserPanel() { New DefaultSwatchChooserPanel, New ColorChooserPanel(New ColorModelHSV), New ColorChooserPanel(New ColorModelHSL), New ColorChooserPanel(New ColorModel), New ColorChooserPanel(New ColorModelCMYK) }
			End Get
		End Property

		Public Property Shared previewPanel As javax.swing.JComponent
			Get
				Return New DefaultPreviewPanel
			End Get
		End Property
	End Class

End Namespace