Imports System

'
' * Copyright (c) 1999, 2013, Oracle and/or its affiliates. All rights reserved.
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

Namespace javax.security.auth.callback

	''' <summary>
	''' <p> Underlying security services instantiate and pass a
	''' {@code PasswordCallback} to the {@code handle}
	''' method of a {@code CallbackHandler} to retrieve password information.
	''' </summary>
	''' <seealso cref= javax.security.auth.callback.CallbackHandler </seealso>
	<Serializable> _
	Public Class PasswordCallback
		Implements Callback

		Private Const serialVersionUID As Long = 2267422647454909926L

		''' <summary>
		''' @serial
		''' @since 1.4
		''' </summary>
		Private prompt As String
		''' <summary>
		''' @serial
		''' @since 1.4
		''' </summary>
		Private echoOn As Boolean
		''' <summary>
		''' @serial
		''' @since 1.4
		''' </summary>
		Private inputPassword As Char()

		''' <summary>
		''' Construct a {@code PasswordCallback} with a prompt
		''' and a boolean specifying whether the password should be displayed
		''' as it is being typed.
		''' 
		''' <p>
		''' </summary>
		''' <param name="prompt"> the prompt used to request the password. <p>
		''' </param>
		''' <param name="echoOn"> true if the password should be displayed
		'''                  as it is being typed.
		''' </param>
		''' <exception cref="IllegalArgumentException"> if {@code prompt} is null or
		'''                  if {@code prompt} has a length of 0. </exception>
		Public Sub New(ByVal prompt As String, ByVal echoOn As Boolean)
			If prompt Is Nothing OrElse prompt.Length = 0 Then Throw New System.ArgumentException

			Me.prompt = prompt
			Me.echoOn = echoOn
		End Sub

		''' <summary>
		''' Get the prompt.
		''' 
		''' <p>
		''' </summary>
		''' <returns> the prompt. </returns>
		Public Overridable Property prompt As String
			Get
				Return prompt
			End Get
		End Property

		''' <summary>
		''' Return whether the password
		''' should be displayed as it is being typed.
		''' 
		''' <p>
		''' </summary>
		''' <returns> the whether the password
		'''          should be displayed as it is being typed. </returns>
		Public Overridable Property echoOn As Boolean
			Get
				Return echoOn
			End Get
		End Property

		''' <summary>
		''' Set the retrieved password.
		''' 
		''' <p> This method makes a copy of the input <i>password</i>
		''' before storing it.
		''' 
		''' <p>
		''' </summary>
		''' <param name="password"> the retrieved password, which may be null.
		''' </param>
		''' <seealso cref= #getPassword </seealso>
		Public Overridable Property password As Char()
			Set(ByVal password As Char())
				Me.inputPassword = (If(password Is Nothing, Nothing, password.clone()))
			End Set
			Get
				Return (If(inputPassword Is Nothing, Nothing, inputPassword.clone()))
			End Get
		End Property


		''' <summary>
		''' Clear the retrieved password.
		''' </summary>
		Public Overridable Sub clearPassword()
			If inputPassword IsNot Nothing Then
				For i As Integer = 0 To inputPassword.Length - 1
					inputPassword(i) = " "c
				Next i
			End If
		End Sub
	End Class

End Namespace