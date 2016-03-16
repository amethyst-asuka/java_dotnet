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
' * Written by Doug Lea, Bill Scherer, and Michael Scott with
' * assistance from members of JCP JSR-166 Expert Group and released to
' * the public domain, as explained at
' * http://creativecommons.org/publicdomain/zero/1.0/
' 

Namespace java.util.concurrent

	''' <summary>
	''' A synchronization point at which threads can pair and swap elements
	''' within pairs.  Each thread presents some object on entry to the
	''' <seealso cref="#exchange exchange"/> method, matches with a partner thread,
	''' and receives its partner's object on return.  An Exchanger may be
	''' viewed as a bidirectional form of a <seealso cref="SynchronousQueue"/>.
	''' Exchangers may be useful in applications such as genetic algorithms
	''' and pipeline designs.
	''' 
	''' <p><b>Sample Usage:</b>
	''' Here are the highlights of a class that uses an {@code Exchanger}
	''' to swap buffers between threads so that the thread filling the
	''' buffer gets a freshly emptied one when it needs it, handing off the
	''' filled one to the thread emptying the buffer.
	'''  <pre> {@code
	''' class FillAndEmpty {
	'''   Exchanger<DataBuffer> exchanger = new Exchanger<DataBuffer>();
	'''   DataBuffer initialEmptyBuffer = ... a made-up type
	'''   DataBuffer initialFullBuffer = ...
	''' 
	'''   class FillingLoop implements Runnable {
	'''     public  Sub  run() {
	'''       DataBuffer currentBuffer = initialEmptyBuffer;
	'''       try {
	'''         while (currentBuffer != null) {
	'''           addToBuffer(currentBuffer);
	'''           if (currentBuffer.isFull())
	'''             currentBuffer = exchanger.exchange(currentBuffer);
	'''         }
	'''       } catch (InterruptedException ex) { ... handle ... }
	'''     }
	'''   }
	''' 
	'''   class EmptyingLoop implements Runnable {
	'''     public  Sub  run() {
	'''       DataBuffer currentBuffer = initialFullBuffer;
	'''       try {
	'''         while (currentBuffer != null) {
	'''           takeFromBuffer(currentBuffer);
	'''           if (currentBuffer.isEmpty())
	'''             currentBuffer = exchanger.exchange(currentBuffer);
	'''         }
	'''       } catch (InterruptedException ex) { ... handle ...}
	'''     }
	'''   }
	''' 
	'''    Sub  start() {
	'''     new Thread(new FillingLoop()).start();
	'''     new Thread(new EmptyingLoop()).start();
	'''   }
	''' }}</pre>
	''' 
	''' <p>Memory consistency effects: For each pair of threads that
	''' successfully exchange objects via an {@code Exchanger}, actions
	''' prior to the {@code exchange()} in each thread
	''' <a href="package-summary.html#MemoryVisibility"><i>happen-before</i></a>
	''' those subsequent to a return from the corresponding {@code exchange()}
	''' in the other thread.
	''' 
	''' @since 1.5
	''' @author Doug Lea and Bill Scherer and Michael Scott </summary>
	''' @param <V> The type of objects that may be exchanged </param>
	Public Class Exchanger(Of V)

	'    
	'     * Overview: The core algorithm is, for an exchange "slot",
	'     * and a participant (caller) with an item:
	'     *
	'     * for (;;) {
	'     *   if (slot is empty) {                       // offer
	'     *     place item in a Node;
	'     *     if (can CAS slot from empty to node) {
	'     *       wait for release;
	'     *       return matching item in node;
	'     *     }
	'     *   }
	'     *   else if (can CAS slot from node to empty) { // release
	'     *     get the item in node;
	'     *     set matching item in node;
	'     *     release waiting thread;
	'     *   }
	'     *   // else retry on CAS failure
	'     * }
	'     *
	'     * This is among the simplest forms of a "dual data structure" --
	'     * see Scott and Scherer's DISC 04 paper and
	'     * http://www.cs.rochester.edu/research/synchronization/pseudocode/duals.html
	'     *
	'     * This works great in principle. But in practice, like many
	'     * algorithms centered on atomic updates to a single location, it
	'     * scales horribly when there are more than a few participants
	'     * using the same Exchanger. So the implementation instead uses a
	'     * form of elimination arena, that spreads out this contention by
	'     * arranging that some threads typically use different slots,
	'     * while still ensuring that eventually, any two parties will be
	'     * able to exchange items. That is, we cannot completely partition
	'     * across threads, but instead give threads arena indices that
	'     * will on average grow under contention and shrink under lack of
	'     * contention. We approach this by defining the Nodes that we need
	'     * anyway as ThreadLocals, and include in them per-thread index
	'     * and related bookkeeping state. (We can safely reuse per-thread
	'     * nodes rather than creating them fresh each time because slots
	'     * alternate between pointing to a node vs null, so cannot
	'     * encounter ABA problems. However, we do need some care in
	'     * resetting them between uses.)
	'     *
	'     * Implementing an effective arena requires allocating a bunch of
	'     * space, so we only do so upon detecting contention (except on
	'     * uniprocessors, where they wouldn't help, so aren't used).
	'     * Otherwise, exchanges use the single-slot slotExchange method.
	'     * On contention, not only must the slots be in different
	'     * locations, but the locations must not encounter memory
	'     * contention due to being on the same cache line (or more
	'     * generally, the same coherence unit).  Because, as of this
	'     * writing, there is no way to determine cacheline size, we define
	'     * a value that is enough for common platforms.  Additionally,
	'     * extra care elsewhere is taken to avoid other false/unintended
	'     * sharing and to enhance locality, including adding padding (via
	'     * sun.misc.Contended) to Nodes, embedding "bound" as an Exchanger
	'     * field, and reworking some park/unpark mechanics compared to
	'     * LockSupport versions.
	'     *
	'     * The arena starts out with only one used slot. We expand the
	'     * effective arena size by tracking collisions; i.e., failed CASes
	'     * while trying to exchange. By nature of the above algorithm, the
	'     * only kinds of collision that reliably indicate contention are
	'     * when two attempted releases collide -- one of two attempted
	'     * offers can legitimately fail to CAS without indicating
	'     * contention by more than one other thread. (Note: it is possible
	'     * but not worthwhile to more precisely detect contention by
	'     * reading slot values after CAS failures.)  When a thread has
	'     * collided at each slot within the current arena bound, it tries
	'     * to expand the arena size by one. We track collisions within
	'     * bounds by using a version (sequence) number on the "bound"
	'     * field, and conservatively reset collision counts when a
	'     * participant notices that bound has been updated (in either
	'     * direction).
	'     *
	'     * The effective arena size is reduced (when there is more than
	'     * one slot) by giving up on waiting after a while and trying to
	'     * decrement the arena size on expiration. The value of "a while"
	'     * is an empirical matter.  We implement by piggybacking on the
	'     * use of spin->yield->block that is essential for reasonable
	'     * waiting performance anyway -- in a busy exchanger, offers are
	'     * usually almost immediately released, in which case context
	'     * switching on multiprocessors is extremely slow/wasteful.  Arena
	'     * waits just omit the blocking part, and instead cancel. The spin
	'     * count is empirically chosen to be a value that avoids blocking
	'     * 99% of the time under maximum sustained exchange rates on a
	'     * range of test machines. Spins and yields entail some limited
	'     * randomness (using a cheap xorshift) to avoid regular patterns
	'     * that can induce unproductive grow/shrink cycles. (Using a
	'     * pseudorandom also helps regularize spin cycle duration by
	'     * making branches unpredictable.)  Also, during an offer, a
	'     * waiter can "know" that it will be released when its slot has
	'     * changed, but cannot yet proceed until match is set.  In the
	'     * mean time it cannot cancel the offer, so instead spins/yields.
	'     * Note: It is possible to avoid this secondary check by changing
	'     * the linearization point to be a CAS of the match field (as done
	'     * in one case in the Scott & Scherer DISC paper), which also
	'     * increases asynchrony a bit, at the expense of poorer collision
	'     * detection and inability to always reuse per-thread nodes. So
	'     * the current scheme is typically a better tradeoff.
	'     *
	'     * On collisions, indices traverse the arena cyclically in reverse
	'     * order, restarting at the maximum index (which will tend to be
	'     * sparsest) when bounds change. (On expirations, indices instead
	'     * are halved until reaching 0.) It is possible (and has been
	'     * tried) to use randomized, prime-value-stepped, or double-hash
	'     * style traversal instead of simple cyclic traversal to reduce
	'     * bunching.  But empirically, whatever benefits these may have
	'     * don't overcome their added overhead: We are managing operations
	'     * that occur very quickly unless there is sustained contention,
	'     * so simpler/faster control policies work better than more
	'     * accurate but slower ones.
	'     *
	'     * Because we use expiration for arena size control, we cannot
	'     * throw TimeoutExceptions in the timed version of the public
	'     * exchange method until the arena size has shrunken to zero (or
	'     * the arena isn't enabled). This may delay response to timeout
	'     * but is still within spec.
	'     *
	'     * Essentially all of the implementation is in methods
	'     * slotExchange and arenaExchange. These have similar overall
	'     * structure, but differ in too many details to combine. The
	'     * slotExchange method uses the single Exchanger field "slot"
	'     * rather than arena array elements. However, it still needs
	'     * minimal collision detection to trigger arena construction.
	'     * (The messiest part is making sure interrupt status and
	'     * InterruptedExceptions come out right during transitions when
	'     * both methods may be called. This is done by using null return
	'     * as a sentinel to recheck interrupt status.)
	'     *
	'     * As is too common in this sort of code, methods are monolithic
	'     * because most of the logic relies on reads of fields that are
	'     * maintained as local variables so can't be nicely factored --
	'     * mainly, here, bulky spin->yield->block/cancel code), and
	'     * heavily dependent on intrinsics (Unsafe) to use inlined
	'     * embedded CAS and related memory access operations (that tend
	'     * not to be as readily inlined by dynamic compilers when they are
	'     * hidden behind other methods that would more nicely name and
	'     * encapsulate the intended effects). This includes the use of
	'     * putOrderedX to clear fields of the per-thread Nodes between
	'     * uses. Note that field Node.item is not declared as volatile
	'     * even though it is read by releasing threads, because they only
	'     * do so after CAS operations that must precede access, and all
	'     * uses by the owning thread are otherwise acceptably ordered by
	'     * other operations. (Because the actual points of atomicity are
	'     * slot CASes, it would also be legal for the write to Node.match
	'     * in a release to be weaker than a full volatile write. However,
	'     * this is not done because it could allow further postponement of
	'     * the write, delaying progress.)
	'     

		''' <summary>
		''' The byte distance (as a shift value) between any two used slots
		''' in the arena.  1 << ASHIFT should be at least cacheline size.
		''' </summary>
		Private Const ASHIFT As Integer = 7

		''' <summary>
		''' The maximum supported arena index. The maximum allocatable
		''' arena size is MMASK + 1. Must be a power of two minus one, less
		''' than (1<<(31-ASHIFT)). The cap of 255 (0xff) more than suffices
		''' for the expected scaling limits of the main algorithms.
		''' </summary>
		Private Const MMASK As Integer = &Hff

		''' <summary>
		''' Unit for sequence/version bits of bound field. Each successful
		''' change to the bound also adds SEQ.
		''' </summary>
		Private Shared ReadOnly SEQ As Integer = MMASK + 1

		''' <summary>
		''' The number of CPUs, for sizing and spin control </summary>
		Private Shared ReadOnly NCPU As Integer = Runtime.runtime.availableProcessors()

		''' <summary>
		''' The maximum slot index of the arena: The number of slots that
		''' can in principle hold all threads without contention, or at
		''' most the maximum indexable value.
		''' </summary>
		Friend Shared ReadOnly FULL As Integer = If(NCPU >= (MMASK << 1), MMASK, NCPU >>> 1)

		''' <summary>
		''' The bound for spins while waiting for a match. The actual
		''' number of iterations will on average be about twice this value
		''' due to randomization. Note: Spinning is disabled when NCPU==1.
		''' </summary>
		Private Shared ReadOnly SPINS As Integer = 1 << 10

		''' <summary>
		''' Value representing null arguments/returns from public
		''' methods. Needed because the API originally didn't disallow null
		''' arguments, which it should have.
		''' </summary>
		Private Shared ReadOnly NULL_ITEM As New Object

		''' <summary>
		''' Sentinel value returned by internal exchange methods upon
		''' timeout, to avoid need for separate timed versions of these
		''' methods.
		''' </summary>
		Private Shared ReadOnly TIMED_OUT As New Object

		''' <summary>
		''' Nodes hold partially exchanged data, plus other per-thread
		''' bookkeeping. Padded via @sun.misc.Contended to reduce memory
		''' contention.
		''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
		Friend NotInheritable Class Node
			Friend index As Integer ' Arena index
			Friend bound As Integer ' Last recorded value of Exchanger.bound
			Friend collides As Integer ' Number of CAS failures at current bound
			Friend hash As Integer ' Pseudo-random for spins
			Friend item As Object ' This thread's current item
'JAVA TO VB CONVERTER TODO TASK: There is no VB equivalent to 'volatile':
			Friend match As Object ' Item provided by releasing thread
'JAVA TO VB CONVERTER TODO TASK: There is no VB equivalent to 'volatile':
			Friend parked As Thread ' Set to this thread when parked, else null
		End Class

		''' <summary>
		''' The corresponding thread local class </summary>
		Friend NotInheritable Class Participant
			Inherits ThreadLocal(Of Node)

			Public Function initialValue() As Node
				Return New Node
			End Function
		End Class

		''' <summary>
		''' Per-thread state
		''' </summary>
		Private ReadOnly participant As Participant

		''' <summary>
		''' Elimination array; null until enabled (within slotExchange).
		''' Element accesses use emulation of volatile gets and CAS.
		''' </summary>
'JAVA TO VB CONVERTER TODO TASK: There is no VB equivalent to 'volatile':
		Private arena As Node()

		''' <summary>
		''' Slot used until contention detected.
		''' </summary>
'JAVA TO VB CONVERTER TODO TASK: There is no VB equivalent to 'volatile':
		Private slot As Node

		''' <summary>
		''' The index of the largest valid arena position, OR'ed with SEQ
		''' number in high bits, incremented on each update.  The initial
		''' update from 0 to SEQ is used to ensure that the arena array is
		''' constructed only once.
		''' </summary>
'JAVA TO VB CONVERTER TODO TASK: There is no VB equivalent to 'volatile':
		Private bound As Integer

		''' <summary>
		''' Exchange function when arenas enabled. See above for explanation.
		''' </summary>
		''' <param name="item"> the (non-null) item to exchange </param>
		''' <param name="timed"> true if the wait is timed </param>
		''' <param name="ns"> if timed, the maximum wait time, else 0L </param>
		''' <returns> the other thread's item; or null if interrupted; or
		''' TIMED_OUT if timed and timed out </returns>
		Private Function arenaExchange(ByVal item As Object, ByVal timed As Boolean, ByVal ns As Long) As Object
			Dim a As Node() = arena
			Dim p As Node = participant.get()
			Dim i As Integer = p.index
			Do ' access slot at i
				Dim b, m, c As Integer ' j is raw array offset
				Dim j As Long
'JAVA TO VB CONVERTER TODO TASK: Assignments within expressions are not supported in VB
				Dim q As Node = CType(U.getObjectVolatile(a, j = (i << ASHIFT) + ABASE), Node)
				If q IsNot Nothing AndAlso U.compareAndSwapObject(a, j, q, Nothing) Then
					Dim v As Object = q.item ' release
					q.match = item
					Dim w As Thread = q.parked
					If w IsNot Nothing Then U.unpark(w)
					Return v
				Else
'JAVA TO VB CONVERTER TODO TASK: Assignments within expressions are not supported in VB
					m = (b = bound) And MMASK
					If i <= m AndAlso q Is Nothing Then
						p.item = item ' offer
						If U.compareAndSwapObject(a, j, Nothing, p) Then
							Dim [end] As Long = If(timed AndAlso m = 0, System.nanoTime() + ns, 0L)
							Dim t As Thread = Thread.CurrentThread ' wait
							Dim h As Integer = p.hash
							Dim spins_Renamed As Integer = SPINS
							Do
								Dim v As Object = p.match
								If v IsNot Nothing Then
									U.putOrderedObject(p, MATCH, Nothing)
									p.item = Nothing ' clear for next use
									p.hash = h
									Return v
								ElseIf spins_Renamed > 0 Then
									h = h Xor h << 1 ' xorshift
									h = h Xor CInt(CUInt(h) >> 3)
									h = h Xor h << 10
									If h = 0 Then ' initialize hash
										h = SPINS Or CInt(t.id)
									Else
										spins_Renamed -= 1
										If h < 0 AndAlso (spins_Renamed And ((CInt(CUInt(SPINS) >> 1)) - 1)) = 0 Then ' approx 50% true Thread.yield() ' two yields per wait
										End If
								ElseIf U.getObjectVolatile(a, j) IsNot p Then
									spins_Renamed = SPINS ' releaser hasn't set match yet
								Else
									ns = [end] - System.nanoTime()
									If (Not t.interrupted) AndAlso m = 0 AndAlso ((Not timed) OrElse ns > 0L) Then
										U.putObject(t, BLOCKER, Me) ' emulate LockSupport
										p.parked = t ' minimize window
										If U.getObjectVolatile(a, j) Is p Then U.park(False, ns)
										p.parked = Nothing
										U.putObject(t, BLOCKER, Nothing)
									ElseIf U.getObjectVolatile(a, j) Is p AndAlso U.compareAndSwapObject(a, j, p, Nothing) Then
										If m <> 0 Then ' try to shrink U.compareAndSwapInt(Me, BOUND, b, b + SEQ - 1)
										p.item = Nothing
										p.hash = h
										i = p.index >>>= 1 ' descend
										If Thread.interrupted() Then Return Nothing
										If timed AndAlso m = 0 AndAlso ns <= 0L Then Return TIMED_OUT
										Exit Do ' expired; restart
									End If
									End If
							Loop
						Else
							p.item = Nothing ' clear offer
						End If
					Else
						If p.bound <> b Then ' stale; reset
					End If
						p.bound = b
						p.collides = 0
						i = If(i <> m OrElse m = 0, m, m - 1)
					Else
						c = p.collides
						If c < m OrElse m = FULL OrElse (Not U.compareAndSwapInt(Me, BOUND, b, b + SEQ + 1)) Then
							p.collides = c + 1
							i = If(i = 0, m, i - 1) ' cyclically traverse
						Else
							i = m + 1 ' grow
						End If
						End If
					p.index = i
					End If
			Loop
		End Function

		''' <summary>
		''' Exchange function used until arenas enabled. See above for explanation.
		''' </summary>
		''' <param name="item"> the item to exchange </param>
		''' <param name="timed"> true if the wait is timed </param>
		''' <param name="ns"> if timed, the maximum wait time, else 0L </param>
		''' <returns> the other thread's item; or null if either the arena
		''' was enabled or the thread was interrupted before completion; or
		''' TIMED_OUT if timed and timed out </returns>
		Private Function slotExchange(ByVal item As Object, ByVal timed As Boolean, ByVal ns As Long) As Object
			Dim p As Node = participant.get()
			Dim t As Thread = Thread.CurrentThread
			If t.interrupted Then ' preserve interrupt status so caller can recheck Return Nothing

			Dim q As Node
			Do
				q = slot
				If q IsNot Nothing Then
					If U.compareAndSwapObject(Me, SLOT, q, Nothing) Then
						Dim v As Object = q.item
						q.match = item
						Dim w As Thread = q.parked
						If w IsNot Nothing Then U.unpark(w)
						Return v
					End If
					' create arena on contention, but continue until slot null
					If NCPU > 1 AndAlso bound = 0 AndAlso U.compareAndSwapInt(Me, BOUND, 0, SEQ) Then arena = New Node((FULL + 2) << ASHIFT - 1){}
				ElseIf arena IsNot Nothing Then
					Return Nothing ' caller must reroute to arenaExchange
				Else
					p.item = item
					If U.compareAndSwapObject(Me, SLOT, Nothing, p) Then Exit Do
					p.item = Nothing
				End If
			Loop

			' await release
			Dim h As Integer = p.hash
			Dim [end] As Long = If(timed, System.nanoTime() + ns, 0L)
			Dim spins_Renamed As Integer = If(NCPU > 1, SPINS, 1)
			Dim v As Object
			v = p.match
			Do While v Is Nothing
				If spins_Renamed > 0 Then
					h = h Xor h << 1
					h = h Xor CInt(CUInt(h) >> 3)
					h = h Xor h << 10
					If h = 0 Then
						h = SPINS Or CInt(t.id)
					Else
						spins_Renamed -= 1
						If h < 0 AndAlso (spins_Renamed And ((CInt(CUInt(SPINS) >> 1)) - 1)) = 0 Then Thread.yield()
						End If
				ElseIf slot IsNot p Then
					spins_Renamed = SPINS
				Else
					ns = [end] - System.nanoTime()
					If (Not t.interrupted) AndAlso arena Is Nothing AndAlso ((Not timed) OrElse ns > 0L) Then
						U.putObject(t, BLOCKER, Me)
						p.parked = t
						If slot Is p Then U.park(False, ns)
						p.parked = Nothing
						U.putObject(t, BLOCKER, Nothing)
					ElseIf U.compareAndSwapObject(Me, SLOT, p, Nothing) Then
						v = If(timed AndAlso ns <= 0L AndAlso (Not t.interrupted), TIMED_OUT, Nothing)
						Exit Do
					End If
					End If
				v = p.match
			Loop
			U.putOrderedObject(p, MATCH, Nothing)
			p.item = Nothing
			p.hash = h
			Return v
		End Function

		''' <summary>
		''' Creates a new Exchanger.
		''' </summary>
		Public Sub New()
			participant = New Participant
		End Sub

		''' <summary>
		''' Waits for another thread to arrive at this exchange point (unless
		''' the current thread is <seealso cref="Thread#interrupt interrupted"/>),
		''' and then transfers the given object to it, receiving its object
		''' in return.
		''' 
		''' <p>If another thread is already waiting at the exchange point then
		''' it is resumed for thread scheduling purposes and receives the object
		''' passed in by the current thread.  The current thread returns immediately,
		''' receiving the object passed to the exchange by that other thread.
		''' 
		''' <p>If no other thread is already waiting at the exchange then the
		''' current thread is disabled for thread scheduling purposes and lies
		''' dormant until one of two things happens:
		''' <ul>
		''' <li>Some other thread enters the exchange; or
		''' <li>Some other thread <seealso cref="Thread#interrupt interrupts"/>
		''' the current thread.
		''' </ul>
		''' <p>If the current thread:
		''' <ul>
		''' <li>has its interrupted status set on entry to this method; or
		''' <li>is <seealso cref="Thread#interrupt interrupted"/> while waiting
		''' for the exchange,
		''' </ul>
		''' then <seealso cref="InterruptedException"/> is thrown and the current thread's
		''' interrupted status is cleared.
		''' </summary>
		''' <param name="x"> the object to exchange </param>
		''' <returns> the object provided by the other thread </returns>
		''' <exception cref="InterruptedException"> if the current thread was
		'''         interrupted while waiting </exception>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
		Public Overridable Function exchange(ByVal x As V) As V
			Dim v As Object
			Dim item As Object = If(x Is Nothing, NULL_ITEM, x) ' translate null args
			v = slotExchange(item, False, 0L)
			v = arenaExchange(item, False, 0L)
			If (arena IsNot Nothing OrElse v Is Nothing) AndAlso ((Thread.interrupted() OrElse v Is Nothing)) Then ' disambiguates null return Throw New InterruptedException
			Return If(v Is NULL_ITEM, Nothing, CType(v, V))
		End Function

		''' <summary>
		''' Waits for another thread to arrive at this exchange point (unless
		''' the current thread is <seealso cref="Thread#interrupt interrupted"/> or
		''' the specified waiting time elapses), and then transfers the given
		''' object to it, receiving its object in return.
		''' 
		''' <p>If another thread is already waiting at the exchange point then
		''' it is resumed for thread scheduling purposes and receives the object
		''' passed in by the current thread.  The current thread returns immediately,
		''' receiving the object passed to the exchange by that other thread.
		''' 
		''' <p>If no other thread is already waiting at the exchange then the
		''' current thread is disabled for thread scheduling purposes and lies
		''' dormant until one of three things happens:
		''' <ul>
		''' <li>Some other thread enters the exchange; or
		''' <li>Some other thread <seealso cref="Thread#interrupt interrupts"/>
		''' the current thread; or
		''' <li>The specified waiting time elapses.
		''' </ul>
		''' <p>If the current thread:
		''' <ul>
		''' <li>has its interrupted status set on entry to this method; or
		''' <li>is <seealso cref="Thread#interrupt interrupted"/> while waiting
		''' for the exchange,
		''' </ul>
		''' then <seealso cref="InterruptedException"/> is thrown and the current thread's
		''' interrupted status is cleared.
		''' 
		''' <p>If the specified waiting time elapses then {@link
		''' TimeoutException} is thrown.  If the time is less than or equal
		''' to zero, the method will not wait at all.
		''' </summary>
		''' <param name="x"> the object to exchange </param>
		''' <param name="timeout"> the maximum time to wait </param>
		''' <param name="unit"> the time unit of the {@code timeout} argument </param>
		''' <returns> the object provided by the other thread </returns>
		''' <exception cref="InterruptedException"> if the current thread was
		'''         interrupted while waiting </exception>
		''' <exception cref="TimeoutException"> if the specified waiting time elapses
		'''         before another thread enters the exchange </exception>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
		Public Overridable Function exchange(ByVal x As V, ByVal timeout As Long, ByVal unit As TimeUnit) As V
			Dim v As Object
			Dim item As Object = If(x Is Nothing, NULL_ITEM, x)
			Dim ns As Long = unit.toNanos(timeout)
			v = slotExchange(item, True, ns)
			v = arenaExchange(item, True, ns)
			If (arena IsNot Nothing OrElse v Is Nothing) AndAlso ((Thread.interrupted() OrElse v Is Nothing)) Then Throw New InterruptedException
			If v Is TIMED_OUT Then Throw New TimeoutException
			Return If(v Is NULL_ITEM, Nothing, CType(v, V))
		End Function

		' Unsafe mechanics
		Private Shared ReadOnly U As sun.misc.Unsafe
		Private Shared ReadOnly BOUND As Long
		Private Shared ReadOnly SLOT As Long
		Private Shared ReadOnly MATCH As Long
		Private Shared ReadOnly BLOCKER As Long
		Private Shared ReadOnly ABASE As Integer
		Shared Sub New()
			Dim s As Integer
			Try
				U = sun.misc.Unsafe.unsafe
				Dim ek As  [Class] = GetType(Exchanger)
				Dim nk As  [Class] = GetType(Node)
				Dim ak As  [Class] = GetType(Node())
				Dim tk As  [Class] = GetType(Thread)
				BOUND = U.objectFieldOffset(ek.getDeclaredField("bound"))
				SLOT = U.objectFieldOffset(ek.getDeclaredField("slot"))
				MATCH = U.objectFieldOffset(nk.getDeclaredField("match"))
				BLOCKER = U.objectFieldOffset(tk.getDeclaredField("parkBlocker"))
				s = U.arrayIndexScale(ak)
				' ABASE absorbs padding in front of element 0
				ABASE = U.arrayBaseOffset(ak) + (1 << ASHIFT)

			Catch e As Exception
				Throw New [Error](e)
			End Try
			If (s And (s-1)) <> 0 OrElse s > (1 << ASHIFT) Then Throw New [Error]("Unsupported array scale")
		End Sub

	End Class

End Namespace