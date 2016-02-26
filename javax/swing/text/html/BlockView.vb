Imports Microsoft.VisualBasic
Imports System
Imports javax.swing.border
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
Namespace javax.swing.text.html

	''' <summary>
	''' A view implementation to display a block (as a box)
	''' with CSS specifications.
	''' 
	''' @author  Timothy Prinzing
	''' </summary>
	Public Class BlockView
		Inherits BoxView

		''' <summary>
		''' Creates a new view that represents an
		''' html box.  This can be used for a number
		''' of elements.
		''' </summary>
		''' <param name="elem"> the element to create a view for </param>
		''' <param name="axis"> either View.X_AXIS or View.Y_AXIS </param>
		Public Sub New(ByVal elem As Element, ByVal axis As Integer)
			MyBase.New(elem, axis)
		End Sub

		''' <summary>
		''' Establishes the parent view for this view.  This is
		''' guaranteed to be called before any other methods if the
		''' parent view is functioning properly.
		''' <p>
		''' This is implemented
		''' to forward to the superclass as well as call the
		''' <seealso cref="#setPropertiesFromAttributes()"/>
		''' method to set the paragraph properties from the css
		''' attributes.  The call is made at this time to ensure
		''' the ability to resolve upward through the parents
		''' view attributes.
		''' </summary>
		''' <param name="parent"> the new parent, or null if the view is
		'''  being removed from a parent it was previously added
		'''  to </param>
		Public Overrides Property parent As View
			Set(ByVal parent As View)
				MyBase.parent = parent
				If parent IsNot Nothing Then propertiesFromAttributestes()
			End Set
		End Property

		''' <summary>
		''' Calculate the requirements of the block along the major
		''' axis (i.e. the axis along with it tiles).  This is implemented
		''' to provide the superclass behavior and then adjust it if the
		''' CSS width or height attribute is specified and applicable to
		''' the axis.
		''' </summary>
		Protected Friend Overrides Function calculateMajorAxisRequirements(ByVal axis As Integer, ByVal r As javax.swing.SizeRequirements) As javax.swing.SizeRequirements
			If r Is Nothing Then r = New javax.swing.SizeRequirements
			If Not spanSetFromAttributes(axis, r, cssWidth, cssHeight) Then
				r = MyBase.calculateMajorAxisRequirements(axis, r)
			Else
				' Offset by the margins so that pref/min/max return the
				' right value.
				Dim parentR As javax.swing.SizeRequirements = MyBase.calculateMajorAxisRequirements(axis, Nothing)
				Dim margin As Integer = If(axis = X_AXIS, leftInset + rightInset, topInset + bottomInset)
				r.minimum -= margin
				r.preferred -= margin
				r.maximum -= margin
				constrainSize(axis, r, parentR)
			End If
			Return r
		End Function

		''' <summary>
		''' Calculate the requirements of the block along the minor
		''' axis (i.e. the axis orthogonal to the axis along with it tiles).
		''' This is implemented
		''' to provide the superclass behavior and then adjust it if the
		''' CSS width or height attribute is specified and applicable to
		''' the axis.
		''' </summary>
		Protected Friend Overrides Function calculateMinorAxisRequirements(ByVal axis As Integer, ByVal r As javax.swing.SizeRequirements) As javax.swing.SizeRequirements
			If r Is Nothing Then r = New javax.swing.SizeRequirements

			If Not spanSetFromAttributes(axis, r, cssWidth, cssHeight) Then

	'            
	'             * The requirements were not directly specified by attributes, so
	'             * compute the aggregate of the requirements of the children.  The
	'             * children that have a percentage value specified will be treated
	'             * as completely stretchable since that child is not limited in any
	'             * way.
	'             
	'
	'            int min = 0;
	'            long pref = 0;
	'            int max = 0;
	'            int n = getViewCount();
	'            for (int i = 0; i < n; i++) {
	'                View v = getView(i);
	'                min = Math.max((int) v.getMinimumSpan(axis), min);
	'                pref = Math.max((int) v.getPreferredSpan(axis), pref);
	'                if (
	'                max = Math.max((int) v.getMaximumSpan(axis), max);
	'
	'            }
	'            r.preferred = (int) pref;
	'            r.minimum = min;
	'            r.maximum = max;
	'            
				r = MyBase.calculateMinorAxisRequirements(axis, r)
			Else
				' Offset by the margins so that pref/min/max return the
				' right value.
				Dim parentR As javax.swing.SizeRequirements = MyBase.calculateMinorAxisRequirements(axis, Nothing)
				Dim margin As Integer = If(axis = X_AXIS, leftInset + rightInset, topInset + bottomInset)
				r.minimum -= margin
				r.preferred -= margin
				r.maximum -= margin
				constrainSize(axis, r, parentR)
			End If

	'        
	'         * Set the alignment based upon the CSS properties if it is
	'         * specified.  For X_AXIS this would be text-align, for
	'         * Y_AXIS this would be vertical-align.
	'         
			If axis = X_AXIS Then
				Dim o As Object = attributes.getAttribute(CSS.Attribute.TEXT_ALIGN)
				If o IsNot Nothing Then
					Dim align As String = o.ToString()
					If align.Equals("center") Then
						r.alignment = 0.5f
					ElseIf align.Equals("right") Then
						r.alignment = 1.0f
					Else
						r.alignment = 0.0f
					End If
				End If
			End If
			' Y_AXIS TBD
			Return r
		End Function

		Friend Overridable Function isPercentage(ByVal axis As Integer, ByVal a As AttributeSet) As Boolean
			If axis = X_AXIS Then
				If cssWidth IsNot Nothing Then Return cssWidth.percentage
			Else
				If cssHeight IsNot Nothing Then Return cssHeight.percentage
			End If
			Return False
		End Function

		''' <summary>
		''' Adjust the given requirements to the CSS width or height if
		''' it is specified along the applicable axis.  Return true if the
		''' size is exactly specified, false if the span is not specified
		''' in an attribute or the size specified is a percentage.
		''' </summary>
		Friend Shared Function spanSetFromAttributes(ByVal axis As Integer, ByVal r As javax.swing.SizeRequirements, ByVal cssWidth As CSS.LengthValue, ByVal cssHeight As CSS.LengthValue) As Boolean
			If axis = X_AXIS Then
				If (cssWidth IsNot Nothing) AndAlso ((Not cssWidth.percentage)) Then
						r.maximum = CInt(Fix(cssWidth.value))
							r.preferred = r.maximum
							r.minimum = r.preferred
					Return True
				End If
			Else
				If (cssHeight IsNot Nothing) AndAlso ((Not cssHeight.percentage)) Then
						r.maximum = CInt(Fix(cssHeight.value))
							r.preferred = r.maximum
							r.minimum = r.preferred
					Return True
				End If
			End If
			Return False
		End Function

		''' <summary>
		''' Performs layout for the minor axis of the box (i.e. the
		''' axis orthogonal to the axis that it represents). The results
		''' of the layout (the offset and span for each children) are
		''' placed in the given arrays which represent the allocations to
		''' the children along the minor axis.
		''' </summary>
		''' <param name="targetSpan"> the total span given to the view, which
		'''  would be used to layout the children. </param>
		''' <param name="axis"> the axis being layed out </param>
		''' <param name="offsets"> the offsets from the origin of the view for
		'''  each of the child views; this is a return value and is
		'''  filled in by the implementation of this method </param>
		''' <param name="spans"> the span of each child view; this is a return
		'''  value and is filled in by the implementation of this method </param>
		Protected Friend Overrides Sub layoutMinorAxis(ByVal targetSpan As Integer, ByVal axis As Integer, ByVal offsets As Integer(), ByVal spans As Integer())
			Dim n As Integer = viewCount
			Dim key As Object = If(axis = X_AXIS, CSS.Attribute.WIDTH, CSS.Attribute.HEIGHT)
			For i As Integer = 0 To n - 1
				Dim v As View = getView(i)
				Dim min As Integer = CInt(Fix(v.getMinimumSpan(axis)))
				Dim max As Integer

				' check for percentage span
				Dim a As AttributeSet = v.attributes
				Dim lv As CSS.LengthValue = CType(a.getAttribute(key), CSS.LengthValue)
				If (lv IsNot Nothing) AndAlso lv.percentage Then
					' bound the span to the percentage specified
					min = Math.Max(CInt(Fix(lv.getValue(targetSpan))), min)
					max = min
				Else
					max = CInt(Fix(v.getMaximumSpan(axis)))
				End If

				' assign the offset and span for the child
				If max < targetSpan Then
					' can't make the child this wide, align it
					Dim align As Single = v.getAlignment(axis)
					offsets(i) = CInt(Fix((targetSpan - max) * align))
					spans(i) = max
				Else
					' make it the target width, or as small as it can get.
					offsets(i) = 0
					spans(i) = Math.Max(min, targetSpan)
				End If
			Next i
		End Sub


		''' <summary>
		''' Renders using the given rendering surface and area on that
		''' surface.  This is implemented to delegate to the css box
		''' painter to paint the border and background prior to the
		''' interior.
		''' </summary>
		''' <param name="g"> the rendering surface to use </param>
		''' <param name="allocation"> the allocated region to render into </param>
		''' <seealso cref= View#paint </seealso>
		Public Overrides Sub paint(ByVal g As Graphics, ByVal allocation As Shape)
			Dim a As Rectangle = CType(allocation, Rectangle)
			painter.paint(g, a.x, a.y, a.width, a.height, Me)
			MyBase.paint(g, a)
		End Sub

		''' <summary>
		''' Fetches the attributes to use when rendering.  This is
		''' implemented to multiplex the attributes specified in the
		''' model with a StyleSheet.
		''' </summary>
		Public Property Overrides attributes As AttributeSet
			Get
				If attr Is Nothing Then
					Dim sheet As StyleSheet = styleSheet
					attr = sheet.getViewAttributes(Me)
				End If
				Return attr
			End Get
		End Property

		''' <summary>
		''' Gets the resize weight.
		''' </summary>
		''' <param name="axis"> may be either X_AXIS or Y_AXIS </param>
		''' <returns> the weight </returns>
		''' <exception cref="IllegalArgumentException"> for an invalid axis </exception>
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
		''' Gets the alignment.
		''' </summary>
		''' <param name="axis"> may be either X_AXIS or Y_AXIS </param>
		''' <returns> the alignment </returns>
		Public Overrides Function getAlignment(ByVal axis As Integer) As Single
			Select Case axis
			Case View.X_AXIS
				Return 0
			Case View.Y_AXIS
				If viewCount = 0 Then Return 0
				Dim ___span As Single = getPreferredSpan(View.Y_AXIS)
				Dim v As View = getView(0)
				Dim above As Single = v.getPreferredSpan(View.Y_AXIS)
				Dim a As Single = If((CInt(Fix(___span))) <> 0, (above * v.getAlignment(View.Y_AXIS)) / ___span, 0)
				Return a
			Case Else
				Throw New System.ArgumentException("Invalid axis: " & axis)
			End Select
		End Function

		Public Overridable Sub changedUpdate(ByVal changes As javax.swing.event.DocumentEvent, ByVal a As Shape, ByVal f As ViewFactory)
			MyBase.changedUpdate(changes, a, f)
			Dim pos As Integer = changes.offset
			If pos <= startOffset AndAlso (pos + changes.length) >= endOffset Then propertiesFromAttributestes()
		End Sub

		''' <summary>
		''' Determines the preferred span for this view along an
		''' axis.
		''' </summary>
		''' <param name="axis"> may be either <code>View.X_AXIS</code>
		'''           or <code>View.Y_AXIS</code> </param>
		''' <returns>   the span the view would like to be rendered into &gt;= 0;
		'''           typically the view is told to render into the span
		'''           that is returned, although there is no guarantee;
		'''           the parent may choose to resize or break the view </returns>
		''' <exception cref="IllegalArgumentException"> for an invalid axis type </exception>
		Public Overrides Function getPreferredSpan(ByVal axis As Integer) As Single
			Return MyBase.getPreferredSpan(axis)
		End Function

		''' <summary>
		''' Determines the minimum span for this view along an
		''' axis.
		''' </summary>
		''' <param name="axis"> may be either <code>View.X_AXIS</code>
		'''           or <code>View.Y_AXIS</code> </param>
		''' <returns>  the span the view would like to be rendered into &gt;= 0;
		'''           typically the view is told to render into the span
		'''           that is returned, although there is no guarantee;
		'''           the parent may choose to resize or break the view </returns>
		''' <exception cref="IllegalArgumentException"> for an invalid axis type </exception>
		Public Overrides Function getMinimumSpan(ByVal axis As Integer) As Single
			Return MyBase.getMinimumSpan(axis)
		End Function

		''' <summary>
		''' Determines the maximum span for this view along an
		''' axis.
		''' </summary>
		''' <param name="axis"> may be either <code>View.X_AXIS</code>
		'''           or <code>View.Y_AXIS</code> </param>
		''' <returns>   the span the view would like to be rendered into &gt;= 0;
		'''           typically the view is told to render into the span
		'''           that is returned, although there is no guarantee;
		'''           the parent may choose to resize or break the view </returns>
		''' <exception cref="IllegalArgumentException"> for an invalid axis type </exception>
		Public Overrides Function getMaximumSpan(ByVal axis As Integer) As Single
			Return MyBase.getMaximumSpan(axis)
		End Function

		''' <summary>
		''' Update any cached values that come from attributes.
		''' </summary>
		Protected Friend Overridable Sub setPropertiesFromAttributes()

			' update attributes
			Dim sheet As StyleSheet = styleSheet
			attr = sheet.getViewAttributes(Me)

			' Reset the painter
			painter = sheet.getBoxPainter(attr)
			If attr IsNot Nothing Then insetsets(CShort(Fix(painter.getInset(TOP, Me))), CShort(Fix(painter.getInset(LEFT, Me))), CShort(Fix(painter.getInset(BOTTOM, Me))), CShort(Fix(painter.getInset(RIGHT, Me))))

			' Get the width/height
			cssWidth = CType(attr.getAttribute(CSS.Attribute.WIDTH), CSS.LengthValue)
			cssHeight = CType(attr.getAttribute(CSS.Attribute.HEIGHT), CSS.LengthValue)
		End Sub

		Protected Friend Overridable Property styleSheet As StyleSheet
			Get
				Dim doc As HTMLDocument = CType(document, HTMLDocument)
				Return doc.styleSheet
			End Get
		End Property

		''' <summary>
		''' Constrains <code>want</code> to fit in the minimum size specified
		''' by <code>min</code>.
		''' </summary>
		Private Sub constrainSize(ByVal axis As Integer, ByVal want As javax.swing.SizeRequirements, ByVal min As javax.swing.SizeRequirements)
			If min.minimum > want.minimum Then
					want.preferred = min.minimum
					want.minimum = want.preferred
				want.maximum = Math.Max(want.maximum, min.maximum)
			End If
		End Sub

		Private attr As AttributeSet
		Private painter As StyleSheet.BoxPainter

		Private cssWidth As CSS.LengthValue
		Private cssHeight As CSS.LengthValue

	End Class

End Namespace