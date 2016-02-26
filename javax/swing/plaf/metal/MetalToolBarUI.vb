Imports System
Imports System.Runtime.CompilerServices
Imports System.Collections.Generic
Imports javax.swing
Imports javax.swing.event
Imports javax.swing.border
Imports javax.swing.plaf
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
	''' A Metal Look and Feel implementation of ToolBarUI.  This implementation
	''' is a "combined" view/controller.
	''' <p>
	''' 
	''' @author Jeff Shapiro
	''' </summary>
	Public Class MetalToolBarUI
		Inherits BasicToolBarUI

		''' <summary>
		''' An array of WeakReferences that point to JComponents. This will contain
		''' instances of JToolBars and JMenuBars and is used to find
		''' JToolBars/JMenuBars that border each other.
		''' </summary>
		Private Shared components As IList(Of WeakReference(Of JComponent)) = New List(Of WeakReference(Of JComponent))

		''' <summary>
		''' This protected field is implementation specific. Do not access directly
		''' or override. Use the create method instead.
		''' </summary>
		''' <seealso cref= #createContainerListener </seealso>
		Protected Friend contListener As ContainerListener

		''' <summary>
		''' This protected field is implementation specific. Do not access directly
		''' or override. Use the create method instead.
		''' </summary>
		''' <seealso cref= #createRolloverListener </seealso>
		Protected Friend rolloverListener As java.beans.PropertyChangeListener

		Private Shared nonRolloverBorder As Border

		''' <summary>
		''' Last menubar the toolbar touched.  This is only useful for ocean.
		''' </summary>
		Private lastMenuBar As JMenuBar

		''' <summary>
		''' Registers the specified component.
		''' </summary>
		<MethodImpl(MethodImplOptions.Synchronized)> _
		Friend Shared Sub register(ByVal c As JComponent)
			If c Is Nothing Then Throw New NullPointerException("JComponent must be non-null")
			components.Add(New WeakReference(Of JComponent)(c))
		End Sub

		''' <summary>
		''' Unregisters the specified component.
		''' </summary>
		<MethodImpl(MethodImplOptions.Synchronized)> _
		Friend Shared Sub unregister(ByVal c As JComponent)
			For counter As Integer = components.Count - 1 To 0 Step -1
				' Search for the component, removing any flushed references
				' along the way.
				Dim target As JComponent = components(counter).get()

				If target Is c OrElse target Is Nothing Then components.RemoveAt(counter)
			Next counter
		End Sub

		''' <summary>
		''' Finds a previously registered component of class <code>target</code>
		''' that shares the JRootPane ancestor of <code>from</code>.
		''' </summary>
		<MethodImpl(MethodImplOptions.Synchronized)> _
		Friend Shared Function findRegisteredComponentOfType(ByVal [from] As JComponent, ByVal target As Type) As Object
			Dim rp As JRootPane = SwingUtilities.getRootPane([from])
			If rp IsNot Nothing Then
				For counter As Integer = components.Count - 1 To 0 Step -1
					Dim component As Object = CType(components(counter), WeakReference).get()

					If component Is Nothing Then
						' WeakReference has gone away, remove the WeakReference
						components.RemoveAt(counter)
					ElseIf target.IsInstanceOfType(component) AndAlso SwingUtilities.getRootPane(CType(component, java.awt.Component)) Is rp Then
						Return component
					End If
				Next counter
			End If
			Return Nothing
		End Function

		''' <summary>
		''' Returns true if the passed in JMenuBar is above a horizontal
		''' JToolBar.
		''' </summary>
		Friend Shared Function doesMenuBarBorderToolBar(ByVal c As JMenuBar) As Boolean
			Dim tb As JToolBar = CType(MetalToolBarUI.findRegisteredComponentOfType(c, GetType(JToolBar)), JToolBar)
			If tb IsNot Nothing AndAlso tb.orientation = JToolBar.HORIZONTAL Then
				Dim rp As JRootPane = SwingUtilities.getRootPane(c)
				Dim point As New java.awt.Point(0, 0)
				point = SwingUtilities.convertPoint(c, point, rp)
				Dim menuX As Integer = point.x
				Dim menuY As Integer = point.y
					point.y = 0
					point.x = point.y
				point = SwingUtilities.convertPoint(tb, point, rp)
				Return (point.x = menuX AndAlso menuY + c.height = point.y AndAlso c.width = tb.width)
			End If
			Return False
		End Function

		Public Shared Function createUI(ByVal c As JComponent) As ComponentUI
			Return New MetalToolBarUI
		End Function

		Public Overrides Sub installUI(ByVal c As JComponent)
			MyBase.installUI(c)
			register(c)
		End Sub

		Public Overrides Sub uninstallUI(ByVal c As JComponent)
			MyBase.uninstallUI(c)
			nonRolloverBorder = Nothing
			unregister(c)
		End Sub

		Protected Friend Overrides Sub installListeners()
			MyBase.installListeners()

			contListener = createContainerListener()
			If contListener IsNot Nothing Then toolBar.addContainerListener(contListener)
			rolloverListener = createRolloverListener()
			If rolloverListener IsNot Nothing Then toolBar.addPropertyChangeListener(rolloverListener)
		End Sub

		Protected Friend Overrides Sub uninstallListeners()
			MyBase.uninstallListeners()

			If contListener IsNot Nothing Then toolBar.removeContainerListener(contListener)
			rolloverListener = createRolloverListener()
			If rolloverListener IsNot Nothing Then toolBar.removePropertyChangeListener(rolloverListener)
		End Sub

		Protected Friend Overrides Function createRolloverBorder() As Border
			Return MyBase.createRolloverBorder()
		End Function

		Protected Friend Overrides Function createNonRolloverBorder() As Border
			Return MyBase.createNonRolloverBorder()
		End Function


		''' <summary>
		''' Creates a non rollover border for Toggle buttons in the toolbar.
		''' </summary>
		Private Function createNonRolloverToggleBorder() As Border
			Return createNonRolloverBorder()
		End Function

		Protected Friend Overridable Property borderToNonRollover As java.awt.Component
			Set(ByVal c As java.awt.Component)
				If TypeOf c Is JToggleButton AndAlso Not(TypeOf c Is JCheckBox) Then
					' 4735514, 4886944: The method createNonRolloverToggleBorder() is
					' private in BasicToolBarUI so we can't override it. We still need
					' to call super from this method so that it can save away the
					' original border and then we install ours.
    
					' Before calling super we get a handle to the old border, because
					' super will install a non-UIResource border that we can't
					' distinguish from one provided by an application.
					Dim b As JToggleButton = CType(c, JToggleButton)
					Dim border As Border = b.border
					MyBase.borderToNonRollover = c
					If TypeOf border Is UIResource Then
						If nonRolloverBorder Is Nothing Then nonRolloverBorder = createNonRolloverToggleBorder()
						b.border = nonRolloverBorder
					End If
				Else
					MyBase.borderToNonRollover = c
				End If
			End Set
		End Property


		''' <summary>
		''' Creates a container listener that will be added to the JToolBar.
		''' If this method returns null then it will not be added to the
		''' toolbar.
		''' </summary>
		''' <returns> an instance of a <code>ContainerListener</code> or null </returns>
		Protected Friend Overridable Function createContainerListener() As ContainerListener
			Return Nothing
		End Function

		''' <summary>
		''' Creates a property change listener that will be added to the JToolBar.
		''' If this method returns null then it will not be added to the
		''' toolbar.
		''' </summary>
		''' <returns> an instance of a <code>PropertyChangeListener</code> or null </returns>
		Protected Friend Overridable Function createRolloverListener() As java.beans.PropertyChangeListener
			Return Nothing
		End Function

		Protected Friend Overrides Function createDockingListener() As MouseInputListener
			Return New MetalDockingListener(Me, toolBar)
		End Function

		Protected Friend Overridable Property dragOffset As java.awt.Point
			Set(ByVal p As java.awt.Point)
				If Not java.awt.GraphicsEnvironment.headless Then
					If dragWindow Is Nothing Then dragWindow = createDragWindow(toolBar)
					dragWindow.offset = p
				End If
			End Set
		End Property

		''' <summary>
		''' If necessary paints the background of the component, then invokes
		''' <code>paint</code>.
		''' </summary>
		''' <param name="g"> Graphics to paint to </param>
		''' <param name="c"> JComponent painting on </param>
		''' <exception cref="NullPointerException"> if <code>g</code> or <code>c</code> is
		'''         null </exception>
		''' <seealso cref= javax.swing.plaf.ComponentUI#update </seealso>
		''' <seealso cref= javax.swing.plaf.ComponentUI#paint
		''' @since 1.5 </seealso>
		Public Overridable Sub update(ByVal g As java.awt.Graphics, ByVal c As JComponent)
			If g Is Nothing Then Throw New NullPointerException("graphics must be non-null")
			If c.opaque AndAlso (TypeOf c.background Is UIResource) AndAlso CType(c, JToolBar).orientation = JToolBar.HORIZONTAL AndAlso UIManager.get("MenuBar.gradient") IsNot Nothing Then
				Dim rp As JRootPane = SwingUtilities.getRootPane(c)
				Dim mb As JMenuBar = CType(findRegisteredComponentOfType(c, GetType(JMenuBar)), JMenuBar)
				If mb IsNot Nothing AndAlso mb.opaque AndAlso (TypeOf mb.background Is UIResource) Then
					Dim point As New java.awt.Point(0, 0)
					point = SwingUtilities.convertPoint(c, point, rp)
					Dim x As Integer = point.x
					Dim y As Integer = point.y
						point.y = 0
						point.x = point.y
					point = SwingUtilities.convertPoint(mb, point, rp)
					If point.x = x AndAlso y = point.y + mb.height AndAlso mb.width = c.width AndAlso MetalUtils.drawGradient(c, g, "MenuBar.gradient", 0, -mb.height, c.width, c.height + mb.height, True) Then
						lastMenuBar = mb
						paint(g, c)
						Return
					End If
				End If
				If MetalUtils.drawGradient(c, g, "MenuBar.gradient", 0, 0, c.width, c.height, True) Then
					lastMenuBar = Nothing
					paint(g, c)
					Return
				End If
			End If
			lastMenuBar = Nothing
			MyBase.update(g, c)
		End Sub

		Private Property lastMenuBar As JMenuBar
			Set(ByVal lastMenuBar As JMenuBar)
				If MetalLookAndFeel.usingOcean() Then
					If Me.lastMenuBar IsNot lastMenuBar Then
						' The menubar we previously touched has changed, force it
						' to repaint.
						If Me.lastMenuBar IsNot Nothing Then Me.lastMenuBar.repaint()
						If lastMenuBar IsNot Nothing Then lastMenuBar.repaint()
						Me.lastMenuBar = lastMenuBar
					End If
				End If
			End Set
		End Property

		' No longer used. Cannot remove for compatibility reasons
		Protected Friend Class MetalContainerListener
			Inherits BasicToolBarUI.ToolBarContListener

			Private ReadOnly outerInstance As MetalToolBarUI

			Public Sub New(ByVal outerInstance As MetalToolBarUI)
				Me.outerInstance = outerInstance
			End Sub

		End Class

		' No longer used. Cannot remove for compatibility reasons
		Protected Friend Class MetalRolloverListener
			Inherits BasicToolBarUI.PropertyListener

			Private ReadOnly outerInstance As MetalToolBarUI

			Public Sub New(ByVal outerInstance As MetalToolBarUI)
				Me.outerInstance = outerInstance
			End Sub

		End Class

		Protected Friend Class MetalDockingListener
			Inherits DockingListener

			Private ReadOnly outerInstance As MetalToolBarUI

			Private pressedInBumps As Boolean = False

			Public Sub New(ByVal outerInstance As MetalToolBarUI, ByVal t As JToolBar)
					Me.outerInstance = outerInstance
				MyBase.New(t)
			End Sub

			Public Overridable Sub mousePressed(ByVal e As MouseEvent)
				MyBase.mousePressed(e)
				If Not outerInstance.toolBar.enabled Then Return
				pressedInBumps = False
				Dim bumpRect As New java.awt.Rectangle

				If outerInstance.toolBar.orientation = JToolBar.HORIZONTAL Then
					Dim x As Integer = If(MetalUtils.isLeftToRight(outerInstance.toolBar), 0, outerInstance.toolBar.size.width-14)
					bumpRect.boundsnds(x, 0, 14, outerInstance.toolBar.size.height) ' vertical
				Else
					bumpRect.boundsnds(0, 0, outerInstance.toolBar.size.width, 14)
				End If
				If bumpRect.contains(e.point) Then
					pressedInBumps = True
					Dim dragOffset As java.awt.Point = e.point
					If Not MetalUtils.isLeftToRight(outerInstance.toolBar) Then dragOffset.x -= (outerInstance.toolBar.size.width - outerInstance.toolBar.preferredSize.width)
					outerInstance.dragOffset = dragOffset
				End If
			End Sub

			Public Overridable Sub mouseDragged(ByVal e As MouseEvent)
				If pressedInBumps Then MyBase.mouseDragged(e)
			End Sub
		End Class ' end class MetalDockingListener
	End Class

End Namespace