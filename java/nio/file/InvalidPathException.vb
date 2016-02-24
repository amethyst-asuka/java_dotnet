'
' * Copyright (c) 2007, 2009, Oracle and/or its affiliates. All rights reserved.
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

Namespace java.nio.file

	''' <summary>
	''' Unchecked exception thrown when path string cannot be converted into a
	''' <seealso cref="Path"/> because the path string contains invalid characters, or
	''' the path string is invalid for other file system specific reasons.
	''' </summary>

	Public Class InvalidPathException
		Inherits IllegalArgumentException

		Friend Shadows Const serialVersionUID As Long = 4355821422286746137L

		Private input As String
		Private index As Integer

		''' <summary>
		''' Constructs an instance from the given input string, reason, and error
		''' index.
		''' </summary>
		''' <param name="input">   the input string </param>
		''' <param name="reason">  a string explaining why the input was rejected </param>
		''' <param name="index">   the index at which the error occurred,
		'''                 or <tt>-1</tt> if the index is not known
		''' </param>
		''' <exception cref="NullPointerException">
		'''          if either the input or reason strings are <tt>null</tt>
		''' </exception>
		''' <exception cref="IllegalArgumentException">
		'''          if the error index is less than <tt>-1</tt> </exception>
		Public Sub New(ByVal input As String, ByVal reason As String, ByVal index As Integer)
			MyBase.New(reason)
			If (input Is Nothing) OrElse (reason Is Nothing) Then Throw New NullPointerException
			If index < -1 Then Throw New IllegalArgumentException
			Me.input = input
			Me.index = index
		End Sub

		''' <summary>
		''' Constructs an instance from the given input string and reason.  The
		''' resulting object will have an error index of <tt>-1</tt>.
		''' </summary>
		''' <param name="input">   the input string </param>
		''' <param name="reason">  a string explaining why the input was rejected
		''' </param>
		''' <exception cref="NullPointerException">
		'''          if either the input or reason strings are <tt>null</tt> </exception>
		Public Sub New(ByVal input As String, ByVal reason As String)
			Me.New(input, reason, -1)
		End Sub

		''' <summary>
		''' Returns the input string.
		''' </summary>
		''' <returns>  the input string </returns>
		Public Overridable Property input As String
			Get
				Return input
			End Get
		End Property

		''' <summary>
		''' Returns a string explaining why the input string was rejected.
		''' </summary>
		''' <returns>  the reason string </returns>
		Public Overridable Property reason As String
			Get
				Return MyBase.message
			End Get
		End Property

		''' <summary>
		''' Returns an index into the input string of the position at which the
		''' error occurred, or <tt>-1</tt> if this position is not known.
		''' </summary>
		''' <returns>  the error index </returns>
		Public Overridable Property index As Integer
			Get
				Return index
			End Get
		End Property

		''' <summary>
		''' Returns a string describing the error.  The resulting string
		''' consists of the reason string followed by a colon character
		''' (<tt>':'</tt>), a space, and the input string.  If the error index is
		''' defined then the string <tt>" at index "</tt> followed by the index, in
		''' decimal, is inserted after the reason string and before the colon
		''' character.
		''' </summary>
		''' <returns>  a string describing the error </returns>
		Public Property Overrides message As String
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