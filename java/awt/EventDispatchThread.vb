Imports System.Runtime.CompilerServices
Imports System.Collections.Generic

'
' * Copyright (c) 1996, 2013, Oracle and/or its affiliates. All rights reserved.
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
	''' EventDispatchThread is a package-private AWT class which takes
	''' events off the EventQueue and dispatches them to the appropriate
	''' AWT components.
	''' 
	''' The Thread starts a "permanent" event pump with a call to
	''' pumpEvents(Conditional) in its run() method. Event handlers can choose to
	''' block this event pump at any time, but should start a new pump (<b>not</b>
	''' a new EventDispatchThread) by again calling pumpEvents(Conditional). This
	''' secondary event pump will exit automatically as soon as the Condtional
	''' evaluate()s to false and an additional Event is pumped and dispatched.
	''' 
	''' @author Tom Ball
	''' @author Amy Fowler
	''' @author Fred Ecks
	''' @author David Mendenhall
	''' 
	''' @since 1.1
	''' </summary>
	Friend Class EventDispatchThread
		Inherits Thread

		Private Shared ReadOnly eventLog As sun.util.logging.PlatformLogger = sun.util.logging.PlatformLogger.getLogger("java.awt.event.EventDispatchThread")

		Private theQueue As EventQueue
'JAVA TO VB CONVERTER TODO TASK: There is no VB equivalent to 'volatile':
		Private doDispatch As Boolean = True

		Private Const ANY_EVENT As Integer = -1

		Private eventFilters As New List(Of EventFilter)

		Friend Sub New(ByVal group As ThreadGroup, ByVal name As String, ByVal queue As EventQueue)
			MyBase.New(group, name)
			eventQueue = queue
		End Sub

	'    
	'     * Must be called on EDT only, that's why no synchronization
	'     
		Public Overridable Sub stopDispatching()
			doDispatch = False
		End Sub

		Public Overrides Sub run()
			Try
				pumpEvents(New ConditionalAnonymousInnerClassHelper
			Finally
				eventQueue.detachDispatchThread(Me)
			End Try
		End Sub

		Private Class ConditionalAnonymousInnerClassHelper
			Implements Conditional

			Public Overridable Function evaluate() As Boolean Implements Conditional.evaluate
				Return True
			End Function
		End Class

		Friend Overridable Sub pumpEvents(ByVal cond As Conditional)
			pumpEvents(ANY_EVENT, cond)
		End Sub

		Friend Overridable Sub pumpEventsForHierarchy(ByVal cond As Conditional, ByVal modalComponent As Component)
			pumpEventsForHierarchy(ANY_EVENT, cond, modalComponent)
		End Sub

		Friend Overridable Sub pumpEvents(ByVal id As Integer, ByVal cond As Conditional)
			pumpEventsForHierarchy(id, cond, Nothing)
		End Sub

		Friend Overridable Sub pumpEventsForHierarchy(ByVal id As Integer, ByVal cond As Conditional, ByVal modalComponent As Component)
			pumpEventsForFilter(id, cond, New HierarchyEventFilter(modalComponent))
		End Sub

		Friend Overridable Sub pumpEventsForFilter(ByVal cond As Conditional, ByVal filter As EventFilter)
			pumpEventsForFilter(ANY_EVENT, cond, filter)
		End Sub

		Friend Overridable Sub pumpEventsForFilter(ByVal id As Integer, ByVal cond As Conditional, ByVal filter As EventFilter)
			addEventFilter(filter)
			doDispatch = True
			Do While doDispatch AndAlso (Not interrupted) AndAlso cond.evaluate()
				pumpOneEventForFilters(id)
			Loop
			removeEventFilter(filter)
		End Sub

		Friend Overridable Sub addEventFilter(ByVal filter As EventFilter)
			If eventLog.isLoggable(sun.util.logging.PlatformLogger.Level.FINEST) Then eventLog.finest("adding the event filter: " & filter)
			SyncLock eventFilters
				If Not eventFilters.Contains(filter) Then
					If TypeOf filter Is ModalEventFilter Then
						Dim newFilter As ModalEventFilter = CType(filter, ModalEventFilter)
						Dim k As Integer = 0
						For k = 0 To eventFilters.Count - 1
							Dim f As EventFilter = eventFilters(k)
							If TypeOf f Is ModalEventFilter Then
								Dim cf As ModalEventFilter = CType(f, ModalEventFilter)
								If cf.CompareTo(newFilter) > 0 Then Exit For
							End If
						Next k
						eventFilters.Insert(k, filter)
					Else
						eventFilters.Add(filter)
					End If
				End If
			End SyncLock
		End Sub

		Friend Overridable Sub removeEventFilter(ByVal filter As EventFilter)
			If eventLog.isLoggable(sun.util.logging.PlatformLogger.Level.FINEST) Then eventLog.finest("removing the event filter: " & filter)
			SyncLock eventFilters
				eventFilters.Remove(filter)
			End SyncLock
		End Sub

		Friend Overridable Sub pumpOneEventForFilters(ByVal id As Integer)
			Dim event_Renamed As AWTEvent = Nothing
			Dim eventOK As Boolean = False
			Try
				Dim eq As EventQueue = Nothing
				Dim [delegate] As sun.awt.EventQueueDelegate.Delegate = Nothing
				Do
					' EventQueue may change during the dispatching
					eq = eventQueue
					[delegate] = sun.awt.EventQueueDelegate.delegate

					If [delegate] IsNot Nothing AndAlso id = ANY_EVENT Then
						event_Renamed = [delegate].getNextEvent(eq)
					Else
						event_Renamed = If(id = ANY_EVENT, eq.nextEvent, eq.getNextEvent(id))
					End If

					eventOK = True
					SyncLock eventFilters
						For i As Integer = eventFilters.Count - 1 To 0 Step -1
							Dim f As EventFilter = eventFilters(i)
							Dim accept As EventFilter.FilterAction = f.acceptEvent(event_Renamed)
							If accept Is EventFilter.FilterAction.REJECT Then
								eventOK = False
								Exit For
							ElseIf accept Is EventFilter.FilterAction.ACCEPT_IMMEDIATELY Then
								Exit For
							End If
						Next i
					End SyncLock
					eventOK = eventOK AndAlso sun.awt.dnd.SunDragSourceContextPeer.checkEvent(event_Renamed)
					If Not eventOK Then event_Renamed.consume()
				Loop While eventOK = False

				If eventLog.isLoggable(sun.util.logging.PlatformLogger.Level.FINEST) Then eventLog.finest("Dispatching: " & event_Renamed)

				Dim handle As Object = Nothing
				If [delegate] IsNot Nothing Then handle = [delegate].beforeDispatch(event_Renamed)
				eq.dispatchEvent(event_Renamed)
				If [delegate] IsNot Nothing Then [delegate].afterDispatch(event_Renamed, handle)
			Catch death As ThreadDeath
				doDispatch = False
				Throw death
			Catch interruptedException As InterruptedException
				doDispatch = False ' AppContext.dispose() interrupts all
									' Threads in the AppContext
			Catch e As Throwable
				processException(e)
			End Try
		End Sub

		Private Sub processException(ByVal e As Throwable)
			If eventLog.isLoggable(sun.util.logging.PlatformLogger.Level.FINE) Then eventLog.fine("Processing exception: " & e)
			uncaughtExceptionHandler.uncaughtException(Me, e)
		End Sub

		<MethodImpl(MethodImplOptions.Synchronized)> _
		Public Overridable Property eventQueue As EventQueue
			Get
				Return theQueue
			End Get
			Set(ByVal eq As EventQueue)
				theQueue = eq
			End Set
		End Property

		Private Class HierarchyEventFilter
			Implements EventFilter

			Private modalComponent As Component
			Public Sub New(ByVal modalComponent As Component)
				Me.modalComponent = modalComponent
			End Sub
			Public Overridable Function acceptEvent(ByVal [event] As AWTEvent) As FilterAction Implements EventFilter.acceptEvent
				If modalComponent IsNot Nothing Then
					Dim eventID As Integer = event_Renamed.iD
					Dim mouseEvent As Boolean = (eventID >= java.awt.event.MouseEvent.MOUSE_FIRST) AndAlso (eventID <= java.awt.event.MouseEvent.MOUSE_LAST)
					Dim actionEvent As Boolean = (eventID >= java.awt.event.ActionEvent.ACTION_FIRST) AndAlso (eventID <= java.awt.event.ActionEvent.ACTION_LAST)
					Dim windowClosingEvent As Boolean = (eventID = java.awt.event.WindowEvent.WINDOW_CLOSING)
	'                
	'                 * filter out MouseEvent and ActionEvent that's outside
	'                 * the modalComponent hierarchy.
	'                 * KeyEvent is handled by using enqueueKeyEvent
	'                 * in Dialog.show
	'                 
					If Component.isInstanceOf(modalComponent, "javax.swing.JInternalFrame") Then Return If(windowClosingEvent, FilterAction.REJECT, FilterAction.ACCEPT)
					If mouseEvent OrElse actionEvent OrElse windowClosingEvent Then
						Dim o As Object = event_Renamed.source
						If TypeOf o Is sun.awt.ModalExclude Then
							' Exclude this object from modality and
							' continue to pump it's events.
							Return FilterAction.ACCEPT
						ElseIf TypeOf o Is Component Then
							Dim c As Component = CType(o, Component)
							' 5.0u3 modal exclusion
							Dim modalExcluded As Boolean = False
							If TypeOf modalComponent Is Container Then
								Do While c IsNot modalComponent AndAlso c IsNot Nothing
									If (TypeOf c Is Window) AndAlso (sun.awt.SunToolkit.isModalExcluded(CType(c, Window))) Then
										' Exclude this window and all its children from
										'  modality and continue to pump it's events.
										modalExcluded = True
										Exit Do
									End If
									c = c.parent
								Loop
							End If
							If (Not modalExcluded) AndAlso (c IsNot modalComponent) Then Return FilterAction.REJECT
						End If
					End If
				End If
				Return FilterAction.ACCEPT
			End Function
		End Class
	End Class

End Namespace