Imports javax.swing.text

'
' * Copyright (c) 1997, 1999, Oracle and/or its affiliates. All rights reserved.
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
	''' A view implementation to display an html list
	''' 
	''' @author  Timothy Prinzing
	''' </summary>
	Public Class ListView
		Inherits BlockView

		''' <summary>
		''' Creates a new view that represents a list element.
		''' </summary>
		''' <param name="elem"> the element to create a view for </param>
		Public Sub New(ByVal elem As Element)
			MyBase.New(elem, View.Y_AXIS)
		End Sub

		''' <summary>
		''' Calculates the desired shape of the list.
		''' </summary>
		''' <returns> the desired span </returns>
		''' <seealso cref= View#getPreferredSpan </seealso>
		Public Overrides Function getAlignment(ByVal axis As Integer) As Single
			Select Case axis
			Case View.X_AXIS
				Return 0.5f
			Case View.Y_AXIS
				Return 0.5f
			Case Else
				Throw New System.ArgumentException("Invalid axis: " & axis)
			End Select
		End Function

		''' <summary>
		''' Renders using the given rendering surface and area on that
		''' surface.
		''' </summary>
		''' <param name="g"> the rendering surface to use </param>
		''' <param name="allocation"> the allocated region to render into </param>
		''' <seealso cref= View#paint </seealso>
		Public Overrides Sub paint(ByVal g As Graphics, ByVal allocation As Shape)
			MyBase.paint(g, allocation)
			Dim alloc As Rectangle = allocation.bounds
			Dim clip As Rectangle = g.clipBounds
			' Since listPainter paints in the insets we have to check for the
			' case where the child is not painted because the paint region is
			' to the left of the child. This assumes the ListPainter paints in
			' the left margin.
			If (clip.x + clip.width) < (alloc.x + leftInset) Then
				Dim childRect As Rectangle = alloc
				alloc = getInsideAllocation(allocation)
				Dim n As Integer = viewCount
				Dim endY As Integer = clip.y + clip.height
				For i As Integer = 0 To n - 1
					childRect.bounds = alloc
					childAllocation(i, childRect)
					If childRect.y < endY Then
						If (childRect.y + childRect.height) >= clip.y Then listPainter.paint(g, childRect.x, childRect.y, childRect.width, childRect.height, Me, i)
					Else
						Exit For
					End If
				Next i
			End If
		End Sub

		''' <summary>
		''' Paints one of the children; called by paint().  By default
		''' that is all it does, but a subclass can use this to paint
		''' things relative to the child.
		''' </summary>
		''' <param name="g"> the graphics context </param>
		''' <param name="alloc"> the allocated region to render the child into </param>
		''' <param name="index"> the index of the child </param>
		Protected Friend Overrides Sub paintChild(ByVal g As Graphics, ByVal alloc As Rectangle, ByVal index As Integer)
			listPainter.paint(g, alloc.x, alloc.y, alloc.width, alloc.height, Me, index)
			MyBase.paintChild(g, alloc, index)
		End Sub

		Protected Friend Overrides Sub setPropertiesFromAttributes()
			MyBase.propertiesFromAttributestes()
			listPainter = styleSheet.getListPainter(attributes)
		End Sub

		Private listPainter As StyleSheet.ListPainter
	End Class

End Namespace