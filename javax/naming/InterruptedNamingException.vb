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
	''' This exception is thrown when the naming operation
	''' being invoked has been interrupted. For example, an application
	''' might interrupt a thread that is performing a search. If the
	''' search supports being interrupted, it will throw
	''' InterruptedNamingException. Whether an operation is interruptible
	''' and when depends on its implementation (as provided by the
	''' service providers). Different implementations have different ways
	''' of protecting their resources and objects from being damaged
	''' due to unexpected interrupts.
	''' <p>
	''' Synchronization and serialization issues that apply to NamingException
	''' apply directly here.
	'''  
	''' @author Rosanna Lee
	''' @author Scott Seligman
	''' </summary>
	''' <seealso cref= Context </seealso>
	''' <seealso cref= javax.naming.directory.DirContext </seealso>
	''' <seealso cref= java.lang.Thread#interrupt </seealso>
	''' <seealso cref= java.lang.InterruptedException
	''' @since 1.3 </seealso>

	Public Class InterruptedNamingException
		Inherits NamingException

		''' <summary>
		''' Constructs an instance of InterruptedNamingException using an
		''' explanation of the problem.
		''' All name resolution-related fields are initialized to null. </summary>
		''' <param name="explanation">      A possibly null message explaining the problem. </param>
		''' <seealso cref= java.lang.Throwable#getMessage </seealso>
		Public Sub New(ByVal explanation As String)
			MyBase.New(explanation)
		End Sub

		''' <summary>
		''' Constructs an instance of InterruptedNamingException with
		''' all name resolution fields and explanation initialized to null.
		''' </summary>
		Public Sub New()
			MyBase.New()
		End Sub

		''' <summary>
		''' Use serialVersionUID from JNDI 1.1.1 for interoperability
		''' </summary>
		Private Const serialVersionUID As Long = 6404516648893194728L
	End Class

End Namespace