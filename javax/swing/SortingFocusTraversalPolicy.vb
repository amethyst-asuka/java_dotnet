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
Namespace javax.swing


	''' <summary>
	''' A FocusTraversalPolicy that determines traversal order by sorting the
	''' Components of a focus traversal cycle based on a given Comparator. Portions
	''' of the Component hierarchy that are not visible and displayable will not be
	''' included.
	''' <p>
	''' By default, SortingFocusTraversalPolicy implicitly transfers focus down-
	''' cycle. That is, during normal focus traversal, the Component
	''' traversed after a focus cycle root will be the focus-cycle-root's default
	''' Component to focus. This behavior can be disabled using the
	''' <code>setImplicitDownCycleTraversal</code> method.
	''' <p>
	''' By default, methods of this class with return a Component only if it is
	''' visible, displayable, enabled, and focusable. Subclasses can modify this
	''' behavior by overriding the <code>accept</code> method.
	''' <p>
	''' This policy takes into account <a
	''' href="../../java/awt/doc-files/FocusSpec.html#FocusTraversalPolicyProviders">focus traversal
	''' policy providers</a>.  When searching for first/last/next/previous Component,
	''' if a focus traversal policy provider is encountered, its focus traversal
	''' policy is used to perform the search operation.
	''' 
	''' @author David Mendenhall
	''' </summary>
	''' <seealso cref= java.util.Comparator
	''' @since 1.4 </seealso>
	Public Class SortingFocusTraversalPolicy
		Inherits InternalFrameFocusTraversalPolicy

'JAVA TO VB CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
		Private comparator As IComparer(Of ?)
		Private implicitDownCycleTraversal As Boolean = True

		Private log As sun.util.logging.PlatformLogger = sun.util.logging.PlatformLogger.getLogger("javax.swing.SortingFocusTraversalPolicy")

		''' <summary>
		''' Used by getComponentAfter and getComponentBefore for efficiency. In
		''' order to maintain compliance with the specification of
		''' FocusTraversalPolicy, if traversal wraps, we should invoke
		''' getFirstComponent or getLastComponent. These methods may be overriden in
		''' subclasses to behave in a non-generic way. However, in the generic case,
		''' these methods will simply return the first or last Components of the
		''' sorted list, respectively. Since getComponentAfter and
		''' getComponentBefore have already built the sorted list before determining
		''' that they need to invoke getFirstComponent or getLastComponent, the
		''' sorted list should be reused if possible.
		''' </summary>
		<NonSerialized> _
		Private cachedRoot As java.awt.Container
		<NonSerialized> _
		Private cachedCycle As IList(Of java.awt.Component)

		' Delegate our fitness test to ContainerOrder so that we only have to
		' code the algorithm once.
		Private Shared ReadOnly fitnessTestPolicy As New SwingContainerOrderFocusTraversalPolicy

		Private ReadOnly FORWARD_TRAVERSAL As Integer = 0
		Private ReadOnly BACKWARD_TRAVERSAL As Integer = 1

	'    
	'     * When true (by default), the legacy merge-sort algo is used to sort an FTP cycle.
	'     * When false, the default (tim-sort) algo is used, which may lead to an exception.
	'     * See: JDK-8048887
	'     
		Private Shared ReadOnly legacySortingFTPEnabled As Boolean
		Private Shared ReadOnly legacyMergeSortMethod As Method

		Shared Sub New()
			legacySortingFTPEnabled = "true".Equals(java.security.AccessController.doPrivileged(New sun.security.action.GetPropertyAction("swing.legacySortingFTPEnabled", "true")))
'JAVA TO VB CONVERTER TODO TASK: Anonymous inner classes are not converted to VB if the base type is not defined in the code being converted:
'			legacyMergeSortMethod = legacySortingFTPEnabled ? java.security.AccessController.doPrivileged(New java.security.PrivilegedAction<Method>()
	'		{
	'				public Method run()
	'				{
	'					try
	'					{
	'						Class c = Class.forName("java.util.Arrays");
	'						Method m = c.getDeclaredMethod("legacyMergeSort", New Class[]{Object[].class, Comparator.class});
	'						m.setAccessible(True);
	'						Return m;
	'					}
	'					catch (ClassNotFoundException | NoSuchMethodException e)
	'					{
	'						' using default sorting algo
	'						Return Nothing;
	'					}
	'				}
	'			}) :
				Nothing
		End Sub

		''' <summary>
		''' Constructs a SortingFocusTraversalPolicy without a Comparator.
		''' Subclasses must set the Comparator using <code>setComparator</code>
		''' before installing this FocusTraversalPolicy on a focus cycle root or
		''' KeyboardFocusManager.
		''' </summary>
		Protected Friend Sub New()
		End Sub

		''' <summary>
		''' Constructs a SortingFocusTraversalPolicy with the specified Comparator.
		''' </summary>
'JAVA TO VB CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
		Public Sub New(Of T1)(ByVal comparator As IComparer(Of T1))
			Me.comparator = comparator
		End Sub

		Private Function getFocusTraversalCycle(ByVal aContainer As java.awt.Container) As IList(Of java.awt.Component)
			Dim cycle As IList(Of java.awt.Component) = New List(Of java.awt.Component)
			enumerateAndSortCycle(aContainer, cycle)
			Return cycle
		End Function
		Private Function getComponentIndex(ByVal cycle As IList(Of java.awt.Component), ByVal aComponent As java.awt.Component) As Integer
			Dim index As Integer
			Try
				index = Collections.binarySearch(cycle, aComponent, comparator)
			Catch e As ClassCastException
				If log.isLoggable(sun.util.logging.PlatformLogger.Level.FINE) Then log.fine("### During the binary search for " & aComponent & " the exception occurred: ", e)
				Return -1
			End Try
			If index < 0 Then index = cycle.IndexOf(aComponent)
			Return index
		End Function

		Private Sub enumerateAndSortCycle(ByVal focusCycleRoot As java.awt.Container, ByVal cycle As IList(Of java.awt.Component))
			If focusCycleRoot.showing Then
				enumerateCycle(focusCycleRoot, cycle)
				If (Not legacySortingFTPEnabled) OrElse (Not legacySort(cycle, comparator)) Then Collections.sort(cycle, comparator)
			End If
		End Sub

'JAVA TO VB CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
		Private Function legacySort(Of T1)(ByVal l As IList(Of java.awt.Component), ByVal c As IComparer(Of T1)) As Boolean
			If legacyMergeSortMethod Is Nothing Then Return False

			Dim a As Object() = l.ToArray()
			Try
				legacyMergeSortMethod.invoke(Nothing, a, c)
'JAVA TO VB CONVERTER TODO TASK: There is no equivalent in VB to Java 'multi-catch' syntax:
			Catch IllegalAccessException Or InvocationTargetException e
				Return False
			End Try
			Dim i As IEnumerator(Of java.awt.Component) = l.GetEnumerator()
			For Each e As Object In a
'JAVA TO VB CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
				i.next()
				i.set(CType(e, java.awt.Component))
			Next e
			Return True
		End Function

		Private Sub enumerateCycle(ByVal container As java.awt.Container, ByVal cycle As IList(Of java.awt.Component))
			If Not(container.visible AndAlso container.displayable) Then Return

			cycle.Add(container)

			Dim components As java.awt.Component() = container.components
			For Each comp As java.awt.Component In components
				If TypeOf comp Is java.awt.Container Then
					Dim cont As java.awt.Container = CType(comp, java.awt.Container)

					If (Not cont.focusCycleRoot) AndAlso (Not cont.focusTraversalPolicyProvider) AndAlso Not((TypeOf cont Is JComponent) AndAlso CType(cont, JComponent).managingFocus) Then
						enumerateCycle(cont, cycle)
						Continue For
					End If
				End If
				cycle.Add(comp)
			Next comp
		End Sub

		Friend Overridable Function getTopmostProvider(ByVal focusCycleRoot As java.awt.Container, ByVal aComponent As java.awt.Component) As java.awt.Container
			Dim aCont As java.awt.Container = aComponent.parent
			Dim ftp As java.awt.Container = Nothing
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
		Private Function getComponentDownCycle(ByVal comp As java.awt.Component, ByVal traversalDirection As Integer) As java.awt.Component
			Dim retComp As java.awt.Component = Nothing

			If TypeOf comp Is java.awt.Container Then
				Dim cont As java.awt.Container = CType(comp, java.awt.Container)

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
		''' By default, SortingFocusTraversalPolicy implicitly transfers focus down-
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
		Public Overridable Function getComponentAfter(ByVal aContainer As java.awt.Container, ByVal aComponent As java.awt.Component) As java.awt.Component
			If log.isLoggable(sun.util.logging.PlatformLogger.Level.FINE) Then log.fine("### Searching in " & aContainer & " for component after " & aComponent)

			If aContainer Is Nothing OrElse aComponent Is Nothing Then Throw New System.ArgumentException("aContainer and aComponent cannot be null")
			If (Not aContainer.focusTraversalPolicyProvider) AndAlso (Not aContainer.focusCycleRoot) Then
				Throw New System.ArgumentException("aContainer should be focus cycle root or focus traversal policy provider")

			ElseIf aContainer.focusCycleRoot AndAlso (Not aComponent.isFocusCycleRoot(aContainer)) Then
				Throw New System.ArgumentException("aContainer is not a focus cycle root of aComponent")
			End If

			' Before all the ckecks below we first see if it's an FTP provider or a focus cycle root.
			' If it's the case just go down cycle (if it's set to "implicit").
			Dim comp As java.awt.Component = getComponentDownCycle(aComponent, FORWARD_TRAVERSAL)
			If comp IsNot Nothing Then Return comp

			' See if the component is inside of policy provider.
			Dim provider As java.awt.Container = getTopmostProvider(aContainer, aComponent)
			If provider IsNot Nothing Then
				If log.isLoggable(sun.util.logging.PlatformLogger.Level.FINE) Then log.fine("### Asking FTP " & provider & " for component after " & aComponent)

				' FTP knows how to find component after the given. We don't.
				Dim policy As java.awt.FocusTraversalPolicy = provider.focusTraversalPolicy
				Dim afterComp As java.awt.Component = policy.getComponentAfter(provider, aComponent)

				' Null result means that we overstepped the limit of the FTP's cycle.
				' In that case we must quit the cycle, otherwise return the component found.
				If afterComp IsNot Nothing Then
					If log.isLoggable(sun.util.logging.PlatformLogger.Level.FINE) Then log.fine("### FTP returned " & afterComp)
					Return afterComp
				End If
				aComponent = provider
			End If

			Dim cycle As IList(Of java.awt.Component) = getFocusTraversalCycle(aContainer)

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
			Return Nothing
		End Function

		''' <summary>
		''' Returns the Component that should receive the focus before aComponent.
		''' aContainer must be a focus cycle root of aComponent or a focus traversal policy provider.
		''' <p>
		''' By default, SortingFocusTraversalPolicy implicitly transfers focus down-
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
		Public Overridable Function getComponentBefore(ByVal aContainer As java.awt.Container, ByVal aComponent As java.awt.Component) As java.awt.Component
			If aContainer Is Nothing OrElse aComponent Is Nothing Then Throw New System.ArgumentException("aContainer and aComponent cannot be null")
			If (Not aContainer.focusTraversalPolicyProvider) AndAlso (Not aContainer.focusCycleRoot) Then
				Throw New System.ArgumentException("aContainer should be focus cycle root or focus traversal policy provider")

			ElseIf aContainer.focusCycleRoot AndAlso (Not aComponent.isFocusCycleRoot(aContainer)) Then
				Throw New System.ArgumentException("aContainer is not a focus cycle root of aComponent")
			End If

			' See if the component is inside of policy provider.
			Dim provider As java.awt.Container = getTopmostProvider(aContainer, aComponent)
			If provider IsNot Nothing Then
				If log.isLoggable(sun.util.logging.PlatformLogger.Level.FINE) Then log.fine("### Asking FTP " & provider & " for component after " & aComponent)

				' FTP knows how to find component after the given. We don't.
				Dim policy As java.awt.FocusTraversalPolicy = provider.focusTraversalPolicy
				Dim beforeComp As java.awt.Component = policy.getComponentBefore(provider, aComponent)

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

			Dim cycle As IList(Of java.awt.Component) = getFocusTraversalCycle(aContainer)

			If log.isLoggable(sun.util.logging.PlatformLogger.Level.FINE) Then log.fine("### Cycle is " & cycle & ", component is " & aComponent)

			Dim index As Integer = getComponentIndex(cycle, aComponent)

			If index < 0 Then
				If log.isLoggable(sun.util.logging.PlatformLogger.Level.FINE) Then log.fine("### Didn't find component " & aComponent & " in a cycle " & aContainer)
				Return getLastComponent(aContainer)
			End If

			Dim comp As java.awt.Component
			Dim tryComp As java.awt.Component

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
			Return Nothing
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
		Public Overridable Function getFirstComponent(ByVal aContainer As java.awt.Container) As java.awt.Component
			Dim cycle As IList(Of java.awt.Component)

			If log.isLoggable(sun.util.logging.PlatformLogger.Level.FINE) Then log.fine("### Getting first component in " & aContainer)
			If aContainer Is Nothing Then Throw New System.ArgumentException("aContainer cannot be null")

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

			For Each comp As java.awt.Component In cycle
				If accept(comp) Then
					Return comp
				Else
					comp = getComponentDownCycle(comp, FORWARD_TRAVERSAL)
					If comp IsNot aContainer AndAlso comp IsNot Nothing Then Return comp
					End If
			Next comp
			Return Nothing
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
		Public Overridable Function getLastComponent(ByVal aContainer As java.awt.Container) As java.awt.Component
			Dim cycle As IList(Of java.awt.Component)
			If log.isLoggable(sun.util.logging.PlatformLogger.Level.FINE) Then log.fine("### Getting last component in " & aContainer)

			If aContainer Is Nothing Then Throw New System.ArgumentException("aContainer cannot be null")

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
				Dim comp As java.awt.Component = cycle(i)
				If accept(comp) Then
					Return comp
				ElseIf TypeOf comp Is java.awt.Container AndAlso comp IsNot aContainer Then
					Dim cont As java.awt.Container = CType(comp, java.awt.Container)
					If cont.focusTraversalPolicyProvider Then
						Dim retComp As java.awt.Component = cont.focusTraversalPolicy.getLastComponent(cont)
						If retComp IsNot Nothing Then Return retComp
					End If
				End If
			Next i
			Return Nothing
		End Function

		''' <summary>
		''' Returns the default Component to focus. This Component will be the first
		''' to receive focus when traversing down into a new focus traversal cycle
		''' rooted at aContainer. The default implementation of this method
		''' returns the same Component as <code>getFirstComponent</code>.
		''' </summary>
		''' <param name="aContainer"> a focus cycle root of aComponent or a focus traversal policy provider whose
		'''        default Component is to be returned </param>
		''' <returns> the default Component in the traversal cycle of aContainer,
		'''         or null if no suitable Component can be found </returns>
		''' <seealso cref= #getFirstComponent </seealso>
		''' <exception cref="IllegalArgumentException"> if aContainer is null </exception>
		Public Overridable Function getDefaultComponent(ByVal aContainer As java.awt.Container) As java.awt.Component
			Return getFirstComponent(aContainer)
		End Function

		''' <summary>
		''' Sets whether this SortingFocusTraversalPolicy transfers focus down-cycle
		''' implicitly. If <code>true</code>, during normal focus traversal,
		''' the Component traversed after a focus cycle root will be the focus-
		''' cycle-root's default Component to focus. If <code>false</code>, the
		''' next Component in the focus traversal cycle rooted at the specified
		''' focus cycle root will be traversed instead. The default value for this
		''' property is <code>true</code>.
		''' </summary>
		''' <param name="implicitDownCycleTraversal"> whether this
		'''        SortingFocusTraversalPolicy transfers focus down-cycle implicitly </param>
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
		''' Sets the Comparator which will be used to sort the Components in a
		''' focus traversal cycle.
		''' </summary>
		''' <param name="comparator"> the Comparator which will be used for sorting </param>
'JAVA TO VB CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
		Protected Friend Overridable Property comparator(Of T1) As IComparer(Of T1)
			Set(ByVal comparator As IComparer(Of T1))
				Me.comparator = comparator
			End Set
			Get
				Return comparator
			End Get
		End Property

		''' <summary>
		''' Returns the Comparator which will be used to sort the Components in a
		''' focus traversal cycle.
		''' </summary>
		''' <returns> the Comparator which will be used for sorting </returns>
'JAVA TO VB CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:

		''' <summary>
		''' Determines whether a Component is an acceptable choice as the new
		''' focus owner. By default, this method will accept a Component if and
		''' only if it is visible, displayable, enabled, and focusable.
		''' </summary>
		''' <param name="aComponent"> the Component whose fitness as a focus owner is to
		'''        be tested </param>
		''' <returns> <code>true</code> if aComponent is visible, displayable,
		'''         enabled, and focusable; <code>false</code> otherwise </returns>
		Protected Friend Overridable Function accept(ByVal aComponent As java.awt.Component) As Boolean
			Return fitnessTestPolicy.accept(aComponent)
		End Function
	End Class

	' Create our own subclass and change accept to public so that we can call
	' accept.
	Friend Class SwingContainerOrderFocusTraversalPolicy
		Inherits java.awt.ContainerOrderFocusTraversalPolicy

		Public Overridable Function accept(ByVal aComponent As java.awt.Component) As Boolean
			Return MyBase.accept(aComponent)
		End Function
	End Class

End Namespace