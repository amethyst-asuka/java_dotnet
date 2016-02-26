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
	''' An optionally-bounded <seealso cref="BlockingQueue blocking queue"/> based on
	''' linked nodes.
	''' This queue orders elements FIFO (first-in-first-out).
	''' The <em>head</em> of the queue is that element that has been on the
	''' queue the longest time.
	''' The <em>tail</em> of the queue is that element that has been on the
	''' queue the shortest time. New elements
	''' are inserted at the tail of the queue, and the queue retrieval
	''' operations obtain elements at the head of the queue.
	''' Linked queues typically have higher throughput than array-based queues but
	''' less predictable performance in most concurrent applications.
	''' 
	''' <p>The optional capacity bound constructor argument serves as a
	''' way to prevent excessive queue expansion. The capacity, if unspecified,
	''' is equal to <seealso cref="Integer#MAX_VALUE"/>.  Linked nodes are
	''' dynamically created upon each insertion unless this would bring the
	''' queue above capacity.
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
	Public Class LinkedBlockingQueue(Of E)
		Inherits java.util.AbstractQueue(Of E)
		Implements BlockingQueue(Of E)

		Private Const serialVersionUID As Long = -6903933977591709194L

	'    
	'     * A variant of the "two lock queue" algorithm.  The putLock gates
	'     * entry to put (and offer), and has an associated condition for
	'     * waiting puts.  Similarly for the takeLock.  The "count" field
	'     * that they both rely on is maintained as an atomic to avoid
	'     * needing to get both locks in most cases. Also, to minimize need
	'     * for puts to get takeLock and vice-versa, cascading notifies are
	'     * used. When a put notices that it has enabled at least one take,
	'     * it signals taker. That taker in turn signals others if more
	'     * items have been entered since the signal. And symmetrically for
	'     * takes signalling puts. Operations such as remove(Object) and
	'     * iterators acquire both locks.
	'     *
	'     * Visibility between writers and readers is provided as follows:
	'     *
	'     * Whenever an element is enqueued, the putLock is acquired and
	'     * count updated.  A subsequent reader guarantees visibility to the
	'     * enqueued Node by either acquiring the putLock (via fullyLock)
	'     * or by acquiring the takeLock, and then reading n = count.get();
	'     * this gives visibility to the first n items.
	'     *
	'     * To implement weakly consistent iterators, it appears we need to
	'     * keep all Nodes GC-reachable from a predecessor dequeued Node.
	'     * That would cause two problems:
	'     * - allow a rogue Iterator to cause unbounded memory retention
	'     * - cause cross-generational linking of old Nodes to new Nodes if
	'     *   a Node was tenured while live, which generational GCs have a
	'     *   hard time dealing with, causing repeated major collections.
	'     * However, only non-deleted Nodes need to be reachable from
	'     * dequeued Nodes, and reachability does not necessarily have to
	'     * be of the kind understood by the GC.  We use the trick of
	'     * linking a Node that has just been dequeued to itself.  Such a
	'     * self-link implicitly means to advance to head.next.
	'     

		''' <summary>
		''' Linked list node class
		''' </summary>
		Friend Class Node(Of E)
			Friend item As E

			''' <summary>
			''' One of:
			''' - the real successor Node
			''' - this Node, meaning the successor is head.next
			''' - null, meaning there is no successor (this is the last node)
			''' </summary>
			Friend [next] As Node(Of E)

			Friend Sub New(ByVal x As E)
				item = x
			End Sub
		End Class

		''' <summary>
		''' The capacity bound, or  [Integer].MAX_VALUE if none </summary>
		Private ReadOnly capacity As Integer

		''' <summary>
		''' Current number of elements </summary>
		Private ReadOnly count As New java.util.concurrent.atomic.AtomicInteger

		''' <summary>
		''' Head of linked list.
		''' Invariant: head.item == null
		''' </summary>
		<NonSerialized> _
		Friend head As Node(Of E)

		''' <summary>
		''' Tail of linked list.
		''' Invariant: last.next == null
		''' </summary>
		<NonSerialized> _
		Private last As Node(Of E)

		''' <summary>
		''' Lock held by take, poll, etc </summary>
		Private ReadOnly takeLock As New java.util.concurrent.locks.ReentrantLock

		''' <summary>
		''' Wait queue for waiting takes </summary>
		Private ReadOnly notEmpty As java.util.concurrent.locks.Condition = takeLock.newCondition()

		''' <summary>
		''' Lock held by put, offer, etc </summary>
		Private ReadOnly putLock As New java.util.concurrent.locks.ReentrantLock

		''' <summary>
		''' Wait queue for waiting puts </summary>
		Private ReadOnly notFull As java.util.concurrent.locks.Condition = putLock.newCondition()

		''' <summary>
		''' Signals a waiting take. Called only from put/offer (which do not
		''' otherwise ordinarily lock takeLock.)
		''' </summary>
		Private Sub signalNotEmpty()
			Dim takeLock As java.util.concurrent.locks.ReentrantLock = Me.takeLock
			takeLock.lock()
			Try
				notEmpty.signal()
			Finally
				takeLock.unlock()
			End Try
		End Sub

		''' <summary>
		''' Signals a waiting put. Called only from take/poll.
		''' </summary>
		Private Sub signalNotFull()
			Dim putLock As java.util.concurrent.locks.ReentrantLock = Me.putLock
			putLock.lock()
			Try
				notFull.signal()
			Finally
				putLock.unlock()
			End Try
		End Sub

		''' <summary>
		''' Links node at end of queue.
		''' </summary>
		''' <param name="node"> the node </param>
		Private Sub enqueue(ByVal node As Node(Of E))
			' assert putLock.isHeldByCurrentThread();
			' assert last.next == null;
				last.next = node
				last = last.next
		End Sub

		''' <summary>
		''' Removes a node from head of queue.
		''' </summary>
		''' <returns> the node </returns>
		Private Function dequeue() As E
			' assert takeLock.isHeldByCurrentThread();
			' assert head.item == null;
			Dim h As Node(Of E) = head
			Dim first As Node(Of E) = h.next
			h.next = h ' help GC
			head = first
			Dim x As E = first.item
			first.item = Nothing
			Return x
		End Function

		''' <summary>
		''' Locks to prevent both puts and takes.
		''' </summary>
		Friend Overridable Sub fullyLock()
			putLock.lock()
			takeLock.lock()
		End Sub

		''' <summary>
		''' Unlocks to allow both puts and takes.
		''' </summary>
		Friend Overridable Sub fullyUnlock()
			takeLock.unlock()
			putLock.unlock()
		End Sub

	'     /**
	'      * Tells whether both locks are held by current thread.
	'      */
	'     boolean isFullyLocked() {
	'         return (putLock.isHeldByCurrentThread() &&
	'                 takeLock.isHeldByCurrentThread());
	'     }

		''' <summary>
		''' Creates a {@code LinkedBlockingQueue} with a capacity of
		''' <seealso cref="Integer#MAX_VALUE"/>.
		''' </summary>
		Public Sub New()
			Me.New(Integer.MaxValue)
		End Sub

		''' <summary>
		''' Creates a {@code LinkedBlockingQueue} with the given (fixed) capacity.
		''' </summary>
		''' <param name="capacity"> the capacity of this queue </param>
		''' <exception cref="IllegalArgumentException"> if {@code capacity} is not greater
		'''         than zero </exception>
		Public Sub New(ByVal capacity As Integer)
			If capacity <= 0 Then Throw New IllegalArgumentException
			Me.capacity = capacity
				head = New Node(Of E)(Nothing)
				last = head
		End Sub

		''' <summary>
		''' Creates a {@code LinkedBlockingQueue} with a capacity of
		''' <seealso cref="Integer#MAX_VALUE"/>, initially containing the elements of the
		''' given collection,
		''' added in traversal order of the collection's iterator.
		''' </summary>
		''' <param name="c"> the collection of elements to initially contain </param>
		''' <exception cref="NullPointerException"> if the specified collection or any
		'''         of its elements are null </exception>
		Public Sub New(Of T1 As E)(ByVal c As ICollection(Of T1))
			Me.New(Integer.MaxValue)
			Dim putLock As java.util.concurrent.locks.ReentrantLock = Me.putLock
			putLock.lock() ' Never contended, but necessary for visibility
			Try
				Dim n As Integer = 0
				For Each e As E In c
					If e Is Nothing Then Throw New NullPointerException
					If n = capacity Then Throw New IllegalStateException("Queue full")
					enqueue(New Node(Of E)(e))
					n += 1
				Next e
				count.set(n)
			Finally
				putLock.unlock()
			End Try
		End Sub

		' this doc comment is overridden to remove the reference to collections
		' greater in size than  [Integer].MAX_VALUE
		''' <summary>
		''' Returns the number of elements in this queue.
		''' </summary>
		''' <returns> the number of elements in this queue </returns>
		Public Overridable Function size() As Integer
			Return count.get()
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
			Return capacity - count.get()
		End Function

		''' <summary>
		''' Inserts the specified element at the tail of this queue, waiting if
		''' necessary for space to become available.
		''' </summary>
		''' <exception cref="InterruptedException"> {@inheritDoc} </exception>
		''' <exception cref="NullPointerException"> {@inheritDoc} </exception>
		Public Overridable Sub put(ByVal e As E)
			If e Is Nothing Then Throw New NullPointerException
			' Note: convention in all put/take/etc is to preset local var
			' holding count negative to indicate failure unless set.
			Dim c As Integer = -1
			Dim node As New Node(Of E)(e)
			Dim putLock As java.util.concurrent.locks.ReentrantLock = Me.putLock
			Dim count As java.util.concurrent.atomic.AtomicInteger = Me.count
			putLock.lockInterruptibly()
			Try
	'            
	'             * Note that count is used in wait guard even though it is
	'             * not protected by lock. This works because count can
	'             * only decrease at this point (all other puts are shut
	'             * out by lock), and we (or some other waiting put) are
	'             * signalled if it ever changes from capacity. Similarly
	'             * for all other uses of count in other wait guards.
	'             
				Do While count.get() = capacity
					notFull.await()
				Loop
				enqueue(node)
				c = count.andIncrement
				If c + 1 < capacity Then notFull.signal()
			Finally
				putLock.unlock()
			End Try
			If c = 0 Then signalNotEmpty()
		End Sub

		''' <summary>
		''' Inserts the specified element at the tail of this queue, waiting if
		''' necessary up to the specified wait time for space to become available.
		''' </summary>
		''' <returns> {@code true} if successful, or {@code false} if
		'''         the specified waiting time elapses before space is available </returns>
		''' <exception cref="InterruptedException"> {@inheritDoc} </exception>
		''' <exception cref="NullPointerException"> {@inheritDoc} </exception>
		Public Overridable Function offer(ByVal e As E, ByVal timeout As Long, ByVal unit As TimeUnit) As Boolean

			If e Is Nothing Then Throw New NullPointerException
			Dim nanos As Long = unit.toNanos(timeout)
			Dim c As Integer = -1
			Dim putLock As java.util.concurrent.locks.ReentrantLock = Me.putLock
			Dim count As java.util.concurrent.atomic.AtomicInteger = Me.count
			putLock.lockInterruptibly()
			Try
				Do While count.get() = capacity
					If nanos <= 0 Then Return False
					nanos = notFull.awaitNanos(nanos)
				Loop
				enqueue(New Node(Of E)(e))
				c = count.andIncrement
				If c + 1 < capacity Then notFull.signal()
			Finally
				putLock.unlock()
			End Try
			If c = 0 Then signalNotEmpty()
			Return True
		End Function

		''' <summary>
		''' Inserts the specified element at the tail of this queue if it is
		''' possible to do so immediately without exceeding the queue's capacity,
		''' returning {@code true} upon success and {@code false} if this queue
		''' is full.
		''' When using a capacity-restricted queue, this method is generally
		''' preferable to method <seealso cref="BlockingQueue#add add"/>, which can fail to
		''' insert an element only by throwing an exception.
		''' </summary>
		''' <exception cref="NullPointerException"> if the specified element is null </exception>
		Public Overridable Function offer(ByVal e As E) As Boolean
			If e Is Nothing Then Throw New NullPointerException
			Dim count As java.util.concurrent.atomic.AtomicInteger = Me.count
			If count.get() = capacity Then Return False
			Dim c As Integer = -1
			Dim node As New Node(Of E)(e)
			Dim putLock As java.util.concurrent.locks.ReentrantLock = Me.putLock
			putLock.lock()
			Try
				If count.get() < capacity Then
					enqueue(node)
					c = count.andIncrement
					If c + 1 < capacity Then notFull.signal()
				End If
			Finally
				putLock.unlock()
			End Try
			If c = 0 Then signalNotEmpty()
			Return c >= 0
		End Function

		Public Overridable Function take() As E Implements BlockingQueue(Of E).take
			Dim x As E
			Dim c As Integer = -1
			Dim count As java.util.concurrent.atomic.AtomicInteger = Me.count
			Dim takeLock As java.util.concurrent.locks.ReentrantLock = Me.takeLock
			takeLock.lockInterruptibly()
			Try
				Do While count.get() = 0
					notEmpty.await()
				Loop
				x = dequeue()
				c = count.andDecrement
				If c > 1 Then notEmpty.signal()
			Finally
				takeLock.unlock()
			End Try
			If c = capacity Then signalNotFull()
			Return x
		End Function

		Public Overridable Function poll(ByVal timeout As Long, ByVal unit As TimeUnit) As E Implements BlockingQueue(Of E).poll
			Dim x As E = Nothing
			Dim c As Integer = -1
			Dim nanos As Long = unit.toNanos(timeout)
			Dim count As java.util.concurrent.atomic.AtomicInteger = Me.count
			Dim takeLock As java.util.concurrent.locks.ReentrantLock = Me.takeLock
			takeLock.lockInterruptibly()
			Try
				Do While count.get() = 0
					If nanos <= 0 Then Return Nothing
					nanos = notEmpty.awaitNanos(nanos)
				Loop
				x = dequeue()
				c = count.andDecrement
				If c > 1 Then notEmpty.signal()
			Finally
				takeLock.unlock()
			End Try
			If c = capacity Then signalNotFull()
			Return x
		End Function

		Public Overridable Function poll() As E
			Dim count As java.util.concurrent.atomic.AtomicInteger = Me.count
			If count.get() = 0 Then Return Nothing
			Dim x As E = Nothing
			Dim c As Integer = -1
			Dim takeLock As java.util.concurrent.locks.ReentrantLock = Me.takeLock
			takeLock.lock()
			Try
				If count.get() > 0 Then
					x = dequeue()
					c = count.andDecrement
					If c > 1 Then notEmpty.signal()
				End If
			Finally
				takeLock.unlock()
			End Try
			If c = capacity Then signalNotFull()
			Return x
		End Function

		Public Overridable Function peek() As E
			If count.get() = 0 Then Return Nothing
			Dim takeLock As java.util.concurrent.locks.ReentrantLock = Me.takeLock
			takeLock.lock()
			Try
				Dim first As Node(Of E) = head.next
				If first Is Nothing Then
					Return Nothing
				Else
					Return first.item
				End If
			Finally
				takeLock.unlock()
			End Try
		End Function

		''' <summary>
		''' Unlinks interior Node p with predecessor trail.
		''' </summary>
		Friend Overridable Sub unlink(ByVal p As Node(Of E), ByVal trail As Node(Of E))
			' assert isFullyLocked();
			' p.next is not changed, to allow iterators that are
			' traversing p to maintain their weak-consistency guarantee.
			p.item = Nothing
			trail.next = p.next
			If last Is p Then last = trail
			If count.andDecrement = capacity Then notFull.signal()
		End Sub

		''' <summary>
		''' Removes a single instance of the specified element from this queue,
		''' if it is present.  More formally, removes an element {@code e} such
		''' that {@code o.equals(e)}, if this queue contains one or more such
		''' elements.
		''' Returns {@code true} if this queue contained the specified element
		''' (or equivalently, if this queue changed as a result of the call).
		''' </summary>
		''' <param name="o"> element to be removed from this queue, if present </param>
		''' <returns> {@code true} if this queue changed as a result of the call </returns>
		Public Overridable Function remove(ByVal o As Object) As Boolean Implements BlockingQueue(Of E).remove
			If o Is Nothing Then Return False
			fullyLock()
			Try
				Dim trail As Node(Of E) = head
				Dim p As Node(Of E) = trail.next
				Do While p IsNot Nothing
					If o.Equals(p.item) Then
						unlink(p, trail)
						Return True
					End If
					trail = p
					p = p.next
				Loop
				Return False
			Finally
				fullyUnlock()
			End Try
		End Function

		''' <summary>
		''' Returns {@code true} if this queue contains the specified element.
		''' More formally, returns {@code true} if and only if this queue contains
		''' at least one element {@code e} such that {@code o.equals(e)}.
		''' </summary>
		''' <param name="o"> object to be checked for containment in this queue </param>
		''' <returns> {@code true} if this queue contains the specified element </returns>
		Public Overridable Function contains(ByVal o As Object) As Boolean Implements BlockingQueue(Of E).contains
			If o Is Nothing Then Return False
			fullyLock()
			Try
				Dim p As Node(Of E) = head.next
				Do While p IsNot Nothing
					If o.Equals(p.item) Then Return True
					p = p.next
				Loop
				Return False
			Finally
				fullyUnlock()
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
			fullyLock()
			Try
				Dim size As Integer = count.get()
				Dim a As Object() = New Object(size - 1){}
				Dim k As Integer = 0
				Dim p As Node(Of E) = head.next
				Do While p IsNot Nothing
					a(k) = p.item
					k += 1
					p = p.next
				Loop
				Return a
			Finally
				fullyUnlock()
			End Try
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
		Public Overridable Function toArray(Of T)(ByVal a As T()) As T()
			fullyLock()
			Try
				Dim size As Integer = count.get()
				If a.Length < size Then a = CType(java.lang.reflect.Array.newInstance(a.GetType().GetElementType(), size), T())

				Dim k As Integer = 0
				Dim p As Node(Of E) = head.next
				Do While p IsNot Nothing
					a(k) = CType(p.item, T)
					k += 1
					p = p.next
				Loop
				If a.Length > k Then a(k) = Nothing
				Return a
			Finally
				fullyUnlock()
			End Try
		End Function

		Public Overrides Function ToString() As String
			fullyLock()
			Try
				Dim p As Node(Of E) = head.next
				If p Is Nothing Then Return "[]"

				Dim sb As New StringBuilder
				sb.append("["c)
				Do
					Dim e As E = p.item
					sb.append(If(e Is Me, "(this Collection)", e))
					p = p.next
					If p Is Nothing Then Return sb.append("]"c).ToString()
					sb.append(","c).append(" "c)
				Loop
			Finally
				fullyUnlock()
			End Try
		End Function

		''' <summary>
		''' Atomically removes all of the elements from this queue.
		''' The queue will be empty after this call returns.
		''' </summary>
		Public Overridable Sub clear()
			fullyLock()
			Try
				Dim p As Node(Of E)
				h = head
'JAVA TO VB CONVERTER TODO TASK: Assignments within expressions are not supported in VB
				Do While (p = h.next) IsNot Nothing
					h.next = h
					p.item = Nothing
					h = p
				Loop
				head = last
				' assert head.item == null && head.next == null;
				If count.getAndSet(0) = capacity Then notFull.signal()
			Finally
				fullyUnlock()
			End Try
		End Sub

		''' <exception cref="UnsupportedOperationException"> {@inheritDoc} </exception>
		''' <exception cref="ClassCastException">            {@inheritDoc} </exception>
		''' <exception cref="NullPointerException">          {@inheritDoc} </exception>
		''' <exception cref="IllegalArgumentException">      {@inheritDoc} </exception>
'JAVA TO VB CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
		Public Overridable Function drainTo(Of T1)(ByVal c As ICollection(Of T1)) As Integer Implements BlockingQueue(Of E).drainTo
			Return drainTo(c, Integer.MaxValue)
		End Function

		''' <exception cref="UnsupportedOperationException"> {@inheritDoc} </exception>
		''' <exception cref="ClassCastException">            {@inheritDoc} </exception>
		''' <exception cref="NullPointerException">          {@inheritDoc} </exception>
		''' <exception cref="IllegalArgumentException">      {@inheritDoc} </exception>
'JAVA TO VB CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
		Public Overridable Function drainTo(Of T1)(ByVal c As ICollection(Of T1), ByVal maxElements As Integer) As Integer Implements BlockingQueue(Of E).drainTo
			If c Is Nothing Then Throw New NullPointerException
			If c Is Me Then Throw New IllegalArgumentException
			If maxElements <= 0 Then Return 0
			Dim signalNotFull As Boolean = False
			Dim takeLock As java.util.concurrent.locks.ReentrantLock = Me.takeLock
			takeLock.lock()
			Try
				Dim n As Integer = Math.Min(maxElements, count.get())
				' count.get provides visibility to first n Nodes
				Dim h As Node(Of E) = head
				Dim i As Integer = 0
				Try
					Do While i < n
						Dim p As Node(Of E) = h.next
						c.Add(p.item)
						p.item = Nothing
						h.next = h
						h = p
						i += 1
					Loop
					Return n
				Finally
					' Restore invariants even if c.add() threw
					If i > 0 Then
						' assert h.item == null;
						head = h
						signalNotFull = (count.getAndAdd(-i) = capacity)
					End If
				End Try
			Finally
				takeLock.unlock()
				If signalNotFull Then signalNotFull()
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

		Private Class Itr
			Implements IEnumerator(Of E)

			Private ReadOnly outerInstance As LinkedBlockingQueue

	'        
	'         * Basic weakly-consistent iterator.  At all times hold the next
	'         * item to hand out so that if hasNext() reports true, we will
	'         * still have it to return even if lost race with a take etc.
	'         

			Private current As Node(Of E)
			Private lastRet As Node(Of E)
			Private currentElement As E

			Friend Sub New(ByVal outerInstance As LinkedBlockingQueue)
					Me.outerInstance = outerInstance
				outerInstance.fullyLock()
				Try
					current = outerInstance.head.next
					If current IsNot Nothing Then currentElement = current.item
				Finally
					outerInstance.fullyUnlock()
				End Try
			End Sub

			Public Overridable Function hasNext() As Boolean
				Return current IsNot Nothing
			End Function

			''' <summary>
			''' Returns the next live successor of p, or null if no such.
			''' 
			''' Unlike other traversal methods, iterators need to handle both:
			''' - dequeued nodes (p.next == p)
			''' - (possibly multiple) interior removed nodes (p.item == null)
			''' </summary>
			Private Function nextNode(ByVal p As Node(Of E)) As Node(Of E)
				Do
					Dim s As Node(Of E) = p.next
					If s Is p Then Return outerInstance.head.next
					If s Is Nothing OrElse s.item IsNot Nothing Then Return s
					p = s
				Loop
			End Function

			Public Overridable Function [next]() As E
				outerInstance.fullyLock()
				Try
					If current Is Nothing Then Throw New java.util.NoSuchElementException
					Dim x As E = currentElement
					lastRet = current
					current = nextNode(current)
					currentElement = If(current Is Nothing, Nothing, current.item)
					Return x
				Finally
					outerInstance.fullyUnlock()
				End Try
			End Function

			Public Overridable Sub remove()
				If lastRet Is Nothing Then Throw New IllegalStateException
				outerInstance.fullyLock()
				Try
					Dim node As Node(Of E) = lastRet
					lastRet = Nothing
					Dim trail As Node(Of E) = outerInstance.head
					Dim p As Node(Of E) = trail.next
					Do While p IsNot Nothing
						If p Is node Then
							outerInstance.unlink(p, trail)
							Exit Do
						End If
						trail = p
						p = p.next
					Loop
				Finally
					outerInstance.fullyUnlock()
				End Try
			End Sub
		End Class

		''' <summary>
		''' A customized variant of Spliterators.IteratorSpliterator </summary>
		Friend NotInheritable Class LBQSpliterator(Of E)
			Implements java.util.Spliterator(Of E)

			Friend Shared ReadOnly MAX_BATCH As Integer = 1 << 25 ' max batch array size;
			Friend ReadOnly queue As LinkedBlockingQueue(Of E)
			Friend current As Node(Of E) ' current node; null until initialized
			Friend batch As Integer ' batch size for splits
			Friend exhausted As Boolean ' true when no more nodes
			Friend est As Long ' size estimate
			Friend Sub New(ByVal queue As LinkedBlockingQueue(Of E))
				Me.queue = queue
				Me.est = queue.size()
			End Sub

			Public Function estimateSize() As Long
				Return est
			End Function

			Public Function trySplit() As java.util.Spliterator(Of E)
				Dim h As Node(Of E)
				Dim q As LinkedBlockingQueue(Of E) = Me.queue
				Dim b As Integer = batch
				Dim n As Integer = If(b <= 0, 1, If(b >= MAX_BATCH, MAX_BATCH, b + 1))
				h = current
				h = q.head.next
				If (Not exhausted) AndAlso (h IsNot Nothing OrElse h IsNot Nothing) AndAlso h.next IsNot Nothing Then
					Dim a As Object() = New Object(n - 1){}
					Dim i As Integer = 0
					Dim p As Node(Of E) = current
					q.fullyLock()
					Try
						p = q.head.next
						If p IsNot Nothing OrElse p IsNot Nothing Then
							Do
'JAVA TO VB CONVERTER TODO TASK: Assignments within expressions are not supported in VB
								If (a(i) = p.item) IsNot Nothing Then i += 1
								p = p.next
							Loop While p IsNot Nothing AndAlso i < n
						End If
					Finally
						q.fullyUnlock()
					End Try
					current = p
					If current Is Nothing Then
						est = 0L
						exhausted = True
					Else
						est -= i
						If est < 0L Then est = 0L
						End If
					If i > 0 Then
						batch = i
						Return java.util.Spliterators.spliterator(a, 0, i, java.util.Spliterator.ORDERED Or java.util.Spliterator.NONNULL Or java.util.Spliterator.CONCURRENT)
					End If
				End If
				Return Nothing
			End Function

'JAVA TO VB CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
			Public Sub forEachRemaining(Of T1)(ByVal action As java.util.function.Consumer(Of T1))
				If action Is Nothing Then Throw New NullPointerException
				Dim q As LinkedBlockingQueue(Of E) = Me.queue
				If Not exhausted Then
					exhausted = True
					Dim p As Node(Of E) = current
					Do
						Dim e As E = Nothing
						q.fullyLock()
						Try
							If p Is Nothing Then p = q.head.next
							Do While p IsNot Nothing
								e = p.item
								p = p.next
								If e IsNot Nothing Then Exit Do
							Loop
						Finally
							q.fullyUnlock()
						End Try
						If e IsNot Nothing Then action.accept(e)
					Loop While p IsNot Nothing
				End If
			End Sub

'JAVA TO VB CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
			Public Function tryAdvance(Of T1)(ByVal action As java.util.function.Consumer(Of T1)) As Boolean
				If action Is Nothing Then Throw New NullPointerException
				Dim q As LinkedBlockingQueue(Of E) = Me.queue
				If Not exhausted Then
					Dim e As E = Nothing
					q.fullyLock()
					Try
						If current Is Nothing Then current = q.head.next
						Do While current IsNot Nothing
							e = current.item
							current = current.next
							If e IsNot Nothing Then Exit Do
						Loop
					Finally
						q.fullyUnlock()
					End Try
					If current Is Nothing Then exhausted = True
					If e IsNot Nothing Then
						action.accept(e)
						Return True
					End If
				End If
				Return False
			End Function

			Public Function characteristics() As Integer
				Return java.util.Spliterator.ORDERED Or java.util.Spliterator.NONNULL Or java.util.Spliterator.CONCURRENT
			End Function
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
			Return New LBQSpliterator(Of E)(Me)
		End Function

		''' <summary>
		''' Saves this queue to a stream (that is, serializes it).
		''' </summary>
		''' <param name="s"> the stream </param>
		''' <exception cref="java.io.IOException"> if an I/O error occurs
		''' @serialData The capacity is emitted (int), followed by all of
		''' its elements (each an {@code Object}) in the proper order,
		''' followed by a null </exception>
		Private Sub writeObject(ByVal s As java.io.ObjectOutputStream)

			fullyLock()
			Try
				' Write out any hidden stuff, plus capacity
				s.defaultWriteObject()

				' Write out all elements in the proper order.
				Dim p As Node(Of E) = head.next
				Do While p IsNot Nothing
					s.writeObject(p.item)
					p = p.next
				Loop

				' Use trailing null as sentinel
				s.writeObject(Nothing)
			Finally
				fullyUnlock()
			End Try
		End Sub

		''' <summary>
		''' Reconstitutes this queue from a stream (that is, deserializes it). </summary>
		''' <param name="s"> the stream </param>
		''' <exception cref="ClassNotFoundException"> if the class of a serialized object
		'''         could not be found </exception>
		''' <exception cref="java.io.IOException"> if an I/O error occurs </exception>
		Private Sub readObject(ByVal s As java.io.ObjectInputStream)
			' Read in capacity, and any hidden stuff
			s.defaultReadObject()

			count.set(0)
				head = New Node(Of E)(Nothing)
				last = head

			' Read in all elements and place in queue
			Do
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
				Dim item As E = CType(s.readObject(), E)
				If item Is Nothing Then Exit Do
				add(item)
			Loop
		End Sub
	End Class

End Namespace