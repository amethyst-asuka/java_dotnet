Imports System
Imports System.Runtime.CompilerServices

'
' * Copyright (c) 2000, 2012, Oracle and/or its affiliates. All rights reserved.
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
	''' Print a brief summary of the {@code LogRecord} in a human readable
	''' format.  The summary will typically be 1 or 2 lines.
	''' 
	''' <p>
	''' <a name="formatting">
	''' <b>Configuration:</b></a>
	''' The {@code SimpleFormatter} is initialized with the
	''' <a href="../Formatter.html#syntax">format string</a>
	''' specified in the {@code java.util.logging.SimpleFormatter.format}
	''' property to <seealso cref="#format format"/> the log messages.
	''' This property can be defined
	''' in the <seealso cref="LogManager#getProperty logging properties"/>
	''' configuration file
	''' or as a system property.  If this property is set in both
	''' the logging properties and system properties,
	''' the format string specified in the system property will be used.
	''' If this property is not defined or the given format string
	''' is <seealso cref="java.util.IllegalFormatException illegal"/>,
	''' the default format is implementation-specific.
	''' 
	''' @since 1.4 </summary>
	''' <seealso cref= java.util.Formatter </seealso>

	Public Class SimpleFormatter
		Inherits Formatter

		' format string for printing the log record
		Private Shared ReadOnly format_Renamed As String = sun.util.logging.LoggingSupport.simpleFormat
		Private ReadOnly dat As DateTime? = New DateTime?()

		''' <summary>
		''' Format the given LogRecord.
		''' <p>
		''' The formatting can be customized by specifying the
		''' <a href="../Formatter.html#syntax">format string</a>
		''' in the <a href="#formatting">
		''' {@code java.util.logging.SimpleFormatter.format}</a> property.
		''' The given {@code LogRecord} will be formatted as if by calling:
		''' <pre>
		'''    <seealso cref="String#format String.format"/>(format, date, source, logger, level, message, thrown);
		''' </pre>
		''' where the arguments are:<br>
		''' <ol>
		''' <li>{@code format} - the {@link java.util.Formatter
		'''     java.util.Formatter} format string specified in the
		'''     {@code java.util.logging.SimpleFormatter.format} property
		'''     or the default format.</li>
		''' <li>{@code date} - a <seealso cref="Date"/> object representing
		'''     <seealso cref="LogRecord#getMillis event time"/> of the log record.</li>
		''' <li>{@code source} - a string representing the caller, if available;
		'''     otherwise, the logger's name.</li>
		''' <li>{@code logger} - the logger's name.</li>
		''' <li>{@code level} - the {@link Level#getLocalizedName
		'''     log level}.</li>
		''' <li>{@code message} - the formatted log message
		'''     returned from the <seealso cref="Formatter#formatMessage(LogRecord)"/>
		'''     method.  It uses <seealso cref="java.text.MessageFormat java.text"/>
		'''     formatting and does not use the {@code java.util.Formatter
		'''     format} argument.</li>
		''' <li>{@code thrown} - a string representing
		'''     the <seealso cref="LogRecord#getThrown throwable"/>
		'''     associated with the log record and its backtrace
		'''     beginning with a newline character, if any;
		'''     otherwise, an empty string.</li>
		''' </ol>
		''' 
		''' <p>Some example formats:<br>
		''' <ul>
		''' <li> {@code java.util.logging.SimpleFormatter.format="%4$s: %5$s [%1$tc]%n"}
		'''     <p>This prints 1 line with the log level ({@code 4$}),
		'''     the log message ({@code 5$}) and the timestamp ({@code 1$}) in
		'''     a square bracket.
		'''     <pre>
		'''     WARNING: warning message [Tue Mar 22 13:11:31 PDT 2011]
		'''     </pre></li>
		''' <li> {@code java.util.logging.SimpleFormatter.format="%1$tc %2$s%n%4$s: %5$s%6$s%n"}
		'''     <p>This prints 2 lines where the first line includes
		'''     the timestamp ({@code 1$}) and the source ({@code 2$});
		'''     the second line includes the log level ({@code 4$}) and
		'''     the log message ({@code 5$}) followed with the throwable
		'''     and its backtrace ({@code 6$}), if any:
		'''     <pre>
		'''     Tue Mar 22 13:11:31 PDT 2011 MyClass fatal
		'''     SEVERE: several message with an exception
		'''     java.lang.IllegalArgumentException: invalid argument
		'''             at MyClass.mash(MyClass.java:9)
		'''             at MyClass.crunch(MyClass.java:6)
		'''             at MyClass.main(MyClass.java:3)
		'''     </pre></li>
		''' <li> {@code java.util.logging.SimpleFormatter.format="%1$tb %1$td, %1$tY %1$tl:%1$tM:%1$tS %1$Tp %2$s%n%4$s: %5$s%n"}
		'''      <p>This prints 2 lines similar to the example above
		'''         with a different date/time formatting and does not print
		'''         the throwable and its backtrace:
		'''     <pre>
		'''     Mar 22, 2011 1:11:31 PM MyClass fatal
		'''     SEVERE: several message with an exception
		'''     </pre></li>
		''' </ul>
		''' <p>This method can also be overridden in a subclass.
		''' It is recommended to use the <seealso cref="Formatter#formatMessage"/>
		''' convenience method to localize and format the message field.
		''' </summary>
		''' <param name="record"> the log record to be formatted. </param>
		''' <returns> a formatted log record </returns>
		<MethodImpl(MethodImplOptions.Synchronized)> _
		Public Overrides Function format(  record As LogRecord) As String
			dat.Value.time = record.millis
			Dim source As String
			If record.sourceClassName IsNot Nothing Then
				source = record.sourceClassName
				If record.sourceMethodName IsNot Nothing Then source &= " " & record.sourceMethodName
			Else
				source = record.loggerName
			End If
			Dim message As String = formatMessage(record)
			Dim throwable As String = ""
			If record.thrown IsNot Nothing Then
				Dim sw As New StringWriter
				Dim pw As New PrintWriter(sw)
				pw.println()
				record.thrown.printStackTrace(pw)
				pw.close()
				throwable = sw.ToString()
			End If
			Return String.format(format_Renamed, dat, source, record.loggerName, record.level.localizedLevelName, message, throwable)
		End Function
	End Class

End Namespace