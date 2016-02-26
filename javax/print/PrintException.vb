Imports System

'
' * Copyright (c) 2000, Oracle and/or its affiliates. All rights reserved.
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

Namespace javax.print

	''' <summary>
	''' Class PrintException encapsulates a printing-related error condition that
	''' occurred while using a Print Service instance. This base class
	''' furnishes only a string description of the error. Subclasses furnish more
	''' detailed information if applicable.
	''' 
	''' </summary>
	Public Class PrintException
		Inherits Exception


		''' <summary>
		''' Construct a print exception with no detail message.
		''' </summary>
		Public Sub New()
			MyBase.New()
		End Sub

		''' <summary>
		''' Construct a print exception with the given detail message.
		''' </summary>
		''' <param name="s">  Detail message, or null if no detail message. </param>
		Public Sub New(ByVal s As String)
			MyBase.New(s)
		End Sub

		''' <summary>
		''' Construct a print exception chaining the supplied exception.
		''' </summary>
		''' <param name="e">  Chained exception. </param>
		Public Sub New(ByVal e As Exception)
			MyBase.New(e)
		End Sub

		''' <summary>
		''' Construct a print exception with the given detail message
		''' and chained exception. </summary>
		''' <param name="s">  Detail message, or null if no detail message. </param>
		''' <param name="e">  Chained exception. </param>
		Public Sub New(ByVal s As String, ByVal e As Exception)
			MyBase.New(s, e)
		End Sub

	End Class

End Namespace