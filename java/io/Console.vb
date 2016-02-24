Imports Microsoft.VisualBasic
Imports System
Imports System.Diagnostics
Imports System.Runtime.InteropServices

'
' * Copyright (c) 2005, 2013, Oracle and/or its affiliates. All rights reserved.
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
	''' Methods to access the character-based console device, if any, associated
	''' with the current Java virtual machine.
	''' 
	''' <p> Whether a virtual machine has a console is dependent upon the
	''' underlying platform and also upon the manner in which the virtual
	''' machine is invoked.  If the virtual machine is started from an
	''' interactive command line without redirecting the standard input and
	''' output streams then its console will exist and will typically be
	''' connected to the keyboard and display from which the virtual machine
	''' was launched.  If the virtual machine is started automatically, for
	''' example by a background job scheduler, then it will typically not
	''' have a console.
	''' <p>
	''' If this virtual machine has a console then it is represented by a
	''' unique instance of this class which can be obtained by invoking the
	''' <seealso cref="java.lang.System#console()"/> method.  If no console device is
	''' available then an invocation of that method will return <tt>null</tt>.
	''' <p>
	''' Read and write operations are synchronized to guarantee the atomic
	''' completion of critical operations; therefore invoking methods
	''' <seealso cref="#readLine()"/>, <seealso cref="#readPassword()"/>, <seealso cref="#format format()"/>,
	''' <seealso cref="#printf printf()"/> as well as the read, format and write operations
	''' on the objects returned by <seealso cref="#reader()"/> and <seealso cref="#writer()"/> may
	''' block in multithreaded scenarios.
	''' <p>
	''' Invoking <tt>close()</tt> on the objects returned by the <seealso cref="#reader()"/>
	''' and the <seealso cref="#writer()"/> will not close the underlying stream of those
	''' objects.
	''' <p>
	''' The console-read methods return <tt>null</tt> when the end of the
	''' console input stream is reached, for example by typing control-D on
	''' Unix or control-Z on Windows.  Subsequent read operations will succeed
	''' if additional characters are later entered on the console's input
	''' device.
	''' <p>
	''' Unless otherwise specified, passing a <tt>null</tt> argument to any method
	''' in this class will cause a <seealso cref="NullPointerException"/> to be thrown.
	''' <p>
	''' <b>Security note:</b>
	''' If an application needs to read a password or other secure data, it should
	''' use <seealso cref="#readPassword()"/> or <seealso cref="#readPassword(String, Object...)"/> and
	''' manually zero the returned character array after processing to minimize the
	''' lifetime of sensitive data in memory.
	''' 
	''' <blockquote><pre>{@code
	''' Console cons;
	''' char[] passwd;
	''' if ((cons = System.console()) != null &&
	'''     (passwd = cons.readPassword("[%s]", "Password:")) != null) {
	'''     ...
	'''     java.util.Arrays.fill(passwd, ' ');
	''' }
	''' }</pre></blockquote>
	''' 
	''' @author  Xueming Shen
	''' @since   1.6
	''' </summary>

	Public NotInheritable Class Console
		Implements Flushable

	   ''' <summary>
	   ''' Retrieves the unique <seealso cref="java.io.PrintWriter PrintWriter"/> object
	   ''' associated with this console.
	   ''' </summary>
	   ''' <returns>  The printwriter associated with this console </returns>
		Public Function writer() As PrintWriter
			Return pw
		End Function

	   ''' <summary>
	   ''' Retrieves the unique <seealso cref="java.io.Reader Reader"/> object associated
	   ''' with this console.
	   ''' <p>
	   ''' This method is intended to be used by sophisticated applications, for
	   ''' example, a <seealso cref="java.util.Scanner"/> object which utilizes the rich
	   ''' parsing/scanning functionality provided by the <tt>Scanner</tt>:
	   ''' <blockquote><pre>
	   ''' Console con = System.console();
	   ''' if (con != null) {
	   '''     Scanner sc = new Scanner(con.reader());
	   '''     ...
	   ''' }
	   ''' </pre></blockquote>
	   ''' <p>
	   ''' For simple applications requiring only line-oriented reading, use
	   ''' <tt><seealso cref="#readLine"/></tt>.
	   ''' <p>
	   ''' The bulk read operations <seealso cref="java.io.Reader#read(char[]) read(char[]) "/>,
	   ''' <seealso cref="java.io.Reader#read(char[], int, int) read(char[], int, int) "/> and
	   ''' <seealso cref="java.io.Reader#read(java.nio.CharBuffer) read(java.nio.CharBuffer)"/>
	   ''' on the returned object will not read in characters beyond the line
	   ''' bound for each invocation, even if the destination buffer has space for
	   ''' more characters. The {@code Reader}'s {@code read} methods may block if a
	   ''' line bound has not been entered or reached on the console's input device.
	   ''' A line bound is considered to be any one of a line feed (<tt>'\n'</tt>),
	   ''' a carriage return (<tt>'\r'</tt>), a carriage return followed immediately
	   ''' by a linefeed, or an end of stream.
	   ''' </summary>
	   ''' <returns>  The reader associated with this console </returns>
		Public Function reader() As Reader
			Return reader_Renamed
		End Function

	   ''' <summary>
	   ''' Writes a formatted string to this console's output stream using
	   ''' the specified format string and arguments.
	   ''' </summary>
	   ''' <param name="fmt">
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
	   ''' <exception cref="IllegalFormatException">
	   '''          If a format string contains an illegal syntax, a format
	   '''          specifier that is incompatible with the given arguments,
	   '''          insufficient arguments given the format string, or other
	   '''          illegal conditions.  For specification of all possible
	   '''          formatting errors, see the <a
	   '''          href="../util/Formatter.html#detail">Details</a> section
	   '''          of the formatter class specification.
	   ''' </exception>
	   ''' <returns>  This console </returns>
		Public Function format(ByVal fmt As String, ParamArray ByVal args As Object()) As Console
			formatter.format(fmt, args).flush()
			Return Me
		End Function

	   ''' <summary>
	   ''' A convenience method to write a formatted string to this console's
	   ''' output stream using the specified format string and arguments.
	   ''' 
	   ''' <p> An invocation of this method of the form <tt>con.printf(format,
	   ''' args)</tt> behaves in exactly the same way as the invocation of
	   ''' <pre>con.format(format, args)</pre>.
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
	   ''' <exception cref="IllegalFormatException">
	   '''          If a format string contains an illegal syntax, a format
	   '''          specifier that is incompatible with the given arguments,
	   '''          insufficient arguments given the format string, or other
	   '''          illegal conditions.  For specification of all possible
	   '''          formatting errors, see the <a
	   '''          href="../util/Formatter.html#detail">Details</a> section of the
	   '''          formatter class specification.
	   ''' </exception>
	   ''' <returns>  This console </returns>
		Public Function printf(ByVal format As String, ParamArray ByVal args As Object()) As Console
			Return format(format, args)
		End Function

	   ''' <summary>
	   ''' Provides a formatted prompt, then reads a single line of text from the
	   ''' console.
	   ''' </summary>
	   ''' <param name="fmt">
	   '''         A format string as described in <a
	   '''         href="../util/Formatter.html#syntax">Format string syntax</a>.
	   ''' </param>
	   ''' <param name="args">
	   '''         Arguments referenced by the format specifiers in the format
	   '''         string.  If there are more arguments than format specifiers, the
	   '''         extra arguments are ignored.  The maximum number of arguments is
	   '''         limited by the maximum dimension of a Java array as defined by
	   '''         <cite>The Java&trade; Virtual Machine Specification</cite>.
	   ''' </param>
	   ''' <exception cref="IllegalFormatException">
	   '''          If a format string contains an illegal syntax, a format
	   '''          specifier that is incompatible with the given arguments,
	   '''          insufficient arguments given the format string, or other
	   '''          illegal conditions.  For specification of all possible
	   '''          formatting errors, see the <a
	   '''          href="../util/Formatter.html#detail">Details</a> section
	   '''          of the formatter class specification.
	   ''' </exception>
	   ''' <exception cref="IOError">
	   '''         If an I/O error occurs.
	   ''' </exception>
	   ''' <returns>  A string containing the line read from the console, not
	   '''          including any line-termination characters, or <tt>null</tt>
	   '''          if an end of stream has been reached. </returns>
		Public Function readLine(ByVal fmt As String, ParamArray ByVal args As Object()) As String
			Dim line As String = Nothing
			SyncLock writeLock
				SyncLock readLock
					If fmt.length() <> 0 Then pw.format(fmt, args)
					Try
						Dim ca As Char() = readline(False)
						If ca IsNot Nothing Then line = New String(ca)
					Catch x As IOException
						Throw New IOError(x)
					End Try
				End SyncLock
			End SyncLock
			Return line
		End Function

	   ''' <summary>
	   ''' Reads a single line of text from the console.
	   ''' </summary>
	   ''' <exception cref="IOError">
	   '''         If an I/O error occurs.
	   ''' </exception>
	   ''' <returns>  A string containing the line read from the console, not
	   '''          including any line-termination characters, or <tt>null</tt>
	   '''          if an end of stream has been reached. </returns>
		Public Function readLine() As String
			Return readLine("")
		End Function

	   ''' <summary>
	   ''' Provides a formatted prompt, then reads a password or passphrase from
	   ''' the console with echoing disabled.
	   ''' </summary>
	   ''' <param name="fmt">
	   '''         A format string as described in <a
	   '''         href="../util/Formatter.html#syntax">Format string syntax</a>
	   '''         for the prompt text.
	   ''' </param>
	   ''' <param name="args">
	   '''         Arguments referenced by the format specifiers in the format
	   '''         string.  If there are more arguments than format specifiers, the
	   '''         extra arguments are ignored.  The maximum number of arguments is
	   '''         limited by the maximum dimension of a Java array as defined by
	   '''         <cite>The Java&trade; Virtual Machine Specification</cite>.
	   ''' </param>
	   ''' <exception cref="IllegalFormatException">
	   '''          If a format string contains an illegal syntax, a format
	   '''          specifier that is incompatible with the given arguments,
	   '''          insufficient arguments given the format string, or other
	   '''          illegal conditions.  For specification of all possible
	   '''          formatting errors, see the <a
	   '''          href="../util/Formatter.html#detail">Details</a>
	   '''          section of the formatter class specification.
	   ''' </exception>
	   ''' <exception cref="IOError">
	   '''         If an I/O error occurs.
	   ''' </exception>
	   ''' <returns>  A character array containing the password or passphrase read
	   '''          from the console, not including any line-termination characters,
	   '''          or <tt>null</tt> if an end of stream has been reached. </returns>
		Public Function readPassword(ByVal fmt As String, ParamArray ByVal args As Object()) As Char()
			Dim passwd As Char() = Nothing
			SyncLock writeLock
				SyncLock readLock
					Try
						echoOff = echo(False)
					Catch x As IOException
						Throw New IOError(x)
					End Try
					Dim ioe As IOError = Nothing
					Try
						If fmt.length() <> 0 Then pw.format(fmt, args)
						passwd = readline(True)
					Catch x As IOException
						ioe = New IOError(x)
					Finally
						Try
							echoOff = echo(True)
						Catch x As IOException
							If ioe Is Nothing Then
								ioe = New IOError(x)
							Else
								ioe.addSuppressed(x)
							End If
						End Try
						If ioe IsNot Nothing Then Throw ioe
					End Try
					pw.println()
				End SyncLock
			End SyncLock
			Return passwd
		End Function

	   ''' <summary>
	   ''' Reads a password or passphrase from the console with echoing disabled
	   ''' </summary>
	   ''' <exception cref="IOError">
	   '''         If an I/O error occurs.
	   ''' </exception>
	   ''' <returns>  A character array containing the password or passphrase read
	   '''          from the console, not including any line-termination characters,
	   '''          or <tt>null</tt> if an end of stream has been reached. </returns>
		Public Function readPassword() As Char()
			Return readPassword("")
		End Function

		''' <summary>
		''' Flushes the console and forces any buffered output to be written
		''' immediately .
		''' </summary>
		Public Sub flush() Implements Flushable.flush
			pw.flush()
		End Sub

		Private readLock As Object
		Private writeLock As Object
		Private reader_Renamed As Reader
		Private out As Writer
		Private pw As PrintWriter
		Private formatter As Formatter
		Private cs As java.nio.charset.Charset
		Private rcb As Char()
'JAVA TO VB CONVERTER TODO TASK: Replace 'unknown' with the appropriate dll name:
		<DllImport("unknown")> _
		Private Shared Function encoding() As String
		End Function
'JAVA TO VB CONVERTER TODO TASK: Replace 'unknown' with the appropriate dll name:
		<DllImport("unknown")> _
		Private Shared Function echo(ByVal [on] As Boolean) As Boolean
		End Function
		Private Shared echoOff As Boolean

		Private Function readline(ByVal zeroOut As Boolean) As Char()
			Dim len As Integer = reader_Renamed.read(rcb, 0, rcb.Length)
			If len < 0 Then Return Nothing 'EOL
			If rcb(len-1) = ControlChars.Cr Then
				len -= 1 'remove CR at end;
			ElseIf rcb(len-1) = ControlChars.Lf Then
				len -= 1 'remove LF at end;
				If len > 0 AndAlso rcb(len-1) = ControlChars.Cr Then len -= 1 'remove the CR, if there is one
			End If
			Dim b As Char() = New Char(len - 1){}
			If len > 0 Then
				Array.Copy(rcb, 0, b, 0, len)
				If zeroOut Then Arrays.fill(rcb, 0, len, " "c)
			End If
			Return b
		End Function

		Private Function grow() As Char()
			Debug.Assert(Thread.holdsLock(readLock))
			Dim t As Char() = New Char(rcb.Length * 2 - 1){}
			Array.Copy(rcb, 0, t, 0, rcb.Length)
			rcb = t
			Return rcb
		End Function

		Friend Class LineReader
			Inherits Reader

			Private ReadOnly outerInstance As Console

			Private [in] As Reader
			Private cb As Char()
			Private nChars, nextChar As Integer
			Friend leftoverLF As Boolean
			Friend Sub New(ByVal outerInstance As Console, ByVal [in] As Reader)
					Me.outerInstance = outerInstance
				Me.in = [in]
				cb = New Char(1023){}
					nChars = 0
					nextChar = nChars
				leftoverLF = False
			End Sub
			Public Overrides Sub close()
			End Sub
			Public Overrides Function ready() As Boolean
				'in.ready synchronizes on readLock already
				Return [in].ready()
			End Function

'JAVA TO VB CONVERTER TODO TASK: The following line could not be converted:
			public int read(char cbuf() , int offset, int length) throws IOException
				Dim [off] As Integer = offset
				Dim [end] As Integer = offset + length
				If offset < 0 OrElse offset > cbuf.length OrElse length < 0 OrElse [end] < 0 OrElse [end] > cbuf.length Then Throw New IndexOutOfBoundsException
				SyncLock outerInstance.readLock
					Dim eof As Boolean = False
					Dim c As Char = 0
					Do
						If nextChar >= nChars Then 'fill
							Dim n As Integer = 0
							Do
								n = [in].read(cb, 0, cb.Length)
							Loop While n = 0
							If n > 0 Then
								nChars = n
								nextChar = 0
								If n < cb.Length AndAlso cb(n-1) <> ControlChars.Lf AndAlso cb(n-1) <> ControlChars.Cr Then
	'                                
	'                                 * we're in canonical mode so each "fill" should
	'                                 * come back with an eol. if there no lf or nl at
	'                                 * the end of returned bytes we reached an eof.
	'                                 
									eof = True
								End If 'EOF
							Else
								If [off] - offset = 0 Then Return -1
								Return [off] - offset
							End If
						End If
						If leftoverLF AndAlso cbuf = outerInstance.rcb AndAlso cb(nextChar) = ControlChars.Lf Then nextChar += 1
						leftoverLF = False
						Do While nextChar < nChars
								cbuf([off]) = cb(nextChar)
								c = cbuf([off])
							[off] += 1
							cb(nextChar) = 0
							nextChar += 1
							If c = ControlChars.Lf Then
								Return [off] - offset
							ElseIf c = ControlChars.Cr Then
								If [off] = [end] Then
	'                                 no space left even the next is LF, so return
	'                                 * whatever we have if the invoker is not our
	'                                 * readLine()
	'                                 
									If cbuf = outerInstance.rcb Then
										cbuf = outerInstance.grow()
										[end] = cbuf.length
									Else
										leftoverLF = True
										Return [off] - offset
									End If
								End If
								If nextChar = nChars AndAlso [in].ready() Then
	'                                
	'                                 * we have a CR and we reached the end of
	'                                 * the read in buffer, fill to make sure we
	'                                 * don't miss a LF, if there is one, it's possible
	'                                 * that it got cut off during last round reading
	'                                 * simply because the read in buffer was full.
	'                                 
									nChars = [in].read(cb, 0, cb.Length)
									nextChar = 0
								End If
								If nextChar < nChars AndAlso cb(nextChar) = ControlChars.Lf Then
									cbuf([off]) = ControlChars.Lf
									[off] += 1
									nextChar += 1
								End If
								Return [off] - offset
							ElseIf [off] = [end] Then
							   If cbuf = outerInstance.rcb Then
									cbuf = outerInstance.grow()
									[end] = cbuf.length
							   Else
								   Return [off] - offset
							   End If
							End If
						Loop
						If eof Then Return [off] - offset
					Loop
				End SyncLock
		End Class

		' Set up JavaIOAccess in SharedSecrets
		static Console()
			Try
				' Add a shutdown hook to restore console's echo state should
				' it be necessary.
				sun.misc.SharedSecrets.javaLangAccess.registerShutdownHook(0, False, New RunnableAnonymousInnerClassHelper ' only register if shutdown is not in progress -  shutdown hook invocation order
			Catch e As IllegalStateException
				' shutdown is already in progress and console is first used
				' by a shutdown hook
			End Try

'JAVA TO VB CONVERTER TODO TASK: Anonymous inner classes are not converted to VB if the base type is not defined in the code being converted:
'			sun.misc.SharedSecrets.setJavaIOAccess(New sun.misc.JavaIOAccess()
	'		{
	'			public Console console()
	'			{
	'				if (istty())
	'				{
	'					if (cons == Nothing)
	'						cons = New Console();
	'					Return cons;
	'				}
	'				Return Nothing;
	'			}
	'
	'			public Charset charset()
	'			{
	'				' This method is called in sun.security.util.Password,
	'				' cons already exists when this method is called
	'				Return cons.cs;
	'			}
	'		});
		private static Console cons
		private native static Boolean istty()
		private Console()
			readLock = New Object
			writeLock = New Object
			Dim csname As String = encoding()
			If csname IsNot Nothing Then
				Try
					cs = java.nio.charset.Charset.forName(csname)
				Catch x As Exception
				End Try
			End If
			If cs Is Nothing Then cs = java.nio.charset.Charset.defaultCharset()
			out = sun.nio.cs.StreamEncoder.forOutputStreamWriter(New FileOutputStream(FileDescriptor.out), writeLock, cs)
			pw = New PrintWriterAnonymousInnerClassHelper
			formatter = New Formatter(out)
			reader_Renamed = New LineReader(Me, sun.nio.cs.StreamDecoder.forInputStreamReader(New FileInputStream(FileDescriptor.in), readLock, cs))
			rcb = New Char(1023){}
	End Class


	Private Class RunnableAnonymousInnerClassHelper
		Implements Runnable

		Public Overridable Sub run() Implements Runnable.run
			Try
				If echoOff Then echo(True)
			Catch x As IOException
			End Try
		End Sub
	End Class

	Private Class PrintWriterAnonymousInnerClassHelper
		Inherits PrintWriter

		Public Overrides Sub close()
		End Sub
	End Class
End Namespace