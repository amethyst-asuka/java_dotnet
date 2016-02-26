Imports Microsoft.VisualBasic
Imports System

'
' * Copyright (c) 2000, 2006, Oracle and/or its affiliates. All rights reserved.
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
	''' output to a regular <code>OutputStream</code>.  A memory buffer is
	''' used to cache at least the data between the discard position and
	''' the current write position.  The only constructor takes an
	''' <code>OutputStream</code>, so this class may not be used for
	''' read/modify/write operations.  Reading can occur only on parts of
	''' the stream that have already been written to the cache and not
	''' yet flushed.
	''' 
	''' </summary>
	Public Class MemoryCacheImageOutputStream
		Inherits ImageOutputStreamImpl

		Private stream As java.io.OutputStream

		Private cache As New MemoryCache

		''' <summary>
		''' Constructs a <code>MemoryCacheImageOutputStream</code> that will write
		''' to a given <code>OutputStream</code>.
		''' </summary>
		''' <param name="stream"> an <code>OutputStream</code> to write to.
		''' </param>
		''' <exception cref="IllegalArgumentException"> if <code>stream</code> is
		''' <code>null</code>. </exception>
		Public Sub New(ByVal stream As java.io.OutputStream)
			If stream Is Nothing Then Throw New System.ArgumentException("stream == null!")
			Me.stream = stream
		End Sub

		Public Overrides Function read() As Integer
			checkClosed()

			bitOffset = 0

			Dim val As Integer = cache.read(streamPos)
			If val <> -1 Then streamPos += 1
			Return val
		End Function

		Public Overrides Function read(ByVal b As SByte(), ByVal [off] As Integer, ByVal len As Integer) As Integer
			checkClosed()

			If b Is Nothing Then Throw New NullPointerException("b == null!")
			' Fix 4467608: read([B,I,I) works incorrectly if len<=0
			If [off] < 0 OrElse len < 0 OrElse [off] + len > b.Length OrElse [off] + len < 0 Then Throw New System.IndexOutOfRangeException("off < 0 || len < 0 || off+len > b.length || off+len < 0!")

			bitOffset = 0

			If len = 0 Then Return 0

			' check if we're already at/past EOF i.e.
			' no more bytes left to read from cache
			Dim bytesLeftInCache As Long = cache.length - streamPos
			If bytesLeftInCache <= 0 Then Return -1 ' EOF

			' guaranteed by now that bytesLeftInCache > 0 && len > 0
			' and so the rest of the error checking is done by cache.read()
			' NOTE that alot of error checking is duplicated
			len = CInt(Fix(Math.Min(bytesLeftInCache, CLng(len))))
			cache.read(b, [off], len, streamPos)
			streamPos += len
			Return len
		End Function

		Public Overrides Sub write(ByVal b As Integer)
			flushBits() ' this will call checkClosed() for us
			cache.write(b, streamPos)
			streamPos += 1
		End Sub

		Public Overrides Sub write(ByVal b As SByte(), ByVal [off] As Integer, ByVal len As Integer)
			flushBits() ' this will call checkClosed() for us
			cache.write(b, [off], len, streamPos)
			streamPos += len
		End Sub

		Public Overrides Function length() As Long
			Try
				checkClosed()
				Return cache.length
			Catch e As java.io.IOException
				Return -1L
			End Try
		End Function

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
		''' Returns <code>false</code> since this
		''' <code>ImageOutputStream</code> does not maintain a file cache.
		''' </summary>
		''' <returns> <code>false</code>.
		''' </returns>
		''' <seealso cref= #isCached </seealso>
		''' <seealso cref= #isCachedMemory </seealso>
		Public Property Overrides cachedFile As Boolean
			Get
				Return False
			End Get
		End Property

		''' <summary>
		''' Returns <code>true</code> since this
		''' <code>ImageOutputStream</code> maintains a main memory cache.
		''' </summary>
		''' <returns> <code>true</code>.
		''' </returns>
		''' <seealso cref= #isCached </seealso>
		''' <seealso cref= #isCachedFile </seealso>
		Public Property Overrides cachedMemory As Boolean
			Get
				Return True
			End Get
		End Property

		''' <summary>
		''' Closes this <code>MemoryCacheImageOutputStream</code>.  All
		''' pending data is flushed to the output, and the cache
		''' is released.  The destination <code>OutputStream</code>
		''' is not closed.
		''' </summary>
		Public Overrides Sub close()
			Dim length As Long = cache.length
			seek(length)
			flushBefore(length)
			MyBase.close()
			cache.reset()
			cache = Nothing
			stream = Nothing
		End Sub

		Public Overrides Sub flushBefore(ByVal pos As Long)
			Dim oFlushedPos As Long = flushedPos
			MyBase.flushBefore(pos) ' this will call checkClosed() for us

			Dim flushBytes As Long = flushedPos - oFlushedPos
			cache.writeToStream(stream, oFlushedPos, flushBytes)
			cache.disposeBefore(flushedPos)
			stream.flush()
		End Sub
	End Class

End Namespace