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
	''' An optionally-bounded <seealso cref="BlockingDeque blocking deque"/> based on
	''' linked nodes.
	''' 
	''' <p>The optional capacity bound constructor argument serves as a
	''' way to prevent excessive expansion. The capacity, if unspecified,
	''' is equal to <seealso cref="Integer#MAX_VALUE"/>.  Linked nodes are
	''' dynamically created upon each insertion unless this would bring the
	''' deque above capacity.
	''' 
	''' <p>Most operations run in constant time (ignoring time spent
	''' blocking).  Exceptions include <seealso cref="#remove(Object) remove"/>,
	''' <seealso cref="#removeFirstOccurrence removeFirstOccurrence"/>, {@link
	''' #removeLastOccurrence removeLastOccurrence}, {@link #contains
	''' contains}, <seealso cref="#iterator iterator.remove()"/>, and the bulk
	''' operations, all of which run in linear time.
	''' 
	''' <p>This class and its iterator implement all of the
	''' <em>optional</em> methods of the <seealso cref="Collection"/> and {@link
	''' Iterator} interfaces.
	''' 
	''' <p>This class is a member of the
	''' <a href="{@docRoot}/../technotes/guides/collections/index.html">
	''' Java Collections Framework</a>.
	''' 
	''' @since 1.6
	''' @author  Doug Lea </summary>
	''' @param <E> the type of elements held in this collection </param>
	<Serializable> _
	Public Class LinkedBlockingDeque(Of E)
		Inherits java.util.AbstractQueue(Of E)
		Implements BlockingDeque(Of E)

	'    
	'     * Implemented as a simple doubly-linked list protected by a
	'     * single lock and using conditions to manage blocking.
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
	'     * self-link implicitly means to jump to "first" (for next links)
	'     * or "last" (for prev links).
	'     

	'    
	'     * We have "diamond" multiple interface/abstract class inheritance
	'     * here, and that introduces ambiguities. Often we want the
	'     * BlockingDeque javadoc combined with the AbstractQueue
	'     * implementation, so a lot of method specs are duplicated here.
	'     

		Private Const serialVersionUID As Long = -387911632671998426L

		''' <summary>
		''' Doubly-linked list node class </summary>
		Friend NotInheritable Class Node(Of E)
			''' <summary>
			''' The item, or null if this node has been removed.
			''' </summary>
			Friend item As E

			''' <summary>
			''' One of:
			''' - the real predecessor Node
			''' - this Node, meaning the predecessor is tail
			''' - null, meaning there is no predecessor
			''' </summary>
			Friend prev As Node(Of E)

			''' <summary>
			''' One of:
			''' - the real successor Node
			''' - this Node, meaning the successor is head
			''' - null, meaning there is no successor
			''' </summary>
			Friend [next] As Node(Of E)

			Friend Sub New(ByVal x As E)
				item = x
			End Sub
		End Class

		''' <summary>
		''' Pointer to first node.
		''' Invariant: (first == null && last == null) ||
		'''            (first.prev == null && first.item != null)
		''' </summary>
		<NonSerialized> _
		Friend first As Node(Of E)

		''' <summary>
		''' Pointer to last node.
		''' Invariant: (first == null && last == null) ||
		'''            (last.next == null && last.item != null)
		''' </summary>
		<NonSerialized> _
		Friend last As Node(Of E)

		''' <summary>
		''' Number of items in the deque </summary>
		<NonSerialized> _
		Private count As Integer

		''' <summary>
		''' Maximum number of items in the deque </summary>
		Private ReadOnly capacity As Integer

		''' <summary>
		''' Main lock guarding all access </summary>
		Friend ReadOnly lock As New java.util.concurrent.locks.ReentrantLock

		''' <summary>
		''' Condition for waiting takes </summary>
		Private ReadOnly notEmpty As java.util.concurrent.locks.Condition = lock.newCondition()

		''' <summary>
		''' Condition for waiting puts </summary>
		Private ReadOnly notFull As java.util.concurrent.locks.Condition = lock.newCondition()

		''' <summary>
		''' Creates a {@code LinkedBlockingDeque} with a capacity of
		''' <seealso cref="Integer#MAX_VALUE"/>.
		''' </summary>
		Public Sub New()
			Me.New(Integer.MaxValue)
		End Sub

		''' <summary>
		''' Creates a {@code LinkedBlockingDeque} with the given (fixed) capacity.
		''' </summary>
		''' <param name="capacity"> the capacity of this deque </param>
		''' <exception cref="IllegalArgumentException"> if {@code capacity} is less than 1 </exception>
		Public Sub New(ByVal capacity As Integer)
			If capacity <= 0 Then Throw New IllegalArgumentException
			Me.capacity = capacity
		End Sub

		''' <summary>
		''' Creates a {@code LinkedBlockingDeque} with a capacity of
		''' <seealso cref="Integer#MAX_VALUE"/>, initially containing the elements of
		''' the given collection, added in traversal order of the
		''' collection's iterator.
		''' </summary>
		''' <param name="c"> the collection of elements to initially contain </param>
		''' <exception cref="NullPointerException"> if the specified collection or any
		'''         of its elements are null </exception>
		Public Sub New(Of T1 As E)(ByVal c As ICollection(Of T1))
			Me.New(Integer.MaxValue)
			Dim lock As java.util.concurrent.locks.ReentrantLock = Me.lock
			lock.lock() ' Never contended, but necessary for visibility
			Try
				For Each e As E In c
					If e Is Nothing Then Throw New NullPointerException
					If Not linkLast(New Node(Of E)(e)) Then Throw New IllegalStateException("Deque full")
				Next e
			Finally
				lock.unlock()
			End Try
		End Sub


		' Basic linking and unlinking operations, called only while holding lock

		''' <summary>
		''' Links node as first element, or returns false if full.
		''' </summary>
		Private Function linkFirst(ByVal node As Node(Of E)) As Boolean
			' assert lock.isHeldByCurrentThread();
			If count >= capacity Then Return False
			Dim f As Node(Of E) = first
			node.next = f
			first = node
			If last Is Nothing Then
				last = node
			Else
				f.prev = node
			End If
			count += 1
			notEmpty.signal()
			Return True
		End Function

		''' <summary>
		''' Links node as last element, or returns false if full.
		''' </summary>
		Private Function linkLast(ByVal node As Node(Of E)) As Boolean
			' assert lock.isHeldByCurrentThread();
			If count >= capacity Then Return False
			Dim l As Node(Of E) = last
			node.prev = l
			last = node
			If first Is Nothing Then
				first = node
			Else
				l.next = node
			End If
			count += 1
			notEmpty.signal()
			Return True
		End Function

		''' <summary>
		''' Removes and returns first element, or null if empty.
		''' </summary>
		Private Function unlinkFirst() As E
			' assert lock.isHeldByCurrentThread();
			Dim f As Node(Of E) = first
			If f Is Nothing Then Return Nothing
			Dim n As Node(Of E) = f.next
			Dim item As E = f.item
			f.item = Nothing
			f.next = f ' help GC
			first = n
			If n Is Nothing Then
				last = Nothing
			Else
				n.prev = Nothing
			End If
			count -= 1
			notFull.signal()
			Return item
		End Function

		''' <summary>
		''' Removes and returns last element, or null if empty.
		''' </summary>
		Private Function unlinkLast() As E
			' assert lock.isHeldByCurrentThread();
			Dim l As Node(Of E) = last
			If l Is Nothing Then Return Nothing
			Dim p As Node(Of E) = l.prev
			Dim item As E = l.item
			l.item = Nothing
			l.prev = l ' help GC
			last = p
			If p Is Nothing Then
				first = Nothing
			Else
				p.next = Nothing
			End If
			count -= 1
			notFull.signal()
			Return item
		End Function

		''' <summary>
		''' Unlinks x.
		''' </summary>
		Friend Overridable Sub unlink(ByVal x As Node(Of E))
			' assert lock.isHeldByCurrentThread();
			Dim p As Node(Of E) = x.prev
			Dim n As Node(Of E) = x.next
			If p Is Nothing Then
				unlinkFirst()
			ElseIf n Is Nothing Then
				unlinkLast()
			Else
				p.next = n
				n.prev = p
				x.item = Nothing
				' Don't mess with x's links.  They may still be in use by
				' an iterator.
				count -= 1
				notFull.signal()
			End If
		End Sub

		' BlockingDeque methods

		''' <exception cref="IllegalStateException"> if this deque is full </exception>
		''' <exception cref="NullPointerException"> {@inheritDoc} </exception>
		Public Overridable Sub addFirst(ByVal e As E)
			If Not offerFirst(e) Then Throw New IllegalStateException("Deque full")
		End Sub

		''' <exception cref="IllegalStateException"> if this deque is full </exception>
		''' <exception cref="NullPointerException">  {@inheritDoc} </exception>
		Public Overridable Sub addLast(ByVal e As E)
			If Not offerLast(e) Then Throw New IllegalStateException("Deque full")
		End Sub

		''' <exception cref="NullPointerException"> {@inheritDoc} </exception>
		Public Overridable Function offerFirst(ByVal e As E) As Boolean
			If e Is Nothing Then Throw New NullPointerException
			Dim node As New Node(Of E)(e)
			Dim lock As java.util.concurrent.locks.ReentrantLock = Me.lock
			lock.lock()
			Try
				Return linkFirst(node)
			Finally
				lock.unlock()
			End Try
		End Function

		''' <exception cref="NullPointerException"> {@inheritDoc} </exception>
		Public Overridable Function offerLast(ByVal e As E) As Boolean
			If e Is Nothing Then Throw New NullPointerException
			Dim node As New Node(Of E)(e)
			Dim lock As java.util.concurrent.locks.ReentrantLock = Me.lock
			lock.lock()
			Try
				Return linkLast(node)
			Finally
				lock.unlock()
			End Try
		End Function

		''' <exception cref="NullPointerException"> {@inheritDoc} </exception>
		''' <exception cref="InterruptedException"> {@inheritDoc} </exception>
		Public Overridable Sub putFirst(ByVal e As E)
			If e Is Nothing Then Throw New NullPointerException
			Dim node As New Node(Of E)(e)
			Dim lock As java.util.concurrent.locks.ReentrantLock = Me.lock
			lock.lock()
			Try
				Do While Not linkFirst(node)
					notFull.await()
				Loop
			Finally
				lock.unlock()
			End Try
		End Sub

		''' <exception cref="NullPointerException"> {@inheritDoc} </exception>
		''' <exception cref="InterruptedException"> {@inheritDoc} </exception>
		Public Overridable Sub putLast(ByVal e As E)
			If e Is Nothing Then Throw New NullPointerException
			Dim node As New Node(Of E)(e)
			Dim lock As java.util.concurrent.locks.ReentrantLock = Me.lock
			lock.lock()
			Try
				Do While Not linkLast(node)
					notFull.await()
				Loop
			Finally
				lock.unlock()
			End Try
		End Sub

		''' <exception cref="NullPointerException"> {@inheritDoc} </exception>
		''' <exception cref="InterruptedException"> {@inheritDoc} </exception>
		Public Overridable Function offerFirst(ByVal e As E, ByVal timeout As Long, ByVal unit As TimeUnit) As Boolean
			If e Is Nothing Then Throw New NullPointerException
			Dim node As New Node(Of E)(e)
			Dim nanos As Long = unit.toNanos(timeout)
			Dim lock As java.util.concurrent.locks.ReentrantLock = Me.lock
			lock.lockInterruptibly()
			Try
				Do While Not linkFirst(node)
					If nanos <= 0 Then Return False
					nanos = notFull.awaitNanos(nanos)
				Loop
				Return True
			Finally
				lock.unlock()
			End Try
		End Function

		''' <exception cref="NullPointerException"> {@inheritDoc} </exception>
		''' <exception cref="InterruptedException"> {@inheritDoc} </exception>
		Public Overridable Function offerLast(ByVal e As E, ByVal timeout As Long, ByVal unit As TimeUnit) As Boolean
			If e Is Nothing Then Throw New NullPointerException
			Dim node As New Node(Of E)(e)
			Dim nanos As Long = unit.toNanos(timeout)
			Dim lock As java.util.concurrent.locks.ReentrantLock = Me.lock
			lock.lockInterruptibly()
			Try
				Do While Not linkLast(node)
					If nanos <= 0 Then Return False
					nanos = notFull.awaitNanos(nanos)
				Loop
				Return True
			Finally
				lock.unlock()
			End Try
		End Function

		''' <exception cref="NoSuchElementException"> {@inheritDoc} </exception>
		Public Overridable Function removeFirst() As E
			Dim x As E = pollFirst()
			If x Is Nothing Then Throw New java.util.NoSuchElementException
			Return x
		End Function

		''' <exception cref="NoSuchElementException"> {@inheritDoc} </exception>
		Public Overridable Function removeLast() As E
			Dim x As E = pollLast()
			If x Is Nothing Then Throw New java.util.NoSuchElementException
			Return x
		End Function

		Public Overridable Function pollFirst() As E
			Dim lock As java.util.concurrent.locks.ReentrantLock = Me.lock
			lock.lock()
			Try
				Return unlinkFirst()
			Finally
				lock.unlock()
			End Try
		End Function

		Public Overridable Function pollLast() As E
			Dim lock As java.util.concurrent.locks.ReentrantLock = Me.lock
			lock.lock()
			Try
				Return unlinkLast()
			Finally
				lock.unlock()
			End Try
		End Function

		Public Overridable Function takeFirst() As E Implements BlockingDeque(Of E).takeFirst
			Dim lock As java.util.concurrent.locks.ReentrantLock = Me.lock
			lock.lock()
			Try
				Dim x As E
				x = unlinkFirst()
				Do While x Is Nothing
					notEmpty.await()
					x = unlinkFirst()
				Loop
				Return x
			Finally
				lock.unlock()
			End Try
		End Function

		Public Overridable Function takeLast() As E Implements BlockingDeque(Of E).takeLast
			Dim lock As java.util.concurrent.locks.ReentrantLock = Me.lock
			lock.lock()
			Try
				Dim x As E
				x = unlinkLast()
				Do While x Is Nothing
					notEmpty.await()
					x = unlinkLast()
				Loop
				Return x
			Finally
				lock.unlock()
			End Try
		End Function

		Public Overridable Function pollFirst(ByVal timeout As Long, ByVal unit As TimeUnit) As E Implements BlockingDeque(Of E).pollFirst
			Dim nanos As Long = unit.toNanos(timeout)
			Dim lock As java.util.concurrent.locks.ReentrantLock = Me.lock
			lock.lockInterruptibly()
			Try
				Dim x As E
				x = unlinkFirst()
				Do While x Is Nothing
					If nanos <= 0 Then Return Nothing
					nanos = notEmpty.awaitNanos(nanos)
					x = unlinkFirst()
				Loop
				Return x
			Finally
				lock.unlock()
			End Try
		End Function

		Public Overridable Function pollLast(ByVal timeout As Long, ByVal unit As TimeUnit) As E Implements BlockingDeque(Of E).pollLast
			Dim nanos As Long = unit.toNanos(timeout)
			Dim lock As java.util.concurrent.locks.ReentrantLock = Me.lock
			lock.lockInterruptibly()
			Try
				Dim x As E
				x = unlinkLast()
				Do While x Is Nothing
					If nanos <= 0 Then Return Nothing
					nanos = notEmpty.awaitNanos(nanos)
					x = unlinkLast()
				Loop
				Return x
			Finally
				lock.unlock()
			End Try
		End Function

		''' <exception cref="NoSuchElementException"> {@inheritDoc} </exception>
		Public Overridable Property first As E
			Get
				Dim x As E = peekFirst()
				If x Is Nothing Then Throw New java.util.NoSuchElementException
				Return x
			End Get
		End Property

		''' <exception cref="NoSuchElementException"> {@inheritDoc} </exception>
		Public Overridable Property last As E
			Get
				Dim x As E = peekLast()
				If x Is Nothing Then Throw New java.util.NoSuchElementException
				Return x
			End Get
		End Property

		Public Overridable Function peekFirst() As E
			Dim lock As java.util.concurrent.locks.ReentrantLock = Me.lock
			lock.lock()
			Try
				Return If(first Is Nothing, Nothing, first.item)
			Finally
				lock.unlock()
			End Try
		End Function

		Public Overridable Function peekLast() As E
			Dim lock As java.util.concurrent.locks.ReentrantLock = Me.lock
			lock.lock()
			Try
				Return If(last Is Nothing, Nothing, last.item)
			Finally
				lock.unlock()
			End Try
		End Function

		Public Overridable Function removeFirstOccurrence(ByVal o As Object) As Boolean Implements BlockingDeque(Of E).removeFirstOccurrence
			If o Is Nothing Then Return False
			Dim lock As java.util.concurrent.locks.ReentrantLock = Me.lock
			lock.lock()
			Try
				Dim p As Node(Of E) = first
				Do While p IsNot Nothing
					If o.Equals(p.item) Then
						unlink(p)
						Return True
					End If
					p = p.next
				Loop
				Return False
			Finally
				lock.unlock()
			End Try
		End Function

		Public Overridable Function removeLastOccurrence(ByVal o As Object) As Boolean Implements BlockingDeque(Of E).removeLastOccurrence
			If o Is Nothing Then Return False
			Dim lock As java.util.concurrent.locks.ReentrantLock = Me.lock
			lock.lock()
			Try
				Dim p As Node(Of E) = last
				Do While p IsNot Nothing
					If o.Equals(p.item) Then
						unlink(p)
						Return True
					End If
					p = p.prev
				Loop
				Return False
			Finally
				lock.unlock()
			End Try
		End Function

		' BlockingQueue methods

		''' <summary>
		''' Inserts the specified element at the end of this deque unless it would
		''' violate capacity restrictions.  When using a capacity-restricted deque,
		''' it is generally preferable to use method <seealso cref="#offer(Object) offer"/>.
		''' 
		''' <p>This method is equivalent to <seealso cref="#addLast"/>.
		''' </summary>
		''' <exception cref="IllegalStateException"> if this deque is full </exception>
		''' <exception cref="NullPointerException"> if the specified element is null </exception>
		Public Overridable Function add(ByVal e As E) As Boolean
			addLast(e)
			Return True
		End Function

		''' <exception cref="NullPointerException"> if the specified element is null </exception>
		Public Overridable Function offer(ByVal e As E) As Boolean
			Return offerLast(e)
		End Function

		''' <exception cref="NullPointerException"> {@inheritDoc} </exception>
		''' <exception cref="InterruptedException"> {@inheritDoc} </exception>
		Public Overridable Sub put(ByVal e As E)
			putLast(e)
		End Sub

		''' <exception cref="NullPointerException"> {@inheritDoc} </exception>
		''' <exception cref="InterruptedException"> {@inheritDoc} </exception>
		Public Overridable Function offer(ByVal e As E, ByVal timeout As Long, ByVal unit As TimeUnit) As Boolean
			Return offerLast(e, timeout, unit)
		End Function

		''' <summary>
		''' Retrieves and removes the head of the queue represented by this deque.
		''' This method differs from <seealso cref="#poll poll"/> only in that it throws an
		''' exception if this deque is empty.
		''' 
		''' <p>This method is equivalent to <seealso cref="#removeFirst() removeFirst"/>.
		''' </summary>
		''' <returns> the head of the queue represented by this deque </returns>
		''' <exception cref="NoSuchElementException"> if this deque is empty </exception>
		Public Overridable Function remove() As E Implements BlockingDeque(Of E).remove
			Return removeFirst()
		End Function

		Public Overridable Function poll() As E Implements BlockingDeque(Of E).poll
			Return pollFirst()
		End Function

		Public Overridable Function take() As E Implements BlockingDeque(Of E).take
			Return takeFirst()
		End Function

		Public Overridable Function poll(ByVal timeout As Long, ByVal unit As TimeUnit) As E Implements BlockingDeque(Of E).poll
			Return pollFirst(timeout, unit)
		End Function

		''' <summary>
		''' Retrieves, but does not remove, the head of the queue represented by
		''' this deque.  This method differs from <seealso cref="#peek peek"/> only in that
		''' it throws an exception if this deque is empty.
		''' 
		''' <p>This method is equivalent to <seealso cref="#getFirst() getFirst"/>.
		''' </summary>
		''' <returns> the head of the queue represented by this deque </returns>
		''' <exception cref="NoSuchElementException"> if this deque is empty </exception>
		Public Overridable Function element() As E Implements BlockingDeque(Of E).element
			Return first
		End Function

		Public Overridable Function peek() As E Implements BlockingDeque(Of E).peek
			Return peekFirst()
		End Function

		''' <summary>
		''' Returns the number of additional elements that this deque can ideally
		''' (in the absence of memory or resource constraints) accept without
		''' blocking. This is always equal to the initial capacity of this deque
		''' less the current {@code size} of this deque.
		''' 
		''' <p>Note that you <em>cannot</em> always tell if an attempt to insert
		''' an element will succeed by inspecting {@code remainingCapacity}
		''' because it may be the case that another thread is about to
		''' insert or remove an element.
		''' </summary>
		Public Overridable Function remainingCapacity() As Integer
			Dim lock As java.util.concurrent.locks.ReentrantLock = Me.lock
			lock.lock()
			Try
				Return capacity - count
			Finally
				lock.unlock()
			End Try
		End Function

		''' <exception cref="UnsupportedOperationException"> {@inheritDoc} </exception>
		''' <exception cref="ClassCastException">            {@inheritDoc} </exception>
		''' <exception cref="NullPointerException">          {@inheritDoc} </exception>
		''' <exception cref="IllegalArgumentException">      {@inheritDoc} </exception>
'JAVA TO VB CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
		Public Overridable Function drainTo(Of T1)(ByVal c As ICollection(Of T1)) As Integer
			Return drainTo(c, Integer.MaxValue)
		End Function

		''' <exception cref="UnsupportedOperationException"> {@inheritDoc} </exception>
		''' <exception cref="ClassCastException">            {@inheritDoc} </exception>
		''' <exception cref="NullPointerException">          {@inheritDoc} </exception>
		''' <exception cref="IllegalArgumentException">      {@inheritDoc} </exception>
'JAVA TO VB CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
		Public Overridable Function drainTo(Of T1)(ByVal c As ICollection(Of T1), ByVal maxElements As Integer) As Integer
			If c Is Nothing Then Throw New NullPointerException
			If c Is Me Then Throw New IllegalArgumentException
			If maxElements <= 0 Then Return 0
			Dim lock As java.util.concurrent.locks.ReentrantLock = Me.lock
			lock.lock()
			Try
				Dim n As Integer = Math.Min(maxElements, count)
				For i As Integer = 0 To n - 1
					c.Add(first.item) ' In this order, in case add() throws.
					unlinkFirst()
				Next i
				Return n
			Finally
				lock.unlock()
			End Try
		End Function

		' Stack methods

		''' <exception cref="IllegalStateException"> if this deque is full </exception>
		''' <exception cref="NullPointerException"> {@inheritDoc} </exception>
		Public Overridable Sub push(ByVal e As E)
			addFirst(e)
		End Sub

		''' <exception cref="NoSuchElementException"> {@inheritDoc} </exception>
		Public Overridable Function pop() As E
			Return removeFirst()
		End Function

		' Collection methods

		''' <summary>
		''' Removes the first occurrence of the specified element from this deque.
		''' If the deque does not contain the element, it is unchanged.
		''' More formally, removes the first element {@code e} such that
		''' {@code o.equals(e)} (if such an element exists).
		''' Returns {@code true} if this deque contained the specified element
		''' (or equivalently, if this deque changed as a result of the call).
		''' 
		''' <p>This method is equivalent to
		''' <seealso cref="#removeFirstOccurrence(Object) removeFirstOccurrence"/>.
		''' </summary>
		''' <param name="o"> element to be removed from this deque, if present </param>
		''' <returns> {@code true} if this deque changed as a result of the call </returns>
		Public Overridable Function remove(ByVal o As Object) As Boolean Implements BlockingDeque(Of E).remove
			Return removeFirstOccurrence(o)
		End Function

		''' <summary>
		''' Returns the number of elements in this deque.
		''' </summary>
		''' <returns> the number of elements in this deque </returns>
		Public Overridable Function size() As Integer Implements BlockingDeque(Of E).size
			Dim lock As java.util.concurrent.locks.ReentrantLock = Me.lock
			lock.lock()
			Try
				Return count
			Finally
				lock.unlock()
			End Try
		End Function

		''' <summary>
		''' Returns {@code true} if this deque contains the specified element.
		''' More formally, returns {@code true} if and only if this deque contains
		''' at least one element {@code e} such that {@code o.equals(e)}.
		''' </summary>
		''' <param name="o"> object to be checked for containment in this deque </param>
		''' <returns> {@code true} if this deque contains the specified element </returns>
		Public Overridable Function contains(ByVal o As Object) As Boolean Implements BlockingDeque(Of E).contains
			If o Is Nothing Then Return False
			Dim lock As java.util.concurrent.locks.ReentrantLock = Me.lock
			lock.lock()
			Try
				Dim p As Node(Of E) = first
				Do While p IsNot Nothing
					If o.Equals(p.item) Then Return True
					p = p.next
				Loop
				Return False
			Finally
				lock.unlock()
			End Try
		End Function

	'    
	'     * TODO: Add support for more efficient bulk operations.
	'     *
	'     * We don't want to acquire the lock for every iteration, but we
	'     * also want other threads a chance to interact with the
	'     * collection, especially when count is close to capacity.
	'     

	'     /**
	'      * Adds all of the elements in the specified collection to this
	'      * queue.  Attempts to addAll of a queue to itself result in
	'      * {@code IllegalArgumentException}. Further, the behavior of
	'      * this operation is undefined if the specified collection is
	'      * modified while the operation is in progress.
	'      *
	'      * @param c collection containing elements to be added to this queue
	'      * @return {@code true} if this queue changed as a result of the call
	'      * @throws ClassCastException            {@inheritDoc}
	'      * @throws NullPointerException          {@inheritDoc}
	'      * @throws IllegalArgumentException      {@inheritDoc}
	'      * @throws IllegalStateException if this deque is full
	'      * @see #add(Object)
	'      */
	'     public boolean addAll(Collection<? extends E> c) {
	'         if (c == null)
	'             throw new NullPointerException();
	'         if (c == this)
	'             throw new IllegalArgumentException();
	'         final ReentrantLock lock = this.lock;
	'         lock.lock();
	'         try {
	'             boolean modified = false;
	'             for (E e : c)
	'                 if (linkLast(e))
	'                     modified = true;
	'             return modified;
	'         } finally {
	'             lock.unlock();
	'         }
	'     }

		''' <summary>
		''' Returns an array containing all of the elements in this deque, in
		''' proper sequence (from first to last element).
		''' 
		''' <p>The returned array will be "safe" in that no references to it are
		''' maintained by this deque.  (In other words, this method must allocate
		''' a new array).  The caller is thus free to modify the returned array.
		''' 
		''' <p>This method acts as bridge between array-based and collection-based
		''' APIs.
		''' </summary>
		''' <returns> an array containing all of the elements in this deque </returns>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
		Public Overridable Function toArray() As Object()
			Dim lock As java.util.concurrent.locks.ReentrantLock = Me.lock
			lock.lock()
			Try
				Dim a As Object() = New Object(count - 1){}
				Dim k As Integer = 0
				Dim p As Node(Of E) = first
				Do While p IsNot Nothing
					a(k) = p.item
					k += 1
					p = p.next
				Loop
				Return a
			Finally
				lock.unlock()
			End Try
		End Function

		''' <summary>
		''' Returns an array containing all of the elements in this deque, in
		''' proper sequence; the runtime type of the returned array is that of
		''' the specified array.  If the deque fits in the specified array, it
		''' is returned therein.  Otherwise, a new array is allocated with the
		''' runtime type of the specified array and the size of this deque.
		''' 
		''' <p>If this deque fits in the specified array with room to spare
		''' (i.e., the array has more elements than this deque), the element in
		''' the array immediately following the end of the deque is set to
		''' {@code null}.
		''' 
		''' <p>Like the <seealso cref="#toArray()"/> method, this method acts as bridge between
		''' array-based and collection-based APIs.  Further, this method allows
		''' precise control over the runtime type of the output array, and may,
		''' under certain circumstances, be used to save allocation costs.
		''' 
		''' <p>Suppose {@code x} is a deque known to contain only strings.
		''' The following code can be used to dump the deque into a newly
		''' allocated array of {@code String}:
		''' 
		'''  <pre> {@code String[] y = x.toArray(new String[0]);}</pre>
		''' 
		''' Note that {@code toArray(new Object[0])} is identical in function to
		''' {@code toArray()}.
		''' </summary>
		''' <param name="a"> the array into which the elements of the deque are to
		'''          be stored, if it is big enough; otherwise, a new array of the
		'''          same runtime type is allocated for this purpose </param>
		''' <returns> an array containing all of the elements in this deque </returns>
		''' <exception cref="ArrayStoreException"> if the runtime type of the specified array
		'''         is not a supertype of the runtime type of every element in
		'''         this deque </exception>
		''' <exception cref="NullPointerException"> if the specified array is null </exception>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
		Public Overridable Function toArray(Of T)(ByVal a As T()) As T()
			Dim lock As java.util.concurrent.locks.ReentrantLock = Me.lock
			lock.lock()
			Try
				If a.Length < count Then a = CType(java.lang.reflect.Array.newInstance(a.GetType().GetElementType(), count), T())

				Dim k As Integer = 0
				Dim p As Node(Of E) = first
				Do While p IsNot Nothing
					a(k) = CType(p.item, T)
					k += 1
					p = p.next
				Loop
				If a.Length > k Then a(k) = Nothing
				Return a
			Finally
				lock.unlock()
			End Try
		End Function

		Public Overrides Function ToString() As String
			Dim lock As java.util.concurrent.locks.ReentrantLock = Me.lock
			lock.lock()
			Try
				Dim p As Node(Of E) = first
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
				lock.unlock()
			End Try
		End Function

		''' <summary>
		''' Atomically removes all of the elements from this deque.
		''' The deque will be empty after this call returns.
		''' </summary>
		Public Overridable Sub clear()
			Dim lock As java.util.concurrent.locks.ReentrantLock = Me.lock
			lock.lock()
			Try
				Dim f As Node(Of E) = first
				Do While f IsNot Nothing
					f.item = Nothing
					Dim n As Node(Of E) = f.next
					f.prev = Nothing
					f.next = Nothing
					f = n
				Loop
					last = Nothing
					first = last
				count = 0
				notFull.signalAll()
			Finally
				lock.unlock()
			End Try
		End Sub

		''' <summary>
		''' Returns an iterator over the elements in this deque in proper sequence.
		''' The elements will be returned in order from first (head) to last (tail).
		''' 
		''' <p>The returned iterator is
		''' <a href="package-summary.html#Weakly"><i>weakly consistent</i></a>.
		''' </summary>
		''' <returns> an iterator over the elements in this deque in proper sequence </returns>
		Public Overridable Function [iterator]() As IEnumerator(Of E)
			Return New Itr(Me)
		End Function

		''' <summary>
		''' Returns an iterator over the elements in this deque in reverse
		''' sequential order.  The elements will be returned in order from
		''' last (tail) to first (head).
		''' 
		''' <p>The returned iterator is
		''' <a href="package-summary.html#Weakly"><i>weakly consistent</i></a>.
		''' </summary>
		''' <returns> an iterator over the elements in this deque in reverse order </returns>
		Public Overridable Function descendingIterator() As IEnumerator(Of E)
			Return New DescendingItr(Me)
		End Function

		''' <summary>
		''' Base class for Iterators for LinkedBlockingDeque
		''' </summary>
		Private MustInherit Class AbstractItr
			Implements IEnumerator(Of E)

			Private ReadOnly outerInstance As LinkedBlockingDeque

			''' <summary>
			''' The next node to return in next()
			''' </summary>
			Friend next_Renamed As Node(Of E)

			''' <summary>
			''' nextItem holds on to item fields because once we claim that
			''' an element exists in hasNext(), we must return item read
			''' under lock (in advance()) even if it was in the process of
			''' being removed when hasNext() was called.
			''' </summary>
			Friend nextItem As E

			''' <summary>
			''' Node returned by most recent call to next. Needed by remove.
			''' Reset to null if this element is deleted by a call to remove.
			''' </summary>
			Private lastRet As Node(Of E)

			Friend MustOverride Function firstNode() As Node(Of E)
			Friend MustOverride Function nextNode(ByVal n As Node(Of E)) As Node(Of E)

			Friend Sub New(ByVal outerInstance As LinkedBlockingDeque)
					Me.outerInstance = outerInstance
				' set to initial position
				Dim lock As java.util.concurrent.locks.ReentrantLock = outerInstance.lock
				lock.lock()
				Try
					next_Renamed = firstNode()
					nextItem = If(next_Renamed Is Nothing, Nothing, next_Renamed.item)
				Finally
					lock.unlock()
				End Try
			End Sub

			''' <summary>
			''' Returns the successor node of the given non-null, but
			''' possibly previously deleted, node.
			''' </summary>
			Private Function succ(ByVal n As Node(Of E)) As Node(Of E)
				' Chains of deleted nodes ending in null or self-links
				' are possible if multiple interior nodes are removed.
				Do
					Dim s As Node(Of E) = nextNode(n)
					If s Is Nothing Then
						Return Nothing
					ElseIf s.item IsNot Nothing Then
						Return s
					ElseIf s Is n Then
						Return firstNode()
					Else
						n = s
					End If
				Loop
			End Function

			''' <summary>
			''' Advances next.
			''' </summary>
			Friend Overridable Sub advance()
				Dim lock As java.util.concurrent.locks.ReentrantLock = outerInstance.lock
				lock.lock()
				Try
					' assert next != null;
					next_Renamed = succ(next_Renamed)
					nextItem = If(next_Renamed Is Nothing, Nothing, next_Renamed.item)
				Finally
					lock.unlock()
				End Try
			End Sub

			Public Overridable Function hasNext() As Boolean
				Return next_Renamed IsNot Nothing
			End Function

			Public Overridable Function [next]() As E
				If next_Renamed Is Nothing Then Throw New java.util.NoSuchElementException
				lastRet = next_Renamed
				Dim x As E = nextItem
				advance()
				Return x
			End Function

			Public Overridable Sub remove()
				Dim n As Node(Of E) = lastRet
				If n Is Nothing Then Throw New IllegalStateException
				lastRet = Nothing
				Dim lock As java.util.concurrent.locks.ReentrantLock = outerInstance.lock
				lock.lock()
				Try
					If n.item IsNot Nothing Then outerInstance.unlink(n)
				Finally
					lock.unlock()
				End Try
			End Sub
		End Class

		''' <summary>
		''' Forward iterator </summary>
		Private Class Itr
			Inherits AbstractItr

			Private ReadOnly outerInstance As LinkedBlockingDeque

			Public Sub New(ByVal outerInstance As LinkedBlockingDeque)
				Me.outerInstance = outerInstance
			End Sub

			Friend Overrides Function firstNode() As Node(Of E)
				Return outerInstance.first
			End Function
			Friend Overrides Function nextNode(ByVal n As Node(Of E)) As Node(Of E)
				Return n.next
			End Function
		End Class

		''' <summary>
		''' Descending iterator </summary>
		Private Class DescendingItr
			Inherits AbstractItr

			Private ReadOnly outerInstance As LinkedBlockingDeque

			Public Sub New(ByVal outerInstance As LinkedBlockingDeque)
				Me.outerInstance = outerInstance
			End Sub

			Friend Overrides Function firstNode() As Node(Of E)
				Return outerInstance.last
			End Function
			Friend Overrides Function nextNode(ByVal n As Node(Of E)) As Node(Of E)
				Return n.prev
			End Function
		End Class

		''' <summary>
		''' A customized variant of Spliterators.IteratorSpliterator </summary>
		Friend NotInheritable Class LBDSpliterator(Of E)
			Implements java.util.Spliterator(Of E)

			Friend Shared ReadOnly MAX_BATCH As Integer = 1 << 25 ' max batch array size;
			Friend ReadOnly queue As LinkedBlockingDeque(Of E)
			Friend current As Node(Of E) ' current node; null until initialized
			Friend batch As Integer ' batch size for splits
			Friend exhausted As Boolean ' true when no more nodes
			Friend est As Long ' size estimate
			Friend Sub New(ByVal queue As LinkedBlockingDeque(Of E))
				Me.queue = queue
				Me.est = queue.size()
			End Sub

			Public Function estimateSize() As Long
				Return est
			End Function

			Public Function trySplit() As java.util.Spliterator(Of E)
				Dim h As Node(Of E)
				Dim q As LinkedBlockingDeque(Of E) = Me.queue
				Dim b As Integer = batch
				Dim n As Integer = If(b <= 0, 1, If(b >= MAX_BATCH, MAX_BATCH, b + 1))
				h = current
				h = q.first
				If (Not exhausted) AndAlso (h IsNot Nothing OrElse h IsNot Nothing) AndAlso h.next IsNot Nothing Then
					Dim a As Object() = New Object(n - 1){}
					Dim lock As java.util.concurrent.locks.ReentrantLock = q.lock
					Dim i As Integer = 0
					Dim p As Node(Of E) = current
					lock.lock()
					Try
						p = q.first
						If p IsNot Nothing OrElse p IsNot Nothing Then
							Do
'JAVA TO VB CONVERTER TODO TASK: Assignments within expressions are not supported in VB
								If (a(i) = p.item) IsNot Nothing Then i += 1
								p = p.next
							Loop While p IsNot Nothing AndAlso i < n
						End If
					Finally
						lock.unlock()
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
				Dim q As LinkedBlockingDeque(Of E) = Me.queue
				Dim lock As java.util.concurrent.locks.ReentrantLock = q.lock
				If Not exhausted Then
					exhausted = True
					Dim p As Node(Of E) = current
					Do
						Dim e As E = Nothing
						lock.lock()
						Try
							If p Is Nothing Then p = q.first
							Do While p IsNot Nothing
								e = p.item
								p = p.next
								If e IsNot Nothing Then Exit Do
							Loop
						Finally
							lock.unlock()
						End Try
						If e IsNot Nothing Then action.accept(e)
					Loop While p IsNot Nothing
				End If
			End Sub

'JAVA TO VB CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
			Public Function tryAdvance(Of T1)(ByVal action As java.util.function.Consumer(Of T1)) As Boolean
				If action Is Nothing Then Throw New NullPointerException
				Dim q As LinkedBlockingDeque(Of E) = Me.queue
				Dim lock As java.util.concurrent.locks.ReentrantLock = q.lock
				If Not exhausted Then
					Dim e As E = Nothing
					lock.lock()
					Try
						If current Is Nothing Then current = q.first
						Do While current IsNot Nothing
							e = current.item
							current = current.next
							If e IsNot Nothing Then Exit Do
						Loop
					Finally
						lock.unlock()
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
		''' Returns a <seealso cref="Spliterator"/> over the elements in this deque.
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
		''' <returns> a {@code Spliterator} over the elements in this deque
		''' @since 1.8 </returns>
		Public Overridable Function spliterator() As java.util.Spliterator(Of E)
			Return New LBDSpliterator(Of E)(Me)
		End Function

		''' <summary>
		''' Saves this deque to a stream (that is, serializes it).
		''' </summary>
		''' <param name="s"> the stream </param>
		''' <exception cref="java.io.IOException"> if an I/O error occurs
		''' @serialData The capacity (int), followed by elements (each an
		''' {@code Object}) in the proper order, followed by a null </exception>
		Private Sub writeObject(ByVal s As java.io.ObjectOutputStream)
			Dim lock As java.util.concurrent.locks.ReentrantLock = Me.lock
			lock.lock()
			Try
				' Write out capacity and any hidden stuff
				s.defaultWriteObject()
				' Write out all elements in the proper order.
				Dim p As Node(Of E) = first
				Do While p IsNot Nothing
					s.writeObject(p.item)
					p = p.next
				Loop
				' Use trailing null as sentinel
				s.writeObject(Nothing)
			Finally
				lock.unlock()
			End Try
		End Sub

		''' <summary>
		''' Reconstitutes this deque from a stream (that is, deserializes it). </summary>
		''' <param name="s"> the stream </param>
		''' <exception cref="ClassNotFoundException"> if the class of a serialized object
		'''         could not be found </exception>
		''' <exception cref="java.io.IOException"> if an I/O error occurs </exception>
		Private Sub readObject(ByVal s As java.io.ObjectInputStream)
			s.defaultReadObject()
			count = 0
			first = Nothing
			last = Nothing
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