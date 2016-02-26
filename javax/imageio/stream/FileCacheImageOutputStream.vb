Imports Microsoft.VisualBasic
Imports System

'
' * Copyright (c) 2000, 2012, Oracle and/or its affiliates. All rights reserved.
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
	''' An implementation of <code>ImageOutputStream</code> that writes its
	''' output to a regular <code>OutputStream</code>.  A file is used to
	''' cache data until it is flushed to the output stream.
	''' 
	''' </summary>
	Public Class FileCacheImageOutputStream
		Inherits ImageOutputStreamImpl

		Private stream As java.io.OutputStream

		Private cacheFile As java.io.File

		Private cache As java.io.RandomAccessFile

		' Pos after last (rightmost) byte written
		Private maxStreamPos As Long = 0L

		''' <summary>
		''' The CloseAction that closes the stream in
		'''  the StreamCloser's shutdown hook                     
		''' </summary>
		Private ReadOnly closeAction As com.sun.imageio.stream.StreamCloser.CloseAction

		''' <summary>
		''' Constructs a <code>FileCacheImageOutputStream</code> that will write
		''' to a given <code>outputStream</code>.
		''' 
		''' <p> A temporary file is used as a cache.  If
		''' <code>cacheDir</code>is non-<code>null</code> and is a
		''' directory, the file will be created there.  If it is
		''' <code>null</code>, the system-dependent default temporary-file
		''' directory will be used (see the documentation for
		''' <code>File.createTempFile</code> for details).
		''' </summary>
		''' <param name="stream"> an <code>OutputStream</code> to write to. </param>
		''' <param name="cacheDir"> a <code>File</code> indicating where the
		''' cache file should be created, or <code>null</code> to use the
		''' system directory.
		''' </param>
		''' <exception cref="IllegalArgumentException"> if <code>stream</code>
		''' is <code>null</code>. </exception>
		''' <exception cref="IllegalArgumentException"> if <code>cacheDir</code> is
		''' non-<code>null</code> but is not a directory. </exception>
		''' <exception cref="IOException"> if a cache file cannot be created. </exception>
		Public Sub New(ByVal stream As java.io.OutputStream, ByVal cacheDir As java.io.File)
			If stream Is Nothing Then Throw New System.ArgumentException("stream == null!")
			If (cacheDir IsNot Nothing) AndAlso Not(cacheDir.directory) Then Throw New System.ArgumentException("Not a directory!")
			Me.stream = stream
			If cacheDir Is Nothing Then
				Me.cacheFile = java.nio.file.Files.createTempFile("imageio", ".tmp").toFile()
			Else
				Me.cacheFile = java.nio.file.Files.createTempFile(cacheDir.toPath(), "imageio", ".tmp").toFile()
			End If
			Me.cache = New java.io.RandomAccessFile(cacheFile, "rw")

			Me.closeAction = com.sun.imageio.stream.StreamCloser.createCloseAction(Me)
			com.sun.imageio.stream.StreamCloser.addToQueue(closeAction)
		End Sub

		Public Overrides Function read() As Integer
			checkClosed()
			bitOffset = 0
			Dim val As Integer = cache.read()
			If val <> -1 Then streamPos += 1
			Return val
		End Function

		Public Overrides Function read(ByVal b As SByte(), ByVal [off] As Integer, ByVal len As Integer) As Integer
			checkClosed()

			If b Is Nothing Then Throw New NullPointerException("b == null!")
			If [off] < 0 OrElse len < 0 OrElse [off] + len > b.Length OrElse [off] + len < 0 Then Throw New System.IndexOutOfRangeException("off < 0 || len < 0 || off+len > b.length || off+len < 0!")

			bitOffset = 0

			If len = 0 Then Return 0

			Dim nbytes As Integer = cache.read(b, [off], len)
			If nbytes <> -1 Then streamPos += nbytes
			Return nbytes
		End Function

		Public Overrides Sub write(ByVal b As Integer)
			flushBits() ' this will call checkClosed() for us
			cache.write(b)
			streamPos += 1
			maxStreamPos = Math.Max(maxStreamPos, streamPos)
		End Sub

		Public Overrides Sub write(ByVal b As SByte(), ByVal [off] As Integer, ByVal len As Integer)
			flushBits() ' this will call checkClosed() for us
			cache.write(b, [off], len)
			streamPos += len
			maxStreamPos = Math.Max(maxStreamPos, streamPos)
		End Sub

		Public Overrides Function length() As Long
			Try
				checkClosed()
				Return cache.length()
			Catch e As java.io.IOException
				Return -1L
			End Try
		End Function

		''' <summary>
		''' Sets the current stream position and resets the bit offset to
		''' 0.  It is legal to seek past the end of the file; an
		''' <code>EOFException</code> will be thrown only if a read is
		''' performed.  The file length will not be increased until a write
		''' is performed.
		''' </summary>
		''' <exception cref="IndexOutOfBoundsException"> if <code>pos</code> is smaller
		''' than the flushed position. </exception>
		''' <exception cref="IOException"> if any other I/O error occurs. </exception>
		Public Overrides Sub seek(ByVal pos As Long)
			checkClosed()

			If pos < flushedPos Then Throw New System.IndexOutOfRangeException

			cache.seek(pos)
			Me.streamPos = cache.filePointer
			maxStreamPos = Math.Max(maxStreamPos, streamPos)
			Me.bitOffset = 0
		End Sub

		''' <summary>
		''' Returns <code>true</code> since this
		''' <code>ImageOutputStream</code> caches data in order to allow
		''' seeking backwards.
		''' </summary>
		''' <returns> <code>true</code>.
		''' </returns>
		''' <seealso cref= #isCachedMemory </seealso>
		''' <seealso cref= #isCachedFile </seealso>
		Public Property Overrides cached As Boolean
			Get
				Return True
			End Get
		End Property

		''' <summary>
		''' Returns <code>true</code> since this
		''' <code>ImageOutputStream</code> maintains a file cache.
		''' </summary>
		''' <returns> <code>true</code>.
		''' </returns>
		''' <seealso cref= #isCached </seealso>
		''' <seealso cref= #isCachedMemory </seealso>
		Public Property Overrides cachedFile As Boolean
			Get
				Return True
			End Get
		End Property

		''' <summary>
		''' Returns <code>false</code> since this
		''' <code>ImageOutputStream</code> does not maintain a main memory
		''' cache.
		''' </summary>
		''' <returns> <code>false</code>.
		''' </returns>
		''' <seealso cref= #isCached </seealso>
		''' <seealso cref= #isCachedFile </seealso>
		Public Property Overrides cachedMemory As Boolean
			Get
				Return False
			End Get
		End Property

		''' <summary>
		''' Closes this <code>FileCacheImageOutputStream</code>.  All
		''' pending data is flushed to the output, and the cache file
		''' is closed and removed.  The destination <code>OutputStream</code>
		''' is not closed.
		''' </summary>
		''' <exception cref="IOException"> if an error occurs. </exception>
		Public Overrides Sub close()
			maxStreamPos = cache.length()

			seek(maxStreamPos)
			flushBefore(maxStreamPos)
			MyBase.close()
			cache.close()
			cache = Nothing
			cacheFile.delete()
			cacheFile = Nothing
			stream.flush()
			stream = Nothing
			com.sun.imageio.stream.StreamCloser.removeFromQueue(closeAction)
		End Sub

		Public Overrides Sub flushBefore(ByVal pos As Long)
			Dim oFlushedPos As Long = flushedPos
			MyBase.flushBefore(pos) ' this will call checkClosed() for us

			Dim flushBytes As Long = flushedPos - oFlushedPos
			If flushBytes > 0 Then
				Dim bufLen As Integer = 512
				Dim buf As SByte() = New SByte(bufLen - 1){}
				cache.seek(oFlushedPos)
				Do While flushBytes > 0
					Dim len As Integer = CInt(Fix(Math.Min(flushBytes, bufLen)))
					cache.readFully(buf, 0, len)
					stream.write(buf, 0, len)
					flushBytes -= len
				Loop
				stream.flush()
			End If
		End Sub
	End Class

End Namespace