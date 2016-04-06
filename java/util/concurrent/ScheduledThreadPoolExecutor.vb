Imports System
Imports System.Collections.Generic
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
	''' A <seealso cref="ThreadPoolExecutor"/> that can additionally schedule
	''' commands to run after a given delay, or to execute
	''' periodically. This class is preferable to <seealso cref="java.util.Timer"/>
	''' when multiple worker threads are needed, or when the additional
	''' flexibility or capabilities of <seealso cref="ThreadPoolExecutor"/> (which
	''' this class extends) are required.
	''' 
	''' <p>Delayed tasks execute no sooner than they are enabled, but
	''' without any real-time guarantees about when, after they are
	''' enabled, they will commence. Tasks scheduled for exactly the same
	''' execution time are enabled in first-in-first-out (FIFO) order of
	''' submission.
	''' 
	''' <p>When a submitted task is cancelled before it is run, execution
	''' is suppressed. By default, such a cancelled task is not
	''' automatically removed from the work queue until its delay
	''' elapses. While this enables further inspection and monitoring, it
	''' may also cause unbounded retention of cancelled tasks. To avoid
	''' this, set <seealso cref="#setRemoveOnCancelPolicy"/> to {@code true}, which
	''' causes tasks to be immediately removed from the work queue at
	''' time of cancellation.
	''' 
	''' <p>Successive executions of a task scheduled via
	''' {@code scheduleAtFixedRate} or
	''' {@code scheduleWithFixedDelay} do not overlap. While different
	''' executions may be performed by different threads, the effects of
	''' prior executions <a
	''' href="package-summary.html#MemoryVisibility"><i>happen-before</i></a>
	''' those of subsequent ones.
	''' 
	''' <p>While this class inherits from <seealso cref="ThreadPoolExecutor"/>, a few
	''' of the inherited tuning methods are not useful for it. In
	''' particular, because it acts as a fixed-sized pool using
	''' {@code corePoolSize} threads and an unbounded queue, adjustments
	''' to {@code maximumPoolSize} have no useful effect. Additionally, it
	''' is almost never a good idea to set {@code corePoolSize} to zero or
	''' use {@code allowCoreThreadTimeOut} because this may leave the pool
	''' without threads to handle tasks once they become eligible to run.
	''' 
	''' <p><b>Extension notes:</b> This class overrides the
	''' <seealso cref="ThreadPoolExecutor#execute(Runnable) execute"/> and
	''' <seealso cref="AbstractExecutorService#submit(Runnable) submit"/>
	''' methods to generate internal <seealso cref="ScheduledFuture"/> objects to
	''' control per-task delays and scheduling.  To preserve
	''' functionality, any further overrides of these methods in
	''' subclasses must invoke superclass versions, which effectively
	''' disables additional task customization.  However, this class
	''' provides alternative protected extension method
	''' {@code decorateTask} (one version each for {@code Runnable} and
	''' {@code Callable}) that can be used to customize the concrete task
	''' types used to execute commands entered via {@code execute},
	''' {@code submit}, {@code schedule}, {@code scheduleAtFixedRate},
	''' and {@code scheduleWithFixedDelay}.  By default, a
	''' {@code ScheduledThreadPoolExecutor} uses a task type extending
	''' <seealso cref="FutureTask"/>. However, this may be modified or replaced using
	''' subclasses of the form:
	''' 
	'''  <pre> {@code
	''' public class CustomScheduledExecutor extends ScheduledThreadPoolExecutor {
	''' 
	'''   static class CustomTask<V> implements RunnableScheduledFuture<V> { ... }
	''' 
	'''   protected <V> RunnableScheduledFuture<V> decorateTask(
	'''                Runnable r, RunnableScheduledFuture<V> task) {
	'''       return new CustomTask<V>(r, task);
	'''   }
	''' 
	'''   protected <V> RunnableScheduledFuture<V> decorateTask(
	'''                Callable<V> c, RunnableScheduledFuture<V> task) {
	'''       return new CustomTask<V>(c, task);
	'''   }
	'''   // ... add constructors, etc.
	''' }}</pre>
	''' 
	''' @since 1.5
	''' @author Doug Lea
	''' </summary>
	Public Class ScheduledThreadPoolExecutor
		Inherits ThreadPoolExecutor
		Implements ScheduledExecutorService

	'    
	'     * This class specializes ThreadPoolExecutor implementation by
	'     *
	'     * 1. Using a custom task type, ScheduledFutureTask for
	'     *    tasks, even those that don't require scheduling (i.e.,
	'     *    those submitted using ExecutorService execute, not
	'     *    ScheduledExecutorService methods) which are treated as
	'     *    delayed tasks with a delay of zero.
	'     *
	'     * 2. Using a custom queue (DelayedWorkQueue), a variant of
	'     *    unbounded DelayQueue. The lack of capacity constraint and
	'     *    the fact that corePoolSize and maximumPoolSize are
	'     *    effectively identical simplifies some execution mechanics
	'     *    (see delayedExecute) compared to ThreadPoolExecutor.
	'     *
	'     * 3. Supporting optional run-after-shutdown parameters, which
	'     *    leads to overrides of shutdown methods to remove and cancel
	'     *    tasks that should NOT be run after shutdown, as well as
	'     *    different recheck logic when task (re)submission overlaps
	'     *    with a shutdown.
	'     *
	'     * 4. Task decoration methods to allow interception and
	'     *    instrumentation, which are needed because subclasses cannot
	'     *    otherwise override submit methods to get this effect. These
	'     *    don't have any impact on pool control logic though.
	'     

		''' <summary>
		''' False if should cancel/suppress periodic tasks on shutdown.
		''' </summary>
'JAVA TO VB CONVERTER TODO TASK: There is no VB equivalent to 'volatile':
		Private continueExistingPeriodicTasksAfterShutdown As Boolean

		''' <summary>
		''' False if should cancel non-periodic tasks on shutdown.
		''' </summary>
'JAVA TO VB CONVERTER TODO TASK: There is no VB equivalent to 'volatile':
		Private executeExistingDelayedTasksAfterShutdown As Boolean = True

		''' <summary>
		''' True if ScheduledFutureTask.cancel should remove from queue
		''' </summary>
'JAVA TO VB CONVERTER TODO TASK: There is no VB equivalent to 'volatile':
		Private removeOnCancel As Boolean = False

		''' <summary>
		''' Sequence number to break scheduling ties, and in turn to
		''' guarantee FIFO order among tied entries.
		''' </summary>
		Private Shared ReadOnly sequencer As New java.util.concurrent.atomic.AtomicLong

		''' <summary>
		''' Returns current nanosecond time.
		''' </summary>
		Friend Function now() As Long
			Return System.nanoTime()
		End Function

		Private Class ScheduledFutureTask(Of V)
			Inherits FutureTask(Of V)
			Implements RunnableScheduledFuture(Of V)

			Private ReadOnly outerInstance As ScheduledThreadPoolExecutor


			''' <summary>
			''' Sequence number to break ties FIFO </summary>
			Private ReadOnly sequenceNumber As Long

			''' <summary>
			''' The time the task is enabled to execute in nanoTime units </summary>
			Private time As Long

			''' <summary>
			''' Period in nanoseconds for repeating tasks.  A positive
			''' value indicates fixed-rate execution.  A negative value
			''' indicates fixed-delay execution.  A value of 0 indicates a
			''' non-repeating task.
			''' </summary>
			Private ReadOnly period As Long

			''' <summary>
			''' The actual task to be re-enqueued by reExecutePeriodic </summary>
			Friend outerTask As RunnableScheduledFuture(Of V) = Me

			''' <summary>
			''' Index into delay queue, to support faster cancellation.
			''' </summary>
			Friend heapIndex As Integer

			''' <summary>
			''' Creates a one-shot action with given nanoTime-based trigger time.
			''' </summary>
			Friend Sub New(  outerInstance As ScheduledThreadPoolExecutor,   r As Runnable,   result As V,   ns As Long)
					Me.outerInstance = outerInstance
				MyBase.New(r, result)
				Me.time = ns
				Me.period = 0
				Me.sequenceNumber = sequencer.andIncrement
			End Sub

			''' <summary>
			''' Creates a periodic action with given nano time and period.
			''' </summary>
			Friend Sub New(  outerInstance As ScheduledThreadPoolExecutor,   r As Runnable,   result As V,   ns As Long,   period As Long)
					Me.outerInstance = outerInstance
				MyBase.New(r, result)
				Me.time = ns
				Me.period = period
				Me.sequenceNumber = sequencer.andIncrement
			End Sub

			''' <summary>
			''' Creates a one-shot action with given nanoTime-based trigger time.
			''' </summary>
			Friend Sub New(  outerInstance As ScheduledThreadPoolExecutor,   callable As Callable(Of V),   ns As Long)
					Me.outerInstance = outerInstance
				MyBase.New(callable)
				Me.time = ns
				Me.period = 0
				Me.sequenceNumber = sequencer.andIncrement
			End Sub

			Public Overridable Function getDelay(  unit As TimeUnit) As Long
				Return unit.convert(time - outerInstance.now(), NANOSECONDS)
			End Function

			Public Overridable Function compareTo(  other As Delayed) As Integer
				If other Is Me Then ' compare zero if same object Return 0
				If TypeOf other Is ScheduledFutureTask Then
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
					Dim x As ScheduledFutureTask(Of ?) = CType(other, ScheduledFutureTask(Of ?))
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
				Dim diff As Long = getDelay(NANOSECONDS) - other.getDelay(NANOSECONDS)
				Return If(diff < 0, -1, If(diff > 0, 1, 0))
			End Function

			''' <summary>
			''' Returns {@code true} if this is a periodic (not a one-shot) action.
			''' </summary>
			''' <returns> {@code true} if periodic </returns>
			Public Overridable Property periodic As Boolean Implements RunnableScheduledFuture(Of V).isPeriodic
				Get
					Return period <> 0
				End Get
			End Property

			''' <summary>
			''' Sets the next time to run for a periodic task.
			''' </summary>
			Private Sub setNextRunTime()
				Dim p As Long = period
				If p > 0 Then
					time += p
				Else
					time = outerInstance.triggerTime(-p)
				End If
			End Sub

			Public Overridable Function cancel(  mayInterruptIfRunning As Boolean) As Boolean
				Dim cancelled_Renamed As Boolean = MyBase.cancel(mayInterruptIfRunning)
				If cancelled_Renamed AndAlso outerInstance.removeOnCancel AndAlso heapIndex >= 0 Then outerInstance.remove(Me)
				Return cancelled_Renamed
			End Function

			''' <summary>
			''' Overrides FutureTask version so as to reset/requeue if periodic.
			''' </summary>
			Public Overridable Sub run()
				Dim periodic_Renamed As Boolean = periodic
				If Not outerInstance.canRunInCurrentRunState(periodic_Renamed) Then
					cancel(False)
				ElseIf Not periodic_Renamed Then
					outerInstance.run()
				ElseIf outerInstance.runAndReset() Then
					nextRunTimeime()
					outerInstance.reExecutePeriodic(outerTask)
				End If
			End Sub
		End Class

		''' <summary>
		''' Returns true if can run a task given current run state
		''' and run-after-shutdown parameters.
		''' </summary>
		''' <param name="periodic"> true if this task periodic, false if delayed </param>
		Friend Overridable Function canRunInCurrentRunState(  periodic As Boolean) As Boolean
			Return isRunningOrShutdown(If(periodic, continueExistingPeriodicTasksAfterShutdown, executeExistingDelayedTasksAfterShutdown))
		End Function

		''' <summary>
		''' Main execution method for delayed or periodic tasks.  If pool
		''' is shut down, rejects the task. Otherwise adds task to queue
		''' and starts a thread, if necessary, to run it.  (We cannot
		''' prestart the thread to run the task because the task (probably)
		''' shouldn't be run yet.)  If the pool is shut down while the task
		''' is being added, cancel and remove it if required by state and
		''' run-after-shutdown parameters.
		''' </summary>
		''' <param name="task"> the task </param>
		Private Sub delayedExecute(Of T1)(  task As RunnableScheduledFuture(Of T1))
			If shutdown Then
				reject(task)
			Else
				MyBase.queue.add(task)
				If shutdown AndAlso (Not canRunInCurrentRunState(task.periodic)) AndAlso remove(task) Then
					task.cancel(False)
				Else
					ensurePrestart()
				End If
			End If
		End Sub

		''' <summary>
		''' Requeues a periodic task unless current run state precludes it.
		''' Same idea as delayedExecute except drops task rather than rejecting.
		''' </summary>
		''' <param name="task"> the task </param>
		Friend Overridable Sub reExecutePeriodic(Of T1)(  task As RunnableScheduledFuture(Of T1))
			If canRunInCurrentRunState(True) Then
				MyBase.queue.add(task)
				If (Not canRunInCurrentRunState(True)) AndAlso remove(task) Then
					task.cancel(False)
				Else
					ensurePrestart()
				End If
			End If
		End Sub

		''' <summary>
		''' Cancels and clears the queue of all tasks that should not be run
		''' due to shutdown policy.  Invoked within super.shutdown.
		''' </summary>
		Friend Overrides Sub onShutdown()
			Dim q As BlockingQueue(Of Runnable) = MyBase.queue
			Dim keepDelayed As Boolean = executeExistingDelayedTasksAfterShutdownPolicy
			Dim keepPeriodic As Boolean = continueExistingPeriodicTasksAfterShutdownPolicy
			If (Not keepDelayed) AndAlso (Not keepPeriodic) Then
				For Each e As Object In q.ToArray()
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
					If TypeOf e Is RunnableScheduledFuture(Of ?) Then CType(e, RunnableScheduledFuture(Of ?)).cancel(False)
				Next e
				q.clear()
			Else
				' Traverse snapshot to avoid iterator exceptions
				For Each e As Object In q.ToArray()
					If TypeOf e Is RunnableScheduledFuture Then
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
						Dim t As RunnableScheduledFuture(Of ?) = CType(e, RunnableScheduledFuture(Of ?))
						If (If(t.periodic, (Not keepPeriodic), (Not keepDelayed))) OrElse t.cancelled Then ' also remove if already cancelled
							If q.remove(t) Then t.cancel(False)
						End If
					End If
				Next e
			End If
			tryTerminate()
		End Sub

		''' <summary>
		''' Modifies or replaces the task used to execute a runnable.
		''' This method can be used to override the concrete
		''' class used for managing internal tasks.
		''' The default implementation simply returns the given task.
		''' </summary>
		''' <param name="runnable"> the submitted Runnable </param>
		''' <param name="task"> the task created to execute the runnable </param>
		''' @param <V> the type of the task's result </param>
		''' <returns> a task that can execute the runnable
		''' @since 1.6 </returns>
		Protected Friend Overridable Function decorateTask(Of V)(  runnable As Runnable,   task As RunnableScheduledFuture(Of V)) As RunnableScheduledFuture(Of V)
			Return task
		End Function

		''' <summary>
		''' Modifies or replaces the task used to execute a callable.
		''' This method can be used to override the concrete
		''' class used for managing internal tasks.
		''' The default implementation simply returns the given task.
		''' </summary>
		''' <param name="callable"> the submitted Callable </param>
		''' <param name="task"> the task created to execute the callable </param>
		''' @param <V> the type of the task's result </param>
		''' <returns> a task that can execute the callable
		''' @since 1.6 </returns>
		Protected Friend Overridable Function decorateTask(Of V)(  callable As Callable(Of V),   task As RunnableScheduledFuture(Of V)) As RunnableScheduledFuture(Of V)
			Return task
		End Function

		''' <summary>
		''' Creates a new {@code ScheduledThreadPoolExecutor} with the
		''' given core pool size.
		''' </summary>
		''' <param name="corePoolSize"> the number of threads to keep in the pool, even
		'''        if they are idle, unless {@code allowCoreThreadTimeOut} is set </param>
		''' <exception cref="IllegalArgumentException"> if {@code corePoolSize < 0} </exception>
		Public Sub New(  corePoolSize As Integer)
			MyBase.New(corePoolSize,  java.lang.[Integer].Max_Value, 0, NANOSECONDS, New DelayedWorkQueue)
		End Sub

		''' <summary>
		''' Creates a new {@code ScheduledThreadPoolExecutor} with the
		''' given initial parameters.
		''' </summary>
		''' <param name="corePoolSize"> the number of threads to keep in the pool, even
		'''        if they are idle, unless {@code allowCoreThreadTimeOut} is set </param>
		''' <param name="threadFactory"> the factory to use when the executor
		'''        creates a new thread </param>
		''' <exception cref="IllegalArgumentException"> if {@code corePoolSize < 0} </exception>
		''' <exception cref="NullPointerException"> if {@code threadFactory} is null </exception>
		Public Sub New(  corePoolSize As Integer,   threadFactory As ThreadFactory)
			MyBase.New(corePoolSize,  java.lang.[Integer].Max_Value, 0, NANOSECONDS, New DelayedWorkQueue, threadFactory)
		End Sub

		''' <summary>
		''' Creates a new ScheduledThreadPoolExecutor with the given
		''' initial parameters.
		''' </summary>
		''' <param name="corePoolSize"> the number of threads to keep in the pool, even
		'''        if they are idle, unless {@code allowCoreThreadTimeOut} is set </param>
		''' <param name="handler"> the handler to use when execution is blocked
		'''        because the thread bounds and queue capacities are reached </param>
		''' <exception cref="IllegalArgumentException"> if {@code corePoolSize < 0} </exception>
		''' <exception cref="NullPointerException"> if {@code handler} is null </exception>
		Public Sub New(  corePoolSize As Integer,   handler As RejectedExecutionHandler)
			MyBase.New(corePoolSize,  java.lang.[Integer].Max_Value, 0, NANOSECONDS, New DelayedWorkQueue, handler)
		End Sub

		''' <summary>
		''' Creates a new ScheduledThreadPoolExecutor with the given
		''' initial parameters.
		''' </summary>
		''' <param name="corePoolSize"> the number of threads to keep in the pool, even
		'''        if they are idle, unless {@code allowCoreThreadTimeOut} is set </param>
		''' <param name="threadFactory"> the factory to use when the executor
		'''        creates a new thread </param>
		''' <param name="handler"> the handler to use when execution is blocked
		'''        because the thread bounds and queue capacities are reached </param>
		''' <exception cref="IllegalArgumentException"> if {@code corePoolSize < 0} </exception>
		''' <exception cref="NullPointerException"> if {@code threadFactory} or
		'''         {@code handler} is null </exception>
		Public Sub New(  corePoolSize As Integer,   threadFactory As ThreadFactory,   handler As RejectedExecutionHandler)
			MyBase.New(corePoolSize,  java.lang.[Integer].Max_Value, 0, NANOSECONDS, New DelayedWorkQueue, threadFactory, handler)
		End Sub

		''' <summary>
		''' Returns the trigger time of a delayed action.
		''' </summary>
		Private Function triggerTime(  delay As Long,   unit As TimeUnit) As Long
			Return triggerTime(unit.toNanos(If(delay < 0, 0, delay)))
		End Function

		''' <summary>
		''' Returns the trigger time of a delayed action.
		''' </summary>
		Friend Overridable Function triggerTime(  delay As Long) As Long
			Return now() + (If(delay < (Long.Max_Value >> 1), delay, overflowFree(delay)))
		End Function

		''' <summary>
		''' Constrains the values of all delays in the queue to be within
		''' java.lang.[Long].MAX_VALUE of each other, to avoid overflow in compareTo.
		''' This may occur if a task is eligible to be dequeued, but has
		''' not yet been, while some other task is added with a delay of
		''' java.lang.[Long].MAX_VALUE.
		''' </summary>
		Private Function overflowFree(  delay As Long) As Long
			Dim head As Delayed = CType(MyBase.queue.peek(), Delayed)
			If head IsNot Nothing Then
				Dim headDelay As Long = head.getDelay(NANOSECONDS)
				If headDelay < 0 AndAlso (delay - headDelay < 0) Then delay = java.lang.[Long].Max_Value + headDelay
			End If
			Return delay
		End Function

		''' <exception cref="RejectedExecutionException"> {@inheritDoc} </exception>
		''' <exception cref="NullPointerException">       {@inheritDoc} </exception>
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
		Public Overridable Function schedule(  command As Runnable,   delay As Long,   unit As TimeUnit) As ScheduledFuture(Of ?) Implements ScheduledExecutorService.schedule
			If command Is Nothing OrElse unit Is Nothing Then Throw New NullPointerException
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
			Dim t As RunnableScheduledFuture(Of ?) = decorateTask(command, New ScheduledFutureTask(Me, Of Void)(command, Nothing, triggerTime(delay, unit)))
			delayedExecute(t)
			Return t
		End Function

		''' <exception cref="RejectedExecutionException"> {@inheritDoc} </exception>
		''' <exception cref="NullPointerException">       {@inheritDoc} </exception>
		Public Overridable Function schedule(Of V)(  callable As Callable(Of V),   delay As Long,   unit As TimeUnit) As ScheduledFuture(Of V) Implements ScheduledExecutorService.schedule
			If callable Is Nothing OrElse unit Is Nothing Then Throw New NullPointerException
			Dim t As RunnableScheduledFuture(Of V) = decorateTask(callable, New ScheduledFutureTask(Me, Of V)(callable, triggerTime(delay, unit)))
			delayedExecute(t)
			Return t
		End Function

		''' <exception cref="RejectedExecutionException"> {@inheritDoc} </exception>
		''' <exception cref="NullPointerException">       {@inheritDoc} </exception>
		''' <exception cref="IllegalArgumentException">   {@inheritDoc} </exception>
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
		Public Overridable Function scheduleAtFixedRate(  command As Runnable,   initialDelay As Long,   period As Long,   unit As TimeUnit) As ScheduledFuture(Of ?) Implements ScheduledExecutorService.scheduleAtFixedRate
			If command Is Nothing OrElse unit Is Nothing Then Throw New NullPointerException
			If period <= 0 Then Throw New IllegalArgumentException
			Dim sft As New ScheduledFutureTask(Me, Of Void)(command, Nothing, triggerTime(initialDelay, unit), unit.toNanos(period))
			Dim t As RunnableScheduledFuture(Of Void) = decorateTask(command, sft)
			sft.outerTask = t
			delayedExecute(t)
			Return t
		End Function

		''' <exception cref="RejectedExecutionException"> {@inheritDoc} </exception>
		''' <exception cref="NullPointerException">       {@inheritDoc} </exception>
		''' <exception cref="IllegalArgumentException">   {@inheritDoc} </exception>
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
		Public Overridable Function scheduleWithFixedDelay(  command As Runnable,   initialDelay As Long,   delay As Long,   unit As TimeUnit) As ScheduledFuture(Of ?) Implements ScheduledExecutorService.scheduleWithFixedDelay
			If command Is Nothing OrElse unit Is Nothing Then Throw New NullPointerException
			If delay <= 0 Then Throw New IllegalArgumentException
			Dim sft As New ScheduledFutureTask(Me, Of Void)(command, Nothing, triggerTime(initialDelay, unit), unit.toNanos(-delay))
			Dim t As RunnableScheduledFuture(Of Void) = decorateTask(command, sft)
			sft.outerTask = t
			delayedExecute(t)
			Return t
		End Function

		''' <summary>
		''' Executes {@code command} with zero required delay.
		''' This has effect equivalent to
		''' <seealso cref="#schedule(Runnable,long,TimeUnit) schedule(command, 0, anyUnit)"/>.
		''' Note that inspections of the queue and of the list returned by
		''' {@code shutdownNow} will access the zero-delayed
		''' <seealso cref="ScheduledFuture"/>, not the {@code command} itself.
		''' 
		''' <p>A consequence of the use of {@code ScheduledFuture} objects is
		''' that <seealso cref="ThreadPoolExecutor#afterExecute afterExecute"/> is always
		''' called with a null second {@code Throwable} argument, even if the
		''' {@code command} terminated abruptly.  Instead, the {@code Throwable}
		''' thrown by such a task can be obtained via <seealso cref="Future#get"/>.
		''' </summary>
		''' <exception cref="RejectedExecutionException"> at discretion of
		'''         {@code RejectedExecutionHandler}, if the task
		'''         cannot be accepted for execution because the
		'''         executor has been shut down </exception>
		''' <exception cref="NullPointerException"> {@inheritDoc} </exception>
		Public Overrides Sub execute(  command As Runnable) Implements Executor.execute
			schedule(command, 0, NANOSECONDS)
		End Sub

		' Override AbstractExecutorService methods

		''' <exception cref="RejectedExecutionException"> {@inheritDoc} </exception>
		''' <exception cref="NullPointerException">       {@inheritDoc} </exception>
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
		Public Overrides Function submit(  task As Runnable) As Future(Of ?) Implements ExecutorService.submit
			Return schedule(task, 0, NANOSECONDS)
		End Function

		''' <exception cref="RejectedExecutionException"> {@inheritDoc} </exception>
		''' <exception cref="NullPointerException">       {@inheritDoc} </exception>
		Public Overrides Function submit(Of T)(  task As Runnable,   result As T) As Future(Of T) Implements ExecutorService.submit
			Return schedule(Executors.callable(task, result), 0, NANOSECONDS)
		End Function

		''' <exception cref="RejectedExecutionException"> {@inheritDoc} </exception>
		''' <exception cref="NullPointerException">       {@inheritDoc} </exception>
		Public Overrides Function submit(Of T)(  task As Callable(Of T)) As Future(Of T) Implements ExecutorService.submit
			Return schedule(task, 0, NANOSECONDS)
		End Function

		''' <summary>
		''' Sets the policy on whether to continue executing existing
		''' periodic tasks even when this executor has been {@code shutdown}.
		''' In this case, these tasks will only terminate upon
		''' {@code shutdownNow} or after setting the policy to
		''' {@code false} when already shutdown.
		''' This value is by default {@code false}.
		''' </summary>
		''' <param name="value"> if {@code true}, continue after shutdown, else don't </param>
		''' <seealso cref= #getContinueExistingPeriodicTasksAfterShutdownPolicy </seealso>
		Public Overridable Property continueExistingPeriodicTasksAfterShutdownPolicy As Boolean
			Set(  value As Boolean)
				continueExistingPeriodicTasksAfterShutdown = value
				If (Not value) AndAlso shutdown Then onShutdown()
			End Set
			Get
				Return continueExistingPeriodicTasksAfterShutdown
			End Get
		End Property


		''' <summary>
		''' Sets the policy on whether to execute existing delayed
		''' tasks even when this executor has been {@code shutdown}.
		''' In this case, these tasks will only terminate upon
		''' {@code shutdownNow}, or after setting the policy to
		''' {@code false} when already shutdown.
		''' This value is by default {@code true}.
		''' </summary>
		''' <param name="value"> if {@code true}, execute after shutdown, else don't </param>
		''' <seealso cref= #getExecuteExistingDelayedTasksAfterShutdownPolicy </seealso>
		Public Overridable Property executeExistingDelayedTasksAfterShutdownPolicy As Boolean
			Set(  value As Boolean)
				executeExistingDelayedTasksAfterShutdown = value
				If (Not value) AndAlso shutdown Then onShutdown()
			End Set
			Get
				Return executeExistingDelayedTasksAfterShutdown
			End Get
		End Property


		''' <summary>
		''' Sets the policy on whether cancelled tasks should be immediately
		''' removed from the work queue at time of cancellation.  This value is
		''' by default {@code false}.
		''' </summary>
		''' <param name="value"> if {@code true}, remove on cancellation, else don't </param>
		''' <seealso cref= #getRemoveOnCancelPolicy
		''' @since 1.7 </seealso>
		Public Overridable Property removeOnCancelPolicy As Boolean
			Set(  value As Boolean)
				removeOnCancel = value
			End Set
			Get
				Return removeOnCancel
			End Get
		End Property


		''' <summary>
		''' Initiates an orderly shutdown in which previously submitted
		''' tasks are executed, but no new tasks will be accepted.
		''' Invocation has no additional effect if already shut down.
		''' 
		''' <p>This method does not wait for previously submitted tasks to
		''' complete execution.  Use <seealso cref="#awaitTermination awaitTermination"/>
		''' to do that.
		''' 
		''' <p>If the {@code ExecuteExistingDelayedTasksAfterShutdownPolicy}
		''' has been set {@code false}, existing delayed tasks whose delays
		''' have not yet elapsed are cancelled.  And unless the {@code
		''' ContinueExistingPeriodicTasksAfterShutdownPolicy} has been set
		''' {@code true}, future executions of existing periodic tasks will
		''' be cancelled.
		''' </summary>
		''' <exception cref="SecurityException"> {@inheritDoc} </exception>
		Public Overrides Sub shutdown() Implements ExecutorService.shutdown
			MyBase.shutdown()
		End Sub

		''' <summary>
		''' Attempts to stop all actively executing tasks, halts the
		''' processing of waiting tasks, and returns a list of the tasks
		''' that were awaiting execution.
		''' 
		''' <p>This method does not wait for actively executing tasks to
		''' terminate.  Use <seealso cref="#awaitTermination awaitTermination"/> to
		''' do that.
		''' 
		''' <p>There are no guarantees beyond best-effort attempts to stop
		''' processing actively executing tasks.  This implementation
		''' cancels tasks via <seealso cref="Thread#interrupt"/>, so any task that
		''' fails to respond to interrupts may never terminate.
		''' </summary>
		''' <returns> list of tasks that never commenced execution.
		'''         Each element of this list is a <seealso cref="ScheduledFuture"/>,
		'''         including those tasks submitted using {@code execute},
		'''         which are for scheduling purposes used as the basis of a
		'''         zero-delay {@code ScheduledFuture}. </returns>
		''' <exception cref="SecurityException"> {@inheritDoc} </exception>
		Public Overrides Function shutdownNow() As List(Of Runnable)
			Return MyBase.shutdownNow()
		End Function

		''' <summary>
		''' Returns the task queue used by this executor.  Each element of
		''' this queue is a <seealso cref="ScheduledFuture"/>, including those
		''' tasks submitted using {@code execute} which are for scheduling
		''' purposes used as the basis of a zero-delay
		''' {@code ScheduledFuture}.  Iteration over this queue is
		''' <em>not</em> guaranteed to traverse tasks in the order in
		''' which they will execute.
		''' </summary>
		''' <returns> the task queue </returns>
		Public  Overrides ReadOnly Property  queue As BlockingQueue(Of Runnable)
			Get
				Return MyBase.queue
			End Get
		End Property

		''' <summary>
		''' Specialized delay queue. To mesh with TPE declarations, this
		''' class must be declared as a BlockingQueue<Runnable> even though
		''' it can only hold RunnableScheduledFutures.
		''' </summary>
		Friend Class DelayedWorkQueue
			Inherits AbstractQueue(Of Runnable)
			Implements BlockingQueue(Of Runnable)

	'        
	'         * A DelayedWorkQueue is based on a heap-based data structure
	'         * like those in DelayQueue and PriorityQueue, except that
	'         * every ScheduledFutureTask also records its index into the
	'         * heap array. This eliminates the need to find a task upon
	'         * cancellation, greatly speeding up removal (down from O(n)
	'         * to O(log n)), and reducing garbage retention that would
	'         * otherwise occur by waiting for the element to rise to top
	'         * before clearing. But because the queue may also hold
	'         * RunnableScheduledFutures that are not ScheduledFutureTasks,
	'         * we are not guaranteed to have such indices available, in
	'         * which case we fall back to linear search. (We expect that
	'         * most tasks will not be decorated, and that the faster cases
	'         * will be much more common.)
	'         *
	'         * All heap operations must record index changes -- mainly
	'         * within siftUp and siftDown. Upon removal, a task's
	'         * heapIndex is set to -1. Note that ScheduledFutureTasks can
	'         * appear at most once in the queue (this need not be true for
	'         * other kinds of tasks or work queues), so are uniquely
	'         * identified by heapIndex.
	'         

			Private Const INITIAL_CAPACITY As Integer = 16
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
			Private queue As RunnableScheduledFuture(Of ?)() = New RunnableScheduledFuture(Of ?)(INITIAL_CAPACITY - 1){}
			Private ReadOnly lock As New java.util.concurrent.locks.ReentrantLock
			Private size_Renamed As Integer = 0

			''' <summary>
			''' Thread designated to wait for the task at the head of the
			''' queue.  This variant of the Leader-Follower pattern
			''' (http://www.cs.wustl.edu/~schmidt/POSA/POSA2/) serves to
			''' minimize unnecessary timed waiting.  When a thread becomes
			''' the leader, it waits only for the next delay to elapse, but
			''' other threads await indefinitely.  The leader thread must
			''' signal some other thread before returning from take() or
			''' poll(...), unless some other thread becomes leader in the
			''' interim.  Whenever the head of the queue is replaced with a
			''' task with an earlier expiration time, the leader field is
			''' invalidated by being reset to null, and some waiting
			''' thread, but not necessarily the current leader, is
			''' signalled.  So waiting threads must be prepared to acquire
			''' and lose leadership while waiting.
			''' </summary>
			Private leader As Thread = Nothing

			''' <summary>
			''' Condition signalled when a newer task becomes available at the
			''' head of the queue or a new thread may need to become leader.
			''' </summary>
			Private ReadOnly available As java.util.concurrent.locks.Condition = lock.newCondition()

			''' <summary>
			''' Sets f's heapIndex if it is a ScheduledFutureTask.
			''' </summary>
			Private Sub setIndex(Of T1)(  f As RunnableScheduledFuture(Of T1),   idx As Integer)
				If TypeOf f Is ScheduledFutureTask Then CType(f, ScheduledFutureTask).heapIndex = idx
			End Sub

			''' <summary>
			''' Sifts element added at bottom up to its heap-ordered spot.
			''' Call only when holding lock.
			''' </summary>
			Private Sub siftUp(Of T1)(  k As Integer,   key As RunnableScheduledFuture(Of T1))
				Do While k > 0
					Dim parent As Integer = CInt(CUInt((k - 1)) >> 1)
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
					Dim e As RunnableScheduledFuture(Of ?) = queue(parent)
					If key.CompareTo(e) >= 0 Then Exit Do
					queue(k) = e
					indexdex(e, k)
					k = parent
				Loop
				queue(k) = key
				indexdex(key, k)
			End Sub

			''' <summary>
			''' Sifts element added at top down to its heap-ordered spot.
			''' Call only when holding lock.
			''' </summary>
			Private Sub siftDown(Of T1)(  k As Integer,   key As RunnableScheduledFuture(Of T1))
				Dim half As Integer = CInt(CUInt(size_Renamed) >> 1)
				Do While k < half
					Dim child As Integer = (k << 1) + 1
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
					Dim c As RunnableScheduledFuture(Of ?) = queue(child)
					Dim right As Integer = child + 1
					If right < size_Renamed AndAlso c.CompareTo(queue(right)) > 0 Then c = queue(child = right)
					If key.CompareTo(c) <= 0 Then Exit Do
					queue(k) = c
					indexdex(c, k)
					k = child
				Loop
				queue(k) = key
				indexdex(key, k)
			End Sub

			''' <summary>
			''' Resizes the heap array.  Call only when holding lock.
			''' </summary>
			Private Sub grow()
				Dim oldCapacity As Integer = queue.Length
				Dim newCapacity As Integer = oldCapacity + (oldCapacity >> 1) ' grow 50%
				If newCapacity < 0 Then ' overflow newCapacity =  java.lang.[Integer].Max_Value
				queue = New java.util.concurrent.RunnableScheduledFuture(Of JavaToDotNetGenericWildcard)(newCapacity - 1){}
				Array.Copy(queue, queue, newCapacity)
			End Sub

			''' <summary>
			''' Finds index of given object, or -1 if absent.
			''' </summary>
			Private Function indexOf(  x As Object) As Integer
				If x IsNot Nothing Then
					If TypeOf x Is ScheduledFutureTask Then
						Dim i As Integer = CType(x, ScheduledFutureTask).heapIndex
						' Sanity check; x could conceivably be a
						' ScheduledFutureTask from some other pool.
						If i >= 0 AndAlso i < size_Renamed AndAlso queue(i) Is x Then Return i
					Else
						For i As Integer = 0 To size_Renamed - 1
							If x.Equals(queue(i)) Then Return i
						Next i
					End If
				End If
				Return -1
			End Function

			Public Overridable Function contains(  x As Object) As Boolean Implements BlockingQueue(Of Runnable).contains
				Dim lock As java.util.concurrent.locks.ReentrantLock = Me.lock
				lock.lock()
				Try
					Return IndexOf(x) <> -1
				Finally
					lock.unlock()
				End Try
			End Function

			Public Overridable Function remove(  x As Object) As Boolean Implements BlockingQueue(Of Runnable).remove
				Dim lock As java.util.concurrent.locks.ReentrantLock = Me.lock
				lock.lock()
				Try
					Dim i As Integer = IndexOf(x)
					If i < 0 Then Return False

					indexdex(queue(i), -1)
					size_Renamed -= 1
					Dim s As Integer = size_Renamed
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
					Dim replacement As RunnableScheduledFuture(Of ?) = queue(s)
					queue(s) = Nothing
					If s <> i Then
						siftDown(i, replacement)
						If queue(i) Is replacement Then siftUp(i, replacement)
					End If
					Return True
				Finally
					lock.unlock()
				End Try
			End Function

			Public Overridable Function size() As Integer
				Dim lock As java.util.concurrent.locks.ReentrantLock = Me.lock
				lock.lock()
				Try
					Return size_Renamed
				Finally
					lock.unlock()
				End Try
			End Function

			Public Overridable Property empty As Boolean
				Get
					Return size() = 0
				End Get
			End Property

			Public Overridable Function remainingCapacity() As Integer Implements BlockingQueue(Of Runnable).remainingCapacity
				Return  java.lang.[Integer].Max_Value
			End Function

'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
			Public Overridable Function peek() As RunnableScheduledFuture(Of ?)
				Dim lock As java.util.concurrent.locks.ReentrantLock = Me.lock
				lock.lock()
				Try
					Return queue(0)
				Finally
					lock.unlock()
				End Try
			End Function

			Public Overridable Function offer(  x As Runnable) As Boolean
				If x Is Nothing Then Throw New NullPointerException
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
				Dim e As RunnableScheduledFuture(Of ?) = CType(x, RunnableScheduledFuture(Of ?))
				Dim lock As java.util.concurrent.locks.ReentrantLock = Me.lock
				lock.lock()
				Try
					Dim i As Integer = size_Renamed
					If i >= queue.Length Then grow()
					size_Renamed = i + 1
					If i = 0 Then
						queue(0) = e
						indexdex(e, 0)
					Else
						siftUp(i, e)
					End If
					If queue(0) Is e Then
						leader = Nothing
						available.signal()
					End If
				Finally
					lock.unlock()
				End Try
				Return True
			End Function

			Public Overridable Sub put(  e As Runnable)
				offer(e)
			End Sub

			Public Overridable Function add(  e As Runnable) As Boolean
				Return offer(e)
			End Function

			Public Overridable Function offer(  e As Runnable,   timeout As Long,   unit As TimeUnit) As Boolean
				Return offer(e)
			End Function

			''' <summary>
			''' Performs common bookkeeping for poll and take: Replaces
			''' first element with last and sifts it down.  Call only when
			''' holding lock. </summary>
			''' <param name="f"> the task to remove and return </param>
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
			Private Function finishPoll(Of T1)(  f As RunnableScheduledFuture(Of T1)) As RunnableScheduledFuture(Of ?)
				size_Renamed -= 1
				Dim s As Integer = size_Renamed
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
				Dim x As RunnableScheduledFuture(Of ?) = queue(s)
				queue(s) = Nothing
				If s <> 0 Then siftDown(0, x)
				indexdex(f, -1)
				Return f
			End Function

'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
			Public Overridable Function poll() As RunnableScheduledFuture(Of ?)
				Dim lock As java.util.concurrent.locks.ReentrantLock = Me.lock
				lock.lock()
				Try
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
					Dim first As RunnableScheduledFuture(Of ?) = queue(0)
					If first Is Nothing OrElse first.getDelay(NANOSECONDS) > 0 Then
						Return Nothing
					Else
						Return finishPoll(first)
					End If
				Finally
					lock.unlock()
				End Try
			End Function

'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
			Public Overridable Function take() As RunnableScheduledFuture(Of ?)
				Dim lock As java.util.concurrent.locks.ReentrantLock = Me.lock
				lock.lockInterruptibly()
				Try
					Do
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
						Dim first As RunnableScheduledFuture(Of ?) = queue(0)
						If first Is Nothing Then
							available.await()
						Else
							Dim delay As Long = first.getDelay(NANOSECONDS)
							If delay <= 0 Then Return finishPoll(first)
							first = Nothing ' don't retain ref while waiting
							If leader IsNot Nothing Then
								available.await()
							Else
								Dim thisThread As Thread = Thread.CurrentThread
								leader = thisThread
								Try
									available.awaitNanos(delay)
								Finally
									If leader Is thisThread Then leader = Nothing
								End Try
							End If
						End If
					Loop
				Finally
					If leader Is Nothing AndAlso queue(0) IsNot Nothing Then available.signal()
					lock.unlock()
				End Try
			End Function

'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
			Public Overridable Function poll(  timeout As Long,   unit As TimeUnit) As RunnableScheduledFuture(Of ?)
				Dim nanos As Long = unit.toNanos(timeout)
				Dim lock As java.util.concurrent.locks.ReentrantLock = Me.lock
				lock.lockInterruptibly()
				Try
					Do
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
						Dim first As RunnableScheduledFuture(Of ?) = queue(0)
						If first Is Nothing Then
							If nanos <= 0 Then
								Return Nothing
							Else
								nanos = available.awaitNanos(nanos)
							End If
						Else
							Dim delay As Long = first.getDelay(NANOSECONDS)
							If delay <= 0 Then Return finishPoll(first)
							If nanos <= 0 Then Return Nothing
							first = Nothing ' don't retain ref while waiting
							If nanos < delay OrElse leader IsNot Nothing Then
								nanos = available.awaitNanos(nanos)
							Else
								Dim thisThread As Thread = Thread.CurrentThread
								leader = thisThread
								Try
									Dim timeLeft As Long = available.awaitNanos(delay)
									nanos -= delay - timeLeft
								Finally
									If leader Is thisThread Then leader = Nothing
								End Try
							End If
						End If
					Loop
				Finally
					If leader Is Nothing AndAlso queue(0) IsNot Nothing Then available.signal()
					lock.unlock()
				End Try
			End Function

			Public Overridable Sub clear()
				Dim lock As java.util.concurrent.locks.ReentrantLock = Me.lock
				lock.lock()
				Try
					For i As Integer = 0 To size_Renamed - 1
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
						Dim t As RunnableScheduledFuture(Of ?) = queue(i)
						If t IsNot Nothing Then
							queue(i) = Nothing
							indexdex(t, -1)
						End If
					Next i
					size_Renamed = 0
				Finally
					lock.unlock()
				End Try
			End Sub

			''' <summary>
			''' Returns first element only if it is expired.
			''' Used only by drainTo.  Call only when holding lock.
			''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
			Private Function peekExpired() As RunnableScheduledFuture(Of ?)
				' assert lock.isHeldByCurrentThread();
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
				Dim first As RunnableScheduledFuture(Of ?) = queue(0)
				Return If(first Is Nothing OrElse first.getDelay(NANOSECONDS) > 0, Nothing, first)
			End Function

'JAVA TO VB CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
			Public Overridable Function drainTo(Of T1)(  c As Collection(Of T1)) As Integer
				If c Is Nothing Then Throw New NullPointerException
				If c Is Me Then Throw New IllegalArgumentException
				Dim lock As java.util.concurrent.locks.ReentrantLock = Me.lock
				lock.lock()
				Try
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
					Dim first As RunnableScheduledFuture(Of ?)
					Dim n As Integer = 0
					first = peekExpired()
					Do While first IsNot Nothing
						c.add(first) ' In this order, in case add() throws.
						finishPoll(first)
						n += 1
						first = peekExpired()
					Loop
					Return n
				Finally
					lock.unlock()
				End Try
			End Function

'JAVA TO VB CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
			Public Overridable Function drainTo(Of T1)(  c As Collection(Of T1),   maxElements As Integer) As Integer
				If c Is Nothing Then Throw New NullPointerException
				If c Is Me Then Throw New IllegalArgumentException
				If maxElements <= 0 Then Return 0
				Dim lock As java.util.concurrent.locks.ReentrantLock = Me.lock
				lock.lock()
				Try
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
					Dim first As RunnableScheduledFuture(Of ?)
					Dim n As Integer = 0
					first = peekExpired()
					Do While n < maxElements AndAlso first IsNot Nothing
						c.add(first) ' In this order, in case add() throws.
						finishPoll(first)
						n += 1
						first = peekExpired()
					Loop
					Return n
				Finally
					lock.unlock()
				End Try
			End Function

			Public Overridable Function toArray() As Object()
				Dim lock As java.util.concurrent.locks.ReentrantLock = Me.lock
				lock.lock()
				Try
					Return Arrays.copyOf(queue, size_Renamed, GetType(Object()))
				Finally
					lock.unlock()
				End Try
			End Function

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
			Public Overridable Function toArray(Of T)(  a As T()) As T()
				Dim lock As java.util.concurrent.locks.ReentrantLock = Me.lock
				lock.lock()
				Try
					If a.Length < size_Renamed Then Return CType(Arrays.copyOf(queue, size_Renamed, a.GetType()), T())
					Array.Copy(queue, 0, a, 0, size_Renamed)
					If a.Length > size_Renamed Then a(size_Renamed) = Nothing
					Return a
				Finally
					lock.unlock()
				End Try
			End Function

			Public Overridable Function [iterator]() As [Iterator](Of Runnable)
				Return New Itr(Me, Arrays.copyOf(queue, size_Renamed))
			End Function

			''' <summary>
			''' Snapshot iterator that works off copy of underlying q array.
			''' </summary>
			Private Class Itr
				Implements Iterator(Of Runnable)

				Private ReadOnly outerInstance As ScheduledThreadPoolExecutor.DelayedWorkQueue

'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
				Friend ReadOnly array As RunnableScheduledFuture(Of ?)()
				Friend cursor As Integer = 0 ' index of next element to return
				Friend lastRet As Integer = -1 ' index of last element, or -1 if no such

				Friend Sub New(  outerInstance As ScheduledThreadPoolExecutor.DelayedWorkQueue, Of T1)(  array As RunnableScheduledFuture(Of T1)())
						Me.outerInstance = outerInstance
					Me.array = array
				End Sub

				Public Overridable Function hasNext() As Boolean Implements Iterator(Of Runnable).hasNext
					Return cursor < array.Length
				End Function

				Public Overridable Function [next]() As Runnable
					If cursor >= array.Length Then Throw New NoSuchElementException
					lastRet = cursor
						Dim tempVar As Integer = cursor
						cursor += 1
						Return array(tempVar)
				End Function

				Public Overridable Sub remove() Implements Iterator(Of Runnable).remove
					If lastRet < 0 Then Throw New IllegalStateException
					outerInstance.remove(array(lastRet))
					lastRet = -1
				End Sub
			End Class
		End Class
	End Class

End Namespace