Imports System.Collections.Generic

'
' * Copyright (c) 2003, 2013, Oracle and/or its affiliates. All rights reserved.
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
	''' Logging is the implementation class of LoggingMXBean.
	''' 
	''' The <tt>LoggingMXBean</tt> interface provides a standard
	''' method for management access to the individual
	''' {@code Logger} objects available at runtime.
	''' 
	''' @author Ron Mann
	''' @author Mandy Chung
	''' @since 1.5
	''' </summary>
	''' <seealso cref= javax.management </seealso>
	''' <seealso cref= Logger </seealso>
	''' <seealso cref= LogManager </seealso>
	Friend Class Logging
		Implements LoggingMXBean

		Private Shared logManager As LogManager = LogManager.logManager

		''' <summary>
		''' Constructor of Logging which is the implementation class
		'''  of LoggingMXBean.
		''' </summary>
		Friend Sub New()
		End Sub

		Public Overridable Property loggerNames As IList(Of String) Implements LoggingMXBean.getLoggerNames
			Get
				Dim loggers As System.Collections.IEnumerator(Of String) = logManager.loggerNames
				Dim array As New List(Of String)
    
	'JAVA TO VB CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
				Do While loggers.hasMoreElements()
	'JAVA TO VB CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
					array.add(loggers.nextElement())
				Loop
				Return array
			End Get
		End Property

		Private Shared EMPTY_STRING As String = ""
		Public Overridable Function getLoggerLevel(  loggerName As String) As String Implements LoggingMXBean.getLoggerLevel
			Dim l As Logger = logManager.getLogger(loggerName)
			If l Is Nothing Then Return Nothing

			Dim level_Renamed As Level = l.level
			If level_Renamed Is Nothing Then
				Return EMPTY_STRING
			Else
				Return level_Renamed.levelName
			End If
		End Function

		Public Overridable Sub setLoggerLevel(  loggerName As String,   levelName As String) Implements LoggingMXBean.setLoggerLevel
			If loggerName Is Nothing Then Throw New NullPointerException("loggerName is null")

			Dim logger_Renamed As Logger = logManager.getLogger(loggerName)
			If logger_Renamed Is Nothing Then Throw New IllegalArgumentException("Logger " & loggerName & "does not exist")

			Dim level_Renamed As Level = Nothing
			If levelName IsNot Nothing Then
				' parse will throw IAE if logLevel is invalid
				level_Renamed = Level.findLevel(levelName)
				If level_Renamed Is Nothing Then Throw New IllegalArgumentException("Unknown level """ & levelName & """")
			End If

			logger_Renamed.level = level_Renamed
		End Sub

		Public Overridable Function getParentLoggerName(  loggerName As String) As String Implements LoggingMXBean.getParentLoggerName
			Dim l As Logger = logManager.getLogger(loggerName)
			If l Is Nothing Then Return Nothing

			Dim p As Logger = l.parent
			If p Is Nothing Then
				' root logger
				Return EMPTY_STRING
			Else
				Return p.name
			End If
		End Function
	End Class

End Namespace