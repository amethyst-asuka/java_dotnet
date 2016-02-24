Imports System

'
' * Copyright (c) 2000, 2013, Oracle and/or its affiliates. All rights reserved.
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

Namespace java.util

	''' <summary>
	''' <p>Hash table and linked list implementation of the <tt>Set</tt> interface,
	''' with predictable iteration order.  This implementation differs from
	''' <tt>HashSet</tt> in that it maintains a doubly-linked list running through
	''' all of its entries.  This linked list defines the iteration ordering,
	''' which is the order in which elements were inserted into the set
	''' (<i>insertion-order</i>).  Note that insertion order is <i>not</i> affected
	''' if an element is <i>re-inserted</i> into the set.  (An element <tt>e</tt>
	''' is reinserted into a set <tt>s</tt> if <tt>s.add(e)</tt> is invoked when
	''' <tt>s.contains(e)</tt> would return <tt>true</tt> immediately prior to
	''' the invocation.)
	''' 
	''' <p>This implementation spares its clients from the unspecified, generally
	''' chaotic ordering provided by <seealso cref="HashSet"/>, without incurring the
	''' increased cost associated with <seealso cref="TreeSet"/>.  It can be used to
	''' produce a copy of a set that has the same order as the original, regardless
	''' of the original set's implementation:
	''' <pre>
	'''     void foo(Set s) {
	'''         Set copy = new LinkedHashSet(s);
	'''         ...
	'''     }
	''' </pre>
	''' This technique is particularly useful if a module takes a set on input,
	''' copies it, and later returns results whose order is determined by that of
	''' the copy.  (Clients generally appreciate having things returned in the same
	''' order they were presented.)
	''' 
	''' <p>This class provides all of the optional <tt>Set</tt> operations, and
	''' permits null elements.  Like <tt>HashSet</tt>, it provides constant-time
	''' performance for the basic operations (<tt>add</tt>, <tt>contains</tt> and
	''' <tt>remove</tt>), assuming the hash function disperses elements
	''' properly among the buckets.  Performance is likely to be just slightly
	''' below that of <tt>HashSet</tt>, due to the added expense of maintaining the
	''' linked list, with one exception: Iteration over a <tt>LinkedHashSet</tt>
	''' requires time proportional to the <i>size</i> of the set, regardless of
	''' its capacity.  Iteration over a <tt>HashSet</tt> is likely to be more
	''' expensive, requiring time proportional to its <i>capacity</i>.
	''' 
	''' <p>A linked hash set has two parameters that affect its performance:
	''' <i>initial capacity</i> and <i>load factor</i>.  They are defined precisely
	''' as for <tt>HashSet</tt>.  Note, however, that the penalty for choosing an
	''' excessively high value for initial capacity is less severe for this class
	''' than for <tt>HashSet</tt>, as iteration times for this class are unaffected
	''' by capacity.
	''' 
	''' <p><strong>Note that this implementation is not synchronized.</strong>
	''' If multiple threads access a linked hash set concurrently, and at least
	''' one of the threads modifies the set, it <em>must</em> be synchronized
	''' externally.  This is typically accomplished by synchronizing on some
	''' object that naturally encapsulates the set.
	''' 
	''' If no such object exists, the set should be "wrapped" using the
	''' <seealso cref="Collections#synchronizedSet Collections.synchronizedSet"/>
	''' method.  This is best done at creation time, to prevent accidental
	''' unsynchronized access to the set: <pre>
	'''   Set s = Collections.synchronizedSet(new LinkedHashSet(...));</pre>
	''' 
	''' <p>The iterators returned by this class's <tt>iterator</tt> method are
	''' <em>fail-fast</em>: if the set is modified at any time after the iterator
	''' is created, in any way except through the iterator's own <tt>remove</tt>
	''' method, the iterator will throw a <seealso cref="ConcurrentModificationException"/>.
	''' Thus, in the face of concurrent modification, the iterator fails quickly
	''' and cleanly, rather than risking arbitrary, non-deterministic behavior at
	''' an undetermined time in the future.
	''' 
	''' <p>Note that the fail-fast behavior of an iterator cannot be guaranteed
	''' as it is, generally speaking, impossible to make any hard guarantees in the
	''' presence of unsynchronized concurrent modification.  Fail-fast iterators
	''' throw <tt>ConcurrentModificationException</tt> on a best-effort basis.
	''' Therefore, it would be wrong to write a program that depended on this
	''' exception for its correctness:   <i>the fail-fast behavior of iterators
	''' should be used only to detect bugs.</i>
	''' 
	''' <p>This class is a member of the
	''' <a href="{@docRoot}/../technotes/guides/collections/index.html">
	''' Java Collections Framework</a>.
	''' </summary>
	''' @param <E> the type of elements maintained by this set
	''' 
	''' @author  Josh Bloch </param>
	''' <seealso cref=     Object#hashCode() </seealso>
	''' <seealso cref=     Collection </seealso>
	''' <seealso cref=     Set </seealso>
	''' <seealso cref=     HashSet </seealso>
	''' <seealso cref=     TreeSet </seealso>
	''' <seealso cref=     Hashtable
	''' @since   1.4 </seealso>

	<Serializable> _
	Public Class LinkedHashSet(Of E)
		Inherits HashSet(Of E)
		Implements Set(Of E), Cloneable

		Private Const serialVersionUID As Long = -2851667679971038690L

		''' <summary>
		''' Constructs a new, empty linked hash set with the specified initial
		''' capacity and load factor.
		''' </summary>
		''' <param name="initialCapacity"> the initial capacity of the linked hash set </param>
		''' <param name="loadFactor">      the load factor of the linked hash set </param>
		''' <exception cref="IllegalArgumentException">  if the initial capacity is less
		'''               than zero, or if the load factor is nonpositive </exception>
		Public Sub New(ByVal initialCapacity As Integer, ByVal loadFactor As Single)
			MyBase.New(initialCapacity, loadFactor, True)
		End Sub

		''' <summary>
		''' Constructs a new, empty linked hash set with the specified initial
		''' capacity and the default load factor (0.75).
		''' </summary>
		''' <param name="initialCapacity">   the initial capacity of the LinkedHashSet </param>
		''' <exception cref="IllegalArgumentException"> if the initial capacity is less
		'''              than zero </exception>
		Public Sub New(ByVal initialCapacity As Integer)
			MyBase.New(initialCapacity,.75f, True)
		End Sub

		''' <summary>
		''' Constructs a new, empty linked hash set with the default initial
		''' capacity (16) and load factor (0.75).
		''' </summary>
		Public Sub New()
			MyBase.New(16,.75f, True)
		End Sub

		''' <summary>
		''' Constructs a new linked hash set with the same elements as the
		''' specified collection.  The linked hash set is created with an initial
		''' capacity sufficient to hold the elements in the specified collection
		''' and the default load factor (0.75).
		''' </summary>
		''' <param name="c">  the collection whose elements are to be placed into
		'''           this set </param>
		''' <exception cref="NullPointerException"> if the specified collection is null </exception>
		Public Sub New(Of T1 As E)(ByVal c As Collection(Of T1))
			MyBase.New(Math.Max(2*c.size(), 11),.75f, True)
			addAll(c)
		End Sub

		''' <summary>
		''' Creates a <em><a href="Spliterator.html#binding">late-binding</a></em>
		''' and <em>fail-fast</em> {@code Spliterator} over the elements in this set.
		''' 
		''' <p>The {@code Spliterator} reports <seealso cref="Spliterator#SIZED"/>,
		''' <seealso cref="Spliterator#DISTINCT"/>, and {@code ORDERED}.  Implementations
		''' should document the reporting of additional characteristic values.
		''' 
		''' @implNote
		''' The implementation creates a
		''' <em><a href="Spliterator.html#binding">late-binding</a></em> spliterator
		''' from the set's {@code Iterator}.  The spliterator inherits the
		''' <em>fail-fast</em> properties of the set's iterator.
		''' The created {@code Spliterator} additionally reports
		''' <seealso cref="Spliterator#SUBSIZED"/>.
		''' </summary>
		''' <returns> a {@code Spliterator} over the elements in this set
		''' @since 1.8 </returns>
		Public Overrides Function spliterator() As Spliterator(Of E) Implements Set(Of E).spliterator
			Return Spliterators.spliterator(Me, Spliterator.DISTINCT Or Spliterator.ORDERED)
		End Function
	End Class

End Namespace