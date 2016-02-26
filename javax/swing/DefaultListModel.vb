Imports System.Collections.Generic
Imports javax.swing.event

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

Namespace javax.swing



	''' <summary>
	''' This class loosely implements the <code>java.util.Vector</code>
	''' API, in that it implements the 1.1.x version of
	''' <code>java.util.Vector</code>, has no collection class support,
	''' and notifies the <code>ListDataListener</code>s when changes occur.
	''' Presently it delegates to a <code>Vector</code>,
	''' in a future release it will be a real Collection implementation.
	''' <p>
	''' <strong>Warning:</strong>
	''' Serialized objects of this class will not be compatible with
	''' future Swing releases. The current serialization support is
	''' appropriate for short term storage or RMI between applications running
	''' the same version of Swing.  As of 1.4, support for long term storage
	''' of all JavaBeans&trade;
	''' has been added to the <code>java.beans</code> package.
	''' Please see <seealso cref="java.beans.XMLEncoder"/>.
	''' </summary>
	''' @param <E> the type of the elements of this model
	''' 
	''' @author Hans Muller </param>
	Public Class DefaultListModel(Of E)
		Inherits AbstractListModel(Of E)

		Private [delegate] As New List(Of E)

		''' <summary>
		''' Returns the number of components in this list.
		''' <p>
		''' This method is identical to <code>size</code>, which implements the
		''' <code>List</code> interface defined in the 1.2 Collections framework.
		''' This method exists in conjunction with <code>setSize</code> so that
		''' <code>size</code> is identifiable as a JavaBean property.
		''' </summary>
		''' <returns>  the number of components in this list </returns>
		''' <seealso cref= #size() </seealso>
'JAVA TO VB CONVERTER TODO TASK: The following line could not be converted:
        Public Overridable Function getSize() As Integer 'JavaToDotNetTempPropertyGetsize
		Public Overridable Property size As Integer
			Get
				Return [delegate].Count
			End Get
			Set(ByVal newSize As Integer)
		End Property

		''' <summary>
		''' Returns the component at the specified index.
		''' <blockquote>
		''' <b>Note:</b> Although this method is not deprecated, the preferred
		'''    method to use is <code>get(int)</code>, which implements the
		'''    <code>List</code> interface defined in the 1.2 Collections framework.
		''' </blockquote> </summary>
		''' <param name="index">   an index into this list </param>
		''' <returns>     the component at the specified index </returns>
		''' <exception cref="ArrayIndexOutOfBoundsException">  if the <code>index</code>
		'''             is negative or greater than the current size of this
		'''             list </exception>
		''' <seealso cref= #get(int) </seealso>
		Public Overridable Function getElementAt(ByVal index As Integer) As E
			Return [delegate](index)
		End Function

		''' <summary>
		''' Copies the components of this list into the specified array.
		''' The array must be big enough to hold all the objects in this list,
		''' else an <code>IndexOutOfBoundsException</code> is thrown.
		''' </summary>
		''' <param name="anArray">   the array into which the components get copied </param>
		''' <seealso cref= Vector#copyInto(Object[]) </seealso>
		Public Overridable Sub copyInto(ByVal anArray As Object())
			[delegate].CopyTo(anArray)
		End Sub

		''' <summary>
		''' Trims the capacity of this list to be the list's current size.
		''' </summary>
		''' <seealso cref= Vector#trimToSize() </seealso>
		Public Overridable Sub trimToSize()
			[delegate].TrimExcess()
		End Sub

		''' <summary>
		''' Increases the capacity of this list, if necessary, to ensure
		''' that it can hold at least the number of components specified by
		''' the minimum capacity argument.
		''' </summary>
		''' <param name="minCapacity">   the desired minimum capacity </param>
		''' <seealso cref= Vector#ensureCapacity(int) </seealso>
		Public Overridable Sub ensureCapacity(ByVal minCapacity As Integer)
			[delegate].Capacity = minCapacity
		End Sub

			Dim oldSize As Integer = [delegate].Count
			[delegate].Capacity = newSize
			If oldSize > newSize Then
				fireIntervalRemoved(Me, newSize, oldSize-1)
			ElseIf oldSize < newSize Then
				fireIntervalAdded(Me, oldSize, newSize-1)
			End If
		End Sub

		''' <summary>
		''' Returns the current capacity of this list.
		''' </summary>
		''' <returns>  the current capacity </returns>
		''' <seealso cref= Vector#capacity() </seealso>
		Public Overridable Function capacity() As Integer
			Return [delegate].capacity()
		End Function

		''' <summary>
		''' Returns the number of components in this list.
		''' </summary>
		''' <returns>  the number of components in this list </returns>
		''' <seealso cref= Vector#size() </seealso>
		Public Overridable Function size() As Integer
			Return [delegate].Count
		End Function

		''' <summary>
		''' Tests whether this list has any components.
		''' </summary>
		''' <returns>  <code>true</code> if and only if this list has
		'''          no components, that is, its size is zero;
		'''          <code>false</code> otherwise </returns>
		''' <seealso cref= Vector#isEmpty() </seealso>
		Public Overridable Property empty As Boolean
			Get
				Return [delegate].Count = 0
			End Get
		End Property

		''' <summary>
		''' Returns an enumeration of the components of this list.
		''' </summary>
		''' <returns>  an enumeration of the components of this list </returns>
		''' <seealso cref= Vector#elements() </seealso>
		Public Overridable Function elements() As System.Collections.IEnumerator(Of E)
			Return [delegate].elements()
		End Function

		''' <summary>
		''' Tests whether the specified object is a component in this list.
		''' </summary>
		''' <param name="elem">   an object </param>
		''' <returns>  <code>true</code> if the specified object
		'''          is the same as a component in this list </returns>
		''' <seealso cref= Vector#contains(Object) </seealso>
		Public Overridable Function contains(ByVal elem As Object) As Boolean
			Return [delegate].Contains(elem)
		End Function

		''' <summary>
		''' Searches for the first occurrence of <code>elem</code>.
		''' </summary>
		''' <param name="elem">   an object </param>
		''' <returns>  the index of the first occurrence of the argument in this
		'''          list; returns <code>-1</code> if the object is not found </returns>
		''' <seealso cref= Vector#indexOf(Object) </seealso>
		Public Overridable Function indexOf(ByVal elem As Object) As Integer
			Return [delegate].IndexOf(elem)
		End Function

		''' <summary>
		''' Searches for the first occurrence of <code>elem</code>, beginning
		''' the search at <code>index</code>.
		''' </summary>
		''' <param name="elem">    an desired component </param>
		''' <param name="index">   the index from which to begin searching </param>
		''' <returns>  the index where the first occurrence of <code>elem</code>
		'''          is found after <code>index</code>; returns <code>-1</code>
		'''          if the <code>elem</code> is not found in the list </returns>
		''' <seealso cref= Vector#indexOf(Object,int) </seealso>
		 Public Overridable Function indexOf(ByVal elem As Object, ByVal index As Integer) As Integer
			Return [delegate].IndexOf(elem, index)
		 End Function

		''' <summary>
		''' Returns the index of the last occurrence of <code>elem</code>.
		''' </summary>
		''' <param name="elem">   the desired component </param>
		''' <returns>  the index of the last occurrence of <code>elem</code>
		'''          in the list; returns <code>-1</code> if the object is not found </returns>
		''' <seealso cref= Vector#lastIndexOf(Object) </seealso>
		Public Overridable Function lastIndexOf(ByVal elem As Object) As Integer
			Return [delegate].LastIndexOf(elem)
		End Function

		''' <summary>
		''' Searches backwards for <code>elem</code>, starting from the
		''' specified index, and returns an index to it.
		''' </summary>
		''' <param name="elem">    the desired component </param>
		''' <param name="index">   the index to start searching from </param>
		''' <returns> the index of the last occurrence of the <code>elem</code>
		'''          in this list at position less than <code>index</code>;
		'''          returns <code>-1</code> if the object is not found </returns>
		''' <seealso cref= Vector#lastIndexOf(Object,int) </seealso>
		Public Overridable Function lastIndexOf(ByVal elem As Object, ByVal index As Integer) As Integer
			Return [delegate].LastIndexOf(elem, index)
		End Function

		''' <summary>
		''' Returns the component at the specified index.
		''' Throws an <code>ArrayIndexOutOfBoundsException</code> if the index
		''' is negative or not less than the size of the list.
		''' <blockquote>
		''' <b>Note:</b> Although this method is not deprecated, the preferred
		'''    method to use is <code>get(int)</code>, which implements the
		'''    <code>List</code> interface defined in the 1.2 Collections framework.
		''' </blockquote>
		''' </summary>
		''' <param name="index">   an index into this list </param>
		''' <returns>     the component at the specified index </returns>
		''' <seealso cref= #get(int) </seealso>
		''' <seealso cref= Vector#elementAt(int) </seealso>
		Public Overridable Function elementAt(ByVal index As Integer) As E
			Return [delegate](index)
		End Function

		''' <summary>
		''' Returns the first component of this list.
		''' Throws a <code>NoSuchElementException</code> if this
		''' vector has no components. </summary>
		''' <returns>     the first component of this list </returns>
		''' <seealso cref= Vector#firstElement() </seealso>
		Public Overridable Function firstElement() As E
			Return [delegate](0)
		End Function

		''' <summary>
		''' Returns the last component of the list.
		''' Throws a <code>NoSuchElementException</code> if this vector
		''' has no components.
		''' </summary>
		''' <returns>  the last component of the list </returns>
		''' <seealso cref= Vector#lastElement() </seealso>
		Public Overridable Function lastElement() As E
			Return [delegate]([delegate].Count - 1)
		End Function

		''' <summary>
		''' Sets the component at the specified <code>index</code> of this
		''' list to be the specified element. The previous component at that
		''' position is discarded.
		''' <p>
		''' Throws an <code>ArrayIndexOutOfBoundsException</code> if the index
		''' is invalid.
		''' <blockquote>
		''' <b>Note:</b> Although this method is not deprecated, the preferred
		'''    method to use is <code>set(int,Object)</code>, which implements the
		'''    <code>List</code> interface defined in the 1.2 Collections framework.
		''' </blockquote>
		''' </summary>
		''' <param name="element"> what the component is to be set to </param>
		''' <param name="index">   the specified index </param>
		''' <seealso cref= #set(int,Object) </seealso>
		''' <seealso cref= Vector#setElementAt(Object,int) </seealso>
		Public Overridable Sub setElementAt(ByVal element As E, ByVal index As Integer)
			[delegate](index) = element
			fireContentsChanged(Me, index, index)
		End Sub

		''' <summary>
		''' Deletes the component at the specified index.
		''' <p>
		''' Throws an <code>ArrayIndexOutOfBoundsException</code> if the index
		''' is invalid.
		''' <blockquote>
		''' <b>Note:</b> Although this method is not deprecated, the preferred
		'''    method to use is <code>remove(int)</code>, which implements the
		'''    <code>List</code> interface defined in the 1.2 Collections framework.
		''' </blockquote>
		''' </summary>
		''' <param name="index">   the index of the object to remove </param>
		''' <seealso cref= #remove(int) </seealso>
		''' <seealso cref= Vector#removeElementAt(int) </seealso>
		Public Overridable Sub removeElementAt(ByVal index As Integer)
			[delegate].RemoveAt(index)
			fireIntervalRemoved(Me, index, index)
		End Sub

		''' <summary>
		''' Inserts the specified element as a component in this list at the
		''' specified <code>index</code>.
		''' <p>
		''' Throws an <code>ArrayIndexOutOfBoundsException</code> if the index
		''' is invalid.
		''' <blockquote>
		''' <b>Note:</b> Although this method is not deprecated, the preferred
		'''    method to use is <code>add(int,Object)</code>, which implements the
		'''    <code>List</code> interface defined in the 1.2 Collections framework.
		''' </blockquote>
		''' </summary>
		''' <param name="element"> the component to insert </param>
		''' <param name="index">   where to insert the new component </param>
		''' <exception cref="ArrayIndexOutOfBoundsException">  if the index was invalid </exception>
		''' <seealso cref= #add(int,Object) </seealso>
		''' <seealso cref= Vector#insertElementAt(Object,int) </seealso>
		Public Overridable Sub insertElementAt(ByVal element As E, ByVal index As Integer)
			[delegate].Insert(index, element)
			fireIntervalAdded(Me, index, index)
		End Sub

		''' <summary>
		''' Adds the specified component to the end of this list.
		''' </summary>
		''' <param name="element">   the component to be added </param>
		''' <seealso cref= Vector#addElement(Object) </seealso>
		Public Overridable Sub addElement(ByVal element As E)
			Dim index As Integer = [delegate].Count
			[delegate].Add(element)
			fireIntervalAdded(Me, index, index)
		End Sub

		''' <summary>
		''' Removes the first (lowest-indexed) occurrence of the argument
		''' from this list.
		''' </summary>
		''' <param name="obj">   the component to be removed </param>
		''' <returns>  <code>true</code> if the argument was a component of this
		'''          list; <code>false</code> otherwise </returns>
		''' <seealso cref= Vector#removeElement(Object) </seealso>
		Public Overridable Function removeElement(ByVal obj As Object) As Boolean
			Dim index As Integer = IndexOf(obj)
			Dim rv As Boolean = [delegate].Remove(obj)
			If index >= 0 Then fireIntervalRemoved(Me, index, index)
			Return rv
		End Function


		''' <summary>
		''' Removes all components from this list and sets its size to zero.
		''' <blockquote>
		''' <b>Note:</b> Although this method is not deprecated, the preferred
		'''    method to use is <code>clear</code>, which implements the
		'''    <code>List</code> interface defined in the 1.2 Collections framework.
		''' </blockquote>
		''' </summary>
		''' <seealso cref= #clear() </seealso>
		''' <seealso cref= Vector#removeAllElements() </seealso>
		Public Overridable Sub removeAllElements()
			Dim index1 As Integer = [delegate].Count-1
			[delegate].Clear()
			If index1 >= 0 Then fireIntervalRemoved(Me, 0, index1)
		End Sub


		''' <summary>
		''' Returns a string that displays and identifies this
		''' object's properties.
		''' </summary>
		''' <returns> a String representation of this object </returns>
	   Public Overrides Function ToString() As String
			Return [delegate].ToString()
	   End Function


	'     The remaining methods are included for compatibility with the
	'     * Java 2 platform Vector class.
	'     

		''' <summary>
		''' Returns an array containing all of the elements in this list in the
		''' correct order.
		''' </summary>
		''' <returns> an array containing the elements of the list </returns>
		''' <seealso cref= Vector#toArray() </seealso>
		Public Overridable Function toArray() As Object()
			Dim rv As Object() = New Object([delegate].Count - 1){}
			[delegate].CopyTo(rv)
			Return rv
		End Function

		''' <summary>
		''' Returns the element at the specified position in this list.
		''' <p>
		''' Throws an <code>ArrayIndexOutOfBoundsException</code>
		''' if the index is out of range
		''' (<code>index &lt; 0 || index &gt;= size()</code>).
		''' </summary>
		''' <param name="index"> index of element to return </param>
		Public Overridable Function [get](ByVal index As Integer) As E
			Return [delegate](index)
		End Function

		''' <summary>
		''' Replaces the element at the specified position in this list with the
		''' specified element.
		''' <p>
		''' Throws an <code>ArrayIndexOutOfBoundsException</code>
		''' if the index is out of range
		''' (<code>index &lt; 0 || index &gt;= size()</code>).
		''' </summary>
		''' <param name="index"> index of element to replace </param>
		''' <param name="element"> element to be stored at the specified position </param>
		''' <returns> the element previously at the specified position </returns>
		Public Overridable Function [set](ByVal index As Integer, ByVal element As E) As E
			Dim rv As E = [delegate](index)
			[delegate](index) = element
			fireContentsChanged(Me, index, index)
			Return rv
		End Function

		''' <summary>
		''' Inserts the specified element at the specified position in this list.
		''' <p>
		''' Throws an <code>ArrayIndexOutOfBoundsException</code> if the
		''' index is out of range
		''' (<code>index &lt; 0 || index &gt; size()</code>).
		''' </summary>
		''' <param name="index"> index at which the specified element is to be inserted </param>
		''' <param name="element"> element to be inserted </param>
		Public Overridable Sub add(ByVal index As Integer, ByVal element As E)
			[delegate].Insert(index, element)
			fireIntervalAdded(Me, index, index)
		End Sub

		''' <summary>
		''' Removes the element at the specified position in this list.
		''' Returns the element that was removed from the list.
		''' <p>
		''' Throws an <code>ArrayIndexOutOfBoundsException</code>
		''' if the index is out of range
		''' (<code>index &lt; 0 || index &gt;= size()</code>).
		''' </summary>
		''' <param name="index"> the index of the element to removed </param>
		''' <returns> the element previously at the specified position </returns>
		Public Overridable Function remove(ByVal index As Integer) As E
			Dim rv As E = [delegate](index)
			[delegate].RemoveAt(index)
			fireIntervalRemoved(Me, index, index)
			Return rv
		End Function

		''' <summary>
		''' Removes all of the elements from this list.  The list will
		''' be empty after this call returns (unless it throws an exception).
		''' </summary>
		Public Overridable Sub clear()
			Dim index1 As Integer = [delegate].Count-1
			[delegate].Clear()
			If index1 >= 0 Then fireIntervalRemoved(Me, 0, index1)
		End Sub

		''' <summary>
		''' Deletes the components at the specified range of indexes.
		''' The removal is inclusive, so specifying a range of (1,5)
		''' removes the component at index 1 and the component at index 5,
		''' as well as all components in between.
		''' <p>
		''' Throws an <code>ArrayIndexOutOfBoundsException</code>
		''' if the index was invalid.
		''' Throws an <code>IllegalArgumentException</code> if
		''' <code>fromIndex &gt; toIndex</code>.
		''' </summary>
		''' <param name="fromIndex"> the index of the lower end of the range </param>
		''' <param name="toIndex">   the index of the upper end of the range </param>
		''' <seealso cref=        #remove(int) </seealso>
		Public Overridable Sub removeRange(ByVal fromIndex As Integer, ByVal toIndex As Integer)
			If fromIndex > toIndex Then Throw New System.ArgumentException("fromIndex must be <= toIndex")
			For i As Integer = toIndex To fromIndex Step -1
				[delegate].RemoveAt(i)
			Next i
			fireIntervalRemoved(Me, fromIndex, toIndex)
		End Sub

	'    
	'    public void addAll(Collection c) {
	'    }
	'
	'    public void addAll(int index, Collection c) {
	'    }
	'    
	End Class

End Namespace