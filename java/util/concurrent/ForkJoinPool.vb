Imports Microsoft.VisualBasic
Imports System
Imports System.Runtime.CompilerServices
Imports System.Collections.Generic
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
	''' An <seealso cref="ExecutorService"/> for running <seealso cref="ForkJoinTask"/>s.
	''' A {@code ForkJoinPool} provides the entry point for submissions
	''' from non-{@code ForkJoinTask} clients, as well as management and
	''' monitoring operations.
	''' 
	''' <p>A {@code ForkJoinPool} differs from other kinds of {@link
	''' ExecutorService} mainly by virtue of employing
	''' <em>work-stealing</em>: all threads in the pool attempt to find and
	''' execute tasks submitted to the pool and/or created by other active
	''' tasks (eventually blocking waiting for work if none exist). This
	''' enables efficient processing when most tasks spawn other subtasks
	''' (as do most {@code ForkJoinTask}s), as well as when many small
	''' tasks are submitted to the pool from external clients.  Especially
	''' when setting <em>asyncMode</em> to true in constructors, {@code
	''' ForkJoinPool}s may also be appropriate for use with event-style
	''' tasks that are never joined.
	''' 
	''' <p>A static <seealso cref="#commonPool()"/> is available and appropriate for
	''' most applications. The common pool is used by any ForkJoinTask that
	''' is not explicitly submitted to a specified pool. Using the common
	''' pool normally reduces resource usage (its threads are slowly
	''' reclaimed during periods of non-use, and reinstated upon subsequent
	''' use).
	''' 
	''' <p>For applications that require separate or custom pools, a {@code
	''' ForkJoinPool} may be constructed with a given target parallelism
	''' level; by default, equal to the number of available processors.
	''' The pool attempts to maintain enough active (or available) threads
	''' by dynamically adding, suspending, or resuming internal worker
	''' threads, even if some tasks are stalled waiting to join others.
	''' However, no such adjustments are guaranteed in the face of blocked
	''' I/O or other unmanaged synchronization. The nested {@link
	''' ManagedBlocker} interface enables extension of the kinds of
	''' synchronization accommodated.
	''' 
	''' <p>In addition to execution and lifecycle control methods, this
	''' class provides status check methods (for example
	''' <seealso cref="#getStealCount"/>) that are intended to aid in developing,
	''' tuning, and monitoring fork/join applications. Also, method
	''' <seealso cref="#toString"/> returns indications of pool state in a
	''' convenient form for informal monitoring.
	''' 
	''' <p>As is the case with other ExecutorServices, there are three
	''' main task execution methods summarized in the following table.
	''' These are designed to be used primarily by clients not already
	''' engaged in fork/join computations in the current pool.  The main
	''' forms of these methods accept instances of {@code ForkJoinTask},
	''' but overloaded forms also allow mixed execution of plain {@code
	''' Runnable}- or {@code Callable}- based activities as well.  However,
	''' tasks that are already executing in a pool should normally instead
	''' use the within-computation forms listed in the table unless using
	''' async event-style tasks that are not usually joined, in which case
	''' there is little difference among choice of methods.
	''' 
	''' <table BORDER CELLPADDING=3 CELLSPACING=1>
	''' <caption>Summary of task execution methods</caption>
	'''  <tr>
	'''    <td></td>
	'''    <td ALIGN=CENTER> <b>Call from non-fork/join clients</b></td>
	'''    <td ALIGN=CENTER> <b>Call from within fork/join computations</b></td>
	'''  </tr>
	'''  <tr>
	'''    <td> <b>Arrange async execution</b></td>
	'''    <td> <seealso cref="#execute(ForkJoinTask)"/></td>
	'''    <td> <seealso cref="ForkJoinTask#fork"/></td>
	'''  </tr>
	'''  <tr>
	'''    <td> <b>Await and obtain result</b></td>
	'''    <td> <seealso cref="#invoke(ForkJoinTask)"/></td>
	'''    <td> <seealso cref="ForkJoinTask#invoke"/></td>
	'''  </tr>
	'''  <tr>
	'''    <td> <b>Arrange exec and obtain Future</b></td>
	'''    <td> <seealso cref="#submit(ForkJoinTask)"/></td>
	'''    <td> <seealso cref="ForkJoinTask#fork"/> (ForkJoinTasks <em>are</em> Futures)</td>
	'''  </tr>
	''' </table>
	''' 
	''' <p>The common pool is by default constructed with default
	''' parameters, but these may be controlled by setting three
	''' <seealso cref="System#getProperty system properties"/>:
	''' <ul>
	''' <li>{@code java.util.concurrent.ForkJoinPool.common.parallelism}
	''' - the parallelism level, a non-negative integer
	''' <li>{@code java.util.concurrent.ForkJoinPool.common.threadFactory}
	''' - the class name of a <seealso cref="ForkJoinWorkerThreadFactory"/>
	''' <li>{@code java.util.concurrent.ForkJoinPool.common.exceptionHandler}
	''' - the class name of a <seealso cref="UncaughtExceptionHandler"/>
	''' </ul>
	''' If a <seealso cref="SecurityManager"/> is present and no factory is
	''' specified, then the default pool uses a factory supplying
	''' threads that have no <seealso cref="Permissions"/> enabled.
	''' The system class loader is used to load these classes.
	''' Upon any error in establishing these settings, default parameters
	''' are used. It is possible to disable or limit the use of threads in
	''' the common pool by setting the parallelism property to zero, and/or
	''' using a factory that may return {@code null}. However doing so may
	''' cause unjoined tasks to never be executed.
	''' 
	''' <p><b>Implementation notes</b>: This implementation restricts the
	''' maximum number of running threads to 32767. Attempts to create
	''' pools with greater than the maximum number result in
	''' {@code IllegalArgumentException}.
	''' 
	''' <p>This implementation rejects submitted tasks (that is, by throwing
	''' <seealso cref="RejectedExecutionException"/>) only when the pool is shut down
	''' or internal resources have been exhausted.
	''' 
	''' @since 1.7
	''' @author Doug Lea
	''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
	Public Class ForkJoinPool
		Inherits java.util.concurrent.AbstractExecutorService

	'    
	'     * Implementation Overview
	'     *
	'     * This class and its nested classes provide the main
	'     * functionality and control for a set of worker threads:
	'     * Submissions from non-FJ threads enter into submission queues.
	'     * Workers take these tasks and typically split them into subtasks
	'     * that may be stolen by other workers.  Preference rules give
	'     * first priority to processing tasks from their own queues (LIFO
	'     * or FIFO, depending on mode), then to randomized FIFO steals of
	'     * tasks in other queues.  This framework began as vehicle for
	'     * supporting tree-structured parallelism using work-stealing.
	'     * Over time, its scalability advantages led to extensions and
	'     * changes to better support more diverse usage contexts.  Because
	'     * most internal methods and nested classes are interrelated,
	'     * their main rationale and descriptions are presented here;
	'     * individual methods and nested classes contain only brief
	'     * comments about details.
	'     *
	'     * WorkQueues
	'     * ==========
	'     *
	'     * Most operations occur within work-stealing queues (in nested
	'     * class WorkQueue).  These are special forms of Deques that
	'     * support only three of the four possible end-operations -- push,
	'     * pop, and poll (aka steal), under the further constraints that
	'     * push and pop are called only from the owning thread (or, as
	'     * extended here, under a lock), while poll may be called from
	'     * other threads.  (If you are unfamiliar with them, you probably
	'     * want to read Herlihy and Shavit's book "The Art of
	'     * Multiprocessor programming", chapter 16 describing these in
	'     * more detail before proceeding.)  The main work-stealing queue
	'     * design is roughly similar to those in the papers "Dynamic
	'     * Circular Work-Stealing Deque" by Chase and Lev, SPAA 2005
	'     * (http://research.sun.com/scalable/pubs/index.html) and
	'     * "Idempotent work stealing" by Michael, Saraswat, and Vechev,
	'     * PPoPP 2009 (http://portal.acm.org/citation.cfm?id=1504186).
	'     * The main differences ultimately stem from GC requirements that
	'     * we null out taken slots as soon as we can, to maintain as small
	'     * a footprint as possible even in programs generating huge
	'     * numbers of tasks. To accomplish this, we shift the CAS
	'     * arbitrating pop vs poll (steal) from being on the indices
	'     * ("base" and "top") to the slots themselves.
	'     *
	'     * Adding tasks then takes the form of a classic array push(task):
	'     *    q.array[q.top] = task; ++q.top;
	'     *
	'     * (The actual code needs to null-check and size-check the array,
	'     * properly fence the accesses, and possibly signal waiting
	'     * workers to start scanning -- see below.)  Both a successful pop
	'     * and poll mainly entail a CAS of a slot from non-null to null.
	'     *
	'     * The pop operation (always performed by owner) is:
	'     *   if ((base != top) and
	'     *        (the task at top slot is not null) and
	'     *        (CAS slot to null))
	'     *           decrement top and return task;
	'     *
	'     * And the poll operation (usually by a stealer) is
	'     *    if ((base != top) and
	'     *        (the task at base slot is not null) and
	'     *        (base has not changed) and
	'     *        (CAS slot to null))
	'     *           increment base and return task;
	'     *
	'     * Because we rely on CASes of references, we do not need tag bits
	'     * on base or top.  They are simple ints as used in any circular
	'     * array-based queue (see for example ArrayDeque).  Updates to the
	'     * indices guarantee that top == base means the queue is empty,
	'     * but otherwise may err on the side of possibly making the queue
	'     * appear nonempty when a push, pop, or poll have not fully
	'     * committed. (Method isEmpty() checks the case of a partially
	'     * completed removal of the last element.)  Because of this, the
	'     * poll operation, considered individually, is not wait-free. One
	'     * thief cannot successfully continue until another in-progress
	'     * one (or, if previously empty, a push) completes.  However, in
	'     * the aggregate, we ensure at least probabilistic
	'     * non-blockingness.  If an attempted steal fails, a thief always
	'     * chooses a different random victim target to try next. So, in
	'     * order for one thief to progress, it suffices for any
	'     * in-progress poll or new push on any empty queue to
	'     * complete. (This is why we normally use method pollAt and its
	'     * variants that try once at the apparent base index, else
	'     * consider alternative actions, rather than method poll, which
	'     * retries.)
	'     *
	'     * This approach also enables support of a user mode in which
	'     * local task processing is in FIFO, not LIFO order, simply by
	'     * using poll rather than pop.  This can be useful in
	'     * message-passing frameworks in which tasks are never joined.
	'     * However neither mode considers affinities, loads, cache
	'     * localities, etc, so rarely provide the best possible
	'     * performance on a given machine, but portably provide good
	'     * throughput by averaging over these factors.  Further, even if
	'     * we did try to use such information, we do not usually have a
	'     * basis for exploiting it.  For example, some sets of tasks
	'     * profit from cache affinities, but others are harmed by cache
	'     * pollution effects. Additionally, even though it requires
	'     * scanning, long-term throughput is often best using random
	'     * selection rather than directed selection policies, so cheap
	'     * randomization of sufficient quality is used whenever
	'     * applicable.  Various Marsaglia XorShifts (some with different
	'     * shift constants) are inlined at use points.
	'     *
	'     * WorkQueues are also used in a similar way for tasks submitted
	'     * to the pool. We cannot mix these tasks in the same queues used
	'     * by workers. Instead, we randomly associate submission queues
	'     * with submitting threads, using a form of hashing.  The
	'     * ThreadLocalRandom probe value serves as a hash code for
	'     * choosing existing queues, and may be randomly repositioned upon
	'     * contention with other submitters.  In essence, submitters act
	'     * like workers except that they are restricted to executing local
	'     * tasks that they submitted (or in the case of CountedCompleters,
	'     * others with the same root task).  Insertion of tasks in shared
	'     * mode requires a lock (mainly to protect in the case of
	'     * resizing) but we use only a simple spinlock (using field
	'     * qlock), because submitters encountering a busy queue move on to
	'     * try or create other queues -- they block only when creating and
	'     * registering new queues. Additionally, "qlock" saturates to an
	'     * unlockable value (-1) at shutdown. Unlocking still can be and
	'     * is performed by cheaper ordered writes of "qlock" in successful
	'     * cases, but uses CAS in unsuccessful cases.
	'     *
	'     * Management
	'     * ==========
	'     *
	'     * The main throughput advantages of work-stealing stem from
	'     * decentralized control -- workers mostly take tasks from
	'     * themselves or each other, at rates that can exceed a billion
	'     * per second.  The pool itself creates, activates (enables
	'     * scanning for and running tasks), deactivates, blocks, and
	'     * terminates threads, all with minimal central information.
	'     * There are only a few properties that we can globally track or
	'     * maintain, so we pack them into a small number of variables,
	'     * often maintaining atomicity without blocking or locking.
	'     * Nearly all essentially atomic control state is held in two
	'     * volatile variables that are by far most often read (not
	'     * written) as status and consistency checks. (Also, field
	'     * "config" holds unchanging configuration state.)
	'     *
	'     * Field "ctl" contains 64 bits holding information needed to
	'     * atomically decide to add, inactivate, enqueue (on an event
	'     * queue), dequeue, and/or re-activate workers.  To enable this
	'     * packing, we restrict maximum parallelism to (1<<15)-1 (which is
	'     * far in excess of normal operating range) to allow ids, counts,
	'     * and their negations (used for thresholding) to fit into 16bit
	'     * subfields.
	'     *
	'     * Field "runState" holds lockable state bits (STARTED, STOP, etc)
	'     * also protecting updates to the workQueues array.  When used as
	'     * a lock, it is normally held only for a few instructions (the
	'     * only exceptions are one-time array initialization and uncommon
	'     * resizing), so is nearly always available after at most a brief
	'     * spin. But to be extra-cautious, after spinning, method
	'     * awaitRunStateLock (called only if an initial CAS fails), uses a
	'     * wait/notify mechanics on a builtin monitor to block when
	'     * (rarely) needed. This would be a terrible idea for a highly
	'     * contended lock, but most pools run without the lock ever
	'     * contending after the spin limit, so this works fine as a more
	'     * conservative alternative. Because we don't otherwise have an
	'     * internal Object to use as a monitor, the "stealCounter" (an
	'     * AtomicLong) is used when available (it too must be lazily
	'     * initialized; see externalSubmit).
	'     *
	'     * Usages of "runState" vs "ctl" interact in only one case:
	'     * deciding to add a worker thread (see tryAddWorker), in which
	'     * case the ctl CAS is performed while the lock is held.
	'     *
	'     * Recording WorkQueues.  WorkQueues are recorded in the
	'     * "workQueues" array. The array is created upon first use (see
	'     * externalSubmit) and expanded if necessary.  Updates to the
	'     * array while recording new workers and unrecording terminated
	'     * ones are protected from each other by the runState lock, but
	'     * the array is otherwise concurrently readable, and accessed
	'     * directly. We also ensure that reads of the array reference
	'     * itself never become too stale. To simplify index-based
	'     * operations, the array size is always a power of two, and all
	'     * readers must tolerate null slots. Worker queues are at odd
	'     * indices. Shared (submission) queues are at even indices, up to
	'     * a maximum of 64 slots, to limit growth even if array needs to
	'     * expand to add more workers. Grouping them together in this way
	'     * simplifies and speeds up task scanning.
	'     *
	'     * All worker thread creation is on-demand, triggered by task
	'     * submissions, replacement of terminated workers, and/or
	'     * compensation for blocked workers. However, all other support
	'     * code is set up to work with other policies.  To ensure that we
	'     * do not hold on to worker references that would prevent GC, All
	'     * accesses to workQueues are via indices into the workQueues
	'     * array (which is one source of some of the messy code
	'     * constructions here). In essence, the workQueues array serves as
	'     * a weak reference mechanism. Thus for example the stack top
	'     * subfield of ctl stores indices, not references.
	'     *
	'     * Queuing Idle Workers. Unlike HPC work-stealing frameworks, we
	'     * cannot let workers spin indefinitely scanning for tasks when
	'     * none can be found immediately, and we cannot start/resume
	'     * workers unless there appear to be tasks available.  On the
	'     * other hand, we must quickly prod them into action when new
	'     * tasks are submitted or generated. In many usages, ramp-up time
	'     * to activate workers is the main limiting factor in overall
	'     * performance, which is compounded at program start-up by JIT
	'     * compilation and allocation. So we streamline this as much as
	'     * possible.
	'     *
	'     * The "ctl" field atomically maintains active and total worker
	'     * counts as well as a queue to place waiting threads so they can
	'     * be located for signalling. Active counts also play the role of
	'     * quiescence indicators, so are decremented when workers believe
	'     * that there are no more tasks to execute. The "queue" is
	'     * actually a form of Treiber stack.  A stack is ideal for
	'     * activating threads in most-recently used order. This improves
	'     * performance and locality, outweighing the disadvantages of
	'     * being prone to contention and inability to release a worker
	'     * unless it is topmost on stack.  We park/unpark workers after
	'     * pushing on the idle worker stack (represented by the lower
	'     * 32bit subfield of ctl) when they cannot find work.  The top
	'     * stack state holds the value of the "scanState" field of the
	'     * worker: its index and status, plus a version counter that, in
	'     * addition to the count subfields (also serving as version
	'     * stamps) provide protection against Treiber stack ABA effects.
	'     *
	'     * Field scanState is used by both workers and the pool to manage
	'     * and track whether a worker is INACTIVE (possibly blocked
	'     * waiting for a signal), or SCANNING for tasks (when neither hold
	'     * it is busy running tasks).  When a worker is inactivated, its
	'     * scanState field is set, and is prevented from executing tasks,
	'     * even though it must scan once for them to avoid queuing
	'     * races. Note that scanState updates lag queue CAS releases so
	'     * usage requires care. When queued, the lower 16 bits of
	'     * scanState must hold its pool index. So we place the index there
	'     * upon initialization (see registerWorker) and otherwise keep it
	'     * there or restore it when necessary.
	'     *
	'     * Memory ordering.  See "Correct and Efficient Work-Stealing for
	'     * Weak Memory Models" by Le, Pop, Cohen, and Nardelli, PPoPP 2013
	'     * (http://www.di.ens.fr/~zappa/readings/ppopp13.pdf) for an
	'     * analysis of memory ordering requirements in work-stealing
	'     * algorithms similar to the one used here.  We usually need
	'     * stronger than minimal ordering because we must sometimes signal
	'     * workers, requiring Dekker-like full-fences to avoid lost
	'     * signals.  Arranging for enough ordering without expensive
	'     * over-fencing requires tradeoffs among the supported means of
	'     * expressing access constraints. The most central operations,
	'     * taking from queues and updating ctl state, require full-fence
	'     * CAS.  Array slots are read using the emulation of volatiles
	'     * provided by Unsafe.  Access from other threads to WorkQueue
	'     * base, top, and array requires a volatile load of the first of
	'     * any of these read.  We use the convention of declaring the
	'     * "base" index volatile, and always read it before other fields.
	'     * The owner thread must ensure ordered updates, so writes use
	'     * ordered intrinsics unless they can piggyback on those for other
	'     * writes.  Similar conventions and rationales hold for other
	'     * WorkQueue fields (such as "currentSteal") that are only written
	'     * by owners but observed by others.
	'     *
	'     * Creating workers. To create a worker, we pre-increment total
	'     * count (serving as a reservation), and attempt to construct a
	'     * ForkJoinWorkerThread via its factory. Upon construction, the
	'     * new thread invokes registerWorker, where it constructs a
	'     * WorkQueue and is assigned an index in the workQueues array
	'     * (expanding the array if necessary). The thread is then
	'     * started. Upon any exception across these steps, or null return
	'     * from factory, deregisterWorker adjusts counts and records
	'     * accordingly.  If a null return, the pool continues running with
	'     * fewer than the target number workers. If exceptional, the
	'     * exception is propagated, generally to some external caller.
	'     * Worker index assignment avoids the bias in scanning that would
	'     * occur if entries were sequentially packed starting at the front
	'     * of the workQueues array. We treat the array as a simple
	'     * power-of-two hash table, expanding as needed. The seedIndex
	'     * increment ensures no collisions until a resize is needed or a
	'     * worker is deregistered and replaced, and thereafter keeps
	'     * probability of collision low. We cannot use
	'     * ThreadLocalRandom.getProbe() for similar purposes here because
	'     * the thread has not started yet, but do so for creating
	'     * submission queues for existing external threads.
	'     *
	'     * Deactivation and waiting. Queuing encounters several intrinsic
	'     * races; most notably that a task-producing thread can miss
	'     * seeing (and signalling) another thread that gave up looking for
	'     * work but has not yet entered the wait queue.  When a worker
	'     * cannot find a task to steal, it deactivates and enqueues. Very
	'     * often, the lack of tasks is transient due to GC or OS
	'     * scheduling. To reduce false-alarm deactivation, scanners
	'     * compute checksums of queue states during sweeps.  (The
	'     * stability checks used here and elsewhere are probabilistic
	'     * variants of snapshot techniques -- see Herlihy & Shavit.)
	'     * Workers give up and try to deactivate only after the sum is
	'     * stable across scans. Further, to avoid missed signals, they
	'     * repeat this scanning process after successful enqueuing until
	'     * again stable.  In this state, the worker cannot take/run a task
	'     * it sees until it is released from the queue, so the worker
	'     * itself eventually tries to release itself or any successor (see
	'     * tryRelease).  Otherwise, upon an empty scan, a deactivated
	'     * worker uses an adaptive local spin construction (see awaitWork)
	'     * before blocking (via park). Note the unusual conventions about
	'     * Thread.interrupts surrounding parking and other blocking:
	'     * Because interrupts are used solely to alert threads to check
	'     * termination, which is checked anyway upon blocking, we clear
	'     * status (using Thread.interrupted) before any call to park, so
	'     * that park does not immediately return due to status being set
	'     * via some other unrelated call to interrupt in user code.
	'     *
	'     * Signalling and activation.  Workers are created or activated
	'     * only when there appears to be at least one task they might be
	'     * able to find and execute.  Upon push (either by a worker or an
	'     * external submission) to a previously (possibly) empty queue,
	'     * workers are signalled if idle, or created if fewer exist than
	'     * the given parallelism level.  These primary signals are
	'     * buttressed by others whenever other threads remove a task from
	'     * a queue and notice that there are other tasks there as well.
	'     * On most platforms, signalling (unpark) overhead time is
	'     * noticeably long, and the time between signalling a thread and
	'     * it actually making progress can be very noticeably long, so it
	'     * is worth offloading these delays from critical paths as much as
	'     * possible. Also, because inactive workers are often rescanning
	'     * or spinning rather than blocking, we set and clear the "parker"
	'     * field of WorkQueues to reduce unnecessary calls to unpark.
	'     * (This requires a secondary recheck to avoid missed signals.)
	'     *
	'     * Trimming workers. To release resources after periods of lack of
	'     * use, a worker starting to wait when the pool is quiescent will
	'     * time out and terminate (see awaitWork) if the pool has remained
	'     * quiescent for period IDLE_TIMEOUT, increasing the period as the
	'     * number of threads decreases, eventually removing all workers.
	'     * Also, when more than two spare threads exist, excess threads
	'     * are immediately terminated at the next quiescent point.
	'     * (Padding by two avoids hysteresis.)
	'     *
	'     * Shutdown and Termination. A call to shutdownNow invokes
	'     * tryTerminate to atomically set a runState bit. The calling
	'     * thread, as well as every other worker thereafter terminating,
	'     * helps terminate others by setting their (qlock) status,
	'     * cancelling their unprocessed tasks, and waking them up, doing
	'     * so repeatedly until stable (but with a loop bounded by the
	'     * number of workers).  Calls to non-abrupt shutdown() preface
	'     * this by checking whether termination should commence. This
	'     * relies primarily on the active count bits of "ctl" maintaining
	'     * consensus -- tryTerminate is called from awaitWork whenever
	'     * quiescent. However, external submitters do not take part in
	'     * this consensus.  So, tryTerminate sweeps through queues (until
	'     * stable) to ensure lack of in-flight submissions and workers
	'     * about to process them before triggering the "STOP" phase of
	'     * termination. (Note: there is an intrinsic conflict if
	'     * helpQuiescePool is called when shutdown is enabled. Both wait
	'     * for quiescence, but tryTerminate is biased to not trigger until
	'     * helpQuiescePool completes.)
	'     *
	'     *
	'     * Joining Tasks
	'     * =============
	'     *
	'     * Any of several actions may be taken when one worker is waiting
	'     * to join a task stolen (or always held) by another.  Because we
	'     * are multiplexing many tasks on to a pool of workers, we can't
	'     * just let them block (as in Thread.join).  We also cannot just
	'     * reassign the joiner's run-time stack with another and replace
	'     * it later, which would be a form of "continuation", that even if
	'     * possible is not necessarily a good idea since we may need both
	'     * an unblocked task and its continuation to progress.  Instead we
	'     * combine two tactics:
	'     *
	'     *   Helping: Arranging for the joiner to execute some task that it
	'     *      would be running if the steal had not occurred.
	'     *
	'     *   Compensating: Unless there are already enough live threads,
	'     *      method tryCompensate() may create or re-activate a spare
	'     *      thread to compensate for blocked joiners until they unblock.
	'     *
	'     * A third form (implemented in tryRemoveAndExec) amounts to
	'     * helping a hypothetical compensator: If we can readily tell that
	'     * a possible action of a compensator is to steal and execute the
	'     * task being joined, the joining thread can do so directly,
	'     * without the need for a compensation thread (although at the
	'     * expense of larger run-time stacks, but the tradeoff is
	'     * typically worthwhile).
	'     *
	'     * The ManagedBlocker extension API can't use helping so relies
	'     * only on compensation in method awaitBlocker.
	'     *
	'     * The algorithm in helpStealer entails a form of "linear
	'     * helping".  Each worker records (in field currentSteal) the most
	'     * recent task it stole from some other worker (or a submission).
	'     * It also records (in field currentJoin) the task it is currently
	'     * actively joining. Method helpStealer uses these markers to try
	'     * to find a worker to help (i.e., steal back a task from and
	'     * execute it) that could hasten completion of the actively joined
	'     * task.  Thus, the joiner executes a task that would be on its
	'     * own local deque had the to-be-joined task not been stolen. This
	'     * is a conservative variant of the approach described in Wagner &
	'     * Calder "Leapfrogging: a portable technique for implementing
	'     * efficient futures" SIGPLAN Notices, 1993
	'     * (http://portal.acm.org/citation.cfm?id=155354). It differs in
	'     * that: (1) We only maintain dependency links across workers upon
	'     * steals, rather than use per-task bookkeeping.  This sometimes
	'     * requires a linear scan of workQueues array to locate stealers,
	'     * but often doesn't because stealers leave hints (that may become
	'     * stale/wrong) of where to locate them.  It is only a hint
	'     * because a worker might have had multiple steals and the hint
	'     * records only one of them (usually the most current).  Hinting
	'     * isolates cost to when it is needed, rather than adding to
	'     * per-task overhead.  (2) It is "shallow", ignoring nesting and
	'     * potentially cyclic mutual steals.  (3) It is intentionally
	'     * racy: field currentJoin is updated only while actively joining,
	'     * which means that we miss links in the chain during long-lived
	'     * tasks, GC stalls etc (which is OK since blocking in such cases
	'     * is usually a good idea).  (4) We bound the number of attempts
	'     * to find work using checksums and fall back to suspending the
	'     * worker and if necessary replacing it with another.
	'     *
	'     * Helping actions for CountedCompleters do not require tracking
	'     * currentJoins: Method helpComplete takes and executes any task
	'     * with the same root as the task being waited on (preferring
	'     * local pops to non-local polls). However, this still entails
	'     * some traversal of completer chains, so is less efficient than
	'     * using CountedCompleters without explicit joins.
	'     *
	'     * Compensation does not aim to keep exactly the target
	'     * parallelism number of unblocked threads running at any given
	'     * time. Some previous versions of this class employed immediate
	'     * compensations for any blocked join. However, in practice, the
	'     * vast majority of blockages are transient byproducts of GC and
	'     * other JVM or OS activities that are made worse by replacement.
	'     * Currently, compensation is attempted only after validating that
	'     * all purportedly active threads are processing tasks by checking
	'     * field WorkQueue.scanState, which eliminates most false
	'     * positives.  Also, compensation is bypassed (tolerating fewer
	'     * threads) in the most common case in which it is rarely
	'     * beneficial: when a worker with an empty queue (thus no
	'     * continuation tasks) blocks on a join and there still remain
	'     * enough threads to ensure liveness.
	'     *
	'     * The compensation mechanism may be bounded.  Bounds for the
	'     * commonPool (see commonMaxSpares) better enable JVMs to cope
	'     * with programming errors and abuse before running out of
	'     * resources to do so. In other cases, users may supply factories
	'     * that limit thread construction. The effects of bounding in this
	'     * pool (like all others) is imprecise.  Total worker counts are
	'     * decremented when threads deregister, not when they exit and
	'     * resources are reclaimed by the JVM and OS. So the number of
	'     * simultaneously live threads may transiently exceed bounds.
	'     *
	'     * Common Pool
	'     * ===========
	'     *
	'     * The static common pool always exists after static
	'     * initialization.  Since it (or any other created pool) need
	'     * never be used, we minimize initial construction overhead and
	'     * footprint to the setup of about a dozen fields, with no nested
	'     * allocation. Most bootstrapping occurs within method
	'     * externalSubmit during the first submission to the pool.
	'     *
	'     * When external threads submit to the common pool, they can
	'     * perform subtask processing (see externalHelpComplete and
	'     * related methods) upon joins.  This caller-helps policy makes it
	'     * sensible to set common pool parallelism level to one (or more)
	'     * less than the total number of available cores, or even zero for
	'     * pure caller-runs.  We do not need to record whether external
	'     * submissions are to the common pool -- if not, external help
	'     * methods return quickly. These submitters would otherwise be
	'     * blocked waiting for completion, so the extra effort (with
	'     * liberally sprinkled task status checks) in inapplicable cases
	'     * amounts to an odd form of limited spin-wait before blocking in
	'     * ForkJoinTask.join.
	'     *
	'     * As a more appropriate default in managed environments, unless
	'     * overridden by system properties, we use workers of subclass
	'     * InnocuousForkJoinWorkerThread when there is a SecurityManager
	'     * present. These workers have no permissions set, do not belong
	'     * to any user-defined ThreadGroup, and erase all ThreadLocals
	'     * after executing any top-level task (see WorkQueue.runTask).
	'     * The associated mechanics (mainly in ForkJoinWorkerThread) may
	'     * be JVM-dependent and must access particular Thread class fields
	'     * to achieve this effect.
	'     *
	'     * Style notes
	'     * ===========
	'     *
	'     * Memory ordering relies mainly on Unsafe intrinsics that carry
	'     * the further responsibility of explicitly performing null- and
	'     * bounds- checks otherwise carried out implicitly by JVMs.  This
	'     * can be awkward and ugly, but also reflects the need to control
	'     * outcomes across the unusual cases that arise in very racy code
	'     * with very few invariants. So these explicit checks would exist
	'     * in some form anyway.  All fields are read into locals before
	'     * use, and null-checked if they are references.  This is usually
	'     * done in a "C"-like style of listing declarations at the heads
	'     * of methods or blocks, and using inline assignments on first
	'     * encounter.  Array bounds-checks are usually performed by
	'     * masking with array.length-1, which relies on the invariant that
	'     * these arrays are created with positive lengths, which is itself
	'     * paranoically checked. Nearly all explicit checks lead to
	'     * bypass/return, not exception throws, because they may
	'     * legitimately arise due to cancellation/revocation during
	'     * shutdown.
	'     *
	'     * There is a lot of representation-level coupling among classes
	'     * ForkJoinPool, ForkJoinWorkerThread, and ForkJoinTask.  The
	'     * fields of WorkQueue maintain data structures managed by
	'     * ForkJoinPool, so are directly accessed.  There is little point
	'     * trying to reduce this, since any associated future changes in
	'     * representations will need to be accompanied by algorithmic
	'     * changes anyway. Several methods intrinsically sprawl because
	'     * they must accumulate sets of consistent reads of fields held in
	'     * local variables.  There are also other coding oddities
	'     * (including several unnecessary-looking hoisted null checks)
	'     * that help some methods perform reasonably even when interpreted
	'     * (not compiled).
	'     *
	'     * The order of declarations in this file is (with a few exceptions):
	'     * (1) Static utility functions
	'     * (2) Nested (static) classes
	'     * (3) Static fields
	'     * (4) Fields, along with constants used when unpacking some of them
	'     * (5) Internal control methods
	'     * (6) Callbacks and other support for ForkJoinTask methods
	'     * (7) Exported methods
	'     * (8) Static block initializing statics in minimally dependent order
	'     

		' Static utilities

		''' <summary>
		''' If there is a security manager, makes sure caller has
		''' permission to modify threads.
		''' </summary>
		Private Shared Sub checkPermission()
			Dim security As SecurityManager = System.securityManager
			If security IsNot Nothing Then security.checkPermission(modifyThreadPermission)
		End Sub

		' Nested classes

		''' <summary>
		''' Factory for creating new <seealso cref="ForkJoinWorkerThread"/>s.
		''' A {@code ForkJoinWorkerThreadFactory} must be defined and used
		''' for {@code ForkJoinWorkerThread} subclasses that extend base
		''' functionality or initialize threads with different contexts.
		''' </summary>
		Public Interface ForkJoinWorkerThreadFactory
			''' <summary>
			''' Returns a new worker thread operating in the given pool.
			''' </summary>
			''' <param name="pool"> the pool this thread works in </param>
			''' <returns> the new worker thread </returns>
			''' <exception cref="NullPointerException"> if the pool is null </exception>
			Function newThread(ByVal pool As ForkJoinPool) As ForkJoinWorkerThread
		End Interface

		''' <summary>
		''' Default ForkJoinWorkerThreadFactory implementation; creates a
		''' new ForkJoinWorkerThread.
		''' </summary>
		Friend NotInheritable Class DefaultForkJoinWorkerThreadFactory
			Implements ForkJoinWorkerThreadFactory

			Public Function newThread(ByVal pool As ForkJoinPool) As ForkJoinWorkerThread
				Return New ForkJoinWorkerThread(pool)
			End Function
		End Class

		''' <summary>
		''' Class for artificial tasks that are used to replace the target
		''' of local joins if they are removed from an interior queue slot
		''' in WorkQueue.tryRemoveAndExec. We don't need the proxy to
		''' actually do anything beyond having a unique identity.
		''' </summary>
		Friend NotInheritable Class EmptyTask
			Inherits ForkJoinTask(Of Void)

			Private Const serialVersionUID As Long = -7721805057305804111L
			Friend Sub New() ' force done
				status = ForkJoinTask.NORMAL
			End Sub
			Public Property rawResult As Void
				Get
					Return Nothing
				End Get
				Set(ByVal x As Void)
				End Set
			End Property
			Public Function exec() As Boolean
				Return True
			End Function
		End Class

		' Constants shared across ForkJoinPool and WorkQueue

		' Bounds
		Friend Const SMASK As Integer = &Hffff ' short bits == max index
		Friend Const MAX_CAP As Integer = &H7fff ' max #workers - 1
		Friend Const EVENMASK As Integer = &Hfffe ' even short bits
		Friend Const SQMASK As Integer = &H7e ' max 64 (even) slots

		' Masks and units for WorkQueue.scanState and ctl sp subfield
		Friend Const SCANNING As Integer = 1 ' false when running tasks
		Friend Shared ReadOnly INACTIVE As Integer = 1 << 31 ' must be negative
		Friend Shared ReadOnly SS_SEQ As Integer = 1 << 16 ' version count

		' Mode bits for ForkJoinPool.config and WorkQueue.config
		Friend Shared ReadOnly MODE_MASK As Integer = &Hffff << 16 ' top half of int
		Friend Const LIFO_QUEUE As Integer = 0
		Friend Shared ReadOnly FIFO_QUEUE As Integer = 1 << 16
		Friend Shared ReadOnly SHARED_QUEUE As Integer = 1 << 31 ' must be negative

		''' <summary>
		''' Queues supporting work-stealing as well as external task
		''' submission. See above for descriptions and algorithms.
		''' Performance on most platforms is very sensitive to placement of
		''' instances of both WorkQueues and their arrays -- we absolutely
		''' do not want multiple WorkQueue instances or multiple queue
		''' arrays sharing cache lines. The @Contended annotation alerts
		''' JVMs to try to keep instances apart.
		''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
		Friend NotInheritable Class WorkQueue

			''' <summary>
			''' Capacity of work-stealing queue array upon initialization.
			''' Must be a power of two; at least 4, but should be larger to
			''' reduce or eliminate cacheline sharing among queues.
			''' Currently, it is much larger, as a partial workaround for
			''' the fact that JVMs often place arrays in locations that
			''' share GC bookkeeping (especially cardmarks) such that
			''' per-write accesses encounter serious memory contention.
			''' </summary>
			Friend Shared ReadOnly INITIAL_QUEUE_CAPACITY As Integer = 1 << 13

			''' <summary>
			''' Maximum size for queue arrays. Must be a power of two less
			''' than or equal to 1 << (31 - width of array entry) to ensure
			''' lack of wraparound of index calculations, but defined to a
			''' value a bit less than this to help users trap runaway
			''' programs before saturating systems.
			''' </summary>
			Friend Shared ReadOnly MAXIMUM_QUEUE_CAPACITY As Integer = 1 << 26 ' 64M

			' Instance fields
'JAVA TO VB CONVERTER TODO TASK: There is no VB equivalent to 'volatile':
			Friend scanState As Integer ' versioned, <0: inactive; odd:scanning
			Friend stackPred As Integer ' pool stack (ctl) predecessor
			Friend nsteals As Integer ' number of steals
			Friend hint As Integer ' randomization and stealer index hint
			Friend config As Integer ' pool index and mode
'JAVA TO VB CONVERTER TODO TASK: There is no VB equivalent to 'volatile':
			Friend qlock As Integer ' 1: locked, < 0: terminate; else 0
'JAVA TO VB CONVERTER TODO TASK: There is no VB equivalent to 'volatile':
			Friend base As Integer ' index of next slot for poll
			Friend top As Integer ' index of next slot for push
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
			Friend array As ForkJoinTask(Of ?)() ' the elements (initially unallocated)
			Friend ReadOnly pool As ForkJoinPool ' the containing pool (may be null)
			Friend ReadOnly owner As ForkJoinWorkerThread ' owning thread or null if shared
'JAVA TO VB CONVERTER TODO TASK: There is no VB equivalent to 'volatile':
			Friend parker As Thread ' == owner during call to park; else null
'JAVA TO VB CONVERTER TODO TASK: There is no VB equivalent to 'volatile':
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
			Friend currentJoin As ForkJoinTask(Of ?) ' task being joined in awaitJoin
'JAVA TO VB CONVERTER TODO TASK: There is no VB equivalent to 'volatile':
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
			Friend currentSteal As ForkJoinTask(Of ?) ' mainly used by helpStealer

			Friend Sub New(ByVal pool As ForkJoinPool, ByVal owner As ForkJoinWorkerThread)
				Me.pool = pool
				Me.owner = owner
				' Place indices in the center of array (that is not yet allocated)
					top = CInt(CUInt(INITIAL_QUEUE_CAPACITY) >> 1)
					base = top
			End Sub

			''' <summary>
			''' Returns an exportable index (used by ForkJoinWorkerThread).
			''' </summary>
			Friend Property poolIndex As Integer
				Get
					CInt(CUInt(Return (config And &Hffff)) >> 1) ' ignore odd/even tag bit
				End Get
			End Property

			''' <summary>
			''' Returns the approximate number of tasks in the queue.
			''' </summary>
			Friend Function queueSize() As Integer
				Dim n As Integer = base - top ' non-owner callers must read base first
				Return If(n >= 0, 0, -n) ' ignore transient negative
			End Function

			''' <summary>
			''' Provides a more accurate estimate of whether this queue has
			''' any tasks than does queueSize, by checking whether a
			''' near-empty queue has at least one unclaimed task.
			''' </summary>
			Friend Property empty As Boolean
				Get
	'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
					Dim a As ForkJoinTask(Of ?)()
					Dim n, m, s As Integer
	'JAVA TO VB CONVERTER TODO TASK: Assignments within expressions are not supported in VB
					Return ((n = base - (s = top)) >= 0 OrElse (n = -1 AndAlso ((a = array) Is Nothing OrElse (m = a.Length - 1) < 0 OrElse U.getObject(a, CLng(Fix((m And (s - 1)) << ASHIFT)) + ABASE) Is Nothing))) ' possibly one task
				End Get
			End Property

			''' <summary>
			''' Pushes a task. Call only by owner in unshared queues.  (The
			''' shared-queue version is embedded in method externalPush.)
			''' </summary>
			''' <param name="task"> the task. Caller must ensure non-null. </param>
			''' <exception cref="RejectedExecutionException"> if array cannot be resized </exception>
			Friend Sub push(Of T1)(ByVal task As ForkJoinTask(Of T1))
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
				Dim a As ForkJoinTask(Of ?)()
				Dim p As ForkJoinPool
				Dim b As Integer = base, s As Integer = top, n As Integer
				a = array
				If a IsNot Nothing Then ' ignore if queue removed
					Dim m As Integer = a.Length - 1 ' fenced write for task visibility
					U.putOrderedObject(a, ((m And s) << ASHIFT) + ABASE, task)
					U.putOrderedInt(Me, QTOP, s + 1)
					n = s - b
					If n <= 1 Then
						p = pool
						If p IsNot Nothing Then p.signalWork(p.workQueues, Me)
					ElseIf n >= m Then
						growArray()
					End If
				End If
			End Sub

			''' <summary>
			''' Initializes or doubles the capacity of array. Call either
			''' by owner or with lock held -- it is OK for base, but not
			''' top, to move while resizings are in progress.
			''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
			Friend Function growArray() As ForkJoinTask(Of ?)()
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
				Dim oldA As ForkJoinTask(Of ?)() = array
				Dim size As Integer = If(oldA IsNot Nothing, oldA.Length << 1, INITIAL_QUEUE_CAPACITY)
				If size > MAXIMUM_QUEUE_CAPACITY Then Throw New java.util.concurrent.RejectedExecutionException("Queue capacity exceeded")
				Dim oldMask, t, b As Integer
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
					array = New ForkJoinTask(Of ?)(size - 1){}
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
					Dim a As ForkJoinTask(Of ?)() = array
				oldMask = oldA.Length - 1
				t = top
				b = base
				If oldA IsNot Nothing AndAlso oldMask >= 0 AndAlso t - b > 0 Then
					Dim mask As Integer = size - 1
					Do ' emulate poll from old array, push to new array
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
						Dim x As ForkJoinTask(Of ?)
						Dim oldj As Integer = ((b And oldMask) << ASHIFT) + ABASE
						Dim j As Integer = ((b And mask) << ASHIFT) + ABASE
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
						x = CType(U.getObjectVolatile(oldA, oldj), ForkJoinTask(Of ?))
						If x IsNot Nothing AndAlso U.compareAndSwapObject(oldA, oldj, x, Nothing) Then U.putObjectVolatile(a, j, x)
						b += 1
					Loop While b <> t
				End If
				Return a
			End Function

			''' <summary>
			''' Takes next task, if one exists, in LIFO order.  Call only
			''' by owner in unshared queues.
			''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
			Friend Function pop() As ForkJoinTask(Of ?)
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
				Dim a As ForkJoinTask(Of ?)()
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
				Dim t As ForkJoinTask(Of ?)
				Dim m As Integer
				a = array
				m = a.Length - 1
				If a IsNot Nothing AndAlso m >= 0 Then
					Dim s As Integer
'JAVA TO VB CONVERTER TODO TASK: Assignments within expressions are not supported in VB
					Do While (s = top - 1) - base >= 0
						Dim j As Long = ((m And s) << ASHIFT) + ABASE
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
						t = CType(U.getObject(a, j), ForkJoinTask(Of ?))
						If t Is Nothing Then Exit Do
						If U.compareAndSwapObject(a, j, t, Nothing) Then
							U.putOrderedInt(Me, QTOP, s)
							Return t
						End If
					Loop
				End If
				Return Nothing
			End Function

			''' <summary>
			''' Takes a task in FIFO order if b is base of queue and a task
			''' can be claimed without contention. Specialized versions
			''' appear in ForkJoinPool methods scan and helpStealer.
			''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
			Friend Function pollAt(ByVal b As Integer) As ForkJoinTask(Of ?)
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
				Dim t As ForkJoinTask(Of ?)
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
				Dim a As ForkJoinTask(Of ?)()
				a = array
				If a IsNot Nothing Then
					Dim j As Integer = (((a.Length - 1) And b) << ASHIFT) + ABASE
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
					t = CType(U.getObjectVolatile(a, j), ForkJoinTask(Of ?))
					If t IsNot Nothing AndAlso base = b AndAlso U.compareAndSwapObject(a, j, t, Nothing) Then
						base = b + 1
						Return t
					End If
				End If
				Return Nothing
			End Function

			''' <summary>
			''' Takes next task, if one exists, in FIFO order.
			''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
			Friend Function poll() As ForkJoinTask(Of ?)
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
				Dim a As ForkJoinTask(Of ?)()
				Dim b As Integer
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
				Dim t As ForkJoinTask(Of ?)
				b = base
				a = array
				Do While b - top < 0 AndAlso a IsNot Nothing
					Dim j As Integer = (((a.Length - 1) And b) << ASHIFT) + ABASE
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
					t = CType(U.getObjectVolatile(a, j), ForkJoinTask(Of ?))
					If base = b Then
						If t IsNot Nothing Then
							If U.compareAndSwapObject(a, j, t, Nothing) Then
								base = b + 1
								Return t
							End If
						ElseIf b + 1 = top Then ' now empty
							Exit Do
						End If
					End If
					b = base
					a = array
				Loop
				Return Nothing
			End Function

			''' <summary>
			''' Takes next task, if one exists, in order specified by mode.
			''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
			Friend Function nextLocalTask() As ForkJoinTask(Of ?)
				Return If((config And FIFO_QUEUE) = 0, pop(), poll())
			End Function

			''' <summary>
			''' Returns next task, if one exists, in order specified by mode.
			''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
			Friend Function peek() As ForkJoinTask(Of ?)
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
				Dim a As ForkJoinTask(Of ?)() = array
				Dim m As Integer
				m = a.Length - 1
				If a Is Nothing OrElse m < 0 Then Return Nothing
				Dim i As Integer = If((config And FIFO_QUEUE) = 0, top - 1, base)
				Dim j As Integer = ((i And m) << ASHIFT) + ABASE
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
				Return CType(U.getObjectVolatile(a, j), ForkJoinTask(Of ?))
			End Function

			''' <summary>
			''' Pops the given task only if it is at the current top.
			''' (A shared version is available only via FJP.tryExternalUnpush)
			''' </summary>
			Friend Function tryUnpush(Of T1)(ByVal t As ForkJoinTask(Of T1)) As Boolean
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
				Dim a As ForkJoinTask(Of ?)()
				Dim s As Integer
				a = array
				s = top
				s -= 1
				If a IsNot Nothing AndAlso s <> base AndAlso U.compareAndSwapObject(a, (((a.Length - 1) And s) << ASHIFT) + ABASE, t, Nothing) Then
					U.putOrderedInt(Me, QTOP, s)
					Return True
				End If
				Return False
			End Function

			''' <summary>
			''' Removes and cancels all known tasks, ignoring any exceptions.
			''' </summary>
			Friend Sub cancelAll()
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
				Dim t As ForkJoinTask(Of ?)
				t = currentJoin
				If t IsNot Nothing Then
					currentJoin = Nothing
					ForkJoinTask.cancelIgnoringExceptions(t)
				End If
				t = currentSteal
				If t IsNot Nothing Then
					currentSteal = Nothing
					ForkJoinTask.cancelIgnoringExceptions(t)
				End If
				t = poll()
				Do While t IsNot Nothing
					ForkJoinTask.cancelIgnoringExceptions(t)
					t = poll()
				Loop
			End Sub

			' Specialized execution methods

			''' <summary>
			''' Polls and runs tasks until empty.
			''' </summary>
			Friend Sub pollAndExecAll()
				Dim t As ForkJoinTask(Of ?)
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
'JAVA TO VB CONVERTER TODO TASK: Assignments within expressions are not supported in VB
				Do While (t = poll()) IsNot Nothing
					t.doExec()
				Loop
			End Sub

			''' <summary>
			''' Removes and executes all local tasks. If LIFO, invokes
			''' pollAndExecAll. Otherwise implements a specialized pop loop
			''' to exec until empty.
			''' </summary>
			Friend Sub execLocalTasks()
				Dim b As Integer = base, m As Integer, s As Integer
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
				Dim a As ForkJoinTask(Of ?)() = array
				s = top - 1
				m = a.Length - 1
				If b - s <= 0 AndAlso a IsNot Nothing AndAlso m >= 0 Then
					If (config And FIFO_QUEUE) = 0 Then
						Dim t As ForkJoinTask(Of ?)
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
						Do
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
							t = CType(U.getAndSetObject(a, ((m And s) << ASHIFT) + ABASE, Nothing), ForkJoinTask(Of ?))
							If t Is Nothing Then Exit Do
							U.putOrderedInt(Me, QTOP, s)
							t.doExec()
							s = top - 1
							If base - s > 0 Then Exit Do
						Loop
					Else
						pollAndExecAll()
					End If
				End If
			End Sub

			''' <summary>
			''' Executes the given task and any remaining local tasks.
			''' </summary>
			Friend Sub runTask(Of T1)(ByVal task As ForkJoinTask(Of T1))
				If task IsNot Nothing Then
					scanState = scanState And Not SCANNING ' mark as busy
						currentSteal = task
						task.doExec()
					U.putOrderedObject(Me, QCURRENTSTEAL, Nothing) ' release for GC
					execLocalTasks()
					Dim thread_Renamed As ForkJoinWorkerThread = owner
					nsteals += 1
					If nsteals < 0 Then ' collect on overflow transferStealCount(pool)
					scanState = scanState Or SCANNING
					If thread_Renamed IsNot Nothing Then thread_Renamed.afterTopLevelExec()
				End If
			End Sub

			''' <summary>
			''' Adds steal count to pool stealCounter if it exists, and resets.
			''' </summary>
			Friend Sub transferStealCount(ByVal p As ForkJoinPool)
				Dim sc As java.util.concurrent.atomic.AtomicLong
				sc = p.stealCounter
				If p IsNot Nothing AndAlso sc IsNot Nothing Then
					Dim s As Integer = nsteals
					nsteals = 0 ' if negative, correct for overflow
					sc.getAndAdd(CLng(Fix(If(s < 0,  java.lang.[Integer].Max_Value, s))))
				End If
			End Sub

			''' <summary>
			''' If present, removes from queue and executes the given task,
			''' or any other cancelled task. Used only by awaitJoin.
			''' </summary>
			''' <returns> true if queue empty and task not known to be done </returns>
			Friend Function tryRemoveAndExec(Of T1)(ByVal task As ForkJoinTask(Of T1)) As Boolean
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
				Dim a As ForkJoinTask(Of ?)()
				Dim m, s, b, n As Integer
				a = array
				m = a.Length - 1
				If a IsNot Nothing AndAlso m >= 0 AndAlso task IsNot Nothing Then
'JAVA TO VB CONVERTER TODO TASK: Assignments within expressions are not supported in VB
					n = (s = top) - (b = base)
					Do While n > 0
						Dim t As ForkJoinTask(Of ?)
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
						Do ' traverse from s to b
							s -= 1
							Dim j As Long = ((s And m) << ASHIFT) + ABASE
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
							t = CType(U.getObject(a, j), ForkJoinTask(Of ?))
							If t Is Nothing Then
								Return s + 1 = top ' shorter than expected
							ElseIf t Is task Then
								Dim removed As Boolean = False
								If s + 1 = top Then ' pop
									If U.compareAndSwapObject(a, j, task, Nothing) Then
										U.putOrderedInt(Me, QTOP, s)
										removed = True
									End If
								ElseIf base = b Then ' replace with proxy
									removed = U.compareAndSwapObject(a, j, task, New EmptyTask)
								End If
								If removed Then task.doExec()
								Exit Do
							ElseIf t.status < 0 AndAlso s + 1 = top Then
								If U.compareAndSwapObject(a, j, t, Nothing) Then U.putOrderedInt(Me, QTOP, s)
								Exit Do ' was cancelled
							End If
							n -= 1
							If n = 0 Then Return False
						Loop
						If task.status < 0 Then Return False
						n = (s = top) - (b = base)
					Loop
				End If
				Return True
			End Function

			''' <summary>
			''' Pops task if in the same CC computation as the given task,
			''' in either shared or owned mode. Used only by helpComplete.
			''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
			Friend Function popCC(Of T1)(ByVal task As CountedCompleter(Of T1), ByVal mode As Integer) As CountedCompleter(Of ?)
				Dim s As Integer
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
				Dim a As ForkJoinTask(Of ?)()
				Dim o As Object
				s = top
				a = array
				If base - s < 0 AndAlso a IsNot Nothing Then
					Dim j As Long = (((a.Length - 1) And (s - 1)) << ASHIFT) + ABASE
					o = U.getObjectVolatile(a, j)
					If o IsNot Nothing AndAlso (TypeOf o Is CountedCompleter) Then
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
						Dim t As CountedCompleter(Of ?) = CType(o, CountedCompleter(Of ?))
						Dim r As CountedCompleter(Of ?) = t
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
						Do
							If r Is task Then
								If mode < 0 Then ' must lock
									If U.compareAndSwapInt(Me, QLOCK, 0, 1) Then
										If top = s AndAlso array = a AndAlso U.compareAndSwapObject(a, j, t, Nothing) Then
											U.putOrderedInt(Me, QTOP, s - 1)
											U.putOrderedInt(Me, QLOCK, 0)
											Return t
										End If
										U.compareAndSwapInt(Me, QLOCK, 1, 0)
									End If
								ElseIf U.compareAndSwapObject(a, j, t, Nothing) Then
									U.putOrderedInt(Me, QTOP, s - 1)
									Return t
								End If
								Exit Do
							Else
								r = r.completer
								If r Is Nothing Then ' try parent Exit Do
								End If
						Loop
					End If
				End If
				Return Nothing
			End Function

			''' <summary>
			''' Steals and runs a task in the same CC computation as the
			''' given task if one exists and can be taken without
			''' contention. Otherwise returns a checksum/control value for
			''' use by method helpComplete.
			''' </summary>
			''' <returns> 1 if successful, 2 if retryable (lost to another
			''' stealer), -1 if non-empty but no matching task found, else
			''' the base index, forced negative. </returns>
			Friend Function pollAndExecCC(Of T1)(ByVal task As CountedCompleter(Of T1)) As Integer
				Dim b, h As Integer
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
				Dim a As ForkJoinTask(Of ?)()
				Dim o As Object
				b = base
				a = array
				If b - top >= 0 OrElse a Is Nothing Then
					h = b Or  java.lang.[Integer].MIN_VALUE ' to sense movement on re-poll
				Else
					Dim j As Long = (((a.Length - 1) And b) << ASHIFT) + ABASE
					o = U.getObjectVolatile(a, j)
					If o Is Nothing Then
						h = 2 ' retryable
					ElseIf Not(TypeOf o Is CountedCompleter) Then
						h = -1 ' unmatchable
					Else
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
						Dim t As CountedCompleter(Of ?) = CType(o, CountedCompleter(Of ?))
						Dim r As CountedCompleter(Of ?) = t
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
						Do
							If r Is task Then
								If base = b AndAlso U.compareAndSwapObject(a, j, t, Nothing) Then
									base = b + 1
									t.doExec()
									h = 1 ' success
								Else
									h = 2 ' lost CAS
								End If
								Exit Do
							Else
								r = r.completer
								If r Is Nothing Then
									h = -1 ' unmatched
									Exit Do
								End If
								End If
						Loop
					End If
				End If
				Return h
			End Function

			''' <summary>
			''' Returns true if owned and not known to be blocked.
			''' </summary>
			Friend Property apparentlyUnblocked As Boolean
				Get
					Dim wt As Thread
					Dim s As Thread.State
	'JAVA TO VB CONVERTER TODO TASK: Assignments within expressions are not supported in VB
					Return (scanState >= 0 AndAlso (wt = owner) IsNot Nothing AndAlso (s = wt.state) <> Thread.State.BLOCKED AndAlso s <> Thread.State.WAITING AndAlso s <> Thread.State.TIMED_WAITING)
				End Get
			End Property

			' Unsafe mechanics. Note that some are (and must be) the same as in FJP
			Private Shared ReadOnly U As sun.misc.Unsafe
			Private Shared ReadOnly ABASE As Integer
			Private Shared ReadOnly ASHIFT As Integer
			Private Shared ReadOnly QTOP As Long
			Private Shared ReadOnly QLOCK As Long
			Private Shared ReadOnly QCURRENTSTEAL As Long
			Shared Sub New()
				Try
					U = sun.misc.Unsafe.unsafe
					Dim wk As  [Class] = GetType(WorkQueue)
					Dim ak As  [Class] = GetType(ForkJoinTask())
					QTOP = U.objectFieldOffset(wk.getDeclaredField("top"))
					QLOCK = U.objectFieldOffset(wk.getDeclaredField("qlock"))
					QCURRENTSTEAL = U.objectFieldOffset(wk.getDeclaredField("currentSteal"))
					ABASE = U.arrayBaseOffset(ak)
					Dim scale As Integer = U.arrayIndexScale(ak)
					If (scale And (scale - 1)) <> 0 Then Throw New [Error]("data type scale not a power of two")
					ASHIFT = 31 -  java.lang.[Integer].numberOfLeadingZeros(scale)
				Catch e As Exception
					Throw New [Error](e)
				End Try
			End Sub
		End Class

		' static fields (initialized in static initializer below)

		''' <summary>
		''' Creates a new ForkJoinWorkerThread. This factory is used unless
		''' overridden in ForkJoinPool constructors.
		''' </summary>
		Public Shared ReadOnly defaultForkJoinWorkerThreadFactory As ForkJoinWorkerThreadFactory

		''' <summary>
		''' Permission required for callers of methods that may start or
		''' kill threads.
		''' </summary>
		Private Shared ReadOnly modifyThreadPermission As RuntimePermission

		''' <summary>
		''' Common (static) pool. Non-null for public use unless a static
		''' construction exception, but internal usages null-check on use
		''' to paranoically avoid potential initialization circularities
		''' as well as to simplify generated code.
		''' </summary>
		Friend Shared ReadOnly common As ForkJoinPool

		''' <summary>
		''' Common pool parallelism. To allow simpler use and management
		''' when common pool threads are disabled, we allow the underlying
		''' common.parallelism field to be zero, but in that case still report
		''' parallelism as 1 to reflect resulting caller-runs mechanics.
		''' </summary>
		Friend Shared ReadOnly commonParallelism As Integer

		''' <summary>
		''' Limit on spare thread construction in tryCompensate.
		''' </summary>
		Private Shared commonMaxSpares As Integer

		''' <summary>
		''' Sequence number for creating workerNamePrefix.
		''' </summary>
		Private Shared poolNumberSequence As Integer

		''' <summary>
		''' Returns the next sequence number. We don't expect this to
		''' ever contend, so use simple builtin sync.
		''' </summary>
		<MethodImpl(MethodImplOptions.Synchronized)> _
		Private Shared Function nextPoolId() As Integer
				poolNumberSequence += 1
				Return poolNumberSequence
		End Function

		' static configuration constants

		''' <summary>
		''' Initial timeout value (in nanoseconds) for the thread
		''' triggering quiescence to park waiting for new work. On timeout,
		''' the thread will instead try to shrink the number of
		''' workers. The value should be large enough to avoid overly
		''' aggressive shrinkage during most transient stalls (long GCs
		''' etc).
		''' </summary>
		Private Shared ReadOnly IDLE_TIMEOUT As Long = 2000L * 1000L * 1000L ' 2sec

		''' <summary>
		''' Tolerance for idle timeouts, to cope with timer undershoots
		''' </summary>
		Private Shared ReadOnly TIMEOUT_SLOP As Long = 20L * 1000L * 1000L ' 20ms

		''' <summary>
		''' The initial value for commonMaxSpares during static
		''' initialization. The value is far in excess of normal
		''' requirements, but also far short of MAX_CAP and typical
		''' OS thread limits, so allows JVMs to catch misuse/abuse
		''' before running out of resources needed to do so.
		''' </summary>
		Private Const DEFAULT_COMMON_MAX_SPARES As Integer = 256

		''' <summary>
		''' Number of times to spin-wait before blocking. The spins (in
		''' awaitRunStateLock and awaitWork) currently use randomized
		''' spins. Currently set to zero to reduce CPU usage.
		''' 
		''' If greater than zero the value of SPINS must be a power
		''' of two, at least 4.  A value of 2048 causes spinning for a
		''' small fraction of typical context-switch times.
		''' 
		''' If/when MWAIT-like intrinsics becomes available, they
		''' may allow quieter spinning.
		''' </summary>
		Private Const SPINS As Integer = 0

		''' <summary>
		''' Increment for seed generators. See class ThreadLocal for
		''' explanation.
		''' </summary>
		Private Const SEED_INCREMENT As Integer = &H9e3779b9L

	'    
	'     * Bits and masks for field ctl, packed with 4 16 bit subfields:
	'     * AC: Number of active running workers minus target parallelism
	'     * TC: Number of total workers minus target parallelism
	'     * SS: version count and status of top waiting thread
	'     * ID: poolIndex of top of Treiber stack of waiters
	'     *
	'     * When convenient, we can extract the lower 32 stack top bits
	'     * (including version bits) as sp=(int)ctl.  The offsets of counts
	'     * by the target parallelism and the positionings of fields makes
	'     * it possible to perform the most common checks via sign tests of
	'     * fields: When ac is negative, there are not enough active
	'     * workers, when tc is negative, there are not enough total
	'     * workers.  When sp is non-zero, there are waiting workers.  To
	'     * deal with possibly negative fields, we use casts in and out of
	'     * "short" and/or signed shifts to maintain signedness.
	'     *
	'     * Because it occupies uppermost bits, we can add one active count
	'     * using getAndAddLong of AC_UNIT, rather than CAS, when returning
	'     * from a blocked join.  Other updates entail multiple subfields
	'     * and masking, requiring CAS.
	'     

		' Lower and upper word masks
		Private Const SP_MASK As Long = &HffffffffL
		Private Shared ReadOnly UC_MASK As Long = Not SP_MASK

		' Active counts
		Private Const AC_SHIFT As Integer = 48
		Private Shared ReadOnly AC_UNIT As Long = &H1L << AC_SHIFT
		Private Shared ReadOnly AC_MASK As Long = &HffffL << AC_SHIFT

		' Total counts
		Private Const TC_SHIFT As Integer = 32
		Private Shared ReadOnly TC_UNIT As Long = &H1L << TC_SHIFT
		Private Shared ReadOnly TC_MASK As Long = &HffffL << TC_SHIFT
		Private Shared ReadOnly ADD_WORKER As Long = &H1L << (TC_SHIFT + 15) ' sign

		' runState bits: SHUTDOWN must be negative, others arbitrary powers of two
		Private Const RSLOCK As Integer = 1
		Private Shared ReadOnly RSIGNAL As Integer = 1 << 1
		Private Shared ReadOnly STARTED As Integer = 1 << 2
		Private Shared ReadOnly [STOP] As Integer = 1 << 29
		Private Shared ReadOnly TERMINATED As Integer = 1 << 30
		Private Shared ReadOnly SHUTDOWN_Renamed As Integer = 1 << 31

		' Instance fields
'JAVA TO VB CONVERTER TODO TASK: There is no VB equivalent to 'volatile':
		Friend ctl As Long ' main pool control
'JAVA TO VB CONVERTER TODO TASK: There is no VB equivalent to 'volatile':
		Friend runState As Integer ' lockable status
		Friend ReadOnly config As Integer ' parallelism, mode
		Friend indexSeed As Integer ' to generate worker index
'JAVA TO VB CONVERTER TODO TASK: There is no VB equivalent to 'volatile':
		Friend workQueues As WorkQueue() ' main registry
		Friend ReadOnly factory As ForkJoinWorkerThreadFactory
		Friend ReadOnly ueh As Thread.UncaughtExceptionHandler ' per-worker UEH
		Friend ReadOnly workerNamePrefix As String ' to create worker name string
'JAVA TO VB CONVERTER TODO TASK: There is no VB equivalent to 'volatile':
		Friend stealCounter As java.util.concurrent.atomic.AtomicLong ' also used as sync monitor

		''' <summary>
		''' Acquires the runState lock; returns current (locked) runState.
		''' </summary>
		Private Function lockRunState() As Integer
			Dim rs As Integer
'JAVA TO VB CONVERTER TODO TASK: Assignments within expressions are not supported in VB
			Return (If(((rs = runState) And RSLOCK) <> 0 OrElse (Not U.compareAndSwapInt(Me, RUNSTATE, rs, rs = rs Or RSLOCK)), awaitRunStateLock(), rs))
		End Function

		''' <summary>
		''' Spins and/or blocks until runstate lock is available.  See
		''' above for explanation.
		''' </summary>
		Private Function awaitRunStateLock() As Integer
			Dim lock As Object
			Dim wasInterrupted As Boolean = False
'JAVA TO VB CONVERTER TODO TASK: The following line could not be converted:
			for (int spins = SPINS, r = 0, rs, ns;;)
				rs = runState
				If (rs And RSLOCK) = 0 Then
'JAVA TO VB CONVERTER TODO TASK: Assignments within expressions are not supported in VB
					If U.compareAndSwapInt(Me, RUNSTATE, rs, ns = rs Or RSLOCK) Then
						If wasInterrupted Then
							Try
								Thread.CurrentThread.Interrupt()
							Catch ignore As SecurityException
							End Try
						End If
						Return ns
					End If
				ElseIf r = 0 Then
					r = java.util.concurrent.ThreadLocalRandom.nextSecondarySeed()
				ElseIf spins_Renamed > 0 Then
					r = r Xor r << 6 ' xorshift
					r = r Xor CInt(CUInt(r) >> 21)
					r = r Xor r << 7
					If r >= 0 Then spins_Renamed -= 1
				Else
					lock = stealCounter
					If (rs And STARTED) = 0 OrElse lock Is Nothing Then
						Thread.yield() ' initialization race
					ElseIf U.compareAndSwapInt(Me, RUNSTATE, rs, rs Or RSIGNAL) Then
						SyncLock lock
							If (runState And RSIGNAL) <> 0 Then
								Try
									lock.wait()
								Catch ie As InterruptedException
									If Not(TypeOf Thread.CurrentThread Is ForkJoinWorkerThread) Then wasInterrupted = True
								End Try
							Else
								lock.notifyAll()
							End If
						End SyncLock
					End If
					End If
		End Function

		''' <summary>
		''' Unlocks and sets runState to newRunState.
		''' </summary>
		''' <param name="oldRunState"> a value returned from lockRunState </param>
		''' <param name="newRunState"> the next value (must have lock bit clear). </param>
		Private Sub unlockRunState(ByVal oldRunState As Integer, ByVal newRunState As Integer)
			If Not U.compareAndSwapInt(Me, RUNSTATE, oldRunState, newRunState) Then
				Dim lock As Object = stealCounter
				runState = newRunState ' clears RSIGNAL bit
				If lock IsNot Nothing Then
					SyncLock lock
						lock.notifyAll()
					End SyncLock
				End If
			End If
		End Sub

		' Creating, registering and deregistering workers

		''' <summary>
		''' Tries to construct and start one worker. Assumes that total
		''' count has already been incremented as a reservation.  Invokes
		''' deregisterWorker on any failure.
		''' </summary>
		''' <returns> true if successful </returns>
		Private Function createWorker() As Boolean
			Dim fac As ForkJoinWorkerThreadFactory = factory
			Dim ex As Throwable = Nothing
			Dim wt As ForkJoinWorkerThread = Nothing
			Try
				wt = fac.newThread(Me)
				If fac IsNot Nothing AndAlso wt IsNot Nothing Then
					wt.start()
					Return True
				End If
			Catch rex As Throwable
				ex = rex
			End Try
			deregisterWorker(wt, ex)
			Return False
		End Function

		''' <summary>
		''' Tries to add one worker, incrementing ctl counts before doing
		''' so, relying on createWorker to back out on failure.
		''' </summary>
		''' <param name="c"> incoming ctl value, with total count negative and no
		''' idle workers.  On CAS failure, c is refreshed and retried if
		''' this holds (otherwise, a new worker is not needed). </param>
		Private Sub tryAddWorker(ByVal c As Long)
			Dim add As Boolean = False
			Do
				Dim nc As Long = ((AC_MASK And (c + AC_UNIT)) Or (TC_MASK And (c + TC_UNIT)))
				If ctl = c Then
					Dim rs, stop_Renamed As Integer ' check if terminating
'JAVA TO VB CONVERTER TODO TASK: Assignments within expressions are not supported in VB
					stop_Renamed = (rs = lockRunState()) And [STOP]
					If stop_Renamed = 0 Then add = U.compareAndSwapLong(Me, CTL, c, nc)
					unlockRunState(rs, rs And (Not RSLOCK))
					If stop_Renamed <> 0 Then Exit Do
					If add Then
						createWorker()
						Exit Do
					End If
				End If
				c = ctl
			Loop While (c And ADD_WORKER) <> 0L AndAlso CInt(c) = 0
		End Sub

		''' <summary>
		''' Callback from ForkJoinWorkerThread constructor to establish and
		''' record its WorkQueue.
		''' </summary>
		''' <param name="wt"> the worker thread </param>
		''' <returns> the worker's queue </returns>
		Friend Function registerWorker(ByVal wt As ForkJoinWorkerThread) As WorkQueue
			Dim handler As Thread.UncaughtExceptionHandler
			wt.daemon = True ' configure thread
			handler = ueh
			If handler IsNot Nothing Then wt.uncaughtExceptionHandler = handler
			Dim w As New WorkQueue(Me, wt)
			Dim i As Integer = 0 ' assign a pool index
			Dim mode As Integer = config And MODE_MASK
			Dim rs As Integer = lockRunState()
			Try
				Dim ws As WorkQueue() ' skip if no array
				Dim n As Integer
				ws = workQueues
				n = ws.Length
				If ws IsNot Nothing AndAlso n > 0 Then
						indexSeed += SEED_INCREMENT
						Dim s As Integer = indexSeed
					Dim m As Integer = n - 1
					i = ((s << 1) Or 1) And m ' odd-numbered indices
					If ws(i) IsNot Nothing Then ' collision
						Dim probes As Integer = 0 ' step by approx half n
						Dim [step] As Integer = If(n <= 4, 2, ((CInt(CUInt(n) >> 1)) And EVENMASK) + 2)
'JAVA TO VB CONVERTER TODO TASK: Assignments within expressions are not supported in VB
						Do While ws(i = (i + [step]) And m) IsNot Nothing
							probes += 1
							If probes >= n Then
									ws = java.util.Arrays.copyOf(ws, n <<= 1)
									workQueues = ws
								m = n - 1
								probes = 0
							End If
						Loop
					End If
					w.hint = s ' use as random seed
					w.config = i Or mode
					w.scanState = i ' publication fence
					ws(i) = w
				End If
			Finally
				unlockRunState(rs, rs And (Not RSLOCK))
			End Try
			wt.name = workerNamePrefix.concat(Convert.ToString(CInt(CUInt(i) >> 1)))
			Return w
		End Function

		''' <summary>
		''' Final callback from terminating worker, as well as upon failure
		''' to construct or start a worker.  Removes record of worker from
		''' array, and adjusts counts. If pool is shutting down, tries to
		''' complete termination.
		''' </summary>
		''' <param name="wt"> the worker thread, or null if construction failed </param>
		''' <param name="ex"> the exception causing failure, or null if none </param>
		Friend Sub deregisterWorker(ByVal wt As ForkJoinWorkerThread, ByVal ex As Throwable)
			Dim w As WorkQueue = Nothing
			w = wt.workQueue
			If wt IsNot Nothing AndAlso w IsNot Nothing Then
				Dim ws As WorkQueue() ' remove index from array
				Dim idx As Integer = w.config And SMASK
				Dim rs As Integer = lockRunState()
				ws = workQueues
				If ws IsNot Nothing AndAlso ws.Length > idx AndAlso ws(idx) Is w Then ws(idx) = Nothing
				unlockRunState(rs, rs And (Not RSLOCK))
			End If
			Dim c As Long ' decrement counts
			Do
'JAVA TO VB CONVERTER TODO TASK: Assignments within expressions are not supported in VB
			Loop While Not U.compareAndSwapLong(Me, CTL, c = ctl, ((AC_MASK And (c - AC_UNIT)) Or (TC_MASK And (c - TC_UNIT)) Or (SP_MASK And c)))
			If w IsNot Nothing Then
				w.qlock = -1 ' ensure set
				w.transferStealCount(Me)
				w.cancelAll() ' cancel remaining tasks
			End If
			Do ' possibly replace
				Dim ws As WorkQueue()
				Dim m, sp As Integer
				ws = workQueues
				m = ws.Length - 1
				If tryTerminate(False, False) OrElse w Is Nothing OrElse w.array Is Nothing OrElse (runState And [STOP]) <> 0 OrElse ws Is Nothing OrElse m < 0 Then ' already terminating Exit Do
'JAVA TO VB CONVERTER TODO TASK: Assignments within expressions are not supported in VB
				sp = CInt(Fix(c = ctl))
				If sp <> 0 Then ' wake up replacement
					If tryRelease(c, ws(sp And m), AC_UNIT) Then Exit Do
				ElseIf ex IsNot Nothing AndAlso (c And ADD_WORKER) <> 0L Then
					tryAddWorker(c) ' create replacement
					Exit Do
				Else ' don't need replacement
					Exit Do
				End If
			Loop
			If ex Is Nothing Then ' help clean on way out
				ForkJoinTask.helpExpungeStaleExceptions()
			Else ' rethrow
				ForkJoinTask.rethrow(ex)
			End If
		End Sub

		' Signalling

		''' <summary>
		''' Tries to create or activate a worker if too few are active.
		''' </summary>
		''' <param name="ws"> the worker array to use to find signallees </param>
		''' <param name="q"> a WorkQueue --if non-null, don't retry if now empty </param>
		Friend Sub signalWork(ByVal ws As WorkQueue(), ByVal q As WorkQueue)
			Dim c As Long
			Dim sp, i As Integer
			Dim v As WorkQueue
			Dim p As Thread
			c = ctl
			Do While c < 0L ' too few active
				sp = CInt(c)
				If sp = 0 Then ' no idle workers
					If (c And ADD_WORKER) <> 0L Then ' too few workers tryAddWorker(c)
					Exit Do
				End If
				If ws Is Nothing Then ' unstarted/terminated Exit Do
				i = sp And SMASK
				If ws.Length <= i Then ' terminated Exit Do
				v = ws(i)
				If v Is Nothing Then ' terminating Exit Do
				Dim vs As Integer = (sp + SS_SEQ) And Not INACTIVE ' next scanState
				Dim d As Integer = sp - v.scanState ' screen CAS
				Dim nc As Long = (UC_MASK And (c + AC_UNIT)) Or (SP_MASK And v.stackPred)
				If d = 0 AndAlso U.compareAndSwapLong(Me, CTL, c, nc) Then
					v.scanState = vs ' activate v
					p = v.parker
					If p IsNot Nothing Then U.unpark(p)
					Exit Do
				End If
				If q IsNot Nothing AndAlso q.base = q.top Then ' no more work Exit Do
				c = ctl
			Loop
		End Sub

		''' <summary>
		''' Signals and releases worker v if it is top of idle worker
		''' stack.  This performs a one-shot version of signalWork only if
		''' there is (apparently) at least one idle worker.
		''' </summary>
		''' <param name="c"> incoming ctl value </param>
		''' <param name="v"> if non-null, a worker </param>
		''' <param name="inc"> the increment to active count (zero when compensating) </param>
		''' <returns> true if successful </returns>
		Private Function tryRelease(ByVal c As Long, ByVal v As WorkQueue, ByVal inc As Long) As Boolean
			Dim sp As Integer = CInt(c), vs As Integer = (sp + SS_SEQ) And Not INACTIVE
			Dim p As Thread
			If v IsNot Nothing AndAlso v.scanState = sp Then ' v is at top of stack
				Dim nc As Long = (UC_MASK And (c + inc)) Or (SP_MASK And v.stackPred)
				If U.compareAndSwapLong(Me, CTL, c, nc) Then
					v.scanState = vs
					p = v.parker
					If p IsNot Nothing Then U.unpark(p)
					Return True
				End If
			End If
			Return False
		End Function

		' Scanning for tasks

		''' <summary>
		''' Top-level runloop for workers, called by ForkJoinWorkerThread.run.
		''' </summary>
		Friend Sub runWorker(ByVal w As WorkQueue)
			w.growArray() ' allocate queue
			Dim seed As Integer = w.hint ' initially holds randomization hint
			Dim r As Integer = If(seed = 0, 1, seed) ' avoid 0 for xorShift
			Dim t As ForkJoinTask(Of ?)
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
			Do
				t = scan(w, r)
				If t IsNot Nothing Then
					w.runTask(t)
				ElseIf Not awaitWork(w, r) Then
					Exit Do
				End If
				r = r Xor r << 13 ' xorshift
				r = r Xor CInt(CUInt(r) >> 17)
				r = r Xor r << 5
			Loop
		End Sub

		''' <summary>
		''' Scans for and tries to steal a top-level task. Scans start at a
		''' random location, randomly moving on apparent contention,
		''' otherwise continuing linearly until reaching two consecutive
		''' empty passes over all queues with the same checksum (summing
		''' each base index of each queue, that moves on each steal), at
		''' which point the worker tries to inactivate and then re-scans,
		''' attempting to re-activate (itself or some other worker) if
		''' finding a task; otherwise returning null to await work.  Scans
		''' otherwise touch as little memory as possible, to reduce
		''' disruption on other scanning threads.
		''' </summary>
		''' <param name="w"> the worker (via its WorkQueue) </param>
		''' <param name="r"> a random seed </param>
		''' <returns> a task, or null if none found </returns>
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
		Private Function scan(ByVal w As WorkQueue, ByVal r As Integer) As ForkJoinTask(Of ?)
			Dim ws As WorkQueue()
			Dim m As Integer
			ws = workQueues
			m = ws.Length - 1
			If ws IsNot Nothing AndAlso m > 0 AndAlso w IsNot Nothing Then
				Dim ss As Integer = w.scanState ' initially non-negative
				Dim origin As Integer = r And m
				Dim k As Integer = origin
				Dim oldSum As Integer = 0
				Dim checkSum As Integer = 0
				Do
					Dim q As WorkQueue
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
					Dim a As ForkJoinTask(Of ?)()
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
					Dim t As ForkJoinTask(Of ?)
					Dim b, n As Integer
					Dim c As Long
					q = ws(k)
					If q IsNot Nothing Then
'JAVA TO VB CONVERTER TODO TASK: Assignments within expressions are not supported in VB
						n = (b = q.base) - q.top
						a = q.array
						If n < 0 AndAlso a IsNot Nothing Then ' non-empty
							Dim i As Long = (((a.Length - 1) And b) << ASHIFT) + ABASE
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
							t = (CType(U.getObjectVolatile(a, i), ForkJoinTask(Of ?)))
							If t IsNot Nothing AndAlso q.base = b Then
								If ss >= 0 Then
									If U.compareAndSwapObject(a, i, t, Nothing) Then
										q.base = b + 1
										If n < -1 Then ' signal others signalWork(ws, q)
										Return t
									End If
								ElseIf oldSum = 0 AndAlso w.scanState < 0 Then ' try to activate
'JAVA TO VB CONVERTER TODO TASK: Assignments within expressions are not supported in VB
									tryRelease(c = ctl, ws(m And CInt(c)), AC_UNIT)
								End If
							End If
							If ss < 0 Then ' refresh ss = w.scanState
							r = r Xor r << 1
							r = r Xor CInt(CUInt(r) >> 3)
							r = r Xor r << 10
								k = r And m
								origin = k
								checkSum = 0
								oldSum = checkSum
							Continue Do
						End If
						checkSum += b
					End If
					k = (k + 1) And m
					If k = origin Then ' continue until stable
						ss = w.scanState
						oldSum = checkSum
						If (ss >= 0 OrElse (ss = ss)) AndAlso oldSum = oldSum Then
							If ss < 0 OrElse w.qlock < 0 Then ' already inactive Exit Do
							Dim ns As Integer = ss Or INACTIVE ' try to inactivate
'JAVA TO VB CONVERTER TODO TASK: Assignments within expressions are not supported in VB
							Dim nc As Long = ((SP_MASK And ns) Or (UC_MASK And ((c = ctl) - AC_UNIT)))
							w.stackPred = CInt(c) ' hold prev stack top
							U.putInt(w, QSCANSTATE, ns)
							If U.compareAndSwapLong(Me, CTL, c, nc) Then
								ss = ns
							Else
								w.scanState = ss ' back out
							End If
						End If
						checkSum = 0
					End If
				Loop
			End If
			Return Nothing
		End Function

		''' <summary>
		''' Possibly blocks worker w waiting for a task to steal, or
		''' returns false if the worker should terminate.  If inactivating
		''' w has caused the pool to become quiescent, checks for pool
		''' termination, and, so long as this is not the only worker, waits
		''' for up to a given duration.  On timeout, if ctl has not
		''' changed, terminates the worker, which will in turn wake up
		''' another worker to possibly repeat this process.
		''' </summary>
		''' <param name="w"> the calling worker </param>
		''' <param name="r"> a random seed (for spins) </param>
		''' <returns> false if the worker should terminate </returns>
		Private Function awaitWork(ByVal w As WorkQueue, ByVal r As Integer) As Boolean
			If w Is Nothing OrElse w.qlock < 0 Then ' w is terminating Return False
'JAVA TO VB CONVERTER TODO TASK: The following line could not be converted:
			for (int pred = w.stackPred, spins = SPINS, ss;;)
				ss = w.scanState
				If ss >= 0 Then
					break
				ElseIf spins_Renamed > 0 Then
					r = r Xor r << 6
					r = r Xor CInt(CUInt(r) >> 21)
					r = r Xor r << 7
					spins_Renamed -= 1
					If r >= 0 AndAlso spins_Renamed = 0 Then ' randomize spins
						Dim v As WorkQueue
						Dim ws As WorkQueue()
						Dim s, j As Integer
						Dim sc As java.util.concurrent.atomic.AtomicLong
						ws = workQueues
						j = pred And SMASK
						v = ws(j)
						If pred <> 0 AndAlso ws IsNot Nothing AndAlso j < ws.Length AndAlso v IsNot Nothing AndAlso (v.parker Is Nothing OrElse v.scanState >= 0) Then ' see if pred parking spins_Renamed = SPINS ' continue spinning
					End If
				ElseIf w.qlock < 0 Then ' recheck after spins
					Return False
				ElseIf Not Thread.interrupted() Then
					Dim c, prevctl, parkTime, deadline As Long
'JAVA TO VB CONVERTER TODO TASK: Assignments within expressions are not supported in VB
					Dim ac As Integer = CInt(Fix((c = ctl) >> AC_SHIFT)) + (config And SMASK)
					If (ac <= 0 AndAlso tryTerminate(False, False)) OrElse (runState And [STOP]) <> 0 Then ' pool terminating Return False
					If ac <= 0 AndAlso ss = CInt(c) Then ' is last waiter
						prevctl = (UC_MASK And (c + AC_UNIT)) Or (SP_MASK And pred)
						Dim t As Integer = CShort(CLng(CULng(c) >> TC_SHIFT)) ' shrink excess spares
						If t > 2 AndAlso U.compareAndSwapLong(Me, CTL, c, prevctl) Then Return False ' else use timed wait
						parkTime = IDLE_TIMEOUT * (If(t >= 0, 1, 1 - t))
						deadline = System.nanoTime() + parkTime - TIMEOUT_SLOP
					Else
							deadline = 0L
								parkTime = deadline
								prevctl = parkTime
					End If
					Dim wt As Thread = Thread.CurrentThread
					U.putObject(wt, PARKBLOCKER, Me) ' emulate LockSupport
					w.parker = wt
					If w.scanState < 0 AndAlso ctl = c Then ' recheck before park U.park(False, parkTime)
					U.putOrderedObject(w, QPARKER, Nothing)
					U.putObject(wt, PARKBLOCKER, Nothing)
					If w.scanState >= 0 Then break
					If parkTime <> 0L AndAlso ctl = c AndAlso deadline - System.nanoTime() <= 0L AndAlso U.compareAndSwapLong(Me, CTL, c, prevctl) Then Return False ' shrink pool
				End If
			Return True
		End Function

		' Joining tasks

		''' <summary>
		''' Tries to steal and run tasks within the target's computation.
		''' Uses a variant of the top-level algorithm, restricted to tasks
		''' with the given task as ancestor: It prefers taking and running
		''' eligible tasks popped from the worker's own queue (via
		''' popCC). Otherwise it scans others, randomly moving on
		''' contention or execution, deciding to give up based on a
		''' checksum (via return codes frob pollAndExecCC). The maxTasks
		''' argument supports external usages; internal calls use zero,
		''' allowing unbounded steps (external calls trap non-positive
		''' values).
		''' </summary>
		''' <param name="w"> caller </param>
		''' <param name="maxTasks"> if non-zero, the maximum number of other tasks to run </param>
		''' <returns> task status on exit </returns>
		Friend Function helpComplete(Of T1)(ByVal w As WorkQueue, ByVal task As CountedCompleter(Of T1), ByVal maxTasks As Integer) As Integer
			Dim ws As WorkQueue()
			Dim s As Integer = 0, m As Integer
			ws = workQueues
			m = ws.Length - 1
			If ws IsNot Nothing AndAlso m >= 0 AndAlso task IsNot Nothing AndAlso w IsNot Nothing Then
				Dim mode As Integer = w.config ' for popCC
				Dim r As Integer = w.hint Xor w.top ' arbitrary seed for origin
				Dim origin As Integer = r And m ' first queue to scan
				Dim h As Integer = 1 ' 1:ran, >1:contended, <0:hash
				Dim k As Integer = origin
				Dim oldSum As Integer = 0
				Dim checkSum As Integer = 0
				Do
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
					Dim p As CountedCompleter(Of ?)
					Dim q As WorkQueue
					s = task.status
					If s < 0 Then Exit Do
					p = w.popCC(task, mode)
					If h = 1 AndAlso p IsNot Nothing Then
						p.doExec() ' run local task
						maxTasks -= 1
						If maxTasks <> 0 AndAlso maxTasks = 0 Then Exit Do
						origin = k ' reset
							checkSum = 0
							oldSum = checkSum
					Else ' poll other queues
						q = ws(k)
						If q Is Nothing Then
							h = 0
						Else
							h = q.pollAndExecCC(task)
							If h < 0 Then checkSum += h
							End If
						If h > 0 Then
							maxTasks -= 1
							If h = 1 AndAlso maxTasks <> 0 AndAlso maxTasks = 0 Then Exit Do
							r = r Xor r << 13 ' xorshift
							r = r Xor CInt(CUInt(r) >> 17)
							r = r Xor r << 5
								k = r And m
								origin = k
								checkSum = 0
								oldSum = checkSum
						Else
							k = (k + 1) And m
							If k = origin Then
								oldSum = checkSum
								If oldSum = oldSum Then Exit Do
								checkSum = 0
							End If
							End If
					End If
				Loop
			End If
			Return s
		End Function

		''' <summary>
		''' Tries to locate and execute tasks for a stealer of the given
		''' task, or in turn one of its stealers, Traces currentSteal ->
		''' currentJoin links looking for a thread working on a descendant
		''' of the given task and with a non-empty queue to steal back and
		''' execute tasks from. The first call to this method upon a
		''' waiting join will often entail scanning/search, (which is OK
		''' because the joiner has nothing better to do), but this method
		''' leaves hints in workers to speed up subsequent calls.
		''' </summary>
		''' <param name="w"> caller </param>
		''' <param name="task"> the task to join </param>
		Private Sub helpStealer(Of T1)(ByVal w As WorkQueue, ByVal task As ForkJoinTask(Of T1))
			Dim ws As WorkQueue() = workQueues
			Dim oldSum As Integer = 0, checkSum As Integer, m As Integer
			m = ws.Length - 1
			If ws IsNot Nothing AndAlso m >= 0 AndAlso w IsNot Nothing AndAlso task IsNot Nothing Then
				Do ' restart point
					checkSum = 0 ' for stability check
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
					Dim subtask As ForkJoinTask(Of ?)
					Dim j As WorkQueue = w, v As WorkQueue ' v is subtask stealer
					descent:
					subtask = task
					Do While subtask.status >= 0
'JAVA TO VB CONVERTER TODO TASK: The following line could not be converted:
						for (int h = j.hint | 1, k = 0, i; ; k += 2)
							If k > m Then ' can't find stealer GoTo descent
'JAVA TO VB CONVERTER TODO TASK: Assignments within expressions are not supported in VB
							v = ws(i = (h + k) And m)
							If v IsNot Nothing Then
								If v.currentSteal Is subtask Then
									j.hint = i
									Exit Do
								End If
								checkSum += v.base
							End If
						Do ' help v or descend
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
							Dim a As ForkJoinTask(Of ?)()
							Dim b As Integer
'JAVA TO VB CONVERTER TODO TASK: Assignments within expressions are not supported in VB
							checkSum += (b = v.base)
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
							Dim [next] As ForkJoinTask(Of ?) = v.currentJoin
							If subtask.status < 0 OrElse j.currentJoin IsNot subtask OrElse v.currentSteal IsNot subtask Then ' stale GoTo descent
							a = v.array
							If b - v.top >= 0 OrElse a Is Nothing Then
								subtask = [next]
								If subtask Is Nothing Then GoTo descent
								j = v
								Exit Do
							End If
							Dim i As Integer = (((a.Length - 1) And b) << ASHIFT) + ABASE
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
							Dim t As ForkJoinTask(Of ?) = (CType(U.getObjectVolatile(a, i), ForkJoinTask(Of ?)))
							If v.base = b Then
								If t Is Nothing Then ' stale GoTo descent
								If U.compareAndSwapObject(a, i, t, Nothing) Then
									v.base = b + 1
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
									Dim ps As ForkJoinTask(Of ?) = w.currentSteal
									Dim top As Integer = w.top
									Do
										U.putOrderedObject(w, QCURRENTSTEAL, t)
										t.doExec() ' clear local tasks too
										t = w.pop()
									Loop While task.status >= 0 AndAlso w.top <> top AndAlso t IsNot Nothing
									U.putOrderedObject(w, QCURRENTSTEAL, ps)
									If w.base <> w.top Then Return ' can't further help
								End If
							End If
						Loop
					Loop
					oldSum = checkSum
				Loop While task.status >= 0 AndAlso oldSum <> oldSum
			End If
		End Sub

		''' <summary>
		''' Tries to decrement active count (sometimes implicitly) and
		''' possibly release or create a compensating worker in preparation
		''' for blocking. Returns false (retryable by caller), on
		''' contention, detected staleness, instability, or termination.
		''' </summary>
		''' <param name="w"> caller </param>
		Private Function tryCompensate(ByVal w As WorkQueue) As Boolean
			Dim canBlock As Boolean
			Dim ws As WorkQueue()
			Dim c As Long
			Dim m, pc, sp As Integer
			ws = workQueues
			m = ws.Length - 1
			pc = config And SMASK
			If w Is Nothing OrElse w.qlock < 0 OrElse ws Is Nothing OrElse m <= 0 OrElse pc = 0 Then ' parallelism disabled -  caller terminating
				canBlock = False
			Else
'JAVA TO VB CONVERTER TODO TASK: Assignments within expressions are not supported in VB
				sp = CInt(Fix(c = ctl))
				If sp <> 0 Then ' release idle worker
					canBlock = tryRelease(c, ws(sp And m), 0L)
				Else
					Dim ac As Integer = CInt(Fix(c >> AC_SHIFT)) + pc
				End If
				Dim tc As Integer = CShort(Fix(c >> TC_SHIFT)) + pc
				Dim nbusy As Integer = 0 ' validate saturation
				For i As Integer = 0 To m ' two passes of odd indices
					Dim v As WorkQueue
					v = ws(((i << 1) Or 1) And m)
					If v IsNot Nothing Then
						If (v.scanState And SCANNING) <> 0 Then Exit For
						nbusy += 1
					End If
				Next i
				If nbusy <> (tc << 1) OrElse ctl <> c Then
					canBlock = False ' unstable or stale
				ElseIf tc >= pc AndAlso ac > 1 AndAlso w.empty Then
					Dim nc As Long = ((AC_MASK And (c - AC_UNIT)) Or ((Not AC_MASK) And c)) ' uncompensated
					canBlock = U.compareAndSwapLong(Me, CTL, c, nc)
				ElseIf tc >= MAX_CAP OrElse (Me Is common AndAlso tc >= pc + commonMaxSpares) Then
					Throw New java.util.concurrent.RejectedExecutionException("Thread limit exceeded replacing blocked worker")
				Else ' similar to tryAddWorker
					Dim add As Boolean = False ' CAS within lock
					Dim rs As Integer
					Dim nc As Long = ((AC_MASK And c) Or (TC_MASK And (c + TC_UNIT)))
					rs = lockRunState()
					If (rs And [STOP]) = 0 Then add = U.compareAndSwapLong(Me, CTL, c, nc)
					unlockRunState(rs, rs And (Not RSLOCK))
					canBlock = add AndAlso createWorker() ' throws on exception
				End If
				End If
			Return canBlock
		End Function

		''' <summary>
		''' Helps and/or blocks until the given task is done or timeout.
		''' </summary>
		''' <param name="w"> caller </param>
		''' <param name="task"> the task </param>
		''' <param name="deadline"> for timed waits, if nonzero </param>
		''' <returns> task status on exit </returns>
		Friend Function awaitJoin(Of T1)(ByVal w As WorkQueue, ByVal task As ForkJoinTask(Of T1), ByVal deadline As Long) As Integer
			Dim s As Integer = 0
			If task IsNot Nothing AndAlso w IsNot Nothing Then
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
				Dim prevJoin As ForkJoinTask(Of ?) = w.currentJoin
				U.putOrderedObject(w, QCURRENTJOIN, task)
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
				Dim cc As CountedCompleter(Of ?) = If(TypeOf task Is CountedCompleter, CType(task, CountedCompleter(Of ?)), Nothing)
				Do
					s = task.status
					If s < 0 Then Exit Do
					If cc IsNot Nothing Then
						helpComplete(w, cc, 0)
					ElseIf w.base = w.top OrElse w.tryRemoveAndExec(task) Then
						helpStealer(w, task)
					End If
					s = task.status
					If s < 0 Then Exit Do
					Dim ms, ns As Long
					If deadline = 0L Then
						ms = 0L
					Else
						ns = deadline - System.nanoTime()
						If ns <= 0L Then
							Exit Do
						Else
							ms = java.util.concurrent.TimeUnit.NANOSECONDS.toMillis(ns)
							If ms <= 0L Then ms = 1L
							End If
						End If
					If tryCompensate(w) Then
						task.internalWait(ms)
						U.getAndAddLong(Me, CTL, AC_UNIT)
					End If
				Loop
				U.putOrderedObject(w, QCURRENTJOIN, prevJoin)
			End If
			Return s
		End Function

		' Specialized scanning

		''' <summary>
		''' Returns a (probably) non-empty steal queue, if one is found
		''' during a scan, else null.  This method must be retried by
		''' caller if, by the time it tries to use the queue, it is empty.
		''' </summary>
		Private Function findNonEmptyStealQueue() As WorkQueue
			Dim ws As WorkQueue() ' one-shot version of scan loop
			Dim m As Integer
			Dim r As Integer = java.util.concurrent.ThreadLocalRandom.nextSecondarySeed()
			ws = workQueues
			m = ws.Length - 1
			If ws IsNot Nothing AndAlso m >= 0 Then
				Dim origin As Integer = r And m
				Dim k As Integer = origin
				Dim oldSum As Integer = 0
				Dim checkSum As Integer = 0
				Do
					Dim q As WorkQueue
					Dim b As Integer
					q = ws(k)
					If q IsNot Nothing Then
						b = q.base
						If b - q.top < 0 Then Return q
						checkSum += b
					End If
					k = (k + 1) And m
					If k = origin Then
						oldSum = checkSum
						If oldSum = oldSum Then Exit Do
						checkSum = 0
					End If
				Loop
			End If
			Return Nothing
		End Function

		''' <summary>
		''' Runs tasks until {@code isQuiescent()}. We piggyback on
		''' active count ctl maintenance, but rather than blocking
		''' when tasks cannot be found, we rescan until all others cannot
		''' find tasks either.
		''' </summary>
		Friend Sub helpQuiescePool(ByVal w As WorkQueue)
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
			Dim ps As ForkJoinTask(Of ?) = w.currentSteal ' save context
			Dim active As Boolean = True
			Do
				Dim c As Long
				Dim q As WorkQueue
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
				Dim t As ForkJoinTask(Of ?)
				Dim b As Integer
				w.execLocalTasks() ' run locals before each scan
				q = findNonEmptyStealQueue()
				If q IsNot Nothing Then
					If Not active Then ' re-establish active count
						active = True
						U.getAndAddLong(Me, CTL, AC_UNIT)
					End If
					b = q.base
					t = q.pollAt(b)
					If b - q.top < 0 AndAlso t IsNot Nothing Then
						U.putOrderedObject(w, QCURRENTSTEAL, t)
						t.doExec()
						w.nsteals += 1
						If w.nsteals < 0 Then w.transferStealCount(Me)
					End If
				ElseIf active Then ' decrement active count without queuing
'JAVA TO VB CONVERTER TODO TASK: Assignments within expressions are not supported in VB
					Dim nc As Long = (AC_MASK And ((c = ctl) - AC_UNIT)) Or ((Not AC_MASK) And c)
					If CInt(Fix(nc >> AC_SHIFT)) + (config And SMASK) <= 0 Then Exit Do ' bypass decrement-then-increment
					If U.compareAndSwapLong(Me, CTL, c, nc) Then active = False
				Else
					c = ctl
					If CInt(Fix(c >> AC_SHIFT)) + (config And SMASK) <= 0 AndAlso U.compareAndSwapLong(Me, CTL, c, c + AC_UNIT) Then Exit Do
					End If
			Loop
			U.putOrderedObject(w, QCURRENTSTEAL, ps)
		End Sub

		''' <summary>
		''' Gets and removes a local or stolen task for the given worker.
		''' </summary>
		''' <returns> a task, if available </returns>
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
		Friend Function nextTaskFor(ByVal w As WorkQueue) As ForkJoinTask(Of ?)
			Dim t As ForkJoinTask(Of ?)
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
			Do
				Dim q As WorkQueue
				Dim b As Integer
				t = w.nextLocalTask()
				If t IsNot Nothing Then Return t
				q = findNonEmptyStealQueue()
				If q Is Nothing Then Return Nothing
				b = q.base
				t = q.pollAt(b)
				If b - q.top < 0 AndAlso t IsNot Nothing Then Return t
			Loop
		End Function

		''' <summary>
		''' Returns a cheap heuristic guide for task partitioning when
		''' programmers, frameworks, tools, or languages have little or no
		''' idea about task granularity.  In essence, by offering this
		''' method, we ask users only about tradeoffs in overhead vs
		''' expected throughput and its variance, rather than how finely to
		''' partition tasks.
		''' 
		''' In a steady state strict (tree-structured) computation, each
		''' thread makes available for stealing enough tasks for other
		''' threads to remain active. Inductively, if all threads play by
		''' the same rules, each thread should make available only a
		''' constant number of tasks.
		''' 
		''' The minimum useful constant is just 1. But using a value of 1
		''' would require immediate replenishment upon each steal to
		''' maintain enough tasks, which is infeasible.  Further,
		''' partitionings/granularities of offered tasks should minimize
		''' steal rates, which in general means that threads nearer the top
		''' of computation tree should generate more than those nearer the
		''' bottom. In perfect steady state, each thread is at
		''' approximately the same level of computation tree. However,
		''' producing extra tasks amortizes the uncertainty of progress and
		''' diffusion assumptions.
		''' 
		''' So, users will want to use values larger (but not much larger)
		''' than 1 to both smooth over transient shortages and hedge
		''' against uneven progress; as traded off against the cost of
		''' extra task overhead. We leave the user to pick a threshold
		''' value to compare with the results of this call to guide
		''' decisions, but recommend values such as 3.
		''' 
		''' When all threads are active, it is on average OK to estimate
		''' surplus strictly locally. In steady-state, if one thread is
		''' maintaining say 2 surplus tasks, then so are others. So we can
		''' just use estimated queue length.  However, this strategy alone
		''' leads to serious mis-estimates in some non-steady-state
		''' conditions (ramp-up, ramp-down, other stalls). We can detect
		''' many of these by further considering the number of "idle"
		''' threads, that are known to have zero queued tasks, so
		''' compensate by a factor of (#idle/#active) threads.
		''' </summary>
		Friend Property Shared surplusQueuedTaskCount As Integer
			Get
				Dim t As Thread
				Dim wt As ForkJoinWorkerThread
				Dim pool As ForkJoinPool
				Dim q As WorkQueue
				t = Thread.CurrentThread
				If (TypeOf t Is ForkJoinWorkerThread) Then
	'JAVA TO VB CONVERTER TODO TASK: Assignments within expressions are not supported in VB
					Dim p As Integer = (pool = (wt = CType(t, ForkJoinWorkerThread)).pool).config And SMASK
	'JAVA TO VB CONVERTER TODO TASK: Assignments within expressions are not supported in VB
					Dim n As Integer = (q = wt.workQueue).top - q.base
					Dim a As Integer = CInt(Fix(pool.ctl >> AC_SHIFT)) + p
					Return n - (If(a > (p >>>= 1), 0, If(a > (p >>>= 1), 1, If(a > (p >>>= 1), 2, If(a > (p >>>= 1), 4, 8)))))
				End If
				Return 0
			End Get
		End Property

		'  Termination

		''' <summary>
		''' Possibly initiates and/or completes termination.
		''' </summary>
		''' <param name="now"> if true, unconditionally terminate, else only
		''' if no work and no active workers </param>
		''' <param name="enable"> if true, enable shutdown when next possible </param>
		''' <returns> true if now terminating or terminated </returns>
		Private Function tryTerminate(ByVal now As Boolean, ByVal enable As Boolean) As Boolean
			Dim rs As Integer
			If Me Is common Then ' cannot shut down Return False
			rs = runState
			If rs >= 0 Then
				If Not enable Then Return False
				rs = lockRunState() ' enter SHUTDOWN phase
				unlockRunState(rs, (rs And (Not RSLOCK)) Or SHUTDOWN_Renamed)
			End If

			If (rs And [STOP]) = 0 Then
				If Not now Then ' check quiescence
					Dim oldSum As Long = 0L
					Do ' repeat until stable
						Dim ws As WorkQueue()
						Dim w As WorkQueue
						Dim m, b As Integer
						Dim c As Long
						Dim checkSum As Long = ctl
						If CInt(Fix(checkSum >> AC_SHIFT)) + (config And SMASK) > 0 Then Return False ' still active workers
						ws = workQueues
						m = ws.Length - 1
						If ws Is Nothing OrElse m <= 0 Then Exit Do ' check queues
						For i As Integer = 0 To m
							w = ws(i)
							If w IsNot Nothing Then
								b = w.base
								If b <> w.top OrElse w.scanState >= 0 OrElse w.currentSteal IsNot Nothing Then
'JAVA TO VB CONVERTER TODO TASK: Assignments within expressions are not supported in VB
									tryRelease(c = ctl, ws(m And CInt(c)), AC_UNIT)
									Return False ' arrange for recheck
								End If
								checkSum += b
								If (i And 1) = 0 Then w.qlock = -1 ' try to disable external
							End If
						Next i
						oldSum = checkSum
						If oldSum = oldSum Then Exit Do
					Loop
				End If
				If (runState And [STOP]) = 0 Then
					rs = lockRunState() ' enter STOP phase
					unlockRunState(rs, (rs And (Not RSLOCK)) Or [STOP])
				End If
			End If

			Dim pass As Integer = 0 ' 3 passes to help terminate
			Dim oldSum As Long = 0L
			Do ' or until done or stable
				Dim ws As WorkQueue()
				Dim w As WorkQueue
				Dim wt As ForkJoinWorkerThread
				Dim m As Integer
				Dim checkSum As Long = ctl
				ws = workQueues
				m = ws.Length - 1
				If CShort(CLng(CULng(checkSum) >> TC_SHIFT)) + (config And SMASK) <= 0 OrElse ws Is Nothing OrElse m <= 0 Then
					If (runState And TERMINATED) = 0 Then
						rs = lockRunState() ' done
						unlockRunState(rs, (rs And (Not RSLOCK)) Or TERMINATED)
						SyncLock Me ' for awaitTermination
							notifyAll()
						End SyncLock
					End If
					Exit Do
				End If
				For i As Integer = 0 To m
					w = ws(i)
					If w IsNot Nothing Then
						checkSum += w.base
						w.qlock = -1 ' try to disable
						If pass > 0 Then
							w.cancelAll() ' clear queue
							wt = w.owner
							If pass > 1 AndAlso wt IsNot Nothing Then
								If Not wt.interrupted Then
									Try ' unblock join
										wt.interrupt()
									Catch ignore As Throwable
									End Try
								End If
								If w.scanState < 0 Then U.unpark(wt) ' wake up
							End If
						End If
					End If
				Next i
				If checkSum <> oldSum Then ' unstable
					oldSum = checkSum
					pass = 0
				ElseIf pass > 3 AndAlso pass > m Then ' can't further help
					Exit Do
				Else
					pass += 1
					If pass > 1 Then ' try to dequeue
						Dim c As Long ' bound attempts
						Dim j As Integer = 0, sp As Integer
'JAVA TO VB CONVERTER TODO TASK: Assignments within expressions are not supported in VB
						Dim tempVar As Boolean = j <= m AndAlso (sp = CInt(Fix(c = ctl))) <> 0
						j += 1
						Do While tempVar
							tryRelease(c, ws(sp And m), AC_UNIT)
							tempVar = j <= m AndAlso (sp = CInt(Fix(c = ctl))) <> 0
							j += 1
						Loop
					End If
					End If
			Loop
			Return True
		End Function

		' External operations

		''' <summary>
		''' Full version of externalPush, handling uncommon cases, as well
		''' as performing secondary initialization upon the first
		''' submission of the first task to the pool.  It also detects
		''' first submission by an external thread and creates a new shared
		''' queue if the one at index if empty or contended.
		''' </summary>
		''' <param name="task"> the task. Caller must ensure non-null. </param>
		Private Sub externalSubmit(Of T1)(ByVal task As ForkJoinTask(Of T1))
			Dim r As Integer ' initialize caller's probe
			r = java.util.concurrent.ThreadLocalRandom.probe
			If r = 0 Then
				java.util.concurrent.ThreadLocalRandom.localInit()
				r = java.util.concurrent.ThreadLocalRandom.probe
			End If
			Do
				Dim ws As WorkQueue()
				Dim q As WorkQueue
				Dim rs, m, k As Integer
				Dim move As Boolean = False
				rs = runState
				If rs < 0 Then
					tryTerminate(False, False) ' help terminate
					Throw New java.util.concurrent.RejectedExecutionException
				Else
					ws = workQueues
					m = ws.Length - 1
					If (rs And STARTED) = 0 OrElse (ws Is Nothing OrElse m < 0) Then ' initialize
						Dim ns As Integer = 0
						rs = lockRunState()
						Try
							If (rs And STARTED) = 0 Then
								U.compareAndSwapObject(Me, STEALCOUNTER, Nothing, New java.util.concurrent.atomic.AtomicLong)
								' create workQueues array with size a power of two
								Dim p As Integer = config And SMASK ' ensure at least 2 slots
								Dim n As Integer = If(p > 1, p - 1, 1)
								n = n Or CInt(CUInt(n) >> 1)
								n = n Or CInt(CUInt(n) >> 2)
								n = n Or CInt(CUInt(n) >> 4)
								n = n Or CInt(CUInt(n) >> 8)
								n = n Or CInt(CUInt(n) >> 16)
								n = (n + 1) << 1
								workQueues = New WorkQueue(n - 1){}
								ns = STARTED
							End If
						Finally
							unlockRunState(rs, (rs And (Not RSLOCK)) Or ns)
						End Try
					Else
'JAVA TO VB CONVERTER TODO TASK: Assignments within expressions are not supported in VB
						q = ws(k = r And m And SQMASK)
						If q IsNot Nothing Then
							If q.qlock = 0 AndAlso U.compareAndSwapInt(q, QLOCK, 0, 1) Then
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
								Dim a As ForkJoinTask(Of ?)() = q.array
								Dim s As Integer = q.top
								Dim submitted As Boolean = False ' initial submission or resizing
								Try ' locked version of push
									a = q.growArray()
									If (a IsNot Nothing AndAlso a.Length > s + 1 - q.base) OrElse a IsNot Nothing Then
										Dim j As Integer = (((a.Length - 1) And s) << ASHIFT) + ABASE
										U.putOrderedObject(a, j, task)
										U.putOrderedInt(q, QTOP, s + 1)
										submitted = True
									End If
								Finally
									U.compareAndSwapInt(q, QLOCK, 1, 0)
								End Try
								If submitted Then
									signalWork(ws, q)
									Return
								End If
							End If
							move = True ' move on failure
						Else
							rs = runState
							If (rs And RSLOCK) = 0 Then ' create new queue
								q = New WorkQueue(Me, Nothing)
								q.hint = r
								q.config = k Or SHARED_QUEUE
								q.scanState = INACTIVE
								rs = lockRunState() ' publish index
								ws = workQueues
								If rs > 0 AndAlso ws IsNot Nothing AndAlso k < ws.Length AndAlso ws(k) Is Nothing Then ws(k) = q ' else terminated
								unlockRunState(rs, rs And (Not RSLOCK))
							Else
								move = True ' move if busy
							End If
							End If
						End If
					End If
				If move Then r = java.util.concurrent.ThreadLocalRandom.advanceProbe(r)
			Loop
		End Sub

		''' <summary>
		''' Tries to add the given task to a submission queue at
		''' submitter's current queue. Only the (vastly) most common path
		''' is directly handled in this method, while screening for need
		''' for externalSubmit.
		''' </summary>
		''' <param name="task"> the task. Caller must ensure non-null. </param>
		Friend Sub externalPush(Of T1)(ByVal task As ForkJoinTask(Of T1))
			Dim ws As WorkQueue()
			Dim q As WorkQueue
			Dim m As Integer
			Dim r As Integer = java.util.concurrent.ThreadLocalRandom.probe
			Dim rs As Integer = runState
			ws = workQueues
			m = (ws.Length - 1)
			q = ws(m And r And SQMASK)
			If ws IsNot Nothing AndAlso m >= 0 AndAlso q IsNot Nothing AndAlso r <> 0 AndAlso rs > 0 AndAlso U.compareAndSwapInt(q, QLOCK, 0, 1) Then
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
				Dim a As ForkJoinTask(Of ?)()
				Dim am, n, s As Integer
				a = q.array
				am = a.Length - 1
'JAVA TO VB CONVERTER TODO TASK: Assignments within expressions are not supported in VB
				n = (s = q.top) - q.base
				If a IsNot Nothing AndAlso am > n Then
					Dim j As Integer = ((am And s) << ASHIFT) + ABASE
					U.putOrderedObject(a, j, task)
					U.putOrderedInt(q, QTOP, s + 1)
					U.putIntVolatile(q, QLOCK, 0)
					If n <= 1 Then signalWork(ws, q)
					Return
				End If
				U.compareAndSwapInt(q, QLOCK, 1, 0)
			End If
			externalSubmit(task)
		End Sub

		''' <summary>
		''' Returns common pool queue for an external thread.
		''' </summary>
		Friend Shared Function commonSubmitterQueue() As WorkQueue
			Dim p As ForkJoinPool = common
			Dim r As Integer = java.util.concurrent.ThreadLocalRandom.probe
			Dim ws As WorkQueue()
			Dim m As Integer
'JAVA TO VB CONVERTER TODO TASK: Assignments within expressions are not supported in VB
			Return If(p IsNot Nothing AndAlso (ws = p.workQueues) IsNot Nothing AndAlso (m = ws.Length - 1) >= 0, ws(m And r And SQMASK), Nothing)
		End Function

		''' <summary>
		''' Performs tryUnpush for an external submitter: Finds queue,
		''' locks if apparently non-empty, validates upon locking, and
		''' adjusts top. Each check can fail but rarely does.
		''' </summary>
		Friend Function tryExternalUnpush(Of T1)(ByVal task As ForkJoinTask(Of T1)) As Boolean
			Dim ws As WorkQueue()
			Dim w As WorkQueue
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
			Dim a As ForkJoinTask(Of ?)()
			Dim m, s As Integer
			Dim r As Integer = java.util.concurrent.ThreadLocalRandom.probe
			ws = workQueues
			m = ws.Length - 1
			w = ws(m And r And SQMASK)
			a = w.array
			s = w.top
			If ws IsNot Nothing AndAlso m >= 0 AndAlso w IsNot Nothing AndAlso a IsNot Nothing AndAlso s <> w.base Then
				Dim j As Long = (((a.Length - 1) And (s - 1)) << ASHIFT) + ABASE
				If U.compareAndSwapInt(w, QLOCK, 0, 1) Then
					If w.top = s AndAlso w.array = a AndAlso U.getObject(a, j) Is task AndAlso U.compareAndSwapObject(a, j, task, Nothing) Then
						U.putOrderedInt(w, QTOP, s - 1)
						U.putOrderedInt(w, QLOCK, 0)
						Return True
					End If
					U.compareAndSwapInt(w, QLOCK, 1, 0)
				End If
			End If
			Return False
		End Function

		''' <summary>
		''' Performs helpComplete for an external submitter.
		''' </summary>
		Friend Function externalHelpComplete(Of T1)(ByVal task As CountedCompleter(Of T1), ByVal maxTasks As Integer) As Integer
			Dim ws As WorkQueue()
			Dim n As Integer
			Dim r As Integer = java.util.concurrent.ThreadLocalRandom.probe
'JAVA TO VB CONVERTER TODO TASK: Assignments within expressions are not supported in VB
			Return If((ws = workQueues) Is Nothing OrElse (n = ws.Length) = 0, 0, helpComplete(ws((n - 1) And r And SQMASK), task, maxTasks))
		End Function

		' Exported methods

		' Constructors

		''' <summary>
		''' Creates a {@code ForkJoinPool} with parallelism equal to {@link
		''' java.lang.Runtime#availableProcessors}, using the {@linkplain
		''' #defaultForkJoinWorkerThreadFactory default thread factory},
		''' no UncaughtExceptionHandler, and non-async LIFO processing mode.
		''' </summary>
		''' <exception cref="SecurityException"> if a security manager exists and
		'''         the caller is not permitted to modify threads
		'''         because it does not hold {@link
		'''         java.lang.RuntimePermission}{@code ("modifyThread")} </exception>
		Public Sub New()
			Me.New (System.Math.Min(MAX_CAP, Runtime.runtime.availableProcessors()), defaultForkJoinWorkerThreadFactory, Nothing, False)
		End Sub

		''' <summary>
		''' Creates a {@code ForkJoinPool} with the indicated parallelism
		''' level, the {@linkplain
		''' #defaultForkJoinWorkerThreadFactory default thread factory},
		''' no UncaughtExceptionHandler, and non-async LIFO processing mode.
		''' </summary>
		''' <param name="parallelism"> the parallelism level </param>
		''' <exception cref="IllegalArgumentException"> if parallelism less than or
		'''         equal to zero, or greater than implementation limit </exception>
		''' <exception cref="SecurityException"> if a security manager exists and
		'''         the caller is not permitted to modify threads
		'''         because it does not hold {@link
		'''         java.lang.RuntimePermission}{@code ("modifyThread")} </exception>
		Public Sub New(ByVal parallelism As Integer)
			Me.New(parallelism, defaultForkJoinWorkerThreadFactory, Nothing, False)
		End Sub

		''' <summary>
		''' Creates a {@code ForkJoinPool} with the given parameters.
		''' </summary>
		''' <param name="parallelism"> the parallelism level. For default value,
		''' use <seealso cref="java.lang.Runtime#availableProcessors"/>. </param>
		''' <param name="factory"> the factory for creating new threads. For default value,
		''' use <seealso cref="#defaultForkJoinWorkerThreadFactory"/>. </param>
		''' <param name="handler"> the handler for internal worker threads that
		''' terminate due to unrecoverable errors encountered while executing
		''' tasks. For default value, use {@code null}. </param>
		''' <param name="asyncMode"> if true,
		''' establishes local first-in-first-out scheduling mode for forked
		''' tasks that are never joined. This mode may be more appropriate
		''' than default locally stack-based mode in applications in which
		''' worker threads only process event-style asynchronous tasks.
		''' For default value, use {@code false}. </param>
		''' <exception cref="IllegalArgumentException"> if parallelism less than or
		'''         equal to zero, or greater than implementation limit </exception>
		''' <exception cref="NullPointerException"> if the factory is null </exception>
		''' <exception cref="SecurityException"> if a security manager exists and
		'''         the caller is not permitted to modify threads
		'''         because it does not hold {@link
		'''         java.lang.RuntimePermission}{@code ("modifyThread")} </exception>
		Public Sub New(ByVal parallelism As Integer, ByVal factory As ForkJoinWorkerThreadFactory, ByVal handler As Thread.UncaughtExceptionHandler, ByVal asyncMode As Boolean)
			Me.New(checkParallelism(parallelism), checkFactory(factory), handler,If(asyncMode, FIFO_QUEUE, LIFO_QUEUE), "ForkJoinPool-" & nextPoolId() & "-worker-")
			checkPermission()
		End Sub

		Private Shared Function checkParallelism(ByVal parallelism As Integer) As Integer
			If parallelism <= 0 OrElse parallelism > MAX_CAP Then Throw New IllegalArgumentException
			Return parallelism
		End Function

		Private Shared Function checkFactory(ByVal factory As ForkJoinWorkerThreadFactory) As ForkJoinWorkerThreadFactory
			If factory Is Nothing Then Throw New NullPointerException
			Return factory
		End Function

		''' <summary>
		''' Creates a {@code ForkJoinPool} with the given parameters, without
		''' any security checks or parameter validation.  Invoked directly by
		''' makeCommonPool.
		''' </summary>
		Private Sub New(ByVal parallelism As Integer, ByVal factory As ForkJoinWorkerThreadFactory, ByVal handler As Thread.UncaughtExceptionHandler, ByVal mode As Integer, ByVal workerNamePrefix As String)
			Me.workerNamePrefix = workerNamePrefix
			Me.factory = factory
			Me.ueh = handler
			Me.config = (parallelism And SMASK) Or mode
			Dim np As Long = CLng(Fix(-parallelism)) ' offset ctl counts
			Me.ctl = ((np << AC_SHIFT) And AC_MASK) Or ((np << TC_SHIFT) And TC_MASK)
		End Sub

		''' <summary>
		''' Returns the common pool instance. This pool is statically
		''' constructed; its run state is unaffected by attempts to {@link
		''' #shutdown} or <seealso cref="#shutdownNow"/>. However this pool and any
		''' ongoing processing are automatically terminated upon program
		''' <seealso cref="System#exit"/>.  Any program that relies on asynchronous
		''' task processing to complete before program termination should
		''' invoke {@code commonPool().}<seealso cref="#awaitQuiescence awaitQuiescence"/>,
		''' before exit.
		''' </summary>
		''' <returns> the common pool instance
		''' @since 1.8 </returns>
		Public Shared Function commonPool() As ForkJoinPool
			' assert common != null : "static init error";
			Return common
		End Function

		' Execution methods

		''' <summary>
		''' Performs the given task, returning its result upon completion.
		''' If the computation encounters an unchecked Exception or Error,
		''' it is rethrown as the outcome of this invocation.  Rethrown
		''' exceptions behave in the same way as regular exceptions, but,
		''' when possible, contain stack traces (as displayed for example
		''' using {@code ex.printStackTrace()}) of both the current thread
		''' as well as the thread actually encountering the exception;
		''' minimally only the latter.
		''' </summary>
		''' <param name="task"> the task </param>
		''' @param <T> the type of the task's result </param>
		''' <returns> the task's result </returns>
		''' <exception cref="NullPointerException"> if the task is null </exception>
		''' <exception cref="RejectedExecutionException"> if the task cannot be
		'''         scheduled for execution </exception>
		Public Overridable Function invoke(Of T)(ByVal task As ForkJoinTask(Of T)) As T
			If task Is Nothing Then Throw New NullPointerException
			externalPush(task)
			Return task.join()
		End Function

		''' <summary>
		''' Arranges for (asynchronous) execution of the given task.
		''' </summary>
		''' <param name="task"> the task </param>
		''' <exception cref="NullPointerException"> if the task is null </exception>
		''' <exception cref="RejectedExecutionException"> if the task cannot be
		'''         scheduled for execution </exception>
		Public Overridable Sub execute(Of T1)(ByVal task As ForkJoinTask(Of T1))
			If task Is Nothing Then Throw New NullPointerException
			externalPush(task)
		End Sub

		' AbstractExecutorService methods

		''' <exception cref="NullPointerException"> if the task is null </exception>
		''' <exception cref="RejectedExecutionException"> if the task cannot be
		'''         scheduled for execution </exception>
		Public Overridable Sub execute(ByVal task As Runnable)
			If task Is Nothing Then Throw New NullPointerException
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
			Dim job As ForkJoinTask(Of ?)
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
			If TypeOf task Is ForkJoinTask(Of ?) Then ' avoid re-wrap
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
				job = CType(task, ForkJoinTask(Of ?))
			Else
				job = New ForkJoinTask.RunnableExecuteAction(task)
			End If
			externalPush(job)
		End Sub

		''' <summary>
		''' Submits a ForkJoinTask for execution.
		''' </summary>
		''' <param name="task"> the task to submit </param>
		''' @param <T> the type of the task's result </param>
		''' <returns> the task </returns>
		''' <exception cref="NullPointerException"> if the task is null </exception>
		''' <exception cref="RejectedExecutionException"> if the task cannot be
		'''         scheduled for execution </exception>
		Public Overridable Function submit(Of T)(ByVal task As ForkJoinTask(Of T)) As ForkJoinTask(Of T)
			If task Is Nothing Then Throw New NullPointerException
			externalPush(task)
			Return task
		End Function

		''' <exception cref="NullPointerException"> if the task is null </exception>
		''' <exception cref="RejectedExecutionException"> if the task cannot be
		'''         scheduled for execution </exception>
		Public Overridable Function submit(Of T)(ByVal task As java.util.concurrent.Callable(Of T)) As ForkJoinTask(Of T)
			Dim job As ForkJoinTask(Of T) = New ForkJoinTask.AdaptedCallable(Of T)(task)
			externalPush(job)
			Return job
		End Function

		''' <exception cref="NullPointerException"> if the task is null </exception>
		''' <exception cref="RejectedExecutionException"> if the task cannot be
		'''         scheduled for execution </exception>
		Public Overrides Function submit(Of T)(ByVal task As Runnable, ByVal result As T) As ForkJoinTask(Of T)
			Dim job As ForkJoinTask(Of T) = New ForkJoinTask.AdaptedRunnable(Of T)(task, result)
			externalPush(job)
			Return job
		End Function

		''' <exception cref="NullPointerException"> if the task is null </exception>
		''' <exception cref="RejectedExecutionException"> if the task cannot be
		'''         scheduled for execution </exception>
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
		Public Overrides Function submit(ByVal task As Runnable) As ForkJoinTask(Of ?)
			If task Is Nothing Then Throw New NullPointerException
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
			Dim job As ForkJoinTask(Of ?)
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
			If TypeOf task Is ForkJoinTask(Of ?) Then ' avoid re-wrap
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
				job = CType(task, ForkJoinTask(Of ?))
			Else
				job = New ForkJoinTask.AdaptedRunnableAction(task)
			End If
			externalPush(job)
			Return job
		End Function

		''' <exception cref="NullPointerException">       {@inheritDoc} </exception>
		''' <exception cref="RejectedExecutionException"> {@inheritDoc} </exception>
		Public Overrides Function invokeAll(Of T, T1 As java.util.concurrent.Callable(Of T)(ByVal tasks As ICollection(Of T1)) As IList(Of java.util.concurrent.Future(Of T))
			' In previous versions of this [Class], this method constructed
			' a task to run ForkJoinTask.invokeAll, but now external
			' invocation of multiple tasks is at least as efficient.
			Dim futures As New List(Of java.util.concurrent.Future(Of T))(tasks.Count)

			Dim done As Boolean = False
			Try
				For Each t As java.util.concurrent.Callable(Of T) In tasks
					Dim f As ForkJoinTask(Of T) = New ForkJoinTask.AdaptedCallable(Of T)(t)
					futures.add(f)
					externalPush(f)
				Next t
				Dim i As Integer = 0
				Dim size As Integer = futures.size()
				Do While i < size
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
					CType(futures.get(i), ForkJoinTask(Of ?)).quietlyJoin()
					i += 1
				Loop
				done = True
				Return futures
			Finally
				If Not done Then
					Dim i As Integer = 0
					Dim size As Integer = futures.size()
					Do While i < size
						futures.get(i).cancel(False)
						i += 1
					Loop
				End If
			End Try
		End Function

		''' <summary>
		''' Returns the factory used for constructing new workers.
		''' </summary>
		''' <returns> the factory used for constructing new workers </returns>
		Public Overridable Property factory As ForkJoinWorkerThreadFactory
			Get
				Return factory
			End Get
		End Property

		''' <summary>
		''' Returns the handler for internal worker threads that terminate
		''' due to unrecoverable errors encountered while executing tasks.
		''' </summary>
		''' <returns> the handler, or {@code null} if none </returns>
		Public Overridable Property uncaughtExceptionHandler As Thread.UncaughtExceptionHandler
			Get
				Return ueh
			End Get
		End Property

		''' <summary>
		''' Returns the targeted parallelism level of this pool.
		''' </summary>
		''' <returns> the targeted parallelism level of this pool </returns>
		Public Overridable Property parallelism As Integer
			Get
				Dim par As Integer
	'JAVA TO VB CONVERTER TODO TASK: Assignments within expressions are not supported in VB
				Return If((par = config And SMASK) > 0, par, 1)
			End Get
		End Property

		''' <summary>
		''' Returns the targeted parallelism level of the common pool.
		''' </summary>
		''' <returns> the targeted parallelism level of the common pool
		''' @since 1.8 </returns>
		Public Property Shared commonPoolParallelism As Integer
			Get
				Return commonParallelism
			End Get
		End Property

		''' <summary>
		''' Returns the number of worker threads that have started but not
		''' yet terminated.  The result returned by this method may differ
		''' from <seealso cref="#getParallelism"/> when threads are created to
		''' maintain parallelism when others are cooperatively blocked.
		''' </summary>
		''' <returns> the number of worker threads </returns>
		Public Overridable Property poolSize As Integer
			Get
				Return (config And SMASK) + CShort(CLng(CULng(ctl) >> TC_SHIFT))
			End Get
		End Property

		''' <summary>
		''' Returns {@code true} if this pool uses local first-in-first-out
		''' scheduling mode for forked tasks that are never joined.
		''' </summary>
		''' <returns> {@code true} if this pool uses async mode </returns>
		Public Overridable Property asyncMode As Boolean
			Get
				Return (config And FIFO_QUEUE) <> 0
			End Get
		End Property

		''' <summary>
		''' Returns an estimate of the number of worker threads that are
		''' not blocked waiting to join tasks or for other managed
		''' synchronization. This method may overestimate the
		''' number of running threads.
		''' </summary>
		''' <returns> the number of worker threads </returns>
		Public Overridable Property runningThreadCount As Integer
			Get
				Dim rc As Integer = 0
				Dim ws As WorkQueue()
				Dim w As WorkQueue
				ws = workQueues
				If ws IsNot Nothing Then
					For i As Integer = 1 To ws.Length - 1 Step 2
						w = ws(i)
						If w IsNot Nothing AndAlso w.apparentlyUnblocked Then rc += 1
					Next i
				End If
				Return rc
			End Get
		End Property

		''' <summary>
		''' Returns an estimate of the number of threads that are currently
		''' stealing or executing tasks. This method may overestimate the
		''' number of active threads.
		''' </summary>
		''' <returns> the number of active threads </returns>
		Public Overridable Property activeThreadCount As Integer
			Get
				Dim r As Integer = (config And SMASK) + CInt(Fix(ctl >> AC_SHIFT))
				Return If(r <= 0, 0, r) ' suppress momentarily negative values
			End Get
		End Property

		''' <summary>
		''' Returns {@code true} if all worker threads are currently idle.
		''' An idle worker is one that cannot obtain a task to execute
		''' because none are available to steal from other threads, and
		''' there are no pending submissions to the pool. This method is
		''' conservative; it might not return {@code true} immediately upon
		''' idleness of all threads, but will eventually become true if
		''' threads remain inactive.
		''' </summary>
		''' <returns> {@code true} if all threads are currently idle </returns>
		Public Overridable Property quiescent As Boolean
			Get
				Return (config And SMASK) + CInt(Fix(ctl >> AC_SHIFT)) <= 0
			End Get
		End Property

		''' <summary>
		''' Returns an estimate of the total number of tasks stolen from
		''' one thread's work queue by another. The reported value
		''' underestimates the actual total number of steals when the pool
		''' is not quiescent. This value may be useful for monitoring and
		''' tuning fork/join programs: in general, steal counts should be
		''' high enough to keep threads busy, but low enough to avoid
		''' overhead and contention across threads.
		''' </summary>
		''' <returns> the number of steals </returns>
		Public Overridable Property stealCount As Long
			Get
				Dim sc As java.util.concurrent.atomic.AtomicLong = stealCounter
				Dim count As Long = If(sc Is Nothing, 0L, sc.get())
				Dim ws As WorkQueue()
				Dim w As WorkQueue
				ws = workQueues
				If ws IsNot Nothing Then
					For i As Integer = 1 To ws.Length - 1 Step 2
						w = ws(i)
						If w IsNot Nothing Then count += w.nsteals
					Next i
				End If
				Return count
			End Get
		End Property

		''' <summary>
		''' Returns an estimate of the total number of tasks currently held
		''' in queues by worker threads (but not including tasks submitted
		''' to the pool that have not begun executing). This value is only
		''' an approximation, obtained by iterating across all threads in
		''' the pool. This method may be useful for tuning task
		''' granularities.
		''' </summary>
		''' <returns> the number of queued tasks </returns>
		Public Overridable Property queuedTaskCount As Long
			Get
				Dim count As Long = 0
				Dim ws As WorkQueue()
				Dim w As WorkQueue
				ws = workQueues
				If ws IsNot Nothing Then
					For i As Integer = 1 To ws.Length - 1 Step 2
						w = ws(i)
						If w IsNot Nothing Then count += w.queueSize()
					Next i
				End If
				Return count
			End Get
		End Property

		''' <summary>
		''' Returns an estimate of the number of tasks submitted to this
		''' pool that have not yet begun executing.  This method may take
		''' time proportional to the number of submissions.
		''' </summary>
		''' <returns> the number of queued submissions </returns>
		Public Overridable Property queuedSubmissionCount As Integer
			Get
				Dim count As Integer = 0
				Dim ws As WorkQueue()
				Dim w As WorkQueue
				ws = workQueues
				If ws IsNot Nothing Then
					For i As Integer = 0 To ws.Length - 1 Step 2
						w = ws(i)
						If w IsNot Nothing Then count += w.queueSize()
					Next i
				End If
				Return count
			End Get
		End Property

		''' <summary>
		''' Returns {@code true} if there are any tasks submitted to this
		''' pool that have not yet begun executing.
		''' </summary>
		''' <returns> {@code true} if there are any queued submissions </returns>
		Public Overridable Function hasQueuedSubmissions() As Boolean
			Dim ws As WorkQueue()
			Dim w As WorkQueue
			ws = workQueues
			If ws IsNot Nothing Then
				For i As Integer = 0 To ws.Length - 1 Step 2
					w = ws(i)
					If w IsNot Nothing AndAlso (Not w.empty) Then Return True
				Next i
			End If
			Return False
		End Function

		''' <summary>
		''' Removes and returns the next unexecuted submission if one is
		''' available.  This method may be useful in extensions to this
		''' class that re-assign work in systems with multiple pools.
		''' </summary>
		''' <returns> the next submission, or {@code null} if none </returns>
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
		Protected Friend Overridable Function pollSubmission() As ForkJoinTask(Of ?)
			Dim ws As WorkQueue()
			Dim w As WorkQueue
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
			Dim t As ForkJoinTask(Of ?)
			ws = workQueues
			If ws IsNot Nothing Then
				For i As Integer = 0 To ws.Length - 1 Step 2
					w = ws(i)
					t = w.poll()
					If w IsNot Nothing AndAlso t IsNot Nothing Then Return t
				Next i
			End If
			Return Nothing
		End Function

		''' <summary>
		''' Removes all available unexecuted submitted and forked tasks
		''' from scheduling queues and adds them to the given collection,
		''' without altering their execution status. These may include
		''' artificially generated or wrapped tasks. This method is
		''' designed to be invoked only when the pool is known to be
		''' quiescent. Invocations at other times may not remove all
		''' tasks. A failure encountered while attempting to add elements
		''' to collection {@code c} may result in elements being in
		''' neither, either or both collections when the associated
		''' exception is thrown.  The behavior of this operation is
		''' undefined if the specified collection is modified while the
		''' operation is in progress.
		''' </summary>
		''' <param name="c"> the collection to transfer elements into </param>
		''' <returns> the number of elements transferred </returns>
'JAVA TO VB CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
		Protected Friend Overridable Function drainTasksTo(Of T1)(ByVal c As ICollection(Of T1)) As Integer
			Dim count As Integer = 0
			Dim ws As WorkQueue()
			Dim w As WorkQueue
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
			Dim t As ForkJoinTask(Of ?)
			ws = workQueues
			If ws IsNot Nothing Then
				For i As Integer = 0 To ws.Length - 1
					w = ws(i)
					If w IsNot Nothing Then
						t = w.poll()
						Do While t IsNot Nothing
							c.Add(t)
							count += 1
							t = w.poll()
						Loop
					End If
				Next i
			End If
			Return count
		End Function

		''' <summary>
		''' Returns a string identifying this pool, as well as its state,
		''' including indications of run state, parallelism level, and
		''' worker and task counts.
		''' </summary>
		''' <returns> a string identifying this pool, as well as its state </returns>
		Public Overrides Function ToString() As String
			' Use a single pass through workQueues to collect counts
			Dim qt As Long = 0L, qs As Long = 0L
			Dim rc As Integer = 0
			Dim sc As java.util.concurrent.atomic.AtomicLong = stealCounter
			Dim st As Long = If(sc Is Nothing, 0L, sc.get())
			Dim c As Long = ctl
			Dim ws As WorkQueue()
			Dim w As WorkQueue
			ws = workQueues
			If ws IsNot Nothing Then
				For i As Integer = 0 To ws.Length - 1
					w = ws(i)
					If w IsNot Nothing Then
						Dim size As Integer = w.queueSize()
						If (i And 1) = 0 Then
							qs += size
						Else
							qt += size
							st += w.nsteals
							If w.apparentlyUnblocked Then rc += 1
						End If
					End If
				Next i
			End If
			Dim pc As Integer = (config And SMASK)
			Dim tc As Integer = pc + CShort(CLng(CULng(c) >> TC_SHIFT))
			Dim ac As Integer = pc + CInt(Fix(c >> AC_SHIFT))
			If ac < 0 Then ' ignore transient negative ac = 0
			Dim rs As Integer = runState
			Dim level As String = (If((rs And TERMINATED) <> 0, "Terminated", If((rs And [STOP]) <> 0, "Terminating", If((rs And SHUTDOWN_Renamed) <> 0, "Shutting down", "Running"))))
			Return MyBase.ToString() & "[" & level & ", parallelism = " & pc & ", size = " & tc & ", active = " & ac & ", running = " & rc & ", steals = " & st & ", tasks = " & qt & ", submissions = " & qs & "]"
		End Function

		''' <summary>
		''' Possibly initiates an orderly shutdown in which previously
		''' submitted tasks are executed, but no new tasks will be
		''' accepted. Invocation has no effect on execution state if this
		''' is the <seealso cref="#commonPool()"/>, and no additional effect if
		''' already shut down.  Tasks that are in the process of being
		''' submitted concurrently during the course of this method may or
		''' may not be rejected.
		''' </summary>
		''' <exception cref="SecurityException"> if a security manager exists and
		'''         the caller is not permitted to modify threads
		'''         because it does not hold {@link
		'''         java.lang.RuntimePermission}{@code ("modifyThread")} </exception>
		Public Overrides Sub shutdown()
			checkPermission()
			tryTerminate(False, True)
		End Sub

		''' <summary>
		''' Possibly attempts to cancel and/or stop all tasks, and reject
		''' all subsequently submitted tasks.  Invocation has no effect on
		''' execution state if this is the <seealso cref="#commonPool()"/>, and no
		''' additional effect if already shut down. Otherwise, tasks that
		''' are in the process of being submitted or executed concurrently
		''' during the course of this method may or may not be
		''' rejected. This method cancels both existing and unexecuted
		''' tasks, in order to permit termination in the presence of task
		''' dependencies. So the method always returns an empty list
		''' (unlike the case for some other Executors).
		''' </summary>
		''' <returns> an empty list </returns>
		''' <exception cref="SecurityException"> if a security manager exists and
		'''         the caller is not permitted to modify threads
		'''         because it does not hold {@link
		'''         java.lang.RuntimePermission}{@code ("modifyThread")} </exception>
		Public Overrides Function shutdownNow() As IList(Of Runnable)
			checkPermission()
			tryTerminate(True, True)
			Return java.util.Collections.emptyList()
		End Function

		''' <summary>
		''' Returns {@code true} if all tasks have completed following shut down.
		''' </summary>
		''' <returns> {@code true} if all tasks have completed following shut down </returns>
		Public Property Overrides terminated As Boolean
			Get
				Return (runState And TERMINATED) <> 0
			End Get
		End Property

		''' <summary>
		''' Returns {@code true} if the process of termination has
		''' commenced but not yet completed.  This method may be useful for
		''' debugging. A return of {@code true} reported a sufficient
		''' period after shutdown may indicate that submitted tasks have
		''' ignored or suppressed interruption, or are waiting for I/O,
		''' causing this executor not to properly terminate. (See the
		''' advisory notes for class <seealso cref="ForkJoinTask"/> stating that
		''' tasks should not normally entail blocking operations.  But if
		''' they do, they must abort them on interrupt.)
		''' </summary>
		''' <returns> {@code true} if terminating but not yet terminated </returns>
		Public Overridable Property terminating As Boolean
			Get
				Dim rs As Integer = runState
				Return (rs And [STOP]) <> 0 AndAlso (rs And TERMINATED) = 0
			End Get
		End Property

		''' <summary>
		''' Returns {@code true} if this pool has been shut down.
		''' </summary>
		''' <returns> {@code true} if this pool has been shut down </returns>
		Public Property Overrides shutdown As Boolean
			Get
				Return (runState And SHUTDOWN_Renamed) <> 0
			End Get
		End Property

		''' <summary>
		''' Blocks until all tasks have completed execution after a
		''' shutdown request, or the timeout occurs, or the current thread
		''' is interrupted, whichever happens first. Because the {@link
		''' #commonPool()} never terminates until program shutdown, when
		''' applied to the common pool, this method is equivalent to {@link
		''' #awaitQuiescence(long, TimeUnit)} but always returns {@code false}.
		''' </summary>
		''' <param name="timeout"> the maximum time to wait </param>
		''' <param name="unit"> the time unit of the timeout argument </param>
		''' <returns> {@code true} if this executor terminated and
		'''         {@code false} if the timeout elapsed before termination </returns>
		''' <exception cref="InterruptedException"> if interrupted while waiting </exception>
		Public Overridable Function awaitTermination(ByVal timeout As Long, ByVal unit As java.util.concurrent.TimeUnit) As Boolean
			If Thread.interrupted() Then Throw New InterruptedException
			If Me Is common Then
				awaitQuiescence(timeout, unit)
				Return False
			End If
			Dim nanos As Long = unit.toNanos(timeout)
			If terminated Then Return True
			If nanos <= 0L Then Return False
			Dim deadline As Long = System.nanoTime() + nanos
			SyncLock Me
				Do
					If terminated Then Return True
					If nanos <= 0L Then Return False
					Dim millis As Long = java.util.concurrent.TimeUnit.NANOSECONDS.toMillis(nanos)
					wait(If(millis > 0L, millis, 1L))
					nanos = deadline - System.nanoTime()
				Loop
			End SyncLock
		End Function

		''' <summary>
		''' If called by a ForkJoinTask operating in this pool, equivalent
		''' in effect to <seealso cref="ForkJoinTask#helpQuiesce"/>. Otherwise,
		''' waits and/or attempts to assist performing tasks until this
		''' pool <seealso cref="#isQuiescent"/> or the indicated timeout elapses.
		''' </summary>
		''' <param name="timeout"> the maximum time to wait </param>
		''' <param name="unit"> the time unit of the timeout argument </param>
		''' <returns> {@code true} if quiescent; {@code false} if the
		''' timeout elapsed. </returns>
		Public Overridable Function awaitQuiescence(ByVal timeout As Long, ByVal unit As java.util.concurrent.TimeUnit) As Boolean
			Dim nanos As Long = unit.toNanos(timeout)
			Dim wt As ForkJoinWorkerThread
			Dim thread_Renamed As Thread = Thread.CurrentThread
			wt = CType(thread_Renamed, ForkJoinWorkerThread)
			If (TypeOf thread_Renamed Is ForkJoinWorkerThread) AndAlso wt .pool Is Me Then
				helpQuiescePool(wt.workQueue)
				Return True
			End If
			Dim startTime As Long = System.nanoTime()
			Dim ws As WorkQueue()
			Dim r As Integer = 0, m As Integer
			Dim found As Boolean = True
			ws = workQueues
			m = ws.Length - 1
			Do While (Not quiescent) AndAlso ws IsNot Nothing AndAlso m >= 0
				If Not found Then
					If (System.nanoTime() - startTime) > nanos Then Return False
					Thread.yield() ' cannot block
				End If
				found = False
				For j As Integer = (m + 1) << 2 To 0 Step -1
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
					Dim t As ForkJoinTask(Of ?)
					Dim q As WorkQueue
					Dim b, k As Integer
'JAVA TO VB CONVERTER TODO TASK: Assignments within expressions are not supported in VB
					Dim tempVar As Boolean = (k = r And m) <= m AndAlso k >= 0 AndAlso (q = ws(k)) IsNot Nothing AndAlso (b = q.base) - q.top < 0
					r += 1
					If tempVar Then
						found = True
						t = q.pollAt(b)
						If t IsNot Nothing Then t.doExec()
						Exit For
					End If
				Next j
				ws = workQueues
				m = ws.Length - 1
			Loop
			Return True
		End Function

		''' <summary>
		''' Waits and/or attempts to assist performing tasks indefinitely
		''' until the <seealso cref="#commonPool()"/> <seealso cref="#isQuiescent"/>.
		''' </summary>
		Friend Shared Sub quiesceCommonPool()
			common.awaitQuiescence(Long.Max_Value, java.util.concurrent.TimeUnit.NANOSECONDS)
		End Sub

		''' <summary>
		''' Interface for extending managed parallelism for tasks running
		''' in <seealso cref="ForkJoinPool"/>s.
		''' 
		''' <p>A {@code ManagedBlocker} provides two methods.  Method
		''' <seealso cref="#isReleasable"/> must return {@code true} if blocking is
		''' not necessary. Method <seealso cref="#block"/> blocks the current thread
		''' if necessary (perhaps internally invoking {@code isReleasable}
		''' before actually blocking). These actions are performed by any
		''' thread invoking <seealso cref="ForkJoinPool#managedBlock(ManagedBlocker)"/>.
		''' The unusual methods in this API accommodate synchronizers that
		''' may, but don't usually, block for long periods. Similarly, they
		''' allow more efficient internal handling of cases in which
		''' additional workers may be, but usually are not, needed to
		''' ensure sufficient parallelism.  Toward this end,
		''' implementations of method {@code isReleasable} must be amenable
		''' to repeated invocation.
		''' 
		''' <p>For example, here is a ManagedBlocker based on a
		''' ReentrantLock:
		'''  <pre> {@code
		''' class ManagedLocker implements ManagedBlocker {
		'''   final ReentrantLock lock;
		'''   boolean hasLock = false;
		'''   ManagedLocker(ReentrantLock lock) { this.lock = lock; }
		'''   public boolean block() {
		'''     if (!hasLock)
		'''       lock.lock();
		'''     return true;
		'''   }
		'''   public boolean isReleasable() {
		'''     return hasLock || (hasLock = lock.tryLock());
		'''   }
		''' }}</pre>
		''' 
		''' <p>Here is a class that possibly blocks waiting for an
		''' item on a given queue:
		'''  <pre> {@code
		''' class QueueTaker<E> implements ManagedBlocker {
		'''   final BlockingQueue<E> queue;
		'''   volatile E item = null;
		'''   QueueTaker(BlockingQueue<E> q) { this.queue = q; }
		'''   public boolean block() throws InterruptedException {
		'''     if (item == null)
		'''       item = queue.take();
		'''     return true;
		'''   }
		'''   public boolean isReleasable() {
		'''     return item != null || (item = queue.poll()) != null;
		'''   }
		'''   public E getItem() { // call after pool.managedBlock completes
		'''     return item;
		'''   }
		''' }}</pre>
		''' </summary>
		Public Interface ManagedBlocker
			''' <summary>
			''' Possibly blocks the current thread, for example waiting for
			''' a lock or condition.
			''' </summary>
			''' <returns> {@code true} if no additional blocking is necessary
			''' (i.e., if isReleasable would return true) </returns>
			''' <exception cref="InterruptedException"> if interrupted while waiting
			''' (the method is not required to do so, but is allowed to) </exception>
			Function block() As Boolean

			''' <summary>
			''' Returns {@code true} if blocking is unnecessary. </summary>
			''' <returns> {@code true} if blocking is unnecessary </returns>
			ReadOnly Property releasable As Boolean
		End Interface

		''' <summary>
		''' Runs the given possibly blocking task.  When {@linkplain
		''' ForkJoinTask#inForkJoinPool() running in a ForkJoinPool}, this
		''' method possibly arranges for a spare thread to be activated if
		''' necessary to ensure sufficient parallelism while the current
		''' thread is blocked in <seealso cref="ManagedBlocker#block blocker.block()"/>.
		''' 
		''' <p>This method repeatedly calls {@code blocker.isReleasable()} and
		''' {@code blocker.block()} until either method returns {@code true}.
		''' Every call to {@code blocker.block()} is preceded by a call to
		''' {@code blocker.isReleasable()} that returned {@code false}.
		''' 
		''' <p>If not running in a ForkJoinPool, this method is
		''' behaviorally equivalent to
		'''  <pre> {@code
		''' while (!blocker.isReleasable())
		'''   if (blocker.block())
		'''     break;}</pre>
		''' 
		''' If running in a ForkJoinPool, the pool may first be expanded to
		''' ensure sufficient parallelism available during the call to
		''' {@code blocker.block()}.
		''' </summary>
		''' <param name="blocker"> the blocker task </param>
		''' <exception cref="InterruptedException"> if {@code blocker.block()} did so </exception>
		Public Shared Sub managedBlock(ByVal blocker As ManagedBlocker)
			Dim p As ForkJoinPool
			Dim wt As ForkJoinWorkerThread
			Dim t As Thread = Thread.CurrentThread
'JAVA TO VB CONVERTER TODO TASK: Assignments within expressions are not supported in VB
			p = (wt = CType(t, ForkJoinWorkerThread)).pool
			If (TypeOf t Is ForkJoinWorkerThread) AndAlso p IsNot Nothing Then
				Dim w As WorkQueue = wt.workQueue
				Do While Not blocker.releasable
					If p.tryCompensate(w) Then
						Try
							Do
							Loop While (Not blocker.releasable) AndAlso Not blocker.block()
						Finally
							U.getAndAddLong(p, CTL, AC_UNIT)
						End Try
						Exit Do
					End If
				Loop
			Else
				Do
				Loop While (Not blocker.releasable) AndAlso Not blocker.block()
			End If
		End Sub

		' AbstractExecutorService overrides.  These rely on undocumented
		' fact that ForkJoinTask.adapt returns ForkJoinTasks that also
		' implement RunnableFuture.

		Protected Friend Overrides Function newTaskFor(Of T)(ByVal runnable As Runnable, ByVal value As T) As java.util.concurrent.RunnableFuture(Of T)
			Return New ForkJoinTask.AdaptedRunnable(Of T)(runnable, value)
		End Function

		Protected Friend Overridable Function newTaskFor(Of T)(ByVal callable As java.util.concurrent.Callable(Of T)) As java.util.concurrent.RunnableFuture(Of T)
			Return New ForkJoinTask.AdaptedCallable(Of T)(callable)
		End Function

		' Unsafe mechanics
		Private Shared ReadOnly U As sun.misc.Unsafe
		Private Shared ReadOnly ABASE As Integer
		Private Shared ReadOnly ASHIFT As Integer
		Private Shared ReadOnly CTL As Long
		Private Shared ReadOnly RUNSTATE As Long
		Private Shared ReadOnly STEALCOUNTER As Long
		Private Shared ReadOnly PARKBLOCKER As Long
		Private Shared ReadOnly QTOP As Long
		Private Shared ReadOnly QLOCK As Long
		Private Shared ReadOnly QSCANSTATE As Long
		Private Shared ReadOnly QPARKER As Long
		Private Shared ReadOnly QCURRENTSTEAL As Long
		Private Shared ReadOnly QCURRENTJOIN As Long

		Shared Sub New()
			' initialize field offsets for CAS etc
			Try
				U = sun.misc.Unsafe.unsafe
				Dim k As  [Class] = GetType(ForkJoinPool)
				CTL = U.objectFieldOffset(k.getDeclaredField("ctl"))
				RUNSTATE = U.objectFieldOffset(k.getDeclaredField("runState"))
				STEALCOUNTER = U.objectFieldOffset(k.getDeclaredField("stealCounter"))
				Dim tk As  [Class] = GetType(Thread)
				PARKBLOCKER = U.objectFieldOffset(tk.getDeclaredField("parkBlocker"))
				Dim wk As  [Class] = GetType(WorkQueue)
				QTOP = U.objectFieldOffset(wk.getDeclaredField("top"))
				QLOCK = U.objectFieldOffset(wk.getDeclaredField("qlock"))
				QSCANSTATE = U.objectFieldOffset(wk.getDeclaredField("scanState"))
				QPARKER = U.objectFieldOffset(wk.getDeclaredField("parker"))
				QCURRENTSTEAL = U.objectFieldOffset(wk.getDeclaredField("currentSteal"))
				QCURRENTJOIN = U.objectFieldOffset(wk.getDeclaredField("currentJoin"))
				Dim ak As  [Class] = GetType(ForkJoinTask())
				ABASE = U.arrayBaseOffset(ak)
				Dim scale As Integer = U.arrayIndexScale(ak)
				If (scale And (scale - 1)) <> 0 Then Throw New [Error]("data type scale not a power of two")
				ASHIFT = 31 -  java.lang.[Integer].numberOfLeadingZeros(scale)
			Catch e As Exception
				Throw New [Error](e)
			End Try

			commonMaxSpares = DEFAULT_COMMON_MAX_SPARES
			defaultForkJoinWorkerThreadFactory = New DefaultForkJoinWorkerThreadFactory
			modifyThreadPermission = New RuntimePermission("modifyThread")

			common = java.security.AccessController.doPrivileged(New PrivilegedActionAnonymousInnerClassHelper(Of T)
			Dim par As Integer = common.config And SMASK ' report 1 even if threads disabled
			commonParallelism = If(par > 0, par, 1)
				Dim innocuousPerms As New java.security.Permissions
				innocuousPerms.add(modifyThreadPermission)
				innocuousPerms.add(New RuntimePermission("enableContextClassLoaderOverride"))
				innocuousPerms.add(New RuntimePermission("modifyThreadGroup"))
				innocuousAcc = New java.security.AccessControlContext(New java.security.ProtectionDomain() { New java.security.ProtectionDomain(Nothing, innocuousPerms)
		End Sub

		Private Class PrivilegedActionAnonymousInnerClassHelper(Of T)
			Implements PrivilegedAction(Of T)

			Public Overridable Function run() As ForkJoinPool
				Return makeCommonPool()
			End Function
		End Class

		''' <summary>
		''' Creates and returns the common pool, respecting user settings
		''' specified via system properties.
		''' </summary>
		Private Shared Function makeCommonPool() As ForkJoinPool
			Dim parallelism_Renamed As Integer = -1
			Dim factory_Renamed As ForkJoinWorkerThreadFactory = Nothing
			Dim handler As Thread.UncaughtExceptionHandler = Nothing
			Try ' ignore exceptions in accessing/parsing properties
				Dim pp As String = System.getProperty("java.util.concurrent.ForkJoinPool.common.parallelism")
				Dim fp As String = System.getProperty("java.util.concurrent.ForkJoinPool.common.threadFactory")
				Dim hp As String = System.getProperty("java.util.concurrent.ForkJoinPool.common.exceptionHandler")
				If pp IsNot Nothing Then parallelism_Renamed = Convert.ToInt32(pp)
				If fp IsNot Nothing Then factory_Renamed = (CType(ClassLoader.systemClassLoader.loadClass(fp).newInstance(), ForkJoinWorkerThreadFactory))
				If hp IsNot Nothing Then handler = (CType(ClassLoader.systemClassLoader.loadClass(hp).newInstance(), Thread.UncaughtExceptionHandler))
			Catch ignore As Exception
			End Try
			If factory_Renamed Is Nothing Then
				If System.securityManager Is Nothing Then
					factory_Renamed = defaultForkJoinWorkerThreadFactory
				Else ' use security-managed default
					factory_Renamed = New InnocuousForkJoinWorkerThreadFactory
				End If
			End If
			parallelism_Renamed = Runtime.runtime.availableProcessors() - 1
			If parallelism_Renamed < 0 AndAlso parallelism_Renamed <= 0 Then ' default 1 less than #cores parallelism_Renamed = 1
			If parallelism_Renamed > MAX_CAP Then parallelism_Renamed = MAX_CAP
			Return New ForkJoinPool(parallelism_Renamed, factory_Renamed, handler, LIFO_QUEUE, "ForkJoinPool.commonPool-worker-")
		End Function

		''' <summary>
		''' Factory for innocuous worker threads
		''' </summary>
		Friend NotInheritable Class InnocuousForkJoinWorkerThreadFactory
			Implements ForkJoinWorkerThreadFactory

			''' <summary>
			''' An ACC to restrict permissions for the factory itself.
			''' The constructed workers have no permissions set.
			''' </summary>
			Private Shared ReadOnly innocuousAcc As java.security.AccessControlContext
		End Class

			Public Function newThread(ByVal pool As ForkJoinPool) As ForkJoinWorkerThread
				Return (ForkJoinWorkerThread.InnocuousForkJoinWorkerThread) java.security.AccessController.doPrivileged(New PrivilegedActionAnonymousInnerClassHelper(Of T)
			End Function

			Private Class PrivilegedActionAnonymousInnerClassHelper(Of T)
				Implements PrivilegedAction(Of T)

				Public Overridable Function run() As ForkJoinWorkerThread
					Return New ForkJoinWorkerThread.InnocuousForkJoinWorkerThread(pool)
				End Function
			End Class
	End Class

	}

End Namespace