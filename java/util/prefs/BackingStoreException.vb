Imports System

'
' * Copyright (c) 2000, 2003, Oracle and/or its affiliates. All rights reserved.
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

Namespace java.util.prefs


	''' <summary>
	''' Thrown to indicate that a preferences operation could not complete because
	''' of a failure in the backing store, or a failure to contact the backing
	''' store.
	''' 
	''' @author  Josh Bloch
	''' @since   1.4
	''' </summary>
	Public Class BackingStoreException
		Inherits Exception

		''' <summary>
		''' Constructs a BackingStoreException with the specified detail message.
		''' </summary>
		''' <param name="s"> the detail message. </param>
		Public Sub New(  s As String)
			MyBase.New(s)
		End Sub

		''' <summary>
		''' Constructs a BackingStoreException with the specified cause.
		''' </summary>
		''' <param name="cause"> the cause </param>
		Public Sub New(  cause As Throwable)
			MyBase.New(cause)
		End Sub

		Private Shadows Const serialVersionUID As Long = 859796500401108469L
	End Class

End Namespace