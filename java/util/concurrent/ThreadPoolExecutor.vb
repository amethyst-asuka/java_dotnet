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
	''' An <seealso cref="ExecutorService"/> that executes each submitted task using
	''' one of possibly several pooled threads, normally configured
	''' using <seealso cref="Executors"/> factory methods.
	''' 
	''' <p>Thread pools address two different problems: they usually
	''' provide improved performance when executing large numbers of
	''' asynchronous tasks, due to reduced per-task invocation overhead,
	''' and they provide a means of bounding and managing the resources,
	''' including threads, consumed when executing a collection of tasks.
	''' Each {@code ThreadPoolExecutor} also maintains some basic
	''' statistics, such as the number of completed tasks.
	''' 
	''' <p>To be useful across a wide range of contexts, this class
	''' provides many adjustable parameters and extensibility
	''' hooks. However, programmers are urged to use the more convenient
	''' <seealso cref="Executors"/> factory methods {@link
	''' Executors#newCachedThreadPool} (unbounded thread pool, with
	''' automatic thread reclamation), <seealso cref="Executors#newFixedThreadPool"/>
	''' (fixed size thread pool) and {@link
	''' Executors#newSingleThreadExecutor} (single background thread), that
	''' preconfigure settings for the most common usage
	''' scenarios. Otherwise, use the following guide when manually
	''' configuring and tuning this class:
	''' 
	''' <dl>
	''' 
	''' <dt>Core and maximum pool sizes</dt>
	''' 
	''' <dd>A {@code ThreadPoolExecutor} will automatically adjust the
	''' pool size (see <seealso cref="#getPoolSize"/>)
	''' according to the bounds set by
	''' corePoolSize (see <seealso cref="#getCorePoolSize"/>) and
	''' maximumPoolSize (see <seealso cref="#getMaximumPoolSize"/>).
	''' 
	''' When a new task is submitted in method <seealso cref="#execute(Runnable)"/>,
	''' and fewer than corePoolSize threads are running, a new thread is
	''' created to handle the request, even if other worker threads are
	''' idle.  If there are more than corePoolSize but less than
	''' maximumPoolSize threads running, a new thread will be created only
	''' if the queue is full.  By setting corePoolSize and maximumPoolSize
	''' the same, you create a fixed-size thread pool. By setting
	''' maximumPoolSize to an essentially unbounded value such as {@code
	'''  java.lang.[Integer].MAX_VALUE}, you allow the pool to accommodate an arbitrary
	''' number of concurrent tasks. Most typically, core and maximum pool
	''' sizes are set only upon construction, but they may also be changed
	''' dynamically using <seealso cref="#setCorePoolSize"/> and {@link
	''' #setMaximumPoolSize}. </dd>
	''' 
	''' <dt>On-demand construction</dt>
	''' 
	''' <dd>By default, even core threads are initially created and
	''' started only when new tasks arrive, but this can be overridden
	''' dynamically using method <seealso cref="#prestartCoreThread"/> or {@link
	''' #prestartAllCoreThreads}.  You probably want to prestart threads if
	''' you construct the pool with a non-empty queue. </dd>
	''' 
	''' <dt>Creating new threads</dt>
	''' 
	''' <dd>New threads are created using a <seealso cref="ThreadFactory"/>.  If not
	''' otherwise specified, a <seealso cref="Executors#defaultThreadFactory"/> is
	''' used, that creates threads to all be in the same {@link
	''' ThreadGroup} and with the same {@code NORM_PRIORITY} priority and
	''' non-daemon status. By supplying a different ThreadFactory, you can
	''' alter the thread's name, thread group, priority, daemon status,
	''' etc. If a {@code ThreadFactory} fails to create a thread when asked
	''' by returning null from {@code newThread}, the executor will
	''' continue, but might not be able to execute any tasks. Threads
	''' should possess the "modifyThread" {@code RuntimePermission}. If
	''' worker threads or other threads using the pool do not possess this
	''' permission, service may be degraded: configuration changes may not
	''' take effect in a timely manner, and a shutdown pool may remain in a
	''' state in which termination is possible but not completed.</dd>
	''' 
	''' <dt>Keep-alive times</dt>
	''' 
	''' <dd>If the pool currently has more than corePoolSize threads,
	''' excess threads will be terminated if they have been idle for more
	''' than the keepAliveTime (see <seealso cref="#getKeepAliveTime(TimeUnit)"/>).
	''' This provides a means of reducing resource consumption when the
	''' pool is not being actively used. If the pool becomes more active
	''' later, new threads will be constructed. This parameter can also be
	''' changed dynamically using method {@link #setKeepAliveTime(long,
	''' TimeUnit)}.  Using a value of {@code java.lang.[Long].MAX_VALUE} {@link
	''' TimeUnit#NANOSECONDS} effectively disables idle threads from ever
	''' terminating prior to shut down. By default, the keep-alive policy
	''' applies only when there are more than corePoolSize threads. But
	''' method <seealso cref="#allowCoreThreadTimeOut(boolean)"/> can be used to
	''' apply this time-out policy to core threads as well, so long as the
	''' keepAliveTime value is non-zero. </dd>
	''' 
	''' <dt>Queuing</dt>
	''' 
	''' <dd>Any <seealso cref="BlockingQueue"/> may be used to transfer and hold
	''' submitted tasks.  The use of this queue interacts with pool sizing:
	''' 
	''' <ul>
	''' 
	''' <li> If fewer than corePoolSize threads are running, the Executor
	''' always prefers adding a new thread
	''' rather than queuing.</li>
	''' 
	''' <li> If corePoolSize or more threads are running, the Executor
	''' always prefers queuing a request rather than adding a new
	''' thread.</li>
	''' 
	''' <li> If a request cannot be queued, a new thread is created unless
	''' this would exceed maximumPoolSize, in which case, the task will be
	''' rejected.</li>
	''' 
	''' </ul>
	''' 
	''' There are three general strategies for queuing:
	''' <ol>
	''' 
	''' <li> <em> Direct handoffs.</em> A good default choice for a work
	''' queue is a <seealso cref="SynchronousQueue"/> that hands off tasks to threads
	''' without otherwise holding them. Here, an attempt to queue a task
	''' will fail if no threads are immediately available to run it, so a
	''' new thread will be constructed. This policy avoids lockups when
	''' handling sets of requests that might have internal dependencies.
	''' Direct handoffs generally require unbounded maximumPoolSizes to
	''' avoid rejection of new submitted tasks. This in turn admits the
	''' possibility of unbounded thread growth when commands continue to
	''' arrive on average faster than they can be processed.  </li>
	''' 
	''' <li><em> Unbounded queues.</em> Using an unbounded queue (for
	''' example a <seealso cref="LinkedBlockingQueue"/> without a predefined
	''' capacity) will cause new tasks to wait in the queue when all
	''' corePoolSize threads are busy. Thus, no more than corePoolSize
	''' threads will ever be created. (And the value of the maximumPoolSize
	''' therefore doesn't have any effect.)  This may be appropriate when
	''' each task is completely independent of others, so tasks cannot
	''' affect each others execution; for example, in a web page server.
	''' While this style of queuing can be useful in smoothing out
	''' transient bursts of requests, it admits the possibility of
	''' unbounded work queue growth when commands continue to arrive on
	''' average faster than they can be processed.  </li>
	''' 
	''' <li><em>Bounded queues.</em> A bounded queue (for example, an
	''' <seealso cref="ArrayBlockingQueue"/>) helps prevent resource exhaustion when
	''' used with finite maximumPoolSizes, but can be more difficult to
	''' tune and control.  Queue sizes and maximum pool sizes may be traded
	''' off for each other: Using large queues and small pools minimizes
	''' CPU usage, OS resources, and context-switching overhead, but can
	''' lead to artificially low throughput.  If tasks frequently block (for
	''' example if they are I/O bound), a system may be able to schedule
	''' time for more threads than you otherwise allow. Use of small queues
	''' generally requires larger pool sizes, which keeps CPUs busier but
	''' may encounter unacceptable scheduling overhead, which also
	''' decreases throughput.  </li>
	''' 
	''' </ol>
	''' 
	''' </dd>
	''' 
	''' <dt>Rejected tasks</dt>
	''' 
	''' <dd>New tasks submitted in method <seealso cref="#execute(Runnable)"/> will be
	''' <em>rejected</em> when the Executor has been shut down, and also when
	''' the Executor uses finite bounds for both maximum threads and work queue
	''' capacity, and is saturated.  In either case, the {@code execute} method
	''' invokes the {@link
	''' RejectedExecutionHandler#rejectedExecution(Runnable, ThreadPoolExecutor)}
	''' method of its <seealso cref="RejectedExecutionHandler"/>.  Four predefined handler
	''' policies are provided:
	''' 
	''' <ol>
	''' 
	''' <li> In the default <seealso cref="ThreadPoolExecutor.AbortPolicy"/>, the
	''' handler throws a runtime <seealso cref="RejectedExecutionException"/> upon
	''' rejection. </li>
	''' 
	''' <li> In <seealso cref="ThreadPoolExecutor.CallerRunsPolicy"/>, the thread
	''' that invokes {@code execute} itself runs the task. This provides a
	''' simple feedback control mechanism that will slow down the rate that
	''' new tasks are submitted. </li>
	''' 
	''' <li> In <seealso cref="ThreadPoolExecutor.DiscardPolicy"/>, a task that
	''' cannot be executed is simply dropped.  </li>
	''' 
	''' <li>In <seealso cref="ThreadPoolExecutor.DiscardOldestPolicy"/>, if the
	''' executor is not shut down, the task at the head of the work queue
	''' is dropped, and then execution is retried (which can fail again,
	''' causing this to be repeated.) </li>
	''' 
	''' </ol>
	''' 
	''' It is possible to define and use other kinds of {@link
	''' RejectedExecutionHandler} classes. Doing so requires some care
	''' especially when policies are designed to work only under particular
	''' capacity or queuing policies. </dd>
	''' 
	''' <dt>Hook methods</dt>
	''' 
	''' <dd>This class provides {@code protected} overridable
	''' <seealso cref="#beforeExecute(Thread, Runnable)"/> and
	''' <seealso cref="#afterExecute(Runnable, Throwable)"/> methods that are called
	''' before and after execution of each task.  These can be used to
	''' manipulate the execution environment; for example, reinitializing
	''' ThreadLocals, gathering statistics, or adding log entries.
	''' Additionally, method <seealso cref="#terminated"/> can be overridden to perform
	''' any special processing that needs to be done once the Executor has
	''' fully terminated.
	''' 
	''' <p>If hook or callback methods throw exceptions, internal worker
	''' threads may in turn fail and abruptly terminate.</dd>
	''' 
	''' <dt>Queue maintenance</dt>
	''' 
	''' <dd>Method <seealso cref="#getQueue()"/> allows access to the work queue
	''' for purposes of monitoring and debugging.  Use of this method for
	''' any other purpose is strongly discouraged.  Two supplied methods,
	''' <seealso cref="#remove(Runnable)"/> and <seealso cref="#purge"/> are available to
	''' assist in storage reclamation when large numbers of queued tasks
	''' become cancelled.</dd>
	''' 
	''' <dt>Finalization</dt>
	''' 
	''' <dd>A pool that is no longer referenced in a program <em>AND</em>
	''' has no remaining threads will be {@code shutdown} automatically. If
	''' you would like to ensure that unreferenced pools are reclaimed even
	''' if users forget to call <seealso cref="#shutdown"/>, then you must arrange
	''' that unused threads eventually die, by setting appropriate
	''' keep-alive times, using a lower bound of zero core threads and/or
	''' setting <seealso cref="#allowCoreThreadTimeOut(boolean)"/>.  </dd>
	''' 
	''' </dl>
	''' 
	''' <p><b>Extension example</b>. Most extensions of this class
	''' override one or more of the protected hook methods. For example,
	''' here is a subclass that adds a simple pause/resume feature:
	''' 
	'''  <pre> {@code
	''' class PausableThreadPoolExecutor extends ThreadPoolExecutor {
	'''   private boolean isPaused;
	'''   private ReentrantLock pauseLock = new ReentrantLock();
	'''   private Condition unpaused = pauseLock.newCondition();
	''' 
	'''   public PausableThreadPoolExecutor(...) { super(...); }
	''' 
	'''   protected void beforeExecute(Thread t, Runnable r) {
	'''     super.beforeExecute(t, r);
	'''     pauseLock.lock();
	'''     try {
	'''       while (isPaused) unpaused.await();
	'''     } catch (InterruptedException ie) {
	'''       t.interrupt();
	'''     } finally {
	'''       pauseLock.unlock();
	'''     }
	'''   }
	''' 
	'''   public void pause() {
	'''     pauseLock.lock();
	'''     try {
	'''       isPaused = true;
	'''     } finally {
	'''       pauseLock.unlock();
	'''     }
	'''   }
	''' 
	'''   public void resume() {
	'''     pauseLock.lock();
	'''     try {
	'''       isPaused = false;
	'''       unpaused.signalAll();
	'''     } finally {
	'''       pauseLock.unlock();
	'''     }
	'''   }
	''' }}</pre>
	''' 
	''' @since 1.5
	''' @author Doug Lea
	''' </summary>
	Public Class ThreadPoolExecutor
		Inherits AbstractExecutorService

		''' <summary>
		''' The main pool control state, ctl, is an atomic integer packing
		''' two conceptual fields
		'''   workerCount, indicating the effective number of threads
		'''   runState,    indicating whether running, shutting down etc
		''' 
		''' In order to pack them into one int, we limit workerCount to
		''' (2^29)-1 (about 500 million) threads rather than (2^31)-1 (2
		''' billion) otherwise representable. If this is ever an issue in
		''' the future, the variable can be changed to be an AtomicLong,
		''' and the shift/mask constants below adjusted. But until the need
		''' arises, this code is a bit faster and simpler using an int.
		''' 
		''' The workerCount is the number of workers that have been
		''' permitted to start and not permitted to stop.  The value may be
		''' transiently different from the actual number of live threads,
		''' for example when a ThreadFactory fails to create a thread when
		''' asked, and when exiting threads are still performing
		''' bookkeeping before terminating. The user-visible pool size is
		''' reported as the current size of the workers set.
		''' 
		''' The runState provides the main lifecycle control, taking on values:
		''' 
		'''   RUNNING:  Accept new tasks and process queued tasks
		'''   SHUTDOWN: Don't accept new tasks, but process queued tasks
		'''   STOP:     Don't accept new tasks, don't process queued tasks,
		'''             and interrupt in-progress tasks
		'''   TIDYING:  All tasks have terminated, workerCount is zero,
		'''             the thread transitioning to state TIDYING
		'''             will run the terminated() hook method
		'''   TERMINATED: terminated() has completed
		''' 
		''' The numerical order among these values matters, to allow
		''' ordered comparisons. The runState monotonically increases over
		''' time, but need not hit each state. The transitions are:
		''' 
		''' RUNNING -> SHUTDOWN
		'''    On invocation of shutdown(), perhaps implicitly in finalize()
		''' (RUNNING or SHUTDOWN) -> STOP
		'''    On invocation of shutdownNow()
		''' SHUTDOWN -> TIDYING
		'''    When both queue and pool are empty
		''' STOP -> TIDYING
		'''    When pool is empty
		''' TIDYING -> TERMINATED
		'''    When the terminated() hook method has completed
		''' 
		''' Threads waiting in awaitTermination() will return when the
		''' state reaches TERMINATED.
		''' 
		''' Detecting the transition from SHUTDOWN to TIDYING is less
		''' straightforward than you'd like because the queue may become
		''' empty after non-empty and vice versa during SHUTDOWN state, but
		''' we can only terminate if, after seeing that it is empty, we see
		''' that workerCount is 0 (which sometimes entails a recheck -- see
		''' below).
		''' </summary>
		Private ReadOnly ctl As New java.util.concurrent.atomic.AtomicInteger(ctlOf(RUNNING, 0))
		Private Shared ReadOnly COUNT_BITS As Integer =  java.lang.[Integer].SIZE - 3
		Private Shared ReadOnly CAPACITY As Integer = (1 << COUNT_BITS) - 1

		' runState is stored in the high-order bits
		Private Shared ReadOnly RUNNING As Integer = -1 << COUNT_BITS
		Private Shared ReadOnly SHUTDOWN_Renamed As Integer = 0 << COUNT_BITS
		Private Shared ReadOnly [STOP] As Integer = 1 << COUNT_BITS
		Private Shared ReadOnly TIDYING As Integer = 2 << COUNT_BITS
		Private Shared ReadOnly TERMINATED_Renamed As Integer = 3 << COUNT_BITS

		' Packing and unpacking ctl
		Private Shared Function runStateOf(ByVal c As Integer) As Integer
			Return c And Not CAPACITY
		End Function
		Private Shared Function workerCountOf(ByVal c As Integer) As Integer
			Return c And CAPACITY
		End Function
		Private Shared Function ctlOf(ByVal rs As Integer, ByVal wc As Integer) As Integer
			Return rs Or wc
		End Function

	'    
	'     * Bit field accessors that don't require unpacking ctl.
	'     * These depend on the bit layout and on workerCount being never negative.
	'     

		Private Shared Function runStateLessThan(ByVal c As Integer, ByVal s As Integer) As Boolean
			Return c < s
		End Function

		Private Shared Function runStateAtLeast(ByVal c As Integer, ByVal s As Integer) As Boolean
			Return c >= s
		End Function

		Private Shared Function isRunning(ByVal c As Integer) As Boolean
			Return c < SHUTDOWN_Renamed
		End Function

		''' <summary>
		''' Attempts to CAS-increment the workerCount field of ctl.
		''' </summary>
		Private Function compareAndIncrementWorkerCount(ByVal expect As Integer) As Boolean
			Return ctl.compareAndSet(expect, expect + 1)
		End Function

		''' <summary>
		''' Attempts to CAS-decrement the workerCount field of ctl.
		''' </summary>
		Private Function compareAndDecrementWorkerCount(ByVal expect As Integer) As Boolean
			Return ctl.compareAndSet(expect, expect - 1)
		End Function

		''' <summary>
		''' Decrements the workerCount field of ctl. This is called only on
		''' abrupt termination of a thread (see processWorkerExit). Other
		''' decrements are performed within getTask.
		''' </summary>
		Private Sub decrementWorkerCount()
			Do
			Loop While Not compareAndDecrementWorkerCount(ctl.get())
		End Sub

		''' <summary>
		''' The queue used for holding tasks and handing off to worker
		''' threads.  We do not require that workQueue.poll() returning
		''' null necessarily means that workQueue.isEmpty(), so rely
		''' solely on isEmpty to see if the queue is empty (which we must
		''' do for example when deciding whether to transition from
		''' SHUTDOWN to TIDYING).  This accommodates special-purpose
		''' queues such as DelayQueues for which poll() is allowed to
		''' return null even if it may later return non-null when delays
		''' expire.
		''' </summary>
		Private ReadOnly workQueue As BlockingQueue(Of Runnable)

		''' <summary>
		''' Lock held on access to workers set and related bookkeeping.
		''' While we could use a concurrent set of some sort, it turns out
		''' to be generally preferable to use a lock. Among the reasons is
		''' that this serializes interruptIdleWorkers, which avoids
		''' unnecessary interrupt storms, especially during shutdown.
		''' Otherwise exiting threads would concurrently interrupt those
		''' that have not yet interrupted. It also simplifies some of the
		''' associated statistics bookkeeping of largestPoolSize etc. We
		''' also hold mainLock on shutdown and shutdownNow, for the sake of
		''' ensuring workers set is stable while separately checking
		''' permission to interrupt and actually interrupting.
		''' </summary>
		Private ReadOnly mainLock As New java.util.concurrent.locks.ReentrantLock

		''' <summary>
		''' Set containing all worker threads in pool. Accessed only when
		''' holding mainLock.
		''' </summary>
		Private ReadOnly workers As New HashSet(Of Worker)

		''' <summary>
		''' Wait condition to support awaitTermination
		''' </summary>
		Private ReadOnly termination As java.util.concurrent.locks.Condition = mainLock.newCondition()

		''' <summary>
		''' Tracks largest attained pool size. Accessed only under
		''' mainLock.
		''' </summary>
		Private largestPoolSize As Integer

		''' <summary>
		''' Counter for completed tasks. Updated only on termination of
		''' worker threads. Accessed only under mainLock.
		''' </summary>
		Private completedTaskCount As Long

	'    
	'     * All user control parameters are declared as volatiles so that
	'     * ongoing actions are based on freshest values, but without need
	'     * for locking, since no internal invariants depend on them
	'     * changing synchronously with respect to other actions.
	'     

		''' <summary>
		''' Factory for new threads. All threads are created using this
		''' factory (via method addWorker).  All callers must be prepared
		''' for addWorker to fail, which may reflect a system or user's
		''' policy limiting the number of threads.  Even though it is not
		''' treated as an error, failure to create threads may result in
		''' new tasks being rejected or existing ones remaining stuck in
		''' the queue.
		''' 
		''' We go further and preserve pool invariants even in the face of
		''' errors such as OutOfMemoryError, that might be thrown while
		''' trying to create threads.  Such errors are rather common due to
		''' the need to allocate a native stack in Thread.start, and users
		''' will want to perform clean pool shutdown to clean up.  There
		''' will likely be enough memory available for the cleanup code to
		''' complete without encountering yet another OutOfMemoryError.
		''' </summary>
'JAVA TO VB CONVERTER TODO TASK: There is no VB equivalent to 'volatile':
		Private threadFactory As ThreadFactory

		''' <summary>
		''' Handler called when saturated or shutdown in execute.
		''' </summary>
'JAVA TO VB CONVERTER TODO TASK: There is no VB equivalent to 'volatile':
		Private handler As RejectedExecutionHandler

		''' <summary>
		''' Timeout in nanoseconds for idle threads waiting for work.
		''' Threads use this timeout when there are more than corePoolSize
		''' present or if allowCoreThreadTimeOut. Otherwise they wait
		''' forever for new work.
		''' </summary>
'JAVA TO VB CONVERTER TODO TASK: There is no VB equivalent to 'volatile':
		Private keepAliveTime As Long

		''' <summary>
		''' If false (default), core threads stay alive even when idle.
		''' If true, core threads use keepAliveTime to time out waiting
		''' for work.
		''' </summary>
'JAVA TO VB CONVERTER TODO TASK: There is no VB equivalent to 'volatile':
		Private allowCoreThreadTimeOut_Renamed As Boolean

		''' <summary>
		''' Core pool size is the minimum number of workers to keep alive
		''' (and not allow to time out etc) unless allowCoreThreadTimeOut
		''' is set, in which case the minimum is zero.
		''' </summary>
'JAVA TO VB CONVERTER TODO TASK: There is no VB equivalent to 'volatile':
		Private corePoolSize As Integer

		''' <summary>
		''' Maximum pool size. Note that the actual maximum is internally
		''' bounded by CAPACITY.
		''' </summary>
'JAVA TO VB CONVERTER TODO TASK: There is no VB equivalent to 'volatile':
		Private maximumPoolSize As Integer

		''' <summary>
		''' The default rejected execution handler
		''' </summary>
		Private Shared ReadOnly defaultHandler As RejectedExecutionHandler = New AbortPolicy

		''' <summary>
		''' Permission required for callers of shutdown and shutdownNow.
		''' We additionally require (see checkShutdownAccess) that callers
		''' have permission to actually interrupt threads in the worker set
		''' (as governed by Thread.interrupt, which relies on
		''' ThreadGroup.checkAccess, which in turn relies on
		''' SecurityManager.checkAccess). Shutdowns are attempted only if
		''' these checks pass.
		''' 
		''' All actual invocations of Thread.interrupt (see
		''' interruptIdleWorkers and interruptWorkers) ignore
		''' SecurityExceptions, meaning that the attempted interrupts
		''' silently fail. In the case of shutdown, they should not fail
		''' unless the SecurityManager has inconsistent policies, sometimes
		''' allowing access to a thread and sometimes not. In such cases,
		''' failure to actually interrupt threads may disable or delay full
		''' termination. Other uses of interruptIdleWorkers are advisory,
		''' and failure to actually interrupt will merely delay response to
		''' configuration changes so is not handled exceptionally.
		''' </summary>
		Private Shared ReadOnly shutdownPerm As New RuntimePermission("modifyThread")

		''' <summary>
		''' Class Worker mainly maintains interrupt control state for
		''' threads running tasks, along with other minor bookkeeping.
		''' This class opportunistically extends AbstractQueuedSynchronizer
		''' to simplify acquiring and releasing a lock surrounding each
		''' task execution.  This protects against interrupts that are
		''' intended to wake up a worker thread waiting for a task from
		''' instead interrupting a task being run.  We implement a simple
		''' non-reentrant mutual exclusion lock rather than use
		''' ReentrantLock because we do not want worker tasks to be able to
		''' reacquire the lock when they invoke pool control methods like
		''' setCorePoolSize.  Additionally, to suppress interrupts until
		''' the thread actually starts running tasks, we initialize lock
		''' state to a negative value, and clear it upon start (in
		''' runWorker).
		''' </summary>
		Private NotInheritable Class Worker
			Inherits java.util.concurrent.locks.AbstractQueuedSynchronizer
			Implements Runnable

			Private ReadOnly outerInstance As ThreadPoolExecutor

			''' <summary>
			''' This class will never be serialized, but we provide a
			''' serialVersionUID to suppress a javac warning.
			''' </summary>
			Private Const serialVersionUID As Long = 6138294804551838833L

			''' <summary>
			''' Thread this worker is running in.  Null if factory fails. </summary>
			Friend ReadOnly thread_Renamed As Thread
			''' <summary>
			''' Initial task to run.  Possibly null. </summary>
			Friend firstTask As Runnable
			''' <summary>
			''' Per-thread task counter </summary>
'JAVA TO VB CONVERTER TODO TASK: There is no VB equivalent to 'volatile':
			Friend completedTasks As Long

			''' <summary>
			''' Creates with given first task and thread from ThreadFactory. </summary>
			''' <param name="firstTask"> the first task (null if none) </param>
			Friend Sub New(ByVal outerInstance As ThreadPoolExecutor, ByVal firstTask As Runnable)
					Me.outerInstance = outerInstance
				state = -1 ' inhibit interrupts until runWorker
				Me.firstTask = firstTask
				Me.thread_Renamed = outerInstance.threadFactory.newThread(Me)
			End Sub

			''' <summary>
			''' Delegates main run loop to outer runWorker </summary>
			Public Sub run() Implements Runnable.run
				outerInstance.runWorker(Me)
			End Sub

			' Lock methods
			'
			' The value 0 represents the unlocked state.
			' The value 1 represents the locked state.

			Protected Friend Property Overrides heldExclusively As Boolean
				Get
					Return state <> 0
				End Get
			End Property

			Protected Friend Overrides Function tryAcquire(ByVal unused As Integer) As Boolean
				If compareAndSetState(0, 1) Then
					exclusiveOwnerThread = Thread.CurrentThread
					Return True
				End If
				Return False
			End Function

			Protected Friend Overrides Function tryRelease(ByVal unused As Integer) As Boolean
				exclusiveOwnerThread = Nothing
				state = 0
				Return True
			End Function

			Public Sub lock()
				acquire(1)
			End Sub
			Public Function tryLock() As Boolean
				Return tryAcquire(1)
			End Function
			Public Sub unlock()
				release(1)
			End Sub
			Public Property locked As Boolean
				Get
					Return heldExclusively
				End Get
			End Property

			Friend Sub interruptIfStarted()
				Dim t As Thread
				t = thread_Renamed
				If state >= 0 AndAlso t IsNot Nothing AndAlso (Not t.interrupted) Then
					Try
						t.interrupt()
					Catch ignore As SecurityException
					End Try
				End If
			End Sub
		End Class

	'    
	'     * Methods for setting control state
	'     

		''' <summary>
		''' Transitions runState to given target, or leaves it alone if
		''' already at least the given target.
		''' </summary>
		''' <param name="targetState"> the desired state, either SHUTDOWN or STOP
		'''        (but not TIDYING or TERMINATED -- use tryTerminate for that) </param>
		Private Sub advanceRunState(ByVal targetState As Integer)
			Do
				Dim c As Integer = ctl.get()
				If runStateAtLeast(c, targetState) OrElse ctl.compareAndSet(c, ctlOf(targetState, workerCountOf(c))) Then Exit Do
			Loop
		End Sub

		''' <summary>
		''' Transitions to TERMINATED state if either (SHUTDOWN and pool
		''' and queue empty) or (STOP and pool empty).  If otherwise
		''' eligible to terminate but workerCount is nonzero, interrupts an
		''' idle worker to ensure that shutdown signals propagate. This
		''' method must be called following any action that might make
		''' termination possible -- reducing worker count or removing tasks
		''' from the queue during shutdown. The method is non-private to
		''' allow access from ScheduledThreadPoolExecutor.
		''' </summary>
		Friend Sub tryTerminate()
			Do
				Dim c As Integer = ctl.get()
				If isRunning(c) OrElse runStateAtLeast(c, TIDYING) OrElse (runStateOf(c) = SHUTDOWN_Renamed AndAlso (Not workQueue.empty)) Then Return
				If workerCountOf(c) <> 0 Then ' Eligible to terminate
					interruptIdleWorkers(ONLY_ONE)
					Return
				End If

				Dim mainLock As java.util.concurrent.locks.ReentrantLock = Me.mainLock
				mainLock.lock()
				Try
					If ctl.compareAndSet(c, ctlOf(TIDYING, 0)) Then
						Try
							terminated()
						Finally
							ctl.set(ctlOf(TERMINATED_Renamed, 0))
							termination.signalAll()
						End Try
						Return
					End If
				Finally
					mainLock.unlock()
				End Try
				' else retry on failed CAS
			Loop
		End Sub

	'    
	'     * Methods for controlling interrupts to worker threads.
	'     

		''' <summary>
		''' If there is a security manager, makes sure caller has
		''' permission to shut down threads in general (see shutdownPerm).
		''' If this passes, additionally makes sure the caller is allowed
		''' to interrupt each worker thread. This might not be true even if
		''' first check passed, if the SecurityManager treats some threads
		''' specially.
		''' </summary>
		Private Sub checkShutdownAccess()
			Dim security As SecurityManager = System.securityManager
			If security IsNot Nothing Then
				security.checkPermission(shutdownPerm)
				Dim mainLock As java.util.concurrent.locks.ReentrantLock = Me.mainLock
				mainLock.lock()
				Try
					For Each w As Worker In workers
						security.checkAccess(w.thread_Renamed)
					Next w
				Finally
					mainLock.unlock()
				End Try
			End If
		End Sub

		''' <summary>
		''' Interrupts all threads, even if active. Ignores SecurityExceptions
		''' (in which case some threads may remain uninterrupted).
		''' </summary>
		Private Sub interruptWorkers()
			Dim mainLock As java.util.concurrent.locks.ReentrantLock = Me.mainLock
			mainLock.lock()
			Try
				For Each w As Worker In workers
					w.interruptIfStarted()
				Next w
			Finally
				mainLock.unlock()
			End Try
		End Sub

		''' <summary>
		''' Interrupts threads that might be waiting for tasks (as
		''' indicated by not being locked) so they can check for
		''' termination or configuration changes. Ignores
		''' SecurityExceptions (in which case some threads may remain
		''' uninterrupted).
		''' </summary>
		''' <param name="onlyOne"> If true, interrupt at most one worker. This is
		''' called only from tryTerminate when termination is otherwise
		''' enabled but there are still other workers.  In this case, at
		''' most one waiting worker is interrupted to propagate shutdown
		''' signals in case all threads are currently waiting.
		''' Interrupting any arbitrary thread ensures that newly arriving
		''' workers since shutdown began will also eventually exit.
		''' To guarantee eventual termination, it suffices to always
		''' interrupt only one idle worker, but shutdown() interrupts all
		''' idle workers so that redundant workers exit promptly, not
		''' waiting for a straggler task to finish. </param>
		Private Sub interruptIdleWorkers(ByVal onlyOne As Boolean)
			Dim mainLock As java.util.concurrent.locks.ReentrantLock = Me.mainLock
			mainLock.lock()
			Try
				For Each w As Worker In workers
					Dim t As Thread = w.thread_Renamed
					If (Not t.interrupted) AndAlso w.tryLock() Then
						Try
							t.interrupt()
						Catch ignore As SecurityException
						Finally
							w.unlock()
						End Try
					End If
					If onlyOne Then Exit For
				Next w
			Finally
				mainLock.unlock()
			End Try
		End Sub

		''' <summary>
		''' Common form of interruptIdleWorkers, to avoid having to
		''' remember what the boolean argument means.
		''' </summary>
		Private Sub interruptIdleWorkers()
			interruptIdleWorkers(False)
		End Sub

		Private Const ONLY_ONE As Boolean = True

	'    
	'     * Misc utilities, most of which are also exported to
	'     * ScheduledThreadPoolExecutor
	'     

		''' <summary>
		''' Invokes the rejected execution handler for the given command.
		''' Package-protected for use by ScheduledThreadPoolExecutor.
		''' </summary>
		Friend Sub reject(ByVal command As Runnable)
			handler.rejectedExecution(command, Me)
		End Sub

		''' <summary>
		''' Performs any further cleanup following run state transition on
		''' invocation of shutdown.  A no-op here, but used by
		''' ScheduledThreadPoolExecutor to cancel delayed tasks.
		''' </summary>
		Friend Overridable Sub onShutdown()
		End Sub

		''' <summary>
		''' State check needed by ScheduledThreadPoolExecutor to
		''' enable running tasks during shutdown.
		''' </summary>
		''' <param name="shutdownOK"> true if should return true if SHUTDOWN </param>
		Friend Function isRunningOrShutdown(ByVal shutdownOK As Boolean) As Boolean
			Dim rs As Integer = runStateOf(ctl.get())
			Return rs = RUNNING OrElse (rs = SHUTDOWN_Renamed AndAlso shutdownOK)
		End Function

		''' <summary>
		''' Drains the task queue into a new list, normally using
		''' drainTo. But if the queue is a DelayQueue or any other kind of
		''' queue for which poll or drainTo may fail to remove some
		''' elements, it deletes them one by one.
		''' </summary>
		Private Function drainQueue() As List(Of Runnable)
			Dim q As BlockingQueue(Of Runnable) = workQueue
			Dim taskList As New List(Of Runnable)
			q.drainTo(taskList)
			If Not q.empty Then
				For Each r As Runnable In q.ToArray(New Runnable(){})
					If q.remove(r) Then taskList.add(r)
				Next r
			End If
			Return taskList
		End Function

	'    
	'     * Methods for creating, running and cleaning up after workers
	'     

		''' <summary>
		''' Checks if a new worker can be added with respect to current
		''' pool state and the given bound (either core or maximum). If so,
		''' the worker count is adjusted accordingly, and, if possible, a
		''' new worker is created and started, running firstTask as its
		''' first task. This method returns false if the pool is stopped or
		''' eligible to shut down. It also returns false if the thread
		''' factory fails to create a thread when asked.  If the thread
		''' creation fails, either due to the thread factory returning
		''' null, or due to an exception (typically OutOfMemoryError in
		''' Thread.start()), we roll back cleanly.
		''' </summary>
		''' <param name="firstTask"> the task the new thread should run first (or
		''' null if none). Workers are created with an initial first task
		''' (in method execute()) to bypass queuing when there are fewer
		''' than corePoolSize threads (in which case we always start one),
		''' or when the queue is full (in which case we must bypass queue).
		''' Initially idle threads are usually created via
		''' prestartCoreThread or to replace other dying workers.
		''' </param>
		''' <param name="core"> if true use corePoolSize as bound, else
		''' maximumPoolSize. (A boolean indicator is used here rather than a
		''' value to ensure reads of fresh values after checking other pool
		''' state). </param>
		''' <returns> true if successful </returns>
		Private Function addWorker(ByVal firstTask As Runnable, ByVal core As Boolean) As Boolean
			retry:
			Do
				Dim c As Integer = ctl.get()
				Dim rs As Integer = runStateOf(c)

				' Check if queue empty only if necessary.
				If rs >= SHUTDOWN_Renamed AndAlso Not(rs = SHUTDOWN_Renamed AndAlso firstTask Is Nothing AndAlso (Not workQueue.empty)) Then Return False

				Do
					Dim wc As Integer = workerCountOf(c)
					If wc >= CAPACITY OrElse wc >= (If(core, corePoolSize, maximumPoolSize)) Then Return False
					If compareAndIncrementWorkerCount(c) Then GoTo retry
					c = ctl.get() ' Re-read ctl
					If runStateOf(c) <> rs Then GoTo retry
					' else CAS failed due to workerCount change; retry inner loop
				Loop
			Loop

			Dim workerStarted As Boolean = False
			Dim workerAdded As Boolean = False
			Dim w As Worker = Nothing
			Try
				w = New Worker(Me, firstTask)
				Dim t As Thread = w.thread_Renamed
				If t IsNot Nothing Then
					Dim mainLock As java.util.concurrent.locks.ReentrantLock = Me.mainLock
					mainLock.lock()
					Try
						' Recheck while holding lock.
						' Back out on ThreadFactory failure or if
						' shut down before lock acquired.
						Dim rs As Integer = runStateOf(ctl.get())

						If rs < SHUTDOWN_Renamed OrElse (rs = SHUTDOWN_Renamed AndAlso firstTask Is Nothing) Then
							If t.alive Then ' precheck that t is startable Throw New IllegalThreadStateException
							workers.add(w)
							Dim s As Integer = workers.size()
							If s > largestPoolSize Then largestPoolSize = s
							workerAdded = True
						End If
					Finally
						mainLock.unlock()
					End Try
					If workerAdded Then
						t.start()
						workerStarted = True
					End If
				End If
			Finally
				If Not workerStarted Then addWorkerFailed(w)
			End Try
			Return workerStarted
		End Function

		''' <summary>
		''' Rolls back the worker thread creation.
		''' - removes worker from workers, if present
		''' - decrements worker count
		''' - rechecks for termination, in case the existence of this
		'''   worker was holding up termination
		''' </summary>
		Private Sub addWorkerFailed(ByVal w As Worker)
			Dim mainLock As java.util.concurrent.locks.ReentrantLock = Me.mainLock
			mainLock.lock()
			Try
				If w IsNot Nothing Then workers.remove(w)
				decrementWorkerCount()
				tryTerminate()
			Finally
				mainLock.unlock()
			End Try
		End Sub

		''' <summary>
		''' Performs cleanup and bookkeeping for a dying worker. Called
		''' only from worker threads. Unless completedAbruptly is set,
		''' assumes that workerCount has already been adjusted to account
		''' for exit.  This method removes thread from worker set, and
		''' possibly terminates the pool or replaces the worker if either
		''' it exited due to user task exception or if fewer than
		''' corePoolSize workers are running or queue is non-empty but
		''' there are no workers.
		''' </summary>
		''' <param name="w"> the worker </param>
		''' <param name="completedAbruptly"> if the worker died due to user exception </param>
		Private Sub processWorkerExit(ByVal w As Worker, ByVal completedAbruptly As Boolean)
			If completedAbruptly Then ' If abrupt, then workerCount wasn't adjusted decrementWorkerCount()

			Dim mainLock As java.util.concurrent.locks.ReentrantLock = Me.mainLock
			mainLock.lock()
			Try
				completedTaskCount += w.completedTasks
				workers.remove(w)
			Finally
				mainLock.unlock()
			End Try

			tryTerminate()

			Dim c As Integer = ctl.get()
			If runStateLessThan(c, [STOP]) Then
				If Not completedAbruptly Then
					Dim min As Integer = If(allowCoreThreadTimeOut_Renamed, 0, corePoolSize)
					If min = 0 AndAlso (Not workQueue.empty) Then min = 1
					If workerCountOf(c) >= min Then Return ' replacement not needed
				End If
				addWorker(Nothing, False)
			End If
		End Sub

		''' <summary>
		''' Performs blocking or timed wait for a task, depending on
		''' current configuration settings, or returns null if this worker
		''' must exit because of any of:
		''' 1. There are more than maximumPoolSize workers (due to
		'''    a call to setMaximumPoolSize).
		''' 2. The pool is stopped.
		''' 3. The pool is shutdown and the queue is empty.
		''' 4. This worker timed out waiting for a task, and timed-out
		'''    workers are subject to termination (that is,
		'''    {@code allowCoreThreadTimeOut || workerCount > corePoolSize})
		'''    both before and after the timed wait, and if the queue is
		'''    non-empty, this worker is not the last thread in the pool.
		''' </summary>
		''' <returns> task, or null if the worker must exit, in which case
		'''         workerCount is decremented </returns>
		Private Property task As Runnable
			Get
				Dim timedOut As Boolean = False ' Did the last poll() time out?
    
				Do
					Dim c As Integer = ctl.get()
					Dim rs As Integer = runStateOf(c)
    
					' Check if queue empty only if necessary.
					If rs >= SHUTDOWN_Renamed AndAlso (rs >= [STOP] OrElse workQueue.empty) Then
						decrementWorkerCount()
						Return Nothing
					End If
    
					Dim wc As Integer = workerCountOf(c)
    
					' Are workers subject to culling?
					Dim timed As Boolean = allowCoreThreadTimeOut_Renamed OrElse wc > corePoolSize
    
					If (wc > maximumPoolSize OrElse (timed AndAlso timedOut)) AndAlso (wc > 1 OrElse workQueue.empty) Then
						If compareAndDecrementWorkerCount(c) Then Return Nothing
						Continue Do
					End If
    
					Try
						Dim r As Runnable = If(timed, workQueue.poll(keepAliveTime, TimeUnit.NANOSECONDS), workQueue.take())
						If r IsNot Nothing Then Return r
						timedOut = True
					Catch retry As InterruptedException
						timedOut = False
					End Try
				Loop
			End Get
		End Property

		''' <summary>
		''' Main worker run loop.  Repeatedly gets tasks from queue and
		''' executes them, while coping with a number of issues:
		''' 
		''' 1. We may start out with an initial task, in which case we
		''' don't need to get the first one. Otherwise, as long as pool is
		''' running, we get tasks from getTask. If it returns null then the
		''' worker exits due to changed pool state or configuration
		''' parameters.  Other exits result from exception throws in
		''' external code, in which case completedAbruptly holds, which
		''' usually leads processWorkerExit to replace this thread.
		''' 
		''' 2. Before running any task, the lock is acquired to prevent
		''' other pool interrupts while the task is executing, and then we
		''' ensure that unless pool is stopping, this thread does not have
		''' its interrupt set.
		''' 
		''' 3. Each task run is preceded by a call to beforeExecute, which
		''' might throw an exception, in which case we cause thread to die
		''' (breaking loop with completedAbruptly true) without processing
		''' the task.
		''' 
		''' 4. Assuming beforeExecute completes normally, we run the task,
		''' gathering any of its thrown exceptions to send to afterExecute.
		''' We separately handle RuntimeException, Error (both of which the
		''' specs guarantee that we trap) and arbitrary Throwables.
		''' Because we cannot rethrow Throwables within Runnable.run, we
		''' wrap them within Errors on the way out (to the thread's
		''' UncaughtExceptionHandler).  Any thrown exception also
		''' conservatively causes thread to die.
		''' 
		''' 5. After task.run completes, we call afterExecute, which may
		''' also throw an exception, which will also cause thread to
		''' die. According to JLS Sec 14.20, this exception is the one that
		''' will be in effect even if task.run throws.
		''' 
		''' The net effect of the exception mechanics is that afterExecute
		''' and the thread's UncaughtExceptionHandler have as accurate
		''' information as we can provide about any problems encountered by
		''' user code.
		''' </summary>
		''' <param name="w"> the worker </param>
		Friend Sub runWorker(ByVal w As Worker)
			Dim wt As Thread = Thread.CurrentThread
			Dim task_Renamed As Runnable = w.firstTask
			w.firstTask = Nothing
			w.unlock() ' allow interrupts
			Dim completedAbruptly As Boolean = True
			Try
				task_Renamed = task
				Do While task_Renamed IsNot Nothing OrElse task_Renamed IsNot Nothing
					w.lock()
					' If pool is stopping, ensure thread is interrupted;
					' if not, ensure thread is not interrupted.  This
					' requires a recheck in second case to deal with
					' shutdownNow race while clearing interrupt
					If (runStateAtLeast(ctl.get(), [STOP]) OrElse (Thread.interrupted() AndAlso runStateAtLeast(ctl.get(), [STOP]))) AndAlso (Not wt.interrupted) Then wt.interrupt()
					Try
						beforeExecute(wt, task_Renamed)
						Dim thrown As Throwable = Nothing
						Try
							task_Renamed.run()
						Catch x As RuntimeException
							thrown = x
							Throw x
						Catch x As [Error]
							thrown = x
							Throw x
						Catch x As Throwable
							thrown = x
							Throw New [Error](x)
						Finally
							afterExecute(task_Renamed, thrown)
						End Try
					Finally
						task_Renamed = Nothing
						w.completedTasks += 1
						w.unlock()
					End Try
					task_Renamed = task
				Loop
				completedAbruptly = False
			Finally
				processWorkerExit(w, completedAbruptly)
			End Try
		End Sub

		' Public constructors and methods

		''' <summary>
		''' Creates a new {@code ThreadPoolExecutor} with the given initial
		''' parameters and default thread factory and rejected execution handler.
		''' It may be more convenient to use one of the <seealso cref="Executors"/> factory
		''' methods instead of this general purpose constructor.
		''' </summary>
		''' <param name="corePoolSize"> the number of threads to keep in the pool, even
		'''        if they are idle, unless {@code allowCoreThreadTimeOut} is set </param>
		''' <param name="maximumPoolSize"> the maximum number of threads to allow in the
		'''        pool </param>
		''' <param name="keepAliveTime"> when the number of threads is greater than
		'''        the core, this is the maximum time that excess idle threads
		'''        will wait for new tasks before terminating. </param>
		''' <param name="unit"> the time unit for the {@code keepAliveTime} argument </param>
		''' <param name="workQueue"> the queue to use for holding tasks before they are
		'''        executed.  This queue will hold only the {@code Runnable}
		'''        tasks submitted by the {@code execute} method. </param>
		''' <exception cref="IllegalArgumentException"> if one of the following holds:<br>
		'''         {@code corePoolSize < 0}<br>
		'''         {@code keepAliveTime < 0}<br>
		'''         {@code maximumPoolSize <= 0}<br>
		'''         {@code maximumPoolSize < corePoolSize} </exception>
		''' <exception cref="NullPointerException"> if {@code workQueue} is null </exception>
		Public Sub New(ByVal corePoolSize As Integer, ByVal maximumPoolSize As Integer, ByVal keepAliveTime As Long, ByVal unit As TimeUnit, ByVal workQueue As BlockingQueue(Of Runnable))
			Me.New(corePoolSize, maximumPoolSize, keepAliveTime, unit, workQueue, Executors.defaultThreadFactory(), defaultHandler)
		End Sub

		''' <summary>
		''' Creates a new {@code ThreadPoolExecutor} with the given initial
		''' parameters and default rejected execution handler.
		''' </summary>
		''' <param name="corePoolSize"> the number of threads to keep in the pool, even
		'''        if they are idle, unless {@code allowCoreThreadTimeOut} is set </param>
		''' <param name="maximumPoolSize"> the maximum number of threads to allow in the
		'''        pool </param>
		''' <param name="keepAliveTime"> when the number of threads is greater than
		'''        the core, this is the maximum time that excess idle threads
		'''        will wait for new tasks before terminating. </param>
		''' <param name="unit"> the time unit for the {@code keepAliveTime} argument </param>
		''' <param name="workQueue"> the queue to use for holding tasks before they are
		'''        executed.  This queue will hold only the {@code Runnable}
		'''        tasks submitted by the {@code execute} method. </param>
		''' <param name="threadFactory"> the factory to use when the executor
		'''        creates a new thread </param>
		''' <exception cref="IllegalArgumentException"> if one of the following holds:<br>
		'''         {@code corePoolSize < 0}<br>
		'''         {@code keepAliveTime < 0}<br>
		'''         {@code maximumPoolSize <= 0}<br>
		'''         {@code maximumPoolSize < corePoolSize} </exception>
		''' <exception cref="NullPointerException"> if {@code workQueue}
		'''         or {@code threadFactory} is null </exception>
		Public Sub New(ByVal corePoolSize As Integer, ByVal maximumPoolSize As Integer, ByVal keepAliveTime As Long, ByVal unit As TimeUnit, ByVal workQueue As BlockingQueue(Of Runnable), ByVal threadFactory As ThreadFactory)
			Me.New(corePoolSize, maximumPoolSize, keepAliveTime, unit, workQueue, threadFactory, defaultHandler)
		End Sub

		''' <summary>
		''' Creates a new {@code ThreadPoolExecutor} with the given initial
		''' parameters and default thread factory.
		''' </summary>
		''' <param name="corePoolSize"> the number of threads to keep in the pool, even
		'''        if they are idle, unless {@code allowCoreThreadTimeOut} is set </param>
		''' <param name="maximumPoolSize"> the maximum number of threads to allow in the
		'''        pool </param>
		''' <param name="keepAliveTime"> when the number of threads is greater than
		'''        the core, this is the maximum time that excess idle threads
		'''        will wait for new tasks before terminating. </param>
		''' <param name="unit"> the time unit for the {@code keepAliveTime} argument </param>
		''' <param name="workQueue"> the queue to use for holding tasks before they are
		'''        executed.  This queue will hold only the {@code Runnable}
		'''        tasks submitted by the {@code execute} method. </param>
		''' <param name="handler"> the handler to use when execution is blocked
		'''        because the thread bounds and queue capacities are reached </param>
		''' <exception cref="IllegalArgumentException"> if one of the following holds:<br>
		'''         {@code corePoolSize < 0}<br>
		'''         {@code keepAliveTime < 0}<br>
		'''         {@code maximumPoolSize <= 0}<br>
		'''         {@code maximumPoolSize < corePoolSize} </exception>
		''' <exception cref="NullPointerException"> if {@code workQueue}
		'''         or {@code handler} is null </exception>
		Public Sub New(ByVal corePoolSize As Integer, ByVal maximumPoolSize As Integer, ByVal keepAliveTime As Long, ByVal unit As TimeUnit, ByVal workQueue As BlockingQueue(Of Runnable), ByVal handler As RejectedExecutionHandler)
			Me.New(corePoolSize, maximumPoolSize, keepAliveTime, unit, workQueue, Executors.defaultThreadFactory(), handler)
		End Sub

		''' <summary>
		''' Creates a new {@code ThreadPoolExecutor} with the given initial
		''' parameters.
		''' </summary>
		''' <param name="corePoolSize"> the number of threads to keep in the pool, even
		'''        if they are idle, unless {@code allowCoreThreadTimeOut} is set </param>
		''' <param name="maximumPoolSize"> the maximum number of threads to allow in the
		'''        pool </param>
		''' <param name="keepAliveTime"> when the number of threads is greater than
		'''        the core, this is the maximum time that excess idle threads
		'''        will wait for new tasks before terminating. </param>
		''' <param name="unit"> the time unit for the {@code keepAliveTime} argument </param>
		''' <param name="workQueue"> the queue to use for holding tasks before they are
		'''        executed.  This queue will hold only the {@code Runnable}
		'''        tasks submitted by the {@code execute} method. </param>
		''' <param name="threadFactory"> the factory to use when the executor
		'''        creates a new thread </param>
		''' <param name="handler"> the handler to use when execution is blocked
		'''        because the thread bounds and queue capacities are reached </param>
		''' <exception cref="IllegalArgumentException"> if one of the following holds:<br>
		'''         {@code corePoolSize < 0}<br>
		'''         {@code keepAliveTime < 0}<br>
		'''         {@code maximumPoolSize <= 0}<br>
		'''         {@code maximumPoolSize < corePoolSize} </exception>
		''' <exception cref="NullPointerException"> if {@code workQueue}
		'''         or {@code threadFactory} or {@code handler} is null </exception>
		Public Sub New(ByVal corePoolSize As Integer, ByVal maximumPoolSize As Integer, ByVal keepAliveTime As Long, ByVal unit As TimeUnit, ByVal workQueue As BlockingQueue(Of Runnable), ByVal threadFactory As ThreadFactory, ByVal handler As RejectedExecutionHandler)
			If corePoolSize < 0 OrElse maximumPoolSize <= 0 OrElse maximumPoolSize < corePoolSize OrElse keepAliveTime < 0 Then Throw New IllegalArgumentException
			If workQueue Is Nothing OrElse threadFactory Is Nothing OrElse handler Is Nothing Then Throw New NullPointerException
			Me.corePoolSize = corePoolSize
			Me.maximumPoolSize = maximumPoolSize
			Me.workQueue = workQueue
			Me.keepAliveTime = unit.toNanos(keepAliveTime)
			Me.threadFactory = threadFactory
			Me.handler = handler
		End Sub

		''' <summary>
		''' Executes the given task sometime in the future.  The task
		''' may execute in a new thread or in an existing pooled thread.
		''' 
		''' If the task cannot be submitted for execution, either because this
		''' executor has been shutdown or because its capacity has been reached,
		''' the task is handled by the current {@code RejectedExecutionHandler}.
		''' </summary>
		''' <param name="command"> the task to execute </param>
		''' <exception cref="RejectedExecutionException"> at discretion of
		'''         {@code RejectedExecutionHandler}, if the task
		'''         cannot be accepted for execution </exception>
		''' <exception cref="NullPointerException"> if {@code command} is null </exception>
		Public Overridable Sub execute(ByVal command As Runnable)
			If command Is Nothing Then Throw New NullPointerException
	'        
	'         * Proceed in 3 steps:
	'         *
	'         * 1. If fewer than corePoolSize threads are running, try to
	'         * start a new thread with the given command as its first
	'         * task.  The call to addWorker atomically checks runState and
	'         * workerCount, and so prevents false alarms that would add
	'         * threads when it shouldn't, by returning false.
	'         *
	'         * 2. If a task can be successfully queued, then we still need
	'         * to double-check whether we should have added a thread
	'         * (because existing ones died since last checking) or that
	'         * the pool shut down since entry into this method. So we
	'         * recheck state and if necessary roll back the enqueuing if
	'         * stopped, or start a new thread if there are none.
	'         *
	'         * 3. If we cannot queue task, then we try to add a new
	'         * thread.  If it fails, we know we are shut down or saturated
	'         * and so reject the task.
	'         
			Dim c As Integer = ctl.get()
			If workerCountOf(c) < corePoolSize Then
				If addWorker(command, True) Then Return
				c = ctl.get()
			End If
			If isRunning(c) AndAlso workQueue.offer(command) Then
				Dim recheck As Integer = ctl.get()
				If (Not isRunning(recheck)) AndAlso remove(command) Then
					reject(command)
				ElseIf workerCountOf(recheck) = 0 Then
					addWorker(Nothing, False)
				End If
			ElseIf Not addWorker(command, False) Then
				reject(command)
			End If
		End Sub

		''' <summary>
		''' Initiates an orderly shutdown in which previously submitted
		''' tasks are executed, but no new tasks will be accepted.
		''' Invocation has no additional effect if already shut down.
		''' 
		''' <p>This method does not wait for previously submitted tasks to
		''' complete execution.  Use <seealso cref="#awaitTermination awaitTermination"/>
		''' to do that.
		''' </summary>
		''' <exception cref="SecurityException"> {@inheritDoc} </exception>
		Public Overrides Sub shutdown()
			Dim mainLock As java.util.concurrent.locks.ReentrantLock = Me.mainLock
			mainLock.lock()
			Try
				checkShutdownAccess()
				advanceRunState(SHUTDOWN_Renamed)
				interruptIdleWorkers()
				onShutdown() ' hook for ScheduledThreadPoolExecutor
			Finally
				mainLock.unlock()
			End Try
			tryTerminate()
		End Sub

		''' <summary>
		''' Attempts to stop all actively executing tasks, halts the
		''' processing of waiting tasks, and returns a list of the tasks
		''' that were awaiting execution. These tasks are drained (removed)
		''' from the task queue upon return from this method.
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
		''' <exception cref="SecurityException"> {@inheritDoc} </exception>
		Public Overrides Function shutdownNow() As List(Of Runnable)
			Dim tasks As List(Of Runnable)
			Dim mainLock As java.util.concurrent.locks.ReentrantLock = Me.mainLock
			mainLock.lock()
			Try
				checkShutdownAccess()
				advanceRunState([STOP])
				interruptWorkers()
				tasks = drainQueue()
			Finally
				mainLock.unlock()
			End Try
			tryTerminate()
			Return tasks
		End Function

		Public Property Overrides shutdown As Boolean
			Get
				Return Not isRunning(ctl.get())
			End Get
		End Property

		''' <summary>
		''' Returns true if this executor is in the process of terminating
		''' after <seealso cref="#shutdown"/> or <seealso cref="#shutdownNow"/> but has not
		''' completely terminated.  This method may be useful for
		''' debugging. A return of {@code true} reported a sufficient
		''' period after shutdown may indicate that submitted tasks have
		''' ignored or suppressed interruption, causing this executor not
		''' to properly terminate.
		''' </summary>
		''' <returns> {@code true} if terminating but not yet terminated </returns>
		Public Overridable Property terminating As Boolean
			Get
				Dim c As Integer = ctl.get()
				Return (Not isRunning(c)) AndAlso runStateLessThan(c, TERMINATED_Renamed)
			End Get
		End Property

		Public Property Overrides terminated As Boolean
			Get
				Return runStateAtLeast(ctl.get(), TERMINATED_Renamed)
			End Get
		End Property

		Public Overrides Function awaitTermination(ByVal timeout As Long, ByVal unit As TimeUnit) As Boolean
			Dim nanos As Long = unit.toNanos(timeout)
			Dim mainLock As java.util.concurrent.locks.ReentrantLock = Me.mainLock
			mainLock.lock()
			Try
				Do
					If runStateAtLeast(ctl.get(), TERMINATED_Renamed) Then Return True
					If nanos <= 0 Then Return False
					nanos = termination.awaitNanos(nanos)
				Loop
			Finally
				mainLock.unlock()
			End Try
		End Function

		''' <summary>
		''' Invokes {@code shutdown} when this executor is no longer
		''' referenced and it has no threads.
		''' </summary>
		Protected Overrides Sub Finalize()
			shutdown()
		End Sub

		''' <summary>
		''' Sets the thread factory used to create new threads.
		''' </summary>
		''' <param name="threadFactory"> the new thread factory </param>
		''' <exception cref="NullPointerException"> if threadFactory is null </exception>
		''' <seealso cref= #getThreadFactory </seealso>
		Public Overridable Property threadFactory As ThreadFactory
			Set(ByVal threadFactory As ThreadFactory)
				If threadFactory Is Nothing Then Throw New NullPointerException
				Me.threadFactory = threadFactory
			End Set
			Get
				Return threadFactory
			End Get
		End Property


		''' <summary>
		''' Sets a new handler for unexecutable tasks.
		''' </summary>
		''' <param name="handler"> the new handler </param>
		''' <exception cref="NullPointerException"> if handler is null </exception>
		''' <seealso cref= #getRejectedExecutionHandler </seealso>
		Public Overridable Property rejectedExecutionHandler As RejectedExecutionHandler
			Set(ByVal handler As RejectedExecutionHandler)
				If handler Is Nothing Then Throw New NullPointerException
				Me.handler = handler
			End Set
			Get
				Return handler
			End Get
		End Property


		''' <summary>
		''' Sets the core number of threads.  This overrides any value set
		''' in the constructor.  If the new value is smaller than the
		''' current value, excess existing threads will be terminated when
		''' they next become idle.  If larger, new threads will, if needed,
		''' be started to execute any queued tasks.
		''' </summary>
		''' <param name="corePoolSize"> the new core size </param>
		''' <exception cref="IllegalArgumentException"> if {@code corePoolSize < 0} </exception>
		''' <seealso cref= #getCorePoolSize </seealso>
		Public Overridable Property corePoolSize As Integer
			Set(ByVal corePoolSize As Integer)
				If corePoolSize < 0 Then Throw New IllegalArgumentException
				Dim delta As Integer = corePoolSize - Me.corePoolSize
				Me.corePoolSize = corePoolSize
				If workerCountOf(ctl.get()) > corePoolSize Then
					interruptIdleWorkers()
				ElseIf delta > 0 Then
					' We don't really know how many new threads are "needed".
					' As a heuristic, prestart enough new workers (up to new
					' core size) to handle the current number of tasks in
					' queue, but stop if queue becomes empty while doing so.
					Dim k As Integer = System.Math.Min(delta, workQueue.size())
					Dim tempVar As Boolean = k > 0 AndAlso addWorker(Nothing, True)
					k -= 1
					Do While tempVar
						If workQueue.empty Then Exit Do
						tempVar = k > 0 AndAlso addWorker(Nothing, True)
						k -= 1
					Loop
				End If
			End Set
			Get
				Return corePoolSize
			End Get
		End Property


		''' <summary>
		''' Starts a core thread, causing it to idly wait for work. This
		''' overrides the default policy of starting core threads only when
		''' new tasks are executed. This method will return {@code false}
		''' if all core threads have already been started.
		''' </summary>
		''' <returns> {@code true} if a thread was started </returns>
		Public Overridable Function prestartCoreThread() As Boolean
			Return workerCountOf(ctl.get()) < corePoolSize AndAlso addWorker(Nothing, True)
		End Function

		''' <summary>
		''' Same as prestartCoreThread except arranges that at least one
		''' thread is started even if corePoolSize is 0.
		''' </summary>
		Friend Overridable Sub ensurePrestart()
			Dim wc As Integer = workerCountOf(ctl.get())
			If wc < corePoolSize Then
				addWorker(Nothing, True)
			ElseIf wc = 0 Then
				addWorker(Nothing, False)
			End If
		End Sub

		''' <summary>
		''' Starts all core threads, causing them to idly wait for work. This
		''' overrides the default policy of starting core threads only when
		''' new tasks are executed.
		''' </summary>
		''' <returns> the number of threads started </returns>
		Public Overridable Function prestartAllCoreThreads() As Integer
			Dim n As Integer = 0
			Do While addWorker(Nothing, True)
				n += 1
			Loop
			Return n
		End Function

		''' <summary>
		''' Returns true if this pool allows core threads to time out and
		''' terminate if no tasks arrive within the keepAlive time, being
		''' replaced if needed when new tasks arrive. When true, the same
		''' keep-alive policy applying to non-core threads applies also to
		''' core threads. When false (the default), core threads are never
		''' terminated due to lack of incoming tasks.
		''' </summary>
		''' <returns> {@code true} if core threads are allowed to time out,
		'''         else {@code false}
		''' 
		''' @since 1.6 </returns>
		Public Overridable Function allowsCoreThreadTimeOut() As Boolean
			Return allowCoreThreadTimeOut_Renamed
		End Function

		''' <summary>
		''' Sets the policy governing whether core threads may time out and
		''' terminate if no tasks arrive within the keep-alive time, being
		''' replaced if needed when new tasks arrive. When false, core
		''' threads are never terminated due to lack of incoming
		''' tasks. When true, the same keep-alive policy applying to
		''' non-core threads applies also to core threads. To avoid
		''' continual thread replacement, the keep-alive time must be
		''' greater than zero when setting {@code true}. This method
		''' should in general be called before the pool is actively used.
		''' </summary>
		''' <param name="value"> {@code true} if should time out, else {@code false} </param>
		''' <exception cref="IllegalArgumentException"> if value is {@code true}
		'''         and the current keep-alive time is not greater than zero
		''' 
		''' @since 1.6 </exception>
		Public Overridable Sub allowCoreThreadTimeOut(ByVal value As Boolean)
			If value AndAlso keepAliveTime <= 0 Then Throw New IllegalArgumentException("Core threads must have nonzero keep alive times")
			If value <> allowCoreThreadTimeOut_Renamed Then
				allowCoreThreadTimeOut_Renamed = value
				If value Then interruptIdleWorkers()
			End If
		End Sub

		''' <summary>
		''' Sets the maximum allowed number of threads. This overrides any
		''' value set in the constructor. If the new value is smaller than
		''' the current value, excess existing threads will be
		''' terminated when they next become idle.
		''' </summary>
		''' <param name="maximumPoolSize"> the new maximum </param>
		''' <exception cref="IllegalArgumentException"> if the new maximum is
		'''         less than or equal to zero, or
		'''         less than the <seealso cref="#getCorePoolSize core pool size"/> </exception>
		''' <seealso cref= #getMaximumPoolSize </seealso>
		Public Overridable Property maximumPoolSize As Integer
			Set(ByVal maximumPoolSize As Integer)
				If maximumPoolSize <= 0 OrElse maximumPoolSize < corePoolSize Then Throw New IllegalArgumentException
				Me.maximumPoolSize = maximumPoolSize
				If workerCountOf(ctl.get()) > maximumPoolSize Then interruptIdleWorkers()
			End Set
			Get
				Return maximumPoolSize
			End Get
		End Property


		''' <summary>
		''' Sets the time limit for which threads may remain idle before
		''' being terminated.  If there are more than the core number of
		''' threads currently in the pool, after waiting this amount of
		''' time without processing a task, excess threads will be
		''' terminated.  This overrides any value set in the constructor.
		''' </summary>
		''' <param name="time"> the time to wait.  A time value of zero will cause
		'''        excess threads to terminate immediately after executing tasks. </param>
		''' <param name="unit"> the time unit of the {@code time} argument </param>
		''' <exception cref="IllegalArgumentException"> if {@code time} less than zero or
		'''         if {@code time} is zero and {@code allowsCoreThreadTimeOut} </exception>
		''' <seealso cref= #getKeepAliveTime(TimeUnit) </seealso>
		Public Overridable Sub setKeepAliveTime(ByVal time As Long, ByVal unit As TimeUnit)
			If time < 0 Then Throw New IllegalArgumentException
			If time = 0 AndAlso allowsCoreThreadTimeOut() Then Throw New IllegalArgumentException("Core threads must have nonzero keep alive times")
			Dim keepAliveTime_Renamed As Long = unit.toNanos(time)
			Dim delta As Long = keepAliveTime_Renamed - Me.keepAliveTime
			Me.keepAliveTime = keepAliveTime_Renamed
			If delta < 0 Then interruptIdleWorkers()
		End Sub

		''' <summary>
		''' Returns the thread keep-alive time, which is the amount of time
		''' that threads in excess of the core pool size may remain
		''' idle before being terminated.
		''' </summary>
		''' <param name="unit"> the desired time unit of the result </param>
		''' <returns> the time limit </returns>
		''' <seealso cref= #setKeepAliveTime(long, TimeUnit) </seealso>
		Public Overridable Function getKeepAliveTime(ByVal unit As TimeUnit) As Long
			Return unit.convert(keepAliveTime, TimeUnit.NANOSECONDS)
		End Function

		' User-level queue utilities 

		''' <summary>
		''' Returns the task queue used by this executor. Access to the
		''' task queue is intended primarily for debugging and monitoring.
		''' This queue may be in active use.  Retrieving the task queue
		''' does not prevent queued tasks from executing.
		''' </summary>
		''' <returns> the task queue </returns>
		Public Overridable Property queue As BlockingQueue(Of Runnable)
			Get
				Return workQueue
			End Get
		End Property

		''' <summary>
		''' Removes this task from the executor's internal queue if it is
		''' present, thus causing it not to be run if it has not already
		''' started.
		''' 
		''' <p>This method may be useful as one part of a cancellation
		''' scheme.  It may fail to remove tasks that have been converted
		''' into other forms before being placed on the internal queue. For
		''' example, a task entered using {@code submit} might be
		''' converted into a form that maintains {@code Future} status.
		''' However, in such cases, method <seealso cref="#purge"/> may be used to
		''' remove those Futures that have been cancelled.
		''' </summary>
		''' <param name="task"> the task to remove </param>
		''' <returns> {@code true} if the task was removed </returns>
		Public Overridable Function remove(ByVal task As Runnable) As Boolean
			Dim removed As Boolean = workQueue.remove(task)
			tryTerminate() ' In case SHUTDOWN and now empty
			Return removed
		End Function

		''' <summary>
		''' Tries to remove from the work queue all <seealso cref="Future"/>
		''' tasks that have been cancelled. This method can be useful as a
		''' storage reclamation operation, that has no other impact on
		''' functionality. Cancelled tasks are never executed, but may
		''' accumulate in work queues until worker threads can actively
		''' remove them. Invoking this method instead tries to remove them now.
		''' However, this method may fail to remove tasks in
		''' the presence of interference by other threads.
		''' </summary>
		Public Overridable Sub purge()
			Dim q As BlockingQueue(Of Runnable) = workQueue
			Try
				Dim it As [Iterator](Of Runnable) = q.GetEnumerator()
				Do While it.MoveNext()
					Dim r As Runnable = it.Current
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
					If TypeOf r Is Future(Of ?) AndAlso CType(r, Future(Of ?)).cancelled Then it.remove()
				Loop
			Catch fallThrough As ConcurrentModificationException
				' Take slow path if we encounter interference during traversal.
				' Make copy for traversal and call remove for cancelled entries.
				' The slow path is more likely to be O(N*N).
				For Each r As Object In q.ToArray()
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
					If TypeOf r Is Future(Of ?) AndAlso CType(r, Future(Of ?)).cancelled Then q.remove(r)
				Next r
			End Try

			tryTerminate() ' In case SHUTDOWN and now empty
		End Sub

		' Statistics 

		''' <summary>
		''' Returns the current number of threads in the pool.
		''' </summary>
		''' <returns> the number of threads </returns>
		Public Overridable Property poolSize As Integer
			Get
				Dim mainLock As java.util.concurrent.locks.ReentrantLock = Me.mainLock
				mainLock.lock()
				Try
					' Remove rare and surprising possibility of
					' isTerminated() && getPoolSize() > 0
					Return If(runStateAtLeast(ctl.get(), TIDYING), 0, workers.size())
				Finally
					mainLock.unlock()
				End Try
			End Get
		End Property

		''' <summary>
		''' Returns the approximate number of threads that are actively
		''' executing tasks.
		''' </summary>
		''' <returns> the number of threads </returns>
		Public Overridable Property activeCount As Integer
			Get
				Dim mainLock As java.util.concurrent.locks.ReentrantLock = Me.mainLock
				mainLock.lock()
				Try
					Dim n As Integer = 0
					For Each w As Worker In workers
						If w.locked Then n += 1
					Next w
					Return n
				Finally
					mainLock.unlock()
				End Try
			End Get
		End Property

		''' <summary>
		''' Returns the largest number of threads that have ever
		''' simultaneously been in the pool.
		''' </summary>
		''' <returns> the number of threads </returns>
		Public Overridable Property largestPoolSize As Integer
			Get
				Dim mainLock As java.util.concurrent.locks.ReentrantLock = Me.mainLock
				mainLock.lock()
				Try
					Return largestPoolSize
				Finally
					mainLock.unlock()
				End Try
			End Get
		End Property

		''' <summary>
		''' Returns the approximate total number of tasks that have ever been
		''' scheduled for execution. Because the states of tasks and
		''' threads may change dynamically during computation, the returned
		''' value is only an approximation.
		''' </summary>
		''' <returns> the number of tasks </returns>
		Public Overridable Property taskCount As Long
			Get
				Dim mainLock As java.util.concurrent.locks.ReentrantLock = Me.mainLock
				mainLock.lock()
				Try
					Dim n As Long = completedTaskCount
					For Each w As Worker In workers
						n += w.completedTasks
						If w.locked Then n += 1
					Next w
					Return n + workQueue.size()
				Finally
					mainLock.unlock()
				End Try
			End Get
		End Property

		''' <summary>
		''' Returns the approximate total number of tasks that have
		''' completed execution. Because the states of tasks and threads
		''' may change dynamically during computation, the returned value
		''' is only an approximation, but one that does not ever decrease
		''' across successive calls.
		''' </summary>
		''' <returns> the number of tasks </returns>
		Public Overridable Property completedTaskCount As Long
			Get
				Dim mainLock As java.util.concurrent.locks.ReentrantLock = Me.mainLock
				mainLock.lock()
				Try
					Dim n As Long = completedTaskCount
					For Each w As Worker In workers
						n += w.completedTasks
					Next w
					Return n
				Finally
					mainLock.unlock()
				End Try
			End Get
		End Property

		''' <summary>
		''' Returns a string identifying this pool, as well as its state,
		''' including indications of run state and estimated worker and
		''' task counts.
		''' </summary>
		''' <returns> a string identifying this pool, as well as its state </returns>
		Public Overrides Function ToString() As String
			Dim ncompleted As Long
			Dim nworkers, nactive As Integer
			Dim mainLock As java.util.concurrent.locks.ReentrantLock = Me.mainLock
			mainLock.lock()
			Try
				ncompleted = completedTaskCount
				nactive = 0
				nworkers = workers.size()
				For Each w As Worker In workers
					ncompleted += w.completedTasks
					If w.locked Then nactive += 1
				Next w
			Finally
				mainLock.unlock()
			End Try
			Dim c As Integer = ctl.get()
			Dim rs As String = (If(runStateLessThan(c, SHUTDOWN_Renamed), "Running", (If(runStateAtLeast(c, TERMINATED_Renamed), "Terminated", "Shutting down"))))
			Return MyBase.ToString() & "[" & rs & ", pool size = " & nworkers & ", active threads = " & nactive & ", queued tasks = " & workQueue.size() & ", completed tasks = " & ncompleted & "]"
		End Function

		' Extension hooks 

		''' <summary>
		''' Method invoked prior to executing the given Runnable in the
		''' given thread.  This method is invoked by thread {@code t} that
		''' will execute task {@code r}, and may be used to re-initialize
		''' ThreadLocals, or to perform logging.
		''' 
		''' <p>This implementation does nothing, but may be customized in
		''' subclasses. Note: To properly nest multiple overridings, subclasses
		''' should generally invoke {@code super.beforeExecute} at the end of
		''' this method.
		''' </summary>
		''' <param name="t"> the thread that will run task {@code r} </param>
		''' <param name="r"> the task that will be executed </param>
		Protected Friend Overridable Sub beforeExecute(ByVal t As Thread, ByVal r As Runnable)
		End Sub

		''' <summary>
		''' Method invoked upon completion of execution of the given Runnable.
		''' This method is invoked by the thread that executed the task. If
		''' non-null, the Throwable is the uncaught {@code RuntimeException}
		''' or {@code Error} that caused execution to terminate abruptly.
		''' 
		''' <p>This implementation does nothing, but may be customized in
		''' subclasses. Note: To properly nest multiple overridings, subclasses
		''' should generally invoke {@code super.afterExecute} at the
		''' beginning of this method.
		''' 
		''' <p><b>Note:</b> When actions are enclosed in tasks (such as
		''' <seealso cref="FutureTask"/>) either explicitly or via methods such as
		''' {@code submit}, these task objects catch and maintain
		''' computational exceptions, and so they do not cause abrupt
		''' termination, and the internal exceptions are <em>not</em>
		''' passed to this method. If you would like to trap both kinds of
		''' failures in this method, you can further probe for such cases,
		''' as in this sample subclass that prints either the direct cause
		''' or the underlying exception if a task has been aborted:
		''' 
		'''  <pre> {@code
		''' class ExtendedExecutor extends ThreadPoolExecutor {
		'''   // ...
		'''   protected void afterExecute(Runnable r, Throwable t) {
		'''     super.afterExecute(r, t);
		'''     if (t == null && r instanceof Future<?>) {
		'''       try {
		'''         Object result = ((Future<?>) r).get();
		'''       } catch (CancellationException ce) {
		'''           t = ce;
		'''       } catch (ExecutionException ee) {
		'''           t = ee.getCause();
		'''       } catch (InterruptedException ie) {
		'''           Thread.currentThread().interrupt(); // ignore/reset
		'''       }
		'''     }
		'''     if (t != null)
		'''       System.out.println(t);
		'''   }
		''' }}</pre>
		''' </summary>
		''' <param name="r"> the runnable that has completed </param>
		''' <param name="t"> the exception that caused termination, or null if
		''' execution completed normally </param>
		Protected Friend Overridable Sub afterExecute(ByVal r As Runnable, ByVal t As Throwable)
		End Sub

		''' <summary>
		''' Method invoked when the Executor has terminated.  Default
		''' implementation does nothing. Note: To properly nest multiple
		''' overridings, subclasses should generally invoke
		''' {@code super.terminated} within this method.
		''' </summary>
		Protected Friend Overridable Sub terminated()
		End Sub

		' Predefined RejectedExecutionHandlers 

		''' <summary>
		''' A handler for rejected tasks that runs the rejected task
		''' directly in the calling thread of the {@code execute} method,
		''' unless the executor has been shut down, in which case the task
		''' is discarded.
		''' </summary>
		Public Class CallerRunsPolicy
			Implements RejectedExecutionHandler

			''' <summary>
			''' Creates a {@code CallerRunsPolicy}.
			''' </summary>
			Public Sub New()
			End Sub

			''' <summary>
			''' Executes task r in the caller's thread, unless the executor
			''' has been shut down, in which case the task is discarded.
			''' </summary>
			''' <param name="r"> the runnable task requested to be executed </param>
			''' <param name="e"> the executor attempting to execute this task </param>
			Public Overridable Sub rejectedExecution(ByVal r As Runnable, ByVal e As ThreadPoolExecutor) Implements RejectedExecutionHandler.rejectedExecution
				If Not e.shutdown Then r.run()
			End Sub
		End Class

		''' <summary>
		''' A handler for rejected tasks that throws a
		''' {@code RejectedExecutionException}.
		''' </summary>
		Public Class AbortPolicy
			Implements RejectedExecutionHandler

			''' <summary>
			''' Creates an {@code AbortPolicy}.
			''' </summary>
			Public Sub New()
			End Sub

			''' <summary>
			''' Always throws RejectedExecutionException.
			''' </summary>
			''' <param name="r"> the runnable task requested to be executed </param>
			''' <param name="e"> the executor attempting to execute this task </param>
			''' <exception cref="RejectedExecutionException"> always </exception>
			Public Overridable Sub rejectedExecution(ByVal r As Runnable, ByVal e As ThreadPoolExecutor) Implements RejectedExecutionHandler.rejectedExecution
				Throw New RejectedExecutionException("Task " & r.ToString() & " rejected from " & e.ToString())
			End Sub
		End Class

		''' <summary>
		''' A handler for rejected tasks that silently discards the
		''' rejected task.
		''' </summary>
		Public Class DiscardPolicy
			Implements RejectedExecutionHandler

			''' <summary>
			''' Creates a {@code DiscardPolicy}.
			''' </summary>
			Public Sub New()
			End Sub

			''' <summary>
			''' Does nothing, which has the effect of discarding task r.
			''' </summary>
			''' <param name="r"> the runnable task requested to be executed </param>
			''' <param name="e"> the executor attempting to execute this task </param>
			Public Overridable Sub rejectedExecution(ByVal r As Runnable, ByVal e As ThreadPoolExecutor) Implements RejectedExecutionHandler.rejectedExecution
			End Sub
		End Class

		''' <summary>
		''' A handler for rejected tasks that discards the oldest unhandled
		''' request and then retries {@code execute}, unless the executor
		''' is shut down, in which case the task is discarded.
		''' </summary>
		Public Class DiscardOldestPolicy
			Implements RejectedExecutionHandler

			''' <summary>
			''' Creates a {@code DiscardOldestPolicy} for the given executor.
			''' </summary>
			Public Sub New()
			End Sub

			''' <summary>
			''' Obtains and ignores the next task that the executor
			''' would otherwise execute, if one is immediately available,
			''' and then retries execution of task r, unless the executor
			''' is shut down, in which case task r is instead discarded.
			''' </summary>
			''' <param name="r"> the runnable task requested to be executed </param>
			''' <param name="e"> the executor attempting to execute this task </param>
			Public Overridable Sub rejectedExecution(ByVal r As Runnable, ByVal e As ThreadPoolExecutor) Implements RejectedExecutionHandler.rejectedExecution
				If Not e.shutdown Then
					e.queue.poll()
					e.execute(r)
				End If
			End Sub
		End Class
	End Class

End Namespace