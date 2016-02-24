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
	''' This class implements an output stream filter for compressing data in
	''' the "deflate" compression format. It is also used as the basis for other
	''' types of compression filters, such as GZIPOutputStream.
	''' </summary>
	''' <seealso cref=         Deflater
	''' @author      David Connelly </seealso>
	Public Class DeflaterOutputStream
		Inherits java.io.FilterOutputStream

		''' <summary>
		''' Compressor for this stream.
		''' </summary>
		Protected Friend def As Deflater

		''' <summary>
		''' Output buffer for writing compressed data.
		''' </summary>
		Protected Friend buf As SByte()

		''' <summary>
		''' Indicates that the stream has been closed.
		''' </summary>

		Private closed As Boolean = False

		Private ReadOnly syncFlush As Boolean

		''' <summary>
		''' Creates a new output stream with the specified compressor,
		''' buffer size and flush mode.
		''' </summary>
		''' <param name="out"> the output stream </param>
		''' <param name="def"> the compressor ("deflater") </param>
		''' <param name="size"> the output buffer size </param>
		''' <param name="syncFlush">
		'''        if {@code true} the <seealso cref="#flush()"/> method of this
		'''        instance flushes the compressor with flush mode
		'''        <seealso cref="Deflater#SYNC_FLUSH"/> before flushing the output
		'''        stream, otherwise only flushes the output stream
		''' </param>
		''' <exception cref="IllegalArgumentException"> if {@code size <= 0}
		''' 
		''' @since 1.7 </exception>
		Public Sub New(ByVal out As java.io.OutputStream, ByVal def As Deflater, ByVal size As Integer, ByVal syncFlush As Boolean)
			MyBase.New(out)
			If out Is Nothing OrElse def Is Nothing Then
				Throw New NullPointerException
			ElseIf size <= 0 Then
				Throw New IllegalArgumentException("buffer size <= 0")
			End If
			Me.def = def
			Me.buf = New SByte(size - 1){}
			Me.syncFlush = syncFlush
		End Sub


		''' <summary>
		''' Creates a new output stream with the specified compressor and
		''' buffer size.
		''' 
		''' <p>The new output stream instance is created as if by invoking
		''' the 4-argument constructor DeflaterOutputStream(out, def, size, false).
		''' </summary>
		''' <param name="out"> the output stream </param>
		''' <param name="def"> the compressor ("deflater") </param>
		''' <param name="size"> the output buffer size </param>
		''' <exception cref="IllegalArgumentException"> if {@code size <= 0} </exception>
		Public Sub New(ByVal out As java.io.OutputStream, ByVal def As Deflater, ByVal size As Integer)
			Me.New(out, def, size, False)
		End Sub

		''' <summary>
		''' Creates a new output stream with the specified compressor, flush
		''' mode and a default buffer size.
		''' </summary>
		''' <param name="out"> the output stream </param>
		''' <param name="def"> the compressor ("deflater") </param>
		''' <param name="syncFlush">
		'''        if {@code true} the <seealso cref="#flush()"/> method of this
		'''        instance flushes the compressor with flush mode
		'''        <seealso cref="Deflater#SYNC_FLUSH"/> before flushing the output
		'''        stream, otherwise only flushes the output stream
		''' 
		''' @since 1.7 </param>
		Public Sub New(ByVal out As java.io.OutputStream, ByVal def As Deflater, ByVal syncFlush As Boolean)
			Me.New(out, def, 512, syncFlush)
		End Sub


		''' <summary>
		''' Creates a new output stream with the specified compressor and
		''' a default buffer size.
		''' 
		''' <p>The new output stream instance is created as if by invoking
		''' the 3-argument constructor DeflaterOutputStream(out, def, false).
		''' </summary>
		''' <param name="out"> the output stream </param>
		''' <param name="def"> the compressor ("deflater") </param>
		Public Sub New(ByVal out As java.io.OutputStream, ByVal def As Deflater)
			Me.New(out, def, 512, False)
		End Sub

		Friend usesDefaultDeflater As Boolean = False


		''' <summary>
		''' Creates a new output stream with a default compressor, a default
		''' buffer size and the specified flush mode.
		''' </summary>
		''' <param name="out"> the output stream </param>
		''' <param name="syncFlush">
		'''        if {@code true} the <seealso cref="#flush()"/> method of this
		'''        instance flushes the compressor with flush mode
		'''        <seealso cref="Deflater#SYNC_FLUSH"/> before flushing the output
		'''        stream, otherwise only flushes the output stream
		''' 
		''' @since 1.7 </param>
		Public Sub New(ByVal out As java.io.OutputStream, ByVal syncFlush As Boolean)
			Me.New(out, New Deflater, 512, syncFlush)
			usesDefaultDeflater = True
		End Sub

		''' <summary>
		''' Creates a new output stream with a default compressor and buffer size.
		''' 
		''' <p>The new output stream instance is created as if by invoking
		''' the 2-argument constructor DeflaterOutputStream(out, false).
		''' </summary>
		''' <param name="out"> the output stream </param>
		Public Sub New(ByVal out As java.io.OutputStream)
			Me.New(out, False)
			usesDefaultDeflater = True
		End Sub

		''' <summary>
		''' Writes a byte to the compressed output stream. This method will
		''' block until the byte can be written. </summary>
		''' <param name="b"> the byte to be written </param>
		''' <exception cref="IOException"> if an I/O error has occurred </exception>
		Public Overrides Sub write(ByVal b As Integer)
			Dim buf As SByte() = New SByte(0){}
			buf(0) = CByte(b And &Hff)
			write(buf, 0, 1)
		End Sub

		''' <summary>
		''' Writes an array of bytes to the compressed output stream. This
		''' method will block until all the bytes are written. </summary>
		''' <param name="b"> the data to be written </param>
		''' <param name="off"> the start offset of the data </param>
		''' <param name="len"> the length of the data </param>
		''' <exception cref="IOException"> if an I/O error has occurred </exception>
		Public Overrides Sub write(ByVal b As SByte(), ByVal [off] As Integer, ByVal len As Integer)
			If def.finished() Then Throw New java.io.IOException("write beyond end of stream")
			If ([off] Or len Or ([off] + len) Or (b.Length - ([off] + len))) < 0 Then
				Throw New IndexOutOfBoundsException
			ElseIf len = 0 Then
				Return
			End If
			If Not def.finished() Then
				def.inputput(b, [off], len)
				Do While Not def.needsInput()
					deflate()
				Loop
			End If
		End Sub

		''' <summary>
		''' Finishes writing compressed data to the output stream without closing
		''' the underlying stream. Use this method when applying multiple filters
		''' in succession to the same output stream. </summary>
		''' <exception cref="IOException"> if an I/O error has occurred </exception>
		Public Overridable Sub finish()
			If Not def.finished() Then
				def.finish()
				Do While Not def.finished()
					deflate()
				Loop
			End If
		End Sub

		''' <summary>
		''' Writes remaining compressed data to the output stream and closes the
		''' underlying stream. </summary>
		''' <exception cref="IOException"> if an I/O error has occurred </exception>
		Public Overrides Sub close()
			If Not closed Then
				finish()
				If usesDefaultDeflater Then def.end()
				out.close()
				closed = True
			End If
		End Sub

		''' <summary>
		''' Writes next block of compressed data to the output stream. </summary>
		''' <exception cref="IOException"> if an I/O error has occurred </exception>
		Protected Friend Overridable Sub deflate()
			Dim len As Integer = def.deflate(buf, 0, buf.Length)
			If len > 0 Then out.write(buf, 0, len)
		End Sub

		''' <summary>
		''' Flushes the compressed output stream.
		''' 
		''' If {@link #DeflaterOutputStream(OutputStream, Deflater, int, boolean)
		''' syncFlush} is {@code true} when this compressed output stream is
		''' constructed, this method first flushes the underlying {@code compressor}
		''' with the flush mode <seealso cref="Deflater#SYNC_FLUSH"/> to force
		''' all pending data to be flushed out to the output stream and then
		''' flushes the output stream. Otherwise this method only flushes the
		''' output stream without flushing the {@code compressor}.
		''' </summary>
		''' <exception cref="IOException"> if an I/O error has occurred
		''' 
		''' @since 1.7 </exception>
		Public Overrides Sub flush()
			If syncFlush AndAlso (Not def.finished()) Then
				Dim len As Integer = 0
				len = def.deflate(buf, 0, buf.Length, Deflater.SYNC_FLUSH)
				Do While len > 0
					out.write(buf, 0, len)
					If len < buf.Length Then Exit Do
					len = def.deflate(buf, 0, buf.Length, Deflater.SYNC_FLUSH)
				Loop
			End If
			out.flush()
		End Sub
	End Class

End Namespace