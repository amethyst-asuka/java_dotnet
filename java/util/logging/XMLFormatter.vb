Imports Microsoft.VisualBasic
Imports System

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
	''' Format a LogRecord into a standard XML format.
	''' <p>
	''' The DTD specification is provided as Appendix A to the
	''' Java Logging APIs specification.
	''' <p>
	''' The XMLFormatter can be used with arbitrary character encodings,
	''' but it is recommended that it normally be used with UTF-8.  The
	''' character encoding can be set on the output Handler.
	''' 
	''' @since 1.4
	''' </summary>

	Public Class XMLFormatter
		Inherits Formatter

		Private manager As LogManager = LogManager.logManager

		' Append a two digit number.
		Private Sub a2(  sb As StringBuilder,   x As Integer)
			If x < 10 Then sb.append("0"c)
			sb.append(x)
		End Sub

		' Append the time and date in ISO 8601 format
		Private Sub appendISO8601(  sb As StringBuilder,   millis As Long)
			Dim cal As New GregorianCalendar
			cal.timeInMillis = millis
			sb.append(cal.get(Calendar.YEAR))
			sb.append("-"c)
			a2(sb, cal.get(Calendar.MONTH) + 1)
			sb.append("-"c)
			a2(sb, cal.get(Calendar.DAY_OF_MONTH))
			sb.append("T"c)
			a2(sb, cal.get(Calendar.HOUR_OF_DAY))
			sb.append(":"c)
			a2(sb, cal.get(Calendar.MINUTE))
			sb.append(":"c)
			a2(sb, cal.get(Calendar.SECOND))
		End Sub

		' Append to the given StringBuilder an escaped version of the
		' given text string where XML special characters have been escaped.
		' For a null string we append "<null>"
		Private Sub escape(  sb As StringBuilder,   text As String)
			If text Is Nothing Then text = "<null>"
			For i As Integer = 0 To text.length() - 1
				Dim ch As Char = text.Chars(i)
				If ch = "<"c Then
					sb.append("&lt;")
				ElseIf ch = ">"c Then
					sb.append("&gt;")
				ElseIf ch = "&"c Then
					sb.append("&amp;")
				Else
					sb.append(ch)
				End If
			Next i
		End Sub

		''' <summary>
		''' Format the given message to XML.
		''' <p>
		''' This method can be overridden in a subclass.
		''' It is recommended to use the <seealso cref="Formatter#formatMessage"/>
		''' convenience method to localize and format the message field.
		''' </summary>
		''' <param name="record"> the log record to be formatted. </param>
		''' <returns> a formatted log record </returns>
		Public Overrides Function format(  record As LogRecord) As String
			Dim sb As New StringBuilder(500)
			sb.append("<record>" & vbLf)

			sb.append("  <date>")
			appendISO8601(sb, record.millis)
			sb.append("</date>" & vbLf)

			sb.append("  <millis>")
			sb.append(record.millis)
			sb.append("</millis>" & vbLf)

			sb.append("  <sequence>")
			sb.append(record.sequenceNumber)
			sb.append("</sequence>" & vbLf)

			Dim name As String = record.loggerName
			If name IsNot Nothing Then
				sb.append("  <logger>")
				escape(sb, name)
				sb.append("</logger>" & vbLf)
			End If

			sb.append("  <level>")
			escape(sb, record.level.ToString())
			sb.append("</level>" & vbLf)

			If record.sourceClassName IsNot Nothing Then
				sb.append("  <class>")
				escape(sb, record.sourceClassName)
				sb.append("</class>" & vbLf)
			End If

			If record.sourceMethodName IsNot Nothing Then
				sb.append("  <method>")
				escape(sb, record.sourceMethodName)
				sb.append("</method>" & vbLf)
			End If

			sb.append("  <thread>")
			sb.append(record.threadID)
			sb.append("</thread>" & vbLf)

			If record.message IsNot Nothing Then
				' Format the message string and its accompanying parameters.
				Dim message As String = formatMessage(record)
				sb.append("  <message>")
				escape(sb, message)
				sb.append("</message>")
				sb.append(vbLf)
			End If

			' If the message is being localized, output the key, resource
			' bundle name, and params.
			Dim bundle As ResourceBundle = record.resourceBundle
			Try
				If bundle IsNot Nothing AndAlso bundle.getString(record.message) IsNot Nothing Then
					sb.append("  <key>")
					escape(sb, record.message)
					sb.append("</key>" & vbLf)
					sb.append("  <catalog>")
					escape(sb, record.resourceBundleName)
					sb.append("</catalog>" & vbLf)
				End If
			Catch ex As Exception
				' The message is not in the catalog.  Drop through.
			End Try

			Dim parameters As Object() = record.parameters
			'  Check to see if the parameter was not a messagetext format
			'  or was not null or empty
			If parameters IsNot Nothing AndAlso parameters.Length <> 0 AndAlso record.message.IndexOf("{") = -1 Then
				For i As Integer = 0 To parameters.Length - 1
					sb.append("  <param>")
					Try
						escape(sb, parameters(i).ToString())
					Catch ex As Exception
						sb.append("???")
					End Try
					sb.append("</param>" & vbLf)
				Next i
			End If

			If record.thrown IsNot Nothing Then
				' Report on the state of the throwable.
				Dim th As Throwable = record.thrown
				sb.append("  <exception>" & vbLf)
				sb.append("    <message>")
				escape(sb, th.ToString())
				sb.append("</message>" & vbLf)
				Dim trace As StackTraceElement() = th.stackTrace
				For i As Integer = 0 To trace.Length - 1
					Dim frame As StackTraceElement = trace(i)
					sb.append("    <frame>" & vbLf)
					sb.append("      <class>")
					escape(sb, frame.className)
					sb.append("</class>" & vbLf)
					sb.append("      <method>")
					escape(sb, frame.methodName)
					sb.append("</method>" & vbLf)
					' Check for a line number.
					If frame.lineNumber >= 0 Then
						sb.append("      <line>")
						sb.append(frame.lineNumber)
						sb.append("</line>" & vbLf)
					End If
					sb.append("    </frame>" & vbLf)
				Next i
				sb.append("  </exception>" & vbLf)
			End If

			sb.append("</record>" & vbLf)
			Return sb.ToString()
		End Function

		''' <summary>
		''' Return the header string for a set of XML formatted records.
		''' </summary>
		''' <param name="h">  The target handler (can be null) </param>
		''' <returns>  a valid XML string </returns>
		Public Overrides Function getHead(  h As Handler) As String
			Dim sb As New StringBuilder
			Dim encoding As String
			sb.append("<?xml version=""1.0""")

			If h IsNot Nothing Then
				encoding = h.encoding
			Else
				encoding = Nothing
			End If

			If encoding Is Nothing Then encoding = java.nio.charset.Charset.defaultCharset().name()
			' Try to map the encoding name to a canonical name.
			Try
				Dim cs As java.nio.charset.Charset = java.nio.charset.Charset.forName(encoding)
				encoding = cs.name()
			Catch ex As Exception
				' We hit problems finding a canonical name.
				' Just use the raw encoding name.
			End Try

			sb.append(" encoding=""")
			sb.append(encoding)
			sb.append("""")
			sb.append(" standalone=""no""?>" & vbLf)
			sb.append("<!DOCTYPE log SYSTEM ""logger.dtd"">" & vbLf)
			sb.append("<log>" & vbLf)
			Return sb.ToString()
		End Function

		''' <summary>
		''' Return the tail string for a set of XML formatted records.
		''' </summary>
		''' <param name="h">  The target handler (can be null) </param>
		''' <returns>  a valid XML string </returns>
		Public Overrides Function getTail(  h As Handler) As String
			Return "</log>" & vbLf
		End Function
	End Class

End Namespace