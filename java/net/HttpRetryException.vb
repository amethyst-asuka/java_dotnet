'
' * Copyright (c) 2004, 2013, Oracle and/or its affiliates. All rights reserved.
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

Namespace java.net


	''' <summary>
	''' Thrown to indicate that a HTTP request needs to be retried
	''' but cannot be retried automatically, due to streaming mode
	''' being enabled.
	''' 
	''' @author  Michael McMahon
	''' @since   1.5
	''' </summary>
	Public Class HttpRetryException
		Inherits java.io.IOException

		Private Shadows Const serialVersionUID As Long = -9186022286469111381L

		Private responseCode_Renamed As Integer
		Private location As String

		''' <summary>
		''' Constructs a new {@code HttpRetryException} from the
		''' specified response code and exception detail message
		''' </summary>
		''' <param name="detail">   the detail message. </param>
		''' <param name="code">   the HTTP response code from server. </param>
		Public Sub New(ByVal detail As String, ByVal code As Integer)
			MyBase.New(detail)
			responseCode_Renamed = code
		End Sub

		''' <summary>
		''' Constructs a new {@code HttpRetryException} with detail message
		''' responseCode and the contents of the Location response header field.
		''' </summary>
		''' <param name="detail">   the detail message. </param>
		''' <param name="code">   the HTTP response code from server. </param>
		''' <param name="location">   the URL to be redirected to </param>
		Public Sub New(ByVal detail As String, ByVal code As Integer, ByVal location As String)
			MyBase.New(detail)
			responseCode_Renamed = code
			Me.location = location
		End Sub

		''' <summary>
		''' Returns the http response code
		''' </summary>
		''' <returns>  The http response code. </returns>
		Public Overridable Function responseCode() As Integer
			Return responseCode_Renamed
		End Function

		''' <summary>
		''' Returns a string explaining why the http request could
		''' not be retried.
		''' </summary>
		''' <returns>  The reason string </returns>
		Public Overridable Property reason As String
			Get
				Return MyBase.message
			End Get
		End Property

		''' <summary>
		''' Returns the value of the Location header field if the
		''' error resulted from redirection.
		''' </summary>
		''' <returns> The location string </returns>
		Public Overridable Property location As String
			Get
				Return location
			End Get
		End Property
	End Class

End Namespace