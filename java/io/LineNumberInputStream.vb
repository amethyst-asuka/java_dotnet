Imports Microsoft.VisualBasic
Imports System

'
' * Copyright (c) 1995, 2012, Oracle and/or its affiliates. All rights reserved.
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
	''' This class is an input stream filter that provides the added
	''' functionality of keeping track of the current line number.
	''' <p>
	''' A line is a sequence of bytes ending with a carriage return
	''' character ({@code '\u005Cr'}), a newline character
	''' ({@code '\u005Cn'}), or a carriage return character followed
	''' immediately by a linefeed character. In all three cases, the line
	''' terminating character(s) are returned as a single newline character.
	''' <p>
	''' The line number begins at {@code 0}, and is incremented by
	''' {@code 1} when a {@code read} returns a newline character.
	''' 
	''' @author     Arthur van Hoff </summary>
	''' <seealso cref=        java.io.LineNumberReader
	''' @since      JDK1.0 </seealso>
	''' @deprecated This class incorrectly assumes that bytes adequately represent
	'''             characters.  As of JDK&nbsp;1.1, the preferred way to operate on
	'''             character streams is via the new character-stream classes, which
	'''             include a class for counting line numbers. 
	<Obsolete("This class incorrectly assumes that bytes adequately represent")> _
	Public Class LineNumberInputStream
		Inherits FilterInputStream

		Friend pushBack As Integer = -1
		Friend lineNumber As Integer
		Friend markLineNumber As Integer
		Friend markPushBack As Integer = -1

		''' <summary>
		''' Constructs a newline number input stream that reads its input
		''' from the specified input stream.
		''' </summary>
		''' <param name="in">   the underlying input stream. </param>
		Public Sub New(ByVal [in] As InputStream)
			MyBase.New([in])
		End Sub

		''' <summary>
		''' Reads the next byte of data from this input stream. The value
		''' byte is returned as an {@code int} in the range
		''' {@code 0} to {@code 255}. If no byte is available
		''' because the end of the stream has been reached, the value
		''' {@code -1} is returned. This method blocks until input data
		''' is available, the end of the stream is detected, or an exception
		''' is thrown.
		''' <p>
		''' The {@code read} method of
		''' {@code LineNumberInputStream} calls the {@code read}
		''' method of the underlying input stream. It checks for carriage
		''' returns and newline characters in the input, and modifies the
		''' current line number as appropriate. A carriage-return character or
		''' a carriage return followed by a newline character are both
		''' converted into a single newline character.
		''' </summary>
		''' <returns>     the next byte of data, or {@code -1} if the end of this
		'''             stream is reached. </returns>
		''' <exception cref="IOException">  if an I/O error occurs. </exception>
		''' <seealso cref=        java.io.FilterInputStream#in </seealso>
		''' <seealso cref=        java.io.LineNumberInputStream#getLineNumber() </seealso>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
		Public Overrides Function read() As Integer
			Dim c As Integer = pushBack

			If c <> -1 Then
				pushBack = -1
			Else
				c = [in].read()
			End If

			Select Case c
			  Case ControlChars.Cr
				pushBack = [in].read()
				If pushBack = ControlChars.Lf Then pushBack = -1
'JAVA TO VB CONVERTER TODO TASK: VB does not allow fall-through from a non-empty 'case':
			  Case ControlChars.Lf
				lineNumber += 1
				Return ControlChars.Lf
			End Select
			Return c
		End Function

		''' <summary>
		''' Reads up to {@code len} bytes of data from this input stream
		''' into an array of bytes. This method blocks until some input is available.
		''' <p>
		''' The {@code read} method of
		''' {@code LineNumberInputStream} repeatedly calls the
		''' {@code read} method of zero arguments to fill in the byte array.
		''' </summary>
		''' <param name="b">     the buffer into which the data is read. </param>
		''' <param name="off">   the start offset of the data. </param>
		''' <param name="len">   the maximum number of bytes read. </param>
		''' <returns>     the total number of bytes read into the buffer, or
		'''             {@code -1} if there is no more data because the end of
		'''             this stream has been reached. </returns>
		''' <exception cref="IOException">  if an I/O error occurs. </exception>
		''' <seealso cref=        java.io.LineNumberInputStream#read() </seealso>
'JAVA TO VB CONVERTER TODO TASK: The following line could not be converted:
		public int read(byte b() , int off, int len) throws IOException
			If b Is Nothing Then
				Throw New NullPointerException
			ElseIf (off < 0) OrElse (off > b.length) OrElse (len < 0) OrElse ((off + len) > b.length) OrElse ((off + len) < 0) Then
				Throw New IndexOutOfBoundsException
			ElseIf len = 0 Then
				Return 0
			End If

			Dim c As Integer = read()
			If c = -1 Then Return -1
			b(off) = CByte(c)

			Dim i As Integer = 1
			Try
				Do While i < len
					c = read()
					If c = -1 Then Exit Do
					If b IsNot Nothing Then b(off + i) = CByte(c)
					i += 1
				Loop
			Catch ee As IOException
			End Try
			Return i

		''' <summary>
		''' Skips over and discards {@code n} bytes of data from this
		''' input stream. The {@code skip} method may, for a variety of
		''' reasons, end up skipping over some smaller number of bytes,
		''' possibly {@code 0}. The actual number of bytes skipped is
		''' returned.  If {@code n} is negative, no bytes are skipped.
		''' <p>
		''' The {@code skip} method of {@code LineNumberInputStream} creates
		''' a byte array and then repeatedly reads into it until
		''' {@code n} bytes have been read or the end of the stream has
		''' been reached.
		''' </summary>
		''' <param name="n">   the number of bytes to be skipped. </param>
		''' <returns>     the actual number of bytes skipped. </returns>
		''' <exception cref="IOException">  if an I/O error occurs. </exception>
		''' <seealso cref=        java.io.FilterInputStream#in </seealso>
		public Long skip(Long n) throws IOException
			Dim chunk As Integer = 2048
			Dim remaining As Long = n
			Dim data As SByte()
			Dim nr As Integer

			If n <= 0 Then Return 0

			data = New SByte(chunk - 1){}
			Do While remaining > 0
				nr = read(data, 0, CInt(Fix(Math.Min(chunk, remaining))))
				If nr < 0 Then Exit Do
				remaining -= nr
			Loop

			Return n - remaining

		''' <summary>
		''' Sets the line number to the specified argument.
		''' </summary>
		''' <param name="lineNumber">   the new line number. </param>
		''' <seealso cref= #getLineNumber </seealso>
		public void lineNumberber(Integer lineNumber)
			Me.lineNumber = lineNumber

		''' <summary>
		''' Returns the current line number.
		''' </summary>
		''' <returns>     the current line number. </returns>
		''' <seealso cref= #setLineNumber </seealso>
		public Integer lineNumber
			Return lineNumber


		''' <summary>
		''' Returns the number of bytes that can be read from this input
		''' stream without blocking.
		''' <p>
		''' Note that if the underlying input stream is able to supply
		''' <i>k</i> input characters without blocking, the
		''' {@code LineNumberInputStream} can guarantee only to provide
		''' <i>k</i>/2 characters without blocking, because the
		''' <i>k</i> characters from the underlying input stream might
		''' consist of <i>k</i>/2 pairs of {@code '\u005Cr'} and
		''' {@code '\u005Cn'}, which are converted to just
		''' <i>k</i>/2 {@code '\u005Cn'} characters.
		''' </summary>
		''' <returns>     the number of bytes that can be read from this input stream
		'''             without blocking. </returns>
		''' <exception cref="IOException">  if an I/O error occurs. </exception>
		''' <seealso cref=        java.io.FilterInputStream#in </seealso>
		public Integer available() throws IOException
			Return If(pushBack = -1, MyBase.available()/2, MyBase.available()/2 + 1)

		''' <summary>
		''' Marks the current position in this input stream. A subsequent
		''' call to the {@code reset} method repositions this stream at
		''' the last marked position so that subsequent reads re-read the same bytes.
		''' <p>
		''' The {@code mark} method of
		''' {@code LineNumberInputStream} remembers the current line
		''' number in a private variable, and then calls the {@code mark}
		''' method of the underlying input stream.
		''' </summary>
		''' <param name="readlimit">   the maximum limit of bytes that can be read before
		'''                      the mark position becomes invalid. </param>
		''' <seealso cref=     java.io.FilterInputStream#in </seealso>
		''' <seealso cref=     java.io.LineNumberInputStream#reset() </seealso>
		public void mark(Integer readlimit)
			markLineNumber = lineNumber
			markPushBack = pushBack
			[in].mark(readlimit)

		''' <summary>
		''' Repositions this stream to the position at the time the
		''' {@code mark} method was last called on this input stream.
		''' <p>
		''' The {@code reset} method of
		''' {@code LineNumberInputStream} resets the line number to be
		''' the line number at the time the {@code mark} method was
		''' called, and then calls the {@code reset} method of the
		''' underlying input stream.
		''' <p>
		''' Stream marks are intended to be used in
		''' situations where you need to read ahead a little to see what's in
		''' the stream. Often this is most easily done by invoking some
		''' general parser. If the stream is of the type handled by the
		''' parser, it just chugs along happily. If the stream is not of
		''' that type, the parser should toss an exception when it fails,
		''' which, if it happens within readlimit bytes, allows the outer
		''' code to reset the stream and try another parser.
		''' </summary>
		''' <exception cref="IOException">  if an I/O error occurs. </exception>
		''' <seealso cref=        java.io.FilterInputStream#in </seealso>
		''' <seealso cref=        java.io.LineNumberInputStream#mark(int) </seealso>
		public void reset() throws IOException
			lineNumber = markLineNumber
			pushBack = markPushBack
			[in].reset()
	End Class

End Namespace