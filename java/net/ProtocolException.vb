'
' * Copyright (c) 1995, 2013, Oracle and/or its affiliates. All rights reserved.
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
	''' Thrown to indicate that there is an error in the underlying
	''' protocol, such as a TCP error.
	''' 
	''' @author  Chris Warth
	''' @since   JDK1.0
	''' </summary>
	Public Class ProtocolException
		Inherits java.io.IOException

		Private Shadows Const serialVersionUID As Long = -6098449442062388080L

		''' <summary>
		''' Constructs a new {@code ProtocolException} with the
		''' specified detail message.
		''' </summary>
		''' <param name="host">   the detail message. </param>
		Public Sub New(ByVal host As String)
			MyBase.New(host)
		End Sub

		''' <summary>
		''' Constructs a new {@code ProtocolException} with no detail message.
		''' </summary>
		Public Sub New()
		End Sub
	End Class

End Namespace