Imports System.Collections.Generic

'
' * Copyright (c) 2005, Oracle and/or its affiliates. All rights reserved.
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

Namespace javax.script


	''' <summary>
	''' A simple implementation of Bindings backed by
	''' a <code>HashMap</code> or some other specified <code>Map</code>.
	''' 
	''' @author Mike Grogan
	''' @since 1.6
	''' </summary>
	Public Class SimpleBindings
		Implements Bindings

		''' <summary>
		''' The <code>Map</code> field stores the attributes.
		''' </summary>
		Private map As IDictionary(Of String, Object)

		''' <summary>
		''' Constructor uses an existing <code>Map</code> to store the values. </summary>
		''' <param name="m"> The <code>Map</code> backing this <code>SimpleBindings</code>. </param>
		''' <exception cref="NullPointerException"> if m is null </exception>
		Public Sub New(ByVal m As IDictionary(Of String, Object))
			If m Is Nothing Then Throw New NullPointerException
			Me.map = m
		End Sub

		''' <summary>
		''' Default constructor uses a <code>HashMap</code>.
		''' </summary>
		Public Sub New()
			Me.New(New Dictionary(Of String, Object))
		End Sub

		''' <summary>
		''' Sets the specified key/value in the underlying <code>map</code> field.
		''' </summary>
		''' <param name="name"> Name of value </param>
		''' <param name="value"> Value to set.
		''' </param>
		''' <returns> Previous value for the specified key.  Returns null if key was previously
		''' unset.
		''' </returns>
		''' <exception cref="NullPointerException"> if the name is null. </exception>
		''' <exception cref="IllegalArgumentException"> if the name is empty. </exception>
		Public Overridable Function put(ByVal name As String, ByVal value As Object) As Object Implements Bindings.put
			checkKey(name)
				map(name) = value
				Return map(name)
		End Function

		''' <summary>
		''' <code>putAll</code> is implemented using <code>Map.putAll</code>.
		''' </summary>
		''' <param name="toMerge"> The <code>Map</code> of values to add.
		''' </param>
		''' <exception cref="NullPointerException">
		'''         if toMerge map is null or if some key in the map is null. </exception>
		''' <exception cref="IllegalArgumentException">
		'''         if some key in the map is an empty String. </exception>
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
		Public Overridable Sub putAll(Of T1 As String, ? As Object)(ByVal toMerge As IDictionary(Of T1)) Implements Bindings.putAll
			If toMerge Is Nothing Then Throw New NullPointerException("toMerge map is null")
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
			For Each entry As KeyValuePair(Of ? As String, ? As Object) In toMerge
				Dim key As String = entry.Key
				checkKey(key)
				put(key, entry.Value)
			Next entry
		End Sub

		''' <summary>
		''' {@inheritDoc} </summary>
		Public Overridable Sub clear()
			map.Clear()
		End Sub

		''' <summary>
		''' Returns <tt>true</tt> if this map contains a mapping for the specified
		''' key.  More formally, returns <tt>true</tt> if and only if
		''' this map contains a mapping for a key <tt>k</tt> such that
		''' <tt>(key==null ? k==null : key.equals(k))</tt>.  (There can be
		''' at most one such mapping.)
		''' </summary>
		''' <param name="key"> key whose presence in this map is to be tested. </param>
		''' <returns> <tt>true</tt> if this map contains a mapping for the specified
		'''         key.
		''' </returns>
		''' <exception cref="NullPointerException"> if key is null </exception>
		''' <exception cref="ClassCastException"> if key is not String </exception>
		''' <exception cref="IllegalArgumentException"> if key is empty String </exception>
		Public Overridable Function containsKey(ByVal key As Object) As Boolean Implements Bindings.containsKey
			checkKey(key)
			Return map.ContainsKey(key)
		End Function

		''' <summary>
		''' {@inheritDoc} </summary>
		Public Overridable Function containsValue(ByVal value As Object) As Boolean
			Return map.ContainsValue(value)
		End Function

		''' <summary>
		''' {@inheritDoc} </summary>
		Public Overridable Function entrySet() As java.util.Set(Of KeyValuePair(Of String, Object))
'JAVA TO VB CONVERTER TODO TASK: There is no .NET Dictionary equivalent to the Java 'entrySet' method:
			Return map.entrySet()
		End Function

		''' <summary>
		''' Returns the value to which this map maps the specified key.  Returns
		''' <tt>null</tt> if the map contains no mapping for this key.  A return
		''' value of <tt>null</tt> does not <i>necessarily</i> indicate that the
		''' map contains no mapping for the key; it's also possible that the map
		''' explicitly maps the key to <tt>null</tt>.  The <tt>containsKey</tt>
		''' operation may be used to distinguish these two cases.
		''' 
		''' <p>More formally, if this map contains a mapping from a key
		''' <tt>k</tt> to a value <tt>v</tt> such that <tt>(key==null ? k==null :
		''' key.equals(k))</tt>, then this method returns <tt>v</tt>; otherwise
		''' it returns <tt>null</tt>.  (There can be at most one such mapping.)
		''' </summary>
		''' <param name="key"> key whose associated value is to be returned. </param>
		''' <returns> the value to which this map maps the specified key, or
		'''         <tt>null</tt> if the map contains no mapping for this key.
		''' </returns>
		''' <exception cref="NullPointerException"> if key is null </exception>
		''' <exception cref="ClassCastException"> if key is not String </exception>
		''' <exception cref="IllegalArgumentException"> if key is empty String </exception>
		Public Overridable Function [get](ByVal key As Object) As Object Implements Bindings.get
			checkKey(key)
			Return map(key)
		End Function

		''' <summary>
		''' {@inheritDoc} </summary>
		Public Overridable Property empty As Boolean
			Get
				Return map.Count = 0
			End Get
		End Property

		''' <summary>
		''' {@inheritDoc} </summary>
		Public Overridable Function keySet() As java.util.Set(Of String)
			Return map.Keys
		End Function

		''' <summary>
		''' Removes the mapping for this key from this map if it is present
		''' (optional operation).   More formally, if this map contains a mapping
		''' from key <tt>k</tt> to value <tt>v</tt> such that
		''' <code>(key==null ?  k==null : key.equals(k))</code>, that mapping
		''' is removed.  (The map can contain at most one such mapping.)
		''' 
		''' <p>Returns the value to which the map previously associated the key, or
		''' <tt>null</tt> if the map contained no mapping for this key.  (A
		''' <tt>null</tt> return can also indicate that the map previously
		''' associated <tt>null</tt> with the specified key if the implementation
		''' supports <tt>null</tt> values.)  The map will not contain a mapping for
		''' the specified  key once the call returns.
		''' </summary>
		''' <param name="key"> key whose mapping is to be removed from the map. </param>
		''' <returns> previous value associated with specified key, or <tt>null</tt>
		'''         if there was no mapping for key.
		''' </returns>
		''' <exception cref="NullPointerException"> if key is null </exception>
		''' <exception cref="ClassCastException"> if key is not String </exception>
		''' <exception cref="IllegalArgumentException"> if key is empty String </exception>
		Public Overridable Function remove(ByVal key As Object) As Object Implements Bindings.remove
			checkKey(key)
			Return map.Remove(key)
		End Function

		''' <summary>
		''' {@inheritDoc} </summary>
		Public Overridable Function size() As Integer
			Return map.Count
		End Function

		''' <summary>
		''' {@inheritDoc} </summary>
		Public Overridable Function values() As ICollection(Of Object)
			Return map.Values
		End Function

		Private Sub checkKey(ByVal key As Object)
			If key Is Nothing Then Throw New NullPointerException("key can not be null")
			If Not(TypeOf key Is String) Then Throw New ClassCastException("key should be a String")
			If key.Equals("") Then Throw New System.ArgumentException("key can not be empty")
		End Sub
	End Class

End Namespace