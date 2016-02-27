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

Namespace java.util.concurrent.locks


	''' <summary>
	''' A capability-based lock with three modes for controlling read/write
	''' access.  The state of a StampedLock consists of a version and mode.
	''' Lock acquisition methods return a stamp that represents and
	''' controls access with respect to a lock state; "try" versions of
	''' these methods may instead return the special value zero to
	''' represent failure to acquire access. Lock release and conversion
	''' methods require stamps as arguments, and fail if they do not match
	''' the state of the lock. The three modes are:
	''' 
	''' <ul>
	''' 
	'''  <li><b>Writing.</b> Method <seealso cref="#writeLock"/> possibly blocks
	'''   waiting for exclusive access, returning a stamp that can be used
	'''   in method <seealso cref="#unlockWrite"/> to release the lock. Untimed and
	'''   timed versions of {@code tryWriteLock} are also provided. When
	'''   the lock is held in write mode, no read locks may be obtained,
	'''   and all optimistic read validations will fail.  </li>
	''' 
	'''  <li><b>Reading.</b> Method <seealso cref="#readLock"/> possibly blocks
	'''   waiting for non-exclusive access, returning a stamp that can be
	'''   used in method <seealso cref="#unlockRead"/> to release the lock. Untimed
	'''   and timed versions of {@code tryReadLock} are also provided. </li>
	''' 
	'''  <li><b>Optimistic Reading.</b> Method <seealso cref="#tryOptimisticRead"/>
	'''   returns a non-zero stamp only if the lock is not currently held
	'''   in write mode. Method <seealso cref="#validate"/> returns true if the lock
	'''   has not been acquired in write mode since obtaining a given
	'''   stamp.  This mode can be thought of as an extremely weak version
	'''   of a read-lock, that can be broken by a writer at any time.  The
	'''   use of optimistic mode for short read-only code segments often
	'''   reduces contention and improves throughput.  However, its use is
	'''   inherently fragile.  Optimistic read sections should only read
	'''   fields and hold them in local variables for later use after
	'''   validation. Fields read while in optimistic mode may be wildly
	'''   inconsistent, so usage applies only when you are familiar enough
	'''   with data representations to check consistency and/or repeatedly
	'''   invoke method {@code validate()}.  For example, such steps are
	'''   typically required when first reading an object or array
	'''   reference, and then accessing one of its fields, elements or
	'''   methods. </li>
	''' 
	''' </ul>
	''' 
	''' <p>This class also supports methods that conditionally provide
	''' conversions across the three modes. For example, method {@link
	''' #tryConvertToWriteLock} attempts to "upgrade" a mode, returning
	''' a valid write stamp if (1) already in writing mode (2) in reading
	''' mode and there are no other readers or (3) in optimistic mode and
	''' the lock is available. The forms of these methods are designed to
	''' help reduce some of the code bloat that otherwise occurs in
	''' retry-based designs.
	''' 
	''' <p>StampedLocks are designed for use as internal utilities in the
	''' development of thread-safe components. Their use relies on
	''' knowledge of the internal properties of the data, objects, and
	''' methods they are protecting.  They are not reentrant, so locked
	''' bodies should not call other unknown methods that may try to
	''' re-acquire locks (although you may pass a stamp to other methods
	''' that can use or convert it).  The use of read lock modes relies on
	''' the associated code sections being side-effect-free.  Unvalidated
	''' optimistic read sections cannot call methods that are not known to
	''' tolerate potential inconsistencies.  Stamps use finite
	''' representations, and are not cryptographically secure (i.e., a
	''' valid stamp may be guessable). Stamp values may recycle after (no
	''' sooner than) one year of continuous operation. A stamp held without
	''' use or validation for longer than this period may fail to validate
	''' correctly.  StampedLocks are serializable, but always deserialize
	''' into initial unlocked state, so they are not useful for remote
	''' locking.
	''' 
	''' <p>The scheduling policy of StampedLock does not consistently
	''' prefer readers over writers or vice versa.  All "try" methods are
	''' best-effort and do not necessarily conform to any scheduling or
	''' fairness policy. A zero return from any "try" method for acquiring
	''' or converting locks does not carry any information about the state
	''' of the lock; a subsequent invocation may succeed.
	''' 
	''' <p>Because it supports coordinated usage across multiple lock
	''' modes, this class does not directly implement the <seealso cref="Lock"/> or
	''' <seealso cref="ReadWriteLock"/> interfaces. However, a StampedLock may be
	''' viewed <seealso cref="#asReadLock()"/>, <seealso cref="#asWriteLock()"/>, or {@link
	''' #asReadWriteLock()} in applications requiring only the associated
	''' set of functionality.
	''' 
	''' <p><b>Sample Usage.</b> The following illustrates some usage idioms
	''' in a class that maintains simple two-dimensional points. The sample
	''' code illustrates some try/catch conventions even though they are
	''' not strictly needed here because no exceptions can occur in their
	''' bodies.<br>
	''' 
	'''  <pre>{@code
	''' class Point {
	'''   private double x, y;
	'''   private final StampedLock sl = new StampedLock();
	''' 
	'''   void move(double deltaX, double deltaY) { // an exclusively locked method
	'''     long stamp = sl.writeLock();
	'''     try {
	'''       x += deltaX;
	'''       y += deltaY;
	'''     } finally {
	'''       sl.unlockWrite(stamp);
	'''     }
	'''   }
	''' 
	'''   double distanceFromOrigin() { // A read-only method
	'''     long stamp = sl.tryOptimisticRead();
	'''     double currentX = x, currentY = y;
	'''     if (!sl.validate(stamp)) {
	'''        stamp = sl.readLock();
	'''        try {
	'''          currentX = x;
	'''          currentY = y;
	'''        } finally {
	'''           sl.unlockRead(stamp);
	'''        }
	'''     }
	'''     return System.Math.sqrt(currentX * currentX + currentY * currentY);
	'''   }
	''' 
	'''   void moveIfAtOrigin(double newX, double newY) { // upgrade
	'''     // Could instead start with optimistic, not read mode
	'''     long stamp = sl.readLock();
	'''     try {
	'''       while (x == 0.0 && y == 0.0) {
	'''         long ws = sl.tryConvertToWriteLock(stamp);
	'''         if (ws != 0L) {
	'''           stamp = ws;
	'''           x = newX;
	'''           y = newY;
	'''           break;
	'''         }
	'''         else {
	'''           sl.unlockRead(stamp);
	'''           stamp = sl.writeLock();
	'''         }
	'''       }
	'''     } finally {
	'''       sl.unlock(stamp);
	'''     }
	'''   }
	''' }}</pre>
	''' 
	''' @since 1.8
	''' @author Doug Lea
	''' </summary>
	<Serializable> _
	Public Class StampedLock
	'    
	'     * Algorithmic notes:
	'     *
	'     * The design employs elements of Sequence locks
	'     * (as used in linux kernels; see Lameter's
	'     * http://www.lameter.com/gelato2005.pdf
	'     * and elsewhere; see
	'     * Boehm's http://www.hpl.hp.com/techreports/2012/HPL-2012-68.html)
	'     * and Ordered RW locks (see Shirako et al
	'     * http://dl.acm.org/citation.cfm?id=2312015)
	'     *
	'     * Conceptually, the primary state of the lock includes a sequence
	'     * number that is odd when write-locked and even otherwise.
	'     * However, this is offset by a reader count that is non-zero when
	'     * read-locked.  The read count is ignored when validating
	'     * "optimistic" seqlock-reader-style stamps.  Because we must use
	'     * a small finite number of bits (currently 7) for readers, a
	'     * supplementary reader overflow word is used when the number of
	'     * readers exceeds the count field. We do this by treating the max
	'     * reader count value (RBITS) as a spinlock protecting overflow
	'     * updates.
	'     *
	'     * Waiters use a modified form of CLH lock used in
	'     * AbstractQueuedSynchronizer (see its internal documentation for
	'     * a fuller account), where each node is tagged (field mode) as
	'     * either a reader or writer. Sets of waiting readers are grouped
	'     * (linked) under a common node (field cowait) so act as a single
	'     * node with respect to most CLH mechanics.  By virtue of the
	'     * queue structure, wait nodes need not actually carry sequence
	'     * numbers; we know each is greater than its predecessor.  This
	'     * simplifies the scheduling policy to a mainly-FIFO scheme that
	'     * incorporates elements of Phase-Fair locks (see Brandenburg &
	'     * Anderson, especially http://www.cs.unc.edu/~bbb/diss/).  In
	'     * particular, we use the phase-fair anti-barging rule: If an
	'     * incoming reader arrives while read lock is held but there is a
	'     * queued writer, this incoming reader is queued.  (This rule is
	'     * responsible for some of the complexity of method acquireRead,
	'     * but without it, the lock becomes highly unfair.) Method release
	'     * does not (and sometimes cannot) itself wake up cowaiters. This
	'     * is done by the primary thread, but helped by any other threads
	'     * with nothing better to do in methods acquireRead and
	'     * acquireWrite.
	'     *
	'     * These rules apply to threads actually queued. All tryLock forms
	'     * opportunistically try to acquire locks regardless of preference
	'     * rules, and so may "barge" their way in.  Randomized spinning is
	'     * used in the acquire methods to reduce (increasingly expensive)
	'     * context switching while also avoiding sustained memory
	'     * thrashing among many threads.  We limit spins to the head of
	'     * queue. A thread spin-waits up to SPINS times (where each
	'     * iteration decreases spin count with 50% probability) before
	'     * blocking. If, upon wakening it fails to obtain lock, and is
	'     * still (or becomes) the first waiting thread (which indicates
	'     * that some other thread barged and obtained lock), it escalates
	'     * spins (up to MAX_HEAD_SPINS) to reduce the likelihood of
	'     * continually losing to barging threads.
	'     *
	'     * Nearly all of these mechanics are carried out in methods
	'     * acquireWrite and acquireRead, that, as typical of such code,
	'     * sprawl out because actions and retries rely on consistent sets
	'     * of locally cached reads.
	'     *
	'     * As noted in Boehm's paper (above), sequence validation (mainly
	'     * method validate()) requires stricter ordering rules than apply
	'     * to normal volatile reads (of "state").  To force orderings of
	'     * reads before a validation and the validation itself in those
	'     * cases where this is not already forced, we use
	'     * Unsafe.loadFence.
	'     *
	'     * The memory layout keeps lock state and queue pointers together
	'     * (normally on the same cache line). This usually works well for
	'     * read-mostly loads. In most other cases, the natural tendency of
	'     * adaptive-spin CLH locks to reduce memory contention lessens
	'     * motivation to further spread out contended locations, but might
	'     * be subject to future improvements.
	'     

		Private Const serialVersionUID As Long = -6001602636862214147L

		''' <summary>
		''' Number of processors, for spin control </summary>
		Private Shared ReadOnly NCPU As Integer = Runtime.runtime.availableProcessors()

		''' <summary>
		''' Maximum number of retries before enqueuing on acquisition </summary>
		Private Shared ReadOnly SPINS As Integer = If(NCPU > 1, 1 << 6, 0)

		''' <summary>
		''' Maximum number of retries before blocking at head on acquisition </summary>
		Private Shared ReadOnly HEAD_SPINS As Integer = If(NCPU > 1, 1 << 10, 0)

		''' <summary>
		''' Maximum number of retries before re-blocking </summary>
		Private Shared ReadOnly MAX_HEAD_SPINS As Integer = If(NCPU > 1, 1 << 16, 0)

		''' <summary>
		''' The period for yielding when waiting for overflow spinlock </summary>
		Private Const OVERFLOW_YIELD_RATE As Integer = 7 ' must be power 2 - 1

		''' <summary>
		''' The number of bits to use for reader count before overflowing </summary>
		Private Const LG_READERS As Integer = 7

		' Values for lock state and stamp operations
		Private Const RUNIT As Long = 1L
		Private Shared ReadOnly WBIT As Long = 1L << LG_READERS
		Private Shared ReadOnly RBITS As Long = WBIT - 1L
		Private Shared ReadOnly RFULL As Long = RBITS - 1L
		Private Shared ReadOnly ABITS As Long = RBITS Or WBIT
		Private Shared ReadOnly SBITS As Long = Not RBITS ' note overlap with ABITS

		' Initial value for lock state; avoid failure value zero
		Private Shared ReadOnly ORIGIN As Long = WBIT << 1

		' Special value from cancelled acquire methods so caller can throw IE
		Private Const INTERRUPTED As Long = 1L

		' Values for node status; order matters
		Private Const WAITING As Integer = -1
		Private Const CANCELLED As Integer = 1

		' Modes for nodes (int not boolean to allow arithmetic)
		Private Const RMODE As Integer = 0
		Private Const WMODE As Integer = 1

		''' <summary>
		''' Wait nodes </summary>
		Friend NotInheritable Class WNode
'JAVA TO VB CONVERTER TODO TASK: There is no VB equivalent to 'volatile':
			Friend prev As WNode
'JAVA TO VB CONVERTER TODO TASK: There is no VB equivalent to 'volatile':
			Friend [next] As WNode
'JAVA TO VB CONVERTER TODO TASK: There is no VB equivalent to 'volatile':
			Friend cowait As WNode ' list of linked readers
'JAVA TO VB CONVERTER TODO TASK: There is no VB equivalent to 'volatile':
			Friend thread_Renamed As Thread ' non-null while possibly parked
'JAVA TO VB CONVERTER TODO TASK: There is no VB equivalent to 'volatile':
			Friend status As Integer ' 0, WAITING, or CANCELLED
			Friend ReadOnly mode As Integer ' RMODE or WMODE
			Friend Sub New(ByVal m As Integer, ByVal p As WNode)
				mode = m
				prev = p
			End Sub
		End Class

		''' <summary>
		''' Head of CLH queue </summary>
'JAVA TO VB CONVERTER TODO TASK: There is no VB equivalent to 'volatile':
		<NonSerialized> _
		Private whead As WNode
		''' <summary>
		''' Tail (last) of CLH queue </summary>
'JAVA TO VB CONVERTER TODO TASK: There is no VB equivalent to 'volatile':
		<NonSerialized> _
		Private wtail As WNode

		' views
		<NonSerialized> _
		Friend readLockView As ReadLockView
		<NonSerialized> _
		Friend writeLockView As WriteLockView
		<NonSerialized> _
		Friend readWriteLockView As ReadWriteLockView

		''' <summary>
		''' Lock sequence/state </summary>
'JAVA TO VB CONVERTER TODO TASK: There is no VB equivalent to 'volatile':
		<NonSerialized> _
		Private state As Long
		''' <summary>
		''' extra reader count when state read count saturated </summary>
		<NonSerialized> _
		Private readerOverflow As Integer

		''' <summary>
		''' Creates a new lock, initially in unlocked state.
		''' </summary>
		Public Sub New()
			state = ORIGIN
		End Sub

		''' <summary>
		''' Exclusively acquires the lock, blocking if necessary
		''' until available.
		''' </summary>
		''' <returns> a stamp that can be used to unlock or convert mode </returns>
		Public Overridable Function writeLock() As Long
			Dim s, [next] As Long ' bypass acquireWrite in fully unlocked case only
'JAVA TO VB CONVERTER TODO TASK: Assignments within expressions are not supported in VB
			Return (If(((s = state) And ABITS) = 0L AndAlso U.compareAndSwapLong(Me, STATE, s, [next] = s + WBIT), [next], acquireWrite(False, 0L)))
		End Function

		''' <summary>
		''' Exclusively acquires the lock if it is immediately available.
		''' </summary>
		''' <returns> a stamp that can be used to unlock or convert mode,
		''' or zero if the lock is not available </returns>
		Public Overridable Function tryWriteLock() As Long
			Dim s, [next] As Long
'JAVA TO VB CONVERTER TODO TASK: Assignments within expressions are not supported in VB
			Return (If(((s = state) And ABITS) = 0L AndAlso U.compareAndSwapLong(Me, STATE, s, [next] = s + WBIT), [next], 0L))
		End Function

		''' <summary>
		''' Exclusively acquires the lock if it is available within the
		''' given time and the current thread has not been interrupted.
		''' Behavior under timeout and interruption matches that specified
		''' for method <seealso cref="Lock#tryLock(long,TimeUnit)"/>.
		''' </summary>
		''' <param name="time"> the maximum time to wait for the lock </param>
		''' <param name="unit"> the time unit of the {@code time} argument </param>
		''' <returns> a stamp that can be used to unlock or convert mode,
		''' or zero if the lock is not available </returns>
		''' <exception cref="InterruptedException"> if the current thread is interrupted
		''' before acquiring the lock </exception>
		Public Overridable Function tryWriteLock(ByVal time As Long, ByVal unit As java.util.concurrent.TimeUnit) As Long
			Dim nanos As Long = unit.toNanos(time)
			If Not Thread.interrupted() Then
				Dim [next], deadline As Long
				[next] = tryWriteLock()
				If [next] <> 0L Then Return [next]
				If nanos <= 0L Then Return 0L
				deadline = System.nanoTime() + nanos
				If deadline = 0L Then deadline = 1L
				[next] = acquireWrite(True, deadline)
				If [next] <> INTERRUPTED Then Return [next]
			End If
			Throw New InterruptedException
		End Function

		''' <summary>
		''' Exclusively acquires the lock, blocking if necessary
		''' until available or the current thread is interrupted.
		''' Behavior under interruption matches that specified
		''' for method <seealso cref="Lock#lockInterruptibly()"/>.
		''' </summary>
		''' <returns> a stamp that can be used to unlock or convert mode </returns>
		''' <exception cref="InterruptedException"> if the current thread is interrupted
		''' before acquiring the lock </exception>
		Public Overridable Function writeLockInterruptibly() As Long
			Dim [next] As Long
			[next] = acquireWrite(True, 0L)
			If (Not Thread.interrupted()) AndAlso [next] <> INTERRUPTED Then Return [next]
			Throw New InterruptedException
		End Function

		''' <summary>
		''' Non-exclusively acquires the lock, blocking if necessary
		''' until available.
		''' </summary>
		''' <returns> a stamp that can be used to unlock or convert mode </returns>
		Public Overridable Function readLock() As Long
			Dim s As Long = state, [next] As Long ' bypass acquireRead on common uncontended case
'JAVA TO VB CONVERTER TODO TASK: Assignments within expressions are not supported in VB
			Return (If(whead Is wtail AndAlso (s And ABITS) < RFULL AndAlso U.compareAndSwapLong(Me, STATE, s, [next] = s + RUNIT), [next], acquireRead(False, 0L)))
		End Function

		''' <summary>
		''' Non-exclusively acquires the lock if it is immediately available.
		''' </summary>
		''' <returns> a stamp that can be used to unlock or convert mode,
		''' or zero if the lock is not available </returns>
		Public Overridable Function tryReadLock() As Long
			Do
				Dim s, m, [next] As Long
'JAVA TO VB CONVERTER TODO TASK: Assignments within expressions are not supported in VB
				m = (s = state) And ABITS
				If m = WBIT Then
					Return 0L
				ElseIf m < RFULL Then
'JAVA TO VB CONVERTER TODO TASK: Assignments within expressions are not supported in VB
					If U.compareAndSwapLong(Me, STATE, s, [next] = s + RUNIT) Then Return [next]
				Else
					[next] = tryIncReaderOverflow(s)
					If [next] <> 0L Then Return [next]
					End If
			Loop
		End Function

		''' <summary>
		''' Non-exclusively acquires the lock if it is available within the
		''' given time and the current thread has not been interrupted.
		''' Behavior under timeout and interruption matches that specified
		''' for method <seealso cref="Lock#tryLock(long,TimeUnit)"/>.
		''' </summary>
		''' <param name="time"> the maximum time to wait for the lock </param>
		''' <param name="unit"> the time unit of the {@code time} argument </param>
		''' <returns> a stamp that can be used to unlock or convert mode,
		''' or zero if the lock is not available </returns>
		''' <exception cref="InterruptedException"> if the current thread is interrupted
		''' before acquiring the lock </exception>
		Public Overridable Function tryReadLock(ByVal time As Long, ByVal unit As java.util.concurrent.TimeUnit) As Long
			Dim s, m, [next], deadline As Long
			Dim nanos As Long = unit.toNanos(time)
			If Not Thread.interrupted() Then
'JAVA TO VB CONVERTER TODO TASK: Assignments within expressions are not supported in VB
				m = (s = state) And ABITS
				If m <> WBIT Then
					If m < RFULL Then
'JAVA TO VB CONVERTER TODO TASK: Assignments within expressions are not supported in VB
						If U.compareAndSwapLong(Me, STATE, s, [next] = s + RUNIT) Then Return [next]
					Else
						[next] = tryIncReaderOverflow(s)
						If [next] <> 0L Then Return [next]
						End If
				End If
				If nanos <= 0L Then Return 0L
				deadline = System.nanoTime() + nanos
				If deadline = 0L Then deadline = 1L
				[next] = acquireRead(True, deadline)
				If [next] <> INTERRUPTED Then Return [next]
			End If
			Throw New InterruptedException
		End Function

		''' <summary>
		''' Non-exclusively acquires the lock, blocking if necessary
		''' until available or the current thread is interrupted.
		''' Behavior under interruption matches that specified
		''' for method <seealso cref="Lock#lockInterruptibly()"/>.
		''' </summary>
		''' <returns> a stamp that can be used to unlock or convert mode </returns>
		''' <exception cref="InterruptedException"> if the current thread is interrupted
		''' before acquiring the lock </exception>
		Public Overridable Function readLockInterruptibly() As Long
			Dim [next] As Long
			[next] = acquireRead(True, 0L)
			If (Not Thread.interrupted()) AndAlso [next] <> INTERRUPTED Then Return [next]
			Throw New InterruptedException
		End Function

		''' <summary>
		''' Returns a stamp that can later be validated, or zero
		''' if exclusively locked.
		''' </summary>
		''' <returns> a stamp, or zero if exclusively locked </returns>
		Public Overridable Function tryOptimisticRead() As Long
			Dim s As Long
'JAVA TO VB CONVERTER TODO TASK: Assignments within expressions are not supported in VB
			Return If(((s = state) And WBIT) = 0L, (s And SBITS), 0L)
		End Function

		''' <summary>
		''' Returns true if the lock has not been exclusively acquired
		''' since issuance of the given stamp. Always returns false if the
		''' stamp is zero. Always returns true if the stamp represents a
		''' currently held lock. Invoking this method with a value not
		''' obtained from <seealso cref="#tryOptimisticRead"/> or a locking method
		''' for this lock has no defined effect or result.
		''' </summary>
		''' <param name="stamp"> a stamp </param>
		''' <returns> {@code true} if the lock has not been exclusively acquired
		''' since issuance of the given stamp; else false </returns>
		Public Overridable Function validate(ByVal stamp As Long) As Boolean
			U.loadFence()
			Return (stamp And SBITS) = (state And SBITS)
		End Function

		''' <summary>
		''' If the lock state matches the given stamp, releases the
		''' exclusive lock.
		''' </summary>
		''' <param name="stamp"> a stamp returned by a write-lock operation </param>
		''' <exception cref="IllegalMonitorStateException"> if the stamp does
		''' not match the current state of this lock </exception>
		Public Overridable Sub unlockWrite(ByVal stamp As Long)
			Dim h As WNode
			If state <> stamp OrElse (stamp And WBIT) = 0L Then Throw New IllegalMonitorStateException
'JAVA TO VB CONVERTER TODO TASK: Assignments within expressions are not supported in VB
			state = If((stamp += WBIT) = 0L, ORIGIN, stamp)
			h = whead
			If h IsNot Nothing AndAlso h.status <> 0 Then release(h)
		End Sub

		''' <summary>
		''' If the lock state matches the given stamp, releases the
		''' non-exclusive lock.
		''' </summary>
		''' <param name="stamp"> a stamp returned by a read-lock operation </param>
		''' <exception cref="IllegalMonitorStateException"> if the stamp does
		''' not match the current state of this lock </exception>
		Public Overridable Sub unlockRead(ByVal stamp As Long)
			Dim s, m As Long
			Dim h As WNode
			Do
				s = state
				m = s And ABITS
				If (s And SBITS) <> (stamp And SBITS) OrElse (stamp And ABITS) = 0L OrElse m = 0L OrElse m = WBIT Then Throw New IllegalMonitorStateException
				If m < RFULL Then
					If U.compareAndSwapLong(Me, STATE, s, s - RUNIT) Then
						h = whead
						If m = RUNIT AndAlso h IsNot Nothing AndAlso h.status <> 0 Then release(h)
						Exit Do
					End If
				ElseIf tryDecReaderOverflow(s) <> 0L Then
					Exit Do
				End If
			Loop
		End Sub

		''' <summary>
		''' If the lock state matches the given stamp, releases the
		''' corresponding mode of the lock.
		''' </summary>
		''' <param name="stamp"> a stamp returned by a lock operation </param>
		''' <exception cref="IllegalMonitorStateException"> if the stamp does
		''' not match the current state of this lock </exception>
		Public Overridable Sub unlock(ByVal stamp As Long)
			Dim a As Long = stamp And ABITS, m As Long, s As Long
			Dim h As WNode
			s = state
			Do While (s And SBITS) = (stamp And SBITS)
				m = s And ABITS
				If m = 0L Then
					Exit Do
				ElseIf m = WBIT Then
					If a <> m Then Exit Do
'JAVA TO VB CONVERTER TODO TASK: Assignments within expressions are not supported in VB
					state = If((s += WBIT) = 0L, ORIGIN, s)
					h = whead
					If h IsNot Nothing AndAlso h.status <> 0 Then release(h)
					Return
				ElseIf a = 0L OrElse a >= WBIT Then
					Exit Do
				ElseIf m < RFULL Then
					If U.compareAndSwapLong(Me, STATE, s, s - RUNIT) Then
						h = whead
						If m = RUNIT AndAlso h IsNot Nothing AndAlso h.status <> 0 Then release(h)
						Return
					End If
				ElseIf tryDecReaderOverflow(s) <> 0L Then
					Return
				End If
				s = state
			Loop
			Throw New IllegalMonitorStateException
		End Sub

		''' <summary>
		''' If the lock state matches the given stamp, performs one of
		''' the following actions. If the stamp represents holding a write
		''' lock, returns it.  Or, if a read lock, if the write lock is
		''' available, releases the read lock and returns a write stamp.
		''' Or, if an optimistic read, returns a write stamp only if
		''' immediately available. This method returns zero in all other
		''' cases.
		''' </summary>
		''' <param name="stamp"> a stamp </param>
		''' <returns> a valid write stamp, or zero on failure </returns>
		Public Overridable Function tryConvertToWriteLock(ByVal stamp As Long) As Long
			Dim a As Long = stamp And ABITS, m As Long, s As Long, [next] As Long
			s = state
			Do While (s And SBITS) = (stamp And SBITS)
				m = s And ABITS
				If m = 0L Then
					If a <> 0L Then Exit Do
'JAVA TO VB CONVERTER TODO TASK: Assignments within expressions are not supported in VB
					If U.compareAndSwapLong(Me, STATE, s, [next] = s + WBIT) Then Return [next]
				ElseIf m = WBIT Then
					If a <> m Then Exit Do
					Return stamp
				ElseIf m = RUNIT AndAlso a <> 0L Then
'JAVA TO VB CONVERTER TODO TASK: Assignments within expressions are not supported in VB
					If U.compareAndSwapLong(Me, STATE, s, [next] = s - RUNIT + WBIT) Then Return [next]
				Else
					Exit Do
				End If
				s = state
			Loop
			Return 0L
		End Function

		''' <summary>
		''' If the lock state matches the given stamp, performs one of
		''' the following actions. If the stamp represents holding a write
		''' lock, releases it and obtains a read lock.  Or, if a read lock,
		''' returns it. Or, if an optimistic read, acquires a read lock and
		''' returns a read stamp only if immediately available. This method
		''' returns zero in all other cases.
		''' </summary>
		''' <param name="stamp"> a stamp </param>
		''' <returns> a valid read stamp, or zero on failure </returns>
		Public Overridable Function tryConvertToReadLock(ByVal stamp As Long) As Long
			Dim a As Long = stamp And ABITS, m As Long, s As Long, [next] As Long
			Dim h As WNode
			s = state
			Do While (s And SBITS) = (stamp And SBITS)
				m = s And ABITS
				If m = 0L Then
					If a <> 0L Then
						Exit Do
					ElseIf m < RFULL Then
'JAVA TO VB CONVERTER TODO TASK: Assignments within expressions are not supported in VB
						If U.compareAndSwapLong(Me, STATE, s, [next] = s + RUNIT) Then Return [next]
					Else
						[next] = tryIncReaderOverflow(s)
						If [next] <> 0L Then Return [next]
						End If
				ElseIf m = WBIT Then
					If a <> m Then Exit Do
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
						= s + (WBIT + RUNIT)
						state = [next]
					h = whead
					If h IsNot Nothing AndAlso h.status <> 0 Then release(h)
					Return [next]
				ElseIf a <> 0L AndAlso a < WBIT Then
					Return stamp
				Else
					Exit Do
				End If
				s = state
			Loop
			Return 0L
		End Function

		''' <summary>
		''' If the lock state matches the given stamp then, if the stamp
		''' represents holding a lock, releases it and returns an
		''' observation stamp.  Or, if an optimistic read, returns it if
		''' validated. This method returns zero in all other cases, and so
		''' may be useful as a form of "tryUnlock".
		''' </summary>
		''' <param name="stamp"> a stamp </param>
		''' <returns> a valid optimistic read stamp, or zero on failure </returns>
		Public Overridable Function tryConvertToOptimisticRead(ByVal stamp As Long) As Long
			Dim a As Long = stamp And ABITS, m As Long, s As Long, [next] As Long
			Dim h As WNode
			U.loadFence()
			Do
				s = state
				If (s And SBITS) <> (stamp And SBITS) Then Exit Do
				m = s And ABITS
				If m = 0L Then
					If a <> 0L Then Exit Do
					Return s
				ElseIf m = WBIT Then
					If a <> m Then Exit Do
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'JAVA TO VB CONVERTER TODO TASK: Assignments within expressions are not supported in VB
						= If((s += WBIT) = 0L, ORIGIN, s)
						state = [next]
					h = whead
					If h IsNot Nothing AndAlso h.status <> 0 Then release(h)
					Return [next]
				ElseIf a = 0L OrElse a >= WBIT Then
					Exit Do
				ElseIf m < RFULL Then
'JAVA TO VB CONVERTER TODO TASK: Assignments within expressions are not supported in VB
					If U.compareAndSwapLong(Me, STATE, s, [next] = s - RUNIT) Then
						h = whead
						If m = RUNIT AndAlso h IsNot Nothing AndAlso h.status <> 0 Then release(h)
						Return [next] And SBITS
					End If
				Else
					[next] = tryDecReaderOverflow(s)
					If [next] <> 0L Then Return [next] And SBITS
					End If
			Loop
			Return 0L
		End Function

		''' <summary>
		''' Releases the write lock if it is held, without requiring a
		''' stamp value. This method may be useful for recovery after
		''' errors.
		''' </summary>
		''' <returns> {@code true} if the lock was held, else false </returns>
		Public Overridable Function tryUnlockWrite() As Boolean
			Dim s As Long
			Dim h As WNode
			s = state
			If (s And WBIT) <> 0L Then
'JAVA TO VB CONVERTER TODO TASK: Assignments within expressions are not supported in VB
				state = If((s += WBIT) = 0L, ORIGIN, s)
				h = whead
				If h IsNot Nothing AndAlso h.status <> 0 Then release(h)
				Return True
			End If
			Return False
		End Function

		''' <summary>
		''' Releases one hold of the read lock if it is held, without
		''' requiring a stamp value. This method may be useful for recovery
		''' after errors.
		''' </summary>
		''' <returns> {@code true} if the read lock was held, else false </returns>
		Public Overridable Function tryUnlockRead() As Boolean
			Dim s, m As Long
			Dim h As WNode
'JAVA TO VB CONVERTER TODO TASK: Assignments within expressions are not supported in VB
			m = (s = state) And ABITS
			Do While m <> 0L AndAlso m < WBIT
				If m < RFULL Then
					If U.compareAndSwapLong(Me, STATE, s, s - RUNIT) Then
						h = whead
						If m = RUNIT AndAlso h IsNot Nothing AndAlso h.status <> 0 Then release(h)
						Return True
					End If
				ElseIf tryDecReaderOverflow(s) <> 0L Then
					Return True
				End If
				m = (s = state) And ABITS
			Loop
			Return False
		End Function

		' status monitoring methods

		''' <summary>
		''' Returns combined state-held and overflow read count for given
		''' state s.
		''' </summary>
		Private Function getReadLockCount(ByVal s As Long) As Integer
			Dim readers As Long
			readers = s And RBITS
			If readers >= RFULL Then readers = RFULL + readerOverflow
			Return CInt(readers)
		End Function

		''' <summary>
		''' Returns {@code true} if the lock is currently held exclusively.
		''' </summary>
		''' <returns> {@code true} if the lock is currently held exclusively </returns>
		Public Overridable Property writeLocked As Boolean
			Get
				Return (state And WBIT) <> 0L
			End Get
		End Property

		''' <summary>
		''' Returns {@code true} if the lock is currently held non-exclusively.
		''' </summary>
		''' <returns> {@code true} if the lock is currently held non-exclusively </returns>
		Public Overridable Property readLocked As Boolean
			Get
				Return (state And RBITS) <> 0L
			End Get
		End Property

		''' <summary>
		''' Queries the number of read locks held for this lock. This
		''' method is designed for use in monitoring system state, not for
		''' synchronization control. </summary>
		''' <returns> the number of read locks held </returns>
		Public Overridable Property readLockCount As Integer
			Get
				Return getReadLockCount(state)
			End Get
		End Property

		''' <summary>
		''' Returns a string identifying this lock, as well as its lock
		''' state.  The state, in brackets, includes the String {@code
		''' "Unlocked"} or the String {@code "Write-locked"} or the String
		''' {@code "Read-locks:"} followed by the current number of
		''' read-locks held.
		''' </summary>
		''' <returns> a string identifying this lock, as well as its lock state </returns>
		Public Overrides Function ToString() As String
			Dim s As Long = state
			Return MyBase.ToString() & (If((s And ABITS) = 0L, "[Unlocked]", If((s And WBIT) <> 0L, "[Write-locked]", "[Read-locks:" & getReadLockCount(s) & "]")))
		End Function

		' views

		''' <summary>
		''' Returns a plain <seealso cref="Lock"/> view of this StampedLock in which
		''' the <seealso cref="Lock#lock"/> method is mapped to <seealso cref="#readLock"/>,
		''' and similarly for other methods. The returned Lock does not
		''' support a <seealso cref="Condition"/>; method {@link
		''' Lock#newCondition()} throws {@code
		''' UnsupportedOperationException}.
		''' </summary>
		''' <returns> the lock </returns>
		Public Overridable Function asReadLock() As java.util.concurrent.locks.Lock
			Dim v As ReadLockView
				v = readLockView
				If v IsNot Nothing Then
					Return (v)
				Else
'JAVA TO VB CONVERTER TODO TASK: Assignments within expressions are not supported in VB
					Return (readLockView = New ReadLockView(Me))
				End If
		End Function

		''' <summary>
		''' Returns a plain <seealso cref="Lock"/> view of this StampedLock in which
		''' the <seealso cref="Lock#lock"/> method is mapped to <seealso cref="#writeLock"/>,
		''' and similarly for other methods. The returned Lock does not
		''' support a <seealso cref="Condition"/>; method {@link
		''' Lock#newCondition()} throws {@code
		''' UnsupportedOperationException}.
		''' </summary>
		''' <returns> the lock </returns>
		Public Overridable Function asWriteLock() As java.util.concurrent.locks.Lock
			Dim v As WriteLockView
				v = writeLockView
				If v IsNot Nothing Then
					Return (v)
				Else
'JAVA TO VB CONVERTER TODO TASK: Assignments within expressions are not supported in VB
					Return (writeLockView = New WriteLockView(Me))
				End If
		End Function

		''' <summary>
		''' Returns a <seealso cref="ReadWriteLock"/> view of this StampedLock in
		''' which the <seealso cref="ReadWriteLock#readLock()"/> method is mapped to
		''' <seealso cref="#asReadLock()"/>, and <seealso cref="ReadWriteLock#writeLock()"/> to
		''' <seealso cref="#asWriteLock()"/>.
		''' </summary>
		''' <returns> the lock </returns>
		Public Overridable Function asReadWriteLock() As java.util.concurrent.locks.ReadWriteLock
			Dim v As ReadWriteLockView
				v = readWriteLockView
				If v IsNot Nothing Then
					Return (v)
				Else
'JAVA TO VB CONVERTER TODO TASK: Assignments within expressions are not supported in VB
					Return (readWriteLockView = New ReadWriteLockView(Me))
				End If
		End Function

		' view classes

		Friend NotInheritable Class ReadLockView
			Implements java.util.concurrent.locks.Lock

			Private ReadOnly outerInstance As StampedLock

			Public Sub New(ByVal outerInstance As StampedLock)
				Me.outerInstance = outerInstance
			End Sub

			Public Sub lock()
				outerInstance.readLock()
			End Sub
			Public Sub lockInterruptibly()
				outerInstance.readLockInterruptibly()
			End Sub
			Public Function tryLock() As Boolean
				Return outerInstance.tryReadLock() <> 0L
			End Function
			Public Function tryLock(ByVal time As Long, ByVal unit As java.util.concurrent.TimeUnit) As Boolean
				Return outerInstance.tryReadLock(time, unit) <> 0L
			End Function
			Public Sub unlock()
				outerInstance.unstampedUnlockRead()
			End Sub
			Public Function newCondition() As java.util.concurrent.locks.Condition
				Throw New UnsupportedOperationException
			End Function
		End Class

		Friend NotInheritable Class WriteLockView
			Implements java.util.concurrent.locks.Lock

			Private ReadOnly outerInstance As StampedLock

			Public Sub New(ByVal outerInstance As StampedLock)
				Me.outerInstance = outerInstance
			End Sub

			Public Sub lock()
				outerInstance.writeLock()
			End Sub
			Public Sub lockInterruptibly()
				outerInstance.writeLockInterruptibly()
			End Sub
			Public Function tryLock() As Boolean
				Return outerInstance.tryWriteLock() <> 0L
			End Function
			Public Function tryLock(ByVal time As Long, ByVal unit As java.util.concurrent.TimeUnit) As Boolean
				Return outerInstance.tryWriteLock(time, unit) <> 0L
			End Function
			Public Sub unlock()
				outerInstance.unstampedUnlockWrite()
			End Sub
			Public Function newCondition() As java.util.concurrent.locks.Condition
				Throw New UnsupportedOperationException
			End Function
		End Class

		Friend NotInheritable Class ReadWriteLockView
			Implements java.util.concurrent.locks.ReadWriteLock

			Private ReadOnly outerInstance As StampedLock

			Public Sub New(ByVal outerInstance As StampedLock)
				Me.outerInstance = outerInstance
			End Sub

			Public Function readLock() As java.util.concurrent.locks.Lock
				Return outerInstance.asReadLock()
			End Function
			Public Function writeLock() As java.util.concurrent.locks.Lock
				Return outerInstance.asWriteLock()
			End Function
		End Class

		' Unlock methods without stamp argument checks for view classes.
		' Needed because view-class lock methods throw away stamps.

		Friend Sub unstampedUnlockWrite()
			Dim h As WNode
			Dim s As Long
			s = state
			If (s And WBIT) = 0L Then Throw New IllegalMonitorStateException
'JAVA TO VB CONVERTER TODO TASK: Assignments within expressions are not supported in VB
			state = If((s += WBIT) = 0L, ORIGIN, s)
			h = whead
			If h IsNot Nothing AndAlso h.status <> 0 Then release(h)
		End Sub

		Friend Sub unstampedUnlockRead()
			Do
				Dim s, m As Long
				Dim h As WNode
'JAVA TO VB CONVERTER TODO TASK: Assignments within expressions are not supported in VB
				m = (s = state) And ABITS
				If m = 0L OrElse m >= WBIT Then
					Throw New IllegalMonitorStateException
				ElseIf m < RFULL Then
					If U.compareAndSwapLong(Me, STATE, s, s - RUNIT) Then
						h = whead
						If m = RUNIT AndAlso h IsNot Nothing AndAlso h.status <> 0 Then release(h)
						Exit Do
					End If
				ElseIf tryDecReaderOverflow(s) <> 0L Then
					Exit Do
				End If
			Loop
		End Sub

		Private Sub readObject(ByVal s As java.io.ObjectInputStream)
			s.defaultReadObject()
			state = ORIGIN ' reset to unlocked state
		End Sub

		' internals

		''' <summary>
		''' Tries to increment readerOverflow by first setting state
		''' access bits value to RBITS, indicating hold of spinlock,
		''' then updating, then releasing.
		''' </summary>
		''' <param name="s"> a reader overflow stamp: (s & ABITS) >= RFULL </param>
		''' <returns> new stamp on success, else zero </returns>
		Private Function tryIncReaderOverflow(ByVal s As Long) As Long
			' assert (s & ABITS) >= RFULL;
			If (s And ABITS) = RFULL Then
				If U.compareAndSwapLong(Me, STATE, s, s Or RBITS) Then
					readerOverflow += 1
					state = s
					Return s
				End If
			ElseIf (java.util.concurrent.locks.LockSupport.nextSecondarySeed() And OVERFLOW_YIELD_RATE) = 0 Then
				Thread.yield()
			End If
			Return 0L
		End Function

		''' <summary>
		''' Tries to decrement readerOverflow.
		''' </summary>
		''' <param name="s"> a reader overflow stamp: (s & ABITS) >= RFULL </param>
		''' <returns> new stamp on success, else zero </returns>
		Private Function tryDecReaderOverflow(ByVal s As Long) As Long
			' assert (s & ABITS) >= RFULL;
			If (s And ABITS) = RFULL Then
				If U.compareAndSwapLong(Me, STATE, s, s Or RBITS) Then
					Dim r As Integer
					Dim [next] As Long
					r = readerOverflow
					If r > 0 Then
						readerOverflow = r - 1
						[next] = s
					Else
						[next] = s - RUNIT
					End If
					 state = [next]
					 Return [next]
				End If
			ElseIf (java.util.concurrent.locks.LockSupport.nextSecondarySeed() And OVERFLOW_YIELD_RATE) = 0 Then
				Thread.yield()
			End If
			Return 0L
		End Function

		''' <summary>
		''' Wakes up the successor of h (normally whead). This is normally
		''' just h.next, but may require traversal from wtail if next
		''' pointers are lagging. This may fail to wake up an acquiring
		''' thread when one or more have been cancelled, but the cancel
		''' methods themselves provide extra safeguards to ensure liveness.
		''' </summary>
		Private Sub release(ByVal h As WNode)
			If h IsNot Nothing Then
				Dim q As WNode
				Dim w As Thread
				U.compareAndSwapInt(h, WSTATUS, WAITING, 0)
				q = h.next
				If q Is Nothing OrElse q.status = CANCELLED Then
					Dim t As WNode = wtail
					Do While t IsNot Nothing AndAlso t IsNot h
						If t.status <= 0 Then q = t
						t = t.prev
					Loop
				End If
				w = q.thread_Renamed
				If q IsNot Nothing AndAlso w IsNot Nothing Then U.unpark(w)
			End If
		End Sub

		''' <summary>
		''' See above for explanation.
		''' </summary>
		''' <param name="interruptible"> true if should check interrupts and if so
		''' return INTERRUPTED </param>
		''' <param name="deadline"> if nonzero, the System.nanoTime value to timeout
		''' at (and return zero) </param>
		''' <returns> next state, or INTERRUPTED </returns>
		Private Function acquireWrite(ByVal interruptible As Boolean, ByVal deadline As Long) As Long
			Dim node As WNode = Nothing, p As WNode
			Dim spins_Renamed As Integer = -1
			Do ' spin while enqueuing
				Dim m, s, ns As Long
'JAVA TO VB CONVERTER TODO TASK: Assignments within expressions are not supported in VB
				m = (s = state) And ABITS
				If m = 0L Then
'JAVA TO VB CONVERTER TODO TASK: Assignments within expressions are not supported in VB
					If U.compareAndSwapLong(Me, STATE, s, ns = s + WBIT) Then Return ns
				ElseIf spins_Renamed < 0 Then
					spins_Renamed = If(m = WBIT AndAlso wtail Is whead, SPINS, 0)
				ElseIf spins_Renamed > 0 Then
					If java.util.concurrent.locks.LockSupport.nextSecondarySeed() >= 0 Then spins_Renamed -= 1
				Else
					p = wtail
					If p Is Nothing Then ' initialize queue
						Dim hd As New WNode(WMODE, Nothing)
						If U.compareAndSwapObject(Me, WHEAD, Nothing, hd) Then wtail = hd
					ElseIf node Is Nothing Then
						node = New WNode(WMODE, p)
					ElseIf node.prev IsNot p Then
						node.prev = p
					ElseIf U.compareAndSwapObject(Me, WTAIL, p, node) Then
						p.next = node
						Exit Do
					End If
					End If
			Loop

			spins_Renamed = -1
			Do
				Dim h, np, pp As WNode
				Dim ps As Integer
				h = whead
				If h Is p Then
					If spins_Renamed < 0 Then
						spins_Renamed = HEAD_SPINS
					ElseIf spins_Renamed < MAX_HEAD_SPINS Then
						spins_Renamed <<= 1
					End If
					Dim k As Integer = spins_Renamed
					Do ' spin at head
						Dim s, ns As Long
						s = state
						If (s And ABITS) = 0L Then
'JAVA TO VB CONVERTER TODO TASK: Assignments within expressions are not supported in VB
							If U.compareAndSwapLong(Me, STATE, s, ns = s + WBIT) Then
								whead = node
								node.prev = Nothing
								Return ns
							End If
						Else
							k -= 1
							If java.util.concurrent.locks.LockSupport.nextSecondarySeed() >= 0 AndAlso k <= 0 Then Exit Do
							End If
					Loop
				ElseIf h IsNot Nothing Then ' help release stale waiters
					Dim c As WNode
					Dim w As Thread
					c = h.cowait
					Do While c IsNot Nothing
						w = c.thread_Renamed
						If U.compareAndSwapObject(h, WCOWAIT, c, c.cowait) AndAlso w IsNot Nothing Then U.unpark(w)
						c = h.cowait
					Loop
				End If
				If whead Is h Then
					np = node.prev
					If np IsNot p Then
						If np IsNot Nothing Then
								p = np
								np.next = node
						End If
					Else
						ps = p.status
						If ps = 0 Then
							U.compareAndSwapInt(p, WSTATUS, 0, WAITING)
						ElseIf ps = CANCELLED Then
							pp = p.prev
							If pp IsNot Nothing Then
								node.prev = pp
								pp.next = node
							End If
						Else
							Dim time As Long ' 0 argument to park means no timeout
						End If
						If deadline = 0L Then
							time = 0L
						Else
							time = deadline - System.nanoTime()
							If time <= 0L Then Return cancelWaiter(node, node, False)
							End If
						Dim wt As Thread = Thread.CurrentThread
						U.putObject(wt, PARKBLOCKER, Me)
						node.thread_Renamed = wt
						If p.status < 0 AndAlso (p IsNot h OrElse (state And ABITS) <> 0L) AndAlso whead Is h AndAlso node.prev Is p Then U.park(False, time) ' emulate LockSupport.park
						node.thread_Renamed = Nothing
						U.putObject(wt, PARKBLOCKER, Nothing)
						If interruptible AndAlso Thread.interrupted() Then Return cancelWaiter(node, node, True)
						End If
				End If
			Loop
		End Function

		''' <summary>
		''' See above for explanation.
		''' </summary>
		''' <param name="interruptible"> true if should check interrupts and if so
		''' return INTERRUPTED </param>
		''' <param name="deadline"> if nonzero, the System.nanoTime value to timeout
		''' at (and return zero) </param>
		''' <returns> next state, or INTERRUPTED </returns>
		Private Function acquireRead(ByVal interruptible As Boolean, ByVal deadline As Long) As Long
			Dim node As WNode = Nothing, p As WNode
			Dim spins_Renamed As Integer = -1
			Do
				Dim h As WNode
				h = whead
				p = wtail
				If h Is p Then
					Dim m As Long
					s
					ns
					Do
'JAVA TO VB CONVERTER TODO TASK: Assignments within expressions are not supported in VB
						m = (s = state) And ABITS
						ns = tryIncReaderOverflow(s)
'JAVA TO VB CONVERTER TODO TASK: Assignments within expressions are not supported in VB
						If If(m < RFULL, U.compareAndSwapLong(Me, STATE, s, ns = s + RUNIT), (m < WBIT AndAlso ns <> 0L)) Then
							Return ns
						ElseIf m >= WBIT Then
							If spins_Renamed > 0 Then
								If java.util.concurrent.locks.LockSupport.nextSecondarySeed() >= 0 Then spins_Renamed -= 1
							Else
								If spins_Renamed = 0 Then
									Dim nh As WNode = whead, np As WNode = wtail
									h = nh
									p = np
									If (nh Is h AndAlso np Is p) OrElse h IsNot p Then Exit Do
								End If
								spins_Renamed = SPINS
							End If
						End If
					Loop
				End If
				If p Is Nothing Then ' initialize queue
					Dim hd As New WNode(WMODE, Nothing)
					If U.compareAndSwapObject(Me, WHEAD, Nothing, hd) Then wtail = hd
				ElseIf node Is Nothing Then
					node = New WNode(RMODE, p)
				ElseIf h Is p OrElse p.mode <> RMODE Then
					If node.prev IsNot p Then
						node.prev = p
					ElseIf U.compareAndSwapObject(Me, WTAIL, p, node) Then
						p.next = node
						Exit Do
					End If
'JAVA TO VB CONVERTER TODO TASK: Assignments within expressions are not supported in VB
				ElseIf Not U.compareAndSwapObject(p, WCOWAIT, node.cowait = p.cowait, node) Then
					node.cowait = Nothing
				Else
					Do
						Dim pp, c As WNode
						Dim w As Thread
						h = whead
						c = h.cowait
						w = c.thread_Renamed
						If h IsNot Nothing AndAlso c IsNot Nothing AndAlso U.compareAndSwapObject(h, WCOWAIT, c, c.cowait) AndAlso w IsNot Nothing Then ' help release U.unpark(w)
						pp = p.prev
						If h Is pp OrElse h Is p OrElse pp Is Nothing Then
							Dim m, s, ns As Long
							Do
'JAVA TO VB CONVERTER TODO TASK: Assignments within expressions are not supported in VB
								m = (s = state) And ABITS
								ns = tryIncReaderOverflow(s)
'JAVA TO VB CONVERTER TODO TASK: Assignments within expressions are not supported in VB
								If If(m < RFULL, U.compareAndSwapLong(Me, STATE, s, ns = s + RUNIT), (m < WBIT AndAlso ns <> 0L)) Then Return ns
							Loop While m < WBIT
						End If
						If whead Is h AndAlso p.prev Is pp Then
							Dim time As Long
							If pp Is Nothing OrElse h Is p OrElse p.status > 0 Then
								node = Nothing ' throw away
								Exit Do
							End If
							If deadline = 0L Then
								time = 0L
							Else
								time = deadline - System.nanoTime()
								If time <= 0L Then Return cancelWaiter(node, p, False)
								End If
							Dim wt As Thread = Thread.CurrentThread
							U.putObject(wt, PARKBLOCKER, Me)
							node.thread_Renamed = wt
							If (h IsNot pp OrElse (state And ABITS) = WBIT) AndAlso whead Is h AndAlso p.prev Is pp Then U.park(False, time)
							node.thread_Renamed = Nothing
							U.putObject(wt, PARKBLOCKER, Nothing)
							If interruptible AndAlso Thread.interrupted() Then Return cancelWaiter(node, p, True)
						End If
					Loop
				End If
			Loop

			spins_Renamed = -1
			Do
				Dim h, np, pp As WNode
				Dim ps As Integer
				h = whead
				If h Is p Then
					If spins_Renamed < 0 Then
						spins_Renamed = HEAD_SPINS
					ElseIf spins_Renamed < MAX_HEAD_SPINS Then
						spins_Renamed <<= 1
					End If
					Dim k As Integer = spins_Renamed
					Do ' spin at head
						Dim m, s, ns As Long
'JAVA TO VB CONVERTER TODO TASK: Assignments within expressions are not supported in VB
						m = (s = state) And ABITS
						ns = tryIncReaderOverflow(s)
'JAVA TO VB CONVERTER TODO TASK: Assignments within expressions are not supported in VB
						If If(m < RFULL, U.compareAndSwapLong(Me, STATE, s, ns = s + RUNIT), (m < WBIT AndAlso ns <> 0L)) Then
							Dim c As WNode
							Dim w As Thread
							whead = node
							node.prev = Nothing
							c = node.cowait
							Do While c IsNot Nothing
								w = c.thread_Renamed
								If U.compareAndSwapObject(node, WCOWAIT, c, c.cowait) AndAlso w IsNot Nothing Then U.unpark(w)
								c = node.cowait
							Loop
							Return ns
						Else
							k -= 1
							If m >= WBIT AndAlso java.util.concurrent.locks.LockSupport.nextSecondarySeed() >= 0 AndAlso k <= 0 Then Exit Do
							End If
					Loop
				ElseIf h IsNot Nothing Then
					Dim c As WNode
					Dim w As Thread
					c = h.cowait
					Do While c IsNot Nothing
						w = c.thread_Renamed
						If U.compareAndSwapObject(h, WCOWAIT, c, c.cowait) AndAlso w IsNot Nothing Then U.unpark(w)
						c = h.cowait
					Loop
				End If
				If whead Is h Then
					np = node.prev
					If np IsNot p Then
						If np IsNot Nothing Then
								p = np
								np.next = node
						End If
					Else
						ps = p.status
						If ps = 0 Then
							U.compareAndSwapInt(p, WSTATUS, 0, WAITING)
						ElseIf ps = CANCELLED Then
							pp = p.prev
							If pp IsNot Nothing Then
								node.prev = pp
								pp.next = node
							End If
						Else
							Dim time As Long
						End If
						If deadline = 0L Then
							time = 0L
						Else
							time = deadline - System.nanoTime()
							If time <= 0L Then Return cancelWaiter(node, node, False)
							End If
						Dim wt As Thread = Thread.CurrentThread
						U.putObject(wt, PARKBLOCKER, Me)
						node.thread_Renamed = wt
						If p.status < 0 AndAlso (p IsNot h OrElse (state And ABITS) = WBIT) AndAlso whead Is h AndAlso node.prev Is p Then U.park(False, time)
						node.thread_Renamed = Nothing
						U.putObject(wt, PARKBLOCKER, Nothing)
						If interruptible AndAlso Thread.interrupted() Then Return cancelWaiter(node, node, True)
						End If
				End If
			Loop
		End Function

		''' <summary>
		''' If node non-null, forces cancel status and unsplices it from
		''' queue if possible and wakes up any cowaiters (of the node, or
		''' group, as applicable), and in any case helps release current
		''' first waiter if lock is free. (Calling with null arguments
		''' serves as a conditional form of release, which is not currently
		''' needed but may be needed under possible future cancellation
		''' policies). This is a variant of cancellation methods in
		''' AbstractQueuedSynchronizer (see its detailed explanation in AQS
		''' internal documentation).
		''' </summary>
		''' <param name="node"> if nonnull, the waiter </param>
		''' <param name="group"> either node or the group node is cowaiting with </param>
		''' <param name="interrupted"> if already interrupted </param>
		''' <returns> INTERRUPTED if interrupted or Thread.interrupted, else zero </returns>
		Private Function cancelWaiter(ByVal node As WNode, ByVal group As WNode, ByVal interrupted As Boolean) As Long
			If node IsNot Nothing AndAlso group IsNot Nothing Then
				Dim w As Thread
				node.status = CANCELLED
				' unsplice cancelled nodes from group
'JAVA TO VB CONVERTER TODO TASK: The following line could not be converted:
				for (WNode p = group, q; (q = p.cowait) != Nothing;)
					If q.status = CANCELLED Then
						U.compareAndSwapObject(p, WCOWAIT, q, q.cowait)
						p = group ' restart
					Else
						p = q
					End If
				If group Is node Then
					Dim r As WNode = group.cowait
					Do While r IsNot Nothing
						w = r.thread_Renamed
						If w IsNot Nothing Then U.unpark(w) ' wake up uncancelled co-waiters
						r = r.cowait
					Loop
					Dim pred As WNode = node.prev
					Do While pred IsNot Nothing ' unsplice
						Dim succ, pp As WNode ' find valid successor
						succ = node.next
						Do While succ Is Nothing OrElse succ.status = CANCELLED
							Dim q As WNode = Nothing ' find successor the slow way
							Dim t As WNode = wtail
							Do While t IsNot Nothing AndAlso t IsNot node
								If t.status <> CANCELLED Then q = t ' don't link if succ cancelled
								t = t.prev
							Loop
'JAVA TO VB CONVERTER TODO TASK: Assignments within expressions are not supported in VB
							If succ Is q OrElse U.compareAndSwapObject(node, WNEXT, succ, succ = q) Then ' ensure accurate successor
								If succ Is Nothing AndAlso node Is wtail Then U.compareAndSwapObject(Me, WTAIL, node, pred)
								Exit Do
							End If
							succ = node.next
						Loop
						If pred.next Is node Then ' unsplice pred link U.compareAndSwapObject(pred, WNEXT, node, succ)
						w = succ.thread_Renamed
						If succ IsNot Nothing AndAlso w IsNot Nothing Then
							succ.thread_Renamed = Nothing
							U.unpark(w) ' wake up succ to observe new pred
						End If
						pp = pred.prev
						If pred.status <> CANCELLED OrElse pp Is Nothing Then Exit Do
						node.prev = pp ' repeat if new pred wrong/cancelled
						U.compareAndSwapObject(pp, WNEXT, pred, succ)
						pred = pp
					Loop
				End If
			End If
			Dim h As WNode ' Possibly release first waiter
			h = whead
			Do While h IsNot Nothing
				Dim s As Long ' similar to release() but check eligibility
				Dim q As WNode
				q = h.next
				If q Is Nothing OrElse q.status = CANCELLED Then
					Dim t As WNode = wtail
					Do While t IsNot Nothing AndAlso t IsNot h
						If t.status <= 0 Then q = t
						t = t.prev
					Loop
				End If
				If h Is whead Then
					s = state
					If q IsNot Nothing AndAlso h.status = 0 AndAlso (s And ABITS) <> WBIT AndAlso (s = 0L OrElse q.mode = RMODE) Then ' waiter is eligible release(h)
					Exit Do
				End If
				h = whead
			Loop
			Return If(interrupted OrElse Thread.interrupted(), StampedLock.INTERRUPTED, 0L)
		End Function

		' Unsafe mechanics
		Private Shared ReadOnly U As sun.misc.Unsafe
		Private Shared ReadOnly STATE As Long
		Private Shared ReadOnly WHEAD As Long
		Private Shared ReadOnly WTAIL As Long
		Private Shared ReadOnly WNEXT As Long
		Private Shared ReadOnly WSTATUS As Long
		Private Shared ReadOnly WCOWAIT As Long
		Private Shared ReadOnly PARKBLOCKER As Long

		Shared Sub New()
			Try
				U = sun.misc.Unsafe.unsafe
				Dim k As  [Class] = GetType(StampedLock)
				Dim wk As  [Class] = GetType(WNode)
				STATE = U.objectFieldOffset(k.getDeclaredField("state"))
				WHEAD = U.objectFieldOffset(k.getDeclaredField("whead"))
				WTAIL = U.objectFieldOffset(k.getDeclaredField("wtail"))
				WSTATUS = U.objectFieldOffset(wk.getDeclaredField("status"))
				WNEXT = U.objectFieldOffset(wk.getDeclaredField("next"))
				WCOWAIT = U.objectFieldOffset(wk.getDeclaredField("cowait"))
				Dim tk As  [Class] = GetType(Thread)
				PARKBLOCKER = U.objectFieldOffset(tk.getDeclaredField("parkBlocker"))

			Catch e As Exception
				Throw New [Error](e)
			End Try
		End Sub
	End Class

End Namespace