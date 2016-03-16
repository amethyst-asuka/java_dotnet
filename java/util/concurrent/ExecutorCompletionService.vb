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
	''' A <seealso cref="CompletionService"/> that uses a supplied <seealso cref="Executor"/>
	''' to execute tasks.  This class arranges that submitted tasks are,
	''' upon completion, placed on a queue accessible using {@code take}.
	''' The class is lightweight enough to be suitable for transient use
	''' when processing groups of tasks.
	''' 
	''' <p>
	''' 
	''' <b>Usage Examples.</b>
	''' 
	''' Suppose you have a set of solvers for a certain problem, each
	''' returning a value of some type {@code Result}, and would like to
	''' run them concurrently, processing the results of each of them that
	''' return a non-null value, in some method {@code use(Result r)}. You
	''' could write this as:
	''' 
	''' <pre> {@code
	'''  Sub  solve(Executor e,
	'''            Collection<Callable<Result>> solvers)
	'''     throws InterruptedException, ExecutionException {
	'''     CompletionService<Result> ecs
	'''         = new ExecutorCompletionService<Result>(e);
	'''     for (Callable<Result> s : solvers)
	'''         ecs.submit(s);
	'''     int n = solvers.size();
	'''     for (int i = 0; i < n; ++i) {
	'''         Result r = ecs.take().get();
	'''         if (r != null)
	'''             use(r);
	'''     }
	''' }}</pre>
	''' 
	''' Suppose instead that you would like to use the first non-null result
	''' of the set of tasks, ignoring any that encounter exceptions,
	''' and cancelling all other tasks when the first one is ready:
	''' 
	''' <pre> {@code
	'''  Sub  solve(Executor e,
	'''            Collection<Callable<Result>> solvers)
	'''     throws InterruptedException {
	'''     CompletionService<Result> ecs
	'''         = new ExecutorCompletionService<Result>(e);
	'''     int n = solvers.size();
	'''     List<Future<Result>> futures
	'''         = new ArrayList<Future<Result>>(n);
	'''     Result result = null;
	'''     try {
	'''         for (Callable<Result> s : solvers)
	'''             futures.add(ecs.submit(s));
	'''         for (int i = 0; i < n; ++i) {
	'''             try {
	'''                 Result r = ecs.take().get();
	'''                 if (r != null) {
	'''                     result = r;
	'''                     break;
	'''                 }
	'''             } catch (ExecutionException ignore) {}
	'''         }
	'''     }
	'''     finally {
	'''         for (Future<Result> f : futures)
	'''             f.cancel(true);
	'''     }
	''' 
	'''     if (result != null)
	'''         use(result);
	''' }}</pre>
	''' </summary>
	Public Class ExecutorCompletionService(Of V)
		Implements CompletionService(Of V)

		Private ReadOnly executor As Executor
		Private ReadOnly aes As AbstractExecutorService
		Private ReadOnly completionQueue As BlockingQueue(Of Future(Of V))

		''' <summary>
		''' FutureTask extension to enqueue upon completion
		''' </summary>
		Private Class QueueingFuture
			Inherits FutureTask(Of Void)

			Private ReadOnly outerInstance As ExecutorCompletionService

			Friend Sub New(ByVal outerInstance As ExecutorCompletionService, ByVal task As RunnableFuture(Of V))
					Me.outerInstance = outerInstance
				MyBase.New(task, Nothing)
				Me.task = task
			End Sub
			Protected Friend Overridable Sub done()
				outerInstance.completionQueue.add(task)
			End Sub
			Private ReadOnly task As Future(Of V)
		End Class

		Private Function newTaskFor(ByVal task As Callable(Of V)) As RunnableFuture(Of V)
			If aes Is Nothing Then
				Return New FutureTask(Of V)(task)
			Else
				Return aes.newTaskFor(task)
			End If
		End Function

		Private Function newTaskFor(ByVal task As Runnable, ByVal result As V) As RunnableFuture(Of V)
			If aes Is Nothing Then
				Return New FutureTask(Of V)(task, result)
			Else
				Return aes.newTaskFor(task, result)
			End If
		End Function

		''' <summary>
		''' Creates an ExecutorCompletionService using the supplied
		''' executor for base task execution and a
		''' <seealso cref="LinkedBlockingQueue"/> as a completion queue.
		''' </summary>
		''' <param name="executor"> the executor to use </param>
		''' <exception cref="NullPointerException"> if executor is {@code null} </exception>
		Public Sub New(ByVal executor As Executor)
			If executor Is Nothing Then Throw New NullPointerException
			Me.executor = executor
			Me.aes = If(TypeOf executor Is AbstractExecutorService, CType(executor, AbstractExecutorService), Nothing)
			Me.completionQueue = New LinkedBlockingQueue(Of Future(Of V))
		End Sub

		''' <summary>
		''' Creates an ExecutorCompletionService using the supplied
		''' executor for base task execution and the supplied queue as its
		''' completion queue.
		''' </summary>
		''' <param name="executor"> the executor to use </param>
		''' <param name="completionQueue"> the queue to use as the completion queue
		'''        normally one dedicated for use by this service. This
		'''        queue is treated as unbounded -- failed attempted
		'''        {@code Queue.add} operations for completed tasks cause
		'''        them not to be retrievable. </param>
		''' <exception cref="NullPointerException"> if executor or completionQueue are {@code null} </exception>
		Public Sub New(ByVal executor As Executor, ByVal completionQueue As BlockingQueue(Of Future(Of V)))
			If executor Is Nothing OrElse completionQueue Is Nothing Then Throw New NullPointerException
			Me.executor = executor
			Me.aes = If(TypeOf executor Is AbstractExecutorService, CType(executor, AbstractExecutorService), Nothing)
			Me.completionQueue = completionQueue
		End Sub

		Public Overridable Function submit(ByVal task As Callable(Of V)) As Future(Of V) Implements CompletionService(Of V).submit
			If task Is Nothing Then Throw New NullPointerException
			Dim f As RunnableFuture(Of V) = newTaskFor(task)
			executor.execute(New QueueingFuture(Me, f))
			Return f
		End Function

		Public Overridable Function submit(ByVal task As Runnable, ByVal result As V) As Future(Of V)
			If task Is Nothing Then Throw New NullPointerException
			Dim f As RunnableFuture(Of V) = newTaskFor(task, result)
			executor.execute(New QueueingFuture(Me, f))
			Return f
		End Function

		Public Overridable Function take() As Future(Of V) Implements CompletionService(Of V).take
			Return completionQueue.take()
		End Function

		Public Overridable Function poll() As Future(Of V) Implements CompletionService(Of V).poll
			Return completionQueue.poll()
		End Function

		Public Overridable Function poll(ByVal timeout As Long, ByVal unit As TimeUnit) As Future(Of V) Implements CompletionService(Of V).poll
			Return completionQueue.poll(timeout, unit)
		End Function

	End Class

End Namespace