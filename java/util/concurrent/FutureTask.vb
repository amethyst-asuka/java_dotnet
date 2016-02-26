Imports System
Imports System.Threading

'
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

'
' *
' *
' *
' *
' *
' * Written by Doug Lea with assistance from members of JCP JSR-166
' * Expert Group and released to the public domain, as explained at
' * http://creativecommons.org/publicdomain/zero/1.0/
' 

Namespace java.util.concurrent

	''' <summary>
	''' A cancellable asynchronous computation.  This class provides a base
	''' implementation of <seealso cref="Future"/>, with methods to start and cancel
	''' a computation, query to see if the computation is complete, and
	''' retrieve the result of the computation.  The result can only be
	''' retrieved when the computation has completed; the {@code get}
	''' methods will block if the computation has not yet completed.  Once
	''' the computation has completed, the computation cannot be restarted
	''' or cancelled (unless the computation is invoked using
	''' <seealso cref="#runAndReset"/>).
	''' 
	''' <p>A {@code FutureTask} can be used to wrap a <seealso cref="Callable"/> or
	''' <seealso cref="Runnable"/> object.  Because {@code FutureTask} implements
	''' {@code Runnable}, a {@code FutureTask} can be submitted to an
	''' <seealso cref="Executor"/> for execution.
	''' 
	''' <p>In addition to serving as a standalone [Class], this class provides
	''' {@code protected} functionality that may be useful when creating
	''' customized task classes.
	''' 
	''' @since 1.5
	''' @author Doug Lea </summary>
	''' @param <V> The result type returned by this FutureTask's {@code get} methods </param>
	Public Class FutureTask(Of V)
		Implements RunnableFuture(Of V)

	'    
	'     * Revision notes: This differs from previous versions of this
	'     * class that relied on AbstractQueuedSynchronizer, mainly to
	'     * avoid surprising users about retaining interrupt status during
	'     * cancellation races. Sync control in the current design relies
	'     * on a "state" field updated via CAS to track completion, along
	'     * with a simple Treiber stack to hold waiting threads.
	'     *
	'     * Style note: As usual, we bypass overhead of using
	'     * AtomicXFieldUpdaters and instead directly use Unsafe intrinsics.
	'     

		''' <summary>
		''' The run state of this task, initially NEW.  The run state
		''' transitions to a terminal state only in methods set,
		''' setException, and cancel.  During completion, state may take on
		''' transient values of COMPLETING (while outcome is being set) or
		''' INTERRUPTING (only while interrupting the runner to satisfy a
		''' cancel(true)). Transitions from these intermediate to final
		''' states use cheaper ordered/lazy writes because values are unique
		''' and cannot be further modified.
		''' 
		''' Possible state transitions:
		''' NEW -> COMPLETING -> NORMAL
		''' NEW -> COMPLETING -> EXCEPTIONAL
		''' NEW -> CANCELLED
		''' NEW -> INTERRUPTING -> INTERRUPTED
		''' </summary>
'JAVA TO VB CONVERTER TODO TASK: There is no VB equivalent to 'volatile':
		Private state As Integer
		Private Const [NEW] As Integer = 0
		Private Const COMPLETING As Integer = 1
		Private Const NORMAL As Integer = 2
		Private Const EXCEPTIONAL As Integer = 3
		Private Const CANCELLED As Integer = 4
		Private Const INTERRUPTING As Integer = 5
		Private Const INTERRUPTED As Integer = 6

		''' <summary>
		''' The underlying callable; nulled out after running </summary>
		Private callable As Callable(Of V)
		''' <summary>
		''' The result to return or exception to throw from get() </summary>
		Private outcome As Object ' non-volatile, protected by state reads/writes
		''' <summary>
		''' The thread running the callable; CASed during run() </summary>
'JAVA TO VB CONVERTER TODO TASK: There is no VB equivalent to 'volatile':
		Private runner As Thread
		''' <summary>
		''' Treiber stack of waiting threads </summary>
'JAVA TO VB CONVERTER TODO TASK: There is no VB equivalent to 'volatile':
		Private waiters As WaitNode

		''' <summary>
		''' Returns result or throws exception for completed task.
		''' </summary>
		''' <param name="s"> completed state value </param>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
		Private Function report(ByVal s As Integer) As V
			Dim x As Object = outcome
			If s = NORMAL Then Return CType(x, V)
			If s >= CANCELLED Then Throw New CancellationException
			Throw New ExecutionException(CType(x, Throwable))
		End Function

		''' <summary>
		''' Creates a {@code FutureTask} that will, upon running, execute the
		''' given {@code Callable}.
		''' </summary>
		''' <param name="callable"> the callable task </param>
		''' <exception cref="NullPointerException"> if the callable is null </exception>
		Public Sub New(ByVal callable As Callable(Of V))
			If callable Is Nothing Then Throw New NullPointerException
			Me.callable = callable
			Me.state = [NEW] ' ensure visibility of callable
		End Sub

		''' <summary>
		''' Creates a {@code FutureTask} that will, upon running, execute the
		''' given {@code Runnable}, and arrange that {@code get} will return the
		''' given result on successful completion.
		''' </summary>
		''' <param name="runnable"> the runnable task </param>
		''' <param name="result"> the result to return on successful completion. If
		''' you don't need a particular result, consider using
		''' constructions of the form:
		''' {@code Future<?> f = new FutureTask<Void>(runnable, null)} </param>
		''' <exception cref="NullPointerException"> if the runnable is null </exception>
		Public Sub New(ByVal runnable As Runnable, ByVal result As V)
			Me.callable = Executors.callable(runnable, result)
			Me.state = [NEW] ' ensure visibility of callable
		End Sub

		Public Overridable Property cancelled As Boolean
			Get
				Return state >= CANCELLED
			End Get
		End Property

		Public Overridable Property done As Boolean
			Get
				Return state <> [NEW]
			End Get
		End Property

		Public Overridable Function cancel(ByVal mayInterruptIfRunning As Boolean) As Boolean
			If Not(state = [NEW] AndAlso UNSAFE.compareAndSwapInt(Me, stateOffset, [NEW],If(mayInterruptIfRunning, INTERRUPTING, CANCELLED))) Then Return False
			Try ' in case call to interrupt throws exception
				If mayInterruptIfRunning Then
					Try
						Dim t As Thread = runner
						If t IsNot Nothing Then
							t.interrupt()
						End If ' final state
					Finally
						UNSAFE.putOrderedInt(Me, stateOffset, INTERRUPTED)
					End Try
				End If
			Finally
				finishCompletion()
			End Try
			Return True
		End Function

		''' <exception cref="CancellationException"> {@inheritDoc} </exception>
		Public Overridable Function [get]() As V
			Dim s As Integer = state
			If s <= COMPLETING Then s = awaitDone(False, 0L)
			Return report(s)
		End Function

		''' <exception cref="CancellationException"> {@inheritDoc} </exception>
		Public Overridable Function [get](ByVal timeout As Long, ByVal unit As TimeUnit) As V
			If unit Is Nothing Then Throw New NullPointerException
			Dim s As Integer = state
			s = awaitDone(True, unit.toNanos(timeout))
			If s <= COMPLETING AndAlso s <= COMPLETING Then Throw New TimeoutException
			Return report(s)
		End Function

		''' <summary>
		''' Protected method invoked when this task transitions to state
		''' {@code isDone} (whether normally or via cancellation). The
		''' default implementation does nothing.  Subclasses may override
		''' this method to invoke completion callbacks or perform
		''' bookkeeping. Note that you can query status inside the
		''' implementation of this method to determine whether this task
		''' has been cancelled.
		''' </summary>
		Protected Friend Overridable Sub done()
		End Sub

		''' <summary>
		''' Sets the result of this future to the given value unless
		''' this future has already been set or has been cancelled.
		''' 
		''' <p>This method is invoked internally by the <seealso cref="#run"/> method
		''' upon successful completion of the computation.
		''' </summary>
		''' <param name="v"> the value </param>
		Protected Friend Overridable Sub [set](ByVal v As V)
			If UNSAFE.compareAndSwapInt(Me, stateOffset, [NEW], COMPLETING) Then
				outcome = v
				UNSAFE.putOrderedInt(Me, stateOffset, NORMAL) ' final state
				finishCompletion()
			End If
		End Sub

		''' <summary>
		''' Causes this future to report an <seealso cref="ExecutionException"/>
		''' with the given throwable as its cause, unless this future has
		''' already been set or has been cancelled.
		''' 
		''' <p>This method is invoked internally by the <seealso cref="#run"/> method
		''' upon failure of the computation.
		''' </summary>
		''' <param name="t"> the cause of failure </param>
		Protected Friend Overridable Property exception As Throwable
			Set(ByVal t As Throwable)
				If UNSAFE.compareAndSwapInt(Me, stateOffset, [NEW], COMPLETING) Then
					outcome = t
					UNSAFE.putOrderedInt(Me, stateOffset, EXCEPTIONAL) ' final state
					finishCompletion()
				End If
			End Set
		End Property

		Public Overridable Sub run() Implements RunnableFuture(Of V).run
			If state <> [NEW] OrElse (Not UNSAFE.compareAndSwapObject(Me, runnerOffset, Nothing, Thread.CurrentThread)) Then Return
			Try
				Dim c As Callable(Of V) = callable
				If c IsNot Nothing AndAlso state = [NEW] Then
					Dim result As V
					Dim ran As Boolean
					Try
						result = c.call()
						ran = True
					Catch ex As Throwable
						result = Nothing
						ran = False
						exception = ex
					End Try
					If ran Then [set](result)
				End If
			Finally
				' runner must be non-null until state is settled to
				' prevent concurrent calls to run()
				runner = Nothing
				' state must be re-read after nulling runner to prevent
				' leaked interrupts
				Dim s As Integer = state
				If s >= INTERRUPTING Then handlePossibleCancellationInterrupt(s)
			End Try
		End Sub

		''' <summary>
		''' Executes the computation without setting its result, and then
		''' resets this future to initial state, failing to do so if the
		''' computation encounters an exception or is cancelled.  This is
		''' designed for use with tasks that intrinsically execute more
		''' than once.
		''' </summary>
		''' <returns> {@code true} if successfully run and reset </returns>
		Protected Friend Overridable Function runAndReset() As Boolean
			If state <> [NEW] OrElse (Not UNSAFE.compareAndSwapObject(Me, runnerOffset, Nothing, Thread.CurrentThread)) Then Return False
			Dim ran As Boolean = False
			Dim s As Integer = state
			Try
				Dim c As Callable(Of V) = callable
				If c IsNot Nothing AndAlso s = [NEW] Then
					Try
						c.call() ' don't set result
						ran = True
					Catch ex As Throwable
						exception = ex
					End Try
				End If
			Finally
				' runner must be non-null until state is settled to
				' prevent concurrent calls to run()
				runner = Nothing
				' state must be re-read after nulling runner to prevent
				' leaked interrupts
				s = state
				If s >= INTERRUPTING Then handlePossibleCancellationInterrupt(s)
			End Try
			Return ran AndAlso s = [NEW]
		End Function

		''' <summary>
		''' Ensures that any interrupt from a possible cancel(true) is only
		''' delivered to a task while in run or runAndReset.
		''' </summary>
		Private Sub handlePossibleCancellationInterrupt(ByVal s As Integer)
			' It is possible for our interrupter to stall before getting a
			' chance to interrupt us.  Let's spin-wait patiently.
			If s = INTERRUPTING Then
				Do While state = INTERRUPTING
					Thread.yield() ' wait out pending interrupt
				Loop
			End If

			' assert state == INTERRUPTED;

			' We want to clear any interrupt we may have received from
			' cancel(true).  However, it is permissible to use interrupts
			' as an independent mechanism for a task to communicate with
			' its caller, and there is no way to clear only the
			' cancellation interrupt.
			'
			' Thread.interrupted();
		End Sub

		''' <summary>
		''' Simple linked list nodes to record waiting threads in a Treiber
		''' stack.  See other classes such as Phaser and SynchronousQueue
		''' for more detailed explanation.
		''' </summary>
		Friend NotInheritable Class WaitNode
'JAVA TO VB CONVERTER TODO TASK: There is no VB equivalent to 'volatile':
			Friend thread_Renamed As Thread
'JAVA TO VB CONVERTER TODO TASK: There is no VB equivalent to 'volatile':
			Friend [next] As WaitNode
			Friend Sub New()
				thread_Renamed = Thread.CurrentThread
			End Sub
		End Class

		''' <summary>
		''' Removes and signals all waiting threads, invokes done(), and
		''' nulls out callable.
		''' </summary>
		Private Sub finishCompletion()
			' assert state > COMPLETING;
			Dim q As WaitNode
'JAVA TO VB CONVERTER TODO TASK: Assignments within expressions are not supported in VB
			Do While (q = waiters) IsNot Nothing
				If UNSAFE.compareAndSwapObject(Me, waitersOffset, q, Nothing) Then
					Do
						Dim t As Thread = q.thread_Renamed
						If t IsNot Nothing Then
							q.thread_Renamed = Nothing
							java.util.concurrent.locks.LockSupport.unpark(t)
						End If
						Dim [next] As WaitNode = q.next
						If [next] Is Nothing Then Exit Do
						q.next = Nothing ' unlink to help gc
						q = [next]
					Loop
					Exit Do
				End If
			Loop

			done()

			callable = Nothing ' to reduce footprint
		End Sub

		''' <summary>
		''' Awaits completion or aborts on interrupt or timeout.
		''' </summary>
		''' <param name="timed"> true if use timed waits </param>
		''' <param name="nanos"> time to wait, if timed </param>
		''' <returns> state upon completion </returns>
		Private Function awaitDone(ByVal timed As Boolean, ByVal nanos As Long) As Integer
			Dim deadline As Long = If(timed, System.nanoTime() + nanos, 0L)
			Dim q As WaitNode = Nothing
			Dim queued As Boolean = False
			Do
				If Thread.interrupted() Then
					removeWaiter(q)
					Throw New InterruptedException
				End If

				Dim s As Integer = state
				If s > COMPLETING Then
					If q IsNot Nothing Then q.thread_Renamed = Nothing
					Return s
				ElseIf s = COMPLETING Then ' cannot time out yet
					Thread.yield()
				ElseIf q Is Nothing Then
					q = New WaitNode
				ElseIf Not queued Then
'JAVA TO VB CONVERTER TODO TASK: Assignments within expressions are not supported in VB
					queued = UNSAFE.compareAndSwapObject(Me, waitersOffset, q.next = waiters, q)
				ElseIf timed Then
					nanos = deadline - System.nanoTime()
					If nanos <= 0L Then
						removeWaiter(q)
						Return state
					End If
					java.util.concurrent.locks.LockSupport.parkNanos(Me, nanos)
				Else
					java.util.concurrent.locks.LockSupport.park(Me)
				End If
			Loop
		End Function

		''' <summary>
		''' Tries to unlink a timed-out or interrupted wait node to avoid
		''' accumulating garbage.  Internal nodes are simply unspliced
		''' without CAS since it is harmless if they are traversed anyway
		''' by releasers.  To avoid effects of unsplicing from already
		''' removed nodes, the list is retraversed in case of an apparent
		''' race.  This is slow when there are a lot of nodes, but we don't
		''' expect lists to be long enough to outweigh higher-overhead
		''' schemes.
		''' </summary>
		Private Sub removeWaiter(ByVal node As WaitNode)
			If node IsNot Nothing Then
				node.thread_Renamed = Nothing
				retry:
				Do ' restart on removeWaiter race
'JAVA TO VB CONVERTER TODO TASK: The following line could not be converted:
					for (WaitNode pred = Nothing, q = waiters, s; q != Nothing; q = s)
						s = q.next
						If q.thread_Renamed IsNot Nothing Then
							pred = q
						ElseIf pred IsNot Nothing Then
							pred.next = s
							If pred.thread_Renamed Is Nothing Then ' check for race GoTo retry
						ElseIf Not UNSAFE.compareAndSwapObject(Me, waitersOffset, q, s) Then
							GoTo retry
						End If
					Exit Do
				Loop
			End If
		End Sub

		' Unsafe mechanics
		Private Shared ReadOnly UNSAFE As sun.misc.Unsafe
		Private Shared ReadOnly stateOffset As Long
		Private Shared ReadOnly runnerOffset As Long
		Private Shared ReadOnly waitersOffset As Long
		Shared Sub New()
			Try
				UNSAFE = sun.misc.Unsafe.unsafe
				Dim k As  [Class] = GetType(FutureTask)
				stateOffset = UNSAFE.objectFieldOffset(k.getDeclaredField("state"))
				runnerOffset = UNSAFE.objectFieldOffset(k.getDeclaredField("runner"))
				waitersOffset = UNSAFE.objectFieldOffset(k.getDeclaredField("waiters"))
			Catch e As Exception
				Throw New [Error](e)
			End Try
		End Sub

	End Class

End Namespace