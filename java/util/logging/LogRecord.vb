Imports System
Imports System.Threading

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
	''' LogRecord objects are used to pass logging requests between
	''' the logging framework and individual log Handlers.
	''' <p>
	''' When a LogRecord is passed into the logging framework it
	''' logically belongs to the framework and should no longer be
	''' used or updated by the client application.
	''' <p>
	''' Note that if the client application has not specified an
	''' explicit source method name and source class name, then the
	''' LogRecord class will infer them automatically when they are
	''' first accessed (due to a call on getSourceMethodName or
	''' getSourceClassName) by analyzing the call stack.  Therefore,
	''' if a logging Handler wants to pass off a LogRecord to another
	''' thread, or to transmit it over RMI, and if it wishes to subsequently
	''' obtain method name or class name information it should call
	''' one of getSourceClassName or getSourceMethodName to force
	''' the values to be filled in.
	''' <p>
	''' <b> Serialization notes:</b>
	''' <ul>
	''' <li>The LogRecord class is serializable.
	''' 
	''' <li> Because objects in the parameters array may not be serializable,
	''' during serialization all objects in the parameters array are
	''' written as the corresponding Strings (using Object.toString).
	''' 
	''' <li> The ResourceBundle is not transmitted as part of the serialized
	''' form, but the resource bundle name is, and the recipient object's
	''' readObject method will attempt to locate a suitable resource bundle.
	''' 
	''' </ul>
	''' 
	''' @since 1.4
	''' </summary>

	<Serializable> _
	Public Class LogRecord
		Private Shared ReadOnly globalSequenceNumber As New java.util.concurrent.atomic.AtomicLong(0)

		''' <summary>
		''' The default value of threadID will be the current thread's
		''' thread id, for ease of correlation, unless it is greater than
		''' MIN_SEQUENTIAL_THREAD_ID, in which case we try harder to keep
		''' our promise to keep threadIDs unique by avoiding collisions due
		''' to 32-bit wraparound.  Unfortunately, LogRecord.getThreadID()
		''' returns int, while Thread.getId() returns java.lang.[Long].
		''' </summary>
		Private Shared ReadOnly MIN_SEQUENTIAL_THREAD_ID As Integer =  java.lang.[Integer].MAX_VALUE / 2

		Private Shared ReadOnly nextThreadId As New java.util.concurrent.atomic.AtomicInteger(MIN_SEQUENTIAL_THREAD_ID)

		Private Shared ReadOnly threadIds As New ThreadLocal(Of Integer?)

		''' <summary>
		''' @serial Logging message level
		''' </summary>
		Private level_Renamed As Level

		''' <summary>
		''' @serial Sequence number
		''' </summary>
		Private sequenceNumber As Long

		''' <summary>
		''' @serial Class that issued logging call
		''' </summary>
		Private sourceClassName As String

		''' <summary>
		''' @serial Method that issued logging call
		''' </summary>
		Private sourceMethodName As String

		''' <summary>
		''' @serial Non-localized raw message text
		''' </summary>
		Private message As String

		''' <summary>
		''' @serial Thread ID for thread that issued logging call.
		''' </summary>
		Private threadID As Integer

		''' <summary>
		''' @serial Event time in milliseconds since 1970
		''' </summary>
		Private millis As Long

		''' <summary>
		''' @serial The Throwable (if any) associated with log message
		''' </summary>
		Private thrown As Throwable

		''' <summary>
		''' @serial Name of the source Logger.
		''' </summary>
		Private loggerName As String

		''' <summary>
		''' @serial Resource bundle name to localized log message.
		''' </summary>
		Private resourceBundleName As String

		<NonSerialized> _
		Private needToInferCaller As Boolean
		<NonSerialized> _
		Private parameters As Object()
		<NonSerialized> _
		Private resourceBundle_Renamed As ResourceBundle

		''' <summary>
		''' Returns the default value for a new LogRecord's threadID.
		''' </summary>
		Private Function defaultThreadID() As Integer
			Dim tid As Long = Thread.CurrentThread.id
			If tid < MIN_SEQUENTIAL_THREAD_ID Then
				Return CInt(tid)
			Else
				Dim id As Integer? = threadIds.get()
				If id Is Nothing Then
					id = nextThreadId.andIncrement
					threadIds.set(id)
				End If
				Return id
			End If
		End Function

		''' <summary>
		''' Construct a LogRecord with the given level and message values.
		''' <p>
		''' The sequence property will be initialized with a new unique value.
		''' These sequence values are allocated in increasing order within a VM.
		''' <p>
		''' The millis property will be initialized to the current time.
		''' <p>
		''' The thread ID property will be initialized with a unique ID for
		''' the current thread.
		''' <p>
		''' All other properties will be initialized to "null".
		''' </summary>
		''' <param name="level">  a logging level value </param>
		''' <param name="msg">  the raw non-localized logging message (may be null) </param>
		Public Sub New(ByVal level_Renamed As Level, ByVal msg As String)
			' Make sure level isn't null, by calling random method.
			level_Renamed.GetType()
			Me.level_Renamed = level_Renamed
			message = msg
			' Assign a thread ID and a unique sequence number.
			sequenceNumber = globalSequenceNumber.andIncrement
			threadID = defaultThreadID()
			millis = System.currentTimeMillis()
			needToInferCaller = True
		End Sub

		''' <summary>
		''' Get the source Logger's name.
		''' </summary>
		''' <returns> source logger name (may be null) </returns>
		Public Overridable Property loggerName As String
			Get
				Return loggerName
			End Get
			Set(ByVal name As String)
				loggerName = name
			End Set
		End Property


		''' <summary>
		''' Get the localization resource bundle
		''' <p>
		''' This is the ResourceBundle that should be used to localize
		''' the message string before formatting it.  The result may
		''' be null if the message is not localizable, or if no suitable
		''' ResourceBundle is available. </summary>
		''' <returns> the localization resource bundle </returns>
		Public Overridable Property resourceBundle As ResourceBundle
			Get
				Return resourceBundle_Renamed
			End Get
			Set(ByVal bundle As ResourceBundle)
				resourceBundle_Renamed = bundle
			End Set
		End Property


		''' <summary>
		''' Get the localization resource bundle name
		''' <p>
		''' This is the name for the ResourceBundle that should be
		''' used to localize the message string before formatting it.
		''' The result may be null if the message is not localizable. </summary>
		''' <returns> the localization resource bundle name </returns>
		Public Overridable Property resourceBundleName As String
			Get
				Return resourceBundleName
			End Get
			Set(ByVal name As String)
				resourceBundleName = name
			End Set
		End Property


		''' <summary>
		''' Get the logging message level, for example Level.SEVERE. </summary>
		''' <returns> the logging message level </returns>
		Public Overridable Property level As Level
			Get
				Return level_Renamed
			End Get
			Set(ByVal level_Renamed As Level)
				If level_Renamed Is Nothing Then Throw New NullPointerException
				Me.level_Renamed = level_Renamed
			End Set
		End Property


		''' <summary>
		''' Get the sequence number.
		''' <p>
		''' Sequence numbers are normally assigned in the LogRecord
		''' constructor, which assigns unique sequence numbers to
		''' each new LogRecord in increasing order. </summary>
		''' <returns> the sequence number </returns>
		Public Overridable Property sequenceNumber As Long
			Get
				Return sequenceNumber
			End Get
			Set(ByVal seq As Long)
				sequenceNumber = seq
			End Set
		End Property


		''' <summary>
		''' Get the  name of the class that (allegedly) issued the logging request.
		''' <p>
		''' Note that this sourceClassName is not verified and may be spoofed.
		''' This information may either have been provided as part of the
		''' logging call, or it may have been inferred automatically by the
		''' logging framework.  In the latter case, the information may only
		''' be approximate and may in fact describe an earlier call on the
		''' stack frame.
		''' <p>
		''' May be null if no information could be obtained.
		''' </summary>
		''' <returns> the source class name </returns>
		Public Overridable Property sourceClassName As String
			Get
				If needToInferCaller Then inferCaller()
				Return sourceClassName
			End Get
			Set(ByVal sourceClassName As String)
				Me.sourceClassName = sourceClassName
				needToInferCaller = False
			End Set
		End Property


		''' <summary>
		''' Get the  name of the method that (allegedly) issued the logging request.
		''' <p>
		''' Note that this sourceMethodName is not verified and may be spoofed.
		''' This information may either have been provided as part of the
		''' logging call, or it may have been inferred automatically by the
		''' logging framework.  In the latter case, the information may only
		''' be approximate and may in fact describe an earlier call on the
		''' stack frame.
		''' <p>
		''' May be null if no information could be obtained.
		''' </summary>
		''' <returns> the source method name </returns>
		Public Overridable Property sourceMethodName As String
			Get
				If needToInferCaller Then inferCaller()
				Return sourceMethodName
			End Get
			Set(ByVal sourceMethodName As String)
				Me.sourceMethodName = sourceMethodName
				needToInferCaller = False
			End Set
		End Property


		''' <summary>
		''' Get the "raw" log message, before localization or formatting.
		''' <p>
		''' May be null, which is equivalent to the empty string "".
		''' <p>
		''' This message may be either the final text or a localization key.
		''' <p>
		''' During formatting, if the source logger has a localization
		''' ResourceBundle and if that ResourceBundle has an entry for
		''' this message string, then the message string is replaced
		''' with the localized value.
		''' </summary>
		''' <returns> the raw message string </returns>
		Public Overridable Property message As String
			Get
				Return message
			End Get
			Set(ByVal message As String)
				Me.message = message
			End Set
		End Property


		''' <summary>
		''' Get the parameters to the log message.
		''' </summary>
		''' <returns> the log message parameters.  May be null if
		'''                  there are no parameters. </returns>
		Public Overridable Property parameters As Object()
			Get
				Return parameters
			End Get
			Set(ByVal parameters As Object())
				Me.parameters = parameters
			End Set
		End Property


		''' <summary>
		''' Get an identifier for the thread where the message originated.
		''' <p>
		''' This is a thread identifier within the Java VM and may or
		''' may not map to any operating system ID.
		''' </summary>
		''' <returns> thread ID </returns>
		Public Overridable Property threadID As Integer
			Get
				Return threadID
			End Get
			Set(ByVal threadID As Integer)
				Me.threadID = threadID
			End Set
		End Property


		''' <summary>
		''' Get event time in milliseconds since 1970.
		''' </summary>
		''' <returns> event time in millis since 1970 </returns>
		Public Overridable Property millis As Long
			Get
				Return millis
			End Get
			Set(ByVal millis As Long)
				Me.millis = millis
			End Set
		End Property


		''' <summary>
		''' Get any throwable associated with the log record.
		''' <p>
		''' If the event involved an exception, this will be the
		''' exception object. Otherwise null.
		''' </summary>
		''' <returns> a throwable </returns>
		Public Overridable Property thrown As Throwable
			Get
				Return thrown
			End Get
			Set(ByVal thrown As Throwable)
				Me.thrown = thrown
			End Set
		End Property


		Private Const serialVersionUID As Long = 5372048053134512534L

		''' <summary>
		''' @serialData Default fields, followed by a two byte version number
		''' (major byte, followed by minor byte), followed by information on
		''' the log record parameter array.  If there is no parameter array,
		''' then -1 is written.  If there is a parameter array (possible of zero
		''' length) then the array length is written as an integer, followed
		''' by String values for each parameter.  If a parameter is null, then
		''' a null String is written.  Otherwise the output of Object.toString()
		''' is written.
		''' </summary>
		Private Sub writeObject(ByVal out As ObjectOutputStream)
			' We have to call defaultWriteObject first.
			out.defaultWriteObject()

			' Write our version number.
			out.writeByte(1)
			out.writeByte(0)
			If parameters Is Nothing Then
				out.writeInt(-1)
				Return
			End If
			out.writeInt(parameters.Length)
			' Write string values for the parameters.
			For i As Integer = 0 To parameters.Length - 1
				If parameters(i) Is Nothing Then
					out.writeObject(Nothing)
				Else
					out.writeObject(parameters(i).ToString())
				End If
			Next i
		End Sub

		Private Sub readObject(ByVal [in] As ObjectInputStream)
			' We have to call defaultReadObject first.
			[in].defaultReadObject()

			' Read version number.
			Dim major As SByte = [in].readByte()
			Dim minor As SByte = [in].readByte()
			If major <> 1 Then Throw New IOException("LogRecord: bad version: " & major & "." & minor)
			Dim len As Integer = [in].readInt()
			If len = -1 Then
				parameters = Nothing
			Else
				parameters = New Object(len - 1){}
				For i As Integer = 0 To parameters.Length - 1
					parameters(i) = [in].readObject()
				Next i
			End If
			' If necessary, try to regenerate the resource bundle.
			If resourceBundleName IsNot Nothing Then
				Try
					' use system class loader to ensure the ResourceBundle
					' instance is a different instance than null loader uses
					Dim bundle As ResourceBundle = ResourceBundle.getBundle(resourceBundleName, Locale.default, ClassLoader.systemClassLoader)
					resourceBundle_Renamed = bundle
				Catch ex As MissingResourceException
					' This is not a good place to throw an exception,
					' so we simply leave the resourceBundle null.
					resourceBundle_Renamed = Nothing
				End Try
			End If

			needToInferCaller = False
		End Sub

		' Private method to infer the caller's class and method names
		Private Sub inferCaller()
			needToInferCaller = False
			Dim access As sun.misc.JavaLangAccess = sun.misc.SharedSecrets.javaLangAccess
			Dim throwable As New Throwable
			Dim depth As Integer = access.getStackTraceDepth(throwable)

			Dim lookingForLogger As Boolean = True
			For ix As Integer = 0 To depth - 1
				' Calling getStackTraceElement directly prevents the VM
				' from paying the cost of building the entire stack frame.
				Dim frame As StackTraceElement = access.getStackTraceElement(throwable, ix)
				Dim cname As String = frame.className
				Dim isLoggerImpl As Boolean = isLoggerImplFrame(cname)
				If lookingForLogger Then
					' Skip all frames until we have found the first logger frame.
					If isLoggerImpl Then lookingForLogger = False
				Else
					If Not isLoggerImpl Then
						' skip reflection call
						If (Not cname.StartsWith("java.lang.reflect.")) AndAlso (Not cname.StartsWith("sun.reflect.")) Then
						   ' We've found the relevant frame.
						   sourceClassName = cname
						   sourceMethodName = frame.methodName
						   Return
						End If
					End If
				End If
			Next ix
			' We haven't found a suitable frame, so just punt.  This is
			' OK as we are only committed to making a "best effort" here.
		End Sub

		Private Function isLoggerImplFrame(ByVal cname As String) As Boolean
			' the log record could be created for a platform logger
			Return (cname.Equals("java.util.logging.Logger") OrElse cname.StartsWith("java.util.logging.LoggingProxyImpl") OrElse cname.StartsWith("sun.util.logging."))
		End Function
	End Class

End Namespace