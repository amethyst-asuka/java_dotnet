Imports Microsoft.VisualBasic
Imports System
Imports System.Diagnostics
Imports System.Collections
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
	''' A hash table supporting full concurrency of retrievals and
	''' high expected concurrency for updates. This class obeys the
	''' same functional specification as <seealso cref="java.util.Hashtable"/>, and
	''' includes versions of methods corresponding to each method of
	''' {@code Hashtable}. However, even though all operations are
	''' thread-safe, retrieval operations do <em>not</em> entail locking,
	''' and there is <em>not</em> any support for locking the entire table
	''' in a way that prevents all access.  This class is fully
	''' interoperable with {@code Hashtable} in programs that rely on its
	''' thread safety but not on its synchronization details.
	''' 
	''' <p>Retrieval operations (including {@code get}) generally do not
	''' block, so may overlap with update operations (including {@code put}
	''' and {@code remove}). Retrievals reflect the results of the most
	''' recently <em>completed</em> update operations holding upon their
	''' onset. (More formally, an update operation for a given key bears a
	''' <em>happens-before</em> relation with any (non-null) retrieval for
	''' that key reporting the updated value.)  For aggregate operations
	''' such as {@code putAll} and {@code clear}, concurrent retrievals may
	''' reflect insertion or removal of only some entries.  Similarly,
	''' Iterators, Spliterators and Enumerations return elements reflecting the
	''' state of the hash table at some point at or since the creation of the
	''' iterator/enumeration.  They do <em>not</em> throw {@link
	''' java.util.ConcurrentModificationException ConcurrentModificationException}.
	''' However, iterators are designed to be used by only one thread at a time.
	''' Bear in mind that the results of aggregate status methods including
	''' {@code size}, {@code isEmpty}, and {@code containsValue} are typically
	''' useful only when a map is not undergoing concurrent updates in other threads.
	''' Otherwise the results of these methods reflect transient states
	''' that may be adequate for monitoring or estimation purposes, but not
	''' for program control.
	''' 
	''' <p>The table is dynamically expanded when there are too many
	''' collisions (i.e., keys that have distinct hash codes but fall into
	''' the same slot modulo the table size), with the expected average
	''' effect of maintaining roughly two bins per mapping (corresponding
	''' to a 0.75 load factor threshold for resizing). There may be much
	''' variance around this average as mappings are added and removed, but
	''' overall, this maintains a commonly accepted time/space tradeoff for
	''' hash tables.  However, resizing this or any other kind of hash
	''' table may be a relatively slow operation. When possible, it is a
	''' good idea to provide a size estimate as an optional {@code
	''' initialCapacity} constructor argument. An additional optional
	''' {@code loadFactor} constructor argument provides a further means of
	''' customizing initial table capacity by specifying the table density
	''' to be used in calculating the amount of space to allocate for the
	''' given number of elements.  Also, for compatibility with previous
	''' versions of this [Class], constructors may optionally specify an
	''' expected {@code concurrencyLevel} as an additional hint for
	''' internal sizing.  Note that using many keys with exactly the same
	''' {@code hashCode()} is a sure way to slow down performance of any
	''' hash table. To ameliorate impact, when keys are <seealso cref="Comparable"/>,
	''' this class may use comparison order among keys to help break ties.
	''' 
	''' <p>A <seealso cref="Set"/> projection of a ConcurrentHashMap may be created
	''' (using <seealso cref="#newKeySet()"/> or <seealso cref="#newKeySet(int)"/>), or viewed
	''' (using <seealso cref="#keySet(Object)"/> when only keys are of interest, and the
	''' mapped values are (perhaps transiently) not used or all take the
	''' same mapping value.
	''' 
	''' <p>A ConcurrentHashMap can be used as scalable frequency map (a
	''' form of histogram or multiset) by using {@link
	''' java.util.concurrent.atomic.LongAdder} values and initializing via
	''' <seealso cref="#computeIfAbsent computeIfAbsent"/>. For example, to add a count
	''' to a {@code ConcurrentHashMap<String,LongAdder> freqs}, you can use
	''' {@code freqs.computeIfAbsent(k -> new LongAdder()).increment();}
	''' 
	''' <p>This class and its views and iterators implement all of the
	''' <em>optional</em> methods of the <seealso cref="Map"/> and <seealso cref="Iterator"/>
	''' interfaces.
	''' 
	''' <p>Like <seealso cref="Hashtable"/> but unlike <seealso cref="HashMap"/>, this class
	''' does <em>not</em> allow {@code null} to be used as a key or value.
	''' 
	''' <p>ConcurrentHashMaps support a set of sequential and parallel bulk
	''' operations that, unlike most <seealso cref="Stream"/> methods, are designed
	''' to be safely, and often sensibly, applied even with maps that are
	''' being concurrently updated by other threads; for example, when
	''' computing a snapshot summary of the values in a shared registry.
	''' There are three kinds of operation, each with four forms, accepting
	''' functions with Keys, Values, Entries, and (Key, Value) arguments
	''' and/or return values. Because the elements of a ConcurrentHashMap
	''' are not ordered in any particular way, and may be processed in
	''' different orders in different parallel executions, the correctness
	''' of supplied functions should not depend on any ordering, or on any
	''' other objects or values that may transiently change while
	''' computation is in progress; and except for forEach actions, should
	''' ideally be side-effect-free. Bulk operations on <seealso cref="java.util.Map.Entry"/>
	''' objects do not support method {@code setValue}.
	''' 
	''' <ul>
	''' <li> forEach: Perform a given action on each element.
	''' A variant form applies a given transformation on each element
	''' before performing the action.</li>
	''' 
	''' <li> search: Return the first available non-null result of
	''' applying a given function on each element; skipping further
	''' search when a result is found.</li>
	''' 
	''' <li> reduce: Accumulate each element.  The supplied reduction
	''' function cannot rely on ordering (more formally, it should be
	''' both associative and commutative).  There are five variants:
	''' 
	''' <ul>
	''' 
	''' <li> Plain reductions. (There is not a form of this method for
	''' (key, value) function arguments since there is no corresponding
	''' return type.)</li>
	''' 
	''' <li> Mapped reductions that accumulate the results of a given
	''' function applied to each element.</li>
	''' 
	''' <li> Reductions to scalar doubles, longs, and ints, using a
	''' given basis value.</li>
	''' 
	''' </ul>
	''' </li>
	''' </ul>
	''' 
	''' <p>These bulk operations accept a {@code parallelismThreshold}
	''' argument. Methods proceed sequentially if the current map size is
	''' estimated to be less than the given threshold. Using a value of
	''' {@code java.lang.[Long].MAX_VALUE} suppresses all parallelism.  Using a value
	''' of {@code 1} results in maximal parallelism by partitioning into
	''' enough subtasks to fully utilize the {@link
	''' ForkJoinPool#commonPool()} that is used for all parallel
	''' computations. Normally, you would initially choose one of these
	''' extreme values, and then measure performance of using in-between
	''' values that trade off overhead versus throughput.
	''' 
	''' <p>The concurrency properties of bulk operations follow
	''' from those of ConcurrentHashMap: Any non-null result returned
	''' from {@code get(key)} and related access methods bears a
	''' happens-before relation with the associated insertion or
	''' update.  The result of any bulk operation reflects the
	''' composition of these per-element relations (but is not
	''' necessarily atomic with respect to the map as a whole unless it
	''' is somehow known to be quiescent).  Conversely, because keys
	''' and values in the map are never null, null serves as a reliable
	''' atomic indicator of the current lack of any result.  To
	''' maintain this property, null serves as an implicit basis for
	''' all non-scalar reduction operations. For the double, long, and
	''' int versions, the basis should be one that, when combined with
	''' any other value, returns that other value (more formally, it
	''' should be the identity element for the reduction). Most common
	''' reductions have these properties; for example, computing a sum
	''' with basis 0 or a minimum with basis MAX_VALUE.
	''' 
	''' <p>Search and transformation functions provided as arguments
	''' should similarly return null to indicate the lack of any result
	''' (in which case it is not used). In the case of mapped
	''' reductions, this also enables transformations to serve as
	''' filters, returning null (or, in the case of primitive
	''' specializations, the identity basis) if the element should not
	''' be combined. You can create compound transformations and
	''' filterings by composing them yourself under this "null means
	''' there is nothing there now" rule before using them in search or
	''' reduce operations.
	''' 
	''' <p>Methods accepting and/or returning Entry arguments maintain
	''' key-value associations. They may be useful for example when
	''' finding the key for the greatest value. Note that "plain" Entry
	''' arguments can be supplied using {@code new
	''' AbstractMap.SimpleEntry(k,v)}.
	''' 
	''' <p>Bulk operations may complete abruptly, throwing an
	''' exception encountered in the application of a supplied
	''' function. Bear in mind when handling such exceptions that other
	''' concurrently executing functions could also have thrown
	''' exceptions, or would have done so if the first exception had
	''' not occurred.
	''' 
	''' <p>Speedups for parallel compared to sequential forms are common
	''' but not guaranteed.  Parallel operations involving brief functions
	''' on small maps may execute more slowly than sequential forms if the
	''' underlying work to parallelize the computation is more expensive
	''' than the computation itself.  Similarly, parallelization may not
	''' lead to much actual parallelism if all processors are busy
	''' performing unrelated tasks.
	''' 
	''' <p>All arguments to all task methods must be non-null.
	''' 
	''' <p>This class is a member of the
	''' <a href="{@docRoot}/../technotes/guides/collections/index.html">
	''' Java Collections Framework</a>.
	''' 
	''' @since 1.5
	''' @author Doug Lea </summary>
	''' @param <K> the type of keys maintained by this map </param>
	''' @param <V> the type of mapped values </param>
	<Serializable> _
	Public Class ConcurrentHashMap(Of K, V)
		Inherits java.util.AbstractMap(Of K, V)
		Implements java.util.concurrent.ConcurrentMap(Of K, V)

		Private Const serialVersionUID As Long = 7249069246763182397L

	'    
	'     * Overview:
	'     *
	'     * The primary design goal of this hash table is to maintain
	'     * concurrent readability (typically method get(), but also
	'     * iterators and related methods) while minimizing update
	'     * contention. Secondary goals are to keep space consumption about
	'     * the same or better than java.util.HashMap, and to support high
	'     * initial insertion rates on an empty table by many threads.
	'     *
	'     * This map usually acts as a binned (bucketed) hash table.  Each
	'     * key-value mapping is held in a Node.  Most nodes are instances
	'     * of the basic Node class with hash, key, value, and next
	'     * fields. However, various subclasses exist: TreeNodes are
	'     * arranged in balanced trees, not lists.  TreeBins hold the roots
	'     * of sets of TreeNodes. ForwardingNodes are placed at the heads
	'     * of bins during resizing. ReservationNodes are used as
	'     * placeholders while establishing values in computeIfAbsent and
	'     * related methods.  The types TreeBin, ForwardingNode, and
	'     * ReservationNode do not hold normal user keys, values, or
	'     * hashes, and are readily distinguishable during search etc
	'     * because they have negative hash fields and null key and value
	'     * fields. (These special nodes are either uncommon or transient,
	'     * so the impact of carrying around some unused fields is
	'     * insignificant.)
	'     *
	'     * The table is lazily initialized to a power-of-two size upon the
	'     * first insertion.  Each bin in the table normally contains a
	'     * list of Nodes (most often, the list has only zero or one Node).
	'     * Table accesses require volatile/atomic reads, writes, and
	'     * CASes.  Because there is no other way to arrange this without
	'     * adding further indirections, we use intrinsics
	'     * (sun.misc.Unsafe) operations.
	'     *
	'     * We use the top (sign) bit of Node hash fields for control
	'     * purposes -- it is available anyway because of addressing
	'     * constraints.  Nodes with negative hash fields are specially
	'     * handled or ignored in map methods.
	'     *
	'     * Insertion (via put or its variants) of the first node in an
	'     * empty bin is performed by just CASing it to the bin.  This is
	'     * by far the most common case for put operations under most
	'     * key/hash distributions.  Other update operations (insert,
	'     * delete, and replace) require locks.  We do not want to waste
	'     * the space required to associate a distinct lock object with
	'     * each bin, so instead use the first node of a bin list itself as
	'     * a lock. Locking support for these locks relies on builtin
	'     * "synchronized" monitors.
	'     *
	'     * Using the first node of a list as a lock does not by itself
	'     * suffice though: When a node is locked, any update must first
	'     * validate that it is still the first node after locking it, and
	'     * retry if not. Because new nodes are always appended to lists,
	'     * once a node is first in a bin, it remains first until deleted
	'     * or the bin becomes invalidated (upon resizing).
	'     *
	'     * The main disadvantage of per-bin locks is that other update
	'     * operations on other nodes in a bin list protected by the same
	'     * lock can stall, for example when user equals() or mapping
	'     * functions take a long time.  However, statistically, under
	'     * random hash codes, this is not a common problem.  Ideally, the
	'     * frequency of nodes in bins follows a Poisson distribution
	'     * (http://en.wikipedia.org/wiki/Poisson_distribution) with a
	'     * parameter of about 0.5 on average, given the resizing threshold
	'     * of 0.75, although with a large variance because of resizing
	'     * granularity. Ignoring variance, the expected occurrences of
	'     * list size k are (exp(-0.5) * pow(0.5, k) / factorial(k)). The
	'     * first values are:
	'     *
	'     * 0:    0.60653066
	'     * 1:    0.30326533
	'     * 2:    0.07581633
	'     * 3:    0.01263606
	'     * 4:    0.00157952
	'     * 5:    0.00015795
	'     * 6:    0.00001316
	'     * 7:    0.00000094
	'     * 8:    0.00000006
	'     * more: less than 1 in ten million
	'     *
	'     * Lock contention probability for two threads accessing distinct
	'     * elements is roughly 1 / (8 * #elements) under random hashes.
	'     *
	'     * Actual hash code distributions encountered in practice
	'     * sometimes deviate significantly from uniform randomness.  This
	'     * includes the case when N > (1<<30), so some keys MUST collide.
	'     * Similarly for dumb or hostile usages in which multiple keys are
	'     * designed to have identical hash codes or ones that differs only
	'     * in masked-out high bits. So we use a secondary strategy that
	'     * applies when the number of nodes in a bin exceeds a
	'     * threshold. These TreeBins use a balanced tree to hold nodes (a
	'     * specialized form of red-black trees), bounding search time to
	'     * O(log N).  Each search step in a TreeBin is at least twice as
	'     * slow as in a regular list, but given that N cannot exceed
	'     * (1<<64) (before running out of addresses) this bounds search
	'     * steps, lock hold times, etc, to reasonable constants (roughly
	'     * 100 nodes inspected per operation worst case) so long as keys
	'     * are Comparable (which is very common -- String, Long, etc).
	'     * TreeBin nodes (TreeNodes) also maintain the same "next"
	'     * traversal pointers as regular nodes, so can be traversed in
	'     * iterators in the same way.
	'     *
	'     * The table is resized when occupancy exceeds a percentage
	'     * threshold (nominally, 0.75, but see below).  Any thread
	'     * noticing an overfull bin may assist in resizing after the
	'     * initiating thread allocates and sets up the replacement array.
	'     * However, rather than stalling, these other threads may proceed
	'     * with insertions etc.  The use of TreeBins shields us from the
	'     * worst case effects of overfilling while resizes are in
	'     * progress.  Resizing proceeds by transferring bins, one by one,
	'     * from the table to the next table. However, threads claim small
	'     * blocks of indices to transfer (via field transferIndex) before
	'     * doing so, reducing contention.  A generation stamp in field
	'     * sizeCtl ensures that resizings do not overlap. Because we are
	'     * using power-of-two expansion, the elements from each bin must
	'     * either stay at same index, or move with a power of two
	'     * offset. We eliminate unnecessary node creation by catching
	'     * cases where old nodes can be reused because their next fields
	'     * won't change.  On average, only about one-sixth of them need
	'     * cloning when a table doubles. The nodes they replace will be
	'     * garbage collectable as soon as they are no longer referenced by
	'     * any reader thread that may be in the midst of concurrently
	'     * traversing table.  Upon transfer, the old table bin contains
	'     * only a special forwarding node (with hash field "MOVED") that
	'     * contains the next table as its key. On encountering a
	'     * forwarding node, access and update operations restart, using
	'     * the new table.
	'     *
	'     * Each bin transfer requires its bin lock, which can stall
	'     * waiting for locks while resizing. However, because other
	'     * threads can join in and help resize rather than contend for
	'     * locks, average aggregate waits become shorter as resizing
	'     * progresses.  The transfer operation must also ensure that all
	'     * accessible bins in both the old and new table are usable by any
	'     * traversal.  This is arranged in part by proceeding from the
	'     * last bin (table.length - 1) up towards the first.  Upon seeing
	'     * a forwarding node, traversals (see class Traverser) arrange to
	'     * move to the new table without revisiting nodes.  To ensure that
	'     * no intervening nodes are skipped even when moved out of order,
	'     * a stack (see class TableStack) is created on first encounter of
	'     * a forwarding node during a traversal, to maintain its place if
	'     * later processing the current table. The need for these
	'     * save/restore mechanics is relatively rare, but when one
	'     * forwarding node is encountered, typically many more will be.
	'     * So Traversers use a simple caching scheme to avoid creating so
	'     * many new TableStack nodes. (Thanks to Peter Levart for
	'     * suggesting use of a stack here.)
	'     *
	'     * The traversal scheme also applies to partial traversals of
	'     * ranges of bins (via an alternate Traverser constructor)
	'     * to support partitioned aggregate operations.  Also, read-only
	'     * operations give up if ever forwarded to a null table, which
	'     * provides support for shutdown-style clearing, which is also not
	'     * currently implemented.
	'     *
	'     * Lazy table initialization minimizes footprint until first use,
	'     * and also avoids resizings when the first operation is from a
	'     * putAll, constructor with map argument, or deserialization.
	'     * These cases attempt to override the initial capacity settings,
	'     * but harmlessly fail to take effect in cases of races.
	'     *
	'     * The element count is maintained using a specialization of
	'     * LongAdder. We need to incorporate a specialization rather than
	'     * just use a LongAdder in order to access implicit
	'     * contention-sensing that leads to creation of multiple
	'     * CounterCells.  The counter mechanics avoid contention on
	'     * updates but can encounter cache thrashing if read too
	'     * frequently during concurrent access. To avoid reading so often,
	'     * resizing under contention is attempted only upon adding to a
	'     * bin already holding two or more nodes. Under uniform hash
	'     * distributions, the probability of this occurring at threshold
	'     * is around 13%, meaning that only about 1 in 8 puts check
	'     * threshold (and after resizing, many fewer do so).
	'     *
	'     * TreeBins use a special form of comparison for search and
	'     * related operations (which is the main reason we cannot use
	'     * existing collections such as TreeMaps). TreeBins contain
	'     * Comparable elements, but may contain others, as well as
	'     * elements that are Comparable but not necessarily Comparable for
	'     * the same T, so we cannot invoke compareTo among them. To handle
	'     * this, the tree is ordered primarily by hash value, then by
	'     * Comparable.compareTo order if applicable.  On lookup at a node,
	'     * if elements are not comparable or compare as 0 then both left
	'     * and right children may need to be searched in the case of tied
	'     * hash values. (This corresponds to the full list search that
	'     * would be necessary if all elements were non-Comparable and had
	'     * tied hashes.) On insertion, to keep a total ordering (or as
	'     * close as is required here) across rebalancings, we compare
	'     * classes and identityHashCodes as tie-breakers. The red-black
	'     * balancing code is updated from pre-jdk-collections
	'     * (http://gee.cs.oswego.edu/dl/classes/collections/RBCell.java)
	'     * based in turn on Cormen, Leiserson, and Rivest "Introduction to
	'     * Algorithms" (CLR).
	'     *
	'     * TreeBins also require an additional locking mechanism.  While
	'     * list traversal is always possible by readers even during
	'     * updates, tree traversal is not, mainly because of tree-rotations
	'     * that may change the root node and/or its linkages.  TreeBins
	'     * include a simple read-write lock mechanism parasitic on the
	'     * main bin-synchronization strategy: Structural adjustments
	'     * associated with an insertion or removal are already bin-locked
	'     * (and so cannot conflict with other writers) but must wait for
	'     * ongoing readers to finish. Since there can be only one such
	'     * waiter, we use a simple scheme using a single "waiter" field to
	'     * block writers.  However, readers need never block.  If the root
	'     * lock is held, they proceed along the slow traversal path (via
	'     * next-pointers) until the lock becomes available or the list is
	'     * exhausted, whichever comes first. These cases are not fast, but
	'     * maximize aggregate expected throughput.
	'     *
	'     * Maintaining API and serialization compatibility with previous
	'     * versions of this class introduces several oddities. Mainly: We
	'     * leave untouched but unused constructor arguments refering to
	'     * concurrencyLevel. We accept a loadFactor constructor argument,
	'     * but apply it only to initial table capacity (which is the only
	'     * time that we can guarantee to honor it.) We also declare an
	'     * unused "Segment" class that is instantiated in minimal form
	'     * only when serializing.
	'     *
	'     * Also, solely for compatibility with previous versions of this
	'     * [Class], it extends AbstractMap, even though all of its methods
	'     * are overridden, so it is just useless baggage.
	'     *
	'     * This file is organized to make things a little easier to follow
	'     * while reading than they might otherwise: First the main static
	'     * declarations and utilities, then fields, then main public
	'     * methods (with a few factorings of multiple public methods into
	'     * internal ones), then sizing methods, trees, traversers, and
	'     * bulk operations.
	'     

		' ---------------- Constants -------------- 

		''' <summary>
		''' The largest possible table capacity.  This value must be
		''' exactly 1<<30 to stay within Java array allocation and indexing
		''' bounds for power of two table sizes, and is further required
		''' because the top two bits of 32bit hash fields are used for
		''' control purposes.
		''' </summary>
		Private Shared ReadOnly MAXIMUM_CAPACITY As Integer = 1 << 30

		''' <summary>
		''' The default initial table capacity.  Must be a power of 2
		''' (i.e., at least 1) and at most MAXIMUM_CAPACITY.
		''' </summary>
		Private Const DEFAULT_CAPACITY As Integer = 16

		''' <summary>
		''' The largest possible (non-power of two) array size.
		''' Needed by toArray and related methods.
		''' </summary>
		Friend Shared ReadOnly MAX_ARRAY_SIZE As Integer =  java.lang.[Integer].MAX_VALUE - 8

		''' <summary>
		''' The default concurrency level for this table. Unused but
		''' defined for compatibility with previous versions of this class.
		''' </summary>
		Private Const DEFAULT_CONCURRENCY_LEVEL As Integer = 16

		''' <summary>
		''' The load factor for this table. Overrides of this value in
		''' constructors affect only the initial table capacity.  The
		''' actual floating point value isn't normally used -- it is
		''' simpler to use expressions such as {@code n - (n >>> 2)} for
		''' the associated resizing threshold.
		''' </summary>
		Private Const LOAD_FACTOR As Single = 0.75f

		''' <summary>
		''' The bin count threshold for using a tree rather than list for a
		''' bin.  Bins are converted to trees when adding an element to a
		''' bin with at least this many nodes. The value must be greater
		''' than 2, and should be at least 8 to mesh with assumptions in
		''' tree removal about conversion back to plain bins upon
		''' shrinkage.
		''' </summary>
		Friend Const TREEIFY_THRESHOLD As Integer = 8

		''' <summary>
		''' The bin count threshold for untreeifying a (split) bin during a
		''' resize operation. Should be less than TREEIFY_THRESHOLD, and at
		''' most 6 to mesh with shrinkage detection under removal.
		''' </summary>
		Friend Const UNTREEIFY_THRESHOLD As Integer = 6

		''' <summary>
		''' The smallest table capacity for which bins may be treeified.
		''' (Otherwise the table is resized if too many nodes in a bin.)
		''' The value should be at least 4 * TREEIFY_THRESHOLD to avoid
		''' conflicts between resizing and treeification thresholds.
		''' </summary>
		Friend Const MIN_TREEIFY_CAPACITY As Integer = 64

		''' <summary>
		''' Minimum number of rebinnings per transfer step. Ranges are
		''' subdivided to allow multiple resizer threads.  This value
		''' serves as a lower bound to avoid resizers encountering
		''' excessive memory contention.  The value should be at least
		''' DEFAULT_CAPACITY.
		''' </summary>
		Private Const MIN_TRANSFER_STRIDE As Integer = 16

		''' <summary>
		''' The number of bits used for generation stamp in sizeCtl.
		''' Must be at least 6 for 32bit arrays.
		''' </summary>
		Private Shared RESIZE_STAMP_BITS As Integer = 16

		''' <summary>
		''' The maximum number of threads that can help resize.
		''' Must fit in 32 - RESIZE_STAMP_BITS bits.
		''' </summary>
		Private Shared ReadOnly MAX_RESIZERS As Integer = (1 << (32 - RESIZE_STAMP_BITS)) - 1

		''' <summary>
		''' The bit shift for recording size stamp in sizeCtl.
		''' </summary>
		Private Shared ReadOnly RESIZE_STAMP_SHIFT As Integer = 32 - RESIZE_STAMP_BITS

	'    
	'     * Encodings for Node hash fields. See above for explanation.
	'     
		Friend Const MOVED As Integer = -1 ' hash for forwarding nodes
		Friend Const TREEBIN As Integer = -2 ' hash for roots of trees
		Friend Const RESERVED As Integer = -3 ' hash for transient reservations
		Friend Const HASH_BITS As Integer = &H7fffffff ' usable bits of normal node hash

		''' <summary>
		''' Number of CPUS, to place bounds on some sizings </summary>
		Friend Shared ReadOnly NCPU As Integer = Runtime.runtime.availableProcessors()

		''' <summary>
		''' For serialization compatibility. </summary>
		Private Shared ReadOnly serialPersistentFields As java.io.ObjectStreamField() = { New java.io.ObjectStreamField("segments", GetType(Segment())), New java.io.ObjectStreamField("segmentMask",  java.lang.[Integer].TYPE), New java.io.ObjectStreamField("segmentShift",  java.lang.[Integer].TYPE) }

		' ---------------- Nodes -------------- 

		''' <summary>
		''' Key-value entry.  This class is never exported out as a
		''' user-mutable Map.Entry (i.e., one supporting setValue; see
		''' MapEntry below), but can be used for read-only traversals used
		''' in bulk tasks.  Subclasses of Node with a negative hash field
		''' are special, and contain null keys and values (but are never
		''' exported).  Otherwise, keys and vals are never null.
		''' </summary>
		Friend Class Node(Of K, V)
			Implements KeyValuePair(Of K, V)

			Friend ReadOnly hash As Integer
			Friend ReadOnly key As K
'JAVA TO VB CONVERTER TODO TASK: There is no VB equivalent to 'volatile':
			Friend val As V
'JAVA TO VB CONVERTER TODO TASK: There is no VB equivalent to 'volatile':
			Friend [next] As Node(Of K, V)

			Friend Sub New(ByVal hash As Integer, ByVal key As K, ByVal val As V, ByVal [next] As Node(Of K, V))
				Me.hash = hash
				Me.key = key
				Me.val = val
				Me.next = [next]
			End Sub

			Public Property key As K
				Get
					Return key
				End Get
			End Property
			Public Property value As V
				Get
					Return val
				End Get
			End Property
			Public NotOverridable Overrides Function GetHashCode() As Integer
				Return key.GetHashCode() Xor val.GetHashCode()
			End Function
			Public NotOverridable Overrides Function ToString() As String
				Return key & "=" & val
			End Function
			Public Function setValue(ByVal value As V) As V
				Throw New UnsupportedOperationException
			End Function

			Public NotOverridable Overrides Function Equals(ByVal o As Object) As Boolean
				Dim k, v, u As Object
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
				Dim e As KeyValuePair(Of ?, ?)
'JAVA TO VB CONVERTER TODO TASK: Assignments within expressions are not supported in VB
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
				Return ((TypeOf o Is DictionaryEntry) AndAlso (k = (e = CType(o, KeyValuePair(Of ?, ?))).key) IsNot Nothing AndAlso (v = e.Value) IsNot Nothing AndAlso (k Is key OrElse k.Equals(key)) AndAlso (v Is (u = val) OrElse v.Equals(u)))
			End Function

			''' <summary>
			''' Virtualized support for map.get(); overridden in subclasses.
			''' </summary>
			Friend Overridable Function find(ByVal h As Integer, ByVal k As Object) As Node(Of K, V)
				Dim e As Node(Of K, V) = Me
				If k IsNot Nothing Then
					Do
						Dim ek As K
						ek = e.key
						If e.hash = h AndAlso (ek Is k OrElse (ek IsNot Nothing AndAlso k.Equals(ek))) Then Return e
						e = e.next
					Loop While e IsNot Nothing
				End If
				Return Nothing
			End Function
		End Class

		' ---------------- Static utilities -------------- 

		''' <summary>
		''' Spreads (XORs) higher bits of hash to lower and also forces top
		''' bit to 0. Because the table uses power-of-two masking, sets of
		''' hashes that vary only in bits above the current mask will
		''' always collide. (Among known examples are sets of Float keys
		''' holding consecutive whole numbers in small tables.)  So we
		''' apply a transform that spreads the impact of higher bits
		''' downward. There is a tradeoff between speed, utility, and
		''' quality of bit-spreading. Because many common sets of hashes
		''' are already reasonably distributed (so don't benefit from
		''' spreading), and because we use trees to handle large sets of
		''' collisions in bins, we just XOR some shifted bits in the
		''' cheapest possible way to reduce systematic lossage, as well as
		''' to incorporate impact of the highest bits that would otherwise
		''' never be used in index calculations because of table bounds.
		''' </summary>
		Friend Shared Function spread(ByVal h As Integer) As Integer
			Return (h Xor (CInt(CUInt(h) >> 16))) And HASH_BITS
		End Function

		''' <summary>
		''' Returns a power of two table size for the given desired capacity.
		''' See Hackers Delight, sec 3.2
		''' </summary>
		Private Shared Function tableSizeFor(ByVal c As Integer) As Integer
			Dim n As Integer = c - 1
			n = n Or CInt(CUInt(n) >> 1)
			n = n Or CInt(CUInt(n) >> 2)
			n = n Or CInt(CUInt(n) >> 4)
			n = n Or CInt(CUInt(n) >> 8)
			n = n Or CInt(CUInt(n) >> 16)
			Return If(n < 0, 1, If(n >= MAXIMUM_CAPACITY, MAXIMUM_CAPACITY, n + 1))
		End Function

		''' <summary>
		''' Returns x's Class if it is of the form "class C implements
		''' Comparable<C>", else null.
		''' </summary>
		Friend Shared Function comparableClassFor(ByVal x As Object) As  [Class]
			If TypeOf x Is Comparable Then
				Dim c As  [Class]
				Dim ts, [as] As Type()
				Dim t As Type
				Dim p As ParameterizedType
				c = x.GetType()
				If c Is GetType(String) Then ' bypass checks Return c
				ts = c.genericInterfaces
				If ts IsNot Nothing Then
					For i As Integer = 0 To ts.Length - 1
						t = ts(i)
						p = CType(t, ParameterizedType)
						[as] = p.actualTypeArguments
						If (TypeOf t Is ParameterizedType) AndAlso (p .rawType = GetType(Comparable)) AndAlso [as] IsNot Nothing AndAlso [as].Length = 1 AndAlso [as](0) Is c Then ' type arg is c Return c
					Next i
				End If
			End If
			Return Nothing
		End Function

		''' <summary>
		''' Returns k.compareTo(x) if x matches kc (k's screened comparable
		''' [Class]), else 0.
		''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
		Friend Shared Function compareComparables(ByVal kc As [Class], ByVal k As Object, ByVal x As Object) As Integer ' for cast to Comparable
			Return (If(x Is Nothing OrElse x.GetType() IsNot kc, 0, CType(k, Comparable).CompareTo(x)))
		End Function

		' ---------------- Table element access -------------- 

	'    
	'     * Volatile access methods are used for table elements as well as
	'     * elements of in-progress next table while resizing.  All uses of
	'     * the tab arguments must be null checked by callers.  All callers
	'     * also paranoically precheck that tab's length is not zero (or an
	'     * equivalent check), thus ensuring that any index argument taking
	'     * the form of a hash value anded with (length - 1) is a valid
	'     * index.  Note that, to be correct wrt arbitrary concurrency
	'     * errors by users, these checks must operate on local variables,
	'     * which accounts for some odd-looking inline assignments below.
	'     * Note that calls to setTabAt always occur within locked regions,
	'     * and so in principle require only release ordering, not
	'     * full volatile semantics, but are currently coded as volatile
	'     * writes to be conservative.
	'     

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
		Friend Shared Function tabAt(Of K, V)(ByVal tab As Node(Of K, V)(), ByVal i As Integer) As Node(Of K, V)
			Return CType(U.getObjectVolatile(tab, (CLng(i) << ASHIFT) + ABASE), Node(Of K, V))
		End Function

		Friend Shared Function casTabAt(Of K, V)(ByVal tab As Node(Of K, V)(), ByVal i As Integer, ByVal c As Node(Of K, V), ByVal v As Node(Of K, V)) As Boolean
			Return U.compareAndSwapObject(tab, (CLng(i) << ASHIFT) + ABASE, c, v)
		End Function

		Friend Shared Sub setTabAt(Of K, V)(ByVal tab As Node(Of K, V)(), ByVal i As Integer, ByVal v As Node(Of K, V))
			U.putObjectVolatile(tab, (CLng(i) << ASHIFT) + ABASE, v)
		End Sub

		' ---------------- Fields -------------- 

		''' <summary>
		''' The array of bins. Lazily initialized upon first insertion.
		''' Size is always a power of two. Accessed directly by iterators.
		''' </summary>
'JAVA TO VB CONVERTER TODO TASK: There is no VB equivalent to 'volatile':
		<NonSerialized> _
		Friend table As Node(Of K, V)()

		''' <summary>
		''' The next table to use; non-null only while resizing.
		''' </summary>
'JAVA TO VB CONVERTER TODO TASK: There is no VB equivalent to 'volatile':
		<NonSerialized> _
		Private nextTable As Node(Of K, V)()

		''' <summary>
		''' Base counter value, used mainly when there is no contention,
		''' but also as a fallback during table initialization
		''' races. Updated via CAS.
		''' </summary>
'JAVA TO VB CONVERTER TODO TASK: There is no VB equivalent to 'volatile':
		<NonSerialized> _
		Private baseCount As Long

		''' <summary>
		''' Table initialization and resizing control.  When negative, the
		''' table is being initialized or resized: -1 for initialization,
		''' else -(1 + the number of active resizing threads).  Otherwise,
		''' when table is null, holds the initial table size to use upon
		''' creation, or 0 for default. After initialization, holds the
		''' next element count value upon which to resize the table.
		''' </summary>
'JAVA TO VB CONVERTER TODO TASK: There is no VB equivalent to 'volatile':
		<NonSerialized> _
		Private sizeCtl As Integer

		''' <summary>
		''' The next table index (plus one) to split while resizing.
		''' </summary>
'JAVA TO VB CONVERTER TODO TASK: There is no VB equivalent to 'volatile':
		<NonSerialized> _
		Private transferIndex As Integer

		''' <summary>
		''' Spinlock (locked via CAS) used when resizing and/or creating CounterCells.
		''' </summary>
'JAVA TO VB CONVERTER TODO TASK: There is no VB equivalent to 'volatile':
		<NonSerialized> _
		Private cellsBusy As Integer

		''' <summary>
		''' Table of counter cells. When non-null, size is a power of 2.
		''' </summary>
'JAVA TO VB CONVERTER TODO TASK: There is no VB equivalent to 'volatile':
		<NonSerialized> _
		Private counterCells As CounterCell()

		' views
		<NonSerialized> _
		Private keySet_Renamed As KeySetView(Of K, V)
		<NonSerialized> _
		Private values_Renamed As ValuesView(Of K, V)
		<NonSerialized> _
		Private entrySet_Renamed As EntrySetView(Of K, V)


		' ---------------- Public operations -------------- 

		''' <summary>
		''' Creates a new, empty map with the default initial table size (16).
		''' </summary>
		Public Sub New()
		End Sub

		''' <summary>
		''' Creates a new, empty map with an initial table size
		''' accommodating the specified number of elements without the need
		''' to dynamically resize.
		''' </summary>
		''' <param name="initialCapacity"> The implementation performs internal
		''' sizing to accommodate this many elements. </param>
		''' <exception cref="IllegalArgumentException"> if the initial capacity of
		''' elements is negative </exception>
		Public Sub New(ByVal initialCapacity As Integer)
			If initialCapacity < 0 Then Throw New IllegalArgumentException
			Dim cap As Integer = (If(initialCapacity >= (CInt(CUInt(MAXIMUM_CAPACITY) >> 1)), MAXIMUM_CAPACITY, tableSizeFor(initialCapacity + (CInt(CUInt(initialCapacity) >> 1)) + 1)))
			Me.sizeCtl = cap
		End Sub

		''' <summary>
		''' Creates a new map with the same mappings as the given map.
		''' </summary>
		''' <param name="m"> the map </param>
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
		Public Sub New(Of T1 As K, ? As V)(ByVal m As IDictionary(Of T1))
			Me.sizeCtl = DEFAULT_CAPACITY
			putAll(m)
		End Sub

		''' <summary>
		''' Creates a new, empty map with an initial table size based on
		''' the given number of elements ({@code initialCapacity}) and
		''' initial table density ({@code loadFactor}).
		''' </summary>
		''' <param name="initialCapacity"> the initial capacity. The implementation
		''' performs internal sizing to accommodate this many elements,
		''' given the specified load factor. </param>
		''' <param name="loadFactor"> the load factor (table density) for
		''' establishing the initial table size </param>
		''' <exception cref="IllegalArgumentException"> if the initial capacity of
		''' elements is negative or the load factor is nonpositive
		''' 
		''' @since 1.6 </exception>
		Public Sub New(ByVal initialCapacity As Integer, ByVal loadFactor As Single)
			Me.New(initialCapacity, loadFactor, 1)
		End Sub

		''' <summary>
		''' Creates a new, empty map with an initial table size based on
		''' the given number of elements ({@code initialCapacity}), table
		''' density ({@code loadFactor}), and number of concurrently
		''' updating threads ({@code concurrencyLevel}).
		''' </summary>
		''' <param name="initialCapacity"> the initial capacity. The implementation
		''' performs internal sizing to accommodate this many elements,
		''' given the specified load factor. </param>
		''' <param name="loadFactor"> the load factor (table density) for
		''' establishing the initial table size </param>
		''' <param name="concurrencyLevel"> the estimated number of concurrently
		''' updating threads. The implementation may use this value as
		''' a sizing hint. </param>
		''' <exception cref="IllegalArgumentException"> if the initial capacity is
		''' negative or the load factor or concurrencyLevel are
		''' nonpositive </exception>
		Public Sub New(ByVal initialCapacity As Integer, ByVal loadFactor As Single, ByVal concurrencyLevel As Integer)
			If Not(loadFactor > 0.0f) OrElse initialCapacity < 0 OrElse concurrencyLevel <= 0 Then Throw New IllegalArgumentException
			If initialCapacity < concurrencyLevel Then ' Use at least as many bins initialCapacity = concurrencyLevel ' as estimated threads
			Dim size As Long = CLng(Fix(1.0 + CLng(initialCapacity) / loadFactor))
			Dim cap As Integer = If(size >= CLng(MAXIMUM_CAPACITY), MAXIMUM_CAPACITY, tableSizeFor(CInt(size)))
			Me.sizeCtl = cap
		End Sub

		' Original (since JDK1.2) Map methods

		''' <summary>
		''' {@inheritDoc}
		''' </summary>
		Public Overridable Function size() As Integer
			Dim n As Long = sumCount()
			Return (If(n < 0L, 0, If(n > (Long) java.lang.[Integer].Max_Value,  java.lang.[Integer].Max_Value, CInt(n))))
		End Function

		''' <summary>
		''' {@inheritDoc}
		''' </summary>
		Public Overridable Property empty As Boolean
			Get
				Return sumCount() <= 0L ' ignore transient negative values
			End Get
		End Property

		''' <summary>
		''' Returns the value to which the specified key is mapped,
		''' or {@code null} if this map contains no mapping for the key.
		''' 
		''' <p>More formally, if this map contains a mapping from a key
		''' {@code k} to a value {@code v} such that {@code key.equals(k)},
		''' then this method returns {@code v}; otherwise it returns
		''' {@code null}.  (There can be at most one such mapping.)
		''' </summary>
		''' <exception cref="NullPointerException"> if the specified key is null </exception>
		Public Overridable Function [get](ByVal key As Object) As V
			Dim tab As Node(Of K, V)()
			Dim e As Node(Of K, V), p As Node(Of K, V)
			Dim n, eh As Integer
			Dim ek As K
			Dim h As Integer = spread(key.GetHashCode())
			tab = table
			n = tab.Length
			e = tabAt(tab, (n - 1) And h)
			If tab IsNot Nothing AndAlso n > 0 AndAlso e IsNot Nothing Then
				eh = e.hash
				If eh = h Then
					ek = e.key
					If ek Is key OrElse (ek IsNot Nothing AndAlso key.Equals(ek)) Then Return e.val
				ElseIf eh < 0 Then
'JAVA TO VB CONVERTER TODO TASK: Assignments within expressions are not supported in VB
					Return If((p = e.find(h, key)) IsNot Nothing, p.val, Nothing)
				End If
				e = e.next
				Do While e IsNot Nothing
					ek = e.key
					If e.hash = h AndAlso (ek Is key OrElse (ek IsNot Nothing AndAlso key.Equals(ek))) Then Return e.val
					e = e.next
				Loop
			End If
			Return Nothing
		End Function

		''' <summary>
		''' Tests if the specified object is a key in this table.
		''' </summary>
		''' <param name="key"> possible key </param>
		''' <returns> {@code true} if and only if the specified object
		'''         is a key in this table, as determined by the
		'''         {@code equals} method; {@code false} otherwise </returns>
		''' <exception cref="NullPointerException"> if the specified key is null </exception>
		Public Overridable Function containsKey(ByVal key As Object) As Boolean
			Return [get](key) IsNot Nothing
		End Function

		''' <summary>
		''' Returns {@code true} if this map maps one or more keys to the
		''' specified value. Note: This method may require a full traversal
		''' of the map, and is much slower than method {@code containsKey}.
		''' </summary>
		''' <param name="value"> value whose presence in this map is to be tested </param>
		''' <returns> {@code true} if this map maps one or more keys to the
		'''         specified value </returns>
		''' <exception cref="NullPointerException"> if the specified value is null </exception>
		Public Overridable Function containsValue(ByVal value As Object) As Boolean
			If value Is Nothing Then Throw New NullPointerException
			Dim t As Node(Of K, V)()
			t = table
			If t IsNot Nothing Then
				Dim it As New Traverser(Of K, V)(t, t.Length, 0, t.Length)
				Dim p As Node(Of K, V)
'JAVA TO VB CONVERTER TODO TASK: Assignments within expressions are not supported in VB
				Do While (p = it.advance()) IsNot Nothing
					Dim v As V
					v = p.val
					If v Is value OrElse (v IsNot Nothing AndAlso value.Equals(v)) Then Return True
				Loop
			End If
			Return False
		End Function

		''' <summary>
		''' Maps the specified key to the specified value in this table.
		''' Neither the key nor the value can be null.
		''' 
		''' <p>The value can be retrieved by calling the {@code get} method
		''' with a key that is equal to the original key.
		''' </summary>
		''' <param name="key"> key with which the specified value is to be associated </param>
		''' <param name="value"> value to be associated with the specified key </param>
		''' <returns> the previous value associated with {@code key}, or
		'''         {@code null} if there was no mapping for {@code key} </returns>
		''' <exception cref="NullPointerException"> if the specified key or value is null </exception>
		Public Overridable Function put(ByVal key As K, ByVal value As V) As V
			Return putVal(key, value, False)
		End Function

		''' <summary>
		''' Implementation for put and putIfAbsent </summary>
		Friend Function putVal(ByVal key As K, ByVal value As V, ByVal onlyIfAbsent As Boolean) As V
			If key Is Nothing OrElse value Is Nothing Then Throw New NullPointerException
			Dim hash As Integer = spread(key.GetHashCode())
			Dim binCount As Integer = 0
			Dim tab As Node(Of K, V)() = table
			Do
				Dim f As Node(Of K, V)
				Dim n, i, fh As Integer
				n = tab.length
				If tab Is Nothing OrElse n = 0 Then
					tab = initTable()
				Else
'JAVA TO VB CONVERTER TODO TASK: Assignments within expressions are not supported in VB
					f = tabAt(tab, i = (n - 1) And hash)
					If f Is Nothing Then
						If casTabAt(tab, i, Nothing, New Node(Of K, V)(hash, key, value, Nothing)) Then Exit Do ' no lock when adding to empty bin
					Else
						fh = f.hash
						If fh = MOVED Then
							tab = helpTransfer(tab, f)
						Else
							Dim oldVal As V = Nothing
						End If
					End If
					SyncLock f
						If tabAt(tab, i) Is f Then
							If fh >= 0 Then
								binCount = 1
								Dim e As Node(Of K, V) = f
								Do
									Dim ek As K
									ek = e.key
									If e.hash = hash AndAlso (ek Is key OrElse (ek IsNot Nothing AndAlso key.Equals(ek))) Then
										oldVal = e.val
										If Not onlyIfAbsent Then e.val = value
										Exit Do
									End If
									Dim pred As Node(Of K, V) = e
									e = e.next
									If e Is Nothing Then
										pred.next = New Node(Of K, V)(hash, key, value, Nothing)
										Exit Do
									End If
									binCount += 1
								Loop
							ElseIf TypeOf f Is TreeBin Then
								Dim p As Node(Of K, V)
								binCount = 2
								p = CType(f, TreeBin(Of K, V)).putTreeVal(hash, key, value)
								If p IsNot Nothing Then
									oldVal = p.val
									If Not onlyIfAbsent Then p.val = value
								End If
							End If
						End If
					End SyncLock
					If binCount <> 0 Then
						If binCount >= TREEIFY_THRESHOLD Then treeifyBin(tab, i)
						If oldVal IsNot Nothing Then Return oldVal
						Exit Do
					End If
						End If
			Loop
			addCount(1L, binCount)
			Return Nothing
		End Function

		''' <summary>
		''' Copies all of the mappings from the specified map to this one.
		''' These mappings replace any mappings that this map had for any of the
		''' keys currently in the specified map.
		''' </summary>
		''' <param name="m"> mappings to be stored in this map </param>
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
		Public Overridable Sub putAll(Of T1 As K, ? As V)(ByVal m As IDictionary(Of T1))
			tryPresize(m.Count)
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
			For Each e As KeyValuePair(Of ? As K, ? As V) In m
				putVal(e.Key, e.Value, False)
			Next e
		End Sub

		''' <summary>
		''' Removes the key (and its corresponding value) from this map.
		''' This method does nothing if the key is not in the map.
		''' </summary>
		''' <param name="key"> the key that needs to be removed </param>
		''' <returns> the previous value associated with {@code key}, or
		'''         {@code null} if there was no mapping for {@code key} </returns>
		''' <exception cref="NullPointerException"> if the specified key is null </exception>
		Public Overridable Function remove(ByVal key As Object) As V
			Return replaceNode(key, Nothing, Nothing)
		End Function

		''' <summary>
		''' Implementation for the four public remove/replace methods:
		''' Replaces node value with v, conditional upon match of cv if
		''' non-null.  If resulting value is null, delete.
		''' </summary>
		Friend Function replaceNode(ByVal key As Object, ByVal value As V, ByVal cv As Object) As V
			Dim hash As Integer = spread(key.GetHashCode())
			Dim tab As Node(Of K, V)() = table
			Do
				Dim f As Node(Of K, V)
				Dim n, i, fh As Integer
				n = tab.length
'JAVA TO VB CONVERTER TODO TASK: Assignments within expressions are not supported in VB
				f = tabAt(tab, i = (n - 1) And hash)
				If tab Is Nothing OrElse n = 0 OrElse f Is Nothing Then
					Exit Do
				Else
					fh = f.hash
					If fh = MOVED Then
						tab = helpTransfer(tab, f)
					Else
						Dim oldVal As V = Nothing
					End If
					Dim validated As Boolean = False
					SyncLock f
						If tabAt(tab, i) Is f Then
							If fh >= 0 Then
								validated = True
								Dim e As Node(Of K, V) = f
								Dim pred As Node(Of K, V) = Nothing
								Do
									Dim ek As K
									ek = e.key
									If e.hash = hash AndAlso (ek Is key OrElse (ek IsNot Nothing AndAlso key.Equals(ek))) Then
										Dim ev As V = e.val
										If cv Is Nothing OrElse cv Is ev OrElse (ev IsNot Nothing AndAlso cv.Equals(ev)) Then
											oldVal = ev
											If value IsNot Nothing Then
												e.val = value
											ElseIf pred IsNot Nothing Then
												pred.next = e.next
											Else
												tabAtbAt(tab, i, e.next)
											End If
										End If
										Exit Do
									End If
									pred = e
									e = e.next
									If e Is Nothing Then Exit Do
								Loop
							ElseIf TypeOf f Is TreeBin Then
								validated = True
								Dim t As TreeBin(Of K, V) = CType(f, TreeBin(Of K, V))
								Dim r As TreeNode(Of K, V), p As TreeNode(Of K, V)
								r = t.root
								p = r.findTreeNode(hash, key, Nothing)
								If r IsNot Nothing AndAlso p IsNot Nothing Then
									Dim pv As V = p.val
									If cv Is Nothing OrElse cv Is pv OrElse (pv IsNot Nothing AndAlso cv.Equals(pv)) Then
										oldVal = pv
										If value IsNot Nothing Then
											p.val = value
										ElseIf t.removeTreeNode(p) Then
											tabAtbAt(tab, i, untreeify(t.first))
										End If
									End If
								End If
							End If
						End If
					End SyncLock
					If validated Then
						If oldVal IsNot Nothing Then
							If value Is Nothing Then addCount(-1L, -1)
							Return oldVal
						End If
						Exit Do
					End If
					End If
			Loop
			Return Nothing
		End Function

		''' <summary>
		''' Removes all of the mappings from this map.
		''' </summary>
		Public Overridable Sub clear()
			Dim delta As Long = 0L ' negative number of deletions
			Dim i As Integer = 0
			Dim tab As Node(Of K, V)() = table
			Do While tab IsNot Nothing AndAlso i < tab.Length
				Dim fh As Integer
				Dim f As Node(Of K, V) = tabAt(tab, i)
				If f Is Nothing Then
					i += 1
				Else
					fh = f.hash
					If fh = MOVED Then
						tab = helpTransfer(tab, f)
						i = 0 ' restart
					Else
						SyncLock f
					End If
						If tabAt(tab, i) Is f Then
							Dim p As Node(Of K, V) = (If(fh >= 0, f, If(TypeOf f Is TreeBin, CType(f, TreeBin(Of K, V)).first, Nothing)))
							Do While p IsNot Nothing
								delta -= 1
								p = p.next
							Loop
							tabAtbAt(tab, i, Nothing)
							i += 1
						End If
						End SyncLock
					End If
			Loop
			If delta <> 0L Then addCount(delta, -1)
		End Sub

		''' <summary>
		''' Returns a <seealso cref="Set"/> view of the keys contained in this map.
		''' The set is backed by the map, so changes to the map are
		''' reflected in the set, and vice-versa. The set supports element
		''' removal, which removes the corresponding mapping from this map,
		''' via the {@code Iterator.remove}, {@code Set.remove},
		''' {@code removeAll}, {@code retainAll}, and {@code clear}
		''' operations.  It does not support the {@code add} or
		''' {@code addAll} operations.
		''' 
		''' <p>The view's iterators and spliterators are
		''' <a href="package-summary.html#Weakly"><i>weakly consistent</i></a>.
		''' 
		''' <p>The view's {@code spliterator} reports <seealso cref="Spliterator#CONCURRENT"/>,
		''' <seealso cref="Spliterator#DISTINCT"/>, and <seealso cref="Spliterator#NONNULL"/>.
		''' </summary>
		''' <returns> the set view </returns>
		Public Overridable Function keySet() As KeySetView(Of K, V)
			Dim ks As KeySetView(Of K, V)
				ks = keySet_Renamed
				If ks IsNot Nothing Then
					Return ks
				Else
'JAVA TO VB CONVERTER TODO TASK: Assignments within expressions are not supported in VB
					Return (keySet_Renamed = New KeySetView(Of K, V)(Me, Nothing))
				End If
		End Function

		''' <summary>
		''' Returns a <seealso cref="Collection"/> view of the values contained in this map.
		''' The collection is backed by the map, so changes to the map are
		''' reflected in the collection, and vice-versa.  The collection
		''' supports element removal, which removes the corresponding
		''' mapping from this map, via the {@code Iterator.remove},
		''' {@code Collection.remove}, {@code removeAll},
		''' {@code retainAll}, and {@code clear} operations.  It does not
		''' support the {@code add} or {@code addAll} operations.
		''' 
		''' <p>The view's iterators and spliterators are
		''' <a href="package-summary.html#Weakly"><i>weakly consistent</i></a>.
		''' 
		''' <p>The view's {@code spliterator} reports <seealso cref="Spliterator#CONCURRENT"/>
		''' and <seealso cref="Spliterator#NONNULL"/>.
		''' </summary>
		''' <returns> the collection view </returns>
		Public Overridable Function values() As ICollection(Of V)
			Dim vs As ValuesView(Of K, V)
				vs = values_Renamed
				If vs IsNot Nothing Then
					Return vs
				Else
'JAVA TO VB CONVERTER TODO TASK: Assignments within expressions are not supported in VB
					Return (values_Renamed = New ValuesView(Of K, V)(Me))
				End If
		End Function

		''' <summary>
		''' Returns a <seealso cref="Set"/> view of the mappings contained in this map.
		''' The set is backed by the map, so changes to the map are
		''' reflected in the set, and vice-versa.  The set supports element
		''' removal, which removes the corresponding mapping from the map,
		''' via the {@code Iterator.remove}, {@code Set.remove},
		''' {@code removeAll}, {@code retainAll}, and {@code clear}
		''' operations.
		''' 
		''' <p>The view's iterators and spliterators are
		''' <a href="package-summary.html#Weakly"><i>weakly consistent</i></a>.
		''' 
		''' <p>The view's {@code spliterator} reports <seealso cref="Spliterator#CONCURRENT"/>,
		''' <seealso cref="Spliterator#DISTINCT"/>, and <seealso cref="Spliterator#NONNULL"/>.
		''' </summary>
		''' <returns> the set view </returns>
		Public Overridable Function entrySet() As java.util.Set(Of KeyValuePair(Of K, V))
			Dim es As EntrySetView(Of K, V)
				es = entrySet_Renamed
				If es IsNot Nothing Then
					Return es
				Else
'JAVA TO VB CONVERTER TODO TASK: Assignments within expressions are not supported in VB
					Return (entrySet_Renamed = New EntrySetView(Of K, V)(Me))
				End If
		End Function

		''' <summary>
		''' Returns the hash code value for this <seealso cref="Map"/>, i.e.,
		''' the sum of, for each key-value pair in the map,
		''' {@code key.hashCode() ^ value.hashCode()}.
		''' </summary>
		''' <returns> the hash code value for this map </returns>
		Public Overrides Function GetHashCode() As Integer
			Dim h As Integer = 0
			Dim t As Node(Of K, V)()
			t = table
			If t IsNot Nothing Then
				Dim it As New Traverser(Of K, V)(t, t.Length, 0, t.Length)
				Dim p As Node(Of K, V)
'JAVA TO VB CONVERTER TODO TASK: Assignments within expressions are not supported in VB
				Do While (p = it.advance()) IsNot Nothing
					h += p.key.GetHashCode() Xor p.val.GetHashCode()
				Loop
			End If
			Return h
		End Function

		''' <summary>
		''' Returns a string representation of this map.  The string
		''' representation consists of a list of key-value mappings (in no
		''' particular order) enclosed in braces ("{@code {}}").  Adjacent
		''' mappings are separated by the characters {@code ", "} (comma
		''' and space).  Each key-value mapping is rendered as the key
		''' followed by an equals sign ("{@code =}") followed by the
		''' associated value.
		''' </summary>
		''' <returns> a string representation of this map </returns>
		Public Overrides Function ToString() As String
			Dim t As Node(Of K, V)()
'JAVA TO VB CONVERTER TODO TASK: Assignments within expressions are not supported in VB
			Dim f As Integer = If((t = table) Is Nothing, 0, t.Length)
			Dim it As New Traverser(Of K, V)(t, f, 0, f)
			Dim sb As New StringBuilder
			sb.append("{"c)
			Dim p As Node(Of K, V)
			p = it.advance()
			If p IsNot Nothing Then
				Do
					Dim k As K = p.key
					Dim v As V = p.val
					sb.append(If(k Is Me, "(this Map)", k))
					sb.append("="c)
					sb.append(If(v Is Me, "(this Map)", v))
					p = it.advance()
					If p Is Nothing Then Exit Do
					sb.append(","c).append(" "c)
				Loop
			End If
			Return sb.append("}"c).ToString()
		End Function

		''' <summary>
		''' Compares the specified object with this map for equality.
		''' Returns {@code true} if the given object is a map with the same
		''' mappings as this map.  This operation may return misleading
		''' results if either map is concurrently modified during execution
		''' of this method.
		''' </summary>
		''' <param name="o"> object to be compared for equality with this map </param>
		''' <returns> {@code true} if the specified object is equal to this map </returns>
		Public Overrides Function Equals(ByVal o As Object) As Boolean
			If o IsNot Me Then
				If Not(TypeOf o Is IDictionary) Then Return False
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
				Dim m As IDictionary(Of ?, ?) = CType(o, IDictionary(Of ?, ?))
				Dim t As Node(Of K, V)()
'JAVA TO VB CONVERTER TODO TASK: Assignments within expressions are not supported in VB
				Dim f As Integer = If((t = table) Is Nothing, 0, t.Length)
				Dim it As New Traverser(Of K, V)(t, f, 0, f)
				Dim p As Node(Of K, V)
'JAVA TO VB CONVERTER TODO TASK: Assignments within expressions are not supported in VB
				Do While (p = it.advance()) IsNot Nothing
					Dim val As V = p.val
					Dim v As Object = m(p.key)
					If v Is Nothing OrElse (v IsNot val AndAlso (Not v.Equals(val))) Then Return False
				Loop
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
				For Each e As KeyValuePair(Of ?, ?) In m
					Dim mk, mv, v As Object
					mk = e.Key
					mv = e.Value
					v = [get](mk)
					If mk Is Nothing OrElse mv Is Nothing OrElse v Is Nothing OrElse (mv IsNot v AndAlso (Not mv.Equals(v))) Then Return False
				Next e
			End If
			Return True
		End Function

		''' <summary>
		''' Stripped-down version of helper class used in previous version,
		''' declared for the sake of serialization compatibility
		''' </summary>
		<Serializable> _
		Friend Class Segment(Of K, V)
			Inherits java.util.concurrent.locks.ReentrantLock

			Private Const serialVersionUID As Long = 2249069246763182397L
			Friend ReadOnly loadFactor As Single
			Friend Sub New(ByVal lf As Single)
				Me.loadFactor = lf
			End Sub
		End Class

		''' <summary>
		''' Saves the state of the {@code ConcurrentHashMap} instance to a
		''' stream (i.e., serializes it). </summary>
		''' <param name="s"> the stream </param>
		''' <exception cref="java.io.IOException"> if an I/O error occurs
		''' @serialData
		''' the key (Object) and value (Object)
		''' for each key-value mapping, followed by a null pair.
		''' The key-value mappings are emitted in no particular order. </exception>
		Private Sub writeObject(ByVal s As java.io.ObjectOutputStream)
			' For serialization compatibility
			' Emulate segment calculation from previous version of this class
			Dim sshift As Integer = 0
			Dim ssize As Integer = 1
			Do While ssize < DEFAULT_CONCURRENCY_LEVEL
				sshift += 1
				ssize <<= 1
			Loop
			Dim segmentShift As Integer = 32 - sshift
			Dim segmentMask As Integer = ssize - 1
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
			Dim segments As Segment(Of K, V)() = CType(New Segment(Of ?, ?)(DEFAULT_CONCURRENCY_LEVEL - 1){}, Segment(Of K, V)())
			For i As Integer = 0 To segments.Length - 1
				segments(i) = New Segment(Of K, V)(LOAD_FACTOR)
			Next i
			s.putFields().put("segments", segments)
			s.putFields().put("segmentShift", segmentShift)
			s.putFields().put("segmentMask", segmentMask)
			s.writeFields()

			Dim t As Node(Of K, V)()
			t = table
			If t IsNot Nothing Then
				Dim it As New Traverser(Of K, V)(t, t.Length, 0, t.Length)
				Dim p As Node(Of K, V)
'JAVA TO VB CONVERTER TODO TASK: Assignments within expressions are not supported in VB
				Do While (p = it.advance()) IsNot Nothing
					s.writeObject(p.key)
					s.writeObject(p.val)
				Loop
			End If
			s.writeObject(Nothing)
			s.writeObject(Nothing)
			segments = Nothing ' throw away
		End Sub

		''' <summary>
		''' Reconstitutes the instance from a stream (that is, deserializes it). </summary>
		''' <param name="s"> the stream </param>
		''' <exception cref="ClassNotFoundException"> if the class of a serialized object
		'''         could not be found </exception>
		''' <exception cref="java.io.IOException"> if an I/O error occurs </exception>
		Private Sub readObject(ByVal s As java.io.ObjectInputStream)
	'        
	'         * To improve performance in typical cases, we create nodes
	'         * while reading, then place in table once size is known.
	'         * However, we must also validate uniqueness and deal with
	'         * overpopulated bins while doing so, which requires
	'         * specialized versions of putVal mechanics.
	'         
			sizeCtl = -1 ' force exclusion for table construction
			s.defaultReadObject()
			Dim size As Long = 0L
			Dim p As Node(Of K, V) = Nothing
			Do
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
				Dim k As K = CType(s.readObject(), K)
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
				Dim v As V = CType(s.readObject(), V)
				If k IsNot Nothing AndAlso v IsNot Nothing Then
					p = New Node(Of K, V)(spread(k.GetHashCode()), k, v, p)
					size += 1
				Else
					Exit Do
				End If
			Loop
			If size = 0L Then
				sizeCtl = 0
			Else
				Dim n As Integer
				If size >= CLng(CInt(CUInt(MAXIMUM_CAPACITY) >> 1)) Then
					n = MAXIMUM_CAPACITY
				Else
					Dim sz As Integer = CInt(size)
					n = tableSizeFor(sz + (CInt(CUInt(sz) >> 1)) + 1)
				End If
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
				Dim tab As Node(Of K, V)() = CType(New Node(Of ?, ?)(n - 1){}, Node(Of K, V)())
				Dim mask As Integer = n - 1
				Dim added As Long = 0L
				Do While p IsNot Nothing
					Dim insertAtFront As Boolean
					Dim [next] As Node(Of K, V) = p.next, first As Node(Of K, V)
					Dim h As Integer = p.hash, j As Integer = h And mask
					first = tabAt(tab, j)
					If first Is Nothing Then
						insertAtFront = True
					Else
						Dim k As K = p.key
						If first.hash < 0 Then
							Dim t As TreeBin(Of K, V) = CType(first, TreeBin(Of K, V))
							If t.putTreeVal(h, k, p.val) Is Nothing Then added += 1
							insertAtFront = False
						Else
							Dim binCount As Integer = 0
							insertAtFront = True
							Dim q As Node(Of K, V)
							Dim qk As K
							q = first
							Do While q IsNot Nothing
								qk = q.key
								If q.hash = h AndAlso (qk Is k OrElse (qk IsNot Nothing AndAlso k.Equals(qk))) Then
									insertAtFront = False
									Exit Do
								End If
								binCount += 1
								q = q.next
							Loop
							If insertAtFront AndAlso binCount >= TREEIFY_THRESHOLD Then
								insertAtFront = False
								added += 1
								p.next = first
								Dim hd As TreeNode(Of K, V) = Nothing, tl As TreeNode(Of K, V) = Nothing
								q = p
								Do While q IsNot Nothing
									Dim t As New TreeNode(Of K, V)(q.hash, q.key, q.val, Nothing, Nothing)
									t.prev = tl
									If t.prev Is Nothing Then
										hd = t
									Else
										tl.next = t
									End If
									tl = t
									q = q.next
								Loop
								tabAtbAt(tab, j, New TreeBin(Of K, V)(hd))
							End If
						End If
					End If
					If insertAtFront Then
						added += 1
						p.next = first
						tabAtbAt(tab, j, p)
					End If
					p = [next]
				Loop
				table = tab
				sizeCtl = n - (CInt(CUInt(n) >> 2))
				baseCount = added
			End If
		End Sub

		' ConcurrentMap methods

		''' <summary>
		''' {@inheritDoc}
		''' </summary>
		''' <returns> the previous value associated with the specified key,
		'''         or {@code null} if there was no mapping for the key </returns>
		''' <exception cref="NullPointerException"> if the specified key or value is null </exception>
		Public Overridable Function putIfAbsent(ByVal key As K, ByVal value As V) As V
			Return putVal(key, value, True)
		End Function

		''' <summary>
		''' {@inheritDoc}
		''' </summary>
		''' <exception cref="NullPointerException"> if the specified key is null </exception>
		Public Overridable Function remove(ByVal key As Object, ByVal value As Object) As Boolean
			If key Is Nothing Then Throw New NullPointerException
			Return value IsNot Nothing AndAlso replaceNode(key, Nothing, value) IsNot Nothing
		End Function

		''' <summary>
		''' {@inheritDoc}
		''' </summary>
		''' <exception cref="NullPointerException"> if any of the arguments are null </exception>
		Public Overridable Function replace(ByVal key As K, ByVal oldValue As V, ByVal newValue As V) As Boolean
			If key Is Nothing OrElse oldValue Is Nothing OrElse newValue Is Nothing Then Throw New NullPointerException
			Return replaceNode(key, newValue, oldValue) IsNot Nothing
		End Function

		''' <summary>
		''' {@inheritDoc}
		''' </summary>
		''' <returns> the previous value associated with the specified key,
		'''         or {@code null} if there was no mapping for the key </returns>
		''' <exception cref="NullPointerException"> if the specified key or value is null </exception>
		Public Overridable Function replace(ByVal key As K, ByVal value As V) As V
			If key Is Nothing OrElse value Is Nothing Then Throw New NullPointerException
			Return replaceNode(key, value, Nothing)
		End Function

		' Overrides of JDK8+ Map extension method defaults

		''' <summary>
		''' Returns the value to which the specified key is mapped, or the
		''' given default value if this map contains no mapping for the
		''' key.
		''' </summary>
		''' <param name="key"> the key whose associated value is to be returned </param>
		''' <param name="defaultValue"> the value to return if this map contains
		''' no mapping for the given key </param>
		''' <returns> the mapping for the key, if present; else the default value </returns>
		''' <exception cref="NullPointerException"> if the specified key is null </exception>
		Public Overridable Function getOrDefault(ByVal key As Object, ByVal defaultValue As V) As V
			Dim v As V
'JAVA TO VB CONVERTER TODO TASK: Assignments within expressions are not supported in VB
			Return If((v = [get](key)) Is Nothing, defaultValue, v)
		End Function

'JAVA TO VB CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
		Public Overridable Sub forEach(Of T1)(ByVal action As java.util.function.BiConsumer(Of T1))
			If action Is Nothing Then Throw New NullPointerException
			Dim t As Node(Of K, V)()
			t = table
			If t IsNot Nothing Then
				Dim it As New Traverser(Of K, V)(t, t.Length, 0, t.Length)
				Dim p As Node(Of K, V)
'JAVA TO VB CONVERTER TODO TASK: Assignments within expressions are not supported in VB
				Do While (p = it.advance()) IsNot Nothing
					action.accept(p.key, p.val)
				Loop
			End If
		End Sub

'JAVA TO VB CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
		Public Overridable Sub replaceAll(Of T1 As V)(ByVal [function] As java.util.function.BiFunction(Of T1))
			If [function] Is Nothing Then Throw New NullPointerException
			Dim t As Node(Of K, V)()
			t = table
			If t IsNot Nothing Then
				Dim it As New Traverser(Of K, V)(t, t.Length, 0, t.Length)
				Dim p As Node(Of K, V)
'JAVA TO VB CONVERTER TODO TASK: Assignments within expressions are not supported in VB
				Do While (p = it.advance()) IsNot Nothing
					Dim oldValue As V = p.val
					Dim key As K = p.key
					Do
						Dim newValue As V = [function].apply(key, oldValue)
						If newValue Is Nothing Then Throw New NullPointerException
						oldValue = [get](key)
						If replaceNode(key, newValue, oldValue) IsNot Nothing OrElse oldValue Is Nothing Then Exit Do
					Loop
				Loop
			End If
		End Sub

		''' <summary>
		''' If the specified key is not already associated with a value,
		''' attempts to compute its value using the given mapping function
		''' and enters it into this map unless {@code null}.  The entire
		''' method invocation is performed atomically, so the function is
		''' applied at most once per key.  Some attempted update operations
		''' on this map by other threads may be blocked while computation
		''' is in progress, so the computation should be short and simple,
		''' and must not attempt to update any other mappings of this map.
		''' </summary>
		''' <param name="key"> key with which the specified value is to be associated </param>
		''' <param name="mappingFunction"> the function to compute a value </param>
		''' <returns> the current (existing or computed) value associated with
		'''         the specified key, or null if the computed value is null </returns>
		''' <exception cref="NullPointerException"> if the specified key or mappingFunction
		'''         is null </exception>
		''' <exception cref="IllegalStateException"> if the computation detectably
		'''         attempts a recursive update to this map that would
		'''         otherwise never complete </exception>
		''' <exception cref="RuntimeException"> or Error if the mappingFunction does so,
		'''         in which case the mapping is left unestablished </exception>
'JAVA TO VB CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
		Public Overridable Function computeIfAbsent(Of T1 As V)(ByVal key As K, ByVal mappingFunction As java.util.function.Function(Of T1)) As V
			If key Is Nothing OrElse mappingFunction Is Nothing Then Throw New NullPointerException
			Dim h As Integer = spread(key.GetHashCode())
			Dim val As V = Nothing
			Dim binCount As Integer = 0
			Dim tab As Node(Of K, V)() = table
			Do
				Dim f As Node(Of K, V)
				Dim n, i, fh As Integer
				n = tab.length
				If tab Is Nothing OrElse n = 0 Then
					tab = initTable()
				Else
'JAVA TO VB CONVERTER TODO TASK: Assignments within expressions are not supported in VB
					f = tabAt(tab, i = (n - 1) And h)
					If f Is Nothing Then
						Dim r As Node(Of K, V) = New ReservationNode(Of K, V)
						SyncLock r
							If casTabAt(tab, i, Nothing, r) Then
								binCount = 1
								Dim node As Node(Of K, V) = Nothing
								Try
									val = mappingFunction.apply(key)
									If val IsNot Nothing Then node = New Node(Of K, V)(h, key, val, Nothing)
								Finally
									tabAtbAt(tab, i, node)
								End Try
							End If
						End SyncLock
						If binCount <> 0 Then Exit Do
					Else
						fh = f.hash
						If fh = MOVED Then
							tab = helpTransfer(tab, f)
						Else
							Dim added As Boolean = False
						End If
					End If
					SyncLock f
						If tabAt(tab, i) Is f Then
							If fh >= 0 Then
								binCount = 1
								Dim e As Node(Of K, V) = f
								Do
									Dim ek As K
									Dim ev As V
									ek = e.key
									If e.hash = h AndAlso (ek Is key OrElse (ek IsNot Nothing AndAlso key.Equals(ek))) Then
										val = e.val
										Exit Do
									End If
									Dim pred As Node(Of K, V) = e
									e = e.next
									If e Is Nothing Then
										val = mappingFunction.apply(key)
										If val IsNot Nothing Then
											added = True
											pred.next = New Node(Of K, V)(h, key, val, Nothing)
										End If
										Exit Do
									End If
									binCount += 1
								Loop
							ElseIf TypeOf f Is TreeBin Then
								binCount = 2
								Dim t As TreeBin(Of K, V) = CType(f, TreeBin(Of K, V))
								Dim r As TreeNode(Of K, V), p As TreeNode(Of K, V)
								r = t.root
								p = r.findTreeNode(h, key, Nothing)
								If r IsNot Nothing AndAlso p IsNot Nothing Then
									val = p.val
								Else
									val = mappingFunction.apply(key)
									If val IsNot Nothing Then
										added = True
										t.putTreeVal(h, key, val)
									End If
									End If
							End If
						End If
					End SyncLock
					If binCount <> 0 Then
						If binCount >= TREEIFY_THRESHOLD Then treeifyBin(tab, i)
						If Not added Then Return val
						Exit Do
					End If
						End If
			Loop
			If val IsNot Nothing Then addCount(1L, binCount)
			Return val
		End Function

		''' <summary>
		''' If the value for the specified key is present, attempts to
		''' compute a new mapping given the key and its current mapped
		''' value.  The entire method invocation is performed atomically.
		''' Some attempted update operations on this map by other threads
		''' may be blocked while computation is in progress, so the
		''' computation should be short and simple, and must not attempt to
		''' update any other mappings of this map.
		''' </summary>
		''' <param name="key"> key with which a value may be associated </param>
		''' <param name="remappingFunction"> the function to compute a value </param>
		''' <returns> the new value associated with the specified key, or null if none </returns>
		''' <exception cref="NullPointerException"> if the specified key or remappingFunction
		'''         is null </exception>
		''' <exception cref="IllegalStateException"> if the computation detectably
		'''         attempts a recursive update to this map that would
		'''         otherwise never complete </exception>
		''' <exception cref="RuntimeException"> or Error if the remappingFunction does so,
		'''         in which case the mapping is unchanged </exception>
'JAVA TO VB CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
		Public Overridable Function computeIfPresent(Of T1 As V)(ByVal key As K, ByVal remappingFunction As java.util.function.BiFunction(Of T1)) As V
			If key Is Nothing OrElse remappingFunction Is Nothing Then Throw New NullPointerException
			Dim h As Integer = spread(key.GetHashCode())
			Dim val As V = Nothing
			Dim delta As Integer = 0
			Dim binCount As Integer = 0
			Dim tab As Node(Of K, V)() = table
			Do
				Dim f As Node(Of K, V)
				Dim n, i, fh As Integer
				n = tab.length
				If tab Is Nothing OrElse n = 0 Then
					tab = initTable()
				Else
'JAVA TO VB CONVERTER TODO TASK: Assignments within expressions are not supported in VB
					f = tabAt(tab, i = (n - 1) And h)
					If f Is Nothing Then
						Exit Do
					Else
						fh = f.hash
						If fh = MOVED Then
							tab = helpTransfer(tab, f)
						Else
							SyncLock f
						End If
					End If
						If tabAt(tab, i) Is f Then
							If fh >= 0 Then
								binCount = 1
								Dim e As Node(Of K, V) = f
								Dim pred As Node(Of K, V) = Nothing
								Do
									Dim ek As K
									ek = e.key
									If e.hash = h AndAlso (ek Is key OrElse (ek IsNot Nothing AndAlso key.Equals(ek))) Then
										val = remappingFunction.apply(key, e.val)
										If val IsNot Nothing Then
											e.val = val
										Else
											delta = -1
											Dim en As Node(Of K, V) = e.next
											If pred IsNot Nothing Then
												pred.next = en
											Else
												tabAtbAt(tab, i, en)
											End If
										End If
										Exit Do
									End If
									pred = e
									e = e.next
									If e Is Nothing Then Exit Do
									binCount += 1
								Loop
							ElseIf TypeOf f Is TreeBin Then
								binCount = 2
								Dim t As TreeBin(Of K, V) = CType(f, TreeBin(Of K, V))
								Dim r As TreeNode(Of K, V), p As TreeNode(Of K, V)
								r = t.root
								p = r.findTreeNode(h, key, Nothing)
								If r IsNot Nothing AndAlso p IsNot Nothing Then
									val = remappingFunction.apply(key, p.val)
									If val IsNot Nothing Then
										p.val = val
									Else
										delta = -1
										If t.removeTreeNode(p) Then tabAtbAt(tab, i, untreeify(t.first))
									End If
								End If
							End If
						End If
							End SyncLock
					If binCount <> 0 Then Exit Do
						End If
			Loop
			If delta <> 0 Then addCount(CLng(delta), binCount)
			Return val
		End Function

		''' <summary>
		''' Attempts to compute a mapping for the specified key and its
		''' current mapped value (or {@code null} if there is no current
		''' mapping). The entire method invocation is performed atomically.
		''' Some attempted update operations on this map by other threads
		''' may be blocked while computation is in progress, so the
		''' computation should be short and simple, and must not attempt to
		''' update any other mappings of this Map.
		''' </summary>
		''' <param name="key"> key with which the specified value is to be associated </param>
		''' <param name="remappingFunction"> the function to compute a value </param>
		''' <returns> the new value associated with the specified key, or null if none </returns>
		''' <exception cref="NullPointerException"> if the specified key or remappingFunction
		'''         is null </exception>
		''' <exception cref="IllegalStateException"> if the computation detectably
		'''         attempts a recursive update to this map that would
		'''         otherwise never complete </exception>
		''' <exception cref="RuntimeException"> or Error if the remappingFunction does so,
		'''         in which case the mapping is unchanged </exception>
'JAVA TO VB CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
		Public Overridable Function compute(Of T1 As V)(ByVal key As K, ByVal remappingFunction As java.util.function.BiFunction(Of T1)) As V
			If key Is Nothing OrElse remappingFunction Is Nothing Then Throw New NullPointerException
			Dim h As Integer = spread(key.GetHashCode())
			Dim val As V = Nothing
			Dim delta As Integer = 0
			Dim binCount As Integer = 0
			Dim tab As Node(Of K, V)() = table
			Do
				Dim f As Node(Of K, V)
				Dim n, i, fh As Integer
				n = tab.length
				If tab Is Nothing OrElse n = 0 Then
					tab = initTable()
				Else
'JAVA TO VB CONVERTER TODO TASK: Assignments within expressions are not supported in VB
					f = tabAt(tab, i = (n - 1) And h)
					If f Is Nothing Then
						Dim r As Node(Of K, V) = New ReservationNode(Of K, V)
						SyncLock r
							If casTabAt(tab, i, Nothing, r) Then
								binCount = 1
								Dim node As Node(Of K, V) = Nothing
								Try
									val = remappingFunction.apply(key, Nothing)
									If val IsNot Nothing Then
										delta = 1
										node = New Node(Of K, V)(h, key, val, Nothing)
									End If
								Finally
									tabAtbAt(tab, i, node)
								End Try
							End If
						End SyncLock
						If binCount <> 0 Then Exit Do
					Else
						fh = f.hash
						If fh = MOVED Then
							tab = helpTransfer(tab, f)
						Else
							SyncLock f
						End If
					End If
						If tabAt(tab, i) Is f Then
							If fh >= 0 Then
								binCount = 1
								Dim e As Node(Of K, V) = f
								Dim pred As Node(Of K, V) = Nothing
								Do
									Dim ek As K
									ek = e.key
									If e.hash = h AndAlso (ek Is key OrElse (ek IsNot Nothing AndAlso key.Equals(ek))) Then
										val = remappingFunction.apply(key, e.val)
										If val IsNot Nothing Then
											e.val = val
										Else
											delta = -1
											Dim en As Node(Of K, V) = e.next
											If pred IsNot Nothing Then
												pred.next = en
											Else
												tabAtbAt(tab, i, en)
											End If
										End If
										Exit Do
									End If
									pred = e
									e = e.next
									If e Is Nothing Then
										val = remappingFunction.apply(key, Nothing)
										If val IsNot Nothing Then
											delta = 1
											pred.next = New Node(Of K, V)(h, key, val, Nothing)
										End If
										Exit Do
									End If
									binCount += 1
								Loop
							ElseIf TypeOf f Is TreeBin Then
								binCount = 1
								Dim t As TreeBin(Of K, V) = CType(f, TreeBin(Of K, V))
								Dim r As TreeNode(Of K, V), p As TreeNode(Of K, V)
								r = t.root
								If r IsNot Nothing Then
									p = r.findTreeNode(h, key, Nothing)
								Else
									p = Nothing
								End If
								Dim pv As V = If(p Is Nothing, Nothing, p.val)
								val = remappingFunction.apply(key, pv)
								If val IsNot Nothing Then
									If p IsNot Nothing Then
										p.val = val
									Else
										delta = 1
										t.putTreeVal(h, key, val)
									End If
								ElseIf p IsNot Nothing Then
									delta = -1
									If t.removeTreeNode(p) Then tabAtbAt(tab, i, untreeify(t.first))
								End If
							End If
						End If
							End SyncLock
					If binCount <> 0 Then
						If binCount >= TREEIFY_THRESHOLD Then treeifyBin(tab, i)
						Exit Do
					End If
						End If
			Loop
			If delta <> 0 Then addCount(CLng(delta), binCount)
			Return val
		End Function

		''' <summary>
		''' If the specified key is not already associated with a
		''' (non-null) value, associates it with the given value.
		''' Otherwise, replaces the value with the results of the given
		''' remapping function, or removes if {@code null}. The entire
		''' method invocation is performed atomically.  Some attempted
		''' update operations on this map by other threads may be blocked
		''' while computation is in progress, so the computation should be
		''' short and simple, and must not attempt to update any other
		''' mappings of this Map.
		''' </summary>
		''' <param name="key"> key with which the specified value is to be associated </param>
		''' <param name="value"> the value to use if absent </param>
		''' <param name="remappingFunction"> the function to recompute a value if present </param>
		''' <returns> the new value associated with the specified key, or null if none </returns>
		''' <exception cref="NullPointerException"> if the specified key or the
		'''         remappingFunction is null </exception>
		''' <exception cref="RuntimeException"> or Error if the remappingFunction does so,
		'''         in which case the mapping is unchanged </exception>
'JAVA TO VB CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
		Public Overridable Function merge(Of T1 As V)(ByVal key As K, ByVal value As V, ByVal remappingFunction As java.util.function.BiFunction(Of T1)) As V
			If key Is Nothing OrElse value Is Nothing OrElse remappingFunction Is Nothing Then Throw New NullPointerException
			Dim h As Integer = spread(key.GetHashCode())
			Dim val As V = Nothing
			Dim delta As Integer = 0
			Dim binCount As Integer = 0
			Dim tab As Node(Of K, V)() = table
			Do
				Dim f As Node(Of K, V)
				Dim n, i, fh As Integer
				n = tab.length
				If tab Is Nothing OrElse n = 0 Then
					tab = initTable()
				Else
'JAVA TO VB CONVERTER TODO TASK: Assignments within expressions are not supported in VB
					f = tabAt(tab, i = (n - 1) And h)
					If f Is Nothing Then
						If casTabAt(tab, i, Nothing, New Node(Of K, V)(h, key, value, Nothing)) Then
							delta = 1
							val = value
							Exit Do
						End If
					Else
						fh = f.hash
						If fh = MOVED Then
							tab = helpTransfer(tab, f)
						Else
							SyncLock f
						End If
					End If
						If tabAt(tab, i) Is f Then
							If fh >= 0 Then
								binCount = 1
								Dim e As Node(Of K, V) = f
								Dim pred As Node(Of K, V) = Nothing
								Do
									Dim ek As K
									ek = e.key
									If e.hash = h AndAlso (ek Is key OrElse (ek IsNot Nothing AndAlso key.Equals(ek))) Then
										val = remappingFunction.apply(e.val, value)
										If val IsNot Nothing Then
											e.val = val
										Else
											delta = -1
											Dim en As Node(Of K, V) = e.next
											If pred IsNot Nothing Then
												pred.next = en
											Else
												tabAtbAt(tab, i, en)
											End If
										End If
										Exit Do
									End If
									pred = e
									e = e.next
									If e Is Nothing Then
										delta = 1
										val = value
										pred.next = New Node(Of K, V)(h, key, val, Nothing)
										Exit Do
									End If
									binCount += 1
								Loop
							ElseIf TypeOf f Is TreeBin Then
								binCount = 2
								Dim t As TreeBin(Of K, V) = CType(f, TreeBin(Of K, V))
								Dim r As TreeNode(Of K, V) = t.root
								Dim p As TreeNode(Of K, V) = If(r Is Nothing, Nothing, r.findTreeNode(h, key, Nothing))
								val = If(p Is Nothing, value, remappingFunction.apply(p.val, value))
								If val IsNot Nothing Then
									If p IsNot Nothing Then
										p.val = val
									Else
										delta = 1
										t.putTreeVal(h, key, val)
									End If
								ElseIf p IsNot Nothing Then
									delta = -1
									If t.removeTreeNode(p) Then tabAtbAt(tab, i, untreeify(t.first))
								End If
							End If
						End If
							End SyncLock
					If binCount <> 0 Then
						If binCount >= TREEIFY_THRESHOLD Then treeifyBin(tab, i)
						Exit Do
					End If
						End If
			Loop
			If delta <> 0 Then addCount(CLng(delta), binCount)
			Return val
		End Function

		' Hashtable legacy methods

		''' <summary>
		''' Legacy method testing if some key maps into the specified value
		''' in this table.  This method is identical in functionality to
		''' <seealso cref="#containsValue(Object)"/>, and exists solely to ensure
		''' full compatibility with class <seealso cref="java.util.Hashtable"/>,
		''' which supported this method prior to introduction of the
		''' Java Collections framework.
		''' </summary>
		''' <param name="value"> a value to search for </param>
		''' <returns> {@code true} if and only if some key maps to the
		'''         {@code value} argument in this table as
		'''         determined by the {@code equals} method;
		'''         {@code false} otherwise </returns>
		''' <exception cref="NullPointerException"> if the specified value is null </exception>
		Public Overridable Function contains(ByVal value As Object) As Boolean
			Return containsValue(value)
		End Function

		''' <summary>
		''' Returns an enumeration of the keys in this table.
		''' </summary>
		''' <returns> an enumeration of the keys in this table </returns>
		''' <seealso cref= #keySet() </seealso>
		Public Overridable Function keys() As System.Collections.IEnumerator(Of K)
			Dim t As Node(Of K, V)()
'JAVA TO VB CONVERTER TODO TASK: Assignments within expressions are not supported in VB
			Dim f As Integer = If((t = table) Is Nothing, 0, t.Length)
			Return New KeyIterator(Of K, V)(t, f, 0, f, Me)
		End Function

		''' <summary>
		''' Returns an enumeration of the values in this table.
		''' </summary>
		''' <returns> an enumeration of the values in this table </returns>
		''' <seealso cref= #values() </seealso>
		Public Overridable Function elements() As System.Collections.IEnumerator(Of V)
			Dim t As Node(Of K, V)()
'JAVA TO VB CONVERTER TODO TASK: Assignments within expressions are not supported in VB
			Dim f As Integer = If((t = table) Is Nothing, 0, t.Length)
			Return New ValueIterator(Of K, V)(t, f, 0, f, Me)
		End Function

		' ConcurrentHashMap-only methods

		''' <summary>
		''' Returns the number of mappings. This method should be used
		''' instead of <seealso cref="#size"/> because a ConcurrentHashMap may
		''' contain more mappings than can be represented as an int. The
		''' value returned is an estimate; the actual count may differ if
		''' there are concurrent insertions or removals.
		''' </summary>
		''' <returns> the number of mappings
		''' @since 1.8 </returns>
		Public Overridable Function mappingCount() As Long
			Dim n As Long = sumCount()
			Return If(n < 0L, 0L, n) ' ignore transient negative values
		End Function

		''' <summary>
		''' Creates a new <seealso cref="Set"/> backed by a ConcurrentHashMap
		''' from the given type to {@code  java.lang.[Boolean].TRUE}.
		''' </summary>
		''' @param <K> the element type of the returned set </param>
		''' <returns> the new set
		''' @since 1.8 </returns>
		Public Shared Function newKeySet(Of K)() As KeySetView(Of K, Boolean?)
			Return New KeySetView(Of K, Boolean?) (New ConcurrentHashMap(Of K, Boolean?),  java.lang.[Boolean].TRUE)
		End Function

		''' <summary>
		''' Creates a new <seealso cref="Set"/> backed by a ConcurrentHashMap
		''' from the given type to {@code  java.lang.[Boolean].TRUE}.
		''' </summary>
		''' <param name="initialCapacity"> The implementation performs internal
		''' sizing to accommodate this many elements. </param>
		''' @param <K> the element type of the returned set </param>
		''' <returns> the new set </returns>
		''' <exception cref="IllegalArgumentException"> if the initial capacity of
		''' elements is negative
		''' @since 1.8 </exception>
		Public Shared Function newKeySet(Of K)(ByVal initialCapacity As Integer) As KeySetView(Of K, Boolean?)
			Return New KeySetView(Of K, Boolean?) (New ConcurrentHashMap(Of K, Boolean?)(initialCapacity),  java.lang.[Boolean].TRUE)
		End Function

		''' <summary>
		''' Returns a <seealso cref="Set"/> view of the keys in this map, using the
		''' given common mapped value for any additions (i.e., {@link
		''' Collection#add} and <seealso cref="Collection#addAll(Collection)"/>).
		''' This is of course only appropriate if it is acceptable to use
		''' the same value for all additions from this view.
		''' </summary>
		''' <param name="mappedValue"> the mapped value to use for any additions </param>
		''' <returns> the set view </returns>
		''' <exception cref="NullPointerException"> if the mappedValue is null </exception>
		Public Overridable Function keySet(ByVal mappedValue As V) As KeySetView(Of K, V)
			If mappedValue Is Nothing Then Throw New NullPointerException
			Return New KeySetView(Of K, V)(Me, mappedValue)
		End Function

		' ---------------- Special Nodes -------------- 

		''' <summary>
		''' A node inserted at head of bins during transfer operations.
		''' </summary>
		Friend NotInheritable Class ForwardingNode(Of K, V)
			Inherits Node(Of K, V)

			Friend ReadOnly nextTable As Node(Of K, V)()
			Friend Sub New(ByVal tab As Node(Of K, V)())
				MyBase.New(MOVED, Nothing, Nothing, Nothing)
				Me.nextTable = tab
			End Sub

			Friend Function find(ByVal h As Integer, ByVal k As Object) As Node(Of K, V)
				' loop to avoid arbitrarily deep recursion on forwarding nodes
				outer:
				Dim tab As Node(Of K, V)() = nextTable
				Do
					Dim e As Node(Of K, V)
					Dim n As Integer
					n = tab.length
					e = tabAt(tab, (n - 1) And h)
					If k Is Nothing OrElse tab Is Nothing OrElse n = 0 OrElse e Is Nothing Then Return Nothing
					Do
						Dim eh As Integer
						Dim ek As K
						eh = e.hash
						ek = e.key
						If eh = h AndAlso (ek Is k OrElse (ek IsNot Nothing AndAlso k.Equals(ek))) Then Return e
						If eh < 0 Then
							If TypeOf e Is ForwardingNode Then
								tab = CType(e, ForwardingNode(Of K, V)).nextTable
								GoTo outer
							Else
								Return e.find(h, k)
							End If
						End If
						e = e.next
						If e Is Nothing Then Return Nothing
					Loop
				Loop
			End Function
		End Class

		''' <summary>
		''' A place-holder node used in computeIfAbsent and compute
		''' </summary>
		Friend NotInheritable Class ReservationNode(Of K, V)
			Inherits Node(Of K, V)

			Friend Sub New()
				MyBase.New(RESERVED, Nothing, Nothing, Nothing)
			End Sub

			Friend Function find(ByVal h As Integer, ByVal k As Object) As Node(Of K, V)
				Return Nothing
			End Function
		End Class

		' ---------------- Table Initialization and Resizing -------------- 

		''' <summary>
		''' Returns the stamp bits for resizing a table of size n.
		''' Must be negative when shifted left by RESIZE_STAMP_SHIFT.
		''' </summary>
		Friend Shared Function resizeStamp(ByVal n As Integer) As Integer
			Return  java.lang.[Integer].numberOfLeadingZeros(n) Or (1 << (RESIZE_STAMP_BITS - 1))
		End Function

		''' <summary>
		''' Initializes table, using the size recorded in sizeCtl.
		''' </summary>
		Private Function initTable() As Node(Of K, V)()
			Dim tab As Node(Of K, V)()
			Dim sc As Integer
			tab = table
			Do While tab Is Nothing OrElse tab.Length = 0
				sc = sizeCtl
				If sc < 0 Then
					Thread.yield() ' lost initialization race; just spin
				ElseIf U.compareAndSwapInt(Me, SIZECTL, sc, -1) Then
					Try
						tab = table
						If tab Is Nothing OrElse tab.Length = 0 Then
							Dim n As Integer = If(sc > 0, sc, DEFAULT_CAPACITY)
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
							Dim nt As Node(Of K, V)() = CType(New Node(Of ?, ?)(n - 1){}, Node(Of K, V)())
								tab = nt
								table = tab
							sc = n - (CInt(CUInt(n) >> 2))
						End If
					Finally
						sizeCtl = sc
					End Try
					Exit Do
				End If
				tab = table
			Loop
			Return tab
		End Function

		''' <summary>
		''' Adds to count, and if table is too small and not already
		''' resizing, initiates transfer. If already resizing, helps
		''' perform transfer if work is available.  Rechecks occupancy
		''' after a transfer to see if another resize is already needed
		''' because resizings are lagging additions.
		''' </summary>
		''' <param name="x"> the count to add </param>
		''' <param name="check"> if <0, don't check resize, if <= 1 only check if uncontended </param>
		Private Sub addCount(ByVal x As Long, ByVal check As Integer)
			Dim [as] As CounterCell()
			Dim b, s As Long
			[as] = counterCells
'JAVA TO VB CONVERTER TODO TASK: Assignments within expressions are not supported in VB
			If [as] IsNot Nothing OrElse (Not U.compareAndSwapLong(Me, BASECOUNT, b = baseCount, s = b + x)) Then
				Dim a As CounterCell
				Dim v As Long
				Dim m As Integer
				Dim uncontended As Boolean = True
				m = [as].Length - 1
				a = [as](ThreadLocalRandom.probe And m)
'JAVA TO VB CONVERTER TODO TASK: Assignments within expressions are not supported in VB
				uncontended = U.compareAndSwapLong(a, CELLVALUE, v = a.value, v + x)
				If [as] Is Nothing OrElse m < 0 OrElse a Is Nothing OrElse (Not uncontended) Then
					fullAddCount(x, uncontended)
					Return
				End If
				If check <= 1 Then Return
				s = sumCount()
			End If
			If check >= 0 Then
				Dim tab As Node(Of K, V)(), nt As Node(Of K, V)()
				Dim n, sc As Integer
				sc = sizeCtl
				tab = table
				n = tab.Length
				Do While s >= CLng(sc) AndAlso tab IsNot Nothing AndAlso n < MAXIMUM_CAPACITY
					Dim rs As Integer = resizeStamp(n)
					If sc < 0 Then
						nt = nextTable
						If (CInt(CUInt(sc) >> RESIZE_STAMP_SHIFT)) <> rs OrElse sc = rs + 1 OrElse sc = rs + MAX_RESIZERS OrElse nt Is Nothing OrElse transferIndex <= 0 Then Exit Do
						If U.compareAndSwapInt(Me, SIZECTL, sc, sc + 1) Then transfer(tab, nt)
					ElseIf U.compareAndSwapInt(Me, SIZECTL, sc, (rs << RESIZE_STAMP_SHIFT) + 2) Then
						transfer(tab, Nothing)
					End If
					s = sumCount()
					sc = sizeCtl
					tab = table
					n = tab.Length
				Loop
			End If
		End Sub

		''' <summary>
		''' Helps transfer if a resize is in progress.
		''' </summary>
		Friend Function helpTransfer(ByVal tab As Node(Of K, V)(), ByVal f As Node(Of K, V)) As Node(Of K, V)()
			Dim nextTab As Node(Of K, V)()
			Dim sc As Integer
			nextTab = CType(f, ForwardingNode(Of K, V)).nextTable
			If tab IsNot Nothing AndAlso (TypeOf f Is ForwardingNode) AndAlso nextTab IsNot Nothing Then
				Dim rs As Integer = resizeStamp(tab.Length)
				sc = sizeCtl
				Do While nextTab = nextTable AndAlso table = tab AndAlso sc < 0
					If (CInt(CUInt(sc) >> RESIZE_STAMP_SHIFT)) <> rs OrElse sc = rs + 1 OrElse sc = rs + MAX_RESIZERS OrElse transferIndex <= 0 Then Exit Do
					If U.compareAndSwapInt(Me, SIZECTL, sc, sc + 1) Then
						transfer(tab, nextTab)
						Exit Do
					End If
					sc = sizeCtl
				Loop
				Return nextTab
			End If
			Return table
		End Function

		''' <summary>
		''' Tries to presize table to accommodate the given number of elements.
		''' </summary>
		''' <param name="size"> number of elements (doesn't need to be perfectly accurate) </param>
		Private Sub tryPresize(ByVal size As Integer)
			Dim c As Integer = If(size >= (CInt(CUInt(MAXIMUM_CAPACITY) >> 1)), MAXIMUM_CAPACITY, tableSizeFor(size + (CInt(CUInt(size) >> 1)) + 1))
			Dim sc As Integer
			sc = sizeCtl
			Do While sc >= 0
				Dim tab As Node(Of K, V)() = table
				Dim n As Integer
				n = tab.Length
				If tab Is Nothing OrElse n = 0 Then
					n = If(sc > c, sc, c)
					If U.compareAndSwapInt(Me, SIZECTL, sc, -1) Then
						Try
							If table = tab Then
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
								Dim nt As Node(Of K, V)() = CType(New Node(Of ?, ?)(n - 1){}, Node(Of K, V)())
								table = nt
								sc = n - (CInt(CUInt(n) >> 2))
							End If
						Finally
							sizeCtl = sc
						End Try
					End If
				ElseIf c <= sc OrElse n >= MAXIMUM_CAPACITY Then
					Exit Do
				ElseIf tab = table Then
					Dim rs As Integer = resizeStamp(n)
					If sc < 0 Then
						Dim nt As Node(Of K, V)()
						nt = nextTable
						If (CInt(CUInt(sc) >> RESIZE_STAMP_SHIFT)) <> rs OrElse sc = rs + 1 OrElse sc = rs + MAX_RESIZERS OrElse nt Is Nothing OrElse transferIndex <= 0 Then Exit Do
						If U.compareAndSwapInt(Me, SIZECTL, sc, sc + 1) Then transfer(tab, nt)
					ElseIf U.compareAndSwapInt(Me, SIZECTL, sc, (rs << RESIZE_STAMP_SHIFT) + 2) Then
						transfer(tab, Nothing)
					End If
				End If
				sc = sizeCtl
			Loop
		End Sub

		''' <summary>
		''' Moves and/or copies the nodes in each bin to new table. See
		''' above for explanation.
		''' </summary>
		Private Sub transfer(ByVal tab As Node(Of K, V)(), ByVal nextTab As Node(Of K, V)())
			Dim n As Integer = tab.Length, stride As Integer
			stride = If(NCPU > 1, (CInt(CUInt(n) >> 3)) \ NCPU, n)
			If stride < MIN_TRANSFER_STRIDE Then stride = MIN_TRANSFER_STRIDE ' subdivide range
			If nextTab Is Nothing Then ' initiating
				Try
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
					Dim nt As Node(Of K, V)() = CType(New Node(Of ?, ?)(n << 1 - 1){}, Node(Of K, V)())
					nextTab = nt ' try to cope with OOME
				Catch ex As Throwable
					sizeCtl =  java.lang.[Integer].Max_Value
					Return
				End Try
				nextTable = nextTab
				transferIndex = n
			End If
			Dim nextn As Integer = nextTab.Length
			Dim fwd As New ForwardingNode(Of K, V)(nextTab)
			Dim advance As Boolean = True
			Dim finishing As Boolean = False ' to ensure sweep before committing nextTab
			Dim i As Integer = 0
			Dim bound As Integer = 0
			Do
				Dim f As Node(Of K, V)
				Dim fh As Integer
				Do While advance
					Dim nextIndex, nextBound As Integer
					i -= 1
					If i >= bound OrElse finishing Then
						advance = False
					Else
						nextIndex = transferIndex
						If nextIndex <= 0 Then
							i = -1
							advance = False
'JAVA TO VB CONVERTER TODO TASK: Assignments within expressions are not supported in VB
						ElseIf U.compareAndSwapInt(Me, TRANSFERINDEX, nextIndex, nextBound = (If(nextIndex > stride, nextIndex - stride, 0))) Then
							bound = nextBound
							i = nextIndex - 1
							advance = False
						End If
						End If
				Loop
				If i < 0 OrElse i >= n OrElse i + n >= nextn Then
					Dim sc As Integer
					If finishing Then
						nextTable = Nothing
						table = nextTab
						sizeCtl = (n << 1) - (CInt(CUInt(n) >> 1))
						Return
					End If
'JAVA TO VB CONVERTER TODO TASK: Assignments within expressions are not supported in VB
					If U.compareAndSwapInt(Me, SIZECTL, sc = sizeCtl, sc - 1) Then
						If (sc - 2) <> resizeStamp(n) << RESIZE_STAMP_SHIFT Then Return
							advance = True
							finishing = advance
						i = n ' recheck before commit
					End If
				Else
					f = tabAt(tab, i)
					If f Is Nothing Then
						advance = casTabAt(tab, i, Nothing, fwd)
					Else
						fh = f.hash
						If fh = MOVED Then
							advance = True ' already processed
						Else
							SyncLock f
						End If
					End If
						If tabAt(tab, i) Is f Then
							Dim ln As Node(Of K, V), hn As Node(Of K, V)
							If fh >= 0 Then
								Dim runBit As Integer = fh And n
								Dim lastRun As Node(Of K, V) = f
								Dim p As Node(Of K, V) = f.next
								Do While p IsNot Nothing
									Dim b As Integer = p.hash And n
									If b <> runBit Then
										runBit = b
										lastRun = p
									End If
									p = p.next
								Loop
								If runBit = 0 Then
									ln = lastRun
									hn = Nothing
								Else
									hn = lastRun
									ln = Nothing
								End If
								p = f
								Do While p IsNot lastRun
									Dim ph As Integer = p.hash
									Dim pk As K = p.key
									Dim pv As V = p.val
									If (ph And n) = 0 Then
										ln = New Node(Of K, V)(ph, pk, pv, ln)
									Else
										hn = New Node(Of K, V)(ph, pk, pv, hn)
									End If
									p = p.next
								Loop
								tabAtbAt(nextTab, i, ln)
								tabAtbAt(nextTab, i + n, hn)
								tabAtbAt(tab, i, fwd)
								advance = True
							ElseIf TypeOf f Is TreeBin Then
								Dim t As TreeBin(Of K, V) = CType(f, TreeBin(Of K, V))
								Dim lo As TreeNode(Of K, V) = Nothing, loTail As TreeNode(Of K, V) = Nothing
								Dim hi As TreeNode(Of K, V) = Nothing, hiTail As TreeNode(Of K, V) = Nothing
								Dim lc As Integer = 0, hc As Integer = 0
								Dim e As Node(Of K, V) = t.first
								Do While e IsNot Nothing
									Dim h As Integer = e.hash
									Dim p As New TreeNode(Of K, V)(h, e.key, e.val, Nothing, Nothing)
									If (h And n) = 0 Then
										p.prev = loTail
										If p.prev Is Nothing Then
											lo = p
										Else
											loTail.next = p
										End If
										loTail = p
										lc += 1
									Else
										p.prev = hiTail
										If p.prev Is Nothing Then
											hi = p
										Else
											hiTail.next = p
										End If
										hiTail = p
										hc += 1
									End If
									e = e.next
								Loop
								ln = If(lc <= UNTREEIFY_THRESHOLD, untreeify(lo), If(hc <> 0, New TreeBin(Of K, V)(lo), t))
								hn = If(hc <= UNTREEIFY_THRESHOLD, untreeify(hi), If(lc <> 0, New TreeBin(Of K, V)(hi), t))
								tabAtbAt(nextTab, i, ln)
								tabAtbAt(nextTab, i + n, hn)
								tabAtbAt(tab, i, fwd)
								advance = True
							End If
						End If
							End SyncLock
						End If
			Loop
		End Sub

		' ---------------- Counter support -------------- 

		''' <summary>
		''' A padded cell for distributing counts.  Adapted from LongAdder
		''' and Striped64.  See their internal docs for explanation.
		''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
		Friend NotInheritable Class CounterCell
'JAVA TO VB CONVERTER TODO TASK: There is no VB equivalent to 'volatile':
			Friend value As Long
			Friend Sub New(ByVal x As Long)
				value = x
			End Sub
		End Class

		Friend Function sumCount() As Long
			Dim [as] As CounterCell() = counterCells
			Dim a As CounterCell
			Dim sum As Long = baseCount
			If [as] IsNot Nothing Then
				For i As Integer = 0 To [as].Length - 1
					a = [as](i)
					If a IsNot Nothing Then sum += a.value
				Next i
			End If
			Return sum
		End Function

		' See LongAdder version for explanation
		Private Sub fullAddCount(ByVal x As Long, ByVal wasUncontended As Boolean)
			Dim h As Integer
			h = ThreadLocalRandom.probe
			If h = 0 Then
				ThreadLocalRandom.localInit() ' force initialization
				h = ThreadLocalRandom.probe
				wasUncontended = True
			End If
			Dim collide As Boolean = False ' True if last slot nonempty
			Do
				Dim [as] As CounterCell()
				Dim a As CounterCell
				Dim n As Integer
				Dim v As Long
				[as] = counterCells
				n = [as].Length
				If [as] IsNot Nothing AndAlso n > 0 Then
					a = [as]((n - 1) And h)
					If a Is Nothing Then
						If cellsBusy = 0 Then ' Try to attach new Cell
							Dim r As New CounterCell(x) ' Optimistic create
							If cellsBusy = 0 AndAlso U.compareAndSwapInt(Me, CELLSBUSY, 0, 1) Then
								Dim created As Boolean = False
								Try ' Recheck under lock
									Dim rs As CounterCell()
									Dim m, j As Integer
									rs = counterCells
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
'JAVA TO VB CONVERTER TODO TASK: Assignments within expressions are not supported in VB
					ElseIf U.compareAndSwapLong(a, CELLVALUE, v = a.value, v + x) Then
						Exit Do
					ElseIf counterCells <> [as] OrElse n >= NCPU Then
						collide = False ' At max size or stale
					ElseIf Not collide Then
						collide = True
					ElseIf cellsBusy = 0 AndAlso U.compareAndSwapInt(Me, CELLSBUSY, 0, 1) Then
						Try
							If counterCells = [as] Then ' Expand table unless stale
								Dim rs As CounterCell() = New CounterCell(n << 1 - 1){}
								For i As Integer = 0 To n - 1
									rs(i) = [as](i)
								Next i
								counterCells = rs
							End If
						Finally
							cellsBusy = 0
						End Try
						collide = False
						Continue Do ' Retry with expanded table
					End If
					h = ThreadLocalRandom.advanceProbe(h)
				ElseIf cellsBusy = 0 AndAlso counterCells = [as] AndAlso U.compareAndSwapInt(Me, CELLSBUSY, 0, 1) Then
					Dim init As Boolean = False
					Try ' Initialize table
						If counterCells = [as] Then
							Dim rs As CounterCell() = New CounterCell(1){}
							rs(h And 1) = New CounterCell(x)
							counterCells = rs
							init = True
						End If
					Finally
						cellsBusy = 0
					End Try
					If init Then Exit Do
'JAVA TO VB CONVERTER TODO TASK: Assignments within expressions are not supported in VB
				ElseIf U.compareAndSwapLong(Me, BASECOUNT, v = baseCount, v + x) Then
					Exit Do ' Fall back on using base
				End If
			Loop
		End Sub

		' ---------------- Conversion from/to TreeBins -------------- 

		''' <summary>
		''' Replaces all linked nodes in bin at given index unless table is
		''' too small, in which case resizes instead.
		''' </summary>
		Private Sub treeifyBin(ByVal tab As Node(Of K, V)(), ByVal index As Integer)
			Dim b As Node(Of K, V)
			Dim n, sc As Integer
			If tab IsNot Nothing Then
				n = tab.Length
				If n < MIN_TREEIFY_CAPACITY Then
					tryPresize(n << 1)
				Else
					b = tabAt(tab, index)
					If b IsNot Nothing AndAlso b.hash >= 0 Then
						SyncLock b
							If tabAt(tab, index) Is b Then
								Dim hd As TreeNode(Of K, V) = Nothing, tl As TreeNode(Of K, V) = Nothing
								Dim e As Node(Of K, V) = b
								Do While e IsNot Nothing
									Dim p As New TreeNode(Of K, V)(e.hash, e.key, e.val, Nothing, Nothing)
									p.prev = tl
									If p.prev Is Nothing Then
										hd = p
									Else
										tl.next = p
									End If
									tl = p
									e = e.next
								Loop
								tabAtbAt(tab, index, New TreeBin(Of K, V)(hd))
							End If
						End SyncLock
					End If
					End If
			End If
		End Sub

		''' <summary>
		''' Returns a list on non-TreeNodes replacing those in given list.
		''' </summary>
		Friend Shared Function untreeify(Of K, V)(ByVal b As Node(Of K, V)) As Node(Of K, V)
			Dim hd As Node(Of K, V) = Nothing, tl As Node(Of K, V) = Nothing
			Dim q As Node(Of K, V) = b
			Do While q IsNot Nothing
				Dim p As New Node(Of K, V)(q.hash, q.key, q.val, Nothing)
				If tl Is Nothing Then
					hd = p
				Else
					tl.next = p
				End If
				tl = p
				q = q.next
			Loop
			Return hd
		End Function

		' ---------------- TreeNodes -------------- 

		''' <summary>
		''' Nodes for use in TreeBins
		''' </summary>
		Friend NotInheritable Class TreeNode(Of K, V)
			Inherits Node(Of K, V)

			Friend parent As TreeNode(Of K, V) ' red-black tree links
			Friend left As TreeNode(Of K, V)
			Friend right As TreeNode(Of K, V)
			Friend prev As TreeNode(Of K, V) ' needed to unlink next upon deletion
			Friend red As Boolean

			Friend Sub New(ByVal hash As Integer, ByVal key As K, ByVal val As V, ByVal [next] As Node(Of K, V), ByVal parent As TreeNode(Of K, V))
				MyBase.New(hash, key, val, [next])
				Me.parent = parent
			End Sub

			Friend Function find(ByVal h As Integer, ByVal k As Object) As Node(Of K, V)
				Return findTreeNode(h, k, Nothing)
			End Function

			''' <summary>
			''' Returns the TreeNode (or null if not found) for the given key
			''' starting at given root.
			''' </summary>
			Friend Function findTreeNode(ByVal h As Integer, ByVal k As Object, ByVal kc As [Class]) As TreeNode(Of K, V)
				If k IsNot Nothing Then
					Dim p As TreeNode(Of K, V) = Me
					Do
						Dim ph, dir As Integer
						Dim pk As K
						Dim q As TreeNode(Of K, V)
						Dim pl As TreeNode(Of K, V) = p.left, pr As TreeNode(Of K, V) = p.right
						ph = p.hash
						If ph > h Then
							p = pl
						ElseIf ph < h Then
							p = pr
						Else
							pk = p.key
							If pk Is k OrElse (pk IsNot Nothing AndAlso k.Equals(pk)) Then
								Return p
							ElseIf pl Is Nothing Then
								p = pr
							ElseIf pr Is Nothing Then
								p = pl
							Else
								kc = comparableClassFor(k)
								dir = compareComparables(kc, k, pk)
								If (kc IsNot Nothing OrElse kc IsNot Nothing) AndAlso dir <> 0 Then
									p = If(dir < 0, pl, pr)
								Else
									q = pr.findTreeNode(h, k, kc)
									If q IsNot Nothing Then
										Return q
									Else
										p = pl
									End If
									End If
								End If
							End If
					Loop While p IsNot Nothing
				End If
				Return Nothing
			End Function
		End Class

		' ---------------- TreeBins -------------- 

		''' <summary>
		''' TreeNodes used at the heads of bins. TreeBins do not hold user
		''' keys or values, but instead point to list of TreeNodes and
		''' their root. They also maintain a parasitic read-write lock
		''' forcing writers (who hold bin lock) to wait for readers (who do
		''' not) to complete before tree restructuring operations.
		''' </summary>
		Friend NotInheritable Class TreeBin(Of K, V)
			Inherits Node(Of K, V)

			Friend root As TreeNode(Of K, V)
'JAVA TO VB CONVERTER TODO TASK: There is no VB equivalent to 'volatile':
			Friend first As TreeNode(Of K, V)
'JAVA TO VB CONVERTER TODO TASK: There is no VB equivalent to 'volatile':
			Friend waiter As Thread
'JAVA TO VB CONVERTER TODO TASK: There is no VB equivalent to 'volatile':
			Friend lockState As Integer
			' values for lockState
			Friend Const WRITER As Integer = 1 ' set while holding write lock
			Friend Const WAITER As Integer = 2 ' set when waiting for write lock
			Friend Const READER As Integer = 4 ' increment value for setting read lock

			''' <summary>
			''' Tie-breaking utility for ordering insertions when equal
			''' hashCodes and non-comparable. We don't require a total
			''' order, just a consistent insertion rule to maintain
			''' equivalence across rebalancings. Tie-breaking further than
			''' necessary simplifies testing a bit.
			''' </summary>
			Friend Shared Function tieBreakOrder(ByVal a As Object, ByVal b As Object) As Integer
				Dim d As Integer
				d = a.GetType().name.CompareTo(b.GetType().name)
				If a Is Nothing OrElse b Is Nothing OrElse d = 0 Then d = (If(System.identityHashCode(a) <= System.identityHashCode(b), -1, 1))
				Return d
			End Function

			''' <summary>
			''' Creates bin with initial set of nodes headed by b.
			''' </summary>
			Friend Sub New(ByVal b As TreeNode(Of K, V))
				MyBase.New(TREEBIN, Nothing, Nothing, Nothing)
				Me.first = b
				Dim r As TreeNode(Of K, V) = Nothing
'JAVA TO VB CONVERTER TODO TASK: The following line could not be converted:
				for (TreeNode<K,V> x = b, next; x != Nothing; x = next)
					[next] = CType(x.next, TreeNode(Of K, V))
						x.right = Nothing
						x.left = x.right
					If r Is Nothing Then
						x.parent = Nothing
						x.red = False
						r = x
					Else
						Dim k As K = x.key
						Dim h As Integer = x.hash
						Dim kc As  [Class] = Nothing
						Dim p As TreeNode(Of K, V) = r
						Do
							Dim dir, ph As Integer
							Dim pk As K = p.key
							ph = p.hash
							If ph > h Then
								dir = -1
							ElseIf ph < h Then
								dir = 1
							Else
								kc = comparableClassFor(k)
								dir = compareComparables(kc, k, pk)
								If (kc Is Nothing AndAlso kc Is Nothing) OrElse dir = 0 Then dir = tieBreakOrder(k, pk)
								End If
								Dim xp As TreeNode(Of K, V) = p
							p = If(dir <= 0, p.left, p.right)
							If p Is Nothing Then
								x.parent = xp
								If dir <= 0 Then
									xp.left = x
								Else
									xp.right = x
								End If
								r = balanceInsertion(r, x)
								Exit Do
							End If
						Loop
					End If
				Me.root = r
				Debug.Assert(checkInvariants(root))
			End Sub

			''' <summary>
			''' Acquires write lock for tree restructuring.
			''' </summary>
			Private Sub lockRoot()
				If Not U.compareAndSwapInt(Me, LOCKSTATE, 0, WRITER) Then contendedLock() ' offload to separate method
			End Sub

			''' <summary>
			''' Releases write lock for tree restructuring.
			''' </summary>
			Private Sub unlockRoot()
				lockState = 0
			End Sub

			''' <summary>
			''' Possibly blocks awaiting root lock.
			''' </summary>
			Private Sub contendedLock()
				Dim waiting As Boolean = False
				Dim s As Integer
				Do
					s = lockState
					If (s And (Not WAITER)) = 0 Then
						If U.compareAndSwapInt(Me, LOCKSTATE, s, WRITER) Then
							If waiting Then waiter = Nothing
							Return
						End If
					ElseIf (s And WAITER) = 0 Then
						If U.compareAndSwapInt(Me, LOCKSTATE, s, s Or WAITER) Then
							waiting = True
							waiter = Thread.CurrentThread
						End If
					ElseIf waiting Then
						java.util.concurrent.locks.LockSupport.park(Me)
					End If
				Loop
			End Sub

			''' <summary>
			''' Returns matching node or null if none. Tries to search
			''' using tree comparisons from root, but continues linear
			''' search when lock not available.
			''' </summary>
			Friend Function find(ByVal h As Integer, ByVal k As Object) As Node(Of K, V)
				If k IsNot Nothing Then
					Dim e As Node(Of K, V) = first
					Do While e IsNot Nothing
						Dim s As Integer
						Dim ek As K
						s = lockState
						If (s And (WAITER Or WRITER)) <> 0 Then
							ek = e.key
							If e.hash = h AndAlso (ek Is k OrElse (ek IsNot Nothing AndAlso k.Equals(ek))) Then Return e
							e = e.next
						ElseIf U.compareAndSwapInt(Me, LOCKSTATE, s, s + READER) Then
							Dim r As TreeNode(Of K, V), p As TreeNode(Of K, V)
							Try
'JAVA TO VB CONVERTER TODO TASK: Assignments within expressions are not supported in VB
								p = (If((r = root) Is Nothing, Nothing, r.findTreeNode(h, k, Nothing)))
							Finally
								Dim w As Thread
								w = waiter
								If U.getAndAddInt(Me, LOCKSTATE, -READER) = (READER Or WAITER) AndAlso w IsNot Nothing Then java.util.concurrent.locks.LockSupport.unpark(w)
							End Try
							Return p
						End If
					Loop
				End If
				Return Nothing
			End Function

			''' <summary>
			''' Finds or adds a node. </summary>
			''' <returns> null if added </returns>
			Friend Function putTreeVal(ByVal h As Integer, ByVal k As K, ByVal v As V) As TreeNode(Of K, V)
				Dim kc As  [Class] = Nothing
				Dim searched As Boolean = False
				Dim p As TreeNode(Of K, V) = root
				Do
					Dim dir, ph As Integer
					Dim pk As K
					If p Is Nothing Then
							root = New TreeNode(Of K, V)(h, k, v, Nothing, Nothing)
							first = root
						Exit Do
					Else
						ph = p.hash
						If ph > h Then
							dir = -1
						ElseIf ph < h Then
							dir = 1
						Else
							pk = p.key
							If pk Is k OrElse (pk IsNot Nothing AndAlso k.Equals(pk)) Then
								Return p
							Else
								kc = comparableClassFor(k)
								dir = compareComparables(kc, k, pk)
								If (kc Is Nothing AndAlso kc Is Nothing) OrElse dir = 0 Then
									If Not searched Then
										Dim q As TreeNode(Of K, V), ch As TreeNode(Of K, V)
										searched = True
										ch = p.left
										q = ch.findTreeNode(h, k, kc)
										ch = p.right
										q = ch.findTreeNode(h, k, kc)
										If (ch IsNot Nothing AndAlso q IsNot Nothing) OrElse (ch IsNot Nothing AndAlso q IsNot Nothing) Then Return q
									End If
									dir = tieBreakOrder(k, pk)
								End If
								End If
							End If
						End If

					Dim xp As TreeNode(Of K, V) = p
					p = If(dir <= 0, p.left, p.right)
					If p Is Nothing Then
						Dim x As TreeNode(Of K, V), f As TreeNode(Of K, V) = first
							x = New TreeNode(Of K, V)(h, k, v, f, xp)
							first = x
						If f IsNot Nothing Then f.prev = x
						If dir <= 0 Then
							xp.left = x
						Else
							xp.right = x
						End If
						If Not xp.red Then
							x.red = True
						Else
							lockRoot()
							Try
								root = balanceInsertion(root, x)
							Finally
								unlockRoot()
							End Try
						End If
						Exit Do
					End If
				Loop
				Debug.Assert(checkInvariants(root))
				Return Nothing
			End Function

			''' <summary>
			''' Removes the given node, that must be present before this
			''' call.  This is messier than typical red-black deletion code
			''' because we cannot swap the contents of an interior node
			''' with a leaf successor that is pinned by "next" pointers
			''' that are accessible independently of lock. So instead we
			''' swap the tree linkages.
			''' </summary>
			''' <returns> true if now too small, so should be untreeified </returns>
			Friend Function removeTreeNode(ByVal p As TreeNode(Of K, V)) As Boolean
				Dim [next] As TreeNode(Of K, V) = CType(p.next, TreeNode(Of K, V))
				Dim pred As TreeNode(Of K, V) = p.prev ' unlink traversal pointers
				Dim r As TreeNode(Of K, V), rl As TreeNode(Of K, V)
				If pred Is Nothing Then
					first = [next]
				Else
					pred.next = [next]
				End If
				If [next] IsNot Nothing Then [next].prev = pred
				If first Is Nothing Then
					root = Nothing
					Return True
				End If
				r = root
				rl = r.left
				If r Is Nothing OrElse r.right Is Nothing OrElse rl Is Nothing OrElse rl.left Is Nothing Then ' too small Return True
				lockRoot()
				Try
					Dim replacement As TreeNode(Of K, V)
					Dim pl As TreeNode(Of K, V) = p.left
					Dim pr As TreeNode(Of K, V) = p.right
					If pl IsNot Nothing AndAlso pr IsNot Nothing Then
						Dim s As TreeNode(Of K, V) = pr, sl As TreeNode(Of K, V)
						sl = s.left
						Do While sl IsNot Nothing ' find successor
							s = sl
							sl = s.left
						Loop
						Dim c As Boolean = s.red ' swap colors
						s.red = p.red
						p.red = c
						Dim sr As TreeNode(Of K, V) = s.right
						Dim pp As TreeNode(Of K, V) = p.parent
						If s Is pr Then ' p was s's direct parent
							p.parent = s
							s.right = p
						Else
							Dim sp As TreeNode(Of K, V) = s.parent
							p.parent = sp
							If p.parent IsNot Nothing Then
								If s Is sp.left Then
									sp.left = p
								Else
									sp.right = p
								End If
							End If
							s.right = pr
							If s.right IsNot Nothing Then pr.parent = s
						End If
						p.left = Nothing
						p.right = sr
						If p.right IsNot Nothing Then sr.parent = p
						s.left = pl
						If s.left IsNot Nothing Then pl.parent = s
						s.parent = pp
						If s.parent Is Nothing Then
							r = s
						ElseIf p Is pp.left Then
							pp.left = s
						Else
							pp.right = s
						End If
						If sr IsNot Nothing Then
							replacement = sr
						Else
							replacement = p
						End If
					ElseIf pl IsNot Nothing Then
						replacement = pl
					ElseIf pr IsNot Nothing Then
						replacement = pr
					Else
						replacement = p
					End If
					If replacement IsNot p Then
							replacement.parent = p.parent
							Dim pp As TreeNode(Of K, V) = replacement.parent
						If pp Is Nothing Then
							r = replacement
						ElseIf p Is pp.left Then
							pp.left = replacement
						Else
							pp.right = replacement
						End If
							p.parent = Nothing
								p.right = p.parent
								p.left = p.right
					End If

					root = If(p.red, r, balanceDeletion(r, replacement))

					If p Is replacement Then ' detach pointers
						Dim pp As TreeNode(Of K, V)
						pp = p.parent
						If pp IsNot Nothing Then
							If p Is pp.left Then
								pp.left = Nothing
							ElseIf p Is pp.right Then
								pp.right = Nothing
							End If
							p.parent = Nothing
						End If
					End If
				Finally
					unlockRoot()
				End Try
				Debug.Assert(checkInvariants(root))
				Return False
			End Function

			' ------------------------------------------------------------ 
			' Red-black tree methods, all adapted from CLR

			Friend Shared Function rotateLeft(Of K, V)(ByVal root As TreeNode(Of K, V), ByVal p As TreeNode(Of K, V)) As TreeNode(Of K, V)
				Dim r As TreeNode(Of K, V), pp As TreeNode(Of K, V), rl As TreeNode(Of K, V)
				r = p.right
				If p IsNot Nothing AndAlso r IsNot Nothing Then
					p.right = r.left
					rl = p.right
					If rl IsNot Nothing Then rl.parent = p
					r.parent = p.parent
					pp = r.parent
					If pp Is Nothing Then
							root = r
							r.red = False
					ElseIf pp.left Is p Then
						pp.left = r
					Else
						pp.right = r
					End If
					r.left = p
					p.parent = r
				End If
				Return root
			End Function

			Friend Shared Function rotateRight(Of K, V)(ByVal root As TreeNode(Of K, V), ByVal p As TreeNode(Of K, V)) As TreeNode(Of K, V)
				Dim l As TreeNode(Of K, V), pp As TreeNode(Of K, V), lr As TreeNode(Of K, V)
				l = p.left
				If p IsNot Nothing AndAlso l IsNot Nothing Then
					p.left = l.right
					lr = p.left
					If lr IsNot Nothing Then lr.parent = p
					l.parent = p.parent
					pp = l.parent
					If pp Is Nothing Then
							root = l
							l.red = False
					ElseIf pp.right Is p Then
						pp.right = l
					Else
						pp.left = l
					End If
					l.right = p
					p.parent = l
				End If
				Return root
			End Function

			Friend Shared Function balanceInsertion(Of K, V)(ByVal root As TreeNode(Of K, V), ByVal x As TreeNode(Of K, V)) As TreeNode(Of K, V)
				x.red = True
				Dim xp As TreeNode(Of K, V)
				xpp
				xppl
				xppr
				Do
					xp = x.parent
					If xp Is Nothing Then
						x.red = False
						Return x
					Else
						xpp = xp.parent
						If (Not xp.red) OrElse xpp Is Nothing Then Return root
						End If
					xppl = xpp.left
					If xp Is xppl Then
						xppr = xpp.right
						If xppr IsNot Nothing AndAlso xppr.red Then
							xppr.red = False
							xp.red = False
							xpp.red = True
							x = xpp
						Else
							If x Is xp.right Then
'JAVA TO VB CONVERTER TODO TASK: Assignments within expressions are not supported in VB
								root = rotateLeft(root, x = xp)
'JAVA TO VB CONVERTER TODO TASK: Assignments within expressions are not supported in VB
								xpp = If((xp = x.parent) Is Nothing, Nothing, xp.parent)
							End If
							If xp IsNot Nothing Then
								xp.red = False
								If xpp IsNot Nothing Then
									xpp.red = True
									root = rotateRight(root, xpp)
								End If
							End If
						End If
					Else
						If xppl IsNot Nothing AndAlso xppl.red Then
							xppl.red = False
							xp.red = False
							xpp.red = True
							x = xpp
						Else
							If x Is xp.left Then
'JAVA TO VB CONVERTER TODO TASK: Assignments within expressions are not supported in VB
								root = rotateRight(root, x = xp)
'JAVA TO VB CONVERTER TODO TASK: Assignments within expressions are not supported in VB
								xpp = If((xp = x.parent) Is Nothing, Nothing, xp.parent)
							End If
							If xp IsNot Nothing Then
								xp.red = False
								If xpp IsNot Nothing Then
									xpp.red = True
									root = rotateLeft(root, xpp)
								End If
							End If
						End If
					End If
				Loop
			End Function

			Friend Shared Function balanceDeletion(Of K, V)(ByVal root As TreeNode(Of K, V), ByVal x As TreeNode(Of K, V)) As TreeNode(Of K, V)
				Dim xp As TreeNode(Of K, V)
				xpl
				xpr
				Do
					If x Is Nothing OrElse x Is root Then
						Return root
					Else
						xp = x.parent
						If xp Is Nothing Then
							x.red = False
							Return x
						ElseIf x.red Then
							x.red = False
							Return root
						Else
							xpl = xp.left
							If xpl Is x Then
								xpr = xp.right
								If xpr IsNot Nothing AndAlso xpr.red Then
									xpr.red = False
									xp.red = True
									root = rotateLeft(root, xp)
'JAVA TO VB CONVERTER TODO TASK: Assignments within expressions are not supported in VB
									xpr = If((xp = x.parent) Is Nothing, Nothing, xp.right)
								End If
								If xpr Is Nothing Then
									x = xp
								Else
									Dim sl As TreeNode(Of K, V) = xpr.left, sr As TreeNode(Of K, V) = xpr.right
									If (sr Is Nothing OrElse (Not sr.red)) AndAlso (sl Is Nothing OrElse (Not sl.red)) Then
										xpr.red = True
										x = xp
									Else
										If sr Is Nothing OrElse (Not sr.red) Then
											If sl IsNot Nothing Then sl.red = False
											xpr.red = True
											root = rotateRight(root, xpr)
'JAVA TO VB CONVERTER TODO TASK: Assignments within expressions are not supported in VB
											xpr = If((xp = x.parent) Is Nothing, Nothing, xp.right)
										End If
										If xpr IsNot Nothing Then
											xpr.red = If(xp Is Nothing, False, xp.red)
											sr = xpr.right
											If sr IsNot Nothing Then sr.red = False
										End If
										If xp IsNot Nothing Then
											xp.red = False
											root = rotateLeft(root, xp)
										End If
										x = root
									End If
								End If
							Else ' symmetric
								If xpl IsNot Nothing AndAlso xpl.red Then
							End If
						End If
							xpl.red = False
							xp.red = True
							root = rotateRight(root, xp)
'JAVA TO VB CONVERTER TODO TASK: Assignments within expressions are not supported in VB
							xpl = If((xp = x.parent) Is Nothing, Nothing, xp.left)
								End If
						If xpl Is Nothing Then
							x = xp
						Else
							Dim sl As TreeNode(Of K, V) = xpl.left, sr As TreeNode(Of K, V) = xpl.right
							If (sl Is Nothing OrElse (Not sl.red)) AndAlso (sr Is Nothing OrElse (Not sr.red)) Then
								xpl.red = True
								x = xp
							Else
								If sl Is Nothing OrElse (Not sl.red) Then
									If sr IsNot Nothing Then sr.red = False
									xpl.red = True
									root = rotateLeft(root, xpl)
'JAVA TO VB CONVERTER TODO TASK: Assignments within expressions are not supported in VB
									xpl = If((xp = x.parent) Is Nothing, Nothing, xp.left)
								End If
								If xpl IsNot Nothing Then
									xpl.red = If(xp Is Nothing, False, xp.red)
									sl = xpl.left
									If sl IsNot Nothing Then sl.red = False
								End If
								If xp IsNot Nothing Then
									xp.red = False
									root = rotateRight(root, xp)
								End If
								x = root
							End If
						End If
							End If
				Loop
			End Function

			''' <summary>
			''' Recursive invariant check
			''' </summary>
			Friend Shared Function checkInvariants(Of K, V)(ByVal t As TreeNode(Of K, V)) As Boolean
				Dim tp As TreeNode(Of K, V) = t.parent, tl As TreeNode(Of K, V) = t.left, tr As TreeNode(Of K, V) = t.right, tb As TreeNode(Of K, V) = t.prev, tn As TreeNode(Of K, V) = CType(t.next, TreeNode(Of K, V))
				If tb IsNot Nothing AndAlso tb.next IsNot t Then Return False
				If tn IsNot Nothing AndAlso tn.prev IsNot t Then Return False
				If tp IsNot Nothing AndAlso t IsNot tp.left AndAlso t IsNot tp.right Then Return False
				If tl IsNot Nothing AndAlso (tl.parent IsNot t OrElse tl.hash > t.hash) Then Return False
				If tr IsNot Nothing AndAlso (tr.parent IsNot t OrElse tr.hash < t.hash) Then Return False
				If t.red AndAlso tl IsNot Nothing AndAlso tl.red AndAlso tr IsNot Nothing AndAlso tr.red Then Return False
				If tl IsNot Nothing AndAlso (Not checkInvariants(tl)) Then Return False
				If tr IsNot Nothing AndAlso (Not checkInvariants(tr)) Then Return False
				Return True
			End Function

			Private Shared ReadOnly U As sun.misc.Unsafe
			Private Shared ReadOnly LOCKSTATE As Long
			Shared Sub New()
				Try
					U = sun.misc.Unsafe.unsafe
					Dim k As  [Class] = GetType(TreeBin)
					LOCKSTATE = U.objectFieldOffset(k.getDeclaredField("lockState"))
				Catch e As Exception
					Throw New [Error](e)
				End Try
			End Sub
		End Class

		' ----------------Table Traversal -------------- 

		''' <summary>
		''' Records the table, its length, and current traversal index for a
		''' traverser that must process a region of a forwarded table before
		''' proceeding with current table.
		''' </summary>
		Friend NotInheritable Class TableStack(Of K, V)
			Friend length As Integer
			Friend index As Integer
			Friend tab As Node(Of K, V)()
			Friend [next] As TableStack(Of K, V)
		End Class

		''' <summary>
		''' Encapsulates traversal for methods such as containsValue; also
		''' serves as a base class for other iterators and spliterators.
		''' 
		''' Method advance visits once each still-valid node that was
		''' reachable upon iterator construction. It might miss some that
		''' were added to a bin after the bin was visited, which is OK wrt
		''' consistency guarantees. Maintaining this property in the face
		''' of possible ongoing resizes requires a fair amount of
		''' bookkeeping state that is difficult to optimize away amidst
		''' volatile accesses.  Even so, traversal maintains reasonable
		''' throughput.
		''' 
		''' Normally, iteration proceeds bin-by-bin traversing lists.
		''' However, if the table has been resized, then all future steps
		''' must traverse both the bin at the current index as well as at
		''' (index + baseSize); and so on for further resizings. To
		''' paranoically cope with potential sharing by users of iterators
		''' across threads, iteration terminates if a bounds checks fails
		''' for a table read.
		''' </summary>
		Friend Class Traverser(Of K, V)
			Friend tab As Node(Of K, V)() ' current table; updated if resized
			Friend [next] As Node(Of K, V) ' the next entry to use
			Friend stack As TableStack(Of K, V), spare As TableStack(Of K, V) ' to save/restore on ForwardingNodes
			Friend index As Integer ' index of bin to use next
			Friend baseIndex As Integer ' current index of initial table
			Friend baseLimit As Integer ' index bound for initial table
			Friend ReadOnly baseSize As Integer ' initial table size

			Friend Sub New(ByVal tab As Node(Of K, V)(), ByVal size As Integer, ByVal index As Integer, ByVal limit As Integer)
				Me.tab = tab
				Me.baseSize = size
					Me.index = index
					Me.baseIndex = Me.index
				Me.baseLimit = limit
				Me.next = Nothing
			End Sub

			''' <summary>
			''' Advances if possible, returning next valid node, or null if none.
			''' </summary>
			Friend Function advance() As Node(Of K, V)
				Dim e As Node(Of K, V)
				e = [next]
				If e IsNot Nothing Then e = e.next
				Do
					Dim t As Node(Of K, V)() ' must use locals in checks
					Dim i, n As Integer
					If e IsNot Nothing Then
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
							= e
							Return [next]
					End If
					t = tab
					n = t.Length
					i = index
					If baseIndex >= baseLimit OrElse t Is Nothing OrElse n <= i OrElse i < 0 Then
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
							= Nothing
							Return [next]
					End If
					e = tabAt(t, i)
					If e IsNot Nothing AndAlso e.hash < 0 Then
						If TypeOf e Is ForwardingNode Then
							tab = CType(e, ForwardingNode(Of K, V)).nextTable
							e = Nothing
							pushState(t, i, n)
							Continue Do
						ElseIf TypeOf e Is TreeBin Then
							e = CType(e, TreeBin(Of K, V)).first
						Else
							e = Nothing
						End If
					End If
					If stack IsNot Nothing Then
						recoverState(n)
					Else
						index = i + baseSize
						If index >= n Then
							baseIndex += 1
							index = baseIndex
						End If
						End If
				Loop
			End Function

			''' <summary>
			''' Saves traversal state upon encountering a forwarding node.
			''' </summary>
			Private Sub pushState(ByVal t As Node(Of K, V)(), ByVal i As Integer, ByVal n As Integer)
				Dim s As TableStack(Of K, V) = spare ' reuse if possible
				If s IsNot Nothing Then
					spare = s.next
				Else
					s = New TableStack(Of K, V)
				End If
				s.tab = t
				s.length = n
				s.index = i
				s.next = stack
				stack = s
			End Sub

			''' <summary>
			''' Possibly pops traversal state.
			''' </summary>
			''' <param name="n"> length of current table </param>
			Private Sub recoverState(ByVal n As Integer)
				Dim s As TableStack(Of K, V)
				Dim len As Integer
				s = stack
'JAVA TO VB CONVERTER TODO TASK: Assignments within expressions are not supported in VB
				index += (len = s.length)
				Do While s IsNot Nothing AndAlso index >= n
					n = len
					index = s.index
					tab = s.tab
					s.tab = Nothing
					Dim [next] As TableStack(Of K, V) = s.next
					s.next = spare ' save for reuse
					stack = [next]
					spare = s
					s = stack
					index += (len = s.length)
				Loop
				index += baseSize
				If s Is Nothing AndAlso index >= n Then
					baseIndex += 1
					index = baseIndex
				End If
			End Sub
		End Class

		''' <summary>
		''' Base of key, value, and entry Iterators. Adds fields to
		''' Traverser to support iterator.remove.
		''' </summary>
		Friend Class BaseIterator(Of K, V)
			Inherits Traverser(Of K, V)

			Friend ReadOnly map As ConcurrentHashMap(Of K, V)
			Friend lastReturned As Node(Of K, V)
			Friend Sub New(ByVal tab As Node(Of K, V)(), ByVal size As Integer, ByVal index As Integer, ByVal limit As Integer, ByVal map As ConcurrentHashMap(Of K, V))
				MyBase.New(tab, size, index, limit)
				Me.map = map
				advance()
			End Sub

			Public Function hasNext() As Boolean
				Return [next] IsNot Nothing
			End Function
			Public Function hasMoreElements() As Boolean
				Return [next] IsNot Nothing
			End Function

			Public Sub remove()
				Dim p As Node(Of K, V)
				p = lastReturned
				If p Is Nothing Then Throw New IllegalStateException
				lastReturned = Nothing
				map.replaceNode(p.key, Nothing, Nothing)
			End Sub
		End Class

		Friend NotInheritable Class KeyIterator(Of K, V)
			Inherits BaseIterator(Of K, V)
			Implements IEnumerator(Of K), System.Collections.IEnumerator(Of K)

			Friend Sub New(ByVal tab As Node(Of K, V)(), ByVal index As Integer, ByVal size As Integer, ByVal limit As Integer, ByVal map As ConcurrentHashMap(Of K, V))
				MyBase.New(tab, index, size, limit, map)
			End Sub

			Public Function [next]() As K
				Dim p As Node(Of K, V)
				p = [next]
				If p Is Nothing Then Throw New java.util.NoSuchElementException
				Dim k As K = p.key
				lastReturned = p
				advance()
				Return k
			End Function

			Public Function nextElement() As K
				Return [next]()
			End Function
		End Class

		Friend NotInheritable Class ValueIterator(Of K, V)
			Inherits BaseIterator(Of K, V)
			Implements IEnumerator(Of V), System.Collections.IEnumerator(Of V)

			Friend Sub New(ByVal tab As Node(Of K, V)(), ByVal index As Integer, ByVal size As Integer, ByVal limit As Integer, ByVal map As ConcurrentHashMap(Of K, V))
				MyBase.New(tab, index, size, limit, map)
			End Sub

			Public Function [next]() As V
				Dim p As Node(Of K, V)
				p = [next]
				If p Is Nothing Then Throw New java.util.NoSuchElementException
				Dim v As V = p.val
				lastReturned = p
				advance()
				Return v
			End Function

			Public Function nextElement() As V
				Return [next]()
			End Function
		End Class

		Friend NotInheritable Class EntryIterator(Of K, V)
			Inherits BaseIterator(Of K, V)
			Implements IEnumerator(Of KeyValuePair(Of K, V))

			Friend Sub New(ByVal tab As Node(Of K, V)(), ByVal index As Integer, ByVal size As Integer, ByVal limit As Integer, ByVal map As ConcurrentHashMap(Of K, V))
				MyBase.New(tab, index, size, limit, map)
			End Sub

			Public Function [next]() As KeyValuePair(Of K, V)
				Dim p As Node(Of K, V)
				p = [next]
				If p Is Nothing Then Throw New java.util.NoSuchElementException
				Dim k As K = p.key
				Dim v As V = p.val
				lastReturned = p
				advance()
				Return New MapEntry(Of K, V)(k, v, map)
			End Function
		End Class

		''' <summary>
		''' Exported Entry for EntryIterator
		''' </summary>
		Friend NotInheritable Class MapEntry(Of K, V)
			Implements KeyValuePair(Of K, V)

			Friend ReadOnly key As K ' non-null
			Friend val As V ' non-null
			Friend ReadOnly map As ConcurrentHashMap(Of K, V)
			Friend Sub New(ByVal key As K, ByVal val As V, ByVal map As ConcurrentHashMap(Of K, V))
				Me.key = key
				Me.val = val
				Me.map = map
			End Sub
			Public Property key As K
				Get
					Return key
				End Get
			End Property
			Public Property value As V
				Get
					Return val
				End Get
			End Property
			Public Overrides Function GetHashCode() As Integer
				Return key.GetHashCode() Xor val.GetHashCode()
			End Function
			Public Overrides Function ToString() As String
				Return key & "=" & val
			End Function

			Public Overrides Function Equals(ByVal o As Object) As Boolean
				Dim k, v As Object
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
				Dim e As KeyValuePair(Of ?, ?)
'JAVA TO VB CONVERTER TODO TASK: Assignments within expressions are not supported in VB
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
				Return ((TypeOf o Is DictionaryEntry) AndAlso (k = (e = CType(o, KeyValuePair(Of ?, ?))).key) IsNot Nothing AndAlso (v = e.Value) IsNot Nothing AndAlso (k Is key OrElse k.Equals(key)) AndAlso (v Is val OrElse v.Equals(val)))
			End Function

			''' <summary>
			''' Sets our entry's value and writes through to the map. The
			''' value to return is somewhat arbitrary here. Since we do not
			''' necessarily track asynchronous changes, the most recent
			''' "previous" value could be different from what we return (or
			''' could even have been removed, in which case the put will
			''' re-establish). We do not and cannot guarantee more.
			''' </summary>
			Public Function setValue(ByVal value As V) As V
				If value Is Nothing Then Throw New NullPointerException
				Dim v As V = val
				val = value
				map.put(key, value)
				Return v
			End Function
		End Class

		Friend NotInheritable Class KeySpliterator(Of K, V)
			Inherits Traverser(Of K, V)
			Implements java.util.Spliterator(Of K)

			Friend est As Long ' size estimate
			Friend Sub New(ByVal tab As Node(Of K, V)(), ByVal size As Integer, ByVal index As Integer, ByVal limit As Integer, ByVal est As Long)
				MyBase.New(tab, size, index, limit)
				Me.est = est
			End Sub

			Public Function trySplit() As java.util.Spliterator(Of K)
				Dim i, f, h As Integer
'JAVA TO VB CONVERTER TODO TASK: Assignments within expressions are not supported in VB
				Return If((h = CInt(CUInt(((i = baseIndex) + (f = baseLimit))) >> 1)) <= i, Nothing, New KeySpliterator(Of K, V)(tab, baseSize, baseLimit = h, f, est >>>= 1))
			End Function

'JAVA TO VB CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
			Public Sub forEachRemaining(Of T1)(ByVal action As java.util.function.Consumer(Of T1))
				If action Is Nothing Then Throw New NullPointerException
				Dim p As Node(Of K, V)
'JAVA TO VB CONVERTER TODO TASK: Assignments within expressions are not supported in VB
				Do While (p = advance()) IsNot Nothing
					action.accept(p.key)
				Loop
			End Sub

'JAVA TO VB CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
			Public Function tryAdvance(Of T1)(ByVal action As java.util.function.Consumer(Of T1)) As Boolean
				If action Is Nothing Then Throw New NullPointerException
				Dim p As Node(Of K, V)
				p = advance()
				If p Is Nothing Then Return False
				action.accept(p.key)
				Return True
			End Function

			Public Function estimateSize() As Long
				Return est
			End Function

			Public Function characteristics() As Integer
				Return java.util.Spliterator.DISTINCT Or java.util.Spliterator.CONCURRENT Or java.util.Spliterator.NONNULL
			End Function
		End Class

		Friend NotInheritable Class ValueSpliterator(Of K, V)
			Inherits Traverser(Of K, V)
			Implements java.util.Spliterator(Of V)

			Friend est As Long ' size estimate
			Friend Sub New(ByVal tab As Node(Of K, V)(), ByVal size As Integer, ByVal index As Integer, ByVal limit As Integer, ByVal est As Long)
				MyBase.New(tab, size, index, limit)
				Me.est = est
			End Sub

			Public Function trySplit() As java.util.Spliterator(Of V)
				Dim i, f, h As Integer
'JAVA TO VB CONVERTER TODO TASK: Assignments within expressions are not supported in VB
				Return If((h = CInt(CUInt(((i = baseIndex) + (f = baseLimit))) >> 1)) <= i, Nothing, New ValueSpliterator(Of K, V)(tab, baseSize, baseLimit = h, f, est >>>= 1))
			End Function

'JAVA TO VB CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
			Public Sub forEachRemaining(Of T1)(ByVal action As java.util.function.Consumer(Of T1))
				If action Is Nothing Then Throw New NullPointerException
				Dim p As Node(Of K, V)
'JAVA TO VB CONVERTER TODO TASK: Assignments within expressions are not supported in VB
				Do While (p = advance()) IsNot Nothing
					action.accept(p.val)
				Loop
			End Sub

'JAVA TO VB CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
			Public Function tryAdvance(Of T1)(ByVal action As java.util.function.Consumer(Of T1)) As Boolean
				If action Is Nothing Then Throw New NullPointerException
				Dim p As Node(Of K, V)
				p = advance()
				If p Is Nothing Then Return False
				action.accept(p.val)
				Return True
			End Function

			Public Function estimateSize() As Long
				Return est
			End Function

			Public Function characteristics() As Integer
				Return java.util.Spliterator.CONCURRENT Or java.util.Spliterator.NONNULL
			End Function
		End Class

		Friend NotInheritable Class EntrySpliterator(Of K, V)
			Inherits Traverser(Of K, V)
			Implements java.util.Spliterator(Of KeyValuePair(Of K, V))

			Friend ReadOnly map As ConcurrentHashMap(Of K, V) ' To export MapEntry
			Friend est As Long ' size estimate
			Friend Sub New(ByVal tab As Node(Of K, V)(), ByVal size As Integer, ByVal index As Integer, ByVal limit As Integer, ByVal est As Long, ByVal map As ConcurrentHashMap(Of K, V))
				MyBase.New(tab, size, index, limit)
				Me.map = map
				Me.est = est
			End Sub

			Public Function trySplit() As java.util.Spliterator(Of KeyValuePair(Of K, V))
				Dim i, f, h As Integer
'JAVA TO VB CONVERTER TODO TASK: Assignments within expressions are not supported in VB
				Return If((h = CInt(CUInt(((i = baseIndex) + (f = baseLimit))) >> 1)) <= i, Nothing, New EntrySpliterator(Of K, V)(tab, baseSize, baseLimit = h, f, est >>>= 1, map))
			End Function

'JAVA TO VB CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
			Public Sub forEachRemaining(Of T1)(ByVal action As java.util.function.Consumer(Of T1))
				If action Is Nothing Then Throw New NullPointerException
				Dim p As Node(Of K, V)
'JAVA TO VB CONVERTER TODO TASK: Assignments within expressions are not supported in VB
				Do While (p = advance()) IsNot Nothing
					action.accept(New MapEntry(Of K, V)(p.key, p.val, map))
				Loop
			End Sub

'JAVA TO VB CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
			Public Function tryAdvance(Of T1)(ByVal action As java.util.function.Consumer(Of T1)) As Boolean
				If action Is Nothing Then Throw New NullPointerException
				Dim p As Node(Of K, V)
				p = advance()
				If p Is Nothing Then Return False
				action.accept(New MapEntry(Of K, V)(p.key, p.val, map))
				Return True
			End Function

			Public Function estimateSize() As Long
				Return est
			End Function

			Public Function characteristics() As Integer
				Return java.util.Spliterator.DISTINCT Or java.util.Spliterator.CONCURRENT Or java.util.Spliterator.NONNULL
			End Function
		End Class

		' Parallel bulk operations

		''' <summary>
		''' Computes initial batch value for bulk tasks. The returned value
		''' is approximately exp2 of the number of times (minus one) to
		''' split task by two before executing leaf action. This value is
		''' faster to compute and more convenient to use as a guide to
		''' splitting than is the depth, since it is used while dividing by
		''' two anyway.
		''' </summary>
		Friend Function batchFor(ByVal b As Long) As Integer
			Dim n As Long
			n = sumCount()
			If b = java.lang.[Long].Max_Value OrElse n <= 1L OrElse n < b Then Return 0
			Dim sp As Integer = java.util.concurrent.ForkJoinPool.commonPoolParallelism << 2 ' slack of 4
'JAVA TO VB CONVERTER TODO TASK: Assignments within expressions are not supported in VB
			Return If(b <= 0L OrElse (n \= b) >= sp, sp, CInt(n))
		End Function

		''' <summary>
		''' Performs the given action for each (key, value).
		''' </summary>
		''' <param name="parallelismThreshold"> the (estimated) number of elements
		''' needed for this operation to be executed in parallel </param>
		''' <param name="action"> the action
		''' @since 1.8 </param>
'JAVA TO VB CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
		Public Overridable Sub forEach(Of T1)(ByVal parallelismThreshold As Long, ByVal action As java.util.function.BiConsumer(Of T1))
			If action Is Nothing Then Throw New NullPointerException
			CType(New ForEachMappingTask(Of K, V) (Nothing, batchFor(parallelismThreshold), 0, 0, table, action), ForEachMappingTask(Of K, V)).invoke()
		End Sub

		''' <summary>
		''' Performs the given action for each non-null transformation
		''' of each (key, value).
		''' </summary>
		''' <param name="parallelismThreshold"> the (estimated) number of elements
		''' needed for this operation to be executed in parallel </param>
		''' <param name="transformer"> a function returning the transformation
		''' for an element, or null if there is no transformation (in
		''' which case the action is not applied) </param>
		''' <param name="action"> the action </param>
		''' @param <U> the return type of the transformer
		''' @since 1.8 </param>
'JAVA TO VB CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
		Public Overridable Sub forEach(Of U, T1 As U, T2)(ByVal parallelismThreshold As Long, ByVal transformer As java.util.function.BiFunction(Of T1), ByVal action As java.util.function.Consumer(Of T2))
			If transformer Is Nothing OrElse action Is Nothing Then Throw New NullPointerException
			CType(New ForEachTransformedMappingTask(Of K, V, U) (Nothing, batchFor(parallelismThreshold), 0, 0, table, transformer, action), ForEachTransformedMappingTask(Of K, V, U)).invoke()
		End Sub

		''' <summary>
		''' Returns a non-null result from applying the given search
		''' function on each (key, value), or null if none.  Upon
		''' success, further element processing is suppressed and the
		''' results of any other parallel invocations of the search
		''' function are ignored.
		''' </summary>
		''' <param name="parallelismThreshold"> the (estimated) number of elements
		''' needed for this operation to be executed in parallel </param>
		''' <param name="searchFunction"> a function returning a non-null
		''' result on success, else null </param>
		''' @param <U> the return type of the search function </param>
		''' <returns> a non-null result from applying the given search
		''' function on each (key, value), or null if none
		''' @since 1.8 </returns>
'JAVA TO VB CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
		Public Overridable Function search(Of U, T1 As U)(ByVal parallelismThreshold As Long, ByVal searchFunction As java.util.function.BiFunction(Of T1)) As U
			If searchFunction Is Nothing Then Throw New NullPointerException
			Return (New SearchMappingsTask(Of K, V, U) (Nothing, batchFor(parallelismThreshold), 0, 0, table, searchFunction, New java.util.concurrent.atomic.AtomicReference(Of U))).invoke()
		End Function

		''' <summary>
		''' Returns the result of accumulating the given transformation
		''' of all (key, value) pairs using the given reducer to
		''' combine values, or null if none.
		''' </summary>
		''' <param name="parallelismThreshold"> the (estimated) number of elements
		''' needed for this operation to be executed in parallel </param>
		''' <param name="transformer"> a function returning the transformation
		''' for an element, or null if there is no transformation (in
		''' which case it is not combined) </param>
		''' <param name="reducer"> a commutative associative combining function </param>
		''' @param <U> the return type of the transformer </param>
		''' <returns> the result of accumulating the given transformation
		''' of all (key, value) pairs
		''' @since 1.8 </returns>
'JAVA TO VB CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
		Public Overridable Function reduce(Of U, T1 As U, T2 As U)(ByVal parallelismThreshold As Long, ByVal transformer As java.util.function.BiFunction(Of T1), ByVal reducer As java.util.function.BiFunction(Of T2)) As U
			If transformer Is Nothing OrElse reducer Is Nothing Then Throw New NullPointerException
			Return (New MapReduceMappingsTask(Of K, V, U) (Nothing, batchFor(parallelismThreshold), 0, 0, table, Nothing, transformer, reducer)).invoke()
		End Function

		''' <summary>
		''' Returns the result of accumulating the given transformation
		''' of all (key, value) pairs using the given reducer to
		''' combine values, and the given basis as an identity value.
		''' </summary>
		''' <param name="parallelismThreshold"> the (estimated) number of elements
		''' needed for this operation to be executed in parallel </param>
		''' <param name="transformer"> a function returning the transformation
		''' for an element </param>
		''' <param name="basis"> the identity (initial default value) for the reduction </param>
		''' <param name="reducer"> a commutative associative combining function </param>
		''' <returns> the result of accumulating the given transformation
		''' of all (key, value) pairs
		''' @since 1.8 </returns>
'JAVA TO VB CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
		Public Overridable Function reduceToDouble(Of T1)(ByVal parallelismThreshold As Long, ByVal transformer As java.util.function.ToDoubleBiFunction(Of T1), ByVal basis As Double, ByVal reducer As java.util.function.DoubleBinaryOperator) As Double
			If transformer Is Nothing OrElse reducer Is Nothing Then Throw New NullPointerException
			Return (New MapReduceMappingsToDoubleTask(Of K, V) (Nothing, batchFor(parallelismThreshold), 0, 0, table, Nothing, transformer, basis, reducer)).invoke()
		End Function

		''' <summary>
		''' Returns the result of accumulating the given transformation
		''' of all (key, value) pairs using the given reducer to
		''' combine values, and the given basis as an identity value.
		''' </summary>
		''' <param name="parallelismThreshold"> the (estimated) number of elements
		''' needed for this operation to be executed in parallel </param>
		''' <param name="transformer"> a function returning the transformation
		''' for an element </param>
		''' <param name="basis"> the identity (initial default value) for the reduction </param>
		''' <param name="reducer"> a commutative associative combining function </param>
		''' <returns> the result of accumulating the given transformation
		''' of all (key, value) pairs
		''' @since 1.8 </returns>
'JAVA TO VB CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
		Public Overridable Function reduceToLong(Of T1)(ByVal parallelismThreshold As Long, ByVal transformer As java.util.function.ToLongBiFunction(Of T1), ByVal basis As Long, ByVal reducer As java.util.function.LongBinaryOperator) As Long
			If transformer Is Nothing OrElse reducer Is Nothing Then Throw New NullPointerException
			Return (New MapReduceMappingsToLongTask(Of K, V) (Nothing, batchFor(parallelismThreshold), 0, 0, table, Nothing, transformer, basis, reducer)).invoke()
		End Function

		''' <summary>
		''' Returns the result of accumulating the given transformation
		''' of all (key, value) pairs using the given reducer to
		''' combine values, and the given basis as an identity value.
		''' </summary>
		''' <param name="parallelismThreshold"> the (estimated) number of elements
		''' needed for this operation to be executed in parallel </param>
		''' <param name="transformer"> a function returning the transformation
		''' for an element </param>
		''' <param name="basis"> the identity (initial default value) for the reduction </param>
		''' <param name="reducer"> a commutative associative combining function </param>
		''' <returns> the result of accumulating the given transformation
		''' of all (key, value) pairs
		''' @since 1.8 </returns>
'JAVA TO VB CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
		Public Overridable Function reduceToInt(Of T1)(ByVal parallelismThreshold As Long, ByVal transformer As java.util.function.ToIntBiFunction(Of T1), ByVal basis As Integer, ByVal reducer As java.util.function.IntBinaryOperator) As Integer
			If transformer Is Nothing OrElse reducer Is Nothing Then Throw New NullPointerException
			Return (New MapReduceMappingsToIntTask(Of K, V) (Nothing, batchFor(parallelismThreshold), 0, 0, table, Nothing, transformer, basis, reducer)).invoke()
		End Function

		''' <summary>
		''' Performs the given action for each key.
		''' </summary>
		''' <param name="parallelismThreshold"> the (estimated) number of elements
		''' needed for this operation to be executed in parallel </param>
		''' <param name="action"> the action
		''' @since 1.8 </param>
'JAVA TO VB CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
		Public Overridable Sub forEachKey(Of T1)(ByVal parallelismThreshold As Long, ByVal action As java.util.function.Consumer(Of T1))
			If action Is Nothing Then Throw New NullPointerException
			CType(New ForEachKeyTask(Of K, V) (Nothing, batchFor(parallelismThreshold), 0, 0, table, action), ForEachKeyTask(Of K, V)).invoke()
		End Sub

		''' <summary>
		''' Performs the given action for each non-null transformation
		''' of each key.
		''' </summary>
		''' <param name="parallelismThreshold"> the (estimated) number of elements
		''' needed for this operation to be executed in parallel </param>
		''' <param name="transformer"> a function returning the transformation
		''' for an element, or null if there is no transformation (in
		''' which case the action is not applied) </param>
		''' <param name="action"> the action </param>
		''' @param <U> the return type of the transformer
		''' @since 1.8 </param>
'JAVA TO VB CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
		Public Overridable Sub forEachKey(Of U, T1 As U, T2)(ByVal parallelismThreshold As Long, ByVal transformer As java.util.function.Function(Of T1), ByVal action As java.util.function.Consumer(Of T2))
			If transformer Is Nothing OrElse action Is Nothing Then Throw New NullPointerException
			CType(New ForEachTransformedKeyTask(Of K, V, U) (Nothing, batchFor(parallelismThreshold), 0, 0, table, transformer, action), ForEachTransformedKeyTask(Of K, V, U)).invoke()
		End Sub

		''' <summary>
		''' Returns a non-null result from applying the given search
		''' function on each key, or null if none. Upon success,
		''' further element processing is suppressed and the results of
		''' any other parallel invocations of the search function are
		''' ignored.
		''' </summary>
		''' <param name="parallelismThreshold"> the (estimated) number of elements
		''' needed for this operation to be executed in parallel </param>
		''' <param name="searchFunction"> a function returning a non-null
		''' result on success, else null </param>
		''' @param <U> the return type of the search function </param>
		''' <returns> a non-null result from applying the given search
		''' function on each key, or null if none
		''' @since 1.8 </returns>
'JAVA TO VB CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
		Public Overridable Function searchKeys(Of U, T1 As U)(ByVal parallelismThreshold As Long, ByVal searchFunction As java.util.function.Function(Of T1)) As U
			If searchFunction Is Nothing Then Throw New NullPointerException
			Return (New SearchKeysTask(Of K, V, U) (Nothing, batchFor(parallelismThreshold), 0, 0, table, searchFunction, New java.util.concurrent.atomic.AtomicReference(Of U))).invoke()
		End Function

		''' <summary>
		''' Returns the result of accumulating all keys using the given
		''' reducer to combine values, or null if none.
		''' </summary>
		''' <param name="parallelismThreshold"> the (estimated) number of elements
		''' needed for this operation to be executed in parallel </param>
		''' <param name="reducer"> a commutative associative combining function </param>
		''' <returns> the result of accumulating all keys using the given
		''' reducer to combine values, or null if none
		''' @since 1.8 </returns>
'JAVA TO VB CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
		Public Overridable Function reduceKeys(Of T1 As K)(ByVal parallelismThreshold As Long, ByVal reducer As java.util.function.BiFunction(Of T1)) As K
			If reducer Is Nothing Then Throw New NullPointerException
			Return (New ReduceKeysTask(Of K, V) (Nothing, batchFor(parallelismThreshold), 0, 0, table, Nothing, reducer)).invoke()
		End Function

		''' <summary>
		''' Returns the result of accumulating the given transformation
		''' of all keys using the given reducer to combine values, or
		''' null if none.
		''' </summary>
		''' <param name="parallelismThreshold"> the (estimated) number of elements
		''' needed for this operation to be executed in parallel </param>
		''' <param name="transformer"> a function returning the transformation
		''' for an element, or null if there is no transformation (in
		''' which case it is not combined) </param>
		''' <param name="reducer"> a commutative associative combining function </param>
		''' @param <U> the return type of the transformer </param>
		''' <returns> the result of accumulating the given transformation
		''' of all keys
		''' @since 1.8 </returns>
'JAVA TO VB CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
		Public Overridable Function reduceKeys(Of U, T1 As U, T2 As U)(ByVal parallelismThreshold As Long, ByVal transformer As java.util.function.Function(Of T1), ByVal reducer As java.util.function.BiFunction(Of T2)) As U
			If transformer Is Nothing OrElse reducer Is Nothing Then Throw New NullPointerException
			Return (New MapReduceKeysTask(Of K, V, U) (Nothing, batchFor(parallelismThreshold), 0, 0, table, Nothing, transformer, reducer)).invoke()
		End Function

		''' <summary>
		''' Returns the result of accumulating the given transformation
		''' of all keys using the given reducer to combine values, and
		''' the given basis as an identity value.
		''' </summary>
		''' <param name="parallelismThreshold"> the (estimated) number of elements
		''' needed for this operation to be executed in parallel </param>
		''' <param name="transformer"> a function returning the transformation
		''' for an element </param>
		''' <param name="basis"> the identity (initial default value) for the reduction </param>
		''' <param name="reducer"> a commutative associative combining function </param>
		''' <returns> the result of accumulating the given transformation
		''' of all keys
		''' @since 1.8 </returns>
'JAVA TO VB CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
		Public Overridable Function reduceKeysToDouble(Of T1)(ByVal parallelismThreshold As Long, ByVal transformer As java.util.function.ToDoubleFunction(Of T1), ByVal basis As Double, ByVal reducer As java.util.function.DoubleBinaryOperator) As Double
			If transformer Is Nothing OrElse reducer Is Nothing Then Throw New NullPointerException
			Return (New MapReduceKeysToDoubleTask(Of K, V) (Nothing, batchFor(parallelismThreshold), 0, 0, table, Nothing, transformer, basis, reducer)).invoke()
		End Function

		''' <summary>
		''' Returns the result of accumulating the given transformation
		''' of all keys using the given reducer to combine values, and
		''' the given basis as an identity value.
		''' </summary>
		''' <param name="parallelismThreshold"> the (estimated) number of elements
		''' needed for this operation to be executed in parallel </param>
		''' <param name="transformer"> a function returning the transformation
		''' for an element </param>
		''' <param name="basis"> the identity (initial default value) for the reduction </param>
		''' <param name="reducer"> a commutative associative combining function </param>
		''' <returns> the result of accumulating the given transformation
		''' of all keys
		''' @since 1.8 </returns>
'JAVA TO VB CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
		Public Overridable Function reduceKeysToLong(Of T1)(ByVal parallelismThreshold As Long, ByVal transformer As java.util.function.ToLongFunction(Of T1), ByVal basis As Long, ByVal reducer As java.util.function.LongBinaryOperator) As Long
			If transformer Is Nothing OrElse reducer Is Nothing Then Throw New NullPointerException
			Return (New MapReduceKeysToLongTask(Of K, V) (Nothing, batchFor(parallelismThreshold), 0, 0, table, Nothing, transformer, basis, reducer)).invoke()
		End Function

		''' <summary>
		''' Returns the result of accumulating the given transformation
		''' of all keys using the given reducer to combine values, and
		''' the given basis as an identity value.
		''' </summary>
		''' <param name="parallelismThreshold"> the (estimated) number of elements
		''' needed for this operation to be executed in parallel </param>
		''' <param name="transformer"> a function returning the transformation
		''' for an element </param>
		''' <param name="basis"> the identity (initial default value) for the reduction </param>
		''' <param name="reducer"> a commutative associative combining function </param>
		''' <returns> the result of accumulating the given transformation
		''' of all keys
		''' @since 1.8 </returns>
'JAVA TO VB CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
		Public Overridable Function reduceKeysToInt(Of T1)(ByVal parallelismThreshold As Long, ByVal transformer As java.util.function.ToIntFunction(Of T1), ByVal basis As Integer, ByVal reducer As java.util.function.IntBinaryOperator) As Integer
			If transformer Is Nothing OrElse reducer Is Nothing Then Throw New NullPointerException
			Return (New MapReduceKeysToIntTask(Of K, V) (Nothing, batchFor(parallelismThreshold), 0, 0, table, Nothing, transformer, basis, reducer)).invoke()
		End Function

		''' <summary>
		''' Performs the given action for each value.
		''' </summary>
		''' <param name="parallelismThreshold"> the (estimated) number of elements
		''' needed for this operation to be executed in parallel </param>
		''' <param name="action"> the action
		''' @since 1.8 </param>
'JAVA TO VB CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
		Public Overridable Sub forEachValue(Of T1)(ByVal parallelismThreshold As Long, ByVal action As java.util.function.Consumer(Of T1))
			If action Is Nothing Then Throw New NullPointerException
			CType(New ForEachValueTask(Of K, V) (Nothing, batchFor(parallelismThreshold), 0, 0, table, action), ForEachValueTask(Of K, V)).invoke()
		End Sub

		''' <summary>
		''' Performs the given action for each non-null transformation
		''' of each value.
		''' </summary>
		''' <param name="parallelismThreshold"> the (estimated) number of elements
		''' needed for this operation to be executed in parallel </param>
		''' <param name="transformer"> a function returning the transformation
		''' for an element, or null if there is no transformation (in
		''' which case the action is not applied) </param>
		''' <param name="action"> the action </param>
		''' @param <U> the return type of the transformer
		''' @since 1.8 </param>
'JAVA TO VB CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
		Public Overridable Sub forEachValue(Of U, T1 As U, T2)(ByVal parallelismThreshold As Long, ByVal transformer As java.util.function.Function(Of T1), ByVal action As java.util.function.Consumer(Of T2))
			If transformer Is Nothing OrElse action Is Nothing Then Throw New NullPointerException
			CType(New ForEachTransformedValueTask(Of K, V, U) (Nothing, batchFor(parallelismThreshold), 0, 0, table, transformer, action), ForEachTransformedValueTask(Of K, V, U)).invoke()
		End Sub

		''' <summary>
		''' Returns a non-null result from applying the given search
		''' function on each value, or null if none.  Upon success,
		''' further element processing is suppressed and the results of
		''' any other parallel invocations of the search function are
		''' ignored.
		''' </summary>
		''' <param name="parallelismThreshold"> the (estimated) number of elements
		''' needed for this operation to be executed in parallel </param>
		''' <param name="searchFunction"> a function returning a non-null
		''' result on success, else null </param>
		''' @param <U> the return type of the search function </param>
		''' <returns> a non-null result from applying the given search
		''' function on each value, or null if none
		''' @since 1.8 </returns>
'JAVA TO VB CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
		Public Overridable Function searchValues(Of U, T1 As U)(ByVal parallelismThreshold As Long, ByVal searchFunction As java.util.function.Function(Of T1)) As U
			If searchFunction Is Nothing Then Throw New NullPointerException
			Return (New SearchValuesTask(Of K, V, U) (Nothing, batchFor(parallelismThreshold), 0, 0, table, searchFunction, New java.util.concurrent.atomic.AtomicReference(Of U))).invoke()
		End Function

		''' <summary>
		''' Returns the result of accumulating all values using the
		''' given reducer to combine values, or null if none.
		''' </summary>
		''' <param name="parallelismThreshold"> the (estimated) number of elements
		''' needed for this operation to be executed in parallel </param>
		''' <param name="reducer"> a commutative associative combining function </param>
		''' <returns> the result of accumulating all values
		''' @since 1.8 </returns>
'JAVA TO VB CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
		Public Overridable Function reduceValues(Of T1 As V)(ByVal parallelismThreshold As Long, ByVal reducer As java.util.function.BiFunction(Of T1)) As V
			If reducer Is Nothing Then Throw New NullPointerException
			Return (New ReduceValuesTask(Of K, V) (Nothing, batchFor(parallelismThreshold), 0, 0, table, Nothing, reducer)).invoke()
		End Function

		''' <summary>
		''' Returns the result of accumulating the given transformation
		''' of all values using the given reducer to combine values, or
		''' null if none.
		''' </summary>
		''' <param name="parallelismThreshold"> the (estimated) number of elements
		''' needed for this operation to be executed in parallel </param>
		''' <param name="transformer"> a function returning the transformation
		''' for an element, or null if there is no transformation (in
		''' which case it is not combined) </param>
		''' <param name="reducer"> a commutative associative combining function </param>
		''' @param <U> the return type of the transformer </param>
		''' <returns> the result of accumulating the given transformation
		''' of all values
		''' @since 1.8 </returns>
'JAVA TO VB CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
		Public Overridable Function reduceValues(Of U, T1 As U, T2 As U)(ByVal parallelismThreshold As Long, ByVal transformer As java.util.function.Function(Of T1), ByVal reducer As java.util.function.BiFunction(Of T2)) As U
			If transformer Is Nothing OrElse reducer Is Nothing Then Throw New NullPointerException
			Return (New MapReduceValuesTask(Of K, V, U) (Nothing, batchFor(parallelismThreshold), 0, 0, table, Nothing, transformer, reducer)).invoke()
		End Function

		''' <summary>
		''' Returns the result of accumulating the given transformation
		''' of all values using the given reducer to combine values,
		''' and the given basis as an identity value.
		''' </summary>
		''' <param name="parallelismThreshold"> the (estimated) number of elements
		''' needed for this operation to be executed in parallel </param>
		''' <param name="transformer"> a function returning the transformation
		''' for an element </param>
		''' <param name="basis"> the identity (initial default value) for the reduction </param>
		''' <param name="reducer"> a commutative associative combining function </param>
		''' <returns> the result of accumulating the given transformation
		''' of all values
		''' @since 1.8 </returns>
'JAVA TO VB CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
		Public Overridable Function reduceValuesToDouble(Of T1)(ByVal parallelismThreshold As Long, ByVal transformer As java.util.function.ToDoubleFunction(Of T1), ByVal basis As Double, ByVal reducer As java.util.function.DoubleBinaryOperator) As Double
			If transformer Is Nothing OrElse reducer Is Nothing Then Throw New NullPointerException
			Return (New MapReduceValuesToDoubleTask(Of K, V) (Nothing, batchFor(parallelismThreshold), 0, 0, table, Nothing, transformer, basis, reducer)).invoke()
		End Function

		''' <summary>
		''' Returns the result of accumulating the given transformation
		''' of all values using the given reducer to combine values,
		''' and the given basis as an identity value.
		''' </summary>
		''' <param name="parallelismThreshold"> the (estimated) number of elements
		''' needed for this operation to be executed in parallel </param>
		''' <param name="transformer"> a function returning the transformation
		''' for an element </param>
		''' <param name="basis"> the identity (initial default value) for the reduction </param>
		''' <param name="reducer"> a commutative associative combining function </param>
		''' <returns> the result of accumulating the given transformation
		''' of all values
		''' @since 1.8 </returns>
'JAVA TO VB CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
		Public Overridable Function reduceValuesToLong(Of T1)(ByVal parallelismThreshold As Long, ByVal transformer As java.util.function.ToLongFunction(Of T1), ByVal basis As Long, ByVal reducer As java.util.function.LongBinaryOperator) As Long
			If transformer Is Nothing OrElse reducer Is Nothing Then Throw New NullPointerException
			Return (New MapReduceValuesToLongTask(Of K, V) (Nothing, batchFor(parallelismThreshold), 0, 0, table, Nothing, transformer, basis, reducer)).invoke()
		End Function

		''' <summary>
		''' Returns the result of accumulating the given transformation
		''' of all values using the given reducer to combine values,
		''' and the given basis as an identity value.
		''' </summary>
		''' <param name="parallelismThreshold"> the (estimated) number of elements
		''' needed for this operation to be executed in parallel </param>
		''' <param name="transformer"> a function returning the transformation
		''' for an element </param>
		''' <param name="basis"> the identity (initial default value) for the reduction </param>
		''' <param name="reducer"> a commutative associative combining function </param>
		''' <returns> the result of accumulating the given transformation
		''' of all values
		''' @since 1.8 </returns>
'JAVA TO VB CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
		Public Overridable Function reduceValuesToInt(Of T1)(ByVal parallelismThreshold As Long, ByVal transformer As java.util.function.ToIntFunction(Of T1), ByVal basis As Integer, ByVal reducer As java.util.function.IntBinaryOperator) As Integer
			If transformer Is Nothing OrElse reducer Is Nothing Then Throw New NullPointerException
			Return (New MapReduceValuesToIntTask(Of K, V) (Nothing, batchFor(parallelismThreshold), 0, 0, table, Nothing, transformer, basis, reducer)).invoke()
		End Function

		''' <summary>
		''' Performs the given action for each entry.
		''' </summary>
		''' <param name="parallelismThreshold"> the (estimated) number of elements
		''' needed for this operation to be executed in parallel </param>
		''' <param name="action"> the action
		''' @since 1.8 </param>
'JAVA TO VB CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
		Public Overridable Sub forEachEntry(Of T1)(ByVal parallelismThreshold As Long, ByVal action As java.util.function.Consumer(Of T1))
			If action Is Nothing Then Throw New NullPointerException
			CType(New ForEachEntryTask(Of K, V)(Nothing, batchFor(parallelismThreshold), 0, 0, table, action), ForEachEntryTask(Of K, V)).invoke()
		End Sub

		''' <summary>
		''' Performs the given action for each non-null transformation
		''' of each entry.
		''' </summary>
		''' <param name="parallelismThreshold"> the (estimated) number of elements
		''' needed for this operation to be executed in parallel </param>
		''' <param name="transformer"> a function returning the transformation
		''' for an element, or null if there is no transformation (in
		''' which case the action is not applied) </param>
		''' <param name="action"> the action </param>
		''' @param <U> the return type of the transformer
		''' @since 1.8 </param>
'JAVA TO VB CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
		Public Overridable Sub forEachEntry(Of U, T1 As U, T2)(ByVal parallelismThreshold As Long, ByVal transformer As java.util.function.Function(Of T1), ByVal action As java.util.function.Consumer(Of T2))
			If transformer Is Nothing OrElse action Is Nothing Then Throw New NullPointerException
			CType(New ForEachTransformedEntryTask(Of K, V, U) (Nothing, batchFor(parallelismThreshold), 0, 0, table, transformer, action), ForEachTransformedEntryTask(Of K, V, U)).invoke()
		End Sub

		''' <summary>
		''' Returns a non-null result from applying the given search
		''' function on each entry, or null if none.  Upon success,
		''' further element processing is suppressed and the results of
		''' any other parallel invocations of the search function are
		''' ignored.
		''' </summary>
		''' <param name="parallelismThreshold"> the (estimated) number of elements
		''' needed for this operation to be executed in parallel </param>
		''' <param name="searchFunction"> a function returning a non-null
		''' result on success, else null </param>
		''' @param <U> the return type of the search function </param>
		''' <returns> a non-null result from applying the given search
		''' function on each entry, or null if none
		''' @since 1.8 </returns>
		Public Overridable Function searchEntries(Of U, T1 As U)(ByVal parallelismThreshold As Long, ByVal searchFunction As java.util.function.Function(Of T1)) As U
			If searchFunction Is Nothing Then Throw New NullPointerException
			Return (New SearchEntriesTask(Of K, V, U) (Nothing, batchFor(parallelismThreshold), 0, 0, table, searchFunction, New java.util.concurrent.atomic.AtomicReference(Of U))).invoke()
		End Function

		''' <summary>
		''' Returns the result of accumulating all entries using the
		''' given reducer to combine values, or null if none.
		''' </summary>
		''' <param name="parallelismThreshold"> the (estimated) number of elements
		''' needed for this operation to be executed in parallel </param>
		''' <param name="reducer"> a commutative associative combining function </param>
		''' <returns> the result of accumulating all entries
		''' @since 1.8 </returns>
		Public Overridable Function reduceEntries(Of T1 As KeyValuePair(Of K, V)(ByVal parallelismThreshold As Long, ByVal reducer As java.util.function.BiFunction(Of T1)) As KeyValuePair(Of K, V)
			If reducer Is Nothing Then Throw New NullPointerException
			Return (New ReduceEntriesTask(Of K, V) (Nothing, batchFor(parallelismThreshold), 0, 0, table, Nothing, reducer)).invoke()
		End Function

		''' <summary>
		''' Returns the result of accumulating the given transformation
		''' of all entries using the given reducer to combine values,
		''' or null if none.
		''' </summary>
		''' <param name="parallelismThreshold"> the (estimated) number of elements
		''' needed for this operation to be executed in parallel </param>
		''' <param name="transformer"> a function returning the transformation
		''' for an element, or null if there is no transformation (in
		''' which case it is not combined) </param>
		''' <param name="reducer"> a commutative associative combining function </param>
		''' @param <U> the return type of the transformer </param>
		''' <returns> the result of accumulating the given transformation
		''' of all entries
		''' @since 1.8 </returns>
'JAVA TO VB CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
		Public Overridable Function reduceEntries(Of U, T1 As U, T2 As U)(ByVal parallelismThreshold As Long, ByVal transformer As java.util.function.Function(Of T1), ByVal reducer As java.util.function.BiFunction(Of T2)) As U
			If transformer Is Nothing OrElse reducer Is Nothing Then Throw New NullPointerException
			Return (New MapReduceEntriesTask(Of K, V, U) (Nothing, batchFor(parallelismThreshold), 0, 0, table, Nothing, transformer, reducer)).invoke()
		End Function

		''' <summary>
		''' Returns the result of accumulating the given transformation
		''' of all entries using the given reducer to combine values,
		''' and the given basis as an identity value.
		''' </summary>
		''' <param name="parallelismThreshold"> the (estimated) number of elements
		''' needed for this operation to be executed in parallel </param>
		''' <param name="transformer"> a function returning the transformation
		''' for an element </param>
		''' <param name="basis"> the identity (initial default value) for the reduction </param>
		''' <param name="reducer"> a commutative associative combining function </param>
		''' <returns> the result of accumulating the given transformation
		''' of all entries
		''' @since 1.8 </returns>
		Public Overridable Function reduceEntriesToDouble(ByVal parallelismThreshold As Long, ByVal transformer As java.util.function.ToDoubleFunction(Of KeyValuePair(Of K, V)), ByVal basis As Double, ByVal reducer As java.util.function.DoubleBinaryOperator) As Double
			If transformer Is Nothing OrElse reducer Is Nothing Then Throw New NullPointerException
			Return (New MapReduceEntriesToDoubleTask(Of K, V) (Nothing, batchFor(parallelismThreshold), 0, 0, table, Nothing, transformer, basis, reducer)).invoke()
		End Function

		''' <summary>
		''' Returns the result of accumulating the given transformation
		''' of all entries using the given reducer to combine values,
		''' and the given basis as an identity value.
		''' </summary>
		''' <param name="parallelismThreshold"> the (estimated) number of elements
		''' needed for this operation to be executed in parallel </param>
		''' <param name="transformer"> a function returning the transformation
		''' for an element </param>
		''' <param name="basis"> the identity (initial default value) for the reduction </param>
		''' <param name="reducer"> a commutative associative combining function </param>
		''' <returns> the result of accumulating the given transformation
		''' of all entries
		''' @since 1.8 </returns>
		Public Overridable Function reduceEntriesToLong(ByVal parallelismThreshold As Long, ByVal transformer As java.util.function.ToLongFunction(Of KeyValuePair(Of K, V)), ByVal basis As Long, ByVal reducer As java.util.function.LongBinaryOperator) As Long
			If transformer Is Nothing OrElse reducer Is Nothing Then Throw New NullPointerException
			Return (New MapReduceEntriesToLongTask(Of K, V) (Nothing, batchFor(parallelismThreshold), 0, 0, table, Nothing, transformer, basis, reducer)).invoke()
		End Function

		''' <summary>
		''' Returns the result of accumulating the given transformation
		''' of all entries using the given reducer to combine values,
		''' and the given basis as an identity value.
		''' </summary>
		''' <param name="parallelismThreshold"> the (estimated) number of elements
		''' needed for this operation to be executed in parallel </param>
		''' <param name="transformer"> a function returning the transformation
		''' for an element </param>
		''' <param name="basis"> the identity (initial default value) for the reduction </param>
		''' <param name="reducer"> a commutative associative combining function </param>
		''' <returns> the result of accumulating the given transformation
		''' of all entries
		''' @since 1.8 </returns>
		Public Overridable Function reduceEntriesToInt(ByVal parallelismThreshold As Long, ByVal transformer As java.util.function.ToIntFunction(Of KeyValuePair(Of K, V)), ByVal basis As Integer, ByVal reducer As java.util.function.IntBinaryOperator) As Integer
			If transformer Is Nothing OrElse reducer Is Nothing Then Throw New NullPointerException
			Return (New MapReduceEntriesToIntTask(Of K, V) (Nothing, batchFor(parallelismThreshold), 0, 0, table, Nothing, transformer, basis, reducer)).invoke()
		End Function


		' ----------------Views -------------- 

		''' <summary>
		''' Base class for views.
		''' </summary>
		<Serializable> _
		Friend MustInherit Class CollectionView(Of K, V, E)
			Implements ICollection(Of E)

			Private Const serialVersionUID As Long = 7249069246763182397L
			Friend ReadOnly map As ConcurrentHashMap(Of K, V)
			Friend Sub New(ByVal map As ConcurrentHashMap(Of K, V))
				Me.map = map
			End Sub

			''' <summary>
			''' Returns the map backing this view.
			''' </summary>
			''' <returns> the map backing this view </returns>
			Public Overridable Property map As ConcurrentHashMap(Of K, V)
				Get
					Return map
				End Get
			End Property

			''' <summary>
			''' Removes all of the elements from this view, by removing all
			''' the mappings from the map backing this view.
			''' </summary>
			Public Sub clear()
				map.clear()
			End Sub
			Public Function size() As Integer
				Return map.size()
			End Function
			Public Property empty As Boolean
				Get
					Return map.empty
				End Get
			End Property

			' implementations below rely on concrete classes supplying these
			' abstract methods
			''' <summary>
			''' Returns an iterator over the elements in this collection.
			''' 
			''' <p>The returned iterator is
			''' <a href="package-summary.html#Weakly"><i>weakly consistent</i></a>.
			''' </summary>
			''' <returns> an iterator over the elements in this collection </returns>
			Public MustOverride Function [iterator]() As IEnumerator(Of E)
			Public MustOverride Function contains(ByVal o As Object) As Boolean
			Public MustOverride Function remove(ByVal o As Object) As Boolean

			Private Const oomeMsg As String = "Required array size too large"

			Public Function toArray() As Object()
				Dim sz As Long = map.mappingCount()
				If sz > MAX_ARRAY_SIZE Then Throw New OutOfMemoryError(oomeMsg)
				Dim n As Integer = CInt(sz)
				Dim r As Object() = New Object(n - 1){}
				Dim i As Integer = 0
				For Each e As E In Me
					If i = n Then
						If n >= MAX_ARRAY_SIZE Then Throw New OutOfMemoryError(oomeMsg)
						If n >= MAX_ARRAY_SIZE - (CInt(CUInt(MAX_ARRAY_SIZE) >> 1)) - 1 Then
							n = MAX_ARRAY_SIZE
						Else
							n += (CInt(CUInt(n) >> 1)) + 1
						End If
						r = java.util.Arrays.copyOf(r, n)
					End If
					r(i) = e
					i += 1
				Next e
				Return If(i = n, r, java.util.Arrays.copyOf(r, i))
			End Function

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
			Public Function toArray(Of T)(ByVal a As T()) As T()
				Dim sz As Long = map.mappingCount()
				If sz > MAX_ARRAY_SIZE Then Throw New OutOfMemoryError(oomeMsg)
				Dim m As Integer = CInt(sz)
				Dim r As T() = If(a.Length >= m, a, CType(java.lang.reflect.Array.newInstance(a.GetType().GetElementType(), m), T()))
				Dim n As Integer = r.Length
				Dim i As Integer = 0
				For Each e As E In Me
					If i = n Then
						If n >= MAX_ARRAY_SIZE Then Throw New OutOfMemoryError(oomeMsg)
						If n >= MAX_ARRAY_SIZE - (CInt(CUInt(MAX_ARRAY_SIZE) >> 1)) - 1 Then
							n = MAX_ARRAY_SIZE
						Else
							n += (CInt(CUInt(n) >> 1)) + 1
						End If
						r = java.util.Arrays.copyOf(r, n)
					End If
					r(i) = CType(e, T)
					i += 1
				Next e
				If a = r AndAlso i < n Then
					r(i) = Nothing ' null-terminate
					Return r
				End If
				Return If(i = n, r, java.util.Arrays.copyOf(r, i))
			End Function

			''' <summary>
			''' Returns a string representation of this collection.
			''' The string representation consists of the string representations
			''' of the collection's elements in the order they are returned by
			''' its iterator, enclosed in square brackets ({@code "[]"}).
			''' Adjacent elements are separated by the characters {@code ", "}
			''' (comma and space).  Elements are converted to strings as by
			''' <seealso cref="String#valueOf(Object)"/>.
			''' </summary>
			''' <returns> a string representation of this collection </returns>
			Public NotOverridable Overrides Function ToString() As String
				Dim sb As New StringBuilder
				sb.append("["c)
				Dim it As IEnumerator(Of E) = [iterator]()
'JAVA TO VB CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
				If it.hasNext() Then
					Do
'JAVA TO VB CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
						Dim e As Object = it.next()
						sb.append(If(e Is Me, "(this Collection)", e))
'JAVA TO VB CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
						If Not it.hasNext() Then Exit Do
						sb.append(","c).append(" "c)
					Loop
				End If
				Return sb.append("]"c).ToString()
			End Function

			Public Function containsAll(Of T1)(ByVal c As ICollection(Of T1)) As Boolean
				If c IsNot Me Then
					For Each e As Object In c
						If e Is Nothing OrElse (Not contains(e)) Then Return False
					Next e
				End If
				Return True
			End Function

			Public Function removeAll(Of T1)(ByVal c As ICollection(Of T1)) As Boolean
				If c Is Nothing Then Throw New NullPointerException
				Dim modified As Boolean = False
				Dim it As IEnumerator(Of E) = [iterator]()
				Do While it.MoveNext()
					If c.Contains(it.Current) Then
						it.remove()
						modified = True
					End If
				Loop
				Return modified
			End Function

			Public Function retainAll(Of T1)(ByVal c As ICollection(Of T1)) As Boolean
				If c Is Nothing Then Throw New NullPointerException
				Dim modified As Boolean = False
				Dim it As IEnumerator(Of E) = [iterator]()
				Do While it.MoveNext()
					If Not c.Contains(it.Current) Then
						it.remove()
						modified = True
					End If
				Loop
				Return modified
			End Function

		End Class

		''' <summary>
		''' A view of a ConcurrentHashMap as a <seealso cref="Set"/> of keys, in
		''' which additions may optionally be enabled by mapping to a
		''' common value.  This class cannot be directly instantiated.
		''' See <seealso cref="#keySet() keySet()"/>,
		''' <seealso cref="#keySet(Object) keySet(V)"/>,
		''' <seealso cref="#newKeySet() newKeySet()"/>,
		''' <seealso cref="#newKeySet(int) newKeySet(int)"/>.
		''' 
		''' @since 1.8
		''' </summary>
		<Serializable> _
		Public Class KeySetView(Of K, V)
			Inherits CollectionView(Of K, V, K)
			Implements java.util.Set(Of K)

			Private Const serialVersionUID As Long = 7249069246763182397L
			Private ReadOnly value As V
			Friend Sub New(ByVal map As ConcurrentHashMap(Of K, V), ByVal value As V) ' non-public
				MyBase.New(map)
				Me.value = value
			End Sub

			''' <summary>
			''' Returns the default mapped value for additions,
			''' or {@code null} if additions are not supported.
			''' </summary>
			''' <returns> the default mapped value for additions, or {@code null}
			''' if not supported </returns>
			Public Overridable Property mappedValue As V
				Get
					Return value
				End Get
			End Property

			''' <summary>
			''' {@inheritDoc} </summary>
			''' <exception cref="NullPointerException"> if the specified key is null </exception>
			Public Overridable Function contains(ByVal o As Object) As Boolean
				Return map.containsKey(o)
			End Function

			''' <summary>
			''' Removes the key from this map view, by removing the key (and its
			''' corresponding value) from the backing map.  This method does
			''' nothing if the key is not in the map.
			''' </summary>
			''' <param name="o"> the key to be removed from the backing map </param>
			''' <returns> {@code true} if the backing map contained the specified key </returns>
			''' <exception cref="NullPointerException"> if the specified key is null </exception>
			Public Overridable Function remove(ByVal o As Object) As Boolean
				Return map.remove(o) IsNot Nothing
			End Function

			''' <returns> an iterator over the keys of the backing map </returns>
			Public Overridable Function [iterator]() As IEnumerator(Of K)
				Dim t As Node(Of K, V)()
				Dim m As ConcurrentHashMap(Of K, V) = map
'JAVA TO VB CONVERTER TODO TASK: Assignments within expressions are not supported in VB
				Dim f As Integer = If((t = m.table) Is Nothing, 0, t.Length)
				Return New KeyIterator(Of K, V)(t, f, 0, f, m)
			End Function

			''' <summary>
			''' Adds the specified key to this set view by mapping the key to
			''' the default mapped value in the backing map, if defined.
			''' </summary>
			''' <param name="e"> key to be added </param>
			''' <returns> {@code true} if this set changed as a result of the call </returns>
			''' <exception cref="NullPointerException"> if the specified key is null </exception>
			''' <exception cref="UnsupportedOperationException"> if no default mapped value
			''' for additions was provided </exception>
			Public Overridable Function add(ByVal e As K) As Boolean
				Dim v As V
				v = value
				If v Is Nothing Then Throw New UnsupportedOperationException
				Return map.putVal(e, v, True) Is Nothing
			End Function

			''' <summary>
			''' Adds all of the elements in the specified collection to this set,
			''' as if by calling <seealso cref="#add"/> on each one.
			''' </summary>
			''' <param name="c"> the elements to be inserted into this set </param>
			''' <returns> {@code true} if this set changed as a result of the call </returns>
			''' <exception cref="NullPointerException"> if the collection or any of its
			''' elements are {@code null} </exception>
			''' <exception cref="UnsupportedOperationException"> if no default mapped value
			''' for additions was provided </exception>
			Public Overridable Function addAll(Of T1 As K)(ByVal c As ICollection(Of T1)) As Boolean
				Dim added As Boolean = False
				Dim v As V
				v = value
				If v Is Nothing Then Throw New UnsupportedOperationException
				For Each e As K In c
					If map.putVal(e, v, True) Is Nothing Then added = True
				Next e
				Return added
			End Function

			Public Overrides Function GetHashCode() As Integer
				Dim h As Integer = 0
				For Each e As K In Me
					h += e.GetHashCode()
				Next e
				Return h
			End Function

			Public Overrides Function Equals(ByVal o As Object) As Boolean
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
				Dim c As java.util.Set(Of ?)
'JAVA TO VB CONVERTER TODO TASK: Assignments within expressions are not supported in VB
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
				Return ((TypeOf o Is java.util.Set) AndAlso ((c = CType(o, java.util.Set(Of ?))) Is Me OrElse (containsAll(c) AndAlso c.containsAll(Me))))
			End Function

			Public Overridable Function spliterator() As java.util.Spliterator(Of K)
				Dim t As Node(Of K, V)()
				Dim m As ConcurrentHashMap(Of K, V) = map
				Dim n As Long = m.sumCount()
'JAVA TO VB CONVERTER TODO TASK: Assignments within expressions are not supported in VB
				Dim f As Integer = If((t = m.table) Is Nothing, 0, t.Length)
				Return New KeySpliterator(Of K, V)(t, f, 0, f,If(n < 0L, 0L, n))
			End Function

'JAVA TO VB CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
			Public Overridable Sub forEach(Of T1)(ByVal action As java.util.function.Consumer(Of T1))
				If action Is Nothing Then Throw New NullPointerException
				Dim t As Node(Of K, V)()
				t = map.table
				If t IsNot Nothing Then
					Dim it As New Traverser(Of K, V)(t, t.Length, 0, t.Length)
					Dim p As Node(Of K, V)
'JAVA TO VB CONVERTER TODO TASK: Assignments within expressions are not supported in VB
					Do While (p = it.advance()) IsNot Nothing
						action.accept(p.key)
					Loop
				End If
			End Sub
		End Class

		''' <summary>
		''' A view of a ConcurrentHashMap as a <seealso cref="Collection"/> of
		''' values, in which additions are disabled. This class cannot be
		''' directly instantiated. See <seealso cref="#values()"/>.
		''' </summary>
		<Serializable> _
		Friend NotInheritable Class ValuesView(Of K, V)
			Inherits CollectionView(Of K, V, V)
			Implements ICollection(Of V)

			Private Const serialVersionUID As Long = 2249069246763182397L
			Friend Sub New(ByVal map As ConcurrentHashMap(Of K, V))
				MyBase.New(map)
			End Sub
			Public Function contains(ByVal o As Object) As Boolean
				Return map.containsValue(o)
			End Function

			Public Function remove(ByVal o As Object) As Boolean
				If o IsNot Nothing Then
					Dim it As IEnumerator(Of V) = [iterator]()
					Do While it.MoveNext()
						If o.Equals(it.Current) Then
							it.remove()
							Return True
						End If
					Loop
				End If
				Return False
			End Function

			Public Function [iterator]() As IEnumerator(Of V)
				Dim m As ConcurrentHashMap(Of K, V) = map
				Dim t As Node(Of K, V)()
'JAVA TO VB CONVERTER TODO TASK: Assignments within expressions are not supported in VB
				Dim f As Integer = If((t = m.table) Is Nothing, 0, t.Length)
				Return New ValueIterator(Of K, V)(t, f, 0, f, m)
			End Function

			Public Function add(ByVal e As V) As Boolean
				Throw New UnsupportedOperationException
			End Function
			Public Function addAll(Of T1 As V)(ByVal c As ICollection(Of T1)) As Boolean
				Throw New UnsupportedOperationException
			End Function

			Public Function spliterator() As java.util.Spliterator(Of V)
				Dim t As Node(Of K, V)()
				Dim m As ConcurrentHashMap(Of K, V) = map
				Dim n As Long = m.sumCount()
'JAVA TO VB CONVERTER TODO TASK: Assignments within expressions are not supported in VB
				Dim f As Integer = If((t = m.table) Is Nothing, 0, t.Length)
				Return New ValueSpliterator(Of K, V)(t, f, 0, f,If(n < 0L, 0L, n))
			End Function

'JAVA TO VB CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
			Public Sub forEach(Of T1)(ByVal action As java.util.function.Consumer(Of T1))
				If action Is Nothing Then Throw New NullPointerException
				Dim t As Node(Of K, V)()
				t = map.table
				If t IsNot Nothing Then
					Dim it As New Traverser(Of K, V)(t, t.Length, 0, t.Length)
					Dim p As Node(Of K, V)
'JAVA TO VB CONVERTER TODO TASK: Assignments within expressions are not supported in VB
					Do While (p = it.advance()) IsNot Nothing
						action.accept(p.val)
					Loop
				End If
			End Sub
		End Class

		''' <summary>
		''' A view of a ConcurrentHashMap as a <seealso cref="Set"/> of (key, value)
		''' entries.  This class cannot be directly instantiated. See
		''' <seealso cref="#entrySet()"/>.
		''' </summary>
		<Serializable> _
		Friend NotInheritable Class EntrySetView(Of K, V)
			Inherits CollectionView(Of K, V, KeyValuePair(Of K, V))
			Implements java.util.Set(Of KeyValuePair(Of K, V))

			Private Const serialVersionUID As Long = 2249069246763182397L
			Friend Sub New(ByVal map As ConcurrentHashMap(Of K, V))
				MyBase.New(map)
			End Sub

			Public Function contains(ByVal o As Object) As Boolean
				Dim k, v, r As Object
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
				Dim e As KeyValuePair(Of ?, ?)
'JAVA TO VB CONVERTER TODO TASK: Assignments within expressions are not supported in VB
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
				Return ((TypeOf o Is DictionaryEntry) AndAlso (k = (e = CType(o, KeyValuePair(Of ?, ?))).key) IsNot Nothing AndAlso (r = map.get(k)) IsNot Nothing AndAlso (v = e.Value) IsNot Nothing AndAlso (v Is r OrElse v.Equals(r)))
			End Function

			Public Function remove(ByVal o As Object) As Boolean
				Dim k, v As Object
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
				Dim e As KeyValuePair(Of ?, ?)
'JAVA TO VB CONVERTER TODO TASK: Assignments within expressions are not supported in VB
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
				Return ((TypeOf o Is DictionaryEntry) AndAlso (k = (e = CType(o, KeyValuePair(Of ?, ?))).key) IsNot Nothing AndAlso (v = e.Value) IsNot Nothing AndAlso map.remove(k, v))
			End Function

			''' <returns> an iterator over the entries of the backing map </returns>
			Public Function [iterator]() As IEnumerator(Of KeyValuePair(Of K, V))
				Dim m As ConcurrentHashMap(Of K, V) = map
				Dim t As Node(Of K, V)()
'JAVA TO VB CONVERTER TODO TASK: Assignments within expressions are not supported in VB
				Dim f As Integer = If((t = m.table) Is Nothing, 0, t.Length)
				Return New EntryIterator(Of K, V)(t, f, 0, f, m)
			End Function

			Public Function add(ByVal e As Entry(Of K, V)) As Boolean
				Return map.putVal(e.key, e.value, False) Is Nothing
			End Function

			Public Function addAll(Of T1 As Entry(Of K, V)(ByVal c As ICollection(Of T1)) As Boolean
				Dim added As Boolean = False
				For Each e As Entry(Of K, V) In c
					If add(e) Then added = True
				Next e
				Return added
			End Function

			Public NotOverridable Overrides Function GetHashCode() As Integer
				Dim h As Integer = 0
				Dim t As Node(Of K, V)()
				t = map.table
				If t IsNot Nothing Then
					Dim it As New Traverser(Of K, V)(t, t.Length, 0, t.Length)
					Dim p As Node(Of K, V)
'JAVA TO VB CONVERTER TODO TASK: Assignments within expressions are not supported in VB
					Do While (p = it.advance()) IsNot Nothing
						h += p.GetHashCode()
					Loop
				End If
				Return h
			End Function

			Public NotOverridable Overrides Function Equals(ByVal o As Object) As Boolean
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
				Dim c As java.util.Set(Of ?)
'JAVA TO VB CONVERTER TODO TASK: Assignments within expressions are not supported in VB
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
				Return ((TypeOf o Is java.util.Set) AndAlso ((c = CType(o, java.util.Set(Of ?))) Is Me OrElse (containsAll(c) AndAlso c.containsAll(Me))))
			End Function

			Public Function spliterator() As java.util.Spliterator(Of KeyValuePair(Of K, V))
				Dim t As Node(Of K, V)()
				Dim m As ConcurrentHashMap(Of K, V) = map
				Dim n As Long = m.sumCount()
'JAVA TO VB CONVERTER TODO TASK: Assignments within expressions are not supported in VB
				Dim f As Integer = If((t = m.table) Is Nothing, 0, t.Length)
				Return New EntrySpliterator(Of K, V)(t, f, 0, f,If(n < 0L, 0L, n), m)
			End Function

'JAVA TO VB CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
			Public Sub forEach(Of T1)(ByVal action As java.util.function.Consumer(Of T1))
				If action Is Nothing Then Throw New NullPointerException
				Dim t As Node(Of K, V)()
				t = map.table
				If t IsNot Nothing Then
					Dim it As New Traverser(Of K, V)(t, t.Length, 0, t.Length)
					Dim p As Node(Of K, V)
'JAVA TO VB CONVERTER TODO TASK: Assignments within expressions are not supported in VB
					Do While (p = it.advance()) IsNot Nothing
						action.accept(New MapEntry(Of K, V)(p.key, p.val, map))
					Loop
				End If
			End Sub

		End Class

		' -------------------------------------------------------

		''' <summary>
		''' Base class for bulk tasks. Repeats some fields and code from
		''' class Traverser, because we need to subclass CountedCompleter.
		''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
		Friend MustInherit Class BulkTask(Of K, V, R)
			Inherits CountedCompleter(Of R)

			Friend tab As Node(Of K, V)() ' same as Traverser
			Friend [next] As Node(Of K, V)
			Friend stack As TableStack(Of K, V), spare As TableStack(Of K, V)
			Friend index As Integer
			Friend baseIndex As Integer
			Friend baseLimit As Integer
			Friend ReadOnly baseSize As Integer
			Friend batch As Integer ' split control

			Friend Sub New(Of T1)(ByVal par As BulkTask(Of T1), ByVal b As Integer, ByVal i As Integer, ByVal f As Integer, ByVal t As Node(Of K, V)())
				MyBase.New(par)
				Me.batch = b
					Me.baseIndex = i
					Me.index = Me.baseIndex
				Me.tab = t
				If Me.tab Is Nothing Then
						Me.baseLimit = 0
						Me.baseSize = Me.baseLimit
				ElseIf par Is Nothing Then
						Me.baseLimit = t.Length
						Me.baseSize = Me.baseLimit
				Else
					Me.baseLimit = f
					Me.baseSize = par.baseSize
				End If
			End Sub

			''' <summary>
			''' Same as Traverser version
			''' </summary>
			Friend Function advance() As Node(Of K, V)
				Dim e As Node(Of K, V)
				e = [next]
				If e IsNot Nothing Then e = e.next
				Do
					Dim t As Node(Of K, V)()
					Dim i, n As Integer
					If e IsNot Nothing Then
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
							= e
							Return [next]
					End If
					t = tab
					n = t.Length
					i = index
					If baseIndex >= baseLimit OrElse t Is Nothing OrElse n <= i OrElse i < 0 Then
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
							= Nothing
							Return [next]
					End If
					e = tabAt(t, i)
					If e IsNot Nothing AndAlso e.hash < 0 Then
						If TypeOf e Is ForwardingNode Then
							tab = CType(e, ForwardingNode(Of K, V)).nextTable
							e = Nothing
							pushState(t, i, n)
							Continue Do
						ElseIf TypeOf e Is TreeBin Then
							e = CType(e, TreeBin(Of K, V)).first
						Else
							e = Nothing
						End If
					End If
					If stack IsNot Nothing Then
						recoverState(n)
					Else
						index = i + baseSize
						If index >= n Then
							baseIndex += 1
							index = baseIndex
						End If
						End If
				Loop
			End Function

			Private Sub pushState(ByVal t As Node(Of K, V)(), ByVal i As Integer, ByVal n As Integer)
				Dim s As TableStack(Of K, V) = spare
				If s IsNot Nothing Then
					spare = s.next
				Else
					s = New TableStack(Of K, V)
				End If
				s.tab = t
				s.length = n
				s.index = i
				s.next = stack
				stack = s
			End Sub

			Private Sub recoverState(ByVal n As Integer)
				Dim s As TableStack(Of K, V)
				Dim len As Integer
				s = stack
'JAVA TO VB CONVERTER TODO TASK: Assignments within expressions are not supported in VB
				index += (len = s.length)
				Do While s IsNot Nothing AndAlso index >= n
					n = len
					index = s.index
					tab = s.tab
					s.tab = Nothing
					Dim [next] As TableStack(Of K, V) = s.next
					s.next = spare ' save for reuse
					stack = [next]
					spare = s
					s = stack
					index += (len = s.length)
				Loop
				index += baseSize
				If s Is Nothing AndAlso index >= n Then
					baseIndex += 1
					index = baseIndex
				End If
			End Sub
		End Class

	'    
	'     * Task classes. Coded in a regular but ugly format/style to
	'     * simplify checks that each variant differs in the right way from
	'     * others. The null screenings exist because compilers cannot tell
	'     * that we've already null-checked task arguments, so we force
	'     * simplest hoisted bypass to help avoid convoluted traps.
	'     
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
		Friend NotInheritable Class ForEachKeyTask(Of K, V)
			Inherits BulkTask(Of K, V, Void)

'JAVA TO VB CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
			Friend ReadOnly action As java.util.function.Consumer(Of ?)
'JAVA TO VB CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
			Friend Sub New(Of T1, T2)(ByVal p As BulkTask(Of T1), ByVal b As Integer, ByVal i As Integer, ByVal f As Integer, ByVal t As Node(Of K, V)(), ByVal action As java.util.function.Consumer(Of T2))
				MyBase.New(p, b, i, f, t)
				Me.action = action
			End Sub
			Public Sub compute()
'JAVA TO VB CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
				Dim action As java.util.function.Consumer(Of ?)
				action = Me.action
				If action IsNot Nothing Then
'JAVA TO VB CONVERTER TODO TASK: The following line could not be converted:
					for (int i = baseIndex, f, h; batch > 0 && (h = ((f = baseLimit) + i) >>> 1) > i;)
						addToPendingCount(1)
'JAVA TO VB CONVERTER TODO TASK: Assignments within expressions are not supported in VB
						CType(New ForEachKeyTask(Of K, V) (Me, batch >>>= 1, baseLimit = h, f, tab, action), ForEachKeyTask(Of K, V)).fork()
					Dim p As Node(Of K, V)
'JAVA TO VB CONVERTER TODO TASK: Assignments within expressions are not supported in VB
					Do While (p = advance()) IsNot Nothing
						action.accept(p.key)
					Loop
					propagateCompletion()
				End If
			End Sub
		End Class

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
		Friend NotInheritable Class ForEachValueTask(Of K, V)
			Inherits BulkTask(Of K, V, Void)

'JAVA TO VB CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
			Friend ReadOnly action As java.util.function.Consumer(Of ?)
'JAVA TO VB CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
			Friend Sub New(Of T1, T2)(ByVal p As BulkTask(Of T1), ByVal b As Integer, ByVal i As Integer, ByVal f As Integer, ByVal t As Node(Of K, V)(), ByVal action As java.util.function.Consumer(Of T2))
				MyBase.New(p, b, i, f, t)
				Me.action = action
			End Sub
			Public Sub compute()
'JAVA TO VB CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
				Dim action As java.util.function.Consumer(Of ?)
				action = Me.action
				If action IsNot Nothing Then
'JAVA TO VB CONVERTER TODO TASK: The following line could not be converted:
					for (int i = baseIndex, f, h; batch > 0 && (h = ((f = baseLimit) + i) >>> 1) > i;)
						addToPendingCount(1)
'JAVA TO VB CONVERTER TODO TASK: Assignments within expressions are not supported in VB
						CType(New ForEachValueTask(Of K, V) (Me, batch >>>= 1, baseLimit = h, f, tab, action), ForEachValueTask(Of K, V)).fork()
					Dim p As Node(Of K, V)
'JAVA TO VB CONVERTER TODO TASK: Assignments within expressions are not supported in VB
					Do While (p = advance()) IsNot Nothing
						action.accept(p.val)
					Loop
					propagateCompletion()
				End If
			End Sub
		End Class

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
		Friend NotInheritable Class ForEachEntryTask(Of K, V)
			Inherits BulkTask(Of K, V, Void)

'JAVA TO VB CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
			Friend ReadOnly action As java.util.function.Consumer(Of ?)
'JAVA TO VB CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
			Friend Sub New(Of T1, T2)(ByVal p As BulkTask(Of T1), ByVal b As Integer, ByVal i As Integer, ByVal f As Integer, ByVal t As Node(Of K, V)(), ByVal action As java.util.function.Consumer(Of T2))
				MyBase.New(p, b, i, f, t)
				Me.action = action
			End Sub
			Public Sub compute()
'JAVA TO VB CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
				Dim action As java.util.function.Consumer(Of ?)
				action = Me.action
				If action IsNot Nothing Then
'JAVA TO VB CONVERTER TODO TASK: The following line could not be converted:
					for (int i = baseIndex, f, h; batch > 0 && (h = ((f = baseLimit) + i) >>> 1) > i;)
						addToPendingCount(1)
'JAVA TO VB CONVERTER TODO TASK: Assignments within expressions are not supported in VB
						CType(New ForEachEntryTask(Of K, V) (Me, batch >>>= 1, baseLimit = h, f, tab, action), ForEachEntryTask(Of K, V)).fork()
					Dim p As Node(Of K, V)
'JAVA TO VB CONVERTER TODO TASK: Assignments within expressions are not supported in VB
					Do While (p = advance()) IsNot Nothing
						action.accept(p)
					Loop
					propagateCompletion()
				End If
			End Sub
		End Class

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
		Friend NotInheritable Class ForEachMappingTask(Of K, V)
			Inherits BulkTask(Of K, V, Void)

'JAVA TO VB CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
			Friend ReadOnly action As java.util.function.BiConsumer(Of ?, ?)
'JAVA TO VB CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
			Friend Sub New(Of T1, T2)(ByVal p As BulkTask(Of T1), ByVal b As Integer, ByVal i As Integer, ByVal f As Integer, ByVal t As Node(Of K, V)(), ByVal action As java.util.function.BiConsumer(Of T2))
				MyBase.New(p, b, i, f, t)
				Me.action = action
			End Sub
			Public Sub compute()
'JAVA TO VB CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
				Dim action As java.util.function.BiConsumer(Of ?, ?)
				action = Me.action
				If action IsNot Nothing Then
'JAVA TO VB CONVERTER TODO TASK: The following line could not be converted:
					for (int i = baseIndex, f, h; batch > 0 && (h = ((f = baseLimit) + i) >>> 1) > i;)
						addToPendingCount(1)
'JAVA TO VB CONVERTER TODO TASK: Assignments within expressions are not supported in VB
						CType(New ForEachMappingTask(Of K, V) (Me, batch >>>= 1, baseLimit = h, f, tab, action), ForEachMappingTask(Of K, V)).fork()
					Dim p As Node(Of K, V)
'JAVA TO VB CONVERTER TODO TASK: Assignments within expressions are not supported in VB
					Do While (p = advance()) IsNot Nothing
						action.accept(p.key, p.val)
					Loop
					propagateCompletion()
				End If
			End Sub
		End Class

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
		Friend NotInheritable Class ForEachTransformedKeyTask(Of K, V, U)
			Inherits BulkTask(Of K, V, Void)

'JAVA TO VB CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
			Friend ReadOnly transformer As java.util.function.Function(Of ?, ? As U)
'JAVA TO VB CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
			Friend ReadOnly action As java.util.function.Consumer(Of ?)
'JAVA TO VB CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
			Friend Sub New(Of T1, T2 As U, T3)(ByVal p As BulkTask(Of T1), ByVal b As Integer, ByVal i As Integer, ByVal f As Integer, ByVal t As Node(Of K, V)(), ByVal transformer As java.util.function.Function(Of T2), ByVal action As java.util.function.Consumer(Of T3))
				MyBase.New(p, b, i, f, t)
				Me.transformer = transformer
				Me.action = action
			End Sub
			Public Sub compute()
'JAVA TO VB CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
				Dim transformer As java.util.function.Function(Of ?, ? As U)
'JAVA TO VB CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
				Dim action As java.util.function.Consumer(Of ?)
				transformer = Me.transformer
				action = Me.action
				If transformer IsNot Nothing AndAlso action IsNot Nothing Then
'JAVA TO VB CONVERTER TODO TASK: The following line could not be converted:
					for (int i = baseIndex, f, h; batch > 0 && (h = ((f = baseLimit) + i) >>> 1) > i;)
						addToPendingCount(1)
'JAVA TO VB CONVERTER TODO TASK: Assignments within expressions are not supported in VB
						CType(New ForEachTransformedKeyTask(Of K, V, U) (Me, batch >>>= 1, baseLimit = h, f, tab, transformer, action), ForEachTransformedKeyTask(Of K, V, U)).fork()
					Dim p As Node(Of K, V)
'JAVA TO VB CONVERTER TODO TASK: Assignments within expressions are not supported in VB
					Do While (p = advance()) IsNot Nothing
						Dim u As U
						u = transformer.apply(p.key)
						If u IsNot Nothing Then action.accept(u)
					Loop
					propagateCompletion()
				End If
			End Sub
		End Class

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
		Friend NotInheritable Class ForEachTransformedValueTask(Of K, V, U)
			Inherits BulkTask(Of K, V, Void)

'JAVA TO VB CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
			Friend ReadOnly transformer As java.util.function.Function(Of ?, ? As U)
'JAVA TO VB CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
			Friend ReadOnly action As java.util.function.Consumer(Of ?)
'JAVA TO VB CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
			Friend Sub New(Of T1, T2 As U, T3)(ByVal p As BulkTask(Of T1), ByVal b As Integer, ByVal i As Integer, ByVal f As Integer, ByVal t As Node(Of K, V)(), ByVal transformer As java.util.function.Function(Of T2), ByVal action As java.util.function.Consumer(Of T3))
				MyBase.New(p, b, i, f, t)
				Me.transformer = transformer
				Me.action = action
			End Sub
			Public Sub compute()
'JAVA TO VB CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
				Dim transformer As java.util.function.Function(Of ?, ? As U)
'JAVA TO VB CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
				Dim action As java.util.function.Consumer(Of ?)
				transformer = Me.transformer
				action = Me.action
				If transformer IsNot Nothing AndAlso action IsNot Nothing Then
'JAVA TO VB CONVERTER TODO TASK: The following line could not be converted:
					for (int i = baseIndex, f, h; batch > 0 && (h = ((f = baseLimit) + i) >>> 1) > i;)
						addToPendingCount(1)
'JAVA TO VB CONVERTER TODO TASK: Assignments within expressions are not supported in VB
						CType(New ForEachTransformedValueTask(Of K, V, U) (Me, batch >>>= 1, baseLimit = h, f, tab, transformer, action), ForEachTransformedValueTask(Of K, V, U)).fork()
					Dim p As Node(Of K, V)
'JAVA TO VB CONVERTER TODO TASK: Assignments within expressions are not supported in VB
					Do While (p = advance()) IsNot Nothing
						Dim u As U
						u = transformer.apply(p.val)
						If u IsNot Nothing Then action.accept(u)
					Loop
					propagateCompletion()
				End If
			End Sub
		End Class

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
		Friend NotInheritable Class ForEachTransformedEntryTask(Of K, V, U)
			Inherits BulkTask(Of K, V, Void)

'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
			Friend ReadOnly transformer As java.util.function.Function(Of KeyValuePair(Of K, V), ? As U)
'JAVA TO VB CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
			Friend ReadOnly action As java.util.function.Consumer(Of ?)
'JAVA TO VB CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
			Friend Sub New(Of T1, T2 As U, T3)(ByVal p As BulkTask(Of T1), ByVal b As Integer, ByVal i As Integer, ByVal f As Integer, ByVal t As Node(Of K, V)(), ByVal transformer As java.util.function.Function(Of T2), ByVal action As java.util.function.Consumer(Of T3))
				MyBase.New(p, b, i, f, t)
				Me.transformer = transformer
				Me.action = action
			End Sub
			Public Sub compute()
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
				Dim transformer As java.util.function.Function(Of KeyValuePair(Of K, V), ? As U)
'JAVA TO VB CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
				Dim action As java.util.function.Consumer(Of ?)
				transformer = Me.transformer
				action = Me.action
				If transformer IsNot Nothing AndAlso action IsNot Nothing Then
'JAVA TO VB CONVERTER TODO TASK: The following line could not be converted:
					for (int i = baseIndex, f, h; batch > 0 && (h = ((f = baseLimit) + i) >>> 1) > i;)
						addToPendingCount(1)
'JAVA TO VB CONVERTER TODO TASK: Assignments within expressions are not supported in VB
						CType(New ForEachTransformedEntryTask(Of K, V, U) (Me, batch >>>= 1, baseLimit = h, f, tab, transformer, action), ForEachTransformedEntryTask(Of K, V, U)).fork()
					Dim p As Node(Of K, V)
'JAVA TO VB CONVERTER TODO TASK: Assignments within expressions are not supported in VB
					Do While (p = advance()) IsNot Nothing
						Dim u As U
						u = transformer.apply(p)
						If u IsNot Nothing Then action.accept(u)
					Loop
					propagateCompletion()
				End If
			End Sub
		End Class

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
		Friend NotInheritable Class ForEachTransformedMappingTask(Of K, V, U)
			Inherits BulkTask(Of K, V, Void)

'JAVA TO VB CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
			Friend ReadOnly transformer As java.util.function.BiFunction(Of ?, ?, ? As U)
'JAVA TO VB CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
			Friend ReadOnly action As java.util.function.Consumer(Of ?)
'JAVA TO VB CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
			Friend Sub New(Of T1, T2 As U, T3)(ByVal p As BulkTask(Of T1), ByVal b As Integer, ByVal i As Integer, ByVal f As Integer, ByVal t As Node(Of K, V)(), ByVal transformer As java.util.function.BiFunction(Of T2), ByVal action As java.util.function.Consumer(Of T3))
				MyBase.New(p, b, i, f, t)
				Me.transformer = transformer
				Me.action = action
			End Sub
			Public Sub compute()
'JAVA TO VB CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
				Dim transformer As java.util.function.BiFunction(Of ?, ?, ? As U)
'JAVA TO VB CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
				Dim action As java.util.function.Consumer(Of ?)
				transformer = Me.transformer
				action = Me.action
				If transformer IsNot Nothing AndAlso action IsNot Nothing Then
'JAVA TO VB CONVERTER TODO TASK: The following line could not be converted:
					for (int i = baseIndex, f, h; batch > 0 && (h = ((f = baseLimit) + i) >>> 1) > i;)
						addToPendingCount(1)
'JAVA TO VB CONVERTER TODO TASK: Assignments within expressions are not supported in VB
						CType(New ForEachTransformedMappingTask(Of K, V, U) (Me, batch >>>= 1, baseLimit = h, f, tab, transformer, action), ForEachTransformedMappingTask(Of K, V, U)).fork()
					Dim p As Node(Of K, V)
'JAVA TO VB CONVERTER TODO TASK: Assignments within expressions are not supported in VB
					Do While (p = advance()) IsNot Nothing
						Dim u As U
						u = transformer.apply(p.key, p.val)
						If u IsNot Nothing Then action.accept(u)
					Loop
					propagateCompletion()
				End If
			End Sub
		End Class

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
		Friend NotInheritable Class SearchKeysTask(Of K, V, U)
			Inherits BulkTask(Of K, V, U)

'JAVA TO VB CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
			Friend ReadOnly searchFunction As java.util.function.Function(Of ?, ? As U)
			Friend ReadOnly result As java.util.concurrent.atomic.AtomicReference(Of U)
'JAVA TO VB CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
			Friend Sub New(Of T1, T2 As U)(ByVal p As BulkTask(Of T1), ByVal b As Integer, ByVal i As Integer, ByVal f As Integer, ByVal t As Node(Of K, V)(), ByVal searchFunction As java.util.function.Function(Of T2), ByVal result As java.util.concurrent.atomic.AtomicReference(Of U))
				MyBase.New(p, b, i, f, t)
				Me.searchFunction = searchFunction
				Me.result = result
			End Sub
			Public Property rawResult As U
				Get
					Return result.get()
				End Get
			End Property
			Public Sub compute()
'JAVA TO VB CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
				Dim searchFunction As java.util.function.Function(Of ?, ? As U)
				Dim result As java.util.concurrent.atomic.AtomicReference(Of U)
				searchFunction = Me.searchFunction
				result = Me.result
				If searchFunction IsNot Nothing AndAlso result IsNot Nothing Then
'JAVA TO VB CONVERTER TODO TASK: The following line could not be converted:
					for (int i = baseIndex, f, h; batch > 0 && (h = ((f = baseLimit) + i) >>> 1) > i;)
						If result.get() IsNot Nothing Then Return
						addToPendingCount(1)
'JAVA TO VB CONVERTER TODO TASK: Assignments within expressions are not supported in VB
						CType(New SearchKeysTask(Of K, V, U) (Me, batch >>>= 1, baseLimit = h, f, tab, searchFunction, result), SearchKeysTask(Of K, V, U)).fork()
					Do While result.get() Is Nothing
						Dim u As U
						Dim p As Node(Of K, V)
						p = advance()
						If p Is Nothing Then
							propagateCompletion()
							Exit Do
						End If
						u = searchFunction.apply(p.key)
						If u IsNot Nothing Then
							If result.compareAndSet(Nothing, u) Then quietlyCompleteRoot()
							Exit Do
						End If
					Loop
				End If
			End Sub
		End Class

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
		Friend NotInheritable Class SearchValuesTask(Of K, V, U)
			Inherits BulkTask(Of K, V, U)

'JAVA TO VB CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
			Friend ReadOnly searchFunction As java.util.function.Function(Of ?, ? As U)
			Friend ReadOnly result As java.util.concurrent.atomic.AtomicReference(Of U)
'JAVA TO VB CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
			Friend Sub New(Of T1, T2 As U)(ByVal p As BulkTask(Of T1), ByVal b As Integer, ByVal i As Integer, ByVal f As Integer, ByVal t As Node(Of K, V)(), ByVal searchFunction As java.util.function.Function(Of T2), ByVal result As java.util.concurrent.atomic.AtomicReference(Of U))
				MyBase.New(p, b, i, f, t)
				Me.searchFunction = searchFunction
				Me.result = result
			End Sub
			Public Property rawResult As U
				Get
					Return result.get()
				End Get
			End Property
			Public Sub compute()
'JAVA TO VB CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
				Dim searchFunction As java.util.function.Function(Of ?, ? As U)
				Dim result As java.util.concurrent.atomic.AtomicReference(Of U)
				searchFunction = Me.searchFunction
				result = Me.result
				If searchFunction IsNot Nothing AndAlso result IsNot Nothing Then
'JAVA TO VB CONVERTER TODO TASK: The following line could not be converted:
					for (int i = baseIndex, f, h; batch > 0 && (h = ((f = baseLimit) + i) >>> 1) > i;)
						If result.get() IsNot Nothing Then Return
						addToPendingCount(1)
'JAVA TO VB CONVERTER TODO TASK: Assignments within expressions are not supported in VB
						CType(New SearchValuesTask(Of K, V, U) (Me, batch >>>= 1, baseLimit = h, f, tab, searchFunction, result), SearchValuesTask(Of K, V, U)).fork()
					Do While result.get() Is Nothing
						Dim u As U
						Dim p As Node(Of K, V)
						p = advance()
						If p Is Nothing Then
							propagateCompletion()
							Exit Do
						End If
						u = searchFunction.apply(p.val)
						If u IsNot Nothing Then
							If result.compareAndSet(Nothing, u) Then quietlyCompleteRoot()
							Exit Do
						End If
					Loop
				End If
			End Sub
		End Class

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
		Friend NotInheritable Class SearchEntriesTask(Of K, V, U)
			Inherits BulkTask(Of K, V, U)

'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
			Friend ReadOnly searchFunction As java.util.function.Function(Of Entry(Of K, V), ? As U)
			Friend ReadOnly result As java.util.concurrent.atomic.AtomicReference(Of U)
			Friend Sub New(Of T1, T2 As U)(ByVal p As BulkTask(Of T1), ByVal b As Integer, ByVal i As Integer, ByVal f As Integer, ByVal t As Node(Of K, V)(), ByVal searchFunction As java.util.function.Function(Of T2), ByVal result As java.util.concurrent.atomic.AtomicReference(Of U))
				MyBase.New(p, b, i, f, t)
				Me.searchFunction = searchFunction
				Me.result = result
			End Sub
			Public Property rawResult As U
				Get
					Return result.get()
				End Get
			End Property
			Public Sub compute()
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
				Dim searchFunction As java.util.function.Function(Of Entry(Of K, V), ? As U)
				Dim result As java.util.concurrent.atomic.AtomicReference(Of U)
				searchFunction = Me.searchFunction
				result = Me.result
				If searchFunction IsNot Nothing AndAlso result IsNot Nothing Then
'JAVA TO VB CONVERTER TODO TASK: The following line could not be converted:
					for (int i = baseIndex, f, h; batch > 0 && (h = ((f = baseLimit) + i) >>> 1) > i;)
						If result.get() IsNot Nothing Then Return
						addToPendingCount(1)
'JAVA TO VB CONVERTER TODO TASK: Assignments within expressions are not supported in VB
						CType(New SearchEntriesTask(Of K, V, U) (Me, batch >>>= 1, baseLimit = h, f, tab, searchFunction, result), SearchEntriesTask(Of K, V, U)).fork()
					Do While result.get() Is Nothing
						Dim u As U
						Dim p As Node(Of K, V)
						p = advance()
						If p Is Nothing Then
							propagateCompletion()
							Exit Do
						End If
						u = searchFunction.apply(p)
						If u IsNot Nothing Then
							If result.compareAndSet(Nothing, u) Then quietlyCompleteRoot()
							Return
						End If
					Loop
				End If
			End Sub
		End Class

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
		Friend NotInheritable Class SearchMappingsTask(Of K, V, U)
			Inherits BulkTask(Of K, V, U)

'JAVA TO VB CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
			Friend ReadOnly searchFunction As java.util.function.BiFunction(Of ?, ?, ? As U)
			Friend ReadOnly result As java.util.concurrent.atomic.AtomicReference(Of U)
'JAVA TO VB CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
			Friend Sub New(Of T1, T2 As U)(ByVal p As BulkTask(Of T1), ByVal b As Integer, ByVal i As Integer, ByVal f As Integer, ByVal t As Node(Of K, V)(), ByVal searchFunction As java.util.function.BiFunction(Of T2), ByVal result As java.util.concurrent.atomic.AtomicReference(Of U))
				MyBase.New(p, b, i, f, t)
				Me.searchFunction = searchFunction
				Me.result = result
			End Sub
			Public Property rawResult As U
				Get
					Return result.get()
				End Get
			End Property
			Public Sub compute()
'JAVA TO VB CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
				Dim searchFunction As java.util.function.BiFunction(Of ?, ?, ? As U)
				Dim result As java.util.concurrent.atomic.AtomicReference(Of U)
				searchFunction = Me.searchFunction
				result = Me.result
				If searchFunction IsNot Nothing AndAlso result IsNot Nothing Then
'JAVA TO VB CONVERTER TODO TASK: The following line could not be converted:
					for (int i = baseIndex, f, h; batch > 0 && (h = ((f = baseLimit) + i) >>> 1) > i;)
						If result.get() IsNot Nothing Then Return
						addToPendingCount(1)
'JAVA TO VB CONVERTER TODO TASK: Assignments within expressions are not supported in VB
						CType(New SearchMappingsTask(Of K, V, U) (Me, batch >>>= 1, baseLimit = h, f, tab, searchFunction, result), SearchMappingsTask(Of K, V, U)).fork()
					Do While result.get() Is Nothing
						Dim u As U
						Dim p As Node(Of K, V)
						p = advance()
						If p Is Nothing Then
							propagateCompletion()
							Exit Do
						End If
						u = searchFunction.apply(p.key, p.val)
						If u IsNot Nothing Then
							If result.compareAndSet(Nothing, u) Then quietlyCompleteRoot()
							Exit Do
						End If
					Loop
				End If
			End Sub
		End Class

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
		Friend NotInheritable Class ReduceKeysTask(Of K, V)
			Inherits BulkTask(Of K, V, K)

'JAVA TO VB CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
			Friend ReadOnly reducer As java.util.function.BiFunction(Of ?, ?, ? As K)
			Friend result As K
			Friend rights As ReduceKeysTask(Of K, V), nextRight As ReduceKeysTask(Of K, V)
'JAVA TO VB CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
			Friend Sub New(Of T1, T2 As K)(ByVal p As BulkTask(Of T1), ByVal b As Integer, ByVal i As Integer, ByVal f As Integer, ByVal t As Node(Of K, V)(), ByVal nextRight As ReduceKeysTask(Of K, V), ByVal reducer As java.util.function.BiFunction(Of T2))
				MyBase.New(p, b, i, f, t)
				Me.nextRight = nextRight
				Me.reducer = reducer
			End Sub
			Public Property rawResult As K
				Get
					Return result
				End Get
			End Property
			Public Sub compute()
'JAVA TO VB CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
				Dim reducer As java.util.function.BiFunction(Of ?, ?, ? As K)
				reducer = Me.reducer
				If reducer IsNot Nothing Then
'JAVA TO VB CONVERTER TODO TASK: The following line could not be converted:
					for (int i = baseIndex, f, h; batch > 0 && (h = ((f = baseLimit) + i) >>> 1) > i;)
						addToPendingCount(1)
'JAVA TO VB CONVERTER TODO TASK: Assignments within expressions are not supported in VB
						(rights = New ReduceKeysTask(Of K, V) (Me, batch >>>= 1, baseLimit = h, f, tab, rights, reducer)).fork()
					Dim r As K = Nothing
					Dim p As Node(Of K, V)
'JAVA TO VB CONVERTER TODO TASK: Assignments within expressions are not supported in VB
					Do While (p = advance()) IsNot Nothing
						Dim u As K = p.key
						r = If(r Is Nothing, u, If(u Is Nothing, r, reducer.apply(r, u)))
					Loop
					result = r
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
					Dim c As CountedCompleter(Of ?)
					c = firstComplete()
					Do While c IsNot Nothing
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
						Dim t As ReduceKeysTask(Of K, V) = CType(c, ReduceKeysTask(Of K, V)), s As ReduceKeysTask(Of K, V) = t.rights
						Do While s IsNot Nothing
							Dim tr, sr As K
							sr = s.result
							If sr IsNot Nothing Then t.result = (If((tr = t.result) Is Nothing, sr, reducer.apply(tr, sr)))
								t.rights = s.nextRight
								s = t.rights
						Loop
						c = c.nextComplete()
					Loop
				End If
			End Sub
		End Class

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
		Friend NotInheritable Class ReduceValuesTask(Of K, V)
			Inherits BulkTask(Of K, V, V)

'JAVA TO VB CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
			Friend ReadOnly reducer As java.util.function.BiFunction(Of ?, ?, ? As V)
			Friend result As V
			Friend rights As ReduceValuesTask(Of K, V), nextRight As ReduceValuesTask(Of K, V)
'JAVA TO VB CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
			Friend Sub New(Of T1, T2 As V)(ByVal p As BulkTask(Of T1), ByVal b As Integer, ByVal i As Integer, ByVal f As Integer, ByVal t As Node(Of K, V)(), ByVal nextRight As ReduceValuesTask(Of K, V), ByVal reducer As java.util.function.BiFunction(Of T2))
				MyBase.New(p, b, i, f, t)
				Me.nextRight = nextRight
				Me.reducer = reducer
			End Sub
			Public Property rawResult As V
				Get
					Return result
				End Get
			End Property
			Public Sub compute()
'JAVA TO VB CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
				Dim reducer As java.util.function.BiFunction(Of ?, ?, ? As V)
				reducer = Me.reducer
				If reducer IsNot Nothing Then
'JAVA TO VB CONVERTER TODO TASK: The following line could not be converted:
					for (int i = baseIndex, f, h; batch > 0 && (h = ((f = baseLimit) + i) >>> 1) > i;)
						addToPendingCount(1)
'JAVA TO VB CONVERTER TODO TASK: Assignments within expressions are not supported in VB
						(rights = New ReduceValuesTask(Of K, V) (Me, batch >>>= 1, baseLimit = h, f, tab, rights, reducer)).fork()
					Dim r As V = Nothing
					Dim p As Node(Of K, V)
'JAVA TO VB CONVERTER TODO TASK: Assignments within expressions are not supported in VB
					Do While (p = advance()) IsNot Nothing
						Dim v As V = p.val
						r = If(r Is Nothing, v, reducer.apply(r, v))
					Loop
					result = r
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
					Dim c As CountedCompleter(Of ?)
					c = firstComplete()
					Do While c IsNot Nothing
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
						Dim t As ReduceValuesTask(Of K, V) = CType(c, ReduceValuesTask(Of K, V)), s As ReduceValuesTask(Of K, V) = t.rights
						Do While s IsNot Nothing
							Dim tr, sr As V
							sr = s.result
							If sr IsNot Nothing Then t.result = (If((tr = t.result) Is Nothing, sr, reducer.apply(tr, sr)))
								t.rights = s.nextRight
								s = t.rights
						Loop
						c = c.nextComplete()
					Loop
				End If
			End Sub
		End Class

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
		Friend NotInheritable Class ReduceEntriesTask(Of K, V)
			Inherits BulkTask(Of K, V, KeyValuePair(Of K, V))

'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
			Friend ReadOnly reducer As java.util.function.BiFunction(Of KeyValuePair(Of K, V), KeyValuePair(Of K, V), ? As KeyValuePair(Of K, V))
			Friend result As KeyValuePair(Of K, V)
			Friend rights As ReduceEntriesTask(Of K, V), nextRight As ReduceEntriesTask(Of K, V)
			Friend Sub New(Of T1, T2 As KeyValuePair(Of K, V)(ByVal p As BulkTask(Of T1), ByVal b As Integer, ByVal i As Integer, ByVal f As Integer, ByVal t As Node(Of K, V)(), ByVal nextRight As ReduceEntriesTask(Of K, V), ByVal reducer As java.util.function.BiFunction(Of T2))
				MyBase.New(p, b, i, f, t)
				Me.nextRight = nextRight
				Me.reducer = reducer
			End Sub
			Public Property rawResult As KeyValuePair(Of K, V)
				Get
					Return result
				End Get
			End Property
			Public Sub compute()
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
				Dim reducer As java.util.function.BiFunction(Of KeyValuePair(Of K, V), KeyValuePair(Of K, V), ? As KeyValuePair(Of K, V))
				reducer = Me.reducer
				If reducer IsNot Nothing Then
'JAVA TO VB CONVERTER TODO TASK: The following line could not be converted:
					for (int i = baseIndex, f, h; batch > 0 && (h = ((f = baseLimit) + i) >>> 1) > i;)
						addToPendingCount(1)
'JAVA TO VB CONVERTER TODO TASK: Assignments within expressions are not supported in VB
						(rights = New ReduceEntriesTask(Of K, V) (Me, batch >>>= 1, baseLimit = h, f, tab, rights, reducer)).fork()
					Dim r As KeyValuePair(Of K, V) = Nothing
					Dim p As Node(Of K, V)
'JAVA TO VB CONVERTER TODO TASK: Assignments within expressions are not supported in VB
					Do While (p = advance()) IsNot Nothing
						r = If(r Is Nothing, p, reducer.apply(r, p))
					Loop
					result = r
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
					Dim c As CountedCompleter(Of ?)
					c = firstComplete()
					Do While c IsNot Nothing
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
						Dim t As ReduceEntriesTask(Of K, V) = CType(c, ReduceEntriesTask(Of K, V)), s As ReduceEntriesTask(Of K, V) = t.rights
						Do While s IsNot Nothing
							Dim tr As KeyValuePair(Of K, V), sr As KeyValuePair(Of K, V)
							sr = s.result
							If sr IsNot Nothing Then t.result = (If((tr = t.result) Is Nothing, sr, reducer.apply(tr, sr)))
								t.rights = s.nextRight
								s = t.rights
						Loop
						c = c.nextComplete()
					Loop
				End If
			End Sub
		End Class

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
		Friend NotInheritable Class MapReduceKeysTask(Of K, V, U)
			Inherits BulkTask(Of K, V, U)

'JAVA TO VB CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
			Friend ReadOnly transformer As java.util.function.Function(Of ?, ? As U)
'JAVA TO VB CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
			Friend ReadOnly reducer As java.util.function.BiFunction(Of ?, ?, ? As U)
			Friend result As U
			Friend rights As MapReduceKeysTask(Of K, V, U), nextRight As MapReduceKeysTask(Of K, V, U)
'JAVA TO VB CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
			Friend Sub New(Of T1, T2 As U, T3 As U)(ByVal p As BulkTask(Of T1), ByVal b As Integer, ByVal i As Integer, ByVal f As Integer, ByVal t As Node(Of K, V)(), ByVal nextRight As MapReduceKeysTask(Of K, V, U), ByVal transformer As java.util.function.Function(Of T2), ByVal reducer As java.util.function.BiFunction(Of T3))
				MyBase.New(p, b, i, f, t)
				Me.nextRight = nextRight
				Me.transformer = transformer
				Me.reducer = reducer
			End Sub
			Public Property rawResult As U
				Get
					Return result
				End Get
			End Property
			Public Sub compute()
'JAVA TO VB CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
				Dim transformer As java.util.function.Function(Of ?, ? As U)
'JAVA TO VB CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
				Dim reducer As java.util.function.BiFunction(Of ?, ?, ? As U)
				transformer = Me.transformer
				reducer = Me.reducer
				If transformer IsNot Nothing AndAlso reducer IsNot Nothing Then
'JAVA TO VB CONVERTER TODO TASK: The following line could not be converted:
					for (int i = baseIndex, f, h; batch > 0 && (h = ((f = baseLimit) + i) >>> 1) > i;)
						addToPendingCount(1)
'JAVA TO VB CONVERTER TODO TASK: Assignments within expressions are not supported in VB
						(rights = New MapReduceKeysTask(Of K, V, U) (Me, batch >>>= 1, baseLimit = h, f, tab, rights, transformer, reducer)).fork()
					Dim r As U = Nothing
					Dim p As Node(Of K, V)
'JAVA TO VB CONVERTER TODO TASK: Assignments within expressions are not supported in VB
					Do While (p = advance()) IsNot Nothing
						Dim u As U
						u = transformer.apply(p.key)
						If u IsNot Nothing Then r = If(r Is Nothing, u, reducer.apply(r, u))
					Loop
					result = r
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
					Dim c As CountedCompleter(Of ?)
					c = firstComplete()
					Do While c IsNot Nothing
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
						Dim t As MapReduceKeysTask(Of K, V, U) = CType(c, MapReduceKeysTask(Of K, V, U)), s As MapReduceKeysTask(Of K, V, U) = t.rights
						Do While s IsNot Nothing
							Dim tr, sr As U
							sr = s.result
							If sr IsNot Nothing Then t.result = (If((tr = t.result) Is Nothing, sr, reducer.apply(tr, sr)))
								t.rights = s.nextRight
								s = t.rights
						Loop
						c = c.nextComplete()
					Loop
				End If
			End Sub
		End Class

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
		Friend NotInheritable Class MapReduceValuesTask(Of K, V, U)
			Inherits BulkTask(Of K, V, U)

'JAVA TO VB CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
			Friend ReadOnly transformer As java.util.function.Function(Of ?, ? As U)
'JAVA TO VB CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
			Friend ReadOnly reducer As java.util.function.BiFunction(Of ?, ?, ? As U)
			Friend result As U
			Friend rights As MapReduceValuesTask(Of K, V, U), nextRight As MapReduceValuesTask(Of K, V, U)
'JAVA TO VB CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
			Friend Sub New(Of T1, T2 As U, T3 As U)(ByVal p As BulkTask(Of T1), ByVal b As Integer, ByVal i As Integer, ByVal f As Integer, ByVal t As Node(Of K, V)(), ByVal nextRight As MapReduceValuesTask(Of K, V, U), ByVal transformer As java.util.function.Function(Of T2), ByVal reducer As java.util.function.BiFunction(Of T3))
				MyBase.New(p, b, i, f, t)
				Me.nextRight = nextRight
				Me.transformer = transformer
				Me.reducer = reducer
			End Sub
			Public Property rawResult As U
				Get
					Return result
				End Get
			End Property
			Public Sub compute()
'JAVA TO VB CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
				Dim transformer As java.util.function.Function(Of ?, ? As U)
'JAVA TO VB CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
				Dim reducer As java.util.function.BiFunction(Of ?, ?, ? As U)
				transformer = Me.transformer
				reducer = Me.reducer
				If transformer IsNot Nothing AndAlso reducer IsNot Nothing Then
'JAVA TO VB CONVERTER TODO TASK: The following line could not be converted:
					for (int i = baseIndex, f, h; batch > 0 && (h = ((f = baseLimit) + i) >>> 1) > i;)
						addToPendingCount(1)
'JAVA TO VB CONVERTER TODO TASK: Assignments within expressions are not supported in VB
						(rights = New MapReduceValuesTask(Of K, V, U) (Me, batch >>>= 1, baseLimit = h, f, tab, rights, transformer, reducer)).fork()
					Dim r As U = Nothing
					Dim p As Node(Of K, V)
'JAVA TO VB CONVERTER TODO TASK: Assignments within expressions are not supported in VB
					Do While (p = advance()) IsNot Nothing
						Dim u As U
						u = transformer.apply(p.val)
						If u IsNot Nothing Then r = If(r Is Nothing, u, reducer.apply(r, u))
					Loop
					result = r
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
					Dim c As CountedCompleter(Of ?)
					c = firstComplete()
					Do While c IsNot Nothing
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
						Dim t As MapReduceValuesTask(Of K, V, U) = CType(c, MapReduceValuesTask(Of K, V, U)), s As MapReduceValuesTask(Of K, V, U) = t.rights
						Do While s IsNot Nothing
							Dim tr, sr As U
							sr = s.result
							If sr IsNot Nothing Then t.result = (If((tr = t.result) Is Nothing, sr, reducer.apply(tr, sr)))
								t.rights = s.nextRight
								s = t.rights
						Loop
						c = c.nextComplete()
					Loop
				End If
			End Sub
		End Class

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
		Friend NotInheritable Class MapReduceEntriesTask(Of K, V, U)
			Inherits BulkTask(Of K, V, U)

'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
			Friend ReadOnly transformer As java.util.function.Function(Of KeyValuePair(Of K, V), ? As U)
'JAVA TO VB CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
			Friend ReadOnly reducer As java.util.function.BiFunction(Of ?, ?, ? As U)
			Friend result As U
			Friend rights As MapReduceEntriesTask(Of K, V, U), nextRight As MapReduceEntriesTask(Of K, V, U)
'JAVA TO VB CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
			Friend Sub New(Of T1, T2 As U, T3 As U)(ByVal p As BulkTask(Of T1), ByVal b As Integer, ByVal i As Integer, ByVal f As Integer, ByVal t As Node(Of K, V)(), ByVal nextRight As MapReduceEntriesTask(Of K, V, U), ByVal transformer As java.util.function.Function(Of T2), ByVal reducer As java.util.function.BiFunction(Of T3))
				MyBase.New(p, b, i, f, t)
				Me.nextRight = nextRight
				Me.transformer = transformer
				Me.reducer = reducer
			End Sub
			Public Property rawResult As U
				Get
					Return result
				End Get
			End Property
			Public Sub compute()
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
				Dim transformer As java.util.function.Function(Of KeyValuePair(Of K, V), ? As U)
'JAVA TO VB CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
				Dim reducer As java.util.function.BiFunction(Of ?, ?, ? As U)
				transformer = Me.transformer
				reducer = Me.reducer
				If transformer IsNot Nothing AndAlso reducer IsNot Nothing Then
'JAVA TO VB CONVERTER TODO TASK: The following line could not be converted:
					for (int i = baseIndex, f, h; batch > 0 && (h = ((f = baseLimit) + i) >>> 1) > i;)
						addToPendingCount(1)
'JAVA TO VB CONVERTER TODO TASK: Assignments within expressions are not supported in VB
						(rights = New MapReduceEntriesTask(Of K, V, U) (Me, batch >>>= 1, baseLimit = h, f, tab, rights, transformer, reducer)).fork()
					Dim r As U = Nothing
					Dim p As Node(Of K, V)
'JAVA TO VB CONVERTER TODO TASK: Assignments within expressions are not supported in VB
					Do While (p = advance()) IsNot Nothing
						Dim u As U
						u = transformer.apply(p)
						If u IsNot Nothing Then r = If(r Is Nothing, u, reducer.apply(r, u))
					Loop
					result = r
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
					Dim c As CountedCompleter(Of ?)
					c = firstComplete()
					Do While c IsNot Nothing
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
						Dim t As MapReduceEntriesTask(Of K, V, U) = CType(c, MapReduceEntriesTask(Of K, V, U)), s As MapReduceEntriesTask(Of K, V, U) = t.rights
						Do While s IsNot Nothing
							Dim tr, sr As U
							sr = s.result
							If sr IsNot Nothing Then t.result = (If((tr = t.result) Is Nothing, sr, reducer.apply(tr, sr)))
								t.rights = s.nextRight
								s = t.rights
						Loop
						c = c.nextComplete()
					Loop
				End If
			End Sub
		End Class

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
		Friend NotInheritable Class MapReduceMappingsTask(Of K, V, U)
			Inherits BulkTask(Of K, V, U)

'JAVA TO VB CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
			Friend ReadOnly transformer As java.util.function.BiFunction(Of ?, ?, ? As U)
'JAVA TO VB CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
			Friend ReadOnly reducer As java.util.function.BiFunction(Of ?, ?, ? As U)
			Friend result As U
			Friend rights As MapReduceMappingsTask(Of K, V, U), nextRight As MapReduceMappingsTask(Of K, V, U)
'JAVA TO VB CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
			Friend Sub New(Of T1, T2 As U, T3 As U)(ByVal p As BulkTask(Of T1), ByVal b As Integer, ByVal i As Integer, ByVal f As Integer, ByVal t As Node(Of K, V)(), ByVal nextRight As MapReduceMappingsTask(Of K, V, U), ByVal transformer As java.util.function.BiFunction(Of T2), ByVal reducer As java.util.function.BiFunction(Of T3))
				MyBase.New(p, b, i, f, t)
				Me.nextRight = nextRight
				Me.transformer = transformer
				Me.reducer = reducer
			End Sub
			Public Property rawResult As U
				Get
					Return result
				End Get
			End Property
			Public Sub compute()
'JAVA TO VB CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
				Dim transformer As java.util.function.BiFunction(Of ?, ?, ? As U)
'JAVA TO VB CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
				Dim reducer As java.util.function.BiFunction(Of ?, ?, ? As U)
				transformer = Me.transformer
				reducer = Me.reducer
				If transformer IsNot Nothing AndAlso reducer IsNot Nothing Then
'JAVA TO VB CONVERTER TODO TASK: The following line could not be converted:
					for (int i = baseIndex, f, h; batch > 0 && (h = ((f = baseLimit) + i) >>> 1) > i;)
						addToPendingCount(1)
'JAVA TO VB CONVERTER TODO TASK: Assignments within expressions are not supported in VB
						(rights = New MapReduceMappingsTask(Of K, V, U) (Me, batch >>>= 1, baseLimit = h, f, tab, rights, transformer, reducer)).fork()
					Dim r As U = Nothing
					Dim p As Node(Of K, V)
'JAVA TO VB CONVERTER TODO TASK: Assignments within expressions are not supported in VB
					Do While (p = advance()) IsNot Nothing
						Dim u As U
						u = transformer.apply(p.key, p.val)
						If u IsNot Nothing Then r = If(r Is Nothing, u, reducer.apply(r, u))
					Loop
					result = r
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
					Dim c As CountedCompleter(Of ?)
					c = firstComplete()
					Do While c IsNot Nothing
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
						Dim t As MapReduceMappingsTask(Of K, V, U) = CType(c, MapReduceMappingsTask(Of K, V, U)), s As MapReduceMappingsTask(Of K, V, U) = t.rights
						Do While s IsNot Nothing
							Dim tr, sr As U
							sr = s.result
							If sr IsNot Nothing Then t.result = (If((tr = t.result) Is Nothing, sr, reducer.apply(tr, sr)))
								t.rights = s.nextRight
								s = t.rights
						Loop
						c = c.nextComplete()
					Loop
				End If
			End Sub
		End Class

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
		Friend NotInheritable Class MapReduceKeysToDoubleTask(Of K, V)
			Inherits BulkTask(Of K, V, Double?)

'JAVA TO VB CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
			Friend ReadOnly transformer As java.util.function.ToDoubleFunction(Of ?)
			Friend ReadOnly reducer As java.util.function.DoubleBinaryOperator
			Friend ReadOnly basis As Double
			Friend result As Double
			Friend rights As MapReduceKeysToDoubleTask(Of K, V), nextRight As MapReduceKeysToDoubleTask(Of K, V)
'JAVA TO VB CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
			Friend Sub New(Of T1, T2)(ByVal p As BulkTask(Of T1), ByVal b As Integer, ByVal i As Integer, ByVal f As Integer, ByVal t As Node(Of K, V)(), ByVal nextRight As MapReduceKeysToDoubleTask(Of K, V), ByVal transformer As java.util.function.ToDoubleFunction(Of T2), ByVal basis As Double, ByVal reducer As java.util.function.DoubleBinaryOperator)
				MyBase.New(p, b, i, f, t)
				Me.nextRight = nextRight
				Me.transformer = transformer
				Me.basis = basis
				Me.reducer = reducer
			End Sub
			Public Property rawResult As Double?
				Get
					Return result
				End Get
			End Property
			Public Sub compute()
'JAVA TO VB CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
				Dim transformer As java.util.function.ToDoubleFunction(Of ?)
				Dim reducer As java.util.function.DoubleBinaryOperator
				transformer = Me.transformer
				reducer = Me.reducer
				If transformer IsNot Nothing AndAlso reducer IsNot Nothing Then
					Dim r As Double = Me.basis
'JAVA TO VB CONVERTER TODO TASK: The following line could not be converted:
					for (int i = baseIndex, f, h; batch > 0 && (h = ((f = baseLimit) + i) >>> 1) > i;)
						addToPendingCount(1)
'JAVA TO VB CONVERTER TODO TASK: Assignments within expressions are not supported in VB
						(rights = New MapReduceKeysToDoubleTask(Of K, V) (Me, batch >>>= 1, baseLimit = h, f, tab, rights, transformer, r, reducer)).fork()
					Dim p As Node(Of K, V)
'JAVA TO VB CONVERTER TODO TASK: Assignments within expressions are not supported in VB
					Do While (p = advance()) IsNot Nothing
						r = reducer.applyAsDouble(r, transformer.applyAsDouble(p.key))
					Loop
					result = r
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
					Dim c As CountedCompleter(Of ?)
					c = firstComplete()
					Do While c IsNot Nothing
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
						Dim t As MapReduceKeysToDoubleTask(Of K, V) = CType(c, MapReduceKeysToDoubleTask(Of K, V)), s As MapReduceKeysToDoubleTask(Of K, V) = t.rights
						Do While s IsNot Nothing
							t.result = reducer.applyAsDouble(t.result, s.result)
								t.rights = s.nextRight
								s = t.rights
						Loop
						c = c.nextComplete()
					Loop
				End If
			End Sub
		End Class

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
		Friend NotInheritable Class MapReduceValuesToDoubleTask(Of K, V)
			Inherits BulkTask(Of K, V, Double?)

'JAVA TO VB CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
			Friend ReadOnly transformer As java.util.function.ToDoubleFunction(Of ?)
			Friend ReadOnly reducer As java.util.function.DoubleBinaryOperator
			Friend ReadOnly basis As Double
			Friend result As Double
			Friend rights As MapReduceValuesToDoubleTask(Of K, V), nextRight As MapReduceValuesToDoubleTask(Of K, V)
'JAVA TO VB CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
			Friend Sub New(Of T1, T2)(ByVal p As BulkTask(Of T1), ByVal b As Integer, ByVal i As Integer, ByVal f As Integer, ByVal t As Node(Of K, V)(), ByVal nextRight As MapReduceValuesToDoubleTask(Of K, V), ByVal transformer As java.util.function.ToDoubleFunction(Of T2), ByVal basis As Double, ByVal reducer As java.util.function.DoubleBinaryOperator)
				MyBase.New(p, b, i, f, t)
				Me.nextRight = nextRight
				Me.transformer = transformer
				Me.basis = basis
				Me.reducer = reducer
			End Sub
			Public Property rawResult As Double?
				Get
					Return result
				End Get
			End Property
			Public Sub compute()
'JAVA TO VB CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
				Dim transformer As java.util.function.ToDoubleFunction(Of ?)
				Dim reducer As java.util.function.DoubleBinaryOperator
				transformer = Me.transformer
				reducer = Me.reducer
				If transformer IsNot Nothing AndAlso reducer IsNot Nothing Then
					Dim r As Double = Me.basis
'JAVA TO VB CONVERTER TODO TASK: The following line could not be converted:
					for (int i = baseIndex, f, h; batch > 0 && (h = ((f = baseLimit) + i) >>> 1) > i;)
						addToPendingCount(1)
'JAVA TO VB CONVERTER TODO TASK: Assignments within expressions are not supported in VB
						(rights = New MapReduceValuesToDoubleTask(Of K, V) (Me, batch >>>= 1, baseLimit = h, f, tab, rights, transformer, r, reducer)).fork()
					Dim p As Node(Of K, V)
'JAVA TO VB CONVERTER TODO TASK: Assignments within expressions are not supported in VB
					Do While (p = advance()) IsNot Nothing
						r = reducer.applyAsDouble(r, transformer.applyAsDouble(p.val))
					Loop
					result = r
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
					Dim c As CountedCompleter(Of ?)
					c = firstComplete()
					Do While c IsNot Nothing
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
						Dim t As MapReduceValuesToDoubleTask(Of K, V) = CType(c, MapReduceValuesToDoubleTask(Of K, V)), s As MapReduceValuesToDoubleTask(Of K, V) = t.rights
						Do While s IsNot Nothing
							t.result = reducer.applyAsDouble(t.result, s.result)
								t.rights = s.nextRight
								s = t.rights
						Loop
						c = c.nextComplete()
					Loop
				End If
			End Sub
		End Class

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
		Friend NotInheritable Class MapReduceEntriesToDoubleTask(Of K, V)
			Inherits BulkTask(Of K, V, Double?)

			Friend ReadOnly transformer As java.util.function.ToDoubleFunction(Of KeyValuePair(Of K, V))
			Friend ReadOnly reducer As java.util.function.DoubleBinaryOperator
			Friend ReadOnly basis As Double
			Friend result As Double
			Friend rights As MapReduceEntriesToDoubleTask(Of K, V), nextRight As MapReduceEntriesToDoubleTask(Of K, V)
			Friend Sub New(Of T1)(ByVal p As BulkTask(Of T1), ByVal b As Integer, ByVal i As Integer, ByVal f As Integer, ByVal t As Node(Of K, V)(), ByVal nextRight As MapReduceEntriesToDoubleTask(Of K, V), ByVal transformer As java.util.function.ToDoubleFunction(Of KeyValuePair(Of K, V)), ByVal basis As Double, ByVal reducer As java.util.function.DoubleBinaryOperator)
				MyBase.New(p, b, i, f, t)
				Me.nextRight = nextRight
				Me.transformer = transformer
				Me.basis = basis
				Me.reducer = reducer
			End Sub
			Public Property rawResult As Double?
				Get
					Return result
				End Get
			End Property
			Public Sub compute()
				Dim transformer As java.util.function.ToDoubleFunction(Of KeyValuePair(Of K, V))
				Dim reducer As java.util.function.DoubleBinaryOperator
				transformer = Me.transformer
				reducer = Me.reducer
				If transformer IsNot Nothing AndAlso reducer IsNot Nothing Then
					Dim r As Double = Me.basis
'JAVA TO VB CONVERTER TODO TASK: The following line could not be converted:
					for (int i = baseIndex, f, h; batch > 0 && (h = ((f = baseLimit) + i) >>> 1) > i;)
						addToPendingCount(1)
'JAVA TO VB CONVERTER TODO TASK: Assignments within expressions are not supported in VB
						(rights = New MapReduceEntriesToDoubleTask(Of K, V) (Me, batch >>>= 1, baseLimit = h, f, tab, rights, transformer, r, reducer)).fork()
					Dim p As Node(Of K, V)
'JAVA TO VB CONVERTER TODO TASK: Assignments within expressions are not supported in VB
					Do While (p = advance()) IsNot Nothing
						r = reducer.applyAsDouble(r, transformer.applyAsDouble(p))
					Loop
					result = r
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
					Dim c As CountedCompleter(Of ?)
					c = firstComplete()
					Do While c IsNot Nothing
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
						Dim t As MapReduceEntriesToDoubleTask(Of K, V) = CType(c, MapReduceEntriesToDoubleTask(Of K, V)), s As MapReduceEntriesToDoubleTask(Of K, V) = t.rights
						Do While s IsNot Nothing
							t.result = reducer.applyAsDouble(t.result, s.result)
								t.rights = s.nextRight
								s = t.rights
						Loop
						c = c.nextComplete()
					Loop
				End If
			End Sub
		End Class

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
		Friend NotInheritable Class MapReduceMappingsToDoubleTask(Of K, V)
			Inherits BulkTask(Of K, V, Double?)

'JAVA TO VB CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
			Friend ReadOnly transformer As java.util.function.ToDoubleBiFunction(Of ?, ?)
			Friend ReadOnly reducer As java.util.function.DoubleBinaryOperator
			Friend ReadOnly basis As Double
			Friend result As Double
			Friend rights As MapReduceMappingsToDoubleTask(Of K, V), nextRight As MapReduceMappingsToDoubleTask(Of K, V)
'JAVA TO VB CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
			Friend Sub New(Of T1, T2)(ByVal p As BulkTask(Of T1), ByVal b As Integer, ByVal i As Integer, ByVal f As Integer, ByVal t As Node(Of K, V)(), ByVal nextRight As MapReduceMappingsToDoubleTask(Of K, V), ByVal transformer As java.util.function.ToDoubleBiFunction(Of T2), ByVal basis As Double, ByVal reducer As java.util.function.DoubleBinaryOperator)
				MyBase.New(p, b, i, f, t)
				Me.nextRight = nextRight
				Me.transformer = transformer
				Me.basis = basis
				Me.reducer = reducer
			End Sub
			Public Property rawResult As Double?
				Get
					Return result
				End Get
			End Property
			Public Sub compute()
'JAVA TO VB CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
				Dim transformer As java.util.function.ToDoubleBiFunction(Of ?, ?)
				Dim reducer As java.util.function.DoubleBinaryOperator
				transformer = Me.transformer
				reducer = Me.reducer
				If transformer IsNot Nothing AndAlso reducer IsNot Nothing Then
					Dim r As Double = Me.basis
'JAVA TO VB CONVERTER TODO TASK: The following line could not be converted:
					for (int i = baseIndex, f, h; batch > 0 && (h = ((f = baseLimit) + i) >>> 1) > i;)
						addToPendingCount(1)
'JAVA TO VB CONVERTER TODO TASK: Assignments within expressions are not supported in VB
						(rights = New MapReduceMappingsToDoubleTask(Of K, V) (Me, batch >>>= 1, baseLimit = h, f, tab, rights, transformer, r, reducer)).fork()
					Dim p As Node(Of K, V)
'JAVA TO VB CONVERTER TODO TASK: Assignments within expressions are not supported in VB
					Do While (p = advance()) IsNot Nothing
						r = reducer.applyAsDouble(r, transformer.applyAsDouble(p.key, p.val))
					Loop
					result = r
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
					Dim c As CountedCompleter(Of ?)
					c = firstComplete()
					Do While c IsNot Nothing
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
						Dim t As MapReduceMappingsToDoubleTask(Of K, V) = CType(c, MapReduceMappingsToDoubleTask(Of K, V)), s As MapReduceMappingsToDoubleTask(Of K, V) = t.rights
						Do While s IsNot Nothing
							t.result = reducer.applyAsDouble(t.result, s.result)
								t.rights = s.nextRight
								s = t.rights
						Loop
						c = c.nextComplete()
					Loop
				End If
			End Sub
		End Class

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
		Friend NotInheritable Class MapReduceKeysToLongTask(Of K, V)
			Inherits BulkTask(Of K, V, Long?)

'JAVA TO VB CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
			Friend ReadOnly transformer As java.util.function.ToLongFunction(Of ?)
			Friend ReadOnly reducer As java.util.function.LongBinaryOperator
			Friend ReadOnly basis As Long
			Friend result As Long
			Friend rights As MapReduceKeysToLongTask(Of K, V), nextRight As MapReduceKeysToLongTask(Of K, V)
'JAVA TO VB CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
			Friend Sub New(Of T1, T2)(ByVal p As BulkTask(Of T1), ByVal b As Integer, ByVal i As Integer, ByVal f As Integer, ByVal t As Node(Of K, V)(), ByVal nextRight As MapReduceKeysToLongTask(Of K, V), ByVal transformer As java.util.function.ToLongFunction(Of T2), ByVal basis As Long, ByVal reducer As java.util.function.LongBinaryOperator)
				MyBase.New(p, b, i, f, t)
				Me.nextRight = nextRight
				Me.transformer = transformer
				Me.basis = basis
				Me.reducer = reducer
			End Sub
			Public Property rawResult As Long?
				Get
					Return result
				End Get
			End Property
			Public Sub compute()
'JAVA TO VB CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
				Dim transformer As java.util.function.ToLongFunction(Of ?)
				Dim reducer As java.util.function.LongBinaryOperator
				transformer = Me.transformer
				reducer = Me.reducer
				If transformer IsNot Nothing AndAlso reducer IsNot Nothing Then
					Dim r As Long = Me.basis
'JAVA TO VB CONVERTER TODO TASK: The following line could not be converted:
					for (int i = baseIndex, f, h; batch > 0 && (h = ((f = baseLimit) + i) >>> 1) > i;)
						addToPendingCount(1)
'JAVA TO VB CONVERTER TODO TASK: Assignments within expressions are not supported in VB
						(rights = New MapReduceKeysToLongTask(Of K, V) (Me, batch >>>= 1, baseLimit = h, f, tab, rights, transformer, r, reducer)).fork()
					Dim p As Node(Of K, V)
'JAVA TO VB CONVERTER TODO TASK: Assignments within expressions are not supported in VB
					Do While (p = advance()) IsNot Nothing
						r = reducer.applyAsLong(r, transformer.applyAsLong(p.key))
					Loop
					result = r
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
					Dim c As CountedCompleter(Of ?)
					c = firstComplete()
					Do While c IsNot Nothing
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
						Dim t As MapReduceKeysToLongTask(Of K, V) = CType(c, MapReduceKeysToLongTask(Of K, V)), s As MapReduceKeysToLongTask(Of K, V) = t.rights
						Do While s IsNot Nothing
							t.result = reducer.applyAsLong(t.result, s.result)
								t.rights = s.nextRight
								s = t.rights
						Loop
						c = c.nextComplete()
					Loop
				End If
			End Sub
		End Class

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
		Friend NotInheritable Class MapReduceValuesToLongTask(Of K, V)
			Inherits BulkTask(Of K, V, Long?)

'JAVA TO VB CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
			Friend ReadOnly transformer As java.util.function.ToLongFunction(Of ?)
			Friend ReadOnly reducer As java.util.function.LongBinaryOperator
			Friend ReadOnly basis As Long
			Friend result As Long
			Friend rights As MapReduceValuesToLongTask(Of K, V), nextRight As MapReduceValuesToLongTask(Of K, V)
'JAVA TO VB CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
			Friend Sub New(Of T1, T2)(ByVal p As BulkTask(Of T1), ByVal b As Integer, ByVal i As Integer, ByVal f As Integer, ByVal t As Node(Of K, V)(), ByVal nextRight As MapReduceValuesToLongTask(Of K, V), ByVal transformer As java.util.function.ToLongFunction(Of T2), ByVal basis As Long, ByVal reducer As java.util.function.LongBinaryOperator)
				MyBase.New(p, b, i, f, t)
				Me.nextRight = nextRight
				Me.transformer = transformer
				Me.basis = basis
				Me.reducer = reducer
			End Sub
			Public Property rawResult As Long?
				Get
					Return result
				End Get
			End Property
			Public Sub compute()
'JAVA TO VB CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
				Dim transformer As java.util.function.ToLongFunction(Of ?)
				Dim reducer As java.util.function.LongBinaryOperator
				transformer = Me.transformer
				reducer = Me.reducer
				If transformer IsNot Nothing AndAlso reducer IsNot Nothing Then
					Dim r As Long = Me.basis
'JAVA TO VB CONVERTER TODO TASK: The following line could not be converted:
					for (int i = baseIndex, f, h; batch > 0 && (h = ((f = baseLimit) + i) >>> 1) > i;)
						addToPendingCount(1)
'JAVA TO VB CONVERTER TODO TASK: Assignments within expressions are not supported in VB
						(rights = New MapReduceValuesToLongTask(Of K, V) (Me, batch >>>= 1, baseLimit = h, f, tab, rights, transformer, r, reducer)).fork()
					Dim p As Node(Of K, V)
'JAVA TO VB CONVERTER TODO TASK: Assignments within expressions are not supported in VB
					Do While (p = advance()) IsNot Nothing
						r = reducer.applyAsLong(r, transformer.applyAsLong(p.val))
					Loop
					result = r
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
					Dim c As CountedCompleter(Of ?)
					c = firstComplete()
					Do While c IsNot Nothing
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
						Dim t As MapReduceValuesToLongTask(Of K, V) = CType(c, MapReduceValuesToLongTask(Of K, V)), s As MapReduceValuesToLongTask(Of K, V) = t.rights
						Do While s IsNot Nothing
							t.result = reducer.applyAsLong(t.result, s.result)
								t.rights = s.nextRight
								s = t.rights
						Loop
						c = c.nextComplete()
					Loop
				End If
			End Sub
		End Class

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
		Friend NotInheritable Class MapReduceEntriesToLongTask(Of K, V)
			Inherits BulkTask(Of K, V, Long?)

			Friend ReadOnly transformer As java.util.function.ToLongFunction(Of KeyValuePair(Of K, V))
			Friend ReadOnly reducer As java.util.function.LongBinaryOperator
			Friend ReadOnly basis As Long
			Friend result As Long
			Friend rights As MapReduceEntriesToLongTask(Of K, V), nextRight As MapReduceEntriesToLongTask(Of K, V)
			Friend Sub New(Of T1)(ByVal p As BulkTask(Of T1), ByVal b As Integer, ByVal i As Integer, ByVal f As Integer, ByVal t As Node(Of K, V)(), ByVal nextRight As MapReduceEntriesToLongTask(Of K, V), ByVal transformer As java.util.function.ToLongFunction(Of KeyValuePair(Of K, V)), ByVal basis As Long, ByVal reducer As java.util.function.LongBinaryOperator)
				MyBase.New(p, b, i, f, t)
				Me.nextRight = nextRight
				Me.transformer = transformer
				Me.basis = basis
				Me.reducer = reducer
			End Sub
			Public Property rawResult As Long?
				Get
					Return result
				End Get
			End Property
			Public Sub compute()
				Dim transformer As java.util.function.ToLongFunction(Of KeyValuePair(Of K, V))
				Dim reducer As java.util.function.LongBinaryOperator
				transformer = Me.transformer
				reducer = Me.reducer
				If transformer IsNot Nothing AndAlso reducer IsNot Nothing Then
					Dim r As Long = Me.basis
'JAVA TO VB CONVERTER TODO TASK: The following line could not be converted:
					for (int i = baseIndex, f, h; batch > 0 && (h = ((f = baseLimit) + i) >>> 1) > i;)
						addToPendingCount(1)
'JAVA TO VB CONVERTER TODO TASK: Assignments within expressions are not supported in VB
						(rights = New MapReduceEntriesToLongTask(Of K, V) (Me, batch >>>= 1, baseLimit = h, f, tab, rights, transformer, r, reducer)).fork()
					Dim p As Node(Of K, V)
'JAVA TO VB CONVERTER TODO TASK: Assignments within expressions are not supported in VB
					Do While (p = advance()) IsNot Nothing
						r = reducer.applyAsLong(r, transformer.applyAsLong(p))
					Loop
					result = r
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
					Dim c As CountedCompleter(Of ?)
					c = firstComplete()
					Do While c IsNot Nothing
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
						Dim t As MapReduceEntriesToLongTask(Of K, V) = CType(c, MapReduceEntriesToLongTask(Of K, V)), s As MapReduceEntriesToLongTask(Of K, V) = t.rights
						Do While s IsNot Nothing
							t.result = reducer.applyAsLong(t.result, s.result)
								t.rights = s.nextRight
								s = t.rights
						Loop
						c = c.nextComplete()
					Loop
				End If
			End Sub
		End Class

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
		Friend NotInheritable Class MapReduceMappingsToLongTask(Of K, V)
			Inherits BulkTask(Of K, V, Long?)

'JAVA TO VB CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
			Friend ReadOnly transformer As java.util.function.ToLongBiFunction(Of ?, ?)
			Friend ReadOnly reducer As java.util.function.LongBinaryOperator
			Friend ReadOnly basis As Long
			Friend result As Long
			Friend rights As MapReduceMappingsToLongTask(Of K, V), nextRight As MapReduceMappingsToLongTask(Of K, V)
'JAVA TO VB CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
			Friend Sub New(Of T1, T2)(ByVal p As BulkTask(Of T1), ByVal b As Integer, ByVal i As Integer, ByVal f As Integer, ByVal t As Node(Of K, V)(), ByVal nextRight As MapReduceMappingsToLongTask(Of K, V), ByVal transformer As java.util.function.ToLongBiFunction(Of T2), ByVal basis As Long, ByVal reducer As java.util.function.LongBinaryOperator)
				MyBase.New(p, b, i, f, t)
				Me.nextRight = nextRight
				Me.transformer = transformer
				Me.basis = basis
				Me.reducer = reducer
			End Sub
			Public Property rawResult As Long?
				Get
					Return result
				End Get
			End Property
			Public Sub compute()
'JAVA TO VB CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
				Dim transformer As java.util.function.ToLongBiFunction(Of ?, ?)
				Dim reducer As java.util.function.LongBinaryOperator
				transformer = Me.transformer
				reducer = Me.reducer
				If transformer IsNot Nothing AndAlso reducer IsNot Nothing Then
					Dim r As Long = Me.basis
'JAVA TO VB CONVERTER TODO TASK: The following line could not be converted:
					for (int i = baseIndex, f, h; batch > 0 && (h = ((f = baseLimit) + i) >>> 1) > i;)
						addToPendingCount(1)
'JAVA TO VB CONVERTER TODO TASK: Assignments within expressions are not supported in VB
						(rights = New MapReduceMappingsToLongTask(Of K, V) (Me, batch >>>= 1, baseLimit = h, f, tab, rights, transformer, r, reducer)).fork()
					Dim p As Node(Of K, V)
'JAVA TO VB CONVERTER TODO TASK: Assignments within expressions are not supported in VB
					Do While (p = advance()) IsNot Nothing
						r = reducer.applyAsLong(r, transformer.applyAsLong(p.key, p.val))
					Loop
					result = r
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
					Dim c As CountedCompleter(Of ?)
					c = firstComplete()
					Do While c IsNot Nothing
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
						Dim t As MapReduceMappingsToLongTask(Of K, V) = CType(c, MapReduceMappingsToLongTask(Of K, V)), s As MapReduceMappingsToLongTask(Of K, V) = t.rights
						Do While s IsNot Nothing
							t.result = reducer.applyAsLong(t.result, s.result)
								t.rights = s.nextRight
								s = t.rights
						Loop
						c = c.nextComplete()
					Loop
				End If
			End Sub
		End Class

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
		Friend NotInheritable Class MapReduceKeysToIntTask(Of K, V)
			Inherits BulkTask(Of K, V, Integer?)

'JAVA TO VB CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
			Friend ReadOnly transformer As java.util.function.ToIntFunction(Of ?)
			Friend ReadOnly reducer As java.util.function.IntBinaryOperator
			Friend ReadOnly basis As Integer
			Friend result As Integer
			Friend rights As MapReduceKeysToIntTask(Of K, V), nextRight As MapReduceKeysToIntTask(Of K, V)
'JAVA TO VB CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
			Friend Sub New(Of T1, T2)(ByVal p As BulkTask(Of T1), ByVal b As Integer, ByVal i As Integer, ByVal f As Integer, ByVal t As Node(Of K, V)(), ByVal nextRight As MapReduceKeysToIntTask(Of K, V), ByVal transformer As java.util.function.ToIntFunction(Of T2), ByVal basis As Integer, ByVal reducer As java.util.function.IntBinaryOperator)
				MyBase.New(p, b, i, f, t)
				Me.nextRight = nextRight
				Me.transformer = transformer
				Me.basis = basis
				Me.reducer = reducer
			End Sub
			Public Property rawResult As Integer?
				Get
					Return result
				End Get
			End Property
			Public Sub compute()
'JAVA TO VB CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
				Dim transformer As java.util.function.ToIntFunction(Of ?)
				Dim reducer As java.util.function.IntBinaryOperator
				transformer = Me.transformer
				reducer = Me.reducer
				If transformer IsNot Nothing AndAlso reducer IsNot Nothing Then
					Dim r As Integer = Me.basis
'JAVA TO VB CONVERTER TODO TASK: The following line could not be converted:
					for (int i = baseIndex, f, h; batch > 0 && (h = ((f = baseLimit) + i) >>> 1) > i;)
						addToPendingCount(1)
'JAVA TO VB CONVERTER TODO TASK: Assignments within expressions are not supported in VB
						(rights = New MapReduceKeysToIntTask(Of K, V) (Me, batch >>>= 1, baseLimit = h, f, tab, rights, transformer, r, reducer)).fork()
					Dim p As Node(Of K, V)
'JAVA TO VB CONVERTER TODO TASK: Assignments within expressions are not supported in VB
					Do While (p = advance()) IsNot Nothing
						r = reducer.applyAsInt(r, transformer.applyAsInt(p.key))
					Loop
					result = r
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
					Dim c As CountedCompleter(Of ?)
					c = firstComplete()
					Do While c IsNot Nothing
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
						Dim t As MapReduceKeysToIntTask(Of K, V) = CType(c, MapReduceKeysToIntTask(Of K, V)), s As MapReduceKeysToIntTask(Of K, V) = t.rights
						Do While s IsNot Nothing
							t.result = reducer.applyAsInt(t.result, s.result)
								t.rights = s.nextRight
								s = t.rights
						Loop
						c = c.nextComplete()
					Loop
				End If
			End Sub
		End Class

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
		Friend NotInheritable Class MapReduceValuesToIntTask(Of K, V)
			Inherits BulkTask(Of K, V, Integer?)

'JAVA TO VB CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
			Friend ReadOnly transformer As java.util.function.ToIntFunction(Of ?)
			Friend ReadOnly reducer As java.util.function.IntBinaryOperator
			Friend ReadOnly basis As Integer
			Friend result As Integer
			Friend rights As MapReduceValuesToIntTask(Of K, V), nextRight As MapReduceValuesToIntTask(Of K, V)
'JAVA TO VB CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
			Friend Sub New(Of T1, T2)(ByVal p As BulkTask(Of T1), ByVal b As Integer, ByVal i As Integer, ByVal f As Integer, ByVal t As Node(Of K, V)(), ByVal nextRight As MapReduceValuesToIntTask(Of K, V), ByVal transformer As java.util.function.ToIntFunction(Of T2), ByVal basis As Integer, ByVal reducer As java.util.function.IntBinaryOperator)
				MyBase.New(p, b, i, f, t)
				Me.nextRight = nextRight
				Me.transformer = transformer
				Me.basis = basis
				Me.reducer = reducer
			End Sub
			Public Property rawResult As Integer?
				Get
					Return result
				End Get
			End Property
			Public Sub compute()
'JAVA TO VB CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
				Dim transformer As java.util.function.ToIntFunction(Of ?)
				Dim reducer As java.util.function.IntBinaryOperator
				transformer = Me.transformer
				reducer = Me.reducer
				If transformer IsNot Nothing AndAlso reducer IsNot Nothing Then
					Dim r As Integer = Me.basis
'JAVA TO VB CONVERTER TODO TASK: The following line could not be converted:
					for (int i = baseIndex, f, h; batch > 0 && (h = ((f = baseLimit) + i) >>> 1) > i;)
						addToPendingCount(1)
'JAVA TO VB CONVERTER TODO TASK: Assignments within expressions are not supported in VB
						(rights = New MapReduceValuesToIntTask(Of K, V) (Me, batch >>>= 1, baseLimit = h, f, tab, rights, transformer, r, reducer)).fork()
					Dim p As Node(Of K, V)
'JAVA TO VB CONVERTER TODO TASK: Assignments within expressions are not supported in VB
					Do While (p = advance()) IsNot Nothing
						r = reducer.applyAsInt(r, transformer.applyAsInt(p.val))
					Loop
					result = r
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
					Dim c As CountedCompleter(Of ?)
					c = firstComplete()
					Do While c IsNot Nothing
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
						Dim t As MapReduceValuesToIntTask(Of K, V) = CType(c, MapReduceValuesToIntTask(Of K, V)), s As MapReduceValuesToIntTask(Of K, V) = t.rights
						Do While s IsNot Nothing
							t.result = reducer.applyAsInt(t.result, s.result)
								t.rights = s.nextRight
								s = t.rights
						Loop
						c = c.nextComplete()
					Loop
				End If
			End Sub
		End Class

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
		Friend NotInheritable Class MapReduceEntriesToIntTask(Of K, V)
			Inherits BulkTask(Of K, V, Integer?)

			Friend ReadOnly transformer As java.util.function.ToIntFunction(Of KeyValuePair(Of K, V))
			Friend ReadOnly reducer As java.util.function.IntBinaryOperator
			Friend ReadOnly basis As Integer
			Friend result As Integer
			Friend rights As MapReduceEntriesToIntTask(Of K, V), nextRight As MapReduceEntriesToIntTask(Of K, V)
			Friend Sub New(Of T1)(ByVal p As BulkTask(Of T1), ByVal b As Integer, ByVal i As Integer, ByVal f As Integer, ByVal t As Node(Of K, V)(), ByVal nextRight As MapReduceEntriesToIntTask(Of K, V), ByVal transformer As java.util.function.ToIntFunction(Of KeyValuePair(Of K, V)), ByVal basis As Integer, ByVal reducer As java.util.function.IntBinaryOperator)
				MyBase.New(p, b, i, f, t)
				Me.nextRight = nextRight
				Me.transformer = transformer
				Me.basis = basis
				Me.reducer = reducer
			End Sub
			Public Property rawResult As Integer?
				Get
					Return result
				End Get
			End Property
			Public Sub compute()
				Dim transformer As java.util.function.ToIntFunction(Of KeyValuePair(Of K, V))
				Dim reducer As java.util.function.IntBinaryOperator
				transformer = Me.transformer
				reducer = Me.reducer
				If transformer IsNot Nothing AndAlso reducer IsNot Nothing Then
					Dim r As Integer = Me.basis
'JAVA TO VB CONVERTER TODO TASK: The following line could not be converted:
					for (int i = baseIndex, f, h; batch > 0 && (h = ((f = baseLimit) + i) >>> 1) > i;)
						addToPendingCount(1)
'JAVA TO VB CONVERTER TODO TASK: Assignments within expressions are not supported in VB
						(rights = New MapReduceEntriesToIntTask(Of K, V) (Me, batch >>>= 1, baseLimit = h, f, tab, rights, transformer, r, reducer)).fork()
					Dim p As Node(Of K, V)
'JAVA TO VB CONVERTER TODO TASK: Assignments within expressions are not supported in VB
					Do While (p = advance()) IsNot Nothing
						r = reducer.applyAsInt(r, transformer.applyAsInt(p))
					Loop
					result = r
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
					Dim c As CountedCompleter(Of ?)
					c = firstComplete()
					Do While c IsNot Nothing
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
						Dim t As MapReduceEntriesToIntTask(Of K, V) = CType(c, MapReduceEntriesToIntTask(Of K, V)), s As MapReduceEntriesToIntTask(Of K, V) = t.rights
						Do While s IsNot Nothing
							t.result = reducer.applyAsInt(t.result, s.result)
								t.rights = s.nextRight
								s = t.rights
						Loop
						c = c.nextComplete()
					Loop
				End If
			End Sub
		End Class

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
		Friend NotInheritable Class MapReduceMappingsToIntTask(Of K, V)
			Inherits BulkTask(Of K, V, Integer?)

'JAVA TO VB CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
			Friend ReadOnly transformer As java.util.function.ToIntBiFunction(Of ?, ?)
			Friend ReadOnly reducer As java.util.function.IntBinaryOperator
			Friend ReadOnly basis As Integer
			Friend result As Integer
			Friend rights As MapReduceMappingsToIntTask(Of K, V), nextRight As MapReduceMappingsToIntTask(Of K, V)
'JAVA TO VB CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
			Friend Sub New(Of T1, T2)(ByVal p As BulkTask(Of T1), ByVal b As Integer, ByVal i As Integer, ByVal f As Integer, ByVal t As Node(Of K, V)(), ByVal nextRight As MapReduceMappingsToIntTask(Of K, V), ByVal transformer As java.util.function.ToIntBiFunction(Of T2), ByVal basis As Integer, ByVal reducer As java.util.function.IntBinaryOperator)
				MyBase.New(p, b, i, f, t)
				Me.nextRight = nextRight
				Me.transformer = transformer
				Me.basis = basis
				Me.reducer = reducer
			End Sub
			Public Property rawResult As Integer?
				Get
					Return result
				End Get
			End Property
			Public Sub compute()
'JAVA TO VB CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
				Dim transformer As java.util.function.ToIntBiFunction(Of ?, ?)
				Dim reducer As java.util.function.IntBinaryOperator
				transformer = Me.transformer
				reducer = Me.reducer
				If transformer IsNot Nothing AndAlso reducer IsNot Nothing Then
					Dim r As Integer = Me.basis
'JAVA TO VB CONVERTER TODO TASK: The following line could not be converted:
					for (int i = baseIndex, f, h; batch > 0 && (h = ((f = baseLimit) + i) >>> 1) > i;)
						addToPendingCount(1)
'JAVA TO VB CONVERTER TODO TASK: Assignments within expressions are not supported in VB
						(rights = New MapReduceMappingsToIntTask(Of K, V) (Me, batch >>>= 1, baseLimit = h, f, tab, rights, transformer, r, reducer)).fork()
					Dim p As Node(Of K, V)
'JAVA TO VB CONVERTER TODO TASK: Assignments within expressions are not supported in VB
					Do While (p = advance()) IsNot Nothing
						r = reducer.applyAsInt(r, transformer.applyAsInt(p.key, p.val))
					Loop
					result = r
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
					Dim c As CountedCompleter(Of ?)
					c = firstComplete()
					Do While c IsNot Nothing
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
						Dim t As MapReduceMappingsToIntTask(Of K, V) = CType(c, MapReduceMappingsToIntTask(Of K, V)), s As MapReduceMappingsToIntTask(Of K, V) = t.rights
						Do While s IsNot Nothing
							t.result = reducer.applyAsInt(t.result, s.result)
								t.rights = s.nextRight
								s = t.rights
						Loop
						c = c.nextComplete()
					Loop
				End If
			End Sub
		End Class

		' Unsafe mechanics
		Private Shared ReadOnly U As sun.misc.Unsafe
		Private Shared ReadOnly SIZECTL As Long
		Private Shared ReadOnly TRANSFERINDEX As Long
		Private Shared ReadOnly BASECOUNT As Long
		Private Shared ReadOnly CELLSBUSY As Long
		Private Shared ReadOnly CELLVALUE As Long
		Private Shared ReadOnly ABASE As Long
		Private Shared ReadOnly ASHIFT As Integer

		Shared Sub New()
			Try
				U = sun.misc.Unsafe.unsafe
				Dim k As  [Class] = GetType(ConcurrentHashMap)
				SIZECTL = U.objectFieldOffset(k.getDeclaredField("sizeCtl"))
				TRANSFERINDEX = U.objectFieldOffset(k.getDeclaredField("transferIndex"))
				BASECOUNT = U.objectFieldOffset(k.getDeclaredField("baseCount"))
				CELLSBUSY = U.objectFieldOffset(k.getDeclaredField("cellsBusy"))
				Dim ck As  [Class] = GetType(CounterCell)
				CELLVALUE = U.objectFieldOffset(ck.getDeclaredField("value"))
				Dim ak As  [Class] = GetType(Node())
				ABASE = U.arrayBaseOffset(ak)
				Dim scale As Integer = U.arrayIndexScale(ak)
				If (scale And (scale - 1)) <> 0 Then Throw New [Error]("data type scale not a power of two")
				ASHIFT = 31 -  java.lang.[Integer].numberOfLeadingZeros(scale)
			Catch e As Exception
				Throw New [Error](e)
			End Try
		End Sub
	End Class

End Namespace