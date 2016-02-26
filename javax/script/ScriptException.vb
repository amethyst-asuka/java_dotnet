Imports System

'
' * Copyright (c) 2005, 2011, Oracle and/or its affiliates. All rights reserved.
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

Namespace javax.script

	''' <summary>
	''' The generic <code>Exception</code> class for the Scripting APIs.  Checked
	''' exception types thrown by underlying scripting implementations must be wrapped in instances of
	''' <code>ScriptException</code>.  The class has members to store line and column numbers and
	''' filenames if this information is available.
	''' 
	''' @author Mike Grogan
	''' @since 1.6
	''' </summary>
	Public Class ScriptException
		Inherits Exception

		Private Const serialVersionUID As Long = 8265071037049225001L

		Private fileName As String
		Private lineNumber As Integer
		Private columnNumber As Integer

		''' <summary>
		''' Creates a <code>ScriptException</code> with a String to be used in its message.
		''' Filename, and line and column numbers are unspecified.
		''' </summary>
		''' <param name="s"> The String to use in the message. </param>
		Public Sub New(ByVal s As String)
			MyBase.New(s)
			fileName = Nothing
			lineNumber = -1
			columnNumber = -1
		End Sub

		''' <summary>
		''' Creates a <code>ScriptException</code> wrapping an <code>Exception</code> thrown by an underlying
		''' interpreter.  Line and column numbers and filename are unspecified.
		''' </summary>
		''' <param name="e"> The wrapped <code>Exception</code>. </param>
		Public Sub New(ByVal e As Exception)
			MyBase.New(e)
			fileName = Nothing
			lineNumber = -1
			columnNumber = -1
		End Sub

		''' <summary>
		''' Creates a <code>ScriptException</code> with message, filename and linenumber to
		''' be used in error messages.
		''' </summary>
		''' <param name="message"> The string to use in the message
		''' </param>
		''' <param name="fileName"> The file or resource name describing the location of a script error
		''' causing the <code>ScriptException</code> to be thrown.
		''' </param>
		''' <param name="lineNumber"> A line number describing the location of a script error causing
		''' the <code>ScriptException</code> to be thrown. </param>
		Public Sub New(ByVal message As String, ByVal fileName As String, ByVal lineNumber As Integer)
			MyBase.New(message)
			Me.fileName = fileName
			Me.lineNumber = lineNumber
			Me.columnNumber = -1
		End Sub

		''' <summary>
		''' <code>ScriptException</code> constructor specifying message, filename, line number
		''' and column number. </summary>
		''' <param name="message"> The message. </param>
		''' <param name="fileName"> The filename </param>
		''' <param name="lineNumber"> the line number. </param>
		''' <param name="columnNumber"> the column number. </param>
		Public Sub New(ByVal message As String, ByVal fileName As String, ByVal lineNumber As Integer, ByVal columnNumber As Integer)
			MyBase.New(message)
			Me.fileName = fileName
			Me.lineNumber = lineNumber
			Me.columnNumber = columnNumber
		End Sub

		''' <summary>
		''' Returns a message containing the String passed to a constructor as well as
		''' line and column numbers and filename if any of these are known. </summary>
		''' <returns> The error message. </returns>
		Public Overridable Property message As String
			Get
				Dim ret As String = MyBase.message
				If fileName IsNot Nothing Then
					ret += (" in " & fileName)
					If lineNumber <> -1 Then ret &= " at line number " & lineNumber
    
					If columnNumber <> -1 Then ret &= " at column number " & columnNumber
				End If
    
				Return ret
			End Get
		End Property

		''' <summary>
		''' Get the line number on which an error occurred. </summary>
		''' <returns> The line number.  Returns -1 if a line number is unavailable. </returns>
		Public Overridable Property lineNumber As Integer
			Get
				Return lineNumber
			End Get
		End Property

		''' <summary>
		''' Get the column number on which an error occurred. </summary>
		''' <returns> The column number.  Returns -1 if a column number is unavailable. </returns>
		Public Overridable Property columnNumber As Integer
			Get
				Return columnNumber
			End Get
		End Property

		''' <summary>
		''' Get the source of the script causing the error. </summary>
		''' <returns> The file name of the script or some other string describing the script
		''' source.  May return some implementation-defined string such as <i>&lt;unknown&gt;</i>
		''' if a description of the source is unavailable. </returns>
		Public Overridable Property fileName As String
			Get
				Return fileName
			End Get
		End Property
	End Class

End Namespace