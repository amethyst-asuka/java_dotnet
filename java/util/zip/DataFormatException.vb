Imports System

'
' * Copyright (c) 1996, 2008, Oracle and/or its affiliates. All rights reserved.
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

Namespace java.util.zip

	''' <summary>
	''' Signals that a data format error has occurred.
	''' 
	''' @author      David Connelly
	''' </summary>
	Public Class DataFormatException
		Inherits Exception

		Private Shadows Const serialVersionUID As Long = 2219632870893641452L

		''' <summary>
		''' Constructs a DataFormatException with no detail message.
		''' </summary>
		Public Sub New()
			MyBase.New()
		End Sub

		''' <summary>
		''' Constructs a DataFormatException with the specified detail message.
		''' A detail message is a String that describes this particular exception. </summary>
		''' <param name="s"> the String containing a detail message </param>
		Public Sub New(  s As String)
			MyBase.New(s)
		End Sub
	End Class

End Namespace