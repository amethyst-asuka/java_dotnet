Imports System.Collections.Generic

'
' * Copyright (c) 1994, 2011, Oracle and/or its affiliates. All rights reserved.
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
	''' A <code>SequenceInputStream</code> represents
	''' the logical concatenation of other input
	''' streams. It starts out with an ordered
	''' collection of input streams and reads from
	''' the first one until end of file is reached,
	''' whereupon it reads from the second one,
	''' and so on, until end of file is reached
	''' on the last of the contained input streams.
	''' 
	''' @author  Author van Hoff
	''' @since   JDK1.0
	''' </summary>
	Public Class SequenceInputStream
		Inherits java.io.InputStream

'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
		Friend e As System.Collections.IEnumerator(Of ? As java.io.InputStream)
		Friend [in] As java.io.InputStream

		''' <summary>
		''' Initializes a newly created <code>SequenceInputStream</code>
		''' by remembering the argument, which must
		''' be an <code>Enumeration</code>  that produces
		''' objects whose run-time type is <code>InputStream</code>.
		''' The input streams that are  produced by
		''' the enumeration will be read, in order,
		''' to provide the bytes to be read  from this
		''' <code>SequenceInputStream</code>. After
		''' each input stream from the enumeration
		''' is exhausted, it is closed by calling its
		''' <code>close</code> method.
		''' </summary>
		''' <param name="e">   an enumeration of input streams. </param>
		''' <seealso cref=     java.util.Enumeration </seealso>
		Public Sub New(Of T1 As java.io.InputStream)(ByVal e As System.Collections.IEnumerator(Of T1))
			Me.e = e
			Try
				nextStream()
			Catch ex As IOException
				' This should never happen
				Throw New [Error]("panic")
			End Try
		End Sub

		''' <summary>
		''' Initializes a newly
		''' created <code>SequenceInputStream</code>
		''' by remembering the two arguments, which
		''' will be read in order, first <code>s1</code>
		''' and then <code>s2</code>, to provide the
		''' bytes to be read from this <code>SequenceInputStream</code>.
		''' </summary>
		''' <param name="s1">   the first input stream to read. </param>
		''' <param name="s2">   the second input stream to read. </param>
		Public Sub New(ByVal s1 As java.io.InputStream, ByVal s2 As java.io.InputStream)
			Dim v As New List(Of java.io.InputStream)(2)

			v.Add(s1)
			v.Add(s2)
			e = v.elements()
			Try
				nextStream()
			Catch ex As IOException
				' This should never happen
				Throw New [Error]("panic")
			End Try
		End Sub

		''' <summary>
		'''  Continues reading in the next stream if an EOF is reached.
		''' </summary>
		Friend Sub nextStream()
			If [in] IsNot Nothing Then [in].close()

'JAVA TO VB CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
			If e.hasMoreElements() Then
'JAVA TO VB CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
				[in] = CType(e.nextElement(), java.io.InputStream)
				If [in] Is Nothing Then Throw New NullPointerException
			Else
				[in] = Nothing
			End If

		End Sub

		''' <summary>
		''' Returns an estimate of the number of bytes that can be read (or
		''' skipped over) from the current underlying input stream without
		''' blocking by the next invocation of a method for the current
		''' underlying input stream. The next invocation might be
		''' the same thread or another thread.  A single read or skip of this
		''' many bytes will not block, but may read or skip fewer bytes.
		''' <p>
		''' This method simply calls {@code available} of the current underlying
		''' input stream and returns the result.
		''' </summary>
		''' <returns> an estimate of the number of bytes that can be read (or
		'''         skipped over) from the current underlying input stream
		'''         without blocking or {@code 0} if this input stream
		'''         has been closed by invoking its <seealso cref="#close()"/> method </returns>
		''' <exception cref="IOException">  if an I/O error occurs.
		''' 
		''' @since   JDK1.1 </exception>
		Public Overrides Function available() As Integer
			If [in] Is Nothing Then Return 0 ' no way to signal EOF from available()
			Return [in].available()
		End Function

		''' <summary>
		''' Reads the next byte of data from this input stream. The byte is
		''' returned as an <code>int</code> in the range <code>0</code> to
		''' <code>255</code>. If no byte is available because the end of the
		''' stream has been reached, the value <code>-1</code> is returned.
		''' This method blocks until input data is available, the end of the
		''' stream is detected, or an exception is thrown.
		''' <p>
		''' This method
		''' tries to read one character from the current substream. If it
		''' reaches the end of the stream, it calls the <code>close</code>
		''' method of the current substream and begins reading from the next
		''' substream.
		''' </summary>
		''' <returns>     the next byte of data, or <code>-1</code> if the end of the
		'''             stream is reached. </returns>
		''' <exception cref="IOException">  if an I/O error occurs. </exception>
		Public Overrides Function read() As Integer
			Do While [in] IsNot Nothing
				Dim c As Integer = [in].read()
				If c <> -1 Then Return c
				nextStream()
			Loop
			Return -1
		End Function

		''' <summary>
		''' Reads up to <code>len</code> bytes of data from this input stream
		''' into an array of bytes.  If <code>len</code> is not zero, the method
		''' blocks until at least 1 byte of input is available; otherwise, no
		''' bytes are read and <code>0</code> is returned.
		''' <p>
		''' The <code>read</code> method of <code>SequenceInputStream</code>
		''' tries to read the data from the current substream. If it fails to
		''' read any characters because the substream has reached the end of
		''' the stream, it calls the <code>close</code> method of the current
		''' substream and begins reading from the next substream.
		''' </summary>
		''' <param name="b">     the buffer into which the data is read. </param>
		''' <param name="off">   the start offset in array <code>b</code>
		'''                   at which the data is written. </param>
		''' <param name="len">   the maximum number of bytes read. </param>
		''' <returns>     int   the number of bytes read. </returns>
		''' <exception cref="NullPointerException"> If <code>b</code> is <code>null</code>. </exception>
		''' <exception cref="IndexOutOfBoundsException"> If <code>off</code> is negative,
		''' <code>len</code> is negative, or <code>len</code> is greater than
		''' <code>b.length - off</code> </exception>
		''' <exception cref="IOException">  if an I/O error occurs. </exception>
'JAVA TO VB CONVERTER TODO TASK: The following line could not be converted:
		public int read(byte b() , int off, int len) throws IOException
			If [in] Is Nothing Then
				Return -1
			ElseIf b Is Nothing Then
				Throw New NullPointerException
			ElseIf off < 0 OrElse len < 0 OrElse len > b.length - off Then
				Throw New IndexOutOfBoundsException
			ElseIf len = 0 Then
				Return 0
			End If
			Do
				Dim n As Integer = [in].read(b, off, len)
				If n > 0 Then Return n
				nextStream()
			Loop While [in] IsNot Nothing
			Return -1

		''' <summary>
		''' Closes this input stream and releases any system resources
		''' associated with the stream.
		''' A closed <code>SequenceInputStream</code>
		''' cannot  perform input operations and cannot
		''' be reopened.
		''' <p>
		''' If this stream was created
		''' from an enumeration, all remaining elements
		''' are requested from the enumeration and closed
		''' before the <code>close</code> method returns.
		''' </summary>
		''' <exception cref="IOException">  if an I/O error occurs. </exception>
		public  Sub  close() throws IOException
			Do
				nextStream()
			Loop While [in] IsNot Nothing
	End Class

End Namespace