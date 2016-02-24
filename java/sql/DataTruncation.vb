'
' * Copyright (c) 1996, 2013, Oracle and/or its affiliates. All rights reserved.
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

Namespace java.sql

	''' <summary>
	''' An exception  thrown as a <code>DataTruncation</code> exception
	''' (on writes) or reported as a
	''' <code>DataTruncation</code> warning (on reads)
	'''  when a data values is unexpectedly truncated for reasons other than its having
	'''  exceeded <code>MaxFieldSize</code>.
	''' 
	''' <P>The SQLstate for a <code>DataTruncation</code> during read is <code>01004</code>.
	''' <P>The SQLstate for a <code>DataTruncation</code> during write is <code>22001</code>.
	''' </summary>

	Public Class DataTruncation
		Inherits SQLWarning

		''' <summary>
		''' Creates a <code>DataTruncation</code> object
		''' with the SQLState initialized
		''' to 01004 when <code>read</code> is set to <code>true</code> and 22001
		''' when <code>read</code> is set to <code>false</code>,
		''' the reason set to "Data truncation", the
		''' vendor code set to 0, and
		''' the other fields set to the given values.
		''' The <code>cause</code> is not initialized, and may subsequently be
		''' initialized by a call to the
		''' <seealso cref="Throwable#initCause(java.lang.Throwable)"/> method.
		''' <p>
		''' </summary>
		''' <param name="index"> The index of the parameter or column value </param>
		''' <param name="parameter"> true if a parameter value was truncated </param>
		''' <param name="read"> true if a read was truncated </param>
		''' <param name="dataSize"> the original size of the data </param>
		''' <param name="transferSize"> the size after truncation </param>
		Public Sub New(ByVal index As Integer, ByVal parameter As Boolean, ByVal read As Boolean, ByVal dataSize As Integer, ByVal transferSize As Integer)
			MyBase.New("Data truncation",If(read = True, "01004", "22001"))
			Me.index = index
			Me.parameter = parameter
			Me.read = read
			Me.dataSize = dataSize
			Me.transferSize = transferSize

		End Sub

		''' <summary>
		''' Creates a <code>DataTruncation</code> object
		''' with the SQLState initialized
		''' to 01004 when <code>read</code> is set to <code>true</code> and 22001
		''' when <code>read</code> is set to <code>false</code>,
		''' the reason set to "Data truncation", the
		''' vendor code set to 0, and
		''' the other fields set to the given values.
		''' <p>
		''' </summary>
		''' <param name="index"> The index of the parameter or column value </param>
		''' <param name="parameter"> true if a parameter value was truncated </param>
		''' <param name="read"> true if a read was truncated </param>
		''' <param name="dataSize"> the original size of the data </param>
		''' <param name="transferSize"> the size after truncation </param>
		''' <param name="cause"> the underlying reason for this <code>DataTruncation</code>
		''' (which is saved for later retrieval by the <code>getCause()</code> method);
		''' may be null indicating the cause is non-existent or unknown.
		''' 
		''' @since 1.6 </param>
		Public Sub New(ByVal index As Integer, ByVal parameter As Boolean, ByVal read As Boolean, ByVal dataSize As Integer, ByVal transferSize As Integer, ByVal cause As Throwable)
			MyBase.New("Data truncation",If(read = True, "01004", "22001"),cause)
			Me.index = index
			Me.parameter = parameter
			Me.read = read
			Me.dataSize = dataSize
			Me.transferSize = transferSize
		End Sub

		''' <summary>
		''' Retrieves the index of the column or parameter that was truncated.
		''' 
		''' <P>This may be -1 if the column or parameter index is unknown, in
		''' which case the <code>parameter</code> and <code>read</code> fields should be ignored.
		''' </summary>
		''' <returns> the index of the truncated parameter or column value </returns>
		Public Overridable Property index As Integer
			Get
				Return index
			End Get
		End Property

		''' <summary>
		''' Indicates whether the value truncated was a parameter value or
		''' a column value.
		''' </summary>
		''' <returns> <code>true</code> if the value truncated was a parameter;
		'''         <code>false</code> if it was a column value </returns>
		Public Overridable Property parameter As Boolean
			Get
				Return parameter
			End Get
		End Property

		''' <summary>
		''' Indicates whether or not the value was truncated on a read.
		''' </summary>
		''' <returns> <code>true</code> if the value was truncated when read from
		'''         the database; <code>false</code> if the data was truncated on a write </returns>
		Public Overridable Property read As Boolean
			Get
				Return read
			End Get
		End Property

		''' <summary>
		''' Gets the number of bytes of data that should have been transferred.
		''' This number may be approximate if data conversions were being
		''' performed.  The value may be <code>-1</code> if the size is unknown.
		''' </summary>
		''' <returns> the number of bytes of data that should have been transferred </returns>
		Public Overridable Property dataSize As Integer
			Get
				Return dataSize
			End Get
		End Property

		''' <summary>
		''' Gets the number of bytes of data actually transferred.
		''' The value may be <code>-1</code> if the size is unknown.
		''' </summary>
		''' <returns> the number of bytes of data actually transferred </returns>
		Public Overridable Property transferSize As Integer
			Get
				Return transferSize
			End Get
		End Property

			''' <summary>
			''' @serial
			''' </summary>
		Private index As Integer

			''' <summary>
			''' @serial
			''' </summary>
		Private parameter As Boolean

			''' <summary>
			''' @serial
			''' </summary>
		Private read As Boolean

			''' <summary>
			''' @serial
			''' </summary>
		Private dataSize As Integer

			''' <summary>
			''' @serial
			''' </summary>
		Private transferSize As Integer

		''' <summary>
		''' @serial
		''' </summary>
		Private Shadows Const serialVersionUID As Long = 6464298989504059473L

	End Class

End Namespace