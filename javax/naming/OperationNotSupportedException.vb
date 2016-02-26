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
	''' This exception is thrown when a context implementation does not support
	''' the operation being invoked.
	''' For example, if a server does not support the Context.bind() method
	''' it would throw OperationNotSupportedException when the bind() method
	''' is invoked on it.
	''' <p>
	''' Synchronization and serialization issues that apply to NamingException
	''' apply directly here.
	'''  
	''' @author Rosanna Lee
	''' @author Scott Seligman
	''' @since 1.3
	''' </summary>

	Public Class OperationNotSupportedException
		Inherits NamingException

		''' <summary>
		''' Constructs a new instance of OperationNotSupportedException.
		''' All fields default to null.
		''' </summary>
		Public Sub New()
			MyBase.New()
		End Sub

		''' <summary>
		''' Constructs a new instance of OperationNotSupportedException using an
		''' explanation. All other fields default to null.
		''' </summary>
		''' <param name="explanation">     Possibly null additional detail about this exception </param>
		''' <seealso cref= java.lang.Throwable#getMessage </seealso>
		Public Sub New(ByVal explanation As String)
			MyBase.New(explanation)
		End Sub

		''' <summary>
		''' Use serialVersionUID from JNDI 1.1.1 for interoperability
		''' </summary>
		Private Const serialVersionUID As Long = 5493232822427682064L
	End Class

End Namespace