Imports Microsoft.VisualBasic

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
Namespace javax.swing.border


	''' <summary>
	''' A class which implements an arbitrary border
	''' with the addition of a String title in a
	''' specified position and justification.
	''' <p>
	''' If the border, font, or color property values are not
	''' specified in the constructor or by invoking the appropriate
	''' set methods, the property values will be defined by the current
	''' look and feel, using the following property names in the
	''' Defaults Table:
	''' <ul>
	''' <li>&quot;TitledBorder.border&quot;
	''' <li>&quot;TitledBorder.font&quot;
	''' <li>&quot;TitledBorder.titleColor&quot;
	''' </ul>
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
	''' @author David Kloba
	''' @author Amy Fowler
	''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
	Public Class TitledBorder
		Inherits AbstractBorder

		Protected Friend title As String
		Protected Friend border As Border
		Protected Friend titlePosition As Integer
		Protected Friend titleJustification As Integer
		Protected Friend titleFont As java.awt.Font
		Protected Friend titleColor As java.awt.Color

		Private ReadOnly label As javax.swing.JLabel

		''' <summary>
		''' Use the default vertical orientation for the title text.
		''' </summary>
		Public Const DEFAULT_POSITION As Integer = 0
		''' <summary>
		''' Position the title above the border's top line. </summary>
		Public Const ABOVE_TOP As Integer = 1
		''' <summary>
		''' Position the title in the middle of the border's top line. </summary>
		Public Const TOP As Integer = 2
		''' <summary>
		''' Position the title below the border's top line. </summary>
		Public Const BELOW_TOP As Integer = 3
		''' <summary>
		''' Position the title above the border's bottom line. </summary>
		Public Const ABOVE_BOTTOM As Integer = 4
		''' <summary>
		''' Position the title in the middle of the border's bottom line. </summary>
		Public Const BOTTOM As Integer = 5
		''' <summary>
		''' Position the title below the border's bottom line. </summary>
		Public Const BELOW_BOTTOM As Integer = 6

		''' <summary>
		''' Use the default justification for the title text.
		''' </summary>
		Public Const DEFAULT_JUSTIFICATION As Integer = 0
		''' <summary>
		''' Position title text at the left side of the border line. </summary>
		Public Const LEFT As Integer = 1
		''' <summary>
		''' Position title text in the center of the border line. </summary>
		Public Const CENTER As Integer = 2
		''' <summary>
		''' Position title text at the right side of the border line. </summary>
		Public Const RIGHT As Integer = 3
		''' <summary>
		''' Position title text at the left side of the border line
		'''  for left to right orientation, at the right side of the
		'''  border line for right to left orientation.
		''' </summary>
		Public Const LEADING As Integer = 4
		''' <summary>
		''' Position title text at the right side of the border line
		'''  for left to right orientation, at the left side of the
		'''  border line for right to left orientation.
		''' </summary>
		Public Const TRAILING As Integer = 5

		' Space between the border and the component's edge
		Protected Friend Const EDGE_SPACING As Integer = 2

		' Space between the border and text
		Protected Friend Const TEXT_SPACING As Integer = 2

		' Horizontal inset of text that is left or right justified
		Protected Friend Const TEXT_INSET_H As Integer = 5

		''' <summary>
		''' Creates a TitledBorder instance.
		''' </summary>
		''' <param name="title">  the title the border should display </param>
		Public Sub New(ByVal title As String)
			Me.New(Nothing, title, LEADING, DEFAULT_POSITION, Nothing, Nothing)
		End Sub

		''' <summary>
		''' Creates a TitledBorder instance with the specified border
		''' and an empty title.
		''' </summary>
		''' <param name="border">  the border </param>
		Public Sub New(ByVal border As Border)
			Me.New(border, "", LEADING, DEFAULT_POSITION, Nothing, Nothing)
		End Sub

		''' <summary>
		''' Creates a TitledBorder instance with the specified border
		''' and title.
		''' </summary>
		''' <param name="border">  the border </param>
		''' <param name="title">  the title the border should display </param>
		Public Sub New(ByVal border As Border, ByVal title As String)
			Me.New(border, title, LEADING, DEFAULT_POSITION, Nothing, Nothing)
		End Sub

		''' <summary>
		''' Creates a TitledBorder instance with the specified border,
		''' title, title-justification, and title-position.
		''' </summary>
		''' <param name="border">  the border </param>
		''' <param name="title">  the title the border should display </param>
		''' <param name="titleJustification"> the justification for the title </param>
		''' <param name="titlePosition"> the position for the title </param>
		Public Sub New(ByVal border As Border, ByVal title As String, ByVal titleJustification As Integer, ByVal titlePosition As Integer)
			Me.New(border, title, titleJustification, titlePosition, Nothing, Nothing)
		End Sub

		''' <summary>
		''' Creates a TitledBorder instance with the specified border,
		''' title, title-justification, title-position, and title-font.
		''' </summary>
		''' <param name="border">  the border </param>
		''' <param name="title">  the title the border should display </param>
		''' <param name="titleJustification"> the justification for the title </param>
		''' <param name="titlePosition"> the position for the title </param>
		''' <param name="titleFont"> the font for rendering the title </param>
		Public Sub New(ByVal border As Border, ByVal title As String, ByVal titleJustification As Integer, ByVal titlePosition As Integer, ByVal titleFont As java.awt.Font)
			Me.New(border, title, titleJustification, titlePosition, titleFont, Nothing)
		End Sub

		''' <summary>
		''' Creates a TitledBorder instance with the specified border,
		''' title, title-justification, title-position, title-font, and
		''' title-color.
		''' </summary>
		''' <param name="border">  the border </param>
		''' <param name="title">  the title the border should display </param>
		''' <param name="titleJustification"> the justification for the title </param>
		''' <param name="titlePosition"> the position for the title </param>
		''' <param name="titleFont"> the font of the title </param>
		''' <param name="titleColor"> the color of the title </param>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
		Public Sub New(ByVal border As Border, ByVal title As String, ByVal titleJustification As Integer, ByVal titlePosition As Integer, ByVal titleFont As java.awt.Font, ByVal titleColor As java.awt.Color)
			Me.title = title
			Me.border = border
			Me.titleFont = titleFont
			Me.titleColor = titleColor

			titleJustification = titleJustification
			titlePosition = titlePosition

			Me.label = New javax.swing.JLabel
			Me.label.opaque = False
			Me.label.putClientProperty(javax.swing.plaf.basic.BasicHTML.propertyKey, Nothing)
		End Sub

		''' <summary>
		''' Paints the border for the specified component with the
		''' specified position and size. </summary>
		''' <param name="c"> the component for which this border is being painted </param>
		''' <param name="g"> the paint graphics </param>
		''' <param name="x"> the x position of the painted border </param>
		''' <param name="y"> the y position of the painted border </param>
		''' <param name="width"> the width of the painted border </param>
		''' <param name="height"> the height of the painted border </param>
		Public Overrides Sub paintBorder(ByVal c As java.awt.Component, ByVal g As java.awt.Graphics, ByVal x As Integer, ByVal y As Integer, ByVal width As Integer, ByVal height As Integer)
			Dim ___border As Border = border
			Dim ___title As String = title
			If (___title IsNot Nothing) AndAlso ___title.Length > 0 Then
				Dim edge As Integer = If(TypeOf ___border Is TitledBorder, 0, EDGE_SPACING)
				Dim ___label As javax.swing.JLabel = getLabel(c)
				Dim size As java.awt.Dimension = ___label.preferredSize
				Dim insets As java.awt.Insets = getBorderInsets(___border, c, New java.awt.Insets(0, 0, 0, 0))

				Dim borderX As Integer = x + edge
				Dim borderY As Integer = y + edge
				Dim borderW As Integer = width - edge - edge
				Dim borderH As Integer = height - edge - edge

				Dim labelY As Integer = y
				Dim labelH As Integer = size.height
				Dim ___position As Integer = position
				Select Case ___position
					Case ABOVE_TOP
						insets.left = 0
						insets.right = 0
						borderY += labelH - edge
						borderH -= labelH - edge
					Case TOP
						insets.top = edge + insets.top/2 - labelH\2
						If insets.top < edge Then
							borderY -= insets.top
							borderH += insets.top
						Else
							labelY += insets.top
						End If
					Case BELOW_TOP
						labelY += insets.top + edge
					Case ABOVE_BOTTOM
						labelY += height - labelH - insets.bottom - edge
					Case BOTTOM
						labelY += height - labelH
						insets.bottom = edge + (insets.bottom - labelH) / 2
						If insets.bottom < edge Then
							borderH += insets.bottom
						Else
							labelY -= insets.bottom
						End If
					Case BELOW_BOTTOM
						insets.left = 0
						insets.right = 0
						labelY += height - labelH
						borderH -= labelH - edge
				End Select
				insets.left += edge + TEXT_INSET_H
				insets.right += edge + TEXT_INSET_H

				Dim labelX As Integer = x
				Dim labelW As Integer = width - insets.left - insets.right
				If labelW > size.width Then labelW = size.width
				Select Case getJustification(c)
					Case LEFT
						labelX += insets.left
					Case RIGHT
						labelX += width - insets.right - labelW
					Case CENTER
						labelX += (width - labelW) \ 2
				End Select

				If ___border IsNot Nothing Then
					If (___position <> TOP) AndAlso (___position <> BOTTOM) Then
						___border.paintBorder(c, g, borderX, borderY, borderW, borderH)
					Else
						Dim g2 As java.awt.Graphics = g.create()
						If TypeOf g2 Is java.awt.Graphics2D Then
							Dim g2d As java.awt.Graphics2D = CType(g2, java.awt.Graphics2D)
							Dim path As java.awt.geom.Path2D = New java.awt.geom.Path2D.Float
							path.append(New java.awt.Rectangle(borderX, borderY, borderW, labelY - borderY), False)
							path.append(New java.awt.Rectangle(borderX, labelY, labelX - borderX - TEXT_SPACING, labelH), False)
							path.append(New java.awt.Rectangle(labelX + labelW + TEXT_SPACING, labelY, borderX - labelX + borderW - labelW - TEXT_SPACING, labelH), False)
							path.append(New java.awt.Rectangle(borderX, labelY + labelH, borderW, borderY - labelY + borderH - labelH), False)
							g2d.clip(path)
						End If
						___border.paintBorder(c, g2, borderX, borderY, borderW, borderH)
						g2.Dispose()
					End If
				End If
				g.translate(labelX, labelY)
				___label.sizeize(labelW, labelH)
				___label.paint(g)
				g.translate(-labelX, -labelY)
			ElseIf ___border IsNot Nothing Then
				___border.paintBorder(c, g, x, y, width, height)
			End If
		End Sub

		''' <summary>
		''' Reinitialize the insets parameter with this Border's current Insets. </summary>
		''' <param name="c"> the component for which this border insets value applies </param>
		''' <param name="insets"> the object to be reinitialized </param>
		Public Overrides Function getBorderInsets(ByVal c As java.awt.Component, ByVal insets As java.awt.Insets) As java.awt.Insets
			Dim ___border As Border = border
			insets = getBorderInsets(___border, c, insets)

			Dim ___title As String = title
			If (___title IsNot Nothing) AndAlso ___title.Length > 0 Then
				Dim edge As Integer = If(TypeOf ___border Is TitledBorder, 0, EDGE_SPACING)
				Dim ___label As javax.swing.JLabel = getLabel(c)
				Dim size As java.awt.Dimension = ___label.preferredSize

				Select Case position
					Case ABOVE_TOP
						insets.top += size.height - edge
					Case TOP
						If insets.top < size.height Then insets.top = size.height - edge
						Exit Select
					Case BELOW_TOP
						insets.top += size.height
					Case ABOVE_BOTTOM
						insets.bottom += size.height
					Case BOTTOM
						If insets.bottom < size.height Then insets.bottom = size.height - edge
						Exit Select
					Case BELOW_BOTTOM
						insets.bottom += size.height - edge
				End Select
				insets.top += edge + TEXT_SPACING
				insets.left += edge + TEXT_SPACING
				insets.right += edge + TEXT_SPACING
				insets.bottom += edge + TEXT_SPACING
			End If
			Return insets
		End Function

		''' <summary>
		''' Returns whether or not the border is opaque.
		''' </summary>
		Public Property Overrides borderOpaque As Boolean
			Get
				Return False
			End Get
		End Property

		''' <summary>
		''' Returns the title of the titled border.
		''' </summary>
		''' <returns> the title of the titled border </returns>
		Public Overridable Property title As String
			Get
				Return title
			End Get
			Set(ByVal title As String)
				Me.title = title
			End Set
		End Property

		''' <summary>
		''' Returns the border of the titled border.
		''' </summary>
		''' <returns> the border of the titled border </returns>
		Public Overridable Property border As Border
			Get
				Return If(border IsNot Nothing, border, javax.swing.UIManager.getBorder("TitledBorder.border"))
			End Get
			Set(ByVal border As Border)
				Me.border = border
			End Set
		End Property

		''' <summary>
		''' Returns the title-position of the titled border.
		''' </summary>
		''' <returns> the title-position of the titled border </returns>
		Public Overridable Property titlePosition As Integer
			Get
				Return titlePosition
			End Get
			Set(ByVal titlePosition As Integer)
				Select Case titlePosition
					Case ABOVE_TOP, TOP, BELOW_TOP, ABOVE_BOTTOM, BOTTOM, BELOW_BOTTOM, DEFAULT_POSITION
						Me.titlePosition = titlePosition
					Case Else
						Throw New System.ArgumentException(titlePosition & " is not a valid title position.")
				End Select
			End Set
		End Property

		''' <summary>
		''' Returns the title-justification of the titled border.
		''' </summary>
		''' <returns> the title-justification of the titled border </returns>
		Public Overridable Property titleJustification As Integer
			Get
				Return titleJustification
			End Get
			Set(ByVal titleJustification As Integer)
				Select Case titleJustification
					Case DEFAULT_JUSTIFICATION, LEFT, CENTER, RIGHT, LEADING, TRAILING
						Me.titleJustification = titleJustification
					Case Else
						Throw New System.ArgumentException(titleJustification & " is not a valid title justification.")
				End Select
			End Set
		End Property

		''' <summary>
		''' Returns the title-font of the titled border.
		''' </summary>
		''' <returns> the title-font of the titled border </returns>
		Public Overridable Property titleFont As java.awt.Font
			Get
				Return If(titleFont Is Nothing, javax.swing.UIManager.getFont("TitledBorder.font"), titleFont)
			End Get
			Set(ByVal titleFont As java.awt.Font)
				Me.titleFont = titleFont
			End Set
		End Property

		''' <summary>
		''' Returns the title-color of the titled border.
		''' </summary>
		''' <returns> the title-color of the titled border </returns>
		Public Overridable Property titleColor As java.awt.Color
			Get
				Return If(titleColor Is Nothing, javax.swing.UIManager.getColor("TitledBorder.titleColor"), titleColor)
			End Get
			Set(ByVal titleColor As java.awt.Color)
				Me.titleColor = titleColor
			End Set
		End Property


		' REMIND(aim): remove all or some of these set methods?







		''' <summary>
		''' Returns the minimum dimensions this border requires
		''' in order to fully display the border and title. </summary>
		''' <param name="c"> the component where this border will be drawn </param>
		''' <returns> the {@code Dimension} object </returns>
		Public Overridable Function getMinimumSize(ByVal c As java.awt.Component) As java.awt.Dimension
			Dim insets As java.awt.Insets = getBorderInsets(c)
			Dim minSize As New java.awt.Dimension(insets.right+insets.left, insets.top+insets.bottom)
			Dim ___title As String = title
			If (___title IsNot Nothing) AndAlso ___title.Length > 0 Then
				Dim ___label As javax.swing.JLabel = getLabel(c)
				Dim size As java.awt.Dimension = ___label.preferredSize

				Dim ___position As Integer = position
				If (___position <> ABOVE_TOP) AndAlso (___position <> BELOW_BOTTOM) Then
					minSize.width += size.width
				ElseIf minSize.width < size.width Then
					minSize.width += size.width
				End If
			End If
			Return minSize
		End Function

		''' <summary>
		''' Returns the baseline.
		''' </summary>
		''' <exception cref="NullPointerException"> {@inheritDoc} </exception>
		''' <exception cref="IllegalArgumentException"> {@inheritDoc} </exception>
		''' <seealso cref= javax.swing.JComponent#getBaseline(int, int)
		''' @since 1.6 </seealso>
		Public Overrides Function getBaseline(ByVal c As java.awt.Component, ByVal width As Integer, ByVal height As Integer) As Integer
			If c Is Nothing Then Throw New NullPointerException("Must supply non-null component")
			If width < 0 Then Throw New System.ArgumentException("Width must be >= 0")
			If height < 0 Then Throw New System.ArgumentException("Height must be >= 0")
			Dim ___border As Border = border
			Dim ___title As String = title
			If (___title IsNot Nothing) AndAlso ___title.Length > 0 Then
				Dim edge As Integer = If(TypeOf ___border Is TitledBorder, 0, EDGE_SPACING)
				Dim ___label As javax.swing.JLabel = getLabel(c)
				Dim size As java.awt.Dimension = ___label.preferredSize
				Dim insets As java.awt.Insets = getBorderInsets(___border, c, New java.awt.Insets(0, 0, 0, 0))

				Dim ___baseline As Integer = ___label.getBaseline(size.width, size.height)
				Select Case position
					Case ABOVE_TOP
						Return ___baseline
					Case TOP
						insets.top = edge + (insets.top - size.height) / 2
						Return If(insets.top < edge, ___baseline, ___baseline + insets.top)
					Case BELOW_TOP
						Return ___baseline + insets.top + edge
					Case ABOVE_BOTTOM
						Return ___baseline + height - size.height - insets.bottom - edge
					Case BOTTOM
						insets.bottom = edge + (insets.bottom - size.height) / 2
						Return If(insets.bottom < edge, ___baseline + height - size.height, ___baseline + height - size.height + insets.bottom)
					Case BELOW_BOTTOM
						Return ___baseline + height - size.height
				End Select
			End If
			Return -1
		End Function

		''' <summary>
		''' Returns an enum indicating how the baseline of the border
		''' changes as the size changes.
		''' </summary>
		''' <exception cref="NullPointerException"> {@inheritDoc} </exception>
		''' <seealso cref= javax.swing.JComponent#getBaseline(int, int)
		''' @since 1.6 </seealso>
		Public Overrides Function getBaselineResizeBehavior(ByVal c As java.awt.Component) As java.awt.Component.BaselineResizeBehavior
			MyBase.getBaselineResizeBehavior(c)
			Select Case position
				Case TitledBorder.ABOVE_TOP, TitledBorder.TOP, TitledBorder.BELOW_TOP
					Return java.awt.Component.BaselineResizeBehavior.CONSTANT_ASCENT
				Case TitledBorder.ABOVE_BOTTOM, TitledBorder.BOTTOM, TitledBorder.BELOW_BOTTOM
					Return javax.swing.JComponent.BaselineResizeBehavior.CONSTANT_DESCENT
			End Select
			Return java.awt.Component.BaselineResizeBehavior.OTHER
		End Function

		Private Property position As Integer
			Get
				Dim ___position As Integer = titlePosition
				If ___position <> DEFAULT_POSITION Then Return ___position
				Dim value As Object = javax.swing.UIManager.get("TitledBorder.position")
				If TypeOf value Is Integer? Then
					Dim i As Integer = CInt(Fix(value))
					If (0 < i) AndAlso (i <= 6) Then Return i
				ElseIf TypeOf value Is String Then
					Dim s As String = CStr(value)
					If s.ToUpper() = "ABOVE_TOP".ToUpper() Then Return ABOVE_TOP
					If s.ToUpper() = "TOP".ToUpper() Then Return TOP
					If s.ToUpper() = "BELOW_TOP".ToUpper() Then Return BELOW_TOP
					If s.ToUpper() = "ABOVE_BOTTOM".ToUpper() Then Return ABOVE_BOTTOM
					If s.ToUpper() = "BOTTOM".ToUpper() Then Return BOTTOM
					If s.ToUpper() = "BELOW_BOTTOM".ToUpper() Then Return BELOW_BOTTOM
				End If
				Return TOP
			End Get
		End Property

		Private Function getJustification(ByVal c As java.awt.Component) As Integer
			Dim ___justification As Integer = titleJustification
			If (___justification = LEADING) OrElse (___justification = DEFAULT_JUSTIFICATION) Then Return If(c.componentOrientation.leftToRight, LEFT, RIGHT)
			If ___justification = TRAILING Then Return If(c.componentOrientation.leftToRight, RIGHT, LEFT)
			Return ___justification
		End Function

		Protected Friend Overridable Function getFont(ByVal c As java.awt.Component) As java.awt.Font
			Dim ___font As java.awt.Font = titleFont
			If ___font IsNot Nothing Then Return ___font
			If c IsNot Nothing Then
				___font = c.font
				If ___font IsNot Nothing Then Return ___font
			End If
			Return New java.awt.Font(java.awt.Font.DIALOG, java.awt.Font.PLAIN, 12)
		End Function

		Private Function getColor(ByVal c As java.awt.Component) As java.awt.Color
			Dim ___color As java.awt.Color = titleColor
			If ___color IsNot Nothing Then Return ___color
			Return If(c IsNot Nothing, c.foreground, Nothing)
		End Function

		Private Function getLabel(ByVal c As java.awt.Component) As javax.swing.JLabel
			Me.label.text = title
			Me.label.font = getFont(c)
			Me.label.foreground = getColor(c)
			Me.label.componentOrientation = c.componentOrientation
			Me.label.enabled = c.enabled
			Return Me.label
		End Function

		Private Shared Function getBorderInsets(ByVal border As Border, ByVal c As java.awt.Component, ByVal insets As java.awt.Insets) As java.awt.Insets
			If border Is Nothing Then
				insets.set(0, 0, 0, 0)
			ElseIf TypeOf border Is AbstractBorder Then
				Dim ab As AbstractBorder = CType(border, AbstractBorder)
				insets = ab.getBorderInsets(c, insets)
			Else
				Dim i As java.awt.Insets = border.getBorderInsets(c)
				insets.set(i.top, i.left, i.bottom, i.right)
			End If
			Return insets
		End Function
	End Class

End Namespace