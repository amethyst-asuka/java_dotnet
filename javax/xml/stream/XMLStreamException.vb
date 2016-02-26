Imports Microsoft.VisualBasic
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
	''' The base exception for unexpected processing errors.  This Exception
	''' class is used to report well-formedness errors as well as unexpected
	''' processing conditions.
	''' @version 1.0
	''' @author Copyright (c) 2009 by Oracle Corporation. All Rights Reserved.
	''' @since 1.6
	''' </summary>

	Public Class XMLStreamException
		Inherits Exception

	  Protected Friend nested As Exception
	  Protected Friend location As Location

	  ''' <summary>
	  ''' Default constructor
	  ''' </summary>
	  Public Sub New()
		MyBase.New()
	  End Sub

	  ''' <summary>
	  ''' Construct an exception with the assocated message.
	  ''' </summary>
	  ''' <param name="msg"> the message to report </param>
	  Public Sub New(ByVal msg As String)
		MyBase.New(msg)
	  End Sub

	  ''' <summary>
	  ''' Construct an exception with the assocated exception
	  ''' </summary>
	  ''' <param name="th"> a nested exception </param>
	  Public Sub New(ByVal th As Exception)
		  MyBase.New(th)
		nested = th
	  End Sub

	  ''' <summary>
	  ''' Construct an exception with the assocated message and exception
	  ''' </summary>
	  ''' <param name="th"> a nested exception </param>
	  ''' <param name="msg"> the message to report </param>
	  Public Sub New(ByVal msg As String, ByVal th As Exception)
		MyBase.New(msg, th)
		nested = th
	  End Sub

	  ''' <summary>
	  ''' Construct an exception with the assocated message, exception and location.
	  ''' </summary>
	  ''' <param name="th"> a nested exception </param>
	  ''' <param name="msg"> the message to report </param>
	  ''' <param name="location"> the location of the error </param>
	  Public Sub New(ByVal msg As String, ByVal location As Location, ByVal th As Exception)
		MyBase.New("ParseError at [row,col]:[" & location.lineNumber & "," & location.columnNumber & "]" & vbLf & "Message: " & msg)
		nested = th
		Me.location = location
	  End Sub

	  ''' <summary>
	  ''' Construct an exception with the assocated message, exception and location.
	  ''' </summary>
	  ''' <param name="msg"> the message to report </param>
	  ''' <param name="location"> the location of the error </param>
	  Public Sub New(ByVal msg As String, ByVal location As Location)
		MyBase.New("ParseError at [row,col]:[" & location.lineNumber & "," & location.columnNumber & "]" & vbLf & "Message: " & msg)
		Me.location = location
	  End Sub


	  ''' <summary>
	  ''' Gets the nested exception.
	  ''' </summary>
	  ''' <returns> Nested exception </returns>
	  Public Overridable Property nestedException As Exception
		  Get
			Return nested
		  End Get
	  End Property

	  ''' <summary>
	  ''' Gets the location of the exception
	  ''' </summary>
	  ''' <returns> the location of the exception, may be null if none is available </returns>
	  Public Overridable Property location As Location
		  Get
			Return location
		  End Get
	  End Property

	End Class

End Namespace