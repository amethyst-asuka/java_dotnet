Imports System.Diagnostics
Imports System.Collections.Generic

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
	''' Provides default implementations of <seealso cref="ExecutorService"/>
	''' execution methods. This class implements the {@code submit},
	''' {@code invokeAny} and {@code invokeAll} methods using a
	''' <seealso cref="RunnableFuture"/> returned by {@code newTaskFor}, which defaults
	''' to the <seealso cref="FutureTask"/> class provided in this package.  For example,
	''' the implementation of {@code submit(Runnable)} creates an
	''' associated {@code RunnableFuture} that is executed and
	''' returned. Subclasses may override the {@code newTaskFor} methods
	''' to return {@code RunnableFuture} implementations other than
	''' {@code FutureTask}.
	''' 
	''' <p><b>Extension example</b>. Here is a sketch of a class
	''' that customizes <seealso cref="ThreadPoolExecutor"/> to use
	''' a {@code CustomTask} class instead of the default {@code FutureTask}:
	'''  <pre> {@code
	''' public class CustomThreadPoolExecutor extends ThreadPoolExecutor {
	''' 
	'''   static class CustomTask<V> implements RunnableFuture<V> {...}
	''' 
	'''   protected <V> RunnableFuture<V> newTaskFor(Callable<V> c) {
	'''       return new CustomTask<V>(c);
	'''   }
	'''   protected <V> RunnableFuture<V> newTaskFor(Runnable r, V v) {
	'''       return new CustomTask<V>(r, v);
	'''   }
	'''   // ... add constructors, etc.
	''' }}</pre>
	''' 
	''' @since 1.5
	''' @author Doug Lea
	''' </summary>
	Public MustInherit Class AbstractExecutorService
		Implements ExecutorService

			Public MustOverride Function invokeAny(Of T1 As Callable(Of T)(  tasks As ICollection(Of T1),   timeout As Long,   unit As TimeUnit) As T Implements ExecutorService.invokeAny
			Public MustOverride Function invokeAny(Of T1 As Callable(Of T)(  tasks As ICollection(Of T1)) As T Implements ExecutorService.invokeAny
			Public MustOverride Function invokeAll(Of T1 As Callable(Of T)(  tasks As ICollection(Of T1),   timeout As Long,   unit As TimeUnit) As IList(Of Future(Of T)) Implements ExecutorService.invokeAll
			Public MustOverride Function invokeAll(Of T1 As Callable(Of T)(  tasks As ICollection(Of T1)) As IList(Of Future(Of T)) Implements ExecutorService.invokeAll
			Public MustOverride Function awaitTermination(  timeout As Long,   unit As TimeUnit) As Boolean Implements ExecutorService.awaitTermination
			Public MustOverride ReadOnly Property terminated As Boolean Implements ExecutorService.isTerminated
			Public MustOverride ReadOnly Property shutdown As Boolean Implements ExecutorService.isShutdown
			Public MustOverride Function shutdownNow() As IList(Of Runnable) Implements ExecutorService.shutdownNow
			Public MustOverride Sub shutdown() Implements ExecutorService.shutdown

		''' <summary>
		''' Returns a {@code RunnableFuture} for the given runnable and default
		''' value.
		''' </summary>
		''' <param name="runnable"> the runnable task being wrapped </param>
		''' <param name="value"> the default value for the returned future </param>
		''' @param <T> the type of the given value </param>
		''' <returns> a {@code RunnableFuture} which, when run, will run the
		''' underlying runnable and which, as a {@code Future}, will yield
		''' the given value as its result and provide for cancellation of
		''' the underlying task
		''' @since 1.6 </returns>
		Protected Friend Overridable Function newTaskFor(Of T)(  runnable As Runnable,   value As T) As RunnableFuture(Of T)
			Return New FutureTask(Of T)(runnable, value)
		End Function

		''' <summary>
		''' Returns a {@code RunnableFuture} for the given callable task.
		''' </summary>
		''' <param name="callable"> the callable task being wrapped </param>
		''' @param <T> the type of the callable's result </param>
		''' <returns> a {@code RunnableFuture} which, when run, will call the
		''' underlying callable and which, as a {@code Future}, will yield
		''' the callable's result as its result and provide for
		''' cancellation of the underlying task
		''' @since 1.6 </returns>
		Protected Friend Overridable Function newTaskFor(Of T)(  callable As Callable(Of T)) As RunnableFuture(Of T)
			Return New FutureTask(Of T)(callable)
		End Function

		''' <exception cref="RejectedExecutionException"> {@inheritDoc} </exception>
		''' <exception cref="NullPointerException">       {@inheritDoc} </exception>
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
		Public Overridable Function submit(  task As Runnable) As Future(Of ?) Implements ExecutorService.submit
			If task Is Nothing Then Throw New NullPointerException
			Dim ftask As RunnableFuture(Of Void) = newTaskFor(task, Nothing)
			execute(ftask)
			Return ftask
		End Function

		''' <exception cref="RejectedExecutionException"> {@inheritDoc} </exception>
		''' <exception cref="NullPointerException">       {@inheritDoc} </exception>
		Public Overridable Function submit(Of T)(  task As Runnable,   result As T) As Future(Of T) Implements ExecutorService.submit
			If task Is Nothing Then Throw New NullPointerException
			Dim ftask As RunnableFuture(Of T) = newTaskFor(task, result)
			execute(ftask)
			Return ftask
		End Function

		''' <exception cref="RejectedExecutionException"> {@inheritDoc} </exception>
		''' <exception cref="NullPointerException">       {@inheritDoc} </exception>
		Public Overridable Function submit(Of T)(  task As Callable(Of T)) As Future(Of T) Implements ExecutorService.submit
			If task Is Nothing Then Throw New NullPointerException
			Dim ftask As RunnableFuture(Of T) = newTaskFor(task)
			execute(ftask)
			Return ftask
		End Function

		''' <summary>
		''' the main mechanics of invokeAny.
		''' </summary>
		Private Function doInvokeAny(Of T, T1 As Callable(Of T)(  tasks As Collection(Of T1),   timed As Boolean,   nanos As Long) As T
			If tasks Is Nothing Then Throw New NullPointerException
			Dim ntasks As Integer = tasks.size()
			If ntasks = 0 Then Throw New IllegalArgumentException
			Dim futures As New List(Of Future(Of T))(ntasks)
			Dim ecs As New ExecutorCompletionService(Of T)(Me)

			' For efficiency, especially in executors with limited
			' parallelism, check to see if previously submitted tasks are
			' done before submitting more of them. This interleaving
			' plus the exception mechanics account for messiness of main
			' loop.

			Try
				' Record exceptions so that if we fail to obtain any
				' result, we can throw the last exception we got.
				Dim ee As ExecutionException = Nothing
				Dim deadline As Long = If(timed, System.nanoTime() + nanos, 0L)
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
				Dim it As [Iterator](Of ? As Callable(Of T)) = tasks.GetEnumerator()

				' Start one task for sure; the rest incrementally
				futures.add(ecs.submit(it.next()))
				ntasks -= 1
				Dim active As Integer = 1

				Do
					Dim f As Future(Of T) = ecs.poll()
					If f Is Nothing Then
						If ntasks > 0 Then
							ntasks -= 1
							futures.add(ecs.submit(it.next()))
							active += 1
						ElseIf active = 0 Then
							Exit Do
						ElseIf timed Then
							f = ecs.poll(nanos, TimeUnit.NANOSECONDS)
							If f Is Nothing Then Throw New TimeoutException
							nanos = deadline - System.nanoTime()
						Else
							f = ecs.take()
						End If
					End If
					If f IsNot Nothing Then
						active -= 1
						Try
							Return f.get()
						Catch eex As ExecutionException
							ee = eex
						Catch rex As RuntimeException
							ee = New ExecutionException(rex)
						End Try
					End If
				Loop

				If ee Is Nothing Then ee = New ExecutionException
				Throw ee

			Finally
				Dim i As Integer = 0
				Dim size As Integer = futures.size()
				Do While i < size
					futures.get(i).cancel(True)
					i += 1
				Loop
			End Try
		End Function

		Public Overridable Function invokeAny(Of T, T1 As Callable(Of T)(  tasks As Collection(Of T1)) As T
			Try
				Return doInvokeAny(tasks, False, 0)
			Catch cannotHappen As TimeoutException
				Debug.Assert(False)
				Return Nothing
			End Try
		End Function

		Public Overridable Function invokeAny(Of T, T1 As Callable(Of T)(  tasks As Collection(Of T1),   timeout As Long,   unit As TimeUnit) As T
			Return doInvokeAny(tasks, True, unit.toNanos(timeout))
		End Function

		Public Overridable Function invokeAll(Of T, T1 As Callable(Of T)(  tasks As Collection(Of T1)) As List(Of Future(Of T))
			If tasks Is Nothing Then Throw New NullPointerException
			Dim futures As New List(Of Future(Of T))(tasks.size())
			Dim done As Boolean = False
			Try
				For Each t As Callable(Of T) In tasks
					Dim f As RunnableFuture(Of T) = newTaskFor(t)
					futures.add(f)
					execute(f)
				Next t
				Dim i As Integer = 0
				Dim size As Integer = futures.size()
				Do While i < size
					Dim f As Future(Of T) = futures.get(i)
					If Not f.done Then
						Try
							f.get()
						Catch ignore As CancellationException
						Catch ignore As ExecutionException
						End Try
					End If
					i += 1
				Loop
				done = True
				Return futures
			Finally
				If Not done Then
					Dim i As Integer = 0
					Dim size As Integer = futures.size()
					Do While i < size
						futures.get(i).cancel(True)
						i += 1
					Loop
				End If
			End Try
		End Function

		Public Overridable Function invokeAll(Of T, T1 As Callable(Of T)(  tasks As Collection(Of T1),   timeout As Long,   unit As TimeUnit) As List(Of Future(Of T))
			If tasks Is Nothing Then Throw New NullPointerException
			Dim nanos As Long = unit.toNanos(timeout)
			Dim futures As New List(Of Future(Of T))(tasks.size())
			Dim done As Boolean = False
			Try
				For Each t As Callable(Of T) In tasks
					futures.add(newTaskFor(t))
				Next t

				Dim deadline As Long = System.nanoTime() + nanos
				Dim size As Integer = futures.size()

				' Interleave time checks and calls to execute in case
				' executor doesn't have any/much parallelism.
				For i As Integer = 0 To size - 1
					execute(CType(futures.get(i), Runnable))
					nanos = deadline - System.nanoTime()
					If nanos <= 0L Then Return futures
				Next i

				For i As Integer = 0 To size - 1
					Dim f As Future(Of T) = futures.get(i)
					If Not f.done Then
						If nanos <= 0L Then Return futures
						Try
							f.get(nanos, TimeUnit.NANOSECONDS)
						Catch ignore As CancellationException
						Catch ignore As ExecutionException
						Catch toe As TimeoutException
							Return futures
						End Try
						nanos = deadline - System.nanoTime()
					End If
				Next i
				done = True
				Return futures
			Finally
				If Not done Then
					Dim i As Integer = 0
					Dim size As Integer = futures.size()
					Do While i < size
						futures.get(i).cancel(True)
						i += 1
					Loop
				End If
			End Try
		End Function

	End Class

End Namespace