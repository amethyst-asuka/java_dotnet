'
' * Copyright (c) 2001, 2004, Oracle and/or its affiliates. All rights reserved.
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
	''' Provides a javax.swing.DefaultFocusManager view onto an arbitrary
	''' java.awt.KeyboardFocusManager. We subclass DefaultFocusManager instead of
	''' FocusManager because it seems more backward-compatible. It is likely that
	''' some pre-1.4 code assumes that the object returned by
	''' FocusManager.getCurrentManager is an instance of DefaultFocusManager unless
	''' set explicitly.
	''' </summary>
	Friend NotInheritable Class DelegatingDefaultFocusManager
		Inherits DefaultFocusManager

		Private ReadOnly [delegate] As KeyboardFocusManager

		Friend Sub New(ByVal [delegate] As KeyboardFocusManager)
			Me.delegate = [delegate]
			defaultFocusTraversalPolicy = gluePolicy
		End Sub

		Friend Property [delegate] As KeyboardFocusManager
			Get
				Return [delegate]
			End Get
		End Property

		' Legacy methods which first appeared in javax.swing.FocusManager.
		' Client code is most likely to invoke these methods.

		Public Sub processKeyEvent(ByVal focusedComponent As Component, ByVal e As KeyEvent)
			[delegate].processKeyEvent(focusedComponent, e)
		End Sub
		Public Sub focusNextComponent(ByVal aComponent As Component)
			[delegate].focusNextComponent(aComponent)
		End Sub
		Public Sub focusPreviousComponent(ByVal aComponent As Component)
			[delegate].focusPreviousComponent(aComponent)
		End Sub

		' Make sure that we delegate all new methods in KeyboardFocusManager
		' as well as the legacy methods from Swing. It is theoretically possible,
		' although unlikely, that a client app will treat this instance as a
		' new-style KeyboardFocusManager. We might as well be safe.
		'
		' The JLS won't let us override the protected methods in
		' KeyboardFocusManager such that they invoke the corresponding methods on
		' the delegate. However, since client code would never be able to call
		' those methods anyways, we don't have to worry about that problem.

		Public Property focusOwner As Component
			Get
				Return [delegate].focusOwner
			End Get
		End Property
		Public Sub clearGlobalFocusOwner()
			[delegate].clearGlobalFocusOwner()
		End Sub
		Public Property permanentFocusOwner As Component
			Get
				Return [delegate].permanentFocusOwner
			End Get
		End Property
		Public Property focusedWindow As Window
			Get
				Return [delegate].focusedWindow
			End Get
		End Property
		Public Property activeWindow As Window
			Get
				Return [delegate].activeWindow
			End Get
		End Property
		Public Property defaultFocusTraversalPolicy As FocusTraversalPolicy
			Get
				Return [delegate].defaultFocusTraversalPolicy
			End Get
			Set(ByVal defaultPolicy As FocusTraversalPolicy)
				If [delegate] IsNot Nothing Then [delegate].defaultFocusTraversalPolicy = defaultPolicy
			End Set
		End Property
		Public Sub setDefaultFocusTraversalKeys(Of T1 As AWTKeyStroke)(ByVal id As Integer, ByVal keystrokes As java.util.Set(Of T1))
			[delegate].defaultFocusTraversalKeyseys(id, keystrokes)
		End Sub
		Public Function getDefaultFocusTraversalKeys(ByVal id As Integer) As java.util.Set(Of AWTKeyStroke)
			Return [delegate].getDefaultFocusTraversalKeys(id)
		End Function
		Public Property currentFocusCycleRoot As Container
			Get
				Return [delegate].currentFocusCycleRoot
			End Get
		End Property
		Public Property globalCurrentFocusCycleRoot As Container
			Set(ByVal newFocusCycleRoot As Container)
				[delegate].globalCurrentFocusCycleRoot = newFocusCycleRoot
			End Set
		End Property
		Public Sub addPropertyChangeListener(ByVal listener As PropertyChangeListener)
			[delegate].addPropertyChangeListener(listener)
		End Sub
		Public Sub removePropertyChangeListener(ByVal listener As PropertyChangeListener)
			[delegate].removePropertyChangeListener(listener)
		End Sub
		Public Sub addPropertyChangeListener(ByVal propertyName As String, ByVal listener As PropertyChangeListener)
			[delegate].addPropertyChangeListener(propertyName, listener)
		End Sub
		Public Sub removePropertyChangeListener(ByVal propertyName As String, ByVal listener As PropertyChangeListener)
			[delegate].removePropertyChangeListener(propertyName, listener)
		End Sub
		Public Sub addVetoableChangeListener(ByVal listener As VetoableChangeListener)
			[delegate].addVetoableChangeListener(listener)
		End Sub
		Public Sub removeVetoableChangeListener(ByVal listener As VetoableChangeListener)
			[delegate].removeVetoableChangeListener(listener)
		End Sub
		Public Sub addVetoableChangeListener(ByVal propertyName As String, ByVal listener As VetoableChangeListener)
			[delegate].addVetoableChangeListener(propertyName, listener)
		End Sub
		Public Sub removeVetoableChangeListener(ByVal propertyName As String, ByVal listener As VetoableChangeListener)
			[delegate].removeVetoableChangeListener(propertyName, listener)
		End Sub
		Public Sub addKeyEventDispatcher(ByVal dispatcher As KeyEventDispatcher)
			[delegate].addKeyEventDispatcher(dispatcher)
		End Sub
		Public Sub removeKeyEventDispatcher(ByVal dispatcher As KeyEventDispatcher)
			[delegate].removeKeyEventDispatcher(dispatcher)
		End Sub
		Public Function dispatchEvent(ByVal e As AWTEvent) As Boolean
			Return [delegate].dispatchEvent(e)
		End Function
		Public Function dispatchKeyEvent(ByVal e As KeyEvent) As Boolean
			Return [delegate].dispatchKeyEvent(e)
		End Function
		Public Sub upFocusCycle(ByVal aComponent As Component)
			[delegate].upFocusCycle(aComponent)
		End Sub
		Public Sub downFocusCycle(ByVal aContainer As Container)
			[delegate].downFocusCycle(aContainer)
		End Sub
	End Class

End Namespace