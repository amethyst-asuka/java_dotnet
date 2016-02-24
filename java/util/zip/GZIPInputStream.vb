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

Namespace java.util.zip


	''' <summary>
	''' This class implements a stream filter for reading compressed data in
	''' the GZIP file format.
	''' </summary>
	''' <seealso cref=         InflaterInputStream
	''' @author      David Connelly
	'''  </seealso>
	Public Class GZIPInputStream
		Inherits InflaterInputStream

		''' <summary>
		''' CRC-32 for uncompressed data.
		''' </summary>
		Protected Friend crc As New CRC32

		''' <summary>
		''' Indicates end of input stream.
		''' </summary>
		Protected Friend eos As Boolean

		Private closed As Boolean = False

		''' <summary>
		''' Check to make sure that this stream has not been closed
		''' </summary>
		Private Sub ensureOpen()
			If closed Then Throw New java.io.IOException("Stream closed")
		End Sub

		''' <summary>
		''' Creates a new input stream with the specified buffer size. </summary>
		''' <param name="in"> the input stream </param>
		''' <param name="size"> the input buffer size
		''' </param>
		''' <exception cref="ZipException"> if a GZIP format error has occurred or the
		'''                         compression method used is unsupported </exception>
		''' <exception cref="IOException"> if an I/O error has occurred </exception>
		''' <exception cref="IllegalArgumentException"> if {@code size <= 0} </exception>
		Public Sub New(ByVal [in] As java.io.InputStream, ByVal size As Integer)
			MyBase.New([in], New Inflater(True), size)
			usesDefaultInflater = True
			readHeader([in])
		End Sub

		''' <summary>
		''' Creates a new input stream with a default buffer size. </summary>
		''' <param name="in"> the input stream
		''' </param>
		''' <exception cref="ZipException"> if a GZIP format error has occurred or the
		'''                         compression method used is unsupported </exception>
		''' <exception cref="IOException"> if an I/O error has occurred </exception>
		Public Sub New(ByVal [in] As java.io.InputStream)
			Me.New([in], 512)
		End Sub

		''' <summary>
		''' Reads uncompressed data into an array of bytes. If <code>len</code> is not
		''' zero, the method will block until some input can be decompressed; otherwise,
		''' no bytes are read and <code>0</code> is returned. </summary>
		''' <param name="buf"> the buffer into which the data is read </param>
		''' <param name="off"> the start offset in the destination array <code>b</code> </param>
		''' <param name="len"> the maximum number of bytes read </param>
		''' <returns>  the actual number of bytes read, or -1 if the end of the
		'''          compressed input stream is reached
		''' </returns>
		''' <exception cref="NullPointerException"> If <code>buf</code> is <code>null</code>. </exception>
		''' <exception cref="IndexOutOfBoundsException"> If <code>off</code> is negative,
		''' <code>len</code> is negative, or <code>len</code> is greater than
		''' <code>buf.length - off</code> </exception>
		''' <exception cref="ZipException"> if the compressed input data is corrupt. </exception>
		''' <exception cref="IOException"> if an I/O error has occurred.
		'''  </exception>
		Public Overrides Function read(ByVal buf As SByte(), ByVal [off] As Integer, ByVal len As Integer) As Integer
			ensureOpen()
			If eos Then Return -1
			Dim n As Integer = MyBase.read(buf, [off], len)
			If n = -1 Then
				If readTrailer() Then
					eos = True
				Else
					Return Me.read(buf, [off], len)
				End If
			Else
				crc.update(buf, [off], n)
			End If
			Return n
		End Function

		''' <summary>
		''' Closes this input stream and releases any system resources associated
		''' with the stream. </summary>
		''' <exception cref="IOException"> if an I/O error has occurred </exception>
		Public Overrides Sub close()
			If Not closed Then
				MyBase.close()
				eos = True
				closed = True
			End If
		End Sub

		''' <summary>
		''' GZIP header magic number.
		''' </summary>
		Public Const GZIP_MAGIC As Integer = &H8b1f

	'    
	'     * File header flags.
	'     
		Private Const FTEXT As Integer = 1 ' Extra text
		Private Const FHCRC As Integer = 2 ' Header CRC
		Private Const FEXTRA As Integer = 4 ' Extra field
		Private Const FNAME As Integer = 8 ' File name
		Private Const FCOMMENT As Integer = 16 ' File comment

	'    
	'     * Reads GZIP member header and returns the total byte number
	'     * of this member header.
	'     
		Private Function readHeader(ByVal this_in As java.io.InputStream) As Integer
			Dim [in] As New CheckedInputStream(this_in, crc)
			crc.reset()
			' Check header magic
			If readUShort([in]) <> GZIP_MAGIC Then Throw New ZipException("Not in GZIP format")
			' Check compression method
			If readUByte([in]) <> 8 Then Throw New ZipException("Unsupported compression method")
			' Read flags
			Dim flg As Integer = readUByte([in])
			' Skip MTIME, XFL, and OS fields
			skipBytes([in], 6)
			Dim n As Integer = 2 + 2 + 6
			' Skip optional extra field
			If (flg And FEXTRA) = FEXTRA Then
				Dim m As Integer = readUShort([in])
				skipBytes([in], m)
				n += m + 2
			End If
			' Skip optional file name
			If (flg And FNAME) = FNAME Then
				Do
					n += 1
				Loop While readUByte([in]) <> 0
			End If
			' Skip optional file comment
			If (flg And FCOMMENT) = FCOMMENT Then
				Do
					n += 1
				Loop While readUByte([in]) <> 0
			End If
			' Check optional header CRC
			If (flg And FHCRC) = FHCRC Then
				Dim v As Integer = CInt(crc.value) And &Hffff
				If readUShort([in]) <> v Then Throw New ZipException("Corrupt GZIP header")
				n += 2
			End If
			crc.reset()
			Return n
		End Function

	'    
	'     * Reads GZIP member trailer and returns true if the eos
	'     * reached, false if there are more (concatenated gzip
	'     * data set)
	'     
		Private Function readTrailer() As Boolean
			Dim [in] As java.io.InputStream = Me.in
			Dim n As Integer = inf.remaining
			If n > 0 Then [in] = New java.io.SequenceInputStream(New java.io.ByteArrayInputStream(buf, len - n, n), New FilterInputStreamAnonymousInnerClassHelper
			' Uses left-to-right evaluation order
			If (readUInt([in]) <> crc.value) OrElse (readUInt([in]) <> (inf.bytesWritten And &HffffffffL)) Then Throw New ZipException("Corrupt GZIP trailer")

			' If there are more bytes available in "in" or
			' the leftover in the "inf" is > 26 bytes:
			' this.trailer(8) + next.header.min(10) + next.trailer(8)
			' try concatenated case
			If Me.in.available() > 0 OrElse n > 26 Then
				Dim m As Integer = 8 ' this.trailer
				Try
					m += readHeader([in]) ' next.header
				Catch ze As java.io.IOException
					Return True ' ignore any malformed, do nothing
				End Try
				inf.reset()
				If n > m Then inf.inputput(buf, len - n + m, n - m)
				Return False
			End If
			Return True
		End Function

		Private Class FilterInputStreamAnonymousInnerClassHelper
			Inherits java.io.FilterInputStream

			Public Overrides Sub close()
			End Sub
		End Class

	'    
	'     * Reads unsigned integer in Intel byte order.
	'     
		Private Function readUInt(ByVal [in] As java.io.InputStream) As Long
			Dim s As Long = readUShort([in])
			Return (CLng(readUShort([in])) << 16) Or s
		End Function

	'    
	'     * Reads unsigned short in Intel byte order.
	'     
		Private Function readUShort(ByVal [in] As java.io.InputStream) As Integer
			Dim b As Integer = readUByte([in])
			Return (readUByte([in]) << 8) Or b
		End Function

	'    
	'     * Reads unsigned byte.
	'     
		Private Function readUByte(ByVal [in] As java.io.InputStream) As Integer
			Dim b As Integer = [in].read()
			If b = -1 Then Throw New java.io.EOFException
			If b < -1 OrElse b > 255 Then Throw New java.io.IOException(Me.in.GetType().name & ".read() returned value out of range -1..255: " & b)
			Return b
		End Function

		Private tmpbuf As SByte() = New SByte(127){}

	'    
	'     * Skips bytes of input data blocking until all bytes are skipped.
	'     * Does not assume that the input stream is capable of seeking.
	'     
		Private Sub skipBytes(ByVal [in] As java.io.InputStream, ByVal n As Integer)
			Do While n > 0
				Dim len As Integer = [in].read(tmpbuf, 0,If(n < tmpbuf.Length, n, tmpbuf.Length))
				If len = -1 Then Throw New java.io.EOFException
				n -= len
			Loop
		End Sub
	End Class

End Namespace