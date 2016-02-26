Imports System
Imports System.Collections.Generic
Imports System.Text

'
' * Copyright (c) 2000, 2011, Oracle and/or its affiliates. All rights reserved.
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


Namespace javax.management.openmbean


	' java import
	'

	' jmx import
	'


	''' <summary>
	''' The <tt>TabularDataSupport</tt> class is the <i>open data</i> class which implements the <tt>TabularData</tt>
	''' and the <tt>Map</tt> interfaces, and which is internally based on a hash map data structure.
	''' 
	''' @since 1.5
	''' </summary>
	' It would make much more sense to implement
	'   Map<List<?>,CompositeData> here, but unfortunately we cannot for
	'   compatibility reasons.  If we did that, then we would have to
	'   define e.g.
	'   CompositeData remove(Object)
	'   instead of
	'   Object remove(Object).
	'
	'   That would mean that if any existing code subclassed
	'   TabularDataSupport and overrode
	'   Object remove(Object),
	'   it would (a) no longer compile and (b) not actually override
	'   CompositeData remove(Object)
	'   in binaries compiled before the change.
	'
	<Serializable> _
	Public Class TabularDataSupport
		Implements TabularData, IDictionary(Of Object, Object), ICloneable


		' Serial version 
		Friend Const serialVersionUID As Long = 5720150593236309827L


		''' <summary>
		''' @serial This tabular data instance's contents: a <seealso cref="HashMap"/>
		''' </summary>
		' field cannot be final because of clone method
		Private dataMap As IDictionary(Of Object, CompositeData)

		''' <summary>
		''' @serial This tabular data instance's tabular type
		''' </summary>
		Private ReadOnly tabularType As TabularType

		''' <summary>
		''' The array of item names that define the index used for rows (convenience field)
		''' </summary>
		<NonSerialized> _
		Private indexNamesArray As String()



		' *** Constructors *** 


		''' <summary>
		''' Creates an empty <tt>TabularDataSupport</tt> instance whose open-type is <var>tabularType</var>,
		''' and whose underlying <tt>HashMap</tt> has a default initial capacity (101) and default load factor (0.75).
		''' <p>
		''' This constructor simply calls <tt>this(tabularType, 101, 0.75f);</tt>
		''' </summary>
		''' <param name="tabularType">               the <i>tabular type</i> describing this <tt>TabularData</tt> instance;
		'''                                   cannot be null.
		''' </param>
		''' <exception cref="IllegalArgumentException">  if the tabular type is null. </exception>
		Public Sub New(ByVal ___tabularType As TabularType)

			Me.New(___tabularType, 16, 0.75f)
		End Sub

		''' <summary>
		''' Creates an empty <tt>TabularDataSupport</tt> instance whose open-type is <var>tabularType</var>,
		''' and whose underlying <tt>HashMap</tt> has the specified initial capacity and load factor.
		''' </summary>
		''' <param name="tabularType">               the <i>tabular type</i> describing this <tt>TabularData</tt> instance;
		'''                           cannot be null.
		''' </param>
		''' <param name="initialCapacity">   the initial capacity of the HashMap.
		''' </param>
		''' <param name="loadFactor">        the load factor of the HashMap
		''' </param>
		''' <exception cref="IllegalArgumentException">  if the initial capacity is less than zero,
		'''                                   or the load factor is nonpositive,
		'''                                   or the tabular type is null. </exception>
		Public Sub New(ByVal ___tabularType As TabularType, ByVal initialCapacity As Integer, ByVal loadFactor As Single)

			' Check tabularType is not null
			'
			If ___tabularType Is Nothing Then Throw New System.ArgumentException("Argument tabularType cannot be null.")

			' Initialize this.tabularType (and indexNamesArray for convenience)
			'
			Me.tabularType = ___tabularType
			Dim tmpNames As IList(Of String) = ___tabularType.indexNames
			Me.indexNamesArray = tmpNames.ToArray()

			' Since LinkedHashMap was introduced in SE 1.4, it's conceivable even
			' if very unlikely that we might be the server of a 1.3 client.  In
			' that case you'll need to set this property.  See CR 6334663.
			Dim useHashMapProp As String = java.security.AccessController.doPrivileged(New com.sun.jmx.mbeanserver.GetPropertyAction("jmx.tabular.data.hash.map"))
			Dim useHashMap As Boolean = "true".equalsIgnoreCase(useHashMapProp)

			' Construct the empty contents HashMap
			'
			Me.dataMap = If(useHashMap, New Dictionary(Of Object, CompositeData)(initialCapacity, loadFactor), New java.util.LinkedHashMap(Of Object, CompositeData)(initialCapacity, loadFactor))
		End Sub




		' *** TabularData specific information methods *** 


		''' <summary>
		''' Returns the <i>tabular type</i> describing this <tt>TabularData</tt> instance.
		''' </summary>
		Public Overridable Property tabularType As TabularType Implements TabularData.getTabularType
			Get
    
				Return tabularType
			End Get
		End Property

		''' <summary>
		''' Calculates the index that would be used in this <tt>TabularData</tt> instance to refer to the specified
		''' composite data <var>value</var> parameter if it were added to this instance.
		''' This method checks for the type validity of the specified <var>value</var>,
		''' but does not check if the calculated index is already used to refer to a value in this <tt>TabularData</tt> instance.
		''' </summary>
		''' <param name="value">                      the composite data value whose index in this
		'''                                    <tt>TabularData</tt> instance is to be calculated;
		'''                                    must be of the same composite type as this instance's row type;
		'''                                    must not be null.
		''' </param>
		''' <returns> the index that the specified <var>value</var> would have in this <tt>TabularData</tt> instance.
		''' </returns>
		''' <exception cref="NullPointerException">       if <var>value</var> is <tt>null</tt>.
		''' </exception>
		''' <exception cref="InvalidOpenTypeException">   if <var>value</var> does not conform to this <tt>TabularData</tt> instance's
		'''                                    row type definition. </exception>
		Public Overridable Function calculateIndex(ByVal value As CompositeData) As Object() Implements TabularData.calculateIndex

			' Check value is valid
			'
			checkValueType(value)

			' Return its calculated index
			'
			Return internalCalculateIndex(value).ToArray()
		End Function




		' *** Content information query methods *** 


		''' <summary>
		''' Returns <tt>true</tt> if and only if this <tt>TabularData</tt> instance contains a <tt>CompositeData</tt> value
		''' (ie a row) whose index is the specified <var>key</var>. If <var>key</var> cannot be cast to a one dimension array
		''' of Object instances, this method simply returns <tt>false</tt>; otherwise it returns the the result of the call to
		''' <tt>this.containsKey((Object[]) key)</tt>.
		''' </summary>
		''' <param name="key">  the index value whose presence in this <tt>TabularData</tt> instance is to be tested.
		''' </param>
		''' <returns>  <tt>true</tt> if this <tt>TabularData</tt> indexes a row value with the specified key. </returns>
		Public Overridable Function containsKey(ByVal key As Object) As Boolean

			' if key is not an array of Object instances, return false
			'
			Dim k As Object()
			Try
				k = CType(key, Object())
			Catch e As ClassCastException
				Return False
			End Try

			Return Me.containsKey(k)
		End Function

		''' <summary>
		''' Returns <tt>true</tt> if and only if this <tt>TabularData</tt> instance contains a <tt>CompositeData</tt> value
		''' (ie a row) whose index is the specified <var>key</var>. If <var>key</var> is <tt>null</tt> or does not conform to
		''' this <tt>TabularData</tt> instance's <tt>TabularType</tt> definition, this method simply returns <tt>false</tt>.
		''' </summary>
		''' <param name="key">  the index value whose presence in this <tt>TabularData</tt> instance is to be tested.
		''' </param>
		''' <returns>  <tt>true</tt> if this <tt>TabularData</tt> indexes a row value with the specified key. </returns>
		Public Overridable Function containsKey(ByVal key As Object()) As Boolean Implements TabularData.containsKey

			Return (If(key Is Nothing, False, dataMap.ContainsKey(java.util.Arrays.asList(key))))
		End Function

		''' <summary>
		''' Returns <tt>true</tt> if and only if this <tt>TabularData</tt> instance contains the specified
		''' <tt>CompositeData</tt> value. If <var>value</var> is <tt>null</tt> or does not conform to
		''' this <tt>TabularData</tt> instance's row type definition, this method simply returns <tt>false</tt>.
		''' </summary>
		''' <param name="value">  the row value whose presence in this <tt>TabularData</tt> instance is to be tested.
		''' </param>
		''' <returns>  <tt>true</tt> if this <tt>TabularData</tt> instance contains the specified row value. </returns>
		Public Overridable Function containsValue(ByVal value As CompositeData) As Boolean Implements TabularData.containsValue

			Return dataMap.ContainsValue(value)
		End Function

		''' <summary>
		''' Returns <tt>true</tt> if and only if this <tt>TabularData</tt> instance contains the specified
		''' value.
		''' </summary>
		''' <param name="value">  the row value whose presence in this <tt>TabularData</tt> instance is to be tested.
		''' </param>
		''' <returns>  <tt>true</tt> if this <tt>TabularData</tt> instance contains the specified row value. </returns>
		Public Overridable Function containsValue(ByVal value As Object) As Boolean

			Return dataMap.ContainsValue(value)
		End Function

		''' <summary>
		''' This method simply calls <tt>get((Object[]) key)</tt>.
		''' </summary>
		''' <exception cref="NullPointerException">  if the <var>key</var> is <tt>null</tt> </exception>
		''' <exception cref="ClassCastException">    if the <var>key</var> is not of the type <tt>Object[]</tt> </exception>
		''' <exception cref="InvalidKeyException">   if the <var>key</var> does not conform to this <tt>TabularData</tt> instance's
		'''                               <tt>TabularType</tt> definition </exception>
		Public Overridable Function [get](ByVal key As Object) As Object

			Return [get](CType(key, Object()))
		End Function

		''' <summary>
		''' Returns the <tt>CompositeData</tt> value whose index is
		''' <var>key</var>, or <tt>null</tt> if there is no value mapping
		''' to <var>key</var>, in this <tt>TabularData</tt> instance.
		''' </summary>
		''' <param name="key"> the index of the value to get in this
		''' <tt>TabularData</tt> instance; * must be valid with this
		''' <tt>TabularData</tt> instance's row type definition; * must not
		''' be null.
		''' </param>
		''' <returns> the value corresponding to <var>key</var>.
		''' </returns>
		''' <exception cref="NullPointerException">  if the <var>key</var> is <tt>null</tt> </exception>
		''' <exception cref="InvalidKeyException">   if the <var>key</var> does not conform to this <tt>TabularData</tt> instance's
		'''                               <tt>TabularType</tt> type definition. </exception>
		Public Overridable Function [get](ByVal key As Object()) As CompositeData Implements TabularData.get

			' Check key is not null and valid with tabularType
			' (throws NullPointerException, InvalidKeyException)
			'
			checkKeyType(key)

			' Return the mapping stored in the parent HashMap
			'
			Return dataMap(java.util.Arrays.asList(key))
		End Function




		' *** Content modification operations (one element at a time) *** 


		''' <summary>
		''' This method simply calls <tt>put((CompositeData) value)</tt> and
		''' therefore ignores its <var>key</var> parameter which can be <tt>null</tt>.
		''' </summary>
		''' <param name="key"> an ignored parameter. </param>
		''' <param name="value"> the <seealso cref="CompositeData"/> to put.
		''' </param>
		''' <returns> the value which is put
		''' </returns>
		''' <exception cref="NullPointerException">  if the <var>value</var> is <tt>null</tt> </exception>
		''' <exception cref="ClassCastException"> if the <var>value</var> is not of
		''' the type <tt>CompositeData</tt> </exception>
		''' <exception cref="InvalidOpenTypeException"> if the <var>value</var> does
		''' not conform to this <tt>TabularData</tt> instance's
		''' <tt>TabularType</tt> definition </exception>
		''' <exception cref="KeyAlreadyExistsException"> if the key for the
		''' <var>value</var> parameter, calculated according to this
		''' <tt>TabularData</tt> instance's <tt>TabularType</tt> definition
		''' already maps to an existing value </exception>
		Public Overridable Function put(ByVal key As Object, ByVal value As Object) As Object
			internalPut(CType(value, CompositeData))
			Return value ' should be return internalPut(...); (5090566)
		End Function

		Public Overridable Sub put(ByVal value As CompositeData) Implements TabularData.put
			internalPut(value)
		End Sub

		Private Function internalPut(ByVal value As CompositeData) As CompositeData
			' Check value is not null, value's type is the same as this instance's row type,
			' and calculate the value's index according to this instance's tabularType and
			' check it is not already used for a mapping in the parent HashMap
			'
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
			Dim index As IList(Of ?) = checkValueAndIndex(value)

			' store the (key, value) mapping in the dataMap HashMap
			'
				dataMap(index) = value
				Return dataMap(index)
		End Function

		''' <summary>
		''' This method simply calls <tt>remove((Object[]) key)</tt>.
		''' </summary>
		''' <param name="key"> an <tt>Object[]</tt> representing the key to remove.
		''' </param>
		''' <returns> previous value associated with specified key, or <tt>null</tt>
		'''         if there was no mapping for key.
		''' </returns>
		''' <exception cref="NullPointerException">  if the <var>key</var> is <tt>null</tt> </exception>
		''' <exception cref="ClassCastException">    if the <var>key</var> is not of the type <tt>Object[]</tt> </exception>
		''' <exception cref="InvalidKeyException">   if the <var>key</var> does not conform to this <tt>TabularData</tt> instance's
		'''                               <tt>TabularType</tt> definition </exception>
		Public Overridable Function remove(ByVal key As Object) As Object

			Return remove(CType(key, Object()))
		End Function

		''' <summary>
		''' Removes the <tt>CompositeData</tt> value whose index is <var>key</var> from this <tt>TabularData</tt> instance,
		''' and returns the removed value, or returns <tt>null</tt> if there is no value whose index is <var>key</var>.
		''' </summary>
		''' <param name="key">  the index of the value to get in this <tt>TabularData</tt> instance;
		'''              must be valid with this <tt>TabularData</tt> instance's row type definition;
		'''              must not be null.
		''' </param>
		''' <returns> previous value associated with specified key, or <tt>null</tt>
		'''         if there was no mapping for key.
		''' </returns>
		''' <exception cref="NullPointerException">  if the <var>key</var> is <tt>null</tt> </exception>
		''' <exception cref="InvalidKeyException">   if the <var>key</var> does not conform to this <tt>TabularData</tt> instance's
		'''                               <tt>TabularType</tt> definition </exception>
		Public Overridable Function remove(ByVal key As Object()) As CompositeData Implements TabularData.remove

			' Check key is not null and valid with tabularType
			' (throws NullPointerException, InvalidKeyException)
			'
			checkKeyType(key)

			' Removes the (key, value) mapping in the parent HashMap
			'
			Return dataMap.Remove(java.util.Arrays.asList(key))
		End Function



		' ***   Content modification bulk operations   *** 


		''' <summary>
		''' Add all the values contained in the specified map <var>t</var>
		''' to this <tt>TabularData</tt> instance.  This method converts
		''' the collection of values contained in this map into an array of
		''' <tt>CompositeData</tt> values, if possible, and then call the
		''' method <tt>putAll(CompositeData[])</tt>. Note that the keys
		''' used in the specified map <var>t</var> are ignored. This method
		''' allows, for example to add the content of another
		''' <tt>TabularData</tt> instance with the same row type (but
		''' possibly different index names) into this instance.
		''' </summary>
		''' <param name="t"> the map whose values are to be added as new rows to
		''' this <tt>TabularData</tt> instance; if <var>t</var> is
		''' <tt>null</tt> or empty, this method returns without doing
		''' anything.
		''' </param>
		''' <exception cref="NullPointerException"> if a value in <var>t</var> is
		''' <tt>null</tt>. </exception>
		''' <exception cref="ClassCastException"> if a value in <var>t</var> is not an
		''' instance of <tt>CompositeData</tt>. </exception>
		''' <exception cref="InvalidOpenTypeException"> if a value in <var>t</var>
		''' does not conform to this <tt>TabularData</tt> instance's row
		''' type definition. </exception>
		''' <exception cref="KeyAlreadyExistsException"> if the index for a value in
		''' <var>t</var>, calculated according to this
		''' <tt>TabularData</tt> instance's <tt>TabularType</tt> definition
		''' already maps to an existing value in this instance, or two
		''' values in <var>t</var> have the same index. </exception>
		Public Overridable Sub putAll(Of T1)(ByVal t As IDictionary(Of T1))

			' if t is null or empty, just return
			'
			If (t Is Nothing) OrElse (t.Count = 0) Then Return

			' Convert the values in t into an array of <tt>CompositeData</tt>
			'
			Dim values As CompositeData()
			Try
				values = t.Values.ToArray(New CompositeData(t.Count - 1){})
			Catch e As java.lang.ArrayStoreException
				Throw New ClassCastException("Map argument t contains values which are not instances of <tt>CompositeData</tt>")
			End Try

			' Add the array of values
			'
			putAll(values)
		End Sub

		''' <summary>
		''' Add all the elements in <var>values</var> to this
		''' <tt>TabularData</tt> instance.  If any element in
		''' <var>values</var> does not satisfy the constraints defined in
		''' <seealso cref="#put(CompositeData) <tt>put</tt>"/>, or if any two
		''' elements in <var>values</var> have the same index calculated
		''' according to this <tt>TabularData</tt> instance's
		''' <tt>TabularType</tt> definition, then an exception describing
		''' the failure is thrown and no element of <var>values</var> is
		''' added, thus leaving this <tt>TabularData</tt> instance
		''' unchanged.
		''' </summary>
		''' <param name="values"> the array of composite data values to be added as
		''' new rows to this <tt>TabularData</tt> instance; if
		''' <var>values</var> is <tt>null</tt> or empty, this method
		''' returns without doing anything.
		''' </param>
		''' <exception cref="NullPointerException"> if an element of <var>values</var>
		''' is <tt>null</tt> </exception>
		''' <exception cref="InvalidOpenTypeException"> if an element of
		''' <var>values</var> does not conform to this
		''' <tt>TabularData</tt> instance's row type definition (ie its
		''' <tt>TabularType</tt> definition) </exception>
		''' <exception cref="KeyAlreadyExistsException"> if the index for an element
		''' of <var>values</var>, calculated according to this
		''' <tt>TabularData</tt> instance's <tt>TabularType</tt> definition
		''' already maps to an existing value in this instance, or two
		''' elements of <var>values</var> have the same index </exception>
		Public Overridable Sub putAll(ByVal values As CompositeData()) Implements TabularData.putAll

			' if values is null or empty, just return
			'
			If (values Is Nothing) OrElse (values.Length = 0) Then Return

			' create the list of indexes corresponding to each value
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
			Dim indexes As IList(Of IList(Of ?)) = New List(Of IList(Of ?))(values.Length + 1)

			' Check all elements in values and build index list
			'
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
			Dim index As IList(Of ?)
			For i As Integer = 0 To values.Length - 1
				' check value and calculate index
				index = checkValueAndIndex(values(i))
				' check index is different of those previously calculated
				If indexes.Contains(index) Then Throw New KeyAlreadyExistsException("Argument elements values[" & i & "] and values[" & indexes.IndexOf(index) & "] have the same indexes, " & "calculated according to this TabularData instance's tabularType.")
				' add to index list
				indexes.Add(index)
			Next i

			' store all (index, value) mappings in the dataMap HashMap
			'
			For i As Integer = 0 To values.Length - 1
				dataMap(indexes(i)) = values(i)
			Next i
		End Sub

		''' <summary>
		''' Removes all rows from this <code>TabularDataSupport</code> instance.
		''' </summary>
		Public Overridable Sub clear() Implements TabularData.clear

			dataMap.Clear()
		End Sub



		' ***  Informational methods from java.util.Map  *** 

		''' <summary>
		''' Returns the number of rows in this <code>TabularDataSupport</code> instance.
		''' </summary>
		''' <returns> the number of rows in this <code>TabularDataSupport</code> instance. </returns>
		Public Overridable Function size() As Integer Implements TabularData.size

			Return dataMap.Count
		End Function

		''' <summary>
		''' Returns <tt>true</tt> if this <code>TabularDataSupport</code> instance contains no rows.
		''' </summary>
		''' <returns> <tt>true</tt> if this <code>TabularDataSupport</code> instance contains no rows. </returns>
		Public Overridable Property empty As Boolean Implements TabularData.isEmpty
			Get
    
				Return (Me.size() = 0)
			End Get
		End Property



		' ***  Collection views from java.util.Map  *** 

		''' <summary>
		''' Returns a set view of the keys contained in the underlying map of this
		''' {@code TabularDataSupport} instance used to index the rows.
		''' Each key contained in this {@code Set} is an unmodifiable {@code List<?>}
		''' so the returned set view is a {@code Set<List<?>>} but is declared as a
		''' {@code Set<Object>} for compatibility reasons.
		''' The set is backed by the underlying map of this
		''' {@code TabularDataSupport} instance, so changes to the
		''' {@code TabularDataSupport} instance are reflected in the
		''' set, and vice-versa.
		''' 
		''' The set supports element removal, which removes the corresponding
		''' row from this {@code TabularDataSupport} instance, via the
		''' <seealso cref="Iterator#remove"/>, <seealso cref="Set#remove"/>, <seealso cref="Set#removeAll"/>,
		''' <seealso cref="Set#retainAll"/>, and <seealso cref="Set#clear"/> operations. It does
		'''  not support the <seealso cref="Set#add"/> or <seealso cref="Set#addAll"/> operations.
		''' </summary>
		''' <returns> a set view ({@code Set<List<?>>}) of the keys used to index
		''' the rows of this {@code TabularDataSupport} instance. </returns>
		Public Overridable Function keySet() As java.util.Set(Of Object) Implements TabularData.keySet

			Return dataMap.Keys
		End Function

		''' <summary>
		''' Returns a collection view of the rows contained in this
		''' {@code TabularDataSupport} instance. The returned {@code Collection}
		''' is a {@code Collection<CompositeData>} but is declared as a
		''' {@code Collection<Object>} for compatibility reasons.
		''' The returned collection can be used to iterate over the values.
		''' The collection is backed by the underlying map, so changes to the
		''' {@code TabularDataSupport} instance are reflected in the collection,
		''' and vice-versa.
		''' 
		''' The collection supports element removal, which removes the corresponding
		''' index to row mapping from this {@code TabularDataSupport} instance, via
		''' the <seealso cref="Iterator#remove"/>, <seealso cref="Collection#remove"/>,
		''' <seealso cref="Collection#removeAll"/>, <seealso cref="Collection#retainAll"/>,
		''' and <seealso cref="Collection#clear"/> operations. It does not support
		''' the <seealso cref="Collection#add"/> or <seealso cref="Collection#addAll"/> operations.
		''' </summary>
		''' <returns> a collection view ({@code Collection<CompositeData>}) of
		''' the values contained in this {@code TabularDataSupport} instance. </returns>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
		Public Overridable Function values() As ICollection(Of Object) Implements TabularData.values ' historical confusion about the return type

			Return com.sun.jmx.mbeanserver.Util.cast(dataMap.Values)
		End Function


		''' <summary>
		''' Returns a collection view of the index to row mappings
		''' contained in this {@code TabularDataSupport} instance.
		''' Each element in the returned collection is
		''' a {@code Map.Entry<List<?>,CompositeData>} but
		''' is declared as a {@code Map.Entry<Object,Object>}
		''' for compatibility reasons. Each of the map entry
		''' keys is an unmodifiable {@code List<?>}.
		''' The collection is backed by the underlying map of this
		''' {@code TabularDataSupport} instance, so changes to the
		''' {@code TabularDataSupport} instance are reflected in
		''' the collection, and vice-versa.
		''' The collection supports element removal, which removes
		''' the corresponding mapping from the map, via the
		''' <seealso cref="Iterator#remove"/>, <seealso cref="Collection#remove"/>,
		''' <seealso cref="Collection#removeAll"/>, <seealso cref="Collection#retainAll"/>,
		''' and <seealso cref="Collection#clear"/> operations. It does not support
		''' the <seealso cref="Collection#add"/> or <seealso cref="Collection#addAll"/>
		''' operations.
		''' <p>
		''' <b>IMPORTANT NOTICE</b>: Do not use the {@code setValue} method of the
		''' {@code Map.Entry} elements contained in the returned collection view.
		''' Doing so would corrupt the index to row mappings contained in this
		''' {@code TabularDataSupport} instance.
		''' </summary>
		''' <returns> a collection view ({@code Set<Map.Entry<List<?>,CompositeData>>})
		''' of the mappings contained in this map. </returns>
		''' <seealso cref= java.util.Map.Entry </seealso>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
		Public Overridable Function entrySet() As java.util.Set(Of KeyValuePair(Of Object, Object)) ' historical confusion about the return type

'JAVA TO VB CONVERTER TODO TASK: There is no .NET Dictionary equivalent to the Java 'entrySet' method:
			Return com.sun.jmx.mbeanserver.Util.cast(dataMap.entrySet())
		End Function


		' ***  Commodity methods from java.lang.Object  *** 


		''' <summary>
		''' Returns a clone of this <code>TabularDataSupport</code> instance:
		''' the clone is obtained by calling <tt>super.clone()</tt>, and then cloning the underlying map.
		''' Only a shallow clone of the underlying map is made, i.e. no cloning of the indexes and row values is made as they are immutable.
		''' </summary>
	'     We cannot use covariance here and return TabularDataSupport
	'       because this would fail with existing code that subclassed
	'       TabularDataSupport and overrode Object clone().  It would not
	'       override the new clone().  
		Public Overridable Function clone() As Object
			Try
				Dim c As TabularDataSupport = CType(MyBase.clone(), TabularDataSupport)
				c.dataMap = New Dictionary(Of Object, CompositeData)(c.dataMap)
				Return c
			Catch e As CloneNotSupportedException
				Throw New InternalError(e.ToString(), e)
			End Try
		End Function


		''' <summary>
		''' Compares the specified <var>obj</var> parameter with this <code>TabularDataSupport</code> instance for equality.
		''' <p>
		''' Returns <tt>true</tt> if and only if all of the following statements are true:
		''' <ul>
		''' <li><var>obj</var> is non null,</li>
		''' <li><var>obj</var> also implements the <code>TabularData</code> interface,</li>
		''' <li>their tabular types are equal</li>
		''' <li>their contents (ie all CompositeData values) are equal.</li>
		''' </ul>
		''' This ensures that this <tt>equals</tt> method works properly for <var>obj</var> parameters which are
		''' different implementations of the <code>TabularData</code> interface.
		''' <br>&nbsp; </summary>
		''' <param name="obj">  the object to be compared for equality with this <code>TabularDataSupport</code> instance;
		''' </param>
		''' <returns>  <code>true</code> if the specified object is equal to this <code>TabularDataSupport</code> instance. </returns>
		Public Overrides Function Equals(ByVal obj As Object) As Boolean

			' if obj is null, return false
			'
			If obj Is Nothing Then Return False

			' if obj is not a TabularData, return false
			'
			Dim other As TabularData
			Try
				other = CType(obj, TabularData)
			Catch e As ClassCastException
				Return False
			End Try

			' Now, really test for equality between this TabularData implementation and the other:
			'

			' their tabularType should be equal
			If Not Me.tabularType.Equals(other.tabularType) Then Return False

			' their contents should be equal:
			' . same size
			' . values in this instance are in the other (we know there are no duplicate elements possible)
			' (row values comparison is enough, because keys are calculated according to tabularType)

			If Me.size() <> other.size() Then Return False
			For Each value As CompositeData In dataMap.Values
				If Not other.containsValue(value) Then Return False
			Next value

			' All tests for equality were successfull
			'
			Return True
		End Function

		''' <summary>
		''' Returns the hash code value for this <code>TabularDataSupport</code> instance.
		''' <p>
		''' The hash code of a <code>TabularDataSupport</code> instance is the sum of the hash codes
		''' of all elements of information used in <code>equals</code> comparisons
		''' (ie: its <i>tabular type</i> and its content, where the content is defined as all the CompositeData values).
		''' <p>
		''' This ensures that <code> t1.equals(t2) </code> implies that <code> t1.hashCode()==t2.hashCode() </code>
		''' for any two <code>TabularDataSupport</code> instances <code>t1</code> and <code>t2</code>,
		''' as required by the general contract of the method
		''' <seealso cref="Object#hashCode() Object.hashCode()"/>.
		''' <p>
		''' However, note that another instance of a class implementing the <code>TabularData</code> interface
		''' may be equal to this <code>TabularDataSupport</code> instance as defined by <seealso cref="#equals"/>,
		''' but may have a different hash code if it is calculated differently.
		''' </summary>
		''' <returns>  the hash code value for this <code>TabularDataSupport</code> instance </returns>
	   Public Overrides Function GetHashCode() As Integer

			Dim result As Integer = 0

			result += Me.tabularType.GetHashCode()
			For Each value As Object In values()
				result += value.GetHashCode()
			Next value

			Return result

	   End Function

		''' <summary>
		''' Returns a string representation of this <code>TabularDataSupport</code> instance.
		''' <p>
		''' The string representation consists of the name of this class (ie <code>javax.management.openmbean.TabularDataSupport</code>),
		''' the string representation of the tabular type of this instance, and the string representation of the contents
		''' (ie list the key=value mappings as returned by a call to
		''' <tt>dataMap.</tt><seealso cref="java.util.HashMap#toString() toString()"/>).
		''' </summary>
		''' <returns>  a string representation of this <code>TabularDataSupport</code> instance </returns>
		Public Overrides Function ToString() As String

			Return (New StringBuilder).Append(Me.GetType().name).append("(tabularType=").append(tabularType.ToString()).append(",contents=").append(dataMap.ToString()).append(")").ToString()
		End Function




		' *** TabularDataSupport internal utility methods *** 


		''' <summary>
		''' Returns the index for value, assuming value is valid for this <tt>TabularData</tt> instance
		''' (ie value is not null, and its composite type is equal to row type).
		''' 
		''' The index is a List, and not an array, so that an index.equals(otherIndex) call will actually compare contents,
		''' not just the objects references as is done for an array object.
		''' 
		''' The returned List is unmodifiable so that once a row has been put into the dataMap, its index cannot be modified,
		''' for example by a user that would attempt to modify an index contained in the Set returned by keySet().
		''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
		Private Function internalCalculateIndex(ByVal value As CompositeData) As IList(Of ?)

			Return java.util.Collections.unmodifiableList(java.util.Arrays.asList(value.getAll(Me.indexNamesArray)))
		End Function

		''' <summary>
		''' Checks if the specified key is valid for this <tt>TabularData</tt> instance.
		''' </summary>
		''' <exception cref="NullPointerException"> </exception>
		''' <exception cref="InvalidOpenTypeException"> </exception>
		Private Sub checkKeyType(ByVal key As Object())

			' Check key is neither null nor empty
			'
			If (key Is Nothing) OrElse (key.Length = 0) Then Throw New NullPointerException("Argument key cannot be null or empty.")

			' Now check key is valid with tabularType index and row type definitions: 

			' key[] should have the size expected for an index
			'
			If key.Length <> Me.indexNamesArray.Length Then Throw New InvalidKeyException("Argument key's length=" & key.Length & " is different from the number of item values, which is " & indexNamesArray.Length & ", specified for the indexing rows in this TabularData instance.")

			' each element in key[] should be a value for its corresponding open type specified in rowType
			'
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
			Dim keyElementType As OpenType(Of ?)
			For i As Integer = 0 To key.Length - 1
				keyElementType = tabularType.rowType.getType(Me.indexNamesArray(i))
				If (key(i) IsNot Nothing) AndAlso ((Not keyElementType.isValue(key(i)))) Then Throw New InvalidKeyException("Argument element key[" & i & "] is not a value for the open type expected for " & "this element of the index, whose name is """ & indexNamesArray(i) & """ and whose open type is " & keyElementType)
			Next i
		End Sub

		''' <summary>
		''' Checks the specified value's type is valid for this <tt>TabularData</tt> instance
		''' (ie value is not null, and its composite type is equal to row type).
		''' </summary>
		''' <exception cref="NullPointerException"> </exception>
		''' <exception cref="InvalidOpenTypeException"> </exception>
		Private Sub checkValueType(ByVal value As CompositeData)

			' Check value is not null
			'
			If value Is Nothing Then Throw New NullPointerException("Argument value cannot be null.")

			' if value's type is not the same as this instance's row type, throw InvalidOpenTypeException
			'
			If Not tabularType.rowType.isValue(value) Then Throw New InvalidOpenTypeException("Argument value's composite type [" & value.compositeType & "] is not assignable to " & "this TabularData instance's row type [" & tabularType.rowType & "].")
		End Sub

		''' <summary>
		''' Checks if the specified value can be put (ie added) in this <tt>TabularData</tt> instance
		''' (ie value is not null, its composite type is equal to row type, and its index is not already used),
		''' and returns the index calculated for this value.
		''' 
		''' The index is a List, and not an array, so that an index.equals(otherIndex) call will actually compare contents,
		''' not just the objects references as is done for an array object.
		''' </summary>
		''' <exception cref="NullPointerException"> </exception>
		''' <exception cref="InvalidOpenTypeException"> </exception>
		''' <exception cref="KeyAlreadyExistsException"> </exception>
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
		Private Function checkValueAndIndex(ByVal value As CompositeData) As IList(Of ?)

			' Check value is valid
			'
			checkValueType(value)

			' Calculate value's index according to this instance's tabularType
			' and check it is not already used for a mapping in the parent HashMap
			'
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
			Dim index As IList(Of ?) = internalCalculateIndex(value)

			If dataMap.ContainsKey(index) Then Throw New KeyAlreadyExistsException("Argument value's index, calculated according to this TabularData " & "instance's tabularType, already refers to a value in this table.")

			' The check is OK, so return the index
			'
			Return index
		End Function

		''' <summary>
		''' Deserializes a <seealso cref="TabularDataSupport"/> from an <seealso cref="ObjectInputStream"/>.
		''' </summary>
		Private Sub readObject(ByVal [in] As java.io.ObjectInputStream)
		  [in].defaultReadObject()
		  Dim tmpNames As IList(Of String) = tabularType.indexNames
		  indexNamesArray = tmpNames.ToArray()
		End Sub
	End Class

End Namespace