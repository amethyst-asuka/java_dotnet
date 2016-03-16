Imports Microsoft.VisualBasic
Imports sun.java2d.StateTrackable.State

'
' * Copyright (c) 1997, 2008, Oracle and/or its affiliates. All rights reserved.
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

' ****************************************************************
' ******************************************************************
' ******************************************************************
' *** COPYRIGHT (c) Eastman Kodak Company, 1997
' *** As  an unpublished  work pursuant to Title 17 of the United
' *** States Code.  All rights reserved.
' ******************************************************************
' ******************************************************************
' *****************************************************************

Namespace java.awt.image

	''' <summary>
	''' This class extends <CODE>DataBuffer</CODE> and stores data internally
	''' as integers.
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
	''' </summary>
	Public NotInheritable Class DataBufferInt
		Inherits DataBuffer

		''' <summary>
		''' The default data bank. </summary>
		Friend data As Integer()

		''' <summary>
		''' All data banks </summary>
		Friend bankdata As Integer()()

		''' <summary>
		''' Constructs an integer-based <CODE>DataBuffer</CODE> with a single bank
		''' and the specified size.
		''' </summary>
		''' <param name="size"> The size of the <CODE>DataBuffer</CODE>. </param>
		Public Sub New(ByVal size As Integer)
			MyBase.New(STABLE, TYPE_INT, size)
			data = New Integer(size - 1){}
			bankdata = New Integer(0)(){}
			bankdata(0) = data
		End Sub

		''' <summary>
		''' Constructs an integer-based <CODE>DataBuffer</CODE> with the specified number of
		''' banks, all of which are the specified size.
		''' </summary>
		''' <param name="size"> The size of the banks in the <CODE>DataBuffer</CODE>. </param>
		''' <param name="numBanks"> The number of banks in the a<CODE>DataBuffer</CODE>. </param>
		Public Sub New(ByVal size As Integer, ByVal numBanks As Integer)
			MyBase.New(STABLE, TYPE_INT, size, numBanks)
			bankdata = New Integer(numBanks - 1)(){}
			For i As Integer = 0 To numBanks - 1
				bankdata(i) = New Integer(size - 1){}
			Next i
			data = bankdata(0)
		End Sub

        ''' <summary>
        ''' Constructs an integer-based <CODE>DataBuffer</CODE> with a single bank using the
        ''' specified array.
        ''' Only the first <CODE>size</CODE> elements should be used by accessors of
        ''' this <CODE>DataBuffer</CODE>.  <CODE>dataArray</CODE> must be large enough to
        ''' hold <CODE>size</CODE> elements.
        ''' <p>
        ''' Note that {@code DataBuffer} objects created by this constructor
        ''' may be incompatible with <a href="#optimizations">performance
        ''' optimizations</a> used by some implementations (such as caching
        ''' an associated image in video memory).
        ''' </summary>
        ''' <param name="dataArray"> The integer array for the <CODE>DataBuffer</CODE>. </param>
        ''' <param name="size"> The size of the <CODE>DataBuffer</CODE> bank. </param>
        Sub New(dataArray() As Integer, size As Integer)
            MyBase.New(UNTRACKABLE, TYPE_INT, size)
            data = dataArray
            bankdata = New Integer(0)() {}
            bankdata(0) = data
        End Sub

        ''' <summary>
        ''' Constructs an integer-based <CODE>DataBuffer</CODE> with a single bank using the
        ''' specified array, size, and offset.  <CODE>dataArray</CODE> must have at least
        ''' <CODE>offset</CODE> + <CODE>size</CODE> elements.  Only elements <CODE>offset</CODE>
        ''' through <CODE>offset</CODE> + <CODE>size</CODE> - 1
        ''' should be used by accessors of this <CODE>DataBuffer</CODE>.
        ''' <p>
        ''' Note that {@code DataBuffer} objects created by this constructor
        ''' may be incompatible with <a href="#optimizations">performance
        ''' optimizations</a> used by some implementations (such as caching
        ''' an associated image in video memory).
        ''' </summary>
        ''' <param name="dataArray"> The integer array for the <CODE>DataBuffer</CODE>. </param>
        ''' <param name="size"> The size of the <CODE>DataBuffer</CODE> bank. </param>
        ''' <param name="offset"> The offset into the <CODE>dataArray</CODE>. </param>
        Sub New(dataArray() As Integer, size As Integer, offset As Integer)
            MyBase.New(UNTRACKABLE, TYPE_INT, size, 1, offset)
            data = dataArray
            bankdata = New Integer(0)() {}
            bankdata(0) = data
        End Sub

        ''' <summary>
        ''' Constructs an integer-based <CODE>DataBuffer</CODE> with the specified arrays.
        ''' The number of banks will be equal to <CODE>dataArray.length</CODE>.
        ''' Only the first <CODE>size</CODE> elements of each array should be used by
        ''' accessors of this <CODE>DataBuffer</CODE>.
        ''' <p>
        ''' Note that {@code DataBuffer} objects created by this constructor
        ''' may be incompatible with <a href="#optimizations">performance
        ''' optimizations</a> used by some implementations (such as caching
        ''' an associated image in video memory).
        ''' </summary>
        ''' <param name="dataArray"> The integer arrays for the <CODE>DataBuffer</CODE>. </param>
        ''' <param name="size"> The size of the banks in the <CODE>DataBuffer</CODE>. </param>
        Sub New(dataArray()() As Integer, size As Integer)
            MyBase.New(UNTRACKABLE, TYPE_INT, size, dataArray.Length)
            bankdata = CType(dataArray.Clone(), Integer()())
            data = bankdata(0)
        End Sub

        ''' <summary>
        ''' Constructs an integer-based <CODE>DataBuffer</CODE> with the specified arrays, size,
        ''' and offsets.
        ''' The number of banks is equal to <CODE>dataArray.length</CODE>.  Each array must
        ''' be at least as large as <CODE>size</CODE> + the corresponding offset.   There must
        ''' be an entry in the offset array for each <CODE>dataArray</CODE> entry.  For each
        ''' bank, only elements <CODE>offset</CODE> through
        ''' <CODE>offset</CODE> + <CODE>size</CODE> - 1 should be
        ''' used by accessors of this <CODE>DataBuffer</CODE>.
        ''' <p>
        ''' Note that {@code DataBuffer} objects created by this constructor
        ''' may be incompatible with <a href="#optimizations">performance
        ''' optimizations</a> used by some implementations (such as caching
        ''' an associated image in video memory).
        ''' </summary>
        ''' <param name="dataArray"> The integer arrays for the <CODE>DataBuffer</CODE>. </param>
        ''' <param name="size"> The size of the banks in the <CODE>DataBuffer</CODE>. </param>
        ''' <param name="offsets"> The offsets into each array. </param>
        Sub New(dataArray()() As Integer, size As Integer, offsets() As Integer)
            MyBase.New(UNTRACKABLE, TYPE_INT, size, dataArray.Length, offsets)
            bankdata = CType(dataArray.Clone(), Integer()())
            data = bankdata(0)
        End Sub

        ''' <summary>
        ''' Returns the default (first) int data array in <CODE>DataBuffer</CODE>.
        ''' <p>
        ''' Note that calling this method may cause this {@code DataBuffer}
        ''' object to be incompatible with <a href="#optimizations">performance
        ''' optimizations</a> used by some implementations (such as caching
        ''' an associated image in video memory).
        ''' </summary>
        ''' <returns> The first integer data array. </returns>
        Public Function data() As Integer()
            theTrackable.untrackableble()
            Return data
        End Function

        ''' <summary>
        ''' Returns the data array for the specified bank.
        ''' <p>
        ''' Note that calling this method may cause this {@code DataBuffer}
        ''' object to be incompatible with <a href="#optimizations">performance
        ''' optimizations</a> used by some implementations (such as caching
        ''' an associated image in video memory).
        ''' </summary>
        ''' <param name="bank"> The bank whose data array you want to get. </param>
        ''' <returns> The data array for the specified bank. </returns>
        Public Function getData(bank As Integer) As Integer()
            theTrackable.untrackableble()
            Return bankdata(bank)
        End Function
        ''' <summary>
        ''' Returns the data arrays for all banks.
        ''' <p>
        ''' Note that calling this method may cause this {@code DataBuffer}
        ''' object to be incompatible with <a href="#optimizations">performance
        ''' optimizations</a> used by some implementations (such as caching
        ''' an associated image in video memory).
        ''' </summary>
        ''' <returns> All of the data arrays. </returns>
        Public Function bankData() As Integer()()
            theTrackable.untrackableble()
            Return CType(bankdata.clone(), Integer()())
        End Function

        ''' <summary>
        ''' Returns the requested data array element from the first (default) bank.
        ''' </summary>
        ''' <param name="i"> The data array element you want to get. </param>
        ''' <returns> The requested data array element as an  java.lang.[Integer]. </returns>
        ''' <seealso cref= #setElem(int, int) </seealso>
        ''' <seealso cref= #setElem(int, int, int) </seealso>
        Public Function getElem(i As Integer) As Integer
            Return data(i + offset)
        End Function
        ''' <summary>
        ''' Returns the requested data array element from the specified bank.
        ''' </summary>
        ''' <param name="bank"> The bank from which you want to get a data array element. </param>
        ''' <param name="i"> The data array element you want to get. </param>
        ''' <returns> The requested data array element as an  java.lang.[Integer]. </returns>
        ''' <seealso cref= #setElem(int, int) </seealso>
        ''' <seealso cref= #setElem(int, int, int) </seealso>
        Public Function getElem(bank As Integer, i As Integer) As Integer
            Return bankdata(bank)(i + offsets(bank))
        End Function

        ''' <summary>
        ''' Sets the requested data array element in the first (default) bank
        ''' to the specified value.
        ''' </summary>
        ''' <param name="i"> The data array element you want to set. </param>
        ''' <param name="val"> The integer value to which you want to set the data array element. </param>
        ''' <seealso cref= #getElem(int) </seealso>
        ''' <seealso cref= #getElem(int, int) </seealso>
        Public Sub elemlem(i As Integer, val As Integer)
            data(i + offset) = val
            theTrackable.markDirty()
        End Sub

        ''' <summary>
        ''' Sets the requested data array element in the specified bank
        ''' to the integer value <CODE>i</CODE>. </summary>
        ''' <param name="bank"> The bank in which you want to set the data array element. </param>
        ''' <param name="i"> The data array element you want to set. </param>
        ''' <param name="val"> The integer value to which you want to set the specified data array element. </param>
        ''' <seealso cref= #getElem(int) </seealso>
        ''' <seealso cref= #getElem(int, int) </seealso>
        Public Sub elemlem(bank As Integer, i As Integer, val As Integer)
            bankdata(bank)(i + offsets(bank)) = CInt(Fix(val))
            theTrackable.markDirty()
        End Sub

    End Class

End Namespace