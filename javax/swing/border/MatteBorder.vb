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
	''' A class which provides a matte-like border of either a solid color
	''' or a tiled icon.
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
	''' @author Amy Fowler
	''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
	Public Class MatteBorder
		Inherits EmptyBorder

		Protected Friend color As java.awt.Color
		Protected Friend tileIcon As javax.swing.Icon

		''' <summary>
		''' Creates a matte border with the specified insets and color. </summary>
		''' <param name="top"> the top inset of the border </param>
		''' <param name="left"> the left inset of the border </param>
		''' <param name="bottom"> the bottom inset of the border </param>
		''' <param name="right"> the right inset of the border </param>
		''' <param name="matteColor"> the color rendered for the border </param>
		Public Sub New(ByVal top As Integer, ByVal left As Integer, ByVal bottom As Integer, ByVal right As Integer, ByVal matteColor As java.awt.Color)
			MyBase.New(top, left, bottom, right)
			Me.color = matteColor
		End Sub

		''' <summary>
		''' Creates a matte border with the specified insets and color. </summary>
		''' <param name="borderInsets"> the insets of the border </param>
		''' <param name="matteColor"> the color rendered for the border
		''' @since 1.3 </param>
		Public Sub New(ByVal borderInsets As java.awt.Insets, ByVal matteColor As java.awt.Color)
			MyBase.New(borderInsets)
			Me.color = matteColor
		End Sub

		''' <summary>
		''' Creates a matte border with the specified insets and tile icon. </summary>
		''' <param name="top"> the top inset of the border </param>
		''' <param name="left"> the left inset of the border </param>
		''' <param name="bottom"> the bottom inset of the border </param>
		''' <param name="right"> the right inset of the border </param>
		''' <param name="tileIcon"> the icon to be used for tiling the border </param>
		Public Sub New(ByVal top As Integer, ByVal left As Integer, ByVal bottom As Integer, ByVal right As Integer, ByVal tileIcon As javax.swing.Icon)
			MyBase.New(top, left, bottom, right)
			Me.tileIcon = tileIcon
		End Sub

		''' <summary>
		''' Creates a matte border with the specified insets and tile icon. </summary>
		''' <param name="borderInsets"> the insets of the border </param>
		''' <param name="tileIcon"> the icon to be used for tiling the border
		''' @since 1.3 </param>
		Public Sub New(ByVal borderInsets As java.awt.Insets, ByVal tileIcon As javax.swing.Icon)
			MyBase.New(borderInsets)
			Me.tileIcon = tileIcon
		End Sub

		''' <summary>
		''' Creates a matte border with the specified tile icon.  The
		''' insets will be calculated dynamically based on the size of
		''' the tile icon, where the top and bottom will be equal to the
		''' tile icon's height, and the left and right will be equal to
		''' the tile icon's width. </summary>
		''' <param name="tileIcon"> the icon to be used for tiling the border </param>
		Public Sub New(ByVal tileIcon As javax.swing.Icon)
			Me.New(-1,-1,-1,-1, tileIcon)
		End Sub

		''' <summary>
		''' Paints the matte border.
		''' </summary>
		Public Overrides Sub paintBorder(ByVal c As java.awt.Component, ByVal g As java.awt.Graphics, ByVal x As Integer, ByVal y As Integer, ByVal width As Integer, ByVal height As Integer)
			Dim insets As java.awt.Insets = getBorderInsets(c)
			Dim oldColor As java.awt.Color = g.color
			g.translate(x, y)

			' If the tileIcon failed loading, paint as gray.
			If tileIcon IsNot Nothing Then color = If(tileIcon.iconWidth = -1, java.awt.Color.gray, Nothing)

			If color IsNot Nothing Then
				g.color = color
				g.fillRect(0, 0, width - insets.right, insets.top)
				g.fillRect(0, insets.top, insets.left, height - insets.top)
				g.fillRect(insets.left, height - insets.bottom, width - insets.left, insets.bottom)
				g.fillRect(width - insets.right, 0, insets.right, height - insets.bottom)

			ElseIf tileIcon IsNot Nothing Then
				Dim tileW As Integer = tileIcon.iconWidth
				Dim tileH As Integer = tileIcon.iconHeight
				paintEdge(c, g, 0, 0, width - insets.right, insets.top, tileW, tileH)
				paintEdge(c, g, 0, insets.top, insets.left, height - insets.top, tileW, tileH)
				paintEdge(c, g, insets.left, height - insets.bottom, width - insets.left, insets.bottom, tileW, tileH)
				paintEdge(c, g, width - insets.right, 0, insets.right, height - insets.bottom, tileW, tileH)
			End If
			g.translate(-x, -y)
			g.color = oldColor

		End Sub

		Private Sub paintEdge(ByVal c As java.awt.Component, ByVal g As java.awt.Graphics, ByVal x As Integer, ByVal y As Integer, ByVal width As Integer, ByVal height As Integer, ByVal tileW As Integer, ByVal tileH As Integer)
			g = g.create(x, y, width, height)
			Dim sY As Integer = -(y Mod tileH)
			For x = -(x Mod tileW) To width - 1 Step tileW
				For y = sY To height - 1 Step tileH
					Me.tileIcon.paintIcon(c, g, x, y)
				Next y
			Next x
			g.Dispose()
		End Sub

		''' <summary>
		''' Reinitialize the insets parameter with this Border's current Insets. </summary>
		''' <param name="c"> the component for which this border insets value applies </param>
		''' <param name="insets"> the object to be reinitialized
		''' @since 1.3 </param>
		Public Overrides Function getBorderInsets(ByVal c As java.awt.Component, ByVal insets As java.awt.Insets) As java.awt.Insets
			Return computeInsets(insets)
		End Function

		''' <summary>
		''' Returns the insets of the border.
		''' @since 1.3
		''' </summary>
		Public Property Overrides borderInsets As java.awt.Insets
			Get
				Return computeInsets(New java.awt.Insets(0,0,0,0))
			End Get
		End Property

		' should be protected once api changes area allowed 
		Private Function computeInsets(ByVal insets As java.awt.Insets) As java.awt.Insets
			If tileIcon IsNot Nothing AndAlso top = -1 AndAlso bottom = -1 AndAlso left = -1 AndAlso right = -1 Then
				Dim w As Integer = tileIcon.iconWidth
				Dim h As Integer = tileIcon.iconHeight
				insets.top = h
				insets.right = w
				insets.bottom = h
				insets.left = w
			Else
				insets.left = left
				insets.top = top
				insets.right = right
				insets.bottom = bottom
			End If
			Return insets
		End Function

		''' <summary>
		''' Returns the color used for tiling the border or null
		''' if a tile icon is being used.
		''' @since 1.3
		''' </summary>
		Public Overridable Property matteColor As java.awt.Color
			Get
				Return color
			End Get
		End Property

	   ''' <summary>
	   ''' Returns the icon used for tiling the border or null
	   ''' if a solid color is being used.
	   ''' @since 1.3
	   ''' </summary>
		Public Overridable Property tileIcon As javax.swing.Icon
			Get
				Return tileIcon
			End Get
		End Property

		''' <summary>
		''' Returns whether or not the border is opaque.
		''' </summary>
		Public Property Overrides borderOpaque As Boolean
			Get
				' If a tileIcon is set, then it may contain transparent bits
				Return color IsNot Nothing
			End Get
		End Property

	End Class

End Namespace