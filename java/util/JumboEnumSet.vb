Imports System

'
' * Copyright (c) 2003, 2012, Oracle and/or its affiliates. All rights reserved.
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
	''' Private implementation class for EnumSet, for "jumbo" enum types
	''' (i.e., those with more than 64 elements).
	''' 
	''' @author Josh Bloch
	''' @since 1.5
	''' @serial exclude
	''' </summary>
	Friend Class JumboEnumSet(Of E As System.Enum(Of E))
		Inherits EnumSet(Of E)

		Private Const serialVersionUID As Long = 334349849919042784L

		''' <summary>
		''' Bit vector representation of this set.  The ith bit of the jth
		''' element of this array represents the  presence of universe[64*j +i]
		''' in this set.
		''' </summary>
		Private elements As Long()

		' Redundant - maintained for performance
		Private size_Renamed As Integer = 0

'JAVA TO VB CONVERTER TODO TASK: The following line could not be converted:
		JumboEnumSet(ClasselementType, Enum<?>() universe)
			MyBase(elementType, universe)
			elements = New Long(CInt(CUInt((universe.length + 63)) >> 6) - 1){}

		void addRange(E from, E to)
			Dim fromIndex As Integer = CInt(CUInt(from.ordinal()) >> 6)
			Dim toIndex As Integer = CInt(CUInt(to.ordinal()) >> 6)

			If fromIndex = toIndex Then
				elements(fromIndex) = (-CInt(CUInt(1L) >> (from.ordinal() - to.ordinal() - 1))) << from.ordinal()
			Else
				elements(fromIndex) = (-1L << from.ordinal())
				For i As Integer = fromIndex + 1 To toIndex - 1
					elements(i) = -1
				Next i
				elements(toIndex) = -CInt(CUInt(1L) >> (63 - to.ordinal()))
			End If
			size_Renamed = to.ordinal() - from.ordinal() + 1

		void addAll()
			For i As Integer = 0 To elements.Length - 1
				elements(i) = -1
			Next i
			elements(elements.Length - 1) >>>= -universe.length
			size_Renamed = universe.length

		void complement()
			For i As Integer = 0 To elements.Length - 1
				elements(i) = Not elements(i)
			Next i
			elements(elements.Length - 1) = elements(elements.Length - 1) And (-CInt(CUInt(1L) >> -universe.length))
			size_Renamed = universe.length - size_Renamed

		''' <summary>
		''' Returns an iterator over the elements contained in this set.  The
		''' iterator traverses the elements in their <i>natural order</i> (which is
		''' the order in which the enum constants are declared). The returned
		''' Iterator is a "weakly consistent" iterator that will never throw {@link
		''' ConcurrentModificationException}.
		''' </summary>
		''' <returns> an iterator over the elements contained in this set </returns>
		public Iterator(Of E) [iterator]()
			Return New EnumSetIterator(Me, Of )()

		private class EnumSetIterator(Of E As System.Enum(Of E)) implements Iterator(Of E)
			''' <summary>
			''' A bit vector representing the elements in the current "word"
			''' of the set not yet returned by this iterator.
			''' </summary>
			Dim unseen As Long

			''' <summary>
			''' The index corresponding to unseen in the elements array.
			''' </summary>
			Dim unseenIndex As Integer = 0

			''' <summary>
			''' The bit representing the last element returned by this iterator
			''' but not removed, or zero if no such element exists.
			''' </summary>
			Dim lastReturned As Long = 0

			''' <summary>
			''' The index corresponding to lastReturned in the elements array.
			''' </summary>
			Dim lastReturnedIndex As Integer = 0

			EnumSetIterator()
				unseen = elements(0)

			public Boolean hasNext()
				Do While unseen = 0 AndAlso unseenIndex < elements.Length - 1
					unseenIndex += 1
					unseen = elements(unseenIndex)
				Loop
				Return unseen <> 0

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
			public E next()
				If Not hasNext() Then Throw New NoSuchElementException
				lastReturned = unseen And -unseen
				lastReturnedIndex = unseenIndex
				unseen -= lastReturned
				Return CType(universe((lastReturnedIndex << 6) + java.lang.[Long].numberOfTrailingZeros(lastReturned)), E)

			public void remove()
				If lastReturned = 0 Then Throw New IllegalStateException
				Dim oldElements As Long = elements(lastReturnedIndex)
				elements(lastReturnedIndex) = elements(lastReturnedIndex) And Not lastReturned
				If oldElements <> elements(lastReturnedIndex) Then size_Renamed -= 1
				lastReturned = 0

		''' <summary>
		''' Returns the number of elements in this set.
		''' </summary>
		''' <returns> the number of elements in this set </returns>
		public Integer size()
			Return size_Renamed

		''' <summary>
		''' Returns <tt>true</tt> if this set contains no elements.
		''' </summary>
		''' <returns> <tt>true</tt> if this set contains no elements </returns>
		public Boolean empty
			Return size_Renamed = 0

		''' <summary>
		''' Returns <tt>true</tt> if this set contains the specified element.
		''' </summary>
		''' <param name="e"> element to be checked for containment in this collection </param>
		''' <returns> <tt>true</tt> if this set contains the specified element </returns>
		public Boolean contains(Object e)
			If e Is Nothing Then Return False
			Dim eClass As  [Class] = e.GetType()
			If eClass IsNot elementType AndAlso eClass.BaseType IsNot elementType Then Return False

'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
			Dim eOrdinal As Integer = CType(e, Enum(Of ?)).ordinal()
			Return (elements(CInt(CUInt(eOrdinal) >> 6)) And (1L << eOrdinal)) <> 0

		' Modification Operations

		''' <summary>
		''' Adds the specified element to this set if it is not already present.
		''' </summary>
		''' <param name="e"> element to be added to this set </param>
		''' <returns> <tt>true</tt> if the set changed as a result of the call
		''' </returns>
		''' <exception cref="NullPointerException"> if <tt>e</tt> is null </exception>
		public Boolean add(E e)
			typeCheck(e)

			Dim eOrdinal As Integer = e.ordinal()
			Dim eWordNum As Integer = CInt(CUInt(eOrdinal) >> 6)

			Dim oldElements As Long = elements(eWordNum)
			elements(eWordNum) = elements(eWordNum) Or (1L << eOrdinal)
			Dim result As Boolean = (elements(eWordNum) <> oldElements)
			If result Then size_Renamed += 1
			Return result

		''' <summary>
		''' Removes the specified element from this set if it is present.
		''' </summary>
		''' <param name="e"> element to be removed from this set, if present </param>
		''' <returns> <tt>true</tt> if the set contained the specified element </returns>
		public Boolean remove(Object e)
			If e Is Nothing Then Return False
			Dim eClass As  [Class] = e.GetType()
			If eClass IsNot elementType AndAlso eClass.BaseType IsNot elementType Then Return False
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
			Dim eOrdinal As Integer = CType(e, Enum(Of ?)).ordinal()
			Dim eWordNum As Integer = CInt(CUInt(eOrdinal) >> 6)

			Dim oldElements As Long = elements(eWordNum)
			elements(eWordNum) = elements(eWordNum) And Not(1L << eOrdinal)
			Dim result As Boolean = (elements(eWordNum) <> oldElements)
			If result Then size_Renamed -= 1
			Return result

		' Bulk Operations

		''' <summary>
		''' Returns <tt>true</tt> if this set contains all of the elements
		''' in the specified collection.
		''' </summary>
		''' <param name="c"> collection to be checked for containment in this set </param>
		''' <returns> <tt>true</tt> if this set contains all of the elements
		'''        in the specified collection </returns>
		''' <exception cref="NullPointerException"> if the specified collection is null </exception>
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
		public Boolean containsAll(Collection(Of ?) c)
			If Not(TypeOf c Is JumboEnumSet) Then Return MyBase.containsAll(c)

'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
			Dim es As JumboEnumSet(Of ?) = CType(c, JumboEnumSet(Of ?))
			If es.elementType <> elementType Then Return es.empty

			For i As Integer = 0 To elements.Length - 1
				If (es.elements(i) And (Not elements(i))) <> 0 Then Return False
			Next i
			Return True

		''' <summary>
		''' Adds all of the elements in the specified collection to this set.
		''' </summary>
		''' <param name="c"> collection whose elements are to be added to this set </param>
		''' <returns> <tt>true</tt> if this set changed as a result of the call </returns>
		''' <exception cref="NullPointerException"> if the specified collection or any of
		'''     its elements are null </exception>
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
		public Boolean addAll(Collection(Of ? As E) c)
			If Not(TypeOf c Is JumboEnumSet) Then Return MyBase.addAll(c)

'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
			Dim es As JumboEnumSet(Of ?) = CType(c, JumboEnumSet(Of ?))
			If es.elementType <> elementType Then
				If es.empty Then
					Return False
				Else
					Throw New ClassCastException(es.elementType & " != " & elementType)
				End If
			End If

			For i As Integer = 0 To elements.Length - 1
				elements(i) = elements(i) Or es.elements(i)
			Next i
			Return recalculateSize()

		''' <summary>
		''' Removes from this set all of its elements that are contained in
		''' the specified collection.
		''' </summary>
		''' <param name="c"> elements to be removed from this set </param>
		''' <returns> <tt>true</tt> if this set changed as a result of the call </returns>
		''' <exception cref="NullPointerException"> if the specified collection is null </exception>
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
		public Boolean removeAll(Collection(Of ?) c)
			If Not(TypeOf c Is JumboEnumSet) Then Return MyBase.removeAll(c)

'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
			Dim es As JumboEnumSet(Of ?) = CType(c, JumboEnumSet(Of ?))
			If es.elementType <> elementType Then Return False

			For i As Integer = 0 To elements.Length - 1
				elements(i) = elements(i) And Not es.elements(i)
			Next i
			Return recalculateSize()

		''' <summary>
		''' Retains only the elements in this set that are contained in the
		''' specified collection.
		''' </summary>
		''' <param name="c"> elements to be retained in this set </param>
		''' <returns> <tt>true</tt> if this set changed as a result of the call </returns>
		''' <exception cref="NullPointerException"> if the specified collection is null </exception>
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
		public Boolean retainAll(Collection(Of ?) c)
			If Not(TypeOf c Is JumboEnumSet) Then Return MyBase.retainAll(c)

'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
			Dim es As JumboEnumSet(Of ?) = CType(c, JumboEnumSet(Of ?))
			If es.elementType <> elementType Then
				Dim changed As Boolean = (size_Renamed <> 0)
				clear()
				Return changed
			End If

			For i As Integer = 0 To elements.Length - 1
				elements(i) = elements(i) And es.elements(i)
			Next i
			Return recalculateSize()

		''' <summary>
		''' Removes all of the elements from this set.
		''' </summary>
		public void clear()
			Arrays.fill(elements, 0)
			size_Renamed = 0

		''' <summary>
		''' Compares the specified object with this set for equality.  Returns
		''' <tt>true</tt> if the given object is also a set, the two sets have
		''' the same size, and every member of the given set is contained in
		''' this set.
		''' </summary>
		''' <param name="o"> object to be compared for equality with this set </param>
		''' <returns> <tt>true</tt> if the specified object is equal to this set </returns>
		public Boolean Equals(Object o)
			If Not(TypeOf o Is JumboEnumSet) Then Return MyBase.Equals(o)

'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
			Dim es As JumboEnumSet(Of ?) = CType(o, JumboEnumSet(Of ?))
			If es.elementType <> elementType Then Return size_Renamed = 0 AndAlso es.size_Renamed = 0

			Return Array.Equals(es.elements, elements)

		''' <summary>
		''' Recalculates the size of the set.  Returns true if it's changed.
		''' </summary>
		private Boolean recalculateSize()
			Dim oldSize As Integer = size_Renamed
			size_Renamed = 0
			For Each elt As Long In elements
				size_Renamed += java.lang.[Long].bitCount(elt)
			Next elt

			Return size_Renamed <> oldSize

		public EnumSet(Of E) clone()
			Dim result As JumboEnumSet(Of E) = CType(MyBase.clone(), JumboEnumSet(Of E))
			result.elements = result.elements.clone()
			Return result
	End Class

End Namespace