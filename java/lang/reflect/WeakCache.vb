Imports System.Runtime.CompilerServices
Imports System.Diagnostics
Imports System.Collections.Concurrent

'
' * Copyright (c) 2013, Oracle and/or its affiliates. All rights reserved.
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
Namespace java.lang.reflect


	''' <summary>
	''' Cache mapping pairs of {@code (key, sub-key) -> value}. Keys and values are
	''' weakly but sub-keys are strongly referenced.  Keys are passed directly to
	''' <seealso cref="#get"/> method which also takes a {@code parameter}. Sub-keys are
	''' calculated from keys and parameters using the {@code subKeyFactory} function
	''' passed to the constructor. Values are calculated from keys and parameters
	''' using the {@code valueFactory} function passed to the constructor.
	''' Keys can be {@code null} and are compared by identity while sub-keys returned by
	''' {@code subKeyFactory} or values returned by {@code valueFactory}
	''' can not be null. Sub-keys are compared using their <seealso cref="#equals"/> method.
	''' Entries are expunged from cache lazily on each invocation to <seealso cref="#get"/>,
	''' <seealso cref="#containsValue"/> or <seealso cref="#size"/> methods when the WeakReferences to
	''' keys are cleared. Cleared WeakReferences to individual values don't cause
	''' expunging, but such entries are logically treated as non-existent and
	''' trigger re-evaluation of {@code valueFactory} on request for their
	''' key/subKey.
	''' 
	''' @author Peter Levart </summary>
	''' @param <K> type of keys </param>
	''' @param <P> type of parameters </param>
	''' @param <V> type of values </param>
	Friend NotInheritable Class WeakCache(Of K, P, V)

		Private ReadOnly refQueue As New ReferenceQueue(Of K)
		' the key type is Object for supporting null key
		Private ReadOnly map As java.util.concurrent.ConcurrentMap(Of Object, java.util.concurrent.ConcurrentMap(Of Object, java.util.function.Supplier(Of V))) = New ConcurrentDictionary(Of Object, java.util.concurrent.ConcurrentMap(Of Object, java.util.function.Supplier(Of V)))
		Private ReadOnly reverseMap As java.util.concurrent.ConcurrentMap(Of java.util.function.Supplier(Of V), Boolean?) = New ConcurrentDictionary(Of java.util.function.Supplier(Of V), Boolean?)
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
		Private ReadOnly subKeyFactory As java.util.function.BiFunction(Of K, P, ?)
		Private ReadOnly valueFactory As java.util.function.BiFunction(Of K, P, V)

		''' <summary>
		''' Construct an instance of {@code WeakCache}
		''' </summary>
		''' <param name="subKeyFactory"> a function mapping a pair of
		'''                      {@code (key, parameter) -> sub-key} </param>
		''' <param name="valueFactory">  a function mapping a pair of
		'''                      {@code (key, parameter) -> value} </param>
		''' <exception cref="NullPointerException"> if {@code subKeyFactory} or
		'''                              {@code valueFactory} is null. </exception>
		Public Sub New(Of T1)(ByVal subKeyFactory As java.util.function.BiFunction(Of T1), ByVal valueFactory As java.util.function.BiFunction(Of K, P, V))
			Me.subKeyFactory = java.util.Objects.requireNonNull(subKeyFactory)
			Me.valueFactory = java.util.Objects.requireNonNull(valueFactory)
		End Sub

		''' <summary>
		''' Look-up the value through the cache. This always evaluates the
		''' {@code subKeyFactory} function and optionally evaluates
		''' {@code valueFactory} function if there is no entry in the cache for given
		''' pair of (key, subKey) or the entry has already been cleared.
		''' </summary>
		''' <param name="key">       possibly null key </param>
		''' <param name="parameter"> parameter used together with key to create sub-key and
		'''                  value (should not be null) </param>
		''' <returns> the cached value (never null) </returns>
		''' <exception cref="NullPointerException"> if {@code parameter} passed in or
		'''                              {@code sub-key} calculated by
		'''                              {@code subKeyFactory} or {@code value}
		'''                              calculated by {@code valueFactory} is null. </exception>
		Public Function [get](ByVal key As K, ByVal parameter As P) As V
			java.util.Objects.requireNonNull(parameter)

			expungeStaleEntries()

			Dim cacheKey_Renamed As Object = CacheKey.valueOf(key, refQueue)

			' lazily install the 2nd level valuesMap for the particular cacheKey
			Dim valuesMap As java.util.concurrent.ConcurrentMap(Of Object, java.util.function.Supplier(Of V)) = map.get(cacheKey_Renamed)
			If valuesMap Is Nothing Then
'JAVA TO VB CONVERTER TODO TASK: Assignments within expressions are not supported in VB
				Dim oldValuesMap As java.util.concurrent.ConcurrentMap(Of Object, java.util.function.Supplier(Of V)) = map.putIfAbsent(cacheKey_Renamed, valuesMap = New ConcurrentDictionary(Of Object, java.util.function.Supplier(Of V)))
				If oldValuesMap IsNot Nothing Then valuesMap = oldValuesMap
			End If

			' create subKey and retrieve the possible Supplier<V> stored by that
			' subKey from valuesMap
			Dim subKey As Object = java.util.Objects.requireNonNull(subKeyFactory.apply(key, parameter))
			Dim supplier As java.util.function.Supplier(Of V) = valuesMap.get(subKey)
			Dim factory As Factory = Nothing

			Do
				If supplier IsNot Nothing Then
					' supplier might be a Factory or a CacheValue<V> instance
					Dim value As V = supplier.get()
					If value IsNot Nothing Then Return value
				End If
				' else no supplier in cache
				' or a supplier that returned null (could be a cleared CacheValue
				' or a Factory that wasn't successful in installing the CacheValue)

				' lazily construct a Factory
				If factory Is Nothing Then factory = New Factory(Me, key, parameter, subKey, valuesMap)

				If supplier Is Nothing Then
					supplier = valuesMap.putIfAbsent(subKey, factory)
					If supplier Is Nothing Then supplier = factory
					' else retry with winning supplier
				Else
					If valuesMap.replace(subKey, supplier, factory) Then
						' successfully replaced
						' cleared CacheEntry / unsuccessful Factory
						' with our Factory
						supplier = factory
					Else
						' retry with current supplier
						supplier = valuesMap.get(subKey)
					End If
				End If
			Loop
		End Function

		''' <summary>
		''' Checks whether the specified non-null value is already present in this
		''' {@code WeakCache}. The check is made using identity comparison regardless
		''' of whether value's class overrides <seealso cref="Object#equals"/> or not.
		''' </summary>
		''' <param name="value"> the non-null value to check </param>
		''' <returns> true if given {@code value} is already cached </returns>
		''' <exception cref="NullPointerException"> if value is null </exception>
		Public Function containsValue(ByVal value As V) As Boolean
			java.util.Objects.requireNonNull(value)

			expungeStaleEntries()
			Return reverseMap.containsKey(New LookupValue(Of )(value))
		End Function

		''' <summary>
		''' Returns the current number of cached entries that
		''' can decrease over time when keys/values are GC-ed.
		''' </summary>
		Public Function size() As Integer
			expungeStaleEntries()
			Return reverseMap.size()
		End Function

		Private Sub expungeStaleEntries()
			Dim cacheKey_Renamed As CacheKey(Of K)
			cacheKey_Renamed = CType(refQueue.poll(), CacheKey(Of K))
			Do While cacheKey_Renamed IsNot Nothing
				cacheKey_Renamed.expungeFrom(map, reverseMap)
				cacheKey_Renamed = CType(refQueue.poll(), CacheKey(Of K))
			Loop
		End Sub

		''' <summary>
		''' A factory <seealso cref="Supplier"/> that implements the lazy synchronized
		''' construction of the value and installment of it into the cache.
		''' </summary>
		Private NotInheritable Class Factory
			Implements java.util.function.Supplier(Of V)

			Private ReadOnly outerInstance As WeakCache


			Private ReadOnly key As K
			Private ReadOnly parameter As P
			Private ReadOnly subKey As Object
			Private ReadOnly valuesMap As java.util.concurrent.ConcurrentMap(Of Object, java.util.function.Supplier(Of V))

			Friend Sub New(ByVal outerInstance As WeakCache, ByVal key As K, ByVal parameter As P, ByVal subKey As Object, ByVal valuesMap As java.util.concurrent.ConcurrentMap(Of Object, java.util.function.Supplier(Of V)))
					Me.outerInstance = outerInstance
				Me.key = key
				Me.parameter = parameter
				Me.subKey = subKey
				Me.valuesMap = valuesMap
			End Sub

			<MethodImpl(MethodImplOptions.Synchronized)> _
			Public Overrides Function [get]() As V ' serialize access
				' re-check
				Dim supplier As java.util.function.Supplier(Of V) = valuesMap.get(subKey)
				If supplier IsNot Me Then Return Nothing
				' else still us (supplier == this)

				' create new value
				Dim value As V = Nothing
				Try
					value = java.util.Objects.requireNonNull(outerInstance.valueFactory.apply(key, parameter))
				Finally
					If value Is Nothing Then ' remove us on failure valuesMap.remove(subKey, Me)
				End Try
				' the only path to reach here is with non-null value
				Debug.Assert(value IsNot Nothing)

				' wrap value with CacheValue (WeakReference)
				Dim cacheValue As New CacheValue(Of V)(value)

				' try replacing us with CacheValue (this should always succeed)
				If valuesMap.replace(subKey, Me, cacheValue) Then
					' put also in reverseMap
					outerInstance.reverseMap.put(cacheValue,  java.lang.[Boolean].TRUE)
				Else
					Throw New AssertionError("Should not reach here")
				End If

				' successfully replaced us with new CacheValue -> return the value
				' wrapped by it
				Return value
			End Function
		End Class

		''' <summary>
		''' Common type of value suppliers that are holding a referent.
		''' The <seealso cref="#equals"/> and <seealso cref="#hashCode"/> of implementations is defined
		''' to compare the referent by identity.
		''' </summary>
		Private Interface Value(Of V)
			Inherits java.util.function.Supplier(Of V)

		End Interface

		''' <summary>
		''' An optimized <seealso cref="Value"/> used to look-up the value in
		''' <seealso cref="WeakCache#containsValue"/> method so that we are not
		''' constructing the whole <seealso cref="CacheValue"/> just to look-up the referent.
		''' </summary>
		Private NotInheritable Class LookupValue(Of V)
			Implements Value(Of V)

			Private ReadOnly value As V

			Friend Sub New(ByVal value As V)
				Me.value = value
			End Sub

			Public Overrides Function [get]() As V
				Return value
			End Function

			Public Overrides Function GetHashCode() As Integer
				Return System.identityHashCode(value) ' compare by identity
			End Function

			Public Overrides Function Equals(ByVal obj As Object) As Boolean
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
				Return obj Is Me OrElse TypeOf obj Is Value AndAlso Me.value Is CType(obj, Value(Of ?)).get() ' compare by identity
			End Function
		End Class

		''' <summary>
		''' A <seealso cref="Value"/> that weakly references the referent.
		''' </summary>
		Private NotInheritable Class CacheValue(Of V)
			Inherits WeakReference(Of V)
			Implements Value(Of V)

			Private ReadOnly hash As Integer

			Friend Sub New(ByVal value As V)
				MyBase.New(value)
				Me.hash = System.identityHashCode(value) ' compare by identity
			End Sub

			Public Overrides Function GetHashCode() As Integer
				Return hash
			End Function

			Public Overrides Function Equals(ByVal obj As Object) As Boolean
				Dim value As V
'JAVA TO VB CONVERTER TODO TASK: Assignments within expressions are not supported in VB
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
				Return obj Is Me OrElse TypeOf obj Is Value AndAlso (value = get()) IsNot Nothing AndAlso value Is CType(obj, Value(Of ?)).get() ' compare by identity
					   ' cleared CacheValue is only equal to itself
			End Function
		End Class

		''' <summary>
		''' CacheKey containing a weakly referenced {@code key}. It registers
		''' itself with the {@code refQueue} so that it can be used to expunge
		''' the entry when the <seealso cref="WeakReference"/> is cleared.
		''' </summary>
		Private NotInheritable Class CacheKey(Of K)
			Inherits WeakReference(Of K)

			' a replacement for null keys
			Private Shared ReadOnly NULL_KEY As New Object

			Friend Shared Function valueOf(Of K)(ByVal key As K, ByVal refQueue As ReferenceQueue(Of K)) As Object
				Return If(key Is Nothing, NULL_KEY, New CacheKey(Of )(key, refQueue))
					   ' null key means we can't weakly reference it,
					   ' so we use a NULL_KEY singleton as cache key
					   ' non-null key requires wrapping with a WeakReference
			End Function

			Private ReadOnly hash As Integer

			Private Sub New(ByVal key As K, ByVal refQueue As ReferenceQueue(Of K))
				MyBase.New(key, refQueue)
				Me.hash = System.identityHashCode(key) ' compare by identity
			End Sub

			Public Overrides Function GetHashCode() As Integer
				Return hash
			End Function

			Public Overrides Function Equals(ByVal obj As Object) As Boolean
				Dim key As K
'JAVA TO VB CONVERTER TODO TASK: Assignments within expressions are not supported in VB
				Return obj Is Me OrElse obj IsNot Nothing AndAlso obj.GetType() Is Me.GetType() AndAlso (key = Me.get()) IsNot Nothing AndAlso key Is CType(obj, CacheKey(Of K)).get()
					   ' cleared CacheKey is only equal to itself
					   ' compare key by identity
			End Function

'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
			Friend Sub expungeFrom(Of T1 As java.util.concurrent.ConcurrentMap(Of ?, ?, T2)(ByVal map As java.util.concurrent.ConcurrentMap(Of T1), ByVal reverseMap As java.util.concurrent.ConcurrentMap(Of T2))
				' removing just by key is always safe here because after a CacheKey
				' is cleared and enqueue-ed it is only equal to itself
				' (see equals method)...
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
				Dim valuesMap As java.util.concurrent.ConcurrentMap(Of ?, ?) = map.remove(Me)
				' remove also from reverseMap if needed
				If valuesMap IsNot Nothing Then
					For Each cacheValue As Object In valuesMap.values()
						reverseMap.remove(cacheValue)
					Next cacheValue
				End If
			End Sub
		End Class
	End Class

End Namespace