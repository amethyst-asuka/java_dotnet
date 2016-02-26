Imports Microsoft.VisualBasic
Imports System
Imports System.Collections

'
' * Copyright (c) 2000, 2003, Oracle and/or its affiliates. All rights reserved.
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
	''' Package-visible class consolidating common code for
	''' <code>MemoryCacheImageInputStream</code> and
	''' <code>MemoryCacheImageOutputStream</code>.
	''' This class keeps an <code>ArrayList</code> of 8K blocks,
	''' loaded sequentially.  Blocks may only be disposed of
	''' from the index 0 forward.  As blocks are freed, the
	''' corresponding entries in the array list are set to
	''' <code>null</code>, but no compacting is performed.
	''' This allows the index for each block to never change,
	''' and the length of the cache is always the same as the
	''' total amount of data ever cached.  Cached data is
	''' therefore always contiguous from the point of last
	''' disposal to the current length.
	''' 
	''' <p> The total number of blocks resident in the cache must not
	''' exceed <code>Integer.MAX_VALUE</code>.  In practice, the limit of
	''' available memory will be exceeded long before this becomes an
	''' issue, since a full cache would contain 8192*2^31 = 16 terabytes of
	''' data.
	''' 
	''' A <code>MemoryCache</code> may be reused after a call
	''' to <code>reset()</code>.
	''' </summary>
	Friend Class MemoryCache

		Private Const BUFFER_LENGTH As Integer = 8192

		Private cache As New ArrayList

		Private cacheStart As Long = 0L

		''' <summary>
		''' The largest position ever written to the cache.
		''' </summary>
		Private length As Long = 0L

		Private Function getCacheBlock(ByVal blockNum As Long) As SByte()
			Dim blockOffset As Long = blockNum - cacheStart
			If blockOffset > Integer.MaxValue Then Throw New java.io.IOException("Cache addressing limit exceeded!")
			Return CType(cache(CInt(blockOffset)), SByte())
		End Function

		''' <summary>
		''' Ensures that at least <code>pos</code> bytes are cached,
		''' or the end of the source is reached.  The return value
		''' is equal to the smaller of <code>pos</code> and the
		''' length of the source.
		''' </summary>
		Public Overridable Function loadFromStream(ByVal stream As java.io.InputStream, ByVal pos As Long) As Long
			' We've already got enough data cached
			If pos < length Then Return pos

			Dim offset As Integer = CInt(Fix(length Mod BUFFER_LENGTH))
			Dim buf As SByte() = Nothing

			Dim len As Long = pos - length
			If offset <> 0 Then buf = getCacheBlock(length\BUFFER_LENGTH)

			Do While len > 0
				If buf Is Nothing Then
					Try
						buf = New SByte(BUFFER_LENGTH - 1){}
					Catch e As System.OutOfMemoryException
						Throw New java.io.IOException("No memory left for cache!")
					End Try
					offset = 0
				End If

				Dim left As Integer = BUFFER_LENGTH - offset
				Dim nbytes As Integer = CInt(Fix(Math.Min(len, CLng(left))))
				nbytes = stream.read(buf, offset, nbytes)
				If nbytes = -1 Then Return length ' EOF

				If offset = 0 Then cache.Add(buf)

				len -= nbytes
				length += nbytes
				offset += nbytes

				If offset >= BUFFER_LENGTH Then buf = Nothing
			Loop

			Return pos
		End Function

		''' <summary>
		''' Writes out a portion of the cache to an <code>OutputStream</code>.
		''' This method preserves no state about the output stream, and does
		''' not dispose of any blocks containing bytes written.  To dispose
		''' blocks, use <seealso cref="#disposeBefore <code>disposeBefore()</code>"/>.
		''' </summary>
		''' <exception cref="IndexOutOfBoundsException"> if any portion of
		''' the requested data is not in the cache (including if <code>pos</code>
		''' is in a block already disposed), or if either <code>pos</code> or
		''' <code>len</code> is < 0. </exception>
		Public Overridable Sub writeToStream(ByVal stream As java.io.OutputStream, ByVal pos As Long, ByVal len As Long)
			If pos + len > length Then Throw New System.IndexOutOfRangeException("Argument out of cache")
			If (pos < 0) OrElse (len < 0) Then Throw New System.IndexOutOfRangeException("Negative pos or len")
			If len = 0 Then Return

			Dim bufIndex As Long = pos\BUFFER_LENGTH
			If bufIndex < cacheStart Then Throw New System.IndexOutOfRangeException("pos already disposed")
			Dim offset As Integer = CInt(Fix(pos Mod BUFFER_LENGTH))

			Dim buf As SByte() = getCacheBlock(bufIndex)
			bufIndex += 1
			Do While len > 0
				If buf Is Nothing Then
					buf = getCacheBlock(bufIndex)
					bufIndex += 1
					offset = 0
				End If
				Dim nbytes As Integer = CInt(Fix(Math.Min(len, CLng(BUFFER_LENGTH - offset))))
				stream.write(buf, offset, nbytes)
				buf = Nothing
				len -= nbytes
			Loop
		End Sub

		''' <summary>
		''' Ensure that there is space to write a byte at the given position.
		''' </summary>
		Private Sub pad(ByVal pos As Long)
			Dim currIndex As Long = cacheStart + cache.Count - 1
			Dim lastIndex As Long = pos\BUFFER_LENGTH
			Dim numNewBuffers As Long = lastIndex - currIndex
			For i As Long = 0 To numNewBuffers - 1
				Try
					cache.Add(New SByte(BUFFER_LENGTH - 1){})
				Catch e As System.OutOfMemoryException
					Throw New java.io.IOException("No memory left for cache!")
				End Try
			Next i
		End Sub

		''' <summary>
		''' Overwrites and/or appends the cache from a byte array.
		''' The length of the cache will be extended as needed to hold
		''' the incoming data.
		''' </summary>
		''' <param name="b"> an array of bytes containing data to be written. </param>
		''' <param name="off"> the starting offset withing the data array. </param>
		''' <param name="len"> the number of bytes to be written. </param>
		''' <param name="pos"> the cache position at which to begin writing.
		''' </param>
		''' <exception cref="NullPointerException"> if <code>b</code> is <code>null</code>. </exception>
		''' <exception cref="IndexOutOfBoundsException"> if <code>off</code>,
		''' <code>len</code>, or <code>pos</code> are negative,
		''' or if <code>off+len > b.length</code>. </exception>
		Public Overridable Sub write(ByVal b As SByte(), ByVal [off] As Integer, ByVal len As Integer, ByVal pos As Long)
			If b Is Nothing Then Throw New NullPointerException("b == null!")
			' Fix 4430357 - if off + len < 0, overflow occurred
			If ([off] < 0) OrElse (len < 0) OrElse (pos < 0) OrElse ([off] + len > b.Length) OrElse ([off] + len < 0) Then Throw New System.IndexOutOfRangeException

			' Ensure there is space for the incoming data
			Dim lastPos As Long = pos + len - 1
			If lastPos >= length Then
				pad(lastPos)
				length = lastPos + 1
			End If

			' Copy the data into the cache, block by block
			Dim offset As Integer = CInt(Fix(pos Mod BUFFER_LENGTH))
			Do While len > 0
				Dim buf As SByte() = getCacheBlock(pos\BUFFER_LENGTH)
				Dim nbytes As Integer = Math.Min(len, BUFFER_LENGTH - offset)
				Array.Copy(b, [off], buf, offset, nbytes)

				pos += nbytes
				[off] += nbytes
				len -= nbytes
				offset = 0 ' Always after the first time
			Loop
		End Sub

		''' <summary>
		''' Overwrites or appends a single byte to the cache.
		''' The length of the cache will be extended as needed to hold
		''' the incoming data.
		''' </summary>
		''' <param name="b"> an <code>int</code> whose 8 least significant bits
		''' will be written. </param>
		''' <param name="pos"> the cache position at which to begin writing.
		''' </param>
		''' <exception cref="IndexOutOfBoundsException"> if <code>pos</code> is negative. </exception>
		Public Overridable Sub write(ByVal b As Integer, ByVal pos As Long)
			If pos < 0 Then Throw New System.IndexOutOfRangeException("pos < 0")

			' Ensure there is space for the incoming data
			If pos >= length Then
				pad(pos)
				length = pos + 1
			End If

			' Insert the data.
			Dim buf As SByte() = getCacheBlock(pos\BUFFER_LENGTH)
			Dim offset As Integer = CInt(Fix(pos Mod BUFFER_LENGTH))
			buf(offset) = CByte(b)
		End Sub

		''' <summary>
		''' Returns the total length of data that has been cached,
		''' regardless of whether any early blocks have been disposed.
		''' This value will only ever increase.
		''' </summary>
		Public Overridable Property length As Long
			Get
				Return length
			End Get
		End Property

		''' <summary>
		''' Returns the single byte at the given position, as an
		''' <code>int</code>.  Returns -1 if this position has
		''' not been cached or has been disposed.
		''' </summary>
		Public Overridable Function read(ByVal pos As Long) As Integer
			If pos >= length Then Return -1

			Dim buf As SByte() = getCacheBlock(pos\BUFFER_LENGTH)
			If buf Is Nothing Then Return -1

			Return buf(CInt(Fix(pos Mod BUFFER_LENGTH))) And &Hff
		End Function

		''' <summary>
		''' Copy <code>len</code> bytes from the cache, starting
		''' at cache position <code>pos</code>, into the array
		''' <code>b</code> at offset <code>off</code>.
		''' </summary>
		''' <exception cref="NullPointerException"> if b is <code>null</code> </exception>
		''' <exception cref="IndexOutOfBoundsException"> if <code>off</code>,
		''' <code>len</code> or <code>pos</code> are negative or if
		''' <code>off + len > b.length</code> or if any portion of the
		''' requested data is not in the cache (including if
		''' <code>pos</code> is in a block that has already been disposed). </exception>
		Public Overridable Sub read(ByVal b As SByte(), ByVal [off] As Integer, ByVal len As Integer, ByVal pos As Long)
			If b Is Nothing Then Throw New NullPointerException("b == null!")
			' Fix 4430357 - if off + len < 0, overflow occurred
			If ([off] < 0) OrElse (len < 0) OrElse (pos < 0) OrElse ([off] + len > b.Length) OrElse ([off] + len < 0) Then Throw New System.IndexOutOfRangeException
			If pos + len > length Then Throw New System.IndexOutOfRangeException

			Dim index As Long = pos\BUFFER_LENGTH
			Dim offset As Integer = CInt(pos) Mod BUFFER_LENGTH
			Do While len > 0
				Dim nbytes As Integer = Math.Min(len, BUFFER_LENGTH - offset)
				Dim buf As SByte() = getCacheBlock(index)
				index += 1
				Array.Copy(buf, offset, b, [off], nbytes)

				len -= nbytes
				[off] += nbytes
				offset = 0 ' Always after the first time
			Loop
		End Sub

		''' <summary>
		''' Free the blocks up to the position <code>pos</code>.
		''' The byte at <code>pos</code> remains available.
		''' </summary>
		''' <exception cref="IndexOutOfBoundsException"> if <code>pos</code>
		''' is in a block that has already been disposed. </exception>
		Public Overridable Sub disposeBefore(ByVal pos As Long)
			Dim index As Long = pos\BUFFER_LENGTH
			If index < cacheStart Then Throw New System.IndexOutOfRangeException("pos already disposed")
			Dim numBlocks As Long = Math.Min(index - cacheStart, cache.Count)
			For i As Long = 0 To numBlocks - 1
				cache.RemoveAt(0)
			Next i
			Me.cacheStart = index
		End Sub

		''' <summary>
		''' Erase the entire cache contents and reset the length to 0.
		''' The cache object may subsequently be reused as though it had just
		''' been allocated.
		''' </summary>
		Public Overridable Sub reset()
			cache.Clear()
			cacheStart = 0
			length = 0L
		End Sub
	End Class

End Namespace