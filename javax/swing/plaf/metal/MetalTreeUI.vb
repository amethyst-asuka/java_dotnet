Imports javax.swing
Imports javax.swing.event
Imports javax.swing.plaf
Imports javax.swing.tree
Imports javax.swing.plaf.basic

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
	''' The metal look and feel implementation of <code>TreeUI</code>.
	''' <p>
	''' <code>MetalTreeUI</code> allows for configuring how to
	''' visually render the spacing and delineation between nodes. The following
	''' hints are supported:
	''' 
	''' <table summary="Descriptions of supported hints: Angled, Horizontal, and None">
	'''  <tr>
	'''    <th><p style="text-align:left">Angled</p></th>
	'''    <td>A line is drawn connecting the child to the parent. For handling
	'''          of the root node refer to
	'''          <seealso cref="javax.swing.JTree#setRootVisible"/> and
	'''          <seealso cref="javax.swing.JTree#setShowsRootHandles"/>.
	'''    </td>
	'''  </tr>
	'''  <tr>
	'''     <th><p style="text-align:left">Horizontal</p></th>
	'''     <td>A horizontal line is drawn dividing the children of the root node.</td>
	'''  </tr>
	'''  <tr>
	'''      <th><p style="text-align:left">None</p></th>
	'''      <td>Do not draw any visual indication between nodes.</td>
	'''  </tr>
	''' </table>
	''' 
	''' <p>
	''' As it is typically impractical to obtain the <code>TreeUI</code> from
	''' the <code>JTree</code> and cast to an instance of <code>MetalTreeUI</code>
	''' you enable this property via the client property
	''' <code>JTree.lineStyle</code>. For example, to switch to
	''' <code>Horizontal</code> style you would do:
	''' <code>tree.putClientProperty("JTree.lineStyle", "Horizontal");</code>
	''' <p>
	''' The default is <code>Angled</code>.
	''' 
	''' @author Tom Santos
	''' @author Steve Wilson (value add stuff)
	''' </summary>
	Public Class MetalTreeUI
		Inherits BasicTreeUI

		Private Shared lineColor As Color

		Private Const LINE_STYLE As String = "JTree.lineStyle"

		Private Const LEG_LINE_STYLE_STRING As String = "Angled"
		Private Const HORIZ_STYLE_STRING As String = "Horizontal"
		Private Const NO_STYLE_STRING As String = "None"

		Private Const LEG_LINE_STYLE As Integer = 2
		Private Const HORIZ_LINE_STYLE As Integer = 1
		Private Const NO_LINE_STYLE As Integer = 0

		Private lineStyle As Integer = LEG_LINE_STYLE
		Private lineStyleListener As PropertyChangeListener = New LineListener(Me)

		' Boilerplate
		Public Shared Function createUI(ByVal x As JComponent) As ComponentUI
			Return New MetalTreeUI
		End Function

		Public Sub New()
			MyBase.New()
		End Sub

		Protected Friend Property Overrides horizontalLegBuffer As Integer
			Get
				  Return 3
			End Get
		End Property

		Public Overrides Sub installUI(ByVal c As JComponent)
			MyBase.installUI(c)
			lineColor = UIManager.getColor("Tree.line")

			Dim lineStyleFlag As Object = c.getClientProperty(LINE_STYLE)
			decodeLineStyle(lineStyleFlag)
			c.addPropertyChangeListener(lineStyleListener)

		End Sub

		Public Overrides Sub uninstallUI(ByVal c As JComponent)
			 c.removePropertyChangeListener(lineStyleListener)
			 MyBase.uninstallUI(c)
		End Sub

		''' <summary>
		''' this function converts between the string passed into the client property
		''' and the internal representation (currently and int)
		'''  
		''' </summary>
		Protected Friend Overridable Sub decodeLineStyle(ByVal lineStyleFlag As Object)
		  If lineStyleFlag Is Nothing OrElse lineStyleFlag.Equals(LEG_LINE_STYLE_STRING) Then
			lineStyle = LEG_LINE_STYLE ' default case
		  Else
			  If lineStyleFlag.Equals(NO_STYLE_STRING) Then
				  lineStyle = NO_LINE_STYLE
			  ElseIf lineStyleFlag.Equals(HORIZ_STYLE_STRING) Then
				  lineStyle = HORIZ_LINE_STYLE
			  End If
		  End If

		End Sub

		Protected Friend Overridable Function isLocationInExpandControl(ByVal row As Integer, ByVal rowLevel As Integer, ByVal mouseX As Integer, ByVal mouseY As Integer) As Boolean
			If tree IsNot Nothing AndAlso (Not isLeaf(row)) Then
				Dim boxWidth As Integer

				If expandedIcon IsNot Nothing Then
					boxWidth = expandedIcon.iconWidth + 6
				Else
					boxWidth = 8
				End If

				Dim i As Insets = tree.insets
				Dim boxLeftX As Integer = If(i IsNot Nothing, i.left, 0)


				boxLeftX += (((rowLevel + depthOffset - 1) * totalChildIndent) + leftChildIndent) - boxWidth\2

				Dim boxRightX As Integer = boxLeftX + boxWidth

				Return mouseX >= boxLeftX AndAlso mouseX <= boxRightX
			End If
			Return False
		End Function

		Public Overrides Sub paint(ByVal g As Graphics, ByVal c As JComponent)
			MyBase.paint(g, c)


			' Paint the lines
			If lineStyle = HORIZ_LINE_STYLE AndAlso (Not largeModel) Then paintHorizontalSeparators(g,c)
		End Sub

		Protected Friend Overridable Sub paintHorizontalSeparators(ByVal g As Graphics, ByVal c As JComponent)
			g.color = lineColor

			Dim clipBounds As Rectangle = g.clipBounds

			Dim beginRow As Integer = getRowForPath(tree, getClosestPathForLocation(tree, 0, clipBounds.y))
			Dim endRow As Integer = getRowForPath(tree, getClosestPathForLocation(tree, 0, clipBounds.y + clipBounds.height - 1))

			If beginRow <= -1 OrElse endRow <= -1 Then Return

			For i As Integer = beginRow To endRow
				Dim path As TreePath = getPathForRow(tree, i)

				If path IsNot Nothing AndAlso path.pathCount = 2 Then
					Dim rowBounds As Rectangle = getPathBounds(tree,getPathForRow(tree, i))

					' Draw a line at the top
					If rowBounds IsNot Nothing Then g.drawLine(clipBounds.x, rowBounds.y, clipBounds.x + clipBounds.width, rowBounds.y)
				End If
			Next i

		End Sub

		Protected Friend Overrides Sub paintVerticalPartOfLeg(ByVal g As Graphics, ByVal clipBounds As Rectangle, ByVal insets As Insets, ByVal path As TreePath)
			If lineStyle = LEG_LINE_STYLE Then MyBase.paintVerticalPartOfLeg(g, clipBounds, insets, path)
		End Sub

		Protected Friend Overrides Sub paintHorizontalPartOfLeg(ByVal g As Graphics, ByVal clipBounds As Rectangle, ByVal insets As Insets, ByVal bounds As Rectangle, ByVal path As TreePath, ByVal row As Integer, ByVal isExpanded As Boolean, ByVal hasBeenExpanded As Boolean, ByVal isLeaf As Boolean)
			If lineStyle = LEG_LINE_STYLE Then MyBase.paintHorizontalPartOfLeg(g, clipBounds, insets, bounds, path, row, isExpanded, hasBeenExpanded, isLeaf)
		End Sub

		''' <summary>
		''' This class listens for changes in line style </summary>
		Friend Class LineListener
			Implements PropertyChangeListener

			Private ReadOnly outerInstance As MetalTreeUI

			Public Sub New(ByVal outerInstance As MetalTreeUI)
				Me.outerInstance = outerInstance
			End Sub

			Public Overridable Sub propertyChange(ByVal e As PropertyChangeEvent)
				Dim name As String = e.propertyName
				If name.Equals(LINE_STYLE) Then outerInstance.decodeLineStyle(e.newValue)
			End Sub
		End Class ' end class PaletteListener

	End Class

End Namespace