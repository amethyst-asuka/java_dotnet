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

Namespace java.net


	''' <summary>
	''' Checked exception thrown to indicate that a string could not be parsed as a
	''' URI reference.
	''' 
	''' @author Mark Reinhold </summary>
	''' <seealso cref= URI
	''' @since 1.4 </seealso>

	Public Class URISyntaxException
		Inherits Exception

		Private Shadows Const serialVersionUID As Long = 2137979680897488891L

		Private input As String
		Private index As Integer

		''' <summary>
		''' Constructs an instance from the given input string, reason, and error
		''' index.
		''' </summary>
		''' <param name="input">   The input string </param>
		''' <param name="reason">  A string explaining why the input could not be parsed </param>
		''' <param name="index">   The index at which the parse error occurred,
		'''                 or {@code -1} if the index is not known
		''' </param>
		''' <exception cref="NullPointerException">
		'''          If either the input or reason strings are {@code null}
		''' </exception>
		''' <exception cref="IllegalArgumentException">
		'''          If the error index is less than {@code -1} </exception>
		Public Sub New(  input As String,   reason As String,   index As Integer)
			MyBase.New(reason)
			If (input Is Nothing) OrElse (reason Is Nothing) Then Throw New NullPointerException
			If index < -1 Then Throw New IllegalArgumentException
			Me.input = input
			Me.index = index
		End Sub

		''' <summary>
		''' Constructs an instance from the given input string and reason.  The
		''' resulting object will have an error index of {@code -1}.
		''' </summary>
		''' <param name="input">   The input string </param>
		''' <param name="reason">  A string explaining why the input could not be parsed
		''' </param>
		''' <exception cref="NullPointerException">
		'''          If either the input or reason strings are {@code null} </exception>
		Public Sub New(  input As String,   reason As String)
			Me.New(input, reason, -1)
		End Sub

		''' <summary>
		''' Returns the input string.
		''' </summary>
		''' <returns>  The input string </returns>
		Public Overridable Property input As String
			Get
				Return input
			End Get
		End Property

		''' <summary>
		''' Returns a string explaining why the input string could not be parsed.
		''' </summary>
		''' <returns>  The reason string </returns>
		Public Overridable Property reason As String
			Get
				Return MyBase.message
			End Get
		End Property

		''' <summary>
		''' Returns an index into the input string of the position at which the
		''' parse error occurred, or {@code -1} if this position is not known.
		''' </summary>
		''' <returns>  The error index </returns>
		Public Overridable Property index As Integer
			Get
				Return index
			End Get
		End Property

		''' <summary>
		''' Returns a string describing the parse error.  The resulting string
		''' consists of the reason string followed by a colon character
		''' ({@code ':'}), a space, and the input string.  If the error index is
		''' defined then the string {@code " at index "} followed by the index, in
		''' decimal, is inserted after the reason string and before the colon
		''' character.
		''' </summary>
		''' <returns>  A string describing the parse error </returns>
		Public  Overrides ReadOnly Property  message As String
			Get
				Dim sb As New StringBuffer
				sb.append(reason)
				If index > -1 Then
					sb.append(" at index ")
					sb.append(index)
				End If
				sb.append(": ")
				sb.append(input)
				Return sb.ToString()
			End Get
		End Property

	End Class

End Namespace