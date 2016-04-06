Imports System
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
	''' A bounded <seealso cref="BlockingQueue blocking queue"/> backed by an
	''' array.  This queue orders elements FIFO (first-in-first-out).  The
	''' <em>head</em> of the queue is that element that has been on the
	''' queue the longest time.  The <em>tail</em> of the queue is that
	''' element that has been on the queue the shortest time. New elements
	''' are inserted at the tail of the queue, and the queue retrieval
	''' operations obtain elements at the head of the queue.
	''' 
	''' <p>This is a classic &quot;bounded buffer&quot;, in which a
	''' fixed-sized array holds elements inserted by producers and
	''' extracted by consumers.  Once created, the capacity cannot be
	''' changed.  Attempts to {@code put} an element into a full queue
	''' will result in the operation blocking; attempts to {@code take} an
	''' element from an empty queue will similarly block.
	''' 
	''' <p>This class supports an optional fairness policy for ordering
	''' waiting producer and consumer threads.  By default, this ordering
	''' is not guaranteed. However, a queue constructed with fairness set
	''' to {@code true} grants threads access in FIFO order. Fairness
	''' generally decreases throughput but reduces variability and avoids
	''' starvation.
	''' 
	''' <p>This class and its iterator implement all of the
	''' <em>optional</em> methods of the <seealso cref="Collection"/> and {@link
	''' Iterator} interfaces.
	''' 
	''' <p>This class is a member of the
	''' <a href="{@docRoot}/../technotes/guides/collections/index.html">
	''' Java Collections Framework</a>.
	''' 
	''' @since 1.5
	''' @author Doug Lea </summary>
	''' @param <E> the type of elements held in this collection </param>
	<Serializable> _
	Public Class ArrayBlockingQueue(Of E)
		Inherits java.util.AbstractQueue(Of E)
		Implements BlockingQueue(Of E)

		''' <summary>
		''' Serialization ID. This class relies on default serialization
		''' even for the items array, which is default-serialized, even if
		''' it is empty. Otherwise it could not be declared final, which is
		''' necessary here.
		''' </summary>
		Private Const serialVersionUID As Long = -817911632652898426L

		''' <summary>
		''' The queued items </summary>
		Friend ReadOnly items As Object()

		''' <summary>
		''' items index for next take, poll, peek or remove </summary>
		Friend takeIndex As Integer

		''' <summary>
		''' items index for next put, offer, or add </summary>
		Friend putIndex As Integer

		''' <summary>
		''' Number of elements in the queue </summary>
		Friend count As Integer

	'    
	'     * Concurrency control uses the classic two-condition algorithm
	'     * found in any textbook.
	'     

		''' <summary>
		''' Main lock guarding all access </summary>
		Friend ReadOnly lock As java.util.concurrent.locks.ReentrantLock

		''' <summary>
		''' Condition for waiting takes </summary>
		Private ReadOnly notEmpty As java.util.concurrent.locks.Condition

		''' <summary>
		''' Condition for waiting puts </summary>
		Private ReadOnly notFull As java.util.concurrent.locks.Condition

		''' <summary>
		''' Shared state for currently active iterators, or null if there
		''' are known not to be any.  Allows queue operations to update
		''' iterator state.
		''' </summary>
		<NonSerialized> _
		Friend itrs As Itrs = Nothing

		' Internal helper methods

		''' <summary>
		''' Circularly decrement i.
		''' </summary>
		Friend Function dec(  i As Integer) As Integer
			Return (If(i = 0, items.Length, i)) - 1
		End Function

		''' <summary>
		''' Returns item at index i.
		''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
		Friend Function itemAt(  i As Integer) As E
			Return CType(items(i), E)
		End Function

		''' <summary>
		''' Throws NullPointerException if argument is null.
		''' </summary>
		''' <param name="v"> the element </param>
		Private Shared Sub checkNotNull(  v As Object)
			If v Is Nothing Then Throw New NullPointerException
		End Sub

		''' <summary>
		''' Inserts element at current put position, advances, and signals.
		''' Call only when holding lock.
		''' </summary>
		Private Sub enqueue(  x As E)
			' assert lock.getHoldCount() == 1;
			' assert items[putIndex] == null;
			Dim items As Object() = Me.items
			items(putIndex) = x
			putIndex += 1
			If putIndex = items.Length Then putIndex = 0
			count += 1
			notEmpty.signal()
		End Sub

		''' <summary>
		''' Extracts element at current take position, advances, and signals.
		''' Call only when holding lock.
		''' </summary>
		Private Function dequeue() As E
			' assert lock.getHoldCount() == 1;
			' assert items[takeIndex] != null;
			Dim items As Object() = Me.items
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
			Dim x As E = CType(items(takeIndex), E)
			items(takeIndex) = Nothing
			takeIndex += 1
			If takeIndex = items.Length Then takeIndex = 0
			count -= 1
			If itrs IsNot Nothing Then itrs.elementDequeued()
			notFull.signal()
			Return x
		End Function

		''' <summary>
		''' Deletes item at array index removeIndex.
		''' Utility for remove(Object) and iterator.remove.
		''' Call only when holding lock.
		''' </summary>
		Friend Overridable Sub removeAt(  removeIndex As Integer)
			' assert lock.getHoldCount() == 1;
			' assert items[removeIndex] != null;
			' assert removeIndex >= 0 && removeIndex < items.length;
			Dim items As Object() = Me.items
			If removeIndex = takeIndex Then
				' removing front item; just advance
				items(takeIndex) = Nothing
				takeIndex += 1
				If takeIndex = items.Length Then takeIndex = 0
				count -= 1
				If itrs IsNot Nothing Then itrs.elementDequeued()
			Else
				' an "interior" remove

				' slide over all others up through putIndex.
				Dim putIndex As Integer = Me.putIndex
				Dim i As Integer = removeIndex
				Do
					Dim [next] As Integer = i + 1
					If [next] = items.Length Then [next] = 0
					If [next] <> putIndex Then
						items(i) = items([next])
						i = [next]
					Else
						items(i) = Nothing
						Me.putIndex = i
						Exit Do
					End If
				Loop
				count -= 1
				If itrs IsNot Nothing Then itrs.removedAt(removeIndex)
			End If
			notFull.signal()
		End Sub

		''' <summary>
		''' Creates an {@code ArrayBlockingQueue} with the given (fixed)
		''' capacity and default access policy.
		''' </summary>
		''' <param name="capacity"> the capacity of this queue </param>
		''' <exception cref="IllegalArgumentException"> if {@code capacity < 1} </exception>
		Public Sub New(  capacity As Integer)
			Me.New(capacity, False)
		End Sub

		''' <summary>
		''' Creates an {@code ArrayBlockingQueue} with the given (fixed)
		''' capacity and the specified access policy.
		''' </summary>
		''' <param name="capacity"> the capacity of this queue </param>
		''' <param name="fair"> if {@code true} then queue accesses for threads blocked
		'''        on insertion or removal, are processed in FIFO order;
		'''        if {@code false} the access order is unspecified. </param>
		''' <exception cref="IllegalArgumentException"> if {@code capacity < 1} </exception>
		Public Sub New(  capacity As Integer,   fair As Boolean)
			If capacity <= 0 Then Throw New IllegalArgumentException
			Me.items = New Object(capacity - 1){}
			lock = New java.util.concurrent.locks.ReentrantLock(fair)
			notEmpty = lock.newCondition()
			notFull = lock.newCondition()
		End Sub

		''' <summary>
		''' Creates an {@code ArrayBlockingQueue} with the given (fixed)
		''' capacity, the specified access policy and initially containing the
		''' elements of the given collection,
		''' added in traversal order of the collection's iterator.
		''' </summary>
		''' <param name="capacity"> the capacity of this queue </param>
		''' <param name="fair"> if {@code true} then queue accesses for threads blocked
		'''        on insertion or removal, are processed in FIFO order;
		'''        if {@code false} the access order is unspecified. </param>
		''' <param name="c"> the collection of elements to initially contain </param>
		''' <exception cref="IllegalArgumentException"> if {@code capacity} is less than
		'''         {@code c.size()}, or less than 1. </exception>
		''' <exception cref="NullPointerException"> if the specified collection or any
		'''         of its elements are null </exception>
		Public Sub New(Of T1 As E)(  capacity As Integer,   fair As Boolean,   c As ICollection(Of T1))
			Me.New(capacity, fair)

			Dim lock As java.util.concurrent.locks.ReentrantLock = Me.lock
			lock.lock() ' Lock only for visibility, not mutual exclusion
			Try
				Dim i As Integer = 0
				Try
					For Each e As E In c
						checkNotNull(e)
						items(i) = e
						i += 1
					Next e
				Catch ex As ArrayIndexOutOfBoundsException
					Throw New IllegalArgumentException
				End Try
				count = i
				putIndex = If(i = capacity, 0, i)
			Finally
				lock.unlock()
			End Try
		End Sub

		''' <summary>
		''' Inserts the specified element at the tail of this queue if it is
		''' possible to do so immediately without exceeding the queue's capacity,
		''' returning {@code true} upon success and throwing an
		''' {@code IllegalStateException} if this queue is full.
		''' </summary>
		''' <param name="e"> the element to add </param>
		''' <returns> {@code true} (as specified by <seealso cref="Collection#add"/>) </returns>
		''' <exception cref="IllegalStateException"> if this queue is full </exception>
		''' <exception cref="NullPointerException"> if the specified element is null </exception>
		Public Overridable Function add(  e As E) As Boolean
			Return MyBase.add(e)
		End Function

		''' <summary>
		''' Inserts the specified element at the tail of this queue if it is
		''' possible to do so immediately without exceeding the queue's capacity,
		''' returning {@code true} upon success and {@code false} if this queue
		''' is full.  This method is generally preferable to method <seealso cref="#add"/>,
		''' which can fail to insert an element only by throwing an exception.
		''' </summary>
		''' <exception cref="NullPointerException"> if the specified element is null </exception>
		Public Overridable Function offer(  e As E) As Boolean
			checkNotNull(e)
			Dim lock As java.util.concurrent.locks.ReentrantLock = Me.lock
			lock.lock()
			Try
				If count = items.Length Then
					Return False
				Else
					enqueue(e)
					Return True
				End If
			Finally
				lock.unlock()
			End Try
		End Function

		''' <summary>
		''' Inserts the specified element at the tail of this queue, waiting
		''' for space to become available if the queue is full.
		''' </summary>
		''' <exception cref="InterruptedException"> {@inheritDoc} </exception>
		''' <exception cref="NullPointerException"> {@inheritDoc} </exception>
		Public Overridable Sub put(  e As E)
			checkNotNull(e)
			Dim lock As java.util.concurrent.locks.ReentrantLock = Me.lock
			lock.lockInterruptibly()
			Try
				Do While count = items.Length
					notFull.await()
				Loop
				enqueue(e)
			Finally
				lock.unlock()
			End Try
		End Sub

		''' <summary>
		''' Inserts the specified element at the tail of this queue, waiting
		''' up to the specified wait time for space to become available if
		''' the queue is full.
		''' </summary>
		''' <exception cref="InterruptedException"> {@inheritDoc} </exception>
		''' <exception cref="NullPointerException"> {@inheritDoc} </exception>
		Public Overridable Function offer(  e As E,   timeout As Long,   unit As TimeUnit) As Boolean

			checkNotNull(e)
			Dim nanos As Long = unit.toNanos(timeout)
			Dim lock As java.util.concurrent.locks.ReentrantLock = Me.lock
			lock.lockInterruptibly()
			Try
				Do While count = items.Length
					If nanos <= 0 Then Return False
					nanos = notFull.awaitNanos(nanos)
				Loop
				enqueue(e)
				Return True
			Finally
				lock.unlock()
			End Try
		End Function

		Public Overridable Function poll() As E
			Dim lock As java.util.concurrent.locks.ReentrantLock = Me.lock
			lock.lock()
			Try
				Return If(count = 0, Nothing, dequeue())
			Finally
				lock.unlock()
			End Try
		End Function

		Public Overridable Function take() As E Implements BlockingQueue(Of E).take
			Dim lock As java.util.concurrent.locks.ReentrantLock = Me.lock
			lock.lockInterruptibly()
			Try
				Do While count = 0
					notEmpty.await()
				Loop
				Return dequeue()
			Finally
				lock.unlock()
			End Try
		End Function

		Public Overridable Function poll(  timeout As Long,   unit As TimeUnit) As E Implements BlockingQueue(Of E).poll
			Dim nanos As Long = unit.toNanos(timeout)
			Dim lock As java.util.concurrent.locks.ReentrantLock = Me.lock
			lock.lockInterruptibly()
			Try
				Do While count = 0
					If nanos <= 0 Then Return Nothing
					nanos = notEmpty.awaitNanos(nanos)
				Loop
				Return dequeue()
			Finally
				lock.unlock()
			End Try
		End Function

		Public Overridable Function peek() As E
			Dim lock As java.util.concurrent.locks.ReentrantLock = Me.lock
			lock.lock()
			Try
				Return itemAt(takeIndex) ' null when queue is empty
			Finally
				lock.unlock()
			End Try
		End Function

		' this doc comment is overridden to remove the reference to collections
		' greater in size than  java.lang.[Integer].MAX_VALUE
		''' <summary>
		''' Returns the number of elements in this queue.
		''' </summary>
		''' <returns> the number of elements in this queue </returns>
		Public Overridable Function size() As Integer
			Dim lock As java.util.concurrent.locks.ReentrantLock = Me.lock
			lock.lock()
			Try
				Return count
			Finally
				lock.unlock()
			End Try
		End Function

		' this doc comment is a modified copy of the inherited doc comment,
		' without the reference to unlimited queues.
		''' <summary>
		''' Returns the number of additional elements that this queue can ideally
		''' (in the absence of memory or resource constraints) accept without
		''' blocking. This is always equal to the initial capacity of this queue
		''' less the current {@code size} of this queue.
		''' 
		''' <p>Note that you <em>cannot</em> always tell if an attempt to insert
		''' an element will succeed by inspecting {@code remainingCapacity}
		''' because it may be the case that another thread is about to
		''' insert or remove an element.
		''' </summary>
		Public Overridable Function remainingCapacity() As Integer Implements BlockingQueue(Of E).remainingCapacity
			Dim lock As java.util.concurrent.locks.ReentrantLock = Me.lock
			lock.lock()
			Try
				Return items.Length - count
			Finally
				lock.unlock()
			End Try
		End Function

		''' <summary>
		''' Removes a single instance of the specified element from this queue,
		''' if it is present.  More formally, removes an element {@code e} such
		''' that {@code o.equals(e)}, if this queue contains one or more such
		''' elements.
		''' Returns {@code true} if this queue contained the specified element
		''' (or equivalently, if this queue changed as a result of the call).
		''' 
		''' <p>Removal of interior elements in circular array based queues
		''' is an intrinsically slow and disruptive operation, so should
		''' be undertaken only in exceptional circumstances, ideally
		''' only when the queue is known not to be accessible by other
		''' threads.
		''' </summary>
		''' <param name="o"> element to be removed from this queue, if present </param>
		''' <returns> {@code true} if this queue changed as a result of the call </returns>
		Public Overridable Function remove(  o As Object) As Boolean Implements BlockingQueue(Of E).remove
			If o Is Nothing Then Return False
			Dim items As Object() = Me.items
			Dim lock As java.util.concurrent.locks.ReentrantLock = Me.lock
			lock.lock()
			Try
				If count > 0 Then
					Dim putIndex As Integer = Me.putIndex
					Dim i As Integer = takeIndex
					Do
						If o.Equals(items(i)) Then
							removeAt(i)
							Return True
						End If
						i += 1
						If i = items.Length Then i = 0
					Loop While i <> putIndex
				End If
				Return False
			Finally
				lock.unlock()
			End Try
		End Function

		''' <summary>
		''' Returns {@code true} if this queue contains the specified element.
		''' More formally, returns {@code true} if and only if this queue contains
		''' at least one element {@code e} such that {@code o.equals(e)}.
		''' </summary>
		''' <param name="o"> object to be checked for containment in this queue </param>
		''' <returns> {@code true} if this queue contains the specified element </returns>
		Public Overridable Function contains(  o As Object) As Boolean Implements BlockingQueue(Of E).contains
			If o Is Nothing Then Return False
			Dim items As Object() = Me.items
			Dim lock As java.util.concurrent.locks.ReentrantLock = Me.lock
			lock.lock()
			Try
				If count > 0 Then
					Dim putIndex As Integer = Me.putIndex
					Dim i As Integer = takeIndex
					Do
						If o.Equals(items(i)) Then Return True
						i += 1
						If i = items.Length Then i = 0
					Loop While i <> putIndex
				End If
				Return False
			Finally
				lock.unlock()
			End Try
		End Function

		''' <summary>
		''' Returns an array containing all of the elements in this queue, in
		''' proper sequence.
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
			Dim a As Object()
			Dim lock As java.util.concurrent.locks.ReentrantLock = Me.lock
			lock.lock()
			Try
				Dim count As Integer = Me.count
				a = New Object(count - 1){}
				Dim n As Integer = items.Length - takeIndex
				If count <= n Then
					Array.Copy(items, takeIndex, a, 0, count)
				Else
					Array.Copy(items, takeIndex, a, 0, n)
					Array.Copy(items, 0, a, n, count - n)
				End If
			Finally
				lock.unlock()
			End Try
			Return a
		End Function

		''' <summary>
		''' Returns an array containing all of the elements in this queue, in
		''' proper sequence; the runtime type of the returned array is that of
		''' the specified array.  If the queue fits in the specified array, it
		''' is returned therein.  Otherwise, a new array is allocated with the
		''' runtime type of the specified array and the size of this queue.
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
		''' <p>Suppose {@code x} is a queue known to contain only strings.
		''' The following code can be used to dump the queue into a newly
		''' allocated array of {@code String}:
		''' 
		'''  <pre> {@code String[] y = x.toArray(new String[0]);}</pre>
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
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
		Public Overridable Function toArray(Of T)(  a As T()) As T()
			Dim items As Object() = Me.items
			Dim lock As java.util.concurrent.locks.ReentrantLock = Me.lock
			lock.lock()
			Try
				Dim count As Integer = Me.count
				Dim len As Integer = a.Length
				If len < count Then a = CType(java.lang.reflect.Array.newInstance(a.GetType().GetElementType(), count), T())
				Dim n As Integer = items.Length - takeIndex
				If count <= n Then
					Array.Copy(items, takeIndex, a, 0, count)
				Else
					Array.Copy(items, takeIndex, a, 0, n)
					Array.Copy(items, 0, a, n, count - n)
				End If
				If len > count Then a(count) = Nothing
			Finally
				lock.unlock()
			End Try
			Return a
		End Function

		Public Overrides Function ToString() As String
			Dim lock As java.util.concurrent.locks.ReentrantLock = Me.lock
			lock.lock()
			Try
				Dim k As Integer = count
				If k = 0 Then Return "[]"

				Dim items As Object() = Me.items
				Dim sb As New StringBuilder
				sb.append("["c)
				Dim i As Integer = takeIndex
				Do
					Dim e As Object = items(i)
					sb.append(If(e Is Me, "(this Collection)", e))
					k -= 1
					If k = 0 Then Return sb.append("]"c).ToString()
					sb.append(","c).append(" "c)
					i += 1
					If i = items.Length Then i = 0
				Loop
			Finally
				lock.unlock()
			End Try
		End Function

		''' <summary>
		''' Atomically removes all of the elements from this queue.
		''' The queue will be empty after this call returns.
		''' </summary>
		Public Overridable Sub clear()
			Dim items As Object() = Me.items
			Dim lock As java.util.concurrent.locks.ReentrantLock = Me.lock
			lock.lock()
			Try
				Dim k As Integer = count
				If k > 0 Then
					Dim putIndex As Integer = Me.putIndex
					Dim i As Integer = takeIndex
					Do
						items(i) = Nothing
						i += 1
						If i = items.Length Then i = 0
					Loop While i <> putIndex
					takeIndex = putIndex
					count = 0
					If itrs IsNot Nothing Then itrs.queueIsEmpty()
					Do While k > 0 AndAlso lock.hasWaiters(notFull)
						notFull.signal()
						k -= 1
					Loop
				End If
			Finally
				lock.unlock()
			End Try
		End Sub

		''' <exception cref="UnsupportedOperationException"> {@inheritDoc} </exception>
		''' <exception cref="ClassCastException">            {@inheritDoc} </exception>
		''' <exception cref="NullPointerException">          {@inheritDoc} </exception>
		''' <exception cref="IllegalArgumentException">      {@inheritDoc} </exception>
'JAVA TO VB CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
		Public Overridable Function drainTo(Of T1)(  c As ICollection(Of T1)) As Integer Implements BlockingQueue(Of E).drainTo
			Return drainTo(c,  java.lang.[Integer].Max_Value)
		End Function

		''' <exception cref="UnsupportedOperationException"> {@inheritDoc} </exception>
		''' <exception cref="ClassCastException">            {@inheritDoc} </exception>
		''' <exception cref="NullPointerException">          {@inheritDoc} </exception>
		''' <exception cref="IllegalArgumentException">      {@inheritDoc} </exception>
'JAVA TO VB CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
		Public Overridable Function drainTo(Of T1)(  c As ICollection(Of T1),   maxElements As Integer) As Integer Implements BlockingQueue(Of E).drainTo
			checkNotNull(c)
			If c Is Me Then Throw New IllegalArgumentException
			If maxElements <= 0 Then Return 0
			Dim items As Object() = Me.items
			Dim lock As java.util.concurrent.locks.ReentrantLock = Me.lock
			lock.lock()
			Try
				Dim n As Integer = System.Math.Min(maxElements, count)
				Dim take As Integer = takeIndex
				Dim i As Integer = 0
				Try
					Do While i < n
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
						Dim x As E = CType(items(take), E)
						c.Add(x)
						items(take) = Nothing
						take += 1
						If take = items.Length Then take = 0
						i += 1
					Loop
					Return n
				Finally
					' Restore invariants even if c.add() threw
					If i > 0 Then
						count -= i
						takeIndex = take
						If itrs IsNot Nothing Then
							If count = 0 Then
								itrs.queueIsEmpty()
							ElseIf i > take Then
								itrs.takeIndexWrapped()
							End If
						End If
						Do While i > 0 AndAlso lock.hasWaiters(notFull)
							notFull.signal()
							i -= 1
						Loop
					End If
				End Try
			Finally
				lock.unlock()
			End Try
		End Function

		''' <summary>
		''' Returns an iterator over the elements in this queue in proper sequence.
		''' The elements will be returned in order from first (head) to last (tail).
		''' 
		''' <p>The returned iterator is
		''' <a href="package-summary.html#Weakly"><i>weakly consistent</i></a>.
		''' </summary>
		''' <returns> an iterator over the elements in this queue in proper sequence </returns>
		Public Overridable Function [iterator]() As IEnumerator(Of E)
			Return New Itr(Me)
		End Function

		''' <summary>
		''' Shared data between iterators and their queue, allowing queue
		''' modifications to update iterators when elements are removed.
		''' 
		''' This adds a lot of complexity for the sake of correctly
		''' handling some uncommon operations, but the combination of
		''' circular-arrays and supporting interior removes (i.e., those
		''' not at head) would cause iterators to sometimes lose their
		''' places and/or (re)report elements they shouldn't.  To avoid
		''' this, when a queue has one or more iterators, it keeps iterator
		''' state consistent by:
		''' 
		''' (1) keeping track of the number of "cycles", that is, the
		'''     number of times takeIndex has wrapped around to 0.
		''' (2) notifying all iterators via the callback removedAt whenever
		'''     an interior element is removed (and thus other elements may
		'''     be shifted).
		''' 
		''' These suffice to eliminate iterator inconsistencies, but
		''' unfortunately add the secondary responsibility of maintaining
		''' the list of iterators.  We track all active iterators in a
		''' simple linked list (accessed only when the queue's lock is
		''' held) of weak references to Itr.  The list is cleaned up using
		''' 3 different mechanisms:
		''' 
		''' (1) Whenever a new iterator is created, do some O(1) checking for
		'''     stale list elements.
		''' 
		''' (2) Whenever takeIndex wraps around to 0, check for iterators
		'''     that have been unused for more than one wrap-around cycle.
		''' 
		''' (3) Whenever the queue becomes empty, all iterators are notified
		'''     and this entire data structure is discarded.
		''' 
		''' So in addition to the removedAt callback that is necessary for
		''' correctness, iterators have the shutdown and takeIndexWrapped
		''' callbacks that help remove stale iterators from the list.
		''' 
		''' Whenever a list element is examined, it is expunged if either
		''' the GC has determined that the iterator is discarded, or if the
		''' iterator reports that it is "detached" (does not need any
		''' further state updates).  Overhead is maximal when takeIndex
		''' never advances, iterators are discarded before they are
		''' exhausted, and all removals are interior removes, in which case
		''' all stale iterators are discovered by the GC.  But even in this
		''' case we don't increase the amortized complexity.
		''' 
		''' Care must be taken to keep list sweeping methods from
		''' reentrantly invoking another such method, causing subtle
		''' corruption bugs.
		''' </summary>
		Friend Class Itrs
			Private ReadOnly outerInstance As ArrayBlockingQueue


			''' <summary>
			''' Node in a linked list of weak iterator references.
			''' </summary>
			Private Class Node
				Inherits WeakReference(Of Itr)

				Private ReadOnly outerInstance As ArrayBlockingQueue.Itrs

				Friend [next] As Node

				Friend Sub New(  outerInstance As ArrayBlockingQueue.Itrs,   [iterator] As Itr,   [next] As Node)
						Me.outerInstance = outerInstance
					MyBase.New([iterator])
					Me.next = [next]
				End Sub
			End Class

			''' <summary>
			''' Incremented whenever takeIndex wraps around to 0 </summary>
			Friend cycles As Integer = 0

			''' <summary>
			''' Linked list of weak iterator references </summary>
			Private head As Node

			''' <summary>
			''' Used to expunge stale iterators </summary>
			Private sweeper As Node = Nothing

			Private Const SHORT_SWEEP_PROBES As Integer = 4
			Private Const LONG_SWEEP_PROBES As Integer = 16

			Friend Sub New(  outerInstance As ArrayBlockingQueue,   initial As Itr)
					Me.outerInstance = outerInstance
				register(initial)
			End Sub

			''' <summary>
			''' Sweeps itrs, looking for and expunging stale iterators.
			''' If at least one was found, tries harder to find more.
			''' Called only from iterating thread.
			''' </summary>
			''' <param name="tryHarder"> whether to start in try-harder mode, because
			''' there is known to be at least one iterator to collect </param>
			Friend Overridable Sub doSomeSweeping(  tryHarder As Boolean)
				' assert lock.getHoldCount() == 1;
				' assert head != null;
				Dim probes As Integer = If(tryHarder, LONG_SWEEP_PROBES, SHORT_SWEEP_PROBES)
				Dim o, p As Node
				Dim sweeper As Node = Me.sweeper
				Dim passedGo As Boolean ' to limit search to one full sweep

				If sweeper Is Nothing Then
					o = Nothing
					p = head
					passedGo = True
				Else
					o = sweeper
					p = o.next
					passedGo = False
				End If

				Do While probes > 0
					If p Is Nothing Then
						If passedGo Then Exit Do
						o = Nothing
						p = head
						passedGo = True
					End If
					Dim it As Itr = p.get()
					Dim [next] As Node = p.next
					If it Is Nothing OrElse it.detached Then
						' found a discarded/exhausted iterator
						probes = LONG_SWEEP_PROBES ' "try harder"
						' unlink p
						p.clear()
						p.next = Nothing
						If o Is Nothing Then
							head = [next]
							If [next] Is Nothing Then
								' We've run out of iterators to track; retire
								outerInstance.itrs = Nothing
								Return
							End If
						Else
							o.next = [next]
						End If
					Else
						o = p
					End If
					p = [next]
					probes -= 1
				Loop

				Me.sweeper = If(p Is Nothing, Nothing, o)
			End Sub

			''' <summary>
			''' Adds a new iterator to the linked list of tracked iterators.
			''' </summary>
			Friend Overridable Sub register(  itr As Itr)
				' assert lock.getHoldCount() == 1;
				head = New Node(Me, itr, head)
			End Sub

			''' <summary>
			''' Called whenever takeIndex wraps around to 0.
			''' 
			''' Notifies all iterators, and expunges any that are now stale.
			''' </summary>
			Friend Overridable Sub takeIndexWrapped()
				' assert lock.getHoldCount() == 1;
				cycles += 1
				Dim o As Node = Nothing
				Dim p As Node = head
				Do While p IsNot Nothing
					Dim it As Itr = p.get()
					Dim [next] As Node = p.next
					If it Is Nothing OrElse it.takeIndexWrapped() Then
						' unlink p
						' assert it == null || it.isDetached();
						p.clear()
						p.next = Nothing
						If o Is Nothing Then
							head = [next]
						Else
							o.next = [next]
						End If
					Else
						o = p
					End If
					p = [next]
				Loop
				If head Is Nothing Then ' no more iterators to track outerInstance.itrs = Nothing
			End Sub

			''' <summary>
			''' Called whenever an interior remove (not at takeIndex) occurred.
			''' 
			''' Notifies all iterators, and expunges any that are now stale.
			''' </summary>
			Friend Overridable Sub removedAt(  removedIndex As Integer)
				Dim o As Node = Nothing
				Dim p As Node = head
				Do While p IsNot Nothing
					Dim it As Itr = p.get()
					Dim [next] As Node = p.next
					If it Is Nothing OrElse it.removedAt(removedIndex) Then
						' unlink p
						' assert it == null || it.isDetached();
						p.clear()
						p.next = Nothing
						If o Is Nothing Then
							head = [next]
						Else
							o.next = [next]
						End If
					Else
						o = p
					End If
					p = [next]
				Loop
				If head Is Nothing Then ' no more iterators to track outerInstance.itrs = Nothing
			End Sub

			''' <summary>
			''' Called whenever the queue becomes empty.
			''' 
			''' Notifies all active iterators that the queue is empty,
			''' clears all weak refs, and unlinks the itrs datastructure.
			''' </summary>
			Friend Overridable Sub queueIsEmpty()
				' assert lock.getHoldCount() == 1;
				Dim p As Node = head
				Do While p IsNot Nothing
					Dim it As Itr = p.get()
					If it IsNot Nothing Then
						p.clear()
						it.shutdown()
					End If
					p = p.next
				Loop
				head = Nothing
				outerInstance.itrs = Nothing
			End Sub

			''' <summary>
			''' Called whenever an element has been dequeued (at takeIndex).
			''' </summary>
			Friend Overridable Sub elementDequeued()
				' assert lock.getHoldCount() == 1;
				If outerInstance.count = 0 Then
					queueIsEmpty()
				ElseIf outerInstance.takeIndex = 0 Then
					takeIndexWrapped()
				End If
			End Sub
		End Class

		''' <summary>
		''' Iterator for ArrayBlockingQueue.
		''' 
		''' To maintain weak consistency with respect to puts and takes, we
		''' read ahead one slot, so as to not report hasNext true but then
		''' not have an element to return.
		''' 
		''' We switch into "detached" mode (allowing prompt unlinking from
		''' itrs without help from the GC) when all indices are negative, or
		''' when hasNext returns false for the first time.  This allows the
		''' iterator to track concurrent updates completely accurately,
		''' except for the corner case of the user calling Iterator.remove()
		''' after hasNext() returned false.  Even in this case, we ensure
		''' that we don't remove the wrong element by keeping track of the
		''' expected element to remove, in lastItem.  Yes, we may fail to
		''' remove lastItem from the queue if it moved due to an interleaved
		''' interior remove while in detached mode.
		''' </summary>
		Private Class Itr
			Implements IEnumerator(Of E)

			Private ReadOnly outerInstance As ArrayBlockingQueue

			''' <summary>
			''' Index to look for new nextItem; NONE at end </summary>
			Private cursor As Integer

			''' <summary>
			''' Element to be returned by next call to next(); null if none </summary>
			Private nextItem As E

			''' <summary>
			''' Index of nextItem; NONE if none, REMOVED if removed elsewhere </summary>
			Private nextIndex As Integer

			''' <summary>
			''' Last element returned; null if none or not detached. </summary>
			Private lastItem As E

			''' <summary>
			''' Index of lastItem, NONE if none, REMOVED if removed elsewhere </summary>
			Private lastRet As Integer

			''' <summary>
			''' Previous value of takeIndex, or DETACHED when detached </summary>
			Private prevTakeIndex As Integer

			''' <summary>
			''' Previous value of iters.cycles </summary>
			Private prevCycles As Integer

			''' <summary>
			''' Special index value indicating "not available" or "undefined" </summary>
			Private Const NONE As Integer = -1

			''' <summary>
			''' Special index value indicating "removed elsewhere", that is,
			''' removed by some operation other than a call to this.remove().
			''' </summary>
			Private Const REMOVED As Integer = -2

			''' <summary>
			''' Special value for prevTakeIndex indicating "detached mode" </summary>
			Private Const DETACHED As Integer = -3

			Friend Sub New(  outerInstance As ArrayBlockingQueue)
					Me.outerInstance = outerInstance
				' assert lock.getHoldCount() == 0;
				lastRet = NONE
				Dim lock As java.util.concurrent.locks.ReentrantLock = outerInstance.lock
				lock.lock()
				Try
					If outerInstance.count = 0 Then
						' assert itrs == null;
						cursor = NONE
						nextIndex = NONE
						prevTakeIndex = DETACHED
					Else
						Dim takeIndex As Integer = outerInstance.takeIndex
						prevTakeIndex = takeIndex
'JAVA TO VB CONVERTER TODO TASK: Assignments within expressions are not supported in VB
						nextItem = outerInstance.itemAt(nextIndex = takeIndex)
						cursor = incCursor(takeIndex)
						If outerInstance.itrs Is Nothing Then
							outerInstance.itrs = New Itrs(Me)
						Else
							outerInstance.itrs.register(Me) ' in this order
							outerInstance.itrs.doSomeSweeping(False)
						End If
						prevCycles = outerInstance.itrs.cycles
						' assert takeIndex >= 0;
						' assert prevTakeIndex == takeIndex;
						' assert nextIndex >= 0;
						' assert nextItem != null;
					End If
				Finally
					lock.unlock()
				End Try
			End Sub

			Friend Overridable Property detached As Boolean
				Get
					' assert lock.getHoldCount() == 1;
					Return prevTakeIndex < 0
				End Get
			End Property

			Private Function incCursor(  index As Integer) As Integer
				' assert lock.getHoldCount() == 1;
				index += 1
				If index = outerInstance.items.Length Then index = 0
				If index = outerInstance.putIndex Then index = NONE
				Return index
			End Function

			''' <summary>
			''' Returns true if index is invalidated by the given number of
			''' dequeues, starting from prevTakeIndex.
			''' </summary>
			Private Function invalidated(  index As Integer,   prevTakeIndex As Integer,   dequeues As Long,   length As Integer) As Boolean
				If index < 0 Then Return False
				Dim distance As Integer = index - prevTakeIndex
				If distance < 0 Then distance += length
				Return dequeues > distance
			End Function

			''' <summary>
			''' Adjusts indices to incorporate all dequeues since the last
			''' operation on this iterator.  Call only from iterating thread.
			''' </summary>
			Private Sub incorporateDequeues()
				' assert lock.getHoldCount() == 1;
				' assert itrs != null;
				' assert !isDetached();
				' assert count > 0;

				Dim cycles As Integer = outerInstance.itrs.cycles
				Dim takeIndex As Integer = outerInstance.takeIndex
				Dim prevCycles As Integer = Me.prevCycles
				Dim prevTakeIndex As Integer = Me.prevTakeIndex

				If cycles <> prevCycles OrElse takeIndex <> prevTakeIndex Then
					Dim len As Integer = outerInstance.items.Length
					' how far takeIndex has advanced since the previous
					' operation of this iterator
					Dim dequeues As Long = (cycles - prevCycles) * len + (takeIndex - prevTakeIndex)

					' Check indices for invalidation
					If invalidated(lastRet, prevTakeIndex, dequeues, len) Then lastRet = REMOVED
					If invalidated(nextIndex, prevTakeIndex, dequeues, len) Then nextIndex = REMOVED
					If invalidated(cursor, prevTakeIndex, dequeues, len) Then cursor = takeIndex

					If cursor < 0 AndAlso nextIndex < 0 AndAlso lastRet < 0 Then
						detach()
					Else
						Me.prevCycles = cycles
						Me.prevTakeIndex = takeIndex
					End If
				End If
			End Sub

			''' <summary>
			''' Called when itrs should stop tracking this iterator, either
			''' because there are no more indices to update (cursor < 0 &&
			''' nextIndex < 0 && lastRet < 0) or as a special exception, when
			''' lastRet >= 0, because hasNext() is about to return false for the
			''' first time.  Call only from iterating thread.
			''' </summary>
			Private Sub detach()
				' Switch to detached mode
				' assert lock.getHoldCount() == 1;
				' assert cursor == NONE;
				' assert nextIndex < 0;
				' assert lastRet < 0 || nextItem == null;
				' assert lastRet < 0 ^ lastItem != null;
				If prevTakeIndex >= 0 Then
					' assert itrs != null;
					prevTakeIndex = DETACHED
					' try to unlink from itrs (but not too hard)
					outerInstance.itrs.doSomeSweeping(True)
				End If
			End Sub

			''' <summary>
			''' For performance reasons, we would like not to acquire a lock in
			''' hasNext in the common case.  To allow for this, we only access
			''' fields (i.e. nextItem) that are not modified by update operations
			''' triggered by queue modifications.
			''' </summary>
			Public Overridable Function hasNext() As Boolean
				' assert lock.getHoldCount() == 0;
				If nextItem IsNot Nothing Then Return True
				noNext()
				Return False
			End Function

			Private Sub noNext()
				Dim lock As java.util.concurrent.locks.ReentrantLock = outerInstance.lock
				lock.lock()
				Try
					' assert cursor == NONE;
					' assert nextIndex == NONE;
					If Not detached Then
						' assert lastRet >= 0;
						incorporateDequeues() ' might update lastRet
						If lastRet >= 0 Then
							lastItem = outerInstance.itemAt(lastRet)
							' assert lastItem != null;
							detach()
						End If
					End If
					' assert isDetached();
					' assert lastRet < 0 ^ lastItem != null;
				Finally
					lock.unlock()
				End Try
			End Sub

			Public Overridable Function [next]() As E
				' assert lock.getHoldCount() == 0;
				Dim x As E = nextItem
				If x Is Nothing Then Throw New java.util.NoSuchElementException
				Dim lock As java.util.concurrent.locks.ReentrantLock = outerInstance.lock
				lock.lock()
				Try
					If Not detached Then incorporateDequeues()
					' assert nextIndex != NONE;
					' assert lastItem == null;
					lastRet = nextIndex
					Dim cursor As Integer = Me.cursor
					If cursor >= 0 Then
'JAVA TO VB CONVERTER TODO TASK: Assignments within expressions are not supported in VB
						nextItem = outerInstance.itemAt(nextIndex = cursor)
						' assert nextItem != null;
						Me.cursor = incCursor(cursor)
					Else
						nextIndex = NONE
						nextItem = Nothing
					End If
				Finally
					lock.unlock()
				End Try
				Return x
			End Function

			Public Overridable Sub remove()
				' assert lock.getHoldCount() == 0;
				Dim lock As java.util.concurrent.locks.ReentrantLock = outerInstance.lock
				lock.lock()
				Try
					If Not detached Then incorporateDequeues() ' might update lastRet or detach
					Dim lastRet As Integer = Me.lastRet
					Me.lastRet = NONE
					If lastRet >= 0 Then
						If Not detached Then
							outerInstance.removeAt(lastRet)
						Else
							Dim lastItem As E = Me.lastItem
							' assert lastItem != null;
							Me.lastItem = Nothing
							If outerInstance.itemAt(lastRet) Is lastItem Then outerInstance.removeAt(lastRet)
						End If
					ElseIf lastRet = NONE Then
						Throw New IllegalStateException
					End If
					' else lastRet == REMOVED and the last returned element was
					' previously asynchronously removed via an operation other
					' than this.remove(), so nothing to do.

					If cursor < 0 AndAlso nextIndex < 0 Then detach()
				Finally
					lock.unlock()
					' assert lastRet == NONE;
					' assert lastItem == null;
				End Try
			End Sub

			''' <summary>
			''' Called to notify the iterator that the queue is empty, or that it
			''' has fallen hopelessly behind, so that it should abandon any
			''' further iteration, except possibly to return one more element
			''' from next(), as promised by returning true from hasNext().
			''' </summary>
			Friend Overridable Sub shutdown()
				' assert lock.getHoldCount() == 1;
				cursor = NONE
				If nextIndex >= 0 Then nextIndex = REMOVED
				If lastRet >= 0 Then
					lastRet = REMOVED
					lastItem = Nothing
				End If
				prevTakeIndex = DETACHED
				' Don't set nextItem to null because we must continue to be
				' able to return it on next().
				'
				' Caller will unlink from itrs when convenient.
			End Sub

			Private Function distance(  index As Integer,   prevTakeIndex As Integer,   length As Integer) As Integer
				Dim distance_Renamed As Integer = index - prevTakeIndex
				If distance_Renamed < 0 Then distance_Renamed += length
				Return distance_Renamed
			End Function

			''' <summary>
			''' Called whenever an interior remove (not at takeIndex) occurred.
			''' </summary>
			''' <returns> true if this iterator should be unlinked from itrs </returns>
			Friend Overridable Function removedAt(  removedIndex As Integer) As Boolean
				' assert lock.getHoldCount() == 1;
				If detached Then Return True

				Dim cycles As Integer = outerInstance.itrs.cycles
				Dim takeIndex As Integer = outerInstance.takeIndex
				Dim prevCycles As Integer = Me.prevCycles
				Dim prevTakeIndex As Integer = Me.prevTakeIndex
				Dim len As Integer = outerInstance.items.Length
				Dim cycleDiff As Integer = cycles - prevCycles
				If removedIndex < takeIndex Then cycleDiff += 1
				Dim removedDistance As Integer = (cycleDiff * len) + (removedIndex - prevTakeIndex)
				' assert removedDistance >= 0;
				Dim cursor As Integer = Me.cursor
				If cursor >= 0 Then
					Dim x As Integer = distance(cursor, prevTakeIndex, len)
					If x = removedDistance Then
						If cursor = outerInstance.putIndex Then
								cursor = NONE
								Me.cursor = cursor
						End If
					ElseIf x > removedDistance Then
						' assert cursor != prevTakeIndex;
							cursor = outerInstance.dec(cursor)
							Me.cursor = cursor
					End If
				End If
				Dim lastRet As Integer = Me.lastRet
				If lastRet >= 0 Then
					Dim x As Integer = distance(lastRet, prevTakeIndex, len)
					If x = removedDistance Then
							lastRet = REMOVED
							Me.lastRet = lastRet
					ElseIf x > removedDistance Then
							lastRet = outerInstance.dec(lastRet)
							Me.lastRet = lastRet
					End If
				End If
				Dim nextIndex As Integer = Me.nextIndex
				If nextIndex >= 0 Then
					Dim x As Integer = distance(nextIndex, prevTakeIndex, len)
					If x = removedDistance Then
							nextIndex = REMOVED
							Me.nextIndex = nextIndex
					ElseIf x > removedDistance Then
							nextIndex = outerInstance.dec(nextIndex)
							Me.nextIndex = nextIndex
					End If
				ElseIf cursor < 0 AndAlso nextIndex < 0 AndAlso lastRet < 0 Then
					Me.prevTakeIndex = DETACHED
					Return True
				End If
				Return False
			End Function

			''' <summary>
			''' Called whenever takeIndex wraps around to zero.
			''' </summary>
			''' <returns> true if this iterator should be unlinked from itrs </returns>
			Friend Overridable Function takeIndexWrapped() As Boolean
				' assert lock.getHoldCount() == 1;
				If detached Then Return True
				If outerInstance.itrs.cycles - prevCycles > 1 Then
					' All the elements that existed at the time of the last
					' operation are gone, so abandon further iteration.
					shutdown()
					Return True
				End If
				Return False
			End Function

	'         /** Uncomment for debugging. */
	'         public String toString() {
	'             return ("cursor=" + cursor + " " +
	'                     "nextIndex=" + nextIndex + " " +
	'                     "lastRet=" + lastRet + " " +
	'                     "nextItem=" + nextItem + " " +
	'                     "lastItem=" + lastItem + " " +
	'                     "prevCycles=" + prevCycles + " " +
	'                     "prevTakeIndex=" + prevTakeIndex + " " +
	'                     "size()=" + size() + " " +
	'                     "remainingCapacity()=" + remainingCapacity());
	'         }
		End Class

		''' <summary>
		''' Returns a <seealso cref="Spliterator"/> over the elements in this queue.
		''' 
		''' <p>The returned spliterator is
		''' <a href="package-summary.html#Weakly"><i>weakly consistent</i></a>.
		''' 
		''' <p>The {@code Spliterator} reports <seealso cref="Spliterator#CONCURRENT"/>,
		''' <seealso cref="Spliterator#ORDERED"/>, and <seealso cref="Spliterator#NONNULL"/>.
		''' 
		''' @implNote
		''' The {@code Spliterator} implements {@code trySplit} to permit limited
		''' parallelism.
		''' </summary>
		''' <returns> a {@code Spliterator} over the elements in this queue
		''' @since 1.8 </returns>
		Public Overridable Function spliterator() As java.util.Spliterator(Of E)
			Return java.util.Spliterators.spliterator(Me, java.util.Spliterator.ORDERED Or java.util.Spliterator.NONNULL Or java.util.Spliterator.CONCURRENT)
		End Function

	End Class

End Namespace