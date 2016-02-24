Imports Microsoft.VisualBasic
Imports System
Imports System.Runtime.CompilerServices

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
	''' This class implements a stream filter for uncompressing data in the
	''' "deflate" compression format. It is also used as the basis for other
	''' decompression filters, such as GZIPInputStream.
	''' </summary>
	''' <seealso cref=         Inflater
	''' @author      David Connelly </seealso>
	Public Class InflaterInputStream
		Inherits java.io.FilterInputStream

		''' <summary>
		''' Decompressor for this stream.
		''' </summary>
		Protected Friend inf As Inflater

		''' <summary>
		''' Input buffer for decompression.
		''' </summary>
		Protected Friend buf As SByte()

		''' <summary>
		''' Length of input buffer.
		''' </summary>
		Protected Friend len As Integer

		Private closed As Boolean = False
		' this flag is set to true after EOF has reached
		Private reachEOF As Boolean = False

		''' <summary>
		''' Check to make sure that this stream has not been closed
		''' </summary>
		Private Sub ensureOpen()
			If closed Then Throw New java.io.IOException("Stream closed")
		End Sub


		''' <summary>
		''' Creates a new input stream with the specified decompressor and
		''' buffer size. </summary>
		''' <param name="in"> the input stream </param>
		''' <param name="inf"> the decompressor ("inflater") </param>
		''' <param name="size"> the input buffer size </param>
		''' <exception cref="IllegalArgumentException"> if {@code size <= 0} </exception>
		Public Sub New(ByVal [in] As java.io.InputStream, ByVal inf As Inflater, ByVal size As Integer)
			MyBase.New([in])
			If [in] Is Nothing OrElse inf Is Nothing Then
				Throw New NullPointerException
			ElseIf size <= 0 Then
				Throw New IllegalArgumentException("buffer size <= 0")
			End If
			Me.inf = inf
			buf = New SByte(size - 1){}
		End Sub

		''' <summary>
		''' Creates a new input stream with the specified decompressor and a
		''' default buffer size. </summary>
		''' <param name="in"> the input stream </param>
		''' <param name="inf"> the decompressor ("inflater") </param>
		Public Sub New(ByVal [in] As java.io.InputStream, ByVal inf As Inflater)
			Me.New([in], inf, 512)
		End Sub

		Friend usesDefaultInflater As Boolean = False

		''' <summary>
		''' Creates a new input stream with a default decompressor and buffer size. </summary>
		''' <param name="in"> the input stream </param>
		Public Sub New(ByVal [in] As java.io.InputStream)
			Me.New([in], New Inflater)
			usesDefaultInflater = True
		End Sub

		Private singleByteBuf As SByte() = New SByte(0){}

		''' <summary>
		''' Reads a byte of uncompressed data. This method will block until
		''' enough input is available for decompression. </summary>
		''' <returns> the byte read, or -1 if end of compressed input is reached </returns>
		''' <exception cref="IOException"> if an I/O error has occurred </exception>
		Public Overrides Function read() As Integer
			ensureOpen()
			Return If(read(singleByteBuf, 0, 1) = -1, -1, Byte.toUnsignedInt(singleByteBuf(0)))
		End Function

		''' <summary>
		''' Reads uncompressed data into an array of bytes. If <code>len</code> is not
		''' zero, the method will block until some input can be decompressed; otherwise,
		''' no bytes are read and <code>0</code> is returned. </summary>
		''' <param name="b"> the buffer into which the data is read </param>
		''' <param name="off"> the start offset in the destination array <code>b</code> </param>
		''' <param name="len"> the maximum number of bytes read </param>
		''' <returns> the actual number of bytes read, or -1 if the end of the
		'''         compressed input is reached or a preset dictionary is needed </returns>
		''' <exception cref="NullPointerException"> If <code>b</code> is <code>null</code>. </exception>
		''' <exception cref="IndexOutOfBoundsException"> If <code>off</code> is negative,
		''' <code>len</code> is negative, or <code>len</code> is greater than
		''' <code>b.length - off</code> </exception>
		''' <exception cref="ZipException"> if a ZIP format error has occurred </exception>
		''' <exception cref="IOException"> if an I/O error has occurred </exception>
		Public Overrides Function read(ByVal b As SByte(), ByVal [off] As Integer, ByVal len As Integer) As Integer
			ensureOpen()
			If b Is Nothing Then
				Throw New NullPointerException
			ElseIf [off] < 0 OrElse len < 0 OrElse len > b.Length - [off] Then
				Throw New IndexOutOfBoundsException
			ElseIf len = 0 Then
				Return 0
			End If
			Try
				Dim n As Integer
				n = inf.inflate(b, [off], len)
				Do While n = 0
					If inf.finished() OrElse inf.needsDictionary() Then
						reachEOF = True
						Return -1
					End If
					If inf.needsInput() Then fill()
					n = inf.inflate(b, [off], len)
				Loop
				Return n
			Catch e As DataFormatException
				Dim s As String = e.Message
				Throw New ZipException(If(s IsNot Nothing, s, "Invalid ZLIB data format"))
			End Try
		End Function

		''' <summary>
		''' Returns 0 after EOF has been reached, otherwise always return 1.
		''' <p>
		''' Programs should not count on this method to return the actual number
		''' of bytes that could be read without blocking.
		''' </summary>
		''' <returns>     1 before EOF and 0 after EOF. </returns>
		''' <exception cref="IOException">  if an I/O error occurs.
		'''  </exception>
		Public Overrides Function available() As Integer
			ensureOpen()
			If reachEOF Then
				Return 0
			Else
				Return 1
			End If
		End Function

		Private b As SByte() = New SByte(511){}

		''' <summary>
		''' Skips specified number of bytes of uncompressed data. </summary>
		''' <param name="n"> the number of bytes to skip </param>
		''' <returns> the actual number of bytes skipped. </returns>
		''' <exception cref="IOException"> if an I/O error has occurred </exception>
		''' <exception cref="IllegalArgumentException"> if {@code n < 0} </exception>
		Public Overrides Function skip(ByVal n As Long) As Long
			If n < 0 Then Throw New IllegalArgumentException("negative skip length")
			ensureOpen()
			Dim max As Integer = CInt(Fix(Math.Min(n, Integer.MaxValue)))
			Dim total As Integer = 0
			Do While total < max
				Dim len As Integer = max - total
				If len > b.Length Then len = b.Length
				len = read(b, 0, len)
				If len = -1 Then
					reachEOF = True
					Exit Do
				End If
				total += len
			Loop
			Return total
		End Function

		''' <summary>
		''' Closes this input stream and releases any system resources associated
		''' with the stream. </summary>
		''' <exception cref="IOException"> if an I/O error has occurred </exception>
		Public Overrides Sub close()
			If Not closed Then
				If usesDefaultInflater Then inf.end()
				[in].close()
				closed = True
			End If
		End Sub

		''' <summary>
		''' Fills input buffer with more data to decompress. </summary>
		''' <exception cref="IOException"> if an I/O error has occurred </exception>
		Protected Friend Overridable Sub fill()
			ensureOpen()
			len = [in].read(buf, 0, buf.Length)
			If len = -1 Then Throw New java.io.EOFException("Unexpected end of ZLIB input stream")
			inf.inputput(buf, 0, len)
		End Sub

		''' <summary>
		''' Tests if this input stream supports the <code>mark</code> and
		''' <code>reset</code> methods. The <code>markSupported</code>
		''' method of <code>InflaterInputStream</code> returns
		''' <code>false</code>.
		''' </summary>
		''' <returns>  a <code>boolean</code> indicating if this stream type supports
		'''          the <code>mark</code> and <code>reset</code> methods. </returns>
		''' <seealso cref=     java.io.InputStream#mark(int) </seealso>
		''' <seealso cref=     java.io.InputStream#reset() </seealso>
		Public Overrides Function markSupported() As Boolean
			Return False
		End Function

		''' <summary>
		''' Marks the current position in this input stream.
		''' 
		''' <p> The <code>mark</code> method of <code>InflaterInputStream</code>
		''' does nothing.
		''' </summary>
		''' <param name="readlimit">   the maximum limit of bytes that can be read before
		'''                      the mark position becomes invalid. </param>
		''' <seealso cref=     java.io.InputStream#reset() </seealso>
		<MethodImpl(MethodImplOptions.Synchronized)> _
		Public Overrides Sub mark(ByVal readlimit As Integer)
		End Sub

		''' <summary>
		''' Repositions this stream to the position at the time the
		''' <code>mark</code> method was last called on this input stream.
		''' 
		''' <p> The method <code>reset</code> for class
		''' <code>InflaterInputStream</code> does nothing except throw an
		''' <code>IOException</code>.
		''' </summary>
		''' <exception cref="IOException">  if this method is invoked. </exception>
		''' <seealso cref=     java.io.InputStream#mark(int) </seealso>
		''' <seealso cref=     java.io.IOException </seealso>
		<MethodImpl(MethodImplOptions.Synchronized)> _
		Public Overrides Sub reset()
			Throw New java.io.IOException("mark/reset not supported")
		End Sub
	End Class

End Namespace