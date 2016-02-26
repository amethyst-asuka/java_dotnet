Imports System
Imports System.Collections.Generic
Imports javax.swing

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

Namespace javax.swing.plaf.metal


	''' <summary>
	''' Factory object that vends <code>Icon</code>s for
	''' the Java&trade; look and feel (Metal).
	''' These icons are used extensively in Metal via the defaults mechanism.
	''' While other look and feels often use GIFs for icons, creating icons
	''' in code facilitates switching to other themes.
	''' 
	''' <p>
	''' Each method in this class returns
	''' either an <code>Icon</code> or <code>null</code>,
	''' where <code>null</code> implies that there is no default icon.
	''' 
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
	''' @author Michael C. Albers
	''' </summary>
	<Serializable> _
	Public Class MetalIconFactory

		' List of code-drawn Icons
		Private Shared fileChooserDetailViewIcon As Icon
		Private Shared fileChooserHomeFolderIcon As Icon
		Private Shared fileChooserListViewIcon As Icon
		Private Shared fileChooserNewFolderIcon As Icon
		Private Shared fileChooserUpFolderIcon As Icon
		Private Shared internalFrameAltMaximizeIcon As Icon
		Private Shared internalFrameCloseIcon As Icon
		Private Shared internalFrameDefaultMenuIcon As Icon
		Private Shared internalFrameMaximizeIcon As Icon
		Private Shared internalFrameMinimizeIcon As Icon
		Private Shared radioButtonIcon As Icon
		Private Shared treeComputerIcon As Icon
		Private Shared treeFloppyDriveIcon As Icon
		Private Shared treeHardDriveIcon As Icon


		Private Shared menuArrowIcon As Icon
		Private Shared menuItemArrowIcon As Icon
		Private Shared checkBoxMenuItemIcon As Icon
		Private Shared radioButtonMenuItemIcon As Icon
		Private Shared checkBoxIcon As Icon


		' Ocean icons
		Private Shared oceanHorizontalSliderThumb As Icon
		Private Shared oceanVerticalSliderThumb As Icon

		' Constants
		Public Const DARK As Boolean = False
		Public Const LIGHT As Boolean = True

		' Accessor functions for Icons. Does the caching work.
		Public Property Shared fileChooserDetailViewIcon As Icon
			Get
				If fileChooserDetailViewIcon Is Nothing Then fileChooserDetailViewIcon = New FileChooserDetailViewIcon
				Return fileChooserDetailViewIcon
			End Get
		End Property

		Public Property Shared fileChooserHomeFolderIcon As Icon
			Get
				If fileChooserHomeFolderIcon Is Nothing Then fileChooserHomeFolderIcon = New FileChooserHomeFolderIcon
				Return fileChooserHomeFolderIcon
			End Get
		End Property

		Public Property Shared fileChooserListViewIcon As Icon
			Get
				If fileChooserListViewIcon Is Nothing Then fileChooserListViewIcon = New FileChooserListViewIcon
				Return fileChooserListViewIcon
			End Get
		End Property

		Public Property Shared fileChooserNewFolderIcon As Icon
			Get
				If fileChooserNewFolderIcon Is Nothing Then fileChooserNewFolderIcon = New FileChooserNewFolderIcon
				Return fileChooserNewFolderIcon
			End Get
		End Property

		Public Property Shared fileChooserUpFolderIcon As Icon
			Get
				If fileChooserUpFolderIcon Is Nothing Then fileChooserUpFolderIcon = New FileChooserUpFolderIcon
				Return fileChooserUpFolderIcon
			End Get
		End Property

		Public Shared Function getInternalFrameAltMaximizeIcon(ByVal size As Integer) As Icon
			Return New InternalFrameAltMaximizeIcon(size)
		End Function

		Public Shared Function getInternalFrameCloseIcon(ByVal size As Integer) As Icon
			Return New InternalFrameCloseIcon(size)
		End Function

		Public Property Shared internalFrameDefaultMenuIcon As Icon
			Get
				If internalFrameDefaultMenuIcon Is Nothing Then internalFrameDefaultMenuIcon = New InternalFrameDefaultMenuIcon
				Return internalFrameDefaultMenuIcon
			End Get
		End Property

		Public Shared Function getInternalFrameMaximizeIcon(ByVal size As Integer) As Icon
			Return New InternalFrameMaximizeIcon(size)
		End Function

		Public Shared Function getInternalFrameMinimizeIcon(ByVal size As Integer) As Icon
			Return New InternalFrameMinimizeIcon(size)
		End Function

		Public Property Shared radioButtonIcon As Icon
			Get
				If radioButtonIcon Is Nothing Then radioButtonIcon = New RadioButtonIcon
				Return radioButtonIcon
			End Get
		End Property

		''' <summary>
		''' Returns a checkbox icon.
		''' @since 1.3
		''' </summary>
		Public Property Shared checkBoxIcon As Icon
			Get
				If checkBoxIcon Is Nothing Then checkBoxIcon = New CheckBoxIcon
				Return checkBoxIcon
			End Get
		End Property

		Public Property Shared treeComputerIcon As Icon
			Get
				If treeComputerIcon Is Nothing Then treeComputerIcon = New TreeComputerIcon
				Return treeComputerIcon
			End Get
		End Property

		Public Property Shared treeFloppyDriveIcon As Icon
			Get
				If treeFloppyDriveIcon Is Nothing Then treeFloppyDriveIcon = New TreeFloppyDriveIcon
				Return treeFloppyDriveIcon
			End Get
		End Property

		Public Property Shared treeFolderIcon As Icon
			Get
				Return New TreeFolderIcon
			End Get
		End Property

		Public Property Shared treeHardDriveIcon As Icon
			Get
				If treeHardDriveIcon Is Nothing Then treeHardDriveIcon = New TreeHardDriveIcon
				Return treeHardDriveIcon
			End Get
		End Property

		Public Property Shared treeLeafIcon As Icon
			Get
				Return New TreeLeafIcon
			End Get
		End Property

		Public Shared Function getTreeControlIcon(ByVal isCollapsed As Boolean) As Icon
				Return New TreeControlIcon(isCollapsed)
		End Function

		Public Property Shared menuArrowIcon As Icon
			Get
				If menuArrowIcon Is Nothing Then menuArrowIcon = New MenuArrowIcon
				Return menuArrowIcon
			End Get
		End Property

		''' <summary>
		''' Returns an icon to be used by <code>JCheckBoxMenuItem</code>.
		''' </summary>
		''' <returns> the default icon for check box menu items,
		'''         or <code>null</code> if no default exists </returns>
		Public Property Shared menuItemCheckIcon As Icon
			Get
				Return Nothing
			End Get
		End Property

		Public Property Shared menuItemArrowIcon As Icon
			Get
				If menuItemArrowIcon Is Nothing Then menuItemArrowIcon = New MenuItemArrowIcon
				Return menuItemArrowIcon
			End Get
		End Property

		Public Property Shared checkBoxMenuItemIcon As Icon
			Get
				If checkBoxMenuItemIcon Is Nothing Then checkBoxMenuItemIcon = New CheckBoxMenuItemIcon
				Return checkBoxMenuItemIcon
			End Get
		End Property

		Public Property Shared radioButtonMenuItemIcon As Icon
			Get
				If radioButtonMenuItemIcon Is Nothing Then radioButtonMenuItemIcon = New RadioButtonMenuItemIcon
				Return radioButtonMenuItemIcon
			End Get
		End Property

		Public Property Shared horizontalSliderThumbIcon As Icon
			Get
				If MetalLookAndFeel.usingOcean() Then
					If oceanHorizontalSliderThumb Is Nothing Then oceanHorizontalSliderThumb = New OceanHorizontalSliderThumbIcon
					Return oceanHorizontalSliderThumb
				End If
			  ' don't cache these, bumps don't get updated otherwise
				Return New HorizontalSliderThumbIcon
			End Get
		End Property

		Public Property Shared verticalSliderThumbIcon As Icon
			Get
				If MetalLookAndFeel.usingOcean() Then
					If oceanVerticalSliderThumb Is Nothing Then oceanVerticalSliderThumb = New OceanVerticalSliderThumbIcon
					Return oceanVerticalSliderThumb
				End If
				' don't cache these, bumps don't get updated otherwise
				Return New VerticalSliderThumbIcon
			End Get
		End Property

		' File Chooser Detail View code
		<Serializable> _
		Private Class FileChooserDetailViewIcon
			Implements Icon, javax.swing.plaf.UIResource

			Public Overridable Sub paintIcon(ByVal c As Component, ByVal g As Graphics, ByVal x As Integer, ByVal y As Integer)
				g.translate(x, y)

				' Draw outside edge of each of the documents
				g.color = MetalLookAndFeel.primaryControlInfo
				'     top
				g.drawLine(2,2, 5,2) ' top
				g.drawLine(2,3, 2,7) ' left
				g.drawLine(3,7, 6,7) ' bottom
				g.drawLine(6,6, 6,3) ' right
				'     bottom
				g.drawLine(2,10, 5,10) ' top
				g.drawLine(2,11, 2,15) ' left
				g.drawLine(3,15, 6,15) ' bottom
				g.drawLine(6,14, 6,11) ' right

				' Draw little dots next to documents
				'     Same color as outside edge
				g.drawLine(8,5, 15,5) ' top
				g.drawLine(8,13, 15,13) ' bottom

				' Draw inner highlight on documents
				g.color = MetalLookAndFeel.primaryControl
				g.drawRect(3,3, 2,3) ' top
				g.drawRect(3,11, 2,3) ' bottom

				' Draw inner inner highlight on documents
				g.color = MetalLookAndFeel.primaryControlHighlight
				g.drawLine(4,4, 4,5) ' top
				g.drawLine(4,12, 4,13) ' bottom

				g.translate(-x, -y)
			End Sub

			Public Overridable Property iconWidth As Integer Implements Icon.getIconWidth
				Get
					Return 18
				End Get
			End Property

			Public Overridable Property iconHeight As Integer Implements Icon.getIconHeight
				Get
					Return 18
				End Get
			End Property
		End Class ' End class FileChooserDetailViewIcon

		' File Chooser Home Folder code
		<Serializable> _
		Private Class FileChooserHomeFolderIcon
			Implements Icon, javax.swing.plaf.UIResource

			Public Overridable Sub paintIcon(ByVal c As Component, ByVal g As Graphics, ByVal x As Integer, ByVal y As Integer)
				g.translate(x, y)

				' Draw outside edge of house
				g.color = MetalLookAndFeel.primaryControlInfo
				g.drawLine(8,1, 1,8) ' left edge of roof
				g.drawLine(8,1, 15,8) ' right edge of roof
				g.drawLine(11,2, 11,3) ' left edge of chimney
				g.drawLine(12,2, 12,4) ' right edge of chimney
				g.drawLine(3,7, 3,15) ' left edge of house
				g.drawLine(13,7, 13,15) ' right edge of house
				g.drawLine(4,15, 12,15) ' bottom edge of house
				' Draw door frame
				'     same color as edge of house
				g.drawLine(6,9, 6,14) ' left
				g.drawLine(10,9, 10,14) ' right
				g.drawLine(7,9, 9, 9) ' top

				' Draw roof body
				g.color = MetalLookAndFeel.controlDarkShadow
				g.fillRect(8,2, 1,1) 'top toward bottom
				g.fillRect(7,3, 3,1)
				g.fillRect(6,4, 5,1)
				g.fillRect(5,5, 7,1)
				g.fillRect(4,6, 9,2)
				' Draw doornob
				'     same color as roof body
				g.drawLine(9,12, 9,12)

				' Paint the house
				g.color = MetalLookAndFeel.primaryControl
				g.drawLine(4,8, 12,8) ' above door
				g.fillRect(4,9, 2,6) ' left of door
				g.fillRect(11,9, 2,6) ' right of door

				g.translate(-x, -y)
			End Sub

			Public Overridable Property iconWidth As Integer Implements Icon.getIconWidth
				Get
					Return 18
				End Get
			End Property

			Public Overridable Property iconHeight As Integer Implements Icon.getIconHeight
				Get
					Return 18
				End Get
			End Property
		End Class ' End class FileChooserHomeFolderIcon

		' File Chooser List View code
		<Serializable> _
		Private Class FileChooserListViewIcon
			Implements Icon, javax.swing.plaf.UIResource

			Public Overridable Sub paintIcon(ByVal c As Component, ByVal g As Graphics, ByVal x As Integer, ByVal y As Integer)
				g.translate(x, y)

				' Draw outside edge of each of the documents
				g.color = MetalLookAndFeel.primaryControlInfo
				'     top left
				g.drawLine(2,2, 5,2) ' top
				g.drawLine(2,3, 2,7) ' left
				g.drawLine(3,7, 6,7) ' bottom
				g.drawLine(6,6, 6,3) ' right
				'     top right
				g.drawLine(10,2, 13,2) ' top
				g.drawLine(10,3, 10,7) ' left
				g.drawLine(11,7, 14,7) ' bottom
				g.drawLine(14,6, 14,3) ' right
				'     bottom left
				g.drawLine(2,10, 5,10) ' top
				g.drawLine(2,11, 2,15) ' left
				g.drawLine(3,15, 6,15) ' bottom
				g.drawLine(6,14, 6,11) ' right
				'     bottom right
				g.drawLine(10,10, 13,10) ' top
				g.drawLine(10,11, 10,15) ' left
				g.drawLine(11,15, 14,15) ' bottom
				g.drawLine(14,14, 14,11) ' right

				' Draw little dots next to documents
				'     Same color as outside edge
				g.drawLine(8,5, 8,5) ' top left
				g.drawLine(16,5, 16,5) ' top right
				g.drawLine(8,13, 8,13) ' bottom left
				g.drawLine(16,13, 16,13) ' bottom right

				' Draw inner highlight on documents
				g.color = MetalLookAndFeel.primaryControl
				g.drawRect(3,3, 2,3) ' top left
				g.drawRect(11,3, 2,3) ' top right
				g.drawRect(3,11, 2,3) ' bottom left
				g.drawRect(11,11, 2,3) ' bottom right

				' Draw inner inner highlight on documents
				g.color = MetalLookAndFeel.primaryControlHighlight
				g.drawLine(4,4, 4,5) ' top left
				g.drawLine(12,4, 12,5) ' top right
				g.drawLine(4,12, 4,13) ' bottom left
				g.drawLine(12,12, 12,13) ' bottom right

				g.translate(-x, -y)
			End Sub

			Public Overridable Property iconWidth As Integer Implements Icon.getIconWidth
				Get
					Return 18
				End Get
			End Property

			Public Overridable Property iconHeight As Integer Implements Icon.getIconHeight
				Get
					Return 18
				End Get
			End Property
		End Class ' End class FileChooserListViewIcon

		' File Chooser New Folder code
		<Serializable> _
		Private Class FileChooserNewFolderIcon
			Implements Icon, javax.swing.plaf.UIResource

			Public Overridable Sub paintIcon(ByVal c As Component, ByVal g As Graphics, ByVal x As Integer, ByVal y As Integer)
				g.translate(x, y)

				' Fill background
				g.color = MetalLookAndFeel.primaryControl
				g.fillRect(3,5, 12,9)

				' Draw outside edge of folder
				g.color = MetalLookAndFeel.primaryControlInfo
				g.drawLine(1,6, 1,14) ' left
				g.drawLine(2,14, 15,14) ' bottom
				g.drawLine(15,13, 15,5) ' right
				g.drawLine(2,5, 9,5) ' top left
				g.drawLine(10,6, 14,6) ' top right

				' Draw inner folder highlight
				g.color = MetalLookAndFeel.primaryControlHighlight
				g.drawLine(2,6, 2,13) ' left
				g.drawLine(3,6, 9,6) ' top left
				g.drawLine(10,7, 14,7) ' top right

				' Draw tab on folder
				g.color = MetalLookAndFeel.primaryControlDarkShadow
				g.drawLine(11,3, 15,3) ' top
				g.drawLine(10,4, 15,4) ' bottom

				g.translate(-x, -y)
			End Sub

			Public Overridable Property iconWidth As Integer Implements Icon.getIconWidth
				Get
					Return 18
				End Get
			End Property

			Public Overridable Property iconHeight As Integer Implements Icon.getIconHeight
				Get
					Return 18
				End Get
			End Property
		End Class ' End class FileChooserNewFolderIcon

		' File Chooser Up Folder code
		<Serializable> _
		Private Class FileChooserUpFolderIcon
			Implements Icon, javax.swing.plaf.UIResource

			Public Overridable Sub paintIcon(ByVal c As Component, ByVal g As Graphics, ByVal x As Integer, ByVal y As Integer)
				g.translate(x, y)

				' Fill background
				g.color = MetalLookAndFeel.primaryControl
				g.fillRect(3,5, 12,9)

				' Draw outside edge of folder
				g.color = MetalLookAndFeel.primaryControlInfo
				g.drawLine(1,6, 1,14) ' left
				g.drawLine(2,14, 15,14) ' bottom
				g.drawLine(15,13, 15,5) ' right
				g.drawLine(2,5, 9,5) ' top left
				g.drawLine(10,6, 14,6) ' top right
				' Draw the UP arrow
				'     same color as edge
				g.drawLine(8,13, 8,16) ' arrow shaft
				g.drawLine(8, 9, 8, 9) ' arrowhead top
				g.drawLine(7,10, 9,10)
				g.drawLine(6,11, 10,11)
				g.drawLine(5,12, 11,12)

				' Draw inner folder highlight
				g.color = MetalLookAndFeel.primaryControlHighlight
				g.drawLine(2,6, 2,13) ' left
				g.drawLine(3,6, 9,6) ' top left
				g.drawLine(10,7, 14,7) ' top right

				' Draw tab on folder
				g.color = MetalLookAndFeel.primaryControlDarkShadow
				g.drawLine(11,3, 15,3) ' top
				g.drawLine(10,4, 15,4) ' bottom

				g.translate(-x, -y)
			End Sub

			Public Overridable Property iconWidth As Integer Implements Icon.getIconWidth
				Get
					Return 18
				End Get
			End Property

			Public Overridable Property iconHeight As Integer Implements Icon.getIconHeight
				Get
					Return 18
				End Get
			End Property
		End Class ' End class FileChooserUpFolderIcon


		''' <summary>
		''' Defines an icon for Palette close
		''' @since 1.3
		''' </summary>
		<Serializable> _
		Public Class PaletteCloseIcon
			Implements Icon, javax.swing.plaf.UIResource

			Friend iconSize As Integer = 7

			Public Overridable Sub paintIcon(ByVal c As Component, ByVal g As Graphics, ByVal x As Integer, ByVal y As Integer)
				Dim parentButton As JButton = CType(c, JButton)
				Dim buttonModel As ButtonModel = parentButton.model

				Dim back As Color
				Dim highlight As Color = MetalLookAndFeel.primaryControlHighlight
				Dim shadow As Color = MetalLookAndFeel.primaryControlInfo
				If buttonModel.pressed AndAlso buttonModel.armed Then
					back = shadow
				Else
					back = MetalLookAndFeel.primaryControlDarkShadow
				End If

				g.translate(x, y)
				g.color = back
				g.drawLine(0, 1, 5, 6)
				g.drawLine(1, 0, 6, 5)
				g.drawLine(1, 1, 6, 6)
				g.drawLine(6, 1, 1, 6)
				g.drawLine(5,0, 0,5)
				g.drawLine(5,1, 1,5)

				g.color = highlight
				g.drawLine(6,2, 5,3)
				g.drawLine(2,6, 3, 5)
				g.drawLine(6,6,6,6)


				g.translate(-x, -y)
			End Sub

			Public Overridable Property iconWidth As Integer Implements Icon.getIconWidth
				Get
					Return iconSize
				End Get
			End Property

			Public Overridable Property iconHeight As Integer Implements Icon.getIconHeight
				Get
					Return iconSize
				End Get
			End Property
		End Class

		' Internal Frame Close code
		<Serializable> _
		Private Class InternalFrameCloseIcon
			Implements Icon, javax.swing.plaf.UIResource

			Friend iconSize As Integer = 16

			Public Sub New(ByVal size As Integer)
				iconSize = size
			End Sub

			Public Overridable Sub paintIcon(ByVal c As Component, ByVal g As Graphics, ByVal x As Integer, ByVal y As Integer)
				Dim parentButton As JButton = CType(c, JButton)
				Dim buttonModel As ButtonModel = parentButton.model

				Dim backgroundColor As Color = MetalLookAndFeel.primaryControl
				Dim internalBackgroundColor As Color = MetalLookAndFeel.primaryControl
				Dim mainItemColor As Color = MetalLookAndFeel.primaryControlDarkShadow
				Dim darkHighlightColor As Color = MetalLookAndFeel.black
				Dim xLightHighlightColor As Color = MetalLookAndFeel.white
				Dim boxLightHighlightColor As Color = MetalLookAndFeel.white

				' if the inactive window
				If parentButton.getClientProperty("paintActive") IsNot Boolean.TRUE Then
					backgroundColor = MetalLookAndFeel.control
					internalBackgroundColor = backgroundColor
					mainItemColor = MetalLookAndFeel.controlDarkShadow
					' if inactive and pressed
					If buttonModel.pressed AndAlso buttonModel.armed Then
						internalBackgroundColor = MetalLookAndFeel.controlShadow
						xLightHighlightColor = internalBackgroundColor
						mainItemColor = darkHighlightColor
					End If
				' if pressed
				ElseIf buttonModel.pressed AndAlso buttonModel.armed Then
					internalBackgroundColor = MetalLookAndFeel.primaryControlShadow
					xLightHighlightColor = internalBackgroundColor
					mainItemColor = darkHighlightColor
					' darkHighlightColor is still "getBlack()"
				End If

				' Some calculations that are needed more than once later on.
				Dim oneHalf As Integer = iconSize \ 2 ' 16 -> 8

				g.translate(x, y)

				' fill background
				g.color = backgroundColor
				g.fillRect(0,0, iconSize,iconSize)

				' fill inside of box area
				g.color = internalBackgroundColor
				g.fillRect(3,3, iconSize-6,iconSize-6)

				' THE BOX
				' the top/left dark higlight - some of this will get overwritten
				g.color = darkHighlightColor
				g.drawRect(1,1, iconSize-3,iconSize-3)
				' draw the inside bottom/right highlight
				g.drawRect(2,2, iconSize-5,iconSize-5)
				' draw the light/outside, bottom/right highlight
				g.color = boxLightHighlightColor
				g.drawRect(2,2, iconSize-3,iconSize-3)
				' draw the "normal" box
				g.color = mainItemColor
				g.drawRect(2,2, iconSize-4,iconSize-4)
				g.drawLine(3,iconSize-3, 3,iconSize-3) ' lower left
				g.drawLine(iconSize-3,3, iconSize-3,3) ' up right

				' THE "X"
				' Dark highlight
				g.color = darkHighlightColor
				g.drawLine(4,5, 5,4) ' far up left
				g.drawLine(4,iconSize-6, iconSize-6,4) ' against body of "X"
				' Light highlight
				g.color = xLightHighlightColor
				g.drawLine(6,iconSize-5, iconSize-5,6) ' against body of "X"
				  ' one pixel over from the body
				g.drawLine(oneHalf,oneHalf+2, oneHalf+2,oneHalf)
				  ' bottom right
				g.drawLine(iconSize-5,iconSize-5, iconSize-4,iconSize-5)
				g.drawLine(iconSize-5,iconSize-4, iconSize-5,iconSize-4)
				' Main color
				g.color = mainItemColor
				  ' Upper left to lower right
				g.drawLine(5,5, iconSize-6,iconSize-6) ' g.drawLine(5,5, 10,10);
				g.drawLine(6,5, iconSize-5,iconSize-6) ' g.drawLine(6,5, 11,10);
				g.drawLine(5,6, iconSize-6,iconSize-5) ' g.drawLine(5,6, 10,11);
				  ' Lower left to upper right
				g.drawLine(5,iconSize-5, iconSize-5,5) ' g.drawLine(5,11, 11,5);
				g.drawLine(5,iconSize-6, iconSize-6,5) ' g.drawLine(5,10, 10,5);

				g.translate(-x, -y)
			End Sub

			Public Overridable Property iconWidth As Integer Implements Icon.getIconWidth
				Get
					Return iconSize
				End Get
			End Property

			Public Overridable Property iconHeight As Integer Implements Icon.getIconHeight
				Get
					Return iconSize
				End Get
			End Property
		End Class ' End class InternalFrameCloseIcon

		' Internal Frame Alternate Maximize code (actually, the un-maximize icon)
		<Serializable> _
		Private Class InternalFrameAltMaximizeIcon
			Implements Icon, javax.swing.plaf.UIResource

			Friend iconSize As Integer = 16

			Public Sub New(ByVal size As Integer)
				iconSize = size
			End Sub

			Public Overridable Sub paintIcon(ByVal c As Component, ByVal g As Graphics, ByVal x As Integer, ByVal y As Integer)
				Dim parentButton As JButton = CType(c, JButton)
				Dim buttonModel As ButtonModel = parentButton.model

				Dim backgroundColor As Color = MetalLookAndFeel.primaryControl
				Dim internalBackgroundColor As Color = MetalLookAndFeel.primaryControl
				Dim mainItemColor As Color = MetalLookAndFeel.primaryControlDarkShadow
				Dim darkHighlightColor As Color = MetalLookAndFeel.black
				' ul = Upper Left and lr = Lower Right
				Dim ulLightHighlightColor As Color = MetalLookAndFeel.white
				Dim lrLightHighlightColor As Color = MetalLookAndFeel.white

				' if the internal frame is inactive
				If parentButton.getClientProperty("paintActive") IsNot Boolean.TRUE Then
					backgroundColor = MetalLookAndFeel.control
					internalBackgroundColor = backgroundColor
					mainItemColor = MetalLookAndFeel.controlDarkShadow
					' if inactive and pressed
					If buttonModel.pressed AndAlso buttonModel.armed Then
						internalBackgroundColor = MetalLookAndFeel.controlShadow
						ulLightHighlightColor = internalBackgroundColor
						mainItemColor = darkHighlightColor
					End If
				' if the button is pressed and the mouse is over it
				ElseIf buttonModel.pressed AndAlso buttonModel.armed Then
					internalBackgroundColor = MetalLookAndFeel.primaryControlShadow
					ulLightHighlightColor = internalBackgroundColor
					mainItemColor = darkHighlightColor
					' darkHighlightColor is still "getBlack()"
				End If

				g.translate(x, y)

				' fill background
				g.color = backgroundColor
				g.fillRect(0,0, iconSize,iconSize)

				' BOX
				' fill inside the box
				g.color = internalBackgroundColor
				g.fillRect(3,6, iconSize-9,iconSize-9)

				' draw dark highlight color
				g.color = darkHighlightColor
				g.drawRect(1,5, iconSize-8,iconSize-8)
				g.drawLine(1,iconSize-2, 1,iconSize-2) ' extra pixel on bottom

				' draw lower right light highlight
				g.color = lrLightHighlightColor
				g.drawRect(2,6, iconSize-7,iconSize-7)
				' draw upper left light highlight
				g.color = ulLightHighlightColor
				g.drawRect(3,7, iconSize-9,iconSize-9)

				' draw the main box
				g.color = mainItemColor
				g.drawRect(2,6, iconSize-8,iconSize-8)

				' Six extraneous pixels to deal with
				g.color = ulLightHighlightColor
				g.drawLine(iconSize-6,8,iconSize-6,8)
				g.drawLine(iconSize-9,6, iconSize-7,8)
				g.color = mainItemColor
				g.drawLine(3,iconSize-3,3,iconSize-3)
				g.color = darkHighlightColor
				g.drawLine(iconSize-6,9,iconSize-6,9)
				g.color = backgroundColor
				g.drawLine(iconSize-9,5,iconSize-9,5)

				' ARROW
				' do the shaft first
				g.color = mainItemColor
				g.fillRect(iconSize-7,3, 3,5) ' do a big block
				g.drawLine(iconSize-6,5, iconSize-3,2) ' top shaft
				g.drawLine(iconSize-6,6, iconSize-2,2) ' bottom shaft
				g.drawLine(iconSize-6,7, iconSize-3,7) ' bottom arrow head

				' draw the dark highlight
				g.color = darkHighlightColor
				g.drawLine(iconSize-8,2, iconSize-7,2) ' top of arrowhead
				g.drawLine(iconSize-8,3, iconSize-8,7) ' left of arrowhead
				g.drawLine(iconSize-6,4, iconSize-3,1) ' top of shaft
				g.drawLine(iconSize-4,6, iconSize-3,6) ' top,right of arrowhead

				' draw the light highlight
				g.color = lrLightHighlightColor
				g.drawLine(iconSize-6,3, iconSize-6,3) ' top
				g.drawLine(iconSize-4,5, iconSize-2,3) ' under shaft
				g.drawLine(iconSize-4,8, iconSize-3,8) ' under arrowhead
				g.drawLine(iconSize-2,8, iconSize-2,7) ' right of arrowhead

				g.translate(-x, -y)
			End Sub

			Public Overridable Property iconWidth As Integer Implements Icon.getIconWidth
				Get
					Return iconSize
				End Get
			End Property

			Public Overridable Property iconHeight As Integer Implements Icon.getIconHeight
				Get
					Return iconSize
				End Get
			End Property
		End Class ' End class InternalFrameAltMaximizeIcon

		' Code for the default icons that goes in the upper left corner
		<Serializable> _
		Private Class InternalFrameDefaultMenuIcon
			Implements Icon, javax.swing.plaf.UIResource

			Public Overridable Sub paintIcon(ByVal c As Component, ByVal g As Graphics, ByVal x As Integer, ByVal y As Integer)

				Dim windowBodyColor As Color = MetalLookAndFeel.windowBackground
				Dim titleColor As Color = MetalLookAndFeel.primaryControl
				Dim edgeColor As Color = MetalLookAndFeel.primaryControlDarkShadow

				g.translate(x, y)

				' draw background color for title area
				' catch four corners and title area
				g.color = titleColor
				g.fillRect(0,0, 16,16)

				' fill body of window
				g.color = windowBodyColor
				g.fillRect(2,6, 13,9)
				' draw light parts of two "bumps"
				g.drawLine(2,2, 2,2)
				g.drawLine(5,2, 5,2)
				g.drawLine(8,2, 8,2)
				g.drawLine(11,2, 11,2)

				' draw line around edge of title and icon
				g.color = edgeColor
				g.drawRect(1,1, 13,13) ' entire inner edge
				g.drawLine(1,0, 14,0) ' top outter edge
				g.drawLine(15,1, 15,14) ' right outter edge
				g.drawLine(1,15, 14,15) ' bottom outter edge
				g.drawLine(0,1, 0,14) ' left outter edge
				g.drawLine(2,5, 13,5) ' bottom of title bar area
				' draw dark part of four "bumps" (same color)
				g.drawLine(3,3, 3,3)
				g.drawLine(6,3, 6,3)
				g.drawLine(9,3, 9,3)
				g.drawLine(12,3, 12,3)

				g.translate(-x, -y)
			End Sub

			Public Overridable Property iconWidth As Integer Implements Icon.getIconWidth
				Get
					Return 16
				End Get
			End Property

			Public Overridable Property iconHeight As Integer Implements Icon.getIconHeight
				Get
					Return 16
				End Get
			End Property
		End Class ' End class InternalFrameDefaultMenuIcon

		' Internal Frame Maximize code
		<Serializable> _
		Private Class InternalFrameMaximizeIcon
			Implements Icon, javax.swing.plaf.UIResource

			Protected Friend iconSize As Integer = 16

			Public Sub New(ByVal size As Integer)
				iconSize = size
			End Sub

			Public Overridable Sub paintIcon(ByVal c As Component, ByVal g As Graphics, ByVal x As Integer, ByVal y As Integer)
				Dim parentButton As JButton = CType(c, JButton)
				Dim buttonModel As ButtonModel = parentButton.model

				Dim backgroundColor As Color = MetalLookAndFeel.primaryControl
				Dim internalBackgroundColor As Color = MetalLookAndFeel.primaryControl
				Dim mainItemColor As Color = MetalLookAndFeel.primaryControlDarkShadow
				Dim darkHighlightColor As Color = MetalLookAndFeel.black
				' ul = Upper Left and lr = Lower Right
				Dim ulLightHighlightColor As Color = MetalLookAndFeel.white
				Dim lrLightHighlightColor As Color = MetalLookAndFeel.white

				' if the internal frame is inactive
				If parentButton.getClientProperty("paintActive") IsNot Boolean.TRUE Then
					backgroundColor = MetalLookAndFeel.control
					internalBackgroundColor = backgroundColor
					mainItemColor = MetalLookAndFeel.controlDarkShadow
					' if inactive and pressed
					If buttonModel.pressed AndAlso buttonModel.armed Then
						internalBackgroundColor = MetalLookAndFeel.controlShadow
						ulLightHighlightColor = internalBackgroundColor
						mainItemColor = darkHighlightColor
					End If
				' if the button is pressed and the mouse is over it
				ElseIf buttonModel.pressed AndAlso buttonModel.armed Then
					internalBackgroundColor = MetalLookAndFeel.primaryControlShadow
					ulLightHighlightColor = internalBackgroundColor
					mainItemColor = darkHighlightColor
					' darkHighlightColor is still "getBlack()"
				End If

				g.translate(x, y)

				' fill background
				g.color = backgroundColor
				g.fillRect(0,0, iconSize,iconSize)

				' BOX drawing
				' fill inside the box
				g.color = internalBackgroundColor
				g.fillRect(3,7, iconSize-10,iconSize-10)

				' light highlight
				g.color = ulLightHighlightColor
				g.drawRect(3,7, iconSize-10,iconSize-10) ' up,left
				g.color = lrLightHighlightColor
				g.drawRect(2,6, iconSize-7,iconSize-7) ' low,right
				' dark highlight
				g.color = darkHighlightColor
				g.drawRect(1,5, iconSize-7,iconSize-7) ' outer
				g.drawRect(2,6, iconSize-9,iconSize-9) ' inner
				' main box
				g.color = mainItemColor
				g.drawRect(2,6, iconSize-8,iconSize-8) ' g.drawRect(2,6, 8,8);

				' ARROW drawing
				' dark highlight
				g.color = darkHighlightColor
				  ' down,left to up,right - inside box
				g.drawLine(3,iconSize-5, iconSize-9,7)
				  ' down,left to up,right - outside box
				g.drawLine(iconSize-6,4, iconSize-5,3)
				  ' outside edge of arrow head
				g.drawLine(iconSize-7,1, iconSize-7,2)
				  ' outside edge of arrow head
				g.drawLine(iconSize-6,1, iconSize-2,1)
				' light highlight
				g.color = ulLightHighlightColor
				  ' down,left to up,right - inside box
				g.drawLine(5,iconSize-4, iconSize-8,9)
				g.color = lrLightHighlightColor
				g.drawLine(iconSize-6,3, iconSize-4,5) ' outside box
				g.drawLine(iconSize-4,5, iconSize-4,6) ' one down from this
				g.drawLine(iconSize-2,7, iconSize-1,7) ' outside edge arrow head
				g.drawLine(iconSize-1,2, iconSize-1,6) ' outside edge arrow head
				' main part of arrow
				g.color = mainItemColor
				g.drawLine(3,iconSize-4, iconSize-3,2) ' top edge of staff
				g.drawLine(3,iconSize-3, iconSize-2,2) ' bottom edge of staff
				g.drawLine(4,iconSize-3, 5,iconSize-3) ' highlights inside of box
				g.drawLine(iconSize-7,8, iconSize-7,9) ' highlights inside of box
				g.drawLine(iconSize-6,2, iconSize-4,2) ' top of arrow head
				g.drawRect(iconSize-3,3, 1,3) ' right of arrow head

				g.translate(-x, -y)
			End Sub

			Public Overridable Property iconWidth As Integer Implements Icon.getIconWidth
				Get
					Return iconSize
				End Get
			End Property

			Public Overridable Property iconHeight As Integer Implements Icon.getIconHeight
				Get
					Return iconSize
				End Get
			End Property
		End Class ' End class InternalFrameMaximizeIcon

		' Internal Frame Minimize code
		<Serializable> _
		Private Class InternalFrameMinimizeIcon
			Implements Icon, javax.swing.plaf.UIResource

			Friend iconSize As Integer = 16

			Public Sub New(ByVal size As Integer)
				iconSize = size
			End Sub

			Public Overridable Sub paintIcon(ByVal c As Component, ByVal g As Graphics, ByVal x As Integer, ByVal y As Integer)
				Dim parentButton As JButton = CType(c, JButton)
				Dim buttonModel As ButtonModel = parentButton.model


				Dim backgroundColor As Color = MetalLookAndFeel.primaryControl
				Dim internalBackgroundColor As Color = MetalLookAndFeel.primaryControl
				Dim mainItemColor As Color = MetalLookAndFeel.primaryControlDarkShadow
				Dim darkHighlightColor As Color = MetalLookAndFeel.black
				' ul = Upper Left and lr = Lower Right
				Dim ulLightHighlightColor As Color = MetalLookAndFeel.white
				Dim lrLightHighlightColor As Color = MetalLookAndFeel.white

				' if the internal frame is inactive
				If parentButton.getClientProperty("paintActive") IsNot Boolean.TRUE Then
					backgroundColor = MetalLookAndFeel.control
					internalBackgroundColor = backgroundColor
					mainItemColor = MetalLookAndFeel.controlDarkShadow
					' if inactive and pressed
					If buttonModel.pressed AndAlso buttonModel.armed Then
						internalBackgroundColor = MetalLookAndFeel.controlShadow
						ulLightHighlightColor = internalBackgroundColor
						mainItemColor = darkHighlightColor
					End If
				' if the button is pressed and the mouse is over it
				ElseIf buttonModel.pressed AndAlso buttonModel.armed Then
					internalBackgroundColor = MetalLookAndFeel.primaryControlShadow
					ulLightHighlightColor = internalBackgroundColor
					mainItemColor = darkHighlightColor
					' darkHighlightColor is still "getBlack()"
				End If

				g.translate(x, y)

				' fill background
				g.color = backgroundColor
				g.fillRect(0,0, iconSize,iconSize)

				' BOX drawing
				' fill inside the box
				g.color = internalBackgroundColor
				g.fillRect(4,11, iconSize-13,iconSize-13)
				' light highlight
				g.color = lrLightHighlightColor
				g.drawRect(2,10, iconSize-10,iconSize-11) ' low,right
				g.color = ulLightHighlightColor
				g.drawRect(3,10, iconSize-12,iconSize-12) ' up,left
				' dark highlight
				g.color = darkHighlightColor
				g.drawRect(1,8, iconSize-10,iconSize-10) ' outer
				g.drawRect(2,9, iconSize-12,iconSize-12) ' inner
				' main box
				g.color = mainItemColor
				g.drawRect(2,9, iconSize-11,iconSize-11)
				g.drawLine(iconSize-10,10, iconSize-10,10) ' up right highlight
				g.drawLine(3,iconSize-3, 3,iconSize-3) ' low left highlight

				' ARROW
				' do the shaft first
				g.color = mainItemColor
				g.fillRect(iconSize-7,3, 3,5) ' do a big block
				g.drawLine(iconSize-6,5, iconSize-3,2) ' top shaft
				g.drawLine(iconSize-6,6, iconSize-2,2) ' bottom shaft
				g.drawLine(iconSize-6,7, iconSize-3,7) ' bottom arrow head

				' draw the dark highlight
				g.color = darkHighlightColor
				g.drawLine(iconSize-8,2, iconSize-7,2) ' top of arrowhead
				g.drawLine(iconSize-8,3, iconSize-8,7) ' left of arrowhead
				g.drawLine(iconSize-6,4, iconSize-3,1) ' top of shaft
				g.drawLine(iconSize-4,6, iconSize-3,6) ' top,right of arrowhead

				' draw the light highlight
				g.color = lrLightHighlightColor
				g.drawLine(iconSize-6,3, iconSize-6,3) ' top
				g.drawLine(iconSize-4,5, iconSize-2,3) ' under shaft
				g.drawLine(iconSize-7,8, iconSize-3,8) ' under arrowhead
				g.drawLine(iconSize-2,8, iconSize-2,7) ' right of arrowhead

				g.translate(-x, -y)
			End Sub

			Public Overridable Property iconWidth As Integer Implements Icon.getIconWidth
				Get
					Return iconSize
				End Get
			End Property

			Public Overridable Property iconHeight As Integer Implements Icon.getIconHeight
				Get
					Return iconSize
				End Get
			End Property
		End Class ' End class InternalFrameMinimizeIcon

		<Serializable> _
		Private Class CheckBoxIcon
			Implements Icon, javax.swing.plaf.UIResource

			Protected Friend Overridable Property controlSize As Integer
				Get
					Return 13
				End Get
			End Property

			Private Sub paintOceanIcon(ByVal c As Component, ByVal g As Graphics, ByVal x As Integer, ByVal y As Integer)
				Dim model As ButtonModel = CType(c, JCheckBox).model

				g.translate(x, y)
				Dim w As Integer = iconWidth
				Dim h As Integer = iconHeight
				If model.enabled Then
					If model.pressed AndAlso model.armed Then
						g.color = MetalLookAndFeel.controlShadow
						g.fillRect(0, 0, w, h)
						g.color = MetalLookAndFeel.controlDarkShadow
						g.fillRect(0, 0, w, 2)
						g.fillRect(0, 2, 2, h - 2)
						g.fillRect(w - 1, 1, 1, h - 1)
						g.fillRect(1, h - 1, w - 2, 1)
					ElseIf model.rollover Then
						MetalUtils.drawGradient(c, g, "CheckBox.gradient", 0, 0, w, h, True)
						g.color = MetalLookAndFeel.controlDarkShadow
						g.drawRect(0, 0, w - 1, h - 1)
						g.color = MetalLookAndFeel.primaryControl
						g.drawRect(1, 1, w - 3, h - 3)
						g.drawRect(2, 2, w - 5, h - 5)
					Else
						MetalUtils.drawGradient(c, g, "CheckBox.gradient", 0, 0, w, h, True)
						g.color = MetalLookAndFeel.controlDarkShadow
						g.drawRect(0, 0, w - 1, h - 1)
					End If
					g.color = MetalLookAndFeel.controlInfo
				Else
					g.color = MetalLookAndFeel.controlDarkShadow
					g.drawRect(0, 0, w - 1, h - 1)
				End If
				g.translate(-x, -y)
				If model.selected Then drawCheck(c,g,x,y)
			End Sub

			Public Overridable Sub paintIcon(ByVal c As Component, ByVal g As Graphics, ByVal x As Integer, ByVal y As Integer)
				If MetalLookAndFeel.usingOcean() Then
					paintOceanIcon(c, g, x, y)
					Return
				End If
				Dim model As ButtonModel = CType(c, JCheckBox).model
				Dim ___controlSize As Integer = controlSize

				If model.enabled Then
					If model.pressed AndAlso model.armed Then
						g.color = MetalLookAndFeel.controlShadow
						g.fillRect(x, y, ___controlSize-1, ___controlSize-1)
						MetalUtils.drawPressed3DBorder(g, x, y, ___controlSize, ___controlSize)
					Else
						MetalUtils.drawFlush3DBorder(g, x, y, ___controlSize, ___controlSize)
					End If
					g.color = c.foreground
				Else
					g.color = MetalLookAndFeel.controlShadow
					g.drawRect(x, y, ___controlSize-2, ___controlSize-2)
				End If

				If model.selected Then drawCheck(c,g,x,y)

			End Sub

			Protected Friend Overridable Sub drawCheck(ByVal c As Component, ByVal g As Graphics, ByVal x As Integer, ByVal y As Integer)
				Dim ___controlSize As Integer = controlSize
				g.fillRect(x+3, y+5, 2, ___controlSize-8)
				g.drawLine(x+(___controlSize-4), y+3, x+5, y+(___controlSize-6))
				g.drawLine(x+(___controlSize-4), y+4, x+5, y+(___controlSize-5))
			End Sub

			Public Overridable Property iconWidth As Integer Implements Icon.getIconWidth
				Get
					Return controlSize
				End Get
			End Property

			Public Overridable Property iconHeight As Integer Implements Icon.getIconHeight
				Get
					Return controlSize
				End Get
			End Property
		End Class ' End class CheckBoxIcon

		' Radio button code
		<Serializable> _
		Private Class RadioButtonIcon
			Implements Icon, javax.swing.plaf.UIResource

			Public Overridable Sub paintOceanIcon(ByVal c As Component, ByVal g As Graphics, ByVal x As Integer, ByVal y As Integer)
				Dim model As ButtonModel = CType(c, JRadioButton).model
				Dim enabled As Boolean = model.enabled
				Dim pressed As Boolean = (enabled AndAlso model.pressed AndAlso model.armed)
				Dim rollover As Boolean = (enabled AndAlso model.rollover)

				g.translate(x, y)
				If enabled AndAlso (Not pressed) Then
					' PENDING: this isn't quite right, when we're sure it won't
					' change it needs to be cleaned.
					MetalUtils.drawGradient(c, g, "RadioButton.gradient", 1, 1, 10, 10, True)
					g.color = c.background
					g.fillRect(1, 1, 1, 1)
					g.fillRect(10, 1, 1, 1)
					g.fillRect(1, 10, 1, 1)
					g.fillRect(10, 10, 1, 1)
				ElseIf pressed OrElse (Not enabled) Then
					If pressed Then
						g.color = MetalLookAndFeel.primaryControl
					Else
						g.color = MetalLookAndFeel.control
					End If
					g.fillRect(2, 2, 8, 8)
					g.fillRect(4, 1, 4, 1)
					g.fillRect(4, 10, 4, 1)
					g.fillRect(1, 4, 1, 4)
					g.fillRect(10, 4, 1, 4)
				End If

				' draw Dark Circle (start at top, go clockwise)
				If Not enabled Then
					g.color = MetalLookAndFeel.inactiveControlTextColor
				Else
					g.color = MetalLookAndFeel.controlDarkShadow
				End If
				g.drawLine(4, 0, 7, 0)
				g.drawLine(8, 1, 9, 1)
				g.drawLine(10, 2, 10, 3)
				g.drawLine(11, 4, 11, 7)
				g.drawLine(10, 8, 10, 9)
				g.drawLine(9,10, 8,10)
				g.drawLine(7,11, 4,11)
				g.drawLine(3,10, 2,10)
				g.drawLine(1, 9, 1, 8)
				g.drawLine(0, 7, 0, 4)
				g.drawLine(1, 3, 1, 2)
				g.drawLine(2, 1, 3, 1)

				If pressed Then
					g.fillRect(1, 4, 1, 4)
					g.fillRect(2, 2, 1, 2)
					g.fillRect(3, 2, 1, 1)
					g.fillRect(4, 1, 4, 1)
				ElseIf rollover Then
					g.color = MetalLookAndFeel.primaryControl
					g.fillRect(4, 1, 4, 2)
					g.fillRect(8, 2, 2, 2)
					g.fillRect(9, 4, 2, 4)
					g.fillRect(8, 8, 2, 2)
					g.fillRect(4, 9, 4, 2)
					g.fillRect(2, 8, 2, 2)
					g.fillRect(1, 4, 2, 4)
					g.fillRect(2, 2, 2, 2)
				End If

				' selected dot
				If model.selected Then
					If enabled Then
						g.color = MetalLookAndFeel.controlInfo
					Else
						g.color = MetalLookAndFeel.controlDarkShadow
					End If
					g.fillRect(4, 4, 4, 4)
					g.drawLine(4, 3, 7, 3)
					g.drawLine(8, 4, 8, 7)
					g.drawLine(7, 8, 4, 8)
					g.drawLine(3, 7, 3, 4)
				End If

				g.translate(-x, -y)
			End Sub

			Public Overridable Sub paintIcon(ByVal c As Component, ByVal g As Graphics, ByVal x As Integer, ByVal y As Integer)
				If MetalLookAndFeel.usingOcean() Then
					paintOceanIcon(c, g, x, y)
					Return
				End If
				Dim rb As JRadioButton = CType(c, JRadioButton)
				Dim model As ButtonModel = rb.model
				Dim drawDot As Boolean = model.selected

				Dim background As Color = c.background
				Dim dotColor As Color = c.foreground
				Dim shadow As Color = MetalLookAndFeel.controlShadow
				Dim darkCircle As Color = MetalLookAndFeel.controlDarkShadow
				Dim whiteInnerLeftArc As Color = MetalLookAndFeel.controlHighlight
				Dim whiteOuterRightArc As Color = MetalLookAndFeel.controlHighlight
				Dim interiorColor As Color = background

				' Set up colors per RadioButtonModel condition
				If Not model.enabled Then
						whiteOuterRightArc = background
						whiteInnerLeftArc = whiteOuterRightArc
						dotColor = shadow
						darkCircle = dotColor
				ElseIf model.pressed AndAlso model.armed Then
						interiorColor = shadow
						whiteInnerLeftArc = interiorColor
				End If

				g.translate(x, y)

				' fill interior
				g.color = interiorColor
				g.fillRect(2,2, 9,9)

				' draw Dark Circle (start at top, go clockwise)
				g.color = darkCircle
				g.drawLine(4, 0, 7, 0)
				g.drawLine(8, 1, 9, 1)
				g.drawLine(10, 2, 10, 3)
				g.drawLine(11, 4, 11, 7)
				g.drawLine(10, 8, 10, 9)
				g.drawLine(9,10, 8,10)
				g.drawLine(7,11, 4,11)
				g.drawLine(3,10, 2,10)
				g.drawLine(1, 9, 1, 8)
				g.drawLine(0, 7, 0, 4)
				g.drawLine(1, 3, 1, 2)
				g.drawLine(2, 1, 3, 1)

				' draw Inner Left (usually) White Arc
				'  start at lower left corner, go clockwise
				g.color = whiteInnerLeftArc
				g.drawLine(2, 9, 2, 8)
				g.drawLine(1, 7, 1, 4)
				g.drawLine(2, 2, 2, 3)
				g.drawLine(2, 2, 3, 2)
				g.drawLine(4, 1, 7, 1)
				g.drawLine(8, 2, 9, 2)
				' draw Outer Right White Arc
				'  start at upper right corner, go clockwise
				g.color = whiteOuterRightArc
				g.drawLine(10, 1, 10, 1)
				g.drawLine(11, 2, 11, 3)
				g.drawLine(12, 4, 12, 7)
				g.drawLine(11, 8, 11, 9)
				g.drawLine(10,10, 10,10)
				g.drawLine(9,11, 8,11)
				g.drawLine(7,12, 4,12)
				g.drawLine(3,11, 2,11)

				' selected dot
				If drawDot Then
					g.color = dotColor
					g.fillRect(4, 4, 4, 4)
					g.drawLine(4, 3, 7, 3)
					g.drawLine(8, 4, 8, 7)
					g.drawLine(7, 8, 4, 8)
					g.drawLine(3, 7, 3, 4)
				End If

				g.translate(-x, -y)
			End Sub

			Public Overridable Property iconWidth As Integer Implements Icon.getIconWidth
				Get
					Return 13
				End Get
			End Property

			Public Overridable Property iconHeight As Integer Implements Icon.getIconHeight
				Get
					Return 13
				End Get
			End Property
		End Class ' End class RadioButtonIcon

		' Tree Computer Icon code
		<Serializable> _
		Private Class TreeComputerIcon
			Implements Icon, javax.swing.plaf.UIResource

			Public Overridable Sub paintIcon(ByVal c As Component, ByVal g As Graphics, ByVal x As Integer, ByVal y As Integer)
				g.translate(x, y)

				' Fill glass portion of monitor
				g.color = MetalLookAndFeel.primaryControl
				g.fillRect(5,4, 6,4)

				' Draw outside edge of monitor
				g.color = MetalLookAndFeel.primaryControlInfo
				g.drawLine(2,2, 2,8) ' left
				g.drawLine(13,2, 13,8) ' right
				g.drawLine(3,1, 12,1) ' top
				g.drawLine(12,9, 12,9) ' bottom right base
				g.drawLine(3,9, 3,9) ' bottom left base
				' Draw the edge of the glass
				g.drawLine(4,4, 4,7) ' left
				g.drawLine(5,3, 10,3) ' top
				g.drawLine(11,4, 11,7) ' right
				g.drawLine(5,8, 10,8) ' bottom
				' Draw the edge of the CPU
				g.drawLine(1,10, 14,10) ' top
				g.drawLine(14,10, 14,14) ' right
				g.drawLine(1,14, 14,14) ' bottom
				g.drawLine(1,10, 1,14) ' left

				' Draw the disk drives
				g.color = MetalLookAndFeel.controlDarkShadow
				g.drawLine(6,12, 8,12) ' left
				g.drawLine(10,12, 12,12) ' right

				g.translate(-x, -y)
			End Sub

			Public Overridable Property iconWidth As Integer Implements Icon.getIconWidth
				Get
					Return 16
				End Get
			End Property

			Public Overridable Property iconHeight As Integer Implements Icon.getIconHeight
				Get
					Return 16
				End Get
			End Property
		End Class ' End class TreeComputerIcon

		' Tree HardDrive Icon code
		<Serializable> _
		Private Class TreeHardDriveIcon
			Implements Icon, javax.swing.plaf.UIResource

			Public Overridable Sub paintIcon(ByVal c As Component, ByVal g As Graphics, ByVal x As Integer, ByVal y As Integer)
				g.translate(x, y)

				' Draw edges of the disks
				g.color = MetalLookAndFeel.primaryControlInfo
				'     top disk
				g.drawLine(1,4, 1,5) ' left
				g.drawLine(2,3, 3,3)
				g.drawLine(4,2, 11,2) ' top
				g.drawLine(12,3, 13,3)
				g.drawLine(14,4, 14,5) ' right
				g.drawLine(12,6, 13,6)
				g.drawLine(4,7, 11,7) ' bottom
				g.drawLine(2,6, 3,6)
				'     middle disk
				g.drawLine(1,7, 1,8) ' left
				g.drawLine(2,9, 3,9)
				g.drawLine(4,10, 11,10) ' bottom
				g.drawLine(12,9, 13,9)
				g.drawLine(14,7, 14, 8) ' right
				'     bottom disk
				g.drawLine(1,10, 1,11) ' left
				g.drawLine(2,12, 3,12)
				g.drawLine(4,13, 11,13) ' bottom
				g.drawLine(12,12, 13,12)
				g.drawLine(14,10, 14,11) ' right

				' Draw the down right shadows
				g.color = MetalLookAndFeel.controlShadow
				'     top disk
				g.drawLine(7,6, 7,6)
				g.drawLine(9,6, 9,6)
				g.drawLine(10,5, 10,5)
				g.drawLine(11,6, 11,6)
				g.drawLine(12,5, 13,5)
				g.drawLine(13,4, 13,4)
				'     middle disk
				g.drawLine(7,9, 7,9)
				g.drawLine(9,9, 9,9)
				g.drawLine(10,8, 10,8)
				g.drawLine(11,9, 11,9)
				g.drawLine(12,8, 13,8)
				g.drawLine(13,7, 13,7)
				'     bottom disk
				g.drawLine(7,12, 7,12)
				g.drawLine(9,12, 9,12)
				g.drawLine(10,11, 10,11)
				g.drawLine(11,12, 11,12)
				g.drawLine(12,11, 13,11)
				g.drawLine(13,10, 13,10)

				' Draw the up left highlight
				g.color = MetalLookAndFeel.controlHighlight
				'     top disk
				g.drawLine(4,3, 5,3)
				g.drawLine(7,3, 9,3)
				g.drawLine(11,3, 11,3)
				g.drawLine(2,4, 6,4)
				g.drawLine(8,4, 8,4)
				g.drawLine(2,5, 3,5)
				g.drawLine(4,6, 4,6)
				'     middle disk
				g.drawLine(2,7, 3,7)
				g.drawLine(2,8, 3,8)
				g.drawLine(4,9, 4,9)
				'     bottom disk
				g.drawLine(2,10, 3,10)
				g.drawLine(2,11, 3,11)
				g.drawLine(4,12, 4,12)

				g.translate(-x, -y)
			End Sub

			Public Overridable Property iconWidth As Integer Implements Icon.getIconWidth
				Get
					Return 16
				End Get
			End Property

			Public Overridable Property iconHeight As Integer Implements Icon.getIconHeight
				Get
					Return 16
				End Get
			End Property
		End Class ' End class TreeHardDriveIcon

		' Tree FloppyDrive Icon code
		<Serializable> _
		Private Class TreeFloppyDriveIcon
			Implements Icon, javax.swing.plaf.UIResource

			Public Overridable Sub paintIcon(ByVal c As Component, ByVal g As Graphics, ByVal x As Integer, ByVal y As Integer)
				g.translate(x, y)

				' Fill body of floppy
				g.color = MetalLookAndFeel.primaryControl
				g.fillRect(2,2, 12,12)

				' Draw outside edge of floppy
				g.color = MetalLookAndFeel.primaryControlInfo
				g.drawLine(1, 1, 13, 1) ' top
				g.drawLine(14, 2, 14,14) ' right
				g.drawLine(1,14, 14,14) ' bottom
				g.drawLine(1, 1, 1,14) ' left

				' Draw grey-ish highlights
				g.color = MetalLookAndFeel.controlDarkShadow
				g.fillRect(5,2, 6,5) ' metal disk protector part
				g.drawLine(4,8, 11,8) ' top of label
				g.drawLine(3,9, 3,13) ' left of label
				g.drawLine(12,9, 12,13) ' right of label

				' Draw label and exposed disk
				g.color = MetalLookAndFeel.primaryControlHighlight
				g.fillRect(8,3, 2,3) ' exposed disk
				g.fillRect(4,9, 8,5) ' label

				' Draw text on label
				g.color = MetalLookAndFeel.primaryControlShadow
				g.drawLine(5,10, 9,10)
				g.drawLine(5,12, 8,12)

				g.translate(-x, -y)
			End Sub

			Public Overridable Property iconWidth As Integer Implements Icon.getIconWidth
				Get
					Return 16
				End Get
			End Property

			Public Overridable Property iconHeight As Integer Implements Icon.getIconHeight
				Get
					Return 16
				End Get
			End Property
		End Class ' End class TreeFloppyDriveIcon


		Private Shared ReadOnly folderIcon16Size As New Dimension(16, 16)

		''' <summary>
		''' Utility class for caching icon images.  This is necessary because
		''' we need a new image whenever we are rendering into a new
		''' GraphicsConfiguration, but we do not want to keep recreating icon
		''' images for GC's that we have already seen (for example,
		''' dragging a window back and forth between monitors on a multimon
		''' system, or drawing an icon to different Components that have different
		''' GC's).
		''' So now whenever we create a new icon image for a given GC, we
		''' cache that image with the GC for later retrieval.
		''' </summary>
		Friend Class ImageCacher

			' PENDING: Replace this class with CachedPainter.

			Friend images As New List(Of ImageGcPair)(1, 1)
			Friend currentImageGcPair As ImageGcPair

			Friend Class ImageGcPair
				Private ReadOnly outerInstance As MetalIconFactory.ImageCacher

				Friend image As Image
				Friend gc As GraphicsConfiguration
				Friend Sub New(ByVal outerInstance As MetalIconFactory.ImageCacher, ByVal image As Image, ByVal gc As GraphicsConfiguration)
						Me.outerInstance = outerInstance
					Me.image = image
					Me.gc = gc
				End Sub

				Friend Overridable Function hasSameConfiguration(ByVal newGC As GraphicsConfiguration) As Boolean
					Return ((newGC IsNot Nothing) AndAlso (newGC.Equals(gc))) OrElse ((newGC Is Nothing) AndAlso (gc Is Nothing))
				End Function

			End Class

			Friend Overridable Function getImage(ByVal newGC As GraphicsConfiguration) As Image
				If (currentImageGcPair Is Nothing) OrElse Not(currentImageGcPair.hasSameConfiguration(newGC)) Then
					For Each imgGcPair As ImageGcPair In images
						If imgGcPair.hasSameConfiguration(newGC) Then
							currentImageGcPair = imgGcPair
							Return imgGcPair.image
						End If
					Next imgGcPair
					Return Nothing
				End If
				Return currentImageGcPair.image
			End Function

			Friend Overridable Sub cacheImage(ByVal image As Image, ByVal gc As GraphicsConfiguration)
				Dim imgGcPair As New ImageGcPair(Me, image, gc)
				images.Add(imgGcPair)
				currentImageGcPair = imgGcPair
			End Sub

		End Class

		''' <summary>
		''' <p>
		''' <strong>Warning:</strong>
		''' Serialized objects of this class will not be compatible with
		''' future Swing releases. The current serialization support is
		''' appropriate for short term storage or RMI between applications running
		''' the same version of Swing.  As of 1.4, support for long term storage
		''' of all JavaBeans&trade;
		''' has been added to the <code>java.beans</code> package.
		''' Please see <seealso cref="java.beans.XMLEncoder"/>.
		''' </summary>
		<Serializable> _
		Public Class FolderIcon16
			Implements Icon

			Friend imageCacher As ImageCacher

			Public Overridable Sub paintIcon(ByVal c As Component, ByVal g As Graphics, ByVal x As Integer, ByVal y As Integer)
				Dim gc As GraphicsConfiguration = c.graphicsConfiguration
				If imageCacher Is Nothing Then imageCacher = New ImageCacher
				Dim image As Image = imageCacher.getImage(gc)
				If image Is Nothing Then
					If gc IsNot Nothing Then
						image = gc.createCompatibleImage(iconWidth, iconHeight, Transparency.BITMASK)
					Else
						image = New java.awt.image.BufferedImage(iconWidth, iconHeight, java.awt.image.BufferedImage.TYPE_INT_ARGB)
					End If
					Dim imageG As Graphics = image.graphics
					paintMe(c,imageG)
					imageG.Dispose()
					imageCacher.cacheImage(image, gc)
				End If
				g.drawImage(image, x, y+shift, Nothing)
			End Sub


			Private Sub paintMe(ByVal c As Component, ByVal g As Graphics)

				Dim right As Integer = folderIcon16Size.width - 1
				Dim bottom As Integer = folderIcon16Size.height - 1

				' Draw tab top
				g.color = MetalLookAndFeel.primaryControlDarkShadow
				g.drawLine(right - 5, 3, right, 3)
				g.drawLine(right - 6, 4, right, 4)

				' Draw folder front
				g.color = MetalLookAndFeel.primaryControl
				g.fillRect(2, 7, 13, 8)

				' Draw tab bottom
				g.color = MetalLookAndFeel.primaryControlShadow
				g.drawLine(right - 6, 5, right - 1, 5)

				' Draw outline
				g.color = MetalLookAndFeel.primaryControlInfo
				g.drawLine(0, 6, 0, bottom) ' left side
				g.drawLine(1, 5, right - 7, 5) ' first part of top
				g.drawLine(right - 6, 6, right - 1, 6) ' second part of top
				g.drawLine(right, 5, right, bottom) ' right side
				g.drawLine(0, bottom, right, bottom) ' bottom

				' Draw highlight
				g.color = MetalLookAndFeel.primaryControlHighlight
				g.drawLine(1, 6, 1, bottom - 1)
				g.drawLine(1, 6, right - 7, 6)
				g.drawLine(right - 6, 7, right - 1, 7)

			End Sub

			Public Overridable Property shift As Integer
				Get
					Return 0
				End Get
			End Property
			Public Overridable Property additionalHeight As Integer
				Get
					Return 0
				End Get
			End Property

			Public Overridable Property iconWidth As Integer Implements Icon.getIconWidth
				Get
					Return folderIcon16Size.width
				End Get
			End Property
			Public Overridable Property iconHeight As Integer Implements Icon.getIconHeight
				Get
					Return folderIcon16Size.height + additionalHeight
				End Get
			End Property
		End Class


		''' <summary>
		''' <p>
		''' <strong>Warning:</strong>
		''' Serialized objects of this class will not be compatible with
		''' future Swing releases. The current serialization support is
		''' appropriate for short term storage or RMI between applications running
		''' the same version of Swing.  As of 1.4, support for long term storage
		''' of all JavaBeans&trade;
		''' has been added to the <code>java.beans</code> package.
		''' Please see <seealso cref="java.beans.XMLEncoder"/>.
		''' </summary>
		Public Class TreeFolderIcon
			Inherits FolderIcon16

			Public Property Overrides shift As Integer
				Get
					Return -1
				End Get
			End Property
			Public Property Overrides additionalHeight As Integer
				Get
					Return 2
				End Get
			End Property
		End Class


		Private Shared ReadOnly fileIcon16Size As New Dimension(16, 16)

		''' <summary>
		''' <p>
		''' <strong>Warning:</strong>
		''' Serialized objects of this class will not be compatible with
		''' future Swing releases. The current serialization support is
		''' appropriate for short term storage or RMI between applications running
		''' the same version of Swing.  As of 1.4, support for long term storage
		''' of all JavaBeans&trade;
		''' has been added to the <code>java.beans</code> package.
		''' Please see <seealso cref="java.beans.XMLEncoder"/>.
		''' </summary>
		<Serializable> _
		Public Class FileIcon16
			Implements Icon

			Friend imageCacher As ImageCacher

			Public Overridable Sub paintIcon(ByVal c As Component, ByVal g As Graphics, ByVal x As Integer, ByVal y As Integer)
				Dim gc As GraphicsConfiguration = c.graphicsConfiguration
				If imageCacher Is Nothing Then imageCacher = New ImageCacher
				Dim image As Image = imageCacher.getImage(gc)
				If image Is Nothing Then
					If gc IsNot Nothing Then
						image = gc.createCompatibleImage(iconWidth, iconHeight, Transparency.BITMASK)
					Else
						image = New java.awt.image.BufferedImage(iconWidth, iconHeight, java.awt.image.BufferedImage.TYPE_INT_ARGB)
					End If
					Dim imageG As Graphics = image.graphics
					paintMe(c,imageG)
					imageG.Dispose()
					imageCacher.cacheImage(image, gc)
				End If
				g.drawImage(image, x, y+shift, Nothing)
			End Sub

			Private Sub paintMe(ByVal c As Component, ByVal g As Graphics)

					Dim right As Integer = fileIcon16Size.width - 1
					Dim bottom As Integer = fileIcon16Size.height - 1

					' Draw fill
					g.color = MetalLookAndFeel.windowBackground
					g.fillRect(4, 2, 9, 12)

					' Draw frame
					g.color = MetalLookAndFeel.primaryControlInfo
					g.drawLine(2, 0, 2, bottom) ' left
					g.drawLine(2, 0, right - 4, 0) ' top
					g.drawLine(2, bottom, right - 1, bottom) ' bottom
					g.drawLine(right - 1, 6, right - 1, bottom) ' right
					g.drawLine(right - 6, 2, right - 2, 6) ' slant 1
					g.drawLine(right - 5, 1, right - 4, 1) ' part of slant 2
					g.drawLine(right - 3, 2, right - 3, 3) ' part of slant 2
					g.drawLine(right - 2, 4, right - 2, 5) ' part of slant 2

					' Draw highlight
					g.color = MetalLookAndFeel.primaryControl
					g.drawLine(3, 1, 3, bottom - 1) ' left
					g.drawLine(3, 1, right - 6, 1) ' top
					g.drawLine(right - 2, 7, right - 2, bottom - 1) ' right
					g.drawLine(right - 5, 2, right - 3, 4) ' slant
					g.drawLine(3, bottom - 1, right - 2, bottom - 1) ' bottom

			End Sub

			Public Overridable Property shift As Integer
				Get
					Return 0
				End Get
			End Property
			Public Overridable Property additionalHeight As Integer
				Get
					Return 0
				End Get
			End Property

			Public Overridable Property iconWidth As Integer Implements Icon.getIconWidth
				Get
					Return fileIcon16Size.width
				End Get
			End Property
			Public Overridable Property iconHeight As Integer Implements Icon.getIconHeight
				Get
					Return fileIcon16Size.height + additionalHeight
				End Get
			End Property
		End Class


		Public Class TreeLeafIcon
			Inherits FileIcon16

			Public Property Overrides shift As Integer
				Get
					Return 2
				End Get
			End Property
			Public Property Overrides additionalHeight As Integer
				Get
					Return 4
				End Get
			End Property
		End Class


		Private Shared ReadOnly treeControlSize As New Dimension(18, 18)

		''' <summary>
		''' <p>
		''' <strong>Warning:</strong>
		''' Serialized objects of this class will not be compatible with
		''' future Swing releases. The current serialization support is
		''' appropriate for short term storage or RMI between applications running
		''' the same version of Swing.  As of 1.4, support for long term storage
		''' of all JavaBeans&trade;
		''' has been added to the <code>java.beans</code> package.
		''' Please see <seealso cref="java.beans.XMLEncoder"/>.
		''' </summary>
		<Serializable> _
		Public Class TreeControlIcon
			Implements Icon

			' This data member should not have been exposed.  It's called
			' isLight, but now it really means isCollapsed.  Since we can't change
			' any APIs... that's life.
			Protected Friend isLight As Boolean


			Public Sub New(ByVal isCollapsed As Boolean)
				isLight = isCollapsed
			End Sub

			Friend imageCacher As ImageCacher

			<NonSerialized> _
			Friend cachedOrientation As Boolean = True

			Public Overridable Sub paintIcon(ByVal c As Component, ByVal g As Graphics, ByVal x As Integer, ByVal y As Integer)

				Dim gc As GraphicsConfiguration = c.graphicsConfiguration

				If imageCacher Is Nothing Then imageCacher = New ImageCacher
				Dim image As Image = imageCacher.getImage(gc)

				If image Is Nothing OrElse cachedOrientation <> MetalUtils.isLeftToRight(c) Then
					cachedOrientation = MetalUtils.isLeftToRight(c)
					If gc IsNot Nothing Then
						image = gc.createCompatibleImage(iconWidth, iconHeight, Transparency.BITMASK)
					Else
						image = New java.awt.image.BufferedImage(iconWidth, iconHeight, java.awt.image.BufferedImage.TYPE_INT_ARGB)
					End If
					Dim imageG As Graphics = image.graphics
					paintMe(c,imageG,x,y)
					imageG.Dispose()
					imageCacher.cacheImage(image, gc)

				End If

				If MetalUtils.isLeftToRight(c) Then
					If isLight Then ' isCollapsed
						g.drawImage(image, x+5, y+3, x+18, y+13, 4,3, 17, 13, Nothing)
					Else
						g.drawImage(image, x+5, y+3, x+18, y+17, 4,3, 17, 17, Nothing)
					End If
				Else
					If isLight Then ' isCollapsed
						g.drawImage(image, x+3, y+3, x+16, y+13, 4, 3, 17, 13, Nothing)
					Else
						g.drawImage(image, x+3, y+3, x+16, y+17, 4, 3, 17, 17, Nothing)
					End If
				End If
			End Sub

			Public Overridable Sub paintMe(ByVal c As Component, ByVal g As Graphics, ByVal x As Integer, ByVal y As Integer)

				g.color = MetalLookAndFeel.primaryControlInfo

				Dim xoff As Integer = If(MetalUtils.isLeftToRight(c), 0, 4)

				' Draw circle
				g.drawLine(xoff + 4, 6, xoff + 4, 9) ' left
				g.drawLine(xoff + 5, 5, xoff + 5, 5) ' top left dot
				g.drawLine(xoff + 6, 4, xoff + 9, 4) ' top
				g.drawLine(xoff + 10, 5, xoff + 10, 5) ' top right dot
				g.drawLine(xoff + 11, 6, xoff + 11, 9) ' right
				g.drawLine(xoff + 10, 10, xoff + 10, 10) ' botom right dot
				g.drawLine(xoff + 6, 11, xoff + 9, 11) ' bottom
				g.drawLine(xoff + 5, 10, xoff + 5, 10) ' bottom left dot

				' Draw Center Dot
				g.drawLine(xoff + 7, 7, xoff + 8, 7)
				g.drawLine(xoff + 7, 8, xoff + 8, 8)

				' Draw Handle
				If isLight Then ' isCollapsed
					If MetalUtils.isLeftToRight(c) Then
						g.drawLine(12, 7, 15, 7)
						g.drawLine(12, 8, 15, 8)
						'  g.setColor( c.getBackground() );
						'  g.drawLine( 16, 7, 16, 8 );
					Else
						g.drawLine(4, 7, 7, 7)
						g.drawLine(4, 8, 7, 8)
					End If
				Else
					g.drawLine(xoff + 7, 12, xoff + 7, 15)
					g.drawLine(xoff + 8, 12, xoff + 8, 15)
					'      g.setColor( c.getBackground() );
					'      g.drawLine( xoff + 7, 16, xoff + 8, 16 );
				End If

				' Draw Fill
				g.color = MetalLookAndFeel.primaryControlDarkShadow
				g.drawLine(xoff + 5, 6, xoff + 5, 9) ' left shadow
				g.drawLine(xoff + 6, 5, xoff + 9, 5) ' top shadow

				g.color = MetalLookAndFeel.primaryControlShadow
				g.drawLine(xoff + 6, 6, xoff + 6, 6) ' top left fill
				g.drawLine(xoff + 9, 6, xoff + 9, 6) ' top right fill
				g.drawLine(xoff + 6, 9, xoff + 6, 9) ' bottom left fill
				g.drawLine(xoff + 10, 6, xoff + 10, 9) ' right fill
				g.drawLine(xoff + 6, 10, xoff + 9, 10) ' bottom fill

				g.color = MetalLookAndFeel.primaryControl
				g.drawLine(xoff + 6, 7, xoff + 6, 8) ' left highlight
				g.drawLine(xoff + 7, 6, xoff + 8, 6) ' top highlight
				g.drawLine(xoff + 9, 7, xoff + 9, 7) ' right highlight
				g.drawLine(xoff + 7, 9, xoff + 7, 9) ' bottom highlight

				g.color = MetalLookAndFeel.primaryControlHighlight
				g.drawLine(xoff + 8, 9, xoff + 9, 9)
				g.drawLine(xoff + 9, 8, xoff + 9, 8)
			End Sub

			Public Overridable Property iconWidth As Integer Implements Icon.getIconWidth
				Get
					Return treeControlSize.width
				End Get
			End Property
			Public Overridable Property iconHeight As Integer Implements Icon.getIconHeight
				Get
					Return treeControlSize.height
				End Get
			End Property
		End Class

	  '
	  ' Menu Icons
	  '

		Private Shared ReadOnly menuArrowIconSize As New Dimension(4, 8)
		Private Shared ReadOnly menuCheckIconSize As New Dimension(10, 10)
		Private Const xOff As Integer = 4

		<Serializable> _
		Private Class MenuArrowIcon
			Implements Icon, javax.swing.plaf.UIResource

			Public Overridable Sub paintIcon(ByVal c As Component, ByVal g As Graphics, ByVal x As Integer, ByVal y As Integer)
				Dim b As JMenuItem = CType(c, JMenuItem)
				Dim model As ButtonModel = b.model

				g.translate(x, y)

				If Not model.enabled Then
					g.color = MetalLookAndFeel.menuDisabledForeground
				Else
					If model.armed OrElse (TypeOf c Is JMenu AndAlso model.selected) Then
						g.color = MetalLookAndFeel.menuSelectedForeground
					Else
						g.color = b.foreground
					End If
				End If
				If MetalUtils.isLeftToRight(b) Then
					g.drawLine(0, 0, 0, 7)
					g.drawLine(1, 1, 1, 6)
					g.drawLine(2, 2, 2, 5)
					g.drawLine(3, 3, 3, 4)
				Else
					g.drawLine(4, 0, 4, 7)
					g.drawLine(3, 1, 3, 6)
					g.drawLine(2, 2, 2, 5)
					g.drawLine(1, 3, 1, 4)
				End If

				g.translate(-x, -y)
			End Sub

			Public Overridable Property iconWidth As Integer Implements Icon.getIconWidth
				Get
					Return menuArrowIconSize.width
				End Get
			End Property

			Public Overridable Property iconHeight As Integer Implements Icon.getIconHeight
				Get
					Return menuArrowIconSize.height
				End Get
			End Property

		End Class ' End class MenuArrowIcon

		<Serializable> _
		Private Class MenuItemArrowIcon
			Implements Icon, javax.swing.plaf.UIResource

			Public Overridable Sub paintIcon(ByVal c As Component, ByVal g As Graphics, ByVal x As Integer, ByVal y As Integer)
			End Sub

			Public Overridable Property iconWidth As Integer Implements Icon.getIconWidth
				Get
					Return menuArrowIconSize.width
				End Get
			End Property

			Public Overridable Property iconHeight As Integer Implements Icon.getIconHeight
				Get
					Return menuArrowIconSize.height
				End Get
			End Property

		End Class ' End class MenuItemArrowIcon

		<Serializable> _
		Private Class CheckBoxMenuItemIcon
			Implements Icon, javax.swing.plaf.UIResource

			Public Overridable Sub paintOceanIcon(ByVal c As Component, ByVal g As Graphics, ByVal x As Integer, ByVal y As Integer)
				Dim model As ButtonModel = CType(c, JMenuItem).model
				Dim isSelected As Boolean = model.selected
				Dim isEnabled As Boolean = model.enabled
				Dim isPressed As Boolean = model.pressed
				Dim isArmed As Boolean = model.armed

				g.translate(x, y)
				If isEnabled Then
					MetalUtils.drawGradient(c, g, "CheckBoxMenuItem.gradient", 1, 1, 7, 7, True)
					If isPressed OrElse isArmed Then
						g.color = MetalLookAndFeel.controlInfo
						g.drawLine(0, 0, 8, 0)
						g.drawLine(0, 0, 0, 8)
						g.drawLine(8, 2, 8, 8)
						g.drawLine(2, 8, 8, 8)

						g.color = MetalLookAndFeel.primaryControl
						g.drawLine(9, 1, 9, 9)
						g.drawLine(1, 9, 9, 9)
					Else
						g.color = MetalLookAndFeel.controlDarkShadow
						g.drawLine(0, 0, 8, 0)
						g.drawLine(0, 0, 0, 8)
						g.drawLine(8, 2, 8, 8)
						g.drawLine(2, 8, 8, 8)

						g.color = MetalLookAndFeel.controlHighlight
						g.drawLine(9, 1, 9, 9)
						g.drawLine(1, 9, 9, 9)
					End If
				Else
					g.color = MetalLookAndFeel.menuDisabledForeground
					g.drawRect(0, 0, 8, 8)
				End If
				If isSelected Then
					If isEnabled Then
						If isArmed OrElse (TypeOf c Is JMenu AndAlso isSelected) Then
							g.color = MetalLookAndFeel.menuSelectedForeground
						Else
							 g.color = MetalLookAndFeel.controlInfo
						End If
					Else
						g.color = MetalLookAndFeel.menuDisabledForeground
					End If

					g.drawLine(2, 2, 2, 6)
					g.drawLine(3, 2, 3, 6)
					g.drawLine(4, 4, 8, 0)
					g.drawLine(4, 5, 9, 0)
				End If
				g.translate(-x, -y)
			End Sub

			Public Overridable Sub paintIcon(ByVal c As Component, ByVal g As Graphics, ByVal x As Integer, ByVal y As Integer)
				If MetalLookAndFeel.usingOcean() Then
					paintOceanIcon(c, g, x, y)
					Return
				End If
				Dim b As JMenuItem = CType(c, JMenuItem)
				Dim model As ButtonModel = b.model

				Dim isSelected As Boolean = model.selected
				Dim isEnabled As Boolean = model.enabled
				Dim isPressed As Boolean = model.pressed
				Dim isArmed As Boolean = model.armed

				g.translate(x, y)

				If isEnabled Then
					If isPressed OrElse isArmed Then
						g.color = MetalLookAndFeel.controlInfo
						g.drawLine(0, 0, 8, 0)
						g.drawLine(0, 0, 0, 8)
						g.drawLine(8, 2, 8, 8)
						g.drawLine(2, 8, 8, 8)

						g.color = MetalLookAndFeel.primaryControl
						g.drawLine(1, 1, 7, 1)
						g.drawLine(1, 1, 1, 7)
						g.drawLine(9, 1, 9, 9)
						g.drawLine(1, 9, 9, 9)
					Else
						g.color = MetalLookAndFeel.controlDarkShadow
						g.drawLine(0, 0, 8, 0)
						g.drawLine(0, 0, 0, 8)
						g.drawLine(8, 2, 8, 8)
						g.drawLine(2, 8, 8, 8)

						g.color = MetalLookAndFeel.controlHighlight
						g.drawLine(1, 1, 7, 1)
						g.drawLine(1, 1, 1, 7)
						g.drawLine(9, 1, 9, 9)
						g.drawLine(1, 9, 9, 9)
					End If
				Else
					g.color = MetalLookAndFeel.menuDisabledForeground
					g.drawRect(0, 0, 8, 8)
				End If

				If isSelected Then
					If isEnabled Then
						If model.armed OrElse (TypeOf c Is JMenu AndAlso model.selected) Then
							g.color = MetalLookAndFeel.menuSelectedForeground
						Else
							g.color = b.foreground
						End If
					Else
						g.color = MetalLookAndFeel.menuDisabledForeground
					End If

					g.drawLine(2, 2, 2, 6)
					g.drawLine(3, 2, 3, 6)
					g.drawLine(4, 4, 8, 0)
					g.drawLine(4, 5, 9, 0)
				End If

				g.translate(-x, -y)
			End Sub

			Public Overridable Property iconWidth As Integer Implements Icon.getIconWidth
				Get
					Return menuCheckIconSize.width
				End Get
			End Property

			Public Overridable Property iconHeight As Integer Implements Icon.getIconHeight
				Get
					Return menuCheckIconSize.height
				End Get
			End Property

		End Class ' End class CheckBoxMenuItemIcon

		<Serializable> _
		Private Class RadioButtonMenuItemIcon
			Implements Icon, javax.swing.plaf.UIResource

			Public Overridable Sub paintOceanIcon(ByVal c As Component, ByVal g As Graphics, ByVal x As Integer, ByVal y As Integer)
				Dim model As ButtonModel = CType(c, JMenuItem).model
				Dim isSelected As Boolean = model.selected
				Dim isEnabled As Boolean = model.enabled
				Dim isPressed As Boolean = model.pressed
				Dim isArmed As Boolean = model.armed

				g.translate(x, y)

				If isEnabled Then
					MetalUtils.drawGradient(c, g, "RadioButtonMenuItem.gradient", 1, 1, 7, 7, True)
					If isPressed OrElse isArmed Then
						g.color = MetalLookAndFeel.primaryControl
					Else
						g.color = MetalLookAndFeel.controlHighlight
					End If
					g.drawLine(2, 9, 7, 9)
					g.drawLine(9, 2, 9, 7)
					g.drawLine(8, 8, 8, 8)

					If isPressed OrElse isArmed Then
						g.color = MetalLookAndFeel.controlInfo
					Else
						g.color = MetalLookAndFeel.controlDarkShadow
					End If
				Else
					g.color = MetalLookAndFeel.menuDisabledForeground
				End If
				g.drawLine(2, 0, 6, 0)
				g.drawLine(2, 8, 6, 8)
				g.drawLine(0, 2, 0, 6)
				g.drawLine(8, 2, 8, 6)
				g.drawLine(1, 1, 1, 1)
				g.drawLine(7, 1, 7, 1)
				g.drawLine(1, 7, 1, 7)
				g.drawLine(7, 7, 7, 7)

				If isSelected Then
					If isEnabled Then
						If isArmed OrElse (TypeOf c Is JMenu AndAlso model.selected) Then
							g.color = MetalLookAndFeel.menuSelectedForeground
						Else
							g.color = MetalLookAndFeel.controlInfo
						End If
					Else
						g.color = MetalLookAndFeel.menuDisabledForeground
					End If
					g.drawLine(3, 2, 5, 2)
					g.drawLine(2, 3, 6, 3)
					g.drawLine(2, 4, 6, 4)
					g.drawLine(2, 5, 6, 5)
					g.drawLine(3, 6, 5, 6)
				End If

				g.translate(-x, -y)
			End Sub

			Public Overridable Sub paintIcon(ByVal c As Component, ByVal g As Graphics, ByVal x As Integer, ByVal y As Integer)
				If MetalLookAndFeel.usingOcean() Then
					paintOceanIcon(c, g, x, y)
					Return
				End If
				Dim b As JMenuItem = CType(c, JMenuItem)
				Dim model As ButtonModel = b.model

				Dim isSelected As Boolean = model.selected
				Dim isEnabled As Boolean = model.enabled
				Dim isPressed As Boolean = model.pressed
				Dim isArmed As Boolean = model.armed

				g.translate(x, y)

				If isEnabled Then
					If isPressed OrElse isArmed Then
						g.color = MetalLookAndFeel.primaryControl
						g.drawLine(3, 1, 8, 1)
						g.drawLine(2, 9, 7, 9)
						g.drawLine(1, 3, 1, 8)
						g.drawLine(9, 2, 9, 7)
						g.drawLine(2, 2, 2, 2)
						g.drawLine(8, 8, 8, 8)

						g.color = MetalLookAndFeel.controlInfo
						g.drawLine(2, 0, 6, 0)
						g.drawLine(2, 8, 6, 8)
						g.drawLine(0, 2, 0, 6)
						g.drawLine(8, 2, 8, 6)
						g.drawLine(1, 1, 1, 1)
						g.drawLine(7, 1, 7, 1)
						g.drawLine(1, 7, 1, 7)
						g.drawLine(7, 7, 7, 7)
					Else
						g.color = MetalLookAndFeel.controlHighlight
						g.drawLine(3, 1, 8, 1)
						g.drawLine(2, 9, 7, 9)
						g.drawLine(1, 3, 1, 8)
						g.drawLine(9, 2, 9, 7)
						g.drawLine(2, 2, 2, 2)
						g.drawLine(8, 8, 8, 8)

						g.color = MetalLookAndFeel.controlDarkShadow
						g.drawLine(2, 0, 6, 0)
						g.drawLine(2, 8, 6, 8)
						g.drawLine(0, 2, 0, 6)
						g.drawLine(8, 2, 8, 6)
						g.drawLine(1, 1, 1, 1)
						g.drawLine(7, 1, 7, 1)
						g.drawLine(1, 7, 1, 7)
						g.drawLine(7, 7, 7, 7)
					End If
				Else
					g.color = MetalLookAndFeel.menuDisabledForeground
					g.drawLine(2, 0, 6, 0)
					g.drawLine(2, 8, 6, 8)
					g.drawLine(0, 2, 0, 6)
					g.drawLine(8, 2, 8, 6)
					g.drawLine(1, 1, 1, 1)
					g.drawLine(7, 1, 7, 1)
					g.drawLine(1, 7, 1, 7)
					g.drawLine(7, 7, 7, 7)
				End If

				If isSelected Then
					If isEnabled Then
						If model.armed OrElse (TypeOf c Is JMenu AndAlso model.selected) Then
							g.color = MetalLookAndFeel.menuSelectedForeground
						Else
							g.color = b.foreground
						End If
					Else
						g.color = MetalLookAndFeel.menuDisabledForeground
					End If

					g.drawLine(3, 2, 5, 2)
					g.drawLine(2, 3, 6, 3)
					g.drawLine(2, 4, 6, 4)
					g.drawLine(2, 5, 6, 5)
					g.drawLine(3, 6, 5, 6)
				End If

				g.translate(-x, -y)
			End Sub

			Public Overridable Property iconWidth As Integer Implements Icon.getIconWidth
				Get
					Return menuCheckIconSize.width
				End Get
			End Property

			Public Overridable Property iconHeight As Integer Implements Icon.getIconHeight
				Get
					Return menuCheckIconSize.height
				End Get
			End Property

		End Class ' End class RadioButtonMenuItemIcon

	<Serializable> _
	Private Class VerticalSliderThumbIcon
		Implements Icon, javax.swing.plaf.UIResource

		Protected Friend Shared controlBumps As MetalBumps
		Protected Friend Shared primaryBumps As MetalBumps

		Public Sub New()
			controlBumps = New MetalBumps(6, 10, MetalLookAndFeel.controlHighlight, MetalLookAndFeel.controlInfo, MetalLookAndFeel.control)
			primaryBumps = New MetalBumps(6, 10, MetalLookAndFeel.primaryControl, MetalLookAndFeel.primaryControlDarkShadow, MetalLookAndFeel.primaryControlShadow)
		End Sub

		Public Overridable Sub paintIcon(ByVal c As Component, ByVal g As Graphics, ByVal x As Integer, ByVal y As Integer)
			Dim leftToRight As Boolean = MetalUtils.isLeftToRight(c)

			g.translate(x, y)

			' Draw the frame
			If c.hasFocus() Then
				g.color = MetalLookAndFeel.primaryControlInfo
			Else
				g.color = If(c.enabled, MetalLookAndFeel.primaryControlInfo, MetalLookAndFeel.controlDarkShadow)
			End If

			If leftToRight Then
				g.drawLine(1,0, 8,0) ' top
				g.drawLine(0,1, 0,13) ' left
				g.drawLine(1,14, 8,14) ' bottom
				g.drawLine(9,1, 15,7) ' top slant
				g.drawLine(9,13, 15,7) ' bottom slant
			Else
				g.drawLine(7,0, 14,0) ' top
				g.drawLine(15,1, 15,13) ' right
				g.drawLine(7,14, 14,14) ' bottom
				g.drawLine(0,7, 6,1) ' top slant
				g.drawLine(0,7, 6,13) ' bottom slant
			End If

			' Fill in the background
			If c.hasFocus() Then
				g.color = c.foreground
			Else
				g.color = MetalLookAndFeel.control
			End If

			If leftToRight Then
				g.fillRect(1,1, 8,13)

				g.drawLine(9,2, 9,12)
				g.drawLine(10,3, 10,11)
				g.drawLine(11,4, 11,10)
				g.drawLine(12,5, 12,9)
				g.drawLine(13,6, 13,8)
				g.drawLine(14,7, 14,7)
			Else
				g.fillRect(7,1, 8,13)

				g.drawLine(6,3, 6,12)
				g.drawLine(5,4, 5,11)
				g.drawLine(4,5, 4,10)
				g.drawLine(3,6, 3,9)
				g.drawLine(2,7, 2,8)
			End If

			' Draw the bumps
			Dim offset As Integer = If(leftToRight, 2, 8)
			If c.enabled Then
				If c.hasFocus() Then
					primaryBumps.paintIcon(c, g, offset, 2)
				Else
					controlBumps.paintIcon(c, g, offset, 2)
				End If
			End If

			' Draw the highlight
			If c.enabled Then
				g.color = If(c.hasFocus(), MetalLookAndFeel.primaryControl, MetalLookAndFeel.controlHighlight)
				If leftToRight Then
					g.drawLine(1, 1, 8, 1)
					g.drawLine(1, 1, 1, 13)
				Else
					g.drawLine(8,1, 14,1) ' top
					g.drawLine(1,7, 7,1) ' top slant
				End If
			End If

			g.translate(-x, -y)
		End Sub

		Public Overridable Property iconWidth As Integer Implements Icon.getIconWidth
			Get
				Return 16
			End Get
		End Property

		Public Overridable Property iconHeight As Integer Implements Icon.getIconHeight
			Get
				Return 15
			End Get
		End Property
	End Class

	<Serializable> _
	Private Class HorizontalSliderThumbIcon
		Implements Icon, javax.swing.plaf.UIResource

		Protected Friend Shared controlBumps As MetalBumps
		Protected Friend Shared primaryBumps As MetalBumps

		Public Sub New()
			controlBumps = New MetalBumps(10, 6, MetalLookAndFeel.controlHighlight, MetalLookAndFeel.controlInfo, MetalLookAndFeel.control)
			primaryBumps = New MetalBumps(10, 6, MetalLookAndFeel.primaryControl, MetalLookAndFeel.primaryControlDarkShadow, MetalLookAndFeel.primaryControlShadow)
		End Sub

		Public Overridable Sub paintIcon(ByVal c As Component, ByVal g As Graphics, ByVal x As Integer, ByVal y As Integer)
			g.translate(x, y)

			' Draw the frame
			If c.hasFocus() Then
				g.color = MetalLookAndFeel.primaryControlInfo
			Else
				g.color = If(c.enabled, MetalLookAndFeel.primaryControlInfo, MetalLookAndFeel.controlDarkShadow)
			End If

			g.drawLine(1,0, 13,0) ' top
			g.drawLine(0,1, 0,8) ' left
			g.drawLine(14,1, 14,8) ' right
			g.drawLine(1,9, 7,15) ' left slant
			g.drawLine(7,15, 14,8) ' right slant

			' Fill in the background
			If c.hasFocus() Then
				g.color = c.foreground
			Else
				g.color = MetalLookAndFeel.control
			End If
			g.fillRect(1,1, 13, 8)

			g.drawLine(2,9, 12,9)
			g.drawLine(3,10, 11,10)
			g.drawLine(4,11, 10,11)
			g.drawLine(5,12, 9,12)
			g.drawLine(6,13, 8,13)
			g.drawLine(7,14, 7,14)

			' Draw the bumps
			If c.enabled Then
				If c.hasFocus() Then
					primaryBumps.paintIcon(c, g, 2, 2)
				Else
					controlBumps.paintIcon(c, g, 2, 2)
				End If
			End If

			' Draw the highlight
			If c.enabled Then
				g.color = If(c.hasFocus(), MetalLookAndFeel.primaryControl, MetalLookAndFeel.controlHighlight)
				g.drawLine(1, 1, 13, 1)
				g.drawLine(1, 1, 1, 8)
			End If

			g.translate(-x, -y)
		End Sub

		Public Overridable Property iconWidth As Integer Implements Icon.getIconWidth
			Get
				Return 15
			End Get
		End Property

		Public Overridable Property iconHeight As Integer Implements Icon.getIconHeight
			Get
				Return 16
			End Get
		End Property
	End Class

		<Serializable> _
		Private Class OceanVerticalSliderThumbIcon
			Inherits sun.swing.CachedPainter
			Implements Icon, javax.swing.plaf.UIResource

			' Used for clipping when the orientation is left to right
			Private Shared LTR_THUMB_SHAPE As Polygon
			' Used for clipping when the orientation is right to left
			Private Shared RTL_THUMB_SHAPE As Polygon

			Shared Sub New()
				LTR_THUMB_SHAPE = New Polygon(New Integer() { 0, 8, 15, 8, 0}, New Integer() { 0, 0, 7, 14, 14 }, 5)
				RTL_THUMB_SHAPE = New Polygon(New Integer() { 15, 15, 7, 0, 7}, New Integer() { 0, 14, 14, 7, 0}, 5)
			End Sub

			Friend Sub New()
				MyBase.New(3)
			End Sub

			Public Overridable Sub paintIcon(ByVal c As Component, ByVal g As Graphics, ByVal x As Integer, ByVal y As Integer)
				If Not(TypeOf g Is Graphics2D) Then Return
				paint(c, g, x, y, iconWidth, iconHeight, MetalUtils.isLeftToRight(c), c.hasFocus(), c.enabled, MetalLookAndFeel.currentTheme)
			End Sub

			Protected Friend Overridable Sub paintToImage(ByVal c As Component, ByVal image As Image, ByVal g2 As Graphics, ByVal w As Integer, ByVal h As Integer, ByVal args As Object())
				Dim g As Graphics2D = CType(g2, Graphics2D)
				Dim leftToRight As Boolean = CBool(args(0))
				Dim hasFocus As Boolean = CBool(args(1))
				Dim enabled As Boolean = CBool(args(2))

				Dim clip As Rectangle = g.clipBounds
				If leftToRight Then
					g.clip(LTR_THUMB_SHAPE)
				Else
					g.clip(RTL_THUMB_SHAPE)
				End If
				If Not enabled Then
					g.color = MetalLookAndFeel.control
					g.fillRect(1, 1, 14, 14)
				ElseIf hasFocus Then
					MetalUtils.drawGradient(c, g, "Slider.focusGradient", 1, 1, 14, 14, False)
				Else
					MetalUtils.drawGradient(c, g, "Slider.gradient", 1, 1, 14, 14, False)
				End If
				g.clip = clip

				' Draw the frame
				If hasFocus Then
					g.color = MetalLookAndFeel.primaryControlDarkShadow
				Else
					g.color = If(enabled, MetalLookAndFeel.primaryControlInfo, MetalLookAndFeel.controlDarkShadow)
				End If

				If leftToRight Then
					g.drawLine(1,0, 8,0) ' top
					g.drawLine(0,1, 0,13) ' left
					g.drawLine(1,14, 8,14) ' bottom
					g.drawLine(9,1, 15,7) ' top slant
					g.drawLine(9,13, 15,7) ' bottom slant
				Else
					g.drawLine(7,0, 14,0) ' top
					g.drawLine(15,1, 15,13) ' right
					g.drawLine(7,14, 14,14) ' bottom
					g.drawLine(0,7, 6,1) ' top slant
					g.drawLine(0,7, 6,13) ' bottom slant
				End If

				If hasFocus AndAlso enabled Then
					' Inner line.
					g.color = MetalLookAndFeel.primaryControl
					If leftToRight Then
						g.drawLine(1,1, 8,1) ' top
						g.drawLine(1,1, 1,13) ' left
						g.drawLine(1,13, 8,13) ' bottom
						g.drawLine(9,2, 14,7) ' top slant
						g.drawLine(9,12, 14,7) ' bottom slant
					Else
						g.drawLine(7,1, 14,1) ' top
						g.drawLine(14,1, 14,13) ' right
						g.drawLine(7,13, 14,13) ' bottom
						g.drawLine(1,7, 7,1) ' top slant
						g.drawLine(1,7, 7,13) ' bottom slant
					End If
				End If
			End Sub

			Public Overridable Property iconWidth As Integer Implements Icon.getIconWidth
				Get
					Return 16
				End Get
			End Property

			Public Overridable Property iconHeight As Integer Implements Icon.getIconHeight
				Get
					Return 15
				End Get
			End Property

			Protected Friend Overridable Function createImage(ByVal c As Component, ByVal w As Integer, ByVal h As Integer, ByVal config As GraphicsConfiguration, ByVal args As Object()) As Image
				If config Is Nothing Then Return New java.awt.image.BufferedImage(w, h,java.awt.image.BufferedImage.TYPE_INT_ARGB)
				Return config.createCompatibleImage(w, h, Transparency.BITMASK)
			End Function
		End Class


		<Serializable> _
		Private Class OceanHorizontalSliderThumbIcon
			Inherits sun.swing.CachedPainter
			Implements Icon, javax.swing.plaf.UIResource

			' Used for clipping
			Private Shared THUMB_SHAPE As Polygon

			Shared Sub New()
				THUMB_SHAPE = New Polygon(New Integer() { 0, 14, 14, 7, 0 }, New Integer() { 0, 0, 8, 15, 8 }, 5)
			End Sub

			Friend Sub New()
				MyBase.New(3)
			End Sub

			Public Overridable Sub paintIcon(ByVal c As Component, ByVal g As Graphics, ByVal x As Integer, ByVal y As Integer)
				If Not(TypeOf g Is Graphics2D) Then Return
				paint(c, g, x, y, iconWidth, iconHeight, c.hasFocus(), c.enabled, MetalLookAndFeel.currentTheme)
			End Sub


			Protected Friend Overridable Function createImage(ByVal c As Component, ByVal w As Integer, ByVal h As Integer, ByVal config As GraphicsConfiguration, ByVal args As Object()) As Image
				If config Is Nothing Then Return New java.awt.image.BufferedImage(w, h,java.awt.image.BufferedImage.TYPE_INT_ARGB)
				Return config.createCompatibleImage(w, h, Transparency.BITMASK)
			End Function

			Protected Friend Overridable Sub paintToImage(ByVal c As Component, ByVal image As Image, ByVal g2 As Graphics, ByVal w As Integer, ByVal h As Integer, ByVal args As Object())
				Dim g As Graphics2D = CType(g2, Graphics2D)
				Dim hasFocus As Boolean = CBool(args(0))
				Dim enabled As Boolean = CBool(args(1))

				' Fill in the background
				Dim clip As Rectangle = g.clipBounds
				g.clip(THUMB_SHAPE)
				If Not enabled Then
					g.color = MetalLookAndFeel.control
					g.fillRect(1, 1, 13, 14)
				ElseIf hasFocus Then
					MetalUtils.drawGradient(c, g, "Slider.focusGradient", 1, 1, 13, 14, True)
				Else
					MetalUtils.drawGradient(c, g, "Slider.gradient", 1, 1, 13, 14, True)
				End If
				g.clip = clip

				' Draw the frame
				If hasFocus Then
					g.color = MetalLookAndFeel.primaryControlDarkShadow
				Else
					g.color = If(enabled, MetalLookAndFeel.primaryControlInfo, MetalLookAndFeel.controlDarkShadow)
				End If

				g.drawLine(1,0, 13,0) ' top
				g.drawLine(0,1, 0,8) ' left
				g.drawLine(14,1, 14,8) ' right
				g.drawLine(1,9, 7,15) ' left slant
				g.drawLine(7,15, 14,8) ' right slant

				If hasFocus AndAlso enabled Then
					' Inner line.
					g.color = MetalLookAndFeel.primaryControl
					g.fillRect(1, 1, 13, 1)
					g.fillRect(1, 2, 1, 7)
					g.fillRect(13, 2, 1, 7)
					g.drawLine(2, 9, 7, 14)
					g.drawLine(8, 13, 12, 9)
				End If
			End Sub

			Public Overridable Property iconWidth As Integer Implements Icon.getIconWidth
				Get
					Return 15
				End Get
			End Property

			Public Overridable Property iconHeight As Integer Implements Icon.getIconHeight
				Get
					Return 16
				End Get
			End Property
		End Class
	End Class

End Namespace