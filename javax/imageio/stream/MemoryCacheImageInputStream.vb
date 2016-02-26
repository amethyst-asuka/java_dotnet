Imports System.Runtime.CompilerServices

'
' * Copyright (c) 2000, 2007, Oracle and/or its affiliates. All rights reserved.
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
	''' input from a regular <code>InputStream</code>.  A memory buffer is
	''' used to cache at least the data between the discard position and
	''' the current read position.
	''' 
	''' <p> In general, it is preferable to use a
	''' <code>FileCacheImageInputStream</code> when reading from a regular
	''' <code>InputStream</code>.  This class is provided for cases where
	''' it is not possible to create a writable temporary file.
	''' 
	''' </summary>
	Public Class MemoryCacheImageInputStream
		Inherits ImageInputStreamImpl

		Private stream As java.io.InputStream

		Private cache As New MemoryCache

		''' <summary>
		''' The referent to be registered with the Disposer. </summary>
		Private ReadOnly disposerReferent As Object

		''' <summary>
		''' The DisposerRecord that resets the underlying MemoryCache. </summary>
		Private ReadOnly disposerRecord As sun.java2d.DisposerRecord

		''' <summary>
		''' Constructs a <code>MemoryCacheImageInputStream</code> that will read
		''' from a given <code>InputStream</code>.
		''' </summary>
		''' <param name="stream"> an <code>InputStream</code> to read from.
		''' </param>
		''' <exception cref="IllegalArgumentException"> if <code>stream</code> is
		''' <code>null</code>. </exception>
		Public Sub New(ByVal stream As java.io.InputStream)
			If stream Is Nothing Then Throw New System.ArgumentException("stream == null!")
			Me.stream = stream

			disposerRecord = New StreamDisposerRecord(cache)
			If Me.GetType() = GetType(MemoryCacheImageInputStream) Then
				disposerReferent = New Object
				sun.java2d.Disposer.addRecord(disposerReferent, disposerRecord)
			Else
				disposerReferent = New com.sun.imageio.stream.StreamFinalizer(Me)
			End If
		End Sub

		Public Overrides Function read() As Integer
			checkClosed()
			bitOffset = 0
			Dim pos As Long = cache.loadFromStream(stream, streamPos+1)
			If pos >= streamPos+1 Then
					Dim tempVar As Long = streamPos
					streamPos += 1
					Return cache.read(tempVar)
			Else
				Return -1
			End If
		End Function

		Public Overrides Function read(ByVal b As SByte(), ByVal [off] As Integer, ByVal len As Integer) As Integer
			checkClosed()

			If b Is Nothing Then Throw New NullPointerException("b == null!")
			If [off] < 0 OrElse len < 0 OrElse [off] + len > b.Length OrElse [off] + len < 0 Then Throw New System.IndexOutOfRangeException("off < 0 || len < 0 || off+len > b.length || off+len < 0!")

			bitOffset = 0

			If len = 0 Then Return 0

			Dim pos As Long = cache.loadFromStream(stream, streamPos+len)

			len = CInt(pos - streamPos) ' In case stream ended early

			If len > 0 Then
				cache.read(b, [off], len, streamPos)
				streamPos += len
				Return len
			Else
				Return -1
			End If
		End Function

		Public Overrides Sub flushBefore(ByVal pos As Long)
			MyBase.flushBefore(pos) ' this will call checkClosed() for us
			cache.disposeBefore(pos)
		End Sub

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
		''' Returns <code>false</code> since this
		''' <code>ImageInputStream</code> does not maintain a file cache.
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
		''' <code>ImageInputStream</code> maintains a main memory cache.
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
		''' Closes this <code>MemoryCacheImageInputStream</code>, freeing
		''' the cache.  The source <code>InputStream</code> is not closed.
		''' </summary>
		Public Overrides Sub close()
			MyBase.close()
			disposerRecord.Dispose() ' this resets the MemoryCache
			stream = Nothing
			cache = Nothing
		End Sub

		''' <summary>
		''' {@inheritDoc}
		''' </summary>
		Protected Overrides Sub Finalize()
			' Empty finalizer: for performance reasons we instead use the
			' Disposer mechanism for ensuring that the underlying
			' MemoryCache is reset prior to garbage collection
		End Sub

		Private Class StreamDisposerRecord
			Implements sun.java2d.DisposerRecord

			Private cache As MemoryCache

			Public Sub New(ByVal cache As MemoryCache)
				Me.cache = cache
			End Sub

			<MethodImpl(MethodImplOptions.Synchronized)> _
			Public Overridable Sub dispose()
				If cache IsNot Nothing Then
					cache.reset()
					cache = Nothing
				End If
			End Sub
		End Class
	End Class

End Namespace