Imports Microsoft.VisualBasic
Imports System
Imports System.Runtime.CompilerServices

'
' * Copyright (c) 2010, 2013, Oracle and/or its affiliates. All rights reserved.
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
	''' Lazily associate a computed value with (potentially) every type.
	''' For example, if a dynamic language needs to construct a message dispatch
	''' table for each class encountered at a message send call site,
	''' it can use a {@code ClassValue} to cache information needed to
	''' perform the message send quickly, for each class encountered.
	''' @author John Rose, JSR 292 EG
	''' @since 1.7
	''' </summary>
	Public MustInherit Class ClassValue(Of T)
		''' <summary>
		''' Sole constructor.  (For invocation by subclass constructors, typically
		''' implicit.)
		''' </summary>
		Protected Friend Sub New()
		End Sub

		''' <summary>
		''' Computes the given class's derived value for this {@code ClassValue}.
		''' <p>
		''' This method will be invoked within the first thread that accesses
		''' the value with the <seealso cref="#get get"/> method.
		''' <p>
		''' Normally, this method is invoked at most once per class,
		''' but it may be invoked again if there has been a call to
		''' <seealso cref="#remove remove"/>.
		''' <p>
		''' If this method throws an exception, the corresponding call to {@code get}
		''' will terminate abnormally with that exception, and no class value will be recorded.
		''' </summary>
		''' <param name="type"> the type whose class value must be computed </param>
		''' <returns> the newly computed value associated with this {@code ClassValue}, for the given class or interface </returns>
		''' <seealso cref= #get </seealso>
		''' <seealso cref= #remove </seealso>
		Protected Friend MustOverride Function computeValue(ByVal type As Class) As T

		''' <summary>
		''' Returns the value for the given class.
		''' If no value has yet been computed, it is obtained by
		''' an invocation of the <seealso cref="#computeValue computeValue"/> method.
		''' <p>
		''' The actual installation of the value on the class
		''' is performed atomically.
		''' At that point, if several racing threads have
		''' computed values, one is chosen, and returned to
		''' all the racing threads.
		''' <p>
		''' The {@code type} parameter is typically a class, but it may be any type,
		''' such as an interface, a primitive type (like {@code int.class}), or {@code void.class}.
		''' <p>
		''' In the absence of {@code remove} calls, a class value has a simple
		''' state diagram:  uninitialized and initialized.
		''' When {@code remove} calls are made,
		''' the rules for value observation are more complex.
		''' See the documentation for <seealso cref="#remove remove"/> for more information.
		''' </summary>
		''' <param name="type"> the type whose class value must be computed or retrieved </param>
		''' <returns> the current value associated with this {@code ClassValue}, for the given class or interface </returns>
		''' <exception cref="NullPointerException"> if the argument is null </exception>
		''' <seealso cref= #remove </seealso>
		''' <seealso cref= #computeValue </seealso>
		Public Overridable Function [get](ByVal type As Class) As T
			' non-racing this.hashCodeForCache : final int
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
			Dim cache As Entry(Of ?)()
'JAVA TO VB CONVERTER TODO TASK: Assignments within expressions are not supported in VB
			Dim e As Entry(Of T) = probeHomeLocation(cache = getCacheCarefully(type), Me)
			' racing e : current value <=> stale value from current cache or from stale cache
			' invariant:  e is null or an Entry with readable Entry.version and Entry.value
			If match(e) Then Return e.value()
			' The fast path can fail for any of these reasons:
			' 1. no entry has been computed yet
			' 2. hash code collision (before or after reduction mod cache.length)
			' 3. an entry has been removed (either on this type or another)
			' 4. the GC has somehow managed to delete e.version and clear the reference
			Return getFromBackup(cache, type)
		End Function

		''' <summary>
		''' Removes the associated value for the given class.
		''' If this value is subsequently <seealso cref="#get read"/> for the same class,
		''' its value will be reinitialized by invoking its <seealso cref="#computeValue computeValue"/> method.
		''' This may result in an additional invocation of the
		''' {@code computeValue} method for the given class.
		''' <p>
		''' In order to explain the interaction between {@code get} and {@code remove} calls,
		''' we must model the state transitions of a class value to take into account
		''' the alternation between uninitialized and initialized states.
		''' To do this, number these states sequentially from zero, and note that
		''' uninitialized (or removed) states are numbered with even numbers,
		''' while initialized (or re-initialized) states have odd numbers.
		''' <p>
		''' When a thread {@code T} removes a class value in state {@code 2N},
		''' nothing happens, since the class value is already uninitialized.
		''' Otherwise, the state is advanced atomically to {@code 2N+1}.
		''' <p>
		''' When a thread {@code T} queries a class value in state {@code 2N},
		''' the thread first attempts to initialize the class value to state {@code 2N+1}
		''' by invoking {@code computeValue} and installing the resulting value.
		''' <p>
		''' When {@code T} attempts to install the newly computed value,
		''' if the state is still at {@code 2N}, the class value will be initialized
		''' with the computed value, advancing it to state {@code 2N+1}.
		''' <p>
		''' Otherwise, whether the new state is even or odd,
		''' {@code T} will discard the newly computed value
		''' and retry the {@code get} operation.
		''' <p>
		''' Discarding and retrying is an important proviso,
		''' since otherwise {@code T} could potentially install
		''' a disastrously stale value.  For example:
		''' <ul>
		''' <li>{@code T} calls {@code CV.get(C)} and sees state {@code 2N}
		''' <li>{@code T} quickly computes a time-dependent value {@code V0} and gets ready to install it
		''' <li>{@code T} is hit by an unlucky paging or scheduling event, and goes to sleep for a long time
		''' <li>...meanwhile, {@code T2} also calls {@code CV.get(C)} and sees state {@code 2N}
		''' <li>{@code T2} quickly computes a similar time-dependent value {@code V1} and installs it on {@code CV.get(C)}
		''' <li>{@code T2} (or a third thread) then calls {@code CV.remove(C)}, undoing {@code T2}'s work
		''' <li> the previous actions of {@code T2} are repeated several times
		''' <li> also, the relevant computed values change over time: {@code V1}, {@code V2}, ...
		''' <li>...meanwhile, {@code T} wakes up and attempts to install {@code V0}; <em>this must fail</em>
		''' </ul>
		''' We can assume in the above scenario that {@code CV.computeValue} uses locks to properly
		''' observe the time-dependent states as it computes {@code V1}, etc.
		''' This does not remove the threat of a stale value, since there is a window of time
		''' between the return of {@code computeValue} in {@code T} and the installation
		''' of the the new value.  No user synchronization is possible during this time.
		''' </summary>
		''' <param name="type"> the type whose class value must be removed </param>
		''' <exception cref="NullPointerException"> if the argument is null </exception>
		Public Overridable Sub remove(ByVal type As Class)
			Dim map_Renamed As ClassValueMap = getMap(type)
			map_Renamed.removeEntry(Me)
		End Sub

		' Possible functionality for JSR 292 MR 1
		'public
	 Friend Overridable Sub put(ByVal type As Class, ByVal value As T)
			Dim map_Renamed As ClassValueMap = getMap(type)
			map_Renamed.changeEntry(Me, value)
	 End Sub

		'/ --------
		'/ Implementation...
		'/ --------

		''' <summary>
		''' Return the cache, if it exists, else a dummy empty cache. </summary>
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
		Private Shared Function getCacheCarefully(ByVal type As Class) As Entry(Of ?)()
			' racing type.classValueMap{.cacheArray} : null => new Entry[X] <=> new Entry[Y]
			Dim map_Renamed As ClassValueMap = type.classValueMap
			If map_Renamed Is Nothing Then Return EMPTY_CACHE
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
			Dim cache As Entry(Of ?)() = map_Renamed.cache
			Return cache
			' invariant:  returned value is safe to dereference and check for an Entry
		End Function

		''' <summary>
		''' Initial, one-element, empty cache used by all Class instances.  Must never be filled. </summary>
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
		Private Shared ReadOnly EMPTY_CACHE As Entry(Of ?)() = { Nothing }

		''' <summary>
		''' Slow tail of ClassValue.get to retry at nearby locations in the cache,
		''' or take a slow lock and check the hash table.
		''' Called only if the first probe was empty or a collision.
		''' This is a separate method, so compilers can process it independently.
		''' </summary>
		Private Function getFromBackup(Of T1)(ByVal cache As Entry(Of T1)(), ByVal type As Class) As T
			Dim e As Entry(Of T) = probeBackupLocations(cache, Me)
			If e IsNot Nothing Then Return e.value()
			Return getFromHashMap(type)
		End Function

		' Hack to suppress warnings on the (T) cast, which is a no-op.
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
		Friend Overridable Function castEntry(Of T1)(ByVal e As Entry(Of T1)) As Entry(Of T)
			Return CType(e, Entry(Of T))
		End Function

		''' <summary>
		''' Called when the fast path of get fails, and cache reprobe also fails.
		''' </summary>
		Private Function getFromHashMap(ByVal type As Class) As T
			' The fail-safe recovery is to fall back to the underlying classValueMap.
			Dim map_Renamed As ClassValueMap = getMap(type)
			Do
				Dim e As Entry(Of T) = map_Renamed.startEntry(Me)
				If Not e.promise Then Return e.value()
				Try
					' Try to make a real entry for the promised version.
					e = makeEntry(e.version(), computeValue(type))
				Finally
					' Whether computeValue throws or returns normally,
					' be sure to remove the empty entry.
					e = map_Renamed.finishEntry(Me, e)
				End Try
				If e IsNot Nothing Then Return e.value()
				' else try again, in case a racing thread called remove (so e == null)
			Loop
		End Function

		''' <summary>
		''' Check that e is non-null, matches this ClassValue, and is live. </summary>
		Friend Overridable Function match(Of T1)(ByVal e As Entry(Of T1)) As Boolean
			' racing e.version : null (blank) => unique Version token => null (GC-ed version)
			' non-racing this.version : v1 => v2 => ... (updates are read faithfully from volatile)
			Return (e IsNot Nothing AndAlso e.get() Is Me.version_Renamed)
			' invariant:  No false positives on version match.  Null is OK for false negative.
			' invariant:  If version matches, then e.value is readable (final set in Entry.<init>)
		End Function

		''' <summary>
		''' Internal hash code for accessing Class.classValueMap.cacheArray. </summary>
		Friend ReadOnly hashCodeForCache As Integer = nextHashCode.getAndAdd(HASH_INCREMENT) And HASH_MASK

		''' <summary>
		''' Value stream for hashCodeForCache.  See similar structure in ThreadLocal. </summary>
		Private Shared ReadOnly nextHashCode As New java.util.concurrent.atomic.AtomicInteger

		''' <summary>
		''' Good for power-of-two tables.  See similar structure in ThreadLocal. </summary>
		Private Const HASH_INCREMENT As Integer = &H61c88647

		''' <summary>
		''' Mask a hash code to be positive but not too large, to prevent wraparound. </summary>
		Friend Shared ReadOnly HASH_MASK As Integer = (-1 >>> 2)

		''' <summary>
		''' Private key for retrieval of this object from ClassValueMap.
		''' </summary>
		Friend Class Identity
		End Class
		''' <summary>
		''' This ClassValue's identity, expressed as an opaque object.
		''' The main object {@code ClassValue.this} is incorrect since
		''' subclasses may override {@code ClassValue.equals}, which
		''' could confuse keys in the ClassValueMap.
		''' </summary>
		Friend ReadOnly identity As New Identity

		''' <summary>
		''' Current version for retrieving this class value from the cache.
		''' Any number of computeValue calls can be cached in association with one version.
		''' But the version changes when a remove (on any type) is executed.
		''' A version change invalidates all cache entries for the affected ClassValue,
		''' by marking them as stale.  Stale cache entries do not force another call
		''' to computeValue, but they do require a synchronized visit to a backing map.
		''' <p>
		''' All user-visible state changes on the ClassValue take place under
		''' a lock inside the synchronized methods of ClassValueMap.
		''' Readers (of ClassValue.get) are notified of such state changes
		''' when this.version is bumped to a new token.
		''' This variable must be volatile so that an unsynchronized reader
		''' will receive the notification without delay.
		''' <p>
		''' If version were not volatile, one thread T1 could persistently hold onto
		''' a stale value this.value == V1, while while another thread T2 advances
		''' (under a lock) to this.value == V2.  This will typically be harmless,
		''' but if T1 and T2 interact causally via some other channel, such that
		''' T1's further actions are constrained (in the JMM) to happen after
		''' the V2 event, then T1's observation of V1 will be an error.
		''' <p>
		''' The practical effect of making this.version be volatile is that it cannot
		''' be hoisted out of a loop (by an optimizing JIT) or otherwise cached.
		''' Some machines may also require a barrier instruction to execute
		''' before this.version.
		''' </summary>
'JAVA TO VB CONVERTER TODO TASK: There is no VB equivalent to 'volatile':
		Private version_Renamed As New Version(Of T)(Me)
		Friend Overridable Function version() As Version(Of T)
			Return version_Renamed
		End Function
		Friend Overridable Sub bumpVersion()
			version_Renamed = New Version(Of )(Me)
		End Sub
		Friend Class Version(Of T)
			Private ReadOnly classValue_Renamed As ClassValue(Of T)
			Private ReadOnly promise_Renamed As New Entry(Of T)(Me)
			Friend Sub New(ByVal classValue_Renamed As ClassValue(Of T))
				Me.classValue_Renamed = classValue_Renamed
			End Sub
			Friend Overridable Function classValue() As ClassValue(Of T)
				Return classValue_Renamed
			End Function
			Friend Overridable Function promise() As Entry(Of T)
				Return promise_Renamed
			End Function
			Friend Overridable Property live As Boolean
				Get
					Return classValue_Renamed.version() Is Me
				End Get
			End Property
		End Class

		''' <summary>
		''' One binding of a value to a class via a ClassValue.
		'''  States are:<ul>
		'''  <li> promise if value == Entry.this
		'''  <li> else dead if version == null
		'''  <li> else stale if version != classValue.version
		'''  <li> else live </ul>
		'''  Promises are never put into the cache; they only live in the
		'''  backing map while a computeValue call is in flight.
		'''  Once an entry goes stale, it can be reset at any time
		'''  into the dead state.
		''' </summary>
		Friend Class Entry(Of T)
			Inherits WeakReference(Of Version(Of T))

			Friend ReadOnly value_Renamed As Object ' usually of type T, but sometimes (Entry)this
			Friend Sub New(ByVal version As Version(Of T), ByVal value As T)
				MyBase.New(version)
				Me.value_Renamed = value ' for a regular entry, value is of type T
			End Sub
			Private Sub assertNotPromise()
				assert((Not promise))
			End Sub
			''' <summary>
			''' For creating a promise. </summary>
			Friend Sub New(ByVal version As Version(Of T))
				MyBase.New(version)
				Me.value_Renamed = Me ' for a promise, value is not of type T, but Entry!
			End Sub
			''' <summary>
			''' Fetch the value.  This entry must not be a promise. </summary>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
			Friend Overridable Function value() As T ' if !isPromise, type is T
				assertNotPromise()
				Return CType(value_Renamed, T)
			End Function
			Friend Overridable Property promise As Boolean
				Get
					Return value_Renamed Is Me
				End Get
			End Property
			Friend Overridable Function version() As Version(Of T)
				Return get()
			End Function
			Friend Overridable Function classValueOrNull() As ClassValue(Of T)
				Dim v As Version(Of T) = version()
				Return If(v Is Nothing, Nothing, v.classValue())
			End Function
			Friend Overridable Property live As Boolean
				Get
					Dim v As Version(Of T) = version()
					If v Is Nothing Then Return False
					If v.live Then Return True
					clear()
					Return False
				End Get
			End Property
			Friend Overridable Function refreshVersion(ByVal v2 As Version(Of T)) As Entry(Of T)
				assertNotPromise()
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
				Dim e2 As New Entry(Of T)(v2, CType(value_Renamed, T)) ' if !isPromise, type is T
				clear()
				' value = null -- caller must drop
				Return e2
			End Function
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
			Friend Shared ReadOnly DEAD_ENTRY As New Entry(Of ?)(Nothing, Nothing)
		End Class

		''' <summary>
		''' Return the backing map associated with this type. </summary>
		Private Shared Function getMap(ByVal type As Class) As ClassValueMap
			' racing type.classValueMap : null (blank) => unique ClassValueMap
			' if a null is observed, a map is created (lazily, synchronously, uniquely)
			' all further access to that map is synchronized
			Dim map_Renamed As ClassValueMap = type.classValueMap
			If map_Renamed IsNot Nothing Then Return map_Renamed
			Return initializeMap(type)
		End Function

		Private Shared ReadOnly CRITICAL_SECTION As New Object
		Private Shared Function initializeMap(ByVal type As Class) As ClassValueMap
			Dim map_Renamed As ClassValueMap
			SyncLock CRITICAL_SECTION ' private object to avoid deadlocks
				' happens about once per type
				map_Renamed = type.classValueMap
				If map_Renamed Is Nothing Then
						map_Renamed = New ClassValueMap(type)
						type.classValueMap = map_Renamed
				End If
			End SyncLock
				Return map_Renamed
		End Function

		Friend Shared Function makeEntry(Of T)(ByVal explicitVersion As Version(Of T), ByVal value As T) As Entry(Of T)
			' Note that explicitVersion might be different from this.version.
			Return New Entry(Of )(explicitVersion, value)

			' As soon as the Entry is put into the cache, the value will be
			' reachable via a data race (as defined by the Java Memory Model).
			' This race is benign, assuming the value object itself can be
			' read safely by multiple threads.  This is up to the user.
			'
			' The entry and version fields themselves can be safely read via
			' a race because they are either final or have controlled states.
			' If the pointer from the entry to the version is still null,
			' or if the version goes immediately dead and is nulled out,
			' the reader will take the slow path and retry under a lock.
		End Function

		' The following class could also be top level and non-public:

		''' <summary>
		''' A backing map for all ClassValues, relative a single given type.
		'''  Gives a fully serialized "true state" for each pair (ClassValue cv, Class type).
		'''  Also manages an unserialized fast-path cache.
		''' </summary>
		Friend Class ClassValueMap
			Inherits java.util.WeakHashMap(Of ClassValue.Identity, Entry(Of JavaToDotNetGenericWildcard))

			Private ReadOnly type As Class
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
			Private cacheArray As Entry(Of ?)()
			Private cacheLoad, cacheLoadLimit As Integer

			''' <summary>
			''' Number of entries initially allocated to each type when first used with any ClassValue.
			'''  It would be pointless to make this much smaller than the Class and ClassValueMap objects themselves.
			'''  Must be a power of 2.
			''' </summary>
			Private Const INITIAL_ENTRIES As Integer = 32

			''' <summary>
			''' Build a backing map for ClassValues, relative the given type.
			'''  Also, create an empty cache array and install it on the class.
			''' </summary>
			Friend Sub New(ByVal type As Class)
				Me.type = type
				sizeCache(INITIAL_ENTRIES)
			End Sub

'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
			Friend Overridable Property cache As Entry(Of ?)()
				Get
					Return cacheArray
				End Get
			End Property

			''' <summary>
			''' Initiate a query.  Store a promise (placeholder) if there is no value yet. </summary>
			 <MethodImpl(MethodImplOptions.Synchronized)> _
			 Friend Overridable Function startEntry(Of T)(ByVal classValue_Renamed As ClassValue(Of T)) As Entry(Of T)
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
				Dim e As Entry(Of T) = CType([get](classValue_Renamed.identity), Entry(Of T)) ' one map has entries for all value types <T>
				Dim v As Version(Of T) = classValue_Renamed.version()
				If e Is Nothing Then
					e = v.promise()
					' The presence of a promise means that a value is pending for v.
					' Eventually, finishEntry will overwrite the promise.
					put(classValue_Renamed.identity, e)
					' Note that the promise is never entered into the cache!
					Return e
				ElseIf e.promise Then
					' Somebody else has asked the same question.
					' Let the races begin!
					If e.version() IsNot v Then
						e = v.promise()
						put(classValue_Renamed.identity, e)
					End If
					Return e
				Else
					' there is already a completed entry here; report it
					If e.version() IsNot v Then
						' There is a stale but valid entry here; make it fresh again.
						' Once an entry is in the hash table, we don't care what its version is.
						e = e.refreshVersion(v)
						put(classValue_Renamed.identity, e)
					End If
					' Add to the cache, to enable the fast path, next time.
					checkCacheLoad()
					addToCache(classValue_Renamed, e)
					Return e
				End If
			 End Function

			''' <summary>
			''' Finish a query.  Overwrite a matching placeholder.  Drop stale incoming values. </summary>
			 <MethodImpl(MethodImplOptions.Synchronized)> _
			 Friend Overridable Function finishEntry(Of T)(ByVal classValue_Renamed As ClassValue(Of T), ByVal e As Entry(Of T)) As Entry(Of T)
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
				Dim e0 As Entry(Of T) = CType([get](classValue_Renamed.identity), Entry(Of T)) ' one map has entries for all value types <T>
				If e Is e0 Then
					' We can get here during exception processing, unwinding from computeValue.
					assert(e.promise)
					remove(classValue_Renamed.identity)
					Return Nothing
				ElseIf e0 IsNot Nothing AndAlso e0.promise AndAlso e0.version() Is e.version() Then
					' If e0 matches the intended entry, there has not been a remove call
					' between the previous startEntry and now.  So now overwrite e0.
					Dim v As Version(Of T) = classValue_Renamed.version()
					If e.version() IsNot v Then e = e.refreshVersion(v)
					put(classValue_Renamed.identity, e)
					' Add to the cache, to enable the fast path, next time.
					checkCacheLoad()
					addToCache(classValue_Renamed, e)
					Return e
				Else
					' Some sort of mismatch; caller must try again.
					Return Nothing
				End If
			 End Function

			''' <summary>
			''' Remove an entry. </summary>
			<MethodImpl(MethodImplOptions.Synchronized)> _
			Friend Overridable Sub removeEntry(Of T1)(ByVal classValue_Renamed As ClassValue(Of T1))
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
				Dim e As Entry(Of ?) = remove(classValue_Renamed.identity)
				If e Is Nothing Then
					' Uninitialized, and no pending calls to computeValue.  No change.
				ElseIf e.promise Then
					' State is uninitialized, with a pending call to finishEntry.
					' Since remove is a no-op in such a state, keep the promise
					' by putting it back into the map.
					put(classValue_Renamed.identity, e)
				Else
					' In an initialized state.  Bump forward, and de-initialize.
					classValue_Renamed.bumpVersion()
					' Make all cache elements for this guy go stale.
					removeStaleEntries(classValue_Renamed)
				End If
			End Sub

			''' <summary>
			''' Change the value for an entry. </summary>
			 <MethodImpl(MethodImplOptions.Synchronized)> _
			 Friend Overridable Sub changeEntry(Of T)(ByVal classValue_Renamed As ClassValue(Of T), ByVal value As T)
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
				Dim e0 As Entry(Of T) = CType([get](classValue_Renamed.identity), Entry(Of T)) ' one map has entries for all value types <T>
				Dim version As Version(Of T) = classValue_Renamed.version()
				If e0 IsNot Nothing Then
					If e0.version() Is version AndAlso e0.value() Is value Then Return
					classValue_Renamed.bumpVersion()
					removeStaleEntries(classValue_Renamed)
				End If
				Dim e As Entry(Of T) = makeEntry(version, value)
				put(classValue_Renamed.identity, e)
				' Add to the cache, to enable the fast path, next time.
				checkCacheLoad()
				addToCache(classValue_Renamed, e)
			 End Sub

			'/ --------
			'/ Cache management.
			'/ --------

			' Statics do not need synchronization.

			''' <summary>
			''' Load the cache entry at the given (hashed) location. </summary>
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
			Friend Shared Function loadFromCache(Of T1)(ByVal cache As Entry(Of T1)(), ByVal i As Integer) As Entry(Of ?)
				' non-racing cache.length : constant
				' racing cache[i & (mask)] : null <=> Entry
				Return cache(i And (cache.Length-1))
				' invariant:  returned value is null or well-constructed (ready to match)
			End Function

			''' <summary>
			''' Look in the cache, at the home location for the given ClassValue. </summary>
			Friend Shared Function probeHomeLocation(Of T, T1)(ByVal cache As Entry(Of T1)(), ByVal classValue_Renamed As ClassValue(Of T)) As Entry(Of T)
				Return classValue_Renamed.castEntry(loadFromCache(cache, classValue_Renamed.hashCodeForCache))
			End Function

			''' <summary>
			''' Given that first probe was a collision, retry at nearby locations. </summary>
			Friend Shared Function probeBackupLocations(Of T, T1)(ByVal cache As Entry(Of T1)(), ByVal classValue_Renamed As ClassValue(Of T)) As Entry(Of T)
				If PROBE_LIMIT <= 0 Then Return Nothing
				' Probe the cache carefully, in a range of slots.
				Dim mask As Integer = (cache.Length-1)
				Dim home As Integer = (classValue_Renamed.hashCodeForCache And mask)
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
				Dim e2 As Entry(Of ?) = cache(home) ' victim, if we find the real guy
				If e2 Is Nothing Then Return Nothing ' if nobody is at home, no need to search nearby
				' assume !classValue.match(e2), but do not assert, because of races
				Dim pos2 As Integer = -1
				For i As Integer = home + 1 To home + PROBE_LIMIT - 1
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
					Dim e As Entry(Of ?) = cache(i And mask)
					If e Is Nothing Then Exit For ' only search within non-null runs
					If classValue_Renamed.match(e) Then
						' relocate colliding entry e2 (from cache[home]) to first empty slot
						cache(home) = e
						If pos2 >= 0 Then
							cache(i And mask) = Entry.DEAD_ENTRY
						Else
							pos2 = i
						End If
						cache(pos2 And mask) = (If(entryDislocation(cache, pos2, e2) < PROBE_LIMIT, e2, Entry.DEAD_ENTRY)) ' put e2 here if it fits
						Return classValue_Renamed.castEntry(e)
					End If
					' Remember first empty slot, if any:
					If (Not e.live) AndAlso pos2 < 0 Then pos2 = i
				Next i
				Return Nothing
			End Function

			''' <summary>
			''' How far out of place is e? </summary>
			Private Shared Function entryDislocation(Of T1, T2)(ByVal cache As Entry(Of T1)(), ByVal pos As Integer, ByVal e As Entry(Of T2)) As Integer
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
				Dim cv As ClassValue(Of ?) = e.classValueOrNull()
				If cv Is Nothing Then ' entry is not live! Return 0
				Dim mask As Integer = (cache.Length-1)
				Return (pos - cv.hashCodeForCache) And mask
			End Function

			'/ --------
			'/ Below this line all functions are private, and assume synchronized access.
			'/ --------

			Private Sub sizeCache(ByVal length As Integer)
				assert((length And (length-1)) = 0) ' must be power of 2
				cacheLoad = 0
				cacheLoadLimit = CInt(Fix(CDbl(length) * CACHE_LOAD_LIMIT \ 100))
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
				cacheArray = New Entry(Of ?)(length - 1){}
			End Sub

			''' <summary>
			''' Make sure the cache load stays below its limit, if possible. </summary>
			Private Sub checkCacheLoad()
				If cacheLoad >= cacheLoadLimit Then reduceCacheLoad()
			End Sub
			Private Sub reduceCacheLoad()
				removeStaleEntries()
				If cacheLoad < cacheLoadLimit Then Return ' win
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
				Dim oldCache As Entry(Of ?)() = cache
				If oldCache.Length > HASH_MASK Then Return ' lose
				sizeCache(oldCache.Length * 2)
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
				For Each e As Entry(Of ?) In oldCache
					If e IsNot Nothing AndAlso e.live Then addToCache(e)
				Next e
			End Sub

			''' <summary>
			''' Remove stale entries in the given range.
			'''  Should be executed under a Map lock.
			''' </summary>
			Private Sub removeStaleEntries(Of T1)(ByVal cache As Entry(Of T1)(), ByVal begin As Integer, ByVal count As Integer)
				If PROBE_LIMIT <= 0 Then Return
				Dim mask As Integer = (cache.Length-1)
				Dim removed As Integer = 0
				For i As Integer = begin To begin + count - 1
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
					Dim e As Entry(Of ?) = cache(i And mask)
					If e Is Nothing OrElse e.live Then Continue For ' skip null and live entries
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
					Dim replacement As Entry(Of ?) = Nothing
					If PROBE_LIMIT > 1 Then replacement = findReplacement(cache, i)
					cache(i And mask) = replacement
					If replacement Is Nothing Then removed += 1
				Next i
				cacheLoad = Math.Max(0, cacheLoad - removed)
			End Sub

			''' <summary>
			''' Clearing a cache slot risks disconnecting following entries
			'''  from the head of a non-null run, which would allow them
			'''  to be found via reprobes.  Find an entry after cache[begin]
			'''  to plug into the hole, or return null if none is needed.
			''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
			Private Function findReplacement(Of T1)(ByVal cache As Entry(Of T1)(), ByVal home1 As Integer) As Entry(Of ?)
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
				Dim replacement As Entry(Of ?) = Nothing
				Dim haveReplacement As Integer = -1, replacementPos As Integer = 0
				Dim mask As Integer = (cache.Length-1)
				For i2 As Integer = home1 + 1 To home1 + PROBE_LIMIT - 1
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
					Dim e2 As Entry(Of ?) = cache(i2 And mask)
					If e2 Is Nothing Then ' End of non-null run. Exit For
					If Not e2.live Then ' Doomed anyway. Continue For
					Dim dis2 As Integer = entryDislocation(cache, i2, e2)
					If dis2 = 0 Then ' e2 already optimally placed Continue For
					Dim home2 As Integer = i2 - dis2
					If home2 <= home1 Then
						' e2 can replace entry at cache[home1]
						If home2 = home1 Then
							' Put e2 exactly where he belongs.
							haveReplacement = 1
							replacementPos = i2
							replacement = e2
						ElseIf haveReplacement <= 0 Then
							haveReplacement = 0
							replacementPos = i2
							replacement = e2
						End If
						' And keep going, so we can favor larger dislocations.
					End If
				Next i2
				If haveReplacement >= 0 Then
					If cache((replacementPos+1) And mask) IsNot Nothing Then
						' Be conservative, to avoid breaking up a non-null run.
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
						cache(replacementPos And mask) = CType(Entry.DEAD_ENTRY, Entry(Of ?))
					Else
						cache(replacementPos And mask) = Nothing
						cacheLoad -= 1
					End If
				End If
				Return replacement
			End Function

			''' <summary>
			''' Remove stale entries in the range near classValue. </summary>
			Private Sub removeStaleEntries(Of T1)(ByVal classValue_Renamed As ClassValue(Of T1))
				removeStaleEntries(cache, classValue_Renamed.hashCodeForCache, PROBE_LIMIT)
			End Sub

			''' <summary>
			''' Remove all stale entries, everywhere. </summary>
			Private Sub removeStaleEntries()
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
				Dim cache_Renamed As Entry(Of ?)() = cache
				removeStaleEntries(cache_Renamed, 0, cache_Renamed.Length + PROBE_LIMIT - 1)
			End Sub

			''' <summary>
			''' Add the given entry to the cache, in its home location, unless it is out of date. </summary>
			Private Sub addToCache(Of T)(ByVal e As Entry(Of T))
				Dim classValue_Renamed As ClassValue(Of T) = e.classValueOrNull()
				If classValue_Renamed IsNot Nothing Then addToCache(classValue_Renamed, e)
			End Sub

			''' <summary>
			''' Add the given entry to the cache, in its home location. </summary>
			Private Sub addToCache(Of T)(ByVal classValue_Renamed As ClassValue(Of T), ByVal e As Entry(Of T))
				If PROBE_LIMIT <= 0 Then ' do not fill cache Return
				' Add e to the cache.
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
				Dim cache_Renamed As Entry(Of ?)() = cache
				Dim mask As Integer = (cache_Renamed.Length-1)
				Dim home As Integer = classValue_Renamed.hashCodeForCache And mask
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
				Dim e2 As Entry(Of ?) = placeInCache(cache_Renamed, home, e, False)
				If e2 Is Nothing Then ' done Return
				If PROBE_LIMIT > 1 Then
					' try to move e2 somewhere else in his probe range
					Dim dis2 As Integer = entryDislocation(cache_Renamed, home, e2)
					Dim home2 As Integer = home - dis2
					For i2 As Integer = home2 To home2 + PROBE_LIMIT - 1
						If placeInCache(cache_Renamed, i2 And mask, e2, True) Is Nothing Then Return
					Next i2
				End If
				' Note:  At this point, e2 is just dropped from the cache.
			End Sub

			''' <summary>
			''' Store the given entry.  Update cacheLoad, and return any live victim.
			'''  'Gently' means return self rather than dislocating a live victim.
			''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
			Private Function placeInCache(Of T1, T2)(ByVal cache As Entry(Of T1)(), ByVal pos As Integer, ByVal e As Entry(Of T2), ByVal gently As Boolean) As Entry(Of ?)
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
				Dim e2 As Entry(Of ?) = overwrittenEntry(cache(pos))
				If gently AndAlso e2 IsNot Nothing Then
					' do not overwrite a live entry
					Return e
				Else
					cache(pos) = e
					Return e2
				End If
			End Function

			''' <summary>
			''' Note an entry that is about to be overwritten.
			'''  If it is not live, quietly replace it by null.
			'''  If it is an actual null, increment cacheLoad,
			'''  because the caller is going to store something
			'''  in its place.
			''' </summary>
			Private Function overwrittenEntry(Of T)(ByVal e2 As Entry(Of T)) As Entry(Of T)
				If e2 Is Nothing Then
					cacheLoad += 1
				ElseIf e2.live Then
					Return e2
				End If
				Return Nothing
			End Function

			''' <summary>
			''' Percent loading of cache before resize. </summary>
			Private Const CACHE_LOAD_LIMIT As Integer = 67 ' 0..100
			''' <summary>
			''' Maximum number of probes to attempt. </summary>
			Private Const PROBE_LIMIT As Integer = 6 ' 1..
			' N.B.  Set PROBE_LIMIT=0 to disable all fast paths.
		End Class
	End Class

End Namespace