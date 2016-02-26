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
	''' This exception is thrown when a method
	''' terminates abnormally due to a user or system specified limit.
	''' This is different from a InsufficientResourceException in that
	''' LimitExceededException is due to a user/system specified limit.
	''' For example, running out of memory to complete the request would
	''' be an insufficient resource. The client asking for 10 answers and
	''' getting back 11 is a size limit exception.
	''' <p>
	''' Examples of these limits include client and server configuration
	''' limits such as size, time, number of hops, etc.
	''' <p>
	''' Synchronization and serialization issues that apply to NamingException
	''' apply directly here.
	'''  
	''' @author Rosanna Lee
	''' @author Scott Seligman
	''' @since 1.3
	''' </summary>

	Public Class LimitExceededException
		Inherits NamingException

		''' <summary>
		''' Constructs a new instance of LimitExceededException with
		''' all name resolution fields and explanation initialized to null.
		''' </summary>
		Public Sub New()
			MyBase.New()
		End Sub

		''' <summary>
		''' Constructs a new instance of LimitExceededException using an
		''' explanation. All other fields default to null. </summary>
		''' <param name="explanation"> Possibly null detail about this exception. </param>
		''' <seealso cref= java.lang.Throwable#getMessage </seealso>
		Public Sub New(ByVal explanation As String)
			MyBase.New(explanation)
		End Sub

		''' <summary>
		''' Use serialVersionUID from JNDI 1.1.1 for interoperability
		''' </summary>
		Private Const serialVersionUID As Long = -776898738660207856L
	End Class

End Namespace