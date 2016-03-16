Imports System

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

Namespace java.io


	''' <summary>
	''' A character stream whose source is a string.
	''' 
	''' @author      Mark Reinhold
	''' @since       JDK1.1
	''' </summary>

	Public Class StringReader
		Inherits Reader

		Private str As String
		Private length As Integer
		Private [next] As Integer = 0
		Private mark_Renamed As Integer = 0

		''' <summary>
		''' Creates a new string reader.
		''' </summary>
		''' <param name="s">  String providing the character stream. </param>
		Public Sub New(ByVal s As String)
			Me.str = s
			Me.length = s.length()
		End Sub

		''' <summary>
		''' Check to make sure that the stream has not been closed </summary>
		Private Sub ensureOpen()
			If str Is Nothing Then Throw New IOException("Stream closed")
		End Sub

		''' <summary>
		''' Reads a single character.
		''' </summary>
		''' <returns>     The character read, or -1 if the end of the stream has been
		'''             reached
		''' </returns>
		''' <exception cref="IOException">  If an I/O error occurs </exception>
		Public Overrides Function read() As Integer
			SyncLock lock
				ensureOpen()
				If [next] >= length Then Return -1
					Dim tempVar As Integer = [next]
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
					+= 1
					Return str.Chars(tempVar)
			End SyncLock
		End Function

		''' <summary>
		''' Reads characters into a portion of an array.
		''' </summary>
		''' <param name="cbuf">  Destination buffer </param>
		''' <param name="off">   Offset at which to start writing characters </param>
		''' <param name="len">   Maximum number of characters to read
		''' </param>
		''' <returns>     The number of characters read, or -1 if the end of the
		'''             stream has been reached
		''' </returns>
		''' <exception cref="IOException">  If an I/O error occurs </exception>
'JAVA TO VB CONVERTER TODO TASK: The following line could not be converted:
		public int read(char cbuf() , int off, int len) throws IOException
			SyncLock lock
				ensureOpen()
				If (off < 0) OrElse (off > cbuf.length) OrElse (len < 0) OrElse ((off + len) > cbuf.length) OrElse ((off + len) < 0) Then
					Throw New IndexOutOfBoundsException
				ElseIf len = 0 Then
					Return 0
				End If
				If [next] >= length Then Return -1
				Dim n As Integer = System.Math.Min(length - [next], len)
				str.getChars([next], [next] + n, cbuf, off)
				[next] += n
				Return n
			End SyncLock

		''' <summary>
		''' Skips the specified number of characters in the stream. Returns
		''' the number of characters that were skipped.
		''' 
		''' <p>The <code>ns</code> parameter may be negative, even though the
		''' <code>skip</code> method of the <seealso cref="Reader"/> superclass throws
		''' an exception in this case. Negative values of <code>ns</code> cause the
		''' stream to skip backwards. Negative return values indicate a skip
		''' backwards. It is not possible to skip backwards past the beginning of
		''' the string.
		''' 
		''' <p>If the entire string has been read or skipped, then this method has
		''' no effect and always returns 0.
		''' </summary>
		''' <exception cref="IOException">  If an I/O error occurs </exception>
		public Long skip(Long ns) throws IOException
			SyncLock lock
				ensureOpen()
				If [next] >= length Then Return 0
				' Bound skip by beginning and end of the source
				Dim n As Long = System.Math.Min(length - [next], ns)
				n = System.Math.Max(-[next], n)
				[next] += n
				Return n
			End SyncLock

		''' <summary>
		''' Tells whether this stream is ready to be read.
		''' </summary>
		''' <returns> True if the next read() is guaranteed not to block for input
		''' </returns>
		''' <exception cref="IOException">  If the stream is closed </exception>
		public Boolean ready() throws IOException
			SyncLock lock
			ensureOpen()
			Return True
			End SyncLock

		''' <summary>
		''' Tells whether this stream supports the mark() operation, which it does.
		''' </summary>
		public Boolean markSupported()
			Return True

		''' <summary>
		''' Marks the present position in the stream.  Subsequent calls to reset()
		''' will reposition the stream to this point.
		''' </summary>
		''' <param name="readAheadLimit">  Limit on the number of characters that may be
		'''                         read while still preserving the mark.  Because
		'''                         the stream's input comes from a string, there
		'''                         is no actual limit, so this argument must not
		'''                         be negative, but is otherwise ignored.
		''' </param>
		''' <exception cref="IllegalArgumentException">  If {@code readAheadLimit < 0} </exception>
		''' <exception cref="IOException">  If an I/O error occurs </exception>
		public  Sub  mark(Integer readAheadLimit) throws IOException
			If readAheadLimit < 0 Then Throw New IllegalArgumentException("Read-ahead limit < 0")
			SyncLock lock
				ensureOpen()
				mark_Renamed = [next]
			End SyncLock

		''' <summary>
		''' Resets the stream to the most recent mark, or to the beginning of the
		''' string if it has never been marked.
		''' </summary>
		''' <exception cref="IOException">  If an I/O error occurs </exception>
		public  Sub  reset() throws IOException
			SyncLock lock
				ensureOpen()
				[next] = mark_Renamed
			End SyncLock

		''' <summary>
		''' Closes the stream and releases any system resources associated with
		''' it. Once the stream has been closed, further read(),
		''' ready(), mark(), or reset() invocations will throw an IOException.
		''' Closing a previously closed stream has no effect.
		''' </summary>
		public  Sub  close()
			str = Nothing
	End Class

End Namespace