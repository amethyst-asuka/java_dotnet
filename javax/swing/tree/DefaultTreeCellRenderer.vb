Imports System

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

Namespace javax.swing.tree


	''' <summary>
	''' Displays an entry in a tree.
	''' <code>DefaultTreeCellRenderer</code> is not opaque and
	''' unless you subclass paint you should not change this.
	''' See <a
	''' href="https://docs.oracle.com/javase/tutorial/uiswing/components/tree.html">How to Use Trees</a>
	''' in <em>The Java Tutorial</em>
	''' for examples of customizing node display using this class.
	''' <p>
	''' The set of icons and colors used by {@code DefaultTreeCellRenderer}
	''' can be configured using the various setter methods. The value for
	''' each property is initialized from the defaults table. When the
	''' look and feel changes ({@code updateUI} is invoked), any properties
	''' that have a value of type {@code UIResource} are refreshed from the
	''' defaults table. The following table lists the mapping between
	''' {@code DefaultTreeCellRenderer} property and defaults table key:
	''' <table border="1" cellpadding="1" cellspacing="0" summary="">
	'''   <tr valign="top"  align="left">
	'''     <th style="background-color:#CCCCFF" align="left">Property:
	'''     <th style="background-color:#CCCCFF" align="left">Key:
	'''   <tr><td>"leafIcon"<td>"Tree.leafIcon"
	'''   <tr><td>"closedIcon"<td>"Tree.closedIcon"
	'''   <tr><td>"openIcon"<td>"Tree.openIcon"
	'''   <tr><td>"textSelectionColor"<td>"Tree.selectionForeground"
	'''   <tr><td>"textNonSelectionColor"<td>"Tree.textForeground"
	'''   <tr><td>"backgroundSelectionColor"<td>"Tree.selectionBackground"
	'''   <tr><td>"backgroundNonSelectionColor"<td>"Tree.textBackground"
	'''   <tr><td>"borderSelectionColor"<td>"Tree.selectionBorderColor"
	''' </table>
	''' <p>
	''' <strong><a name="override">Implementation Note:</a></strong>
	''' This class overrides
	''' <code>invalidate</code>,
	''' <code>validate</code>,
	''' <code>revalidate</code>,
	''' <code>repaint</code>,
	''' and
	''' <code>firePropertyChange</code>
	''' solely to improve performance.
	''' If not overridden, these frequently called methods would execute code paths
	''' that are unnecessary for the default tree cell renderer.
	''' If you write your own renderer,
	''' take care to weigh the benefits and
	''' drawbacks of overriding these methods.
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
	''' @author Rob Davis
	''' @author Ray Ryan
	''' @author Scott Violet
	''' </summary>
	Public Class DefaultTreeCellRenderer
		Inherits javax.swing.JLabel
		Implements TreeCellRenderer

		''' <summary>
		''' Last tree the renderer was painted in. </summary>
		Private tree As javax.swing.JTree

		''' <summary>
		''' Is the value currently selected. </summary>
		Protected Friend selected As Boolean
		''' <summary>
		''' True if has focus. </summary>
		Protected Friend hasFocus As Boolean
		''' <summary>
		''' True if draws focus border around icon as well. </summary>
		Private drawsFocusBorderAroundIcon As Boolean
		''' <summary>
		''' If true, a dashed line is drawn as the focus indicator. </summary>
		Private drawDashedFocusIndicator As Boolean

		' If drawDashedFocusIndicator is true, the following are used.
		''' <summary>
		''' Background color of the tree.
		''' </summary>
		Private treeBGColor As java.awt.Color
		''' <summary>
		''' Color to draw the focus indicator in, determined from the background.
		''' color.
		''' </summary>
		Private focusBGColor As java.awt.Color

		' Icons
		''' <summary>
		''' Icon used to show non-leaf nodes that aren't expanded. </summary>
		<NonSerialized> _
		Protected Friend closedIcon As javax.swing.Icon

		''' <summary>
		''' Icon used to show leaf nodes. </summary>
		<NonSerialized> _
		Protected Friend leafIcon As javax.swing.Icon

		''' <summary>
		''' Icon used to show non-leaf nodes that are expanded. </summary>
		<NonSerialized> _
		Protected Friend openIcon As javax.swing.Icon

		' Colors
		''' <summary>
		''' Color to use for the foreground for selected nodes. </summary>
		Protected Friend textSelectionColor As java.awt.Color

		''' <summary>
		''' Color to use for the foreground for non-selected nodes. </summary>
		Protected Friend textNonSelectionColor As java.awt.Color

		''' <summary>
		''' Color to use for the background when a node is selected. </summary>
		Protected Friend backgroundSelectionColor As java.awt.Color

		''' <summary>
		''' Color to use for the background when the node isn't selected. </summary>
		Protected Friend backgroundNonSelectionColor As java.awt.Color

		''' <summary>
		''' Color to use for the focus indicator when the node has focus. </summary>
		Protected Friend borderSelectionColor As java.awt.Color

		Private isDropCell As Boolean
		Private fillBackground As Boolean

		''' <summary>
		''' Set to true after the constructor has run.
		''' </summary>
		Private inited As Boolean

		''' <summary>
		''' Creates a {@code DefaultTreeCellRenderer}. Icons and text color are
		''' determined from the {@code UIManager}.
		''' </summary>
		Public Sub New()
			inited = True
		End Sub

		''' <summary>
		''' {@inheritDoc}
		''' 
		''' @since 1.7
		''' </summary>
		Public Overrides Sub updateUI()
			MyBase.updateUI()
			' To avoid invoking new methods from the constructor, the
			' inited field is first checked. If inited is false, the constructor
			' has not run and there is no point in checking the value. As
			' all look and feels have a non-null value for these properties,
			' a null value means the developer has specifically set it to
			' null. As such, if the value is null, this does not reset the
			' value.
			If (Not inited) OrElse (TypeOf leafIcon Is javax.swing.plaf.UIResource) Then leafIcon = sun.swing.DefaultLookup.getIcon(Me, ui, "Tree.leafIcon")
			If (Not inited) OrElse (TypeOf closedIcon Is javax.swing.plaf.UIResource) Then closedIcon = sun.swing.DefaultLookup.getIcon(Me, ui, "Tree.closedIcon")
			If (Not inited) OrElse (TypeOf openIcon Is javax.swing.UIManager) Then openIcon = sun.swing.DefaultLookup.getIcon(Me, ui, "Tree.openIcon")
			If (Not inited) OrElse (TypeOf textSelectionColor Is javax.swing.plaf.UIResource) Then textSelectionColor = sun.swing.DefaultLookup.getColor(Me, ui, "Tree.selectionForeground")
			If (Not inited) OrElse (TypeOf textNonSelectionColor Is javax.swing.plaf.UIResource) Then textNonSelectionColor = sun.swing.DefaultLookup.getColor(Me, ui, "Tree.textForeground")
			If (Not inited) OrElse (TypeOf backgroundSelectionColor Is javax.swing.plaf.UIResource) Then backgroundSelectionColor = sun.swing.DefaultLookup.getColor(Me, ui, "Tree.selectionBackground")
			If (Not inited) OrElse (TypeOf backgroundNonSelectionColor Is javax.swing.plaf.UIResource) Then backgroundNonSelectionColor = sun.swing.DefaultLookup.getColor(Me, ui, "Tree.textBackground")
			If (Not inited) OrElse (TypeOf borderSelectionColor Is javax.swing.plaf.UIResource) Then borderSelectionColor = sun.swing.DefaultLookup.getColor(Me, ui, "Tree.selectionBorderColor")
			drawsFocusBorderAroundIcon = sun.swing.DefaultLookup.getBoolean(Me, ui, "Tree.drawsFocusBorderAroundIcon", False)
			drawDashedFocusIndicator = sun.swing.DefaultLookup.getBoolean(Me, ui, "Tree.drawDashedFocusIndicator", False)

			fillBackground = sun.swing.DefaultLookup.getBoolean(Me, ui, "Tree.rendererFillBackground", True)
			Dim margins As java.awt.Insets = sun.swing.DefaultLookup.getInsets(Me, ui, "Tree.rendererMargins")
			If margins IsNot Nothing Then border = New javax.swing.border.EmptyBorder(margins.top, margins.left, margins.bottom, margins.right)

			name = "Tree.cellRenderer"
		End Sub


		''' <summary>
		''' Returns the default icon, for the current laf, that is used to
		''' represent non-leaf nodes that are expanded.
		''' </summary>
		Public Overridable Property defaultOpenIcon As javax.swing.Icon
			Get
				Return sun.swing.DefaultLookup.getIcon(Me, ui, "Tree.openIcon")
			End Get
		End Property

		''' <summary>
		''' Returns the default icon, for the current laf, that is used to
		''' represent non-leaf nodes that are not expanded.
		''' </summary>
		Public Overridable Property defaultClosedIcon As javax.swing.Icon
			Get
				Return sun.swing.DefaultLookup.getIcon(Me, ui, "Tree.closedIcon")
			End Get
		End Property

		''' <summary>
		''' Returns the default icon, for the current laf, that is used to
		''' represent leaf nodes.
		''' </summary>
		Public Overridable Property defaultLeafIcon As javax.swing.Icon
			Get
				Return sun.swing.DefaultLookup.getIcon(Me, ui, "Tree.leafIcon")
			End Get
		End Property

		''' <summary>
		''' Sets the icon used to represent non-leaf nodes that are expanded.
		''' </summary>
		Public Overridable Property openIcon As javax.swing.Icon
			Set(ByVal newIcon As javax.swing.Icon)
				openIcon = newIcon
			End Set
			Get
				Return openIcon
			End Get
		End Property


		''' <summary>
		''' Sets the icon used to represent non-leaf nodes that are not expanded.
		''' </summary>
		Public Overridable Property closedIcon As javax.swing.Icon
			Set(ByVal newIcon As javax.swing.Icon)
				closedIcon = newIcon
			End Set
			Get
				Return closedIcon
			End Get
		End Property


		''' <summary>
		''' Sets the icon used to represent leaf nodes.
		''' </summary>
		Public Overridable Property leafIcon As javax.swing.Icon
			Set(ByVal newIcon As javax.swing.Icon)
				leafIcon = newIcon
			End Set
			Get
				Return leafIcon
			End Get
		End Property


		''' <summary>
		''' Sets the color the text is drawn with when the node is selected.
		''' </summary>
		Public Overridable Property textSelectionColor As java.awt.Color
			Set(ByVal newColor As java.awt.Color)
				textSelectionColor = newColor
			End Set
			Get
				Return textSelectionColor
			End Get
		End Property


		''' <summary>
		''' Sets the color the text is drawn with when the node isn't selected.
		''' </summary>
		Public Overridable Property textNonSelectionColor As java.awt.Color
			Set(ByVal newColor As java.awt.Color)
				textNonSelectionColor = newColor
			End Set
			Get
				Return textNonSelectionColor
			End Get
		End Property


		''' <summary>
		''' Sets the color to use for the background if node is selected.
		''' </summary>
		Public Overridable Property backgroundSelectionColor As java.awt.Color
			Set(ByVal newColor As java.awt.Color)
				backgroundSelectionColor = newColor
			End Set
			Get
				Return backgroundSelectionColor
			End Get
		End Property



		''' <summary>
		''' Sets the background color to be used for non selected nodes.
		''' </summary>
		Public Overridable Property backgroundNonSelectionColor As java.awt.Color
			Set(ByVal newColor As java.awt.Color)
				backgroundNonSelectionColor = newColor
			End Set
			Get
				Return backgroundNonSelectionColor
			End Get
		End Property


		''' <summary>
		''' Sets the color to use for the border.
		''' </summary>
		Public Overridable Property borderSelectionColor As java.awt.Color
			Set(ByVal newColor As java.awt.Color)
				borderSelectionColor = newColor
			End Set
			Get
				Return borderSelectionColor
			End Get
		End Property


		''' <summary>
		''' Subclassed to map <code>FontUIResource</code>s to null. If
		''' <code>font</code> is null, or a <code>FontUIResource</code>, this
		''' has the effect of letting the font of the JTree show
		''' through. On the other hand, if <code>font</code> is non-null, and not
		''' a <code>FontUIResource</code>, the font becomes <code>font</code>.
		''' </summary>
		Public Overridable Property font As java.awt.Font
			Set(ByVal font As java.awt.Font)
				If TypeOf font Is javax.swing.plaf.FontUIResource Then font = Nothing
				MyBase.font = font
			End Set
			Get
				Dim ___font As java.awt.Font = MyBase.font
    
				If ___font Is Nothing AndAlso tree IsNot Nothing Then ___font = tree.font
				Return ___font
			End Get
		End Property


		''' <summary>
		''' Subclassed to map <code>ColorUIResource</code>s to null. If
		''' <code>color</code> is null, or a <code>ColorUIResource</code>, this
		''' has the effect of letting the background color of the JTree show
		''' through. On the other hand, if <code>color</code> is non-null, and not
		''' a <code>ColorUIResource</code>, the background becomes
		''' <code>color</code>.
		''' </summary>
		Public Overridable Property background As java.awt.Color
			Set(ByVal color As java.awt.Color)
				If TypeOf color Is javax.swing.plaf.ColorUIResource Then color = Nothing
				MyBase.background = color
			End Set
		End Property

		''' <summary>
		''' Configures the renderer based on the passed in components.
		''' The value is set from messaging the tree with
		''' <code>convertValueToText</code>, which ultimately invokes
		''' <code>toString</code> on <code>value</code>.
		''' The foreground color is set based on the selection and the icon
		''' is set based on the <code>leaf</code> and <code>expanded</code>
		''' parameters.
		''' </summary>
		Public Overridable Function getTreeCellRendererComponent(ByVal tree As javax.swing.JTree, ByVal value As Object, ByVal sel As Boolean, ByVal expanded As Boolean, ByVal leaf As Boolean, ByVal row As Integer, ByVal hasFocus As Boolean) As java.awt.Component Implements TreeCellRenderer.getTreeCellRendererComponent
			Dim stringValue As String = tree.convertValueToText(value, sel, expanded, leaf, row, hasFocus)

			Me.tree = tree
			Me.hasFocus = hasFocus
			text = stringValue

			Dim fg As java.awt.Color = Nothing
			isDropCell = False

			Dim ___dropLocation As javax.swing.JTree.DropLocation = tree.dropLocation
			If ___dropLocation IsNot Nothing AndAlso ___dropLocation.childIndex = -1 AndAlso tree.getRowForPath(___dropLocation.path) = row Then

				Dim col As java.awt.Color = sun.swing.DefaultLookup.getColor(Me, ui, "Tree.dropCellForeground")
				If col IsNot Nothing Then
					fg = col
				Else
					fg = textSelectionColor
				End If

				isDropCell = True
			ElseIf sel Then
				fg = textSelectionColor
			Else
				fg = textNonSelectionColor
			End If

			foreground = fg

			Dim ___icon As javax.swing.Icon = Nothing
			If leaf Then
				___icon = leafIcon
			ElseIf expanded Then
				___icon = openIcon
			Else
				___icon = closedIcon
			End If

			If Not tree.enabled Then
				enabled = False
				Dim laf As javax.swing.LookAndFeel = javax.swing.UIManager.lookAndFeel
				Dim ___disabledIcon As javax.swing.Icon = laf.getDisabledIcon(tree, ___icon)
				If ___disabledIcon IsNot Nothing Then ___icon = ___disabledIcon
				disabledIcon = ___icon
			Else
				enabled = True
				icon = ___icon
			End If
			componentOrientation = tree.componentOrientation

			selected = sel

			Return Me
		End Function

		''' <summary>
		''' Paints the value.  The background is filled based on selected.
		''' </summary>
		Public Overridable Sub paint(ByVal g As java.awt.Graphics)
			Dim bColor As java.awt.Color

			If isDropCell Then
				bColor = sun.swing.DefaultLookup.getColor(Me, ui, "Tree.dropCellBackground")
				If bColor Is Nothing Then bColor = backgroundSelectionColor
			ElseIf selected Then
				bColor = backgroundSelectionColor
			Else
				bColor = backgroundNonSelectionColor
				If bColor Is Nothing Then bColor = background
			End If

			Dim imageOffset As Integer = -1
			If bColor IsNot Nothing AndAlso fillBackground Then
				imageOffset = labelStart
				g.color = bColor
				If componentOrientation.leftToRight Then
					g.fillRect(imageOffset, 0, width - imageOffset, height)
				Else
					g.fillRect(0, 0, width - imageOffset, height)
				End If
			End If

			If hasFocus Then
				If drawsFocusBorderAroundIcon Then
					imageOffset = 0
				ElseIf imageOffset = -1 Then
					imageOffset = labelStart
				End If
				If componentOrientation.leftToRight Then
					paintFocus(g, imageOffset, 0, width - imageOffset, height, bColor)
				Else
					paintFocus(g, 0, 0, width - imageOffset, height, bColor)
				End If
			End If
			MyBase.paint(g)
		End Sub

		Private Sub paintFocus(ByVal g As java.awt.Graphics, ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer, ByVal notColor As java.awt.Color)
			Dim bsColor As java.awt.Color = borderSelectionColor

			If bsColor IsNot Nothing AndAlso (selected OrElse (Not drawDashedFocusIndicator)) Then
				g.color = bsColor
				g.drawRect(x, y, w - 1, h - 1)
			End If
			If drawDashedFocusIndicator AndAlso notColor IsNot Nothing Then
				If treeBGColor IsNot notColor Then
					treeBGColor = notColor
					focusBGColor = New java.awt.Color((Not notColor.rGB))
				End If
				g.color = focusBGColor
				javax.swing.plaf.basic.BasicGraphicsUtils.drawDashedRect(g, x, y, w, h)
			End If
		End Sub

		Private Property labelStart As Integer
			Get
				Dim currentI As javax.swing.Icon = icon
				If currentI IsNot Nothing AndAlso text IsNot Nothing Then Return currentI.iconWidth + Math.Max(0, iconTextGap - 1)
				Return 0
			End Get
		End Property

		''' <summary>
		''' Overrides <code>JComponent.getPreferredSize</code> to
		''' return slightly wider preferred size value.
		''' </summary>
		Public Property Overrides preferredSize As java.awt.Dimension
			Get
				Dim retDimension As java.awt.Dimension = MyBase.preferredSize
    
				If retDimension IsNot Nothing Then retDimension = New java.awt.Dimension(retDimension.width + 3, retDimension.height)
				Return retDimension
			End Get
		End Property

	   ''' <summary>
	   ''' Overridden for performance reasons.
	   ''' See the <a href="#override">Implementation Note</a>
	   ''' for more information.
	   ''' </summary>
		Public Overridable Sub validate()
		End Sub

	   ''' <summary>
	   ''' Overridden for performance reasons.
	   ''' See the <a href="#override">Implementation Note</a>
	   ''' for more information.
	   ''' 
	   ''' @since 1.5
	   ''' </summary>
		Public Overridable Sub invalidate()
		End Sub

	   ''' <summary>
	   ''' Overridden for performance reasons.
	   ''' See the <a href="#override">Implementation Note</a>
	   ''' for more information.
	   ''' </summary>
		Public Overrides Sub revalidate()
		End Sub

	   ''' <summary>
	   ''' Overridden for performance reasons.
	   ''' See the <a href="#override">Implementation Note</a>
	   ''' for more information.
	   ''' </summary>
		Public Overrides Sub repaint(ByVal tm As Long, ByVal x As Integer, ByVal y As Integer, ByVal width As Integer, ByVal height As Integer)
		End Sub

	   ''' <summary>
	   ''' Overridden for performance reasons.
	   ''' See the <a href="#override">Implementation Note</a>
	   ''' for more information.
	   ''' </summary>
		Public Overridable Sub repaint(ByVal r As java.awt.Rectangle)
		End Sub

	   ''' <summary>
	   ''' Overridden for performance reasons.
	   ''' See the <a href="#override">Implementation Note</a>
	   ''' for more information.
	   ''' 
	   ''' @since 1.5
	   ''' </summary>
		Public Overridable Sub repaint()
		End Sub

	   ''' <summary>
	   ''' Overridden for performance reasons.
	   ''' See the <a href="#override">Implementation Note</a>
	   ''' for more information.
	   ''' </summary>
		Protected Friend Overridable Sub firePropertyChange(ByVal propertyName As String, ByVal oldValue As Object, ByVal newValue As Object)
			' Strings get interned...
			If propertyName = "text" OrElse ((propertyName = "font" OrElse propertyName = "foreground") AndAlso oldValue IsNot newValue AndAlso getClientProperty(javax.swing.plaf.basic.BasicHTML.propertyKey) IsNot Nothing) Then MyBase.firePropertyChange(propertyName, oldValue, newValue)
		End Sub

	   ''' <summary>
	   ''' Overridden for performance reasons.
	   ''' See the <a href="#override">Implementation Note</a>
	   ''' for more information.
	   ''' </summary>
		Public Overridable Sub firePropertyChange(ByVal propertyName As String, ByVal oldValue As SByte, ByVal newValue As SByte)
		End Sub

	   ''' <summary>
	   ''' Overridden for performance reasons.
	   ''' See the <a href="#override">Implementation Note</a>
	   ''' for more information.
	   ''' </summary>
		Public Overrides Sub firePropertyChange(ByVal propertyName As String, ByVal oldValue As Char, ByVal newValue As Char)
		End Sub

	   ''' <summary>
	   ''' Overridden for performance reasons.
	   ''' See the <a href="#override">Implementation Note</a>
	   ''' for more information.
	   ''' </summary>
		Public Overridable Sub firePropertyChange(ByVal propertyName As String, ByVal oldValue As Short, ByVal newValue As Short)
		End Sub

	   ''' <summary>
	   ''' Overridden for performance reasons.
	   ''' See the <a href="#override">Implementation Note</a>
	   ''' for more information.
	   ''' </summary>
		Public Overrides Sub firePropertyChange(ByVal propertyName As String, ByVal oldValue As Integer, ByVal newValue As Integer)
		End Sub

	   ''' <summary>
	   ''' Overridden for performance reasons.
	   ''' See the <a href="#override">Implementation Note</a>
	   ''' for more information.
	   ''' </summary>
		Public Overridable Sub firePropertyChange(ByVal propertyName As String, ByVal oldValue As Long, ByVal newValue As Long)
		End Sub

	   ''' <summary>
	   ''' Overridden for performance reasons.
	   ''' See the <a href="#override">Implementation Note</a>
	   ''' for more information.
	   ''' </summary>
		Public Overridable Sub firePropertyChange(ByVal propertyName As String, ByVal oldValue As Single, ByVal newValue As Single)
		End Sub

	   ''' <summary>
	   ''' Overridden for performance reasons.
	   ''' See the <a href="#override">Implementation Note</a>
	   ''' for more information.
	   ''' </summary>
		Public Overridable Sub firePropertyChange(ByVal propertyName As String, ByVal oldValue As Double, ByVal newValue As Double)
		End Sub

	   ''' <summary>
	   ''' Overridden for performance reasons.
	   ''' See the <a href="#override">Implementation Note</a>
	   ''' for more information.
	   ''' </summary>
		Public Overrides Sub firePropertyChange(ByVal propertyName As String, ByVal oldValue As Boolean, ByVal newValue As Boolean)
		End Sub

	End Class

End Namespace