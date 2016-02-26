Imports Microsoft.VisualBasic
Imports System
Imports javax.swing
Imports javax.swing.event

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
Namespace javax.swing.text

	''' <summary>
	''' Extends the multi-line plain text view to be suitable
	''' for a single-line editor view.  If the view is
	''' allocated extra space, the field must adjust for it.
	''' If the hosting component is a JTextField, this view
	''' will manage the ranges of the associated BoundedRangeModel
	''' and will adjust the horizontal allocation to match the
	''' current visibility settings of the JTextField.
	''' 
	''' @author  Timothy Prinzing </summary>
	''' <seealso cref=     View </seealso>
	Public Class FieldView
		Inherits PlainView

		''' <summary>
		''' Constructs a new FieldView wrapped on an element.
		''' </summary>
		''' <param name="elem"> the element </param>
		Public Sub New(ByVal elem As Element)
			MyBase.New(elem)
		End Sub

		''' <summary>
		''' Fetches the font metrics associated with the component hosting
		''' this view.
		''' </summary>
		''' <returns> the metrics </returns>
		Protected Friend Overridable Property fontMetrics As FontMetrics
			Get
				Dim c As Component = container
				Return c.getFontMetrics(c.font)
			End Get
		End Property

		''' <summary>
		''' Adjusts the allocation given to the view
		''' to be a suitable allocation for a text field.
		''' If the view has been allocated more than the
		''' preferred span vertically, the allocation is
		''' changed to be centered vertically.  Horizontally
		''' the view is adjusted according to the horizontal
		''' alignment property set on the associated JTextField
		''' (if that is the type of the hosting component).
		''' </summary>
		''' <param name="a"> the allocation given to the view, which may need
		'''  to be adjusted. </param>
		''' <returns> the allocation that the superclass should use. </returns>
		Protected Friend Overridable Function adjustAllocation(ByVal a As Shape) As Shape
			If a IsNot Nothing Then
				Dim bounds As Rectangle = a.bounds
				Dim vspan As Integer = CInt(Fix(getPreferredSpan(Y_AXIS)))
				Dim hspan As Integer = CInt(Fix(getPreferredSpan(X_AXIS)))
				If bounds.height <> vspan Then
					Dim slop As Integer = bounds.height - vspan
					bounds.y += slop \ 2
					bounds.height -= slop
				End If

				' horizontal adjustments
				Dim c As Component = container
				If TypeOf c Is JTextField Then
					Dim field As JTextField = CType(c, JTextField)
					Dim vis As BoundedRangeModel = field.horizontalVisibility
					Dim max As Integer = Math.Max(hspan, bounds.width)
					Dim value As Integer = vis.value
					Dim extent As Integer = Math.Min(max, bounds.width - 1)
					If (value + extent) > max Then value = max - extent
					vis.rangePropertiesies(value, extent, vis.minimum, max, False)
					If hspan < bounds.width Then
						' horizontally align the interior
						Dim slop As Integer = bounds.width - 1 - hspan

						Dim align As Integer = CType(c, JTextField).horizontalAlignment
						If Utilities.isLeftToRight(c) Then
							If align=LEADING Then
								align = LEFT
							ElseIf align=TRAILING Then
								align = RIGHT
							End If
						Else
							If align=LEADING Then
								align = RIGHT
							ElseIf align=TRAILING Then
								align = LEFT
							End If
						End If

						Select Case align
						Case SwingConstants.CENTER
							bounds.x += slop \ 2
							bounds.width -= slop
						Case SwingConstants.RIGHT
							bounds.x += slop
							bounds.width -= slop
						End Select
					Else
						' adjust the allocation to match the bounded range.
						bounds.width = hspan
						bounds.x -= vis.value
					End If
				End If
				Return bounds
			End If
			Return Nothing
		End Function

		''' <summary>
		''' Update the visibility model with the associated JTextField
		''' (if there is one) to reflect the current visibility as a
		''' result of changes to the document model.  The bounded
		''' range properties are updated.  If the view hasn't yet been
		''' shown the extent will be zero and we just set it to be full
		''' until determined otherwise.
		''' </summary>
		Friend Overridable Sub updateVisibilityModel()
			Dim c As Component = container
			If TypeOf c Is JTextField Then
				Dim field As JTextField = CType(c, JTextField)
				Dim vis As BoundedRangeModel = field.horizontalVisibility
				Dim hspan As Integer = CInt(Fix(getPreferredSpan(X_AXIS)))
				Dim extent As Integer = vis.extent
				Dim maximum As Integer = Math.Max(hspan, extent)
				extent = If(extent = 0, maximum, extent)
				Dim value As Integer = maximum - extent
				Dim oldValue As Integer = vis.value
				If (oldValue + extent) > maximum Then oldValue = maximum - extent
				value = Math.Max(0, Math.Min(value, oldValue))
				vis.rangePropertiesies(value, extent, 0, maximum, False)
			End If
		End Sub

		' --- View methods -------------------------------------------

		''' <summary>
		''' Renders using the given rendering surface and area on that surface.
		''' The view may need to do layout and create child views to enable
		''' itself to render into the given allocation.
		''' </summary>
		''' <param name="g"> the rendering surface to use </param>
		''' <param name="a"> the allocated region to render into
		''' </param>
		''' <seealso cref= View#paint </seealso>
		Public Overrides Sub paint(ByVal g As Graphics, ByVal a As Shape)
			Dim r As Rectangle = CType(a, Rectangle)
			g.clipRect(r.x, r.y, r.width, r.height)
			MyBase.paint(g, a)
		End Sub

		''' <summary>
		''' Adjusts <code>a</code> based on the visible region and returns it.
		''' </summary>
		Friend Overrides Function adjustPaintRegion(ByVal a As Shape) As Shape
			Return adjustAllocation(a)
		End Function

		''' <summary>
		''' Determines the preferred span for this view along an
		''' axis.
		''' </summary>
		''' <param name="axis"> may be either View.X_AXIS or View.Y_AXIS </param>
		''' <returns>   the span the view would like to be rendered into &gt;= 0.
		'''           Typically the view is told to render into the span
		'''           that is returned, although there is no guarantee.
		'''           The parent may choose to resize or break the view. </returns>
		Public Overrides Function getPreferredSpan(ByVal axis As Integer) As Single
			Select Case axis
			Case View.X_AXIS
				Dim buff As Segment = SegmentCache.sharedSegment
				Dim doc As Document = document
				Dim width As Integer
				Try
					Dim fm As FontMetrics = fontMetrics
					doc.getText(0, doc.length, buff)
					width = Utilities.getTabbedTextWidth(buff, fm, 0, Me, 0)
					If buff.count > 0 Then
						Dim c As Component = container
						firstLineOffset = sun.swing.SwingUtilities2.0 getLeftSideBearing(If(TypeOf c Is JComponent, CType(c, JComponent), Nothing), fm, buff.array(buff.offset))
						firstLineOffset = Math.Max(0, -firstLineOffset)
					Else
						firstLineOffset = 0
					End If
				Catch bl As BadLocationException
					width = 0
				End Try
				SegmentCache.releaseSharedSegment(buff)
				Return width + firstLineOffset
			Case Else
				Return MyBase.getPreferredSpan(axis)
			End Select
		End Function

		''' <summary>
		''' Determines the resizability of the view along the
		''' given axis.  A value of 0 or less is not resizable.
		''' </summary>
		''' <param name="axis"> View.X_AXIS or View.Y_AXIS </param>
		''' <returns> the weight -&gt; 1 for View.X_AXIS, else 0 </returns>
		Public Overrides Function getResizeWeight(ByVal axis As Integer) As Integer
			If axis = View.X_AXIS Then Return 1
			Return 0
		End Function

		''' <summary>
		''' Provides a mapping from the document model coordinate space
		''' to the coordinate space of the view mapped to it.
		''' </summary>
		''' <param name="pos"> the position to convert &gt;= 0 </param>
		''' <param name="a"> the allocated region to render into </param>
		''' <returns> the bounding box of the given position </returns>
		''' <exception cref="BadLocationException">  if the given position does not
		'''   represent a valid location in the associated document </exception>
		''' <seealso cref= View#modelToView </seealso>
		Public Overrides Function modelToView(ByVal pos As Integer, ByVal a As Shape, ByVal b As Position.Bias) As Shape
			Return MyBase.modelToView(pos, adjustAllocation(a), b)
		End Function

		''' <summary>
		''' Provides a mapping from the view coordinate space to the logical
		''' coordinate space of the model.
		''' </summary>
		''' <param name="fx"> the X coordinate &gt;= 0.0f </param>
		''' <param name="fy"> the Y coordinate &gt;= 0.0f </param>
		''' <param name="a"> the allocated region to render into </param>
		''' <returns> the location within the model that best represents the
		'''  given point in the view </returns>
		''' <seealso cref= View#viewToModel </seealso>
		Public Overrides Function viewToModel(ByVal fx As Single, ByVal fy As Single, ByVal a As Shape, ByVal bias As Position.Bias()) As Integer
			Return MyBase.viewToModel(fx, fy, adjustAllocation(a), bias)
		End Function

		''' <summary>
		''' Gives notification that something was inserted into the document
		''' in a location that this view is responsible for.
		''' </summary>
		''' <param name="changes"> the change information from the associated document </param>
		''' <param name="a"> the current allocation of the view </param>
		''' <param name="f"> the factory to use to rebuild if the view has children </param>
		''' <seealso cref= View#insertUpdate </seealso>
		Public Overrides Sub insertUpdate(ByVal changes As DocumentEvent, ByVal a As Shape, ByVal f As ViewFactory)
			MyBase.insertUpdate(changes, adjustAllocation(a), f)
			updateVisibilityModel()
		End Sub

		''' <summary>
		''' Gives notification that something was removed from the document
		''' in a location that this view is responsible for.
		''' </summary>
		''' <param name="changes"> the change information from the associated document </param>
		''' <param name="a"> the current allocation of the view </param>
		''' <param name="f"> the factory to use to rebuild if the view has children </param>
		''' <seealso cref= View#removeUpdate </seealso>
		Public Overrides Sub removeUpdate(ByVal changes As DocumentEvent, ByVal a As Shape, ByVal f As ViewFactory)
			MyBase.removeUpdate(changes, adjustAllocation(a), f)
			updateVisibilityModel()
		End Sub

	End Class

End Namespace