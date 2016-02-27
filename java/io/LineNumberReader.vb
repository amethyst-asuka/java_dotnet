Imports Microsoft.VisualBasic
Imports System

'
' * Copyright (c) 1996, 2011, Oracle and/or its affiliates. All rights reserved.
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
	''' A buffered character-input stream that keeps track of line numbers.  This
	''' class defines methods <seealso cref="#setLineNumber(int)"/> and {@link
	''' #getLineNumber()} for setting and getting the current line number
	''' respectively.
	''' 
	''' <p> By default, line numbering begins at 0. This number increments at every
	''' <a href="#lt">line terminator</a> as the data is read, and can be changed
	''' with a call to <tt>setLineNumber(int)</tt>.  Note however, that
	''' <tt>setLineNumber(int)</tt> does not actually change the current position in
	''' the stream; it only changes the value that will be returned by
	''' <tt>getLineNumber()</tt>.
	''' 
	''' <p> A line is considered to be <a name="lt">terminated</a> by any one of a
	''' line feed ('\n'), a carriage return ('\r'), or a carriage return followed
	''' immediately by a linefeed.
	''' 
	''' @author      Mark Reinhold
	''' @since       JDK1.1
	''' </summary>

	Public Class LineNumberReader
		Inherits BufferedReader

		''' <summary>
		''' The current line number </summary>
		Private lineNumber As Integer = 0

		''' <summary>
		''' The line number of the mark, if any </summary>
		Private markedLineNumber As Integer ' Defaults to 0

		''' <summary>
		''' If the next character is a line feed, skip it </summary>
		Private skipLF As Boolean

		''' <summary>
		''' The skipLF flag when the mark was set </summary>
		Private markedSkipLF As Boolean

		''' <summary>
		''' Create a new line-numbering reader, using the default input-buffer
		''' size.
		''' </summary>
		''' <param name="in">
		'''         A Reader object to provide the underlying stream </param>
		Public Sub New(ByVal [in] As Reader)
			MyBase.New([in])
		End Sub

		''' <summary>
		''' Create a new line-numbering reader, reading characters into a buffer of
		''' the given size.
		''' </summary>
		''' <param name="in">
		'''         A Reader object to provide the underlying stream
		''' </param>
		''' <param name="sz">
		'''         An int specifying the size of the buffer </param>
		Public Sub New(ByVal [in] As Reader, ByVal sz As Integer)
			MyBase.New([in], sz)
		End Sub

		''' <summary>
		''' Set the current line number.
		''' </summary>
		''' <param name="lineNumber">
		'''         An int specifying the line number
		''' </param>
		''' <seealso cref= #getLineNumber </seealso>
		Public Overridable Property lineNumber As Integer
			Set(ByVal lineNumber As Integer)
				Me.lineNumber = lineNumber
			End Set
			Get
				Return lineNumber
			End Get
		End Property


		''' <summary>
		''' Read a single character.  <a href="#lt">Line terminators</a> are
		''' compressed into single newline ('\n') characters.  Whenever a line
		''' terminator is read the current line number is incremented.
		''' </summary>
		''' <returns>  The character read, or -1 if the end of the stream has been
		'''          reached
		''' </returns>
		''' <exception cref="IOException">
		'''          If an I/O error occurs </exception>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
		Public Overrides Function read() As Integer
			SyncLock lock
				Dim c As Integer = MyBase.read()
				If skipLF Then
					If c = ControlChars.Lf Then c = MyBase.read()
					skipLF = False
				End If
				Select Case c
				Case ControlChars.Cr
					skipLF = True
'JAVA TO VB CONVERTER TODO TASK: VB does not allow fall-through from a non-empty 'case':
				Case ControlChars.Lf ' Fall through
					lineNumber += 1
					Return ControlChars.Lf
				End Select
				Return c
			End SyncLock
		End Function

		''' <summary>
		''' Read characters into a portion of an array.  Whenever a <a
		''' href="#lt">line terminator</a> is read the current line number is
		''' incremented.
		''' </summary>
		''' <param name="cbuf">
		'''         Destination buffer
		''' </param>
		''' <param name="off">
		'''         Offset at which to start storing characters
		''' </param>
		''' <param name="len">
		'''         Maximum number of characters to read
		''' </param>
		''' <returns>  The number of bytes read, or -1 if the end of the stream has
		'''          already been reached
		''' </returns>
		''' <exception cref="IOException">
		'''          If an I/O error occurs </exception>
'JAVA TO VB CONVERTER TODO TASK: The following line could not be converted:
		public int read(char cbuf() , int off, int len) throws IOException
			SyncLock lock
				Dim n As Integer = MyBase.read(cbuf, off, len)

				For i As Integer = off To off + n - 1
					Dim c As Integer = cbuf(i)
					If skipLF Then
						skipLF = False
						If c = ControlChars.Lf Then Continue For
					End If
					Select Case c
					Case ControlChars.Cr
						skipLF = True
'JAVA TO VB CONVERTER TODO TASK: VB does not allow fall-through from a non-empty 'case':
					Case ControlChars.Lf ' Fall through
						lineNumber += 1
					End Select
				Next i

				Return n
			End SyncLock

		''' <summary>
		''' Read a line of text.  Whenever a <a href="#lt">line terminator</a> is
		''' read the current line number is incremented.
		''' </summary>
		''' <returns>  A String containing the contents of the line, not including
		'''          any <a href="#lt">line termination characters</a>, or
		'''          <tt>null</tt> if the end of the stream has been reached
		''' </returns>
		''' <exception cref="IOException">
		'''          If an I/O error occurs </exception>
		public String readLine() throws IOException
			SyncLock lock
				Dim l As String = MyBase.readLine(skipLF)
				skipLF = False
				If l IsNot Nothing Then lineNumber += 1
				Return l
			End SyncLock

		''' <summary>
		''' Maximum skip-buffer size </summary>
		private static final Integer maxSkipBufferSize = 8192

		''' <summary>
		''' Skip buffer, null until allocated </summary>
		private Char skipBuffer() = Nothing

		''' <summary>
		''' Skip characters.
		''' </summary>
		''' <param name="n">
		'''         The number of characters to skip
		''' </param>
		''' <returns>  The number of characters actually skipped
		''' </returns>
		''' <exception cref="IOException">
		'''          If an I/O error occurs
		''' </exception>
		''' <exception cref="IllegalArgumentException">
		'''          If <tt>n</tt> is negative </exception>
		public Long skip(Long n) throws IOException
			If n < 0 Then Throw New IllegalArgumentException("skip() value is negative")
			Dim nn As Integer = CInt(Fix (System.Math.Min(n, maxSkipBufferSize)))
			SyncLock lock
				If (skipBuffer Is Nothing) OrElse (skipBuffer.Length < nn) Then skipBuffer = New Char(nn - 1){}
				Dim r As Long = n
				Do While r > 0
					Dim nc As Integer = read(skipBuffer, 0, CInt(Fix (System.Math.Min(r, nn))))
					If nc = -1 Then Exit Do
					r -= nc
				Loop
				Return n - r
			End SyncLock

		''' <summary>
		''' Mark the present position in the stream.  Subsequent calls to reset()
		''' will attempt to reposition the stream to this point, and will also reset
		''' the line number appropriately.
		''' </summary>
		''' <param name="readAheadLimit">
		'''         Limit on the number of characters that may be read while still
		'''         preserving the mark.  After reading this many characters,
		'''         attempting to reset the stream may fail.
		''' </param>
		''' <exception cref="IOException">
		'''          If an I/O error occurs </exception>
		public void mark(Integer readAheadLimit) throws IOException
			SyncLock lock
				MyBase.mark(readAheadLimit)
				markedLineNumber = lineNumber
				markedSkipLF = skipLF
			End SyncLock

		''' <summary>
		''' Reset the stream to the most recent mark.
		''' </summary>
		''' <exception cref="IOException">
		'''          If the stream has not been marked, or if the mark has been
		'''          invalidated </exception>
		public void reset() throws IOException
			SyncLock lock
				MyBase.reset()
				lineNumber = markedLineNumber
				skipLF = markedSkipLF
			End SyncLock

	End Class

End Namespace