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
	''' Thrown to indicate that a malformed URL has occurred. Either no
	''' legal protocol could be found in a specification string or the
	''' string could not be parsed.
	''' 
	''' @author  Arthur van Hoff
	''' @since   JDK1.0
	''' </summary>
	Public Class MalformedURLException
		Inherits java.io.IOException

		Private Shadows Const serialVersionUID As Long = -182787522200415866L

		''' <summary>
		''' Constructs a {@code MalformedURLException} with no detail message.
		''' </summary>
		Public Sub New()
		End Sub

		''' <summary>
		''' Constructs a {@code MalformedURLException} with the
		''' specified detail message.
		''' </summary>
		''' <param name="msg">   the detail message. </param>
		Public Sub New(ByVal msg As String)
			MyBase.New(msg)
		End Sub
	End Class

End Namespace