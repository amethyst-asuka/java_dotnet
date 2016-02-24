Imports Microsoft.VisualBasic
Imports sun.java2d.StateTrackable.State

'
' * Copyright (c) 2000, 2007, Oracle and/or its affiliates. All rights reserved.
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

Namespace java.awt.image

	''' <summary>
	''' This class extends <code>DataBuffer</code> and stores data internally
	''' in <code>double</code> form.
	''' <p>
	''' <a name="optimizations">
	''' Note that some implementations may function more efficiently
	''' if they can maintain control over how the data for an image is
	''' stored.
	''' For example, optimizations such as caching an image in video
	''' memory require that the implementation track all modifications
	''' to that data.
	''' Other implementations may operate better if they can store the
	''' data in locations other than a Java array.
	''' To maintain optimum compatibility with various optimizations
	''' it is best to avoid constructors and methods which expose the
	''' underlying storage as a Java array as noted below in the
	''' documentation for those methods.
	''' </a>
	''' 
	''' @since 1.4
	''' </summary>

	Public NotInheritable Class DataBufferDouble
		Inherits DataBuffer

		''' <summary>
		''' The array of data banks. </summary>
		Friend bankdata As Double()()

		''' <summary>
		''' A reference to the default data bank. </summary>
		Friend data As Double()

		''' <summary>
		''' Constructs a <code>double</code>-based <code>DataBuffer</code>
		''' with a specified size.
		''' </summary>
		''' <param name="size"> The number of elements in the <code>DataBuffer</code>. </param>
		Public Sub New(ByVal size As Integer)
			MyBase.New(STABLE, TYPE_DOUBLE, size)
			data = New Double(size - 1){}
			bankdata = New Double(0)(){}
			bankdata(0) = data
		End Sub

		''' <summary>
		''' Constructs a <code>double</code>-based <code>DataBuffer</code>
		''' with a specified number of banks, all of which are of a
		''' specified size.
		''' </summary>
		''' <param name="size"> The number of elements in each bank of the
		'''        <code>DataBuffer</code>. </param>
		''' <param name="numBanks"> The number of banks in the <code>DataBuffer</code>. </param>
		Public Sub New(ByVal size As Integer, ByVal numBanks As Integer)
			MyBase.New(STABLE, TYPE_DOUBLE, size, numBanks)
			bankdata = New Double(numBanks - 1)(){}
			For i As Integer = 0 To numBanks - 1
				bankdata(i) = New Double(size - 1){}
			Next i
			data = bankdata(0)
		End Sub

		''' <summary>
		''' Constructs a <code>double</code>-based <code>DataBuffer</code>
		''' with the specified data array.  Only the first
		''' <code>size</code> elements are available for use by this
		''' <code>DataBuffer</code>.  The array must be large enough to
		''' hold <code>size</code> elements.
		''' <p>
		''' Note that {@code DataBuffer} objects created by this constructor
		''' may be incompatible with <a href="#optimizations">performance
		''' optimizations</a> used by some implementations (such as caching
		''' an associated image in video memory).
		''' </summary>
		''' <param name="dataArray"> An array of <code>double</code>s to be used as the
		'''                  first and only bank of this <code>DataBuffer</code>. </param>
		''' <param name="size"> The number of elements of the array to be used. </param>
'JAVA TO VB CONVERTER TODO TASK: The following line could not be converted:
		public DataBufferDouble(double dataArray() , int size)
			MyBase(UNTRACKABLE, TYPE_DOUBLE, size)
			data = dataArray
			bankdata = New Double(0)(){}
			bankdata(0) = data

		''' <summary>
		''' Constructs a <code>double</code>-based <code>DataBuffer</code>
		''' with the specified data array.  Only the elements between
		''' <code>offset</code> and <code>offset + size - 1</code> are
		''' available for use by this <code>DataBuffer</code>.  The array
		''' must be large enough to hold <code>offset + size</code> elements.
		''' <p>
		''' Note that {@code DataBuffer} objects created by this constructor
		''' may be incompatible with <a href="#optimizations">performance
		''' optimizations</a> used by some implementations (such as caching
		''' an associated image in video memory).
		''' </summary>
		''' <param name="dataArray"> An array of <code>double</code>s to be used as the
		'''                  first and only bank of this <code>DataBuffer</code>. </param>
		''' <param name="size"> The number of elements of the array to be used. </param>
		''' <param name="offset"> The offset of the first element of the array
		'''               that will be used. </param>
		public DataBufferDouble(Double dataArray() , Integer size, Integer offset)
			MyBase(UNTRACKABLE, TYPE_DOUBLE, size, 1, offset)
			data = dataArray
			bankdata = New Double(0)(){}
			bankdata(0) = data

		''' <summary>
		''' Constructs a <code>double</code>-based <code>DataBuffer</code>
		''' with the specified data arrays.  Only the first
		''' <code>size</code> elements of each array are available for use
		''' by this <code>DataBuffer</code>.  The number of banks will be
		''' equal <code>to dataArray.length</code>.
		''' <p>
		''' Note that {@code DataBuffer} objects created by this constructor
		''' may be incompatible with <a href="#optimizations">performance
		''' optimizations</a> used by some implementations (such as caching
		''' an associated image in video memory).
		''' </summary>
		''' <param name="dataArray"> An array of arrays of <code>double</code>s to be
		'''        used as the banks of this <code>DataBuffer</code>. </param>
		''' <param name="size"> The number of elements of each array to be used. </param>
		public DataBufferDouble(Double dataArray()(), Integer size)
			MyBase(UNTRACKABLE, TYPE_DOUBLE, size, dataArray.length)
			bankdata = CType(dataArray.clone(), Double()())
			data = bankdata(0)

		''' <summary>
		''' Constructs a <code>double</code>-based <code>DataBuffer</code>
		''' with the specified data arrays, size, and per-bank offsets.
		''' The number of banks is equal to dataArray.length.  Each array
		''' must be at least as large as <code>size</code> plus the
		''' corresponding offset.  There must be an entry in the
		''' <code>offsets</code> array for each data array.
		''' <p>
		''' Note that {@code DataBuffer} objects created by this constructor
		''' may be incompatible with <a href="#optimizations">performance
		''' optimizations</a> used by some implementations (such as caching
		''' an associated image in video memory).
		''' </summary>
		''' <param name="dataArray"> An array of arrays of <code>double</code>s to be
		'''        used as the banks of this <code>DataBuffer</code>. </param>
		''' <param name="size"> The number of elements of each array to be used. </param>
		''' <param name="offsets"> An array of integer offsets, one for each bank. </param>
		public DataBufferDouble(Double dataArray()(), Integer size, Integer offsets())
			MyBase(UNTRACKABLE, TYPE_DOUBLE, size, dataArray.length, offsets)
			bankdata = CType(dataArray.clone(), Double()())
			data = bankdata(0)

		''' <summary>
		''' Returns the default (first) <code>double</code> data array.
		''' <p>
		''' Note that calling this method may cause this {@code DataBuffer}
		''' object to be incompatible with <a href="#optimizations">performance
		''' optimizations</a> used by some implementations (such as caching
		''' an associated image in video memory).
		''' </summary>
		''' <returns> the first double data array. </returns>
		public Double() data
			theTrackable.untrackableble()
			Return data

		''' <summary>
		''' Returns the data array for the specified bank.
		''' <p>
		''' Note that calling this method may cause this {@code DataBuffer}
		''' object to be incompatible with <a href="#optimizations">performance
		''' optimizations</a> used by some implementations (such as caching
		''' an associated image in video memory).
		''' </summary>
		''' <param name="bank"> the data array </param>
		''' <returns> the data array specified by <code>bank</code>. </returns>
		public Double() getData(Integer bank)
			theTrackable.untrackableble()
			Return bankdata(bank)

		''' <summary>
		''' Returns the data array for all banks.
		''' <p>
		''' Note that calling this method may cause this {@code DataBuffer}
		''' object to be incompatible with <a href="#optimizations">performance
		''' optimizations</a> used by some implementations (such as caching
		''' an associated image in video memory).
		''' </summary>
		''' <returns> all data arrays from this data buffer. </returns>
		public Double()() bankData
			theTrackable.untrackableble()
			Return CType(bankdata.clone(), Double()())

		''' <summary>
		''' Returns the requested data array element from the first
		''' (default) bank as an <code>int</code>.
		''' </summary>
		''' <param name="i"> The desired data array element. </param>
		''' <returns> The data entry as an <code>int</code>. </returns>
		''' <seealso cref= #setElem(int, int) </seealso>
		''' <seealso cref= #setElem(int, int, int) </seealso>
		public Integer getElem(Integer i)
			Return CInt(Fix(data(i+offset)))

		''' <summary>
		''' Returns the requested data array element from the specified
		''' bank as an <code>int</code>.
		''' </summary>
		''' <param name="bank"> The bank number. </param>
		''' <param name="i"> The desired data array element.
		''' </param>
		''' <returns> The data entry as an <code>int</code>. </returns>
		''' <seealso cref= #setElem(int, int) </seealso>
		''' <seealso cref= #setElem(int, int, int) </seealso>
		public Integer getElem(Integer bank, Integer i)
			Return CInt(Fix(bankdata(bank)(i+offsets(bank))))

		''' <summary>
		''' Sets the requested data array element in the first (default)
		''' bank to the given <code>int</code>.
		''' </summary>
		''' <param name="i"> The desired data array element. </param>
		''' <param name="val"> The value to be set. </param>
		''' <seealso cref= #getElem(int) </seealso>
		''' <seealso cref= #getElem(int, int) </seealso>
		public void elemlem(Integer i, Integer val)
			data(i+offset) = CDbl(val)
			theTrackable.markDirty()

		''' <summary>
		''' Sets the requested data array element in the specified bank
		''' to the given <code>int</code>.
		''' </summary>
		''' <param name="bank"> The bank number. </param>
		''' <param name="i"> The desired data array element. </param>
		''' <param name="val"> The value to be set. </param>
		''' <seealso cref= #getElem(int) </seealso>
		''' <seealso cref= #getElem(int, int) </seealso>
		public void elemlem(Integer bank, Integer i, Integer val)
			bankdata(bank)(i+offsets(bank)) = CDbl(val)
			theTrackable.markDirty()

		''' <summary>
		''' Returns the requested data array element from the first
		''' (default) bank as a <code>float</code>.
		''' </summary>
		''' <param name="i"> The desired data array element.
		''' </param>
		''' <returns> The data entry as a <code>float</code>. </returns>
		''' <seealso cref= #setElemFloat(int, float) </seealso>
		''' <seealso cref= #setElemFloat(int, int, float) </seealso>
		public Single getElemFloat(Integer i)
			Return CSng(data(i+offset))

		''' <summary>
		''' Returns the requested data array element from the specified
		''' bank as a <code>float</code>.
		''' </summary>
		''' <param name="bank"> The bank number. </param>
		''' <param name="i"> The desired data array element.
		''' </param>
		''' <returns> The data entry as a <code>float</code>. </returns>
		''' <seealso cref= #setElemFloat(int, float) </seealso>
		''' <seealso cref= #setElemFloat(int, int, float) </seealso>
		public Single getElemFloat(Integer bank, Integer i)
			Return CSng(bankdata(bank)(i+offsets(bank)))

		''' <summary>
		''' Sets the requested data array element in the first (default)
		''' bank to the given <code>float</code>.
		''' </summary>
		''' <param name="i"> The desired data array element. </param>
		''' <param name="val"> The value to be set. </param>
		''' <seealso cref= #getElemFloat(int) </seealso>
		''' <seealso cref= #getElemFloat(int, int) </seealso>
		public void elemFloatoat(Integer i, Single val)
			data(i+offset) = CDbl(val)
			theTrackable.markDirty()

		''' <summary>
		''' Sets the requested data array element in the specified bank to
		''' the given <code>float</code>.
		''' </summary>
		''' <param name="bank"> The bank number. </param>
		''' <param name="i"> The desired data array element. </param>
		''' <param name="val"> The value to be set. </param>
		''' <seealso cref= #getElemFloat(int) </seealso>
		''' <seealso cref= #getElemFloat(int, int) </seealso>
		public void elemFloatoat(Integer bank, Integer i, Single val)
			bankdata(bank)(i+offsets(bank)) = CDbl(val)
			theTrackable.markDirty()

		''' <summary>
		''' Returns the requested data array element from the first
		''' (default) bank as a <code>double</code>.
		''' </summary>
		''' <param name="i"> The desired data array element.
		''' </param>
		''' <returns> The data entry as a <code>double</code>. </returns>
		''' <seealso cref= #setElemDouble(int, double) </seealso>
		''' <seealso cref= #setElemDouble(int, int, double) </seealso>
		public Double getElemDouble(Integer i)
			Return data(i+offset)

		''' <summary>
		''' Returns the requested data array element from the specified
		''' bank as a <code>double</code>.
		''' </summary>
		''' <param name="bank"> The bank number. </param>
		''' <param name="i"> The desired data array element.
		''' </param>
		''' <returns> The data entry as a <code>double</code>. </returns>
		''' <seealso cref= #setElemDouble(int, double) </seealso>
		''' <seealso cref= #setElemDouble(int, int, double) </seealso>
		public Double getElemDouble(Integer bank, Integer i)
			Return bankdata(bank)(i+offsets(bank))

		''' <summary>
		''' Sets the requested data array element in the first (default)
		''' bank to the given <code>double</code>.
		''' </summary>
		''' <param name="i"> The desired data array element. </param>
		''' <param name="val"> The value to be set. </param>
		''' <seealso cref= #getElemDouble(int) </seealso>
		''' <seealso cref= #getElemDouble(int, int) </seealso>
		public void elemDoubleble(Integer i, Double val)
			data(i+offset) = val
			theTrackable.markDirty()

		''' <summary>
		''' Sets the requested data array element in the specified bank to
		''' the given <code>double</code>.
		''' </summary>
		''' <param name="bank"> The bank number. </param>
		''' <param name="i"> The desired data array element. </param>
		''' <param name="val"> The value to be set. </param>
		''' <seealso cref= #getElemDouble(int) </seealso>
		''' <seealso cref= #getElemDouble(int, int) </seealso>
		public void elemDoubleble(Integer bank, Integer i, Double val)
			bankdata(bank)(i+offsets(bank)) = val
			theTrackable.markDirty()
	End Class

End Namespace