Imports System

'
' * Copyright (c) 1994, 2013, Oracle and/or its affiliates. All rights reserved.
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

Namespace java.io

	''' <summary>
	''' A <code>ByteArrayInputStream</code> contains
	''' an internal buffer that contains bytes that
	''' may be read from the stream. An internal
	''' counter keeps track of the next byte to
	''' be supplied by the <code>read</code> method.
	''' <p>
	''' Closing a <tt>ByteArrayInputStream</tt> has no effect. The methods in
	''' this class can be called after the stream has been closed without
	''' generating an <tt>IOException</tt>.
	''' 
	''' @author  Arthur van Hoff </summary>
	''' <seealso cref=     java.io.StringBufferInputStream
	''' @since   JDK1.0 </seealso>
	Public Class ByteArrayInputStream
		Inherits InputStream

		''' <summary>
		''' An array of bytes that was provided
		''' by the creator of the stream. Elements <code>buf[0]</code>
		''' through <code>buf[count-1]</code> are the
		''' only bytes that can ever be read from the
		''' stream;  element <code>buf[pos]</code> is
		''' the next byte to be read.
		''' </summary>
		Protected Friend buf As SByte()

		''' <summary>
		''' The index of the next character to read from the input stream buffer.
		''' This value should always be nonnegative
		''' and not larger than the value of <code>count</code>.
		''' The next byte to be read from the input stream buffer
		''' will be <code>buf[pos]</code>.
		''' </summary>
		Protected Friend pos As Integer

		''' <summary>
		''' The currently marked position in the stream.
		''' ByteArrayInputStream objects are marked at position zero by
		''' default when constructed.  They may be marked at another
		''' position within the buffer by the <code>mark()</code> method.
		''' The current buffer position is set to this point by the
		''' <code>reset()</code> method.
		''' <p>
		''' If no mark has been set, then the value of mark is the offset
		''' passed to the constructor (or 0 if the offset was not supplied).
		''' 
		''' @since   JDK1.1
		''' </summary>
		Protected Friend mark_Renamed As Integer = 0

		''' <summary>
		''' The index one greater than the last valid character in the input
		''' stream buffer.
		''' This value should always be nonnegative
		''' and not larger than the length of <code>buf</code>.
		''' It  is one greater than the position of
		''' the last byte within <code>buf</code> that
		''' can ever be read  from the input stream buffer.
		''' </summary>
		Protected Friend count As Integer

		''' <summary>
		''' Creates a <code>ByteArrayInputStream</code>
		''' so that it  uses <code>buf</code> as its
		''' buffer array.
		''' The buffer array is not copied.
		''' The initial value of <code>pos</code>
		''' is <code>0</code> and the initial value
		''' of  <code>count</code> is the length of
		''' <code>buf</code>.
		''' </summary>
		''' <param name="buf">   the input buffer. </param>
		Public Sub New(  buf As SByte())
			Me.buf = buf
			Me.pos = 0
			Me.count = buf.Length
		End Sub

        ''' <summary>
        ''' Creates <code>ByteArrayInputStream</code>
        ''' that uses <code>buf</code> as its
        ''' buffer array. The initial value of <code>pos</code>
        ''' is <code>offset</code> and the initial value
        ''' of <code>count</code> is the minimum of <code>offset+length</code>
        ''' and <code>buf.length</code>.
        ''' The buffer array is not copied. The buffer's mark is
        ''' set to the specified offset.
        ''' </summary>
        ''' <param name="buf">      the input buffer. </param>
        ''' <param name="offset">   the offset in the buffer of the first byte to read. </param>
        ''' <param name="length">   the maximum number of bytes to read from the buffer. </param>
        Public Sub New(buf() As Byte, offset As Integer, length As Integer)
            Me.buf = buf
            Me.pos = offset
            Me.count = System.Math.Min(offset + length, buf.Length)
            Me.mark_Renamed = offset
        End Sub

        ''' <summary>
        ''' Reads the next byte of data from this input stream. The value
        ''' byte is returned as an <code>int</code> in the range
        ''' <code>0</code> to <code>255</code>. If no byte is available
        ''' because the end of the stream has been reached, the value
        ''' <code>-1</code> is returned.
        ''' <p>
        ''' This <code>read</code> method
        ''' cannot block.
        ''' </summary>
        ''' <returns>  the next byte of data, or <code>-1</code> if the end of the
        '''          stream has been reached. </returns>
        Public  Integer read()
				Dim tempVar As Integer = pos
				pos += 1
				Return If(pos < count, (buf(tempVar) And &Hff), -1)

		''' <summary>
		''' Reads up to <code>len</code> bytes of data into an array of bytes
		''' from this input stream.
		''' If <code>pos</code> equals <code>count</code>,
		''' then <code>-1</code> is returned to indicate
		''' end of file. Otherwise, the  number <code>k</code>
		''' of bytes read is equal to the smaller of
		''' <code>len</code> and <code>count-pos</code>.
		''' If <code>k</code> is positive, then bytes
		''' <code>buf[pos]</code> through <code>buf[pos+k-1]</code>
		''' are copied into <code>b[off]</code>  through
		''' <code>b[off+k-1]</code> in the manner performed
		''' by <code>System.arraycopy</code>. The
		''' value <code>k</code> is added into <code>pos</code>
		''' and <code>k</code> is returned.
		''' <p>
		''' This <code>read</code> method cannot block.
		''' </summary>
		''' <param name="b">     the buffer into which the data is read. </param>
		''' <param name="off">   the start offset in the destination array <code>b</code> </param>
		''' <param name="len">   the maximum number of bytes read. </param>
		''' <returns>  the total number of bytes read into the buffer, or
		'''          <code>-1</code> if there is no more data because the end of
		'''          the stream has been reached. </returns>
		''' <exception cref="NullPointerException"> If <code>b</code> is <code>null</code>. </exception>
		''' <exception cref="IndexOutOfBoundsException"> If <code>off</code> is negative,
		''' <code>len</code> is negative, or <code>len</code> is greater than
		''' <code>b.length - off</code> </exception>
		Public  Integer read(SByte b() , Integer off, Integer len)
			If b Is Nothing Then
				Throw New NullPointerException
			ElseIf off < 0 OrElse len < 0 OrElse len > b.length - off Then
				Throw New IndexOutOfBoundsException
			End If

			If pos >= count Then Return -1

			Dim avail As Integer = count - pos
			If len > avail Then len = avail
			If len <= 0 Then Return 0
			Array.Copy(buf, pos, b, off, len)
			pos += len
			Return len

		''' <summary>
		''' Skips <code>n</code> bytes of input from this input stream. Fewer
		''' bytes might be skipped if the end of the input stream is reached.
		''' The actual number <code>k</code>
		''' of bytes to be skipped is equal to the smaller
		''' of <code>n</code> and  <code>count-pos</code>.
		''' The value <code>k</code> is added into <code>pos</code>
		''' and <code>k</code> is returned.
		''' </summary>
		''' <param name="n">   the number of bytes to be skipped. </param>
		''' <returns>  the actual number of bytes skipped. </returns>
		Public  Long skip(Long n)
			Dim k As Long = count - pos
			If n < k Then k = If(n < 0, 0, n)

			pos += k
			Return k

		''' <summary>
		''' Returns the number of remaining bytes that can be read (or skipped over)
		''' from this input stream.
		''' <p>
		''' The value returned is <code>count&nbsp;- pos</code>,
		''' which is the number of bytes remaining to be read from the input buffer.
		''' </summary>
		''' <returns>  the number of remaining bytes that can be read (or skipped
		'''          over) from this input stream without blocking. </returns>
		Public  Integer available()
			Return count - pos

		''' <summary>
		''' Tests if this <code>InputStream</code> supports mark/reset. The
		''' <code>markSupported</code> method of <code>ByteArrayInputStream</code>
		''' always returns <code>true</code>.
		''' 
		''' @since   JDK1.1
		''' </summary>
		public Boolean markSupported()
			Return True

		''' <summary>
		''' Set the current marked position in the stream.
		''' ByteArrayInputStream objects are marked at position zero by
		''' default when constructed.  They may be marked at another
		''' position within the buffer by this method.
		''' <p>
		''' If no mark has been set, then the value of the mark is the
		''' offset passed to the constructor (or 0 if the offset was not
		''' supplied).
		''' 
		''' <p> Note: The <code>readAheadLimit</code> for this class
		'''  has no meaning.
		''' 
		''' @since   JDK1.1
		''' </summary>
		public  Sub  mark(Integer readAheadLimit)
			mark_Renamed = pos

		''' <summary>
		''' Resets the buffer to the marked position.  The marked position
		''' is 0 unless another position was marked or an offset was specified
		''' in the constructor.
		''' </summary>
		Public   Sub  reset()
			pos = mark_Renamed

		''' <summary>
		''' Closing a <tt>ByteArrayInputStream</tt> has no effect. The methods in
		''' this class can be called after the stream has been closed without
		''' generating an <tt>IOException</tt>.
		''' </summary>
		public  Sub  close() throws IOException

	End Class

End Namespace