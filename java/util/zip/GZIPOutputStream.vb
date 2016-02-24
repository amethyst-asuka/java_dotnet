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
	''' This class implements a stream filter for writing compressed data in
	''' the GZIP file format.
	''' @author      David Connelly
	''' 
	''' </summary>
	Public Class GZIPOutputStream
		Inherits DeflaterOutputStream

		''' <summary>
		''' CRC-32 of uncompressed data.
		''' </summary>
		Protected Friend crc As New CRC32

	'    
	'     * GZIP header magic number.
	'     
		Private Const GZIP_MAGIC As Integer = &H8b1f

	'    
	'     * Trailer size in bytes.
	'     *
	'     
		Private Const TRAILER_SIZE As Integer = 8

		''' <summary>
		''' Creates a new output stream with the specified buffer size.
		''' 
		''' <p>The new output stream instance is created as if by invoking
		''' the 3-argument constructor GZIPOutputStream(out, size, false).
		''' </summary>
		''' <param name="out"> the output stream </param>
		''' <param name="size"> the output buffer size </param>
		''' <exception cref="IOException"> If an I/O error has occurred. </exception>
		''' <exception cref="IllegalArgumentException"> if {@code size <= 0} </exception>
		Public Sub New(ByVal out As java.io.OutputStream, ByVal size As Integer)
			Me.New(out, size, False)
		End Sub

		''' <summary>
		''' Creates a new output stream with the specified buffer size and
		''' flush mode.
		''' </summary>
		''' <param name="out"> the output stream </param>
		''' <param name="size"> the output buffer size </param>
		''' <param name="syncFlush">
		'''        if {@code true} invocation of the inherited
		'''        <seealso cref="DeflaterOutputStream#flush() flush()"/> method of
		'''        this instance flushes the compressor with flush mode
		'''        <seealso cref="Deflater#SYNC_FLUSH"/> before flushing the output
		'''        stream, otherwise only flushes the output stream </param>
		''' <exception cref="IOException"> If an I/O error has occurred. </exception>
		''' <exception cref="IllegalArgumentException"> if {@code size <= 0}
		''' 
		''' @since 1.7 </exception>
		Public Sub New(ByVal out As java.io.OutputStream, ByVal size As Integer, ByVal syncFlush As Boolean)
			MyBase.New(out, New Deflater(Deflater.DEFAULT_COMPRESSION, True), size, syncFlush)
			usesDefaultDeflater = True
			writeHeader()
			crc.reset()
		End Sub


		''' <summary>
		''' Creates a new output stream with a default buffer size.
		''' 
		''' <p>The new output stream instance is created as if by invoking
		''' the 2-argument constructor GZIPOutputStream(out, false).
		''' </summary>
		''' <param name="out"> the output stream </param>
		''' <exception cref="IOException"> If an I/O error has occurred. </exception>
		Public Sub New(ByVal out As java.io.OutputStream)
			Me.New(out, 512, False)
		End Sub

		''' <summary>
		''' Creates a new output stream with a default buffer size and
		''' the specified flush mode.
		''' </summary>
		''' <param name="out"> the output stream </param>
		''' <param name="syncFlush">
		'''        if {@code true} invocation of the inherited
		'''        <seealso cref="DeflaterOutputStream#flush() flush()"/> method of
		'''        this instance flushes the compressor with flush mode
		'''        <seealso cref="Deflater#SYNC_FLUSH"/> before flushing the output
		'''        stream, otherwise only flushes the output stream
		''' </param>
		''' <exception cref="IOException"> If an I/O error has occurred.
		''' 
		''' @since 1.7 </exception>
		Public Sub New(ByVal out As java.io.OutputStream, ByVal syncFlush As Boolean)
			Me.New(out, 512, syncFlush)
		End Sub

		''' <summary>
		''' Writes array of bytes to the compressed output stream. This method
		''' will block until all the bytes are written. </summary>
		''' <param name="buf"> the data to be written </param>
		''' <param name="off"> the start offset of the data </param>
		''' <param name="len"> the length of the data </param>
		''' <exception cref="IOException"> If an I/O error has occurred. </exception>
		<MethodImpl(MethodImplOptions.Synchronized)> _
		Public Overrides Sub write(ByVal buf As SByte(), ByVal [off] As Integer, ByVal len As Integer)
			MyBase.write(buf, [off], len)
			crc.update(buf, [off], len)
		End Sub

		''' <summary>
		''' Finishes writing compressed data to the output stream without closing
		''' the underlying stream. Use this method when applying multiple filters
		''' in succession to the same output stream. </summary>
		''' <exception cref="IOException"> if an I/O error has occurred </exception>
		Public Overrides Sub finish()
			If Not def.finished() Then
				def.finish()
				Do While Not def.finished()
					Dim len As Integer = def.deflate(buf, 0, buf.Length)
					If def.finished() AndAlso len <= buf.Length - TRAILER_SIZE Then
						' last deflater buffer. Fit trailer at the end
						writeTrailer(buf, len)
						len = len + TRAILER_SIZE
						out.write(buf, 0, len)
						Return
					End If
					If len > 0 Then out.write(buf, 0, len)
				Loop
				' if we can't fit the trailer at the end of the last
				' deflater buffer, we write it separately
				Dim trailer As SByte() = New SByte(TRAILER_SIZE - 1){}
				writeTrailer(trailer, 0)
				out.write(trailer)
			End If
		End Sub

	'    
	'     * Writes GZIP member header.
	'     
		Private Sub writeHeader()
			out.write(New SByte() { CByte(GZIP_MAGIC), CByte(GZIP_MAGIC >> 8), Deflater.DEFLATED, 0, 0, 0, 0, 0, 0, 0 }) ' Operating system (OS) -  Extra flags (XFLG) -  Modification time MTIME (int) -  Modification time MTIME (int) -  Modification time MTIME (int) -  Modification time MTIME (int) -  Flags (FLG) -  Compression method (CM) -  Magic number (short) -  Magic number (short)
		End Sub

	'    
	'     * Writes GZIP member trailer to a byte array, starting at a given
	'     * offset.
	'     
		Private Sub writeTrailer(ByVal buf As SByte(), ByVal offset As Integer)
			writeInt(CInt(crc.value), buf, offset) ' CRC-32 of uncompr. data
			writeInt(def.totalIn, buf, offset + 4) ' Number of uncompr. bytes
		End Sub

	'    
	'     * Writes integer in Intel byte order to a byte array, starting at a
	'     * given offset.
	'     
		Private Sub writeInt(ByVal i As Integer, ByVal buf As SByte(), ByVal offset As Integer)
			writeShort(i And &Hffff, buf, offset)
			writeShort((i >> 16) And &Hffff, buf, offset + 2)
		End Sub

	'    
	'     * Writes short integer in Intel byte order to a byte array, starting
	'     * at a given offset
	'     
		Private Sub writeShort(ByVal s As Integer, ByVal buf As SByte(), ByVal offset As Integer)
			buf(offset) = CByte(s And &Hff)
			buf(offset + 1) = CByte((s >> 8) And &Hff)
		End Sub
	End Class

End Namespace