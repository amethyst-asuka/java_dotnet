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
	''' Implements an output stream filter for uncompressing data stored in the
	''' "deflate" compression format.
	''' 
	''' @since       1.6
	''' @author      David R Tribble (david@tribble.com)
	''' </summary>
	''' <seealso cref= InflaterInputStream </seealso>
	''' <seealso cref= DeflaterInputStream </seealso>
	''' <seealso cref= DeflaterOutputStream </seealso>

	Public Class InflaterOutputStream
		Inherits java.io.FilterOutputStream

		''' <summary>
		''' Decompressor for this stream. </summary>
		Protected Friend ReadOnly inf As Inflater

		''' <summary>
		''' Output buffer for writing uncompressed data. </summary>
		Protected Friend ReadOnly buf As SByte()

		''' <summary>
		''' Temporary write buffer. </summary>
		Private ReadOnly wbuf As SByte() = New SByte(0){}

		''' <summary>
		''' Default decompressor is used. </summary>
		Private usesDefaultInflater As Boolean = False

		''' <summary>
		''' true iff <seealso cref="#close()"/> has been called. </summary>
		Private closed As Boolean = False

		''' <summary>
		''' Checks to make sure that this stream has not been closed.
		''' </summary>
		Private Sub ensureOpen()
			If closed Then Throw New java.io.IOException("Stream closed")
		End Sub

		''' <summary>
		''' Creates a new output stream with a default decompressor and buffer
		''' size.
		''' </summary>
		''' <param name="out"> output stream to write the uncompressed data to </param>
		''' <exception cref="NullPointerException"> if {@code out} is null </exception>
		Public Sub New(ByVal out As java.io.OutputStream)
			Me.New(out, New Inflater)
			usesDefaultInflater = True
		End Sub

		''' <summary>
		''' Creates a new output stream with the specified decompressor and a
		''' default buffer size.
		''' </summary>
		''' <param name="out"> output stream to write the uncompressed data to </param>
		''' <param name="infl"> decompressor ("inflater") for this stream </param>
		''' <exception cref="NullPointerException"> if {@code out} or {@code infl} is null </exception>
		Public Sub New(ByVal out As java.io.OutputStream, ByVal infl As Inflater)
			Me.New(out, infl, 512)
		End Sub

		''' <summary>
		''' Creates a new output stream with the specified decompressor and
		''' buffer size.
		''' </summary>
		''' <param name="out"> output stream to write the uncompressed data to </param>
		''' <param name="infl"> decompressor ("inflater") for this stream </param>
		''' <param name="bufLen"> decompression buffer size </param>
		''' <exception cref="IllegalArgumentException"> if {@code bufLen <= 0} </exception>
		''' <exception cref="NullPointerException"> if {@code out} or {@code infl} is null </exception>
		Public Sub New(ByVal out As java.io.OutputStream, ByVal infl As Inflater, ByVal bufLen As Integer)
			MyBase.New(out)

			' Sanity checks
			If out Is Nothing Then Throw New NullPointerException("Null output")
			If infl Is Nothing Then Throw New NullPointerException("Null inflater")
			If bufLen <= 0 Then Throw New IllegalArgumentException("Buffer size < 1")

			' Initialize
			inf = infl
			buf = New SByte(bufLen - 1){}
		End Sub

		''' <summary>
		''' Writes any remaining uncompressed data to the output stream and closes
		''' the underlying output stream.
		''' </summary>
		''' <exception cref="IOException"> if an I/O error occurs </exception>
		Public Overrides Sub close()
			If Not closed Then
				' Complete the uncompressed output
				Try
					finish()
				Finally
					out.close()
					closed = True
				End Try
			End If
		End Sub

		''' <summary>
		''' Flushes this output stream, forcing any pending buffered output bytes to be
		''' written.
		''' </summary>
		''' <exception cref="IOException"> if an I/O error occurs or this stream is already
		''' closed </exception>
		Public Overrides Sub flush()
			ensureOpen()

			' Finish decompressing and writing pending output data
			If Not inf.finished() Then
				Try
					Do While (Not inf.finished()) AndAlso Not inf.needsInput()
						Dim n As Integer

						' Decompress pending output data
						n = inf.inflate(buf, 0, buf.Length)
						If n < 1 Then Exit Do

						' Write the uncompressed output data block
						out.write(buf, 0, n)
					Loop
					MyBase.flush()
				Catch ex As DataFormatException
					' Improperly formatted compressed (ZIP) data
					Dim msg As String = ex.Message
					If msg Is Nothing Then msg = "Invalid ZLIB data format"
					Throw New ZipException(msg)
				End Try
			End If
		End Sub

		''' <summary>
		''' Finishes writing uncompressed data to the output stream without closing
		''' the underlying stream.  Use this method when applying multiple filters in
		''' succession to the same output stream.
		''' </summary>
		''' <exception cref="IOException"> if an I/O error occurs or this stream is already
		''' closed </exception>
		Public Overridable Sub finish()
			ensureOpen()

			' Finish decompressing and writing pending output data
			flush()
			If usesDefaultInflater Then inf.end()
		End Sub

		''' <summary>
		''' Writes a byte to the uncompressed output stream.
		''' </summary>
		''' <param name="b"> a single byte of compressed data to decompress and write to
		''' the output stream </param>
		''' <exception cref="IOException"> if an I/O error occurs or this stream is already
		''' closed </exception>
		''' <exception cref="ZipException"> if a compression (ZIP) format error occurs </exception>
		Public Overrides Sub write(ByVal b As Integer)
			' Write a single byte of data
			wbuf(0) = CByte(b)
			write(wbuf, 0, 1)
		End Sub

		''' <summary>
		''' Writes an array of bytes to the uncompressed output stream.
		''' </summary>
		''' <param name="b"> buffer containing compressed data to decompress and write to
		''' the output stream </param>
		''' <param name="off"> starting offset of the compressed data within {@code b} </param>
		''' <param name="len"> number of bytes to decompress from {@code b} </param>
		''' <exception cref="IndexOutOfBoundsException"> if {@code off < 0}, or if
		''' {@code len < 0}, or if {@code len > b.length - off} </exception>
		''' <exception cref="IOException"> if an I/O error occurs or this stream is already
		''' closed </exception>
		''' <exception cref="NullPointerException"> if {@code b} is null </exception>
		''' <exception cref="ZipException"> if a compression (ZIP) format error occurs </exception>
		Public Overrides Sub write(ByVal b As SByte(), ByVal [off] As Integer, ByVal len As Integer)
			' Sanity checks
			ensureOpen()
			If b Is Nothing Then
				Throw New NullPointerException("Null buffer for read")
			ElseIf [off] < 0 OrElse len < 0 OrElse len > b.Length - [off] Then
				Throw New IndexOutOfBoundsException
			ElseIf len = 0 Then
				Return
			End If

			' Write uncompressed data to the output stream
			Try
				Do
					Dim n As Integer

					' Fill the decompressor buffer with output data
					If inf.needsInput() Then
						Dim part As Integer

						If len < 1 Then Exit Do

						part = (If(len < 512, len, 512))
						inf.inputput(b, [off], part)
						[off] += part
						len -= part
					End If

					' Decompress and write blocks of output data
					Do
						n = inf.inflate(buf, 0, buf.Length)
						If n > 0 Then out.write(buf, 0, n)
					Loop While n > 0

					' Check the decompressor
					If inf.finished() Then Exit Do
					If inf.needsDictionary() Then Throw New ZipException("ZLIB dictionary missing")
				Loop
			Catch ex As DataFormatException
				' Improperly formatted compressed (ZIP) data
				Dim msg As String = ex.Message
				If msg Is Nothing Then msg = "Invalid ZLIB data format"
				Throw New ZipException(msg)
			End Try
		End Sub
	End Class

End Namespace