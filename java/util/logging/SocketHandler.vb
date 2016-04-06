Imports System
Imports System.Runtime.CompilerServices

'
' * Copyright (c) 2000, 2013, Oracle and/or its affiliates. All rights reserved.
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


Namespace java.util.logging


	''' <summary>
	''' Simple network logging <tt>Handler</tt>.
	''' <p>
	''' <tt>LogRecords</tt> are published to a network stream connection.  By default
	''' the <tt>XMLFormatter</tt> class is used for formatting.
	''' <p>
	''' <b>Configuration:</b>
	''' By default each <tt>SocketHandler</tt> is initialized using the following
	''' <tt>LogManager</tt> configuration properties where <tt>&lt;handler-name&gt;</tt>
	''' refers to the fully-qualified class name of the handler.
	''' If properties are not defined
	''' (or have invalid values) then the specified default values are used.
	''' <ul>
	''' <li>   &lt;handler-name&gt;.level
	'''        specifies the default level for the <tt>Handler</tt>
	'''        (defaults to <tt>Level.ALL</tt>). </li>
	''' <li>   &lt;handler-name&gt;.filter
	'''        specifies the name of a <tt>Filter</tt> class to use
	'''        (defaults to no <tt>Filter</tt>). </li>
	''' <li>   &lt;handler-name&gt;.formatter
	'''        specifies the name of a <tt>Formatter</tt> class to use
	'''        (defaults to <tt>java.util.logging.XMLFormatter</tt>). </li>
	''' <li>   &lt;handler-name&gt;.encoding
	'''        the name of the character set encoding to use (defaults to
	'''        the default platform encoding). </li>
	''' <li>   &lt;handler-name&gt;.host
	'''        specifies the target host name to connect to (no default). </li>
	''' <li>   &lt;handler-name&gt;.port
	'''        specifies the target TCP port to use (no default). </li>
	''' </ul>
	''' <p>
	''' For example, the properties for {@code SocketHandler} would be:
	''' <ul>
	''' <li>   java.util.logging.SocketHandler.level=INFO </li>
	''' <li>   java.util.logging.SocketHandler.formatter=java.util.logging.SimpleFormatter </li>
	''' </ul>
	''' <p>
	''' For a custom handler, e.g. com.foo.MyHandler, the properties would be:
	''' <ul>
	''' <li>   com.foo.MyHandler.level=INFO </li>
	''' <li>   com.foo.MyHandler.formatter=java.util.logging.SimpleFormatter </li>
	''' </ul>
	''' <p>
	''' The output IO stream is buffered, but is flushed after each
	''' <tt>LogRecord</tt> is written.
	''' 
	''' @since 1.4
	''' </summary>

	Public Class SocketHandler
		Inherits StreamHandler

		Private sock As Socket
		Private host As String
		Private port As Integer

		' Private method to configure a SocketHandler from LogManager
		' properties and/or default values as specified in the class
		' javadoc.
		Private Sub configure()
			Dim manager As LogManager = LogManager.logManager
			Dim cname As String = Me.GetType().name

			level = manager.getLevelProperty(cname & ".level", Level.ALL)
			filter = manager.getFilterProperty(cname & ".filter", Nothing)
			formatter = manager.getFormatterProperty(cname & ".formatter", New XMLFormatter)
			Try
				encoding = manager.getStringProperty(cname & ".encoding", Nothing)
			Catch ex As Exception
				Try
					encoding = Nothing
				Catch ex2 As Exception
					' doing a setEncoding with null should always work.
					' assert false;
				End Try
			End Try
			port = manager.getIntProperty(cname & ".port", 0)
			host = manager.getStringProperty(cname & ".host", Nothing)
		End Sub


		''' <summary>
		''' Create a <tt>SocketHandler</tt>, using only <tt>LogManager</tt> properties
		''' (or their defaults). </summary>
		''' <exception cref="IllegalArgumentException"> if the host or port are invalid or
		'''          are not specified as LogManager properties. </exception>
		''' <exception cref="IOException"> if we are unable to connect to the target
		'''         host and port. </exception>
		Public Sub New()
			' We are going to use the logging defaults.
			sealed = False
			configure()

			Try
				connect()
			Catch ix As IOException
				Console.Error.WriteLine("SocketHandler: connect failed to " & host & ":" & port)
				Throw ix
			End Try
			sealed = True
		End Sub

		''' <summary>
		''' Construct a <tt>SocketHandler</tt> using a specified host and port.
		''' 
		''' The <tt>SocketHandler</tt> is configured based on <tt>LogManager</tt>
		''' properties (or their default values) except that the given target host
		''' and port arguments are used. If the host argument is empty, but not
		''' null String then the localhost is used.
		''' </summary>
		''' <param name="host"> target host. </param>
		''' <param name="port"> target port.
		''' </param>
		''' <exception cref="IllegalArgumentException"> if the host or port are invalid. </exception>
		''' <exception cref="IOException"> if we are unable to connect to the target
		'''         host and port. </exception>
		Public Sub New(  host As String,   port As Integer)
			sealed = False
			configure()
			sealed = True
			Me.port = port
			Me.host = host
			connect()
		End Sub

		Private Sub connect()
			' Check the arguments are valid.
			If port = 0 Then Throw New IllegalArgumentException("Bad port: " & port)
			If host Is Nothing Then Throw New IllegalArgumentException("Null host name: " & host)

			' Try to open a new socket.
			sock = New Socket(host, port)
			Dim out As OutputStream = sock.outputStream
			Dim bout As New BufferedOutputStream(out)
			outputStream = bout
		End Sub

		''' <summary>
		''' Close this output stream.
		''' </summary>
		''' <exception cref="SecurityException">  if a security manager exists and if
		'''             the caller does not have <tt>LoggingPermission("control")</tt>. </exception>
		<MethodImpl(MethodImplOptions.Synchronized)> _
		Public Overrides Sub close()
			MyBase.close()
			If sock IsNot Nothing Then
				Try
					sock.close()
				Catch ix As IOException
					' drop through.
				End Try
			End If
			sock = Nothing
		End Sub

		''' <summary>
		''' Format and publish a <tt>LogRecord</tt>.
		''' </summary>
		''' <param name="record">  description of the log event. A null record is
		'''                 silently ignored and is not published </param>
		<MethodImpl(MethodImplOptions.Synchronized)> _
		Public Overrides Sub publish(  record As LogRecord)
			If Not isLoggable(record) Then Return
			MyBase.publish(record)
			flush()
		End Sub
	End Class

End Namespace