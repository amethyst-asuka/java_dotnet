'
' * Copyright (c) 1999, 2001, Oracle and/or its affiliates. All rights reserved.
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
	''' This exception is thrown when attempting to communicate with a
	''' directory or naming service and that service is not available.
	''' It might be unavailable for different reasons. For example,
	''' the server might be too busy to service the request, or the server
	''' might not be registered to service any requests, etc.
	''' <p>
	''' Synchronization and serialization issues that apply to NamingException
	''' apply directly here.
	'''  
	''' @author Rosanna Lee
	''' @author Scott Seligman
	'''  
	''' @since 1.3
	''' </summary>

	Public Class ServiceUnavailableException
		Inherits NamingException

		''' <summary>
		''' Constructs a new instance of ServiceUnavailableException using an
		''' explanation. All other fields default to null.
		''' </summary>
		''' <param name="explanation">     Possibly null additional detail about this exception. </param>
		''' <seealso cref= java.lang.Throwable#getMessage </seealso>
		Public Sub New(ByVal explanation As String)
			MyBase.New(explanation)
		End Sub

		''' <summary>
		''' Constructs a new instance of ServiceUnavailableException.
		''' All fields default to null.
		''' </summary>
		Public Sub New()
			MyBase.New()
		End Sub

		''' <summary>
		''' Use serialVersionUID from JNDI 1.1.1 for interoperability
		''' </summary>
		Private Const serialVersionUID As Long = -4996964726566773444L
	End Class

End Namespace