Imports System

'
' * Copyright (c) 1996, 2005, Oracle and/or its affiliates. All rights reserved.
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
	''' This class implements a character buffer that can be used as an Writer.
	''' The buffer automatically grows when data is written to the stream.  The data
	''' can be retrieved using toCharArray() and toString().
	''' <P>
	''' Note: Invoking close() on this class has no effect, and methods
	''' of this class can be called after the stream has closed
	''' without generating an IOException.
	''' 
	''' @author      Herb Jellinek
	''' @since       JDK1.1
	''' </summary>
	Public Class CharArrayWriter
		Inherits Writer

		''' <summary>
		''' The buffer where data is stored.
		''' </summary>
		Protected Friend buf As Char()

		''' <summary>
		''' The number of chars in the buffer.
		''' </summary>
		Protected Friend count As Integer

		''' <summary>
		''' Creates a new CharArrayWriter.
		''' </summary>
		Public Sub New()
			Me.New(32)
		End Sub

		''' <summary>
		''' Creates a new CharArrayWriter with the specified initial size.
		''' </summary>
		''' <param name="initialSize">  an int specifying the initial buffer size. </param>
		''' <exception cref="IllegalArgumentException"> if initialSize is negative </exception>
		Public Sub New(ByVal initialSize As Integer)
			If initialSize < 0 Then Throw New IllegalArgumentException("Negative initial size: " & initialSize)
			buf = New Char(initialSize - 1){}
		End Sub

		''' <summary>
		''' Writes a character to the buffer.
		''' </summary>
		Public Overrides Sub write(ByVal c As Integer)
			SyncLock lock
				Dim newcount As Integer = count + 1
				If newcount > buf.Length Then buf = java.util.Arrays.copyOf(buf, System.Math.Max(buf.Length << 1, newcount))
				buf(count) = ChrW(c)
				count = newcount
			End SyncLock
		End Sub

		''' <summary>
		''' Writes characters to the buffer. </summary>
		''' <param name="c"> the data to be written </param>
		''' <param name="off">       the start offset in the data </param>
		''' <param name="len">       the number of chars that are written </param>
'JAVA TO VB CONVERTER TODO TASK: The following line could not be converted:
		public void write(char c() , int off, int len)
			If (off < 0) OrElse (off > c.length) OrElse (len < 0) OrElse ((off + len) > c.length) OrElse ((off + len) < 0) Then
				Throw New IndexOutOfBoundsException
			ElseIf len = 0 Then
				Return
			End If
			SyncLock lock
				Dim newcount As Integer = count + len
				If newcount > buf.Length Then buf = java.util.Arrays.copyOf(buf, System.Math.Max(buf.Length << 1, newcount))
				Array.Copy(c, off, buf, count, len)
				count = newcount
			End SyncLock

		''' <summary>
		''' Write a portion of a string to the buffer. </summary>
		''' <param name="str">  String to be written from </param>
		''' <param name="off">  Offset from which to start reading characters </param>
		''' <param name="len">  Number of characters to be written </param>
		public void write(String str, Integer off, Integer len)
			SyncLock lock
				Dim newcount As Integer = count + len
				If newcount > buf.Length Then buf = java.util.Arrays.copyOf(buf, System.Math.Max(buf.Length << 1, newcount))
				str.getChars(off, off + len, buf, count)
				count = newcount
			End SyncLock

		''' <summary>
		''' Writes the contents of the buffer to another character stream.
		''' </summary>
		''' <param name="out">       the output stream to write to </param>
		''' <exception cref="IOException"> If an I/O error occurs. </exception>
		public void writeTo(Writer out) throws IOException
			SyncLock lock
				out.write(buf, 0, count)
			End SyncLock

		''' <summary>
		''' Appends the specified character sequence to this writer.
		''' 
		''' <p> An invocation of this method of the form <tt>out.append(csq)</tt>
		''' behaves in exactly the same way as the invocation
		''' 
		''' <pre>
		'''     out.write(csq.toString()) </pre>
		''' 
		''' <p> Depending on the specification of <tt>toString</tt> for the
		''' character sequence <tt>csq</tt>, the entire sequence may not be
		''' appended. For instance, invoking the <tt>toString</tt> method of a
		''' character buffer will return a subsequence whose content depends upon
		''' the buffer's position and limit.
		''' </summary>
		''' <param name="csq">
		'''         The character sequence to append.  If <tt>csq</tt> is
		'''         <tt>null</tt>, then the four characters <tt>"null"</tt> are
		'''         appended to this writer.
		''' </param>
		''' <returns>  This writer
		''' 
		''' @since  1.5 </returns>
		public CharArrayWriter append(CharSequence csq)
			Dim s As String = (If(csq Is Nothing, "null", csq.ToString()))
			write(s, 0, s.length())
			Return Me

		''' <summary>
		''' Appends a subsequence of the specified character sequence to this writer.
		''' 
		''' <p> An invocation of this method of the form <tt>out.append(csq, start,
		''' end)</tt> when <tt>csq</tt> is not <tt>null</tt>, behaves in
		''' exactly the same way as the invocation
		''' 
		''' <pre>
		'''     out.write(csq.subSequence(start, end).toString()) </pre>
		''' </summary>
		''' <param name="csq">
		'''         The character sequence from which a subsequence will be
		'''         appended.  If <tt>csq</tt> is <tt>null</tt>, then characters
		'''         will be appended as if <tt>csq</tt> contained the four
		'''         characters <tt>"null"</tt>.
		''' </param>
		''' <param name="start">
		'''         The index of the first character in the subsequence
		''' </param>
		''' <param name="end">
		'''         The index of the character following the last character in the
		'''         subsequence
		''' </param>
		''' <returns>  This writer
		''' </returns>
		''' <exception cref="IndexOutOfBoundsException">
		'''          If <tt>start</tt> or <tt>end</tt> are negative, <tt>start</tt>
		'''          is greater than <tt>end</tt>, or <tt>end</tt> is greater than
		'''          <tt>csq.length()</tt>
		''' 
		''' @since  1.5 </exception>
		public CharArrayWriter append(CharSequence csq, Integer start, Integer end)
			Dim s As String = (If(csq Is Nothing, "null", csq)).subSequence(start, end).ToString()
			write(s, 0, s.length())
			Return Me

		''' <summary>
		''' Appends the specified character to this writer.
		''' 
		''' <p> An invocation of this method of the form <tt>out.append(c)</tt>
		''' behaves in exactly the same way as the invocation
		''' 
		''' <pre>
		'''     out.write(c) </pre>
		''' </summary>
		''' <param name="c">
		'''         The 16-bit character to append
		''' </param>
		''' <returns>  This writer
		''' 
		''' @since 1.5 </returns>
		public CharArrayWriter append(Char c)
			write(c)
			Return Me

		''' <summary>
		''' Resets the buffer so that you can use it again without
		''' throwing away the already allocated buffer.
		''' </summary>
		public void reset()
			count = 0

		''' <summary>
		''' Returns a copy of the input data.
		''' </summary>
		''' <returns> an array of chars copied from the input data. </returns>
		public Char ToCharArray()() { synchronized(lock) { Return java.util.Arrays.copyOf(buf, count); } } public Integer size()
		''' <summary>
		''' Returns the current size of the buffer.
		''' </summary>
		''' <returns> an int representing the current size of the buffer. </returns>
			Return count

		''' <summary>
		''' Converts input data to a string. </summary>
		''' <returns> the string. </returns>
		public String ToString()
			SyncLock lock
				Return New String(buf, 0, count)
			End SyncLock

		''' <summary>
		''' Flush the stream.
		''' </summary>
		public void flush()

		''' <summary>
		''' Close the stream.  This method does not release the buffer, since its
		''' contents might still be required. Note: Invoking this method in this class
		''' will have no effect.
		''' </summary>
		public void close()

	End Class

End Namespace