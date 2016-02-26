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
	''' does not terminate within the specified time limit.
	''' This can happen, for example, if the user specifies that
	''' the method should take no longer than 10 seconds, and the
	''' method fails to complete with 10 seconds.
	''' 
	''' <p> Synchronization and serialization issues that apply to NamingException
	''' apply directly here.
	''' 
	''' @author Rosanna Lee
	''' @author Scott Seligman
	''' 
	''' @since 1.3
	''' </summary>
	Public Class TimeLimitExceededException
		Inherits LimitExceededException

		''' <summary>
		''' Constructs a new instance of TimeLimitExceededException.
		''' All fields default to null.
		''' </summary>
		Public Sub New()
			MyBase.New()
		End Sub

		''' <summary>
		''' Constructs a new instance of TimeLimitExceededException
		''' using the argument supplied. </summary>
		''' <param name="explanation"> possibly null detail about this exception. </param>
		''' <seealso cref= java.lang.Throwable#getMessage </seealso>
		Public Sub New(ByVal explanation As String)
			MyBase.New(explanation)
		End Sub

		''' <summary>
		''' Use serialVersionUID from JNDI 1.1.1 for interoperability
		''' </summary>
		Private Const serialVersionUID As Long = -3597009011385034696L
	End Class

End Namespace