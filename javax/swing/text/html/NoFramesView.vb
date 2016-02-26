Imports javax.swing.text

'
' * Copyright (c) 1998, 2000, Oracle and/or its affiliates. All rights reserved.
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
	''' This is the view associated with the html tag NOFRAMES.
	''' This view has been written to ignore the contents of the
	''' NOFRAMES tag.  The contents of the tag will only be visible
	''' when the JTextComponent the view is contained in is editable.
	''' 
	''' @author  Sunita Mani
	''' </summary>
	Friend Class NoFramesView
		Inherits BlockView

		''' <summary>
		''' Creates a new view that represents an
		''' html box.  This can be used for a number
		''' of elements.  By default this view is not
		''' visible.
		''' </summary>
		''' <param name="elem"> the element to create a view for </param>
		''' <param name="axis"> either View.X_AXIS or View.Y_AXIS </param>
		Public Sub New(ByVal elem As Element, ByVal axis As Integer)
			MyBase.New(elem, axis)
			visible = False
		End Sub


		''' <summary>
		''' If this view is not visible, then it returns.
		''' Otherwise it invokes the superclass.
		''' </summary>
		''' <param name="g"> the rendering surface to use </param>
		''' <param name="allocation"> the allocated region to render into </param>
		''' <seealso cref= #isVisible </seealso>
		''' <seealso cref= text.ParagraphView#paint </seealso>
		Public Overrides Sub paint(ByVal g As Graphics, ByVal allocation As Shape)
			Dim host As Container = container
			If host IsNot Nothing AndAlso visible <> CType(host, JTextComponent).editable Then visible = CType(host, JTextComponent).editable

			If Not visible Then Return
			MyBase.paint(g, allocation)
		End Sub


		''' <summary>
		''' Determines if the JTextComponent that the view
		''' is contained in is editable. If so, then this
		''' view and all its child views are visible.
		''' Once this has been determined, the superclass
		''' is invoked to continue processing.
		''' </summary>
		''' <param name="p"> the parent View. </param>
		''' <seealso cref= BlockView#setParent </seealso>
		Public Overrides Property parent As View
			Set(ByVal p As View)
				If p IsNot Nothing Then
					Dim host As Container = p.container
					If host IsNot Nothing Then visible = CType(host, JTextComponent).editable
				End If
				MyBase.parent = p
			End Set
		End Property

		''' <summary>
		''' Returns a true/false value that represents
		''' whether the view is visible or not.
		''' </summary>
		Public Property Overrides visible As Boolean
			Get
				Return visible
			End Get
		End Property


		''' <summary>
		''' Do nothing if the view is not visible, otherwise
		''' invoke the superclass to perform layout.
		''' </summary>
		Protected Friend Overrides Sub layout(ByVal width As Integer, ByVal height As Integer)
			If Not visible Then Return
			MyBase.layout(width, height)
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
		''' <seealso cref= text.ParagraphView#getPreferredSpan </seealso>
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
		'''          <code>View.Y_AXIS</code> </param>
		''' <returns>  the minimum span the view can be rendered into </returns>
		''' <seealso cref= text.ParagraphView#getMinimumSpan </seealso>
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
		''' <seealso cref= text.ParagraphView#getMaximumSpan </seealso>
		Public Overrides Function getMaximumSpan(ByVal axis As Integer) As Single
			If Not visible Then Return 0
			Return MyBase.getMaximumSpan(axis)
		End Function

		Friend visible As Boolean
	End Class

End Namespace