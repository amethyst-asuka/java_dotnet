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
	''' This exception is thrown when
	''' a loop was detected will attempting to resolve a link, or an implementation
	''' specific limit on link counts has been reached.
	''' <p>
	''' Synchronization and serialization issues that apply to LinkException
	''' apply directly here.
	'''  
	''' @author Rosanna Lee
	''' @author Scott Seligman
	''' </summary>
	''' <seealso cref= LinkRef
	''' @since 1.3 </seealso>

	Public Class LinkLoopException
		Inherits LinkException

		''' <summary>
		''' Constructs a new instance of LinkLoopException with an explanation
		''' All the other fields are initialized to null. </summary>
		''' <param name="explanation">     A possibly null string containing additional
		'''                         detail about this exception. </param>
		''' <seealso cref= java.lang.Throwable#getMessage </seealso>
		Public Sub New(ByVal explanation As String)
			MyBase.New(explanation)
		End Sub

		''' <summary>
		''' Constructs a new instance of LinkLoopException.
		''' All the non-link-related and link-related fields are initialized to null.
		''' </summary>
		Public Sub New()
			MyBase.New()
		End Sub

		''' <summary>
		''' Use serialVersionUID from JNDI 1.1.1 for interoperability
		''' </summary>
		Private Const serialVersionUID As Long = -3119189944325198009L
	End Class

End Namespace