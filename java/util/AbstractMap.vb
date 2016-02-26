Imports System
Imports System.Collections
Imports System.Collections.Generic

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
	''' This class provides a skeletal implementation of the <tt>Map</tt>
	''' interface, to minimize the effort required to implement this interface.
	''' 
	''' <p>To implement an unmodifiable map, the programmer needs only to extend this
	''' class and provide an implementation for the <tt>entrySet</tt> method, which
	''' returns a set-view of the map's mappings.  Typically, the returned set
	''' will, in turn, be implemented atop <tt>AbstractSet</tt>.  This set should
	''' not support the <tt>add</tt> or <tt>remove</tt> methods, and its iterator
	''' should not support the <tt>remove</tt> method.
	''' 
	''' <p>To implement a modifiable map, the programmer must additionally override
	''' this class's <tt>put</tt> method (which otherwise throws an
	''' <tt>UnsupportedOperationException</tt>), and the iterator returned by
	''' <tt>entrySet().iterator()</tt> must additionally implement its
	''' <tt>remove</tt> method.
	''' 
	''' <p>The programmer should generally provide a void (no argument) and map
	''' constructor, as per the recommendation in the <tt>Map</tt> interface
	''' specification.
	''' 
	''' <p>The documentation for each non-abstract method in this class describes its
	''' implementation in detail.  Each of these methods may be overridden if the
	''' map being implemented admits a more efficient implementation.
	''' 
	''' <p>This class is a member of the
	''' <a href="{@docRoot}/../technotes/guides/collections/index.html">
	''' Java Collections Framework</a>.
	''' </summary>
	''' @param <K> the type of keys maintained by this map </param>
	''' @param <V> the type of mapped values
	''' 
	''' @author  Josh Bloch
	''' @author  Neal Gafter </param>
	''' <seealso cref= Map </seealso>
	''' <seealso cref= Collection
	''' @since 1.2 </seealso>

	Public MustInherit Class AbstractMap(Of K, V)
		Implements Map(Of K, V)

		''' <summary>
		''' Sole constructor.  (For invocation by subclass constructors, typically
		''' implicit.)
		''' </summary>
		Protected Friend Sub New()
		End Sub

		' Query Operations

		''' <summary>
		''' {@inheritDoc}
		''' 
		''' @implSpec
		''' This implementation returns <tt>entrySet().size()</tt>.
		''' </summary>
		Public Overridable Function size() As Integer Implements Map(Of K, V).size
			Return entrySet().size()
		End Function

		''' <summary>
		''' {@inheritDoc}
		''' 
		''' @implSpec
		''' This implementation returns <tt>size() == 0</tt>.
		''' </summary>
		Public Overridable Property empty As Boolean Implements Map(Of K, V).isEmpty
			Get
				Return size() = 0
			End Get
		End Property

		''' <summary>
		''' {@inheritDoc}
		''' 
		''' @implSpec
		''' This implementation iterates over <tt>entrySet()</tt> searching
		''' for an entry with the specified value.  If such an entry is found,
		''' <tt>true</tt> is returned.  If the iteration terminates without
		''' finding such an entry, <tt>false</tt> is returned.  Note that this
		''' implementation requires linear time in the size of the map.
		''' </summary>
		''' <exception cref="ClassCastException">   {@inheritDoc} </exception>
		''' <exception cref="NullPointerException"> {@inheritDoc} </exception>
		Public Overridable Function containsValue(ByVal value As Object) As Boolean Implements Map(Of K, V).containsValue
			Dim i As [Iterator](Of KeyValuePair(Of K, V)) = entrySet().GetEnumerator()
			If value Is Nothing Then
				Do While i.MoveNext()
					Dim e As KeyValuePair(Of K, V) = i.Current
					If e.Value Is Nothing Then Return True
				Loop
			Else
				Do While i.MoveNext()
					Dim e As KeyValuePair(Of K, V) = i.Current
					If value.Equals(e.Value) Then Return True
				Loop
			End If
			Return False
		End Function

		''' <summary>
		''' {@inheritDoc}
		''' 
		''' @implSpec
		''' This implementation iterates over <tt>entrySet()</tt> searching
		''' for an entry with the specified key.  If such an entry is found,
		''' <tt>true</tt> is returned.  If the iteration terminates without
		''' finding such an entry, <tt>false</tt> is returned.  Note that this
		''' implementation requires linear time in the size of the map; many
		''' implementations will override this method.
		''' </summary>
		''' <exception cref="ClassCastException">   {@inheritDoc} </exception>
		''' <exception cref="NullPointerException"> {@inheritDoc} </exception>
		Public Overridable Function containsKey(ByVal key As Object) As Boolean Implements Map(Of K, V).containsKey
			Dim i As [Iterator](Of KeyValuePair(Of K, V)) = entrySet().GetEnumerator()
			If key Is Nothing Then
				Do While i.MoveNext()
					Dim e As KeyValuePair(Of K, V) = i.Current
					If e.Key Is Nothing Then Return True
				Loop
			Else
				Do While i.MoveNext()
					Dim e As KeyValuePair(Of K, V) = i.Current
					If key.Equals(e.Key) Then Return True
				Loop
			End If
			Return False
		End Function

		''' <summary>
		''' {@inheritDoc}
		''' 
		''' @implSpec
		''' This implementation iterates over <tt>entrySet()</tt> searching
		''' for an entry with the specified key.  If such an entry is found,
		''' the entry's value is returned.  If the iteration terminates without
		''' finding such an entry, <tt>null</tt> is returned.  Note that this
		''' implementation requires linear time in the size of the map; many
		''' implementations will override this method.
		''' </summary>
		''' <exception cref="ClassCastException">            {@inheritDoc} </exception>
		''' <exception cref="NullPointerException">          {@inheritDoc} </exception>
		Public Overridable Function [get](ByVal key As Object) As V Implements Map(Of K, V).get
			Dim i As [Iterator](Of KeyValuePair(Of K, V)) = entrySet().GetEnumerator()
			If key Is Nothing Then
				Do While i.MoveNext()
					Dim e As KeyValuePair(Of K, V) = i.Current
					If e.Key Is Nothing Then Return e.Value
				Loop
			Else
				Do While i.MoveNext()
					Dim e As KeyValuePair(Of K, V) = i.Current
					If key.Equals(e.Key) Then Return e.Value
				Loop
			End If
			Return Nothing
		End Function


		' Modification Operations

		''' <summary>
		''' {@inheritDoc}
		''' 
		''' @implSpec
		''' This implementation always throws an
		''' <tt>UnsupportedOperationException</tt>.
		''' </summary>
		''' <exception cref="UnsupportedOperationException"> {@inheritDoc} </exception>
		''' <exception cref="ClassCastException">            {@inheritDoc} </exception>
		''' <exception cref="NullPointerException">          {@inheritDoc} </exception>
		''' <exception cref="IllegalArgumentException">      {@inheritDoc} </exception>
		Public Overridable Function put(ByVal key As K, ByVal value As V) As V Implements Map(Of K, V).put
			Throw New UnsupportedOperationException
		End Function

		''' <summary>
		''' {@inheritDoc}
		''' 
		''' @implSpec
		''' This implementation iterates over <tt>entrySet()</tt> searching for an
		''' entry with the specified key.  If such an entry is found, its value is
		''' obtained with its <tt>getValue</tt> operation, the entry is removed
		''' from the collection (and the backing map) with the iterator's
		''' <tt>remove</tt> operation, and the saved value is returned.  If the
		''' iteration terminates without finding such an entry, <tt>null</tt> is
		''' returned.  Note that this implementation requires linear time in the
		''' size of the map; many implementations will override this method.
		''' 
		''' <p>Note that this implementation throws an
		''' <tt>UnsupportedOperationException</tt> if the <tt>entrySet</tt>
		''' iterator does not support the <tt>remove</tt> method and this map
		''' contains a mapping for the specified key.
		''' </summary>
		''' <exception cref="UnsupportedOperationException"> {@inheritDoc} </exception>
		''' <exception cref="ClassCastException">            {@inheritDoc} </exception>
		''' <exception cref="NullPointerException">          {@inheritDoc} </exception>
		Public Overridable Function remove(ByVal key As Object) As V Implements Map(Of K, V).remove
			Dim i As [Iterator](Of KeyValuePair(Of K, V)) = entrySet().GetEnumerator()
			Dim correctEntry As KeyValuePair(Of K, V) = Nothing
			If key Is Nothing Then
				Do While correctEntry Is Nothing AndAlso i.MoveNext()
					Dim e As KeyValuePair(Of K, V) = i.Current
					If e.Key Is Nothing Then correctEntry = e
				Loop
			Else
				Do While correctEntry Is Nothing AndAlso i.MoveNext()
					Dim e As KeyValuePair(Of K, V) = i.Current
					If key.Equals(e.Key) Then correctEntry = e
				Loop
			End If

			Dim oldValue As V = Nothing
			If correctEntry IsNot Nothing Then
				oldValue = correctEntry.Value
				i.remove()
			End If
			Return oldValue
		End Function


		' Bulk Operations

		''' <summary>
		''' {@inheritDoc}
		''' 
		''' @implSpec
		''' This implementation iterates over the specified map's
		''' <tt>entrySet()</tt> collection, and calls this map's <tt>put</tt>
		''' operation once for each entry returned by the iteration.
		''' 
		''' <p>Note that this implementation throws an
		''' <tt>UnsupportedOperationException</tt> if this map does not support
		''' the <tt>put</tt> operation and the specified map is nonempty.
		''' </summary>
		''' <exception cref="UnsupportedOperationException"> {@inheritDoc} </exception>
		''' <exception cref="ClassCastException">            {@inheritDoc} </exception>
		''' <exception cref="NullPointerException">          {@inheritDoc} </exception>
		''' <exception cref="IllegalArgumentException">      {@inheritDoc} </exception>
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
		Public Overridable Sub putAll(Of T1 As K, ? As V)(ByVal m As Map(Of T1)) Implements Map(Of K, V).putAll
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
			For Each e As KeyValuePair(Of ? As K, ? As V) In m.entrySet()
				put(e.Key, e.Value)
			Next e
		End Sub

		''' <summary>
		''' {@inheritDoc}
		''' 
		''' @implSpec
		''' This implementation calls <tt>entrySet().clear()</tt>.
		''' 
		''' <p>Note that this implementation throws an
		''' <tt>UnsupportedOperationException</tt> if the <tt>entrySet</tt>
		''' does not support the <tt>clear</tt> operation.
		''' </summary>
		''' <exception cref="UnsupportedOperationException"> {@inheritDoc} </exception>
		Public Overridable Sub clear() Implements Map(Of K, V).clear
			entrySet().clear()
		End Sub


		' Views

		''' <summary>
		''' Each of these fields are initialized to contain an instance of the
		''' appropriate view the first time this view is requested.  The views are
		''' stateless, so there's no reason to create more than one of each.
		''' </summary>
'JAVA TO VB CONVERTER TODO TASK: There is no VB equivalent to 'volatile':
		<NonSerialized> _
		Friend keySet_Renamed As [Set](Of K)
'JAVA TO VB CONVERTER TODO TASK: There is no VB equivalent to 'volatile':
		<NonSerialized> _
		Friend values_Renamed As Collection(Of V)

		''' <summary>
		''' {@inheritDoc}
		''' 
		''' @implSpec
		''' This implementation returns a set that subclasses <seealso cref="AbstractSet"/>.
		''' The subclass's iterator method returns a "wrapper object" over this
		''' map's <tt>entrySet()</tt> iterator.  The <tt>size</tt> method
		''' delegates to this map's <tt>size</tt> method and the
		''' <tt>contains</tt> method delegates to this map's
		''' <tt>containsKey</tt> method.
		''' 
		''' <p>The set is created the first time this method is called,
		''' and returned in response to all subsequent calls.  No synchronization
		''' is performed, so there is a slight chance that multiple calls to this
		''' method will not all return the same set.
		''' </summary>
		Public Overridable Function keySet() As [Set](Of K) Implements Map(Of K, V).keySet
			If keySet_Renamed Is Nothing Then keySet_Renamed = New AbstractSetAnonymousInnerClassHelper(Of E)
			Return keySet_Renamed
		End Function

		Private Class AbstractSetAnonymousInnerClassHelper(Of E)
			Inherits AbstractSet(Of E)

			Public Overridable Function [iterator]() As [Iterator](Of K)
				Return New IteratorAnonymousInnerClassHelper(Of E)
			End Function

			Private Class IteratorAnonymousInnerClassHelper(Of E)
				Implements Iterator(Of E)

				Private i As [Iterator](Of KeyValuePair(Of K, V)) = entrySet().GetEnumerator()

				Public Overridable Function hasNext() As Boolean Implements Iterator(Of E).hasNext
					Return i.hasNext()
				End Function

				Public Overridable Function [next]() As K
					Return i.next().key
				End Function

				Public Overridable Sub remove() Implements Iterator(Of E).remove
					i.remove()
				End Sub
			End Class

			Public Overridable Function size() As Integer
				Return outerInstance.size()
			End Function

			Public Overridable Property empty As Boolean
				Get
					Return outerInstance.empty
				End Get
			End Property

			Public Overridable Sub clear()
				outerInstance.clear()
			End Sub

			Public Overridable Function contains(ByVal k As Object) As Boolean
				Return outerInstance.containsKey(k)
			End Function
		End Class

		''' <summary>
		''' {@inheritDoc}
		''' 
		''' @implSpec
		''' This implementation returns a collection that subclasses {@link
		''' AbstractCollection}.  The subclass's iterator method returns a
		''' "wrapper object" over this map's <tt>entrySet()</tt> iterator.
		''' The <tt>size</tt> method delegates to this map's <tt>size</tt>
		''' method and the <tt>contains</tt> method delegates to this map's
		''' <tt>containsValue</tt> method.
		''' 
		''' <p>The collection is created the first time this method is called, and
		''' returned in response to all subsequent calls.  No synchronization is
		''' performed, so there is a slight chance that multiple calls to this
		''' method will not all return the same collection.
		''' </summary>
		Public Overridable Function values() As Collection(Of V) Implements Map(Of K, V).values
			If values_Renamed Is Nothing Then values_Renamed = New AbstractCollectionAnonymousInnerClassHelper(Of E)
			Return values_Renamed
		End Function

		Private Class AbstractCollectionAnonymousInnerClassHelper(Of E)
			Inherits AbstractCollection(Of E)

			Public Overridable Function [iterator]() As [Iterator](Of V)
				Return New IteratorAnonymousInnerClassHelper2(Of E)
			End Function

			Private Class IteratorAnonymousInnerClassHelper2(Of E)
				Implements Iterator(Of E)

				Private i As [Iterator](Of KeyValuePair(Of K, V)) = entrySet().GetEnumerator()

				Public Overridable Function hasNext() As Boolean Implements Iterator(Of E).hasNext
					Return i.hasNext()
				End Function

				Public Overridable Function [next]() As V
					Return i.next().value
				End Function

				Public Overridable Sub remove() Implements Iterator(Of E).remove
					i.remove()
				End Sub
			End Class

			Public Overridable Function size() As Integer
				Return outerInstance.size()
			End Function

			Public Overridable Property empty As Boolean
				Get
					Return outerInstance.empty
				End Get
			End Property

			Public Overridable Sub clear()
				outerInstance.clear()
			End Sub

			Public Overridable Function contains(ByVal v As Object) As Boolean
				Return outerInstance.containsValue(v)
			End Function
		End Class

		Public MustOverride Function entrySet() As [Set](Of KeyValuePair(Of K, V)) Implements Map(Of K, V).entrySet


		' Comparison and hashing

		''' <summary>
		''' Compares the specified object with this map for equality.  Returns
		''' <tt>true</tt> if the given object is also a map and the two maps
		''' represent the same mappings.  More formally, two maps <tt>m1</tt> and
		''' <tt>m2</tt> represent the same mappings if
		''' <tt>m1.entrySet().equals(m2.entrySet())</tt>.  This ensures that the
		''' <tt>equals</tt> method works properly across different implementations
		''' of the <tt>Map</tt> interface.
		''' 
		''' @implSpec
		''' This implementation first checks if the specified object is this map;
		''' if so it returns <tt>true</tt>.  Then, it checks if the specified
		''' object is a map whose size is identical to the size of this map; if
		''' not, it returns <tt>false</tt>.  If so, it iterates over this map's
		''' <tt>entrySet</tt> collection, and checks that the specified map
		''' contains each mapping that this map contains.  If the specified map
		''' fails to contain such a mapping, <tt>false</tt> is returned.  If the
		''' iteration completes, <tt>true</tt> is returned.
		''' </summary>
		''' <param name="o"> object to be compared for equality with this map </param>
		''' <returns> <tt>true</tt> if the specified object is equal to this map </returns>
		Public Overrides Function Equals(ByVal o As Object) As Boolean
			If o Is Me Then Return True

			If Not(TypeOf o Is Map) Then Return False
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
			Dim m As Map(Of ?, ?) = CType(o, Map(Of ?, ?))
			If m.size() <> size() Then Return False

			Try
				Dim i As [Iterator](Of KeyValuePair(Of K, V)) = entrySet().GetEnumerator()
				Do While i.MoveNext()
					Dim e As KeyValuePair(Of K, V) = i.Current
					Dim key As K = e.Key
					Dim value As V = e.Value
					If value Is Nothing Then
						If Not(m.get(key) Is Nothing AndAlso m.containsKey(key)) Then Return False
					Else
						If Not value.Equals(m.get(key)) Then Return False
					End If
				Loop
			Catch unused As  [Class]CastException
				Return False
			Catch unused As NullPointerException
				Return False
			End Try

			Return True
		End Function

		''' <summary>
		''' Returns the hash code value for this map.  The hash code of a map is
		''' defined to be the sum of the hash codes of each entry in the map's
		''' <tt>entrySet()</tt> view.  This ensures that <tt>m1.equals(m2)</tt>
		''' implies that <tt>m1.hashCode()==m2.hashCode()</tt> for any two maps
		''' <tt>m1</tt> and <tt>m2</tt>, as required by the general contract of
		''' <seealso cref="Object#hashCode"/>.
		''' 
		''' @implSpec
		''' This implementation iterates over <tt>entrySet()</tt>, calling
		''' <seealso cref="Map.Entry#hashCode hashCode()"/> on each element (entry) in the
		''' set, and adding up the results.
		''' </summary>
		''' <returns> the hash code value for this map </returns>
		''' <seealso cref= Map.Entry#hashCode() </seealso>
		''' <seealso cref= Object#equals(Object) </seealso>
		''' <seealso cref= Set#equals(Object) </seealso>
		Public Overrides Function GetHashCode() As Integer
			Dim h As Integer = 0
			Dim i As [Iterator](Of KeyValuePair(Of K, V)) = entrySet().GetEnumerator()
			Do While i.hasNext()
				h += i.next().GetHashCode()
			Loop
			Return h
		End Function

		''' <summary>
		''' Returns a string representation of this map.  The string representation
		''' consists of a list of key-value mappings in the order returned by the
		''' map's <tt>entrySet</tt> view's iterator, enclosed in braces
		''' (<tt>"{}"</tt>).  Adjacent mappings are separated by the characters
		''' <tt>", "</tt> (comma and space).  Each key-value mapping is rendered as
		''' the key followed by an equals sign (<tt>"="</tt>) followed by the
		''' associated value.  Keys and values are converted to strings as by
		''' <seealso cref="String#valueOf(Object)"/>.
		''' </summary>
		''' <returns> a string representation of this map </returns>
		Public Overrides Function ToString() As String
			Dim i As [Iterator](Of KeyValuePair(Of K, V)) = entrySet().GetEnumerator()
			If Not i.hasNext() Then Return "{}"

			Dim sb As New StringBuilder
			sb.append("{"c)
			Do
				Dim e As KeyValuePair(Of K, V) = i.next()
				Dim key As K = e.Key
				Dim value As V = e.Value
				sb.append(If(key Is Me, "(this Map)", key))
				sb.append("="c)
				sb.append(If(value Is Me, "(this Map)", value))
				If Not i.hasNext() Then Return sb.append("}"c).ToString()
				sb.append(","c).append(" "c)
			Loop
		End Function

		''' <summary>
		''' Returns a shallow copy of this <tt>AbstractMap</tt> instance: the keys
		''' and values themselves are not cloned.
		''' </summary>
		''' <returns> a shallow copy of this map </returns>
		Protected Friend Overridable Function clone() As Object
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
			Dim result As AbstractMap(Of ?, ?) = CType(MyBase.clone(), AbstractMap(Of ?, ?))
			result.keySet_Renamed = Nothing
			result.values_Renamed = Nothing
			Return result
		End Function

		''' <summary>
		''' Utility method for SimpleEntry and SimpleImmutableEntry.
		''' Test for equality, checking for nulls.
		''' 
		''' NB: Do not replace with Object.equals until JDK-8015417 is resolved.
		''' </summary>
		Private Shared Function eq(ByVal o1 As Object, ByVal o2 As Object) As Boolean
			Return If(o1 Is Nothing, o2 Is Nothing, o1.Equals(o2))
		End Function

		' Implementation Note: SimpleEntry and SimpleImmutableEntry
		' are distinct unrelated classes, even though they share
		' some code. Since you can't add or subtract final-ness
		' of a field in a subclass, they can't share representations,
		' and the amount of duplicated code is too small to warrant
		' exposing a common abstract class.


		''' <summary>
		''' An Entry maintaining a key and a value.  The value may be
		''' changed using the <tt>setValue</tt> method.  This class
		''' facilitates the process of building custom map
		''' implementations. For example, it may be convenient to return
		''' arrays of <tt>SimpleEntry</tt> instances in method
		''' <tt>Map.entrySet().toArray</tt>.
		''' 
		''' @since 1.6
		''' </summary>
		<Serializable> _
		Public Class SimpleEntry(Of K, V)
			Implements KeyValuePair(Of K, V)

			Private Const serialVersionUID As Long = -8499721149061103585L

			Private ReadOnly key As K
			Private value As V

			''' <summary>
			''' Creates an entry representing a mapping from the specified
			''' key to the specified value.
			''' </summary>
			''' <param name="key"> the key represented by this entry </param>
			''' <param name="value"> the value represented by this entry </param>
			Public Sub New(ByVal key As K, ByVal value As V)
				Me.key = key
				Me.value = value
			End Sub

			''' <summary>
			''' Creates an entry representing the same mapping as the
			''' specified entry.
			''' </summary>
			''' <param name="entry"> the entry to copy </param>
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
			Public Sub New(Of T1 As K, ? As V)(ByVal entry As KeyValuePair(Of T1))
				Me.key = entry.Key
				Me.value = entry.Value
			End Sub

			''' <summary>
			''' Returns the key corresponding to this entry.
			''' </summary>
			''' <returns> the key corresponding to this entry </returns>
			Public Overridable Property key As K
				Get
					Return key
				End Get
			End Property

			''' <summary>
			''' Returns the value corresponding to this entry.
			''' </summary>
			''' <returns> the value corresponding to this entry </returns>
			Public Overridable Property value As V
				Get
					Return value
				End Get
			End Property

			''' <summary>
			''' Replaces the value corresponding to this entry with the specified
			''' value.
			''' </summary>
			''' <param name="value"> new value to be stored in this entry </param>
			''' <returns> the old value corresponding to the entry </returns>
			Public Overridable Function setValue(ByVal value As V) As V
				Dim oldValue As V = Me.value
				Me.value = value
				Return oldValue
			End Function

			''' <summary>
			''' Compares the specified object with this entry for equality.
			''' Returns {@code true} if the given object is also a map entry and
			''' the two entries represent the same mapping.  More formally, two
			''' entries {@code e1} and {@code e2} represent the same mapping
			''' if<pre>
			'''   (e1.getKey()==null ?
			'''    e2.getKey()==null :
			'''    e1.getKey().equals(e2.getKey()))
			'''   &amp;&amp;
			'''   (e1.getValue()==null ?
			'''    e2.getValue()==null :
			'''    e1.getValue().equals(e2.getValue()))</pre>
			''' This ensures that the {@code equals} method works properly across
			''' different implementations of the {@code Map.Entry} interface.
			''' </summary>
			''' <param name="o"> object to be compared for equality with this map entry </param>
			''' <returns> {@code true} if the specified object is equal to this map
			'''         entry </returns>
			''' <seealso cref=    #hashCode </seealso>
			Public Overrides Function Equals(ByVal o As Object) As Boolean
				If Not(TypeOf o Is DictionaryEntry) Then Return False
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
				Dim e As KeyValuePair(Of ?, ?) = CType(o, KeyValuePair(Of ?, ?))
				Return eq(key, e.Key) AndAlso eq(value, e.Value)
			End Function

			''' <summary>
			''' Returns the hash code value for this map entry.  The hash code
			''' of a map entry {@code e} is defined to be: <pre>
			'''   (e.getKey()==null   ? 0 : e.getKey().hashCode()) ^
			'''   (e.getValue()==null ? 0 : e.getValue().hashCode())</pre>
			''' This ensures that {@code e1.equals(e2)} implies that
			''' {@code e1.hashCode()==e2.hashCode()} for any two Entries
			''' {@code e1} and {@code e2}, as required by the general
			''' contract of <seealso cref="Object#hashCode"/>.
			''' </summary>
			''' <returns> the hash code value for this map entry </returns>
			''' <seealso cref=    #equals </seealso>
			Public Overrides Function GetHashCode() As Integer
				Return (If(key Is Nothing, 0, key.GetHashCode())) Xor (If(value Is Nothing, 0, value.GetHashCode()))
			End Function

			''' <summary>
			''' Returns a String representation of this map entry.  This
			''' implementation returns the string representation of this
			''' entry's key followed by the equals character ("<tt>=</tt>")
			''' followed by the string representation of this entry's value.
			''' </summary>
			''' <returns> a String representation of this map entry </returns>
			Public Overrides Function ToString() As String
				Return key & "=" & value
			End Function

		End Class

		''' <summary>
		''' An Entry maintaining an immutable key and value.  This class
		''' does not support method <tt>setValue</tt>.  This class may be
		''' convenient in methods that return thread-safe snapshots of
		''' key-value mappings.
		''' 
		''' @since 1.6
		''' </summary>
		<Serializable> _
		Public Class SimpleImmutableEntry(Of K, V)
			Implements KeyValuePair(Of K, V)

			Private Const serialVersionUID As Long = 7138329143949025153L

			Private ReadOnly key As K
			Private ReadOnly value As V

			''' <summary>
			''' Creates an entry representing a mapping from the specified
			''' key to the specified value.
			''' </summary>
			''' <param name="key"> the key represented by this entry </param>
			''' <param name="value"> the value represented by this entry </param>
			Public Sub New(ByVal key As K, ByVal value As V)
				Me.key = key
				Me.value = value
			End Sub

			''' <summary>
			''' Creates an entry representing the same mapping as the
			''' specified entry.
			''' </summary>
			''' <param name="entry"> the entry to copy </param>
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
			Public Sub New(Of T1 As K, ? As V)(ByVal entry As KeyValuePair(Of T1))
				Me.key = entry.Key
				Me.value = entry.Value
			End Sub

			''' <summary>
			''' Returns the key corresponding to this entry.
			''' </summary>
			''' <returns> the key corresponding to this entry </returns>
			Public Overridable Property key As K
				Get
					Return key
				End Get
			End Property

			''' <summary>
			''' Returns the value corresponding to this entry.
			''' </summary>
			''' <returns> the value corresponding to this entry </returns>
			Public Overridable Property value As V
				Get
					Return value
				End Get
			End Property

			''' <summary>
			''' Replaces the value corresponding to this entry with the specified
			''' value (optional operation).  This implementation simply throws
			''' <tt>UnsupportedOperationException</tt>, as this class implements
			''' an <i>immutable</i> map entry.
			''' </summary>
			''' <param name="value"> new value to be stored in this entry </param>
			''' <returns> (Does not return) </returns>
			''' <exception cref="UnsupportedOperationException"> always </exception>
			Public Overridable Function setValue(ByVal value As V) As V
				Throw New UnsupportedOperationException
			End Function

			''' <summary>
			''' Compares the specified object with this entry for equality.
			''' Returns {@code true} if the given object is also a map entry and
			''' the two entries represent the same mapping.  More formally, two
			''' entries {@code e1} and {@code e2} represent the same mapping
			''' if<pre>
			'''   (e1.getKey()==null ?
			'''    e2.getKey()==null :
			'''    e1.getKey().equals(e2.getKey()))
			'''   &amp;&amp;
			'''   (e1.getValue()==null ?
			'''    e2.getValue()==null :
			'''    e1.getValue().equals(e2.getValue()))</pre>
			''' This ensures that the {@code equals} method works properly across
			''' different implementations of the {@code Map.Entry} interface.
			''' </summary>
			''' <param name="o"> object to be compared for equality with this map entry </param>
			''' <returns> {@code true} if the specified object is equal to this map
			'''         entry </returns>
			''' <seealso cref=    #hashCode </seealso>
			Public Overrides Function Equals(ByVal o As Object) As Boolean
				If Not(TypeOf o Is DictionaryEntry) Then Return False
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
				Dim e As KeyValuePair(Of ?, ?) = CType(o, KeyValuePair(Of ?, ?))
				Return eq(key, e.Key) AndAlso eq(value, e.Value)
			End Function

			''' <summary>
			''' Returns the hash code value for this map entry.  The hash code
			''' of a map entry {@code e} is defined to be: <pre>
			'''   (e.getKey()==null   ? 0 : e.getKey().hashCode()) ^
			'''   (e.getValue()==null ? 0 : e.getValue().hashCode())</pre>
			''' This ensures that {@code e1.equals(e2)} implies that
			''' {@code e1.hashCode()==e2.hashCode()} for any two Entries
			''' {@code e1} and {@code e2}, as required by the general
			''' contract of <seealso cref="Object#hashCode"/>.
			''' </summary>
			''' <returns> the hash code value for this map entry </returns>
			''' <seealso cref=    #equals </seealso>
			Public Overrides Function GetHashCode() As Integer
				Return (If(key Is Nothing, 0, key.GetHashCode())) Xor (If(value Is Nothing, 0, value.GetHashCode()))
			End Function

			''' <summary>
			''' Returns a String representation of this map entry.  This
			''' implementation returns the string representation of this
			''' entry's key followed by the equals character ("<tt>=</tt>")
			''' followed by the string representation of this entry's value.
			''' </summary>
			''' <returns> a String representation of this map entry </returns>
			Public Overrides Function ToString() As String
				Return key & "=" & value
			End Function

		End Class

	End Class

End Namespace