Imports System.Collections

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
	''' A <seealso cref="java.util.Map"/> providing thread safety and atomicity
	''' guarantees.
	''' 
	''' <p>Memory consistency effects: As with other concurrent
	''' collections, actions in a thread prior to placing an object into a
	''' {@code ConcurrentMap} as a key or value
	''' <a href="package-summary.html#MemoryVisibility"><i>happen-before</i></a>
	''' actions subsequent to the access or removal of that object from
	''' the {@code ConcurrentMap} in another thread.
	''' 
	''' <p>This interface is a member of the
	''' <a href="{@docRoot}/../technotes/guides/collections/index.html">
	''' Java Collections Framework</a>.
	''' 
	''' @since 1.5
	''' @author Doug Lea </summary>
	''' @param <K> the type of keys maintained by this map </param>
	''' @param <V> the type of mapped values </param>
	Public Interface ConcurrentMap(Of K, V)
		Inherits IDictionary(Of K, V)

		''' <summary>
		''' {@inheritDoc}
		''' 
		''' @implNote This implementation assumes that the ConcurrentMap cannot
		''' contain null values and {@code get()} returning null unambiguously means
		''' the key is absent. Implementations which support null values
		''' <strong>must</strong> override this default implementation.
		''' </summary>
		''' <exception cref="ClassCastException"> {@inheritDoc} </exception>
		''' <exception cref="NullPointerException"> {@inheritDoc}
		''' @since 1.8 </exception>
		default Overrides Function getOrDefault(ByVal key As Object, ByVal defaultValue As V) As V
'JAVA TO VB CONVERTER TODO TASK: Interfaces cannot contain fields in .NET:
'			V v;
			Sub [New]((v = get(key)) != ByVal [Nothing] As )

	   ''' <summary>
	   ''' {@inheritDoc}
	   '''  
	   ''' @implSpec The default implementation is equivalent to, for this
	   ''' {@code map}:
	   ''' <pre> {@code
	   ''' for ((Map.Entry<K, V> entry : map.entrySet())
	   '''     action.accept(entry.getKey(), entry.getValue());
	   ''' }</pre>
	   '''  
	   ''' @implNote The default implementation assumes that
	   ''' {@code IllegalStateException} thrown by {@code getKey()} or
	   ''' {@code getValue()} indicates that the entry has been removed and cannot
	   ''' be processed. Operation continues for subsequent entries.
	   ''' </summary>
	   ''' <exception cref="NullPointerException"> {@inheritDoc}
	   ''' @since 1.8 </exception>
'JAVA TO VB CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
		default Overrides Sub forEach(Of T1)(ByVal action As java.util.function.BiConsumer(Of T1))
'JAVA TO VB CONVERTER TODO TASK: The following line could not be converted:
			java.util.Objects.requireNonNull(action);
			Sub [New](DictionaryEntry entry : entrySet() ByVal  As (Of K, V))
'JAVA TO VB CONVERTER TODO TASK: Interfaces cannot contain fields in .NET:
'				K k;
'JAVA TO VB CONVERTER TODO TASK: Interfaces cannot contain fields in .NET:
'				V v;
				Try
					ReadOnly Property k = entry.getKey() As
					ReadOnly Property v = entry.getValue() As
				Sub [New](ByVal ise As IllegalStateException)
					' this usually means the entry is no longer in the map.
					continue
'JAVA TO VB CONVERTER TODO TASK: The following line could not be converted:
				action.accept(k, v);

		''' <summary>
		''' If the specified key is not already associated
		''' with a value, associate it with the given value.
		''' This is equivalent to
		'''  <pre> {@code
		''' if (!map.containsKey(key))
		'''   return map.put(key, value);
		''' else
		'''   return map.get(key);
		''' }</pre>
		''' 
		''' except that the action is performed atomically.
		''' 
		''' @implNote This implementation intentionally re-abstracts the
		''' inappropriate default provided in {@code Map}.
		''' </summary>
		''' <param name="key"> key with which the specified value is to be associated </param>
		''' <param name="value"> value to be associated with the specified key </param>
		''' <returns> the previous value associated with the specified key, or
		'''         {@code null} if there was no mapping for the key.
		'''         (A {@code null} return can also indicate that the map
		'''         previously associated {@code null} with the key,
		'''         if the implementation supports null values.) </returns>
		''' <exception cref="UnsupportedOperationException"> if the {@code put} operation
		'''         is not supported by this map </exception>
		''' <exception cref="ClassCastException"> if the class of the specified key or value
		'''         prevents it from being stored in this map </exception>
		''' <exception cref="NullPointerException"> if the specified key or value is null,
		'''         and this map does not permit null keys or values </exception>
		''' <exception cref="IllegalArgumentException"> if some property of the specified key
		'''         or value prevents it from being stored in this map </exception>
		 Function putIfAbsent(ByVal key As K, ByVal value As V) As V

		''' <summary>
		''' Removes the entry for a key only if currently mapped to a given value.
		''' This is equivalent to
		'''  <pre> {@code
		''' if (map.containsKey(key) && Objects.equals(map.get(key), value)) {
		'''   map.remove(key);
		'''   return true;
		''' } else
		'''   return false;
		''' }</pre>
		''' 
		''' except that the action is performed atomically.
		''' 
		''' @implNote This implementation intentionally re-abstracts the
		''' inappropriate default provided in {@code Map}.
		''' </summary>
		''' <param name="key"> key with which the specified value is associated </param>
		''' <param name="value"> value expected to be associated with the specified key </param>
		''' <returns> {@code true} if the value was removed </returns>
		''' <exception cref="UnsupportedOperationException"> if the {@code remove} operation
		'''         is not supported by this map </exception>
		''' <exception cref="ClassCastException"> if the key or value is of an inappropriate
		'''         type for this map
		'''         (<a href="../Collection.html#optional-restrictions">optional</a>) </exception>
		''' <exception cref="NullPointerException"> if the specified key or value is null,
		'''         and this map does not permit null keys or values
		'''         (<a href="../Collection.html#optional-restrictions">optional</a>) </exception>
		Function remove(ByVal key As Object, ByVal value As Object) As Boolean

		''' <summary>
		''' Replaces the entry for a key only if currently mapped to a given value.
		''' This is equivalent to
		'''  <pre> {@code
		''' if (map.containsKey(key) && Objects.equals(map.get(key), oldValue)) {
		'''   map.put(key, newValue);
		'''   return true;
		''' } else
		'''   return false;
		''' }</pre>
		''' 
		''' except that the action is performed atomically.
		''' 
		''' @implNote This implementation intentionally re-abstracts the
		''' inappropriate default provided in {@code Map}.
		''' </summary>
		''' <param name="key"> key with which the specified value is associated </param>
		''' <param name="oldValue"> value expected to be associated with the specified key </param>
		''' <param name="newValue"> value to be associated with the specified key </param>
		''' <returns> {@code true} if the value was replaced </returns>
		''' <exception cref="UnsupportedOperationException"> if the {@code put} operation
		'''         is not supported by this map </exception>
		''' <exception cref="ClassCastException"> if the class of a specified key or value
		'''         prevents it from being stored in this map </exception>
		''' <exception cref="NullPointerException"> if a specified key or value is null,
		'''         and this map does not permit null keys or values </exception>
		''' <exception cref="IllegalArgumentException"> if some property of a specified key
		'''         or value prevents it from being stored in this map </exception>
		Function replace(ByVal key As K, ByVal oldValue As V, ByVal newValue As V) As Boolean

		''' <summary>
		''' Replaces the entry for a key only if currently mapped to some value.
		''' This is equivalent to
		'''  <pre> {@code
		''' if (map.containsKey(key)) {
		'''   return map.put(key, value);
		''' } else
		'''   return null;
		''' }</pre>
		''' 
		''' except that the action is performed atomically.
		''' 
		''' @implNote This implementation intentionally re-abstracts the
		''' inappropriate default provided in {@code Map}.
		''' </summary>
		''' <param name="key"> key with which the specified value is associated </param>
		''' <param name="value"> value to be associated with the specified key </param>
		''' <returns> the previous value associated with the specified key, or
		'''         {@code null} if there was no mapping for the key.
		'''         (A {@code null} return can also indicate that the map
		'''         previously associated {@code null} with the key,
		'''         if the implementation supports null values.) </returns>
		''' <exception cref="UnsupportedOperationException"> if the {@code put} operation
		'''         is not supported by this map </exception>
		''' <exception cref="ClassCastException"> if the class of the specified key or value
		'''         prevents it from being stored in this map </exception>
		''' <exception cref="NullPointerException"> if the specified key or value is null,
		'''         and this map does not permit null keys or values </exception>
		''' <exception cref="IllegalArgumentException"> if some property of the specified key
		'''         or value prevents it from being stored in this map </exception>
		Function replace(ByVal key As K, ByVal value As V) As V

		''' <summary>
		''' {@inheritDoc}
		''' 
		''' @implSpec
		''' <p>The default implementation is equivalent to, for this {@code map}:
		''' <pre> {@code
		''' for ((Map.Entry<K, V> entry : map.entrySet())
		'''     do {
		'''        K k = entry.getKey();
		'''        V v = entry.getValue();
		'''     } while(!replace(k, v, function.apply(k, v)));
		''' }</pre>
		''' 
		''' The default implementation may retry these steps when multiple
		''' threads attempt updates including potentially calling the function
		''' repeatedly for a given key.
		''' 
		''' <p>This implementation assumes that the ConcurrentMap cannot contain null
		''' values and {@code get()} returning null unambiguously means the key is
		''' absent. Implementations which support null values <strong>must</strong>
		''' override this default implementation.
		''' </summary>
		''' <exception cref="UnsupportedOperationException"> {@inheritDoc} </exception>
		''' <exception cref="NullPointerException"> {@inheritDoc} </exception>
		''' <exception cref="ClassCastException"> {@inheritDoc} </exception>
		''' <exception cref="IllegalArgumentException"> {@inheritDoc}
		''' @since 1.8 </exception>
'JAVA TO VB CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
		default Overrides Sub replaceAll(Of T1 As V)(ByVal [function] As java.util.function.BiFunction(Of T1))
'JAVA TO VB CONVERTER TODO TASK: The following line could not be converted:
			java.util.Objects.requireNonNull(function);
			Sub [New]((k,v) -> { while(!replace(k, v, function.apply(k, v))) { if((v = get(k)) == Nothing) { break; } } } ByVal  As )
					' v changed or k is gone
						' k is no longer in the map.

		''' <summary>
		''' {@inheritDoc}
		''' 
		''' @implSpec
		''' The default implementation is equivalent to the following steps for this
		''' {@code map}, then returning the current value or {@code null} if now
		''' absent:
		''' 
		''' <pre> {@code
		''' if (map.get(key) == null) {
		'''     V newValue = mappingFunction.apply(key);
		'''     if (newValue != null)
		'''         return map.putIfAbsent(key, newValue);
		''' }
		''' }</pre>
		''' 
		''' The default implementation may retry these steps when multiple
		''' threads attempt updates including potentially calling the mapping
		''' function multiple times.
		''' 
		''' <p>This implementation assumes that the ConcurrentMap cannot contain null
		''' values and {@code get()} returning null unambiguously means the key is
		''' absent. Implementations which support null values <strong>must</strong>
		''' override this default implementation.
		''' </summary>
		''' <exception cref="UnsupportedOperationException"> {@inheritDoc} </exception>
		''' <exception cref="ClassCastException"> {@inheritDoc} </exception>
		''' <exception cref="NullPointerException"> {@inheritDoc}
		''' @since 1.8 </exception>
'JAVA TO VB CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
		default Overrides Function computeIfAbsent(Of T1 As V)(ByVal key As K, ByVal mappingFunction As java.util.function.Function(Of T1)) As V
'JAVA TO VB CONVERTER TODO TASK: The following line could not be converted:
			java.util.Objects.requireNonNull(mappingFunction);
'JAVA TO VB CONVERTER TODO TASK: Interfaces cannot contain fields in .NET:
'			V v, newValue;
			Sub [New]((v = get(key)) == Nothing && (newValue = mappingFunction.apply(key)) != Nothing && (v = putIfAbsent(key, newValue)) == ByVal [Nothing] As )

		''' <summary>
		''' {@inheritDoc}
		''' 
		''' @implSpec
		''' The default implementation is equivalent to performing the following
		''' steps for this {@code map}, then returning the current value or
		''' {@code null} if now absent. :
		''' 
		''' <pre> {@code
		''' if (map.get(key) != null) {
		'''     V oldValue = map.get(key);
		'''     V newValue = remappingFunction.apply(key, oldValue);
		'''     if (newValue != null)
		'''         map.replace(key, oldValue, newValue);
		'''     else
		'''         map.remove(key, oldValue);
		''' }
		''' }</pre>
		''' 
		''' The default implementation may retry these steps when multiple threads
		''' attempt updates including potentially calling the remapping function
		''' multiple times.
		''' 
		''' <p>This implementation assumes that the ConcurrentMap cannot contain null
		''' values and {@code get()} returning null unambiguously means the key is
		''' absent. Implementations which support null values <strong>must</strong>
		''' override this default implementation.
		''' </summary>
		''' <exception cref="UnsupportedOperationException"> {@inheritDoc} </exception>
		''' <exception cref="ClassCastException"> {@inheritDoc} </exception>
		''' <exception cref="NullPointerException"> {@inheritDoc}
		''' @since 1.8 </exception>
'JAVA TO VB CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
		default Overrides Function computeIfPresent(Of T1 As V)(ByVal key As K, ByVal remappingFunction As java.util.function.BiFunction(Of T1)) As V
'JAVA TO VB CONVERTER TODO TASK: The following line could not be converted:
			java.util.Objects.requireNonNull(remappingFunction);
'JAVA TO VB CONVERTER TODO TASK: Interfaces cannot contain fields in .NET:
'			V oldValue;
			Sub [New]((oldValue = get(key)) != ByVal [Nothing] As )
'JAVA TO VB CONVERTER TODO TASK: Interfaces cannot contain fields in .NET:
'				V newValue = remappingFunction.apply(key, oldValue);
				Sub [New](newValue != ByVal [Nothing] As )
					Sub [New](replace(key, oldValue, newValue) ByVal  As )
'JAVA TO VB CONVERTER TODO TASK: Interfaces cannot contain fields in .NET:
'						Return newValue;
				Function [if](remove(key, oldValue) ByVal  As ) As else
'JAVA TO VB CONVERTER TODO TASK: Interfaces cannot contain fields in .NET:
'				   Return Nothing;
'JAVA TO VB CONVERTER TODO TASK: Interfaces cannot contain fields in .NET:
'			Return oldValue;

		''' <summary>
		''' {@inheritDoc}
		''' 
		''' @implSpec
		''' The default implementation is equivalent to performing the following
		''' steps for this {@code map}, then returning the current value or
		''' {@code null} if absent:
		''' 
		''' <pre> {@code
		''' V oldValue = map.get(key);
		''' V newValue = remappingFunction.apply(key, oldValue);
		''' if (oldValue != null ) {
		'''    if (newValue != null)
		'''       map.replace(key, oldValue, newValue);
		'''    else
		'''       map.remove(key, oldValue);
		''' } else {
		'''    if (newValue != null)
		'''       map.putIfAbsent(key, newValue);
		'''    else
		'''       return null;
		''' }
		''' }</pre>
		''' 
		''' The default implementation may retry these steps when multiple
		''' threads attempt updates including potentially calling the remapping
		''' function multiple times.
		''' 
		''' <p>This implementation assumes that the ConcurrentMap cannot contain null
		''' values and {@code get()} returning null unambiguously means the key is
		''' absent. Implementations which support null values <strong>must</strong>
		''' override this default implementation.
		''' </summary>
		''' <exception cref="UnsupportedOperationException"> {@inheritDoc} </exception>
		''' <exception cref="ClassCastException"> {@inheritDoc} </exception>
		''' <exception cref="NullPointerException"> {@inheritDoc}
		''' @since 1.8 </exception>
'JAVA TO VB CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
		default Overrides Function compute(Of T1 As V)(ByVal key As K, ByVal remappingFunction As java.util.function.BiFunction(Of T1)) As V
'JAVA TO VB CONVERTER TODO TASK: The following line could not be converted:
			java.util.Objects.requireNonNull(remappingFunction);
'JAVA TO VB CONVERTER TODO TASK: Interfaces cannot contain fields in .NET:
'			V oldValue = get(key);
			Sub [New](;; ByVal  As )
'JAVA TO VB CONVERTER TODO TASK: Interfaces cannot contain fields in .NET:
'				V newValue = remappingFunction.apply(key, oldValue);
				Sub [New](newValue == ByVal [Nothing] As )
					' delete mapping
					Sub [New](oldValue != Nothing || containsKey(key) ByVal  As )
						' something to remove
						Sub [New](remove(key, oldValue) ByVal  As )
							' removed the old value as expected
'JAVA TO VB CONVERTER TODO TASK: Interfaces cannot contain fields in .NET:
'							Return Nothing;

						' some other value replaced old value. try again.
'JAVA TO VB CONVERTER TODO TASK: The following line could not be converted:
						oldValue = get(key);
					Else
						' nothing to do. Leave things as they were.
'JAVA TO VB CONVERTER TODO TASK: Interfaces cannot contain fields in .NET:
'						Return Nothing;
					End If
				Else
					' add or replace old mapping
					Sub [New](oldValue != ByVal [Nothing] As )
						' replace
						Sub [New](replace(key, oldValue, newValue) ByVal  As )
							' replaced as expected.
'JAVA TO VB CONVERTER TODO TASK: Interfaces cannot contain fields in .NET:
'							Return newValue;

						' some other value replaced old value. try again.
'JAVA TO VB CONVERTER TODO TASK: The following line could not be converted:
						oldValue = get(key);
					Else
						' add (replace if oldValue was null)
						Sub [New]((oldValue = putIfAbsent(key, newValue)) == ByVal [Nothing] As )
							' replaced
'JAVA TO VB CONVERTER TODO TASK: Interfaces cannot contain fields in .NET:
'							Return newValue;

						' some other value replaced old value. try again.
					End If
				End If


		''' <summary>
		''' {@inheritDoc}
		''' 
		''' @implSpec
		''' The default implementation is equivalent to performing the following
		''' steps for this {@code map}, then returning the current value or
		''' {@code null} if absent:
		''' 
		''' <pre> {@code
		''' V oldValue = map.get(key);
		''' V newValue = (oldValue == null) ? value :
		'''              remappingFunction.apply(oldValue, value);
		''' if (newValue == null)
		'''     map.remove(key);
		''' else
		'''     map.put(key, newValue);
		''' }</pre>
		''' 
		''' <p>The default implementation may retry these steps when multiple
		''' threads attempt updates including potentially calling the remapping
		''' function multiple times.
		''' 
		''' <p>This implementation assumes that the ConcurrentMap cannot contain null
		''' values and {@code get()} returning null unambiguously means the key is
		''' absent. Implementations which support null values <strong>must</strong>
		''' override this default implementation.
		''' </summary>
		''' <exception cref="UnsupportedOperationException"> {@inheritDoc} </exception>
		''' <exception cref="ClassCastException"> {@inheritDoc} </exception>
		''' <exception cref="NullPointerException"> {@inheritDoc}
		''' @since 1.8 </exception>
'JAVA TO VB CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
		default Overrides Function merge(Of T1 As V)(ByVal key As K, ByVal value As V, ByVal remappingFunction As java.util.function.BiFunction(Of T1)) As V
'JAVA TO VB CONVERTER TODO TASK: The following line could not be converted:
			java.util.Objects.requireNonNull(remappingFunction);
'JAVA TO VB CONVERTER TODO TASK: The following line could not be converted:
			java.util.Objects.requireNonNull(value);
'JAVA TO VB CONVERTER TODO TASK: Interfaces cannot contain fields in .NET:
'			V oldValue = get(key);
			Sub [New](;; ByVal  As )
				Sub [New](oldValue != ByVal [Nothing] As )
'JAVA TO VB CONVERTER TODO TASK: Interfaces cannot contain fields in .NET:
'					V newValue = remappingFunction.apply(oldValue, value);
					Sub [New](newValue != ByVal [Nothing] As )
						Sub [New](replace(key, oldValue, newValue) ByVal  As )
'JAVA TO VB CONVERTER TODO TASK: Interfaces cannot contain fields in .NET:
'							Return newValue;
					Function [if](remove(key, oldValue) ByVal  As ) As else
'JAVA TO VB CONVERTER TODO TASK: Interfaces cannot contain fields in .NET:
'						Return Nothing;
'JAVA TO VB CONVERTER TODO TASK: The following line could not be converted:
					oldValue = get(key);
				Else
					Sub [New]((oldValue = putIfAbsent(key, value)) == ByVal [Nothing] As )
'JAVA TO VB CONVERTER TODO TASK: Interfaces cannot contain fields in .NET:
'						Return value;
				End If
	End Interface

End Namespace