Imports Microsoft.VisualBasic
Imports System
Imports System.Threading

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
	''' A <code>PrintStream</code> adds functionality to another output stream,
	''' namely the ability to print representations of various data values
	''' conveniently.  Two other features are provided as well.  Unlike other output
	''' streams, a <code>PrintStream</code> never throws an
	''' <code>IOException</code>; instead, exceptional situations merely set an
	''' internal flag that can be tested via the <code>checkError</code> method.
	''' Optionally, a <code>PrintStream</code> can be created so as to flush
	''' automatically; this means that the <code>flush</code> method is
	''' automatically invoked after a byte array is written, one of the
	''' <code>println</code> methods is invoked, or a newline character or byte
	''' (<code>'\n'</code>) is written.
	''' 
	''' <p> All characters printed by a <code>PrintStream</code> are converted into
	''' bytes using the platform's default character encoding.  The <code>{@link
	''' PrintWriter}</code> class should be used in situations that require writing
	''' characters rather than bytes.
	''' 
	''' @author     Frank Yellin
	''' @author     Mark Reinhold
	''' @since      JDK1.0
	''' </summary>

	Public Class PrintStream
		Inherits FilterOutputStream
		Implements Appendable, Closeable

		Private ReadOnly autoFlush As Boolean
		Private trouble As Boolean = False
		Private formatter As java.util.Formatter

		''' <summary>
		''' Track both the text- and character-output streams, so that their buffers
		''' can be flushed without flushing the entire stream.
		''' </summary>
		Private textOut As BufferedWriter
		Private charOut As OutputStreamWriter

		''' <summary>
		''' requireNonNull is explicitly declared here so as not to create an extra
		''' dependency on java.util.Objects.requireNonNull. PrintStream is loaded
		''' early during system initialization.
		''' </summary>
		Private Shared Function requireNonNull(Of T)(ByVal obj As T, ByVal message As String) As T
			If obj Is Nothing Then Throw New NullPointerException(message)
			Return obj
		End Function

		''' <summary>
		''' Returns a charset object for the given charset name. </summary>
		''' <exception cref="NullPointerException">          is csn is null </exception>
		''' <exception cref="UnsupportedEncodingException">  if the charset is not supported </exception>
		Private Shared Function toCharset(ByVal csn As String) As java.nio.charset.Charset
			requireNonNull(csn, "charsetName")
			Try
				Return java.nio.charset.Charset.forName(csn)
'JAVA TO VB CONVERTER TODO TASK: There is no equivalent in VB to Java 'multi-catch' syntax:
			Catch java.nio.charset.IllegalCharsetNameException Or java.nio.charset.UnsupportedCharsetException unused
				' UnsupportedEncodingException should be thrown
				Throw New UnsupportedEncodingException(csn)
			End Try
		End Function

		' Private constructors 
		Private Sub New(ByVal autoFlush As Boolean, ByVal out As OutputStream)
			MyBase.New(out)
			Me.autoFlush = autoFlush
			Me.charOut = New OutputStreamWriter(Me)
			Me.textOut = New BufferedWriter(charOut)
		End Sub

		Private Sub New(ByVal autoFlush As Boolean, ByVal out As OutputStream, ByVal charset As java.nio.charset.Charset)
			MyBase.New(out)
			Me.autoFlush = autoFlush
			Me.charOut = New OutputStreamWriter(Me, charset)
			Me.textOut = New BufferedWriter(charOut)
		End Sub

	'     Variant of the private constructor so that the given charset name
	'     * can be verified before evaluating the OutputStream argument. Used
	'     * by constructors creating a FileOutputStream that also take a
	'     * charset name.
	'     
		Private Sub New(ByVal autoFlush As Boolean, ByVal charset As java.nio.charset.Charset, ByVal out As OutputStream)
			Me.New(autoFlush, out, charset)
		End Sub

		''' <summary>
		''' Creates a new print stream.  This stream will not flush automatically.
		''' </summary>
		''' <param name="out">        The output stream to which values and objects will be
		'''                    printed
		''' </param>
		''' <seealso cref= java.io.PrintWriter#PrintWriter(java.io.OutputStream) </seealso>
		Public Sub New(ByVal out As OutputStream)
			Me.New(out, False)
		End Sub

		''' <summary>
		''' Creates a new print stream.
		''' </summary>
		''' <param name="out">        The output stream to which values and objects will be
		'''                    printed </param>
		''' <param name="autoFlush">  A boolean; if true, the output buffer will be flushed
		'''                    whenever a byte array is written, one of the
		'''                    <code>println</code> methods is invoked, or a newline
		'''                    character or byte (<code>'\n'</code>) is written
		''' </param>
		''' <seealso cref= java.io.PrintWriter#PrintWriter(java.io.OutputStream, boolean) </seealso>
		Public Sub New(ByVal out As OutputStream, ByVal autoFlush As Boolean)
			Me.New(autoFlush, requireNonNull(out, "Null output stream"))
		End Sub

		''' <summary>
		''' Creates a new print stream.
		''' </summary>
		''' <param name="out">        The output stream to which values and objects will be
		'''                    printed </param>
		''' <param name="autoFlush">  A boolean; if true, the output buffer will be flushed
		'''                    whenever a byte array is written, one of the
		'''                    <code>println</code> methods is invoked, or a newline
		'''                    character or byte (<code>'\n'</code>) is written </param>
		''' <param name="encoding">   The name of a supported
		'''                    <a href="../lang/package-summary.html#charenc">
		'''                    character encoding</a>
		''' </param>
		''' <exception cref="UnsupportedEncodingException">
		'''          If the named encoding is not supported
		''' 
		''' @since  1.4 </exception>
		Public Sub New(ByVal out As OutputStream, ByVal autoFlush As Boolean, ByVal encoding As String)
			Me.New(autoFlush, requireNonNull(out, "Null output stream"), toCharset(encoding))
		End Sub

		''' <summary>
		''' Creates a new print stream, without automatic line flushing, with the
		''' specified file name.  This convenience constructor creates
		''' the necessary intermediate {@link java.io.OutputStreamWriter
		''' OutputStreamWriter}, which will encode characters using the
		''' <seealso cref="java.nio.charset.Charset#defaultCharset() default charset"/>
		''' for this instance of the Java virtual machine.
		''' </summary>
		''' <param name="fileName">
		'''         The name of the file to use as the destination of this print
		'''         stream.  If the file exists, then it will be truncated to
		'''         zero size; otherwise, a new file will be created.  The output
		'''         will be written to the file and is buffered.
		''' </param>
		''' <exception cref="FileNotFoundException">
		'''          If the given file object does not denote an existing, writable
		'''          regular file and a new regular file of that name cannot be
		'''          created, or if some other error occurs while opening or
		'''          creating the file
		''' </exception>
		''' <exception cref="SecurityException">
		'''          If a security manager is present and {@link
		'''          SecurityManager#checkWrite checkWrite(fileName)} denies write
		'''          access to the file
		''' 
		''' @since  1.5 </exception>
		Public Sub New(ByVal fileName As String)
			Me.New(False, New FileOutputStream(fileName))
		End Sub

		''' <summary>
		''' Creates a new print stream, without automatic line flushing, with the
		''' specified file name and charset.  This convenience constructor creates
		''' the necessary intermediate {@link java.io.OutputStreamWriter
		''' OutputStreamWriter}, which will encode characters using the provided
		''' charset.
		''' </summary>
		''' <param name="fileName">
		'''         The name of the file to use as the destination of this print
		'''         stream.  If the file exists, then it will be truncated to
		'''         zero size; otherwise, a new file will be created.  The output
		'''         will be written to the file and is buffered.
		''' </param>
		''' <param name="csn">
		'''         The name of a supported {@link java.nio.charset.Charset
		'''         charset}
		''' </param>
		''' <exception cref="FileNotFoundException">
		'''          If the given file object does not denote an existing, writable
		'''          regular file and a new regular file of that name cannot be
		'''          created, or if some other error occurs while opening or
		'''          creating the file
		''' </exception>
		''' <exception cref="SecurityException">
		'''          If a security manager is present and {@link
		'''          SecurityManager#checkWrite checkWrite(fileName)} denies write
		'''          access to the file
		''' </exception>
		''' <exception cref="UnsupportedEncodingException">
		'''          If the named charset is not supported
		''' 
		''' @since  1.5 </exception>
		Public Sub New(ByVal fileName As String, ByVal csn As String)
			' ensure charset is checked before the file is opened
			Me.New(False, toCharset(csn), New FileOutputStream(fileName))
		End Sub

		''' <summary>
		''' Creates a new print stream, without automatic line flushing, with the
		''' specified file.  This convenience constructor creates the necessary
		''' intermediate <seealso cref="java.io.OutputStreamWriter OutputStreamWriter"/>,
		''' which will encode characters using the {@linkplain
		''' java.nio.charset.Charset#defaultCharset() default charset} for this
		''' instance of the Java virtual machine.
		''' </summary>
		''' <param name="file">
		'''         The file to use as the destination of this print stream.  If the
		'''         file exists, then it will be truncated to zero size; otherwise,
		'''         a new file will be created.  The output will be written to the
		'''         file and is buffered.
		''' </param>
		''' <exception cref="FileNotFoundException">
		'''          If the given file object does not denote an existing, writable
		'''          regular file and a new regular file of that name cannot be
		'''          created, or if some other error occurs while opening or
		'''          creating the file
		''' </exception>
		''' <exception cref="SecurityException">
		'''          If a security manager is present and {@link
		'''          SecurityManager#checkWrite checkWrite(file.getPath())}
		'''          denies write access to the file
		''' 
		''' @since  1.5 </exception>
		Public Sub New(ByVal file_Renamed As File)
			Me.New(False, New FileOutputStream(file_Renamed))
		End Sub

		''' <summary>
		''' Creates a new print stream, without automatic line flushing, with the
		''' specified file and charset.  This convenience constructor creates
		''' the necessary intermediate {@link java.io.OutputStreamWriter
		''' OutputStreamWriter}, which will encode characters using the provided
		''' charset.
		''' </summary>
		''' <param name="file">
		'''         The file to use as the destination of this print stream.  If the
		'''         file exists, then it will be truncated to zero size; otherwise,
		'''         a new file will be created.  The output will be written to the
		'''         file and is buffered.
		''' </param>
		''' <param name="csn">
		'''         The name of a supported {@link java.nio.charset.Charset
		'''         charset}
		''' </param>
		''' <exception cref="FileNotFoundException">
		'''          If the given file object does not denote an existing, writable
		'''          regular file and a new regular file of that name cannot be
		'''          created, or if some other error occurs while opening or
		'''          creating the file
		''' </exception>
		''' <exception cref="SecurityException">
		'''          If a security manager is present and {@link
		'''          SecurityManager#checkWrite checkWrite(file.getPath())}
		'''          denies write access to the file
		''' </exception>
		''' <exception cref="UnsupportedEncodingException">
		'''          If the named charset is not supported
		''' 
		''' @since  1.5 </exception>
		Public Sub New(ByVal file_Renamed As File, ByVal csn As String)
			' ensure charset is checked before the file is opened
			Me.New(False, toCharset(csn), New FileOutputStream(file_Renamed))
		End Sub

		''' <summary>
		''' Check to make sure that the stream has not been closed </summary>
		Private Sub ensureOpen()
			If out Is Nothing Then Throw New IOException("Stream closed")
		End Sub

		''' <summary>
		''' Flushes the stream.  This is done by writing any buffered output bytes to
		''' the underlying output stream and then flushing that stream.
		''' </summary>
		''' <seealso cref=        java.io.OutputStream#flush() </seealso>
		Public Overrides Sub flush()
			SyncLock Me
				Try
					ensureOpen()
					out.flush()
				Catch x As IOException
					trouble = True
				End Try
			End SyncLock
		End Sub

		Private closing As Boolean = False ' To avoid recursive closing

		''' <summary>
		''' Closes the stream.  This is done by flushing the stream and then closing
		''' the underlying output stream.
		''' </summary>
		''' <seealso cref=        java.io.OutputStream#close() </seealso>
		Public Overrides Sub close() Implements Closeable.close
			SyncLock Me
				If Not closing Then
					closing = True
					Try
						textOut.close()
						out.close()
					Catch x As IOException
						trouble = True
					End Try
					textOut = Nothing
					charOut = Nothing
					out = Nothing
				End If
			End SyncLock
		End Sub

		''' <summary>
		''' Flushes the stream and checks its error state. The internal error state
		''' is set to <code>true</code> when the underlying output stream throws an
		''' <code>IOException</code> other than <code>InterruptedIOException</code>,
		''' and when the <code>setError</code> method is invoked.  If an operation
		''' on the underlying output stream throws an
		''' <code>InterruptedIOException</code>, then the <code>PrintStream</code>
		''' converts the exception back into an interrupt by doing:
		''' <pre>
		'''     Thread.currentThread().interrupt();
		''' </pre>
		''' or the equivalent.
		''' </summary>
		''' <returns> <code>true</code> if and only if this stream has encountered an
		'''         <code>IOException</code> other than
		'''         <code>InterruptedIOException</code>, or the
		'''         <code>setError</code> method has been invoked </returns>
		Public Overridable Function checkError() As Boolean
			If out IsNot Nothing Then flush()
			If TypeOf out Is java.io.PrintStream Then
				Dim ps As PrintStream = CType(out, PrintStream)
				Return ps.checkError()
			End If
			Return trouble
		End Function

		''' <summary>
		''' Sets the error state of the stream to <code>true</code>.
		''' 
		''' <p> This method will cause subsequent invocations of {@link
		''' #checkError()} to return <tt>true</tt> until {@link
		''' #clearError()} is invoked.
		''' 
		''' @since JDK1.1
		''' </summary>
		Protected Friend Overridable Sub setError()
			trouble = True
		End Sub

		''' <summary>
		''' Clears the internal error state of this stream.
		''' 
		''' <p> This method will cause subsequent invocations of {@link
		''' #checkError()} to return <tt>false</tt> until another write
		''' operation fails and invokes <seealso cref="#setError()"/>.
		''' 
		''' @since 1.6
		''' </summary>
		Protected Friend Overridable Sub clearError()
			trouble = False
		End Sub

	'    
	'     * Exception-catching, synchronized output operations,
	'     * which also implement the write() methods of OutputStream
	'     

		''' <summary>
		''' Writes the specified byte to this stream.  If the byte is a newline and
		''' automatic flushing is enabled then the <code>flush</code> method will be
		''' invoked.
		''' 
		''' <p> Note that the byte is written as given; to write a character that
		''' will be translated according to the platform's default character
		''' encoding, use the <code>print(char)</code> or <code>println(char)</code>
		''' methods.
		''' </summary>
		''' <param name="b">  The byte to be written </param>
		''' <seealso cref= #print(char) </seealso>
		''' <seealso cref= #println(char) </seealso>
		Public Overrides Sub write(ByVal b As Integer)
			Try
				SyncLock Me
					ensureOpen()
					out.write(b)
					If (b = ControlChars.Lf) AndAlso autoFlush Then out.flush()
				End SyncLock
			Catch x As InterruptedIOException
				Thread.CurrentThread.Interrupt()
			Catch x As IOException
				trouble = True
			End Try
		End Sub

		''' <summary>
		''' Writes <code>len</code> bytes from the specified byte array starting at
		''' offset <code>off</code> to this stream.  If automatic flushing is
		''' enabled then the <code>flush</code> method will be invoked.
		''' 
		''' <p> Note that the bytes will be written as given; to write characters
		''' that will be translated according to the platform's default character
		''' encoding, use the <code>print(char)</code> or <code>println(char)</code>
		''' methods.
		''' </summary>
		''' <param name="buf">   A byte array </param>
		''' <param name="off">   Offset from which to start taking bytes </param>
		''' <param name="len">   Number of bytes to write </param>
'JAVA TO VB CONVERTER TODO TASK: The following line could not be converted:
		public void write(byte buf() , int off, int len)
			Try
				SyncLock Me
					ensureOpen()
					out.write(buf, off, len)
					If autoFlush Then out.flush()
				End SyncLock
			Catch x As InterruptedIOException
				Thread.CurrentThread.Interrupt()
			Catch x As IOException
				trouble = True
			End Try

	'    
	'     * The following private methods on the text- and character-output streams
	'     * always flush the stream buffers, so that writes to the underlying byte
	'     * stream occur as promptly as with the original PrintStream.
	'     

		private void write(Char buf())
			Try
				SyncLock Me
					ensureOpen()
					textOut.write(buf)
					textOut.flushBuffer()
					charOut.flushBuffer()
					If autoFlush Then
						For i As Integer = 0 To buf.length - 1
							If buf(i) = ControlChars.Lf Then out.flush()
						Next i
					End If
				End SyncLock
			Catch x As InterruptedIOException
				Thread.CurrentThread.Interrupt()
			Catch x As IOException
				trouble = True
			End Try

		private void write(String s)
			Try
				SyncLock Me
					ensureOpen()
					textOut.write(s)
					textOut.flushBuffer()
					charOut.flushBuffer()
					If autoFlush AndAlso (s.IndexOf(ControlChars.Lf) >= 0) Then out.flush()
				End SyncLock
			Catch x As InterruptedIOException
				Thread.CurrentThread.Interrupt()
			Catch x As IOException
				trouble = True
			End Try

		private void newLine()
			Try
				SyncLock Me
					ensureOpen()
					textOut.newLine()
					textOut.flushBuffer()
					charOut.flushBuffer()
					If autoFlush Then out.flush()
				End SyncLock
			Catch x As InterruptedIOException
				Thread.CurrentThread.Interrupt()
			Catch x As IOException
				trouble = True
			End Try

		' Methods that do not terminate lines 

		''' <summary>
		''' Prints a boolean value.  The string produced by <code>{@link
		''' java.lang.String#valueOf(boolean)}</code> is translated into bytes
		''' according to the platform's default character encoding, and these bytes
		''' are written in exactly the manner of the
		''' <code><seealso cref="#write(int)"/></code> method.
		''' </summary>
		''' <param name="b">   The <code>boolean</code> to be printed </param>
		public void print(Boolean b)
			write(If(b, "true", "false"))

		''' <summary>
		''' Prints a character.  The character is translated into one or more bytes
		''' according to the platform's default character encoding, and these bytes
		''' are written in exactly the manner of the
		''' <code><seealso cref="#write(int)"/></code> method.
		''' </summary>
		''' <param name="c">   The <code>char</code> to be printed </param>
		public void print(Char c)
			write(Convert.ToString(c))

		''' <summary>
		''' Prints an integer.  The string produced by <code>{@link
		''' java.lang.String#valueOf(int)}</code> is translated into bytes
		''' according to the platform's default character encoding, and these bytes
		''' are written in exactly the manner of the
		''' <code><seealso cref="#write(int)"/></code> method.
		''' </summary>
		''' <param name="i">   The <code>int</code> to be printed </param>
		''' <seealso cref=        java.lang.Integer#toString(int) </seealso>
		public void print(Integer i)
			write(Convert.ToString(i))

		''' <summary>
		''' Prints a long integer.  The string produced by <code>{@link
		''' java.lang.String#valueOf(long)}</code> is translated into bytes
		''' according to the platform's default character encoding, and these bytes
		''' are written in exactly the manner of the
		''' <code><seealso cref="#write(int)"/></code> method.
		''' </summary>
		''' <param name="l">   The <code>long</code> to be printed </param>
		''' <seealso cref=        java.lang.Long#toString(long) </seealso>
		public void print(Long l)
			write(Convert.ToString(l))

		''' <summary>
		''' Prints a floating-point number.  The string produced by <code>{@link
		''' java.lang.String#valueOf(float)}</code> is translated into bytes
		''' according to the platform's default character encoding, and these bytes
		''' are written in exactly the manner of the
		''' <code><seealso cref="#write(int)"/></code> method.
		''' </summary>
		''' <param name="f">   The <code>float</code> to be printed </param>
		''' <seealso cref=        java.lang.Float#toString(float) </seealso>
		public void print(Single f)
			write(Convert.ToString(f))

		''' <summary>
		''' Prints a double-precision floating-point number.  The string produced by
		''' <code><seealso cref="java.lang.String#valueOf(double)"/></code> is translated into
		''' bytes according to the platform's default character encoding, and these
		''' bytes are written in exactly the manner of the <code>{@link
		''' #write(int)}</code> method.
		''' </summary>
		''' <param name="d">   The <code>double</code> to be printed </param>
		''' <seealso cref=        java.lang.Double#toString(double) </seealso>
		public void print(Double d)
			write(Convert.ToString(d))

		''' <summary>
		''' Prints an array of characters.  The characters are converted into bytes
		''' according to the platform's default character encoding, and these bytes
		''' are written in exactly the manner of the
		''' <code><seealso cref="#write(int)"/></code> method.
		''' </summary>
		''' <param name="s">   The array of chars to be printed
		''' </param>
		''' <exception cref="NullPointerException">  If <code>s</code> is <code>null</code> </exception>
		public void print(Char s())
			write(s)

		''' <summary>
		''' Prints a string.  If the argument is <code>null</code> then the string
		''' <code>"null"</code> is printed.  Otherwise, the string's characters are
		''' converted into bytes according to the platform's default character
		''' encoding, and these bytes are written in exactly the manner of the
		''' <code><seealso cref="#write(int)"/></code> method.
		''' </summary>
		''' <param name="s">   The <code>String</code> to be printed </param>
		public void print(String s)
			If s Is Nothing Then s = "null"
			write(s)

		''' <summary>
		''' Prints an object.  The string produced by the <code>{@link
		''' java.lang.String#valueOf(Object)}</code> method is translated into bytes
		''' according to the platform's default character encoding, and these bytes
		''' are written in exactly the manner of the
		''' <code><seealso cref="#write(int)"/></code> method.
		''' </summary>
		''' <param name="obj">   The <code>Object</code> to be printed </param>
		''' <seealso cref=        java.lang.Object#toString() </seealso>
		public void print(Object obj)
			write(Convert.ToString(obj))


		' Methods that do terminate lines 

		''' <summary>
		''' Terminates the current line by writing the line separator string.  The
		''' line separator string is defined by the system property
		''' <code>line.separator</code>, and is not necessarily a single newline
		''' character (<code>'\n'</code>).
		''' </summary>
		public void println()
			newLine()

		''' <summary>
		''' Prints a boolean and then terminate the line.  This method behaves as
		''' though it invokes <code><seealso cref="#print(boolean)"/></code> and then
		''' <code><seealso cref="#println()"/></code>.
		''' </summary>
		''' <param name="x">  The <code>boolean</code> to be printed </param>
		public void println(Boolean x)
			SyncLock Me
				print(x)
				newLine()
			End SyncLock

		''' <summary>
		''' Prints a character and then terminate the line.  This method behaves as
		''' though it invokes <code><seealso cref="#print(char)"/></code> and then
		''' <code><seealso cref="#println()"/></code>.
		''' </summary>
		''' <param name="x">  The <code>char</code> to be printed. </param>
		public void println(Char x)
			SyncLock Me
				print(x)
				newLine()
			End SyncLock

		''' <summary>
		''' Prints an integer and then terminate the line.  This method behaves as
		''' though it invokes <code><seealso cref="#print(int)"/></code> and then
		''' <code><seealso cref="#println()"/></code>.
		''' </summary>
		''' <param name="x">  The <code>int</code> to be printed. </param>
		public void println(Integer x)
			SyncLock Me
				print(x)
				newLine()
			End SyncLock

		''' <summary>
		''' Prints a long and then terminate the line.  This method behaves as
		''' though it invokes <code><seealso cref="#print(long)"/></code> and then
		''' <code><seealso cref="#println()"/></code>.
		''' </summary>
		''' <param name="x">  a The <code>long</code> to be printed. </param>
		public void println(Long x)
			SyncLock Me
				print(x)
				newLine()
			End SyncLock

		''' <summary>
		''' Prints a float and then terminate the line.  This method behaves as
		''' though it invokes <code><seealso cref="#print(float)"/></code> and then
		''' <code><seealso cref="#println()"/></code>.
		''' </summary>
		''' <param name="x">  The <code>float</code> to be printed. </param>
		public void println(Single x)
			SyncLock Me
				print(x)
				newLine()
			End SyncLock

		''' <summary>
		''' Prints a double and then terminate the line.  This method behaves as
		''' though it invokes <code><seealso cref="#print(double)"/></code> and then
		''' <code><seealso cref="#println()"/></code>.
		''' </summary>
		''' <param name="x">  The <code>double</code> to be printed. </param>
		public void println(Double x)
			SyncLock Me
				print(x)
				newLine()
			End SyncLock

		''' <summary>
		''' Prints an array of characters and then terminate the line.  This method
		''' behaves as though it invokes <code><seealso cref="#print(char[])"/></code> and
		''' then <code><seealso cref="#println()"/></code>.
		''' </summary>
		''' <param name="x">  an array of chars to print. </param>
		public void println(Char x())
			SyncLock Me
				print(x)
				newLine()
			End SyncLock

		''' <summary>
		''' Prints a String and then terminate the line.  This method behaves as
		''' though it invokes <code><seealso cref="#print(String)"/></code> and then
		''' <code><seealso cref="#println()"/></code>.
		''' </summary>
		''' <param name="x">  The <code>String</code> to be printed. </param>
		public void println(String x)
			SyncLock Me
				print(x)
				newLine()
			End SyncLock

		''' <summary>
		''' Prints an Object and then terminate the line.  This method calls
		''' at first String.valueOf(x) to get the printed object's string value,
		''' then behaves as
		''' though it invokes <code><seealso cref="#print(String)"/></code> and then
		''' <code><seealso cref="#println()"/></code>.
		''' </summary>
		''' <param name="x">  The <code>Object</code> to be printed. </param>
		public void println(Object x)
			Dim s As String = Convert.ToString(x)
			SyncLock Me
				print(s)
				newLine()
			End SyncLock


		''' <summary>
		''' A convenience method to write a formatted string to this output stream
		''' using the specified format string and arguments.
		''' 
		''' <p> An invocation of this method of the form <tt>out.printf(format,
		''' args)</tt> behaves in exactly the same way as the invocation
		''' 
		''' <pre>
		'''     out.format(format, args) </pre>
		''' </summary>
		''' <param name="format">
		'''         A format string as described in <a
		'''         href="../util/Formatter.html#syntax">Format string syntax</a>
		''' </param>
		''' <param name="args">
		'''         Arguments referenced by the format specifiers in the format
		'''         string.  If there are more arguments than format specifiers, the
		'''         extra arguments are ignored.  The number of arguments is
		'''         variable and may be zero.  The maximum number of arguments is
		'''         limited by the maximum dimension of a Java array as defined by
		'''         <cite>The Java&trade; Virtual Machine Specification</cite>.
		'''         The behaviour on a
		'''         <tt>null</tt> argument depends on the <a
		'''         href="../util/Formatter.html#syntax">conversion</a>.
		''' </param>
		''' <exception cref="java.util.IllegalFormatException">
		'''          If a format string contains an illegal syntax, a format
		'''          specifier that is incompatible with the given arguments,
		'''          insufficient arguments given the format string, or other
		'''          illegal conditions.  For specification of all possible
		'''          formatting errors, see the <a
		'''          href="../util/Formatter.html#detail">Details</a> section of the
		'''          formatter class specification.
		''' </exception>
		''' <exception cref="NullPointerException">
		'''          If the <tt>format</tt> is <tt>null</tt>
		''' </exception>
		''' <returns>  This output stream
		''' 
		''' @since  1.5 </returns>
		public PrintStream printf(String format, Object... args)
			Return format(format, args)

		''' <summary>
		''' A convenience method to write a formatted string to this output stream
		''' using the specified format string and arguments.
		''' 
		''' <p> An invocation of this method of the form <tt>out.printf(l, format,
		''' args)</tt> behaves in exactly the same way as the invocation
		''' 
		''' <pre>
		'''     out.format(l, format, args) </pre>
		''' </summary>
		''' <param name="l">
		'''         The <seealso cref="java.util.Locale locale"/> to apply during
		'''         formatting.  If <tt>l</tt> is <tt>null</tt> then no localization
		'''         is applied.
		''' </param>
		''' <param name="format">
		'''         A format string as described in <a
		'''         href="../util/Formatter.html#syntax">Format string syntax</a>
		''' </param>
		''' <param name="args">
		'''         Arguments referenced by the format specifiers in the format
		'''         string.  If there are more arguments than format specifiers, the
		'''         extra arguments are ignored.  The number of arguments is
		'''         variable and may be zero.  The maximum number of arguments is
		'''         limited by the maximum dimension of a Java array as defined by
		'''         <cite>The Java&trade; Virtual Machine Specification</cite>.
		'''         The behaviour on a
		'''         <tt>null</tt> argument depends on the <a
		'''         href="../util/Formatter.html#syntax">conversion</a>.
		''' </param>
		''' <exception cref="java.util.IllegalFormatException">
		'''          If a format string contains an illegal syntax, a format
		'''          specifier that is incompatible with the given arguments,
		'''          insufficient arguments given the format string, or other
		'''          illegal conditions.  For specification of all possible
		'''          formatting errors, see the <a
		'''          href="../util/Formatter.html#detail">Details</a> section of the
		'''          formatter class specification.
		''' </exception>
		''' <exception cref="NullPointerException">
		'''          If the <tt>format</tt> is <tt>null</tt>
		''' </exception>
		''' <returns>  This output stream
		''' 
		''' @since  1.5 </returns>
		public PrintStream printf(java.util.Locale l, String format, Object... args)
			Return format(l, format, args)

		''' <summary>
		''' Writes a formatted string to this output stream using the specified
		''' format string and arguments.
		''' 
		''' <p> The locale always used is the one returned by {@link
		''' java.util.Locale#getDefault() Locale.getDefault()}, regardless of any
		''' previous invocations of other formatting methods on this object.
		''' </summary>
		''' <param name="format">
		'''         A format string as described in <a
		'''         href="../util/Formatter.html#syntax">Format string syntax</a>
		''' </param>
		''' <param name="args">
		'''         Arguments referenced by the format specifiers in the format
		'''         string.  If there are more arguments than format specifiers, the
		'''         extra arguments are ignored.  The number of arguments is
		'''         variable and may be zero.  The maximum number of arguments is
		'''         limited by the maximum dimension of a Java array as defined by
		'''         <cite>The Java&trade; Virtual Machine Specification</cite>.
		'''         The behaviour on a
		'''         <tt>null</tt> argument depends on the <a
		'''         href="../util/Formatter.html#syntax">conversion</a>.
		''' </param>
		''' <exception cref="java.util.IllegalFormatException">
		'''          If a format string contains an illegal syntax, a format
		'''          specifier that is incompatible with the given arguments,
		'''          insufficient arguments given the format string, or other
		'''          illegal conditions.  For specification of all possible
		'''          formatting errors, see the <a
		'''          href="../util/Formatter.html#detail">Details</a> section of the
		'''          formatter class specification.
		''' </exception>
		''' <exception cref="NullPointerException">
		'''          If the <tt>format</tt> is <tt>null</tt>
		''' </exception>
		''' <returns>  This output stream
		''' 
		''' @since  1.5 </returns>
		public PrintStream format(String format, Object... args)
			Try
				SyncLock Me
					ensureOpen()
					If (formatter Is Nothing) OrElse (formatter.locale() IsNot java.util.Locale.default) Then formatter = New java.util.Formatter(CType(Me, Appendable))
					formatter.format(java.util.Locale.default, format, args)
				End SyncLock
			Catch x As InterruptedIOException
				Thread.CurrentThread.Interrupt()
			Catch x As IOException
				trouble = True
			End Try
			Return Me

		''' <summary>
		''' Writes a formatted string to this output stream using the specified
		''' format string and arguments.
		''' </summary>
		''' <param name="l">
		'''         The <seealso cref="java.util.Locale locale"/> to apply during
		'''         formatting.  If <tt>l</tt> is <tt>null</tt> then no localization
		'''         is applied.
		''' </param>
		''' <param name="format">
		'''         A format string as described in <a
		'''         href="../util/Formatter.html#syntax">Format string syntax</a>
		''' </param>
		''' <param name="args">
		'''         Arguments referenced by the format specifiers in the format
		'''         string.  If there are more arguments than format specifiers, the
		'''         extra arguments are ignored.  The number of arguments is
		'''         variable and may be zero.  The maximum number of arguments is
		'''         limited by the maximum dimension of a Java array as defined by
		'''         <cite>The Java&trade; Virtual Machine Specification</cite>.
		'''         The behaviour on a
		'''         <tt>null</tt> argument depends on the <a
		'''         href="../util/Formatter.html#syntax">conversion</a>.
		''' </param>
		''' <exception cref="java.util.IllegalFormatException">
		'''          If a format string contains an illegal syntax, a format
		'''          specifier that is incompatible with the given arguments,
		'''          insufficient arguments given the format string, or other
		'''          illegal conditions.  For specification of all possible
		'''          formatting errors, see the <a
		'''          href="../util/Formatter.html#detail">Details</a> section of the
		'''          formatter class specification.
		''' </exception>
		''' <exception cref="NullPointerException">
		'''          If the <tt>format</tt> is <tt>null</tt>
		''' </exception>
		''' <returns>  This output stream
		''' 
		''' @since  1.5 </returns>
		public PrintStream format(java.util.Locale l, String format, Object... args)
			Try
				SyncLock Me
					ensureOpen()
					If (formatter Is Nothing) OrElse (formatter.locale() IsNot l) Then formatter = New java.util.Formatter(Me, l)
					formatter.format(l, format, args)
				End SyncLock
			Catch x As InterruptedIOException
				Thread.CurrentThread.Interrupt()
			Catch x As IOException
				trouble = True
			End Try
			Return Me

		''' <summary>
		''' Appends the specified character sequence to this output stream.
		''' 
		''' <p> An invocation of this method of the form <tt>out.append(csq)</tt>
		''' behaves in exactly the same way as the invocation
		''' 
		''' <pre>
		'''     out.print(csq.toString()) </pre>
		''' 
		''' <p> Depending on the specification of <tt>toString</tt> for the
		''' character sequence <tt>csq</tt>, the entire sequence may not be
		''' appended.  For instance, invoking then <tt>toString</tt> method of a
		''' character buffer will return a subsequence whose content depends upon
		''' the buffer's position and limit.
		''' </summary>
		''' <param name="csq">
		'''         The character sequence to append.  If <tt>csq</tt> is
		'''         <tt>null</tt>, then the four characters <tt>"null"</tt> are
		'''         appended to this output stream.
		''' </param>
		''' <returns>  This output stream
		''' 
		''' @since  1.5 </returns>
		public PrintStream append(CharSequence csq)
			If csq Is Nothing Then
				print("null")
			Else
				print(csq.ToString())
			End If
			Return Me

		''' <summary>
		''' Appends a subsequence of the specified character sequence to this output
		''' stream.
		''' 
		''' <p> An invocation of this method of the form <tt>out.append(csq, start,
		''' end)</tt> when <tt>csq</tt> is not <tt>null</tt>, behaves in
		''' exactly the same way as the invocation
		''' 
		''' <pre>
		'''     out.print(csq.subSequence(start, end).toString()) </pre>
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
		''' <returns>  This output stream
		''' </returns>
		''' <exception cref="IndexOutOfBoundsException">
		'''          If <tt>start</tt> or <tt>end</tt> are negative, <tt>start</tt>
		'''          is greater than <tt>end</tt>, or <tt>end</tt> is greater than
		'''          <tt>csq.length()</tt>
		''' 
		''' @since  1.5 </exception>
		public PrintStream append(CharSequence csq, Integer start, Integer end)
			Dim cs As CharSequence = (If(csq Is Nothing, "null", csq))
			write(cs.subSequence(start, end).ToString())
			Return Me

		''' <summary>
		''' Appends the specified character to this output stream.
		''' 
		''' <p> An invocation of this method of the form <tt>out.append(c)</tt>
		''' behaves in exactly the same way as the invocation
		''' 
		''' <pre>
		'''     out.print(c) </pre>
		''' </summary>
		''' <param name="c">
		'''         The 16-bit character to append
		''' </param>
		''' <returns>  This output stream
		''' 
		''' @since  1.5 </returns>
		public PrintStream append(Char c)
			print(c)
			Return Me

	End Class

End Namespace