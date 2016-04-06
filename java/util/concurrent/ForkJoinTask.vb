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
	''' Abstract base class for tasks that run within a <seealso cref="ForkJoinPool"/>.
	''' A {@code ForkJoinTask} is a thread-like entity that is much
	''' lighter weight than a normal thread.  Huge numbers of tasks and
	''' subtasks may be hosted by a small number of actual threads in a
	''' ForkJoinPool, at the price of some usage limitations.
	''' 
	''' <p>A "main" {@code ForkJoinTask} begins execution when it is
	''' explicitly submitted to a <seealso cref="ForkJoinPool"/>, or, if not already
	''' engaged in a ForkJoin computation, commenced in the {@link
	''' ForkJoinPool#commonPool()} via <seealso cref="#fork"/>, <seealso cref="#invoke"/>, or
	''' related methods.  Once started, it will usually in turn start other
	''' subtasks.  As indicated by the name of this [Class], many programs
	''' using {@code ForkJoinTask} employ only methods <seealso cref="#fork"/> and
	''' <seealso cref="#join"/>, or derivatives such as {@link
	''' #invokeAll(ForkJoinTask...) invokeAll}.  However, this class also
	''' provides a number of other methods that can come into play in
	''' advanced usages, as well as extension mechanics that allow support
	''' of new forms of fork/join processing.
	''' 
	''' <p>A {@code ForkJoinTask} is a lightweight form of <seealso cref="Future"/>.
	''' The efficiency of {@code ForkJoinTask}s stems from a set of
	''' restrictions (that are only partially statically enforceable)
	''' reflecting their main use as computational tasks calculating pure
	''' functions or operating on purely isolated objects.  The primary
	''' coordination mechanisms are <seealso cref="#fork"/>, that arranges
	''' asynchronous execution, and <seealso cref="#join"/>, that doesn't proceed
	''' until the task's result has been computed.  Computations should
	''' ideally avoid {@code synchronized} methods or blocks, and should
	''' minimize other blocking synchronization apart from joining other
	''' tasks or using synchronizers such as Phasers that are advertised to
	''' cooperate with fork/join scheduling. Subdividable tasks should also
	''' not perform blocking I/O, and should ideally access variables that
	''' are completely independent of those accessed by other running
	''' tasks. These guidelines are loosely enforced by not permitting
	''' checked exceptions such as {@code IOExceptions} to be
	''' thrown. However, computations may still encounter unchecked
	''' exceptions, that are rethrown to callers attempting to join
	''' them. These exceptions may additionally include {@link
	''' RejectedExecutionException} stemming from internal resource
	''' exhaustion, such as failure to allocate internal task
	''' queues. Rethrown exceptions behave in the same way as regular
	''' exceptions, but, when possible, contain stack traces (as displayed
	''' for example using {@code ex.printStackTrace()}) of both the thread
	''' that initiated the computation as well as the thread actually
	''' encountering the exception; minimally only the latter.
	''' 
	''' <p>It is possible to define and use ForkJoinTasks that may block,
	''' but doing do requires three further considerations: (1) Completion
	''' of few if any <em>other</em> tasks should be dependent on a task
	''' that blocks on external synchronization or I/O. Event-style async
	''' tasks that are never joined (for example, those subclassing {@link
	''' CountedCompleter}) often fall into this category.  (2) To minimize
	''' resource impact, tasks should be small; ideally performing only the
	''' (possibly) blocking action. (3) Unless the {@link
	''' ForkJoinPool.ManagedBlocker} API is used, or the number of possibly
	''' blocked tasks is known to be less than the pool's {@link
	''' ForkJoinPool#getParallelism} level, the pool cannot guarantee that
	''' enough threads will be available to ensure progress or good
	''' performance.
	''' 
	''' <p>The primary method for awaiting completion and extracting
	''' results of a task is <seealso cref="#join"/>, but there are several variants:
	''' The <seealso cref="Future#get"/> methods support interruptible and/or timed
	''' waits for completion and report results using {@code Future}
	''' conventions. Method <seealso cref="#invoke"/> is semantically
	''' equivalent to {@code fork(); join()} but always attempts to begin
	''' execution in the current thread. The "<em>quiet</em>" forms of
	''' these methods do not extract results or report exceptions. These
	''' may be useful when a set of tasks are being executed, and you need
	''' to delay processing of results or exceptions until all complete.
	''' Method {@code invokeAll} (available in multiple versions)
	''' performs the most common form of parallel invocation: forking a set
	''' of tasks and joining them all.
	''' 
	''' <p>In the most typical usages, a fork-join pair act like a call
	''' (fork) and return (join) from a parallel recursive function. As is
	''' the case with other forms of recursive calls, returns (joins)
	''' should be performed innermost-first. For example, {@code a.fork();
	''' b.fork(); b.join(); a.join();} is likely to be substantially more
	''' efficient than joining {@code a} before {@code b}.
	''' 
	''' <p>The execution status of tasks may be queried at several levels
	''' of detail: <seealso cref="#isDone"/> is true if a task completed in any way
	''' (including the case where a task was cancelled without executing);
	''' <seealso cref="#isCompletedNormally"/> is true if a task completed without
	''' cancellation or encountering an exception; <seealso cref="#isCancelled"/> is
	''' true if the task was cancelled (in which case <seealso cref="#getException"/>
	''' returns a <seealso cref="java.util.concurrent.CancellationException"/>); and
	''' <seealso cref="#isCompletedAbnormally"/> is true if a task was either
	''' cancelled or encountered an exception, in which case {@link
	''' #getException} will return either the encountered exception or
	''' <seealso cref="java.util.concurrent.CancellationException"/>.
	''' 
	''' <p>The ForkJoinTask class is not usually directly subclassed.
	''' Instead, you subclass one of the abstract classes that support a
	''' particular style of fork/join processing, typically {@link
	''' RecursiveAction} for most computations that do not return results,
	''' <seealso cref="RecursiveTask"/> for those that do, and {@link
	''' CountedCompleter} for those in which completed actions trigger
	''' other actions.  Normally, a concrete ForkJoinTask subclass declares
	''' fields comprising its parameters, established in a constructor, and
	''' then defines a {@code compute} method that somehow uses the control
	''' methods supplied by this base class.
	''' 
	''' <p>Method <seealso cref="#join"/> and its variants are appropriate for use
	''' only when completion dependencies are acyclic; that is, the
	''' parallel computation can be described as a directed acyclic graph
	''' (DAG). Otherwise, executions may encounter a form of deadlock as
	''' tasks cyclically wait for each other.  However, this framework
	''' supports other methods and techniques (for example the use of
	''' <seealso cref="Phaser"/>, <seealso cref="#helpQuiesce"/>, and <seealso cref="#complete"/>) that
	''' may be of use in constructing custom subclasses for problems that
	''' are not statically structured as DAGs. To support such usages, a
	''' ForkJoinTask may be atomically <em>tagged</em> with a {@code short}
	''' value using <seealso cref="#setForkJoinTaskTag"/> or {@link
	''' #compareAndSetForkJoinTaskTag} and checked using {@link
	''' #getForkJoinTaskTag}. The ForkJoinTask implementation does not use
	''' these {@code protected} methods or tags for any purpose, but they
	''' may be of use in the construction of specialized subclasses.  For
	''' example, parallel graph traversals can use the supplied methods to
	''' avoid revisiting nodes/tasks that have already been processed.
	''' (Method names for tagging are bulky in part to encourage definition
	''' of methods that reflect their usage patterns.)
	''' 
	''' <p>Most base support methods are {@code final}, to prevent
	''' overriding of implementations that are intrinsically tied to the
	''' underlying lightweight task scheduling framework.  Developers
	''' creating new basic styles of fork/join processing should minimally
	''' implement {@code protected} methods <seealso cref="#exec"/>, {@link
	''' #setRawResult}, and <seealso cref="#getRawResult"/>, while also introducing
	''' an abstract computational method that can be implemented in its
	''' subclasses, possibly relying on other {@code protected} methods
	''' provided by this class.
	''' 
	''' <p>ForkJoinTasks should perform relatively small amounts of
	''' computation. Large tasks should be split into smaller subtasks,
	''' usually via recursive decomposition. As a very rough rule of thumb,
	''' a task should perform more than 100 and less than 10000 basic
	''' computational steps, and should avoid indefinite looping. If tasks
	''' are too big, then parallelism cannot improve throughput. If too
	''' small, then memory and internal task maintenance overhead may
	''' overwhelm processing.
	''' 
	''' <p>This class provides {@code adapt} methods for <seealso cref="Runnable"/>
	''' and <seealso cref="Callable"/>, that may be of use when mixing execution of
	''' {@code ForkJoinTasks} with other kinds of tasks. When all tasks are
	''' of this form, consider using a pool constructed in <em>asyncMode</em>.
	''' 
	''' <p>ForkJoinTasks are {@code Serializable}, which enables them to be
	''' used in extensions such as remote execution frameworks. It is
	''' sensible to serialize tasks only before or after, but not during,
	''' execution. Serialization is not relied on during execution itself.
	''' 
	''' @since 1.7
	''' @author Doug Lea
	''' </summary>
	<Serializable> _
	Public MustInherit Class ForkJoinTask(Of V)
		Implements java.util.concurrent.Future(Of V)

	'    
	'     * See the internal documentation of class ForkJoinPool for a
	'     * general implementation overview.  ForkJoinTasks are mainly
	'     * responsible for maintaining their "status" field amidst relays
	'     * to methods in ForkJoinWorkerThread and ForkJoinPool.
	'     *
	'     * The methods of this class are more-or-less layered into
	'     * (1) basic status maintenance
	'     * (2) execution and awaiting completion
	'     * (3) user-level methods that additionally report results.
	'     * This is sometimes hard to see because this file orders exported
	'     * methods in a way that flows well in javadocs.
	'     

	'    
	'     * The status field holds run control status bits packed into a
	'     * single int to minimize footprint and to ensure atomicity (via
	'     * CAS).  Status is initially zero, and takes on nonnegative
	'     * values until completed, upon which status (anded with
	'     * DONE_MASK) holds value NORMAL, CANCELLED, or EXCEPTIONAL. Tasks
	'     * undergoing blocking waits by other threads have the SIGNAL bit
	'     * set.  Completion of a stolen task with SIGNAL set awakens any
	'     * waiters via notifyAll. Even though suboptimal for some
	'     * purposes, we use basic builtin wait/notify to take advantage of
	'     * "monitor inflation" in JVMs that we would otherwise need to
	'     * emulate to avoid adding further per-task bookkeeping overhead.
	'     * We want these monitors to be "fat", i.e., not use biasing or
	'     * thin-lock techniques, so use some odd coding idioms that tend
	'     * to avoid them, mainly by arranging that every synchronized
	'     * block performs a wait, notifyAll or both.
	'     *
	'     * These control bits occupy only (some of) the upper half (16
	'     * bits) of status field. The lower bits are used for user-defined
	'     * tags.
	'     

		''' <summary>
		''' The run status of this task </summary>
'JAVA TO VB CONVERTER TODO TASK: There is no VB equivalent to 'volatile':
		Friend status As Integer ' accessed directly by pool and workers
		Friend Const DONE_MASK As Integer = &Hf0000000L ' mask out non-completion bits
		Friend Const NORMAL As Integer = &Hf0000000L ' must be negative
		Friend Const CANCELLED As Integer = &Hc0000000L ' must be < NORMAL
		Friend Const EXCEPTIONAL As Integer = &H80000000L ' must be < CANCELLED
		Friend Const SIGNAL As Integer = &H10000 ' must be >= 1 << 16
		Friend Const SMASK As Integer = &Hffff ' short bits for tags

		''' <summary>
		''' Marks completion and wakes up threads waiting to join this
		''' task.
		''' </summary>
		''' <param name="completion"> one of NORMAL, CANCELLED, EXCEPTIONAL </param>
		''' <returns> completion status on exit </returns>
		Private Function setCompletion(  completion As Integer) As Integer
			Dim s As Integer
			Do
				s = status
				If s < 0 Then Return s
				If U.compareAndSwapInt(Me, STATUS, s, s Or completion) Then
					If (CInt(CUInt(s) >> 16)) <> 0 Then
						SyncLock Me
							notifyAll()
						End SyncLock
					End If
					Return completion
				End If
			Loop
		End Function

		''' <summary>
		''' Primary execution method for stolen tasks. Unless done, calls
		''' exec and records status if completed, but doesn't wait for
		''' completion otherwise.
		''' </summary>
		''' <returns> status on exit from this method </returns>
		Friend Function doExec() As Integer
			Dim s As Integer
			Dim completed As Boolean
			s = status
			If s >= 0 Then
				Try
					completed = exec()
				Catch rex As Throwable
					Return exceptionalCompletionion(rex)
				End Try
				If completed Then s = completionion(NORMAL)
			End If
			Return s
		End Function

		''' <summary>
		''' If not done, sets SIGNAL status and performs Object.wait(timeout).
		''' This task may or may not be done on exit. Ignores interrupts.
		''' </summary>
		''' <param name="timeout"> using Object.wait conventions. </param>
		Friend Sub internalWait(  timeout As Long)
			Dim s As Integer
			s = status
			If s >= 0 AndAlso U.compareAndSwapInt(Me, STATUS, s, s Or SIGNAL) Then ' force completer to issue notify
				SyncLock Me
					If status >= 0 Then
						Try
							wait(timeout)
						Catch ie As InterruptedException
						End Try
					Else
						notifyAll()
					End If
				End SyncLock
			End If
		End Sub

		''' <summary>
		''' Blocks a non-worker-thread until completion. </summary>
		''' <returns> status upon completion </returns>
		Private Function externalAwaitDone() As Integer
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
			Dim s As Integer = (If(TypeOf Me Is CountedCompleter, ForkJoinPool.common.externalHelpComplete(CType(Me, CountedCompleter(Of ?)), 0), If(ForkJoinPool.common.tryExternalUnpush(Me), doExec(), 0))) ' try helping
			s = status
			If s >= 0 AndAlso s >= 0 Then
				Dim interrupted As Boolean = False
				Do
					If U.compareAndSwapInt(Me, STATUS, s, s Or SIGNAL) Then
						SyncLock Me
							If status >= 0 Then
								Try
									wait(0L)
								Catch ie As InterruptedException
									interrupted = True
								End Try
							Else
								notifyAll()
							End If
						End SyncLock
					End If
					s = status
				Loop While s >= 0
				If interrupted Then Thread.CurrentThread.Interrupt()
			End If
			Return s
		End Function

		''' <summary>
		''' Blocks a non-worker-thread until completion or interruption.
		''' </summary>
		Private Function externalInterruptibleAwaitDone() As Integer
			Dim s As Integer
			If Thread.interrupted() Then Throw New InterruptedException
			s = status
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
			s = (If(TypeOf Me Is CountedCompleter, ForkJoinPool.common.externalHelpComplete(CType(Me, CountedCompleter(Of ?)), 0), If(ForkJoinPool.common.tryExternalUnpush(Me), doExec(), 0)))
			If s >= 0 AndAlso s >= 0 Then
				s = status
				Do While s >= 0
					If U.compareAndSwapInt(Me, STATUS, s, s Or SIGNAL) Then
						SyncLock Me
							If status >= 0 Then
								wait(0L)
							Else
								notifyAll()
							End If
						End SyncLock
					End If
					s = status
				Loop
			End If
			Return s
		End Function

		''' <summary>
		''' Implementation for join, get, quietlyJoin. Directly handles
		''' only cases of already-completed, external wait, and
		''' unfork+exec.  Others are relayed to ForkJoinPool.awaitJoin.
		''' </summary>
		''' <returns> status upon completion </returns>
		Private Function doJoin() As Integer
			Dim s As Integer
			Dim t As Thread
			Dim wt As ForkJoinWorkerThread
			Dim w As ForkJoinPool.WorkQueue
'JAVA TO VB CONVERTER TODO TASK: Assignments within expressions are not supported in VB
			Return If((s = status) < 0, s, If(TypeOf (t = Thread.CurrentThread) Is ForkJoinWorkerThread, If((w = (wt = CType(t, ForkJoinWorkerThread)).workQueue).tryUnpush(Me) AndAlso (s = doExec()) < 0, s, wt.pool.awaitJoin(w, Me, 0L)), externalAwaitDone()))
		End Function

		''' <summary>
		''' Implementation for invoke, quietlyInvoke.
		''' </summary>
		''' <returns> status upon completion </returns>
		Private Function doInvoke() As Integer
			Dim s As Integer
			Dim t As Thread
			Dim wt As ForkJoinWorkerThread
				s = doExec()
				If s < 0 Then
					Return s
				Else
						t = Thread.CurrentThread
						If TypeOf t Is ForkJoinWorkerThread Then
'JAVA TO VB CONVERTER TODO TASK: Assignments within expressions are not supported in VB
							Return (wt = CType(t, ForkJoinWorkerThread)).pool.awaitJoin(wt.workQueue, Me, 0L)
						Else
							Return externalAwaitDone()
						End If
				End If
		End Function

		' Exception table support

		''' <summary>
		''' Table of exceptions thrown by tasks, to enable reporting by
		''' callers. Because exceptions are rare, we don't directly keep
		''' them with task objects, but instead use a weak ref table.  Note
		''' that cancellation exceptions don't appear in the table, but are
		''' instead recorded as status values.
		''' 
		''' Note: These statics are initialized below in static block.
		''' </summary>
		Private Shared ReadOnly exceptionTable As ExceptionNode()
		Private Shared ReadOnly exceptionTableLock As java.util.concurrent.locks.ReentrantLock
		Private Shared ReadOnly exceptionTableRefQueue As ReferenceQueue(Of Object)

		''' <summary>
		''' Fixed capacity for exceptionTable.
		''' </summary>
		Private Const EXCEPTION_MAP_CAPACITY As Integer = 32

		''' <summary>
		''' Key-value nodes for exception table.  The chained hash table
		''' uses identity comparisons, full locking, and weak references
		''' for keys. The table has a fixed capacity because it only
		''' maintains task exceptions long enough for joiners to access
		''' them, so should never become very large for sustained
		''' periods. However, since we do not know when the last joiner
		''' completes, we must use weak references and expunge them. We do
		''' so on each operation (hence full locking). Also, some thread in
		''' any ForkJoinPool will call helpExpungeStaleExceptions when its
		''' pool becomes isQuiescent.
		''' </summary>
		Friend NotInheritable Class ExceptionNode
			Inherits WeakReference(Of ForkJoinTask(Of JavaToDotNetGenericWildcard))

			Friend ReadOnly ex As Throwable
			Friend [next] As ExceptionNode
			Friend ReadOnly thrower As Long ' use id not ref to avoid weak cycles
			Friend ReadOnly hashCode As Integer ' store task hashCode before weak ref disappears
			Friend Sub New(Of T1)(  task As ForkJoinTask(Of T1),   ex As Throwable,   [next] As ExceptionNode)
				MyBase.New(task, exceptionTableRefQueue)
				Me.ex = ex
				Me.next = [next]
				Me.thrower = Thread.CurrentThread.id
				Me.hashCode = System.identityHashCode(task)
			End Sub
		End Class

		''' <summary>
		''' Records exception and sets status.
		''' </summary>
		''' <returns> status on exit </returns>
		Friend Function recordExceptionalCompletion(  ex As Throwable) As Integer
			Dim s As Integer
			s = status
			If s >= 0 Then
				Dim h As Integer = System.identityHashCode(Me)
				Dim lock As java.util.concurrent.locks.ReentrantLock = exceptionTableLock
				lock.lock()
				Try
					expungeStaleExceptions()
					Dim t As ExceptionNode() = exceptionTable
					Dim i As Integer = h And (t.Length - 1)
					Dim e As ExceptionNode = t(i)
					Do
						If e Is Nothing Then
							t(i) = New ExceptionNode(Me, ex, t(i))
							Exit Do
						End If
						If e.get() Is Me Then ' already present Exit Do
						e = e.next
					Loop
				Finally
					lock.unlock()
				End Try
				s = completionion(EXCEPTIONAL)
			End If
			Return s
		End Function

		''' <summary>
		''' Records exception and possibly propagates.
		''' </summary>
		''' <returns> status on exit </returns>
		Private Function setExceptionalCompletion(  ex As Throwable) As Integer
			Dim s As Integer = recordExceptionalCompletion(ex)
			If (s And DONE_MASK) = EXCEPTIONAL Then internalPropagateException(ex)
			Return s
		End Function

		''' <summary>
		''' Hook for exception propagation support for tasks with completers.
		''' </summary>
		Friend Overridable Sub internalPropagateException(  ex As Throwable)
		End Sub

		''' <summary>
		''' Cancels, ignoring any exceptions thrown by cancel. Used during
		''' worker and pool shutdown. Cancel is spec'ed not to throw any
		''' exceptions, but if it does anyway, we have no recourse during
		''' shutdown, so guard against this case.
		''' </summary>
		Shared Sub cancelIgnoringExceptions(Of T1)(  t As ForkJoinTask(Of T1))
			If t IsNot Nothing AndAlso t.status >= 0 Then
				Try
					t.cancel(False)
				Catch ignore As Throwable
				End Try
			End If
		End Sub

		''' <summary>
		''' Removes exception node and clears status.
		''' </summary>
		Private Sub clearExceptionalCompletion()
			Dim h As Integer = System.identityHashCode(Me)
			Dim lock As java.util.concurrent.locks.ReentrantLock = exceptionTableLock
			lock.lock()
			Try
				Dim t As ExceptionNode() = exceptionTable
				Dim i As Integer = h And (t.Length - 1)
				Dim e As ExceptionNode = t(i)
				Dim pred As ExceptionNode = Nothing
				Do While e IsNot Nothing
					Dim [next] As ExceptionNode = e.next
					If e.get() Is Me Then
						If pred Is Nothing Then
							t(i) = [next]
						Else
							pred.next = [next]
						End If
						Exit Do
					End If
					pred = e
					e = [next]
				Loop
				expungeStaleExceptions()
				status = 0
			Finally
				lock.unlock()
			End Try
		End Sub

		''' <summary>
		''' Returns a rethrowable exception for the given task, if
		''' available. To provide accurate stack traces, if the exception
		''' was not thrown by the current thread, we try to create a new
		''' exception of the same type as the one thrown, but with the
		''' recorded exception as its cause. If there is no such
		''' constructor, we instead try to use a no-arg constructor,
		''' followed by initCause, to the same effect. If none of these
		''' apply, or any fail due to other exceptions, we return the
		''' recorded exception, which is still correct, although it may
		''' contain a misleading stack trace.
		''' </summary>
		''' <returns> the exception, or null if none </returns>
		Private Property throwableException As Throwable
			Get
				If (status And DONE_MASK) <> EXCEPTIONAL Then Return Nothing
				Dim h As Integer = System.identityHashCode(Me)
				Dim e As ExceptionNode
				Dim lock As java.util.concurrent.locks.ReentrantLock = exceptionTableLock
				lock.lock()
				Try
					expungeStaleExceptions()
					Dim t As ExceptionNode() = exceptionTable
					e = t(h And (t.Length - 1))
					Do While e IsNot Nothing AndAlso e.get() IsNot Me
						e = e.next
					Loop
				Finally
					lock.unlock()
				End Try
				Dim ex As Throwable
				ex = e.ex
				If e Is Nothing OrElse ex Is Nothing Then Return Nothing
				If e.thrower <> Thread.CurrentThread.id Then
					Dim ec As  [Class] = ex.GetType()
					Try
	'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
						Dim noArgCtor As Constructor(Of ?) = Nothing
	'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
						Dim cs As Constructor(Of ?)() = ec.constructors ' public ctors only
						For i As Integer = 0 To cs.Length - 1
	'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
							Dim c As Constructor(Of ?) = cs(i)
							Dim ps As  [Class]() = c.parameterTypes
							If ps.Length = 0 Then
								noArgCtor = c
							ElseIf ps.Length = 1 AndAlso ps(0) Is GetType(Throwable) Then
								Dim wx As Throwable = CType(c.newInstance(ex), Throwable)
								Return If(wx Is Nothing, ex, wx)
							End If
						Next i
						If noArgCtor IsNot Nothing Then
							Dim wx As Throwable = CType(noArgCtor.newInstance(), Throwable)
							If wx IsNot Nothing Then
								wx.initCause(ex)
								Return wx
							End If
						End If
					Catch ignore As Exception
					End Try
				End If
				Return ex
			End Get
		End Property

		''' <summary>
		''' Poll stale refs and remove them. Call only while holding lock.
		''' </summary>
		Private Shared Sub expungeStaleExceptions()
			Dim x As Object
'JAVA TO VB CONVERTER TODO TASK: Assignments within expressions are not supported in VB
			Do While (x = exceptionTableRefQueue.poll()) IsNot Nothing
				If TypeOf x Is ExceptionNode Then
					Dim hashCode As Integer = CType(x, ExceptionNode).hashCode
					Dim t As ExceptionNode() = exceptionTable
					Dim i As Integer = hashCode And (t.Length - 1)
					Dim e As ExceptionNode = t(i)
					Dim pred As ExceptionNode = Nothing
					Do While e IsNot Nothing
						Dim [next] As ExceptionNode = e.next
						If e Is x Then
							If pred Is Nothing Then
								t(i) = [next]
							Else
								pred.next = [next]
							End If
							Exit Do
						End If
						pred = e
						e = [next]
					Loop
				End If
			Loop
		End Sub

		''' <summary>
		''' If lock is available, poll stale refs and remove them.
		''' Called from ForkJoinPool when pools become quiescent.
		''' </summary>
		Friend Shared Sub helpExpungeStaleExceptions()
			Dim lock As java.util.concurrent.locks.ReentrantLock = exceptionTableLock
			If lock.tryLock() Then
				Try
					expungeStaleExceptions()
				Finally
					lock.unlock()
				End Try
			End If
		End Sub

		''' <summary>
		''' A version of "sneaky throw" to relay exceptions
		''' </summary>
		Friend Shared Sub rethrow(  ex As Throwable)
			If ex IsNot Nothing Then ForkJoinTask.uncheckedThrow(Of RuntimeException)(ex)
		End Sub

		''' <summary>
		''' The sneaky part of sneaky throw, relying on generics
		''' limitations to evade compiler complaints about rethrowing
		''' unchecked exceptions
		''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
		Friend Shared Sub uncheckedThrow(Of T As Throwable)(  t As Throwable)
			Throw CType(t, T) ' rely on vacuous cast
		End Sub

		''' <summary>
		''' Throws exception, if any, associated with the given status.
		''' </summary>
		Private Sub reportException(  s As Integer)
			If s = CANCELLED Then Throw New java.util.concurrent.CancellationException
			If s = EXCEPTIONAL Then rethrow(throwableException)
		End Sub

		' public methods

		''' <summary>
		''' Arranges to asynchronously execute this task in the pool the
		''' current task is running in, if applicable, or using the {@link
		''' ForkJoinPool#commonPool()} if not <seealso cref="#inForkJoinPool"/>.  While
		''' it is not necessarily enforced, it is a usage error to fork a
		''' task more than once unless it has completed and been
		''' reinitialized.  Subsequent modifications to the state of this
		''' task or any data it operates on are not necessarily
		''' consistently observable by any thread other than the one
		''' executing it unless preceded by a call to <seealso cref="#join"/> or
		''' related methods, or a call to <seealso cref="#isDone"/> returning {@code
		''' true}.
		''' </summary>
		''' <returns> {@code this}, to simplify usage </returns>
		Public Function fork() As ForkJoinTask(Of V)
			Dim t As Thread
			t = Thread.CurrentThread
			If TypeOf t Is ForkJoinWorkerThread Then
				CType(t, ForkJoinWorkerThread).workQueue.push(Me)
			Else
				ForkJoinPool.common.externalPush(Me)
			End If
			Return Me
		End Function

		''' <summary>
		''' Returns the result of the computation when it {@link #isDone is
		''' done}.  This method differs from <seealso cref="#get()"/> in that
		''' abnormal completion results in {@code RuntimeException} or
		''' {@code Error}, not {@code ExecutionException}, and that
		''' interrupts of the calling thread do <em>not</em> cause the
		''' method to abruptly return by throwing {@code
		''' InterruptedException}.
		''' </summary>
		''' <returns> the computed result </returns>
		Public Function join() As V
			Dim s As Integer
			s = doJoin() And DONE_MASK
			If s <> NORMAL Then reportException(s)
			Return rawResult
		End Function

		''' <summary>
		''' Commences performing this task, awaits its completion if
		''' necessary, and returns its result, or throws an (unchecked)
		''' {@code RuntimeException} or {@code Error} if the underlying
		''' computation did so.
		''' </summary>
		''' <returns> the computed result </returns>
		Public Function invoke() As V
			Dim s As Integer
			s = doInvoke() And DONE_MASK
			If s <> NORMAL Then reportException(s)
			Return rawResult
		End Function

		''' <summary>
		''' Forks the given tasks, returning when {@code isDone} holds for
		''' each task or an (unchecked) exception is encountered, in which
		''' case the exception is rethrown. If more than one task
		''' encounters an exception, then this method throws any one of
		''' these exceptions. If any task encounters an exception, the
		''' other may be cancelled. However, the execution status of
		''' individual tasks is not guaranteed upon exceptional return. The
		''' status of each task may be obtained using {@link
		''' #getException()} and related methods to check if they have been
		''' cancelled, completed normally or exceptionally, or left
		''' unprocessed.
		''' </summary>
		''' <param name="t1"> the first task </param>
		''' <param name="t2"> the second task </param>
		''' <exception cref="NullPointerException"> if any task is null </exception>
		Public Shared Sub invokeAll(Of T1, T2)(  t1 As ForkJoinTask(Of T1),   t2 As ForkJoinTask(Of T2))
			Dim s1, s2 As Integer
			t2.fork()
			s1 = t1.doInvoke() And DONE_MASK
			If s1 <> NORMAL Then t1.reportException(s1)
			s2 = t2.doJoin() And DONE_MASK
			If s2 <> NORMAL Then t2.reportException(s2)
		End Sub

		''' <summary>
		''' Forks the given tasks, returning when {@code isDone} holds for
		''' each task or an (unchecked) exception is encountered, in which
		''' case the exception is rethrown. If more than one task
		''' encounters an exception, then this method throws any one of
		''' these exceptions. If any task encounters an exception, others
		''' may be cancelled. However, the execution status of individual
		''' tasks is not guaranteed upon exceptional return. The status of
		''' each task may be obtained using <seealso cref="#getException()"/> and
		''' related methods to check if they have been cancelled, completed
		''' normally or exceptionally, or left unprocessed.
		''' </summary>
		''' <param name="tasks"> the tasks </param>
		''' <exception cref="NullPointerException"> if any task is null </exception>
		Public Shared Sub invokeAll(Of T1)(ParamArray   tasks As ForkJoinTask(Of T1)())
			Dim ex As Throwable = Nothing
			Dim last As Integer = tasks.Length - 1
			For i As Integer = last To 0 Step -1
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
				Dim t As ForkJoinTask(Of ?) = tasks(i)
				If t Is Nothing Then
					If ex Is Nothing Then ex = New NullPointerException
				ElseIf i <> 0 Then
					t.fork()
				ElseIf t.doInvoke() < NORMAL AndAlso ex Is Nothing Then
					ex = t.exception
				End If
			Next i
			For i As Integer = 1 To last
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
				Dim t As ForkJoinTask(Of ?) = tasks(i)
				If t IsNot Nothing Then
					If ex IsNot Nothing Then
						t.cancel(False)
					ElseIf t.doJoin() < NORMAL Then
						ex = t.exception
					End If
				End If
			Next i
			If ex IsNot Nothing Then rethrow(ex)
		End Sub

		''' <summary>
		''' Forks all tasks in the specified collection, returning when
		''' {@code isDone} holds for each task or an (unchecked) exception
		''' is encountered, in which case the exception is rethrown. If
		''' more than one task encounters an exception, then this method
		''' throws any one of these exceptions. If any task encounters an
		''' exception, others may be cancelled. However, the execution
		''' status of individual tasks is not guaranteed upon exceptional
		''' return. The status of each task may be obtained using {@link
		''' #getException()} and related methods to check if they have been
		''' cancelled, completed normally or exceptionally, or left
		''' unprocessed.
		''' </summary>
		''' <param name="tasks"> the collection of tasks </param>
		''' @param <T> the type of the values returned from the tasks </param>
		''' <returns> the tasks argument, to simplify usage </returns>
		''' <exception cref="NullPointerException"> if tasks or any element are null </exception>
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
		Public Shared Function invokeAll(Of T As ForkJoinTask(Of ?))(  tasks As ICollection(Of T)) As ICollection(Of T)
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
			If Not(TypeOf tasks Is java.util.RandomAccess) OrElse Not(TypeOf tasks Is IList(Of ?)) Then
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
				invokeAll(tasks.ToArray(New ForkJoinTask(Of ?)(tasks.Count - 1){}))
				Return tasks
			End If
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
			Dim ts As IList(Of ? As ForkJoinTask(Of ?)) = CType(tasks, IList(Of ? As ForkJoinTask(Of ?)))
			Dim ex As Throwable = Nothing
			Dim last As Integer = ts.Count - 1
			For i As Integer = last To 0 Step -1
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
				Dim t As ForkJoinTask(Of ?) = ts(i)
				If t Is Nothing Then
					If ex Is Nothing Then ex = New NullPointerException
				ElseIf i <> 0 Then
					t.fork()
				ElseIf t.doInvoke() < NORMAL AndAlso ex Is Nothing Then
					ex = t.exception
				End If
			Next i
			For i As Integer = 1 To last
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
				Dim t As ForkJoinTask(Of ?) = ts(i)
				If t IsNot Nothing Then
					If ex IsNot Nothing Then
						t.cancel(False)
					ElseIf t.doJoin() < NORMAL Then
						ex = t.exception
					End If
				End If
			Next i
			If ex IsNot Nothing Then rethrow(ex)
			Return tasks
		End Function

		''' <summary>
		''' Attempts to cancel execution of this task. This attempt will
		''' fail if the task has already completed or could not be
		''' cancelled for some other reason. If successful, and this task
		''' has not started when {@code cancel} is called, execution of
		''' this task is suppressed. After this method returns
		''' successfully, unless there is an intervening call to {@link
		''' #reinitialize}, subsequent calls to <seealso cref="#isCancelled"/>,
		''' <seealso cref="#isDone"/>, and {@code cancel} will return {@code true}
		''' and calls to <seealso cref="#join"/> and related methods will result in
		''' {@code CancellationException}.
		''' 
		''' <p>This method may be overridden in subclasses, but if so, must
		''' still ensure that these properties hold. In particular, the
		''' {@code cancel} method itself must not throw exceptions.
		''' 
		''' <p>This method is designed to be invoked by <em>other</em>
		''' tasks. To terminate the current task, you can just return or
		''' throw an unchecked exception from its computation method, or
		''' invoke <seealso cref="#completeExceptionally(Throwable)"/>.
		''' </summary>
		''' <param name="mayInterruptIfRunning"> this value has no effect in the
		''' default implementation because interrupts are not used to
		''' control cancellation.
		''' </param>
		''' <returns> {@code true} if this task is now cancelled </returns>
		Public Overridable Function cancel(  mayInterruptIfRunning As Boolean) As Boolean
			Return (completionion(CANCELLED) And DONE_MASK) = CANCELLED
		End Function

		Public Property done As Boolean
			Get
				Return status < 0
			End Get
		End Property

		Public Property cancelled As Boolean
			Get
				Return (status And DONE_MASK) = CANCELLED
			End Get
		End Property

		''' <summary>
		''' Returns {@code true} if this task threw an exception or was cancelled.
		''' </summary>
		''' <returns> {@code true} if this task threw an exception or was cancelled </returns>
		Public Property completedAbnormally As Boolean
			Get
				Return status < NORMAL
			End Get
		End Property

		''' <summary>
		''' Returns {@code true} if this task completed without throwing an
		''' exception and was not cancelled.
		''' </summary>
		''' <returns> {@code true} if this task completed without throwing an
		''' exception and was not cancelled </returns>
		Public Property completedNormally As Boolean
			Get
				Return (status And DONE_MASK) = NORMAL
			End Get
		End Property

		''' <summary>
		''' Returns the exception thrown by the base computation, or a
		''' {@code CancellationException} if cancelled, or {@code null} if
		''' none or if the method has not yet completed.
		''' </summary>
		''' <returns> the exception, or {@code null} if none </returns>
		Public Property exception As Throwable
			Get
				Dim s As Integer = status And DONE_MASK
				Return (If(s >= NORMAL, Nothing, If(s = CANCELLED, New java.util.concurrent.CancellationException, throwableException)))
			End Get
		End Property

		''' <summary>
		''' Completes this task abnormally, and if not already aborted or
		''' cancelled, causes it to throw the given exception upon
		''' {@code join} and related operations. This method may be used
		''' to induce exceptions in asynchronous tasks, or to force
		''' completion of tasks that would not otherwise complete.  Its use
		''' in other situations is discouraged.  This method is
		''' overridable, but overridden versions must invoke {@code super}
		''' implementation to maintain guarantees.
		''' </summary>
		''' <param name="ex"> the exception to throw. If this exception is not a
		''' {@code RuntimeException} or {@code Error}, the actual exception
		''' thrown will be a {@code RuntimeException} with cause {@code ex}. </param>
		Public Overridable Sub completeExceptionally(  ex As Throwable)
			exceptionalCompletion = If((TypeOf ex Is RuntimeException) OrElse (TypeOf ex Is Error), ex, New RuntimeException(ex))
		End Sub

		''' <summary>
		''' Completes this task, and if not already aborted or cancelled,
		''' returning the given value as the result of subsequent
		''' invocations of {@code join} and related operations. This method
		''' may be used to provide results for asynchronous tasks, or to
		''' provide alternative handling for tasks that would not otherwise
		''' complete normally. Its use in other situations is
		''' discouraged. This method is overridable, but overridden
		''' versions must invoke {@code super} implementation to maintain
		''' guarantees.
		''' </summary>
		''' <param name="value"> the result value for this task </param>
		Public Overridable Sub complete(  value As V)
			Try
				rawResult = value
			Catch rex As Throwable
				exceptionalCompletion = rex
				Return
			End Try
			completion = NORMAL
		End Sub

		''' <summary>
		''' Completes this task normally without setting a value. The most
		''' recent value established by <seealso cref="#setRawResult"/> (or {@code
		''' null} by default) will be returned as the result of subsequent
		''' invocations of {@code join} and related operations.
		''' 
		''' @since 1.8
		''' </summary>
		Public Sub quietlyComplete()
			completion = NORMAL
		End Sub

		''' <summary>
		''' Waits if necessary for the computation to complete, and then
		''' retrieves its result.
		''' </summary>
		''' <returns> the computed result </returns>
		''' <exception cref="CancellationException"> if the computation was cancelled </exception>
		''' <exception cref="ExecutionException"> if the computation threw an
		''' exception </exception>
		''' <exception cref="InterruptedException"> if the current thread is not a
		''' member of a ForkJoinPool and was interrupted while waiting </exception>
		Public Function [get]() As V
			Dim s As Integer = If(TypeOf Thread.CurrentThread Is ForkJoinWorkerThread, doJoin(), externalInterruptibleAwaitDone())
			Dim ex As Throwable
'JAVA TO VB CONVERTER TODO TASK: Assignments within expressions are not supported in VB
			If (s = s And DONE_MASK) = CANCELLED Then Throw New java.util.concurrent.CancellationException
			ex = throwableException
			If s = EXCEPTIONAL AndAlso ex IsNot Nothing Then Throw New java.util.concurrent.ExecutionException(ex)
			Return rawResult
		End Function

		''' <summary>
		''' Waits if necessary for at most the given time for the computation
		''' to complete, and then retrieves its result, if available.
		''' </summary>
		''' <param name="timeout"> the maximum time to wait </param>
		''' <param name="unit"> the time unit of the timeout argument </param>
		''' <returns> the computed result </returns>
		''' <exception cref="CancellationException"> if the computation was cancelled </exception>
		''' <exception cref="ExecutionException"> if the computation threw an
		''' exception </exception>
		''' <exception cref="InterruptedException"> if the current thread is not a
		''' member of a ForkJoinPool and was interrupted while waiting </exception>
		''' <exception cref="TimeoutException"> if the wait timed out </exception>
		Public Function [get](  timeout As Long,   unit As java.util.concurrent.TimeUnit) As V
			Dim s As Integer
			Dim nanos As Long = unit.toNanos(timeout)
			If Thread.interrupted() Then Throw New InterruptedException
			s = status
			If s >= 0 AndAlso nanos > 0L Then
				Dim d As Long = System.nanoTime() + nanos
				Dim deadline As Long = If(d = 0L, 1L, d) ' avoid 0
				Dim t As Thread = Thread.CurrentThread
				If TypeOf t Is ForkJoinWorkerThread Then
					Dim wt As ForkJoinWorkerThread = CType(t, ForkJoinWorkerThread)
					s = wt.pool.awaitJoin(wt.workQueue, Me, deadline)
				Else
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
					s = (If(TypeOf Me Is CountedCompleter, ForkJoinPool.common.externalHelpComplete(CType(Me, CountedCompleter(Of ?)), 0), If(ForkJoinPool.common.tryExternalUnpush(Me), doExec(), 0)))
					If s >= 0 Then
						Dim ns, ms As Long ' measure in nanosecs, but wait in millisecs
						s = status
						ns = deadline - System.nanoTime()
						Do While s >= 0 AndAlso ns > 0L
							ms = java.util.concurrent.TimeUnit.NANOSECONDS.toMillis(ns)
							If ms > 0L AndAlso U.compareAndSwapInt(Me, STATUS, s, s Or SIGNAL) Then
								SyncLock Me
									If status >= 0 Then
										wait(ms) ' OK to throw InterruptedException
									Else
										notifyAll()
									End If
								End SyncLock
							End If
							s = status
							ns = deadline - System.nanoTime()
						Loop
					End If
					End If
			End If
			If s >= 0 Then s = status
'JAVA TO VB CONVERTER TODO TASK: Assignments within expressions are not supported in VB
			If (s = s And DONE_MASK) <> NORMAL Then
				Dim ex As Throwable
				If s = CANCELLED Then Throw New java.util.concurrent.CancellationException
				If s <> EXCEPTIONAL Then Throw New java.util.concurrent.TimeoutException
				ex = throwableException
				If ex IsNot Nothing Then Throw New java.util.concurrent.ExecutionException(ex)
			End If
			Return rawResult
		End Function

		''' <summary>
		''' Joins this task, without returning its result or throwing its
		''' exception. This method may be useful when processing
		''' collections of tasks when some have been cancelled or otherwise
		''' known to have aborted.
		''' </summary>
		Public Sub quietlyJoin()
			doJoin()
		End Sub

		''' <summary>
		''' Commences performing this task and awaits its completion if
		''' necessary, without returning its result or throwing its
		''' exception.
		''' </summary>
		Public Sub quietlyInvoke()
			doInvoke()
		End Sub

		''' <summary>
		''' Possibly executes tasks until the pool hosting the current task
		''' <seealso cref="ForkJoinPool#isQuiescent is quiescent"/>. This method may
		''' be of use in designs in which many tasks are forked, but none
		''' are explicitly joined, instead executing them until all are
		''' processed.
		''' </summary>
		Public Shared Sub helpQuiesce()
			Dim t As Thread
			t = Thread.CurrentThread
			If TypeOf t Is ForkJoinWorkerThread Then
				Dim wt As ForkJoinWorkerThread = CType(t, ForkJoinWorkerThread)
				wt.pool.helpQuiescePool(wt.workQueue)
			Else
				ForkJoinPool.quiesceCommonPool()
			End If
		End Sub

		''' <summary>
		''' Resets the internal bookkeeping state of this task, allowing a
		''' subsequent {@code fork}. This method allows repeated reuse of
		''' this task, but only if reuse occurs when this task has either
		''' never been forked, or has been forked, then completed and all
		''' outstanding joins of this task have also completed. Effects
		''' under any other usage conditions are not guaranteed.
		''' This method may be useful when executing
		''' pre-constructed trees of subtasks in loops.
		''' 
		''' <p>Upon completion of this method, {@code isDone()} reports
		''' {@code false}, and {@code getException()} reports {@code
		''' null}. However, the value returned by {@code getRawResult} is
		''' unaffected. To clear this value, you can invoke {@code
		''' setRawResult(null)}.
		''' </summary>
		Public Overridable Sub reinitialize()
			If (status And DONE_MASK) = EXCEPTIONAL Then
				clearExceptionalCompletion()
			Else
				status = 0
			End If
		End Sub

		''' <summary>
		''' Returns the pool hosting the current task execution, or null
		''' if this task is executing outside of any ForkJoinPool.
		''' </summary>
		''' <seealso cref= #inForkJoinPool </seealso>
		''' <returns> the pool, or {@code null} if none </returns>
		PublicShared ReadOnly Propertypool As ForkJoinPool
			Get
				Dim t As Thread = Thread.CurrentThread
				Return If(TypeOf t Is ForkJoinWorkerThread, CType(t, ForkJoinWorkerThread).pool, Nothing)
			End Get
		End Property

		''' <summary>
		''' Returns {@code true} if the current thread is a {@link
		''' ForkJoinWorkerThread} executing as a ForkJoinPool computation.
		''' </summary>
		''' <returns> {@code true} if the current thread is a {@link
		''' ForkJoinWorkerThread} executing as a ForkJoinPool computation,
		''' or {@code false} otherwise </returns>
		Public Shared Function inForkJoinPool() As Boolean
			Return TypeOf Thread.CurrentThread Is ForkJoinWorkerThread
		End Function

		''' <summary>
		''' Tries to unschedule this task for execution. This method will
		''' typically (but is not guaranteed to) succeed if this task is
		''' the most recently forked task by the current thread, and has
		''' not commenced executing in another thread.  This method may be
		''' useful when arranging alternative local processing of tasks
		''' that could have been, but were not, stolen.
		''' </summary>
		''' <returns> {@code true} if unforked </returns>
		Public Overridable Function tryUnfork() As Boolean
			Dim t As Thread
'JAVA TO VB CONVERTER TODO TASK: Assignments within expressions are not supported in VB
			Return (If(TypeOf (t = Thread.CurrentThread) Is ForkJoinWorkerThread, CType(t, ForkJoinWorkerThread).workQueue.tryUnpush(Me), ForkJoinPool.common.tryExternalUnpush(Me)))
		End Function

		''' <summary>
		''' Returns an estimate of the number of tasks that have been
		''' forked by the current worker thread but not yet executed. This
		''' value may be useful for heuristic decisions about whether to
		''' fork other tasks.
		''' </summary>
		''' <returns> the number of tasks </returns>
		PublicShared ReadOnly PropertyqueuedTaskCount As Integer
			Get
				Dim t As Thread
				Dim q As ForkJoinPool.WorkQueue
				t = Thread.CurrentThread
				If TypeOf t Is ForkJoinWorkerThread Then
					q = CType(t, ForkJoinWorkerThread).workQueue
				Else
					q = ForkJoinPool.commonSubmitterQueue()
				End If
				Return If(q Is Nothing, 0, q.queueSize())
			End Get
		End Property

		''' <summary>
		''' Returns an estimate of how many more locally queued tasks are
		''' held by the current worker thread than there are other worker
		''' threads that might steal them, or zero if this thread is not
		''' operating in a ForkJoinPool. This value may be useful for
		''' heuristic decisions about whether to fork other tasks. In many
		''' usages of ForkJoinTasks, at steady state, each worker should
		''' aim to maintain a small constant surplus (for example, 3) of
		''' tasks, and to process computations locally if this threshold is
		''' exceeded.
		''' </summary>
		''' <returns> the surplus number of tasks, which may be negative </returns>
		PublicShared ReadOnly PropertysurplusQueuedTaskCount As Integer
			Get
				Return ForkJoinPool.surplusQueuedTaskCount
			End Get
		End Property

		' Extension methods

		''' <summary>
		''' Returns the result that would be returned by <seealso cref="#join"/>, even
		''' if this task completed abnormally, or {@code null} if this task
		''' is not known to have been completed.  This method is designed
		''' to aid debugging, as well as to support extensions. Its use in
		''' any other context is discouraged.
		''' </summary>
		''' <returns> the result, or {@code null} if not completed </returns>
		Public MustOverride Property rawResult As V


		''' <summary>
		''' Immediately performs the base action of this task and returns
		''' true if, upon return from this method, this task is guaranteed
		''' to have completed normally. This method may return false
		''' otherwise, to indicate that this task is not necessarily
		''' complete (or is not known to be complete), for example in
		''' asynchronous actions that require explicit invocations of
		''' completion methods. This method may also throw an (unchecked)
		''' exception to indicate abnormal exit. This method is designed to
		''' support extensions, and should not in general be called
		''' otherwise.
		''' </summary>
		''' <returns> {@code true} if this task is known to have completed normally </returns>
		Protected Friend MustOverride Function exec() As Boolean

		''' <summary>
		''' Returns, but does not unschedule or execute, a task queued by
		''' the current thread but not yet executed, if one is immediately
		''' available. There is no guarantee that this task will actually
		''' be polled or executed next. Conversely, this method may return
		''' null even if a task exists but cannot be accessed without
		''' contention with other threads.  This method is designed
		''' primarily to support extensions, and is unlikely to be useful
		''' otherwise.
		''' </summary>
		''' <returns> the next task, or {@code null} if none are available </returns>
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
		Protected Friend Shared Function peekNextLocalTask() As ForkJoinTask(Of ?)
			Dim t As Thread
			Dim q As ForkJoinPool.WorkQueue
			t = Thread.CurrentThread
			If TypeOf t Is ForkJoinWorkerThread Then
				q = CType(t, ForkJoinWorkerThread).workQueue
			Else
				q = ForkJoinPool.commonSubmitterQueue()
			End If
			Return If(q Is Nothing, Nothing, q.peek())
		End Function

		''' <summary>
		''' Unschedules and returns, without executing, the next task
		''' queued by the current thread but not yet executed, if the
		''' current thread is operating in a ForkJoinPool.  This method is
		''' designed primarily to support extensions, and is unlikely to be
		''' useful otherwise.
		''' </summary>
		''' <returns> the next task, or {@code null} if none are available </returns>
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
		Protected Friend Shared Function pollNextLocalTask() As ForkJoinTask(Of ?)
			Dim t As Thread
'JAVA TO VB CONVERTER TODO TASK: Assignments within expressions are not supported in VB
			Return If(TypeOf (t = Thread.CurrentThread) Is ForkJoinWorkerThread, CType(t, ForkJoinWorkerThread).workQueue.nextLocalTask(), Nothing)
		End Function

		''' <summary>
		''' If the current thread is operating in a ForkJoinPool,
		''' unschedules and returns, without executing, the next task
		''' queued by the current thread but not yet executed, if one is
		''' available, or if not available, a task that was forked by some
		''' other thread, if available. Availability may be transient, so a
		''' {@code null} result does not necessarily imply quiescence of
		''' the pool this task is operating in.  This method is designed
		''' primarily to support extensions, and is unlikely to be useful
		''' otherwise.
		''' </summary>
		''' <returns> a task, or {@code null} if none are available </returns>
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
		Protected Friend Shared Function pollTask() As ForkJoinTask(Of ?)
			Dim t As Thread
			Dim wt As ForkJoinWorkerThread
				t = Thread.CurrentThread
				If TypeOf t Is ForkJoinWorkerThread Then
'JAVA TO VB CONVERTER TODO TASK: Assignments within expressions are not supported in VB
					Return (wt = CType(t, ForkJoinWorkerThread)).pool.nextTaskFor(wt.workQueue)
				Else
					Return Nothing
				End If
		End Function

		' tag operations

		''' <summary>
		''' Returns the tag for this task.
		''' </summary>
		''' <returns> the tag for this task
		''' @since 1.8 </returns>
		Public Property forkJoinTaskTag As Short
			Get
				Return CShort(status)
			End Get
		End Property

		''' <summary>
		''' Atomically sets the tag value for this task.
		''' </summary>
		''' <param name="tag"> the tag value </param>
		''' <returns> the previous value of the tag
		''' @since 1.8 </returns>
		Public Function setForkJoinTaskTag(  tag As Short) As Short
			Dim s As Integer
			Do
'JAVA TO VB CONVERTER TODO TASK: Assignments within expressions are not supported in VB
				If U.compareAndSwapInt(Me, STATUS, s = status, (s And (Not SMASK)) Or (tag And SMASK)) Then Return CShort(s)
			Loop
		End Function

		''' <summary>
		''' Atomically conditionally sets the tag value for this task.
		''' Among other applications, tags can be used as visit markers
		''' in tasks operating on graphs, as in methods that check: {@code
		''' if (task.compareAndSetForkJoinTaskTag((short)0, (short)1))}
		''' before processing, otherwise exiting because the node has
		''' already been visited.
		''' </summary>
		''' <param name="e"> the expected tag value </param>
		''' <param name="tag"> the new tag value </param>
		''' <returns> {@code true} if successful; i.e., the current value was
		''' equal to e and is now tag.
		''' @since 1.8 </returns>
		Public Function compareAndSetForkJoinTaskTag(  e As Short,   tag As Short) As Boolean
			Dim s As Integer
			Do
				s = status
				If CShort(s) <> e Then Return False
				If U.compareAndSwapInt(Me, STATUS, s, (s And (Not SMASK)) Or (tag And SMASK)) Then Return True
			Loop
		End Function

		''' <summary>
		''' Adaptor for Runnables. This implements RunnableFuture
		''' to be compliant with AbstractExecutorService constraints
		''' when used in ForkJoinPool.
		''' </summary>
		Friend NotInheritable Class AdaptedRunnable(Of T)
			Inherits ForkJoinTask(Of T)
			Implements java.util.concurrent.RunnableFuture(Of T)

			Friend ReadOnly runnable As Runnable
			Friend result As T
			Friend Sub New(  runnable As Runnable,   result As T)
				If runnable Is Nothing Then Throw New NullPointerException
				Me.runnable = runnable
				Me.result = result ' OK to set this even before completion
			End Sub
			Public Property rawResult As T
				Get
					Return result
				End Get
				Set(  v As T)
					result = v
				End Set
			End Property
			Public Function exec() As Boolean
				runnable.run()
				Return True
			End Function
			Public Sub run()
				invoke()
			End Sub
			Private Const serialVersionUID As Long = 5232453952276885070L
		End Class

		''' <summary>
		''' Adaptor for Runnables without results
		''' </summary>
		Friend NotInheritable Class AdaptedRunnableAction
			Inherits ForkJoinTask(Of Void)
			Implements java.util.concurrent.RunnableFuture(Of Void)

			Friend ReadOnly runnable As Runnable
			Friend Sub New(  runnable As Runnable)
				If runnable Is Nothing Then Throw New NullPointerException
				Me.runnable = runnable
			End Sub
			Public Property rawResult As Void
				Get
					Return Nothing
				End Get
				Set(  v As Void)
				End Set
			End Property
			Public Function exec() As Boolean
				runnable.run()
				Return True
			End Function
			Public Sub run()
				invoke()
			End Sub
			Private Const serialVersionUID As Long = 5232453952276885070L
		End Class

		''' <summary>
		''' Adaptor for Runnables in which failure forces worker exception
		''' </summary>
		Friend NotInheritable Class RunnableExecuteAction
			Inherits ForkJoinTask(Of Void)

			Friend ReadOnly runnable As Runnable
			Friend Sub New(  runnable As Runnable)
				If runnable Is Nothing Then Throw New NullPointerException
				Me.runnable = runnable
			End Sub
			Public Property rawResult As Void
				Get
					Return Nothing
				End Get
				Set(  v As Void)
				End Set
			End Property
			Public Function exec() As Boolean
				runnable.run()
				Return True
			End Function
			Friend Sub internalPropagateException(  ex As Throwable)
				rethrow(ex) ' rethrow outside exec() catches.
			End Sub
			Private Const serialVersionUID As Long = 5232453952276885070L
		End Class

		''' <summary>
		''' Adaptor for Callables
		''' </summary>
		Friend NotInheritable Class AdaptedCallable(Of T)
			Inherits ForkJoinTask(Of T)
			Implements java.util.concurrent.RunnableFuture(Of T)

'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
			Friend ReadOnly callable As java.util.concurrent.Callable(Of ? As T)
			Friend result As T
			Friend Sub New(Of T1 As T)(  callable As java.util.concurrent.Callable(Of T1))
				If callable Is Nothing Then Throw New NullPointerException
				Me.callable = callable
			End Sub
			Public Property rawResult As T
				Get
					Return result
				End Get
				Set(  v As T)
					result = v
				End Set
			End Property
			Public Function exec() As Boolean
				Try
					result = callable.call()
					Return True
				Catch err As [Error]
					Throw err
				Catch rex As RuntimeException
					Throw rex
				Catch ex As Exception
					Throw New RuntimeException(ex)
				End Try
			End Function
			Public Sub run()
				invoke()
			End Sub
			Private Const serialVersionUID As Long = 2838392045355241008L
		End Class

		''' <summary>
		''' Returns a new {@code ForkJoinTask} that performs the {@code run}
		''' method of the given {@code Runnable} as its action, and returns
		''' a null result upon <seealso cref="#join"/>.
		''' </summary>
		''' <param name="runnable"> the runnable action </param>
		''' <returns> the task </returns>
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
		Public Shared Function adapt(  runnable As Runnable) As ForkJoinTask(Of ?)
			Return New AdaptedRunnableAction(runnable)
		End Function

		''' <summary>
		''' Returns a new {@code ForkJoinTask} that performs the {@code run}
		''' method of the given {@code Runnable} as its action, and returns
		''' the given result upon <seealso cref="#join"/>.
		''' </summary>
		''' <param name="runnable"> the runnable action </param>
		''' <param name="result"> the result upon completion </param>
		''' @param <T> the type of the result </param>
		''' <returns> the task </returns>
		Public Shared Function adapt(Of T)(  runnable As Runnable,   result As T) As ForkJoinTask(Of T)
			Return New AdaptedRunnable(Of T)(runnable, result)
		End Function

		''' <summary>
		''' Returns a new {@code ForkJoinTask} that performs the {@code call}
		''' method of the given {@code Callable} as its action, and returns
		''' its result upon <seealso cref="#join"/>, translating any checked exceptions
		''' encountered into {@code RuntimeException}.
		''' </summary>
		''' <param name="callable"> the callable action </param>
		''' @param <T> the type of the callable's result </param>
		''' <returns> the task </returns>
		Public Shared Function adapt(Of T, T1 As T)(  callable As java.util.concurrent.Callable(Of T1)) As ForkJoinTask(Of T)
			Return New AdaptedCallable(Of T)(callable)
		End Function

		' Serialization support

		Private Const serialVersionUID As Long = -7721805057305804111L

		''' <summary>
		''' Saves this task to a stream (that is, serializes it).
		''' </summary>
		''' <param name="s"> the stream </param>
		''' <exception cref="java.io.IOException"> if an I/O error occurs
		''' @serialData the current run status and the exception thrown
		''' during execution, or {@code null} if none </exception>
		Private Sub writeObject(  s As java.io.ObjectOutputStream)
			s.defaultWriteObject()
			s.writeObject(exception)
		End Sub

		''' <summary>
		''' Reconstitutes this task from a stream (that is, deserializes it). </summary>
		''' <param name="s"> the stream </param>
		''' <exception cref="ClassNotFoundException"> if the class of a serialized object
		'''         could not be found </exception>
		''' <exception cref="java.io.IOException"> if an I/O error occurs </exception>
		Private Sub readObject(  s As java.io.ObjectInputStream)
			s.defaultReadObject()
			Dim ex As Object = s.readObject()
			If ex IsNot Nothing Then exceptionalCompletion = CType(ex, Throwable)
		End Sub

		' Unsafe mechanics
		Private Shared ReadOnly U As sun.misc.Unsafe
		Private Shared ReadOnly STATUS As Long

		Shared Sub New()
			exceptionTableLock = New java.util.concurrent.locks.ReentrantLock
			exceptionTableRefQueue = New ReferenceQueue(Of Object)
			exceptionTable = New ExceptionNode(EXCEPTION_MAP_CAPACITY - 1){}
			Try
				U = sun.misc.Unsafe.unsafe
				Dim k As  [Class] = GetType(ForkJoinTask)
				STATUS = U.objectFieldOffset(k.getDeclaredField("status"))
			Catch e As Exception
				Throw New [Error](e)
			End Try
		End Sub

	End Class

End Namespace