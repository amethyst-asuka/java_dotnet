'
' * Copyright (c) 1995, 2008, Oracle and/or its affiliates. All rights reserved.
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
	''' Thrown when the "verifier" detects that a class file,
	''' though well formed, contains some sort of internal inconsistency
	''' or security problem.
	''' 
	''' @author  unascribed
	''' @since   JDK1.0
	''' </summary>
	Public Class VerifyError
		Inherits LinkageError

		Private Shadows Const serialVersionUID As Long = 7001962396098498785L

		''' <summary>
		''' Constructs an <code>VerifyError</code> with no detail message.
		''' </summary>
		Public Sub New()
			MyBase.New()
		End Sub

		''' <summary>
		''' Constructs an <code>VerifyError</code> with the specified detail message.
		''' </summary>
		''' <param name="s">   the detail message. </param>
		Public Sub New(ByVal s As String)
			MyBase.New(s)
		End Sub
	End Class

End Namespace