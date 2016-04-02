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
	''' Factory and utility methods for <seealso cref="Executor"/>, {@link
	''' ExecutorService}, <seealso cref="ScheduledExecutorService"/>, {@link
	''' ThreadFactory}, and <seealso cref="Callable"/> classes defined in this
	''' package. This class supports the following kinds of methods:
	''' 
	''' <ul>
	'''   <li> Methods that create and return an <seealso cref="ExecutorService"/>
	'''        set up with commonly useful configuration settings.
	'''   <li> Methods that create and return a <seealso cref="ScheduledExecutorService"/>
	'''        set up with commonly useful configuration settings.
	'''   <li> Methods that create and return a "wrapped" ExecutorService, that
	'''        disables reconfiguration by making implementation-specific methods
	'''        inaccessible.
	'''   <li> Methods that create and return a <seealso cref="ThreadFactory"/>
	'''        that sets newly created threads to a known state.
	'''   <li> Methods that create and return a <seealso cref="Callable"/>
	'''        out of other closure-like forms, so they can be used
	'''        in execution methods requiring {@code Callable}.
	''' </ul>
	''' 
	''' @since 1.5
	''' @author Doug Lea
	''' </summary>
	Public Class Executors

		''' <summary>
		''' Creates a thread pool that reuses a fixed number of threads
		''' operating off a shared unbounded queue.  At any point, at most
		''' {@code nThreads} threads will be active processing tasks.
		''' If additional tasks are submitted when all threads are active,
		''' they will wait in the queue until a thread is available.
		''' If any thread terminates due to a failure during execution
		''' prior to shutdown, a new one will take its place if needed to
		''' execute subsequent tasks.  The threads in the pool will exist
		''' until it is explicitly <seealso cref="ExecutorService#shutdown shutdown"/>.
		''' </summary>
		''' <param name="nThreads"> the number of threads in the pool </param>
		''' <returns> the newly created thread pool </returns>
		''' <exception cref="IllegalArgumentException"> if {@code nThreads <= 0} </exception>
		Public Shared Function newFixedThreadPool(ByVal nThreads As Integer) As ExecutorService
			Return New ThreadPoolExecutor(nThreads, nThreads, 0L, TimeUnit.MILLISECONDS, New LinkedBlockingQueue(Of Runnable))
		End Function

		''' <summary>
		''' Creates a thread pool that maintains enough threads to support
		''' the given parallelism level, and may use multiple queues to
		''' reduce contention. The parallelism level corresponds to the
		''' maximum number of threads actively engaged in, or available to
		''' engage in, task processing. The actual number of threads may
		''' grow and shrink dynamically. A work-stealing pool makes no
		''' guarantees about the order in which submitted tasks are
		''' executed.
		''' </summary>
		''' <param name="parallelism"> the targeted parallelism level </param>
		''' <returns> the newly created thread pool </returns>
		''' <exception cref="IllegalArgumentException"> if {@code parallelism <= 0}
		''' @since 1.8 </exception>
		Public Shared Function newWorkStealingPool(ByVal parallelism As Integer) As ExecutorService
			Return New ForkJoinPool(parallelism, ForkJoinPool.defaultForkJoinWorkerThreadFactory, Nothing, True)
		End Function

		''' <summary>
		''' Creates a work-stealing thread pool using all
		''' <seealso cref="Runtime#availableProcessors available processors"/>
		''' as its target parallelism level. </summary>
		''' <returns> the newly created thread pool </returns>
		''' <seealso cref= #newWorkStealingPool(int)
		''' @since 1.8 </seealso>
		Public Shared Function newWorkStealingPool() As ExecutorService
			Return New ForkJoinPool(Runtime.runtime.availableProcessors(), ForkJoinPool.defaultForkJoinWorkerThreadFactory, Nothing, True)
		End Function

		''' <summary>
		''' Creates a thread pool that reuses a fixed number of threads
		''' operating off a shared unbounded queue, using the provided
		''' ThreadFactory to create new threads when needed.  At any point,
		''' at most {@code nThreads} threads will be active processing
		''' tasks.  If additional tasks are submitted when all threads are
		''' active, they will wait in the queue until a thread is
		''' available.  If any thread terminates due to a failure during
		''' execution prior to shutdown, a new one will take its place if
		''' needed to execute subsequent tasks.  The threads in the pool will
		''' exist until it is explicitly {@link ExecutorService#shutdown
		''' shutdown}.
		''' </summary>
		''' <param name="nThreads"> the number of threads in the pool </param>
		''' <param name="threadFactory"> the factory to use when creating new threads </param>
		''' <returns> the newly created thread pool </returns>
		''' <exception cref="NullPointerException"> if threadFactory is null </exception>
		''' <exception cref="IllegalArgumentException"> if {@code nThreads <= 0} </exception>
		Public Shared Function newFixedThreadPool(ByVal nThreads As Integer, ByVal threadFactory As ThreadFactory) As ExecutorService
			Return New ThreadPoolExecutor(nThreads, nThreads, 0L, TimeUnit.MILLISECONDS, New LinkedBlockingQueue(Of Runnable), threadFactory)
		End Function

		''' <summary>
		''' Creates an Executor that uses a single worker thread operating
		''' off an unbounded queue. (Note however that if this single
		''' thread terminates due to a failure during execution prior to
		''' shutdown, a new one will take its place if needed to execute
		''' subsequent tasks.)  Tasks are guaranteed to execute
		''' sequentially, and no more than one task will be active at any
		''' given time. Unlike the otherwise equivalent
		''' {@code newFixedThreadPool(1)} the returned executor is
		''' guaranteed not to be reconfigurable to use additional threads.
		''' </summary>
		''' <returns> the newly created single-threaded Executor </returns>
		Public Shared Function newSingleThreadExecutor() As ExecutorService
			Return New FinalizableDelegatedExecutorService(New ThreadPoolExecutor(1, 1, 0L, TimeUnit.MILLISECONDS, New LinkedBlockingQueue(Of Runnable)))
		End Function

		''' <summary>
		''' Creates an Executor that uses a single worker thread operating
		''' off an unbounded queue, and uses the provided ThreadFactory to
		''' create a new thread when needed. Unlike the otherwise
		''' equivalent {@code newFixedThreadPool(1, threadFactory)} the
		''' returned executor is guaranteed not to be reconfigurable to use
		''' additional threads.
		''' </summary>
		''' <param name="threadFactory"> the factory to use when creating new
		''' threads
		''' </param>
		''' <returns> the newly created single-threaded Executor </returns>
		''' <exception cref="NullPointerException"> if threadFactory is null </exception>
		Public Shared Function newSingleThreadExecutor(ByVal threadFactory As ThreadFactory) As ExecutorService
			Return New FinalizableDelegatedExecutorService(New ThreadPoolExecutor(1, 1, 0L, TimeUnit.MILLISECONDS, New LinkedBlockingQueue(Of Runnable), threadFactory))
		End Function

		''' <summary>
		''' Creates a thread pool that creates new threads as needed, but
		''' will reuse previously constructed threads when they are
		''' available.  These pools will typically improve the performance
		''' of programs that execute many short-lived asynchronous tasks.
		''' Calls to {@code execute} will reuse previously constructed
		''' threads if available. If no existing thread is available, a new
		''' thread will be created and added to the pool. Threads that have
		''' not been used for sixty seconds are terminated and removed from
		''' the cache. Thus, a pool that remains idle for long enough will
		''' not consume any resources. Note that pools with similar
		''' properties but different details (for example, timeout parameters)
		''' may be created using <seealso cref="ThreadPoolExecutor"/> constructors.
		''' </summary>
		''' <returns> the newly created thread pool </returns>
		Public Shared Function newCachedThreadPool() As ExecutorService
			Return New ThreadPoolExecutor(0,  java.lang.[Integer].Max_Value, 60L, TimeUnit.SECONDS, New SynchronousQueue(Of Runnable))
		End Function

		''' <summary>
		''' Creates a thread pool that creates new threads as needed, but
		''' will reuse previously constructed threads when they are
		''' available, and uses the provided
		''' ThreadFactory to create new threads when needed. </summary>
		''' <param name="threadFactory"> the factory to use when creating new threads </param>
		''' <returns> the newly created thread pool </returns>
		''' <exception cref="NullPointerException"> if threadFactory is null </exception>
		Public Shared Function newCachedThreadPool(ByVal threadFactory As ThreadFactory) As ExecutorService
			Return New ThreadPoolExecutor(0,  java.lang.[Integer].Max_Value, 60L, TimeUnit.SECONDS, New SynchronousQueue(Of Runnable), threadFactory)
		End Function

		''' <summary>
		''' Creates a single-threaded executor that can schedule commands
		''' to run after a given delay, or to execute periodically.
		''' (Note however that if this single
		''' thread terminates due to a failure during execution prior to
		''' shutdown, a new one will take its place if needed to execute
		''' subsequent tasks.)  Tasks are guaranteed to execute
		''' sequentially, and no more than one task will be active at any
		''' given time. Unlike the otherwise equivalent
		''' {@code newScheduledThreadPool(1)} the returned executor is
		''' guaranteed not to be reconfigurable to use additional threads. </summary>
		''' <returns> the newly created scheduled executor </returns>
		Public Shared Function newSingleThreadScheduledExecutor() As ScheduledExecutorService
			Return New DelegatedScheduledExecutorService(New ScheduledThreadPoolExecutor(1))
		End Function

		''' <summary>
		''' Creates a single-threaded executor that can schedule commands
		''' to run after a given delay, or to execute periodically.  (Note
		''' however that if this single thread terminates due to a failure
		''' during execution prior to shutdown, a new one will take its
		''' place if needed to execute subsequent tasks.)  Tasks are
		''' guaranteed to execute sequentially, and no more than one task
		''' will be active at any given time. Unlike the otherwise
		''' equivalent {@code newScheduledThreadPool(1, threadFactory)}
		''' the returned executor is guaranteed not to be reconfigurable to
		''' use additional threads. </summary>
		''' <param name="threadFactory"> the factory to use when creating new
		''' threads </param>
		''' <returns> a newly created scheduled executor </returns>
		''' <exception cref="NullPointerException"> if threadFactory is null </exception>
		Public Shared Function newSingleThreadScheduledExecutor(ByVal threadFactory As ThreadFactory) As ScheduledExecutorService
			Return New DelegatedScheduledExecutorService(New ScheduledThreadPoolExecutor(1, threadFactory))
		End Function

		''' <summary>
		''' Creates a thread pool that can schedule commands to run after a
		''' given delay, or to execute periodically. </summary>
		''' <param name="corePoolSize"> the number of threads to keep in the pool,
		''' even if they are idle </param>
		''' <returns> a newly created scheduled thread pool </returns>
		''' <exception cref="IllegalArgumentException"> if {@code corePoolSize < 0} </exception>
		Public Shared Function newScheduledThreadPool(ByVal corePoolSize As Integer) As ScheduledExecutorService
			Return New ScheduledThreadPoolExecutor(corePoolSize)
		End Function

		''' <summary>
		''' Creates a thread pool that can schedule commands to run after a
		''' given delay, or to execute periodically. </summary>
		''' <param name="corePoolSize"> the number of threads to keep in the pool,
		''' even if they are idle </param>
		''' <param name="threadFactory"> the factory to use when the executor
		''' creates a new thread </param>
		''' <returns> a newly created scheduled thread pool </returns>
		''' <exception cref="IllegalArgumentException"> if {@code corePoolSize < 0} </exception>
		''' <exception cref="NullPointerException"> if threadFactory is null </exception>
		Public Shared Function newScheduledThreadPool(ByVal corePoolSize As Integer, ByVal threadFactory As ThreadFactory) As ScheduledExecutorService
			Return New ScheduledThreadPoolExecutor(corePoolSize, threadFactory)
		End Function

		''' <summary>
		''' Returns an object that delegates all defined {@link
		''' ExecutorService} methods to the given executor, but not any
		''' other methods that might otherwise be accessible using
		''' casts. This provides a way to safely "freeze" configuration and
		''' disallow tuning of a given concrete implementation. </summary>
		''' <param name="executor"> the underlying implementation </param>
		''' <returns> an {@code ExecutorService} instance </returns>
		''' <exception cref="NullPointerException"> if executor null </exception>
		Public Shared Function unconfigurableExecutorService(ByVal executor As ExecutorService) As ExecutorService
			If executor Is Nothing Then Throw New NullPointerException
			Return New DelegatedExecutorService(executor)
		End Function

		''' <summary>
		''' Returns an object that delegates all defined {@link
		''' ScheduledExecutorService} methods to the given executor, but
		''' not any other methods that might otherwise be accessible using
		''' casts. This provides a way to safely "freeze" configuration and
		''' disallow tuning of a given concrete implementation. </summary>
		''' <param name="executor"> the underlying implementation </param>
		''' <returns> a {@code ScheduledExecutorService} instance </returns>
		''' <exception cref="NullPointerException"> if executor null </exception>
		Public Shared Function unconfigurableScheduledExecutorService(ByVal executor As ScheduledExecutorService) As ScheduledExecutorService
			If executor Is Nothing Then Throw New NullPointerException
			Return New DelegatedScheduledExecutorService(executor)
		End Function

		''' <summary>
		''' Returns a default thread factory used to create new threads.
		''' This factory creates all new threads used by an Executor in the
		''' same <seealso cref="ThreadGroup"/>. If there is a {@link
		''' java.lang.SecurityManager}, it uses the group of {@link
		''' System#getSecurityManager}, else the group of the thread
		''' invoking this {@code defaultThreadFactory} method. Each new
		''' thread is created as a non-daemon thread with priority set to
		''' the smaller of {@code Thread.NORM_PRIORITY} and the maximum
		''' priority permitted in the thread group.  New threads have names
		''' accessible via <seealso cref="Thread#getName"/> of
		''' <em>pool-N-thread-M</em>, where <em>N</em> is the sequence
		''' number of this factory, and <em>M</em> is the sequence number
		''' of the thread created by this factory. </summary>
		''' <returns> a thread factory </returns>
		Public Shared Function defaultThreadFactory() As ThreadFactory
			Return New DefaultThreadFactory
		End Function

		''' <summary>
		''' Returns a thread factory used to create new threads that
		''' have the same permissions as the current thread.
		''' This factory creates threads with the same settings as {@link
		''' Executors#defaultThreadFactory}, additionally setting the
		''' AccessControlContext and contextClassLoader of new threads to
		''' be the same as the thread invoking this
		''' {@code privilegedThreadFactory} method.  A new
		''' {@code privilegedThreadFactory} can be created within an
		''' <seealso cref="AccessController#doPrivileged AccessController.doPrivileged"/>
		''' action setting the current thread's access control context to
		''' create threads with the selected permission settings holding
		''' within that action.
		''' 
		''' <p>Note that while tasks running within such threads will have
		''' the same access control and class loader settings as the
		''' current thread, they need not have the same {@link
		''' java.lang.ThreadLocal} or {@link
		''' java.lang.InheritableThreadLocal} values. If necessary,
		''' particular values of thread locals can be set or reset before
		''' any task runs in <seealso cref="ThreadPoolExecutor"/> subclasses using
		''' <seealso cref="ThreadPoolExecutor#beforeExecute(Thread, Runnable)"/>.
		''' Also, if it is necessary to initialize worker threads to have
		''' the same InheritableThreadLocal settings as some other
		''' designated thread, you can create a custom ThreadFactory in
		''' which that thread waits for and services requests to create
		''' others that will inherit its values.
		''' </summary>
		''' <returns> a thread factory </returns>
		''' <exception cref="AccessControlException"> if the current access control
		''' context does not have permission to both get and set context
		''' class loader </exception>
		Public Shared Function privilegedThreadFactory() As ThreadFactory
			Return New PrivilegedThreadFactory
		End Function

		''' <summary>
		''' Returns a <seealso cref="Callable"/> object that, when
		''' called, runs the given task and returns the given result.  This
		''' can be useful when applying methods requiring a
		''' {@code Callable} to an otherwise resultless action. </summary>
		''' <param name="task"> the task to run </param>
		''' <param name="result"> the result to return </param>
		''' @param <T> the type of the result </param>
		''' <returns> a callable object </returns>
		''' <exception cref="NullPointerException"> if task null </exception>
		Public Shared Function callable(Of T)(ByVal task As Runnable, ByVal result As T) As Callable(Of T)
			If task Is Nothing Then Throw New NullPointerException
			Return New RunnableAdapter(Of T)(task, result)
		End Function

		''' <summary>
		''' Returns a <seealso cref="Callable"/> object that, when
		''' called, runs the given task and returns {@code null}. </summary>
		''' <param name="task"> the task to run </param>
		''' <returns> a callable object </returns>
		''' <exception cref="NullPointerException"> if task null </exception>
		Public Shared Function callable(ByVal task As Runnable) As Callable(Of Object)
			If task Is Nothing Then Throw New NullPointerException
			Return New RunnableAdapter(Of Object)(task, Nothing)
		End Function

		''' <summary>
		''' Returns a <seealso cref="Callable"/> object that, when
		''' called, runs the given privileged action and returns its result. </summary>
		''' <param name="action"> the privileged action to run </param>
		''' <returns> a callable object </returns>
		''' <exception cref="NullPointerException"> if action null </exception>
		Public Shared Function callable(Of T1)(ByVal action As java.security.PrivilegedAction(Of T1)) As Callable(Of Object)
			If action Is Nothing Then Throw New NullPointerException
			Return New CallableAnonymousInnerClassHelper(Of V)
		End Function

		Private Class CallableAnonymousInnerClassHelper(Of V)
			Implements Callable(Of V)

			Public Overridable Function [call]() As Object
				Return action.run()
			End Function
		End Class

		''' <summary>
		''' Returns a <seealso cref="Callable"/> object that, when
		''' called, runs the given privileged exception action and returns
		''' its result. </summary>
		''' <param name="action"> the privileged exception action to run </param>
		''' <returns> a callable object </returns>
		''' <exception cref="NullPointerException"> if action null </exception>
		Public Shared Function callable(Of T1)(ByVal action As java.security.PrivilegedExceptionAction(Of T1)) As Callable(Of Object)
			If action Is Nothing Then Throw New NullPointerException
			Return New CallableAnonymousInnerClassHelper2(Of V)
		End Function

		Private Class CallableAnonymousInnerClassHelper2(Of V)
			Implements Callable(Of V)

			Public Overridable Function [call]() As Object
				Return action.run()
			End Function
		End Class

		''' <summary>
		''' Returns a <seealso cref="Callable"/> object that will, when called,
		''' execute the given {@code callable} under the current access
		''' control context. This method should normally be invoked within
		''' an <seealso cref="AccessController#doPrivileged AccessController.doPrivileged"/>
		''' action to create callables that will, if possible, execute
		''' under the selected permission settings holding within that
		''' action; or if not possible, throw an associated {@link
		''' AccessControlException}. </summary>
		''' <param name="callable"> the underlying task </param>
		''' @param <T> the type of the callable's result </param>
		''' <returns> a callable object </returns>
		''' <exception cref="NullPointerException"> if callable null </exception>
		Public Shared Function privilegedCallable(Of T)(ByVal callable As Callable(Of T)) As Callable(Of T)
			If callable Is Nothing Then Throw New NullPointerException
			Return New PrivilegedCallable(Of T)(callable)
		End Function

		''' <summary>
		''' Returns a <seealso cref="Callable"/> object that will, when called,
		''' execute the given {@code callable} under the current access
		''' control context, with the current context class loader as the
		''' context class loader. This method should normally be invoked
		''' within an
		''' <seealso cref="AccessController#doPrivileged AccessController.doPrivileged"/>
		''' action to create callables that will, if possible, execute
		''' under the selected permission settings holding within that
		''' action; or if not possible, throw an associated {@link
		''' AccessControlException}.
		''' </summary>
		''' <param name="callable"> the underlying task </param>
		''' @param <T> the type of the callable's result </param>
		''' <returns> a callable object </returns>
		''' <exception cref="NullPointerException"> if callable null </exception>
		''' <exception cref="AccessControlException"> if the current access control
		''' context does not have permission to both set and get context
		''' class loader </exception>
		Public Shared Function privilegedCallableUsingCurrentClassLoader(Of T)(ByVal callable As Callable(Of T)) As Callable(Of T)
			If callable Is Nothing Then Throw New NullPointerException
			Return New PrivilegedCallableUsingCurrentClassLoader(Of T)(callable)
		End Function

		' Non-public classes supporting the public methods

		''' <summary>
		''' A callable that runs given task and returns given result
		''' </summary>
		Friend NotInheritable Class RunnableAdapter(Of T)
			Implements Callable(Of T)

			Friend ReadOnly task As Runnable
			Friend ReadOnly result As T
			Friend Sub New(ByVal task As Runnable, ByVal result As T)
				Me.task = task
				Me.result = result
			End Sub
			Public Function [call]() As T
				task.run()
				Return result
			End Function
		End Class

		''' <summary>
		''' A callable that runs under established access control settings
		''' </summary>
		Friend NotInheritable Class PrivilegedCallable(Of T)
			Implements Callable(Of T)

			Private ReadOnly task As Callable(Of T)
			Private ReadOnly acc As java.security.AccessControlContext

			Friend Sub New(ByVal task As Callable(Of T))
				Me.task = task
				Me.acc = java.security.AccessController.context
			End Sub

			Public Function [call]() As T
				Try
					Return java.security.AccessController.doPrivileged(New PrivilegedExceptionActionAnonymousInnerClassHelper(Of T)
				Catch e As java.security.PrivilegedActionException
					Throw e.exception
				End Try
			End Function

			Private Class PrivilegedExceptionActionAnonymousInnerClassHelper(Of T)
				Implements java.security.PrivilegedExceptionAction(Of T)

				Public Overridable Function run() As T
					Return outerInstance.task.call()
				End Function
			End Class
		End Class

		''' <summary>
		''' A callable that runs under established access control settings and
		''' current ClassLoader
		''' </summary>
		Friend NotInheritable Class PrivilegedCallableUsingCurrentClassLoader(Of T)
			Implements Callable(Of T)

			Private ReadOnly task As Callable(Of T)
			Private ReadOnly acc As java.security.AccessControlContext
			Private ReadOnly ccl As  ClassLoader

			Friend Sub New(ByVal task As Callable(Of T))
				Dim sm As SecurityManager = System.securityManager
				If sm IsNot Nothing Then
					' Calls to getContextClassLoader from this class
					' never trigger a security check, but we check
					' whether our callers have this permission anyways.
					sm.checkPermission(sun.security.util.SecurityConstants.GET_CLASSLOADER_PERMISSION)

					' Whether setContextClassLoader turns out to be necessary
					' or not, we fail fast if permission is not available.
					sm.checkPermission(New RuntimePermission("setContextClassLoader"))
				End If
				Me.task = task
				Me.acc = java.security.AccessController.context
				Me.ccl = Thread.CurrentThread.contextClassLoader
			End Sub

			Public Function [call]() As T
				Try
					Return java.security.AccessController.doPrivileged(New PrivilegedExceptionActionAnonymousInnerClassHelper(Of T)
				Catch e As java.security.PrivilegedActionException
					Throw e.exception
				End Try
			End Function

			Private Class PrivilegedExceptionActionAnonymousInnerClassHelper(Of T)
				Implements java.security.PrivilegedExceptionAction(Of T)

				Public Overridable Function run() As T
					Dim t As Thread = Thread.CurrentThread
					Dim cl As  ClassLoader = t.contextClassLoader
					If outerInstance.ccl Is cl Then
						Return outerInstance.task.call()
					Else
						t.contextClassLoader = outerInstance.ccl
						Try
							Return outerInstance.task.call()
						Finally
							t.contextClassLoader = cl
						End Try
					End If
				End Function
			End Class
		End Class

		''' <summary>
		''' The default thread factory
		''' </summary>
		Friend Class DefaultThreadFactory
			Implements ThreadFactory

			Private Shared ReadOnly poolNumber As New java.util.concurrent.atomic.AtomicInteger(1)
			Private ReadOnly group As ThreadGroup
			Private ReadOnly threadNumber As New java.util.concurrent.atomic.AtomicInteger(1)
			Private ReadOnly namePrefix As String

			Friend Sub New()
				Dim s As SecurityManager = System.securityManager
				group = If(s IsNot Nothing, s.threadGroup, Thread.CurrentThread.threadGroup)
				namePrefix = "pool-" & poolNumber.andIncrement & "-thread-"
			End Sub

			Public Overridable Function newThread(ByVal r As Runnable) As Thread Implements ThreadFactory.newThread
				Dim t As New Thread(group, r, namePrefix + threadNumber.andIncrement, 0)
				If t.daemon Then t.daemon = False
				If t.priority <> Thread.NORM_PRIORITY Then t.priority = Thread.NORM_PRIORITY
				Return t
			End Function
		End Class

		''' <summary>
		''' Thread factory capturing access control context and class loader
		''' </summary>
		Friend Class PrivilegedThreadFactory
			Inherits DefaultThreadFactory

			Private ReadOnly acc As java.security.AccessControlContext
			Private ReadOnly ccl As  ClassLoader

			Friend Sub New()
				MyBase.New()
				Dim sm As SecurityManager = System.securityManager
				If sm IsNot Nothing Then
					' Calls to getContextClassLoader from this class
					' never trigger a security check, but we check
					' whether our callers have this permission anyways.
					sm.checkPermission(sun.security.util.SecurityConstants.GET_CLASSLOADER_PERMISSION)

					' Fail fast
					sm.checkPermission(New RuntimePermission("setContextClassLoader"))
				End If
				Me.acc = java.security.AccessController.context
				Me.ccl = Thread.CurrentThread.contextClassLoader
			End Sub

			Public Overrides Function newThread(ByVal r As Runnable) As Thread
				Return MyBase.newThread(New RunnableAnonymousInnerClassHelper
			End Function

			Private Class RunnableAnonymousInnerClassHelper
				Implements Runnable

				Public Overridable Sub run() Implements Runnable.run
					java.security.AccessController.doPrivileged(New PrivilegedActionAnonymousInnerClassHelper(Of T)
				End Sub

				Private Class PrivilegedActionAnonymousInnerClassHelper(Of T)
					Implements java.security.PrivilegedAction(Of T)

					Public Overridable Function run() As Void
						Thread.CurrentThread.contextClassLoader = ccl
						r.run()
						Return Nothing
					End Function
				End Class
			End Class
		End Class

		''' <summary>
		''' A wrapper class that exposes only the ExecutorService methods
		''' of an ExecutorService implementation.
		''' </summary>
		Friend Class DelegatedExecutorService
			Inherits AbstractExecutorService

			Private ReadOnly e As ExecutorService
			Friend Sub New(ByVal executor As ExecutorService)
				e = executor
			End Sub
			Public Overridable Sub execute(ByVal command As Runnable)
				e.execute(command)
			End Sub
			Public Overrides Sub shutdown()
				e.shutdown()
			End Sub
			Public Overrides Function shutdownNow() As List(Of Runnable)
				Return e.shutdownNow()
			End Function
			Public  Overrides ReadOnly Property  shutdown As Boolean
				Get
					Return e.shutdown
				End Get
			End Property
			Public  Overrides ReadOnly Property  terminated As Boolean
				Get
					Return e.terminated
				End Get
			End Property
			Public Overrides Function awaitTermination(ByVal timeout As Long, ByVal unit As TimeUnit) As Boolean
				Return e.awaitTermination(timeout, unit)
			End Function
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
			Public Overrides Function submit(ByVal task As Runnable) As Future(Of ?)
				Return e.submit(task)
			End Function
			Public Overrides Function submit(Of T)(ByVal task As Callable(Of T)) As Future(Of T)
				Return e.submit(task)
			End Function
			Public Overrides Function submit(Of T)(ByVal task As Runnable, ByVal result As T) As Future(Of T)
				Return e.submit(task, result)
			End Function
			Public Overrides Function invokeAll(Of T, T1 As Callable(Of T)(ByVal tasks As Collection(Of T1)) As List(Of Future(Of T))
				Return e.invokeAll(tasks)
			End Function
			Public Overrides Function invokeAll(Of T, T1 As Callable(Of T)(ByVal tasks As Collection(Of T1), ByVal timeout As Long, ByVal unit As TimeUnit) As List(Of Future(Of T))
				Return e.invokeAll(tasks, timeout, unit)
			End Function
			Public Overrides Function invokeAny(Of T, T1 As Callable(Of T)(ByVal tasks As Collection(Of T1)) As T
				Return e.invokeAny(tasks)
			End Function
			Public Overrides Function invokeAny(Of T, T1 As Callable(Of T)(ByVal tasks As Collection(Of T1), ByVal timeout As Long, ByVal unit As TimeUnit) As T
				Return e.invokeAny(tasks, timeout, unit)
			End Function
		End Class

		Friend Class FinalizableDelegatedExecutorService
			Inherits DelegatedExecutorService

			Friend Sub New(ByVal executor As ExecutorService)
				MyBase.New(executor)
			End Sub
			Protected Overrides Sub Finalize()
				MyBase.shutdown()
			End Sub
		End Class

		''' <summary>
		''' A wrapper class that exposes only the ScheduledExecutorService
		''' methods of a ScheduledExecutorService implementation.
		''' </summary>
		Friend Class DelegatedScheduledExecutorService
			Inherits DelegatedExecutorService
			Implements ScheduledExecutorService

			Private ReadOnly e As ScheduledExecutorService
			Friend Sub New(ByVal executor As ScheduledExecutorService)
				MyBase.New(executor)
				e = executor
			End Sub
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
			Public Overridable Function schedule(ByVal command As Runnable, ByVal delay As Long, ByVal unit As TimeUnit) As ScheduledFuture(Of ?) Implements ScheduledExecutorService.schedule
				Return e.schedule(command, delay, unit)
			End Function
			Public Overridable Function schedule(Of V)(ByVal callable As Callable(Of V), ByVal delay As Long, ByVal unit As TimeUnit) As ScheduledFuture(Of V) Implements ScheduledExecutorService.schedule
				Return e.schedule(callable, delay, unit)
			End Function
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
			Public Overridable Function scheduleAtFixedRate(ByVal command As Runnable, ByVal initialDelay As Long, ByVal period As Long, ByVal unit As TimeUnit) As ScheduledFuture(Of ?) Implements ScheduledExecutorService.scheduleAtFixedRate
				Return e.scheduleAtFixedRate(command, initialDelay, period, unit)
			End Function
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
			Public Overridable Function scheduleWithFixedDelay(ByVal command As Runnable, ByVal initialDelay As Long, ByVal delay As Long, ByVal unit As TimeUnit) As ScheduledFuture(Of ?) Implements ScheduledExecutorService.scheduleWithFixedDelay
				Return e.scheduleWithFixedDelay(command, initialDelay, delay, unit)
			End Function
		End Class

		''' <summary>
		''' Cannot instantiate. </summary>
		Private Sub New()
		End Sub
	End Class

End Namespace