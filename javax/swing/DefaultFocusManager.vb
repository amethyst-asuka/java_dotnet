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
	''' This class has been obsoleted by the 1.4 focus APIs. While client code may
	''' still use this class, developers are strongly encouraged to use
	''' <code>java.awt.KeyboardFocusManager</code> and
	''' <code>java.awt.DefaultKeyboardFocusManager</code> instead.
	''' <p>
	''' Please see
	''' <a href="https://docs.oracle.com/javase/tutorial/uiswing/misc/focus.html">
	''' How to Use the Focus Subsystem</a>,
	''' a section in <em>The Java Tutorial</em>, and the
	''' <a href="../../java/awt/doc-files/FocusSpec.html">Focus Specification</a>
	''' for more information.
	''' 
	''' @author Arnaud Weber
	''' @author David Mendenhall
	''' </summary>
	Public Class DefaultFocusManager
		Inherits FocusManager

		Friend ReadOnly gluePolicy As java.awt.FocusTraversalPolicy = New LegacyGlueFocusTraversalPolicy(Me)
		Private ReadOnly layoutPolicy As java.awt.FocusTraversalPolicy = New LegacyLayoutFocusTraversalPolicy(Me)
		Private ReadOnly comparator As New LayoutComparator

		Public Sub New()
			defaultFocusTraversalPolicy = gluePolicy
		End Sub

		Public Overridable Function getComponentAfter(ByVal aContainer As java.awt.Container, ByVal aComponent As java.awt.Component) As java.awt.Component
			Dim root As java.awt.Container = If(aContainer.focusCycleRoot, aContainer, aContainer.focusCycleRootAncestor)

			' Support for mixed 1.4/pre-1.4 focus APIs. If a particular root's
			' traversal policy is non-legacy, then honor it.
			If root IsNot Nothing Then
				Dim policy As java.awt.FocusTraversalPolicy = root.focusTraversalPolicy
				If policy IsNot gluePolicy Then Return policy.getComponentAfter(root, aComponent)

				comparator.componentOrientation = root.componentOrientation
				Return layoutPolicy.getComponentAfter(root, aComponent)
			End If

			Return Nothing
		End Function

		Public Overridable Function getComponentBefore(ByVal aContainer As java.awt.Container, ByVal aComponent As java.awt.Component) As java.awt.Component
			Dim root As java.awt.Container = If(aContainer.focusCycleRoot, aContainer, aContainer.focusCycleRootAncestor)

			' Support for mixed 1.4/pre-1.4 focus APIs. If a particular root's
			' traversal policy is non-legacy, then honor it.
			If root IsNot Nothing Then
				Dim policy As java.awt.FocusTraversalPolicy = root.focusTraversalPolicy
				If policy IsNot gluePolicy Then Return policy.getComponentBefore(root, aComponent)

				comparator.componentOrientation = root.componentOrientation
				Return layoutPolicy.getComponentBefore(root, aComponent)
			End If

			Return Nothing
		End Function

		Public Overridable Function getFirstComponent(ByVal aContainer As java.awt.Container) As java.awt.Component
			Dim root As java.awt.Container = If(aContainer.focusCycleRoot, aContainer, aContainer.focusCycleRootAncestor)

			' Support for mixed 1.4/pre-1.4 focus APIs. If a particular root's
			' traversal policy is non-legacy, then honor it.
			If root IsNot Nothing Then
				Dim policy As java.awt.FocusTraversalPolicy = root.focusTraversalPolicy
				If policy IsNot gluePolicy Then Return policy.getFirstComponent(root)

				comparator.componentOrientation = root.componentOrientation
				Return layoutPolicy.getFirstComponent(root)
			End If

			Return Nothing
		End Function

		Public Overridable Function getLastComponent(ByVal aContainer As java.awt.Container) As java.awt.Component
			Dim root As java.awt.Container = If(aContainer.focusCycleRoot, aContainer, aContainer.focusCycleRootAncestor)

			' Support for mixed 1.4/pre-1.4 focus APIs. If a particular root's
			' traversal policy is non-legacy, then honor it.
			If root IsNot Nothing Then
				Dim policy As java.awt.FocusTraversalPolicy = root.focusTraversalPolicy
				If policy IsNot gluePolicy Then Return policy.getLastComponent(root)

				comparator.componentOrientation = root.componentOrientation
				Return layoutPolicy.getLastComponent(root)
			End If

			Return Nothing
		End Function

		Public Overridable Function compareTabOrder(ByVal a As java.awt.Component, ByVal b As java.awt.Component) As Boolean
			Return (comparator.Compare(a, b) < 0)
		End Function
	End Class

	Friend NotInheritable Class LegacyLayoutFocusTraversalPolicy
		Inherits LayoutFocusTraversalPolicy

		Friend Sub New(ByVal defaultFocusManager As DefaultFocusManager)
			MyBase.New(New CompareTabOrderComparator(defaultFocusManager))
		End Sub
	End Class

	Friend NotInheritable Class CompareTabOrderComparator
		Implements IComparer(Of java.awt.Component)

		Private ReadOnly defaultFocusManager As DefaultFocusManager

		Friend Sub New(ByVal defaultFocusManager As DefaultFocusManager)
			Me.defaultFocusManager = defaultFocusManager
		End Sub

		Public Function compare(ByVal o1 As java.awt.Component, ByVal o2 As java.awt.Component) As Integer
			If o1 Is o2 Then Return 0
			Return If(defaultFocusManager.compareTabOrder(o1, o2), -1, 1)
		End Function
	End Class

End Namespace