Imports Microsoft.VisualBasic

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
Namespace javax.swing.text.html


	''' <summary>
	''' Displays the a paragraph, and uses css attributes for its
	''' configuration.
	''' 
	''' @author  Timothy Prinzing
	''' </summary>

	Public Class ParagraphView
		Inherits javax.swing.text.ParagraphView

		''' <summary>
		''' Constructs a ParagraphView for the given element.
		''' </summary>
		''' <param name="elem"> the element that this view is responsible for </param>
		Public Sub New(ByVal elem As javax.swing.text.Element)
			MyBase.New(elem)
		End Sub

		''' <summary>
		''' Establishes the parent view for this view.  This is
		''' guaranteed to be called before any other methods if the
		''' parent view is functioning properly.
		''' <p>
		''' This is implemented
		''' to forward to the superclass as well as call the
		''' <seealso cref="#setPropertiesFromAttributes setPropertiesFromAttributes"/>
		''' method to set the paragraph properties from the css
		''' attributes.  The call is made at this time to ensure
		''' the ability to resolve upward through the parents
		''' view attributes.
		''' </summary>
		''' <param name="parent"> the new parent, or null if the view is
		'''  being removed from a parent it was previously added
		'''  to </param>
		Public Overridable Property parent As javax.swing.text.View
			Set(ByVal parent As javax.swing.text.View)
				MyBase.parent = parent
				If parent IsNot Nothing Then propertiesFromAttributestes()
			End Set
		End Property

		''' <summary>
		''' Fetches the attributes to use when rendering.  This is
		''' implemented to multiplex the attributes specified in the
		''' model with a StyleSheet.
		''' </summary>
		Public Property Overrides attributes As javax.swing.text.AttributeSet
			Get
				If attr Is Nothing Then
					Dim sheet As StyleSheet = styleSheet
					attr = sheet.getViewAttributes(Me)
				End If
				Return attr
			End Get
		End Property

		''' <summary>
		''' Sets up the paragraph from css attributes instead of
		''' the values found in StyleConstants (i.e. which are used
		''' by the superclass).  Since
		''' </summary>
		Protected Friend Overrides Sub setPropertiesFromAttributes()
			Dim sheet As StyleSheet = styleSheet
			attr = sheet.getViewAttributes(Me)
			painter = sheet.getBoxPainter(attr)
			If attr IsNot Nothing Then
				MyBase.propertiesFromAttributestes()
				insetsets(CShort(Fix(painter.getInset(TOP, Me))), CShort(Fix(painter.getInset(LEFT, Me))), CShort(Fix(painter.getInset(BOTTOM, Me))), CShort(Fix(painter.getInset(RIGHT, Me))))
				Dim o As Object = attr.getAttribute(CSS.Attribute.TEXT_ALIGN)
				If o IsNot Nothing Then
					' set horizontal alignment
					Dim ta As String = o.ToString()
					If ta.Equals("left") Then
						justification = javax.swing.text.StyleConstants.ALIGN_LEFT
					ElseIf ta.Equals("center") Then
						justification = javax.swing.text.StyleConstants.ALIGN_CENTER
					ElseIf ta.Equals("right") Then
						justification = javax.swing.text.StyleConstants.ALIGN_RIGHT
					ElseIf ta.Equals("justify") Then
						justification = javax.swing.text.StyleConstants.ALIGN_JUSTIFIED
					End If
				End If
				' Get the width/height
				cssWidth = CType(attr.getAttribute(CSS.Attribute.WIDTH), CSS.LengthValue)
				cssHeight = CType(attr.getAttribute(CSS.Attribute.HEIGHT), CSS.LengthValue)
			End If
		End Sub

		Protected Friend Overridable Property styleSheet As StyleSheet
			Get
				Dim doc As HTMLDocument = CType(document, HTMLDocument)
				Return doc.styleSheet
			End Get
		End Property


		''' <summary>
		''' Calculate the needs for the paragraph along the minor axis.
		''' 
		''' <p>If size requirements are explicitly specified for the paragraph,
		''' use that requirements.  Otherwise, use the requirements of the
		''' superclass <seealso cref="javax.swing.text.ParagraphView"/>.</p>
		''' 
		''' <p>If the {@code axis} parameter is neither {@code View.X_AXIS} nor
		''' {@code View.Y_AXIS}, <seealso cref="IllegalArgumentException"/> is thrown.  If the
		''' {@code r} parameter is {@code null,} a new {@code SizeRequirements}
		''' object is created, otherwise the supplied {@code SizeRequirements}
		''' object is returned.</p>
		''' </summary>
		''' <param name="axis">  the minor axis </param>
		''' <param name="r">     the input {@code SizeRequirements} object </param>
		''' <returns>      the new or adjusted {@code SizeRequirements} object </returns>
		''' <exception cref="IllegalArgumentException">  if the {@code axis} parameter is invalid </exception>
		Protected Friend Overrides Function calculateMinorAxisRequirements(ByVal axis As Integer, ByVal r As javax.swing.SizeRequirements) As javax.swing.SizeRequirements
			r = MyBase.calculateMinorAxisRequirements(axis, r)

			If BlockView.spanSetFromAttributes(axis, r, cssWidth, cssHeight) Then
				' Offset by the margins so that pref/min/max return the
				' right value.
				Dim margin As Integer = If(axis = X_AXIS, leftInset + rightInset, topInset + bottomInset)
				r.minimum -= margin
				r.preferred -= margin
				r.maximum -= margin
			End If
			Return r
		End Function


		''' <summary>
		''' Indicates whether or not this view should be
		''' displayed.  If none of the children wish to be
		''' displayed and the only visible child is the
		''' break that ends the paragraph, the paragraph
		''' will not be considered visible.  Otherwise,
		''' it will be considered visible and return true.
		''' </summary>
		''' <returns> true if the paragraph should be displayed </returns>
		Public Property Overrides visible As Boolean
			Get
    
				Dim n As Integer = layoutViewCount - 1
				For i As Integer = 0 To n - 1
					Dim v As javax.swing.text.View = getLayoutView(i)
					If v.visible Then Return True
				Next i
				If n > 0 Then
					Dim v As javax.swing.text.View = getLayoutView(n)
					If (v.endOffset - v.startOffset) = 1 Then Return False
				End If
				' If it's the last paragraph and not editable, it shouldn't
				' be visible.
				If startOffset = document.length Then
					Dim editable As Boolean = False
					Dim c As Component = container
					If TypeOf c Is javax.swing.text.JTextComponent Then editable = CType(c, javax.swing.text.JTextComponent).editable
					If Not editable Then Return False
				End If
				Return True
			End Get
		End Property

		''' <summary>
		''' Renders using the given rendering surface and area on that
		''' surface.  This is implemented to delegate to the superclass
		''' after stashing the base coordinate for tab calculations.
		''' </summary>
		''' <param name="g"> the rendering surface to use </param>
		''' <param name="a"> the allocated region to render into </param>
		''' <seealso cref= View#paint </seealso>
		Public Overrides Sub paint(ByVal g As Graphics, ByVal a As Shape)
			If a Is Nothing Then Return

			Dim r As Rectangle
			If TypeOf a Is Rectangle Then
				r = CType(a, Rectangle)
			Else
				r = a.bounds
			End If
			painter.paint(g, r.x, r.y, r.width, r.height, Me)
			MyBase.paint(g, a)
		End Sub

		''' <summary>
		''' Determines the preferred span for this view.  Returns
		''' 0 if the view is not visible, otherwise it calls the
		''' superclass method to get the preferred span.
		''' axis.
		''' </summary>
		''' <param name="axis"> may be either View.X_AXIS or View.Y_AXIS </param>
		''' <returns>   the span the view would like to be rendered into;
		'''           typically the view is told to render into the span
		'''           that is returned, although there is no guarantee;
		'''           the parent may choose to resize or break the view </returns>
		''' <seealso cref= javax.swing.text.ParagraphView#getPreferredSpan </seealso>
		Public Overrides Function getPreferredSpan(ByVal axis As Integer) As Single
			If Not visible Then Return 0
			Return MyBase.getPreferredSpan(axis)
		End Function

		''' <summary>
		''' Determines the minimum span for this view along an
		''' axis.  Returns 0 if the view is not visible, otherwise
		''' it calls the superclass method to get the minimum span.
		''' </summary>
		''' <param name="axis"> may be either <code>View.X_AXIS</code> or
		'''  <code>View.Y_AXIS</code> </param>
		''' <returns>  the minimum span the view can be rendered into </returns>
		''' <seealso cref= javax.swing.text.ParagraphView#getMinimumSpan </seealso>
		Public Overrides Function getMinimumSpan(ByVal axis As Integer) As Single
			If Not visible Then Return 0
			Return MyBase.getMinimumSpan(axis)
		End Function

		''' <summary>
		''' Determines the maximum span for this view along an
		''' axis.  Returns 0 if the view is not visible, otherwise
		''' it calls the superclass method ot get the maximum span.
		''' </summary>
		''' <param name="axis"> may be either <code>View.X_AXIS</code> or
		'''  <code>View.Y_AXIS</code> </param>
		''' <returns>  the maximum span the view can be rendered into </returns>
		''' <seealso cref= javax.swing.text.ParagraphView#getMaximumSpan </seealso>
		Public Overrides Function getMaximumSpan(ByVal axis As Integer) As Single
			If Not visible Then Return 0
			Return MyBase.getMaximumSpan(axis)
		End Function

		Private attr As javax.swing.text.AttributeSet
		Private painter As StyleSheet.BoxPainter
		Private cssWidth As CSS.LengthValue
		Private cssHeight As CSS.LengthValue
	End Class

End Namespace