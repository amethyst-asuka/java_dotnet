Imports javax.swing.text

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

	''' <summary>
	''' Text editor user interface
	''' 
	''' @author  Timothy Prinzing
	''' </summary>
	Public MustInherit Class TextUI
		Inherits ComponentUI

		''' <summary>
		''' Converts the given location in the model to a place in
		''' the view coordinate system.
		''' </summary>
		''' <param name="pos">  the local location in the model to translate &gt;= 0 </param>
		''' <returns> the coordinates as a rectangle </returns>
		''' <exception cref="BadLocationException">  if the given position does not
		'''   represent a valid location in the associated document </exception>
		Public MustOverride Function modelToView(ByVal t As JTextComponent, ByVal pos As Integer) As java.awt.Rectangle

		''' <summary>
		''' Converts the given location in the model to a place in
		''' the view coordinate system.
		''' </summary>
		''' <param name="pos">  the local location in the model to translate &gt;= 0 </param>
		''' <returns> the coordinates as a rectangle </returns>
		''' <exception cref="BadLocationException">  if the given position does not
		'''   represent a valid location in the associated document </exception>
		Public MustOverride Function modelToView(ByVal t As JTextComponent, ByVal pos As Integer, ByVal bias As Position.Bias) As java.awt.Rectangle

		''' <summary>
		''' Converts the given place in the view coordinate system
		''' to the nearest representative location in the model.
		''' </summary>
		''' <param name="pt">  the location in the view to translate.  This
		'''   should be in the same coordinate system as the mouse
		'''   events. </param>
		''' <returns> the offset from the start of the document &gt;= 0 </returns>
		Public MustOverride Function viewToModel(ByVal t As JTextComponent, ByVal pt As java.awt.Point) As Integer

		''' <summary>
		''' Provides a mapping from the view coordinate space to the logical
		''' coordinate space of the model.
		''' </summary>
		''' <param name="pt"> the location in the view to translate.
		'''           This should be in the same coordinate system
		'''           as the mouse events. </param>
		''' <param name="biasReturn">
		'''           filled in by this method to indicate whether
		'''           the point given is closer to the previous or the next
		'''           character in the model
		''' </param>
		''' <returns> the location within the model that best represents the
		'''         given point in the view &gt;= 0 </returns>
		Public MustOverride Function viewToModel(ByVal t As JTextComponent, ByVal pt As java.awt.Point, ByVal biasReturn As Position.Bias()) As Integer

		''' <summary>
		''' Provides a way to determine the next visually represented model
		''' location that one might place a caret.  Some views may not be visible,
		''' they might not be in the same order found in the model, or they just
		''' might not allow access to some of the locations in the model.
		''' </summary>
		''' <param name="t"> the text component for which this UI is installed </param>
		''' <param name="pos"> the position to convert &gt;= 0 </param>
		''' <param name="b"> the bias for the position </param>
		''' <param name="direction"> the direction from the current position that can
		'''  be thought of as the arrow keys typically found on a keyboard.
		'''  This may be SwingConstants.WEST, SwingConstants.EAST,
		'''  SwingConstants.NORTH, or SwingConstants.SOUTH </param>
		''' <param name="biasRet"> an array to contain the bias for the returned position </param>
		''' <returns> the location within the model that best represents the next
		'''  location visual position </returns>
		''' <exception cref="BadLocationException"> </exception>
		''' <exception cref="IllegalArgumentException"> for an invalid direction </exception>
		Public MustOverride Function getNextVisualPositionFrom(ByVal t As JTextComponent, ByVal pos As Integer, ByVal b As Position.Bias, ByVal direction As Integer, ByVal biasRet As Position.Bias()) As Integer

		''' <summary>
		''' Causes the portion of the view responsible for the
		''' given part of the model to be repainted.
		''' </summary>
		''' <param name="p0"> the beginning of the range &gt;= 0 </param>
		''' <param name="p1"> the end of the range &gt;= p0 </param>
		Public MustOverride Sub damageRange(ByVal t As JTextComponent, ByVal p0 As Integer, ByVal p1 As Integer)

		''' <summary>
		''' Causes the portion of the view responsible for the
		''' given part of the model to be repainted.
		''' </summary>
		''' <param name="p0"> the beginning of the range &gt;= 0 </param>
		''' <param name="p1"> the end of the range &gt;= p0 </param>
		Public MustOverride Sub damageRange(ByVal t As JTextComponent, ByVal p0 As Integer, ByVal p1 As Integer, ByVal firstBias As Position.Bias, ByVal secondBias As Position.Bias)

		''' <summary>
		''' Fetches the binding of services that set a policy
		''' for the type of document being edited.  This contains
		''' things like the commands available, stream readers and
		''' writers, etc.
		''' </summary>
		''' <returns> the editor kit binding </returns>
		Public MustOverride Function getEditorKit(ByVal t As JTextComponent) As EditorKit

		''' <summary>
		''' Fetches a View with the allocation of the associated
		''' text component (i.e. the root of the hierarchy) that
		''' can be traversed to determine how the model is being
		''' represented spatially.
		''' </summary>
		''' <returns> the view </returns>
		Public MustOverride Function getRootView(ByVal t As JTextComponent) As View

		''' <summary>
		''' Returns the string to be used as the tooltip at the passed in location.
		''' </summary>
		''' <seealso cref= javax.swing.text.JTextComponent#getToolTipText
		''' @since 1.4 </seealso>
		Public Overridable Function getToolTipText(ByVal t As JTextComponent, ByVal pt As java.awt.Point) As String
			Return Nothing
		End Function
	End Class

End Namespace