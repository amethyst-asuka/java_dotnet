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
	''' Thrown to indicate that the IP address of a host could not be determined.
	''' 
	''' @author  Jonathan Payne
	''' @since   JDK1.0
	''' </summary>
	Public Class UnknownHostException
		Inherits java.io.IOException

		Private Shadows Const serialVersionUID As Long = -4639126076052875403L

		''' <summary>
		''' Constructs a new {@code UnknownHostException} with the
		''' specified detail message.
		''' </summary>
		''' <param name="host">   the detail message. </param>
		Public Sub New(  host As String)
			MyBase.New(host)
		End Sub

		''' <summary>
		''' Constructs a new {@code UnknownHostException} with no detail
		''' message.
		''' </summary>
		Public Sub New()
		End Sub
	End Class

End Namespace