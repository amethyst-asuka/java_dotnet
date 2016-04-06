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
	''' Thrown to indicate that an unknown service exception has
	''' occurred. Either the MIME type returned by a URL connection does
	''' not make sense, or the application is attempting to write to a
	''' read-only URL connection.
	''' 
	''' @author  unascribed
	''' @since   JDK1.0
	''' </summary>
	Public Class UnknownServiceException
		Inherits java.io.IOException

		Private Shadows Const serialVersionUID As Long = -4169033248853639508L

		''' <summary>
		''' Constructs a new {@code UnknownServiceException} with no
		''' detail message.
		''' </summary>
		Public Sub New()
		End Sub

		''' <summary>
		''' Constructs a new {@code UnknownServiceException} with the
		''' specified detail message.
		''' </summary>
		''' <param name="msg">   the detail message. </param>
		Public Sub New(  msg As String)
			MyBase.New(msg)
		End Sub
	End Class

End Namespace