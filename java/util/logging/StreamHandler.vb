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
	''' Stream based logging <tt>Handler</tt>.
	''' <p>
	''' This is primarily intended as a base class or support class to
	''' be used in implementing other logging <tt>Handlers</tt>.
	''' <p>
	''' <tt>LogRecords</tt> are published to a given <tt>java.io.OutputStream</tt>.
	''' <p>
	''' <b>Configuration:</b>
	''' By default each <tt>StreamHandler</tt> is initialized using the following
	''' <tt>LogManager</tt> configuration properties where <tt>&lt;handler-name&gt;</tt>
	''' refers to the fully-qualified class name of the handler.
	''' If properties are not defined
	''' (or have invalid values) then the specified default values are used.
	''' <ul>
	''' <li>   &lt;handler-name&gt;.level
	'''        specifies the default level for the <tt>Handler</tt>
	'''        (defaults to <tt>Level.INFO</tt>). </li>
	''' <li>   &lt;handler-name&gt;.filter
	'''        specifies the name of a <tt>Filter</tt> class to use
	'''         (defaults to no <tt>Filter</tt>). </li>
	''' <li>   &lt;handler-name&gt;.formatter
	'''        specifies the name of a <tt>Formatter</tt> class to use
	'''        (defaults to <tt>java.util.logging.SimpleFormatter</tt>). </li>
	''' <li>   &lt;handler-name&gt;.encoding
	'''        the name of the character set encoding to use (defaults to
	'''        the default platform encoding). </li>
	''' </ul>
	''' <p>
	''' For example, the properties for {@code StreamHandler} would be:
	''' <ul>
	''' <li>   java.util.logging.StreamHandler.level=INFO </li>
	''' <li>   java.util.logging.StreamHandler.formatter=java.util.logging.SimpleFormatter </li>
	''' </ul>
	''' <p>
	''' For a custom handler, e.g. com.foo.MyHandler, the properties would be:
	''' <ul>
	''' <li>   com.foo.MyHandler.level=INFO </li>
	''' <li>   com.foo.MyHandler.formatter=java.util.logging.SimpleFormatter </li>
	''' </ul>
	''' <p>
	''' @since 1.4
	''' </summary>

	Public Class StreamHandler
		Inherits Handler

		Private output As OutputStream
		Private doneHeader As Boolean
'JAVA TO VB CONVERTER TODO TASK: There is no VB equivalent to 'volatile':
		Private writer As Writer

		' Private method to configure a StreamHandler from LogManager
		' properties and/or default values as specified in the class
		' javadoc.
		Private Sub configure()
			Dim manager As LogManager = LogManager.logManager
			Dim cname As String = Me.GetType().name

			level = manager.getLevelProperty(cname & ".level", Level.INFO)
			filter = manager.getFilterProperty(cname & ".filter", Nothing)
			formatter = manager.getFormatterProperty(cname & ".formatter", New SimpleFormatter)
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
		End Sub

		''' <summary>
		''' Create a <tt>StreamHandler</tt>, with no current output stream.
		''' </summary>
		Public Sub New()
			sealed = False
			configure()
			sealed = True
		End Sub

		''' <summary>
		''' Create a <tt>StreamHandler</tt> with a given <tt>Formatter</tt>
		''' and output stream.
		''' <p> </summary>
		''' <param name="out">         the target output stream </param>
		''' <param name="formatter">   Formatter to be used to format output </param>
		Public Sub New(ByVal out As OutputStream, ByVal formatter As Formatter)
			sealed = False
			configure()
			formatter = formatter
			outputStream = out
			sealed = True
		End Sub

		''' <summary>
		''' Change the output stream.
		''' <P>
		''' If there is a current output stream then the <tt>Formatter</tt>'s
		''' tail string is written and the stream is flushed and closed.
		''' Then the output stream is replaced with the new output stream.
		''' </summary>
		''' <param name="out">   New output stream.  May not be null. </param>
		''' <exception cref="SecurityException">  if a security manager exists and if
		'''             the caller does not have <tt>LoggingPermission("control")</tt>. </exception>
		<MethodImpl(MethodImplOptions.Synchronized)> _
		Protected Friend Overridable Property outputStream As OutputStream
			Set(ByVal out As OutputStream)
				If out Is Nothing Then Throw New NullPointerException
				flushAndClose()
				output = out
				doneHeader = False
				Dim encoding_Renamed As String = encoding
				If encoding_Renamed Is Nothing Then
					writer = New OutputStreamWriter(output)
				Else
					Try
						writer = New OutputStreamWriter(output, encoding_Renamed)
					Catch ex As UnsupportedEncodingException
						' This shouldn't happen.  The setEncoding method
						' should have validated that the encoding is OK.
						Throw New [Error]("Unexpected exception " & ex)
					End Try
				End If
			End Set
		End Property

		''' <summary>
		''' Set (or change) the character encoding used by this <tt>Handler</tt>.
		''' <p>
		''' The encoding should be set before any <tt>LogRecords</tt> are written
		''' to the <tt>Handler</tt>.
		''' </summary>
		''' <param name="encoding">  The name of a supported character encoding.
		'''        May be null, to indicate the default platform encoding. </param>
		''' <exception cref="SecurityException">  if a security manager exists and if
		'''             the caller does not have <tt>LoggingPermission("control")</tt>. </exception>
		''' <exception cref="UnsupportedEncodingException"> if the named encoding is
		'''          not supported. </exception>
		<MethodImpl(MethodImplOptions.Synchronized)> _
		Public Overrides Property encoding As String
			Set(ByVal encoding As String)
				MyBase.encoding = encoding
				If output Is Nothing Then Return
				' Replace the current writer with a writer for the new encoding.
				flush()
				If encoding Is Nothing Then
					writer = New OutputStreamWriter(output)
				Else
					writer = New OutputStreamWriter(output, encoding)
				End If
			End Set
		End Property

		''' <summary>
		''' Format and publish a <tt>LogRecord</tt>.
		''' <p>
		''' The <tt>StreamHandler</tt> first checks if there is an <tt>OutputStream</tt>
		''' and if the given <tt>LogRecord</tt> has at least the required log level.
		''' If not it silently returns.  If so, it calls any associated
		''' <tt>Filter</tt> to check if the record should be published.  If so,
		''' it calls its <tt>Formatter</tt> to format the record and then writes
		''' the result to the current output stream.
		''' <p>
		''' If this is the first <tt>LogRecord</tt> to be written to a given
		''' <tt>OutputStream</tt>, the <tt>Formatter</tt>'s "head" string is
		''' written to the stream before the <tt>LogRecord</tt> is written.
		''' </summary>
		''' <param name="record">  description of the log event. A null record is
		'''                 silently ignored and is not published </param>
		<MethodImpl(MethodImplOptions.Synchronized)> _
		Public Overrides Sub publish(ByVal record As LogRecord)
			If Not isLoggable(record) Then Return
			Dim msg As String
			Try
				msg = formatter.format(record)
			Catch ex As Exception
				' We don't want to throw an exception here, but we
				' report the exception to any registered ErrorManager.
				reportError(Nothing, ex, ErrorManager.FORMAT_FAILURE)
				Return
			End Try

			Try
				If Not doneHeader Then
					writer.write(formatter.getHead(Me))
					doneHeader = True
				End If
				writer.write(msg)
			Catch ex As Exception
				' We don't want to throw an exception here, but we
				' report the exception to any registered ErrorManager.
				reportError(Nothing, ex, ErrorManager.WRITE_FAILURE)
			End Try
		End Sub


		''' <summary>
		''' Check if this <tt>Handler</tt> would actually log a given <tt>LogRecord</tt>.
		''' <p>
		''' This method checks if the <tt>LogRecord</tt> has an appropriate level and
		''' whether it satisfies any <tt>Filter</tt>.  It will also return false if
		''' no output stream has been assigned yet or the LogRecord is null.
		''' <p> </summary>
		''' <param name="record">  a <tt>LogRecord</tt> </param>
		''' <returns> true if the <tt>LogRecord</tt> would be logged.
		'''  </returns>
		Public Overrides Function isLoggable(ByVal record As LogRecord) As Boolean
			If writer Is Nothing OrElse record Is Nothing Then Return False
			Return MyBase.isLoggable(record)
		End Function

		''' <summary>
		''' Flush any buffered messages.
		''' </summary>
		<MethodImpl(MethodImplOptions.Synchronized)> _
		Public Overrides Sub flush()
			If writer IsNot Nothing Then
				Try
					writer.flush()
				Catch ex As Exception
					' We don't want to throw an exception here, but we
					' report the exception to any registered ErrorManager.
					reportError(Nothing, ex, ErrorManager.FLUSH_FAILURE)
				End Try
			End If
		End Sub

		<MethodImpl(MethodImplOptions.Synchronized)> _
		Private Sub flushAndClose()
			checkPermission()
			If writer IsNot Nothing Then
				Try
					If Not doneHeader Then
						writer.write(formatter.getHead(Me))
						doneHeader = True
					End If
					writer.write(formatter.getTail(Me))
					writer.flush()
					writer.close()
				Catch ex As Exception
					' We don't want to throw an exception here, but we
					' report the exception to any registered ErrorManager.
					reportError(Nothing, ex, ErrorManager.CLOSE_FAILURE)
				End Try
				writer = Nothing
				output = Nothing
			End If
		End Sub

		''' <summary>
		''' Close the current output stream.
		''' <p>
		''' The <tt>Formatter</tt>'s "tail" string is written to the stream before it
		''' is closed.  In addition, if the <tt>Formatter</tt>'s "head" string has not
		''' yet been written to the stream, it will be written before the
		''' "tail" string.
		''' </summary>
		''' <exception cref="SecurityException">  if a security manager exists and if
		'''             the caller does not have LoggingPermission("control"). </exception>
		<MethodImpl(MethodImplOptions.Synchronized)> _
		Public Overrides Sub close()
			flushAndClose()
		End Sub
	End Class

End Namespace