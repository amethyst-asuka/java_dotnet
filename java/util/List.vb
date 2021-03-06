Imports System.Collections.Generic
Imports System.Runtime.CompilerServices

'
' * Copyright (c) 1997, 2014, Oracle and/or its affiliates. All rights reserved.
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
    ''' An ordered collection (also known as a <i>sequence</i>).  The user of this
    ''' interface has precise control over where in the list each element is
    ''' inserted.  The user can access elements by their integer index (position in
    ''' the list), and search for elements in the list.<p>
    ''' 
    ''' Unlike sets, lists typically allow duplicate elements.  More formally,
    ''' lists typically allow pairs of elements <tt>e1</tt> and <tt>e2</tt>
    ''' such that <tt>e1.equals(e2)</tt>, and they typically allow multiple
    ''' null elements if they allow null elements at all.  It is not inconceivable
    ''' that someone might wish to implement a list that prohibits duplicates, by
    ''' throwing runtime exceptions when the user attempts to insert them, but we
    ''' expect this usage to be rare.<p>
    ''' 
    ''' The <tt>List</tt> interface places additional stipulations, beyond those
    ''' specified in the <tt>Collection</tt> interface, on the contracts of the
    ''' <tt>iterator</tt>, <tt>add</tt>, <tt>remove</tt>, <tt>equals</tt>, and
    ''' <tt>hashCode</tt> methods.  Declarations for other inherited methods are
    ''' also included here for convenience.<p>
    ''' 
    ''' The <tt>List</tt> interface provides four methods for positional (indexed)
    ''' access to list elements.  Lists (like Java arrays) are zero based.  Note
    ''' that these operations may execute in time proportional to the index value
    ''' for some implementations (the <tt>LinkedList</tt> [Class], for
    ''' example). Thus, iterating over the elements in a list is typically
    ''' preferable to indexing through it if the caller does not know the
    ''' implementation.<p>
    ''' 
    ''' The <tt>List</tt> interface provides a special iterator, called a
    ''' <tt>ListIterator</tt>, that allows element insertion and replacement, and
    ''' bidirectional access in addition to the normal operations that the
    ''' <tt>Iterator</tt> interface provides.  A method is provided to obtain a
    ''' list iterator that starts at a specified position in the list.<p>
    ''' 
    ''' The <tt>List</tt> interface provides two methods to search for a specified
    ''' object.  From a performance standpoint, these methods should be used with
    ''' caution.  In many implementations they will perform costly linear
    ''' searches.<p>
    ''' 
    ''' The <tt>List</tt> interface provides two methods to efficiently insert and
    ''' remove multiple elements at an arbitrary point in the list.<p>
    ''' 
    ''' Note: While it is permissible for lists to contain themselves as elements,
    ''' extreme caution is advised: the <tt>equals</tt> and <tt>hashCode</tt>
    ''' methods are no longer well defined on such a list.
    ''' 
    ''' <p>Some list implementations have restrictions on the elements that
    ''' they may contain.  For example, some implementations prohibit null elements,
    ''' and some have restrictions on the types of their elements.  Attempting to
    ''' add an ineligible element throws an unchecked exception, typically
    ''' <tt>NullPointerException</tt> or <tt>ClassCastException</tt>.  Attempting
    ''' to query the presence of an ineligible element may throw an exception,
    ''' or it may simply return false; some implementations will exhibit the former
    ''' behavior and some will exhibit the latter.  More generally, attempting an
    ''' operation on an ineligible element whose completion would not result in
    ''' the insertion of an ineligible element into the list may throw an
    ''' exception or it may succeed, at the option of the implementation.
    ''' Such exceptions are marked as "optional" in the specification for this
    ''' interface.
    ''' 
    ''' <p>This interface is a member of the
    ''' <a href="{@docRoot}/../technotes/guides/collections/index.html">
    ''' Java Collections Framework</a>.
    ''' </summary>
    ''' @param <E> the type of elements in this list
    ''' 
    ''' @author  Josh Bloch
    ''' @author  Neal Gafter </param>
    ''' <seealso cref= Collection </seealso>
    ''' <seealso cref= Set </seealso>
    ''' <seealso cref= ArrayList </seealso>
    ''' <seealso cref= LinkedList </seealso>
    ''' <seealso cref= Vector </seealso>
    ''' <seealso cref= Arrays#asList(Object[]) </seealso>
    ''' <seealso cref= Collections#nCopies(int, Object) </seealso>
    ''' <seealso cref= Collections#EMPTY_LIST </seealso>
    ''' <seealso cref= AbstractList </seealso>
    ''' <seealso cref= AbstractSequentialList
    ''' @since 1.2 </seealso>

    Public Interface IList(Of E)
        Inherits Collection(Of E)

        ' Query Operations

        ''' <summary>
        ''' Returns the number of elements in this list.  If this list contains
        ''' more than <tt> java.lang.[Integer].MAX_VALUE</tt> elements, returns
        ''' <tt> java.lang.[Integer].MAX_VALUE</tt>.
        ''' </summary>
        ''' <returns> the number of elements in this list </returns>
        Function size() As Integer

        ''' <summary>
        ''' Returns <tt>true</tt> if this list contains no elements.
        ''' </summary>
        ''' <returns> <tt>true</tt> if this list contains no elements </returns>
        ReadOnly Property empty As Boolean

        ''' <summary>
        ''' Returns <tt>true</tt> if this list contains the specified element.
        ''' More formally, returns <tt>true</tt> if and only if this list contains
        ''' at least one element <tt>e</tt> such that
        ''' <tt>(o==null&nbsp;?&nbsp;e==null&nbsp;:&nbsp;o.equals(e))</tt>.
        ''' </summary>
        ''' <param name="o"> element whose presence in this list is to be tested </param>
        ''' <returns> <tt>true</tt> if this list contains the specified element </returns>
        ''' <exception cref="ClassCastException"> if the type of the specified element
        '''         is incompatible with this list
        ''' (<a href="Collection.html#optional-restrictions">optional</a>) </exception>
        ''' <exception cref="NullPointerException"> if the specified element is null and this
        '''         list does not permit null elements
        ''' (<a href="Collection.html#optional-restrictions">optional</a>) </exception>
        Function contains(  o As Object) As Boolean

        ''' <summary>
        ''' Returns an iterator over the elements in this list in proper sequence.
        ''' </summary>
        ''' <returns> an iterator over the elements in this list in proper sequence </returns>
        Function [iterator]() As [Iterator](Of E)

        ''' <summary>
        ''' Returns an array containing all of the elements in this list in proper
        ''' sequence (from first to last element).
        ''' 
        ''' <p>The returned array will be "safe" in that no references to it are
        ''' maintained by this list.  (In other words, this method must
        ''' allocate a new array even if this list is backed by an array).
        ''' The caller is thus free to modify the returned array.
        ''' 
        ''' <p>This method acts as bridge between array-based and collection-based
        ''' APIs.
        ''' </summary>
        ''' <returns> an array containing all of the elements in this list in proper
        '''         sequence </returns>
        ''' <seealso cref= Arrays#asList(Object[]) </seealso>
        Function toArray() As Object()

        ''' <summary>
        ''' Returns an array containing all of the elements in this list in
        ''' proper sequence (from first to last element); the runtime type of
        ''' the returned array is that of the specified array.  If the list fits
        ''' in the specified array, it is returned therein.  Otherwise, a new
        ''' array is allocated with the runtime type of the specified array and
        ''' the size of this list.
        ''' 
        ''' <p>If the list fits in the specified array with room to spare (i.e.,
        ''' the array has more elements than the list), the element in the array
        ''' immediately following the end of the list is set to <tt>null</tt>.
        ''' (This is useful in determining the length of the list <i>only</i> if
        ''' the caller knows that the list does not contain any null elements.)
        ''' 
        ''' <p>Like the <seealso cref="#toArray()"/> method, this method acts as bridge between
        ''' array-based and collection-based APIs.  Further, this method allows
        ''' precise control over the runtime type of the output array, and may,
        ''' under certain circumstances, be used to save allocation costs.
        ''' 
        ''' <p>Suppose <tt>x</tt> is a list known to contain only strings.
        ''' The following code can be used to dump the list into a newly
        ''' allocated array of <tt>String</tt>:
        ''' 
        ''' <pre>{@code
        '''     String[] y = x.toArray(new String[0]);
        ''' }</pre>
        ''' 
        ''' Note that <tt>toArray(new Object[0])</tt> is identical in function to
        ''' <tt>toArray()</tt>.
        ''' </summary>
        ''' <param name="a"> the array into which the elements of this list are to
        '''          be stored, if it is big enough; otherwise, a new array of the
        '''          same runtime type is allocated for this purpose. </param>
        ''' <returns> an array containing the elements of this list </returns>
        ''' <exception cref="ArrayStoreException"> if the runtime type of the specified array
        '''         is not a supertype of the runtime type of every element in
        '''         this list </exception>
        ''' <exception cref="NullPointerException"> if the specified array is null </exception>
        Function toArray(Of T)(  a As T()) As T()


        ' Modification Operations

        ''' <summary>
        ''' Appends the specified element to the end of this list (optional
        ''' operation).
        ''' 
        ''' <p>Lists that support this operation may place limitations on what
        ''' elements may be added to this list.  In particular, some
        ''' lists will refuse to add null elements, and others will impose
        ''' restrictions on the type of elements that may be added.  List
        ''' classes should clearly specify in their documentation any restrictions
        ''' on what elements may be added.
        ''' </summary>
        ''' <param name="e"> element to be appended to this list </param>
        ''' <returns> <tt>true</tt> (as specified by <seealso cref="Collection#add"/>) </returns>
        ''' <exception cref="UnsupportedOperationException"> if the <tt>add</tt> operation
        '''         is not supported by this list </exception>
        ''' <exception cref="ClassCastException"> if the class of the specified element
        '''         prevents it from being added to this list </exception>
        ''' <exception cref="NullPointerException"> if the specified element is null and this
        '''         list does not permit null elements </exception>
        ''' <exception cref="IllegalArgumentException"> if some property of this element
        '''         prevents it from being added to this list </exception>
        Function add(  e As E) As Boolean

        ''' <summary>
        ''' Removes the first occurrence of the specified element from this list,
        ''' if it is present (optional operation).  If this list does not contain
        ''' the element, it is unchanged.  More formally, removes the element with
        ''' the lowest index <tt>i</tt> such that
        ''' <tt>(o==null&nbsp;?&nbsp;get(i)==null&nbsp;:&nbsp;o.equals(get(i)))</tt>
        ''' (if such an element exists).  Returns <tt>true</tt> if this list
        ''' contained the specified element (or equivalently, if this list changed
        ''' as a result of the call).
        ''' </summary>
        ''' <param name="o"> element to be removed from this list, if present </param>
        ''' <returns> <tt>true</tt> if this list contained the specified element </returns>
        ''' <exception cref="ClassCastException"> if the type of the specified element
        '''         is incompatible with this list
        ''' (<a href="Collection.html#optional-restrictions">optional</a>) </exception>
        ''' <exception cref="NullPointerException"> if the specified element is null and this
        '''         list does not permit null elements
        ''' (<a href="Collection.html#optional-restrictions">optional</a>) </exception>
        ''' <exception cref="UnsupportedOperationException"> if the <tt>remove</tt> operation
        '''         is not supported by this list </exception>
        Function remove(  o As Object) As Boolean


        ' Bulk Modification Operations

        ''' <summary>
        ''' Returns <tt>true</tt> if this list contains all of the elements of the
        ''' specified collection.
        ''' </summary>
        ''' <param name="c"> collection to be checked for containment in this list </param>
        ''' <returns> <tt>true</tt> if this list contains all of the elements of the
        '''         specified collection </returns>
        ''' <exception cref="ClassCastException"> if the types of one or more elements
        '''         in the specified collection are incompatible with this
        '''         list
        ''' (<a href="Collection.html#optional-restrictions">optional</a>) </exception>
        ''' <exception cref="NullPointerException"> if the specified collection contains one
        '''         or more null elements and this list does not permit null
        '''         elements
        '''         (<a href="Collection.html#optional-restrictions">optional</a>),
        '''         or if the specified collection is null </exception>
        ''' <seealso cref= #contains(Object) </seealso>
        Function containsAll(Of T1)(  c As Collection(Of T1)) As Boolean

        ''' <summary>
        ''' Appends all of the elements in the specified collection to the end of
        ''' this list, in the order that they are returned by the specified
        ''' collection's iterator (optional operation).  The behavior of this
        ''' operation is undefined if the specified collection is modified while
        ''' the operation is in progress.  (Note that this will occur if the
        ''' specified collection is this list, and it's nonempty.)
        ''' </summary>
        ''' <param name="c"> collection containing elements to be added to this list </param>
        ''' <returns> <tt>true</tt> if this list changed as a result of the call </returns>
        ''' <exception cref="UnsupportedOperationException"> if the <tt>addAll</tt> operation
        '''         is not supported by this list </exception>
        ''' <exception cref="ClassCastException"> if the class of an element of the specified
        '''         collection prevents it from being added to this list </exception>
        ''' <exception cref="NullPointerException"> if the specified collection contains one
        '''         or more null elements and this list does not permit null
        '''         elements, or if the specified collection is null </exception>
        ''' <exception cref="IllegalArgumentException"> if some property of an element of the
        '''         specified collection prevents it from being added to this list </exception>
        ''' <seealso cref= #add(Object) </seealso>
        Function addAll(Of T1 As E)(  c As Collection(Of T1)) As Boolean

        ''' <summary>
        ''' Inserts all of the elements in the specified collection into this
        ''' list at the specified position (optional operation).  Shifts the
        ''' element currently at that position (if any) and any subsequent
        ''' elements to the right (increases their indices).  The new elements
        ''' will appear in this list in the order that they are returned by the
        ''' specified collection's iterator.  The behavior of this operation is
        ''' undefined if the specified collection is modified while the
        ''' operation is in progress.  (Note that this will occur if the specified
        ''' collection is this list, and it's nonempty.)
        ''' </summary>
        ''' <param name="index"> index at which to insert the first element from the
        '''              specified collection </param>
        ''' <param name="c"> collection containing elements to be added to this list </param>
        ''' <returns> <tt>true</tt> if this list changed as a result of the call </returns>
        ''' <exception cref="UnsupportedOperationException"> if the <tt>addAll</tt> operation
        '''         is not supported by this list </exception>
        ''' <exception cref="ClassCastException"> if the class of an element of the specified
        '''         collection prevents it from being added to this list </exception>
        ''' <exception cref="NullPointerException"> if the specified collection contains one
        '''         or more null elements and this list does not permit null
        '''         elements, or if the specified collection is null </exception>
        ''' <exception cref="IllegalArgumentException"> if some property of an element of the
        '''         specified collection prevents it from being added to this list </exception>
        ''' <exception cref="IndexOutOfBoundsException"> if the index is out of range
        '''         (<tt>index &lt; 0 || index &gt; size()</tt>) </exception>
        Function addAll(Of T1 As E)(  index As Integer,   c As Collection(Of T1)) As Boolean

        ''' <summary>
        ''' Removes from this list all of its elements that are contained in the
        ''' specified collection (optional operation).
        ''' </summary>
        ''' <param name="c"> collection containing elements to be removed from this list </param>
        ''' <returns> <tt>true</tt> if this list changed as a result of the call </returns>
        ''' <exception cref="UnsupportedOperationException"> if the <tt>removeAll</tt> operation
        '''         is not supported by this list </exception>
        ''' <exception cref="ClassCastException"> if the class of an element of this list
        '''         is incompatible with the specified collection
        ''' (<a href="Collection.html#optional-restrictions">optional</a>) </exception>
        ''' <exception cref="NullPointerException"> if this list contains a null element and the
        '''         specified collection does not permit null elements
        '''         (<a href="Collection.html#optional-restrictions">optional</a>),
        '''         or if the specified collection is null </exception>
        ''' <seealso cref= #remove(Object) </seealso>
        ''' <seealso cref= #contains(Object) </seealso>
        Function removeAll(Of T1)(  c As Collection(Of T1)) As Boolean

        ''' <summary>
        ''' Retains only the elements in this list that are contained in the
        ''' specified collection (optional operation).  In other words, removes
        ''' from this list all of its elements that are not contained in the
        ''' specified collection.
        ''' </summary>
        ''' <param name="c"> collection containing elements to be retained in this list </param>
        ''' <returns> <tt>true</tt> if this list changed as a result of the call </returns>
        ''' <exception cref="UnsupportedOperationException"> if the <tt>retainAll</tt> operation
        '''         is not supported by this list </exception>
        ''' <exception cref="ClassCastException"> if the class of an element of this list
        '''         is incompatible with the specified collection
        ''' (<a href="Collection.html#optional-restrictions">optional</a>) </exception>
        ''' <exception cref="NullPointerException"> if this list contains a null element and the
        '''         specified collection does not permit null elements
        '''         (<a href="Collection.html#optional-restrictions">optional</a>),
        '''         or if the specified collection is null </exception>
        ''' <seealso cref= #remove(Object) </seealso>
        ''' <seealso cref= #contains(Object) </seealso>
        Function retainAll(Of T1)(  c As Collection(Of T1)) As Boolean

        ''' <summary>
        ''' Removes all of the elements from this list (optional operation).
        ''' The list will be empty after this call returns.
        ''' </summary>
        ''' <exception cref="UnsupportedOperationException"> if the <tt>clear</tt> operation
        '''         is not supported by this list </exception>
        Sub clear()


        ' Comparison and hashing

        ''' <summary>
        ''' Compares the specified object with this list for equality.  Returns
        ''' <tt>true</tt> if and only if the specified object is also a list, both
        ''' lists have the same size, and all corresponding pairs of elements in
        ''' the two lists are <i>equal</i>.  (Two elements <tt>e1</tt> and
        ''' <tt>e2</tt> are <i>equal</i> if <tt>(e1==null ? e2==null :
        ''' e1.equals(e2))</tt>.)  In other words, two lists are defined to be
        ''' equal if they contain the same elements in the same order.  This
        ''' definition ensures that the equals method works properly across
        ''' different implementations of the <tt>List</tt> interface.
        ''' </summary>
        ''' <param name="o"> the object to be compared for equality with this list </param>
        ''' <returns> <tt>true</tt> if the specified object is equal to this list </returns>
        Function Equals(  o As Object) As Boolean

        ''' <summary>
        ''' Returns the hash code value for this list.  The hash code of a list
        ''' is defined to be the result of the following calculation:
        ''' <pre>{@code
        '''     int hashCode = 1;
        '''     for (E e : list)
        '''         hashCode = 31*hashCode + (e==null ? 0 : e.hashCode());
        ''' }</pre>
        ''' This ensures that <tt>list1.equals(list2)</tt> implies that
        ''' <tt>list1.hashCode()==list2.hashCode()</tt> for any two lists,
        ''' <tt>list1</tt> and <tt>list2</tt>, as required by the general
        ''' contract of <seealso cref="Object#hashCode"/>.
        ''' </summary>
        ''' <returns> the hash code value for this list </returns>
        ''' <seealso cref= Object#equals(Object) </seealso>
        ''' <seealso cref= #equals(Object) </seealso>
        Function GetHashCode() As Integer


        ' Positional Access Operations

        ''' <summary>
        ''' Returns the element at the specified position in this list.
        ''' </summary>
        ''' <param name="index"> index of the element to return </param>
        ''' <returns> the element at the specified position in this list </returns>
        ''' <exception cref="IndexOutOfBoundsException"> if the index is out of range
        '''         (<tt>index &lt; 0 || index &gt;= size()</tt>) </exception>
        Function [get](  index As Integer) As E

        ''' <summary>
        ''' Replaces the element at the specified position in this list with the
        ''' specified element (optional operation).
        ''' </summary>
        ''' <param name="index"> index of the element to replace </param>
        ''' <param name="element"> element to be stored at the specified position </param>
        ''' <returns> the element previously at the specified position </returns>
        ''' <exception cref="UnsupportedOperationException"> if the <tt>set</tt> operation
        '''         is not supported by this list </exception>
        ''' <exception cref="ClassCastException"> if the class of the specified element
        '''         prevents it from being added to this list </exception>
        ''' <exception cref="NullPointerException"> if the specified element is null and
        '''         this list does not permit null elements </exception>
        ''' <exception cref="IllegalArgumentException"> if some property of the specified
        '''         element prevents it from being added to this list </exception>
        ''' <exception cref="IndexOutOfBoundsException"> if the index is out of range
        '''         (<tt>index &lt; 0 || index &gt;= size()</tt>) </exception>
        Function [set](  index As Integer,   element As E) As E

        ''' <summary>
        ''' Inserts the specified element at the specified position in this list
        ''' (optional operation).  Shifts the element currently at that position
        ''' (if any) and any subsequent elements to the right (adds one to their
        ''' indices).
        ''' </summary>
        ''' <param name="index"> index at which the specified element is to be inserted </param>
        ''' <param name="element"> element to be inserted </param>
        ''' <exception cref="UnsupportedOperationException"> if the <tt>add</tt> operation
        '''         is not supported by this list </exception>
        ''' <exception cref="ClassCastException"> if the class of the specified element
        '''         prevents it from being added to this list </exception>
        ''' <exception cref="NullPointerException"> if the specified element is null and
        '''         this list does not permit null elements </exception>
        ''' <exception cref="IllegalArgumentException"> if some property of the specified
        '''         element prevents it from being added to this list </exception>
        ''' <exception cref="IndexOutOfBoundsException"> if the index is out of range
        '''         (<tt>index &lt; 0 || index &gt; size()</tt>) </exception>
        Sub add(  index As Integer,   element As E)

        ''' <summary>
        ''' Removes the element at the specified position in this list (optional
        ''' operation).  Shifts any subsequent elements to the left (subtracts one
        ''' from their indices).  Returns the element that was removed from the
        ''' list.
        ''' </summary>
        ''' <param name="index"> the index of the element to be removed </param>
        ''' <returns> the element previously at the specified position </returns>
        ''' <exception cref="UnsupportedOperationException"> if the <tt>remove</tt> operation
        '''         is not supported by this list </exception>
        ''' <exception cref="IndexOutOfBoundsException"> if the index is out of range
        '''         (<tt>index &lt; 0 || index &gt;= size()</tt>) </exception>
        Function remove(  index As Integer) As E


        ' Search Operations

        ''' <summary>
        ''' Returns the index of the first occurrence of the specified element
        ''' in this list, or -1 if this list does not contain the element.
        ''' More formally, returns the lowest index <tt>i</tt> such that
        ''' <tt>(o==null&nbsp;?&nbsp;get(i)==null&nbsp;:&nbsp;o.equals(get(i)))</tt>,
        ''' or -1 if there is no such index.
        ''' </summary>
        ''' <param name="o"> element to search for </param>
        ''' <returns> the index of the first occurrence of the specified element in
        '''         this list, or -1 if this list does not contain the element </returns>
        ''' <exception cref="ClassCastException"> if the type of the specified element
        '''         is incompatible with this list
        '''         (<a href="Collection.html#optional-restrictions">optional</a>) </exception>
        ''' <exception cref="NullPointerException"> if the specified element is null and this
        '''         list does not permit null elements
        '''         (<a href="Collection.html#optional-restrictions">optional</a>) </exception>
        Function indexOf(  o As Object) As Integer

        ''' <summary>
        ''' Returns the index of the last occurrence of the specified element
        ''' in this list, or -1 if this list does not contain the element.
        ''' More formally, returns the highest index <tt>i</tt> such that
        ''' <tt>(o==null&nbsp;?&nbsp;get(i)==null&nbsp;:&nbsp;o.equals(get(i)))</tt>,
        ''' or -1 if there is no such index.
        ''' </summary>
        ''' <param name="o"> element to search for </param>
        ''' <returns> the index of the last occurrence of the specified element in
        '''         this list, or -1 if this list does not contain the element </returns>
        ''' <exception cref="ClassCastException"> if the type of the specified element
        '''         is incompatible with this list
        '''         (<a href="Collection.html#optional-restrictions">optional</a>) </exception>
        ''' <exception cref="NullPointerException"> if the specified element is null and this
        '''         list does not permit null elements
        '''         (<a href="Collection.html#optional-restrictions">optional</a>) </exception>
        Function lastIndexOf(  o As Object) As Integer


        ' List Iterators

        ''' <summary>
        ''' Returns a list iterator over the elements in this list (in proper
        ''' sequence).
        ''' </summary>
        ''' <returns> a list iterator over the elements in this list (in proper
        '''         sequence) </returns>
        Function listIterator() As ListIterator(Of E)

        ''' <summary>
        ''' Returns a list iterator over the elements in this list (in proper
        ''' sequence), starting at the specified position in the list.
        ''' The specified index indicates the first element that would be
        ''' returned by an initial call to <seealso cref="ListIterator#next next"/>.
        ''' An initial call to <seealso cref="ListIterator#previous previous"/> would
        ''' return the element with the specified index minus one.
        ''' </summary>
        ''' <param name="index"> index of the first element to be returned from the
        '''        list iterator (by a call to <seealso cref="ListIterator#next next"/>) </param>
        ''' <returns> a list iterator over the elements in this list (in proper
        '''         sequence), starting at the specified position in the list </returns>
        ''' <exception cref="IndexOutOfBoundsException"> if the index is out of range
        '''         ({@code index < 0 || index > size()}) </exception>
        Function listIterator(  index As Integer) As ListIterator(Of E)

        ' View

        ''' <summary>
        ''' Returns a view of the portion of this list between the specified
        ''' <tt>fromIndex</tt>, inclusive, and <tt>toIndex</tt>, exclusive.  (If
        ''' <tt>fromIndex</tt> and <tt>toIndex</tt> are equal, the returned list is
        ''' empty.)  The returned list is backed by this list, so non-structural
        ''' changes in the returned list are reflected in this list, and vice-versa.
        ''' The returned list supports all of the optional list operations supported
        ''' by this list.<p>
        ''' 
        ''' This method eliminates the need for explicit range operations (of
        ''' the sort that commonly exist for arrays).  Any operation that expects
        ''' a list can be used as a range operation by passing a subList view
        ''' instead of a whole list.  For example, the following idiom
        ''' removes a range of elements from a list:
        ''' <pre>{@code
        '''      list.subList(from, to).clear();
        ''' }</pre>
        ''' Similar idioms may be constructed for <tt>indexOf</tt> and
        ''' <tt>lastIndexOf</tt>, and all of the algorithms in the
        ''' <tt>Collections</tt> class can be applied to a subList.<p>
        ''' 
        ''' The semantics of the list returned by this method become undefined if
        ''' the backing list (i.e., this list) is <i>structurally modified</i> in
        ''' any way other than via the returned list.  (Structural modifications are
        ''' those that change the size of this list, or otherwise perturb it in such
        ''' a fashion that iterations in progress may yield incorrect results.)
        ''' </summary>
        ''' <param name="fromIndex"> low endpoint (inclusive) of the subList </param>
        ''' <param name="toIndex"> high endpoint (exclusive) of the subList </param>
        ''' <returns> a view of the specified range within this list </returns>
        ''' <exception cref="IndexOutOfBoundsException"> for an illegal endpoint index value
        '''         (<tt>fromIndex &lt; 0 || toIndex &gt; size ||
        '''         fromIndex &gt; toIndex</tt>) </exception>
        Function subList(  fromIndex As Integer,   toIndex As Integer) As List(Of E)

    End Interface

    Public Module IList

        ''' <summary>
        ''' Creates a <seealso cref="Spliterator"/> over the elements in this list.
        ''' 
        ''' <p>The {@code Spliterator} reports <seealso cref="Spliterator#SIZED"/> and
        ''' <seealso cref="Spliterator#ORDERED"/>.  Implementations should document the
        ''' reporting of additional characteristic values.
        ''' 
        ''' @implSpec
        ''' The default implementation creates a
        ''' <em><a href="Spliterator.html#binding">late-binding</a></em> spliterator
        ''' from the list's {@code Iterator}.  The spliterator inherits the
        ''' <em>fail-fast</em> properties of the list's iterator.
        ''' 
        ''' @implNote
        ''' The created {@code Spliterator} additionally reports
        ''' <seealso cref="Spliterator#SUBSIZED"/>.
        ''' </summary>
        ''' <returns> a {@code Spliterator} over the elements in this list
        ''' @since 1.8 </returns>
        <Extension> Public Function spliterator(Of T)(list As IList(Of T)) As Spliterator(Of T)
            'JAVA TO VB CONVERTER TODO TASK: The following line could not be converted:
            Return Spliterators.spliterator(list, spliterator.ORDERED)
        End Function

        ''' <summary>
        ''' Replaces each element of this list with the result of applying the
        ''' operator to that element.  Errors or runtime exceptions thrown by
        ''' the operator are relayed to the caller.
        ''' 
        ''' @implSpec
        ''' The default implementation is equivalent to, for this {@code list}:
        ''' <pre>{@code
        '''     final ListIterator<E> li = list.listIterator();
        '''     while (li.hasNext()) {
        '''         li.set(operator.apply(li.next()));
        '''     }
        ''' }</pre>
        ''' 
        ''' If the list's list-iterator does not support the {@code set} operation
        ''' then an {@code UnsupportedOperationException} will be thrown when
        ''' replacing the first element.
        ''' </summary>
        ''' <param name="operator"> the operator to apply to each element </param>
        ''' <exception cref="UnsupportedOperationException"> if this list is unmodifiable.
        '''         Implementations may throw this exception if an element
        '''         cannot be replaced or if, in general, modification is not
        '''         supported </exception>
        ''' <exception cref="NullPointerException"> if the specified operator is null or
        '''         if the operator result is a null value and this list does
        '''         not permit null elements
        '''         (<a href="Collection.html#optional-restrictions">optional</a>)
        ''' @since 1.8 </exception>
        <Extension> Public Sub replaceAll(Of E)(list As IList(Of E),   [operator] As java.util.function.UnaryOperator(Of E))
            'JAVA TO VB CONVERTER TODO TASK: The following line could not be converted:
            Objects.requireNonNull(Of E)([operator])
        End Sub

        '	final ListIterator(Of E) li = Me.listIterator();
        ' Sub [New](li.hasNext()    As )
        ' Sub [New](operator.apply(li.Next())    As )

        ''' <summary>
        ''' Sorts this list according to the order induced by the specified
        ''' <seealso cref="Comparator"/>.
        ''' 
        ''' <p>All elements in this list must be <i>mutually comparable</i> using the
        ''' specified comparator (that is, {@code c.compare(e1, e2)} must not throw
        ''' a {@code ClassCastException} for any elements {@code e1} and {@code e2}
        ''' in the list).
        ''' 
        ''' <p>If the specified comparator is {@code null} then all elements in this
        ''' list must implement the <seealso cref="Comparable"/> interface and the elements'
        ''' <seealso cref="Comparable natural ordering"/> should be used.
        ''' 
        ''' <p>This list must be modifiable, but need not be resizable.
        ''' 
        ''' @implSpec
        ''' The default implementation obtains an array containing all elements in
        ''' this list, sorts the array, and iterates over this list resetting each
        ''' element from the corresponding position in the array. (This avoids the
        ''' n<sup>2</sup> log(n) performance that would result from attempting
        ''' to sort a linked list in place.)
        ''' 
        ''' @implNote
        ''' This implementation is a stable, adaptive, iterative mergesort that
        ''' requires far fewer than n lg(n) comparisons when the input array is
        ''' partially sorted, while offering the performance of a traditional
        ''' mergesort when the input array is randomly ordered.  If the input array
        ''' is nearly sorted, the implementation requires approximately n
        ''' comparisons.  Temporary storage requirements vary from a small constant
        ''' for nearly sorted input arrays to n/2 object references for randomly
        ''' ordered input arrays.
        ''' 
        ''' <p>The implementation takes equal advantage of ascending and
        ''' descending order in its input array, and can take advantage of
        ''' ascending and descending order in different parts of the same
        ''' input array.  It is well-suited to merging two or more sorted arrays:
        ''' simply concatenate the arrays and sort the resulting array.
        ''' 
        ''' <p>The implementation was adapted from Tim Peters's list sort for Python
        ''' (<a href="http://svn.python.org/projects/python/trunk/Objects/listsort.txt">
        ''' TimSort</a>).  It uses techniques from Peter McIlroy's "Optimistic
        ''' Sorting and Information Theoretic Complexity", in Proceedings of the
        ''' Fourth Annual ACM-SIAM Symposium on Discrete Algorithms, pp 467-474,
        ''' January 1993.
        ''' </summary>
        ''' <param name="c"> the {@code Comparator} used to compare list elements.
        '''          A {@code null} value indicates that the elements'
        '''          <seealso cref="Comparable natural ordering"/> should be used </param>
        ''' <exception cref="ClassCastException"> if the list contains elements that are not
        '''         <i>mutually comparable</i> using the specified comparator </exception>
        ''' <exception cref="UnsupportedOperationException"> if the list's list-iterator does
        '''         not support the {@code set} operation </exception>
        ''' <exception cref="IllegalArgumentException">
        '''         (<a href="Collection.html#optional-restrictions">optional</a>)
        '''         if the comparator is found to violate the <seealso cref="Comparator"/>
        '''         contract
        ''' @since 1.8 </exception>
        <Extension> Public Sub sort(Of T1)(  c As Comparator(Of T1))
            'JAVA TO VB CONVERTER TODO TASK: Interfaces cannot contain fields in .NET:
            '			Object[] a = Me.toArray();
            'JAVA TO VB CONVERTER TODO TASK: The following line could not be converted:
            Arrays.sort(A, (Comparator) c)
        End Sub

        '			ListIterator(Of E) i = Me.listIterator();
        ' Sub [New](Object e :   a As )
        ' Sub [New]()
        ' Sub [New]((E)   e As )
    End Module
End Namespace