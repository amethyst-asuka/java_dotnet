'
' * Copyright (c) 1999, Oracle and/or its affiliates. All rights reserved.
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

Namespace javax.naming

	''' <summary>
	''' This exception is thrown when attempting to perform an operation
	''' for which the client has no permission. The access control/permission
	''' model is dictated by the directory/naming server.
	'''  
	''' <p>
	''' Synchronization and serialization issues that apply to NamingException
	''' apply directly here.
	'''  
	''' @author Rosanna Lee
	''' @author Scott Seligman
	''' @since 1.3
	''' </summary>

	Public Class NoPermissionException
		Inherits NamingSecurityException

		''' <summary>
		''' Constructs a new instance of NoPermissionException using an
		''' explanation. All other fields default to null.
		''' </summary>
		''' <param name="explanation">     Possibly null additional detail about this exception. </param>
		Public Sub New(ByVal explanation As String)
			MyBase.New(explanation)
		End Sub

		''' <summary>
		''' Constructs a new instance of NoPermissionException.
		''' All fields are initialized to null.
		''' </summary>
		Public Sub New()
			MyBase.New()
		End Sub
		''' <summary>
		''' Use serialVersionUID from JNDI 1.1.1 for interoperability
		''' </summary>
		Private Const serialVersionUID As Long = 8395332708699751775L
	End Class

End Namespace