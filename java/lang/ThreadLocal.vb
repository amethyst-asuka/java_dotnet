Imports System.Threading

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

Namespace java.lang

	''' <summary>
	''' This class provides thread-local variables.  These variables differ from
	''' their normal counterparts in that each thread that accesses one (via its
	''' {@code get} or {@code set} method) has its own, independently initialized
	''' copy of the variable.  {@code ThreadLocal} instances are typically private
	''' static fields in classes that wish to associate state with a thread (e.g.,
	''' a user ID or Transaction ID).
	''' 
	''' <p>For example, the class below generates unique identifiers local to each
	''' thread.
	''' A thread's id is assigned the first time it invokes {@code ThreadId.get()}
	''' and remains unchanged on subsequent calls.
	''' <pre>
	''' import java.util.concurrent.atomic.AtomicInteger;
	''' 
	''' public class ThreadId {
	'''     // Atomic integer containing the next thread ID to be assigned
	'''     private static final AtomicInteger nextId = new AtomicInteger(0);
	''' 
	'''     // Thread local variable containing each thread's ID
	'''     private static final ThreadLocal&lt;Integer&gt; threadId =
	'''         new ThreadLocal&lt;Integer&gt;() {
	'''             &#64;Override protected Integer initialValue() {
	'''                 return nextId.getAndIncrement();
	'''         }
	'''     };
	''' 
	'''     // Returns the current thread's unique ID, assigning it if necessary
	'''     public static int get() {
	'''         return threadId.get();
	'''     }
	''' }
	''' </pre>
	''' <p>Each thread holds an implicit reference to its copy of a thread-local
	''' variable as long as the thread is alive and the {@code ThreadLocal}
	''' instance is accessible; after a thread goes away, all of its copies of
	''' thread-local instances are subject to garbage collection (unless other
	''' references to these copies exist).
	''' 
	''' @author  Josh Bloch and Doug Lea
	''' @since   1.2
	''' </summary>
	Public Class ThreadLocal(Of T)
		''' <summary>
		''' ThreadLocals rely on per-thread linear-probe hash maps attached
		''' to each thread (Thread.threadLocals and
		''' inheritableThreadLocals).  The ThreadLocal objects act as keys,
		''' searched via threadLocalHashCode.  This is a custom hash code
		''' (useful only within ThreadLocalMaps) that eliminates collisions
		''' in the common case where consecutively constructed ThreadLocals
		''' are used by the same threads, while remaining well-behaved in
		''' less common cases.
		''' </summary>
		Private ReadOnly threadLocalHashCode As Integer = nextHashCode()

		''' <summary>
		''' The next hash code to be given out. Updated atomically. Starts at
		''' zero.
		''' </summary>
		Private Shared nextHashCode_Renamed As New java.util.concurrent.atomic.AtomicInteger

		''' <summary>
		''' The difference between successively generated hash codes - turns
		''' implicit sequential thread-local IDs into near-optimally spread
		''' multiplicative hash values for power-of-two-sized tables.
		''' </summary>
		Private Const HASH_INCREMENT As Integer = &H61c88647

		''' <summary>
		''' Returns the next hash code.
		''' </summary>
		Private Shared Function nextHashCode() As Integer
			Return nextHashCode_Renamed.getAndAdd(HASH_INCREMENT)
		End Function

		''' <summary>
		''' Returns the current thread's "initial value" for this
		''' thread-local variable.  This method will be invoked the first
		''' time a thread accesses the variable with the <seealso cref="#get"/>
		''' method, unless the thread previously invoked the <seealso cref="#set"/>
		''' method, in which case the {@code initialValue} method will not
		''' be invoked for the thread.  Normally, this method is invoked at
		''' most once per thread, but it may be invoked again in case of
		''' subsequent invocations of <seealso cref="#remove"/> followed by <seealso cref="#get"/>.
		''' 
		''' <p>This implementation simply returns {@code null}; if the
		''' programmer desires thread-local variables to have an initial
		''' value other than {@code null}, {@code ThreadLocal} must be
		''' subclassed, and this method overridden.  Typically, an
		''' anonymous inner class will be used.
		''' </summary>
		''' <returns> the initial value for this thread-local </returns>
		Protected Friend Overridable Function initialValue() As T
			Return Nothing
		End Function

		''' <summary>
		''' Creates a thread local variable. The initial value of the variable is
		''' determined by invoking the {@code get} method on the {@code Supplier}.
		''' </summary>
		''' @param <S> the type of the thread local's value </param>
		''' <param name="supplier"> the supplier to be used to determine the initial value </param>
		''' <returns> a new thread local variable </returns>
		''' <exception cref="NullPointerException"> if the specified supplier is null
		''' @since 1.8 </exception>
		Public Shared Function withInitial(Of S, T1 As S)(ByVal supplier As java.util.function.Supplier(Of T1)) As ThreadLocal(Of S)
			Return New SuppliedThreadLocal(Of )(supplier)
		End Function

		''' <summary>
		''' Creates a thread local variable. </summary>
		''' <seealso cref= #withInitial(java.util.function.Supplier) </seealso>
		Public Sub New()
		End Sub

		''' <summary>
		''' Returns the value in the current thread's copy of this
		''' thread-local variable.  If the variable has no value for the
		''' current thread, it is first initialized to the value returned
		''' by an invocation of the <seealso cref="#initialValue"/> method.
		''' </summary>
		''' <returns> the current thread's value of this thread-local </returns>
		Public Overridable Function [get]() As T
			Dim t As Thread = Thread.CurrentThread
			Dim map_Renamed As ThreadLocalMap = getMap(t)
			If map_Renamed IsNot Nothing Then
				Dim e As ThreadLocalMap.Entry = map_Renamed.getEntry(Me)
				If e IsNot Nothing Then
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
					Dim result As T = CType(e.value, T)
					Return result
				End If
			End If
			Return initialValuelue()
		End Function

		''' <summary>
		''' Variant of set() to establish initialValue. Used instead
		''' of set() in case user has overridden the set() method.
		''' </summary>
		''' <returns> the initial value </returns>
		Private Function setInitialValue() As T
			Dim value As T = initialValue()
			Dim t As Thread = Thread.CurrentThread
			Dim map_Renamed As ThreadLocalMap = getMap(t)
			If map_Renamed IsNot Nothing Then
				map_Renamed.set(Me, value)
			Else
				createMap(t, value)
			End If
			Return value
		End Function

		''' <summary>
		''' Sets the current thread's copy of this thread-local variable
		''' to the specified value.  Most subclasses will have no need to
		''' override this method, relying solely on the <seealso cref="#initialValue"/>
		''' method to set the values of thread-locals.
		''' </summary>
		''' <param name="value"> the value to be stored in the current thread's copy of
		'''        this thread-local. </param>
		Public Overridable Sub [set](ByVal value As T)
			Dim t As Thread = Thread.CurrentThread
			Dim map_Renamed As ThreadLocalMap = getMap(t)
			If map_Renamed IsNot Nothing Then
				map_Renamed.set(Me, value)
			Else
				createMap(t, value)
			End If
		End Sub

		''' <summary>
		''' Removes the current thread's value for this thread-local
		''' variable.  If this thread-local variable is subsequently
		''' <seealso cref="#get read"/> by the current thread, its value will be
		''' reinitialized by invoking its <seealso cref="#initialValue"/> method,
		''' unless its value is <seealso cref="#set set"/> by the current thread
		''' in the interim.  This may result in multiple invocations of the
		''' {@code initialValue} method in the current thread.
		''' 
		''' @since 1.5
		''' </summary>
		 Public Overridable Sub remove()
			 Dim m As ThreadLocalMap = getMap(Thread.CurrentThread)
			 If m IsNot Nothing Then m.remove(Me)
		 End Sub

		''' <summary>
		''' Get the map associated with a ThreadLocal. Overridden in
		''' InheritableThreadLocal.
		''' </summary>
		''' <param name="t"> the current thread </param>
		''' <returns> the map </returns>
		Friend Overridable Function getMap(ByVal t As Thread) As ThreadLocalMap
			Return t.threadLocals
		End Function

		''' <summary>
		''' Create the map associated with a ThreadLocal. Overridden in
		''' InheritableThreadLocal.
		''' </summary>
		''' <param name="t"> the current thread </param>
		''' <param name="firstValue"> value for the initial entry of the map </param>
		Friend Overridable Sub createMap(ByVal t As Thread, ByVal firstValue As T)
			t.threadLocals = New ThreadLocalMap(Me, firstValue)
		End Sub

		''' <summary>
		''' Factory method to create map of inherited thread locals.
		''' Designed to be called only from Thread constructor.
		''' </summary>
		''' <param name="parentMap"> the map associated with parent thread </param>
		''' <returns> a map containing the parent's inheritable bindings </returns>
		Friend Shared Function createInheritedMap(ByVal parentMap As ThreadLocalMap) As ThreadLocalMap
			Return New ThreadLocalMap(parentMap)
		End Function

		''' <summary>
		''' Method childValue is visibly defined in subclass
		''' InheritableThreadLocal, but is internally defined here for the
		''' sake of providing createInheritedMap factory method without
		''' needing to subclass the map class in InheritableThreadLocal.
		''' This technique is preferable to the alternative of embedding
		''' instanceof tests in methods.
		''' </summary>
		Friend Overridable Function childValue(ByVal parentValue As T) As T
			Throw New UnsupportedOperationException
		End Function

		''' <summary>
		''' An extension of ThreadLocal that obtains its initial value from
		''' the specified {@code Supplier}.
		''' </summary>
		Friend NotInheritable Class SuppliedThreadLocal(Of T)
			Inherits ThreadLocal(Of T)

'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
			Private ReadOnly supplier As java.util.function.Supplier(Of ? As T)

			Friend Sub New(Of T1 As T)(ByVal supplier As java.util.function.Supplier(Of T1))
				Me.supplier = java.util.Objects.requireNonNull(supplier)
			End Sub

			Protected Friend Overrides Function initialValue() As T
				Return supplier.get()
			End Function
		End Class

		''' <summary>
		''' ThreadLocalMap is a customized hash map suitable only for
		''' maintaining thread local values. No operations are exported
		''' outside of the ThreadLocal class. The class is package private to
		''' allow declaration of fields in class Thread.  To help deal with
		''' very large and long-lived usages, the hash table entries use
		''' WeakReferences for keys. However, since reference queues are not
		''' used, stale entries are guaranteed to be removed only when
		''' the table starts running out of space.
		''' </summary>
		Friend Class ThreadLocalMap

			''' <summary>
			''' The entries in this hash map extend WeakReference, using
			''' its main ref field as the key (which is always a
			''' ThreadLocal object).  Note that null keys (i.e. entry.get()
			''' == null) mean that the key is no longer referenced, so the
			''' entry can be expunged from table.  Such entries are referred to
			''' as "stale entries" in the code that follows.
			''' </summary>
			Friend Class Entry
				Inherits WeakReference(Of ThreadLocal(Of JavaToDotNetGenericWildcard))

				''' <summary>
				''' The value associated with this ThreadLocal. </summary>
				Friend value As Object

				Friend Sub New(Of T1)(ByVal k As ThreadLocal(Of T1), ByVal v As Object)
					MyBase.New(k)
					value = v
				End Sub
			End Class

			''' <summary>
			''' The initial capacity -- MUST be a power of two.
			''' </summary>
			Private Const INITIAL_CAPACITY As Integer = 16

			''' <summary>
			''' The table, resized as necessary.
			''' table.length MUST always be a power of two.
			''' </summary>
			Private table As Entry()

			''' <summary>
			''' The number of entries in the table.
			''' </summary>
			Private size As Integer = 0

			''' <summary>
			''' The next size value at which to resize.
			''' </summary>
			Private threshold As Integer ' Default to 0

			''' <summary>
			''' Set the resize threshold to maintain at worst a 2/3 load factor.
			''' </summary>
			Private Property threshold As Integer
				Set(ByVal len As Integer)
					threshold = len * 2 \ 3
				End Set
			End Property

			''' <summary>
			''' Increment i modulo len.
			''' </summary>
			Private Shared Function nextIndex(ByVal i As Integer, ByVal len As Integer) As Integer
				Return (If(i + 1 < len, i + 1, 0))
			End Function

			''' <summary>
			''' Decrement i modulo len.
			''' </summary>
			Private Shared Function prevIndex(ByVal i As Integer, ByVal len As Integer) As Integer
				Return (If(i - 1 >= 0, i - 1, len - 1))
			End Function

			''' <summary>
			''' Construct a new map initially containing (firstKey, firstValue).
			''' ThreadLocalMaps are constructed lazily, so we only create
			''' one when we have at least one entry to put in it.
			''' </summary>
			Friend Sub New(Of T1)(ByVal firstKey As ThreadLocal(Of T1), ByVal firstValue As Object)
				table = New Entry(INITIAL_CAPACITY - 1){}
				Dim i As Integer = firstKey.threadLocalHashCode And (INITIAL_CAPACITY - 1)
				table(i) = New Entry(firstKey, firstValue)
				size = 1
				threshold = INITIAL_CAPACITY
			End Sub

			''' <summary>
			''' Construct a new map including all Inheritable ThreadLocals
			''' from given parent map. Called only by createInheritedMap.
			''' </summary>
			''' <param name="parentMap"> the map associated with parent thread. </param>
			Private Sub New(ByVal parentMap As ThreadLocalMap)
				Dim parentTable As Entry() = parentMap.table
				Dim len As Integer = parentTable.Length
				threshold = len
				table = New Entry(len - 1){}

				For j As Integer = 0 To len - 1
					Dim e As Entry = parentTable(j)
					If e IsNot Nothing Then
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
						Dim key As ThreadLocal(Of Object) = CType(e.get(), ThreadLocal(Of Object))
						If key IsNot Nothing Then
							Dim value As Object = key.childValue(e.value)
							Dim c As New Entry(key, value)
							Dim h As Integer = key.threadLocalHashCode And (len - 1)
							Do While table(h) IsNot Nothing
								h = nextIndex(h, len)
							Loop
							table(h) = c
							size += 1
						End If
					End If
				Next j
			End Sub

			''' <summary>
			''' Get the entry associated with key.  This method
			''' itself handles only the fast path: a direct hit of existing
			''' key. It otherwise relays to getEntryAfterMiss.  This is
			''' designed to maximize performance for direct hits, in part
			''' by making this method readily inlinable.
			''' </summary>
			''' <param name="key"> the thread local object </param>
			''' <returns> the entry associated with key, or null if no such </returns>
			Private Function getEntry(Of T1)(ByVal key As ThreadLocal(Of T1)) As Entry
				Dim i As Integer = key.threadLocalHashCode And (table.Length - 1)
				Dim e As Entry = table(i)
				If e IsNot Nothing AndAlso e.get() Is key Then
					Return e
				Else
					Return getEntryAfterMiss(key, i, e)
				End If
			End Function

			''' <summary>
			''' Version of getEntry method for use when key is not found in
			''' its direct hash slot.
			''' </summary>
			''' <param name="key"> the thread local object </param>
			''' <param name="i"> the table index for key's hash code </param>
			''' <param name="e"> the entry at table[i] </param>
			''' <returns> the entry associated with key, or null if no such </returns>
			Private Function getEntryAfterMiss(Of T1)(ByVal key As ThreadLocal(Of T1), ByVal i As Integer, ByVal e As Entry) As Entry
				Dim tab As Entry() = table
				Dim len As Integer = tab.Length

				Do While e IsNot Nothing
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
					Dim k As ThreadLocal(Of ?) = e.get()
					If k Is key Then Return e
					If k Is Nothing Then
						expungeStaleEntry(i)
					Else
						i = nextIndex(i, len)
					End If
					e = tab(i)
				Loop
				Return Nothing
			End Function

			''' <summary>
			''' Set the value associated with key.
			''' </summary>
			''' <param name="key"> the thread local object </param>
			''' <param name="value"> the value to be set </param>
			Private Sub [set](Of T1)(ByVal key As ThreadLocal(Of T1), ByVal value As Object)

				' We don't use a fast path as with get() because it is at
				' least as common to use set() to create new entries as
				' it is to replace existing ones, in which case, a fast
				' path would fail more often than not.

				Dim tab As Entry() = table
				Dim len As Integer = tab.Length
				Dim i As Integer = key.threadLocalHashCode And (len-1)

				Dim e As Entry = tab(i)
'JAVA TO VB CONVERTER TODO TASK: Assignments within expressions are not supported in VB
				Do While e IsNot Nothing
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
					Dim k As ThreadLocal(Of ?) = e.get()

					If k Is key Then
						e.value = value
						Return
					End If

					If k Is Nothing Then
						replaceStaleEntry(key, value, i)
						Return
					End If
					e = tab(i = nextIndex(i, len))
				Loop

				tab(i) = New Entry(key, value)
				size += 1
				Dim sz As Integer = size
				If (Not cleanSomeSlots(i, sz)) AndAlso sz >= threshold Then rehash()
			End Sub

			''' <summary>
			''' Remove the entry for key.
			''' </summary>
			Private Sub remove(Of T1)(ByVal key As ThreadLocal(Of T1))
				Dim tab As Entry() = table
				Dim len As Integer = tab.Length
				Dim i As Integer = key.threadLocalHashCode And (len-1)
				Dim e As Entry = tab(i)
'JAVA TO VB CONVERTER TODO TASK: Assignments within expressions are not supported in VB
				Do While e IsNot Nothing
					If e.get() Is key Then
						e.clear()
						expungeStaleEntry(i)
						Return
					End If
					e = tab(i = nextIndex(i, len))
				Loop
			End Sub

			''' <summary>
			''' Replace a stale entry encountered during a set operation
			''' with an entry for the specified key.  The value passed in
			''' the value parameter is stored in the entry, whether or not
			''' an entry already exists for the specified key.
			''' 
			''' As a side effect, this method expunges all stale entries in the
			''' "run" containing the stale entry.  (A run is a sequence of entries
			''' between two null slots.)
			''' </summary>
			''' <param name="key"> the key </param>
			''' <param name="value"> the value to be associated with key </param>
			''' <param name="staleSlot"> index of the first stale entry encountered while
			'''         searching for key. </param>
			Private Sub replaceStaleEntry(Of T1)(ByVal key As ThreadLocal(Of T1), ByVal value As Object, ByVal staleSlot As Integer)
				Dim tab As Entry() = table
				Dim len As Integer = tab.Length
				Dim e As Entry

				' Back up to check for prior stale entry in current run.
				' We clean out whole runs at a time to avoid continual
				' incremental rehashing due to garbage collector freeing
				' up refs in bunches (i.e., whenever the collector runs).
				Dim slotToExpunge As Integer = staleSlot
				Dim i As Integer = prevIndex(staleSlot, len)
'JAVA TO VB CONVERTER TODO TASK: Assignments within expressions are not supported in VB
				Do While (e = tab(i)) IsNot Nothing
					If e.get() Is Nothing Then slotToExpunge = i
					i = prevIndex(i, len)
				Loop

				' Find either the key or trailing null slot of run, whichever
				' occurs first
				i = nextIndex(staleSlot, len)
'JAVA TO VB CONVERTER TODO TASK: Assignments within expressions are not supported in VB
				Do While (e = tab(i)) IsNot Nothing
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
					Dim k As ThreadLocal(Of ?) = e.get()

					' If we find key, then we need to swap it
					' with the stale entry to maintain hash table order.
					' The newly stale slot, or any other stale slot
					' encountered above it, can then be sent to expungeStaleEntry
					' to remove or rehash all of the other entries in run.
					If k Is key Then
						e.value = value

						tab(i) = tab(staleSlot)
						tab(staleSlot) = e

						' Start expunge at preceding stale entry if it exists
						If slotToExpunge = staleSlot Then slotToExpunge = i
						cleanSomeSlots(expungeStaleEntry(slotToExpunge), len)
						Return
					End If

					' If we didn't find stale entry on backward scan, the
					' first stale entry seen while scanning for key is the
					' first still present in the run.
					If k Is Nothing AndAlso slotToExpunge = staleSlot Then slotToExpunge = i
					i = nextIndex(i, len)
				Loop

				' If key not found, put new entry in stale slot
				tab(staleSlot).value = Nothing
				tab(staleSlot) = New Entry(key, value)

				' If there are any other stale entries in run, expunge them
				If slotToExpunge <> staleSlot Then cleanSomeSlots(expungeStaleEntry(slotToExpunge), len)
			End Sub

			''' <summary>
			''' Expunge a stale entry by rehashing any possibly colliding entries
			''' lying between staleSlot and the next null slot.  This also expunges
			''' any other stale entries encountered before the trailing null.  See
			''' Knuth, Section 6.4
			''' </summary>
			''' <param name="staleSlot"> index of slot known to have null key </param>
			''' <returns> the index of the next null slot after staleSlot
			''' (all between staleSlot and this slot will have been checked
			''' for expunging). </returns>
			Private Function expungeStaleEntry(ByVal staleSlot As Integer) As Integer
				Dim tab As Entry() = table
				Dim len As Integer = tab.Length

				' expunge entry at staleSlot
				tab(staleSlot).value = Nothing
				tab(staleSlot) = Nothing
				size -= 1

				' Rehash until we encounter null
				Dim e As Entry
				Dim i As Integer
				i = nextIndex(staleSlot, len)
'JAVA TO VB CONVERTER TODO TASK: Assignments within expressions are not supported in VB
				Do While (e = tab(i)) IsNot Nothing
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
					Dim k As ThreadLocal(Of ?) = e.get()
					If k Is Nothing Then
						e.value = Nothing
						tab(i) = Nothing
						size -= 1
					Else
						Dim h As Integer = k.threadLocalHashCode And (len - 1)
						If h <> i Then
							tab(i) = Nothing

							' Unlike Knuth 6.4 Algorithm R, we must scan until
							' null because multiple entries could have been stale.
							Do While tab(h) IsNot Nothing
								h = nextIndex(h, len)
							Loop
							tab(h) = e
						End If
					End If
					i = nextIndex(i, len)
				Loop
				Return i
			End Function

			''' <summary>
			''' Heuristically scan some cells looking for stale entries.
			''' This is invoked when either a new element is added, or
			''' another stale one has been expunged. It performs a
			''' logarithmic number of scans, as a balance between no
			''' scanning (fast but retains garbage) and a number of scans
			''' proportional to number of elements, that would find all
			''' garbage but would cause some insertions to take O(n) time.
			''' </summary>
			''' <param name="i"> a position known NOT to hold a stale entry. The
			''' scan starts at the element after i.
			''' </param>
			''' <param name="n"> scan control: {@code log2(n)} cells are scanned,
			''' unless a stale entry is found, in which case
			''' {@code log2(table.length)-1} additional cells are scanned.
			''' When called from insertions, this parameter is the number
			''' of elements, but when from replaceStaleEntry, it is the
			''' table length. (Note: all this could be changed to be either
			''' more or less aggressive by weighting n instead of just
			''' using straight log n. But this version is simple, fast, and
			''' seems to work well.)
			''' </param>
			''' <returns> true if any stale entries have been removed. </returns>
			Private Function cleanSomeSlots(ByVal i As Integer, ByVal n As Integer) As Boolean
				Dim removed As Boolean = False
				Dim tab As Entry() = table
				Dim len As Integer = tab.Length
				Do
					i = nextIndex(i, len)
					Dim e As Entry = tab(i)
					If e IsNot Nothing AndAlso e.get() Is Nothing Then
						n = len
						removed = True
						i = expungeStaleEntry(i)
					End If
				Loop While (n >>>= 1) <> 0
				Return removed
			End Function

			''' <summary>
			''' Re-pack and/or re-size the table. First scan the entire
			''' table removing stale entries. If this doesn't sufficiently
			''' shrink the size of the table, double the table size.
			''' </summary>
			Private Sub rehash()
				expungeStaleEntries()

				' Use lower threshold for doubling to avoid hysteresis
				If size >= threshold - threshold \ 4 Then resize()
			End Sub

			''' <summary>
			''' Double the capacity of the table.
			''' </summary>
			Private Sub resize()
				Dim oldTab As Entry() = table
				Dim oldLen As Integer = oldTab.Length
				Dim newLen As Integer = oldLen * 2
				Dim newTab As Entry() = New Entry(newLen - 1){}
				Dim count As Integer = 0

				For j As Integer = 0 To oldLen - 1
					Dim e As Entry = oldTab(j)
					If e IsNot Nothing Then
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
						Dim k As ThreadLocal(Of ?) = e.get()
						If k Is Nothing Then
							e.value = Nothing ' Help the GC
						Else
							Dim h As Integer = k.threadLocalHashCode And (newLen - 1)
							Do While newTab(h) IsNot Nothing
								h = nextIndex(h, newLen)
							Loop
							newTab(h) = e
							count += 1
						End If
					End If
				Next j

				threshold = newLen
				size = count
				table = newTab
			End Sub

			''' <summary>
			''' Expunge all stale entries in the table.
			''' </summary>
			Private Sub expungeStaleEntries()
				Dim tab As Entry() = table
				Dim len As Integer = tab.Length
				For j As Integer = 0 To len - 1
					Dim e As Entry = tab(j)
					If e IsNot Nothing AndAlso e.get() Is Nothing Then expungeStaleEntry(j)
				Next j
			End Sub
		End Class
	End Class

End Namespace