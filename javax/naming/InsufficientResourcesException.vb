'
' * Copyright (c) 1999, 2013, Oracle and/or its affiliates. All rights reserved.
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
	''' This exception is thrown when resources are not available to complete
	''' the requested operation. This might due to a lack of resources on
	''' the server or on the client. There are no restrictions to resource types,
	''' as different services might make use of different resources. Such
	''' restrictions might be due to physical limits and/or administrative quotas.
	''' Examples of limited resources are internal buffers, memory, network bandwidth.
	''' <p>
	''' InsufficientResourcesException is different from LimitExceededException in that
	''' the latter is due to user/system specified limits. See LimitExceededException
	''' for details.
	''' <p>
	''' Synchronization and serialization issues that apply to NamingException
	''' apply directly here.
	'''  
	''' @author Rosanna Lee
	''' @author Scott Seligman
	''' @since 1.3
	''' </summary>

	Public Class InsufficientResourcesException
		Inherits NamingException

		''' <summary>
		''' Constructs a new instance of InsufficientResourcesException using an
		''' explanation. All other fields default to null.
		''' </summary>
		''' <param name="explanation">     Possibly null additional detail about this exception. </param>
		''' <seealso cref= java.lang.Throwable#getMessage </seealso>
		Public Sub New(ByVal explanation As String)
			MyBase.New(explanation)
		End Sub

		''' <summary>
		''' Constructs a new instance of InsufficientResourcesException with
		''' all name resolution fields and explanation initialized to null.
		''' </summary>
		Public Sub New()
			MyBase.New()
		End Sub

		''' <summary>
		''' Use serialVersionUID from JNDI 1.1.1 for interoperability
		''' </summary>
		Private Const serialVersionUID As Long = 6227672693037844532L
	End Class

End Namespace