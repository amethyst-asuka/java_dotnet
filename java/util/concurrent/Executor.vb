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
	''' An object that executes submitted <seealso cref="Runnable"/> tasks. This
	''' interface provides a way of decoupling task submission from the
	''' mechanics of how each task will be run, including details of thread
	''' use, scheduling, etc.  An {@code Executor} is normally used
	''' instead of explicitly creating threads. For example, rather than
	''' invoking {@code new Thread(new(RunnableTask())).start()} for each
	''' of a set of tasks, you might use:
	''' 
	''' <pre>
	''' Executor executor = <em>anExecutor</em>;
	''' executor.execute(new RunnableTask1());
	''' executor.execute(new RunnableTask2());
	''' ...
	''' </pre>
	''' 
	''' However, the {@code Executor} interface does not strictly
	''' require that execution be asynchronous. In the simplest case, an
	''' executor can run the submitted task immediately in the caller's
	''' thread:
	''' 
	'''  <pre> {@code
	''' class DirectExecutor implements Executor {
	'''   public  Sub  execute(Runnable r) {
	'''     r.run();
	'''   }
	''' }}</pre>
	''' 
	''' More typically, tasks are executed in some thread other
	''' than the caller's thread.  The executor below spawns a new thread
	''' for each task.
	''' 
	'''  <pre> {@code
	''' class ThreadPerTaskExecutor implements Executor {
	'''   public  Sub  execute(Runnable r) {
	'''     new Thread(r).start();
	'''   }
	''' }}</pre>
	''' 
	''' Many {@code Executor} implementations impose some sort of
	''' limitation on how and when tasks are scheduled.  The executor below
	''' serializes the submission of tasks to a second executor,
	''' illustrating a composite executor.
	''' 
	'''  <pre> {@code
	''' class SerialExecutor implements Executor {
	'''   final Queue<Runnable> tasks = new ArrayDeque<Runnable>();
	'''   final Executor executor;
	'''   Runnable active;
	''' 
	'''   SerialExecutor(Executor executor) {
	'''     this.executor = executor;
	'''   }
	''' 
	'''   Public   Sub  execute(final Runnable r) {
	'''     tasks.offer(new Runnable() {
	'''       public  Sub  run() {
	'''         try {
	'''           r.run();
	'''         } finally {
	'''           scheduleNext();
	'''         }
	'''       }
	'''     });
	'''     if (active == null) {
	'''       scheduleNext();
	'''     }
	'''   }
	''' 
	'''   protected synchronized  Sub  scheduleNext() {
	'''     if ((active = tasks.poll()) != null) {
	'''       executor.execute(active);
	'''     }
	'''   }
	''' }}</pre>
	''' 
	''' The {@code Executor} implementations provided in this package
	''' implement <seealso cref="ExecutorService"/>, which is a more extensive
	''' interface.  The <seealso cref="ThreadPoolExecutor"/> class provides an
	''' extensible thread pool implementation. The <seealso cref="Executors"/> class
	''' provides convenient factory methods for these Executors.
	''' 
	''' <p>Memory consistency effects: Actions in a thread prior to
	''' submitting a {@code Runnable} object to an {@code Executor}
	''' <a href="package-summary.html#MemoryVisibility"><i>happen-before</i></a>
	''' its execution begins, perhaps in another thread.
	''' 
	''' @since 1.5
	''' @author Doug Lea
	''' </summary>
	Public Interface Executor

		''' <summary>
		''' Executes the given command at some time in the future.  The command
		''' may execute in a new thread, in a pooled thread, or in the calling
		''' thread, at the discretion of the {@code Executor} implementation.
		''' </summary>
		''' <param name="command"> the runnable task </param>
		''' <exception cref="RejectedExecutionException"> if this task cannot be
		''' accepted for execution </exception>
		''' <exception cref="NullPointerException"> if command is null </exception>
		Sub execute(  command As Runnable)
	End Interface

End Namespace