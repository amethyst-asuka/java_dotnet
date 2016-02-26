Imports Microsoft.VisualBasic
Imports System
Imports System.Diagnostics
Imports System.Collections

'
' * Copyright (c) 2002, 2013, Oracle and/or its affiliates. All rights reserved.
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
	''' Provides the Synth L&amp;F UI delegate for
	''' <seealso cref="javax.swing.JTree"/>.
	''' 
	''' @author Scott Violet
	''' @since 1.7
	''' </summary>
	Public Class SynthTreeUI
		Inherits javax.swing.plaf.basic.BasicTreeUI
		Implements java.beans.PropertyChangeListener, SynthUI

		Private style As SynthStyle
		Private cellStyle As SynthStyle

		Private paintContext As SynthContext

		Private drawHorizontalLines As Boolean
		Private drawVerticalLines As Boolean

		Private linesStyle As Object

		Private padding As Integer

		Private useTreeColors As Boolean

		Private expandedIconWrapper As javax.swing.Icon = New ExpandedIconWrapper(Me)

		''' <summary>
		''' Creates a new UI object for the given component.
		''' </summary>
		''' <param name="x"> component to create UI object for </param>
		''' <returns> the UI object </returns>
		Public Shared Function createUI(ByVal x As javax.swing.JComponent) As javax.swing.plaf.ComponentUI
			Return New SynthTreeUI
		End Function

		''' <summary>
		''' {@inheritDoc}
		''' </summary>
		Public Property Overrides expandedIcon As javax.swing.Icon
			Get
				Return expandedIconWrapper
			End Get
		End Property

		''' <summary>
		''' {@inheritDoc}
		''' </summary>
		Protected Friend Overrides Sub installDefaults()
			updateStyle(tree)
		End Sub

		Private Sub updateStyle(ByVal tree As javax.swing.JTree)
			Dim ___context As SynthContext = getContext(tree, ENABLED)
			Dim oldStyle As SynthStyle = style

			style = SynthLookAndFeel.updateStyle(___context, Me)
			If style IsNot oldStyle Then
				Dim value As Object

				expandedIcon = style.getIcon(___context, "Tree.expandedIcon")
				collapsedIcon = style.getIcon(___context, "Tree.collapsedIcon")

				leftChildIndent = style.getInt(___context, "Tree.leftChildIndent", 0)
				rightChildIndent = style.getInt(___context, "Tree.rightChildIndent", 0)

				drawHorizontalLines = style.getBoolean(___context, "Tree.drawHorizontalLines",True)
				drawVerticalLines = style.getBoolean(___context, "Tree.drawVerticalLines", True)
				linesStyle = style.get(___context, "Tree.linesStyle")

					value = style.get(___context, "Tree.rowHeight")
					If value IsNot Nothing Then javax.swing.LookAndFeel.installProperty(tree, "rowHeight", value)

					value = style.get(___context, "Tree.scrollsOnExpand")
					javax.swing.LookAndFeel.installProperty(tree, "scrollsOnExpand",If(value IsNot Nothing, value, Boolean.TRUE))

				padding = style.getInt(___context, "Tree.padding", 0)

				largeModel = (tree.largeModel AndAlso tree.rowHeight > 0)

				useTreeColors = style.getBoolean(___context, "Tree.rendererUseTreeColors", True)

				Dim ___showsRootHandles As Boolean? = style.getBoolean(___context, "Tree.showsRootHandles", Boolean.TRUE)
				javax.swing.LookAndFeel.installProperty(tree, javax.swing.JTree.SHOWS_ROOT_HANDLES_PROPERTY, ___showsRootHandles)

				If oldStyle IsNot Nothing Then
					uninstallKeyboardActions()
					installKeyboardActions()
				End If
			End If
			___context.Dispose()

			___context = getContext(tree, Region.TREE_CELL, ENABLED)
			cellStyle = SynthLookAndFeel.updateStyle(___context, Me)
			___context.Dispose()
		End Sub

		''' <summary>
		''' {@inheritDoc}
		''' </summary>
		Protected Friend Overrides Sub installListeners()
			MyBase.installListeners()
			tree.addPropertyChangeListener(Me)
		End Sub

		''' <summary>
		''' {@inheritDoc}
		''' </summary>
		Public Overrides Function getContext(ByVal c As javax.swing.JComponent) As SynthContext Implements SynthUI.getContext
			Return getContext(c, SynthLookAndFeel.getComponentState(c))
		End Function

		Private Function getContext(ByVal c As javax.swing.JComponent, ByVal state As Integer) As SynthContext
			Return SynthContext.getContext(c, style, state)
		End Function

		Private Function getContext(ByVal c As javax.swing.JComponent, ByVal ___region As Region) As SynthContext
			Return getContext(c, ___region, getComponentState(c, ___region))
		End Function

		Private Function getContext(ByVal c As javax.swing.JComponent, ByVal ___region As Region, ByVal state As Integer) As SynthContext
			Return SynthContext.getContext(c, ___region, cellStyle, state)
		End Function

		Private Function getComponentState(ByVal c As javax.swing.JComponent, ByVal ___region As Region) As Integer
			' Always treat the cell as selected, will be adjusted appropriately
			' when painted.
			Return ENABLED Or SELECTED
		End Function

		''' <summary>
		''' {@inheritDoc}
		''' </summary>
		Protected Friend Overrides Function createDefaultCellEditor() As javax.swing.tree.TreeCellEditor
			Dim renderer As javax.swing.tree.TreeCellRenderer = tree.cellRenderer
			Dim editor As javax.swing.tree.DefaultTreeCellEditor

			If renderer IsNot Nothing AndAlso (TypeOf renderer Is javax.swing.tree.DefaultTreeCellRenderer) Then
				editor = New SynthTreeCellEditor(tree, CType(renderer, javax.swing.tree.DefaultTreeCellRenderer))
			Else
				editor = New SynthTreeCellEditor(tree, Nothing)
			End If
			Return editor
		End Function

		''' <summary>
		''' {@inheritDoc}
		''' </summary>
		Protected Friend Overrides Function createDefaultCellRenderer() As javax.swing.tree.TreeCellRenderer
			Return New SynthTreeCellRenderer(Me)
		End Function

		''' <summary>
		''' {@inheritDoc}
		''' </summary>
		Protected Friend Overrides Sub uninstallDefaults()
			Dim ___context As SynthContext = getContext(tree, ENABLED)

			style.uninstallDefaults(___context)
			___context.Dispose()
			style = Nothing

			___context = getContext(tree, Region.TREE_CELL, ENABLED)
			cellStyle.uninstallDefaults(___context)
			___context.Dispose()
			cellStyle = Nothing


			If TypeOf tree.transferHandler Is javax.swing.plaf.UIResource Then tree.transferHandler = Nothing
		End Sub

		''' <summary>
		''' {@inheritDoc}
		''' </summary>
		Protected Friend Overrides Sub uninstallListeners()
			MyBase.uninstallListeners()
			tree.removePropertyChangeListener(Me)
		End Sub

		''' <summary>
		''' Notifies this UI delegate to repaint the specified component.
		''' This method paints the component background, then calls
		''' the <seealso cref="#paint(SynthContext,Graphics)"/> method.
		''' 
		''' <p>In general, this method does not need to be overridden by subclasses.
		''' All Look and Feel rendering code should reside in the {@code paint} method.
		''' </summary>
		''' <param name="g"> the {@code Graphics} object used for painting </param>
		''' <param name="c"> the component being painted </param>
		''' <seealso cref= #paint(SynthContext,Graphics) </seealso>
		Public Overrides Sub update(ByVal g As java.awt.Graphics, ByVal c As javax.swing.JComponent)
			Dim ___context As SynthContext = getContext(c)

			SynthLookAndFeel.update(___context, g)
			___context.painter.paintTreeBackground(___context, g, 0, 0, c.width, c.height)
			paint(___context, g)
			___context.Dispose()
		End Sub

		''' <summary>
		''' {@inheritDoc}
		''' </summary>
		Public Overrides Sub paintBorder(ByVal context As SynthContext, ByVal g As java.awt.Graphics, ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer) Implements SynthUI.paintBorder
			context.painter.paintTreeBorder(context, g, x, y, w, h)
		End Sub

		''' <summary>
		''' Paints the specified component according to the Look and Feel.
		''' <p>This method is not used by Synth Look and Feel.
		''' Painting is handled by the <seealso cref="#paint(SynthContext,Graphics)"/> method.
		''' </summary>
		''' <param name="g"> the {@code Graphics} object used for painting </param>
		''' <param name="c"> the component being painted </param>
		''' <seealso cref= #paint(SynthContext,Graphics) </seealso>
		Public Overrides Sub paint(ByVal g As java.awt.Graphics, ByVal c As javax.swing.JComponent)
			Dim ___context As SynthContext = getContext(c)

			paint(___context, g)
			___context.Dispose()
		End Sub

		''' <summary>
		''' Paints the specified component.
		''' </summary>
		''' <param name="context"> context for the component being painted </param>
		''' <param name="g"> the {@code Graphics} object used for painting </param>
		''' <seealso cref= #update(Graphics,JComponent) </seealso>
		Protected Friend Overridable Sub paint(ByVal context As SynthContext, ByVal g As java.awt.Graphics)
			paintContext = context

			updateLeadSelectionRow()

			Dim paintBounds As java.awt.Rectangle = g.clipBounds
			Dim insets As java.awt.Insets = tree.insets
			Dim initialPath As javax.swing.tree.TreePath = getClosestPathForLocation(tree, 0, paintBounds.y)
			Dim paintingEnumerator As System.Collections.IEnumerator = treeState.getVisiblePathsFrom(initialPath)
			Dim row As Integer = treeState.getRowForPath(initialPath)
			Dim endY As Integer = paintBounds.y + paintBounds.height
			Dim treeModel As javax.swing.tree.TreeModel = tree.model
			Dim cellContext As SynthContext = getContext(tree, Region.TREE_CELL)

			drawingCache.Clear()

			hashColor = context.style.getColor(context, ColorType.FOREGROUND)

			If paintingEnumerator IsNot Nothing Then
				' First pass, draw the rows

				Dim done As Boolean = False
				Dim isExpanded As Boolean
				Dim hasBeenExpanded As Boolean
				Dim isLeaf As Boolean
				Dim rowBounds As New java.awt.Rectangle(0, 0, tree.width,0)
				Dim bounds As java.awt.Rectangle
				Dim path As javax.swing.tree.TreePath
				Dim renderer As javax.swing.tree.TreeCellRenderer = tree.cellRenderer
				Dim dtcr As javax.swing.tree.DefaultTreeCellRenderer = If(TypeOf renderer Is javax.swing.tree.DefaultTreeCellRenderer, CType(renderer, javax.swing.tree.DefaultTreeCellRenderer), Nothing)

				configureRenderer(cellContext)
				Do While (Not done) AndAlso paintingEnumerator.hasMoreElements()
					path = CType(paintingEnumerator.nextElement(), javax.swing.tree.TreePath)
					bounds = getPathBounds(tree, path)
					If (path IsNot Nothing) AndAlso (bounds IsNot Nothing) Then
						isLeaf = treeModel.isLeaf(path.lastPathComponent)
						If isLeaf Then
								hasBeenExpanded = False
								isExpanded = hasBeenExpanded
						Else
							isExpanded = treeState.getExpandedState(path)
							hasBeenExpanded = tree.hasBeenExpanded(path)
						End If
						rowBounds.y = bounds.y
						rowBounds.height = bounds.height
						paintRow(renderer, dtcr, context, cellContext, g, paintBounds, insets, bounds, rowBounds, path, row, isExpanded, hasBeenExpanded, isLeaf)
						If (bounds.y + bounds.height) >= endY Then done = True
					Else
						done = True
					End If
					row += 1
				Loop

				' Draw the connecting lines and controls.
				' Find each parent and have them draw a line to their last child
				Dim ___rootVisible As Boolean = tree.rootVisible
				Dim parentPath As javax.swing.tree.TreePath = initialPath
				parentPath = parentPath.parentPath
				Do While parentPath IsNot Nothing
					paintVerticalPartOfLeg(g, paintBounds, insets, parentPath)
					drawingCache(parentPath) = Boolean.TRUE
					parentPath = parentPath.parentPath
				Loop
				done = False
				paintingEnumerator = treeState.getVisiblePathsFrom(initialPath)
				Do While (Not done) AndAlso paintingEnumerator.hasMoreElements()
					path = CType(paintingEnumerator.nextElement(), javax.swing.tree.TreePath)
					bounds = getPathBounds(tree, path)
					If (path IsNot Nothing) AndAlso (bounds IsNot Nothing) Then
						isLeaf = treeModel.isLeaf(path.lastPathComponent)
						If isLeaf Then
								hasBeenExpanded = False
								isExpanded = hasBeenExpanded
						Else
							isExpanded = treeState.getExpandedState(path)
							hasBeenExpanded = tree.hasBeenExpanded(path)
						End If
						' See if the vertical line to the parent has been drawn.
						parentPath = path.parentPath
						If parentPath IsNot Nothing Then
							If drawingCache(parentPath) Is Nothing Then
								paintVerticalPartOfLeg(g, paintBounds, insets, parentPath)
								drawingCache(parentPath) = Boolean.TRUE
							End If
							paintHorizontalPartOfLeg(g, paintBounds, insets, bounds, path, row, isExpanded, hasBeenExpanded, isLeaf)
						ElseIf ___rootVisible AndAlso row = 0 Then
							paintHorizontalPartOfLeg(g, paintBounds, insets, bounds, path, row, isExpanded, hasBeenExpanded, isLeaf)
						End If
						If shouldPaintExpandControl(path, row, isExpanded, hasBeenExpanded, isLeaf) Then paintExpandControl(g, paintBounds, insets, bounds, path, row, isExpanded, hasBeenExpanded,isLeaf)
						If (bounds.y + bounds.height) >= endY Then done = True
					Else
						done = True
					End If
					row += 1
				Loop
			End If
			cellContext.Dispose()

			paintDropLine(g)

			' Empty out the renderer pane, allowing renderers to be gc'ed.
			rendererPane.removeAll()

			paintContext = Nothing
		End Sub

		Private Sub configureRenderer(ByVal context As SynthContext)
			Dim renderer As javax.swing.tree.TreeCellRenderer = tree.cellRenderer

			If TypeOf renderer Is javax.swing.tree.DefaultTreeCellRenderer Then
				Dim r As javax.swing.tree.DefaultTreeCellRenderer = CType(renderer, javax.swing.tree.DefaultTreeCellRenderer)
				Dim style As SynthStyle = context.style

				context.componentState = ENABLED Or SELECTED
				Dim color As java.awt.Color = r.textSelectionColor
				If color Is Nothing OrElse (TypeOf color Is javax.swing.plaf.UIResource) Then r.textSelectionColor = style.getColor(context, ColorType.TEXT_FOREGROUND)
				color = r.backgroundSelectionColor
				If color Is Nothing OrElse (TypeOf color Is javax.swing.plaf.UIResource) Then r.backgroundSelectionColor = style.getColor(context, ColorType.TEXT_BACKGROUND)

				context.componentState = ENABLED
				color = r.textNonSelectionColor
				If color Is Nothing OrElse TypeOf color Is javax.swing.plaf.UIResource Then r.textNonSelectionColor = style.getColorForState(context, ColorType.TEXT_FOREGROUND)
				color = r.backgroundNonSelectionColor
				If color Is Nothing OrElse TypeOf color Is javax.swing.plaf.UIResource Then r.backgroundNonSelectionColor = style.getColorForState(context, ColorType.TEXT_BACKGROUND)
			End If
		End Sub

		''' <summary>
		''' {@inheritDoc}
		''' </summary>
		Protected Friend Overrides Sub paintHorizontalPartOfLeg(ByVal g As java.awt.Graphics, ByVal clipBounds As java.awt.Rectangle, ByVal insets As java.awt.Insets, ByVal bounds As java.awt.Rectangle, ByVal path As javax.swing.tree.TreePath, ByVal row As Integer, ByVal isExpanded As Boolean, ByVal hasBeenExpanded As Boolean, ByVal isLeaf As Boolean)
			If drawHorizontalLines Then MyBase.paintHorizontalPartOfLeg(g, clipBounds, insets, bounds, path, row, isExpanded, hasBeenExpanded, isLeaf)
		End Sub

		''' <summary>
		''' {@inheritDoc}
		''' </summary>
		Protected Friend Overrides Sub paintHorizontalLine(ByVal g As java.awt.Graphics, ByVal c As javax.swing.JComponent, ByVal y As Integer, ByVal left As Integer, ByVal right As Integer)
			paintContext.style.getGraphicsUtils(paintContext).drawLine(paintContext, "Tree.horizontalLine", g, left, y, right, y, linesStyle)
		End Sub

		''' <summary>
		''' {@inheritDoc}
		''' </summary>
		Protected Friend Overrides Sub paintVerticalPartOfLeg(ByVal g As java.awt.Graphics, ByVal clipBounds As java.awt.Rectangle, ByVal insets As java.awt.Insets, ByVal path As javax.swing.tree.TreePath)
			If drawVerticalLines Then MyBase.paintVerticalPartOfLeg(g, clipBounds, insets, path)
		End Sub

		''' <summary>
		''' {@inheritDoc}
		''' </summary>
		Protected Friend Overrides Sub paintVerticalLine(ByVal g As java.awt.Graphics, ByVal c As javax.swing.JComponent, ByVal x As Integer, ByVal top As Integer, ByVal bottom As Integer)
			paintContext.style.getGraphicsUtils(paintContext).drawLine(paintContext, "Tree.verticalLine", g, x, top, x, bottom, linesStyle)
		End Sub

		Private Sub paintRow(ByVal renderer As javax.swing.tree.TreeCellRenderer, ByVal dtcr As javax.swing.tree.DefaultTreeCellRenderer, ByVal treeContext As SynthContext, ByVal cellContext As SynthContext, ByVal g As java.awt.Graphics, ByVal clipBounds As java.awt.Rectangle, ByVal insets As java.awt.Insets, ByVal bounds As java.awt.Rectangle, ByVal rowBounds As java.awt.Rectangle, ByVal path As javax.swing.tree.TreePath, ByVal row As Integer, ByVal isExpanded As Boolean, ByVal hasBeenExpanded As Boolean, ByVal isLeaf As Boolean)
			' Don't paint the renderer if editing this row.
			Dim selected As Boolean = tree.isRowSelected(row)

			Dim dropLocation As javax.swing.JTree.DropLocation = tree.dropLocation
			Dim isDrop As Boolean = dropLocation IsNot Nothing AndAlso dropLocation.childIndex = -1 AndAlso path Is dropLocation.path

			Dim state As Integer = ENABLED
			If selected OrElse isDrop Then state = state Or SELECTED

			If tree.focusOwner AndAlso row = leadSelectionRow Then state = state Or FOCUSED

			cellContext.componentState = state

			If dtcr IsNot Nothing AndAlso (TypeOf dtcr.borderSelectionColor Is javax.swing.plaf.UIResource) Then dtcr.borderSelectionColor = style.getColor(cellContext, ColorType.FOCUS)
			SynthLookAndFeel.updateSubregion(cellContext, g, rowBounds)
			cellContext.painter.paintTreeCellBackground(cellContext, g, rowBounds.x, rowBounds.y, rowBounds.width, rowBounds.height)
			cellContext.painter.paintTreeCellBorder(cellContext, g, rowBounds.x, rowBounds.y, rowBounds.width, rowBounds.height)
			If editingComponent IsNot Nothing AndAlso editingRow = row Then Return

			Dim leadIndex As Integer

			If tree.hasFocus() Then
				leadIndex = leadSelectionRow
			Else
				leadIndex = -1
			End If

			Dim component As java.awt.Component = renderer.getTreeCellRendererComponent(tree, path.lastPathComponent, selected, isExpanded, isLeaf, row, (leadIndex = row))

			rendererPane.paintComponent(g, component, tree, bounds.x, bounds.y, bounds.width, bounds.height, True)
		End Sub

		Private Function findCenteredX(ByVal x As Integer, ByVal iconWidth As Integer) As Integer
			Return If(tree.componentOrientation.leftToRight, x - CInt(Fix(Math.Ceiling(iconWidth / 2.0))), x - CInt(Fix(Math.Floor(iconWidth / 2.0))))
		End Function

		''' <summary>
		''' {@inheritDoc}
		''' </summary>
		Protected Friend Overrides Sub paintExpandControl(ByVal g As java.awt.Graphics, ByVal clipBounds As java.awt.Rectangle, ByVal insets As java.awt.Insets, ByVal bounds As java.awt.Rectangle, ByVal path As javax.swing.tree.TreePath, ByVal row As Integer, ByVal isExpanded As Boolean, ByVal hasBeenExpanded As Boolean, ByVal isLeaf As Boolean)
			'modify the paintContext's state to match the state for the row
			'this is a hack in that it requires knowledge of the subsequent
			'method calls. The point is, the context used in drawCentered
			'should reflect the state of the row, not of the tree.
			Dim isSelected As Boolean = tree.selectionModel.isPathSelected(path)
			Dim state As Integer = paintContext.componentState
			If isSelected Then paintContext.componentState = state Or SynthConstants.SELECTED
			MyBase.paintExpandControl(g, clipBounds, insets, bounds, path, row, isExpanded, hasBeenExpanded, isLeaf)
			paintContext.componentState = state
		End Sub

		''' <summary>
		''' {@inheritDoc}
		''' </summary>
		Protected Friend Overrides Sub drawCentered(ByVal c As java.awt.Component, ByVal graphics As java.awt.Graphics, ByVal icon As javax.swing.Icon, ByVal x As Integer, ByVal y As Integer)
			Dim w As Integer = sun.swing.plaf.synth.SynthIcon.getIconWidth(icon, paintContext)
			Dim h As Integer = sun.swing.plaf.synth.SynthIcon.getIconHeight(icon, paintContext)

			sun.swing.plaf.synth.SynthIcon.paintIcon(icon, paintContext, graphics, findCenteredX(x, w), y - h\2, w, h)
		End Sub

		''' <summary>
		''' {@inheritDoc}
		''' </summary>
		Public Overrides Sub propertyChange(ByVal [event] As java.beans.PropertyChangeEvent)
			If SynthLookAndFeel.shouldUpdateStyle([event]) Then updateStyle(CType([event].source, javax.swing.JTree))

			If "dropLocation" = [event].propertyName Then
				Dim oldValue As javax.swing.JTree.DropLocation = CType([event].oldValue, javax.swing.JTree.DropLocation)
				repaintDropLocation(oldValue)
				repaintDropLocation(tree.dropLocation)
			End If
		End Sub

		''' <summary>
		''' {@inheritDoc}
		''' </summary>
		Protected Friend Overrides Sub paintDropLine(ByVal g As java.awt.Graphics)
			Dim loc As javax.swing.JTree.DropLocation = tree.dropLocation
			If Not isDropLine(loc) Then Return

			Dim c As java.awt.Color = CType(style.get(paintContext, "Tree.dropLineColor"), java.awt.Color)
			If c IsNot Nothing Then
				g.color = c
				Dim rect As java.awt.Rectangle = getDropLineRect(loc)
				g.fillRect(rect.x, rect.y, rect.width, rect.height)
			End If
		End Sub

		Private Sub repaintDropLocation(ByVal loc As javax.swing.JTree.DropLocation)
			If loc Is Nothing Then Return

			Dim r As java.awt.Rectangle

			If isDropLine(loc) Then
				r = getDropLineRect(loc)
			Else
				r = tree.getPathBounds(loc.path)
				If r IsNot Nothing Then
					r.x = 0
					r.width = tree.width
				End If
			End If

			If r IsNot Nothing Then tree.repaint(r)
		End Sub

		''' <summary>
		''' {@inheritDoc}
		''' </summary>
		Protected Friend Overrides Function getRowX(ByVal row As Integer, ByVal depth As Integer) As Integer
			Return MyBase.getRowX(row, depth) + padding
		End Function


		Private Class SynthTreeCellRenderer
			Inherits javax.swing.tree.DefaultTreeCellRenderer
			Implements javax.swing.plaf.UIResource

			Private ReadOnly outerInstance As SynthTreeUI

			Friend Sub New(ByVal outerInstance As SynthTreeUI)
					Me.outerInstance = outerInstance
			End Sub

			Public Property Overrides name As String
				Get
					Return "Tree.cellRenderer"
				End Get
			End Property

			Public Overrides Function getTreeCellRendererComponent(ByVal tree As javax.swing.JTree, ByVal value As Object, ByVal sel As Boolean, ByVal expanded As Boolean, ByVal leaf As Boolean, ByVal row As Integer, ByVal hasFocus As Boolean) As java.awt.Component
				If (Not outerInstance.useTreeColors) AndAlso (sel OrElse hasFocus) Then
					SynthLookAndFeel.selectedUIdUI(CType(SynthLookAndFeel.getUIOfType(uI, GetType(SynthLabelUI)), SynthLabelUI), sel, hasFocus, tree.enabled, False)
				Else
					SynthLookAndFeel.resetSelectedUI()
				End If
				Return MyBase.getTreeCellRendererComponent(tree, value, sel, expanded, leaf, row, hasFocus)
			End Function

			Public Overrides Sub paint(ByVal g As java.awt.Graphics)
				paintComponent(g)
				If hasFocus Then
					Dim context As SynthContext = outerInstance.getContext(outerInstance.tree, Region.TREE_CELL)

					If context.style Is Nothing Then
						Debug.Assert(False, "SynthTreeCellRenderer is being used " & "outside of UI that created it")
						Return
					End If
					Dim imageOffset As Integer = 0
					Dim currentI As javax.swing.Icon = icon

					If currentI IsNot Nothing AndAlso text IsNot Nothing Then imageOffset = currentI.iconWidth + Math.Max(0, iconTextGap - 1)
					If selected Then
						context.componentState = ENABLED Or SELECTED
					Else
						context.componentState = ENABLED
					End If
					If componentOrientation.leftToRight Then
						context.painter.paintTreeCellFocus(context, g, imageOffset, 0, width - imageOffset, height)
					Else
						context.painter.paintTreeCellFocus(context, g, 0, 0, width - imageOffset, height)
					End If
					context.Dispose()
				End If
				SynthLookAndFeel.resetSelectedUI()
			End Sub
		End Class


		Private Class SynthTreeCellEditor
			Inherits javax.swing.tree.DefaultTreeCellEditor

			Public Sub New(ByVal tree As javax.swing.JTree, ByVal renderer As javax.swing.tree.DefaultTreeCellRenderer)
				MyBase.New(tree, renderer)
				borderSelectionColor = Nothing
			End Sub

			Protected Friend Overrides Function createTreeCellEditor() As javax.swing.tree.TreeCellEditor
				Dim tf As javax.swing.JTextField = New JTextFieldAnonymousInnerClassHelper
				Dim editor As New javax.swing.DefaultCellEditor(tf)

				' One click to edit.
				editor.clickCountToStart = 1
				Return editor
			End Function

			Private Class JTextFieldAnonymousInnerClassHelper
				Inherits javax.swing.JTextField

				Public Property Overrides name As String
					Get
						Return "Tree.cellEditor"
					End Get
				End Property
			End Class
		End Class

		'
		' BasicTreeUI directly uses expandIcon outside of the Synth methods.
		' To get the correct context we return an instance of this that fetches
		' the SynthContext as needed.
		'
		Private Class ExpandedIconWrapper
			Inherits sun.swing.plaf.synth.SynthIcon

			Private ReadOnly outerInstance As SynthTreeUI

			Public Sub New(ByVal outerInstance As SynthTreeUI)
				Me.outerInstance = outerInstance
			End Sub

			Public Overridable Sub paintIcon(ByVal context As SynthContext, ByVal g As java.awt.Graphics, ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer)
				If context Is Nothing Then
					context = outerInstance.getContext(outerInstance.tree)
					sun.swing.plaf.synth.SynthIcon.paintIcon(outerInstance.expandedIcon, context, g, x, y, w, h)
					context.Dispose()
				Else
					sun.swing.plaf.synth.SynthIcon.paintIcon(outerInstance.expandedIcon, context, g, x, y, w, h)
				End If
			End Sub

			Public Overridable Function getIconWidth(ByVal context As SynthContext) As Integer
				Dim width As Integer
				If context Is Nothing Then
					context = outerInstance.getContext(outerInstance.tree)
					width = sun.swing.plaf.synth.SynthIcon.getIconWidth(outerInstance.expandedIcon, context)
					context.Dispose()
				Else
					width = sun.swing.plaf.synth.SynthIcon.getIconWidth(outerInstance.expandedIcon, context)
				End If
				Return width
			End Function

			Public Overridable Function getIconHeight(ByVal context As SynthContext) As Integer
				Dim height As Integer
				If context Is Nothing Then
					context = outerInstance.getContext(outerInstance.tree)
					height = sun.swing.plaf.synth.SynthIcon.getIconHeight(outerInstance.expandedIcon, context)
					context.Dispose()
				Else
					height = sun.swing.plaf.synth.SynthIcon.getIconHeight(outerInstance.expandedIcon, context)
				End If
				Return height
			End Function
		End Class
	End Class

End Namespace