Imports System
Imports javax.swing
Imports javax.swing.event
Imports javax.swing.plaf

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



Namespace javax.swing.plaf.basic






	''' <summary>
	''' Divider used by BasicSplitPaneUI. Subclassers may wish to override
	''' paint to do something more interesting.
	''' The border effect is drawn in BasicSplitPaneUI, so if you don't like
	''' that border, reset it there.
	''' To conditionally drag from certain areas subclass mousePressed and
	''' call super when you wish the dragging to begin.
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
	''' @author Scott Violet
	''' </summary>
	Public Class BasicSplitPaneDivider
		Inherits Container
		Implements PropertyChangeListener

		''' <summary>
		''' Width or height of the divider based on orientation
		''' BasicSplitPaneUI adds two to this.
		''' </summary>
		Protected Friend Const ONE_TOUCH_SIZE As Integer = 6
		Protected Friend Const ONE_TOUCH_OFFSET As Integer = 2

		''' <summary>
		''' Handles mouse dragging message to do the actual dragging.
		''' </summary>
		Protected Friend dragger As DragController

		''' <summary>
		''' UI this instance was created from.
		''' </summary>
		Protected Friend splitPaneUI As BasicSplitPaneUI

		''' <summary>
		''' Size of the divider.
		''' </summary>
		Protected Friend dividerSize As Integer = 0 ' default - SET TO 0???

		''' <summary>
		''' Divider that is used for noncontinuous layout mode.
		''' </summary>
		Protected Friend hiddenDivider As Component

		''' <summary>
		''' JSplitPane the receiver is contained in.
		''' </summary>
		Protected Friend splitPane As JSplitPane

		''' <summary>
		''' Handles mouse events from both this class, and the split pane.
		''' Mouse events are handled for the splitpane since you want to be able
		''' to drag when clicking on the border of the divider, which is not
		''' drawn by the divider.
		''' </summary>
		Protected Friend mouseHandler As MouseHandler

		''' <summary>
		''' Orientation of the JSplitPane.
		''' </summary>
		Protected Friend orientation As Integer

		''' <summary>
		''' Button for quickly toggling the left component.
		''' </summary>
		Protected Friend leftButton As JButton

		''' <summary>
		''' Button for quickly toggling the right component.
		''' </summary>
		Protected Friend rightButton As JButton

		''' <summary>
		''' Border. </summary>
		Private border As javax.swing.border.Border

		''' <summary>
		''' Is the mouse over the divider?
		''' </summary>
		Private mouseOver As Boolean

		Private oneTouchSize As Integer
		Private oneTouchOffset As Integer

		''' <summary>
		''' If true the one touch buttons are centered on the divider.
		''' </summary>
		Private centerOneTouchButtons As Boolean


		''' <summary>
		''' Creates an instance of BasicSplitPaneDivider. Registers this
		''' instance for mouse events and mouse dragged events.
		''' </summary>
		Public Sub New(ByVal ui As BasicSplitPaneUI)
			oneTouchSize = sun.swing.DefaultLookup.getInt(ui.splitPane, ui, "SplitPane.oneTouchButtonSize", ONE_TOUCH_SIZE)
			oneTouchOffset = sun.swing.DefaultLookup.getInt(ui.splitPane, ui, "SplitPane.oneTouchButtonOffset", ONE_TOUCH_OFFSET)
			centerOneTouchButtons = sun.swing.DefaultLookup.getBoolean(ui.splitPane, ui, "SplitPane.centerOneTouchButtons", True)
			layout = New DividerLayout(Me)
			basicSplitPaneUI = ui
			orientation = splitPane.orientation
			cursor = If(orientation = JSplitPane.HORIZONTAL_SPLIT, Cursor.getPredefinedCursor(Cursor.E_RESIZE_CURSOR), Cursor.getPredefinedCursor(Cursor.S_RESIZE_CURSOR))
			background = UIManager.getColor("SplitPane.background")
		End Sub

		Private Sub revalidateSplitPane()
			invalidate()
			If splitPane IsNot Nothing Then splitPane.revalidate()
		End Sub

		''' <summary>
		''' Sets the SplitPaneUI that is using the receiver.
		''' </summary>
		Public Overridable Property basicSplitPaneUI As BasicSplitPaneUI
			Set(ByVal newUI As BasicSplitPaneUI)
				If splitPane IsNot Nothing Then
					splitPane.removePropertyChangeListener(Me)
				   If mouseHandler IsNot Nothing Then
					   splitPane.removeMouseListener(mouseHandler)
					   splitPane.removeMouseMotionListener(mouseHandler)
					   removeMouseListener(mouseHandler)
					   removeMouseMotionListener(mouseHandler)
					   mouseHandler = Nothing
				   End If
				End If
				splitPaneUI = newUI
				If newUI IsNot Nothing Then
					splitPane = newUI.splitPane
					If splitPane IsNot Nothing Then
						If mouseHandler Is Nothing Then mouseHandler = New MouseHandler(Me)
						splitPane.addMouseListener(mouseHandler)
						splitPane.addMouseMotionListener(mouseHandler)
						addMouseListener(mouseHandler)
						addMouseMotionListener(mouseHandler)
						splitPane.addPropertyChangeListener(Me)
						If splitPane.oneTouchExpandable Then oneTouchExpandableChanged()
					End If
				Else
					splitPane = Nothing
				End If
			End Set
			Get
				Return splitPaneUI
			End Get
		End Property




		''' <summary>
		''' Sets the size of the divider to <code>newSize</code>. That is
		''' the width if the splitpane is <code>HORIZONTAL_SPLIT</code>, or
		''' the height of <code>VERTICAL_SPLIT</code>.
		''' </summary>
		Public Overridable Property dividerSize As Integer
			Set(ByVal newSize As Integer)
				dividerSize = newSize
			End Set
			Get
				Return dividerSize
			End Get
		End Property




		''' <summary>
		''' Sets the border of this component.
		''' @since 1.3
		''' </summary>
		Public Overridable Property border As javax.swing.border.Border
			Set(ByVal border As javax.swing.border.Border)
				Dim oldBorder As javax.swing.border.Border = Me.border
    
				Me.border = border
			End Set
			Get
				Return border
			End Get
		End Property


		''' <summary>
		''' If a border has been set on this component, returns the
		''' border's insets, else calls super.getInsets.
		''' </summary>
		''' <returns> the value of the insets property. </returns>
		''' <seealso cref= #setBorder </seealso>
		Public Overridable Property insets As Insets
			Get
				Dim ___border As javax.swing.border.Border = border
    
				If ___border IsNot Nothing Then Return ___border.getBorderInsets(Me)
				Return MyBase.insets
			End Get
		End Property

		''' <summary>
		''' Sets whether or not the mouse is currently over the divider.
		''' </summary>
		''' <param name="mouseOver"> whether or not the mouse is currently over the divider
		''' @since 1.5 </param>
		Protected Friend Overridable Property mouseOver As Boolean
			Set(ByVal mouseOver As Boolean)
				Me.mouseOver = mouseOver
			End Set
			Get
				Return mouseOver
			End Get
		End Property


		''' <summary>
		''' Returns dividerSize x dividerSize
		''' </summary>
		Public Overridable Property preferredSize As Dimension
			Get
				' Ideally this would return the size from the layout manager,
				' but that could result in the layed out size being different from
				' the dividerSize, which may break developers as well as
				' BasicSplitPaneUI.
				If orientation = JSplitPane.HORIZONTAL_SPLIT Then Return New Dimension(dividerSize, 1)
				Return New Dimension(1, dividerSize)
			End Get
		End Property

		''' <summary>
		''' Returns dividerSize x dividerSize
		''' </summary>
		Public Overridable Property minimumSize As Dimension
			Get
				Return preferredSize
			End Get
		End Property


		''' <summary>
		''' Property change event, presumably from the JSplitPane, will message
		''' updateOrientation if necessary.
		''' </summary>
		Public Overridable Sub propertyChange(ByVal e As PropertyChangeEvent)
			If e.source Is splitPane Then
				If e.propertyName = JSplitPane.ORIENTATION_PROPERTY Then
					orientation = splitPane.orientation
					cursor = If(orientation = JSplitPane.HORIZONTAL_SPLIT, Cursor.getPredefinedCursor(Cursor.E_RESIZE_CURSOR), Cursor.getPredefinedCursor(Cursor.S_RESIZE_CURSOR))
					revalidateSplitPane()
				ElseIf e.propertyName = JSplitPane.ONE_TOUCH_EXPANDABLE_PROPERTY Then
					oneTouchExpandableChanged()
				End If
			End If
		End Sub


		''' <summary>
		''' Paints the divider.
		''' </summary>
		Public Overridable Sub paint(ByVal g As Graphics)
		  MyBase.paint(g)

		  ' Paint the border.
		  Dim ___border As javax.swing.border.Border = border

		  If ___border IsNot Nothing Then
			  Dim size As Dimension = size

			  ___border.paintBorder(Me, g, 0, 0, size.width, size.height)
		  End If
		End Sub


		''' <summary>
		''' Messaged when the oneTouchExpandable value of the JSplitPane the
		''' receiver is contained in changes. Will create the
		''' <code>leftButton</code> and <code>rightButton</code> if they
		''' are null. invalidates the receiver as well.
		''' </summary>
		Protected Friend Overridable Sub oneTouchExpandableChanged()
			If Not sun.swing.DefaultLookup.getBoolean(splitPane, splitPaneUI, "SplitPane.supportsOneTouchButtons", True) Then Return
			If splitPane.oneTouchExpandable AndAlso leftButton Is Nothing AndAlso rightButton Is Nothing Then
	'             Create the left button and add an action listener to
	'               expand/collapse it. 
				leftButton = createLeftOneTouchButton()
				If leftButton IsNot Nothing Then leftButton.addActionListener(New OneTouchActionHandler(Me, True))


	'             Create the right button and add an action listener to
	'               expand/collapse it. 
				rightButton = createRightOneTouchButton()
				If rightButton IsNot Nothing Then rightButton.addActionListener(New OneTouchActionHandler(Me, False))

				If leftButton IsNot Nothing AndAlso rightButton IsNot Nothing Then
					add(leftButton)
					add(rightButton)
				End If
			End If
			revalidateSplitPane()
		End Sub


		''' <summary>
		''' Creates and return an instance of JButton that can be used to
		''' collapse the left component in the split pane.
		''' </summary>
		Protected Friend Overridable Function createLeftOneTouchButton() As JButton
			Dim b As JButton = New JButtonAnonymousInnerClassHelper
			b.minimumSize = New Dimension(oneTouchSize, oneTouchSize)
			b.cursor = Cursor.getPredefinedCursor(Cursor.DEFAULT_CURSOR)
			b.focusPainted = False
			b.borderPainted = False
			b.requestFocusEnabled = False
			Return b
		End Function

		Private Class JButtonAnonymousInnerClassHelper
			Inherits JButton

			Public Overridable Property border As javax.swing.border.Border
				Set(ByVal b As javax.swing.border.Border)
				End Set
			End Property
			Public Overrides Sub paint(ByVal g As Graphics)
				If outerInstance.splitPane IsNot Nothing Then
					Dim xs As Integer() = New Integer(2){}
					Dim ys As Integer() = New Integer(2){}
					Dim blockSize As Integer

					' Fill the background first ...
					g.color = Me.background
					g.fillRect(0, 0, Me.width, Me.height)

					' ... then draw the arrow.
					g.color = Color.black
					If outerInstance.orientation = JSplitPane.VERTICAL_SPLIT Then
						blockSize = Math.Min(height, outerInstance.oneTouchSize)
						xs(0) = blockSize
						xs(1) = 0
						xs(2) = blockSize << 1
						ys(0) = 0
							ys(2) = blockSize
							ys(1) = ys(2)
						g.drawPolygon(xs, ys, 3) ' Little trick to make the
												  ' arrows of equal size
					Else
						blockSize = Math.Min(width, outerInstance.oneTouchSize)
							xs(2) = blockSize
							xs(0) = xs(2)
						xs(1) = 0
						ys(0) = 0
						ys(1) = blockSize
						ys(2) = blockSize << 1
					End If
					g.fillPolygon(xs, ys, 3)
				End If
			End Sub
			' Don't want the button to participate in focus traversable.
			Public Overridable Property focusTraversable As Boolean
				Get
					Return False
				End Get
			End Property
		End Class


		''' <summary>
		''' Creates and return an instance of JButton that can be used to
		''' collapse the right component in the split pane.
		''' </summary>
		Protected Friend Overridable Function createRightOneTouchButton() As JButton
			Dim b As JButton = New JButtonAnonymousInnerClassHelper2
			b.minimumSize = New Dimension(oneTouchSize, oneTouchSize)
			b.cursor = Cursor.getPredefinedCursor(Cursor.DEFAULT_CURSOR)
			b.focusPainted = False
			b.borderPainted = False
			b.requestFocusEnabled = False
			Return b
		End Function

		Private Class JButtonAnonymousInnerClassHelper2
			Inherits JButton

			Public Overridable Property border As javax.swing.border.Border
				Set(ByVal border As javax.swing.border.Border)
				End Set
			End Property
			Public Overrides Sub paint(ByVal g As Graphics)
				If outerInstance.splitPane IsNot Nothing Then
					Dim xs As Integer() = New Integer(2){}
					Dim ys As Integer() = New Integer(2){}
					Dim blockSize As Integer

					' Fill the background first ...
					g.color = Me.background
					g.fillRect(0, 0, Me.width, Me.height)

					' ... then draw the arrow.
					If outerInstance.orientation = JSplitPane.VERTICAL_SPLIT Then
						blockSize = Math.Min(height, outerInstance.oneTouchSize)
						xs(0) = blockSize
						xs(1) = blockSize << 1
						xs(2) = 0
						ys(0) = blockSize
							ys(2) = 0
							ys(1) = ys(2)
					Else
						blockSize = Math.Min(width, outerInstance.oneTouchSize)
							xs(2) = 0
							xs(0) = xs(2)
						xs(1) = blockSize
						ys(0) = 0
						ys(1) = blockSize
						ys(2) = blockSize << 1
					End If
					g.color = Color.black
					g.fillPolygon(xs, ys, 3)
				End If
			End Sub
			' Don't want the button to participate in focus traversable.
			Public Overridable Property focusTraversable As Boolean
				Get
					Return False
				End Get
			End Property
		End Class


		''' <summary>
		''' Message to prepare for dragging. This messages the BasicSplitPaneUI
		''' with startDragging.
		''' </summary>
		Protected Friend Overridable Sub prepareForDragging()
			splitPaneUI.startDragging()
		End Sub


		''' <summary>
		''' Messages the BasicSplitPaneUI with dragDividerTo that this instance
		''' is contained in.
		''' </summary>
		Protected Friend Overridable Sub dragDividerTo(ByVal location As Integer)
			splitPaneUI.dragDividerTo(location)
		End Sub


		''' <summary>
		''' Messages the BasicSplitPaneUI with finishDraggingTo that this instance
		''' is contained in.
		''' </summary>
		Protected Friend Overridable Sub finishDraggingTo(ByVal location As Integer)
			splitPaneUI.finishDraggingTo(location)
		End Sub


		''' <summary>
		''' MouseHandler is responsible for converting mouse events
		''' (released, dragged...) into the appropriate DragController
		''' methods.
		''' 
		''' </summary>
		Protected Friend Class MouseHandler
			Inherits MouseAdapter
			Implements MouseMotionListener

			Private ReadOnly outerInstance As BasicSplitPaneDivider

			Public Sub New(ByVal outerInstance As BasicSplitPaneDivider)
				Me.outerInstance = outerInstance
			End Sub

			''' <summary>
			''' Starts the dragging session by creating the appropriate instance
			''' of DragController.
			''' </summary>
			Public Overridable Sub mousePressed(ByVal e As MouseEvent)
				If (e.source = BasicSplitPaneDivider.this OrElse e.source Is outerInstance.splitPane) AndAlso outerInstance.dragger Is Nothing AndAlso outerInstance.splitPane.enabled Then
					Dim newHiddenDivider As Component = outerInstance.splitPaneUI.nonContinuousLayoutDivider

					If outerInstance.hiddenDivider IsNot newHiddenDivider Then
						If outerInstance.hiddenDivider IsNot Nothing Then
							outerInstance.hiddenDivider.removeMouseListener(Me)
							outerInstance.hiddenDivider.removeMouseMotionListener(Me)
						End If
						outerInstance.hiddenDivider = newHiddenDivider
						If outerInstance.hiddenDivider IsNot Nothing Then
							outerInstance.hiddenDivider.addMouseMotionListener(Me)
							outerInstance.hiddenDivider.addMouseListener(Me)
						End If
					End If
					If outerInstance.splitPane.leftComponent IsNot Nothing AndAlso outerInstance.splitPane.rightComponent IsNot Nothing Then
						If outerInstance.orientation = JSplitPane.HORIZONTAL_SPLIT Then
							outerInstance.dragger = New DragController(e)
						Else
							outerInstance.dragger = New VerticalDragController(e)
						End If
						If Not outerInstance.dragger.valid Then
							outerInstance.dragger = Nothing
						Else
							outerInstance.prepareForDragging()
							outerInstance.dragger.continueDrag(e)
						End If
					End If
					e.consume()
				End If
			End Sub


			''' <summary>
			''' If dragger is not null it is messaged with completeDrag.
			''' </summary>
			Public Overridable Sub mouseReleased(ByVal e As MouseEvent)
				If outerInstance.dragger IsNot Nothing Then
					If e.source Is outerInstance.splitPane Then
						outerInstance.dragger.completeDrag(e.x, e.y)
					ElseIf e.source = BasicSplitPaneDivider.this Then
						Dim ourLoc As Point = location

						outerInstance.dragger.completeDrag(e.x + ourLoc.x, e.y + ourLoc.y)
					ElseIf e.source Is outerInstance.hiddenDivider Then
						Dim hDividerLoc As Point = outerInstance.hiddenDivider.location
						Dim ourX As Integer = e.x + hDividerLoc.x
						Dim ourY As Integer = e.y + hDividerLoc.y

						outerInstance.dragger.completeDrag(ourX, ourY)
					End If
					outerInstance.dragger = Nothing
					e.consume()
				End If
			End Sub


			'
			' MouseMotionListener
			'

			''' <summary>
			''' If dragger is not null it is messaged with continueDrag.
			''' </summary>
			Public Overridable Sub mouseDragged(ByVal e As MouseEvent)
				If outerInstance.dragger IsNot Nothing Then
					If e.source Is outerInstance.splitPane Then
						outerInstance.dragger.continueDrag(e.x, e.y)
					ElseIf e.source = BasicSplitPaneDivider.this Then
						Dim ourLoc As Point = location

						outerInstance.dragger.continueDrag(e.x + ourLoc.x, e.y + ourLoc.y)
					ElseIf e.source Is outerInstance.hiddenDivider Then
						Dim hDividerLoc As Point = outerInstance.hiddenDivider.location
						Dim ourX As Integer = e.x + hDividerLoc.x
						Dim ourY As Integer = e.y + hDividerLoc.y

						outerInstance.dragger.continueDrag(ourX, ourY)
					End If
					e.consume()
				End If
			End Sub


			''' <summary>
			'''  Resets the cursor based on the orientation.
			''' </summary>
			Public Overridable Sub mouseMoved(ByVal e As MouseEvent)
			End Sub

			''' <summary>
			''' Invoked when the mouse enters a component.
			''' </summary>
			''' <param name="e"> MouseEvent describing the details of the enter event.
			''' @since 1.5 </param>
			Public Overridable Sub mouseEntered(ByVal e As MouseEvent)
				If e.source = BasicSplitPaneDivider.this Then outerInstance.mouseOver = True
			End Sub

			''' <summary>
			''' Invoked when the mouse exits a component.
			''' </summary>
			''' <param name="e"> MouseEvent describing the details of the exit event.
			''' @since 1.5 </param>
			Public Overridable Sub mouseExited(ByVal e As MouseEvent)
				If e.source = BasicSplitPaneDivider.this Then outerInstance.mouseOver = False
			End Sub
		End Class


		''' <summary>
		''' Handles the events during a dragging session for a
		''' HORIZONTAL_SPLIT oriented split pane. This continually
		''' messages <code>dragDividerTo</code> and then when done messages
		''' <code>finishDraggingTo</code>. When an instance is created it should be
		''' messaged with <code>isValid</code> to insure that dragging can happen
		''' (dragging won't be allowed if the two views can not be resized).
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
		Protected Friend Class DragController
			Private ReadOnly outerInstance As BasicSplitPaneDivider

			''' <summary>
			''' Initial location of the divider.
			''' </summary>
			Friend initialX As Integer

			''' <summary>
			''' Maximum and minimum positions to drag to.
			''' </summary>
			Friend maxX, minX As Integer

			''' <summary>
			''' Initial location the mouse down happened at.
			''' </summary>
			Friend offset As Integer


			Protected Friend Sub New(ByVal outerInstance As BasicSplitPaneDivider, ByVal e As MouseEvent)
					Me.outerInstance = outerInstance
				Dim splitPane As JSplitPane = outerInstance.splitPaneUI.splitPane
				Dim leftC As Component = splitPane.leftComponent
				Dim rightC As Component = splitPane.rightComponent

				initialX = location.x
				If e.source = BasicSplitPaneDivider.this Then
					offset = e.x
				Else ' splitPane
					offset = e.x - initialX
				End If
				If leftC Is Nothing OrElse rightC Is Nothing OrElse offset < -1 OrElse offset >= size.width Then
					' Don't allow dragging.
					maxX = -1
				Else
					Dim insets As Insets = splitPane.insets

					If leftC.visible Then
						minX = leftC.minimumSize.width
						If insets IsNot Nothing Then minX += insets.left
					Else
						minX = 0
					End If
					If rightC.visible Then
						Dim right As Integer = If(insets IsNot Nothing, insets.right, 0)
						maxX = Math.Max(0, splitPane.size.width - (size.width + right) - rightC.minimumSize.width)
					Else
						Dim right As Integer = If(insets IsNot Nothing, insets.right, 0)
						maxX = Math.Max(0, splitPane.size.width - (size.width + right))
					End If
					If maxX < minX Then
							maxX = 0
							minX = maxX
					End If
				End If
			End Sub


			''' <summary>
			''' Returns true if the dragging session is valid.
			''' </summary>
			Protected Friend Overridable Property valid As Boolean
				Get
					Return (maxX > 0)
				End Get
			End Property


			''' <summary>
			''' Returns the new position to put the divider at based on
			''' the passed in MouseEvent.
			''' </summary>
			Protected Friend Overridable Function positionForMouseEvent(ByVal e As MouseEvent) As Integer
				Dim newX As Integer = If(e.source = BasicSplitPaneDivider.this, (e.x + location.x), e.x)

				newX = Math.Min(maxX, Math.Max(minX, newX - offset))
				Return newX
			End Function


			''' <summary>
			''' Returns the x argument, since this is used for horizontal
			''' splits.
			''' </summary>
			Protected Friend Overridable Function getNeededLocation(ByVal x As Integer, ByVal y As Integer) As Integer
				Dim newX As Integer

				newX = Math.Min(maxX, Math.Max(minX, x - offset))
				Return newX
			End Function


			Protected Friend Overridable Sub continueDrag(ByVal newX As Integer, ByVal newY As Integer)
				outerInstance.dragDividerTo(getNeededLocation(newX, newY))
			End Sub


			''' <summary>
			''' Messages dragDividerTo with the new location for the mouse
			''' event.
			''' </summary>
			Protected Friend Overridable Sub continueDrag(ByVal e As MouseEvent)
				outerInstance.dragDividerTo(positionForMouseEvent(e))
			End Sub


			Protected Friend Overridable Sub completeDrag(ByVal x As Integer, ByVal y As Integer)
				outerInstance.finishDraggingTo(getNeededLocation(x, y))
			End Sub


			''' <summary>
			''' Messages finishDraggingTo with the new location for the mouse
			''' event.
			''' </summary>
			Protected Friend Overridable Sub completeDrag(ByVal e As MouseEvent)
				outerInstance.finishDraggingTo(positionForMouseEvent(e))
			End Sub
		End Class ' End of BasicSplitPaneDivider.DragController


		''' <summary>
		''' Handles the events during a dragging session for a
		''' VERTICAL_SPLIT oriented split pane. This continually
		''' messages <code>dragDividerTo</code> and then when done messages
		''' <code>finishDraggingTo</code>. When an instance is created it should be
		''' messaged with <code>isValid</code> to insure that dragging can happen
		''' (dragging won't be allowed if the two views can not be resized).
		''' </summary>
		Protected Friend Class VerticalDragController
			Inherits DragController

			Private ReadOnly outerInstance As BasicSplitPaneDivider

			' DragControllers ivars are now in terms of y, not x. 
			Protected Friend Sub New(ByVal outerInstance As BasicSplitPaneDivider, ByVal e As MouseEvent)
					Me.outerInstance = outerInstance
				MyBase.New(e)
				Dim splitPane As JSplitPane = outerInstance.splitPaneUI.splitPane
				Dim leftC As Component = splitPane.leftComponent
				Dim rightC As Component = splitPane.rightComponent

				initialX = location.y
				If e.source = BasicSplitPaneDivider.this Then
					offset = e.y
				Else
					offset = e.y - initialX
				End If
				If leftC Is Nothing OrElse rightC Is Nothing OrElse offset < -1 OrElse offset > size.height Then
					' Don't allow dragging.
					maxX = -1
				Else
					Dim insets As Insets = splitPane.insets

					If leftC.visible Then
						minX = leftC.minimumSize.height
						If insets IsNot Nothing Then minX += insets.top
					Else
						minX = 0
					End If
					If rightC.visible Then
						Dim bottom As Integer = If(insets IsNot Nothing, insets.bottom, 0)

						maxX = Math.Max(0, splitPane.size.height - (size.height + bottom) - rightC.minimumSize.height)
					Else
						Dim bottom As Integer = If(insets IsNot Nothing, insets.bottom, 0)

						maxX = Math.Max(0, splitPane.size.height - (size.height + bottom))
					End If
					If maxX < minX Then
							maxX = 0
							minX = maxX
					End If
				End If
			End Sub


			''' <summary>
			''' Returns the y argument, since this is used for vertical
			''' splits.
			''' </summary>
			Protected Friend Overrides Function getNeededLocation(ByVal x As Integer, ByVal y As Integer) As Integer
				Dim newY As Integer

				newY = Math.Min(maxX, Math.Max(minX, y - offset))
				Return newY
			End Function


			''' <summary>
			''' Returns the new position to put the divider at based on
			''' the passed in MouseEvent.
			''' </summary>
			Protected Friend Overrides Function positionForMouseEvent(ByVal e As MouseEvent) As Integer
				Dim newY As Integer = If(e.source = BasicSplitPaneDivider.this, (e.y + location.y), e.y)


				newY = Math.Min(maxX, Math.Max(minX, newY - offset))
				Return newY
			End Function
		End Class ' End of BasicSplitPaneDividier.VerticalDragController


		''' <summary>
		''' Used to layout a <code>BasicSplitPaneDivider</code>.
		''' Layout for the divider
		''' involves appropriately moving the left/right buttons around.
		''' 
		''' </summary>
		Protected Friend Class DividerLayout
			Implements LayoutManager

			Private ReadOnly outerInstance As BasicSplitPaneDivider

			Public Sub New(ByVal outerInstance As BasicSplitPaneDivider)
				Me.outerInstance = outerInstance
			End Sub

			Public Overridable Sub layoutContainer(ByVal c As Container)
				If outerInstance.leftButton IsNot Nothing AndAlso outerInstance.rightButton IsNot Nothing AndAlso c Is BasicSplitPaneDivider.this Then
					If outerInstance.splitPane.oneTouchExpandable Then
						Dim insets As Insets = outerInstance.insets

						If outerInstance.orientation = JSplitPane.VERTICAL_SPLIT Then
							Dim extraX As Integer = If(insets IsNot Nothing, insets.left, 0)
							Dim blockSize As Integer = height

							If insets IsNot Nothing Then
								blockSize -= (insets.top + insets.bottom)
								blockSize = Math.Max(blockSize, 0)
							End If
							blockSize = Math.Min(blockSize, outerInstance.oneTouchSize)

							Dim y As Integer = (c.size.height - blockSize) / 2

							If Not outerInstance.centerOneTouchButtons Then
								y = If(insets IsNot Nothing, insets.top, 0)
								extraX = 0
							End If
							outerInstance.leftButton.boundsnds(extraX + outerInstance.oneTouchOffset, y, blockSize * 2, blockSize)
							outerInstance.rightButton.boundsnds(extraX + outerInstance.oneTouchOffset + outerInstance.oneTouchSize * 2, y, blockSize * 2, blockSize)
						Else
							Dim extraY As Integer = If(insets IsNot Nothing, insets.top, 0)
							Dim blockSize As Integer = width

							If insets IsNot Nothing Then
								blockSize -= (insets.left + insets.right)
								blockSize = Math.Max(blockSize, 0)
							End If
							blockSize = Math.Min(blockSize, outerInstance.oneTouchSize)

							Dim x As Integer = (c.size.width - blockSize) / 2

							If Not outerInstance.centerOneTouchButtons Then
								x = If(insets IsNot Nothing, insets.left, 0)
								extraY = 0
							End If

							outerInstance.leftButton.boundsnds(x, extraY + outerInstance.oneTouchOffset, blockSize, blockSize * 2)
							outerInstance.rightButton.boundsnds(x, extraY + outerInstance.oneTouchOffset + outerInstance.oneTouchSize * 2, blockSize, blockSize * 2)
						End If
					Else
						outerInstance.leftButton.boundsnds(-5, -5, 1, 1)
						outerInstance.rightButton.boundsnds(-5, -5, 1, 1)
					End If
				End If
			End Sub


			Public Overridable Function minimumLayoutSize(ByVal c As Container) As Dimension
				' NOTE: This isn't really used, refer to
				' BasicSplitPaneDivider.getPreferredSize for the reason.
				' I leave it in hopes of having this used at some point.
				If c IsNot BasicSplitPaneDivider.this OrElse outerInstance.splitPane Is Nothing Then Return New Dimension(0,0)
				Dim buttonMinSize As Dimension = Nothing

				If outerInstance.splitPane.oneTouchExpandable AndAlso outerInstance.leftButton IsNot Nothing Then buttonMinSize = outerInstance.leftButton.minimumSize

				Dim insets As Insets = outerInstance.insets
				Dim width As Integer = outerInstance.dividerSize
				Dim height As Integer = width

				If outerInstance.orientation = JSplitPane.VERTICAL_SPLIT Then
					If buttonMinSize IsNot Nothing Then
						Dim size As Integer = buttonMinSize.height
						If insets IsNot Nothing Then size += insets.top + insets.bottom
						height = Math.Max(height, size)
					End If
					width = 1
				Else
					If buttonMinSize IsNot Nothing Then
						Dim size As Integer = buttonMinSize.width
						If insets IsNot Nothing Then size += insets.left + insets.right
						width = Math.Max(width, size)
					End If
					height = 1
				End If
				Return New Dimension(width, height)
			End Function


			Public Overridable Function preferredLayoutSize(ByVal c As Container) As Dimension
				Return minimumLayoutSize(c)
			End Function


			Public Overridable Sub removeLayoutComponent(ByVal c As Component)
			End Sub

			Public Overridable Sub addLayoutComponent(ByVal [string] As String, ByVal c As Component)
			End Sub
		End Class ' End of class BasicSplitPaneDivider.DividerLayout


		''' <summary>
		''' Listeners installed on the one touch expandable buttons.
		''' </summary>
		Private Class OneTouchActionHandler
			Implements ActionListener

			Private ReadOnly outerInstance As BasicSplitPaneDivider

			''' <summary>
			''' True indicates the resize should go the minimum (top or left)
			''' vs false which indicates the resize should go to the maximum.
			''' </summary>
			Private toMinimum As Boolean

			Friend Sub New(ByVal outerInstance As BasicSplitPaneDivider, ByVal toMinimum As Boolean)
					Me.outerInstance = outerInstance
				Me.toMinimum = toMinimum
			End Sub

			Public Overridable Sub actionPerformed(ByVal e As ActionEvent)
				Dim insets As Insets = outerInstance.splitPane.insets
				Dim lastLoc As Integer = outerInstance.splitPane.lastDividerLocation
				Dim currentLoc As Integer = outerInstance.splitPaneUI.getDividerLocation(outerInstance.splitPane)
				Dim newLoc As Integer

				' We use the location from the UI directly, as the location the
				' JSplitPane itself maintains is not necessarly correct.
				If toMinimum Then
					If outerInstance.orientation = JSplitPane.VERTICAL_SPLIT Then
						If currentLoc >= (outerInstance.splitPane.height - insets.bottom - height) Then
							Dim maxLoc As Integer = outerInstance.splitPane.maximumDividerLocation
							newLoc = Math.Min(lastLoc, maxLoc)
							outerInstance.splitPaneUI.keepHidden = False
						Else
							newLoc = insets.top
							outerInstance.splitPaneUI.keepHidden = True
						End If
					Else
						If currentLoc >= (outerInstance.splitPane.width - insets.right - width) Then
							Dim maxLoc As Integer = outerInstance.splitPane.maximumDividerLocation
							newLoc = Math.Min(lastLoc, maxLoc)
							outerInstance.splitPaneUI.keepHidden = False
						Else
							newLoc = insets.left
							outerInstance.splitPaneUI.keepHidden = True
						End If
					End If
				Else
					If outerInstance.orientation = JSplitPane.VERTICAL_SPLIT Then
						If currentLoc = insets.top Then
							Dim maxLoc As Integer = outerInstance.splitPane.maximumDividerLocation
							newLoc = Math.Min(lastLoc, maxLoc)
							outerInstance.splitPaneUI.keepHidden = False
						Else
							newLoc = outerInstance.splitPane.height - height - insets.top
							outerInstance.splitPaneUI.keepHidden = True
						End If
					Else
						If currentLoc = insets.left Then
							Dim maxLoc As Integer = outerInstance.splitPane.maximumDividerLocation
							newLoc = Math.Min(lastLoc, maxLoc)
							outerInstance.splitPaneUI.keepHidden = False
						Else
							newLoc = outerInstance.splitPane.width - width - insets.left
							outerInstance.splitPaneUI.keepHidden = True
						End If
					End If
				End If
				If currentLoc <> newLoc Then
					outerInstance.splitPane.dividerLocation = newLoc
					' We do this in case the dividers notion of the location
					' differs from the real location.
					outerInstance.splitPane.lastDividerLocation = currentLoc
				End If
			End Sub
		End Class ' End of class BasicSplitPaneDivider.LeftActionListener
	End Class

End Namespace