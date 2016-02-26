'
' * Copyright (c) 1999, 2013, Oracle and/or its affiliates. All rights reserved.
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
	''' A <code>SizeSequence</code> object
	''' efficiently maintains an ordered list
	''' of sizes and corresponding positions.
	''' One situation for which <code>SizeSequence</code>
	''' might be appropriate is in a component
	''' that displays multiple rows of unequal size.
	''' In this case, a single <code>SizeSequence</code>
	''' object could be used to track the heights
	''' and Y positions of all rows.
	''' <p>
	''' Another example would be a multi-column component,
	''' such as a <code>JTable</code>,
	''' in which the column sizes are not all equal.
	''' The <code>JTable</code> might use a single
	''' <code>SizeSequence</code> object
	''' to store the widths and X positions of all the columns.
	''' The <code>JTable</code> could then use the
	''' <code>SizeSequence</code> object
	''' to find the column corresponding to a certain position.
	''' The <code>JTable</code> could update the
	''' <code>SizeSequence</code> object
	''' whenever one or more column sizes changed.
	''' 
	''' <p>
	''' The following figure shows the relationship between size and position data
	''' for a multi-column component.
	''' 
	''' <center>
	''' <img src="doc-files/SizeSequence-1.gif" width=384 height = 100
	''' alt="The first item begins at position 0, the second at the position equal
	''' to the size of the previous item, and so on.">
	''' </center>
	''' <p>
	''' In the figure, the first index (0) corresponds to the first column,
	''' the second index (1) to the second column, and so on.
	''' The first column's position starts at 0,
	''' and the column occupies <em>size<sub>0</sub></em> pixels,
	''' where <em>size<sub>0</sub></em> is the value returned by
	''' <code>getSize(0)</code>.
	''' Thus, the first column ends at <em>size<sub>0</sub></em> - 1.
	''' The second column then begins at
	''' the position <em>size<sub>0</sub></em>
	''' and occupies <em>size<sub>1</sub></em> (<code>getSize(1)</code>) pixels.
	''' <p>
	''' Note that a <code>SizeSequence</code> object simply represents intervals
	''' along an axis.
	''' In our examples, the intervals represent height or width in pixels.
	''' However, any other unit of measure (for example, time in days)
	''' could be just as valid.
	''' 
	''' 
	''' <h3>Implementation Notes</h3>
	''' 
	''' Normally when storing the size and position of entries,
	''' one would choose between
	''' storing the sizes or storing their positions
	''' instead. The two common operations that are needed during
	''' rendering are: <code>getIndex(position)</code>
	''' and <code>setSize(index, size)</code>.
	''' Whichever choice of internal format is made one of these
	''' operations is costly when the number of entries becomes large.
	''' If sizes are stored, finding the index of the entry
	''' that encloses a particular position is linear in the
	''' number of entries. If positions are stored instead, setting
	''' the size of an entry at a particular index requires updating
	''' the positions of the affected entries, which is also a linear
	''' calculation.
	''' <p>
	''' Like the above techniques this class holds an array of N integers
	''' internally but uses a hybrid encoding, which is halfway
	''' between the size-based and positional-based approaches.
	''' The result is a data structure that takes the same space to store
	''' the information but can perform most operations in Log(N) time
	''' instead of O(N), where N is the number of entries in the list.
	''' <p>
	''' Two operations that remain O(N) in the number of entries are
	''' the <code>insertEntries</code>
	''' and <code>removeEntries</code> methods, both
	''' of which are implemented by converting the internal array to
	''' a set of integer sizes, copying it into the new array, and then
	''' reforming the hybrid representation in place.
	''' 
	''' @author Philip Milne
	''' @since 1.3
	''' </summary>

	'
	' *   Each method is implemented by taking the minimum and
	' *   maximum of the range of integers that need to be operated
	' *   upon. All the algorithms work by dividing this range
	' *   into two smaller ranges and recursing. The recursion
	' *   is terminated when the upper and lower bounds are equal.
	' 

	Public Class SizeSequence

		Private Shared emptyArray As Integer() = New Integer(){}
		Private a As Integer()

		''' <summary>
		''' Creates a new <code>SizeSequence</code> object
		''' that contains no entries.  To add entries, you
		''' can use <code>insertEntries</code> or <code>setSizes</code>.
		''' </summary>
		''' <seealso cref= #insertEntries </seealso>
		''' <seealso cref= #setSizes(int[]) </seealso>
		Public Sub New()
			a = emptyArray
		End Sub

		''' <summary>
		''' Creates a new <code>SizeSequence</code> object
		''' that contains the specified number of entries,
		''' all initialized to have size 0.
		''' </summary>
		''' <param name="numEntries">  the number of sizes to track </param>
		''' <exception cref="NegativeArraySizeException"> if
		'''    <code>numEntries &lt; 0</code> </exception>
		Public Sub New(ByVal numEntries As Integer)
			Me.New(numEntries, 0)
		End Sub

		''' <summary>
		''' Creates a new <code>SizeSequence</code> object
		''' that contains the specified number of entries,
		''' all initialized to have size <code>value</code>.
		''' </summary>
		''' <param name="numEntries">  the number of sizes to track </param>
		''' <param name="value">       the initial value of each size </param>
		Public Sub New(ByVal numEntries As Integer, ByVal value As Integer)
			Me.New()
			insertEntries(0, numEntries, value)
		End Sub

		''' <summary>
		''' Creates a new <code>SizeSequence</code> object
		''' that contains the specified sizes.
		''' </summary>
		''' <param name="sizes">  the array of sizes to be contained in
		'''               the <code>SizeSequence</code> </param>
		Public Sub New(ByVal sizes As Integer())
			Me.New()
			sizes = sizes
		End Sub

		''' <summary>
		''' Resets the size sequence to contain <code>length</code> items
		''' all with a size of <code>size</code>.
		''' </summary>
		Friend Overridable Sub setSizes(ByVal length As Integer, ByVal size As Integer)
			If a.Length <> length Then a = New Integer(length - 1){}
			sizeszes(0, length, size)
		End Sub

		Private Function setSizes(ByVal [from] As Integer, ByVal [to] As Integer, ByVal size As Integer) As Integer
			If [to] <= [from] Then Return 0
			Dim m As Integer = ([from] + [to])\2
			a(m) = size + sizeszes([from], m, size)
			Return a(m) + sizeszes(m + 1, [to], size)
		End Function

		''' <summary>
		''' Resets this <code>SizeSequence</code> object,
		''' using the data in the <code>sizes</code> argument.
		''' This method reinitializes this object so that it
		''' contains as many entries as the <code>sizes</code> array.
		''' Each entry's size is initialized to the value of the
		''' corresponding item in <code>sizes</code>.
		''' </summary>
		''' <param name="sizes">  the array of sizes to be contained in
		'''               this <code>SizeSequence</code> </param>
'JAVA TO VB CONVERTER TODO TASK: The following line could not be converted:
        Public Overridable Sub setSizes(ByVal sizes As Integer[]) 'JavaToDotNetTempPropertySetsizes
		Public Overridable Property sizes As Integer()
			Set(ByVal sizes As Integer())
				If a.Length <> sizes.Length Then a = New Integer(sizes.Length - 1){}
				sizeszes(0, a.Length, sizes)
			End Set
			Get
		End Property

		Private Function setSizes(ByVal [from] As Integer, ByVal [to] As Integer, ByVal sizes As Integer()) As Integer
			If [to] <= [from] Then Return 0
			Dim m As Integer = ([from] + [to])\2
			a(m) = sizes(m) + sizeszes([from], m, sizes)
			Return a(m) + sizeszes(m + 1, [to], sizes)
		End Function

			Dim n As Integer = a.Length
			Dim ___sizes As Integer() = New Integer(n - 1){}
			getSizes(0, n, ___sizes)
			Return ___sizes
		End Function

		Private Function getSizes(ByVal [from] As Integer, ByVal [to] As Integer, ByVal sizes As Integer()) As Integer
			If [to] <= [from] Then Return 0
			Dim m As Integer = ([from] + [to])\2
			sizes(m) = a(m) - getSizes([from], m, sizes)
			Return a(m) + getSizes(m + 1, [to], sizes)
		End Function

		''' <summary>
		''' Returns the start position for the specified entry.
		''' For example, <code>getPosition(0)</code> returns 0,
		''' <code>getPosition(1)</code> is equal to
		'''   <code>getSize(0)</code>,
		''' <code>getPosition(2)</code> is equal to
		'''   <code>getSize(0)</code> + <code>getSize(1)</code>,
		''' and so on.
		''' <p>Note that if <code>index</code> is greater than
		''' <code>length</code> the value returned may
		''' be meaningless.
		''' </summary>
		''' <param name="index">  the index of the entry whose position is desired </param>
		''' <returns>       the starting position of the specified entry </returns>
		Public Overridable Function getPosition(ByVal index As Integer) As Integer
			Return getPosition(0, a.Length, index)
		End Function

		Private Function getPosition(ByVal [from] As Integer, ByVal [to] As Integer, ByVal index As Integer) As Integer
			If [to] <= [from] Then Return 0
			Dim m As Integer = ([from] + [to])\2
			If index <= m Then
				Return getPosition([from], m, index)
			Else
				Return a(m) + getPosition(m + 1, [to], index)
			End If
		End Function

		''' <summary>
		''' Returns the index of the entry
		''' that corresponds to the specified position.
		''' For example, <code>getIndex(0)</code> is 0,
		''' since the first entry always starts at position 0.
		''' </summary>
		''' <param name="position">  the position of the entry </param>
		''' <returns>  the index of the entry that occupies the specified position </returns>
		Public Overridable Function getIndex(ByVal position As Integer) As Integer
			Return getIndex(0, a.Length, position)
		End Function

		Private Function getIndex(ByVal [from] As Integer, ByVal [to] As Integer, ByVal position As Integer) As Integer
			If [to] <= [from] Then Return [from]
			Dim m As Integer = ([from] + [to])\2
			Dim pivot As Integer = a(m)
			If position < pivot Then
			   Return getIndex([from], m, position)
			Else
				Return getIndex(m + 1, [to], position - pivot)
			End If
		End Function

		''' <summary>
		''' Returns the size of the specified entry.
		''' If <code>index</code> is out of the range
		''' <code>(0 &lt;= index &lt; getSizes().length)</code>
		''' the behavior is unspecified.
		''' </summary>
		''' <param name="index">  the index corresponding to the entry </param>
		''' <returns>  the size of the entry </returns>
		Public Overridable Function getSize(ByVal index As Integer) As Integer
			Return getPosition(index + 1) - getPosition(index)
		End Function

		''' <summary>
		''' Sets the size of the specified entry.
		''' Note that if the value of <code>index</code>
		''' does not fall in the range:
		''' <code>(0 &lt;= index &lt; getSizes().length)</code>
		''' the behavior is unspecified.
		''' </summary>
		''' <param name="index">  the index corresponding to the entry </param>
		''' <param name="size">   the size of the entry </param>
		Public Overridable Sub setSize(ByVal index As Integer, ByVal size As Integer)
			changeSize(0, a.Length, index, size - getSize(index))
		End Sub

		Private Sub changeSize(ByVal [from] As Integer, ByVal [to] As Integer, ByVal index As Integer, ByVal delta As Integer)
			If [to] <= [from] Then Return
			Dim m As Integer = ([from] + [to])\2
			If index <= m Then
				a(m) += delta
				changeSize([from], m, index, delta)
			Else
				changeSize(m + 1, [to], index, delta)
			End If
		End Sub

		''' <summary>
		''' Adds a contiguous group of entries to this <code>SizeSequence</code>.
		''' Note that the values of <code>start</code> and
		''' <code>length</code> must satisfy the following
		''' conditions:  <code>(0 &lt;= start &lt; getSizes().length)
		''' AND (length &gt;= 0)</code>.  If these conditions are
		''' not met, the behavior is unspecified and an exception
		''' may be thrown.
		''' </summary>
		''' <param name="start">   the index to be assigned to the first entry
		'''                in the group </param>
		''' <param name="length">  the number of entries in the group </param>
		''' <param name="value">   the size to be assigned to each new entry </param>
		''' <exception cref="ArrayIndexOutOfBoundsException"> if the parameters
		'''   are outside of the range:
		'''   (<code>0 &lt;= start &lt; (getSizes().length)) AND (length &gt;= 0)</code> </exception>
		Public Overridable Sub insertEntries(ByVal start As Integer, ByVal length As Integer, ByVal value As Integer)
			Dim ___sizes As Integer() = sizes
			Dim [end] As Integer = start + length
			Dim n As Integer = a.Length + length
			a = New Integer(n - 1){}
			For i As Integer = 0 To start - 1
				a(i) = ___sizes(i)
			Next i
			For i As Integer = start To [end] - 1
				a(i) = value
			Next i
			For i As Integer = end To n - 1
				a(i) = ___sizes(i-length)
			Next i
			sizes = a
		End Sub

		''' <summary>
		''' Removes a contiguous group of entries
		''' from this <code>SizeSequence</code>.
		''' Note that the values of <code>start</code> and
		''' <code>length</code> must satisfy the following
		''' conditions:  <code>(0 &lt;= start &lt; getSizes().length)
		''' AND (length &gt;= 0)</code>.  If these conditions are
		''' not met, the behavior is unspecified and an exception
		''' may be thrown.
		''' </summary>
		''' <param name="start">   the index of the first entry to be removed </param>
		''' <param name="length">  the number of entries to be removed </param>
		Public Overridable Sub removeEntries(ByVal start As Integer, ByVal length As Integer)
			Dim ___sizes As Integer() = sizes
			Dim [end] As Integer = start + length
			Dim n As Integer = a.Length - length
			a = New Integer(n - 1){}
			For i As Integer = 0 To start - 1
				a(i) = ___sizes(i)
			Next i
			For i As Integer = start To n - 1
				a(i) = ___sizes(i+length)
			Next i
			sizes = a
		End Sub
	End Class

End Namespace