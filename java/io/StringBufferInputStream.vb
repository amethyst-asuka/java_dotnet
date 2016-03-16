Imports System
Imports System.Runtime.CompilerServices

'
' * Copyright (c) 1995, 2004, Oracle and/or its affiliates. All rights reserved.
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
	''' This class allows an application to create an input stream in
	''' which the bytes read are supplied by the contents of a string.
	''' Applications can also read bytes from a byte array by using a
	''' <code>ByteArrayInputStream</code>.
	''' <p>
	''' Only the low eight bits of each character in the string are used by
	''' this class.
	''' 
	''' @author     Arthur van Hoff </summary>
	''' <seealso cref=        java.io.ByteArrayInputStream </seealso>
	''' <seealso cref=        java.io.StringReader
	''' @since      JDK1.0 </seealso>
	''' @deprecated This class does not properly convert characters into bytes.  As
	'''             of JDK&nbsp;1.1, the preferred way to create a stream from a
	'''             string is via the <code>StringReader</code> class. 
	<Obsolete("This class does not properly convert characters into bytes.  As")> _
	Public Class StringBufferInputStream
		Inherits InputStream

		''' <summary>
		''' The string from which bytes are read.
		''' </summary>
		Protected Friend buffer As String

		''' <summary>
		''' The index of the next character to read from the input stream buffer.
		''' </summary>
		''' <seealso cref=        java.io.StringBufferInputStream#buffer </seealso>
		Protected Friend pos As Integer

		''' <summary>
		''' The number of valid characters in the input stream buffer.
		''' </summary>
		''' <seealso cref=        java.io.StringBufferInputStream#buffer </seealso>
		Protected Friend count As Integer

		''' <summary>
		''' Creates a string input stream to read data from the specified string.
		''' </summary>
		''' <param name="s">   the underlying input buffer. </param>
		Public Sub New(ByVal s As String)
			Me.buffer = s
			count = s.length()
		End Sub

		''' <summary>
		''' Reads the next byte of data from this input stream. The value
		''' byte is returned as an <code>int</code> in the range
		''' <code>0</code> to <code>255</code>. If no byte is available
		''' because the end of the stream has been reached, the value
		''' <code>-1</code> is returned.
		''' <p>
		''' The <code>read</code> method of
		''' <code>StringBufferInputStream</code> cannot block. It returns the
		''' low eight bits of the next character in this input stream's buffer.
		''' </summary>
		''' <returns>     the next byte of data, or <code>-1</code> if the end of the
		'''             stream is reached. </returns>
		<MethodImpl(MethodImplOptions.Synchronized)> _
		Public Overrides Function read() As Integer
				Dim tempVar As Integer = pos
				pos += 1
				Return If(pos < count, (AscW(buffer.Chars(tempVar)) And &HFF), -1)
		End Function

		''' <summary>
		''' Reads up to <code>len</code> bytes of data from this input stream
		''' into an array of bytes.
		''' <p>
		''' The <code>read</code> method of
		''' <code>StringBufferInputStream</code> cannot block. It copies the
		''' low eight bits from the characters in this input stream's buffer into
		''' the byte array argument.
		''' </summary>
		''' <param name="b">     the buffer into which the data is read. </param>
		''' <param name="off">   the start offset of the data. </param>
		''' <param name="len">   the maximum number of bytes read. </param>
		''' <returns>     the total number of bytes read into the buffer, or
		'''             <code>-1</code> if there is no more data because the end of
		'''             the stream has been reached. </returns>
'JAVA TO VB CONVERTER TODO TASK: The following line could not be converted:
		public synchronized int read(byte b() , int off, int len)
			If b Is Nothing Then
				Throw New NullPointerException
			ElseIf (off < 0) OrElse (off > b.length) OrElse (len < 0) OrElse ((off + len) > b.length) OrElse ((off + len) < 0) Then
				Throw New IndexOutOfBoundsException
			End If
			If pos >= count Then Return -1
			If pos + len > count Then len = count - pos
			If len <= 0 Then Return 0
			Dim s As String = buffer
			Dim cnt As Integer = len
			cnt -= 1
			Do While cnt >= 0
				b(off) = AscW(s.Chars(pos))
				pos += 1
				off += 1
				cnt -= 1
			Loop

			Return len

		''' <summary>
		''' Skips <code>n</code> bytes of input from this input stream. Fewer
		''' bytes might be skipped if the end of the input stream is reached.
		''' </summary>
		''' <param name="n">   the number of bytes to be skipped. </param>
		''' <returns>     the actual number of bytes skipped. </returns>
		public synchronized Long skip(Long n)
			If n < 0 Then Return 0
			If n > count - pos Then n = count - pos
			pos += n
			Return n

		''' <summary>
		''' Returns the number of bytes that can be read from the input
		''' stream without blocking.
		''' </summary>
		''' <returns>     the value of <code>count&nbsp;-&nbsp;pos</code>, which is the
		'''             number of bytes remaining to be read from the input buffer. </returns>
		public synchronized Integer available()
			Return count - pos

		''' <summary>
		''' Resets the input stream to begin reading from the first character
		''' of this input stream's underlying buffer.
		''' </summary>
		public synchronized  Sub  reset()
			pos = 0
	End Class

End Namespace