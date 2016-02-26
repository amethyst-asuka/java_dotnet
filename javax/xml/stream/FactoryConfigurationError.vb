Imports System

'
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

'
' * Copyright (c) 2009 by Oracle Corporation. All Rights Reserved.
' 

Namespace javax.xml.stream

	''' <summary>
	''' An error class for reporting factory configuration errors.
	''' 
	''' @version 1.0
	''' @author Copyright (c) 2009 by Oracle Corporation. All Rights Reserved.
	''' @since 1.6
	''' </summary>
	Public Class FactoryConfigurationError
		Inherits Exception

		Private Const serialVersionUID As Long = -2994412584589975744L

	  Friend nested As Exception

	  ''' <summary>
	  ''' Default constructor
	  ''' </summary>
	  Public Sub New()
	  End Sub

	  ''' <summary>
	  ''' Construct an exception with a nested inner exception
	  ''' </summary>
	  ''' <param name="e"> the exception to nest </param>
	  Public Sub New(ByVal e As Exception)
		nested = e
	  End Sub

	  ''' <summary>
	  ''' Construct an exception with a nested inner exception
	  ''' and a message
	  ''' </summary>
	  ''' <param name="e"> the exception to nest </param>
	  ''' <param name="msg"> the message to report </param>
	  Public Sub New(ByVal e As Exception, ByVal msg As String)
		MyBase.New(msg)
		nested = e
	  End Sub

	  ''' <summary>
	  ''' Construct an exception with a nested inner exception
	  ''' and a message
	  ''' </summary>
	  ''' <param name="msg"> the message to report </param>
	  ''' <param name="e"> the exception to nest </param>
	  Public Sub New(ByVal msg As String, ByVal e As Exception)
		MyBase.New(msg)
		nested = e
	  End Sub

	  ''' <summary>
	  ''' Construct an exception with associated message
	  ''' </summary>
	  ''' <param name="msg"> the message to report </param>
	  Public Sub New(ByVal msg As String)
		MyBase.New(msg)
	  End Sub

	  ''' <summary>
	  ''' Return the nested exception (if any)
	  ''' </summary>
	  ''' <returns> the nested exception or null </returns>
	  Public Overridable Property exception As Exception
		  Get
			Return nested
		  End Get
	  End Property
		''' <summary>
		''' use the exception chaining mechanism of JDK1.4
		''' </summary>
		Public Property Overrides cause As Exception
			Get
				Return nested
			End Get
		End Property

	  ''' <summary>
	  ''' Report the message associated with this error
	  ''' </summary>
	  ''' <returns> the string value of the message </returns>
	  Public Overridable Property message As String
		  Get
			Dim msg As String = MyBase.message
			If msg IsNot Nothing Then Return msg
			If nested IsNot Nothing Then
			  msg = nested.Message
			  If msg Is Nothing Then msg = nested.GetType().ToString()
			End If
			Return msg
		  End Get
	  End Property



	End Class

End Namespace