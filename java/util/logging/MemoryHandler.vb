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
	''' <tt>Handler</tt> that buffers requests in a circular buffer in memory.
	''' <p>
	''' Normally this <tt>Handler</tt> simply stores incoming <tt>LogRecords</tt>
	''' into its memory buffer and discards earlier records.  This buffering
	''' is very cheap and avoids formatting costs.  On certain trigger
	''' conditions, the <tt>MemoryHandler</tt> will push out its current buffer
	''' contents to a target <tt>Handler</tt>, which will typically publish
	''' them to the outside world.
	''' <p>
	''' There are three main models for triggering a push of the buffer:
	''' <ul>
	''' <li>
	''' An incoming <tt>LogRecord</tt> has a type that is greater than
	''' a pre-defined level, the <tt>pushLevel</tt>. </li>
	''' <li>
	''' An external class calls the <tt>push</tt> method explicitly. </li>
	''' <li>
	''' A subclass overrides the <tt>log</tt> method and scans each incoming
	''' <tt>LogRecord</tt> and calls <tt>push</tt> if a record matches some
	''' desired criteria. </li>
	''' </ul>
	''' <p>
	''' <b>Configuration:</b>
	''' By default each <tt>MemoryHandler</tt> is initialized using the following
	''' <tt>LogManager</tt> configuration properties where <tt>&lt;handler-name&gt;</tt>
	''' refers to the fully-qualified class name of the handler.
	''' If properties are not defined
	''' (or have invalid values) then the specified default values are used.
	''' If no default value is defined then a RuntimeException is thrown.
	''' <ul>
	''' <li>   &lt;handler-name&gt;.level
	'''        specifies the level for the <tt>Handler</tt>
	'''        (defaults to <tt>Level.ALL</tt>). </li>
	''' <li>   &lt;handler-name&gt;.filter
	'''        specifies the name of a <tt>Filter</tt> class to use
	'''        (defaults to no <tt>Filter</tt>). </li>
	''' <li>   &lt;handler-name&gt;.size
	'''        defines the buffer size (defaults to 1000). </li>
	''' <li>   &lt;handler-name&gt;.push
	'''        defines the <tt>pushLevel</tt> (defaults to <tt>level.SEVERE</tt>). </li>
	''' <li>   &lt;handler-name&gt;.target
	'''        specifies the name of the target <tt>Handler </tt> class.
	'''        (no default). </li>
	''' </ul>
	''' <p>
	''' For example, the properties for {@code MemoryHandler} would be:
	''' <ul>
	''' <li>   java.util.logging.MemoryHandler.level=INFO </li>
	''' <li>   java.util.logging.MemoryHandler.formatter=java.util.logging.SimpleFormatter </li>
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

	Public Class MemoryHandler
		Inherits Handler

		Private Const DEFAULT_SIZE As Integer = 1000
'JAVA TO VB CONVERTER TODO TASK: There is no VB equivalent to 'volatile':
		Private pushLevel As Level
		Private size As Integer
		Private target As Handler
		Private buffer As LogRecord()
		Friend start, count As Integer

		' Private method to configure a MemoryHandler from LogManager
		' properties and/or default values as specified in the class
		' javadoc.
		Private Sub configure()
			Dim manager As LogManager = LogManager.logManager
			Dim cname As String = Me.GetType().name

			pushLevel = manager.getLevelProperty(cname & ".push", Level.SEVERE)
			size = manager.getIntProperty(cname & ".size", DEFAULT_SIZE)
			If size <= 0 Then size = DEFAULT_SIZE
			level = manager.getLevelProperty(cname & ".level", Level.ALL)
			filter = manager.getFilterProperty(cname & ".filter", Nothing)
			formatter = manager.getFormatterProperty(cname & ".formatter", New SimpleFormatter)
		End Sub

		''' <summary>
		''' Create a <tt>MemoryHandler</tt> and configure it based on
		''' <tt>LogManager</tt> configuration properties.
		''' </summary>
		Public Sub New()
			sealed = False
			configure()
			sealed = True

			Dim manager As LogManager = LogManager.logManager
			Dim handlerName As String = Me.GetType().name
			Dim targetName As String = manager.getProperty(handlerName & ".target")
			If targetName Is Nothing Then Throw New RuntimeException("The handler " & handlerName & " does not specify a target")
			Dim clz As  [Class]
			Try
				clz = ClassLoader.systemClassLoader.loadClass(targetName)
				target = CType(clz.newInstance(), Handler)
'JAVA TO VB CONVERTER TODO TASK: There is no equivalent in VB to Java 'multi-catch' syntax:
			Catch ClassNotFoundException Or InstantiationException Or IllegalAccessException e
				Throw New RuntimeException("MemoryHandler can't load handler target """ & targetName & """", e)
			End Try
			init()
		End Sub

		' Initialize.  Size is a count of LogRecords.
		Private Sub init()
			buffer = New LogRecord(size - 1){}
			start = 0
			count = 0
		End Sub

		''' <summary>
		''' Create a <tt>MemoryHandler</tt>.
		''' <p>
		''' The <tt>MemoryHandler</tt> is configured based on <tt>LogManager</tt>
		''' properties (or their default values) except that the given <tt>pushLevel</tt>
		''' argument and buffer size argument are used.
		''' </summary>
		''' <param name="target">  the Handler to which to publish output. </param>
		''' <param name="size">    the number of log records to buffer (must be greater than zero) </param>
		''' <param name="pushLevel">  message level to push on
		''' </param>
		''' <exception cref="IllegalArgumentException"> if {@code size is <= 0} </exception>
		Public Sub New(ByVal target As Handler, ByVal size As Integer, ByVal pushLevel As Level)
			If target Is Nothing OrElse pushLevel Is Nothing Then Throw New NullPointerException
			If size <= 0 Then Throw New IllegalArgumentException
			sealed = False
			configure()
			sealed = True
			Me.target = target
			Me.pushLevel = pushLevel
			Me.size = size
			init()
		End Sub

		''' <summary>
		''' Store a <tt>LogRecord</tt> in an internal buffer.
		''' <p>
		''' If there is a <tt>Filter</tt>, its <tt>isLoggable</tt>
		''' method is called to check if the given log record is loggable.
		''' If not we return.  Otherwise the given record is copied into
		''' an internal circular buffer.  Then the record's level property is
		''' compared with the <tt>pushLevel</tt>. If the given level is
		''' greater than or equal to the <tt>pushLevel</tt> then <tt>push</tt>
		''' is called to write all buffered records to the target output
		''' <tt>Handler</tt>.
		''' </summary>
		''' <param name="record">  description of the log event. A null record is
		'''                 silently ignored and is not published </param>
		<MethodImpl(MethodImplOptions.Synchronized)> _
		Public Overrides Sub publish(ByVal record As LogRecord)
			If Not isLoggable(record) Then Return
			Dim ix As Integer = (start+count) Mod buffer.Length
			buffer(ix) = record
			If count < buffer.Length Then
				count += 1
			Else
				start += 1
				start = start Mod buffer.Length
			End If
			If record.level >= pushLevel Then push()
		End Sub

		''' <summary>
		''' Push any buffered output to the target <tt>Handler</tt>.
		''' <p>
		''' The buffer is then cleared.
		''' </summary>
		<MethodImpl(MethodImplOptions.Synchronized)> _
		Public Overridable Sub push()
			For i As Integer = 0 To count - 1
				Dim ix As Integer = (start+i) Mod buffer.Length
				Dim record As LogRecord = buffer(ix)
				target.publish(record)
			Next i
			' Empty the buffer.
			start = 0
			count = 0
		End Sub

		''' <summary>
		''' Causes a flush on the target <tt>Handler</tt>.
		''' <p>
		''' Note that the current contents of the <tt>MemoryHandler</tt>
		''' buffer are <b>not</b> written out.  That requires a "push".
		''' </summary>
		Public Overrides Sub flush()
			target.flush()
		End Sub

		''' <summary>
		''' Close the <tt>Handler</tt> and free all associated resources.
		''' This will also close the target <tt>Handler</tt>.
		''' </summary>
		''' <exception cref="SecurityException">  if a security manager exists and if
		'''             the caller does not have <tt>LoggingPermission("control")</tt>. </exception>
		Public Overrides Sub close()
			target.close()
			level = Level.OFF
		End Sub

		''' <summary>
		''' Set the <tt>pushLevel</tt>.  After a <tt>LogRecord</tt> is copied
		''' into our internal buffer, if its level is greater than or equal to
		''' the <tt>pushLevel</tt>, then <tt>push</tt> will be called.
		''' </summary>
		''' <param name="newLevel"> the new value of the <tt>pushLevel</tt> </param>
		''' <exception cref="SecurityException">  if a security manager exists and if
		'''             the caller does not have <tt>LoggingPermission("control")</tt>. </exception>
		<MethodImpl(MethodImplOptions.Synchronized)> _
		Public Overridable Property pushLevel As Level
			Set(ByVal newLevel As Level)
				If newLevel Is Nothing Then Throw New NullPointerException
				checkPermission()
				pushLevel = newLevel
			End Set
			Get
				Return pushLevel
			End Get
		End Property


		''' <summary>
		''' Check if this <tt>Handler</tt> would actually log a given
		''' <tt>LogRecord</tt> into its internal buffer.
		''' <p>
		''' This method checks if the <tt>LogRecord</tt> has an appropriate level and
		''' whether it satisfies any <tt>Filter</tt>.  However it does <b>not</b>
		''' check whether the <tt>LogRecord</tt> would result in a "push" of the
		''' buffer contents. It will return false if the <tt>LogRecord</tt> is null.
		''' <p> </summary>
		''' <param name="record">  a <tt>LogRecord</tt> </param>
		''' <returns> true if the <tt>LogRecord</tt> would be logged.
		'''  </returns>
		Public Overrides Function isLoggable(ByVal record As LogRecord) As Boolean
			Return MyBase.isLoggable(record)
		End Function
	End Class

End Namespace