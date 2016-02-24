Imports System
Imports System.Collections.Generic

'
' * Copyright (c) 2000, 2013, Oracle and/or its affiliates. All rights reserved.
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
	''' A FocusTraversalPolicy that determines traversal order based on the order
	''' of child Components in a Container. From a particular focus cycle root, the
	''' policy makes a pre-order traversal of the Component hierarchy, and traverses
	''' a Container's children according to the ordering of the array returned by
	''' <code>Container.getComponents()</code>. Portions of the hierarchy that are
	''' not visible and displayable will not be searched.
	''' <p>
	''' By default, ContainerOrderFocusTraversalPolicy implicitly transfers focus
	''' down-cycle. That is, during normal forward focus traversal, the Component
	''' traversed after a focus cycle root will be the focus-cycle-root's default
	''' Component to focus. This behavior can be disabled using the
	''' <code>setImplicitDownCycleTraversal</code> method.
	''' <p>
	''' By default, methods of this class will return a Component only if it is
	''' visible, displayable, enabled, and focusable. Subclasses can modify this
	''' behavior by overriding the <code>accept</code> method.
	''' <p>
	''' This policy takes into account <a
	''' href="doc-files/FocusSpec.html#FocusTraversalPolicyProviders">focus traversal
	''' policy providers</a>.  When searching for first/last/next/previous Component,
	''' if a focus traversal policy provider is encountered, its focus traversal
	''' policy is used to perform the search operation.
	''' 
	''' @author David Mendenhall
	''' </summary>
	''' <seealso cref= Container#getComponents
	''' @since 1.4 </seealso>
	<Serializable> _
	Public Class ContainerOrderFocusTraversalPolicy
		Inherits FocusTraversalPolicy

		Private Shared ReadOnly log As sun.util.logging.PlatformLogger = sun.util.logging.PlatformLogger.getLogger("java.awt.ContainerOrderFocusTraversalPolicy")

		Private ReadOnly FORWARD_TRAVERSAL As Integer = 0
		Private ReadOnly BACKWARD_TRAVERSAL As Integer = 1

	'    
	'     * JDK 1.4 serialVersionUID
	'     
		Private Const serialVersionUID As Long = 486933713763926351L

		Private implicitDownCycleTraversal As Boolean = True

		''' <summary>
		''' Used by getComponentAfter and getComponentBefore for efficiency. In
		''' order to maintain compliance with the specification of
		''' FocusTraversalPolicy, if traversal wraps, we should invoke
		''' getFirstComponent or getLastComponent. These methods may be overriden in
		''' subclasses to behave in a non-generic way. However, in the generic case,
		''' these methods will simply return the first or last Components of the
		''' sorted list, respectively. Since getComponentAfter and
		''' getComponentBefore have already built the list before determining
		''' that they need to invoke getFirstComponent or getLastComponent, the
		''' list should be reused if possible.
		''' </summary>
		<NonSerialized> _
		Private cachedRoot As Container
		<NonSerialized> _
		Private cachedCycle As IList(Of Component)

	'    
	'     * We suppose to use getFocusTraversalCycle & getComponentIndex methods in order
	'     * to divide the policy into two parts:
	'     * 1) Making the focus traversal cycle.
	'     * 2) Traversing the cycle.
	'     * The 1st point assumes producing a list of components representing the focus
	'     * traversal cycle. The two methods mentioned above should implement this logic.
	'     * The 2nd point assumes implementing the common concepts of operating on the
	'     * cycle: traversing back and forth, retrieving the initial/default/first/last
	'     * component. These concepts are described in the AWT Focus Spec and they are
	'     * applied to the FocusTraversalPolicy in general.
	'     * Thus, a descendant of this policy may wish to not reimplement the logic of
	'     * the 2nd point but just override the implementation of the 1st one.
	'     * A striking example of such a descendant is the javax.swing.SortingFocusTraversalPolicy.
	'     
		'protected
	 Private Function getFocusTraversalCycle(ByVal aContainer As Container) As IList(Of Component)
			Dim cycle As IList(Of Component) = New List(Of Component)
			enumerateCycle(aContainer, cycle)
			Return cycle
	 End Function
		'protected
	 Private Function getComponentIndex(ByVal cycle As IList(Of Component), ByVal aComponent As Component) As Integer
			Return cycle.IndexOf(aComponent)
	 End Function

		Private Sub enumerateCycle(ByVal container_Renamed As Container, ByVal cycle As IList(Of Component))
			If Not(container_Renamed.visible AndAlso container_Renamed.displayable) Then Return

			cycle.Add(container_Renamed)

			Dim components As Component() = container_Renamed.components
			For i As Integer = 0 To components.Length - 1
				Dim comp As Component = components(i)
				If TypeOf comp Is Container Then
					Dim cont As Container = CType(comp, Container)

					If (Not cont.focusCycleRoot) AndAlso (Not cont.focusTraversalPolicyProvider) Then
						enumerateCycle(cont, cycle)
						Continue For
					End If
				End If
				cycle.Add(comp)
			Next i
		End Sub

		Private Function getTopmostProvider(ByVal focusCycleRoot As Container, ByVal aComponent As Component) As Container
			Dim aCont As Container = aComponent.parent
			Dim ftp As Container = Nothing
			Do While aCont IsNot focusCycleRoot AndAlso aCont IsNot Nothing
				If aCont.focusTraversalPolicyProvider Then ftp = aCont
				aCont = aCont.parent
			Loop
			If aCont Is Nothing Then Return Nothing
			Return ftp
		End Function

	'    
	'     * Checks if a new focus cycle takes place and returns a Component to traverse focus to.
	'     * @param comp a possible focus cycle root or policy provider
	'     * @param traversalDirection the direction of the traversal
	'     * @return a Component to traverse focus to if {@code comp} is a root or provider
	'     *         and implicit down-cycle is set, otherwise {@code null}
	'     
		Private Function getComponentDownCycle(ByVal comp As Component, ByVal traversalDirection As Integer) As Component
			Dim retComp As Component = Nothing

			If TypeOf comp Is Container Then
				Dim cont As Container = CType(comp, Container)

				If cont.focusCycleRoot Then
					If implicitDownCycleTraversal Then
						retComp = cont.focusTraversalPolicy.getDefaultComponent(cont)

						If retComp IsNot Nothing AndAlso log.isLoggable(sun.util.logging.PlatformLogger.Level.FINE) Then log.fine("### Transfered focus down-cycle to " & retComp & " in the focus cycle root " & cont)
					Else
						Return Nothing
					End If
				ElseIf cont.focusTraversalPolicyProvider Then
					retComp = (If(traversalDirection = FORWARD_TRAVERSAL, cont.focusTraversalPolicy.getDefaultComponent(cont), cont.focusTraversalPolicy.getLastComponent(cont)))

					If retComp IsNot Nothing AndAlso log.isLoggable(sun.util.logging.PlatformLogger.Level.FINE) Then log.fine("### Transfered focus to " & retComp & " in the FTP provider " & cont)
				End If
			End If
			Return retComp
		End Function

		''' <summary>
		''' Returns the Component that should receive the focus after aComponent.
		''' aContainer must be a focus cycle root of aComponent or a focus traversal policy provider.
		''' <p>
		''' By default, ContainerOrderFocusTraversalPolicy implicitly transfers
		''' focus down-cycle. That is, during normal forward focus traversal, the
		''' Component traversed after a focus cycle root will be the focus-cycle-
		''' root's default Component to focus. This behavior can be disabled using
		''' the <code>setImplicitDownCycleTraversal</code> method.
		''' <p>
		''' If aContainer is <a href="doc-files/FocusSpec.html#FocusTraversalPolicyProviders">focus
		''' traversal policy provider</a>, the focus is always transferred down-cycle.
		''' </summary>
		''' <param name="aContainer"> a focus cycle root of aComponent or a focus traversal policy provider </param>
		''' <param name="aComponent"> a (possibly indirect) child of aContainer, or
		'''        aContainer itself </param>
		''' <returns> the Component that should receive the focus after aComponent, or
		'''         null if no suitable Component can be found </returns>
		''' <exception cref="IllegalArgumentException"> if aContainer is not a focus cycle
		'''         root of aComponent or focus traversal policy provider, or if either aContainer or
		'''         aComponent is null </exception>
		Public Overrides Function getComponentAfter(ByVal aContainer As Container, ByVal aComponent As Component) As Component
			If log.isLoggable(sun.util.logging.PlatformLogger.Level.FINE) Then log.fine("### Searching in " & aContainer & " for component after " & aComponent)

			If aContainer Is Nothing OrElse aComponent Is Nothing Then Throw New IllegalArgumentException("aContainer and aComponent cannot be null")
			If (Not aContainer.focusTraversalPolicyProvider) AndAlso (Not aContainer.focusCycleRoot) Then
				Throw New IllegalArgumentException("aContainer should be focus cycle root or focus traversal policy provider")

			ElseIf aContainer.focusCycleRoot AndAlso (Not aComponent.isFocusCycleRoot(aContainer)) Then
				Throw New IllegalArgumentException("aContainer is not a focus cycle root of aComponent")
			End If

			SyncLock aContainer.treeLock

				If Not(aContainer.visible AndAlso aContainer.displayable) Then Return Nothing

				' Before all the ckecks below we first see if it's an FTP provider or a focus cycle root.
				' If it's the case just go down cycle (if it's set to "implicit").
				Dim comp As Component = getComponentDownCycle(aComponent, FORWARD_TRAVERSAL)
				If comp IsNot Nothing Then Return comp

				' See if the component is inside of policy provider.
				Dim provider As Container = getTopmostProvider(aContainer, aComponent)
				If provider IsNot Nothing Then
					If log.isLoggable(sun.util.logging.PlatformLogger.Level.FINE) Then log.fine("### Asking FTP " & provider & " for component after " & aComponent)

					' FTP knows how to find component after the given. We don't.
					Dim policy As FocusTraversalPolicy = provider.focusTraversalPolicy
					Dim afterComp As Component = policy.getComponentAfter(provider, aComponent)

					' Null result means that we overstepped the limit of the FTP's cycle.
					' In that case we must quit the cycle, otherwise return the component found.
					If afterComp IsNot Nothing Then
						If log.isLoggable(sun.util.logging.PlatformLogger.Level.FINE) Then log.fine("### FTP returned " & afterComp)
						Return afterComp
					End If
					aComponent = provider
				End If

				Dim cycle As IList(Of Component) = getFocusTraversalCycle(aContainer)

				If log.isLoggable(sun.util.logging.PlatformLogger.Level.FINE) Then log.fine("### Cycle is " & cycle & ", component is " & aComponent)

				Dim index As Integer = getComponentIndex(cycle, aComponent)

				If index < 0 Then
					If log.isLoggable(sun.util.logging.PlatformLogger.Level.FINE) Then log.fine("### Didn't find component " & aComponent & " in a cycle " & aContainer)
					Return getFirstComponent(aContainer)
				End If

				For index = index + 1 To cycle.Count - 1
					comp = cycle(index)
					If accept(comp) Then
						Return comp
					Else
						comp = getComponentDownCycle(comp, FORWARD_TRAVERSAL)
						If comp IsNot Nothing Then Return comp
						End If
				Next index

				If aContainer.focusCycleRoot Then
					Me.cachedRoot = aContainer
					Me.cachedCycle = cycle

					comp = getFirstComponent(aContainer)

					Me.cachedRoot = Nothing
					Me.cachedCycle = Nothing

					Return comp
				End If
			End SyncLock
			Return Nothing
		End Function

		''' <summary>
		''' Returns the Component that should receive the focus before aComponent.
		''' aContainer must be a focus cycle root of aComponent or a <a
		''' href="doc-files/FocusSpec.html#FocusTraversalPolicyProviders">focus traversal policy
		''' provider</a>.
		''' </summary>
		''' <param name="aContainer"> a focus cycle root of aComponent or focus traversal policy provider </param>
		''' <param name="aComponent"> a (possibly indirect) child of aContainer, or
		'''        aContainer itself </param>
		''' <returns> the Component that should receive the focus before aComponent,
		'''         or null if no suitable Component can be found </returns>
		''' <exception cref="IllegalArgumentException"> if aContainer is not a focus cycle
		'''         root of aComponent or focus traversal policy provider, or if either aContainer or
		'''         aComponent is null </exception>
		Public Overrides Function getComponentBefore(ByVal aContainer As Container, ByVal aComponent As Component) As Component
			If aContainer Is Nothing OrElse aComponent Is Nothing Then Throw New IllegalArgumentException("aContainer and aComponent cannot be null")
			If (Not aContainer.focusTraversalPolicyProvider) AndAlso (Not aContainer.focusCycleRoot) Then
				Throw New IllegalArgumentException("aContainer should be focus cycle root or focus traversal policy provider")

			ElseIf aContainer.focusCycleRoot AndAlso (Not aComponent.isFocusCycleRoot(aContainer)) Then
				Throw New IllegalArgumentException("aContainer is not a focus cycle root of aComponent")
			End If

			SyncLock aContainer.treeLock

				If Not(aContainer.visible AndAlso aContainer.displayable) Then Return Nothing

				' See if the component is inside of policy provider.
				Dim provider As Container = getTopmostProvider(aContainer, aComponent)
				If provider IsNot Nothing Then
					If log.isLoggable(sun.util.logging.PlatformLogger.Level.FINE) Then log.fine("### Asking FTP " & provider & " for component after " & aComponent)

					' FTP knows how to find component after the given. We don't.
					Dim policy As FocusTraversalPolicy = provider.focusTraversalPolicy
					Dim beforeComp As Component = policy.getComponentBefore(provider, aComponent)

					' Null result means that we overstepped the limit of the FTP's cycle.
					' In that case we must quit the cycle, otherwise return the component found.
					If beforeComp IsNot Nothing Then
						If log.isLoggable(sun.util.logging.PlatformLogger.Level.FINE) Then log.fine("### FTP returned " & beforeComp)
						Return beforeComp
					End If
					aComponent = provider

					' If the provider is traversable it's returned.
					If accept(aComponent) Then Return aComponent
				End If

				Dim cycle As IList(Of Component) = getFocusTraversalCycle(aContainer)

				If log.isLoggable(sun.util.logging.PlatformLogger.Level.FINE) Then log.fine("### Cycle is " & cycle & ", component is " & aComponent)

				Dim index As Integer = getComponentIndex(cycle, aComponent)

				If index < 0 Then
					If log.isLoggable(sun.util.logging.PlatformLogger.Level.FINE) Then log.fine("### Didn't find component " & aComponent & " in a cycle " & aContainer)
					Return getLastComponent(aContainer)
				End If

				Dim comp As Component = Nothing
				Dim tryComp As Component = Nothing

				For index = index - 1 To 0 Step -1
					comp = cycle(index)
					tryComp = getComponentDownCycle(comp, BACKWARD_TRAVERSAL)
					If comp IsNot aContainer AndAlso tryComp IsNot Nothing Then
						Return tryComp
					ElseIf accept(comp) Then
						Return comp
					End If
				Next index

				If aContainer.focusCycleRoot Then
					Me.cachedRoot = aContainer
					Me.cachedCycle = cycle

					comp = getLastComponent(aContainer)

					Me.cachedRoot = Nothing
					Me.cachedCycle = Nothing

					Return comp
				End If
			End SyncLock
			Return Nothing
		End Function

		''' <summary>
		''' Returns the first Component in the traversal cycle. This method is used
		''' to determine the next Component to focus when traversal wraps in the
		''' forward direction.
		''' </summary>
		''' <param name="aContainer"> the focus cycle root or focus traversal policy provider whose first
		'''        Component is to be returned </param>
		''' <returns> the first Component in the traversal cycle of aContainer,
		'''         or null if no suitable Component can be found </returns>
		''' <exception cref="IllegalArgumentException"> if aContainer is null </exception>
		Public Overrides Function getFirstComponent(ByVal aContainer As Container) As Component
			Dim cycle As IList(Of Component)

			If log.isLoggable(sun.util.logging.PlatformLogger.Level.FINE) Then log.fine("### Getting first component in " & aContainer)
			If aContainer Is Nothing Then Throw New IllegalArgumentException("aContainer cannot be null")

			SyncLock aContainer.treeLock

				If Not(aContainer.visible AndAlso aContainer.displayable) Then Return Nothing

				If Me.cachedRoot Is aContainer Then
					cycle = Me.cachedCycle
				Else
					cycle = getFocusTraversalCycle(aContainer)
				End If

				If cycle.Count = 0 Then
					If log.isLoggable(sun.util.logging.PlatformLogger.Level.FINE) Then log.fine("### Cycle is empty")
					Return Nothing
				End If
				If log.isLoggable(sun.util.logging.PlatformLogger.Level.FINE) Then log.fine("### Cycle is " & cycle)

				For Each comp As Component In cycle
					If accept(comp) Then
						Return comp
					Else
						comp = getComponentDownCycle(comp, FORWARD_TRAVERSAL)
						If comp IsNot aContainer AndAlso comp IsNot Nothing Then Return comp
						End If
				Next comp
			End SyncLock
			Return Nothing
		End Function

		''' <summary>
		''' Returns the last Component in the traversal cycle. This method is used
		''' to determine the next Component to focus when traversal wraps in the
		''' reverse direction.
		''' </summary>
		''' <param name="aContainer"> the focus cycle root or focus traversal policy provider whose last
		'''        Component is to be returned </param>
		''' <returns> the last Component in the traversal cycle of aContainer,
		'''         or null if no suitable Component can be found </returns>
		''' <exception cref="IllegalArgumentException"> if aContainer is null </exception>
		Public Overrides Function getLastComponent(ByVal aContainer As Container) As Component
			Dim cycle As IList(Of Component)
			If log.isLoggable(sun.util.logging.PlatformLogger.Level.FINE) Then log.fine("### Getting last component in " & aContainer)

			If aContainer Is Nothing Then Throw New IllegalArgumentException("aContainer cannot be null")

			SyncLock aContainer.treeLock

				If Not(aContainer.visible AndAlso aContainer.displayable) Then Return Nothing

				If Me.cachedRoot Is aContainer Then
					cycle = Me.cachedCycle
				Else
					cycle = getFocusTraversalCycle(aContainer)
				End If

				If cycle.Count = 0 Then
					If log.isLoggable(sun.util.logging.PlatformLogger.Level.FINE) Then log.fine("### Cycle is empty")
					Return Nothing
				End If
				If log.isLoggable(sun.util.logging.PlatformLogger.Level.FINE) Then log.fine("### Cycle is " & cycle)

				For i As Integer = cycle.Count - 1 To 0 Step -1
					Dim comp As Component = cycle(i)
					If accept(comp) Then
						Return comp
					ElseIf TypeOf comp Is Container AndAlso comp IsNot aContainer Then
						Dim cont As Container = CType(comp, Container)
						If cont.focusTraversalPolicyProvider Then
							Dim retComp As Component = cont.focusTraversalPolicy.getLastComponent(cont)
							If retComp IsNot Nothing Then Return retComp
						End If
					End If
				Next i
			End SyncLock
			Return Nothing
		End Function

		''' <summary>
		''' Returns the default Component to focus. This Component will be the first
		''' to receive focus when traversing down into a new focus traversal cycle
		''' rooted at aContainer. The default implementation of this method
		''' returns the same Component as <code>getFirstComponent</code>.
		''' </summary>
		''' <param name="aContainer"> the focus cycle root or focus traversal policy provider whose default
		'''        Component is to be returned </param>
		''' <returns> the default Component in the traversal cycle of aContainer,
		'''         or null if no suitable Component can be found </returns>
		''' <seealso cref= #getFirstComponent </seealso>
		''' <exception cref="IllegalArgumentException"> if aContainer is null </exception>
		Public Overrides Function getDefaultComponent(ByVal aContainer As Container) As Component
			Return getFirstComponent(aContainer)
		End Function

		''' <summary>
		''' Sets whether this ContainerOrderFocusTraversalPolicy transfers focus
		''' down-cycle implicitly. If <code>true</code>, during normal forward focus
		''' traversal, the Component traversed after a focus cycle root will be the
		''' focus-cycle-root's default Component to focus. If <code>false</code>,
		''' the next Component in the focus traversal cycle rooted at the specified
		''' focus cycle root will be traversed instead. The default value for this
		''' property is <code>true</code>.
		''' </summary>
		''' <param name="implicitDownCycleTraversal"> whether this
		'''        ContainerOrderFocusTraversalPolicy transfers focus down-cycle
		'''        implicitly </param>
		''' <seealso cref= #getImplicitDownCycleTraversal </seealso>
		''' <seealso cref= #getFirstComponent </seealso>
		Public Overridable Property implicitDownCycleTraversal As Boolean
			Set(ByVal implicitDownCycleTraversal As Boolean)
				Me.implicitDownCycleTraversal = implicitDownCycleTraversal
			End Set
			Get
				Return implicitDownCycleTraversal
			End Get
		End Property


		''' <summary>
		''' Determines whether a Component is an acceptable choice as the new
		''' focus owner. By default, this method will accept a Component if and
		''' only if it is visible, displayable, enabled, and focusable.
		''' </summary>
		''' <param name="aComponent"> the Component whose fitness as a focus owner is to
		'''        be tested </param>
		''' <returns> <code>true</code> if aComponent is visible, displayable,
		'''         enabled, and focusable; <code>false</code> otherwise </returns>
		Protected Friend Overridable Function accept(ByVal aComponent As Component) As Boolean
			If Not aComponent.canBeFocusOwner() Then Return False

			' Verify that the Component is recursively enabled. Disabling a
			' heavyweight Container disables its children, whereas disabling
			' a lightweight Container does not.
			If Not(TypeOf aComponent Is Window) Then
				Dim enableTest As Container = aComponent.parent
				Do While enableTest IsNot Nothing
					If Not(enableTest.enabled OrElse enableTest.lightweight) Then Return False
					If TypeOf enableTest Is Window Then Exit Do
					enableTest = enableTest.parent
				Loop
			End If

			Return True
		End Function
	End Class

End Namespace