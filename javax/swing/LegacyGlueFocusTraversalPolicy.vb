Imports System
Imports System.Collections.Generic

'
' * Copyright (c) 2000, 2008, Oracle and/or its affiliates. All rights reserved.
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
	''' A FocusTraversalPolicy which provides support for legacy applications which
	''' handle focus traversal via JComponent.setNextFocusableComponent or by
	''' installing a custom DefaultFocusManager. If a specific traversal has not
	''' been hard coded, then that traversal is provided either by the custom
	''' DefaultFocusManager, or by a wrapped FocusTraversalPolicy instance.
	''' 
	''' @author David Mendenhall
	''' </summary>
	<Serializable> _
	Friend NotInheritable Class LegacyGlueFocusTraversalPolicy
		Inherits java.awt.FocusTraversalPolicy

		<NonSerialized> _
		Private delegatePolicy As java.awt.FocusTraversalPolicy
		<NonSerialized> _
		Private delegateManager As DefaultFocusManager

		Private forwardMap As New Dictionary(Of java.awt.Component, java.awt.Component), backwardMap As New Dictionary(Of java.awt.Component, java.awt.Component)

		Friend Sub New(ByVal delegatePolicy As java.awt.FocusTraversalPolicy)
			Me.delegatePolicy = delegatePolicy
		End Sub
		Friend Sub New(ByVal delegateManager As DefaultFocusManager)
			Me.delegateManager = delegateManager
		End Sub

		Friend Sub setNextFocusableComponent(ByVal left As java.awt.Component, ByVal right As java.awt.Component)
			forwardMap(left) = right
			backwardMap(right) = left
		End Sub
		Friend Sub unsetNextFocusableComponent(ByVal left As java.awt.Component, ByVal right As java.awt.Component)
			forwardMap.Remove(left)
			backwardMap.Remove(right)
		End Sub

		Public Function getComponentAfter(ByVal focusCycleRoot As java.awt.Container, ByVal aComponent As java.awt.Component) As java.awt.Component
			Dim hardCoded As java.awt.Component = aComponent, prevHardCoded As java.awt.Component
			Dim sanity As New HashSet(Of java.awt.Component)

			Do
				prevHardCoded = hardCoded
				hardCoded = forwardMap(hardCoded)
				If hardCoded Is Nothing Then
					If delegatePolicy IsNot Nothing AndAlso prevHardCoded.isFocusCycleRoot(focusCycleRoot) Then
						Return delegatePolicy.getComponentAfter(focusCycleRoot, prevHardCoded)
					ElseIf delegateManager IsNot Nothing Then
						Return delegateManager.getComponentAfter(focusCycleRoot, aComponent)
					Else
						Return Nothing
					End If
				End If
				If sanity.Contains(hardCoded) Then Return Nothing
				sanity.Add(hardCoded)
			Loop While Not accept(hardCoded)

			Return hardCoded
		End Function
		Public Function getComponentBefore(ByVal focusCycleRoot As java.awt.Container, ByVal aComponent As java.awt.Component) As java.awt.Component
			Dim hardCoded As java.awt.Component = aComponent, prevHardCoded As java.awt.Component
			Dim sanity As New HashSet(Of java.awt.Component)

			Do
				prevHardCoded = hardCoded
				hardCoded = backwardMap(hardCoded)
				If hardCoded Is Nothing Then
					If delegatePolicy IsNot Nothing AndAlso prevHardCoded.isFocusCycleRoot(focusCycleRoot) Then
						Return delegatePolicy.getComponentBefore(focusCycleRoot, prevHardCoded)
					ElseIf delegateManager IsNot Nothing Then
						Return delegateManager.getComponentBefore(focusCycleRoot, aComponent)
					Else
						Return Nothing
					End If
				End If
				If sanity.Contains(hardCoded) Then Return Nothing
				sanity.Add(hardCoded)
			Loop While Not accept(hardCoded)

			Return hardCoded
		End Function
		Public Function getFirstComponent(ByVal focusCycleRoot As java.awt.Container) As java.awt.Component
			If delegatePolicy IsNot Nothing Then
				Return delegatePolicy.getFirstComponent(focusCycleRoot)
			ElseIf delegateManager IsNot Nothing Then
				Return delegateManager.getFirstComponent(focusCycleRoot)
			Else
				Return Nothing
			End If
		End Function
		Public Function getLastComponent(ByVal focusCycleRoot As java.awt.Container) As java.awt.Component
			If delegatePolicy IsNot Nothing Then
				Return delegatePolicy.getLastComponent(focusCycleRoot)
			ElseIf delegateManager IsNot Nothing Then
				Return delegateManager.getLastComponent(focusCycleRoot)
			Else
				Return Nothing
			End If
		End Function
		Public Function getDefaultComponent(ByVal focusCycleRoot As java.awt.Container) As java.awt.Component
			If delegatePolicy IsNot Nothing Then
				Return delegatePolicy.getDefaultComponent(focusCycleRoot)
			Else
				Return getFirstComponent(focusCycleRoot)
			End If
		End Function
		Private Function accept(ByVal aComponent As java.awt.Component) As Boolean
			If Not(aComponent.visible AndAlso aComponent.displayable AndAlso aComponent.focusable AndAlso aComponent.enabled) Then Return False

			' Verify that the Component is recursively enabled. Disabling a
			' heavyweight Container disables its children, whereas disabling
			' a lightweight Container does not.
			If Not(TypeOf aComponent Is java.awt.Window) Then
				Dim enableTest As java.awt.Container = aComponent.parent
				Do While enableTest IsNot Nothing
					If Not(enableTest.enabled OrElse enableTest.lightweight) Then Return False
					If TypeOf enableTest Is java.awt.Window Then Exit Do
					enableTest = enableTest.parent
				Loop
			End If

			Return True
		End Function
		Private Sub writeObject(ByVal out As ObjectOutputStream)
			out.defaultWriteObject()

			If TypeOf delegatePolicy Is Serializable Then
				out.writeObject(delegatePolicy)
			Else
				out.writeObject(Nothing)
			End If

			If TypeOf delegateManager Is Serializable Then
				out.writeObject(delegateManager)
			Else
				out.writeObject(Nothing)
			End If
		End Sub
		Private Sub readObject(ByVal [in] As ObjectInputStream)
			[in].defaultReadObject()
			delegatePolicy = CType([in].readObject(), java.awt.FocusTraversalPolicy)
			delegateManager = CType([in].readObject(), DefaultFocusManager)
		End Sub
	End Class

End Namespace