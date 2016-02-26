Imports javax.swing.text

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
	''' Displays the <dfn>inline element</dfn> styles
	''' based upon css attributes.
	''' 
	''' @author  Timothy Prinzing
	''' </summary>
	Public Class InlineView
		Inherits LabelView

		''' <summary>
		''' Constructs a new view wrapped on an element.
		''' </summary>
		''' <param name="elem"> the element </param>
		Public Sub New(ByVal elem As Element)
			MyBase.New(elem)
			Dim sheet As StyleSheet = styleSheet
			attr = sheet.getViewAttributes(Me)
		End Sub

		''' <summary>
		''' Gives notification that something was inserted into
		''' the document in a location that this view is responsible for.
		''' If either parameter is <code>null</code>, behavior of this method is
		''' implementation dependent.
		''' </summary>
		''' <param name="e"> the change information from the associated document </param>
		''' <param name="a"> the current allocation of the view </param>
		''' <param name="f"> the factory to use to rebuild if the view has children
		''' @since 1.5 </param>
		''' <seealso cref= View#insertUpdate </seealso>
		Public Overridable Sub insertUpdate(ByVal e As javax.swing.event.DocumentEvent, ByVal a As Shape, ByVal f As ViewFactory)
			MyBase.insertUpdate(e, a, f)
		End Sub

		''' <summary>
		''' Gives notification that something was removed from the document
		''' in a location that this view is responsible for.
		''' If either parameter is <code>null</code>, behavior of this method is
		''' implementation dependent.
		''' </summary>
		''' <param name="e"> the change information from the associated document </param>
		''' <param name="a"> the current allocation of the view </param>
		''' <param name="f"> the factory to use to rebuild if the view has children
		''' @since 1.5 </param>
		''' <seealso cref= View#removeUpdate </seealso>
		Public Overridable Sub removeUpdate(ByVal e As javax.swing.event.DocumentEvent, ByVal a As Shape, ByVal f As ViewFactory)
			MyBase.removeUpdate(e, a, f)
		End Sub

		''' <summary>
		''' Gives notification from the document that attributes were changed
		''' in a location that this view is responsible for.
		''' </summary>
		''' <param name="e"> the change information from the associated document </param>
		''' <param name="a"> the current allocation of the view </param>
		''' <param name="f"> the factory to use to rebuild if the view has children </param>
		''' <seealso cref= View#changedUpdate </seealso>
		Public Overridable Sub changedUpdate(ByVal e As javax.swing.event.DocumentEvent, ByVal a As Shape, ByVal f As ViewFactory)
			MyBase.changedUpdate(e, a, f)
			Dim sheet As StyleSheet = styleSheet
			attr = sheet.getViewAttributes(Me)
			preferenceChanged(Nothing, True, True)
		End Sub

		''' <summary>
		''' Fetches the attributes to use when rendering.  This is
		''' implemented to multiplex the attributes specified in the
		''' model with a StyleSheet.
		''' </summary>
		Public Property Overrides attributes As AttributeSet
			Get
				Return attr
			End Get
		End Property

		''' <summary>
		''' Determines how attractive a break opportunity in
		''' this view is.  This can be used for determining which
		''' view is the most attractive to call <code>breakView</code>
		''' on in the process of formatting.  A view that represents
		''' text that has whitespace in it might be more attractive
		''' than a view that has no whitespace, for example.  The
		''' higher the weight, the more attractive the break.  A
		''' value equal to or lower than <code>BadBreakWeight</code>
		''' should not be considered for a break.  A value greater
		''' than or equal to <code>ForcedBreakWeight</code> should
		''' be broken.
		''' <p>
		''' This is implemented to provide the default behavior
		''' of returning <code>BadBreakWeight</code> unless the length
		''' is greater than the length of the view in which case the
		''' entire view represents the fragment.  Unless a view has
		''' been written to support breaking behavior, it is not
		''' attractive to try and break the view.  An example of
		''' a view that does support breaking is <code>LabelView</code>.
		''' An example of a view that uses break weight is
		''' <code>ParagraphView</code>.
		''' </summary>
		''' <param name="axis"> may be either View.X_AXIS or View.Y_AXIS </param>
		''' <param name="pos"> the potential location of the start of the
		'''   broken view &gt;= 0.  This may be useful for calculating tab
		'''   positions. </param>
		''' <param name="len"> specifies the relative length from <em>pos</em>
		'''   where a potential break is desired &gt;= 0. </param>
		''' <returns> the weight, which should be a value between
		'''   ForcedBreakWeight and BadBreakWeight. </returns>
		''' <seealso cref= LabelView </seealso>
		''' <seealso cref= ParagraphView </seealso>
		''' <seealso cref= javax.swing.text.View#BadBreakWeight </seealso>
		''' <seealso cref= javax.swing.text.View#GoodBreakWeight </seealso>
		''' <seealso cref= javax.swing.text.View#ExcellentBreakWeight </seealso>
		''' <seealso cref= javax.swing.text.View#ForcedBreakWeight </seealso>
		Public Overrides Function getBreakWeight(ByVal axis As Integer, ByVal pos As Single, ByVal len As Single) As Integer
			If nowrap Then Return BadBreakWeight
			Return MyBase.getBreakWeight(axis, pos, len)
		End Function

		''' <summary>
		''' Tries to break this view on the given axis. Refer to
		''' <seealso cref="javax.swing.text.View#breakView"/> for a complete
		''' description of this method.
		''' <p>Behavior of this method is unspecified in case <code>axis</code>
		''' is neither <code>View.X_AXIS</code> nor <code>View.Y_AXIS</code>, and
		''' in case <code>offset</code>, <code>pos</code>, or <code>len</code>
		''' is null.
		''' </summary>
		''' <param name="axis"> may be either <code>View.X_AXIS</code> or
		'''          <code>View.Y_AXIS</code> </param>
		''' <param name="offset"> the location in the document model
		'''   that a broken fragment would occupy &gt;= 0.  This
		'''   would be the starting offset of the fragment
		'''   returned </param>
		''' <param name="pos"> the position along the axis that the
		'''  broken view would occupy &gt;= 0.  This may be useful for
		'''  things like tab calculations </param>
		''' <param name="len"> specifies the distance along the axis
		'''  where a potential break is desired &gt;= 0 </param>
		''' <returns> the fragment of the view that represents the
		'''  given span.
		''' @since 1.5 </returns>
		''' <seealso cref= javax.swing.text.View#breakView </seealso>
		Public Overrides Function breakView(ByVal axis As Integer, ByVal offset As Integer, ByVal pos As Single, ByVal len As Single) As View
			Return MyBase.breakView(axis, offset, pos, len)
		End Function


		''' <summary>
		''' Set the cached properties from the attributes.
		''' </summary>
		Protected Friend Overrides Sub setPropertiesFromAttributes()
			MyBase.propertiesFromAttributestes()
			Dim a As AttributeSet = attributes
			Dim decor As Object = a.getAttribute(CSS.Attribute.TEXT_DECORATION)
			Dim u As Boolean = If(decor IsNot Nothing, (decor.ToString().IndexOf("underline") >= 0), False)
			underline = u
			Dim s As Boolean = If(decor IsNot Nothing, (decor.ToString().IndexOf("line-through") >= 0), False)
			strikeThrough = s
			Dim vAlign As Object = a.getAttribute(CSS.Attribute.VERTICAL_ALIGN)
			s = If(vAlign IsNot Nothing, (vAlign.ToString().IndexOf("sup") >= 0), False)
			superscript = s
			s = If(vAlign IsNot Nothing, (vAlign.ToString().IndexOf("sub") >= 0), False)
			subscript = s

			Dim whitespace As Object = a.getAttribute(CSS.Attribute.WHITE_SPACE)
			If (whitespace IsNot Nothing) AndAlso whitespace.Equals("nowrap") Then
				nowrap = True
			Else
				nowrap = False
			End If

			Dim doc As HTMLDocument = CType(document, HTMLDocument)
			' fetches background color from stylesheet if specified
			Dim bg As Color = doc.getBackground(a)
			If bg IsNot Nothing Then background = bg
		End Sub


		Protected Friend Overridable Property styleSheet As StyleSheet
			Get
				Dim doc As HTMLDocument = CType(document, HTMLDocument)
				Return doc.styleSheet
			End Get
		End Property

		Private nowrap As Boolean
		Private attr As AttributeSet
	End Class

End Namespace