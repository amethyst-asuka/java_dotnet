Imports Microsoft.VisualBasic
Imports javax.swing
Imports javax.swing.border
Imports javax.swing.event
Imports javax.swing.text

'
' * Copyright (c) 1997, 2003, Oracle and/or its affiliates. All rights reserved.
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
Namespace javax.swing.text.html

	''' <summary>
	''' A view implementation to display an unwrapped
	''' preformatted line.<p>
	''' This subclasses ParagraphView, but this really only contains one
	''' Row of text.
	''' 
	''' @author  Timothy Prinzing
	''' </summary>
	Friend Class LineView
		Inherits ParagraphView

		''' <summary>
		''' Last place painted at. </summary>
		Friend tabBase As Integer

		''' <summary>
		''' Creates a LineView object.
		''' </summary>
		''' <param name="elem"> the element to wrap in a view </param>
		Public Sub New(ByVal elem As Element)
			MyBase.New(elem)
		End Sub

		''' <summary>
		''' Preformatted lines are not suppressed if they
		''' have only whitespace, so they are always visible.
		''' </summary>
		Public Property Overrides visible As Boolean
			Get
				Return True
			End Get
		End Property

		''' <summary>
		''' Determines the minimum span for this view along an
		''' axis.  The preformatted line should refuse to be
		''' sized less than the preferred size.
		''' </summary>
		''' <param name="axis"> may be either <code>View.X_AXIS</code> or
		'''  <code>View.Y_AXIS</code> </param>
		''' <returns>  the minimum span the view can be rendered into </returns>
		''' <seealso cref= View#getPreferredSpan </seealso>
		Public Overrides Function getMinimumSpan(ByVal axis As Integer) As Single
			Return getPreferredSpan(axis)
		End Function

		''' <summary>
		''' Gets the resize weight for the specified axis.
		''' </summary>
		''' <param name="axis"> may be either X_AXIS or Y_AXIS </param>
		''' <returns> the weight </returns>
		Public Overrides Function getResizeWeight(ByVal axis As Integer) As Integer
			Select Case axis
			Case View.X_AXIS
				Return 1
			Case View.Y_AXIS
				Return 0
			Case Else
				Throw New System.ArgumentException("Invalid axis: " & axis)
			End Select
		End Function

		''' <summary>
		''' Gets the alignment for an axis.
		''' </summary>
		''' <param name="axis"> may be either X_AXIS or Y_AXIS </param>
		''' <returns> the alignment </returns>
		Public Overrides Function getAlignment(ByVal axis As Integer) As Single
			If axis = View.X_AXIS Then Return 0
			Return MyBase.getAlignment(axis)
		End Function

		''' <summary>
		''' Lays out the children.  If the layout span has changed,
		''' the rows are rebuilt.  The superclass functionality
		''' is called after checking and possibly rebuilding the
		''' rows.  If the height has changed, the
		''' <code>preferenceChanged</code> method is called
		''' on the parent since the vertical preference is
		''' rigid.
		''' </summary>
		''' <param name="width">  the width to lay out against >= 0.  This is
		'''   the width inside of the inset area. </param>
		''' <param name="height"> the height to lay out against >= 0 (not used
		'''   by paragraph, but used by the superclass).  This
		'''   is the height inside of the inset area. </param>
		Protected Friend Overrides Sub layout(ByVal width As Integer, ByVal height As Integer)
			MyBase.layout(Integer.MaxValue - 1, height)
		End Sub

		''' <summary>
		''' Returns the next tab stop position given a reference position.
		''' This view implements the tab coordinate system, and calls
		''' <code>getTabbedSpan</code> on the logical children in the process
		''' of layout to determine the desired span of the children.  The
		''' logical children can delegate their tab expansion upward to
		''' the paragraph which knows how to expand tabs.
		''' <code>LabelView</code> is an example of a view that delegates
		''' its tab expansion needs upward to the paragraph.
		''' <p>
		''' This is implemented to try and locate a <code>TabSet</code>
		''' in the paragraph element's attribute set.  If one can be
		''' found, its settings will be used, otherwise a default expansion
		''' will be provided.  The base location for for tab expansion
		''' is the left inset from the paragraphs most recent allocation
		''' (which is what the layout of the children is based upon).
		''' </summary>
		''' <param name="x"> the X reference position </param>
		''' <param name="tabOffset"> the position within the text stream
		'''   that the tab occurred at >= 0. </param>
		''' <returns> the trailing end of the tab expansion >= 0 </returns>
		''' <seealso cref= TabSet </seealso>
		''' <seealso cref= TabStop </seealso>
		''' <seealso cref= LabelView </seealso>
		Public Overrides Function nextTabStop(ByVal x As Single, ByVal tabOffset As Integer) As Single
			' If the text isn't left justified, offset by 10 pixels!
			If tabSet Is Nothing AndAlso StyleConstants.getAlignment(attributes) = StyleConstants.ALIGN_LEFT Then Return getPreTab(x, tabOffset)
			Return MyBase.nextTabStop(x, tabOffset)
		End Function

		''' <summary>
		''' Returns the location for the tab.
		''' </summary>
		Protected Friend Overridable Function getPreTab(ByVal x As Single, ByVal tabOffset As Integer) As Single
			Dim d As Document = document
			Dim v As View = getViewAtPosition(tabOffset, Nothing)
			If (TypeOf d Is StyledDocument) AndAlso v IsNot Nothing Then
				' Assume f is fixed point.
				Dim f As Font = CType(d, StyledDocument).getFont(v.attributes)
				Dim c As Container = container
				Dim fm As FontMetrics = If(c IsNot Nothing, c.getFontMetrics(f), Toolkit.defaultToolkit.getFontMetrics(f))
				Dim ___width As Integer = charactersPerTab * fm.charWidth("W"c)
				Dim tb As Integer = CInt(Fix(tabBase))
				Return CSng(((CInt(Fix(x)) - tb) \ ___width + 1) * ___width + tb)
			End If
			Return 10.0f + x
		End Function

		''' <returns> number of characters per tab, 8. </returns>
		Protected Friend Overridable Property charactersPerTab As Integer
			Get
				Return 8
			End Get
		End Property
	End Class

End Namespace