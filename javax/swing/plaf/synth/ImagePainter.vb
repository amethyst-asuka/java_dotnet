Imports System.Text
Imports javax.swing

'
' * Copyright (c) 2002, 2008, Oracle and/or its affiliates. All rights reserved.
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
Namespace javax.swing.plaf.synth


	''' <summary>
	''' ImagePainter fills in the specified region using an Image. The Image
	''' is split into 9 segments: north, north east, east, south east, south,
	''' south west, west, north west and the center. The corners are defined
	''' by way of an insets, and the remaining regions are either tiled or
	''' scaled to fit.
	''' 
	''' @author Scott Violet
	''' </summary>
	Friend Class ImagePainter
		Inherits SynthPainter

		Private Shared ReadOnly CACHE_KEY As New StringBuilder("SynthCacheKey")

		Private image As Image
		Private sInsets As Insets
		Private dInsets As Insets
		Private path As URL
		Private tiles As Boolean
		Private paintCenter As Boolean
		Private imageCache As sun.swing.plaf.synth.Paint9Painter
		Private center As Boolean

		Private Property Shared paint9Painter As sun.swing.plaf.synth.Paint9Painter
			Get
				' A SynthPainter is created per <imagePainter>.  We want the
				' cache to be shared by all, and we don't use a static because we
				' don't want it to persist between look and feels.  For that reason
				' we use a AppContext specific Paint9Painter.  It's backed via
				' a WeakRef so that it can go away if the look and feel changes.
				SyncLock CACHE_KEY
					Dim cacheRef As WeakReference(Of sun.swing.plaf.synth.Paint9Painter) = CType(sun.awt.AppContext.appContext.get(CACHE_KEY), WeakReference(Of sun.swing.plaf.synth.Paint9Painter))
					Dim painter As sun.swing.plaf.synth.Paint9Painter
					painter = cacheRef.get()
					If cacheRef Is Nothing OrElse painter Is Nothing Then
						painter = New sun.swing.plaf.synth.Paint9Painter(30)
						cacheRef = New WeakReference(Of sun.swing.plaf.synth.Paint9Painter)(painter)
						sun.awt.AppContext.appContext.put(CACHE_KEY, cacheRef)
					End If
					Return painter
				End SyncLock
			End Get
		End Property

		Friend Sub New(ByVal tiles As Boolean, ByVal paintCenter As Boolean, ByVal sourceInsets As Insets, ByVal destinationInsets As Insets, ByVal path As URL, ByVal center As Boolean)
			If sourceInsets IsNot Nothing Then Me.sInsets = CType(sourceInsets.clone(), Insets)
			If destinationInsets Is Nothing Then
				dInsets = sInsets
			Else
				Me.dInsets = CType(destinationInsets.clone(), Insets)
			End If
			Me.tiles = tiles
			Me.paintCenter = paintCenter
			Me.imageCache = paint9Painter
			Me.path = path
			Me.center = center
		End Sub

		Public Overridable Property tiles As Boolean
			Get
				Return tiles
			End Get
		End Property

		Public Overridable Property paintsCenter As Boolean
			Get
				Return paintCenter
			End Get
		End Property

		Public Overridable Property center As Boolean
			Get
				Return center
			End Get
		End Property

		Public Overridable Function getInsets(ByVal insets As Insets) As Insets
			If insets Is Nothing Then Return CType(Me.dInsets.clone(), Insets)
			insets.left = Me.dInsets.left
			insets.right = Me.dInsets.right
			insets.top = Me.dInsets.top
			insets.bottom = Me.dInsets.bottom
			Return insets
		End Function

		Public Overridable Property image As Image
			Get
				If image Is Nothing Then image = (New ImageIcon(path, Nothing)).image
				Return image
			End Get
		End Property

		Private Sub paint(ByVal context As SynthContext, ByVal g As Graphics, ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer)
			Dim ___image As Image = image
			If sun.swing.plaf.synth.Paint9Painter.validImage(___image) Then
				Dim type As sun.swing.plaf.synth.Paint9Painter.PaintType
				If center Then
					type = sun.swing.plaf.synth.Paint9Painter.PaintType.CENTER
				ElseIf Not tiles Then
					type = sun.swing.plaf.synth.Paint9Painter.PaintType.PAINT9_STRETCH
				Else
					type = sun.swing.plaf.synth.Paint9Painter.PaintType.PAINT9_TILE
				End If
				Dim mask As Integer = sun.swing.plaf.synth.Paint9Painter.PAINT_ALL
				If (Not center) AndAlso (Not paintsCenter) Then mask = mask Or sun.swing.plaf.synth.Paint9Painter.PAINT_CENTER
				imageCache.paint(context.component, g, x, y, w, h, ___image, sInsets, dInsets, type, mask)
			End If
		End Sub


		' SynthPainter
		Public Overrides Sub paintArrowButtonBackground(ByVal context As SynthContext, ByVal g As Graphics, ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer)
			paint(context, g, x, y, w, h)
		End Sub

		Public Overrides Sub paintArrowButtonBorder(ByVal context As SynthContext, ByVal g As Graphics, ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer)
			paint(context, g, x, y, w, h)
		End Sub

		Public Overrides Sub paintArrowButtonForeground(ByVal context As SynthContext, ByVal g As Graphics, ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer, ByVal direction As Integer)
			paint(context, g, x, y, w, h)
		End Sub

		' BUTTON
		Public Overrides Sub paintButtonBackground(ByVal context As SynthContext, ByVal g As Graphics, ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer)
			paint(context, g, x, y, w, h)
		End Sub

		Public Overrides Sub paintButtonBorder(ByVal context As SynthContext, ByVal g As Graphics, ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer)
			paint(context, g, x, y, w, h)
		End Sub

		' CHECK_BOX_MENU_ITEM
		Public Overrides Sub paintCheckBoxMenuItemBackground(ByVal context As SynthContext, ByVal g As Graphics, ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer)
			paint(context, g, x, y, w, h)
		End Sub

		Public Overrides Sub paintCheckBoxMenuItemBorder(ByVal context As SynthContext, ByVal g As Graphics, ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer)
			paint(context, g, x, y, w, h)
		End Sub

		' CHECK_BOX
		Public Overrides Sub paintCheckBoxBackground(ByVal context As SynthContext, ByVal g As Graphics, ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer)
			paint(context, g, x, y, w, h)
		End Sub

		Public Overrides Sub paintCheckBoxBorder(ByVal context As SynthContext, ByVal g As Graphics, ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer)
			paint(context, g, x, y, w, h)
		End Sub

		' COLOR_CHOOSER
		Public Overrides Sub paintColorChooserBackground(ByVal context As SynthContext, ByVal g As Graphics, ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer)
			paint(context, g, x, y, w, h)
		End Sub

		Public Overrides Sub paintColorChooserBorder(ByVal context As SynthContext, ByVal g As Graphics, ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer)
			paint(context, g, x, y, w, h)
		End Sub

		' COMBO_BOX
		Public Overrides Sub paintComboBoxBackground(ByVal context As SynthContext, ByVal g As Graphics, ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer)
			paint(context, g, x, y, w, h)
		End Sub

		Public Overrides Sub paintComboBoxBorder(ByVal context As SynthContext, ByVal g As Graphics, ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer)
			paint(context, g, x, y, w, h)
		End Sub

		' DESKTOP_ICON
		Public Overrides Sub paintDesktopIconBackground(ByVal context As SynthContext, ByVal g As Graphics, ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer)
			paint(context, g, x, y, w, h)
		End Sub

		Public Overrides Sub paintDesktopIconBorder(ByVal context As SynthContext, ByVal g As Graphics, ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer)
			paint(context, g, x, y, w, h)
		End Sub

		' DESKTOP_PANE
		Public Overrides Sub paintDesktopPaneBackground(ByVal context As SynthContext, ByVal g As Graphics, ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer)
			paint(context, g, x, y, w, h)
		End Sub

		Public Overrides Sub paintDesktopPaneBorder(ByVal context As SynthContext, ByVal g As Graphics, ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer)
			paint(context, g, x, y, w, h)
		End Sub

		' EDITOR_PANE
		Public Overrides Sub paintEditorPaneBackground(ByVal context As SynthContext, ByVal g As Graphics, ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer)
			paint(context, g, x, y, w, h)
		End Sub

		Public Overrides Sub paintEditorPaneBorder(ByVal context As SynthContext, ByVal g As Graphics, ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer)
			paint(context, g, x, y, w, h)
		End Sub

		' FILE_CHOOSER
		Public Overrides Sub paintFileChooserBackground(ByVal context As SynthContext, ByVal g As Graphics, ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer)
			paint(context, g, x, y, w, h)
		End Sub

		Public Overrides Sub paintFileChooserBorder(ByVal context As SynthContext, ByVal g As Graphics, ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer)
			paint(context, g, x, y, w, h)
		End Sub

		' FORMATTED_TEXT_FIELD
		Public Overrides Sub paintFormattedTextFieldBackground(ByVal context As SynthContext, ByVal g As Graphics, ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer)
			paint(context, g, x, y, w, h)
		End Sub

		Public Overrides Sub paintFormattedTextFieldBorder(ByVal context As SynthContext, ByVal g As Graphics, ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer)
			paint(context, g, x, y, w, h)
		End Sub

		' INTERNAL_FRAME_TITLE_PANE
		Public Overrides Sub paintInternalFrameTitlePaneBackground(ByVal context As SynthContext, ByVal g As Graphics, ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer)
			paint(context, g, x, y, w, h)
		End Sub

		Public Overrides Sub paintInternalFrameTitlePaneBorder(ByVal context As SynthContext, ByVal g As Graphics, ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer)
			paint(context, g, x, y, w, h)
		End Sub

		' INTERNAL_FRAME
		Public Overrides Sub paintInternalFrameBackground(ByVal context As SynthContext, ByVal g As Graphics, ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer)
			paint(context, g, x, y, w, h)
		End Sub

		Public Overrides Sub paintInternalFrameBorder(ByVal context As SynthContext, ByVal g As Graphics, ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer)
			paint(context, g, x, y, w, h)
		End Sub

		' LABEL
		Public Overrides Sub paintLabelBackground(ByVal context As SynthContext, ByVal g As Graphics, ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer)
			paint(context, g, x, y, w, h)
		End Sub

		Public Overrides Sub paintLabelBorder(ByVal context As SynthContext, ByVal g As Graphics, ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer)
			paint(context, g, x, y, w, h)
		End Sub

		' LIST
		Public Overrides Sub paintListBackground(ByVal context As SynthContext, ByVal g As Graphics, ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer)
			paint(context, g, x, y, w, h)
		End Sub

		Public Overrides Sub paintListBorder(ByVal context As SynthContext, ByVal g As Graphics, ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer)
			paint(context, g, x, y, w, h)
		End Sub

		' MENU_BAR
		Public Overrides Sub paintMenuBarBackground(ByVal context As SynthContext, ByVal g As Graphics, ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer)
			paint(context, g, x, y, w, h)
		End Sub

		Public Overrides Sub paintMenuBarBorder(ByVal context As SynthContext, ByVal g As Graphics, ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer)
			paint(context, g, x, y, w, h)
		End Sub

		' MENU_ITEM
		Public Overrides Sub paintMenuItemBackground(ByVal context As SynthContext, ByVal g As Graphics, ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer)
			paint(context, g, x, y, w, h)
		End Sub

		Public Overrides Sub paintMenuItemBorder(ByVal context As SynthContext, ByVal g As Graphics, ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer)
			paint(context, g, x, y, w, h)
		End Sub

		' MENU
		Public Overrides Sub paintMenuBackground(ByVal context As SynthContext, ByVal g As Graphics, ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer)
			paint(context, g, x, y, w, h)
		End Sub

		Public Overrides Sub paintMenuBorder(ByVal context As SynthContext, ByVal g As Graphics, ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer)
			paint(context, g, x, y, w, h)
		End Sub

		' OPTION_PANE
		Public Overrides Sub paintOptionPaneBackground(ByVal context As SynthContext, ByVal g As Graphics, ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer)
			paint(context, g, x, y, w, h)
		End Sub

		Public Overrides Sub paintOptionPaneBorder(ByVal context As SynthContext, ByVal g As Graphics, ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer)
			paint(context, g, x, y, w, h)
		End Sub

		' PANEL
		Public Overrides Sub paintPanelBackground(ByVal context As SynthContext, ByVal g As Graphics, ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer)
			paint(context, g, x, y, w, h)
		End Sub

		Public Overrides Sub paintPanelBorder(ByVal context As SynthContext, ByVal g As Graphics, ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer)
			paint(context, g, x, y, w, h)
		End Sub

		' PANEL
		Public Overrides Sub paintPasswordFieldBackground(ByVal context As SynthContext, ByVal g As Graphics, ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer)
			paint(context, g, x, y, w, h)
		End Sub

		Public Overrides Sub paintPasswordFieldBorder(ByVal context As SynthContext, ByVal g As Graphics, ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer)
			paint(context, g, x, y, w, h)
		End Sub

		' POPUP_MENU
		Public Overrides Sub paintPopupMenuBackground(ByVal context As SynthContext, ByVal g As Graphics, ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer)
			paint(context, g, x, y, w, h)
		End Sub

		Public Overrides Sub paintPopupMenuBorder(ByVal context As SynthContext, ByVal g As Graphics, ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer)
			paint(context, g, x, y, w, h)
		End Sub

		' PROGRESS_BAR
		Public Overrides Sub paintProgressBarBackground(ByVal context As SynthContext, ByVal g As Graphics, ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer)
			paint(context, g, x, y, w, h)
		End Sub

		Public Overrides Sub paintProgressBarBackground(ByVal context As SynthContext, ByVal g As Graphics, ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer, ByVal orientation As Integer)
			paint(context, g, x, y, w, h)
		End Sub

		Public Overrides Sub paintProgressBarBorder(ByVal context As SynthContext, ByVal g As Graphics, ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer)
			paint(context, g, x, y, w, h)
		End Sub

		Public Overrides Sub paintProgressBarBorder(ByVal context As SynthContext, ByVal g As Graphics, ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer, ByVal orientation As Integer)
			paint(context, g, x, y, w, h)
		End Sub

		Public Overrides Sub paintProgressBarForeground(ByVal context As SynthContext, ByVal g As Graphics, ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer, ByVal orientation As Integer)
			paint(context, g, x, y, w, h)
		End Sub

		' RADIO_BUTTON_MENU_ITEM
		Public Overrides Sub paintRadioButtonMenuItemBackground(ByVal context As SynthContext, ByVal g As Graphics, ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer)
			paint(context, g, x, y, w, h)
		End Sub

		Public Overrides Sub paintRadioButtonMenuItemBorder(ByVal context As SynthContext, ByVal g As Graphics, ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer)
			paint(context, g, x, y, w, h)
		End Sub

		' RADIO_BUTTON
		Public Overrides Sub paintRadioButtonBackground(ByVal context As SynthContext, ByVal g As Graphics, ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer)
			paint(context, g, x, y, w, h)
		End Sub

		Public Overrides Sub paintRadioButtonBorder(ByVal context As SynthContext, ByVal g As Graphics, ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer)
			paint(context, g, x, y, w, h)
		End Sub

		' ROOT_PANE
		Public Overrides Sub paintRootPaneBackground(ByVal context As SynthContext, ByVal g As Graphics, ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer)
			paint(context, g, x, y, w, h)
		End Sub

		Public Overrides Sub paintRootPaneBorder(ByVal context As SynthContext, ByVal g As Graphics, ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer)
			paint(context, g, x, y, w, h)
		End Sub

		' SCROLL_BAR
		Public Overrides Sub paintScrollBarBackground(ByVal context As SynthContext, ByVal g As Graphics, ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer)
			paint(context, g, x, y, w, h)
		End Sub

		Public Overrides Sub paintScrollBarBackground(ByVal context As SynthContext, ByVal g As Graphics, ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer, ByVal orientation As Integer)
			paint(context, g, x, y, w, h)
		End Sub

		Public Overrides Sub paintScrollBarBorder(ByVal context As SynthContext, ByVal g As Graphics, ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer)
			paint(context, g, x, y, w, h)
		End Sub

		Public Overrides Sub paintScrollBarBorder(ByVal context As SynthContext, ByVal g As Graphics, ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer, ByVal orientation As Integer)
			paint(context, g, x, y, w, h)
		End Sub

		' SCROLL_BAR_THUMB
		Public Overrides Sub paintScrollBarThumbBackground(ByVal context As SynthContext, ByVal g As Graphics, ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer, ByVal orientation As Integer)
			paint(context, g, x, y, w, h)
		End Sub

		Public Overrides Sub paintScrollBarThumbBorder(ByVal context As SynthContext, ByVal g As Graphics, ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer, ByVal orientation As Integer)
			paint(context, g, x, y, w, h)
		End Sub

		' SCROLL_BAR_TRACK
		Public Overrides Sub paintScrollBarTrackBackground(ByVal context As SynthContext, ByVal g As Graphics, ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer)
			paint(context, g, x, y, w, h)
		End Sub

		Public Overrides Sub paintScrollBarTrackBackground(ByVal context As SynthContext, ByVal g As Graphics, ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer, ByVal orientation As Integer)
			 paint(context, g, x, y, w, h)
		End Sub

		Public Overrides Sub paintScrollBarTrackBorder(ByVal context As SynthContext, ByVal g As Graphics, ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer)
			paint(context, g, x, y, w, h)
		End Sub

		Public Overrides Sub paintScrollBarTrackBorder(ByVal context As SynthContext, ByVal g As Graphics, ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer, ByVal orientation As Integer)
			paint(context, g, x, y, w, h)
		End Sub

		' SCROLL_PANE
		Public Overrides Sub paintScrollPaneBackground(ByVal context As SynthContext, ByVal g As Graphics, ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer)
			paint(context, g, x, y, w, h)
		End Sub

		Public Overrides Sub paintScrollPaneBorder(ByVal context As SynthContext, ByVal g As Graphics, ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer)
			paint(context, g, x, y, w, h)
		End Sub

		' SEPARATOR
		Public Overrides Sub paintSeparatorBackground(ByVal context As SynthContext, ByVal g As Graphics, ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer)
			paint(context, g, x, y, w, h)
		End Sub

		Public Overrides Sub paintSeparatorBackground(ByVal context As SynthContext, ByVal g As Graphics, ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer, ByVal orientation As Integer)
			paint(context, g, x, y, w, h)
		End Sub

		Public Overrides Sub paintSeparatorBorder(ByVal context As SynthContext, ByVal g As Graphics, ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer)
			paint(context, g, x, y, w, h)
		End Sub

		Public Overrides Sub paintSeparatorBorder(ByVal context As SynthContext, ByVal g As Graphics, ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer, ByVal orientation As Integer)
			paint(context, g, x, y, w, h)
		End Sub

		Public Overrides Sub paintSeparatorForeground(ByVal context As SynthContext, ByVal g As Graphics, ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer, ByVal orientation As Integer)
			paint(context, g, x, y, w, h)
		End Sub

		' SLIDER
		Public Overrides Sub paintSliderBackground(ByVal context As SynthContext, ByVal g As Graphics, ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer)
			paint(context, g, x, y, w, h)
		End Sub

		Public Overrides Sub paintSliderBackground(ByVal context As SynthContext, ByVal g As Graphics, ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer, ByVal orientation As Integer)
			paint(context, g, x, y, w, h)
		End Sub

		Public Overrides Sub paintSliderBorder(ByVal context As SynthContext, ByVal g As Graphics, ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer)
			paint(context, g, x, y, w, h)
		End Sub

		Public Overrides Sub paintSliderBorder(ByVal context As SynthContext, ByVal g As Graphics, ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer, ByVal orientation As Integer)
			 paint(context, g, x, y, w, h)
		End Sub

		' SLIDER_THUMB
		Public Overrides Sub paintSliderThumbBackground(ByVal context As SynthContext, ByVal g As Graphics, ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer, ByVal orientation As Integer)
			paint(context, g, x, y, w, h)
		End Sub

		Public Overrides Sub paintSliderThumbBorder(ByVal context As SynthContext, ByVal g As Graphics, ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer, ByVal orientation As Integer)
			paint(context, g, x, y, w, h)
		End Sub

		' SLIDER_TRACK
		Public Overrides Sub paintSliderTrackBackground(ByVal context As SynthContext, ByVal g As Graphics, ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer)
			paint(context, g, x, y, w, h)
		End Sub

		Public Overrides Sub paintSliderTrackBackground(ByVal context As SynthContext, ByVal g As Graphics, ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer, ByVal orientation As Integer)
			paint(context, g, x, y, w, h)
		End Sub

		Public Overrides Sub paintSliderTrackBorder(ByVal context As SynthContext, ByVal g As Graphics, ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer)
			paint(context, g, x, y, w, h)
		End Sub


		Public Overrides Sub paintSliderTrackBorder(ByVal context As SynthContext, ByVal g As Graphics, ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer, ByVal orientation As Integer)
			paint(context, g, x, y, w, h)
		End Sub

		' SPINNER
		Public Overrides Sub paintSpinnerBackground(ByVal context As SynthContext, ByVal g As Graphics, ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer)
			paint(context, g, x, y, w, h)
		End Sub

		Public Overrides Sub paintSpinnerBorder(ByVal context As SynthContext, ByVal g As Graphics, ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer)
			paint(context, g, x, y, w, h)
		End Sub

		' SPLIT_PANE_DIVIDER
		Public Overrides Sub paintSplitPaneDividerBackground(ByVal context As SynthContext, ByVal g As Graphics, ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer)
			paint(context, g, x, y, w, h)
		End Sub

		Public Overrides Sub paintSplitPaneDividerBackground(ByVal context As SynthContext, ByVal g As Graphics, ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer, ByVal orientation As Integer)
			paint(context, g, x, y, w, h)
		End Sub

		Public Overrides Sub paintSplitPaneDividerForeground(ByVal context As SynthContext, ByVal g As Graphics, ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer, ByVal orientation As Integer)
			paint(context, g, x, y, w, h)
		End Sub

		Public Overrides Sub paintSplitPaneDragDivider(ByVal context As SynthContext, ByVal g As Graphics, ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer, ByVal orientation As Integer)
			paint(context, g, x, y, w, h)
		End Sub

		' SPLIT_PANE
		Public Overrides Sub paintSplitPaneBackground(ByVal context As SynthContext, ByVal g As Graphics, ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer)
			paint(context, g, x, y, w, h)
		End Sub

		Public Overrides Sub paintSplitPaneBorder(ByVal context As SynthContext, ByVal g As Graphics, ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer)
			paint(context, g, x, y, w, h)
		End Sub

		' TABBED_PANE
		Public Overrides Sub paintTabbedPaneBackground(ByVal context As SynthContext, ByVal g As Graphics, ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer)
			paint(context, g, x, y, w, h)
		End Sub

		Public Overrides Sub paintTabbedPaneBorder(ByVal context As SynthContext, ByVal g As Graphics, ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer)
			paint(context, g, x, y, w, h)
		End Sub

		' TABBED_PANE_TAB_AREA
		Public Overrides Sub paintTabbedPaneTabAreaBackground(ByVal context As SynthContext, ByVal g As Graphics, ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer)
			paint(context, g, x, y, w, h)
		End Sub

		Public Overrides Sub paintTabbedPaneTabAreaBackground(ByVal context As SynthContext, ByVal g As Graphics, ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer, ByVal orientation As Integer)
			paint(context, g, x, y, w, h)
		End Sub

		Public Overrides Sub paintTabbedPaneTabAreaBorder(ByVal context As SynthContext, ByVal g As Graphics, ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer)
			paint(context, g, x, y, w, h)
		End Sub

		Public Overrides Sub paintTabbedPaneTabAreaBorder(ByVal context As SynthContext, ByVal g As Graphics, ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer, ByVal orientation As Integer)
			paint(context, g, x, y, w, h)
		End Sub

		' TABBED_PANE_TAB
		Public Overrides Sub paintTabbedPaneTabBackground(ByVal context As SynthContext, ByVal g As Graphics, ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer, ByVal tabIndex As Integer)
			paint(context, g, x, y, w, h)
		End Sub

		Public Overrides Sub paintTabbedPaneTabBackground(ByVal context As SynthContext, ByVal g As Graphics, ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer, ByVal tabIndex As Integer, ByVal orientation As Integer)
			paint(context, g, x, y, w, h)
		End Sub

		Public Overrides Sub paintTabbedPaneTabBorder(ByVal context As SynthContext, ByVal g As Graphics, ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer, ByVal tabIndex As Integer)
			paint(context, g, x, y, w, h)
		End Sub

		Public Overrides Sub paintTabbedPaneTabBorder(ByVal context As SynthContext, ByVal g As Graphics, ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer, ByVal tabIndex As Integer, ByVal orientation As Integer)
			paint(context, g, x, y, w, h)
		End Sub

		' TABBED_PANE_CONTENT
		Public Overrides Sub paintTabbedPaneContentBackground(ByVal context As SynthContext, ByVal g As Graphics, ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer)
			paint(context, g, x, y, w, h)
		End Sub

		Public Overrides Sub paintTabbedPaneContentBorder(ByVal context As SynthContext, ByVal g As Graphics, ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer)
			paint(context, g, x, y, w, h)
		End Sub

		' TABLE_HEADER
		Public Overrides Sub paintTableHeaderBackground(ByVal context As SynthContext, ByVal g As Graphics, ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer)
			paint(context, g, x, y, w, h)
		End Sub

		Public Overrides Sub paintTableHeaderBorder(ByVal context As SynthContext, ByVal g As Graphics, ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer)
			paint(context, g, x, y, w, h)
		End Sub

		' TABLE
		Public Overrides Sub paintTableBackground(ByVal context As SynthContext, ByVal g As Graphics, ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer)
			paint(context, g, x, y, w, h)
		End Sub

		Public Overrides Sub paintTableBorder(ByVal context As SynthContext, ByVal g As Graphics, ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer)
			paint(context, g, x, y, w, h)
		End Sub

		' TEXT_AREA
		Public Overrides Sub paintTextAreaBackground(ByVal context As SynthContext, ByVal g As Graphics, ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer)
			paint(context, g, x, y, w, h)
		End Sub

		Public Overrides Sub paintTextAreaBorder(ByVal context As SynthContext, ByVal g As Graphics, ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer)
			paint(context, g, x, y, w, h)
		End Sub

		' TEXT_PANE
		Public Overrides Sub paintTextPaneBackground(ByVal context As SynthContext, ByVal g As Graphics, ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer)
			paint(context, g, x, y, w, h)
		End Sub

		Public Overrides Sub paintTextPaneBorder(ByVal context As SynthContext, ByVal g As Graphics, ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer)
			paint(context, g, x, y, w, h)
		End Sub

		' TEXT_FIELD
		Public Overrides Sub paintTextFieldBackground(ByVal context As SynthContext, ByVal g As Graphics, ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer)
			paint(context, g, x, y, w, h)
		End Sub

		Public Overrides Sub paintTextFieldBorder(ByVal context As SynthContext, ByVal g As Graphics, ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer)
			paint(context, g, x, y, w, h)
		End Sub

		' TOGGLE_BUTTON
		Public Overrides Sub paintToggleButtonBackground(ByVal context As SynthContext, ByVal g As Graphics, ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer)
			paint(context, g, x, y, w, h)
		End Sub

		Public Overrides Sub paintToggleButtonBorder(ByVal context As SynthContext, ByVal g As Graphics, ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer)
			paint(context, g, x, y, w, h)
		End Sub

		' TOOL_BAR
		Public Overrides Sub paintToolBarBackground(ByVal context As SynthContext, ByVal g As Graphics, ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer)
			paint(context, g, x, y, w, h)
		End Sub

		Public Overrides Sub paintToolBarBackground(ByVal context As SynthContext, ByVal g As Graphics, ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer, ByVal orientation As Integer)
			paint(context, g, x, y, w, h)
		End Sub

		Public Overrides Sub paintToolBarBorder(ByVal context As SynthContext, ByVal g As Graphics, ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer)
			paint(context, g, x, y, w, h)
		End Sub

		Public Overrides Sub paintToolBarBorder(ByVal context As SynthContext, ByVal g As Graphics, ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer, ByVal orientation As Integer)
			paint(context, g, x, y, w, h)
		End Sub

		' TOOL_BAR_CONTENT
		Public Overrides Sub paintToolBarContentBackground(ByVal context As SynthContext, ByVal g As Graphics, ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer)
			paint(context, g, x, y, w, h)
		End Sub

		Public Overrides Sub paintToolBarContentBackground(ByVal context As SynthContext, ByVal g As Graphics, ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer, ByVal orientation As Integer)
			paint(context, g, x, y, w, h)
		End Sub

		Public Overrides Sub paintToolBarContentBorder(ByVal context As SynthContext, ByVal g As Graphics, ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer)
			paint(context, g, x, y, w, h)
		End Sub

		Public Overrides Sub paintToolBarContentBorder(ByVal context As SynthContext, ByVal g As Graphics, ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer, ByVal orientation As Integer)
			paint(context, g, x, y, w, h)
		End Sub

		' TOOL_DRAG_WINDOW
		Public Overrides Sub paintToolBarDragWindowBackground(ByVal context As SynthContext, ByVal g As Graphics, ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer)
			paint(context, g, x, y, w, h)
		End Sub

		Public Overrides Sub paintToolBarDragWindowBackground(ByVal context As SynthContext, ByVal g As Graphics, ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer, ByVal orientation As Integer)
			paint(context, g, x, y, w, h)
		End Sub

		Public Overrides Sub paintToolBarDragWindowBorder(ByVal context As SynthContext, ByVal g As Graphics, ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer)
			paint(context, g, x, y, w, h)
		End Sub

		Public Overrides Sub paintToolBarDragWindowBorder(ByVal context As SynthContext, ByVal g As Graphics, ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer, ByVal orientation As Integer)
			paint(context, g, x, y, w, h)
		End Sub

		' TOOL_TIP
		Public Overrides Sub paintToolTipBackground(ByVal context As SynthContext, ByVal g As Graphics, ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer)
			paint(context, g, x, y, w, h)
		End Sub

		Public Overrides Sub paintToolTipBorder(ByVal context As SynthContext, ByVal g As Graphics, ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer)
			paint(context, g, x, y, w, h)
		End Sub

		' TREE
		Public Overrides Sub paintTreeBackground(ByVal context As SynthContext, ByVal g As Graphics, ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer)
			paint(context, g, x, y, w, h)
		End Sub

		Public Overrides Sub paintTreeBorder(ByVal context As SynthContext, ByVal g As Graphics, ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer)
			paint(context, g, x, y, w, h)
		End Sub

		' TREE_CELL
		Public Overrides Sub paintTreeCellBackground(ByVal context As SynthContext, ByVal g As Graphics, ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer)
			paint(context, g, x, y, w, h)
		End Sub

		Public Overrides Sub paintTreeCellBorder(ByVal context As SynthContext, ByVal g As Graphics, ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer)
			paint(context, g, x, y, w, h)
		End Sub

		Public Overrides Sub paintTreeCellFocus(ByVal context As SynthContext, ByVal g As Graphics, ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer)
			paint(context, g, x, y, w, h)
		End Sub

		' VIEWPORT
		Public Overrides Sub paintViewportBackground(ByVal context As SynthContext, ByVal g As Graphics, ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer)
			paint(context, g, x, y, w, h)
		End Sub

		Public Overrides Sub paintViewportBorder(ByVal context As SynthContext, ByVal g As Graphics, ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer)
			paint(context, g, x, y, w, h)
		End Sub
	End Class

End Namespace