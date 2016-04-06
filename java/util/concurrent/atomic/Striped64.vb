Imports System
Imports System.Threading

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

Namespace java.util.concurrent.atomic

	''' <summary>
	''' A package-local class holding common representation and mechanics
	''' for classes supporting dynamic striping on 64bit values. The class
	''' extends Number so that concrete subclasses must publicly do so.
	''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
	Friend MustInherit Class Striped64
		Inherits Number
	'    
	'     * This class maintains a lazily-initialized table of atomically
	'     * updated variables, plus an extra "base" field. The table size
	'     * is a power of two. Indexing uses masked per-thread hash codes.
	'     * Nearly all declarations in this class are package-private,
	'     * accessed directly by subclasses.
	'     *
	'     * Table entries are of class Cell; a variant of AtomicLong padded
	'     * (via @sun.misc.Contended) to reduce cache contention. Padding
	'     * is overkill for most Atomics because they are usually
	'     * irregularly scattered in memory and thus don't interfere much
	'     * with each other. But Atomic objects residing in arrays will
	'     * tend to be placed adjacent to each other, and so will most
	'     * often share cache lines (with a huge negative performance
	'     * impact) without this precaution.
	'     *
	'     * In part because Cells are relatively large, we avoid creating
	'     * them until they are needed.  When there is no contention, all
	'     * updates are made to the base field.  Upon first contention (a
	'     * failed CAS on base update), the table is initialized to size 2.
	'     * The table size is doubled upon further contention until
	'     * reaching the nearest power of two greater than or equal to the
	'     * number of CPUS. Table slots remain empty (null) until they are
	'     * needed.
	'     *
	'     * A single spinlock ("cellsBusy") is used for initializing and
	'     * resizing the table, as well as populating slots with new Cells.
	'     * There is no need for a blocking lock; when the lock is not
	'     * available, threads try other slots (or the base).  During these
	'     * retries, there is increased contention and reduced locality,
	'     * which is still better than alternatives.
	'     *
	'     * The Thread probe fields maintained via ThreadLocalRandom serve
	'     * as per-thread hash codes. We let them remain uninitialized as
	'     * zero (if they come in this way) until they contend at slot
	'     * 0. They are then initialized to values that typically do not
	'     * often conflict with others.  Contention and/or table collisions
	'     * are indicated by failed CASes when performing an update
	'     * operation. Upon a collision, if the table size is less than
	'     * the capacity, it is doubled in size unless some other thread
	'     * holds the lock. If a hashed slot is empty, and lock is
	'     * available, a new Cell is created. Otherwise, if the slot
	'     * exists, a CAS is tried.  Retries proceed by "double hashing",
	'     * using a secondary hash (Marsaglia XorShift) to try to find a
	'     * free slot.
	'     *
	'     * The table size is capped because, when there are more threads
	'     * than CPUs, supposing that each thread were bound to a CPU,
	'     * there would exist a perfect hash function mapping threads to
	'     * slots that eliminates collisions. When we reach capacity, we
	'     * search for this mapping by randomly varying the hash codes of
	'     * colliding threads.  Because search is random, and collisions
	'     * only become known via CAS failures, convergence can be slow,
	'     * and because threads are typically not bound to CPUS forever,
	'     * may not occur at all. However, despite these limitations,
	'     * observed contention rates are typically low in these cases.
	'     *
	'     * It is possible for a Cell to become unused when threads that
	'     * once hashed to it terminate, as well as in the case where
	'     * doubling the table causes no thread to hash to it under
	'     * expanded mask.  We do not try to detect or remove such cells,
	'     * under the assumption that for long-running instances, observed
	'     * contention levels will recur, so the cells will eventually be
	'     * needed again; and for short-lived ones, it does not matter.
	'     
		''' <summary>
		''' Padded variant of AtomicLong supporting only raw accesses plus CAS.
		''' 
		''' JVM intrinsics note: It would be possible to use a release-only
		''' form of CAS here, if it were provided.
		''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
		Friend NotInheritable Class Cell
'JAVA TO VB CONVERTER TODO TASK: There is no VB equivalent to 'volatile':
			Friend value As Long
			Friend Sub New(  x As Long)
				value = x
			End Sub
			Friend Function cas(  cmp As Long,   val As Long) As Boolean
				Return UNSAFE.compareAndSwapLong(Me, valueOffset, cmp, val)
			End Function

			' Unsafe mechanics
			Private Shared ReadOnly UNSAFE As sun.misc.Unsafe
			Private Shared ReadOnly valueOffset As Long
			Shared Sub New()
				Try
					UNSAFE = sun.misc.Unsafe.unsafe
					Dim ak As  [Class] = GetType(Cell)
					valueOffset = UNSAFE.objectFieldOffset(ak.getDeclaredField("value"))
				Catch e As Exception
					Throw New [Error](e)
				End Try
			End Sub
		End Class

		''' <summary>
		''' Number of CPUS, to place bound on table size </summary>
		Friend Shared ReadOnly NCPU As Integer = Runtime.runtime.availableProcessors()

		''' <summary>
		''' Table of cells. When non-null, size is a power of 2.
		''' </summary>
'JAVA TO VB CONVERTER TODO TASK: There is no VB equivalent to 'volatile':
		<NonSerialized> _
		Friend cells As Cell()

		''' <summary>
		''' Base value, used mainly when there is no contention, but also as
		''' a fallback during table initialization races. Updated via CAS.
		''' </summary>
'JAVA TO VB CONVERTER TODO TASK: There is no VB equivalent to 'volatile':
		<NonSerialized> _
		Friend base As Long

		''' <summary>
		''' Spinlock (locked via CAS) used when resizing and/or creating Cells.
		''' </summary>
'JAVA TO VB CONVERTER TODO TASK: There is no VB equivalent to 'volatile':
		<NonSerialized> _
		Friend cellsBusy As Integer

		''' <summary>
		''' Package-private default constructor
		''' </summary>
		Friend Sub New()
		End Sub

		''' <summary>
		''' CASes the base field.
		''' </summary>
		Friend Function casBase(  cmp As Long,   val As Long) As Boolean
			Return UNSAFE.compareAndSwapLong(Me, BASE, cmp, val)
		End Function

		''' <summary>
		''' CASes the cellsBusy field from 0 to 1 to acquire lock.
		''' </summary>
		Friend Function casCellsBusy() As Boolean
			Return UNSAFE.compareAndSwapInt(Me, CELLSBUSY, 0, 1)
		End Function

		''' <summary>
		''' Returns the probe value for the current thread.
		''' Duplicated from ThreadLocalRandom because of packaging restrictions.
		''' </summary>
		FriendShared ReadOnly Propertyprobe As Integer
			Get
				Return UNSAFE.getInt(Thread.CurrentThread, PROBE)
			End Get
		End Property

		''' <summary>
		''' Pseudo-randomly advances and records the given probe value for the
		''' given thread.
		''' Duplicated from ThreadLocalRandom because of packaging restrictions.
		''' </summary>
		Friend Shared Function advanceProbe(  probe As Integer) As Integer
			probe = probe Xor probe << 13 ' xorshift
			probe = probe Xor CInt(CUInt(probe) >> 17)
			probe = probe Xor probe << 5
			UNSAFE.putInt(Thread.CurrentThread, Striped64.PROBE, probe)
			Return probe
		End Function

		''' <summary>
		''' Handles cases of updates involving initialization, resizing,
		''' creating new Cells, and/or contention. See above for
		''' explanation. This method suffers the usual non-modularity
		''' problems of optimistic retry code, relying on rechecked sets of
		''' reads.
		''' </summary>
		''' <param name="x"> the value </param>
		''' <param name="fn"> the update function, or null for add (this convention
		''' avoids the need for an extra field or function in LongAdder). </param>
		''' <param name="wasUncontended"> false if CAS failed before call </param>
		Friend Sub longAccumulate(  x As Long,   fn As java.util.function.LongBinaryOperator,   wasUncontended As Boolean)
			Dim h As Integer
			h = probe
			If h = 0 Then
				java.util.concurrent.ThreadLocalRandom.current() ' force initialization
				h = probe
				wasUncontended = True
			End If
			Dim collide As Boolean = False ' True if last slot nonempty
			Do
				Dim [as] As Cell()
				Dim a As Cell
				Dim n As Integer
				Dim v As Long
				[as] = cells
				n = [as].Length
				If [as] IsNot Nothing AndAlso n > 0 Then
					a = [as]((n - 1) And h)
					If a Is Nothing Then
						If cellsBusy = 0 Then ' Try to attach new Cell
							Dim r As New Cell(x) ' Optimistically create
							If cellsBusy = 0 AndAlso casCellsBusy() Then
								Dim created As Boolean = False
								Try ' Recheck under lock
									Dim rs As Cell()
									Dim m, j As Integer
									rs = cells
									m = rs.Length
'JAVA TO VB CONVERTER TODO TASK: Assignments within expressions are not supported in VB
									If rs IsNot Nothing AndAlso m > 0 AndAlso rs(j = (m - 1) And h) Is Nothing Then
										rs(j) = r
										created = True
									End If
								Finally
									cellsBusy = 0
								End Try
								If created Then Exit Do
								Continue Do ' Slot is now non-empty
							End If
						End If
						collide = False
					ElseIf Not wasUncontended Then ' CAS already known to fail
						wasUncontended = True ' Continue after rehash
					Else
						v = a.value, (If(fn Is Nothing, v + x, fn.applyAsLong(v, x)))
						If a.casv Then
							Exit Do
						ElseIf n >= NCPU OrElse cells <> [as] Then
							collide = False ' At max size or stale
						ElseIf Not collide Then
							collide = True
						ElseIf cellsBusy = 0 AndAlso casCellsBusy() Then
							Try
								If cells = [as] Then ' Expand table unless stale
									Dim rs As Cell() = New Cell(n << 1 - 1){}
									For i As Integer = 0 To n - 1
										rs(i) = [as](i)
									Next i
									cells = rs
								End If
							Finally
								cellsBusy = 0
							End Try
							collide = False
							Continue Do ' Retry with expanded table
						End If
						End If
					h = advanceProbe(h)
				ElseIf cellsBusy = 0 AndAlso cells = [as] AndAlso casCellsBusy() Then
					Dim init As Boolean = False
					Try ' Initialize table
						If cells = [as] Then
							Dim rs As Cell() = New Cell(1){}
							rs(h And 1) = New Cell(x)
							cells = rs
							init = True
						End If
					Finally
						cellsBusy = 0
					End Try
					If init Then Exit Do
				Else
					v = base, (If(fn Is Nothing, v + x, fn.applyAsLong(v, x)))
					If casBasev Then Exit Do ' Fall back on using base
					End If
			Loop
		End Sub

		''' <summary>
		''' Same as longAccumulate, but injecting long/double conversions
		''' in too many places to sensibly merge with long version, given
		''' the low-overhead requirements of this class. So must instead be
		''' maintained by copy/paste/adapt.
		''' </summary>
		Friend Sub doubleAccumulate(  x As Double,   fn As java.util.function.DoubleBinaryOperator,   wasUncontended As Boolean)
			Dim h As Integer
			h = probe
			If h = 0 Then
				java.util.concurrent.ThreadLocalRandom.current() ' force initialization
				h = probe
				wasUncontended = True
			End If
			Dim collide As Boolean = False ' True if last slot nonempty
			Do
				Dim [as] As Cell()
				Dim a As Cell
				Dim n As Integer
				Dim v As Long
				[as] = cells
				n = [as].Length
				If [as] IsNot Nothing AndAlso n > 0 Then
					a = [as]((n - 1) And h)
					If a Is Nothing Then
						If cellsBusy = 0 Then ' Try to attach new Cell
							Dim r As New Cell(Double.doubleToRawLongBits(x))
							If cellsBusy = 0 AndAlso casCellsBusy() Then
								Dim created As Boolean = False
								Try ' Recheck under lock
									Dim rs As Cell()
									Dim m, j As Integer
									rs = cells
									m = rs.Length
'JAVA TO VB CONVERTER TODO TASK: Assignments within expressions are not supported in VB
									If rs IsNot Nothing AndAlso m > 0 AndAlso rs(j = (m - 1) And h) Is Nothing Then
										rs(j) = r
										created = True
									End If
								Finally
									cellsBusy = 0
								End Try
								If created Then Exit Do
								Continue Do ' Slot is now non-empty
							End If
						End If
						collide = False
					ElseIf Not wasUncontended Then ' CAS already known to fail
						wasUncontended = True ' Continue after rehash
					Else
						v = a.value, (If(fn Is Nothing, java.lang.[Double].doubleToRawLongBits(Double.longBitsToDouble(v) + x), java.lang.[Double].doubleToRawLongBits(fn.applyAsDouble(Double.longBitsToDouble(v), x))))
						If a.casv Then
							Exit Do
						ElseIf n >= NCPU OrElse cells <> [as] Then
							collide = False ' At max size or stale
						ElseIf Not collide Then
							collide = True
						ElseIf cellsBusy = 0 AndAlso casCellsBusy() Then
							Try
								If cells = [as] Then ' Expand table unless stale
									Dim rs As Cell() = New Cell(n << 1 - 1){}
									For i As Integer = 0 To n - 1
										rs(i) = [as](i)
									Next i
									cells = rs
								End If
							Finally
								cellsBusy = 0
							End Try
							collide = False
							Continue Do ' Retry with expanded table
						End If
						End If
					h = advanceProbe(h)
				ElseIf cellsBusy = 0 AndAlso cells = [as] AndAlso casCellsBusy() Then
					Dim init As Boolean = False
					Try ' Initialize table
						If cells = [as] Then
							Dim rs As Cell() = New Cell(1){}
							rs(h And 1) = New Cell(Double.doubleToRawLongBits(x))
							cells = rs
							init = True
						End If
					Finally
						cellsBusy = 0
					End Try
					If init Then Exit Do
				Else
					v = base, (If(fn Is Nothing, java.lang.[Double].doubleToRawLongBits(Double.longBitsToDouble(v) + x), java.lang.[Double].doubleToRawLongBits(fn.applyAsDouble(Double.longBitsToDouble(v), x))))
					If casBasev Then Exit Do ' Fall back on using base
					End If
			Loop
		End Sub

		' Unsafe mechanics
		Private Shared ReadOnly UNSAFE As sun.misc.Unsafe
		Private Shared ReadOnly BASE As Long
		Private Shared ReadOnly CELLSBUSY As Long
		Private Shared ReadOnly PROBE As Long
		Shared Sub New()
			Try
				UNSAFE = sun.misc.Unsafe.unsafe
				Dim sk As  [Class] = GetType(Striped64)
				BASE = UNSAFE.objectFieldOffset(sk.getDeclaredField("base"))
				CELLSBUSY = UNSAFE.objectFieldOffset(sk.getDeclaredField("cellsBusy"))
				Dim tk As  [Class] = GetType(Thread)
				PROBE = UNSAFE.objectFieldOffset(tk.getDeclaredField("threadLocalRandomProbe"))
			Catch e As Exception
				Throw New [Error](e)
			End Try
		End Sub

	End Class

End Namespace