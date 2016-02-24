Imports System

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

Namespace java.security.acl

	''' <summary>
	''' This is an exception that is thrown whenever the modification of an object
	''' (such as an Access Control List) is only allowed to be done by an owner of
	''' the object, but the Principal attempting the modification is not an owner.
	''' 
	''' @author      Satish Dharmaraj
	''' </summary>
	Public Class NotOwnerException
		Inherits Exception

		Private Shadows Const serialVersionUID As Long = -5555597911163362399L

		''' <summary>
		''' Constructs a NotOwnerException.
		''' </summary>
		Public Sub New()
		End Sub
	End Class

End Namespace