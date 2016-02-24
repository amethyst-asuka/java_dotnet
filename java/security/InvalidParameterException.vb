'
' * Copyright (c) 1996, 2003, Oracle and/or its affiliates. All rights reserved.
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

Namespace java.security

	''' <summary>
	''' This exception, designed for use by the JCA/JCE engine classes,
	''' is thrown when an invalid parameter is passed
	''' to a method.
	''' 
	''' @author Benjamin Renaud
	''' </summary>

	Public Class InvalidParameterException
		Inherits IllegalArgumentException

		Private Shadows Const serialVersionUID As Long = -857968536935667808L

		''' <summary>
		''' Constructs an InvalidParameterException with no detail message.
		''' A detail message is a String that describes this particular
		''' exception.
		''' </summary>
		Public Sub New()
			MyBase.New()
		End Sub

		''' <summary>
		''' Constructs an InvalidParameterException with the specified
		''' detail message.  A detail message is a String that describes
		''' this particular exception.
		''' </summary>
		''' <param name="msg"> the detail message. </param>
		Public Sub New(ByVal msg As String)
			MyBase.New(msg)
		End Sub
	End Class

End Namespace