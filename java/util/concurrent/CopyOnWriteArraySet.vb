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
	''' A <seealso cref="java.util.Set"/> that uses an internal <seealso cref="CopyOnWriteArrayList"/>
	''' for all of its operations.  Thus, it shares the same basic properties:
	''' <ul>
	'''  <li>It is best suited for applications in which set sizes generally
	'''       stay small, read-only operations
	'''       vastly outnumber mutative operations, and you need
	'''       to prevent interference among threads during traversal.
	'''  <li>It is thread-safe.
	'''  <li>Mutative operations ({@code add}, {@code set}, {@code remove}, etc.)
	'''      are expensive since they usually entail copying the entire underlying
	'''      array.
	'''  <li>Iterators do not support the mutative {@code remove} operation.
	'''  <li>Traversal via iterators is fast and cannot encounter
	'''      interference from other threads. Iterators rely on
	'''      unchanging snapshots of the array at the time the iterators were
	'''      constructed.
	''' </ul>
	''' 
	''' <p><b>Sample Usage.</b> The following code sketch uses a
	''' copy-on-write set to maintain a set of Handler objects that
	''' perform some action upon state updates.
	''' 
	'''  <pre> {@code
	''' class Handler {  Sub  handle(); ... }
	''' 
	''' class X {
	'''   private final CopyOnWriteArraySet<Handler> handlers
	'''     = new CopyOnWriteArraySet<Handler>();
	'''   public  Sub  addHandler(Handler h) { handlers.add(h); }
	''' 
	'''   private long internalState;
	'''   private synchronized  Sub  changeState() { internalState = ...; }
	''' 
	'''   public  Sub  update() {
	'''     changeState();
	'''     for (Handler handler : handlers)
	'''       handler.handle();
	'''   }
	''' }}</pre>
	''' 
	''' <p>This class is a member of the
	''' <a href="{@docRoot}/../technotes/guides/collections/index.html">
	''' Java Collections Framework</a>.
	''' </summary>
	''' <seealso cref= CopyOnWriteArrayList
	''' @since 1.5
	''' @author Doug Lea </seealso>
	''' @param <E> the type of elements held in this collection </param>
	<Serializable> _
	Public Class CopyOnWriteArraySet(Of E)
		Inherits java.util.AbstractSet(Of E)

		Private Const serialVersionUID As Long = 5457747651344034263L

		Private ReadOnly al As CopyOnWriteArrayList(Of E)

		''' <summary>
		''' Creates an empty set.
		''' </summary>
		Public Sub New()
			al = New CopyOnWriteArrayList(Of E)
		End Sub

		''' <summary>
		''' Creates a set containing all of the elements of the specified
		''' collection.
		''' </summary>
		''' <param name="c"> the collection of elements to initially contain </param>
		''' <exception cref="NullPointerException"> if the specified collection is null </exception>
		Public Sub New(Of T1 As E)(  c As ICollection(Of T1))
			If c.GetType() Is GetType(CopyOnWriteArraySet) Then
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
				Dim cc As CopyOnWriteArraySet(Of E) = CType(c, CopyOnWriteArraySet(Of E))
				al = New CopyOnWriteArrayList(Of E)(cc.al)
			Else
				al = New CopyOnWriteArrayList(Of E)
				al.addAllAbsent(c)
			End If
		End Sub

		''' <summary>
		''' Returns the number of elements in this set.
		''' </summary>
		''' <returns> the number of elements in this set </returns>
		Public Overridable Function size() As Integer
			Return al.size()
		End Function

		''' <summary>
		''' Returns {@code true} if this set contains no elements.
		''' </summary>
		''' <returns> {@code true} if this set contains no elements </returns>
		Public Overridable Property empty As Boolean
			Get
				Return al.empty
			End Get
		End Property

		''' <summary>
		''' Returns {@code true} if this set contains the specified element.
		''' More formally, returns {@code true} if and only if this set
		''' contains an element {@code e} such that
		''' <tt>(o==null&nbsp;?&nbsp;e==null&nbsp;:&nbsp;o.equals(e))</tt>.
		''' </summary>
		''' <param name="o"> element whose presence in this set is to be tested </param>
		''' <returns> {@code true} if this set contains the specified element </returns>
		Public Overridable Function contains(  o As Object) As Boolean
			Return al.contains(o)
		End Function

		''' <summary>
		''' Returns an array containing all of the elements in this set.
		''' If this set makes any guarantees as to what order its elements
		''' are returned by its iterator, this method must return the
		''' elements in the same order.
		''' 
		''' <p>The returned array will be "safe" in that no references to it
		''' are maintained by this set.  (In other words, this method must
		''' allocate a new array even if this set is backed by an array).
		''' The caller is thus free to modify the returned array.
		''' 
		''' <p>This method acts as bridge between array-based and collection-based
		''' APIs.
		''' </summary>
		''' <returns> an array containing all the elements in this set </returns>
		Public Overridable Function toArray() As Object()
			Return al.ToArray()
		End Function

		''' <summary>
		''' Returns an array containing all of the elements in this set; the
		''' runtime type of the returned array is that of the specified array.
		''' If the set fits in the specified array, it is returned therein.
		''' Otherwise, a new array is allocated with the runtime type of the
		''' specified array and the size of this set.
		''' 
		''' <p>If this set fits in the specified array with room to spare
		''' (i.e., the array has more elements than this set), the element in
		''' the array immediately following the end of the set is set to
		''' {@code null}.  (This is useful in determining the length of this
		''' set <i>only</i> if the caller knows that this set does not contain
		''' any null elements.)
		''' 
		''' <p>If this set makes any guarantees as to what order its elements
		''' are returned by its iterator, this method must return the elements
		''' in the same order.
		''' 
		''' <p>Like the <seealso cref="#toArray()"/> method, this method acts as bridge between
		''' array-based and collection-based APIs.  Further, this method allows
		''' precise control over the runtime type of the output array, and may,
		''' under certain circumstances, be used to save allocation costs.
		''' 
		''' <p>Suppose {@code x} is a set known to contain only strings.
		''' The following code can be used to dump the set into a newly allocated
		''' array of {@code String}:
		''' 
		'''  <pre> {@code String[] y = x.toArray(new String[0]);}</pre>
		''' 
		''' Note that {@code toArray(new Object[0])} is identical in function to
		''' {@code toArray()}.
		''' </summary>
		''' <param name="a"> the array into which the elements of this set are to be
		'''        stored, if it is big enough; otherwise, a new array of the same
		'''        runtime type is allocated for this purpose. </param>
		''' <returns> an array containing all the elements in this set </returns>
		''' <exception cref="ArrayStoreException"> if the runtime type of the specified array
		'''         is not a supertype of the runtime type of every element in this
		'''         set </exception>
		''' <exception cref="NullPointerException"> if the specified array is null </exception>
		Public Overridable Function toArray(Of T)(  a As T()) As T()
			Return al.ToArray(a)
		End Function

		''' <summary>
		''' Removes all of the elements from this set.
		''' The set will be empty after this call returns.
		''' </summary>
		Public Overridable Sub clear()
			al.clear()
		End Sub

		''' <summary>
		''' Removes the specified element from this set if it is present.
		''' More formally, removes an element {@code e} such that
		''' <tt>(o==null&nbsp;?&nbsp;e==null&nbsp;:&nbsp;o.equals(e))</tt>,
		''' if this set contains such an element.  Returns {@code true} if
		''' this set contained the element (or equivalently, if this set
		''' changed as a result of the call).  (This set will not contain the
		''' element once the call returns.)
		''' </summary>
		''' <param name="o"> object to be removed from this set, if present </param>
		''' <returns> {@code true} if this set contained the specified element </returns>
		Public Overridable Function remove(  o As Object) As Boolean
			Return al.remove(o)
		End Function

		''' <summary>
		''' Adds the specified element to this set if it is not already present.
		''' More formally, adds the specified element {@code e} to this set if
		''' the set contains no element {@code e2} such that
		''' <tt>(e==null&nbsp;?&nbsp;e2==null&nbsp;:&nbsp;e.equals(e2))</tt>.
		''' If this set already contains the element, the call leaves the set
		''' unchanged and returns {@code false}.
		''' </summary>
		''' <param name="e"> element to be added to this set </param>
		''' <returns> {@code true} if this set did not already contain the specified
		'''         element </returns>
		Public Overridable Function add(  e As E) As Boolean
			Return al.addIfAbsent(e)
		End Function

		''' <summary>
		''' Returns {@code true} if this set contains all of the elements of the
		''' specified collection.  If the specified collection is also a set, this
		''' method returns {@code true} if it is a <i>subset</i> of this set.
		''' </summary>
		''' <param name="c"> collection to be checked for containment in this set </param>
		''' <returns> {@code true} if this set contains all of the elements of the
		'''         specified collection </returns>
		''' <exception cref="NullPointerException"> if the specified collection is null </exception>
		''' <seealso cref= #contains(Object) </seealso>
		Public Overridable Function containsAll(Of T1)(  c As ICollection(Of T1)) As Boolean
			Return al.containsAll(c)
		End Function

		''' <summary>
		''' Adds all of the elements in the specified collection to this set if
		''' they're not already present.  If the specified collection is also a
		''' set, the {@code addAll} operation effectively modifies this set so
		''' that its value is the <i>union</i> of the two sets.  The behavior of
		''' this operation is undefined if the specified collection is modified
		''' while the operation is in progress.
		''' </summary>
		''' <param name="c"> collection containing elements to be added to this set </param>
		''' <returns> {@code true} if this set changed as a result of the call </returns>
		''' <exception cref="NullPointerException"> if the specified collection is null </exception>
		''' <seealso cref= #add(Object) </seealso>
		Public Overridable Function addAll(Of T1 As E)(  c As ICollection(Of T1)) As Boolean
			Return al.addAllAbsent(c) > 0
		End Function

		''' <summary>
		''' Removes from this set all of its elements that are contained in the
		''' specified collection.  If the specified collection is also a set,
		''' this operation effectively modifies this set so that its value is the
		''' <i>asymmetric set difference</i> of the two sets.
		''' </summary>
		''' <param name="c"> collection containing elements to be removed from this set </param>
		''' <returns> {@code true} if this set changed as a result of the call </returns>
		''' <exception cref="ClassCastException"> if the class of an element of this set
		'''         is incompatible with the specified collection (optional) </exception>
		''' <exception cref="NullPointerException"> if this set contains a null element and the
		'''         specified collection does not permit null elements (optional),
		'''         or if the specified collection is null </exception>
		''' <seealso cref= #remove(Object) </seealso>
		Public Overridable Function removeAll(Of T1)(  c As ICollection(Of T1)) As Boolean
			Return al.removeAll(c)
		End Function

		''' <summary>
		''' Retains only the elements in this set that are contained in the
		''' specified collection.  In other words, removes from this set all of
		''' its elements that are not contained in the specified collection.  If
		''' the specified collection is also a set, this operation effectively
		''' modifies this set so that its value is the <i>intersection</i> of the
		''' two sets.
		''' </summary>
		''' <param name="c"> collection containing elements to be retained in this set </param>
		''' <returns> {@code true} if this set changed as a result of the call </returns>
		''' <exception cref="ClassCastException"> if the class of an element of this set
		'''         is incompatible with the specified collection (optional) </exception>
		''' <exception cref="NullPointerException"> if this set contains a null element and the
		'''         specified collection does not permit null elements (optional),
		'''         or if the specified collection is null </exception>
		''' <seealso cref= #remove(Object) </seealso>
		Public Overridable Function retainAll(Of T1)(  c As ICollection(Of T1)) As Boolean
			Return al.retainAll(c)
		End Function

		''' <summary>
		''' Returns an iterator over the elements contained in this set
		''' in the order in which these elements were added.
		''' 
		''' <p>The returned iterator provides a snapshot of the state of the set
		''' when the iterator was constructed. No synchronization is needed while
		''' traversing the iterator. The iterator does <em>NOT</em> support the
		''' {@code remove} method.
		''' </summary>
		''' <returns> an iterator over the elements in this set </returns>
		Public Overridable Function [iterator]() As IEnumerator(Of E)
			Return al.GetEnumerator()
		End Function

		''' <summary>
		''' Compares the specified object with this set for equality.
		''' Returns {@code true} if the specified object is the same object
		''' as this object, or if it is also a <seealso cref="Set"/> and the elements
		''' returned by an <seealso cref="Set#iterator() iterator"/> over the
		''' specified set are the same as the elements returned by an
		''' iterator over this set.  More formally, the two iterators are
		''' considered to return the same elements if they return the same
		''' number of elements and for every element {@code e1} returned by
		''' the iterator over the specified set, there is an element
		''' {@code e2} returned by the iterator over this set such that
		''' {@code (e1==null ? e2==null : e1.equals(e2))}.
		''' </summary>
		''' <param name="o"> object to be compared for equality with this set </param>
		''' <returns> {@code true} if the specified object is equal to this set </returns>
		Public Overrides Function Equals(  o As Object) As Boolean
			If o Is Me Then Return True
			If Not(TypeOf o Is java.util.Set) Then Return False
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
			Dim [set] As java.util.Set(Of ?) = CType(o, java.util.Set(Of ?))
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
			Dim it As IEnumerator(Of ?) = [set].GetEnumerator()

			' Uses O(n^2) algorithm that is only appropriate
			' for small sets, which CopyOnWriteArraySets should be.

			'  Use a single snapshot of underlying array
			Dim elements As Object() = al.array
			Dim len As Integer = elements.Length
			' Mark matched elements to avoid re-checking
			Dim matched As Boolean() = New Boolean(len - 1){}
			Dim k As Integer = 0
			outer:
			Do While it.MoveNext()
				k += 1
				If k > len Then Return False
				Dim x As Object = it.Current
				For i As Integer = 0 To len - 1
					If (Not matched(i)) AndAlso eq(x, elements(i)) Then
						matched(i) = True
						GoTo outer
					End If
				Next i
				Return False
			Loop
			Return k = len
		End Function

'JAVA TO VB CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
		Public Overridable Function removeIf(Of T1)(  filter As java.util.function.Predicate(Of T1)) As Boolean
			Return al.removeIf(filter)
		End Function

'JAVA TO VB CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
		Public Overridable Sub forEach(Of T1)(  action As java.util.function.Consumer(Of T1))
			al.forEach(action)
		End Sub

		''' <summary>
		''' Returns a <seealso cref="Spliterator"/> over the elements in this set in the order
		''' in which these elements were added.
		''' 
		''' <p>The {@code Spliterator} reports <seealso cref="Spliterator#IMMUTABLE"/>,
		''' <seealso cref="Spliterator#DISTINCT"/>, <seealso cref="Spliterator#SIZED"/>, and
		''' <seealso cref="Spliterator#SUBSIZED"/>.
		''' 
		''' <p>The spliterator provides a snapshot of the state of the set
		''' when the spliterator was constructed. No synchronization is needed while
		''' operating on the spliterator.
		''' </summary>
		''' <returns> a {@code Spliterator} over the elements in this set
		''' @since 1.8 </returns>
		Public Overridable Function spliterator() As java.util.Spliterator(Of E)
			Return java.util.Spliterators.spliterator(al.array, java.util.Spliterator.IMMUTABLE Or java.util.Spliterator.DISTINCT)
		End Function

		''' <summary>
		''' Tests for equality, coping with nulls.
		''' </summary>
		Private Shared Function eq(  o1 As Object,   o2 As Object) As Boolean
			Return If(o1 Is Nothing, o2 Is Nothing, o1.Equals(o2))
		End Function
	End Class

End Namespace