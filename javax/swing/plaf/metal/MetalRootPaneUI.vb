Imports System
Imports javax.swing
Imports javax.swing.border
Imports javax.swing.event
Imports javax.swing.plaf
Imports javax.swing.plaf.basic

'
' * Copyright (c) 2000, 2013, Oracle and/or its affiliates. All rights reserved.
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
	''' Provides the metal look and feel implementation of <code>RootPaneUI</code>.
	''' <p>
	''' <code>MetalRootPaneUI</code> provides support for the
	''' <code>windowDecorationStyle</code> property of <code>JRootPane</code>.
	''' <code>MetalRootPaneUI</code> does this by way of installing a custom
	''' <code>LayoutManager</code>, a private <code>Component</code> to render
	''' the appropriate widgets, and a private <code>Border</code>. The
	''' <code>LayoutManager</code> is always installed, regardless of the value of
	''' the <code>windowDecorationStyle</code> property, but the
	''' <code>Border</code> and <code>Component</code> are only installed/added if
	''' the <code>windowDecorationStyle</code> is other than
	''' <code>JRootPane.NONE</code>.
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
	''' @author Terry Kellerman
	''' @since 1.4
	''' </summary>
	Public Class MetalRootPaneUI
		Inherits BasicRootPaneUI

		''' <summary>
		''' Keys to lookup borders in defaults table.
		''' </summary>
		Private Shared ReadOnly borderKeys As String() = { Nothing, "RootPane.frameBorder", "RootPane.plainDialogBorder", "RootPane.informationDialogBorder", "RootPane.errorDialogBorder", "RootPane.colorChooserDialogBorder", "RootPane.fileChooserDialogBorder", "RootPane.questionDialogBorder", "RootPane.warningDialogBorder" }
		''' <summary>
		''' The amount of space (in pixels) that the cursor is changed on.
		''' </summary>
		Private Const CORNER_DRAG_WIDTH As Integer = 16

		''' <summary>
		''' Region from edges that dragging is active from.
		''' </summary>
		Private Const BORDER_DRAG_THICKNESS As Integer = 5

		''' <summary>
		''' Window the <code>JRootPane</code> is in.
		''' </summary>
		Private window As Window

		''' <summary>
		''' <code>JComponent</code> providing window decorations. This will be
		''' null if not providing window decorations.
		''' </summary>
		Private titlePane As JComponent

		''' <summary>
		''' <code>MouseInputListener</code> that is added to the parent
		''' <code>Window</code> the <code>JRootPane</code> is contained in.
		''' </summary>
		Private mouseInputListener As MouseInputListener

		''' <summary>
		''' The <code>LayoutManager</code> that is set on the
		''' <code>JRootPane</code>.
		''' </summary>
		Private layoutManager As LayoutManager

		''' <summary>
		''' <code>LayoutManager</code> of the <code>JRootPane</code> before we
		''' replaced it.
		''' </summary>
		Private savedOldLayout As LayoutManager

		''' <summary>
		''' <code>JRootPane</code> providing the look and feel for.
		''' </summary>
		Private root As JRootPane

		''' <summary>
		''' <code>Cursor</code> used to track the cursor set by the user.
		''' This is initially <code>Cursor.DEFAULT_CURSOR</code>.
		''' </summary>
		Private lastCursor As Cursor = Cursor.getPredefinedCursor(Cursor.DEFAULT_CURSOR)

		''' <summary>
		''' Creates a UI for a <code>JRootPane</code>.
		''' </summary>
		''' <param name="c"> the JRootPane the RootPaneUI will be created for </param>
		''' <returns> the RootPaneUI implementation for the passed in JRootPane </returns>
		Public Shared Function createUI(ByVal c As JComponent) As ComponentUI
			Return New MetalRootPaneUI
		End Function

		''' <summary>
		''' Invokes supers implementation of <code>installUI</code> to install
		''' the necessary state onto the passed in <code>JRootPane</code>
		''' to render the metal look and feel implementation of
		''' <code>RootPaneUI</code>. If
		''' the <code>windowDecorationStyle</code> property of the
		''' <code>JRootPane</code> is other than <code>JRootPane.NONE</code>,
		''' this will add a custom <code>Component</code> to render the widgets to
		''' <code>JRootPane</code>, as well as installing a custom
		''' <code>Border</code> and <code>LayoutManager</code> on the
		''' <code>JRootPane</code>.
		''' </summary>
		''' <param name="c"> the JRootPane to install state onto </param>
		Public Overrides Sub installUI(ByVal c As JComponent)
			MyBase.installUI(c)
			root = CType(c, JRootPane)
			Dim style As Integer = root.windowDecorationStyle
			If style <> JRootPane.NONE Then installClientDecorations(root)
		End Sub


		''' <summary>
		''' Invokes supers implementation to uninstall any of its state. This will
		''' also reset the <code>LayoutManager</code> of the <code>JRootPane</code>.
		''' If a <code>Component</code> has been added to the <code>JRootPane</code>
		''' to render the window decoration style, this method will remove it.
		''' Similarly, this will revert the Border and LayoutManager of the
		''' <code>JRootPane</code> to what it was before <code>installUI</code>
		''' was invoked.
		''' </summary>
		''' <param name="c"> the JRootPane to uninstall state from </param>
		Public Overrides Sub uninstallUI(ByVal c As JComponent)
			MyBase.uninstallUI(c)
			uninstallClientDecorations(root)

			layoutManager = Nothing
			mouseInputListener = Nothing
			root = Nothing
		End Sub

		''' <summary>
		''' Installs the appropriate <code>Border</code> onto the
		''' <code>JRootPane</code>.
		''' </summary>
		Friend Overridable Sub installBorder(ByVal root As JRootPane)
			Dim style As Integer = root.windowDecorationStyle

			If style = JRootPane.NONE Then
				LookAndFeel.uninstallBorder(root)
			Else
				LookAndFeel.installBorder(root, borderKeys(style))
			End If
		End Sub

		''' <summary>
		''' Removes any border that may have been installed.
		''' </summary>
		Private Sub uninstallBorder(ByVal root As JRootPane)
			LookAndFeel.uninstallBorder(root)
		End Sub

		''' <summary>
		''' Installs the necessary Listeners on the parent <code>Window</code>,
		''' if there is one.
		''' <p>
		''' This takes the parent so that cleanup can be done from
		''' <code>removeNotify</code>, at which point the parent hasn't been
		''' reset yet.
		''' </summary>
		''' <param name="parent"> The parent of the JRootPane </param>
		Private Sub installWindowListeners(ByVal root As JRootPane, ByVal parent As Component)
			If TypeOf parent Is Window Then
				window = CType(parent, Window)
			Else
				window = SwingUtilities.getWindowAncestor(parent)
			End If
			If window IsNot Nothing Then
				If mouseInputListener Is Nothing Then mouseInputListener = createWindowMouseInputListener(root)
				window.addMouseListener(mouseInputListener)
				window.addMouseMotionListener(mouseInputListener)
			End If
		End Sub

		''' <summary>
		''' Uninstalls the necessary Listeners on the <code>Window</code> the
		''' Listeners were last installed on.
		''' </summary>
		Private Sub uninstallWindowListeners(ByVal root As JRootPane)
			If window IsNot Nothing Then
				window.removeMouseListener(mouseInputListener)
				window.removeMouseMotionListener(mouseInputListener)
			End If
		End Sub

		''' <summary>
		''' Installs the appropriate LayoutManager on the <code>JRootPane</code>
		''' to render the window decorations.
		''' </summary>
		Private Sub installLayout(ByVal root As JRootPane)
			If layoutManager Is Nothing Then layoutManager = createLayoutManager()
			savedOldLayout = root.layout
			root.layout = layoutManager
		End Sub

		''' <summary>
		''' Uninstalls the previously installed <code>LayoutManager</code>.
		''' </summary>
		Private Sub uninstallLayout(ByVal root As JRootPane)
			If savedOldLayout IsNot Nothing Then
				root.layout = savedOldLayout
				savedOldLayout = Nothing
			End If
		End Sub

		''' <summary>
		''' Installs the necessary state onto the JRootPane to render client
		''' decorations. This is ONLY invoked if the <code>JRootPane</code>
		''' has a decoration style other than <code>JRootPane.NONE</code>.
		''' </summary>
		Private Sub installClientDecorations(ByVal root As JRootPane)
			installBorder(root)

			Dim ___titlePane As JComponent = createTitlePane(root)

			titlePaneane(root, ___titlePane)
			installWindowListeners(root, root.parent)
			installLayout(root)
			If window IsNot Nothing Then
				root.revalidate()
				root.repaint()
			End If
		End Sub

		''' <summary>
		''' Uninstalls any state that <code>installClientDecorations</code> has
		''' installed.
		''' <p>
		''' NOTE: This may be called if you haven't installed client decorations
		''' yet (ie before <code>installClientDecorations</code> has been invoked).
		''' </summary>
		Private Sub uninstallClientDecorations(ByVal root As JRootPane)
			uninstallBorder(root)
			uninstallWindowListeners(root)
			titlePaneane(root, Nothing)
			uninstallLayout(root)
			' We have to revalidate/repaint root if the style is JRootPane.NONE
			' only. When we needs to call revalidate/repaint with other styles
			' the installClientDecorations is always called after this method
			' imediatly and it will cause the revalidate/repaint at the proper
			' time.
			Dim style As Integer = root.windowDecorationStyle
			If style = JRootPane.NONE Then
				root.repaint()
				root.revalidate()
			End If
			' Reset the cursor, as we may have changed it to a resize cursor
			If window IsNot Nothing Then window.cursor = Cursor.getPredefinedCursor(Cursor.DEFAULT_CURSOR)
			window = Nothing
		End Sub

		''' <summary>
		''' Returns the <code>JComponent</code> to render the window decoration
		''' style.
		''' </summary>
		Private Function createTitlePane(ByVal root As JRootPane) As JComponent
			Return New MetalTitlePane(root, Me)
		End Function

		''' <summary>
		''' Returns a <code>MouseListener</code> that will be added to the
		''' <code>Window</code> containing the <code>JRootPane</code>.
		''' </summary>
		Private Function createWindowMouseInputListener(ByVal root As JRootPane) As MouseInputListener
			Return New MouseInputHandler(Me)
		End Function

		''' <summary>
		''' Returns a <code>LayoutManager</code> that will be set on the
		''' <code>JRootPane</code>.
		''' </summary>
		Private Function createLayoutManager() As LayoutManager
			Return New MetalRootLayout
		End Function

		''' <summary>
		''' Sets the window title pane -- the JComponent used to provide a plaf a
		''' way to override the native operating system's window title pane with
		''' one whose look and feel are controlled by the plaf.  The plaf creates
		''' and sets this value; the default is null, implying a native operating
		''' system window title pane.
		''' </summary>
		''' <param name="content"> the <code>JComponent</code> to use for the window title pane. </param>
		Private Sub setTitlePane(ByVal root As JRootPane, ByVal titlePane As JComponent)
			Dim layeredPane As JLayeredPane = root.layeredPane
			Dim oldTitlePane As JComponent = titlePane

			If oldTitlePane IsNot Nothing Then
				oldTitlePane.visible = False
				layeredPane.remove(oldTitlePane)
			End If
			If titlePane IsNot Nothing Then
				layeredPane.add(titlePane, JLayeredPane.FRAME_CONTENT_LAYER)
				titlePane.visible = True
			End If
			Me.titlePane = titlePane
		End Sub

		''' <summary>
		''' Returns the <code>JComponent</code> rendering the title pane. If this
		''' returns null, it implies there is no need to render window decorations.
		''' </summary>
		''' <returns> the current window title pane, or null </returns>
		''' <seealso cref= #setTitlePane </seealso>
		Private Property titlePane As JComponent
			Get
				Return titlePane
			End Get
		End Property

		''' <summary>
		''' Returns the <code>JRootPane</code> we're providing the look and
		''' feel for.
		''' </summary>
		Private Property rootPane As JRootPane
			Get
				Return root
			End Get
		End Property

		''' <summary>
		''' Invoked when a property changes. <code>MetalRootPaneUI</code> is
		''' primarily interested in events originating from the
		''' <code>JRootPane</code> it has been installed on identifying the
		''' property <code>windowDecorationStyle</code>. If the
		''' <code>windowDecorationStyle</code> has changed to a value other
		''' than <code>JRootPane.NONE</code>, this will add a <code>Component</code>
		''' to the <code>JRootPane</code> to render the window decorations, as well
		''' as installing a <code>Border</code> on the <code>JRootPane</code>.
		''' On the other hand, if the <code>windowDecorationStyle</code> has
		''' changed to <code>JRootPane.NONE</code>, this will remove the
		''' <code>Component</code> that has been added to the <code>JRootPane</code>
		''' as well resetting the Border to what it was before
		''' <code>installUI</code> was invoked.
		''' </summary>
		''' <param name="e"> A PropertyChangeEvent object describing the event source
		'''          and the property that has changed. </param>
		Public Overrides Sub propertyChange(ByVal e As java.beans.PropertyChangeEvent)
			MyBase.propertyChange(e)

			Dim propertyName As String = e.propertyName
			If propertyName Is Nothing Then Return

			If propertyName.Equals("windowDecorationStyle") Then
				Dim root As JRootPane = CType(e.source, JRootPane)
				Dim style As Integer = root.windowDecorationStyle

				' This is potentially more than needs to be done,
				' but it rarely happens and makes the install/uninstall process
				' simpler. MetalTitlePane also assumes it will be recreated if
				' the decoration style changes.
				uninstallClientDecorations(root)
				If style <> JRootPane.NONE Then installClientDecorations(root)
			ElseIf propertyName.Equals("ancestor") Then
				uninstallWindowListeners(root)
				If CType(e.source, JRootPane).windowDecorationStyle <> JRootPane.NONE Then installWindowListeners(root, root.parent)
			End If
			Return
		End Sub

		''' <summary>
		''' A custom layout manager that is responsible for the layout of
		''' layeredPane, glassPane, menuBar and titlePane, if one has been
		''' installed.
		''' </summary>
		' NOTE: Ideally this would extends JRootPane.RootLayout, but that
		'       would force this to be non-static.
		Private Class MetalRootLayout
			Implements LayoutManager2

			''' <summary>
			''' Returns the amount of space the layout would like to have.
			''' </summary>
			''' <param name="the"> Container for which this layout manager is being used </param>
			''' <returns> a Dimension object containing the layout's preferred size </returns>
			Public Overridable Function preferredLayoutSize(ByVal parent As Container) As Dimension
				Dim cpd, mbd, tpd As Dimension
				Dim cpWidth As Integer = 0
				Dim cpHeight As Integer = 0
				Dim mbWidth As Integer = 0
				Dim mbHeight As Integer = 0
				Dim tpWidth As Integer = 0
				Dim tpHeight As Integer = 0
				Dim i As Insets = parent.insets
				Dim root As JRootPane = CType(parent, JRootPane)

				If root.contentPane IsNot Nothing Then
					cpd = root.contentPane.preferredSize
				Else
					cpd = root.size
				End If
				If cpd IsNot Nothing Then
					cpWidth = cpd.width
					cpHeight = cpd.height
				End If

				If root.menuBar IsNot Nothing Then
					mbd = root.menuBar.preferredSize
					If mbd IsNot Nothing Then
						mbWidth = mbd.width
						mbHeight = mbd.height
					End If
				End If

				If root.windowDecorationStyle <> JRootPane.NONE AndAlso (TypeOf root.uI Is MetalRootPaneUI) Then
					Dim titlePane As JComponent = CType(root.uI, MetalRootPaneUI).titlePane
					If titlePane IsNot Nothing Then
						tpd = titlePane.preferredSize
						If tpd IsNot Nothing Then
							tpWidth = tpd.width
							tpHeight = tpd.height
						End If
					End If
				End If

				Return New Dimension(Math.Max(Math.Max(cpWidth, mbWidth), tpWidth) + i.left + i.right, cpHeight + mbHeight + tpWidth + i.top + i.bottom)
			End Function

			''' <summary>
			''' Returns the minimum amount of space the layout needs.
			''' </summary>
			''' <param name="the"> Container for which this layout manager is being used </param>
			''' <returns> a Dimension object containing the layout's minimum size </returns>
			Public Overridable Function minimumLayoutSize(ByVal parent As Container) As Dimension
				Dim cpd, mbd, tpd As Dimension
				Dim cpWidth As Integer = 0
				Dim cpHeight As Integer = 0
				Dim mbWidth As Integer = 0
				Dim mbHeight As Integer = 0
				Dim tpWidth As Integer = 0
				Dim tpHeight As Integer = 0
				Dim i As Insets = parent.insets
				Dim root As JRootPane = CType(parent, JRootPane)

				If root.contentPane IsNot Nothing Then
					cpd = root.contentPane.minimumSize
				Else
					cpd = root.size
				End If
				If cpd IsNot Nothing Then
					cpWidth = cpd.width
					cpHeight = cpd.height
				End If

				If root.menuBar IsNot Nothing Then
					mbd = root.menuBar.minimumSize
					If mbd IsNot Nothing Then
						mbWidth = mbd.width
						mbHeight = mbd.height
					End If
				End If
				If root.windowDecorationStyle <> JRootPane.NONE AndAlso (TypeOf root.uI Is MetalRootPaneUI) Then
					Dim titlePane As JComponent = CType(root.uI, MetalRootPaneUI).titlePane
					If titlePane IsNot Nothing Then
						tpd = titlePane.minimumSize
						If tpd IsNot Nothing Then
							tpWidth = tpd.width
							tpHeight = tpd.height
						End If
					End If
				End If

				Return New Dimension(Math.Max(Math.Max(cpWidth, mbWidth), tpWidth) + i.left + i.right, cpHeight + mbHeight + tpWidth + i.top + i.bottom)
			End Function

			''' <summary>
			''' Returns the maximum amount of space the layout can use.
			''' </summary>
			''' <param name="the"> Container for which this layout manager is being used </param>
			''' <returns> a Dimension object containing the layout's maximum size </returns>
			Public Overridable Function maximumLayoutSize(ByVal target As Container) As Dimension
				Dim cpd, mbd, tpd As Dimension
				Dim cpWidth As Integer = Integer.MaxValue
				Dim cpHeight As Integer = Integer.MaxValue
				Dim mbWidth As Integer = Integer.MaxValue
				Dim mbHeight As Integer = Integer.MaxValue
				Dim tpWidth As Integer = Integer.MaxValue
				Dim tpHeight As Integer = Integer.MaxValue
				Dim i As Insets = target.insets
				Dim root As JRootPane = CType(target, JRootPane)

				If root.contentPane IsNot Nothing Then
					cpd = root.contentPane.maximumSize
					If cpd IsNot Nothing Then
						cpWidth = cpd.width
						cpHeight = cpd.height
					End If
				End If

				If root.menuBar IsNot Nothing Then
					mbd = root.menuBar.maximumSize
					If mbd IsNot Nothing Then
						mbWidth = mbd.width
						mbHeight = mbd.height
					End If
				End If

				If root.windowDecorationStyle <> JRootPane.NONE AndAlso (TypeOf root.uI Is MetalRootPaneUI) Then
					Dim titlePane As JComponent = CType(root.uI, MetalRootPaneUI).titlePane
					If titlePane IsNot Nothing Then
						tpd = titlePane.maximumSize
						If tpd IsNot Nothing Then
							tpWidth = tpd.width
							tpHeight = tpd.height
						End If
					End If
				End If

				Dim maxHeight As Integer = Math.Max(Math.Max(cpHeight, mbHeight), tpHeight)
				' Only overflows if 3 real non-MAX_VALUE heights, sum to > MAX_VALUE
				' Only will happen if sums to more than 2 billion units.  Not likely.
				If maxHeight <> Integer.MaxValue Then maxHeight = cpHeight + mbHeight + tpHeight + i.top + i.bottom

				Dim maxWidth As Integer = Math.Max(Math.Max(cpWidth, mbWidth), tpWidth)
				' Similar overflow comment as above
				If maxWidth <> Integer.MaxValue Then maxWidth += i.left + i.right

				Return New Dimension(maxWidth, maxHeight)
			End Function

			''' <summary>
			''' Instructs the layout manager to perform the layout for the specified
			''' container.
			''' </summary>
			''' <param name="the"> Container for which this layout manager is being used </param>
			Public Overridable Sub layoutContainer(ByVal parent As Container)
				Dim root As JRootPane = CType(parent, JRootPane)
				Dim b As Rectangle = root.bounds
				Dim i As Insets = root.insets
				Dim nextY As Integer = 0
				Dim w As Integer = b.width - i.right - i.left
				Dim h As Integer = b.height - i.top - i.bottom

				If root.layeredPane IsNot Nothing Then root.layeredPane.boundsnds(i.left, i.top, w, h)
				If root.glassPane IsNot Nothing Then root.glassPane.boundsnds(i.left, i.top, w, h)
				' Note: This is laying out the children in the layeredPane,
				' technically, these are not our children.
				If root.windowDecorationStyle <> JRootPane.NONE AndAlso (TypeOf root.uI Is MetalRootPaneUI) Then
					Dim titlePane As JComponent = CType(root.uI, MetalRootPaneUI).titlePane
					If titlePane IsNot Nothing Then
						Dim tpd As Dimension = titlePane.preferredSize
						If tpd IsNot Nothing Then
							Dim tpHeight As Integer = tpd.height
							titlePane.boundsnds(0, 0, w, tpHeight)
							nextY += tpHeight
						End If
					End If
				End If
				If root.menuBar IsNot Nothing Then
					Dim mbd As Dimension = root.menuBar.preferredSize
					root.menuBar.boundsnds(0, nextY, w, mbd.height)
					nextY += mbd.height
				End If
				If root.contentPane IsNot Nothing Then
					Dim cpd As Dimension = root.contentPane.preferredSize
					root.contentPane.boundsnds(0, nextY, w,If(h < nextY, 0, h - nextY))
				End If
			End Sub

			Public Overridable Sub addLayoutComponent(ByVal name As String, ByVal comp As Component)
			End Sub
			Public Overridable Sub removeLayoutComponent(ByVal comp As Component)
			End Sub
			Public Overridable Sub addLayoutComponent(ByVal comp As Component, ByVal constraints As Object)
			End Sub
			Public Overridable Function getLayoutAlignmentX(ByVal target As Container) As Single
				Return 0.0f
			End Function
			Public Overridable Function getLayoutAlignmentY(ByVal target As Container) As Single
				Return 0.0f
			End Function
			Public Overridable Sub invalidateLayout(ByVal target As Container)
			End Sub
		End Class


		''' <summary>
		''' Maps from positions to cursor type. Refer to calculateCorner and
		''' calculatePosition for details of this.
		''' </summary>
		Private Shared ReadOnly cursorMapping As Integer() = { Cursor.NW_RESIZE_CURSOR, Cursor.NW_RESIZE_CURSOR, Cursor.N_RESIZE_CURSOR, Cursor.NE_RESIZE_CURSOR, Cursor.NE_RESIZE_CURSOR, Cursor.NW_RESIZE_CURSOR, 0, 0, 0, Cursor.NE_RESIZE_CURSOR, Cursor.W_RESIZE_CURSOR, 0, 0, 0, Cursor.E_RESIZE_CURSOR, Cursor.SW_RESIZE_CURSOR, 0, 0, 0, Cursor.SE_RESIZE_CURSOR, Cursor.SW_RESIZE_CURSOR, Cursor.SW_RESIZE_CURSOR, Cursor.S_RESIZE_CURSOR, Cursor.SE_RESIZE_CURSOR, Cursor.SE_RESIZE_CURSOR }

		''' <summary>
		''' MouseInputHandler is responsible for handling resize/moving of
		''' the Window. It sets the cursor directly on the Window when then
		''' mouse moves over a hot spot.
		''' </summary>
		Private Class MouseInputHandler
			Implements MouseInputListener

			Private ReadOnly outerInstance As MetalRootPaneUI

			Public Sub New(ByVal outerInstance As MetalRootPaneUI)
				Me.outerInstance = outerInstance
			End Sub

			''' <summary>
			''' Set to true if the drag operation is moving the window.
			''' </summary>
			Private isMovingWindow As Boolean

			''' <summary>
			''' Used to determine the corner the resize is occurring from.
			''' </summary>
			Private dragCursor As Integer

			''' <summary>
			''' X location the mouse went down on for a drag operation.
			''' </summary>
			Private dragOffsetX As Integer

			''' <summary>
			''' Y location the mouse went down on for a drag operation.
			''' </summary>
			Private dragOffsetY As Integer

			''' <summary>
			''' Width of the window when the drag started.
			''' </summary>
			Private dragWidth As Integer

			''' <summary>
			''' Height of the window when the drag started.
			''' </summary>
			Private dragHeight As Integer

			Public Overridable Sub mousePressed(ByVal ev As MouseEvent)
				Dim rootPane As JRootPane = outerInstance.rootPane

				If rootPane.windowDecorationStyle = JRootPane.NONE Then Return
				Dim dragWindowOffset As Point = ev.point
				Dim w As Window = CType(ev.source, Window)
				If w IsNot Nothing Then w.toFront()
				Dim convertedDragWindowOffset As Point = SwingUtilities.convertPoint(w, dragWindowOffset, outerInstance.titlePane)

				Dim f As Frame = Nothing
				Dim d As Dialog = Nothing

				If TypeOf w Is Frame Then
					f = CType(w, Frame)
				ElseIf TypeOf w Is Dialog Then
					d = CType(w, Dialog)
				End If

				Dim frameState As Integer = If(f IsNot Nothing, f.extendedState, 0)

				If outerInstance.titlePane IsNot Nothing AndAlso outerInstance.titlePane.contains(convertedDragWindowOffset) Then
					If (f IsNot Nothing AndAlso ((frameState And Frame.MAXIMIZED_BOTH) = 0) OrElse (d IsNot Nothing)) AndAlso dragWindowOffset.y >= BORDER_DRAG_THICKNESS AndAlso dragWindowOffset.x >= BORDER_DRAG_THICKNESS AndAlso dragWindowOffset.x < w.width - BORDER_DRAG_THICKNESS Then
						isMovingWindow = True
						dragOffsetX = dragWindowOffset.x
						dragOffsetY = dragWindowOffset.y
					End If
				ElseIf f IsNot Nothing AndAlso f.resizable AndAlso ((frameState And Frame.MAXIMIZED_BOTH) = 0) OrElse (d IsNot Nothing AndAlso d.resizable) Then
					dragOffsetX = dragWindowOffset.x
					dragOffsetY = dragWindowOffset.y
					dragWidth = w.width
					dragHeight = w.height
					dragCursor = getCursor(calculateCorner(w, dragWindowOffset.x, dragWindowOffset.y))
				End If
			End Sub

			Public Overridable Sub mouseReleased(ByVal ev As MouseEvent)
				If dragCursor <> 0 AndAlso outerInstance.window IsNot Nothing AndAlso (Not outerInstance.window.valid) Then
					' Some Window systems validate as you resize, others won't,
					' thus the check for validity before repainting.
					outerInstance.window.validate()
					outerInstance.rootPane.repaint()
				End If
				isMovingWindow = False
				dragCursor = 0
			End Sub

			Public Overridable Sub mouseMoved(ByVal ev As MouseEvent)
				Dim root As JRootPane = outerInstance.rootPane

				If root.windowDecorationStyle = JRootPane.NONE Then Return

				Dim w As Window = CType(ev.source, Window)

				Dim f As Frame = Nothing
				Dim d As Dialog = Nothing

				If TypeOf w Is Frame Then
					f = CType(w, Frame)
				ElseIf TypeOf w Is Dialog Then
					d = CType(w, Dialog)
				End If

				' Update the cursor
				Dim ___cursor As Integer = getCursor(calculateCorner(w, ev.x, ev.y))

				If ___cursor <> 0 AndAlso ((f IsNot Nothing AndAlso (f.resizable AndAlso (f.extendedState And Frame.MAXIMIZED_BOTH) = 0)) OrElse (d IsNot Nothing AndAlso d.resizable)) Then
					w.cursor = Cursor.getPredefinedCursor(___cursor)
				Else
					w.cursor = outerInstance.lastCursor
				End If
			End Sub

			Private Sub adjust(ByVal bounds As Rectangle, ByVal min As Dimension, ByVal deltaX As Integer, ByVal deltaY As Integer, ByVal deltaWidth As Integer, ByVal deltaHeight As Integer)
				bounds.x += deltaX
				bounds.y += deltaY
				bounds.width += deltaWidth
				bounds.height += deltaHeight
				If min IsNot Nothing Then
					If bounds.width < min.width Then
						Dim correction As Integer = min.width - bounds.width
						If deltaX <> 0 Then bounds.x -= correction
						bounds.width = min.width
					End If
					If bounds.height < min.height Then
						Dim correction As Integer = min.height - bounds.height
						If deltaY <> 0 Then bounds.y -= correction
						bounds.height = min.height
					End If
				End If
			End Sub

			Public Overridable Sub mouseDragged(ByVal ev As MouseEvent)
				Dim w As Window = CType(ev.source, Window)
				Dim pt As Point = ev.point

				If isMovingWindow Then
					Dim eventLocationOnScreen As Point = ev.locationOnScreen
					w.locationion(eventLocationOnScreen.x - dragOffsetX, eventLocationOnScreen.y - dragOffsetY)
				ElseIf dragCursor <> 0 Then
					Dim r As Rectangle = w.bounds
					Dim startBounds As New Rectangle(r)
					Dim min As Dimension = w.minimumSize

					Select Case dragCursor
					Case Cursor.E_RESIZE_CURSOR
						adjust(r, min, 0, 0, pt.x + (dragWidth - dragOffsetX) - r.width, 0)
					Case Cursor.S_RESIZE_CURSOR
						adjust(r, min, 0, 0, 0, pt.y + (dragHeight - dragOffsetY) - r.height)
					Case Cursor.N_RESIZE_CURSOR
						adjust(r, min, 0, pt.y -dragOffsetY, 0, -(pt.y - dragOffsetY))
					Case Cursor.W_RESIZE_CURSOR
						adjust(r, min, pt.x - dragOffsetX, 0, -(pt.x - dragOffsetX), 0)
					Case Cursor.NE_RESIZE_CURSOR
						adjust(r, min, 0, pt.y - dragOffsetY, pt.x + (dragWidth - dragOffsetX) - r.width, -(pt.y - dragOffsetY))
					Case Cursor.SE_RESIZE_CURSOR
						adjust(r, min, 0, 0, pt.x + (dragWidth - dragOffsetX) - r.width, pt.y + (dragHeight - dragOffsetY) - r.height)
					Case Cursor.NW_RESIZE_CURSOR
						adjust(r, min, pt.x - dragOffsetX, pt.y - dragOffsetY, -(pt.x - dragOffsetX), -(pt.y - dragOffsetY))
					Case Cursor.SW_RESIZE_CURSOR
						adjust(r, min, pt.x - dragOffsetX, 0, -(pt.x - dragOffsetX), pt.y + (dragHeight - dragOffsetY) - r.height)
					Case Else
					End Select
					If Not r.Equals(startBounds) Then
						w.bounds = r
						' Defer repaint/validate on mouseReleased unless dynamic
						' layout is active.
						If Toolkit.defaultToolkit.dynamicLayoutActive Then
							w.validate()
							outerInstance.rootPane.repaint()
						End If
					End If
				End If
			End Sub

			Public Overridable Sub mouseEntered(ByVal ev As MouseEvent)
				Dim w As Window = CType(ev.source, Window)
				outerInstance.lastCursor = w.cursor
				mouseMoved(ev)
			End Sub

			Public Overridable Sub mouseExited(ByVal ev As MouseEvent)
				Dim w As Window = CType(ev.source, Window)
				w.cursor = outerInstance.lastCursor
			End Sub

			Public Overridable Sub mouseClicked(ByVal ev As MouseEvent)
				Dim w As Window = CType(ev.source, Window)
				Dim f As Frame = Nothing

				If TypeOf w Is Frame Then
					f = CType(w, Frame)
				Else
					Return
				End If

				Dim convertedPoint As Point = SwingUtilities.convertPoint(w, ev.point, outerInstance.titlePane)

				Dim state As Integer = f.extendedState
				If outerInstance.titlePane IsNot Nothing AndAlso outerInstance.titlePane.contains(convertedPoint) Then
					If (ev.clickCount Mod 2) = 0 AndAlso ((ev.modifiers And InputEvent.BUTTON1_MASK) <> 0) Then
						If f.resizable Then
							If (state And Frame.MAXIMIZED_BOTH) <> 0 Then
								f.extendedState = state And (Not Frame.MAXIMIZED_BOTH)
							Else
								f.extendedState = state Or Frame.MAXIMIZED_BOTH
							End If
							Return
						End If
					End If
				End If
			End Sub

			''' <summary>
			''' Returns the corner that contains the point <code>x</code>,
			''' <code>y</code>, or -1 if the position doesn't match a corner.
			''' </summary>
			Private Function calculateCorner(ByVal w As Window, ByVal x As Integer, ByVal y As Integer) As Integer
				Dim insets As Insets = w.insets
				Dim xPosition As Integer = calculatePosition(x - insets.left, w.width - insets.left - insets.right)
				Dim yPosition As Integer = calculatePosition(y - insets.top, w.height - insets.top - insets.bottom)

				If xPosition = -1 OrElse yPosition = -1 Then Return -1
				Return yPosition * 5 + xPosition
			End Function

			''' <summary>
			''' Returns the Cursor to render for the specified corner. This returns
			''' 0 if the corner doesn't map to a valid Cursor
			''' </summary>
			Private Function getCursor(ByVal corner As Integer) As Integer
				If corner = -1 Then Return 0
				Return cursorMapping(corner)
			End Function

			''' <summary>
			''' Returns an integer indicating the position of <code>spot</code>
			''' in <code>width</code>. The return value will be:
			''' 0 if < BORDER_DRAG_THICKNESS
			''' 1 if < CORNER_DRAG_WIDTH
			''' 2 if >= CORNER_DRAG_WIDTH && < width - BORDER_DRAG_THICKNESS
			''' 3 if >= width - CORNER_DRAG_WIDTH
			''' 4 if >= width - BORDER_DRAG_THICKNESS
			''' 5 otherwise
			''' </summary>
			Private Function calculatePosition(ByVal spot As Integer, ByVal width As Integer) As Integer
				If spot < BORDER_DRAG_THICKNESS Then Return 0
				If spot < CORNER_DRAG_WIDTH Then Return 1
				If spot >= (width - BORDER_DRAG_THICKNESS) Then Return 4
				If spot >= (width - CORNER_DRAG_WIDTH) Then Return 3
				Return 2
			End Function
		End Class
	End Class

End Namespace