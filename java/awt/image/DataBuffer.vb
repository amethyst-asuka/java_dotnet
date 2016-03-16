Imports Microsoft.VisualBasic
Imports sun.java2d.StateTrackable.State

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
	''' This class exists to wrap one or more data arrays.  Each data array in
	''' the DataBuffer is referred to as a bank.  Accessor methods for getting
	''' and setting elements of the DataBuffer's banks exist with and without
	''' a bank specifier.  The methods without a bank specifier use the default 0th
	''' bank.  The DataBuffer can optionally take an offset per bank, so that
	''' data in an existing array can be used even if the interesting data
	''' doesn't start at array location zero.  Getting or setting the 0th
	''' element of a bank, uses the (0+offset)th element of the array.  The
	''' size field specifies how much of the data array is available for
	''' use.  Size + offset for a given bank should never be greater
	''' than the length of the associated data array.  The data type of
	''' a data buffer indicates the type of the data array(s) and may also
	''' indicate additional semantics, e.g. storing unsigned 8-bit data
	''' in elements of a byte array.  The data type may be TYPE_UNDEFINED
	''' or one of the types defined below.  Other types may be added in
	''' the future.  Generally, an object of class DataBuffer will be cast down
	''' to one of its data type specific subclasses to access data type specific
	''' methods for improved performance.  Currently, the Java 2D(tm) API
	''' image classes use TYPE_BYTE, TYPE_USHORT, TYPE_INT, TYPE_SHORT,
	''' TYPE_FLOAT, and TYPE_DOUBLE DataBuffers to store image data. </summary>
	''' <seealso cref= java.awt.image.Raster </seealso>
	''' <seealso cref= java.awt.image.SampleModel </seealso>
	Public MustInherit Class DataBuffer

		''' <summary>
		''' Tag for unsigned byte data. </summary>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
		Public Const TYPE_BYTE As Integer = 0

		''' <summary>
		''' Tag for unsigned short data. </summary>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
		Public Const TYPE_USHORT As Integer = 1

		''' <summary>
		''' Tag for signed short data.  Placeholder for future use. </summary>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
		Public Const TYPE_SHORT As Integer = 2

		''' <summary>
		''' Tag for int data. </summary>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
		Public Const TYPE_INT As Integer = 3

		''' <summary>
		''' Tag for float data.  Placeholder for future use. </summary>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
		Public Const TYPE_FLOAT As Integer = 4

		''' <summary>
		''' Tag for double data.  Placeholder for future use. </summary>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
		Public Const TYPE_DOUBLE As Integer = 5

		''' <summary>
		''' Tag for undefined data. </summary>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
		Public Const TYPE_UNDEFINED As Integer = 32

		''' <summary>
		''' The data type of this DataBuffer. </summary>
		Protected Friend dataType As Integer

		''' <summary>
		''' The number of banks in this DataBuffer. </summary>
		Protected Friend banks As Integer

		''' <summary>
		''' Offset into default (first) bank from which to get the first element. </summary>
		Protected Friend offset As Integer

		''' <summary>
		''' Usable size of all banks. </summary>
		Protected Friend size As Integer

		''' <summary>
		''' Offsets into all banks. </summary>
		Protected Friend offsets As Integer()

		' The current StateTrackable state. 
		Friend theTrackable As sun.java2d.StateTrackableDelegate

		''' <summary>
		''' Size of the data types indexed by DataType tags defined above. </summary>
		Private Shared ReadOnly dataTypeSize As Integer() = {8,16,16,32,32,64}

		''' <summary>
		''' Returns the size (in bits) of the data type, given a datatype tag. </summary>
		''' <param name="type"> the value of one of the defined datatype tags </param>
		''' <returns> the size of the data type </returns>
		''' <exception cref="IllegalArgumentException"> if <code>type</code> is less than
		'''         zero or greater than <seealso cref="#TYPE_DOUBLE"/> </exception>
		Public Shared Function getDataTypeSize(ByVal type As Integer) As Integer
			If type < TYPE_BYTE OrElse type > TYPE_DOUBLE Then Throw New IllegalArgumentException("Unknown data type " & type)
			Return dataTypeSize(type)
		End Function

		''' <summary>
		'''  Constructs a DataBuffer containing one bank of the specified
		'''  data type and size.
		''' </summary>
		'''  <param name="dataType"> the data type of this <code>DataBuffer</code> </param>
		'''  <param name="size"> the size of the banks </param>
		Protected Friend Sub New(ByVal dataType As Integer, ByVal size As Integer)
			Me.New(UNTRACKABLE, dataType, size)
		End Sub

		''' <summary>
		'''  Constructs a DataBuffer containing one bank of the specified
		'''  data type and size with the indicated initial <seealso cref="State State"/>.
		''' </summary>
		'''  <param name="initialState"> the initial <seealso cref="State State"/> state of the data </param>
		'''  <param name="dataType"> the data type of this <code>DataBuffer</code> </param>
		'''  <param name="size"> the size of the banks
		'''  @since 1.7 </param>
		Friend Sub New(ByVal initialState As sun.java2d.StateTrackable.State, ByVal dataType As Integer, ByVal size As Integer)
			Me.theTrackable = sun.java2d.StateTrackableDelegate.createInstance(initialState)
			Me.dataType = dataType
			Me.banks = 1
			Me.size = size
			Me.offset = 0
			Me.offsets = New Integer(0){} ' init to 0 by new
		End Sub

		''' <summary>
		'''  Constructs a DataBuffer containing the specified number of
		'''  banks.  Each bank has the specified size and an offset of 0.
		''' </summary>
		'''  <param name="dataType"> the data type of this <code>DataBuffer</code> </param>
		'''  <param name="size"> the size of the banks </param>
		'''  <param name="numBanks"> the number of banks in this
		'''         <code>DataBuffer</code> </param>
		Protected Friend Sub New(ByVal dataType As Integer, ByVal size As Integer, ByVal numBanks As Integer)
			Me.New(UNTRACKABLE, dataType, size, numBanks)
		End Sub

		''' <summary>
		'''  Constructs a DataBuffer containing the specified number of
		'''  banks with the indicated initial <seealso cref="State State"/>.
		'''  Each bank has the specified size and an offset of 0.
		''' </summary>
		'''  <param name="initialState"> the initial <seealso cref="State State"/> state of the data </param>
		'''  <param name="dataType"> the data type of this <code>DataBuffer</code> </param>
		'''  <param name="size"> the size of the banks </param>
		'''  <param name="numBanks"> the number of banks in this
		'''         <code>DataBuffer</code>
		'''  @since 1.7 </param>
		Friend Sub New(ByVal initialState As sun.java2d.StateTrackable.State, ByVal dataType As Integer, ByVal size As Integer, ByVal numBanks As Integer)
			Me.theTrackable = sun.java2d.StateTrackableDelegate.createInstance(initialState)
			Me.dataType = dataType
			Me.banks = numBanks
			Me.size = size
			Me.offset = 0
			Me.offsets = New Integer(banks - 1){} ' init to 0 by new
		End Sub

		''' <summary>
		'''  Constructs a DataBuffer that contains the specified number
		'''  of banks.  Each bank has the specified datatype, size and offset.
		''' </summary>
		'''  <param name="dataType"> the data type of this <code>DataBuffer</code> </param>
		'''  <param name="size"> the size of the banks </param>
		'''  <param name="numBanks"> the number of banks in this
		'''         <code>DataBuffer</code> </param>
		'''  <param name="offset"> the offset for each bank </param>
		Protected Friend Sub New(ByVal dataType As Integer, ByVal size As Integer, ByVal numBanks As Integer, ByVal offset As Integer)
			Me.New(UNTRACKABLE, dataType, size, numBanks, offset)
		End Sub

		''' <summary>
		'''  Constructs a DataBuffer that contains the specified number
		'''  of banks with the indicated initial <seealso cref="State State"/>.
		'''  Each bank has the specified datatype, size and offset.
		''' </summary>
		'''  <param name="initialState"> the initial <seealso cref="State State"/> state of the data </param>
		'''  <param name="dataType"> the data type of this <code>DataBuffer</code> </param>
		'''  <param name="size"> the size of the banks </param>
		'''  <param name="numBanks"> the number of banks in this
		'''         <code>DataBuffer</code> </param>
		'''  <param name="offset"> the offset for each bank
		'''  @since 1.7 </param>
		Friend Sub New(ByVal initialState As sun.java2d.StateTrackable.State, ByVal dataType As Integer, ByVal size As Integer, ByVal numBanks As Integer, ByVal offset As Integer)
			Me.theTrackable = sun.java2d.StateTrackableDelegate.createInstance(initialState)
			Me.dataType = dataType
			Me.banks = numBanks
			Me.size = size
			Me.offset = offset
			Me.offsets = New Integer(numBanks - 1){}
			For i As Integer = 0 To numBanks - 1
				Me.offsets(i) = offset
			Next i
		End Sub

		''' <summary>
		'''  Constructs a DataBuffer which contains the specified number
		'''  of banks.  Each bank has the specified datatype and size.  The
		'''  offset for each bank is specified by its respective entry in
		'''  the offsets array.
		''' </summary>
		'''  <param name="dataType"> the data type of this <code>DataBuffer</code> </param>
		'''  <param name="size"> the size of the banks </param>
		'''  <param name="numBanks"> the number of banks in this
		'''         <code>DataBuffer</code> </param>
		'''  <param name="offsets"> an array containing an offset for each bank. </param>
		'''  <exception cref="ArrayIndexOutOfBoundsException"> if <code>numBanks</code>
		'''          does not equal the length of <code>offsets</code> </exception>
		Protected Friend Sub New(ByVal dataType As Integer, ByVal size As Integer, ByVal numBanks As Integer, ByVal offsets As Integer())
			Me.New(UNTRACKABLE, dataType, size, numBanks, offsets)
		End Sub

		''' <summary>
		'''  Constructs a DataBuffer which contains the specified number
		'''  of banks with the indicated initial <seealso cref="State State"/>.
		'''  Each bank has the specified datatype and size.  The
		'''  offset for each bank is specified by its respective entry in
		'''  the offsets array.
		''' </summary>
		'''  <param name="initialState"> the initial <seealso cref="State State"/> state of the data </param>
		'''  <param name="dataType"> the data type of this <code>DataBuffer</code> </param>
		'''  <param name="size"> the size of the banks </param>
		'''  <param name="numBanks"> the number of banks in this
		'''         <code>DataBuffer</code> </param>
		'''  <param name="offsets"> an array containing an offset for each bank. </param>
		'''  <exception cref="ArrayIndexOutOfBoundsException"> if <code>numBanks</code>
		'''          does not equal the length of <code>offsets</code>
		'''  @since 1.7 </exception>
		Friend Sub New(ByVal initialState As sun.java2d.StateTrackable.State, ByVal dataType As Integer, ByVal size As Integer, ByVal numBanks As Integer, ByVal offsets As Integer())
			If numBanks <> offsets.Length Then Throw New ArrayIndexOutOfBoundsException("Number of banks" & " does not match number of bank offsets")
			Me.theTrackable = sun.java2d.StateTrackableDelegate.createInstance(initialState)
			Me.dataType = dataType
			Me.banks = numBanks
			Me.size = size
			Me.offset = offsets(0)
			Me.offsets = CType(offsets.clone(), Integer())
		End Sub

		''' <summary>
		'''  Returns the data type of this DataBuffer. </summary>
		'''   <returns> the data type of this <code>DataBuffer</code>. </returns>
		Public Overridable Property dataType As Integer
			Get
				Return dataType
			End Get
		End Property

		''' <summary>
		'''  Returns the size (in array elements) of all banks. </summary>
		'''   <returns> the size of all banks. </returns>
		Public Overridable Property size As Integer
			Get
				Return size
			End Get
		End Property

		''' <summary>
		''' Returns the offset of the default bank in array elements. </summary>
		'''  <returns> the offset of the default bank. </returns>
		Public Overridable Property offset As Integer
			Get
				Return offset
			End Get
		End Property

		''' <summary>
		''' Returns the offsets (in array elements) of all the banks. </summary>
		'''  <returns> the offsets of all banks. </returns>
		Public Overridable Property offsets As Integer()
			Get
				Return CType(offsets.clone(), Integer())
			End Get
		End Property

		''' <summary>
		''' Returns the number of banks in this DataBuffer. </summary>
		'''  <returns> the number of banks. </returns>
		Public Overridable Property numBanks As Integer
			Get
				Return banks
			End Get
		End Property

		''' <summary>
		''' Returns the requested data array element from the first (default) bank
		''' as an  java.lang.[Integer]. </summary>
		''' <param name="i"> the index of the requested data array element </param>
		''' <returns> the data array element at the specified index. </returns>
		''' <seealso cref= #setElem(int, int) </seealso>
		''' <seealso cref= #setElem(int, int, int) </seealso>
		Public Overridable Function getElem(ByVal i As Integer) As Integer
			Return getElem(0,i)
		End Function

		''' <summary>
		''' Returns the requested data array element from the specified bank
		''' as an  java.lang.[Integer]. </summary>
		''' <param name="bank"> the specified bank </param>
		''' <param name="i"> the index of the requested data array element </param>
		''' <returns> the data array element at the specified index from the
		'''         specified bank at the specified index. </returns>
		''' <seealso cref= #setElem(int, int) </seealso>
		''' <seealso cref= #setElem(int, int, int) </seealso>
		Public MustOverride Function getElem(ByVal bank As Integer, ByVal i As Integer) As Integer

		''' <summary>
		''' Sets the requested data array element in the first (default) bank
		''' from the given  java.lang.[Integer]. </summary>
		''' <param name="i"> the specified index into the data array </param>
		''' <param name="val"> the data to set the element at the specified index in
		''' the data array </param>
		''' <seealso cref= #getElem(int) </seealso>
		''' <seealso cref= #getElem(int, int) </seealso>
		Public Overridable Sub setElem(ByVal i As Integer, ByVal val As Integer)
			elemlem(0,i,val)
		End Sub

		''' <summary>
		''' Sets the requested data array element in the specified bank
		''' from the given  java.lang.[Integer]. </summary>
		''' <param name="bank"> the specified bank </param>
		''' <param name="i"> the specified index into the data array </param>
		''' <param name="val">  the data to set the element in the specified bank
		''' at the specified index in the data array </param>
		''' <seealso cref= #getElem(int) </seealso>
		''' <seealso cref= #getElem(int, int) </seealso>
		Public MustOverride Sub setElem(ByVal bank As Integer, ByVal i As Integer, ByVal val As Integer)

		''' <summary>
		''' Returns the requested data array element from the first (default) bank
		''' as a float.  The implementation in this class is to cast getElem(i)
		''' to a float.  Subclasses may override this method if another
		''' implementation is needed. </summary>
		''' <param name="i"> the index of the requested data array element </param>
		''' <returns> a float value representing the data array element at the
		'''  specified index. </returns>
		''' <seealso cref= #setElemFloat(int, float) </seealso>
		''' <seealso cref= #setElemFloat(int, int, float) </seealso>
		Public Overridable Function getElemFloat(ByVal i As Integer) As Single
			Return CSng(getElem(i))
		End Function

		''' <summary>
		''' Returns the requested data array element from the specified bank
		''' as a float.  The implementation in this class is to cast
		''' <seealso cref="#getElem(int, int)"/>
		''' to a float.  Subclasses can override this method if another
		''' implementation is needed. </summary>
		''' <param name="bank"> the specified bank </param>
		''' <param name="i"> the index of the requested data array element </param>
		''' <returns> a float value representing the data array element from the
		''' specified bank at the specified index. </returns>
		''' <seealso cref= #setElemFloat(int, float) </seealso>
		''' <seealso cref= #setElemFloat(int, int, float) </seealso>
		Public Overridable Function getElemFloat(ByVal bank As Integer, ByVal i As Integer) As Single
			Return CSng(getElem(bank,i))
		End Function

		''' <summary>
		''' Sets the requested data array element in the first (default) bank
		''' from the given float.  The implementation in this class is to cast
		''' val to an int and call <seealso cref="#setElem(int, int)"/>.  Subclasses
		''' can override this method if another implementation is needed. </summary>
		''' <param name="i"> the specified index </param>
		''' <param name="val"> the value to set the element at the specified index in
		''' the data array </param>
		''' <seealso cref= #getElemFloat(int) </seealso>
		''' <seealso cref= #getElemFloat(int, int) </seealso>
		Public Overridable Sub setElemFloat(ByVal i As Integer, ByVal val As Single)
			elemlem(i,CInt(Fix(val)))
		End Sub

		''' <summary>
		''' Sets the requested data array element in the specified bank
		''' from the given float.  The implementation in this class is to cast
		''' val to an int and call <seealso cref="#setElem(int, int)"/>.  Subclasses can
		''' override this method if another implementation is needed. </summary>
		''' <param name="bank"> the specified bank </param>
		''' <param name="i"> the specified index </param>
		''' <param name="val"> the value to set the element in the specified bank at
		''' the specified index in the data array </param>
		''' <seealso cref= #getElemFloat(int) </seealso>
		''' <seealso cref= #getElemFloat(int, int) </seealso>
		Public Overridable Sub setElemFloat(ByVal bank As Integer, ByVal i As Integer, ByVal val As Single)
			elemlem(bank,i,CInt(Fix(val)))
		End Sub

		''' <summary>
		''' Returns the requested data array element from the first (default) bank
		''' as a java.lang.[Double].  The implementation in this class is to cast
		''' <seealso cref="#getElem(int)"/>
		''' to a java.lang.[Double].  Subclasses can override this method if another
		''' implementation is needed. </summary>
		''' <param name="i"> the specified index </param>
		''' <returns> a double value representing the element at the specified
		''' index in the data array. </returns>
		''' <seealso cref= #setElemDouble(int, double) </seealso>
		''' <seealso cref= #setElemDouble(int, int, double) </seealso>
		Public Overridable Function getElemDouble(ByVal i As Integer) As Double
			Return CDbl(getElem(i))
		End Function

		''' <summary>
		''' Returns the requested data array element from the specified bank as
		''' a java.lang.[Double].  The implementation in this class is to cast getElem(bank, i)
		''' to a java.lang.[Double].  Subclasses may override this method if another
		''' implementation is needed. </summary>
		''' <param name="bank"> the specified bank </param>
		''' <param name="i"> the specified index </param>
		''' <returns> a double value representing the element from the specified
		''' bank at the specified index in the data array. </returns>
		''' <seealso cref= #setElemDouble(int, double) </seealso>
		''' <seealso cref= #setElemDouble(int, int, double) </seealso>
		Public Overridable Function getElemDouble(ByVal bank As Integer, ByVal i As Integer) As Double
			Return CDbl(getElem(bank,i))
		End Function

		''' <summary>
		''' Sets the requested data array element in the first (default) bank
		''' from the given java.lang.[Double].  The implementation in this class is to cast
		''' val to an int and call <seealso cref="#setElem(int, int)"/>.  Subclasses can
		''' override this method if another implementation is needed. </summary>
		''' <param name="i"> the specified index </param>
		''' <param name="val"> the value to set the element at the specified index
		''' in the data array </param>
		''' <seealso cref= #getElemDouble(int) </seealso>
		''' <seealso cref= #getElemDouble(int, int) </seealso>
		Public Overridable Sub setElemDouble(ByVal i As Integer, ByVal val As Double)
			elemlem(i,CInt(Fix(val)))
		End Sub

		''' <summary>
		''' Sets the requested data array element in the specified bank
		''' from the given java.lang.[Double].  The implementation in this class is to cast
		''' val to an int and call <seealso cref="#setElem(int, int)"/>.  Subclasses can
		''' override this method if another implementation is needed. </summary>
		''' <param name="bank"> the specified bank </param>
		''' <param name="i"> the specified index </param>
		''' <param name="val"> the value to set the element in the specified bank
		''' at the specified index of the data array </param>
		''' <seealso cref= #getElemDouble(int) </seealso>
		''' <seealso cref= #getElemDouble(int, int) </seealso>
		Public Overridable Sub setElemDouble(ByVal bank As Integer, ByVal i As Integer, ByVal val As Double)
			elemlem(bank,i,CInt(Fix(val)))
		End Sub

		Friend Shared Function toIntArray(ByVal obj As Object) As Integer()
			If TypeOf obj Is Integer() Then
				Return CType(obj, Integer())
			ElseIf obj Is Nothing Then
				Return Nothing
			ElseIf TypeOf obj Is Short() Then
				Dim sdata As Short() = CType(obj, Short())
				Dim idata As Integer() = New Integer(sdata.Length - 1){}
				For i As Integer = 0 To sdata.Length - 1
					idata(i) = CInt(sdata(i)) And &Hffff
				Next i
				Return idata
			ElseIf TypeOf obj Is SByte() Then
				Dim bdata As SByte() = CType(obj, SByte())
				Dim idata As Integer() = New Integer(bdata.Length - 1){}
				For i As Integer = 0 To bdata.Length - 1
					idata(i) = &Hff And CInt(bdata(i))
				Next i
				Return idata
			End If
			Return Nothing
		End Function

		Shared Sub New()
'JAVA TO VB CONVERTER TODO TASK: Anonymous inner classes are not converted to VB if the base type is not defined in the code being converted:
'			sun.awt.image.SunWritableRaster.setDataStealer(New sun.awt.image.SunWritableRaster.DataStealer()
	'		{
	'			public byte[] getData(DataBufferByte dbb, int bank)
	'			{
	'				Return dbb.bankdata[bank];
	'			}
	'
	'			public short[] getData(DataBufferUShort dbus, int bank)
	'			{
	'				Return dbus.bankdata[bank];
	'			}
	'
	'			public int[] getData(DataBufferInt dbi, int bank)
	'			{
	'				Return dbi.bankdata[bank];
	'			}
	'
	'			public StateTrackableDelegate getTrackable(DataBuffer db)
	'			{
	'				Return db.theTrackable;
	'			}
	'
	'			public  Sub  setTrackable(DataBuffer db, StateTrackableDelegate trackable)
	'			{
	'				db.theTrackable = trackable;
	'			}
	'		});
		End Sub
	End Class

End Namespace