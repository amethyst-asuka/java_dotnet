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
	''' Private implementation class for EnumSet, for "regular sized" enum types
	''' (i.e., those with 64 or fewer enum constants).
	''' 
	''' @author Josh Bloch
	''' @since 1.5
	''' @serial exclude
	''' </summary>
	Friend Class RegularEnumSet(Of E As System.Enum(Of E))
		Inherits EnumSet(Of E)

		Private Const serialVersionUID As Long = 3411599620347842686L
		''' <summary>
		''' Bit vector representation of this set.  The 2^k bit indicates the
		''' presence of universe[k] in this set.
		''' </summary>
		Private elements As Long = 0L

'JAVA TO VB CONVERTER TODO TASK: The following line could not be converted:
		RegularEnumSet(ClasselementType, Enum<?>() universe)
			MyBase(elementType, universe)

		void addRange(E from, E to)
			elements = (-CInt(CUInt(1L) >> (from.ordinal() - to.ordinal() - 1))) << from.ordinal()

		void addAll()
			If universe.length <> 0 Then elements = -CInt(CUInt(1L) >> -universe.length)

		void complement()
			If universe.length <> 0 Then
				elements = Not elements
				elements = elements And -CInt(CUInt(1L) >> -universe.length) ' Mask unused bits
			End If

		''' <summary>
		''' Returns an iterator over the elements contained in this set.  The
		''' iterator traverses the elements in their <i>natural order</i> (which is
		''' the order in which the enum constants are declared). The returned
		''' Iterator is a "snapshot" iterator that will never throw {@link
		''' ConcurrentModificationException}; the elements are traversed as they
		''' existed when this call was invoked.
		''' </summary>
		''' <returns> an iterator over the elements contained in this set </returns>
		public Iterator(Of E) [iterator]()
			Return New EnumSetIterator(Me, Of )()

		private class EnumSetIterator(Of E As System.Enum(Of E)) implements Iterator(Of E)
			''' <summary>
			''' A bit vector representing the elements in the set not yet
			''' returned by this iterator.
			''' </summary>
			Dim unseen As Long

			''' <summary>
			''' The bit representing the last element returned by this iterator
			''' but not removed, or zero if no such element exists.
			''' </summary>
			Dim lastReturned As Long = 0

			EnumSetIterator()
				unseen = elements

			public Boolean hasNext()
				Return unseen <> 0

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
			public E next()
				If unseen = 0 Then Throw New NoSuchElementException
				lastReturned = unseen And -unseen
				unseen -= lastReturned
				Return CType(universe(Long.numberOfTrailingZeros(lastReturned)), E)

			public void remove()
				If lastReturned = 0 Then Throw New IllegalStateException
				elements = elements And Not lastReturned
				lastReturned = 0

		''' <summary>
		''' Returns the number of elements in this set.
		''' </summary>
		''' <returns> the number of elements in this set </returns>
		public Integer size()
			Return java.lang.[Long].bitCount(elements)

		''' <summary>
		''' Returns <tt>true</tt> if this set contains no elements.
		''' </summary>
		''' <returns> <tt>true</tt> if this set contains no elements </returns>
		public Boolean empty
			Return elements = 0

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
			Return (elements And (1L << CType(e, Enum(Of ?)).ordinal())) <> 0

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

			Dim oldElements As Long = elements
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
			elements = elements Or (1L << CType(e, Enum(Of ?)).ordinal())
			Return elements <> oldElements

		''' <summary>
		''' Removes the specified element from this set if it is present.
		''' </summary>
		''' <param name="e"> element to be removed from this set, if present </param>
		''' <returns> <tt>true</tt> if the set contained the specified element </returns>
		public Boolean remove(Object e)
			If e Is Nothing Then Return False
			Dim eClass As  [Class] = e.GetType()
			If eClass IsNot elementType AndAlso eClass.BaseType IsNot elementType Then Return False

			Dim oldElements As Long = elements
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
			elements = elements And Not(1L << CType(e, Enum(Of ?)).ordinal())
			Return elements <> oldElements

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
			If Not(TypeOf c Is RegularEnumSet) Then Return MyBase.containsAll(c)

'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
			Dim es As RegularEnumSet(Of ?) = CType(c, RegularEnumSet(Of ?))
			If es.elementType <> elementType Then Return es.empty

			Return (es.elements And (Not elements)) = 0

		''' <summary>
		''' Adds all of the elements in the specified collection to this set.
		''' </summary>
		''' <param name="c"> collection whose elements are to be added to this set </param>
		''' <returns> <tt>true</tt> if this set changed as a result of the call </returns>
		''' <exception cref="NullPointerException"> if the specified collection or any
		'''     of its elements are null </exception>
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
		public Boolean addAll(Collection(Of ? As E) c)
			If Not(TypeOf c Is RegularEnumSet) Then Return MyBase.addAll(c)

'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
			Dim es As RegularEnumSet(Of ?) = CType(c, RegularEnumSet(Of ?))
			If es.elementType <> elementType Then
				If es.empty Then
					Return False
				Else
					Throw New ClassCastException(es.elementType & " != " & elementType)
				End If
			End If

			Dim oldElements As Long = elements
			elements = elements Or es.elements
			Return elements <> oldElements

		''' <summary>
		''' Removes from this set all of its elements that are contained in
		''' the specified collection.
		''' </summary>
		''' <param name="c"> elements to be removed from this set </param>
		''' <returns> <tt>true</tt> if this set changed as a result of the call </returns>
		''' <exception cref="NullPointerException"> if the specified collection is null </exception>
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
		public Boolean removeAll(Collection(Of ?) c)
			If Not(TypeOf c Is RegularEnumSet) Then Return MyBase.removeAll(c)

'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
			Dim es As RegularEnumSet(Of ?) = CType(c, RegularEnumSet(Of ?))
			If es.elementType <> elementType Then Return False

			Dim oldElements As Long = elements
			elements = elements And Not es.elements
			Return elements <> oldElements

		''' <summary>
		''' Retains only the elements in this set that are contained in the
		''' specified collection.
		''' </summary>
		''' <param name="c"> elements to be retained in this set </param>
		''' <returns> <tt>true</tt> if this set changed as a result of the call </returns>
		''' <exception cref="NullPointerException"> if the specified collection is null </exception>
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
		public Boolean retainAll(Collection(Of ?) c)
			If Not(TypeOf c Is RegularEnumSet) Then Return MyBase.retainAll(c)

'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
			Dim es As RegularEnumSet(Of ?) = CType(c, RegularEnumSet(Of ?))
			If es.elementType <> elementType Then
				Dim changed As Boolean = (elements <> 0)
				elements = 0
				Return changed
			End If

			Dim oldElements As Long = elements
			elements = elements And es.elements
			Return elements <> oldElements

		''' <summary>
		''' Removes all of the elements from this set.
		''' </summary>
		public void clear()
			elements = 0

		''' <summary>
		''' Compares the specified object with this set for equality.  Returns
		''' <tt>true</tt> if the given object is also a set, the two sets have
		''' the same size, and every member of the given set is contained in
		''' this set.
		''' </summary>
		''' <param name="o"> object to be compared for equality with this set </param>
		''' <returns> <tt>true</tt> if the specified object is equal to this set </returns>
		public Boolean Equals(Object o)
			If Not(TypeOf o Is RegularEnumSet) Then Return MyBase.Equals(o)

'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
			Dim es As RegularEnumSet(Of ?) = CType(o, RegularEnumSet(Of ?))
			If es.elementType <> elementType Then Return elements = 0 AndAlso es.elements = 0
			Return es.elements = elements
	End Class

End Namespace