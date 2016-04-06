Imports System.Collections.Generic

'
' * Copyright (c) 2009, 2013, Oracle and/or its affiliates. All rights reserved.
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
	''' Implementation of LoggingProxy when java.util.logging classes exist.
	''' </summary>
	Friend Class LoggingProxyImpl
		Implements sun.util.logging.LoggingProxy

		Friend Shared ReadOnly INSTANCE As sun.util.logging.LoggingProxy = New LoggingProxyImpl

		Private Sub New()
		End Sub

		Public Overrides Function getLogger(  name As String) As Object
			' always create a platform logger with the resource bundle name
			Return Logger.getPlatformLogger(name)
		End Function

		Public Overrides Function getLevel(  logger_Renamed As Object) As Object
			Return CType(logger_Renamed, Logger).level
		End Function

		Public Overrides Sub setLevel(  logger_Renamed As Object,   newLevel As Object)
			CType(logger_Renamed, Logger).level = CType(newLevel, Level)
		End Sub

		Public Overrides Function isLoggable(  logger_Renamed As Object,   level_Renamed As Object) As Boolean
			Return CType(logger_Renamed, Logger).isLoggable(CType(level_Renamed, Level))
		End Function

		Public Overrides Sub log(  logger_Renamed As Object,   level_Renamed As Object,   msg As String)
			CType(logger_Renamed, Logger).log(CType(level_Renamed, Level), msg)
		End Sub

		Public Overrides Sub log(  logger_Renamed As Object,   level_Renamed As Object,   msg As String,   t As Throwable)
			CType(logger_Renamed, Logger).log(CType(level_Renamed, Level), msg, t)
		End Sub

		Public Overrides Sub log(  logger_Renamed As Object,   level_Renamed As Object,   msg As String, ParamArray   params As Object())
			CType(logger_Renamed, Logger).log(CType(level_Renamed, Level), msg, params)
		End Sub

		Public  Overrides ReadOnly Property  loggerNames As IList(Of String)
			Get
				Return LogManager.loggingMXBean.loggerNames
			End Get
		End Property

		Public Overrides Function getLoggerLevel(  loggerName As String) As String
			Return LogManager.loggingMXBean.getLoggerLevel(loggerName)
		End Function

		Public Overrides Sub setLoggerLevel(  loggerName As String,   levelName As String)
			LogManager.loggingMXBean.loggerLevelvel(loggerName, levelName)
		End Sub

		Public Overrides Function getParentLoggerName(  loggerName As String) As String
			Return LogManager.loggingMXBean.getParentLoggerName(loggerName)
		End Function

		Public Overrides Function parseLevel(  levelName As String) As Object
			Dim level_Renamed As Level = Level.findLevel(levelName)
			If level_Renamed Is Nothing Then Throw New IllegalArgumentException("Unknown level """ & levelName & """")
			Return level_Renamed
		End Function

		Public Overrides Function getLevelName(  level_Renamed As Object) As String
			Return CType(level_Renamed, Level).levelName
		End Function

		Public Overrides Function getLevelValue(  level_Renamed As Object) As Integer
			Return CType(level_Renamed, Level)
		End Function

		Public Overrides Function getProperty(  key As String) As String
			Return LogManager.logManager.getProperty(key)
		End Function
	End Class

End Namespace