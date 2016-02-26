Imports System
Imports System.Collections
Imports System.Collections.Generic
Imports javax.swing
Imports javax.swing.border
Imports javax.swing.event

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

Namespace javax.swing.tree


	''' <summary>
	''' A <code>TreeCellEditor</code>. You need to supply an
	''' instance of <code>DefaultTreeCellRenderer</code>
	''' so that the icons can be obtained. You can optionally supply
	''' a <code>TreeCellEditor</code> that will be layed out according
	''' to the icon in the <code>DefaultTreeCellRenderer</code>.
	''' If you do not supply a <code>TreeCellEditor</code>,
	''' a <code>TextField</code> will be used. Editing is started
	''' on a triple mouse click, or after a click, pause, click and
	''' a delay of 1200 milliseconds.
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
	''' <seealso cref= javax.swing.JTree
	''' 
	''' @author Scott Violet </seealso>
	Public Class DefaultTreeCellEditor
		Implements ActionListener, TreeCellEditor, TreeSelectionListener

		''' <summary>
		''' Editor handling the editing. </summary>
		Protected Friend realEditor As TreeCellEditor

		''' <summary>
		''' Renderer, used to get border and offsets from. </summary>
		Protected Friend renderer As DefaultTreeCellRenderer

		''' <summary>
		''' Editing container, will contain the <code>editorComponent</code>. </summary>
		Protected Friend editingContainer As Container

		''' <summary>
		''' Component used in editing, obtained from the
		''' <code>editingContainer</code>.
		''' </summary>
		<NonSerialized> _
		Protected Friend editingComponent As Component

		''' <summary>
		''' As of Java 2 platform v1.4 this field should no longer be used. If
		''' you wish to provide similar behavior you should directly override
		''' <code>isCellEditable</code>.
		''' </summary>
		Protected Friend canEdit As Boolean

		''' <summary>
		''' Used in editing. Indicates x position to place
		''' <code>editingComponent</code>.
		''' </summary>
		<NonSerialized> _
		Protected Friend offset As Integer

		''' <summary>
		''' <code>JTree</code> instance listening too. </summary>
		<NonSerialized> _
		Protected Friend tree As JTree

		''' <summary>
		''' Last path that was selected. </summary>
		<NonSerialized> _
		Protected Friend lastPath As TreePath

		''' <summary>
		''' Used before starting the editing session. </summary>
		<NonSerialized> _
		Protected Friend ___timer As Timer

		''' <summary>
		''' Row that was last passed into
		''' <code>getTreeCellEditorComponent</code>.
		''' </summary>
		<NonSerialized> _
		Protected Friend lastRow As Integer

		''' <summary>
		''' True if the border selection color should be drawn. </summary>
		Protected Friend borderSelectionColor As Color

		''' <summary>
		''' Icon to use when editing. </summary>
		<NonSerialized> _
		Protected Friend editingIcon As Icon

		''' <summary>
		''' Font to paint with, <code>null</code> indicates
		''' font of renderer is to be used.
		''' </summary>
		Protected Friend font As Font


		''' <summary>
		''' Constructs a <code>DefaultTreeCellEditor</code>
		''' object for a JTree using the specified renderer and
		''' a default editor. (Use this constructor for normal editing.)
		''' </summary>
		''' <param name="tree">      a <code>JTree</code> object </param>
		''' <param name="renderer">  a <code>DefaultTreeCellRenderer</code> object </param>
		Public Sub New(ByVal tree As JTree, ByVal renderer As DefaultTreeCellRenderer)
			Me.New(tree, renderer, Nothing)
		End Sub

		''' <summary>
		''' Constructs a <code>DefaultTreeCellEditor</code>
		''' object for a <code>JTree</code> using the
		''' specified renderer and the specified editor. (Use this constructor
		''' for specialized editing.)
		''' </summary>
		''' <param name="tree">      a <code>JTree</code> object </param>
		''' <param name="renderer">  a <code>DefaultTreeCellRenderer</code> object </param>
		''' <param name="editor">    a <code>TreeCellEditor</code> object </param>
		Public Sub New(ByVal tree As JTree, ByVal renderer As DefaultTreeCellRenderer, ByVal editor As TreeCellEditor)
			Me.renderer = renderer
			realEditor = editor
			If realEditor Is Nothing Then realEditor = createTreeCellEditor()
			editingContainer = createContainer()
			tree = tree
			borderSelectionColor = UIManager.getColor("Tree.editorBorderSelectionColor")
		End Sub

		''' <summary>
		''' Sets the color to use for the border. </summary>
		''' <param name="newColor"> the new border color </param>
		Public Overridable Property borderSelectionColor As Color
			Set(ByVal newColor As Color)
				borderSelectionColor = newColor
			End Set
			Get
				Return borderSelectionColor
			End Get
		End Property


		''' <summary>
		''' Sets the font to edit with. <code>null</code> indicates
		''' the renderers font should be used. This will NOT
		''' override any font you have set in the editor
		''' the receiver was instantiated with. If <code>null</code>
		''' for an editor was passed in a default editor will be
		''' created that will pick up this font.
		''' </summary>
		''' <param name="font">  the editing <code>Font</code> </param>
		''' <seealso cref= #getFont </seealso>
		Public Overridable Property font As Font
			Set(ByVal font As Font)
				Me.font = font
			End Set
			Get
				Return font
			End Get
		End Property


		'
		' TreeCellEditor
		'

		''' <summary>
		''' Configures the editor.  Passed onto the <code>realEditor</code>.
		''' </summary>
		Public Overridable Function getTreeCellEditorComponent(ByVal tree As JTree, ByVal value As Object, ByVal isSelected As Boolean, ByVal expanded As Boolean, ByVal leaf As Boolean, ByVal row As Integer) As Component
			tree = tree
			lastRow = row
			determineOffset(tree, value, isSelected, expanded, leaf, row)

			If editingComponent IsNot Nothing Then editingContainer.remove(editingComponent)
			editingComponent = realEditor.getTreeCellEditorComponent(tree, value, isSelected, expanded,leaf, row)


			' this is kept for backwards compatibility but isn't really needed
			' with the current BasicTreeUI implementation.
			Dim newPath As TreePath = tree.getPathForRow(row)

			canEdit = (lastPath IsNot Nothing AndAlso newPath IsNot Nothing AndAlso lastPath.Equals(newPath))

			Dim ___font As Font = font

			If ___font Is Nothing Then
				If renderer IsNot Nothing Then ___font = renderer.font
				If ___font Is Nothing Then ___font = tree.font
			End If
			editingContainer.font = ___font
			prepareForEditing()
			Return editingContainer
		End Function

		''' <summary>
		''' Returns the value currently being edited. </summary>
		''' <returns> the value currently being edited </returns>
		Public Overridable Property cellEditorValue As Object Implements CellEditor.getCellEditorValue
			Get
				Return realEditor.cellEditorValue
			End Get
		End Property

		''' <summary>
		''' If the <code>realEditor</code> returns true to this
		''' message, <code>prepareForEditing</code>
		''' is messaged and true is returned.
		''' </summary>
		Public Overridable Function isCellEditable(ByVal [event] As java.util.EventObject) As Boolean Implements CellEditor.isCellEditable
			Dim retValue As Boolean = False
			Dim editable As Boolean = False

			If [event] IsNot Nothing Then
				If TypeOf [event].source Is JTree Then
					tree = CType([event].source, JTree)
					If TypeOf [event] Is MouseEvent Then
						Dim path As TreePath = tree.getPathForLocation(CType([event], MouseEvent).x, CType([event], MouseEvent).y)
						editable = (lastPath IsNot Nothing AndAlso path IsNot Nothing AndAlso lastPath.Equals(path))
						If path IsNot Nothing Then
							lastRow = tree.getRowForPath(path)
							Dim value As Object = path.lastPathComponent
							Dim isSelected As Boolean = tree.isRowSelected(lastRow)
							Dim expanded As Boolean = tree.isExpanded(path)
							Dim treeModel As TreeModel = tree.model
							Dim leaf As Boolean = treeModel.isLeaf(value)
							determineOffset(tree, value, isSelected, expanded, leaf, lastRow)
						End If
					End If
				End If
			End If
			If Not realEditor.isCellEditable([event]) Then Return False
			If canEditImmediately([event]) Then
				retValue = True
			ElseIf editable AndAlso shouldStartEditingTimer([event]) Then
				startEditingTimer()
			ElseIf ___timer IsNot Nothing AndAlso ___timer.running Then
				___timer.stop()
			End If
			If retValue Then prepareForEditing()
			Return retValue
		End Function

		''' <summary>
		''' Messages the <code>realEditor</code> for the return value.
		''' </summary>
		Public Overridable Function shouldSelectCell(ByVal [event] As java.util.EventObject) As Boolean Implements CellEditor.shouldSelectCell
			Return realEditor.shouldSelectCell([event])
		End Function

		''' <summary>
		''' If the <code>realEditor</code> will allow editing to stop,
		''' the <code>realEditor</code> is removed and true is returned,
		''' otherwise false is returned.
		''' </summary>
		Public Overridable Function stopCellEditing() As Boolean Implements CellEditor.stopCellEditing
			If realEditor.stopCellEditing() Then
				cleanupAfterEditing()
				Return True
			End If
			Return False
		End Function

		''' <summary>
		''' Messages <code>cancelCellEditing</code> to the
		''' <code>realEditor</code> and removes it from this instance.
		''' </summary>
		Public Overridable Sub cancelCellEditing() Implements CellEditor.cancelCellEditing
			realEditor.cancelCellEditing()
			cleanupAfterEditing()
		End Sub

		''' <summary>
		''' Adds the <code>CellEditorListener</code>. </summary>
		''' <param name="l"> the listener to be added </param>
		Public Overridable Sub addCellEditorListener(ByVal l As CellEditorListener) Implements CellEditor.addCellEditorListener
			realEditor.addCellEditorListener(l)
		End Sub

		''' <summary>
		''' Removes the previously added <code>CellEditorListener</code>. </summary>
		''' <param name="l"> the listener to be removed </param>
		Public Overridable Sub removeCellEditorListener(ByVal l As CellEditorListener) Implements CellEditor.removeCellEditorListener
			realEditor.removeCellEditorListener(l)
		End Sub

		''' <summary>
		''' Returns an array of all the <code>CellEditorListener</code>s added
		''' to this DefaultTreeCellEditor with addCellEditorListener().
		''' </summary>
		''' <returns> all of the <code>CellEditorListener</code>s added or an empty
		'''         array if no listeners have been added
		''' @since 1.4 </returns>
		Public Overridable Property cellEditorListeners As CellEditorListener()
			Get
				Return CType(realEditor, DefaultCellEditor).cellEditorListeners
			End Get
		End Property

		'
		' TreeSelectionListener
		'

		''' <summary>
		''' Resets <code>lastPath</code>.
		''' </summary>
		Public Overridable Sub valueChanged(ByVal e As TreeSelectionEvent) Implements TreeSelectionListener.valueChanged
			If tree IsNot Nothing Then
				If tree.selectionCount = 1 Then
					lastPath = tree.selectionPath
				Else
					lastPath = Nothing
				End If
			End If
			If ___timer IsNot Nothing Then ___timer.stop()
		End Sub

		'
		' ActionListener (for Timer).
		'

		''' <summary>
		''' Messaged when the timer fires, this will start the editing
		''' session.
		''' </summary>
		Public Overridable Sub actionPerformed(ByVal e As ActionEvent)
			If tree IsNot Nothing AndAlso lastPath IsNot Nothing Then tree.startEditingAtPath(lastPath)
		End Sub

		'
		' Local methods
		'

		''' <summary>
		''' Sets the tree currently editing for. This is needed to add
		''' a selection listener. </summary>
		''' <param name="newTree"> the new tree to be edited </param>
		Protected Friend Overridable Property tree As JTree
			Set(ByVal newTree As JTree)
				If tree IsNot newTree Then
					If tree IsNot Nothing Then tree.removeTreeSelectionListener(Me)
					tree = newTree
					If tree IsNot Nothing Then tree.addTreeSelectionListener(Me)
					If ___timer IsNot Nothing Then ___timer.stop()
				End If
			End Set
		End Property

		''' <summary>
		''' Returns true if <code>event</code> is a <code>MouseEvent</code>
		''' and the click count is 1. </summary>
		''' <param name="event">  the event being studied </param>
		Protected Friend Overridable Function shouldStartEditingTimer(ByVal [event] As java.util.EventObject) As Boolean
			If (TypeOf [event] Is MouseEvent) AndAlso SwingUtilities.isLeftMouseButton(CType([event], MouseEvent)) Then
				Dim [me] As MouseEvent = CType([event], MouseEvent)

				Return ([me].clickCount = 1 AndAlso inHitRegion([me].x, [me].y))
			End If
			Return False
		End Function

		''' <summary>
		''' Starts the editing timer.
		''' </summary>
		Protected Friend Overridable Sub startEditingTimer()
			If ___timer Is Nothing Then
				___timer = New Timer(1200, Me)
				___timer.repeats = False
			End If
			___timer.start()
		End Sub

		''' <summary>
		''' Returns true if <code>event</code> is <code>null</code>,
		''' or it is a <code>MouseEvent</code> with a click count &gt; 2
		''' and <code>inHitRegion</code> returns true. </summary>
		''' <param name="event"> the event being studied </param>
		Protected Friend Overridable Function canEditImmediately(ByVal [event] As java.util.EventObject) As Boolean
			If (TypeOf [event] Is MouseEvent) AndAlso SwingUtilities.isLeftMouseButton(CType([event], MouseEvent)) Then
				Dim [me] As MouseEvent = CType([event], MouseEvent)

				Return (([me].clickCount > 2) AndAlso inHitRegion([me].x, [me].y))
			End If
			Return ([event] Is Nothing)
		End Function

		''' <summary>
		''' Returns true if the passed in location is a valid mouse location
		''' to start editing from. This is implemented to return false if
		''' <code>x</code> is &lt;= the width of the icon and icon gap displayed
		''' by the renderer. In other words this returns true if the user
		''' clicks over the text part displayed by the renderer, and false
		''' otherwise. </summary>
		''' <param name="x"> the x-coordinate of the point </param>
		''' <param name="y"> the y-coordinate of the point </param>
		''' <returns> true if the passed in location is a valid mouse location </returns>
		Protected Friend Overridable Function inHitRegion(ByVal x As Integer, ByVal y As Integer) As Boolean
			If lastRow <> -1 AndAlso tree IsNot Nothing Then
				Dim bounds As Rectangle = tree.getRowBounds(lastRow)
				Dim treeOrientation As ComponentOrientation = tree.componentOrientation

				If treeOrientation.leftToRight Then
					If bounds IsNot Nothing AndAlso x <= (bounds.x + offset) AndAlso offset < (bounds.width - 5) Then Return False
				ElseIf bounds IsNot Nothing AndAlso (x >= (bounds.x+bounds.width-offset+5) OrElse x <= (bounds.x + 5)) AndAlso offset < (bounds.width - 5) Then
					Return False
				End If
			End If
			Return True
		End Function

		Protected Friend Overridable Sub determineOffset(ByVal tree As JTree, ByVal value As Object, ByVal isSelected As Boolean, ByVal expanded As Boolean, ByVal leaf As Boolean, ByVal row As Integer)
			If renderer IsNot Nothing Then
				If leaf Then
					editingIcon = renderer.leafIcon
				ElseIf expanded Then
					editingIcon = renderer.openIcon
				Else
					editingIcon = renderer.closedIcon
				End If
				If editingIcon IsNot Nothing Then
					offset = renderer.iconTextGap + editingIcon.iconWidth
				Else
					offset = renderer.iconTextGap
				End If
			Else
				editingIcon = Nothing
				offset = 0
			End If
		End Sub

		''' <summary>
		''' Invoked just before editing is to start. Will add the
		''' <code>editingComponent</code> to the
		''' <code>editingContainer</code>.
		''' </summary>
		Protected Friend Overridable Sub prepareForEditing()
			If editingComponent IsNot Nothing Then editingContainer.add(editingComponent)
		End Sub

		''' <summary>
		''' Creates the container to manage placement of
		''' <code>editingComponent</code>.
		''' </summary>
		Protected Friend Overridable Function createContainer() As Container
			Return New EditorContainer(Me)
		End Function

		''' <summary>
		''' This is invoked if a <code>TreeCellEditor</code>
		''' is not supplied in the constructor.
		''' It returns a <code>TextField</code> editor. </summary>
		''' <returns> a new <code>TextField</code> editor </returns>
		Protected Friend Overridable Function createTreeCellEditor() As TreeCellEditor
			Dim aBorder As Border = UIManager.getBorder("Tree.editorBorder")
			Dim editor As DefaultCellEditor = New DefaultCellEditorAnonymousInnerClassHelper

			' One click to edit.
			editor.clickCountToStart = 1
			Return editor
		End Function

		Private Class DefaultCellEditorAnonymousInnerClassHelper
			Inherits DefaultCellEditor

			Public Overrides Function shouldSelectCell(ByVal [event] As java.util.EventObject) As Boolean
				Dim retValue As Boolean = MyBase.shouldSelectCell([event])
				Return retValue
			End Function
		End Class

		''' <summary>
		''' Cleans up any state after editing has completed. Removes the
		''' <code>editingComponent</code> the <code>editingContainer</code>.
		''' </summary>
		Private Sub cleanupAfterEditing()
			If editingComponent IsNot Nothing Then editingContainer.remove(editingComponent)
			editingComponent = Nothing
		End Sub

		' Serialization support.
		Private Sub writeObject(ByVal s As ObjectOutputStream)
			Dim values As New List(Of Object)

			s.defaultWriteObject()
			' Save the realEditor, if its Serializable.
			If realEditor IsNot Nothing AndAlso TypeOf realEditor Is Serializable Then
				values.Add("realEditor")
				values.Add(realEditor)
			End If
			s.writeObject(values)
		End Sub

		Private Sub readObject(ByVal s As ObjectInputStream)
			s.defaultReadObject()

			Dim values As ArrayList = CType(s.readObject(), ArrayList)
			Dim indexCounter As Integer = 0
			Dim maxCounter As Integer = values.Count

			If indexCounter < maxCounter AndAlso values(indexCounter).Equals("realEditor") Then
				indexCounter += 1
				realEditor = CType(values(indexCounter), TreeCellEditor)
				indexCounter += 1
			End If
		End Sub


		''' <summary>
		''' <code>TextField</code> used when no editor is supplied.
		''' This textfield locks into the border it is constructed with.
		''' It also prefers its parents font over its font. And if the
		''' renderer is not <code>null</code> and no font
		''' has been specified the preferred height is that of the renderer.
		''' </summary>
		Public Class DefaultTextField
			Inherits JTextField

			Private ReadOnly outerInstance As DefaultTreeCellEditor

			''' <summary>
			''' Border to use. </summary>
			Protected Friend border As Border

			''' <summary>
			''' Constructs a
			''' <code>DefaultTreeCellEditor.DefaultTextField</code> object.
			''' </summary>
			''' <param name="border">  a <code>Border</code> object
			''' @since 1.4 </param>
			Public Sub New(ByVal outerInstance As DefaultTreeCellEditor, ByVal border As Border)
					Me.outerInstance = outerInstance
				border = border
			End Sub

			''' <summary>
			''' Sets the border of this component.<p>
			''' This is a bound property.
			''' </summary>
			''' <param name="border"> the border to be rendered for this component </param>
			''' <seealso cref= Border </seealso>
			''' <seealso cref= CompoundBorder
			''' @beaninfo
			'''        bound: true
			'''    preferred: true
			'''    attribute: visualUpdate true
			'''  description: The component's border. </seealso>
			Public Overrides Property border As Border
				Set(ByVal border As Border)
					MyBase.border = border
					Me.border = border
				End Set
				Get
					Return border
				End Get
			End Property


			' implements java.awt.MenuContainer
			Public Overridable Property font As Font
				Get
					Dim ___font As Font = MyBase.font
    
					' Prefer the parent containers font if our font is a
					' FontUIResource
					If TypeOf ___font Is javax.swing.plaf.FontUIResource Then
						Dim parent As Container = parent
    
						If parent IsNot Nothing AndAlso parent.font IsNot Nothing Then ___font = parent.font
					End If
					Return ___font
				End Get
			End Property

			''' <summary>
			''' Overrides <code>JTextField.getPreferredSize</code> to
			''' return the preferred size based on current font, if set,
			''' or else use renderer's font. </summary>
			''' <returns> a <code>Dimension</code> object containing
			'''   the preferred size </returns>
			Public Property Overrides preferredSize As Dimension
				Get
					Dim ___size As Dimension = MyBase.preferredSize
    
					' If not font has been set, prefer the renderers height.
					If outerInstance.renderer IsNot Nothing AndAlso outerInstance.font Is Nothing Then
						Dim rSize As Dimension = outerInstance.renderer.preferredSize
    
						___size.height = rSize.height
					End If
					Return ___size
				End Get
			End Property
		End Class


		''' <summary>
		''' Container responsible for placing the <code>editingComponent</code>.
		''' </summary>
		Public Class EditorContainer
			Inherits Container

			Private ReadOnly outerInstance As DefaultTreeCellEditor

			''' <summary>
			''' Constructs an <code>EditorContainer</code> object.
			''' </summary>
			Public Sub New(ByVal outerInstance As DefaultTreeCellEditor)
					Me.outerInstance = outerInstance
				layout = Nothing
			End Sub

			' This should not be used. It will be removed when new API is
			' allowed.
			Public void Sub New(ByVal outerInstance As DefaultTreeCellEditor)
					Me.outerInstance = outerInstance
				layout = Nothing
			End Sub

			''' <summary>
			''' Overrides <code>Container.paint</code> to paint the node's
			''' icon and use the selection color for the background.
			''' </summary>
			Public Overridable Sub paint(ByVal g As Graphics)
				Dim width As Integer = width
				Dim height As Integer = height

				' Then the icon.
				If outerInstance.editingIcon IsNot Nothing Then
					Dim yLoc As Integer = calculateIconY(outerInstance.editingIcon)

					If componentOrientation.leftToRight Then
						outerInstance.editingIcon.paintIcon(Me, g, 0, yLoc)
					Else
						outerInstance.editingIcon.paintIcon(Me, g, width - outerInstance.editingIcon.iconWidth, yLoc)
					End If
				End If

				' Border selection color
				Dim background As Color = outerInstance.borderSelectionColor
				If background IsNot Nothing Then
					g.color = background
					g.drawRect(0, 0, width - 1, height - 1)
				End If
				MyBase.paint(g)
			End Sub

			''' <summary>
			''' Lays out this <code>Container</code>.  If editing,
			''' the editor will be placed at
			''' <code>offset</code> in the x direction and 0 for y.
			''' </summary>
			Public Overridable Sub doLayout()
				If outerInstance.editingComponent IsNot Nothing Then
					Dim width As Integer = width
					Dim height As Integer = height
					If componentOrientation.leftToRight Then
						outerInstance.editingComponent.boundsnds(outerInstance.offset, 0, width - outerInstance.offset, height)
					Else
						outerInstance.editingComponent.boundsnds(0, 0, width - outerInstance.offset, height)
					End If
				End If
			End Sub

			''' <summary>
			''' Calculate the y location for the icon.
			''' </summary>
			Private Function calculateIconY(ByVal icon As Icon) As Integer
				' To make sure the icon position matches that of the
				' renderer, use the same algorithm as JLabel
				' (SwingUtilities.layoutCompoundLabel).
				Dim iconHeight As Integer = icon.iconHeight
				Dim textHeight As Integer = outerInstance.editingComponent.getFontMetrics(outerInstance.editingComponent.font).height
				Dim textY As Integer = iconHeight \ 2 - textHeight \ 2
				Dim totalY As Integer = Math.Min(0, textY)
				Dim totalHeight As Integer = Math.Max(iconHeight, textY + textHeight) - totalY
				Return height / 2 - (totalY + (totalHeight \ 2))
			End Function

			''' <summary>
			''' Returns the preferred size for the <code>Container</code>.
			''' This will be at least preferred size of the editor plus
			''' <code>offset</code>. </summary>
			''' <returns> a <code>Dimension</code> containing the preferred
			'''   size for the <code>Container</code>; if
			'''   <code>editingComponent</code> is <code>null</code> the
			'''   <code>Dimension</code> returned is 0, 0 </returns>
			Public Overridable Property preferredSize As Dimension
				Get
					If outerInstance.editingComponent IsNot Nothing Then
						Dim pSize As Dimension = outerInstance.editingComponent.preferredSize
    
						pSize.width += outerInstance.offset + 5
    
						Dim rSize As Dimension = If(outerInstance.renderer IsNot Nothing, outerInstance.renderer.preferredSize, Nothing)
    
						If rSize IsNot Nothing Then pSize.height = Math.Max(pSize.height, rSize.height)
						If outerInstance.editingIcon IsNot Nothing Then pSize.height = Math.Max(pSize.height, outerInstance.editingIcon.iconHeight)
    
						' Make sure width is at least 100.
						pSize.width = Math.Max(pSize.width, 100)
						Return pSize
					End If
					Return New Dimension(0, 0)
				End Get
			End Property
		End Class
	End Class

End Namespace