Imports System
Imports System.Diagnostics
Imports javax.swing
Imports javax.swing.event
Imports javax.swing.border
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
	''' A default L&amp;F implementation of ScrollPaneUI.
	''' 
	''' @author Hans Muller
	''' </summary>
	Public Class BasicScrollPaneUI
		Inherits ScrollPaneUI
		Implements ScrollPaneConstants

		Protected Friend scrollpane As JScrollPane
		Protected Friend vsbChangeListener As ChangeListener
		Protected Friend hsbChangeListener As ChangeListener
		Protected Friend viewportChangeListener As ChangeListener
		Protected Friend spPropertyChangeListener As java.beans.PropertyChangeListener
		Private mouseScrollListener As MouseWheelListener
		Private oldExtent As Integer = Integer.MIN_VALUE

		''' <summary>
		''' PropertyChangeListener installed on the vertical scrollbar.
		''' </summary>
		Private vsbPropertyChangeListener As java.beans.PropertyChangeListener

		''' <summary>
		''' PropertyChangeListener installed on the horizontal scrollbar.
		''' </summary>
		Private hsbPropertyChangeListener As java.beans.PropertyChangeListener

		Private handler As Handler

		''' <summary>
		''' State flag that shows whether setValue() was called from a user program
		''' before the value of "extent" was set in right-to-left component
		''' orientation.
		''' </summary>
		Private valueCalledled As Boolean = False


		Public Shared Function createUI(ByVal x As JComponent) As ComponentUI
			Return New BasicScrollPaneUI
		End Function

		Friend Shared Sub loadActionMap(ByVal map As LazyActionMap)
			map.put(New Actions(Actions.SCROLL_UP))
			map.put(New Actions(Actions.SCROLL_DOWN))
			map.put(New Actions(Actions.SCROLL_HOME))
			map.put(New Actions(Actions.SCROLL_END))
			map.put(New Actions(Actions.UNIT_SCROLL_UP))
			map.put(New Actions(Actions.UNIT_SCROLL_DOWN))
			map.put(New Actions(Actions.SCROLL_LEFT))
			map.put(New Actions(Actions.SCROLL_RIGHT))
			map.put(New Actions(Actions.UNIT_SCROLL_RIGHT))
			map.put(New Actions(Actions.UNIT_SCROLL_LEFT))
		End Sub



		Public Overridable Sub paint(ByVal g As java.awt.Graphics, ByVal c As JComponent)
			Dim vpBorder As Border = scrollpane.viewportBorder
			If vpBorder IsNot Nothing Then
				Dim r As java.awt.Rectangle = scrollpane.viewportBorderBounds
				vpBorder.paintBorder(scrollpane, g, r.x, r.y, r.width, r.height)
			End If
		End Sub


		''' <returns> new Dimension(Short.MAX_VALUE, Short.MAX_VALUE) </returns>
		Public Overridable Function getMaximumSize(ByVal c As JComponent) As java.awt.Dimension
			Return New java.awt.Dimension(Short.MaxValue, Short.MaxValue)
		End Function


		Protected Friend Overridable Sub installDefaults(ByVal scrollpane As JScrollPane)
			LookAndFeel.installBorder(scrollpane, "ScrollPane.border")
			LookAndFeel.installColorsAndFont(scrollpane, "ScrollPane.background", "ScrollPane.foreground", "ScrollPane.font")

			Dim vpBorder As Border = scrollpane.viewportBorder
			If (vpBorder Is Nothing) OrElse (TypeOf vpBorder Is UIResource) Then
				vpBorder = UIManager.getBorder("ScrollPane.viewportBorder")
				scrollpane.viewportBorder = vpBorder
			End If
			LookAndFeel.installProperty(scrollpane, "opaque", Boolean.TRUE)
		End Sub


		Protected Friend Overridable Sub installListeners(ByVal c As JScrollPane)
			vsbChangeListener = createVSBChangeListener()
			vsbPropertyChangeListener = createVSBPropertyChangeListener()
			hsbChangeListener = createHSBChangeListener()
			hsbPropertyChangeListener = createHSBPropertyChangeListener()
			viewportChangeListener = createViewportChangeListener()
			spPropertyChangeListener = createPropertyChangeListener()

			Dim viewport As JViewport = scrollpane.viewport
			Dim vsb As JScrollBar = scrollpane.verticalScrollBar
			Dim hsb As JScrollBar = scrollpane.horizontalScrollBar

			If viewport IsNot Nothing Then viewport.addChangeListener(viewportChangeListener)
			If vsb IsNot Nothing Then
				vsb.model.addChangeListener(vsbChangeListener)
				vsb.addPropertyChangeListener(vsbPropertyChangeListener)
			End If
			If hsb IsNot Nothing Then
				hsb.model.addChangeListener(hsbChangeListener)
				hsb.addPropertyChangeListener(hsbPropertyChangeListener)
			End If

			scrollpane.addPropertyChangeListener(spPropertyChangeListener)

		mouseScrollListener = createMouseWheelListener()
		scrollpane.addMouseWheelListener(mouseScrollListener)

		End Sub

		Protected Friend Overridable Sub installKeyboardActions(ByVal c As JScrollPane)
			Dim ___inputMap As InputMap = getInputMap(JComponent.WHEN_ANCESTOR_OF_FOCUSED_COMPONENT)

			SwingUtilities.replaceUIInputMap(c, JComponent.WHEN_ANCESTOR_OF_FOCUSED_COMPONENT, ___inputMap)

			LazyActionMap.installLazyActionMap(c, GetType(BasicScrollPaneUI), "ScrollPane.actionMap")
		End Sub

		Friend Overridable Function getInputMap(ByVal condition As Integer) As InputMap
			If condition = JComponent.WHEN_ANCESTOR_OF_FOCUSED_COMPONENT Then
				Dim keyMap As InputMap = CType(sun.swing.DefaultLookup.get(scrollpane, Me, "ScrollPane.ancestorInputMap"), InputMap)
				Dim rtlKeyMap As InputMap

				rtlKeyMap = CType(sun.swing.DefaultLookup.get(scrollpane, Me, "ScrollPane.ancestorInputMap.RightToLeft"), InputMap)
				If scrollpane.componentOrientation.leftToRight OrElse (rtlKeyMap Is Nothing) Then
					Return keyMap
				Else
					rtlKeyMap.parent = keyMap
					Return rtlKeyMap
				End If
			End If
			Return Nothing
		End Function

		Public Overridable Sub installUI(ByVal x As JComponent)
			scrollpane = CType(x, JScrollPane)
			installDefaults(scrollpane)
			installListeners(scrollpane)
			installKeyboardActions(scrollpane)
		End Sub


		Protected Friend Overridable Sub uninstallDefaults(ByVal c As JScrollPane)
			LookAndFeel.uninstallBorder(scrollpane)

			If TypeOf scrollpane.viewportBorder Is UIResource Then scrollpane.viewportBorder = Nothing
		End Sub


		Protected Friend Overridable Sub uninstallListeners(ByVal c As JComponent)
			Dim viewport As JViewport = scrollpane.viewport
			Dim vsb As JScrollBar = scrollpane.verticalScrollBar
			Dim hsb As JScrollBar = scrollpane.horizontalScrollBar

			If viewport IsNot Nothing Then viewport.removeChangeListener(viewportChangeListener)
			If vsb IsNot Nothing Then
				vsb.model.removeChangeListener(vsbChangeListener)
				vsb.removePropertyChangeListener(vsbPropertyChangeListener)
			End If
			If hsb IsNot Nothing Then
				hsb.model.removeChangeListener(hsbChangeListener)
				hsb.removePropertyChangeListener(hsbPropertyChangeListener)
			End If

			scrollpane.removePropertyChangeListener(spPropertyChangeListener)

		If mouseScrollListener IsNot Nothing Then scrollpane.removeMouseWheelListener(mouseScrollListener)

			vsbChangeListener = Nothing
			hsbChangeListener = Nothing
			viewportChangeListener = Nothing
			spPropertyChangeListener = Nothing
			mouseScrollListener = Nothing
			handler = Nothing
		End Sub


		Protected Friend Overridable Sub uninstallKeyboardActions(ByVal c As JScrollPane)
			SwingUtilities.replaceUIActionMap(c, Nothing)
			SwingUtilities.replaceUIInputMap(c, JComponent.WHEN_ANCESTOR_OF_FOCUSED_COMPONENT, Nothing)
		End Sub


		Public Overridable Sub uninstallUI(ByVal c As JComponent)
			uninstallDefaults(scrollpane)
			uninstallListeners(scrollpane)
			uninstallKeyboardActions(scrollpane)
			scrollpane = Nothing
		End Sub

		Private Property handler As Handler
			Get
				If handler Is Nothing Then handler = New Handler(Me)
				Return handler
			End Get
		End Property

		Protected Friend Overridable Sub syncScrollPaneWithViewport()
			Dim viewport As JViewport = scrollpane.viewport
			Dim vsb As JScrollBar = scrollpane.verticalScrollBar
			Dim hsb As JScrollBar = scrollpane.horizontalScrollBar
			Dim rowHead As JViewport = scrollpane.rowHeader
			Dim colHead As JViewport = scrollpane.columnHeader
			Dim ltr As Boolean = scrollpane.componentOrientation.leftToRight

			If viewport IsNot Nothing Then
				Dim extentSize As java.awt.Dimension = viewport.extentSize
				Dim viewSize As java.awt.Dimension = viewport.viewSize
				Dim viewPosition As java.awt.Point = viewport.viewPosition

				If vsb IsNot Nothing Then
					Dim extent As Integer = extentSize.height
					Dim max As Integer = viewSize.height
					Dim value As Integer = Math.Max(0, Math.Min(viewPosition.y, max - extent))
					vsb.valuesues(value, extent, 0, max)
				End If

				If hsb IsNot Nothing Then
					Dim extent As Integer = extentSize.width
					Dim max As Integer = viewSize.width
					Dim value As Integer

					If ltr Then
						value = Math.Max(0, Math.Min(viewPosition.x, max - extent))
					Else
						Dim currentValue As Integer = hsb.value

	'                     Use a particular formula to calculate "value"
	'                     * until effective x coordinate is calculated.
	'                     
						If valueCalledled AndAlso ((max - currentValue) = viewPosition.x) Then
							value = Math.Max(0, Math.Min(max - extent, currentValue))
	'                         After "extent" is set, turn setValueCalled flag off.
	'                         
							If extent <> 0 Then valueCalledled = False
						Else
							If extent > max Then
								viewPosition.x = max - extent
								viewport.viewPosition = viewPosition
								value = 0
							Else
	'                            The following line can't handle a small value of
	'                            * viewPosition.x like Integer.MIN_VALUE correctly
	'                            * because (max - extent - viewPositoiin.x) causes
	'                            * an overflow. As a result, value becomes zero.
	'                            * (e.g. setViewPosition(Integer.MAX_VALUE, ...)
	'                            *       in a user program causes a overflow.
	'                            *       Its expected value is (max - extent).)
	'                            * However, this seems a trivial bug and adding a
	'                            * fix makes this often-called method slow, so I'll
	'                            * leave it until someone claims.
	'                            
								value = Math.Max(0, Math.Min(max - extent, max - extent - viewPosition.x))
								If oldExtent > extent Then value -= oldExtent - extent
							End If
						End If
					End If
					oldExtent = extent
					hsb.valuesues(value, extent, 0, max)
				End If

				If rowHead IsNot Nothing Then
					Dim p As java.awt.Point = rowHead.viewPosition
					p.y = viewport.viewPosition.y
					p.x = 0
					rowHead.viewPosition = p
				End If

				If colHead IsNot Nothing Then
					Dim p As java.awt.Point = colHead.viewPosition
					If ltr Then
						p.x = viewport.viewPosition.x
					Else
						p.x = Math.Max(0, viewport.viewPosition.x)
					End If
					p.y = 0
					colHead.viewPosition = p
				End If
			End If
		End Sub

		''' <summary>
		''' Returns the baseline.
		''' </summary>
		''' <exception cref="NullPointerException"> {@inheritDoc} </exception>
		''' <exception cref="IllegalArgumentException"> {@inheritDoc} </exception>
		''' <seealso cref= javax.swing.JComponent#getBaseline(int, int)
		''' @since 1.6 </seealso>
		Public Overridable Function getBaseline(ByVal c As JComponent, ByVal width As Integer, ByVal height As Integer) As Integer
			If c Is Nothing Then Throw New NullPointerException("Component must be non-null")

			If width < 0 OrElse height < 0 Then Throw New System.ArgumentException("Width and height must be >= 0")

			Dim viewport As JViewport = scrollpane.viewport
			Dim spInsets As java.awt.Insets = scrollpane.insets
			Dim y As Integer = spInsets.top
			height = height - spInsets.top - spInsets.bottom
			width = width - spInsets.left - spInsets.right
			Dim columnHeader As JViewport = scrollpane.columnHeader
			If columnHeader IsNot Nothing AndAlso columnHeader.visible Then
				Dim header As java.awt.Component = columnHeader.view
				If header IsNot Nothing AndAlso header.visible Then
					' Header is always given it's preferred size.
					Dim headerPref As java.awt.Dimension = header.preferredSize
					Dim ___baseline As Integer = header.getBaseline(headerPref.width, headerPref.height)
					If ___baseline >= 0 Then Return y + ___baseline
				End If
				Dim columnPref As java.awt.Dimension = columnHeader.preferredSize
				height -= columnPref.height
				y += columnPref.height
			End If
			Dim view As java.awt.Component = If(viewport Is Nothing, Nothing, viewport.view)
			If view IsNot Nothing AndAlso view.visible AndAlso view.baselineResizeBehavior = java.awt.Component.BaselineResizeBehavior.CONSTANT_ASCENT Then
				Dim viewportBorder As Border = scrollpane.viewportBorder
				If viewportBorder IsNot Nothing Then
					Dim vpbInsets As java.awt.Insets = viewportBorder.getBorderInsets(scrollpane)
					y += vpbInsets.top
					height = height - vpbInsets.top - vpbInsets.bottom
					width = width - vpbInsets.left - vpbInsets.right
				End If
				If view.width > 0 AndAlso view.height > 0 Then
					Dim min As java.awt.Dimension = view.minimumSize
					width = Math.Max(min.width, view.width)
					height = Math.Max(min.height, view.height)
				End If
				If width > 0 AndAlso height > 0 Then
					Dim ___baseline As Integer = view.getBaseline(width, height)
					If ___baseline > 0 Then Return y + ___baseline
				End If
			End If
			Return -1
		End Function

		''' <summary>
		''' Returns an enum indicating how the baseline of the component
		''' changes as the size changes.
		''' </summary>
		''' <exception cref="NullPointerException"> {@inheritDoc} </exception>
		''' <seealso cref= javax.swing.JComponent#getBaseline(int, int)
		''' @since 1.6 </seealso>
		Public Overridable Function getBaselineResizeBehavior(ByVal c As JComponent) As java.awt.Component.BaselineResizeBehavior
			MyBase.getBaselineResizeBehavior(c)
			' Baseline is either from the header, in which case it's always
			' the same size and therefor can be created as CONSTANT_ASCENT.
			' If the header doesn't have a baseline than the baseline will only
			' be valid if it's BaselineResizeBehavior is
			' CONSTANT_ASCENT, so, return CONSTANT_ASCENT.
			Return java.awt.Component.BaselineResizeBehavior.CONSTANT_ASCENT
		End Function


		''' <summary>
		''' Listener for viewport events.
		''' </summary>
		Public Class ViewportChangeHandler
			Implements ChangeListener

			Private ReadOnly outerInstance As BasicScrollPaneUI

			Public Sub New(ByVal outerInstance As BasicScrollPaneUI)
				Me.outerInstance = outerInstance
			End Sub


			' NOTE: This class exists only for backward compatibility. All
			' its functionality has been moved into Handler. If you need to add
			' new functionality add it to the Handler, but make sure this
			' class calls into the Handler.

			Public Overridable Sub stateChanged(ByVal e As ChangeEvent) Implements ChangeListener.stateChanged
				outerInstance.handler.stateChanged(e)
			End Sub
		End Class

		Protected Friend Overridable Function createViewportChangeListener() As ChangeListener
			Return handler
		End Function


		''' <summary>
		''' Horizontal scrollbar listener.
		''' </summary>
		Public Class HSBChangeListener
			Implements ChangeListener

			Private ReadOnly outerInstance As BasicScrollPaneUI

			Public Sub New(ByVal outerInstance As BasicScrollPaneUI)
				Me.outerInstance = outerInstance
			End Sub


			' NOTE: This class exists only for backward compatibility. All
			' its functionality has been moved into Handler. If you need to add
			' new functionality add it to the Handler, but make sure this
			' class calls into the Handler.

			Public Overridable Sub stateChanged(ByVal e As ChangeEvent) Implements ChangeListener.stateChanged
				outerInstance.handler.stateChanged(e)
			End Sub
		End Class

		''' <summary>
		''' Returns a <code>PropertyChangeListener</code> that will be installed
		''' on the horizontal <code>JScrollBar</code>.
		''' </summary>
		Private Function createHSBPropertyChangeListener() As java.beans.PropertyChangeListener
			Return handler
		End Function

		Protected Friend Overridable Function createHSBChangeListener() As ChangeListener
			Return handler
		End Function


		''' <summary>
		''' Vertical scrollbar listener.
		''' </summary>
		Public Class VSBChangeListener
			Implements ChangeListener

			Private ReadOnly outerInstance As BasicScrollPaneUI

			Public Sub New(ByVal outerInstance As BasicScrollPaneUI)
				Me.outerInstance = outerInstance
			End Sub


			' NOTE: This class exists only for backward compatibility. All
			' its functionality has been moved into Handler. If you need to add
			' new functionality add it to the Handler, but make sure this
			' class calls into the Handler.

			Public Overridable Sub stateChanged(ByVal e As ChangeEvent) Implements ChangeListener.stateChanged
				outerInstance.handler.stateChanged(e)
			End Sub
		End Class


		''' <summary>
		''' Returns a <code>PropertyChangeListener</code> that will be installed
		''' on the vertical <code>JScrollBar</code>.
		''' </summary>
		Private Function createVSBPropertyChangeListener() As java.beans.PropertyChangeListener
			Return handler
		End Function

		Protected Friend Overridable Function createVSBChangeListener() As ChangeListener
			Return handler
		End Function

		''' <summary>
		''' MouseWheelHandler is an inner class which implements the
		''' MouseWheelListener interface.  MouseWheelHandler responds to
		''' MouseWheelEvents by scrolling the JScrollPane appropriately.
		''' If the scroll pane's
		''' <code>isWheelScrollingEnabled</code>
		''' method returns false, no scrolling occurs.
		''' </summary>
		''' <seealso cref= javax.swing.JScrollPane#isWheelScrollingEnabled </seealso>
		''' <seealso cref= #createMouseWheelListener </seealso>
		''' <seealso cref= java.awt.event.MouseWheelListener </seealso>
		''' <seealso cref= java.awt.event.MouseWheelEvent
		''' @since 1.4 </seealso>
		Protected Friend Class MouseWheelHandler
			Implements MouseWheelListener

			Private ReadOnly outerInstance As BasicScrollPaneUI

			Public Sub New(ByVal outerInstance As BasicScrollPaneUI)
				Me.outerInstance = outerInstance
			End Sub


			' NOTE: This class exists only for backward compatibility. All
			' its functionality has been moved into Handler. If you need to add
			' new functionality add it to the Handler, but make sure this
			' class calls into the Handler.

			''' <summary>
			''' Called when the mouse wheel is rotated while over a
			''' JScrollPane.
			''' </summary>
			''' <param name="e">     MouseWheelEvent to be handled
			''' @since 1.4 </param>
			Public Overridable Sub mouseWheelMoved(ByVal e As MouseWheelEvent)
				outerInstance.handler.mouseWheelMoved(e)
			End Sub
		End Class

		''' <summary>
		''' Creates an instance of MouseWheelListener, which is added to the
		''' JScrollPane by installUI().  The returned MouseWheelListener is used
		''' to handle mouse wheel-driven scrolling.
		''' </summary>
		''' <returns>      MouseWheelListener which implements wheel-driven scrolling </returns>
		''' <seealso cref= #installUI </seealso>
		''' <seealso cref= MouseWheelHandler
		''' @since 1.4 </seealso>
		Protected Friend Overridable Function createMouseWheelListener() As MouseWheelListener
			Return handler
		End Function

		Protected Friend Overridable Sub updateScrollBarDisplayPolicy(ByVal e As java.beans.PropertyChangeEvent)
			scrollpane.revalidate()
			scrollpane.repaint()
		End Sub


		Protected Friend Overridable Sub updateViewport(ByVal e As java.beans.PropertyChangeEvent)
			Dim oldViewport As JViewport = CType(e.oldValue, JViewport)
			Dim newViewport As JViewport = CType(e.newValue, JViewport)

			If oldViewport IsNot Nothing Then oldViewport.removeChangeListener(viewportChangeListener)

			If newViewport IsNot Nothing Then
				Dim p As java.awt.Point = newViewport.viewPosition
				If scrollpane.componentOrientation.leftToRight Then
					p.x = Math.Max(p.x, 0)
				Else
					Dim max As Integer = newViewport.viewSize.width
					Dim extent As Integer = newViewport.extentSize.width
					If extent > max Then
						p.x = max - extent
					Else
						p.x = Math.Max(0, Math.Min(max - extent, p.x))
					End If
				End If
				p.y = Math.Max(p.y, 0)
				newViewport.viewPosition = p
				newViewport.addChangeListener(viewportChangeListener)
			End If
		End Sub


		Protected Friend Overridable Sub updateRowHeader(ByVal e As java.beans.PropertyChangeEvent)
			Dim newRowHead As JViewport = CType(e.newValue, JViewport)
			If newRowHead IsNot Nothing Then
				Dim viewport As JViewport = scrollpane.viewport
				Dim p As java.awt.Point = newRowHead.viewPosition
				p.y = If(viewport IsNot Nothing, viewport.viewPosition.y, 0)
				newRowHead.viewPosition = p
			End If
		End Sub


		Protected Friend Overridable Sub updateColumnHeader(ByVal e As java.beans.PropertyChangeEvent)
			Dim newColHead As JViewport = CType(e.newValue, JViewport)
			If newColHead IsNot Nothing Then
				Dim viewport As JViewport = scrollpane.viewport
				Dim p As java.awt.Point = newColHead.viewPosition
				If viewport Is Nothing Then
					p.x = 0
				Else
					If scrollpane.componentOrientation.leftToRight Then
						p.x = viewport.viewPosition.x
					Else
						p.x = Math.Max(0, viewport.viewPosition.x)
					End If
				End If
				newColHead.viewPosition = p
				scrollpane.add(newColHead, COLUMN_HEADER)
			End If
		End Sub

		Private Sub updateHorizontalScrollBar(ByVal pce As java.beans.PropertyChangeEvent)
			updateScrollBar(pce, hsbChangeListener, hsbPropertyChangeListener)
		End Sub

		Private Sub updateVerticalScrollBar(ByVal pce As java.beans.PropertyChangeEvent)
			updateScrollBar(pce, vsbChangeListener, vsbPropertyChangeListener)
		End Sub

		Private Sub updateScrollBar(ByVal pce As java.beans.PropertyChangeEvent, ByVal cl As ChangeListener, ByVal pcl As java.beans.PropertyChangeListener)
			Dim sb As JScrollBar = CType(pce.oldValue, JScrollBar)
			If sb IsNot Nothing Then
				If cl IsNot Nothing Then sb.model.removeChangeListener(cl)
				If pcl IsNot Nothing Then sb.removePropertyChangeListener(pcl)
			End If
			sb = CType(pce.newValue, JScrollBar)
			If sb IsNot Nothing Then
				If cl IsNot Nothing Then sb.model.addChangeListener(cl)
				If pcl IsNot Nothing Then sb.addPropertyChangeListener(pcl)
			End If
		End Sub

		Public Class PropertyChangeHandler
			Implements java.beans.PropertyChangeListener

			Private ReadOnly outerInstance As BasicScrollPaneUI

			Public Sub New(ByVal outerInstance As BasicScrollPaneUI)
				Me.outerInstance = outerInstance
			End Sub


			' NOTE: This class exists only for backward compatibility. All
			' its functionality has been moved into Handler. If you need to add
			' new functionality add it to the Handler, but make sure this
			' class calls into the Handler.

			Public Overridable Sub propertyChange(ByVal e As java.beans.PropertyChangeEvent)
				outerInstance.handler.propertyChange(e)
			End Sub
		End Class



		''' <summary>
		''' Creates an instance of PropertyChangeListener that's added to
		''' the JScrollPane by installUI().  Subclasses can override this method
		''' to return a custom PropertyChangeListener, e.g.
		''' <pre>
		''' class MyScrollPaneUI extends BasicScrollPaneUI {
		'''    protected PropertyChangeListener <b>createPropertyChangeListener</b>() {
		'''        return new MyPropertyChangeListener();
		'''    }
		'''    public class MyPropertyChangeListener extends PropertyChangeListener {
		'''        public void propertyChange(PropertyChangeEvent e) {
		'''            if (e.getPropertyName().equals("viewport")) {
		'''                // do some extra work when the viewport changes
		'''            }
		'''            super.propertyChange(e);
		'''        }
		'''    }
		''' }
		''' </pre>
		''' </summary>
		''' <seealso cref= java.beans.PropertyChangeListener </seealso>
		''' <seealso cref= #installUI </seealso>
		Protected Friend Overridable Function createPropertyChangeListener() As java.beans.PropertyChangeListener
			Return handler
		End Function


		Private Class Actions
			Inherits sun.swing.UIAction

			Private Const SCROLL_UP As String = "scrollUp"
			Private Const SCROLL_DOWN As String = "scrollDown"
			Private Const SCROLL_HOME As String = "scrollHome"
			Private Const SCROLL_END As String = "scrollEnd"
			Private Const UNIT_SCROLL_UP As String = "unitScrollUp"
			Private Const UNIT_SCROLL_DOWN As String = "unitScrollDown"
			Private Const SCROLL_LEFT As String = "scrollLeft"
			Private Const SCROLL_RIGHT As String = "scrollRight"
			Private Const UNIT_SCROLL_LEFT As String = "unitScrollLeft"
			Private Const UNIT_SCROLL_RIGHT As String = "unitScrollRight"


			Friend Sub New(ByVal key As String)
				MyBase.New(key)
			End Sub

			Public Overridable Sub actionPerformed(ByVal e As ActionEvent)
				Dim scrollPane As JScrollPane = CType(e.source, JScrollPane)
				Dim ltr As Boolean = scrollPane.componentOrientation.leftToRight
				Dim key As String = name

				If key = SCROLL_UP Then
					scroll(scrollPane, SwingConstants.VERTICAL, -1, True)
				ElseIf key = SCROLL_DOWN Then
					scroll(scrollPane, SwingConstants.VERTICAL, 1, True)
				ElseIf key = SCROLL_HOME Then
					scrollHome(scrollPane)
				ElseIf key = SCROLL_END Then
					scrollEnd(scrollPane)
				ElseIf key = UNIT_SCROLL_UP Then
					scroll(scrollPane, SwingConstants.VERTICAL, -1, False)
				ElseIf key = UNIT_SCROLL_DOWN Then
					scroll(scrollPane, SwingConstants.VERTICAL, 1, False)
				ElseIf key = SCROLL_LEFT Then
					scroll(scrollPane, SwingConstants.HORIZONTAL,If(ltr, -1, 1), True)
				ElseIf key = SCROLL_RIGHT Then
					scroll(scrollPane, SwingConstants.HORIZONTAL,If(ltr, 1, -1), True)
				ElseIf key = UNIT_SCROLL_LEFT Then
					scroll(scrollPane, SwingConstants.HORIZONTAL,If(ltr, -1, 1), False)
				ElseIf key = UNIT_SCROLL_RIGHT Then
					scroll(scrollPane, SwingConstants.HORIZONTAL,If(ltr, 1, -1), False)
				End If
			End Sub

			Private Sub scrollEnd(ByVal scrollpane As JScrollPane)
				Dim vp As JViewport = scrollpane.viewport
				Dim view As java.awt.Component
				view = vp.view
				If vp IsNot Nothing AndAlso view IsNot Nothing Then
					Dim visRect As java.awt.Rectangle = vp.viewRect
					Dim bounds As java.awt.Rectangle = view.bounds
					If scrollpane.componentOrientation.leftToRight Then
						vp.viewPosition = New java.awt.Point(bounds.width - visRect.width, bounds.height - visRect.height)
					Else
						vp.viewPosition = New java.awt.Point(0, bounds.height - visRect.height)
					End If
				End If
			End Sub

			Private Sub scrollHome(ByVal scrollpane As JScrollPane)
				Dim vp As JViewport = scrollpane.viewport
				Dim view As java.awt.Component
				view = vp.view
				If vp IsNot Nothing AndAlso view IsNot Nothing Then
					If scrollpane.componentOrientation.leftToRight Then
						vp.viewPosition = New java.awt.Point(0, 0)
					Else
						Dim visRect As java.awt.Rectangle = vp.viewRect
						Dim bounds As java.awt.Rectangle = view.bounds
						vp.viewPosition = New java.awt.Point(bounds.width - visRect.width, 0)
					End If
				End If
			End Sub

			Private Sub scroll(ByVal scrollpane As JScrollPane, ByVal orientation As Integer, ByVal direction As Integer, ByVal block As Boolean)
				Dim vp As JViewport = scrollpane.viewport
				Dim view As java.awt.Component
				view = vp.view
				If vp IsNot Nothing AndAlso view IsNot Nothing Then
					Dim visRect As java.awt.Rectangle = vp.viewRect
					Dim vSize As java.awt.Dimension = view.size
					Dim amount As Integer

					If TypeOf view Is Scrollable Then
						If block Then
							amount = CType(view, Scrollable).getScrollableBlockIncrement(visRect, orientation, direction)
						Else
							amount = CType(view, Scrollable).getScrollableUnitIncrement(visRect, orientation, direction)
						End If
					Else
						If block Then
							If orientation = SwingConstants.VERTICAL Then
								amount = visRect.height
							Else
								amount = visRect.width
							End If
						Else
							amount = 10
						End If
					End If
					If orientation = SwingConstants.VERTICAL Then
						visRect.y += (amount * direction)
						If (visRect.y + visRect.height) > vSize.height Then
							visRect.y = Math.Max(0, vSize.height - visRect.height)
						ElseIf visRect.y < 0 Then
							visRect.y = 0
						End If
					Else
						If scrollpane.componentOrientation.leftToRight Then
							visRect.x += (amount * direction)
							If (visRect.x + visRect.width) > vSize.width Then
								visRect.x = Math.Max(0, vSize.width - visRect.width)
							ElseIf visRect.x < 0 Then
								visRect.x = 0
							End If
						Else
							visRect.x -= (amount * direction)
							If visRect.width > vSize.width Then
								visRect.x = vSize.width - visRect.width
							Else
								visRect.x = Math.Max(0, Math.Min(vSize.width - visRect.width, visRect.x))
							End If
						End If
					End If
					vp.viewPosition = visRect.location
				End If
			End Sub
		End Class


		Friend Class Handler
			Implements ChangeListener, java.beans.PropertyChangeListener, MouseWheelListener

			Private ReadOnly outerInstance As BasicScrollPaneUI

			Public Sub New(ByVal outerInstance As BasicScrollPaneUI)
				Me.outerInstance = outerInstance
			End Sub

			'
			' MouseWheelListener
			'
			Public Overridable Sub mouseWheelMoved(ByVal e As MouseWheelEvent)
				If outerInstance.scrollpane.wheelScrollingEnabled AndAlso e.wheelRotation <> 0 Then
					Dim toScroll As JScrollBar = outerInstance.scrollpane.verticalScrollBar
					Dim direction As Integer = If(e.wheelRotation < 0, -1, 1)
					Dim orientation As Integer = SwingConstants.VERTICAL

					' find which scrollbar to scroll, or return if none
					If toScroll Is Nothing OrElse (Not toScroll.visible) Then
						toScroll = outerInstance.scrollpane.horizontalScrollBar
						If toScroll Is Nothing OrElse (Not toScroll.visible) Then Return
						orientation = SwingConstants.HORIZONTAL
					ElseIf e.shiftDown Then
						Dim hScroll As JScrollBar = outerInstance.scrollpane.horizontalScrollBar
						If hScroll IsNot Nothing AndAlso hScroll.visible Then
							toScroll = hScroll
							orientation = SwingConstants.HORIZONTAL
						End If
					End If

					e.consume()

					If e.scrollType = MouseWheelEvent.WHEEL_UNIT_SCROLL Then
						Dim vp As JViewport = outerInstance.scrollpane.viewport
						If vp Is Nothing Then Return
						Dim comp As java.awt.Component = vp.view
						Dim units As Integer = Math.Abs(e.unitsToScroll)

						' When the scrolling speed is set to maximum, it's possible
						' for a single wheel click to scroll by more units than
						' will fit in the visible area.  This makes it
						' hard/impossible to get to certain parts of the scrolling
						' Component with the wheel.  To make for more accurate
						' low-speed scrolling, we limit scrolling to the block
						' increment if the wheel was only rotated one click.
						Dim limitScroll As Boolean = Math.Abs(e.wheelRotation) = 1

						' Check if we should use the visibleRect trick
						Dim fastWheelScroll As Object = toScroll.getClientProperty("JScrollBar.fastWheelScrolling")
						If Boolean.TRUE Is fastWheelScroll AndAlso TypeOf comp Is Scrollable Then
							' 5078454: Under maximum acceleration, we may scroll
							' by many 100s of units in ~1 second.
							'
							' BasicScrollBarUI.scrollByUnits() can bog down the EDT
							' with repaints in this situation.  However, the
							' Scrollable interface allows us to pass in an
							' arbitrary visibleRect.  This allows us to accurately
							' calculate the total scroll amount, and then update
							' the GUI once.  This technique provides much faster
							' accelerated wheel scrolling.
							Dim scrollComp As Scrollable = CType(comp, Scrollable)
							Dim viewRect As java.awt.Rectangle = vp.viewRect
							Dim startingX As Integer = viewRect.x
							Dim leftToRight As Boolean = comp.componentOrientation.leftToRight
							Dim scrollMin As Integer = toScroll.minimum
							Dim scrollMax As Integer = toScroll.maximum - toScroll.model.extent

							If limitScroll Then
								Dim blockIncr As Integer = scrollComp.getScrollableBlockIncrement(viewRect, orientation, direction)
								If direction < 0 Then
									scrollMin = Math.Max(scrollMin, toScroll.value - blockIncr)
								Else
									scrollMax = Math.Min(scrollMax, toScroll.value + blockIncr)
								End If
							End If

							For i As Integer = 0 To units - 1
								Dim unitIncr As Integer = scrollComp.getScrollableUnitIncrement(viewRect, orientation, direction)
								' Modify the visible rect for the next unit, and
								' check to see if we're at the end already.
								If orientation = SwingConstants.VERTICAL Then
									If direction < 0 Then
										viewRect.y -= unitIncr
										If viewRect.y <= scrollMin Then
											viewRect.y = scrollMin
											Exit For
										End If
									Else ' (direction > 0
										viewRect.y += unitIncr
										If viewRect.y >= scrollMax Then
											viewRect.y = scrollMax
											Exit For
										End If
									End If
								Else
									' Scroll left
									If (leftToRight AndAlso direction < 0) OrElse ((Not leftToRight) AndAlso direction > 0) Then
										viewRect.x -= unitIncr
										If leftToRight Then
											If viewRect.x < scrollMin Then
												viewRect.x = scrollMin
												Exit For
											End If
										End If
									' Scroll right
									ElseIf (leftToRight AndAlso direction > 0) OrElse ((Not leftToRight) AndAlso direction < 0) Then
										viewRect.x += unitIncr
										If leftToRight Then
											If viewRect.x > scrollMax Then
												viewRect.x = scrollMax
												Exit For
											End If
										End If
									Else
										Debug.Assert(False, "Non-sensical ComponentOrientation / scroll direction")
									End If
								End If
							Next i
							' Set the final view position on the ScrollBar
							If orientation = SwingConstants.VERTICAL Then
								toScroll.value = viewRect.y
							Else
								If leftToRight Then
									toScroll.value = viewRect.x
								Else
									' rightToLeft scrollbars are oriented with
									' minValue on the right and maxValue on the
									' left.
									Dim newPos As Integer = toScroll.value - (viewRect.x - startingX)
									If newPos < scrollMin Then
										newPos = scrollMin
									ElseIf newPos > scrollMax Then
										newPos = scrollMax
									End If
									toScroll.value = newPos
								End If
							End If
						Else
							' Viewport's view is not a Scrollable, or fast wheel
							' scrolling is not enabled.
							BasicScrollBarUI.scrollByUnits(toScroll, direction, units, limitScroll)
						End If
					ElseIf e.scrollType = MouseWheelEvent.WHEEL_BLOCK_SCROLL Then
						BasicScrollBarUI.scrollByBlock(toScroll, direction)
					End If
				End If
			End Sub

			'
			' ChangeListener: This is added to the vieport, and hsb/vsb models.
			'
			Public Overridable Sub stateChanged(ByVal e As ChangeEvent) Implements ChangeListener.stateChanged
				Dim viewport As JViewport = outerInstance.scrollpane.viewport

				If viewport IsNot Nothing Then
					If e.source Is viewport Then
						outerInstance.syncScrollPaneWithViewport()
					Else
						Dim hsb As JScrollBar = outerInstance.scrollpane.horizontalScrollBar
						If hsb IsNot Nothing AndAlso e.source Is hsb.model Then
							hsbStateChanged(viewport, e)
						Else
							Dim vsb As JScrollBar = outerInstance.scrollpane.verticalScrollBar
							If vsb IsNot Nothing AndAlso e.source Is vsb.model Then vsbStateChanged(viewport, e)
						End If
					End If
				End If
			End Sub

			Private Sub vsbStateChanged(ByVal viewport As JViewport, ByVal e As ChangeEvent)
				Dim model As BoundedRangeModel = CType(e.source, BoundedRangeModel)
				Dim p As java.awt.Point = viewport.viewPosition
				p.y = model.value
				viewport.viewPosition = p
			End Sub

			Private Sub hsbStateChanged(ByVal viewport As JViewport, ByVal e As ChangeEvent)
				Dim model As BoundedRangeModel = CType(e.source, BoundedRangeModel)
				Dim p As java.awt.Point = viewport.viewPosition
				Dim value As Integer = model.value
				If outerInstance.scrollpane.componentOrientation.leftToRight Then
					p.x = value
				Else
					Dim max As Integer = viewport.viewSize.width
					Dim extent As Integer = viewport.extentSize.width
					Dim oldX As Integer = p.x

	'                 Set new X coordinate based on "value".
	'                 
					p.x = max - extent - value

	'                 If setValue() was called before "extent" was fixed,
	'                 * turn setValueCalled flag on.
	'                 
					If (extent = 0) AndAlso (value <> 0) AndAlso (oldX = max) Then
						outerInstance.valueCalledled = True
					Else
	'                     When a pane without a horizontal scroll bar was
	'                     * reduced and the bar appeared, the viewport should
	'                     * show the right side of the view.
	'                     
						If (extent <> 0) AndAlso (oldX < 0) AndAlso (p.x = 0) Then p.x += value
					End If
				End If
				viewport.viewPosition = p
			End Sub

			'
			' PropertyChangeListener: This is installed on both the JScrollPane
			' and the horizontal/vertical scrollbars.
			'

			' Listens for changes in the model property and reinstalls the
			' horizontal/vertical PropertyChangeListeners.
			Public Overridable Sub propertyChange(ByVal e As java.beans.PropertyChangeEvent)
				If e.source Is outerInstance.scrollpane Then
					scrollPanePropertyChange(e)
				Else
					sbPropertyChange(e)
				End If
			End Sub

			Private Sub scrollPanePropertyChange(ByVal e As java.beans.PropertyChangeEvent)
				Dim propertyName As String = e.propertyName

				If propertyName = "verticalScrollBarDisplayPolicy" Then
					outerInstance.updateScrollBarDisplayPolicy(e)
				ElseIf propertyName = "horizontalScrollBarDisplayPolicy" Then
					outerInstance.updateScrollBarDisplayPolicy(e)
				ElseIf propertyName = "viewport" Then
					outerInstance.updateViewport(e)
				ElseIf propertyName = "rowHeader" Then
					outerInstance.updateRowHeader(e)
				ElseIf propertyName = "columnHeader" Then
					outerInstance.updateColumnHeader(e)
				ElseIf propertyName = "verticalScrollBar" Then
					outerInstance.updateVerticalScrollBar(e)
				ElseIf propertyName = "horizontalScrollBar" Then
					outerInstance.updateHorizontalScrollBar(e)
				ElseIf propertyName = "componentOrientation" Then
					outerInstance.scrollpane.revalidate()
					outerInstance.scrollpane.repaint()
				End If
			End Sub

			' PropertyChangeListener for the horizontal and vertical scrollbars.
			Private Sub sbPropertyChange(ByVal e As java.beans.PropertyChangeEvent)
				Dim propertyName As String = e.propertyName
				Dim source As Object = e.source

				If "model" = propertyName Then
					Dim sb As JScrollBar = outerInstance.scrollpane.verticalScrollBar
					Dim oldModel As BoundedRangeModel = CType(e.oldValue, BoundedRangeModel)
					Dim cl As ChangeListener = Nothing

					If source Is sb Then
						cl = outerInstance.vsbChangeListener
					ElseIf source Is outerInstance.scrollpane.horizontalScrollBar Then
						sb = outerInstance.scrollpane.horizontalScrollBar
						cl = outerInstance.hsbChangeListener
					End If
					If cl IsNot Nothing Then
						If oldModel IsNot Nothing Then oldModel.removeChangeListener(cl)
						If sb.model IsNot Nothing Then sb.model.addChangeListener(cl)
					End If
				ElseIf "componentOrientation" = propertyName Then
					If source Is outerInstance.scrollpane.horizontalScrollBar Then
						Dim hsb As JScrollBar = outerInstance.scrollpane.horizontalScrollBar
						Dim viewport As JViewport = outerInstance.scrollpane.viewport
						Dim p As java.awt.Point = viewport.viewPosition
						If outerInstance.scrollpane.componentOrientation.leftToRight Then
							p.x = hsb.value
						Else
							p.x = viewport.viewSize.width - viewport.extentSize.width - hsb.value
						End If
						viewport.viewPosition = p
					End If
				End If
			End Sub
		End Class
	End Class

End Namespace