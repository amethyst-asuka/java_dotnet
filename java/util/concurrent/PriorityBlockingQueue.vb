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
	''' An unbounded <seealso cref="BlockingQueue blocking queue"/> that uses
	''' the same ordering rules As  [Class] <seealso cref="PriorityQueue"/> and supplies
	''' blocking retrieval operations.  While this queue is logically
	''' unbounded, attempted additions may fail due to resource exhaustion
	''' (causing {@code OutOfMemoryError}). This class does not permit
	''' {@code null} elements.  A priority queue relying on {@linkplain
	''' Comparable natural ordering} also does not permit insertion of
	''' non-comparable objects (doing so results in
	''' {@code ClassCastException}).
	''' 
	''' <p>This class and its iterator implement all of the
	''' <em>optional</em> methods of the <seealso cref="Collection"/> and {@link
	''' Iterator} interfaces.  The Iterator provided in method {@link
	''' #iterator()} is <em>not</em> guaranteed to traverse the elements of
	''' the PriorityBlockingQueue in any particular order. If you need
	''' ordered traversal, consider using
	''' {@code Arrays.sort(pq.toArray())}.  Also, method {@code drainTo}
	''' can be used to <em>remove</em> some or all elements in priority
	''' order and place them in another collection.
	''' 
	''' <p>Operations on this class make no guarantees about the ordering
	''' of elements with equal priority. If you need to enforce an
	''' ordering, you can define custom classes or comparators that use a
	''' secondary key to break ties in primary priority values.  For
	''' example, here is a class that applies first-in-first-out
	''' tie-breaking to comparable elements. To use it, you would insert a
	''' {@code new FIFOEntry(anEntry)} instead of a plain entry object.
	''' 
	'''  <pre> {@code
	''' class FIFOEntry<E extends Comparable<? super E>>
	'''     implements Comparable<FIFOEntry<E>> {
	'''   static final AtomicLong seq = new AtomicLong(0);
	'''   final long seqNum;
	'''   final E entry;
	'''   public FIFOEntry(E entry) {
	'''     seqNum = seq.getAndIncrement();
	'''     this.entry = entry;
	'''   }
	'''   public E getEntry() { return entry; }
	'''   public int compareTo(FIFOEntry<E> other) {
	'''     int res = entry.compareTo(other.entry);
	'''     if (res == 0 && other.entry != this.entry)
	'''       res = (seqNum < other.seqNum ? -1 : 1);
	'''     return res;
	'''   }
	''' }}</pre>
	''' 
	''' <p>This class is a member of the
	''' <a href="{@docRoot}/../technotes/guides/collections/index.html">
	''' Java Collections Framework</a>.
	''' 
	''' @since 1.5
	''' @author Doug Lea </summary>
	''' @param <E> the type of elements held in this collection </param>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
	<Serializable> _
	Public Class PriorityBlockingQueue(Of E)
		Inherits java.util.AbstractQueue(Of E)
		Implements BlockingQueue(Of E)

		Private Const serialVersionUID As Long = 5595510919245408276L

	'    
	'     * The implementation uses an array-based binary heap, with public
	'     * operations protected with a single lock. However, allocation
	'     * during resizing uses a simple spinlock (used only while not
	'     * holding main lock) in order to allow takes to operate
	'     * concurrently with allocation.  This avoids repeated
	'     * postponement of waiting consumers and consequent element
	'     * build-up. The need to back away from lock during allocation
	'     * makes it impossible to simply wrap delegated
	'     * java.util.PriorityQueue operations within a lock, as was done
	'     * in a previous version of this class. To maintain
	'     * interoperability, a plain PriorityQueue is still used during
	'     * serialization, which maintains compatibility at the expense of
	'     * transiently doubling overhead.
	'     

		''' <summary>
		''' Default array capacity.
		''' </summary>
		Private Const DEFAULT_INITIAL_CAPACITY As Integer = 11

		''' <summary>
		''' The maximum size of array to allocate.
		''' Some VMs reserve some header words in an array.
		''' Attempts to allocate larger arrays may result in
		''' OutOfMemoryError: Requested array size exceeds VM limit
		''' </summary>
		Private Shared ReadOnly MAX_ARRAY_SIZE As Integer =  [Integer].MAX_VALUE - 8

		''' <summary>
		''' Priority queue represented as a balanced binary heap: the two
		''' children of queue[n] are queue[2*n+1] and queue[2*(n+1)].  The
		''' priority queue is ordered by comparator, or by the elements'
		''' natural ordering, if comparator is null: For each node n in the
		''' heap and each descendant d of n, n <= d.  The element with the
		''' lowest value is in queue[0], assuming the queue is nonempty.
		''' </summary>
		<NonSerialized> _
		Private queue As Object()

		''' <summary>
		''' The number of elements in the priority queue.
		''' </summary>
		<NonSerialized> _
		Private size_Renamed As Integer

		''' <summary>
		''' The comparator, or null if priority queue uses elements'
		''' natural ordering.
		''' </summary>
'JAVA TO VB CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
		<NonSerialized> _
		Private comparator_Renamed As IComparer(Of ?)

		''' <summary>
		''' Lock used for all public operations
		''' </summary>
		Private ReadOnly lock As java.util.concurrent.locks.ReentrantLock

		''' <summary>
		''' Condition for blocking when empty
		''' </summary>
		Private ReadOnly notEmpty As java.util.concurrent.locks.Condition

		''' <summary>
		''' Spinlock for allocation, acquired via CAS.
		''' </summary>
'JAVA TO VB CONVERTER TODO TASK: There is no VB equivalent to 'volatile':
		<NonSerialized> _
		Private allocationSpinLock As Integer

		''' <summary>
		''' A plain PriorityQueue used only for serialization,
		''' to maintain compatibility with previous versions
		''' of this class. Non-null only during serialization/deserialization.
		''' </summary>
		Private q As java.util.PriorityQueue(Of E)

		''' <summary>
		''' Creates a {@code PriorityBlockingQueue} with the default
		''' initial capacity (11) that orders its elements according to
		''' their <seealso cref="Comparable natural ordering"/>.
		''' </summary>
		Public Sub New()
			Me.New(DEFAULT_INITIAL_CAPACITY, Nothing)
		End Sub

		''' <summary>
		''' Creates a {@code PriorityBlockingQueue} with the specified
		''' initial capacity that orders its elements according to their
		''' <seealso cref="Comparable natural ordering"/>.
		''' </summary>
		''' <param name="initialCapacity"> the initial capacity for this priority queue </param>
		''' <exception cref="IllegalArgumentException"> if {@code initialCapacity} is less
		'''         than 1 </exception>
		Public Sub New(ByVal initialCapacity As Integer)
			Me.New(initialCapacity, Nothing)
		End Sub

		''' <summary>
		''' Creates a {@code PriorityBlockingQueue} with the specified initial
		''' capacity that orders its elements according to the specified
		''' comparator.
		''' </summary>
		''' <param name="initialCapacity"> the initial capacity for this priority queue </param>
		''' <param name="comparator"> the comparator that will be used to order this
		'''         priority queue.  If {@code null}, the {@link Comparable
		'''         natural ordering} of the elements will be used. </param>
		''' <exception cref="IllegalArgumentException"> if {@code initialCapacity} is less
		'''         than 1 </exception>
'JAVA TO VB CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
		Public Sub New(Of T1)(ByVal initialCapacity As Integer, ByVal comparator As IComparer(Of T1))
			If initialCapacity < 1 Then Throw New IllegalArgumentException
			Me.lock = New java.util.concurrent.locks.ReentrantLock
			Me.notEmpty = lock.newCondition()
			Me.comparator_Renamed = comparator
			Me.queue = New Object(initialCapacity - 1){}
		End Sub

		''' <summary>
		''' Creates a {@code PriorityBlockingQueue} containing the elements
		''' in the specified collection.  If the specified collection is a
		''' <seealso cref="SortedSet"/> or a <seealso cref="PriorityQueue"/>, this
		''' priority queue will be ordered according to the same ordering.
		''' Otherwise, this priority queue will be ordered according to the
		''' <seealso cref="Comparable natural ordering"/> of its elements.
		''' </summary>
		''' <param name="c"> the collection whose elements are to be placed
		'''         into this priority queue </param>
		''' <exception cref="ClassCastException"> if elements of the specified collection
		'''         cannot be compared to one another according to the priority
		'''         queue's ordering </exception>
		''' <exception cref="NullPointerException"> if the specified collection or any
		'''         of its elements are null </exception>
		Public Sub New(Of T1 As E)(ByVal c As ICollection(Of T1))
			Me.lock = New java.util.concurrent.locks.ReentrantLock
			Me.notEmpty = lock.newCondition()
			Dim heapify As Boolean = True ' true if not known to be in heap order
			Dim screen As Boolean = True ' true if must screen for nulls
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
			If TypeOf c Is java.util.SortedSet(Of ?) Then
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
				Dim ss As java.util.SortedSet(Of ? As E) = CType(c, java.util.SortedSet(Of ? As E))
'JAVA TO VB CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
				Me.comparator_Renamed = CType(ss.comparator(), IComparer(Of ?))
				heapify = False
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
			ElseIf TypeOf c Is PriorityBlockingQueue(Of ?) Then
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
				Dim pq As PriorityBlockingQueue(Of ? As E) = CType(c, PriorityBlockingQueue(Of ? As E))
'JAVA TO VB CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
				Me.comparator_Renamed = CType(pq.comparator(), IComparer(Of ?))
				screen = False
				If pq.GetType() Is GetType(PriorityBlockingQueue) Then ' exact match heapify = False
			End If
			Dim a As Object() = c.ToArray()
			Dim n As Integer = a.Length
			' If c.toArray incorrectly doesn't return Object[], copy it.
			If a.GetType() IsNot GetType(Object()) Then a = java.util.Arrays.copyOf(a, n, GetType(Object()))
			If screen AndAlso (n = 1 OrElse Me.comparator_Renamed IsNot Nothing) Then
				For i As Integer = 0 To n - 1
					If a(i) Is Nothing Then Throw New NullPointerException
				Next i
			End If
			Me.queue = a
			Me.size_Renamed = n
			If heapify Then heapify()
		End Sub

		''' <summary>
		''' Tries to grow array to accommodate at least one more element
		''' (but normally expand by about 50%), giving up (allowing retry)
		''' on contention (which we expect to be rare). Call only while
		''' holding lock.
		''' </summary>
		''' <param name="array"> the heap array </param>
		''' <param name="oldCap"> the length of the array </param>
		Private Sub tryGrow(ByVal array As Object(), ByVal oldCap As Integer)
			lock.unlock() ' must release and then re-acquire main lock
			Dim newArray As Object() = Nothing
			If allocationSpinLock = 0 AndAlso UNSAFE.compareAndSwapInt(Me, allocationSpinLockOffset, 0, 1) Then
				Try
					Dim newCap As Integer = oldCap + (If(oldCap < 64, (oldCap + 2), (oldCap >> 1))) ' grow faster if small
					If newCap - MAX_ARRAY_SIZE > 0 Then ' possible overflow
						Dim minCap As Integer = oldCap + 1
						If minCap < 0 OrElse minCap > MAX_ARRAY_SIZE Then Throw New OutOfMemoryError
						newCap = MAX_ARRAY_SIZE
					End If
					If newCap > oldCap AndAlso queue = array Then newArray = New Object(newCap - 1){}
				Finally
					allocationSpinLock = 0
				End Try
			End If
			If newArray Is Nothing Then ' back off if another thread is allocating Thread.yield()
			lock.lock()
			If newArray IsNot Nothing AndAlso queue = array Then
				queue = newArray
				Array.Copy(array, 0, newArray, 0, oldCap)
			End If
		End Sub

		''' <summary>
		''' Mechanics for poll().  Call only while holding lock.
		''' </summary>
		Private Function dequeue() As E
			Dim n As Integer = size_Renamed - 1
			If n < 0 Then
				Return Nothing
			Else
				Dim array As Object() = queue
				Dim result As E = CType(array(0), E)
				Dim x As E = CType(array(n), E)
				array(n) = Nothing
'JAVA TO VB CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
				Dim cmp As IComparer(Of ?) = comparator_Renamed
				If cmp Is Nothing Then
					siftDownComparable(0, x, array, n)
				Else
					siftDownUsingComparator(0, x, array, n, cmp)
				End If
				size_Renamed = n
				Return result
			End If
		End Function

		''' <summary>
		''' Inserts item x at position k, maintaining heap invariant by
		''' promoting x up the tree until it is greater than or equal to
		''' its parent, or is the root.
		''' 
		''' To simplify and speed up coercions and comparisons. the
		''' Comparable and Comparator versions are separated into different
		''' methods that are otherwise identical. (Similarly for siftDown.)
		''' These methods are static, with heap state as arguments, to
		''' simplify use in light of possible comparator exceptions.
		''' </summary>
		''' <param name="k"> the position to fill </param>
		''' <param name="x"> the item to insert </param>
		''' <param name="array"> the heap array </param>
		Private Shared Sub siftUpComparable(Of T)(ByVal k As Integer, ByVal x As T, ByVal array As Object())
'JAVA TO VB CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
			Dim key As Comparable(Of ?) = CType(x, Comparable(Of ?))
			Do While k > 0
				Dim parent As Integer = CInt(CUInt((k - 1)) >> 1)
				Dim e As Object = array(parent)
				If key.CompareTo(CType(e, T)) >= 0 Then Exit Do
				array(k) = e
				k = parent
			Loop
			array(k) = key
		End Sub

'JAVA TO VB CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
		Private Shared Sub siftUpUsingComparator(Of T, T1)(ByVal k As Integer, ByVal x As T, ByVal array As Object(), ByVal cmp As IComparer(Of T1))
			Do While k > 0
				Dim parent As Integer = CInt(CUInt((k - 1)) >> 1)
				Dim e As Object = array(parent)
				If cmp.Compare(x, CType(e, T)) >= 0 Then Exit Do
				array(k) = e
				k = parent
			Loop
			array(k) = x
		End Sub

		''' <summary>
		''' Inserts item x at position k, maintaining heap invariant by
		''' demoting x down the tree repeatedly until it is less than or
		''' equal to its children or is a leaf.
		''' </summary>
		''' <param name="k"> the position to fill </param>
		''' <param name="x"> the item to insert </param>
		''' <param name="array"> the heap array </param>
		''' <param name="n"> heap size </param>
		Private Shared Sub siftDownComparable(Of T)(ByVal k As Integer, ByVal x As T, ByVal array As Object(), ByVal n As Integer)
			If n > 0 Then
'JAVA TO VB CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
				Dim key As Comparable(Of ?) = CType(x, Comparable(Of ?))
				Dim half As Integer = CInt(CUInt(n) >> 1) ' loop while a non-leaf
				Do While k < half
					Dim child As Integer = (k << 1) + 1 ' assume left child is least
					Dim c As Object = array(child)
					Dim right As Integer = child + 1
'JAVA TO VB CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
					If right < n AndAlso CType(c, Comparable(Of ?)).CompareTo(CType(array(right), T)) > 0 Then c = array(child = right)
					If key.CompareTo(CType(c, T)) <= 0 Then Exit Do
					array(k) = c
					k = child
				Loop
				array(k) = key
			End If
		End Sub

'JAVA TO VB CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
		Private Shared Sub siftDownUsingComparator(Of T, T1)(ByVal k As Integer, ByVal x As T, ByVal array As Object(), ByVal n As Integer, ByVal cmp As IComparer(Of T1))
			If n > 0 Then
				Dim half As Integer = CInt(CUInt(n) >> 1)
				Do While k < half
					Dim child As Integer = (k << 1) + 1
					Dim c As Object = array(child)
					Dim right As Integer = child + 1
					If right < n AndAlso cmp.Compare(CType(c, T), CType(array(right), T)) > 0 Then c = array(child = right)
					If cmp.Compare(x, CType(c, T)) <= 0 Then Exit Do
					array(k) = c
					k = child
				Loop
				array(k) = x
			End If
		End Sub

		''' <summary>
		''' Establishes the heap invariant (described above) in the entire tree,
		''' assuming nothing about the order of the elements prior to the call.
		''' </summary>
		Private Sub heapify()
			Dim array As Object() = queue
			Dim n As Integer = size_Renamed
			Dim half As Integer = (CInt(CUInt(n) >> 1)) - 1
'JAVA TO VB CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
			Dim cmp As IComparer(Of ?) = comparator_Renamed
			If cmp Is Nothing Then
				For i As Integer = half To 0 Step -1
					siftDownComparable(i, CType(array(i), E), array, n)
				Next i
			Else
				For i As Integer = half To 0 Step -1
					siftDownUsingComparator(i, CType(array(i), E), array, n, cmp)
				Next i
			End If
		End Sub

		''' <summary>
		''' Inserts the specified element into this priority queue.
		''' </summary>
		''' <param name="e"> the element to add </param>
		''' <returns> {@code true} (as specified by <seealso cref="Collection#add"/>) </returns>
		''' <exception cref="ClassCastException"> if the specified element cannot be compared
		'''         with elements currently in the priority queue according to the
		'''         priority queue's ordering </exception>
		''' <exception cref="NullPointerException"> if the specified element is null </exception>
		Public Overridable Function add(ByVal e As E) As Boolean
			Return offer(e)
		End Function

		''' <summary>
		''' Inserts the specified element into this priority queue.
		''' As the queue is unbounded, this method will never return {@code false}.
		''' </summary>
		''' <param name="e"> the element to add </param>
		''' <returns> {@code true} (as specified by <seealso cref="Queue#offer"/>) </returns>
		''' <exception cref="ClassCastException"> if the specified element cannot be compared
		'''         with elements currently in the priority queue according to the
		'''         priority queue's ordering </exception>
		''' <exception cref="NullPointerException"> if the specified element is null </exception>
		Public Overridable Function offer(ByVal e As E) As Boolean
			If e Is Nothing Then Throw New NullPointerException
			Dim lock As java.util.concurrent.locks.ReentrantLock = Me.lock
			lock.lock()
			Dim n, cap As Integer
			Dim array As Object()
			n = size_Renamed
'JAVA TO VB CONVERTER TODO TASK: Assignments within expressions are not supported in VB
			cap = (array = queue).length
			Do While n >= cap
				tryGrow(array, cap)
				n = size_Renamed
				cap = (array = queue).length
			Loop
			Try
'JAVA TO VB CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
				Dim cmp As IComparer(Of ?) = comparator_Renamed
				If cmp Is Nothing Then
					siftUpComparable(n, e, array)
				Else
					siftUpUsingComparator(n, e, array, cmp)
				End If
				size_Renamed = n + 1
				notEmpty.signal()
			Finally
				lock.unlock()
			End Try
			Return True
		End Function

		''' <summary>
		''' Inserts the specified element into this priority queue.
		''' As the queue is unbounded, this method will never block.
		''' </summary>
		''' <param name="e"> the element to add </param>
		''' <exception cref="ClassCastException"> if the specified element cannot be compared
		'''         with elements currently in the priority queue according to the
		'''         priority queue's ordering </exception>
		''' <exception cref="NullPointerException"> if the specified element is null </exception>
		Public Overridable Sub put(ByVal e As E)
			offer(e) ' never need to block
		End Sub

		''' <summary>
		''' Inserts the specified element into this priority queue.
		''' As the queue is unbounded, this method will never block or
		''' return {@code false}.
		''' </summary>
		''' <param name="e"> the element to add </param>
		''' <param name="timeout"> This parameter is ignored as the method never blocks </param>
		''' <param name="unit"> This parameter is ignored as the method never blocks </param>
		''' <returns> {@code true} (as specified by
		'''  <seealso cref="BlockingQueue#offer(Object,long,TimeUnit) BlockingQueue.offer"/>) </returns>
		''' <exception cref="ClassCastException"> if the specified element cannot be compared
		'''         with elements currently in the priority queue according to the
		'''         priority queue's ordering </exception>
		''' <exception cref="NullPointerException"> if the specified element is null </exception>
		Public Overridable Function offer(ByVal e As E, ByVal timeout As Long, ByVal unit As TimeUnit) As Boolean
			Return offer(e) ' never need to block
		End Function

		Public Overridable Function poll() As E
			Dim lock As java.util.concurrent.locks.ReentrantLock = Me.lock
			lock.lock()
			Try
				Return dequeue()
			Finally
				lock.unlock()
			End Try
		End Function

		Public Overridable Function take() As E Implements BlockingQueue(Of E).take
			Dim lock As java.util.concurrent.locks.ReentrantLock = Me.lock
			lock.lockInterruptibly()
			Dim result As E
			Try
				result = dequeue()
				Do While result Is Nothing
					notEmpty.await()
					result = dequeue()
				Loop
			Finally
				lock.unlock()
			End Try
			Return result
		End Function

		Public Overridable Function poll(ByVal timeout As Long, ByVal unit As TimeUnit) As E Implements BlockingQueue(Of E).poll
			Dim nanos As Long = unit.toNanos(timeout)
			Dim lock As java.util.concurrent.locks.ReentrantLock = Me.lock
			lock.lockInterruptibly()
			Dim result As E
			Try
				result = dequeue()
				Do While result Is Nothing AndAlso nanos > 0
					nanos = notEmpty.awaitNanos(nanos)
					result = dequeue()
				Loop
			Finally
				lock.unlock()
			End Try
			Return result
		End Function

		Public Overridable Function peek() As E
			Dim lock As java.util.concurrent.locks.ReentrantLock = Me.lock
			lock.lock()
			Try
				Return If(size_Renamed = 0, Nothing, CType(queue(0), E))
			Finally
				lock.unlock()
			End Try
		End Function

		''' <summary>
		''' Returns the comparator used to order the elements in this queue,
		''' or {@code null} if this queue uses the {@link Comparable
		''' natural ordering} of its elements.
		''' </summary>
		''' <returns> the comparator used to order the elements in this queue,
		'''         or {@code null} if this queue uses the natural
		'''         ordering of its elements </returns>
'JAVA TO VB CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
		Public Overridable Function comparator() As IComparer(Of ?)
			Return comparator_Renamed
		End Function

		Public Overridable Function size() As Integer
			Dim lock As java.util.concurrent.locks.ReentrantLock = Me.lock
			lock.lock()
			Try
				Return size_Renamed
			Finally
				lock.unlock()
			End Try
		End Function

		''' <summary>
		''' Always returns {@code  [Integer].MAX_VALUE} because
		''' a {@code PriorityBlockingQueue} is not capacity constrained. </summary>
		''' <returns> {@code  [Integer].MAX_VALUE} always </returns>
		Public Overridable Function remainingCapacity() As Integer Implements BlockingQueue(Of E).remainingCapacity
			Return Integer.MaxValue
		End Function

		Private Function indexOf(ByVal o As Object) As Integer
			If o IsNot Nothing Then
				Dim array As Object() = queue
				Dim n As Integer = size_Renamed
				For i As Integer = 0 To n - 1
					If o.Equals(array(i)) Then Return i
				Next i
			End If
			Return -1
		End Function

		''' <summary>
		''' Removes the ith element from queue.
		''' </summary>
		Private Sub removeAt(ByVal i As Integer)
			Dim array As Object() = queue
			Dim n As Integer = size_Renamed - 1
			If n = i Then ' removed last element
				array(i) = Nothing
			Else
				Dim moved As E = CType(array(n), E)
				array(n) = Nothing
'JAVA TO VB CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
				Dim cmp As IComparer(Of ?) = comparator_Renamed
				If cmp Is Nothing Then
					siftDownComparable(i, moved, array, n)
				Else
					siftDownUsingComparator(i, moved, array, n, cmp)
				End If
				If array(i) Is moved Then
					If cmp Is Nothing Then
						siftUpComparable(i, moved, array)
					Else
						siftUpUsingComparator(i, moved, array, cmp)
					End If
				End If
			End If
			size_Renamed = n
		End Sub

		''' <summary>
		''' Removes a single instance of the specified element from this queue,
		''' if it is present.  More formally, removes an element {@code e} such
		''' that {@code o.equals(e)}, if this queue contains one or more such
		''' elements.  Returns {@code true} if and only if this queue contained
		''' the specified element (or equivalently, if this queue changed as a
		''' result of the call).
		''' </summary>
		''' <param name="o"> element to be removed from this queue, if present </param>
		''' <returns> {@code true} if this queue changed as a result of the call </returns>
		Public Overridable Function remove(ByVal o As Object) As Boolean Implements BlockingQueue(Of E).remove
			Dim lock As java.util.concurrent.locks.ReentrantLock = Me.lock
			lock.lock()
			Try
				Dim i As Integer = IndexOf(o)
				If i = -1 Then Return False
				removeAt(i)
				Return True
			Finally
				lock.unlock()
			End Try
		End Function

		''' <summary>
		''' Identity-based version for use in Itr.remove
		''' </summary>
		Friend Overridable Sub removeEQ(ByVal o As Object)
			Dim lock As java.util.concurrent.locks.ReentrantLock = Me.lock
			lock.lock()
			Try
				Dim array As Object() = queue
				Dim i As Integer = 0
				Dim n As Integer = size_Renamed
				Do While i < n
					If o Is array(i) Then
						removeAt(i)
						Exit Do
					End If
					i += 1
				Loop
			Finally
				lock.unlock()
			End Try
		End Sub

		''' <summary>
		''' Returns {@code true} if this queue contains the specified element.
		''' More formally, returns {@code true} if and only if this queue contains
		''' at least one element {@code e} such that {@code o.equals(e)}.
		''' </summary>
		''' <param name="o"> object to be checked for containment in this queue </param>
		''' <returns> {@code true} if this queue contains the specified element </returns>
		Public Overridable Function contains(ByVal o As Object) As Boolean Implements BlockingQueue(Of E).contains
			Dim lock As java.util.concurrent.locks.ReentrantLock = Me.lock
			lock.lock()
			Try
				Return IndexOf(o) <> -1
			Finally
				lock.unlock()
			End Try
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
				Return java.util.Arrays.copyOf(queue, size_Renamed)
			Finally
				lock.unlock()
			End Try
		End Function

		Public Overrides Function ToString() As String
			Dim lock As java.util.concurrent.locks.ReentrantLock = Me.lock
			lock.lock()
			Try
				Dim n As Integer = size_Renamed
				If n = 0 Then Return "[]"
				Dim sb As New StringBuilder
				sb.append("["c)
				For i As Integer = 0 To n - 1
					Dim e As Object = queue(i)
					sb.append(If(e Is Me, "(this Collection)", e))
					If i <> n - 1 Then sb.append(","c).append(" "c)
				Next i
				Return sb.append("]"c).ToString()
			Finally
				lock.unlock()
			End Try
		End Function

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
			Dim lock As java.util.concurrent.locks.ReentrantLock = Me.lock
			lock.lock()
			Try
				Dim n As Integer = Math.Min(size_Renamed, maxElements)
				For i As Integer = 0 To n - 1
					c.Add(CType(queue(0), E)) ' In this order, in case add() throws.
					dequeue()
				Next i
				Return n
			Finally
				lock.unlock()
			End Try
		End Function

		''' <summary>
		''' Atomically removes all of the elements from this queue.
		''' The queue will be empty after this call returns.
		''' </summary>
		Public Overridable Sub clear()
			Dim lock As java.util.concurrent.locks.ReentrantLock = Me.lock
			lock.lock()
			Try
				Dim array As Object() = queue
				Dim n As Integer = size_Renamed
				size_Renamed = 0
				For i As Integer = 0 To n - 1
					array(i) = Nothing
				Next i
			Finally
				lock.unlock()
			End Try
		End Sub

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
		Public Overridable Function toArray(Of T)(ByVal a As T()) As T()
			Dim lock As java.util.concurrent.locks.ReentrantLock = Me.lock
			lock.lock()
			Try
				Dim n As Integer = size_Renamed
				If a.Length < n Then Return CType(java.util.Arrays.copyOf(queue, size_Renamed, a.GetType()), T())
				Array.Copy(queue, 0, a, 0, n)
				If a.Length > n Then a(n) = Nothing
				Return a
			Finally
				lock.unlock()
			End Try
		End Function

		''' <summary>
		''' Returns an iterator over the elements in this queue. The
		''' iterator does not return the elements in any particular order.
		''' 
		''' <p>The returned iterator is
		''' <a href="package-summary.html#Weakly"><i>weakly consistent</i></a>.
		''' </summary>
		''' <returns> an iterator over the elements in this queue </returns>
		Public Overridable Function [iterator]() As IEnumerator(Of E)
			Return New Itr(Me, ToArray())
		End Function

		''' <summary>
		''' Snapshot iterator that works off copy of underlying q array.
		''' </summary>
		Friend NotInheritable Class Itr
			Implements IEnumerator(Of E)

			Private ReadOnly outerInstance As PriorityBlockingQueue

			Friend ReadOnly array As Object() ' Array of all elements
			Friend cursor As Integer ' index of next element to return
			Friend lastRet As Integer ' index of last element, or -1 if no such

			Friend Sub New(ByVal outerInstance As PriorityBlockingQueue, ByVal array As Object())
					Me.outerInstance = outerInstance
				lastRet = -1
				Me.array = array
			End Sub

			Public Function hasNext() As Boolean
				Return cursor < array.Length
			End Function

			Public Function [next]() As E
				If cursor >= array.Length Then Throw New java.util.NoSuchElementException
				lastRet = cursor
					Dim tempVar As Integer = cursor
					cursor += 1
					Return CType(array(tempVar), E)
			End Function

			Public Sub remove()
				If lastRet < 0 Then Throw New IllegalStateException
				outerInstance.removeEQ(array(lastRet))
				lastRet = -1
			End Sub
		End Class

		''' <summary>
		''' Saves this queue to a stream (that is, serializes it).
		''' 
		''' For compatibility with previous version of this [Class], elements
		''' are first copied to a java.util.PriorityQueue, which is then
		''' serialized.
		''' </summary>
		''' <param name="s"> the stream </param>
		''' <exception cref="java.io.IOException"> if an I/O error occurs </exception>
		Private Sub writeObject(ByVal s As java.io.ObjectOutputStream)
			lock.lock()
			Try
				' avoid zero capacity argument
				q = New java.util.PriorityQueue(Of E)(Math.Max(size_Renamed, 1), comparator_Renamed)
				q.addAll(Me)
				s.defaultWriteObject()
			Finally
				q = Nothing
				lock.unlock()
			End Try
		End Sub

		''' <summary>
		''' Reconstitutes this queue from a stream (that is, deserializes it). </summary>
		''' <param name="s"> the stream </param>
		''' <exception cref="ClassNotFoundException"> if the class of a serialized object
		'''         could not be found </exception>
		''' <exception cref="java.io.IOException"> if an I/O error occurs </exception>
		Private Sub readObject(ByVal s As java.io.ObjectInputStream)
			Try
				s.defaultReadObject()
				Me.queue = New Object(q.size() - 1){}
				comparator_Renamed = q.comparator()
				addAll(q)
			Finally
				q = Nothing
			End Try
		End Sub

		' Similar to Collections.ArraySnapshotSpliterator but avoids
		' commitment to toArray until needed
		Friend NotInheritable Class PBQSpliterator(Of E)
			Implements java.util.Spliterator(Of E)

			Friend ReadOnly queue As PriorityBlockingQueue(Of E)
			Friend array As Object()
			Friend index As Integer
			Friend fence As Integer

			Friend Sub New(ByVal queue As PriorityBlockingQueue(Of E), ByVal array As Object(), ByVal index As Integer, ByVal fence As Integer)
				Me.queue = queue
				Me.array = array
				Me.index = index
				Me.fence = fence
			End Sub

			Friend Property fence As Integer
				Get
					Dim hi As Integer
					hi = fence
					If hi < 0 Then
	'JAVA TO VB CONVERTER TODO TASK: Assignments within expressions are not supported in VB
							fence = (array = queue.ToArray()).length
							hi = fence
					End If
					Return hi
				End Get
			End Property

			Public Function trySplit() As java.util.Spliterator(Of E)
				Dim hi As Integer = fence, lo As Integer = index, mid As Integer = CInt(CUInt((lo + hi)) >> 1)
'JAVA TO VB CONVERTER TODO TASK: Assignments within expressions are not supported in VB
				Return If(lo >= mid, Nothing, New PBQSpliterator(Of E)(queue, array, lo, index = mid))
			End Function

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'JAVA TO VB CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
			Public Sub forEachRemaining(Of T1)(ByVal action As java.util.function.Consumer(Of T1))
				Dim a As Object() ' hoist accesses and checks from loop
				Dim i, hi As Integer
				If action Is Nothing Then Throw New NullPointerException
				a = array
				If a Is Nothing Then fence = (a = queue.ToArray()).length
				hi = fence
				i = index
				index = hi
				If hi <= a.Length AndAlso i >= 0 AndAlso i < index Then
					Do
						action.accept(CType(a(i), E))
						i += 1
					Loop While i < hi
				End If
			End Sub

'JAVA TO VB CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
			Public Function tryAdvance(Of T1)(ByVal action As java.util.function.Consumer(Of T1)) As Boolean
				If action Is Nothing Then Throw New NullPointerException
				If fence > index AndAlso index >= 0 Then
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
					Dim e As E = CType(array(index), E)
					index += 1
					action.accept(e)
					Return True
				End If
				Return False
			End Function

			Public Function estimateSize() As Long
				Return CLng(fence - index)
			End Function

			Public Function characteristics() As Integer
				Return java.util.Spliterator.NONNULL Or java.util.Spliterator.SIZED Or java.util.Spliterator.SUBSIZED
			End Function
		End Class

		''' <summary>
		''' Returns a <seealso cref="Spliterator"/> over the elements in this queue.
		''' 
		''' <p>The returned spliterator is
		''' <a href="package-summary.html#Weakly"><i>weakly consistent</i></a>.
		''' 
		''' <p>The {@code Spliterator} reports <seealso cref="Spliterator#SIZED"/> and
		''' <seealso cref="Spliterator#NONNULL"/>.
		''' 
		''' @implNote
		''' The {@code Spliterator} additionally reports <seealso cref="Spliterator#SUBSIZED"/>.
		''' </summary>
		''' <returns> a {@code Spliterator} over the elements in this queue
		''' @since 1.8 </returns>
		Public Overridable Function spliterator() As java.util.Spliterator(Of E)
			Return New PBQSpliterator(Of E)(Me, Nothing, 0, -1)
		End Function

		' Unsafe mechanics
		Private Shared ReadOnly UNSAFE As sun.misc.Unsafe
		Private Shared ReadOnly allocationSpinLockOffset As Long
		Shared Sub New()
			Try
				UNSAFE = sun.misc.Unsafe.unsafe
				Dim k As  [Class] = GetType(PriorityBlockingQueue)
				allocationSpinLockOffset = UNSAFE.objectFieldOffset(k.getDeclaredField("allocationSpinLock"))
			Catch e As Exception
				Throw New [Error](e)
			End Try
		End Sub
	End Class

End Namespace