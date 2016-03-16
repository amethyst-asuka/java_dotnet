Imports Microsoft.VisualBasic
Imports System

'
' * Copyright (c) 1994, 2006, Oracle and/or its affiliates. All rights reserved.
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
	''' A data input stream lets an application read primitive Java data
	''' types from an underlying input stream in a machine-independent
	''' way. An application uses a data output stream to write data that
	''' can later be read by a data input stream.
	''' <p>
	''' DataInputStream is not necessarily safe for multithreaded access.
	''' Thread safety is optional and is the responsibility of users of
	''' methods in this class.
	''' 
	''' @author  Arthur van Hoff </summary>
	''' <seealso cref=     java.io.DataOutputStream
	''' @since   JDK1.0 </seealso>
	Public Class DataInputStream
		Inherits FilterInputStream
		Implements DataInput

		''' <summary>
		''' Creates a DataInputStream that uses the specified
		''' underlying InputStream.
		''' </summary>
		''' <param name="in">   the specified input stream </param>
		Public Sub New(ByVal [in] As InputStream)
			MyBase.New([in])
		End Sub

		''' <summary>
		''' working arrays initialized on demand by readUTF
		''' </summary>
		Private bytearr As SByte() = New SByte(79){}
		Private chararr As Char() = New Char(79){}

		''' <summary>
		''' Reads some number of bytes from the contained input stream and
		''' stores them into the buffer array <code>b</code>. The number of
		''' bytes actually read is returned as an  java.lang.[Integer]. This method blocks
		''' until input data is available, end of file is detected, or an
		''' exception is thrown.
		''' 
		''' <p>If <code>b</code> is null, a <code>NullPointerException</code> is
		''' thrown. If the length of <code>b</code> is zero, then no bytes are
		''' read and <code>0</code> is returned; otherwise, there is an attempt
		''' to read at least one java.lang.[Byte]. If no byte is available because the
		''' stream is at end of file, the value <code>-1</code> is returned;
		''' otherwise, at least one byte is read and stored into <code>b</code>.
		''' 
		''' <p>The first byte read is stored into element <code>b[0]</code>, the
		''' next one into <code>b[1]</code>, and so on. The number of bytes read
		''' is, at most, equal to the length of <code>b</code>. Let <code>k</code>
		''' be the number of bytes actually read; these bytes will be stored in
		''' elements <code>b[0]</code> through <code>b[k-1]</code>, leaving
		''' elements <code>b[k]</code> through <code>b[b.length-1]</code>
		''' unaffected.
		''' 
		''' <p>The <code>read(b)</code> method has the same effect as:
		''' <blockquote><pre>
		''' read(b, 0, b.length)
		''' </pre></blockquote>
		''' </summary>
		''' <param name="b">   the buffer into which the data is read. </param>
		''' <returns>     the total number of bytes read into the buffer, or
		'''             <code>-1</code> if there is no more data because the end
		'''             of the stream has been reached. </returns>
		''' <exception cref="IOException"> if the first byte cannot be read for any reason
		''' other than end of file, the stream has been closed and the underlying
		''' input stream does not support reading after close, or another I/O
		''' error occurs. </exception>
		''' <seealso cref=        java.io.FilterInputStream#in </seealso>
		''' <seealso cref=        java.io.InputStream#read(byte[], int, int) </seealso>
		Public NotOverridable Overrides Function read(ByVal b As SByte()) As Integer
			Return [in].read(b, 0, b.Length)
		End Function

		''' <summary>
		''' Reads up to <code>len</code> bytes of data from the contained
		''' input stream into an array of bytes.  An attempt is made to read
		''' as many as <code>len</code> bytes, but a smaller number may be read,
		''' possibly zero. The number of bytes actually read is returned as an
		'''  java.lang.[Integer].
		''' 
		''' <p> This method blocks until input data is available, end of file is
		''' detected, or an exception is thrown.
		''' 
		''' <p> If <code>len</code> is zero, then no bytes are read and
		''' <code>0</code> is returned; otherwise, there is an attempt to read at
		''' least one java.lang.[Byte]. If no byte is available because the stream is at end of
		''' file, the value <code>-1</code> is returned; otherwise, at least one
		''' byte is read and stored into <code>b</code>.
		''' 
		''' <p> The first byte read is stored into element <code>b[off]</code>, the
		''' next one into <code>b[off+1]</code>, and so on. The number of bytes read
		''' is, at most, equal to <code>len</code>. Let <i>k</i> be the number of
		''' bytes actually read; these bytes will be stored in elements
		''' <code>b[off]</code> through <code>b[off+</code><i>k</i><code>-1]</code>,
		''' leaving elements <code>b[off+</code><i>k</i><code>]</code> through
		''' <code>b[off+len-1]</code> unaffected.
		''' 
		''' <p> In every case, elements <code>b[0]</code> through
		''' <code>b[off]</code> and elements <code>b[off+len]</code> through
		''' <code>b[b.length-1]</code> are unaffected.
		''' </summary>
		''' <param name="b">     the buffer into which the data is read. </param>
		''' <param name="off"> the start offset in the destination array <code>b</code> </param>
		''' <param name="len">   the maximum number of bytes read. </param>
		''' <returns>     the total number of bytes read into the buffer, or
		'''             <code>-1</code> if there is no more data because the end
		'''             of the stream has been reached. </returns>
		''' <exception cref="NullPointerException"> If <code>b</code> is <code>null</code>. </exception>
		''' <exception cref="IndexOutOfBoundsException"> If <code>off</code> is negative,
		''' <code>len</code> is negative, or <code>len</code> is greater than
		''' <code>b.length - off</code> </exception>
		''' <exception cref="IOException"> if the first byte cannot be read for any reason
		''' other than end of file, the stream has been closed and the underlying
		''' input stream does not support reading after close, or another I/O
		''' error occurs. </exception>
		''' <seealso cref=        java.io.FilterInputStream#in </seealso>
		''' <seealso cref=        java.io.InputStream#read(byte[], int, int) </seealso>
'JAVA TO VB CONVERTER TODO TASK: The following line could not be converted:
		public final int read(byte b() , int off, int len) throws IOException
			Return [in].read(b, off, len)

		''' <summary>
		''' See the general contract of the <code>readFully</code>
		''' method of <code>DataInput</code>.
		''' <p>
		''' Bytes
		''' for this operation are read from the contained
		''' input stream.
		''' </summary>
		''' <param name="b">   the buffer into which the data is read. </param>
		''' <exception cref="EOFException">  if this input stream reaches the end before
		'''             reading all the bytes. </exception>
		''' <exception cref="IOException">   the stream has been closed and the contained
		'''             input stream does not support reading after close, or
		'''             another I/O error occurs. </exception>
		''' <seealso cref=        java.io.FilterInputStream#in </seealso>
		public final  Sub  readFully(SByte b()) throws IOException
			readFully(b, 0, b.length)

		''' <summary>
		''' See the general contract of the <code>readFully</code>
		''' method of <code>DataInput</code>.
		''' <p>
		''' Bytes
		''' for this operation are read from the contained
		''' input stream.
		''' </summary>
		''' <param name="b">     the buffer into which the data is read. </param>
		''' <param name="off">   the start offset of the data. </param>
		''' <param name="len">   the number of bytes to read. </param>
		''' <exception cref="EOFException">  if this input stream reaches the end before
		'''               reading all the bytes. </exception>
		''' <exception cref="IOException">   the stream has been closed and the contained
		'''             input stream does not support reading after close, or
		'''             another I/O error occurs. </exception>
		''' <seealso cref=        java.io.FilterInputStream#in </seealso>
		public final  Sub  readFully(SByte b() , Integer off, Integer len) throws IOException
			If len < 0 Then Throw New IndexOutOfBoundsException
			Dim n As Integer = 0
			Do While n < len
				Dim count As Integer = [in].read(b, off + n, len - n)
				If count < 0 Then Throw New EOFException
				n += count
			Loop

		''' <summary>
		''' See the general contract of the <code>skipBytes</code>
		''' method of <code>DataInput</code>.
		''' <p>
		''' Bytes for this operation are read from the contained
		''' input stream.
		''' </summary>
		''' <param name="n">   the number of bytes to be skipped. </param>
		''' <returns>     the actual number of bytes skipped. </returns>
		''' <exception cref="IOException">  if the contained input stream does not support
		'''             seek, or the stream has been closed and
		'''             the contained input stream does not support
		'''             reading after close, or another I/O error occurs. </exception>
		public final Integer skipBytes(Integer n) throws IOException
			Dim total As Integer = 0
			Dim cur As Integer = 0

			cur = CInt([in].skip(n-total))
			Do While (total<n) AndAlso (cur > 0)
				total += cur
				cur = CInt([in].skip(n-total))
			Loop

			Return total

		''' <summary>
		''' See the general contract of the <code>readBoolean</code>
		''' method of <code>DataInput</code>.
		''' <p>
		''' Bytes for this operation are read from the contained
		''' input stream.
		''' </summary>
		''' <returns>     the <code>boolean</code> value read. </returns>
		''' <exception cref="EOFException">  if this input stream has reached the end. </exception>
		''' <exception cref="IOException">   the stream has been closed and the contained
		'''             input stream does not support reading after close, or
		'''             another I/O error occurs. </exception>
		''' <seealso cref=        java.io.FilterInputStream#in </seealso>
		public final Boolean readBoolean() throws IOException
			Dim ch As Integer = [in].read()
			If ch < 0 Then Throw New EOFException
			Return (ch <> 0)

		''' <summary>
		''' See the general contract of the <code>readByte</code>
		''' method of <code>DataInput</code>.
		''' <p>
		''' Bytes
		''' for this operation are read from the contained
		''' input stream.
		''' </summary>
		''' <returns>     the next byte of this input stream as a signed 8-bit
		'''             <code>byte</code>. </returns>
		''' <exception cref="EOFException">  if this input stream has reached the end. </exception>
		''' <exception cref="IOException">   the stream has been closed and the contained
		'''             input stream does not support reading after close, or
		'''             another I/O error occurs. </exception>
		''' <seealso cref=        java.io.FilterInputStream#in </seealso>
		public final SByte readByte() throws IOException
			Dim ch As Integer = [in].read()
			If ch < 0 Then Throw New EOFException
			Return CByte(ch)

		''' <summary>
		''' See the general contract of the <code>readUnsignedByte</code>
		''' method of <code>DataInput</code>.
		''' <p>
		''' Bytes
		''' for this operation are read from the contained
		''' input stream.
		''' </summary>
		''' <returns>     the next byte of this input stream, interpreted as an
		'''             unsigned 8-bit number. </returns>
		''' <exception cref="EOFException">  if this input stream has reached the end. </exception>
		''' <exception cref="IOException">   the stream has been closed and the contained
		'''             input stream does not support reading after close, or
		'''             another I/O error occurs. </exception>
		''' <seealso cref=         java.io.FilterInputStream#in </seealso>
		public final Integer readUnsignedByte() throws IOException
			Dim ch As Integer = [in].read()
			If ch < 0 Then Throw New EOFException
			Return ch

		''' <summary>
		''' See the general contract of the <code>readShort</code>
		''' method of <code>DataInput</code>.
		''' <p>
		''' Bytes
		''' for this operation are read from the contained
		''' input stream.
		''' </summary>
		''' <returns>     the next two bytes of this input stream, interpreted as a
		'''             signed 16-bit number. </returns>
		''' <exception cref="EOFException">  if this input stream reaches the end before
		'''               reading two bytes. </exception>
		''' <exception cref="IOException">   the stream has been closed and the contained
		'''             input stream does not support reading after close, or
		'''             another I/O error occurs. </exception>
		''' <seealso cref=        java.io.FilterInputStream#in </seealso>
		public final Short readShort() throws IOException
			Dim ch1 As Integer = [in].read()
			Dim ch2 As Integer = [in].read()
			If (ch1 Or ch2) < 0 Then Throw New EOFException
			Return CShort(Fix((ch1 << 8) + (ch2 << 0)))

		''' <summary>
		''' See the general contract of the <code>readUnsignedShort</code>
		''' method of <code>DataInput</code>.
		''' <p>
		''' Bytes
		''' for this operation are read from the contained
		''' input stream.
		''' </summary>
		''' <returns>     the next two bytes of this input stream, interpreted as an
		'''             unsigned 16-bit  java.lang.[Integer]. </returns>
		''' <exception cref="EOFException">  if this input stream reaches the end before
		'''             reading two bytes. </exception>
		''' <exception cref="IOException">   the stream has been closed and the contained
		'''             input stream does not support reading after close, or
		'''             another I/O error occurs. </exception>
		''' <seealso cref=        java.io.FilterInputStream#in </seealso>
		public final Integer readUnsignedShort() throws IOException
			Dim ch1 As Integer = [in].read()
			Dim ch2 As Integer = [in].read()
			If (ch1 Or ch2) < 0 Then Throw New EOFException
			Return (ch1 << 8) + (ch2 << 0)

		''' <summary>
		''' See the general contract of the <code>readChar</code>
		''' method of <code>DataInput</code>.
		''' <p>
		''' Bytes
		''' for this operation are read from the contained
		''' input stream.
		''' </summary>
		''' <returns>     the next two bytes of this input stream, interpreted as a
		'''             <code>char</code>. </returns>
		''' <exception cref="EOFException">  if this input stream reaches the end before
		'''               reading two bytes. </exception>
		''' <exception cref="IOException">   the stream has been closed and the contained
		'''             input stream does not support reading after close, or
		'''             another I/O error occurs. </exception>
		''' <seealso cref=        java.io.FilterInputStream#in </seealso>
		public final Char readChar() throws IOException
			Dim ch1 As Integer = [in].read()
			Dim ch2 As Integer = [in].read()
			If (ch1 Or ch2) < 0 Then Throw New EOFException
			Return CChar((ch1 << 8) + (ch2 << 0))

		''' <summary>
		''' See the general contract of the <code>readInt</code>
		''' method of <code>DataInput</code>.
		''' <p>
		''' Bytes
		''' for this operation are read from the contained
		''' input stream.
		''' </summary>
		''' <returns>     the next four bytes of this input stream, interpreted as an
		'''             <code>int</code>. </returns>
		''' <exception cref="EOFException">  if this input stream reaches the end before
		'''               reading four bytes. </exception>
		''' <exception cref="IOException">   the stream has been closed and the contained
		'''             input stream does not support reading after close, or
		'''             another I/O error occurs. </exception>
		''' <seealso cref=        java.io.FilterInputStream#in </seealso>
		public final Integer readInt() throws IOException
			Dim ch1 As Integer = [in].read()
			Dim ch2 As Integer = [in].read()
			Dim ch3 As Integer = [in].read()
			Dim ch4 As Integer = [in].read()
			If (ch1 Or ch2 Or ch3 Or ch4) < 0 Then Throw New EOFException
			Return ((ch1 << 24) + (ch2 << 16) + (ch3 << 8) + (ch4 << 0))

		private SByte readBuffer() = New SByte(7){}

		''' <summary>
		''' See the general contract of the <code>readLong</code>
		''' method of <code>DataInput</code>.
		''' <p>
		''' Bytes
		''' for this operation are read from the contained
		''' input stream.
		''' </summary>
		''' <returns>     the next eight bytes of this input stream, interpreted as a
		'''             <code>long</code>. </returns>
		''' <exception cref="EOFException">  if this input stream reaches the end before
		'''               reading eight bytes. </exception>
		''' <exception cref="IOException">   the stream has been closed and the contained
		'''             input stream does not support reading after close, or
		'''             another I/O error occurs. </exception>
		''' <seealso cref=        java.io.FilterInputStream#in </seealso>
		public final Long readLong() throws IOException
			readFully(readBuffer, 0, 8)
			Return ((CLng(readBuffer(0)) << 56) + (CLng(readBuffer(1) And 255) << 48) + (CLng(readBuffer(2) And 255) << 40) + (CLng(readBuffer(3) And 255) << 32) + (CLng(readBuffer(4) And 255) << 24) + ((readBuffer(5) And 255) << 16) + ((readBuffer(6) And 255) << 8) + ((readBuffer(7) And 255) << 0))

		''' <summary>
		''' See the general contract of the <code>readFloat</code>
		''' method of <code>DataInput</code>.
		''' <p>
		''' Bytes
		''' for this operation are read from the contained
		''' input stream.
		''' </summary>
		''' <returns>     the next four bytes of this input stream, interpreted as a
		'''             <code>float</code>. </returns>
		''' <exception cref="EOFException">  if this input stream reaches the end before
		'''               reading four bytes. </exception>
		''' <exception cref="IOException">   the stream has been closed and the contained
		'''             input stream does not support reading after close, or
		'''             another I/O error occurs. </exception>
		''' <seealso cref=        java.io.DataInputStream#readInt() </seealso>
		''' <seealso cref=        java.lang.Float#intBitsToFloat(int) </seealso>
		public final Single readFloat() throws IOException
			Return Float.intBitsToFloat(readInt())

		''' <summary>
		''' See the general contract of the <code>readDouble</code>
		''' method of <code>DataInput</code>.
		''' <p>
		''' Bytes
		''' for this operation are read from the contained
		''' input stream.
		''' </summary>
		''' <returns>     the next eight bytes of this input stream, interpreted as a
		'''             <code>double</code>. </returns>
		''' <exception cref="EOFException">  if this input stream reaches the end before
		'''               reading eight bytes. </exception>
		''' <exception cref="IOException">   the stream has been closed and the contained
		'''             input stream does not support reading after close, or
		'''             another I/O error occurs. </exception>
		''' <seealso cref=        java.io.DataInputStream#readLong() </seealso>
		''' <seealso cref=        java.lang.Double#longBitsToDouble(long) </seealso>
		public final Double readDouble() throws IOException
			Return java.lang.[Double].longBitsToDouble(readLong())

		private Char lineBuffer()

		''' <summary>
		''' See the general contract of the <code>readLine</code>
		''' method of <code>DataInput</code>.
		''' <p>
		''' Bytes
		''' for this operation are read from the contained
		''' input stream.
		''' </summary>
		''' @deprecated This method does not properly convert bytes to characters.
		''' As of JDK&nbsp;1.1, the preferred way to read lines of text is via the
		''' <code>BufferedReader.readLine()</code> method.  Programs that use the
		''' <code>DataInputStream</code> class to read lines can be converted to use
		''' the <code>BufferedReader</code> class by replacing code of the form:
		''' <blockquote><pre>
		'''     DataInputStream d =&nbsp;new&nbsp;DataInputStream(in);
		''' </pre></blockquote>
		''' with:
		''' <blockquote><pre>
		'''     BufferedReader d
		'''          =&nbsp;new&nbsp;BufferedReader(new&nbsp;InputStreamReader(in));
		''' </pre></blockquote>
		''' 
		''' <returns>     the next line of text from this input stream. </returns>
		''' <exception cref="IOException">  if an I/O error occurs. </exception>
		''' <seealso cref=        java.io.BufferedReader#readLine() </seealso>
		''' <seealso cref=        java.io.FilterInputStream#in </seealso>
		<Obsolete("This method does not properly convert bytes to characters.")> _
		public final String readLine() throws IOException
			Dim buf As Char() = lineBuffer

			If buf Is Nothing Then
					lineBuffer = New Char(127){}
					buf = lineBuffer
			End If

			Dim room As Integer = buf.Length
			Dim offset As Integer = 0
			Dim c As Integer

	loop:
	Do
'JAVA TO VB CONVERTER TODO TASK: Assignments within expressions are not supported in VB
				Select Case c = [in].read()
				  Case -1, ControlChars.Lf
					GoTo loop

				  Case ControlChars.Cr
					Dim c2 As Integer = [in].read()
					If (c2 <> ControlChars.Lf) AndAlso (c2 <> -1) Then
						If Not(TypeOf [in] Is PushbackInputStream) Then Me.in = New PushbackInputStream([in])
						CType([in], PushbackInputStream).unread(c2)
					End If
					GoTo loop

				  Case Else
					room -= 1
					If room < 0 Then
						buf = New Char(offset + 128 - 1){}
						room = buf.Length - offset - 1
						Array.Copy(lineBuffer, 0, buf, 0, offset)
						lineBuffer = buf
					End If
					buf(offset) = ChrW(c)
					offset += 1
				End Select
	Loop
			If (c = -1) AndAlso (offset = 0) Then Return Nothing
			Return String.copyValueOf(buf, 0, offset)

		''' <summary>
		''' See the general contract of the <code>readUTF</code>
		''' method of <code>DataInput</code>.
		''' <p>
		''' Bytes
		''' for this operation are read from the contained
		''' input stream.
		''' </summary>
		''' <returns>     a Unicode string. </returns>
		''' <exception cref="EOFException">  if this input stream reaches the end before
		'''               reading all the bytes. </exception>
		''' <exception cref="IOException">   the stream has been closed and the contained
		'''             input stream does not support reading after close, or
		'''             another I/O error occurs. </exception>
		''' <exception cref="UTFDataFormatException"> if the bytes do not represent a valid
		'''             modified UTF-8 encoding of a string. </exception>
		''' <seealso cref=        java.io.DataInputStream#readUTF(java.io.DataInput) </seealso>
		public final String readUTF() throws IOException
			Return readUTF(Me)

		''' <summary>
		''' Reads from the
		''' stream <code>in</code> a representation
		''' of a Unicode  character string encoded in
		''' <a href="DataInput.html#modified-utf-8">modified UTF-8</a> format;
		''' this string of characters is then returned as a <code>String</code>.
		''' The details of the modified UTF-8 representation
		''' are  exactly the same as for the <code>readUTF</code>
		''' method of <code>DataInput</code>.
		''' </summary>
		''' <param name="in">   a data input stream. </param>
		''' <returns>     a Unicode string. </returns>
		''' <exception cref="EOFException">            if the input stream reaches the end
		'''               before all the bytes. </exception>
		''' <exception cref="IOException">   the stream has been closed and the contained
		'''             input stream does not support reading after close, or
		'''             another I/O error occurs. </exception>
		''' <exception cref="UTFDataFormatException">  if the bytes do not represent a
		'''               valid modified UTF-8 encoding of a Unicode string. </exception>
		''' <seealso cref=        java.io.DataInputStream#readUnsignedShort() </seealso>
		public final static String readUTF(DataInput [in]) throws IOException
			Dim utflen As Integer = [in].readUnsignedShort()
			Dim bytearr As SByte() = Nothing
			Dim chararr As Char() = Nothing
			If TypeOf [in] Is DataInputStream Then
				Dim dis As DataInputStream = CType([in], DataInputStream)
				If dis.bytearr.Length < utflen Then
					dis.bytearr = New SByte(utflen*2 - 1){}
					dis.chararr = New Char(utflen*2 - 1){}
				End If
				chararr = dis.chararr
				bytearr = dis.bytearr
			Else
				bytearr = New SByte(utflen - 1){}
				chararr = New Char(utflen - 1){}
			End If

			Dim c, char2, char3 As Integer
			Dim count As Integer = 0
			Dim chararr_count As Integer=0

			[in].readFully(bytearr, 0, utflen)

			Do While count < utflen
				c = CInt(bytearr(count)) And &Hff
				If c > 127 Then Exit Do
				count += 1
				chararr(chararr_count)=ChrW(c)
				chararr_count += 1
			Loop

			Do While count < utflen
				c = CInt(bytearr(count)) And &Hff
				Select Case c >> 4
					Case 0, 1, 2, 3, 4, 5, 6, 7
						' 0xxxxxxx
						count += 1
						chararr(chararr_count)=ChrW(c)
						chararr_count += 1
					Case 12, 13
						' 110x xxxx   10xx xxxx
						count += 2
						If count > utflen Then Throw New UTFDataFormatException("malformed input: partial character at end")
						char2 = CInt(bytearr(count-1))
						If (char2 And &HC0) <> &H80 Then Throw New UTFDataFormatException("malformed input around byte " & count)
						chararr(chararr_count)=CChar(((c And &H1F) << 6) Or (char2 And &H3F))
						chararr_count += 1
					Case 14
						' 1110 xxxx  10xx xxxx  10xx xxxx 
						count += 3
						If count > utflen Then Throw New UTFDataFormatException("malformed input: partial character at end")
						char2 = CInt(bytearr(count-2))
						char3 = CInt(bytearr(count-1))
						If ((char2 And &HC0) <> &H80) OrElse ((char3 And &HC0) <> &H80) Then Throw New UTFDataFormatException("malformed input around byte " & (count-1))
						chararr(chararr_count)=CChar(((c And &HF) << 12) Or ((char2 And &H3F) << 6) Or ((char3 And &H3F) << 0))
						chararr_count += 1
					Case Else
						' 10xx xxxx,  1111 xxxx 
						Throw New UTFDataFormatException("malformed input around byte " & count)
				End Select
			Loop
			' The number of chars produced may be less than utflen
			Return New String(chararr, 0, chararr_count)
	End Class

End Namespace