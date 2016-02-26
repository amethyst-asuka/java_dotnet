Imports System

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
	''' A SortingFocusTraversalPolicy which sorts Components based on their size,
	''' position, and orientation. Based on their size and position, Components are
	''' roughly categorized into rows and columns. For a Container with horizontal
	''' orientation, columns run left-to-right or right-to-left, and rows run top-
	''' to-bottom. For a Container with vertical orientation, columns run top-to-
	''' bottom and rows run left-to-right or right-to-left. See
	''' <code>ComponentOrientation</code> for more information. All columns in a
	''' row are fully traversed before proceeding to the next row.
	''' 
	''' @author David Mendenhall
	''' </summary>
	''' <seealso cref= java.awt.ComponentOrientation
	''' @since 1.4 </seealso>
	<Serializable> _
	Public Class LayoutFocusTraversalPolicy
		Inherits SortingFocusTraversalPolicy

		' Delegate most of our fitness test to Default so that we only have to
		' code the algorithm once.
		Private Shared ReadOnly fitnessTestPolicy As New SwingDefaultFocusTraversalPolicy

		''' <summary>
		''' Constructs a LayoutFocusTraversalPolicy.
		''' </summary>
		Public Sub New()
			MyBase.New(New LayoutComparator)
		End Sub

		''' <summary>
		''' Constructs a LayoutFocusTraversalPolicy with the passed in
		''' <code>Comparator</code>.
		''' </summary>
'JAVA TO VB CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
		Friend Sub New(Of T1)(ByVal c As IComparer(Of T1))
			MyBase.New(c)
		End Sub

		''' <summary>
		''' Returns the Component that should receive the focus after aComponent.
		''' aContainer must be a focus cycle root of aComponent.
		''' <p>
		''' By default, LayoutFocusTraversalPolicy implicitly transfers focus down-
		''' cycle. That is, during normal focus traversal, the Component
		''' traversed after a focus cycle root will be the focus-cycle-root's
		''' default Component to focus. This behavior can be disabled using the
		''' <code>setImplicitDownCycleTraversal</code> method.
		''' <p>
		''' If aContainer is <a href="../../java/awt/doc-files/FocusSpec.html#FocusTraversalPolicyProviders">focus
		''' traversal policy provider</a>, the focus is always transferred down-cycle.
		''' </summary>
		''' <param name="aContainer"> a focus cycle root of aComponent or a focus traversal policy provider </param>
		''' <param name="aComponent"> a (possibly indirect) child of aContainer, or
		'''        aContainer itself </param>
		''' <returns> the Component that should receive the focus after aComponent, or
		'''         null if no suitable Component can be found </returns>
		''' <exception cref="IllegalArgumentException"> if aContainer is not a focus cycle
		'''         root of aComponent or a focus traversal policy provider, or if either aContainer or
		'''         aComponent is null </exception>
		Public Overrides Function getComponentAfter(ByVal aContainer As java.awt.Container, ByVal aComponent As java.awt.Component) As java.awt.Component
			If aContainer Is Nothing OrElse aComponent Is Nothing Then Throw New System.ArgumentException("aContainer and aComponent cannot be null")
			Dim ___comparator As IComparer = comparator
			If TypeOf ___comparator Is LayoutComparator Then CType(___comparator, LayoutComparator).componentOrientation = aContainer.componentOrientation
			Return MyBase.getComponentAfter(aContainer, aComponent)
		End Function

		''' <summary>
		''' Returns the Component that should receive the focus before aComponent.
		''' aContainer must be a focus cycle root of aComponent.
		''' <p>
		''' By default, LayoutFocusTraversalPolicy implicitly transfers focus down-
		''' cycle. That is, during normal focus traversal, the Component
		''' traversed after a focus cycle root will be the focus-cycle-root's
		''' default Component to focus. This behavior can be disabled using the
		''' <code>setImplicitDownCycleTraversal</code> method.
		''' <p>
		''' If aContainer is <a href="../../java/awt/doc-files/FocusSpec.html#FocusTraversalPolicyProviders">focus
		''' traversal policy provider</a>, the focus is always transferred down-cycle.
		''' </summary>
		''' <param name="aContainer"> a focus cycle root of aComponent or a focus traversal policy provider </param>
		''' <param name="aComponent"> a (possibly indirect) child of aContainer, or
		'''        aContainer itself </param>
		''' <returns> the Component that should receive the focus before aComponent,
		'''         or null if no suitable Component can be found </returns>
		''' <exception cref="IllegalArgumentException"> if aContainer is not a focus cycle
		'''         root of aComponent or a focus traversal policy provider, or if either aContainer or
		'''         aComponent is null </exception>
		Public Overrides Function getComponentBefore(ByVal aContainer As java.awt.Container, ByVal aComponent As java.awt.Component) As java.awt.Component
			If aContainer Is Nothing OrElse aComponent Is Nothing Then Throw New System.ArgumentException("aContainer and aComponent cannot be null")
			Dim ___comparator As IComparer = comparator
			If TypeOf ___comparator Is LayoutComparator Then CType(___comparator, LayoutComparator).componentOrientation = aContainer.componentOrientation
			Return MyBase.getComponentBefore(aContainer, aComponent)
		End Function

		''' <summary>
		''' Returns the first Component in the traversal cycle. This method is used
		''' to determine the next Component to focus when traversal wraps in the
		''' forward direction.
		''' </summary>
		''' <param name="aContainer"> a focus cycle root of aComponent or a focus traversal policy provider whose
		'''        first Component is to be returned </param>
		''' <returns> the first Component in the traversal cycle of aContainer,
		'''         or null if no suitable Component can be found </returns>
		''' <exception cref="IllegalArgumentException"> if aContainer is null </exception>
		Public Overrides Function getFirstComponent(ByVal aContainer As java.awt.Container) As java.awt.Component
			If aContainer Is Nothing Then Throw New System.ArgumentException("aContainer cannot be null")
			Dim ___comparator As IComparer = comparator
			If TypeOf ___comparator Is LayoutComparator Then CType(___comparator, LayoutComparator).componentOrientation = aContainer.componentOrientation
			Return MyBase.getFirstComponent(aContainer)
		End Function

		''' <summary>
		''' Returns the last Component in the traversal cycle. This method is used
		''' to determine the next Component to focus when traversal wraps in the
		''' reverse direction.
		''' </summary>
		''' <param name="aContainer"> a focus cycle root of aComponent or a focus traversal policy provider whose
		'''        last Component is to be returned </param>
		''' <returns> the last Component in the traversal cycle of aContainer,
		'''         or null if no suitable Component can be found </returns>
		''' <exception cref="IllegalArgumentException"> if aContainer is null </exception>
		Public Overrides Function getLastComponent(ByVal aContainer As java.awt.Container) As java.awt.Component
			If aContainer Is Nothing Then Throw New System.ArgumentException("aContainer cannot be null")
			Dim ___comparator As IComparer = comparator
			If TypeOf ___comparator Is LayoutComparator Then CType(___comparator, LayoutComparator).componentOrientation = aContainer.componentOrientation
			Return MyBase.getLastComponent(aContainer)
		End Function

		''' <summary>
		''' Determines whether the specified <code>Component</code>
		''' is an acceptable choice as the new focus owner.
		''' This method performs the following sequence of operations:
		''' <ol>
		''' <li>Checks whether <code>aComponent</code> is visible, displayable,
		'''     enabled, and focusable.  If any of these properties is
		'''     <code>false</code>, this method returns <code>false</code>.
		''' <li>If <code>aComponent</code> is an instance of <code>JTable</code>,
		'''     returns <code>true</code>.
		''' <li>If <code>aComponent</code> is an instance of <code>JComboBox</code>,
		'''     then returns the value of
		'''     <code>aComponent.getUI().isFocusTraversable(aComponent)</code>.
		''' <li>If <code>aComponent</code> is a <code>JComponent</code>
		'''     with a <code>JComponent.WHEN_FOCUSED</code>
		'''     <code>InputMap</code> that is neither <code>null</code>
		'''     nor empty, returns <code>true</code>.
		''' <li>Returns the value of
		'''     <code>DefaultFocusTraversalPolicy.accept(aComponent)</code>.
		''' </ol>
		''' </summary>
		''' <param name="aComponent"> the <code>Component</code> whose fitness
		'''                   as a focus owner is to be tested </param>
		''' <seealso cref= java.awt.Component#isVisible </seealso>
		''' <seealso cref= java.awt.Component#isDisplayable </seealso>
		''' <seealso cref= java.awt.Component#isEnabled </seealso>
		''' <seealso cref= java.awt.Component#isFocusable </seealso>
		''' <seealso cref= javax.swing.plaf.ComboBoxUI#isFocusTraversable </seealso>
		''' <seealso cref= javax.swing.JComponent#getInputMap </seealso>
		''' <seealso cref= java.awt.DefaultFocusTraversalPolicy#accept </seealso>
		''' <returns> <code>true</code> if <code>aComponent</code> is a valid choice
		'''         for a focus owner;
		'''         otherwise <code>false</code> </returns>
		 Protected Friend Overrides Function accept(ByVal aComponent As java.awt.Component) As Boolean
			If Not MyBase.accept(aComponent) Then
				Return False
			ElseIf sun.awt.SunToolkit.isInstanceOf(aComponent, "javax.swing.JTable") Then
				' JTable only has ancestor focus bindings, we thus force it
				' to be focusable by returning true here.
				Return True
			ElseIf sun.awt.SunToolkit.isInstanceOf(aComponent, "javax.swing.JComboBox") Then
				Dim ___box As JComboBox = CType(aComponent, JComboBox)
				Return ___box.uI.isFocusTraversable(___box)
			ElseIf TypeOf aComponent Is JComponent Then
				Dim ___jComponent As JComponent = CType(aComponent, JComponent)
				Dim inputMap As InputMap = ___jComponent.getInputMap(JComponent.WHEN_FOCUSED, False)
				Do While inputMap IsNot Nothing AndAlso inputMap.size() = 0
					inputMap = inputMap.parent
				Loop
				If inputMap IsNot Nothing Then Return True
				' Delegate to the fitnessTestPolicy, this will test for the
				' case where the developer has overriden isFocusTraversable to
				' return true.
			End If
			Return fitnessTestPolicy.accept(aComponent)
		 End Function

		Private Sub writeObject(ByVal out As ObjectOutputStream)
			out.writeObject(comparator)
			out.writeBoolean(implicitDownCycleTraversal)
		End Sub
		Private Sub readObject(ByVal [in] As ObjectInputStream)
			comparator = CType([in].readObject(), IComparer)
			implicitDownCycleTraversal = [in].readBoolean()
		End Sub
	End Class

	' Create our own subclass and change accept to public so that we can call
	' accept.
	Friend Class SwingDefaultFocusTraversalPolicy
		Inherits java.awt.DefaultFocusTraversalPolicy

		Public Overridable Function accept(ByVal aComponent As java.awt.Component) As Boolean
			Return MyBase.accept(aComponent)
		End Function
	End Class

End Namespace