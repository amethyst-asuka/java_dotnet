Imports Microsoft.VisualBasic
Imports System

'
' * Copyright (c) 2006, 2013, Oracle and/or its affiliates. All rights reserved.
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
	''' Implements an input stream filter for compressing data in the "deflate"
	''' compression format.
	''' 
	''' @since       1.6
	''' @author      David R Tribble (david@tribble.com)
	''' </summary>
	''' <seealso cref= DeflaterOutputStream </seealso>
	''' <seealso cref= InflaterOutputStream </seealso>
	''' <seealso cref= InflaterInputStream </seealso>

	Public Class DeflaterInputStream
		Inherits java.io.FilterInputStream

		''' <summary>
		''' Compressor for this stream. </summary>
		Protected Friend ReadOnly def As Deflater

		''' <summary>
		''' Input buffer for reading compressed data. </summary>
		Protected Friend ReadOnly buf As SByte()

		''' <summary>
		''' Temporary read buffer. </summary>
		Private rbuf As SByte() = New SByte(0){}

		''' <summary>
		''' Default compressor is used. </summary>
		Private usesDefaultDeflater As Boolean = False

		''' <summary>
		''' End of the underlying input stream has been reached. </summary>
		Private reachEOF As Boolean = False

		''' <summary>
		''' Check to make sure that this stream has not been closed.
		''' </summary>
		Private Sub ensureOpen()
			If [in] Is Nothing Then Throw New java.io.IOException("Stream closed")
		End Sub

		''' <summary>
		''' Creates a new input stream with a default compressor and buffer
		''' size.
		''' </summary>
		''' <param name="in"> input stream to read the uncompressed data to </param>
		''' <exception cref="NullPointerException"> if {@code in} is null </exception>
		Public Sub New(ByVal [in] As java.io.InputStream)
			Me.New([in], New Deflater)
			usesDefaultDeflater = True
		End Sub

		''' <summary>
		''' Creates a new input stream with the specified compressor and a
		''' default buffer size.
		''' </summary>
		''' <param name="in"> input stream to read the uncompressed data to </param>
		''' <param name="defl"> compressor ("deflater") for this stream </param>
		''' <exception cref="NullPointerException"> if {@code in} or {@code defl} is null </exception>
		Public Sub New(ByVal [in] As java.io.InputStream, ByVal defl As Deflater)
			Me.New([in], defl, 512)
		End Sub

		''' <summary>
		''' Creates a new input stream with the specified compressor and buffer
		''' size.
		''' </summary>
		''' <param name="in"> input stream to read the uncompressed data to </param>
		''' <param name="defl"> compressor ("deflater") for this stream </param>
		''' <param name="bufLen"> compression buffer size </param>
		''' <exception cref="IllegalArgumentException"> if {@code bufLen <= 0} </exception>
		''' <exception cref="NullPointerException"> if {@code in} or {@code defl} is null </exception>
		Public Sub New(ByVal [in] As java.io.InputStream, ByVal defl As Deflater, ByVal bufLen As Integer)
			MyBase.New([in])

			' Sanity checks
			If [in] Is Nothing Then Throw New NullPointerException("Null input")
			If defl Is Nothing Then Throw New NullPointerException("Null deflater")
			If bufLen < 1 Then Throw New IllegalArgumentException("Buffer size < 1")

			' Initialize
			def = defl
			buf = New SByte(bufLen - 1){}
		End Sub

		''' <summary>
		''' Closes this input stream and its underlying input stream, discarding
		''' any pending uncompressed data.
		''' </summary>
		''' <exception cref="IOException"> if an I/O error occurs </exception>
		Public Overrides Sub close()
			If [in] IsNot Nothing Then
				Try
					' Clean up
					If usesDefaultDeflater Then def.end()

					[in].close()
				Finally
					[in] = Nothing
				End Try
			End If
		End Sub

		''' <summary>
		''' Reads a single byte of compressed data from the input stream.
		''' This method will block until some input can be read and compressed.
		''' </summary>
		''' <returns> a single byte of compressed data, or -1 if the end of the
		''' uncompressed input stream is reached </returns>
		''' <exception cref="IOException"> if an I/O error occurs or if this stream is
		''' already closed </exception>
		Public Overrides Function read() As Integer
			' Read a single byte of compressed data
			Dim len As Integer = read(rbuf, 0, 1)
			If len <= 0 Then Return -1
			Return (rbuf(0) And &HFF)
		End Function

		''' <summary>
		''' Reads compressed data into a byte array.
		''' This method will block until some input can be read and compressed.
		''' </summary>
		''' <param name="b"> buffer into which the data is read </param>
		''' <param name="off"> starting offset of the data within {@code b} </param>
		''' <param name="len"> maximum number of compressed bytes to read into {@code b} </param>
		''' <returns> the actual number of bytes read, or -1 if the end of the
		''' uncompressed input stream is reached </returns>
		''' <exception cref="IndexOutOfBoundsException">  if {@code len > b.length - off} </exception>
		''' <exception cref="IOException"> if an I/O error occurs or if this input stream is
		''' already closed </exception>
		Public Overrides Function read(ByVal b As SByte(), ByVal [off] As Integer, ByVal len As Integer) As Integer
			' Sanity checks
			ensureOpen()
			If b Is Nothing Then
				Throw New NullPointerException("Null buffer for read")
			ElseIf [off] < 0 OrElse len < 0 OrElse len > b.Length - [off] Then
				Throw New IndexOutOfBoundsException
			ElseIf len = 0 Then
				Return 0
			End If

			' Read and compress (deflate) input data bytes
			Dim cnt As Integer = 0
			Do While len > 0 AndAlso Not def.finished()
				Dim n As Integer

				' Read data from the input stream
				If def.needsInput() Then
					n = [in].read(buf, 0, buf.Length)
					If n < 0 Then
						' End of the input stream reached
						def.finish()
					ElseIf n > 0 Then
						def.inputput(buf, 0, n)
					End If
				End If

				' Compress the input data, filling the read buffer
				n = def.deflate(b, [off], len)
				cnt += n
				[off] += n
				len -= n
			Loop
			If cnt = 0 AndAlso def.finished() Then
				reachEOF = True
				cnt = -1
			End If

			Return cnt
		End Function

		''' <summary>
		''' Skips over and discards data from the input stream.
		''' This method may block until the specified number of bytes are read and
		''' skipped. <em>Note:</em> While {@code n} is given as a {@code long},
		''' the maximum number of bytes which can be skipped is
		''' {@code  java.lang.[Integer].MAX_VALUE}.
		''' </summary>
		''' <param name="n"> number of bytes to be skipped </param>
		''' <returns> the actual number of bytes skipped </returns>
		''' <exception cref="IOException"> if an I/O error occurs or if this stream is
		''' already closed </exception>
		Public Overrides Function skip(ByVal n As Long) As Long
			If n < 0 Then Throw New IllegalArgumentException("negative skip length")
			ensureOpen()

			' Skip bytes by repeatedly decompressing small blocks
			If rbuf.Length < 512 Then rbuf = New SByte(511){}

			Dim total As Integer = CInt(Fix (System.Math.Min(n,  java.lang.[Integer].Max_Value)))
			Dim cnt As Long = 0
			Do While total > 0
				' Read a small block of uncompressed bytes
				Dim len As Integer = read(rbuf, 0, (If(total <= rbuf.Length, total, rbuf.Length)))

				If len < 0 Then Exit Do
				cnt += len
				total -= len
			Loop
			Return cnt
		End Function

		''' <summary>
		''' Returns 0 after EOF has been reached, otherwise always return 1.
		''' <p>
		''' Programs should not count on this method to return the actual number
		''' of bytes that could be read without blocking </summary>
		''' <returns> zero after the end of the underlying input stream has been
		''' reached, otherwise always returns 1 </returns>
		''' <exception cref="IOException"> if an I/O error occurs or if this stream is
		''' already closed </exception>
		Public Overrides Function available() As Integer
			ensureOpen()
			If reachEOF Then Return 0
			Return 1
		End Function

		''' <summary>
		''' Always returns {@code false} because this input stream does not support
		''' the <seealso cref="#mark mark()"/> and <seealso cref="#reset reset()"/> methods.
		''' </summary>
		''' <returns> false, always </returns>
		Public Overrides Function markSupported() As Boolean
			Return False
		End Function

		''' <summary>
		''' <i>This operation is not supported</i>.
		''' </summary>
		''' <param name="limit"> maximum bytes that can be read before invalidating the position marker </param>
		Public Overrides Sub mark(ByVal limit As Integer)
			' Operation not supported
		End Sub

		''' <summary>
		''' <i>This operation is not supported</i>.
		''' </summary>
		''' <exception cref="IOException"> always thrown </exception>
		Public Overrides Sub reset()
			Throw New java.io.IOException("mark/reset not supported")
		End Sub
	End Class

End Namespace