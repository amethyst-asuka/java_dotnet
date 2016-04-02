Imports System

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
	''' A <seealso cref="ForkJoinTask"/> with a completion action performed when
	''' triggered and there are no remaining pending actions.
	''' CountedCompleters are in general more robust in the
	''' presence of subtask stalls and blockage than are other forms of
	''' ForkJoinTasks, but are less intuitive to program.  Uses of
	''' CountedCompleter are similar to those of other completion based
	''' components (such as <seealso cref="java.nio.channels.CompletionHandler"/>)
	''' except that multiple <em>pending</em> completions may be necessary
	''' to trigger the completion action <seealso cref="#onCompletion(CountedCompleter)"/>,
	''' not just one.
	''' Unless initialized otherwise, the {@link #getPendingCount pending
	''' count} starts at zero, but may be (atomically) changed using
	''' methods <seealso cref="#setPendingCount"/>, <seealso cref="#addToPendingCount"/>, and
	''' <seealso cref="#compareAndSetPendingCount"/>. Upon invocation of {@link
	''' #tryComplete}, if the pending action count is nonzero, it is
	''' decremented; otherwise, the completion action is performed, and if
	''' this completer itself has a completer, the process is continued
	''' with its completer.  As is the case with related synchronization
	''' components such as <seealso cref="java.util.concurrent.Phaser Phaser"/> and
	''' <seealso cref="java.util.concurrent.Semaphore Semaphore"/>, these methods
	''' affect only internal counts; they do not establish any further
	''' internal bookkeeping. In particular, the identities of pending
	''' tasks are not maintained. As illustrated below, you can create
	''' subclasses that do record some or all pending tasks or their
	''' results when needed.  As illustrated below, utility methods
	''' supporting customization of completion traversals are also
	''' provided. However, because CountedCompleters provide only basic
	''' synchronization mechanisms, it may be useful to create further
	''' abstract subclasses that maintain linkages, fields, and additional
	''' support methods appropriate for a set of related usages.
	''' 
	''' <p>A concrete CountedCompleter class must define method {@link
	''' #compute}, that should in most cases (as illustrated below), invoke
	''' {@code tryComplete()} once before returning. The class may also
	''' optionally override method <seealso cref="#onCompletion(CountedCompleter)"/>
	''' to perform an action upon normal completion, and method
	''' <seealso cref="#onExceptionalCompletion(Throwable, CountedCompleter)"/> to
	''' perform an action upon any exception.
	''' 
	''' <p>CountedCompleters most often do not bear results, in which case
	''' they are normally declared as {@code CountedCompleter<Void>}, and
	''' will always return {@code null} as a result value.  In other cases,
	''' you should override method <seealso cref="#getRawResult"/> to provide a
	''' result from {@code join(), invoke()}, and related methods.  In
	''' general, this method should return the value of a field (or a
	''' function of one or more fields) of the CountedCompleter object that
	''' holds the result upon completion. Method <seealso cref="#setRawResult"/> by
	''' default plays no role in CountedCompleters.  It is possible, but
	''' rarely applicable, to override this method to maintain other
	''' objects or fields holding result data.
	''' 
	''' <p>A CountedCompleter that does not itself have a completer (i.e.,
	''' one for which <seealso cref="#getCompleter"/> returns {@code null}) can be
	''' used as a regular ForkJoinTask with this added functionality.
	''' However, any completer that in turn has another completer serves
	''' only as an internal helper for other computations, so its own task
	''' status (as reported in methods such as <seealso cref="ForkJoinTask#isDone"/>)
	''' is arbitrary; this status changes only upon explicit invocations of
	''' <seealso cref="#complete"/>, <seealso cref="ForkJoinTask#cancel"/>,
	''' <seealso cref="ForkJoinTask#completeExceptionally(Throwable)"/> or upon
	''' exceptional completion of method {@code compute}. Upon any
	''' exceptional completion, the exception may be relayed to a task's
	''' completer (and its completer, and so on), if one exists and it has
	''' not otherwise already completed. Similarly, cancelling an internal
	''' CountedCompleter has only a local effect on that completer, so is
	''' not often useful.
	''' 
	''' <p><b>Sample Usages.</b>
	''' 
	''' <p><b>Parallel recursive decomposition.</b> CountedCompleters may
	''' be arranged in trees similar to those often used with {@link
	''' RecursiveAction}s, although the constructions involved in setting
	''' them up typically vary. Here, the completer of each task is its
	''' parent in the computation tree. Even though they entail a bit more
	''' bookkeeping, CountedCompleters may be better choices when applying
	''' a possibly time-consuming operation (that cannot be further
	''' subdivided) to each element of an array or collection; especially
	''' when the operation takes a significantly different amount of time
	''' to complete for some elements than others, either because of
	''' intrinsic variation (for example I/O) or auxiliary effects such as
	''' garbage collection.  Because CountedCompleters provide their own
	''' continuations, other threads need not block waiting to perform
	''' them.
	''' 
	''' <p>For example, here is an initial version of a class that uses
	''' divide-by-two recursive decomposition to divide work into single
	''' pieces (leaf tasks). Even when work is split into individual calls,
	''' tree-based techniques are usually preferable to directly forking
	''' leaf tasks, because they reduce inter-thread communication and
	''' improve load balancing. In the recursive case, the second of each
	''' pair of subtasks to finish triggers completion of its parent
	''' (because no result combination is performed, the default no-op
	''' implementation of method {@code onCompletion} is not overridden).
	''' A static utility method sets up the base task and invokes it
	''' (here, implicitly using the <seealso cref="ForkJoinPool#commonPool()"/>).
	''' 
	''' <pre> {@code
	''' class MyOperation<E> {  Sub  apply(E e) { ... }  }
	''' 
	''' class ForEach<E> extends CountedCompleter<Void> {
	''' 
	'''   Public Shared <E>  Sub  forEach(E[] array, MyOperation<E> op) {
	'''     new ForEach<E>(null, array, op, 0, array.length).invoke();
	'''   }
	''' 
	'''   final E[] array; final MyOperation<E> op; final int lo, hi;
	'''   ForEach(CountedCompleter<?> p, E[] array, MyOperation<E> op, int lo, int hi) {
	'''     super(p);
	'''     this.array = array; this.op = op; this.lo = lo; this.hi = hi;
	'''   }
	''' 
	'''   public  Sub  compute() { // version 1
	'''     if (hi - lo >= 2) {
	'''       int mid = (lo + hi) >>> 1;
	'''       setPendingCount(2); // must set pending count before fork
	'''       new ForEach(this, array, op, mid, hi).fork(); // right child
	'''       new ForEach(this, array, op, lo, mid).fork(); // left child
	'''     }
	'''     else if (hi > lo)
	'''       op.apply(array[lo]);
	'''     tryComplete();
	'''   }
	''' }}</pre>
	''' 
	''' This design can be improved by noticing that in the recursive case,
	''' the task has nothing to do after forking its right task, so can
	''' directly invoke its left task before returning. (This is an analog
	''' of tail recursion removal.)  Also, because the task returns upon
	''' executing its left task (rather than falling through to invoke
	''' {@code tryComplete}) the pending count is set to one:
	''' 
	''' <pre> {@code
	''' class ForEach<E> ...
	'''   public  Sub  compute() { // version 2
	'''     if (hi - lo >= 2) {
	'''       int mid = (lo + hi) >>> 1;
	'''       setPendingCount(1); // only one pending
	'''       new ForEach(this, array, op, mid, hi).fork(); // right child
	'''       new ForEach(this, array, op, lo, mid).compute(); // direct invoke
	'''     }
	'''     else {
	'''       if (hi > lo)
	'''         op.apply(array[lo]);
	'''       tryComplete();
	'''     }
	'''   }
	''' }</pre>
	''' 
	''' As a further improvement, notice that the left task need not even exist.
	''' Instead of creating a new one, we can iterate using the original task,
	''' and add a pending count for each fork.  Additionally, because no task
	''' in this tree implements an <seealso cref="#onCompletion(CountedCompleter)"/> method,
	''' {@code tryComplete()} can be replaced with <seealso cref="#propagateCompletion"/>.
	''' 
	''' <pre> {@code
	''' class ForEach<E> ...
	'''   public  Sub  compute() { // version 3
	'''     int l = lo,  h = hi;
	'''     while (h - l >= 2) {
	'''       int mid = (l + h) >>> 1;
	'''       addToPendingCount(1);
	'''       new ForEach(this, array, op, mid, h).fork(); // right child
	'''       h = mid;
	'''     }
	'''     if (h > l)
	'''       op.apply(array[l]);
	'''     propagateCompletion();
	'''   }
	''' }</pre>
	''' 
	''' Additional improvements of such classes might entail precomputing
	''' pending counts so that they can be established in constructors,
	''' specializing classes for leaf steps, subdividing by say, four,
	''' instead of two per iteration, and using an adaptive threshold
	''' instead of always subdividing down to single elements.
	''' 
	''' <p><b>Searching.</b> A tree of CountedCompleters can search for a
	''' value or property in different parts of a data structure, and
	''' report a result in an {@link
	''' java.util.concurrent.atomic.AtomicReference AtomicReference} as
	''' soon as one is found. The others can poll the result to avoid
	''' unnecessary work. (You could additionally {@link #cancel
	''' cancel} other tasks, but it is usually simpler and more efficient
	''' to just let them notice that the result is set and if so skip
	''' further processing.)  Illustrating again with an array using full
	''' partitioning (again, in practice, leaf tasks will almost always
	''' process more than one element):
	''' 
	''' <pre> {@code
	''' class Searcher<E> extends CountedCompleter<E> {
	'''   final E[] array; final AtomicReference<E> result; final int lo, hi;
	'''   Searcher(CountedCompleter<?> p, E[] array, AtomicReference<E> result, int lo, int hi) {
	'''     super(p);
	'''     this.array = array; this.result = result; this.lo = lo; this.hi = hi;
	'''   }
	'''   public E getRawResult() { return result.get(); }
	'''   public  Sub  compute() { // similar to ForEach version 3
	'''     int l = lo,  h = hi;
	'''     while (result.get() == null && h >= l) {
	'''       if (h - l >= 2) {
	'''         int mid = (l + h) >>> 1;
	'''         addToPendingCount(1);
	'''         new Searcher(this, array, result, mid, h).fork();
	'''         h = mid;
	'''       }
	'''       else {
	'''         E x = array[l];
	'''         if (matches(x) && result.compareAndSet(null, x))
	'''           quietlyCompleteRoot(); // root task is now joinable
	'''         break;
	'''       }
	'''     }
	'''     tryComplete(); // normally complete whether or not found
	'''   }
	'''   boolean matches(E e) { ... } // return true if found
	''' 
	'''   Public Shared <E> E search(E[] array) {
	'''       return new Searcher<E>(null, array, new AtomicReference<E>(), 0, array.length).invoke();
	'''   }
	''' }}</pre>
	''' 
	''' In this example, as well as others in which tasks have no other
	''' effects except to compareAndSet a common result, the trailing
	''' unconditional invocation of {@code tryComplete} could be made
	''' conditional ({@code if (result.get() == null) tryComplete();})
	''' because no further bookkeeping is required to manage completions
	''' once the root task completes.
	''' 
	''' <p><b>Recording subtasks.</b> CountedCompleter tasks that combine
	''' results of multiple subtasks usually need to access these results
	''' in method <seealso cref="#onCompletion(CountedCompleter)"/>. As illustrated in the following
	''' class (that performs a simplified form of map-reduce where mappings
	''' and reductions are all of type {@code E}), one way to do this in
	''' divide and conquer designs is to have each subtask record its
	''' sibling, so that it can be accessed in method {@code onCompletion}.
	''' This technique applies to reductions in which the order of
	''' combining left and right results does not matter; ordered
	''' reductions require explicit left/right designations.  Variants of
	''' other streamlinings seen in the above examples may also apply.
	''' 
	''' <pre> {@code
	''' class MyMapper<E> { E apply(E v) {  ...  } }
	''' class MyReducer<E> { E apply(E x, E y) {  ...  } }
	''' class MapReducer<E> extends CountedCompleter<E> {
	'''   final E[] array; final MyMapper<E> mapper;
	'''   final MyReducer<E> reducer; final int lo, hi;
	'''   MapReducer<E> sibling;
	'''   E result;
	'''   MapReducer(CountedCompleter<?> p, E[] array, MyMapper<E> mapper,
	'''              MyReducer<E> reducer, int lo, int hi) {
	'''     super(p);
	'''     this.array = array; this.mapper = mapper;
	'''     this.reducer = reducer; this.lo = lo; this.hi = hi;
	'''   }
	'''   public  Sub  compute() {
	'''     if (hi - lo >= 2) {
	'''       int mid = (lo + hi) >>> 1;
	'''       MapReducer<E> left = new MapReducer(this, array, mapper, reducer, lo, mid);
	'''       MapReducer<E> right = new MapReducer(this, array, mapper, reducer, mid, hi);
	'''       left.sibling = right;
	'''       right.sibling = left;
	'''       setPendingCount(1); // only right is pending
	'''       right.fork();
	'''       left.compute();     // directly execute left
	'''     }
	'''     else {
	'''       if (hi > lo)
	'''           result = mapper.apply(array[lo]);
	'''       tryComplete();
	'''     }
	'''   }
	'''   public  Sub  onCompletion(CountedCompleter<?> caller) {
	'''     if (caller != this) {
	'''       MapReducer<E> child = (MapReducer<E>)caller;
	'''       MapReducer<E> sib = child.sibling;
	'''       if (sib == null || sib.result == null)
	'''         result = child.result;
	'''       else
	'''         result = reducer.apply(child.result, sib.result);
	'''     }
	'''   }
	'''   public E getRawResult() { return result; }
	''' 
	'''   Public Shared <E> E mapReduce(E[] array, MyMapper<E> mapper, MyReducer<E> reducer) {
	'''     return new MapReducer<E>(null, array, mapper, reducer,
	'''                              0, array.length).invoke();
	'''   }
	''' }}</pre>
	''' 
	''' Here, method {@code onCompletion} takes a form common to many
	''' completion designs that combine results. This callback-style method
	''' is triggered once per task, in either of the two different contexts
	''' in which the pending count is, or becomes, zero: (1) by a task
	''' itself, if its pending count is zero upon invocation of {@code
	''' tryComplete}, or (2) by any of its subtasks when they complete and
	''' decrement the pending count to zero. The {@code caller} argument
	''' distinguishes cases.  Most often, when the caller is {@code this},
	''' no action is necessary. Otherwise the caller argument can be used
	''' (usually via a cast) to supply a value (and/or links to other
	''' values) to be combined.  Assuming proper use of pending counts, the
	''' actions inside {@code onCompletion} occur (once) upon completion of
	''' a task and its subtasks. No additional synchronization is required
	''' within this method to ensure thread safety of accesses to fields of
	''' this task or other completed tasks.
	''' 
	''' <p><b>Completion Traversals</b>. If using {@code onCompletion} to
	''' process completions is inapplicable or inconvenient, you can use
	''' methods <seealso cref="#firstComplete"/> and <seealso cref="#nextComplete"/> to create
	''' custom traversals.  For example, to define a MapReducer that only
	''' splits out right-hand tasks in the form of the third ForEach
	''' example, the completions must cooperatively reduce along
	''' unexhausted subtask links, which can be done as follows:
	''' 
	''' <pre> {@code
	''' class MapReducer<E> extends CountedCompleter<E> { // version 2
	'''   final E[] array; final MyMapper<E> mapper;
	'''   final MyReducer<E> reducer; final int lo, hi;
	'''   MapReducer<E> forks, next; // record subtask forks in list
	'''   E result;
	'''   MapReducer(CountedCompleter<?> p, E[] array, MyMapper<E> mapper,
	'''              MyReducer<E> reducer, int lo, int hi, MapReducer<E> next) {
	'''     super(p);
	'''     this.array = array; this.mapper = mapper;
	'''     this.reducer = reducer; this.lo = lo; this.hi = hi;
	'''     this.next = next;
	'''   }
	'''   public  Sub  compute() {
	'''     int l = lo,  h = hi;
	'''     while (h - l >= 2) {
	'''       int mid = (l + h) >>> 1;
	'''       addToPendingCount(1);
	'''       (forks = new MapReducer(this, array, mapper, reducer, mid, h, forks)).fork();
	'''       h = mid;
	'''     }
	'''     if (h > l)
	'''       result = mapper.apply(array[l]);
	'''     // process completions by reducing along and advancing subtask links
	'''     for (CountedCompleter<?> c = firstComplete(); c != null; c = c.nextComplete()) {
	'''       for (MapReducer t = (MapReducer)c, s = t.forks;  s != null; s = t.forks = s.next)
	'''         t.result = reducer.apply(t.result, s.result);
	'''     }
	'''   }
	'''   public E getRawResult() { return result; }
	''' 
	'''   Public Shared <E> E mapReduce(E[] array, MyMapper<E> mapper, MyReducer<E> reducer) {
	'''     return new MapReducer<E>(null, array, mapper, reducer,
	'''                              0, array.length, null).invoke();
	'''   }
	''' }}</pre>
	''' 
	''' <p><b>Triggers.</b> Some CountedCompleters are themselves never
	''' forked, but instead serve as bits of plumbing in other designs;
	''' including those in which the completion of one or more async tasks
	''' triggers another async task. For example:
	''' 
	''' <pre> {@code
	''' class HeaderBuilder extends CountedCompleter<...> { ... }
	''' class BodyBuilder extends CountedCompleter<...> { ... }
	''' class PacketSender extends CountedCompleter<...> {
	'''   PacketSender(...) { super(null, 1); ... } // trigger on second completion
	'''   public  Sub  compute() { } // never called
	'''   public  Sub  onCompletion(CountedCompleter<?> caller) { sendPacket(); }
	''' }
	''' // sample use:
	''' PacketSender p = new PacketSender();
	''' new HeaderBuilder(p, ...).fork();
	''' new BodyBuilder(p, ...).fork();
	''' }</pre>
	''' 
	''' @since 1.8
	''' @author Doug Lea
	''' </summary>
	Public MustInherit Class CountedCompleter(Of T)
		Inherits ForkJoinTask(Of T)

		Private Const serialVersionUID As Long = 5232453752276485070L

		''' <summary>
		''' This task's completer, or null if none </summary>
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
		Friend ReadOnly completer As CountedCompleter(Of ?)
		''' <summary>
		''' The number of pending tasks until completion </summary>
'JAVA TO VB CONVERTER TODO TASK: There is no VB equivalent to 'volatile':
		Friend pending As Integer

		''' <summary>
		''' Creates a new CountedCompleter with the given completer
		''' and initial pending count.
		''' </summary>
		''' <param name="completer"> this task's completer, or {@code null} if none </param>
		''' <param name="initialPendingCount"> the initial pending count </param>
		Protected Friend Sub New(Of T1)(ByVal completer As CountedCompleter(Of T1), ByVal initialPendingCount As Integer)
			Me.completer = completer
			Me.pending = initialPendingCount
		End Sub

		''' <summary>
		''' Creates a new CountedCompleter with the given completer
		''' and an initial pending count of zero.
		''' </summary>
		''' <param name="completer"> this task's completer, or {@code null} if none </param>
		Protected Friend Sub New(Of T1)(ByVal completer As CountedCompleter(Of T1))
			Me.completer = completer
		End Sub

		''' <summary>
		''' Creates a new CountedCompleter with no completer
		''' and an initial pending count of zero.
		''' </summary>
		Protected Friend Sub New()
			Me.completer = Nothing
		End Sub

		''' <summary>
		''' The main computation performed by this task.
		''' </summary>
		Public MustOverride Sub compute()

		''' <summary>
		''' Performs an action when method <seealso cref="#tryComplete"/> is invoked
		''' and the pending count is zero, or when the unconditional
		''' method <seealso cref="#complete"/> is invoked.  By default, this method
		''' does nothing. You can distinguish cases by checking the
		''' identity of the given caller argument. If not equal to {@code
		''' this}, then it is typically a subtask that may contain results
		''' (and/or links to other results) to combine.
		''' </summary>
		''' <param name="caller"> the task invoking this method (which may
		''' be this task itself) </param>
		Public Overridable Sub onCompletion(Of T1)(ByVal caller As CountedCompleter(Of T1))
		End Sub

		''' <summary>
		''' Performs an action when method {@link
		''' #completeExceptionally(Throwable)} is invoked or method {@link
		''' #compute} throws an exception, and this task has not already
		''' otherwise completed normally. On entry to this method, this task
		''' <seealso cref="ForkJoinTask#isCompletedAbnormally"/>.  The return value
		''' of this method controls further propagation: If {@code true}
		''' and this task has a completer that has not completed, then that
		''' completer is also completed exceptionally, with the same
		''' exception as this completer.  The default implementation of
		''' this method does nothing except return {@code true}.
		''' </summary>
		''' <param name="ex"> the exception </param>
		''' <param name="caller"> the task invoking this method (which may
		''' be this task itself) </param>
		''' <returns> {@code true} if this exception should be propagated to this
		''' task's completer, if one exists </returns>
		Public Overridable Function onExceptionalCompletion(Of T1)(ByVal ex As Throwable, ByVal caller As CountedCompleter(Of T1)) As Boolean
			Return True
		End Function

		''' <summary>
		''' Returns the completer established in this task's constructor,
		''' or {@code null} if none.
		''' </summary>
		''' <returns> the completer </returns>
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
		Public Property completer As CountedCompleter(Of ?)
			Get
				Return completer
			End Get
		End Property

		''' <summary>
		''' Returns the current pending count.
		''' </summary>
		''' <returns> the current pending count </returns>
		Public Property pendingCount As Integer
			Get
				Return pending
			End Get
			Set(ByVal count As Integer)
				pending = count
			End Set
		End Property


		''' <summary>
		''' Adds (atomically) the given value to the pending count.
		''' </summary>
		''' <param name="delta"> the value to add </param>
		Public Sub addToPendingCount(ByVal delta As Integer)
			U.getAndAddInt(Me, PENDING, delta)
		End Sub

		''' <summary>
		''' Sets (atomically) the pending count to the given count only if
		''' it currently holds the given expected value.
		''' </summary>
		''' <param name="expected"> the expected value </param>
		''' <param name="count"> the new value </param>
		''' <returns> {@code true} if successful </returns>
		Public Function compareAndSetPendingCount(ByVal expected As Integer, ByVal count As Integer) As Boolean
			Return U.compareAndSwapInt(Me, PENDING, expected, count)
		End Function

		''' <summary>
		''' If the pending count is nonzero, (atomically) decrements it.
		''' </summary>
		''' <returns> the initial (undecremented) pending count holding on entry
		''' to this method </returns>
		Public Function decrementPendingCountUnlessZero() As Integer
			Dim c As Integer
			Do
				c = pending
			Loop While c <> 0 AndAlso Not U.compareAndSwapInt(Me, PENDING, c, c - 1)
			Return c
		End Function

		''' <summary>
		''' Returns the root of the current computation; i.e., this
		''' task if it has no completer, else its completer's root.
		''' </summary>
		''' <returns> the root of the current computation </returns>
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
		Public Property root As CountedCompleter(Of ?)
			Get
	'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
				Dim a As CountedCompleter(Of ?) = Me, p As CountedCompleter(Of ?)
				p = a.completer
				Do While p IsNot Nothing
					a = p
					p = a.completer
				Loop
				Return a
			End Get
		End Property

		''' <summary>
		''' If the pending count is nonzero, decrements the count;
		''' otherwise invokes <seealso cref="#onCompletion(CountedCompleter)"/>
		''' and then similarly tries to complete this task's completer,
		''' if one exists, else marks this task as complete.
		''' </summary>
		Public Sub tryComplete()
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
			Dim a As CountedCompleter(Of ?) = Me, s As CountedCompleter(Of ?) = a
			Dim c As Integer
			Do
				c = a.pending
				If c = 0 Then
					a.onCompletion(s)
'JAVA TO VB CONVERTER TODO TASK: Assignments within expressions are not supported in VB
					a = (s = a).completer
					If a Is Nothing Then
						s.quietlyComplete()
						Return
					End If
				ElseIf U.compareAndSwapInt(a, PENDING, c, c - 1) Then
					Return
				End If
			Loop
		End Sub

		''' <summary>
		''' Equivalent to <seealso cref="#tryComplete"/> but does not invoke {@link
		''' #onCompletion(CountedCompleter)} along the completion path:
		''' If the pending count is nonzero, decrements the count;
		''' otherwise, similarly tries to complete this task's completer, if
		''' one exists, else marks this task as complete. This method may be
		''' useful in cases where {@code onCompletion} should not, or need
		''' not, be invoked for each completer in a computation.
		''' </summary>
		Public Sub propagateCompletion()
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
			Dim a As CountedCompleter(Of ?) = Me, s As CountedCompleter(Of ?) = a
			Dim c As Integer
			Do
				c = a.pending
				If c = 0 Then
'JAVA TO VB CONVERTER TODO TASK: Assignments within expressions are not supported in VB
					a = (s = a).completer
					If a Is Nothing Then
						s.quietlyComplete()
						Return
					End If
				ElseIf U.compareAndSwapInt(a, PENDING, c, c - 1) Then
					Return
				End If
			Loop
		End Sub

		''' <summary>
		''' Regardless of pending count, invokes
		''' <seealso cref="#onCompletion(CountedCompleter)"/>, marks this task as
		''' complete and further triggers <seealso cref="#tryComplete"/> on this
		''' task's completer, if one exists.  The given rawResult is
		''' used as an argument to <seealso cref="#setRawResult"/> before invoking
		''' <seealso cref="#onCompletion(CountedCompleter)"/> or marking this task
		''' as complete; its value is meaningful only for classes
		''' overriding {@code setRawResult}.  This method does not modify
		''' the pending count.
		''' 
		''' <p>This method may be useful when forcing completion as soon as
		''' any one (versus all) of several subtask results are obtained.
		''' However, in the common (and recommended) case in which {@code
		''' setRawResult} is not overridden, this effect can be obtained
		''' more simply using {@code quietlyCompleteRoot();}.
		''' </summary>
		''' <param name="rawResult"> the raw result </param>
		Public Overridable Sub complete(ByVal rawResult As T)
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
			Dim p As CountedCompleter(Of ?)
			rawResult = rawResult
			onCompletion(Me)
			quietlyComplete()
			p = completer
			If p IsNot Nothing Then p.tryComplete()
		End Sub

		''' <summary>
		''' If this task's pending count is zero, returns this task;
		''' otherwise decrements its pending count and returns {@code
		''' null}. This method is designed to be used with {@link
		''' #nextComplete} in completion traversal loops.
		''' </summary>
		''' <returns> this task, if pending count was zero, else {@code null} </returns>
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
		Public Function firstComplete() As CountedCompleter(Of ?)
			Dim c As Integer
			Do
				c = pending
				If c = 0 Then
					Return Me
				ElseIf U.compareAndSwapInt(Me, PENDING, c, c - 1) Then
					Return Nothing
				End If
			Loop
		End Function

		''' <summary>
		''' If this task does not have a completer, invokes {@link
		''' ForkJoinTask#quietlyComplete} and returns {@code null}.  Or, if
		''' the completer's pending count is non-zero, decrements that
		''' pending count and returns {@code null}.  Otherwise, returns the
		''' completer.  This method can be used as part of a completion
		''' traversal loop for homogeneous task hierarchies:
		''' 
		''' <pre> {@code
		''' for (CountedCompleter<?> c = firstComplete();
		'''      c != null;
		'''      c = c.nextComplete()) {
		'''   // ... process c ...
		''' }}</pre>
		''' </summary>
		''' <returns> the completer, or {@code null} if none </returns>
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
		Public Function nextComplete() As CountedCompleter(Of ?)
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
			Dim p As CountedCompleter(Of ?)
			p = completer
			If p IsNot Nothing Then
				Return p.firstComplete()
			Else
				quietlyComplete()
				Return Nothing
			End If
		End Function

		''' <summary>
		''' Equivalent to {@code getRoot().quietlyComplete()}.
		''' </summary>
		Public Sub quietlyCompleteRoot()
'JAVA TO VB CONVERTER TODO TASK: The following line could not be converted:
			for (CountedCompleter<?> a = Me, p;;)
				p = a.completer
				If p Is Nothing Then
					a.quietlyComplete()
					Return
				End If
				a = p
		End Sub

		''' <summary>
		''' If this task has not completed, attempts to process at most the
		''' given number of other unprocessed tasks for which this task is
		''' on the completion path, if any are known to exist.
		''' </summary>
		''' <param name="maxTasks"> the maximum number of tasks to process.  If
		'''                 less than or equal to zero, then no tasks are
		'''                 processed. </param>
		Public Sub helpComplete(ByVal maxTasks As Integer)
			Dim t As Thread
			Dim wt As ForkJoinWorkerThread
			If maxTasks > 0 AndAlso status >= 0 Then
				t = Thread.CurrentThread
				If TypeOf t Is ForkJoinWorkerThread Then
'JAVA TO VB CONVERTER TODO TASK: Assignments within expressions are not supported in VB
					(wt = CType(t, ForkJoinWorkerThread)).pool.helpComplete(wt.workQueue, Me, maxTasks)
				Else
					ForkJoinPool.common.externalHelpComplete(Me, maxTasks)
				End If
			End If
		End Sub

		''' <summary>
		''' Supports ForkJoinTask exception propagation.
		''' </summary>
		Friend Overridable Sub internalPropagateException(ByVal ex As Throwable)
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
			Dim a As CountedCompleter(Of ?) = Me, s As CountedCompleter(Of ?) = a
'JAVA TO VB CONVERTER TODO TASK: Assignments within expressions are not supported in VB
			a = (s = a).completer
			Do While a.onExceptionalCompletion(ex, s) AndAlso a IsNot Nothing AndAlso a.status >= 0 AndAlso a.recordExceptionalCompletion(ex) = EXCEPTIONAL

				a = (s = a).completer
			Loop
		End Sub

		''' <summary>
		''' Implements execution conventions for CountedCompleters.
		''' </summary>
		Protected Friend Function exec() As Boolean
			compute()
			Return False
		End Function

		''' <summary>
		''' Returns the result of the computation. By default
		''' returns {@code null}, which is appropriate for {@code Void}
		''' actions, but in other cases should be overridden, almost
		''' always to return a field or function of a field that
		''' holds the result upon completion.
		''' </summary>
		''' <returns> the result of the computation </returns>
		Public Overridable Property rawResult As T
			Get
				Return Nothing
			End Get
			Set(ByVal t As T)
			End Set
		End Property


		' Unsafe mechanics
		Private Shared ReadOnly U As sun.misc.Unsafe
		Private Shared ReadOnly PENDING As Long
		Shared Sub New()
			Try
				U = sun.misc.Unsafe.unsafe
				PENDING = U.objectFieldOffset(GetType(CountedCompleter).getDeclaredField("pending"))
			Catch e As Exception
				Throw New [Error](e)
			End Try
		End Sub
	End Class

End Namespace