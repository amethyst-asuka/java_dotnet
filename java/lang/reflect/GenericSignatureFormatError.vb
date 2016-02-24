'
' * Copyright (c) 2003, 2011, Oracle and/or its affiliates. All rights reserved.
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

Namespace java.lang.reflect


	''' <summary>
	''' Thrown when a syntactically malformed signature attribute is
	''' encountered by a reflective method that needs to interpret the
	''' generic signature information for a type, method or constructor.
	''' 
	''' @since 1.5
	''' </summary>
	Public Class GenericSignatureFormatError
		Inherits ClassFormatError

		Private Shadows Const serialVersionUID As Long = 6709919147137911034L

		''' <summary>
		''' Constructs a new {@code GenericSignatureFormatError}.
		''' 
		''' </summary>
		Public Sub New()
			MyBase.New()
		End Sub

		''' <summary>
		''' Constructs a new {@code GenericSignatureFormatError} with the
		''' specified message.
		''' </summary>
		''' <param name="message"> the detail message, may be {@code null} </param>
		Public Sub New(ByVal message As String)
			MyBase.New(message)
		End Sub
	End Class

End Namespace