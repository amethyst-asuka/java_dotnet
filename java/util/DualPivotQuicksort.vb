Imports Microsoft.VisualBasic
Imports System

'
' * Copyright (c) 2009, 2013, Oracle and/or its affiliates. All rights reserved.
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
	''' This class implements the Dual-Pivot Quicksort algorithm by
	''' Vladimir Yaroslavskiy, Jon Bentley, and Josh Bloch. The algorithm
	''' offers O(n log(n)) performance on many data sets that cause other
	''' quicksorts to degrade to quadratic performance, and is typically
	''' faster than traditional (one-pivot) Quicksort implementations.
	''' 
	''' All exposed methods are package-private, designed to be invoked
	''' from public methods (in class Arrays) after performing any
	''' necessary array bounds checks and expanding parameters into the
	''' required forms.
	''' 
	''' @author Vladimir Yaroslavskiy
	''' @author Jon Bentley
	''' @author Josh Bloch
	''' 
	''' @version 2011.02.11 m765.827.12i:5\7pm
	''' @since 1.7
	''' </summary>
	Friend NotInheritable Class DualPivotQuicksort

		''' <summary>
		''' Prevents instantiation.
		''' </summary>
		Private Sub New()
		End Sub

	'    
	'     * Tuning parameters.
	'     

		''' <summary>
		''' The maximum number of runs in merge sort.
		''' </summary>
		Private Const MAX_RUN_COUNT As Integer = 67

		''' <summary>
		''' The maximum length of run in merge sort.
		''' </summary>
		Private Const MAX_RUN_LENGTH As Integer = 33

		''' <summary>
		''' If the length of an array to be sorted is less than this
		''' constant, Quicksort is used in preference to merge sort.
		''' </summary>
		Private Const QUICKSORT_THRESHOLD As Integer = 286

		''' <summary>
		''' If the length of an array to be sorted is less than this
		''' constant, insertion sort is used in preference to Quicksort.
		''' </summary>
		Private Const INSERTION_SORT_THRESHOLD As Integer = 47

		''' <summary>
		''' If the length of a byte array to be sorted is greater than this
		''' constant, counting sort is used in preference to insertion sort.
		''' </summary>
		Private Const COUNTING_SORT_THRESHOLD_FOR_BYTE As Integer = 29

		''' <summary>
		''' If the length of a short or char array to be sorted is greater
		''' than this constant, counting sort is used in preference to Quicksort.
		''' </summary>
		Private Const COUNTING_SORT_THRESHOLD_FOR_SHORT_OR_CHAR As Integer = 3200

	'    
	'     * Sorting methods for seven primitive types.
	'     

		''' <summary>
		''' Sorts the specified range of the array using the given
		''' workspace array slice if possible for merging
		''' </summary>
		''' <param name="a"> the array to be sorted </param>
		''' <param name="left"> the index of the first element, inclusive, to be sorted </param>
		''' <param name="right"> the index of the last element, inclusive, to be sorted </param>
		''' <param name="work"> a workspace array (slice) </param>
		''' <param name="workBase"> origin of usable space in work array </param>
		''' <param name="workLen"> usable size of work array </param>
		Friend Shared Sub sort(ByVal a As Integer(), ByVal left As Integer, ByVal right As Integer, ByVal work As Integer(), ByVal workBase As Integer, ByVal workLen As Integer)
			' Use Quicksort on small arrays
			If right - left < QUICKSORT_THRESHOLD Then
				sort(a, left, right, True)
				Return
			End If

	'        
	'         * Index run[i] is the start of i-th run
	'         * (ascending or descending sequence).
	'         
			Dim run As Integer() = New Integer(MAX_RUN_COUNT){}
			Dim count As Integer = 0
			run(0) = left

			' Check if the array is nearly sorted
			Dim k As Integer = left
			Do While k < right
				If a(k) < a(k + 1) Then ' ascending
					k += 1
					Do While k <= right AndAlso a(k - 1) <= a(k)

						k += 1
					Loop ' descending
				ElseIf a(k) > a(k + 1) Then
					k += 1
					Do While k <= right AndAlso a(k - 1) >= a(k)

						k += 1
					Loop
					Dim lo As Integer = run(count) - 1
					Dim hi As Integer = k
'JAVA TO VB CONVERTER TODO TASK: Assignments within expressions are not supported in VB
					Do While lo += 1 < hi -= 1
						Dim t As Integer = a(lo)
						a(lo) = a(hi)
						a(hi) = t
					Loop ' equal
				Else
					Dim m As Integer = MAX_RUN_LENGTH
'JAVA TO VB CONVERTER TODO TASK: Assignments within expressions are not supported in VB
					Do While k += 1 <= right AndAlso a(k - 1) = a(k)
						m -= 1
						If m = 0 Then
							sort(a, left, right, True)
							Return
						End If
					Loop
				End If

	'            
	'             * The array is not highly structured,
	'             * use Quicksort instead of merge sort.
	'             
				count += 1
				If count = MAX_RUN_COUNT Then
					sort(a, left, right, True)
					Return
				End If
				run(count) = k
			Loop

			' Check special cases
			' Implementation note: variable "right" is increased by 1.
			Then += 1
			If run(count) = rightThen ' The last run contains one element
				count += 1
				run(count) = right ' The array is already sorted
			ElseIf count = 1 Then
				Return
			End If

			' Determine alternation base for merge
			Dim odd As SByte = 0
			Dim n As Integer = 1
			Do While (n <<= 1) < count

				odd = odd Xor 1
			Loop

			' Use or create temporary array b for merging
			Dim b As Integer() ' temp array; alternates with a
			Dim ao, bo As Integer ' array offsets from 'left'
			Dim blen As Integer = right - left ' space needed for b
			If work Is Nothing OrElse workLen < blen OrElse workBase + blen > work.Length Then
				work = New Integer(blen - 1){}
				workBase = 0
			End If
			If odd = 0 Then
				Array.Copy(a, left, work, workBase, blen)
				b = a
				bo = 0
				a = work
				ao = workBase - left
			Else
				b = work
				ao = 0
				bo = workBase - left
			End If

			' Merging
			Dim last As Integer
			Do While count > 1
				k = (last = 0) + 2
'JAVA TO VB CONVERTER TODO TASK: Assignments within expressions are not supported in VB
				Do While k <= count
					Dim hi As Integer = run(k), mi As Integer = run(k - 1)
					Dim i As Integer = run(k - 2)
					Dim p As Integer = i
					Dim q As Integer = mi
					Do While i < hi
						If q >= hi OrElse p < mi AndAlso a(p + ao) <= a(q + ao) Then
							b(i + bo) = a(p + ao)
							p += 1
						Else
							b(i + bo) = a(q + ao)
							q += 1
						End If
						i += 1
					Loop
					last += 1
					run(last) = hi
					k += 2
				Loop
				If (count And 1) <> 0 Then
					Dim i As Integer = right
					Dim lo As Integer = run(count - 1)
'JAVA TO VB CONVERTER TODO TASK: Assignments within expressions are not supported in VB
					Do While i -= 1 >= lo

						b(i + bo) = a(i + ao)
					Loop
					last += 1
					run(last) = right
				End If
				Dim t As Integer() = a
				a = b
				b = t
				Dim o As Integer = ao
				ao = bo
				bo = o
				count = last
			Loop
		End Sub

		''' <summary>
		''' Sorts the specified range of the array by Dual-Pivot Quicksort.
		''' </summary>
		''' <param name="a"> the array to be sorted </param>
		''' <param name="left"> the index of the first element, inclusive, to be sorted </param>
		''' <param name="right"> the index of the last element, inclusive, to be sorted </param>
		''' <param name="leftmost"> indicates if this part is the leftmost in the range </param>
		Private Shared Sub sort(ByVal a As Integer(), ByVal left As Integer, ByVal right As Integer, ByVal leftmost As Boolean)
			Dim length As Integer = right - left + 1

			' Use insertion sort on tiny arrays
			If length < INSERTION_SORT_THRESHOLD Then
				If leftmost Then
	'                
	'                 * Traditional (without sentinel) insertion sort,
	'                 * optimized for server VM, is used in case of
	'                 * the leftmost part.
	'                 
					Dim i As Integer = left
					Dim j As Integer = i
					Do While i < right
						Dim ai As Integer = a(i + 1)
						Do While ai < a(j)
							a(j + 1) = a(j)
							Dim tempVar As Boolean = j = left
							j -= 1
							If tempVar Then Exit Do
						Loop
						a(j + 1) = ai
						i += 1
j = i
					Loop
				Else
	'                
	'                 * Skip the longest ascending sequence.
	'                 
					Do
						If left >= right Then Return
						left += 1
					Loop While a(left) >= a(left - 1)

	'                
	'                 * Every element from adjoining part plays the role
	'                 * of sentinel, therefore this allows us to avoid the
	'                 * left range check on each iteration. Moreover, we use
	'                 * the more optimized algorithm, so called pair insertion
	'                 * sort, which is faster (in the context of Quicksort)
	'                 * than traditional implementation of insertion sort.
	'                 
					Dim k As Integer = left
'JAVA TO VB CONVERTER TODO TASK: Assignments within expressions are not supported in VB
					Do While left += 1 <= right
						Dim a1 As Integer = a(k), a2 As Integer = a(left)

						If a1 < a2 Then
							a2 = a1
							a1 = a(left)
						End If
						k -= 1
						Do While a1 < a(k)
							a(k + 2) = a(k)
							k -= 1
						Loop
						k += 1
						a(k + 1) = a1

						k -= 1
						Do While a2 < a(k)
							a(k + 1) = a(k)
							k -= 1
						Loop
						a(k + 1) = a2
						left += 1
k = left
					Loop
					Dim last As Integer = a(right)

					right -= 1
					Do While last < a(right)
						a(right + 1) = a(right)
						right -= 1
					Loop
					a(right + 1) = last
				End If
				Return
			End If

			' Inexpensive approximation of length / 7
			Dim seventh As Integer = (length >> 3) + (length >> 6) + 1

	'        
	'         * Sort five evenly spaced elements around (and including) the
	'         * center element in the range. These elements will be used for
	'         * pivot selection as described below. The choice for spacing
	'         * these elements was empirically determined to work well on
	'         * a wide variety of inputs.
	'         
			Dim e3 As Integer = CInt(CUInt((left + right)) >> 1) ' The midpoint
			Dim e2 As Integer = e3 - seventh
			Dim e1 As Integer = e2 - seventh
			Dim e4 As Integer = e3 + seventh
			Dim e5 As Integer = e4 + seventh

			' Sort these elements using insertion sort
			If a(e2) < a(e1) Then
				Dim t As Integer = a(e2)
				a(e2) = a(e1)
				a(e1) = t
			End If

			If a(e3) < a(e2) Then
				Dim t As Integer = a(e3)
				a(e3) = a(e2)
				a(e2) = t
				If t < a(e1) Then
					a(e2) = a(e1)
					a(e1) = t
				End If
			End If
			If a(e4) < a(e3) Then
				Dim t As Integer = a(e4)
				a(e4) = a(e3)
				a(e3) = t
				If t < a(e2) Then
					a(e3) = a(e2)
					a(e2) = t
					If t < a(e1) Then
						a(e2) = a(e1)
						a(e1) = t
					End If
				End If
			End If
			If a(e5) < a(e4) Then
				Dim t As Integer = a(e5)
				a(e5) = a(e4)
				a(e4) = t
				If t < a(e3) Then
					a(e4) = a(e3)
					a(e3) = t
					If t < a(e2) Then
						a(e3) = a(e2)
						a(e2) = t
						If t < a(e1) Then
							a(e2) = a(e1)
							a(e1) = t
						End If
					End If
				End If
			End If

			' Pointers
			Dim less As Integer = left ' The index of the first element of center part
			Dim great As Integer = right ' The index before the first element of right part

			If a(e1) <> a(e2) AndAlso a(e2) <> a(e3) AndAlso a(e3) <> a(e4) AndAlso a(e4) <> a(e5) Then
	'            
	'             * Use the second and fourth of the five sorted elements as pivots.
	'             * These values are inexpensive approximations of the first and
	'             * second terciles of the array. Note that pivot1 <= pivot2.
	'             
				Dim pivot1 As Integer = a(e2)
				Dim pivot2 As Integer = a(e4)

	'            
	'             * The first and the last elements to be sorted are moved to the
	'             * locations formerly occupied by the pivots. When partitioning
	'             * is complete, the pivots are swapped back into their final
	'             * positions, and excluded from subsequent sorting.
	'             
				a(e2) = a(left)
				a(e4) = a(right)

	'            
	'             * Skip elements, which are less or greater than pivot values.
	'             
				less += 1
				Do While a(less) < pivot1

					less += 1
				Loop
				great -= 1
				Do While a(great) > pivot2

					great -= 1
				Loop

	'            
	'             * Partitioning:
	'             *
	'             *   left part           center part                   right part
	'             * +--------------------------------------------------------------+
	'             * |  < pivot1  |  pivot1 <= && <= pivot2  |    ?    |  > pivot2  |
	'             * +--------------------------------------------------------------+
	'             *               ^                          ^       ^
	'             *               |                          |       |
	'             *              less                        k     great
	'             *
	'             * Invariants:
	'             *
	'             *              all in (left, less)   < pivot1
	'             *    pivot1 <= all in [less, k)     <= pivot2
	'             *              all in (great, right) > pivot2
	'             *
	'             * Pointer k is the first index of ?-part.
	'             
				outer:
				Dim k As Integer = less - 1
'JAVA TO VB CONVERTER TODO TASK: Assignments within expressions are not supported in VB
				Do While k += 1 <= great
					Dim ak As Integer = a(k)
					If ak < pivot1 Then ' Move a[k] to left part
						a(k) = a(less)
	'                    
	'                     * Here and below we use "a[i] = b; i++;" instead
	'                     * of "a[i++] = b;" due to performance issue.
	'                     
						a(less) = ak
						less += 1 ' Move a[k] to right part
					ElseIf ak > pivot2 Then
						Do While a(great) > pivot2
							Dim tempVar2 As Boolean = great = k
							great -= 1
							If tempVar2 Then GoTo outer
						Loop
						If a(great) < pivot1 Then ' a[great] <= pivot2
							a(k) = a(less)
							a(less) = a(great)
							less += 1 ' pivot1 <= a[great] <= pivot2
						Else
							a(k) = a(great)
						End If
	'                    
	'                     * Here and below we use "a[i] = b; i--;" instead
	'                     * of "a[i--] = b;" due to performance issue.
	'                     
						a(great) = ak
						great -= 1
					End If
				Loop

				' Swap pivots into their final positions
				a(left) = a(less - 1)
				a(less - 1) = pivot1
				a(right) = a(great + 1)
				a(great + 1) = pivot2

				' Sort left and right parts recursively, excluding known pivots
				sort(a, left, less - 2, leftmost)
				sort(a, great + 2, right, False)

	'            
	'             * If center part is too large (comprises > 4/7 of the array),
	'             * swap internal pivot values to ends.
	'             
				If less < e1 AndAlso e5 < great Then
	'                
	'                 * Skip elements, which are equal to pivot values.
	'                 
					Do While a(less) = pivot1
						less += 1
					Loop

					Do While a(great) = pivot2
						great -= 1
					Loop

	'                
	'                 * Partitioning:
	'                 *
	'                 *   left part         center part                  right part
	'                 * +----------------------------------------------------------+
	'                 * | == pivot1 |  pivot1 < && < pivot2  |    ?    | == pivot2 |
	'                 * +----------------------------------------------------------+
	'                 *              ^                        ^       ^
	'                 *              |                        |       |
	'                 *             less                      k     great
	'                 *
	'                 * Invariants:
	'                 *
	'                 *              all in (*,  less) == pivot1
	'                 *     pivot1 < all in [less,  k)  < pivot2
	'                 *              all in (great, *) == pivot2
	'                 *
	'                 * Pointer k is the first index of ?-part.
	'                 
					outer:
					k = less - 1
'JAVA TO VB CONVERTER TODO TASK: Assignments within expressions are not supported in VB
					Do While k += 1 <= great
						Dim ak As Integer = a(k)
						If ak = pivot1 Then ' Move a[k] to left part
							a(k) = a(less)
							a(less) = ak
							less += 1 ' Move a[k] to right part
						ElseIf ak = pivot2 Then
							Do While a(great) = pivot2
								Dim tempVar3 As Boolean = great = k
								great -= 1
								If tempVar3 Then GoTo outer
							Loop
							If a(great) = pivot1 Then ' a[great] < pivot2
								a(k) = a(less)
	'                            
	'                             * Even though a[great] equals to pivot1, the
	'                             * assignment a[less] = pivot1 may be incorrect,
	'                             * if a[great] and pivot1 are floating-point zeros
	'                             * of different signs. Therefore in float and
	'                             * double sorting methods we have to use more
	'                             * accurate assignment a[less] = a[great].
	'                             
								a(less) = pivot1
								less += 1 ' pivot1 < a[great] < pivot2
							Else
								a(k) = a(great)
							End If
							a(great) = ak
							great -= 1
						End If
					Loop
				End If

				' Sort center part recursively
				sort(a, less, great, False)
 ' Partitioning with one pivot
			Else
	'            
	'             * Use the third of the five sorted elements as pivot.
	'             * This value is inexpensive approximation of the median.
	'             
				Dim pivot As Integer = a(e3)

	'            
	'             * Partitioning degenerates to the traditional 3-way
	'             * (or "Dutch National Flag") schema:
	'             *
	'             *   left part    center part              right part
	'             * +-------------------------------------------------+
	'             * |  < pivot  |   == pivot   |     ?    |  > pivot  |
	'             * +-------------------------------------------------+
	'             *              ^              ^        ^
	'             *              |              |        |
	'             *             less            k      great
	'             *
	'             * Invariants:
	'             *
	'             *   all in (left, less)   < pivot
	'             *   all in [less, k)     == pivot
	'             *   all in (great, right) > pivot
	'             *
	'             * Pointer k is the first index of ?-part.
	'             
				Dim k As Integer = less
				Do While k <= great
					If a(k) = pivot Then
						k += 1
						Continue Do
					End If
					Dim ak As Integer = a(k)
					If ak < pivot Then ' Move a[k] to left part
						a(k) = a(less)
						a(less) = ak
						less += 1 ' a[k] > pivot - Move a[k] to right part
					Else
						Do While a(great) > pivot
							great -= 1
						Loop
						If a(great) < pivot Then ' a[great] <= pivot
							a(k) = a(less)
							a(less) = a(great)
							less += 1 ' a[great] == pivot
						Else
	'                        
	'                         * Even though a[great] equals to pivot, the
	'                         * assignment a[k] = pivot may be incorrect,
	'                         * if a[great] and pivot are floating-point
	'                         * zeros of different signs. Therefore in float
	'                         * and double sorting methods we have to use
	'                         * more accurate assignment a[k] = a[great].
	'                         
							a(k) = pivot
						End If
						a(great) = ak
						great -= 1
					End If
					k += 1
				Loop

	'            
	'             * Sort left and right parts recursively.
	'             * All elements from center part are equal
	'             * and, therefore, already sorted.
	'             
				sort(a, left, less - 1, leftmost)
				sort(a, great + 1, right, False)
			End If
		End Sub

		''' <summary>
		''' Sorts the specified range of the array using the given
		''' workspace array slice if possible for merging
		''' </summary>
		''' <param name="a"> the array to be sorted </param>
		''' <param name="left"> the index of the first element, inclusive, to be sorted </param>
		''' <param name="right"> the index of the last element, inclusive, to be sorted </param>
		''' <param name="work"> a workspace array (slice) </param>
		''' <param name="workBase"> origin of usable space in work array </param>
		''' <param name="workLen"> usable size of work array </param>
		Friend Shared Sub sort(ByVal a As Long(), ByVal left As Integer, ByVal right As Integer, ByVal work As Long(), ByVal workBase As Integer, ByVal workLen As Integer)
			' Use Quicksort on small arrays
			If right - left < QUICKSORT_THRESHOLD Then
				sort(a, left, right, True)
				Return
			End If

	'        
	'         * Index run[i] is the start of i-th run
	'         * (ascending or descending sequence).
	'         
			Dim run As Integer() = New Integer(MAX_RUN_COUNT){}
			Dim count As Integer = 0
			run(0) = left

			' Check if the array is nearly sorted
			Dim k As Integer = left
			Do While k < right
				If a(k) < a(k + 1) Then ' ascending
					k += 1
					Do While k <= right AndAlso a(k - 1) <= a(k)

						k += 1
					Loop ' descending
				ElseIf a(k) > a(k + 1) Then
					k += 1
					Do While k <= right AndAlso a(k - 1) >= a(k)

						k += 1
					Loop
					Dim lo As Integer = run(count) - 1
					Dim hi As Integer = k
'JAVA TO VB CONVERTER TODO TASK: Assignments within expressions are not supported in VB
					Do While lo += 1 < hi -= 1
						Dim t As Long = a(lo)
						a(lo) = a(hi)
						a(hi) = t
					Loop ' equal
				Else
					Dim m As Integer = MAX_RUN_LENGTH
'JAVA TO VB CONVERTER TODO TASK: Assignments within expressions are not supported in VB
					Do While k += 1 <= right AndAlso a(k - 1) = a(k)
						m -= 1
						If m = 0 Then
							sort(a, left, right, True)
							Return
						End If
					Loop
				End If

	'            
	'             * The array is not highly structured,
	'             * use Quicksort instead of merge sort.
	'             
				count += 1
				If count = MAX_RUN_COUNT Then
					sort(a, left, right, True)
					Return
				End If
				run(count) = k
			Loop

			' Check special cases
			' Implementation note: variable "right" is increased by 1.
			Then += 1
			If run(count) = rightThen ' The last run contains one element
				count += 1
				run(count) = right ' The array is already sorted
			ElseIf count = 1 Then
				Return
			End If

			' Determine alternation base for merge
			Dim odd As SByte = 0
			Dim n As Integer = 1
			Do While (n <<= 1) < count

				odd = odd Xor 1
			Loop

			' Use or create temporary array b for merging
			Dim b As Long() ' temp array; alternates with a
			Dim ao, bo As Integer ' array offsets from 'left'
			Dim blen As Integer = right - left ' space needed for b
			If work Is Nothing OrElse workLen < blen OrElse workBase + blen > work.Length Then
				work = New Long(blen - 1){}
				workBase = 0
			End If
			If odd = 0 Then
				Array.Copy(a, left, work, workBase, blen)
				b = a
				bo = 0
				a = work
				ao = workBase - left
			Else
				b = work
				ao = 0
				bo = workBase - left
			End If

			' Merging
			Dim last As Integer
			Do While count > 1
				k = (last = 0) + 2
'JAVA TO VB CONVERTER TODO TASK: Assignments within expressions are not supported in VB
				Do While k <= count
					Dim hi As Integer = run(k), mi As Integer = run(k - 1)
					Dim i As Integer = run(k - 2)
					Dim p As Integer = i
					Dim q As Integer = mi
					Do While i < hi
						If q >= hi OrElse p < mi AndAlso a(p + ao) <= a(q + ao) Then
							b(i + bo) = a(p + ao)
							p += 1
						Else
							b(i + bo) = a(q + ao)
							q += 1
						End If
						i += 1
					Loop
					last += 1
					run(last) = hi
					k += 2
				Loop
				If (count And 1) <> 0 Then
					Dim i As Integer = right
					Dim lo As Integer = run(count - 1)
'JAVA TO VB CONVERTER TODO TASK: Assignments within expressions are not supported in VB
					Do While i -= 1 >= lo

						b(i + bo) = a(i + ao)
					Loop
					last += 1
					run(last) = right
				End If
				Dim t As Long() = a
				a = b
				b = t
				Dim o As Integer = ao
				ao = bo
				bo = o
				count = last
			Loop
		End Sub

		''' <summary>
		''' Sorts the specified range of the array by Dual-Pivot Quicksort.
		''' </summary>
		''' <param name="a"> the array to be sorted </param>
		''' <param name="left"> the index of the first element, inclusive, to be sorted </param>
		''' <param name="right"> the index of the last element, inclusive, to be sorted </param>
		''' <param name="leftmost"> indicates if this part is the leftmost in the range </param>
		Private Shared Sub sort(ByVal a As Long(), ByVal left As Integer, ByVal right As Integer, ByVal leftmost As Boolean)
			Dim length As Integer = right - left + 1

			' Use insertion sort on tiny arrays
			If length < INSERTION_SORT_THRESHOLD Then
				If leftmost Then
	'                
	'                 * Traditional (without sentinel) insertion sort,
	'                 * optimized for server VM, is used in case of
	'                 * the leftmost part.
	'                 
					Dim i As Integer = left
					Dim j As Integer = i
					Do While i < right
						Dim ai As Long = a(i + 1)
						Do While ai < a(j)
							a(j + 1) = a(j)
							Dim tempVar As Boolean = j = left
							j -= 1
							If tempVar Then Exit Do
						Loop
						a(j + 1) = ai
						i += 1
j = i
					Loop
				Else
	'                
	'                 * Skip the longest ascending sequence.
	'                 
					Do
						If left >= right Then Return
						left += 1
					Loop While a(left) >= a(left - 1)

	'                
	'                 * Every element from adjoining part plays the role
	'                 * of sentinel, therefore this allows us to avoid the
	'                 * left range check on each iteration. Moreover, we use
	'                 * the more optimized algorithm, so called pair insertion
	'                 * sort, which is faster (in the context of Quicksort)
	'                 * than traditional implementation of insertion sort.
	'                 
					Dim k As Integer = left
'JAVA TO VB CONVERTER TODO TASK: Assignments within expressions are not supported in VB
					Do While left += 1 <= right
						Dim a1 As Long = a(k), a2 As Long = a(left)

						If a1 < a2 Then
							a2 = a1
							a1 = a(left)
						End If
						k -= 1
						Do While a1 < a(k)
							a(k + 2) = a(k)
							k -= 1
						Loop
						k += 1
						a(k + 1) = a1

						k -= 1
						Do While a2 < a(k)
							a(k + 1) = a(k)
							k -= 1
						Loop
						a(k + 1) = a2
						left += 1
k = left
					Loop
					Dim last As Long = a(right)

					right -= 1
					Do While last < a(right)
						a(right + 1) = a(right)
						right -= 1
					Loop
					a(right + 1) = last
				End If
				Return
			End If

			' Inexpensive approximation of length / 7
			Dim seventh As Integer = (length >> 3) + (length >> 6) + 1

	'        
	'         * Sort five evenly spaced elements around (and including) the
	'         * center element in the range. These elements will be used for
	'         * pivot selection as described below. The choice for spacing
	'         * these elements was empirically determined to work well on
	'         * a wide variety of inputs.
	'         
			Dim e3 As Integer = CInt(CUInt((left + right)) >> 1) ' The midpoint
			Dim e2 As Integer = e3 - seventh
			Dim e1 As Integer = e2 - seventh
			Dim e4 As Integer = e3 + seventh
			Dim e5 As Integer = e4 + seventh

			' Sort these elements using insertion sort
			If a(e2) < a(e1) Then
				Dim t As Long = a(e2)
				a(e2) = a(e1)
				a(e1) = t
			End If

			If a(e3) < a(e2) Then
				Dim t As Long = a(e3)
				a(e3) = a(e2)
				a(e2) = t
				If t < a(e1) Then
					a(e2) = a(e1)
					a(e1) = t
				End If
			End If
			If a(e4) < a(e3) Then
				Dim t As Long = a(e4)
				a(e4) = a(e3)
				a(e3) = t
				If t < a(e2) Then
					a(e3) = a(e2)
					a(e2) = t
					If t < a(e1) Then
						a(e2) = a(e1)
						a(e1) = t
					End If
				End If
			End If
			If a(e5) < a(e4) Then
				Dim t As Long = a(e5)
				a(e5) = a(e4)
				a(e4) = t
				If t < a(e3) Then
					a(e4) = a(e3)
					a(e3) = t
					If t < a(e2) Then
						a(e3) = a(e2)
						a(e2) = t
						If t < a(e1) Then
							a(e2) = a(e1)
							a(e1) = t
						End If
					End If
				End If
			End If

			' Pointers
			Dim less As Integer = left ' The index of the first element of center part
			Dim great As Integer = right ' The index before the first element of right part

			If a(e1) <> a(e2) AndAlso a(e2) <> a(e3) AndAlso a(e3) <> a(e4) AndAlso a(e4) <> a(e5) Then
	'            
	'             * Use the second and fourth of the five sorted elements as pivots.
	'             * These values are inexpensive approximations of the first and
	'             * second terciles of the array. Note that pivot1 <= pivot2.
	'             
				Dim pivot1 As Long = a(e2)
				Dim pivot2 As Long = a(e4)

	'            
	'             * The first and the last elements to be sorted are moved to the
	'             * locations formerly occupied by the pivots. When partitioning
	'             * is complete, the pivots are swapped back into their final
	'             * positions, and excluded from subsequent sorting.
	'             
				a(e2) = a(left)
				a(e4) = a(right)

	'            
	'             * Skip elements, which are less or greater than pivot values.
	'             
				less += 1
				Do While a(less) < pivot1

					less += 1
				Loop
				great -= 1
				Do While a(great) > pivot2

					great -= 1
				Loop

	'            
	'             * Partitioning:
	'             *
	'             *   left part           center part                   right part
	'             * +--------------------------------------------------------------+
	'             * |  < pivot1  |  pivot1 <= && <= pivot2  |    ?    |  > pivot2  |
	'             * +--------------------------------------------------------------+
	'             *               ^                          ^       ^
	'             *               |                          |       |
	'             *              less                        k     great
	'             *
	'             * Invariants:
	'             *
	'             *              all in (left, less)   < pivot1
	'             *    pivot1 <= all in [less, k)     <= pivot2
	'             *              all in (great, right) > pivot2
	'             *
	'             * Pointer k is the first index of ?-part.
	'             
				outer:
				Dim k As Integer = less - 1
'JAVA TO VB CONVERTER TODO TASK: Assignments within expressions are not supported in VB
				Do While k += 1 <= great
					Dim ak As Long = a(k)
					If ak < pivot1 Then ' Move a[k] to left part
						a(k) = a(less)
	'                    
	'                     * Here and below we use "a[i] = b; i++;" instead
	'                     * of "a[i++] = b;" due to performance issue.
	'                     
						a(less) = ak
						less += 1 ' Move a[k] to right part
					ElseIf ak > pivot2 Then
						Do While a(great) > pivot2
							Dim tempVar2 As Boolean = great = k
							great -= 1
							If tempVar2 Then GoTo outer
						Loop
						If a(great) < pivot1 Then ' a[great] <= pivot2
							a(k) = a(less)
							a(less) = a(great)
							less += 1 ' pivot1 <= a[great] <= pivot2
						Else
							a(k) = a(great)
						End If
	'                    
	'                     * Here and below we use "a[i] = b; i--;" instead
	'                     * of "a[i--] = b;" due to performance issue.
	'                     
						a(great) = ak
						great -= 1
					End If
				Loop

				' Swap pivots into their final positions
				a(left) = a(less - 1)
				a(less - 1) = pivot1
				a(right) = a(great + 1)
				a(great + 1) = pivot2

				' Sort left and right parts recursively, excluding known pivots
				sort(a, left, less - 2, leftmost)
				sort(a, great + 2, right, False)

	'            
	'             * If center part is too large (comprises > 4/7 of the array),
	'             * swap internal pivot values to ends.
	'             
				If less < e1 AndAlso e5 < great Then
	'                
	'                 * Skip elements, which are equal to pivot values.
	'                 
					Do While a(less) = pivot1
						less += 1
					Loop

					Do While a(great) = pivot2
						great -= 1
					Loop

	'                
	'                 * Partitioning:
	'                 *
	'                 *   left part         center part                  right part
	'                 * +----------------------------------------------------------+
	'                 * | == pivot1 |  pivot1 < && < pivot2  |    ?    | == pivot2 |
	'                 * +----------------------------------------------------------+
	'                 *              ^                        ^       ^
	'                 *              |                        |       |
	'                 *             less                      k     great
	'                 *
	'                 * Invariants:
	'                 *
	'                 *              all in (*,  less) == pivot1
	'                 *     pivot1 < all in [less,  k)  < pivot2
	'                 *              all in (great, *) == pivot2
	'                 *
	'                 * Pointer k is the first index of ?-part.
	'                 
					outer:
					k = less - 1
'JAVA TO VB CONVERTER TODO TASK: Assignments within expressions are not supported in VB
					Do While k += 1 <= great
						Dim ak As Long = a(k)
						If ak = pivot1 Then ' Move a[k] to left part
							a(k) = a(less)
							a(less) = ak
							less += 1 ' Move a[k] to right part
						ElseIf ak = pivot2 Then
							Do While a(great) = pivot2
								Dim tempVar3 As Boolean = great = k
								great -= 1
								If tempVar3 Then GoTo outer
							Loop
							If a(great) = pivot1 Then ' a[great] < pivot2
								a(k) = a(less)
	'                            
	'                             * Even though a[great] equals to pivot1, the
	'                             * assignment a[less] = pivot1 may be incorrect,
	'                             * if a[great] and pivot1 are floating-point zeros
	'                             * of different signs. Therefore in float and
	'                             * double sorting methods we have to use more
	'                             * accurate assignment a[less] = a[great].
	'                             
								a(less) = pivot1
								less += 1 ' pivot1 < a[great] < pivot2
							Else
								a(k) = a(great)
							End If
							a(great) = ak
							great -= 1
						End If
					Loop
				End If

				' Sort center part recursively
				sort(a, less, great, False)
 ' Partitioning with one pivot
			Else
	'            
	'             * Use the third of the five sorted elements as pivot.
	'             * This value is inexpensive approximation of the median.
	'             
				Dim pivot As Long = a(e3)

	'            
	'             * Partitioning degenerates to the traditional 3-way
	'             * (or "Dutch National Flag") schema:
	'             *
	'             *   left part    center part              right part
	'             * +-------------------------------------------------+
	'             * |  < pivot  |   == pivot   |     ?    |  > pivot  |
	'             * +-------------------------------------------------+
	'             *              ^              ^        ^
	'             *              |              |        |
	'             *             less            k      great
	'             *
	'             * Invariants:
	'             *
	'             *   all in (left, less)   < pivot
	'             *   all in [less, k)     == pivot
	'             *   all in (great, right) > pivot
	'             *
	'             * Pointer k is the first index of ?-part.
	'             
				Dim k As Integer = less
				Do While k <= great
					If a(k) = pivot Then
						k += 1
						Continue Do
					End If
					Dim ak As Long = a(k)
					If ak < pivot Then ' Move a[k] to left part
						a(k) = a(less)
						a(less) = ak
						less += 1 ' a[k] > pivot - Move a[k] to right part
					Else
						Do While a(great) > pivot
							great -= 1
						Loop
						If a(great) < pivot Then ' a[great] <= pivot
							a(k) = a(less)
							a(less) = a(great)
							less += 1 ' a[great] == pivot
						Else
	'                        
	'                         * Even though a[great] equals to pivot, the
	'                         * assignment a[k] = pivot may be incorrect,
	'                         * if a[great] and pivot are floating-point
	'                         * zeros of different signs. Therefore in float
	'                         * and double sorting methods we have to use
	'                         * more accurate assignment a[k] = a[great].
	'                         
							a(k) = pivot
						End If
						a(great) = ak
						great -= 1
					End If
					k += 1
				Loop

	'            
	'             * Sort left and right parts recursively.
	'             * All elements from center part are equal
	'             * and, therefore, already sorted.
	'             
				sort(a, left, less - 1, leftmost)
				sort(a, great + 1, right, False)
			End If
		End Sub

		''' <summary>
		''' Sorts the specified range of the array using the given
		''' workspace array slice if possible for merging
		''' </summary>
		''' <param name="a"> the array to be sorted </param>
		''' <param name="left"> the index of the first element, inclusive, to be sorted </param>
		''' <param name="right"> the index of the last element, inclusive, to be sorted </param>
		''' <param name="work"> a workspace array (slice) </param>
		''' <param name="workBase"> origin of usable space in work array </param>
		''' <param name="workLen"> usable size of work array </param>
		Friend Shared Sub sort(ByVal a As Short(), ByVal left As Integer, ByVal right As Integer, ByVal work As Short(), ByVal workBase As Integer, ByVal workLen As Integer)
			' Use counting sort on large arrays
			If right - left > COUNTING_SORT_THRESHOLD_FOR_SHORT_OR_CHAR Then
				Dim count As Integer() = New Integer(NUM_SHORT_VALUES - 1){}

				Dim i As Integer = left - 1
'JAVA TO VB CONVERTER TODO TASK: Assignments within expressions are not supported in VB
				Do While i += 1 <= right

					count(a(i) - Short.MinValue) += 1
				Loop
				i = NUM_SHORT_VALUES
				Dim k As Integer = right + 1
				Do While k > left
					i -= 1
					Do While count(i) = 0

						i -= 1
					Loop
					Dim value As Short = CShort(Fix(i + Short.MinValue))
					Dim s As Integer = count(i)

					Do
						k -= 1
						a(k) = value
						s -= 1
					Loop While s > 0
				Loop ' Use Dual-Pivot Quicksort on small arrays
			Else
				doSort(a, left, right, work, workBase, workLen)
			End If
		End Sub

		''' <summary>
		''' The number of distinct short values. </summary>
		Private Shared ReadOnly NUM_SHORT_VALUES As Integer = 1 << 16

		''' <summary>
		''' Sorts the specified range of the array.
		''' </summary>
		''' <param name="a"> the array to be sorted </param>
		''' <param name="left"> the index of the first element, inclusive, to be sorted </param>
		''' <param name="right"> the index of the last element, inclusive, to be sorted </param>
		''' <param name="work"> a workspace array (slice) </param>
		''' <param name="workBase"> origin of usable space in work array </param>
		''' <param name="workLen"> usable size of work array </param>
		Private Shared Sub doSort(ByVal a As Short(), ByVal left As Integer, ByVal right As Integer, ByVal work As Short(), ByVal workBase As Integer, ByVal workLen As Integer)
			' Use Quicksort on small arrays
			If right - left < QUICKSORT_THRESHOLD Then
				sort(a, left, right, True)
				Return
			End If

	'        
	'         * Index run[i] is the start of i-th run
	'         * (ascending or descending sequence).
	'         
			Dim run As Integer() = New Integer(MAX_RUN_COUNT){}
			Dim count As Integer = 0
			run(0) = left

			' Check if the array is nearly sorted
			Dim k As Integer = left
			Do While k < right
				If a(k) < a(k + 1) Then ' ascending
					k += 1
					Do While k <= right AndAlso a(k - 1) <= a(k)

						k += 1
					Loop ' descending
				ElseIf a(k) > a(k + 1) Then
					k += 1
					Do While k <= right AndAlso a(k - 1) >= a(k)

						k += 1
					Loop
					Dim lo As Integer = run(count) - 1
					Dim hi As Integer = k
'JAVA TO VB CONVERTER TODO TASK: Assignments within expressions are not supported in VB
					Do While lo += 1 < hi -= 1
						Dim t As Short = a(lo)
						a(lo) = a(hi)
						a(hi) = t
					Loop ' equal
				Else
					Dim m As Integer = MAX_RUN_LENGTH
'JAVA TO VB CONVERTER TODO TASK: Assignments within expressions are not supported in VB
					Do While k += 1 <= right AndAlso a(k - 1) = a(k)
						m -= 1
						If m = 0 Then
							sort(a, left, right, True)
							Return
						End If
					Loop
				End If

	'            
	'             * The array is not highly structured,
	'             * use Quicksort instead of merge sort.
	'             
				count += 1
				If count = MAX_RUN_COUNT Then
					sort(a, left, right, True)
					Return
				End If
				run(count) = k
			Loop

			' Check special cases
			' Implementation note: variable "right" is increased by 1.
			Then += 1
			If run(count) = rightThen ' The last run contains one element
				count += 1
				run(count) = right ' The array is already sorted
			ElseIf count = 1 Then
				Return
			End If

			' Determine alternation base for merge
			Dim odd As SByte = 0
			Dim n As Integer = 1
			Do While (n <<= 1) < count

				odd = odd Xor 1
			Loop

			' Use or create temporary array b for merging
			Dim b As Short() ' temp array; alternates with a
			Dim ao, bo As Integer ' array offsets from 'left'
			Dim blen As Integer = right - left ' space needed for b
			If work Is Nothing OrElse workLen < blen OrElse workBase + blen > work.Length Then
				work = New Short(blen - 1){}
				workBase = 0
			End If
			If odd = 0 Then
				Array.Copy(a, left, work, workBase, blen)
				b = a
				bo = 0
				a = work
				ao = workBase - left
			Else
				b = work
				ao = 0
				bo = workBase - left
			End If

			' Merging
			Dim last As Integer
			Do While count > 1
				k = (last = 0) + 2
'JAVA TO VB CONVERTER TODO TASK: Assignments within expressions are not supported in VB
				Do While k <= count
					Dim hi As Integer = run(k), mi As Integer = run(k - 1)
					Dim i As Integer = run(k - 2)
					Dim p As Integer = i
					Dim q As Integer = mi
					Do While i < hi
						If q >= hi OrElse p < mi AndAlso a(p + ao) <= a(q + ao) Then
							b(i + bo) = a(p + ao)
							p += 1
						Else
							b(i + bo) = a(q + ao)
							q += 1
						End If
						i += 1
					Loop
					last += 1
					run(last) = hi
					k += 2
				Loop
				If (count And 1) <> 0 Then
					Dim i As Integer = right
					Dim lo As Integer = run(count - 1)
'JAVA TO VB CONVERTER TODO TASK: Assignments within expressions are not supported in VB
					Do While i -= 1 >= lo

						b(i + bo) = a(i + ao)
					Loop
					last += 1
					run(last) = right
				End If
				Dim t As Short() = a
				a = b
				b = t
				Dim o As Integer = ao
				ao = bo
				bo = o
				count = last
			Loop
		End Sub

		''' <summary>
		''' Sorts the specified range of the array by Dual-Pivot Quicksort.
		''' </summary>
		''' <param name="a"> the array to be sorted </param>
		''' <param name="left"> the index of the first element, inclusive, to be sorted </param>
		''' <param name="right"> the index of the last element, inclusive, to be sorted </param>
		''' <param name="leftmost"> indicates if this part is the leftmost in the range </param>
		Private Shared Sub sort(ByVal a As Short(), ByVal left As Integer, ByVal right As Integer, ByVal leftmost As Boolean)
			Dim length As Integer = right - left + 1

			' Use insertion sort on tiny arrays
			If length < INSERTION_SORT_THRESHOLD Then
				If leftmost Then
	'                
	'                 * Traditional (without sentinel) insertion sort,
	'                 * optimized for server VM, is used in case of
	'                 * the leftmost part.
	'                 
					Dim i As Integer = left
					Dim j As Integer = i
					Do While i < right
						Dim ai As Short = a(i + 1)
						Do While ai < a(j)
							a(j + 1) = a(j)
							Dim tempVar As Boolean = j = left
							j -= 1
							If tempVar Then Exit Do
						Loop
						a(j + 1) = ai
						i += 1
j = i
					Loop
				Else
	'                
	'                 * Skip the longest ascending sequence.
	'                 
					Do
						If left >= right Then Return
						left += 1
					Loop While a(left) >= a(left - 1)

	'                
	'                 * Every element from adjoining part plays the role
	'                 * of sentinel, therefore this allows us to avoid the
	'                 * left range check on each iteration. Moreover, we use
	'                 * the more optimized algorithm, so called pair insertion
	'                 * sort, which is faster (in the context of Quicksort)
	'                 * than traditional implementation of insertion sort.
	'                 
					Dim k As Integer = left
'JAVA TO VB CONVERTER TODO TASK: Assignments within expressions are not supported in VB
					Do While left += 1 <= right
						Dim a1 As Short = a(k), a2 As Short = a(left)

						If a1 < a2 Then
							a2 = a1
							a1 = a(left)
						End If
						k -= 1
						Do While a1 < a(k)
							a(k + 2) = a(k)
							k -= 1
						Loop
						k += 1
						a(k + 1) = a1

						k -= 1
						Do While a2 < a(k)
							a(k + 1) = a(k)
							k -= 1
						Loop
						a(k + 1) = a2
						left += 1
k = left
					Loop
					Dim last As Short = a(right)

					right -= 1
					Do While last < a(right)
						a(right + 1) = a(right)
						right -= 1
					Loop
					a(right + 1) = last
				End If
				Return
			End If

			' Inexpensive approximation of length / 7
			Dim seventh As Integer = (length >> 3) + (length >> 6) + 1

	'        
	'         * Sort five evenly spaced elements around (and including) the
	'         * center element in the range. These elements will be used for
	'         * pivot selection as described below. The choice for spacing
	'         * these elements was empirically determined to work well on
	'         * a wide variety of inputs.
	'         
			Dim e3 As Integer = CInt(CUInt((left + right)) >> 1) ' The midpoint
			Dim e2 As Integer = e3 - seventh
			Dim e1 As Integer = e2 - seventh
			Dim e4 As Integer = e3 + seventh
			Dim e5 As Integer = e4 + seventh

			' Sort these elements using insertion sort
			If a(e2) < a(e1) Then
				Dim t As Short = a(e2)
				a(e2) = a(e1)
				a(e1) = t
			End If

			If a(e3) < a(e2) Then
				Dim t As Short = a(e3)
				a(e3) = a(e2)
				a(e2) = t
				If t < a(e1) Then
					a(e2) = a(e1)
					a(e1) = t
				End If
			End If
			If a(e4) < a(e3) Then
				Dim t As Short = a(e4)
				a(e4) = a(e3)
				a(e3) = t
				If t < a(e2) Then
					a(e3) = a(e2)
					a(e2) = t
					If t < a(e1) Then
						a(e2) = a(e1)
						a(e1) = t
					End If
				End If
			End If
			If a(e5) < a(e4) Then
				Dim t As Short = a(e5)
				a(e5) = a(e4)
				a(e4) = t
				If t < a(e3) Then
					a(e4) = a(e3)
					a(e3) = t
					If t < a(e2) Then
						a(e3) = a(e2)
						a(e2) = t
						If t < a(e1) Then
							a(e2) = a(e1)
							a(e1) = t
						End If
					End If
				End If
			End If

			' Pointers
			Dim less As Integer = left ' The index of the first element of center part
			Dim great As Integer = right ' The index before the first element of right part

			If a(e1) <> a(e2) AndAlso a(e2) <> a(e3) AndAlso a(e3) <> a(e4) AndAlso a(e4) <> a(e5) Then
	'            
	'             * Use the second and fourth of the five sorted elements as pivots.
	'             * These values are inexpensive approximations of the first and
	'             * second terciles of the array. Note that pivot1 <= pivot2.
	'             
				Dim pivot1 As Short = a(e2)
				Dim pivot2 As Short = a(e4)

	'            
	'             * The first and the last elements to be sorted are moved to the
	'             * locations formerly occupied by the pivots. When partitioning
	'             * is complete, the pivots are swapped back into their final
	'             * positions, and excluded from subsequent sorting.
	'             
				a(e2) = a(left)
				a(e4) = a(right)

	'            
	'             * Skip elements, which are less or greater than pivot values.
	'             
				less += 1
				Do While a(less) < pivot1

					less += 1
				Loop
				great -= 1
				Do While a(great) > pivot2

					great -= 1
				Loop

	'            
	'             * Partitioning:
	'             *
	'             *   left part           center part                   right part
	'             * +--------------------------------------------------------------+
	'             * |  < pivot1  |  pivot1 <= && <= pivot2  |    ?    |  > pivot2  |
	'             * +--------------------------------------------------------------+
	'             *               ^                          ^       ^
	'             *               |                          |       |
	'             *              less                        k     great
	'             *
	'             * Invariants:
	'             *
	'             *              all in (left, less)   < pivot1
	'             *    pivot1 <= all in [less, k)     <= pivot2
	'             *              all in (great, right) > pivot2
	'             *
	'             * Pointer k is the first index of ?-part.
	'             
				outer:
				Dim k As Integer = less - 1
'JAVA TO VB CONVERTER TODO TASK: Assignments within expressions are not supported in VB
				Do While k += 1 <= great
					Dim ak As Short = a(k)
					If ak < pivot1 Then ' Move a[k] to left part
						a(k) = a(less)
	'                    
	'                     * Here and below we use "a[i] = b; i++;" instead
	'                     * of "a[i++] = b;" due to performance issue.
	'                     
						a(less) = ak
						less += 1 ' Move a[k] to right part
					ElseIf ak > pivot2 Then
						Do While a(great) > pivot2
							Dim tempVar2 As Boolean = great = k
							great -= 1
							If tempVar2 Then GoTo outer
						Loop
						If a(great) < pivot1 Then ' a[great] <= pivot2
							a(k) = a(less)
							a(less) = a(great)
							less += 1 ' pivot1 <= a[great] <= pivot2
						Else
							a(k) = a(great)
						End If
	'                    
	'                     * Here and below we use "a[i] = b; i--;" instead
	'                     * of "a[i--] = b;" due to performance issue.
	'                     
						a(great) = ak
						great -= 1
					End If
				Loop

				' Swap pivots into their final positions
				a(left) = a(less - 1)
				a(less - 1) = pivot1
				a(right) = a(great + 1)
				a(great + 1) = pivot2

				' Sort left and right parts recursively, excluding known pivots
				sort(a, left, less - 2, leftmost)
				sort(a, great + 2, right, False)

	'            
	'             * If center part is too large (comprises > 4/7 of the array),
	'             * swap internal pivot values to ends.
	'             
				If less < e1 AndAlso e5 < great Then
	'                
	'                 * Skip elements, which are equal to pivot values.
	'                 
					Do While a(less) = pivot1
						less += 1
					Loop

					Do While a(great) = pivot2
						great -= 1
					Loop

	'                
	'                 * Partitioning:
	'                 *
	'                 *   left part         center part                  right part
	'                 * +----------------------------------------------------------+
	'                 * | == pivot1 |  pivot1 < && < pivot2  |    ?    | == pivot2 |
	'                 * +----------------------------------------------------------+
	'                 *              ^                        ^       ^
	'                 *              |                        |       |
	'                 *             less                      k     great
	'                 *
	'                 * Invariants:
	'                 *
	'                 *              all in (*,  less) == pivot1
	'                 *     pivot1 < all in [less,  k)  < pivot2
	'                 *              all in (great, *) == pivot2
	'                 *
	'                 * Pointer k is the first index of ?-part.
	'                 
					outer:
					k = less - 1
'JAVA TO VB CONVERTER TODO TASK: Assignments within expressions are not supported in VB
					Do While k += 1 <= great
						Dim ak As Short = a(k)
						If ak = pivot1 Then ' Move a[k] to left part
							a(k) = a(less)
							a(less) = ak
							less += 1 ' Move a[k] to right part
						ElseIf ak = pivot2 Then
							Do While a(great) = pivot2
								Dim tempVar3 As Boolean = great = k
								great -= 1
								If tempVar3 Then GoTo outer
							Loop
							If a(great) = pivot1 Then ' a[great] < pivot2
								a(k) = a(less)
	'                            
	'                             * Even though a[great] equals to pivot1, the
	'                             * assignment a[less] = pivot1 may be incorrect,
	'                             * if a[great] and pivot1 are floating-point zeros
	'                             * of different signs. Therefore in float and
	'                             * double sorting methods we have to use more
	'                             * accurate assignment a[less] = a[great].
	'                             
								a(less) = pivot1
								less += 1 ' pivot1 < a[great] < pivot2
							Else
								a(k) = a(great)
							End If
							a(great) = ak
							great -= 1
						End If
					Loop
				End If

				' Sort center part recursively
				sort(a, less, great, False)
 ' Partitioning with one pivot
			Else
	'            
	'             * Use the third of the five sorted elements as pivot.
	'             * This value is inexpensive approximation of the median.
	'             
				Dim pivot As Short = a(e3)

	'            
	'             * Partitioning degenerates to the traditional 3-way
	'             * (or "Dutch National Flag") schema:
	'             *
	'             *   left part    center part              right part
	'             * +-------------------------------------------------+
	'             * |  < pivot  |   == pivot   |     ?    |  > pivot  |
	'             * +-------------------------------------------------+
	'             *              ^              ^        ^
	'             *              |              |        |
	'             *             less            k      great
	'             *
	'             * Invariants:
	'             *
	'             *   all in (left, less)   < pivot
	'             *   all in [less, k)     == pivot
	'             *   all in (great, right) > pivot
	'             *
	'             * Pointer k is the first index of ?-part.
	'             
				Dim k As Integer = less
				Do While k <= great
					If a(k) = pivot Then
						k += 1
						Continue Do
					End If
					Dim ak As Short = a(k)
					If ak < pivot Then ' Move a[k] to left part
						a(k) = a(less)
						a(less) = ak
						less += 1 ' a[k] > pivot - Move a[k] to right part
					Else
						Do While a(great) > pivot
							great -= 1
						Loop
						If a(great) < pivot Then ' a[great] <= pivot
							a(k) = a(less)
							a(less) = a(great)
							less += 1 ' a[great] == pivot
						Else
	'                        
	'                         * Even though a[great] equals to pivot, the
	'                         * assignment a[k] = pivot may be incorrect,
	'                         * if a[great] and pivot are floating-point
	'                         * zeros of different signs. Therefore in float
	'                         * and double sorting methods we have to use
	'                         * more accurate assignment a[k] = a[great].
	'                         
							a(k) = pivot
						End If
						a(great) = ak
						great -= 1
					End If
					k += 1
				Loop

	'            
	'             * Sort left and right parts recursively.
	'             * All elements from center part are equal
	'             * and, therefore, already sorted.
	'             
				sort(a, left, less - 1, leftmost)
				sort(a, great + 1, right, False)
			End If
		End Sub

		''' <summary>
		''' Sorts the specified range of the array using the given
		''' workspace array slice if possible for merging
		''' </summary>
		''' <param name="a"> the array to be sorted </param>
		''' <param name="left"> the index of the first element, inclusive, to be sorted </param>
		''' <param name="right"> the index of the last element, inclusive, to be sorted </param>
		''' <param name="work"> a workspace array (slice) </param>
		''' <param name="workBase"> origin of usable space in work array </param>
		''' <param name="workLen"> usable size of work array </param>
		Friend Shared Sub sort(ByVal a As Char(), ByVal left As Integer, ByVal right As Integer, ByVal work As Char(), ByVal workBase As Integer, ByVal workLen As Integer)
			' Use counting sort on large arrays
			If right - left > COUNTING_SORT_THRESHOLD_FOR_SHORT_OR_CHAR Then
				Dim count As Integer() = New Integer(NUM_CHAR_VALUES - 1){}

				Dim i As Integer = left - 1
'JAVA TO VB CONVERTER TODO TASK: Assignments within expressions are not supported in VB
				Do While i += 1 <= right

					count(AscW(a(i))) += 1
				Loop
				i = NUM_CHAR_VALUES
				Dim k As Integer = right + 1
				Do While k > left
					i -= 1
					Do While count(i) = 0

						i -= 1
					Loop
					Dim value As Char = ChrW(i)
					Dim s As Integer = count(i)

					Do
						k -= 1
						a(k) = value
						s -= 1
					Loop While s > 0
				Loop ' Use Dual-Pivot Quicksort on small arrays
			Else
				doSort(a, left, right, work, workBase, workLen)
			End If
		End Sub

		''' <summary>
		''' The number of distinct char values. </summary>
		Private Shared ReadOnly NUM_CHAR_VALUES As Integer = 1 << 16

		''' <summary>
		''' Sorts the specified range of the array.
		''' </summary>
		''' <param name="a"> the array to be sorted </param>
		''' <param name="left"> the index of the first element, inclusive, to be sorted </param>
		''' <param name="right"> the index of the last element, inclusive, to be sorted </param>
		''' <param name="work"> a workspace array (slice) </param>
		''' <param name="workBase"> origin of usable space in work array </param>
		''' <param name="workLen"> usable size of work array </param>
		Private Shared Sub doSort(ByVal a As Char(), ByVal left As Integer, ByVal right As Integer, ByVal work As Char(), ByVal workBase As Integer, ByVal workLen As Integer)
			' Use Quicksort on small arrays
			If right - left < QUICKSORT_THRESHOLD Then
				sort(a, left, right, True)
				Return
			End If

	'        
	'         * Index run[i] is the start of i-th run
	'         * (ascending or descending sequence).
	'         
			Dim run As Integer() = New Integer(MAX_RUN_COUNT){}
			Dim count As Integer = 0
			run(0) = left

			' Check if the array is nearly sorted
			Dim k As Integer = left
			Do While k < right
				If a(k) < a(k + 1) Then ' ascending
					k += 1
					Do While k <= right AndAlso a(k - 1) <= a(k)

						k += 1
					Loop ' descending
				ElseIf a(k) > a(k + 1) Then
					k += 1
					Do While k <= right AndAlso a(k - 1) >= a(k)

						k += 1
					Loop
					Dim lo As Integer = run(count) - 1
					Dim hi As Integer = k
'JAVA TO VB CONVERTER TODO TASK: Assignments within expressions are not supported in VB
					Do While lo += 1 < hi -= 1
						Dim t As Char = a(lo)
						a(lo) = a(hi)
						a(hi) = t
					Loop ' equal
				Else
					Dim m As Integer = MAX_RUN_LENGTH
'JAVA TO VB CONVERTER TODO TASK: Assignments within expressions are not supported in VB
					Do While k += 1 <= right AndAlso a(k - 1) = a(k)
						m -= 1
						If m = 0 Then
							sort(a, left, right, True)
							Return
						End If
					Loop
				End If

	'            
	'             * The array is not highly structured,
	'             * use Quicksort instead of merge sort.
	'             
				count += 1
				If count = MAX_RUN_COUNT Then
					sort(a, left, right, True)
					Return
				End If
				run(count) = k
			Loop

			' Check special cases
			' Implementation note: variable "right" is increased by 1.
			Then += 1
			If run(count) = rightThen ' The last run contains one element
				count += 1
				run(count) = right ' The array is already sorted
			ElseIf count = 1 Then
				Return
			End If

			' Determine alternation base for merge
			Dim odd As SByte = 0
			Dim n As Integer = 1
			Do While (n <<= 1) < count

				odd = odd Xor 1
			Loop

			' Use or create temporary array b for merging
			Dim b As Char() ' temp array; alternates with a
			Dim ao, bo As Integer ' array offsets from 'left'
			Dim blen As Integer = right - left ' space needed for b
			If work Is Nothing OrElse workLen < blen OrElse workBase + blen > work.Length Then
				work = New Char(blen - 1){}
				workBase = 0
			End If
			If odd = 0 Then
				Array.Copy(a, left, work, workBase, blen)
				b = a
				bo = 0
				a = work
				ao = workBase - left
			Else
				b = work
				ao = 0
				bo = workBase - left
			End If

			' Merging
			Dim last As Integer
			Do While count > 1
				k = (last = 0) + 2
'JAVA TO VB CONVERTER TODO TASK: Assignments within expressions are not supported in VB
				Do While k <= count
					Dim hi As Integer = run(k), mi As Integer = run(k - 1)
					Dim i As Integer = run(k - 2)
					Dim p As Integer = i
					Dim q As Integer = mi
					Do While i < hi
						If q >= hi OrElse p < mi AndAlso a(p + ao) <= a(q + ao) Then
							b(i + bo) = a(p + ao)
							p += 1
						Else
							b(i + bo) = a(q + ao)
							q += 1
						End If
						i += 1
					Loop
					last += 1
					run(last) = hi
					k += 2
				Loop
				If (count And 1) <> 0 Then
					Dim i As Integer = right
					Dim lo As Integer = run(count - 1)
'JAVA TO VB CONVERTER TODO TASK: Assignments within expressions are not supported in VB
					Do While i -= 1 >= lo

						b(i + bo) = a(i + ao)
					Loop
					last += 1
					run(last) = right
				End If
				Dim t As Char() = a
				a = b
				b = t
				Dim o As Integer = ao
				ao = bo
				bo = o
				count = last
			Loop
		End Sub

		''' <summary>
		''' Sorts the specified range of the array by Dual-Pivot Quicksort.
		''' </summary>
		''' <param name="a"> the array to be sorted </param>
		''' <param name="left"> the index of the first element, inclusive, to be sorted </param>
		''' <param name="right"> the index of the last element, inclusive, to be sorted </param>
		''' <param name="leftmost"> indicates if this part is the leftmost in the range </param>
		Private Shared Sub sort(ByVal a As Char(), ByVal left As Integer, ByVal right As Integer, ByVal leftmost As Boolean)
			Dim length As Integer = right - left + 1

			' Use insertion sort on tiny arrays
			If length < INSERTION_SORT_THRESHOLD Then
				If leftmost Then
	'                
	'                 * Traditional (without sentinel) insertion sort,
	'                 * optimized for server VM, is used in case of
	'                 * the leftmost part.
	'                 
					Dim i As Integer = left
					Dim j As Integer = i
					Do While i < right
						Dim ai As Char = a(i + 1)
						Do While ai < a(j)
							a(j + 1) = a(j)
							Dim tempVar As Boolean = j = left
							j -= 1
							If tempVar Then Exit Do
						Loop
						a(j + 1) = ai
						i += 1
j = i
					Loop
				Else
	'                
	'                 * Skip the longest ascending sequence.
	'                 
					Do
						If left >= right Then Return
						left += 1
					Loop While a(left) >= a(left - 1)

	'                
	'                 * Every element from adjoining part plays the role
	'                 * of sentinel, therefore this allows us to avoid the
	'                 * left range check on each iteration. Moreover, we use
	'                 * the more optimized algorithm, so called pair insertion
	'                 * sort, which is faster (in the context of Quicksort)
	'                 * than traditional implementation of insertion sort.
	'                 
					Dim k As Integer = left
'JAVA TO VB CONVERTER TODO TASK: Assignments within expressions are not supported in VB
					Do While left += 1 <= right
						Dim a1 As Char = a(k), a2 As Char = a(left)

						If a1 < a2 Then
							a2 = a1
							a1 = a(left)
						End If
						k -= 1
						Do While a1 < a(k)
							a(k + 2) = a(k)
							k -= 1
						Loop
						k += 1
						a(k + 1) = a1

						k -= 1
						Do While a2 < a(k)
							a(k + 1) = a(k)
							k -= 1
						Loop
						a(k + 1) = a2
						left += 1
k = left
					Loop
					Dim last As Char = a(right)

					right -= 1
					Do While last < a(right)
						a(right + 1) = a(right)
						right -= 1
					Loop
					a(right + 1) = last
				End If
				Return
			End If

			' Inexpensive approximation of length / 7
			Dim seventh As Integer = (length >> 3) + (length >> 6) + 1

	'        
	'         * Sort five evenly spaced elements around (and including) the
	'         * center element in the range. These elements will be used for
	'         * pivot selection as described below. The choice for spacing
	'         * these elements was empirically determined to work well on
	'         * a wide variety of inputs.
	'         
			Dim e3 As Integer = CInt(CUInt((left + right)) >> 1) ' The midpoint
			Dim e2 As Integer = e3 - seventh
			Dim e1 As Integer = e2 - seventh
			Dim e4 As Integer = e3 + seventh
			Dim e5 As Integer = e4 + seventh

			' Sort these elements using insertion sort
			If a(e2) < a(e1) Then
				Dim t As Char = a(e2)
				a(e2) = a(e1)
				a(e1) = t
			End If

			If a(e3) < a(e2) Then
				Dim t As Char = a(e3)
				a(e3) = a(e2)
				a(e2) = t
				If t < a(e1) Then
					a(e2) = a(e1)
					a(e1) = t
				End If
			End If
			If a(e4) < a(e3) Then
				Dim t As Char = a(e4)
				a(e4) = a(e3)
				a(e3) = t
				If t < a(e2) Then
					a(e3) = a(e2)
					a(e2) = t
					If t < a(e1) Then
						a(e2) = a(e1)
						a(e1) = t
					End If
				End If
			End If
			If a(e5) < a(e4) Then
				Dim t As Char = a(e5)
				a(e5) = a(e4)
				a(e4) = t
				If t < a(e3) Then
					a(e4) = a(e3)
					a(e3) = t
					If t < a(e2) Then
						a(e3) = a(e2)
						a(e2) = t
						If t < a(e1) Then
							a(e2) = a(e1)
							a(e1) = t
						End If
					End If
				End If
			End If

			' Pointers
			Dim less As Integer = left ' The index of the first element of center part
			Dim great As Integer = right ' The index before the first element of right part

			If a(e1) <> a(e2) AndAlso a(e2) <> a(e3) AndAlso a(e3) <> a(e4) AndAlso a(e4) <> a(e5) Then
	'            
	'             * Use the second and fourth of the five sorted elements as pivots.
	'             * These values are inexpensive approximations of the first and
	'             * second terciles of the array. Note that pivot1 <= pivot2.
	'             
				Dim pivot1 As Char = a(e2)
				Dim pivot2 As Char = a(e4)

	'            
	'             * The first and the last elements to be sorted are moved to the
	'             * locations formerly occupied by the pivots. When partitioning
	'             * is complete, the pivots are swapped back into their final
	'             * positions, and excluded from subsequent sorting.
	'             
				a(e2) = a(left)
				a(e4) = a(right)

	'            
	'             * Skip elements, which are less or greater than pivot values.
	'             
				less += 1
				Do While a(less) < pivot1

					less += 1
				Loop
				great -= 1
				Do While a(great) > pivot2

					great -= 1
				Loop

	'            
	'             * Partitioning:
	'             *
	'             *   left part           center part                   right part
	'             * +--------------------------------------------------------------+
	'             * |  < pivot1  |  pivot1 <= && <= pivot2  |    ?    |  > pivot2  |
	'             * +--------------------------------------------------------------+
	'             *               ^                          ^       ^
	'             *               |                          |       |
	'             *              less                        k     great
	'             *
	'             * Invariants:
	'             *
	'             *              all in (left, less)   < pivot1
	'             *    pivot1 <= all in [less, k)     <= pivot2
	'             *              all in (great, right) > pivot2
	'             *
	'             * Pointer k is the first index of ?-part.
	'             
				outer:
				Dim k As Integer = less - 1
'JAVA TO VB CONVERTER TODO TASK: Assignments within expressions are not supported in VB
				Do While k += 1 <= great
					Dim ak As Char = a(k)
					If ak < pivot1 Then ' Move a[k] to left part
						a(k) = a(less)
	'                    
	'                     * Here and below we use "a[i] = b; i++;" instead
	'                     * of "a[i++] = b;" due to performance issue.
	'                     
						a(less) = ak
						less += 1 ' Move a[k] to right part
					ElseIf ak > pivot2 Then
						Do While a(great) > pivot2
							Dim tempVar2 As Boolean = great = k
							great -= 1
							If tempVar2 Then GoTo outer
						Loop
						If a(great) < pivot1 Then ' a[great] <= pivot2
							a(k) = a(less)
							a(less) = a(great)
							less += 1 ' pivot1 <= a[great] <= pivot2
						Else
							a(k) = a(great)
						End If
	'                    
	'                     * Here and below we use "a[i] = b; i--;" instead
	'                     * of "a[i--] = b;" due to performance issue.
	'                     
						a(great) = ak
						great -= 1
					End If
				Loop

				' Swap pivots into their final positions
				a(left) = a(less - 1)
				a(less - 1) = pivot1
				a(right) = a(great + 1)
				a(great + 1) = pivot2

				' Sort left and right parts recursively, excluding known pivots
				sort(a, left, less - 2, leftmost)
				sort(a, great + 2, right, False)

	'            
	'             * If center part is too large (comprises > 4/7 of the array),
	'             * swap internal pivot values to ends.
	'             
				If less < e1 AndAlso e5 < great Then
	'                
	'                 * Skip elements, which are equal to pivot values.
	'                 
					Do While a(less) = pivot1
						less += 1
					Loop

					Do While a(great) = pivot2
						great -= 1
					Loop

	'                
	'                 * Partitioning:
	'                 *
	'                 *   left part         center part                  right part
	'                 * +----------------------------------------------------------+
	'                 * | == pivot1 |  pivot1 < && < pivot2  |    ?    | == pivot2 |
	'                 * +----------------------------------------------------------+
	'                 *              ^                        ^       ^
	'                 *              |                        |       |
	'                 *             less                      k     great
	'                 *
	'                 * Invariants:
	'                 *
	'                 *              all in (*,  less) == pivot1
	'                 *     pivot1 < all in [less,  k)  < pivot2
	'                 *              all in (great, *) == pivot2
	'                 *
	'                 * Pointer k is the first index of ?-part.
	'                 
					outer:
					k = less - 1
'JAVA TO VB CONVERTER TODO TASK: Assignments within expressions are not supported in VB
					Do While k += 1 <= great
						Dim ak As Char = a(k)
						If ak = pivot1 Then ' Move a[k] to left part
							a(k) = a(less)
							a(less) = ak
							less += 1 ' Move a[k] to right part
						ElseIf ak = pivot2 Then
							Do While a(great) = pivot2
								Dim tempVar3 As Boolean = great = k
								great -= 1
								If tempVar3 Then GoTo outer
							Loop
							If a(great) = pivot1 Then ' a[great] < pivot2
								a(k) = a(less)
	'                            
	'                             * Even though a[great] equals to pivot1, the
	'                             * assignment a[less] = pivot1 may be incorrect,
	'                             * if a[great] and pivot1 are floating-point zeros
	'                             * of different signs. Therefore in float and
	'                             * double sorting methods we have to use more
	'                             * accurate assignment a[less] = a[great].
	'                             
								a(less) = pivot1
								less += 1 ' pivot1 < a[great] < pivot2
							Else
								a(k) = a(great)
							End If
							a(great) = ak
							great -= 1
						End If
					Loop
				End If

				' Sort center part recursively
				sort(a, less, great, False)
 ' Partitioning with one pivot
			Else
	'            
	'             * Use the third of the five sorted elements as pivot.
	'             * This value is inexpensive approximation of the median.
	'             
				Dim pivot As Char = a(e3)

	'            
	'             * Partitioning degenerates to the traditional 3-way
	'             * (or "Dutch National Flag") schema:
	'             *
	'             *   left part    center part              right part
	'             * +-------------------------------------------------+
	'             * |  < pivot  |   == pivot   |     ?    |  > pivot  |
	'             * +-------------------------------------------------+
	'             *              ^              ^        ^
	'             *              |              |        |
	'             *             less            k      great
	'             *
	'             * Invariants:
	'             *
	'             *   all in (left, less)   < pivot
	'             *   all in [less, k)     == pivot
	'             *   all in (great, right) > pivot
	'             *
	'             * Pointer k is the first index of ?-part.
	'             
				Dim k As Integer = less
				Do While k <= great
					If a(k) = pivot Then
						k += 1
						Continue Do
					End If
					Dim ak As Char = a(k)
					If ak < pivot Then ' Move a[k] to left part
						a(k) = a(less)
						a(less) = ak
						less += 1 ' a[k] > pivot - Move a[k] to right part
					Else
						Do While a(great) > pivot
							great -= 1
						Loop
						If a(great) < pivot Then ' a[great] <= pivot
							a(k) = a(less)
							a(less) = a(great)
							less += 1 ' a[great] == pivot
						Else
	'                        
	'                         * Even though a[great] equals to pivot, the
	'                         * assignment a[k] = pivot may be incorrect,
	'                         * if a[great] and pivot are floating-point
	'                         * zeros of different signs. Therefore in float
	'                         * and double sorting methods we have to use
	'                         * more accurate assignment a[k] = a[great].
	'                         
							a(k) = pivot
						End If
						a(great) = ak
						great -= 1
					End If
					k += 1
				Loop

	'            
	'             * Sort left and right parts recursively.
	'             * All elements from center part are equal
	'             * and, therefore, already sorted.
	'             
				sort(a, left, less - 1, leftmost)
				sort(a, great + 1, right, False)
			End If
		End Sub

		''' <summary>
		''' The number of distinct byte values. </summary>
		Private Shared ReadOnly NUM_BYTE_VALUES As Integer = 1 << 8

		''' <summary>
		''' Sorts the specified range of the array.
		''' </summary>
		''' <param name="a"> the array to be sorted </param>
		''' <param name="left"> the index of the first element, inclusive, to be sorted </param>
		''' <param name="right"> the index of the last element, inclusive, to be sorted </param>
		Friend Shared Sub sort(ByVal a As SByte(), ByVal left As Integer, ByVal right As Integer)
			' Use counting sort on large arrays
			If right - left > COUNTING_SORT_THRESHOLD_FOR_BYTE Then
				Dim count As Integer() = New Integer(NUM_BYTE_VALUES - 1){}

				Dim i As Integer = left - 1
'JAVA TO VB CONVERTER TODO TASK: Assignments within expressions are not supported in VB
				Do While i += 1 <= right

					count(a(i) - Byte.MinValue) += 1
				Loop
				i = NUM_BYTE_VALUES
				Dim k As Integer = right + 1
				Do While k > left
					i -= 1
					Do While count(i) = 0

						i -= 1
					Loop
					Dim value As SByte = CByte(i + Byte.MinValue)
					Dim s As Integer = count(i)

					Do
						k -= 1
						a(k) = value
						s -= 1
					Loop While s > 0
				Loop ' Use insertion sort on small arrays
			Else
				Dim i As Integer = left
				Dim j As Integer = i
				Do While i < right
					Dim ai As SByte = a(i + 1)
					Do While ai < a(j)
						a(j + 1) = a(j)
						Dim tempVar As Boolean = j = left
						j -= 1
						If tempVar Then Exit Do
					Loop
					a(j + 1) = ai
					i += 1
j = i
				Loop
			End If
		End Sub

		''' <summary>
		''' Sorts the specified range of the array using the given
		''' workspace array slice if possible for merging
		''' </summary>
		''' <param name="a"> the array to be sorted </param>
		''' <param name="left"> the index of the first element, inclusive, to be sorted </param>
		''' <param name="right"> the index of the last element, inclusive, to be sorted </param>
		''' <param name="work"> a workspace array (slice) </param>
		''' <param name="workBase"> origin of usable space in work array </param>
		''' <param name="workLen"> usable size of work array </param>
		Friend Shared Sub sort(ByVal a As Single(), ByVal left As Integer, ByVal right As Integer, ByVal work As Single(), ByVal workBase As Integer, ByVal workLen As Integer)
	'        
	'         * Phase 1: Move NaNs to the end of the array.
	'         
			Do While left <= right AndAlso Float.IsNaN(a(right))
				right -= 1
			Loop
			Dim k As Integer = right
'JAVA TO VB CONVERTER TODO TASK: Assignments within expressions are not supported in VB
			Do While k -= 1 >= left
				Dim ak As Single = a(k)
				If ak <> ak Then ' a[k] is NaN
					a(k) = a(right)
					a(right) = ak
					right -= 1
				End If
			Loop

	'        
	'         * Phase 2: Sort everything except NaNs (which are already in place).
	'         
			doSort(a, left, right, work, workBase, workLen)

	'        
	'         * Phase 3: Place negative zeros before positive zeros.
	'         
			Dim hi As Integer = right

	'        
	'         * Find the first zero, or first positive, or last negative element.
	'         
			Do While left < hi
				Dim middle As Integer = CInt(CUInt((left + hi)) >> 1)
				Dim middleValue As Single = a(middle)

				If middleValue < 0.0f Then
					left = middle + 1
				Else
					hi = middle
				End If
			Loop

	'        
	'         * Skip the last negative value (if any) or all leading negative zeros.
	'         
			Do While left <= right AndAlso Float.floatToRawIntBits(a(left)) < 0
				left += 1
			Loop

	'        
	'         * Move negative zeros to the beginning of the sub-range.
	'         *
	'         * Partitioning:
	'         *
	'         * +----------------------------------------------------+
	'         * |   < 0.0   |   -0.0   |   0.0   |   ?  ( >= 0.0 )   |
	'         * +----------------------------------------------------+
	'         *              ^          ^         ^
	'         *              |          |         |
	'         *             left        p         k
	'         *
	'         * Invariants:
	'         *
	'         *   all in (*,  left)  <  0.0
	'         *   all in [left,  p) == -0.0
	'         *   all in [p,     k) ==  0.0
	'         *   all in [k, right] >=  0.0
	'         *
	'         * Pointer k is the first index of ?-part.
	'         
			k = left
			Dim p As Integer = left - 1
'JAVA TO VB CONVERTER TODO TASK: Assignments within expressions are not supported in VB
			Do While k += 1 <= right
				Dim ak As Single = a(k)
				If ak <> 0.0f Then Exit Do
				If Float.floatToRawIntBits(ak) < 0 Then ' ak is -0.0f
					a(k) = 0.0f
					p += 1
					a(p) = -0.0f
				End If
			Loop
		End Sub

		''' <summary>
		''' Sorts the specified range of the array.
		''' </summary>
		''' <param name="a"> the array to be sorted </param>
		''' <param name="left"> the index of the first element, inclusive, to be sorted </param>
		''' <param name="right"> the index of the last element, inclusive, to be sorted </param>
		''' <param name="work"> a workspace array (slice) </param>
		''' <param name="workBase"> origin of usable space in work array </param>
		''' <param name="workLen"> usable size of work array </param>
		Private Shared Sub doSort(ByVal a As Single(), ByVal left As Integer, ByVal right As Integer, ByVal work As Single(), ByVal workBase As Integer, ByVal workLen As Integer)
			' Use Quicksort on small arrays
			If right - left < QUICKSORT_THRESHOLD Then
				sort(a, left, right, True)
				Return
			End If

	'        
	'         * Index run[i] is the start of i-th run
	'         * (ascending or descending sequence).
	'         
			Dim run As Integer() = New Integer(MAX_RUN_COUNT){}
			Dim count As Integer = 0
			run(0) = left

			' Check if the array is nearly sorted
			Dim k As Integer = left
			Do While k < right
				If a(k) < a(k + 1) Then ' ascending
					k += 1
					Do While k <= right AndAlso a(k - 1) <= a(k)

						k += 1
					Loop ' descending
				ElseIf a(k) > a(k + 1) Then
					k += 1
					Do While k <= right AndAlso a(k - 1) >= a(k)

						k += 1
					Loop
					Dim lo As Integer = run(count) - 1
					Dim hi As Integer = k
'JAVA TO VB CONVERTER TODO TASK: Assignments within expressions are not supported in VB
					Do While lo += 1 < hi -= 1
						Dim t As Single = a(lo)
						a(lo) = a(hi)
						a(hi) = t
					Loop ' equal
				Else
					Dim m As Integer = MAX_RUN_LENGTH
'JAVA TO VB CONVERTER TODO TASK: Assignments within expressions are not supported in VB
					Do While k += 1 <= right AndAlso a(k - 1) = a(k)
						m -= 1
						If m = 0 Then
							sort(a, left, right, True)
							Return
						End If
					Loop
				End If

	'            
	'             * The array is not highly structured,
	'             * use Quicksort instead of merge sort.
	'             
				count += 1
				If count = MAX_RUN_COUNT Then
					sort(a, left, right, True)
					Return
				End If
				run(count) = k
			Loop

			' Check special cases
			' Implementation note: variable "right" is increased by 1.
			Then += 1
			If run(count) = rightThen ' The last run contains one element
				count += 1
				run(count) = right ' The array is already sorted
			ElseIf count = 1 Then
				Return
			End If

			' Determine alternation base for merge
			Dim odd As SByte = 0
			Dim n As Integer = 1
			Do While (n <<= 1) < count

				odd = odd Xor 1
			Loop

			' Use or create temporary array b for merging
			Dim b As Single() ' temp array; alternates with a
			Dim ao, bo As Integer ' array offsets from 'left'
			Dim blen As Integer = right - left ' space needed for b
			If work Is Nothing OrElse workLen < blen OrElse workBase + blen > work.Length Then
				work = New Single(blen - 1){}
				workBase = 0
			End If
			If odd = 0 Then
				Array.Copy(a, left, work, workBase, blen)
				b = a
				bo = 0
				a = work
				ao = workBase - left
			Else
				b = work
				ao = 0
				bo = workBase - left
			End If

			' Merging
			Dim last As Integer
			Do While count > 1
				k = (last = 0) + 2
'JAVA TO VB CONVERTER TODO TASK: Assignments within expressions are not supported in VB
				Do While k <= count
					Dim hi As Integer = run(k), mi As Integer = run(k - 1)
					Dim i As Integer = run(k - 2)
					Dim p As Integer = i
					Dim q As Integer = mi
					Do While i < hi
						If q >= hi OrElse p < mi AndAlso a(p + ao) <= a(q + ao) Then
							b(i + bo) = a(p + ao)
							p += 1
						Else
							b(i + bo) = a(q + ao)
							q += 1
						End If
						i += 1
					Loop
					last += 1
					run(last) = hi
					k += 2
				Loop
				If (count And 1) <> 0 Then
					Dim i As Integer = right
					Dim lo As Integer = run(count - 1)
'JAVA TO VB CONVERTER TODO TASK: Assignments within expressions are not supported in VB
					Do While i -= 1 >= lo

						b(i + bo) = a(i + ao)
					Loop
					last += 1
					run(last) = right
				End If
				Dim t As Single() = a
				a = b
				b = t
				Dim o As Integer = ao
				ao = bo
				bo = o
				count = last
			Loop
		End Sub

		''' <summary>
		''' Sorts the specified range of the array by Dual-Pivot Quicksort.
		''' </summary>
		''' <param name="a"> the array to be sorted </param>
		''' <param name="left"> the index of the first element, inclusive, to be sorted </param>
		''' <param name="right"> the index of the last element, inclusive, to be sorted </param>
		''' <param name="leftmost"> indicates if this part is the leftmost in the range </param>
		Private Shared Sub sort(ByVal a As Single(), ByVal left As Integer, ByVal right As Integer, ByVal leftmost As Boolean)
			Dim length As Integer = right - left + 1

			' Use insertion sort on tiny arrays
			If length < INSERTION_SORT_THRESHOLD Then
				If leftmost Then
	'                
	'                 * Traditional (without sentinel) insertion sort,
	'                 * optimized for server VM, is used in case of
	'                 * the leftmost part.
	'                 
					Dim i As Integer = left
					Dim j As Integer = i
					Do While i < right
						Dim ai As Single = a(i + 1)
						Do While ai < a(j)
							a(j + 1) = a(j)
							Dim tempVar As Boolean = j = left
							j -= 1
							If tempVar Then Exit Do
						Loop
						a(j + 1) = ai
						i += 1
j = i
					Loop
				Else
	'                
	'                 * Skip the longest ascending sequence.
	'                 
					Do
						If left >= right Then Return
						left += 1
					Loop While a(left) >= a(left - 1)

	'                
	'                 * Every element from adjoining part plays the role
	'                 * of sentinel, therefore this allows us to avoid the
	'                 * left range check on each iteration. Moreover, we use
	'                 * the more optimized algorithm, so called pair insertion
	'                 * sort, which is faster (in the context of Quicksort)
	'                 * than traditional implementation of insertion sort.
	'                 
					Dim k As Integer = left
'JAVA TO VB CONVERTER TODO TASK: Assignments within expressions are not supported in VB
					Do While left += 1 <= right
						Dim a1 As Single = a(k), a2 As Single = a(left)

						If a1 < a2 Then
							a2 = a1
							a1 = a(left)
						End If
						k -= 1
						Do While a1 < a(k)
							a(k + 2) = a(k)
							k -= 1
						Loop
						k += 1
						a(k + 1) = a1

						k -= 1
						Do While a2 < a(k)
							a(k + 1) = a(k)
							k -= 1
						Loop
						a(k + 1) = a2
						left += 1
k = left
					Loop
					Dim last As Single = a(right)

					right -= 1
					Do While last < a(right)
						a(right + 1) = a(right)
						right -= 1
					Loop
					a(right + 1) = last
				End If
				Return
			End If

			' Inexpensive approximation of length / 7
			Dim seventh As Integer = (length >> 3) + (length >> 6) + 1

	'        
	'         * Sort five evenly spaced elements around (and including) the
	'         * center element in the range. These elements will be used for
	'         * pivot selection as described below. The choice for spacing
	'         * these elements was empirically determined to work well on
	'         * a wide variety of inputs.
	'         
			Dim e3 As Integer = CInt(CUInt((left + right)) >> 1) ' The midpoint
			Dim e2 As Integer = e3 - seventh
			Dim e1 As Integer = e2 - seventh
			Dim e4 As Integer = e3 + seventh
			Dim e5 As Integer = e4 + seventh

			' Sort these elements using insertion sort
			If a(e2) < a(e1) Then
				Dim t As Single = a(e2)
				a(e2) = a(e1)
				a(e1) = t
			End If

			If a(e3) < a(e2) Then
				Dim t As Single = a(e3)
				a(e3) = a(e2)
				a(e2) = t
				If t < a(e1) Then
					a(e2) = a(e1)
					a(e1) = t
				End If
			End If
			If a(e4) < a(e3) Then
				Dim t As Single = a(e4)
				a(e4) = a(e3)
				a(e3) = t
				If t < a(e2) Then
					a(e3) = a(e2)
					a(e2) = t
					If t < a(e1) Then
						a(e2) = a(e1)
						a(e1) = t
					End If
				End If
			End If
			If a(e5) < a(e4) Then
				Dim t As Single = a(e5)
				a(e5) = a(e4)
				a(e4) = t
				If t < a(e3) Then
					a(e4) = a(e3)
					a(e3) = t
					If t < a(e2) Then
						a(e3) = a(e2)
						a(e2) = t
						If t < a(e1) Then
							a(e2) = a(e1)
							a(e1) = t
						End If
					End If
				End If
			End If

			' Pointers
			Dim less As Integer = left ' The index of the first element of center part
			Dim great As Integer = right ' The index before the first element of right part

			If a(e1) <> a(e2) AndAlso a(e2) <> a(e3) AndAlso a(e3) <> a(e4) AndAlso a(e4) <> a(e5) Then
	'            
	'             * Use the second and fourth of the five sorted elements as pivots.
	'             * These values are inexpensive approximations of the first and
	'             * second terciles of the array. Note that pivot1 <= pivot2.
	'             
				Dim pivot1 As Single = a(e2)
				Dim pivot2 As Single = a(e4)

	'            
	'             * The first and the last elements to be sorted are moved to the
	'             * locations formerly occupied by the pivots. When partitioning
	'             * is complete, the pivots are swapped back into their final
	'             * positions, and excluded from subsequent sorting.
	'             
				a(e2) = a(left)
				a(e4) = a(right)

	'            
	'             * Skip elements, which are less or greater than pivot values.
	'             
				less += 1
				Do While a(less) < pivot1

					less += 1
				Loop
				great -= 1
				Do While a(great) > pivot2

					great -= 1
				Loop

	'            
	'             * Partitioning:
	'             *
	'             *   left part           center part                   right part
	'             * +--------------------------------------------------------------+
	'             * |  < pivot1  |  pivot1 <= && <= pivot2  |    ?    |  > pivot2  |
	'             * +--------------------------------------------------------------+
	'             *               ^                          ^       ^
	'             *               |                          |       |
	'             *              less                        k     great
	'             *
	'             * Invariants:
	'             *
	'             *              all in (left, less)   < pivot1
	'             *    pivot1 <= all in [less, k)     <= pivot2
	'             *              all in (great, right) > pivot2
	'             *
	'             * Pointer k is the first index of ?-part.
	'             
				outer:
				Dim k As Integer = less - 1
'JAVA TO VB CONVERTER TODO TASK: Assignments within expressions are not supported in VB
				Do While k += 1 <= great
					Dim ak As Single = a(k)
					If ak < pivot1 Then ' Move a[k] to left part
						a(k) = a(less)
	'                    
	'                     * Here and below we use "a[i] = b; i++;" instead
	'                     * of "a[i++] = b;" due to performance issue.
	'                     
						a(less) = ak
						less += 1 ' Move a[k] to right part
					ElseIf ak > pivot2 Then
						Do While a(great) > pivot2
							Dim tempVar2 As Boolean = great = k
							great -= 1
							If tempVar2 Then GoTo outer
						Loop
						If a(great) < pivot1 Then ' a[great] <= pivot2
							a(k) = a(less)
							a(less) = a(great)
							less += 1 ' pivot1 <= a[great] <= pivot2
						Else
							a(k) = a(great)
						End If
	'                    
	'                     * Here and below we use "a[i] = b; i--;" instead
	'                     * of "a[i--] = b;" due to performance issue.
	'                     
						a(great) = ak
						great -= 1
					End If
				Loop

				' Swap pivots into their final positions
				a(left) = a(less - 1)
				a(less - 1) = pivot1
				a(right) = a(great + 1)
				a(great + 1) = pivot2

				' Sort left and right parts recursively, excluding known pivots
				sort(a, left, less - 2, leftmost)
				sort(a, great + 2, right, False)

	'            
	'             * If center part is too large (comprises > 4/7 of the array),
	'             * swap internal pivot values to ends.
	'             
				If less < e1 AndAlso e5 < great Then
	'                
	'                 * Skip elements, which are equal to pivot values.
	'                 
					Do While a(less) = pivot1
						less += 1
					Loop

					Do While a(great) = pivot2
						great -= 1
					Loop

	'                
	'                 * Partitioning:
	'                 *
	'                 *   left part         center part                  right part
	'                 * +----------------------------------------------------------+
	'                 * | == pivot1 |  pivot1 < && < pivot2  |    ?    | == pivot2 |
	'                 * +----------------------------------------------------------+
	'                 *              ^                        ^       ^
	'                 *              |                        |       |
	'                 *             less                      k     great
	'                 *
	'                 * Invariants:
	'                 *
	'                 *              all in (*,  less) == pivot1
	'                 *     pivot1 < all in [less,  k)  < pivot2
	'                 *              all in (great, *) == pivot2
	'                 *
	'                 * Pointer k is the first index of ?-part.
	'                 
					outer:
					k = less - 1
'JAVA TO VB CONVERTER TODO TASK: Assignments within expressions are not supported in VB
					Do While k += 1 <= great
						Dim ak As Single = a(k)
						If ak = pivot1 Then ' Move a[k] to left part
							a(k) = a(less)
							a(less) = ak
							less += 1 ' Move a[k] to right part
						ElseIf ak = pivot2 Then
							Do While a(great) = pivot2
								Dim tempVar3 As Boolean = great = k
								great -= 1
								If tempVar3 Then GoTo outer
							Loop
							If a(great) = pivot1 Then ' a[great] < pivot2
								a(k) = a(less)
	'                            
	'                             * Even though a[great] equals to pivot1, the
	'                             * assignment a[less] = pivot1 may be incorrect,
	'                             * if a[great] and pivot1 are floating-point zeros
	'                             * of different signs. Therefore in float and
	'                             * double sorting methods we have to use more
	'                             * accurate assignment a[less] = a[great].
	'                             
								a(less) = a(great)
								less += 1 ' pivot1 < a[great] < pivot2
							Else
								a(k) = a(great)
							End If
							a(great) = ak
							great -= 1
						End If
					Loop
				End If

				' Sort center part recursively
				sort(a, less, great, False)
 ' Partitioning with one pivot
			Else
	'            
	'             * Use the third of the five sorted elements as pivot.
	'             * This value is inexpensive approximation of the median.
	'             
				Dim pivot As Single = a(e3)

	'            
	'             * Partitioning degenerates to the traditional 3-way
	'             * (or "Dutch National Flag") schema:
	'             *
	'             *   left part    center part              right part
	'             * +-------------------------------------------------+
	'             * |  < pivot  |   == pivot   |     ?    |  > pivot  |
	'             * +-------------------------------------------------+
	'             *              ^              ^        ^
	'             *              |              |        |
	'             *             less            k      great
	'             *
	'             * Invariants:
	'             *
	'             *   all in (left, less)   < pivot
	'             *   all in [less, k)     == pivot
	'             *   all in (great, right) > pivot
	'             *
	'             * Pointer k is the first index of ?-part.
	'             
				Dim k As Integer = less
				Do While k <= great
					If a(k) = pivot Then
						k += 1
						Continue Do
					End If
					Dim ak As Single = a(k)
					If ak < pivot Then ' Move a[k] to left part
						a(k) = a(less)
						a(less) = ak
						less += 1 ' a[k] > pivot - Move a[k] to right part
					Else
						Do While a(great) > pivot
							great -= 1
						Loop
						If a(great) < pivot Then ' a[great] <= pivot
							a(k) = a(less)
							a(less) = a(great)
							less += 1 ' a[great] == pivot
						Else
	'                        
	'                         * Even though a[great] equals to pivot, the
	'                         * assignment a[k] = pivot may be incorrect,
	'                         * if a[great] and pivot are floating-point
	'                         * zeros of different signs. Therefore in float
	'                         * and double sorting methods we have to use
	'                         * more accurate assignment a[k] = a[great].
	'                         
							a(k) = a(great)
						End If
						a(great) = ak
						great -= 1
					End If
					k += 1
				Loop

	'            
	'             * Sort left and right parts recursively.
	'             * All elements from center part are equal
	'             * and, therefore, already sorted.
	'             
				sort(a, left, less - 1, leftmost)
				sort(a, great + 1, right, False)
			End If
		End Sub

		''' <summary>
		''' Sorts the specified range of the array using the given
		''' workspace array slice if possible for merging
		''' </summary>
		''' <param name="a"> the array to be sorted </param>
		''' <param name="left"> the index of the first element, inclusive, to be sorted </param>
		''' <param name="right"> the index of the last element, inclusive, to be sorted </param>
		''' <param name="work"> a workspace array (slice) </param>
		''' <param name="workBase"> origin of usable space in work array </param>
		''' <param name="workLen"> usable size of work array </param>
		Friend Shared Sub sort(ByVal a As Double(), ByVal left As Integer, ByVal right As Integer, ByVal work As Double(), ByVal workBase As Integer, ByVal workLen As Integer)
	'        
	'         * Phase 1: Move NaNs to the end of the array.
	'         
			Do While left <= right AndAlso Double.IsNaN(a(right))
				right -= 1
			Loop
			Dim k As Integer = right
'JAVA TO VB CONVERTER TODO TASK: Assignments within expressions are not supported in VB
			Do While k -= 1 >= left
				Dim ak As Double = a(k)
				If ak <> ak Then ' a[k] is NaN
					a(k) = a(right)
					a(right) = ak
					right -= 1
				End If
			Loop

	'        
	'         * Phase 2: Sort everything except NaNs (which are already in place).
	'         
			doSort(a, left, right, work, workBase, workLen)

	'        
	'         * Phase 3: Place negative zeros before positive zeros.
	'         
			Dim hi As Integer = right

	'        
	'         * Find the first zero, or first positive, or last negative element.
	'         
			Do While left < hi
				Dim middle As Integer = CInt(CUInt((left + hi)) >> 1)
				Dim middleValue As Double = a(middle)

				If middleValue < 0.0R Then
					left = middle + 1
				Else
					hi = middle
				End If
			Loop

	'        
	'         * Skip the last negative value (if any) or all leading negative zeros.
	'         
			Do While left <= right AndAlso Double.doubleToRawLongBits(a(left)) < 0
				left += 1
			Loop

	'        
	'         * Move negative zeros to the beginning of the sub-range.
	'         *
	'         * Partitioning:
	'         *
	'         * +----------------------------------------------------+
	'         * |   < 0.0   |   -0.0   |   0.0   |   ?  ( >= 0.0 )   |
	'         * +----------------------------------------------------+
	'         *              ^          ^         ^
	'         *              |          |         |
	'         *             left        p         k
	'         *
	'         * Invariants:
	'         *
	'         *   all in (*,  left)  <  0.0
	'         *   all in [left,  p) == -0.0
	'         *   all in [p,     k) ==  0.0
	'         *   all in [k, right] >=  0.0
	'         *
	'         * Pointer k is the first index of ?-part.
	'         
			k = left
			Dim p As Integer = left - 1
'JAVA TO VB CONVERTER TODO TASK: Assignments within expressions are not supported in VB
			Do While k += 1 <= right
				Dim ak As Double = a(k)
				If ak <> 0.0R Then Exit Do
				If Double.doubleToRawLongBits(ak) < 0 Then ' ak is -0.0d
					a(k) = 0.0R
					p += 1
					a(p) = -0.0R
				End If
			Loop
		End Sub

		''' <summary>
		''' Sorts the specified range of the array.
		''' </summary>
		''' <param name="a"> the array to be sorted </param>
		''' <param name="left"> the index of the first element, inclusive, to be sorted </param>
		''' <param name="right"> the index of the last element, inclusive, to be sorted </param>
		''' <param name="work"> a workspace array (slice) </param>
		''' <param name="workBase"> origin of usable space in work array </param>
		''' <param name="workLen"> usable size of work array </param>
		Private Shared Sub doSort(ByVal a As Double(), ByVal left As Integer, ByVal right As Integer, ByVal work As Double(), ByVal workBase As Integer, ByVal workLen As Integer)
			' Use Quicksort on small arrays
			If right - left < QUICKSORT_THRESHOLD Then
				sort(a, left, right, True)
				Return
			End If

	'        
	'         * Index run[i] is the start of i-th run
	'         * (ascending or descending sequence).
	'         
			Dim run As Integer() = New Integer(MAX_RUN_COUNT){}
			Dim count As Integer = 0
			run(0) = left

			' Check if the array is nearly sorted
			Dim k As Integer = left
			Do While k < right
				If a(k) < a(k + 1) Then ' ascending
					k += 1
					Do While k <= right AndAlso a(k - 1) <= a(k)

						k += 1
					Loop ' descending
				ElseIf a(k) > a(k + 1) Then
					k += 1
					Do While k <= right AndAlso a(k - 1) >= a(k)

						k += 1
					Loop
					Dim lo As Integer = run(count) - 1
					Dim hi As Integer = k
'JAVA TO VB CONVERTER TODO TASK: Assignments within expressions are not supported in VB
					Do While lo += 1 < hi -= 1
						Dim t As Double = a(lo)
						a(lo) = a(hi)
						a(hi) = t
					Loop ' equal
				Else
					Dim m As Integer = MAX_RUN_LENGTH
'JAVA TO VB CONVERTER TODO TASK: Assignments within expressions are not supported in VB
					Do While k += 1 <= right AndAlso a(k - 1) = a(k)
						m -= 1
						If m = 0 Then
							sort(a, left, right, True)
							Return
						End If
					Loop
				End If

	'            
	'             * The array is not highly structured,
	'             * use Quicksort instead of merge sort.
	'             
				count += 1
				If count = MAX_RUN_COUNT Then
					sort(a, left, right, True)
					Return
				End If
				run(count) = k
			Loop

			' Check special cases
			' Implementation note: variable "right" is increased by 1.
			Then += 1
			If run(count) = rightThen ' The last run contains one element
				count += 1
				run(count) = right ' The array is already sorted
			ElseIf count = 1 Then
				Return
			End If

			' Determine alternation base for merge
			Dim odd As SByte = 0
			Dim n As Integer = 1
			Do While (n <<= 1) < count

				odd = odd Xor 1
			Loop

			' Use or create temporary array b for merging
			Dim b As Double() ' temp array; alternates with a
			Dim ao, bo As Integer ' array offsets from 'left'
			Dim blen As Integer = right - left ' space needed for b
			If work Is Nothing OrElse workLen < blen OrElse workBase + blen > work.Length Then
				work = New Double(blen - 1){}
				workBase = 0
			End If
			If odd = 0 Then
				Array.Copy(a, left, work, workBase, blen)
				b = a
				bo = 0
				a = work
				ao = workBase - left
			Else
				b = work
				ao = 0
				bo = workBase - left
			End If

			' Merging
			Dim last As Integer
			Do While count > 1
				k = (last = 0) + 2
'JAVA TO VB CONVERTER TODO TASK: Assignments within expressions are not supported in VB
				Do While k <= count
					Dim hi As Integer = run(k), mi As Integer = run(k - 1)
					Dim i As Integer = run(k - 2)
					Dim p As Integer = i
					Dim q As Integer = mi
					Do While i < hi
						If q >= hi OrElse p < mi AndAlso a(p + ao) <= a(q + ao) Then
							b(i + bo) = a(p + ao)
							p += 1
						Else
							b(i + bo) = a(q + ao)
							q += 1
						End If
						i += 1
					Loop
					last += 1
					run(last) = hi
					k += 2
				Loop
				If (count And 1) <> 0 Then
					Dim i As Integer = right
					Dim lo As Integer = run(count - 1)
'JAVA TO VB CONVERTER TODO TASK: Assignments within expressions are not supported in VB
					Do While i -= 1 >= lo

						b(i + bo) = a(i + ao)
					Loop
					last += 1
					run(last) = right
				End If
				Dim t As Double() = a
				a = b
				b = t
				Dim o As Integer = ao
				ao = bo
				bo = o
				count = last
			Loop
		End Sub

		''' <summary>
		''' Sorts the specified range of the array by Dual-Pivot Quicksort.
		''' </summary>
		''' <param name="a"> the array to be sorted </param>
		''' <param name="left"> the index of the first element, inclusive, to be sorted </param>
		''' <param name="right"> the index of the last element, inclusive, to be sorted </param>
		''' <param name="leftmost"> indicates if this part is the leftmost in the range </param>
		Private Shared Sub sort(ByVal a As Double(), ByVal left As Integer, ByVal right As Integer, ByVal leftmost As Boolean)
			Dim length As Integer = right - left + 1

			' Use insertion sort on tiny arrays
			If length < INSERTION_SORT_THRESHOLD Then
				If leftmost Then
	'                
	'                 * Traditional (without sentinel) insertion sort,
	'                 * optimized for server VM, is used in case of
	'                 * the leftmost part.
	'                 
					Dim i As Integer = left
					Dim j As Integer = i
					Do While i < right
						Dim ai As Double = a(i + 1)
						Do While ai < a(j)
							a(j + 1) = a(j)
							Dim tempVar As Boolean = j = left
							j -= 1
							If tempVar Then Exit Do
						Loop
						a(j + 1) = ai
						i += 1
j = i
					Loop
				Else
	'                
	'                 * Skip the longest ascending sequence.
	'                 
					Do
						If left >= right Then Return
						left += 1
					Loop While a(left) >= a(left - 1)

	'                
	'                 * Every element from adjoining part plays the role
	'                 * of sentinel, therefore this allows us to avoid the
	'                 * left range check on each iteration. Moreover, we use
	'                 * the more optimized algorithm, so called pair insertion
	'                 * sort, which is faster (in the context of Quicksort)
	'                 * than traditional implementation of insertion sort.
	'                 
					Dim k As Integer = left
'JAVA TO VB CONVERTER TODO TASK: Assignments within expressions are not supported in VB
					Do While left += 1 <= right
						Dim a1 As Double = a(k), a2 As Double = a(left)

						If a1 < a2 Then
							a2 = a1
							a1 = a(left)
						End If
						k -= 1
						Do While a1 < a(k)
							a(k + 2) = a(k)
							k -= 1
						Loop
						k += 1
						a(k + 1) = a1

						k -= 1
						Do While a2 < a(k)
							a(k + 1) = a(k)
							k -= 1
						Loop
						a(k + 1) = a2
						left += 1
k = left
					Loop
					Dim last As Double = a(right)

					right -= 1
					Do While last < a(right)
						a(right + 1) = a(right)
						right -= 1
					Loop
					a(right + 1) = last
				End If
				Return
			End If

			' Inexpensive approximation of length / 7
			Dim seventh As Integer = (length >> 3) + (length >> 6) + 1

	'        
	'         * Sort five evenly spaced elements around (and including) the
	'         * center element in the range. These elements will be used for
	'         * pivot selection as described below. The choice for spacing
	'         * these elements was empirically determined to work well on
	'         * a wide variety of inputs.
	'         
			Dim e3 As Integer = CInt(CUInt((left + right)) >> 1) ' The midpoint
			Dim e2 As Integer = e3 - seventh
			Dim e1 As Integer = e2 - seventh
			Dim e4 As Integer = e3 + seventh
			Dim e5 As Integer = e4 + seventh

			' Sort these elements using insertion sort
			If a(e2) < a(e1) Then
				Dim t As Double = a(e2)
				a(e2) = a(e1)
				a(e1) = t
			End If

			If a(e3) < a(e2) Then
				Dim t As Double = a(e3)
				a(e3) = a(e2)
				a(e2) = t
				If t < a(e1) Then
					a(e2) = a(e1)
					a(e1) = t
				End If
			End If
			If a(e4) < a(e3) Then
				Dim t As Double = a(e4)
				a(e4) = a(e3)
				a(e3) = t
				If t < a(e2) Then
					a(e3) = a(e2)
					a(e2) = t
					If t < a(e1) Then
						a(e2) = a(e1)
						a(e1) = t
					End If
				End If
			End If
			If a(e5) < a(e4) Then
				Dim t As Double = a(e5)
				a(e5) = a(e4)
				a(e4) = t
				If t < a(e3) Then
					a(e4) = a(e3)
					a(e3) = t
					If t < a(e2) Then
						a(e3) = a(e2)
						a(e2) = t
						If t < a(e1) Then
							a(e2) = a(e1)
							a(e1) = t
						End If
					End If
				End If
			End If

			' Pointers
			Dim less As Integer = left ' The index of the first element of center part
			Dim great As Integer = right ' The index before the first element of right part

			If a(e1) <> a(e2) AndAlso a(e2) <> a(e3) AndAlso a(e3) <> a(e4) AndAlso a(e4) <> a(e5) Then
	'            
	'             * Use the second and fourth of the five sorted elements as pivots.
	'             * These values are inexpensive approximations of the first and
	'             * second terciles of the array. Note that pivot1 <= pivot2.
	'             
				Dim pivot1 As Double = a(e2)
				Dim pivot2 As Double = a(e4)

	'            
	'             * The first and the last elements to be sorted are moved to the
	'             * locations formerly occupied by the pivots. When partitioning
	'             * is complete, the pivots are swapped back into their final
	'             * positions, and excluded from subsequent sorting.
	'             
				a(e2) = a(left)
				a(e4) = a(right)

	'            
	'             * Skip elements, which are less or greater than pivot values.
	'             
				less += 1
				Do While a(less) < pivot1

					less += 1
				Loop
				great -= 1
				Do While a(great) > pivot2

					great -= 1
				Loop

	'            
	'             * Partitioning:
	'             *
	'             *   left part           center part                   right part
	'             * +--------------------------------------------------------------+
	'             * |  < pivot1  |  pivot1 <= && <= pivot2  |    ?    |  > pivot2  |
	'             * +--------------------------------------------------------------+
	'             *               ^                          ^       ^
	'             *               |                          |       |
	'             *              less                        k     great
	'             *
	'             * Invariants:
	'             *
	'             *              all in (left, less)   < pivot1
	'             *    pivot1 <= all in [less, k)     <= pivot2
	'             *              all in (great, right) > pivot2
	'             *
	'             * Pointer k is the first index of ?-part.
	'             
				outer:
				Dim k As Integer = less - 1
'JAVA TO VB CONVERTER TODO TASK: Assignments within expressions are not supported in VB
				Do While k += 1 <= great
					Dim ak As Double = a(k)
					If ak < pivot1 Then ' Move a[k] to left part
						a(k) = a(less)
	'                    
	'                     * Here and below we use "a[i] = b; i++;" instead
	'                     * of "a[i++] = b;" due to performance issue.
	'                     
						a(less) = ak
						less += 1 ' Move a[k] to right part
					ElseIf ak > pivot2 Then
						Do While a(great) > pivot2
							Dim tempVar2 As Boolean = great = k
							great -= 1
							If tempVar2 Then GoTo outer
						Loop
						If a(great) < pivot1 Then ' a[great] <= pivot2
							a(k) = a(less)
							a(less) = a(great)
							less += 1 ' pivot1 <= a[great] <= pivot2
						Else
							a(k) = a(great)
						End If
	'                    
	'                     * Here and below we use "a[i] = b; i--;" instead
	'                     * of "a[i--] = b;" due to performance issue.
	'                     
						a(great) = ak
						great -= 1
					End If
				Loop

				' Swap pivots into their final positions
				a(left) = a(less - 1)
				a(less - 1) = pivot1
				a(right) = a(great + 1)
				a(great + 1) = pivot2

				' Sort left and right parts recursively, excluding known pivots
				sort(a, left, less - 2, leftmost)
				sort(a, great + 2, right, False)

	'            
	'             * If center part is too large (comprises > 4/7 of the array),
	'             * swap internal pivot values to ends.
	'             
				If less < e1 AndAlso e5 < great Then
	'                
	'                 * Skip elements, which are equal to pivot values.
	'                 
					Do While a(less) = pivot1
						less += 1
					Loop

					Do While a(great) = pivot2
						great -= 1
					Loop

	'                
	'                 * Partitioning:
	'                 *
	'                 *   left part         center part                  right part
	'                 * +----------------------------------------------------------+
	'                 * | == pivot1 |  pivot1 < && < pivot2  |    ?    | == pivot2 |
	'                 * +----------------------------------------------------------+
	'                 *              ^                        ^       ^
	'                 *              |                        |       |
	'                 *             less                      k     great
	'                 *
	'                 * Invariants:
	'                 *
	'                 *              all in (*,  less) == pivot1
	'                 *     pivot1 < all in [less,  k)  < pivot2
	'                 *              all in (great, *) == pivot2
	'                 *
	'                 * Pointer k is the first index of ?-part.
	'                 
					outer:
					k = less - 1
'JAVA TO VB CONVERTER TODO TASK: Assignments within expressions are not supported in VB
					Do While k += 1 <= great
						Dim ak As Double = a(k)
						If ak = pivot1 Then ' Move a[k] to left part
							a(k) = a(less)
							a(less) = ak
							less += 1 ' Move a[k] to right part
						ElseIf ak = pivot2 Then
							Do While a(great) = pivot2
								Dim tempVar3 As Boolean = great = k
								great -= 1
								If tempVar3 Then GoTo outer
							Loop
							If a(great) = pivot1 Then ' a[great] < pivot2
								a(k) = a(less)
	'                            
	'                             * Even though a[great] equals to pivot1, the
	'                             * assignment a[less] = pivot1 may be incorrect,
	'                             * if a[great] and pivot1 are floating-point zeros
	'                             * of different signs. Therefore in float and
	'                             * double sorting methods we have to use more
	'                             * accurate assignment a[less] = a[great].
	'                             
								a(less) = a(great)
								less += 1 ' pivot1 < a[great] < pivot2
							Else
								a(k) = a(great)
							End If
							a(great) = ak
							great -= 1
						End If
					Loop
				End If

				' Sort center part recursively
				sort(a, less, great, False)
 ' Partitioning with one pivot
			Else
	'            
	'             * Use the third of the five sorted elements as pivot.
	'             * This value is inexpensive approximation of the median.
	'             
				Dim pivot As Double = a(e3)

	'            
	'             * Partitioning degenerates to the traditional 3-way
	'             * (or "Dutch National Flag") schema:
	'             *
	'             *   left part    center part              right part
	'             * +-------------------------------------------------+
	'             * |  < pivot  |   == pivot   |     ?    |  > pivot  |
	'             * +-------------------------------------------------+
	'             *              ^              ^        ^
	'             *              |              |        |
	'             *             less            k      great
	'             *
	'             * Invariants:
	'             *
	'             *   all in (left, less)   < pivot
	'             *   all in [less, k)     == pivot
	'             *   all in (great, right) > pivot
	'             *
	'             * Pointer k is the first index of ?-part.
	'             
				Dim k As Integer = less
				Do While k <= great
					If a(k) = pivot Then
						k += 1
						Continue Do
					End If
					Dim ak As Double = a(k)
					If ak < pivot Then ' Move a[k] to left part
						a(k) = a(less)
						a(less) = ak
						less += 1 ' a[k] > pivot - Move a[k] to right part
					Else
						Do While a(great) > pivot
							great -= 1
						Loop
						If a(great) < pivot Then ' a[great] <= pivot
							a(k) = a(less)
							a(less) = a(great)
							less += 1 ' a[great] == pivot
						Else
	'                        
	'                         * Even though a[great] equals to pivot, the
	'                         * assignment a[k] = pivot may be incorrect,
	'                         * if a[great] and pivot are floating-point
	'                         * zeros of different signs. Therefore in float
	'                         * and double sorting methods we have to use
	'                         * more accurate assignment a[k] = a[great].
	'                         
							a(k) = a(great)
						End If
						a(great) = ak
						great -= 1
					End If
					k += 1
				Loop

	'            
	'             * Sort left and right parts recursively.
	'             * All elements from center part are equal
	'             * and, therefore, already sorted.
	'             
				sort(a, left, less - 1, leftmost)
				sort(a, great + 1, right, False)
			End If
		End Sub
	End Class

End Namespace