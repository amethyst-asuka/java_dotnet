Imports Microsoft.VisualBasic
Imports System
Imports System.Collections.Generic
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
	''' A Basic L&amp;F implementation of ToolBarUI.  This implementation
	''' is a "combined" view/controller.
	''' <p>
	''' 
	''' @author Georges Saab
	''' @author Jeff Shapiro
	''' </summary>
	Public Class BasicToolBarUI
		Inherits ToolBarUI
		Implements SwingConstants

		Protected Friend toolBar As JToolBar
		Private floating As Boolean
		Private floatingX As Integer
		Private floatingY As Integer
		Private floatingFrame As JFrame
		Private floatingToolBar As RootPaneContainer
		Protected Friend dragWindow As DragWindow
		Private dockingSource As Container
		Private dockingSensitivity As Integer = 0
		Protected Friend focusedCompIndex As Integer = -1

		Protected Friend dockingColor As Color = Nothing
		Protected Friend floatingColor As Color = Nothing
		Protected Friend dockingBorderColor As Color = Nothing
		Protected Friend floatingBorderColor As Color = Nothing

		Protected Friend dockingListener As MouseInputListener
		Protected Friend propertyListener As PropertyChangeListener

		Protected Friend toolBarContListener As ContainerListener
		Protected Friend toolBarFocusListener As FocusListener
		Private handler As Handler

		Protected Friend constraintBeforeFloating As String = BorderLayout.NORTH

		' Rollover button implementation.
		Private Shared IS_ROLLOVER As String = "JToolBar.isRollover"
		Private Shared rolloverBorder As Border
		Private Shared nonRolloverBorder As Border
		Private Shared nonRolloverToggleBorder As Border
		Private rolloverBorders As Boolean = False

		Private borderTable As New Dictionary(Of AbstractButton, Border)
		Private rolloverTable As New Dictionary(Of AbstractButton, Boolean?)


		''' <summary>
		''' As of Java 2 platform v1.3 this previously undocumented field is no
		''' longer used.
		''' Key bindings are now defined by the LookAndFeel, please refer to
		''' the key bindings specification for further details.
		''' </summary>
		''' @deprecated As of Java 2 platform v1.3. 
		<Obsolete("As of Java 2 platform v1.3.")> _
		Protected Friend upKey As KeyStroke
		''' <summary>
		''' As of Java 2 platform v1.3 this previously undocumented field is no
		''' longer used.
		''' Key bindings are now defined by the LookAndFeel, please refer to
		''' the key bindings specification for further details.
		''' </summary>
		''' @deprecated As of Java 2 platform v1.3. 
		<Obsolete("As of Java 2 platform v1.3.")> _
		Protected Friend downKey As KeyStroke
		''' <summary>
		''' As of Java 2 platform v1.3 this previously undocumented field is no
		''' longer used.
		''' Key bindings are now defined by the LookAndFeel, please refer to
		''' the key bindings specification for further details.
		''' </summary>
		''' @deprecated As of Java 2 platform v1.3. 
		<Obsolete("As of Java 2 platform v1.3.")> _
		Protected Friend leftKey As KeyStroke
		''' <summary>
		''' As of Java 2 platform v1.3 this previously undocumented field is no
		''' longer used.
		''' Key bindings are now defined by the LookAndFeel, please refer to
		''' the key bindings specification for further details.
		''' </summary>
		''' @deprecated As of Java 2 platform v1.3. 
		<Obsolete("As of Java 2 platform v1.3.")> _
		Protected Friend rightKey As KeyStroke


		Private Shared FOCUSED_COMP_INDEX As String = "JToolBar.focusedCompIndex"

		Public Shared Function createUI(ByVal c As JComponent) As ComponentUI
			Return New BasicToolBarUI
		End Function

		Public Overridable Sub installUI(ByVal c As JComponent)
			toolBar = CType(c, JToolBar)

			' Set defaults
			installDefaults()
			installComponents()
			installListeners()
			installKeyboardActions()

			' Initialize instance vars
			dockingSensitivity = 0
			floating = False
				floatingY = 0
				floatingX = floatingY
			floatingToolBar = Nothing

			orientation = toolBar.orientation
			LookAndFeel.installProperty(c, "opaque", Boolean.TRUE)

			If c.getClientProperty(FOCUSED_COMP_INDEX) IsNot Nothing Then focusedCompIndex = CInt(Fix(c.getClientProperty(FOCUSED_COMP_INDEX)))
		End Sub

		Public Overridable Sub uninstallUI(ByVal c As JComponent)

			' Clear defaults
			uninstallDefaults()
			uninstallComponents()
			uninstallListeners()
			uninstallKeyboardActions()

			' Clear instance vars
			If floating Then floatinging(False, Nothing)

			floatingToolBar = Nothing
			dragWindow = Nothing
			dockingSource = Nothing

			c.putClientProperty(FOCUSED_COMP_INDEX, Convert.ToInt32(focusedCompIndex))
		End Sub

		Protected Friend Overridable Sub installDefaults()
			LookAndFeel.installBorder(toolBar,"ToolBar.border")
			LookAndFeel.installColorsAndFont(toolBar, "ToolBar.background", "ToolBar.foreground", "ToolBar.font")
			' Toolbar specific defaults
			If dockingColor Is Nothing OrElse TypeOf dockingColor Is UIResource Then dockingColor = UIManager.getColor("ToolBar.dockingBackground")
			If floatingColor Is Nothing OrElse TypeOf floatingColor Is UIResource Then floatingColor = UIManager.getColor("ToolBar.floatingBackground")
			If dockingBorderColor Is Nothing OrElse TypeOf dockingBorderColor Is UIResource Then dockingBorderColor = UIManager.getColor("ToolBar.dockingForeground")
			If floatingBorderColor Is Nothing OrElse TypeOf floatingBorderColor Is UIResource Then floatingBorderColor = UIManager.getColor("ToolBar.floatingForeground")

			' ToolBar rollover button borders
			Dim rolloverProp As Object = toolBar.getClientProperty(IS_ROLLOVER)
			If rolloverProp Is Nothing Then rolloverProp = UIManager.get("ToolBar.isRollover")
			If rolloverProp IsNot Nothing Then rolloverBorders = CBool(rolloverProp)

			If rolloverBorder Is Nothing Then rolloverBorder = createRolloverBorder()
			If nonRolloverBorder Is Nothing Then nonRolloverBorder = createNonRolloverBorder()
			If nonRolloverToggleBorder Is Nothing Then nonRolloverToggleBorder = createNonRolloverToggleBorder()


			rolloverBorders = rolloverBorders
		End Sub

		Protected Friend Overridable Sub uninstallDefaults()
			LookAndFeel.uninstallBorder(toolBar)
			dockingColor = Nothing
			floatingColor = Nothing
			dockingBorderColor = Nothing
			floatingBorderColor = Nothing

			installNormalBorders(toolBar)

			rolloverBorder = Nothing
			nonRolloverBorder = Nothing
			nonRolloverToggleBorder = Nothing
		End Sub

		Protected Friend Overridable Sub installComponents()
		End Sub

		Protected Friend Overridable Sub uninstallComponents()
		End Sub

		Protected Friend Overridable Sub installListeners()
			dockingListener = createDockingListener()

			If dockingListener IsNot Nothing Then
				toolBar.addMouseMotionListener(dockingListener)
				toolBar.addMouseListener(dockingListener)
			End If

			propertyListener = createPropertyListener() ' added in setFloating
			If propertyListener IsNot Nothing Then toolBar.addPropertyChangeListener(propertyListener)

			toolBarContListener = createToolBarContListener()
			If toolBarContListener IsNot Nothing Then toolBar.addContainerListener(toolBarContListener)

			toolBarFocusListener = createToolBarFocusListener()

			If toolBarFocusListener IsNot Nothing Then
				' Put focus listener on all components in toolbar
				Dim components As Component() = toolBar.components

				For Each component As Component In components
					component.addFocusListener(toolBarFocusListener)
				Next component
			End If
		End Sub

		Protected Friend Overridable Sub uninstallListeners()
			If dockingListener IsNot Nothing Then
				toolBar.removeMouseMotionListener(dockingListener)
				toolBar.removeMouseListener(dockingListener)

				dockingListener = Nothing
			End If

			If propertyListener IsNot Nothing Then
				toolBar.removePropertyChangeListener(propertyListener)
				propertyListener = Nothing ' removed in setFloating
			End If

			If toolBarContListener IsNot Nothing Then
				toolBar.removeContainerListener(toolBarContListener)
				toolBarContListener = Nothing
			End If

			If toolBarFocusListener IsNot Nothing Then
				' Remove focus listener from all components in toolbar
				Dim components As Component() = toolBar.components

				For Each component As Component In components
					component.removeFocusListener(toolBarFocusListener)
				Next component

				toolBarFocusListener = Nothing
			End If
			handler = Nothing
		End Sub

		Protected Friend Overridable Sub installKeyboardActions()
			Dim km As InputMap = getInputMap(JComponent.WHEN_ANCESTOR_OF_FOCUSED_COMPONENT)

			SwingUtilities.replaceUIInputMap(toolBar, JComponent.WHEN_ANCESTOR_OF_FOCUSED_COMPONENT, km)

		LazyActionMap.installLazyActionMap(toolBar, GetType(BasicToolBarUI), "ToolBar.actionMap")
		End Sub

		Friend Overridable Function getInputMap(ByVal condition As Integer) As InputMap
			If condition = JComponent.WHEN_ANCESTOR_OF_FOCUSED_COMPONENT Then Return CType(sun.swing.DefaultLookup.get(toolBar, Me, "ToolBar.ancestorInputMap"), InputMap)
			Return Nothing
		End Function

		Friend Shared Sub loadActionMap(ByVal map As LazyActionMap)
			map.put(New Actions(Actions.NAVIGATE_RIGHT))
			map.put(New Actions(Actions.NAVIGATE_LEFT))
			map.put(New Actions(Actions.NAVIGATE_UP))
			map.put(New Actions(Actions.NAVIGATE_DOWN))
		End Sub

		Protected Friend Overridable Sub uninstallKeyboardActions()
			SwingUtilities.replaceUIActionMap(toolBar, Nothing)
			SwingUtilities.replaceUIInputMap(toolBar, JComponent.WHEN_ANCESTOR_OF_FOCUSED_COMPONENT, Nothing)
		End Sub

		Protected Friend Overridable Sub navigateFocusedComp(ByVal direction As Integer)
			Dim nComp As Integer = toolBar.componentCount
			Dim j As Integer

			Select Case direction
				Case EAST, SOUTH

					If focusedCompIndex < 0 OrElse focusedCompIndex >= nComp Then Exit Select

					j = focusedCompIndex + 1

					Do While j <> focusedCompIndex
						If j >= nComp Then j = 0
						Dim comp As Component = toolBar.getComponentAtIndex(j)
						j += 1

						If comp IsNot Nothing AndAlso comp.focusTraversable AndAlso comp.enabled Then
							comp.requestFocus()
							Exit Do
						End If
					Loop


				Case WEST, NORTH

					If focusedCompIndex < 0 OrElse focusedCompIndex >= nComp Then Exit Select

					j = focusedCompIndex - 1

					Do While j <> focusedCompIndex
						If j < 0 Then j = nComp - 1
						Dim comp As Component = toolBar.getComponentAtIndex(j)
						j -= 1

						If comp IsNot Nothing AndAlso comp.focusTraversable AndAlso comp.enabled Then
							comp.requestFocus()
							Exit Do
						End If
					Loop


				Case Else
			End Select
		End Sub

		''' <summary>
		''' Creates a rollover border for toolbar components. The
		''' rollover border will be installed if rollover borders are
		''' enabled.
		''' <p>
		''' Override this method to provide an alternate rollover border.
		''' 
		''' @since 1.4
		''' </summary>
		Protected Friend Overridable Function createRolloverBorder() As Border
			Dim border As Object = UIManager.get("ToolBar.rolloverBorder")
			If border IsNot Nothing Then Return CType(border, Border)
			Dim table As UIDefaults = UIManager.lookAndFeelDefaults
			Return New CompoundBorder(New BasicBorders.RolloverButtonBorder(table.getColor("controlShadow"), table.getColor("controlDkShadow"), table.getColor("controlHighlight"), table.getColor("controlLtHighlight")), New BasicBorders.RolloverMarginBorder)
		End Function

		''' <summary>
		''' Creates the non rollover border for toolbar components. This
		''' border will be installed as the border for components added
		''' to the toolbar if rollover borders are not enabled.
		''' <p>
		''' Override this method to provide an alternate rollover border.
		''' 
		''' @since 1.4
		''' </summary>
		Protected Friend Overridable Function createNonRolloverBorder() As Border
			Dim border As Object = UIManager.get("ToolBar.nonrolloverBorder")
			If border IsNot Nothing Then Return CType(border, Border)
			Dim table As UIDefaults = UIManager.lookAndFeelDefaults
			Return New CompoundBorder(New BasicBorders.ButtonBorder(table.getColor("Button.shadow"), table.getColor("Button.darkShadow"), table.getColor("Button.light"), table.getColor("Button.highlight")), New BasicBorders.RolloverMarginBorder)
		End Function

		''' <summary>
		''' Creates a non rollover border for Toggle buttons in the toolbar.
		''' </summary>
		Private Function createNonRolloverToggleBorder() As Border
			Dim table As UIDefaults = UIManager.lookAndFeelDefaults
			Return New CompoundBorder(New BasicBorders.RadioButtonBorder(table.getColor("ToggleButton.shadow"), table.getColor("ToggleButton.darkShadow"), table.getColor("ToggleButton.light"), table.getColor("ToggleButton.highlight")), New BasicBorders.RolloverMarginBorder)
		End Function

		''' <summary>
		''' No longer used, use BasicToolBarUI.createFloatingWindow(JToolBar) </summary>
		''' <seealso cref= #createFloatingWindow </seealso>
		Protected Friend Overridable Function createFloatingFrame(ByVal toolbar As JToolBar) As JFrame
			Dim window As Window = SwingUtilities.getWindowAncestor(toolbar)
			Dim frame As JFrame = New JFrameAnonymousInnerClassHelper
			frame.rootPane.name = "ToolBar.FloatingFrame"
			frame.resizable = False
			Dim wl As WindowListener = createFrameListener()
			frame.addWindowListener(wl)
			Return frame
		End Function

		Private Class JFrameAnonymousInnerClassHelper
			Inherits JFrame

					' Override createRootPane() to automatically resize
					' the frame when contents change
			Protected Friend Overrides Function createRootPane() As JRootPane
				Dim rootPane As JRootPane = New JRootPaneAnonymousInnerClassHelper
				rootPane.opaque = True
				Return rootPane
			End Function

			Private Class JRootPaneAnonymousInnerClassHelper
				Inherits JRootPane

				Private packing As Boolean = False

				Public Overridable Sub validate()
					MyBase.validate()
					If Not packing Then
						packing = True
						pack()
						packing = False
					End If
				End Sub
			End Class
		End Class

		''' <summary>
		''' Creates a window which contains the toolbar after it has been
		''' dragged out from its container </summary>
		''' <returns> a <code>RootPaneContainer</code> object, containing the toolbar.
		''' @since 1.4 </returns>
		Protected Friend Overridable Function createFloatingWindow(ByVal toolbar As JToolBar) As RootPaneContainer
'JAVA TO VB CONVERTER TODO TASK: Local classes are not converted by Java to VB Converter:
'			class ToolBarDialog extends JDialog
	'		{
	'			public ToolBarDialog(Frame owner, String title, boolean modal)
	'			{
	'				MyBase(owner, title, modal);
	'			}
	'
	'			public ToolBarDialog(Dialog owner, String title, boolean modal)
	'			{
	'				MyBase(owner, title, modal);
	'			}
	'
	'			' Override createRootPane() to automatically resize
	'			' the frame when contents change
	'			protected JRootPane createRootPane()
	'			{
	'				JRootPane rootPane = New JRootPane()
	'				{
	'					private boolean packing = False;
	'
	'					public void validate()
	'					{
	'						MyBase.validate();
	'						if (!packing)
	'						{
	'							packing = True;
	'							pack();
	'							packing = False;
	'						}
	'					}
	'				};
	'				rootPane.setOpaque(True);
	'				Return rootPane;
	'			}
	'		}

			Dim dialog As JDialog
			Dim window As Window = SwingUtilities.getWindowAncestor(toolbar)
			If TypeOf window Is Frame Then
				dialog = New ToolBarDialog(CType(window, Frame), toolbar.name, False)
			ElseIf TypeOf window Is Dialog Then
				dialog = New ToolBarDialog(CType(window, Dialog), toolbar.name, False)
			Else
				dialog = New ToolBarDialog(CType(Nothing, Frame), toolbar.name, False)
			End If

			dialog.rootPane.name = "ToolBar.FloatingWindow"
			dialog.title = toolbar.name
			dialog.resizable = False
			Dim wl As WindowListener = createFrameListener()
			dialog.addWindowListener(wl)
			Return dialog
		End Function

		Protected Friend Overridable Function createDragWindow(ByVal toolbar As JToolBar) As DragWindow
			Dim frame As Window = Nothing
			If Me.toolBar IsNot Nothing Then
				Dim p As Container
				p = Me.toolBar.parent
				Do While p IsNot Nothing AndAlso Not(TypeOf p Is Window)

					p = p.parent
				Loop
				If p IsNot Nothing AndAlso TypeOf p Is Window Then frame = CType(p, Window)
			End If
			If floatingToolBar Is Nothing Then floatingToolBar = createFloatingWindow(Me.toolBar)
			If TypeOf floatingToolBar Is Window Then frame = CType(floatingToolBar, Window)
			Dim dragWindow As New DragWindow(Me, frame)
			Return dragWindow
		End Function

		''' <summary>
		''' Returns a flag to determine whether rollover button borders
		''' are enabled.
		''' </summary>
		''' <returns> true if rollover borders are enabled; false otherwise </returns>
		''' <seealso cref= #setRolloverBorders
		''' @since 1.4 </seealso>
		Public Overridable Property rolloverBorders As Boolean
			Get
				Return rolloverBorders
			End Get
			Set(ByVal rollover As Boolean)
				rolloverBorders = rollover
    
				If rolloverBorders Then
					installRolloverBorders(toolBar)
				Else
					installNonRolloverBorders(toolBar)
				End If
			End Set
		End Property


		''' <summary>
		''' Installs rollover borders on all the child components of the JComponent.
		''' <p>
		''' This is a convenience method to call <code>setBorderToRollover</code>
		''' for each child component.
		''' </summary>
		''' <param name="c"> container which holds the child components (usually a JToolBar) </param>
		''' <seealso cref= #setBorderToRollover
		''' @since 1.4 </seealso>
		Protected Friend Overridable Sub installRolloverBorders(ByVal c As JComponent)
			' Put rollover borders on buttons
			Dim components As Component() = c.components

			For Each component As Component In components
				If TypeOf component Is JComponent Then
					CType(component, JComponent).updateUI()
					borderToRollover = component
				End If
			Next component
		End Sub

		''' <summary>
		''' Installs non-rollover borders on all the child components of the JComponent.
		''' A non-rollover border is the border that is installed on the child component
		''' while it is in the toolbar.
		''' <p>
		''' This is a convenience method to call <code>setBorderToNonRollover</code>
		''' for each child component.
		''' </summary>
		''' <param name="c"> container which holds the child components (usually a JToolBar) </param>
		''' <seealso cref= #setBorderToNonRollover
		''' @since 1.4 </seealso>
		Protected Friend Overridable Sub installNonRolloverBorders(ByVal c As JComponent)
			' Put non-rollover borders on buttons. These borders reduce the margin.
			Dim components As Component() = c.components

			For Each component As Component In components
				If TypeOf component Is JComponent Then
					CType(component, JComponent).updateUI()
					borderToNonRollover = component
				End If
			Next component
		End Sub

		''' <summary>
		''' Installs normal borders on all the child components of the JComponent.
		''' A normal border is the original border that was installed on the child
		''' component before it was added to the toolbar.
		''' <p>
		''' This is a convenience method to call <code>setBorderNormal</code>
		''' for each child component.
		''' </summary>
		''' <param name="c"> container which holds the child components (usually a JToolBar) </param>
		''' <seealso cref= #setBorderToNonRollover
		''' @since 1.4 </seealso>
		Protected Friend Overridable Sub installNormalBorders(ByVal c As JComponent)
			' Put back the normal borders on buttons
			Dim components As Component() = c.components

			For Each component As Component In components
				borderToNormal = component
			Next component
		End Sub

		''' <summary>
		''' Sets the border of the component to have a rollover border which
		''' was created by the <seealso cref="#createRolloverBorder"/> method.
		''' </summary>
		''' <param name="c"> component which will have a rollover border installed </param>
		''' <seealso cref= #createRolloverBorder
		''' @since 1.4 </seealso>
		Protected Friend Overridable Property borderToRollover As Component
			Set(ByVal c As Component)
				If TypeOf c Is AbstractButton Then
					Dim b As AbstractButton = CType(c, AbstractButton)
    
					Dim border As Border = borderTable(b)
					If border Is Nothing OrElse TypeOf border Is UIResource Then borderTable(b) = b.border
    
					' Only set the border if its the default border
					If TypeOf b.border Is UIResource Then b.border = getRolloverBorder(b)
    
					rolloverTable(b) = If(b.rolloverEnabled, Boolean.TRUE, Boolean.FALSE)
					b.rolloverEnabled = True
				End If
			End Set
		End Property

		''' <summary>
		''' Returns a rollover border for the button.
		''' </summary>
		''' <param name="b"> the button to calculate the rollover border for </param>
		''' <returns> the rollover border </returns>
		''' <seealso cref= #setBorderToRollover
		''' @since 1.6 </seealso>
		Protected Friend Overridable Function getRolloverBorder(ByVal b As AbstractButton) As Border
			Return rolloverBorder
		End Function

		''' <summary>
		''' Sets the border of the component to have a non-rollover border which
		''' was created by the <seealso cref="#createNonRolloverBorder"/> method.
		''' </summary>
		''' <param name="c"> component which will have a non-rollover border installed </param>
		''' <seealso cref= #createNonRolloverBorder
		''' @since 1.4 </seealso>
		Protected Friend Overridable Property borderToNonRollover As Component
			Set(ByVal c As Component)
				If TypeOf c Is AbstractButton Then
					Dim b As AbstractButton = CType(c, AbstractButton)
    
					Dim border As Border = borderTable(b)
					If border Is Nothing OrElse TypeOf border Is UIResource Then borderTable(b) = b.border
    
					' Only set the border if its the default border
					If TypeOf b.border Is UIResource Then b.border = getNonRolloverBorder(b)
					rolloverTable(b) = If(b.rolloverEnabled, Boolean.TRUE, Boolean.FALSE)
					b.rolloverEnabled = False
				End If
			End Set
		End Property

		''' <summary>
		''' Returns a non-rollover border for the button.
		''' </summary>
		''' <param name="b"> the button to calculate the non-rollover border for </param>
		''' <returns> the non-rollover border </returns>
		''' <seealso cref= #setBorderToNonRollover
		''' @since 1.6 </seealso>
		Protected Friend Overridable Function getNonRolloverBorder(ByVal b As AbstractButton) As Border
			If TypeOf b Is JToggleButton Then
				Return nonRolloverToggleBorder
			Else
				Return nonRolloverBorder
			End If
		End Function

		''' <summary>
		''' Sets the border of the component to have a normal border.
		''' A normal border is the original border that was installed on the child
		''' component before it was added to the toolbar.
		''' </summary>
		''' <param name="c"> component which will have a normal border re-installed </param>
		''' <seealso cref= #createNonRolloverBorder
		''' @since 1.4 </seealso>
		Protected Friend Overridable Property borderToNormal As Component
			Set(ByVal c As Component)
				If TypeOf c Is AbstractButton Then
					Dim b As AbstractButton = CType(c, AbstractButton)
    
					Dim border As Border = borderTable.Remove(b)
					b.border = border
    
					Dim value As Boolean? = rolloverTable.Remove(b)
					If value IsNot Nothing Then b.rolloverEnabled = value
				End If
			End Set
		End Property

		Public Overridable Sub setFloatingLocation(ByVal x As Integer, ByVal y As Integer)
			floatingX = x
			floatingY = y
		End Sub

		Public Overridable Property floating As Boolean
			Get
				Return floating
			End Get
		End Property

		Public Overridable Sub setFloating(ByVal b As Boolean, ByVal p As Point)
			If toolBar.floatable Then
				Dim visible As Boolean = False
				Dim ancestor As Window = SwingUtilities.getWindowAncestor(toolBar)
				If ancestor IsNot Nothing Then visible = ancestor.visible
				If dragWindow IsNot Nothing Then dragWindow.visible = False
				Me.floating = b
				If floatingToolBar Is Nothing Then floatingToolBar = createFloatingWindow(toolBar)
				If b = True Then
					If dockingSource Is Nothing Then
						dockingSource = toolBar.parent
						dockingSource.remove(toolBar)
					End If
					constraintBeforeFloating = calculateConstraint()
					If propertyListener IsNot Nothing Then UIManager.addPropertyChangeListener(propertyListener)
					floatingToolBar.contentPane.add(toolBar,BorderLayout.CENTER)
					If TypeOf floatingToolBar Is Window Then
						CType(floatingToolBar, Window).pack()
						CType(floatingToolBar, Window).locationion(floatingX, floatingY)
						If visible Then
							CType(floatingToolBar, Window).show()
						Else
'JAVA TO VB CONVERTER TODO TASK: Anonymous inner classes are not converted to VB if the base type is not defined in the code being converted:
'							ancestor.addWindowListener(New WindowAdapter()
	'						{
	'							public void windowOpened(WindowEvent e)
	'							{
	'								((Window)floatingToolBar).show();
	'							}
	'						});
						End If
					End If
				Else
					If floatingToolBar Is Nothing Then floatingToolBar = createFloatingWindow(toolBar)
					If TypeOf floatingToolBar Is Window Then CType(floatingToolBar, Window).visible = False
					floatingToolBar.contentPane.remove(toolBar)
					Dim constraint As String = getDockingConstraint(dockingSource, p)
					If constraint Is Nothing Then constraint = BorderLayout.NORTH
					Dim ___orientation As Integer = mapConstraintToOrientation(constraint)
					orientation = ___orientation
					If dockingSource Is Nothing Then dockingSource = toolBar.parent
					If propertyListener IsNot Nothing Then UIManager.removePropertyChangeListener(propertyListener)
					dockingSource.add(constraint, toolBar)
				End If
				dockingSource.invalidate()
				Dim dockingSourceParent As Container = dockingSource.parent
				If dockingSourceParent IsNot Nothing Then dockingSourceParent.validate()
				dockingSource.repaint()
			End If
		End Sub

		Private Function mapConstraintToOrientation(ByVal constraint As String) As Integer
			Dim ___orientation As Integer = toolBar.orientation

			If constraint IsNot Nothing Then
				If constraint.Equals(BorderLayout.EAST) OrElse constraint.Equals(BorderLayout.WEST) Then
					___orientation = JToolBar.VERTICAL
				ElseIf constraint.Equals(BorderLayout.NORTH) OrElse constraint.Equals(BorderLayout.SOUTH) Then
					___orientation = JToolBar.HORIZONTAL
				End If
			End If

			Return ___orientation
		End Function

		Public Overridable Property orientation As Integer
			Set(ByVal orientation As Integer)
				toolBar.orientation = orientation
    
				If dragWindow IsNot Nothing Then dragWindow.orientation = orientation
			End Set
		End Property

		''' <summary>
		''' Gets the color displayed when over a docking area
		''' </summary>
		Public Overridable Property dockingColor As Color
			Get
				Return dockingColor
			End Get
			Set(ByVal c As Color)
				Me.dockingColor = c
		   End Set
		End Property


		''' <summary>
		''' Gets the color displayed when over a floating area
		''' </summary>
		Public Overridable Property floatingColor As Color
			Get
				Return floatingColor
			End Get
			Set(ByVal c As Color)
				Me.floatingColor = c
			End Set
		End Property


		Private Function isBlocked(ByVal comp As Component, ByVal constraint As Object) As Boolean
			If TypeOf comp Is Container Then
				Dim cont As Container = CType(comp, Container)
				Dim lm As LayoutManager = cont.layout
				If TypeOf lm Is BorderLayout Then
					Dim blm As BorderLayout = CType(lm, BorderLayout)
					Dim c As Component = blm.getLayoutComponent(cont, constraint)
					Return (c IsNot Nothing AndAlso c IsNot toolBar)
				End If
			End If
			Return False
		End Function

		Public Overridable Function canDock(ByVal c As Component, ByVal p As Point) As Boolean
			Return (p IsNot Nothing AndAlso getDockingConstraint(c, p) IsNot Nothing)
		End Function

		Private Function calculateConstraint() As String
			Dim constraint As String = Nothing
			Dim lm As LayoutManager = dockingSource.layout
			If TypeOf lm Is BorderLayout Then constraint = CStr(CType(lm, BorderLayout).getConstraints(toolBar))
			Return If(constraint IsNot Nothing, constraint, constraintBeforeFloating)
		End Function



		Private Function getDockingConstraint(ByVal c As Component, ByVal p As Point) As String
			If p Is Nothing Then Return constraintBeforeFloating
			If c.contains(p) Then
				dockingSensitivity = If(toolBar.orientation = JToolBar.HORIZONTAL, toolBar.size.height, toolBar.size.width)
				' North  (Base distance on height for now!)
				If p.y < dockingSensitivity AndAlso (Not isBlocked(c, BorderLayout.NORTH)) Then Return BorderLayout.NORTH
				' East  (Base distance on height for now!)
				If p.x >= c.width - dockingSensitivity AndAlso (Not isBlocked(c, BorderLayout.EAST)) Then Return BorderLayout.EAST
				' West  (Base distance on height for now!)
				If p.x < dockingSensitivity AndAlso (Not isBlocked(c, BorderLayout.WEST)) Then Return BorderLayout.WEST
				If p.y >= c.height - dockingSensitivity AndAlso (Not isBlocked(c, BorderLayout.SOUTH)) Then Return BorderLayout.SOUTH
			End If
			Return Nothing
		End Function

		Protected Friend Overridable Sub dragTo(ByVal position As Point, ByVal origin As Point)
			If toolBar.floatable Then
			  Try
				If dragWindow Is Nothing Then dragWindow = createDragWindow(toolBar)
				Dim offset As Point = dragWindow.offset
				If offset Is Nothing Then
					Dim size As Dimension = toolBar.preferredSize
					offset = New Point(size.width/2, size.height/2)
					dragWindow.offset = offset
				End If
				Dim [global] As New Point(origin.x+ position.x, origin.y+position.y)
				Dim dragPoint As New Point([global].x- offset.x, [global].y- offset.y)
				If dockingSource Is Nothing Then dockingSource = toolBar.parent
					constraintBeforeFloating = calculateConstraint()
				Dim dockingPosition As Point = dockingSource.locationOnScreen
				Dim comparisonPoint As New Point([global].x-dockingPosition.x, [global].y-dockingPosition.y)
				If canDock(dockingSource, comparisonPoint) Then
					dragWindow.background = dockingColor
					Dim constraint As String = getDockingConstraint(dockingSource, comparisonPoint)
					Dim ___orientation As Integer = mapConstraintToOrientation(constraint)
					dragWindow.orientation = ___orientation
					dragWindow.borderColor = dockingBorderColor
				Else
					dragWindow.background = floatingColor
					dragWindow.borderColor = floatingBorderColor
					dragWindow.orientation = toolBar.orientation
				End If

				dragWindow.locationion(dragPoint.x, dragPoint.y)
				If dragWindow.visible = False Then
					Dim size As Dimension = toolBar.preferredSize
					dragWindow.sizeize(size.width, size.height)
					dragWindow.show()
				End If
			  Catch e As IllegalComponentStateException
			  End Try
			End If
		End Sub

		Protected Friend Overridable Sub floatAt(ByVal position As Point, ByVal origin As Point)
			If toolBar.floatable Then
			  Try
				Dim offset As Point = dragWindow.offset
				If offset Is Nothing Then
					offset = position
					dragWindow.offset = offset
				End If
				Dim [global] As New Point(origin.x+ position.x, origin.y+position.y)
				floatingLocationion([global].x-offset.x, [global].y-offset.y)
				If dockingSource IsNot Nothing Then
					Dim dockingPosition As Point = dockingSource.locationOnScreen
					Dim comparisonPoint As New Point([global].x-dockingPosition.x, [global].y-dockingPosition.y)
					If canDock(dockingSource, comparisonPoint) Then
						floatinging(False, comparisonPoint)
					Else
						floatinging(True, Nothing)
					End If
				Else
					floatinging(True, Nothing)
				End If
				dragWindow.offset = Nothing
			  Catch e As IllegalComponentStateException
			  End Try
			End If
		End Sub

		Private Property handler As Handler
			Get
				If handler Is Nothing Then handler = New Handler(Me)
				Return handler
			End Get
		End Property

		Protected Friend Overridable Function createToolBarContListener() As ContainerListener
			Return handler
		End Function

		Protected Friend Overridable Function createToolBarFocusListener() As FocusListener
			Return handler
		End Function

		Protected Friend Overridable Function createPropertyListener() As PropertyChangeListener
			Return handler
		End Function

		Protected Friend Overridable Function createDockingListener() As MouseInputListener
			handler.tb = toolBar
			Return handler
		End Function

		Protected Friend Overridable Function createFrameListener() As WindowListener
			Return New FrameListener(Me)
		End Function

		''' <summary>
		''' Paints the contents of the window used for dragging.
		''' </summary>
		''' <param name="g"> Graphics to paint to. </param>
		''' <exception cref="NullPointerException"> is <code>g</code> is null
		''' @since 1.5 </exception>
		Protected Friend Overridable Sub paintDragWindow(ByVal g As Graphics)
			g.color = dragWindow.background
			Dim w As Integer = dragWindow.width
			Dim h As Integer = dragWindow.height
			g.fillRect(0, 0, w, h)
			g.color = dragWindow.borderColor
			g.drawRect(0, 0, w - 1, h - 1)
		End Sub


		Private Class Actions
			Inherits sun.swing.UIAction

			Private Const NAVIGATE_RIGHT As String = "navigateRight"
			Private Const NAVIGATE_LEFT As String = "navigateLeft"
			Private Const NAVIGATE_UP As String = "navigateUp"
			Private Const NAVIGATE_DOWN As String = "navigateDown"

			Public Sub New(ByVal name As String)
				MyBase.New(name)
			End Sub

			Public Overridable Sub actionPerformed(ByVal evt As ActionEvent)
				Dim key As String = name
				Dim toolBar As JToolBar = CType(evt.source, JToolBar)
				Dim ui As BasicToolBarUI = CType(BasicLookAndFeel.getUIOfType(toolBar.uI, GetType(BasicToolBarUI)), BasicToolBarUI)

				If NAVIGATE_RIGHT = key Then
					ui.navigateFocusedComp(EAST)
				ElseIf NAVIGATE_LEFT = key Then
					ui.navigateFocusedComp(WEST)
				ElseIf NAVIGATE_UP = key Then
					ui.navigateFocusedComp(NORTH)
				ElseIf NAVIGATE_DOWN = key Then
					ui.navigateFocusedComp(SOUTH)
				End If
			End Sub
		End Class


		Private Class Handler
			Implements ContainerListener, FocusListener, MouseInputListener, PropertyChangeListener

			Private ReadOnly outerInstance As BasicToolBarUI

			Public Sub New(ByVal outerInstance As BasicToolBarUI)
				Me.outerInstance = outerInstance
			End Sub


			'
			' ContainerListener
			'
			Public Overridable Sub componentAdded(ByVal evt As ContainerEvent)
				Dim c As Component = evt.child

				If outerInstance.toolBarFocusListener IsNot Nothing Then c.addFocusListener(outerInstance.toolBarFocusListener)

				If outerInstance.rolloverBorders Then
					outerInstance.borderToRollover = c
				Else
					outerInstance.borderToNonRollover = c
				End If
			End Sub

			Public Overridable Sub componentRemoved(ByVal evt As ContainerEvent)
				Dim c As Component = evt.child

				If outerInstance.toolBarFocusListener IsNot Nothing Then c.removeFocusListener(outerInstance.toolBarFocusListener)

				' Revert the button border
				outerInstance.borderToNormal = c
			End Sub


			'
			' FocusListener
			'
			Public Overridable Sub focusGained(ByVal evt As FocusEvent)
				Dim c As Component = evt.component
				outerInstance.focusedCompIndex = outerInstance.toolBar.getComponentIndex(c)
			End Sub

			Public Overridable Sub focusLost(ByVal evt As FocusEvent)
			End Sub


			'
			' MouseInputListener (DockingListener)
			'
			Friend tb As JToolBar
			Friend isDragging As Boolean = False
			Friend origin As Point = Nothing

			Public Overridable Sub mousePressed(ByVal evt As MouseEvent)
				If Not tb.enabled Then Return
				isDragging = False
			End Sub

			Public Overridable Sub mouseReleased(ByVal evt As MouseEvent)
				If Not tb.enabled Then Return
				If isDragging Then
					Dim position As Point = evt.point
					If origin Is Nothing Then origin = evt.component.locationOnScreen
					outerInstance.floatAt(position, origin)
				End If
				origin = Nothing
				isDragging = False
			End Sub

			Public Overridable Sub mouseDragged(ByVal evt As MouseEvent)
				If Not tb.enabled Then Return
				isDragging = True
				Dim position As Point = evt.point
				If origin Is Nothing Then origin = evt.component.locationOnScreen
				outerInstance.dragTo(position, origin)
			End Sub

			Public Overridable Sub mouseClicked(ByVal evt As MouseEvent)
			End Sub
			Public Overridable Sub mouseEntered(ByVal evt As MouseEvent)
			End Sub
			Public Overridable Sub mouseExited(ByVal evt As MouseEvent)
			End Sub
			Public Overridable Sub mouseMoved(ByVal evt As MouseEvent)
			End Sub


			'
			' PropertyChangeListener
			'
			Public Overridable Sub propertyChange(ByVal evt As PropertyChangeEvent)
				Dim propertyName As String = evt.propertyName
				If propertyName = "lookAndFeel" Then
					outerInstance.toolBar.updateUI()
				ElseIf propertyName = "orientation" Then
					' Search for JSeparator components and change it's orientation
					' to match the toolbar and flip it's orientation.
					Dim components As Component() = outerInstance.toolBar.components
					Dim orientation As Integer = CInt(Fix(evt.newValue))
					Dim separator As JToolBar.Separator

					For i As Integer = 0 To components.Length - 1
						If TypeOf components(i) Is JToolBar.Separator Then
							separator = CType(components(i), JToolBar.Separator)
							If (orientation = JToolBar.HORIZONTAL) Then
								separator.orientation = JSeparator.VERTICAL
							Else
								separator.orientation = JSeparator.HORIZONTAL
							End If
							Dim size As Dimension = separator.separatorSize
							If size IsNot Nothing AndAlso size.width <> size.height Then
								' Flip the orientation.
								Dim newSize As New Dimension(size.height, size.width)
								separator.separatorSize = newSize
							End If
						End If
					Next i
				ElseIf propertyName = IS_ROLLOVER Then
					outerInstance.installNormalBorders(outerInstance.toolBar)
					outerInstance.rolloverBorders = CBool(evt.newValue)
				End If
			End Sub
		End Class

		Protected Friend Class FrameListener
			Inherits WindowAdapter

			Private ReadOnly outerInstance As BasicToolBarUI

			Public Sub New(ByVal outerInstance As BasicToolBarUI)
				Me.outerInstance = outerInstance
			End Sub

			Public Overridable Sub windowClosing(ByVal w As WindowEvent)
				If outerInstance.toolBar.floatable Then
					If outerInstance.dragWindow IsNot Nothing Then outerInstance.dragWindow.visible = False
					outerInstance.floating = False
					If outerInstance.floatingToolBar Is Nothing Then outerInstance.floatingToolBar = outerInstance.createFloatingWindow(outerInstance.toolBar)
					If TypeOf outerInstance.floatingToolBar Is Window Then CType(outerInstance.floatingToolBar, Window).visible = False
					outerInstance.floatingToolBar.contentPane.remove(outerInstance.toolBar)
					Dim constraint As String = outerInstance.constraintBeforeFloating
					If outerInstance.toolBar.orientation = JToolBar.HORIZONTAL Then
						If constraint = "West" OrElse constraint = "East" Then constraint = "North"
					Else
						If constraint = "North" OrElse constraint = "South" Then constraint = "West"
					End If
					If outerInstance.dockingSource Is Nothing Then outerInstance.dockingSource = outerInstance.toolBar.parent
					If outerInstance.propertyListener IsNot Nothing Then UIManager.removePropertyChangeListener(outerInstance.propertyListener)
					outerInstance.dockingSource.add(outerInstance.toolBar, constraint)
					outerInstance.dockingSource.invalidate()
					Dim dockingSourceParent As Container = outerInstance.dockingSource.parent
					If dockingSourceParent IsNot Nothing Then dockingSourceParent.validate()
					outerInstance.dockingSource.repaint()
				End If
			End Sub

		End Class

		Protected Friend Class ToolBarContListener
			Implements ContainerListener

			Private ReadOnly outerInstance As BasicToolBarUI

			Public Sub New(ByVal outerInstance As BasicToolBarUI)
				Me.outerInstance = outerInstance
			End Sub

			' NOTE: This class exists only for backward compatibility. All
			' its functionality has been moved into Handler. If you need to add
			' new functionality add it to the Handler, but make sure this
			' class calls into the Handler.
			Public Overridable Sub componentAdded(ByVal e As ContainerEvent)
				outerInstance.handler.componentAdded(e)
			End Sub

			Public Overridable Sub componentRemoved(ByVal e As ContainerEvent)
				outerInstance.handler.componentRemoved(e)
			End Sub

		End Class

		Protected Friend Class ToolBarFocusListener
			Implements FocusListener

			Private ReadOnly outerInstance As BasicToolBarUI

			Public Sub New(ByVal outerInstance As BasicToolBarUI)
				Me.outerInstance = outerInstance
			End Sub

			' NOTE: This class exists only for backward compatibility. All
			' its functionality has been moved into Handler. If you need to add
			' new functionality add it to the Handler, but make sure this
			' class calls into the Handler.
			Public Overridable Sub focusGained(ByVal e As FocusEvent)
				outerInstance.handler.focusGained(e)
			End Sub

			Public Overridable Sub focusLost(ByVal e As FocusEvent)
				outerInstance.handler.focusLost(e)
			End Sub
		End Class

		Protected Friend Class PropertyListener
			Implements PropertyChangeListener

			Private ReadOnly outerInstance As BasicToolBarUI

			Public Sub New(ByVal outerInstance As BasicToolBarUI)
				Me.outerInstance = outerInstance
			End Sub

			' NOTE: This class exists only for backward compatibility. All
			' its functionality has been moved into Handler. If you need to add
			' new functionality add it to the Handler, but make sure this
			' class calls into the Handler.
			Public Overridable Sub propertyChange(ByVal e As PropertyChangeEvent)
				outerInstance.handler.propertyChange(e)
			End Sub
		End Class

		''' <summary>
		''' This class should be treated as a &quot;protected&quot; inner class.
		''' Instantiate it only within subclasses of BasicToolBarUI.
		''' </summary>
		Public Class DockingListener
			Implements MouseInputListener

			Private ReadOnly outerInstance As BasicToolBarUI

			' NOTE: This class exists only for backward compatibility. All
			' its functionality has been moved into Handler. If you need to add
			' new functionality add it to the Handler, but make sure this
			' class calls into the Handler.
			Protected Friend toolBar As JToolBar
			Protected Friend isDragging As Boolean = False
			Protected Friend origin As Point = Nothing

			Public Sub New(ByVal outerInstance As BasicToolBarUI, ByVal t As JToolBar)
					Me.outerInstance = outerInstance
				Me.toolBar = t
				outerInstance.handler.tb = t
			End Sub

			Public Overridable Sub mouseClicked(ByVal e As MouseEvent)
			outerInstance.handler.mouseClicked(e)
			End Sub

			Public Overridable Sub mousePressed(ByVal e As MouseEvent)
			outerInstance.handler.tb = toolBar
			outerInstance.handler.mousePressed(e)
			isDragging = outerInstance.handler.isDragging
			End Sub

			Public Overridable Sub mouseReleased(ByVal e As MouseEvent)
			outerInstance.handler.tb = toolBar
			outerInstance.handler.isDragging = isDragging
			outerInstance.handler.origin = origin
			outerInstance.handler.mouseReleased(e)
			isDragging = outerInstance.handler.isDragging
			origin = outerInstance.handler.origin
			End Sub

			Public Overridable Sub mouseEntered(ByVal e As MouseEvent)
			outerInstance.handler.mouseEntered(e)
			End Sub

			Public Overridable Sub mouseExited(ByVal e As MouseEvent)
			outerInstance.handler.mouseExited(e)
			End Sub

			Public Overridable Sub mouseDragged(ByVal e As MouseEvent)
			outerInstance.handler.tb = toolBar
			outerInstance.handler.origin = origin
			outerInstance.handler.mouseDragged(e)
			isDragging = outerInstance.handler.isDragging
			origin = outerInstance.handler.origin
			End Sub

			Public Overridable Sub mouseMoved(ByVal e As MouseEvent)
			outerInstance.handler.mouseMoved(e)
			End Sub
		End Class

		Protected Friend Class DragWindow
			Inherits Window

			Private ReadOnly outerInstance As BasicToolBarUI

			Friend borderColor As Color = Color.gray
			Friend orientation As Integer = outerInstance.toolBar.orientation
			Friend offset As Point ' offset of the mouse cursor inside the DragWindow

			Friend Sub New(ByVal outerInstance As BasicToolBarUI, ByVal w As Window)
					Me.outerInstance = outerInstance
				MyBase.New(w)
			End Sub

		''' <summary>
		''' Returns the orientation of the toolbar window when the toolbar is
		''' floating. The orientation is either one of <code>JToolBar.HORIZONTAL</code>
		''' or <code>JToolBar.VERTICAL</code>.
		''' </summary>
		''' <returns> the orientation of the toolbar window
		''' @since 1.6 </returns>
		Public Overridable Property orientation As Integer
			Get
				Return orientation
			End Get
			Set(ByVal o As Integer)
					If showing Then
						If o = Me.orientation Then Return
						Me.orientation = o
						Dim size As Dimension = size
						size = New Dimension(size.height, size.width)
						If offset IsNot Nothing Then
							If BasicGraphicsUtils.isLeftToRight(outerInstance.toolBar) Then
								offset = New Point(offset.y, offset.x)
							ElseIf o = JToolBar.HORIZONTAL Then
								offset = New Point(size.height-offset.y, offset.x)
							Else
								offset = New Point(offset.y, size.width-offset.x)
							End If
						End If
						repaint()
					End If
				End Set
		End Property


			Public Overridable Property offset As Point
				Get
					Return offset
				End Get
				Set(ByVal p As Point)
					Me.offset = p
				End Set
			End Property


			Public Overridable Property borderColor As Color
				Set(ByVal c As Color)
					If Me.borderColor Is c Then Return
					Me.borderColor = c
					repaint()
				End Set
				Get
					Return Me.borderColor
				End Get
			End Property


			Public Overridable Sub paint(ByVal g As Graphics)
				outerInstance.paintDragWindow(g)
				' Paint the children
				MyBase.paint(g)
			End Sub
			Public Overridable Property insets As Insets
				Get
					Return New Insets(1,1,1,1)
				End Get
			End Property
		End Class
	End Class

End Namespace