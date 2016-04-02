Imports Microsoft.VisualBasic
Imports System
Imports System.Runtime.CompilerServices
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
Namespace java.rmi.server


	''' <summary>
	''' <code>LogStream</code> provides a mechanism for logging errors that are
	''' of possible interest to those monitoring a system.
	''' 
	''' @author  Ann Wollrath (lots of code stolen from Ken Arnold)
	''' @since   JDK1.1 </summary>
	''' @deprecated no replacement 
	<Obsolete("no replacement")> _
	Public Class LogStream
		Inherits PrintStream

		''' <summary>
		''' table mapping known log names to log stream objects </summary>
		Private Shared known As Map(Of String, LogStream) = New HashMap(Of String, LogStream)(5)
		''' <summary>
		''' default output stream for new logs </summary>
		Private Shared defaultStream As PrintStream = System.err

		''' <summary>
		''' log name for this log </summary>
		Private name As String

		''' <summary>
		''' stream where output of this log is sent to </summary>
		Private logOut As OutputStream

		''' <summary>
		''' string writer for writing message prefixes to log stream </summary>
		Private logWriter As OutputStreamWriter

		''' <summary>
		''' string buffer used for constructing log message prefixes </summary>
		Private buffer As New StringBuffer

		''' <summary>
		''' stream used for buffering lines </summary>
		Private bufOut As ByteArrayOutputStream

		''' <summary>
		''' Create a new LogStream object.  Since this only constructor is
		''' private, users must have a LogStream created through the "log"
		''' method. </summary>
		''' <param name="name"> string identifying messages from this log
		''' @out output stream that log messages will be sent to
		''' @since JDK1.1 </param>
		''' @deprecated no replacement 
		<Obsolete("no replacement")> _
		Private Sub New(ByVal name As String, ByVal out As OutputStream)
			MyBase.New(New ByteArrayOutputStream)
			bufOut = CType(MyBase.out, ByteArrayOutputStream)

			Me.name = name
			outputStream = out
		End Sub

		''' <summary>
		''' Return the LogStream identified by the given name.  If
		''' a log corresponding to "name" does not exist, a log using
		''' the default stream is created. </summary>
		''' <param name="name"> name identifying the desired LogStream </param>
		''' <returns> log associated with given name
		''' @since JDK1.1 </returns>
		''' @deprecated no replacement 
		<Obsolete("no replacement")> _
		Public Shared Function log(ByVal name As String) As LogStream
			Dim stream As LogStream
			SyncLock known
				stream = known.get(name)
				If stream Is Nothing Then stream = New LogStream(name, defaultStream)
				known.put(name, stream)
			End SyncLock
			Return stream
		End Function

		''' <summary>
		''' Return the current default stream for new logs. </summary>
		''' <returns> default log stream </returns>
		''' <seealso cref= #setDefaultStream
		''' @since JDK1.1 </seealso>
		''' @deprecated no replacement 
		<Obsolete("no replacement"), MethodImpl(MethodImplOptions.Synchronized)> _
		Public Property Shared defaultStream As PrintStream
			Get
				Return defaultStream
			End Get
			Set(ByVal newDefault As PrintStream)
				Dim sm As SecurityManager = System.securityManager
    
				If sm IsNot Nothing Then sm.checkPermission(New java.util.logging.LoggingPermission("control", Nothing))
    
				defaultStream = newDefault
			End Set
		End Property


		''' <summary>
		''' Return the current stream to which output from this log is sent. </summary>
		''' <returns> output stream for this log </returns>
		''' <seealso cref= #setOutputStream
		''' @since JDK1.1 </seealso>
		''' @deprecated no replacement 
		<Obsolete("no replacement"), MethodImpl(MethodImplOptions.Synchronized)> _
		Public Overridable Property outputStream As OutputStream
			Get
				Return logOut
			End Get
			Set(ByVal out As OutputStream)
				logOut = out
				' Maintain an OutputStreamWriter with default CharToByteConvertor
				' (just like new PrintStream) for writing log message prefixes.
				logWriter = New OutputStreamWriter(logOut)
			End Set
		End Property


		''' <summary>
		''' Write a byte of data to the stream.  If it is not a newline, then
		''' the byte is appended to the internal buffer.  If it is a newline,
		''' then the currently buffered line is sent to the log's output
		''' stream, prefixed with the appropriate logging information.
		''' @since JDK1.1 </summary>
		''' @deprecated no replacement 
		<Obsolete("no replacement")> _
		Public Overrides Sub write(ByVal b As Integer)
			If b = ControlChars.Lf Then
				' synchronize on "this" first to avoid potential deadlock
				SyncLock Me
					SyncLock logOut
						' construct prefix for log messages:
						buffer.length = 0
						buffer.append((DateTime.Now).ToString()) ' date/time stamp...
						buffer.append(":"c)
						buffer.append(name) ' ...log name...
						buffer.append(":"c)
						buffer.append(Thread.CurrentThread.name)
						buffer.append(":"c) ' ...and thread name

						Try
							' write prefix through to underlying byte stream
							logWriter.write(buffer.ToString())
							logWriter.flush()

							' finally, write the already converted bytes of
							' the log message
							bufOut.writeTo(logOut)
							logOut.write(b)
							logOut.flush()
						Catch e As IOException
							errorror()
						Finally
							bufOut.reset()
						End Try
					End SyncLock
				End SyncLock
			Else
				MyBase.write(b)
			End If
		End Sub

		''' <summary>
		''' Write a subarray of bytes.  Pass each through write byte method.
		''' @since JDK1.1 </summary>
		''' @deprecated no replacement 
'JAVA TO VB CONVERTER TODO TASK: The following line could not be converted:
		public  Sub  write(byte b() , int off, int len)
			If len < 0 Then Throw New ArrayIndexOutOfBoundsException(len)
			For i As Integer = 0 To len - 1
				write(b(off + i))
			Next i

		''' <summary>
		''' Return log name as string representation. </summary>
		''' <returns> log name
		''' @since JDK1.1 </returns>
		''' @deprecated no replacement 
		<Obsolete("no replacement")> _
		public String ToString()
			Return name

		''' <summary>
		''' log level constant (no logging). </summary>
		Public Shared final Integer SILENT = 0
		''' <summary>
		''' log level constant (brief logging). </summary>
		Public Shared final Integer BRIEF = 10
		''' <summary>
		''' log level constant (verbose logging). </summary>
		Public Shared final Integer VERBOSE = 20

		''' <summary>
		''' Convert a string name of a logging level to its internal
		''' integer representation. </summary>
		''' <param name="s"> name of logging level (e.g., 'SILENT', 'BRIEF', 'VERBOSE') </param>
		''' <returns> corresponding integer log level
		''' @since JDK1.1 </returns>
		''' @deprecated no replacement 
		<Obsolete("no replacement")> _
		Public Shared Integer parseLevel(String s)
			If (s Is Nothing) OrElse (s.length() < 1) Then Return -1

			Try
				Return Convert.ToInt32(s)
			Catch e As NumberFormatException
			End Try
			If s.length() < 1 Then Return -1

			If "SILENT".StartsWith(s.ToUpper()) Then
				Return SILENT
			ElseIf "BRIEF".StartsWith(s.ToUpper()) Then
				Return BRIEF
			ElseIf "VERBOSE".StartsWith(s.ToUpper()) Then
				Return VERBOSE
			End If

			Return -1
	End Class

End Namespace