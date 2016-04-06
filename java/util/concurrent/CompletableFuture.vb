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
	''' A <seealso cref="Future"/> that may be explicitly completed (setting its
	''' value and status), and may be used as a <seealso cref="CompletionStage"/>,
	''' supporting dependent functions and actions that trigger upon its
	''' completion.
	''' 
	''' <p>When two or more threads attempt to
	''' <seealso cref="#complete complete"/>,
	''' <seealso cref="#completeExceptionally completeExceptionally"/>, or
	''' <seealso cref="#cancel cancel"/>
	''' a CompletableFuture, only one of them succeeds.
	''' 
	''' <p>In addition to these and related methods for directly
	''' manipulating status and results, CompletableFuture implements
	''' interface <seealso cref="CompletionStage"/> with the following policies: <ul>
	''' 
	''' <li>Actions supplied for dependent completions of
	''' <em>non-async</em> methods may be performed by the thread that
	''' completes the current CompletableFuture, or by any other caller of
	''' a completion method.</li>
	''' 
	''' <li>All <em>async</em> methods without an explicit Executor
	''' argument are performed using the <seealso cref="ForkJoinPool#commonPool()"/>
	''' (unless it does not support a parallelism level of at least two, in
	''' which case, a new Thread is created to run each task).  To simplify
	''' monitoring, debugging, and tracking, all generated asynchronous
	''' tasks are instances of the marker interface {@link
	''' AsynchronousCompletionTask}. </li>
	''' 
	''' <li>All CompletionStage methods are implemented independently of
	''' other public methods, so the behavior of one method is not impacted
	''' by overrides of others in subclasses.  </li> </ul>
	''' 
	''' <p>CompletableFuture also implements <seealso cref="Future"/> with the following
	''' policies: <ul>
	''' 
	''' <li>Since (unlike <seealso cref="FutureTask"/>) this class has no direct
	''' control over the computation that causes it to be completed,
	''' cancellation is treated as just another form of exceptional
	''' completion.  Method <seealso cref="#cancel cancel"/> has the same effect as
	''' {@code completeExceptionally(new CancellationException())}. Method
	''' <seealso cref="#isCompletedExceptionally"/> can be used to determine if a
	''' CompletableFuture completed in any exceptional fashion.</li>
	''' 
	''' <li>In case of exceptional completion with a CompletionException,
	''' methods <seealso cref="#get()"/> and <seealso cref="#get(long, TimeUnit)"/> throw an
	''' <seealso cref="ExecutionException"/> with the same cause as held in the
	''' corresponding CompletionException.  To simplify usage in most
	''' contexts, this class also defines methods <seealso cref="#join()"/> and
	''' <seealso cref="#getNow"/> that instead throw the CompletionException directly
	''' in these cases.</li> </ul>
	''' 
	''' @author Doug Lea
	''' @since 1.8
	''' </summary>
	Public Class CompletableFuture(Of T)
		Implements java.util.concurrent.Future(Of T), java.util.concurrent.CompletionStage(Of T)

	'    
	'     * Overview:
	'     *
	'     * A CompletableFuture may have dependent completion actions,
	'     * collected in a linked stack. It atomically completes by CASing
	'     * a result field, and then pops off and runs those actions. This
	'     * applies across normal vs exceptional outcomes, sync vs async
	'     * actions, binary triggers, and various forms of completions.
	'     *
	'     * Non-nullness of field result (set via CAS) indicates done.  An
	'     * AltResult is used to box null as a result, as well as to hold
	'     * exceptions.  Using a single field makes completion simple to
	'     * detect and trigger.  Encoding and decoding is straightforward
	'     * but adds to the sprawl of trapping and associating exceptions
	'     * with targets.  Minor simplifications rely on (static) NIL (to
	'     * box null results) being the only AltResult with a null
	'     * exception field, so we don't usually need explicit comparisons.
	'     * Even though some of the generics casts are unchecked (see
	'     * SuppressWarnings annotations), they are placed to be
	'     * appropriate even if checked.
	'     *
	'     * Dependent actions are represented by Completion objects linked
	'     * as Treiber stacks headed by field "stack". There are Completion
	'     * classes for each kind of action, grouped into single-input
	'     * (UniCompletion), two-input (BiCompletion), projected
	'     * (BiCompletions using either (not both) of two inputs), shared
	'     * (CoCompletion, used by the second of two sources), zero-input
	'     * source actions, and Signallers that unblock waiters. Class
	'     * Completion extends ForkJoinTask to enable async execution
	'     * (adding no space overhead because we exploit its "tag" methods
	'     * to maintain claims). It is also declared as Runnable to allow
	'     * usage with arbitrary executors.
	'     *
	'     * Support for each kind of CompletionStage relies on a separate
	'     * [Class], along with two CompletableFuture methods:
	'     *
	'     * * A Completion class with name X corresponding to function,
	'     *   prefaced with "Uni", "Bi", or "Or". Each class contains
	'     *   fields for source(s), actions, and dependent. They are
	'     *   boringly similar, differing from others only with respect to
	'     *   underlying functional forms. We do this so that users don't
	'     *   encounter layers of adaptors in common usages. We also
	'     *   include "Relay" classes/methods that don't correspond to user
	'     *   methods; they copy results from one stage to another.
	'     *
	'     * * Boolean CompletableFuture method x(...) (for example
	'     *   uniApply) takes all of the arguments needed to check that an
	'     *   action is triggerable, and then either runs the action or
	'     *   arranges its async execution by executing its Completion
	'     *   argument, if present. The method returns true if known to be
	'     *   complete.
	'     *
	'     * * Completion method tryFire(int mode) invokes the associated x
	'     *   method with its held arguments, and on success cleans up.
	'     *   The mode argument allows tryFire to be called twice (SYNC,
	'     *   then ASYNC); the first to screen and trap exceptions while
	'     *   arranging to execute, and the second when called from a
	'     *   task. (A few classes are not used async so take slightly
	'     *   different forms.)  The claim() callback suppresses function
	'     *   invocation if already claimed by another thread.
	'     *
	'     * * CompletableFuture method xStage(...) is called from a public
	'     *   stage method of CompletableFuture x. It screens user
	'     *   arguments and invokes and/or creates the stage object.  If
	'     *   not async and x is already complete, the action is run
	'     *   immediately.  Otherwise a Completion c is created, pushed to
	'     *   x's stack (unless done), and started or triggered via
	'     *   c.tryFire.  This also covers races possible if x completes
	'     *   while pushing.  Classes with two inputs (for example BiApply)
	'     *   deal with races across both while pushing actions.  The
	'     *   second completion is a CoCompletion pointing to the first,
	'     *   shared so that at most one performs the action.  The
	'     *   multiple-arity methods allOf and anyOf do this pairwise to
	'     *   form trees of completions.
	'     *
	'     * Note that the generic type parameters of methods vary according
	'     * to whether "this" is a source, dependent, or completion.
	'     *
	'     * Method postComplete is called upon completion unless the target
	'     * is guaranteed not to be observable (i.e., not yet returned or
	'     * linked). Multiple threads can call postComplete, which
	'     * atomically pops each dependent action, and tries to trigger it
	'     * via method tryFire, in NESTED mode.  Triggering can propagate
	'     * recursively, so NESTED mode returns its completed dependent (if
	'     * one exists) for further processing by its caller (see method
	'     * postFire).
	'     *
	'     * Blocking methods get() and join() rely on Signaller Completions
	'     * that wake up waiting threads.  The mechanics are similar to
	'     * Treiber stack wait-nodes used in FutureTask, Phaser, and
	'     * SynchronousQueue. See their internal documentation for
	'     * algorithmic details.
	'     *
	'     * Without precautions, CompletableFutures would be prone to
	'     * garbage accumulation as chains of Completions build up, each
	'     * pointing back to its sources. So we null out fields as soon as
	'     * possible (see especially method Completion.detach). The
	'     * screening checks needed anyway harmlessly ignore null arguments
	'     * that may have been obtained during races with threads nulling
	'     * out fields.  We also try to unlink fired Completions from
	'     * stacks that might never be popped (see method postFire).
	'     * Completion fields need not be declared as final or volatile
	'     * because they are only visible to other threads upon safe
	'     * publication.
	'     

'JAVA TO VB CONVERTER TODO TASK: There is no VB equivalent to 'volatile':
		Friend result As Object ' Either the result or boxed AltResult
'JAVA TO VB CONVERTER TODO TASK: There is no VB equivalent to 'volatile':
		Friend stack As Completion ' Top of Treiber stack of dependent actions

		Friend Function internalComplete(  r As Object) As Boolean ' CAS from null to r
			Return UNSAFE.compareAndSwapObject(Me, RESULT, Nothing, r)
		End Function

		Friend Function casStack(  cmp As Completion,   val As Completion) As Boolean
			Return UNSAFE.compareAndSwapObject(Me, STACK, cmp, val)
		End Function

		''' <summary>
		''' Returns true if successfully pushed c onto stack. </summary>
		Friend Function tryPushStack(  c As Completion) As Boolean
			Dim h As Completion = stack
			lazySetNext(c, h)
			Return UNSAFE.compareAndSwapObject(Me, STACK, h, c)
		End Function

		''' <summary>
		''' Unconditionally pushes c onto stack, retrying if necessary. </summary>
		Friend Sub pushStack(  c As Completion)
			Do
			Loop While Not tryPushStack(c)
		End Sub

		' ------------- Encoding and decoding outcomes -------------- 

		Friend NotInheritable Class AltResult ' See above
			Friend ReadOnly ex As Throwable ' null only for NIL
			Friend Sub New(  x As Throwable)
				Me.ex = x
			End Sub
		End Class

		''' <summary>
		''' The encoding of the null value. </summary>
		Friend Shared ReadOnly NIL As New AltResult(Nothing)

		''' <summary>
		''' Completes with the null value, unless already completed. </summary>
		Friend Function completeNull() As Boolean
			Return UNSAFE.compareAndSwapObject(Me, RESULT, Nothing, NIL)
		End Function

		''' <summary>
		''' Returns the encoding of the given non-exceptional value. </summary>
		Friend Function encodeValue(  t As T) As Object
			Return If(t Is Nothing, NIL, t)
		End Function

		''' <summary>
		''' Completes with a non-exceptional result, unless already completed. </summary>
		Friend Function completeValue(  t As T) As Boolean
			Return UNSAFE.compareAndSwapObject(Me, RESULT, Nothing,If(t Is Nothing, NIL, t))
		End Function

		''' <summary>
		''' Returns the encoding of the given (non-null) exception as a
		''' wrapped CompletionException unless it is one already.
		''' </summary>
		Friend Shared Function encodeThrowable(  x As Throwable) As AltResult
			Return New AltResult(If(TypeOf x Is java.util.concurrent.CompletionException, x, New java.util.concurrent.CompletionException(x)))
		End Function

		''' <summary>
		''' Completes with an exceptional result, unless already completed. </summary>
		Friend Function completeThrowable(  x As Throwable) As Boolean
			Return UNSAFE.compareAndSwapObject(Me, RESULT, Nothing, encodeThrowable(x))
		End Function

		''' <summary>
		''' Returns the encoding of the given (non-null) exception as a
		''' wrapped CompletionException unless it is one already.  May
		''' return the given Object r (which must have been the result of a
		''' source future) if it is equivalent, i.e. if this is a simple
		''' relay of an existing CompletionException.
		''' </summary>
		Friend Shared Function encodeThrowable(  x As Throwable,   r As Object) As Object
			If Not(TypeOf x Is java.util.concurrent.CompletionException) Then
				x = New java.util.concurrent.CompletionException(x)
			ElseIf TypeOf r Is AltResult AndAlso x Is CType(r, AltResult).ex Then
				Return r
			End If
			Return New AltResult(x)
		End Function

		''' <summary>
		''' Completes with the given (non-null) exceptional result as a
		''' wrapped CompletionException unless it is one already, unless
		''' already completed.  May complete with the given Object r
		''' (which must have been the result of a source future) if it is
		''' equivalent, i.e. if this is a simple propagation of an
		''' existing CompletionException.
		''' </summary>
		Friend Function completeThrowable(  x As Throwable,   r As Object) As Boolean
			Return UNSAFE.compareAndSwapObject(Me, RESULT, Nothing, encodeThrowable(x, r))
		End Function

		''' <summary>
		''' Returns the encoding of the given arguments: if the exception
		''' is non-null, encodes as AltResult.  Otherwise uses the given
		''' value, boxed as NIL if null.
		''' </summary>
		Friend Overridable Function encodeOutcome(  t As T,   x As Throwable) As Object
			Return If(x Is Nothing, If(t Is Nothing, NIL, t), encodeThrowable(x))
		End Function

		''' <summary>
		''' Returns the encoding of a copied outcome; if exceptional,
		''' rewraps as a CompletionException, else returns argument.
		''' </summary>
		Friend Shared Function encodeRelay(  r As Object) As Object
			Dim x As Throwable
'JAVA TO VB CONVERTER TODO TASK: Assignments within expressions are not supported in VB
			Return (If((TypeOf r Is AltResult) AndAlso (x = CType(r, AltResult).ex) IsNot Nothing AndAlso Not(TypeOf x Is java.util.concurrent.CompletionException), New AltResult(New java.util.concurrent.CompletionException(x)), r))
		End Function

		''' <summary>
		''' Completes with r or a copy of r, unless already completed.
		''' If exceptional, r is first coerced to a CompletionException.
		''' </summary>
		Friend Function completeRelay(  r As Object) As Boolean
			Return UNSAFE.compareAndSwapObject(Me, RESULT, Nothing, encodeRelay(r))
		End Function

		''' <summary>
		''' Reports result using Future.get conventions.
		''' </summary>
		Private Shared Function reportGet(Of T)(  r As Object) As T
			If r Is Nothing Then ' by convention below, null means interrupted Throw New InterruptedException
			If TypeOf r Is AltResult Then
				Dim x, cause As Throwable
				x = CType(r, AltResult).ex
				If x Is Nothing Then Return Nothing
				If TypeOf x Is java.util.concurrent.CancellationException Then Throw CType(x, java.util.concurrent.CancellationException)
				cause = x.cause
				If (TypeOf x Is java.util.concurrent.CompletionException) AndAlso cause IsNot Nothing Then x = cause
				Throw New java.util.concurrent.ExecutionException(x)
			End If
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
			Dim t As T = CType(r, T)
			Return t
		End Function

		''' <summary>
		''' Decodes outcome to return result or throw unchecked exception.
		''' </summary>
		Private Shared Function reportJoin(Of T)(  r As Object) As T
			If TypeOf r Is AltResult Then
				Dim x As Throwable
				x = CType(r, AltResult).ex
				If x Is Nothing Then Return Nothing
				If TypeOf x Is java.util.concurrent.CancellationException Then Throw CType(x, java.util.concurrent.CancellationException)
				If TypeOf x Is java.util.concurrent.CompletionException Then Throw CType(x, java.util.concurrent.CompletionException)
				Throw New java.util.concurrent.CompletionException(x)
			End If
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
			Dim t As T = CType(r, T)
			Return t
		End Function

		' ------------- Async task preliminaries -------------- 

		''' <summary>
		''' A marker interface identifying asynchronous tasks produced by
		''' {@code async} methods. This may be useful for monitoring,
		''' debugging, and tracking asynchronous activities.
		''' 
		''' @since 1.8
		''' </summary>
		Public Interface AsynchronousCompletionTask
		End Interface

		Private Shared ReadOnly useCommonPool As Boolean = (java.util.concurrent.ForkJoinPool.commonPoolParallelism > 1)

		''' <summary>
		''' Default executor -- ForkJoinPool.commonPool() unless it cannot
		''' support parallelism.
		''' </summary>
		Private Shared ReadOnly asyncPool As java.util.concurrent.Executor = If(useCommonPool, java.util.concurrent.ForkJoinPool.commonPool(), New ThreadPerTaskExecutor)

		''' <summary>
		''' Fallback if ForkJoinPool.commonPool() cannot support parallelism </summary>
		Friend NotInheritable Class ThreadPerTaskExecutor
			Implements java.util.concurrent.Executor

			Public Sub execute(  r As Runnable)
				CType(New Thread(r), Thread).start()
			End Sub
		End Class

		''' <summary>
		''' Null-checks user executor argument, and translates uses of
		''' commonPool to asyncPool in case parallelism disabled.
		''' </summary>
		Friend Shared Function screenExecutor(  e As java.util.concurrent.Executor) As java.util.concurrent.Executor
			If (Not useCommonPool) AndAlso e Is java.util.concurrent.ForkJoinPool.commonPool() Then Return asyncPool
			If e Is Nothing Then Throw New NullPointerException
			Return e
		End Function

		' Modes for Completion.tryFire. Signedness matters.
		Friend Const SYNC As Integer = 0
		Friend Const [ASYNC] As Integer = 1
		Friend Const NESTED As Integer = -1

		' ------------- Base Completion classes and operations -------------- 

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
		Friend MustInherit Class Completion
			Inherits java.util.concurrent.ForkJoinTask(Of Void)
			Implements Runnable, AsynchronousCompletionTask

'JAVA TO VB CONVERTER TODO TASK: There is no VB equivalent to 'volatile':
			Friend [next] As Completion ' Treiber stack link

			''' <summary>
			''' Performs completion action if triggered, returning a
			''' dependent that may need propagation, if one exists.
			''' </summary>
			''' <param name="mode"> SYNC, ASYNC, or NESTED </param>
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
			Friend MustOverride Function tryFire(  mode As Integer) As CompletableFuture(Of ?)

			''' <summary>
			''' Returns true if possibly still triggerable. Used by cleanStack. </summary>
			Friend MustOverride ReadOnly Property live As Boolean

			Public Sub run() Implements Runnable.run
				tryFire([ASYNC])
			End Sub
			Public Function exec() As Boolean
				tryFire([ASYNC])
				Return True
			End Function
			Public Property rawResult As Void
				Get
					Return Nothing
				End Get
				Set(  v As Void)
				End Set
			End Property
		End Class

		Friend Shared Sub lazySetNext(  c As Completion,   [next] As Completion)
			UNSAFE.putOrderedObject(c, CompletableFuture.NEXT, [next])
		End Sub

		''' <summary>
		''' Pops and tries to trigger all reachable dependents.  Call only
		''' when known to be done.
		''' </summary>
		Friend Sub postComplete()
	'        
	'         * On each step, variable f holds current dependents to pop
	'         * and run.  It is extended along only one path at a time,
	'         * pushing others to avoid unbounded recursion.
	'         
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
			Dim f As CompletableFuture(Of ?) = Me
			Dim h As Completion
			h = f.stack
'JAVA TO VB CONVERTER TODO TASK: Assignments within expressions are not supported in VB
			h = (f = Me).stack
			Do While h IsNot Nothing OrElse (f IsNot Me AndAlso h IsNot Nothing)
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
				Dim d As CompletableFuture(Of ?)
				Dim t As Completion
'JAVA TO VB CONVERTER TODO TASK: Assignments within expressions are not supported in VB
				If f.casStack(h, t = h.next) Then
					If t IsNot Nothing Then
						If f IsNot Me Then
							pushStack(h)
							h = f.stack
				h = (f = Me).stack
							Continue Do
						End If
						h.next = Nothing ' detach
					End If
'JAVA TO VB CONVERTER TODO TASK: Assignments within expressions are not supported in VB
					f = If((d = h.tryFire(NESTED)) Is Nothing, Me, d)
				End If
				h = f.stack
				h = (f = Me).stack
			Loop
		End Sub

		''' <summary>
		''' Traverses stack and unlinks dead Completions. </summary>
		Friend Sub cleanStack()
			Dim p As Completion = Nothing
			Dim q As Completion = stack
			Do While q IsNot Nothing
				Dim s As Completion = q.next
				If q.live Then
					p = q
					q = s
				ElseIf p Is Nothing Then
					casStack(q, s)
					q = stack
				Else
					p.next = s
					If p.live Then
						q = s
					Else
						p = Nothing ' restart
						q = stack
					End If
				End If
			Loop
		End Sub

		' ------------- One-input Completions -------------- 

		''' <summary>
		''' A Completion with a source, dependent, and executor. </summary>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
		Friend MustInherit Class UniCompletion(Of T, V)
			Inherits Completion

			Friend executor As java.util.concurrent.Executor ' executor to use (null if none)
			Friend dep As CompletableFuture(Of V) ' the dependent to complete
			Friend src As CompletableFuture(Of T) ' source for action

			Friend Sub New(  executor As java.util.concurrent.Executor,   dep As CompletableFuture(Of V),   src As CompletableFuture(Of T))
				Me.executor = executor
				Me.dep = dep
				Me.src = src
			End Sub

			''' <summary>
			''' Returns true if action can be run. Call only when known to
			''' be triggerable. Uses FJ tag bit to ensure that only one
			''' thread claims ownership.  If async, starts as task -- a
			''' later call to tryFire will run action.
			''' </summary>
			Friend Function claim() As Boolean
				Dim e As java.util.concurrent.Executor = executor
				If compareAndSetForkJoinTaskTag(CShort(0), CShort(1)) Then
					If e Is Nothing Then Return True
					executor = Nothing ' disable
					e.execute(Me)
				End If
				Return False
			End Function

			Friend Property NotOverridable Overrides live As Boolean
				Get
					Return dep IsNot Nothing
				End Get
			End Property
		End Class

		''' <summary>
		''' Pushes the given completion (if it exists) unless done. </summary>
		Friend Sub push(Of T1)(  c As UniCompletion(Of T1))
			If c IsNot Nothing Then
				Do While result Is Nothing AndAlso Not tryPushStack(c)
					lazySetNext(c, Nothing) ' clear on failure
				Loop
			End If
		End Sub

		''' <summary>
		''' Post-processing by dependent after successful UniCompletion
		''' tryFire.  Tries to clean stack of source a, and then either runs
		''' postComplete or returns this to caller, depending on mode.
		''' </summary>
		Friend Function postFire(Of T1)(  a As CompletableFuture(Of T1),   mode As Integer) As CompletableFuture(Of T)
			If a IsNot Nothing AndAlso a.stack IsNot Nothing Then
				If mode < 0 OrElse a.result Is Nothing Then
					a.cleanStack()
				Else
					a.postComplete()
				End If
			End If
			If result IsNot Nothing AndAlso stack IsNot Nothing Then
				If mode < 0 Then
					Return Me
				Else
					postComplete()
				End If
			End If
			Return Nothing
		End Function

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
		Friend NotInheritable Class UniApply(Of T, V)
			Inherits UniCompletion(Of T, V)

'JAVA TO VB CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
			Friend fn As java.util.function.Function(Of ?, ? As V)
'JAVA TO VB CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
			Friend Sub New(Of T1 As V)(  executor As java.util.concurrent.Executor,   dep As CompletableFuture(Of V),   src As CompletableFuture(Of T),   fn As java.util.function.Function(Of T1))
				MyBase.New(executor, dep, src)
				Me.fn = fn
			End Sub
			Friend Function tryFire(  mode As Integer) As CompletableFuture(Of V)
				Dim d As CompletableFuture(Of V)
				Dim a As CompletableFuture(Of T)
				d = dep
				a = src, fn,If(mode > 0, Nothing, Me)
				If d Is Nothing OrElse (Not d.uniApplya) Then Return Nothing
				dep = Nothing
				src = Nothing
				fn = Nothing
				Return d.postFire(a, mode)
			End Function
		End Class

'JAVA TO VB CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
		Friend Function uniApply(Of S, T1 As T)(  a As CompletableFuture(Of S),   f As java.util.function.Function(Of T1),   c As UniApply(Of S, T)) As Boolean
			Dim r As Object
			Dim x As Throwable
			r = a.result
			If a Is Nothing OrElse r Is Nothing OrElse f Is Nothing Then Return False
			tryComplete:
			If result Is Nothing Then
				If TypeOf r Is AltResult Then
					x = CType(r, AltResult).ex
					If x IsNot Nothing Then
						completeThrowable(x, r)
						GoTo tryComplete
					End If
					r = Nothing
				End If
				Try
					If c IsNot Nothing AndAlso (Not c.claim()) Then Return False
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
					Dim s As S = CType(r, S)
					completeValue(f.apply(s))
				Catch ex As Throwable
					completeThrowable(ex)
				End Try
			End If
			Return True
		End Function

'JAVA TO VB CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
		Private Function uniApplyStage(Of V, T1 As V)(  e As java.util.concurrent.Executor,   f As java.util.function.Function(Of T1)) As CompletableFuture(Of V)
			If f Is Nothing Then Throw New NullPointerException
			Dim d As New CompletableFuture(Of V)
			If e IsNot Nothing OrElse (Not d.uniApply(Me, f, Nothing)) Then
				Dim c As New UniApply(Of T, V)(e, d, Me, f)
				push(c)
				c.tryFire(SYNC)
			End If
			Return d
		End Function

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
		Friend NotInheritable Class UniAccept(Of T)
			Inherits UniCompletion(Of T, Void)

'JAVA TO VB CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
			Friend fn As java.util.function.Consumer(Of ?)
'JAVA TO VB CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
			Friend Sub New(Of T1)(  executor As java.util.concurrent.Executor,   dep As CompletableFuture(Of Void),   src As CompletableFuture(Of T),   fn As java.util.function.Consumer(Of T1))
				MyBase.New(executor, dep, src)
				Me.fn = fn
			End Sub
			Friend Function tryFire(  mode As Integer) As CompletableFuture(Of Void)
				Dim d As CompletableFuture(Of Void)
				Dim a As CompletableFuture(Of T)
				d = dep
				a = src, fn,If(mode > 0, Nothing, Me)
				If d Is Nothing OrElse (Not d.uniAccepta) Then Return Nothing
				dep = Nothing
				src = Nothing
				fn = Nothing
				Return d.postFire(a, mode)
			End Function
		End Class

'JAVA TO VB CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
		Friend Function uniAccept(Of S, T1)(  a As CompletableFuture(Of S),   f As java.util.function.Consumer(Of T1),   c As UniAccept(Of S)) As Boolean
			Dim r As Object
			Dim x As Throwable
			r = a.result
			If a Is Nothing OrElse r Is Nothing OrElse f Is Nothing Then Return False
			tryComplete:
			If result Is Nothing Then
				If TypeOf r Is AltResult Then
					x = CType(r, AltResult).ex
					If x IsNot Nothing Then
						completeThrowable(x, r)
						GoTo tryComplete
					End If
					r = Nothing
				End If
				Try
					If c IsNot Nothing AndAlso (Not c.claim()) Then Return False
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
					Dim s As S = CType(r, S)
					f.accept(s)
					completeNull()
				Catch ex As Throwable
					completeThrowable(ex)
				End Try
			End If
			Return True
		End Function

'JAVA TO VB CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
		Private Function uniAcceptStage(Of T1)(  e As java.util.concurrent.Executor,   f As java.util.function.Consumer(Of T1)) As CompletableFuture(Of Void)
			If f Is Nothing Then Throw New NullPointerException
			Dim d As New CompletableFuture(Of Void)
			If e IsNot Nothing OrElse (Not d.uniAccept(Me, f, Nothing)) Then
				Dim c As New UniAccept(Of T)(e, d, Me, f)
				push(c)
				c.tryFire(SYNC)
			End If
			Return d
		End Function

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
		Friend NotInheritable Class UniRun(Of T)
			Inherits UniCompletion(Of T, Void)

			Friend fn As Runnable
			Friend Sub New(  executor As java.util.concurrent.Executor,   dep As CompletableFuture(Of Void),   src As CompletableFuture(Of T),   fn As Runnable)
				MyBase.New(executor, dep, src)
				Me.fn = fn
			End Sub
			Friend Function tryFire(  mode As Integer) As CompletableFuture(Of Void)
				Dim d As CompletableFuture(Of Void)
				Dim a As CompletableFuture(Of T)
				d = dep
				a = src, fn,If(mode > 0, Nothing, Me)
				If d Is Nothing OrElse (Not d.uniRuna) Then Return Nothing
				dep = Nothing
				src = Nothing
				fn = Nothing
				Return d.postFire(a, mode)
			End Function
		End Class

		Friend Function uniRun(Of T1, T2)(  a As CompletableFuture(Of T1),   f As Runnable,   c As UniRun(Of T2)) As Boolean
			Dim r As Object
			Dim x As Throwable
			r = a.result
			If a Is Nothing OrElse r Is Nothing OrElse f Is Nothing Then Return False
			If result Is Nothing Then
				x = CType(r, AltResult).ex
				If TypeOf r Is AltResult AndAlso x IsNot Nothing Then
					completeThrowable(x, r)
				Else
					Try
						If c IsNot Nothing AndAlso (Not c.claim()) Then Return False
						f.run()
						completeNull()
					Catch ex As Throwable
						completeThrowable(ex)
					End Try
				End If
			End If
			Return True
		End Function

		Private Function uniRunStage(  e As java.util.concurrent.Executor,   f As Runnable) As CompletableFuture(Of Void)
			If f Is Nothing Then Throw New NullPointerException
			Dim d As New CompletableFuture(Of Void)
			If e IsNot Nothing OrElse (Not d.uniRun(Me, f, Nothing)) Then
				Dim c As New UniRun(Of T)(e, d, Me, f)
				push(c)
				c.tryFire(SYNC)
			End If
			Return d
		End Function

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
		Friend NotInheritable Class UniWhenComplete(Of T)
			Inherits UniCompletion(Of T, T)

'JAVA TO VB CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
			Friend fn As java.util.function.BiConsumer(Of ?, ?)
'JAVA TO VB CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
			Friend Sub New(Of T1)(  executor As java.util.concurrent.Executor,   dep As CompletableFuture(Of T),   src As CompletableFuture(Of T),   fn As java.util.function.BiConsumer(Of T1))
				MyBase.New(executor, dep, src)
				Me.fn = fn
			End Sub
			Friend Function tryFire(  mode As Integer) As CompletableFuture(Of T)
				Dim d As CompletableFuture(Of T)
				Dim a As CompletableFuture(Of T)
				d = dep
				a = src, fn,If(mode > 0, Nothing, Me)
				If d Is Nothing OrElse (Not d.uniWhenCompletea) Then Return Nothing
				dep = Nothing
				src = Nothing
				fn = Nothing
				Return d.postFire(a, mode)
			End Function
		End Class

'JAVA TO VB CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
		Friend Function uniWhenComplete(Of T1)(  a As CompletableFuture(Of T),   f As java.util.function.BiConsumer(Of T1),   c As UniWhenComplete(Of T)) As Boolean
			Dim r As Object
			Dim t As T
			Dim x As Throwable = Nothing
			r = a.result
			If a Is Nothing OrElse r Is Nothing OrElse f Is Nothing Then Return False
			If result Is Nothing Then
				Try
					If c IsNot Nothing AndAlso (Not c.claim()) Then Return False
					If TypeOf r Is AltResult Then
						x = CType(r, AltResult).ex
						t = Nothing
					Else
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
						Dim tr As T = CType(r, T)
						t = tr
					End If
					f.accept(t, x)
					If x Is Nothing Then
						internalComplete(r)
						Return True
					End If
				Catch ex As Throwable
					If x Is Nothing Then x = ex
				End Try
				completeThrowable(x, r)
			End If
			Return True
		End Function

'JAVA TO VB CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
		Private Function uniWhenCompleteStage(Of T1)(  e As java.util.concurrent.Executor,   f As java.util.function.BiConsumer(Of T1)) As CompletableFuture(Of T)
			If f Is Nothing Then Throw New NullPointerException
			Dim d As New CompletableFuture(Of T)
			If e IsNot Nothing OrElse (Not d.uniWhenComplete(Me, f, Nothing)) Then
				Dim c As New UniWhenComplete(Of T)(e, d, Me, f)
				push(c)
				c.tryFire(SYNC)
			End If
			Return d
		End Function

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
		Friend NotInheritable Class UniHandle(Of T, V)
			Inherits UniCompletion(Of T, V)

'JAVA TO VB CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
			Friend fn As java.util.function.BiFunction(Of ?, Throwable, ? As V)
'JAVA TO VB CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
			Friend Sub New(Of T1 As V)(  executor As java.util.concurrent.Executor,   dep As CompletableFuture(Of V),   src As CompletableFuture(Of T),   fn As java.util.function.BiFunction(Of T1))
				MyBase.New(executor, dep, src)
				Me.fn = fn
			End Sub
			Friend Function tryFire(  mode As Integer) As CompletableFuture(Of V)
				Dim d As CompletableFuture(Of V)
				Dim a As CompletableFuture(Of T)
				d = dep
				a = src, fn,If(mode > 0, Nothing, Me)
				If d Is Nothing OrElse (Not d.uniHandlea) Then Return Nothing
				dep = Nothing
				src = Nothing
				fn = Nothing
				Return d.postFire(a, mode)
			End Function
		End Class

'JAVA TO VB CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
		Friend Function uniHandle(Of S, T1 As T)(  a As CompletableFuture(Of S),   f As java.util.function.BiFunction(Of T1),   c As UniHandle(Of S, T)) As Boolean
			Dim r As Object
			Dim s As S
			Dim x As Throwable
			r = a.result
			If a Is Nothing OrElse r Is Nothing OrElse f Is Nothing Then Return False
			If result Is Nothing Then
				Try
					If c IsNot Nothing AndAlso (Not c.claim()) Then Return False
					If TypeOf r Is AltResult Then
						x = CType(r, AltResult).ex
						s = Nothing
					Else
						x = Nothing
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
						Dim ss As S = CType(r, S)
						s = ss
					End If
					completeValue(f.apply(s, x))
				Catch ex As Throwable
					completeThrowable(ex)
				End Try
			End If
			Return True
		End Function

'JAVA TO VB CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
		Private Function uniHandleStage(Of V, T1 As V)(  e As java.util.concurrent.Executor,   f As java.util.function.BiFunction(Of T1)) As CompletableFuture(Of V)
			If f Is Nothing Then Throw New NullPointerException
			Dim d As New CompletableFuture(Of V)
			If e IsNot Nothing OrElse (Not d.uniHandle(Me, f, Nothing)) Then
				Dim c As New UniHandle(Of T, V)(e, d, Me, f)
				push(c)
				c.tryFire(SYNC)
			End If
			Return d
		End Function

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
		Friend NotInheritable Class UniExceptionally(Of T)
			Inherits UniCompletion(Of T, T)

'JAVA TO VB CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
			Friend fn As java.util.function.Function(Of ?, ? As T)
'JAVA TO VB CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
			Friend Sub New(Of T1 As T)(  dep As CompletableFuture(Of T),   src As CompletableFuture(Of T),   fn As java.util.function.Function(Of T1))
				MyBase.New(Nothing, dep, src)
				Me.fn = fn
			End Sub
			Friend Function tryFire(  mode As Integer) As CompletableFuture(Of T) ' never ASYNC
				' assert mode != ASYNC;
				Dim d As CompletableFuture(Of T)
				Dim a As CompletableFuture(Of T)
				d = dep
				a = src, fn, Me
				If d Is Nothing OrElse (Not d.uniExceptionallya) Then Return Nothing
				dep = Nothing
				src = Nothing
				fn = Nothing
				Return d.postFire(a, mode)
			End Function
		End Class

'JAVA TO VB CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
		Friend Function uniExceptionally(Of T1 As T)(  a As CompletableFuture(Of T),   f As java.util.function.Function(Of T1),   c As UniExceptionally(Of T)) As Boolean
			Dim r As Object
			Dim x As Throwable
			r = a.result
			If a Is Nothing OrElse r Is Nothing OrElse f Is Nothing Then Return False
			If result Is Nothing Then
				Try
					x = CType(r, AltResult).ex
					If TypeOf r Is AltResult AndAlso x IsNot Nothing Then
						If c IsNot Nothing AndAlso (Not c.claim()) Then Return False
						completeValue(f.apply(x))
					Else
						internalComplete(r)
					End If
				Catch ex As Throwable
					completeThrowable(ex)
				End Try
			End If
			Return True
		End Function

		Private Function uniExceptionallyStage(Of T1 As T)(  f As java.util.function.Function(Of T1)) As CompletableFuture(Of T)
			If f Is Nothing Then Throw New NullPointerException
			Dim d As New CompletableFuture(Of T)
			If Not d.uniExceptionally(Me, f, Nothing) Then
				Dim c As New UniExceptionally(Of T)(d, Me, f)
				push(c)
				c.tryFire(SYNC)
			End If
			Return d
		End Function

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
		Friend NotInheritable Class UniRelay(Of T)
			Inherits UniCompletion(Of T, T)
 ' for Compose
			Friend Sub New(  dep As CompletableFuture(Of T),   src As CompletableFuture(Of T))
				MyBase.New(Nothing, dep, src)
			End Sub
			Friend Function tryFire(  mode As Integer) As CompletableFuture(Of T)
				Dim d As CompletableFuture(Of T)
				Dim a As CompletableFuture(Of T)
				d = dep
				a = src
				If d Is Nothing OrElse (Not d.uniRelaya) Then Return Nothing
				src = Nothing
				dep = Nothing
				Return d.postFire(a, mode)
			End Function
		End Class

		Friend Function uniRelay(  a As CompletableFuture(Of T)) As Boolean
			Dim r As Object
			r = a.result
			If a Is Nothing OrElse r Is Nothing Then Return False
			If result Is Nothing Then ' no need to claim completeRelay(r)
			Return True
		End Function

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
		Friend NotInheritable Class UniCompose(Of T, V)
			Inherits UniCompletion(Of T, V)

'JAVA TO VB CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
			Friend fn As java.util.function.Function(Of ?, ? As java.util.concurrent.CompletionStage(Of V))
'JAVA TO VB CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
			Friend Sub New(Of T1 As java.util.concurrent.CompletionStage(Of V)(  executor As java.util.concurrent.Executor,   dep As CompletableFuture(Of V),   src As CompletableFuture(Of T),   fn As java.util.function.Function(Of T1))
				MyBase.New(executor, dep, src)
				Me.fn = fn
			End Sub
			Friend Function tryFire(  mode As Integer) As CompletableFuture(Of V)
				Dim d As CompletableFuture(Of V)
				Dim a As CompletableFuture(Of T)
				d = dep
				a = src, fn,If(mode > 0, Nothing, Me)
				If d Is Nothing OrElse (Not d.uniComposea) Then Return Nothing
				dep = Nothing
				src = Nothing
				fn = Nothing
				Return d.postFire(a, mode)
			End Function
		End Class

'JAVA TO VB CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
		Friend Function uniCompose(Of S, T1 As java.util.concurrent.CompletionStage(Of T)(  a As CompletableFuture(Of S),   f As java.util.function.Function(Of T1),   c As UniCompose(Of S, T)) As Boolean
			Dim r As Object
			Dim x As Throwable
			r = a.result
			If a Is Nothing OrElse r Is Nothing OrElse f Is Nothing Then Return False
			tryComplete:
			If result Is Nothing Then
				If TypeOf r Is AltResult Then
					x = CType(r, AltResult).ex
					If x IsNot Nothing Then
						completeThrowable(x, r)
						GoTo tryComplete
					End If
					r = Nothing
				End If
				Try
					If c IsNot Nothing AndAlso (Not c.claim()) Then Return False
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
					Dim s As S = CType(r, S)
					Dim g As CompletableFuture(Of T) = f.apply(s).toCompletableFuture()
					If g.result Is Nothing OrElse (Not uniRelay(g)) Then
						Dim copy As New UniRelay(Of T)(Me, g)
						g.push(copy)
						copy.tryFire(SYNC)
						If result Is Nothing Then Return False
					End If
				Catch ex As Throwable
					completeThrowable(ex)
				End Try
			End If
			Return True
		End Function

'JAVA TO VB CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
		Private Function uniComposeStage(Of V, T1 As java.util.concurrent.CompletionStage(Of V)(  e As java.util.concurrent.Executor,   f As java.util.function.Function(Of T1)) As CompletableFuture(Of V)
			If f Is Nothing Then Throw New NullPointerException
			Dim r As Object
			Dim x As Throwable
			r = result
			If e Is Nothing AndAlso r IsNot Nothing Then
				' try to return function result directly
				If TypeOf r Is AltResult Then
					x = CType(r, AltResult).ex
					If x IsNot Nothing Then Return New CompletableFuture(Of V)(encodeThrowable(x, r))
					r = Nothing
				End If
				Try
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
					Dim t As T = CType(r, T)
					Dim g As CompletableFuture(Of V) = f.apply(t).toCompletableFuture()
					Dim s As Object = g.result
					If s IsNot Nothing Then Return New CompletableFuture(Of V)(encodeRelay(s))
					Dim d As New CompletableFuture(Of V)
					Dim copy As New UniRelay(Of V)(d, g)
					g.push(copy)
					copy.tryFire(SYNC)
					Return d
				Catch ex As Throwable
					Return New CompletableFuture(Of V)(encodeThrowable(ex))
				End Try
			End If
			Dim d As New CompletableFuture(Of V)
			Dim c As New UniCompose(Of T, V)(e, d, Me, f)
			push(c)
			c.tryFire(SYNC)
			Return d
		End Function

		' ------------- Two-input Completions -------------- 

		''' <summary>
		''' A Completion for an action with two sources </summary>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
		Friend MustInherit Class BiCompletion(Of T, U, V)
			Inherits UniCompletion(Of T, V)

			Friend snd As CompletableFuture(Of U) ' second source for action
			Friend Sub New(  executor As java.util.concurrent.Executor,   dep As CompletableFuture(Of V),   src As CompletableFuture(Of T),   snd As CompletableFuture(Of U))
				MyBase.New(executor, dep, src)
				Me.snd = snd
			End Sub
		End Class

		''' <summary>
		''' A Completion delegating to a BiCompletion </summary>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
		Friend NotInheritable Class CoCompletion
			Inherits Completion

'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
			Friend base As BiCompletion(Of ?, ?, ?)
			Friend Sub New(Of T1)(  base As BiCompletion(Of T1))
				Me.base = base
			End Sub
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
			Friend NotOverridable Overrides Function tryFire(  mode As Integer) As CompletableFuture(Of ?)
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
				Dim c As BiCompletion(Of ?, ?, ?)
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
				Dim d As CompletableFuture(Of ?)
				c = base
				d = c.tryFire(mode)
				If c Is Nothing OrElse d Is Nothing Then Return Nothing
				base = Nothing ' detach
				Return d
			End Function
			Friend Property NotOverridable Overrides live As Boolean
				Get
	'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
					Dim c As BiCompletion(Of ?, ?, ?)
	'JAVA TO VB CONVERTER TODO TASK: Assignments within expressions are not supported in VB
					Return (c = base) IsNot Nothing AndAlso c.dep IsNot Nothing
				End Get
			End Property
		End Class

		''' <summary>
		''' Pushes completion to this and b unless both done. </summary>
		Friend Sub bipush(Of T1, T2)(  b As CompletableFuture(Of T1),   c As BiCompletion(Of T2))
			If c IsNot Nothing Then
				Dim r As Object
				r = result
				Do While r Is Nothing AndAlso Not tryPushStack(c)
					lazySetNext(c, Nothing) ' clear on failure
					r = result
				Loop
				If b IsNot Nothing AndAlso b IsNot Me AndAlso b.result Is Nothing Then
					Dim q As Completion = If(r IsNot Nothing, c, New CoCompletion(c))
					Do While b.result Is Nothing AndAlso Not b.tryPushStack(q)
						lazySetNext(q, Nothing) ' clear on failure
					Loop
				End If
			End If
		End Sub

		''' <summary>
		''' Post-processing after successful BiCompletion tryFire. </summary>
		Friend Function postFire(Of T1, T2)(  a As CompletableFuture(Of T1),   b As CompletableFuture(Of T2),   mode As Integer) As CompletableFuture(Of T)
			If b IsNot Nothing AndAlso b.stack IsNot Nothing Then ' clean second source
				If mode < 0 OrElse b.result Is Nothing Then
					b.cleanStack()
				Else
					b.postComplete()
				End If
			End If
			Return postFire(a, mode)
		End Function

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
		Friend NotInheritable Class BiApply(Of T, U, V)
			Inherits BiCompletion(Of T, U, V)

'JAVA TO VB CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
			Friend fn As java.util.function.BiFunction(Of ?, ?, ? As V)
'JAVA TO VB CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
			Friend Sub New(Of T1 As V)(  executor As java.util.concurrent.Executor,   dep As CompletableFuture(Of V),   src As CompletableFuture(Of T),   snd As CompletableFuture(Of U),   fn As java.util.function.BiFunction(Of T1))
				MyBase.New(executor, dep, src, snd)
				Me.fn = fn
			End Sub
			Friend Function tryFire(  mode As Integer) As CompletableFuture(Of V)
				Dim d As CompletableFuture(Of V)
				Dim a As CompletableFuture(Of T)
				Dim b As CompletableFuture(Of U)
				d = dep
				src, b = snd, fn,If(mode > 0, Nothing, Me)
				a = src, b
				If d Is Nothing OrElse (Not d.biApplya) Then Return Nothing
				dep = Nothing
				src = Nothing
				snd = Nothing
				fn = Nothing
				Return d.postFire(a, b, mode)
			End Function
		End Class

'JAVA TO VB CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
		Friend Function biApply(Of R, S, T1 As T)(  a As CompletableFuture(Of R),   b As CompletableFuture(Of S),   f As java.util.function.BiFunction(Of T1),   c As BiApply(Of R, S, T)) As Boolean
			Dim r, s As Object
			Dim x As Throwable
			r = a.result
			s = b.result
			If a Is Nothing OrElse r Is Nothing OrElse b Is Nothing OrElse s Is Nothing OrElse f Is Nothing Then Return False
			tryComplete:
			If result Is Nothing Then
				If TypeOf r Is AltResult Then
					x = CType(r, AltResult).ex
					If x IsNot Nothing Then
						completeThrowable(x, r)
						GoTo tryComplete
					End If
					r = Nothing
				End If
				If TypeOf s Is AltResult Then
					x = CType(s, AltResult).ex
					If x IsNot Nothing Then
						completeThrowable(x, s)
						GoTo tryComplete
					End If
					s = Nothing
				End If
				Try
					If c IsNot Nothing AndAlso (Not c.claim()) Then Return False
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
					Dim rr As R = CType(r, R)
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
					Dim ss As S = CType(s, S)
					completeValue(f.apply(rr, ss))
				Catch ex As Throwable
					completeThrowable(ex)
				End Try
			End If
			Return True
		End Function

'JAVA TO VB CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
		Private Function biApplyStage(Of U, V, T1 As V)(  e As java.util.concurrent.Executor,   o As java.util.concurrent.CompletionStage(Of U),   f As java.util.function.BiFunction(Of T1)) As CompletableFuture(Of V)
			Dim b As CompletableFuture(Of U)
			b = o.toCompletableFuture()
			If f Is Nothing OrElse b Is Nothing Then Throw New NullPointerException
			Dim d As New CompletableFuture(Of V)
			If e IsNot Nothing OrElse (Not d.biApply(Me, b, f, Nothing)) Then
				Dim c As New BiApply(Of T, U, V)(e, d, Me, b, f)
				bipush(b, c)
				c.tryFire(SYNC)
			End If
			Return d
		End Function

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
		Friend NotInheritable Class BiAccept(Of T, U)
			Inherits BiCompletion(Of T, U, Void)

'JAVA TO VB CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
			Friend fn As java.util.function.BiConsumer(Of ?, ?)
'JAVA TO VB CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
			Friend Sub New(Of T1)(  executor As java.util.concurrent.Executor,   dep As CompletableFuture(Of Void),   src As CompletableFuture(Of T),   snd As CompletableFuture(Of U),   fn As java.util.function.BiConsumer(Of T1))
				MyBase.New(executor, dep, src, snd)
				Me.fn = fn
			End Sub
			Friend Function tryFire(  mode As Integer) As CompletableFuture(Of Void)
				Dim d As CompletableFuture(Of Void)
				Dim a As CompletableFuture(Of T)
				Dim b As CompletableFuture(Of U)
				d = dep
				src, b = snd, fn,If(mode > 0, Nothing, Me)
				a = src, b
				If d Is Nothing OrElse (Not d.biAccepta) Then Return Nothing
				dep = Nothing
				src = Nothing
				snd = Nothing
				fn = Nothing
				Return d.postFire(a, b, mode)
			End Function
		End Class

'JAVA TO VB CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
		Friend Function biAccept(Of R, S, T1)(  a As CompletableFuture(Of R),   b As CompletableFuture(Of S),   f As java.util.function.BiConsumer(Of T1),   c As BiAccept(Of R, S)) As Boolean
			Dim r, s As Object
			Dim x As Throwable
			r = a.result
			s = b.result
			If a Is Nothing OrElse r Is Nothing OrElse b Is Nothing OrElse s Is Nothing OrElse f Is Nothing Then Return False
			tryComplete:
			If result Is Nothing Then
				If TypeOf r Is AltResult Then
					x = CType(r, AltResult).ex
					If x IsNot Nothing Then
						completeThrowable(x, r)
						GoTo tryComplete
					End If
					r = Nothing
				End If
				If TypeOf s Is AltResult Then
					x = CType(s, AltResult).ex
					If x IsNot Nothing Then
						completeThrowable(x, s)
						GoTo tryComplete
					End If
					s = Nothing
				End If
				Try
					If c IsNot Nothing AndAlso (Not c.claim()) Then Return False
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
					Dim rr As R = CType(r, R)
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
					Dim ss As S = CType(s, S)
					f.accept(rr, ss)
					completeNull()
				Catch ex As Throwable
					completeThrowable(ex)
				End Try
			End If
			Return True
		End Function

'JAVA TO VB CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
		Private Function biAcceptStage(Of U, T1)(  e As java.util.concurrent.Executor,   o As java.util.concurrent.CompletionStage(Of U),   f As java.util.function.BiConsumer(Of T1)) As CompletableFuture(Of Void)
			Dim b As CompletableFuture(Of U)
			b = o.toCompletableFuture()
			If f Is Nothing OrElse b Is Nothing Then Throw New NullPointerException
			Dim d As New CompletableFuture(Of Void)
			If e IsNot Nothing OrElse (Not d.biAccept(Me, b, f, Nothing)) Then
				Dim c As New BiAccept(Of T, U)(e, d, Me, b, f)
				bipush(b, c)
				c.tryFire(SYNC)
			End If
			Return d
		End Function

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
		Friend NotInheritable Class BiRun(Of T, U)
			Inherits BiCompletion(Of T, U, Void)

			Friend fn As Runnable
			Friend Sub New(  executor As java.util.concurrent.Executor,   dep As CompletableFuture(Of Void),   src As CompletableFuture(Of T),   snd As CompletableFuture(Of U),   fn As Runnable)
				MyBase.New(executor, dep, src, snd)
				Me.fn = fn
			End Sub
			Friend Function tryFire(  mode As Integer) As CompletableFuture(Of Void)
				Dim d As CompletableFuture(Of Void)
				Dim a As CompletableFuture(Of T)
				Dim b As CompletableFuture(Of U)
				d = dep
				src, b = snd, fn,If(mode > 0, Nothing, Me)
				a = src, b
				If d Is Nothing OrElse (Not d.biRuna) Then Return Nothing
				dep = Nothing
				src = Nothing
				snd = Nothing
				fn = Nothing
				Return d.postFire(a, b, mode)
			End Function
		End Class

		Friend Function biRun(Of T1, T2, T3)(  a As CompletableFuture(Of T1),   b As CompletableFuture(Of T2),   f As Runnable,   c As BiRun(Of T3)) As Boolean
			Dim r, s As Object
			Dim x As Throwable
			r = a.result
			s = b.result
			If a Is Nothing OrElse r Is Nothing OrElse b Is Nothing OrElse s Is Nothing OrElse f Is Nothing Then Return False
			If result Is Nothing Then
				x = CType(r, AltResult).ex
				If TypeOf r Is AltResult AndAlso x IsNot Nothing Then
					completeThrowable(x, r)
				Else
					x = CType(s, AltResult).ex
					If TypeOf s Is AltResult AndAlso x IsNot Nothing Then
						completeThrowable(x, s)
					Else
						Try
					End If
						If c IsNot Nothing AndAlso (Not c.claim()) Then Return False
						f.run()
						completeNull()
					Catch ex As Throwable
						completeThrowable(ex)
					End Try
					End If
			End If
			Return True
		End Function

		Private Function biRunStage(Of T1)(  e As java.util.concurrent.Executor,   o As java.util.concurrent.CompletionStage(Of T1),   f As Runnable) As CompletableFuture(Of Void)
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
			Dim b As CompletableFuture(Of ?)
			b = o.toCompletableFuture()
			If f Is Nothing OrElse b Is Nothing Then Throw New NullPointerException
			Dim d As New CompletableFuture(Of Void)
			If e IsNot Nothing OrElse (Not d.biRun(Me, b, f, Nothing)) Then
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
				Dim c As New BiRun(Of T, ?)(e, d, Me, b, f)
				bipush(b, c)
				c.tryFire(SYNC)
			End If
			Return d
		End Function

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
		Friend NotInheritable Class BiRelay(Of T, U)
			Inherits BiCompletion(Of T, U, Void)
 ' for And
			Friend Sub New(  dep As CompletableFuture(Of Void),   src As CompletableFuture(Of T),   snd As CompletableFuture(Of U))
				MyBase.New(Nothing, dep, src, snd)
			End Sub
			Friend Function tryFire(  mode As Integer) As CompletableFuture(Of Void)
				Dim d As CompletableFuture(Of Void)
				Dim a As CompletableFuture(Of T)
				Dim b As CompletableFuture(Of U)
				d = dep
				src, b = snd
				a = src, b
				If d Is Nothing OrElse (Not d.biRelaya) Then Return Nothing
				src = Nothing
				snd = Nothing
				dep = Nothing
				Return d.postFire(a, b, mode)
			End Function
		End Class

		Friend Overridable Function biRelay(Of T1, T2)(  a As CompletableFuture(Of T1),   b As CompletableFuture(Of T2)) As Boolean
			Dim r, s As Object
			Dim x As Throwable
			r = a.result
			s = b.result
			If a Is Nothing OrElse r Is Nothing OrElse b Is Nothing OrElse s Is Nothing Then Return False
			If result Is Nothing Then
				x = CType(r, AltResult).ex
				If TypeOf r Is AltResult AndAlso x IsNot Nothing Then
					completeThrowable(x, r)
				Else
					x = CType(s, AltResult).ex
					If TypeOf s Is AltResult AndAlso x IsNot Nothing Then
						completeThrowable(x, s)
					Else
						completeNull()
					End If
					End If
			End If
			Return True
		End Function

		''' <summary>
		''' Recursively constructs a tree of completions. </summary>
		Shared Function andTree(Of T1)(  cfs As CompletableFuture(Of T1)(),   lo As Integer,   hi As Integer) As CompletableFuture(Of Void)
			Dim d As New CompletableFuture(Of Void)
			If lo > hi Then ' empty
				d.result = NIL
			Else
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
				Dim a As CompletableFuture(Of ?), b As CompletableFuture(Of ?)
				Dim mid As Integer = CInt(CUInt((lo + hi)) >> 1)
				a = (If(lo = mid, cfs(lo), andTree(cfs, lo, mid)))
				b = (If(lo = hi, a, If(hi = mid+1, cfs(hi), andTree(cfs, mid+1, hi))))
				If a Is Nothing OrElse b Is Nothing Then Throw New NullPointerException
				If Not d.biRelay(a, b) Then
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
					Dim c As New BiRelay(Of ?, ?)(d, a, b)
					a.bipush(b, c)
					c.tryFire(SYNC)
				End If
			End If
			Return d
		End Function

		' ------------- Projected (Ored) BiCompletions -------------- 

		''' <summary>
		''' Pushes completion to this and b unless either done. </summary>
		Friend Sub orpush(Of T1, T2)(  b As CompletableFuture(Of T1),   c As BiCompletion(Of T2))
			If c IsNot Nothing Then
				Do While (b Is Nothing OrElse b.result Is Nothing) AndAlso result Is Nothing
					If tryPushStack(c) Then
						If b IsNot Nothing AndAlso b IsNot Me AndAlso b.result Is Nothing Then
							Dim q As Completion = New CoCompletion(c)
							Do While result Is Nothing AndAlso b.result Is Nothing AndAlso Not b.tryPushStack(q)
								lazySetNext(q, Nothing) ' clear on failure
							Loop
						End If
						Exit Do
					End If
					lazySetNext(c, Nothing) ' clear on failure
				Loop
			End If
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
		Friend NotInheritable Class OrApply(Of T, U As T, V)
			Inherits BiCompletion(Of T, U, V)

'JAVA TO VB CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
			Friend fn As java.util.function.Function(Of ?, ? As V)
'JAVA TO VB CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
			Friend Sub New(Of T1 As V)(  executor As java.util.concurrent.Executor,   dep As CompletableFuture(Of V),   src As CompletableFuture(Of T),   snd As CompletableFuture(Of U),   fn As java.util.function.Function(Of T1))
				MyBase.New(executor, dep, src, snd)
				Me.fn = fn
			End Sub
			Friend Function tryFire(  mode As Integer) As CompletableFuture(Of V)
				Dim d As CompletableFuture(Of V)
				Dim a As CompletableFuture(Of T)
				Dim b As CompletableFuture(Of U)
				d = dep
				src, b = snd, fn,If(mode > 0, Nothing, Me)
				a = src, b
				If d Is Nothing OrElse (Not d.orApplya) Then Return Nothing
				dep = Nothing
				src = Nothing
				snd = Nothing
				fn = Nothing
				Return d.postFire(a, b, mode)
			End Function
		End Class

'JAVA TO VB CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
		Friend Function orApply(Of R, S As R, T1 As T)(  a As CompletableFuture(Of R),   b As CompletableFuture(Of S),   f As java.util.function.Function(Of T1),   c As OrApply(Of R, S, T)) As Boolean
			Dim r As Object
			Dim x As Throwable
			r = a.result
			r = b.result
			If a Is Nothing OrElse b Is Nothing OrElse (r Is Nothing AndAlso r Is Nothing) OrElse f Is Nothing Then Return False
			tryComplete:
			If result Is Nothing Then
				Try
					If c IsNot Nothing AndAlso (Not c.claim()) Then Return False
					If TypeOf r Is AltResult Then
						x = CType(r, AltResult).ex
						If x IsNot Nothing Then
							completeThrowable(x, r)
							GoTo tryComplete
						End If
						r = Nothing
					End If
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
					Dim rr As R = CType(r, R)
					completeValue(f.apply(rr))
				Catch ex As Throwable
					completeThrowable(ex)
				End Try
			End If
			Return True
		End Function

'JAVA TO VB CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
		Private Function orApplyStage(Of U As T, V, T1 As V)(  e As java.util.concurrent.Executor,   o As java.util.concurrent.CompletionStage(Of U),   f As java.util.function.Function(Of T1)) As CompletableFuture(Of V)
			Dim b As CompletableFuture(Of U)
			b = o.toCompletableFuture()
			If f Is Nothing OrElse b Is Nothing Then Throw New NullPointerException
			Dim d As New CompletableFuture(Of V)
			If e IsNot Nothing OrElse (Not d.orApply(Me, b, f, Nothing)) Then
				Dim c As New OrApply(Of T, U, V)(e, d, Me, b, f)
				orpush(b, c)
				c.tryFire(SYNC)
			End If
			Return d
		End Function

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
		Friend NotInheritable Class OrAccept(Of T, U As T)
			Inherits BiCompletion(Of T, U, Void)

'JAVA TO VB CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
			Friend fn As java.util.function.Consumer(Of ?)
'JAVA TO VB CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
			Friend Sub New(Of T1)(  executor As java.util.concurrent.Executor,   dep As CompletableFuture(Of Void),   src As CompletableFuture(Of T),   snd As CompletableFuture(Of U),   fn As java.util.function.Consumer(Of T1))
				MyBase.New(executor, dep, src, snd)
				Me.fn = fn
			End Sub
			Friend Function tryFire(  mode As Integer) As CompletableFuture(Of Void)
				Dim d As CompletableFuture(Of Void)
				Dim a As CompletableFuture(Of T)
				Dim b As CompletableFuture(Of U)
				d = dep
				src, b = snd, fn,If(mode > 0, Nothing, Me)
				a = src, b
				If d Is Nothing OrElse (Not d.orAccepta) Then Return Nothing
				dep = Nothing
				src = Nothing
				snd = Nothing
				fn = Nothing
				Return d.postFire(a, b, mode)
			End Function
		End Class

'JAVA TO VB CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
		Friend Function orAccept(Of R, S As R, T1)(  a As CompletableFuture(Of R),   b As CompletableFuture(Of S),   f As java.util.function.Consumer(Of T1),   c As OrAccept(Of R, S)) As Boolean
			Dim r As Object
			Dim x As Throwable
			r = a.result
			r = b.result
			If a Is Nothing OrElse b Is Nothing OrElse (r Is Nothing AndAlso r Is Nothing) OrElse f Is Nothing Then Return False
			tryComplete:
			If result Is Nothing Then
				Try
					If c IsNot Nothing AndAlso (Not c.claim()) Then Return False
					If TypeOf r Is AltResult Then
						x = CType(r, AltResult).ex
						If x IsNot Nothing Then
							completeThrowable(x, r)
							GoTo tryComplete
						End If
						r = Nothing
					End If
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
					Dim rr As R = CType(r, R)
					f.accept(rr)
					completeNull()
				Catch ex As Throwable
					completeThrowable(ex)
				End Try
			End If
			Return True
		End Function

'JAVA TO VB CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
		Private Function orAcceptStage(Of U As T, T1)(  e As java.util.concurrent.Executor,   o As java.util.concurrent.CompletionStage(Of U),   f As java.util.function.Consumer(Of T1)) As CompletableFuture(Of Void)
			Dim b As CompletableFuture(Of U)
			b = o.toCompletableFuture()
			If f Is Nothing OrElse b Is Nothing Then Throw New NullPointerException
			Dim d As New CompletableFuture(Of Void)
			If e IsNot Nothing OrElse (Not d.orAccept(Me, b, f, Nothing)) Then
				Dim c As New OrAccept(Of T, U)(e, d, Me, b, f)
				orpush(b, c)
				c.tryFire(SYNC)
			End If
			Return d
		End Function

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
		Friend NotInheritable Class OrRun(Of T, U)
			Inherits BiCompletion(Of T, U, Void)

			Friend fn As Runnable
			Friend Sub New(  executor As java.util.concurrent.Executor,   dep As CompletableFuture(Of Void),   src As CompletableFuture(Of T),   snd As CompletableFuture(Of U),   fn As Runnable)
				MyBase.New(executor, dep, src, snd)
				Me.fn = fn
			End Sub
			Friend Function tryFire(  mode As Integer) As CompletableFuture(Of Void)
				Dim d As CompletableFuture(Of Void)
				Dim a As CompletableFuture(Of T)
				Dim b As CompletableFuture(Of U)
				d = dep
				src, b = snd, fn,If(mode > 0, Nothing, Me)
				a = src, b
				If d Is Nothing OrElse (Not d.orRuna) Then Return Nothing
				dep = Nothing
				src = Nothing
				snd = Nothing
				fn = Nothing
				Return d.postFire(a, b, mode)
			End Function
		End Class

		Friend Function orRun(Of T1, T2, T3)(  a As CompletableFuture(Of T1),   b As CompletableFuture(Of T2),   f As Runnable,   c As OrRun(Of T3)) As Boolean
			Dim r As Object
			Dim x As Throwable
			r = a.result
			r = b.result
			If a Is Nothing OrElse b Is Nothing OrElse (r Is Nothing AndAlso r Is Nothing) OrElse f Is Nothing Then Return False
			If result Is Nothing Then
				Try
					If c IsNot Nothing AndAlso (Not c.claim()) Then Return False
					x = CType(r, AltResult).ex
					If TypeOf r Is AltResult AndAlso x IsNot Nothing Then
						completeThrowable(x, r)
					Else
						f.run()
						completeNull()
					End If
				Catch ex As Throwable
					completeThrowable(ex)
				End Try
			End If
			Return True
		End Function

		Private Function orRunStage(Of T1)(  e As java.util.concurrent.Executor,   o As java.util.concurrent.CompletionStage(Of T1),   f As Runnable) As CompletableFuture(Of Void)
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
			Dim b As CompletableFuture(Of ?)
			b = o.toCompletableFuture()
			If f Is Nothing OrElse b Is Nothing Then Throw New NullPointerException
			Dim d As New CompletableFuture(Of Void)
			If e IsNot Nothing OrElse (Not d.orRun(Me, b, f, Nothing)) Then
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
				Dim c As New OrRun(Of T, ?)(e, d, Me, b, f)
				orpush(b, c)
				c.tryFire(SYNC)
			End If
			Return d
		End Function

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
		Friend NotInheritable Class OrRelay(Of T, U)
			Inherits BiCompletion(Of T, U, Object)
 ' for Or
			Friend Sub New(  dep As CompletableFuture(Of Object),   src As CompletableFuture(Of T),   snd As CompletableFuture(Of U))
				MyBase.New(Nothing, dep, src, snd)
			End Sub
			Friend Function tryFire(  mode As Integer) As CompletableFuture(Of Object)
				Dim d As CompletableFuture(Of Object)
				Dim a As CompletableFuture(Of T)
				Dim b As CompletableFuture(Of U)
				d = dep
				src, b = snd
				a = src, b
				If d Is Nothing OrElse (Not d.orRelaya) Then Return Nothing
				src = Nothing
				snd = Nothing
				dep = Nothing
				Return d.postFire(a, b, mode)
			End Function
		End Class

		Friend Function orRelay(Of T1, T2)(  a As CompletableFuture(Of T1),   b As CompletableFuture(Of T2)) As Boolean
			Dim r As Object
			r = a.result
			r = b.result
			If a Is Nothing OrElse b Is Nothing OrElse (r Is Nothing AndAlso r Is Nothing) Then Return False
			If result Is Nothing Then completeRelay(r)
			Return True
		End Function

		''' <summary>
		''' Recursively constructs a tree of completions. </summary>
		Shared Function orTree(Of T1)(  cfs As CompletableFuture(Of T1)(),   lo As Integer,   hi As Integer) As CompletableFuture(Of Object)
			Dim d As New CompletableFuture(Of Object)
			If lo <= hi Then
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
				Dim a As CompletableFuture(Of ?), b As CompletableFuture(Of ?)
				Dim mid As Integer = CInt(CUInt((lo + hi)) >> 1)
				a = (If(lo = mid, cfs(lo), orTree(cfs, lo, mid)))
				b = (If(lo = hi, a, If(hi = mid+1, cfs(hi), orTree(cfs, mid+1, hi))))
				If a Is Nothing OrElse b Is Nothing Then Throw New NullPointerException
				If Not d.orRelay(a, b) Then
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
					Dim c As New OrRelay(Of ?, ?)(d, a, b)
					a.orpush(b, c)
					c.tryFire(SYNC)
				End If
			End If
			Return d
		End Function

		' ------------- Zero-input Async forms -------------- 

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
		Friend NotInheritable Class AsyncSupply(Of T)
			Inherits java.util.concurrent.ForkJoinTask(Of Void)
			Implements Runnable, AsynchronousCompletionTask

			Friend dep As CompletableFuture(Of T)
			Friend fn As java.util.function.Supplier(Of T)
			Friend Sub New(  dep As CompletableFuture(Of T),   fn As java.util.function.Supplier(Of T))
				Me.dep = dep
				Me.fn = fn
			End Sub

			Public Property rawResult As Void
				Get
					Return Nothing
				End Get
				Set(  v As Void)
				End Set
			End Property
			Public Function exec() As Boolean
				run()
				Return True
			End Function

			Public Sub run() Implements Runnable.run
				Dim d As CompletableFuture(Of T)
				Dim f As java.util.function.Supplier(Of T)
				d = dep
				f = fn
				If d IsNot Nothing AndAlso f IsNot Nothing Then
					dep = Nothing
					fn = Nothing
					If d.result Is Nothing Then
						Try
							d.completeValue(f.get())
						Catch ex As Throwable
							d.completeThrowable(ex)
						End Try
					End If
					d.postComplete()
				End If
			End Sub
		End Class

		Shared Function asyncSupplyStage(Of U)(  e As java.util.concurrent.Executor,   f As java.util.function.Supplier(Of U)) As CompletableFuture(Of U)
			If f Is Nothing Then Throw New NullPointerException
			Dim d As New CompletableFuture(Of U)
			e.execute(New AsyncSupply(Of U)(d, f))
			Return d
		End Function

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
		Friend NotInheritable Class AsyncRun
			Inherits java.util.concurrent.ForkJoinTask(Of Void)
			Implements Runnable, AsynchronousCompletionTask

			Friend dep As CompletableFuture(Of Void)
			Friend fn As Runnable
			Friend Sub New(  dep As CompletableFuture(Of Void),   fn As Runnable)
				Me.dep = dep
				Me.fn = fn
			End Sub

			Public Property rawResult As Void
				Get
					Return Nothing
				End Get
				Set(  v As Void)
				End Set
			End Property
			Public Function exec() As Boolean
				run()
				Return True
			End Function

			Public Sub run() Implements Runnable.run
				Dim d As CompletableFuture(Of Void)
				Dim f As Runnable
				d = dep
				f = fn
				If d IsNot Nothing AndAlso f IsNot Nothing Then
					dep = Nothing
					fn = Nothing
					If d.result Is Nothing Then
						Try
							f.run()
							d.completeNull()
						Catch ex As Throwable
							d.completeThrowable(ex)
						End Try
					End If
					d.postComplete()
				End If
			End Sub
		End Class

		Shared Function asyncRunStage(  e As java.util.concurrent.Executor,   f As Runnable) As CompletableFuture(Of Void)
			If f Is Nothing Then Throw New NullPointerException
			Dim d As New CompletableFuture(Of Void)
			e.execute(New AsyncRun(d, f))
			Return d
		End Function

		' ------------- Signallers -------------- 

		''' <summary>
		''' Completion for recording and releasing a waiting thread.  This
		''' class implements ManagedBlocker to avoid starvation when
		''' blocking actions pile up in ForkJoinPools.
		''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
		Friend NotInheritable Class Signaller
			Inherits Completion
			Implements java.util.concurrent.ForkJoinPool.ManagedBlocker

			Friend nanos As Long ' wait time if timed
			Friend ReadOnly deadline As Long ' non-zero if timed
'JAVA TO VB CONVERTER TODO TASK: There is no VB equivalent to 'volatile':
			Friend interruptControl As Integer ' > 0: interruptible, < 0: interrupted
'JAVA TO VB CONVERTER TODO TASK: There is no VB equivalent to 'volatile':
			Friend thread_Renamed As Thread

			Friend Sub New(  interruptible As Boolean,   nanos As Long,   deadline As Long)
				Me.thread_Renamed = Thread.CurrentThread
				Me.interruptControl = If(interruptible, 1, 0)
				Me.nanos = nanos
				Me.deadline = deadline
			End Sub
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
			Friend NotOverridable Overrides Function tryFire(  ignore As Integer) As CompletableFuture(Of ?)
				Dim w As Thread ' no need to atomically claim
				w = thread_Renamed
				If w IsNot Nothing Then
					thread_Renamed = Nothing
					java.util.concurrent.locks.LockSupport.unpark(w)
				End If
				Return Nothing
			End Function
			Public Property releasable As Boolean
				Get
					If thread_Renamed Is Nothing Then Return True
					If Thread.interrupted() Then
						Dim i As Integer = interruptControl
						interruptControl = -1
						If i > 0 Then Return True
					End If
					nanos = deadline - System.nanoTime()
					If deadline <> 0L AndAlso (nanos <= 0L OrElse nanos <= 0L) Then
						thread_Renamed = Nothing
						Return True
					End If
					Return False
				End Get
			End Property
			Public Function block() As Boolean
				If releasable Then
					Return True
				ElseIf deadline = 0L Then
					java.util.concurrent.locks.LockSupport.park(Me)
				ElseIf nanos > 0L Then
					java.util.concurrent.locks.LockSupport.parkNanos(Me, nanos)
				End If
				Return releasable
			End Function
			Friend Property NotOverridable Overrides live As Boolean
				Get
					Return thread_Renamed IsNot Nothing
				End Get
			End Property
		End Class

		''' <summary>
		''' Returns raw result after waiting, or null if interruptible and
		''' interrupted.
		''' </summary>
		Private Function waitingGet(  interruptible As Boolean) As Object
			Dim q As Signaller = Nothing
			Dim queued As Boolean = False
			Dim spins As Integer = -1
			Dim r As Object
			r = result
			Do While r Is Nothing
				If spins < 0 Then
					spins = If(Runtime.runtime.availableProcessors() > 1, 1 << 8, 0) ' Use brief spin-wait on multiprocessors
				ElseIf spins > 0 Then
					If java.util.concurrent.ThreadLocalRandom.nextSecondarySeed() >= 0 Then spins -= 1
				ElseIf q Is Nothing Then
					q = New Signaller(interruptible, 0L, 0L)
				ElseIf Not queued Then
					queued = tryPushStack(q)
				ElseIf interruptible AndAlso q.interruptControl < 0 Then
					q.thread_Renamed = Nothing
					cleanStack()
					Return Nothing
				ElseIf q.thread_Renamed IsNot Nothing AndAlso result Is Nothing Then
					Try
						java.util.concurrent.ForkJoinPool.managedBlock(q)
					Catch ie As InterruptedException
						q.interruptControl = -1
					End Try
				End If
				r = result
			Loop
			If q IsNot Nothing Then
				q.thread_Renamed = Nothing
				If q.interruptControl < 0 Then
					If interruptible Then
						r = Nothing ' report interruption
					Else
						Thread.CurrentThread.Interrupt()
					End If
				End If
			End If
			postComplete()
			Return r
		End Function

		''' <summary>
		''' Returns raw result after waiting, or null if interrupted, or
		''' throws TimeoutException on timeout.
		''' </summary>
		Private Function timedGet(  nanos As Long) As Object
			If Thread.interrupted() Then Return Nothing
			If nanos <= 0L Then Throw New java.util.concurrent.TimeoutException
			Dim d As Long = System.nanoTime() + nanos
			Dim q As New Signaller(True, nanos,If(d = 0L, 1L, d)) ' avoid 0
			Dim queued As Boolean = False
			Dim r As Object
			' We intentionally don't spin here (as waitingGet does) because
			' the call to nanoTime() above acts much like a spin.
			r = result
			Do While r Is Nothing
				If Not queued Then
					queued = tryPushStack(q)
				ElseIf q.interruptControl < 0 OrElse q.nanos <= 0L Then
					q.thread_Renamed = Nothing
					cleanStack()
					If q.interruptControl < 0 Then Return Nothing
					Throw New java.util.concurrent.TimeoutException
				ElseIf q.thread_Renamed IsNot Nothing AndAlso result Is Nothing Then
					Try
						java.util.concurrent.ForkJoinPool.managedBlock(q)
					Catch ie As InterruptedException
						q.interruptControl = -1
					End Try
				End If
				r = result
			Loop
			If q.interruptControl < 0 Then r = Nothing
			q.thread_Renamed = Nothing
			postComplete()
			Return r
		End Function

		' ------------- public methods -------------- 

		''' <summary>
		''' Creates a new incomplete CompletableFuture.
		''' </summary>
		Public Sub New()
		End Sub

		''' <summary>
		''' Creates a new complete CompletableFuture with given encoded result.
		''' </summary>
		Private Sub New(  r As Object)
			Me.result = r
		End Sub

		''' <summary>
		''' Returns a new CompletableFuture that is asynchronously completed
		''' by a task running in the <seealso cref="ForkJoinPool#commonPool()"/> with
		''' the value obtained by calling the given Supplier.
		''' </summary>
		''' <param name="supplier"> a function returning the value to be used
		''' to complete the returned CompletableFuture </param>
		''' @param <U> the function's return type </param>
		''' <returns> the new CompletableFuture </returns>
		Public Shared Function supplyAsync(Of U)(  supplier As java.util.function.Supplier(Of U)) As CompletableFuture(Of U)
			Return asyncSupplyStage(asyncPool, supplier)
		End Function

		''' <summary>
		''' Returns a new CompletableFuture that is asynchronously completed
		''' by a task running in the given executor with the value obtained
		''' by calling the given Supplier.
		''' </summary>
		''' <param name="supplier"> a function returning the value to be used
		''' to complete the returned CompletableFuture </param>
		''' <param name="executor"> the executor to use for asynchronous execution </param>
		''' @param <U> the function's return type </param>
		''' <returns> the new CompletableFuture </returns>
		Public Shared Function supplyAsync(Of U)(  supplier As java.util.function.Supplier(Of U),   executor As java.util.concurrent.Executor) As CompletableFuture(Of U)
			Return asyncSupplyStage(screenExecutor(executor), supplier)
		End Function

		''' <summary>
		''' Returns a new CompletableFuture that is asynchronously completed
		''' by a task running in the <seealso cref="ForkJoinPool#commonPool()"/> after
		''' it runs the given action.
		''' </summary>
		''' <param name="runnable"> the action to run before completing the
		''' returned CompletableFuture </param>
		''' <returns> the new CompletableFuture </returns>
		Public Shared Function runAsync(  runnable As Runnable) As CompletableFuture(Of Void)
			Return asyncRunStage(asyncPool, runnable)
		End Function

		''' <summary>
		''' Returns a new CompletableFuture that is asynchronously completed
		''' by a task running in the given executor after it runs the given
		''' action.
		''' </summary>
		''' <param name="runnable"> the action to run before completing the
		''' returned CompletableFuture </param>
		''' <param name="executor"> the executor to use for asynchronous execution </param>
		''' <returns> the new CompletableFuture </returns>
		Public Shared Function runAsync(  runnable As Runnable,   executor As java.util.concurrent.Executor) As CompletableFuture(Of Void)
			Return asyncRunStage(screenExecutor(executor), runnable)
		End Function

		''' <summary>
		''' Returns a new CompletableFuture that is already completed with
		''' the given value.
		''' </summary>
		''' <param name="value"> the value </param>
		''' @param <U> the type of the value </param>
		''' <returns> the completed CompletableFuture </returns>
		Public Shared Function completedFuture(Of U)(  value As U) As CompletableFuture(Of U)
			Return New CompletableFuture(Of U)(If(value Is Nothing, NIL, value))
		End Function

		''' <summary>
		''' Returns {@code true} if completed in any fashion: normally,
		''' exceptionally, or via cancellation.
		''' </summary>
		''' <returns> {@code true} if completed </returns>
		Public Overridable Property done As Boolean
			Get
				Return result IsNot Nothing
			End Get
		End Property

		''' <summary>
		''' Waits if necessary for this future to complete, and then
		''' returns its result.
		''' </summary>
		''' <returns> the result value </returns>
		''' <exception cref="CancellationException"> if this future was cancelled </exception>
		''' <exception cref="ExecutionException"> if this future completed exceptionally </exception>
		''' <exception cref="InterruptedException"> if the current thread was interrupted
		''' while waiting </exception>
		Public Overridable Function [get]() As T
			Dim r As Object
'JAVA TO VB CONVERTER TODO TASK: Assignments within expressions are not supported in VB
			Return reportGet(If((r = result) Is Nothing, waitingGet(True), r))
		End Function

		''' <summary>
		''' Waits if necessary for at most the given time for this future
		''' to complete, and then returns its result, if available.
		''' </summary>
		''' <param name="timeout"> the maximum time to wait </param>
		''' <param name="unit"> the time unit of the timeout argument </param>
		''' <returns> the result value </returns>
		''' <exception cref="CancellationException"> if this future was cancelled </exception>
		''' <exception cref="ExecutionException"> if this future completed exceptionally </exception>
		''' <exception cref="InterruptedException"> if the current thread was interrupted
		''' while waiting </exception>
		''' <exception cref="TimeoutException"> if the wait timed out </exception>
		Public Overridable Function [get](  timeout As Long,   unit As java.util.concurrent.TimeUnit) As T
			Dim r As Object
			Dim nanos As Long = unit.toNanos(timeout)
'JAVA TO VB CONVERTER TODO TASK: Assignments within expressions are not supported in VB
			Return reportGet(If((r = result) Is Nothing, timedGet(nanos), r))
		End Function

		''' <summary>
		''' Returns the result value when complete, or throws an
		''' (unchecked) exception if completed exceptionally. To better
		''' conform with the use of common functional forms, if a
		''' computation involved in the completion of this
		''' CompletableFuture threw an exception, this method throws an
		''' (unchecked) <seealso cref="CompletionException"/> with the underlying
		''' exception as its cause.
		''' </summary>
		''' <returns> the result value </returns>
		''' <exception cref="CancellationException"> if the computation was cancelled </exception>
		''' <exception cref="CompletionException"> if this future completed
		''' exceptionally or a completion computation threw an exception </exception>
		Public Overridable Function join() As T
			Dim r As Object
'JAVA TO VB CONVERTER TODO TASK: Assignments within expressions are not supported in VB
			Return reportJoin(If((r = result) Is Nothing, waitingGet(False), r))
		End Function

		''' <summary>
		''' Returns the result value (or throws any encountered exception)
		''' if completed, else returns the given valueIfAbsent.
		''' </summary>
		''' <param name="valueIfAbsent"> the value to return if not completed </param>
		''' <returns> the result value, if completed, else the given valueIfAbsent </returns>
		''' <exception cref="CancellationException"> if the computation was cancelled </exception>
		''' <exception cref="CompletionException"> if this future completed
		''' exceptionally or a completion computation threw an exception </exception>
		Public Overridable Function getNow(  valueIfAbsent As T) As T
			Dim r As Object
'JAVA TO VB CONVERTER TODO TASK: Assignments within expressions are not supported in VB
			Return If((r = result) Is Nothing, valueIfAbsent, reportJoin(r))
		End Function

		''' <summary>
		''' If not already completed, sets the value returned by {@link
		''' #get()} and related methods to the given value.
		''' </summary>
		''' <param name="value"> the result value </param>
		''' <returns> {@code true} if this invocation caused this CompletableFuture
		''' to transition to a completed state, else {@code false} </returns>
		Public Overridable Function complete(  value As T) As Boolean
			Dim triggered As Boolean = completeValue(value)
			postComplete()
			Return triggered
		End Function

		''' <summary>
		''' If not already completed, causes invocations of <seealso cref="#get()"/>
		''' and related methods to throw the given exception.
		''' </summary>
		''' <param name="ex"> the exception </param>
		''' <returns> {@code true} if this invocation caused this CompletableFuture
		''' to transition to a completed state, else {@code false} </returns>
		Public Overridable Function completeExceptionally(  ex As Throwable) As Boolean
			If ex Is Nothing Then Throw New NullPointerException
			Dim triggered As Boolean = internalComplete(New AltResult(ex))
			postComplete()
			Return triggered
		End Function

'JAVA TO VB CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
		Public Overridable Function thenApply(Of U, T1 As U)(  fn As java.util.function.Function(Of T1)) As CompletableFuture(Of U)
			Return uniApplyStage(Nothing, fn)
		End Function

'JAVA TO VB CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
		Public Overridable Function thenApplyAsync(Of U, T1 As U)(  fn As java.util.function.Function(Of T1)) As CompletableFuture(Of U)
			Return uniApplyStage(asyncPool, fn)
		End Function

'JAVA TO VB CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
		Public Overridable Function thenApplyAsync(Of U, T1 As U)(  fn As java.util.function.Function(Of T1),   executor As java.util.concurrent.Executor) As CompletableFuture(Of U)
			Return uniApplyStage(screenExecutor(executor), fn)
		End Function

'JAVA TO VB CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
		Public Overridable Function thenAccept(Of T1)(  action As java.util.function.Consumer(Of T1)) As CompletableFuture(Of Void)
			Return uniAcceptStage(Nothing, action)
		End Function

'JAVA TO VB CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
		Public Overridable Function thenAcceptAsync(Of T1)(  action As java.util.function.Consumer(Of T1)) As CompletableFuture(Of Void)
			Return uniAcceptStage(asyncPool, action)
		End Function

'JAVA TO VB CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
		Public Overridable Function thenAcceptAsync(Of T1)(  action As java.util.function.Consumer(Of T1),   executor As java.util.concurrent.Executor) As CompletableFuture(Of Void)
			Return uniAcceptStage(screenExecutor(executor), action)
		End Function

		Public Overridable Function thenRun(  action As Runnable) As CompletableFuture(Of Void)
			Return uniRunStage(Nothing, action)
		End Function

		Public Overridable Function thenRunAsync(  action As Runnable) As CompletableFuture(Of Void)
			Return uniRunStage(asyncPool, action)
		End Function

		Public Overridable Function thenRunAsync(  action As Runnable,   executor As java.util.concurrent.Executor) As CompletableFuture(Of Void)
			Return uniRunStage(screenExecutor(executor), action)
		End Function

'JAVA TO VB CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
		Public Overridable Function thenCombine(Of U, V, T1 As U, T2 As V)(  other As java.util.concurrent.CompletionStage(Of T1),   fn As java.util.function.BiFunction(Of T2)) As CompletableFuture(Of V)
			Return biApplyStage(Nothing, other, fn)
		End Function

'JAVA TO VB CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
		Public Overridable Function thenCombineAsync(Of U, V, T1 As U, T2 As V)(  other As java.util.concurrent.CompletionStage(Of T1),   fn As java.util.function.BiFunction(Of T2)) As CompletableFuture(Of V)
			Return biApplyStage(asyncPool, other, fn)
		End Function

'JAVA TO VB CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
		Public Overridable Function thenCombineAsync(Of U, V, T1 As U, T2 As V)(  other As java.util.concurrent.CompletionStage(Of T1),   fn As java.util.function.BiFunction(Of T2),   executor As java.util.concurrent.Executor) As CompletableFuture(Of V)
			Return biApplyStage(screenExecutor(executor), other, fn)
		End Function

'JAVA TO VB CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
		Public Overridable Function thenAcceptBoth(Of U, T1 As U, T2)(  other As java.util.concurrent.CompletionStage(Of T1),   action As java.util.function.BiConsumer(Of T2)) As CompletableFuture(Of Void)
			Return biAcceptStage(Nothing, other, action)
		End Function

'JAVA TO VB CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
		Public Overridable Function thenAcceptBothAsync(Of U, T1 As U, T2)(  other As java.util.concurrent.CompletionStage(Of T1),   action As java.util.function.BiConsumer(Of T2)) As CompletableFuture(Of Void)
			Return biAcceptStage(asyncPool, other, action)
		End Function

'JAVA TO VB CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
		Public Overridable Function thenAcceptBothAsync(Of U, T1 As U, T2)(  other As java.util.concurrent.CompletionStage(Of T1),   action As java.util.function.BiConsumer(Of T2),   executor As java.util.concurrent.Executor) As CompletableFuture(Of Void)
			Return biAcceptStage(screenExecutor(executor), other, action)
		End Function

		Public Overridable Function runAfterBoth(Of T1)(  other As java.util.concurrent.CompletionStage(Of T1),   action As Runnable) As CompletableFuture(Of Void)
			Return biRunStage(Nothing, other, action)
		End Function

		Public Overridable Function runAfterBothAsync(Of T1)(  other As java.util.concurrent.CompletionStage(Of T1),   action As Runnable) As CompletableFuture(Of Void)
			Return biRunStage(asyncPool, other, action)
		End Function

		Public Overridable Function runAfterBothAsync(Of T1)(  other As java.util.concurrent.CompletionStage(Of T1),   action As Runnable,   executor As java.util.concurrent.Executor) As CompletableFuture(Of Void)
			Return biRunStage(screenExecutor(executor), other, action)
		End Function

'JAVA TO VB CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
		Public Overridable Function applyToEither(Of U, T1 As T, T2)(  other As java.util.concurrent.CompletionStage(Of T1),   fn As java.util.function.Function(Of T2)) As CompletableFuture(Of U)
			Return orApplyStage(Nothing, other, fn)
		End Function

'JAVA TO VB CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
		Public Overridable Function applyToEitherAsync(Of U, T1 As T, T2)(  other As java.util.concurrent.CompletionStage(Of T1),   fn As java.util.function.Function(Of T2)) As CompletableFuture(Of U)
			Return orApplyStage(asyncPool, other, fn)
		End Function

'JAVA TO VB CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
		Public Overridable Function applyToEitherAsync(Of U, T1 As T, T2)(  other As java.util.concurrent.CompletionStage(Of T1),   fn As java.util.function.Function(Of T2),   executor As java.util.concurrent.Executor) As CompletableFuture(Of U)
			Return orApplyStage(screenExecutor(executor), other, fn)
		End Function

'JAVA TO VB CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
		Public Overridable Function acceptEither(Of T1 As T, T2)(  other As java.util.concurrent.CompletionStage(Of T1),   action As java.util.function.Consumer(Of T2)) As CompletableFuture(Of Void)
			Return orAcceptStage(Nothing, other, action)
		End Function

'JAVA TO VB CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
		Public Overridable Function acceptEitherAsync(Of T1 As T, T2)(  other As java.util.concurrent.CompletionStage(Of T1),   action As java.util.function.Consumer(Of T2)) As CompletableFuture(Of Void)
			Return orAcceptStage(asyncPool, other, action)
		End Function

'JAVA TO VB CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
		Public Overridable Function acceptEitherAsync(Of T1 As T, T2)(  other As java.util.concurrent.CompletionStage(Of T1),   action As java.util.function.Consumer(Of T2),   executor As java.util.concurrent.Executor) As CompletableFuture(Of Void)
			Return orAcceptStage(screenExecutor(executor), other, action)
		End Function

		Public Overridable Function runAfterEither(Of T1)(  other As java.util.concurrent.CompletionStage(Of T1),   action As Runnable) As CompletableFuture(Of Void)
			Return orRunStage(Nothing, other, action)
		End Function

		Public Overridable Function runAfterEitherAsync(Of T1)(  other As java.util.concurrent.CompletionStage(Of T1),   action As Runnable) As CompletableFuture(Of Void)
			Return orRunStage(asyncPool, other, action)
		End Function

		Public Overridable Function runAfterEitherAsync(Of T1)(  other As java.util.concurrent.CompletionStage(Of T1),   action As Runnable,   executor As java.util.concurrent.Executor) As CompletableFuture(Of Void)
			Return orRunStage(screenExecutor(executor), other, action)
		End Function

'JAVA TO VB CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
		Public Overridable Function thenCompose(Of U, T1 As java.util.concurrent.CompletionStage(Of U)(  fn As java.util.function.Function(Of T1)) As CompletableFuture(Of U)
			Return uniComposeStage(Nothing, fn)
		End Function

'JAVA TO VB CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
		Public Overridable Function thenComposeAsync(Of U, T1 As java.util.concurrent.CompletionStage(Of U)(  fn As java.util.function.Function(Of T1)) As CompletableFuture(Of U)
			Return uniComposeStage(asyncPool, fn)
		End Function

'JAVA TO VB CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
		Public Overridable Function thenComposeAsync(Of U, T1 As java.util.concurrent.CompletionStage(Of U)(  fn As java.util.function.Function(Of T1),   executor As java.util.concurrent.Executor) As CompletableFuture(Of U)
			Return uniComposeStage(screenExecutor(executor), fn)
		End Function

'JAVA TO VB CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
		Public Overridable Function whenComplete(Of T1)(  action As java.util.function.BiConsumer(Of T1)) As CompletableFuture(Of T)
			Return uniWhenCompleteStage(Nothing, action)
		End Function

'JAVA TO VB CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
		Public Overridable Function whenCompleteAsync(Of T1)(  action As java.util.function.BiConsumer(Of T1)) As CompletableFuture(Of T)
			Return uniWhenCompleteStage(asyncPool, action)
		End Function

'JAVA TO VB CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
		Public Overridable Function whenCompleteAsync(Of T1)(  action As java.util.function.BiConsumer(Of T1),   executor As java.util.concurrent.Executor) As CompletableFuture(Of T)
			Return uniWhenCompleteStage(screenExecutor(executor), action)
		End Function

'JAVA TO VB CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
		Public Overridable Function handle(Of U, T1 As U)(  fn As java.util.function.BiFunction(Of T1)) As CompletableFuture(Of U)
			Return uniHandleStage(Nothing, fn)
		End Function

'JAVA TO VB CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
		Public Overridable Function handleAsync(Of U, T1 As U)(  fn As java.util.function.BiFunction(Of T1)) As CompletableFuture(Of U)
			Return uniHandleStage(asyncPool, fn)
		End Function

'JAVA TO VB CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
		Public Overridable Function handleAsync(Of U, T1 As U)(  fn As java.util.function.BiFunction(Of T1),   executor As java.util.concurrent.Executor) As CompletableFuture(Of U)
			Return uniHandleStage(screenExecutor(executor), fn)
		End Function

		''' <summary>
		''' Returns this CompletableFuture.
		''' </summary>
		''' <returns> this CompletableFuture </returns>
		Public Overridable Function toCompletableFuture() As CompletableFuture(Of T)
			Return Me
		End Function

		' not in interface CompletionStage

		''' <summary>
		''' Returns a new CompletableFuture that is completed when this
		''' CompletableFuture completes, with the result of the given
		''' function of the exception triggering this CompletableFuture's
		''' completion when it completes exceptionally; otherwise, if this
		''' CompletableFuture completes normally, then the returned
		''' CompletableFuture also completes normally with the same value.
		''' Note: More flexible versions of this functionality are
		''' available using methods {@code whenComplete} and {@code handle}.
		''' </summary>
		''' <param name="fn"> the function to use to compute the value of the
		''' returned CompletableFuture if this CompletableFuture completed
		''' exceptionally </param>
		''' <returns> the new CompletableFuture </returns>
		Public Overridable Function exceptionally(Of T1 As T)(  fn As java.util.function.Function(Of T1)) As CompletableFuture(Of T)
			Return uniExceptionallyStage(fn)
		End Function

		' ------------- Arbitrary-arity constructions -------------- 

		''' <summary>
		''' Returns a new CompletableFuture that is completed when all of
		''' the given CompletableFutures complete.  If any of the given
		''' CompletableFutures complete exceptionally, then the returned
		''' CompletableFuture also does so, with a CompletionException
		''' holding this exception as its cause.  Otherwise, the results,
		''' if any, of the given CompletableFutures are not reflected in
		''' the returned CompletableFuture, but may be obtained by
		''' inspecting them individually. If no CompletableFutures are
		''' provided, returns a CompletableFuture completed with the value
		''' {@code null}.
		''' 
		''' <p>Among the applications of this method is to await completion
		''' of a set of independent CompletableFutures before continuing a
		''' program, as in: {@code CompletableFuture.allOf(c1, c2,
		''' c3).join();}.
		''' </summary>
		''' <param name="cfs"> the CompletableFutures </param>
		''' <returns> a new CompletableFuture that is completed when all of the
		''' given CompletableFutures complete </returns>
		''' <exception cref="NullPointerException"> if the array or any of its elements are
		''' {@code null} </exception>
		Public Shared Function allOf(Of T1)(ParamArray   cfs As CompletableFuture(Of T1)()) As CompletableFuture(Of Void)
			Return andTree(cfs, 0, cfs.Length - 1)
		End Function

		''' <summary>
		''' Returns a new CompletableFuture that is completed when any of
		''' the given CompletableFutures complete, with the same result.
		''' Otherwise, if it completed exceptionally, the returned
		''' CompletableFuture also does so, with a CompletionException
		''' holding this exception as its cause.  If no CompletableFutures
		''' are provided, returns an incomplete CompletableFuture.
		''' </summary>
		''' <param name="cfs"> the CompletableFutures </param>
		''' <returns> a new CompletableFuture that is completed with the
		''' result or exception of any of the given CompletableFutures when
		''' one completes </returns>
		''' <exception cref="NullPointerException"> if the array or any of its elements are
		''' {@code null} </exception>
		Public Shared Function anyOf(Of T1)(ParamArray   cfs As CompletableFuture(Of T1)()) As CompletableFuture(Of Object)
			Return orTree(cfs, 0, cfs.Length - 1)
		End Function

		' ------------- Control and status methods -------------- 

		''' <summary>
		''' If not already completed, completes this CompletableFuture with
		''' a <seealso cref="CancellationException"/>. Dependent CompletableFutures
		''' that have not already completed will also complete
		''' exceptionally, with a <seealso cref="CompletionException"/> caused by
		''' this {@code CancellationException}.
		''' </summary>
		''' <param name="mayInterruptIfRunning"> this value has no effect in this
		''' implementation because interrupts are not used to control
		''' processing.
		''' </param>
		''' <returns> {@code true} if this task is now cancelled </returns>
		Public Overridable Function cancel(  mayInterruptIfRunning As Boolean) As Boolean
			Dim cancelled_Renamed As Boolean = (result Is Nothing) AndAlso internalComplete(New AltResult(New java.util.concurrent.CancellationException))
			postComplete()
			Return cancelled_Renamed OrElse cancelled
		End Function

		''' <summary>
		''' Returns {@code true} if this CompletableFuture was cancelled
		''' before it completed normally.
		''' </summary>
		''' <returns> {@code true} if this CompletableFuture was cancelled
		''' before it completed normally </returns>
		Public Overridable Property cancelled As Boolean
			Get
				Dim r As Object
	'JAVA TO VB CONVERTER TODO TASK: Assignments within expressions are not supported in VB
				Return (TypeOf (r = result) Is AltResult) AndAlso (TypeOf CType(r, AltResult).ex Is java.util.concurrent.CancellationException)
			End Get
		End Property

		''' <summary>
		''' Returns {@code true} if this CompletableFuture completed
		''' exceptionally, in any way. Possible causes include
		''' cancellation, explicit invocation of {@code
		''' completeExceptionally}, and abrupt termination of a
		''' CompletionStage action.
		''' </summary>
		''' <returns> {@code true} if this CompletableFuture completed
		''' exceptionally </returns>
		Public Overridable Property completedExceptionally As Boolean
			Get
				Dim r As Object
	'JAVA TO VB CONVERTER TODO TASK: Assignments within expressions are not supported in VB
				Return (TypeOf (r = result) Is AltResult) AndAlso r IsNot NIL
			End Get
		End Property

		''' <summary>
		''' Forcibly sets or resets the value subsequently returned by
		''' method <seealso cref="#get()"/> and related methods, whether or not
		''' already completed. This method is designed for use only in
		''' error recovery actions, and even in such situations may result
		''' in ongoing dependent completions using established versus
		''' overwritten outcomes.
		''' </summary>
		''' <param name="value"> the completion value </param>
		Public Overridable Sub obtrudeValue(  value As T)
			result = If(value Is Nothing, NIL, value)
			postComplete()
		End Sub

		''' <summary>
		''' Forcibly causes subsequent invocations of method <seealso cref="#get()"/>
		''' and related methods to throw the given exception, whether or
		''' not already completed. This method is designed for use only in
		''' error recovery actions, and even in such situations may result
		''' in ongoing dependent completions using established versus
		''' overwritten outcomes.
		''' </summary>
		''' <param name="ex"> the exception </param>
		''' <exception cref="NullPointerException"> if the exception is null </exception>
		Public Overridable Sub obtrudeException(  ex As Throwable)
			If ex Is Nothing Then Throw New NullPointerException
			result = New AltResult(ex)
			postComplete()
		End Sub

		''' <summary>
		''' Returns the estimated number of CompletableFutures whose
		''' completions are awaiting completion of this CompletableFuture.
		''' This method is designed for use in monitoring system state, not
		''' for synchronization control.
		''' </summary>
		''' <returns> the number of dependent CompletableFutures </returns>
		Public Overridable Property numberOfDependents As Integer
			Get
				Dim count As Integer = 0
				Dim p As Completion = stack
				Do While p IsNot Nothing
					count += 1
					p = p.next
				Loop
				Return count
			End Get
		End Property

		''' <summary>
		''' Returns a string identifying this CompletableFuture, as well as
		''' its completion state.  The state, in brackets, contains the
		''' String {@code "Completed Normally"} or the String {@code
		''' "Completed Exceptionally"}, or the String {@code "Not
		''' completed"} followed by the number of CompletableFutures
		''' dependent upon its completion, if any.
		''' </summary>
		''' <returns> a string identifying this CompletableFuture, as well as its state </returns>
		Public Overrides Function ToString() As String
			Dim r As Object = result
			Dim count As Integer
'JAVA TO VB CONVERTER TODO TASK: Assignments within expressions are not supported in VB
			Return MyBase.ToString() & (If(r Is Nothing, (If((count = numberOfDependents) = 0, "[Not completed]", "[Not completed, " & count & " dependents]")), (If((TypeOf r Is AltResult) AndAlso CType(r, AltResult).ex IsNot Nothing, "[Completed exceptionally]", "[Completed normally]"))))
		End Function

		' Unsafe mechanics
		Private Shared ReadOnly UNSAFE As sun.misc.Unsafe
		Private Shared ReadOnly RESULT As Long
		Private Shared ReadOnly STACK As Long
		Private Shared ReadOnly [NEXT] As Long
		Shared Sub New()
			Try
				Dim u As sun.misc.Unsafe
					u = sun.misc.Unsafe.unsafe
					UNSAFE = u
				Dim k As  [Class] = GetType(CompletableFuture)
				RESULT = u.objectFieldOffset(k.getDeclaredField("result"))
				STACK = u.objectFieldOffset(k.getDeclaredField("stack"))
				[NEXT] = u.objectFieldOffset(GetType(Completion).getDeclaredField("next"))
			Catch x As Exception
				Throw New [Error](x)
			End Try
		End Sub
	End Class

End Namespace