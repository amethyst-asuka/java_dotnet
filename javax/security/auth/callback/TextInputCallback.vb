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
	''' {@code TextInputCallback} to the {@code handle}
	''' method of a {@code CallbackHandler} to retrieve generic text
	''' information.
	''' </summary>
	''' <seealso cref= javax.security.auth.callback.CallbackHandler </seealso>
	<Serializable> _
	Public Class TextInputCallback
		Implements Callback

		Private Const serialVersionUID As Long = -8064222478852811804L

		''' <summary>
		''' @serial
		''' @since 1.4
		''' </summary>
		Private prompt As String
		''' <summary>
		''' @serial
		''' @since 1.4
		''' </summary>
		Private defaultText As String
		''' <summary>
		''' @serial
		''' @since 1.4
		''' </summary>
		Private inputText As String

		''' <summary>
		''' Construct a {@code TextInputCallback} with a prompt.
		''' 
		''' <p>
		''' </summary>
		''' <param name="prompt"> the prompt used to request the information.
		''' </param>
		''' <exception cref="IllegalArgumentException"> if {@code prompt} is null
		'''                  or if {@code prompt} has a length of 0. </exception>
		Public Sub New(ByVal prompt As String)
			If prompt Is Nothing OrElse prompt.Length = 0 Then Throw New System.ArgumentException
			Me.prompt = prompt
		End Sub

		''' <summary>
		''' Construct a {@code TextInputCallback} with a prompt
		''' and default input value.
		''' 
		''' <p>
		''' </summary>
		''' <param name="prompt"> the prompt used to request the information. <p>
		''' </param>
		''' <param name="defaultText"> the text to be used as the default text displayed
		'''                  with the prompt.
		''' </param>
		''' <exception cref="IllegalArgumentException"> if {@code prompt} is null,
		'''                  if {@code prompt} has a length of 0,
		'''                  if {@code defaultText} is null
		'''                  or if {@code defaultText} has a length of 0. </exception>
		Public Sub New(ByVal prompt As String, ByVal defaultText As String)
			If prompt Is Nothing OrElse prompt.Length = 0 OrElse defaultText Is Nothing OrElse defaultText.Length = 0 Then Throw New System.ArgumentException

			Me.prompt = prompt
			Me.defaultText = defaultText
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
		''' Get the default text.
		''' 
		''' <p>
		''' </summary>
		''' <returns> the default text, or null if this {@code TextInputCallback}
		'''          was not instantiated with {@code defaultText}. </returns>
		Public Overridable Property defaultText As String
			Get
				Return defaultText
			End Get
		End Property

		''' <summary>
		''' Set the retrieved text.
		''' 
		''' <p>
		''' </summary>
		''' <param name="text"> the retrieved text, which may be null.
		''' </param>
		''' <seealso cref= #getText </seealso>
		Public Overridable Property text As String
			Set(ByVal text As String)
				Me.inputText = text
			End Set
			Get
				Return inputText
			End Get
		End Property

	End Class

End Namespace