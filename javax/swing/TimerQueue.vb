Imports System.Diagnostics
Imports System.Text

'
' * Copyright (c) 1997, 2012, Oracle and/or its affiliates. All rights reserved.
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
	''' Internal class to manage all Timers using one thread.
	''' TimerQueue manages a queue of Timers. The Timers are chained
	''' together in a linked list sorted by the order in which they will expire.
	''' 
	''' @author Dave Moore
	''' @author Igor Kushnirskiy
	''' </summary>
	Friend Class TimerQueue
		Implements Runnable

		Private Shared ReadOnly sharedInstanceKey As Object = New StringBuilder("TimerQueue.sharedInstanceKey")
		Private Shared ReadOnly expiredTimersKey As Object = New StringBuilder("TimerQueue.expiredTimersKey")

		Private ReadOnly queue As DelayQueue(Of DelayedTimer)
'JAVA TO VB CONVERTER TODO TASK: There is no VB equivalent to 'volatile':
		Private running As Boolean
		Private ReadOnly runningLock As Lock

	'     Lock object used in place of class object for synchronization.
	'     * (4187686)
	'     
		Private Shared ReadOnly classLock As New Object

		''' <summary>
		''' Base of nanosecond timings, to avoid wrapping </summary>
		Private Shared ReadOnly NANO_ORIGIN As Long = System.nanoTime()

		''' <summary>
		''' Constructor for TimerQueue.
		''' </summary>
		Public Sub New()
			MyBase.New()
			queue = New DelayQueue(Of DelayedTimer)
			' Now start the TimerQueue thread.
			runningLock = New ReentrantLock
			startIfNeeded()
		End Sub


		Public Shared Function sharedInstance() As TimerQueue
			SyncLock classLock
				Dim sharedInst As TimerQueue = CType(SwingUtilities.appContextGet(sharedInstanceKey), TimerQueue)
				If sharedInst Is Nothing Then
					sharedInst = New TimerQueue
					SwingUtilities.appContextPut(sharedInstanceKey, sharedInst)
				End If
				Return sharedInst
			End SyncLock
		End Function


		Friend Overridable Sub startIfNeeded()
			If Not running Then
				runningLock.lock()
				Try
					Dim threadGroup As ThreadGroup = sun.awt.AppContext.appContext.threadGroup
'JAVA TO VB CONVERTER TODO TASK: Anonymous inner classes are not converted to VB if the base type is not defined in the code being converted:
'					java.security.AccessController.doPrivileged(New java.security.PrivilegedAction<Object>()
	'				{
	'					public Object run()
	'					{
	'						Thread timerThread = New Thread(threadGroup, TimerQueue.this, "TimerQueue");
	'						timerThread.setDaemon(True);
	'						timerThread.setPriority(Thread.NORM_PRIORITY);
	'						timerThread.start();
	'						Return Nothing;
	'					}
	'				});
					running = True
				Finally
					runningLock.unlock()
				End Try
			End If
		End Sub

		Friend Overridable Sub addTimer(ByVal ___timer As Timer, ByVal delayMillis As Long)
			___timer.lock.lock()
			Try
				' If the Timer is already in the queue, then ignore the add.
				If Not containsTimer(___timer) Then addTimer(New DelayedTimer(___timer, TimeUnit.MILLISECONDS.toNanos(delayMillis) + now()))
			Finally
				___timer.lock.unlock()
			End Try
		End Sub

		Private Sub addTimer(ByVal delayedTimer As DelayedTimer)
			Debug.Assert(delayedTimer IsNot Nothing AndAlso (Not containsTimer(delayedTimer.timer)))

			Dim ___timer As Timer = delayedTimer.timer
			___timer.lock.lock()
			Try
				___timer.delayedTimer = delayedTimer
				queue.add(delayedTimer)
			Finally
				___timer.lock.unlock()
			End Try
		End Sub

		Friend Overridable Sub removeTimer(ByVal ___timer As Timer)
			___timer.lock.lock()
			Try
				If ___timer.delayedTimer IsNot Nothing Then
					queue.remove(___timer.delayedTimer)
					___timer.delayedTimer = Nothing
				End If
			Finally
				___timer.lock.unlock()
			End Try
		End Sub

		Friend Overridable Function containsTimer(ByVal ___timer As Timer) As Boolean
			___timer.lock.lock()
			Try
				Return ___timer.delayedTimer IsNot Nothing
			Finally
				___timer.lock.unlock()
			End Try
		End Function


		Public Overridable Sub run()
			runningLock.lock()
			Try
				Do While running
					Try
						Dim ___timer As Timer = queue.take().timer
						___timer.lock.lock()
						Try
							Dim delayedTimer As DelayedTimer = ___timer.delayedTimer
							If delayedTimer IsNot Nothing Then
	'                            
	'                             * Timer is not removed after we get it from
	'                             * the queue and before the lock on the timer is
	'                             * acquired
	'                             
								___timer.post() ' have timer post an event
								___timer.delayedTimer = Nothing
								If ___timer.repeats Then
									delayedTimer.time = now() + TimeUnit.MILLISECONDS.toNanos(___timer.delay)
									addTimer(delayedTimer)
								End If
							End If

							' Allow run other threads on systems without kernel threads
							___timer.lock.newCondition().awaitNanos(1)
						Catch ignore As SecurityException
						Finally
							___timer.lock.unlock()
						End Try
					Catch ie As InterruptedException
						' Shouldn't ignore InterruptedExceptions here, so AppContext
						' is disposed gracefully, see 6799345 for details
						If sun.awt.AppContext.appContext.disposed Then Exit Do
					End Try
				Loop
			Catch td As ThreadDeath
				' Mark all the timers we contain as not being queued.
				For Each delayedTimer As DelayedTimer In queue
					delayedTimer.timer.cancelEvent()
				Next delayedTimer
				Throw td
			Finally
				running = False
				runningLock.unlock()
			End Try
		End Sub


		Public Overrides Function ToString() As String
			Dim buf As New StringBuilder
			buf.Append("TimerQueue (")
			Dim isFirst As Boolean = True
			For Each delayedTimer As DelayedTimer In queue
				If Not isFirst Then buf.Append(", ")
				buf.Append(delayedTimer.timer.ToString())
				isFirst = False
			Next delayedTimer
			buf.Append(")")
			Return buf.ToString()
		End Function

		''' <summary>
		''' Returns nanosecond time offset by origin
		''' </summary>
		Private Shared Function now() As Long
			Return System.nanoTime() - NANO_ORIGIN
		End Function

		Friend Class DelayedTimer
			Implements Delayed

			' most of it copied from
			' java.util.concurrent.ScheduledThreadPoolExecutor

			''' <summary>
			''' Sequence number to break scheduling ties, and in turn to
			''' guarantee FIFO order among tied entries.
			''' </summary>
			Private Shared ReadOnly sequencer As New java.util.concurrent.atomic.AtomicLong(0)

			''' <summary>
			''' Sequence number to break ties FIFO </summary>
			Private ReadOnly sequenceNumber As Long


			''' <summary>
			''' The time the task is enabled to execute in nanoTime units </summary>
'JAVA TO VB CONVERTER TODO TASK: There is no VB equivalent to 'volatile':
			Private time As Long

			Private ReadOnly ___timer As Timer

			Friend Sub New(ByVal ___timer As Timer, ByVal nanos As Long)
				Me.___timer = ___timer
				time = nanos
				sequenceNumber = sequencer.andIncrement
			End Sub


			Public Function getDelay(ByVal unit As TimeUnit) As Long
				Return unit.convert(time - now(), TimeUnit.NANOSECONDS)
			End Function

			Friend Property time As Long
				Set(ByVal nanos As Long)
					time = nanos
				End Set
			End Property

			Friend Property timer As Timer
				Get
					Return ___timer
				End Get
			End Property

			Public Overridable Function compareTo(ByVal other As Delayed) As Integer
				If other Is Me Then ' compare zero ONLY if same object Return 0
				If TypeOf other Is DelayedTimer Then
					Dim x As DelayedTimer = CType(other, DelayedTimer)
					Dim diff As Long = time - x.time
					If diff < 0 Then
						Return -1
					ElseIf diff > 0 Then
						Return 1
					ElseIf sequenceNumber < x.sequenceNumber Then
						Return -1
					Else
						Return 1
					End If
				End If
				Dim d As Long = (getDelay(TimeUnit.NANOSECONDS) - other.getDelay(TimeUnit.NANOSECONDS))
				Return If(d = 0, 0, (If(d < 0, -1, 1)))
			End Function
		End Class
	End Class

End Namespace