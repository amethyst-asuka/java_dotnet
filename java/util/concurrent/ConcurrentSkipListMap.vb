Imports System
Imports System.Collections
Imports System.Collections.Generic

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
	''' A scalable concurrent <seealso cref="ConcurrentNavigableMap"/> implementation.
	''' The map is sorted according to the {@link Comparable natural
	''' ordering} of its keys, or by a <seealso cref="Comparator"/> provided at map
	''' creation time, depending on which constructor is used.
	''' 
	''' <p>This class implements a concurrent variant of <a
	''' href="http://en.wikipedia.org/wiki/Skip_list" target="_top">SkipLists</a>
	''' providing expected average <i>log(n)</i> time cost for the
	''' {@code containsKey}, {@code get}, {@code put} and
	''' {@code remove} operations and their variants.  Insertion, removal,
	''' update, and access operations safely execute concurrently by
	''' multiple threads.
	''' 
	''' <p>Iterators and spliterators are
	''' <a href="package-summary.html#Weakly"><i>weakly consistent</i></a>.
	''' 
	''' <p>Ascending key ordered views and their iterators are faster than
	''' descending ones.
	''' 
	''' <p>All {@code Map.Entry} pairs returned by methods in this class
	''' and its views represent snapshots of mappings at the time they were
	''' produced. They do <em>not</em> support the {@code Entry.setValue}
	''' method. (Note however that it is possible to change mappings in the
	''' associated map using {@code put}, {@code putIfAbsent}, or
	''' {@code replace}, depending on exactly which effect you need.)
	''' 
	''' <p>Beware that, unlike in most collections, the {@code size}
	''' method is <em>not</em> a constant-time operation. Because of the
	''' asynchronous nature of these maps, determining the current number
	''' of elements requires a traversal of the elements, and so may report
	''' inaccurate results if this collection is modified during traversal.
	''' Additionally, the bulk operations {@code putAll}, {@code equals},
	''' {@code toArray}, {@code containsValue}, and {@code clear} are
	''' <em>not</em> guaranteed to be performed atomically. For example, an
	''' iterator operating concurrently with a {@code putAll} operation
	''' might view only some of the added elements.
	''' 
	''' <p>This class and its views and iterators implement all of the
	''' <em>optional</em> methods of the <seealso cref="Map"/> and <seealso cref="Iterator"/>
	''' interfaces. Like most other concurrent collections, this class does
	''' <em>not</em> permit the use of {@code null} keys or values because some
	''' null return values cannot be reliably distinguished from the absence of
	''' elements.
	''' 
	''' <p>This class is a member of the
	''' <a href="{@docRoot}/../technotes/guides/collections/index.html">
	''' Java Collections Framework</a>.
	''' 
	''' @author Doug Lea </summary>
	''' @param <K> the type of keys maintained by this map </param>
	''' @param <V> the type of mapped values
	''' @since 1.6 </param>
	<Serializable> _
	Public Class ConcurrentSkipListMap(Of K, V)
		Inherits java.util.AbstractMap(Of K, V)
		Implements java.util.concurrent.ConcurrentNavigableMap(Of K, V), Cloneable

	'    
	'     * This class implements a tree-like two-dimensionally linked skip
	'     * list in which the index levels are represented in separate
	'     * nodes from the base nodes holding data.  There are two reasons
	'     * for taking this approach instead of the usual array-based
	'     * structure: 1) Array based implementations seem to encounter
	'     * more complexity and overhead 2) We can use cheaper algorithms
	'     * for the heavily-traversed index lists than can be used for the
	'     * base lists.  Here's a picture of some of the basics for a
	'     * possible list with 2 levels of index:
	'     *
	'     * Head nodes          Index nodes
	'     * +-+    right        +-+                      +-+
	'     * |2|---------------->| |--------------------->| |->null
	'     * +-+                 +-+                      +-+
	'     *  | down              |                        |
	'     *  v                   v                        v
	'     * +-+            +-+  +-+       +-+            +-+       +-+
	'     * |1|----------->| |->| |------>| |----------->| |------>| |->null
	'     * +-+            +-+  +-+       +-+            +-+       +-+
	'     *  v              |    |         |              |         |
	'     * Nodes  next     v    v         v              v         v
	'     * +-+  +-+  +-+  +-+  +-+  +-+  +-+  +-+  +-+  +-+  +-+  +-+
	'     * | |->|A|->|B|->|C|->|D|->|E|->|F|->|G|->|H|->|I|->|J|->|K|->null
	'     * +-+  +-+  +-+  +-+  +-+  +-+  +-+  +-+  +-+  +-+  +-+  +-+
	'     *
	'     * The base lists use a variant of the HM linked ordered set
	'     * algorithm. See Tim Harris, "A pragmatic implementation of
	'     * non-blocking linked lists"
	'     * http://www.cl.cam.ac.uk/~tlh20/publications.html and Maged
	'     * Michael "High Performance Dynamic Lock-Free Hash Tables and
	'     * List-Based Sets"
	'     * http://www.research.ibm.com/people/m/michael/pubs.htm.  The
	'     * basic idea in these lists is to mark the "next" pointers of
	'     * deleted nodes when deleting to avoid conflicts with concurrent
	'     * insertions, and when traversing to keep track of triples
	'     * (predecessor, node, successor) in order to detect when and how
	'     * to unlink these deleted nodes.
	'     *
	'     * Rather than using mark-bits to mark list deletions (which can
	'     * be slow and space-intensive using AtomicMarkedReference), nodes
	'     * use direct CAS'able next pointers.  On deletion, instead of
	'     * marking a pointer, they splice in another node that can be
	'     * thought of as standing for a marked pointer (indicating this by
	'     * using otherwise impossible field values).  Using plain nodes
	'     * acts roughly like "boxed" implementations of marked pointers,
	'     * but uses new nodes only when nodes are deleted, not for every
	'     * link.  This requires less space and supports faster
	'     * traversal. Even if marked references were better supported by
	'     * JVMs, traversal using this technique might still be faster
	'     * because any search need only read ahead one more node than
	'     * otherwise required (to check for trailing marker) rather than
	'     * unmasking mark bits or whatever on each read.
	'     *
	'     * This approach maintains the essential property needed in the HM
	'     * algorithm of changing the next-pointer of a deleted node so
	'     * that any other CAS of it will fail, but implements the idea by
	'     * changing the pointer to point to a different node, not by
	'     * marking it.  While it would be possible to further squeeze
	'     * space by defining marker nodes not to have key/value fields, it
	'     * isn't worth the extra type-testing overhead.  The deletion
	'     * markers are rarely encountered during traversal and are
	'     * normally quickly garbage collected. (Note that this technique
	'     * would not work well in systems without garbage collection.)
	'     *
	'     * In addition to using deletion markers, the lists also use
	'     * nullness of value fields to indicate deletion, in a style
	'     * similar to typical lazy-deletion schemes.  If a node's value is
	'     * null, then it is considered logically deleted and ignored even
	'     * though it is still reachable. This maintains proper control of
	'     * concurrent replace vs delete operations -- an attempted replace
	'     * must fail if a delete beat it by nulling field, and a delete
	'     * must return the last non-null value held in the field. (Note:
	'     * Null, rather than some special marker, is used for value fields
	'     * here because it just so happens to mesh with the Map API
	'     * requirement that method get returns null if there is no
	'     * mapping, which allows nodes to remain concurrently readable
	'     * even when deleted. Using any other marker value here would be
	'     * messy at best.)
	'     *
	'     * Here's the sequence of events for a deletion of node n with
	'     * predecessor b and successor f, initially:
	'     *
	'     *        +------+       +------+      +------+
	'     *   ...  |   b  |------>|   n  |----->|   f  | ...
	'     *        +------+       +------+      +------+
	'     *
	'     * 1. CAS n's value field from non-null to null.
	'     *    From this point on, no public operations encountering
	'     *    the node consider this mapping to exist. However, other
	'     *    ongoing insertions and deletions might still modify
	'     *    n's next pointer.
	'     *
	'     * 2. CAS n's next pointer to point to a new marker node.
	'     *    From this point on, no other nodes can be appended to n.
	'     *    which avoids deletion errors in CAS-based linked lists.
	'     *
	'     *        +------+       +------+      +------+       +------+
	'     *   ...  |   b  |------>|   n  |----->|marker|------>|   f  | ...
	'     *        +------+       +------+      +------+       +------+
	'     *
	'     * 3. CAS b's next pointer over both n and its marker.
	'     *    From this point on, no new traversals will encounter n,
	'     *    and it can eventually be GCed.
	'     *        +------+                                    +------+
	'     *   ...  |   b  |----------------------------------->|   f  | ...
	'     *        +------+                                    +------+
	'     *
	'     * A failure at step 1 leads to simple retry due to a lost race
	'     * with another operation. Steps 2-3 can fail because some other
	'     * thread noticed during a traversal a node with null value and
	'     * helped out by marking and/or unlinking.  This helping-out
	'     * ensures that no thread can become stuck waiting for progress of
	'     * the deleting thread.  The use of marker nodes slightly
	'     * complicates helping-out code because traversals must track
	'     * consistent reads of up to four nodes (b, n, marker, f), not
	'     * just (b, n, f), although the next field of a marker is
	'     * immutable, and once a next field is CAS'ed to point to a
	'     * marker, it never again changes, so this requires less care.
	'     *
	'     * Skip lists add indexing to this scheme, so that the base-level
	'     * traversals start close to the locations being found, inserted
	'     * or deleted -- usually base level traversals only traverse a few
	'     * nodes. This doesn't change the basic algorithm except for the
	'     * need to make sure base traversals start at predecessors (here,
	'     * b) that are not (structurally) deleted, otherwise retrying
	'     * after processing the deletion.
	'     *
	'     * Index levels are maintained as lists with volatile next fields,
	'     * using CAS to link and unlink.  Races are allowed in index-list
	'     * operations that can (rarely) fail to link in a new index node
	'     * or delete one. (We can't do this of course for data nodes.)
	'     * However, even when this happens, the index lists remain sorted,
	'     * so correctly serve as indices.  This can impact performance,
	'     * but since skip lists are probabilistic anyway, the net result
	'     * is that under contention, the effective "p" value may be lower
	'     * than its nominal value. And race windows are kept small enough
	'     * that in practice these failures are rare, even under a lot of
	'     * contention.
	'     *
	'     * The fact that retries (for both base and index lists) are
	'     * relatively cheap due to indexing allows some minor
	'     * simplifications of retry logic. Traversal restarts are
	'     * performed after most "helping-out" CASes. This isn't always
	'     * strictly necessary, but the implicit backoffs tend to help
	'     * reduce other downstream failed CAS's enough to outweigh restart
	'     * cost.  This worsens the worst case, but seems to improve even
	'     * highly contended cases.
	'     *
	'     * Unlike most skip-list implementations, index insertion and
	'     * deletion here require a separate traversal pass occurring after
	'     * the base-level action, to add or remove index nodes.  This adds
	'     * to single-threaded overhead, but improves contended
	'     * multithreaded performance by narrowing interference windows,
	'     * and allows deletion to ensure that all index nodes will be made
	'     * unreachable upon return from a public remove operation, thus
	'     * avoiding unwanted garbage retention. This is more important
	'     * here than in some other data structures because we cannot null
	'     * out node fields referencing user keys since they might still be
	'     * read by other ongoing traversals.
	'     *
	'     * Indexing uses skip list parameters that maintain good search
	'     * performance while using sparser-than-usual indices: The
	'     * hardwired parameters k=1, p=0.5 (see method doPut) mean
	'     * that about one-quarter of the nodes have indices. Of those that
	'     * do, half have one level, a quarter have two, and so on (see
	'     * Pugh's Skip List Cookbook, sec 3.4).  The expected total space
	'     * requirement for a map is slightly less than for the current
	'     * implementation of java.util.TreeMap.
	'     *
	'     * Changing the level of the index (i.e, the height of the
	'     * tree-like structure) also uses CAS. The head index has initial
	'     * level/height of one. Creation of an index with height greater
	'     * than the current level adds a level to the head index by
	'     * CAS'ing on a new top-most head. To maintain good performance
	'     * after a lot of removals, deletion methods heuristically try to
	'     * reduce the height if the topmost levels appear to be empty.
	'     * This may encounter races in which it possible (but rare) to
	'     * reduce and "lose" a level just as it is about to contain an
	'     * index (that will then never be encountered). This does no
	'     * structural harm, and in practice appears to be a better option
	'     * than allowing unrestrained growth of levels.
	'     *
	'     * The code for all this is more verbose than you'd like. Most
	'     * operations entail locating an element (or position to insert an
	'     * element). The code to do this can't be nicely factored out
	'     * because subsequent uses require a snapshot of predecessor
	'     * and/or successor and/or value fields which can't be returned
	'     * all at once, at least not without creating yet another object
	'     * to hold them -- creating such little objects is an especially
	'     * bad idea for basic internal search operations because it adds
	'     * to GC overhead.  (This is one of the few times I've wished Java
	'     * had macros.) Instead, some traversal code is interleaved within
	'     * insertion and removal operations.  The control logic to handle
	'     * all the retry conditions is sometimes twisty. Most search is
	'     * broken into 2 parts. findPredecessor() searches index nodes
	'     * only, returning a base-level predecessor of the key. findNode()
	'     * finishes out the base-level search. Even with this factoring,
	'     * there is a fair amount of near-duplication of code to handle
	'     * variants.
	'     *
	'     * To produce random values without interference across threads,
	'     * we use within-JDK thread local random support (via the
	'     * "secondary seed", to avoid interference with user-level
	'     * ThreadLocalRandom.)
	'     *
	'     * A previous version of this class wrapped non-comparable keys
	'     * with their comparators to emulate Comparables when using
	'     * comparators vs Comparables.  However, JVMs now appear to better
	'     * handle infusing comparator-vs-comparable choice into search
	'     * loops. Static method cpr(comparator, x, y) is used for all
	'     * comparisons, which works well as long as the comparator
	'     * argument is set up outside of loops (thus sometimes passed as
	'     * an argument to internal methods) to avoid field re-reads.
	'     *
	'     * For explanation of algorithms sharing at least a couple of
	'     * features with this one, see Mikhail Fomitchev's thesis
	'     * (http://www.cs.yorku.ca/~mikhail/), Keir Fraser's thesis
	'     * (http://www.cl.cam.ac.uk/users/kaf24/), and Hakan Sundell's
	'     * thesis (http://www.cs.chalmers.se/~phs/).
	'     *
	'     * Given the use of tree-like index nodes, you might wonder why
	'     * this doesn't use some kind of search tree instead, which would
	'     * support somewhat faster search operations. The reason is that
	'     * there are no known efficient lock-free insertion and deletion
	'     * algorithms for search trees. The immutability of the "down"
	'     * links of index nodes (as opposed to mutable "left" fields in
	'     * true trees) makes this tractable using only CAS operations.
	'     *
	'     * Notation guide for local variables
	'     * Node:         b, n, f    for  predecessor, node, successor
	'     * Index:        q, r, d    for index node, right, down.
	'     *               t          for another index node
	'     * Head:         h
	'     * Levels:       j
	'     * Keys:         k, key
	'     * Values:       v, value
	'     * Comparisons:  c
	'     

		Private Const serialVersionUID As Long = -8627078645895051609L

		''' <summary>
		''' Special value used to identify base-level header
		''' </summary>
		Private Shared ReadOnly BASE_HEADER As New Object

		''' <summary>
		''' The topmost head index of the skiplist.
		''' </summary>
'JAVA TO VB CONVERTER TODO TASK: There is no VB equivalent to 'volatile':
		<NonSerialized> _
		Private head As HeadIndex(Of K, V)

		''' <summary>
		''' The comparator used to maintain order in this map, or null if
		''' using natural ordering.  (Non-private to simplify access in
		''' nested classes.)
		''' @serial
		''' </summary>
'JAVA TO VB CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
		Friend ReadOnly comparator_Renamed As IComparer(Of ?)

		''' <summary>
		''' Lazily initialized key set </summary>
		<NonSerialized> _
		Private keySet_Renamed As KeySet(Of K)
		''' <summary>
		''' Lazily initialized entry set </summary>
		<NonSerialized> _
		Private entrySet_Renamed As EntrySet(Of K, V)
		''' <summary>
		''' Lazily initialized values collection </summary>
		<NonSerialized> _
		Private values_Renamed As Values(Of V)
		''' <summary>
		''' Lazily initialized descending key set </summary>
		<NonSerialized> _
		Private descendingMap_Renamed As java.util.concurrent.ConcurrentNavigableMap(Of K, V)

		''' <summary>
		''' Initializes or resets state. Needed by constructors, clone,
		''' clear, readObject. and ConcurrentSkipListSet.clone.
		''' (Note that comparator must be separately initialized.)
		''' </summary>
		Private Sub initialize()
			keySet_Renamed = Nothing
			entrySet_Renamed = Nothing
			values_Renamed = Nothing
			descendingMap_Renamed = Nothing
			head = New HeadIndex(Of K, V)(New Node(Of K, V)(Nothing, BASE_HEADER, Nothing), Nothing, Nothing, 1)
		End Sub

		''' <summary>
		''' compareAndSet head node
		''' </summary>
		Private Function casHead(ByVal cmp As HeadIndex(Of K, V), ByVal val As HeadIndex(Of K, V)) As Boolean
			Return UNSAFE.compareAndSwapObject(Me, headOffset, cmp, val)
		End Function

		' ---------------- Nodes -------------- 

		''' <summary>
		''' Nodes hold keys and values, and are singly linked in sorted
		''' order, possibly with some intervening marker nodes. The list is
		''' headed by a dummy node accessible as head.node. The value field
		''' is declared only as Object because it takes special non-V
		''' values for marker and header nodes.
		''' </summary>
		Friend NotInheritable Class Node(Of K, V)
			Friend ReadOnly key As K
'JAVA TO VB CONVERTER TODO TASK: There is no VB equivalent to 'volatile':
			Friend value As Object
'JAVA TO VB CONVERTER TODO TASK: There is no VB equivalent to 'volatile':
			Friend [next] As Node(Of K, V)

			''' <summary>
			''' Creates a new regular node.
			''' </summary>
			Friend Sub New(ByVal key As K, ByVal value As Object, ByVal [next] As Node(Of K, V))
				Me.key = key
				Me.value = value
				Me.next = [next]
			End Sub

			''' <summary>
			''' Creates a new marker node. A marker is distinguished by
			''' having its value field point to itself.  Marker nodes also
			''' have null keys, a fact that is exploited in a few places,
			''' but this doesn't distinguish markers from the base-level
			''' header node (head.node), which also has a null key.
			''' </summary>
			Friend Sub New(ByVal [next] As Node(Of K, V))
				Me.key = Nothing
				Me.value = Me
				Me.next = [next]
			End Sub

			''' <summary>
			''' compareAndSet value field
			''' </summary>
			Friend Function casValue(ByVal cmp As Object, ByVal val As Object) As Boolean
				Return UNSAFE.compareAndSwapObject(Me, valueOffset, cmp, val)
			End Function

			''' <summary>
			''' compareAndSet next field
			''' </summary>
			Friend Function casNext(ByVal cmp As Node(Of K, V), ByVal val As Node(Of K, V)) As Boolean
				Return UNSAFE.compareAndSwapObject(Me, nextOffset, cmp, val)
			End Function

			''' <summary>
			''' Returns true if this node is a marker. This method isn't
			''' actually called in any current code checking for markers
			''' because callers will have already read value field and need
			''' to use that read (not another done here) and so directly
			''' test if value points to node.
			''' </summary>
			''' <returns> true if this node is a marker node </returns>
			Friend Property marker As Boolean
				Get
					Return value Is Me
				End Get
			End Property

			''' <summary>
			''' Returns true if this node is the header of base-level list. </summary>
			''' <returns> true if this node is header node </returns>
			Friend Property baseHeader As Boolean
				Get
					Return value Is BASE_HEADER
				End Get
			End Property

			''' <summary>
			''' Tries to append a deletion marker to this node. </summary>
			''' <param name="f"> the assumed current successor of this node </param>
			''' <returns> true if successful </returns>
			Friend Function appendMarker(ByVal f As Node(Of K, V)) As Boolean
				Return casNext(f, New Node(Of K, V)(f))
			End Function

			''' <summary>
			''' Helps out a deletion by appending marker or unlinking from
			''' predecessor. This is called during traversals when value
			''' field seen to be null. </summary>
			''' <param name="b"> predecessor </param>
			''' <param name="f"> successor </param>
			Friend Sub helpDelete(ByVal b As Node(Of K, V), ByVal f As Node(Of K, V))
	'            
	'             * Rechecking links and then doing only one of the
	'             * help-out stages per call tends to minimize CAS
	'             * interference among helping threads.
	'             
				If f Is [next] AndAlso Me Is b.next Then
					If f Is Nothing OrElse f.value IsNot f Then ' not already marked
						casNext(f, New Node(Of K, V)(f))
					Else
						b.casNext(Me, f.next)
					End If
				End If
			End Sub

			''' <summary>
			''' Returns value if this node contains a valid key-value pair,
			''' else null. </summary>
			''' <returns> this node's value if it isn't a marker or header or
			''' is deleted, else null </returns>
			Friend Property validValue As V
				Get
					Dim v As Object = value
					If v Is Me OrElse v Is BASE_HEADER Then Return Nothing
	'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
					Dim vv As V = CType(v, V)
					Return vv
				End Get
			End Property

			''' <summary>
			''' Creates and returns a new SimpleImmutableEntry holding current
			''' mapping if this node holds a valid value, else null. </summary>
			''' <returns> new entry or null </returns>
			Friend Function createSnapshot() As java.util.AbstractMap.SimpleImmutableEntry(Of K, V)
				Dim v As Object = value
				If v Is Nothing OrElse v Is Me OrElse v Is BASE_HEADER Then Return Nothing
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
				Dim vv As V = CType(v, V)
				Return New java.util.AbstractMap.SimpleImmutableEntry(Of K, V)(key, vv)
			End Function

			' UNSAFE mechanics

			Private Shared ReadOnly UNSAFE As sun.misc.Unsafe
			Private Shared ReadOnly valueOffset As Long
			Private Shared ReadOnly nextOffset As Long

			Shared Sub New()
				Try
					UNSAFE = sun.misc.Unsafe.unsafe
					Dim k As  [Class] = GetType(Node)
					valueOffset = UNSAFE.objectFieldOffset(k.getDeclaredField("value"))
					nextOffset = UNSAFE.objectFieldOffset(k.getDeclaredField("next"))
				Catch e As Exception
					Throw New [Error](e)
				End Try
			End Sub
		End Class

		' ---------------- Indexing -------------- 

		''' <summary>
		''' Index nodes represent the levels of the skip list.  Note that
		''' even though both Nodes and Indexes have forward-pointing
		''' fields, they have different types and are handled in different
		''' ways, that can't nicely be captured by placing field in a
		''' shared abstract class.
		''' </summary>
		Friend Class Index(Of K, V)
			Friend ReadOnly node_Renamed As Node(Of K, V)
			Friend ReadOnly down As Index(Of K, V)
'JAVA TO VB CONVERTER TODO TASK: There is no VB equivalent to 'volatile':
			Friend right As Index(Of K, V)

			''' <summary>
			''' Creates index node with given values.
			''' </summary>
			Friend Sub New(ByVal node_Renamed As Node(Of K, V), ByVal down As Index(Of K, V), ByVal right As Index(Of K, V))
				Me.node_Renamed = node_Renamed
				Me.down = down
				Me.right = right
			End Sub

			''' <summary>
			''' compareAndSet right field
			''' </summary>
			Friend Function casRight(ByVal cmp As Index(Of K, V), ByVal val As Index(Of K, V)) As Boolean
				Return UNSAFE.compareAndSwapObject(Me, rightOffset, cmp, val)
			End Function

			''' <summary>
			''' Returns true if the node this indexes has been deleted. </summary>
			''' <returns> true if indexed node is known to be deleted </returns>
			Friend Function indexesDeletedNode() As Boolean
				Return node_Renamed.value Is Nothing
			End Function

			''' <summary>
			''' Tries to CAS newSucc as successor.  To minimize races with
			''' unlink that may lose this index node, if the node being
			''' indexed is known to be deleted, it doesn't try to link in. </summary>
			''' <param name="succ"> the expected current successor </param>
			''' <param name="newSucc"> the new successor </param>
			''' <returns> true if successful </returns>
			Friend Function link(ByVal succ As Index(Of K, V), ByVal newSucc As Index(Of K, V)) As Boolean
				Dim n As Node(Of K, V) = node_Renamed
				newSucc.right = succ
				Return n.value IsNot Nothing AndAlso casRight(succ, newSucc)
			End Function

			''' <summary>
			''' Tries to CAS right field to skip over apparent successor
			''' succ.  Fails (forcing a retraversal by caller) if this node
			''' is known to be deleted. </summary>
			''' <param name="succ"> the expected current successor </param>
			''' <returns> true if successful </returns>
			Friend Function unlink(ByVal succ As Index(Of K, V)) As Boolean
				Return node_Renamed.value IsNot Nothing AndAlso casRight(succ, succ.right)
			End Function

			' Unsafe mechanics
			Private Shared ReadOnly UNSAFE As sun.misc.Unsafe
			Private Shared ReadOnly rightOffset As Long
			Shared Sub New()
				Try
					UNSAFE = sun.misc.Unsafe.unsafe
					Dim k As  [Class] = GetType(Index)
					rightOffset = UNSAFE.objectFieldOffset(k.getDeclaredField("right"))
				Catch e As Exception
					Throw New [Error](e)
				End Try
			End Sub
		End Class

		' ---------------- Head nodes -------------- 

		''' <summary>
		''' Nodes heading each level keep track of their level.
		''' </summary>
		Friend NotInheritable Class HeadIndex(Of K, V)
			Inherits Index(Of K, V)

			Friend ReadOnly level As Integer
			Friend Sub New(ByVal node_Renamed As Node(Of K, V), ByVal down As Index(Of K, V), ByVal right As Index(Of K, V), ByVal level As Integer)
				MyBase.New(node_Renamed, down, right)
				Me.level = level
			End Sub
		End Class

		' ---------------- Comparison utilities -------------- 

		''' <summary>
		''' Compares using comparator or natural ordering if null.
		''' Called only by methods that have performed required type checks.
		''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
		Friend Shared Function cpr(ByVal c As IComparer, ByVal x As Object, ByVal y As Object) As Integer
			Return If(c IsNot Nothing, c.Compare(x, y), CType(x, Comparable).CompareTo(y))
		End Function

		' ---------------- Traversal -------------- 

		''' <summary>
		''' Returns a base-level node with key strictly less than given key,
		''' or the base-level header if there is no such node.  Also
		''' unlinks indexes to deleted nodes found along the way.  Callers
		''' rely on this side-effect of clearing indices to deleted nodes. </summary>
		''' <param name="key"> the key </param>
		''' <returns> a predecessor of key </returns>
'JAVA TO VB CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
		Private Function findPredecessor(Of T1)(ByVal key As Object, ByVal cmp As IComparer(Of T1)) As Node(Of K, V)
			If key Is Nothing Then Throw New NullPointerException ' don't postpone errors
			Do
'JAVA TO VB CONVERTER TODO TASK: The following line could not be converted:
				for (Index<K,V> q = head, r = q.right, d;;)
					If r IsNot Nothing Then
						Dim n As Node(Of K, V) = r.node_Renamed
						Dim k As K = n.key
						If n.value Is Nothing Then
							If Not q.unlink(r) Then Exit Do ' restart
							r = q.right ' reread r
							Continue Do
						End If
						If cpr(cmp, key, k) > 0 Then
							q = r
							r = r.right
							Continue Do
						End If
					End If
					d = q.down
					If d Is Nothing Then Return q.node_Renamed
					q = d
					r = d.right
			Loop
		End Function

		''' <summary>
		''' Returns node holding key or null if no such, clearing out any
		''' deleted nodes seen along the way.  Repeatedly traverses at
		''' base-level looking for key starting at predecessor returned
		''' from findPredecessor, processing base-level deletions as
		''' encountered. Some callers rely on this side-effect of clearing
		''' deleted nodes.
		''' 
		''' Restarts occur, at traversal step centered on node n, if:
		''' 
		'''   (1) After reading n's next field, n is no longer assumed
		'''       predecessor b's current successor, which means that
		'''       we don't have a consistent 3-node snapshot and so cannot
		'''       unlink any subsequent deleted nodes encountered.
		''' 
		'''   (2) n's value field is null, indicating n is deleted, in
		'''       which case we help out an ongoing structural deletion
		'''       before retrying.  Even though there are cases where such
		'''       unlinking doesn't require restart, they aren't sorted out
		'''       here because doing so would not usually outweigh cost of
		'''       restarting.
		''' 
		'''   (3) n is a marker or n's predecessor's value field is null,
		'''       indicating (among other possibilities) that
		'''       findPredecessor returned a deleted node. We can't unlink
		'''       the node because we don't know its predecessor, so rely
		'''       on another call to findPredecessor to notice and return
		'''       some earlier predecessor, which it will do. This check is
		'''       only strictly needed at beginning of loop, (and the
		'''       b.value check isn't strictly needed at all) but is done
		'''       each iteration to help avoid contention with other
		'''       threads by callers that will fail to be able to change
		'''       links, and so will retry anyway.
		''' 
		''' The traversal loops in doPut, doRemove, and findNear all
		''' include the same three kinds of checks. And specialized
		''' versions appear in findFirst, and findLast and their
		''' variants. They can't easily share code because each uses the
		''' reads of fields held in locals occurring in the orders they
		''' were performed.
		''' </summary>
		''' <param name="key"> the key </param>
		''' <returns> node holding key, or null if no such </returns>
		Private Function findNode(ByVal key As Object) As Node(Of K, V)
			If key Is Nothing Then Throw New NullPointerException ' don't postpone errors
'JAVA TO VB CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
			Dim cmp As IComparer(Of ?) = comparator_Renamed
			outer:
			Do
				Dim b As Node(Of K, V) = findPredecessor(key, cmp)
				Dim n As Node(Of K, V) = b.next
				Do
					Dim v As Object
					Dim c As Integer
					If n Is Nothing Then GoTo outer
					Dim f As Node(Of K, V) = n.next
					If n IsNot b.next Then ' inconsistent read Exit Do
					v = n.value
					If v Is Nothing Then ' n is deleted
						n.helpDelete(b, f)
						Exit Do
					End If
					If b.value Is Nothing OrElse v Is n Then ' b is deleted Exit Do
					c = cpr(cmp, key, n.key)
					If c = 0 Then Return n
					If c < 0 Then GoTo outer
					b = n
					n = f
				Loop
			Loop
			Return Nothing
		End Function

		''' <summary>
		''' Gets value for key. Almost the same as findNode, but returns
		''' the found value (to avoid retries during re-reads)
		''' </summary>
		''' <param name="key"> the key </param>
		''' <returns> the value, or null if absent </returns>
		Private Function doGet(ByVal key As Object) As V
			If key Is Nothing Then Throw New NullPointerException
'JAVA TO VB CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
			Dim cmp As IComparer(Of ?) = comparator_Renamed
			outer:
			Do
				Dim b As Node(Of K, V) = findPredecessor(key, cmp)
				Dim n As Node(Of K, V) = b.next
				Do
					Dim v As Object
					Dim c As Integer
					If n Is Nothing Then GoTo outer
					Dim f As Node(Of K, V) = n.next
					If n IsNot b.next Then ' inconsistent read Exit Do
					v = n.value
					If v Is Nothing Then ' n is deleted
						n.helpDelete(b, f)
						Exit Do
					End If
					If b.value Is Nothing OrElse v Is n Then ' b is deleted Exit Do
					c = cpr(cmp, key, n.key)
					If c = 0 Then
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
						Dim vv As V = CType(v, V)
						Return vv
					End If
					If c < 0 Then GoTo outer
					b = n
					n = f
				Loop
			Loop
			Return Nothing
		End Function

		' ---------------- Insertion -------------- 

		''' <summary>
		''' Main insertion method.  Adds element if not present, or
		''' replaces value if present and onlyIfAbsent is false. </summary>
		''' <param name="key"> the key </param>
		''' <param name="value"> the value that must be associated with key </param>
		''' <param name="onlyIfAbsent"> if should not insert if already present </param>
		''' <returns> the old value, or null if newly inserted </returns>
		Private Function doPut(ByVal key As K, ByVal value As V, ByVal onlyIfAbsent As Boolean) As V
			Dim z As Node(Of K, V) ' added node
			If key Is Nothing Then Throw New NullPointerException
'JAVA TO VB CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
			Dim cmp As IComparer(Of ?) = comparator_Renamed
			outer:
			Do
				Dim b As Node(Of K, V) = findPredecessor(key, cmp)
				Dim n As Node(Of K, V) = b.next
				Do
					If n IsNot Nothing Then
						Dim v As Object
						Dim c As Integer
						Dim f As Node(Of K, V) = n.next
						If n IsNot b.next Then ' inconsistent read Exit Do
						v = n.value
						If v Is Nothing Then ' n is deleted
							n.helpDelete(b, f)
							Exit Do
						End If
						If b.value Is Nothing OrElse v Is n Then ' b is deleted Exit Do
						c = cpr(cmp, key, n.key)
						If c > 0 Then
							b = n
							n = f
							Continue Do
						End If
						If c = 0 Then
							If onlyIfAbsent OrElse n.casValue(v, value) Then
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
								Dim vv As V = CType(v, V)
								Return vv
							End If
							Exit Do ' restart if lost race to replace value
						End If
						' else c < 0; fall through
					End If

					z = New Node(Of K, V)(key, value, n)
					If Not b.casNext(n, z) Then Exit Do ' restart if lost race to append to b
					GoTo outer
				Loop
			Loop

			Dim rnd As Integer = ThreadLocalRandom.nextSecondarySeed()
			If (rnd And &H80000001L) = 0 Then ' test highest and lowest bits
				Dim level As Integer = 1, max As Integer
				Do While ((rnd >>>= 1) And 1) <> 0
					level += 1
				Loop
				Dim idx As Index(Of K, V) = Nothing
				Dim h As HeadIndex(Of K, V) = head
				max = h.level
				If level <= max Then
					Dim i As Integer = 1
					Do While i <= level
						idx = New Index(Of K, V)(z, idx, Nothing)
						i += 1
					Loop
				Else ' try to grow by one level
					level = max + 1 ' hold in array and later pick the one to use
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
					Dim idxs As Index(Of K, V)() = CType(New Index(Of ?, ?)(level){}, Index(Of K, V)())
					For i As Integer = 1 To level
							idx = New Index(Of K, V)(z, idx, Nothing)
							idxs(i) = idx
					Next i
					Do
						h = head
						Dim oldLevel As Integer = h.level
						If level <= oldLevel Then ' lost race to add level Exit Do
						Dim newh As HeadIndex(Of K, V) = h
						Dim oldbase As Node(Of K, V) = h.node_Renamed
						For j As Integer = oldLevel+1 To level
							newh = New HeadIndex(Of K, V)(oldbase, newh, idxs(j), j)
						Next j
						If casHead(h, newh) Then
							h = newh
'JAVA TO VB CONVERTER TODO TASK: Assignments within expressions are not supported in VB
							idx = idxs(level = oldLevel)
							Exit Do
						End If
					Loop
				End If
				' find insertion points and splice in
				splice:
				Dim insertionLevel As Integer = level
				Do
					Dim j As Integer = h.level
					Dim q As Index(Of K, V) = h
					Dim r As Index(Of K, V) = q.right
					Dim t As Index(Of K, V) = idx
					Do
						If q Is Nothing OrElse t Is Nothing Then GoTo splice
						If r IsNot Nothing Then
							Dim n As Node(Of K, V) = r.node_Renamed
							' compare before deletion check avoids needing recheck
							Dim c As Integer = cpr(cmp, key, n.key)
							If n.value Is Nothing Then
								If Not q.unlink(r) Then Exit Do
								r = q.right
								Continue Do
							End If
							If c > 0 Then
								q = r
								r = r.right
								Continue Do
							End If
						End If

						If j = insertionLevel Then
							If Not q.link(r, t) Then Exit Do ' restart
							If t.node_Renamed.value Is Nothing Then
								findNode(key)
								GoTo splice
							End If
							insertionLevel -= 1
							If insertionLevel = 0 Then GoTo splice
						End If

						j -= 1
						If j >= insertionLevel AndAlso j < level Then t = t.down
						q = q.down
						r = q.right
					Loop
				Loop
			End If
			Return Nothing
		End Function

		' ---------------- Deletion -------------- 

		''' <summary>
		''' Main deletion method. Locates node, nulls value, appends a
		''' deletion marker, unlinks predecessor, removes associated index
		''' nodes, and possibly reduces head index level.
		''' 
		''' Index nodes are cleared out simply by calling findPredecessor.
		''' which unlinks indexes to deleted nodes found along path to key,
		''' which will include the indexes to this node.  This is done
		''' unconditionally. We can't check beforehand whether there are
		''' index nodes because it might be the case that some or all
		''' indexes hadn't been inserted yet for this node during initial
		''' search for it, and we'd like to ensure lack of garbage
		''' retention, so must call to be sure.
		''' </summary>
		''' <param name="key"> the key </param>
		''' <param name="value"> if non-null, the value that must be
		''' associated with key </param>
		''' <returns> the node, or null if not found </returns>
		Friend Function doRemove(ByVal key As Object, ByVal value As Object) As V
			If key Is Nothing Then Throw New NullPointerException
'JAVA TO VB CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
			Dim cmp As IComparer(Of ?) = comparator_Renamed
			outer:
			Do
				Dim b As Node(Of K, V) = findPredecessor(key, cmp)
				Dim n As Node(Of K, V) = b.next
				Do
					Dim v As Object
					Dim c As Integer
					If n Is Nothing Then GoTo outer
					Dim f As Node(Of K, V) = n.next
					If n IsNot b.next Then ' inconsistent read Exit Do
					v = n.value
					If v Is Nothing Then ' n is deleted
						n.helpDelete(b, f)
						Exit Do
					End If
					If b.value Is Nothing OrElse v Is n Then ' b is deleted Exit Do
					c = cpr(cmp, key, n.key)
					If c < 0 Then GoTo outer
					If c > 0 Then
						b = n
						n = f
						Continue Do
					End If
					If value IsNot Nothing AndAlso (Not value.Equals(v)) Then GoTo outer
					If Not n.casValue(v, Nothing) Then Exit Do
					If (Not n.appendMarker(f)) OrElse (Not b.casNext(n, f)) Then
						findNode(key) ' retry via findNode
					Else
						findPredecessor(key, cmp) ' clean index
						If head.right Is Nothing Then tryReduceLevel()
					End If
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
					Dim vv As V = CType(v, V)
					Return vv
				Loop
			Loop
			Return Nothing
		End Function

		''' <summary>
		''' Possibly reduce head level if it has no nodes.  This method can
		''' (rarely) make mistakes, in which case levels can disappear even
		''' though they are about to contain index nodes. This impacts
		''' performance, not correctness.  To minimize mistakes as well as
		''' to reduce hysteresis, the level is reduced by one only if the
		''' topmost three levels look empty. Also, if the removed level
		''' looks non-empty after CAS, we try to change it back quick
		''' before anyone notices our mistake! (This trick works pretty
		''' well because this method will practically never make mistakes
		''' unless current thread stalls immediately before first CAS, in
		''' which case it is very unlikely to stall again immediately
		''' afterwards, so will recover.)
		''' 
		''' We put up with all this rather than just let levels grow
		''' because otherwise, even a small map that has undergone a large
		''' number of insertions and removals will have a lot of levels,
		''' slowing down access more than would an occasional unwanted
		''' reduction.
		''' </summary>
		Private Sub tryReduceLevel()
			Dim h As HeadIndex(Of K, V) = head
			Dim d As HeadIndex(Of K, V)
			Dim e As HeadIndex(Of K, V)
			d = CType(h.down, HeadIndex(Of K, V))
			e = CType(d.down, HeadIndex(Of K, V))
			If h.level > 3 AndAlso d IsNot Nothing AndAlso e IsNot Nothing AndAlso e.right Is Nothing AndAlso d.right Is Nothing AndAlso h.right Is Nothing AndAlso casHead(h, d) AndAlso h.right IsNot Nothing Then ' recheck -  try to set casHead(d, h) ' try to backout
		End Sub

		' ---------------- Finding and removing first element -------------- 

		''' <summary>
		''' Specialized variant of findNode to get first valid node. </summary>
		''' <returns> first node or null if empty </returns>
		Friend Function findFirst() As Node(Of K, V)
			Dim b As Node(Of K, V)
			n
			Do
'JAVA TO VB CONVERTER TODO TASK: Assignments within expressions are not supported in VB
				n = (b = head.node_Renamed).next
				If n Is Nothing Then Return Nothing
				If n.value IsNot Nothing Then Return n
				n.helpDelete(b, n.next)
			Loop
		End Function

		''' <summary>
		''' Removes first entry; returns its snapshot. </summary>
		''' <returns> null if empty, else snapshot of first entry </returns>
		Private Function doRemoveFirstEntry() As KeyValuePair(Of K, V)
			Dim b As Node(Of K, V)
			n
			Do
'JAVA TO VB CONVERTER TODO TASK: Assignments within expressions are not supported in VB
				n = (b = head.node_Renamed).next
				If n Is Nothing Then Return Nothing
				Dim f As Node(Of K, V) = n.next
				If n IsNot b.next Then Continue Do
				Dim v As Object = n.value
				If v Is Nothing Then
					n.helpDelete(b, f)
					Continue Do
				End If
				If Not n.casValue(v, Nothing) Then Continue Do
				If (Not n.appendMarker(f)) OrElse (Not b.casNext(n, f)) Then findFirst() ' retry
				clearIndexToFirst()
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
				Dim vv As V = CType(v, V)
				Return New java.util.AbstractMap.SimpleImmutableEntry(Of K, V)(n.key, vv)
			Loop
		End Function

		''' <summary>
		''' Clears out index nodes associated with deleted first entry.
		''' </summary>
		Private Sub clearIndexToFirst()
			Do
				Dim q As Index(Of K, V) = head
				Do
					Dim r As Index(Of K, V) = q.right
					If r IsNot Nothing AndAlso r.indexesDeletedNode() AndAlso (Not q.unlink(r)) Then Exit Do
					q = q.down
					If q Is Nothing Then
						If head.right Is Nothing Then tryReduceLevel()
						Return
					End If
				Loop
			Loop
		End Sub

		''' <summary>
		''' Removes last entry; returns its snapshot.
		''' Specialized variant of doRemove. </summary>
		''' <returns> null if empty, else snapshot of last entry </returns>
		Private Function doRemoveLastEntry() As KeyValuePair(Of K, V)
			Do
				Dim b As Node(Of K, V) = findPredecessorOfLast()
				Dim n As Node(Of K, V) = b.next
				If n Is Nothing Then
					If b.baseHeader Then ' empty
						Return Nothing
					Else
						Continue Do ' all b's successors are deleted; retry
					End If
				End If
				Do
					Dim f As Node(Of K, V) = n.next
					If n IsNot b.next Then ' inconsistent read Exit Do
					Dim v As Object = n.value
					If v Is Nothing Then ' n is deleted
						n.helpDelete(b, f)
						Exit Do
					End If
					If b.value Is Nothing OrElse v Is n Then ' b is deleted Exit Do
					If f IsNot Nothing Then
						b = n
						n = f
						Continue Do
					End If
					If Not n.casValue(v, Nothing) Then Exit Do
					Dim key As K = n.key
					If (Not n.appendMarker(f)) OrElse (Not b.casNext(n, f)) Then
						findNode(key) ' retry via findNode
					Else ' clean index
						findPredecessor(key, comparator_Renamed)
						If head.right Is Nothing Then tryReduceLevel()
					End If
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
					Dim vv As V = CType(v, V)
					Return New java.util.AbstractMap.SimpleImmutableEntry(Of K, V)(key, vv)
				Loop
			Loop
		End Function

		' ---------------- Finding and removing last element -------------- 

		''' <summary>
		''' Specialized version of find to get last valid node. </summary>
		''' <returns> last node or null if empty </returns>
		Friend Function findLast() As Node(Of K, V)
	'        
	'         * findPredecessor can't be used to traverse index level
	'         * because this doesn't use comparisons.  So traversals of
	'         * both levels are folded together.
	'         
			Dim q As Index(Of K, V) = head
			Do
				Dim d As Index(Of K, V), r As Index(Of K, V)
				r = q.right
				If r IsNot Nothing Then
					If r.indexesDeletedNode() Then
						q.unlink(r)
						q = head ' restart
					Else
						q = r
					End If
				Else
					d = q.down
					If d IsNot Nothing Then
						q = d
					Else
						Dim b As Node(Of K, V) = q.node_Renamed
						Dim n As Node(Of K, V) = b.next
						Do
					End If
						If n Is Nothing Then Return If(b.baseHeader, Nothing, b)
						Dim f As Node(Of K, V) = n.next ' inconsistent read
						If n IsNot b.next Then Exit Do
						Dim v As Object = n.value
						If v Is Nothing Then ' n is deleted
							n.helpDelete(b, f)
							Exit Do
						End If
						If b.value Is Nothing OrElse v Is n Then ' b is deleted Exit Do
						b = n
						n = f
						Loop
					q = head ' restart
					End If
			Loop
		End Function

		''' <summary>
		''' Specialized variant of findPredecessor to get predecessor of last
		''' valid node.  Needed when removing the last entry.  It is possible
		''' that all successors of returned node will have been deleted upon
		''' return, in which case this method can be retried. </summary>
		''' <returns> likely predecessor of last node </returns>
		Private Function findPredecessorOfLast() As Node(Of K, V)
			Do
				Dim q As Index(Of K, V) = head
				Do
					Dim d As Index(Of K, V), r As Index(Of K, V)
					r = q.right
					If r IsNot Nothing Then
						If r.indexesDeletedNode() Then
							q.unlink(r)
							Exit Do ' must restart
						End If
						' proceed as far across as possible without overshooting
						If r.node_Renamed.next IsNot Nothing Then
							q = r
							Continue Do
						End If
					End If
					d = q.down
					If d IsNot Nothing Then
						q = d
					Else
						Return q.node_Renamed
					End If
				Loop
			Loop
		End Function

		' ---------------- Relational operations -------------- 

		' Control values OR'ed as arguments to findNear

		Private Const EQ As Integer = 1
		Private Const LT As Integer = 2
		Private Const GT As Integer = 0 ' Actually checked as !LT

		''' <summary>
		''' Utility for ceiling, floor, lower, higher methods. </summary>
		''' <param name="key"> the key </param>
		''' <param name="rel"> the relation -- OR'ed combination of EQ, LT, GT </param>
		''' <returns> nearest node fitting relation, or null if no such </returns>
'JAVA TO VB CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
		Friend Function findNear(Of T1)(ByVal key As K, ByVal rel As Integer, ByVal cmp As IComparer(Of T1)) As Node(Of K, V)
			If key Is Nothing Then Throw New NullPointerException
			Do
				Dim b As Node(Of K, V) = findPredecessor(key, cmp)
				Dim n As Node(Of K, V) = b.next
				Do
					Dim v As Object
					If n Is Nothing Then Return If((rel And LT) = 0 OrElse b.baseHeader, Nothing, b)
					Dim f As Node(Of K, V) = n.next
					If n IsNot b.next Then ' inconsistent read Exit Do
					v = n.value
					If v Is Nothing Then ' n is deleted
						n.helpDelete(b, f)
						Exit Do
					End If
					If b.value Is Nothing OrElse v Is n Then ' b is deleted Exit Do
					Dim c As Integer = cpr(cmp, key, n.key)
					If (c = 0 AndAlso (rel And EQ) <> 0) OrElse (c < 0 AndAlso (rel And LT) = 0) Then Return n
					If c <= 0 AndAlso (rel And LT) <> 0 Then Return If(b.baseHeader, Nothing, b)
					b = n
					n = f
				Loop
			Loop
		End Function

		''' <summary>
		''' Returns SimpleImmutableEntry for results of findNear. </summary>
		''' <param name="key"> the key </param>
		''' <param name="rel"> the relation -- OR'ed combination of EQ, LT, GT </param>
		''' <returns> Entry fitting relation, or null if no such </returns>
		Friend Function getNear(ByVal key As K, ByVal rel As Integer) As java.util.AbstractMap.SimpleImmutableEntry(Of K, V)
'JAVA TO VB CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
			Dim cmp As IComparer(Of ?) = comparator_Renamed
			Do
				Dim n As Node(Of K, V) = findNear(key, rel, cmp)
				If n Is Nothing Then Return Nothing
				Dim e As java.util.AbstractMap.SimpleImmutableEntry(Of K, V) = n.createSnapshot()
				If e IsNot Nothing Then Return e
			Loop
		End Function

		' ---------------- Constructors -------------- 

		''' <summary>
		''' Constructs a new, empty map, sorted according to the
		''' <seealso cref="Comparable natural ordering"/> of the keys.
		''' </summary>
		Public Sub New()
			Me.comparator_Renamed = Nothing
			initialize()
		End Sub

		''' <summary>
		''' Constructs a new, empty map, sorted according to the specified
		''' comparator.
		''' </summary>
		''' <param name="comparator"> the comparator that will be used to order this map.
		'''        If {@code null}, the {@link Comparable natural
		'''        ordering} of the keys will be used. </param>
'JAVA TO VB CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
		Public Sub New(Of T1)(ByVal comparator As IComparer(Of T1))
			Me.comparator_Renamed = comparator
			initialize()
		End Sub

		''' <summary>
		''' Constructs a new map containing the same mappings as the given map,
		''' sorted according to the <seealso cref="Comparable natural ordering"/> of
		''' the keys.
		''' </summary>
		''' <param name="m"> the map whose mappings are to be placed in this map </param>
		''' <exception cref="ClassCastException"> if the keys in {@code m} are not
		'''         <seealso cref="Comparable"/>, or are not mutually comparable </exception>
		''' <exception cref="NullPointerException"> if the specified map or any of its keys
		'''         or values are null </exception>
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
		Public Sub New(Of T1 As K, ? As V)(ByVal m As IDictionary(Of T1))
			Me.comparator_Renamed = Nothing
			initialize()
			putAll(m)
		End Sub

		''' <summary>
		''' Constructs a new map containing the same mappings and using the
		''' same ordering as the specified sorted map.
		''' </summary>
		''' <param name="m"> the sorted map whose mappings are to be placed in this
		'''        map, and whose comparator is to be used to sort this map </param>
		''' <exception cref="NullPointerException"> if the specified sorted map or any of
		'''         its keys or values are null </exception>
		Public Sub New(Of T1 As V)(ByVal m As java.util.SortedMap(Of T1))
			Me.comparator_Renamed = m.comparator()
			initialize()
			buildFromSorted(m)
		End Sub

		''' <summary>
		''' Returns a shallow copy of this {@code ConcurrentSkipListMap}
		''' instance. (The keys and values themselves are not cloned.)
		''' </summary>
		''' <returns> a shallow copy of this map </returns>
		Public Overridable Function clone() As ConcurrentSkipListMap(Of K, V)
			Try
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
				Dim clone_Renamed As ConcurrentSkipListMap(Of K, V) = CType(MyBase.clone(), ConcurrentSkipListMap(Of K, V))
				clone_Renamed.initialize()
				clone_Renamed.buildFromSorted(Me)
				Return clone_Renamed
			Catch e As CloneNotSupportedException
				Throw New InternalError
			End Try
		End Function

		''' <summary>
		''' Streamlined bulk insertion to initialize from elements of
		''' given sorted map.  Call only from constructor or clone
		''' method.
		''' </summary>
		Private Sub buildFromSorted(Of T1 As V)(ByVal map As java.util.SortedMap(Of T1))
			If map Is Nothing Then Throw New NullPointerException

			Dim h As HeadIndex(Of K, V) = head
			Dim basepred As Node(Of K, V) = h.node_Renamed

			' Track the current rightmost node at each level. Uses an
			' ArrayList to avoid committing to initial or maximum level.
			Dim preds As New List(Of Index(Of K, V))

			' initialize
			For i As Integer = 0 To h.level
				preds.add(Nothing)
			Next i
			Dim q As Index(Of K, V) = h
			For i As Integer = h.level To 1 Step -1
				preds.set(i, q)
				q = q.down
			Next i

'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
			Dim it As IEnumerator(Of ? As KeyValuePair(Of ? As K, ? As V)) = map.entrySet().GetEnumerator()
			Do While it.MoveNext()
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
				Dim e As KeyValuePair(Of ? As K, ? As V) = it.Current
				Dim rnd As Integer = ThreadLocalRandom.current().Next()
				Dim j As Integer = 0
				If (rnd And &H80000001L) = 0 Then
					Do
						j += 1
					Loop While ((rnd >>>= 1) And 1) <> 0
					If j > h.level Then j = h.level + 1
				End If
				Dim k As K = e.Key
				Dim v As V = e.Value
				If k Is Nothing OrElse v Is Nothing Then Throw New NullPointerException
				Dim z As New Node(Of K, V)(k, v, Nothing)
				basepred.next = z
				basepred = z
				If j > 0 Then
					Dim idx As Index(Of K, V) = Nothing
					For i As Integer = 1 To j
						idx = New Index(Of K, V)(z, idx, Nothing)
						If i > h.level Then h = New HeadIndex(Of K, V)(h.node_Renamed, h, idx, i)

						If i < preds.size() Then
							preds.get(i).right = idx
							preds.set(i, idx)
						Else
							preds.add(idx)
						End If
					Next i
				End If
			Loop
			head = h
		End Sub

		' ---------------- Serialization -------------- 

		''' <summary>
		''' Saves this map to a stream (that is, serializes it).
		''' </summary>
		''' <param name="s"> the stream </param>
		''' <exception cref="java.io.IOException"> if an I/O error occurs
		''' @serialData The key (Object) and value (Object) for each
		''' key-value mapping represented by the map, followed by
		''' {@code null}. The key-value mappings are emitted in key-order
		''' (as determined by the Comparator, or by the keys' natural
		''' ordering if no Comparator). </exception>
		Private Sub writeObject(ByVal s As java.io.ObjectOutputStream)
			' Write out the Comparator and any hidden stuff
			s.defaultWriteObject()

			' Write out keys and values (alternating)
			Dim n As Node(Of K, V) = findFirst()
			Do While n IsNot Nothing
				Dim v As V = n.validValue
				If v IsNot Nothing Then
					s.writeObject(n.key)
					s.writeObject(v)
				End If
				n = n.next
			Loop
			s.writeObject(Nothing)
		End Sub

		''' <summary>
		''' Reconstitutes this map from a stream (that is, deserializes it). </summary>
		''' <param name="s"> the stream </param>
		''' <exception cref="ClassNotFoundException"> if the class of a serialized object
		'''         could not be found </exception>
		''' <exception cref="java.io.IOException"> if an I/O error occurs </exception>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
		Private Sub readObject(ByVal s As java.io.ObjectInputStream)
			' Read in the Comparator and any hidden stuff
			s.defaultReadObject()
			' Reset transients
			initialize()

	'        
	'         * This is nearly identical to buildFromSorted, but is
	'         * distinct because readObject calls can't be nicely adapted
	'         * as the kind of iterator needed by buildFromSorted. (They
	'         * can be, but doing so requires type cheats and/or creation
	'         * of adaptor classes.) It is simpler to just adapt the code.
	'         

			Dim h As HeadIndex(Of K, V) = head
			Dim basepred As Node(Of K, V) = h.node_Renamed
			Dim preds As New List(Of Index(Of K, V))
			For i As Integer = 0 To h.level
				preds.add(Nothing)
			Next i
			Dim q As Index(Of K, V) = h
			For i As Integer = h.level To 1 Step -1
				preds.set(i, q)
				q = q.down
			Next i

			Do
				Dim k As Object = s.readObject()
				If k Is Nothing Then Exit Do
				Dim v As Object = s.readObject()
				If v Is Nothing Then Throw New NullPointerException
				Dim key As K = CType(k, K)
				Dim val As V = CType(v, V)
				Dim rnd As Integer = ThreadLocalRandom.current().Next()
				Dim j As Integer = 0
				If (rnd And &H80000001L) = 0 Then
					Do
						j += 1
					Loop While ((rnd >>>= 1) And 1) <> 0
					If j > h.level Then j = h.level + 1
				End If
				Dim z As New Node(Of K, V)(key, val, Nothing)
				basepred.next = z
				basepred = z
				If j > 0 Then
					Dim idx As Index(Of K, V) = Nothing
					For i As Integer = 1 To j
						idx = New Index(Of K, V)(z, idx, Nothing)
						If i > h.level Then h = New HeadIndex(Of K, V)(h.node_Renamed, h, idx, i)

						If i < preds.size() Then
							preds.get(i).right = idx
							preds.set(i, idx)
						Else
							preds.add(idx)
						End If
					Next i
				End If
			Loop
			head = h
		End Sub

		' ------ Map API methods ------ 

		''' <summary>
		''' Returns {@code true} if this map contains a mapping for the specified
		''' key.
		''' </summary>
		''' <param name="key"> key whose presence in this map is to be tested </param>
		''' <returns> {@code true} if this map contains a mapping for the specified key </returns>
		''' <exception cref="ClassCastException"> if the specified key cannot be compared
		'''         with the keys currently in the map </exception>
		''' <exception cref="NullPointerException"> if the specified key is null </exception>
		Public Overridable Function containsKey(ByVal key As Object) As Boolean
			Return doGet(key) IsNot Nothing
		End Function

		''' <summary>
		''' Returns the value to which the specified key is mapped,
		''' or {@code null} if this map contains no mapping for the key.
		''' 
		''' <p>More formally, if this map contains a mapping from a key
		''' {@code k} to a value {@code v} such that {@code key} compares
		''' equal to {@code k} according to the map's ordering, then this
		''' method returns {@code v}; otherwise it returns {@code null}.
		''' (There can be at most one such mapping.)
		''' </summary>
		''' <exception cref="ClassCastException"> if the specified key cannot be compared
		'''         with the keys currently in the map </exception>
		''' <exception cref="NullPointerException"> if the specified key is null </exception>
		Public Overridable Function [get](ByVal key As Object) As V
			Return doGet(key)
		End Function

		''' <summary>
		''' Returns the value to which the specified key is mapped,
		''' or the given defaultValue if this map contains no mapping for the key.
		''' </summary>
		''' <param name="key"> the key </param>
		''' <param name="defaultValue"> the value to return if this map contains
		''' no mapping for the given key </param>
		''' <returns> the mapping for the key, if present; else the defaultValue </returns>
		''' <exception cref="NullPointerException"> if the specified key is null
		''' @since 1.8 </exception>
		Public Overridable Function getOrDefault(ByVal key As Object, ByVal defaultValue As V) As V
			Dim v As V
'JAVA TO VB CONVERTER TODO TASK: Assignments within expressions are not supported in VB
			Return If((v = doGet(key)) Is Nothing, defaultValue, v)
		End Function

		''' <summary>
		''' Associates the specified value with the specified key in this map.
		''' If the map previously contained a mapping for the key, the old
		''' value is replaced.
		''' </summary>
		''' <param name="key"> key with which the specified value is to be associated </param>
		''' <param name="value"> value to be associated with the specified key </param>
		''' <returns> the previous value associated with the specified key, or
		'''         {@code null} if there was no mapping for the key </returns>
		''' <exception cref="ClassCastException"> if the specified key cannot be compared
		'''         with the keys currently in the map </exception>
		''' <exception cref="NullPointerException"> if the specified key or value is null </exception>
		Public Overridable Function put(ByVal key As K, ByVal value As V) As V
			If value Is Nothing Then Throw New NullPointerException
			Return doPut(key, value, False)
		End Function

		''' <summary>
		''' Removes the mapping for the specified key from this map if present.
		''' </summary>
		''' <param name="key"> key for which mapping should be removed </param>
		''' <returns> the previous value associated with the specified key, or
		'''         {@code null} if there was no mapping for the key </returns>
		''' <exception cref="ClassCastException"> if the specified key cannot be compared
		'''         with the keys currently in the map </exception>
		''' <exception cref="NullPointerException"> if the specified key is null </exception>
		Public Overridable Function remove(ByVal key As Object) As V
			Return doRemove(key, Nothing)
		End Function

		''' <summary>
		''' Returns {@code true} if this map maps one or more keys to the
		''' specified value.  This operation requires time linear in the
		''' map size. Additionally, it is possible for the map to change
		''' during execution of this method, in which case the returned
		''' result may be inaccurate.
		''' </summary>
		''' <param name="value"> value whose presence in this map is to be tested </param>
		''' <returns> {@code true} if a mapping to {@code value} exists;
		'''         {@code false} otherwise </returns>
		''' <exception cref="NullPointerException"> if the specified value is null </exception>
		Public Overridable Function containsValue(ByVal value As Object) As Boolean
			If value Is Nothing Then Throw New NullPointerException
			Dim n As Node(Of K, V) = findFirst()
			Do While n IsNot Nothing
				Dim v As V = n.validValue
				If v IsNot Nothing AndAlso value.Equals(v) Then Return True
				n = n.next
			Loop
			Return False
		End Function

		''' <summary>
		''' Returns the number of key-value mappings in this map.  If this map
		''' contains more than {@code  java.lang.[Integer].MAX_VALUE} elements, it
		''' returns {@code  java.lang.[Integer].MAX_VALUE}.
		''' 
		''' <p>Beware that, unlike in most collections, this method is
		''' <em>NOT</em> a constant-time operation. Because of the
		''' asynchronous nature of these maps, determining the current
		''' number of elements requires traversing them all to count them.
		''' Additionally, it is possible for the size to change during
		''' execution of this method, in which case the returned result
		''' will be inaccurate. Thus, this method is typically not very
		''' useful in concurrent applications.
		''' </summary>
		''' <returns> the number of elements in this map </returns>
		Public Overridable Function size() As Integer
			Dim count As Long = 0
			Dim n As Node(Of K, V) = findFirst()
			Do While n IsNot Nothing
				If n.validValue IsNot Nothing Then count += 1
				n = n.next
			Loop
			Return If(count >=  java.lang.[Integer].Max_Value,  java.lang.[Integer].Max_Value, CInt(count))
		End Function

		''' <summary>
		''' Returns {@code true} if this map contains no key-value mappings. </summary>
		''' <returns> {@code true} if this map contains no key-value mappings </returns>
		Public Overridable Property empty As Boolean
			Get
				Return findFirst() Is Nothing
			End Get
		End Property

		''' <summary>
		''' Removes all of the mappings from this map.
		''' </summary>
		Public Overridable Sub clear()
			initialize()
		End Sub

		''' <summary>
		''' If the specified key is not already associated with a value,
		''' attempts to compute its value using the given mapping function
		''' and enters it into this map unless {@code null}.  The function
		''' is <em>NOT</em> guaranteed to be applied once atomically only
		''' if the value is not present.
		''' </summary>
		''' <param name="key"> key with which the specified value is to be associated </param>
		''' <param name="mappingFunction"> the function to compute a value </param>
		''' <returns> the current (existing or computed) value associated with
		'''         the specified key, or null if the computed value is null </returns>
		''' <exception cref="NullPointerException"> if the specified key is null
		'''         or the mappingFunction is null
		''' @since 1.8 </exception>
'JAVA TO VB CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
		Public Overridable Function computeIfAbsent(Of T1 As V)(ByVal key As K, ByVal mappingFunction As java.util.function.Function(Of T1)) As V
			If key Is Nothing OrElse mappingFunction Is Nothing Then Throw New NullPointerException
			Dim v, p, r As V
			v = doGet(key)
			r = mappingFunction.apply(key)
			If v Is Nothing AndAlso r IsNot Nothing Then v = If((p = doPut(key, r, True)) Is Nothing, r, p)
			Return v
		End Function

		''' <summary>
		''' If the value for the specified key is present, attempts to
		''' compute a new mapping given the key and its current mapped
		''' value. The function is <em>NOT</em> guaranteed to be applied
		''' once atomically.
		''' </summary>
		''' <param name="key"> key with which a value may be associated </param>
		''' <param name="remappingFunction"> the function to compute a value </param>
		''' <returns> the new value associated with the specified key, or null if none </returns>
		''' <exception cref="NullPointerException"> if the specified key is null
		'''         or the remappingFunction is null
		''' @since 1.8 </exception>
'JAVA TO VB CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
		Public Overridable Function computeIfPresent(Of T1 As V)(ByVal key As K, ByVal remappingFunction As java.util.function.BiFunction(Of T1)) As V
			If key Is Nothing OrElse remappingFunction Is Nothing Then Throw New NullPointerException
			Dim n As Node(Of K, V)
			Dim v As Object
			n = findNode(key)
			Do While n IsNot Nothing
				v = n.value
				If v IsNot Nothing Then
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
					Dim vv As V = CType(v, V)
					Dim r As V = remappingFunction.apply(key, vv)
					If r IsNot Nothing Then
						If n.casValue(vv, r) Then Return r
					ElseIf doRemove(key, vv) IsNot Nothing Then
						Exit Do
					End If
				End If
				n = findNode(key)
			Loop
			Return Nothing
		End Function

		''' <summary>
		''' Attempts to compute a mapping for the specified key and its
		''' current mapped value (or {@code null} if there is no current
		''' mapping). The function is <em>NOT</em> guaranteed to be applied
		''' once atomically.
		''' </summary>
		''' <param name="key"> key with which the specified value is to be associated </param>
		''' <param name="remappingFunction"> the function to compute a value </param>
		''' <returns> the new value associated with the specified key, or null if none </returns>
		''' <exception cref="NullPointerException"> if the specified key is null
		'''         or the remappingFunction is null
		''' @since 1.8 </exception>
'JAVA TO VB CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
		Public Overridable Function compute(Of T1 As V)(ByVal key As K, ByVal remappingFunction As java.util.function.BiFunction(Of T1)) As V
			If key Is Nothing OrElse remappingFunction Is Nothing Then Throw New NullPointerException
			Do
				Dim n As Node(Of K, V)
				Dim v As Object
				Dim r As V
				n = findNode(key)
				If n Is Nothing Then
					r = remappingFunction.apply(key, Nothing)
					If r Is Nothing Then Exit Do
					If doPut(key, r, True) Is Nothing Then Return r
				Else
					v = n.value
					If v IsNot Nothing Then
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
						Dim vv As V = CType(v, V)
						r = remappingFunction.apply(key, vv)
						If r IsNot Nothing Then
							If n.casValue(vv, r) Then Return r
						ElseIf doRemove(key, vv) IsNot Nothing Then
							Exit Do
						End If
					End If
					End If
			Loop
			Return Nothing
		End Function

		''' <summary>
		''' If the specified key is not already associated with a value,
		''' associates it with the given value.  Otherwise, replaces the
		''' value with the results of the given remapping function, or
		''' removes if {@code null}. The function is <em>NOT</em>
		''' guaranteed to be applied once atomically.
		''' </summary>
		''' <param name="key"> key with which the specified value is to be associated </param>
		''' <param name="value"> the value to use if absent </param>
		''' <param name="remappingFunction"> the function to recompute a value if present </param>
		''' <returns> the new value associated with the specified key, or null if none </returns>
		''' <exception cref="NullPointerException"> if the specified key or value is null
		'''         or the remappingFunction is null
		''' @since 1.8 </exception>
'JAVA TO VB CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
		Public Overridable Function merge(Of T1 As V)(ByVal key As K, ByVal value As V, ByVal remappingFunction As java.util.function.BiFunction(Of T1)) As V
			If key Is Nothing OrElse value Is Nothing OrElse remappingFunction Is Nothing Then Throw New NullPointerException
			Do
				Dim n As Node(Of K, V)
				Dim v As Object
				Dim r As V
				n = findNode(key)
				If n Is Nothing Then
					If doPut(key, value, True) Is Nothing Then Return value
				Else
					v = n.value
					If v IsNot Nothing Then
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
						Dim vv As V = CType(v, V)
						r = remappingFunction.apply(vv, value)
						If r IsNot Nothing Then
							If n.casValue(vv, r) Then Return r
						ElseIf doRemove(key, vv) IsNot Nothing Then
							Return Nothing
						End If
					End If
					End If
			Loop
		End Function

		' ---------------- View methods -------------- 

	'    
	'     * Note: Lazy initialization works for views because view classes
	'     * are stateless/immutable so it doesn't matter wrt correctness if
	'     * more than one is created (which will only rarely happen).  Even
	'     * so, the following idiom conservatively ensures that the method
	'     * returns the one it created if it does so, not one created by
	'     * another racing thread.
	'     

		''' <summary>
		''' Returns a <seealso cref="NavigableSet"/> view of the keys contained in this map.
		''' 
		''' <p>The set's iterator returns the keys in ascending order.
		''' The set's spliterator additionally reports <seealso cref="Spliterator#CONCURRENT"/>,
		''' <seealso cref="Spliterator#NONNULL"/>, <seealso cref="Spliterator#SORTED"/> and
		''' <seealso cref="Spliterator#ORDERED"/>, with an encounter order that is ascending
		''' key order.  The spliterator's comparator (see
		''' <seealso cref="java.util.Spliterator#getComparator()"/>) is {@code null} if
		''' the map's comparator (see <seealso cref="#comparator()"/>) is {@code null}.
		''' Otherwise, the spliterator's comparator is the same as or imposes the
		''' same total ordering as the map's comparator.
		''' 
		''' <p>The set is backed by the map, so changes to the map are
		''' reflected in the set, and vice-versa.  The set supports element
		''' removal, which removes the corresponding mapping from the map,
		''' via the {@code Iterator.remove}, {@code Set.remove},
		''' {@code removeAll}, {@code retainAll}, and {@code clear}
		''' operations.  It does not support the {@code add} or {@code addAll}
		''' operations.
		''' 
		''' <p>The view's iterators and spliterators are
		''' <a href="package-summary.html#Weakly"><i>weakly consistent</i></a>.
		''' 
		''' <p>This method is equivalent to method {@code navigableKeySet}.
		''' </summary>
		''' <returns> a navigable set view of the keys in this map </returns>
		Public Overridable Function keySet() As java.util.NavigableSet(Of K)
			Dim ks As KeySet(Of K) = keySet_Renamed
				If ks IsNot Nothing Then
					Return ks
				Else
'JAVA TO VB CONVERTER TODO TASK: Assignments within expressions are not supported in VB
					Return (keySet_Renamed = New KeySet(Of K)(Me))
				End If
		End Function

		Public Overridable Function navigableKeySet() As java.util.NavigableSet(Of K)
			Dim ks As KeySet(Of K) = keySet_Renamed
				If ks IsNot Nothing Then
					Return ks
				Else
'JAVA TO VB CONVERTER TODO TASK: Assignments within expressions are not supported in VB
					Return (keySet_Renamed = New KeySet(Of K)(Me))
				End If
		End Function

		''' <summary>
		''' Returns a <seealso cref="Collection"/> view of the values contained in this map.
		''' <p>The collection's iterator returns the values in ascending order
		''' of the corresponding keys. The collections's spliterator additionally
		''' reports <seealso cref="Spliterator#CONCURRENT"/>, <seealso cref="Spliterator#NONNULL"/> and
		''' <seealso cref="Spliterator#ORDERED"/>, with an encounter order that is ascending
		''' order of the corresponding keys.
		''' 
		''' <p>The collection is backed by the map, so changes to the map are
		''' reflected in the collection, and vice-versa.  The collection
		''' supports element removal, which removes the corresponding
		''' mapping from the map, via the {@code Iterator.remove},
		''' {@code Collection.remove}, {@code removeAll},
		''' {@code retainAll} and {@code clear} operations.  It does not
		''' support the {@code add} or {@code addAll} operations.
		''' 
		''' <p>The view's iterators and spliterators are
		''' <a href="package-summary.html#Weakly"><i>weakly consistent</i></a>.
		''' </summary>
		Public Overridable Function values() As ICollection(Of V)
			Dim vs As Values(Of V) = values_Renamed
				If vs IsNot Nothing Then
					Return vs
				Else
'JAVA TO VB CONVERTER TODO TASK: Assignments within expressions are not supported in VB
					Return (values_Renamed = New Values(Of V)(Me))
				End If
		End Function

		''' <summary>
		''' Returns a <seealso cref="Set"/> view of the mappings contained in this map.
		''' 
		''' <p>The set's iterator returns the entries in ascending key order.  The
		''' set's spliterator additionally reports <seealso cref="Spliterator#CONCURRENT"/>,
		''' <seealso cref="Spliterator#NONNULL"/>, <seealso cref="Spliterator#SORTED"/> and
		''' <seealso cref="Spliterator#ORDERED"/>, with an encounter order that is ascending
		''' key order.
		''' 
		''' <p>The set is backed by the map, so changes to the map are
		''' reflected in the set, and vice-versa.  The set supports element
		''' removal, which removes the corresponding mapping from the map,
		''' via the {@code Iterator.remove}, {@code Set.remove},
		''' {@code removeAll}, {@code retainAll} and {@code clear}
		''' operations.  It does not support the {@code add} or
		''' {@code addAll} operations.
		''' 
		''' <p>The view's iterators and spliterators are
		''' <a href="package-summary.html#Weakly"><i>weakly consistent</i></a>.
		''' 
		''' <p>The {@code Map.Entry} elements traversed by the {@code iterator}
		''' or {@code spliterator} do <em>not</em> support the {@code setValue}
		''' operation.
		''' </summary>
		''' <returns> a set view of the mappings contained in this map,
		'''         sorted in ascending key order </returns>
		Public Overridable Function entrySet() As java.util.Set(Of KeyValuePair(Of K, V))
			Dim es As EntrySet(Of K, V) = entrySet_Renamed
				If es IsNot Nothing Then
					Return es
				Else
'JAVA TO VB CONVERTER TODO TASK: Assignments within expressions are not supported in VB
					Return (entrySet_Renamed = New EntrySet(Of K, V)(Me))
				End If
		End Function

		Public Overridable Function descendingMap() As java.util.concurrent.ConcurrentNavigableMap(Of K, V)
			Dim dm As java.util.concurrent.ConcurrentNavigableMap(Of K, V) = descendingMap_Renamed
				If dm IsNot Nothing Then
					Return dm
				Else
'JAVA TO VB CONVERTER TODO TASK: Assignments within expressions are not supported in VB
					Return (descendingMap_Renamed = New SubMap(Of K, V) (Me, Nothing, False, Nothing, False, True))
				End If
		End Function

		Public Overridable Function descendingKeySet() As java.util.NavigableSet(Of K)
			Return descendingMap().navigableKeySet()
		End Function

		' ---------------- AbstractMap Overrides -------------- 

		''' <summary>
		''' Compares the specified object with this map for equality.
		''' Returns {@code true} if the given object is also a map and the
		''' two maps represent the same mappings.  More formally, two maps
		''' {@code m1} and {@code m2} represent the same mappings if
		''' {@code m1.entrySet().equals(m2.entrySet())}.  This
		''' operation may return misleading results if either map is
		''' concurrently modified during execution of this method.
		''' </summary>
		''' <param name="o"> object to be compared for equality with this map </param>
		''' <returns> {@code true} if the specified object is equal to this map </returns>
		Public Overrides Function Equals(ByVal o As Object) As Boolean
			If o Is Me Then Return True
			If Not(TypeOf o Is IDictionary) Then Return False
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
			Dim m As IDictionary(Of ?, ?) = CType(o, IDictionary(Of ?, ?))
			Try
				For Each e As KeyValuePair(Of K, V) In Me.entrySet()
					If Not e.Value.Equals(m(e.Key)) Then Return False
				Next e
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
				For Each e As KeyValuePair(Of ?, ?) In m
					Dim k As Object = e.Key
					Dim v As Object = e.Value
					If k Is Nothing OrElse v Is Nothing OrElse (Not v.Equals([get](k))) Then Return False
				Next e
				Return True
			Catch unused As  [Class]CastException
				Return False
			Catch unused As NullPointerException
				Return False
			End Try
		End Function

		' ------ ConcurrentMap API methods ------ 

		''' <summary>
		''' {@inheritDoc}
		''' </summary>
		''' <returns> the previous value associated with the specified key,
		'''         or {@code null} if there was no mapping for the key </returns>
		''' <exception cref="ClassCastException"> if the specified key cannot be compared
		'''         with the keys currently in the map </exception>
		''' <exception cref="NullPointerException"> if the specified key or value is null </exception>
		Public Overridable Function putIfAbsent(ByVal key As K, ByVal value As V) As V
			If value Is Nothing Then Throw New NullPointerException
			Return doPut(key, value, True)
		End Function

		''' <summary>
		''' {@inheritDoc}
		''' </summary>
		''' <exception cref="ClassCastException"> if the specified key cannot be compared
		'''         with the keys currently in the map </exception>
		''' <exception cref="NullPointerException"> if the specified key is null </exception>
		Public Overridable Function remove(ByVal key As Object, ByVal value As Object) As Boolean
			If key Is Nothing Then Throw New NullPointerException
			Return value IsNot Nothing AndAlso doRemove(key, value) IsNot Nothing
		End Function

		''' <summary>
		''' {@inheritDoc}
		''' </summary>
		''' <exception cref="ClassCastException"> if the specified key cannot be compared
		'''         with the keys currently in the map </exception>
		''' <exception cref="NullPointerException"> if any of the arguments are null </exception>
		Public Overridable Function replace(ByVal key As K, ByVal oldValue As V, ByVal newValue As V) As Boolean
			If key Is Nothing OrElse oldValue Is Nothing OrElse newValue Is Nothing Then Throw New NullPointerException
			Do
				Dim n As Node(Of K, V)
				Dim v As Object
				n = findNode(key)
				If n Is Nothing Then Return False
				v = n.value
				If v IsNot Nothing Then
					If Not oldValue.Equals(v) Then Return False
					If n.casValue(v, newValue) Then Return True
				End If
			Loop
		End Function

		''' <summary>
		''' {@inheritDoc}
		''' </summary>
		''' <returns> the previous value associated with the specified key,
		'''         or {@code null} if there was no mapping for the key </returns>
		''' <exception cref="ClassCastException"> if the specified key cannot be compared
		'''         with the keys currently in the map </exception>
		''' <exception cref="NullPointerException"> if the specified key or value is null </exception>
		Public Overridable Function replace(ByVal key As K, ByVal value As V) As V
			If key Is Nothing OrElse value Is Nothing Then Throw New NullPointerException
			Do
				Dim n As Node(Of K, V)
				Dim v As Object
				n = findNode(key)
				If n Is Nothing Then Return Nothing
				v = n.value
				If v IsNot Nothing AndAlso n.casValue(v, value) Then
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
					Dim vv As V = CType(v, V)
					Return vv
				End If
			Loop
		End Function

		' ------ SortedMap API methods ------ 

'JAVA TO VB CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
		Public Overridable Function comparator() As IComparer(Of ?)
			Return comparator_Renamed
		End Function

		''' <exception cref="NoSuchElementException"> {@inheritDoc} </exception>
		Public Overridable Function firstKey() As K
			Dim n As Node(Of K, V) = findFirst()
			If n Is Nothing Then Throw New java.util.NoSuchElementException
			Return n.key
		End Function

		''' <exception cref="NoSuchElementException"> {@inheritDoc} </exception>
		Public Overridable Function lastKey() As K
			Dim n As Node(Of K, V) = findLast()
			If n Is Nothing Then Throw New java.util.NoSuchElementException
			Return n.key
		End Function

		''' <exception cref="ClassCastException"> {@inheritDoc} </exception>
		''' <exception cref="NullPointerException"> if {@code fromKey} or {@code toKey} is null </exception>
		''' <exception cref="IllegalArgumentException"> {@inheritDoc} </exception>
		Public Overridable Function subMap(ByVal fromKey As K, ByVal fromInclusive As Boolean, ByVal toKey As K, ByVal toInclusive As Boolean) As java.util.concurrent.ConcurrentNavigableMap(Of K, V)
			If fromKey Is Nothing OrElse toKey Is Nothing Then Throw New NullPointerException
			Return New SubMap(Of K, V) (Me, fromKey, fromInclusive, toKey, toInclusive, False)
		End Function

		''' <exception cref="ClassCastException"> {@inheritDoc} </exception>
		''' <exception cref="NullPointerException"> if {@code toKey} is null </exception>
		''' <exception cref="IllegalArgumentException"> {@inheritDoc} </exception>
		Public Overridable Function headMap(ByVal toKey As K, ByVal inclusive As Boolean) As java.util.concurrent.ConcurrentNavigableMap(Of K, V)
			If toKey Is Nothing Then Throw New NullPointerException
			Return New SubMap(Of K, V) (Me, Nothing, False, toKey, inclusive, False)
		End Function

		''' <exception cref="ClassCastException"> {@inheritDoc} </exception>
		''' <exception cref="NullPointerException"> if {@code fromKey} is null </exception>
		''' <exception cref="IllegalArgumentException"> {@inheritDoc} </exception>
		Public Overridable Function tailMap(ByVal fromKey As K, ByVal inclusive As Boolean) As java.util.concurrent.ConcurrentNavigableMap(Of K, V)
			If fromKey Is Nothing Then Throw New NullPointerException
			Return New SubMap(Of K, V) (Me, fromKey, inclusive, Nothing, False, False)
		End Function

		''' <exception cref="ClassCastException"> {@inheritDoc} </exception>
		''' <exception cref="NullPointerException"> if {@code fromKey} or {@code toKey} is null </exception>
		''' <exception cref="IllegalArgumentException"> {@inheritDoc} </exception>
		Public Overridable Function subMap(ByVal fromKey As K, ByVal toKey As K) As java.util.concurrent.ConcurrentNavigableMap(Of K, V)
			Return subMap(fromKey, True, toKey, False)
		End Function

		''' <exception cref="ClassCastException"> {@inheritDoc} </exception>
		''' <exception cref="NullPointerException"> if {@code toKey} is null </exception>
		''' <exception cref="IllegalArgumentException"> {@inheritDoc} </exception>
		Public Overridable Function headMap(ByVal toKey As K) As java.util.concurrent.ConcurrentNavigableMap(Of K, V)
			Return headMap(toKey, False)
		End Function

		''' <exception cref="ClassCastException"> {@inheritDoc} </exception>
		''' <exception cref="NullPointerException"> if {@code fromKey} is null </exception>
		''' <exception cref="IllegalArgumentException"> {@inheritDoc} </exception>
		Public Overridable Function tailMap(ByVal fromKey As K) As java.util.concurrent.ConcurrentNavigableMap(Of K, V)
			Return tailMap(fromKey, True)
		End Function

		' ---------------- Relational operations -------------- 

		''' <summary>
		''' Returns a key-value mapping associated with the greatest key
		''' strictly less than the given key, or {@code null} if there is
		''' no such key. The returned entry does <em>not</em> support the
		''' {@code Entry.setValue} method.
		''' </summary>
		''' <exception cref="ClassCastException"> {@inheritDoc} </exception>
		''' <exception cref="NullPointerException"> if the specified key is null </exception>
		Public Overridable Function lowerEntry(ByVal key As K) As KeyValuePair(Of K, V)
			Return getNear(key, LT)
		End Function

		''' <exception cref="ClassCastException"> {@inheritDoc} </exception>
		''' <exception cref="NullPointerException"> if the specified key is null </exception>
		Public Overridable Function lowerKey(ByVal key As K) As K
			Dim n As Node(Of K, V) = findNear(key, LT, comparator_Renamed)
			Return If(n Is Nothing, Nothing, n.key)
		End Function

		''' <summary>
		''' Returns a key-value mapping associated with the greatest key
		''' less than or equal to the given key, or {@code null} if there
		''' is no such key. The returned entry does <em>not</em> support
		''' the {@code Entry.setValue} method.
		''' </summary>
		''' <param name="key"> the key </param>
		''' <exception cref="ClassCastException"> {@inheritDoc} </exception>
		''' <exception cref="NullPointerException"> if the specified key is null </exception>
		Public Overridable Function floorEntry(ByVal key As K) As KeyValuePair(Of K, V)
			Return getNear(key, LT Or EQ)
		End Function

		''' <param name="key"> the key </param>
		''' <exception cref="ClassCastException"> {@inheritDoc} </exception>
		''' <exception cref="NullPointerException"> if the specified key is null </exception>
		Public Overridable Function floorKey(ByVal key As K) As K
			Dim n As Node(Of K, V) = findNear(key, LT Or EQ, comparator_Renamed)
			Return If(n Is Nothing, Nothing, n.key)
		End Function

		''' <summary>
		''' Returns a key-value mapping associated with the least key
		''' greater than or equal to the given key, or {@code null} if
		''' there is no such entry. The returned entry does <em>not</em>
		''' support the {@code Entry.setValue} method.
		''' </summary>
		''' <exception cref="ClassCastException"> {@inheritDoc} </exception>
		''' <exception cref="NullPointerException"> if the specified key is null </exception>
		Public Overridable Function ceilingEntry(ByVal key As K) As KeyValuePair(Of K, V)
			Return getNear(key, GT Or EQ)
		End Function

		''' <exception cref="ClassCastException"> {@inheritDoc} </exception>
		''' <exception cref="NullPointerException"> if the specified key is null </exception>
		Public Overridable Function ceilingKey(ByVal key As K) As K
			Dim n As Node(Of K, V) = findNear(key, GT Or EQ, comparator_Renamed)
			Return If(n Is Nothing, Nothing, n.key)
		End Function

		''' <summary>
		''' Returns a key-value mapping associated with the least key
		''' strictly greater than the given key, or {@code null} if there
		''' is no such key. The returned entry does <em>not</em> support
		''' the {@code Entry.setValue} method.
		''' </summary>
		''' <param name="key"> the key </param>
		''' <exception cref="ClassCastException"> {@inheritDoc} </exception>
		''' <exception cref="NullPointerException"> if the specified key is null </exception>
		Public Overridable Function higherEntry(ByVal key As K) As KeyValuePair(Of K, V)
			Return getNear(key, GT)
		End Function

		''' <param name="key"> the key </param>
		''' <exception cref="ClassCastException"> {@inheritDoc} </exception>
		''' <exception cref="NullPointerException"> if the specified key is null </exception>
		Public Overridable Function higherKey(ByVal key As K) As K
			Dim n As Node(Of K, V) = findNear(key, GT, comparator_Renamed)
			Return If(n Is Nothing, Nothing, n.key)
		End Function

		''' <summary>
		''' Returns a key-value mapping associated with the least
		''' key in this map, or {@code null} if the map is empty.
		''' The returned entry does <em>not</em> support
		''' the {@code Entry.setValue} method.
		''' </summary>
		Public Overridable Function firstEntry() As KeyValuePair(Of K, V)
			Do
				Dim n As Node(Of K, V) = findFirst()
				If n Is Nothing Then Return Nothing
				Dim e As java.util.AbstractMap.SimpleImmutableEntry(Of K, V) = n.createSnapshot()
				If e IsNot Nothing Then Return e
			Loop
		End Function

		''' <summary>
		''' Returns a key-value mapping associated with the greatest
		''' key in this map, or {@code null} if the map is empty.
		''' The returned entry does <em>not</em> support
		''' the {@code Entry.setValue} method.
		''' </summary>
		Public Overridable Function lastEntry() As KeyValuePair(Of K, V)
			Do
				Dim n As Node(Of K, V) = findLast()
				If n Is Nothing Then Return Nothing
				Dim e As java.util.AbstractMap.SimpleImmutableEntry(Of K, V) = n.createSnapshot()
				If e IsNot Nothing Then Return e
			Loop
		End Function

		''' <summary>
		''' Removes and returns a key-value mapping associated with
		''' the least key in this map, or {@code null} if the map is empty.
		''' The returned entry does <em>not</em> support
		''' the {@code Entry.setValue} method.
		''' </summary>
		Public Overridable Function pollFirstEntry() As KeyValuePair(Of K, V)
			Return doRemoveFirstEntry()
		End Function

		''' <summary>
		''' Removes and returns a key-value mapping associated with
		''' the greatest key in this map, or {@code null} if the map is empty.
		''' The returned entry does <em>not</em> support
		''' the {@code Entry.setValue} method.
		''' </summary>
		Public Overridable Function pollLastEntry() As KeyValuePair(Of K, V)
			Return doRemoveLastEntry()
		End Function


		' ---------------- Iterators -------------- 

		''' <summary>
		''' Base of iterator classes:
		''' </summary>
		Friend MustInherit Class Iter(Of T)
			Implements IEnumerator(Of T)

			Private ReadOnly outerInstance As ConcurrentSkipListMap

			''' <summary>
			''' the last node returned by next() </summary>
			Friend lastReturned As Node(Of K, V)
			''' <summary>
			''' the next node to return from next(); </summary>
			Friend [next] As Node(Of K, V)
			''' <summary>
			''' Cache of next value field to maintain weak consistency </summary>
			Friend nextValue As V

			''' <summary>
			''' Initializes ascending iterator for entire range. </summary>
			Friend Sub New(ByVal outerInstance As ConcurrentSkipListMap)
					Me.outerInstance = outerInstance
				[next] = outerInstance.findFirst()
				Do While [next] IsNot Nothing
					Dim x As Object = [next].value
					If x IsNot Nothing AndAlso x IsNot [next] Then
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
						Dim vv As V = CType(x, V)
						nextValue = vv
						Exit Do
					End If
					[next] = outerInstance.findFirst()
				Loop
			End Sub

			Public Function hasNext() As Boolean
				Return [next] IsNot Nothing
			End Function

			''' <summary>
			''' Advances next to higher entry. </summary>
			Friend Sub advance()
				If [next] Is Nothing Then Throw New java.util.NoSuchElementException
				lastReturned = [next]
				[next] = [next].next
				Do While [next] IsNot Nothing
					Dim x As Object = [next].value
					If x IsNot Nothing AndAlso x IsNot [next] Then
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
						Dim vv As V = CType(x, V)
						nextValue = vv
						Exit Do
					End If
					[next] = [next].next
				Loop
			End Sub

			Public Overridable Sub remove()
				Dim l As Node(Of K, V) = lastReturned
				If l Is Nothing Then Throw New IllegalStateException
				' It would not be worth all of the overhead to directly
				' unlink from here. Using remove is fast enough.
				outerInstance.remove(l.key)
				lastReturned = Nothing
			End Sub

		End Class

		Friend NotInheritable Class ValueIterator
			Inherits Iter(Of V)

			Private ReadOnly outerInstance As ConcurrentSkipListMap

			Public Sub New(ByVal outerInstance As ConcurrentSkipListMap)
				Me.outerInstance = outerInstance
			End Sub

			Public Function [next]() As V
				Dim v As V = nextValue
				advance()
				Return v
			End Function
		End Class

		Friend NotInheritable Class KeyIterator
			Inherits Iter(Of K)

			Private ReadOnly outerInstance As ConcurrentSkipListMap

			Public Sub New(ByVal outerInstance As ConcurrentSkipListMap)
				Me.outerInstance = outerInstance
			End Sub

			Public Function [next]() As K
				Dim n As Node(Of K, V) = [next]
				advance()
				Return n.key
			End Function
		End Class

		Friend NotInheritable Class EntryIterator
			Inherits Iter(Of KeyValuePair(Of K, V))

			Private ReadOnly outerInstance As ConcurrentSkipListMap

			Public Sub New(ByVal outerInstance As ConcurrentSkipListMap)
				Me.outerInstance = outerInstance
			End Sub

			Public Function [next]() As KeyValuePair(Of K, V)
				Dim n As Node(Of K, V) = [next]
				Dim v As V = nextValue
				advance()
				Return New java.util.AbstractMap.SimpleImmutableEntry(Of K, V)(n.key, v)
			End Function
		End Class

		' Factory methods for iterators needed by ConcurrentSkipListSet etc

		Friend Overridable Function keyIterator() As IEnumerator(Of K)
			Return New KeyIterator(Me)
		End Function

		Friend Overridable Function valueIterator() As IEnumerator(Of V)
			Return New ValueIterator(Me)
		End Function

		Friend Overridable Function entryIterator() As IEnumerator(Of KeyValuePair(Of K, V))
			Return New EntryIterator(Me)
		End Function

		' ---------------- View Classes -------------- 

	'    
	'     * View classes are static, delegating to a ConcurrentNavigableMap
	'     * to allow use by SubMaps, which outweighs the ugliness of
	'     * needing type-tests for Iterator methods.
	'     

		Friend Shared Function toList(Of E)(ByVal c As ICollection(Of E)) As IList(Of E)
			' Using size() here would be a pessimization.
			Dim list As New List(Of E)
			For Each e As E In c
				list.add(e)
			Next e
			Return list
		End Function

		Friend NotInheritable Class KeySet(Of E)
			Inherits java.util.AbstractSet(Of E)
			Implements java.util.NavigableSet(Of E)

'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
			Friend ReadOnly m As java.util.concurrent.ConcurrentNavigableMap(Of E, ?)
			Friend Sub New(Of T1)(ByVal map As java.util.concurrent.ConcurrentNavigableMap(Of T1))
				m = map
			End Sub
			Public Function size() As Integer
				Return m.size()
			End Function
			Public Property empty As Boolean
				Get
					Return m.empty
				End Get
			End Property
			Public Function contains(ByVal o As Object) As Boolean
				Return m.containsKey(o)
			End Function
			Public Function remove(ByVal o As Object) As Boolean
				Return m.remove(o) IsNot Nothing
			End Function
			Public Sub clear()
				m.clear()
			End Sub
			Public Function lower(ByVal e As E) As E
				Return m.lowerKey(e)
			End Function
			Public Function floor(ByVal e As E) As E
				Return m.floorKey(e)
			End Function
			Public Function ceiling(ByVal e As E) As E
				Return m.ceilingKey(e)
			End Function
			Public Function higher(ByVal e As E) As E
				Return m.higherKey(e)
			End Function
'JAVA TO VB CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
			Public Function comparator() As IComparer(Of ?)
				Return m.comparator()
			End Function
			Public Function first() As E
				Return m.firstKey()
			End Function
			Public Function last() As E
				Return m.lastKey()
			End Function
			Public Function pollFirst() As E
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
				Dim e As KeyValuePair(Of E, ?) = m.pollFirstEntry()
				Return If(e Is Nothing, Nothing, e.Key)
			End Function
			Public Function pollLast() As E
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
				Dim e As KeyValuePair(Of E, ?) = m.pollLastEntry()
				Return If(e Is Nothing, Nothing, e.Key)
			End Function
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
			Public Function [iterator]() As IEnumerator(Of E)
				If TypeOf m Is ConcurrentSkipListMap Then
					Return CType(m, ConcurrentSkipListMap(Of E, Object)).keyIterator()
				Else
					Return CType(m, ConcurrentSkipListMap.SubMap(Of E, Object)).keyIterator()
				End If
			End Function
			Public Overrides Function Equals(ByVal o As Object) As Boolean
				If o Is Me Then Return True
				If Not(TypeOf o Is java.util.Set) Then Return False
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
				Dim c As ICollection(Of ?) = CType(o, ICollection(Of ?))
				Try
'JAVA TO VB CONVERTER TODO TASK: There is no .NET equivalent to the java.util.Collection 'containsAll' method:
					Return containsAll(c) AndAlso c.containsAll(Me)
				Catch unused As  [Class]CastException
					Return False
				Catch unused As NullPointerException
					Return False
				End Try
			End Function
			Public Function toArray() As Object()
				Return toList(Me).ToArray()
			End Function
			Public Function toArray(Of T)(ByVal a As T()) As T()
				Return toList(Me).ToArray(a)
			End Function
			Public Function descendingIterator() As IEnumerator(Of E)
				Return descendingSet().GetEnumerator()
			End Function
			Public Function subSet(ByVal fromElement As E, ByVal fromInclusive As Boolean, ByVal toElement As E, ByVal toInclusive As Boolean) As java.util.NavigableSet(Of E)
				Return New KeySet(Of E)(m.subMap(fromElement, fromInclusive, toElement, toInclusive))
			End Function
			Public Function headSet(ByVal toElement As E, ByVal inclusive As Boolean) As java.util.NavigableSet(Of E)
				Return New KeySet(Of E)(m.headMap(toElement, inclusive))
			End Function
			Public Function tailSet(ByVal fromElement As E, ByVal inclusive As Boolean) As java.util.NavigableSet(Of E)
				Return New KeySet(Of E)(m.tailMap(fromElement, inclusive))
			End Function
			Public Function subSet(ByVal fromElement As E, ByVal toElement As E) As java.util.NavigableSet(Of E)
				Return subSet(fromElement, True, toElement, False)
			End Function
			Public Function headSet(ByVal toElement As E) As java.util.NavigableSet(Of E)
				Return headSet(toElement, False)
			End Function
			Public Function tailSet(ByVal fromElement As E) As java.util.NavigableSet(Of E)
				Return tailSet(fromElement, True)
			End Function
			Public Function descendingSet() As java.util.NavigableSet(Of E)
				Return New KeySet(Of E)(m.descendingMap())
			End Function
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
			Public Function spliterator() As java.util.Spliterator(Of E)
				If TypeOf m Is ConcurrentSkipListMap Then
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
					Return CType(m, ConcurrentSkipListMap(Of E, ?)).keySpliterator()
				Else
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
					Return CType(CType(m, SubMap(Of E, ?)).keyIterator(), java.util.Spliterator(Of E))
				End If
			End Function
		End Class

		Friend NotInheritable Class Values(Of E)
			Inherits java.util.AbstractCollection(Of E)

'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
			Friend ReadOnly m As java.util.concurrent.ConcurrentNavigableMap(Of ?, E)
			Friend Sub New(Of T1)(ByVal map As java.util.concurrent.ConcurrentNavigableMap(Of T1))
				m = map
			End Sub
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
			Public Function [iterator]() As IEnumerator(Of E)
				If TypeOf m Is ConcurrentSkipListMap Then
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
					Return CType(m, ConcurrentSkipListMap(Of ?, E)).valueIterator()
				Else
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
					Return CType(m, SubMap(Of ?, E)).valueIterator()
				End If
			End Function
			Public Property empty As Boolean
				Get
					Return m.empty
				End Get
			End Property
			Public Function size() As Integer
				Return m.size()
			End Function
			Public Function contains(ByVal o As Object) As Boolean
				Return m.containsValue(o)
			End Function
			Public Sub clear()
				m.clear()
			End Sub
			Public Function toArray() As Object()
				Return toList(Me).ToArray()
			End Function
			Public Function toArray(Of T)(ByVal a As T()) As T()
				Return toList(Me).ToArray(a)
			End Function
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
			Public Function spliterator() As java.util.Spliterator(Of E)
				If TypeOf m Is ConcurrentSkipListMap Then
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
					Return CType(m, ConcurrentSkipListMap(Of ?, E)).valueSpliterator()
				Else
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
					Return CType(CType(m, SubMap(Of ?, E)).valueIterator(), java.util.Spliterator(Of E))
				End If
			End Function
		End Class

		Friend NotInheritable Class EntrySet(Of K1, V1)
			Inherits java.util.AbstractSet(Of KeyValuePair(Of K1, V1))

			Friend ReadOnly m As java.util.concurrent.ConcurrentNavigableMap(Of K1, V1)
			Friend Sub New(ByVal map As java.util.concurrent.ConcurrentNavigableMap(Of K1, V1))
				m = map
			End Sub
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
			Public Function [iterator]() As IEnumerator(Of KeyValuePair(Of K1, V1))
				If TypeOf m Is ConcurrentSkipListMap Then
					Return CType(m, ConcurrentSkipListMap(Of K1, V1)).entryIterator()
				Else
					Return CType(m, SubMap(Of K1, V1)).entryIterator()
				End If
			End Function

			Public Function contains(ByVal o As Object) As Boolean
				If Not(TypeOf o Is DictionaryEntry) Then Return False
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
				Dim e As KeyValuePair(Of ?, ?) = CType(o, KeyValuePair(Of ?, ?))
				Dim v As V1 = m.get(e.Key)
				Return v IsNot Nothing AndAlso v.Equals(e.Value)
			End Function
			Public Function remove(ByVal o As Object) As Boolean
				If Not(TypeOf o Is DictionaryEntry) Then Return False
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
				Dim e As KeyValuePair(Of ?, ?) = CType(o, KeyValuePair(Of ?, ?))
				Return m.remove(e.Key, e.Value)
			End Function
			Public Property empty As Boolean
				Get
					Return m.empty
				End Get
			End Property
			Public Function size() As Integer
				Return m.size()
			End Function
			Public Sub clear()
				m.clear()
			End Sub
			Public Overrides Function Equals(ByVal o As Object) As Boolean
				If o Is Me Then Return True
				If Not(TypeOf o Is java.util.Set) Then Return False
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
				Dim c As ICollection(Of ?) = CType(o, ICollection(Of ?))
				Try
'JAVA TO VB CONVERTER TODO TASK: There is no .NET equivalent to the java.util.Collection 'containsAll' method:
					Return containsAll(c) AndAlso c.containsAll(Me)
				Catch unused As  [Class]CastException
					Return False
				Catch unused As NullPointerException
					Return False
				End Try
			End Function
			Public Function toArray() As Object()
				Return toList(Me).ToArray()
			End Function
			Public Function toArray(Of T)(ByVal a As T()) As T()
				Return toList(Me).ToArray(a)
			End Function
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
			Public Function spliterator() As java.util.Spliterator(Of KeyValuePair(Of K1, V1))
				If TypeOf m Is ConcurrentSkipListMap Then
					Return CType(m, ConcurrentSkipListMap(Of K1, V1)).entrySpliterator()
				Else
					Return CType(CType(m, SubMap(Of K1, V1)).entryIterator(), java.util.Spliterator(Of KeyValuePair(Of K1, V1)))
				End If
			End Function
		End Class

		''' <summary>
		''' Submaps returned by <seealso cref="ConcurrentSkipListMap"/> submap operations
		''' represent a subrange of mappings of their underlying
		''' maps. Instances of this class support all methods of their
		''' underlying maps, differing in that mappings outside their range are
		''' ignored, and attempts to add mappings outside their ranges result
		''' in <seealso cref="IllegalArgumentException"/>.  Instances of this class are
		''' constructed only using the {@code subMap}, {@code headMap}, and
		''' {@code tailMap} methods of their underlying maps.
		''' 
		''' @serial include
		''' </summary>
		<Serializable> _
		Friend NotInheritable Class SubMap(Of K, V)
			Inherits java.util.AbstractMap(Of K, V)
			Implements java.util.concurrent.ConcurrentNavigableMap(Of K, V), Cloneable

			Private Const serialVersionUID As Long = -7647078645895051609L

			''' <summary>
			''' Underlying map </summary>
			Private ReadOnly m As ConcurrentSkipListMap(Of K, V)
			''' <summary>
			''' lower bound key, or null if from start </summary>
			Private ReadOnly lo As K
			''' <summary>
			''' upper bound key, or null if to end </summary>
			Private ReadOnly hi As K
			''' <summary>
			''' inclusion flag for lo </summary>
			Private ReadOnly loInclusive As Boolean
			''' <summary>
			''' inclusion flag for hi </summary>
			Private ReadOnly hiInclusive As Boolean
			''' <summary>
			''' direction </summary>
			Private ReadOnly isDescending As Boolean

			' Lazily initialized view holders
			<NonSerialized> _
			Private keySetView As KeySet(Of K)
			<NonSerialized> _
			Private entrySetView As java.util.Set(Of KeyValuePair(Of K, V))
			<NonSerialized> _
			Private valuesView As ICollection(Of V)

			''' <summary>
			''' Creates a new submap, initializing all fields.
			''' </summary>
			Friend Sub New(ByVal map As ConcurrentSkipListMap(Of K, V), ByVal fromKey As K, ByVal fromInclusive As Boolean, ByVal toKey As K, ByVal toInclusive As Boolean, ByVal isDescending As Boolean)
'JAVA TO VB CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
				Dim cmp As IComparer(Of ?) = map.comparator_Renamed
				If fromKey IsNot Nothing AndAlso toKey IsNot Nothing AndAlso cpr(cmp, fromKey, toKey) > 0 Then Throw New IllegalArgumentException("inconsistent range")
				Me.m = map
				Me.lo = fromKey
				Me.hi = toKey
				Me.loInclusive = fromInclusive
				Me.hiInclusive = toInclusive
				Me.isDescending = isDescending
			End Sub

			' ----------------  Utilities -------------- 

'JAVA TO VB CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
			Friend Function tooLow(Of T1)(ByVal key As Object, ByVal cmp As IComparer(Of T1)) As Boolean
				Dim c As Integer
'JAVA TO VB CONVERTER TODO TASK: Assignments within expressions are not supported in VB
				Return (lo IsNot Nothing AndAlso ((c = cpr(cmp, key, lo)) < 0 OrElse (c = 0 AndAlso (Not loInclusive))))
			End Function

'JAVA TO VB CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
			Friend Function tooHigh(Of T1)(ByVal key As Object, ByVal cmp As IComparer(Of T1)) As Boolean
				Dim c As Integer
'JAVA TO VB CONVERTER TODO TASK: Assignments within expressions are not supported in VB
				Return (hi IsNot Nothing AndAlso ((c = cpr(cmp, key, hi)) > 0 OrElse (c = 0 AndAlso (Not hiInclusive))))
			End Function

'JAVA TO VB CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
			Friend Function inBounds(Of T1)(ByVal key As Object, ByVal cmp As IComparer(Of T1)) As Boolean
				Return (Not tooLow(key, cmp)) AndAlso Not tooHigh(key, cmp)
			End Function

'JAVA TO VB CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
			Friend Sub checkKeyBounds(Of T1)(ByVal key As K, ByVal cmp As IComparer(Of T1))
				If key Is Nothing Then Throw New NullPointerException
				If Not inBounds(key, cmp) Then Throw New IllegalArgumentException("key out of range")
			End Sub

			''' <summary>
			''' Returns true if node key is less than upper bound of range.
			''' </summary>
'JAVA TO VB CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
			Friend Function isBeforeEnd(Of T1)(ByVal n As ConcurrentSkipListMap.Node(Of K, V), ByVal cmp As IComparer(Of T1)) As Boolean
				If n Is Nothing Then Return False
				If hi Is Nothing Then Return True
				Dim k As K = n.key
				If k Is Nothing Then ' pass by markers and headers Return True
				Dim c As Integer = cpr(cmp, k, hi)
				If c > 0 OrElse (c = 0 AndAlso (Not hiInclusive)) Then Return False
				Return True
			End Function

			''' <summary>
			''' Returns lowest node. This node might not be in range, so
			''' most usages need to check bounds.
			''' </summary>
'JAVA TO VB CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
			Friend Function loNode(Of T1)(ByVal cmp As IComparer(Of T1)) As ConcurrentSkipListMap.Node(Of K, V)
				If lo Is Nothing Then
					Return m.findFirst()
				ElseIf loInclusive Then
					Return m.findNear(lo, GT Or EQ, cmp)
				Else
					Return m.findNear(lo, GT, cmp)
				End If
			End Function

			''' <summary>
			''' Returns highest node. This node might not be in range, so
			''' most usages need to check bounds.
			''' </summary>
'JAVA TO VB CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
			Friend Function hiNode(Of T1)(ByVal cmp As IComparer(Of T1)) As ConcurrentSkipListMap.Node(Of K, V)
				If hi Is Nothing Then
					Return m.findLast()
				ElseIf hiInclusive Then
					Return m.findNear(hi, LT Or EQ, cmp)
				Else
					Return m.findNear(hi, LT, cmp)
				End If
			End Function

			''' <summary>
			''' Returns lowest absolute key (ignoring directonality).
			''' </summary>
			Friend Function lowestKey() As K
'JAVA TO VB CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
				Dim cmp As IComparer(Of ?) = m.comparator_Renamed
				Dim n As ConcurrentSkipListMap.Node(Of K, V) = loNode(cmp)
				If isBeforeEnd(n, cmp) Then
					Return n.key
				Else
					Throw New java.util.NoSuchElementException
				End If
			End Function

			''' <summary>
			''' Returns highest absolute key (ignoring directonality).
			''' </summary>
			Friend Function highestKey() As K
'JAVA TO VB CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
				Dim cmp As IComparer(Of ?) = m.comparator_Renamed
				Dim n As ConcurrentSkipListMap.Node(Of K, V) = hiNode(cmp)
				If n IsNot Nothing Then
					Dim last As K = n.key
					If inBounds(last, cmp) Then Return last
				End If
				Throw New java.util.NoSuchElementException
			End Function

			Friend Function lowestEntry() As KeyValuePair(Of K, V)
'JAVA TO VB CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
				Dim cmp As IComparer(Of ?) = m.comparator_Renamed
				Do
					Dim n As ConcurrentSkipListMap.Node(Of K, V) = loNode(cmp)
					If Not isBeforeEnd(n, cmp) Then Return Nothing
					Dim e As KeyValuePair(Of K, V) = n.createSnapshot()
					If e IsNot Nothing Then Return e
				Loop
			End Function

			Friend Function highestEntry() As KeyValuePair(Of K, V)
'JAVA TO VB CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
				Dim cmp As IComparer(Of ?) = m.comparator_Renamed
				Do
					Dim n As ConcurrentSkipListMap.Node(Of K, V) = hiNode(cmp)
					If n Is Nothing OrElse (Not inBounds(n.key, cmp)) Then Return Nothing
					Dim e As KeyValuePair(Of K, V) = n.createSnapshot()
					If e IsNot Nothing Then Return e
				Loop
			End Function

			Friend Function removeLowest() As KeyValuePair(Of K, V)
'JAVA TO VB CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
				Dim cmp As IComparer(Of ?) = m.comparator_Renamed
				Do
					Dim n As Node(Of K, V) = loNode(cmp)
					If n Is Nothing Then Return Nothing
					Dim k As K = n.key
					If Not inBounds(k, cmp) Then Return Nothing
					Dim v As V = m.doRemove(k, Nothing)
					If v IsNot Nothing Then Return New java.util.AbstractMap.SimpleImmutableEntry(Of K, V)(k, v)
				Loop
			End Function

			Friend Function removeHighest() As KeyValuePair(Of K, V)
'JAVA TO VB CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
				Dim cmp As IComparer(Of ?) = m.comparator_Renamed
				Do
					Dim n As Node(Of K, V) = hiNode(cmp)
					If n Is Nothing Then Return Nothing
					Dim k As K = n.key
					If Not inBounds(k, cmp) Then Return Nothing
					Dim v As V = m.doRemove(k, Nothing)
					If v IsNot Nothing Then Return New java.util.AbstractMap.SimpleImmutableEntry(Of K, V)(k, v)
				Loop
			End Function

			''' <summary>
			''' Submap version of ConcurrentSkipListMap.getNearEntry
			''' </summary>
			Friend Function getNearEntry(ByVal key As K, ByVal rel As Integer) As KeyValuePair(Of K, V)
'JAVA TO VB CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
				Dim cmp As IComparer(Of ?) = m.comparator_Renamed
				If isDescending Then ' adjust relation for direction
					If (rel And LT) = 0 Then
						rel = rel Or LT
					Else
						rel = rel And Not LT
					End If
				End If
				If tooLow(key, cmp) Then Return If((rel And LT) <> 0, Nothing, lowestEntry())
				If tooHigh(key, cmp) Then Return If((rel And LT) <> 0, highestEntry(), Nothing)
				Do
					Dim n As Node(Of K, V) = m.findNear(key, rel, cmp)
					If n Is Nothing OrElse (Not inBounds(n.key, cmp)) Then Return Nothing
					Dim k As K = n.key
					Dim v As V = n.validValue
					If v IsNot Nothing Then Return New java.util.AbstractMap.SimpleImmutableEntry(Of K, V)(k, v)
				Loop
			End Function

			' Almost the same as getNearEntry, except for keys
			Friend Function getNearKey(ByVal key As K, ByVal rel As Integer) As K
'JAVA TO VB CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
				Dim cmp As IComparer(Of ?) = m.comparator_Renamed
				If isDescending Then ' adjust relation for direction
					If (rel And LT) = 0 Then
						rel = rel Or LT
					Else
						rel = rel And Not LT
					End If
				End If
				If tooLow(key, cmp) Then
					If (rel And LT) = 0 Then
						Dim n As ConcurrentSkipListMap.Node(Of K, V) = loNode(cmp)
						If isBeforeEnd(n, cmp) Then Return n.key
					End If
					Return Nothing
				End If
				If tooHigh(key, cmp) Then
					If (rel And LT) <> 0 Then
						Dim n As ConcurrentSkipListMap.Node(Of K, V) = hiNode(cmp)
						If n IsNot Nothing Then
							Dim last As K = n.key
							If inBounds(last, cmp) Then Return last
						End If
					End If
					Return Nothing
				End If
				Do
					Dim n As Node(Of K, V) = m.findNear(key, rel, cmp)
					If n Is Nothing OrElse (Not inBounds(n.key, cmp)) Then Return Nothing
					Dim k As K = n.key
					Dim v As V = n.validValue
					If v IsNot Nothing Then Return k
				Loop
			End Function

			' ----------------  Map API methods -------------- 

			Public Function containsKey(ByVal key As Object) As Boolean
				If key Is Nothing Then Throw New NullPointerException
				Return inBounds(key, m.comparator_Renamed) AndAlso m.containsKey(key)
			End Function

			Public Function [get](ByVal key As Object) As V
				If key Is Nothing Then Throw New NullPointerException
				Return If((Not inBounds(key, m.comparator_Renamed)), Nothing, m.get(key))
			End Function

			Public Function put(ByVal key As K, ByVal value As V) As V
				checkKeyBounds(key, m.comparator_Renamed)
				Return m.put(key, value)
			End Function

			Public Function remove(ByVal key As Object) As V
				Return If((Not inBounds(key, m.comparator_Renamed)), Nothing, m.remove(key))
			End Function

			Public Function size() As Integer
'JAVA TO VB CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
				Dim cmp As IComparer(Of ?) = m.comparator_Renamed
				Dim count As Long = 0
				Dim n As ConcurrentSkipListMap.Node(Of K, V) = loNode(cmp)
				Do While isBeforeEnd(n, cmp)
					If n.validValue IsNot Nothing Then count += 1
					n = n.next
				Loop
				Return If(count >=  java.lang.[Integer].Max_Value,  java.lang.[Integer].Max_Value, CInt(count))
			End Function

			Public Property empty As Boolean
				Get
	'JAVA TO VB CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
	'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
					Dim cmp As IComparer(Of ?) = m.comparator_Renamed
					Return Not isBeforeEnd(loNode(cmp), cmp)
				End Get
			End Property

			Public Function containsValue(ByVal value As Object) As Boolean
				If value Is Nothing Then Throw New NullPointerException
'JAVA TO VB CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
				Dim cmp As IComparer(Of ?) = m.comparator_Renamed
				Dim n As ConcurrentSkipListMap.Node(Of K, V) = loNode(cmp)
				Do While isBeforeEnd(n, cmp)
					Dim v As V = n.validValue
					If v IsNot Nothing AndAlso value.Equals(v) Then Return True
					n = n.next
				Loop
				Return False
			End Function

			Public Sub clear()
'JAVA TO VB CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
				Dim cmp As IComparer(Of ?) = m.comparator_Renamed
				Dim n As ConcurrentSkipListMap.Node(Of K, V) = loNode(cmp)
				Do While isBeforeEnd(n, cmp)
					If n.validValue IsNot Nothing Then m.remove(n.key)
					n = n.next
				Loop
			End Sub

			' ----------------  ConcurrentMap API methods -------------- 

			Public Function putIfAbsent(ByVal key As K, ByVal value As V) As V
				checkKeyBounds(key, m.comparator_Renamed)
				Return m.putIfAbsent(key, value)
			End Function

			Public Function remove(ByVal key As Object, ByVal value As Object) As Boolean
				Return inBounds(key, m.comparator_Renamed) AndAlso m.remove(key, value)
			End Function

			Public Function replace(ByVal key As K, ByVal oldValue As V, ByVal newValue As V) As Boolean
				checkKeyBounds(key, m.comparator_Renamed)
				Return m.replace(key, oldValue, newValue)
			End Function

			Public Function replace(ByVal key As K, ByVal value As V) As V
				checkKeyBounds(key, m.comparator_Renamed)
				Return m.replace(key, value)
			End Function

			' ----------------  SortedMap API methods -------------- 

'JAVA TO VB CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
			Public Function comparator() As IComparer(Of ?)
'JAVA TO VB CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
				Dim cmp As IComparer(Of ?) = m.comparator()
				If isDescending Then
					Return java.util.Collections.reverseOrder(cmp)
				Else
					Return cmp
				End If
			End Function

			''' <summary>
			''' Utility to create submaps, where given bounds override
			''' unbounded(null) ones and/or are checked against bounded ones.
			''' </summary>
			Friend Function newSubMap(ByVal fromKey As K, ByVal fromInclusive As Boolean, ByVal toKey As K, ByVal toInclusive As Boolean) As SubMap(Of K, V)
'JAVA TO VB CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
				Dim cmp As IComparer(Of ?) = m.comparator_Renamed
				If isDescending Then ' flip senses
					Dim tk As K = fromKey
					fromKey = toKey
					toKey = tk
					Dim ti As Boolean = fromInclusive
					fromInclusive = toInclusive
					toInclusive = ti
				End If
				If lo IsNot Nothing Then
					If fromKey Is Nothing Then
						fromKey = lo
						fromInclusive = loInclusive
					Else
						Dim c As Integer = cpr(cmp, fromKey, lo)
						If c < 0 OrElse (c = 0 AndAlso (Not loInclusive) AndAlso fromInclusive) Then Throw New IllegalArgumentException("key out of range")
					End If
				End If
				If hi IsNot Nothing Then
					If toKey Is Nothing Then
						toKey = hi
						toInclusive = hiInclusive
					Else
						Dim c As Integer = cpr(cmp, toKey, hi)
						If c > 0 OrElse (c = 0 AndAlso (Not hiInclusive) AndAlso toInclusive) Then Throw New IllegalArgumentException("key out of range")
					End If
				End If
				Return New SubMap(Of K, V)(m, fromKey, fromInclusive, toKey, toInclusive, isDescending)
			End Function

			Public Function subMap(ByVal fromKey As K, ByVal fromInclusive As Boolean, ByVal toKey As K, ByVal toInclusive As Boolean) As SubMap(Of K, V)
				If fromKey Is Nothing OrElse toKey Is Nothing Then Throw New NullPointerException
				Return newSubMap(fromKey, fromInclusive, toKey, toInclusive)
			End Function

			Public Function headMap(ByVal toKey As K, ByVal inclusive As Boolean) As SubMap(Of K, V)
				If toKey Is Nothing Then Throw New NullPointerException
				Return newSubMap(Nothing, False, toKey, inclusive)
			End Function

			Public Function tailMap(ByVal fromKey As K, ByVal inclusive As Boolean) As SubMap(Of K, V)
				If fromKey Is Nothing Then Throw New NullPointerException
				Return newSubMap(fromKey, inclusive, Nothing, False)
			End Function

			Public Function subMap(ByVal fromKey As K, ByVal toKey As K) As SubMap(Of K, V)
				Return subMap(fromKey, True, toKey, False)
			End Function

			Public Function headMap(ByVal toKey As K) As SubMap(Of K, V)
				Return headMap(toKey, False)
			End Function

			Public Function tailMap(ByVal fromKey As K) As SubMap(Of K, V)
				Return tailMap(fromKey, True)
			End Function

			Public Function descendingMap() As SubMap(Of K, V)
				Return New SubMap(Of K, V)(m, lo, loInclusive, hi, hiInclusive, (Not isDescending))
			End Function

			' ----------------  Relational methods -------------- 

			Public Function ceilingEntry(ByVal key As K) As KeyValuePair(Of K, V)
				Return getNearEntry(key, GT Or EQ)
			End Function

			Public Function ceilingKey(ByVal key As K) As K
				Return getNearKey(key, GT Or EQ)
			End Function

			Public Function lowerEntry(ByVal key As K) As KeyValuePair(Of K, V)
				Return getNearEntry(key, LT)
			End Function

			Public Function lowerKey(ByVal key As K) As K
				Return getNearKey(key, LT)
			End Function

			Public Function floorEntry(ByVal key As K) As KeyValuePair(Of K, V)
				Return getNearEntry(key, LT Or EQ)
			End Function

			Public Function floorKey(ByVal key As K) As K
				Return getNearKey(key, LT Or EQ)
			End Function

			Public Function higherEntry(ByVal key As K) As KeyValuePair(Of K, V)
				Return getNearEntry(key, GT)
			End Function

			Public Function higherKey(ByVal key As K) As K
				Return getNearKey(key, GT)
			End Function

			Public Function firstKey() As K
				Return If(isDescending, highestKey(), lowestKey())
			End Function

			Public Function lastKey() As K
				Return If(isDescending, lowestKey(), highestKey())
			End Function

			Public Function firstEntry() As KeyValuePair(Of K, V)
				Return If(isDescending, highestEntry(), lowestEntry())
			End Function

			Public Function lastEntry() As KeyValuePair(Of K, V)
				Return If(isDescending, lowestEntry(), highestEntry())
			End Function

			Public Function pollFirstEntry() As KeyValuePair(Of K, V)
				Return If(isDescending, removeHighest(), removeLowest())
			End Function

			Public Function pollLastEntry() As KeyValuePair(Of K, V)
				Return If(isDescending, removeLowest(), removeHighest())
			End Function

			' ---------------- Submap Views -------------- 

			Public Function keySet() As java.util.NavigableSet(Of K)
				Dim ks As KeySet(Of K) = keySetView
					If ks IsNot Nothing Then
						Return ks
					Else
'JAVA TO VB CONVERTER TODO TASK: Assignments within expressions are not supported in VB
						Return (keySetView = New KeySet(Of K)(Me))
					End If
			End Function

			Public Function navigableKeySet() As java.util.NavigableSet(Of K)
				Dim ks As KeySet(Of K) = keySetView
					If ks IsNot Nothing Then
						Return ks
					Else
'JAVA TO VB CONVERTER TODO TASK: Assignments within expressions are not supported in VB
						Return (keySetView = New KeySet(Of K)(Me))
					End If
			End Function

			Public Function values() As ICollection(Of V)
				Dim vs As ICollection(Of V) = valuesView
					If vs IsNot Nothing Then
						Return vs
					Else
'JAVA TO VB CONVERTER TODO TASK: Assignments within expressions are not supported in VB
						Return (valuesView = New Values(Of V)(Me))
					End If
			End Function

			Public Function entrySet() As java.util.Set(Of KeyValuePair(Of K, V))
				Dim es As java.util.Set(Of KeyValuePair(Of K, V)) = entrySetView
					If es IsNot Nothing Then
						Return es
					Else
'JAVA TO VB CONVERTER TODO TASK: Assignments within expressions are not supported in VB
						Return (entrySetView = New EntrySet(Of K, V)(Me))
					End If
			End Function

			Public Function descendingKeySet() As java.util.NavigableSet(Of K)
				Return descendingMap().navigableKeySet()
			End Function

			Friend Function keyIterator() As IEnumerator(Of K)
				Return New SubMapKeyIterator(Me)
			End Function

			Friend Function valueIterator() As IEnumerator(Of V)
				Return New SubMapValueIterator(Me)
			End Function

			Friend Function entryIterator() As IEnumerator(Of KeyValuePair(Of K, V))
				Return New SubMapEntryIterator(Me)
			End Function

			''' <summary>
			''' Variant of main Iter class to traverse through submaps.
			''' Also serves as back-up Spliterator for views
			''' </summary>
			Friend MustInherit Class SubMapIter(Of T)
				Implements IEnumerator(Of T), java.util.Spliterator(Of T)

				Private ReadOnly outerInstance As ConcurrentSkipListMap.SubMap

				''' <summary>
				''' the last node returned by next() </summary>
				Friend lastReturned As Node(Of K, V)
				''' <summary>
				''' the next node to return from next(); </summary>
				Friend [next] As Node(Of K, V)
				''' <summary>
				''' Cache of next value field to maintain weak consistency </summary>
				Friend nextValue As V

				Friend Sub New(ByVal outerInstance As ConcurrentSkipListMap.SubMap)
						Me.outerInstance = outerInstance
'JAVA TO VB CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
					Dim cmp As IComparer(Of ?) = outerInstance.m.comparator
					Do
						[next] = If(outerInstance.isDescending, outerInstance.hiNode(cmp), outerInstance.loNode(cmp))
						If [next] Is Nothing Then Exit Do
						Dim x As Object = [next].value
						If x IsNot Nothing AndAlso x IsNot [next] Then
							If Not outerInstance.inBounds([next].key, cmp) Then
								[next] = Nothing
							Else
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
								Dim vv As V = CType(x, V)
								nextValue = vv
							End If
							Exit Do
						End If
					Loop
				End Sub

				Public Function hasNext() As Boolean
					Return [next] IsNot Nothing
				End Function

				Friend Sub advance()
					If [next] Is Nothing Then Throw New java.util.NoSuchElementException
					lastReturned = [next]
					If outerInstance.isDescending Then
						descend()
					Else
						ascend()
					End If
				End Sub

				Private Sub ascend()
'JAVA TO VB CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
					Dim cmp As IComparer(Of ?) = outerInstance.m.comparator
					Do
						[next] = [next].next
						If [next] Is Nothing Then Exit Do
						Dim x As Object = [next].value
						If x IsNot Nothing AndAlso x IsNot [next] Then
							If outerInstance.tooHigh([next].key, cmp) Then
								[next] = Nothing
							Else
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
								Dim vv As V = CType(x, V)
								nextValue = vv
							End If
							Exit Do
						End If
					Loop
				End Sub

				Private Sub descend()
'JAVA TO VB CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
					Dim cmp As IComparer(Of ?) = outerInstance.m.comparator
					Do
						[next] = outerInstance.m.findNear(lastReturned.key, LT, cmp)
						If [next] Is Nothing Then Exit Do
						Dim x As Object = [next].value
						If x IsNot Nothing AndAlso x IsNot [next] Then
							If outerInstance.tooLow([next].key, cmp) Then
								[next] = Nothing
							Else
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
								Dim vv As V = CType(x, V)
								nextValue = vv
							End If
							Exit Do
						End If
					Loop
				End Sub

				Public Overridable Sub remove()
					Dim l As Node(Of K, V) = lastReturned
					If l Is Nothing Then Throw New IllegalStateException
					outerInstance.m.remove(l.key)
					lastReturned = Nothing
				End Sub

				Public Overridable Function trySplit() As java.util.Spliterator(Of T)
					Return Nothing
				End Function

'JAVA TO VB CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
				Public Overridable Function tryAdvance(Of T1)(ByVal action As java.util.function.Consumer(Of T1)) As Boolean
					If hasNext() Then
						action.accept(next())
						Return True
					End If
					Return False
				End Function

'JAVA TO VB CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
				Public Overridable Sub forEachRemaining(Of T1)(ByVal action As java.util.function.Consumer(Of T1))
					Do While hasNext()
						action.accept(next())
					Loop
				End Sub

				Public Overridable Function estimateSize() As Long
					Return java.lang.[Long].Max_Value
				End Function

			End Class

			Friend NotInheritable Class SubMapValueIterator
				Inherits SubMapIter(Of V)

				Private ReadOnly outerInstance As ConcurrentSkipListMap.SubMap

				Public Sub New(ByVal outerInstance As ConcurrentSkipListMap.SubMap)
					Me.outerInstance = outerInstance
				End Sub

				Public Function [next]() As V
					Dim v As V = nextValue
					advance()
					Return v
				End Function
				Public Function characteristics() As Integer
					Return 0
				End Function
			End Class

			Friend NotInheritable Class SubMapKeyIterator
				Inherits SubMapIter(Of K)

				Private ReadOnly outerInstance As ConcurrentSkipListMap.SubMap

				Public Sub New(ByVal outerInstance As ConcurrentSkipListMap.SubMap)
					Me.outerInstance = outerInstance
				End Sub

				Public Function [next]() As K
					Dim n As Node(Of K, V) = [next]
					advance()
					Return n.key
				End Function
				Public Function characteristics() As Integer
					Return java.util.Spliterator.DISTINCT Or java.util.Spliterator.ORDERED Or java.util.Spliterator.SORTED
				End Function
'JAVA TO VB CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
				Public Property comparator As IComparer(Of ?)
					Get
						Return outerInstance.comparator()
					End Get
				End Property
			End Class

			Friend NotInheritable Class SubMapEntryIterator
				Inherits SubMapIter(Of KeyValuePair(Of K, V))

				Private ReadOnly outerInstance As ConcurrentSkipListMap.SubMap

				Public Sub New(ByVal outerInstance As ConcurrentSkipListMap.SubMap)
					Me.outerInstance = outerInstance
				End Sub

				Public Function [next]() As KeyValuePair(Of K, V)
					Dim n As Node(Of K, V) = [next]
					Dim v As V = nextValue
					advance()
					Return New java.util.AbstractMap.SimpleImmutableEntry(Of K, V)(n.key, v)
				End Function
				Public Function characteristics() As Integer
					Return java.util.Spliterator.DISTINCT
				End Function
			End Class
		End Class

		' default Map method overrides

'JAVA TO VB CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
		Public Overridable Sub forEach(Of T1)(ByVal action As java.util.function.BiConsumer(Of T1))
			If action Is Nothing Then Throw New NullPointerException
			Dim v As V
			Dim n As Node(Of K, V) = findFirst()
			Do While n IsNot Nothing
				v = n.validValue
				If v IsNot Nothing Then action.accept(n.key, v)
				n = n.next
			Loop
		End Sub

'JAVA TO VB CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
		Public Overridable Sub replaceAll(Of T1 As V)(ByVal [function] As java.util.function.BiFunction(Of T1))
			If [function] Is Nothing Then Throw New NullPointerException
			Dim v As V
			Dim n As Node(Of K, V) = findFirst()
			Do While n IsNot Nothing
				v = n.validValue
				Do While v IsNot Nothing
					Dim r As V = [function].apply(n.key, v)
					If r Is Nothing Then Throw New NullPointerException
					If n.casValue(v, r) Then Exit Do
					v = n.validValue
				Loop
				n = n.next
			Loop
		End Sub

		''' <summary>
		''' Base class providing common structure for Spliterators.
		''' (Although not all that much common functionality; as usual for
		''' view classes, details annoyingly vary in key, value, and entry
		''' subclasses in ways that are not worth abstracting out for
		''' internal classes.)
		''' 
		''' The basic split strategy is to recursively descend from top
		''' level, row by row, descending to next row when either split
		''' off, or the end of row is encountered. Control of the number of
		''' splits relies on some statistical estimation: The expected
		''' remaining number of elements of a skip list when advancing
		''' either across or down decreases by about 25%. To make this
		''' observation useful, we need to know initial size, which we
		''' don't. But we can just use  java.lang.[Integer].MAX_VALUE so that we
		''' don't prematurely zero out while splitting.
		''' </summary>
		Friend MustInherit Class CSLMSpliterator(Of K, V)
'JAVA TO VB CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
			Friend ReadOnly comparator As IComparer(Of ?)
			Friend ReadOnly fence As K ' exclusive upper bound for keys, or null if to end
			Friend row As Index(Of K, V) ' the level to split out
			Friend current As Node(Of K, V) ' current traversal node; initialize at origin
			Friend est As Integer ' pseudo-size estimate
'JAVA TO VB CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
			Friend Sub New(Of T1)(ByVal comparator As IComparer(Of T1), ByVal row As Index(Of K, V), ByVal origin As Node(Of K, V), ByVal fence As K, ByVal est As Integer)
				Me.comparator = comparator
				Me.row = row
				Me.current = origin
				Me.fence = fence
				Me.est = est
			End Sub

			Public Function estimateSize() As Long
				Return CLng(est)
			End Function
		End Class

		Friend NotInheritable Class KeySpliterator(Of K, V)
			Inherits CSLMSpliterator(Of K, V)
			Implements java.util.Spliterator(Of K)

'JAVA TO VB CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
			Friend Sub New(Of T1)(ByVal comparator As IComparer(Of T1), ByVal row As Index(Of K, V), ByVal origin As Node(Of K, V), ByVal fence As K, ByVal est As Integer)
				MyBase.New(comparator, row, origin, fence, est)
			End Sub

			Public Function trySplit() As java.util.Spliterator(Of K)
				Dim e As Node(Of K, V)
				Dim ek As K
'JAVA TO VB CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
				Dim cmp As IComparer(Of ?) = comparator
				Dim f As K = fence
				e = current
				ek = e.key
				If e IsNot Nothing AndAlso ek IsNot Nothing Then
					Dim q As Index(Of K, V) = row
					Do While q IsNot Nothing
						Dim s As Index(Of K, V)
						Dim b As Node(Of K, V), n As Node(Of K, V)
						Dim sk As K
						s = q.right
						b = s.node_Renamed
						n = b.next
						sk = n.key
						If s IsNot Nothing AndAlso b IsNot Nothing AndAlso n IsNot Nothing AndAlso n.value IsNot Nothing AndAlso sk IsNot Nothing AndAlso cpr(cmp, sk, ek) > 0 AndAlso (f Is Nothing OrElse cpr(cmp, sk, f) < 0) Then
							current = n
							Dim r As Index(Of K, V) = q.down
							row = If(s.right IsNot Nothing, s, s.down)
							est -= CInt(CUInt(est) >> 2)
							Return New KeySpliterator(Of K, V)(cmp, r, e, sk, est)
						End If
						row = q.down
q = row
					Loop
				End If
				Return Nothing
			End Function

'JAVA TO VB CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
			Public Sub forEachRemaining(Of T1)(ByVal action As java.util.function.Consumer(Of T1))
				If action Is Nothing Then Throw New NullPointerException
'JAVA TO VB CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
				Dim cmp As IComparer(Of ?) = comparator
				Dim f As K = fence
				Dim e As Node(Of K, V) = current
				current = Nothing
				Do While e IsNot Nothing
					Dim k As K
					Dim v As Object
					k = e.key
					If k IsNot Nothing AndAlso f IsNot Nothing AndAlso cpr(cmp, f, k) <= 0 Then Exit Do
					v = e.value
					If v IsNot Nothing AndAlso v IsNot e Then action.accept(k)
					e = e.next
				Loop
			End Sub

'JAVA TO VB CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
			Public Function tryAdvance(Of T1)(ByVal action As java.util.function.Consumer(Of T1)) As Boolean
				If action Is Nothing Then Throw New NullPointerException
'JAVA TO VB CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
				Dim cmp As IComparer(Of ?) = comparator
				Dim f As K = fence
				Dim e As Node(Of K, V) = current
				Do While e IsNot Nothing
					Dim k As K
					Dim v As Object
					k = e.key
					If k IsNot Nothing AndAlso f IsNot Nothing AndAlso cpr(cmp, f, k) <= 0 Then
						e = Nothing
						Exit Do
					End If
					v = e.value
					If v IsNot Nothing AndAlso v IsNot e Then
						current = e.next
						action.accept(k)
						Return True
					End If
					e = e.next
				Loop
				current = e
				Return False
			End Function

			Public Function characteristics() As Integer
				Return java.util.Spliterator.DISTINCT Or java.util.Spliterator.SORTED Or java.util.Spliterator.ORDERED Or java.util.Spliterator.CONCURRENT Or java.util.Spliterator.NONNULL
			End Function

'JAVA TO VB CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
			Public Property comparator As IComparer(Of ?)
				Get
					Return comparator
				End Get
			End Property
		End Class
		' factory method for KeySpliterator
		Friend Function keySpliterator() As KeySpliterator(Of K, V)
'JAVA TO VB CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
			Dim cmp As IComparer(Of ?) = comparator_Renamed
			Do ' ensure h corresponds to origin p
				Dim h As HeadIndex(Of K, V)
				Dim p As Node(Of K, V)
'JAVA TO VB CONVERTER TODO TASK: Assignments within expressions are not supported in VB
				Dim b As Node(Of K, V) = (h = head).node
				p = b.next
				If p Is Nothing OrElse p.value IsNot Nothing Then Return New KeySpliterator(Of K, V)(cmp, h, p, Nothing,If(p Is Nothing, 0,  java.lang.[Integer].Max_Value))
				p.helpDelete(b, p.next)
			Loop
		End Function

		Friend NotInheritable Class ValueSpliterator(Of K, V)
			Inherits CSLMSpliterator(Of K, V)
			Implements java.util.Spliterator(Of V)

'JAVA TO VB CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
			Friend Sub New(Of T1)(ByVal comparator As IComparer(Of T1), ByVal row As Index(Of K, V), ByVal origin As Node(Of K, V), ByVal fence As K, ByVal est As Integer)
				MyBase.New(comparator, row, origin, fence, est)
			End Sub

			Public Function trySplit() As java.util.Spliterator(Of V)
				Dim e As Node(Of K, V)
				Dim ek As K
'JAVA TO VB CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
				Dim cmp As IComparer(Of ?) = comparator
				Dim f As K = fence
				e = current
				ek = e.key
				If e IsNot Nothing AndAlso ek IsNot Nothing Then
					Dim q As Index(Of K, V) = row
					Do While q IsNot Nothing
						Dim s As Index(Of K, V)
						Dim b As Node(Of K, V), n As Node(Of K, V)
						Dim sk As K
						s = q.right
						b = s.node_Renamed
						n = b.next
						sk = n.key
						If s IsNot Nothing AndAlso b IsNot Nothing AndAlso n IsNot Nothing AndAlso n.value IsNot Nothing AndAlso sk IsNot Nothing AndAlso cpr(cmp, sk, ek) > 0 AndAlso (f Is Nothing OrElse cpr(cmp, sk, f) < 0) Then
							current = n
							Dim r As Index(Of K, V) = q.down
							row = If(s.right IsNot Nothing, s, s.down)
							est -= CInt(CUInt(est) >> 2)
							Return New ValueSpliterator(Of K, V)(cmp, r, e, sk, est)
						End If
						row = q.down
q = row
					Loop
				End If
				Return Nothing
			End Function

'JAVA TO VB CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
			Public Sub forEachRemaining(Of T1)(ByVal action As java.util.function.Consumer(Of T1))
				If action Is Nothing Then Throw New NullPointerException
'JAVA TO VB CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
				Dim cmp As IComparer(Of ?) = comparator
				Dim f As K = fence
				Dim e As Node(Of K, V) = current
				current = Nothing
				Do While e IsNot Nothing
					Dim k As K
					Dim v As Object
					k = e.key
					If k IsNot Nothing AndAlso f IsNot Nothing AndAlso cpr(cmp, f, k) <= 0 Then Exit Do
					v = e.value
					If v IsNot Nothing AndAlso v IsNot e Then
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
						Dim vv As V = CType(v, V)
						action.accept(vv)
					End If
					e = e.next
				Loop
			End Sub

'JAVA TO VB CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
			Public Function tryAdvance(Of T1)(ByVal action As java.util.function.Consumer(Of T1)) As Boolean
				If action Is Nothing Then Throw New NullPointerException
'JAVA TO VB CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
				Dim cmp As IComparer(Of ?) = comparator
				Dim f As K = fence
				Dim e As Node(Of K, V) = current
				Do While e IsNot Nothing
					Dim k As K
					Dim v As Object
					k = e.key
					If k IsNot Nothing AndAlso f IsNot Nothing AndAlso cpr(cmp, f, k) <= 0 Then
						e = Nothing
						Exit Do
					End If
					v = e.value
					If v IsNot Nothing AndAlso v IsNot e Then
						current = e.next
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
						Dim vv As V = CType(v, V)
						action.accept(vv)
						Return True
					End If
					e = e.next
				Loop
				current = e
				Return False
			End Function

			Public Function characteristics() As Integer
				Return java.util.Spliterator.CONCURRENT Or java.util.Spliterator.ORDERED Or java.util.Spliterator.NONNULL
			End Function
		End Class

		' Almost the same as keySpliterator()
		Friend Function valueSpliterator() As ValueSpliterator(Of K, V)
'JAVA TO VB CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
			Dim cmp As IComparer(Of ?) = comparator_Renamed
			Do
				Dim h As HeadIndex(Of K, V)
				Dim p As Node(Of K, V)
'JAVA TO VB CONVERTER TODO TASK: Assignments within expressions are not supported in VB
				Dim b As Node(Of K, V) = (h = head).node
				p = b.next
				If p Is Nothing OrElse p.value IsNot Nothing Then Return New ValueSpliterator(Of K, V)(cmp, h, p, Nothing,If(p Is Nothing, 0,  java.lang.[Integer].Max_Value))
				p.helpDelete(b, p.next)
			Loop
		End Function

		Friend NotInheritable Class EntrySpliterator(Of K, V)
			Inherits CSLMSpliterator(Of K, V)
			Implements java.util.Spliterator(Of KeyValuePair(Of K, V))

'JAVA TO VB CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
			Friend Sub New(Of T1)(ByVal comparator As IComparer(Of T1), ByVal row As Index(Of K, V), ByVal origin As Node(Of K, V), ByVal fence As K, ByVal est As Integer)
				MyBase.New(comparator, row, origin, fence, est)
			End Sub

			Public Function trySplit() As java.util.Spliterator(Of KeyValuePair(Of K, V))
				Dim e As Node(Of K, V)
				Dim ek As K
'JAVA TO VB CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
				Dim cmp As IComparer(Of ?) = comparator
				Dim f As K = fence
				e = current
				ek = e.key
				If e IsNot Nothing AndAlso ek IsNot Nothing Then
					Dim q As Index(Of K, V) = row
					Do While q IsNot Nothing
						Dim s As Index(Of K, V)
						Dim b As Node(Of K, V), n As Node(Of K, V)
						Dim sk As K
						s = q.right
						b = s.node_Renamed
						n = b.next
						sk = n.key
						If s IsNot Nothing AndAlso b IsNot Nothing AndAlso n IsNot Nothing AndAlso n.value IsNot Nothing AndAlso sk IsNot Nothing AndAlso cpr(cmp, sk, ek) > 0 AndAlso (f Is Nothing OrElse cpr(cmp, sk, f) < 0) Then
							current = n
							Dim r As Index(Of K, V) = q.down
							row = If(s.right IsNot Nothing, s, s.down)
							est -= CInt(CUInt(est) >> 2)
							Return New EntrySpliterator(Of K, V)(cmp, r, e, sk, est)
						End If
						row = q.down
q = row
					Loop
				End If
				Return Nothing
			End Function

'JAVA TO VB CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
			Public Sub forEachRemaining(Of T1)(ByVal action As java.util.function.Consumer(Of T1))
				If action Is Nothing Then Throw New NullPointerException
'JAVA TO VB CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
				Dim cmp As IComparer(Of ?) = comparator
				Dim f As K = fence
				Dim e As Node(Of K, V) = current
				current = Nothing
				Do While e IsNot Nothing
					Dim k As K
					Dim v As Object
					k = e.key
					If k IsNot Nothing AndAlso f IsNot Nothing AndAlso cpr(cmp, f, k) <= 0 Then Exit Do
					v = e.value
					If v IsNot Nothing AndAlso v IsNot e Then
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
						Dim vv As V = CType(v, V)
						action.accept(New java.util.AbstractMap.SimpleImmutableEntry(Of K, V)(k, vv))
					End If
					e = e.next
				Loop
			End Sub

'JAVA TO VB CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
			Public Function tryAdvance(Of T1)(ByVal action As java.util.function.Consumer(Of T1)) As Boolean
				If action Is Nothing Then Throw New NullPointerException
'JAVA TO VB CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
				Dim cmp As IComparer(Of ?) = comparator
				Dim f As K = fence
				Dim e As Node(Of K, V) = current
				Do While e IsNot Nothing
					Dim k As K
					Dim v As Object
					k = e.key
					If k IsNot Nothing AndAlso f IsNot Nothing AndAlso cpr(cmp, f, k) <= 0 Then
						e = Nothing
						Exit Do
					End If
					v = e.value
					If v IsNot Nothing AndAlso v IsNot e Then
						current = e.next
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
						Dim vv As V = CType(v, V)
						action.accept(New java.util.AbstractMap.SimpleImmutableEntry(Of K, V)(k, vv))
						Return True
					End If
					e = e.next
				Loop
				current = e
				Return False
			End Function

			Public Function characteristics() As Integer
				Return java.util.Spliterator.DISTINCT Or java.util.Spliterator.SORTED Or java.util.Spliterator.ORDERED Or java.util.Spliterator.CONCURRENT Or java.util.Spliterator.NONNULL
			End Function

			Public Property comparator As IComparer(Of KeyValuePair(Of K, V))
				Get
					' Adapt or create a key-based comparator
					If comparator IsNot Nothing Then
						Return DictionaryEntry.comparingByKey(comparator)
					Else
						Return (IComparer(Of KeyValuePair(Of K, V)) And java.io.Serializable)(e1, e2) ->
	'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
	'JAVA TO VB CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
	'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
							Dim k1 As Comparable(Of ?) = CType(e1.key, Comparable(Of ?))
							Return k1.CompareTo(e2.key)
					End If
				End Get
			End Property
		End Class

		' Almost the same as keySpliterator()
		Friend Function entrySpliterator() As EntrySpliterator(Of K, V)
'JAVA TO VB CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
			Dim cmp As IComparer(Of ?) = comparator_Renamed
			Do ' almost same as key version
				Dim h As HeadIndex(Of K, V)
				Dim p As Node(Of K, V)
'JAVA TO VB CONVERTER TODO TASK: Assignments within expressions are not supported in VB
				Dim b As Node(Of K, V) = (h = head).node
				p = b.next
				If p Is Nothing OrElse p.value IsNot Nothing Then Return New EntrySpliterator(Of K, V)(cmp, h, p, Nothing,If(p Is Nothing, 0,  java.lang.[Integer].Max_Value))
				p.helpDelete(b, p.next)
			Loop
		End Function

		' Unsafe mechanics
		Private Shared ReadOnly UNSAFE As sun.misc.Unsafe
		Private Shared ReadOnly headOffset As Long
		Private Shared ReadOnly SECONDARY As Long
		Shared Sub New()
			Try
				UNSAFE = sun.misc.Unsafe.unsafe
				Dim k As  [Class] = GetType(ConcurrentSkipListMap)
				headOffset = UNSAFE.objectFieldOffset(k.getDeclaredField("head"))
				Dim tk As  [Class] = GetType(Thread)
				SECONDARY = UNSAFE.objectFieldOffset(tk.getDeclaredField("threadLocalRandomSecondarySeed"))

			Catch e As Exception
				Throw New [Error](e)
			End Try
		End Sub
	End Class

End Namespace