Imports System
Imports System.Threading

'
' * Copyright (c) 1996, 2012, Oracle and/or its affiliates. All rights reserved.
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
	''' Prints formatted representations of objects to a text-output stream.  This
	''' class implements all of the <tt>print</tt> methods found in {@link
	''' PrintStream}.  It does not contain methods for writing raw bytes, for which
	''' a program should use unencoded byte streams.
	''' 
	''' <p> Unlike the <seealso cref="PrintStream"/> [Class], if automatic flushing is enabled
	''' it will be done only when one of the <tt>println</tt>, <tt>printf</tt>, or
	''' <tt>format</tt> methods is invoked, rather than whenever a newline character
	''' happens to be output.  These methods use the platform's own notion of line
	''' separator rather than the newline character.
	''' 
	''' <p> Methods in this class never throw I/O exceptions, although some of its
	''' constructors may.  The client may inquire as to whether any errors have
	''' occurred by invoking <seealso cref="#checkError checkError()"/>.
	''' 
	''' @author      Frank Yellin
	''' @author      Mark Reinhold
	''' @since       JDK1.1
	''' </summary>

	Public Class PrintWriter
		Inherits Writer

		''' <summary>
		''' The underlying character-output stream of this
		''' <code>PrintWriter</code>.
		''' 
		''' @since 1.2
		''' </summary>
		Protected Friend out As Writer

		Private ReadOnly autoFlush As Boolean
		Private trouble As Boolean = False
		Private formatter As java.util.Formatter
		Private psOut As PrintStream = Nothing

		''' <summary>
		''' Line separator string.  This is the value of the line.separator
		''' property at the moment that the stream was created.
		''' </summary>
		Private ReadOnly lineSeparator As String

		''' <summary>
		''' Returns a charset object for the given charset name. </summary>
		''' <exception cref="NullPointerException">          is csn is null </exception>
		''' <exception cref="UnsupportedEncodingException">  if the charset is not supported </exception>
		Private Shared Function toCharset(ByVal csn As String) As java.nio.charset.Charset
			java.util.Objects.requireNonNull(csn, "charsetName")
			Try
				Return java.nio.charset.Charset.forName(csn)
'JAVA TO VB CONVERTER TODO TASK: There is no equivalent in VB to Java 'multi-catch' syntax:
			Catch java.nio.charset.IllegalCharsetNameException Or java.nio.charset.UnsupportedCharsetException unused
				' UnsupportedEncodingException should be thrown
				Throw New UnsupportedEncodingException(csn)
			End Try
		End Function

		''' <summary>
		''' Creates a new PrintWriter, without automatic line flushing.
		''' </summary>
		''' <param name="out">        A character-output stream </param>
		Public Sub New(ByVal out As Writer)
			Me.New(out, False)
		End Sub

		''' <summary>
		''' Creates a new PrintWriter.
		''' </summary>
		''' <param name="out">        A character-output stream </param>
		''' <param name="autoFlush">  A boolean; if true, the <tt>println</tt>,
		'''                    <tt>printf</tt>, or <tt>format</tt> methods will
		'''                    flush the output buffer </param>
		Public Sub New(ByVal out As Writer, ByVal autoFlush As Boolean)
			MyBase.New(out)
			Me.out = out
			Me.autoFlush = autoFlush
			lineSeparator = java.security.AccessController.doPrivileged(New sun.security.action.GetPropertyAction("line.separator"))
		End Sub

		''' <summary>
		''' Creates a new PrintWriter, without automatic line flushing, from an
		''' existing OutputStream.  This convenience constructor creates the
		''' necessary intermediate OutputStreamWriter, which will convert characters
		''' into bytes using the default character encoding.
		''' </summary>
		''' <param name="out">        An output stream
		''' </param>
		''' <seealso cref= java.io.OutputStreamWriter#OutputStreamWriter(java.io.OutputStream) </seealso>
		Public Sub New(ByVal out As OutputStream)
			Me.New(out, False)
		End Sub

		''' <summary>
		''' Creates a new PrintWriter from an existing OutputStream.  This
		''' convenience constructor creates the necessary intermediate
		''' OutputStreamWriter, which will convert characters into bytes using the
		''' default character encoding.
		''' </summary>
		''' <param name="out">        An output stream </param>
		''' <param name="autoFlush">  A boolean; if true, the <tt>println</tt>,
		'''                    <tt>printf</tt>, or <tt>format</tt> methods will
		'''                    flush the output buffer
		''' </param>
		''' <seealso cref= java.io.OutputStreamWriter#OutputStreamWriter(java.io.OutputStream) </seealso>
		Public Sub New(ByVal out As OutputStream, ByVal autoFlush As Boolean)
			Me.New(New BufferedWriter(New OutputStreamWriter(out)), autoFlush)

			' save print stream for error propagation
			If TypeOf out Is java.io.PrintStream Then psOut = CType(out, PrintStream)
		End Sub

		''' <summary>
		''' Creates a new PrintWriter, without automatic line flushing, with the
		''' specified file name.  This convenience constructor creates the necessary
		''' intermediate <seealso cref="java.io.OutputStreamWriter OutputStreamWriter"/>,
		''' which will encode characters using the {@linkplain
		''' java.nio.charset.Charset#defaultCharset() default charset} for this
		''' instance of the Java virtual machine.
		''' </summary>
		''' <param name="fileName">
		'''         The name of the file to use as the destination of this writer.
		'''         If the file exists then it will be truncated to zero size;
		'''         otherwise, a new file will be created.  The output will be
		'''         written to the file and is buffered.
		''' </param>
		''' <exception cref="FileNotFoundException">
		'''          If the given string does not denote an existing, writable
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
			Me.New(New BufferedWriter(New OutputStreamWriter(New FileOutputStream(fileName))), False)
		End Sub

		' Private constructor 
		Private Sub New(ByVal charset As java.nio.charset.Charset, ByVal file_Renamed As File)
			Me.New(New BufferedWriter(New OutputStreamWriter(New FileOutputStream(file_Renamed), charset)), False)
		End Sub

		''' <summary>
		''' Creates a new PrintWriter, without automatic line flushing, with the
		''' specified file name and charset.  This convenience constructor creates
		''' the necessary intermediate {@link java.io.OutputStreamWriter
		''' OutputStreamWriter}, which will encode characters using the provided
		''' charset.
		''' </summary>
		''' <param name="fileName">
		'''         The name of the file to use as the destination of this writer.
		'''         If the file exists then it will be truncated to zero size;
		'''         otherwise, a new file will be created.  The output will be
		'''         written to the file and is buffered.
		''' </param>
		''' <param name="csn">
		'''         The name of a supported {@link java.nio.charset.Charset
		'''         charset}
		''' </param>
		''' <exception cref="FileNotFoundException">
		'''          If the given string does not denote an existing, writable
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
			Me.New(toCharset(csn), New File(fileName))
		End Sub

		''' <summary>
		''' Creates a new PrintWriter, without automatic line flushing, with the
		''' specified file.  This convenience constructor creates the necessary
		''' intermediate <seealso cref="java.io.OutputStreamWriter OutputStreamWriter"/>,
		''' which will encode characters using the {@linkplain
		''' java.nio.charset.Charset#defaultCharset() default charset} for this
		''' instance of the Java virtual machine.
		''' </summary>
		''' <param name="file">
		'''         The file to use as the destination of this writer.  If the file
		'''         exists then it will be truncated to zero size; otherwise, a new
		'''         file will be created.  The output will be written to the file
		'''         and is buffered.
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
			Me.New(New BufferedWriter(New OutputStreamWriter(New FileOutputStream(file_Renamed))), False)
		End Sub

		''' <summary>
		''' Creates a new PrintWriter, without automatic line flushing, with the
		''' specified file and charset.  This convenience constructor creates the
		''' necessary intermediate {@link java.io.OutputStreamWriter
		''' OutputStreamWriter}, which will encode characters using the provided
		''' charset.
		''' </summary>
		''' <param name="file">
		'''         The file to use as the destination of this writer.  If the file
		'''         exists then it will be truncated to zero size; otherwise, a new
		'''         file will be created.  The output will be written to the file
		'''         and is buffered.
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
			Me.New(toCharset(csn), file_Renamed)
		End Sub

		''' <summary>
		''' Checks to make sure that the stream has not been closed </summary>
		Private Sub ensureOpen()
			If out Is Nothing Then Throw New IOException("Stream closed")
		End Sub

		''' <summary>
		''' Flushes the stream. </summary>
		''' <seealso cref= #checkError() </seealso>
		Public Overrides Sub flush()
			Try
				SyncLock lock
					ensureOpen()
					out.flush()
				End SyncLock
			Catch x As IOException
				trouble = True
			End Try
		End Sub

		''' <summary>
		''' Closes the stream and releases any system resources associated
		''' with it. Closing a previously closed stream has no effect.
		''' </summary>
		''' <seealso cref= #checkError() </seealso>
		Public Overrides Sub close()
			Try
				SyncLock lock
					If out Is Nothing Then Return
					out.close()
					out = Nothing
				End SyncLock
			Catch x As IOException
				trouble = True
			End Try
		End Sub

		''' <summary>
		''' Flushes the stream if it's not closed and checks its error state.
		''' </summary>
		''' <returns> <code>true</code> if the print stream has encountered an error,
		'''          either on the underlying output stream or during a format
		'''          conversion. </returns>
		Public Overridable Function checkError() As Boolean
			If out IsNot Nothing Then flush()
			If TypeOf out Is java.io.PrintWriter Then
				Dim pw As PrintWriter = CType(out, PrintWriter)
				Return pw.checkError()
			ElseIf psOut IsNot Nothing Then
				Return psOut.checkError()
			End If
			Return trouble
		End Function

		''' <summary>
		''' Indicates that an error has occurred.
		''' 
		''' <p> This method will cause subsequent invocations of {@link
		''' #checkError()} to return <tt>true</tt> until {@link
		''' #clearError()} is invoked.
		''' </summary>
		Protected Friend Overridable Sub setError()
			trouble = True
		End Sub

		''' <summary>
		''' Clears the error state of this stream.
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
	'     * which also implement the write() methods of Writer
	'     

		''' <summary>
		''' Writes a single character. </summary>
		''' <param name="c"> int specifying a character to be written. </param>
		Public Overrides Sub write(ByVal c As Integer)
			Try
				SyncLock lock
					ensureOpen()
					out.write(c)
				End SyncLock
			Catch x As InterruptedIOException
				Thread.CurrentThread.Interrupt()
			Catch x As IOException
				trouble = True
			End Try
		End Sub

		''' <summary>
		''' Writes A Portion of an array of characters. </summary>
		''' <param name="buf"> Array of characters </param>
		''' <param name="off"> Offset from which to start writing characters </param>
		''' <param name="len"> Number of characters to write </param>
'JAVA TO VB CONVERTER TODO TASK: The following line could not be converted:
		public void write(char buf() , int off, int len)
			Try
				SyncLock lock
					ensureOpen()
					out.write(buf, off, len)
				End SyncLock
			Catch x As InterruptedIOException
				Thread.CurrentThread.Interrupt()
			Catch x As IOException
				trouble = True
			End Try

		''' <summary>
		''' Writes an array of characters.  This method cannot be inherited from the
		''' Writer class because it must suppress I/O exceptions. </summary>
		''' <param name="buf"> Array of characters to be written </param>
		public void write(Char buf())
			write(buf, 0, buf.length)

		''' <summary>
		''' Writes a portion of a string. </summary>
		''' <param name="s"> A String </param>
		''' <param name="off"> Offset from which to start writing characters </param>
		''' <param name="len"> Number of characters to write </param>
		public void write(String s, Integer off, Integer len)
			Try
				SyncLock lock
					ensureOpen()
					out.write(s, off, len)
				End SyncLock
			Catch x As InterruptedIOException
				Thread.CurrentThread.Interrupt()
			Catch x As IOException
				trouble = True
			End Try

		''' <summary>
		''' Writes a string.  This method cannot be inherited from the Writer class
		''' because it must suppress I/O exceptions. </summary>
		''' <param name="s"> String to be written </param>
		public void write(String s)
			write(s, 0, s.length())

		private void newLine()
			Try
				SyncLock lock
					ensureOpen()
					out.write(lineSeparator)
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
		''' are written in exactly the manner of the <code>{@link
		''' #write(int)}</code> method.
		''' </summary>
		''' <param name="b">   The <code>boolean</code> to be printed </param>
		public void print(Boolean b)
			write(If(b, "true", "false"))

		''' <summary>
		''' Prints a character.  The character is translated into one or more bytes
		''' according to the platform's default character encoding, and these bytes
		''' are written in exactly the manner of the <code>{@link
		''' #write(int)}</code> method.
		''' </summary>
		''' <param name="c">   The <code>char</code> to be printed </param>
		public void print(Char c)
			write(c)

		''' <summary>
		''' Prints an  java.lang.[Integer].  The string produced by <code>{@link
		''' java.lang.String#valueOf(int)}</code> is translated into bytes according
		''' to the platform's default character encoding, and these bytes are
		''' written in exactly the manner of the <code><seealso cref="#write(int)"/></code>
		''' method.
		''' </summary>
		''' <param name="i">   The <code>int</code> to be printed </param>
		''' <seealso cref=        java.lang.Integer#toString(int) </seealso>
		public void print(Integer i)
			write(Convert.ToString(i))

		''' <summary>
		''' Prints a long  java.lang.[Integer].  The string produced by <code>{@link
		''' java.lang.String#valueOf(long)}</code> is translated into bytes
		''' according to the platform's default character encoding, and these bytes
		''' are written in exactly the manner of the <code><seealso cref="#write(int)"/></code>
		''' method.
		''' </summary>
		''' <param name="l">   The <code>long</code> to be printed </param>
		''' <seealso cref=        java.lang.Long#toString(long) </seealso>
		public void print(Long l)
			write(Convert.ToString(l))

		''' <summary>
		''' Prints a floating-point number.  The string produced by <code>{@link
		''' java.lang.String#valueOf(float)}</code> is translated into bytes
		''' according to the platform's default character encoding, and these bytes
		''' are written in exactly the manner of the <code><seealso cref="#write(int)"/></code>
		''' method.
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
		''' are written in exactly the manner of the <code><seealso cref="#write(int)"/></code>
		''' method.
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
		''' are written in exactly the manner of the <code><seealso cref="#write(int)"/></code>
		''' method.
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
		''' Prints a boolean value and then terminates the line.  This method behaves
		''' as though it invokes <code><seealso cref="#print(boolean)"/></code> and then
		''' <code><seealso cref="#println()"/></code>.
		''' </summary>
		''' <param name="x"> the <code>boolean</code> value to be printed </param>
		public void println(Boolean x)
			SyncLock lock
				print(x)
				println()
			End SyncLock

		''' <summary>
		''' Prints a character and then terminates the line.  This method behaves as
		''' though it invokes <code><seealso cref="#print(char)"/></code> and then <code>{@link
		''' #println()}</code>.
		''' </summary>
		''' <param name="x"> the <code>char</code> value to be printed </param>
		public void println(Char x)
			SyncLock lock
				print(x)
				println()
			End SyncLock

		''' <summary>
		''' Prints an integer and then terminates the line.  This method behaves as
		''' though it invokes <code><seealso cref="#print(int)"/></code> and then <code>{@link
		''' #println()}</code>.
		''' </summary>
		''' <param name="x"> the <code>int</code> value to be printed </param>
		public void println(Integer x)
			SyncLock lock
				print(x)
				println()
			End SyncLock

		''' <summary>
		''' Prints a long integer and then terminates the line.  This method behaves
		''' as though it invokes <code><seealso cref="#print(long)"/></code> and then
		''' <code><seealso cref="#println()"/></code>.
		''' </summary>
		''' <param name="x"> the <code>long</code> value to be printed </param>
		public void println(Long x)
			SyncLock lock
				print(x)
				println()
			End SyncLock

		''' <summary>
		''' Prints a floating-point number and then terminates the line.  This method
		''' behaves as though it invokes <code><seealso cref="#print(float)"/></code> and then
		''' <code><seealso cref="#println()"/></code>.
		''' </summary>
		''' <param name="x"> the <code>float</code> value to be printed </param>
		public void println(Single x)
			SyncLock lock
				print(x)
				println()
			End SyncLock

		''' <summary>
		''' Prints a double-precision floating-point number and then terminates the
		''' line.  This method behaves as though it invokes <code>{@link
		''' #print(double)}</code> and then <code><seealso cref="#println()"/></code>.
		''' </summary>
		''' <param name="x"> the <code>double</code> value to be printed </param>
		public void println(Double x)
			SyncLock lock
				print(x)
				println()
			End SyncLock

		''' <summary>
		''' Prints an array of characters and then terminates the line.  This method
		''' behaves as though it invokes <code><seealso cref="#print(char[])"/></code> and then
		''' <code><seealso cref="#println()"/></code>.
		''' </summary>
		''' <param name="x"> the array of <code>char</code> values to be printed </param>
		public void println(Char x())
			SyncLock lock
				print(x)
				println()
			End SyncLock

		''' <summary>
		''' Prints a String and then terminates the line.  This method behaves as
		''' though it invokes <code><seealso cref="#print(String)"/></code> and then
		''' <code><seealso cref="#println()"/></code>.
		''' </summary>
		''' <param name="x"> the <code>String</code> value to be printed </param>
		public void println(String x)
			SyncLock lock
				print(x)
				println()
			End SyncLock

		''' <summary>
		''' Prints an Object and then terminates the line.  This method calls
		''' at first String.valueOf(x) to get the printed object's string value,
		''' then behaves as
		''' though it invokes <code><seealso cref="#print(String)"/></code> and then
		''' <code><seealso cref="#println()"/></code>.
		''' </summary>
		''' <param name="x">  The <code>Object</code> to be printed. </param>
		public void println(Object x)
			Dim s As String = Convert.ToString(x)
			SyncLock lock
				print(s)
				println()
			End SyncLock

		''' <summary>
		''' A convenience method to write a formatted string to this writer using
		''' the specified format string and arguments.  If automatic flushing is
		''' enabled, calls to this method will flush the output buffer.
		''' 
		''' <p> An invocation of this method of the form <tt>out.printf(format,
		''' args)</tt> behaves in exactly the same way as the invocation
		''' 
		''' <pre>
		'''     out.format(format, args) </pre>
		''' </summary>
		''' <param name="format">
		'''         A format string as described in <a
		'''         href="../util/Formatter.html#syntax">Format string syntax</a>.
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
		''' <returns>  This writer
		''' 
		''' @since  1.5 </returns>
		public PrintWriter printf(String format, Object... args)
			Return format(format, args)

		''' <summary>
		''' A convenience method to write a formatted string to this writer using
		''' the specified format string and arguments.  If automatic flushing is
		''' enabled, calls to this method will flush the output buffer.
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
		'''         href="../util/Formatter.html#syntax">Format string syntax</a>.
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
		''' <returns>  This writer
		''' 
		''' @since  1.5 </returns>
		public PrintWriter printf(java.util.Locale l, String format, Object... args)
			Return format(l, format, args)

		''' <summary>
		''' Writes a formatted string to this writer using the specified format
		''' string and arguments.  If automatic flushing is enabled, calls to this
		''' method will flush the output buffer.
		''' 
		''' <p> The locale always used is the one returned by {@link
		''' java.util.Locale#getDefault() Locale.getDefault()}, regardless of any
		''' previous invocations of other formatting methods on this object.
		''' </summary>
		''' <param name="format">
		'''         A format string as described in <a
		'''         href="../util/Formatter.html#syntax">Format string syntax</a>.
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
		'''          Formatter class specification.
		''' </exception>
		''' <exception cref="NullPointerException">
		'''          If the <tt>format</tt> is <tt>null</tt>
		''' </exception>
		''' <returns>  This writer
		''' 
		''' @since  1.5 </returns>
		public PrintWriter format(String format, Object... args)
			Try
				SyncLock lock
					ensureOpen()
					If (formatter Is Nothing) OrElse (formatter.locale() IsNot java.util.Locale.default) Then formatter = New java.util.Formatter(Me)
					formatter.format(java.util.Locale.default, format, args)
					If autoFlush Then out.flush()
				End SyncLock
			Catch x As InterruptedIOException
				Thread.CurrentThread.Interrupt()
			Catch x As IOException
				trouble = True
			End Try
			Return Me

		''' <summary>
		''' Writes a formatted string to this writer using the specified format
		''' string and arguments.  If automatic flushing is enabled, calls to this
		''' method will flush the output buffer.
		''' </summary>
		''' <param name="l">
		'''         The <seealso cref="java.util.Locale locale"/> to apply during
		'''         formatting.  If <tt>l</tt> is <tt>null</tt> then no localization
		'''         is applied.
		''' </param>
		''' <param name="format">
		'''         A format string as described in <a
		'''         href="../util/Formatter.html#syntax">Format string syntax</a>.
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
		''' <returns>  This writer
		''' 
		''' @since  1.5 </returns>
		public PrintWriter format(java.util.Locale l, String format, Object... args)
			Try
				SyncLock lock
					ensureOpen()
					If (formatter Is Nothing) OrElse (formatter.locale() IsNot l) Then formatter = New java.util.Formatter(Me, l)
					formatter.format(l, format, args)
					If autoFlush Then out.flush()
				End SyncLock
			Catch x As InterruptedIOException
				Thread.CurrentThread.Interrupt()
			Catch x As IOException
				trouble = True
			End Try
			Return Me

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
		public PrintWriter append(CharSequence csq)
			If csq Is Nothing Then
				write("null")
			Else
				write(csq.ToString())
			End If
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
		public PrintWriter append(CharSequence csq, Integer start, Integer end)
			Dim cs As CharSequence = (If(csq Is Nothing, "null", csq))
			write(cs.subSequence(start, end).ToString())
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
		public PrintWriter append(Char c)
			write(c)
			Return Me
	End Class

End Namespace