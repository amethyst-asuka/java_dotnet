Imports System
Imports System.Runtime.CompilerServices

'
' * Copyright (c) 2000, 2006, Oracle and/or its affiliates. All rights reserved.
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
	''' A Formatter provides support for formatting LogRecords.
	''' <p>
	''' Typically each logging Handler will have a Formatter associated
	''' with it.  The Formatter takes a LogRecord and converts it to
	''' a string.
	''' <p>
	''' Some formatters (such as the XMLFormatter) need to wrap head
	''' and tail strings around a set of formatted records. The getHeader
	''' and getTail methods can be used to obtain these strings.
	''' 
	''' @since 1.4
	''' </summary>

	Public MustInherit Class Formatter

		''' <summary>
		''' Construct a new formatter.
		''' </summary>
		Protected Friend Sub New()
		End Sub

		''' <summary>
		''' Format the given log record and return the formatted string.
		''' <p>
		''' The resulting formatted String will normally include a
		''' localized and formatted version of the LogRecord's message field.
		''' It is recommended to use the <seealso cref="Formatter#formatMessage"/>
		''' convenience method to localize and format the message field.
		''' </summary>
		''' <param name="record"> the log record to be formatted. </param>
		''' <returns> the formatted log record </returns>
		Public MustOverride Function format(ByVal record As LogRecord) As String


		''' <summary>
		''' Return the header string for a set of formatted records.
		''' <p>
		''' This base class returns an empty string, but this may be
		''' overridden by subclasses.
		''' </summary>
		''' <param name="h">  The target handler (can be null) </param>
		''' <returns>  header string </returns>
		Public Overridable Function getHead(ByVal h As Handler) As String
			Return ""
		End Function

		''' <summary>
		''' Return the tail string for a set of formatted records.
		''' <p>
		''' This base class returns an empty string, but this may be
		''' overridden by subclasses.
		''' </summary>
		''' <param name="h">  The target handler (can be null) </param>
		''' <returns>  tail string </returns>
		Public Overridable Function getTail(ByVal h As Handler) As String
			Return ""
		End Function


		''' <summary>
		''' Localize and format the message string from a log record.  This
		''' method is provided as a convenience for Formatter subclasses to
		''' use when they are performing formatting.
		''' <p>
		''' The message string is first localized to a format string using
		''' the record's ResourceBundle.  (If there is no ResourceBundle,
		''' or if the message key is not found, then the key is used as the
		''' format string.)  The format String uses java.text style
		''' formatting.
		''' <ul>
		''' <li>If there are no parameters, no formatter is used.
		''' <li>Otherwise, if the string contains "{0" then
		'''     java.text.MessageFormat  is used to format the string.
		''' <li>Otherwise no formatting is performed.
		''' </ul>
		''' <p>
		''' </summary>
		''' <param name="record">  the log record containing the raw message </param>
		''' <returns>   a localized and formatted message </returns>
		<MethodImpl(MethodImplOptions.Synchronized)> _
		Public Overridable Function formatMessage(ByVal record As LogRecord) As String
			Dim format As String = record.message
			Dim catalog As java.util.ResourceBundle = record.resourceBundle
			If catalog IsNot Nothing Then
				Try
					format = catalog.getString(record.message)
				Catch ex As java.util.MissingResourceException
					' Drop through.  Use record message as format
					format = record.message
				End Try
			End If
			' Do the formatting.
			Try
				Dim parameters As Object() = record.parameters
				If parameters Is Nothing OrElse parameters.Length = 0 Then Return format
				' Is it a java.text style format?
				' Ideally we could match with
				' Pattern.compile("\\{\\d").matcher(format).find())
				' However the cost is 14% higher, so we cheaply check for
				' 1 of the first 4 parameters
				If format.IndexOf("{0") >= 0 OrElse format.IndexOf("{1") >=0 OrElse format.IndexOf("{2") >=0 OrElse format.IndexOf("{3") >=0 Then Return java.text.MessageFormat.format(format, parameters)
				Return format

			Catch ex As Exception
				' Formatting failed: use localized format string.
				Return format
			End Try
		End Function
	End Class

End Namespace