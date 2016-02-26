Imports System.Collections.Generic

'
' * Copyright (c) 2005, 2006, Oracle and/or its affiliates. All rights reserved.
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
	''' A mapping of key/value pairs, all of whose keys are
	''' <code>Strings</code>.
	''' 
	''' @author Mike Grogan
	''' @since 1.6
	''' </summary>
	Public Interface Bindings
		Inherits IDictionary(Of String, Object)

		''' <summary>
		''' Set a named value.
		''' </summary>
		''' <param name="name"> The name associated with the value. </param>
		''' <param name="value"> The value associated with the name.
		''' </param>
		''' <returns> The value previously associated with the given name.
		''' Returns null if no value was previously associated with the name.
		''' </returns>
		''' <exception cref="NullPointerException"> if the name is null. </exception>
		''' <exception cref="IllegalArgumentException"> if the name is empty String. </exception>
		Function put(ByVal name As String, ByVal value As Object) As Object

		''' <summary>
		''' Adds all the mappings in a given <code>Map</code> to this <code>Bindings</code>. </summary>
		''' <param name="toMerge"> The <code>Map</code> to merge with this one.
		''' </param>
		''' <exception cref="NullPointerException">
		'''         if toMerge map is null or if some key in the map is null. </exception>
		''' <exception cref="IllegalArgumentException">
		'''         if some key in the map is an empty String. </exception>
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
		Sub putAll(Of T1 As String, ? As Object)(ByVal toMerge As IDictionary(Of T1))

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
		Function containsKey(ByVal key As Object) As Boolean

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
		Function [get](ByVal key As Object) As Object

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
		Function remove(ByVal key As Object) As Object
	End Interface

End Namespace