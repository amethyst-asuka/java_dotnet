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
	''' This exception is thrown when attempting to destroy a context that
	''' is not empty.
	''' <p>
	''' If the program wants to handle this exception in particular, it
	''' should catch ContextNotEmptyException explicitly before attempting to
	''' catch NamingException. For example, after catching ContextNotEmptyException,
	''' the program might try to remove the contents of the context before
	''' reattempting the destroy.
	''' <p>
	''' Synchronization and serialization issues that apply to NamingException
	''' apply directly here.
	'''  
	''' @author Rosanna Lee
	''' @author Scott Seligman
	''' </summary>
	''' <seealso cref= Context#destroySubcontext
	''' @since 1.3 </seealso>
	Public Class ContextNotEmptyException
		Inherits NamingException

		''' <summary>
		''' Constructs a new instance of ContextNotEmptyException using an
		''' explanation. All other fields default to null.
		''' </summary>
		''' <param name="explanation">     Possibly null string containing
		''' additional detail about this exception. </param>
		''' <seealso cref= java.lang.Throwable#getMessage </seealso>
		Public Sub New(ByVal explanation As String)
			MyBase.New(explanation)
		End Sub

		''' <summary>
		''' Constructs a new instance of ContextNotEmptyException with
		''' all name resolution fields and explanation initialized to null.
		''' </summary>
		Public Sub New()
			MyBase.New()
		End Sub

		''' <summary>
		''' Use serialVersionUID from JNDI 1.1.1 for interoperability
		''' </summary>
		Private Const serialVersionUID As Long = 1090963683348219877L
	End Class

End Namespace