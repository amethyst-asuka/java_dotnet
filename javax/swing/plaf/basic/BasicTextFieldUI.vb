Imports Microsoft.VisualBasic
Imports System
Imports javax.swing
Imports javax.swing.border
Imports javax.swing.event
Imports javax.swing.text
Imports javax.swing.plaf

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
Namespace javax.swing.plaf.basic


	''' <summary>
	''' Basis of a look and feel for a JTextField.
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
	''' @author  Timothy Prinzing
	''' </summary>
	Public Class BasicTextFieldUI
		Inherits BasicTextUI

		''' <summary>
		''' Creates a UI for a JTextField.
		''' </summary>
		''' <param name="c"> the text field </param>
		''' <returns> the UI </returns>
		Public Shared Function createUI(ByVal c As JComponent) As ComponentUI
			Return New BasicTextFieldUI
		End Function

		''' <summary>
		''' Creates a new BasicTextFieldUI.
		''' </summary>
		Public Sub New()
			MyBase.New()
		End Sub

		''' <summary>
		''' Fetches the name used as a key to lookup properties through the
		''' UIManager.  This is used as a prefix to all the standard
		''' text properties.
		''' </summary>
		''' <returns> the name ("TextField") </returns>
		Protected Friend Property Overrides propertyPrefix As String
			Get
				Return "TextField"
			End Get
		End Property

		''' <summary>
		''' Creates a view (FieldView) based on an element.
		''' </summary>
		''' <param name="elem"> the element </param>
		''' <returns> the view </returns>
		Public Overrides Function create(ByVal elem As Element) As View
			Dim doc As Document = elem.document
			Dim i18nFlag As Object = doc.getProperty("i18n") 'AbstractDocument.I18NProperty
			If Boolean.TRUE.Equals(i18nFlag) Then
				' To support bidirectional text, we build a more heavyweight
				' representation of the field.
				Dim kind As String = elem.name
				If kind IsNot Nothing Then
					If kind.Equals(AbstractDocument.ContentElementName) Then
						Return New GlyphView(elem)
					ElseIf kind.Equals(AbstractDocument.ParagraphElementName) Then
						Return New I18nFieldView(elem)
					End If
				End If
				' this shouldn't happen, should probably throw in this case.
			End If
			Return New FieldView(elem)
		End Function

		''' <summary>
		''' Returns the baseline.
		''' </summary>
		''' <exception cref="NullPointerException"> {@inheritDoc} </exception>
		''' <exception cref="IllegalArgumentException"> {@inheritDoc} </exception>
		''' <seealso cref= javax.swing.JComponent#getBaseline(int, int)
		''' @since 1.6 </seealso>
		Public Overridable Function getBaseline(ByVal c As JComponent, ByVal width As Integer, ByVal height As Integer) As Integer
			MyBase.getBaseline(c, width, height)
			Dim ___rootView As View = getRootView(CType(c, JTextComponent))
			If ___rootView.viewCount > 0 Then
				Dim insets As Insets = c.insets
				height = height - insets.top - insets.bottom
				If height > 0 Then
					Dim ___baseline As Integer = insets.top
					Dim fieldView As View = ___rootView.getView(0)
					Dim vspan As Integer = CInt(Fix(fieldView.getPreferredSpan(View.Y_AXIS)))
					If height <> vspan Then
						Dim slop As Integer = height - vspan
						___baseline += slop \ 2
					End If
					If TypeOf fieldView Is I18nFieldView Then
						Dim fieldBaseline As Integer = BasicHTML.getBaseline(fieldView, width - insets.left - insets.right, height)
						If fieldBaseline < 0 Then Return -1
						___baseline += fieldBaseline
					Else
						Dim fm As FontMetrics = c.getFontMetrics(c.font)
						___baseline += fm.ascent
					End If
					Return ___baseline
				End If
			End If
			Return -1
		End Function

		''' <summary>
		''' Returns an enum indicating how the baseline of the component
		''' changes as the size changes.
		''' </summary>
		''' <exception cref="NullPointerException"> {@inheritDoc} </exception>
		''' <seealso cref= javax.swing.JComponent#getBaseline(int, int)
		''' @since 1.6 </seealso>
		Public Overridable Function getBaselineResizeBehavior(ByVal c As JComponent) As Component.BaselineResizeBehavior
			MyBase.getBaselineResizeBehavior(c)
			Return Component.BaselineResizeBehavior.CENTER_OFFSET
		End Function


		''' <summary>
		''' A field view that support bidirectional text via the
		''' support provided by ParagraphView.
		''' </summary>
		Friend Class I18nFieldView
			Inherits ParagraphView

			Friend Sub New(ByVal elem As Element)
				MyBase.New(elem)
			End Sub

			''' <summary>
			''' Fetch the constraining span to flow against for
			''' the given child index.  There is no limit for
			''' a field since it scrolls, so this is implemented to
			''' return <code>Integer.MAX_VALUE</code>.
			''' </summary>
			Public Overrides Function getFlowSpan(ByVal index As Integer) As Integer
				Return Integer.MaxValue
			End Function

			Protected Friend Overrides Property justification As Integer
				Set(ByVal j As Integer)
					' Justification is done in adjustAllocation(), so disable
					' ParagraphView's justification handling by doing nothing here.
				End Set
			End Property

			Friend Shared Function isLeftToRight(ByVal c As java.awt.Component) As Boolean
				Return c.componentOrientation.leftToRight
			End Function

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
			Friend Overridable Function adjustAllocation(ByVal a As Shape) As Shape
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
							If isLeftToRight(c) Then
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
				MyBase.paint(g, adjustAllocation(a))
			End Sub

			''' <summary>
			''' Determines the resizability of the view along the
			''' given axis.  A value of 0 or less is not resizable.
			''' </summary>
			''' <param name="axis"> View.X_AXIS or View.Y_AXIS </param>
			''' <returns> the weight -> 1 for View.X_AXIS, else 0 </returns>
			Public Overrides Function getResizeWeight(ByVal axis As Integer) As Integer
				If axis = View.X_AXIS Then Return 1
				Return 0
			End Function

			''' <summary>
			''' Provides a mapping from the document model coordinate space
			''' to the coordinate space of the view mapped to it.
			''' </summary>
			''' <param name="pos"> the position to convert >= 0 </param>
			''' <param name="a"> the allocated region to render into </param>
			''' <returns> the bounding box of the given position </returns>
			''' <exception cref="BadLocationException">  if the given position does not
			'''   represent a valid location in the associated document </exception>
			''' <seealso cref= View#modelToView </seealso>
			Public Overrides Function modelToView(ByVal pos As Integer, ByVal a As Shape, ByVal b As Position.Bias) As Shape
				Return MyBase.modelToView(pos, adjustAllocation(a), b)
			End Function

			''' <summary>
			''' Provides a mapping from the document model coordinate space
			''' to the coordinate space of the view mapped to it.
			''' </summary>
			''' <param name="p0"> the position to convert >= 0 </param>
			''' <param name="b0"> the bias toward the previous character or the
			'''  next character represented by p0, in case the
			'''  position is a boundary of two views. </param>
			''' <param name="p1"> the position to convert >= 0 </param>
			''' <param name="b1"> the bias toward the previous character or the
			'''  next character represented by p1, in case the
			'''  position is a boundary of two views. </param>
			''' <param name="a"> the allocated region to render into </param>
			''' <returns> the bounding box of the given position is returned </returns>
			''' <exception cref="BadLocationException">  if the given position does
			'''   not represent a valid location in the associated document </exception>
			''' <exception cref="IllegalArgumentException"> for an invalid bias argument </exception>
			''' <seealso cref= View#viewToModel </seealso>
			Public Overrides Function modelToView(ByVal p0 As Integer, ByVal b0 As Position.Bias, ByVal p1 As Integer, ByVal b1 As Position.Bias, ByVal a As Shape) As Shape
				Return MyBase.modelToView(p0, b0, p1, b1, adjustAllocation(a))
			End Function

			''' <summary>
			''' Provides a mapping from the view coordinate space to the logical
			''' coordinate space of the model.
			''' </summary>
			''' <param name="fx"> the X coordinate >= 0.0f </param>
			''' <param name="fy"> the Y coordinate >= 0.0f </param>
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

	End Class

End Namespace