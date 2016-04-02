Imports System.Runtime.CompilerServices
Imports System.Threading

'
' * Copyright (c) 2010, 2013, Oracle and/or its affiliates. All rights reserved.
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
	''' This utility class is used to suspend execution on a thread
	''' while still allowing {@code EventDispatchThread} to dispatch events.
	''' The API methods of the class are thread-safe.
	''' 
	''' @author Anton Tarasov, Artem Ananiev
	''' 
	''' @since 1.7
	''' </summary>
	Friend Class WaitDispatchSupport
		Implements SecondaryLoop

		Private Shared ReadOnly log As sun.util.logging.PlatformLogger = sun.util.logging.PlatformLogger.getLogger("java.awt.event.WaitDispatchSupport")

		Private dispatchThread As EventDispatchThread
		Private filter As EventFilter

'JAVA TO VB CONVERTER TODO TASK: There is no VB equivalent to 'volatile':
		Private extCondition As Conditional
'JAVA TO VB CONVERTER TODO TASK: There is no VB equivalent to 'volatile':
		Private condition As Conditional

		Private interval As Long
		' Use a shared daemon timer to serve all the WaitDispatchSupports
		Private Shared timer As java.util.Timer
		' When this WDS expires, we cancel the timer task leaving the
		' shared timer up and running
		Private timerTask As java.util.TimerTask

		Private keepBlockingEDT As New java.util.concurrent.atomic.AtomicBoolean(False)
		Private keepBlockingCT As New java.util.concurrent.atomic.AtomicBoolean(False)

		<MethodImpl(MethodImplOptions.Synchronized)> _
		Private Shared Sub initializeTimer()
			If timer Is Nothing Then timer = New java.util.Timer("AWT-WaitDispatchSupport-Timer", True)
		End Sub

		''' <summary>
		''' Creates a {@code WaitDispatchSupport} instance to
		''' serve the given event dispatch thread.
		''' </summary>
		''' <param name="dispatchThread"> An event dispatch thread that
		'''        should not stop dispatching events while waiting
		''' 
		''' @since 1.7 </param>
		Public Sub New(ByVal dispatchThread As EventDispatchThread)
			Me.New(dispatchThread, Nothing)
		End Sub

		''' <summary>
		''' Creates a {@code WaitDispatchSupport} instance to
		''' serve the given event dispatch thread.
		''' </summary>
		''' <param name="dispatchThread"> An event dispatch thread that
		'''        should not stop dispatching events while waiting </param>
		''' <param name="extCond"> A conditional object used to determine
		'''        if the loop should be terminated
		''' 
		''' @since 1.7 </param>
		Public Sub New(ByVal dispatchThread As EventDispatchThread, ByVal extCond As Conditional)
			If dispatchThread Is Nothing Then Throw New IllegalArgumentException("The dispatchThread can not be null")

			Me.dispatchThread = dispatchThread
			Me.extCondition = extCond
			Me.condition = New ConditionalAnonymousInnerClassHelper
		End Sub

		Private Class ConditionalAnonymousInnerClassHelper
			Implements Conditional

			Public Overrides Function evaluate() As Boolean Implements Conditional.evaluate
				If log.isLoggable(sun.util.logging.PlatformLogger.Level.FINEST) Then log.finest("evaluate(): blockingEDT=" & outerInstance.keepBlockingEDT.get() & ", blockingCT=" & outerInstance.keepBlockingCT.get())
				Dim extEvaluate As Boolean = If(outerInstance.extCondition IsNot Nothing, outerInstance.extCondition.evaluate(), True)
				If (Not outerInstance.keepBlockingEDT.get()) OrElse (Not extEvaluate) Then
					If outerInstance.timerTask IsNot Nothing Then
						outerInstance.timerTask.cancel()
						outerInstance.timerTask = Nothing
					End If
					Return False
				End If
				Return True
			End Function
		End Class

		''' <summary>
		''' Creates a {@code WaitDispatchSupport} instance to
		''' serve the given event dispatch thread.
		''' <p>
		''' The <seealso cref="EventFilter"/> is set on the {@code dispatchThread}
		''' while waiting. The filter is removed on completion of the
		''' waiting process.
		''' <p>
		''' 
		''' </summary>
		''' <param name="dispatchThread"> An event dispatch thread that
		'''        should not stop dispatching events while waiting </param>
		''' <param name="filter"> {@code EventFilter} to be set </param>
		''' <param name="interval"> A time interval to wait for. Note that
		'''        when the waiting process takes place on EDT
		'''        there is no guarantee to stop it in the given time
		''' 
		''' @since 1.7 </param>
		Public Sub New(ByVal dispatchThread As EventDispatchThread, ByVal extCondition As Conditional, ByVal filter As EventFilter, ByVal interval As Long)
			Me.New(dispatchThread, extCondition)
			Me.filter = filter
			If interval < 0 Then Throw New IllegalArgumentException("The interval value must be >= 0")
			Me.interval = interval
			If interval <> 0 Then initializeTimer()
		End Sub

		''' <summary>
		''' {@inheritDoc}
		''' </summary>
		Public Overrides Function enter() As Boolean Implements SecondaryLoop.enter
			If log.isLoggable(sun.util.logging.PlatformLogger.Level.FINE) Then log.fine("enter(): blockingEDT=" & keepBlockingEDT.get() & ", blockingCT=" & keepBlockingCT.get())

			If Not keepBlockingEDT.compareAndSet(False, True) Then
				log.fine("The secondary loop is already running, aborting")
				Return False
			End If

			Dim run As Runnable = New RunnableAnonymousInnerClassHelper

			' We have two mechanisms for blocking: if we're on the
			' dispatch thread, start a new event pump; if we're
			' on any other thread, call wait() on the treelock

			Dim currentThread As Thread = Thread.CurrentThread
			If currentThread Is dispatchThread Then
				If log.isLoggable(sun.util.logging.PlatformLogger.Level.FINEST) Then log.finest("On dispatch thread: " & dispatchThread)
				If interval <> 0 Then
					If log.isLoggable(sun.util.logging.PlatformLogger.Level.FINEST) Then log.finest("scheduling the timer for " & interval & " ms")
'JAVA TO VB CONVERTER TODO TASK: Assignments within expressions are not supported in VB
					timer.schedule(timerTask = New TimerTaskAnonymousInnerClassHelper
				End If
				' Dispose SequencedEvent we are dispatching on the the current
				' AppContext, to prevent us from hang - see 4531693 for details
				Dim currentSE As SequencedEvent = KeyboardFocusManager.currentKeyboardFocusManager.currentSequencedEvent
				If currentSE IsNot Nothing Then
					If log.isLoggable(sun.util.logging.PlatformLogger.Level.FINE) Then log.fine("Dispose current SequencedEvent: " & currentSE)
					currentSE.Dispose()
				End If
				' In case the exit() method is called before starting
				' new event pump it will post the waking event to EDT.
				' The event will be handled after the the new event pump
				' starts. Thus, the enter() method will not hang.
				'
				' Event pump should be privileged. See 6300270.
				java.security.AccessController.doPrivileged(New PrivilegedActionAnonymousInnerClassHelper(Of T)
			Else
				If log.isLoggable(sun.util.logging.PlatformLogger.Level.FINEST) Then log.finest("On non-dispatch thread: " & currentThread)
				SyncLock treeLock
					If filter IsNot Nothing Then dispatchThread.addEventFilter(filter)
					Try
						Dim eq As EventQueue = dispatchThread.eventQueue
						eq.postEvent(New sun.awt.PeerEvent(Me, run, sun.awt.PeerEvent.PRIORITY_EVENT))
						keepBlockingCT.set(True)
						If interval > 0 Then
							Dim currTime As Long = System.currentTimeMillis()
							Do While keepBlockingCT.get() AndAlso (If(extCondition IsNot Nothing, extCondition.evaluate(), True)) AndAlso (currTime + interval > System.currentTimeMillis())
								treeLock.wait(interval)
							Loop
						Else
							Do While keepBlockingCT.get() AndAlso (If(extCondition IsNot Nothing, extCondition.evaluate(), True))
								treeLock.wait()
							Loop
						End If
						If log.isLoggable(sun.util.logging.PlatformLogger.Level.FINE) Then log.fine("waitDone " & keepBlockingEDT.get() & " " & keepBlockingCT.get())
					Catch e As InterruptedException
						If log.isLoggable(sun.util.logging.PlatformLogger.Level.FINE) Then log.fine("Exception caught while waiting: " & e)
					Finally
						If filter IsNot Nothing Then dispatchThread.removeEventFilter(filter)
					End Try
					' If the waiting process has been stopped because of the
					' time interval passed or an exception occurred, the state
					' should be changed
					keepBlockingEDT.set(False)
					keepBlockingCT.set(False)
				End SyncLock
			End If

			Return True
		End Function

		Private Class RunnableAnonymousInnerClassHelper
			Implements Runnable

			Public Overridable Sub run() Implements Runnable.run
				log.fine("Starting a new event pump")
				If outerInstance.filter Is Nothing Then
					outerInstance.dispatchThread.pumpEvents(outerInstance.condition)
				Else
					outerInstance.dispatchThread.pumpEventsForFilter(outerInstance.condition, outerInstance.filter)
				End If
			End Sub
		End Class

		Private Class TimerTaskAnonymousInnerClassHelper
			Inherits java.util.TimerTask

			Public Overrides Sub run()
				If outerInstance.keepBlockingEDT.compareAndSet(True, False) Then outerInstance.wakeupEDT()
			End Sub
		End Class

		Private Class PrivilegedActionAnonymousInnerClassHelper(Of T)
			Implements java.security.PrivilegedAction(Of T)

			Public Overridable Function run() As Void
				run.run()
				Return Nothing
			End Function
		End Class

		''' <summary>
		''' {@inheritDoc}
		''' </summary>
		Public Overridable Function [exit]() As Boolean Implements SecondaryLoop.exit
			If log.isLoggable(sun.util.logging.PlatformLogger.Level.FINE) Then log.fine("exit(): blockingEDT=" & keepBlockingEDT.get() & ", blockingCT=" & keepBlockingCT.get())
			If keepBlockingEDT.compareAndSet(True, False) Then
				wakeupEDT()
				Return True
			End If
			Return False
		End Function

		PrivateShared ReadOnly PropertytreeLock As Object
			Get
				Return Component.LOCK
			End Get
		End Property

		Private ReadOnly wakingRunnable As Runnable = New RunnableAnonymousInnerClassHelper2

		Private Class RunnableAnonymousInnerClassHelper2
			Implements Runnable

			Public Overridable Sub run() Implements Runnable.run
				log.fine("Wake up EDT")
				SyncLock treeLock
					outerInstance.keepBlockingCT.set(False)
					treeLock.notifyAll()
				End SyncLock
				log.fine("Wake up EDT done")
			End Sub
		End Class

		Private Sub wakeupEDT()
			If log.isLoggable(sun.util.logging.PlatformLogger.Level.FINEST) Then log.finest("wakeupEDT(): EDT == " & dispatchThread)
			Dim eq As EventQueue = dispatchThread.eventQueue
			eq.postEvent(New sun.awt.PeerEvent(Me, wakingRunnable, sun.awt.PeerEvent.PRIORITY_EVENT))
		End Sub
	End Class

End Namespace