Imports System
Imports javax.swing
Imports javax.swing.plaf

'
' * Copyright (c) 1997, 2015, Oracle and/or its affiliates. All rights reserved.
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
	''' Basic L&amp;F for a desktop.
	''' 
	''' @author Steve Wilson
	''' </summary>
	Public Class BasicDesktopPaneUI
		Inherits DesktopPaneUI

		' Old actions forward to an instance of this.
		Private Shared ReadOnly SHARED_ACTION As New Actions
		Private handler As Handler
		Private pcl As PropertyChangeListener

		Protected Friend desktop As JDesktopPane
		Protected Friend desktopManager As DesktopManager

		''' <summary>
		''' As of Java 2 platform v1.3 this previously undocumented field is no
		''' longer used.
		''' Key bindings are now defined by the LookAndFeel, please refer to
		''' the key bindings specification for further details.
		''' </summary>
		''' @deprecated As of 1.3. 
		<Obsolete("As of 1.3.")> _
		Protected Friend minimizeKey As KeyStroke
		''' <summary>
		''' As of Java 2 platform v1.3 this previously undocumented field is no
		''' longer used.
		''' Key bindings are now defined by the LookAndFeel, please refer to
		''' the key bindings specification for further details.
		''' </summary>
		''' @deprecated As of 1.3. 
		<Obsolete("As of 1.3.")> _
		Protected Friend maximizeKey As KeyStroke
		''' <summary>
		''' As of Java 2 platform v1.3 this previously undocumented field is no
		''' longer used.
		''' Key bindings are now defined by the LookAndFeel, please refer to
		''' the key bindings specification for further details.
		''' </summary>
		''' @deprecated As of 1.3. 
		<Obsolete("As of 1.3.")> _
		Protected Friend closeKey As KeyStroke
		''' <summary>
		''' As of Java 2 platform v1.3 this previously undocumented field is no
		''' longer used.
		''' Key bindings are now defined by the LookAndFeel, please refer to
		''' the key bindings specification for further details.
		''' </summary>
		''' @deprecated As of 1.3. 
		<Obsolete("As of 1.3.")> _
		Protected Friend navigateKey As KeyStroke
		''' <summary>
		''' As of Java 2 platform v1.3 this previously undocumented field is no
		''' longer used.
		''' Key bindings are now defined by the LookAndFeel, please refer to
		''' the key bindings specification for further details.
		''' </summary>
		''' @deprecated As of 1.3. 
		<Obsolete("As of 1.3.")> _
		Protected Friend navigateKey2 As KeyStroke

		Public Shared Function createUI(ByVal c As JComponent) As ComponentUI
			Return New BasicDesktopPaneUI
		End Function

		Public Sub New()
		End Sub

		Public Overridable Sub installUI(ByVal c As JComponent)
			desktop = CType(c, JDesktopPane)
			installDefaults()
			installDesktopManager()
			installListeners()
			installKeyboardActions()
		End Sub

		Public Overridable Sub uninstallUI(ByVal c As JComponent)
			uninstallKeyboardActions()
			uninstallListeners()
			uninstallDesktopManager()
			uninstallDefaults()
			desktop = Nothing
			handler = Nothing
		End Sub

		Protected Friend Overridable Sub installDefaults()
			If desktop.background Is Nothing OrElse TypeOf desktop.background Is UIResource Then desktop.background = UIManager.getColor("Desktop.background")
			LookAndFeel.installProperty(desktop, "opaque", Boolean.TRUE)
		End Sub

		Protected Friend Overridable Sub uninstallDefaults()
		End Sub

		''' <summary>
		''' Installs the <code>PropertyChangeListener</code> returned from
		''' <code>createPropertyChangeListener</code> on the
		''' <code>JDesktopPane</code>.
		''' 
		''' @since 1.5 </summary>
		''' <seealso cref= #createPropertyChangeListener </seealso>
		Protected Friend Overridable Sub installListeners()
			pcl = createPropertyChangeListener()
			desktop.addPropertyChangeListener(pcl)
		End Sub

		''' <summary>
		''' Uninstalls the <code>PropertyChangeListener</code> returned from
		''' <code>createPropertyChangeListener</code> from the
		''' <code>JDesktopPane</code>.
		''' 
		''' @since 1.5 </summary>
		''' <seealso cref= #createPropertyChangeListener </seealso>
		Protected Friend Overridable Sub uninstallListeners()
			desktop.removePropertyChangeListener(pcl)
			pcl = Nothing
		End Sub

		Protected Friend Overridable Sub installDesktopManager()
			desktopManager = desktop.desktopManager
			If desktopManager Is Nothing Then
				desktopManager = New BasicDesktopManager(Me)
				desktop.desktopManager = desktopManager
			End If
		End Sub

		Protected Friend Overridable Sub uninstallDesktopManager()
			If TypeOf desktop.desktopManager Is UIResource Then desktop.desktopManager = Nothing
			desktopManager = Nothing
		End Sub

		Protected Friend Overridable Sub installKeyboardActions()
			Dim ___inputMap As InputMap = getInputMap(JComponent.WHEN_IN_FOCUSED_WINDOW)
			If ___inputMap IsNot Nothing Then SwingUtilities.replaceUIInputMap(desktop, JComponent.WHEN_IN_FOCUSED_WINDOW, ___inputMap)
			___inputMap = getInputMap(JComponent.WHEN_ANCESTOR_OF_FOCUSED_COMPONENT)
			If ___inputMap IsNot Nothing Then SwingUtilities.replaceUIInputMap(desktop, JComponent.WHEN_ANCESTOR_OF_FOCUSED_COMPONENT, ___inputMap)

			LazyActionMap.installLazyActionMap(desktop, GetType(BasicDesktopPaneUI), "DesktopPane.actionMap")
			registerKeyboardActions()
		End Sub

		Protected Friend Overridable Sub registerKeyboardActions()
		End Sub

		Protected Friend Overridable Sub unregisterKeyboardActions()
		End Sub

		Friend Overridable Function getInputMap(ByVal condition As Integer) As InputMap
			If condition = JComponent.WHEN_IN_FOCUSED_WINDOW Then
				Return createInputMap(condition)
			ElseIf condition = JComponent.WHEN_ANCESTOR_OF_FOCUSED_COMPONENT Then
				Return CType(sun.swing.DefaultLookup.get(desktop, Me, "Desktop.ancestorInputMap"), InputMap)
			End If
			Return Nothing
		End Function

		Friend Overridable Function createInputMap(ByVal condition As Integer) As InputMap
			If condition = JComponent.WHEN_IN_FOCUSED_WINDOW Then
				Dim bindings As Object() = CType(sun.swing.DefaultLookup.get(desktop, Me, "Desktop.windowBindings"), Object())

				If bindings IsNot Nothing Then Return LookAndFeel.makeComponentInputMap(desktop, bindings)
			End If
			Return Nothing
		End Function

		Friend Shared Sub loadActionMap(ByVal map As LazyActionMap)
			map.put(New Actions(Actions.RESTORE))
			map.put(New Actions(Actions.CLOSE))
			map.put(New Actions(Actions.MOVE))
			map.put(New Actions(Actions.RESIZE))
			map.put(New Actions(Actions.LEFT))
			map.put(New Actions(Actions.SHRINK_LEFT))
			map.put(New Actions(Actions.RIGHT))
			map.put(New Actions(Actions.SHRINK_RIGHT))
			map.put(New Actions(Actions.UP))
			map.put(New Actions(Actions.SHRINK_UP))
			map.put(New Actions(Actions.DOWN))
			map.put(New Actions(Actions.SHRINK_DOWN))
			map.put(New Actions(Actions.ESCAPE))
			map.put(New Actions(Actions.MINIMIZE))
			map.put(New Actions(Actions.MAXIMIZE))
			map.put(New Actions(Actions.NEXT_FRAME))
			map.put(New Actions(Actions.PREVIOUS_FRAME))
			map.put(New Actions(Actions.NAVIGATE_NEXT))
			map.put(New Actions(Actions.NAVIGATE_PREVIOUS))
		End Sub

		Protected Friend Overridable Sub uninstallKeyboardActions()
		  unregisterKeyboardActions()
		  SwingUtilities.replaceUIInputMap(desktop, JComponent.WHEN_IN_FOCUSED_WINDOW, Nothing)
		  SwingUtilities.replaceUIInputMap(desktop, JComponent.WHEN_ANCESTOR_OF_FOCUSED_COMPONENT, Nothing)
		  SwingUtilities.replaceUIActionMap(desktop, Nothing)
		End Sub

		Public Overridable Sub paint(ByVal g As java.awt.Graphics, ByVal c As JComponent)
		End Sub

		Public Overrides Function getPreferredSize(ByVal c As JComponent) As java.awt.Dimension
			Return Nothing
		End Function

		Public Overrides Function getMinimumSize(ByVal c As JComponent) As java.awt.Dimension
			Return New java.awt.Dimension(0, 0)
		End Function

		Public Overrides Function getMaximumSize(ByVal c As JComponent) As java.awt.Dimension
			Return New java.awt.Dimension(Integer.MaxValue, Integer.MaxValue)
		End Function

		''' <summary>
		''' Returns the <code>PropertyChangeListener</code> to install on
		''' the <code>JDesktopPane</code>.
		''' 
		''' @since 1.5 </summary>
		''' <returns> The PropertyChangeListener that will be added to track
		''' changes in the desktop pane. </returns>
		Protected Friend Overridable Function createPropertyChangeListener() As PropertyChangeListener
			Return handler
		End Function

		Private Property handler As Handler
			Get
				If handler Is Nothing Then handler = New Handler(Me)
				Return handler
			End Get
		End Property

		Private Class Handler
			Implements PropertyChangeListener

			Private ReadOnly outerInstance As BasicDesktopPaneUI

			Public Sub New(ByVal outerInstance As BasicDesktopPaneUI)
				Me.outerInstance = outerInstance
			End Sub

			Public Overridable Sub propertyChange(ByVal evt As PropertyChangeEvent)
				Dim propertyName As String = evt.propertyName
				If "desktopManager" = propertyName Then outerInstance.installDesktopManager()
			End Sub
		End Class

		''' <summary>
		''' The default DesktopManager installed by the UI.
		''' </summary>
		Private Class BasicDesktopManager
			Inherits DefaultDesktopManager
			Implements UIResource

			Private ReadOnly outerInstance As BasicDesktopPaneUI

			Public Sub New(ByVal outerInstance As BasicDesktopPaneUI)
				Me.outerInstance = outerInstance
			End Sub

		End Class

		Private Class Actions
			Inherits sun.swing.UIAction

			Private Shared CLOSE As String = "close"
			Private Shared ESCAPE As String = "escape"
			Private Shared MAXIMIZE As String = "maximize"
			Private Shared MINIMIZE As String = "minimize"
			Private Shared MOVE As String = "move"
			Private Shared RESIZE As String = "resize"
			Private Shared RESTORE As String = "restore"
			Private Shared LEFT As String = "left"
			Private Shared RIGHT As String = "right"
			Private Shared UP As String = "up"
			Private Shared DOWN As String = "down"
			Private Shared SHRINK_LEFT As String = "shrinkLeft"
			Private Shared SHRINK_RIGHT As String = "shrinkRight"
			Private Shared SHRINK_UP As String = "shrinkUp"
			Private Shared SHRINK_DOWN As String = "shrinkDown"
			Private Shared NEXT_FRAME As String = "selectNextFrame"
			Private Shared PREVIOUS_FRAME As String = "selectPreviousFrame"
			Private Shared NAVIGATE_NEXT As String = "navigateNext"
			Private Shared NAVIGATE_PREVIOUS As String = "navigatePrevious"
			Private ReadOnly MOVE_RESIZE_INCREMENT As Integer = 10
			Private Shared moving As Boolean = False
			Private Shared resizing As Boolean = False
			Private Shared sourceFrame As JInternalFrame = Nothing
			Private Shared focusOwner As Component = Nothing

			Friend Sub New()
				MyBase.New(Nothing)
			End Sub

			Friend Sub New(ByVal name As String)
				MyBase.New(name)
			End Sub

			Public Overridable Sub actionPerformed(ByVal e As ActionEvent)
				Dim dp As JDesktopPane = CType(e.source, JDesktopPane)
				Dim key As String = name

				If CLOSE = key OrElse MAXIMIZE = key OrElse MINIMIZE = key OrElse RESTORE = key Then
					stateate(dp, key)
				ElseIf ESCAPE = key Then
					If sourceFrame Is dp.selectedFrame AndAlso focusOwner IsNot Nothing Then focusOwner.requestFocus()
					moving = False
					resizing = False
					sourceFrame = Nothing
					focusOwner = Nothing
				ElseIf MOVE = key OrElse RESIZE = key Then
					sourceFrame = dp.selectedFrame
					If sourceFrame Is Nothing Then Return
					moving = If(key = MOVE, True, False)
					resizing = If(key = RESIZE, True, False)

					focusOwner = java.awt.KeyboardFocusManager.currentKeyboardFocusManager.focusOwner
					If Not SwingUtilities.isDescendingFrom(focusOwner, sourceFrame) Then focusOwner = Nothing
					sourceFrame.requestFocus()
				ElseIf LEFT = key OrElse RIGHT = key OrElse UP = key OrElse DOWN = key OrElse SHRINK_RIGHT = key OrElse SHRINK_LEFT = key OrElse SHRINK_UP = key OrElse SHRINK_DOWN = key Then
					Dim c As JInternalFrame = dp.selectedFrame
					If sourceFrame Is Nothing OrElse c IsNot sourceFrame OrElse java.awt.KeyboardFocusManager.currentKeyboardFocusManager.focusOwner IsNot sourceFrame Then Return
					Dim minOnScreenInsets As java.awt.Insets = UIManager.getInsets("Desktop.minOnScreenInsets")
					Dim size As java.awt.Dimension = c.size
					Dim minSize As java.awt.Dimension = c.minimumSize
					Dim dpWidth As Integer = dp.width
					Dim dpHeight As Integer = dp.height
					Dim delta As Integer
					Dim loc As Point = c.location
					If LEFT = key Then
						If moving Then
							c.locationion(If(loc.x + size.width - MOVE_RESIZE_INCREMENT < minOnScreenInsets.right, -size.width + minOnScreenInsets.right, loc.x - MOVE_RESIZE_INCREMENT), loc.y)
						ElseIf resizing Then
							c.locationion(loc.x - MOVE_RESIZE_INCREMENT, loc.y)
							c.sizeize(size.width + MOVE_RESIZE_INCREMENT, size.height)
						End If
					ElseIf RIGHT = key Then
						If moving Then
							c.locationion(If(loc.x + MOVE_RESIZE_INCREMENT > dpWidth - minOnScreenInsets.left, dpWidth - minOnScreenInsets.left, loc.x + MOVE_RESIZE_INCREMENT), loc.y)
						ElseIf resizing Then
							c.sizeize(size.width + MOVE_RESIZE_INCREMENT, size.height)
						End If
					ElseIf UP = key Then
						If moving Then
							c.locationion(loc.x,If(loc.y + size.height - MOVE_RESIZE_INCREMENT < minOnScreenInsets.bottom, -size.height + minOnScreenInsets.bottom, loc.y - MOVE_RESIZE_INCREMENT))
						ElseIf resizing Then
							c.locationion(loc.x, loc.y - MOVE_RESIZE_INCREMENT)
							c.sizeize(size.width, size.height + MOVE_RESIZE_INCREMENT)
						End If
					ElseIf DOWN = key Then
						If moving Then
							c.locationion(loc.x,If(loc.y + MOVE_RESIZE_INCREMENT > dpHeight - minOnScreenInsets.top, dpHeight - minOnScreenInsets.top, loc.y + MOVE_RESIZE_INCREMENT))
						ElseIf resizing Then
							c.sizeize(size.width, size.height + MOVE_RESIZE_INCREMENT)
						End If
					ElseIf SHRINK_LEFT = key AndAlso resizing Then
						' Make sure we don't resize less than minimum size.
						If minSize.width < (size.width - MOVE_RESIZE_INCREMENT) Then
							delta = MOVE_RESIZE_INCREMENT
						Else
							delta = size.width - minSize.width
						End If

						' Ensure that we keep the internal frame on the desktop.
						If loc.x + size.width - delta < minOnScreenInsets.left Then delta = loc.x + size.width - minOnScreenInsets.left
						c.sizeize(size.width - delta, size.height)
					ElseIf SHRINK_RIGHT = key AndAlso resizing Then
						' Make sure we don't resize less than minimum size.
						If minSize.width < (size.width - MOVE_RESIZE_INCREMENT) Then
							delta = MOVE_RESIZE_INCREMENT
						Else
							delta = size.width - minSize.width
						End If

						' Ensure that we keep the internal frame on the desktop.
						If loc.x + delta > dpWidth - minOnScreenInsets.right Then delta = (dpWidth - minOnScreenInsets.right) - loc.x

						c.locationion(loc.x + delta, loc.y)
						c.sizeize(size.width - delta, size.height)
					ElseIf SHRINK_UP = key AndAlso resizing Then
						' Make sure we don't resize less than minimum size.
						If minSize.height < (size.height - MOVE_RESIZE_INCREMENT) Then
							delta = MOVE_RESIZE_INCREMENT
						Else
							delta = size.height - minSize.height
						End If

						' Ensure that we keep the internal frame on the desktop.
						If loc.y + size.height - delta < minOnScreenInsets.bottom Then delta = loc.y + size.height - minOnScreenInsets.bottom

						c.sizeize(size.width, size.height - delta)
					ElseIf SHRINK_DOWN = key AndAlso resizing Then
						' Make sure we don't resize less than minimum size.
						If minSize.height < (size.height - MOVE_RESIZE_INCREMENT) Then
							delta = MOVE_RESIZE_INCREMENT
						Else
							delta = size.height - minSize.height
						End If

						' Ensure that we keep the internal frame on the desktop.
						If loc.y + delta > dpHeight - minOnScreenInsets.top Then delta = (dpHeight - minOnScreenInsets.top) - loc.y

						c.locationion(loc.x, loc.y + delta)
						c.sizeize(size.width, size.height - delta)
					End If
				ElseIf NEXT_FRAME = key OrElse PREVIOUS_FRAME = key Then
					dp.selectFrame(If(key = NEXT_FRAME, True, False))
				ElseIf NAVIGATE_NEXT = key OrElse NAVIGATE_PREVIOUS = key Then
					Dim moveForward As Boolean = True
					If NAVIGATE_PREVIOUS = key Then moveForward = False
					Dim cycleRoot As Container = dp.focusCycleRootAncestor

					If cycleRoot IsNot Nothing Then
						Dim policy As FocusTraversalPolicy = cycleRoot.focusTraversalPolicy
						If policy IsNot Nothing AndAlso TypeOf policy Is SortingFocusTraversalPolicy Then
							Dim sPolicy As SortingFocusTraversalPolicy = CType(policy, SortingFocusTraversalPolicy)
							Dim idc As Boolean = sPolicy.implicitDownCycleTraversal
							Try
								sPolicy.implicitDownCycleTraversal = False
								If moveForward Then
									java.awt.KeyboardFocusManager.currentKeyboardFocusManager.focusNextComponent(dp)
								Else
									java.awt.KeyboardFocusManager.currentKeyboardFocusManager.focusPreviousComponent(dp)
								End If
							Finally
								sPolicy.implicitDownCycleTraversal = idc
							End Try
						End If
					End If
				End If
			End Sub

			Private Sub setState(ByVal dp As JDesktopPane, ByVal state As String)
				If state = CLOSE Then
					Dim f As JInternalFrame = dp.selectedFrame
					If f Is Nothing Then Return
					f.doDefaultCloseAction()
				ElseIf state = MAXIMIZE Then
					' maximize the selected frame
					Dim f As JInternalFrame = dp.selectedFrame
					If f Is Nothing Then Return
					If Not f.maximum Then
						If f.icon Then
							Try
								f.icon = False
								f.maximum = True
							Catch pve As PropertyVetoException
							End Try
						Else
							Try
								f.maximum = True
							Catch pve As PropertyVetoException
							End Try
						End If
					End If
				ElseIf state = MINIMIZE Then
					' minimize the selected frame
					Dim f As JInternalFrame = dp.selectedFrame
					If f Is Nothing Then Return
					If Not f.icon Then
						Try
							f.icon = True
						Catch pve As PropertyVetoException
						End Try
					End If
				ElseIf state = RESTORE Then
					' restore the selected minimized or maximized frame
					Dim f As JInternalFrame = dp.selectedFrame
					If f Is Nothing Then Return
					Try
						If f.icon Then
							f.icon = False
						ElseIf f.maximum Then
							f.maximum = False
						End If
						f.selected = True
					Catch pve As PropertyVetoException
					End Try
				End If
			End Sub

			Public Overridable Function isEnabled(ByVal sender As Object) As Boolean
				If TypeOf sender Is JDesktopPane Then
					Dim dp As JDesktopPane = CType(sender, JDesktopPane)
					Dim action As String = name
					If action = Actions.NEXT_FRAME OrElse action = Actions.PREVIOUS_FRAME Then Return True
					Dim iFrame As JInternalFrame = dp.selectedFrame
					If iFrame Is Nothing Then
						Return False
					ElseIf action = Actions.CLOSE Then
						Return iFrame.closable
					ElseIf action = Actions.MINIMIZE Then
						Return iFrame.iconifiable
					ElseIf action = Actions.MAXIMIZE Then
						Return iFrame.maximizable
					End If
					Return True
				End If
				Return False
			End Function
		End Class


		''' <summary>
		''' Handles restoring a minimized or maximized internal frame.
		''' @since 1.3
		''' </summary>
		Protected Friend Class OpenAction
			Inherits AbstractAction

			Private ReadOnly outerInstance As BasicDesktopPaneUI

			Public Sub New(ByVal outerInstance As BasicDesktopPaneUI)
				Me.outerInstance = outerInstance
			End Sub

			Public Overridable Sub actionPerformed(ByVal evt As ActionEvent)
				Dim dp As JDesktopPane = CType(evt.source, JDesktopPane)
				SHARED_ACTION.stateate(dp, Actions.RESTORE)
			End Sub

			Public Property Overrides enabled As Boolean
				Get
					Return True
				End Get
			End Property
		End Class

		''' <summary>
		''' Handles closing an internal frame.
		''' </summary>
		Protected Friend Class CloseAction
			Inherits AbstractAction

			Private ReadOnly outerInstance As BasicDesktopPaneUI

			Public Sub New(ByVal outerInstance As BasicDesktopPaneUI)
				Me.outerInstance = outerInstance
			End Sub

			Public Overridable Sub actionPerformed(ByVal evt As ActionEvent)
				Dim dp As JDesktopPane = CType(evt.source, JDesktopPane)
				SHARED_ACTION.stateate(dp, Actions.CLOSE)
			End Sub

			Public Property Overrides enabled As Boolean
				Get
					Dim iFrame As JInternalFrame = outerInstance.desktop.selectedFrame
					If iFrame IsNot Nothing Then Return iFrame.closable
					Return False
				End Get
			End Property
		End Class

		''' <summary>
		''' Handles minimizing an internal frame.
		''' </summary>
		Protected Friend Class MinimizeAction
			Inherits AbstractAction

			Private ReadOnly outerInstance As BasicDesktopPaneUI

			Public Sub New(ByVal outerInstance As BasicDesktopPaneUI)
				Me.outerInstance = outerInstance
			End Sub

			Public Overridable Sub actionPerformed(ByVal evt As ActionEvent)
				Dim dp As JDesktopPane = CType(evt.source, JDesktopPane)
				SHARED_ACTION.stateate(dp, Actions.MINIMIZE)
			End Sub

			Public Property Overrides enabled As Boolean
				Get
					Dim iFrame As JInternalFrame = outerInstance.desktop.selectedFrame
					If iFrame IsNot Nothing Then Return iFrame.iconifiable
					Return False
				End Get
			End Property
		End Class

		''' <summary>
		''' Handles maximizing an internal frame.
		''' </summary>
		Protected Friend Class MaximizeAction
			Inherits AbstractAction

			Private ReadOnly outerInstance As BasicDesktopPaneUI

			Public Sub New(ByVal outerInstance As BasicDesktopPaneUI)
				Me.outerInstance = outerInstance
			End Sub

			Public Overridable Sub actionPerformed(ByVal evt As ActionEvent)
				Dim dp As JDesktopPane = CType(evt.source, JDesktopPane)
				SHARED_ACTION.stateate(dp, Actions.MAXIMIZE)
			End Sub

			Public Property Overrides enabled As Boolean
				Get
					Dim iFrame As JInternalFrame = outerInstance.desktop.selectedFrame
					If iFrame IsNot Nothing Then Return iFrame.maximizable
					Return False
				End Get
			End Property
		End Class

		''' <summary>
		''' Handles navigating to the next internal frame.
		''' </summary>
		Protected Friend Class NavigateAction
			Inherits AbstractAction

			Private ReadOnly outerInstance As BasicDesktopPaneUI

			Public Sub New(ByVal outerInstance As BasicDesktopPaneUI)
				Me.outerInstance = outerInstance
			End Sub

			Public Overridable Sub actionPerformed(ByVal evt As ActionEvent)
				Dim dp As JDesktopPane = CType(evt.source, JDesktopPane)
				dp.selectFrame(True)
			End Sub

			Public Property Overrides enabled As Boolean
				Get
					Return True
				End Get
			End Property
		End Class
	End Class

End Namespace