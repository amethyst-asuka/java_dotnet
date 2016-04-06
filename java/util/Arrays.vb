Imports Microsoft.VisualBasic
Imports System
Imports System.Diagnostics
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
	''' This class contains various methods for manipulating arrays (such as
	''' sorting and searching). This class also contains a static factory
	''' that allows arrays to be viewed as lists.
	''' 
	''' <p>The methods in this class all throw a {@code NullPointerException},
	''' if the specified array reference is null, except where noted.
	''' 
	''' <p>The documentation for the methods contained in this class includes
	''' briefs description of the <i>implementations</i>. Such descriptions should
	''' be regarded as <i>implementation notes</i>, rather than parts of the
	''' <i>specification</i>. Implementors should feel free to substitute other
	''' algorithms, so long as the specification itself is adhered to. (For
	''' example, the algorithm used by {@code sort(Object[])} does not have to be
	''' a MergeSort, but it does have to be <i>stable</i>.)
	''' 
	''' <p>This class is a member of the
	''' <a href="{@docRoot}/../technotes/guides/collections/index.html">
	''' Java Collections Framework</a>.
	''' 
	''' @author Josh Bloch
	''' @author Neal Gafter
	''' @author John Rose
	''' @since  1.2
	''' </summary>
	Public Class Arrays

		''' <summary>
		''' The minimum array length below which a parallel sorting
		''' algorithm will not further partition the sorting task. Using
		''' smaller sizes typically results in memory contention across
		''' tasks that makes parallel speedups unlikely.
		''' </summary>
		Private Shared ReadOnly MIN_ARRAY_SORT_GRAN As Integer = 1 << 13

		' Suppresses default constructor, ensuring non-instantiability.
		Private Sub New()
		End Sub

		''' <summary>
		''' A comparator that implements the natural ordering of a group of
		''' mutually comparable elements. May be used when a supplied
		''' comparator is null. To simplify code-sharing within underlying
		''' implementations, the compare method only declares type Object
		''' for its second argument.
		''' 
		''' Arrays class implementor's note: It is an empirical matter
		''' whether ComparableTimSort offers any performance benefit over
		''' TimSort used with this comparator.  If not, you are better off
		''' deleting or bypassing ComparableTimSort.  There is currently no
		''' empirical case for separating them for parallel sorting, so all
		''' public Object parallelSort methods use the same comparator
		''' based implementation.
		''' </summary>
		Friend NotInheritable Class NaturalOrder
			Implements Comparator(Of Object)

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
			Public Function compare(  first As Object,   second As Object) As Integer Implements Comparator(Of Object).compare
				Return CType(first, Comparable(Of Object)).CompareTo(second)
			End Function
			Friend Shared ReadOnly INSTANCE As New NaturalOrder
		End Class

		''' <summary>
		''' Checks that {@code fromIndex} and {@code toIndex} are in
		''' the range and throws an exception if they aren't.
		''' </summary>
		Private Shared Sub rangeCheck(  arrayLength As Integer,   fromIndex As Integer,   toIndex As Integer)
			If fromIndex > toIndex Then Throw New IllegalArgumentException("fromIndex(" & fromIndex & ") > toIndex(" & toIndex & ")")
			If fromIndex < 0 Then Throw New ArrayIndexOutOfBoundsException(fromIndex)
			If toIndex > arrayLength Then Throw New ArrayIndexOutOfBoundsException(toIndex)
		End Sub

	'    
	'     * Sorting methods. Note that all public "sort" methods take the
	'     * same form: Performing argument checks if necessary, and then
	'     * expanding arguments into those required for the internal
	'     * implementation methods residing in other package-private
	'     * classes (except for legacyMergeSort, included in this [Class]).
	'     

		''' <summary>
		''' Sorts the specified array into ascending numerical order.
		''' 
		''' <p>Implementation note: The sorting algorithm is a Dual-Pivot Quicksort
		''' by Vladimir Yaroslavskiy, Jon Bentley, and Joshua Bloch. This algorithm
		''' offers O(n log(n)) performance on many data sets that cause other
		''' quicksorts to degrade to quadratic performance, and is typically
		''' faster than traditional (one-pivot) Quicksort implementations.
		''' </summary>
		''' <param name="a"> the array to be sorted </param>
		Public Shared Sub sort(  a As Integer())
			DualPivotQuicksort.sort(a, 0, a.Length - 1, Nothing, 0, 0)
		End Sub

		''' <summary>
		''' Sorts the specified range of the array into ascending order. The range
		''' to be sorted extends from the index {@code fromIndex}, inclusive, to
		''' the index {@code toIndex}, exclusive. If {@code fromIndex == toIndex},
		''' the range to be sorted is empty.
		''' 
		''' <p>Implementation note: The sorting algorithm is a Dual-Pivot Quicksort
		''' by Vladimir Yaroslavskiy, Jon Bentley, and Joshua Bloch. This algorithm
		''' offers O(n log(n)) performance on many data sets that cause other
		''' quicksorts to degrade to quadratic performance, and is typically
		''' faster than traditional (one-pivot) Quicksort implementations.
		''' </summary>
		''' <param name="a"> the array to be sorted </param>
		''' <param name="fromIndex"> the index of the first element, inclusive, to be sorted </param>
		''' <param name="toIndex"> the index of the last element, exclusive, to be sorted
		''' </param>
		''' <exception cref="IllegalArgumentException"> if {@code fromIndex > toIndex} </exception>
		''' <exception cref="ArrayIndexOutOfBoundsException">
		'''     if {@code fromIndex < 0} or {@code toIndex > a.length} </exception>
		Public Shared Sub sort(  a As Integer(),   fromIndex As Integer,   toIndex As Integer)
			rangeCheck(a.Length, fromIndex, toIndex)
			DualPivotQuicksort.sort(a, fromIndex, toIndex - 1, Nothing, 0, 0)
		End Sub

		''' <summary>
		''' Sorts the specified array into ascending numerical order.
		''' 
		''' <p>Implementation note: The sorting algorithm is a Dual-Pivot Quicksort
		''' by Vladimir Yaroslavskiy, Jon Bentley, and Joshua Bloch. This algorithm
		''' offers O(n log(n)) performance on many data sets that cause other
		''' quicksorts to degrade to quadratic performance, and is typically
		''' faster than traditional (one-pivot) Quicksort implementations.
		''' </summary>
		''' <param name="a"> the array to be sorted </param>
		Public Shared Sub sort(  a As Long())
			DualPivotQuicksort.sort(a, 0, a.Length - 1, Nothing, 0, 0)
		End Sub

		''' <summary>
		''' Sorts the specified range of the array into ascending order. The range
		''' to be sorted extends from the index {@code fromIndex}, inclusive, to
		''' the index {@code toIndex}, exclusive. If {@code fromIndex == toIndex},
		''' the range to be sorted is empty.
		''' 
		''' <p>Implementation note: The sorting algorithm is a Dual-Pivot Quicksort
		''' by Vladimir Yaroslavskiy, Jon Bentley, and Joshua Bloch. This algorithm
		''' offers O(n log(n)) performance on many data sets that cause other
		''' quicksorts to degrade to quadratic performance, and is typically
		''' faster than traditional (one-pivot) Quicksort implementations.
		''' </summary>
		''' <param name="a"> the array to be sorted </param>
		''' <param name="fromIndex"> the index of the first element, inclusive, to be sorted </param>
		''' <param name="toIndex"> the index of the last element, exclusive, to be sorted
		''' </param>
		''' <exception cref="IllegalArgumentException"> if {@code fromIndex > toIndex} </exception>
		''' <exception cref="ArrayIndexOutOfBoundsException">
		'''     if {@code fromIndex < 0} or {@code toIndex > a.length} </exception>
		Public Shared Sub sort(  a As Long(),   fromIndex As Integer,   toIndex As Integer)
			rangeCheck(a.Length, fromIndex, toIndex)
			DualPivotQuicksort.sort(a, fromIndex, toIndex - 1, Nothing, 0, 0)
		End Sub

		''' <summary>
		''' Sorts the specified array into ascending numerical order.
		''' 
		''' <p>Implementation note: The sorting algorithm is a Dual-Pivot Quicksort
		''' by Vladimir Yaroslavskiy, Jon Bentley, and Joshua Bloch. This algorithm
		''' offers O(n log(n)) performance on many data sets that cause other
		''' quicksorts to degrade to quadratic performance, and is typically
		''' faster than traditional (one-pivot) Quicksort implementations.
		''' </summary>
		''' <param name="a"> the array to be sorted </param>
		Public Shared Sub sort(  a As Short())
			DualPivotQuicksort.sort(a, 0, a.Length - 1, Nothing, 0, 0)
		End Sub

		''' <summary>
		''' Sorts the specified range of the array into ascending order. The range
		''' to be sorted extends from the index {@code fromIndex}, inclusive, to
		''' the index {@code toIndex}, exclusive. If {@code fromIndex == toIndex},
		''' the range to be sorted is empty.
		''' 
		''' <p>Implementation note: The sorting algorithm is a Dual-Pivot Quicksort
		''' by Vladimir Yaroslavskiy, Jon Bentley, and Joshua Bloch. This algorithm
		''' offers O(n log(n)) performance on many data sets that cause other
		''' quicksorts to degrade to quadratic performance, and is typically
		''' faster than traditional (one-pivot) Quicksort implementations.
		''' </summary>
		''' <param name="a"> the array to be sorted </param>
		''' <param name="fromIndex"> the index of the first element, inclusive, to be sorted </param>
		''' <param name="toIndex"> the index of the last element, exclusive, to be sorted
		''' </param>
		''' <exception cref="IllegalArgumentException"> if {@code fromIndex > toIndex} </exception>
		''' <exception cref="ArrayIndexOutOfBoundsException">
		'''     if {@code fromIndex < 0} or {@code toIndex > a.length} </exception>
		Public Shared Sub sort(  a As Short(),   fromIndex As Integer,   toIndex As Integer)
			rangeCheck(a.Length, fromIndex, toIndex)
			DualPivotQuicksort.sort(a, fromIndex, toIndex - 1, Nothing, 0, 0)
		End Sub

		''' <summary>
		''' Sorts the specified array into ascending numerical order.
		''' 
		''' <p>Implementation note: The sorting algorithm is a Dual-Pivot Quicksort
		''' by Vladimir Yaroslavskiy, Jon Bentley, and Joshua Bloch. This algorithm
		''' offers O(n log(n)) performance on many data sets that cause other
		''' quicksorts to degrade to quadratic performance, and is typically
		''' faster than traditional (one-pivot) Quicksort implementations.
		''' </summary>
		''' <param name="a"> the array to be sorted </param>
		Public Shared Sub sort(  a As Char())
			DualPivotQuicksort.sort(a, 0, a.Length - 1, Nothing, 0, 0)
		End Sub

		''' <summary>
		''' Sorts the specified range of the array into ascending order. The range
		''' to be sorted extends from the index {@code fromIndex}, inclusive, to
		''' the index {@code toIndex}, exclusive. If {@code fromIndex == toIndex},
		''' the range to be sorted is empty.
		''' 
		''' <p>Implementation note: The sorting algorithm is a Dual-Pivot Quicksort
		''' by Vladimir Yaroslavskiy, Jon Bentley, and Joshua Bloch. This algorithm
		''' offers O(n log(n)) performance on many data sets that cause other
		''' quicksorts to degrade to quadratic performance, and is typically
		''' faster than traditional (one-pivot) Quicksort implementations.
		''' </summary>
		''' <param name="a"> the array to be sorted </param>
		''' <param name="fromIndex"> the index of the first element, inclusive, to be sorted </param>
		''' <param name="toIndex"> the index of the last element, exclusive, to be sorted
		''' </param>
		''' <exception cref="IllegalArgumentException"> if {@code fromIndex > toIndex} </exception>
		''' <exception cref="ArrayIndexOutOfBoundsException">
		'''     if {@code fromIndex < 0} or {@code toIndex > a.length} </exception>
		Public Shared Sub sort(  a As Char(),   fromIndex As Integer,   toIndex As Integer)
			rangeCheck(a.Length, fromIndex, toIndex)
			DualPivotQuicksort.sort(a, fromIndex, toIndex - 1, Nothing, 0, 0)
		End Sub

		''' <summary>
		''' Sorts the specified array into ascending numerical order.
		''' 
		''' <p>Implementation note: The sorting algorithm is a Dual-Pivot Quicksort
		''' by Vladimir Yaroslavskiy, Jon Bentley, and Joshua Bloch. This algorithm
		''' offers O(n log(n)) performance on many data sets that cause other
		''' quicksorts to degrade to quadratic performance, and is typically
		''' faster than traditional (one-pivot) Quicksort implementations.
		''' </summary>
		''' <param name="a"> the array to be sorted </param>
		Public Shared Sub sort(  a As SByte())
			DualPivotQuicksort.sort(a, 0, a.Length - 1)
		End Sub

		''' <summary>
		''' Sorts the specified range of the array into ascending order. The range
		''' to be sorted extends from the index {@code fromIndex}, inclusive, to
		''' the index {@code toIndex}, exclusive. If {@code fromIndex == toIndex},
		''' the range to be sorted is empty.
		''' 
		''' <p>Implementation note: The sorting algorithm is a Dual-Pivot Quicksort
		''' by Vladimir Yaroslavskiy, Jon Bentley, and Joshua Bloch. This algorithm
		''' offers O(n log(n)) performance on many data sets that cause other
		''' quicksorts to degrade to quadratic performance, and is typically
		''' faster than traditional (one-pivot) Quicksort implementations.
		''' </summary>
		''' <param name="a"> the array to be sorted </param>
		''' <param name="fromIndex"> the index of the first element, inclusive, to be sorted </param>
		''' <param name="toIndex"> the index of the last element, exclusive, to be sorted
		''' </param>
		''' <exception cref="IllegalArgumentException"> if {@code fromIndex > toIndex} </exception>
		''' <exception cref="ArrayIndexOutOfBoundsException">
		'''     if {@code fromIndex < 0} or {@code toIndex > a.length} </exception>
		Public Shared Sub sort(  a As SByte(),   fromIndex As Integer,   toIndex As Integer)
			rangeCheck(a.Length, fromIndex, toIndex)
			DualPivotQuicksort.sort(a, fromIndex, toIndex - 1)
		End Sub

		''' <summary>
		''' Sorts the specified array into ascending numerical order.
		''' 
		''' <p>The {@code <} relation does not provide a total order on all float
		''' values: {@code -0.0f == 0.0f} is {@code true} and a {@code Float.NaN}
		''' value compares neither less than, greater than, nor equal to any value,
		''' even itself. This method uses the total order imposed by the method
		''' <seealso cref="Float#compareTo"/>: {@code -0.0f} is treated as less than value
		''' {@code 0.0f} and {@code Float.NaN} is considered greater than any
		''' other value and all {@code Float.NaN} values are considered equal.
		''' 
		''' <p>Implementation note: The sorting algorithm is a Dual-Pivot Quicksort
		''' by Vladimir Yaroslavskiy, Jon Bentley, and Joshua Bloch. This algorithm
		''' offers O(n log(n)) performance on many data sets that cause other
		''' quicksorts to degrade to quadratic performance, and is typically
		''' faster than traditional (one-pivot) Quicksort implementations.
		''' </summary>
		''' <param name="a"> the array to be sorted </param>
		Public Shared Sub sort(  a As Single())
			DualPivotQuicksort.sort(a, 0, a.Length - 1, Nothing, 0, 0)
		End Sub

		''' <summary>
		''' Sorts the specified range of the array into ascending order. The range
		''' to be sorted extends from the index {@code fromIndex}, inclusive, to
		''' the index {@code toIndex}, exclusive. If {@code fromIndex == toIndex},
		''' the range to be sorted is empty.
		''' 
		''' <p>The {@code <} relation does not provide a total order on all float
		''' values: {@code -0.0f == 0.0f} is {@code true} and a {@code Float.NaN}
		''' value compares neither less than, greater than, nor equal to any value,
		''' even itself. This method uses the total order imposed by the method
		''' <seealso cref="Float#compareTo"/>: {@code -0.0f} is treated as less than value
		''' {@code 0.0f} and {@code Float.NaN} is considered greater than any
		''' other value and all {@code Float.NaN} values are considered equal.
		''' 
		''' <p>Implementation note: The sorting algorithm is a Dual-Pivot Quicksort
		''' by Vladimir Yaroslavskiy, Jon Bentley, and Joshua Bloch. This algorithm
		''' offers O(n log(n)) performance on many data sets that cause other
		''' quicksorts to degrade to quadratic performance, and is typically
		''' faster than traditional (one-pivot) Quicksort implementations.
		''' </summary>
		''' <param name="a"> the array to be sorted </param>
		''' <param name="fromIndex"> the index of the first element, inclusive, to be sorted </param>
		''' <param name="toIndex"> the index of the last element, exclusive, to be sorted
		''' </param>
		''' <exception cref="IllegalArgumentException"> if {@code fromIndex > toIndex} </exception>
		''' <exception cref="ArrayIndexOutOfBoundsException">
		'''     if {@code fromIndex < 0} or {@code toIndex > a.length} </exception>
		Public Shared Sub sort(  a As Single(),   fromIndex As Integer,   toIndex As Integer)
			rangeCheck(a.Length, fromIndex, toIndex)
			DualPivotQuicksort.sort(a, fromIndex, toIndex - 1, Nothing, 0, 0)
		End Sub

		''' <summary>
		''' Sorts the specified array into ascending numerical order.
		''' 
		''' <p>The {@code <} relation does not provide a total order on all double
		''' values: {@code -0.0d == 0.0d} is {@code true} and a {@code java.lang.[Double].NaN}
		''' value compares neither less than, greater than, nor equal to any value,
		''' even itself. This method uses the total order imposed by the method
		''' <seealso cref="Double#compareTo"/>: {@code -0.0d} is treated as less than value
		''' {@code 0.0d} and {@code java.lang.[Double].NaN} is considered greater than any
		''' other value and all {@code java.lang.[Double].NaN} values are considered equal.
		''' 
		''' <p>Implementation note: The sorting algorithm is a Dual-Pivot Quicksort
		''' by Vladimir Yaroslavskiy, Jon Bentley, and Joshua Bloch. This algorithm
		''' offers O(n log(n)) performance on many data sets that cause other
		''' quicksorts to degrade to quadratic performance, and is typically
		''' faster than traditional (one-pivot) Quicksort implementations.
		''' </summary>
		''' <param name="a"> the array to be sorted </param>
		Public Shared Sub sort(  a As Double())
			DualPivotQuicksort.sort(a, 0, a.Length - 1, Nothing, 0, 0)
		End Sub

		''' <summary>
		''' Sorts the specified range of the array into ascending order. The range
		''' to be sorted extends from the index {@code fromIndex}, inclusive, to
		''' the index {@code toIndex}, exclusive. If {@code fromIndex == toIndex},
		''' the range to be sorted is empty.
		''' 
		''' <p>The {@code <} relation does not provide a total order on all double
		''' values: {@code -0.0d == 0.0d} is {@code true} and a {@code java.lang.[Double].NaN}
		''' value compares neither less than, greater than, nor equal to any value,
		''' even itself. This method uses the total order imposed by the method
		''' <seealso cref="Double#compareTo"/>: {@code -0.0d} is treated as less than value
		''' {@code 0.0d} and {@code java.lang.[Double].NaN} is considered greater than any
		''' other value and all {@code java.lang.[Double].NaN} values are considered equal.
		''' 
		''' <p>Implementation note: The sorting algorithm is a Dual-Pivot Quicksort
		''' by Vladimir Yaroslavskiy, Jon Bentley, and Joshua Bloch. This algorithm
		''' offers O(n log(n)) performance on many data sets that cause other
		''' quicksorts to degrade to quadratic performance, and is typically
		''' faster than traditional (one-pivot) Quicksort implementations.
		''' </summary>
		''' <param name="a"> the array to be sorted </param>
		''' <param name="fromIndex"> the index of the first element, inclusive, to be sorted </param>
		''' <param name="toIndex"> the index of the last element, exclusive, to be sorted
		''' </param>
		''' <exception cref="IllegalArgumentException"> if {@code fromIndex > toIndex} </exception>
		''' <exception cref="ArrayIndexOutOfBoundsException">
		'''     if {@code fromIndex < 0} or {@code toIndex > a.length} </exception>
		Public Shared Sub sort(  a As Double(),   fromIndex As Integer,   toIndex As Integer)
			rangeCheck(a.Length, fromIndex, toIndex)
			DualPivotQuicksort.sort(a, fromIndex, toIndex - 1, Nothing, 0, 0)
		End Sub

		''' <summary>
		''' Sorts the specified array into ascending numerical order.
		''' 
		''' @implNote The sorting algorithm is a parallel sort-merge that breaks the
		''' array into sub-arrays that are themselves sorted and then merged. When
		''' the sub-array length reaches a minimum granularity, the sub-array is
		''' sorted using the appropriate <seealso cref="Arrays#sort(byte[]) Arrays.sort"/>
		''' method. If the length of the specified array is less than the minimum
		''' granularity, then it is sorted using the appropriate {@link
		''' Arrays#sort(byte[]) Arrays.sort} method. The algorithm requires a
		''' working space no greater than the size of the original array. The
		''' <seealso cref="ForkJoinPool#commonPool() ForkJoin common pool"/> is used to
		''' execute any parallel tasks.
		''' </summary>
		''' <param name="a"> the array to be sorted
		''' 
		''' @since 1.8 </param>
		Public Shared Sub parallelSort(  a As SByte())
			Dim n As Integer = a.Length, p As Integer, g As Integer
			p = java.util.concurrent.ForkJoinPool.commonPoolParallelism
			If n <= MIN_ARRAY_SORT_GRAN OrElse p = 1 Then
				DualPivotQuicksort.sort(a, 0, n - 1)
			Else
'JAVA TO VB CONVERTER TODO TASK: Assignments within expressions are not supported in VB
				CType(New ArraysParallelSortHelpers.FJByte.Sorter(Nothing, a, New SByte(n - 1){}, 0, n, 0,If((g = n / (p << 2)) <= MIN_ARRAY_SORT_GRAN, MIN_ARRAY_SORT_GRAN, g)), ArraysParallelSortHelpers.FJByte.Sorter).invoke()
			End If
		End Sub

		''' <summary>
		''' Sorts the specified range of the array into ascending numerical order.
		''' The range to be sorted extends from the index {@code fromIndex},
		''' inclusive, to the index {@code toIndex}, exclusive. If
		''' {@code fromIndex == toIndex}, the range to be sorted is empty.
		''' 
		''' @implNote The sorting algorithm is a parallel sort-merge that breaks the
		''' array into sub-arrays that are themselves sorted and then merged. When
		''' the sub-array length reaches a minimum granularity, the sub-array is
		''' sorted using the appropriate <seealso cref="Arrays#sort(byte[]) Arrays.sort"/>
		''' method. If the length of the specified array is less than the minimum
		''' granularity, then it is sorted using the appropriate {@link
		''' Arrays#sort(byte[]) Arrays.sort} method. The algorithm requires a working
		''' space no greater than the size of the specified range of the original
		''' array. The <seealso cref="ForkJoinPool#commonPool() ForkJoin common pool"/> is
		''' used to execute any parallel tasks.
		''' </summary>
		''' <param name="a"> the array to be sorted </param>
		''' <param name="fromIndex"> the index of the first element, inclusive, to be sorted </param>
		''' <param name="toIndex"> the index of the last element, exclusive, to be sorted
		''' </param>
		''' <exception cref="IllegalArgumentException"> if {@code fromIndex > toIndex} </exception>
		''' <exception cref="ArrayIndexOutOfBoundsException">
		'''     if {@code fromIndex < 0} or {@code toIndex > a.length}
		''' 
		''' @since 1.8 </exception>
		Public Shared Sub parallelSort(  a As SByte(),   fromIndex As Integer,   toIndex As Integer)
			rangeCheck(a.Length, fromIndex, toIndex)
			Dim n As Integer = toIndex - fromIndex, p As Integer, g As Integer
			p = java.util.concurrent.ForkJoinPool.commonPoolParallelism
			If n <= MIN_ARRAY_SORT_GRAN OrElse p = 1 Then
				DualPivotQuicksort.sort(a, fromIndex, toIndex - 1)
			Else
'JAVA TO VB CONVERTER TODO TASK: Assignments within expressions are not supported in VB
				CType(New ArraysParallelSortHelpers.FJByte.Sorter(Nothing, a, New SByte(n - 1){}, fromIndex, n, 0,If((g = n / (p << 2)) <= MIN_ARRAY_SORT_GRAN, MIN_ARRAY_SORT_GRAN, g)), ArraysParallelSortHelpers.FJByte.Sorter).invoke()
			End If
		End Sub

		''' <summary>
		''' Sorts the specified array into ascending numerical order.
		''' 
		''' @implNote The sorting algorithm is a parallel sort-merge that breaks the
		''' array into sub-arrays that are themselves sorted and then merged. When
		''' the sub-array length reaches a minimum granularity, the sub-array is
		''' sorted using the appropriate <seealso cref="Arrays#sort(char[]) Arrays.sort"/>
		''' method. If the length of the specified array is less than the minimum
		''' granularity, then it is sorted using the appropriate {@link
		''' Arrays#sort(char[]) Arrays.sort} method. The algorithm requires a
		''' working space no greater than the size of the original array. The
		''' <seealso cref="ForkJoinPool#commonPool() ForkJoin common pool"/> is used to
		''' execute any parallel tasks.
		''' </summary>
		''' <param name="a"> the array to be sorted
		''' 
		''' @since 1.8 </param>
		Public Shared Sub parallelSort(  a As Char())
			Dim n As Integer = a.Length, p As Integer, g As Integer
			p = java.util.concurrent.ForkJoinPool.commonPoolParallelism
			If n <= MIN_ARRAY_SORT_GRAN OrElse p = 1 Then
				DualPivotQuicksort.sort(a, 0, n - 1, Nothing, 0, 0)
			Else
'JAVA TO VB CONVERTER TODO TASK: Assignments within expressions are not supported in VB
				CType(New ArraysParallelSortHelpers.FJChar.Sorter(Nothing, a, New Char(n - 1){}, 0, n, 0,If((g = n / (p << 2)) <= MIN_ARRAY_SORT_GRAN, MIN_ARRAY_SORT_GRAN, g)), ArraysParallelSortHelpers.FJChar.Sorter).invoke()
			End If
		End Sub

		''' <summary>
		''' Sorts the specified range of the array into ascending numerical order.
		''' The range to be sorted extends from the index {@code fromIndex},
		''' inclusive, to the index {@code toIndex}, exclusive. If
		''' {@code fromIndex == toIndex}, the range to be sorted is empty.
		''' 
		'''  @implNote The sorting algorithm is a parallel sort-merge that breaks the
		''' array into sub-arrays that are themselves sorted and then merged. When
		''' the sub-array length reaches a minimum granularity, the sub-array is
		''' sorted using the appropriate <seealso cref="Arrays#sort(char[]) Arrays.sort"/>
		''' method. If the length of the specified array is less than the minimum
		''' granularity, then it is sorted using the appropriate {@link
		''' Arrays#sort(char[]) Arrays.sort} method. The algorithm requires a working
		''' space no greater than the size of the specified range of the original
		''' array. The <seealso cref="ForkJoinPool#commonPool() ForkJoin common pool"/> is
		''' used to execute any parallel tasks.
		''' </summary>
		''' <param name="a"> the array to be sorted </param>
		''' <param name="fromIndex"> the index of the first element, inclusive, to be sorted </param>
		''' <param name="toIndex"> the index of the last element, exclusive, to be sorted
		''' </param>
		''' <exception cref="IllegalArgumentException"> if {@code fromIndex > toIndex} </exception>
		''' <exception cref="ArrayIndexOutOfBoundsException">
		'''     if {@code fromIndex < 0} or {@code toIndex > a.length}
		''' 
		''' @since 1.8 </exception>
		Public Shared Sub parallelSort(  a As Char(),   fromIndex As Integer,   toIndex As Integer)
			rangeCheck(a.Length, fromIndex, toIndex)
			Dim n As Integer = toIndex - fromIndex, p As Integer, g As Integer
			p = java.util.concurrent.ForkJoinPool.commonPoolParallelism
			If n <= MIN_ARRAY_SORT_GRAN OrElse p = 1 Then
				DualPivotQuicksort.sort(a, fromIndex, toIndex - 1, Nothing, 0, 0)
			Else
'JAVA TO VB CONVERTER TODO TASK: Assignments within expressions are not supported in VB
				CType(New ArraysParallelSortHelpers.FJChar.Sorter(Nothing, a, New Char(n - 1){}, fromIndex, n, 0,If((g = n / (p << 2)) <= MIN_ARRAY_SORT_GRAN, MIN_ARRAY_SORT_GRAN, g)), ArraysParallelSortHelpers.FJChar.Sorter).invoke()
			End If
		End Sub

		''' <summary>
		''' Sorts the specified array into ascending numerical order.
		''' 
		''' @implNote The sorting algorithm is a parallel sort-merge that breaks the
		''' array into sub-arrays that are themselves sorted and then merged. When
		''' the sub-array length reaches a minimum granularity, the sub-array is
		''' sorted using the appropriate <seealso cref="Arrays#sort(short[]) Arrays.sort"/>
		''' method. If the length of the specified array is less than the minimum
		''' granularity, then it is sorted using the appropriate {@link
		''' Arrays#sort(short[]) Arrays.sort} method. The algorithm requires a
		''' working space no greater than the size of the original array. The
		''' <seealso cref="ForkJoinPool#commonPool() ForkJoin common pool"/> is used to
		''' execute any parallel tasks.
		''' </summary>
		''' <param name="a"> the array to be sorted
		''' 
		''' @since 1.8 </param>
		Public Shared Sub parallelSort(  a As Short())
			Dim n As Integer = a.Length, p As Integer, g As Integer
			p = java.util.concurrent.ForkJoinPool.commonPoolParallelism
			If n <= MIN_ARRAY_SORT_GRAN OrElse p = 1 Then
				DualPivotQuicksort.sort(a, 0, n - 1, Nothing, 0, 0)
			Else
'JAVA TO VB CONVERTER TODO TASK: Assignments within expressions are not supported in VB
				CType(New ArraysParallelSortHelpers.FJ java.lang.[Short].Sorter(Nothing, a, New Short(n - 1){}, 0, n, 0,If((g = n / (p << 2)) <= MIN_ARRAY_SORT_GRAN, MIN_ARRAY_SORT_GRAN, g)), ArraysParallelSortHelpers.FJ java.lang.[Short].Sorter).invoke()
			End If
		End Sub

		''' <summary>
		''' Sorts the specified range of the array into ascending numerical order.
		''' The range to be sorted extends from the index {@code fromIndex},
		''' inclusive, to the index {@code toIndex}, exclusive. If
		''' {@code fromIndex == toIndex}, the range to be sorted is empty.
		''' 
		''' @implNote The sorting algorithm is a parallel sort-merge that breaks the
		''' array into sub-arrays that are themselves sorted and then merged. When
		''' the sub-array length reaches a minimum granularity, the sub-array is
		''' sorted using the appropriate <seealso cref="Arrays#sort(short[]) Arrays.sort"/>
		''' method. If the length of the specified array is less than the minimum
		''' granularity, then it is sorted using the appropriate {@link
		''' Arrays#sort(short[]) Arrays.sort} method. The algorithm requires a working
		''' space no greater than the size of the specified range of the original
		''' array. The <seealso cref="ForkJoinPool#commonPool() ForkJoin common pool"/> is
		''' used to execute any parallel tasks.
		''' </summary>
		''' <param name="a"> the array to be sorted </param>
		''' <param name="fromIndex"> the index of the first element, inclusive, to be sorted </param>
		''' <param name="toIndex"> the index of the last element, exclusive, to be sorted
		''' </param>
		''' <exception cref="IllegalArgumentException"> if {@code fromIndex > toIndex} </exception>
		''' <exception cref="ArrayIndexOutOfBoundsException">
		'''     if {@code fromIndex < 0} or {@code toIndex > a.length}
		''' 
		''' @since 1.8 </exception>
		Public Shared Sub parallelSort(  a As Short(),   fromIndex As Integer,   toIndex As Integer)
			rangeCheck(a.Length, fromIndex, toIndex)
			Dim n As Integer = toIndex - fromIndex, p As Integer, g As Integer
			p = java.util.concurrent.ForkJoinPool.commonPoolParallelism
			If n <= MIN_ARRAY_SORT_GRAN OrElse p = 1 Then
				DualPivotQuicksort.sort(a, fromIndex, toIndex - 1, Nothing, 0, 0)
			Else
'JAVA TO VB CONVERTER TODO TASK: Assignments within expressions are not supported in VB
				CType(New ArraysParallelSortHelpers.FJ java.lang.[Short].Sorter(Nothing, a, New Short(n - 1){}, fromIndex, n, 0,If((g = n / (p << 2)) <= MIN_ARRAY_SORT_GRAN, MIN_ARRAY_SORT_GRAN, g)), ArraysParallelSortHelpers.FJ java.lang.[Short].Sorter).invoke()
			End If
		End Sub

		''' <summary>
		''' Sorts the specified array into ascending numerical order.
		''' 
		''' @implNote The sorting algorithm is a parallel sort-merge that breaks the
		''' array into sub-arrays that are themselves sorted and then merged. When
		''' the sub-array length reaches a minimum granularity, the sub-array is
		''' sorted using the appropriate <seealso cref="Arrays#sort(int[]) Arrays.sort"/>
		''' method. If the length of the specified array is less than the minimum
		''' granularity, then it is sorted using the appropriate {@link
		''' Arrays#sort(int[]) Arrays.sort} method. The algorithm requires a
		''' working space no greater than the size of the original array. The
		''' <seealso cref="ForkJoinPool#commonPool() ForkJoin common pool"/> is used to
		''' execute any parallel tasks.
		''' </summary>
		''' <param name="a"> the array to be sorted
		''' 
		''' @since 1.8 </param>
		Public Shared Sub parallelSort(  a As Integer())
			Dim n As Integer = a.Length, p As Integer, g As Integer
			p = java.util.concurrent.ForkJoinPool.commonPoolParallelism
			If n <= MIN_ARRAY_SORT_GRAN OrElse p = 1 Then
				DualPivotQuicksort.sort(a, 0, n - 1, Nothing, 0, 0)
			Else
'JAVA TO VB CONVERTER TODO TASK: Assignments within expressions are not supported in VB
				CType(New ArraysParallelSortHelpers.FJInt.Sorter(Nothing, a, New Integer(n - 1){}, 0, n, 0,If((g = n / (p << 2)) <= MIN_ARRAY_SORT_GRAN, MIN_ARRAY_SORT_GRAN, g)), ArraysParallelSortHelpers.FJInt.Sorter).invoke()
			End If
		End Sub

		''' <summary>
		''' Sorts the specified range of the array into ascending numerical order.
		''' The range to be sorted extends from the index {@code fromIndex},
		''' inclusive, to the index {@code toIndex}, exclusive. If
		''' {@code fromIndex == toIndex}, the range to be sorted is empty.
		''' 
		''' @implNote The sorting algorithm is a parallel sort-merge that breaks the
		''' array into sub-arrays that are themselves sorted and then merged. When
		''' the sub-array length reaches a minimum granularity, the sub-array is
		''' sorted using the appropriate <seealso cref="Arrays#sort(int[]) Arrays.sort"/>
		''' method. If the length of the specified array is less than the minimum
		''' granularity, then it is sorted using the appropriate {@link
		''' Arrays#sort(int[]) Arrays.sort} method. The algorithm requires a working
		''' space no greater than the size of the specified range of the original
		''' array. The <seealso cref="ForkJoinPool#commonPool() ForkJoin common pool"/> is
		''' used to execute any parallel tasks.
		''' </summary>
		''' <param name="a"> the array to be sorted </param>
		''' <param name="fromIndex"> the index of the first element, inclusive, to be sorted </param>
		''' <param name="toIndex"> the index of the last element, exclusive, to be sorted
		''' </param>
		''' <exception cref="IllegalArgumentException"> if {@code fromIndex > toIndex} </exception>
		''' <exception cref="ArrayIndexOutOfBoundsException">
		'''     if {@code fromIndex < 0} or {@code toIndex > a.length}
		''' 
		''' @since 1.8 </exception>
		Public Shared Sub parallelSort(  a As Integer(),   fromIndex As Integer,   toIndex As Integer)
			rangeCheck(a.Length, fromIndex, toIndex)
			Dim n As Integer = toIndex - fromIndex, p As Integer, g As Integer
			p = java.util.concurrent.ForkJoinPool.commonPoolParallelism
			If n <= MIN_ARRAY_SORT_GRAN OrElse p = 1 Then
				DualPivotQuicksort.sort(a, fromIndex, toIndex - 1, Nothing, 0, 0)
			Else
'JAVA TO VB CONVERTER TODO TASK: Assignments within expressions are not supported in VB
				CType(New ArraysParallelSortHelpers.FJInt.Sorter(Nothing, a, New Integer(n - 1){}, fromIndex, n, 0,If((g = n / (p << 2)) <= MIN_ARRAY_SORT_GRAN, MIN_ARRAY_SORT_GRAN, g)), ArraysParallelSortHelpers.FJInt.Sorter).invoke()
			End If
		End Sub

		''' <summary>
		''' Sorts the specified array into ascending numerical order.
		''' 
		''' @implNote The sorting algorithm is a parallel sort-merge that breaks the
		''' array into sub-arrays that are themselves sorted and then merged. When
		''' the sub-array length reaches a minimum granularity, the sub-array is
		''' sorted using the appropriate <seealso cref="Arrays#sort(long[]) Arrays.sort"/>
		''' method. If the length of the specified array is less than the minimum
		''' granularity, then it is sorted using the appropriate {@link
		''' Arrays#sort(long[]) Arrays.sort} method. The algorithm requires a
		''' working space no greater than the size of the original array. The
		''' <seealso cref="ForkJoinPool#commonPool() ForkJoin common pool"/> is used to
		''' execute any parallel tasks.
		''' </summary>
		''' <param name="a"> the array to be sorted
		''' 
		''' @since 1.8 </param>
		Public Shared Sub parallelSort(  a As Long())
			Dim n As Integer = a.Length, p As Integer, g As Integer
			p = java.util.concurrent.ForkJoinPool.commonPoolParallelism
			If n <= MIN_ARRAY_SORT_GRAN OrElse p = 1 Then
				DualPivotQuicksort.sort(a, 0, n - 1, Nothing, 0, 0)
			Else
'JAVA TO VB CONVERTER TODO TASK: Assignments within expressions are not supported in VB
				CType(New ArraysParallelSortHelpers.FJLong.Sorter(Nothing, a, New Long(n - 1){}, 0, n, 0,If((g = n / (p << 2)) <= MIN_ARRAY_SORT_GRAN, MIN_ARRAY_SORT_GRAN, g)), ArraysParallelSortHelpers.FJLong.Sorter).invoke()
			End If
		End Sub

		''' <summary>
		''' Sorts the specified range of the array into ascending numerical order.
		''' The range to be sorted extends from the index {@code fromIndex},
		''' inclusive, to the index {@code toIndex}, exclusive. If
		''' {@code fromIndex == toIndex}, the range to be sorted is empty.
		''' 
		''' @implNote The sorting algorithm is a parallel sort-merge that breaks the
		''' array into sub-arrays that are themselves sorted and then merged. When
		''' the sub-array length reaches a minimum granularity, the sub-array is
		''' sorted using the appropriate <seealso cref="Arrays#sort(long[]) Arrays.sort"/>
		''' method. If the length of the specified array is less than the minimum
		''' granularity, then it is sorted using the appropriate {@link
		''' Arrays#sort(long[]) Arrays.sort} method. The algorithm requires a working
		''' space no greater than the size of the specified range of the original
		''' array. The <seealso cref="ForkJoinPool#commonPool() ForkJoin common pool"/> is
		''' used to execute any parallel tasks.
		''' </summary>
		''' <param name="a"> the array to be sorted </param>
		''' <param name="fromIndex"> the index of the first element, inclusive, to be sorted </param>
		''' <param name="toIndex"> the index of the last element, exclusive, to be sorted
		''' </param>
		''' <exception cref="IllegalArgumentException"> if {@code fromIndex > toIndex} </exception>
		''' <exception cref="ArrayIndexOutOfBoundsException">
		'''     if {@code fromIndex < 0} or {@code toIndex > a.length}
		''' 
		''' @since 1.8 </exception>
		Public Shared Sub parallelSort(  a As Long(),   fromIndex As Integer,   toIndex As Integer)
			rangeCheck(a.Length, fromIndex, toIndex)
			Dim n As Integer = toIndex - fromIndex, p As Integer, g As Integer
			p = java.util.concurrent.ForkJoinPool.commonPoolParallelism
			If n <= MIN_ARRAY_SORT_GRAN OrElse p = 1 Then
				DualPivotQuicksort.sort(a, fromIndex, toIndex - 1, Nothing, 0, 0)
			Else
'JAVA TO VB CONVERTER TODO TASK: Assignments within expressions are not supported in VB
				CType(New ArraysParallelSortHelpers.FJLong.Sorter(Nothing, a, New Long(n - 1){}, fromIndex, n, 0,If((g = n / (p << 2)) <= MIN_ARRAY_SORT_GRAN, MIN_ARRAY_SORT_GRAN, g)), ArraysParallelSortHelpers.FJLong.Sorter).invoke()
			End If
		End Sub

		''' <summary>
		''' Sorts the specified array into ascending numerical order.
		''' 
		''' <p>The {@code <} relation does not provide a total order on all float
		''' values: {@code -0.0f == 0.0f} is {@code true} and a {@code Float.NaN}
		''' value compares neither less than, greater than, nor equal to any value,
		''' even itself. This method uses the total order imposed by the method
		''' <seealso cref="Float#compareTo"/>: {@code -0.0f} is treated as less than value
		''' {@code 0.0f} and {@code Float.NaN} is considered greater than any
		''' other value and all {@code Float.NaN} values are considered equal.
		''' 
		''' @implNote The sorting algorithm is a parallel sort-merge that breaks the
		''' array into sub-arrays that are themselves sorted and then merged. When
		''' the sub-array length reaches a minimum granularity, the sub-array is
		''' sorted using the appropriate <seealso cref="Arrays#sort(float[]) Arrays.sort"/>
		''' method. If the length of the specified array is less than the minimum
		''' granularity, then it is sorted using the appropriate {@link
		''' Arrays#sort(float[]) Arrays.sort} method. The algorithm requires a
		''' working space no greater than the size of the original array. The
		''' <seealso cref="ForkJoinPool#commonPool() ForkJoin common pool"/> is used to
		''' execute any parallel tasks.
		''' </summary>
		''' <param name="a"> the array to be sorted
		''' 
		''' @since 1.8 </param>
		Public Shared Sub parallelSort(  a As Single())
			Dim n As Integer = a.Length, p As Integer, g As Integer
			p = java.util.concurrent.ForkJoinPool.commonPoolParallelism
			If n <= MIN_ARRAY_SORT_GRAN OrElse p = 1 Then
				DualPivotQuicksort.sort(a, 0, n - 1, Nothing, 0, 0)
			Else
'JAVA TO VB CONVERTER TODO TASK: Assignments within expressions are not supported in VB
				CType(New ArraysParallelSortHelpers.FJFloat.Sorter(Nothing, a, New Single(n - 1){}, 0, n, 0,If((g = n / (p << 2)) <= MIN_ARRAY_SORT_GRAN, MIN_ARRAY_SORT_GRAN, g)), ArraysParallelSortHelpers.FJFloat.Sorter).invoke()
			End If
		End Sub

		''' <summary>
		''' Sorts the specified range of the array into ascending numerical order.
		''' The range to be sorted extends from the index {@code fromIndex},
		''' inclusive, to the index {@code toIndex}, exclusive. If
		''' {@code fromIndex == toIndex}, the range to be sorted is empty.
		''' 
		''' <p>The {@code <} relation does not provide a total order on all float
		''' values: {@code -0.0f == 0.0f} is {@code true} and a {@code Float.NaN}
		''' value compares neither less than, greater than, nor equal to any value,
		''' even itself. This method uses the total order imposed by the method
		''' <seealso cref="Float#compareTo"/>: {@code -0.0f} is treated as less than value
		''' {@code 0.0f} and {@code Float.NaN} is considered greater than any
		''' other value and all {@code Float.NaN} values are considered equal.
		''' 
		''' @implNote The sorting algorithm is a parallel sort-merge that breaks the
		''' array into sub-arrays that are themselves sorted and then merged. When
		''' the sub-array length reaches a minimum granularity, the sub-array is
		''' sorted using the appropriate <seealso cref="Arrays#sort(float[]) Arrays.sort"/>
		''' method. If the length of the specified array is less than the minimum
		''' granularity, then it is sorted using the appropriate {@link
		''' Arrays#sort(float[]) Arrays.sort} method. The algorithm requires a working
		''' space no greater than the size of the specified range of the original
		''' array. The <seealso cref="ForkJoinPool#commonPool() ForkJoin common pool"/> is
		''' used to execute any parallel tasks.
		''' </summary>
		''' <param name="a"> the array to be sorted </param>
		''' <param name="fromIndex"> the index of the first element, inclusive, to be sorted </param>
		''' <param name="toIndex"> the index of the last element, exclusive, to be sorted
		''' </param>
		''' <exception cref="IllegalArgumentException"> if {@code fromIndex > toIndex} </exception>
		''' <exception cref="ArrayIndexOutOfBoundsException">
		'''     if {@code fromIndex < 0} or {@code toIndex > a.length}
		''' 
		''' @since 1.8 </exception>
		Public Shared Sub parallelSort(  a As Single(),   fromIndex As Integer,   toIndex As Integer)
			rangeCheck(a.Length, fromIndex, toIndex)
			Dim n As Integer = toIndex - fromIndex, p As Integer, g As Integer
			p = java.util.concurrent.ForkJoinPool.commonPoolParallelism
			If n <= MIN_ARRAY_SORT_GRAN OrElse p = 1 Then
				DualPivotQuicksort.sort(a, fromIndex, toIndex - 1, Nothing, 0, 0)
			Else
'JAVA TO VB CONVERTER TODO TASK: Assignments within expressions are not supported in VB
				CType(New ArraysParallelSortHelpers.FJFloat.Sorter(Nothing, a, New Single(n - 1){}, fromIndex, n, 0,If((g = n / (p << 2)) <= MIN_ARRAY_SORT_GRAN, MIN_ARRAY_SORT_GRAN, g)), ArraysParallelSortHelpers.FJFloat.Sorter).invoke()
			End If
		End Sub

		''' <summary>
		''' Sorts the specified array into ascending numerical order.
		''' 
		''' <p>The {@code <} relation does not provide a total order on all double
		''' values: {@code -0.0d == 0.0d} is {@code true} and a {@code java.lang.[Double].NaN}
		''' value compares neither less than, greater than, nor equal to any value,
		''' even itself. This method uses the total order imposed by the method
		''' <seealso cref="Double#compareTo"/>: {@code -0.0d} is treated as less than value
		''' {@code 0.0d} and {@code java.lang.[Double].NaN} is considered greater than any
		''' other value and all {@code java.lang.[Double].NaN} values are considered equal.
		''' 
		''' @implNote The sorting algorithm is a parallel sort-merge that breaks the
		''' array into sub-arrays that are themselves sorted and then merged. When
		''' the sub-array length reaches a minimum granularity, the sub-array is
		''' sorted using the appropriate <seealso cref="Arrays#sort(double[]) Arrays.sort"/>
		''' method. If the length of the specified array is less than the minimum
		''' granularity, then it is sorted using the appropriate {@link
		''' Arrays#sort(double[]) Arrays.sort} method. The algorithm requires a
		''' working space no greater than the size of the original array. The
		''' <seealso cref="ForkJoinPool#commonPool() ForkJoin common pool"/> is used to
		''' execute any parallel tasks.
		''' </summary>
		''' <param name="a"> the array to be sorted
		''' 
		''' @since 1.8 </param>
		Public Shared Sub parallelSort(  a As Double())
			Dim n As Integer = a.Length, p As Integer, g As Integer
			p = java.util.concurrent.ForkJoinPool.commonPoolParallelism
			If n <= MIN_ARRAY_SORT_GRAN OrElse p = 1 Then
				DualPivotQuicksort.sort(a, 0, n - 1, Nothing, 0, 0)
			Else
'JAVA TO VB CONVERTER TODO TASK: Assignments within expressions are not supported in VB
				CType(New ArraysParallelSortHelpers.FJDouble.Sorter(Nothing, a, New Double(n - 1){}, 0, n, 0,If((g = n / (p << 2)) <= MIN_ARRAY_SORT_GRAN, MIN_ARRAY_SORT_GRAN, g)), ArraysParallelSortHelpers.FJDouble.Sorter).invoke()
			End If
		End Sub

		''' <summary>
		''' Sorts the specified range of the array into ascending numerical order.
		''' The range to be sorted extends from the index {@code fromIndex},
		''' inclusive, to the index {@code toIndex}, exclusive. If
		''' {@code fromIndex == toIndex}, the range to be sorted is empty.
		''' 
		''' <p>The {@code <} relation does not provide a total order on all double
		''' values: {@code -0.0d == 0.0d} is {@code true} and a {@code java.lang.[Double].NaN}
		''' value compares neither less than, greater than, nor equal to any value,
		''' even itself. This method uses the total order imposed by the method
		''' <seealso cref="Double#compareTo"/>: {@code -0.0d} is treated as less than value
		''' {@code 0.0d} and {@code java.lang.[Double].NaN} is considered greater than any
		''' other value and all {@code java.lang.[Double].NaN} values are considered equal.
		''' 
		''' @implNote The sorting algorithm is a parallel sort-merge that breaks the
		''' array into sub-arrays that are themselves sorted and then merged. When
		''' the sub-array length reaches a minimum granularity, the sub-array is
		''' sorted using the appropriate <seealso cref="Arrays#sort(double[]) Arrays.sort"/>
		''' method. If the length of the specified array is less than the minimum
		''' granularity, then it is sorted using the appropriate {@link
		''' Arrays#sort(double[]) Arrays.sort} method. The algorithm requires a working
		''' space no greater than the size of the specified range of the original
		''' array. The <seealso cref="ForkJoinPool#commonPool() ForkJoin common pool"/> is
		''' used to execute any parallel tasks.
		''' </summary>
		''' <param name="a"> the array to be sorted </param>
		''' <param name="fromIndex"> the index of the first element, inclusive, to be sorted </param>
		''' <param name="toIndex"> the index of the last element, exclusive, to be sorted
		''' </param>
		''' <exception cref="IllegalArgumentException"> if {@code fromIndex > toIndex} </exception>
		''' <exception cref="ArrayIndexOutOfBoundsException">
		'''     if {@code fromIndex < 0} or {@code toIndex > a.length}
		''' 
		''' @since 1.8 </exception>
		Public Shared Sub parallelSort(  a As Double(),   fromIndex As Integer,   toIndex As Integer)
			rangeCheck(a.Length, fromIndex, toIndex)
			Dim n As Integer = toIndex - fromIndex, p As Integer, g As Integer
			p = java.util.concurrent.ForkJoinPool.commonPoolParallelism
			If n <= MIN_ARRAY_SORT_GRAN OrElse p = 1 Then
				DualPivotQuicksort.sort(a, fromIndex, toIndex - 1, Nothing, 0, 0)
			Else
'JAVA TO VB CONVERTER TODO TASK: Assignments within expressions are not supported in VB
				CType(New ArraysParallelSortHelpers.FJDouble.Sorter(Nothing, a, New Double(n - 1){}, fromIndex, n, 0,If((g = n / (p << 2)) <= MIN_ARRAY_SORT_GRAN, MIN_ARRAY_SORT_GRAN, g)), ArraysParallelSortHelpers.FJDouble.Sorter).invoke()
			End If
		End Sub

		''' <summary>
		''' Sorts the specified array of objects into ascending order, according
		''' to the <seealso cref="Comparable natural ordering"/> of its elements.
		''' All elements in the array must implement the <seealso cref="Comparable"/>
		''' interface.  Furthermore, all elements in the array must be
		''' <i>mutually comparable</i> (that is, {@code e1.compareTo(e2)} must
		''' not throw a {@code ClassCastException} for any elements {@code e1}
		''' and {@code e2} in the array).
		''' 
		''' <p>This sort is guaranteed to be <i>stable</i>:  equal elements will
		''' not be reordered as a result of the sort.
		''' 
		''' @implNote The sorting algorithm is a parallel sort-merge that breaks the
		''' array into sub-arrays that are themselves sorted and then merged. When
		''' the sub-array length reaches a minimum granularity, the sub-array is
		''' sorted using the appropriate <seealso cref="Arrays#sort(Object[]) Arrays.sort"/>
		''' method. If the length of the specified array is less than the minimum
		''' granularity, then it is sorted using the appropriate {@link
		''' Arrays#sort(Object[]) Arrays.sort} method. The algorithm requires a
		''' working space no greater than the size of the original array. The
		''' <seealso cref="ForkJoinPool#commonPool() ForkJoin common pool"/> is used to
		''' execute any parallel tasks.
		''' </summary>
		''' @param <T> the class of the objects to be sorted </param>
		''' <param name="a"> the array to be sorted
		''' </param>
		''' <exception cref="ClassCastException"> if the array contains elements that are not
		'''         <i>mutually comparable</i> (for example, strings and integers) </exception>
		''' <exception cref="IllegalArgumentException"> (optional) if the natural
		'''         ordering of the array elements is found to violate the
		'''         <seealso cref="Comparable"/> contract
		''' 
		''' @since 1.8 </exception>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'JAVA TO VB CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
		Public Shared Sub parallelSort(Of T As Comparable(Of ?))(  a As T())
			Dim n As Integer = a.Length, p As Integer, g As Integer
			p = java.util.concurrent.ForkJoinPool.commonPoolParallelism
			If n <= MIN_ARRAY_SORT_GRAN OrElse p = 1 Then
				TimSort.sort(a, 0, n, NaturalOrder.INSTANCE, Nothing, 0, 0)
			Else
'JAVA TO VB CONVERTER TODO TASK: Assignments within expressions are not supported in VB
				CType(New ArraysParallelSortHelpers.FJObject.Sorter(Of T) (Nothing, a, CType(Array.newInstance(a.GetType().GetElementType(), n), T()), 0, n, 0,If((g = n / (p << 2)) <= MIN_ARRAY_SORT_GRAN, MIN_ARRAY_SORT_GRAN, g), NaturalOrder.INSTANCE), ArraysParallelSortHelpers.FJObject.Sorter(Of T)).invoke()
			End If
		End Sub

		''' <summary>
		''' Sorts the specified range of the specified array of objects into
		''' ascending order, according to the
		''' <seealso cref="Comparable natural ordering"/> of its
		''' elements.  The range to be sorted extends from index
		''' {@code fromIndex}, inclusive, to index {@code toIndex}, exclusive.
		''' (If {@code fromIndex==toIndex}, the range to be sorted is empty.)  All
		''' elements in this range must implement the <seealso cref="Comparable"/>
		''' interface.  Furthermore, all elements in this range must be <i>mutually
		''' comparable</i> (that is, {@code e1.compareTo(e2)} must not throw a
		''' {@code ClassCastException} for any elements {@code e1} and
		''' {@code e2} in the array).
		''' 
		''' <p>This sort is guaranteed to be <i>stable</i>:  equal elements will
		''' not be reordered as a result of the sort.
		''' 
		''' @implNote The sorting algorithm is a parallel sort-merge that breaks the
		''' array into sub-arrays that are themselves sorted and then merged. When
		''' the sub-array length reaches a minimum granularity, the sub-array is
		''' sorted using the appropriate <seealso cref="Arrays#sort(Object[]) Arrays.sort"/>
		''' method. If the length of the specified array is less than the minimum
		''' granularity, then it is sorted using the appropriate {@link
		''' Arrays#sort(Object[]) Arrays.sort} method. The algorithm requires a working
		''' space no greater than the size of the specified range of the original
		''' array. The <seealso cref="ForkJoinPool#commonPool() ForkJoin common pool"/> is
		''' used to execute any parallel tasks.
		''' </summary>
		''' @param <T> the class of the objects to be sorted </param>
		''' <param name="a"> the array to be sorted </param>
		''' <param name="fromIndex"> the index of the first element (inclusive) to be
		'''        sorted </param>
		''' <param name="toIndex"> the index of the last element (exclusive) to be sorted </param>
		''' <exception cref="IllegalArgumentException"> if {@code fromIndex > toIndex} or
		'''         (optional) if the natural ordering of the array elements is
		'''         found to violate the <seealso cref="Comparable"/> contract </exception>
		''' <exception cref="ArrayIndexOutOfBoundsException"> if {@code fromIndex < 0} or
		'''         {@code toIndex > a.length} </exception>
		''' <exception cref="ClassCastException"> if the array contains elements that are
		'''         not <i>mutually comparable</i> (for example, strings and
		'''         integers).
		''' 
		''' @since 1.8 </exception>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'JAVA TO VB CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
		Public Shared Sub parallelSort(Of T As Comparable(Of ?))(  a As T(),   fromIndex As Integer,   toIndex As Integer)
			rangeCheck(a.Length, fromIndex, toIndex)
			Dim n As Integer = toIndex - fromIndex, p As Integer, g As Integer
			p = java.util.concurrent.ForkJoinPool.commonPoolParallelism
			If n <= MIN_ARRAY_SORT_GRAN OrElse p = 1 Then
				TimSort.sort(a, fromIndex, toIndex, NaturalOrder.INSTANCE, Nothing, 0, 0)
			Else
'JAVA TO VB CONVERTER TODO TASK: Assignments within expressions are not supported in VB
				CType(New ArraysParallelSortHelpers.FJObject.Sorter(Of T) (Nothing, a, CType(Array.newInstance(a.GetType().GetElementType(), n), T()), fromIndex, n, 0,If((g = n / (p << 2)) <= MIN_ARRAY_SORT_GRAN, MIN_ARRAY_SORT_GRAN, g), NaturalOrder.INSTANCE), ArraysParallelSortHelpers.FJObject.Sorter(Of T)).invoke()
			End If
		End Sub

		''' <summary>
		''' Sorts the specified array of objects according to the order induced by
		''' the specified comparator.  All elements in the array must be
		''' <i>mutually comparable</i> by the specified comparator (that is,
		''' {@code c.compare(e1, e2)} must not throw a {@code ClassCastException}
		''' for any elements {@code e1} and {@code e2} in the array).
		''' 
		''' <p>This sort is guaranteed to be <i>stable</i>:  equal elements will
		''' not be reordered as a result of the sort.
		''' 
		''' @implNote The sorting algorithm is a parallel sort-merge that breaks the
		''' array into sub-arrays that are themselves sorted and then merged. When
		''' the sub-array length reaches a minimum granularity, the sub-array is
		''' sorted using the appropriate <seealso cref="Arrays#sort(Object[]) Arrays.sort"/>
		''' method. If the length of the specified array is less than the minimum
		''' granularity, then it is sorted using the appropriate {@link
		''' Arrays#sort(Object[]) Arrays.sort} method. The algorithm requires a
		''' working space no greater than the size of the original array. The
		''' <seealso cref="ForkJoinPool#commonPool() ForkJoin common pool"/> is used to
		''' execute any parallel tasks.
		''' </summary>
		''' @param <T> the class of the objects to be sorted </param>
		''' <param name="a"> the array to be sorted </param>
		''' <param name="cmp"> the comparator to determine the order of the array.  A
		'''        {@code null} value indicates that the elements'
		'''        <seealso cref="Comparable natural ordering"/> should be used. </param>
		''' <exception cref="ClassCastException"> if the array contains elements that are
		'''         not <i>mutually comparable</i> using the specified comparator </exception>
		''' <exception cref="IllegalArgumentException"> (optional) if the comparator is
		'''         found to violate the <seealso cref="java.util.Comparator"/> contract
		''' 
		''' @since 1.8 </exception>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'JAVA TO VB CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
		Public Shared Sub parallelSort(Of T, T1)(  a As T(),   cmp As Comparator(Of T1))
			If cmp Is Nothing Then cmp = NaturalOrder.INSTANCE
			Dim n As Integer = a.Length, p As Integer, g As Integer
			p = java.util.concurrent.ForkJoinPool.commonPoolParallelism
			If n <= MIN_ARRAY_SORT_GRAN OrElse p = 1 Then
				TimSort.sort(a, 0, n, cmp, Nothing, 0, 0)
			Else
'JAVA TO VB CONVERTER TODO TASK: Assignments within expressions are not supported in VB
				CType(New ArraysParallelSortHelpers.FJObject.Sorter(Of T) (Nothing, a, CType(Array.newInstance(a.GetType().GetElementType(), n), T()), 0, n, 0,If((g = n / (p << 2)) <= MIN_ARRAY_SORT_GRAN, MIN_ARRAY_SORT_GRAN, g), cmp), ArraysParallelSortHelpers.FJObject.Sorter(Of T)).invoke()
			End If
		End Sub

		''' <summary>
		''' Sorts the specified range of the specified array of objects according
		''' to the order induced by the specified comparator.  The range to be
		''' sorted extends from index {@code fromIndex}, inclusive, to index
		''' {@code toIndex}, exclusive.  (If {@code fromIndex==toIndex}, the
		''' range to be sorted is empty.)  All elements in the range must be
		''' <i>mutually comparable</i> by the specified comparator (that is,
		''' {@code c.compare(e1, e2)} must not throw a {@code ClassCastException}
		''' for any elements {@code e1} and {@code e2} in the range).
		''' 
		''' <p>This sort is guaranteed to be <i>stable</i>:  equal elements will
		''' not be reordered as a result of the sort.
		''' 
		''' @implNote The sorting algorithm is a parallel sort-merge that breaks the
		''' array into sub-arrays that are themselves sorted and then merged. When
		''' the sub-array length reaches a minimum granularity, the sub-array is
		''' sorted using the appropriate <seealso cref="Arrays#sort(Object[]) Arrays.sort"/>
		''' method. If the length of the specified array is less than the minimum
		''' granularity, then it is sorted using the appropriate {@link
		''' Arrays#sort(Object[]) Arrays.sort} method. The algorithm requires a working
		''' space no greater than the size of the specified range of the original
		''' array. The <seealso cref="ForkJoinPool#commonPool() ForkJoin common pool"/> is
		''' used to execute any parallel tasks.
		''' </summary>
		''' @param <T> the class of the objects to be sorted </param>
		''' <param name="a"> the array to be sorted </param>
		''' <param name="fromIndex"> the index of the first element (inclusive) to be
		'''        sorted </param>
		''' <param name="toIndex"> the index of the last element (exclusive) to be sorted </param>
		''' <param name="cmp"> the comparator to determine the order of the array.  A
		'''        {@code null} value indicates that the elements'
		'''        <seealso cref="Comparable natural ordering"/> should be used. </param>
		''' <exception cref="IllegalArgumentException"> if {@code fromIndex > toIndex} or
		'''         (optional) if the natural ordering of the array elements is
		'''         found to violate the <seealso cref="Comparable"/> contract </exception>
		''' <exception cref="ArrayIndexOutOfBoundsException"> if {@code fromIndex < 0} or
		'''         {@code toIndex > a.length} </exception>
		''' <exception cref="ClassCastException"> if the array contains elements that are
		'''         not <i>mutually comparable</i> (for example, strings and
		'''         integers).
		''' 
		''' @since 1.8 </exception>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'JAVA TO VB CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
		Public Shared Sub parallelSort(Of T, T1)(  a As T(),   fromIndex As Integer,   toIndex As Integer,   cmp As Comparator(Of T1))
			rangeCheck(a.Length, fromIndex, toIndex)
			If cmp Is Nothing Then cmp = NaturalOrder.INSTANCE
			Dim n As Integer = toIndex - fromIndex, p As Integer, g As Integer
			p = java.util.concurrent.ForkJoinPool.commonPoolParallelism
			If n <= MIN_ARRAY_SORT_GRAN OrElse p = 1 Then
				TimSort.sort(a, fromIndex, toIndex, cmp, Nothing, 0, 0)
			Else
'JAVA TO VB CONVERTER TODO TASK: Assignments within expressions are not supported in VB
				CType(New ArraysParallelSortHelpers.FJObject.Sorter(Of T) (Nothing, a, CType(Array.newInstance(a.GetType().GetElementType(), n), T()), fromIndex, n, 0,If((g = n / (p << 2)) <= MIN_ARRAY_SORT_GRAN, MIN_ARRAY_SORT_GRAN, g), cmp), ArraysParallelSortHelpers.FJObject.Sorter(Of T)).invoke()
			End If
		End Sub

	'    
	'     * Sorting of complex type arrays.
	'     

		''' <summary>
		''' Old merge sort implementation can be selected (for
		''' compatibility with broken comparators) using a system property.
		''' Cannot be a static boolean in the enclosing class due to
		''' circular dependencies. To be removed in a future release.
		''' </summary>
		Friend NotInheritable Class LegacyMergeSort
			Private Shared ReadOnly userRequested As Boolean = java.security.AccessController.doPrivileged(New sun.security.action.GetBooleanAction("java.util.Arrays.useLegacyMergeSort"))
		End Class

		''' <summary>
		''' Sorts the specified array of objects into ascending order, according
		''' to the <seealso cref="Comparable natural ordering"/> of its elements.
		''' All elements in the array must implement the <seealso cref="Comparable"/>
		''' interface.  Furthermore, all elements in the array must be
		''' <i>mutually comparable</i> (that is, {@code e1.compareTo(e2)} must
		''' not throw a {@code ClassCastException} for any elements {@code e1}
		''' and {@code e2} in the array).
		''' 
		''' <p>This sort is guaranteed to be <i>stable</i>:  equal elements will
		''' not be reordered as a result of the sort.
		''' 
		''' <p>Implementation note: This implementation is a stable, adaptive,
		''' iterative mergesort that requires far fewer than n lg(n) comparisons
		''' when the input array is partially sorted, while offering the
		''' performance of a traditional mergesort when the input array is
		''' randomly ordered.  If the input array is nearly sorted, the
		''' implementation requires approximately n comparisons.  Temporary
		''' storage requirements vary from a small constant for nearly sorted
		''' input arrays to n/2 object references for randomly ordered input
		''' arrays.
		''' 
		''' <p>The implementation takes equal advantage of ascending and
		''' descending order in its input array, and can take advantage of
		''' ascending and descending order in different parts of the the same
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
		''' <param name="a"> the array to be sorted </param>
		''' <exception cref="ClassCastException"> if the array contains elements that are not
		'''         <i>mutually comparable</i> (for example, strings and integers) </exception>
		''' <exception cref="IllegalArgumentException"> (optional) if the natural
		'''         ordering of the array elements is found to violate the
		'''         <seealso cref="Comparable"/> contract </exception>
		Public Shared Sub sort(  a As Object())
			If LegacyMergeSort.userRequested Then
				legacyMergeSort(a)
			Else
				ComparableTimSort.sort(a, 0, a.Length, Nothing, 0, 0)
			End If
		End Sub

		''' <summary>
		''' To be removed in a future release. </summary>
		Private Shared Sub legacyMergeSort(  a As Object())
			Dim aux As Object() = a.clone()
			mergeSort(aux, a, 0, a.Length, 0)
		End Sub

		''' <summary>
		''' Sorts the specified range of the specified array of objects into
		''' ascending order, according to the
		''' <seealso cref="Comparable natural ordering"/> of its
		''' elements.  The range to be sorted extends from index
		''' {@code fromIndex}, inclusive, to index {@code toIndex}, exclusive.
		''' (If {@code fromIndex==toIndex}, the range to be sorted is empty.)  All
		''' elements in this range must implement the <seealso cref="Comparable"/>
		''' interface.  Furthermore, all elements in this range must be <i>mutually
		''' comparable</i> (that is, {@code e1.compareTo(e2)} must not throw a
		''' {@code ClassCastException} for any elements {@code e1} and
		''' {@code e2} in the array).
		''' 
		''' <p>This sort is guaranteed to be <i>stable</i>:  equal elements will
		''' not be reordered as a result of the sort.
		''' 
		''' <p>Implementation note: This implementation is a stable, adaptive,
		''' iterative mergesort that requires far fewer than n lg(n) comparisons
		''' when the input array is partially sorted, while offering the
		''' performance of a traditional mergesort when the input array is
		''' randomly ordered.  If the input array is nearly sorted, the
		''' implementation requires approximately n comparisons.  Temporary
		''' storage requirements vary from a small constant for nearly sorted
		''' input arrays to n/2 object references for randomly ordered input
		''' arrays.
		''' 
		''' <p>The implementation takes equal advantage of ascending and
		''' descending order in its input array, and can take advantage of
		''' ascending and descending order in different parts of the the same
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
		''' <param name="a"> the array to be sorted </param>
		''' <param name="fromIndex"> the index of the first element (inclusive) to be
		'''        sorted </param>
		''' <param name="toIndex"> the index of the last element (exclusive) to be sorted </param>
		''' <exception cref="IllegalArgumentException"> if {@code fromIndex > toIndex} or
		'''         (optional) if the natural ordering of the array elements is
		'''         found to violate the <seealso cref="Comparable"/> contract </exception>
		''' <exception cref="ArrayIndexOutOfBoundsException"> if {@code fromIndex < 0} or
		'''         {@code toIndex > a.length} </exception>
		''' <exception cref="ClassCastException"> if the array contains elements that are
		'''         not <i>mutually comparable</i> (for example, strings and
		'''         integers). </exception>
		Public Shared Sub sort(  a As Object(),   fromIndex As Integer,   toIndex As Integer)
			rangeCheck(a.Length, fromIndex, toIndex)
			If LegacyMergeSort.userRequested Then
				legacyMergeSort(a, fromIndex, toIndex)
			Else
				ComparableTimSort.sort(a, fromIndex, toIndex, Nothing, 0, 0)
			End If
		End Sub

		''' <summary>
		''' To be removed in a future release. </summary>
		Private Shared Sub legacyMergeSort(  a As Object(),   fromIndex As Integer,   toIndex As Integer)
			Dim aux As Object() = copyOfRange(a, fromIndex, toIndex)
			mergeSort(aux, a, fromIndex, toIndex, -fromIndex)
		End Sub

		''' <summary>
		''' Tuning parameter: list size at or below which insertion sort will be
		''' used in preference to mergesort.
		''' To be removed in a future release.
		''' </summary>
		Private Const INSERTIONSORT_THRESHOLD As Integer = 7

		''' <summary>
		''' Src is the source array that starts at index 0
		''' Dest is the (possibly larger) array destination with a possible offset
		''' low is the index in dest to start sorting
		''' high is the end index in dest to end sorting
		''' off is the offset to generate corresponding low, high in src
		''' To be removed in a future release.
		''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
		Private Shared Sub mergeSort(  src As Object(),   dest As Object(),   low As Integer,   high As Integer,   [off] As Integer)
			Dim length As Integer = high - low

			' Insertion sort on smallest arrays
			If length < INSERTIONSORT_THRESHOLD Then
				For i As Integer = low To high - 1
					Dim j As Integer=i
					Do While j>low AndAlso CType(dest(j-1), Comparable).CompareTo(dest(j))>0
						swap(dest, j, j-1)
						j -= 1
					Loop
				Next i
				Return
			End If

			' Recursively sort halves of dest into src
			Dim destLow As Integer = low
			Dim destHigh As Integer = high
			low += [off]
			high += [off]
			Dim mid As Integer = CInt(CUInt((low + high)) >> 1)
			mergeSort(dest, src, low, mid, -[off])
			mergeSort(dest, src, mid, high, -[off])

			' If list is already sorted, just copy from src to dest.  This is an
			' optimization that results in faster sorts for nearly ordered lists.
			If CType(src(mid-1), Comparable).CompareTo(src(mid)) <= 0 Then
				Array.Copy(src, low, dest, destLow, length)
				Return
			End If

			' Merge sorted halves (now in src) into dest
			Dim i As Integer = destLow
			Dim p As Integer = low
			Dim q As Integer = mid
			Do While i < destHigh
				If q >= high OrElse p < mid AndAlso CType(src(p), Comparable).CompareTo(src(q))<=0 Then
					dest(i) = src(p)
					p += 1
				Else
					dest(i) = src(q)
					q += 1
				End If
				i += 1
			Loop
		End Sub

		''' <summary>
		''' Swaps x[a] with x[b].
		''' </summary>
		Private Shared Sub swap(  x As Object(),   a As Integer,   b As Integer)
			Dim t As Object = x(a)
			x(a) = x(b)
			x(b) = t
		End Sub

		''' <summary>
		''' Sorts the specified array of objects according to the order induced by
		''' the specified comparator.  All elements in the array must be
		''' <i>mutually comparable</i> by the specified comparator (that is,
		''' {@code c.compare(e1, e2)} must not throw a {@code ClassCastException}
		''' for any elements {@code e1} and {@code e2} in the array).
		''' 
		''' <p>This sort is guaranteed to be <i>stable</i>:  equal elements will
		''' not be reordered as a result of the sort.
		''' 
		''' <p>Implementation note: This implementation is a stable, adaptive,
		''' iterative mergesort that requires far fewer than n lg(n) comparisons
		''' when the input array is partially sorted, while offering the
		''' performance of a traditional mergesort when the input array is
		''' randomly ordered.  If the input array is nearly sorted, the
		''' implementation requires approximately n comparisons.  Temporary
		''' storage requirements vary from a small constant for nearly sorted
		''' input arrays to n/2 object references for randomly ordered input
		''' arrays.
		''' 
		''' <p>The implementation takes equal advantage of ascending and
		''' descending order in its input array, and can take advantage of
		''' ascending and descending order in different parts of the the same
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
		''' @param <T> the class of the objects to be sorted </param>
		''' <param name="a"> the array to be sorted </param>
		''' <param name="c"> the comparator to determine the order of the array.  A
		'''        {@code null} value indicates that the elements'
		'''        <seealso cref="Comparable natural ordering"/> should be used. </param>
		''' <exception cref="ClassCastException"> if the array contains elements that are
		'''         not <i>mutually comparable</i> using the specified comparator </exception>
		''' <exception cref="IllegalArgumentException"> (optional) if the comparator is
		'''         found to violate the <seealso cref="Comparator"/> contract </exception>
'JAVA TO VB CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
		Public Shared Sub sort(Of T, T1)(  a As T(),   c As Comparator(Of T1))
			If c Is Nothing Then
				sort(a)
			Else
				If LegacyMergeSort.userRequested Then
					legacyMergeSort(a, c)
				Else
					TimSort.sort(a, 0, a.Length, c, Nothing, 0, 0)
				End If
			End If
		End Sub

		''' <summary>
		''' To be removed in a future release. </summary>
'JAVA TO VB CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
		Private Shared Sub legacyMergeSort(Of T, T1)(  a As T(),   c As Comparator(Of T1))
			Dim aux As T() = a.clone()
			If c Is Nothing Then
				mergeSort(aux, a, 0, a.Length, 0)
			Else
				mergeSort(aux, a, 0, a.Length, 0, c)
			End If
		End Sub

		''' <summary>
		''' Sorts the specified range of the specified array of objects according
		''' to the order induced by the specified comparator.  The range to be
		''' sorted extends from index {@code fromIndex}, inclusive, to index
		''' {@code toIndex}, exclusive.  (If {@code fromIndex==toIndex}, the
		''' range to be sorted is empty.)  All elements in the range must be
		''' <i>mutually comparable</i> by the specified comparator (that is,
		''' {@code c.compare(e1, e2)} must not throw a {@code ClassCastException}
		''' for any elements {@code e1} and {@code e2} in the range).
		''' 
		''' <p>This sort is guaranteed to be <i>stable</i>:  equal elements will
		''' not be reordered as a result of the sort.
		''' 
		''' <p>Implementation note: This implementation is a stable, adaptive,
		''' iterative mergesort that requires far fewer than n lg(n) comparisons
		''' when the input array is partially sorted, while offering the
		''' performance of a traditional mergesort when the input array is
		''' randomly ordered.  If the input array is nearly sorted, the
		''' implementation requires approximately n comparisons.  Temporary
		''' storage requirements vary from a small constant for nearly sorted
		''' input arrays to n/2 object references for randomly ordered input
		''' arrays.
		''' 
		''' <p>The implementation takes equal advantage of ascending and
		''' descending order in its input array, and can take advantage of
		''' ascending and descending order in different parts of the the same
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
		''' @param <T> the class of the objects to be sorted </param>
		''' <param name="a"> the array to be sorted </param>
		''' <param name="fromIndex"> the index of the first element (inclusive) to be
		'''        sorted </param>
		''' <param name="toIndex"> the index of the last element (exclusive) to be sorted </param>
		''' <param name="c"> the comparator to determine the order of the array.  A
		'''        {@code null} value indicates that the elements'
		'''        <seealso cref="Comparable natural ordering"/> should be used. </param>
		''' <exception cref="ClassCastException"> if the array contains elements that are not
		'''         <i>mutually comparable</i> using the specified comparator. </exception>
		''' <exception cref="IllegalArgumentException"> if {@code fromIndex > toIndex} or
		'''         (optional) if the comparator is found to violate the
		'''         <seealso cref="Comparator"/> contract </exception>
		''' <exception cref="ArrayIndexOutOfBoundsException"> if {@code fromIndex < 0} or
		'''         {@code toIndex > a.length} </exception>
'JAVA TO VB CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
		Public Shared Sub sort(Of T, T1)(  a As T(),   fromIndex As Integer,   toIndex As Integer,   c As Comparator(Of T1))
			If c Is Nothing Then
				sort(a, fromIndex, toIndex)
			Else
				rangeCheck(a.Length, fromIndex, toIndex)
				If LegacyMergeSort.userRequested Then
					legacyMergeSort(a, fromIndex, toIndex, c)
				Else
					TimSort.sort(a, fromIndex, toIndex, c, Nothing, 0, 0)
				End If
			End If
		End Sub

		''' <summary>
		''' To be removed in a future release. </summary>
'JAVA TO VB CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
		Private Shared Sub legacyMergeSort(Of T, T1)(  a As T(),   fromIndex As Integer,   toIndex As Integer,   c As Comparator(Of T1))
			Dim aux As T() = copyOfRange(a, fromIndex, toIndex)
			If c Is Nothing Then
				mergeSort(aux, a, fromIndex, toIndex, -fromIndex)
			Else
				mergeSort(aux, a, fromIndex, toIndex, -fromIndex, c)
			End If
		End Sub

		''' <summary>
		''' Src is the source array that starts at index 0
		''' Dest is the (possibly larger) array destination with a possible offset
		''' low is the index in dest to start sorting
		''' high is the end index in dest to end sorting
		''' off is the offset into src corresponding to low in dest
		''' To be removed in a future release.
		''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
		Private Shared Sub mergeSort(  src As Object(),   dest As Object(),   low As Integer,   high As Integer,   [off] As Integer,   c As Comparator)
			Dim length As Integer = high - low

			' Insertion sort on smallest arrays
			If length < INSERTIONSORT_THRESHOLD Then
				For i As Integer = low To high - 1
					Dim j As Integer=i
					Do While j>low AndAlso c.Compare(dest(j-1), dest(j))>0
						swap(dest, j, j-1)
						j -= 1
					Loop
				Next i
				Return
			End If

			' Recursively sort halves of dest into src
			Dim destLow As Integer = low
			Dim destHigh As Integer = high
			low += [off]
			high += [off]
			Dim mid As Integer = CInt(CUInt((low + high)) >> 1)
			mergeSort(dest, src, low, mid, -[off], c)
			mergeSort(dest, src, mid, high, -[off], c)

			' If list is already sorted, just copy from src to dest.  This is an
			' optimization that results in faster sorts for nearly ordered lists.
			If c.Compare(src(mid-1), src(mid)) <= 0 Then
			   Array.Copy(src, low, dest, destLow, length)
			   Return
			End If

			' Merge sorted halves (now in src) into dest
			Dim i As Integer = destLow
			Dim p As Integer = low
			Dim q As Integer = mid
			Do While i < destHigh
				If q >= high OrElse p < mid AndAlso c.Compare(src(p), src(q)) <= 0 Then
					dest(i) = src(p)
					p += 1
				Else
					dest(i) = src(q)
					q += 1
				End If
				i += 1
			Loop
		End Sub

		' Parallel prefix

		''' <summary>
		''' Cumulates, in parallel, each element of the given array in place,
		''' using the supplied function. For example if the array initially
		''' holds {@code [2, 1, 0, 3]} and the operation performs addition,
		''' then upon return the array holds {@code [2, 3, 3, 6]}.
		''' Parallel prefix computation is usually more efficient than
		''' sequential loops for large arrays.
		''' </summary>
		''' @param <T> the class of the objects in the array </param>
		''' <param name="array"> the array, which is modified in-place by this method </param>
		''' <param name="op"> a side-effect-free, associative function to perform the
		''' cumulation </param>
		''' <exception cref="NullPointerException"> if the specified array or function is null
		''' @since 1.8 </exception>
		Public Shared Sub parallelPrefix(Of T)(  array As T(),   op As java.util.function.BinaryOperator(Of T))
			Objects.requireNonNull(op)
			If array.Length > 0 Then CType(New ArrayPrefixHelpers.CumulateTask(Of ) (Nothing, op, array, 0, array.Length), ArrayPrefixHelpers.CumulateTask(Of )).invoke()
		End Sub

		''' <summary>
		''' Performs <seealso cref="#parallelPrefix(Object[], BinaryOperator)"/>
		''' for the given subrange of the array.
		''' </summary>
		''' @param <T> the class of the objects in the array </param>
		''' <param name="array"> the array </param>
		''' <param name="fromIndex"> the index of the first element, inclusive </param>
		''' <param name="toIndex"> the index of the last element, exclusive </param>
		''' <param name="op"> a side-effect-free, associative function to perform the
		''' cumulation </param>
		''' <exception cref="IllegalArgumentException"> if {@code fromIndex > toIndex} </exception>
		''' <exception cref="ArrayIndexOutOfBoundsException">
		'''     if {@code fromIndex < 0} or {@code toIndex > array.length} </exception>
		''' <exception cref="NullPointerException"> if the specified array or function is null
		''' @since 1.8 </exception>
		Public Shared Sub parallelPrefix(Of T)(  array As T(),   fromIndex As Integer,   toIndex As Integer,   op As java.util.function.BinaryOperator(Of T))
			Objects.requireNonNull(op)
			rangeCheck(array.Length, fromIndex, toIndex)
			If fromIndex < toIndex Then CType(New ArrayPrefixHelpers.CumulateTask(Of ) (Nothing, op, array, fromIndex, toIndex), ArrayPrefixHelpers.CumulateTask(Of )).invoke()
		End Sub

		''' <summary>
		''' Cumulates, in parallel, each element of the given array in place,
		''' using the supplied function. For example if the array initially
		''' holds {@code [2, 1, 0, 3]} and the operation performs addition,
		''' then upon return the array holds {@code [2, 3, 3, 6]}.
		''' Parallel prefix computation is usually more efficient than
		''' sequential loops for large arrays.
		''' </summary>
		''' <param name="array"> the array, which is modified in-place by this method </param>
		''' <param name="op"> a side-effect-free, associative function to perform the
		''' cumulation </param>
		''' <exception cref="NullPointerException"> if the specified array or function is null
		''' @since 1.8 </exception>
		Public Shared Sub parallelPrefix(  array As Long(),   op As java.util.function.LongBinaryOperator)
			Objects.requireNonNull(op)
			If array.Length > 0 Then CType(New ArrayPrefixHelpers.LongCumulateTask(Nothing, op, array, 0, array.Length), ArrayPrefixHelpers.LongCumulateTask).invoke()
		End Sub

		''' <summary>
		''' Performs <seealso cref="#parallelPrefix(long[], LongBinaryOperator)"/>
		''' for the given subrange of the array.
		''' </summary>
		''' <param name="array"> the array </param>
		''' <param name="fromIndex"> the index of the first element, inclusive </param>
		''' <param name="toIndex"> the index of the last element, exclusive </param>
		''' <param name="op"> a side-effect-free, associative function to perform the
		''' cumulation </param>
		''' <exception cref="IllegalArgumentException"> if {@code fromIndex > toIndex} </exception>
		''' <exception cref="ArrayIndexOutOfBoundsException">
		'''     if {@code fromIndex < 0} or {@code toIndex > array.length} </exception>
		''' <exception cref="NullPointerException"> if the specified array or function is null
		''' @since 1.8 </exception>
		Public Shared Sub parallelPrefix(  array As Long(),   fromIndex As Integer,   toIndex As Integer,   op As java.util.function.LongBinaryOperator)
			Objects.requireNonNull(op)
			rangeCheck(array.Length, fromIndex, toIndex)
			If fromIndex < toIndex Then CType(New ArrayPrefixHelpers.LongCumulateTask(Nothing, op, array, fromIndex, toIndex), ArrayPrefixHelpers.LongCumulateTask).invoke()
		End Sub

		''' <summary>
		''' Cumulates, in parallel, each element of the given array in place,
		''' using the supplied function. For example if the array initially
		''' holds {@code [2.0, 1.0, 0.0, 3.0]} and the operation performs addition,
		''' then upon return the array holds {@code [2.0, 3.0, 3.0, 6.0]}.
		''' Parallel prefix computation is usually more efficient than
		''' sequential loops for large arrays.
		''' 
		''' <p> Because floating-point operations may not be strictly associative,
		''' the returned result may not be identical to the value that would be
		''' obtained if the operation was performed sequentially.
		''' </summary>
		''' <param name="array"> the array, which is modified in-place by this method </param>
		''' <param name="op"> a side-effect-free function to perform the cumulation </param>
		''' <exception cref="NullPointerException"> if the specified array or function is null
		''' @since 1.8 </exception>
		Public Shared Sub parallelPrefix(  array As Double(),   op As java.util.function.DoubleBinaryOperator)
			Objects.requireNonNull(op)
			If array.Length > 0 Then CType(New ArrayPrefixHelpers.DoubleCumulateTask(Nothing, op, array, 0, array.Length), ArrayPrefixHelpers.DoubleCumulateTask).invoke()
		End Sub

		''' <summary>
		''' Performs <seealso cref="#parallelPrefix(double[], DoubleBinaryOperator)"/>
		''' for the given subrange of the array.
		''' </summary>
		''' <param name="array"> the array </param>
		''' <param name="fromIndex"> the index of the first element, inclusive </param>
		''' <param name="toIndex"> the index of the last element, exclusive </param>
		''' <param name="op"> a side-effect-free, associative function to perform the
		''' cumulation </param>
		''' <exception cref="IllegalArgumentException"> if {@code fromIndex > toIndex} </exception>
		''' <exception cref="ArrayIndexOutOfBoundsException">
		'''     if {@code fromIndex < 0} or {@code toIndex > array.length} </exception>
		''' <exception cref="NullPointerException"> if the specified array or function is null
		''' @since 1.8 </exception>
		Public Shared Sub parallelPrefix(  array As Double(),   fromIndex As Integer,   toIndex As Integer,   op As java.util.function.DoubleBinaryOperator)
			Objects.requireNonNull(op)
			rangeCheck(array.Length, fromIndex, toIndex)
			If fromIndex < toIndex Then CType(New ArrayPrefixHelpers.DoubleCumulateTask(Nothing, op, array, fromIndex, toIndex), ArrayPrefixHelpers.DoubleCumulateTask).invoke()
		End Sub

		''' <summary>
		''' Cumulates, in parallel, each element of the given array in place,
		''' using the supplied function. For example if the array initially
		''' holds {@code [2, 1, 0, 3]} and the operation performs addition,
		''' then upon return the array holds {@code [2, 3, 3, 6]}.
		''' Parallel prefix computation is usually more efficient than
		''' sequential loops for large arrays.
		''' </summary>
		''' <param name="array"> the array, which is modified in-place by this method </param>
		''' <param name="op"> a side-effect-free, associative function to perform the
		''' cumulation </param>
		''' <exception cref="NullPointerException"> if the specified array or function is null
		''' @since 1.8 </exception>
		Public Shared Sub parallelPrefix(  array As Integer(),   op As java.util.function.IntBinaryOperator)
			Objects.requireNonNull(op)
			If array.Length > 0 Then CType(New ArrayPrefixHelpers.IntCumulateTask(Nothing, op, array, 0, array.Length), ArrayPrefixHelpers.IntCumulateTask).invoke()
		End Sub

		''' <summary>
		''' Performs <seealso cref="#parallelPrefix(int[], IntBinaryOperator)"/>
		''' for the given subrange of the array.
		''' </summary>
		''' <param name="array"> the array </param>
		''' <param name="fromIndex"> the index of the first element, inclusive </param>
		''' <param name="toIndex"> the index of the last element, exclusive </param>
		''' <param name="op"> a side-effect-free, associative function to perform the
		''' cumulation </param>
		''' <exception cref="IllegalArgumentException"> if {@code fromIndex > toIndex} </exception>
		''' <exception cref="ArrayIndexOutOfBoundsException">
		'''     if {@code fromIndex < 0} or {@code toIndex > array.length} </exception>
		''' <exception cref="NullPointerException"> if the specified array or function is null
		''' @since 1.8 </exception>
		Public Shared Sub parallelPrefix(  array As Integer(),   fromIndex As Integer,   toIndex As Integer,   op As java.util.function.IntBinaryOperator)
			Objects.requireNonNull(op)
			rangeCheck(array.Length, fromIndex, toIndex)
			If fromIndex < toIndex Then CType(New ArrayPrefixHelpers.IntCumulateTask(Nothing, op, array, fromIndex, toIndex), ArrayPrefixHelpers.IntCumulateTask).invoke()
		End Sub

		' Searching

		''' <summary>
		''' Searches the specified array of longs for the specified value using the
		''' binary search algorithm.  The array must be sorted (as
		''' by the <seealso cref="#sort(long[])"/> method) prior to making this call.  If it
		''' is not sorted, the results are undefined.  If the array contains
		''' multiple elements with the specified value, there is no guarantee which
		''' one will be found.
		''' </summary>
		''' <param name="a"> the array to be searched </param>
		''' <param name="key"> the value to be searched for </param>
		''' <returns> index of the search key, if it is contained in the array;
		'''         otherwise, <tt>(-(<i>insertion point</i>) - 1)</tt>.  The
		'''         <i>insertion point</i> is defined as the point at which the
		'''         key would be inserted into the array: the index of the first
		'''         element greater than the key, or <tt>a.length</tt> if all
		'''         elements in the array are less than the specified key.  Note
		'''         that this guarantees that the return value will be &gt;= 0 if
		'''         and only if the key is found. </returns>
		Public Shared Function binarySearch(  a As Long(),   key As Long) As Integer
			Return binarySearch0(a, 0, a.Length, key)
		End Function

		''' <summary>
		''' Searches a range of
		''' the specified array of longs for the specified value using the
		''' binary search algorithm.
		''' The range must be sorted (as
		''' by the <seealso cref="#sort(long[], int, int)"/> method)
		''' prior to making this call.  If it
		''' is not sorted, the results are undefined.  If the range contains
		''' multiple elements with the specified value, there is no guarantee which
		''' one will be found.
		''' </summary>
		''' <param name="a"> the array to be searched </param>
		''' <param name="fromIndex"> the index of the first element (inclusive) to be
		'''          searched </param>
		''' <param name="toIndex"> the index of the last element (exclusive) to be searched </param>
		''' <param name="key"> the value to be searched for </param>
		''' <returns> index of the search key, if it is contained in the array
		'''         within the specified range;
		'''         otherwise, <tt>(-(<i>insertion point</i>) - 1)</tt>.  The
		'''         <i>insertion point</i> is defined as the point at which the
		'''         key would be inserted into the array: the index of the first
		'''         element in the range greater than the key,
		'''         or <tt>toIndex</tt> if all
		'''         elements in the range are less than the specified key.  Note
		'''         that this guarantees that the return value will be &gt;= 0 if
		'''         and only if the key is found. </returns>
		''' <exception cref="IllegalArgumentException">
		'''         if {@code fromIndex > toIndex} </exception>
		''' <exception cref="ArrayIndexOutOfBoundsException">
		'''         if {@code fromIndex < 0 or toIndex > a.length}
		''' @since 1.6 </exception>
		Public Shared Function binarySearch(  a As Long(),   fromIndex As Integer,   toIndex As Integer,   key As Long) As Integer
			rangeCheck(a.Length, fromIndex, toIndex)
			Return binarySearch0(a, fromIndex, toIndex, key)
		End Function

		' Like public version, but without range checks.
		Private Shared Function binarySearch0(  a As Long(),   fromIndex As Integer,   toIndex As Integer,   key As Long) As Integer
			Dim low As Integer = fromIndex
			Dim high As Integer = toIndex - 1

			Do While low <= high
				Dim mid As Integer = CInt(CUInt((low + high)) >> 1)
				Dim midVal As Long = a(mid)

				If midVal < key Then
					low = mid + 1
				ElseIf midVal > key Then
					high = mid - 1
				Else
					Return mid ' key found
				End If
			Loop
			Return -(low + 1) ' key not found.
		End Function

		''' <summary>
		''' Searches the specified array of ints for the specified value using the
		''' binary search algorithm.  The array must be sorted (as
		''' by the <seealso cref="#sort(int[])"/> method) prior to making this call.  If it
		''' is not sorted, the results are undefined.  If the array contains
		''' multiple elements with the specified value, there is no guarantee which
		''' one will be found.
		''' </summary>
		''' <param name="a"> the array to be searched </param>
		''' <param name="key"> the value to be searched for </param>
		''' <returns> index of the search key, if it is contained in the array;
		'''         otherwise, <tt>(-(<i>insertion point</i>) - 1)</tt>.  The
		'''         <i>insertion point</i> is defined as the point at which the
		'''         key would be inserted into the array: the index of the first
		'''         element greater than the key, or <tt>a.length</tt> if all
		'''         elements in the array are less than the specified key.  Note
		'''         that this guarantees that the return value will be &gt;= 0 if
		'''         and only if the key is found. </returns>
		Public Shared Function binarySearch(  a As Integer(),   key As Integer) As Integer
			Return binarySearch0(a, 0, a.Length, key)
		End Function

		''' <summary>
		''' Searches a range of
		''' the specified array of ints for the specified value using the
		''' binary search algorithm.
		''' The range must be sorted (as
		''' by the <seealso cref="#sort(int[], int, int)"/> method)
		''' prior to making this call.  If it
		''' is not sorted, the results are undefined.  If the range contains
		''' multiple elements with the specified value, there is no guarantee which
		''' one will be found.
		''' </summary>
		''' <param name="a"> the array to be searched </param>
		''' <param name="fromIndex"> the index of the first element (inclusive) to be
		'''          searched </param>
		''' <param name="toIndex"> the index of the last element (exclusive) to be searched </param>
		''' <param name="key"> the value to be searched for </param>
		''' <returns> index of the search key, if it is contained in the array
		'''         within the specified range;
		'''         otherwise, <tt>(-(<i>insertion point</i>) - 1)</tt>.  The
		'''         <i>insertion point</i> is defined as the point at which the
		'''         key would be inserted into the array: the index of the first
		'''         element in the range greater than the key,
		'''         or <tt>toIndex</tt> if all
		'''         elements in the range are less than the specified key.  Note
		'''         that this guarantees that the return value will be &gt;= 0 if
		'''         and only if the key is found. </returns>
		''' <exception cref="IllegalArgumentException">
		'''         if {@code fromIndex > toIndex} </exception>
		''' <exception cref="ArrayIndexOutOfBoundsException">
		'''         if {@code fromIndex < 0 or toIndex > a.length}
		''' @since 1.6 </exception>
		Public Shared Function binarySearch(  a As Integer(),   fromIndex As Integer,   toIndex As Integer,   key As Integer) As Integer
			rangeCheck(a.Length, fromIndex, toIndex)
			Return binarySearch0(a, fromIndex, toIndex, key)
		End Function

		' Like public version, but without range checks.
		Private Shared Function binarySearch0(  a As Integer(),   fromIndex As Integer,   toIndex As Integer,   key As Integer) As Integer
			Dim low As Integer = fromIndex
			Dim high As Integer = toIndex - 1

			Do While low <= high
				Dim mid As Integer = CInt(CUInt((low + high)) >> 1)
				Dim midVal As Integer = a(mid)

				If midVal < key Then
					low = mid + 1
				ElseIf midVal > key Then
					high = mid - 1
				Else
					Return mid ' key found
				End If
			Loop
			Return -(low + 1) ' key not found.
		End Function

		''' <summary>
		''' Searches the specified array of shorts for the specified value using
		''' the binary search algorithm.  The array must be sorted
		''' (as by the <seealso cref="#sort(short[])"/> method) prior to making this call.  If
		''' it is not sorted, the results are undefined.  If the array contains
		''' multiple elements with the specified value, there is no guarantee which
		''' one will be found.
		''' </summary>
		''' <param name="a"> the array to be searched </param>
		''' <param name="key"> the value to be searched for </param>
		''' <returns> index of the search key, if it is contained in the array;
		'''         otherwise, <tt>(-(<i>insertion point</i>) - 1)</tt>.  The
		'''         <i>insertion point</i> is defined as the point at which the
		'''         key would be inserted into the array: the index of the first
		'''         element greater than the key, or <tt>a.length</tt> if all
		'''         elements in the array are less than the specified key.  Note
		'''         that this guarantees that the return value will be &gt;= 0 if
		'''         and only if the key is found. </returns>
		Public Shared Function binarySearch(  a As Short(),   key As Short) As Integer
			Return binarySearch0(a, 0, a.Length, key)
		End Function

		''' <summary>
		''' Searches a range of
		''' the specified array of shorts for the specified value using
		''' the binary search algorithm.
		''' The range must be sorted
		''' (as by the <seealso cref="#sort(short[], int, int)"/> method)
		''' prior to making this call.  If
		''' it is not sorted, the results are undefined.  If the range contains
		''' multiple elements with the specified value, there is no guarantee which
		''' one will be found.
		''' </summary>
		''' <param name="a"> the array to be searched </param>
		''' <param name="fromIndex"> the index of the first element (inclusive) to be
		'''          searched </param>
		''' <param name="toIndex"> the index of the last element (exclusive) to be searched </param>
		''' <param name="key"> the value to be searched for </param>
		''' <returns> index of the search key, if it is contained in the array
		'''         within the specified range;
		'''         otherwise, <tt>(-(<i>insertion point</i>) - 1)</tt>.  The
		'''         <i>insertion point</i> is defined as the point at which the
		'''         key would be inserted into the array: the index of the first
		'''         element in the range greater than the key,
		'''         or <tt>toIndex</tt> if all
		'''         elements in the range are less than the specified key.  Note
		'''         that this guarantees that the return value will be &gt;= 0 if
		'''         and only if the key is found. </returns>
		''' <exception cref="IllegalArgumentException">
		'''         if {@code fromIndex > toIndex} </exception>
		''' <exception cref="ArrayIndexOutOfBoundsException">
		'''         if {@code fromIndex < 0 or toIndex > a.length}
		''' @since 1.6 </exception>
		Public Shared Function binarySearch(  a As Short(),   fromIndex As Integer,   toIndex As Integer,   key As Short) As Integer
			rangeCheck(a.Length, fromIndex, toIndex)
			Return binarySearch0(a, fromIndex, toIndex, key)
		End Function

		' Like public version, but without range checks.
		Private Shared Function binarySearch0(  a As Short(),   fromIndex As Integer,   toIndex As Integer,   key As Short) As Integer
			Dim low As Integer = fromIndex
			Dim high As Integer = toIndex - 1

			Do While low <= high
				Dim mid As Integer = CInt(CUInt((low + high)) >> 1)
				Dim midVal As Short = a(mid)

				If midVal < key Then
					low = mid + 1
				ElseIf midVal > key Then
					high = mid - 1
				Else
					Return mid ' key found
				End If
			Loop
			Return -(low + 1) ' key not found.
		End Function

		''' <summary>
		''' Searches the specified array of chars for the specified value using the
		''' binary search algorithm.  The array must be sorted (as
		''' by the <seealso cref="#sort(char[])"/> method) prior to making this call.  If it
		''' is not sorted, the results are undefined.  If the array contains
		''' multiple elements with the specified value, there is no guarantee which
		''' one will be found.
		''' </summary>
		''' <param name="a"> the array to be searched </param>
		''' <param name="key"> the value to be searched for </param>
		''' <returns> index of the search key, if it is contained in the array;
		'''         otherwise, <tt>(-(<i>insertion point</i>) - 1)</tt>.  The
		'''         <i>insertion point</i> is defined as the point at which the
		'''         key would be inserted into the array: the index of the first
		'''         element greater than the key, or <tt>a.length</tt> if all
		'''         elements in the array are less than the specified key.  Note
		'''         that this guarantees that the return value will be &gt;= 0 if
		'''         and only if the key is found. </returns>
		Public Shared Function binarySearch(  a As Char(),   key As Char) As Integer
			Return binarySearch0(a, 0, a.Length, key)
		End Function

		''' <summary>
		''' Searches a range of
		''' the specified array of chars for the specified value using the
		''' binary search algorithm.
		''' The range must be sorted (as
		''' by the <seealso cref="#sort(char[], int, int)"/> method)
		''' prior to making this call.  If it
		''' is not sorted, the results are undefined.  If the range contains
		''' multiple elements with the specified value, there is no guarantee which
		''' one will be found.
		''' </summary>
		''' <param name="a"> the array to be searched </param>
		''' <param name="fromIndex"> the index of the first element (inclusive) to be
		'''          searched </param>
		''' <param name="toIndex"> the index of the last element (exclusive) to be searched </param>
		''' <param name="key"> the value to be searched for </param>
		''' <returns> index of the search key, if it is contained in the array
		'''         within the specified range;
		'''         otherwise, <tt>(-(<i>insertion point</i>) - 1)</tt>.  The
		'''         <i>insertion point</i> is defined as the point at which the
		'''         key would be inserted into the array: the index of the first
		'''         element in the range greater than the key,
		'''         or <tt>toIndex</tt> if all
		'''         elements in the range are less than the specified key.  Note
		'''         that this guarantees that the return value will be &gt;= 0 if
		'''         and only if the key is found. </returns>
		''' <exception cref="IllegalArgumentException">
		'''         if {@code fromIndex > toIndex} </exception>
		''' <exception cref="ArrayIndexOutOfBoundsException">
		'''         if {@code fromIndex < 0 or toIndex > a.length}
		''' @since 1.6 </exception>
		Public Shared Function binarySearch(  a As Char(),   fromIndex As Integer,   toIndex As Integer,   key As Char) As Integer
			rangeCheck(a.Length, fromIndex, toIndex)
			Return binarySearch0(a, fromIndex, toIndex, key)
		End Function

		' Like public version, but without range checks.
		Private Shared Function binarySearch0(  a As Char(),   fromIndex As Integer,   toIndex As Integer,   key As Char) As Integer
			Dim low As Integer = fromIndex
			Dim high As Integer = toIndex - 1

			Do While low <= high
				Dim mid As Integer = CInt(CUInt((low + high)) >> 1)
				Dim midVal As Char = a(mid)

				If midVal < key Then
					low = mid + 1
				ElseIf midVal > key Then
					high = mid - 1
				Else
					Return mid ' key found
				End If
			Loop
			Return -(low + 1) ' key not found.
		End Function

		''' <summary>
		''' Searches the specified array of bytes for the specified value using the
		''' binary search algorithm.  The array must be sorted (as
		''' by the <seealso cref="#sort(byte[])"/> method) prior to making this call.  If it
		''' is not sorted, the results are undefined.  If the array contains
		''' multiple elements with the specified value, there is no guarantee which
		''' one will be found.
		''' </summary>
		''' <param name="a"> the array to be searched </param>
		''' <param name="key"> the value to be searched for </param>
		''' <returns> index of the search key, if it is contained in the array;
		'''         otherwise, <tt>(-(<i>insertion point</i>) - 1)</tt>.  The
		'''         <i>insertion point</i> is defined as the point at which the
		'''         key would be inserted into the array: the index of the first
		'''         element greater than the key, or <tt>a.length</tt> if all
		'''         elements in the array are less than the specified key.  Note
		'''         that this guarantees that the return value will be &gt;= 0 if
		'''         and only if the key is found. </returns>
		Public Shared Function binarySearch(  a As SByte(),   key As SByte) As Integer
			Return binarySearch0(a, 0, a.Length, key)
		End Function

		''' <summary>
		''' Searches a range of
		''' the specified array of bytes for the specified value using the
		''' binary search algorithm.
		''' The range must be sorted (as
		''' by the <seealso cref="#sort(byte[], int, int)"/> method)
		''' prior to making this call.  If it
		''' is not sorted, the results are undefined.  If the range contains
		''' multiple elements with the specified value, there is no guarantee which
		''' one will be found.
		''' </summary>
		''' <param name="a"> the array to be searched </param>
		''' <param name="fromIndex"> the index of the first element (inclusive) to be
		'''          searched </param>
		''' <param name="toIndex"> the index of the last element (exclusive) to be searched </param>
		''' <param name="key"> the value to be searched for </param>
		''' <returns> index of the search key, if it is contained in the array
		'''         within the specified range;
		'''         otherwise, <tt>(-(<i>insertion point</i>) - 1)</tt>.  The
		'''         <i>insertion point</i> is defined as the point at which the
		'''         key would be inserted into the array: the index of the first
		'''         element in the range greater than the key,
		'''         or <tt>toIndex</tt> if all
		'''         elements in the range are less than the specified key.  Note
		'''         that this guarantees that the return value will be &gt;= 0 if
		'''         and only if the key is found. </returns>
		''' <exception cref="IllegalArgumentException">
		'''         if {@code fromIndex > toIndex} </exception>
		''' <exception cref="ArrayIndexOutOfBoundsException">
		'''         if {@code fromIndex < 0 or toIndex > a.length}
		''' @since 1.6 </exception>
		Public Shared Function binarySearch(  a As SByte(),   fromIndex As Integer,   toIndex As Integer,   key As SByte) As Integer
			rangeCheck(a.Length, fromIndex, toIndex)
			Return binarySearch0(a, fromIndex, toIndex, key)
		End Function

		' Like public version, but without range checks.
		Private Shared Function binarySearch0(  a As SByte(),   fromIndex As Integer,   toIndex As Integer,   key As SByte) As Integer
			Dim low As Integer = fromIndex
			Dim high As Integer = toIndex - 1

			Do While low <= high
				Dim mid As Integer = CInt(CUInt((low + high)) >> 1)
				Dim midVal As SByte = a(mid)

				If midVal < key Then
					low = mid + 1
				ElseIf midVal > key Then
					high = mid - 1
				Else
					Return mid ' key found
				End If
			Loop
			Return -(low + 1) ' key not found.
		End Function

		''' <summary>
		''' Searches the specified array of doubles for the specified value using
		''' the binary search algorithm.  The array must be sorted
		''' (as by the <seealso cref="#sort(double[])"/> method) prior to making this call.
		''' If it is not sorted, the results are undefined.  If the array contains
		''' multiple elements with the specified value, there is no guarantee which
		''' one will be found.  This method considers all NaN values to be
		''' equivalent and equal.
		''' </summary>
		''' <param name="a"> the array to be searched </param>
		''' <param name="key"> the value to be searched for </param>
		''' <returns> index of the search key, if it is contained in the array;
		'''         otherwise, <tt>(-(<i>insertion point</i>) - 1)</tt>.  The
		'''         <i>insertion point</i> is defined as the point at which the
		'''         key would be inserted into the array: the index of the first
		'''         element greater than the key, or <tt>a.length</tt> if all
		'''         elements in the array are less than the specified key.  Note
		'''         that this guarantees that the return value will be &gt;= 0 if
		'''         and only if the key is found. </returns>
		Public Shared Function binarySearch(  a As Double(),   key As Double) As Integer
			Return binarySearch0(a, 0, a.Length, key)
		End Function

		''' <summary>
		''' Searches a range of
		''' the specified array of doubles for the specified value using
		''' the binary search algorithm.
		''' The range must be sorted
		''' (as by the <seealso cref="#sort(double[], int, int)"/> method)
		''' prior to making this call.
		''' If it is not sorted, the results are undefined.  If the range contains
		''' multiple elements with the specified value, there is no guarantee which
		''' one will be found.  This method considers all NaN values to be
		''' equivalent and equal.
		''' </summary>
		''' <param name="a"> the array to be searched </param>
		''' <param name="fromIndex"> the index of the first element (inclusive) to be
		'''          searched </param>
		''' <param name="toIndex"> the index of the last element (exclusive) to be searched </param>
		''' <param name="key"> the value to be searched for </param>
		''' <returns> index of the search key, if it is contained in the array
		'''         within the specified range;
		'''         otherwise, <tt>(-(<i>insertion point</i>) - 1)</tt>.  The
		'''         <i>insertion point</i> is defined as the point at which the
		'''         key would be inserted into the array: the index of the first
		'''         element in the range greater than the key,
		'''         or <tt>toIndex</tt> if all
		'''         elements in the range are less than the specified key.  Note
		'''         that this guarantees that the return value will be &gt;= 0 if
		'''         and only if the key is found. </returns>
		''' <exception cref="IllegalArgumentException">
		'''         if {@code fromIndex > toIndex} </exception>
		''' <exception cref="ArrayIndexOutOfBoundsException">
		'''         if {@code fromIndex < 0 or toIndex > a.length}
		''' @since 1.6 </exception>
		Public Shared Function binarySearch(  a As Double(),   fromIndex As Integer,   toIndex As Integer,   key As Double) As Integer
			rangeCheck(a.Length, fromIndex, toIndex)
			Return binarySearch0(a, fromIndex, toIndex, key)
		End Function

		' Like public version, but without range checks.
		Private Shared Function binarySearch0(  a As Double(),   fromIndex As Integer,   toIndex As Integer,   key As Double) As Integer
			Dim low As Integer = fromIndex
			Dim high As Integer = toIndex - 1

			Do While low <= high
				Dim mid As Integer = CInt(CUInt((low + high)) >> 1)
				Dim midVal As Double = a(mid)

				If midVal < key Then
					low = mid + 1 ' Neither val is NaN, thisVal is smaller
				ElseIf midVal > key Then
					high = mid - 1 ' Neither val is NaN, thisVal is larger
				Else
					Dim midBits As Long = java.lang.[Double].doubleToLongBits(midVal)
					Dim keyBits As Long = java.lang.[Double].doubleToLongBits(key)
					If midBits = keyBits Then ' Values are equal
						Return mid ' Key found
					ElseIf midBits < keyBits Then ' (-0.0, 0.0) or (!NaN, NaN)
						low = mid + 1
					Else ' (0.0, -0.0) or (NaN, !NaN)
						high = mid - 1
					End If
				End If
			Loop
			Return -(low + 1) ' key not found.
		End Function

		''' <summary>
		''' Searches the specified array of floats for the specified value using
		''' the binary search algorithm. The array must be sorted
		''' (as by the <seealso cref="#sort(float[])"/> method) prior to making this call. If
		''' it is not sorted, the results are undefined. If the array contains
		''' multiple elements with the specified value, there is no guarantee which
		''' one will be found. This method considers all NaN values to be
		''' equivalent and equal.
		''' </summary>
		''' <param name="a"> the array to be searched </param>
		''' <param name="key"> the value to be searched for </param>
		''' <returns> index of the search key, if it is contained in the array;
		'''         otherwise, <tt>(-(<i>insertion point</i>) - 1)</tt>. The
		'''         <i>insertion point</i> is defined as the point at which the
		'''         key would be inserted into the array: the index of the first
		'''         element greater than the key, or <tt>a.length</tt> if all
		'''         elements in the array are less than the specified key. Note
		'''         that this guarantees that the return value will be &gt;= 0 if
		'''         and only if the key is found. </returns>
		Public Shared Function binarySearch(  a As Single(),   key As Single) As Integer
			Return binarySearch0(a, 0, a.Length, key)
		End Function

		''' <summary>
		''' Searches a range of
		''' the specified array of floats for the specified value using
		''' the binary search algorithm.
		''' The range must be sorted
		''' (as by the <seealso cref="#sort(float[], int, int)"/> method)
		''' prior to making this call. If
		''' it is not sorted, the results are undefined. If the range contains
		''' multiple elements with the specified value, there is no guarantee which
		''' one will be found. This method considers all NaN values to be
		''' equivalent and equal.
		''' </summary>
		''' <param name="a"> the array to be searched </param>
		''' <param name="fromIndex"> the index of the first element (inclusive) to be
		'''          searched </param>
		''' <param name="toIndex"> the index of the last element (exclusive) to be searched </param>
		''' <param name="key"> the value to be searched for </param>
		''' <returns> index of the search key, if it is contained in the array
		'''         within the specified range;
		'''         otherwise, <tt>(-(<i>insertion point</i>) - 1)</tt>. The
		'''         <i>insertion point</i> is defined as the point at which the
		'''         key would be inserted into the array: the index of the first
		'''         element in the range greater than the key,
		'''         or <tt>toIndex</tt> if all
		'''         elements in the range are less than the specified key. Note
		'''         that this guarantees that the return value will be &gt;= 0 if
		'''         and only if the key is found. </returns>
		''' <exception cref="IllegalArgumentException">
		'''         if {@code fromIndex > toIndex} </exception>
		''' <exception cref="ArrayIndexOutOfBoundsException">
		'''         if {@code fromIndex < 0 or toIndex > a.length}
		''' @since 1.6 </exception>
		Public Shared Function binarySearch(  a As Single(),   fromIndex As Integer,   toIndex As Integer,   key As Single) As Integer
			rangeCheck(a.Length, fromIndex, toIndex)
			Return binarySearch0(a, fromIndex, toIndex, key)
		End Function

		' Like public version, but without range checks.
		Private Shared Function binarySearch0(  a As Single(),   fromIndex As Integer,   toIndex As Integer,   key As Single) As Integer
			Dim low As Integer = fromIndex
			Dim high As Integer = toIndex - 1

			Do While low <= high
				Dim mid As Integer = CInt(CUInt((low + high)) >> 1)
				Dim midVal As Single = a(mid)

				If midVal < key Then
					low = mid + 1 ' Neither val is NaN, thisVal is smaller
				ElseIf midVal > key Then
					high = mid - 1 ' Neither val is NaN, thisVal is larger
				Else
					Dim midBits As Integer = Float.floatToIntBits(midVal)
					Dim keyBits As Integer = Float.floatToIntBits(key)
					If midBits = keyBits Then ' Values are equal
						Return mid ' Key found
					ElseIf midBits < keyBits Then ' (-0.0, 0.0) or (!NaN, NaN)
						low = mid + 1
					Else ' (0.0, -0.0) or (NaN, !NaN)
						high = mid - 1
					End If
				End If
			Loop
			Return -(low + 1) ' key not found.
		End Function

		''' <summary>
		''' Searches the specified array for the specified object using the binary
		''' search algorithm. The array must be sorted into ascending order
		''' according to the
		''' <seealso cref="Comparable natural ordering"/>
		''' of its elements (as by the
		''' <seealso cref="#sort(Object[])"/> method) prior to making this call.
		''' If it is not sorted, the results are undefined.
		''' (If the array contains elements that are not mutually comparable (for
		''' example, strings and integers), it <i>cannot</i> be sorted according
		''' to the natural ordering of its elements, hence results are undefined.)
		''' If the array contains multiple
		''' elements equal to the specified object, there is no guarantee which
		''' one will be found.
		''' </summary>
		''' <param name="a"> the array to be searched </param>
		''' <param name="key"> the value to be searched for </param>
		''' <returns> index of the search key, if it is contained in the array;
		'''         otherwise, <tt>(-(<i>insertion point</i>) - 1)</tt>.  The
		'''         <i>insertion point</i> is defined as the point at which the
		'''         key would be inserted into the array: the index of the first
		'''         element greater than the key, or <tt>a.length</tt> if all
		'''         elements in the array are less than the specified key.  Note
		'''         that this guarantees that the return value will be &gt;= 0 if
		'''         and only if the key is found. </returns>
		''' <exception cref="ClassCastException"> if the search key is not comparable to the
		'''         elements of the array. </exception>
		Public Shared Function binarySearch(  a As Object(),   key As Object) As Integer
			Return binarySearch0(a, 0, a.Length, key)
		End Function

		''' <summary>
		''' Searches a range of
		''' the specified array for the specified object using the binary
		''' search algorithm.
		''' The range must be sorted into ascending order
		''' according to the
		''' <seealso cref="Comparable natural ordering"/>
		''' of its elements (as by the
		''' <seealso cref="#sort(Object[], int, int)"/> method) prior to making this
		''' call.  If it is not sorted, the results are undefined.
		''' (If the range contains elements that are not mutually comparable (for
		''' example, strings and integers), it <i>cannot</i> be sorted according
		''' to the natural ordering of its elements, hence results are undefined.)
		''' If the range contains multiple
		''' elements equal to the specified object, there is no guarantee which
		''' one will be found.
		''' </summary>
		''' <param name="a"> the array to be searched </param>
		''' <param name="fromIndex"> the index of the first element (inclusive) to be
		'''          searched </param>
		''' <param name="toIndex"> the index of the last element (exclusive) to be searched </param>
		''' <param name="key"> the value to be searched for </param>
		''' <returns> index of the search key, if it is contained in the array
		'''         within the specified range;
		'''         otherwise, <tt>(-(<i>insertion point</i>) - 1)</tt>.  The
		'''         <i>insertion point</i> is defined as the point at which the
		'''         key would be inserted into the array: the index of the first
		'''         element in the range greater than the key,
		'''         or <tt>toIndex</tt> if all
		'''         elements in the range are less than the specified key.  Note
		'''         that this guarantees that the return value will be &gt;= 0 if
		'''         and only if the key is found. </returns>
		''' <exception cref="ClassCastException"> if the search key is not comparable to the
		'''         elements of the array within the specified range. </exception>
		''' <exception cref="IllegalArgumentException">
		'''         if {@code fromIndex > toIndex} </exception>
		''' <exception cref="ArrayIndexOutOfBoundsException">
		'''         if {@code fromIndex < 0 or toIndex > a.length}
		''' @since 1.6 </exception>
		Public Shared Function binarySearch(  a As Object(),   fromIndex As Integer,   toIndex As Integer,   key As Object) As Integer
			rangeCheck(a.Length, fromIndex, toIndex)
			Return binarySearch0(a, fromIndex, toIndex, key)
		End Function

		' Like public version, but without range checks.
		Private Shared Function binarySearch0(  a As Object(),   fromIndex As Integer,   toIndex As Integer,   key As Object) As Integer
			Dim low As Integer = fromIndex
			Dim high As Integer = toIndex - 1

			Do While low <= high
				Dim mid As Integer = CInt(CUInt((low + high)) >> 1)
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
				Dim midVal As Comparable = CType(a(mid), Comparable)
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
				Dim cmp As Integer = midVal.CompareTo(key)

				If cmp < 0 Then
					low = mid + 1
				ElseIf cmp > 0 Then
					high = mid - 1
				Else
					Return mid ' key found
				End If
			Loop
			Return -(low + 1) ' key not found.
		End Function

		''' <summary>
		''' Searches the specified array for the specified object using the binary
		''' search algorithm.  The array must be sorted into ascending order
		''' according to the specified comparator (as by the
		''' <seealso cref="#sort(Object[], Comparator) sort(T[], Comparator)"/>
		''' method) prior to making this call.  If it is
		''' not sorted, the results are undefined.
		''' If the array contains multiple
		''' elements equal to the specified object, there is no guarantee which one
		''' will be found.
		''' </summary>
		''' @param <T> the class of the objects in the array </param>
		''' <param name="a"> the array to be searched </param>
		''' <param name="key"> the value to be searched for </param>
		''' <param name="c"> the comparator by which the array is ordered.  A
		'''        <tt>null</tt> value indicates that the elements'
		'''        <seealso cref="Comparable natural ordering"/> should be used. </param>
		''' <returns> index of the search key, if it is contained in the array;
		'''         otherwise, <tt>(-(<i>insertion point</i>) - 1)</tt>.  The
		'''         <i>insertion point</i> is defined as the point at which the
		'''         key would be inserted into the array: the index of the first
		'''         element greater than the key, or <tt>a.length</tt> if all
		'''         elements in the array are less than the specified key.  Note
		'''         that this guarantees that the return value will be &gt;= 0 if
		'''         and only if the key is found. </returns>
		''' <exception cref="ClassCastException"> if the array contains elements that are not
		'''         <i>mutually comparable</i> using the specified comparator,
		'''         or the search key is not comparable to the
		'''         elements of the array using this comparator. </exception>
'JAVA TO VB CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
		Public Shared Function binarySearch(Of T, T1)(  a As T(),   key As T,   c As Comparator(Of T1)) As Integer
			Return binarySearch0(a, 0, a.Length, key, c)
		End Function

		''' <summary>
		''' Searches a range of
		''' the specified array for the specified object using the binary
		''' search algorithm.
		''' The range must be sorted into ascending order
		''' according to the specified comparator (as by the
		''' {@link #sort(Object[], int, int, Comparator)
		''' sort(T[], int, int, Comparator)}
		''' method) prior to making this call.
		''' If it is not sorted, the results are undefined.
		''' If the range contains multiple elements equal to the specified object,
		''' there is no guarantee which one will be found.
		''' </summary>
		''' @param <T> the class of the objects in the array </param>
		''' <param name="a"> the array to be searched </param>
		''' <param name="fromIndex"> the index of the first element (inclusive) to be
		'''          searched </param>
		''' <param name="toIndex"> the index of the last element (exclusive) to be searched </param>
		''' <param name="key"> the value to be searched for </param>
		''' <param name="c"> the comparator by which the array is ordered.  A
		'''        <tt>null</tt> value indicates that the elements'
		'''        <seealso cref="Comparable natural ordering"/> should be used. </param>
		''' <returns> index of the search key, if it is contained in the array
		'''         within the specified range;
		'''         otherwise, <tt>(-(<i>insertion point</i>) - 1)</tt>.  The
		'''         <i>insertion point</i> is defined as the point at which the
		'''         key would be inserted into the array: the index of the first
		'''         element in the range greater than the key,
		'''         or <tt>toIndex</tt> if all
		'''         elements in the range are less than the specified key.  Note
		'''         that this guarantees that the return value will be &gt;= 0 if
		'''         and only if the key is found. </returns>
		''' <exception cref="ClassCastException"> if the range contains elements that are not
		'''         <i>mutually comparable</i> using the specified comparator,
		'''         or the search key is not comparable to the
		'''         elements in the range using this comparator. </exception>
		''' <exception cref="IllegalArgumentException">
		'''         if {@code fromIndex > toIndex} </exception>
		''' <exception cref="ArrayIndexOutOfBoundsException">
		'''         if {@code fromIndex < 0 or toIndex > a.length}
		''' @since 1.6 </exception>
'JAVA TO VB CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
		Public Shared Function binarySearch(Of T, T1)(  a As T(),   fromIndex As Integer,   toIndex As Integer,   key As T,   c As Comparator(Of T1)) As Integer
			rangeCheck(a.Length, fromIndex, toIndex)
			Return binarySearch0(a, fromIndex, toIndex, key, c)
		End Function

		' Like public version, but without range checks.
'JAVA TO VB CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
		Private Shared Function binarySearch0(Of T, T1)(  a As T(),   fromIndex As Integer,   toIndex As Integer,   key As T,   c As Comparator(Of T1)) As Integer
			If c Is Nothing Then Return binarySearch0(a, fromIndex, toIndex, key)
			Dim low As Integer = fromIndex
			Dim high As Integer = toIndex - 1

			Do While low <= high
				Dim mid As Integer = CInt(CUInt((low + high)) >> 1)
				Dim midVal As T = a(mid)
				Dim cmp As Integer = c.Compare(midVal, key)
				If cmp < 0 Then
					low = mid + 1
				ElseIf cmp > 0 Then
					high = mid - 1
				Else
					Return mid ' key found
				End If
			Loop
			Return -(low + 1) ' key not found.
		End Function

		' Equality Testing

		''' <summary>
		''' Returns <tt>true</tt> if the two specified arrays of longs are
		''' <i>equal</i> to one another.  Two arrays are considered equal if both
		''' arrays contain the same number of elements, and all corresponding pairs
		''' of elements in the two arrays are equal.  In other words, two arrays
		''' are equal if they contain the same elements in the same order.  Also,
		''' two array references are considered equal if both are <tt>null</tt>.<p>
		''' </summary>
		''' <param name="a"> one array to be tested for equality </param>
		''' <param name="a2"> the other array to be tested for equality </param>
		''' <returns> <tt>true</tt> if the two arrays are equal </returns>
		Public Shared Function Equals(  a As Long(),   a2 As Long()) As Boolean
			If a=a2 Then Return True
			If a Is Nothing OrElse a2 Is Nothing Then Return False

			Dim length As Integer = a.Length
			If a2.Length <> length Then Return False

			For i As Integer = 0 To length - 1
				If a(i) <> a2(i) Then Return False
			Next i

			Return True
		End Function

		''' <summary>
		''' Returns <tt>true</tt> if the two specified arrays of ints are
		''' <i>equal</i> to one another.  Two arrays are considered equal if both
		''' arrays contain the same number of elements, and all corresponding pairs
		''' of elements in the two arrays are equal.  In other words, two arrays
		''' are equal if they contain the same elements in the same order.  Also,
		''' two array references are considered equal if both are <tt>null</tt>.<p>
		''' </summary>
		''' <param name="a"> one array to be tested for equality </param>
		''' <param name="a2"> the other array to be tested for equality </param>
		''' <returns> <tt>true</tt> if the two arrays are equal </returns>
		Public Shared Function Equals(  a As Integer(),   a2 As Integer()) As Boolean
			If a=a2 Then Return True
			If a Is Nothing OrElse a2 Is Nothing Then Return False

			Dim length As Integer = a.Length
			If a2.Length <> length Then Return False

			For i As Integer = 0 To length - 1
				If a(i) <> a2(i) Then Return False
			Next i

			Return True
		End Function

		''' <summary>
		''' Returns <tt>true</tt> if the two specified arrays of shorts are
		''' <i>equal</i> to one another.  Two arrays are considered equal if both
		''' arrays contain the same number of elements, and all corresponding pairs
		''' of elements in the two arrays are equal.  In other words, two arrays
		''' are equal if they contain the same elements in the same order.  Also,
		''' two array references are considered equal if both are <tt>null</tt>.<p>
		''' </summary>
		''' <param name="a"> one array to be tested for equality </param>
		''' <param name="a2"> the other array to be tested for equality </param>
		''' <returns> <tt>true</tt> if the two arrays are equal </returns>
		Public Shared Function Equals(  a As Short(),   a2 As Short()) As Boolean
			If a=a2 Then Return True
			If a Is Nothing OrElse a2 Is Nothing Then Return False

			Dim length As Integer = a.Length
			If a2.Length <> length Then Return False

			For i As Integer = 0 To length - 1
				If a(i) <> a2(i) Then Return False
			Next i

			Return True
		End Function

		''' <summary>
		''' Returns <tt>true</tt> if the two specified arrays of chars are
		''' <i>equal</i> to one another.  Two arrays are considered equal if both
		''' arrays contain the same number of elements, and all corresponding pairs
		''' of elements in the two arrays are equal.  In other words, two arrays
		''' are equal if they contain the same elements in the same order.  Also,
		''' two array references are considered equal if both are <tt>null</tt>.<p>
		''' </summary>
		''' <param name="a"> one array to be tested for equality </param>
		''' <param name="a2"> the other array to be tested for equality </param>
		''' <returns> <tt>true</tt> if the two arrays are equal </returns>
		Public Shared Function Equals(  a As Char(),   a2 As Char()) As Boolean
			If a=a2 Then Return True
			If a Is Nothing OrElse a2 Is Nothing Then Return False

			Dim length As Integer = a.Length
			If a2.Length <> length Then Return False

			For i As Integer = 0 To length - 1
				If a(i) <> a2(i) Then Return False
			Next i

			Return True
		End Function

		''' <summary>
		''' Returns <tt>true</tt> if the two specified arrays of bytes are
		''' <i>equal</i> to one another.  Two arrays are considered equal if both
		''' arrays contain the same number of elements, and all corresponding pairs
		''' of elements in the two arrays are equal.  In other words, two arrays
		''' are equal if they contain the same elements in the same order.  Also,
		''' two array references are considered equal if both are <tt>null</tt>.<p>
		''' </summary>
		''' <param name="a"> one array to be tested for equality </param>
		''' <param name="a2"> the other array to be tested for equality </param>
		''' <returns> <tt>true</tt> if the two arrays are equal </returns>
		Public Shared Function Equals(  a As SByte(),   a2 As SByte()) As Boolean
			If a=a2 Then Return True
			If a Is Nothing OrElse a2 Is Nothing Then Return False

			Dim length As Integer = a.Length
			If a2.Length <> length Then Return False

			For i As Integer = 0 To length - 1
				If a(i) <> a2(i) Then Return False
			Next i

			Return True
		End Function

		''' <summary>
		''' Returns <tt>true</tt> if the two specified arrays of booleans are
		''' <i>equal</i> to one another.  Two arrays are considered equal if both
		''' arrays contain the same number of elements, and all corresponding pairs
		''' of elements in the two arrays are equal.  In other words, two arrays
		''' are equal if they contain the same elements in the same order.  Also,
		''' two array references are considered equal if both are <tt>null</tt>.<p>
		''' </summary>
		''' <param name="a"> one array to be tested for equality </param>
		''' <param name="a2"> the other array to be tested for equality </param>
		''' <returns> <tt>true</tt> if the two arrays are equal </returns>
		Public Shared Function Equals(  a As Boolean(),   a2 As Boolean()) As Boolean
			If a=a2 Then Return True
			If a Is Nothing OrElse a2 Is Nothing Then Return False

			Dim length As Integer = a.Length
			If a2.Length <> length Then Return False

			For i As Integer = 0 To length - 1
				If a(i) <> a2(i) Then Return False
			Next i

			Return True
		End Function

		''' <summary>
		''' Returns <tt>true</tt> if the two specified arrays of doubles are
		''' <i>equal</i> to one another.  Two arrays are considered equal if both
		''' arrays contain the same number of elements, and all corresponding pairs
		''' of elements in the two arrays are equal.  In other words, two arrays
		''' are equal if they contain the same elements in the same order.  Also,
		''' two array references are considered equal if both are <tt>null</tt>.<p>
		''' 
		''' Two doubles <tt>d1</tt> and <tt>d2</tt> are considered equal if:
		''' <pre>    <tt>new Double(d1).equals(new Double(d2))</tt></pre>
		''' (Unlike the <tt>==</tt> operator, this method considers
		''' <tt>NaN</tt> equals to itself, and 0.0d unequal to -0.0d.)
		''' </summary>
		''' <param name="a"> one array to be tested for equality </param>
		''' <param name="a2"> the other array to be tested for equality </param>
		''' <returns> <tt>true</tt> if the two arrays are equal </returns>
		''' <seealso cref= Double#equals(Object) </seealso>
		Public Shared Function Equals(  a As Double(),   a2 As Double()) As Boolean
			If a=a2 Then Return True
			If a Is Nothing OrElse a2 Is Nothing Then Return False

			Dim length As Integer = a.Length
			If a2.Length <> length Then Return False

			For i As Integer = 0 To length - 1
				If java.lang.[Double].doubleToLongBits(a(i))<>Double.doubleToLongBits(a2(i)) Then Return False
			Next i

			Return True
		End Function

		''' <summary>
		''' Returns <tt>true</tt> if the two specified arrays of floats are
		''' <i>equal</i> to one another.  Two arrays are considered equal if both
		''' arrays contain the same number of elements, and all corresponding pairs
		''' of elements in the two arrays are equal.  In other words, two arrays
		''' are equal if they contain the same elements in the same order.  Also,
		''' two array references are considered equal if both are <tt>null</tt>.<p>
		''' 
		''' Two floats <tt>f1</tt> and <tt>f2</tt> are considered equal if:
		''' <pre>    <tt>new Float(f1).equals(new Float(f2))</tt></pre>
		''' (Unlike the <tt>==</tt> operator, this method considers
		''' <tt>NaN</tt> equals to itself, and 0.0f unequal to -0.0f.)
		''' </summary>
		''' <param name="a"> one array to be tested for equality </param>
		''' <param name="a2"> the other array to be tested for equality </param>
		''' <returns> <tt>true</tt> if the two arrays are equal </returns>
		''' <seealso cref= Float#equals(Object) </seealso>
		Public Shared Function Equals(  a As Single(),   a2 As Single()) As Boolean
			If a=a2 Then Return True
			If a Is Nothing OrElse a2 Is Nothing Then Return False

			Dim length As Integer = a.Length
			If a2.Length <> length Then Return False

			For i As Integer = 0 To length - 1
				If Float.floatToIntBits(a(i))<>Float.floatToIntBits(a2(i)) Then Return False
			Next i

			Return True
		End Function

		''' <summary>
		''' Returns <tt>true</tt> if the two specified arrays of Objects are
		''' <i>equal</i> to one another.  The two arrays are considered equal if
		''' both arrays contain the same number of elements, and all corresponding
		''' pairs of elements in the two arrays are equal.  Two objects <tt>e1</tt>
		''' and <tt>e2</tt> are considered <i>equal</i> if <tt>(e1==null ? e2==null
		''' : e1.equals(e2))</tt>.  In other words, the two arrays are equal if
		''' they contain the same elements in the same order.  Also, two array
		''' references are considered equal if both are <tt>null</tt>.<p>
		''' </summary>
		''' <param name="a"> one array to be tested for equality </param>
		''' <param name="a2"> the other array to be tested for equality </param>
		''' <returns> <tt>true</tt> if the two arrays are equal </returns>
		Public Shared Function Equals(  a As Object(),   a2 As Object()) As Boolean
			If a=a2 Then Return True
			If a Is Nothing OrElse a2 Is Nothing Then Return False

			Dim length As Integer = a.Length
			If a2.Length <> length Then Return False

			For i As Integer = 0 To length - 1
				Dim o1 As Object = a(i)
				Dim o2 As Object = a2(i)
				If Not(If(o1 Is Nothing, o2 Is Nothing, o1.Equals(o2))) Then Return False
			Next i

			Return True
		End Function

		' Filling

		''' <summary>
		''' Assigns the specified long value to each element of the specified array
		''' of longs.
		''' </summary>
		''' <param name="a"> the array to be filled </param>
		''' <param name="val"> the value to be stored in all elements of the array </param>
		Public Shared Sub fill(  a As Long(),   val As Long)
			Dim i As Integer = 0
			Dim len As Integer = a.Length
			Do While i < len
				a(i) = val
				i += 1
			Loop
		End Sub

		''' <summary>
		''' Assigns the specified long value to each element of the specified
		''' range of the specified array of longs.  The range to be filled
		''' extends from index <tt>fromIndex</tt>, inclusive, to index
		''' <tt>toIndex</tt>, exclusive.  (If <tt>fromIndex==toIndex</tt>, the
		''' range to be filled is empty.)
		''' </summary>
		''' <param name="a"> the array to be filled </param>
		''' <param name="fromIndex"> the index of the first element (inclusive) to be
		'''        filled with the specified value </param>
		''' <param name="toIndex"> the index of the last element (exclusive) to be
		'''        filled with the specified value </param>
		''' <param name="val"> the value to be stored in all elements of the array </param>
		''' <exception cref="IllegalArgumentException"> if <tt>fromIndex &gt; toIndex</tt> </exception>
		''' <exception cref="ArrayIndexOutOfBoundsException"> if <tt>fromIndex &lt; 0</tt> or
		'''         <tt>toIndex &gt; a.length</tt> </exception>
		Public Shared Sub fill(  a As Long(),   fromIndex As Integer,   toIndex As Integer,   val As Long)
			rangeCheck(a.Length, fromIndex, toIndex)
			For i As Integer = fromIndex To toIndex - 1
				a(i) = val
			Next i
		End Sub

		''' <summary>
		''' Assigns the specified int value to each element of the specified array
		''' of ints.
		''' </summary>
		''' <param name="a"> the array to be filled </param>
		''' <param name="val"> the value to be stored in all elements of the array </param>
		Public Shared Sub fill(  a As Integer(),   val As Integer)
			Dim i As Integer = 0
			Dim len As Integer = a.Length
			Do While i < len
				a(i) = val
				i += 1
			Loop
		End Sub

		''' <summary>
		''' Assigns the specified int value to each element of the specified
		''' range of the specified array of ints.  The range to be filled
		''' extends from index <tt>fromIndex</tt>, inclusive, to index
		''' <tt>toIndex</tt>, exclusive.  (If <tt>fromIndex==toIndex</tt>, the
		''' range to be filled is empty.)
		''' </summary>
		''' <param name="a"> the array to be filled </param>
		''' <param name="fromIndex"> the index of the first element (inclusive) to be
		'''        filled with the specified value </param>
		''' <param name="toIndex"> the index of the last element (exclusive) to be
		'''        filled with the specified value </param>
		''' <param name="val"> the value to be stored in all elements of the array </param>
		''' <exception cref="IllegalArgumentException"> if <tt>fromIndex &gt; toIndex</tt> </exception>
		''' <exception cref="ArrayIndexOutOfBoundsException"> if <tt>fromIndex &lt; 0</tt> or
		'''         <tt>toIndex &gt; a.length</tt> </exception>
		Public Shared Sub fill(  a As Integer(),   fromIndex As Integer,   toIndex As Integer,   val As Integer)
			rangeCheck(a.Length, fromIndex, toIndex)
			For i As Integer = fromIndex To toIndex - 1
				a(i) = val
			Next i
		End Sub

		''' <summary>
		''' Assigns the specified short value to each element of the specified array
		''' of shorts.
		''' </summary>
		''' <param name="a"> the array to be filled </param>
		''' <param name="val"> the value to be stored in all elements of the array </param>
		Public Shared Sub fill(  a As Short(),   val As Short)
			Dim i As Integer = 0
			Dim len As Integer = a.Length
			Do While i < len
				a(i) = val
				i += 1
			Loop
		End Sub

		''' <summary>
		''' Assigns the specified short value to each element of the specified
		''' range of the specified array of shorts.  The range to be filled
		''' extends from index <tt>fromIndex</tt>, inclusive, to index
		''' <tt>toIndex</tt>, exclusive.  (If <tt>fromIndex==toIndex</tt>, the
		''' range to be filled is empty.)
		''' </summary>
		''' <param name="a"> the array to be filled </param>
		''' <param name="fromIndex"> the index of the first element (inclusive) to be
		'''        filled with the specified value </param>
		''' <param name="toIndex"> the index of the last element (exclusive) to be
		'''        filled with the specified value </param>
		''' <param name="val"> the value to be stored in all elements of the array </param>
		''' <exception cref="IllegalArgumentException"> if <tt>fromIndex &gt; toIndex</tt> </exception>
		''' <exception cref="ArrayIndexOutOfBoundsException"> if <tt>fromIndex &lt; 0</tt> or
		'''         <tt>toIndex &gt; a.length</tt> </exception>
		Public Shared Sub fill(  a As Short(),   fromIndex As Integer,   toIndex As Integer,   val As Short)
			rangeCheck(a.Length, fromIndex, toIndex)
			For i As Integer = fromIndex To toIndex - 1
				a(i) = val
			Next i
		End Sub

		''' <summary>
		''' Assigns the specified char value to each element of the specified array
		''' of chars.
		''' </summary>
		''' <param name="a"> the array to be filled </param>
		''' <param name="val"> the value to be stored in all elements of the array </param>
		Public Shared Sub fill(  a As Char(),   val As Char)
			Dim i As Integer = 0
			Dim len As Integer = a.Length
			Do While i < len
				a(i) = val
				i += 1
			Loop
		End Sub

		''' <summary>
		''' Assigns the specified char value to each element of the specified
		''' range of the specified array of chars.  The range to be filled
		''' extends from index <tt>fromIndex</tt>, inclusive, to index
		''' <tt>toIndex</tt>, exclusive.  (If <tt>fromIndex==toIndex</tt>, the
		''' range to be filled is empty.)
		''' </summary>
		''' <param name="a"> the array to be filled </param>
		''' <param name="fromIndex"> the index of the first element (inclusive) to be
		'''        filled with the specified value </param>
		''' <param name="toIndex"> the index of the last element (exclusive) to be
		'''        filled with the specified value </param>
		''' <param name="val"> the value to be stored in all elements of the array </param>
		''' <exception cref="IllegalArgumentException"> if <tt>fromIndex &gt; toIndex</tt> </exception>
		''' <exception cref="ArrayIndexOutOfBoundsException"> if <tt>fromIndex &lt; 0</tt> or
		'''         <tt>toIndex &gt; a.length</tt> </exception>
		Public Shared Sub fill(  a As Char(),   fromIndex As Integer,   toIndex As Integer,   val As Char)
			rangeCheck(a.Length, fromIndex, toIndex)
			For i As Integer = fromIndex To toIndex - 1
				a(i) = val
			Next i
		End Sub

		''' <summary>
		''' Assigns the specified byte value to each element of the specified array
		''' of bytes.
		''' </summary>
		''' <param name="a"> the array to be filled </param>
		''' <param name="val"> the value to be stored in all elements of the array </param>
		Public Shared Sub fill(  a As SByte(),   val As SByte)
			Dim i As Integer = 0
			Dim len As Integer = a.Length
			Do While i < len
				a(i) = val
				i += 1
			Loop
		End Sub

		''' <summary>
		''' Assigns the specified byte value to each element of the specified
		''' range of the specified array of bytes.  The range to be filled
		''' extends from index <tt>fromIndex</tt>, inclusive, to index
		''' <tt>toIndex</tt>, exclusive.  (If <tt>fromIndex==toIndex</tt>, the
		''' range to be filled is empty.)
		''' </summary>
		''' <param name="a"> the array to be filled </param>
		''' <param name="fromIndex"> the index of the first element (inclusive) to be
		'''        filled with the specified value </param>
		''' <param name="toIndex"> the index of the last element (exclusive) to be
		'''        filled with the specified value </param>
		''' <param name="val"> the value to be stored in all elements of the array </param>
		''' <exception cref="IllegalArgumentException"> if <tt>fromIndex &gt; toIndex</tt> </exception>
		''' <exception cref="ArrayIndexOutOfBoundsException"> if <tt>fromIndex &lt; 0</tt> or
		'''         <tt>toIndex &gt; a.length</tt> </exception>
		Public Shared Sub fill(  a As SByte(),   fromIndex As Integer,   toIndex As Integer,   val As SByte)
			rangeCheck(a.Length, fromIndex, toIndex)
			For i As Integer = fromIndex To toIndex - 1
				a(i) = val
			Next i
		End Sub

		''' <summary>
		''' Assigns the specified boolean value to each element of the specified
		''' array of booleans.
		''' </summary>
		''' <param name="a"> the array to be filled </param>
		''' <param name="val"> the value to be stored in all elements of the array </param>
		Public Shared Sub fill(  a As Boolean(),   val As Boolean)
			Dim i As Integer = 0
			Dim len As Integer = a.Length
			Do While i < len
				a(i) = val
				i += 1
			Loop
		End Sub

		''' <summary>
		''' Assigns the specified boolean value to each element of the specified
		''' range of the specified array of booleans.  The range to be filled
		''' extends from index <tt>fromIndex</tt>, inclusive, to index
		''' <tt>toIndex</tt>, exclusive.  (If <tt>fromIndex==toIndex</tt>, the
		''' range to be filled is empty.)
		''' </summary>
		''' <param name="a"> the array to be filled </param>
		''' <param name="fromIndex"> the index of the first element (inclusive) to be
		'''        filled with the specified value </param>
		''' <param name="toIndex"> the index of the last element (exclusive) to be
		'''        filled with the specified value </param>
		''' <param name="val"> the value to be stored in all elements of the array </param>
		''' <exception cref="IllegalArgumentException"> if <tt>fromIndex &gt; toIndex</tt> </exception>
		''' <exception cref="ArrayIndexOutOfBoundsException"> if <tt>fromIndex &lt; 0</tt> or
		'''         <tt>toIndex &gt; a.length</tt> </exception>
		Public Shared Sub fill(  a As Boolean(),   fromIndex As Integer,   toIndex As Integer,   val As Boolean)
			rangeCheck(a.Length, fromIndex, toIndex)
			For i As Integer = fromIndex To toIndex - 1
				a(i) = val
			Next i
		End Sub

		''' <summary>
		''' Assigns the specified double value to each element of the specified
		''' array of doubles.
		''' </summary>
		''' <param name="a"> the array to be filled </param>
		''' <param name="val"> the value to be stored in all elements of the array </param>
		Public Shared Sub fill(  a As Double(),   val As Double)
			Dim i As Integer = 0
			Dim len As Integer = a.Length
			Do While i < len
				a(i) = val
				i += 1
			Loop
		End Sub

		''' <summary>
		''' Assigns the specified double value to each element of the specified
		''' range of the specified array of doubles.  The range to be filled
		''' extends from index <tt>fromIndex</tt>, inclusive, to index
		''' <tt>toIndex</tt>, exclusive.  (If <tt>fromIndex==toIndex</tt>, the
		''' range to be filled is empty.)
		''' </summary>
		''' <param name="a"> the array to be filled </param>
		''' <param name="fromIndex"> the index of the first element (inclusive) to be
		'''        filled with the specified value </param>
		''' <param name="toIndex"> the index of the last element (exclusive) to be
		'''        filled with the specified value </param>
		''' <param name="val"> the value to be stored in all elements of the array </param>
		''' <exception cref="IllegalArgumentException"> if <tt>fromIndex &gt; toIndex</tt> </exception>
		''' <exception cref="ArrayIndexOutOfBoundsException"> if <tt>fromIndex &lt; 0</tt> or
		'''         <tt>toIndex &gt; a.length</tt> </exception>
		Public Shared Sub fill(  a As Double(),   fromIndex As Integer,   toIndex As Integer,   val As Double)
			rangeCheck(a.Length, fromIndex, toIndex)
			For i As Integer = fromIndex To toIndex - 1
				a(i) = val
			Next i
		End Sub

		''' <summary>
		''' Assigns the specified float value to each element of the specified array
		''' of floats.
		''' </summary>
		''' <param name="a"> the array to be filled </param>
		''' <param name="val"> the value to be stored in all elements of the array </param>
		Public Shared Sub fill(  a As Single(),   val As Single)
			Dim i As Integer = 0
			Dim len As Integer = a.Length
			Do While i < len
				a(i) = val
				i += 1
			Loop
		End Sub

		''' <summary>
		''' Assigns the specified float value to each element of the specified
		''' range of the specified array of floats.  The range to be filled
		''' extends from index <tt>fromIndex</tt>, inclusive, to index
		''' <tt>toIndex</tt>, exclusive.  (If <tt>fromIndex==toIndex</tt>, the
		''' range to be filled is empty.)
		''' </summary>
		''' <param name="a"> the array to be filled </param>
		''' <param name="fromIndex"> the index of the first element (inclusive) to be
		'''        filled with the specified value </param>
		''' <param name="toIndex"> the index of the last element (exclusive) to be
		'''        filled with the specified value </param>
		''' <param name="val"> the value to be stored in all elements of the array </param>
		''' <exception cref="IllegalArgumentException"> if <tt>fromIndex &gt; toIndex</tt> </exception>
		''' <exception cref="ArrayIndexOutOfBoundsException"> if <tt>fromIndex &lt; 0</tt> or
		'''         <tt>toIndex &gt; a.length</tt> </exception>
		Public Shared Sub fill(  a As Single(),   fromIndex As Integer,   toIndex As Integer,   val As Single)
			rangeCheck(a.Length, fromIndex, toIndex)
			For i As Integer = fromIndex To toIndex - 1
				a(i) = val
			Next i
		End Sub

		''' <summary>
		''' Assigns the specified Object reference to each element of the specified
		''' array of Objects.
		''' </summary>
		''' <param name="a"> the array to be filled </param>
		''' <param name="val"> the value to be stored in all elements of the array </param>
		''' <exception cref="ArrayStoreException"> if the specified value is not of a
		'''         runtime type that can be stored in the specified array </exception>
		Public Shared Sub fill(  a As Object(),   val As Object)
			Dim i As Integer = 0
			Dim len As Integer = a.Length
			Do While i < len
				a(i) = val
				i += 1
			Loop
		End Sub

		''' <summary>
		''' Assigns the specified Object reference to each element of the specified
		''' range of the specified array of Objects.  The range to be filled
		''' extends from index <tt>fromIndex</tt>, inclusive, to index
		''' <tt>toIndex</tt>, exclusive.  (If <tt>fromIndex==toIndex</tt>, the
		''' range to be filled is empty.)
		''' </summary>
		''' <param name="a"> the array to be filled </param>
		''' <param name="fromIndex"> the index of the first element (inclusive) to be
		'''        filled with the specified value </param>
		''' <param name="toIndex"> the index of the last element (exclusive) to be
		'''        filled with the specified value </param>
		''' <param name="val"> the value to be stored in all elements of the array </param>
		''' <exception cref="IllegalArgumentException"> if <tt>fromIndex &gt; toIndex</tt> </exception>
		''' <exception cref="ArrayIndexOutOfBoundsException"> if <tt>fromIndex &lt; 0</tt> or
		'''         <tt>toIndex &gt; a.length</tt> </exception>
		''' <exception cref="ArrayStoreException"> if the specified value is not of a
		'''         runtime type that can be stored in the specified array </exception>
		Public Shared Sub fill(  a As Object(),   fromIndex As Integer,   toIndex As Integer,   val As Object)
			rangeCheck(a.Length, fromIndex, toIndex)
			For i As Integer = fromIndex To toIndex - 1
				a(i) = val
			Next i
		End Sub

		' Cloning

		''' <summary>
		''' Copies the specified array, truncating or padding with nulls (if necessary)
		''' so the copy has the specified length.  For all indices that are
		''' valid in both the original array and the copy, the two arrays will
		''' contain identical values.  For any indices that are valid in the
		''' copy but not the original, the copy will contain <tt>null</tt>.
		''' Such indices will exist if and only if the specified length
		''' is greater than that of the original array.
		''' The resulting array is of exactly the same class as the original array.
		''' </summary>
		''' @param <T> the class of the objects in the array </param>
		''' <param name="original"> the array to be copied </param>
		''' <param name="newLength"> the length of the copy to be returned </param>
		''' <returns> a copy of the original array, truncated or padded with nulls
		'''     to obtain the specified length </returns>
		''' <exception cref="NegativeArraySizeException"> if <tt>newLength</tt> is negative </exception>
		''' <exception cref="NullPointerException"> if <tt>original</tt> is null
		''' @since 1.6 </exception>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
		Public Shared Function copyOf(Of T)(  original As T(),   newLength As Integer) As T()
			Return CType(copyOf(original, newLength, original.GetType()), T())
		End Function

		''' <summary>
		''' Copies the specified array, truncating or padding with nulls (if necessary)
		''' so the copy has the specified length.  For all indices that are
		''' valid in both the original array and the copy, the two arrays will
		''' contain identical values.  For any indices that are valid in the
		''' copy but not the original, the copy will contain <tt>null</tt>.
		''' Such indices will exist if and only if the specified length
		''' is greater than that of the original array.
		''' The resulting array is of the class <tt>newType</tt>.
		''' </summary>
		''' @param <U> the class of the objects in the original array </param>
		''' @param <T> the class of the objects in the returned array </param>
		''' <param name="original"> the array to be copied </param>
		''' <param name="newLength"> the length of the copy to be returned </param>
		''' <param name="newType"> the class of the copy to be returned </param>
		''' <returns> a copy of the original array, truncated or padded with nulls
		'''     to obtain the specified length </returns>
		''' <exception cref="NegativeArraySizeException"> if <tt>newLength</tt> is negative </exception>
		''' <exception cref="NullPointerException"> if <tt>original</tt> is null </exception>
		''' <exception cref="ArrayStoreException"> if an element copied from
		'''     <tt>original</tt> is not of a runtime type that can be stored in
		'''     an array of class <tt>newType</tt>
		''' @since 1.6 </exception>
		Public Shared Function copyOf(Of T, U)(  original As U(),   newLength As Integer,   newType As [Class]) As T()
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
			Dim copy As T() = If(CObj(newType) Is GetType(CObj(Object())), CType(New Object(newLength - 1){}, T()), CType(Array.newInstance(newType.componentType, newLength), T()))
			Array.Copy(original, 0, copy, 0, System.Math.Min(original.Length, newLength))
			Return copy
		End Function

		''' <summary>
		''' Copies the specified array, truncating or padding with zeros (if necessary)
		''' so the copy has the specified length.  For all indices that are
		''' valid in both the original array and the copy, the two arrays will
		''' contain identical values.  For any indices that are valid in the
		''' copy but not the original, the copy will contain <tt>(byte)0</tt>.
		''' Such indices will exist if and only if the specified length
		''' is greater than that of the original array.
		''' </summary>
		''' <param name="original"> the array to be copied </param>
		''' <param name="newLength"> the length of the copy to be returned </param>
		''' <returns> a copy of the original array, truncated or padded with zeros
		'''     to obtain the specified length </returns>
		''' <exception cref="NegativeArraySizeException"> if <tt>newLength</tt> is negative </exception>
		''' <exception cref="NullPointerException"> if <tt>original</tt> is null
		''' @since 1.6 </exception>
		Public Shared Function copyOf(  original As SByte(),   newLength As Integer) As SByte()
			Dim copy As SByte() = New SByte(newLength - 1){}
			Array.Copy(original, 0, copy, 0, System.Math.Min(original.Length, newLength))
			Return copy
		End Function

		''' <summary>
		''' Copies the specified array, truncating or padding with zeros (if necessary)
		''' so the copy has the specified length.  For all indices that are
		''' valid in both the original array and the copy, the two arrays will
		''' contain identical values.  For any indices that are valid in the
		''' copy but not the original, the copy will contain <tt>(short)0</tt>.
		''' Such indices will exist if and only if the specified length
		''' is greater than that of the original array.
		''' </summary>
		''' <param name="original"> the array to be copied </param>
		''' <param name="newLength"> the length of the copy to be returned </param>
		''' <returns> a copy of the original array, truncated or padded with zeros
		'''     to obtain the specified length </returns>
		''' <exception cref="NegativeArraySizeException"> if <tt>newLength</tt> is negative </exception>
		''' <exception cref="NullPointerException"> if <tt>original</tt> is null
		''' @since 1.6 </exception>
		Public Shared Function copyOf(  original As Short(),   newLength As Integer) As Short()
			Dim copy As Short() = New Short(newLength - 1){}
			Array.Copy(original, 0, copy, 0, System.Math.Min(original.Length, newLength))
			Return copy
		End Function

		''' <summary>
		''' Copies the specified array, truncating or padding with zeros (if necessary)
		''' so the copy has the specified length.  For all indices that are
		''' valid in both the original array and the copy, the two arrays will
		''' contain identical values.  For any indices that are valid in the
		''' copy but not the original, the copy will contain <tt>0</tt>.
		''' Such indices will exist if and only if the specified length
		''' is greater than that of the original array.
		''' </summary>
		''' <param name="original"> the array to be copied </param>
		''' <param name="newLength"> the length of the copy to be returned </param>
		''' <returns> a copy of the original array, truncated or padded with zeros
		'''     to obtain the specified length </returns>
		''' <exception cref="NegativeArraySizeException"> if <tt>newLength</tt> is negative </exception>
		''' <exception cref="NullPointerException"> if <tt>original</tt> is null
		''' @since 1.6 </exception>
		Public Shared Function copyOf(  original As Integer(),   newLength As Integer) As Integer()
			Dim copy As Integer() = New Integer(newLength - 1){}
			Array.Copy(original, 0, copy, 0, System.Math.Min(original.Length, newLength))
			Return copy
		End Function

		''' <summary>
		''' Copies the specified array, truncating or padding with zeros (if necessary)
		''' so the copy has the specified length.  For all indices that are
		''' valid in both the original array and the copy, the two arrays will
		''' contain identical values.  For any indices that are valid in the
		''' copy but not the original, the copy will contain <tt>0L</tt>.
		''' Such indices will exist if and only if the specified length
		''' is greater than that of the original array.
		''' </summary>
		''' <param name="original"> the array to be copied </param>
		''' <param name="newLength"> the length of the copy to be returned </param>
		''' <returns> a copy of the original array, truncated or padded with zeros
		'''     to obtain the specified length </returns>
		''' <exception cref="NegativeArraySizeException"> if <tt>newLength</tt> is negative </exception>
		''' <exception cref="NullPointerException"> if <tt>original</tt> is null
		''' @since 1.6 </exception>
		Public Shared Function copyOf(  original As Long(),   newLength As Integer) As Long()
			Dim copy As Long() = New Long(newLength - 1){}
			Array.Copy(original, 0, copy, 0, System.Math.Min(original.Length, newLength))
			Return copy
		End Function

		''' <summary>
		''' Copies the specified array, truncating or padding with null characters (if necessary)
		''' so the copy has the specified length.  For all indices that are valid
		''' in both the original array and the copy, the two arrays will contain
		''' identical values.  For any indices that are valid in the copy but not
		''' the original, the copy will contain <tt>'\\u000'</tt>.  Such indices
		''' will exist if and only if the specified length is greater than that of
		''' the original array.
		''' </summary>
		''' <param name="original"> the array to be copied </param>
		''' <param name="newLength"> the length of the copy to be returned </param>
		''' <returns> a copy of the original array, truncated or padded with null characters
		'''     to obtain the specified length </returns>
		''' <exception cref="NegativeArraySizeException"> if <tt>newLength</tt> is negative </exception>
		''' <exception cref="NullPointerException"> if <tt>original</tt> is null
		''' @since 1.6 </exception>
		Public Shared Function copyOf(  original As Char(),   newLength As Integer) As Char()
			Dim copy As Char() = New Char(newLength - 1){}
			Array.Copy(original, 0, copy, 0, System.Math.Min(original.Length, newLength))
			Return copy
		End Function

		''' <summary>
		''' Copies the specified array, truncating or padding with zeros (if necessary)
		''' so the copy has the specified length.  For all indices that are
		''' valid in both the original array and the copy, the two arrays will
		''' contain identical values.  For any indices that are valid in the
		''' copy but not the original, the copy will contain <tt>0f</tt>.
		''' Such indices will exist if and only if the specified length
		''' is greater than that of the original array.
		''' </summary>
		''' <param name="original"> the array to be copied </param>
		''' <param name="newLength"> the length of the copy to be returned </param>
		''' <returns> a copy of the original array, truncated or padded with zeros
		'''     to obtain the specified length </returns>
		''' <exception cref="NegativeArraySizeException"> if <tt>newLength</tt> is negative </exception>
		''' <exception cref="NullPointerException"> if <tt>original</tt> is null
		''' @since 1.6 </exception>
		Public Shared Function copyOf(  original As Single(),   newLength As Integer) As Single()
			Dim copy As Single() = New Single(newLength - 1){}
			Array.Copy(original, 0, copy, 0, System.Math.Min(original.Length, newLength))
			Return copy
		End Function

		''' <summary>
		''' Copies the specified array, truncating or padding with zeros (if necessary)
		''' so the copy has the specified length.  For all indices that are
		''' valid in both the original array and the copy, the two arrays will
		''' contain identical values.  For any indices that are valid in the
		''' copy but not the original, the copy will contain <tt>0d</tt>.
		''' Such indices will exist if and only if the specified length
		''' is greater than that of the original array.
		''' </summary>
		''' <param name="original"> the array to be copied </param>
		''' <param name="newLength"> the length of the copy to be returned </param>
		''' <returns> a copy of the original array, truncated or padded with zeros
		'''     to obtain the specified length </returns>
		''' <exception cref="NegativeArraySizeException"> if <tt>newLength</tt> is negative </exception>
		''' <exception cref="NullPointerException"> if <tt>original</tt> is null
		''' @since 1.6 </exception>
		Public Shared Function copyOf(  original As Double(),   newLength As Integer) As Double()
			Dim copy As Double() = New Double(newLength - 1){}
			Array.Copy(original, 0, copy, 0, System.Math.Min(original.Length, newLength))
			Return copy
		End Function

		''' <summary>
		''' Copies the specified array, truncating or padding with <tt>false</tt> (if necessary)
		''' so the copy has the specified length.  For all indices that are
		''' valid in both the original array and the copy, the two arrays will
		''' contain identical values.  For any indices that are valid in the
		''' copy but not the original, the copy will contain <tt>false</tt>.
		''' Such indices will exist if and only if the specified length
		''' is greater than that of the original array.
		''' </summary>
		''' <param name="original"> the array to be copied </param>
		''' <param name="newLength"> the length of the copy to be returned </param>
		''' <returns> a copy of the original array, truncated or padded with false elements
		'''     to obtain the specified length </returns>
		''' <exception cref="NegativeArraySizeException"> if <tt>newLength</tt> is negative </exception>
		''' <exception cref="NullPointerException"> if <tt>original</tt> is null
		''' @since 1.6 </exception>
		Public Shared Function copyOf(  original As Boolean(),   newLength As Integer) As Boolean()
			Dim copy As Boolean() = New Boolean(newLength - 1){}
			Array.Copy(original, 0, copy, 0, System.Math.Min(original.Length, newLength))
			Return copy
		End Function

		''' <summary>
		''' Copies the specified range of the specified array into a new array.
		''' The initial index of the range (<tt>from</tt>) must lie between zero
		''' and <tt>original.length</tt>, inclusive.  The value at
		''' <tt>original[from]</tt> is placed into the initial element of the copy
		''' (unless <tt>from == original.length</tt> or <tt>from == to</tt>).
		''' Values from subsequent elements in the original array are placed into
		''' subsequent elements in the copy.  The final index of the range
		''' (<tt>to</tt>), which must be greater than or equal to <tt>from</tt>,
		''' may be greater than <tt>original.length</tt>, in which case
		''' <tt>null</tt> is placed in all elements of the copy whose index is
		''' greater than or equal to <tt>original.length - from</tt>.  The length
		''' of the returned array will be <tt>to - from</tt>.
		''' <p>
		''' The resulting array is of exactly the same class as the original array.
		''' </summary>
		''' @param <T> the class of the objects in the array </param>
		''' <param name="original"> the array from which a range is to be copied </param>
		''' <param name="from"> the initial index of the range to be copied, inclusive </param>
		''' <param name="to"> the final index of the range to be copied, exclusive.
		'''     (This index may lie outside the array.) </param>
		''' <returns> a new array containing the specified range from the original array,
		'''     truncated or padded with nulls to obtain the required length </returns>
		''' <exception cref="ArrayIndexOutOfBoundsException"> if {@code from < 0}
		'''     or {@code from > original.length} </exception>
		''' <exception cref="IllegalArgumentException"> if <tt>from &gt; to</tt> </exception>
		''' <exception cref="NullPointerException"> if <tt>original</tt> is null
		''' @since 1.6 </exception>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
		Public Shared Function copyOfRange(Of T)(  original As T(),   [from] As Integer,   [to] As Integer) As T()
			Return copyOfRange(original, [from], [to], CType(original.GetType(), [Class]))
		End Function

		''' <summary>
		''' Copies the specified range of the specified array into a new array.
		''' The initial index of the range (<tt>from</tt>) must lie between zero
		''' and <tt>original.length</tt>, inclusive.  The value at
		''' <tt>original[from]</tt> is placed into the initial element of the copy
		''' (unless <tt>from == original.length</tt> or <tt>from == to</tt>).
		''' Values from subsequent elements in the original array are placed into
		''' subsequent elements in the copy.  The final index of the range
		''' (<tt>to</tt>), which must be greater than or equal to <tt>from</tt>,
		''' may be greater than <tt>original.length</tt>, in which case
		''' <tt>null</tt> is placed in all elements of the copy whose index is
		''' greater than or equal to <tt>original.length - from</tt>.  The length
		''' of the returned array will be <tt>to - from</tt>.
		''' The resulting array is of the class <tt>newType</tt>.
		''' </summary>
		''' @param <U> the class of the objects in the original array </param>
		''' @param <T> the class of the objects in the returned array </param>
		''' <param name="original"> the array from which a range is to be copied </param>
		''' <param name="from"> the initial index of the range to be copied, inclusive </param>
		''' <param name="to"> the final index of the range to be copied, exclusive.
		'''     (This index may lie outside the array.) </param>
		''' <param name="newType"> the class of the copy to be returned </param>
		''' <returns> a new array containing the specified range from the original array,
		'''     truncated or padded with nulls to obtain the required length </returns>
		''' <exception cref="ArrayIndexOutOfBoundsException"> if {@code from < 0}
		'''     or {@code from > original.length} </exception>
		''' <exception cref="IllegalArgumentException"> if <tt>from &gt; to</tt> </exception>
		''' <exception cref="NullPointerException"> if <tt>original</tt> is null </exception>
		''' <exception cref="ArrayStoreException"> if an element copied from
		'''     <tt>original</tt> is not of a runtime type that can be stored in
		'''     an array of class <tt>newType</tt>.
		''' @since 1.6 </exception>
		Public Shared Function copyOfRange(Of T, U)(  original As U(),   [from] As Integer,   [to] As Integer,   newType As [Class]) As T()
			Dim newLength As Integer = [to] - [from]
			If newLength < 0 Then Throw New IllegalArgumentException([from] & " > " & [to])
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
			Dim copy As T() = If(CObj(newType) Is GetType(CObj(Object())), CType(New Object(newLength - 1){}, T()), CType(Array.newInstance(newType.componentType, newLength), T()))
			Array.Copy(original, [from], copy, 0, System.Math.Min(original.Length - [from], newLength))
			Return copy
		End Function

		''' <summary>
		''' Copies the specified range of the specified array into a new array.
		''' The initial index of the range (<tt>from</tt>) must lie between zero
		''' and <tt>original.length</tt>, inclusive.  The value at
		''' <tt>original[from]</tt> is placed into the initial element of the copy
		''' (unless <tt>from == original.length</tt> or <tt>from == to</tt>).
		''' Values from subsequent elements in the original array are placed into
		''' subsequent elements in the copy.  The final index of the range
		''' (<tt>to</tt>), which must be greater than or equal to <tt>from</tt>,
		''' may be greater than <tt>original.length</tt>, in which case
		''' <tt>(byte)0</tt> is placed in all elements of the copy whose index is
		''' greater than or equal to <tt>original.length - from</tt>.  The length
		''' of the returned array will be <tt>to - from</tt>.
		''' </summary>
		''' <param name="original"> the array from which a range is to be copied </param>
		''' <param name="from"> the initial index of the range to be copied, inclusive </param>
		''' <param name="to"> the final index of the range to be copied, exclusive.
		'''     (This index may lie outside the array.) </param>
		''' <returns> a new array containing the specified range from the original array,
		'''     truncated or padded with zeros to obtain the required length </returns>
		''' <exception cref="ArrayIndexOutOfBoundsException"> if {@code from < 0}
		'''     or {@code from > original.length} </exception>
		''' <exception cref="IllegalArgumentException"> if <tt>from &gt; to</tt> </exception>
		''' <exception cref="NullPointerException"> if <tt>original</tt> is null
		''' @since 1.6 </exception>
		Public Shared Function copyOfRange(  original As SByte(),   [from] As Integer,   [to] As Integer) As SByte()
			Dim newLength As Integer = [to] - [from]
			If newLength < 0 Then Throw New IllegalArgumentException([from] & " > " & [to])
			Dim copy As SByte() = New SByte(newLength - 1){}
			Array.Copy(original, [from], copy, 0, System.Math.Min(original.Length - [from], newLength))
			Return copy
		End Function

		''' <summary>
		''' Copies the specified range of the specified array into a new array.
		''' The initial index of the range (<tt>from</tt>) must lie between zero
		''' and <tt>original.length</tt>, inclusive.  The value at
		''' <tt>original[from]</tt> is placed into the initial element of the copy
		''' (unless <tt>from == original.length</tt> or <tt>from == to</tt>).
		''' Values from subsequent elements in the original array are placed into
		''' subsequent elements in the copy.  The final index of the range
		''' (<tt>to</tt>), which must be greater than or equal to <tt>from</tt>,
		''' may be greater than <tt>original.length</tt>, in which case
		''' <tt>(short)0</tt> is placed in all elements of the copy whose index is
		''' greater than or equal to <tt>original.length - from</tt>.  The length
		''' of the returned array will be <tt>to - from</tt>.
		''' </summary>
		''' <param name="original"> the array from which a range is to be copied </param>
		''' <param name="from"> the initial index of the range to be copied, inclusive </param>
		''' <param name="to"> the final index of the range to be copied, exclusive.
		'''     (This index may lie outside the array.) </param>
		''' <returns> a new array containing the specified range from the original array,
		'''     truncated or padded with zeros to obtain the required length </returns>
		''' <exception cref="ArrayIndexOutOfBoundsException"> if {@code from < 0}
		'''     or {@code from > original.length} </exception>
		''' <exception cref="IllegalArgumentException"> if <tt>from &gt; to</tt> </exception>
		''' <exception cref="NullPointerException"> if <tt>original</tt> is null
		''' @since 1.6 </exception>
		Public Shared Function copyOfRange(  original As Short(),   [from] As Integer,   [to] As Integer) As Short()
			Dim newLength As Integer = [to] - [from]
			If newLength < 0 Then Throw New IllegalArgumentException([from] & " > " & [to])
			Dim copy As Short() = New Short(newLength - 1){}
			Array.Copy(original, [from], copy, 0, System.Math.Min(original.Length - [from], newLength))
			Return copy
		End Function

		''' <summary>
		''' Copies the specified range of the specified array into a new array.
		''' The initial index of the range (<tt>from</tt>) must lie between zero
		''' and <tt>original.length</tt>, inclusive.  The value at
		''' <tt>original[from]</tt> is placed into the initial element of the copy
		''' (unless <tt>from == original.length</tt> or <tt>from == to</tt>).
		''' Values from subsequent elements in the original array are placed into
		''' subsequent elements in the copy.  The final index of the range
		''' (<tt>to</tt>), which must be greater than or equal to <tt>from</tt>,
		''' may be greater than <tt>original.length</tt>, in which case
		''' <tt>0</tt> is placed in all elements of the copy whose index is
		''' greater than or equal to <tt>original.length - from</tt>.  The length
		''' of the returned array will be <tt>to - from</tt>.
		''' </summary>
		''' <param name="original"> the array from which a range is to be copied </param>
		''' <param name="from"> the initial index of the range to be copied, inclusive </param>
		''' <param name="to"> the final index of the range to be copied, exclusive.
		'''     (This index may lie outside the array.) </param>
		''' <returns> a new array containing the specified range from the original array,
		'''     truncated or padded with zeros to obtain the required length </returns>
		''' <exception cref="ArrayIndexOutOfBoundsException"> if {@code from < 0}
		'''     or {@code from > original.length} </exception>
		''' <exception cref="IllegalArgumentException"> if <tt>from &gt; to</tt> </exception>
		''' <exception cref="NullPointerException"> if <tt>original</tt> is null
		''' @since 1.6 </exception>
		Public Shared Function copyOfRange(  original As Integer(),   [from] As Integer,   [to] As Integer) As Integer()
			Dim newLength As Integer = [to] - [from]
			If newLength < 0 Then Throw New IllegalArgumentException([from] & " > " & [to])
			Dim copy As Integer() = New Integer(newLength - 1){}
			Array.Copy(original, [from], copy, 0, System.Math.Min(original.Length - [from], newLength))
			Return copy
		End Function

		''' <summary>
		''' Copies the specified range of the specified array into a new array.
		''' The initial index of the range (<tt>from</tt>) must lie between zero
		''' and <tt>original.length</tt>, inclusive.  The value at
		''' <tt>original[from]</tt> is placed into the initial element of the copy
		''' (unless <tt>from == original.length</tt> or <tt>from == to</tt>).
		''' Values from subsequent elements in the original array are placed into
		''' subsequent elements in the copy.  The final index of the range
		''' (<tt>to</tt>), which must be greater than or equal to <tt>from</tt>,
		''' may be greater than <tt>original.length</tt>, in which case
		''' <tt>0L</tt> is placed in all elements of the copy whose index is
		''' greater than or equal to <tt>original.length - from</tt>.  The length
		''' of the returned array will be <tt>to - from</tt>.
		''' </summary>
		''' <param name="original"> the array from which a range is to be copied </param>
		''' <param name="from"> the initial index of the range to be copied, inclusive </param>
		''' <param name="to"> the final index of the range to be copied, exclusive.
		'''     (This index may lie outside the array.) </param>
		''' <returns> a new array containing the specified range from the original array,
		'''     truncated or padded with zeros to obtain the required length </returns>
		''' <exception cref="ArrayIndexOutOfBoundsException"> if {@code from < 0}
		'''     or {@code from > original.length} </exception>
		''' <exception cref="IllegalArgumentException"> if <tt>from &gt; to</tt> </exception>
		''' <exception cref="NullPointerException"> if <tt>original</tt> is null
		''' @since 1.6 </exception>
		Public Shared Function copyOfRange(  original As Long(),   [from] As Integer,   [to] As Integer) As Long()
			Dim newLength As Integer = [to] - [from]
			If newLength < 0 Then Throw New IllegalArgumentException([from] & " > " & [to])
			Dim copy As Long() = New Long(newLength - 1){}
			Array.Copy(original, [from], copy, 0, System.Math.Min(original.Length - [from], newLength))
			Return copy
		End Function

		''' <summary>
		''' Copies the specified range of the specified array into a new array.
		''' The initial index of the range (<tt>from</tt>) must lie between zero
		''' and <tt>original.length</tt>, inclusive.  The value at
		''' <tt>original[from]</tt> is placed into the initial element of the copy
		''' (unless <tt>from == original.length</tt> or <tt>from == to</tt>).
		''' Values from subsequent elements in the original array are placed into
		''' subsequent elements in the copy.  The final index of the range
		''' (<tt>to</tt>), which must be greater than or equal to <tt>from</tt>,
		''' may be greater than <tt>original.length</tt>, in which case
		''' <tt>'\\u000'</tt> is placed in all elements of the copy whose index is
		''' greater than or equal to <tt>original.length - from</tt>.  The length
		''' of the returned array will be <tt>to - from</tt>.
		''' </summary>
		''' <param name="original"> the array from which a range is to be copied </param>
		''' <param name="from"> the initial index of the range to be copied, inclusive </param>
		''' <param name="to"> the final index of the range to be copied, exclusive.
		'''     (This index may lie outside the array.) </param>
		''' <returns> a new array containing the specified range from the original array,
		'''     truncated or padded with null characters to obtain the required length </returns>
		''' <exception cref="ArrayIndexOutOfBoundsException"> if {@code from < 0}
		'''     or {@code from > original.length} </exception>
		''' <exception cref="IllegalArgumentException"> if <tt>from &gt; to</tt> </exception>
		''' <exception cref="NullPointerException"> if <tt>original</tt> is null
		''' @since 1.6 </exception>
		Public Shared Function copyOfRange(  original As Char(),   [from] As Integer,   [to] As Integer) As Char()
			Dim newLength As Integer = [to] - [from]
			If newLength < 0 Then Throw New IllegalArgumentException([from] & " > " & [to])
			Dim copy As Char() = New Char(newLength - 1){}
			Array.Copy(original, [from], copy, 0, System.Math.Min(original.Length - [from], newLength))
			Return copy
		End Function

		''' <summary>
		''' Copies the specified range of the specified array into a new array.
		''' The initial index of the range (<tt>from</tt>) must lie between zero
		''' and <tt>original.length</tt>, inclusive.  The value at
		''' <tt>original[from]</tt> is placed into the initial element of the copy
		''' (unless <tt>from == original.length</tt> or <tt>from == to</tt>).
		''' Values from subsequent elements in the original array are placed into
		''' subsequent elements in the copy.  The final index of the range
		''' (<tt>to</tt>), which must be greater than or equal to <tt>from</tt>,
		''' may be greater than <tt>original.length</tt>, in which case
		''' <tt>0f</tt> is placed in all elements of the copy whose index is
		''' greater than or equal to <tt>original.length - from</tt>.  The length
		''' of the returned array will be <tt>to - from</tt>.
		''' </summary>
		''' <param name="original"> the array from which a range is to be copied </param>
		''' <param name="from"> the initial index of the range to be copied, inclusive </param>
		''' <param name="to"> the final index of the range to be copied, exclusive.
		'''     (This index may lie outside the array.) </param>
		''' <returns> a new array containing the specified range from the original array,
		'''     truncated or padded with zeros to obtain the required length </returns>
		''' <exception cref="ArrayIndexOutOfBoundsException"> if {@code from < 0}
		'''     or {@code from > original.length} </exception>
		''' <exception cref="IllegalArgumentException"> if <tt>from &gt; to</tt> </exception>
		''' <exception cref="NullPointerException"> if <tt>original</tt> is null
		''' @since 1.6 </exception>
		Public Shared Function copyOfRange(  original As Single(),   [from] As Integer,   [to] As Integer) As Single()
			Dim newLength As Integer = [to] - [from]
			If newLength < 0 Then Throw New IllegalArgumentException([from] & " > " & [to])
			Dim copy As Single() = New Single(newLength - 1){}
			Array.Copy(original, [from], copy, 0, System.Math.Min(original.Length - [from], newLength))
			Return copy
		End Function

		''' <summary>
		''' Copies the specified range of the specified array into a new array.
		''' The initial index of the range (<tt>from</tt>) must lie between zero
		''' and <tt>original.length</tt>, inclusive.  The value at
		''' <tt>original[from]</tt> is placed into the initial element of the copy
		''' (unless <tt>from == original.length</tt> or <tt>from == to</tt>).
		''' Values from subsequent elements in the original array are placed into
		''' subsequent elements in the copy.  The final index of the range
		''' (<tt>to</tt>), which must be greater than or equal to <tt>from</tt>,
		''' may be greater than <tt>original.length</tt>, in which case
		''' <tt>0d</tt> is placed in all elements of the copy whose index is
		''' greater than or equal to <tt>original.length - from</tt>.  The length
		''' of the returned array will be <tt>to - from</tt>.
		''' </summary>
		''' <param name="original"> the array from which a range is to be copied </param>
		''' <param name="from"> the initial index of the range to be copied, inclusive </param>
		''' <param name="to"> the final index of the range to be copied, exclusive.
		'''     (This index may lie outside the array.) </param>
		''' <returns> a new array containing the specified range from the original array,
		'''     truncated or padded with zeros to obtain the required length </returns>
		''' <exception cref="ArrayIndexOutOfBoundsException"> if {@code from < 0}
		'''     or {@code from > original.length} </exception>
		''' <exception cref="IllegalArgumentException"> if <tt>from &gt; to</tt> </exception>
		''' <exception cref="NullPointerException"> if <tt>original</tt> is null
		''' @since 1.6 </exception>
		Public Shared Function copyOfRange(  original As Double(),   [from] As Integer,   [to] As Integer) As Double()
			Dim newLength As Integer = [to] - [from]
			If newLength < 0 Then Throw New IllegalArgumentException([from] & " > " & [to])
			Dim copy As Double() = New Double(newLength - 1){}
			Array.Copy(original, [from], copy, 0, System.Math.Min(original.Length - [from], newLength))
			Return copy
		End Function

		''' <summary>
		''' Copies the specified range of the specified array into a new array.
		''' The initial index of the range (<tt>from</tt>) must lie between zero
		''' and <tt>original.length</tt>, inclusive.  The value at
		''' <tt>original[from]</tt> is placed into the initial element of the copy
		''' (unless <tt>from == original.length</tt> or <tt>from == to</tt>).
		''' Values from subsequent elements in the original array are placed into
		''' subsequent elements in the copy.  The final index of the range
		''' (<tt>to</tt>), which must be greater than or equal to <tt>from</tt>,
		''' may be greater than <tt>original.length</tt>, in which case
		''' <tt>false</tt> is placed in all elements of the copy whose index is
		''' greater than or equal to <tt>original.length - from</tt>.  The length
		''' of the returned array will be <tt>to - from</tt>.
		''' </summary>
		''' <param name="original"> the array from which a range is to be copied </param>
		''' <param name="from"> the initial index of the range to be copied, inclusive </param>
		''' <param name="to"> the final index of the range to be copied, exclusive.
		'''     (This index may lie outside the array.) </param>
		''' <returns> a new array containing the specified range from the original array,
		'''     truncated or padded with false elements to obtain the required length </returns>
		''' <exception cref="ArrayIndexOutOfBoundsException"> if {@code from < 0}
		'''     or {@code from > original.length} </exception>
		''' <exception cref="IllegalArgumentException"> if <tt>from &gt; to</tt> </exception>
		''' <exception cref="NullPointerException"> if <tt>original</tt> is null
		''' @since 1.6 </exception>
		Public Shared Function copyOfRange(  original As Boolean(),   [from] As Integer,   [to] As Integer) As Boolean()
			Dim newLength As Integer = [to] - [from]
			If newLength < 0 Then Throw New IllegalArgumentException([from] & " > " & [to])
			Dim copy As Boolean() = New Boolean(newLength - 1){}
			Array.Copy(original, [from], copy, 0, System.Math.Min(original.Length - [from], newLength))
			Return copy
		End Function

		' Misc

		''' <summary>
		''' Returns a fixed-size list backed by the specified array.  (Changes to
		''' the returned list "write through" to the array.)  This method acts
		''' as bridge between array-based and collection-based APIs, in
		''' combination with <seealso cref="Collection#toArray"/>.  The returned list is
		''' serializable and implements <seealso cref="RandomAccess"/>.
		''' 
		''' <p>This method also provides a convenient way to create a fixed-size
		''' list initialized to contain several elements:
		''' <pre>
		'''     List&lt;String&gt; stooges = Arrays.asList("Larry", "Moe", "Curly");
		''' </pre>
		''' </summary>
		''' @param <T> the class of the objects in the array </param>
		''' <param name="a"> the array by which the list will be backed </param>
		''' <returns> a list view of the specified array </returns>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
		Public Shared Function asList(Of T)(ParamArray   a As T()) As List(Of T)
			Return New List(Of )(a)
		End Function

		''' <summary>
		''' @serial include
		''' </summary>
		<Serializable> _
		Private Class List(Of E)
			Inherits AbstractList(Of E)
			Implements RandomAccess

			Private Const serialVersionUID As Long = -2764017481108945198L
			Private ReadOnly a As E()

			Friend Sub New(  array As E())
				a = Objects.requireNonNull(array)
			End Sub

			Public Overrides Function size() As Integer
				Return a.Length
			End Function

			Public Overrides Function toArray() As Object()
				Return a.clone()
			End Function

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
			Public Overrides Function toArray(Of T)(  a As T()) As T()
				Dim size As Integer = size()
				If a.Length < size Then Return Arrays.copyOf(Me.a, size, CType(a.GetType(), [Class]))
				Array.Copy(Me.a, 0, a, 0, size)
				If a.Length > size Then a(size) = Nothing
				Return a
			End Function

			Public Overrides Function [get](  index As Integer) As E
				Return a(index)
			End Function

			Public Overrides Function [set](  index As Integer,   element As E) As E
				Dim oldValue As E = a(index)
				a(index) = element
				Return oldValue
			End Function

			Public Overrides Function indexOf(  o As Object) As Integer
				Dim a As E() = Me.a
				If o Is Nothing Then
					For i As Integer = 0 To a.Length - 1
						If a(i) Is Nothing Then Return i
					Next i
				Else
					For i As Integer = 0 To a.Length - 1
						If o.Equals(a(i)) Then Return i
					Next i
				End If
				Return -1
			End Function

			Public Overrides Function contains(  o As Object) As Boolean
				Return IndexOf(o) <> -1
			End Function

			Public Overrides Function spliterator() As Spliterator(Of E)
				Return Spliterators.spliterator(a, Spliterator.ORDERED)
			End Function

'JAVA TO VB CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
			Public Overrides Sub forEach(Of T1)(  action As java.util.function.Consumer(Of T1))
				Objects.requireNonNull(action)
				For Each e As E In a
					action.accept(e)
				Next e
			End Sub

			Public Overrides Sub replaceAll(  [operator] As java.util.function.UnaryOperator(Of E))
				Objects.requireNonNull([operator])
				Dim a As E() = Me.a
				For i As Integer = 0 To a.Length - 1
					a(i) = [operator].apply(a(i))
				Next i
			End Sub

'JAVA TO VB CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
			Public Overrides Sub sort(Of T1)(  c As Comparator(Of T1))
				Array.Sort(a, c)
			End Sub
		End Class

		''' <summary>
		''' Returns a hash code based on the contents of the specified array.
		''' For any two <tt>long</tt> arrays <tt>a</tt> and <tt>b</tt>
		''' such that <tt>Arrays.equals(a, b)</tt>, it is also the case that
		''' <tt>Arrays.hashCode(a) == Arrays.hashCode(b)</tt>.
		''' 
		''' <p>The value returned by this method is the same value that would be
		''' obtained by invoking the <seealso cref="List#hashCode() <tt>hashCode</tt>"/>
		''' method on a <seealso cref="List"/> containing a sequence of <seealso cref="Long"/>
		''' instances representing the elements of <tt>a</tt> in the same order.
		''' If <tt>a</tt> is <tt>null</tt>, this method returns 0.
		''' </summary>
		''' <param name="a"> the array whose hash value to compute </param>
		''' <returns> a content-based hash code for <tt>a</tt>
		''' @since 1.5 </returns>
		Public Shared Function GetHashCode(  a As Long()) As Integer
			If a Is Nothing Then Return 0

			Dim result As Integer = 1
			For Each element As Long In a
				Dim elementHash As Integer = CInt(Fix(element Xor (CLng(CULng(element) >> 32))))
				result = 31 * result + elementHash
			Next element

			Return result
		End Function

		''' <summary>
		''' Returns a hash code based on the contents of the specified array.
		''' For any two non-null <tt>int</tt> arrays <tt>a</tt> and <tt>b</tt>
		''' such that <tt>Arrays.equals(a, b)</tt>, it is also the case that
		''' <tt>Arrays.hashCode(a) == Arrays.hashCode(b)</tt>.
		''' 
		''' <p>The value returned by this method is the same value that would be
		''' obtained by invoking the <seealso cref="List#hashCode() <tt>hashCode</tt>"/>
		''' method on a <seealso cref="List"/> containing a sequence of <seealso cref="Integer"/>
		''' instances representing the elements of <tt>a</tt> in the same order.
		''' If <tt>a</tt> is <tt>null</tt>, this method returns 0.
		''' </summary>
		''' <param name="a"> the array whose hash value to compute </param>
		''' <returns> a content-based hash code for <tt>a</tt>
		''' @since 1.5 </returns>
		Public Shared Function GetHashCode(  a As Integer()) As Integer
			If a Is Nothing Then Return 0

			Dim result As Integer = 1
			For Each element As Integer In a
				result = 31 * result + element
			Next element

			Return result
		End Function

		''' <summary>
		''' Returns a hash code based on the contents of the specified array.
		''' For any two <tt>short</tt> arrays <tt>a</tt> and <tt>b</tt>
		''' such that <tt>Arrays.equals(a, b)</tt>, it is also the case that
		''' <tt>Arrays.hashCode(a) == Arrays.hashCode(b)</tt>.
		''' 
		''' <p>The value returned by this method is the same value that would be
		''' obtained by invoking the <seealso cref="List#hashCode() <tt>hashCode</tt>"/>
		''' method on a <seealso cref="List"/> containing a sequence of <seealso cref="Short"/>
		''' instances representing the elements of <tt>a</tt> in the same order.
		''' If <tt>a</tt> is <tt>null</tt>, this method returns 0.
		''' </summary>
		''' <param name="a"> the array whose hash value to compute </param>
		''' <returns> a content-based hash code for <tt>a</tt>
		''' @since 1.5 </returns>
		Public Shared Function GetHashCode(  a As Short()) As Integer
			If a Is Nothing Then Return 0

			Dim result As Integer = 1
			For Each element As Short In a
				result = 31 * result + element
			Next element

			Return result
		End Function

		''' <summary>
		''' Returns a hash code based on the contents of the specified array.
		''' For any two <tt>char</tt> arrays <tt>a</tt> and <tt>b</tt>
		''' such that <tt>Arrays.equals(a, b)</tt>, it is also the case that
		''' <tt>Arrays.hashCode(a) == Arrays.hashCode(b)</tt>.
		''' 
		''' <p>The value returned by this method is the same value that would be
		''' obtained by invoking the <seealso cref="List#hashCode() <tt>hashCode</tt>"/>
		''' method on a <seealso cref="List"/> containing a sequence of <seealso cref="Character"/>
		''' instances representing the elements of <tt>a</tt> in the same order.
		''' If <tt>a</tt> is <tt>null</tt>, this method returns 0.
		''' </summary>
		''' <param name="a"> the array whose hash value to compute </param>
		''' <returns> a content-based hash code for <tt>a</tt>
		''' @since 1.5 </returns>
		Public Shared Function GetHashCode(  a As Char()) As Integer
			If a Is Nothing Then Return 0

			Dim result As Integer = 1
			For Each element As Char In a
				result = 31 * result + AscW(element)
			Next element

			Return result
		End Function

		''' <summary>
		''' Returns a hash code based on the contents of the specified array.
		''' For any two <tt>byte</tt> arrays <tt>a</tt> and <tt>b</tt>
		''' such that <tt>Arrays.equals(a, b)</tt>, it is also the case that
		''' <tt>Arrays.hashCode(a) == Arrays.hashCode(b)</tt>.
		''' 
		''' <p>The value returned by this method is the same value that would be
		''' obtained by invoking the <seealso cref="List#hashCode() <tt>hashCode</tt>"/>
		''' method on a <seealso cref="List"/> containing a sequence of <seealso cref="Byte"/>
		''' instances representing the elements of <tt>a</tt> in the same order.
		''' If <tt>a</tt> is <tt>null</tt>, this method returns 0.
		''' </summary>
		''' <param name="a"> the array whose hash value to compute </param>
		''' <returns> a content-based hash code for <tt>a</tt>
		''' @since 1.5 </returns>
		Public Shared Function GetHashCode(  a As SByte()) As Integer
			If a Is Nothing Then Return 0

			Dim result As Integer = 1
			For Each element As SByte In a
				result = 31 * result + element
			Next element

			Return result
		End Function

		''' <summary>
		''' Returns a hash code based on the contents of the specified array.
		''' For any two <tt>boolean</tt> arrays <tt>a</tt> and <tt>b</tt>
		''' such that <tt>Arrays.equals(a, b)</tt>, it is also the case that
		''' <tt>Arrays.hashCode(a) == Arrays.hashCode(b)</tt>.
		''' 
		''' <p>The value returned by this method is the same value that would be
		''' obtained by invoking the <seealso cref="List#hashCode() <tt>hashCode</tt>"/>
		''' method on a <seealso cref="List"/> containing a sequence of <seealso cref="Boolean"/>
		''' instances representing the elements of <tt>a</tt> in the same order.
		''' If <tt>a</tt> is <tt>null</tt>, this method returns 0.
		''' </summary>
		''' <param name="a"> the array whose hash value to compute </param>
		''' <returns> a content-based hash code for <tt>a</tt>
		''' @since 1.5 </returns>
		Public Shared Function GetHashCode(  a As Boolean()) As Integer
			If a Is Nothing Then Return 0

			Dim result As Integer = 1
			For Each element As Boolean In a
				result = 31 * result + (If(element, 1231, 1237))
			Next element

			Return result
		End Function

		''' <summary>
		''' Returns a hash code based on the contents of the specified array.
		''' For any two <tt>float</tt> arrays <tt>a</tt> and <tt>b</tt>
		''' such that <tt>Arrays.equals(a, b)</tt>, it is also the case that
		''' <tt>Arrays.hashCode(a) == Arrays.hashCode(b)</tt>.
		''' 
		''' <p>The value returned by this method is the same value that would be
		''' obtained by invoking the <seealso cref="List#hashCode() <tt>hashCode</tt>"/>
		''' method on a <seealso cref="List"/> containing a sequence of <seealso cref="Float"/>
		''' instances representing the elements of <tt>a</tt> in the same order.
		''' If <tt>a</tt> is <tt>null</tt>, this method returns 0.
		''' </summary>
		''' <param name="a"> the array whose hash value to compute </param>
		''' <returns> a content-based hash code for <tt>a</tt>
		''' @since 1.5 </returns>
		Public Shared Function GetHashCode(  a As Single()) As Integer
			If a Is Nothing Then Return 0

			Dim result As Integer = 1
			For Each element As Single In a
				result = 31 * result + Float.floatToIntBits(element)
			Next element

			Return result
		End Function

		''' <summary>
		''' Returns a hash code based on the contents of the specified array.
		''' For any two <tt>double</tt> arrays <tt>a</tt> and <tt>b</tt>
		''' such that <tt>Arrays.equals(a, b)</tt>, it is also the case that
		''' <tt>Arrays.hashCode(a) == Arrays.hashCode(b)</tt>.
		''' 
		''' <p>The value returned by this method is the same value that would be
		''' obtained by invoking the <seealso cref="List#hashCode() <tt>hashCode</tt>"/>
		''' method on a <seealso cref="List"/> containing a sequence of <seealso cref="Double"/>
		''' instances representing the elements of <tt>a</tt> in the same order.
		''' If <tt>a</tt> is <tt>null</tt>, this method returns 0.
		''' </summary>
		''' <param name="a"> the array whose hash value to compute </param>
		''' <returns> a content-based hash code for <tt>a</tt>
		''' @since 1.5 </returns>
		Public Shared Function GetHashCode(  a As Double()) As Integer
			If a Is Nothing Then Return 0

			Dim result As Integer = 1
			For Each element As Double In a
				Dim bits As Long = java.lang.[Double].doubleToLongBits(element)
				result = 31 * result + CInt(Fix(bits Xor (CLng(CULng(bits) >> 32))))
			Next element
			Return result
		End Function

		''' <summary>
		''' Returns a hash code based on the contents of the specified array.  If
		''' the array contains other arrays as elements, the hash code is based on
		''' their identities rather than their contents.  It is therefore
		''' acceptable to invoke this method on an array that contains itself as an
		''' element,  either directly or indirectly through one or more levels of
		''' arrays.
		''' 
		''' <p>For any two arrays <tt>a</tt> and <tt>b</tt> such that
		''' <tt>Arrays.equals(a, b)</tt>, it is also the case that
		''' <tt>Arrays.hashCode(a) == Arrays.hashCode(b)</tt>.
		''' 
		''' <p>The value returned by this method is equal to the value that would
		''' be returned by <tt>Arrays.asList(a).hashCode()</tt>, unless <tt>a</tt>
		''' is <tt>null</tt>, in which case <tt>0</tt> is returned.
		''' </summary>
		''' <param name="a"> the array whose content-based hash code to compute </param>
		''' <returns> a content-based hash code for <tt>a</tt> </returns>
		''' <seealso cref= #deepHashCode(Object[])
		''' @since 1.5 </seealso>
		Public Shared Function GetHashCode(  a As Object()) As Integer
			If a Is Nothing Then Return 0

			Dim result As Integer = 1

			For Each element As Object In a
				result = 31 * result + (If(element Is Nothing, 0, element.GetHashCode()))
			Next element

			Return result
		End Function

		''' <summary>
		''' Returns a hash code based on the "deep contents" of the specified
		''' array.  If the array contains other arrays as elements, the
		''' hash code is based on their contents and so on, ad infinitum.
		''' It is therefore unacceptable to invoke this method on an array that
		''' contains itself as an element, either directly or indirectly through
		''' one or more levels of arrays.  The behavior of such an invocation is
		''' undefined.
		''' 
		''' <p>For any two arrays <tt>a</tt> and <tt>b</tt> such that
		''' <tt>Arrays.deepEquals(a, b)</tt>, it is also the case that
		''' <tt>Arrays.deepHashCode(a) == Arrays.deepHashCode(b)</tt>.
		''' 
		''' <p>The computation of the value returned by this method is similar to
		''' that of the value returned by <seealso cref="List#hashCode()"/> on a list
		''' containing the same elements as <tt>a</tt> in the same order, with one
		''' difference: If an element <tt>e</tt> of <tt>a</tt> is itself an array,
		''' its hash code is computed not by calling <tt>e.hashCode()</tt>, but as
		''' by calling the appropriate overloading of <tt>Arrays.hashCode(e)</tt>
		''' if <tt>e</tt> is an array of a primitive type, or as by calling
		''' <tt>Arrays.deepHashCode(e)</tt> recursively if <tt>e</tt> is an array
		''' of a reference type.  If <tt>a</tt> is <tt>null</tt>, this method
		''' returns 0.
		''' </summary>
		''' <param name="a"> the array whose deep-content-based hash code to compute </param>
		''' <returns> a deep-content-based hash code for <tt>a</tt> </returns>
		''' <seealso cref= #hashCode(Object[])
		''' @since 1.5 </seealso>
		Public Shared Function deepHashCode(  a As Object()) As Integer
			If a Is Nothing Then Return 0

			Dim result As Integer = 1

			For Each element As Object In a
				Dim elementHash As Integer = 0
				If TypeOf element Is Object() Then
					elementHash = deepHashCode(CType(element, Object()))
				ElseIf TypeOf element Is SByte() Then
					elementHash = hashCode(CType(element, SByte()))
				ElseIf TypeOf element Is Short() Then
					elementHash = hashCode(CType(element, Short()))
				ElseIf TypeOf element Is Integer() Then
					elementHash = hashCode(CType(element, Integer()))
				ElseIf TypeOf element Is Long() Then
					elementHash = hashCode(CType(element, Long()))
				ElseIf TypeOf element Is Char() Then
					elementHash = hashCode(CType(element, Char()))
				ElseIf TypeOf element Is Single() Then
					elementHash = hashCode(CType(element, Single()))
				ElseIf TypeOf element Is Double() Then
					elementHash = hashCode(CType(element, Double()))
				ElseIf TypeOf element Is Boolean() Then
					elementHash = hashCode(CType(element, Boolean()))
				ElseIf element IsNot Nothing Then
					elementHash = element.GetHashCode()
				End If

				result = 31 * result + elementHash
			Next element

			Return result
		End Function

		''' <summary>
		''' Returns <tt>true</tt> if the two specified arrays are <i>deeply
		''' equal</i> to one another.  Unlike the <seealso cref="#equals(Object[],Object[])"/>
		''' method, this method is appropriate for use with nested arrays of
		''' arbitrary depth.
		''' 
		''' <p>Two array references are considered deeply equal if both
		''' are <tt>null</tt>, or if they refer to arrays that contain the same
		''' number of elements and all corresponding pairs of elements in the two
		''' arrays are deeply equal.
		''' 
		''' <p>Two possibly <tt>null</tt> elements <tt>e1</tt> and <tt>e2</tt> are
		''' deeply equal if any of the following conditions hold:
		''' <ul>
		'''    <li> <tt>e1</tt> and <tt>e2</tt> are both arrays of object reference
		'''         types, and <tt>Arrays.deepEquals(e1, e2) would return true</tt>
		'''    <li> <tt>e1</tt> and <tt>e2</tt> are arrays of the same primitive
		'''         type, and the appropriate overloading of
		'''         <tt>Arrays.equals(e1, e2)</tt> would return true.
		'''    <li> <tt>e1 == e2</tt>
		'''    <li> <tt>e1.equals(e2)</tt> would return true.
		''' </ul>
		''' Note that this definition permits <tt>null</tt> elements at any depth.
		''' 
		''' <p>If either of the specified arrays contain themselves as elements
		''' either directly or indirectly through one or more levels of arrays,
		''' the behavior of this method is undefined.
		''' </summary>
		''' <param name="a1"> one array to be tested for equality </param>
		''' <param name="a2"> the other array to be tested for equality </param>
		''' <returns> <tt>true</tt> if the two arrays are equal </returns>
		''' <seealso cref= #equals(Object[],Object[]) </seealso>
		''' <seealso cref= Objects#deepEquals(Object, Object)
		''' @since 1.5 </seealso>
		Public Shared Function deepEquals(  a1 As Object(),   a2 As Object()) As Boolean
			If a1 = a2 Then Return True
			If a1 Is Nothing OrElse a2 Is Nothing Then Return False
			Dim length As Integer = a1.Length
			If a2.Length <> length Then Return False

			For i As Integer = 0 To length - 1
				Dim e1 As Object = a1(i)
				Dim e2 As Object = a2(i)

				If e1 Is e2 Then Continue For
				If e1 Is Nothing Then Return False

				' Figure out whether the two elements are equal
				Dim eq As Boolean = deepEquals0(e1, e2)

				If Not eq Then Return False
			Next i
			Return True
		End Function

		Friend Shared Function deepEquals0(  e1 As Object,   e2 As Object) As Boolean
			Debug.Assert(e1 IsNot Nothing)
			Dim eq As Boolean
			If TypeOf e1 Is Object() AndAlso TypeOf e2 Is Object() Then
				eq = deepEquals(CType(e1, Object()), CType(e2, Object()))
			ElseIf TypeOf e1 Is SByte() AndAlso TypeOf e2 Is SByte() Then
				eq = Equals(CType(e1, SByte()), CType(e2, SByte()))
			ElseIf TypeOf e1 Is Short() AndAlso TypeOf e2 Is Short() Then
				eq = Equals(CType(e1, Short()), CType(e2, Short()))
			ElseIf TypeOf e1 Is Integer() AndAlso TypeOf e2 Is Integer() Then
				eq = Equals(CType(e1, Integer()), CType(e2, Integer()))
			ElseIf TypeOf e1 Is Long() AndAlso TypeOf e2 Is Long() Then
				eq = Equals(CType(e1, Long()), CType(e2, Long()))
			ElseIf TypeOf e1 Is Char() AndAlso TypeOf e2 Is Char() Then
				eq = Equals(CType(e1, Char()), CType(e2, Char()))
			ElseIf TypeOf e1 Is Single() AndAlso TypeOf e2 Is Single() Then
				eq = Equals(CType(e1, Single()), CType(e2, Single()))
			ElseIf TypeOf e1 Is Double() AndAlso TypeOf e2 Is Double() Then
				eq = Equals(CType(e1, Double()), CType(e2, Double()))
			ElseIf TypeOf e1 Is Boolean() AndAlso TypeOf e2 Is Boolean() Then
				eq = Equals(CType(e1, Boolean()), CType(e2, Boolean()))
			Else
				eq = e1.Equals(e2)
			End If
			Return eq
		End Function

		''' <summary>
		''' Returns a string representation of the contents of the specified array.
		''' The string representation consists of a list of the array's elements,
		''' enclosed in square brackets (<tt>"[]"</tt>).  Adjacent elements are
		''' separated by the characters <tt>", "</tt> (a comma followed by a
		''' space).  Elements are converted to strings as by
		''' <tt>String.valueOf(long)</tt>.  Returns <tt>"null"</tt> if <tt>a</tt>
		''' is <tt>null</tt>.
		''' </summary>
		''' <param name="a"> the array whose string representation to return </param>
		''' <returns> a string representation of <tt>a</tt>
		''' @since 1.5 </returns>
		Public Shared Function ToString(  a As Long()) As String
			If a Is Nothing Then Return "null"
			Dim iMax As Integer = a.Length - 1
			If iMax = -1 Then Return "[]"

			Dim b As New StringBuilder
			b.append("["c)
			Dim i As Integer = 0
			Do
				b.append(a(i))
				If i = iMax Then Return b.append("]"c).ToString()
				b.append(", ")
				i += 1
			Loop
		End Function

		''' <summary>
		''' Returns a string representation of the contents of the specified array.
		''' The string representation consists of a list of the array's elements,
		''' enclosed in square brackets (<tt>"[]"</tt>).  Adjacent elements are
		''' separated by the characters <tt>", "</tt> (a comma followed by a
		''' space).  Elements are converted to strings as by
		''' <tt>String.valueOf(int)</tt>.  Returns <tt>"null"</tt> if <tt>a</tt> is
		''' <tt>null</tt>.
		''' </summary>
		''' <param name="a"> the array whose string representation to return </param>
		''' <returns> a string representation of <tt>a</tt>
		''' @since 1.5 </returns>
		Public Shared Function ToString(  a As Integer()) As String
			If a Is Nothing Then Return "null"
			Dim iMax As Integer = a.Length - 1
			If iMax = -1 Then Return "[]"

			Dim b As New StringBuilder
			b.append("["c)
			Dim i As Integer = 0
			Do
				b.append(a(i))
				If i = iMax Then Return b.append("]"c).ToString()
				b.append(", ")
				i += 1
			Loop
		End Function

		''' <summary>
		''' Returns a string representation of the contents of the specified array.
		''' The string representation consists of a list of the array's elements,
		''' enclosed in square brackets (<tt>"[]"</tt>).  Adjacent elements are
		''' separated by the characters <tt>", "</tt> (a comma followed by a
		''' space).  Elements are converted to strings as by
		''' <tt>String.valueOf(short)</tt>.  Returns <tt>"null"</tt> if <tt>a</tt>
		''' is <tt>null</tt>.
		''' </summary>
		''' <param name="a"> the array whose string representation to return </param>
		''' <returns> a string representation of <tt>a</tt>
		''' @since 1.5 </returns>
		Public Shared Function ToString(  a As Short()) As String
			If a Is Nothing Then Return "null"
			Dim iMax As Integer = a.Length - 1
			If iMax = -1 Then Return "[]"

			Dim b As New StringBuilder
			b.append("["c)
			Dim i As Integer = 0
			Do
				b.append(a(i))
				If i = iMax Then Return b.append("]"c).ToString()
				b.append(", ")
				i += 1
			Loop
		End Function

		''' <summary>
		''' Returns a string representation of the contents of the specified array.
		''' The string representation consists of a list of the array's elements,
		''' enclosed in square brackets (<tt>"[]"</tt>).  Adjacent elements are
		''' separated by the characters <tt>", "</tt> (a comma followed by a
		''' space).  Elements are converted to strings as by
		''' <tt>String.valueOf(char)</tt>.  Returns <tt>"null"</tt> if <tt>a</tt>
		''' is <tt>null</tt>.
		''' </summary>
		''' <param name="a"> the array whose string representation to return </param>
		''' <returns> a string representation of <tt>a</tt>
		''' @since 1.5 </returns>
		Public Shared Function ToString(  a As Char()) As String
			If a Is Nothing Then Return "null"
			Dim iMax As Integer = a.Length - 1
			If iMax = -1 Then Return "[]"

			Dim b As New StringBuilder
			b.append("["c)
			Dim i As Integer = 0
			Do
				b.append(a(i))
				If i = iMax Then Return b.append("]"c).ToString()
				b.append(", ")
				i += 1
			Loop
		End Function

		''' <summary>
		''' Returns a string representation of the contents of the specified array.
		''' The string representation consists of a list of the array's elements,
		''' enclosed in square brackets (<tt>"[]"</tt>).  Adjacent elements
		''' are separated by the characters <tt>", "</tt> (a comma followed
		''' by a space).  Elements are converted to strings as by
		''' <tt>String.valueOf(byte)</tt>.  Returns <tt>"null"</tt> if
		''' <tt>a</tt> is <tt>null</tt>.
		''' </summary>
		''' <param name="a"> the array whose string representation to return </param>
		''' <returns> a string representation of <tt>a</tt>
		''' @since 1.5 </returns>
		Public Shared Function ToString(  a As SByte()) As String
			If a Is Nothing Then Return "null"
			Dim iMax As Integer = a.Length - 1
			If iMax = -1 Then Return "[]"

			Dim b As New StringBuilder
			b.append("["c)
			Dim i As Integer = 0
			Do
				b.append(a(i))
				If i = iMax Then Return b.append("]"c).ToString()
				b.append(", ")
				i += 1
			Loop
		End Function

		''' <summary>
		''' Returns a string representation of the contents of the specified array.
		''' The string representation consists of a list of the array's elements,
		''' enclosed in square brackets (<tt>"[]"</tt>).  Adjacent elements are
		''' separated by the characters <tt>", "</tt> (a comma followed by a
		''' space).  Elements are converted to strings as by
		''' <tt>String.valueOf(boolean)</tt>.  Returns <tt>"null"</tt> if
		''' <tt>a</tt> is <tt>null</tt>.
		''' </summary>
		''' <param name="a"> the array whose string representation to return </param>
		''' <returns> a string representation of <tt>a</tt>
		''' @since 1.5 </returns>
		Public Shared Function ToString(  a As Boolean()) As String
			If a Is Nothing Then Return "null"
			Dim iMax As Integer = a.Length - 1
			If iMax = -1 Then Return "[]"

			Dim b As New StringBuilder
			b.append("["c)
			Dim i As Integer = 0
			Do
				b.append(a(i))
				If i = iMax Then Return b.append("]"c).ToString()
				b.append(", ")
				i += 1
			Loop
		End Function

		''' <summary>
		''' Returns a string representation of the contents of the specified array.
		''' The string representation consists of a list of the array's elements,
		''' enclosed in square brackets (<tt>"[]"</tt>).  Adjacent elements are
		''' separated by the characters <tt>", "</tt> (a comma followed by a
		''' space).  Elements are converted to strings as by
		''' <tt>String.valueOf(float)</tt>.  Returns <tt>"null"</tt> if <tt>a</tt>
		''' is <tt>null</tt>.
		''' </summary>
		''' <param name="a"> the array whose string representation to return </param>
		''' <returns> a string representation of <tt>a</tt>
		''' @since 1.5 </returns>
		Public Shared Function ToString(  a As Single()) As String
			If a Is Nothing Then Return "null"

			Dim iMax As Integer = a.Length - 1
			If iMax = -1 Then Return "[]"

			Dim b As New StringBuilder
			b.append("["c)
			Dim i As Integer = 0
			Do
				b.append(a(i))
				If i = iMax Then Return b.append("]"c).ToString()
				b.append(", ")
				i += 1
			Loop
		End Function

		''' <summary>
		''' Returns a string representation of the contents of the specified array.
		''' The string representation consists of a list of the array's elements,
		''' enclosed in square brackets (<tt>"[]"</tt>).  Adjacent elements are
		''' separated by the characters <tt>", "</tt> (a comma followed by a
		''' space).  Elements are converted to strings as by
		''' <tt>String.valueOf(double)</tt>.  Returns <tt>"null"</tt> if <tt>a</tt>
		''' is <tt>null</tt>.
		''' </summary>
		''' <param name="a"> the array whose string representation to return </param>
		''' <returns> a string representation of <tt>a</tt>
		''' @since 1.5 </returns>
		Public Shared Function ToString(  a As Double()) As String
			If a Is Nothing Then Return "null"
			Dim iMax As Integer = a.Length - 1
			If iMax = -1 Then Return "[]"

			Dim b As New StringBuilder
			b.append("["c)
			Dim i As Integer = 0
			Do
				b.append(a(i))
				If i = iMax Then Return b.append("]"c).ToString()
				b.append(", ")
				i += 1
			Loop
		End Function

		''' <summary>
		''' Returns a string representation of the contents of the specified array.
		''' If the array contains other arrays as elements, they are converted to
		''' strings by the <seealso cref="Object#toString"/> method inherited from
		''' <tt>Object</tt>, which describes their <i>identities</i> rather than
		''' their contents.
		''' 
		''' <p>The value returned by this method is equal to the value that would
		''' be returned by <tt>Arrays.asList(a).toString()</tt>, unless <tt>a</tt>
		''' is <tt>null</tt>, in which case <tt>"null"</tt> is returned.
		''' </summary>
		''' <param name="a"> the array whose string representation to return </param>
		''' <returns> a string representation of <tt>a</tt> </returns>
		''' <seealso cref= #deepToString(Object[])
		''' @since 1.5 </seealso>
		Public Shared Function ToString(  a As Object()) As String
			If a Is Nothing Then Return "null"

			Dim iMax As Integer = a.Length - 1
			If iMax = -1 Then Return "[]"

			Dim b As New StringBuilder
			b.append("["c)
			Dim i As Integer = 0
			Do
				b.append(Convert.ToString(a(i)))
				If i = iMax Then Return b.append("]"c).ToString()
				b.append(", ")
				i += 1
			Loop
		End Function

		''' <summary>
		''' Returns a string representation of the "deep contents" of the specified
		''' array.  If the array contains other arrays as elements, the string
		''' representation contains their contents and so on.  This method is
		''' designed for converting multidimensional arrays to strings.
		''' 
		''' <p>The string representation consists of a list of the array's
		''' elements, enclosed in square brackets (<tt>"[]"</tt>).  Adjacent
		''' elements are separated by the characters <tt>", "</tt> (a comma
		''' followed by a space).  Elements are converted to strings as by
		''' <tt>String.valueOf(Object)</tt>, unless they are themselves
		''' arrays.
		''' 
		''' <p>If an element <tt>e</tt> is an array of a primitive type, it is
		''' converted to a string as by invoking the appropriate overloading of
		''' <tt>Arrays.toString(e)</tt>.  If an element <tt>e</tt> is an array of a
		''' reference type, it is converted to a string as by invoking
		''' this method recursively.
		''' 
		''' <p>To avoid infinite recursion, if the specified array contains itself
		''' as an element, or contains an indirect reference to itself through one
		''' or more levels of arrays, the self-reference is converted to the string
		''' <tt>"[...]"</tt>.  For example, an array containing only a reference
		''' to itself would be rendered as <tt>"[[...]]"</tt>.
		''' 
		''' <p>This method returns <tt>"null"</tt> if the specified array
		''' is <tt>null</tt>.
		''' </summary>
		''' <param name="a"> the array whose string representation to return </param>
		''' <returns> a string representation of <tt>a</tt> </returns>
		''' <seealso cref= #toString(Object[])
		''' @since 1.5 </seealso>
		Public Shared Function deepToString(  a As Object()) As String
			If a Is Nothing Then Return "null"

			Dim bufLen As Integer = 20 * a.Length
			If a.Length <> 0 AndAlso bufLen <= 0 Then bufLen =  java.lang.[Integer].Max_Value
			Dim buf As New StringBuilder(bufLen)
			deepToString(a, buf, New HashSet(Of Object()))
			Return buf.ToString()
		End Function

		Private Shared Sub deepToString(  a As Object(),   buf As StringBuilder,   dejaVu As [Set](Of Object()))
			If a Is Nothing Then
				buf.append("null")
				Return
			End If
			Dim iMax As Integer = a.Length - 1
			If iMax = -1 Then
				buf.append("[]")
				Return
			End If

			dejaVu.add(a)
			buf.append("["c)
			Dim i As Integer = 0
			Do

				Dim element As Object = a(i)
				If element Is Nothing Then
					buf.append("null")
				Else
					Dim eClass As  [Class] = element.GetType()

					If eClass.array Then
						If eClass Is GetType(SByte()) Then
							buf.append(ToString(CType(element, SByte())))
						ElseIf eClass Is GetType(Short()) Then
							buf.append(ToString(CType(element, Short())))
						ElseIf eClass Is GetType(Integer()) Then
							buf.append(ToString(CType(element, Integer())))
						ElseIf eClass Is GetType(Long()) Then
							buf.append(ToString(CType(element, Long())))
						ElseIf eClass Is GetType(Char()) Then
							buf.append(ToString(CType(element, Char())))
						ElseIf eClass Is GetType(Single()) Then
							buf.append(ToString(CType(element, Single())))
						ElseIf eClass Is GetType(Double()) Then
							buf.append(ToString(CType(element, Double())))
						ElseIf eClass Is GetType(Boolean()) Then
							buf.append(ToString(CType(element, Boolean())))
						Else ' element is an array of object references
							If dejaVu.contains(element) Then
								buf.append("[...]")
							Else
								deepToString(CType(element, Object()), buf, dejaVu)
							End If
						End If ' element is non-null and not an array
					Else
						buf.append(element.ToString())
					End If
				End If
				If i = iMax Then Exit Do
				buf.append(", ")
				i += 1
			Loop
			buf.append("]"c)
			dejaVu.remove(a)
		End Sub


		''' <summary>
		''' Set all elements of the specified array, using the provided
		''' generator function to compute each element.
		''' 
		''' <p>If the generator function throws an exception, it is relayed to
		''' the caller and the array is left in an indeterminate state.
		''' </summary>
		''' @param <T> type of elements of the array </param>
		''' <param name="array"> array to be initialized </param>
		''' <param name="generator"> a function accepting an index and producing the desired
		'''        value for that position </param>
		''' <exception cref="NullPointerException"> if the generator is null
		''' @since 1.8 </exception>
		Public Shared Sub setAll(Of T, T1 As T)(  array As T(),   generator As java.util.function.IntFunction(Of T1))
			Objects.requireNonNull(generator)
			For i As Integer = 0 To array.Length - 1
				array(i) = generator.apply(i)
			Next i
		End Sub

		''' <summary>
		''' Set all elements of the specified array, in parallel, using the
		''' provided generator function to compute each element.
		''' 
		''' <p>If the generator function throws an exception, an unchecked exception
		''' is thrown from {@code parallelSetAll} and the array is left in an
		''' indeterminate state.
		''' </summary>
		''' @param <T> type of elements of the array </param>
		''' <param name="array"> array to be initialized </param>
		''' <param name="generator"> a function accepting an index and producing the desired
		'''        value for that position </param>
		''' <exception cref="NullPointerException"> if the generator is null
		''' @since 1.8 </exception>
		Public Shared Sub parallelSetAll(Of T, T1 As T)(  array As T(),   generator As java.util.function.IntFunction(Of T1))
			Objects.requireNonNull(generator)
'JAVA TO VB CONVERTER TODO TASK: Assignments within expressions are not supported in VB
			java.util.stream.IntStream.range(0, array.Length).parallel().forEach(i -> { array(i) = generator.apply(i); })
		End Sub

		''' <summary>
		''' Set all elements of the specified array, using the provided
		''' generator function to compute each element.
		''' 
		''' <p>If the generator function throws an exception, it is relayed to
		''' the caller and the array is left in an indeterminate state.
		''' </summary>
		''' <param name="array"> array to be initialized </param>
		''' <param name="generator"> a function accepting an index and producing the desired
		'''        value for that position </param>
		''' <exception cref="NullPointerException"> if the generator is null
		''' @since 1.8 </exception>
		Public Shared Sub setAll(  array As Integer(),   generator As java.util.function.IntUnaryOperator)
			Objects.requireNonNull(generator)
			For i As Integer = 0 To array.Length - 1
				array(i) = generator.applyAsInt(i)
			Next i
		End Sub

		''' <summary>
		''' Set all elements of the specified array, in parallel, using the
		''' provided generator function to compute each element.
		''' 
		''' <p>If the generator function throws an exception, an unchecked exception
		''' is thrown from {@code parallelSetAll} and the array is left in an
		''' indeterminate state.
		''' </summary>
		''' <param name="array"> array to be initialized </param>
		''' <param name="generator"> a function accepting an index and producing the desired
		''' value for that position </param>
		''' <exception cref="NullPointerException"> if the generator is null
		''' @since 1.8 </exception>
		Public Shared Sub parallelSetAll(  array As Integer(),   generator As java.util.function.IntUnaryOperator)
			Objects.requireNonNull(generator)
'JAVA TO VB CONVERTER TODO TASK: Assignments within expressions are not supported in VB
			java.util.stream.IntStream.range(0, array.Length).parallel().forEach(i -> { array(i) = generator.applyAsInt(i); })
		End Sub

		''' <summary>
		''' Set all elements of the specified array, using the provided
		''' generator function to compute each element.
		''' 
		''' <p>If the generator function throws an exception, it is relayed to
		''' the caller and the array is left in an indeterminate state.
		''' </summary>
		''' <param name="array"> array to be initialized </param>
		''' <param name="generator"> a function accepting an index and producing the desired
		'''        value for that position </param>
		''' <exception cref="NullPointerException"> if the generator is null
		''' @since 1.8 </exception>
		Public Shared Sub setAll(  array As Long(),   generator As java.util.function.IntToLongFunction)
			Objects.requireNonNull(generator)
			For i As Integer = 0 To array.Length - 1
				array(i) = generator.applyAsLong(i)
			Next i
		End Sub

		''' <summary>
		''' Set all elements of the specified array, in parallel, using the
		''' provided generator function to compute each element.
		''' 
		''' <p>If the generator function throws an exception, an unchecked exception
		''' is thrown from {@code parallelSetAll} and the array is left in an
		''' indeterminate state.
		''' </summary>
		''' <param name="array"> array to be initialized </param>
		''' <param name="generator"> a function accepting an index and producing the desired
		'''        value for that position </param>
		''' <exception cref="NullPointerException"> if the generator is null
		''' @since 1.8 </exception>
		Public Shared Sub parallelSetAll(  array As Long(),   generator As java.util.function.IntToLongFunction)
			Objects.requireNonNull(generator)
'JAVA TO VB CONVERTER TODO TASK: Assignments within expressions are not supported in VB
			java.util.stream.IntStream.range(0, array.Length).parallel().forEach(i -> { array(i) = generator.applyAsLong(i); })
		End Sub

		''' <summary>
		''' Set all elements of the specified array, using the provided
		''' generator function to compute each element.
		''' 
		''' <p>If the generator function throws an exception, it is relayed to
		''' the caller and the array is left in an indeterminate state.
		''' </summary>
		''' <param name="array"> array to be initialized </param>
		''' <param name="generator"> a function accepting an index and producing the desired
		'''        value for that position </param>
		''' <exception cref="NullPointerException"> if the generator is null
		''' @since 1.8 </exception>
		Public Shared Sub setAll(  array As Double(),   generator As java.util.function.IntToDoubleFunction)
			Objects.requireNonNull(generator)
			For i As Integer = 0 To array.Length - 1
				array(i) = generator.applyAsDouble(i)
			Next i
		End Sub

		''' <summary>
		''' Set all elements of the specified array, in parallel, using the
		''' provided generator function to compute each element.
		''' 
		''' <p>If the generator function throws an exception, an unchecked exception
		''' is thrown from {@code parallelSetAll} and the array is left in an
		''' indeterminate state.
		''' </summary>
		''' <param name="array"> array to be initialized </param>
		''' <param name="generator"> a function accepting an index and producing the desired
		'''        value for that position </param>
		''' <exception cref="NullPointerException"> if the generator is null
		''' @since 1.8 </exception>
		Public Shared Sub parallelSetAll(  array As Double(),   generator As java.util.function.IntToDoubleFunction)
			Objects.requireNonNull(generator)
'JAVA TO VB CONVERTER TODO TASK: Assignments within expressions are not supported in VB
			java.util.stream.IntStream.range(0, array.Length).parallel().forEach(i -> { array(i) = generator.applyAsDouble(i); })
		End Sub

		''' <summary>
		''' Returns a <seealso cref="Spliterator"/> covering all of the specified array.
		''' 
		''' <p>The spliterator reports <seealso cref="Spliterator#SIZED"/>,
		''' <seealso cref="Spliterator#SUBSIZED"/>, <seealso cref="Spliterator#ORDERED"/>, and
		''' <seealso cref="Spliterator#IMMUTABLE"/>.
		''' </summary>
		''' @param <T> type of elements </param>
		''' <param name="array"> the array, assumed to be unmodified during use </param>
		''' <returns> a spliterator for the array elements
		''' @since 1.8 </returns>
		Public Shared Function spliterator(Of T)(  array As T()) As Spliterator(Of T)
			Return Spliterators.spliterator(array, Spliterator.ORDERED Or Spliterator.IMMUTABLE)
		End Function

		''' <summary>
		''' Returns a <seealso cref="Spliterator"/> covering the specified range of the
		''' specified array.
		''' 
		''' <p>The spliterator reports <seealso cref="Spliterator#SIZED"/>,
		''' <seealso cref="Spliterator#SUBSIZED"/>, <seealso cref="Spliterator#ORDERED"/>, and
		''' <seealso cref="Spliterator#IMMUTABLE"/>.
		''' </summary>
		''' @param <T> type of elements </param>
		''' <param name="array"> the array, assumed to be unmodified during use </param>
		''' <param name="startInclusive"> the first index to cover, inclusive </param>
		''' <param name="endExclusive"> index immediately past the last index to cover </param>
		''' <returns> a spliterator for the array elements </returns>
		''' <exception cref="ArrayIndexOutOfBoundsException"> if {@code startInclusive} is
		'''         negative, {@code endExclusive} is less than
		'''         {@code startInclusive}, or {@code endExclusive} is greater than
		'''         the array size
		''' @since 1.8 </exception>
		Public Shared Function spliterator(Of T)(  array As T(),   startInclusive As Integer,   endExclusive As Integer) As Spliterator(Of T)
			Return Spliterators.spliterator(array, startInclusive, endExclusive, Spliterator.ORDERED Or Spliterator.IMMUTABLE)
		End Function

		''' <summary>
		''' Returns a <seealso cref="Spliterator.OfInt"/> covering all of the specified array.
		''' 
		''' <p>The spliterator reports <seealso cref="Spliterator#SIZED"/>,
		''' <seealso cref="Spliterator#SUBSIZED"/>, <seealso cref="Spliterator#ORDERED"/>, and
		''' <seealso cref="Spliterator#IMMUTABLE"/>.
		''' </summary>
		''' <param name="array"> the array, assumed to be unmodified during use </param>
		''' <returns> a spliterator for the array elements
		''' @since 1.8 </returns>
		Public Shared Function spliterator(  array As Integer()) As Spliterator.OfInt
			Return Spliterators.spliterator(array, Spliterator.ORDERED Or Spliterator.IMMUTABLE)
		End Function

		''' <summary>
		''' Returns a <seealso cref="Spliterator.OfInt"/> covering the specified range of the
		''' specified array.
		''' 
		''' <p>The spliterator reports <seealso cref="Spliterator#SIZED"/>,
		''' <seealso cref="Spliterator#SUBSIZED"/>, <seealso cref="Spliterator#ORDERED"/>, and
		''' <seealso cref="Spliterator#IMMUTABLE"/>.
		''' </summary>
		''' <param name="array"> the array, assumed to be unmodified during use </param>
		''' <param name="startInclusive"> the first index to cover, inclusive </param>
		''' <param name="endExclusive"> index immediately past the last index to cover </param>
		''' <returns> a spliterator for the array elements </returns>
		''' <exception cref="ArrayIndexOutOfBoundsException"> if {@code startInclusive} is
		'''         negative, {@code endExclusive} is less than
		'''         {@code startInclusive}, or {@code endExclusive} is greater than
		'''         the array size
		''' @since 1.8 </exception>
		Public Shared Function spliterator(  array As Integer(),   startInclusive As Integer,   endExclusive As Integer) As Spliterator.OfInt
			Return Spliterators.spliterator(array, startInclusive, endExclusive, Spliterator.ORDERED Or Spliterator.IMMUTABLE)
		End Function

		''' <summary>
		''' Returns a <seealso cref="Spliterator.OfLong"/> covering all of the specified array.
		''' 
		''' <p>The spliterator reports <seealso cref="Spliterator#SIZED"/>,
		''' <seealso cref="Spliterator#SUBSIZED"/>, <seealso cref="Spliterator#ORDERED"/>, and
		''' <seealso cref="Spliterator#IMMUTABLE"/>.
		''' </summary>
		''' <param name="array"> the array, assumed to be unmodified during use </param>
		''' <returns> the spliterator for the array elements
		''' @since 1.8 </returns>
		Public Shared Function spliterator(  array As Long()) As Spliterator.OfLong
			Return Spliterators.spliterator(array, Spliterator.ORDERED Or Spliterator.IMMUTABLE)
		End Function

		''' <summary>
		''' Returns a <seealso cref="Spliterator.OfLong"/> covering the specified range of the
		''' specified array.
		''' 
		''' <p>The spliterator reports <seealso cref="Spliterator#SIZED"/>,
		''' <seealso cref="Spliterator#SUBSIZED"/>, <seealso cref="Spliterator#ORDERED"/>, and
		''' <seealso cref="Spliterator#IMMUTABLE"/>.
		''' </summary>
		''' <param name="array"> the array, assumed to be unmodified during use </param>
		''' <param name="startInclusive"> the first index to cover, inclusive </param>
		''' <param name="endExclusive"> index immediately past the last index to cover </param>
		''' <returns> a spliterator for the array elements </returns>
		''' <exception cref="ArrayIndexOutOfBoundsException"> if {@code startInclusive} is
		'''         negative, {@code endExclusive} is less than
		'''         {@code startInclusive}, or {@code endExclusive} is greater than
		'''         the array size
		''' @since 1.8 </exception>
		Public Shared Function spliterator(  array As Long(),   startInclusive As Integer,   endExclusive As Integer) As Spliterator.OfLong
			Return Spliterators.spliterator(array, startInclusive, endExclusive, Spliterator.ORDERED Or Spliterator.IMMUTABLE)
		End Function

		''' <summary>
		''' Returns a <seealso cref="Spliterator.OfDouble"/> covering all of the specified
		''' array.
		''' 
		''' <p>The spliterator reports <seealso cref="Spliterator#SIZED"/>,
		''' <seealso cref="Spliterator#SUBSIZED"/>, <seealso cref="Spliterator#ORDERED"/>, and
		''' <seealso cref="Spliterator#IMMUTABLE"/>.
		''' </summary>
		''' <param name="array"> the array, assumed to be unmodified during use </param>
		''' <returns> a spliterator for the array elements
		''' @since 1.8 </returns>
		Public Shared Function spliterator(  array As Double()) As Spliterator.OfDouble
			Return Spliterators.spliterator(array, Spliterator.ORDERED Or Spliterator.IMMUTABLE)
		End Function

		''' <summary>
		''' Returns a <seealso cref="Spliterator.OfDouble"/> covering the specified range of
		''' the specified array.
		''' 
		''' <p>The spliterator reports <seealso cref="Spliterator#SIZED"/>,
		''' <seealso cref="Spliterator#SUBSIZED"/>, <seealso cref="Spliterator#ORDERED"/>, and
		''' <seealso cref="Spliterator#IMMUTABLE"/>.
		''' </summary>
		''' <param name="array"> the array, assumed to be unmodified during use </param>
		''' <param name="startInclusive"> the first index to cover, inclusive </param>
		''' <param name="endExclusive"> index immediately past the last index to cover </param>
		''' <returns> a spliterator for the array elements </returns>
		''' <exception cref="ArrayIndexOutOfBoundsException"> if {@code startInclusive} is
		'''         negative, {@code endExclusive} is less than
		'''         {@code startInclusive}, or {@code endExclusive} is greater than
		'''         the array size
		''' @since 1.8 </exception>
		Public Shared Function spliterator(  array As Double(),   startInclusive As Integer,   endExclusive As Integer) As Spliterator.OfDouble
			Return Spliterators.spliterator(array, startInclusive, endExclusive, Spliterator.ORDERED Or Spliterator.IMMUTABLE)
		End Function

		''' <summary>
		''' Returns a sequential <seealso cref="Stream"/> with the specified array as its
		''' source.
		''' </summary>
		''' @param <T> The type of the array elements </param>
		''' <param name="array"> The array, assumed to be unmodified during use </param>
		''' <returns> a {@code Stream} for the array
		''' @since 1.8 </returns>
		Public Shared Function stream(Of T)(  array As T()) As java.util.stream.Stream(Of T)
			Return stream(array, 0, array.Length)
		End Function

		''' <summary>
		''' Returns a sequential <seealso cref="Stream"/> with the specified range of the
		''' specified array as its source.
		''' </summary>
		''' @param <T> the type of the array elements </param>
		''' <param name="array"> the array, assumed to be unmodified during use </param>
		''' <param name="startInclusive"> the first index to cover, inclusive </param>
		''' <param name="endExclusive"> index immediately past the last index to cover </param>
		''' <returns> a {@code Stream} for the array range </returns>
		''' <exception cref="ArrayIndexOutOfBoundsException"> if {@code startInclusive} is
		'''         negative, {@code endExclusive} is less than
		'''         {@code startInclusive}, or {@code endExclusive} is greater than
		'''         the array size
		''' @since 1.8 </exception>
		Public Shared Function stream(Of T)(  array As T(),   startInclusive As Integer,   endExclusive As Integer) As java.util.stream.Stream(Of T)
			Return java.util.stream.StreamSupport.stream(spliterator(array, startInclusive, endExclusive), False)
		End Function

		''' <summary>
		''' Returns a sequential <seealso cref="IntStream"/> with the specified array as its
		''' source.
		''' </summary>
		''' <param name="array"> the array, assumed to be unmodified during use </param>
		''' <returns> an {@code IntStream} for the array
		''' @since 1.8 </returns>
		Public Shared Function stream(  array As Integer()) As java.util.stream.IntStream
			Return stream(array, 0, array.Length)
		End Function

		''' <summary>
		''' Returns a sequential <seealso cref="IntStream"/> with the specified range of the
		''' specified array as its source.
		''' </summary>
		''' <param name="array"> the array, assumed to be unmodified during use </param>
		''' <param name="startInclusive"> the first index to cover, inclusive </param>
		''' <param name="endExclusive"> index immediately past the last index to cover </param>
		''' <returns> an {@code IntStream} for the array range </returns>
		''' <exception cref="ArrayIndexOutOfBoundsException"> if {@code startInclusive} is
		'''         negative, {@code endExclusive} is less than
		'''         {@code startInclusive}, or {@code endExclusive} is greater than
		'''         the array size
		''' @since 1.8 </exception>
		Public Shared Function stream(  array As Integer(),   startInclusive As Integer,   endExclusive As Integer) As java.util.stream.IntStream
			Return java.util.stream.StreamSupport.intStream(spliterator(array, startInclusive, endExclusive), False)
		End Function

		''' <summary>
		''' Returns a sequential <seealso cref="LongStream"/> with the specified array as its
		''' source.
		''' </summary>
		''' <param name="array"> the array, assumed to be unmodified during use </param>
		''' <returns> a {@code LongStream} for the array
		''' @since 1.8 </returns>
		Public Shared Function stream(  array As Long()) As java.util.stream.LongStream
			Return stream(array, 0, array.Length)
		End Function

		''' <summary>
		''' Returns a sequential <seealso cref="LongStream"/> with the specified range of the
		''' specified array as its source.
		''' </summary>
		''' <param name="array"> the array, assumed to be unmodified during use </param>
		''' <param name="startInclusive"> the first index to cover, inclusive </param>
		''' <param name="endExclusive"> index immediately past the last index to cover </param>
		''' <returns> a {@code LongStream} for the array range </returns>
		''' <exception cref="ArrayIndexOutOfBoundsException"> if {@code startInclusive} is
		'''         negative, {@code endExclusive} is less than
		'''         {@code startInclusive}, or {@code endExclusive} is greater than
		'''         the array size
		''' @since 1.8 </exception>
		Public Shared Function stream(  array As Long(),   startInclusive As Integer,   endExclusive As Integer) As java.util.stream.LongStream
			Return java.util.stream.StreamSupport.longStream(spliterator(array, startInclusive, endExclusive), False)
		End Function

		''' <summary>
		''' Returns a sequential <seealso cref="DoubleStream"/> with the specified array as its
		''' source.
		''' </summary>
		''' <param name="array"> the array, assumed to be unmodified during use </param>
		''' <returns> a {@code DoubleStream} for the array
		''' @since 1.8 </returns>
		Public Shared Function stream(  array As Double()) As java.util.stream.DoubleStream
			Return stream(array, 0, array.Length)
		End Function

		''' <summary>
		''' Returns a sequential <seealso cref="DoubleStream"/> with the specified range of the
		''' specified array as its source.
		''' </summary>
		''' <param name="array"> the array, assumed to be unmodified during use </param>
		''' <param name="startInclusive"> the first index to cover, inclusive </param>
		''' <param name="endExclusive"> index immediately past the last index to cover </param>
		''' <returns> a {@code DoubleStream} for the array range </returns>
		''' <exception cref="ArrayIndexOutOfBoundsException"> if {@code startInclusive} is
		'''         negative, {@code endExclusive} is less than
		'''         {@code startInclusive}, or {@code endExclusive} is greater than
		'''         the array size
		''' @since 1.8 </exception>
		Public Shared Function stream(  array As Double(),   startInclusive As Integer,   endExclusive As Integer) As java.util.stream.DoubleStream
			Return java.util.stream.StreamSupport.doubleStream(spliterator(array, startInclusive, endExclusive), False)
		End Function
	End Class

End Namespace