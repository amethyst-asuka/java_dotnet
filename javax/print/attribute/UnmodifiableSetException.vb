Imports System

'
' * Copyright (c) 2001, 2005, Oracle and/or its affiliates. All rights reserved.
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

Namespace javax.print.attribute

	''' <summary>
	''' Thrown to indicate that the requested operation cannot be performed
	''' because the set is unmodifiable.
	''' 
	''' @author  Phil Race
	''' @since   1.4
	''' </summary>
	Public Class UnmodifiableSetException
		Inherits Exception

		''' <summary>
		''' Constructs an UnsupportedOperationException with no detail message.
		''' </summary>
		Public Sub New()
		End Sub

		''' <summary>
		''' Constructs an UnmodifiableSetException with the specified
		''' detail message.
		''' </summary>
		''' <param name="message"> the detail message </param>
		Public Sub New(ByVal message As String)
			MyBase.New(message)
		End Sub
	End Class

End Namespace