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
	''' A {@code Future} represents the result of an asynchronous
	''' computation.  Methods are provided to check if the computation is
	''' complete, to wait for its completion, and to retrieve the result of
	''' the computation.  The result can only be retrieved using method
	''' {@code get} when the computation has completed, blocking if
	''' necessary until it is ready.  Cancellation is performed by the
	''' {@code cancel} method.  Additional methods are provided to
	''' determine if the task completed normally or was cancelled. Once a
	''' computation has completed, the computation cannot be cancelled.
	''' If you would like to use a {@code Future} for the sake
	''' of cancellability but not provide a usable result, you can
	''' declare types of the form {@code Future<?>} and
	''' return {@code null} as a result of the underlying task.
	''' 
	''' <p>
	''' <b>Sample Usage</b> (Note that the following classes are all
	''' made-up.)
	''' <pre> {@code
	''' interface ArchiveSearcher { String search(String target); }
	''' class App {
	'''   ExecutorService executor = ...
	'''   ArchiveSearcher searcher = ...
	'''    Sub  showSearch(final String target)
	'''       throws InterruptedException {
	'''     Future<String> future
	'''       = executor.submit(new Callable<String>() {
	'''         public String call() {
	'''             return searcher.search(target);
	'''         }});
	'''     displayOtherThings(); // do other things while searching
	'''     try {
	'''       displayText(future.get()); // use future
	'''     } catch (ExecutionException ex) { cleanup(); return; }
	'''   }
	''' }}</pre>
	''' 
	''' The <seealso cref="FutureTask"/> class is an implementation of {@code Future} that
	''' implements {@code Runnable}, and so may be executed by an {@code Executor}.
	''' For example, the above construction with {@code submit} could be replaced by:
	'''  <pre> {@code
	''' FutureTask<String> future =
	'''   new FutureTask<String>(new Callable<String>() {
	'''     public String call() {
	'''       return searcher.search(target);
	'''   }});
	''' executor.execute(future);}</pre>
	''' 
	''' <p>Memory consistency effects: Actions taken by the asynchronous computation
	''' <a href="package-summary.html#MemoryVisibility"> <i>happen-before</i></a>
	''' actions following the corresponding {@code Future.get()} in another thread.
	''' </summary>
	''' <seealso cref= FutureTask </seealso>
	''' <seealso cref= Executor
	''' @since 1.5
	''' @author Doug Lea </seealso>
	''' @param <V> The result type returned by this Future's {@code get} method </param>
	Public Interface Future(Of V)

		''' <summary>
		''' Attempts to cancel execution of this task.  This attempt will
		''' fail if the task has already completed, has already been cancelled,
		''' or could not be cancelled for some other reason. If successful,
		''' and this task has not started when {@code cancel} is called,
		''' this task should never run.  If the task has already started,
		''' then the {@code mayInterruptIfRunning} parameter determines
		''' whether the thread executing this task should be interrupted in
		''' an attempt to stop the task.
		''' 
		''' <p>After this method returns, subsequent calls to <seealso cref="#isDone"/> will
		''' always return {@code true}.  Subsequent calls to <seealso cref="#isCancelled"/>
		''' will always return {@code true} if this method returned {@code true}.
		''' </summary>
		''' <param name="mayInterruptIfRunning"> {@code true} if the thread executing this
		''' task should be interrupted; otherwise, in-progress tasks are allowed
		''' to complete </param>
		''' <returns> {@code false} if the task could not be cancelled,
		''' typically because it has already completed normally;
		''' {@code true} otherwise </returns>
		Function cancel(ByVal mayInterruptIfRunning As Boolean) As Boolean

		''' <summary>
		''' Returns {@code true} if this task was cancelled before it completed
		''' normally.
		''' </summary>
		''' <returns> {@code true} if this task was cancelled before it completed </returns>
		ReadOnly Property cancelled As Boolean

		''' <summary>
		''' Returns {@code true} if this task completed.
		''' 
		''' Completion may be due to normal termination, an exception, or
		''' cancellation -- in all of these cases, this method will return
		''' {@code true}.
		''' </summary>
		''' <returns> {@code true} if this task completed </returns>
		ReadOnly Property done As Boolean

		''' <summary>
		''' Waits if necessary for the computation to complete, and then
		''' retrieves its result.
		''' </summary>
		''' <returns> the computed result </returns>
		''' <exception cref="CancellationException"> if the computation was cancelled </exception>
		''' <exception cref="ExecutionException"> if the computation threw an
		''' exception </exception>
		''' <exception cref="InterruptedException"> if the current thread was interrupted
		''' while waiting </exception>
		Function [get]() As V

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
		''' <exception cref="InterruptedException"> if the current thread was interrupted
		''' while waiting </exception>
		''' <exception cref="TimeoutException"> if the wait timed out </exception>
		Function [get](ByVal timeout As Long, ByVal unit As TimeUnit) As V
	End Interface

End Namespace