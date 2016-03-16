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

Namespace java.util.concurrent


	''' <summary>
	''' A reusable synchronization barrier, similar in functionality to
	''' <seealso cref="java.util.concurrent.CyclicBarrier CyclicBarrier"/> and
	''' <seealso cref="java.util.concurrent.CountDownLatch CountDownLatch"/>
	''' but supporting more flexible usage.
	''' 
	''' <p><b>Registration.</b> Unlike the case for other barriers, the
	''' number of parties <em>registered</em> to synchronize on a phaser
	''' may vary over time.  Tasks may be registered at any time (using
	''' methods <seealso cref="#register"/>, <seealso cref="#bulkRegister"/>, or forms of
	''' constructors establishing initial numbers of parties), and
	''' optionally deregistered upon any arrival (using {@link
	''' #arriveAndDeregister}).  As is the case with most basic
	''' synchronization constructs, registration and deregistration affect
	''' only internal counts; they do not establish any further internal
	''' bookkeeping, so tasks cannot query whether they are registered.
	''' (However, you can introduce such bookkeeping by subclassing this
	''' class.)
	''' 
	''' <p><b>Synchronization.</b> Like a {@code CyclicBarrier}, a {@code
	''' Phaser} may be repeatedly awaited.  Method {@link
	''' #arriveAndAwaitAdvance} has effect analogous to {@link
	''' java.util.concurrent.CyclicBarrier#await CyclicBarrier.await}. Each
	''' generation of a phaser has an associated phase number. The phase
	''' number starts at zero, and advances when all parties arrive at the
	''' phaser, wrapping around to zero after reaching {@code
	'''  java.lang.[Integer].MAX_VALUE}. The use of phase numbers enables independent
	''' control of actions upon arrival at a phaser and upon awaiting
	''' others, via two kinds of methods that may be invoked by any
	''' registered party:
	''' 
	''' <ul>
	''' 
	'''   <li> <b>Arrival.</b> Methods <seealso cref="#arrive"/> and
	'''       <seealso cref="#arriveAndDeregister"/> record arrival.  These methods
	'''       do not block, but return an associated <em>arrival phase
	'''       number</em>; that is, the phase number of the phaser to which
	'''       the arrival applied. When the final party for a given phase
	'''       arrives, an optional action is performed and the phase
	'''       advances.  These actions are performed by the party
	'''       triggering a phase advance, and are arranged by overriding
	'''       method <seealso cref="#onAdvance(int, int)"/>, which also controls
	'''       termination. Overriding this method is similar to, but more
	'''       flexible than, providing a barrier action to a {@code
	'''       CyclicBarrier}.
	''' 
	'''   <li> <b>Waiting.</b> Method <seealso cref="#awaitAdvance"/> requires an
	'''       argument indicating an arrival phase number, and returns when
	'''       the phaser advances to (or is already at) a different phase.
	'''       Unlike similar constructions using {@code CyclicBarrier},
	'''       method {@code awaitAdvance} continues to wait even if the
	'''       waiting thread is interrupted. Interruptible and timeout
	'''       versions are also available, but exceptions encountered while
	'''       tasks wait interruptibly or with timeout do not change the
	'''       state of the phaser. If necessary, you can perform any
	'''       associated recovery within handlers of those exceptions,
	'''       often after invoking {@code forceTermination}.  Phasers may
	'''       also be used by tasks executing in a <seealso cref="ForkJoinPool"/>,
	'''       which will ensure sufficient parallelism to execute tasks
	'''       when others are blocked waiting for a phase to advance.
	''' 
	''' </ul>
	''' 
	''' <p><b>Termination.</b> A phaser may enter a <em>termination</em>
	''' state, that may be checked using method <seealso cref="#isTerminated"/>. Upon
	''' termination, all synchronization methods immediately return without
	''' waiting for advance, as indicated by a negative return value.
	''' Similarly, attempts to register upon termination have no effect.
	''' Termination is triggered when an invocation of {@code onAdvance}
	''' returns {@code true}. The default implementation returns {@code
	''' true} if a deregistration has caused the number of registered
	''' parties to become zero.  As illustrated below, when phasers control
	''' actions with a fixed number of iterations, it is often convenient
	''' to override this method to cause termination when the current phase
	''' number reaches a threshold. Method <seealso cref="#forceTermination"/> is
	''' also available to abruptly release waiting threads and allow them
	''' to terminate.
	''' 
	''' <p><b>Tiering.</b> Phasers may be <em>tiered</em> (i.e.,
	''' constructed in tree structures) to reduce contention. Phasers with
	''' large numbers of parties that would otherwise experience heavy
	''' synchronization contention costs may instead be set up so that
	''' groups of sub-phasers share a common parent.  This may greatly
	''' increase throughput even though it incurs greater per-operation
	''' overhead.
	''' 
	''' <p>In a tree of tiered phasers, registration and deregistration of
	''' child phasers with their parent are managed automatically.
	''' Whenever the number of registered parties of a child phaser becomes
	''' non-zero (as established in the <seealso cref="#Phaser(Phaser,int)"/>
	''' constructor, <seealso cref="#register"/>, or <seealso cref="#bulkRegister"/>), the
	''' child phaser is registered with its parent.  Whenever the number of
	''' registered parties becomes zero as the result of an invocation of
	''' <seealso cref="#arriveAndDeregister"/>, the child phaser is deregistered
	''' from its parent.
	''' 
	''' <p><b>Monitoring.</b> While synchronization methods may be invoked
	''' only by registered parties, the current state of a phaser may be
	''' monitored by any caller.  At any given moment there are {@link
	''' #getRegisteredParties} parties in total, of which {@link
	''' #getArrivedParties} have arrived at the current phase ({@link
	''' #getPhase}).  When the remaining (<seealso cref="#getUnarrivedParties"/>)
	''' parties arrive, the phase advances.  The values returned by these
	''' methods may reflect transient states and so are not in general
	''' useful for synchronization control.  Method <seealso cref="#toString"/>
	''' returns snapshots of these state queries in a form convenient for
	''' informal monitoring.
	''' 
	''' <p><b>Sample usages:</b>
	''' 
	''' <p>A {@code Phaser} may be used instead of a {@code CountDownLatch}
	''' to control a one-shot action serving a variable number of parties.
	''' The typical idiom is for the method setting this up to first
	''' register, then start the actions, then deregister, as in:
	''' 
	'''  <pre> {@code
	'''  Sub  runTasks(List<Runnable> tasks) {
	'''   final Phaser phaser = new Phaser(1); // "1" to register self
	'''   // create and start threads
	'''   for (final Runnable task : tasks) {
	'''     phaser.register();
	'''     new Thread() {
	'''       public  Sub  run() {
	'''         phaser.arriveAndAwaitAdvance(); // await all creation
	'''         task.run();
	'''       }
	'''     }.start();
	'''   }
	''' 
	'''   // allow threads to start and deregister self
	'''   phaser.arriveAndDeregister();
	''' }}</pre>
	''' 
	''' <p>One way to cause a set of threads to repeatedly perform actions
	''' for a given number of iterations is to override {@code onAdvance}:
	''' 
	'''  <pre> {@code
	'''  Sub  startTasks(List<Runnable> tasks, final int iterations) {
	'''   final Phaser phaser = new Phaser() {
	'''     protected boolean onAdvance(int phase, int registeredParties) {
	'''       return phase >= iterations || registeredParties == 0;
	'''     }
	'''   };
	'''   phaser.register();
	'''   for (final Runnable task : tasks) {
	'''     phaser.register();
	'''     new Thread() {
	'''       public  Sub  run() {
	'''         do {
	'''           task.run();
	'''           phaser.arriveAndAwaitAdvance();
	'''         } while (!phaser.isTerminated());
	'''       }
	'''     }.start();
	'''   }
	'''   phaser.arriveAndDeregister(); // deregister self, don't wait
	''' }}</pre>
	''' 
	''' If the main task must later await termination, it
	''' may re-register and then execute a similar loop:
	'''  <pre> {@code
	'''   // ...
	'''   phaser.register();
	'''   while (!phaser.isTerminated())
	'''     phaser.arriveAndAwaitAdvance();}</pre>
	''' 
	''' <p>Related constructions may be used to await particular phase numbers
	''' in contexts where you are sure that the phase will never wrap around
	''' {@code  java.lang.[Integer].MAX_VALUE}. For example:
	''' 
	'''  <pre> {@code
	'''  Sub  awaitPhase(Phaser phaser, int phase) {
	'''   int p = phaser.register(); // assumes caller not already registered
	'''   while (p < phase) {
	'''     if (phaser.isTerminated())
	'''       // ... deal with unexpected termination
	'''     else
	'''       p = phaser.arriveAndAwaitAdvance();
	'''   }
	'''   phaser.arriveAndDeregister();
	''' }}</pre>
	''' 
	''' 
	''' <p>To create a set of {@code n} tasks using a tree of phasers, you
	''' could use code of the following form, assuming a Task class with a
	''' constructor accepting a {@code Phaser} that it registers with upon
	''' construction. After invocation of {@code build(new Task[n], 0, n,
	''' new Phaser())}, these tasks could then be started, for example by
	''' submitting to a pool:
	''' 
	'''  <pre> {@code
	'''  Sub  build(Task[] tasks, int lo, int hi, Phaser ph) {
	'''   if (hi - lo > TASKS_PER_PHASER) {
	'''     for (int i = lo; i < hi; i += TASKS_PER_PHASER) {
	'''       int j = System.Math.min(i + TASKS_PER_PHASER, hi);
	'''       build(tasks, i, j, new Phaser(ph));
	'''     }
	'''   } else {
	'''     for (int i = lo; i < hi; ++i)
	'''       tasks[i] = new Task(ph);
	'''       // assumes new Task(ph) performs ph.register()
	'''   }
	''' }}</pre>
	''' 
	''' The best value of {@code TASKS_PER_PHASER} depends mainly on
	''' expected synchronization rates. A value as low as four may
	''' be appropriate for extremely small per-phase task bodies (thus
	''' high rates), or up to hundreds for extremely large ones.
	''' 
	''' <p><b>Implementation notes</b>: This implementation restricts the
	''' maximum number of parties to 65535. Attempts to register additional
	''' parties result in {@code IllegalStateException}. However, you can and
	''' should create tiered phasers to accommodate arbitrarily large sets
	''' of participants.
	''' 
	''' @since 1.7
	''' @author Doug Lea
	''' </summary>
	Public Class Phaser
	'    
	'     * This class implements an extension of X10 "clocks".  Thanks to
	'     * Vijay Saraswat for the idea, and to Vivek Sarkar for
	'     * enhancements to extend functionality.
	'     

		''' <summary>
		''' Primary state representation, holding four bit-fields:
		''' 
		''' unarrived  -- the number of parties yet to hit barrier (bits  0-15)
		''' parties    -- the number of parties to wait            (bits 16-31)
		''' phase      -- the generation of the barrier            (bits 32-62)
		''' terminated -- set if barrier is terminated             (bit  63 / sign)
		''' 
		''' Except that a phaser with no registered parties is
		''' distinguished by the otherwise illegal state of having zero
		''' parties and one unarrived parties (encoded as EMPTY below).
		''' 
		''' To efficiently maintain atomicity, these values are packed into
		''' a single (atomic) java.lang.[Long]. Good performance relies on keeping
		''' state decoding and encoding simple, and keeping race windows
		'''  java.lang.[Short].
		''' 
		''' All state updates are performed via CAS except initial
		''' registration of a sub-phaser (i.e., one with a non-null
		''' parent).  In this (relatively rare) case, we use built-in
		''' synchronization to lock while first registering with its
		''' parent.
		''' 
		''' The phase of a subphaser is allowed to lag that of its
		''' ancestors until it is actually accessed -- see method
		''' reconcileState.
		''' </summary>
'JAVA TO VB CONVERTER TODO TASK: There is no VB equivalent to 'volatile':
		Private state As Long

		Private Const MAX_PARTIES As Integer = &Hffff
		Private Shared ReadOnly MAX_PHASE As Integer =  java.lang.[Integer].MAX_VALUE
		Private Const PARTIES_SHIFT As Integer = 16
		Private Const PHASE_SHIFT As Integer = 32
		Private Const UNARRIVED_MASK As Integer = &Hffff ' to mask ints
		Private Const PARTIES_MASK As Long = &Hffff0000L ' to mask longs
		Private Const COUNTS_MASK As Long = &HffffffffL
		Private Shared ReadOnly TERMINATION_BIT As Long = 1L << 63

		' some special values
		Private Const ONE_ARRIVAL As Integer = 1
		Private Shared ReadOnly ONE_PARTY As Integer = 1 << PARTIES_SHIFT
		Private Shared ReadOnly ONE_DEREGISTER As Integer = ONE_ARRIVAL Or ONE_PARTY
		Private Const EMPTY As Integer = 1

		' The following unpacking methods are usually manually inlined

		Private Shared Function unarrivedOf(ByVal s As Long) As Integer
			Dim counts As Integer = CInt(s)
			Return If(counts = EMPTY, 0, (counts And UNARRIVED_MASK))
		End Function

		Private Shared Function partiesOf(ByVal s As Long) As Integer
			CInt(CUInt(Return CInt(s)) >> PARTIES_SHIFT)
		End Function

		Private Shared Function phaseOf(ByVal s As Long) As Integer
			Return CInt(CLng(CULng(s) >> PHASE_SHIFT))
		End Function

		Private Shared Function arrivedOf(ByVal s As Long) As Integer
			Dim counts As Integer = CInt(s)
			Return If(counts = EMPTY, 0, (CInt(CUInt(counts) >> PARTIES_SHIFT)) - (counts And UNARRIVED_MASK))
		End Function

		''' <summary>
		''' The parent of this phaser, or null if none
		''' </summary>
		Private ReadOnly parent As Phaser

		''' <summary>
		''' The root of phaser tree. Equals this if not in a tree.
		''' </summary>
		Private ReadOnly root As Phaser

		''' <summary>
		''' Heads of Treiber stacks for waiting threads. To eliminate
		''' contention when releasing some threads while adding others, we
		''' use two of them, alternating across even and odd phases.
		''' Subphasers share queues with root to speed up releases.
		''' </summary>
		Private ReadOnly evenQ As java.util.concurrent.atomic.AtomicReference(Of QNode)
		Private ReadOnly oddQ As java.util.concurrent.atomic.AtomicReference(Of QNode)

		Private Function queueFor(ByVal phase As Integer) As java.util.concurrent.atomic.AtomicReference(Of QNode)
			Return If((phase And 1) = 0, evenQ, oddQ)
		End Function

		''' <summary>
		''' Returns message string for bounds exceptions on arrival.
		''' </summary>
		Private Function badArrive(ByVal s As Long) As String
			Return "Attempted arrival of unregistered party for " & stateToString(s)
		End Function

		''' <summary>
		''' Returns message string for bounds exceptions on registration.
		''' </summary>
		Private Function badRegister(ByVal s As Long) As String
			Return "Attempt to register more than " & MAX_PARTIES & " parties for " & stateToString(s)
		End Function

		''' <summary>
		''' Main implementation for methods arrive and arriveAndDeregister.
		''' Manually tuned to speed up and minimize race windows for the
		''' common case of just decrementing unarrived field.
		''' </summary>
		''' <param name="adjust"> value to subtract from state;
		'''               ONE_ARRIVAL for arrive,
		'''               ONE_DEREGISTER for arriveAndDeregister </param>
		Private Function doArrive(ByVal adjust As Integer) As Integer
			Dim root_Renamed As Phaser = Me.root
			Do
				Dim s As Long = If(root_Renamed Is Me, state, reconcileState())
				Dim phase_Renamed As Integer = CInt(CLng(CULng(s) >> PHASE_SHIFT))
				If phase_Renamed < 0 Then Return phase_Renamed
				Dim counts As Integer = CInt(s)
				Dim unarrived As Integer = If(counts = EMPTY, 0, (counts And UNARRIVED_MASK))
				If unarrived <= 0 Then Throw New IllegalStateException(badArrive(s))
'JAVA TO VB CONVERTER TODO TASK: Assignments within expressions are not supported in VB
				If UNSAFE.compareAndSwapLong(Me, stateOffset, s, s-=adjust) Then
					If unarrived = 1 Then
						Dim n As Long = s And PARTIES_MASK ' base of next state
						Dim nextUnarrived As Integer = CInt(CUInt(CInt(n)) >> PARTIES_SHIFT)
						If root_Renamed Is Me Then
							If onAdvance(phase_Renamed, nextUnarrived) Then
								n = n Or TERMINATION_BIT
							ElseIf nextUnarrived = 0 Then
								n = n Or EMPTY
							Else
								n = n Or nextUnarrived
							End If
							Dim nextPhase As Integer = (phase_Renamed + 1) And MAX_PHASE
							n = n Or CLng(nextPhase) << PHASE_SHIFT
							UNSAFE.compareAndSwapLong(Me, stateOffset, s, n)
							releaseWaiters(phase_Renamed)
						ElseIf nextUnarrived = 0 Then ' propagate deregistration
							phase_Renamed = parent.doArrive(ONE_DEREGISTER)
							UNSAFE.compareAndSwapLong(Me, stateOffset, s, s Or EMPTY)
						Else
							phase_Renamed = parent.doArrive(ONE_ARRIVAL)
						End If
					End If
					Return phase_Renamed
				End If
			Loop
		End Function

		''' <summary>
		''' Implementation of register, bulkRegister
		''' </summary>
		''' <param name="registrations"> number to add to both parties and
		''' unarrived fields. Must be greater than zero. </param>
		Private Function doRegister(ByVal registrations As Integer) As Integer
			' adjustment to state
			Dim adjust As Long = (CLng(registrations) << PARTIES_SHIFT) Or registrations
			Dim parent_Renamed As Phaser = Me.parent
			Dim phase_Renamed As Integer
			Do
				Dim s As Long = If(parent_Renamed Is Nothing, state, reconcileState())
				Dim counts As Integer = CInt(s)
				Dim parties As Integer = CInt(CUInt(counts) >> PARTIES_SHIFT)
				Dim unarrived As Integer = counts And UNARRIVED_MASK
				If registrations > MAX_PARTIES - parties Then Throw New IllegalStateException(badRegister(s))
				phase_Renamed = CInt(CLng(CULng(s) >> PHASE_SHIFT))
				If phase_Renamed < 0 Then Exit Do
				If counts <> EMPTY Then ' not 1st registration
					If parent_Renamed Is Nothing OrElse reconcileState() = s Then
						If unarrived = 0 Then ' wait out advance
							root.internalAwaitAdvance(phase_Renamed, Nothing)
						ElseIf UNSAFE.compareAndSwapLong(Me, stateOffset, s, s + adjust) Then
							Exit Do
						End If
					End If
				ElseIf parent_Renamed Is Nothing Then ' 1st root registration
					Dim [next] As Long = (CLng(phase_Renamed) << PHASE_SHIFT) Or adjust
					If UNSAFE.compareAndSwapLong(Me, stateOffset, s, [next]) Then Exit Do
				Else
					SyncLock Me ' 1st sub registration
						If state = s Then ' recheck under lock
							phase_Renamed = parent_Renamed.doRegister(1)
							If phase_Renamed < 0 Then Exit Do
							' finish registration whenever parent registration
							' succeeded, even when racing with termination,
							' since these are part of the same "transaction".
							Do While Not UNSAFE.compareAndSwapLong(Me, stateOffset, s, (CLng(phase_Renamed) << PHASE_SHIFT) Or adjust)
								s = state
								phase_Renamed = CInt(CInt(CUInt(root.state) >> PHASE_SHIFT))
								' assert (int)s == EMPTY;
							Loop
							Exit Do
						End If
					End SyncLock
				End If
			Loop
			Return phase_Renamed
		End Function

		''' <summary>
		''' Resolves lagged phase propagation from root if necessary.
		''' Reconciliation normally occurs when root has advanced but
		''' subphasers have not yet done so, in which case they must finish
		''' their own advance by setting unarrived to parties (or if
		''' parties is zero, resetting to unregistered EMPTY state).
		''' </summary>
		''' <returns> reconciled state </returns>
		Private Function reconcileState() As Long
			Dim root_Renamed As Phaser = Me.root
			Dim s As Long = state
			If root_Renamed IsNot Me Then
				Dim phase_Renamed, p As Integer
				' CAS to root phase with current parties, tripping unarrived
				phase_Renamed = CInt(CInt(CUInt(root_Renamed.state) >> PHASE_SHIFT))
				p = CInt(CUInt(CInt(s)) >> PARTIES_SHIFT)
'JAVA TO VB CONVERTER TODO TASK: Assignments within expressions are not supported in VB
				Do While phase_Renamed <> CInt(CLng(CULng(s) >> PHASE_SHIFT)) AndAlso Not UNSAFE.compareAndSwapLong(Me, stateOffset, s, s = ((CLng(phase_Renamed) << PHASE_SHIFT) Or (If(phase_Renamed < 0, (s And COUNTS_MASK), (If(p = 0, EMPTY, ((s And PARTIES_MASK) Or p)))))))
					s = state
					phase_Renamed = CInt(CInt(CUInt(root_Renamed.state) >> PHASE_SHIFT))
					p = CInt(CUInt(CInt(s)) >> PARTIES_SHIFT)
				Loop
			End If
			Return s
		End Function

		''' <summary>
		''' Creates a new phaser with no initially registered parties, no
		''' parent, and initial phase number 0. Any thread using this
		''' phaser will need to first register for it.
		''' </summary>
		Public Sub New()
			Me.New(Nothing, 0)
		End Sub

		''' <summary>
		''' Creates a new phaser with the given number of registered
		''' unarrived parties, no parent, and initial phase number 0.
		''' </summary>
		''' <param name="parties"> the number of parties required to advance to the
		''' next phase </param>
		''' <exception cref="IllegalArgumentException"> if parties less than zero
		''' or greater than the maximum number of parties supported </exception>
		Public Sub New(ByVal parties As Integer)
			Me.New(Nothing, parties)
		End Sub

		''' <summary>
		''' Equivalent to <seealso cref="#Phaser(Phaser, int) Phaser(parent, 0)"/>.
		''' </summary>
		''' <param name="parent"> the parent phaser </param>
		Public Sub New(ByVal parent As Phaser)
			Me.New(parent, 0)
		End Sub

		''' <summary>
		''' Creates a new phaser with the given parent and number of
		''' registered unarrived parties.  When the given parent is non-null
		''' and the given number of parties is greater than zero, this
		''' child phaser is registered with its parent.
		''' </summary>
		''' <param name="parent"> the parent phaser </param>
		''' <param name="parties"> the number of parties required to advance to the
		''' next phase </param>
		''' <exception cref="IllegalArgumentException"> if parties less than zero
		''' or greater than the maximum number of parties supported </exception>
		Public Sub New(ByVal parent As Phaser, ByVal parties As Integer)
			If CInt(CUInt(parties) >> PARTIES_SHIFT <> 0) Then Throw New IllegalArgumentException("Illegal number of parties")
			Dim phase_Renamed As Integer = 0
			Me.parent = parent
			If parent IsNot Nothing Then
				Dim root_Renamed As Phaser = parent.root
				Me.root = root_Renamed
				Me.evenQ = root_Renamed.evenQ
				Me.oddQ = root_Renamed.oddQ
				If parties <> 0 Then phase_Renamed = parent.doRegister(1)
			Else
				Me.root = Me
				Me.evenQ = New java.util.concurrent.atomic.AtomicReference(Of QNode)
				Me.oddQ = New java.util.concurrent.atomic.AtomicReference(Of QNode)
			End If
			Me.state = If(parties = 0, CLng(EMPTY), (CLng(phase_Renamed) << PHASE_SHIFT) Or (CLng(parties) << PARTIES_SHIFT) Or (CLng(parties)))
		End Sub

		''' <summary>
		''' Adds a new unarrived party to this phaser.  If an ongoing
		''' invocation of <seealso cref="#onAdvance"/> is in progress, this method
		''' may await its completion before returning.  If this phaser has
		''' a parent, and this phaser previously had no registered parties,
		''' this child phaser is also registered with its parent. If
		''' this phaser is terminated, the attempt to register has
		''' no effect, and a negative value is returned.
		''' </summary>
		''' <returns> the arrival phase number to which this registration
		''' applied.  If this value is negative, then this phaser has
		''' terminated, in which case registration has no effect. </returns>
		''' <exception cref="IllegalStateException"> if attempting to register more
		''' than the maximum supported number of parties </exception>
		Public Overridable Function register() As Integer
			Return doRegister(1)
		End Function

		''' <summary>
		''' Adds the given number of new unarrived parties to this phaser.
		''' If an ongoing invocation of <seealso cref="#onAdvance"/> is in progress,
		''' this method may await its completion before returning.  If this
		''' phaser has a parent, and the given number of parties is greater
		''' than zero, and this phaser previously had no registered
		''' parties, this child phaser is also registered with its parent.
		''' If this phaser is terminated, the attempt to register has no
		''' effect, and a negative value is returned.
		''' </summary>
		''' <param name="parties"> the number of additional parties required to
		''' advance to the next phase </param>
		''' <returns> the arrival phase number to which this registration
		''' applied.  If this value is negative, then this phaser has
		''' terminated, in which case registration has no effect. </returns>
		''' <exception cref="IllegalStateException"> if attempting to register more
		''' than the maximum supported number of parties </exception>
		''' <exception cref="IllegalArgumentException"> if {@code parties < 0} </exception>
		Public Overridable Function bulkRegister(ByVal parties As Integer) As Integer
			If parties < 0 Then Throw New IllegalArgumentException
			If parties = 0 Then Return phase
			Return doRegister(parties)
		End Function

		''' <summary>
		''' Arrives at this phaser, without waiting for others to arrive.
		''' 
		''' <p>It is a usage error for an unregistered party to invoke this
		''' method.  However, this error may result in an {@code
		''' IllegalStateException} only upon some subsequent operation on
		''' this phaser, if ever.
		''' </summary>
		''' <returns> the arrival phase number, or a negative value if terminated </returns>
		''' <exception cref="IllegalStateException"> if not terminated and the number
		''' of unarrived parties would become negative </exception>
		Public Overridable Function arrive() As Integer
			Return doArrive(ONE_ARRIVAL)
		End Function

		''' <summary>
		''' Arrives at this phaser and deregisters from it without waiting
		''' for others to arrive. Deregistration reduces the number of
		''' parties required to advance in future phases.  If this phaser
		''' has a parent, and deregistration causes this phaser to have
		''' zero parties, this phaser is also deregistered from its parent.
		''' 
		''' <p>It is a usage error for an unregistered party to invoke this
		''' method.  However, this error may result in an {@code
		''' IllegalStateException} only upon some subsequent operation on
		''' this phaser, if ever.
		''' </summary>
		''' <returns> the arrival phase number, or a negative value if terminated </returns>
		''' <exception cref="IllegalStateException"> if not terminated and the number
		''' of registered or unarrived parties would become negative </exception>
		Public Overridable Function arriveAndDeregister() As Integer
			Return doArrive(ONE_DEREGISTER)
		End Function

		''' <summary>
		''' Arrives at this phaser and awaits others. Equivalent in effect
		''' to {@code awaitAdvance(arrive())}.  If you need to await with
		''' interruption or timeout, you can arrange this with an analogous
		''' construction using one of the other forms of the {@code
		''' awaitAdvance} method.  If instead you need to deregister upon
		''' arrival, use {@code awaitAdvance(arriveAndDeregister())}.
		''' 
		''' <p>It is a usage error for an unregistered party to invoke this
		''' method.  However, this error may result in an {@code
		''' IllegalStateException} only upon some subsequent operation on
		''' this phaser, if ever.
		''' </summary>
		''' <returns> the arrival phase number, or the (negative)
		''' <seealso cref="#getPhase() current phase"/> if terminated </returns>
		''' <exception cref="IllegalStateException"> if not terminated and the number
		''' of unarrived parties would become negative </exception>
		Public Overridable Function arriveAndAwaitAdvance() As Integer
			' Specialization of doArrive+awaitAdvance eliminating some reads/paths
			Dim root_Renamed As Phaser = Me.root
			Do
				Dim s As Long = If(root_Renamed Is Me, state, reconcileState())
				Dim phase_Renamed As Integer = CInt(CLng(CULng(s) >> PHASE_SHIFT))
				If phase_Renamed < 0 Then Return phase_Renamed
				Dim counts As Integer = CInt(s)
				Dim unarrived As Integer = If(counts = EMPTY, 0, (counts And UNARRIVED_MASK))
				If unarrived <= 0 Then Throw New IllegalStateException(badArrive(s))
'JAVA TO VB CONVERTER TODO TASK: Assignments within expressions are not supported in VB
				If UNSAFE.compareAndSwapLong(Me, stateOffset, s, s -= ONE_ARRIVAL) Then
					If unarrived > 1 Then Return root_Renamed.internalAwaitAdvance(phase_Renamed, Nothing)
					If root_Renamed IsNot Me Then Return parent.arriveAndAwaitAdvance()
					Dim n As Long = s And PARTIES_MASK ' base of next state
					Dim nextUnarrived As Integer = CInt(CUInt(CInt(n)) >> PARTIES_SHIFT)
					If onAdvance(phase_Renamed, nextUnarrived) Then
						n = n Or TERMINATION_BIT
					ElseIf nextUnarrived = 0 Then
						n = n Or EMPTY
					Else
						n = n Or nextUnarrived
					End If
					Dim nextPhase As Integer = (phase_Renamed + 1) And MAX_PHASE
					n = n Or CLng(nextPhase) << PHASE_SHIFT
					If Not UNSAFE.compareAndSwapLong(Me, stateOffset, s, n) Then Return CInt(CLng(CULng(state) >> PHASE_SHIFT)) ' terminated
					releaseWaiters(phase_Renamed)
					Return nextPhase
				End If
			Loop
		End Function

		''' <summary>
		''' Awaits the phase of this phaser to advance from the given phase
		''' value, returning immediately if the current phase is not equal
		''' to the given phase value or this phaser is terminated.
		''' </summary>
		''' <param name="phase"> an arrival phase number, or negative value if
		''' terminated; this argument is normally the value returned by a
		''' previous call to {@code arrive} or {@code arriveAndDeregister}. </param>
		''' <returns> the next arrival phase number, or the argument if it is
		''' negative, or the (negative) <seealso cref="#getPhase() current phase"/>
		''' if terminated </returns>
		Public Overridable Function awaitAdvance(ByVal phase As Integer) As Integer
			Dim root_Renamed As Phaser = Me.root
			Dim s As Long = If(root_Renamed Is Me, state, reconcileState())
			Dim p As Integer = CInt(CLng(CULng(s) >> PHASE_SHIFT))
			If phase < 0 Then Return phase
			If p = phase Then Return root_Renamed.internalAwaitAdvance(phase, Nothing)
			Return p
		End Function

		''' <summary>
		''' Awaits the phase of this phaser to advance from the given phase
		''' value, throwing {@code InterruptedException} if interrupted
		''' while waiting, or returning immediately if the current phase is
		''' not equal to the given phase value or this phaser is
		''' terminated.
		''' </summary>
		''' <param name="phase"> an arrival phase number, or negative value if
		''' terminated; this argument is normally the value returned by a
		''' previous call to {@code arrive} or {@code arriveAndDeregister}. </param>
		''' <returns> the next arrival phase number, or the argument if it is
		''' negative, or the (negative) <seealso cref="#getPhase() current phase"/>
		''' if terminated </returns>
		''' <exception cref="InterruptedException"> if thread interrupted while waiting </exception>
		Public Overridable Function awaitAdvanceInterruptibly(ByVal phase As Integer) As Integer
			Dim root_Renamed As Phaser = Me.root
			Dim s As Long = If(root_Renamed Is Me, state, reconcileState())
			Dim p As Integer = CInt(CLng(CULng(s) >> PHASE_SHIFT))
			If phase < 0 Then Return phase
			If p = phase Then
				Dim node As New QNode(Me, phase, True, False, 0L)
				p = root_Renamed.internalAwaitAdvance(phase, node)
				If node.wasInterrupted Then Throw New InterruptedException
			End If
			Return p
		End Function

		''' <summary>
		''' Awaits the phase of this phaser to advance from the given phase
		''' value or the given timeout to elapse, throwing {@code
		''' InterruptedException} if interrupted while waiting, or
		''' returning immediately if the current phase is not equal to the
		''' given phase value or this phaser is terminated.
		''' </summary>
		''' <param name="phase"> an arrival phase number, or negative value if
		''' terminated; this argument is normally the value returned by a
		''' previous call to {@code arrive} or {@code arriveAndDeregister}. </param>
		''' <param name="timeout"> how long to wait before giving up, in units of
		'''        {@code unit} </param>
		''' <param name="unit"> a {@code TimeUnit} determining how to interpret the
		'''        {@code timeout} parameter </param>
		''' <returns> the next arrival phase number, or the argument if it is
		''' negative, or the (negative) <seealso cref="#getPhase() current phase"/>
		''' if terminated </returns>
		''' <exception cref="InterruptedException"> if thread interrupted while waiting </exception>
		''' <exception cref="TimeoutException"> if timed out while waiting </exception>
		Public Overridable Function awaitAdvanceInterruptibly(ByVal phase As Integer, ByVal timeout As Long, ByVal unit As java.util.concurrent.TimeUnit) As Integer
			Dim nanos As Long = unit.toNanos(timeout)
			Dim root_Renamed As Phaser = Me.root
			Dim s As Long = If(root_Renamed Is Me, state, reconcileState())
			Dim p As Integer = CInt(CLng(CULng(s) >> PHASE_SHIFT))
			If phase < 0 Then Return phase
			If p = phase Then
				Dim node As New QNode(Me, phase, True, True, nanos)
				p = root_Renamed.internalAwaitAdvance(phase, node)
				If node.wasInterrupted Then
					Throw New InterruptedException
				ElseIf p = phase Then
					Throw New java.util.concurrent.TimeoutException
				End If
			End If
			Return p
		End Function

		''' <summary>
		''' Forces this phaser to enter termination state.  Counts of
		''' registered parties are unaffected.  If this phaser is a member
		''' of a tiered set of phasers, then all of the phasers in the set
		''' are terminated.  If this phaser is already terminated, this
		''' method has no effect.  This method may be useful for
		''' coordinating recovery after one or more tasks encounter
		''' unexpected exceptions.
		''' </summary>
		Public Overridable Sub forceTermination()
			' Only need to change root state
			Dim root_Renamed As Phaser = Me.root
			Dim s As Long
			s = root_Renamed.state
			Do While s >= 0
				If UNSAFE.compareAndSwapLong(root_Renamed, stateOffset, s, s Or TERMINATION_BIT) Then
					' signal all threads
					releaseWaiters(0) ' Waiters on evenQ
					releaseWaiters(1) ' Waiters on oddQ
					Return
				End If
				s = root_Renamed.state
			Loop
		End Sub

		''' <summary>
		''' Returns the current phase number. The maximum phase number is
		''' {@code  java.lang.[Integer].MAX_VALUE}, after which it restarts at
		''' zero. Upon termination, the phase number is negative,
		''' in which case the prevailing phase prior to termination
		''' may be obtained via {@code getPhase() +  java.lang.[Integer].MIN_VALUE}.
		''' </summary>
		''' <returns> the phase number, or a negative value if terminated </returns>
		Public Property phase As Integer
			Get
				Return CInt(CInt(CUInt(root.state) >> PHASE_SHIFT))
			End Get
		End Property

		''' <summary>
		''' Returns the number of parties registered at this phaser.
		''' </summary>
		''' <returns> the number of parties </returns>
		Public Overridable Property registeredParties As Integer
			Get
				Return partiesOf(state)
			End Get
		End Property

		''' <summary>
		''' Returns the number of registered parties that have arrived at
		''' the current phase of this phaser. If this phaser has terminated,
		''' the returned value is meaningless and arbitrary.
		''' </summary>
		''' <returns> the number of arrived parties </returns>
		Public Overridable Property arrivedParties As Integer
			Get
				Return arrivedOf(reconcileState())
			End Get
		End Property

		''' <summary>
		''' Returns the number of registered parties that have not yet
		''' arrived at the current phase of this phaser. If this phaser has
		''' terminated, the returned value is meaningless and arbitrary.
		''' </summary>
		''' <returns> the number of unarrived parties </returns>
		Public Overridable Property unarrivedParties As Integer
			Get
				Return unarrivedOf(reconcileState())
			End Get
		End Property

		''' <summary>
		''' Returns the parent of this phaser, or {@code null} if none.
		''' </summary>
		''' <returns> the parent of this phaser, or {@code null} if none </returns>
		Public Overridable Property parent As Phaser
			Get
				Return parent
			End Get
		End Property

		''' <summary>
		''' Returns the root ancestor of this phaser, which is the same as
		''' this phaser if it has no parent.
		''' </summary>
		''' <returns> the root ancestor of this phaser </returns>
		Public Overridable Property root As Phaser
			Get
				Return root
			End Get
		End Property

		''' <summary>
		''' Returns {@code true} if this phaser has been terminated.
		''' </summary>
		''' <returns> {@code true} if this phaser has been terminated </returns>
		Public Overridable Property terminated As Boolean
			Get
				Return root.state < 0L
			End Get
		End Property

		''' <summary>
		''' Overridable method to perform an action upon impending phase
		''' advance, and to control termination. This method is invoked
		''' upon arrival of the party advancing this phaser (when all other
		''' waiting parties are dormant).  If this method returns {@code
		''' true}, this phaser will be set to a final termination state
		''' upon advance, and subsequent calls to <seealso cref="#isTerminated"/>
		''' will return true. Any (unchecked) Exception or Error thrown by
		''' an invocation of this method is propagated to the party
		''' attempting to advance this phaser, in which case no advance
		''' occurs.
		''' 
		''' <p>The arguments to this method provide the state of the phaser
		''' prevailing for the current transition.  The effects of invoking
		''' arrival, registration, and waiting methods on this phaser from
		''' within {@code onAdvance} are unspecified and should not be
		''' relied on.
		''' 
		''' <p>If this phaser is a member of a tiered set of phasers, then
		''' {@code onAdvance} is invoked only for its root phaser on each
		''' advance.
		''' 
		''' <p>To support the most common use cases, the default
		''' implementation of this method returns {@code true} when the
		''' number of registered parties has become zero as the result of a
		''' party invoking {@code arriveAndDeregister}.  You can disable
		''' this behavior, thus enabling continuation upon future
		''' registrations, by overriding this method to always return
		''' {@code false}:
		''' 
		''' <pre> {@code
		''' Phaser phaser = new Phaser() {
		'''   protected boolean onAdvance(int phase, int parties) { return false; }
		''' }}</pre>
		''' </summary>
		''' <param name="phase"> the current phase number on entry to this method,
		''' before this phaser is advanced </param>
		''' <param name="registeredParties"> the current number of registered parties </param>
		''' <returns> {@code true} if this phaser should terminate </returns>
		Protected Friend Overridable Function onAdvance(ByVal phase As Integer, ByVal registeredParties As Integer) As Boolean
			Return registeredParties = 0
		End Function

		''' <summary>
		''' Returns a string identifying this phaser, as well as its
		''' state.  The state, in brackets, includes the String {@code
		''' "phase = "} followed by the phase number, {@code "parties = "}
		''' followed by the number of registered parties, and {@code
		''' "arrived = "} followed by the number of arrived parties.
		''' </summary>
		''' <returns> a string identifying this phaser, as well as its state </returns>
		Public Overrides Function ToString() As String
			Return stateToString(reconcileState())
		End Function

		''' <summary>
		''' Implementation of toString and string-based error messages
		''' </summary>
		Private Function stateToString(ByVal s As Long) As String
			Return MyBase.ToString() & "[phase = " & phaseOf(s) & " parties = " & partiesOf(s) & " arrived = " & arrivedOf(s) & "]"
		End Function

		' Waiting mechanics

		''' <summary>
		''' Removes and signals threads from queue for phase.
		''' </summary>
		Private Sub releaseWaiters(ByVal phase As Integer)
			Dim q As QNode ' first element of queue
			Dim t As Thread ' its thread
			Dim head As java.util.concurrent.atomic.AtomicReference(Of QNode) = If((phase And 1) = 0, evenQ, oddQ)
			q = head.get()
			Do While q IsNot Nothing AndAlso q.phase <> CInt(CInt(CUInt(root.state) >> PHASE_SHIFT))
				t = q.thread_Renamed
				If head.compareAndSet(q, q.next) AndAlso t IsNot Nothing Then
					q.thread_Renamed = Nothing
					java.util.concurrent.locks.LockSupport.unpark(t)
				End If
				q = head.get()
			Loop
		End Sub

		''' <summary>
		''' Variant of releaseWaiters that additionally tries to remove any
		''' nodes no longer waiting for advance due to timeout or
		''' interrupt. Currently, nodes are removed only if they are at
		''' head of queue, which suffices to reduce memory footprint in
		''' most usages.
		''' </summary>
		''' <returns> current phase on exit </returns>
		Private Function abortWait(ByVal phase As Integer) As Integer
			Dim head As java.util.concurrent.atomic.AtomicReference(Of QNode) = If((phase And 1) = 0, evenQ, oddQ)
			Do
				Dim t As Thread
				Dim q As QNode = head.get()
				Dim p As Integer = CInt(CInt(CUInt(root.state) >> PHASE_SHIFT))
				t = q.thread_Renamed
				If q Is Nothing OrElse (t IsNot Nothing AndAlso q.phase = p) Then Return p
				If head.compareAndSet(q, q.next) AndAlso t IsNot Nothing Then
					q.thread_Renamed = Nothing
					java.util.concurrent.locks.LockSupport.unpark(t)
				End If
			Loop
		End Function

		''' <summary>
		''' The number of CPUs, for spin control </summary>
		Private Shared ReadOnly NCPU As Integer = Runtime.runtime.availableProcessors()

		''' <summary>
		''' The number of times to spin before blocking while waiting for
		''' advance, per arrival while waiting. On multiprocessors, fully
		''' blocking and waking up a large number of threads all at once is
		''' usually a very slow process, so we use rechargeable spins to
		''' avoid it when threads regularly arrive: When a thread in
		''' internalAwaitAdvance notices another arrival before blocking,
		''' and there appear to be enough CPUs available, it spins
		''' SPINS_PER_ARRIVAL more times before blocking. The value trades
		''' off good-citizenship vs big unnecessary slowdowns.
		''' </summary>
		Friend Shared ReadOnly SPINS_PER_ARRIVAL As Integer = If(NCPU < 2, 1, 1 << 8)

		''' <summary>
		''' Possibly blocks and waits for phase to advance unless aborted.
		''' Call only on root phaser.
		''' </summary>
		''' <param name="phase"> current phase </param>
		''' <param name="node"> if non-null, the wait node to track interrupt and timeout;
		''' if null, denotes noninterruptible wait </param>
		''' <returns> current phase </returns>
		Private Function internalAwaitAdvance(ByVal phase As Integer, ByVal node As QNode) As Integer
			' assert root == this;
			releaseWaiters(phase-1) ' ensure old queue clean
			Dim queued As Boolean = False ' true when node is enqueued
			Dim lastUnarrived As Integer = 0 ' to increase spins upon change
			Dim spins As Integer = SPINS_PER_ARRIVAL
			Dim s As Long
			Dim p As Integer
'JAVA TO VB CONVERTER TODO TASK: Assignments within expressions are not supported in VB
			p = CInt(CInt(CUInt((s = state)) >> PHASE_SHIFT))
			Do While p = phase
				If node Is Nothing Then ' spinning in noninterruptible mode
					Dim unarrived As Integer = CInt(s) And UNARRIVED_MASK
					lastUnarrived = unarrived
					If unarrived <> lastUnarrived AndAlso lastUnarrived < NCPU Then spins += SPINS_PER_ARRIVAL
					Dim interrupted As Boolean = Thread.interrupted()
					spins -= 1
					If interrupted OrElse spins < 0 Then ' need node to record intr
						node = New QNode(Me, phase, False, False, 0L)
						node.wasInterrupted = interrupted
					End If
				ElseIf node.releasable Then ' done or aborted
					Exit Do
				ElseIf Not queued Then ' push onto queue
					Dim head As java.util.concurrent.atomic.AtomicReference(Of QNode) = If((phase And 1) = 0, evenQ, oddQ)
						node.next = head.get()
						Dim q As QNode = node.next
					If (q Is Nothing OrElse q.phase = phase) AndAlso CInt(CLng(CULng(state) >> PHASE_SHIFT)) = phase Then ' avoid stale enq queued = head.compareAndSet(q, node)
				Else
					Try
						ForkJoinPool.managedBlock(node)
					Catch ie As InterruptedException
						node.wasInterrupted = True
					End Try
				End If
				p = CInt(CInt(CUInt((s = state)) >> PHASE_SHIFT))
			Loop

			If node IsNot Nothing Then
				If node.thread_Renamed IsNot Nothing Then node.thread_Renamed = Nothing ' avoid need for unpark()
				If node.wasInterrupted AndAlso (Not node.interruptible) Then Thread.CurrentThread.Interrupt()
				p = CInt(CLng(CULng(state) >> PHASE_SHIFT))
				If p = phase AndAlso p = phase Then Return abortWait(phase) ' possibly clean up on abort
			End If
			releaseWaiters(phase)
			Return p
		End Function

		''' <summary>
		''' Wait nodes for Treiber stack representing wait queue
		''' </summary>
		Friend NotInheritable Class QNode
			Implements ForkJoinPool.ManagedBlocker

			Friend ReadOnly phaser_Renamed As Phaser
			Friend ReadOnly phase As Integer
			Friend ReadOnly interruptible As Boolean
			Friend ReadOnly timed As Boolean
			Friend wasInterrupted As Boolean
			Friend nanos As Long
			Friend ReadOnly deadline As Long
'JAVA TO VB CONVERTER TODO TASK: There is no VB equivalent to 'volatile':
			Friend thread_Renamed As Thread ' nulled to cancel wait
			Friend [next] As QNode

			Friend Sub New(ByVal phaser_Renamed As Phaser, ByVal phase As Integer, ByVal interruptible As Boolean, ByVal timed As Boolean, ByVal nanos As Long)
				Me.phaser_Renamed = phaser_Renamed
				Me.phase = phase
				Me.interruptible = interruptible
				Me.nanos = nanos
				Me.timed = timed
				Me.deadline = If(timed, System.nanoTime() + nanos, 0L)
				thread_Renamed = Thread.CurrentThread
			End Sub

			Public Property releasable As Boolean Implements ForkJoinPool.ManagedBlocker.isReleasable
				Get
					If thread_Renamed Is Nothing Then Return True
					If phaser_Renamed.phase <> phase Then
						thread_Renamed = Nothing
						Return True
					End If
					If Thread.interrupted() Then wasInterrupted = True
					If wasInterrupted AndAlso interruptible Then
						thread_Renamed = Nothing
						Return True
					End If
					If timed Then
						If nanos > 0L Then nanos = deadline - System.nanoTime()
						If nanos <= 0L Then
							thread_Renamed = Nothing
							Return True
						End If
					End If
					Return False
				End Get
			End Property

			Public Function block() As Boolean Implements ForkJoinPool.ManagedBlocker.block
				If releasable Then
					Return True
				ElseIf Not timed Then
					java.util.concurrent.locks.LockSupport.park(Me)
				ElseIf nanos > 0L Then
					java.util.concurrent.locks.LockSupport.parkNanos(Me, nanos)
				End If
				Return releasable
			End Function
		End Class

		' Unsafe mechanics

		Private Shared ReadOnly UNSAFE As sun.misc.Unsafe
		Private Shared ReadOnly stateOffset As Long
		Shared Sub New()
			Try
				UNSAFE = sun.misc.Unsafe.unsafe
				Dim k As  [Class] = GetType(Phaser)
				stateOffset = UNSAFE.objectFieldOffset(k.getDeclaredField("state"))
			Catch e As Exception
				Throw New [Error](e)
			End Try
		End Sub
	End Class

End Namespace