'
' * Copyright (c) 2002, 2003, Oracle and/or its affiliates. All rights reserved.
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
	''' SynthUI is used to fetch the SynthContext for a particular Component.
	''' 
	''' @author Scott Violet
	''' @since 1.7
	''' </summary>
	Public Interface SynthUI
		Inherits SynthConstants

		''' <summary>
		''' Returns the Context for the specified component.
		''' </summary>
		''' <param name="c"> Component requesting SynthContext. </param>
		''' <returns> SynthContext describing component. </returns>
		Function getContext(ByVal c As javax.swing.JComponent) As SynthContext

		''' <summary>
		''' Paints the border.
		''' </summary>
		''' <param name="context"> a component context </param>
		''' <param name="g"> {@code Graphics} to paint on </param>
		''' <param name="x"> the X coordinate </param>
		''' <param name="y"> the Y coordinate </param>
		''' <param name="w"> width of the border </param>
		''' <param name="h"> height of the border </param>
		Sub paintBorder(ByVal context As SynthContext, ByVal g As java.awt.Graphics, ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer)
	End Interface

End Namespace