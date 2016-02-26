Imports System
Imports System.Diagnostics

'
' * Copyright (c) 2009, 2013, Oracle and/or its affiliates. All rights reserved.
' * Copyright 2009 Google Inc.  All Rights Reserved.
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
    ''' A stable, adaptive, iterative mergesort that requires far fewer than
    ''' n lg(n) comparisons when running on partially sorted arrays, while
    ''' offering performance comparable to a traditional mergesort when run
    ''' on random arrays.  Like all proper mergesorts, this sort is stable and
    ''' runs O(n log n) time (worst case).  In the worst case, this sort requires
    ''' temporary storage space for n/2 object references; in the best case,
    ''' it requires only a small constant amount of space.
    ''' 
    ''' This implementation was adapted from Tim Peters's list sort for
    ''' Python, which is described in detail here:
    ''' 
    '''   http://svn.python.org/projects/python/trunk/Objects/listsort.txt
    ''' 
    ''' Tim's C code may be found here:
    ''' 
    '''   http://svn.python.org/projects/python/trunk/Objects/listobject.c
    ''' 
    ''' The underlying techniques are described in this paper (and may have
    ''' even earlier origins):
    ''' 
    '''  "Optimistic Sorting and Information Theoretic Complexity"
    '''  Peter McIlroy
    '''  SODA (Fourth Annual ACM-SIAM Symposium on Discrete Algorithms),
    '''  pp 467-474, Austin, Texas, 25-27 January 1993.
    ''' 
    ''' While the API to this class consists solely of static methods, it is
    ''' (privately) instantiable; a TimSort instance holds the state of an ongoing
    ''' sort, assuming the input array is large enough to warrant the full-blown
    ''' TimSort. Small arrays are sorted in place, using a binary insertion sort.
    ''' 
    ''' @author Josh Bloch
    ''' </summary>
    Friend Class TimSort(Of T, T1)
        ''' <summary>
        ''' This is the minimum sized sequence that will be merged.  Shorter
        ''' sequences will be lengthened by calling binarySort.  If the entire
        ''' array is less than this length, no merges will be performed.
        ''' 
        ''' This constant should be a power of two.  It was 64 in Tim Peter's C
        ''' implementation, but 32 was empirically determined to work better in
        ''' this implementation.  In the unlikely event that you set this constant
        ''' to be a number that's not a power of two, you'll need to change the
        ''' <seealso cref="#minRunLength"/> computation.
        ''' 
        ''' If you decrease this constant, you must change the stackLen
        ''' computation in the TimSort constructor, or you risk an
        ''' ArrayOutOfBounds exception.  See listsort.txt for a discussion
        ''' of the minimum stack length required as a function of the length
        ''' of the array being sorted and the minimum merge sequence length.
        ''' </summary>
        Private Const MIN_MERGE As Integer = 32

        ''' <summary>
        ''' The array being sorted.
        ''' </summary>
        Private ReadOnly a As T()

        ''' <summary>
        ''' The comparator for this sort.
        ''' </summary>
        Private ReadOnly c As Comparator(Of T1)

        ''' <summary>
        ''' When we get into galloping mode, we stay there until both runs win less
        ''' often than MIN_GALLOP consecutive times.
        ''' </summary>
        Private Const MIN_GALLOP As Integer = 7

        ''' <summary>
        ''' This controls when we get *into* galloping mode.  It is initialized
        ''' to MIN_GALLOP.  The mergeLo and mergeHi methods nudge it higher for
        ''' random data, and lower for highly structured data.
        ''' </summary>
        Private minGallop As Integer = MIN_GALLOP

        ''' <summary>
        ''' Maximum initial size of tmp array, which is used for merging.  The array
        ''' can grow to accommodate demand.
        ''' 
        ''' Unlike Tim's original C version, we do not allocate this much storage
        ''' when sorting smaller arrays.  This change was required for performance.
        ''' </summary>
        Private Const INITIAL_TMP_STORAGE_LENGTH As Integer = 256

        ''' <summary>
        ''' Temp storage for merges. A workspace array may optionally be
        ''' provided in constructor, and if so will be used as long as it
        ''' is big enough.
        ''' </summary>
        Private tmp As T()
        Private tmpBase As Integer ' base of tmp array slice
        Private tmpLen As Integer ' length of tmp array slice

        ''' <summary>
        ''' A stack of pending runs yet to be merged.  Run i starts at
        ''' address base[i] and extends for len[i] elements.  It's always
        ''' true (so long as the indices are in bounds) that:
        ''' 
        '''     runBase[i] + runLen[i] == runBase[i + 1]
        ''' 
        ''' so we could cut the storage for this, but it's a minor amount,
        ''' and keeping all the info explicit simplifies the code.
        ''' </summary>
        Private stackSize As Integer = 0 ' Number of pending runs on stack
        Private ReadOnly runBase As Integer()
        Private ReadOnly runLen As Integer()

        ''' <summary>
        ''' Creates a TimSort instance to maintain the state of an ongoing sort.
        ''' </summary>
        ''' <param name="a"> the array to be sorted </param>
        ''' <param name="c"> the comparator to determine the order of the sort </param>
        ''' <param name="work"> a workspace array (slice) </param>
        ''' <param name="workBase"> origin of usable space in work array </param>
        ''' <param name="workLen"> usable size of work array </param>
        'JAVA TO VB CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
        Private Sub New(ByVal a As T(), ByVal c As Comparator(Of T1), ByVal work As T(), ByVal workBase As Integer, ByVal workLen As Integer)
            Me.a = a
            Me.c = c

            ' Allocate temp storage (which may be increased later if necessary)
            Dim len As Integer = a.Length
            Dim tlen As Integer = If(len < 2 * INITIAL_TMP_STORAGE_LENGTH, CInt(CUInt(len) >> 1), INITIAL_TMP_STORAGE_LENGTH)
            If work Is Nothing OrElse workLen < tlen OrElse workBase + tlen > work.Length Then
                'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
                Dim newArray As T() = CType(java.lang.reflect.Array.newInstance(a.GetType().GetElementType(), tlen), T())
                tmp = newArray
                tmpBase = 0
                tmpLen = tlen
            Else
                tmp = work
                tmpBase = workBase
                tmpLen = workLen
            End If

            '        
            '         * Allocate runs-to-be-merged stack (which cannot be expanded).  The
            '         * stack length requirements are described in listsort.txt.  The C
            '         * version always uses the same stack length (85), but this was
            '         * measured to be too expensive when sorting "mid-sized" arrays (e.g.,
            '         * 100 elements) in Java.  Therefore, we use smaller (but sufficiently
            '         * large) stack lengths for smaller arrays.  The "magic numbers" in the
            '         * computation below must be changed if MIN_MERGE is decreased.  See
            '         * the MIN_MERGE declaration above for more information.
            '         * The maximum value of 49 allows for an array up to length
            '         * Integer.MAX_VALUE-4, if array is filled by the worst case stack size
            '         * increasing scenario. More explanations are given in section 4 of:
            '         * http://envisage-project.eu/wp-content/uploads/2015/02/sorting.pdf
            '         
            Dim stackLen As Integer = (If(len < 120, 5, If(len < 1542, 10, If(len < 119151, 24, 49))))
            runBase = New Integer(stackLen - 1) {}
            runLen = New Integer(stackLen - 1) {}
        End Sub

        '    
        '     * The next method (package private and static) constitutes the
        '     * entire API of this class.
        '     

        ''' <summary>
        ''' Sorts the given range, using the given workspace array slice
        ''' for temp storage when possible. This method is designed to be
        ''' invoked from public methods (in class Arrays) after performing
        ''' any necessary array bounds checks and expanding parameters into
        ''' the required forms.
        ''' </summary>
        ''' <param name="a"> the array to be sorted </param>
        ''' <param name="lo"> the index of the first element, inclusive, to be sorted </param>
        ''' <param name="hi"> the index of the last element, exclusive, to be sorted </param>
        ''' <param name="c"> the comparator to use </param>
        ''' <param name="work"> a workspace array (slice) </param>
        ''' <param name="workBase"> origin of usable space in work array </param>
        ''' <param name="workLen"> usable size of work array
        ''' @since 1.8 </param>
        'JAVA TO VB CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
        Friend Shared Sub sort(ByVal a As T(), ByVal lo As Integer, ByVal hi As Integer, ByVal c As Comparator(Of T1), ByVal work As T(), ByVal workBase As Integer, ByVal workLen As Integer)
            Debug.Assert(c IsNot Nothing AndAlso a IsNot Nothing AndAlso lo >= 0 AndAlso lo <= hi AndAlso hi <= a.Length)

            Dim nRemaining As Integer = hi - lo
            If nRemaining < 2 Then Return ' Arrays of size 0 and 1 are always sorted

            ' If array is small, do a "mini-TimSort" with no merges
            If nRemaining < MIN_MERGE Then
                Dim initRunLen As Integer = countRunAndMakeAscending(a, lo, hi, c)
                binarySort(a, lo, hi, lo + initRunLen, c)
                Return
            End If

            ''' <summary>
            ''' March over the array once, left to right, finding natural runs,
            ''' extending short natural runs to minRun elements, and merging runs
            ''' to maintain stack invariant.
            ''' </summary>
            Dim ts As New TimSort(Of T, T1)(a, c, work, workBase, workLen)
            Dim minRun As Integer = minRunLength(nRemaining)
            Do
                ' Identify next run
                Dim runLen As Integer = countRunAndMakeAscending(a, lo, hi, c)

                ' If run is short, extend to min(minRun, nRemaining)
                If runLen < minRun Then
                    Dim force As Integer = If(nRemaining <= minRun, nRemaining, minRun)
                    binarySort(a, lo, lo + force, lo + runLen, c)
                    runLen = force
                End If

                ' Push run onto pending-run stack, and maybe merge
                ts.pushRun(lo, runLen)
                ts.mergeCollapse()

                ' Advance to find next run
                lo += runLen
                nRemaining -= runLen
            Loop While nRemaining <> 0

            ' Merge all remaining runs to complete sort
            Debug.Assert(lo = hi)
            ts.mergeForceCollapse()
            Debug.Assert(ts.stackSize = 1)
        End Sub

        ''' <summary>
        ''' Sorts the specified portion of the specified array using a binary
        ''' insertion sort.  This is the best method for sorting small numbers
        ''' of elements.  It requires O(n log n) compares, but O(n^2) data
        ''' movement (worst case).
        ''' 
        ''' If the initial part of the specified range is already sorted,
        ''' this method can take advantage of it: the method assumes that the
        ''' elements from index {@code lo}, inclusive, to {@code start},
        ''' exclusive are already sorted.
        ''' </summary>
        ''' <param name="a"> the array in which a range is to be sorted </param>
        ''' <param name="lo"> the index of the first element in the range to be sorted </param>
        ''' <param name="hi"> the index after the last element in the range to be sorted </param>
        ''' <param name="start"> the index of the first element in the range that is
        '''        not already known to be sorted ({@code lo <= start <= hi}) </param>
        ''' <param name="c"> comparator to used for the sort </param>
        Private Shared Sub binarySort(ByVal a As T(), ByVal lo As Integer, ByVal hi As Integer, ByVal start As Integer, ByVal c As Comparator(Of T1))
            Debug.Assert(lo <= start AndAlso start <= hi)
            If start = lo Then start += 1
            Do While start < hi
                Dim pivot As T = a(start)

                ' Set left (and right) to the index where a[start] (pivot) belongs
                Dim left As Integer = lo
                Dim right As Integer = start
                Debug.Assert(left <= right)
                '            
                '             * Invariants:
                '             *   pivot >= all in [lo, left).
                '             *   pivot <  all in [right, start).
                '             
                Do While left < right
                    Dim mid As Integer = CInt(CUInt((left + right)) >> 1)
                    If c.compare(pivot, a(mid)) < 0 Then
                        right = mid
                    Else
                        left = mid + 1
                    End If
                Loop
                Debug.Assert(left = right)

                '            
                '             * The invariants still hold: pivot >= all in [lo, left) and
                '             * pivot < all in [left, start), so pivot belongs at left.  Note
                '             * that if there are elements equal to pivot, left points to the
                '             * first slot after them -- that's why this sort is stable.
                '             * Slide elements over to make room for pivot.
                '             
                Dim n As Integer = start - left ' The number of elements to move
                ' Switch is just an optimization for arraycopy in default case
                Select Case n
                    Case 2
                        a(left + 2) = a(left + 1)
'JAVA TO VB CONVERTER TODO TASK: VB does not allow fall-through from a non-empty 'case':
                    Case 1
                        a(left + 1) = a(left)
                    Case Else
                        Array.Copy(a, left, a, left + 1, n)
                End Select
                a(left) = pivot
                start += 1
            Loop
        End Sub

        ''' <summary>
        ''' Returns the length of the run beginning at the specified position in
        ''' the specified array and reverses the run if it is descending (ensuring
        ''' that the run will always be ascending when the method returns).
        ''' 
        ''' A run is the longest ascending sequence with:
        ''' 
        '''    a[lo] <= a[lo + 1] <= a[lo + 2] <= ...
        ''' 
        ''' or the longest descending sequence with:
        ''' 
        '''    a[lo] >  a[lo + 1] >  a[lo + 2] >  ...
        ''' 
        ''' For its intended use in a stable mergesort, the strictness of the
        ''' definition of "descending" is needed so that the call can safely
        ''' reverse a descending sequence without violating stability.
        ''' </summary>
        ''' <param name="a"> the array in which a run is to be counted and possibly reversed </param>
        ''' <param name="lo"> index of the first element in the run </param>
        ''' <param name="hi"> index after the last element that may be contained in the run.
        '''          It is required that {@code lo < hi}. </param>
        ''' <param name="c"> the comparator to used for the sort </param>
        ''' <returns>  the length of the run beginning at the specified position in
        '''          the specified array </returns>
        'JAVA TO VB CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
        Private Shared Function countRunAndMakeAscending(ByVal a As T(), ByVal lo As Integer, ByVal hi As Integer, ByVal c As Comparator(Of T1)) As Integer
            Debug.Assert(lo < hi)
            Dim runHi As Integer = lo + 1
            If runHi = hi Then Return 1

            ' Find end of run, and reverse range if descending
            Dim tempVar As Boolean = c.compare(a(runHi), a(lo)) < 0
            runHi += 1
            If tempVar Then ' Descending
                Do While runHi < hi AndAlso c.compare(a(runHi), a(runHi - 1)) < 0
                    runHi += 1
                Loop
                reverseRange(a, lo, runHi) ' Ascending
            Else
                Do While runHi < hi AndAlso c.compare(a(runHi), a(runHi - 1)) >= 0
                    runHi += 1
                Loop
            End If

            Return runHi - lo
        End Function

        ''' <summary>
        ''' Reverse the specified range of the specified array.
        ''' </summary>
        ''' <param name="a"> the array in which a range is to be reversed </param>
        ''' <param name="lo"> the index of the first element in the range to be reversed </param>
        ''' <param name="hi"> the index after the last element in the range to be reversed </param>
        Private Shared Sub reverseRange(ByVal a As Object(), ByVal lo As Integer, ByVal hi As Integer)
            hi -= 1
            Do While lo < hi
                Dim t As Object = a(lo)
                a(lo) = a(hi)
                lo += 1
                a(hi) = t
                hi -= 1
            Loop
        End Sub

        ''' <summary>
        ''' Returns the minimum acceptable run length for an array of the specified
        ''' length. Natural runs shorter than this will be extended with
        ''' <seealso cref="#binarySort"/>.
        ''' 
        ''' Roughly speaking, the computation is:
        ''' 
        '''  If n < MIN_MERGE, return n (it's too small to bother with fancy stuff).
        '''  Else if n is an exact power of 2, return MIN_MERGE/2.
        '''  Else return an int k, MIN_MERGE/2 <= k <= MIN_MERGE, such that n/k
        '''   is close to, but strictly less than, an exact power of 2.
        ''' 
        ''' For the rationale, see listsort.txt.
        ''' </summary>
        ''' <param name="n"> the length of the array to be sorted </param>
        ''' <returns> the length of the minimum run to be merged </returns>
        Private Shared Function minRunLength(ByVal n As Integer) As Integer
            Debug.Assert(n >= 0)
            Dim r As Integer = 0 ' Becomes 1 if any 1 bits are shifted off
            Do While n >= MIN_MERGE
                r = r Or (n And 1)
                n >>= 1
            Loop
            Return n + r
        End Function

        ''' <summary>
        ''' Pushes the specified run onto the pending-run stack.
        ''' </summary>
        ''' <param name="runBase"> index of the first element in the run </param>
        ''' <param name="runLen">  the number of elements in the run </param>
        Private Sub pushRun(ByVal runBase As Integer, ByVal runLen As Integer)
            Me.runBase(stackSize) = runBase
            Me.runLen(stackSize) = runLen
            stackSize += 1
        End Sub

        ''' <summary>
        ''' Examines the stack of runs waiting to be merged and merges adjacent runs
        ''' until the stack invariants are reestablished:
        ''' 
        '''     1. runLen[i - 3] > runLen[i - 2] + runLen[i - 1]
        '''     2. runLen[i - 2] > runLen[i - 1]
        ''' 
        ''' This method is called each time a new run is pushed onto the stack,
        ''' so the invariants are guaranteed to hold for i < stackSize upon
        ''' entry to the method.
        ''' </summary>
        Private Sub mergeCollapse()
            Do While stackSize > 1
                Dim n As Integer = stackSize - 2
                If n > 0 AndAlso runLen(n - 1) <= runLen(n) + runLen(n + 1) Then
                    If runLen(n - 1) < runLen(n + 1) Then n -= 1
                    mergeAt(n)
                ElseIf runLen(n) <= runLen(n + 1) Then
                    mergeAt(n)
                Else
                    Exit Do ' Invariant is established
                End If
            Loop
        End Sub

        ''' <summary>
        ''' Merges all runs on the stack until only one remains.  This method is
        ''' called once, to complete the sort.
        ''' </summary>
        Private Sub mergeForceCollapse()
            Do While stackSize > 1
                Dim n As Integer = stackSize - 2
                If n > 0 AndAlso runLen(n - 1) < runLen(n + 1) Then n -= 1
                mergeAt(n)
            Loop
        End Sub

        ''' <summary>
        ''' Merges the two runs at stack indices i and i+1.  Run i must be
        ''' the penultimate or antepenultimate run on the stack.  In other words,
        ''' i must be equal to stackSize-2 or stackSize-3.
        ''' </summary>
        ''' <param name="i"> stack index of the first of the two runs to merge </param>
        Private Sub mergeAt(ByVal i As Integer)
            Debug.Assert(stackSize >= 2)
            Debug.Assert(i >= 0)
            Debug.Assert(i = stackSize - 2 OrElse i = stackSize - 3)

            Dim base1 As Integer = runBase(i)
            Dim len1 As Integer = runLen(i)
            Dim base2 As Integer = runBase(i + 1)
            Dim len2 As Integer = runLen(i + 1)
            Debug.Assert(len1 > 0 AndAlso len2 > 0)
            Debug.Assert(base1 + len1 = base2)

            '        
            '         * Record the length of the combined runs; if i is the 3rd-last
            '         * run now, also slide over the last run (which isn't involved
            '         * in this merge).  The current run (i+1) goes away in any case.
            '         
            runLen(i) = len1 + len2
            If i = stackSize - 3 Then
                runBase(i + 1) = runBase(i + 2)
                runLen(i + 1) = runLen(i + 2)
            End If
            stackSize -= 1

            '        
            '         * Find where the first element of run2 goes in run1. Prior elements
            '         * in run1 can be ignored (because they're already in place).
            '         
            Dim k As Integer = gallopRight(a(base2), a, base1, len1, 0, c)
            Debug.Assert(k >= 0)
            base1 += k
            len1 -= k
            If len1 = 0 Then Return

            '        
            '         * Find where the last element of run1 goes in run2. Subsequent elements
            '         * in run2 can be ignored (because they're already in place).
            '         
            len2 = gallopLeft(a(base1 + len1 - 1), a, base2, len2, len2 - 1, c)
            Debug.Assert(len2 >= 0)
            If len2 = 0 Then Return

            ' Merge remaining runs, using tmp array with min(len1, len2) elements
            If len1 <= len2 Then
                mergeLo(base1, len1, base2, len2)
            Else
                mergeHi(base1, len1, base2, len2)
            End If
        End Sub

        ''' <summary>
        ''' Locates the position at which to insert the specified key into the
        ''' specified sorted range; if the range contains an element equal to key,
        ''' returns the index of the leftmost equal element.
        ''' </summary>
        ''' <param name="key"> the key whose insertion point to search for </param>
        ''' <param name="a"> the array in which to search </param>
        ''' <param name="base"> the index of the first element in the range </param>
        ''' <param name="len"> the length of the range; must be > 0 </param>
        ''' <param name="hint"> the index at which to begin the search, 0 <= hint < n.
        '''     The closer hint is to the result, the faster this method will run. </param>
        ''' <param name="c"> the comparator used to order the range, and to search </param>
        ''' <returns> the int k,  0 <= k <= n such that a[b + k - 1] < key <= a[b + k],
        '''    pretending that a[b - 1] is minus infinity and a[b + n] is infinity.
        '''    In other words, key belongs at index b + k; or in other words,
        '''    the first k elements of a should precede key, and the last n - k
        '''    should follow it. </returns>
        'JAVA TO VB CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
        Private Shared Function gallopLeft(Of T, T1)(ByVal key As T, ByVal a As T(), ByVal base As Integer, ByVal len As Integer, ByVal hint As Integer, ByVal c As Comparator(Of T1)) As Integer
            Debug.Assert(len > 0 AndAlso hint >= 0 AndAlso hint < len)
            Dim lastOfs As Integer = 0
            Dim ofs As Integer = 1
            If c.compare(key, a(base + hint)) > 0 Then
                ' Gallop right until a[base+hint+lastOfs] < key <= a[base+hint+ofs]
                Dim maxOfs As Integer = len - hint
                Do While ofs < maxOfs AndAlso c.compare(key, a(base + hint + ofs)) > 0
                    lastOfs = ofs
                    ofs = (ofs << 1) + 1
                    If ofs <= 0 Then ' int overflow ofs = maxOfs
                Loop
                If ofs > maxOfs Then ofs = maxOfs

                ' Make offsets relative to base
                lastOfs += hint
                ofs += hint ' key <= a[base + hint]
            Else
                ' Gallop left until a[base+hint-ofs] < key <= a[base+hint-lastOfs]
                Dim maxOfs As Integer = hint + 1
                Do While ofs < maxOfs AndAlso c.compare(key, a(base + hint - ofs)) <= 0
                    lastOfs = ofs
                    ofs = (ofs << 1) + 1
                    If ofs <= 0 Then ' int overflow ofs = maxOfs
                Loop
                If ofs > maxOfs Then ofs = maxOfs

                ' Make offsets relative to base
                Dim tmp As Integer = lastOfs
                lastOfs = hint - ofs
                ofs = hint - tmp
            End If
            Debug.Assert(-1 <= lastOfs AndAlso lastOfs < ofs AndAlso ofs <= len)

            '        
            '         * Now a[base+lastOfs] < key <= a[base+ofs], so key belongs somewhere
            '         * to the right of lastOfs but no farther right than ofs.  Do a binary
            '         * search, with invariant a[base + lastOfs - 1] < key <= a[base + ofs].
            '         
            lastOfs += 1
            Do While lastOfs < ofs
                Dim m As Integer = lastOfs + (CInt(CUInt((ofs - lastOfs)) >> 1))

                If c.compare(key, a(base + m)) > 0 Then
                    lastOfs = m + 1 ' a[base + m] < key
                Else
                    ofs = m ' key <= a[base + m]
                End If
            Loop
            Debug.Assert(lastOfs = ofs) ' so a[base + ofs - 1] < key <= a[base + ofs]
            Return ofs
        End Function

        ''' <summary>
        ''' Like gallopLeft, except that if the range contains an element equal to
        ''' key, gallopRight returns the index after the rightmost equal element.
        ''' </summary>
        ''' <param name="key"> the key whose insertion point to search for </param>
        ''' <param name="a"> the array in which to search </param>
        ''' <param name="base"> the index of the first element in the range </param>
        ''' <param name="len"> the length of the range; must be > 0 </param>
        ''' <param name="hint"> the index at which to begin the search, 0 <= hint < n.
        '''     The closer hint is to the result, the faster this method will run. </param>
        ''' <param name="c"> the comparator used to order the range, and to search </param>
        ''' <returns> the int k,  0 <= k <= n such that a[b + k - 1] <= key < a[b + k] </returns>
        'JAVA TO VB CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
        Private Shared Function gallopRight(Of T, T1)(ByVal key As T, ByVal a As T(), ByVal base As Integer, ByVal len As Integer, ByVal hint As Integer, ByVal c As Comparator(Of T1)) As Integer
            Debug.Assert(len > 0 AndAlso hint >= 0 AndAlso hint < len)

            Dim ofs As Integer = 1
            Dim lastOfs As Integer = 0
            If c.compare(key, a(base + hint)) < 0 Then
                ' Gallop left until a[b+hint - ofs] <= key < a[b+hint - lastOfs]
                Dim maxOfs As Integer = hint + 1
                Do While ofs < maxOfs AndAlso c.compare(key, a(base + hint - ofs)) < 0
                    lastOfs = ofs
                    ofs = (ofs << 1) + 1
                    If ofs <= 0 Then ' int overflow ofs = maxOfs
                Loop
                If ofs > maxOfs Then ofs = maxOfs

                ' Make offsets relative to b
                Dim tmp As Integer = lastOfs
                lastOfs = hint - ofs
                ofs = hint - tmp ' a[b + hint] <= key
            Else
                ' Gallop right until a[b+hint + lastOfs] <= key < a[b+hint + ofs]
                Dim maxOfs As Integer = len - hint
                Do While ofs < maxOfs AndAlso c.compare(key, a(base + hint + ofs)) >= 0
                    lastOfs = ofs
                    ofs = (ofs << 1) + 1
                    If ofs <= 0 Then ' int overflow ofs = maxOfs
                Loop
                If ofs > maxOfs Then ofs = maxOfs

                ' Make offsets relative to b
                lastOfs += hint
                ofs += hint
            End If
            Debug.Assert(-1 <= lastOfs AndAlso lastOfs < ofs AndAlso ofs <= len)

            '        
            '         * Now a[b + lastOfs] <= key < a[b + ofs], so key belongs somewhere to
            '         * the right of lastOfs but no farther right than ofs.  Do a binary
            '         * search, with invariant a[b + lastOfs - 1] <= key < a[b + ofs].
            '         
            lastOfs += 1
            Do While lastOfs < ofs
                Dim m As Integer = lastOfs + (CInt(CUInt((ofs - lastOfs)) >> 1))

                If c.compare(key, a(base + m)) < 0 Then
                    ofs = m ' key < a[b + m]
                Else
                    lastOfs = m + 1 ' a[b + m] <= key
                End If
            Loop
            Debug.Assert(lastOfs = ofs) ' so a[b + ofs - 1] <= key < a[b + ofs]
            Return ofs
        End Function

        ''' <summary>
        ''' Merges two adjacent runs in place, in a stable fashion.  The first
        ''' element of the first run must be greater than the first element of the
        ''' second run (a[base1] > a[base2]), and the last element of the first run
        ''' (a[base1 + len1-1]) must be greater than all elements of the second run.
        ''' 
        ''' For performance, this method should be called only when len1 <= len2;
        ''' its twin, mergeHi should be called if len1 >= len2.  (Either method
        ''' may be called if len1 == len2.)
        ''' </summary>
        ''' <param name="base1"> index of first element in first run to be merged </param>
        ''' <param name="len1">  length of first run to be merged (must be > 0) </param>
        ''' <param name="base2"> index of first element in second run to be merged
        '''        (must be aBase + aLen) </param>
        ''' <param name="len2">  length of second run to be merged (must be > 0) </param>
        Private Sub mergeLo(ByVal base1 As Integer, ByVal len1 As Integer, ByVal base2 As Integer, ByVal len2 As Integer)
            Debug.Assert(len1 > 0 AndAlso len2 > 0 AndAlso base1 + len1 = base2)

            ' Copy first run into temp array
            Dim a As T() = Me.a ' For performance
            Dim tmp As T() = ensureCapacity(len1)
            Dim cursor1 As Integer = tmpBase ' Indexes into tmp array
            Dim cursor2 As Integer = base2 ' Indexes int a
            Dim dest As Integer = base1 ' Indexes int a
            Array.Copy(a, base1, tmp, cursor1, len1)

            ' Move first element of second run and deal with degenerate cases
            a(dest) = a(cursor2)
            cursor2 += 1
            dest += 1
            len2 -= 1
            If len2 = 0 Then
                Array.Copy(tmp, cursor1, a, dest, len1)
                Return
            End If
            If len1 = 1 Then
                Array.Copy(a, cursor2, a, dest, len2)
                a(dest + len2) = tmp(cursor1) ' Last elt of run 1 to end of merge
                Return
            End If

            'JAVA TO VB CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
            'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
            Dim c As Comparator(Of ?) = Me.c ' Use local variable for performance
            Dim minGallop As Integer = Me.minGallop '  "    "       "     "      "
outer:
            Do
                Dim count1 As Integer = 0 ' Number of times in a row that first run won
                Dim count2 As Integer = 0 ' Number of times in a row that second run won

                '            
                '             * Do the straightforward thing until (if ever) one run starts
                '             * winning consistently.
                '             
                Do
                    Debug.Assert(len1 > 1 AndAlso len2 > 0)
                    If c.compare(a(cursor2), tmp(cursor1)) < 0 Then
                        a(dest) = a(cursor2)
                        cursor2 += 1
                        dest += 1
                        count2 += 1
                        count1 = 0
                        len2 -= 1
                        If len2 = 0 Then GoTo outer
                    Else
                        a(dest) = tmp(cursor1)
                        cursor1 += 1
                        dest += 1
                        count1 += 1
                        count2 = 0
                        len1 -= 1
                        If len1 = 1 Then GoTo outer
                    End If
                Loop While (count1 Or count2) < minGallop

                '            
                '             * One run is winning so consistently that galloping may be a
                '             * huge win. So try that, and continue galloping until (if ever)
                '             * neither run appears to be winning consistently anymore.
                '             
                Do
                    Debug.Assert(len1 > 1 AndAlso len2 > 0)
                    count1 = gallopRight(a(cursor2), tmp, cursor1, len1, 0, c)
                    If count1 <> 0 Then
                        Array.Copy(tmp, cursor1, a, dest, count1)
                        dest += count1
                        cursor1 += count1
                        len1 -= count1
                        If len1 <= 1 Then ' len1 == 1 || len1 == 0 GoTo outer
                        End If
                        a(dest) = a(cursor2)
                        cursor2 += 1
                        dest += 1
                        len2 -= 1
                        If len2 = 0 Then GoTo outer

                        count2 = gallopLeft(tmp(cursor1), a, cursor2, len2, 0, c)
                        If count2 <> 0 Then
                            Array.Copy(a, cursor2, a, dest, count2)
                            dest += count2
                            cursor2 += count2
                            len2 -= count2
                            If len2 = 0 Then GoTo outer
                        End If
                        a(dest) = tmp(cursor1)
                        cursor1 += 1
                        dest += 1
                        len1 -= 1
                        If len1 = 1 Then GoTo outer
                        minGallop -= 1
                Loop While count1 >= MIN_GALLOP Or count2 >= MIN_GALLOP
                If minGallop < 0 Then minGallop = 0
                minGallop += 2 ' Penalize for leaving gallop mode
            Loop ' End of "outer" loop
            Me.minGallop = If(minGallop < 1, 1, minGallop) ' Write back to field

            If len1 = 1 Then
                Debug.Assert(len2 > 0)
                Array.Copy(a, cursor2, a, dest, len2)
                a(dest + len2) = tmp(cursor1) '  Last elt of run 1 to end of merge
            ElseIf len1 = 0 Then
                Throw New IllegalArgumentException("Comparison method violates its general contract!")
            Else
                Debug.Assert(len2 = 0)
                Debug.Assert(len1 > 1)
                Array.Copy(tmp, cursor1, a, dest, len1)
            End If
        End Sub

        ''' <summary>
        ''' Like mergeLo, except that this method should be called only if
        ''' len1 >= len2; mergeLo should be called if len1 <= len2.  (Either method
        ''' may be called if len1 == len2.)
        ''' </summary>
        ''' <param name="base1"> index of first element in first run to be merged </param>
        ''' <param name="len1">  length of first run to be merged (must be > 0) </param>
        ''' <param name="base2"> index of first element in second run to be merged
        '''        (must be aBase + aLen) </param>
        ''' <param name="len2">  length of second run to be merged (must be > 0) </param>
        Private Sub mergeHi(ByVal base1 As Integer, ByVal len1 As Integer, ByVal base2 As Integer, ByVal len2 As Integer)
            Debug.Assert(len1 > 0 AndAlso len2 > 0 AndAlso base1 + len1 = base2)

            ' Copy second run into temp array
            Dim a As T() = Me.a ' For performance
            Dim tmp As T() = ensureCapacity(len2)
            Dim tmpBase As Integer = Me.tmpBase
            Array.Copy(a, base2, tmp, tmpBase, len2)

            Dim cursor1 As Integer = base1 + len1 - 1 ' Indexes into a
            Dim cursor2 As Integer = tmpBase + len2 - 1 ' Indexes into tmp array
            Dim dest As Integer = base2 + len2 - 1 ' Indexes into a

            ' Move last element of first run and deal with degenerate cases
            a(dest) = a(cursor1)
            cursor1 -= 1
            dest -= 1
            len1 -= 1
            If len1 = 0 Then
                Array.Copy(tmp, tmpBase, a, dest - (len2 - 1), len2)
                Return
            End If
            If len2 = 1 Then
                dest -= len1
                cursor1 -= len1
                Array.Copy(a, cursor1 + 1, a, dest + 1, len1)
                a(dest) = tmp(cursor2)
                Return
            End If

            'JAVA TO VB CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
            'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
            Dim c As Comparator(Of ?) = Me.c ' Use local variable for performance
            Dim minGallop As Integer = Me.minGallop '  "    "       "     "      "
outer:
            Do
                Dim count1 As Integer = 0 ' Number of times in a row that first run won
                Dim count2 As Integer = 0 ' Number of times in a row that second run won

                '            
                '             * Do the straightforward thing until (if ever) one run
                '             * appears to win consistently.
                '             
                Do
                    Debug.Assert(len1 > 0 AndAlso len2 > 1)
                    If c.compare(tmp(cursor2), a(cursor1)) < 0 Then
                        a(dest) = a(cursor1)
                        cursor1 -= 1
                        dest -= 1
                        count1 += 1
                        count2 = 0
                        len1 -= 1
                        If len1 = 0 Then GoTo outer
                    Else
                        a(dest) = tmp(cursor2)
                        cursor2 -= 1
                        dest -= 1
                        count2 += 1
                        count1 = 0
                        len2 -= 1
                        If len2 = 1 Then GoTo outer
                    End If
                Loop While (count1 Or count2) < minGallop

                '            
                '             * One run is winning so consistently that galloping may be a
                '             * huge win. So try that, and continue galloping until (if ever)
                '             * neither run appears to be winning consistently anymore.
                '             
                Do
                    Debug.Assert(len1 > 0 AndAlso len2 > 1)
                    count1 = len1 - gallopRight(tmp(cursor2), a, base1, len1, len1 - 1, c)
                    If count1 <> 0 Then
                        dest -= count1
                        cursor1 -= count1
                        len1 -= count1
                        Array.Copy(a, cursor1 + 1, a, dest + 1, count1)
                        If len1 = 0 Then GoTo outer
                    End If
                    a(dest) = tmp(cursor2)
                    cursor2 -= 1
                    dest -= 1
                    len2 -= 1
                    If len2 = 1 Then GoTo outer

                    count2 = len2 - gallopLeft(a(cursor1), tmp, tmpBase, len2, len2 - 1, c)
                    If count2 <> 0 Then
                        dest -= count2
                        cursor2 -= count2
                        len2 -= count2
                        Array.Copy(tmp, cursor2 + 1, a, dest + 1, count2)
                        If len2 <= 1 Then ' len2 == 1 || len2 == 0 GoTo outer
                        End If
                        a(dest) = a(cursor1)
                        cursor1 -= 1
                        dest -= 1
                        len1 -= 1
                        If len1 = 0 Then GoTo outer
                        minGallop -= 1
                Loop While count1 >= MIN_GALLOP Or count2 >= MIN_GALLOP
                If minGallop < 0 Then minGallop = 0
                minGallop += 2 ' Penalize for leaving gallop mode
            Loop ' End of "outer" loop
            Me.minGallop = If(minGallop < 1, 1, minGallop) ' Write back to field

            If len2 = 1 Then
                Debug.Assert(len1 > 0)
                dest -= len1
                cursor1 -= len1
                Array.Copy(a, cursor1 + 1, a, dest + 1, len1)
                a(dest) = tmp(cursor2) ' Move first elt of run2 to front of merge
            ElseIf len2 = 0 Then
                Throw New IllegalArgumentException("Comparison method violates its general contract!")
            Else
                Debug.Assert(len1 = 0)
                Debug.Assert(len2 > 0)
                Array.Copy(tmp, tmpBase, a, dest - (len2 - 1), len2)
            End If
        End Sub

        ''' <summary>
        ''' Ensures that the external array tmp has at least the specified
        ''' number of elements, increasing its size if necessary.  The size
        ''' increases exponentially to ensure amortized linear time complexity.
        ''' </summary>
        ''' <param name="minCapacity"> the minimum required capacity of the tmp array </param>
        ''' <returns> tmp, whether or not it grew </returns>
        Private Function ensureCapacity(ByVal minCapacity As Integer) As T()
            If tmpLen < minCapacity Then
                ' Compute smallest power of 2 > minCapacity
                Dim newSize As Integer = minCapacity
                newSize = newSize Or newSize >> 1
                newSize = newSize Or newSize >> 2
                newSize = newSize Or newSize >> 4
                newSize = newSize Or newSize >> 8
                newSize = newSize Or newSize >> 16
                newSize += 1

                If newSize < 0 Then ' Not bloody likely!
                    newSize = minCapacity
                Else
                    newSize = math.Min(newSize, CInt(CUInt(a.Length) >> 1))
                End If

                'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
                Dim newArray As T() = CType(java.lang.reflect.Array.newInstance(a.GetType().GetElementType(), newSize), T())
                tmp = newArray
                tmpLen = newSize
                tmpBase = 0
            End If
            Return tmp
        End Function
    End Class

End Namespace