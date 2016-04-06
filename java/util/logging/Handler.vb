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
	''' A <tt>Handler</tt> object takes log messages from a <tt>Logger</tt> and
	''' exports them.  It might for example, write them to a console
	''' or write them to a file, or send them to a network logging service,
	''' or forward them to an OS log, or whatever.
	''' <p>
	''' A <tt>Handler</tt> can be disabled by doing a <tt>setLevel(Level.OFF)</tt>
	''' and can  be re-enabled by doing a <tt>setLevel</tt> with an appropriate level.
	''' <p>
	''' <tt>Handler</tt> classes typically use <tt>LogManager</tt> properties to set
	''' default values for the <tt>Handler</tt>'s <tt>Filter</tt>, <tt>Formatter</tt>,
	''' and <tt>Level</tt>.  See the specific documentation for each concrete
	''' <tt>Handler</tt> class.
	''' 
	''' 
	''' @since 1.4
	''' </summary>

	Public MustInherit Class Handler
		Private Shared ReadOnly offValue As Integer = Level.OFF
		Private ReadOnly manager As LogManager = LogManager.logManager

		' We're using volatile here to avoid synchronizing getters, which
		' would prevent other threads from calling isLoggable()
		' while publish() is executing.
		' On the other hand, setters will be synchronized to exclude concurrent
		' execution with more complex methods, such as StreamHandler.publish().
		' We wouldn't want 'level' to be changed by another thread in the middle
		' of the execution of a 'publish' call.
'JAVA TO VB CONVERTER TODO TASK: There is no VB equivalent to 'volatile':
		Private filter As Filter
'JAVA TO VB CONVERTER TODO TASK: There is no VB equivalent to 'volatile':
		Private formatter As Formatter
'JAVA TO VB CONVERTER TODO TASK: There is no VB equivalent to 'volatile':
		Private logLevel As Level = Level.ALL
'JAVA TO VB CONVERTER TODO TASK: There is no VB equivalent to 'volatile':
		Private errorManager_Renamed As New ErrorManager
'JAVA TO VB CONVERTER TODO TASK: There is no VB equivalent to 'volatile':
		Private encoding As String

		' Package private support for security checking.  When sealed
		' is true, we access check updates to the class.
		Friend sealed As Boolean = True

		''' <summary>
		''' Default constructor.  The resulting <tt>Handler</tt> has a log
		''' level of <tt>Level.ALL</tt>, no <tt>Formatter</tt>, and no
		''' <tt>Filter</tt>.  A default <tt>ErrorManager</tt> instance is installed
		''' as the <tt>ErrorManager</tt>.
		''' </summary>
		Protected Friend Sub New()
		End Sub

		''' <summary>
		''' Publish a <tt>LogRecord</tt>.
		''' <p>
		''' The logging request was made initially to a <tt>Logger</tt> object,
		''' which initialized the <tt>LogRecord</tt> and forwarded it here.
		''' <p>
		''' The <tt>Handler</tt>  is responsible for formatting the message, when and
		''' if necessary.  The formatting should include localization.
		''' </summary>
		''' <param name="record">  description of the log event. A null record is
		'''                 silently ignored and is not published </param>
		Public MustOverride Sub publish(  record As LogRecord)

		''' <summary>
		''' Flush any buffered output.
		''' </summary>
		Public MustOverride Sub flush()

		''' <summary>
		''' Close the <tt>Handler</tt> and free all associated resources.
		''' <p>
		''' The close method will perform a <tt>flush</tt> and then close the
		''' <tt>Handler</tt>.   After close has been called this <tt>Handler</tt>
		''' should no longer be used.  Method calls may either be silently
		''' ignored or may throw runtime exceptions.
		''' </summary>
		''' <exception cref="SecurityException">  if a security manager exists and if
		'''             the caller does not have <tt>LoggingPermission("control")</tt>. </exception>
		Public MustOverride Sub close()

		''' <summary>
		''' Set a <tt>Formatter</tt>.  This <tt>Formatter</tt> will be used
		''' to format <tt>LogRecords</tt> for this <tt>Handler</tt>.
		''' <p>
		''' Some <tt>Handlers</tt> may not use <tt>Formatters</tt>, in
		''' which case the <tt>Formatter</tt> will be remembered, but not used.
		''' <p> </summary>
		''' <param name="newFormatter"> the <tt>Formatter</tt> to use (may not be null) </param>
		''' <exception cref="SecurityException">  if a security manager exists and if
		'''             the caller does not have <tt>LoggingPermission("control")</tt>. </exception>
		<MethodImpl(MethodImplOptions.Synchronized)> _
		Public Overridable Property formatter As Formatter
			Set(  newFormatter As Formatter)
				checkPermission()
				' Check for a null pointer:
				newFormatter.GetType()
				formatter = newFormatter
			End Set
			Get
				Return formatter
			End Get
		End Property


		''' <summary>
		''' Set the character encoding used by this <tt>Handler</tt>.
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
		Public Overridable Property encoding As String
			Set(  encoding As String)
				checkPermission()
				If encoding IsNot Nothing Then
					Try
						If Not java.nio.charset.Charset.isSupported(encoding) Then Throw New java.io.UnsupportedEncodingException(encoding)
					Catch e As java.nio.charset.IllegalCharsetNameException
						Throw New java.io.UnsupportedEncodingException(encoding)
					End Try
				End If
				Me.encoding = encoding
			End Set
			Get
				Return encoding
			End Get
		End Property


		''' <summary>
		''' Set a <tt>Filter</tt> to control output on this <tt>Handler</tt>.
		''' <P>
		''' For each call of <tt>publish</tt> the <tt>Handler</tt> will call
		''' this <tt>Filter</tt> (if it is non-null) to check if the
		''' <tt>LogRecord</tt> should be published or discarded.
		''' </summary>
		''' <param name="newFilter">  a <tt>Filter</tt> object (may be null) </param>
		''' <exception cref="SecurityException">  if a security manager exists and if
		'''             the caller does not have <tt>LoggingPermission("control")</tt>. </exception>
		<MethodImpl(MethodImplOptions.Synchronized)> _
		Public Overridable Property filter As Filter
			Set(  newFilter As Filter)
				checkPermission()
				filter = newFilter
			End Set
			Get
				Return filter
			End Get
		End Property


		''' <summary>
		''' Define an ErrorManager for this Handler.
		''' <p>
		''' The ErrorManager's "error" method will be invoked if any
		''' errors occur while using this Handler.
		''' </summary>
		''' <param name="em">  the new ErrorManager </param>
		''' <exception cref="SecurityException">  if a security manager exists and if
		'''             the caller does not have <tt>LoggingPermission("control")</tt>. </exception>
		<MethodImpl(MethodImplOptions.Synchronized)> _
		Public Overridable Property errorManager As ErrorManager
			Set(  em As ErrorManager)
				checkPermission()
				If em Is Nothing Then Throw New NullPointerException
				errorManager_Renamed = em
			End Set
			Get
				checkPermission()
				Return errorManager_Renamed
			End Get
		End Property


	   ''' <summary>
	   ''' Protected convenience method to report an error to this Handler's
	   ''' ErrorManager.  Note that this method retrieves and uses the ErrorManager
	   ''' without doing a security check.  It can therefore be used in
	   ''' environments where the caller may be non-privileged.
	   ''' </summary>
	   ''' <param name="msg">    a descriptive string (may be null) </param>
	   ''' <param name="ex">     an exception (may be null) </param>
	   ''' <param name="code">   an error code defined in ErrorManager </param>
		Protected Friend Overridable Sub reportError(  msg As String,   ex As Exception,   code As Integer)
			Try
				errorManager_Renamed.error(msg, ex, code)
			Catch ex2 As Exception
				Console.Error.WriteLine("Handler.reportError caught:")
				ex2.printStackTrace()
			End Try
		End Sub

		''' <summary>
		''' Set the log level specifying which message levels will be
		''' logged by this <tt>Handler</tt>.  Message levels lower than this
		''' value will be discarded.
		''' <p>
		''' The intention is to allow developers to turn on voluminous
		''' logging, but to limit the messages that are sent to certain
		''' <tt>Handlers</tt>.
		''' </summary>
		''' <param name="newLevel">   the new value for the log level </param>
		''' <exception cref="SecurityException">  if a security manager exists and if
		'''             the caller does not have <tt>LoggingPermission("control")</tt>. </exception>
		<MethodImpl(MethodImplOptions.Synchronized)> _
		Public Overridable Property level As Level
			Set(  newLevel As Level)
				If newLevel Is Nothing Then Throw New NullPointerException
				checkPermission()
				logLevel = newLevel
			End Set
			Get
				Return logLevel
			End Get
		End Property


		''' <summary>
		''' Check if this <tt>Handler</tt> would actually log a given <tt>LogRecord</tt>.
		''' <p>
		''' This method checks if the <tt>LogRecord</tt> has an appropriate
		''' <tt>Level</tt> and  whether it satisfies any <tt>Filter</tt>.  It also
		''' may make other <tt>Handler</tt> specific checks that might prevent a
		''' handler from logging the <tt>LogRecord</tt>. It will return false if
		''' the <tt>LogRecord</tt> is null.
		''' <p> </summary>
		''' <param name="record">  a <tt>LogRecord</tt> </param>
		''' <returns> true if the <tt>LogRecord</tt> would be logged.
		'''  </returns>
		Public Overridable Function isLoggable(  record As LogRecord) As Boolean
			Dim levelValue As Integer = level
			If record.level < levelValue OrElse levelValue = offValue Then Return False
			Dim filter_Renamed As Filter = filter
			If filter_Renamed Is Nothing Then Return True
			Return filter_Renamed.isLoggable(record)
		End Function

		' Package-private support method for security checks.
		' If "sealed" is true, we check that the caller has
		' appropriate security privileges to update Handler
		' state and if not throw a SecurityException.
		Friend Overridable Sub checkPermission()
			If sealed Then manager.checkPermission()
		End Sub
	End Class

End Namespace