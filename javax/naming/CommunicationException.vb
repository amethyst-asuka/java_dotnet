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
	''' This exception is thrown when the client is
	''' unable to communicate with the directory or naming service.
	''' The inability to communicate with the service might be a result
	''' of many factors, such as network partitioning, hardware or interface problems,
	''' failures on either the client or server side.
	''' This exception is meant to be used to capture such communication problems.
	''' <p>
	''' Synchronization and serialization issues that apply to NamingException
	''' apply directly here.
	'''  
	''' @author Rosanna Lee
	''' @author Scott Seligman
	''' @since 1.3
	''' </summary>
	Public Class CommunicationException
		Inherits NamingException

		''' <summary>
		''' Constructs a new instance of CommunicationException using the
		''' arguments supplied.
		''' </summary>
		''' <param name="explanation">     Additional detail about this exception. </param>
		''' <seealso cref= java.lang.Throwable#getMessage </seealso>
		Public Sub New(ByVal explanation As String)
			MyBase.New(explanation)
		End Sub

		''' <summary>
		''' Constructs a new instance of CommunicationException.
		''' </summary>
		Public Sub New()
			MyBase.New()
		End Sub

		''' <summary>
		''' Use serialVersionUID from JNDI 1.1.1 for interoperability
		''' </summary>
		Private Const serialVersionUID As Long = 3618507780299986611L
	End Class

End Namespace