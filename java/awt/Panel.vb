Imports javax.accessibility

'
' * Copyright (c) 1995, 2007, Oracle and/or its affiliates. All rights reserved.
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
Namespace java.awt

	''' <summary>
	''' <code>Panel</code> is the simplest container class. A panel
	''' provides space in which an application can attach any other
	''' component, including other panels.
	''' <p>
	''' The default layout manager for a panel is the
	''' <code>FlowLayout</code> layout manager.
	''' 
	''' @author      Sami Shaio </summary>
	''' <seealso cref=     java.awt.FlowLayout
	''' @since   JDK1.0 </seealso>
	Public Class Panel
		Inherits Container
		Implements Accessible

		Private Const base As String = "panel"
		Private Shared nameCounter As Integer = 0

	'    
	'     * JDK 1.1 serialVersionUID
	'     
		 Private Const serialVersionUID As Long = -2728009084054400034L

		''' <summary>
		''' Creates a new panel using the default layout manager.
		''' The default layout manager for all panels is the
		''' <code>FlowLayout</code> class.
		''' </summary>
		Public Sub New()
			Me.New(New FlowLayout)
		End Sub

		''' <summary>
		''' Creates a new panel with the specified layout manager. </summary>
		''' <param name="layout"> the layout manager for this panel.
		''' @since JDK1.1 </param>
		Public Sub New(ByVal layout As LayoutManager)
			layout = layout
		End Sub

		''' <summary>
		''' Construct a name for this component.  Called by getName() when the
		''' name is null.
		''' </summary>
		Friend Overrides Function constructComponentName() As String
			SyncLock GetType(Panel)
					Dim tempVar As Integer = nameCounter
					nameCounter += 1
					Return base + tempVar
			End SyncLock
		End Function

		''' <summary>
		''' Creates the Panel's peer.  The peer allows you to modify the
		''' appearance of the panel without changing its functionality.
		''' </summary>

		Public Overrides Sub addNotify()
			SyncLock treeLock
				If peer Is Nothing Then peer = toolkit.createPanel(Me)
				MyBase.addNotify()
			End SyncLock
		End Sub

	'///////////////
	' Accessibility support
	'//////////////

		''' <summary>
		''' Gets the AccessibleContext associated with this Panel.
		''' For panels, the AccessibleContext takes the form of an
		''' AccessibleAWTPanel.
		''' A new AccessibleAWTPanel instance is created if necessary.
		''' </summary>
		''' <returns> an AccessibleAWTPanel that serves as the
		'''         AccessibleContext of this Panel
		''' @since 1.3 </returns>
		Public Property Overrides accessibleContext As AccessibleContext
			Get
				If accessibleContext Is Nothing Then accessibleContext = New AccessibleAWTPanel(Me)
				Return accessibleContext
			End Get
		End Property

		''' <summary>
		''' This class implements accessibility support for the
		''' <code>Panel</code> class.  It provides an implementation of the
		''' Java Accessibility API appropriate to panel user-interface elements.
		''' @since 1.3
		''' </summary>
		Protected Friend Class AccessibleAWTPanel
			Inherits AccessibleAWTContainer

			Private ReadOnly outerInstance As Panel

			Public Sub New(ByVal outerInstance As Panel)
				Me.outerInstance = outerInstance
			End Sub


			Private Const serialVersionUID As Long = -6409552226660031050L

			''' <summary>
			''' Get the role of this object.
			''' </summary>
			''' <returns> an instance of AccessibleRole describing the role of the
			''' object </returns>
			Public Overridable Property accessibleRole As AccessibleRole
				Get
					Return AccessibleRole.PANEL
				End Get
			End Property
		End Class

	End Class

End Namespace