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
	''' An unbounded <seealso cref="BlockingQueue blocking queue"/> of
	''' {@code Delayed} elements, in which an element can only be taken
	''' when its delay has expired.  The <em>head</em> of the queue is that
	''' {@code Delayed} element whose delay expired furthest in the
	''' past.  If no delay has expired there is no head and {@code poll}
	''' will return {@code null}. Expiration occurs when an element's
	''' {@code getDelay(TimeUnit.NANOSECONDS)} method returns a value less
	''' than or equal to zero.  Even though unexpired elements cannot be
	''' removed using {@code take} or {@code poll}, they are otherwise
	''' treated as normal elements. For example, the {@code size} method
	''' returns the count of both expired and unexpired elements.
	''' This queue does not permit null elements.
	''' 
	''' <p>This class and its iterator implement all of the
	''' <em>optional</em> methods of the <seealso cref="Collection"/> and {@link
	''' Iterator} interfaces.  The Iterator provided in method {@link
	''' #iterator()} is <em>not</em> guaranteed to traverse the elements of
	''' the DelayQueue in any particular order.
	''' 
	''' <p>This class is a member of the
	''' <a href="{@docRoot}/../technotes/guides/collections/index.html">
	''' Java Collections Framework</a>.
	''' 
	''' @since 1.5
	''' @author Doug Lea </summary>
	''' @param <E> the type of elements held in this collection </param>
	Public Class DelayQueue(Of E As Delayed)
		Inherits AbstractQueue(Of E)
		Implements BlockingQueue(Of E)

		<NonSerialized> _
		Private ReadOnly lock As New java.util.concurrent.locks.ReentrantLock
		Private ReadOnly q As New PriorityQueue(Of E)

		''' <summary>
		''' Thread designated to wait for the element at the head of
		''' the queue.  This variant of the Leader-Follower pattern
		''' (http://www.cs.wustl.edu/~schmidt/POSA/POSA2/) serves to
		''' minimize unnecessary timed waiting.  When a thread becomes
		''' the leader, it waits only for the next delay to elapse, but
		''' other threads await indefinitely.  The leader thread must
		''' signal some other thread before returning from take() or
		''' poll(...), unless some other thread becomes leader in the
		''' interim.  Whenever the head of the queue is replaced with
		''' an element with an earlier expiration time, the leader
		''' field is invalidated by being reset to null, and some
		''' waiting thread, but not necessarily the current leader, is
		''' signalled.  So waiting threads must be prepared to acquire
		''' and lose leadership while waiting.
		''' </summary>
		Private leader As Thread = Nothing

		''' <summary>
		''' Condition signalled when a newer element becomes available
		''' at the head of the queue or a new thread may need to
		''' become leader.
		''' </summary>
		Private ReadOnly available As java.util.concurrent.locks.Condition = lock.newCondition()

		''' <summary>
		''' Creates a new {@code DelayQueue} that is initially empty.
		''' </summary>
		Public Sub New()
		End Sub

		''' <summary>
		''' Creates a {@code DelayQueue} initially containing the elements of the
		''' given collection of <seealso cref="Delayed"/> instances.
		''' </summary>
		''' <param name="c"> the collection of elements to initially contain </param>
		''' <exception cref="NullPointerException"> if the specified collection or any
		'''         of its elements are null </exception>
		Public Sub New(Of T1 As E)(  c As Collection(Of T1))
			Me.addAll(c)
		End Sub

		''' <summary>
		''' Inserts the specified element into this delay queue.
		''' </summary>
		''' <param name="e"> the element to add </param>
		''' <returns> {@code true} (as specified by <seealso cref="Collection#add"/>) </returns>
		''' <exception cref="NullPointerException"> if the specified element is null </exception>
		Public Overridable Function add(  e As E) As Boolean
			Return offer(e)
		End Function

		''' <summary>
		''' Inserts the specified element into this delay queue.
		''' </summary>
		''' <param name="e"> the element to add </param>
		''' <returns> {@code true} </returns>
		''' <exception cref="NullPointerException"> if the specified element is null </exception>
		Public Overridable Function offer(  e As E) As Boolean
			Dim lock As java.util.concurrent.locks.ReentrantLock = Me.lock
			lock.lock()
			Try
				q.offer(e)
				If q.peek() Is e Then
					leader = Nothing
					available.signal()
				End If
				Return True
			Finally
				lock.unlock()
			End Try
		End Function

		''' <summary>
		''' Inserts the specified element into this delay queue. As the queue is
		''' unbounded this method will never block.
		''' </summary>
		''' <param name="e"> the element to add </param>
		''' <exception cref="NullPointerException"> {@inheritDoc} </exception>
		Public Overridable Sub put(  e As E)
			offer(e)
		End Sub

		''' <summary>
		''' Inserts the specified element into this delay queue. As the queue is
		''' unbounded this method will never block.
		''' </summary>
		''' <param name="e"> the element to add </param>
		''' <param name="timeout"> This parameter is ignored as the method never blocks </param>
		''' <param name="unit"> This parameter is ignored as the method never blocks </param>
		''' <returns> {@code true} </returns>
		''' <exception cref="NullPointerException"> {@inheritDoc} </exception>
		Public Overridable Function offer(  e As E,   timeout As Long,   unit As TimeUnit) As Boolean
			Return offer(e)
		End Function

		''' <summary>
		''' Retrieves and removes the head of this queue, or returns {@code null}
		''' if this queue has no elements with an expired delay.
		''' </summary>
		''' <returns> the head of this queue, or {@code null} if this
		'''         queue has no elements with an expired delay </returns>
		Public Overridable Function poll() As E
			Dim lock As java.util.concurrent.locks.ReentrantLock = Me.lock
			lock.lock()
			Try
				Dim first As E = q.peek()
				If first Is Nothing OrElse first.getDelay(NANOSECONDS) > 0 Then
					Return Nothing
				Else
					Return q.poll()
				End If
			Finally
				lock.unlock()
			End Try
		End Function

		''' <summary>
		''' Retrieves and removes the head of this queue, waiting if necessary
		''' until an element with an expired delay is available on this queue.
		''' </summary>
		''' <returns> the head of this queue </returns>
		''' <exception cref="InterruptedException"> {@inheritDoc} </exception>
		Public Overridable Function take() As E Implements BlockingQueue(Of E).take
			Dim lock As java.util.concurrent.locks.ReentrantLock = Me.lock
			lock.lockInterruptibly()
			Try
				Do
					Dim first As E = q.peek()
					If first Is Nothing Then
						available.await()
					Else
						Dim delay As Long = first.getDelay(NANOSECONDS)
						If delay <= 0 Then Return q.poll()
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
				If leader Is Nothing AndAlso q.peek() IsNot Nothing Then available.signal()
				lock.unlock()
			End Try
		End Function

		''' <summary>
		''' Retrieves and removes the head of this queue, waiting if necessary
		''' until an element with an expired delay is available on this queue,
		''' or the specified wait time expires.
		''' </summary>
		''' <returns> the head of this queue, or {@code null} if the
		'''         specified waiting time elapses before an element with
		'''         an expired delay becomes available </returns>
		''' <exception cref="InterruptedException"> {@inheritDoc} </exception>
		Public Overridable Function poll(  timeout As Long,   unit As TimeUnit) As E Implements BlockingQueue(Of E).poll
			Dim nanos As Long = unit.toNanos(timeout)
			Dim lock As java.util.concurrent.locks.ReentrantLock = Me.lock
			lock.lockInterruptibly()
			Try
				Do
					Dim first As E = q.peek()
					If first Is Nothing Then
						If nanos <= 0 Then
							Return Nothing
						Else
							nanos = available.awaitNanos(nanos)
						End If
					Else
						Dim delay As Long = first.getDelay(NANOSECONDS)
						If delay <= 0 Then Return q.poll()
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
				If leader Is Nothing AndAlso q.peek() IsNot Nothing Then available.signal()
				lock.unlock()
			End Try
		End Function

		''' <summary>
		''' Retrieves, but does not remove, the head of this queue, or
		''' returns {@code null} if this queue is empty.  Unlike
		''' {@code poll}, if no expired elements are available in the queue,
		''' this method returns the element that will expire next,
		''' if one exists.
		''' </summary>
		''' <returns> the head of this queue, or {@code null} if this
		'''         queue is empty </returns>
		Public Overridable Function peek() As E
			Dim lock As java.util.concurrent.locks.ReentrantLock = Me.lock
			lock.lock()
			Try
				Return q.peek()
			Finally
				lock.unlock()
			End Try
		End Function

		Public Overridable Function size() As Integer
			Dim lock As java.util.concurrent.locks.ReentrantLock = Me.lock
			lock.lock()
			Try
				Return q.size()
			Finally
				lock.unlock()
			End Try
		End Function

		''' <summary>
		''' Returns first element only if it is expired.
		''' Used only by drainTo.  Call only when holding lock.
		''' </summary>
		Private Function peekExpired() As E
			' assert lock.isHeldByCurrentThread();
			Dim first As E = q.peek()
			Return If(first Is Nothing OrElse first.getDelay(NANOSECONDS) > 0, Nothing, first)
		End Function

		''' <exception cref="UnsupportedOperationException"> {@inheritDoc} </exception>
		''' <exception cref="ClassCastException">            {@inheritDoc} </exception>
		''' <exception cref="NullPointerException">          {@inheritDoc} </exception>
		''' <exception cref="IllegalArgumentException">      {@inheritDoc} </exception>
'JAVA TO VB CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
		Public Overridable Function drainTo(Of T1)(  c As Collection(Of T1)) As Integer
			If c Is Nothing Then Throw New NullPointerException
			If c Is Me Then Throw New IllegalArgumentException
			Dim lock As java.util.concurrent.locks.ReentrantLock = Me.lock
			lock.lock()
			Try
				Dim n As Integer = 0
				Dim e As E
'JAVA TO VB CONVERTER TODO TASK: Assignments within expressions are not supported in VB
				Do While (e = peekExpired()) IsNot Nothing
					c.add(e) ' In this order, in case add() throws.
					q.poll()
					n += 1
				Loop
				Return n
			Finally
				lock.unlock()
			End Try
		End Function

		''' <exception cref="UnsupportedOperationException"> {@inheritDoc} </exception>
		''' <exception cref="ClassCastException">            {@inheritDoc} </exception>
		''' <exception cref="NullPointerException">          {@inheritDoc} </exception>
		''' <exception cref="IllegalArgumentException">      {@inheritDoc} </exception>
'JAVA TO VB CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
		Public Overridable Function drainTo(Of T1)(  c As Collection(Of T1),   maxElements As Integer) As Integer
			If c Is Nothing Then Throw New NullPointerException
			If c Is Me Then Throw New IllegalArgumentException
			If maxElements <= 0 Then Return 0
			Dim lock As java.util.concurrent.locks.ReentrantLock = Me.lock
			lock.lock()
			Try
				Dim n As Integer = 0
				Dim e As E
'JAVA TO VB CONVERTER TODO TASK: Assignments within expressions are not supported in VB
				Do While n < maxElements AndAlso (e = peekExpired()) IsNot Nothing
					c.add(e) ' In this order, in case add() throws.
					q.poll()
					n += 1
				Loop
				Return n
			Finally
				lock.unlock()
			End Try
		End Function

		''' <summary>
		''' Atomically removes all of the elements from this delay queue.
		''' The queue will be empty after this call returns.
		''' Elements with an unexpired delay are not waited for; they are
		''' simply discarded from the queue.
		''' </summary>
		Public Overridable Sub clear()
			Dim lock As java.util.concurrent.locks.ReentrantLock = Me.lock
			lock.lock()
			Try
				q.clear()
			Finally
				lock.unlock()
			End Try
		End Sub

		''' <summary>
		''' Always returns {@code  java.lang.[Integer].MAX_VALUE} because
		''' a {@code DelayQueue} is not capacity constrained.
		''' </summary>
		''' <returns> {@code  java.lang.[Integer].MAX_VALUE} </returns>
		Public Overridable Function remainingCapacity() As Integer Implements BlockingQueue(Of E).remainingCapacity
			Return  java.lang.[Integer].Max_Value
		End Function

		''' <summary>
		''' Returns an array containing all of the elements in this queue.
		''' The returned array elements are in no particular order.
		''' 
		''' <p>The returned array will be "safe" in that no references to it are
		''' maintained by this queue.  (In other words, this method must allocate
		''' a new array).  The caller is thus free to modify the returned array.
		''' 
		''' <p>This method acts as bridge between array-based and collection-based
		''' APIs.
		''' </summary>
		''' <returns> an array containing all of the elements in this queue </returns>
		Public Overridable Function toArray() As Object()
			Dim lock As java.util.concurrent.locks.ReentrantLock = Me.lock
			lock.lock()
			Try
				Return q.ToArray()
			Finally
				lock.unlock()
			End Try
		End Function

		''' <summary>
		''' Returns an array containing all of the elements in this queue; the
		''' runtime type of the returned array is that of the specified array.
		''' The returned array elements are in no particular order.
		''' If the queue fits in the specified array, it is returned therein.
		''' Otherwise, a new array is allocated with the runtime type of the
		''' specified array and the size of this queue.
		''' 
		''' <p>If this queue fits in the specified array with room to spare
		''' (i.e., the array has more elements than this queue), the element in
		''' the array immediately following the end of the queue is set to
		''' {@code null}.
		''' 
		''' <p>Like the <seealso cref="#toArray()"/> method, this method acts as bridge between
		''' array-based and collection-based APIs.  Further, this method allows
		''' precise control over the runtime type of the output array, and may,
		''' under certain circumstances, be used to save allocation costs.
		''' 
		''' <p>The following code can be used to dump a delay queue into a newly
		''' allocated array of {@code Delayed}:
		''' 
		''' <pre> {@code Delayed[] a = q.toArray(new Delayed[0]);}</pre>
		''' 
		''' Note that {@code toArray(new Object[0])} is identical in function to
		''' {@code toArray()}.
		''' </summary>
		''' <param name="a"> the array into which the elements of the queue are to
		'''          be stored, if it is big enough; otherwise, a new array of the
		'''          same runtime type is allocated for this purpose </param>
		''' <returns> an array containing all of the elements in this queue </returns>
		''' <exception cref="ArrayStoreException"> if the runtime type of the specified array
		'''         is not a supertype of the runtime type of every element in
		'''         this queue </exception>
		''' <exception cref="NullPointerException"> if the specified array is null </exception>
		Public Overridable Function toArray(Of T)(  a As T()) As T()
			Dim lock As java.util.concurrent.locks.ReentrantLock = Me.lock
			lock.lock()
			Try
				Return q.ToArray(a)
			Finally
				lock.unlock()
			End Try
		End Function

		''' <summary>
		''' Removes a single instance of the specified element from this
		''' queue, if it is present, whether or not it has expired.
		''' </summary>
		Public Overridable Function remove(  o As Object) As Boolean Implements BlockingQueue(Of E).remove
			Dim lock As java.util.concurrent.locks.ReentrantLock = Me.lock
			lock.lock()
			Try
				Return q.remove(o)
			Finally
				lock.unlock()
			End Try
		End Function

		''' <summary>
		''' Identity-based version for use in Itr.remove
		''' </summary>
		Friend Overridable Sub removeEQ(  o As Object)
			Dim lock As java.util.concurrent.locks.ReentrantLock = Me.lock
			lock.lock()
			Try
				Dim it As [Iterator](Of E) = q.GetEnumerator()
				Do While it.MoveNext()
					If o Is it.Current Then
						it.remove()
						Exit Do
					End If
				Loop
			Finally
				lock.unlock()
			End Try
		End Sub

		''' <summary>
		''' Returns an iterator over all the elements (both expired and
		''' unexpired) in this queue. The iterator does not return the
		''' elements in any particular order.
		''' 
		''' <p>The returned iterator is
		''' <a href="package-summary.html#Weakly"><i>weakly consistent</i></a>.
		''' </summary>
		''' <returns> an iterator over the elements in this queue </returns>
		Public Overridable Function [iterator]() As [Iterator](Of E)
			Return New Itr(Me, ToArray())
		End Function

		''' <summary>
		''' Snapshot iterator that works off copy of underlying q array.
		''' </summary>
		Private Class Itr
			Implements Iterator(Of E)

			Private ReadOnly outerInstance As DelayQueue

			Friend ReadOnly array As Object() ' Array of all elements
			Friend cursor As Integer ' index of next element to return
			Friend lastRet As Integer ' index of last element, or -1 if no such

			Friend Sub New(  outerInstance As DelayQueue,   array As Object())
					Me.outerInstance = outerInstance
				lastRet = -1
				Me.array = array
			End Sub

			Public Overridable Function hasNext() As Boolean Implements Iterator(Of E).hasNext
				Return cursor < array.Length
			End Function

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
			Public Overridable Function [next]() As E Implements Iterator(Of E).next
				If cursor >= array.Length Then Throw New NoSuchElementException
				lastRet = cursor
					Dim tempVar As Integer = cursor
					cursor += 1
					Return CType(array(tempVar), E)
			End Function

			Public Overridable Sub remove() Implements Iterator(Of E).remove
				If lastRet < 0 Then Throw New IllegalStateException
				outerInstance.removeEQ(array(lastRet))
				lastRet = -1
			End Sub
		End Class

	End Class

End Namespace