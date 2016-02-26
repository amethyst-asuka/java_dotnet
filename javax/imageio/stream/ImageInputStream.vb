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

Namespace javax.imageio.stream


	''' <summary>
	''' A seekable input stream interface for use by
	''' <code>ImageReader</code>s.  Various input sources, such as
	''' <code>InputStream</code>s and <code>File</code>s,
	''' as well as future fast I/O sources may be "wrapped" by a suitable
	''' implementation of this interface for use by the Image I/O API.
	''' </summary>
	''' <seealso cref= ImageInputStreamImpl </seealso>
	''' <seealso cref= FileImageInputStream </seealso>
	''' <seealso cref= FileCacheImageInputStream </seealso>
	''' <seealso cref= MemoryCacheImageInputStream
	'''  </seealso>
	Public Interface ImageInputStream
		Inherits java.io.DataInput, java.io.Closeable

		''' <summary>
		''' Sets the desired byte order for future reads of data values
		''' from this stream.  For example, the sequence of bytes '0x01
		''' 0x02 0x03 0x04' if read as a 4-byte integer would have the
		''' value '0x01020304' using network byte order and the value
		''' '0x04030201' under the reverse byte order.
		''' 
		''' <p> The enumeration class <code>java.nio.ByteOrder</code> is
		''' used to specify the byte order.  A value of
		''' <code>ByteOrder.BIG_ENDIAN</code> specifies so-called
		''' big-endian or network byte order, in which the high-order byte
		''' comes first.  Motorola and Sparc processors store data in this
		''' format, while Intel processors store data in the reverse
		''' <code>ByteOrder.LITTLE_ENDIAN</code> order.
		''' 
		''' <p> The byte order has no effect on the results returned from
		''' the <code>readBits</code> method (or the value written by
		''' <code>ImageOutputStream.writeBits</code>).
		''' </summary>
		''' <param name="byteOrder"> one of <code>ByteOrder.BIG_ENDIAN</code> or
		''' <code>java.nio.ByteOrder.LITTLE_ENDIAN</code>, indicating whether
		''' network byte order or its reverse will be used for future
		''' reads.
		''' </param>
		''' <seealso cref= java.nio.ByteOrder </seealso>
		''' <seealso cref= #getByteOrder </seealso>
		''' <seealso cref= #readBits(int) </seealso>
		Property byteOrder As java.nio.ByteOrder


		''' <summary>
		''' Reads a single byte from the stream and returns it as an
		''' integer between 0 and 255.  If the end of the stream is
		''' reached, -1 is returned.
		''' 
		''' <p> The bit offset within the stream is reset to zero before
		''' the read occurs.
		''' </summary>
		''' <returns> a byte value from the stream, as an int, or -1 to
		''' indicate EOF.
		''' </returns>
		''' <exception cref="IOException"> if an I/O error occurs. </exception>
		Function read() As Integer

		''' <summary>
		''' Reads up to <code>b.length</code> bytes from the stream, and
		''' stores them into <code>b</code> starting at index 0.  The
		''' number of bytes read is returned.  If no bytes can be read
		''' because the end of the stream has been reached, -1 is returned.
		''' 
		''' <p> The bit offset within the stream is reset to zero before
		''' the read occurs.
		''' </summary>
		''' <param name="b"> an array of bytes to be written to.
		''' </param>
		''' <returns> the number of bytes actually read, or <code>-1</code>
		''' to indicate EOF.
		''' </returns>
		''' <exception cref="NullPointerException"> if <code>b</code> is
		''' <code>null</code>.
		''' </exception>
		''' <exception cref="IOException"> if an I/O error occurs. </exception>
		Function read(ByVal b As SByte()) As Integer

		''' <summary>
		''' Reads up to <code>len</code> bytes from the stream, and stores
		''' them into <code>b</code> starting at index <code>off</code>.
		''' The number of bytes read is returned.  If no bytes can be read
		''' because the end of the stream has been reached, <code>-1</code>
		''' is returned.
		''' 
		''' <p> The bit offset within the stream is reset to zero before
		''' the read occurs.
		''' </summary>
		''' <param name="b"> an array of bytes to be written to. </param>
		''' <param name="off"> the starting position within <code>b</code> to write to. </param>
		''' <param name="len"> the maximum number of <code>byte</code>s to read.
		''' </param>
		''' <returns> the number of bytes actually read, or <code>-1</code>
		''' to indicate EOF.
		''' </returns>
		''' <exception cref="NullPointerException"> if <code>b</code> is
		''' <code>null</code>. </exception>
		''' <exception cref="IndexOutOfBoundsException"> if <code>off</code> is
		''' negative, <code>len</code> is negative, or <code>off +
		''' len</code> is greater than <code>b.length</code>. </exception>
		''' <exception cref="IOException"> if an I/O error occurs. </exception>
		Function read(ByVal b As SByte(), ByVal [off] As Integer, ByVal len As Integer) As Integer

		''' <summary>
		''' Reads up to <code>len</code> bytes from the stream, and
		''' modifies the supplied <code>IIOByteBuffer</code> to indicate
		''' the byte array, offset, and length where the data may be found.
		''' The caller should not attempt to modify the data found in the
		''' <code>IIOByteBuffer</code>.
		''' 
		''' <p> The bit offset within the stream is reset to zero before
		''' the read occurs.
		''' </summary>
		''' <param name="buf"> an IIOByteBuffer object to be modified. </param>
		''' <param name="len"> the maximum number of <code>byte</code>s to read.
		''' </param>
		''' <exception cref="IndexOutOfBoundsException"> if <code>len</code> is
		''' negative. </exception>
		''' <exception cref="NullPointerException"> if <code>buf</code> is
		''' <code>null</code>.
		''' </exception>
		''' <exception cref="IOException"> if an I/O error occurs. </exception>
		Sub readBytes(ByVal buf As IIOByteBuffer, ByVal len As Integer)

		''' <summary>
		''' Reads a byte from the stream and returns a <code>boolean</code>
		''' value of <code>true</code> if it is nonzero, <code>false</code>
		''' if it is zero.
		''' 
		''' <p> The bit offset within the stream is reset to zero before
		''' the read occurs.
		''' </summary>
		''' <returns> a boolean value from the stream.
		''' </returns>
		''' <exception cref="java.io.EOFException"> if the end of the stream is reached. </exception>
		''' <exception cref="IOException"> if an I/O error occurs. </exception>
		Function readBoolean() As Boolean

		''' <summary>
		''' Reads a byte from the stream and returns it as a
		''' <code>byte</code> value.  Byte values between <code>0x00</code>
		''' and <code>0x7f</code> represent integer values between
		''' <code>0</code> and <code>127</code>.  Values between
		''' <code>0x80</code> and <code>0xff</code> represent negative
		''' values from <code>-128</code> to <code>/1</code>.
		''' 
		''' <p> The bit offset within the stream is reset to zero before
		''' the read occurs.
		''' </summary>
		''' <returns> a signed byte value from the stream.
		''' </returns>
		''' <exception cref="java.io.EOFException"> if the end of the stream is reached. </exception>
		''' <exception cref="IOException"> if an I/O error occurs. </exception>
		Function readByte() As SByte

		''' <summary>
		''' Reads a byte from the stream, and (conceptually) converts it to
		''' an int, masks it with <code>0xff</code> in order to strip off
		''' any sign-extension bits, and returns it as a <code>byte</code>
		''' value.
		''' 
		''' <p> Thus, byte values between <code>0x00</code> and
		''' <code>0x7f</code> are simply returned as integer values between
		''' <code>0</code> and <code>127</code>.  Values between
		''' <code>0x80</code> and <code>0xff</code>, which normally
		''' represent negative <code>byte</code>values, will be mapped into
		''' positive integers between <code>128</code> and
		''' <code>255</code>.
		''' 
		''' <p> The bit offset within the stream is reset to zero before
		''' the read occurs.
		''' </summary>
		''' <returns> an unsigned byte value from the stream.
		''' </returns>
		''' <exception cref="java.io.EOFException"> if the end of the stream is reached. </exception>
		''' <exception cref="IOException"> if an I/O error occurs. </exception>
		Function readUnsignedByte() As Integer

		''' <summary>
		''' Reads two bytes from the stream, and (conceptually)
		''' concatenates them according to the current byte order, and
		''' returns the result as a <code>short</code> value.
		''' 
		''' <p> The bit offset within the stream is reset to zero before
		''' the read occurs.
		''' </summary>
		''' <returns> a signed short value from the stream.
		''' </returns>
		''' <exception cref="java.io.EOFException"> if the stream reaches the end before
		''' reading all the bytes. </exception>
		''' <exception cref="IOException"> if an I/O error occurs.
		''' </exception>
		''' <seealso cref= #getByteOrder </seealso>
		Function readShort() As Short

		''' <summary>
		''' Reads two bytes from the stream, and (conceptually)
		''' concatenates them according to the current byte order, converts
		''' the resulting value to an <code>int</code>, masks it with
		''' <code>0xffff</code> in order to strip off any sign-extension
		''' buts, and returns the result as an unsigned <code>int</code>
		''' value.
		''' 
		''' <p> The bit offset within the stream is reset to zero before
		''' the read occurs.
		''' </summary>
		''' <returns> an unsigned short value from the stream, as an int.
		''' </returns>
		''' <exception cref="java.io.EOFException"> if the stream reaches the end before
		''' reading all the bytes. </exception>
		''' <exception cref="IOException"> if an I/O error occurs.
		''' </exception>
		''' <seealso cref= #getByteOrder </seealso>
		Function readUnsignedShort() As Integer

		''' <summary>
		''' Equivalent to <code>readUnsignedShort</code>, except that the
		''' result is returned using the <code>char</code> datatype.
		''' 
		''' <p> The bit offset within the stream is reset to zero before
		''' the read occurs.
		''' </summary>
		''' <returns> an unsigned char value from the stream.
		''' </returns>
		''' <exception cref="java.io.EOFException"> if the stream reaches the end before
		''' reading all the bytes. </exception>
		''' <exception cref="IOException"> if an I/O error occurs.
		''' </exception>
		''' <seealso cref= #readUnsignedShort </seealso>
		Function readChar() As Char

		''' <summary>
		''' Reads 4 bytes from the stream, and (conceptually) concatenates
		''' them according to the current byte order and returns the result
		''' as an <code>int</code>.
		''' 
		''' <p> The bit offset within the stream is ignored and treated as
		''' though it were zero.
		''' </summary>
		''' <returns> a signed int value from the stream.
		''' </returns>
		''' <exception cref="java.io.EOFException"> if the stream reaches the end before
		''' reading all the bytes. </exception>
		''' <exception cref="IOException"> if an I/O error occurs.
		''' </exception>
		''' <seealso cref= #getByteOrder </seealso>
		Function readInt() As Integer

		''' <summary>
		''' Reads 4 bytes from the stream, and (conceptually) concatenates
		''' them according to the current byte order, converts the result
		''' to a long, masks it with <code>0xffffffffL</code> in order to
		''' strip off any sign-extension bits, and returns the result as an
		''' unsigned <code>long</code> value.
		''' 
		''' <p> The bit offset within the stream is reset to zero before
		''' the read occurs.
		''' </summary>
		''' <returns> an unsigned int value from the stream, as a long.
		''' </returns>
		''' <exception cref="java.io.EOFException"> if the stream reaches the end before
		''' reading all the bytes. </exception>
		''' <exception cref="IOException"> if an I/O error occurs.
		''' </exception>
		''' <seealso cref= #getByteOrder </seealso>
		Function readUnsignedInt() As Long

		''' <summary>
		''' Reads 8 bytes from the stream, and (conceptually) concatenates
		''' them according to the current byte order and returns the result
		''' as a <code>long</code>.
		''' 
		''' <p> The bit offset within the stream is reset to zero before
		''' the read occurs.
		''' </summary>
		''' <returns> a signed long value from the stream.
		''' </returns>
		''' <exception cref="java.io.EOFException"> if the stream reaches the end before
		''' reading all the bytes. </exception>
		''' <exception cref="IOException"> if an I/O error occurs.
		''' </exception>
		''' <seealso cref= #getByteOrder </seealso>
		Function readLong() As Long

		''' <summary>
		''' Reads 4 bytes from the stream, and (conceptually) concatenates
		''' them according to the current byte order and returns the result
		''' as a <code>float</code>.
		''' 
		''' <p> The bit offset within the stream is reset to zero before
		''' the read occurs.
		''' </summary>
		''' <returns> a float value from the stream.
		''' </returns>
		''' <exception cref="java.io.EOFException"> if the stream reaches the end before
		''' reading all the bytes. </exception>
		''' <exception cref="IOException"> if an I/O error occurs.
		''' </exception>
		''' <seealso cref= #getByteOrder </seealso>
		Function readFloat() As Single

		''' <summary>
		''' Reads 8 bytes from the stream, and (conceptually) concatenates
		''' them according to the current byte order and returns the result
		''' as a <code>double</code>.
		''' 
		''' <p> The bit offset within the stream is reset to zero before
		''' the read occurs.
		''' </summary>
		''' <returns> a double value from the stream.
		''' </returns>
		''' <exception cref="java.io.EOFException"> if the stream reaches the end before
		''' reading all the bytes. </exception>
		''' <exception cref="IOException"> if an I/O error occurs.
		''' </exception>
		''' <seealso cref= #getByteOrder </seealso>
		Function readDouble() As Double

		''' <summary>
		''' Reads the next line of text from the input stream.  It reads
		''' successive bytes, converting each byte separately into a
		''' character, until it encounters a line terminator or end of
		''' file; the characters read are then returned as a
		''' <code>String</code>. Note that because this method processes
		''' bytes, it does not support input of the full Unicode character
		''' set.
		''' 
		''' <p> If end of file is encountered before even one byte can be
		''' read, then <code>null</code> is returned. Otherwise, each byte
		''' that is read is converted to type <code>char</code> by
		''' zero-extension. If the character <code>'\n'</code> is
		''' encountered, it is discarded and reading ceases. If the
		''' character <code>'\r'</code> is encountered, it is discarded
		''' and, if the following byte converts &#32;to the character
		''' <code>'\n'</code>, then that is discarded also; reading then
		''' ceases. If end of file is encountered before either of the
		''' characters <code>'\n'</code> and <code>'\r'</code> is
		''' encountered, reading ceases. Once reading has ceased, a
		''' <code>String</code> is returned that contains all the
		''' characters read and not discarded, taken in order.  Note that
		''' every character in this string will have a value less than
		''' <code>&#92;u0100</code>, that is, <code>(char)256</code>.
		''' 
		''' <p> The bit offset within the stream is reset to zero before
		''' the read occurs.
		''' </summary>
		''' <returns> a String containing a line of text from the stream.
		''' </returns>
		''' <exception cref="IOException"> if an I/O error occurs. </exception>
		Function readLine() As String

		''' <summary>
		''' Reads in a string that has been encoded using a
		''' <a href="../../../java/io/DataInput.html#modified-utf-8">modified
		''' UTF-8</a>
		''' format.  The general contract of <code>readUTF</code> is that
		''' it reads a representation of a Unicode character string encoded
		''' in modified UTF-8 format; this string of characters is
		''' then returned as a <code>String</code>.
		''' 
		''' <p> First, two bytes are read and used to construct an unsigned
		''' 16-bit integer in the manner of the
		''' <code>readUnsignedShort</code> method, using network byte order
		''' (regardless of the current byte order setting). This integer
		''' value is called the <i>UTF length</i> and specifies the number
		''' of additional bytes to be read. These bytes are then converted
		''' to characters by considering them in groups. The length of each
		''' group is computed from the value of the first byte of the
		''' group. The byte following a group, if any, is the first byte of
		''' the next group.
		''' 
		''' <p> If the first byte of a group matches the bit pattern
		''' <code>0xxxxxxx</code> (where <code>x</code> means "may be
		''' <code>0</code> or <code>1</code>"), then the group consists of
		''' just that byte. The byte is zero-extended to form a character.
		''' 
		''' <p> If the first byte of a group matches the bit pattern
		''' <code>110xxxxx</code>, then the group consists of that byte
		''' <code>a</code> and a second byte <code>b</code>. If there is no
		''' byte <code>b</code> (because byte <code>a</code> was the last
		''' of the bytes to be read), or if byte <code>b</code> does not
		''' match the bit pattern <code>10xxxxxx</code>, then a
		''' <code>UTFDataFormatException</code> is thrown. Otherwise, the
		''' group is converted to the character:
		''' 
		''' <p> <pre><code>
		''' (char)(((a&amp; 0x1F) &lt;&lt; 6) | (b &amp; 0x3F))
		''' </code></pre>
		''' 
		''' If the first byte of a group matches the bit pattern
		''' <code>1110xxxx</code>, then the group consists of that byte
		''' <code>a</code> and two more bytes <code>b</code> and
		''' <code>c</code>.  If there is no byte <code>c</code> (because
		''' byte <code>a</code> was one of the last two of the bytes to be
		''' read), or either byte <code>b</code> or byte <code>c</code>
		''' does not match the bit pattern <code>10xxxxxx</code>, then a
		''' <code>UTFDataFormatException</code> is thrown. Otherwise, the
		''' group is converted to the character:
		''' 
		''' <p> <pre><code>
		''' (char)(((a &amp; 0x0F) &lt;&lt; 12) | ((b &amp; 0x3F) &lt;&lt; 6) | (c &amp; 0x3F))
		''' </code></pre>
		''' 
		''' If the first byte of a group matches the pattern
		''' <code>1111xxxx</code> or the pattern <code>10xxxxxx</code>,
		''' then a <code>UTFDataFormatException</code> is thrown.
		''' 
		''' <p> If end of file is encountered at any time during this
		''' entire process, then an <code>java.io.EOFException</code> is thrown.
		''' 
		''' <p> After every group has been converted to a character by this
		''' process, the characters are gathered, in the same order in
		''' which their corresponding groups were read from the input
		''' stream, to form a <code>String</code>, which is returned.
		''' 
		''' <p> The current byte order setting is ignored.
		''' 
		''' <p> The bit offset within the stream is reset to zero before
		''' the read occurs.
		''' 
		''' <p><strong>Note:</strong> This method should not be used in
		''' the  implementation of image formats that use standard UTF-8,
		''' because  the modified UTF-8 used here is incompatible with
		''' standard UTF-8.
		''' </summary>
		''' <returns> a String read from the stream.
		''' </returns>
		''' <exception cref="java.io.EOFException">  if this stream reaches the end
		''' before reading all the bytes. </exception>
		''' <exception cref="java.io.UTFDataFormatException"> if the bytes do not represent
		''' a valid modified UTF-8 encoding of a string. </exception>
		''' <exception cref="IOException"> if an I/O error occurs. </exception>
		Function readUTF() As String

		''' <summary>
		''' Reads <code>len</code> bytes from the stream, and stores them
		''' into <code>b</code> starting at index <code>off</code>.
		''' If the end of the stream is reached, an <code>java.io.EOFException</code>
		''' will be thrown.
		''' 
		''' <p> The bit offset within the stream is reset to zero before
		''' the read occurs.
		''' </summary>
		''' <param name="b"> an array of bytes to be written to. </param>
		''' <param name="off"> the starting position within <code>b</code> to write to. </param>
		''' <param name="len"> the maximum number of <code>byte</code>s to read.
		''' </param>
		''' <exception cref="IndexOutOfBoundsException"> if <code>off</code> is
		''' negative, <code>len</code> is negative, or <code>off +
		''' len</code> is greater than <code>b.length</code>. </exception>
		''' <exception cref="NullPointerException"> if <code>b</code> is
		''' <code>null</code>. </exception>
		''' <exception cref="java.io.EOFException"> if the stream reaches the end before
		''' reading all the bytes. </exception>
		''' <exception cref="IOException"> if an I/O error occurs. </exception>
		Sub readFully(ByVal b As SByte(), ByVal [off] As Integer, ByVal len As Integer)

		''' <summary>
		''' Reads <code>b.length</code> bytes from the stream, and stores them
		''' into <code>b</code> starting at index <code>0</code>.
		''' If the end of the stream is reached, an <code>java.io.EOFException</code>
		''' will be thrown.
		''' 
		''' <p> The bit offset within the stream is reset to zero before
		''' the read occurs.
		''' </summary>
		''' <param name="b"> an array of <code>byte</code>s.
		''' </param>
		''' <exception cref="NullPointerException"> if <code>b</code> is
		''' <code>null</code>. </exception>
		''' <exception cref="java.io.EOFException"> if the stream reaches the end before
		''' reading all the bytes. </exception>
		''' <exception cref="IOException"> if an I/O error occurs. </exception>
		Sub readFully(ByVal b As SByte())

		''' <summary>
		''' Reads <code>len</code> shorts (signed 16-bit integers) from the
		''' stream according to the current byte order, and
		''' stores them into <code>s</code> starting at index
		''' <code>off</code>.  If the end of the stream is reached, an
		''' <code>java.io.EOFException</code> will be thrown.
		''' 
		''' <p> The bit offset within the stream is reset to zero before
		''' the read occurs.
		''' </summary>
		''' <param name="s"> an array of shorts to be written to. </param>
		''' <param name="off"> the starting position within <code>s</code> to write to. </param>
		''' <param name="len"> the maximum number of <code>short</code>s to read.
		''' </param>
		''' <exception cref="IndexOutOfBoundsException"> if <code>off</code> is
		''' negative, <code>len</code> is negative, or <code>off +
		''' len</code> is greater than <code>s.length</code>. </exception>
		''' <exception cref="NullPointerException"> if <code>s</code> is
		''' <code>null</code>. </exception>
		''' <exception cref="java.io.EOFException"> if the stream reaches the end before
		''' reading all the bytes. </exception>
		''' <exception cref="IOException"> if an I/O error occurs. </exception>
		Sub readFully(ByVal s As Short(), ByVal [off] As Integer, ByVal len As Integer)

		''' <summary>
		''' Reads <code>len</code> chars (unsigned 16-bit integers) from the
		''' stream according to the current byte order, and
		''' stores them into <code>c</code> starting at index
		''' <code>off</code>.  If the end of the stream is reached, an
		''' <code>java.io.EOFException</code> will be thrown.
		''' 
		''' <p> The bit offset within the stream is reset to zero before
		''' the read occurs.
		''' </summary>
		''' <param name="c"> an array of chars to be written to. </param>
		''' <param name="off"> the starting position within <code>c</code> to write to. </param>
		''' <param name="len"> the maximum number of <code>char</code>s to read.
		''' </param>
		''' <exception cref="IndexOutOfBoundsException"> if <code>off</code> is
		''' negative, <code>len</code> is negative, or <code>off +
		''' len</code> is greater than <code>c.length</code>. </exception>
		''' <exception cref="NullPointerException"> if <code>c</code> is
		''' <code>null</code>. </exception>
		''' <exception cref="java.io.EOFException"> if the stream reaches the end before
		''' reading all the bytes. </exception>
		''' <exception cref="IOException"> if an I/O error occurs. </exception>
		Sub readFully(ByVal c As Char(), ByVal [off] As Integer, ByVal len As Integer)

		''' <summary>
		''' Reads <code>len</code> ints (signed 32-bit integers) from the
		''' stream according to the current byte order, and
		''' stores them into <code>i</code> starting at index
		''' <code>off</code>.  If the end of the stream is reached, an
		''' <code>java.io.EOFException</code> will be thrown.
		''' 
		''' <p> The bit offset within the stream is reset to zero before
		''' the read occurs.
		''' </summary>
		''' <param name="i"> an array of ints to be written to. </param>
		''' <param name="off"> the starting position within <code>i</code> to write to. </param>
		''' <param name="len"> the maximum number of <code>int</code>s to read.
		''' </param>
		''' <exception cref="IndexOutOfBoundsException"> if <code>off</code> is
		''' negative, <code>len</code> is negative, or <code>off +
		''' len</code> is greater than <code>i.length</code>. </exception>
		''' <exception cref="NullPointerException"> if <code>i</code> is
		''' <code>null</code>. </exception>
		''' <exception cref="java.io.EOFException"> if the stream reaches the end before
		''' reading all the bytes. </exception>
		''' <exception cref="IOException"> if an I/O error occurs. </exception>
		Sub readFully(ByVal i As Integer(), ByVal [off] As Integer, ByVal len As Integer)

		''' <summary>
		''' Reads <code>len</code> longs (signed 64-bit integers) from the
		''' stream according to the current byte order, and
		''' stores them into <code>l</code> starting at index
		''' <code>off</code>.  If the end of the stream is reached, an
		''' <code>java.io.EOFException</code> will be thrown.
		''' 
		''' <p> The bit offset within the stream is reset to zero before
		''' the read occurs.
		''' </summary>
		''' <param name="l"> an array of longs to be written to. </param>
		''' <param name="off"> the starting position within <code>l</code> to write to. </param>
		''' <param name="len"> the maximum number of <code>long</code>s to read.
		''' </param>
		''' <exception cref="IndexOutOfBoundsException"> if <code>off</code> is
		''' negative, <code>len</code> is negative, or <code>off +
		''' len</code> is greater than <code>l.length</code>. </exception>
		''' <exception cref="NullPointerException"> if <code>l</code> is
		''' <code>null</code>. </exception>
		''' <exception cref="java.io.EOFException"> if the stream reaches the end before
		''' reading all the bytes. </exception>
		''' <exception cref="IOException"> if an I/O error occurs. </exception>
		Sub readFully(ByVal l As Long(), ByVal [off] As Integer, ByVal len As Integer)

		''' <summary>
		''' Reads <code>len</code> floats (32-bit IEEE single-precision
		''' floats) from the stream according to the current byte order,
		''' and stores them into <code>f</code> starting at
		''' index <code>off</code>.  If the end of the stream is reached,
		''' an <code>java.io.EOFException</code> will be thrown.
		''' 
		''' <p> The bit offset within the stream is reset to zero before
		''' the read occurs.
		''' </summary>
		''' <param name="f"> an array of floats to be written to. </param>
		''' <param name="off"> the starting position within <code>f</code> to write to. </param>
		''' <param name="len"> the maximum number of <code>float</code>s to read.
		''' </param>
		''' <exception cref="IndexOutOfBoundsException"> if <code>off</code> is
		''' negative, <code>len</code> is negative, or <code>off +
		''' len</code> is greater than <code>f.length</code>. </exception>
		''' <exception cref="NullPointerException"> if <code>f</code> is
		''' <code>null</code>. </exception>
		''' <exception cref="java.io.EOFException"> if the stream reaches the end before
		''' reading all the bytes. </exception>
		''' <exception cref="IOException"> if an I/O error occurs. </exception>
		Sub readFully(ByVal f As Single(), ByVal [off] As Integer, ByVal len As Integer)

		''' <summary>
		''' Reads <code>len</code> doubles (64-bit IEEE double-precision
		''' floats) from the stream according to the current byte order,
		''' and stores them into <code>d</code> starting at
		''' index <code>off</code>.  If the end of the stream is reached,
		''' an <code>java.io.EOFException</code> will be thrown.
		''' 
		''' <p> The bit offset within the stream is reset to zero before
		''' the read occurs.
		''' </summary>
		''' <param name="d"> an array of doubles to be written to. </param>
		''' <param name="off"> the starting position within <code>d</code> to write to. </param>
		''' <param name="len"> the maximum number of <code>double</code>s to read.
		''' </param>
		''' <exception cref="IndexOutOfBoundsException"> if <code>off</code> is
		''' negative, <code>len</code> is negative, or <code>off +
		''' len</code> is greater than <code>d.length</code>. </exception>
		''' <exception cref="NullPointerException"> if <code>d</code> is
		''' <code>null</code>. </exception>
		''' <exception cref="java.io.EOFException"> if the stream reaches the end before
		''' reading all the bytes. </exception>
		''' <exception cref="IOException"> if an I/O error occurs. </exception>
		Sub readFully(ByVal d As Double(), ByVal [off] As Integer, ByVal len As Integer)

		''' <summary>
		''' Returns the current byte position of the stream.  The next read
		''' will take place starting at this offset.
		''' </summary>
		''' <returns> a long containing the position of the stream.
		''' </returns>
		''' <exception cref="IOException"> if an I/O error occurs. </exception>
		ReadOnly Property streamPosition As Long

		''' <summary>
		''' Returns the current bit offset, as an integer between 0 and 7,
		''' inclusive.  The bit offset is updated implicitly by calls to
		''' the <code>readBits</code> method.  A value of 0 indicates the
		''' most-significant bit, and a value of 7 indicates the least
		''' significant bit, of the byte being read.
		''' 
		''' <p> The bit offset is set to 0 when a stream is first
		''' opened, and is reset to 0 by calls to <code>seek</code>,
		''' <code>skipBytes</code>, or any <code>read</code> or
		''' <code>readFully</code> method.
		''' </summary>
		''' <returns> an <code>int</code> containing the bit offset between
		''' 0 and 7, inclusive.
		''' </returns>
		''' <exception cref="IOException"> if an I/O error occurs.
		''' </exception>
		''' <seealso cref= #setBitOffset </seealso>
		Property bitOffset As Integer


		''' <summary>
		''' Reads a single bit from the stream and returns it as an
		''' <code>int</code> with the value <code>0</code> or
		''' <code>1</code>.  The bit offset is advanced by one and reduced
		''' modulo 8.
		''' </summary>
		''' <returns> an <code>int</code> containing the value <code>0</code>
		''' or <code>1</code>.
		''' </returns>
		''' <exception cref="java.io.EOFException"> if the stream reaches the end before
		''' reading all the bits. </exception>
		''' <exception cref="IOException"> if an I/O error occurs. </exception>
		Function readBit() As Integer

		''' <summary>
		''' Reads a bitstring from the stream and returns it as a
		''' <code>long</code>, with the first bit read becoming the most
		''' significant bit of the output.  The read starts within the byte
		''' indicated by <code>getStreamPosition</code>, at the bit given
		''' by <code>getBitOffset</code>.  The bit offset is advanced by
		''' <code>numBits</code> and reduced modulo 8.
		''' 
		''' <p> The byte order of the stream has no effect on this
		''' method.  The return value of this method is constructed as
		''' though the bits were read one at a time, and shifted into
		''' the right side of the return value, as shown by the following
		''' pseudo-code:
		''' 
		''' <pre>{@code
		''' long accum = 0L;
		''' for (int i = 0; i < numBits; i++) {
		'''   accum <<= 1; // Shift left one bit to make room
		'''   accum |= readBit();
		''' }
		''' }</pre>
		''' 
		''' Note that the result of <code>readBits(32)</code> may thus not
		''' be equal to that of <code>readInt()</code> if a reverse network
		''' byte order is being used (i.e., <code>getByteOrder() ==
		''' false</code>).
		''' 
		''' <p> If the end of the stream is encountered before all the bits
		''' have been read, an <code>java.io.EOFException</code> is thrown.
		''' </summary>
		''' <param name="numBits"> the number of bits to read, as an <code>int</code>
		''' between 0 and 64, inclusive. </param>
		''' <returns> the bitstring, as a <code>long</code> with the last bit
		''' read stored in the least significant bit.
		''' </returns>
		''' <exception cref="IllegalArgumentException"> if <code>numBits</code>
		''' is not between 0 and 64, inclusive. </exception>
		''' <exception cref="java.io.EOFException"> if the stream reaches the end before
		''' reading all the bits. </exception>
		''' <exception cref="IOException"> if an I/O error occurs. </exception>
		Function readBits(ByVal numBits As Integer) As Long

		''' <summary>
		''' Returns the total length of the stream, if known.  Otherwise,
		''' <code>-1</code> is returned.
		''' </summary>
		''' <returns> a <code>long</code> containing the length of the
		''' stream, if known, or else <code>-1</code>.
		''' </returns>
		''' <exception cref="IOException"> if an I/O error occurs. </exception>
		Function length() As Long

		''' <summary>
		''' Moves the stream position forward by a given number of bytes.  It
		''' is possible that this method will only be able to skip forward
		''' by a smaller number of bytes than requested, for example if the
		''' end of the stream is reached.  In all cases, the actual number
		''' of bytes skipped is returned.  The bit offset is set to zero
		''' prior to advancing the position.
		''' </summary>
		''' <param name="n"> an <code>int</code> containing the number of bytes to
		''' be skipped.
		''' </param>
		''' <returns> an <code>int</code> representing the number of bytes skipped.
		''' </returns>
		''' <exception cref="IOException"> if an I/O error occurs. </exception>
		Function skipBytes(ByVal n As Integer) As Integer

		''' <summary>
		''' Moves the stream position forward by a given number of bytes.
		''' This method is identical to <code>skipBytes(int)</code> except
		''' that it allows for a larger skip distance.
		''' </summary>
		''' <param name="n"> a <code>long</code> containing the number of bytes to
		''' be skipped.
		''' </param>
		''' <returns> a <code>long</code> representing the number of bytes
		''' skipped.
		''' </returns>
		''' <exception cref="IOException"> if an I/O error occurs. </exception>
		Function skipBytes(ByVal n As Long) As Long

		''' <summary>
		''' Sets the current stream position to the desired location.  The
		''' next read will occur at this location.  The bit offset is set
		''' to 0.
		''' 
		''' <p> An <code>IndexOutOfBoundsException</code> will be thrown if
		''' <code>pos</code> is smaller than the flushed position (as
		''' returned by <code>getflushedPosition</code>).
		''' 
		''' <p> It is legal to seek past the end of the file; an
		''' <code>java.io.EOFException</code> will be thrown only if a read is
		''' performed.
		''' </summary>
		''' <param name="pos"> a <code>long</code> containing the desired file
		''' pointer position.
		''' </param>
		''' <exception cref="IndexOutOfBoundsException"> if <code>pos</code> is smaller
		''' than the flushed position. </exception>
		''' <exception cref="IOException"> if any other I/O error occurs. </exception>
		Sub seek(ByVal pos As Long)

		''' <summary>
		''' Marks a position in the stream to be returned to by a
		''' subsequent call to <code>reset</code>.  Unlike a standard
		''' <code>InputStream</code>, all <code>ImageInputStream</code>s
		''' support marking.  Additionally, calls to <code>mark</code> and
		''' <code>reset</code> may be nested arbitrarily.
		''' 
		''' <p> Unlike the <code>mark</code> methods declared by the
		''' <code>Reader</code> and <code>InputStream</code> interfaces, no
		''' <code>readLimit</code> parameter is used.  An arbitrary amount
		''' of data may be read following the call to <code>mark</code>.
		''' 
		''' <p> The bit position used by the <code>readBits</code> method
		''' is saved and restored by each pair of calls to
		''' <code>mark</code> and <code>reset</code>.
		''' 
		''' <p> Note that it is valid for an <code>ImageReader</code> to call
		''' <code>flushBefore</code> as part of a read operation.
		''' Therefore, if an application calls <code>mark</code> prior to
		''' passing that stream to an <code>ImageReader</code>, the application
		''' should not assume that the marked position will remain valid after
		''' the read operation has completed.
		''' </summary>
		Sub mark()

		''' <summary>
		''' Returns the stream pointer to its previous position, including
		''' the bit offset, at the time of the most recent unmatched call
		''' to <code>mark</code>.
		''' 
		''' <p> Calls to <code>reset</code> without a corresponding call
		''' to <code>mark</code> have no effect.
		''' 
		''' <p> An <code>IOException</code> will be thrown if the previous
		''' marked position lies in the discarded portion of the stream.
		''' </summary>
		''' <exception cref="IOException"> if an I/O error occurs. </exception>
		Sub reset()

		''' <summary>
		''' Discards the initial portion of the stream prior to the
		''' indicated position.  Attempting to seek to an offset within the
		''' flushed portion of the stream will result in an
		''' <code>IndexOutOfBoundsException</code>.
		''' 
		''' <p> Calling <code>flushBefore</code> may allow classes
		''' implementing this interface to free up resources such as memory
		''' or disk space that are being used to store data from the
		''' stream.
		''' </summary>
		''' <param name="pos"> a <code>long</code> containing the length of the
		''' stream prefix that may be flushed.
		''' </param>
		''' <exception cref="IndexOutOfBoundsException"> if <code>pos</code> lies
		''' in the flushed portion of the stream or past the current stream
		''' position. </exception>
		''' <exception cref="IOException"> if an I/O error occurs. </exception>
		Sub flushBefore(ByVal pos As Long)

		''' <summary>
		''' Discards the initial position of the stream prior to the current
		''' stream position.  Equivalent to
		''' <code>flushBefore(getStreamPosition())</code>.
		''' </summary>
		''' <exception cref="IOException"> if an I/O error occurs. </exception>
		Sub flush()

		''' <summary>
		''' Returns the earliest position in the stream to which seeking
		''' may be performed.  The returned value will be the maximum of
		''' all values passed into previous calls to
		''' <code>flushBefore</code>.
		''' </summary>
		''' <returns> the earliest legal position for seeking, as a
		''' <code>long</code>. </returns>
		ReadOnly Property flushedPosition As Long

		''' <summary>
		''' Returns <code>true</code> if this <code>ImageInputStream</code>
		''' caches data itself in order to allow seeking backwards.
		''' Applications may consult this in order to decide how frequently,
		''' or whether, to flush in order to conserve cache resources.
		''' </summary>
		''' <returns> <code>true</code> if this <code>ImageInputStream</code>
		''' caches data.
		''' </returns>
		''' <seealso cref= #isCachedMemory </seealso>
		''' <seealso cref= #isCachedFile </seealso>
		ReadOnly Property cached As Boolean

		''' <summary>
		''' Returns <code>true</code> if this <code>ImageInputStream</code>
		''' caches data itself in order to allow seeking backwards, and
		''' the cache is kept in main memory.  Applications may consult
		''' this in order to decide how frequently, or whether, to flush
		''' in order to conserve cache resources.
		''' </summary>
		''' <returns> <code>true</code> if this <code>ImageInputStream</code>
		''' caches data in main memory.
		''' </returns>
		''' <seealso cref= #isCached </seealso>
		''' <seealso cref= #isCachedFile </seealso>
		ReadOnly Property cachedMemory As Boolean

		''' <summary>
		''' Returns <code>true</code> if this <code>ImageInputStream</code>
		''' caches data itself in order to allow seeking backwards, and
		''' the cache is kept in a temporary file.  Applications may consult
		''' this in order to decide how frequently, or whether, to flush
		''' in order to conserve cache resources.
		''' </summary>
		''' <returns> <code>true</code> if this <code>ImageInputStream</code>
		''' caches data in a temporary file.
		''' </returns>
		''' <seealso cref= #isCached </seealso>
		''' <seealso cref= #isCachedMemory </seealso>
		ReadOnly Property cachedFile As Boolean

		''' <summary>
		''' Closes the stream.  Attempts to access a stream that has been
		''' closed may result in <code>IOException</code>s or incorrect
		''' behavior.  Calling this method may allow classes implementing
		''' this interface to release resources associated with the stream
		''' such as memory, disk space, or file descriptors.
		''' </summary>
		''' <exception cref="IOException"> if an I/O error occurs. </exception>
		Sub close()
	End Interface

End Namespace