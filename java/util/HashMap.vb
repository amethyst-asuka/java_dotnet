Imports Microsoft.VisualBasic
Imports System
Imports System.Diagnostics
Imports System.Collections
Imports System.Collections.Generic
Imports java.util

'
' * Copyright (c) 1997, 2013, Oracle and/or its affiliates. All rights reserved.
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
    ''' Hash table based implementation of the <tt>Map</tt> interface.  This
    ''' implementation provides all of the optional map operations, and permits
    ''' <tt>null</tt> values and the <tt>null</tt> key.  (The <tt>HashMap</tt>
    ''' class is roughly equivalent to <tt>Hashtable</tt>, except that it is
    ''' unsynchronized and permits nulls.)  This class makes no guarantees as to
    ''' the order of the map; in particular, it does not guarantee that the order
    ''' will remain constant over time.
    ''' 
    ''' <p>This implementation provides constant-time performance for the basic
    ''' operations (<tt>get</tt> and <tt>put</tt>), assuming the hash function
    ''' disperses the elements properly among the buckets.  Iteration over
    ''' collection views requires time proportional to the "capacity" of the
    ''' <tt>HashMap</tt> instance (the number of buckets) plus its size (the number
    ''' of key-value mappings).  Thus, it's very important not to set the initial
    ''' capacity too high (or the load factor too low) if iteration performance is
    ''' important.
    ''' 
    ''' <p>An instance of <tt>HashMap</tt> has two parameters that affect its
    ''' performance: <i>initial capacity</i> and <i>load factor</i>.  The
    ''' <i>capacity</i> is the number of buckets in the hash table, and the initial
    ''' capacity is simply the capacity at the time the hash table is created.  The
    ''' <i>load factor</i> is a measure of how full the hash table is allowed to
    ''' get before its capacity is automatically increased.  When the number of
    ''' entries in the hash table exceeds the product of the load factor and the
    ''' current capacity, the hash table is <i>rehashed</i> (that is, internal data
    ''' structures are rebuilt) so that the hash table has approximately twice the
    ''' number of buckets.
    ''' 
    ''' <p>As a general rule, the default load factor (.75) offers a good
    ''' tradeoff between time and space costs.  Higher values decrease the
    ''' space overhead but increase the lookup cost (reflected in most of
    ''' the operations of the <tt>HashMap</tt> [Class], including
    ''' <tt>get</tt> and <tt>put</tt>).  The expected number of entries in
    ''' the map and its load factor should be taken into account when
    ''' setting its initial capacity, so as to minimize the number of
    ''' rehash operations.  If the initial capacity is greater than the
    ''' maximum number of entries divided by the load factor, no rehash
    ''' operations will ever occur.
    ''' 
    ''' <p>If many mappings are to be stored in a <tt>HashMap</tt>
    ''' instance, creating it with a sufficiently large capacity will allow
    ''' the mappings to be stored more efficiently than letting it perform
    ''' automatic rehashing as needed to grow the table.  Note that using
    ''' many keys with the same {@code hashCode()} is a sure way to slow
    ''' down performance of any hash table. To ameliorate impact, when keys
    ''' are <seealso cref="Comparable"/>, this class may use comparison order among
    ''' keys to help break ties.
    ''' 
    ''' <p><strong>Note that this implementation is not synchronized.</strong>
    ''' If multiple threads access a hash map concurrently, and at least one of
    ''' the threads modifies the map structurally, it <i>must</i> be
    ''' synchronized externally.  (A structural modification is any operation
    ''' that adds or deletes one or more mappings; merely changing the value
    ''' associated with a key that an instance already contains is not a
    ''' structural modification.)  This is typically accomplished by
    ''' synchronizing on some object that naturally encapsulates the map.
    ''' 
    ''' If no such object exists, the map should be "wrapped" using the
    ''' <seealso cref="Collections#synchronizedMap Collections.synchronizedMap"/>
    ''' method.  This is best done at creation time, to prevent accidental
    ''' unsynchronized access to the map:<pre>
    '''   Map m = Collections.synchronizedMap(new HashMap(...));</pre>
    ''' 
    ''' <p>The iterators returned by all of this class's "collection view methods"
    ''' are <i>fail-fast</i>: if the map is structurally modified at any time after
    ''' the iterator is created, in any way except through the iterator's own
    ''' <tt>remove</tt> method, the iterator will throw a
    ''' <seealso cref="ConcurrentModificationException"/>.  Thus, in the face of concurrent
    ''' modification, the iterator fails quickly and cleanly, rather than risking
    ''' arbitrary, non-deterministic behavior at an undetermined time in the
    ''' future.
    ''' 
    ''' <p>Note that the fail-fast behavior of an iterator cannot be guaranteed
    ''' as it is, generally speaking, impossible to make any hard guarantees in the
    ''' presence of unsynchronized concurrent modification.  Fail-fast iterators
    ''' throw <tt>ConcurrentModificationException</tt> on a best-effort basis.
    ''' Therefore, it would be wrong to write a program that depended on this
    ''' exception for its correctness: <i>the fail-fast behavior of iterators
    ''' should be used only to detect bugs.</i>
    ''' 
    ''' <p>This class is a member of the
    ''' <a href="{@docRoot}/../technotes/guides/collections/index.html">
    ''' Java Collections Framework</a>.
    ''' </summary>
    ''' @param <K> the type of keys maintained by this map </param>
    ''' @param <V> the type of mapped values
    ''' 
    ''' @author  Doug Lea
    ''' @author  Josh Bloch
    ''' @author  Arthur van Hoff
    ''' @author  Neal Gafter </param>
    ''' <seealso cref=     Object#hashCode() </seealso>
    ''' <seealso cref=     Collection </seealso>
    ''' <seealso cref=     Map </seealso>
    ''' <seealso cref=     TreeMap </seealso>
    ''' <seealso cref=     Hashtable
    ''' @since   1.2 </seealso>
    <Serializable>
    Public Class HashMap(Of K, V)
        Inherits AbstractMap(Of K, V)
        Implements Map(Of K, V), Cloneable

        Private Const serialVersionUID As Long = 362498820763181265L

        '    
        '     * Implementation notes.
        '     *
        '     * This map usually acts as a binned (bucketed) hash table, but
        '     * when bins get too large, they are transformed into bins of
        '     * TreeNodes, each structured similarly to those in
        '     * java.util.TreeMap. Most methods try to use normal bins, but
        '     * relay to TreeNode methods when applicable (simply by checking
        '     * instanceof a node).  Bins of TreeNodes may be traversed and
        '     * used like any others, but additionally support faster lookup
        '     * when overpopulated. However, since the vast majority of bins in
        '     * normal use are not overpopulated, checking for existence of
        '     * tree bins may be delayed in the course of table methods.
        '     *
        '     * Tree bins (i.e., bins whose elements are all TreeNodes) are
        '     * ordered primarily by hashCode, but in the case of ties, if two
        '     * elements are of the same "class C implements Comparable<C>",
        '     * type then their compareTo method is used for ordering. (We
        '     * conservatively check generic types via reflection to validate
        '     * this -- see method comparableClassFor).  The added complexity
        '     * of tree bins is worthwhile in providing worst-case O(log n)
        '     * operations when keys either have distinct hashes or are
        '     * orderable, Thus, performance degrades gracefully under
        '     * accidental or malicious usages in which hashCode() methods
        '     * return values that are poorly distributed, as well as those in
        '     * which many keys share a hashCode, so long as they are also
        '     * Comparable. (If neither of these apply, we may waste about a
        '     * factor of two in time and space compared to taking no
        '     * precautions. But the only known cases stem from poor user
        '     * programming practices that are already so slow that this makes
        '     * little difference.)
        '     *
        '     * Because TreeNodes are about twice the size of regular nodes, we
        '     * use them only when bins contain enough nodes to warrant use
        '     * (see TREEIFY_THRESHOLD). And when they become too small (due to
        '     * removal or resizing) they are converted back to plain bins.  In
        '     * usages with well-distributed user hashCodes, tree bins are
        '     * rarely used.  Ideally, under random hashCodes, the frequency of
        '     * nodes in bins follows a Poisson distribution
        '     * (http://en.wikipedia.org/wiki/Poisson_distribution) with a
        '     * parameter of about 0.5 on average for the default resizing
        '     * threshold of 0.75, although with a large variance because of
        '     * resizing granularity. Ignoring variance, the expected
        '     * occurrences of list size k are (exp(-0.5) * pow(0.5, k) /
        '     * factorial(k)). The first values are:
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
        '     * The root of a tree bin is normally its first node.  However,
        '     * sometimes (currently only upon Iterator.remove), the root might
        '     * be elsewhere, but can be recovered following parent links
        '     * (method TreeNode.root()).
        '     *
        '     * All applicable internal methods accept a hash code as an
        '     * argument (as normally supplied from a public method), allowing
        '     * them to call each other without recomputing user hashCodes.
        '     * Most internal methods also accept a "tab" argument, that is
        '     * normally the current table, but may be a new or old one when
        '     * resizing or converting.
        '     *
        '     * When bin lists are treeified, split, or untreeified, we keep
        '     * them in the same relative access/traversal order (i.e., field
        '     * Node.next) to better preserve locality, and to slightly
        '     * simplify handling of splits and traversals that invoke
        '     * iterator.remove. When using comparators on insertion, to keep a
        '     * total ordering (or as close as is required here) across
        '     * rebalancings, we compare classes and identityHashCodes as
        '     * tie-breakers.
        '     *
        '     * The use and transitions among plain vs tree modes is
        '     * complicated by the existence of subclass LinkedHashMap. See
        '     * below for hook methods defined to be invoked upon insertion,
        '     * removal and access that allow LinkedHashMap internals to
        '     * otherwise remain independent of these mechanics. (This also
        '     * requires that a map instance be passed to some utility methods
        '     * that may create new nodes.)
        '     *
        '     * The concurrent-programming-like SSA-based coding style helps
        '     * avoid aliasing errors amid all of the twisty pointer operations.
        '     

        ''' <summary>
        ''' The default initial capacity - MUST be a power of two.
        ''' </summary>
        Friend Shared ReadOnly DEFAULT_INITIAL_CAPACITY As Integer = 1 << 4 ' aka 16

        ''' <summary>
        ''' The maximum capacity, used if a higher value is implicitly specified
        ''' by either of the constructors with arguments.
        ''' MUST be a power of two <= 1<<30.
        ''' </summary>
        Friend Shared ReadOnly MAXIMUM_CAPACITY As Integer = 1 << 30

        ''' <summary>
        ''' The load factor used when none specified in constructor.
        ''' </summary>
        Friend Const DEFAULT_LOAD_FACTOR As Single = 0.75F

        ''' <summary>
        ''' The bin count threshold for using a tree rather than list for a
        ''' bin.  Bins are converted to trees when adding an element to a
        ''' bin with at least this many nodes. The value must be greater
        ''' than 2 and should be at least 8 to mesh with assumptions in
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
        ''' Should be at least 4 * TREEIFY_THRESHOLD to avoid conflicts
        ''' between resizing and treeification thresholds.
        ''' </summary>
        Friend Const MIN_TREEIFY_CAPACITY As Integer = 64

        ''' <summary>
        ''' Basic hash bin node, used for most entries.  (See below for
        ''' TreeNode subclass, and in LinkedHashMap for its Entry subclass.)
        ''' </summary>
        Friend Class Node(Of K, V)
            Implements KeyValuePair(Of K, V)

            Friend ReadOnly hash As Integer
            Friend ReadOnly key As K
            Friend value As V
            Friend [next] As Node(Of K, V)

            Friend Sub New(ByVal hash As Integer, ByVal key As K, ByVal value As V, ByVal [next] As Node(Of K, V))
                Me.hash = hash
                Me.key = key
                Me.value = value
                Me.next = [next]
            End Sub

            Public Property key As K
                Get
                    Return key
                End Get
            End Property
            Public Property value As V
                Get
                    Return value
                End Get
            End Property
            Public NotOverridable Overrides Function ToString() As String
                Return key & "=" & value
            End Function

            Public NotOverridable Overrides Function GetHashCode() As Integer
                Return Objects.hashCode(key) Xor Objects.hashCode(value)
            End Function

            Public Function setValue(ByVal newValue As V) As V
                Dim oldValue As V = value
                value = newValue
                Return oldValue
            End Function

            Public NotOverridable Overrides Function Equals(ByVal o As Object) As Boolean
                If o Is Me Then Return True
                If TypeOf o Is DictionaryEntry Then
                    'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
                    Dim e As KeyValuePair(Of ?, ?) = CType(o, KeyValuePair(Of ?, ?))
                    If Objects.Equals(key, e.Key) AndAlso Objects.Equals(value, e.Value) Then Return True
                End If
                Return False
            End Function
        End Class

        ' ---------------- Static utilities -------------- 

        ''' <summary>
        ''' Computes key.hashCode() and spreads (XORs) higher bits of hash
        ''' to lower.  Because the table uses power-of-two masking, sets of
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
        Friend Shared Function hash(ByVal key As Object) As Integer
            Dim h As Integer
            If key Is Nothing Then
                Return 0
            Else
                'JAVA TO VB CONVERTER TODO TASK: Assignments within expressions are not supported in VB
                Return (h = key.GetHashCode()) Xor (CInt(CUInt(h) >> 16))
            End If
        End Function

        ''' <summary>
        ''' Returns x's Class if it is of the form "class C implements
        ''' Comparable<C>", else null.
        ''' </summary>
        Friend Shared Function comparableClassFor(ByVal x As Object) As [Class]
            If TypeOf x Is Comparable Then
                Dim c As [Class]
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
                            If (TypeOf t Is ParameterizedType) AndAlso (p.rawType = GetType(Comparable)) AndAlso [as] IsNot Nothing AndAlso [as].Length = 1 AndAlso [as](0) Is c Then ' type arg is c Return c
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

        ''' <summary>
        ''' Returns a power of two size for the given target capacity.
        ''' </summary>
        Friend Shared Function tableSizeFor(ByVal cap As Integer) As Integer
            Dim n As Integer = cap - 1
            n = n Or CInt(CUInt(n) >> 1)
            n = n Or CInt(CUInt(n) >> 2)
            n = n Or CInt(CUInt(n) >> 4)
            n = n Or CInt(CUInt(n) >> 8)
            n = n Or CInt(CUInt(n) >> 16)
            Return If(n < 0, 1, If(n >= MAXIMUM_CAPACITY, MAXIMUM_CAPACITY, n + 1))
        End Function

        ' ---------------- Fields -------------- 

        ''' <summary>
        ''' The table, initialized on first use, and resized as
        ''' necessary. When allocated, length is always a power of two.
        ''' (We also tolerate length zero in some operations to allow
        ''' bootstrapping mechanics that are currently not needed.)
        ''' </summary>
        <NonSerialized>
        Friend table As Node(Of K, V)()

        ''' <summary>
        ''' Holds cached entrySet(). Note that AbstractMap fields are used
        ''' for keySet() and values().
        ''' </summary>
        <NonSerialized>
        Friend entrySet_Renamed As [Set](Of KeyValuePair(Of K, V))

        ''' <summary>
        ''' The number of key-value mappings contained in this map.
        ''' </summary>
        <NonSerialized>
        Friend size_Renamed As Integer

        ''' <summary>
        ''' The number of times this HashMap has been structurally modified
        ''' Structural modifications are those that change the number of mappings in
        ''' the HashMap or otherwise modify its internal structure (e.g.,
        ''' rehash).  This field is used to make iterators on Collection-views of
        ''' the HashMap fail-fast.  (See ConcurrentModificationException).
        ''' </summary>
        <NonSerialized>
        Friend modCount As Integer

        ''' <summary>
        ''' The next size value at which to resize (capacity * load factor).
        ''' 
        ''' @serial
        ''' </summary>
        ' (The javadoc description is true upon serialization.
        ' Additionally, if the table array has not been allocated, this
        ' field holds the initial array capacity, or zero signifying
        ' DEFAULT_INITIAL_CAPACITY.)
        Friend threshold As Integer

        ''' <summary>
        ''' The load factor for the hash table.
        ''' 
        ''' @serial
        ''' </summary>
        Friend ReadOnly loadFactor_Renamed As Single

        ' ---------------- Public operations -------------- 

        ''' <summary>
        ''' Constructs an empty <tt>HashMap</tt> with the specified initial
        ''' capacity and load factor.
        ''' </summary>
        ''' <param name="initialCapacity"> the initial capacity </param>
        ''' <param name="loadFactor">      the load factor </param>
        ''' <exception cref="IllegalArgumentException"> if the initial capacity is negative
        '''         or the load factor is nonpositive </exception>
        Public Sub New(ByVal initialCapacity As Integer, ByVal loadFactor As Single)
            If initialCapacity < 0 Then Throw New IllegalArgumentException("Illegal initial capacity: " & initialCapacity)
            If initialCapacity > MAXIMUM_CAPACITY Then initialCapacity = MAXIMUM_CAPACITY
            If loadFactor <= 0 OrElse Float.IsNaN(loadFactor) Then Throw New IllegalArgumentException("Illegal load factor: " & loadFactor)
            Me.loadFactor_Renamed = loadFactor
            Me.threshold = tableSizeFor(initialCapacity)
        End Sub

        ''' <summary>
        ''' Constructs an empty <tt>HashMap</tt> with the specified initial
        ''' capacity and the default load factor (0.75).
        ''' </summary>
        ''' <param name="initialCapacity"> the initial capacity. </param>
        ''' <exception cref="IllegalArgumentException"> if the initial capacity is negative. </exception>
        Public Sub New(ByVal initialCapacity As Integer)
            Me.New(initialCapacity, DEFAULT_LOAD_FACTOR)
        End Sub

        ''' <summary>
        ''' Constructs an empty <tt>HashMap</tt> with the default initial capacity
        ''' (16) and the default load factor (0.75).
        ''' </summary>
        Public Sub New()
            Me.loadFactor_Renamed = DEFAULT_LOAD_FACTOR ' all other fields defaulted
        End Sub

        ''' <summary>
        ''' Constructs a new <tt>HashMap</tt> with the same mappings as the
        ''' specified <tt>Map</tt>.  The <tt>HashMap</tt> is created with
        ''' default load factor (0.75) and an initial capacity sufficient to
        ''' hold the mappings in the specified <tt>Map</tt>.
        ''' </summary>
        ''' <param name="m"> the map whose mappings are to be placed in this map </param>
        ''' <exception cref="NullPointerException"> if the specified map is null </exception>
        'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
        Public Sub New(Of T1 As K, ? As V)(ByVal m As Map(Of T1))
			Me.loadFactor_Renamed = DEFAULT_LOAD_FACTOR
            putMapEntries(m, False)
        End Sub

        ''' <summary>
        ''' Implements Map.putAll and Map constructor
        ''' </summary>
        ''' <param name="m"> the map </param>
        ''' <param name="evict"> false when initially constructing this map, else
        ''' true (relayed to method afterNodeInsertion). </param>
        'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
        Friend Sub putMapEntries(Of T1 As K, ? As V)(ByVal m As Map(Of T1), ByVal evict As Boolean)
			Dim s As Integer = m.size()
            If s > 0 Then
                If table Is Nothing Then ' pre-size
                    Dim ft As Single = (CSng(s) / loadFactor_Renamed) + 1.0F
                    Dim t As Integer = (If(ft < CSng(MAXIMUM_CAPACITY), CInt(Fix(ft)), MAXIMUM_CAPACITY))
                    If t > threshold Then threshold = tableSizeFor(t)
                ElseIf s > threshold Then
                    resize()
                End If
                'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
                For Each e As KeyValuePair(Of ? As K, ? As V) In m.entrySet()
                    Dim key As K = e.Key
                    Dim value As V = e.Value
                    putVal(hash(key), key, value, False, evict)
                Next e
            End If
        End Sub

        ''' <summary>
        ''' Returns the number of key-value mappings in this map.
        ''' </summary>
        ''' <returns> the number of key-value mappings in this map </returns>
        Public Overridable Function size() As Integer Implements Map(Of K, V).size
            Return size_Renamed
        End Function

        ''' <summary>
        ''' Returns <tt>true</tt> if this map contains no key-value mappings.
        ''' </summary>
        ''' <returns> <tt>true</tt> if this map contains no key-value mappings </returns>
        Public Overridable Property empty As Boolean Implements Map(Of K, V).isEmpty
            Get
                Return size_Renamed = 0
            End Get
        End Property

        ''' <summary>
        ''' Returns the value to which the specified key is mapped,
        ''' or {@code null} if this map contains no mapping for the key.
        ''' 
        ''' <p>More formally, if this map contains a mapping from a key
        ''' {@code k} to a value {@code v} such that {@code (key==null ? k==null :
        ''' key.equals(k))}, then this method returns {@code v}; otherwise
        ''' it returns {@code null}.  (There can be at most one such mapping.)
        ''' 
        ''' <p>A return value of {@code null} does not <i>necessarily</i>
        ''' indicate that the map contains no mapping for the key; it's also
        ''' possible that the map explicitly maps the key to {@code null}.
        ''' The <seealso cref="#containsKey containsKey"/> operation may be used to
        ''' distinguish these two cases.
        ''' </summary>
        ''' <seealso cref= #put(Object, Object) </seealso>
        Public Overridable Function [get](ByVal key As Object) As V Implements Map(Of K, V).get
            Dim e As Node(Of K, V)
            'JAVA TO VB CONVERTER TODO TASK: Assignments within expressions are not supported in VB
            Return If((e = getNode(hash(key), key)) Is Nothing, Nothing, e.value)
        End Function

        ''' <summary>
        ''' Implements Map.get and related methods
        ''' </summary>
        ''' <param name="hash"> hash for key </param>
        ''' <param name="key"> the key </param>
        ''' <returns> the node, or null if none </returns>
        Friend Function getNode(ByVal hash As Integer, ByVal key As Object) As Node(Of K, V)
            Dim tab As Node(Of K, V)()
            Dim first As Node(Of K, V), e As Node(Of K, V)
            Dim n As Integer
            Dim k As K
            tab = table
            n = tab.Length
            first = tab((n - 1) And hash)
            If tab IsNot Nothing AndAlso n > 0 AndAlso first IsNot Nothing Then
                k = first.key
                If first.hash = hash AndAlso (k Is key OrElse (key IsNot Nothing AndAlso key.Equals(k))) Then ' always check first node Return first
                    e = first.next
                    If e IsNot Nothing Then
                        If TypeOf first Is TreeNode Then Return CType(first, TreeNode(Of K, V)).getTreeNode(hash, key)
                        Do
                            k = e.key
                            If e.hash = hash AndAlso (k Is key OrElse (key IsNot Nothing AndAlso key.Equals(k))) Then Return e
                            e = e.next
                        Loop While e IsNot Nothing
                    End If
                End If
                Return Nothing
        End Function

        ''' <summary>
        ''' Returns <tt>true</tt> if this map contains a mapping for the
        ''' specified key.
        ''' </summary>
        ''' <param name="key">   The key whose presence in this map is to be tested </param>
        ''' <returns> <tt>true</tt> if this map contains a mapping for the specified
        ''' key. </returns>
        Public Overridable Function containsKey(ByVal key As Object) As Boolean Implements Map(Of K, V).containsKey
            Return getNode(hash(key), key) IsNot Nothing
        End Function

        ''' <summary>
        ''' Associates the specified value with the specified key in this map.
        ''' If the map previously contained a mapping for the key, the old
        ''' value is replaced.
        ''' </summary>
        ''' <param name="key"> key with which the specified value is to be associated </param>
        ''' <param name="value"> value to be associated with the specified key </param>
        ''' <returns> the previous value associated with <tt>key</tt>, or
        '''         <tt>null</tt> if there was no mapping for <tt>key</tt>.
        '''         (A <tt>null</tt> return can also indicate that the map
        '''         previously associated <tt>null</tt> with <tt>key</tt>.) </returns>
        Public Overridable Function put(ByVal key As K, ByVal value As V) As V Implements Map(Of K, V).put
            Return putVal(hash(key), key, value, False, True)
        End Function

        ''' <summary>
        ''' Implements Map.put and related methods
        ''' </summary>
        ''' <param name="hash"> hash for key </param>
        ''' <param name="key"> the key </param>
        ''' <param name="value"> the value to put </param>
        ''' <param name="onlyIfAbsent"> if true, don't change existing value </param>
        ''' <param name="evict"> if false, the table is in creation mode. </param>
        ''' <returns> previous value, or null if none </returns>
        Friend Function putVal(ByVal hash As Integer, ByVal key As K, ByVal value As V, ByVal onlyIfAbsent As Boolean, ByVal evict As Boolean) As V
            Dim tab As Node(Of K, V)()
            Dim p As Node(Of K, V)
            Dim n, i As Integer
            tab = table
            n = tab.Length
            If tab Is Nothing OrElse n = 0 Then n = (tab = resize()).length
            'JAVA TO VB CONVERTER TODO TASK: Assignments within expressions are not supported in VB
            p = tab(i = (n - 1) And hash)
            If p Is Nothing Then
                tab(i) = newNode(hash, key, value, Nothing)
            Else
                Dim e As Node(Of K, V)
                Dim k As K
                k = p.key
                If p.hash = hash AndAlso (k Is key OrElse (key IsNot Nothing AndAlso key.Equals(k))) Then
                    e = p
                ElseIf TypeOf p Is TreeNode Then
                    e = CType(p, TreeNode(Of K, V)).putTreeVal(Me, tab, hash, key, value)
                Else
                    Dim binCount As Integer = 0
                    Do
                        e = p.next
                        If e Is Nothing Then
                            p.next = newNode(hash, key, value, Nothing)
                            If binCount >= TREEIFY_THRESHOLD - 1 Then ' -1 for 1st treeifyBin(tab, hash)
                                Exit Do
                            End If
                            k = e.key
                            If e.hash = hash AndAlso (k Is key OrElse (key IsNot Nothing AndAlso key.Equals(k))) Then Exit Do
                            p = e
                            binCount += 1
                    Loop
                End If
                If e IsNot Nothing Then ' existing mapping for key
                    Dim oldValue As V = e.value
                    If (Not onlyIfAbsent) OrElse oldValue Is Nothing Then e.value = value
                    afterNodeAccess(e)
                    Return oldValue
                End If
            End If
            modCount += 1
            size_Renamed += 1
            If size_Renamed > threshold Then resize()
            afterNodeInsertion(evict)
            Return Nothing
        End Function

        ''' <summary>
        ''' Initializes or doubles table size.  If null, allocates in
        ''' accord with initial capacity target held in field threshold.
        ''' Otherwise, because we are using power-of-two expansion, the
        ''' elements from each bin must either stay at same index, or move
        ''' with a power of two offset in the new table.
        ''' </summary>
        ''' <returns> the table </returns>
        Friend Function resize() As Node(Of K, V)()
            Dim oldTab As Node(Of K, V)() = table
            Dim oldCap As Integer = If(oldTab Is Nothing, 0, oldTab.Length)
            Dim oldThr As Integer = threshold
            Dim newCap As Integer, newThr As Integer = 0
            If oldCap > 0 Then
                If oldCap >= MAXIMUM_CAPACITY Then
                    threshold =  java.lang.[Integer].Max_Value
                    Return oldTab
                Else
                    newCap = oldCap << 1
                    If newCap < MAXIMUM_CAPACITY AndAlso oldCap >= DEFAULT_INITIAL_CAPACITY Then newThr = oldThr << 1 ' double threshold
                End If
            ElseIf oldThr > 0 Then ' initial capacity was placed in threshold
                newCap = oldThr
            Else ' zero initial threshold signifies using defaults
                newCap = DEFAULT_INITIAL_CAPACITY
                newThr = CInt(Fix(DEFAULT_LOAD_FACTOR * DEFAULT_INITIAL_CAPACITY))
            End If
            If newThr = 0 Then
                Dim ft As Single = CSng(newCap) * loadFactor_Renamed
                newThr = (If(newCap < MAXIMUM_CAPACITY AndAlso ft < CSng(MAXIMUM_CAPACITY), CInt(Fix(ft)),  java.lang.[Integer].Max_Value))
            End If
            threshold = newThr
            'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
            Dim newTab As Node(Of K, V)() = CType(New Node(newCap - 1) {}, Node(Of K, V)())
            table = newTab
            If oldTab IsNot Nothing Then
                For j As Integer = 0 To oldCap - 1
                    Dim e As Node(Of K, V)
                    e = oldTab(j)
                    If e IsNot Nothing Then
                        oldTab(j) = Nothing
                        If e.next Is Nothing Then
                            newTab(e.hash And (newCap - 1)) = e
                        ElseIf TypeOf e Is TreeNode Then
                            CType(e, TreeNode(Of K, V)).split(Me, newTab, j, oldCap)
                        Else ' preserve order
                            Dim loHead As Node(Of K, V) = Nothing, loTail As Node(Of K, V) = Nothing
                            Dim hiHead As Node(Of K, V) = Nothing, hiTail As Node(Of K, V) = Nothing
                            Dim [next] As Node(Of K, V)
                            Do
                                [next] = e.next
                                If (e.hash And oldCap) = 0 Then
                                    If loTail Is Nothing Then
                                        loHead = e
                                    Else
                                        loTail.next = e
                                    End If
                                    loTail = e
                                Else
                                    If hiTail Is Nothing Then
                                        hiHead = e
                                    Else
                                        hiTail.next = e
                                    End If
                                    hiTail = e
                                End If
                                e = [next]
                            Loop While e IsNot Nothing
                            If loTail IsNot Nothing Then
                                loTail.next = Nothing
                                newTab(j) = loHead
                            End If
                            If hiTail IsNot Nothing Then
                                hiTail.next = Nothing
                                newTab(j + oldCap) = hiHead
                            End If
                        End If
                    End If
                Next j
            End If
            Return newTab
        End Function

        ''' <summary>
        ''' Replaces all linked nodes in bin at index for given hash unless
        ''' table is too small, in which case resizes instead.
        ''' </summary>
        Friend Sub treeifyBin(ByVal tab As Node(Of K, V)(), ByVal hash As Integer)
            Dim n, index As Integer
            Dim e As Node(Of K, V)
            n = tab.Length
            If tab Is Nothing OrElse n < MIN_TREEIFY_CAPACITY Then
                resize()
            Else
                'JAVA TO VB CONVERTER TODO TASK: Assignments within expressions are not supported in VB
                e = tab(index = (n - 1) And hash)
                If e IsNot Nothing Then
                    Dim hd As TreeNode(Of K, V) = Nothing, tl As TreeNode(Of K, V) = Nothing
                    Do
                        Dim p As TreeNode(Of K, V) = replacementTreeNode(e, Nothing)
                        If tl Is Nothing Then
                            hd = p
                        Else
                            p.prev = tl
                            tl.next = p
                        End If
                        tl = p
                        e = e.next
                    Loop While e IsNot Nothing
                    'JAVA TO VB CONVERTER TODO TASK: Assignments within expressions are not supported in VB
                    If (tab(index) = hd) IsNot Nothing Then hd.treeify(tab)
                End If
            End If
        End Sub

        ''' <summary>
        ''' Copies all of the mappings from the specified map to this map.
        ''' These mappings will replace any mappings that this map had for
        ''' any of the keys currently in the specified map.
        ''' </summary>
        ''' <param name="m"> mappings to be stored in this map </param>
        ''' <exception cref="NullPointerException"> if the specified map is null </exception>
        'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
        Public Overridable Sub putAll(Of T1 As K, ? As V)(ByVal m As Map(Of T1)) Implements Map(Of K, V).putAll
			putMapEntries(m, True)
        End Sub

        ''' <summary>
        ''' Removes the mapping for the specified key from this map if present.
        ''' </summary>
        ''' <param name="key"> key whose mapping is to be removed from the map </param>
        ''' <returns> the previous value associated with <tt>key</tt>, or
        '''         <tt>null</tt> if there was no mapping for <tt>key</tt>.
        '''         (A <tt>null</tt> return can also indicate that the map
        '''         previously associated <tt>null</tt> with <tt>key</tt>.) </returns>
        Public Overridable Function remove(ByVal key As Object) As V Implements Map(Of K, V).remove
            Dim e As Node(Of K, V)
            'JAVA TO VB CONVERTER TODO TASK: Assignments within expressions are not supported in VB
            Return If((e = removeNode(hash(key), key, Nothing, False, True)) Is Nothing, Nothing, e.value)
        End Function

        ''' <summary>
        ''' Implements Map.remove and related methods
        ''' </summary>
        ''' <param name="hash"> hash for key </param>
        ''' <param name="key"> the key </param>
        ''' <param name="value"> the value to match if matchValue, else ignored </param>
        ''' <param name="matchValue"> if true only remove if value is equal </param>
        ''' <param name="movable"> if false do not move other nodes while removing </param>
        ''' <returns> the node, or null if none </returns>
        Friend Function removeNode(ByVal hash As Integer, ByVal key As Object, ByVal value As Object, ByVal matchValue As Boolean, ByVal movable As Boolean) As Node(Of K, V)
            Dim tab As Node(Of K, V)()
            Dim p As Node(Of K, V)
            Dim n, index As Integer
            tab = table
            n = tab.Length
            'JAVA TO VB CONVERTER TODO TASK: Assignments within expressions are not supported in VB
            p = tab(index = (n - 1) And hash)
            If tab IsNot Nothing AndAlso n > 0 AndAlso p IsNot Nothing Then
                Dim node_Renamed As Node(Of K, V) = Nothing, e As Node(Of K, V)
                Dim k As K
                Dim v As V
                k = p.key
                If p.hash = hash AndAlso (k Is key OrElse (key IsNot Nothing AndAlso key.Equals(k))) Then
                    node_Renamed = p
                Else
                    e = p.next
                    If e IsNot Nothing Then
                        If TypeOf p Is TreeNode Then
                            node_Renamed = CType(p, TreeNode(Of K, V)).getTreeNode(hash, key)
                        Else
                            Do
                                k = e.key
                                If e.hash = hash AndAlso (k Is key OrElse (key IsNot Nothing AndAlso key.Equals(k))) Then
                                    node_Renamed = e
                                    Exit Do
                                End If
                                p = e
                                e = e.next
                            Loop While e IsNot Nothing
                        End If
                    End If
                End If
                v = node_Renamed.value
                If node_Renamed IsNot Nothing AndAlso ((Not matchValue) OrElse v Is value OrElse (value IsNot Nothing AndAlso value.Equals(v))) Then
                    If TypeOf node_Renamed Is TreeNode Then
                        CType(node_Renamed, TreeNode(Of K, V)).removeTreeNode(Me, tab, movable)
                    ElseIf node_Renamed Is p Then
                        tab(index) = node_Renamed.next
                    Else
                        p.next = node_Renamed.next
                    End If
                    modCount += 1
                    size_Renamed -= 1
                    afterNodeRemoval(node_Renamed)
                    Return node_Renamed
                End If
            End If
            Return Nothing
        End Function

        ''' <summary>
        ''' Removes all of the mappings from this map.
        ''' The map will be empty after this call returns.
        ''' </summary>
        Public Overridable Sub clear() Implements Map(Of K, V).clear
            Dim tab As Node(Of K, V)()
            modCount += 1
            tab = table
            If tab IsNot Nothing AndAlso size_Renamed > 0 Then
                size_Renamed = 0
                For i As Integer = 0 To tab.Length - 1
                    tab(i) = Nothing
                Next i
            End If
        End Sub

        ''' <summary>
        ''' Returns <tt>true</tt> if this map maps one or more keys to the
        ''' specified value.
        ''' </summary>
        ''' <param name="value"> value whose presence in this map is to be tested </param>
        ''' <returns> <tt>true</tt> if this map maps one or more keys to the
        '''         specified value </returns>
        Public Overridable Function containsValue(ByVal value As Object) As Boolean Implements Map(Of K, V).containsValue
            Dim tab As Node(Of K, V)()
            Dim v As V
            tab = table
            If tab IsNot Nothing AndAlso size_Renamed > 0 Then
                For i As Integer = 0 To tab.Length - 1
                    Dim e As Node(Of K, V) = tab(i)
                    Do While e IsNot Nothing
                        v = e.value
                        If v Is value OrElse (value IsNot Nothing AndAlso value.Equals(v)) Then Return True
                        e = e.next
                    Loop
                Next i
            End If
            Return False
        End Function

        ''' <summary>
        ''' Returns a <seealso cref="Set"/> view of the keys contained in this map.
        ''' The set is backed by the map, so changes to the map are
        ''' reflected in the set, and vice-versa.  If the map is modified
        ''' while an iteration over the set is in progress (except through
        ''' the iterator's own <tt>remove</tt> operation), the results of
        ''' the iteration are undefined.  The set supports element removal,
        ''' which removes the corresponding mapping from the map, via the
        ''' <tt>Iterator.remove</tt>, <tt>Set.remove</tt>,
        ''' <tt>removeAll</tt>, <tt>retainAll</tt>, and <tt>clear</tt>
        ''' operations.  It does not support the <tt>add</tt> or <tt>addAll</tt>
        ''' operations.
        ''' </summary>
        ''' <returns> a set view of the keys contained in this map </returns>
        Public Overridable Function keySet() As [Set](Of K) Implements Map(Of K, V).keySet
            Dim ks As [Set](Of K)
            ks = keySet_Renamed
            If ks Is Nothing Then
                keySet_Renamed = New KeySet(Me, Me)
                Return keySet_Renamed
            Else
                Return ks
            End If
        End Function

        Friend NotInheritable Class KeySet
            Inherits AbstractSet(Of K)

            Private ReadOnly outerInstance As HashMap

            Public Sub New(ByVal outerInstance As HashMap)
                Me.outerInstance = outerInstance
            End Sub

            Public Function size() As Integer
                Return outerInstance.size_Renamed
            End Function
            Public Sub clear()
                outerInstance.clear()
            End Sub
            Public Function [iterator]() As [Iterator](Of K)
                Return New KeyIterator
            End Function
            Public Function contains(ByVal o As Object) As Boolean
                Return outerInstance.containsKey(o)
            End Function
            Public Function remove(ByVal key As Object) As Boolean
                Return outerInstance.removeNode(hash(key), key, Nothing, False, True) IsNot Nothing
            End Function
            Public Function spliterator() As Spliterator(Of K)
                Return New KeySpliterator(Of )(HashMap.this, 0, -1, 0, 0)
            End Function
            'JAVA TO VB CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
            Public Sub forEach(Of T1)(ByVal action As java.util.function.Consumer(Of T1))
                Dim tab As Node(Of K, V)()
                If action Is Nothing Then Throw New NullPointerException
                tab = outerInstance.table
                If outerInstance.size_Renamed > 0 AndAlso tab IsNot Nothing Then
                    Dim mc As Integer = outerInstance.modCount
                    For i As Integer = 0 To tab.Length - 1
                        Dim e As Node(Of K, V) = tab(i)
                        Do While e IsNot Nothing
                            action.accept(e.key)
                            e = e.next
                        Loop
                    Next i
                    If outerInstance.modCount <> mc Then Throw New ConcurrentModificationException
                End If
            End Sub
        End Class

        ''' <summary>
        ''' Returns a <seealso cref="Collection"/> view of the values contained in this map.
        ''' The collection is backed by the map, so changes to the map are
        ''' reflected in the collection, and vice-versa.  If the map is
        ''' modified while an iteration over the collection is in progress
        ''' (except through the iterator's own <tt>remove</tt> operation),
        ''' the results of the iteration are undefined.  The collection
        ''' supports element removal, which removes the corresponding
        ''' mapping from the map, via the <tt>Iterator.remove</tt>,
        ''' <tt>Collection.remove</tt>, <tt>removeAll</tt>,
        ''' <tt>retainAll</tt> and <tt>clear</tt> operations.  It does not
        ''' support the <tt>add</tt> or <tt>addAll</tt> operations.
        ''' </summary>
        ''' <returns> a view of the values contained in this map </returns>
        Public Overridable Function values() As Collection(Of V) Implements Map(Of K, V).values
            Dim vs As Collection(Of V)
            vs = values_Renamed
            If vs Is Nothing Then
                values_Renamed = New Values(Me, Me)
                Return values_Renamed
            Else
                Return vs
            End If
        End Function

        Friend NotInheritable Class Values
            Inherits AbstractCollection(Of V)

            Private ReadOnly outerInstance As HashMap

            Public Sub New(ByVal outerInstance As HashMap)
                Me.outerInstance = outerInstance
            End Sub

            Public Function size() As Integer
                Return outerInstance.size_Renamed
            End Function
            Public Sub clear()
                outerInstance.clear()
            End Sub
            Public Function [iterator]() As [Iterator](Of V)
                Return New ValueIterator
            End Function
            Public Function contains(ByVal o As Object) As Boolean
                Return outerInstance.containsValue(o)
            End Function
            Public Function spliterator() As Spliterator(Of V)
                Return New ValueSpliterator(Of )(HashMap.this, 0, -1, 0, 0)
            End Function
            'JAVA TO VB CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
            Public Sub forEach(Of T1)(ByVal action As java.util.function.Consumer(Of T1))
                Dim tab As Node(Of K, V)()
                If action Is Nothing Then Throw New NullPointerException
                tab = outerInstance.table
                If outerInstance.size_Renamed > 0 AndAlso tab IsNot Nothing Then
                    Dim mc As Integer = outerInstance.modCount
                    For i As Integer = 0 To tab.Length - 1
                        Dim e As Node(Of K, V) = tab(i)
                        Do While e IsNot Nothing
                            action.accept(e.value)
                            e = e.next
                        Loop
                    Next i
                    If outerInstance.modCount <> mc Then Throw New ConcurrentModificationException
                End If
            End Sub
        End Class

        ''' <summary>
        ''' Returns a <seealso cref="Set"/> view of the mappings contained in this map.
        ''' The set is backed by the map, so changes to the map are
        ''' reflected in the set, and vice-versa.  If the map is modified
        ''' while an iteration over the set is in progress (except through
        ''' the iterator's own <tt>remove</tt> operation, or through the
        ''' <tt>setValue</tt> operation on a map entry returned by the
        ''' iterator) the results of the iteration are undefined.  The set
        ''' supports element removal, which removes the corresponding
        ''' mapping from the map, via the <tt>Iterator.remove</tt>,
        ''' <tt>Set.remove</tt>, <tt>removeAll</tt>, <tt>retainAll</tt> and
        ''' <tt>clear</tt> operations.  It does not support the
        ''' <tt>add</tt> or <tt>addAll</tt> operations.
        ''' </summary>
        ''' <returns> a set view of the mappings contained in this map </returns>
        Public Overridable Function entrySet() As [Set](Of KeyValuePair(Of K, V)) Implements Map(Of K, V).entrySet
            Dim es As [Set](Of KeyValuePair(Of K, V))
            es = entrySet_Renamed
            If es Is Nothing Then
                entrySet_Renamed = New EntrySet(Me, Me)
                Return entrySet_Renamed
            Else
                Return es
            End If
        End Function

        Friend NotInheritable Class EntrySet
            Inherits AbstractSet(Of KeyValuePair(Of K, V))

            Private ReadOnly outerInstance As HashMap

            Public Sub New(ByVal outerInstance As HashMap)
                Me.outerInstance = outerInstance
            End Sub

            Public Function size() As Integer
                Return outerInstance.size_Renamed
            End Function
            Public Sub clear()
                outerInstance.clear()
            End Sub
            Public Function [iterator]() As [Iterator](Of KeyValuePair(Of K, V))
                Return New EntryIterator
            End Function
            Public Function contains(ByVal o As Object) As Boolean
                If Not (TypeOf o Is DictionaryEntry) Then Return False
                'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
                Dim e As KeyValuePair(Of ?, ?) = CType(o, KeyValuePair(Of ?, ?))
                Dim key As Object = e.Key
                Dim candidate As Node(Of K, V) = outerInstance.getNode(hash(key), key)
                Return candidate IsNot Nothing AndAlso candidate.Equals(e)
            End Function
            Public Function remove(ByVal o As Object) As Boolean
                If TypeOf o Is DictionaryEntry Then
                    'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
                    Dim e As KeyValuePair(Of ?, ?) = CType(o, KeyValuePair(Of ?, ?))
                    Dim key As Object = e.Key
                    Dim value As Object = e.Value
                    Return outerInstance.removeNode(hash(key), key, value, True, True) IsNot Nothing
                End If
                Return False
            End Function
            Public Function spliterator() As Spliterator(Of KeyValuePair(Of K, V))
                Return New EntrySpliterator(Of )(HashMap.this, 0, -1, 0, 0)
            End Function
            'JAVA TO VB CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
            Public Sub forEach(Of T1)(ByVal action As java.util.function.Consumer(Of T1))
                Dim tab As Node(Of K, V)()
                If action Is Nothing Then Throw New NullPointerException
                tab = outerInstance.table
                If outerInstance.size_Renamed > 0 AndAlso tab IsNot Nothing Then
                    Dim mc As Integer = outerInstance.modCount
                    For i As Integer = 0 To tab.Length - 1
                        Dim e As Node(Of K, V) = tab(i)
                        Do While e IsNot Nothing
                            action.accept(e)
                            e = e.next
                        Loop
                    Next i
                    If outerInstance.modCount <> mc Then Throw New ConcurrentModificationException
                End If
            End Sub
        End Class

        ' Overrides of JDK8 Map extension methods

        Public Overrides Function getOrDefault(ByVal key As Object, ByVal defaultValue As V) As V Implements Map(Of K, V).getOrDefault
            Dim e As Node(Of K, V)
            'JAVA TO VB CONVERTER TODO TASK: Assignments within expressions are not supported in VB
            Return If((e = getNode(hash(key), key)) Is Nothing, defaultValue, e.value)
        End Function

        Public Overrides Function putIfAbsent(ByVal key As K, ByVal value As V) As V Implements Map(Of K, V).putIfAbsent
            Return putVal(hash(key), key, value, True, True)
        End Function

        Public Overrides Function remove(ByVal key As Object, ByVal value As Object) As Boolean Implements Map(Of K, V).remove
            Return removeNode(hash(key), key, value, True, True) IsNot Nothing
        End Function

        Public Overrides Function replace(ByVal key As K, ByVal oldValue As V, ByVal newValue As V) As Boolean Implements Map(Of K, V).replace
            Dim e As Node(Of K, V)
            Dim v As V
            e = getNode(hash(key), key)
            v = e.value
            If e IsNot Nothing AndAlso (v Is oldValue OrElse (v IsNot Nothing AndAlso v.Equals(oldValue))) Then
                e.value = newValue
                afterNodeAccess(e)
                Return True
            End If
            Return False
        End Function

        Public Overrides Function replace(ByVal key As K, ByVal value As V) As V Implements Map(Of K, V).replace
            Dim e As Node(Of K, V)
            e = getNode(hash(key), key)
            If e IsNot Nothing Then
                Dim oldValue As V = e.value
                e.value = value
                afterNodeAccess(e)
                Return oldValue
            End If
            Return Nothing
        End Function

        'JAVA TO VB CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
        Public Overrides Function computeIfAbsent(Of T1 As V)(ByVal key As K, ByVal mappingFunction As java.util.function.Function(Of T1)) As V Implements Map(Of K, V).computeIfAbsent
            If mappingFunction Is Nothing Then Throw New NullPointerException
            Dim hash As Integer = hash(key)
            Dim tab As Node(Of K, V)()
            Dim first As Node(Of K, V)
            Dim n, i As Integer
            Dim binCount As Integer = 0
            Dim t As TreeNode(Of K, V) = Nothing
            Dim old As Node(Of K, V) = Nothing
            tab = table
            n = tab.Length
            If size_Renamed > threshold OrElse tab Is Nothing OrElse n = 0 Then n = (tab = resize()).length
            'JAVA TO VB CONVERTER TODO TASK: Assignments within expressions are not supported in VB
            first = tab(i = (n - 1) And hash)
            If first IsNot Nothing Then
                If TypeOf first Is TreeNode Then
                    'JAVA TO VB CONVERTER TODO TASK: Assignments within expressions are not supported in VB
                    old = (t = CType(first, TreeNode(Of K, V))).getTreeNode(hash, key)
                Else
                    Dim e As Node(Of K, V) = first
                    Dim k As K
                    Do
                        k = e.key
                        If e.hash = hash AndAlso (k Is key OrElse (key IsNot Nothing AndAlso key.Equals(k))) Then
                            old = e
                            Exit Do
                        End If
                        binCount += 1
                        e = e.next
                    Loop While e IsNot Nothing
                End If
                Dim oldValue As V
                oldValue = old.value
                If old IsNot Nothing AndAlso oldValue IsNot Nothing Then
                    afterNodeAccess(old)
                    Return oldValue
                End If
            End If
            Dim v As V = mappingFunction.apply(key)
            If v Is Nothing Then
                Return Nothing
            ElseIf old IsNot Nothing Then
                old.value = v
                afterNodeAccess(old)
                Return v
            ElseIf t IsNot Nothing Then
                t.putTreeVal(Me, tab, hash, key, v)
            Else
                tab(i) = newNode(hash, key, v, first)
                If binCount >= TREEIFY_THRESHOLD - 1 Then treeifyBin(tab, hash)
            End If
            modCount += 1
            size_Renamed += 1
            afterNodeInsertion(True)
            Return v
        End Function

        'JAVA TO VB CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
        Public Overridable Function computeIfPresent(Of T1 As V)(ByVal key As K, ByVal remappingFunction As java.util.function.BiFunction(Of T1)) As V Implements Map(Of K, V).computeIfPresent
            If remappingFunction Is Nothing Then Throw New NullPointerException
            Dim e As Node(Of K, V)
            Dim oldValue As V
            Dim hash As Integer = hash(key)
            e = getNode(hash, key)
            oldValue = e.value
            If e IsNot Nothing AndAlso oldValue IsNot Nothing Then
                Dim v As V = remappingFunction.apply(key, oldValue)
                If v IsNot Nothing Then
                    e.value = v
                    afterNodeAccess(e)
                    Return v
                Else
                    removeNode(hash, key, Nothing, False, True)
                End If
            End If
            Return Nothing
        End Function

        'JAVA TO VB CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
        Public Overrides Function compute(Of T1 As V)(ByVal key As K, ByVal remappingFunction As java.util.function.BiFunction(Of T1)) As V Implements Map(Of K, V).compute
            If remappingFunction Is Nothing Then Throw New NullPointerException
            Dim hash As Integer = hash(key)
            Dim tab As Node(Of K, V)()
            Dim first As Node(Of K, V)
            Dim n, i As Integer
            Dim binCount As Integer = 0
            Dim t As TreeNode(Of K, V) = Nothing
            Dim old As Node(Of K, V) = Nothing
            tab = table
            n = tab.Length
            If size_Renamed > threshold OrElse tab Is Nothing OrElse n = 0 Then n = (tab = resize()).length
            'JAVA TO VB CONVERTER TODO TASK: Assignments within expressions are not supported in VB
            first = tab(i = (n - 1) And hash)
            If first IsNot Nothing Then
                If TypeOf first Is TreeNode Then
                    'JAVA TO VB CONVERTER TODO TASK: Assignments within expressions are not supported in VB
                    old = (t = CType(first, TreeNode(Of K, V))).getTreeNode(hash, key)
                Else
                    Dim e As Node(Of K, V) = first
                    Dim k As K
                    Do
                        k = e.key
                        If e.hash = hash AndAlso (k Is key OrElse (key IsNot Nothing AndAlso key.Equals(k))) Then
                            old = e
                            Exit Do
                        End If
                        binCount += 1
                        e = e.next
                    Loop While e IsNot Nothing
                End If
            End If
            Dim oldValue As V = If(old Is Nothing, Nothing, old.value)
            Dim v As V = remappingFunction.apply(key, oldValue)
            If old IsNot Nothing Then
                If v IsNot Nothing Then
                    old.value = v
                    afterNodeAccess(old)
                Else
                    removeNode(hash, key, Nothing, False, True)
                End If
            ElseIf v IsNot Nothing Then
                If t IsNot Nothing Then
                    t.putTreeVal(Me, tab, hash, key, v)
                Else
                    tab(i) = newNode(hash, key, v, first)
                    If binCount >= TREEIFY_THRESHOLD - 1 Then treeifyBin(tab, hash)
                End If
                modCount += 1
                size_Renamed += 1
                afterNodeInsertion(True)
            End If
            Return v
        End Function

        'JAVA TO VB CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
        Public Overrides Function merge(Of T1 As V)(ByVal key As K, ByVal value As V, ByVal remappingFunction As java.util.function.BiFunction(Of T1)) As V Implements Map(Of K, V).merge
            If value Is Nothing Then Throw New NullPointerException
            If remappingFunction Is Nothing Then Throw New NullPointerException
            Dim hash As Integer = hash(key)
            Dim tab As Node(Of K, V)()
            Dim first As Node(Of K, V)
            Dim n, i As Integer
            Dim binCount As Integer = 0
            Dim t As TreeNode(Of K, V) = Nothing
            Dim old As Node(Of K, V) = Nothing
            tab = table
            n = tab.Length
            If size_Renamed > threshold OrElse tab Is Nothing OrElse n = 0 Then n = (tab = resize()).length
            'JAVA TO VB CONVERTER TODO TASK: Assignments within expressions are not supported in VB
            first = tab(i = (n - 1) And hash)
            If first IsNot Nothing Then
                If TypeOf first Is TreeNode Then
                    'JAVA TO VB CONVERTER TODO TASK: Assignments within expressions are not supported in VB
                    old = (t = CType(first, TreeNode(Of K, V))).getTreeNode(hash, key)
                Else
                    Dim e As Node(Of K, V) = first
                    Dim k As K
                    Do
                        k = e.key
                        If e.hash = hash AndAlso (k Is key OrElse (key IsNot Nothing AndAlso key.Equals(k))) Then
                            old = e
                            Exit Do
                        End If
                        binCount += 1
                        e = e.next
                    Loop While e IsNot Nothing
                End If
            End If
            If old IsNot Nothing Then
                Dim v As V
                If old.value IsNot Nothing Then
                    v = remappingFunction.apply(old.value, value)
                Else
                    v = value
                End If
                If v IsNot Nothing Then
                    old.value = v
                    afterNodeAccess(old)
                Else
                    removeNode(hash, key, Nothing, False, True)
                End If
                Return v
            End If
            If value IsNot Nothing Then
                If t IsNot Nothing Then
                    t.putTreeVal(Me, tab, hash, key, value)
                Else
                    tab(i) = newNode(hash, key, value, first)
                    If binCount >= TREEIFY_THRESHOLD - 1 Then treeifyBin(tab, hash)
                End If
                modCount += 1
                size_Renamed += 1
                afterNodeInsertion(True)
            End If
            Return value
        End Function

        'JAVA TO VB CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
        Public Overrides Sub forEach(Of T1)(ByVal action As java.util.function.BiConsumer(Of T1)) Implements Map(Of K, V).forEach
            Dim tab As Node(Of K, V)()
            If action Is Nothing Then Throw New NullPointerException
            tab = table
            If size_Renamed > 0 AndAlso tab IsNot Nothing Then
                Dim mc As Integer = modCount
                For i As Integer = 0 To tab.Length - 1
                    Dim e As Node(Of K, V) = tab(i)
                    Do While e IsNot Nothing
                        action.accept(e.key, e.value)
                        e = e.next
                    Loop
                Next i
                If modCount <> mc Then Throw New ConcurrentModificationException
            End If
        End Sub

        'JAVA TO VB CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
        Public Overrides Sub replaceAll(Of T1 As V)(ByVal [function] As java.util.function.BiFunction(Of T1)) Implements Map(Of K, V).replaceAll
            Dim tab As Node(Of K, V)()
            If [function] Is Nothing Then Throw New NullPointerException
            tab = table
            If size_Renamed > 0 AndAlso tab IsNot Nothing Then
                Dim mc As Integer = modCount
                For i As Integer = 0 To tab.Length - 1
                    Dim e As Node(Of K, V) = tab(i)
                    Do While e IsNot Nothing
                        e.value = [function].apply(e.key, e.value)
                        e = e.next
                    Loop
                Next i
                If modCount <> mc Then Throw New ConcurrentModificationException
            End If
        End Sub

        ' ------------------------------------------------------------ 
        ' Cloning and serialization

        ''' <summary>
        ''' Returns a shallow copy of this <tt>HashMap</tt> instance: the keys and
        ''' values themselves are not cloned.
        ''' </summary>
        ''' <returns> a shallow copy of this map </returns>
        'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
        Public Overrides Function clone() As Object
            Dim result As HashMap(Of K, V)
            Try
                result = CType(MyBase.clone(), HashMap(Of K, V))
            Catch e As CloneNotSupportedException
                ' this shouldn't happen, since we are Cloneable
                Throw New InternalError(e)
            End Try
            result.reinitialize()
            result.putMapEntries(Me, False)
            Return result
        End Function

        ' These methods are also used when serializing HashSets
        Friend Function loadFactor() As Single
            Return loadFactor_Renamed
        End Function
        Friend Function capacity() As Integer
            Return If(table IsNot Nothing, table.Length, If(threshold > 0, threshold, DEFAULT_INITIAL_CAPACITY))
        End Function

        ''' <summary>
        ''' Save the state of the <tt>HashMap</tt> instance to a stream (i.e.,
        ''' serialize it).
        ''' 
        ''' @serialData The <i>capacity</i> of the HashMap (the length of the
        '''             bucket array) is emitted (int), followed by the
        '''             <i>size</i> (an int, the number of key-value
        '''             mappings), followed by the key (Object) and value (Object)
        '''             for each key-value mapping.  The key-value mappings are
        '''             emitted in no particular order.
        ''' </summary>
        Private Sub writeObject(ByVal s As java.io.ObjectOutputStream)
            Dim buckets As Integer = capacity()
            ' Write out the threshold, loadfactor, and any hidden stuff
            s.defaultWriteObject()
            s.writeInt(buckets)
            s.writeInt(size_Renamed)
            internalWriteEntries(s)
        End Sub

        ''' <summary>
        ''' Reconstitute the {@code HashMap} instance from a stream (i.e.,
        ''' deserialize it).
        ''' </summary>
        Private Sub readObject(ByVal s As java.io.ObjectInputStream)
            ' Read in the threshold (ignored), loadfactor, and any hidden stuff
            s.defaultReadObject()
            reinitialize()
            If loadFactor_Renamed <= 0 OrElse Float.IsNaN(loadFactor_Renamed) Then Throw New java.io.InvalidObjectException("Illegal load factor: " & loadFactor_Renamed)
            s.readInt() ' Read and ignore number of buckets
            Dim mappings As Integer = s.readInt() ' Read number of mappings (size)
            If mappings < 0 Then
                Throw New java.io.InvalidObjectException("Illegal mappings count: " & mappings)
            ElseIf mappings > 0 Then ' (if zero, use defaults)
                ' Size the table using given load factor only if within
                ' range of 0.25...4.0
                Dim lf As Single = System.Math.Min (System.Math.Max(0.25F, loadFactor_Renamed), 4.0F)
                Dim fc As Single = CSng(mappings) / lf + 1.0F
                Dim cap As Integer = (If(fc < DEFAULT_INITIAL_CAPACITY, DEFAULT_INITIAL_CAPACITY, If(fc >= MAXIMUM_CAPACITY, MAXIMUM_CAPACITY, tableSizeFor(CInt(Fix(fc))))))
                Dim ft As Single = CSng(cap) * lf
                threshold = (If(cap < MAXIMUM_CAPACITY AndAlso ft < MAXIMUM_CAPACITY, CInt(Fix(ft)),  java.lang.[Integer].Max_Value))
                'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
                Dim tab As Node(Of K, V)() = CType(New Node(cap - 1) {}, Node(Of K, V)())
                table = tab

                ' Read the keys and values, and put the mappings in the HashMap
                For i As Integer = 0 To mappings - 1
                    'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
                    Dim key As K = CType(s.readObject(), K)
                    'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
                    Dim value As V = CType(s.readObject(), V)
                    putVal(hash(key), key, value, False, False)
                Next i
            End If
        End Sub

        ' ------------------------------------------------------------ 
        ' iterators

        Friend MustInherit Class HashIterator
            Private ReadOnly outerInstance As HashMap

            Friend [next] As Node(Of K, V) ' next entry to return
            Friend current As Node(Of K, V) ' current entry
            Friend expectedModCount As Integer ' for fast-fail
            Friend index As Integer ' current slot

            Friend Sub New(ByVal outerInstance As HashMap)
                Me.outerInstance = outerInstance
                expectedModCount = outerInstance.modCount
                Dim t As Node(Of K, V)() = outerInstance.table
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
					= Nothing
					current = [next]
                index = 0
                If t IsNot Nothing AndAlso outerInstance.size_Renamed > 0 Then ' advance to first entry
                    Dim tempVar As Boolean
                    Do
                        'JAVA TO VB CONVERTER TODO TASK: Assignments within expressions are not supported in VB
                        tempVar = index < t.Length AndAlso ([next] = t(index)) Is Nothing
                        index += 1
                    Loop While tempVar
                End If
            End Sub

            Public Function hasNext() As Boolean
                Return [next] IsNot Nothing
            End Function

            Friend Function nextNode() As Node(Of K, V)
                Dim t As Node(Of K, V)()
                Dim e As Node(Of K, V) = [next]
                If outerInstance.modCount <> expectedModCount Then Throw New ConcurrentModificationException
                If e Is Nothing Then Throw New NoSuchElementException
                'JAVA TO VB CONVERTER TODO TASK: Assignments within expressions are not supported in VB
                [next] = (current = e).next
                t = outerInstance.table
                If [next] Is Nothing AndAlso t IsNot Nothing Then
                    Dim tempVar As Boolean
                    Do
                        'JAVA TO VB CONVERTER TODO TASK: Assignments within expressions are not supported in VB
                        tempVar = index < t.Length AndAlso ([next] = t(index)) Is Nothing
                        index += 1
                    Loop While tempVar
                End If
                Return e
            End Function

            Public Sub remove()
                Dim p As Node(Of K, V) = current
                If p Is Nothing Then Throw New IllegalStateException
                If outerInstance.modCount <> expectedModCount Then Throw New ConcurrentModificationException
                current = Nothing
                Dim key As K = p.key
                outerInstance.removeNode(hash(key), key, Nothing, False, False)
                expectedModCount = outerInstance.modCount
            End Sub
        End Class

        Friend NotInheritable Class KeyIterator
            Inherits HashIterator
            Implements Iterator(Of K)

            Private ReadOnly outerInstance As HashMap

            Public Sub New(ByVal outerInstance As HashMap)
                Me.outerInstance = outerInstance
            End Sub

            Public Function [next]() As K
                Return nextNode().key
            End Function
        End Class

        Friend NotInheritable Class ValueIterator
            Inherits HashIterator
            Implements Iterator(Of V)

            Private ReadOnly outerInstance As HashMap

            Public Sub New(ByVal outerInstance As HashMap)
                Me.outerInstance = outerInstance
            End Sub

            Public Function [next]() As V
                Return nextNode().value
            End Function
        End Class

        Friend NotInheritable Class EntryIterator
            Inherits HashIterator
            Implements Iterator(Of KeyValuePair(Of K, V))

            Private ReadOnly outerInstance As HashMap

            Public Sub New(ByVal outerInstance As HashMap)
                Me.outerInstance = outerInstance
            End Sub

            Public Function [next]() As KeyValuePair(Of K, V)
                Return nextNode()
            End Function
        End Class

        ' ------------------------------------------------------------ 
        ' spliterators

        Friend Class HashMapSpliterator(Of K, V)
            Friend ReadOnly map As HashMap(Of K, V)
            Friend current As Node(Of K, V) ' current node
            Friend index As Integer ' current index, modified on advance/split
            Friend fence As Integer ' one past last index
            Friend est As Integer ' size estimate
            Friend expectedModCount As Integer ' for comodification checks

            Friend Sub New(ByVal m As HashMap(Of K, V), ByVal origin As Integer, ByVal fence As Integer, ByVal est As Integer, ByVal expectedModCount As Integer)
                Me.map = m
                Me.index = origin
                Me.fence = fence
                Me.est = est
                Me.expectedModCount = expectedModCount
            End Sub

            Friend Property fence As Integer
                Get
                    Dim hi As Integer
                    hi = fence
                    If hi < 0 Then
                        Dim m As HashMap(Of K, V) = map
                        est = m.size_Renamed
                        expectedModCount = m.modCount
                        Dim tab As Node(Of K, V)() = m.table
                        fence = If(tab Is Nothing, 0, tab.Length)
                        hi = fence
                    End If
                    Return hi
                End Get
            End Property

            Public Function estimateSize() As Long
                fence ' force init
                Return CLng(est)
            End Function
        End Class

        Friend NotInheritable Class KeySpliterator(Of K, V)
            Inherits HashMapSpliterator(Of K, V)
            Implements Spliterator(Of K)

            Friend Sub New(ByVal m As HashMap(Of K, V), ByVal origin As Integer, ByVal fence As Integer, ByVal est As Integer, ByVal expectedModCount As Integer)
                MyBase.New(m, origin, fence, est, expectedModCount)
            End Sub

            Public Function trySplit() As KeySpliterator(Of K, V)
                Dim hi As Integer = fence, lo As Integer = index, mid As Integer = CInt(CUInt((lo + hi)) >> 1)
                'JAVA TO VB CONVERTER TODO TASK: Assignments within expressions are not supported in VB
                Return If(lo >= mid OrElse current IsNot Nothing, Nothing, New KeySpliterator(Of )(map, lo, index = mid, est >>>= 1, expectedModCount))
            End Function

            'JAVA TO VB CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
            Public Sub forEachRemaining(Of T1)(ByVal action As java.util.function.Consumer(Of T1)) Implements Spliterator(Of K).forEachRemaining
                Dim i, hi, mc As Integer
                If action Is Nothing Then Throw New NullPointerException
                Dim m As HashMap(Of K, V) = map
                Dim tab As Node(Of K, V)() = m.table
                hi = fence
                If hi < 0 Then
                    expectedModCount = m.modCount
                    mc = expectedModCount
                    fence = If(tab Is Nothing, 0, tab.Length)
                    hi = fence
                Else
                    mc = expectedModCount
                End If
                i = index
                index = hi
                If tab IsNot Nothing AndAlso tab.Length >= hi AndAlso i >= 0 AndAlso (i < index OrElse current IsNot Nothing) Then
                    Dim p As Node(Of K, V) = current
                    current = Nothing
                    Do
                        If p Is Nothing Then
                            p = tab(i)
                            i += 1
                        Else
                            action.accept(p.key)
                            p = p.next
                        End If
                    Loop While p IsNot Nothing OrElse i < hi
                    If m.modCount <> mc Then Throw New ConcurrentModificationException
                End If
            End Sub

            'JAVA TO VB CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
            Public Function tryAdvance(Of T1)(ByVal action As java.util.function.Consumer(Of T1)) As Boolean Implements Spliterator(Of K).tryAdvance
                Dim hi As Integer
                If action Is Nothing Then Throw New NullPointerException
                Dim tab As Node(Of K, V)() = map.table
                hi = fence
                If tab IsNot Nothing AndAlso tab.Length >= hi AndAlso index >= 0 Then
                    Do While current IsNot Nothing OrElse index < hi
                        If current Is Nothing Then
                            current = tab(index)
                            index += 1
                        Else
                            Dim k As K = current.key
                            current = current.next
                            action.accept(k)
                            If map.modCount <> expectedModCount Then Throw New ConcurrentModificationException
                            Return True
                        End If
                    Loop
                End If
                Return False
            End Function

            Public Function characteristics() As Integer Implements Spliterator(Of K).characteristics
                Return (If(fence < 0 OrElse est = map.size_Renamed, Spliterator.SIZED, 0)) Or Spliterator.DISTINCT
            End Function
        End Class

        Friend NotInheritable Class ValueSpliterator(Of K, V)
            Inherits HashMapSpliterator(Of K, V)
            Implements Spliterator(Of V)

            Friend Sub New(ByVal m As HashMap(Of K, V), ByVal origin As Integer, ByVal fence As Integer, ByVal est As Integer, ByVal expectedModCount As Integer)
                MyBase.New(m, origin, fence, est, expectedModCount)
            End Sub

            Public Function trySplit() As ValueSpliterator(Of K, V)
                Dim hi As Integer = fence, lo As Integer = index, mid As Integer = CInt(CUInt((lo + hi)) >> 1)
                'JAVA TO VB CONVERTER TODO TASK: Assignments within expressions are not supported in VB
                Return If(lo >= mid OrElse current IsNot Nothing, Nothing, New ValueSpliterator(Of )(map, lo, index = mid, est >>>= 1, expectedModCount))
            End Function

            'JAVA TO VB CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
            Public Sub forEachRemaining(Of T1)(ByVal action As java.util.function.Consumer(Of T1)) Implements Spliterator(Of V).forEachRemaining
                Dim i, hi, mc As Integer
                If action Is Nothing Then Throw New NullPointerException
                Dim m As HashMap(Of K, V) = map
                Dim tab As Node(Of K, V)() = m.table
                hi = fence
                If hi < 0 Then
                    expectedModCount = m.modCount
                    mc = expectedModCount
                    fence = If(tab Is Nothing, 0, tab.Length)
                    hi = fence
                Else
                    mc = expectedModCount
                End If
                i = index
                index = hi
                If tab IsNot Nothing AndAlso tab.Length >= hi AndAlso i >= 0 AndAlso (i < index OrElse current IsNot Nothing) Then
                    Dim p As Node(Of K, V) = current
                    current = Nothing
                    Do
                        If p Is Nothing Then
                            p = tab(i)
                            i += 1
                        Else
                            action.accept(p.value)
                            p = p.next
                        End If
                    Loop While p IsNot Nothing OrElse i < hi
                    If m.modCount <> mc Then Throw New ConcurrentModificationException
                End If
            End Sub

            'JAVA TO VB CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
            Public Function tryAdvance(Of T1)(ByVal action As java.util.function.Consumer(Of T1)) As Boolean Implements Spliterator(Of V).tryAdvance
                Dim hi As Integer
                If action Is Nothing Then Throw New NullPointerException
                Dim tab As Node(Of K, V)() = map.table
                hi = fence
                If tab IsNot Nothing AndAlso tab.Length >= hi AndAlso index >= 0 Then
                    Do While current IsNot Nothing OrElse index < hi
                        If current Is Nothing Then
                            current = tab(index)
                            index += 1
                        Else
                            Dim v As V = current.value
                            current = current.next
                            action.accept(v)
                            If map.modCount <> expectedModCount Then Throw New ConcurrentModificationException
                            Return True
                        End If
                    Loop
                End If
                Return False
            End Function

            Public Function characteristics() As Integer Implements Spliterator(Of V).characteristics
                Return (If(fence < 0 OrElse est = map.size_Renamed, Spliterator.SIZED, 0))
            End Function
        End Class

        Friend NotInheritable Class EntrySpliterator(Of K, V)
            Inherits HashMapSpliterator(Of K, V)
            Implements Spliterator(Of KeyValuePair(Of K, V))

            Friend Sub New(ByVal m As HashMap(Of K, V), ByVal origin As Integer, ByVal fence As Integer, ByVal est As Integer, ByVal expectedModCount As Integer)
                MyBase.New(m, origin, fence, est, expectedModCount)
            End Sub

            Public Function trySplit() As EntrySpliterator(Of K, V)
                Dim hi As Integer = fence, lo As Integer = index, mid As Integer = CInt(CUInt((lo + hi)) >> 1)
                'JAVA TO VB CONVERTER TODO TASK: Assignments within expressions are not supported in VB
                Return If(lo >= mid OrElse current IsNot Nothing, Nothing, New EntrySpliterator(Of )(map, lo, index = mid, est >>>= 1, expectedModCount))
            End Function

            'JAVA TO VB CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
            Public Sub forEachRemaining(Of T1)(ByVal action As java.util.function.Consumer(Of T1)) Implements Spliterator(Of KeyValuePair(Of K, V)).forEachRemaining
                Dim i, hi, mc As Integer
                If action Is Nothing Then Throw New NullPointerException
                Dim m As HashMap(Of K, V) = map
                Dim tab As Node(Of K, V)() = m.table
                hi = fence
                If hi < 0 Then
                    expectedModCount = m.modCount
                    mc = expectedModCount
                    fence = If(tab Is Nothing, 0, tab.Length)
                    hi = fence
                Else
                    mc = expectedModCount
                End If
                i = index
                index = hi
                If tab IsNot Nothing AndAlso tab.Length >= hi AndAlso i >= 0 AndAlso (i < index OrElse current IsNot Nothing) Then
                    Dim p As Node(Of K, V) = current
                    current = Nothing
                    Do
                        If p Is Nothing Then
                            p = tab(i)
                            i += 1
                        Else
                            action.accept(p)
                            p = p.next
                        End If
                    Loop While p IsNot Nothing OrElse i < hi
                    If m.modCount <> mc Then Throw New ConcurrentModificationException
                End If
            End Sub

            'JAVA TO VB CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
            Public Function tryAdvance(Of T1)(ByVal action As java.util.function.Consumer(Of T1)) As Boolean Implements Spliterator(Of KeyValuePair(Of K, V)).tryAdvance
                Dim hi As Integer
                If action Is Nothing Then Throw New NullPointerException
                Dim tab As Node(Of K, V)() = map.table
                hi = fence
                If tab IsNot Nothing AndAlso tab.Length >= hi AndAlso index >= 0 Then
                    Do While current IsNot Nothing OrElse index < hi
                        If current Is Nothing Then
                            current = tab(index)
                            index += 1
                        Else
                            Dim e As Node(Of K, V) = current
                            current = current.next
                            action.accept(e)
                            If map.modCount <> expectedModCount Then Throw New ConcurrentModificationException
                            Return True
                        End If
                    Loop
                End If
                Return False
            End Function

            Public Function characteristics() As Integer Implements Spliterator(Of KeyValuePair(Of K, V)).characteristics
                Return (If(fence < 0 OrElse est = map.size_Renamed, Spliterator.SIZED, 0)) Or Spliterator.DISTINCT
            End Function
        End Class

        ' ------------------------------------------------------------ 
        ' LinkedHashMap support


        '    
        '     * The following package-protected methods are designed to be
        '     * overridden by LinkedHashMap, but not by any other subclass.
        '     * Nearly all other internal methods are also package-protected
        '     * but are declared final, so can be used by LinkedHashMap, view
        '     * classes, and HashSet.
        '     

        ' Create a regular (non-tree) node
        Friend Overridable Function newNode(ByVal hash As Integer, ByVal key As K, ByVal value As V, ByVal [next] As Node(Of K, V)) As Node(Of K, V)
            Return New Node(Of )(hash, key, value, [next])
        End Function

        ' For conversion from TreeNodes to plain nodes
        Friend Overridable Function replacementNode(ByVal p As Node(Of K, V), ByVal [next] As Node(Of K, V)) As Node(Of K, V)
            Return New Node(Of )(p.hash, p.key, p.value, [next])
        End Function

        ' Create a tree bin node
        Friend Overridable Function newTreeNode(ByVal hash As Integer, ByVal key As K, ByVal value As V, ByVal [next] As Node(Of K, V)) As TreeNode(Of K, V)
            Return New TreeNode(Of )(hash, key, value, [next])
        End Function

        ' For treeifyBin
        Friend Overridable Function replacementTreeNode(ByVal p As Node(Of K, V), ByVal [next] As Node(Of K, V)) As TreeNode(Of K, V)
            Return New TreeNode(Of )(p.hash, p.key, p.value, [next])
        End Function

        ''' <summary>
        ''' Reset to initial default state.  Called by clone and readObject.
        ''' </summary>
        Friend Overridable Sub reinitialize()
            table = Nothing
            entrySet_Renamed = Nothing
            keySet_Renamed = Nothing
            values_Renamed = Nothing
            modCount = 0
            threshold = 0
            size_Renamed = 0
        End Sub

        ' Callbacks to allow LinkedHashMap post-actions
        Friend Overridable Sub afterNodeAccess(ByVal p As Node(Of K, V))
        End Sub
        Friend Overridable Sub afterNodeInsertion(ByVal evict As Boolean)
        End Sub
        Friend Overridable Sub afterNodeRemoval(ByVal p As Node(Of K, V))
        End Sub

        ' Called only from writeObject, to ensure compatible ordering.
        Friend Overridable Sub internalWriteEntries(ByVal s As java.io.ObjectOutputStream)
            Dim tab As Node(Of K, V)()
            tab = table
            If size_Renamed > 0 AndAlso tab IsNot Nothing Then
                For i As Integer = 0 To tab.Length - 1
                    Dim e As Node(Of K, V) = tab(i)
                    Do While e IsNot Nothing
                        s.writeObject(e.key)
                        s.writeObject(e.value)
                        e = e.next
                    Loop
                Next i
            End If
        End Sub

        ' ------------------------------------------------------------ 
        ' Tree bins

        ''' <summary>
        ''' Entry for Tree bins. Extends LinkedHashMap.Entry (which in turn
        ''' extends Node) so can be used as extension of either regular or
        ''' linked node.
        ''' </summary>
        Friend NotInheritable Class TreeNode(Of K, V)
            Inherits LinkedHashMap.Entry(Of K, V)

            Friend parent As TreeNode(Of K, V) ' red-black tree links
            Friend left As TreeNode(Of K, V)
            Friend right As TreeNode(Of K, V)
            Friend prev As TreeNode(Of K, V) ' needed to unlink next upon deletion
            Friend red As Boolean
            Friend Sub New(ByVal hash As Integer, ByVal key As K, ByVal val As V, ByVal [next] As Node(Of K, V))
                MyBase.New(hash, key, val, [next])
            End Sub

            ''' <summary>
            ''' Returns root of tree containing this node.
            ''' </summary>
            Friend Function root() As TreeNode(Of K, V)
                'JAVA TO VB CONVERTER TODO TASK: The following line could not be converted:
                For (TreeNode < K,V> r = Me, p;;)
					p = r.parent
                    If p Is Nothing Then Return r
                    r = p
            End Function

            ''' <summary>
            ''' Ensures that the given root is the first node of its bin.
            ''' </summary>
            Shared Sub moveRootToFront(Of K, V)(ByVal tab As Node(Of K, V)(), ByVal root As TreeNode(Of K, V))
                Dim n As Integer
                n = tab.Length
                If root IsNot Nothing AndAlso tab IsNot Nothing AndAlso n > 0 Then
                    Dim index As Integer = (n - 1) And root.hash
                    Dim first As TreeNode(Of K, V) = CType(tab(index), TreeNode(Of K, V))
                    If root IsNot first Then
                        Dim rn As Node(Of K, V)
                        tab(index) = root
                        Dim rp As TreeNode(Of K, V) = root.prev
                        rn = root.next
                        If rn IsNot Nothing Then CType(rn, TreeNode(Of K, V)).prev = rp
                        If rp IsNot Nothing Then rp.next = rn
                        If first IsNot Nothing Then first.prev = root
                        root.next = first
                        root.prev = Nothing
                    End If
                    Debug.Assert(checkInvariants(root))
                End If
            End Sub

            ''' <summary>
            ''' Finds the node starting at root p with the given hash and key.
            ''' The kc argument caches comparableClassFor(key) upon first use
            ''' comparing keys.
            ''' </summary>
            Friend Function find(ByVal h As Integer, ByVal k As Object, ByVal kc As [Class]) As TreeNode(Of K, V)
                Dim p As TreeNode(Of K, V) = Me
                Do
                    Dim ph, dir As Integer
                    Dim pk As K
                    Dim pl As TreeNode(Of K, V) = p.left, pr As TreeNode(Of K, V) = p.right, q As TreeNode(Of K, V)
                    ph = p.hash
                    If ph > h Then
                        p = pl
                    ElseIf ph < h Then
                        p = pr
                    Else
                        pk = p.key
                        If pk Is k OrElse (k IsNot Nothing AndAlso k.Equals(pk)) Then
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
                                q = pr.find(h, k, kc)
                                If q IsNot Nothing Then
                                    Return q
                                Else
                                    p = pl
                                End If
                            End If
                        End If
                    End If
                Loop While p IsNot Nothing
                Return Nothing
            End Function

            ''' <summary>
            ''' Calls find for root node.
            ''' </summary>
            Friend Function getTreeNode(ByVal h As Integer, ByVal k As Object) As TreeNode(Of K, V)
                Return (If(parent IsNot Nothing, root(), Me)).find(h, k, Nothing)
            End Function

            ''' <summary>
            ''' Tie-breaking utility for ordering insertions when equal
            ''' hashCodes and non-comparable. We don't require a total
            ''' order, just a consistent insertion rule to maintain
            ''' equivalence across rebalancings. Tie-breaking further than
            ''' necessary simplifies testing a bit.
            ''' </summary>
            Friend Shared Function tieBreakOrder(ByVal a As Object, ByVal b As Object) As Integer
                Dim d As Integer
                d = a.GetType().Name.CompareTo(b.GetType().Name)
                If a Is Nothing OrElse b Is Nothing OrElse d = 0 Then d = (If(System.identityHashCode(a) <= System.identityHashCode(b), -1, 1))
                Return d
            End Function

            ''' <summary>
            ''' Forms tree of the nodes linked from this node. </summary>
            ''' <returns> root of tree </returns>
            Friend Sub treeify(ByVal tab As Node(Of K, V)())
                Dim root As TreeNode(Of K, V) = Nothing
                'JAVA TO VB CONVERTER TODO TASK: The following line could not be converted:
                For (TreeNode < K,V> x = Me, next; x != Nothing; x = next)
					[next] = CType(x.next, TreeNode(Of K, V))
                    x.right = Nothing
                    x.left = x.right
                    If root Is Nothing Then
                        x.parent = Nothing
                        x.red = False
                        root = x
                    Else
                        Dim k As K = x.key
                        Dim h As Integer = x.hash
                        Dim kc As [Class] = Nothing
                        Dim p As TreeNode(Of K, V) = root
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
                                root = balanceInsertion(root, x)
                                Exit Do
                            End If
                        Loop
                    End If
                    moveRootToFront(tab, root)
            End Sub

            ''' <summary>
            ''' Returns a list of non-TreeNodes replacing those linked from
            ''' this node.
            ''' </summary>
            Friend Function untreeify(ByVal map As HashMap(Of K, V)) As Node(Of K, V)
                Dim hd As Node(Of K, V) = Nothing, tl As Node(Of K, V) = Nothing
                Dim q As Node(Of K, V) = Me
                Do While q IsNot Nothing
                    Dim p As Node(Of K, V) = map.replacementNode(q, Nothing)
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

            ''' <summary>
            ''' Tree version of putVal.
            ''' </summary>
            Friend Function putTreeVal(ByVal map As HashMap(Of K, V), ByVal tab As Node(Of K, V)(), ByVal h As Integer, ByVal k As K, ByVal v As V) As TreeNode(Of K, V)
                Dim kc As [Class] = Nothing
                Dim searched As Boolean = False
                Dim root As TreeNode(Of K, V) = If(parent IsNot Nothing, root(), Me)
                Dim p As TreeNode(Of K, V) = root
                Do
                    Dim dir, ph As Integer
                    Dim pk As K
                    ph = p.hash
                    If ph > h Then
                        dir = -1
                    ElseIf ph < h Then
                        dir = 1
                    Else
                        pk = p.key
                        If pk Is k OrElse (k IsNot Nothing AndAlso k.Equals(pk)) Then
                            Return p
                        Else
                            kc = comparableClassFor(k)
                            dir = compareComparables(kc, k, pk)
                            If (kc Is Nothing AndAlso kc Is Nothing) OrElse dir = 0 Then
                                If Not searched Then
                                    Dim q As TreeNode(Of K, V), ch As TreeNode(Of K, V)
                                    searched = True
                                    ch = p.left
                                    q = ch.find(h, k, kc)
                                    ch = p.right
                                    q = ch.find(h, k, kc)
                                    If (ch IsNot Nothing AndAlso q IsNot Nothing) OrElse (ch IsNot Nothing AndAlso q IsNot Nothing) Then Return q
                                End If
                                dir = tieBreakOrder(k, pk)
                            End If
                        End If
                    End If

                    Dim xp As TreeNode(Of K, V) = p
                    p = If(dir <= 0, p.left, p.right)
                    If p Is Nothing Then
                        Dim xpn As Node(Of K, V) = xp.next
                        Dim x As TreeNode(Of K, V) = map.newTreeNode(h, k, v, xpn)
                        If dir <= 0 Then
                            xp.left = x
                        Else
                            xp.right = x
                        End If
                        xp.next = x
                        x.prev = xp
                        x.parent = x.prev
                        If xpn IsNot Nothing Then CType(xpn, TreeNode(Of K, V)).prev = x
                        moveRootToFront(tab, balanceInsertion(root, x))
                        Return Nothing
                    End If
                Loop
            End Function

            ''' <summary>
            ''' Removes the given node, that must be present before this call.
            ''' This is messier than typical red-black deletion code because we
            ''' cannot swap the contents of an interior node with a leaf
            ''' successor that is pinned by "next" pointers that are accessible
            ''' independently during traversal. So instead we swap the tree
            ''' linkages. If the current tree appears to have too few nodes,
            ''' the bin is converted back to a plain bin. (The test triggers
            ''' somewhere between 2 and 6 nodes, depending on tree structure).
            ''' </summary>
            Friend Sub removeTreeNode(ByVal map As HashMap(Of K, V), ByVal tab As Node(Of K, V)(), ByVal movable As Boolean)
                Dim n As Integer
                n = tab.Length
                If tab Is Nothing OrElse n = 0 Then Return
                Dim index As Integer = (n - 1) And hash
                Dim first As TreeNode(Of K, V) = CType(tab(index), TreeNode(Of K, V)), root As TreeNode(Of K, V) = first, rl As TreeNode(Of K, V)
                Dim succ As TreeNode(Of K, V) = CType([next], TreeNode(Of K, V)), pred As TreeNode(Of K, V) = prev
                If pred Is Nothing Then
                    first = succ
                    tab(index) = first
                Else
                    pred.next = succ
                End If
                If succ IsNot Nothing Then succ.prev = pred
                If first Is Nothing Then Return
                If root.parent IsNot Nothing Then root = root.root()
                rl = root.left
                If root Is Nothing OrElse root.right Is Nothing OrElse rl Is Nothing OrElse rl.left Is Nothing Then
                    tab(index) = first.untreeify(map) ' too small
                    Return
                End If
                Dim p As TreeNode(Of K, V) = Me, pl As TreeNode(Of K, V) = left, pr As TreeNode(Of K, V) = right, replacement As TreeNode(Of K, V)
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
                        root = s
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
                        root = replacement
                    ElseIf p Is pp.left Then
                        pp.left = replacement
                    Else
                        pp.right = replacement
                    End If
                    p.parent = Nothing
                    p.right = p.parent
                    p.left = p.right
                End If

                Dim r As TreeNode(Of K, V) = If(p.red, root, balanceDeletion(root, replacement))

                If replacement Is p Then ' detach
                    Dim pp As TreeNode(Of K, V) = p.parent
                    p.parent = Nothing
                    If pp IsNot Nothing Then
                        If p Is pp.left Then
                            pp.left = Nothing
                        ElseIf p Is pp.right Then
                            pp.right = Nothing
                        End If
                    End If
                End If
                If movable Then moveRootToFront(tab, r)
            End Sub

            ''' <summary>
            ''' Splits nodes in a tree bin into lower and upper tree bins,
            ''' or untreeifies if now too small. Called only from resize;
            ''' see above discussion about split bits and indices.
            ''' </summary>
            ''' <param name="map"> the map </param>
            ''' <param name="tab"> the table for recording bin heads </param>
            ''' <param name="index"> the index of the table being split </param>
            ''' <param name="bit"> the bit of hash to split on </param>
            Friend Sub split(ByVal map As HashMap(Of K, V), ByVal tab As Node(Of K, V)(), ByVal index As Integer, ByVal bit As Integer)
                Dim b As TreeNode(Of K, V) = Me
                ' Relink into lo and hi lists, preserving order
                Dim loHead As TreeNode(Of K, V) = Nothing, loTail As TreeNode(Of K, V) = Nothing
                Dim hiHead As TreeNode(Of K, V) = Nothing, hiTail As TreeNode(Of K, V) = Nothing
                Dim lc As Integer = 0, hc As Integer = 0
                'JAVA TO VB CONVERTER TODO TASK: The following line could not be converted:
                For (TreeNode < K,V> e = b, next; e != Nothing; e = next)
					[next] = CType(e.next, TreeNode(Of K, V))
                    e.next = Nothing
                    If (e.hash And bit) = 0 Then
                        e.prev = loTail
                        If e.prev Is Nothing Then
                            loHead = e
                        Else
                            loTail.next = e
                        End If
                        loTail = e
                        lc += 1
                    Else
                        e.prev = hiTail
                        If e.prev Is Nothing Then
                            hiHead = e
                        Else
                            hiTail.next = e
                        End If
                        hiTail = e
                        hc += 1
                    End If

                    If loHead IsNot Nothing Then
                        If lc <= UNTREEIFY_THRESHOLD Then
                            tab(index) = loHead.untreeify(map)
                        Else
                            tab(index) = loHead
                            If hiHead IsNot Nothing Then ' (else is already treeified) loHead.treeify(tab)
                            End If
                        End If
                        If hiHead IsNot Nothing Then
                            If hc <= UNTREEIFY_THRESHOLD Then
                                tab(index + bit) = hiHead.untreeify(map)
                            Else
                                tab(index + bit) = hiHead
                                If loHead IsNot Nothing Then hiHead.treeify(tab)
                            End If
                        End If
            End Sub

            ' ------------------------------------------------------------ 
            ' Red-black tree methods, all adapted from CLR

            Shared Function rotateLeft(Of K, V)(ByVal root As TreeNode(Of K, V), ByVal p As TreeNode(Of K, V)) As TreeNode(Of K, V)
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

            Shared Function rotateRight(Of K, V)(ByVal root As TreeNode(Of K, V), ByVal p As TreeNode(Of K, V)) As TreeNode(Of K, V)
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

            Shared Function balanceInsertion(Of K, V)(ByVal root As TreeNode(Of K, V), ByVal x As TreeNode(Of K, V)) As TreeNode(Of K, V)
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

            Shared Function balanceDeletion(Of K, V)(ByVal root As TreeNode(Of K, V), ByVal x As TreeNode(Of K, V)) As TreeNode(Of K, V)
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
            Shared Function checkInvariants(Of K, V)(ByVal t As TreeNode(Of K, V)) As Boolean
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
        End Class

    End Class

End Namespace