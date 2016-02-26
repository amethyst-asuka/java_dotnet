'
' * Copyright (c) 1999, 2001, Oracle and/or its affiliates. All rights reserved.
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

Namespace javax.imageio.stream

	''' <summary>
	''' A class representing a mutable reference to an array of bytes and
	''' an offset and length within that array.  <code>IIOByteBuffer</code>
	''' is used by <code>ImageInputStream</code> to supply a sequence of bytes
	''' to the caller, possibly with fewer copies than using the conventional
	''' <code>read</code> methods that take a user-supplied byte array.
	''' 
	''' <p> The byte array referenced by an <code>IIOByteBuffer</code> will
	''' generally be part of an internal data structure belonging to an
	''' <code>ImageReader</code> implementation; its contents should be
	''' considered read-only and must not be modified.
	''' 
	''' </summary>
	Public Class IIOByteBuffer

		Private data As SByte()

		Private offset As Integer

		Private length As Integer

		''' <summary>
		''' Constructs an <code>IIOByteBuffer</code> that references a
		''' given byte array, offset, and length.
		''' </summary>
		''' <param name="data"> a byte array. </param>
		''' <param name="offset"> an int offset within the array. </param>
		''' <param name="length"> an int specifying the length of the data of
		''' interest within byte array, in bytes. </param>
		Public Sub New(ByVal data As SByte(), ByVal offset As Integer, ByVal length As Integer)
			Me.data = data
			Me.offset = offset
			Me.length = length
		End Sub

		''' <summary>
		''' Returns a reference to the byte array.  The returned value should
		''' be treated as read-only, and only the portion specified by the
		''' values of <code>getOffset</code> and <code>getLength</code> should
		''' be used.
		''' </summary>
		''' <returns> a byte array reference.
		''' </returns>
		''' <seealso cref= #getOffset </seealso>
		''' <seealso cref= #getLength </seealso>
		''' <seealso cref= #setData </seealso>
		Public Overridable Property data As SByte()
			Get
				Return data
			End Get
			Set(ByVal data As SByte())
				Me.data = data
			End Set
		End Property


		''' <summary>
		''' Returns the offset within the byte array returned by
		''' <code>getData</code> at which the data of interest start.
		''' </summary>
		''' <returns> an int offset.
		''' </returns>
		''' <seealso cref= #getData </seealso>
		''' <seealso cref= #getLength </seealso>
		''' <seealso cref= #setOffset </seealso>
		Public Overridable Property offset As Integer
			Get
				Return offset
			End Get
			Set(ByVal offset As Integer)
				Me.offset = offset
			End Set
		End Property


		''' <summary>
		''' Returns the length of the data of interest within the byte
		''' array returned by <code>getData</code>.
		''' </summary>
		''' <returns> an int length.
		''' </returns>
		''' <seealso cref= #getData </seealso>
		''' <seealso cref= #getOffset </seealso>
		''' <seealso cref= #setLength </seealso>
		Public Overridable Property length As Integer
			Get
				Return length
			End Get
			Set(ByVal length As Integer)
				Me.length = length
			End Set
		End Property

	End Class

End Namespace