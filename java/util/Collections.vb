Imports System
Imports System.Diagnostics
Imports System.Collections
Imports System.Collections.Generic

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
	''' This class consists exclusively of static methods that operate on or return
	''' collections.  It contains polymorphic algorithms that operate on
	''' collections, "wrappers", which return a new collection backed by a
	''' specified collection, and a few other odds and ends.
	''' 
	''' <p>The methods of this class all throw a <tt>NullPointerException</tt>
	''' if the collections or class objects provided to them are null.
	''' 
	''' <p>The documentation for the polymorphic algorithms contained in this class
	''' generally includes a brief description of the <i>implementation</i>.  Such
	''' descriptions should be regarded as <i>implementation notes</i>, rather than
	''' parts of the <i>specification</i>.  Implementors should feel free to
	''' substitute other algorithms, so long as the specification itself is adhered
	''' to.  (For example, the algorithm used by <tt>sort</tt> does not have to be
	''' a mergesort, but it does have to be <i>stable</i>.)
	''' 
	''' <p>The "destructive" algorithms contained in this class, that is, the
	''' algorithms that modify the collection on which they operate, are specified
	''' to throw <tt>UnsupportedOperationException</tt> if the collection does not
	''' support the appropriate mutation primitive(s), such as the <tt>set</tt>
	''' method.  These algorithms may, but are not required to, throw this
	''' exception if an invocation would have no effect on the collection.  For
	''' example, invoking the <tt>sort</tt> method on an unmodifiable list that is
	''' already sorted may or may not throw <tt>UnsupportedOperationException</tt>.
	''' 
	''' <p>This class is a member of the
	''' <a href="{@docRoot}/../technotes/guides/collections/index.html">
	''' Java Collections Framework</a>.
	''' 
	''' @author  Josh Bloch
	''' @author  Neal Gafter </summary>
	''' <seealso cref=     Collection </seealso>
	''' <seealso cref=     Set </seealso>
	''' <seealso cref=     List </seealso>
	''' <seealso cref=     Map
	''' @since   1.2 </seealso>

	Public Class Collections
		' Suppresses default constructor, ensuring non-instantiability.
		Private Sub New()
		End Sub

		' Algorithms

	'    
	'     * Tuning parameters for algorithms - Many of the List algorithms have
	'     * two implementations, one of which is appropriate for RandomAccess
	'     * lists, the other for "sequential."  Often, the random access variant
	'     * yields better performance on small sequential access lists.  The
	'     * tuning parameters below determine the cutoff point for what constitutes
	'     * a "small" sequential access list for each algorithm.  The values below
	'     * were empirically determined to work well for LinkedList. Hopefully
	'     * they should be reasonable for other sequential access List
	'     * implementations.  Those doing performance work on this code would
	'     * do well to validate the values of these parameters from time to time.
	'     * (The first word of each tuning parameter name is the algorithm to which
	'     * it applies.)
	'     
		Private Const BINARYSEARCH_THRESHOLD As Integer = 5000
		Private Const REVERSE_THRESHOLD As Integer = 18
		Private Const SHUFFLE_THRESHOLD As Integer = 5
		Private Const FILL_THRESHOLD As Integer = 25
		Private Const ROTATE_THRESHOLD As Integer = 100
		Private Const COPY_THRESHOLD As Integer = 10
		Private Const REPLACEALL_THRESHOLD As Integer = 11
		Private Const INDEXOFSUBLIST_THRESHOLD As Integer = 35

		''' <summary>
		''' Sorts the specified list into ascending order, according to the
		''' <seealso cref="Comparable natural ordering"/> of its elements.
		''' All elements in the list must implement the <seealso cref="Comparable"/>
		''' interface.  Furthermore, all elements in the list must be
		''' <i>mutually comparable</i> (that is, {@code e1.compareTo(e2)}
		''' must not throw a {@code ClassCastException} for any elements
		''' {@code e1} and {@code e2} in the list).
		''' 
		''' <p>This sort is guaranteed to be <i>stable</i>:  equal elements will
		''' not be reordered as a result of the sort.
		''' 
		''' <p>The specified list must be modifiable, but need not be resizable.
		''' 
		''' @implNote
		''' This implementation defers to the <seealso cref="List#sort(Comparator)"/>
		''' method using the specified list and a {@code null} comparator.
		''' </summary>
		''' @param  <T> the class of the objects in the list </param>
		''' <param name="list"> the list to be sorted. </param>
		''' <exception cref="ClassCastException"> if the list contains elements that are not
		'''         <i>mutually comparable</i> (for example, strings and integers). </exception>
		''' <exception cref="UnsupportedOperationException"> if the specified list's
		'''         list-iterator does not support the {@code set} operation. </exception>
		''' <exception cref="IllegalArgumentException"> (optional) if the implementation
		'''         detects that the natural ordering of the list elements is
		'''         found to violate the <seealso cref="Comparable"/> contract </exception>
		''' <seealso cref= List#sort(Comparator) </seealso>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'JAVA TO VB CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
		Public Shared Sub sort(Of T As Comparable(Of ?))(ByVal list As List(Of T))
			list.sort(Nothing)
		End Sub

		''' <summary>
		''' Sorts the specified list according to the order induced by the
		''' specified comparator.  All elements in the list must be <i>mutually
		''' comparable</i> using the specified comparator (that is,
		''' {@code c.compare(e1, e2)} must not throw a {@code ClassCastException}
		''' for any elements {@code e1} and {@code e2} in the list).
		''' 
		''' <p>This sort is guaranteed to be <i>stable</i>:  equal elements will
		''' not be reordered as a result of the sort.
		''' 
		''' <p>The specified list must be modifiable, but need not be resizable.
		''' 
		''' @implNote
		''' This implementation defers to the <seealso cref="List#sort(Comparator)"/>
		''' method using the specified list and comparator.
		''' </summary>
		''' @param  <T> the class of the objects in the list </param>
		''' <param name="list"> the list to be sorted. </param>
		''' <param name="c"> the comparator to determine the order of the list.  A
		'''        {@code null} value indicates that the elements' <i>natural
		'''        ordering</i> should be used. </param>
		''' <exception cref="ClassCastException"> if the list contains elements that are not
		'''         <i>mutually comparable</i> using the specified comparator. </exception>
		''' <exception cref="UnsupportedOperationException"> if the specified list's
		'''         list-iterator does not support the {@code set} operation. </exception>
		''' <exception cref="IllegalArgumentException"> (optional) if the comparator is
		'''         found to violate the <seealso cref="Comparator"/> contract </exception>
		''' <seealso cref= List#sort(Comparator) </seealso>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'JAVA TO VB CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
		Public Shared Sub sort(Of T, T1)(ByVal list As List(Of T), ByVal c As Comparator(Of T1))
			list.sort(c)
		End Sub


		''' <summary>
		''' Searches the specified list for the specified object using the binary
		''' search algorithm.  The list must be sorted into ascending order
		''' according to the <seealso cref="Comparable natural ordering"/> of its
		''' elements (as by the <seealso cref="#sort(List)"/> method) prior to making this
		''' call.  If it is not sorted, the results are undefined.  If the list
		''' contains multiple elements equal to the specified object, there is no
		''' guarantee which one will be found.
		''' 
		''' <p>This method runs in log(n) time for a "random access" list (which
		''' provides near-constant-time positional access).  If the specified list
		''' does not implement the <seealso cref="RandomAccess"/> interface and is large,
		''' this method will do an iterator-based binary search that performs
		''' O(n) link traversals and O(log n) element comparisons.
		''' </summary>
		''' @param  <T> the class of the objects in the list </param>
		''' <param name="list"> the list to be searched. </param>
		''' <param name="key"> the key to be searched for. </param>
		''' <returns> the index of the search key, if it is contained in the list;
		'''         otherwise, <tt>(-(<i>insertion point</i>) - 1)</tt>.  The
		'''         <i>insertion point</i> is defined as the point at which the
		'''         key would be inserted into the list: the index of the first
		'''         element greater than the key, or <tt>list.size()</tt> if all
		'''         elements in the list are less than the specified key.  Note
		'''         that this guarantees that the return value will be &gt;= 0 if
		'''         and only if the key is found. </returns>
		''' <exception cref="ClassCastException"> if the list contains elements that are not
		'''         <i>mutually comparable</i> (for example, strings and
		'''         integers), or the search key is not mutually comparable
		'''         with the elements of the list. </exception>
'JAVA TO VB CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
		Public Shared Function binarySearch(Of T, T1 As Comparable(Of ?)(ByVal list As List(Of T1), ByVal key As T) As Integer
			If TypeOf list Is RandomAccess OrElse list.size()<BINARYSEARCH_THRESHOLD Then
				Return Collections.indexedBinarySearch(list, key)
			Else
				Return Collections.iteratorBinarySearch(list, key)
			End If
		End Function

'JAVA TO VB CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
		Private Shared Function indexedBinarySearch(Of T, T1 As Comparable(Of ?)(ByVal list As List(Of T1), ByVal key As T) As Integer
			Dim low As Integer = 0
			Dim high As Integer = list.size()-1

			Do While low <= high
				Dim mid As Integer = CInt(CUInt((low + high)) >> 1)
'JAVA TO VB CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
				Dim midVal As Comparable(Of ?) = list.get(mid)
				Dim cmp As Integer = midVal.CompareTo(key)

				If cmp < 0 Then
					low = mid + 1
				ElseIf cmp > 0 Then
					high = mid - 1
				Else
					Return mid ' key found
				End If
			Loop
			Return -(low + 1) ' key not found
		End Function

'JAVA TO VB CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
		Private Shared Function iteratorBinarySearch(Of T, T1 As Comparable(Of ?)(ByVal list As List(Of T1), ByVal key As T) As Integer
			Dim low As Integer = 0
			Dim high As Integer = list.size()-1
'JAVA TO VB CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
			Dim i As ListIterator(Of ? As Comparable(Of ?)) = list.GetEnumerator()

			Do While low <= high
				Dim mid As Integer = CInt(CUInt((low + high)) >> 1)
'JAVA TO VB CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
				Dim midVal As Comparable(Of ?) = [get](i, mid)
				Dim cmp As Integer = midVal.CompareTo(key)

				If cmp < 0 Then
					low = mid + 1
				ElseIf cmp > 0 Then
					high = mid - 1
				Else
					Return mid ' key found
				End If
			Loop
			Return -(low + 1) ' key not found
		End Function

		''' <summary>
		''' Gets the ith element from the given list by repositioning the specified
		''' list listIterator.
		''' </summary>
		Private Shared Function [get](Of T, T1 As T)(ByVal i As ListIterator(Of T1), ByVal index As Integer) As T
			Dim obj As T = Nothing
			Dim pos As Integer = i.nextIndex()
			If pos <= index Then
				Dim tempVar As Boolean
				Do
					obj = i.next()
					tempVar = pos < index
					pos += 1
				Loop While tempVar
			Else
				Do
					obj = i.previous()
					pos -= 1
				Loop While pos > index
			End If
			Return obj
		End Function

		''' <summary>
		''' Searches the specified list for the specified object using the binary
		''' search algorithm.  The list must be sorted into ascending order
		''' according to the specified comparator (as by the
		''' <seealso cref="#sort(List, Comparator) sort(List, Comparator)"/>
		''' method), prior to making this call.  If it is
		''' not sorted, the results are undefined.  If the list contains multiple
		''' elements equal to the specified object, there is no guarantee which one
		''' will be found.
		''' 
		''' <p>This method runs in log(n) time for a "random access" list (which
		''' provides near-constant-time positional access).  If the specified list
		''' does not implement the <seealso cref="RandomAccess"/> interface and is large,
		''' this method will do an iterator-based binary search that performs
		''' O(n) link traversals and O(log n) element comparisons.
		''' </summary>
		''' @param  <T> the class of the objects in the list </param>
		''' <param name="list"> the list to be searched. </param>
		''' <param name="key"> the key to be searched for. </param>
		''' <param name="c"> the comparator by which the list is ordered.
		'''         A <tt>null</tt> value indicates that the elements'
		'''         <seealso cref="Comparable natural ordering"/> should be used. </param>
		''' <returns> the index of the search key, if it is contained in the list;
		'''         otherwise, <tt>(-(<i>insertion point</i>) - 1)</tt>.  The
		'''         <i>insertion point</i> is defined as the point at which the
		'''         key would be inserted into the list: the index of the first
		'''         element greater than the key, or <tt>list.size()</tt> if all
		'''         elements in the list are less than the specified key.  Note
		'''         that this guarantees that the return value will be &gt;= 0 if
		'''         and only if the key is found. </returns>
		''' <exception cref="ClassCastException"> if the list contains elements that are not
		'''         <i>mutually comparable</i> using the specified comparator,
		'''         or the search key is not mutually comparable with the
		'''         elements of the list using this comparator. </exception>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'JAVA TO VB CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
		Public Shared Function binarySearch(Of T, T1 As T, T2)(ByVal list As List(Of T1), ByVal key As T, ByVal c As Comparator(Of T2)) As Integer
			If c Is Nothing Then Return binarySearch(CType(list, List(Of ? As Comparable(Of ?))), key)

			If TypeOf list Is RandomAccess OrElse list.size()<BINARYSEARCH_THRESHOLD Then
				Return Collections.indexedBinarySearch(list, key, c)
			Else
				Return Collections.iteratorBinarySearch(list, key, c)
			End If
		End Function

'JAVA TO VB CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
		Private Shared Function indexedBinarySearch(Of T, T1 As T, T2)(ByVal l As List(Of T1), ByVal key As T, ByVal c As Comparator(Of T2)) As Integer
			Dim low As Integer = 0
			Dim high As Integer = l.size()-1

			Do While low <= high
				Dim mid As Integer = CInt(CUInt((low + high)) >> 1)
				Dim midVal As T = l.get(mid)
				Dim cmp As Integer = c.Compare(midVal, key)

				If cmp < 0 Then
					low = mid + 1
				ElseIf cmp > 0 Then
					high = mid - 1
				Else
					Return mid ' key found
				End If
			Loop
			Return -(low + 1) ' key not found
		End Function

'JAVA TO VB CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
		Private Shared Function iteratorBinarySearch(Of T, T1 As T, T2)(ByVal l As List(Of T1), ByVal key As T, ByVal c As Comparator(Of T2)) As Integer
			Dim low As Integer = 0
			Dim high As Integer = l.size()-1
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
			Dim i As ListIterator(Of ? As T) = l.GetEnumerator()

			Do While low <= high
				Dim mid As Integer = CInt(CUInt((low + high)) >> 1)
				Dim midVal As T = [get](i, mid)
				Dim cmp As Integer = c.Compare(midVal, key)

				If cmp < 0 Then
					low = mid + 1
				ElseIf cmp > 0 Then
					high = mid - 1
				Else
					Return mid ' key found
				End If
			Loop
			Return -(low + 1) ' key not found
		End Function

		''' <summary>
		''' Reverses the order of the elements in the specified list.<p>
		''' 
		''' This method runs in linear time.
		''' </summary>
		''' <param name="list"> the list whose elements are to be reversed. </param>
		''' <exception cref="UnsupportedOperationException"> if the specified list or
		'''         its list-iterator does not support the <tt>set</tt> operation. </exception>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
		Public Shared Sub reverse(Of T1)(ByVal list As List(Of T1))
			Dim size As Integer = list.size()
			If size < REVERSE_THRESHOLD OrElse TypeOf list Is RandomAccess Then
				Dim i As Integer=0
				Dim mid As Integer=size>>1
				Dim j As Integer=size-1
				Do While i<mid
					swap(list, i, j)
					i += 1
					j -= 1
				Loop
			Else
				' instead of using a raw type here, it's possible to capture
				' the wildcard but it will require a call to a supplementary
				' private method
				Dim fwd As ListIterator = list.GetEnumerator()
				Dim rev As ListIterator = list.listIterator(size)
				Dim i As Integer=0
				Dim mid As Integer=list.size()>>1
				Do While i<mid
					Dim tmp As Object = fwd.next()
					fwd.set(rev.previous())
					rev.set(tmp)
					i += 1
				Loop
			End If
		End Sub

		''' <summary>
		''' Randomly permutes the specified list using a default source of
		''' randomness.  All permutations occur with approximately equal
		''' likelihood.
		''' 
		''' <p>The hedge "approximately" is used in the foregoing description because
		''' default source of randomness is only approximately an unbiased source
		''' of independently chosen bits. If it were a perfect source of randomly
		''' chosen bits, then the algorithm would choose permutations with perfect
		''' uniformity.
		''' 
		''' <p>This implementation traverses the list backwards, from the last
		''' element up to the second, repeatedly swapping a randomly selected element
		''' into the "current position".  Elements are randomly selected from the
		''' portion of the list that runs from the first element to the current
		''' position, inclusive.
		''' 
		''' <p>This method runs in linear time.  If the specified list does not
		''' implement the <seealso cref="RandomAccess"/> interface and is large, this
		''' implementation dumps the specified list into an array before shuffling
		''' it, and dumps the shuffled array back into the list.  This avoids the
		''' quadratic behavior that would result from shuffling a "sequential
		''' access" list in place.
		''' </summary>
		''' <param name="list"> the list to be shuffled. </param>
		''' <exception cref="UnsupportedOperationException"> if the specified list or
		'''         its list-iterator does not support the <tt>set</tt> operation. </exception>
		Public Shared Sub shuffle(Of T1)(ByVal list As List(Of T1))
			Dim rnd As Random = r
			If rnd Is Nothing Then
					rnd = New Random
					r = rnd
			End If
			shuffle(list, rnd)
		End Sub

		Private Shared r As Random

		''' <summary>
		''' Randomly permute the specified list using the specified source of
		''' randomness.  All permutations occur with equal likelihood
		''' assuming that the source of randomness is fair.<p>
		''' 
		''' This implementation traverses the list backwards, from the last element
		''' up to the second, repeatedly swapping a randomly selected element into
		''' the "current position".  Elements are randomly selected from the
		''' portion of the list that runs from the first element to the current
		''' position, inclusive.<p>
		''' 
		''' This method runs in linear time.  If the specified list does not
		''' implement the <seealso cref="RandomAccess"/> interface and is large, this
		''' implementation dumps the specified list into an array before shuffling
		''' it, and dumps the shuffled array back into the list.  This avoids the
		''' quadratic behavior that would result from shuffling a "sequential
		''' access" list in place.
		''' </summary>
		''' <param name="list"> the list to be shuffled. </param>
		''' <param name="rnd"> the source of randomness to use to shuffle the list. </param>
		''' <exception cref="UnsupportedOperationException"> if the specified list or its
		'''         list-iterator does not support the <tt>set</tt> operation. </exception>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
		Public Shared Sub shuffle(Of T1)(ByVal list As List(Of T1), ByVal rnd As Random)
			Dim size As Integer = list.size()
			If size < SHUFFLE_THRESHOLD OrElse TypeOf list Is RandomAccess Then
				For i As Integer = size To 2 Step -1
					swap(list, i-1, rnd.Next(i))
				Next i
			Else
				Dim arr As Object() = list.ToArray()

				' Shuffle array
				For i As Integer = size To 2 Step -1
					swap(arr, i-1, rnd.Next(i))
				Next i

				' Dump array back into list
				' instead of using a raw type here, it's possible to capture
				' the wildcard but it will require a call to a supplementary
				' private method
				Dim it As ListIterator = list.GetEnumerator()
				For i As Integer = 0 To arr.Length - 1
					it.next()
					it.set(arr(i))
				Next i
			End If
		End Sub

		''' <summary>
		''' Swaps the elements at the specified positions in the specified list.
		''' (If the specified positions are equal, invoking this method leaves
		''' the list unchanged.)
		''' </summary>
		''' <param name="list"> The list in which to swap elements. </param>
		''' <param name="i"> the index of one element to be swapped. </param>
		''' <param name="j"> the index of the other element to be swapped. </param>
		''' <exception cref="IndexOutOfBoundsException"> if either <tt>i</tt> or <tt>j</tt>
		'''         is out of range (i &lt; 0 || i &gt;= list.size()
		'''         || j &lt; 0 || j &gt;= list.size()).
		''' @since 1.4 </exception>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
		Public Shared Sub swap(Of T1)(ByVal list As List(Of T1), ByVal i As Integer, ByVal j As Integer)
			' instead of using a raw type here, it's possible to capture
			' the wildcard but it will require a call to a supplementary
			' private method
			Dim l As List = list
			l.set(i, l.set(j, l.get(i)))
		End Sub

		''' <summary>
		''' Swaps the two specified elements in the specified array.
		''' </summary>
		Private Shared Sub swap(ByVal arr As Object(), ByVal i As Integer, ByVal j As Integer)
			Dim tmp As Object = arr(i)
			arr(i) = arr(j)
			arr(j) = tmp
		End Sub

		''' <summary>
		''' Replaces all of the elements of the specified list with the specified
		''' element. <p>
		''' 
		''' This method runs in linear time.
		''' </summary>
		''' @param  <T> the class of the objects in the list </param>
		''' <param name="list"> the list to be filled with the specified element. </param>
		''' <param name="obj"> The element with which to fill the specified list. </param>
		''' <exception cref="UnsupportedOperationException"> if the specified list or its
		'''         list-iterator does not support the <tt>set</tt> operation. </exception>
'JAVA TO VB CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
		Public Shared Sub fill(Of T, T1)(ByVal list As List(Of T1), ByVal obj As T)
			Dim size As Integer = list.size()

			If size < FILL_THRESHOLD OrElse TypeOf list Is RandomAccess Then
				For i As Integer = 0 To size - 1
					list.set(i, obj)
				Next i
			Else
'JAVA TO VB CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
				Dim itr As ListIterator(Of ?) = list.GetEnumerator()
				For i As Integer = 0 To size - 1
					itr.next()
					itr.set(obj)
				Next i
			End If
		End Sub

		''' <summary>
		''' Copies all of the elements from one list into another.  After the
		''' operation, the index of each copied element in the destination list
		''' will be identical to its index in the source list.  The destination
		''' list must be at least as long as the source list.  If it is longer, the
		''' remaining elements in the destination list are unaffected. <p>
		''' 
		''' This method runs in linear time.
		''' </summary>
		''' @param  <T> the class of the objects in the lists </param>
		''' <param name="dest"> The destination list. </param>
		''' <param name="src"> The source list. </param>
		''' <exception cref="IndexOutOfBoundsException"> if the destination list is too small
		'''         to contain the entire source List. </exception>
		''' <exception cref="UnsupportedOperationException"> if the destination list's
		'''         list-iterator does not support the <tt>set</tt> operation. </exception>
'JAVA TO VB CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
		Public Shared Sub copy(Of T, T1, T2 As T)(ByVal dest As List(Of T1), ByVal src As List(Of T2))
			Dim srcSize As Integer = src.size()
			If srcSize > dest.size() Then Throw New IndexOutOfBoundsException("Source does not fit in dest")

			If srcSize < COPY_THRESHOLD OrElse (TypeOf src Is RandomAccess AndAlso TypeOf dest Is RandomAccess) Then
				For i As Integer = 0 To srcSize - 1
					dest.set(i, src.get(i))
				Next i
			Else
'JAVA TO VB CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
				Dim di As ListIterator(Of ?)=dest.GetEnumerator()
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
				Dim si As ListIterator(Of ? As T)=src.GetEnumerator()
				For i As Integer = 0 To srcSize - 1
					di.next()
					di.set(si.next())
				Next i
			End If
		End Sub

		''' <summary>
		''' Returns the minimum element of the given collection, according to the
		''' <i>natural ordering</i> of its elements.  All elements in the
		''' collection must implement the <tt>Comparable</tt> interface.
		''' Furthermore, all elements in the collection must be <i>mutually
		''' comparable</i> (that is, <tt>e1.compareTo(e2)</tt> must not throw a
		''' <tt>ClassCastException</tt> for any elements <tt>e1</tt> and
		''' <tt>e2</tt> in the collection).<p>
		''' 
		''' This method iterates over the entire collection, hence it requires
		''' time proportional to the size of the collection.
		''' </summary>
		''' @param  <T> the class of the objects in the collection </param>
		''' <param name="coll"> the collection whose minimum element is to be determined. </param>
		''' <returns> the minimum element of the given collection, according
		'''         to the <i>natural ordering</i> of its elements. </returns>
		''' <exception cref="ClassCastException"> if the collection contains elements that are
		'''         not <i>mutually comparable</i> (for example, strings and
		'''         integers). </exception>
		''' <exception cref="NoSuchElementException"> if the collection is empty. </exception>
		''' <seealso cref= Comparable </seealso>
'JAVA TO VB CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
		Public Shared Function min(Of T As {Object, Comparable(Of ?)}, T1 As T)(ByVal coll As Collection(Of T1)) As T
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
			Dim i As [Iterator](Of ? As T) = coll.GetEnumerator()
			Dim candidate As T = i.next()

			Do While i.MoveNext()
				Dim [next] As T = i.Current
				If [next].CompareTo(candidate) < 0 Then candidate = [next]
			Loop
			Return candidate
		End Function

		''' <summary>
		''' Returns the minimum element of the given collection, according to the
		''' order induced by the specified comparator.  All elements in the
		''' collection must be <i>mutually comparable</i> by the specified
		''' comparator (that is, <tt>comp.compare(e1, e2)</tt> must not throw a
		''' <tt>ClassCastException</tt> for any elements <tt>e1</tt> and
		''' <tt>e2</tt> in the collection).<p>
		''' 
		''' This method iterates over the entire collection, hence it requires
		''' time proportional to the size of the collection.
		''' </summary>
		''' @param  <T> the class of the objects in the collection </param>
		''' <param name="coll"> the collection whose minimum element is to be determined. </param>
		''' <param name="comp"> the comparator with which to determine the minimum element.
		'''         A <tt>null</tt> value indicates that the elements' <i>natural
		'''         ordering</i> should be used. </param>
		''' <returns> the minimum element of the given collection, according
		'''         to the specified comparator. </returns>
		''' <exception cref="ClassCastException"> if the collection contains elements that are
		'''         not <i>mutually comparable</i> using the specified comparator. </exception>
		''' <exception cref="NoSuchElementException"> if the collection is empty. </exception>
		''' <seealso cref= Comparable </seealso>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'JAVA TO VB CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
		Public Shared Function min(Of T, T1 As T, T2)(ByVal coll As Collection(Of T1), ByVal comp As Comparator(Of T2)) As T
			If comp Is Nothing Then Return CType(min(CType(coll, Collection)), T)

'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
			Dim i As [Iterator](Of ? As T) = coll.GetEnumerator()
			Dim candidate As T = i.next()

			Do While i.MoveNext()
				Dim [next] As T = i.Current
				If comp.Compare([next], candidate) < 0 Then candidate = [next]
			Loop
			Return candidate
		End Function

		''' <summary>
		''' Returns the maximum element of the given collection, according to the
		''' <i>natural ordering</i> of its elements.  All elements in the
		''' collection must implement the <tt>Comparable</tt> interface.
		''' Furthermore, all elements in the collection must be <i>mutually
		''' comparable</i> (that is, <tt>e1.compareTo(e2)</tt> must not throw a
		''' <tt>ClassCastException</tt> for any elements <tt>e1</tt> and
		''' <tt>e2</tt> in the collection).<p>
		''' 
		''' This method iterates over the entire collection, hence it requires
		''' time proportional to the size of the collection.
		''' </summary>
		''' @param  <T> the class of the objects in the collection </param>
		''' <param name="coll"> the collection whose maximum element is to be determined. </param>
		''' <returns> the maximum element of the given collection, according
		'''         to the <i>natural ordering</i> of its elements. </returns>
		''' <exception cref="ClassCastException"> if the collection contains elements that are
		'''         not <i>mutually comparable</i> (for example, strings and
		'''         integers). </exception>
		''' <exception cref="NoSuchElementException"> if the collection is empty. </exception>
		''' <seealso cref= Comparable </seealso>
'JAVA TO VB CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
		Public Shared Function max(Of T As {Object, Comparable(Of ?)}, T1 As T)(ByVal coll As Collection(Of T1)) As T
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
			Dim i As [Iterator](Of ? As T) = coll.GetEnumerator()
			Dim candidate As T = i.next()

			Do While i.MoveNext()
				Dim [next] As T = i.Current
				If [next].CompareTo(candidate) > 0 Then candidate = [next]
			Loop
			Return candidate
		End Function

		''' <summary>
		''' Returns the maximum element of the given collection, according to the
		''' order induced by the specified comparator.  All elements in the
		''' collection must be <i>mutually comparable</i> by the specified
		''' comparator (that is, <tt>comp.compare(e1, e2)</tt> must not throw a
		''' <tt>ClassCastException</tt> for any elements <tt>e1</tt> and
		''' <tt>e2</tt> in the collection).<p>
		''' 
		''' This method iterates over the entire collection, hence it requires
		''' time proportional to the size of the collection.
		''' </summary>
		''' @param  <T> the class of the objects in the collection </param>
		''' <param name="coll"> the collection whose maximum element is to be determined. </param>
		''' <param name="comp"> the comparator with which to determine the maximum element.
		'''         A <tt>null</tt> value indicates that the elements' <i>natural
		'''        ordering</i> should be used. </param>
		''' <returns> the maximum element of the given collection, according
		'''         to the specified comparator. </returns>
		''' <exception cref="ClassCastException"> if the collection contains elements that are
		'''         not <i>mutually comparable</i> using the specified comparator. </exception>
		''' <exception cref="NoSuchElementException"> if the collection is empty. </exception>
		''' <seealso cref= Comparable </seealso>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'JAVA TO VB CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
		Public Shared Function max(Of T, T1 As T, T2)(ByVal coll As Collection(Of T1), ByVal comp As Comparator(Of T2)) As T
			If comp Is Nothing Then Return CType(max(CType(coll, Collection)), T)

'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
			Dim i As [Iterator](Of ? As T) = coll.GetEnumerator()
			Dim candidate As T = i.next()

			Do While i.MoveNext()
				Dim [next] As T = i.Current
				If comp.Compare([next], candidate) > 0 Then candidate = [next]
			Loop
			Return candidate
		End Function

		''' <summary>
		''' Rotates the elements in the specified list by the specified distance.
		''' After calling this method, the element at index <tt>i</tt> will be
		''' the element previously at index <tt>(i - distance)</tt> mod
		''' <tt>list.size()</tt>, for all values of <tt>i</tt> between <tt>0</tt>
		''' and <tt>list.size()-1</tt>, inclusive.  (This method has no effect on
		''' the size of the list.)
		''' 
		''' <p>For example, suppose <tt>list</tt> comprises<tt> [t, a, n, k, s]</tt>.
		''' After invoking <tt>Collections.rotate(list, 1)</tt> (or
		''' <tt>Collections.rotate(list, -4)</tt>), <tt>list</tt> will comprise
		''' <tt>[s, t, a, n, k]</tt>.
		''' 
		''' <p>Note that this method can usefully be applied to sublists to
		''' move one or more elements within a list while preserving the
		''' order of the remaining elements.  For example, the following idiom
		''' moves the element at index <tt>j</tt> forward to position
		''' <tt>k</tt> (which must be greater than or equal to <tt>j</tt>):
		''' <pre>
		'''     Collections.rotate(list.subList(j, k+1), -1);
		''' </pre>
		''' To make this concrete, suppose <tt>list</tt> comprises
		''' <tt>[a, b, c, d, e]</tt>.  To move the element at index <tt>1</tt>
		''' (<tt>b</tt>) forward two positions, perform the following invocation:
		''' <pre>
		'''     Collections.rotate(l.subList(1, 4), -1);
		''' </pre>
		''' The resulting list is <tt>[a, c, d, b, e]</tt>.
		''' 
		''' <p>To move more than one element forward, increase the absolute value
		''' of the rotation distance.  To move elements backward, use a positive
		''' shift distance.
		''' 
		''' <p>If the specified list is small or implements the {@link
		''' RandomAccess} interface, this implementation exchanges the first
		''' element into the location it should go, and then repeatedly exchanges
		''' the displaced element into the location it should go until a displaced
		''' element is swapped into the first element.  If necessary, the process
		''' is repeated on the second and successive elements, until the rotation
		''' is complete.  If the specified list is large and doesn't implement the
		''' <tt>RandomAccess</tt> interface, this implementation breaks the
		''' list into two sublist views around index <tt>-distance mod size</tt>.
		''' Then the <seealso cref="#reverse(List)"/> method is invoked on each sublist view,
		''' and finally it is invoked on the entire list.  For a more complete
		''' description of both algorithms, see Section 2.3 of Jon Bentley's
		''' <i>Programming Pearls</i> (Addison-Wesley, 1986).
		''' </summary>
		''' <param name="list"> the list to be rotated. </param>
		''' <param name="distance"> the distance to rotate the list.  There are no
		'''        constraints on this value; it may be zero, negative, or
		'''        greater than <tt>list.size()</tt>. </param>
		''' <exception cref="UnsupportedOperationException"> if the specified list or
		'''         its list-iterator does not support the <tt>set</tt> operation.
		''' @since 1.4 </exception>
		Public Shared Sub rotate(Of T1)(ByVal list As List(Of T1), ByVal distance As Integer)
			If TypeOf list Is RandomAccess OrElse list.size() < ROTATE_THRESHOLD Then
				rotate1(list, distance)
			Else
				rotate2(list, distance)
			End If
		End Sub

		Private Shared Sub rotate1(Of T)(ByVal list As List(Of T), ByVal distance As Integer)
			Dim size As Integer = list.size()
			If size = 0 Then Return
			distance = distance Mod size
			If distance < 0 Then distance += size
			If distance = 0 Then Return

			Dim cycleStart As Integer = 0
			Dim nMoved As Integer = 0
			Do While nMoved <> size
				Dim displaced As T = list.get(cycleStart)
				Dim i As Integer = cycleStart
				Do
					i += distance
					If i >= size Then i -= size
					displaced = list.set(i, displaced)
					nMoved += 1
				Loop While i <> cycleStart
				cycleStart += 1
			Loop
		End Sub

		Private Shared Sub rotate2(Of T1)(ByVal list As List(Of T1), ByVal distance As Integer)
			Dim size As Integer = list.size()
			If size = 0 Then Return
			Dim mid As Integer = -distance Mod size
			If mid < 0 Then mid += size
			If mid = 0 Then Return

			reverse(list.subList(0, mid))
			reverse(list.subList(mid, size))
			reverse(list)
		End Sub

		''' <summary>
		''' Replaces all occurrences of one specified value in a list with another.
		''' More formally, replaces with <tt>newVal</tt> each element <tt>e</tt>
		''' in <tt>list</tt> such that
		''' <tt>(oldVal==null ? e==null : oldVal.equals(e))</tt>.
		''' (This method has no effect on the size of the list.)
		''' </summary>
		''' @param  <T> the class of the objects in the list </param>
		''' <param name="list"> the list in which replacement is to occur. </param>
		''' <param name="oldVal"> the old value to be replaced. </param>
		''' <param name="newVal"> the new value with which <tt>oldVal</tt> is to be
		'''        replaced. </param>
		''' <returns> <tt>true</tt> if <tt>list</tt> contained one or more elements
		'''         <tt>e</tt> such that
		'''         <tt>(oldVal==null ?  e==null : oldVal.equals(e))</tt>. </returns>
		''' <exception cref="UnsupportedOperationException"> if the specified list or
		'''         its list-iterator does not support the <tt>set</tt> operation.
		''' @since  1.4 </exception>
		Public Shared Function replaceAll(Of T)(ByVal list As List(Of T), ByVal oldVal As T, ByVal newVal As T) As Boolean
			Dim result As Boolean = False
			Dim size As Integer = list.size()
			If size < REPLACEALL_THRESHOLD OrElse TypeOf list Is RandomAccess Then
				If oldVal Is Nothing Then
					For i As Integer = 0 To size - 1
						If list.get(i) Is Nothing Then
							list.set(i, newVal)
							result = True
						End If
					Next i
				Else
					For i As Integer = 0 To size - 1
						If oldVal.Equals(list.get(i)) Then
							list.set(i, newVal)
							result = True
						End If
					Next i
				End If
			Else
				Dim itr As ListIterator(Of T)=list.GetEnumerator()
				If oldVal Is Nothing Then
					For i As Integer = 0 To size - 1
						If itr.next() Is Nothing Then
							itr.set(newVal)
							result = True
						End If
					Next i
				Else
					For i As Integer = 0 To size - 1
						If oldVal.Equals(itr.next()) Then
							itr.set(newVal)
							result = True
						End If
					Next i
				End If
			End If
			Return result
		End Function

		''' <summary>
		''' Returns the starting position of the first occurrence of the specified
		''' target list within the specified source list, or -1 if there is no
		''' such occurrence.  More formally, returns the lowest index <tt>i</tt>
		''' such that {@code source.subList(i, i+target.size()).equals(target)},
		''' or -1 if there is no such index.  (Returns -1 if
		''' {@code target.size() > source.size()})
		''' 
		''' <p>This implementation uses the "brute force" technique of scanning
		''' over the source list, looking for a match with the target at each
		''' location in turn.
		''' </summary>
		''' <param name="source"> the list in which to search for the first occurrence
		'''        of <tt>target</tt>. </param>
		''' <param name="target"> the list to search for as a subList of <tt>source</tt>. </param>
		''' <returns> the starting position of the first occurrence of the specified
		'''         target list within the specified source list, or -1 if there
		'''         is no such occurrence.
		''' @since  1.4 </returns>
		Public Shared Function indexOfSubList(Of T1, T2)(ByVal source As List(Of T1), ByVal target As List(Of T2)) As Integer
			Dim sourceSize As Integer = source.size()
			Dim targetSize As Integer = target.size()
			Dim maxCandidate As Integer = sourceSize - targetSize

			If sourceSize < INDEXOFSUBLIST_THRESHOLD OrElse (TypeOf source Is RandomAccess AndAlso TypeOf target Is RandomAccess) Then
			nextCand:
				For candidate As Integer = 0 To maxCandidate
					Dim i As Integer=0
					Dim j As Integer=candidate
					Do While i<targetSize
						If Not eq(target.get(i), source.get(j)) Then GoTo nextCand ' Element mismatch, try next cand
						i += 1
						j += 1
					Loop
					Return candidate ' All elements of candidate matched target
				Next candidate ' Iterator version of above algorithm
			Else
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
				Dim si As ListIterator(Of ?) = source.GetEnumerator()
			nextCand:
				For candidate As Integer = 0 To maxCandidate
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
					Dim ti As ListIterator(Of ?) = target.GetEnumerator()
					For i As Integer = 0 To targetSize - 1
						If Not eq(ti.next(), si.next()) Then
							' Back up source iterator to next candidate
							For j As Integer = 0 To i - 1
								si.previous()
							Next j
							GoTo nextCand
						End If
					Next i
					Return candidate
				Next candidate
			End If
			Return -1 ' No candidate matched the target
		End Function

		''' <summary>
		''' Returns the starting position of the last occurrence of the specified
		''' target list within the specified source list, or -1 if there is no such
		''' occurrence.  More formally, returns the highest index <tt>i</tt>
		''' such that {@code source.subList(i, i+target.size()).equals(target)},
		''' or -1 if there is no such index.  (Returns -1 if
		''' {@code target.size() > source.size()})
		''' 
		''' <p>This implementation uses the "brute force" technique of iterating
		''' over the source list, looking for a match with the target at each
		''' location in turn.
		''' </summary>
		''' <param name="source"> the list in which to search for the last occurrence
		'''        of <tt>target</tt>. </param>
		''' <param name="target"> the list to search for as a subList of <tt>source</tt>. </param>
		''' <returns> the starting position of the last occurrence of the specified
		'''         target list within the specified source list, or -1 if there
		'''         is no such occurrence.
		''' @since  1.4 </returns>
		Public Shared Function lastIndexOfSubList(Of T1, T2)(ByVal source As List(Of T1), ByVal target As List(Of T2)) As Integer
			Dim sourceSize As Integer = source.size()
			Dim targetSize As Integer = target.size()
			Dim maxCandidate As Integer = sourceSize - targetSize

			If sourceSize < INDEXOFSUBLIST_THRESHOLD OrElse TypeOf source Is RandomAccess Then ' Index access version
			nextCand:
				For candidate As Integer = maxCandidate To 0 Step -1
					Dim i As Integer=0
					Dim j As Integer=candidate
					Do While i<targetSize
						If Not eq(target.get(i), source.get(j)) Then GoTo nextCand ' Element mismatch, try next cand
						i += 1
						j += 1
					Loop
					Return candidate ' All elements of candidate matched target
				Next candidate ' Iterator version of above algorithm
			Else
				If maxCandidate < 0 Then Return -1
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
				Dim si As ListIterator(Of ?) = source.listIterator(maxCandidate)
			nextCand:
				For candidate As Integer = maxCandidate To 0 Step -1
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
					Dim ti As ListIterator(Of ?) = target.GetEnumerator()
					For i As Integer = 0 To targetSize - 1
						If Not eq(ti.next(), si.next()) Then
							If candidate <> 0 Then
								' Back up source iterator to next candidate
								For j As Integer = 0 To i+1
									si.previous()
								Next j
							End If
							GoTo nextCand
						End If
					Next i
					Return candidate
				Next candidate
			End If
			Return -1 ' No candidate matched the target
		End Function


		' Unmodifiable Wrappers

		''' <summary>
		''' Returns an unmodifiable view of the specified collection.  This method
		''' allows modules to provide users with "read-only" access to internal
		''' collections.  Query operations on the returned collection "read through"
		''' to the specified collection, and attempts to modify the returned
		''' collection, whether direct or via its iterator, result in an
		''' <tt>UnsupportedOperationException</tt>.<p>
		''' 
		''' The returned collection does <i>not</i> pass the hashCode and equals
		''' operations through to the backing collection, but relies on
		''' <tt>Object</tt>'s <tt>equals</tt> and <tt>hashCode</tt> methods.  This
		''' is necessary to preserve the contracts of these operations in the case
		''' that the backing collection is a set or a list.<p>
		''' 
		''' The returned collection will be serializable if the specified collection
		''' is serializable.
		''' </summary>
		''' @param  <T> the class of the objects in the collection </param>
		''' <param name="c"> the collection for which an unmodifiable view is to be
		'''         returned. </param>
		''' <returns> an unmodifiable view of the specified collection. </returns>
		Public Shared Function unmodifiableCollection(Of T, T1 As T)(ByVal c As Collection(Of T1)) As Collection(Of T)
			Return New UnmodifiableCollection(Of )(c)
		End Function

		''' <summary>
		''' @serial include
		''' </summary>
		<Serializable> _
		Friend Class UnmodifiableCollection(Of E)
			Implements Collection(Of E)

			Private Const serialVersionUID As Long = 1820017752578914078L

'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
			Friend ReadOnly c As Collection(Of ? As E)

			Friend Sub New(Of T1 As E)(ByVal c As Collection(Of T1))
				If c Is Nothing Then Throw New NullPointerException
				Me.c = c
			End Sub

			Public Overridable Function size() As Integer Implements Collection(Of E).size
				Return c.size()
			End Function
			Public Overridable Property empty As Boolean Implements Collection(Of E).isEmpty
				Get
					Return c.empty
				End Get
			End Property
			Public Overridable Function contains(ByVal o As Object) As Boolean Implements Collection(Of E).contains
				Return c.contains(o)
			End Function
			Public Overridable Function toArray() As Object() Implements Collection(Of E).toArray
				Return c.ToArray()
			End Function
			Public Overridable Function toArray(Of T)(ByVal a As T()) As T() Implements Collection(Of E).toArray
				Return c.ToArray(a)
			End Function
			Public Overrides Function ToString() As String
				Return c.ToString()
			End Function

			Public Overridable Function [iterator]() As [Iterator](Of E) Implements Collection(Of E).iterator
				Return New IteratorAnonymousInnerClassHelper(Of E)
			End Function

			Private Class IteratorAnonymousInnerClassHelper(Of E)
				Implements Iterator(Of E)

'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
				Private ReadOnly i As [Iterator](Of ? As E) = outerInstance.c.GetEnumerator()

				Public Overridable Function hasNext() As Boolean Implements Iterator(Of E).hasNext
					Return i.hasNext()
				End Function
				Public Overridable Function [next]() As E Implements Iterator(Of E).next
					Return i.next()
				End Function
				Public Overridable Sub remove() Implements Iterator(Of E).remove
					Throw New UnsupportedOperationException
				End Sub
'JAVA TO VB CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
				Public Overrides Sub forEachRemaining(Of T1)(ByVal action As java.util.function.Consumer(Of T1)) Implements Iterator(Of E).forEachRemaining
					' Use backing collection version
					i.forEachRemaining(action)
				End Sub
			End Class

			Public Overridable Function add(ByVal e As E) As Boolean
				Throw New UnsupportedOperationException
			End Function
			Public Overridable Function remove(ByVal o As Object) As Boolean Implements Collection(Of E).remove
				Throw New UnsupportedOperationException
			End Function

			Public Overridable Function containsAll(Of T1)(ByVal coll As Collection(Of T1)) As Boolean Implements Collection(Of E).containsAll
				Return c.containsAll(coll)
			End Function
			Public Overridable Function addAll(Of T1 As E)(ByVal coll As Collection(Of T1)) As Boolean Implements Collection(Of E).addAll
				Throw New UnsupportedOperationException
			End Function
			Public Overridable Function removeAll(Of T1)(ByVal coll As Collection(Of T1)) As Boolean Implements Collection(Of E).removeAll
				Throw New UnsupportedOperationException
			End Function
			Public Overridable Function retainAll(Of T1)(ByVal coll As Collection(Of T1)) As Boolean Implements Collection(Of E).retainAll
				Throw New UnsupportedOperationException
			End Function
			Public Overridable Sub clear() Implements Collection(Of E).clear
				Throw New UnsupportedOperationException
			End Sub

			' Override default methods in Collection
'JAVA TO VB CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
			Public Overrides Sub forEach(Of T1)(ByVal action As java.util.function.Consumer(Of T1))
				c.forEach(action)
			End Sub
'JAVA TO VB CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
			Public Overrides Function removeIf(Of T1)(ByVal filter As java.util.function.Predicate(Of T1)) As Boolean Implements Collection(Of E).removeIf
				Throw New UnsupportedOperationException
			End Function
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
			Public Overrides Function spliterator() As Spliterator(Of E) Implements Collection(Of E).spliterator
				Return CType(c.spliterator(), Spliterator(Of E))
			End Function
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
			Public Overrides Function stream() As java.util.stream.Stream(Of E) Implements Collection(Of E).stream
				Return CType(c.stream(), java.util.stream.Stream(Of E))
			End Function
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
			Public Overrides Function parallelStream() As java.util.stream.Stream(Of E) Implements Collection(Of E).parallelStream
				Return CType(c.parallelStream(), java.util.stream.Stream(Of E))
			End Function
		End Class

		''' <summary>
		''' Returns an unmodifiable view of the specified set.  This method allows
		''' modules to provide users with "read-only" access to internal sets.
		''' Query operations on the returned set "read through" to the specified
		''' set, and attempts to modify the returned set, whether direct or via its
		''' iterator, result in an <tt>UnsupportedOperationException</tt>.<p>
		''' 
		''' The returned set will be serializable if the specified set
		''' is serializable.
		''' </summary>
		''' @param  <T> the class of the objects in the set </param>
		''' <param name="s"> the set for which an unmodifiable view is to be returned. </param>
		''' <returns> an unmodifiable view of the specified set. </returns>
		Public Shared Function unmodifiableSet(Of T, T1 As T)(ByVal s As [Set](Of T1)) As [Set](Of T)
			Return New UnmodifiableSet(Of )(s)
		End Function

		''' <summary>
		''' @serial include
		''' </summary>
		<Serializable> _
		Friend Class UnmodifiableSet(Of E)
			Inherits UnmodifiableCollection(Of E)
			Implements Set(Of E)

			Private Const serialVersionUID As Long = -9215047833775013803L

			Friend Sub New(Of T1 As E)(ByVal s As [Set](Of T1))
				MyBase.New(s)
			End Sub
			Public Overrides Function Equals(ByVal o As Object) As Boolean
				Return o Is Me OrElse c.Equals(o)
			End Function
			Public Overrides Function GetHashCode() As Integer
				Return c.GetHashCode()
			End Function
		End Class

		''' <summary>
		''' Returns an unmodifiable view of the specified sorted set.  This method
		''' allows modules to provide users with "read-only" access to internal
		''' sorted sets.  Query operations on the returned sorted set "read
		''' through" to the specified sorted set.  Attempts to modify the returned
		''' sorted set, whether direct, via its iterator, or via its
		''' <tt>subSet</tt>, <tt>headSet</tt>, or <tt>tailSet</tt> views, result in
		''' an <tt>UnsupportedOperationException</tt>.<p>
		''' 
		''' The returned sorted set will be serializable if the specified sorted set
		''' is serializable.
		''' </summary>
		''' @param  <T> the class of the objects in the set </param>
		''' <param name="s"> the sorted set for which an unmodifiable view is to be
		'''        returned. </param>
		''' <returns> an unmodifiable view of the specified sorted set. </returns>
		Public Shared Function unmodifiableSortedSet(Of T)(ByVal s As SortedSet(Of T)) As SortedSet(Of T)
			Return New UnmodifiableSortedSet(Of )(s)
		End Function

		''' <summary>
		''' @serial include
		''' </summary>
		<Serializable> _
		Friend Class UnmodifiableSortedSet(Of E)
			Inherits UnmodifiableSet(Of E)
			Implements SortedSet(Of E)

			Private Const serialVersionUID As Long = -4929149591599911165L
			Private ReadOnly ss As SortedSet(Of E)

			Friend Sub New(ByVal s As SortedSet(Of E))
				MyBase.New(s)
				ss = s
			End Sub

'JAVA TO VB CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
			Public Overridable Function comparator() As Comparator(Of ?) Implements SortedSet(Of E).comparator
				Return ss.comparator()
			End Function

			Public Overridable Function subSet(ByVal fromElement As E, ByVal toElement As E) As SortedSet(Of E)
				Return New UnmodifiableSortedSet(Of )(ss.subSet(fromElement,toElement))
			End Function
			Public Overridable Function headSet(ByVal toElement As E) As SortedSet(Of E)
				Return New UnmodifiableSortedSet(Of )(ss.headSet(toElement))
			End Function
			Public Overridable Function tailSet(ByVal fromElement As E) As SortedSet(Of E)
				Return New UnmodifiableSortedSet(Of )(ss.tailSet(fromElement))
			End Function

			Public Overridable Function first() As E Implements SortedSet(Of E).first
				Return ss.first()
			End Function
			Public Overridable Function last() As E Implements SortedSet(Of E).last
				Return ss.last()
			End Function
		End Class

		''' <summary>
		''' Returns an unmodifiable view of the specified navigable set.  This method
		''' allows modules to provide users with "read-only" access to internal
		''' navigable sets.  Query operations on the returned navigable set "read
		''' through" to the specified navigable set.  Attempts to modify the returned
		''' navigable set, whether direct, via its iterator, or via its
		''' {@code subSet}, {@code headSet}, or {@code tailSet} views, result in
		''' an {@code UnsupportedOperationException}.<p>
		''' 
		''' The returned navigable set will be serializable if the specified
		''' navigable set is serializable.
		''' </summary>
		''' @param  <T> the class of the objects in the set </param>
		''' <param name="s"> the navigable set for which an unmodifiable view is to be
		'''        returned </param>
		''' <returns> an unmodifiable view of the specified navigable set
		''' @since 1.8 </returns>
		Public Shared Function unmodifiableNavigableSet(Of T)(ByVal s As NavigableSet(Of T)) As NavigableSet(Of T)
			Return New UnmodifiableNavigableSet(Of )(s)
		End Function

		''' <summary>
		''' Wraps a navigable set and disables all of the mutative operations.
		''' </summary>
		''' @param <E> type of elements
		''' @serial include </param>
		<Serializable> _
		Friend Class UnmodifiableNavigableSet(Of E)
			Inherits UnmodifiableSortedSet(Of E)
			Implements NavigableSet(Of E)

			Private Const serialVersionUID As Long = -6027448201786391929L

			''' <summary>
			''' A singleton empty unmodifiable navigable set used for
			''' <seealso cref="#emptyNavigableSet()"/>.
			''' </summary>
			''' @param <E> type of elements, if there were any, and bounds </param>
			<Serializable> _
			Private Class EmptyNavigableSet(Of E)
				Inherits UnmodifiableNavigableSet(Of E)

				Private Const serialVersionUID As Long = -6291252904449939134L

				Public Sub New()
					MyBase.New(New TreeSet(Of E))
				End Sub

				Private Function readResolve() As Object
					Return EMPTY_NAVIGABLE_SET
				End Function
			End Class

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
			Private Shared ReadOnly EMPTY_NAVIGABLE_SET As NavigableSet(Of ?) = New EmptyNavigableSet(Of ?)

			''' <summary>
			''' The instance we are protecting.
			''' </summary>
			Private ReadOnly ns As NavigableSet(Of E)

			Friend Sub New(ByVal s As NavigableSet(Of E))
				MyBase.New(s)
				ns = s
			End Sub

			Public Overridable Function lower(ByVal e As E) As E
				Return ns.lower(e)
			End Function
			Public Overridable Function floor(ByVal e As E) As E
				Return ns.floor(e)
			End Function
			Public Overridable Function ceiling(ByVal e As E) As E
				Return ns.ceiling(e)
			End Function
			Public Overridable Function higher(ByVal e As E) As E
				Return ns.higher(e)
			End Function
			Public Overridable Function pollFirst() As E Implements NavigableSet(Of E).pollFirst
				Throw New UnsupportedOperationException
			End Function
			Public Overridable Function pollLast() As E Implements NavigableSet(Of E).pollLast
				Throw New UnsupportedOperationException
			End Function
			Public Overridable Function descendingSet() As NavigableSet(Of E) Implements NavigableSet(Of E).descendingSet
						 Return New UnmodifiableNavigableSet(Of )(ns.descendingSet())
			End Function
			Public Overridable Function descendingIterator() As [Iterator](Of E) Implements NavigableSet(Of E).descendingIterator
												 Return descendingSet().GetEnumerator()
			End Function

			Public Overridable Function subSet(ByVal fromElement As E, ByVal fromInclusive As Boolean, ByVal toElement As E, ByVal toInclusive As Boolean) As NavigableSet(Of E)
				Return New UnmodifiableNavigableSet(Of )(ns.subSet(fromElement, fromInclusive, toElement, toInclusive))
			End Function

			Public Overridable Function headSet(ByVal toElement As E, ByVal inclusive As Boolean) As NavigableSet(Of E)
				Return New UnmodifiableNavigableSet(Of )(ns.headSet(toElement, inclusive))
			End Function

			Public Overridable Function tailSet(ByVal fromElement As E, ByVal inclusive As Boolean) As NavigableSet(Of E)
				Return New UnmodifiableNavigableSet(Of )(ns.tailSet(fromElement, inclusive))
			End Function
		End Class

		''' <summary>
		''' Returns an unmodifiable view of the specified list.  This method allows
		''' modules to provide users with "read-only" access to internal
		''' lists.  Query operations on the returned list "read through" to the
		''' specified list, and attempts to modify the returned list, whether
		''' direct or via its iterator, result in an
		''' <tt>UnsupportedOperationException</tt>.<p>
		''' 
		''' The returned list will be serializable if the specified list
		''' is serializable. Similarly, the returned list will implement
		''' <seealso cref="RandomAccess"/> if the specified list does.
		''' </summary>
		''' @param  <T> the class of the objects in the list </param>
		''' <param name="list"> the list for which an unmodifiable view is to be returned. </param>
		''' <returns> an unmodifiable view of the specified list. </returns>
		Public Shared Function unmodifiableList(Of T, T1 As T)(ByVal list As List(Of T1)) As List(Of T)
			Return (If(TypeOf list Is RandomAccess, New UnmodifiableRandomAccessList(Of )(list), New UnmodifiableList(Of )(list)))
		End Function

		''' <summary>
		''' @serial include
		''' </summary>
		Friend Class UnmodifiableList(Of E)
			Inherits UnmodifiableCollection(Of E)
			Implements List(Of E)

			Private Const serialVersionUID As Long = -283967356065247728L

'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
			Friend ReadOnly list As List(Of ? As E)

			Friend Sub New(Of T1 As E)(ByVal list As List(Of T1))
				MyBase.New(list)
				Me.list = list
			End Sub

			Public Overrides Function Equals(ByVal o As Object) As Boolean
				Return o Is Me OrElse list.Equals(o)
			End Function
			Public Overrides Function GetHashCode() As Integer
				Return list.GetHashCode()
			End Function

			Public Overridable Function [get](ByVal index As Integer) As E Implements List(Of E).get
				Return list.get(index)
			End Function
			Public Overridable Function [set](ByVal index As Integer, ByVal element As E) As E
				Throw New UnsupportedOperationException
			End Function
			Public Overridable Sub add(ByVal index As Integer, ByVal element As E)
				Throw New UnsupportedOperationException
			End Sub
			Public Overridable Function remove(ByVal index As Integer) As E Implements List(Of E).remove
				Throw New UnsupportedOperationException
			End Function
			Public Overridable Function indexOf(ByVal o As Object) As Integer Implements List(Of E).indexOf
				Return list.IndexOf(o)
			End Function
			Public Overridable Function lastIndexOf(ByVal o As Object) As Integer Implements List(Of E).lastIndexOf
				Return list.LastIndexOf(o)
			End Function
			Public Overridable Function addAll(Of T1 As E)(ByVal index As Integer, ByVal c As Collection(Of T1)) As Boolean Implements List(Of E).addAll
				Throw New UnsupportedOperationException
			End Function

			Public Overrides Sub replaceAll(ByVal [operator] As java.util.function.UnaryOperator(Of E)) Implements List(Of E).replaceAll
				Throw New UnsupportedOperationException
			End Sub
'JAVA TO VB CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
			Public Overrides Sub sort(Of T1)(ByVal c As Comparator(Of T1)) Implements List(Of E).sort
				Throw New UnsupportedOperationException
			End Sub

			Public Overridable Function listIterator() As ListIterator(Of E) Implements List(Of E).listIterator
				Return listIterator(0)
			End Function

			Public Overridable Function listIterator(ByVal index As Integer) As ListIterator(Of E) Implements List(Of E).listIterator
				Return New ListIteratorAnonymousInnerClassHelper(Of E)
			End Function

			Private Class ListIteratorAnonymousInnerClassHelper(Of E)
				Implements ListIterator(Of E)

'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
				Private ReadOnly i As ListIterator(Of ? As E) = outerInstance.list.listIterator(index)

				Public Overridable Function hasNext() As Boolean Implements ListIterator(Of E).hasNext
					Return i.hasNext()
				End Function
				Public Overridable Function [next]() As E Implements ListIterator(Of E).next
					Return i.next()
				End Function
				Public Overridable Function hasPrevious() As Boolean Implements ListIterator(Of E).hasPrevious
					Return i.hasPrevious()
				End Function
				Public Overridable Function previous() As E Implements ListIterator(Of E).previous
					Return i.previous()
				End Function
				Public Overridable Function nextIndex() As Integer Implements ListIterator(Of E).nextIndex
					Return i.nextIndex()
				End Function
				Public Overridable Function previousIndex() As Integer Implements ListIterator(Of E).previousIndex
					Return i.previousIndex()
				End Function

				Public Overridable Sub remove() Implements ListIterator(Of E).remove
					Throw New UnsupportedOperationException
				End Sub
				Public Overridable Sub [set](ByVal e As E)
					Throw New UnsupportedOperationException
				End Sub
				Public Overridable Sub add(ByVal e As E)
					Throw New UnsupportedOperationException
				End Sub

'JAVA TO VB CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
				Public Overrides Sub forEachRemaining(Of T1)(ByVal action As java.util.function.Consumer(Of T1))
					i.forEachRemaining(action)
				End Sub
			End Class

			Public Overridable Function subList(ByVal fromIndex As Integer, ByVal toIndex As Integer) As List(Of E) Implements List(Of E).subList
				Return New UnmodifiableList(Of )(list.subList(fromIndex, toIndex))
			End Function

			''' <summary>
			''' UnmodifiableRandomAccessList instances are serialized as
			''' UnmodifiableList instances to allow them to be deserialized
			''' in pre-1.4 JREs (which do not have UnmodifiableRandomAccessList).
			''' This method inverts the transformation.  As a beneficial
			''' side-effect, it also grafts the RandomAccess marker onto
			''' UnmodifiableList instances that were serialized in pre-1.4 JREs.
			''' 
			''' Note: Unfortunately, UnmodifiableRandomAccessList instances
			''' serialized in 1.4.1 and deserialized in 1.4 will become
			''' UnmodifiableList instances, as this method was missing in 1.4.
			''' </summary>
			Private Function readResolve() As Object
				Return (TypeOf list Is RandomAccess ? New UnmodifiableRandomAccessList(Of )(list)
						: Me)
			End Function
		End Class

		''' <summary>
		''' @serial include
		''' </summary>
		Friend Class UnmodifiableRandomAccessList(Of E)
			Inherits UnmodifiableList(Of E)
			Implements RandomAccess

			Friend Sub New(Of T1 As E)(ByVal list As List(Of T1))
				MyBase.New(list)
			End Sub

			Public Overridable Function subList(ByVal fromIndex As Integer, ByVal toIndex As Integer) As List(Of E)
				Return New UnmodifiableRandomAccessList(Of )(list.subList(fromIndex, toIndex))
			End Function

			Private Const serialVersionUID As Long = -2542308836966382001L

			''' <summary>
			''' Allows instances to be deserialized in pre-1.4 JREs (which do
			''' not have UnmodifiableRandomAccessList).  UnmodifiableList has
			''' a readResolve method that inverts this transformation upon
			''' deserialization.
			''' </summary>
			Private Function writeReplace() As Object
				Return New UnmodifiableList(Of )(list)
			End Function
		End Class

		''' <summary>
		''' Returns an unmodifiable view of the specified map.  This method
		''' allows modules to provide users with "read-only" access to internal
		''' maps.  Query operations on the returned map "read through"
		''' to the specified map, and attempts to modify the returned
		''' map, whether direct or via its collection views, result in an
		''' <tt>UnsupportedOperationException</tt>.<p>
		''' 
		''' The returned map will be serializable if the specified map
		''' is serializable.
		''' </summary>
		''' @param <K> the class of the map keys </param>
		''' @param <V> the class of the map values </param>
		''' <param name="m"> the map for which an unmodifiable view is to be returned. </param>
		''' <returns> an unmodifiable view of the specified map. </returns>
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
		Public Shared Function unmodifiableMap(Of K, V, T1 As K, ? As V)(ByVal m As Map(Of T1)) As Map(Of K, V)
			Return New UnmodifiableMap(Of )(m)
		End Function

		''' <summary>
		''' @serial include
		''' </summary>
		<Serializable> _
		Private Class UnmodifiableMap(Of K, V)
			Implements Map(Of K, V)

			Private Const serialVersionUID As Long = -1034234728574286014L

'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
			Private ReadOnly m As Map(Of ? As K, ? As V)

'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
			Friend Sub New(Of T1 As K, ? As V)(ByVal m As Map(Of T1))
				If m Is Nothing Then Throw New NullPointerException
				Me.m = m
			End Sub

			Public Overridable Function size() As Integer Implements Map(Of K, V).size
				Return m.size()
			End Function
			Public Overridable Property empty As Boolean Implements Map(Of K, V).isEmpty
				Get
					Return m.empty
				End Get
			End Property
			Public Overridable Function containsKey(ByVal key As Object) As Boolean Implements Map(Of K, V).containsKey
				Return m.containsKey(key)
			End Function
			Public Overridable Function containsValue(ByVal val As Object) As Boolean Implements Map(Of K, V).containsValue
				Return m.containsValue(val)
			End Function
			Public Overridable Function [get](ByVal key As Object) As V Implements Map(Of K, V).get
				Return m.get(key)
			End Function

			Public Overridable Function put(ByVal key As K, ByVal value As V) As V Implements Map(Of K, V).put
				Throw New UnsupportedOperationException
			End Function
			Public Overridable Function remove(ByVal key As Object) As V Implements Map(Of K, V).remove
				Throw New UnsupportedOperationException
			End Function
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
			Public Overridable Sub putAll(Of T1 As K, ? As V)(ByVal m As Map(Of T1)) Implements Map(Of K, V).putAll
				Throw New UnsupportedOperationException
			End Sub
			Public Overridable Sub clear() Implements Map(Of K, V).clear
				Throw New UnsupportedOperationException
			End Sub

			<NonSerialized> _
			Private keySet_Renamed As [Set](Of K)
			<NonSerialized> _
			Private entrySet_Renamed As [Set](Of KeyValuePair(Of K, V))
			<NonSerialized> _
			Private values_Renamed As Collection(Of V)

			Public Overridable Function keySet() As [Set](Of K) Implements Map(Of K, V).keySet
				If keySet_Renamed Is Nothing Then keySet_Renamed = unmodifiableSet(m.Keys)
				Return keySet_Renamed
			End Function

			Public Overridable Function entrySet() As [Set](Of KeyValuePair(Of K, V)) Implements Map(Of K, V).entrySet
				If entrySet_Renamed Is Nothing Then entrySet_Renamed = New UnmodifiableEntrySet(Of )(m.entrySet())
				Return entrySet_Renamed
			End Function

			Public Overridable Function values() As Collection(Of V) Implements Map(Of K, V).values
				If values_Renamed Is Nothing Then values_Renamed = unmodifiableCollection(m.values())
				Return values_Renamed
			End Function

			Public Overrides Function Equals(ByVal o As Object) As Boolean
				Return o Is Me OrElse m.Equals(o)
			End Function
			Public Overrides Function GetHashCode() As Integer
				Return m.GetHashCode()
			End Function
			Public Overrides Function ToString() As String
				Return m.ToString()
			End Function

			' Override default methods in Map
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
			Public Overrides Function getOrDefault(ByVal k As Object, ByVal defaultValue As V) As V Implements Map(Of K, V).getOrDefault
				' Safe cast as we don't change the value
				Return CType(m, Map(Of K, V)).getOrDefault(k, defaultValue)
			End Function

'JAVA TO VB CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
			Public Overrides Sub forEach(Of T1)(ByVal action As java.util.function.BiConsumer(Of T1)) Implements Map(Of K, V).forEach
				m.forEach(action)
			End Sub

'JAVA TO VB CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
			Public Overrides Sub replaceAll(Of T1 As V)(ByVal [function] As java.util.function.BiFunction(Of T1)) Implements Map(Of K, V).replaceAll
				Throw New UnsupportedOperationException
			End Sub

			Public Overrides Function putIfAbsent(ByVal key As K, ByVal value As V) As V Implements Map(Of K, V).putIfAbsent
				Throw New UnsupportedOperationException
			End Function

			Public Overrides Function remove(ByVal key As Object, ByVal value As Object) As Boolean Implements Map(Of K, V).remove
				Throw New UnsupportedOperationException
			End Function

			Public Overrides Function replace(ByVal key As K, ByVal oldValue As V, ByVal newValue As V) As Boolean Implements Map(Of K, V).replace
				Throw New UnsupportedOperationException
			End Function

			Public Overrides Function replace(ByVal key As K, ByVal value As V) As V Implements Map(Of K, V).replace
				Throw New UnsupportedOperationException
			End Function

'JAVA TO VB CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
			Public Overrides Function computeIfAbsent(Of T1 As V)(ByVal key As K, ByVal mappingFunction As java.util.function.Function(Of T1)) As V Implements Map(Of K, V).computeIfAbsent
				Throw New UnsupportedOperationException
			End Function

'JAVA TO VB CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
			Public Overrides Function computeIfPresent(Of T1 As V)(ByVal key As K, ByVal remappingFunction As java.util.function.BiFunction(Of T1)) As V Implements Map(Of K, V).computeIfPresent
				Throw New UnsupportedOperationException
			End Function

'JAVA TO VB CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
			Public Overrides Function compute(Of T1 As V)(ByVal key As K, ByVal remappingFunction As java.util.function.BiFunction(Of T1)) As V Implements Map(Of K, V).compute
				Throw New UnsupportedOperationException
			End Function

'JAVA TO VB CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
			Public Overrides Function merge(Of T1 As V)(ByVal key As K, ByVal value As V, ByVal remappingFunction As java.util.function.BiFunction(Of T1)) As V Implements Map(Of K, V).merge
				Throw New UnsupportedOperationException
			End Function

			''' <summary>
			''' We need this class in addition to UnmodifiableSet as
			''' Map.Entries themselves permit modification of the backing Map
			''' via their setValue operation.  This class is subtle: there are
			''' many possible attacks that must be thwarted.
			''' 
			''' @serial include
			''' </summary>
			Friend Class UnmodifiableEntrySet(Of K, V)
				Inherits UnmodifiableSet(Of KeyValuePair(Of K, V))

				Private Const serialVersionUID As Long = 7854390611657943733L

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
				Friend Sub New(Of T1 As KeyValuePair(Of ? As K, ? As V)(ByVal s As [Set](Of T1))
					' Need to cast to raw in order to work around a limitation in the type system
					MyBase.New(CType(s, [Set]))
				End Sub

'JAVA TO VB CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
				Friend Shared Function entryConsumer(Of K, V, T1)(ByVal action As java.util.function.Consumer(Of T1)) As java.util.function.Consumer(Of KeyValuePair(Of K, V))
					Return e -> action.accept(New UnmodifiableEntry(Of )(e))
				End Function

'JAVA TO VB CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
				Public Overridable Sub forEach(Of T1)(ByVal action As java.util.function.Consumer(Of T1))
					Objects.requireNonNull(action)
					c.forEach(entryConsumer(action))
				End Sub

				Friend NotInheritable Class UnmodifiableEntrySetSpliterator(Of K, V)
					Implements Spliterator(Of Entry(Of K, V))

					Friend ReadOnly s As Spliterator(Of KeyValuePair(Of K, V))

					Friend Sub New(ByVal s As Spliterator(Of Entry(Of K, V)))
						Me.s = s
					End Sub

'JAVA TO VB CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
					Public Overrides Function tryAdvance(Of T1)(ByVal action As java.util.function.Consumer(Of T1)) As Boolean Implements Spliterator(Of Entry(Of K, V)).tryAdvance
						Objects.requireNonNull(action)
						Return s.tryAdvance(entryConsumer(action))
					End Function

'JAVA TO VB CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
					Public Overrides Sub forEachRemaining(Of T1)(ByVal action As java.util.function.Consumer(Of T1)) Implements Spliterator(Of Entry(Of K, V)).forEachRemaining
						Objects.requireNonNull(action)
						s.forEachRemaining(entryConsumer(action))
					End Sub

					Public Overrides Function trySplit() As Spliterator(Of Entry(Of K, V)) Implements Spliterator(Of Entry(Of K, V)).trySplit
						Dim Split As Spliterator(Of Entry(Of K, V)) = s.trySplit()
						Return If(Split Is Nothing, Nothing, New UnmodifiableEntrySetSpliterator(Of )(Split))
					End Function

					Public Overrides Function estimateSize() As Long Implements Spliterator(Of Entry(Of K, V)).estimateSize
						Return s.estimateSize()
					End Function

					Public Property Overrides exactSizeIfKnown As Long Implements Spliterator(Of Entry(Of K, V)).getExactSizeIfKnown
						Get
							Return s.exactSizeIfKnown
						End Get
					End Property

					Public Overrides Function characteristics() As Integer Implements Spliterator(Of Entry(Of K, V)).characteristics
						Return s.characteristics()
					End Function

					Public Overrides Function hasCharacteristics(ByVal characteristics As Integer) As Boolean Implements Spliterator(Of Entry(Of K, V)).hasCharacteristics
						Return s.hasCharacteristics(characteristics)
					End Function

'JAVA TO VB CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
					Public Overrides Function getComparator() As Comparator(Of ?) Implements Spliterator(Of Entry(Of K, V)).getComparator
						Return s.comparator
					End Function
				End Class

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
				Public Overridable Function spliterator() As Spliterator(Of Entry(Of K, V))
					Return New UnmodifiableEntrySetSpliterator(Of )(CType(c.spliterator(), Spliterator(Of KeyValuePair(Of K, V))))
				End Function

				Public Overrides Function stream() As java.util.stream.Stream(Of Entry(Of K, V))
					Return java.util.stream.StreamSupport.stream(spliterator(), False)
				End Function

				Public Overrides Function parallelStream() As java.util.stream.Stream(Of Entry(Of K, V))
					Return java.util.stream.StreamSupport.stream(spliterator(), True)
				End Function

				Public Overridable Function [iterator]() As [Iterator](Of KeyValuePair(Of K, V))
					Return New IteratorAnonymousInnerClassHelper(Of E)
				End Function

				Private Class IteratorAnonymousInnerClassHelper(Of E)
					Implements Iterator(Of E)

'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
					Private ReadOnly i As [Iterator](Of ? As KeyValuePair(Of ? As K, ? As V)) = outerInstance.c.GetEnumerator()

					Public Overridable Function hasNext() As Boolean Implements Iterator(Of E).hasNext
						Return i.hasNext()
					End Function
					Public Overridable Function [next]() As KeyValuePair(Of K, V)
						Return New UnmodifiableEntry(Of )(i.next())
					End Function
					Public Overridable Sub remove() Implements Iterator(Of E).remove
						Throw New UnsupportedOperationException
					End Sub
				End Class

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
				Public Overridable Function toArray() As Object()
					Dim a As Object() = c.ToArray()
					For i As Integer = 0 To a.Length - 1
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
						a(i) = New UnmodifiableEntry(Of )(CType(a(i), KeyValuePair(Of ? As K, ? As V)))
					Next i
					Return a
				End Function

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
				Public Overridable Function toArray(Of T)(ByVal a As T()) As T()
					' We don't pass a to c.toArray, to avoid window of
					' vulnerability wherein an unscrupulous multithreaded client
					' could get his hands on raw (unwrapped) Entries from c.
					Dim arr As Object() = c.ToArray(If(a.Length=0, a, Arrays.copyOf(a, 0)))

					For i As Integer = 0 To arr.Length - 1
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
						arr(i) = New UnmodifiableEntry(Of )(CType(arr(i), KeyValuePair(Of ? As K, ? As V)))
					Next i

					If arr.Length > a.Length Then Return CType(arr, T())

					Array.Copy(arr, 0, a, 0, arr.Length)
					If a.Length > arr.Length Then a(arr.Length) = Nothing
					Return a
				End Function

				''' <summary>
				''' This method is overridden to protect the backing set against
				''' an object with a nefarious equals function that senses
				''' that the equality-candidate is Map.Entry and calls its
				''' setValue method.
				''' </summary>
				Public Overridable Function contains(ByVal o As Object) As Boolean
					If Not(TypeOf o Is DictionaryEntry) Then Return False
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
					Return c.contains(New UnmodifiableEntry(Of )(CType(o, KeyValuePair(Of ?, ?))))
				End Function

				''' <summary>
				''' The next two methods are overridden to protect against
				''' an unscrupulous List whose contains(Object o) method senses
				''' when o is a Map.Entry, and calls o.setValue.
				''' </summary>
				Public Overridable Function containsAll(Of T1)(ByVal coll As Collection(Of T1)) As Boolean
					For Each e As Object In coll
						If Not contains(e) Then ' Invokes safe contains() above Return False
					Next e
					Return True
				End Function
				Public Overrides Function Equals(ByVal o As Object) As Boolean
					If o Is Me Then Return True

					If Not(TypeOf o Is Set) Then Return False
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
					Dim s As [Set](Of ?) = CType(o, Set(Of ?))
					If s.size() <> c.size() Then Return False
					Return containsAll(s) ' Invokes safe containsAll() above
				End Function

				''' <summary>
				''' This "wrapper class" serves two purposes: it prevents
				''' the client from modifying the backing Map, by short-circuiting
				''' the setValue method, and it protects the backing Map against
				''' an ill-behaved Map.Entry that attempts to modify another
				''' Map Entry when asked to perform an equality check.
				''' </summary>
				Private Class UnmodifiableEntry(Of K, V)
					Implements KeyValuePair(Of K, V)

'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
					Private e As KeyValuePair(Of ? As K, ? As V)

'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
					Friend Sub New(Of T1 As K, ? As V)(ByVal e As KeyValuePair(Of T1))
								Me.e = Objects.requireNonNull(e)
					End Sub

					Public Overridable Property key As K
						Get
							Return e.Key
						End Get
					End Property
					Public Overridable Property value As V
						Get
							Return e.Value
						End Get
					End Property
					Public Overridable Function setValue(ByVal value As V) As V
						Throw New UnsupportedOperationException
					End Function
					Public Overrides Function GetHashCode() As Integer
						Return e.GetHashCode()
					End Function
					Public Overrides Function Equals(ByVal o As Object) As Boolean
						If Me Is o Then Return True
						If Not(TypeOf o Is DictionaryEntry) Then Return False
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
						Dim t As KeyValuePair(Of ?, ?) = CType(o, KeyValuePair(Of ?, ?))
						Return eq(e.Key, t.Key) AndAlso eq(e.Value, t.Value)
					End Function
					Public Overrides Function ToString() As String
						Return e.ToString()
					End Function
				End Class
			End Class
		End Class

		''' <summary>
		''' Returns an unmodifiable view of the specified sorted map.  This method
		''' allows modules to provide users with "read-only" access to internal
		''' sorted maps.  Query operations on the returned sorted map "read through"
		''' to the specified sorted map.  Attempts to modify the returned
		''' sorted map, whether direct, via its collection views, or via its
		''' <tt>subMap</tt>, <tt>headMap</tt>, or <tt>tailMap</tt> views, result in
		''' an <tt>UnsupportedOperationException</tt>.<p>
		''' 
		''' The returned sorted map will be serializable if the specified sorted map
		''' is serializable.
		''' </summary>
		''' @param <K> the class of the map keys </param>
		''' @param <V> the class of the map values </param>
		''' <param name="m"> the sorted map for which an unmodifiable view is to be
		'''        returned. </param>
		''' <returns> an unmodifiable view of the specified sorted map. </returns>
		Public Shared Function unmodifiableSortedMap(Of K, V, T1 As V)(ByVal m As SortedMap(Of T1)) As SortedMap(Of K, V)
			Return New UnmodifiableSortedMap(Of )(m)
		End Function

		''' <summary>
		''' @serial include
		''' </summary>
		<Serializable> _
		Friend Class UnmodifiableSortedMap(Of K, V)
			Inherits UnmodifiableMap(Of K, V)
			Implements SortedMap(Of K, V)

			Private Const serialVersionUID As Long = -8806743815996713206L

'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
			Private ReadOnly sm As SortedMap(Of K, ? As V)

			Friend Sub New(Of T1 As V)(ByVal m As SortedMap(Of T1))
				MyBase.New(m)
				sm = m
			End Sub
'JAVA TO VB CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
			Public Overridable Function comparator() As Comparator(Of ?) Implements SortedMap(Of K, V).comparator
				Return sm.comparator()
			End Function
			Public Overridable Function subMap(ByVal fromKey As K, ByVal toKey As K) As SortedMap(Of K, V) Implements SortedMap(Of K, V).subMap
					 Return New UnmodifiableSortedMap(Of )(sm.subMap(fromKey, toKey))
			End Function
			Public Overridable Function headMap(ByVal toKey As K) As SortedMap(Of K, V) Implements SortedMap(Of K, V).headMap
							 Return New UnmodifiableSortedMap(Of )(sm.headMap(toKey))
			End Function
			Public Overridable Function tailMap(ByVal fromKey As K) As SortedMap(Of K, V) Implements SortedMap(Of K, V).tailMap
						   Return New UnmodifiableSortedMap(Of )(sm.tailMap(fromKey))
			End Function
			Public Overridable Function firstKey() As K Implements SortedMap(Of K, V).firstKey
				Return sm.firstKey()
			End Function
			Public Overridable Function lastKey() As K Implements SortedMap(Of K, V).lastKey
				Return sm.lastKey()
			End Function
		End Class

		''' <summary>
		''' Returns an unmodifiable view of the specified navigable map.  This method
		''' allows modules to provide users with "read-only" access to internal
		''' navigable maps.  Query operations on the returned navigable map "read
		''' through" to the specified navigable map.  Attempts to modify the returned
		''' navigable map, whether direct, via its collection views, or via its
		''' {@code subMap}, {@code headMap}, or {@code tailMap} views, result in
		''' an {@code UnsupportedOperationException}.<p>
		''' 
		''' The returned navigable map will be serializable if the specified
		''' navigable map is serializable.
		''' </summary>
		''' @param <K> the class of the map keys </param>
		''' @param <V> the class of the map values </param>
		''' <param name="m"> the navigable map for which an unmodifiable view is to be
		'''        returned </param>
		''' <returns> an unmodifiable view of the specified navigable map
		''' @since 1.8 </returns>
		Public Shared Function unmodifiableNavigableMap(Of K, V, T1 As V)(ByVal m As NavigableMap(Of T1)) As NavigableMap(Of K, V)
			Return New UnmodifiableNavigableMap(Of )(m)
		End Function

		''' <summary>
		''' @serial include
		''' </summary>
		<Serializable> _
		Friend Class UnmodifiableNavigableMap(Of K, V)
			Inherits UnmodifiableSortedMap(Of K, V)
			Implements NavigableMap(Of K, V)

			Private Const serialVersionUID As Long = -4858195264774772197L

			''' <summary>
			''' A class for the <seealso cref="EMPTY_NAVIGABLE_MAP"/> which needs readResolve
			''' to preserve singleton property.
			''' </summary>
			''' @param <K> type of keys, if there were any, and of bounds </param>
			''' @param <V> type of values, if there were any </param>
			<Serializable> _
			Private Class EmptyNavigableMap(Of K, V)
				Inherits UnmodifiableNavigableMap(Of K, V)

				Private Const serialVersionUID As Long = -2239321462712562324L

				Friend Sub New()
					MyBase.New(New TreeMap(Of K, V))
				End Sub

				Public Overrides Function navigableKeySet() As NavigableSet(Of K)
														Return emptyNavigableSet()
				End Function

				Private Function readResolve() As Object
					Return EMPTY_NAVIGABLE_MAP
				End Function
			End Class

			''' <summary>
			''' Singleton for <seealso cref="emptyNavigableMap()"/> which is also immutable.
			''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
			Private Shared ReadOnly EMPTY_NAVIGABLE_MAP As New EmptyNavigableMap(Of ?, ?)

			''' <summary>
			''' The instance we wrap and protect.
			''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
			Private ReadOnly nm As NavigableMap(Of K, ? As V)

			Friend Sub New(Of T1 As V)(ByVal m As NavigableMap(Of T1))
																	MyBase.New(m)
																	nm = m
			End Sub

			Public Overridable Function lowerKey(ByVal key As K) As K Implements NavigableMap(Of K, V).lowerKey
				Return nm.lowerKey(key)
			End Function
			Public Overridable Function floorKey(ByVal key As K) As K Implements NavigableMap(Of K, V).floorKey
				Return nm.floorKey(key)
			End Function
			Public Overridable Function ceilingKey(ByVal key As K) As K Implements NavigableMap(Of K, V).ceilingKey
				Return nm.ceilingKey(key)
			End Function
			Public Overridable Function higherKey(ByVal key As K) As K Implements NavigableMap(Of K, V).higherKey
				Return nm.higherKey(key)
			End Function

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
			Public Overridable Function lowerEntry(ByVal key As K) As Entry(Of K, V)
				Dim lower As Entry(Of K, V) = CType(nm.lowerEntry(key), Entry(Of K, V))
				Return If(Nothing IsNot lower, New UnmodifiableEntrySet.UnmodifiableEntry(Of )(lower), Nothing)
			End Function

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
			Public Overridable Function floorEntry(ByVal key As K) As Entry(Of K, V)
				Dim floor As Entry(Of K, V) = CType(nm.floorEntry(key), Entry(Of K, V))
				Return If(Nothing IsNot floor, New UnmodifiableEntrySet.UnmodifiableEntry(Of )(floor), Nothing)
			End Function

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
			Public Overridable Function ceilingEntry(ByVal key As K) As Entry(Of K, V)
				Dim ceiling As Entry(Of K, V) = CType(nm.ceilingEntry(key), Entry(Of K, V))
				Return If(Nothing IsNot ceiling, New UnmodifiableEntrySet.UnmodifiableEntry(Of )(ceiling), Nothing)
			End Function


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
			Public Overridable Function higherEntry(ByVal key As K) As Entry(Of K, V)
				Dim higher As Entry(Of K, V) = CType(nm.higherEntry(key), Entry(Of K, V))
				Return If(Nothing IsNot higher, New UnmodifiableEntrySet.UnmodifiableEntry(Of )(higher), Nothing)
			End Function

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
			Public Overridable Function firstEntry() As Entry(Of K, V)
				Dim first As Entry(Of K, V) = CType(nm.firstEntry(), Entry(Of K, V))
				Return If(Nothing IsNot first, New UnmodifiableEntrySet.UnmodifiableEntry(Of )(first), Nothing)
			End Function

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
			Public Overridable Function lastEntry() As Entry(Of K, V)
				Dim last As Entry(Of K, V) = CType(nm.lastEntry(), Entry(Of K, V))
				Return If(Nothing IsNot last, New UnmodifiableEntrySet.UnmodifiableEntry(Of )(last), Nothing)
			End Function

			Public Overridable Function pollFirstEntry() As Entry(Of K, V)
										 Throw New UnsupportedOperationException
			End Function
			Public Overridable Function pollLastEntry() As Entry(Of K, V)
										 Throw New UnsupportedOperationException
			End Function
			Public Overridable Function descendingMap() As NavigableMap(Of K, V) Implements NavigableMap(Of K, V).descendingMap
							   Return unmodifiableNavigableMap(nm.descendingMap())
			End Function
			Public Overridable Function navigableKeySet() As NavigableSet(Of K) Implements NavigableMap(Of K, V).navigableKeySet
							 Return unmodifiableNavigableSet(nm.navigableKeySet())
			End Function
			Public Overridable Function descendingKeySet() As NavigableSet(Of K) Implements NavigableMap(Of K, V).descendingKeySet
							Return unmodifiableNavigableSet(nm.descendingKeySet())
			End Function

			Public Overridable Function subMap(ByVal fromKey As K, ByVal fromInclusive As Boolean, ByVal toKey As K, ByVal toInclusive As Boolean) As NavigableMap(Of K, V) Implements NavigableMap(Of K, V).subMap
				Return unmodifiableNavigableMap(nm.subMap(fromKey, fromInclusive, toKey, toInclusive))
			End Function

			Public Overridable Function headMap(ByVal toKey As K, ByVal inclusive As Boolean) As NavigableMap(Of K, V) Implements NavigableMap(Of K, V).headMap
					 Return unmodifiableNavigableMap(nm.headMap(toKey, inclusive))
			End Function
			Public Overridable Function tailMap(ByVal fromKey As K, ByVal inclusive As Boolean) As NavigableMap(Of K, V) Implements NavigableMap(Of K, V).tailMap
				   Return unmodifiableNavigableMap(nm.tailMap(fromKey, inclusive))
			End Function
		End Class

		' Synch Wrappers

		''' <summary>
		''' Returns a synchronized (thread-safe) collection backed by the specified
		''' collection.  In order to guarantee serial access, it is critical that
		''' <strong>all</strong> access to the backing collection is accomplished
		''' through the returned collection.<p>
		''' 
		''' It is imperative that the user manually synchronize on the returned
		''' collection when traversing it via <seealso cref="Iterator"/>, <seealso cref="Spliterator"/>
		''' or <seealso cref="Stream"/>:
		''' <pre>
		'''  Collection c = Collections.synchronizedCollection(myCollection);
		'''     ...
		'''  synchronized (c) {
		'''      Iterator i = c.iterator(); // Must be in the synchronized block
		'''      while (i.hasNext())
		'''         foo(i.next());
		'''  }
		''' </pre>
		''' Failure to follow this advice may result in non-deterministic behavior.
		''' 
		''' <p>The returned collection does <i>not</i> pass the {@code hashCode}
		''' and {@code equals} operations through to the backing collection, but
		''' relies on {@code Object}'s equals and hashCode methods.  This is
		''' necessary to preserve the contracts of these operations in the case
		''' that the backing collection is a set or a list.<p>
		''' 
		''' The returned collection will be serializable if the specified collection
		''' is serializable.
		''' </summary>
		''' @param  <T> the class of the objects in the collection </param>
		''' <param name="c"> the collection to be "wrapped" in a synchronized collection. </param>
		''' <returns> a synchronized view of the specified collection. </returns>
		Public Shared Function synchronizedCollection(Of T)(ByVal c As Collection(Of T)) As Collection(Of T)
			Return New SynchronizedCollection(Of )(c)
		End Function

		Friend Shared Function synchronizedCollection(Of T)(ByVal c As Collection(Of T), ByVal mutex As Object) As Collection(Of T)
			Return New SynchronizedCollection(Of )(c, mutex)
		End Function

		''' <summary>
		''' @serial include
		''' </summary>
		<Serializable> _
		Friend Class SynchronizedCollection(Of E)
			Implements Collection(Of E)

			Private Const serialVersionUID As Long = 3053995032091335093L

			Friend ReadOnly c As Collection(Of E) ' Backing Collection
			Friend ReadOnly mutex As Object ' Object on which to synchronize

			Friend Sub New(ByVal c As Collection(Of E))
				Me.c = Objects.requireNonNull(c)
				mutex = Me
			End Sub

			Friend Sub New(ByVal c As Collection(Of E), ByVal mutex As Object)
				Me.c = Objects.requireNonNull(c)
				Me.mutex = Objects.requireNonNull(mutex)
			End Sub

			Public Overridable Function size() As Integer Implements Collection(Of E).size
				SyncLock mutex
					Return c.size()
				End SyncLock
			End Function
			Public Overridable Property empty As Boolean Implements Collection(Of E).isEmpty
				Get
					SyncLock mutex
						Return c.empty
					End SyncLock
				End Get
			End Property
			Public Overridable Function contains(ByVal o As Object) As Boolean Implements Collection(Of E).contains
				SyncLock mutex
					Return c.contains(o)
				End SyncLock
			End Function
			Public Overridable Function toArray() As Object() Implements Collection(Of E).toArray
				SyncLock mutex
					Return c.ToArray()
				End SyncLock
			End Function
			Public Overridable Function toArray(Of T)(ByVal a As T()) As T() Implements Collection(Of E).toArray
				SyncLock mutex
					Return c.ToArray(a)
				End SyncLock
			End Function

			Public Overridable Function [iterator]() As [Iterator](Of E) Implements Collection(Of E).iterator
				Return c.GetEnumerator() ' Must be manually synched by user!
			End Function

			Public Overridable Function add(ByVal e As E) As Boolean
				SyncLock mutex
					Return c.add(e)
				End SyncLock
			End Function
			Public Overridable Function remove(ByVal o As Object) As Boolean Implements Collection(Of E).remove
				SyncLock mutex
					Return c.remove(o)
				End SyncLock
			End Function

			Public Overridable Function containsAll(Of T1)(ByVal coll As Collection(Of T1)) As Boolean Implements Collection(Of E).containsAll
				SyncLock mutex
					Return c.containsAll(coll)
				End SyncLock
			End Function
			Public Overridable Function addAll(Of T1 As E)(ByVal coll As Collection(Of T1)) As Boolean Implements Collection(Of E).addAll
				SyncLock mutex
					Return c.addAll(coll)
				End SyncLock
			End Function
			Public Overridable Function removeAll(Of T1)(ByVal coll As Collection(Of T1)) As Boolean Implements Collection(Of E).removeAll
				SyncLock mutex
					Return c.removeAll(coll)
				End SyncLock
			End Function
			Public Overridable Function retainAll(Of T1)(ByVal coll As Collection(Of T1)) As Boolean Implements Collection(Of E).retainAll
				SyncLock mutex
					Return c.retainAll(coll)
				End SyncLock
			End Function
			Public Overridable Sub clear() Implements Collection(Of E).clear
				SyncLock mutex
					c.clear()
				End SyncLock
			End Sub
			Public Overrides Function ToString() As String
				SyncLock mutex
					Return c.ToString()
				End SyncLock
			End Function
			' Override default methods in Collection
'JAVA TO VB CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
			Public Overrides Sub forEach(Of T1)(ByVal consumer As java.util.function.Consumer(Of T1))
				SyncLock mutex
					c.forEach(consumer)
				End SyncLock
			End Sub
'JAVA TO VB CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
			Public Overrides Function removeIf(Of T1)(ByVal filter As java.util.function.Predicate(Of T1)) As Boolean Implements Collection(Of E).removeIf
				SyncLock mutex
					Return c.removeIf(filter)
				End SyncLock
			End Function
			Public Overrides Function spliterator() As Spliterator(Of E) Implements Collection(Of E).spliterator
				Return c.spliterator() ' Must be manually synched by user!
			End Function
			Public Overrides Function stream() As java.util.stream.Stream(Of E) Implements Collection(Of E).stream
				Return c.stream() ' Must be manually synched by user!
			End Function
			Public Overrides Function parallelStream() As java.util.stream.Stream(Of E) Implements Collection(Of E).parallelStream
				Return c.parallelStream() ' Must be manually synched by user!
			End Function
			Private Sub writeObject(ByVal s As java.io.ObjectOutputStream)
				SyncLock mutex
					s.defaultWriteObject()
				End SyncLock
			End Sub
		End Class

		''' <summary>
		''' Returns a synchronized (thread-safe) set backed by the specified
		''' set.  In order to guarantee serial access, it is critical that
		''' <strong>all</strong> access to the backing set is accomplished
		''' through the returned set.<p>
		''' 
		''' It is imperative that the user manually synchronize on the returned
		''' set when iterating over it:
		''' <pre>
		'''  Set s = Collections.synchronizedSet(new HashSet());
		'''      ...
		'''  synchronized (s) {
		'''      Iterator i = s.iterator(); // Must be in the synchronized block
		'''      while (i.hasNext())
		'''          foo(i.next());
		'''  }
		''' </pre>
		''' Failure to follow this advice may result in non-deterministic behavior.
		''' 
		''' <p>The returned set will be serializable if the specified set is
		''' serializable.
		''' </summary>
		''' @param  <T> the class of the objects in the set </param>
		''' <param name="s"> the set to be "wrapped" in a synchronized set. </param>
		''' <returns> a synchronized view of the specified set. </returns>
		Public Shared Function synchronizedSet(Of T)(ByVal s As [Set](Of T)) As [Set](Of T)
			Return New SynchronizedSet(Of )(s)
		End Function

		Friend Shared Function synchronizedSet(Of T)(ByVal s As [Set](Of T), ByVal mutex As Object) As [Set](Of T)
			Return New SynchronizedSet(Of )(s, mutex)
		End Function

		''' <summary>
		''' @serial include
		''' </summary>
		Friend Class SynchronizedSet(Of E)
			Inherits SynchronizedCollection(Of E)
			Implements Set(Of E)

			Private Const serialVersionUID As Long = 487447009682186044L

			Friend Sub New(ByVal s As [Set](Of E))
				MyBase.New(s)
			End Sub
			Friend Sub New(ByVal s As [Set](Of E), ByVal mutex As Object)
				MyBase.New(s, mutex)
			End Sub

			Public Overrides Function Equals(ByVal o As Object) As Boolean
				If Me Is o Then Return True
				SyncLock mutex
					Return c.Equals(o)
				End SyncLock
			End Function
			Public Overrides Function GetHashCode() As Integer
				SyncLock mutex
					Return c.GetHashCode()
				End SyncLock
			End Function
		End Class

		''' <summary>
		''' Returns a synchronized (thread-safe) sorted set backed by the specified
		''' sorted set.  In order to guarantee serial access, it is critical that
		''' <strong>all</strong> access to the backing sorted set is accomplished
		''' through the returned sorted set (or its views).<p>
		''' 
		''' It is imperative that the user manually synchronize on the returned
		''' sorted set when iterating over it or any of its <tt>subSet</tt>,
		''' <tt>headSet</tt>, or <tt>tailSet</tt> views.
		''' <pre>
		'''  SortedSet s = Collections.synchronizedSortedSet(new TreeSet());
		'''      ...
		'''  synchronized (s) {
		'''      Iterator i = s.iterator(); // Must be in the synchronized block
		'''      while (i.hasNext())
		'''          foo(i.next());
		'''  }
		''' </pre>
		''' or:
		''' <pre>
		'''  SortedSet s = Collections.synchronizedSortedSet(new TreeSet());
		'''  SortedSet s2 = s.headSet(foo);
		'''      ...
		'''  synchronized (s) {  // Note: s, not s2!!!
		'''      Iterator i = s2.iterator(); // Must be in the synchronized block
		'''      while (i.hasNext())
		'''          foo(i.next());
		'''  }
		''' </pre>
		''' Failure to follow this advice may result in non-deterministic behavior.
		''' 
		''' <p>The returned sorted set will be serializable if the specified
		''' sorted set is serializable.
		''' </summary>
		''' @param  <T> the class of the objects in the set </param>
		''' <param name="s"> the sorted set to be "wrapped" in a synchronized sorted set. </param>
		''' <returns> a synchronized view of the specified sorted set. </returns>
		Public Shared Function synchronizedSortedSet(Of T)(ByVal s As SortedSet(Of T)) As SortedSet(Of T)
			Return New SynchronizedSortedSet(Of )(s)
		End Function

		''' <summary>
		''' @serial include
		''' </summary>
		Friend Class SynchronizedSortedSet(Of E)
			Inherits SynchronizedSet(Of E)
			Implements SortedSet(Of E)

			Private Const serialVersionUID As Long = 8695801310862127406L

			Private ReadOnly ss As SortedSet(Of E)

			Friend Sub New(ByVal s As SortedSet(Of E))
				MyBase.New(s)
				ss = s
			End Sub
			Friend Sub New(ByVal s As SortedSet(Of E), ByVal mutex As Object)
				MyBase.New(s, mutex)
				ss = s
			End Sub

'JAVA TO VB CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
			Public Overridable Function comparator() As Comparator(Of ?) Implements SortedSet(Of E).comparator
				SyncLock mutex
					Return ss.comparator()
				End SyncLock
			End Function

			Public Overridable Function subSet(ByVal fromElement As E, ByVal toElement As E) As SortedSet(Of E)
				SyncLock mutex
					Return New SynchronizedSortedSet(Of )(ss.subSet(fromElement, toElement), mutex)
				End SyncLock
			End Function
			Public Overridable Function headSet(ByVal toElement As E) As SortedSet(Of E)
				SyncLock mutex
					Return New SynchronizedSortedSet(Of )(ss.headSet(toElement), mutex)
				End SyncLock
			End Function
			Public Overridable Function tailSet(ByVal fromElement As E) As SortedSet(Of E)
				SyncLock mutex
				   Return New SynchronizedSortedSet(Of )(ss.tailSet(fromElement),mutex)
				End SyncLock
			End Function

			Public Overridable Function first() As E Implements SortedSet(Of E).first
				SyncLock mutex
					Return ss.first()
				End SyncLock
			End Function
			Public Overridable Function last() As E Implements SortedSet(Of E).last
				SyncLock mutex
					Return ss.last()
				End SyncLock
			End Function
		End Class

		''' <summary>
		''' Returns a synchronized (thread-safe) navigable set backed by the
		''' specified navigable set.  In order to guarantee serial access, it is
		''' critical that <strong>all</strong> access to the backing navigable set is
		''' accomplished through the returned navigable set (or its views).<p>
		''' 
		''' It is imperative that the user manually synchronize on the returned
		''' navigable set when iterating over it or any of its {@code subSet},
		''' {@code headSet}, or {@code tailSet} views.
		''' <pre>
		'''  NavigableSet s = Collections.synchronizedNavigableSet(new TreeSet());
		'''      ...
		'''  synchronized (s) {
		'''      Iterator i = s.iterator(); // Must be in the synchronized block
		'''      while (i.hasNext())
		'''          foo(i.next());
		'''  }
		''' </pre>
		''' or:
		''' <pre>
		'''  NavigableSet s = Collections.synchronizedNavigableSet(new TreeSet());
		'''  NavigableSet s2 = s.headSet(foo, true);
		'''      ...
		'''  synchronized (s) {  // Note: s, not s2!!!
		'''      Iterator i = s2.iterator(); // Must be in the synchronized block
		'''      while (i.hasNext())
		'''          foo(i.next());
		'''  }
		''' </pre>
		''' Failure to follow this advice may result in non-deterministic behavior.
		''' 
		''' <p>The returned navigable set will be serializable if the specified
		''' navigable set is serializable.
		''' </summary>
		''' @param  <T> the class of the objects in the set </param>
		''' <param name="s"> the navigable set to be "wrapped" in a synchronized navigable
		''' set </param>
		''' <returns> a synchronized view of the specified navigable set
		''' @since 1.8 </returns>
		Public Shared Function synchronizedNavigableSet(Of T)(ByVal s As NavigableSet(Of T)) As NavigableSet(Of T)
			Return New SynchronizedNavigableSet(Of )(s)
		End Function

		''' <summary>
		''' @serial include
		''' </summary>
		Friend Class SynchronizedNavigableSet(Of E)
			Inherits SynchronizedSortedSet(Of E)
			Implements NavigableSet(Of E)

			Private Const serialVersionUID As Long = -5505529816273629798L

			Private ReadOnly ns As NavigableSet(Of E)

			Friend Sub New(ByVal s As NavigableSet(Of E))
				MyBase.New(s)
				ns = s
			End Sub

			Friend Sub New(ByVal s As NavigableSet(Of E), ByVal mutex As Object)
				MyBase.New(s, mutex)
				ns = s
			End Sub
			Public Overridable Function lower(ByVal e As E) As E
				SyncLock mutex
					Return ns.lower(e)
				End SyncLock
			End Function
			Public Overridable Function floor(ByVal e As E) As E
				SyncLock mutex
					Return ns.floor(e)
				End SyncLock
			End Function
			Public Overridable Function ceiling(ByVal e As E) As E
				SyncLock mutex
					Return ns.ceiling(e)
				End SyncLock
			End Function
			Public Overridable Function higher(ByVal e As E) As E
				SyncLock mutex
					Return ns.higher(e)
				End SyncLock
			End Function
			Public Overridable Function pollFirst() As E Implements NavigableSet(Of E).pollFirst
				SyncLock mutex
					Return ns.pollFirst()
				End SyncLock
			End Function
			Public Overridable Function pollLast() As E Implements NavigableSet(Of E).pollLast
				SyncLock mutex
					Return ns.pollLast()
				End SyncLock
			End Function

			Public Overridable Function descendingSet() As NavigableSet(Of E) Implements NavigableSet(Of E).descendingSet
				SyncLock mutex
					Return New SynchronizedNavigableSet(Of )(ns.descendingSet(), mutex)
				End SyncLock
			End Function

			Public Overridable Function descendingIterator() As [Iterator](Of E) Implements NavigableSet(Of E).descendingIterator
						 SyncLock mutex
							 Return descendingSet().GetEnumerator()
						 End SyncLock
			End Function

			Public Overridable Function subSet(ByVal fromElement As E, ByVal toElement As E) As NavigableSet(Of E)
				SyncLock mutex
					Return New SynchronizedNavigableSet(Of )(ns.subSet(fromElement, True, toElement, False), mutex)
				End SyncLock
			End Function
			Public Overridable Function headSet(ByVal toElement As E) As NavigableSet(Of E)
				SyncLock mutex
					Return New SynchronizedNavigableSet(Of )(ns.headSet(toElement, False), mutex)
				End SyncLock
			End Function
			Public Overridable Function tailSet(ByVal fromElement As E) As NavigableSet(Of E)
				SyncLock mutex
					Return New SynchronizedNavigableSet(Of )(ns.tailSet(fromElement, True), mutex)
				End SyncLock
			End Function

			Public Overridable Function subSet(ByVal fromElement As E, ByVal fromInclusive As Boolean, ByVal toElement As E, ByVal toInclusive As Boolean) As NavigableSet(Of E)
				SyncLock mutex
					Return New SynchronizedNavigableSet(Of )(ns.subSet(fromElement, fromInclusive, toElement, toInclusive), mutex)
				End SyncLock
			End Function

			Public Overridable Function headSet(ByVal toElement As E, ByVal inclusive As Boolean) As NavigableSet(Of E)
				SyncLock mutex
					Return New SynchronizedNavigableSet(Of )(ns.headSet(toElement, inclusive), mutex)
				End SyncLock
			End Function

			Public Overridable Function tailSet(ByVal fromElement As E, ByVal inclusive As Boolean) As NavigableSet(Of E)
				SyncLock mutex
					Return New SynchronizedNavigableSet(Of )(ns.tailSet(fromElement, inclusive), mutex)
				End SyncLock
			End Function
		End Class

		''' <summary>
		''' Returns a synchronized (thread-safe) list backed by the specified
		''' list.  In order to guarantee serial access, it is critical that
		''' <strong>all</strong> access to the backing list is accomplished
		''' through the returned list.<p>
		''' 
		''' It is imperative that the user manually synchronize on the returned
		''' list when iterating over it:
		''' <pre>
		'''  List list = Collections.synchronizedList(new ArrayList());
		'''      ...
		'''  synchronized (list) {
		'''      Iterator i = list.iterator(); // Must be in synchronized block
		'''      while (i.hasNext())
		'''          foo(i.next());
		'''  }
		''' </pre>
		''' Failure to follow this advice may result in non-deterministic behavior.
		''' 
		''' <p>The returned list will be serializable if the specified list is
		''' serializable.
		''' </summary>
		''' @param  <T> the class of the objects in the list </param>
		''' <param name="list"> the list to be "wrapped" in a synchronized list. </param>
		''' <returns> a synchronized view of the specified list. </returns>
		Public Shared Function synchronizedList(Of T)(ByVal list As List(Of T)) As List(Of T)
			Return (If(TypeOf list Is RandomAccess, New SynchronizedRandomAccessList(Of )(list), New SynchronizedList(Of )(list)))
		End Function

		Friend Shared Function synchronizedList(Of T)(ByVal list As List(Of T), ByVal mutex As Object) As List(Of T)
			Return (If(TypeOf list Is RandomAccess, New SynchronizedRandomAccessList(Of )(list, mutex), New SynchronizedList(Of )(list, mutex)))
		End Function

		''' <summary>
		''' @serial include
		''' </summary>
		Friend Class SynchronizedList(Of E)
			Inherits SynchronizedCollection(Of E)
			Implements List(Of E)

			Private Const serialVersionUID As Long = -7754090372962971524L

			Friend ReadOnly list As List(Of E)

			Friend Sub New(ByVal list As List(Of E))
				MyBase.New(list)
				Me.list = list
			End Sub
			Friend Sub New(ByVal list As List(Of E), ByVal mutex As Object)
				MyBase.New(list, mutex)
				Me.list = list
			End Sub

			Public Overrides Function Equals(ByVal o As Object) As Boolean
				If Me Is o Then Return True
				SyncLock mutex
					Return list.Equals(o)
				End SyncLock
			End Function
			Public Overrides Function GetHashCode() As Integer
				SyncLock mutex
					Return list.GetHashCode()
				End SyncLock
			End Function

			Public Overridable Function [get](ByVal index As Integer) As E Implements List(Of E).get
				SyncLock mutex
					Return list.get(index)
				End SyncLock
			End Function
			Public Overridable Function [set](ByVal index As Integer, ByVal element As E) As E
				SyncLock mutex
					Return list.set(index, element)
				End SyncLock
			End Function
			Public Overridable Sub add(ByVal index As Integer, ByVal element As E)
				SyncLock mutex
					list.add(index, element)
				End SyncLock
			End Sub
			Public Overridable Function remove(ByVal index As Integer) As E Implements List(Of E).remove
				SyncLock mutex
					Return list.remove(index)
				End SyncLock
			End Function

			Public Overridable Function indexOf(ByVal o As Object) As Integer Implements List(Of E).indexOf
				SyncLock mutex
					Return list.IndexOf(o)
				End SyncLock
			End Function
			Public Overridable Function lastIndexOf(ByVal o As Object) As Integer Implements List(Of E).lastIndexOf
				SyncLock mutex
					Return list.LastIndexOf(o)
				End SyncLock
			End Function

			Public Overridable Function addAll(Of T1 As E)(ByVal index As Integer, ByVal c As Collection(Of T1)) As Boolean Implements List(Of E).addAll
				SyncLock mutex
					Return list.addAll(index, c)
				End SyncLock
			End Function

			Public Overridable Function listIterator() As ListIterator(Of E) Implements List(Of E).listIterator
				Return list.GetEnumerator() ' Must be manually synched by user
			End Function

			Public Overridable Function listIterator(ByVal index As Integer) As ListIterator(Of E) Implements List(Of E).listIterator
				Return list.listIterator(index) ' Must be manually synched by user
			End Function

			Public Overridable Function subList(ByVal fromIndex As Integer, ByVal toIndex As Integer) As List(Of E) Implements List(Of E).subList
				SyncLock mutex
					Return New SynchronizedList(Of )(list.subList(fromIndex, toIndex), mutex)
				End SyncLock
			End Function

			Public Overrides Sub replaceAll(ByVal [operator] As java.util.function.UnaryOperator(Of E)) Implements List(Of E).replaceAll
				SyncLock mutex
					list.replaceAll([operator])
				End SyncLock
			End Sub
'JAVA TO VB CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
			Public Overrides Sub sort(Of T1)(ByVal c As Comparator(Of T1)) Implements List(Of E).sort
				SyncLock mutex
					list.sort(c)
				End SyncLock
			End Sub

			''' <summary>
			''' SynchronizedRandomAccessList instances are serialized as
			''' SynchronizedList instances to allow them to be deserialized
			''' in pre-1.4 JREs (which do not have SynchronizedRandomAccessList).
			''' This method inverts the transformation.  As a beneficial
			''' side-effect, it also grafts the RandomAccess marker onto
			''' SynchronizedList instances that were serialized in pre-1.4 JREs.
			''' 
			''' Note: Unfortunately, SynchronizedRandomAccessList instances
			''' serialized in 1.4.1 and deserialized in 1.4 will become
			''' SynchronizedList instances, as this method was missing in 1.4.
			''' </summary>
			Private Function readResolve() As Object
				Return (TypeOf list Is RandomAccess ? New SynchronizedRandomAccessList(Of )(list)
						: Me)
			End Function
		End Class

		''' <summary>
		''' @serial include
		''' </summary>
		Friend Class SynchronizedRandomAccessList(Of E)
			Inherits SynchronizedList(Of E)
			Implements RandomAccess

			Friend Sub New(ByVal list As List(Of E))
				MyBase.New(list)
			End Sub

			Friend Sub New(ByVal list As List(Of E), ByVal mutex As Object)
				MyBase.New(list, mutex)
			End Sub

			Public Overridable Function subList(ByVal fromIndex As Integer, ByVal toIndex As Integer) As List(Of E)
				SyncLock mutex
					Return New SynchronizedRandomAccessList(Of )(list.subList(fromIndex, toIndex), mutex)
				End SyncLock
			End Function

			Private Const serialVersionUID As Long = 1530674583602358482L

			''' <summary>
			''' Allows instances to be deserialized in pre-1.4 JREs (which do
			''' not have SynchronizedRandomAccessList).  SynchronizedList has
			''' a readResolve method that inverts this transformation upon
			''' deserialization.
			''' </summary>
			Private Function writeReplace() As Object
				Return New SynchronizedList(Of )(list)
			End Function
		End Class

		''' <summary>
		''' Returns a synchronized (thread-safe) map backed by the specified
		''' map.  In order to guarantee serial access, it is critical that
		''' <strong>all</strong> access to the backing map is accomplished
		''' through the returned map.<p>
		''' 
		''' It is imperative that the user manually synchronize on the returned
		''' map when iterating over any of its collection views:
		''' <pre>
		'''  Map m = Collections.synchronizedMap(new HashMap());
		'''      ...
		'''  Set s = m.keySet();  // Needn't be in synchronized block
		'''      ...
		'''  synchronized (m) {  // Synchronizing on m, not s!
		'''      Iterator i = s.iterator(); // Must be in synchronized block
		'''      while (i.hasNext())
		'''          foo(i.next());
		'''  }
		''' </pre>
		''' Failure to follow this advice may result in non-deterministic behavior.
		''' 
		''' <p>The returned map will be serializable if the specified map is
		''' serializable.
		''' </summary>
		''' @param <K> the class of the map keys </param>
		''' @param <V> the class of the map values </param>
		''' <param name="m"> the map to be "wrapped" in a synchronized map. </param>
		''' <returns> a synchronized view of the specified map. </returns>
		Public Shared Function synchronizedMap(Of K, V)(ByVal m As Map(Of K, V)) As Map(Of K, V)
			Return New SynchronizedMap(Of )(m)
		End Function

		''' <summary>
		''' @serial include
		''' </summary>
		<Serializable> _
		Private Class SynchronizedMap(Of K, V)
			Implements Map(Of K, V)

			Private Const serialVersionUID As Long = 1978198479659022715L

			Private ReadOnly m As Map(Of K, V) ' Backing Map
			Friend ReadOnly mutex As Object ' Object on which to synchronize

			Friend Sub New(ByVal m As Map(Of K, V))
				Me.m = Objects.requireNonNull(m)
				mutex = Me
			End Sub

			Friend Sub New(ByVal m As Map(Of K, V), ByVal mutex As Object)
				Me.m = m
				Me.mutex = mutex
			End Sub

			Public Overridable Function size() As Integer Implements Map(Of K, V).size
				SyncLock mutex
					Return m.size()
				End SyncLock
			End Function
			Public Overridable Property empty As Boolean Implements Map(Of K, V).isEmpty
				Get
					SyncLock mutex
						Return m.empty
					End SyncLock
				End Get
			End Property
			Public Overridable Function containsKey(ByVal key As Object) As Boolean Implements Map(Of K, V).containsKey
				SyncLock mutex
					Return m.containsKey(key)
				End SyncLock
			End Function
			Public Overridable Function containsValue(ByVal value As Object) As Boolean Implements Map(Of K, V).containsValue
				SyncLock mutex
					Return m.containsValue(value)
				End SyncLock
			End Function
			Public Overridable Function [get](ByVal key As Object) As V Implements Map(Of K, V).get
				SyncLock mutex
					Return m.get(key)
				End SyncLock
			End Function

			Public Overridable Function put(ByVal key As K, ByVal value As V) As V Implements Map(Of K, V).put
				SyncLock mutex
					Return m.put(key, value)
				End SyncLock
			End Function
			Public Overridable Function remove(ByVal key As Object) As V Implements Map(Of K, V).remove
				SyncLock mutex
					Return m.remove(key)
				End SyncLock
			End Function
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
			Public Overridable Sub putAll(Of T1 As K, ? As V)(ByVal map As Map(Of T1)) Implements Map(Of K, V).putAll
				SyncLock mutex
					m.putAll(map)
				End SyncLock
			End Sub
			Public Overridable Sub clear() Implements Map(Of K, V).clear
				SyncLock mutex
					m.clear()
				End SyncLock
			End Sub

			<NonSerialized> _
			Private keySet_Renamed As [Set](Of K)
			<NonSerialized> _
			Private entrySet_Renamed As [Set](Of KeyValuePair(Of K, V))
			<NonSerialized> _
			Private values_Renamed As Collection(Of V)

			Public Overridable Function keySet() As [Set](Of K) Implements Map(Of K, V).keySet
				SyncLock mutex
					If keySet_Renamed Is Nothing Then keySet_Renamed = New SynchronizedSet(Of )(m.Keys, mutex)
					Return keySet_Renamed
				End SyncLock
			End Function

			Public Overridable Function entrySet() As [Set](Of KeyValuePair(Of K, V)) Implements Map(Of K, V).entrySet
				SyncLock mutex
					If entrySet_Renamed Is Nothing Then entrySet_Renamed = New SynchronizedSet(Of )(m.entrySet(), mutex)
					Return entrySet_Renamed
				End SyncLock
			End Function

			Public Overridable Function values() As Collection(Of V) Implements Map(Of K, V).values
				SyncLock mutex
					If values_Renamed Is Nothing Then values_Renamed = New SynchronizedCollection(Of )(m.values(), mutex)
					Return values_Renamed
				End SyncLock
			End Function

			Public Overrides Function Equals(ByVal o As Object) As Boolean
				If Me Is o Then Return True
				SyncLock mutex
					Return m.Equals(o)
				End SyncLock
			End Function
			Public Overrides Function GetHashCode() As Integer
				SyncLock mutex
					Return m.GetHashCode()
				End SyncLock
			End Function
			Public Overrides Function ToString() As String
				SyncLock mutex
					Return m.ToString()
				End SyncLock
			End Function

			' Override default methods in Map
			Public Overrides Function getOrDefault(ByVal k As Object, ByVal defaultValue As V) As V Implements Map(Of K, V).getOrDefault
				SyncLock mutex
					Return m.getOrDefault(k, defaultValue)
				End SyncLock
			End Function
'JAVA TO VB CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
			Public Overrides Sub forEach(Of T1)(ByVal action As java.util.function.BiConsumer(Of T1)) Implements Map(Of K, V).forEach
				SyncLock mutex
					m.forEach(action)
				End SyncLock
			End Sub
'JAVA TO VB CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
			Public Overrides Sub replaceAll(Of T1 As V)(ByVal [function] As java.util.function.BiFunction(Of T1)) Implements Map(Of K, V).replaceAll
				SyncLock mutex
					m.replaceAll([function])
				End SyncLock
			End Sub
			Public Overrides Function putIfAbsent(ByVal key As K, ByVal value As V) As V Implements Map(Of K, V).putIfAbsent
				SyncLock mutex
					Return m.putIfAbsent(key, value)
				End SyncLock
			End Function
			Public Overrides Function remove(ByVal key As Object, ByVal value As Object) As Boolean Implements Map(Of K, V).remove
				SyncLock mutex
					Return m.remove(key, value)
				End SyncLock
			End Function
			Public Overrides Function replace(ByVal key As K, ByVal oldValue As V, ByVal newValue As V) As Boolean Implements Map(Of K, V).replace
				SyncLock mutex
					Return m.replace(key, oldValue, newValue)
				End SyncLock
			End Function
			Public Overrides Function replace(ByVal key As K, ByVal value As V) As V Implements Map(Of K, V).replace
				SyncLock mutex
					Return m.replace(key, value)
				End SyncLock
			End Function
'JAVA TO VB CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
			Public Overrides Function computeIfAbsent(Of T1 As V)(ByVal key As K, ByVal mappingFunction As java.util.function.Function(Of T1)) As V Implements Map(Of K, V).computeIfAbsent
				SyncLock mutex
					Return m.computeIfAbsent(key, mappingFunction)
				End SyncLock
			End Function
'JAVA TO VB CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
			Public Overrides Function computeIfPresent(Of T1 As V)(ByVal key As K, ByVal remappingFunction As java.util.function.BiFunction(Of T1)) As V Implements Map(Of K, V).computeIfPresent
				SyncLock mutex
					Return m.computeIfPresent(key, remappingFunction)
				End SyncLock
			End Function
'JAVA TO VB CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
			Public Overrides Function compute(Of T1 As V)(ByVal key As K, ByVal remappingFunction As java.util.function.BiFunction(Of T1)) As V Implements Map(Of K, V).compute
				SyncLock mutex
					Return m.compute(key, remappingFunction)
				End SyncLock
			End Function
'JAVA TO VB CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
			Public Overrides Function merge(Of T1 As V)(ByVal key As K, ByVal value As V, ByVal remappingFunction As java.util.function.BiFunction(Of T1)) As V Implements Map(Of K, V).merge
				SyncLock mutex
					Return m.merge(key, value, remappingFunction)
				End SyncLock
			End Function

			Private Sub writeObject(ByVal s As java.io.ObjectOutputStream)
				SyncLock mutex
					s.defaultWriteObject()
				End SyncLock
			End Sub
		End Class

		''' <summary>
		''' Returns a synchronized (thread-safe) sorted map backed by the specified
		''' sorted map.  In order to guarantee serial access, it is critical that
		''' <strong>all</strong> access to the backing sorted map is accomplished
		''' through the returned sorted map (or its views).<p>
		''' 
		''' It is imperative that the user manually synchronize on the returned
		''' sorted map when iterating over any of its collection views, or the
		''' collections views of any of its <tt>subMap</tt>, <tt>headMap</tt> or
		''' <tt>tailMap</tt> views.
		''' <pre>
		'''  SortedMap m = Collections.synchronizedSortedMap(new TreeMap());
		'''      ...
		'''  Set s = m.keySet();  // Needn't be in synchronized block
		'''      ...
		'''  synchronized (m) {  // Synchronizing on m, not s!
		'''      Iterator i = s.iterator(); // Must be in synchronized block
		'''      while (i.hasNext())
		'''          foo(i.next());
		'''  }
		''' </pre>
		''' or:
		''' <pre>
		'''  SortedMap m = Collections.synchronizedSortedMap(new TreeMap());
		'''  SortedMap m2 = m.subMap(foo, bar);
		'''      ...
		'''  Set s2 = m2.keySet();  // Needn't be in synchronized block
		'''      ...
		'''  synchronized (m) {  // Synchronizing on m, not m2 or s2!
		'''      Iterator i = s.iterator(); // Must be in synchronized block
		'''      while (i.hasNext())
		'''          foo(i.next());
		'''  }
		''' </pre>
		''' Failure to follow this advice may result in non-deterministic behavior.
		''' 
		''' <p>The returned sorted map will be serializable if the specified
		''' sorted map is serializable.
		''' </summary>
		''' @param <K> the class of the map keys </param>
		''' @param <V> the class of the map values </param>
		''' <param name="m"> the sorted map to be "wrapped" in a synchronized sorted map. </param>
		''' <returns> a synchronized view of the specified sorted map. </returns>
		Public Shared Function synchronizedSortedMap(Of K, V)(ByVal m As SortedMap(Of K, V)) As SortedMap(Of K, V)
			Return New SynchronizedSortedMap(Of )(m)
		End Function

		''' <summary>
		''' @serial include
		''' </summary>
		Friend Class SynchronizedSortedMap(Of K, V)
			Inherits SynchronizedMap(Of K, V)
			Implements SortedMap(Of K, V)

			Private Const serialVersionUID As Long = -8798146769416483793L

			Private ReadOnly sm As SortedMap(Of K, V)

			Friend Sub New(ByVal m As SortedMap(Of K, V))
				MyBase.New(m)
				sm = m
			End Sub
			Friend Sub New(ByVal m As SortedMap(Of K, V), ByVal mutex As Object)
				MyBase.New(m, mutex)
				sm = m
			End Sub

'JAVA TO VB CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
			Public Overridable Function comparator() As Comparator(Of ?) Implements SortedMap(Of K, V).comparator
				SyncLock mutex
					Return sm.comparator()
				End SyncLock
			End Function

			Public Overridable Function subMap(ByVal fromKey As K, ByVal toKey As K) As SortedMap(Of K, V) Implements SortedMap(Of K, V).subMap
				SyncLock mutex
					Return New SynchronizedSortedMap(Of )(sm.subMap(fromKey, toKey), mutex)
				End SyncLock
			End Function
			Public Overridable Function headMap(ByVal toKey As K) As SortedMap(Of K, V) Implements SortedMap(Of K, V).headMap
				SyncLock mutex
					Return New SynchronizedSortedMap(Of )(sm.headMap(toKey), mutex)
				End SyncLock
			End Function
			Public Overridable Function tailMap(ByVal fromKey As K) As SortedMap(Of K, V) Implements SortedMap(Of K, V).tailMap
				SyncLock mutex
				   Return New SynchronizedSortedMap(Of )(sm.tailMap(fromKey),mutex)
				End SyncLock
			End Function

			Public Overridable Function firstKey() As K Implements SortedMap(Of K, V).firstKey
				SyncLock mutex
					Return sm.firstKey()
				End SyncLock
			End Function
			Public Overridable Function lastKey() As K Implements SortedMap(Of K, V).lastKey
				SyncLock mutex
					Return sm.lastKey()
				End SyncLock
			End Function
		End Class

		''' <summary>
		''' Returns a synchronized (thread-safe) navigable map backed by the
		''' specified navigable map.  In order to guarantee serial access, it is
		''' critical that <strong>all</strong> access to the backing navigable map is
		''' accomplished through the returned navigable map (or its views).<p>
		''' 
		''' It is imperative that the user manually synchronize on the returned
		''' navigable map when iterating over any of its collection views, or the
		''' collections views of any of its {@code subMap}, {@code headMap} or
		''' {@code tailMap} views.
		''' <pre>
		'''  NavigableMap m = Collections.synchronizedNavigableMap(new TreeMap());
		'''      ...
		'''  Set s = m.keySet();  // Needn't be in synchronized block
		'''      ...
		'''  synchronized (m) {  // Synchronizing on m, not s!
		'''      Iterator i = s.iterator(); // Must be in synchronized block
		'''      while (i.hasNext())
		'''          foo(i.next());
		'''  }
		''' </pre>
		''' or:
		''' <pre>
		'''  NavigableMap m = Collections.synchronizedNavigableMap(new TreeMap());
		'''  NavigableMap m2 = m.subMap(foo, true, bar, false);
		'''      ...
		'''  Set s2 = m2.keySet();  // Needn't be in synchronized block
		'''      ...
		'''  synchronized (m) {  // Synchronizing on m, not m2 or s2!
		'''      Iterator i = s.iterator(); // Must be in synchronized block
		'''      while (i.hasNext())
		'''          foo(i.next());
		'''  }
		''' </pre>
		''' Failure to follow this advice may result in non-deterministic behavior.
		''' 
		''' <p>The returned navigable map will be serializable if the specified
		''' navigable map is serializable.
		''' </summary>
		''' @param <K> the class of the map keys </param>
		''' @param <V> the class of the map values </param>
		''' <param name="m"> the navigable map to be "wrapped" in a synchronized navigable
		'''              map </param>
		''' <returns> a synchronized view of the specified navigable map.
		''' @since 1.8 </returns>
		Public Shared Function synchronizedNavigableMap(Of K, V)(ByVal m As NavigableMap(Of K, V)) As NavigableMap(Of K, V)
			Return New SynchronizedNavigableMap(Of )(m)
		End Function

		''' <summary>
		''' A synchronized NavigableMap.
		''' 
		''' @serial include
		''' </summary>
		Friend Class SynchronizedNavigableMap(Of K, V)
			Inherits SynchronizedSortedMap(Of K, V)
			Implements NavigableMap(Of K, V)

			Private Const serialVersionUID As Long = 699392247599746807L

			Private ReadOnly nm As NavigableMap(Of K, V)

			Friend Sub New(ByVal m As NavigableMap(Of K, V))
				MyBase.New(m)
				nm = m
			End Sub
			Friend Sub New(ByVal m As NavigableMap(Of K, V), ByVal mutex As Object)
				MyBase.New(m, mutex)
				nm = m
			End Sub

			Public Overridable Function lowerEntry(ByVal key As K) As Entry(Of K, V)
								SyncLock mutex
									Return nm.lowerEntry(key)
								End SyncLock
			End Function
			Public Overridable Function lowerKey(ByVal key As K) As K Implements NavigableMap(Of K, V).lowerKey
								  SyncLock mutex
									  Return nm.lowerKey(key)
								  End SyncLock
			End Function
			Public Overridable Function floorEntry(ByVal key As K) As Entry(Of K, V)
								SyncLock mutex
									Return nm.floorEntry(key)
								End SyncLock
			End Function
			Public Overridable Function floorKey(ByVal key As K) As K Implements NavigableMap(Of K, V).floorKey
								  SyncLock mutex
									  Return nm.floorKey(key)
								  End SyncLock
			End Function
			Public Overridable Function ceilingEntry(ByVal key As K) As Entry(Of K, V)
							  SyncLock mutex
								  Return nm.ceilingEntry(key)
							  End SyncLock
			End Function
			Public Overridable Function ceilingKey(ByVal key As K) As K Implements NavigableMap(Of K, V).ceilingKey
								SyncLock mutex
									Return nm.ceilingKey(key)
								End SyncLock
			End Function
			Public Overridable Function higherEntry(ByVal key As K) As Entry(Of K, V)
							   SyncLock mutex
								   Return nm.higherEntry(key)
							   End SyncLock
			End Function
			Public Overridable Function higherKey(ByVal key As K) As K Implements NavigableMap(Of K, V).higherKey
								 SyncLock mutex
									 Return nm.higherKey(key)
								 End SyncLock
			End Function
			Public Overridable Function firstEntry() As Entry(Of K, V)
								   SyncLock mutex
									   Return nm.firstEntry()
								   End SyncLock
			End Function
			Public Overridable Function lastEntry() As Entry(Of K, V)
									SyncLock mutex
										Return nm.lastEntry()
									End SyncLock
			End Function
			Public Overridable Function pollFirstEntry() As Entry(Of K, V)
							   SyncLock mutex
								   Return nm.pollFirstEntry()
							   End SyncLock
			End Function
			Public Overridable Function pollLastEntry() As Entry(Of K, V)
								SyncLock mutex
									Return nm.pollLastEntry()
								End SyncLock
			End Function

			Public Overridable Function descendingMap() As NavigableMap(Of K, V) Implements NavigableMap(Of K, V).descendingMap
				SyncLock mutex
					Return New SynchronizedNavigableMap(Of )(nm.descendingMap(), mutex)
				End SyncLock
			End Function

			Public Overridable Function keySet() As NavigableSet(Of K)
				Return navigableKeySet()
			End Function

			Public Overridable Function navigableKeySet() As NavigableSet(Of K) Implements NavigableMap(Of K, V).navigableKeySet
				SyncLock mutex
					Return New SynchronizedNavigableSet(Of )(nm.navigableKeySet(), mutex)
				End SyncLock
			End Function

			Public Overridable Function descendingKeySet() As NavigableSet(Of K) Implements NavigableMap(Of K, V).descendingKeySet
				SyncLock mutex
					Return New SynchronizedNavigableSet(Of )(nm.descendingKeySet(), mutex)
				End SyncLock
			End Function


			Public Overridable Function subMap(ByVal fromKey As K, ByVal toKey As K) As SortedMap(Of K, V) Implements NavigableMap(Of K, V).subMap
				SyncLock mutex
					Return New SynchronizedNavigableMap(Of )(nm.subMap(fromKey, True, toKey, False), mutex)
				End SyncLock
			End Function
			Public Overridable Function headMap(ByVal toKey As K) As SortedMap(Of K, V) Implements NavigableMap(Of K, V).headMap
				SyncLock mutex
					Return New SynchronizedNavigableMap(Of )(nm.headMap(toKey, False), mutex)
				End SyncLock
			End Function
			Public Overridable Function tailMap(ByVal fromKey As K) As SortedMap(Of K, V) Implements NavigableMap(Of K, V).tailMap
				SyncLock mutex
			Return New SynchronizedNavigableMap(Of )(nm.tailMap(fromKey, True),mutex)
				End SyncLock
			End Function

			Public Overridable Function subMap(ByVal fromKey As K, ByVal fromInclusive As Boolean, ByVal toKey As K, ByVal toInclusive As Boolean) As NavigableMap(Of K, V) Implements NavigableMap(Of K, V).subMap
				SyncLock mutex
					Return New SynchronizedNavigableMap(Of )(nm.subMap(fromKey, fromInclusive, toKey, toInclusive), mutex)
				End SyncLock
			End Function

			Public Overridable Function headMap(ByVal toKey As K, ByVal inclusive As Boolean) As NavigableMap(Of K, V) Implements NavigableMap(Of K, V).headMap
				SyncLock mutex
					Return New SynchronizedNavigableMap(Of )(nm.headMap(toKey, inclusive), mutex)
				End SyncLock
			End Function

			Public Overridable Function tailMap(ByVal fromKey As K, ByVal inclusive As Boolean) As NavigableMap(Of K, V) Implements NavigableMap(Of K, V).tailMap
				SyncLock mutex
					Return New SynchronizedNavigableMap(Of )(nm.tailMap(fromKey, inclusive), mutex)
				End SyncLock
			End Function
		End Class

		' Dynamically typesafe collection wrappers

		''' <summary>
		''' Returns a dynamically typesafe view of the specified collection.
		''' Any attempt to insert an element of the wrong type will result in an
		''' immediate <seealso cref="ClassCastException"/>.  Assuming a collection
		''' contains no incorrectly typed elements prior to the time a
		''' dynamically typesafe view is generated, and that all subsequent
		''' access to the collection takes place through the view, it is
		''' <i>guaranteed</i> that the collection cannot contain an incorrectly
		''' typed element.
		''' 
		''' <p>The generics mechanism in the language provides compile-time
		''' (static) type checking, but it is possible to defeat this mechanism
		''' with unchecked casts.  Usually this is not a problem, as the compiler
		''' issues warnings on all such unchecked operations.  There are, however,
		''' times when static type checking alone is not sufficient.  For example,
		''' suppose a collection is passed to a third-party library and it is
		''' imperative that the library code not corrupt the collection by
		''' inserting an element of the wrong type.
		''' 
		''' <p>Another use of dynamically typesafe views is debugging.  Suppose a
		''' program fails with a {@code ClassCastException}, indicating that an
		''' incorrectly typed element was put into a parameterized collection.
		''' Unfortunately, the exception can occur at any time after the erroneous
		''' element is inserted, so it typically provides little or no information
		''' as to the real source of the problem.  If the problem is reproducible,
		''' one can quickly determine its source by temporarily modifying the
		''' program to wrap the collection with a dynamically typesafe view.
		''' For example, this declaration:
		'''  <pre> {@code
		'''     Collection<String> c = new HashSet<>();
		''' }</pre>
		''' may be replaced temporarily by this one:
		'''  <pre> {@code
		'''     Collection<String> c = Collections.checkedCollection(
		'''         new HashSet<>(), String.class);
		''' }</pre>
		''' Running the program again will cause it to fail at the point where
		''' an incorrectly typed element is inserted into the collection, clearly
		''' identifying the source of the problem.  Once the problem is fixed, the
		''' modified declaration may be reverted back to the original.
		''' 
		''' <p>The returned collection does <i>not</i> pass the hashCode and equals
		''' operations through to the backing collection, but relies on
		''' {@code Object}'s {@code equals} and {@code hashCode} methods.  This
		''' is necessary to preserve the contracts of these operations in the case
		''' that the backing collection is a set or a list.
		''' 
		''' <p>The returned collection will be serializable if the specified
		''' collection is serializable.
		''' 
		''' <p>Since {@code null} is considered to be a value of any reference
		''' type, the returned collection permits insertion of null elements
		''' whenever the backing collection does.
		''' </summary>
		''' @param <E> the class of the objects in the collection </param>
		''' <param name="c"> the collection for which a dynamically typesafe view is to be
		'''          returned </param>
		''' <param name="type"> the type of element that {@code c} is permitted to hold </param>
		''' <returns> a dynamically typesafe view of the specified collection
		''' @since 1.5 </returns>
		Public Shared Function checkedCollection(Of E)(ByVal c As Collection(Of E), ByVal type As Class) As Collection(Of E)
			Return New CheckedCollection(Of )(c, type)
		End Function

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
		Friend Shared Function zeroLengthArray(Of T)(ByVal type As Class) As T()
			Return CType(Array.newInstance(type, 0), T())
		End Function

		''' <summary>
		''' @serial include
		''' </summary>
		<Serializable> _
		Friend Class CheckedCollection(Of E)
			Implements Collection(Of E)

			Private Const serialVersionUID As Long = 1578914078182001775L

			Friend ReadOnly c As Collection(Of E)
			Friend ReadOnly type As Class

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
			Friend Overridable Function typeCheck(ByVal o As Object) As E
				If o IsNot Nothing AndAlso (Not type.isInstance(o)) Then Throw New ClassCastException(badElementMsg(o))
				Return CType(o, E)
			End Function

			Private Function badElementMsg(ByVal o As Object) As String
				Return "Attempt to insert " & o.GetType() & " element into collection with element type " & type
			End Function

			Friend Sub New(ByVal c As Collection(Of E), ByVal type As Class)
				Me.c = Objects.requireNonNull(c, "c")
				Me.type = Objects.requireNonNull(type, "type")
			End Sub

			Public Overridable Function size() As Integer Implements Collection(Of E).size
				Return c.size()
			End Function
			Public Overridable Property empty As Boolean Implements Collection(Of E).isEmpty
				Get
					Return c.empty
				End Get
			End Property
			Public Overridable Function contains(ByVal o As Object) As Boolean Implements Collection(Of E).contains
				Return c.contains(o)
			End Function
			Public Overridable Function toArray() As Object() Implements Collection(Of E).toArray
				Return c.ToArray()
			End Function
			Public Overridable Function toArray(Of T)(ByVal a As T()) As T() Implements Collection(Of E).toArray
				Return c.ToArray(a)
			End Function
			Public Overrides Function ToString() As String
				Return c.ToString()
			End Function
			Public Overridable Function remove(ByVal o As Object) As Boolean Implements Collection(Of E).remove
				Return c.remove(o)
			End Function
			Public Overridable Sub clear() Implements Collection(Of E).clear
				c.clear()
			End Sub

			Public Overridable Function containsAll(Of T1)(ByVal coll As Collection(Of T1)) As Boolean Implements Collection(Of E).containsAll
				Return c.containsAll(coll)
			End Function
			Public Overridable Function removeAll(Of T1)(ByVal coll As Collection(Of T1)) As Boolean Implements Collection(Of E).removeAll
				Return c.removeAll(coll)
			End Function
			Public Overridable Function retainAll(Of T1)(ByVal coll As Collection(Of T1)) As Boolean Implements Collection(Of E).retainAll
				Return c.retainAll(coll)
			End Function

			Public Overridable Function [iterator]() As [Iterator](Of E) Implements Collection(Of E).iterator
				' JDK-6363904 - unwrapped iterator could be typecast to
				' ListIterator with unsafe set()
				Dim it As [Iterator](Of E) = c.GetEnumerator()
				Return New IteratorAnonymousInnerClassHelper(Of E)
			End Function

			Private Class IteratorAnonymousInnerClassHelper(Of E)
				Implements Iterator(Of E)

				Public Overridable Function hasNext() As Boolean Implements Iterator(Of E).hasNext
					Return it.hasNext()
				End Function
				Public Overridable Function [next]() As E Implements Iterator(Of E).next
					Return it.next()
				End Function
				Public Overridable Sub remove() Implements Iterator(Of E).remove
					it.remove()
				End Sub
			End Class

			Public Overridable Function add(ByVal e As E) As Boolean
				Return c.add(typeCheck(e))
			End Function

			Private zeroLengthElementArray_Renamed As E() ' Lazily initialized

			Private Function zeroLengthElementArray() As E()
					If zeroLengthElementArray_Renamed IsNot Nothing Then
						Return zeroLengthElementArray_Renamed
					Else
'JAVA TO VB CONVERTER TODO TASK: Assignments within expressions are not supported in VB
						Return (zeroLengthElementArray_Renamed = zeroLengthArray(type))
					End If
			End Function

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
			Friend Overridable Function checkedCopyOf(Of T1 As E)(ByVal coll As Collection(Of T1)) As Collection(Of E)
				Dim a As Object()
				Try
					Dim z As E() = zeroLengthElementArray()
					a = coll.ToArray(z)
					' Defend against coll violating the toArray contract
					If a.GetType() IsNot z.GetType() Then a = Arrays.copyOf(a, a.Length, z.GetType())
				Catch ignore As ArrayStoreException
					' To get better and consistent diagnostics,
					' we call typeCheck explicitly on each element.
					' We call clone() to defend against coll retaining a
					' reference to the returned array and storing a bad
					' element into it after it has been type checked.
					a = coll.ToArray().clone()
					For Each o As Object In a
						typeCheck(o)
					Next o
				End Try
				' A slight abuse of the type system, but safe here.
				Return CType(a, Collection(Of E))
			End Function

			Public Overridable Function addAll(Of T1 As E)(ByVal coll As Collection(Of T1)) As Boolean Implements Collection(Of E).addAll
				' Doing things this way insulates us from concurrent changes
				' in the contents of coll and provides all-or-nothing
				' semantics (which we wouldn't get if we type-checked each
				' element as we added it)
				Return c.addAll(checkedCopyOf(coll))
			End Function

			' Override default methods in Collection
'JAVA TO VB CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
			Public Overrides Sub forEach(Of T1)(ByVal action As java.util.function.Consumer(Of T1))
				c.forEach(action)
			End Sub
'JAVA TO VB CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
			Public Overrides Function removeIf(Of T1)(ByVal filter As java.util.function.Predicate(Of T1)) As Boolean Implements Collection(Of E).removeIf
				Return c.removeIf(filter)
			End Function
			Public Overrides Function spliterator() As Spliterator(Of E) Implements Collection(Of E).spliterator
				Return c.spliterator()
			End Function
			Public Overrides Function stream() As java.util.stream.Stream(Of E) Implements Collection(Of E).stream
				Return c.stream()
			End Function
			Public Overrides Function parallelStream() As java.util.stream.Stream(Of E) Implements Collection(Of E).parallelStream
				Return c.parallelStream()
			End Function
		End Class

		''' <summary>
		''' Returns a dynamically typesafe view of the specified queue.
		''' Any attempt to insert an element of the wrong type will result in
		''' an immediate <seealso cref="ClassCastException"/>.  Assuming a queue contains
		''' no incorrectly typed elements prior to the time a dynamically typesafe
		''' view is generated, and that all subsequent access to the queue
		''' takes place through the view, it is <i>guaranteed</i> that the
		''' queue cannot contain an incorrectly typed element.
		''' 
		''' <p>A discussion of the use of dynamically typesafe views may be
		''' found in the documentation for the {@link #checkedCollection
		''' checkedCollection} method.
		''' 
		''' <p>The returned queue will be serializable if the specified queue
		''' is serializable.
		''' 
		''' <p>Since {@code null} is considered to be a value of any reference
		''' type, the returned queue permits insertion of {@code null} elements
		''' whenever the backing queue does.
		''' </summary>
		''' @param <E> the class of the objects in the queue </param>
		''' <param name="queue"> the queue for which a dynamically typesafe view is to be
		'''             returned </param>
		''' <param name="type"> the type of element that {@code queue} is permitted to hold </param>
		''' <returns> a dynamically typesafe view of the specified queue
		''' @since 1.8 </returns>
		Public Shared Function checkedQueue(Of E)(ByVal queue As Queue(Of E), ByVal type As Class) As Queue(Of E)
			Return New CheckedQueue(Of )(queue, type)
		End Function

		''' <summary>
		''' @serial include
		''' </summary>
		<Serializable> _
		Friend Class CheckedQueue(Of E)
			Inherits CheckedCollection(Of E)
			Implements Queue(Of E)

			Private Const serialVersionUID As Long = 1433151992604707767L
			Friend ReadOnly queue As Queue(Of E)

			Friend Sub New(ByVal queue As Queue(Of E), ByVal elementType As Class)
				MyBase.New(queue, elementType)
				Me.queue = queue
			End Sub

			Public Overridable Function element() As E Implements Queue(Of E).element
				Return queue.element()
			End Function
			Public Overrides Function Equals(ByVal o As Object) As Boolean
				Return o Is Me OrElse c.Equals(o)
			End Function
			Public Overrides Function GetHashCode() As Integer
				Return c.GetHashCode()
			End Function
			Public Overridable Function peek() As E Implements Queue(Of E).peek
				Return queue.peek()
			End Function
			Public Overridable Function poll() As E Implements Queue(Of E).poll
				Return queue.poll()
			End Function
			Public Overridable Function remove() As E Implements Queue(Of E).remove
				Return queue.remove()
			End Function
			Public Overridable Function offer(ByVal e As E) As Boolean
				Return queue.offer(typeCheck(e))
			End Function
		End Class

		''' <summary>
		''' Returns a dynamically typesafe view of the specified set.
		''' Any attempt to insert an element of the wrong type will result in
		''' an immediate <seealso cref="ClassCastException"/>.  Assuming a set contains
		''' no incorrectly typed elements prior to the time a dynamically typesafe
		''' view is generated, and that all subsequent access to the set
		''' takes place through the view, it is <i>guaranteed</i> that the
		''' set cannot contain an incorrectly typed element.
		''' 
		''' <p>A discussion of the use of dynamically typesafe views may be
		''' found in the documentation for the {@link #checkedCollection
		''' checkedCollection} method.
		''' 
		''' <p>The returned set will be serializable if the specified set is
		''' serializable.
		''' 
		''' <p>Since {@code null} is considered to be a value of any reference
		''' type, the returned set permits insertion of null elements whenever
		''' the backing set does.
		''' </summary>
		''' @param <E> the class of the objects in the set </param>
		''' <param name="s"> the set for which a dynamically typesafe view is to be
		'''          returned </param>
		''' <param name="type"> the type of element that {@code s} is permitted to hold </param>
		''' <returns> a dynamically typesafe view of the specified set
		''' @since 1.5 </returns>
		Public Shared Function checkedSet(Of E)(ByVal s As [Set](Of E), ByVal type As Class) As [Set](Of E)
			Return New CheckedSet(Of )(s, type)
		End Function

		''' <summary>
		''' @serial include
		''' </summary>
		<Serializable> _
		Friend Class CheckedSet(Of E)
			Inherits CheckedCollection(Of E)
			Implements Set(Of E)

			Private Const serialVersionUID As Long = 4694047833775013803L

			Friend Sub New(ByVal s As [Set](Of E), ByVal elementType As Class)
				MyBase.New(s, elementType)
			End Sub

			Public Overrides Function Equals(ByVal o As Object) As Boolean
				Return o Is Me OrElse c.Equals(o)
			End Function
			Public Overrides Function GetHashCode() As Integer
				Return c.GetHashCode()
			End Function
		End Class

		''' <summary>
		''' Returns a dynamically typesafe view of the specified sorted set.
		''' Any attempt to insert an element of the wrong type will result in an
		''' immediate <seealso cref="ClassCastException"/>.  Assuming a sorted set
		''' contains no incorrectly typed elements prior to the time a
		''' dynamically typesafe view is generated, and that all subsequent
		''' access to the sorted set takes place through the view, it is
		''' <i>guaranteed</i> that the sorted set cannot contain an incorrectly
		''' typed element.
		''' 
		''' <p>A discussion of the use of dynamically typesafe views may be
		''' found in the documentation for the {@link #checkedCollection
		''' checkedCollection} method.
		''' 
		''' <p>The returned sorted set will be serializable if the specified sorted
		''' set is serializable.
		''' 
		''' <p>Since {@code null} is considered to be a value of any reference
		''' type, the returned sorted set permits insertion of null elements
		''' whenever the backing sorted set does.
		''' </summary>
		''' @param <E> the class of the objects in the set </param>
		''' <param name="s"> the sorted set for which a dynamically typesafe view is to be
		'''          returned </param>
		''' <param name="type"> the type of element that {@code s} is permitted to hold </param>
		''' <returns> a dynamically typesafe view of the specified sorted set
		''' @since 1.5 </returns>
		Public Shared Function checkedSortedSet(Of E)(ByVal s As SortedSet(Of E), ByVal type As Class) As SortedSet(Of E)
			Return New CheckedSortedSet(Of )(s, type)
		End Function

		''' <summary>
		''' @serial include
		''' </summary>
		<Serializable> _
		Friend Class CheckedSortedSet(Of E)
			Inherits CheckedSet(Of E)
			Implements SortedSet(Of E)

			Private Const serialVersionUID As Long = 1599911165492914959L

			Private ReadOnly ss As SortedSet(Of E)

			Friend Sub New(ByVal s As SortedSet(Of E), ByVal type As Class)
				MyBase.New(s, type)
				ss = s
			End Sub

'JAVA TO VB CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
			Public Overridable Function comparator() As Comparator(Of ?) Implements SortedSet(Of E).comparator
				Return ss.comparator()
			End Function
			Public Overridable Function first() As E Implements SortedSet(Of E).first
				Return ss.first()
			End Function
			Public Overridable Function last() As E Implements SortedSet(Of E).last
				Return ss.last()
			End Function

			Public Overridable Function subSet(ByVal fromElement As E, ByVal toElement As E) As SortedSet(Of E)
				Return checkedSortedSet(ss.subSet(fromElement,toElement), type)
			End Function
			Public Overridable Function headSet(ByVal toElement As E) As SortedSet(Of E)
				Return checkedSortedSet(ss.headSet(toElement), type)
			End Function
			Public Overridable Function tailSet(ByVal fromElement As E) As SortedSet(Of E)
				Return checkedSortedSet(ss.tailSet(fromElement), type)
			End Function
		End Class

	''' <summary>
	''' Returns a dynamically typesafe view of the specified navigable set.
	''' Any attempt to insert an element of the wrong type will result in an
	''' immediate <seealso cref="ClassCastException"/>.  Assuming a navigable set
	''' contains no incorrectly typed elements prior to the time a
	''' dynamically typesafe view is generated, and that all subsequent
	''' access to the navigable set takes place through the view, it is
	''' <em>guaranteed</em> that the navigable set cannot contain an incorrectly
	''' typed element.
	'''     
	''' <p>A discussion of the use of dynamically typesafe views may be
	''' found in the documentation for the {@link #checkedCollection
	''' checkedCollection} method.
	'''     
	''' <p>The returned navigable set will be serializable if the specified
	''' navigable set is serializable.
	'''     
	''' <p>Since {@code null} is considered to be a value of any reference
	''' type, the returned navigable set permits insertion of null elements
	''' whenever the backing sorted set does.
	''' </summary>
	''' @param <E> the class of the objects in the set </param>
	''' <param name="s"> the navigable set for which a dynamically typesafe view is to be
	'''          returned </param>
	''' <param name="type"> the type of element that {@code s} is permitted to hold </param>
	''' <returns> a dynamically typesafe view of the specified navigable set
	''' @since 1.8 </returns>
		Public Shared Function checkedNavigableSet(Of E)(ByVal s As NavigableSet(Of E), ByVal type As Class) As NavigableSet(Of E)
			Return New CheckedNavigableSet(Of )(s, type)
		End Function

		''' <summary>
		''' @serial include
		''' </summary>
		<Serializable> _
		Friend Class CheckedNavigableSet(Of E)
			Inherits CheckedSortedSet(Of E)
			Implements NavigableSet(Of E)

			Private Const serialVersionUID As Long = -5429120189805438922L

			Private ReadOnly ns As NavigableSet(Of E)

			Friend Sub New(ByVal s As NavigableSet(Of E), ByVal type As Class)
				MyBase.New(s, type)
				ns = s
			End Sub

			Public Overridable Function lower(ByVal e As E) As E
				Return ns.lower(e)
			End Function
			Public Overridable Function floor(ByVal e As E) As E
				Return ns.floor(e)
			End Function
			Public Overridable Function ceiling(ByVal e As E) As E
				Return ns.ceiling(e)
			End Function
			Public Overridable Function higher(ByVal e As E) As E
				Return ns.higher(e)
			End Function
			Public Overridable Function pollFirst() As E Implements NavigableSet(Of E).pollFirst
				Return ns.pollFirst()
			End Function
			Public Overridable Function pollLast() As E Implements NavigableSet(Of E).pollLast
				Return ns.pollLast()
			End Function
			Public Overridable Function descendingSet() As NavigableSet(Of E) Implements NavigableSet(Of E).descendingSet
							  Return checkedNavigableSet(ns.descendingSet(), type)
			End Function
			Public Overridable Function descendingIterator() As [Iterator](Of E) Implements NavigableSet(Of E).descendingIterator
					Return checkedNavigableSet(ns.descendingSet(), type).GetEnumerator()
			End Function

			Public Overridable Function subSet(ByVal fromElement As E, ByVal toElement As E) As NavigableSet(Of E)
				Return checkedNavigableSet(ns.subSet(fromElement, True, toElement, False), type)
			End Function
			Public Overridable Function headSet(ByVal toElement As E) As NavigableSet(Of E)
				Return checkedNavigableSet(ns.headSet(toElement, False), type)
			End Function
			Public Overridable Function tailSet(ByVal fromElement As E) As NavigableSet(Of E)
				Return checkedNavigableSet(ns.tailSet(fromElement, True), type)
			End Function

			Public Overridable Function subSet(ByVal fromElement As E, ByVal fromInclusive As Boolean, ByVal toElement As E, ByVal toInclusive As Boolean) As NavigableSet(Of E)
				Return checkedNavigableSet(ns.subSet(fromElement, fromInclusive, toElement, toInclusive), type)
			End Function

			Public Overridable Function headSet(ByVal toElement As E, ByVal inclusive As Boolean) As NavigableSet(Of E)
				Return checkedNavigableSet(ns.headSet(toElement, inclusive), type)
			End Function

			Public Overridable Function tailSet(ByVal fromElement As E, ByVal inclusive As Boolean) As NavigableSet(Of E)
				Return checkedNavigableSet(ns.tailSet(fromElement, inclusive), type)
			End Function
		End Class

		''' <summary>
		''' Returns a dynamically typesafe view of the specified list.
		''' Any attempt to insert an element of the wrong type will result in
		''' an immediate <seealso cref="ClassCastException"/>.  Assuming a list contains
		''' no incorrectly typed elements prior to the time a dynamically typesafe
		''' view is generated, and that all subsequent access to the list
		''' takes place through the view, it is <i>guaranteed</i> that the
		''' list cannot contain an incorrectly typed element.
		''' 
		''' <p>A discussion of the use of dynamically typesafe views may be
		''' found in the documentation for the {@link #checkedCollection
		''' checkedCollection} method.
		''' 
		''' <p>The returned list will be serializable if the specified list
		''' is serializable.
		''' 
		''' <p>Since {@code null} is considered to be a value of any reference
		''' type, the returned list permits insertion of null elements whenever
		''' the backing list does.
		''' </summary>
		''' @param <E> the class of the objects in the list </param>
		''' <param name="list"> the list for which a dynamically typesafe view is to be
		'''             returned </param>
		''' <param name="type"> the type of element that {@code list} is permitted to hold </param>
		''' <returns> a dynamically typesafe view of the specified list
		''' @since 1.5 </returns>
		Public Shared Function checkedList(Of E)(ByVal list As List(Of E), ByVal type As Class) As List(Of E)
			Return (If(TypeOf list Is RandomAccess, New CheckedRandomAccessList(Of )(list, type), New CheckedList(Of )(list, type)))
		End Function

		''' <summary>
		''' @serial include
		''' </summary>
		Friend Class CheckedList(Of E)
			Inherits CheckedCollection(Of E)
			Implements List(Of E)

			Private Const serialVersionUID As Long = 65247728283967356L
			Friend ReadOnly list As List(Of E)

			Friend Sub New(ByVal list As List(Of E), ByVal type As Class)
				MyBase.New(list, type)
				Me.list = list
			End Sub

			Public Overrides Function Equals(ByVal o As Object) As Boolean
				Return o Is Me OrElse list.Equals(o)
			End Function
			Public Overrides Function GetHashCode() As Integer
				Return list.GetHashCode()
			End Function
			Public Overridable Function [get](ByVal index As Integer) As E Implements List(Of E).get
				Return list.get(index)
			End Function
			Public Overridable Function remove(ByVal index As Integer) As E Implements List(Of E).remove
				Return list.remove(index)
			End Function
			Public Overridable Function indexOf(ByVal o As Object) As Integer Implements List(Of E).indexOf
				Return list.IndexOf(o)
			End Function
			Public Overridable Function lastIndexOf(ByVal o As Object) As Integer Implements List(Of E).lastIndexOf
				Return list.LastIndexOf(o)
			End Function

			Public Overridable Function [set](ByVal index As Integer, ByVal element As E) As E
				Return list.set(index, typeCheck(element))
			End Function

			Public Overridable Sub add(ByVal index As Integer, ByVal element As E)
				list.add(index, typeCheck(element))
			End Sub

			Public Overridable Function addAll(Of T1 As E)(ByVal index As Integer, ByVal c As Collection(Of T1)) As Boolean Implements List(Of E).addAll
				Return list.addAll(index, checkedCopyOf(c))
			End Function
			Public Overridable Function listIterator() As ListIterator(Of E) Implements List(Of E).listIterator
				Return listIterator(0)
			End Function

			Public Overridable Function listIterator(ByVal index As Integer) As ListIterator(Of E) Implements List(Of E).listIterator
				Dim i As ListIterator(Of E) = list.listIterator(index)

				Return New ListIteratorAnonymousInnerClassHelper(Of E)
			End Function

			Private Class ListIteratorAnonymousInnerClassHelper(Of E)
				Implements ListIterator(Of E)

				Public Overridable Function hasNext() As Boolean Implements ListIterator(Of E).hasNext
					Return i.hasNext()
				End Function
				Public Overridable Function [next]() As E Implements ListIterator(Of E).next
					Return i.next()
				End Function
				Public Overridable Function hasPrevious() As Boolean Implements ListIterator(Of E).hasPrevious
					Return i.hasPrevious()
				End Function
				Public Overridable Function previous() As E Implements ListIterator(Of E).previous
					Return i.previous()
				End Function
				Public Overridable Function nextIndex() As Integer Implements ListIterator(Of E).nextIndex
					Return i.nextIndex()
				End Function
				Public Overridable Function previousIndex() As Integer Implements ListIterator(Of E).previousIndex
					Return i.previousIndex()
				End Function
				Public Overridable Sub remove() Implements ListIterator(Of E).remove
					i.remove()
				End Sub

				Public Overridable Sub [set](ByVal e As E)
					i.set(outerInstance.typeCheck(e))
				End Sub

				Public Overridable Sub add(ByVal e As E)
					i.add(outerInstance.typeCheck(e))
				End Sub

'JAVA TO VB CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
				Public Overrides Sub forEachRemaining(Of T1)(ByVal action As java.util.function.Consumer(Of T1))
					i.forEachRemaining(action)
				End Sub
			End Class

			Public Overridable Function subList(ByVal fromIndex As Integer, ByVal toIndex As Integer) As List(Of E) Implements List(Of E).subList
				Return New CheckedList(Of )(list.subList(fromIndex, toIndex), type)
			End Function

			''' <summary>
			''' {@inheritDoc}
			''' </summary>
			''' <exception cref="ClassCastException"> if the class of an element returned by the
			'''         operator prevents it from being added to this collection. The
			'''         exception may be thrown after some elements of the list have
			'''         already been replaced. </exception>
			Public Overrides Sub replaceAll(ByVal [operator] As java.util.function.UnaryOperator(Of E)) Implements List(Of E).replaceAll
				Objects.requireNonNull([operator])
				list.replaceAll(e -> typeCheck([operator].apply(e)))
			End Sub

'JAVA TO VB CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
			Public Overrides Sub sort(Of T1)(ByVal c As Comparator(Of T1)) Implements List(Of E).sort
				list.sort(c)
			End Sub
		End Class

		''' <summary>
		''' @serial include
		''' </summary>
		Friend Class CheckedRandomAccessList(Of E)
			Inherits CheckedList(Of E)
			Implements RandomAccess

			Private Const serialVersionUID As Long = 1638200125423088369L

			Friend Sub New(ByVal list As List(Of E), ByVal type As Class)
				MyBase.New(list, type)
			End Sub

			Public Overridable Function subList(ByVal fromIndex As Integer, ByVal toIndex As Integer) As List(Of E)
				Return New CheckedRandomAccessList(Of )(list.subList(fromIndex, toIndex), type)
			End Function
		End Class

		''' <summary>
		''' Returns a dynamically typesafe view of the specified map.
		''' Any attempt to insert a mapping whose key or value have the wrong
		''' type will result in an immediate <seealso cref="ClassCastException"/>.
		''' Similarly, any attempt to modify the value currently associated with
		''' a key will result in an immediate <seealso cref="ClassCastException"/>,
		''' whether the modification is attempted directly through the map
		''' itself, or through a <seealso cref="Map.Entry"/> instance obtained from the
		''' map's <seealso cref="Map#entrySet() entry set"/> view.
		''' 
		''' <p>Assuming a map contains no incorrectly typed keys or values
		''' prior to the time a dynamically typesafe view is generated, and
		''' that all subsequent access to the map takes place through the view
		''' (or one of its collection views), it is <i>guaranteed</i> that the
		''' map cannot contain an incorrectly typed key or value.
		''' 
		''' <p>A discussion of the use of dynamically typesafe views may be
		''' found in the documentation for the {@link #checkedCollection
		''' checkedCollection} method.
		''' 
		''' <p>The returned map will be serializable if the specified map is
		''' serializable.
		''' 
		''' <p>Since {@code null} is considered to be a value of any reference
		''' type, the returned map permits insertion of null keys or values
		''' whenever the backing map does.
		''' </summary>
		''' @param <K> the class of the map keys </param>
		''' @param <V> the class of the map values </param>
		''' <param name="m"> the map for which a dynamically typesafe view is to be
		'''          returned </param>
		''' <param name="keyType"> the type of key that {@code m} is permitted to hold </param>
		''' <param name="valueType"> the type of value that {@code m} is permitted to hold </param>
		''' <returns> a dynamically typesafe view of the specified map
		''' @since 1.5 </returns>
		Public Shared Function checkedMap(Of K, V)(ByVal m As Map(Of K, V), ByVal keyType As Class, ByVal valueType As Class) As Map(Of K, V)
			Return New CheckedMap(Of )(m, keyType, valueType)
		End Function


		''' <summary>
		''' @serial include
		''' </summary>
		<Serializable> _
		Private Class CheckedMap(Of K, V)
			Implements Map(Of K, V)

			Private Const serialVersionUID As Long = 5742860141034234728L

			Private ReadOnly m As Map(Of K, V)
			Friend ReadOnly keyType As Class
			Friend ReadOnly valueType As Class

			Private Sub typeCheck(ByVal key As Object, ByVal value As Object)
				If key IsNot Nothing AndAlso (Not keyType.isInstance(key)) Then Throw New ClassCastException(badKeyMsg(key))

				If value IsNot Nothing AndAlso (Not valueType.isInstance(value)) Then Throw New ClassCastException(badValueMsg(value))
			End Sub

'JAVA TO VB CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
			Private Function typeCheck(Of T1 As V)(ByVal func As java.util.function.BiFunction(Of T1)) As java.util.function.BiFunction(Of ?, ?, ? As V)
				Objects.requireNonNull(func)
				Return (k, v) ->
					Dim newValue As V = func.apply(k, v)
					typeCheck(k, newValue)
					Return newValue
			End Function

			Private Function badKeyMsg(ByVal key As Object) As String
				Return "Attempt to insert " & key.GetType() & " key into map with key type " & keyType
			End Function

			Private Function badValueMsg(ByVal value As Object) As String
				Return "Attempt to insert " & value.GetType() & " value into map with value type " & valueType
			End Function

			Friend Sub New(ByVal m As Map(Of K, V), ByVal keyType As Class, ByVal valueType As Class)
				Me.m = Objects.requireNonNull(m)
				Me.keyType = Objects.requireNonNull(keyType)
				Me.valueType = Objects.requireNonNull(valueType)
			End Sub

			Public Overridable Function size() As Integer Implements Map(Of K, V).size
				Return m.size()
			End Function
			Public Overridable Property empty As Boolean Implements Map(Of K, V).isEmpty
				Get
					Return m.empty
				End Get
			End Property
			Public Overridable Function containsKey(ByVal key As Object) As Boolean Implements Map(Of K, V).containsKey
				Return m.containsKey(key)
			End Function
			Public Overridable Function containsValue(ByVal v As Object) As Boolean Implements Map(Of K, V).containsValue
				Return m.containsValue(v)
			End Function
			Public Overridable Function [get](ByVal key As Object) As V Implements Map(Of K, V).get
				Return m.get(key)
			End Function
			Public Overridable Function remove(ByVal key As Object) As V Implements Map(Of K, V).remove
				Return m.remove(key)
			End Function
			Public Overridable Sub clear() Implements Map(Of K, V).clear
				m.clear()
			End Sub
			Public Overridable Function keySet() As [Set](Of K) Implements Map(Of K, V).keySet
				Return m.Keys
			End Function
			Public Overridable Function values() As Collection(Of V) Implements Map(Of K, V).values
				Return m.values()
			End Function
			Public Overrides Function Equals(ByVal o As Object) As Boolean
				Return o Is Me OrElse m.Equals(o)
			End Function
			Public Overrides Function GetHashCode() As Integer
				Return m.GetHashCode()
			End Function
			Public Overrides Function ToString() As String
				Return m.ToString()
			End Function

			Public Overridable Function put(ByVal key As K, ByVal value As V) As V Implements Map(Of K, V).put
				typeCheck(key, value)
				Return m.put(key, value)
			End Function

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
			Public Overridable Sub putAll(Of T1 As K, ? As V)(ByVal t As Map(Of T1)) Implements Map(Of K, V).putAll
				' Satisfy the following goals:
				' - good diagnostics in case of type mismatch
				' - all-or-nothing semantics
				' - protection from malicious t
				' - correct behavior if t is a concurrent map
				Dim entries As Object() = t.entrySet().ToArray()
				Dim checked As List(Of KeyValuePair(Of K, V)) = New List(Of KeyValuePair(Of K, V))(entries.Length)
				For Each o As Object In entries
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
					Dim e As KeyValuePair(Of ?, ?) = CType(o, KeyValuePair(Of ?, ?))
					Dim k As Object = e.Key
					Dim v As Object = e.Value
					typeCheck(k, v)
					checked.add(New AbstractMap.SimpleImmutableEntry(Of )(CType(k, K), CType(v, V)))
				Next o
				For Each e As KeyValuePair(Of K, V) In checked
					m.put(e.Key, e.Value)
				Next e
			End Sub

			<NonSerialized> _
			Private entrySet_Renamed As [Set](Of KeyValuePair(Of K, V))

			Public Overridable Function entrySet() As [Set](Of KeyValuePair(Of K, V)) Implements Map(Of K, V).entrySet
				If entrySet_Renamed Is Nothing Then entrySet_Renamed = New CheckedEntrySet(Of )(m.entrySet(), valueType)
				Return entrySet_Renamed
			End Function

			' Override default methods in Map
'JAVA TO VB CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
			Public Overrides Sub forEach(Of T1)(ByVal action As java.util.function.BiConsumer(Of T1)) Implements Map(Of K, V).forEach
				m.forEach(action)
			End Sub

'JAVA TO VB CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
			Public Overrides Sub replaceAll(Of T1 As V)(ByVal [function] As java.util.function.BiFunction(Of T1)) Implements Map(Of K, V).replaceAll
				m.replaceAll(typeCheck([function]))
			End Sub

			Public Overrides Function putIfAbsent(ByVal key As K, ByVal value As V) As V Implements Map(Of K, V).putIfAbsent
				typeCheck(key, value)
				Return m.putIfAbsent(key, value)
			End Function

			Public Overrides Function remove(ByVal key As Object, ByVal value As Object) As Boolean Implements Map(Of K, V).remove
				Return m.remove(key, value)
			End Function

			Public Overrides Function replace(ByVal key As K, ByVal oldValue As V, ByVal newValue As V) As Boolean Implements Map(Of K, V).replace
				typeCheck(key, newValue)
				Return m.replace(key, oldValue, newValue)
			End Function

			Public Overrides Function replace(ByVal key As K, ByVal value As V) As V Implements Map(Of K, V).replace
				typeCheck(key, value)
				Return m.replace(key, value)
			End Function

'JAVA TO VB CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
			Public Overrides Function computeIfAbsent(Of T1 As V)(ByVal key As K, ByVal mappingFunction As java.util.function.Function(Of T1)) As V Implements Map(Of K, V).computeIfAbsent
				Objects.requireNonNull(mappingFunction)
'JAVA TO VB CONVERTER TODO TASK: Assignments within expressions are not supported in VB
				Return m.computeIfAbsent(key, k -> { V value = mappingFunction.apply(k); typeCheck(k, value); Return value; })
			End Function

'JAVA TO VB CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
			Public Overrides Function computeIfPresent(Of T1 As V)(ByVal key As K, ByVal remappingFunction As java.util.function.BiFunction(Of T1)) As V Implements Map(Of K, V).computeIfPresent
				Return m.computeIfPresent(key, typeCheck(remappingFunction))
			End Function

'JAVA TO VB CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
			Public Overrides Function compute(Of T1 As V)(ByVal key As K, ByVal remappingFunction As java.util.function.BiFunction(Of T1)) As V Implements Map(Of K, V).compute
				Return m.compute(key, typeCheck(remappingFunction))
			End Function

'JAVA TO VB CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
			Public Overrides Function merge(Of T1 As V)(ByVal key As K, ByVal value As V, ByVal remappingFunction As java.util.function.BiFunction(Of T1)) As V Implements Map(Of K, V).merge
				Objects.requireNonNull(remappingFunction)
'JAVA TO VB CONVERTER TODO TASK: Assignments within expressions are not supported in VB
				Return m.merge(key, value, (v1, v2) -> { V newValue = remappingFunction.apply(v1, v2); typeCheck(Nothing, newValue); Return newValue; })
			End Function

			''' <summary>
			''' We need this class in addition to CheckedSet as Map.Entry permits
			''' modification of the backing Map via the setValue operation.  This
			''' class is subtle: there are many possible attacks that must be
			''' thwarted.
			''' 
			''' @serial exclude
			''' </summary>
			Friend Class CheckedEntrySet(Of K, V)
				Implements Set(Of KeyValuePair(Of K, V))

				Private ReadOnly s As [Set](Of KeyValuePair(Of K, V))
				Private ReadOnly valueType As Class

				Friend Sub New(ByVal s As [Set](Of KeyValuePair(Of K, V)), ByVal valueType As Class)
					Me.s = s
					Me.valueType = valueType
				End Sub

				Public Overridable Function size() As Integer Implements Set(Of KeyValuePair(Of K, V)).size
					Return s.size()
				End Function
				Public Overridable Property empty As Boolean Implements Set(Of KeyValuePair(Of K, V)).isEmpty
					Get
						Return s.empty
					End Get
				End Property
				Public Overrides Function ToString() As String
					Return s.ToString()
				End Function
				Public Overrides Function GetHashCode() As Integer
					Return s.GetHashCode()
				End Function
				Public Overridable Sub clear() Implements Set(Of KeyValuePair(Of K, V)).clear
					s.clear()
				End Sub

				Public Overridable Function add(ByVal e As KeyValuePair(Of K, V)) As Boolean
					Throw New UnsupportedOperationException
				End Function
				Public Overridable Function addAll(Of T1 As KeyValuePair(Of K, V)(ByVal coll As Collection(Of T1)) As Boolean Implements Set(Of KeyValuePair(Of K, V)).addAll
					Throw New UnsupportedOperationException
				End Function

				Public Overridable Function [iterator]() As [Iterator](Of KeyValuePair(Of K, V)) Implements Set(Of KeyValuePair(Of K, V)).iterator
					Dim i As [Iterator](Of KeyValuePair(Of K, V)) = s.GetEnumerator()
					Dim valueType As Class = Me.valueType

					Return New IteratorAnonymousInnerClassHelper(Of E)
				End Function

				Private Class IteratorAnonymousInnerClassHelper(Of E)
					Implements Iterator(Of E)

					Public Overridable Function hasNext() As Boolean Implements Iterator(Of E).hasNext
						Return i.hasNext()
					End Function
					Public Overridable Sub remove() Implements Iterator(Of E).remove
						i.remove()
					End Sub

					Public Overridable Function [next]() As KeyValuePair(Of K, V)
						Return checkedEntry(i.next(), outerInstance.valueType)
					End Function
				End Class

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
				Public Overridable Function toArray() As Object() Implements Set(Of KeyValuePair(Of K, V)).toArray
					Dim source As Object() = s.ToArray()

	'                
	'                 * Ensure that we don't get an ArrayStoreException even if
	'                 * s.toArray returns an array of something other than Object
	'                 
					Dim dest As Object() = (If(GetType(CheckedEntry).isInstance(source.GetType().GetElementType()), source, New Object(source.Length - 1){}))

					For i As Integer = 0 To source.Length - 1
						dest(i) = checkedEntry(CType(source(i), KeyValuePair(Of K, V)), valueType)
					Next i
					Return dest
				End Function

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
				Public Overridable Function toArray(Of T)(ByVal a As T()) As T() Implements Set(Of KeyValuePair(Of K, V)).toArray
					' We don't pass a to s.toArray, to avoid window of
					' vulnerability wherein an unscrupulous multithreaded client
					' could get his hands on raw (unwrapped) Entries from s.
					Dim arr As T() = s.ToArray(If(a.Length=0, a, Arrays.copyOf(a, 0)))

					For i As Integer = 0 To arr.Length - 1
						arr(i) = CType(checkedEntry(CType(arr(i), KeyValuePair(Of K, V)), valueType), T)
					Next i
					If arr.Length > a.Length Then Return arr

					Array.Copy(arr, 0, a, 0, arr.Length)
					If a.Length > arr.Length Then a(arr.Length) = Nothing
					Return a
				End Function

				''' <summary>
				''' This method is overridden to protect the backing set against
				''' an object with a nefarious equals function that senses
				''' that the equality-candidate is Map.Entry and calls its
				''' setValue method.
				''' </summary>
				Public Overridable Function contains(ByVal o As Object) As Boolean Implements Set(Of KeyValuePair(Of K, V)).contains
					If Not(TypeOf o Is DictionaryEntry) Then Return False
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
					Dim e As KeyValuePair(Of ?, ?) = CType(o, KeyValuePair(Of ?, ?))
					Return s.contains(If(TypeOf e Is CheckedEntry, e, checkedEntry(e, valueType)))
				End Function

				''' <summary>
				''' The bulk collection methods are overridden to protect
				''' against an unscrupulous collection whose contains(Object o)
				''' method senses when o is a Map.Entry, and calls o.setValue.
				''' </summary>
				Public Overridable Function containsAll(Of T1)(ByVal c As Collection(Of T1)) As Boolean Implements Set(Of KeyValuePair(Of K, V)).containsAll
					For Each o As Object In c
						If Not contains(o) Then ' Invokes safe contains() above Return False
					Next o
					Return True
				End Function

				Public Overridable Function remove(ByVal o As Object) As Boolean Implements Set(Of KeyValuePair(Of K, V)).remove
					If Not(TypeOf o Is DictionaryEntry) Then Return False
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
					Return s.remove(New AbstractMap.SimpleImmutableEntry (Of )(CType(o, KeyValuePair(Of ?, ?))))
				End Function

				Public Overridable Function removeAll(Of T1)(ByVal c As Collection(Of T1)) As Boolean Implements Set(Of KeyValuePair(Of K, V)).removeAll
					Return batchRemove(c, False)
				End Function
				Public Overridable Function retainAll(Of T1)(ByVal c As Collection(Of T1)) As Boolean Implements Set(Of KeyValuePair(Of K, V)).retainAll
					Return batchRemove(c, True)
				End Function
				Private Function batchRemove(Of T1)(ByVal c As Collection(Of T1), ByVal complement As Boolean) As Boolean
					Objects.requireNonNull(c)
					Dim modified As Boolean = False
					Dim it As [Iterator](Of KeyValuePair(Of K, V)) = [iterator]()
					Do While it.MoveNext()
						If c.contains(it.Current) <> complement Then
							it.remove()
							modified = True
						End If
					Loop
					Return modified
				End Function

				Public Overrides Function Equals(ByVal o As Object) As Boolean
					If o Is Me Then Return True
					If Not(TypeOf o Is Set) Then Return False
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
					Dim that As [Set](Of ?) = CType(o, Set(Of ?))
					Return that.size() = s.size() AndAlso containsAll(that) ' Invokes safe containsAll() above
				End Function

				Friend Shared Function checkedEntry(Of K, V, T)(ByVal e As KeyValuePair(Of K, V), ByVal valueType As Class) As CheckedEntry(Of K, V, T)
					Return New CheckedEntry(Of )(e, valueType)
				End Function

				''' <summary>
				''' This "wrapper class" serves two purposes: it prevents
				''' the client from modifying the backing Map, by short-circuiting
				''' the setValue method, and it protects the backing Map against
				''' an ill-behaved Map.Entry that attempts to modify another
				''' Map.Entry when asked to perform an equality check.
				''' </summary>
				Private Class CheckedEntry(Of K, V, T)
					Implements KeyValuePair(Of K, V)

					Private ReadOnly e As KeyValuePair(Of K, V)
					Private ReadOnly valueType As Class

					Friend Sub New(ByVal e As KeyValuePair(Of K, V), ByVal valueType As Class)
						Me.e = Objects.requireNonNull(e)
						Me.valueType = Objects.requireNonNull(valueType)
					End Sub

					Public Overridable Property key As K
						Get
							Return e.Key
						End Get
					End Property
					Public Overridable Property value As V
						Get
							Return e.Value
						End Get
					End Property
					Public Overrides Function GetHashCode() As Integer
						Return e.GetHashCode()
					End Function
					Public Overrides Function ToString() As String
						Return e.ToString()
					End Function

					Public Overridable Function setValue(ByVal value As V) As V
						If value IsNot Nothing AndAlso (Not valueType.isInstance(value)) Then Throw New ClassCastException(badValueMsg(value))
						Return e.valuelue(value)
					End Function

					Private Function badValueMsg(ByVal value As Object) As String
						Return "Attempt to insert " & value.GetType() & " value into map with value type " & valueType
					End Function

					Public Overrides Function Equals(ByVal o As Object) As Boolean
						If o Is Me Then Return True
						If Not(TypeOf o Is DictionaryEntry) Then Return False
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
						Return e.Equals(New AbstractMap.SimpleImmutableEntry (Of )(CType(o, KeyValuePair(Of ?, ?))))
					End Function
				End Class
			End Class
		End Class

		''' <summary>
		''' Returns a dynamically typesafe view of the specified sorted map.
		''' Any attempt to insert a mapping whose key or value have the wrong
		''' type will result in an immediate <seealso cref="ClassCastException"/>.
		''' Similarly, any attempt to modify the value currently associated with
		''' a key will result in an immediate <seealso cref="ClassCastException"/>,
		''' whether the modification is attempted directly through the map
		''' itself, or through a <seealso cref="Map.Entry"/> instance obtained from the
		''' map's <seealso cref="Map#entrySet() entry set"/> view.
		''' 
		''' <p>Assuming a map contains no incorrectly typed keys or values
		''' prior to the time a dynamically typesafe view is generated, and
		''' that all subsequent access to the map takes place through the view
		''' (or one of its collection views), it is <i>guaranteed</i> that the
		''' map cannot contain an incorrectly typed key or value.
		''' 
		''' <p>A discussion of the use of dynamically typesafe views may be
		''' found in the documentation for the {@link #checkedCollection
		''' checkedCollection} method.
		''' 
		''' <p>The returned map will be serializable if the specified map is
		''' serializable.
		''' 
		''' <p>Since {@code null} is considered to be a value of any reference
		''' type, the returned map permits insertion of null keys or values
		''' whenever the backing map does.
		''' </summary>
		''' @param <K> the class of the map keys </param>
		''' @param <V> the class of the map values </param>
		''' <param name="m"> the map for which a dynamically typesafe view is to be
		'''          returned </param>
		''' <param name="keyType"> the type of key that {@code m} is permitted to hold </param>
		''' <param name="valueType"> the type of value that {@code m} is permitted to hold </param>
		''' <returns> a dynamically typesafe view of the specified map
		''' @since 1.5 </returns>
		Public Shared Function checkedSortedMap(Of K, V)(ByVal m As SortedMap(Of K, V), ByVal keyType As Class, ByVal valueType As Class) As SortedMap(Of K, V)
			Return New CheckedSortedMap(Of )(m, keyType, valueType)
		End Function

		''' <summary>
		''' @serial include
		''' </summary>
		<Serializable> _
		Friend Class CheckedSortedMap(Of K, V)
			Inherits CheckedMap(Of K, V)
			Implements SortedMap(Of K, V)

			Private Const serialVersionUID As Long = 1599671320688067438L

			Private ReadOnly sm As SortedMap(Of K, V)

			Friend Sub New(ByVal m As SortedMap(Of K, V), ByVal keyType As Class, ByVal valueType As Class)
				MyBase.New(m, keyType, valueType)
				sm = m
			End Sub

'JAVA TO VB CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
			Public Overridable Function comparator() As Comparator(Of ?) Implements SortedMap(Of K, V).comparator
				Return sm.comparator()
			End Function
			Public Overridable Function firstKey() As K Implements SortedMap(Of K, V).firstKey
				Return sm.firstKey()
			End Function
			Public Overridable Function lastKey() As K Implements SortedMap(Of K, V).lastKey
				Return sm.lastKey()
			End Function

			Public Overridable Function subMap(ByVal fromKey As K, ByVal toKey As K) As SortedMap(Of K, V) Implements SortedMap(Of K, V).subMap
				Return checkedSortedMap(sm.subMap(fromKey, toKey), keyType, valueType)
			End Function
			Public Overridable Function headMap(ByVal toKey As K) As SortedMap(Of K, V) Implements SortedMap(Of K, V).headMap
				Return checkedSortedMap(sm.headMap(toKey), keyType, valueType)
			End Function
			Public Overridable Function tailMap(ByVal fromKey As K) As SortedMap(Of K, V) Implements SortedMap(Of K, V).tailMap
				Return checkedSortedMap(sm.tailMap(fromKey), keyType, valueType)
			End Function
		End Class

		''' <summary>
		''' Returns a dynamically typesafe view of the specified navigable map.
		''' Any attempt to insert a mapping whose key or value have the wrong
		''' type will result in an immediate <seealso cref="ClassCastException"/>.
		''' Similarly, any attempt to modify the value currently associated with
		''' a key will result in an immediate <seealso cref="ClassCastException"/>,
		''' whether the modification is attempted directly through the map
		''' itself, or through a <seealso cref="Map.Entry"/> instance obtained from the
		''' map's <seealso cref="Map#entrySet() entry set"/> view.
		''' 
		''' <p>Assuming a map contains no incorrectly typed keys or values
		''' prior to the time a dynamically typesafe view is generated, and
		''' that all subsequent access to the map takes place through the view
		''' (or one of its collection views), it is <em>guaranteed</em> that the
		''' map cannot contain an incorrectly typed key or value.
		''' 
		''' <p>A discussion of the use of dynamically typesafe views may be
		''' found in the documentation for the {@link #checkedCollection
		''' checkedCollection} method.
		''' 
		''' <p>The returned map will be serializable if the specified map is
		''' serializable.
		''' 
		''' <p>Since {@code null} is considered to be a value of any reference
		''' type, the returned map permits insertion of null keys or values
		''' whenever the backing map does.
		''' </summary>
		''' @param <K> type of map keys </param>
		''' @param <V> type of map values </param>
		''' <param name="m"> the map for which a dynamically typesafe view is to be
		'''          returned </param>
		''' <param name="keyType"> the type of key that {@code m} is permitted to hold </param>
		''' <param name="valueType"> the type of value that {@code m} is permitted to hold </param>
		''' <returns> a dynamically typesafe view of the specified map
		''' @since 1.8 </returns>
		Public Shared Function checkedNavigableMap(Of K, V)(ByVal m As NavigableMap(Of K, V), ByVal keyType As Class, ByVal valueType As Class) As NavigableMap(Of K, V)
			Return New CheckedNavigableMap(Of )(m, keyType, valueType)
		End Function

		''' <summary>
		''' @serial include
		''' </summary>
		<Serializable> _
		Friend Class CheckedNavigableMap(Of K, V)
			Inherits CheckedSortedMap(Of K, V)
			Implements NavigableMap(Of K, V)

			Private Const serialVersionUID As Long = -4852462692372534096L

			Private ReadOnly nm As NavigableMap(Of K, V)

			Friend Sub New(ByVal m As NavigableMap(Of K, V), ByVal keyType As Class, ByVal valueType As Class)
				MyBase.New(m, keyType, valueType)
				nm = m
			End Sub

'JAVA TO VB CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
			Public Overridable Function comparator() As Comparator(Of ?)
				Return nm.comparator()
			End Function
			Public Overridable Function firstKey() As K
				Return nm.firstKey()
			End Function
			Public Overridable Function lastKey() As K
				Return nm.lastKey()
			End Function

			Public Overridable Function lowerEntry(ByVal key As K) As Entry(Of K, V)
				Dim lower As Entry(Of K, V) = nm.lowerEntry(key)
				Return If(Nothing IsNot lower, New CheckedMap.CheckedEntrySet.CheckedEntry(Of )(lower, valueType), Nothing)
			End Function

			Public Overridable Function lowerKey(ByVal key As K) As K Implements NavigableMap(Of K, V).lowerKey
				Return nm.lowerKey(key)
			End Function

			Public Overridable Function floorEntry(ByVal key As K) As Entry(Of K, V)
				Dim floor As Entry(Of K, V) = nm.floorEntry(key)
				Return If(Nothing IsNot floor, New CheckedMap.CheckedEntrySet.CheckedEntry(Of )(floor, valueType), Nothing)
			End Function

			Public Overridable Function floorKey(ByVal key As K) As K Implements NavigableMap(Of K, V).floorKey
				Return nm.floorKey(key)
			End Function

			Public Overridable Function ceilingEntry(ByVal key As K) As Entry(Of K, V)
				Dim ceiling As Entry(Of K, V) = nm.ceilingEntry(key)
				Return If(Nothing IsNot ceiling, New CheckedMap.CheckedEntrySet.CheckedEntry(Of )(ceiling, valueType), Nothing)
			End Function

			Public Overridable Function ceilingKey(ByVal key As K) As K Implements NavigableMap(Of K, V).ceilingKey
				Return nm.ceilingKey(key)
			End Function

			Public Overridable Function higherEntry(ByVal key As K) As Entry(Of K, V)
				Dim higher As Entry(Of K, V) = nm.higherEntry(key)
				Return If(Nothing IsNot higher, New CheckedMap.CheckedEntrySet.CheckedEntry(Of )(higher, valueType), Nothing)
			End Function

			Public Overridable Function higherKey(ByVal key As K) As K Implements NavigableMap(Of K, V).higherKey
				Return nm.higherKey(key)
			End Function

			Public Overridable Function firstEntry() As Entry(Of K, V)
				Dim first As Entry(Of K, V) = nm.firstEntry()
				Return If(Nothing IsNot first, New CheckedMap.CheckedEntrySet.CheckedEntry(Of )(first, valueType), Nothing)
			End Function

			Public Overridable Function lastEntry() As Entry(Of K, V)
				Dim last As Entry(Of K, V) = nm.lastEntry()
				Return If(Nothing IsNot last, New CheckedMap.CheckedEntrySet.CheckedEntry(Of )(last, valueType), Nothing)
			End Function

			Public Overridable Function pollFirstEntry() As Entry(Of K, V)
				Dim entry As Entry(Of K, V) = nm.pollFirstEntry()
				Return If(Nothing Is entry, Nothing, New CheckedMap.CheckedEntrySet.CheckedEntry(Of )(entry, valueType))
			End Function

			Public Overridable Function pollLastEntry() As Entry(Of K, V)
				Dim entry As Entry(Of K, V) = nm.pollLastEntry()
				Return If(Nothing Is entry, Nothing, New CheckedMap.CheckedEntrySet.CheckedEntry(Of )(entry, valueType))
			End Function

			Public Overridable Function descendingMap() As NavigableMap(Of K, V) Implements NavigableMap(Of K, V).descendingMap
				Return checkedNavigableMap(nm.descendingMap(), keyType, valueType)
			End Function

			Public Overridable Function keySet() As NavigableSet(Of K)
				Return navigableKeySet()
			End Function

			Public Overridable Function navigableKeySet() As NavigableSet(Of K) Implements NavigableMap(Of K, V).navigableKeySet
				Return checkedNavigableSet(nm.navigableKeySet(), keyType)
			End Function

			Public Overridable Function descendingKeySet() As NavigableSet(Of K) Implements NavigableMap(Of K, V).descendingKeySet
				Return checkedNavigableSet(nm.descendingKeySet(), keyType)
			End Function

			Public Overrides Function subMap(ByVal fromKey As K, ByVal toKey As K) As NavigableMap(Of K, V)
				Return checkedNavigableMap(nm.subMap(fromKey, True, toKey, False), keyType, valueType)
			End Function

			Public Overrides Function headMap(ByVal toKey As K) As NavigableMap(Of K, V)
				Return checkedNavigableMap(nm.headMap(toKey, False), keyType, valueType)
			End Function

			Public Overrides Function tailMap(ByVal fromKey As K) As NavigableMap(Of K, V)
				Return checkedNavigableMap(nm.tailMap(fromKey, True), keyType, valueType)
			End Function

			Public Overridable Function subMap(ByVal fromKey As K, ByVal fromInclusive As Boolean, ByVal toKey As K, ByVal toInclusive As Boolean) As NavigableMap(Of K, V) Implements NavigableMap(Of K, V).subMap
				Return checkedNavigableMap(nm.subMap(fromKey, fromInclusive, toKey, toInclusive), keyType, valueType)
			End Function

			Public Overridable Function headMap(ByVal toKey As K, ByVal inclusive As Boolean) As NavigableMap(Of K, V) Implements NavigableMap(Of K, V).headMap
				Return checkedNavigableMap(nm.headMap(toKey, inclusive), keyType, valueType)
			End Function

			Public Overridable Function tailMap(ByVal fromKey As K, ByVal inclusive As Boolean) As NavigableMap(Of K, V) Implements NavigableMap(Of K, V).tailMap
				Return checkedNavigableMap(nm.tailMap(fromKey, inclusive), keyType, valueType)
			End Function
		End Class

		' Empty collections

		''' <summary>
		''' Returns an iterator that has no elements.  More precisely,
		''' 
		''' <ul>
		''' <li><seealso cref="Iterator#hasNext hasNext"/> always returns {@code
		''' false}.</li>
		''' <li><seealso cref="Iterator#next next"/> always throws {@link
		''' NoSuchElementException}.</li>
		''' <li><seealso cref="Iterator#remove remove"/> always throws {@link
		''' IllegalStateException}.</li>
		''' </ul>
		''' 
		''' <p>Implementations of this method are permitted, but not
		''' required, to return the same object from multiple invocations.
		''' </summary>
		''' @param <T> type of elements, if there were any, in the iterator </param>
		''' <returns> an empty iterator
		''' @since 1.7 </returns>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
		Public Shared Function emptyIterator(Of T)() As [Iterator](Of T)
			Return CType(EmptyIterator.EMPTY_ITERATOR, Iterator(Of T))
		End Function

		Private Class EmptyIterator(Of E)
			Implements Iterator(Of E)

			Friend Shared ReadOnly EMPTY_ITERATOR As New EmptyIterator(Of Object)

			Public Overridable Function hasNext() As Boolean Implements Iterator(Of E).hasNext
				Return False
			End Function
			Public Overridable Function [next]() As E Implements Iterator(Of E).next
				Throw New NoSuchElementException
			End Function
			Public Overridable Sub remove() Implements Iterator(Of E).remove
				Throw New IllegalStateException
			End Sub
'JAVA TO VB CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
			Public Overrides Sub forEachRemaining(Of T1)(ByVal action As java.util.function.Consumer(Of T1)) Implements Iterator(Of E).forEachRemaining
				Objects.requireNonNull(action)
			End Sub
		End Class

		''' <summary>
		''' Returns a list iterator that has no elements.  More precisely,
		''' 
		''' <ul>
		''' <li><seealso cref="Iterator#hasNext hasNext"/> and {@link
		''' ListIterator#hasPrevious hasPrevious} always return {@code
		''' false}.</li>
		''' <li><seealso cref="Iterator#next next"/> and {@link ListIterator#previous
		''' previous} always throw <seealso cref="NoSuchElementException"/>.</li>
		''' <li><seealso cref="Iterator#remove remove"/> and {@link ListIterator#set
		''' set} always throw <seealso cref="IllegalStateException"/>.</li>
		''' <li><seealso cref="ListIterator#add add"/> always throws {@link
		''' UnsupportedOperationException}.</li>
		''' <li><seealso cref="ListIterator#nextIndex nextIndex"/> always returns
		''' {@code 0}.</li>
		''' <li><seealso cref="ListIterator#previousIndex previousIndex"/> always
		''' returns {@code -1}.</li>
		''' </ul>
		''' 
		''' <p>Implementations of this method are permitted, but not
		''' required, to return the same object from multiple invocations.
		''' </summary>
		''' @param <T> type of elements, if there were any, in the iterator </param>
		''' <returns> an empty list iterator
		''' @since 1.7 </returns>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
		Public Shared Function emptyListIterator(Of T)() As ListIterator(Of T)
			Return CType(EmptyListIterator.EMPTY_ITERATOR, ListIterator(Of T))
		End Function

		Private Class EmptyListIterator(Of E)
			Inherits EmptyIterator(Of E)
			Implements ListIterator(Of E)

			Friend Shared ReadOnly EMPTY_ITERATOR As New EmptyListIterator(Of Object)

			Public Overridable Function hasPrevious() As Boolean Implements ListIterator(Of E).hasPrevious
				Return False
			End Function
			Public Overridable Function previous() As E Implements ListIterator(Of E).previous
				Throw New NoSuchElementException
			End Function
			Public Overridable Function nextIndex() As Integer Implements ListIterator(Of E).nextIndex
				Return 0
			End Function
			Public Overridable Function previousIndex() As Integer Implements ListIterator(Of E).previousIndex
				Return -1
			End Function
			Public Overridable Sub [set](ByVal e As E)
				Throw New IllegalStateException
			End Sub
			Public Overridable Sub add(ByVal e As E)
				Throw New UnsupportedOperationException
			End Sub
		End Class

		''' <summary>
		''' Returns an enumeration that has no elements.  More precisely,
		''' 
		''' <ul>
		''' <li><seealso cref="Enumeration#hasMoreElements hasMoreElements"/> always
		''' returns {@code false}.</li>
		''' <li> <seealso cref="Enumeration#nextElement nextElement"/> always throws
		''' <seealso cref="NoSuchElementException"/>.</li>
		''' </ul>
		''' 
		''' <p>Implementations of this method are permitted, but not
		''' required, to return the same object from multiple invocations.
		''' </summary>
		''' @param  <T> the class of the objects in the enumeration </param>
		''' <returns> an empty enumeration
		''' @since 1.7 </returns>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
		Public Shared Function emptyEnumeration(Of T)() As Enumeration(Of T)
			Return CType(EmptyEnumeration.EMPTY_ENUMERATION, Enumeration(Of T))
		End Function

		Private Class EmptyEnumeration(Of E)
			Implements Enumeration(Of E)

			Friend Shared ReadOnly EMPTY_ENUMERATION As New EmptyEnumeration(Of Object)

			Public Overridable Function hasMoreElements() As Boolean Implements Enumeration(Of E).hasMoreElements
				Return False
			End Function
			Public Overridable Function nextElement() As E Implements Enumeration(Of E).nextElement
				Throw New NoSuchElementException
			End Function
		End Class

		''' <summary>
		''' The empty set (immutable).  This set is serializable.
		''' </summary>
		''' <seealso cref= #emptySet() </seealso>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
		Public Shared ReadOnly EMPTY_SET As [Set] = New EmptySet(Of )

		''' <summary>
		''' Returns an empty set (immutable).  This set is serializable.
		''' Unlike the like-named field, this method is parameterized.
		''' 
		''' <p>This example illustrates the type-safe way to obtain an empty set:
		''' <pre>
		'''     Set&lt;String&gt; s = Collections.emptySet();
		''' </pre>
		''' @implNote Implementations of this method need not create a separate
		''' {@code Set} object for each call.  Using this method is likely to have
		''' comparable cost to using the like-named field.  (Unlike this method, the
		''' field does not provide type safety.)
		''' </summary>
		''' @param  <T> the class of the objects in the set </param>
		''' <returns> the empty set
		''' </returns>
		''' <seealso cref= #EMPTY_SET
		''' @since 1.5 </seealso>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
		Public Shared Function emptySet(Of T)() As [Set](Of T)
			Return CType(EMPTY_SET, Set(Of T))
		End Function

		''' <summary>
		''' @serial include
		''' </summary>
		<Serializable> _
		Private Class EmptySet(Of E)
			Inherits AbstractSet(Of E)

			Private Const serialVersionUID As Long = 1582296315990362920L

			Public Overridable Function [iterator]() As [Iterator](Of E)
				Return emptyIterator()
			End Function

			Public Overridable Function size() As Integer
				Return 0
			End Function
			Public Overridable Property empty As Boolean
				Get
					Return True
				End Get
			End Property

			Public Overridable Function contains(ByVal obj As Object) As Boolean
				Return False
			End Function
			Public Overridable Function containsAll(Of T1)(ByVal c As Collection(Of T1)) As Boolean
				Return c.empty
			End Function

			Public Overridable Function toArray() As Object()
				Return New Object(){}
			End Function

			Public Overridable Function toArray(Of T)(ByVal a As T()) As T()
				If a.Length > 0 Then a(0) = Nothing
				Return a
			End Function

			' Override default methods in Collection
'JAVA TO VB CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
			Public Overrides Sub forEach(Of T1)(ByVal action As java.util.function.Consumer(Of T1))
				Objects.requireNonNull(action)
			End Sub
'JAVA TO VB CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
			Public Overrides Function removeIf(Of T1)(ByVal filter As java.util.function.Predicate(Of T1)) As Boolean
				Objects.requireNonNull(filter)
				Return False
			End Function
			Public Overrides Function spliterator() As Spliterator(Of E)
				Return Spliterators.emptySpliterator()
			End Function

			' Preserves singleton property
			Private Function readResolve() As Object
				Return EMPTY_SET
			End Function
		End Class

		''' <summary>
		''' Returns an empty sorted set (immutable).  This set is serializable.
		''' 
		''' <p>This example illustrates the type-safe way to obtain an empty
		''' sorted set:
		''' <pre> {@code
		'''     SortedSet<String> s = Collections.emptySortedSet();
		''' }</pre>
		''' 
		''' @implNote Implementations of this method need not create a separate
		''' {@code SortedSet} object for each call.
		''' </summary>
		''' @param <E> type of elements, if there were any, in the set </param>
		''' <returns> the empty sorted set
		''' @since 1.8 </returns>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
		Public Shared Function emptySortedSet(Of E)() As SortedSet(Of E)
			Return CType(UnmodifiableNavigableSet.EMPTY_NAVIGABLE_SET, SortedSet(Of E))
		End Function

		''' <summary>
		''' Returns an empty navigable set (immutable).  This set is serializable.
		''' 
		''' <p>This example illustrates the type-safe way to obtain an empty
		''' navigable set:
		''' <pre> {@code
		'''     NavigableSet<String> s = Collections.emptyNavigableSet();
		''' }</pre>
		''' 
		''' @implNote Implementations of this method need not
		''' create a separate {@code NavigableSet} object for each call.
		''' </summary>
		''' @param <E> type of elements, if there were any, in the set </param>
		''' <returns> the empty navigable set
		''' @since 1.8 </returns>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
		Public Shared Function emptyNavigableSet(Of E)() As NavigableSet(Of E)
			Return CType(UnmodifiableNavigableSet.EMPTY_NAVIGABLE_SET, NavigableSet(Of E))
		End Function

		''' <summary>
		''' The empty list (immutable).  This list is serializable.
		''' </summary>
		''' <seealso cref= #emptyList() </seealso>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
		Public Shared ReadOnly EMPTY_LIST As List = New EmptyList(Of )

		''' <summary>
		''' Returns an empty list (immutable).  This list is serializable.
		''' 
		''' <p>This example illustrates the type-safe way to obtain an empty list:
		''' <pre>
		'''     List&lt;String&gt; s = Collections.emptyList();
		''' </pre>
		''' 
		''' @implNote
		''' Implementations of this method need not create a separate <tt>List</tt>
		''' object for each call.   Using this method is likely to have comparable
		''' cost to using the like-named field.  (Unlike this method, the field does
		''' not provide type safety.)
		''' </summary>
		''' @param <T> type of elements, if there were any, in the list </param>
		''' <returns> an empty immutable list
		''' </returns>
		''' <seealso cref= #EMPTY_LIST
		''' @since 1.5 </seealso>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
		Public Shared Function emptyList(Of T)() As List(Of T)
			Return CType(EMPTY_LIST, List(Of T))
		End Function

		''' <summary>
		''' @serial include
		''' </summary>
		<Serializable> _
		Private Class EmptyList(Of E)
			Inherits AbstractList(Of E)
			Implements RandomAccess

			Private Const serialVersionUID As Long = 8842843931221139166L

			Public Overridable Function [iterator]() As [Iterator](Of E)
				Return emptyIterator()
			End Function
			Public Overridable Function listIterator() As ListIterator(Of E)
				Return emptyListIterator()
			End Function

			Public Overridable Function size() As Integer
				Return 0
			End Function
			Public Overridable Property empty As Boolean
				Get
					Return True
				End Get
			End Property

			Public Overridable Function contains(ByVal obj As Object) As Boolean
				Return False
			End Function
			Public Overridable Function containsAll(Of T1)(ByVal c As Collection(Of T1)) As Boolean
				Return c.empty
			End Function

			Public Overridable Function toArray() As Object()
				Return New Object(){}
			End Function

			Public Overridable Function toArray(Of T)(ByVal a As T()) As T()
				If a.Length > 0 Then a(0) = Nothing
				Return a
			End Function

			Public Overridable Function [get](ByVal index As Integer) As E
				Throw New IndexOutOfBoundsException("Index: " & index)
			End Function

			Public Overrides Function Equals(ByVal o As Object) As Boolean
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
				Return (TypeOf o Is List) AndAlso CType(o, List(Of ?)).empty
			End Function

			Public Overrides Function GetHashCode() As Integer
				Return 1
			End Function

'JAVA TO VB CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
			Public Overrides Function removeIf(Of T1)(ByVal filter As java.util.function.Predicate(Of T1)) As Boolean
				Objects.requireNonNull(filter)
				Return False
			End Function
			Public Overrides Sub replaceAll(ByVal [operator] As java.util.function.UnaryOperator(Of E))
				Objects.requireNonNull([operator])
			End Sub
'JAVA TO VB CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
			Public Overrides Sub sort(Of T1)(ByVal c As Comparator(Of T1))
			End Sub

			' Override default methods in Collection
'JAVA TO VB CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
			Public Overrides Sub forEach(Of T1)(ByVal action As java.util.function.Consumer(Of T1))
				Objects.requireNonNull(action)
			End Sub

			Public Overrides Function spliterator() As Spliterator(Of E)
				Return Spliterators.emptySpliterator()
			End Function

			' Preserves singleton property
			Private Function readResolve() As Object
				Return EMPTY_LIST
			End Function
		End Class

		''' <summary>
		''' The empty map (immutable).  This map is serializable.
		''' </summary>
		''' <seealso cref= #emptyMap()
		''' @since 1.3 </seealso>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
		Public Shared ReadOnly EMPTY_MAP As Map = New EmptyMap(Of )

		''' <summary>
		''' Returns an empty map (immutable).  This map is serializable.
		''' 
		''' <p>This example illustrates the type-safe way to obtain an empty map:
		''' <pre>
		'''     Map&lt;String, Date&gt; s = Collections.emptyMap();
		''' </pre>
		''' @implNote Implementations of this method need not create a separate
		''' {@code Map} object for each call.  Using this method is likely to have
		''' comparable cost to using the like-named field.  (Unlike this method, the
		''' field does not provide type safety.)
		''' </summary>
		''' @param <K> the class of the map keys </param>
		''' @param <V> the class of the map values </param>
		''' <returns> an empty map </returns>
		''' <seealso cref= #EMPTY_MAP
		''' @since 1.5 </seealso>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
		Public Shared Function emptyMap(Of K, V)() As Map(Of K, V)
			Return CType(EMPTY_MAP, Map(Of K, V))
		End Function

		''' <summary>
		''' Returns an empty sorted map (immutable).  This map is serializable.
		''' 
		''' <p>This example illustrates the type-safe way to obtain an empty map:
		''' <pre> {@code
		'''     SortedMap<String, Date> s = Collections.emptySortedMap();
		''' }</pre>
		''' 
		''' @implNote Implementations of this method need not create a separate
		''' {@code SortedMap} object for each call.
		''' </summary>
		''' @param <K> the class of the map keys </param>
		''' @param <V> the class of the map values </param>
		''' <returns> an empty sorted map
		''' @since 1.8 </returns>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
		Public Shared Function emptySortedMap(Of K, V)() As SortedMap(Of K, V)
			Return CType(UnmodifiableNavigableMap.EMPTY_NAVIGABLE_MAP, SortedMap(Of K, V))
		End Function

		''' <summary>
		''' Returns an empty navigable map (immutable).  This map is serializable.
		''' 
		''' <p>This example illustrates the type-safe way to obtain an empty map:
		''' <pre> {@code
		'''     NavigableMap<String, Date> s = Collections.emptyNavigableMap();
		''' }</pre>
		''' 
		''' @implNote Implementations of this method need not create a separate
		''' {@code NavigableMap} object for each call.
		''' </summary>
		''' @param <K> the class of the map keys </param>
		''' @param <V> the class of the map values </param>
		''' <returns> an empty navigable map
		''' @since 1.8 </returns>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
		Public Shared Function emptyNavigableMap(Of K, V)() As NavigableMap(Of K, V)
			Return CType(UnmodifiableNavigableMap.EMPTY_NAVIGABLE_MAP, NavigableMap(Of K, V))
		End Function

		''' <summary>
		''' @serial include
		''' </summary>
		<Serializable> _
		Private Class EmptyMap(Of K, V)
			Inherits AbstractMap(Of K, V)

			Private Const serialVersionUID As Long = 6428348081105594320L

			Public Overridable Function size() As Integer
				Return 0
			End Function
			Public Overridable Property empty As Boolean
				Get
					Return True
				End Get
			End Property
			Public Overridable Function containsKey(ByVal key As Object) As Boolean
				Return False
			End Function
			Public Overridable Function containsValue(ByVal value As Object) As Boolean
				Return False
			End Function
			Public Overridable Function [get](ByVal key As Object) As V
				Return Nothing
			End Function
			Public Overridable Function keySet() As [Set](Of K)
				Return emptySet()
			End Function
			Public Overridable Function values() As Collection(Of V)
				Return emptySet()
			End Function
			Public Overridable Function entrySet() As [Set](Of KeyValuePair(Of K, V))
				Return emptySet()
			End Function

			Public Overrides Function Equals(ByVal o As Object) As Boolean
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
				Return (TypeOf o Is Map) AndAlso CType(o, Map(Of ?, ?)).empty
			End Function

			Public Overrides Function GetHashCode() As Integer
				Return 0
			End Function

			' Override default methods in Map
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
			Public Overrides Function getOrDefault(ByVal k As Object, ByVal defaultValue As V) As V
				Return defaultValue
			End Function

'JAVA TO VB CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
			Public Overrides Sub forEach(Of T1)(ByVal action As java.util.function.BiConsumer(Of T1))
				Objects.requireNonNull(action)
			End Sub

'JAVA TO VB CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
			Public Overrides Sub replaceAll(Of T1 As V)(ByVal [function] As java.util.function.BiFunction(Of T1))
				Objects.requireNonNull([function])
			End Sub

			Public Overrides Function putIfAbsent(ByVal key As K, ByVal value As V) As V
				Throw New UnsupportedOperationException
			End Function

			Public Overrides Function remove(ByVal key As Object, ByVal value As Object) As Boolean
				Throw New UnsupportedOperationException
			End Function

			Public Overrides Function replace(ByVal key As K, ByVal oldValue As V, ByVal newValue As V) As Boolean
				Throw New UnsupportedOperationException
			End Function

			Public Overrides Function replace(ByVal key As K, ByVal value As V) As V
				Throw New UnsupportedOperationException
			End Function

'JAVA TO VB CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
			Public Overrides Function computeIfAbsent(Of T1 As V)(ByVal key As K, ByVal mappingFunction As java.util.function.Function(Of T1)) As V
				Throw New UnsupportedOperationException
			End Function

'JAVA TO VB CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
			Public Overrides Function computeIfPresent(Of T1 As V)(ByVal key As K, ByVal remappingFunction As java.util.function.BiFunction(Of T1)) As V
				Throw New UnsupportedOperationException
			End Function

'JAVA TO VB CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
			Public Overrides Function compute(Of T1 As V)(ByVal key As K, ByVal remappingFunction As java.util.function.BiFunction(Of T1)) As V
				Throw New UnsupportedOperationException
			End Function

'JAVA TO VB CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
			Public Overrides Function merge(Of T1 As V)(ByVal key As K, ByVal value As V, ByVal remappingFunction As java.util.function.BiFunction(Of T1)) As V
				Throw New UnsupportedOperationException
			End Function

			' Preserves singleton property
			Private Function readResolve() As Object
				Return EMPTY_MAP
			End Function
		End Class

		' Singleton collections

		''' <summary>
		''' Returns an immutable set containing only the specified object.
		''' The returned set is serializable.
		''' </summary>
		''' @param  <T> the class of the objects in the set </param>
		''' <param name="o"> the sole object to be stored in the returned set. </param>
		''' <returns> an immutable set containing only the specified object. </returns>
		Public Shared Function singleton(Of T)(ByVal o As T) As [Set](Of T)
			Return New SingletonSet(Of )(o)
		End Function

		Friend Shared Function singletonIterator(Of E)(ByVal e As E) As [Iterator](Of E)
			Return New IteratorAnonymousInnerClassHelper(Of E)
		End Function

		Private Class IteratorAnonymousInnerClassHelper(Of E)
			Implements Iterator(Of E)

			Private hasNext As Boolean = True
			Public Overridable Function hasNext() As Boolean Implements Iterator(Of E).hasNext
				Return hasNext
			End Function
			Public Overridable Function [next]() As E Implements Iterator(Of E).next
				If hasNext Then
					hasNext = False
					Return e
				End If
				Throw New NoSuchElementException
			End Function
			Public Overridable Sub remove() Implements Iterator(Of E).remove
				Throw New UnsupportedOperationException
			End Sub
'JAVA TO VB CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
			Public Overrides Sub forEachRemaining(Of T1)(ByVal action As java.util.function.Consumer(Of T1)) Implements Iterator(Of E).forEachRemaining
				Objects.requireNonNull(action)
				If hasNext Then
					action.accept(e)
					hasNext = False
				End If
			End Sub
		End Class

		''' <summary>
		''' Creates a {@code Spliterator} with only the specified element
		''' </summary>
		''' @param <T> Type of elements </param>
		''' <returns> A singleton {@code Spliterator} </returns>
		Friend Shared Function singletonSpliterator(Of T)(ByVal element As T) As Spliterator(Of T)
			Return New SpliteratorAnonymousInnerClassHelper(Of T)
		End Function

		Private Class SpliteratorAnonymousInnerClassHelper(Of T)
			Implements Spliterator(Of T)

			Friend est As Long = 1

			Public Overrides Function trySplit() As Spliterator(Of T) Implements Spliterator(Of T).trySplit
				Return Nothing
			End Function

'JAVA TO VB CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
			Public Overrides Function tryAdvance(Of T1)(ByVal consumer As java.util.function.Consumer(Of T1)) As Boolean Implements Spliterator(Of T).tryAdvance
				Objects.requireNonNull(consumer)
				If est > 0 Then
					est -= 1
					consumer.accept(element)
					Return True
				End If
				Return False
			End Function

'JAVA TO VB CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
			Public Overrides Sub forEachRemaining(Of T1)(ByVal consumer As java.util.function.Consumer(Of T1)) Implements Spliterator(Of T).forEachRemaining
				tryAdvance(consumer)
			End Sub

			Public Overrides Function estimateSize() As Long Implements Spliterator(Of T).estimateSize
				Return est
			End Function

			Public Overrides Function characteristics() As Integer Implements Spliterator(Of T).characteristics
				Dim value As Integer = If(element IsNot Nothing, Spliterator.NONNULL, 0)

				Return value Or Spliterator.SIZED Or Spliterator.SUBSIZED Or Spliterator.IMMUTABLE Or Spliterator.DISTINCT Or Spliterator.ORDERED
			End Function
		End Class

		''' <summary>
		''' @serial include
		''' </summary>
		<Serializable> _
		Private Class SingletonSet(Of E)
			Inherits AbstractSet(Of E)

			Private Const serialVersionUID As Long = 3193687207550431679L

			Private ReadOnly element As E

			Friend Sub New(ByVal e As E)
				element = e
			End Sub

			Public Overridable Function [iterator]() As [Iterator](Of E)
				Return singletonIterator(element)
			End Function

			Public Overridable Function size() As Integer
				Return 1
			End Function

			Public Overridable Function contains(ByVal o As Object) As Boolean
				Return eq(o, element)
			End Function

			' Override default methods for Collection
'JAVA TO VB CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
			Public Overrides Sub forEach(Of T1)(ByVal action As java.util.function.Consumer(Of T1))
				action.accept(element)
			End Sub
			Public Overrides Function spliterator() As Spliterator(Of E)
				Return singletonSpliterator(element)
			End Function
'JAVA TO VB CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
			Public Overrides Function removeIf(Of T1)(ByVal filter As java.util.function.Predicate(Of T1)) As Boolean
				Throw New UnsupportedOperationException
			End Function
		End Class

		''' <summary>
		''' Returns an immutable list containing only the specified object.
		''' The returned list is serializable.
		''' </summary>
		''' @param  <T> the class of the objects in the list </param>
		''' <param name="o"> the sole object to be stored in the returned list. </param>
		''' <returns> an immutable list containing only the specified object.
		''' @since 1.3 </returns>
		Public Shared Function singletonList(Of T)(ByVal o As T) As List(Of T)
			Return New SingletonList(Of )(o)
		End Function

		''' <summary>
		''' @serial include
		''' </summary>
		<Serializable> _
		Private Class SingletonList(Of E)
			Inherits AbstractList(Of E)
			Implements RandomAccess

			Private Const serialVersionUID As Long = 3093736618740652951L

			Private ReadOnly element As E

			Friend Sub New(ByVal obj As E)
				element = obj
			End Sub

			Public Overridable Function [iterator]() As [Iterator](Of E)
				Return singletonIterator(element)
			End Function

			Public Overridable Function size() As Integer
				Return 1
			End Function

			Public Overridable Function contains(ByVal obj As Object) As Boolean
				Return eq(obj, element)
			End Function

			Public Overridable Function [get](ByVal index As Integer) As E
				If index <> 0 Then Throw New IndexOutOfBoundsException("Index: " & index & ", Size: 1")
				Return element
			End Function

			' Override default methods for Collection
'JAVA TO VB CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
			Public Overrides Sub forEach(Of T1)(ByVal action As java.util.function.Consumer(Of T1))
				action.accept(element)
			End Sub
'JAVA TO VB CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
			Public Overrides Function removeIf(Of T1)(ByVal filter As java.util.function.Predicate(Of T1)) As Boolean
				Throw New UnsupportedOperationException
			End Function
			Public Overrides Sub replaceAll(ByVal [operator] As java.util.function.UnaryOperator(Of E))
				Throw New UnsupportedOperationException
			End Sub
'JAVA TO VB CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
			Public Overrides Sub sort(Of T1)(ByVal c As Comparator(Of T1))
			End Sub
			Public Overrides Function spliterator() As Spliterator(Of E)
				Return singletonSpliterator(element)
			End Function
		End Class

		''' <summary>
		''' Returns an immutable map, mapping only the specified key to the
		''' specified value.  The returned map is serializable.
		''' </summary>
		''' @param <K> the class of the map keys </param>
		''' @param <V> the class of the map values </param>
		''' <param name="key"> the sole key to be stored in the returned map. </param>
		''' <param name="value"> the value to which the returned map maps <tt>key</tt>. </param>
		''' <returns> an immutable map containing only the specified key-value
		'''         mapping.
		''' @since 1.3 </returns>
		Public Shared Function singletonMap(Of K, V)(ByVal key As K, ByVal value As V) As Map(Of K, V)
			Return New SingletonMap(Of )(key, value)
		End Function

		''' <summary>
		''' @serial include
		''' </summary>
		<Serializable> _
		Private Class SingletonMap(Of K, V)
			Inherits AbstractMap(Of K, V)

			Private Const serialVersionUID As Long = -6979724477215052911L

			Private ReadOnly k As K
			Private ReadOnly v As V

			Friend Sub New(ByVal key As K, ByVal value As V)
				k = key
				v = value
			End Sub

			Public Overridable Function size() As Integer
				Return 1
			End Function
			Public Overridable Property empty As Boolean
				Get
					Return False
				End Get
			End Property
			Public Overridable Function containsKey(ByVal key As Object) As Boolean
				Return eq(key, k)
			End Function
			Public Overridable Function containsValue(ByVal value As Object) As Boolean
				Return eq(value, v)
			End Function
			Public Overridable Function [get](ByVal key As Object) As V
				Return (If(eq(key, k), v, Nothing))
			End Function

			<NonSerialized> _
			Private keySet_Renamed As [Set](Of K)
			<NonSerialized> _
			Private entrySet_Renamed As [Set](Of KeyValuePair(Of K, V))
			<NonSerialized> _
			Private values_Renamed As Collection(Of V)

			Public Overridable Function keySet() As [Set](Of K)
				If keySet_Renamed Is Nothing Then keySet_Renamed = singleton(k)
				Return keySet_Renamed
			End Function

			Public Overridable Function entrySet() As [Set](Of KeyValuePair(Of K, V))
				If entrySet_Renamed Is Nothing Then entrySet_Renamed = Collections.singleton(Of KeyValuePair(Of K, V))(New SimpleImmutableEntry(Of )(k, v))
				Return entrySet_Renamed
			End Function

			Public Overridable Function values() As Collection(Of V)
				If values_Renamed Is Nothing Then values_Renamed = singleton(v)
				Return values_Renamed
			End Function

			' Override default methods in Map
			Public Overrides Function getOrDefault(ByVal key As Object, ByVal defaultValue As V) As V
				Return If(eq(key, k), v, defaultValue)
			End Function

'JAVA TO VB CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
			Public Overrides Sub forEach(Of T1)(ByVal action As java.util.function.BiConsumer(Of T1))
				action.accept(k, v)
			End Sub

'JAVA TO VB CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
			Public Overrides Sub replaceAll(Of T1 As V)(ByVal [function] As java.util.function.BiFunction(Of T1))
				Throw New UnsupportedOperationException
			End Sub

			Public Overrides Function putIfAbsent(ByVal key As K, ByVal value As V) As V
				Throw New UnsupportedOperationException
			End Function

			Public Overrides Function remove(ByVal key As Object, ByVal value As Object) As Boolean
				Throw New UnsupportedOperationException
			End Function

			Public Overrides Function replace(ByVal key As K, ByVal oldValue As V, ByVal newValue As V) As Boolean
				Throw New UnsupportedOperationException
			End Function

			Public Overrides Function replace(ByVal key As K, ByVal value As V) As V
				Throw New UnsupportedOperationException
			End Function

'JAVA TO VB CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
			Public Overrides Function computeIfAbsent(Of T1 As V)(ByVal key As K, ByVal mappingFunction As java.util.function.Function(Of T1)) As V
				Throw New UnsupportedOperationException
			End Function

'JAVA TO VB CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
			Public Overrides Function computeIfPresent(Of T1 As V)(ByVal key As K, ByVal remappingFunction As java.util.function.BiFunction(Of T1)) As V
				Throw New UnsupportedOperationException
			End Function

'JAVA TO VB CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
			Public Overrides Function compute(Of T1 As V)(ByVal key As K, ByVal remappingFunction As java.util.function.BiFunction(Of T1)) As V
				Throw New UnsupportedOperationException
			End Function

'JAVA TO VB CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
			Public Overrides Function merge(Of T1 As V)(ByVal key As K, ByVal value As V, ByVal remappingFunction As java.util.function.BiFunction(Of T1)) As V
				Throw New UnsupportedOperationException
			End Function
		End Class

		' Miscellaneous

		''' <summary>
		''' Returns an immutable list consisting of <tt>n</tt> copies of the
		''' specified object.  The newly allocated data object is tiny (it contains
		''' a single reference to the data object).  This method is useful in
		''' combination with the <tt>List.addAll</tt> method to grow lists.
		''' The returned list is serializable.
		''' </summary>
		''' @param  <T> the class of the object to copy and of the objects
		'''         in the returned list. </param>
		''' <param name="n"> the number of elements in the returned list. </param>
		''' <param name="o"> the element to appear repeatedly in the returned list. </param>
		''' <returns> an immutable list consisting of <tt>n</tt> copies of the
		'''         specified object. </returns>
		''' <exception cref="IllegalArgumentException"> if {@code n < 0} </exception>
		''' <seealso cref=    List#addAll(Collection) </seealso>
		''' <seealso cref=    List#addAll(int, Collection) </seealso>
		Public Shared Function nCopies(Of T)(ByVal n As Integer, ByVal o As T) As List(Of T)
			If n < 0 Then Throw New IllegalArgumentException("List length = " & n)
			Return New CopiesList(Of )(n, o)
		End Function

		''' <summary>
		''' @serial include
		''' </summary>
		<Serializable> _
		Private Class CopiesList(Of E)
			Inherits AbstractList(Of E)
			Implements RandomAccess

			Private Const serialVersionUID As Long = 2739099268398711800L

			Friend ReadOnly n As Integer
			Friend ReadOnly element As E

			Friend Sub New(ByVal n As Integer, ByVal e As E)
				Debug.Assert(n >= 0)
				Me.n = n
				element = e
			End Sub

			Public Overridable Function size() As Integer
				Return n
			End Function

			Public Overridable Function contains(ByVal obj As Object) As Boolean
				Return n <> 0 AndAlso eq(obj, element)
			End Function

			Public Overridable Function indexOf(ByVal o As Object) As Integer
				Return If(contains(o), 0, -1)
			End Function

			Public Overridable Function lastIndexOf(ByVal o As Object) As Integer
				Return If(contains(o), n - 1, -1)
			End Function

			Public Overridable Function [get](ByVal index As Integer) As E
				If index < 0 OrElse index >= n Then Throw New IndexOutOfBoundsException("Index: " & index & ", Size: " & n)
				Return element
			End Function

			Public Overridable Function toArray() As Object()
				Dim a As Object() = New Object(n - 1){}
				If element IsNot Nothing Then Arrays.fill(a, 0, n, element)
				Return a
			End Function

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
			Public Overridable Function toArray(Of T)(ByVal a As T()) As T()
				Dim n As Integer = Me.n
				If a.Length < n Then
					a = CType(java.lang.reflect.Array.newInstance(a.GetType().GetElementType(), n), T())
					If element IsNot Nothing Then Arrays.fill(a, 0, n, element)
				Else
					Arrays.fill(a, 0, n, element)
					If a.Length > n Then a(n) = Nothing
				End If
				Return a
			End Function

			Public Overridable Function subList(ByVal fromIndex As Integer, ByVal toIndex As Integer) As List(Of E)
				If fromIndex < 0 Then Throw New IndexOutOfBoundsException("fromIndex = " & fromIndex)
				If toIndex > n Then Throw New IndexOutOfBoundsException("toIndex = " & toIndex)
				If fromIndex > toIndex Then Throw New IllegalArgumentException("fromIndex(" & fromIndex & ") > toIndex(" & toIndex & ")")
				Return New CopiesList(Of )(toIndex - fromIndex, element)
			End Function

			' Override default methods in Collection
			Public Overrides Function stream() As java.util.stream.Stream(Of E)
				Return java.util.stream.IntStream.range(0, n).mapToObj(i -> element)
			End Function

			Public Overrides Function parallelStream() As java.util.stream.Stream(Of E)
				Return java.util.stream.IntStream.range(0, n).parallel().mapToObj(i -> element)
			End Function

			Public Overrides Function spliterator() As Spliterator(Of E)
				Return stream().spliterator()
			End Function
		End Class

		''' <summary>
		''' Returns a comparator that imposes the reverse of the <em>natural
		''' ordering</em> on a collection of objects that implement the
		''' {@code Comparable} interface.  (The natural ordering is the ordering
		''' imposed by the objects' own {@code compareTo} method.)  This enables a
		''' simple idiom for sorting (or maintaining) collections (or arrays) of
		''' objects that implement the {@code Comparable} interface in
		''' reverse-natural-order.  For example, suppose {@code a} is an array of
		''' strings. Then: <pre>
		'''          Arrays.sort(a, Collections.reverseOrder());
		''' </pre> sorts the array in reverse-lexicographic (alphabetical) order.<p>
		''' 
		''' The returned comparator is serializable.
		''' </summary>
		''' @param  <T> the class of the objects compared by the comparator </param>
		''' <returns> A comparator that imposes the reverse of the <i>natural
		'''         ordering</i> on a collection of objects that implement
		'''         the <tt>Comparable</tt> interface. </returns>
		''' <seealso cref= Comparable </seealso>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
		Public Shared Function reverseOrder(Of T)() As Comparator(Of T)
			Return CType(ReverseComparator.REVERSE_ORDER, Comparator(Of T))
		End Function

		''' <summary>
		''' @serial include
		''' </summary>
		<Serializable> _
		Private Class ReverseComparator
			Implements Comparator(Of Comparable(Of Object))

			Private Const serialVersionUID As Long = 7207038068494060240L

			Friend Shared ReadOnly REVERSE_ORDER As New ReverseComparator

			Public Overridable Function compare(ByVal c1 As Comparable(Of Object), ByVal c2 As Comparable(Of Object)) As Integer
				Return c2.CompareTo(c1)
			End Function

			Private Function readResolve() As Object
				Return Collections.reverseOrder()
			End Function

			Public Overrides Function reversed() As Comparator(Of Comparable(Of Object)) Implements Comparator(Of Comparable(Of Object)).reversed
				Return Comparator.naturalOrder()
			End Function
		End Class

		''' <summary>
		''' Returns a comparator that imposes the reverse ordering of the specified
		''' comparator.  If the specified comparator is {@code null}, this method is
		''' equivalent to <seealso cref="#reverseOrder()"/> (in other words, it returns a
		''' comparator that imposes the reverse of the <em>natural ordering</em> on
		''' a collection of objects that implement the Comparable interface).
		''' 
		''' <p>The returned comparator is serializable (assuming the specified
		''' comparator is also serializable or {@code null}).
		''' </summary>
		''' @param <T> the class of the objects compared by the comparator </param>
		''' <param name="cmp"> a comparator who's ordering is to be reversed by the returned
		''' comparator or {@code null} </param>
		''' <returns> A comparator that imposes the reverse ordering of the
		'''         specified comparator.
		''' @since 1.5 </returns>
		Public Shared Function reverseOrder(Of T)(ByVal cmp As Comparator(Of T)) As Comparator(Of T)
			If cmp Is Nothing Then Return reverseOrder()

			If TypeOf cmp Is ReverseComparator2 Then Return CType(cmp, ReverseComparator2(Of T)).cmp

			Return New ReverseComparator2(Of )(cmp)
		End Function

		''' <summary>
		''' @serial include
		''' </summary>
		<Serializable> _
		Private Class ReverseComparator2(Of T)
			Implements Comparator(Of T)

			Private Const serialVersionUID As Long = 4374092139857L

			''' <summary>
			''' The comparator specified in the static factory.  This will never
			''' be null, as the static factory returns a ReverseComparator
			''' instance if its argument is null.
			''' 
			''' @serial
			''' </summary>
			Friend ReadOnly cmp As Comparator(Of T)

			Friend Sub New(ByVal cmp As Comparator(Of T))
				Debug.Assert(cmp IsNot Nothing)
				Me.cmp = cmp
			End Sub

			Public Overridable Function compare(ByVal t1 As T, ByVal t2 As T) As Integer Implements Comparator(Of T).compare
				Return cmp.Compare(t2, t1)
			End Function

			Public Overrides Function Equals(ByVal o As Object) As Boolean
				Return (o Is Me) OrElse (TypeOf o Is ReverseComparator2 AndAlso cmp.Equals(CType(o, ReverseComparator2).cmp))
			End Function

			Public Overrides Function GetHashCode() As Integer
				Return cmp.GetHashCode() Xor Integer.MinValue
			End Function

			Public Overrides Function reversed() As Comparator(Of T) Implements Comparator(Of T).reversed
				Return cmp
			End Function
		End Class

		''' <summary>
		''' Returns an enumeration over the specified collection.  This provides
		''' interoperability with legacy APIs that require an enumeration
		''' as input.
		''' </summary>
		''' @param  <T> the class of the objects in the collection </param>
		''' <param name="c"> the collection for which an enumeration is to be returned. </param>
		''' <returns> an enumeration over the specified collection. </returns>
		''' <seealso cref= Enumeration </seealso>
		Public Shared Function enumeration(Of T)(ByVal c As Collection(Of T)) As Enumeration(Of T)
			Return New EnumerationAnonymousInnerClassHelper(Of E)
		End Function

		Private Class EnumerationAnonymousInnerClassHelper(Of E)
			Implements Enumeration(Of E)

			Private ReadOnly i As [Iterator](Of T) = c.GetEnumerator()

			Public Overridable Function hasMoreElements() As Boolean Implements Enumeration(Of E).hasMoreElements
				Return i.hasNext()
			End Function

			Public Overridable Function nextElement() As T
				Return i.next()
			End Function
		End Class

		''' <summary>
		''' Returns an array list containing the elements returned by the
		''' specified enumeration in the order they are returned by the
		''' enumeration.  This method provides interoperability between
		''' legacy APIs that return enumerations and new APIs that require
		''' collections.
		''' </summary>
		''' @param <T> the class of the objects returned by the enumeration </param>
		''' <param name="e"> enumeration providing elements for the returned
		'''          array list </param>
		''' <returns> an array list containing the elements returned
		'''         by the specified enumeration.
		''' @since 1.4 </returns>
		''' <seealso cref= Enumeration </seealso>
		''' <seealso cref= ArrayList </seealso>
		Public Shared Function list(Of T)(ByVal e As Enumeration(Of T)) As List(Of T)
			Dim l As New List(Of T)
			Do While e.hasMoreElements()
				l.add(e.nextElement())
			Loop
			Return l
		End Function

		''' <summary>
		''' Returns true if the specified arguments are equal, or both null.
		''' 
		''' NB: Do not replace with Object.equals until JDK-8015417 is resolved.
		''' </summary>
		Friend Shared Function eq(ByVal o1 As Object, ByVal o2 As Object) As Boolean
			Return If(o1 Is Nothing, o2 Is Nothing, o1.Equals(o2))
		End Function

		''' <summary>
		''' Returns the number of elements in the specified collection equal to the
		''' specified object.  More formally, returns the number of elements
		''' <tt>e</tt> in the collection such that
		''' <tt>(o == null ? e == null : o.equals(e))</tt>.
		''' </summary>
		''' <param name="c"> the collection in which to determine the frequency
		'''     of <tt>o</tt> </param>
		''' <param name="o"> the object whose frequency is to be determined </param>
		''' <returns> the number of elements in {@code c} equal to {@code o} </returns>
		''' <exception cref="NullPointerException"> if <tt>c</tt> is null
		''' @since 1.5 </exception>
		Public Shared Function frequency(Of T1)(ByVal c As Collection(Of T1), ByVal o As Object) As Integer
			Dim result As Integer = 0
			If o Is Nothing Then
				For Each e As Object In c
					If e Is Nothing Then result += 1
				Next e
			Else
				For Each e As Object In c
					If o.Equals(e) Then result += 1
				Next e
			End If
			Return result
		End Function

		''' <summary>
		''' Returns {@code true} if the two specified collections have no
		''' elements in common.
		''' 
		''' <p>Care must be exercised if this method is used on collections that
		''' do not comply with the general contract for {@code Collection}.
		''' Implementations may elect to iterate over either collection and test
		''' for containment in the other collection (or to perform any equivalent
		''' computation).  If either collection uses a nonstandard equality test
		''' (as does a <seealso cref="SortedSet"/> whose ordering is not <em>compatible with
		''' equals</em>, or the key set of an <seealso cref="IdentityHashMap"/>), both
		''' collections must use the same nonstandard equality test, or the
		''' result of this method is undefined.
		''' 
		''' <p>Care must also be exercised when using collections that have
		''' restrictions on the elements that they may contain. Collection
		''' implementations are allowed to throw exceptions for any operation
		''' involving elements they deem ineligible. For absolute safety the
		''' specified collections should contain only elements which are
		''' eligible elements for both collections.
		''' 
		''' <p>Note that it is permissible to pass the same collection in both
		''' parameters, in which case the method will return {@code true} if and
		''' only if the collection is empty.
		''' </summary>
		''' <param name="c1"> a collection </param>
		''' <param name="c2"> a collection </param>
		''' <returns> {@code true} if the two specified collections have no
		''' elements in common. </returns>
		''' <exception cref="NullPointerException"> if either collection is {@code null}. </exception>
		''' <exception cref="NullPointerException"> if one collection contains a {@code null}
		''' element and {@code null} is not an eligible element for the other collection.
		''' (<a href="Collection.html#optional-restrictions">optional</a>) </exception>
		''' <exception cref="ClassCastException"> if one collection contains an element that is
		''' of a type which is ineligible for the other collection.
		''' (<a href="Collection.html#optional-restrictions">optional</a>)
		''' @since 1.5 </exception>
		Public Shared Function disjoint(Of T1, T2)(ByVal c1 As Collection(Of T1), ByVal c2 As Collection(Of T2)) As Boolean
			' The collection to be used for contains(). Preference is given to
			' the collection who's contains() has lower O() complexity.
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
			Dim contains As Collection(Of ?) = c2
			' The collection to be iterated. If the collections' contains() impl
			' are of different O() complexity, the collection with slower
			' contains() will be used for iteration. For collections who's
			' contains() are of the same complexity then best performance is
			' achieved by iterating the smaller collection.
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
			Dim iterate As Collection(Of ?) = c1

			' Performance optimization cases. The heuristics:
			'   1. Generally iterate over c1.
			'   2. If c1 is a Set then iterate over c2.
			'   3. If either collection is empty then result is always true.
			'   4. Iterate over the smaller Collection.
			If TypeOf c1 Is Set Then
				' Use c1 for contains as a Set's contains() is expected to perform
				' better than O(N/2)
				iterate = c2
				contains = c1
			ElseIf Not(TypeOf c2 Is Set) Then
				' Both are mere Collections. Iterate over smaller collection.
				' Example: If c1 contains 3 elements and c2 contains 50 elements and
				' assuming contains() requires ceiling(N/2) comparisons then
				' checking for all c1 elements in c2 would require 75 comparisons
				' (3 * ceiling(50/2)) vs. checking all c2 elements in c1 requiring
				' 100 comparisons (50 * ceiling(3/2)).
				Dim c1size As Integer = c1.size()
				Dim c2size As Integer = c2.size()
				If c1size = 0 OrElse c2size = 0 Then Return True

				If c1size > c2size Then
					iterate = c2
					contains = c1
				End If
			End If

			For Each e As Object In iterate
				If contains.contains(e) Then Return False
			Next e

			' No common elements were found.
			Return True
		End Function

		''' <summary>
		''' Adds all of the specified elements to the specified collection.
		''' Elements to be added may be specified individually or as an array.
		''' The behavior of this convenience method is identical to that of
		''' <tt>c.addAll(Arrays.asList(elements))</tt>, but this method is likely
		''' to run significantly faster under most implementations.
		''' 
		''' <p>When elements are specified individually, this method provides a
		''' convenient way to add a few elements to an existing collection:
		''' <pre>
		'''     Collections.addAll(flavors, "Peaches 'n Plutonium", "Rocky Racoon");
		''' </pre>
		''' </summary>
		''' @param  <T> the class of the elements to add and of the collection </param>
		''' <param name="c"> the collection into which <tt>elements</tt> are to be inserted </param>
		''' <param name="elements"> the elements to insert into <tt>c</tt> </param>
		''' <returns> <tt>true</tt> if the collection changed as a result of the call </returns>
		''' <exception cref="UnsupportedOperationException"> if <tt>c</tt> does not support
		'''         the <tt>add</tt> operation </exception>
		''' <exception cref="NullPointerException"> if <tt>elements</tt> contains one or more
		'''         null values and <tt>c</tt> does not permit null elements, or
		'''         if <tt>c</tt> or <tt>elements</tt> are <tt>null</tt> </exception>
		''' <exception cref="IllegalArgumentException"> if some property of a value in
		'''         <tt>elements</tt> prevents it from being added to <tt>c</tt> </exception>
		''' <seealso cref= Collection#addAll(Collection)
		''' @since 1.5 </seealso>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'JAVA TO VB CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
		Public Shared Function addAll(Of T, T1)(ByVal c As Collection(Of T1), ParamArray ByVal elements As T()) As Boolean
			Dim result As Boolean = False
			For Each element As T In elements
				result = result Or c.add(element)
			Next element
			Return result
		End Function

		''' <summary>
		''' Returns a set backed by the specified map.  The resulting set displays
		''' the same ordering, concurrency, and performance characteristics as the
		''' backing map.  In essence, this factory method provides a <seealso cref="Set"/>
		''' implementation corresponding to any <seealso cref="Map"/> implementation.  There
		''' is no need to use this method on a <seealso cref="Map"/> implementation that
		''' already has a corresponding <seealso cref="Set"/> implementation (such as {@link
		''' HashMap} or <seealso cref="TreeMap"/>).
		''' 
		''' <p>Each method invocation on the set returned by this method results in
		''' exactly one method invocation on the backing map or its <tt>keySet</tt>
		''' view, with one exception.  The <tt>addAll</tt> method is implemented
		''' as a sequence of <tt>put</tt> invocations on the backing map.
		''' 
		''' <p>The specified map must be empty at the time this method is invoked,
		''' and should not be accessed directly after this method returns.  These
		''' conditions are ensured if the map is created empty, passed directly
		''' to this method, and no reference to the map is retained, as illustrated
		''' in the following code fragment:
		''' <pre>
		'''    Set&lt;Object&gt; weakHashSet = Collections.newSetFromMap(
		'''        new WeakHashMap&lt;Object, Boolean&gt;());
		''' </pre>
		''' </summary>
		''' @param <E> the class of the map keys and of the objects in the
		'''        returned set </param>
		''' <param name="map"> the backing map </param>
		''' <returns> the set backed by the map </returns>
		''' <exception cref="IllegalArgumentException"> if <tt>map</tt> is not empty
		''' @since 1.6 </exception>
		Public Shared Function newSetFromMap(Of E)(ByVal map As Map(Of E, Boolean?)) As [Set](Of E)
			Return New SetFromMap(Of )(map)
		End Function

		''' <summary>
		''' @serial include
		''' </summary>
		<Serializable> _
		Private Class SetFromMap(Of E)
			Inherits AbstractSet(Of E)
			Implements Set(Of E)

			Private ReadOnly m As Map(Of E, Boolean?) ' The backing map
			<NonSerialized> _
			Private s As [Set](Of E) ' Its keySet

			Friend Sub New(ByVal map As Map(Of E, Boolean?))
				If Not map.empty Then Throw New IllegalArgumentException("Map is non-empty")
				m = map
				s = map.Keys
			End Sub

			Public Overridable Sub clear() Implements Set(Of E).clear
				m.clear()
			End Sub
			Public Overridable Function size() As Integer Implements Set(Of E).size
				Return m.size()
			End Function
			Public Overridable Property empty As Boolean Implements Set(Of E).isEmpty
				Get
					Return m.empty
				End Get
			End Property
			Public Overridable Function contains(ByVal o As Object) As Boolean Implements Set(Of E).contains
				Return m.containsKey(o)
			End Function
			Public Overridable Function remove(ByVal o As Object) As Boolean Implements Set(Of E).remove
				Return m.remove(o) IsNot Nothing
			End Function
			Public Overridable Function add(ByVal e As E) As Boolean
				Return m.put(e, Boolean.TRUE) Is Nothing
			End Function
			Public Overridable Function [iterator]() As [Iterator](Of E) Implements Set(Of E).iterator
				Return s.GetEnumerator()
			End Function
			Public Overridable Function toArray() As Object() Implements Set(Of E).toArray
				Return s.ToArray()
			End Function
			Public Overridable Function toArray(Of T)(ByVal a As T()) As T() Implements Set(Of E).toArray
				Return s.ToArray(a)
			End Function
			Public Overrides Function ToString() As String
				Return s.ToString()
			End Function
			Public Overrides Function GetHashCode() As Integer
				Return s.GetHashCode()
			End Function
			Public Overrides Function Equals(ByVal o As Object) As Boolean
				Return o Is Me OrElse s.Equals(o)
			End Function
			Public Overridable Function containsAll(Of T1)(ByVal c As Collection(Of T1)) As Boolean Implements Set(Of E).containsAll
				Return s.containsAll(c)
			End Function
			Public Overridable Function removeAll(Of T1)(ByVal c As Collection(Of T1)) As Boolean Implements Set(Of E).removeAll
				Return s.removeAll(c)
			End Function
			Public Overridable Function retainAll(Of T1)(ByVal c As Collection(Of T1)) As Boolean Implements Set(Of E).retainAll
				Return s.retainAll(c)
			End Function
			' addAll is the only inherited implementation

			' Override default methods in Collection
'JAVA TO VB CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
			Public Overrides Sub forEach(Of T1)(ByVal action As java.util.function.Consumer(Of T1))
				s.forEach(action)
			End Sub
'JAVA TO VB CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
			Public Overrides Function removeIf(Of T1)(ByVal filter As java.util.function.Predicate(Of T1)) As Boolean
				Return s.removeIf(filter)
			End Function

			Public Overrides Function spliterator() As Spliterator(Of E) Implements Set(Of E).spliterator
				Return s.spliterator()
			End Function
			Public Overrides Function stream() As java.util.stream.Stream(Of E)
				Return s.stream()
			End Function
			Public Overrides Function parallelStream() As java.util.stream.Stream(Of E)
				Return s.parallelStream()
			End Function

			Private Const serialVersionUID As Long = 2454657854757543876L

			Private Sub readObject(ByVal stream As java.io.ObjectInputStream)
				stream.defaultReadObject()
				s = m.Keys
			End Sub
		End Class

		''' <summary>
		''' Returns a view of a <seealso cref="Deque"/> as a Last-in-first-out (Lifo)
		''' <seealso cref="Queue"/>. Method <tt>add</tt> is mapped to <tt>push</tt>,
		''' <tt>remove</tt> is mapped to <tt>pop</tt> and so on. This
		''' view can be useful when you would like to use a method
		''' requiring a <tt>Queue</tt> but you need Lifo ordering.
		''' 
		''' <p>Each method invocation on the queue returned by this method
		''' results in exactly one method invocation on the backing deque, with
		''' one exception.  The <seealso cref="Queue#addAll addAll"/> method is
		''' implemented as a sequence of <seealso cref="Deque#addFirst addFirst"/>
		''' invocations on the backing deque.
		''' </summary>
		''' @param  <T> the class of the objects in the deque </param>
		''' <param name="deque"> the deque </param>
		''' <returns> the queue
		''' @since  1.6 </returns>
		Public Shared Function asLifoQueue(Of T)(ByVal deque As Deque(Of T)) As Queue(Of T)
			Return New AsLIFOQueue(Of )(deque)
		End Function

		''' <summary>
		''' @serial include
		''' </summary>
		<Serializable> _
		Friend Class AsLIFOQueue(Of E)
			Inherits AbstractQueue(Of E)
			Implements Queue(Of E)

			Private Const serialVersionUID As Long = 1802017725587941708L
			Private ReadOnly q As Deque(Of E)
			Friend Sub New(ByVal q As Deque(Of E))
				Me.q = q
			End Sub
			Public Overridable Function add(ByVal e As E) As Boolean
				q.addFirst(e)
				Return True
			End Function
			Public Overridable Function offer(ByVal e As E) As Boolean
				Return q.offerFirst(e)
			End Function
			Public Overridable Function poll() As E Implements Queue(Of E).poll
				Return q.pollFirst()
			End Function
			Public Overridable Function remove() As E Implements Queue(Of E).remove
				Return q.removeFirst()
			End Function
			Public Overridable Function peek() As E Implements Queue(Of E).peek
				Return q.peekFirst()
			End Function
			Public Overridable Function element() As E Implements Queue(Of E).element
				Return q.first
			End Function
			Public Overridable Sub clear()
				q.clear()
			End Sub
			Public Overridable Function size() As Integer
				Return q.size()
			End Function
			Public Overridable Property empty As Boolean
				Get
					Return q.empty
				End Get
			End Property
			Public Overridable Function contains(ByVal o As Object) As Boolean
				Return q.contains(o)
			End Function
			Public Overridable Function remove(ByVal o As Object) As Boolean
				Return q.remove(o)
			End Function
			Public Overridable Function [iterator]() As [Iterator](Of E)
				Return q.GetEnumerator()
			End Function
			Public Overridable Function toArray() As Object()
				Return q.ToArray()
			End Function
			Public Overridable Function toArray(Of T)(ByVal a As T()) As T()
				Return q.ToArray(a)
			End Function
			Public Overrides Function ToString() As String
				Return q.ToString()
			End Function
			Public Overridable Function containsAll(Of T1)(ByVal c As Collection(Of T1)) As Boolean
				Return q.containsAll(c)
			End Function
			Public Overridable Function removeAll(Of T1)(ByVal c As Collection(Of T1)) As Boolean
				Return q.removeAll(c)
			End Function
			Public Overridable Function retainAll(Of T1)(ByVal c As Collection(Of T1)) As Boolean
				Return q.retainAll(c)
			End Function
			' We use inherited addAll; forwarding addAll would be wrong

			' Override default methods in Collection
'JAVA TO VB CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
			Public Overrides Sub forEach(Of T1)(ByVal action As java.util.function.Consumer(Of T1))
				q.forEach(action)
			End Sub
'JAVA TO VB CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
			Public Overrides Function removeIf(Of T1)(ByVal filter As java.util.function.Predicate(Of T1)) As Boolean
				Return q.removeIf(filter)
			End Function
			Public Overrides Function spliterator() As Spliterator(Of E)
				Return q.spliterator()
			End Function
			Public Overrides Function stream() As java.util.stream.Stream(Of E)
				Return q.stream()
			End Function
			Public Overrides Function parallelStream() As java.util.stream.Stream(Of E)
				Return q.parallelStream()
			End Function
		End Class
	End Class

End Namespace