Imports Microsoft.VisualBasic
Imports System
Imports System.Runtime.CompilerServices

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
	''' An implementation of <code>ImageInputStream</code> that gets its
	''' input from a regular <code>InputStream</code>.  A file is used to
	''' cache previously read data.
	''' 
	''' </summary>
	Public Class FileCacheImageInputStream
		Inherits ImageInputStreamImpl

		Private stream As java.io.InputStream

		Private cacheFile As java.io.File

		Private cache As java.io.RandomAccessFile

		Private Const BUFFER_LENGTH As Integer = 1024

		Private buf As SByte() = New SByte(BUFFER_LENGTH - 1){}

		Private length As Long = 0L

		Private foundEOF As Boolean = False

		''' <summary>
		''' The referent to be registered with the Disposer. </summary>
		Private ReadOnly disposerReferent As Object

		''' <summary>
		''' The DisposerRecord that closes the underlying cache. </summary>
		Private ReadOnly disposerRecord As sun.java2d.DisposerRecord

		''' <summary>
		''' The CloseAction that closes the stream in
		'''  the StreamCloser's shutdown hook                     
		''' </summary>
		Private ReadOnly closeAction As com.sun.imageio.stream.StreamCloser.CloseAction

		''' <summary>
		''' Constructs a <code>FileCacheImageInputStream</code> that will read
		''' from a given <code>InputStream</code>.
		''' 
		''' <p> A temporary file is used as a cache.  If
		''' <code>cacheDir</code>is non-<code>null</code> and is a
		''' directory, the file will be created there.  If it is
		''' <code>null</code>, the system-dependent default temporary-file
		''' directory will be used (see the documentation for
		''' <code>File.createTempFile</code> for details).
		''' </summary>
		''' <param name="stream"> an <code>InputStream</code> to read from. </param>
		''' <param name="cacheDir"> a <code>File</code> indicating where the
		''' cache file should be created, or <code>null</code> to use the
		''' system directory.
		''' </param>
		''' <exception cref="IllegalArgumentException"> if <code>stream</code> is
		''' <code>null</code>. </exception>
		''' <exception cref="IllegalArgumentException"> if <code>cacheDir</code> is
		''' non-<code>null</code> but is not a directory. </exception>
		''' <exception cref="IOException"> if a cache file cannot be created. </exception>
		Public Sub New(ByVal stream As java.io.InputStream, ByVal cacheDir As java.io.File)
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

			disposerRecord = New StreamDisposerRecord(cacheFile, cache)
			If Me.GetType() = GetType(FileCacheImageInputStream) Then
				disposerReferent = New Object
				sun.java2d.Disposer.addRecord(disposerReferent, disposerRecord)
			Else
				disposerReferent = New com.sun.imageio.stream.StreamFinalizer(Me)
			End If
		End Sub

		''' <summary>
		''' Ensures that at least <code>pos</code> bytes are cached,
		''' or the end of the source is reached.  The return value
		''' is equal to the smaller of <code>pos</code> and the
		''' length of the source file.
		''' </summary>
		Private Function readUntil(ByVal pos As Long) As Long
			' We've already got enough data cached
			If pos < length Then Return pos
			' pos >= length but length isn't getting any bigger, so return it
			If foundEOF Then Return length

			Dim len As Long = pos - length
			cache.seek(length)
			Do While len > 0
				' Copy a buffer's worth of data from the source to the cache
				' BUFFER_LENGTH will always fit into an int so this is safe
				Dim nbytes As Integer = stream.read(buf, 0, CInt(Fix(Math.Min(len, CLng(BUFFER_LENGTH)))))
				If nbytes = -1 Then
					foundEOF = True
					Return length
				End If

				cache.write(buf, 0, nbytes)
				len -= nbytes
				length += nbytes
			Loop

			Return pos
		End Function

		Public Overrides Function read() As Integer
			checkClosed()
			bitOffset = 0
			Dim [next] As Long = streamPos + 1
			Dim pos As Long = readUntil([next])
			If pos >= [next] Then
				cache.seek(streamPos)
				streamPos += 1
				Return cache.read()
			Else
				Return -1
			End If
		End Function

		Public Overrides Function read(ByVal b As SByte(), ByVal [off] As Integer, ByVal len As Integer) As Integer
			checkClosed()

			If b Is Nothing Then Throw New NullPointerException("b == null!")
			' Fix 4430357 - if off + len < 0, overflow occurred
			If [off] < 0 OrElse len < 0 OrElse [off] + len > b.Length OrElse [off] + len < 0 Then Throw New System.IndexOutOfRangeException("off < 0 || len < 0 || off+len > b.length || off+len < 0!")

			bitOffset = 0

			If len = 0 Then Return 0

			Dim pos As Long = readUntil(streamPos + len)

			' len will always fit into an int so this is safe
			len = CInt(Fix(Math.Min(CLng(len), pos - streamPos)))
			If len > 0 Then
				cache.seek(streamPos)
				cache.readFully(b, [off], len)
				streamPos += len
				Return len
			Else
				Return -1
			End If
		End Function

		''' <summary>
		''' Returns <code>true</code> since this
		''' <code>ImageInputStream</code> caches data in order to allow
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
		''' <code>ImageInputStream</code> maintains a file cache.
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
		''' <code>ImageInputStream</code> does not maintain a main memory
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
		''' Closes this <code>FileCacheImageInputStream</code>, closing
		''' and removing the cache file.  The source <code>InputStream</code>
		''' is not closed.
		''' </summary>
		''' <exception cref="IOException"> if an error occurs. </exception>
		Public Overrides Sub close()
			MyBase.close()
			disposerRecord.Dispose() ' this will close/delete the cache file
			stream = Nothing
			cache = Nothing
			cacheFile = Nothing
			com.sun.imageio.stream.StreamCloser.removeFromQueue(closeAction)
		End Sub

		''' <summary>
		''' {@inheritDoc}
		''' </summary>
		Protected Overrides Sub Finalize()
			' Empty finalizer: for performance reasons we instead use the
			' Disposer mechanism for ensuring that the underlying
			' RandomAccessFile is closed/deleted prior to garbage collection
		End Sub

		Private Class StreamDisposerRecord
			Implements sun.java2d.DisposerRecord

			Private cacheFile As java.io.File
			Private cache As java.io.RandomAccessFile

			Public Sub New(ByVal cacheFile As java.io.File, ByVal cache As java.io.RandomAccessFile)
				Me.cacheFile = cacheFile
				Me.cache = cache
			End Sub

			<MethodImpl(MethodImplOptions.Synchronized)> _
			Public Overridable Sub dispose()
				If cache IsNot Nothing Then
					Try
						cache.close()
					Catch e As java.io.IOException
					Finally
						cache = Nothing
					End Try
				End If
				If cacheFile IsNot Nothing Then
					cacheFile.delete()
					cacheFile = Nothing
				End If
				' Note: Explicit removal of the stream from the StreamCloser
				' queue is not mandatory in this case, as it will be removed
				' automatically by GC shortly after this method is called.
			End Sub
		End Class
	End Class

End Namespace