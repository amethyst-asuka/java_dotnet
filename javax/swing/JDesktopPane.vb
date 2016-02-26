Imports Microsoft.VisualBasic
Imports System
Imports System.Collections.Generic
Imports javax.swing.plaf
Imports javax.accessibility

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

Namespace javax.swing

	''' <summary>
	''' A container used to create a multiple-document interface or a virtual desktop.
	''' You create <code>JInternalFrame</code> objects and add them to the
	''' <code>JDesktopPane</code>. <code>JDesktopPane</code> extends
	''' <code>JLayeredPane</code> to manage the potentially overlapping internal
	''' frames. It also maintains a reference to an instance of
	''' <code>DesktopManager</code> that is set by the UI
	''' class for the current look and feel (L&amp;F).  Note that <code>JDesktopPane</code>
	''' does not support borders.
	''' <p>
	''' This class is normally used as the parent of <code>JInternalFrames</code>
	''' to provide a pluggable <code>DesktopManager</code> object to the
	''' <code>JInternalFrames</code>. The <code>installUI</code> of the
	''' L&amp;F specific implementation is responsible for setting the
	''' <code>desktopManager</code> variable appropriately.
	''' When the parent of a <code>JInternalFrame</code> is a <code>JDesktopPane</code>,
	''' it should delegate most of its behavior to the <code>desktopManager</code>
	''' (closing, resizing, etc).
	''' <p>
	''' For further documentation and examples see
	''' <a href="https://docs.oracle.com/javase/tutorial/uiswing/components/internalframe.html">How to Use Internal Frames</a>,
	''' a section in <em>The Java Tutorial</em>.
	''' <p>
	''' <strong>Warning:</strong> Swing is not thread safe. For more
	''' information see <a
	''' href="package-summary.html#threading">Swing's Threading
	''' Policy</a>.
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
	''' <seealso cref= JInternalFrame </seealso>
	''' <seealso cref= JInternalFrame.JDesktopIcon </seealso>
	''' <seealso cref= DesktopManager
	''' 
	''' @author David Kloba </seealso>
	Public Class JDesktopPane
		Inherits JLayeredPane
		Implements Accessible

		''' <seealso cref= #getUIClassID </seealso>
		''' <seealso cref= #readObject </seealso>
		Private Const uiClassID As String = "DesktopPaneUI"

		<NonSerialized> _
		Friend desktopManager As DesktopManager

		<NonSerialized> _
		Private selectedFrame As JInternalFrame = Nothing

		''' <summary>
		''' Indicates that the entire contents of the item being dragged
		''' should appear inside the desktop pane.
		''' </summary>
		''' <seealso cref= #OUTLINE_DRAG_MODE </seealso>
		''' <seealso cref= #setDragMode </seealso>
		Public Const LIVE_DRAG_MODE As Integer = 0

		''' <summary>
		''' Indicates that an outline only of the item being dragged
		''' should appear inside the desktop pane.
		''' </summary>
		''' <seealso cref= #LIVE_DRAG_MODE </seealso>
		''' <seealso cref= #setDragMode </seealso>
		Public Const OUTLINE_DRAG_MODE As Integer = 1

		Private dragMode As Integer = LIVE_DRAG_MODE
		Private dragModeSet As Boolean = False
		<NonSerialized> _
		Private framesCache As IList(Of JInternalFrame)
		Private componentOrderCheckingEnabled As Boolean = True
		Private componentOrderChanged As Boolean = False

		''' <summary>
		''' Creates a new <code>JDesktopPane</code>.
		''' </summary>
		Public Sub New()
			uIPropertyrty("opaque", Boolean.TRUE)
			focusCycleRoot = True

			focusTraversalPolicyicy(New LayoutFocusTraversalPolicyAnonymousInnerClassHelper
			updateUI()
		End Sub

		Private Class LayoutFocusTraversalPolicyAnonymousInnerClassHelper
			Inherits LayoutFocusTraversalPolicy

			Public Overrides Function getDefaultComponent(ByVal c As java.awt.Container) As java.awt.Component
				Dim jifArray As JInternalFrame() = outerInstance.allFrames
				Dim comp As java.awt.Component = Nothing
				For Each jif As JInternalFrame In jifArray
					comp = jif.focusTraversalPolicy.getDefaultComponent(jif)
					If comp IsNot Nothing Then Exit For
				Next jif
				Return comp
			End Function
		End Class

		''' <summary>
		''' Returns the L&amp;F object that renders this component.
		''' </summary>
		''' <returns> the <code>DesktopPaneUI</code> object that
		'''   renders this component </returns>
		Public Overridable Property uI As DesktopPaneUI
			Get
				Return CType(ui, DesktopPaneUI)
			End Get
			Set(ByVal ui As DesktopPaneUI)
				MyBase.uI = ui
			End Set
		End Property


		''' <summary>
		''' Sets the "dragging style" used by the desktop pane.
		''' You may want to change to one mode or another for
		''' performance or aesthetic reasons.
		''' </summary>
		''' <param name="dragMode"> the style of drag to use for items in the Desktop
		''' </param>
		''' <seealso cref= #LIVE_DRAG_MODE </seealso>
		''' <seealso cref= #OUTLINE_DRAG_MODE
		''' 
		''' @beaninfo
		'''        bound: true
		'''  description: Dragging style for internal frame children.
		'''         enum: LIVE_DRAG_MODE JDesktopPane.LIVE_DRAG_MODE
		'''               OUTLINE_DRAG_MODE JDesktopPane.OUTLINE_DRAG_MODE
		''' @since 1.3 </seealso>
		Public Overridable Property dragMode As Integer
			Set(ByVal dragMode As Integer)
				Dim oldDragMode As Integer = Me.dragMode
				Me.dragMode = dragMode
				firePropertyChange("dragMode", oldDragMode, Me.dragMode)
				dragModeSet = True
			End Set
			Get
				 Return dragMode
			 End Get
		End Property


		''' <summary>
		''' Returns the <code>DesktopManger</code> that handles
		''' desktop-specific UI actions.
		''' </summary>
		Public Overridable Property desktopManager As DesktopManager
			Get
				Return desktopManager
			End Get
			Set(ByVal d As DesktopManager)
				Dim oldValue As DesktopManager = desktopManager
				desktopManager = d
				firePropertyChange("desktopManager", oldValue, desktopManager)
			End Set
		End Property


		''' <summary>
		''' Notification from the <code>UIManager</code> that the L&amp;F has changed.
		''' Replaces the current UI object with the latest version from the
		''' <code>UIManager</code>.
		''' </summary>
		''' <seealso cref= JComponent#updateUI </seealso>
		Public Overrides Sub updateUI()
			uI = CType(UIManager.getUI(Me), DesktopPaneUI)
		End Sub


		''' <summary>
		''' Returns the name of the L&amp;F class that renders this component.
		''' </summary>
		''' <returns> the string "DesktopPaneUI" </returns>
		''' <seealso cref= JComponent#getUIClassID </seealso>
		''' <seealso cref= UIDefaults#getUI </seealso>
		Public Property Overrides uIClassID As String
			Get
				Return uiClassID
			End Get
		End Property

		''' <summary>
		''' Returns all <code>JInternalFrames</code> currently displayed in the
		''' desktop. Returns iconified frames as well as expanded frames.
		''' </summary>
		''' <returns> an array of <code>JInternalFrame</code> objects </returns>
		Public Overridable Property allFrames As JInternalFrame()
			Get
				Return getAllFrames(Me).ToArray(New JInternalFrame(){})
			End Get
		End Property

		Private Shared Function getAllFrames(ByVal parent As java.awt.Container) As ICollection(Of JInternalFrame)
			Dim i, count As Integer
			Dim results As ICollection(Of JInternalFrame) = New java.util.LinkedHashSet(Of JInternalFrame)
			count = parent.componentCount
			For i = 0 To count - 1
				Dim [next] As java.awt.Component = parent.getComponent(i)
				If TypeOf [next] Is JInternalFrame Then
					results.Add(CType([next], JInternalFrame))
				ElseIf TypeOf [next] Is JInternalFrame.JDesktopIcon Then
					Dim tmp As JInternalFrame = CType([next], JInternalFrame.JDesktopIcon).internalFrame
					If tmp IsNot Nothing Then results.Add(tmp)
				ElseIf TypeOf [next] Is java.awt.Container Then
					results.addAll(getAllFrames(CType([next], java.awt.Container)))
				End If
			Next i
			Return results
		End Function

		''' <summary>
		''' Returns the currently active <code>JInternalFrame</code>
		''' in this <code>JDesktopPane</code>, or <code>null</code>
		''' if no <code>JInternalFrame</code> is currently active.
		''' </summary>
		''' <returns> the currently active <code>JInternalFrame</code> or
		'''   <code>null</code>
		''' @since 1.3 </returns>

		Public Overridable Property selectedFrame As JInternalFrame
			Get
			  Return selectedFrame
			End Get
			Set(ByVal f As JInternalFrame)
			  selectedFrame = f
			End Set
		End Property



		''' <summary>
		''' Returns all <code>JInternalFrames</code> currently displayed in the
		''' specified layer of the desktop. Returns iconified frames as well
		''' expanded frames.
		''' </summary>
		''' <param name="layer">  an int specifying the desktop layer </param>
		''' <returns> an array of <code>JInternalFrame</code> objects </returns>
		''' <seealso cref= JLayeredPane </seealso>
		Public Overridable Function getAllFramesInLayer(ByVal layer As Integer) As JInternalFrame()
			Dim ___allFrames As ICollection(Of JInternalFrame) = getAllFrames(Me)
			Dim [iterator] As IEnumerator(Of JInternalFrame) = ___allFrames.GetEnumerator()
			Do While [iterator].MoveNext()
				If [iterator].Current.layer <> layer Then [iterator].remove()
			Loop
			Return ___allFrames.ToArray(New JInternalFrame(){})
		End Function

		Private Property frames As IList(Of JInternalFrame)
			Get
				Dim c As java.awt.Component
				Dim [set] As java.util.Set(Of ComponentPosition) = New SortedSet(Of ComponentPosition)
				For i As Integer = 0 To componentCount - 1
					c = getComponent(i)
					If TypeOf c Is JInternalFrame Then
						[set].add(New ComponentPosition(CType(c, JInternalFrame), getLayer(c), i))
					ElseIf TypeOf c Is JInternalFrame.JDesktopIcon Then
						c = CType(c, JInternalFrame.JDesktopIcon).internalFrame
						[set].add(New ComponentPosition(CType(c, JInternalFrame), getLayer(c), i))
					End If
				Next i
				Dim ___frames As IList(Of JInternalFrame) = New List(Of JInternalFrame)([set].size())
				For Each ___position As ComponentPosition In [set]
					___frames.Add(___position.component)
				Next ___position
				Return ___frames
			End Get
		End Property

		Private Class ComponentPosition
			Implements IComparable(Of ComponentPosition)

			Private ReadOnly component As JInternalFrame
			Private ReadOnly layer As Integer
			Private ReadOnly zOrder As Integer

			Friend Sub New(ByVal component As JInternalFrame, ByVal layer As Integer, ByVal zOrder As Integer)
				Me.component = component
				Me.layer = layer
				Me.zOrder = zOrder
			End Sub

			Public Overridable Function compareTo(ByVal o As ComponentPosition) As Integer
				Dim delta As Integer = o.layer - layer
				If delta = 0 Then Return zOrder - o.zOrder
				Return delta
			End Function
		End Class

		Private Function getNextFrame(ByVal f As JInternalFrame, ByVal forward As Boolean) As JInternalFrame
			verifyFramesCache()
			If f Is Nothing Then Return topInternalFrame
			Dim i As Integer = framesCache.IndexOf(f)
			If i = -1 OrElse framesCache.Count = 1 Then Return Nothing
			If forward Then
				' navigate to the next frame
				i += 1
				If i = framesCache.Count Then i = 0
			Else
				' navigate to the previous frame
				i -= 1
				If i = -1 Then i = framesCache.Count - 1
			End If
			Return framesCache(i)
		End Function

		Friend Overridable Function getNextFrame(ByVal f As JInternalFrame) As JInternalFrame
			Return getNextFrame(f, True)
		End Function

		Private Property topInternalFrame As JInternalFrame
			Get
				If framesCache.Count = 0 Then Return Nothing
				Return framesCache(0)
			End Get
		End Property

		Private Sub updateFramesCache()
			framesCache = frames
		End Sub

		Private Sub verifyFramesCache()
			' If framesCache is dirty, then recreate it.
			If componentOrderChanged Then
				componentOrderChanged = False
				updateFramesCache()
			End If
		End Sub

		''' <summary>
		''' {@inheritDoc}
		''' </summary>
		Public Overrides Sub remove(ByVal comp As java.awt.Component)
			MyBase.remove(comp)
			updateFramesCache()
		End Sub

		''' <summary>
		''' Selects the next <code>JInternalFrame</code> in this desktop pane.
		''' </summary>
		''' <param name="forward"> a boolean indicating which direction to select in;
		'''        <code>true</code> for forward, <code>false</code> for
		'''        backward </param>
		''' <returns> the JInternalFrame that was selected or <code>null</code>
		'''         if nothing was selected
		''' @since 1.6 </returns>
		Public Overridable Function selectFrame(ByVal forward As Boolean) As JInternalFrame
			Dim ___selectedFrame As JInternalFrame = selectedFrame
			Dim frameToSelect As JInternalFrame = getNextFrame(___selectedFrame, forward)
			If frameToSelect Is Nothing Then Return Nothing
			' Maintain navigation traversal order until an
			' external stack change, such as a click on a frame.
			componentOrderCheckingEnabled = False
			If forward AndAlso ___selectedFrame IsNot Nothing Then ___selectedFrame.moveToBack() ' For Windows MDI fidelity.
			Try
				frameToSelect.selected = True
			Catch pve As java.beans.PropertyVetoException
			End Try
			componentOrderCheckingEnabled = True
			Return frameToSelect
		End Function

	'    
	'     * Sets whether component order checking is enabled.
	'     * @param enable a boolean value, where <code>true</code> means
	'     * a change in component order will cause a change in the keyboard
	'     * navigation order.
	'     * @since 1.6
	'     
		Friend Overridable Property componentOrderCheckingEnabled As Boolean
			Set(ByVal enable As Boolean)
				componentOrderCheckingEnabled = enable
			End Set
		End Property

		''' <summary>
		''' {@inheritDoc}
		''' @since 1.6
		''' </summary>
		Protected Friend Overrides Sub addImpl(ByVal comp As java.awt.Component, ByVal constraints As Object, ByVal index As Integer)
			MyBase.addImpl(comp, constraints, index)
			If componentOrderCheckingEnabled Then
				If TypeOf comp Is JInternalFrame OrElse TypeOf comp Is JInternalFrame.JDesktopIcon Then componentOrderChanged = True
			End If
		End Sub

		''' <summary>
		''' {@inheritDoc}
		''' @since 1.6
		''' </summary>
		Public Overrides Sub remove(ByVal index As Integer)
			If componentOrderCheckingEnabled Then
				Dim comp As java.awt.Component = getComponent(index)
				If TypeOf comp Is JInternalFrame OrElse TypeOf comp Is JInternalFrame.JDesktopIcon Then componentOrderChanged = True
			End If
			MyBase.remove(index)
		End Sub

		''' <summary>
		''' {@inheritDoc}
		''' @since 1.6
		''' </summary>
		Public Overrides Sub removeAll()
			If componentOrderCheckingEnabled Then
				Dim count As Integer = componentCount
				For i As Integer = 0 To count - 1
					Dim comp As java.awt.Component = getComponent(i)
					If TypeOf comp Is JInternalFrame OrElse TypeOf comp Is JInternalFrame.JDesktopIcon Then
						componentOrderChanged = True
						Exit For
					End If
				Next i
			End If
			MyBase.removeAll()
		End Sub

		''' <summary>
		''' {@inheritDoc}
		''' @since 1.6
		''' </summary>
		Public Overridable Sub setComponentZOrder(ByVal comp As java.awt.Component, ByVal index As Integer)
			MyBase.componentZOrderder(comp, index)
			If componentOrderCheckingEnabled Then
				If TypeOf comp Is JInternalFrame OrElse TypeOf comp Is JInternalFrame.JDesktopIcon Then componentOrderChanged = True
			End If
		End Sub

		''' <summary>
		''' See readObject() and writeObject() in JComponent for more
		''' information about serialization in Swing.
		''' </summary>
		Private Sub writeObject(ByVal s As java.io.ObjectOutputStream)
			s.defaultWriteObject()
			If uIClassID.Equals(uiClassID) Then
				Dim count As SByte = JComponent.getWriteObjCounter(Me)
				count -= 1
				JComponent.writeObjCounterter(Me, count)
				If count = 0 AndAlso ui IsNot Nothing Then ui.installUI(Me)
			End If
		End Sub

		Friend Overrides Sub setUIProperty(ByVal propertyName As String, ByVal value As Object)
			If propertyName = "dragMode" Then
				If Not dragModeSet Then
					dragMode = CInt(Fix(value))
					dragModeSet = False
				End If
			Else
				MyBase.uIPropertyrty(propertyName, value)
			End If
		End Sub

		''' <summary>
		''' Returns a string representation of this <code>JDesktopPane</code>.
		''' This method is intended to be used only for debugging purposes, and the
		''' content and format of the returned string may vary between
		''' implementations. The returned string may be empty but may not
		''' be <code>null</code>.
		''' </summary>
		''' <returns>  a string representation of this <code>JDesktopPane</code> </returns>
		Protected Friend Overrides Function paramString() As String
			Dim desktopManagerString As String = (If(desktopManager IsNot Nothing, desktopManager.ToString(), ""))

			Return MyBase.paramString() & ",desktopManager=" & desktopManagerString
		End Function

	'///////////////
	' Accessibility support
	'//////////////

		''' <summary>
		''' Gets the <code>AccessibleContext</code> associated with this
		''' <code>JDesktopPane</code>. For desktop panes, the
		''' <code>AccessibleContext</code> takes the form of an
		''' <code>AccessibleJDesktopPane</code>.
		''' A new <code>AccessibleJDesktopPane</code> instance is created if necessary.
		''' </summary>
		''' <returns> an <code>AccessibleJDesktopPane</code> that serves as the
		'''         <code>AccessibleContext</code> of this <code>JDesktopPane</code> </returns>
		Public Property Overrides accessibleContext As AccessibleContext Implements Accessible.getAccessibleContext
			Get
				If accessibleContext Is Nothing Then accessibleContext = New AccessibleJDesktopPane(Me)
				Return accessibleContext
			End Get
		End Property

		''' <summary>
		''' This class implements accessibility support for the
		''' <code>JDesktopPane</code> class.  It provides an implementation of the
		''' Java Accessibility API appropriate to desktop pane user-interface
		''' elements.
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
		Protected Friend Class AccessibleJDesktopPane
			Inherits AccessibleJComponent

			Private ReadOnly outerInstance As JDesktopPane

			Public Sub New(ByVal outerInstance As JDesktopPane)
				Me.outerInstance = outerInstance
			End Sub


			''' <summary>
			''' Get the role of this object.
			''' </summary>
			''' <returns> an instance of AccessibleRole describing the role of the
			''' object </returns>
			''' <seealso cref= AccessibleRole </seealso>
			Public Overridable Property accessibleRole As AccessibleRole
				Get
					Return AccessibleRole.DESKTOP_PANE
				End Get
			End Property
		End Class
	End Class

End Namespace