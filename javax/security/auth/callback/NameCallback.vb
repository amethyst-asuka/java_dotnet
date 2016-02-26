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
	''' {@code NameCallback} to the {@code handle}
	''' method of a {@code CallbackHandler} to retrieve name information.
	''' </summary>
	''' <seealso cref= javax.security.auth.callback.CallbackHandler </seealso>
	<Serializable> _
	Public Class NameCallback
		Implements Callback

		Private Const serialVersionUID As Long = 3770938795909392253L

		''' <summary>
		''' @serial
		''' @since 1.4
		''' </summary>
		Private prompt As String
		''' <summary>
		''' @serial
		''' @since 1.4
		''' </summary>
		Private defaultName As String
		''' <summary>
		''' @serial
		''' @since 1.4
		''' </summary>
		Private inputName As String

		''' <summary>
		''' Construct a {@code NameCallback} with a prompt.
		''' 
		''' <p>
		''' </summary>
		''' <param name="prompt"> the prompt used to request the name.
		''' </param>
		''' <exception cref="IllegalArgumentException"> if {@code prompt} is null
		'''                  or if {@code prompt} has a length of 0. </exception>
		Public Sub New(ByVal prompt As String)
			If prompt Is Nothing OrElse prompt.Length = 0 Then Throw New System.ArgumentException
			Me.prompt = prompt
		End Sub

		''' <summary>
		''' Construct a {@code NameCallback} with a prompt
		''' and default name.
		''' 
		''' <p>
		''' </summary>
		''' <param name="prompt"> the prompt used to request the information. <p>
		''' </param>
		''' <param name="defaultName"> the name to be used as the default name displayed
		'''                  with the prompt.
		''' </param>
		''' <exception cref="IllegalArgumentException"> if {@code prompt} is null,
		'''                  if {@code prompt} has a length of 0,
		'''                  if {@code defaultName} is null,
		'''                  or if {@code defaultName} has a length of 0. </exception>
		Public Sub New(ByVal prompt As String, ByVal defaultName As String)
			If prompt Is Nothing OrElse prompt.Length = 0 OrElse defaultName Is Nothing OrElse defaultName.Length = 0 Then Throw New System.ArgumentException

			Me.prompt = prompt
			Me.defaultName = defaultName
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
		''' Get the default name.
		''' 
		''' <p>
		''' </summary>
		''' <returns> the default name, or null if this {@code NameCallback}
		'''          was not instantiated with a {@code defaultName}. </returns>
		Public Overridable Property defaultName As String
			Get
				Return defaultName
			End Get
		End Property

		''' <summary>
		''' Set the retrieved name.
		''' 
		''' <p>
		''' </summary>
		''' <param name="name"> the retrieved name (which may be null).
		''' </param>
		''' <seealso cref= #getName </seealso>
		Public Overridable Property name As String
			Set(ByVal name As String)
				Me.inputName = name
			End Set
			Get
				Return inputName
			End Get
		End Property

	End Class

End Namespace