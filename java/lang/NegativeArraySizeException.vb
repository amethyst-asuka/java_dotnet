'
' * Copyright (c) 1994, 2008, Oracle and/or its affiliates. All rights reserved.
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

Namespace java.lang

	''' <summary>
	''' Thrown if an application tries to create an array with negative size.
	''' 
	''' @author  unascribed
	''' @since   JDK1.0
	''' </summary>
	Public Class NegativeArraySizeException
		Inherits RuntimeException

		Private Shadows Const serialVersionUID As Long = -8960118058596991861L

		''' <summary>
		''' Constructs a <code>NegativeArraySizeException</code> with no
		''' detail message.
		''' </summary>
		Public Sub New()
			MyBase.New()
		End Sub

		''' <summary>
		''' Constructs a <code>NegativeArraySizeException</code> with the
		''' specified detail message.
		''' </summary>
		''' <param name="s">   the detail message. </param>
		Public Sub New(  s As String)
			MyBase.New(s)
		End Sub
	End Class

End Namespace