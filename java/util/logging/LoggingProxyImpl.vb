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

		Public Overrides Function getLogger(ByVal name As String) As Object
			' always create a platform logger with the resource bundle name
			Return Logger.getPlatformLogger(name)
		End Function

		Public Overrides Function getLevel(ByVal logger_Renamed As Object) As Object
			Return CType(logger_Renamed, Logger).level
		End Function

		Public Overrides Sub setLevel(ByVal logger_Renamed As Object, ByVal newLevel As Object)
			CType(logger_Renamed, Logger).level = CType(newLevel, Level)
		End Sub

		Public Overrides Function isLoggable(ByVal logger_Renamed As Object, ByVal level_Renamed As Object) As Boolean
			Return CType(logger_Renamed, Logger).isLoggable(CType(level_Renamed, Level))
		End Function

		Public Overrides Sub log(ByVal logger_Renamed As Object, ByVal level_Renamed As Object, ByVal msg As String)
			CType(logger_Renamed, Logger).log(CType(level_Renamed, Level), msg)
		End Sub

		Public Overrides Sub log(ByVal logger_Renamed As Object, ByVal level_Renamed As Object, ByVal msg As String, ByVal t As Throwable)
			CType(logger_Renamed, Logger).log(CType(level_Renamed, Level), msg, t)
		End Sub

		Public Overrides Sub log(ByVal logger_Renamed As Object, ByVal level_Renamed As Object, ByVal msg As String, ParamArray ByVal params As Object())
			CType(logger_Renamed, Logger).log(CType(level_Renamed, Level), msg, params)
		End Sub

		Public  Overrides ReadOnly Property  loggerNames As IList(Of String)
			Get
				Return LogManager.loggingMXBean.loggerNames
			End Get
		End Property

		Public Overrides Function getLoggerLevel(ByVal loggerName As String) As String
			Return LogManager.loggingMXBean.getLoggerLevel(loggerName)
		End Function

		Public Overrides Sub setLoggerLevel(ByVal loggerName As String, ByVal levelName As String)
			LogManager.loggingMXBean.loggerLevelvel(loggerName, levelName)
		End Sub

		Public Overrides Function getParentLoggerName(ByVal loggerName As String) As String
			Return LogManager.loggingMXBean.getParentLoggerName(loggerName)
		End Function

		Public Overrides Function parseLevel(ByVal levelName As String) As Object
			Dim level_Renamed As Level = Level.findLevel(levelName)
			If level_Renamed Is Nothing Then Throw New IllegalArgumentException("Unknown level """ & levelName & """")
			Return level_Renamed
		End Function

		Public Overrides Function getLevelName(ByVal level_Renamed As Object) As String
			Return CType(level_Renamed, Level).levelName
		End Function

		Public Overrides Function getLevelValue(ByVal level_Renamed As Object) As Integer
			Return CType(level_Renamed, Level)
		End Function

		Public Overrides Function getProperty(ByVal key As String) As String
			Return LogManager.logManager.getProperty(key)
		End Function
	End Class

End Namespace