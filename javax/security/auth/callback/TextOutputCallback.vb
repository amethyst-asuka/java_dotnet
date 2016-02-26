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
	''' {@code TextOutputCallback} to the {@code handle}
	''' method of a {@code CallbackHandler} to display information messages,
	''' warning messages and error messages.
	''' </summary>
	''' <seealso cref= javax.security.auth.callback.CallbackHandler </seealso>
	<Serializable> _
	Public Class TextOutputCallback
		Implements Callback

		Private Const serialVersionUID As Long = 1689502495511663102L

		''' <summary>
		''' Information message. </summary>
		Public Const INFORMATION As Integer = 0
		''' <summary>
		''' Warning message. </summary>
		Public Const WARNING As Integer = 1
		''' <summary>
		''' Error message. </summary>
		Public Const [ERROR] As Integer = 2

		''' <summary>
		''' @serial
		''' @since 1.4
		''' </summary>
		Private messageType As Integer
		''' <summary>
		''' @serial
		''' @since 1.4
		''' </summary>
		Private message As String

		''' <summary>
		''' Construct a TextOutputCallback with a message type and message
		''' to be displayed.
		''' 
		''' <p>
		''' </summary>
		''' <param name="messageType"> the message type ({@code INFORMATION},
		'''                  {@code WARNING} or {@code ERROR}). <p>
		''' </param>
		''' <param name="message"> the message to be displayed. <p>
		''' </param>
		''' <exception cref="IllegalArgumentException"> if {@code messageType}
		'''                  is not either {@code INFORMATION},
		'''                  {@code WARNING} or {@code ERROR},
		'''                  if {@code message} is null,
		'''                  or if {@code message} has a length of 0. </exception>
		Public Sub New(ByVal messageType As Integer, ByVal message As String)
			If (messageType <> INFORMATION AndAlso messageType <> WARNING AndAlso messageType <> [ERROR]) OrElse message Is Nothing OrElse message.Length = 0 Then Throw New System.ArgumentException

			Me.messageType = messageType
			Me.message = message
		End Sub

		''' <summary>
		''' Get the message type.
		''' 
		''' <p>
		''' </summary>
		''' <returns> the message type ({@code INFORMATION},
		'''                  {@code WARNING} or {@code ERROR}). </returns>
		Public Overridable Property messageType As Integer
			Get
				Return messageType
			End Get
		End Property

		''' <summary>
		''' Get the message to be displayed.
		''' 
		''' <p>
		''' </summary>
		''' <returns> the message to be displayed. </returns>
		Public Overridable Property message As String
			Get
				Return message
			End Get
		End Property
	End Class

End Namespace