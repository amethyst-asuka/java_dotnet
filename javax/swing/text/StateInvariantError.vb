Imports System

'
' * Copyright (c) 1997, 1998, Oracle and/or its affiliates. All rights reserved.
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
Namespace javax.swing.text

	''' <summary>
	''' This exception is to report the failure of state invarient
	''' assertion that was made.  This indicates an internal error
	''' has occurred.
	''' 
	''' @author  Timothy Prinzing
	''' </summary>
	Friend Class StateInvariantError
		Inherits Exception

		''' <summary>
		''' Creates a new StateInvariantFailure object.
		''' </summary>
		''' <param name="s">         a string indicating the assertion that failed </param>
		Public Sub New(ByVal s As String)
			MyBase.New(s)
		End Sub

	End Class

End Namespace