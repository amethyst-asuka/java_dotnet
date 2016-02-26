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
	''' Icon decorator that implements the view interface.  The
	''' entire element is used to represent the icon.  This acts
	''' as a gateway from the display-only View implementations to
	''' interactive lightweight icons (that is, it allows icons
	''' to be embedded into the View hierarchy.  The parent of the icon
	''' is the container that is handed out by the associated view
	''' factory.
	''' 
	''' @author Timothy Prinzing
	''' </summary>
	Public Class IconView
		Inherits View

		''' <summary>
		''' Creates a new icon view that represents an element.
		''' </summary>
		''' <param name="elem"> the element to create a view for </param>
		Public Sub New(ByVal elem As Element)
			MyBase.New(elem)
			Dim attr As AttributeSet = elem.attributes
			c = StyleConstants.getIcon(attr)
		End Sub

		' --- View methods ---------------------------------------------

		''' <summary>
		''' Paints the icon.
		''' The real paint behavior occurs naturally from the association
		''' that the icon has with its parent container (the same
		''' container hosting this view), so this simply allows us to
		''' position the icon properly relative to the view.  Since
		''' the coordinate system for the view is simply the parent
		''' containers, positioning the child icon is easy.
		''' </summary>
		''' <param name="g"> the rendering surface to use </param>
		''' <param name="a"> the allocated region to render into </param>
		''' <seealso cref= View#paint </seealso>
		Public Overrides Sub paint(ByVal g As Graphics, ByVal a As Shape)
			Dim alloc As Rectangle = a.bounds
			c.paintIcon(container, g, alloc.x, alloc.y)
		End Sub

		''' <summary>
		''' Determines the preferred span for this view along an
		''' axis.
		''' </summary>
		''' <param name="axis"> may be either View.X_AXIS or View.Y_AXIS </param>
		''' <returns>  the span the view would like to be rendered into
		'''           Typically the view is told to render into the span
		'''           that is returned, although there is no guarantee.
		'''           The parent may choose to resize or break the view. </returns>
		''' <exception cref="IllegalArgumentException"> for an invalid axis </exception>
		Public Overrides Function getPreferredSpan(ByVal axis As Integer) As Single
			Select Case axis
			Case View.X_AXIS
				Return c.iconWidth
			Case View.Y_AXIS
				Return c.iconHeight
			Case Else
				Throw New System.ArgumentException("Invalid axis: " & axis)
			End Select
		End Function

		''' <summary>
		''' Determines the desired alignment for this view along an
		''' axis.  This is implemented to give the alignment to the
		''' bottom of the icon along the y axis, and the default
		''' along the x axis.
		''' </summary>
		''' <param name="axis"> may be either View.X_AXIS or View.Y_AXIS </param>
		''' <returns> the desired alignment &gt;= 0.0f &amp;&amp; &lt;= 1.0f.  This should be
		'''   a value between 0.0 and 1.0 where 0 indicates alignment at the
		'''   origin and 1.0 indicates alignment to the full span
		'''   away from the origin.  An alignment of 0.5 would be the
		'''   center of the view. </returns>
		Public Overrides Function getAlignment(ByVal axis As Integer) As Single
			Select Case axis
			Case View.Y_AXIS
				Return 1
			Case Else
				Return MyBase.getAlignment(axis)
			End Select
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
			Dim p0 As Integer = startOffset
			Dim p1 As Integer = endOffset
			If (pos >= p0) AndAlso (pos <= p1) Then
				Dim r As Rectangle = a.bounds
				If pos = p1 Then r.x += r.width
				r.width = 0
				Return r
			End If
			Throw New BadLocationException(pos & " not in range " & p0 & "," & p1, pos)
		End Function

		''' <summary>
		''' Provides a mapping from the view coordinate space to the logical
		''' coordinate space of the model.
		''' </summary>
		''' <param name="x"> the X coordinate &gt;= 0 </param>
		''' <param name="y"> the Y coordinate &gt;= 0 </param>
		''' <param name="a"> the allocated region to render into </param>
		''' <returns> the location within the model that best represents the
		'''  given point of view &gt;= 0 </returns>
		''' <seealso cref= View#viewToModel </seealso>
		Public Overrides Function viewToModel(ByVal x As Single, ByVal y As Single, ByVal a As Shape, ByVal bias As Position.Bias()) As Integer
			Dim alloc As Rectangle = CType(a, Rectangle)
			If x < alloc.x + (alloc.width / 2) Then
				bias(0) = Position.Bias.Forward
				Return startOffset
			End If
			bias(0) = Position.Bias.Backward
			Return endOffset
		End Function

		' --- member variables ------------------------------------------------

		Private c As javax.swing.Icon
	End Class

End Namespace