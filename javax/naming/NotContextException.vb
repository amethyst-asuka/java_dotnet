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
	''' This exception is thrown when a naming operation proceeds to a point
	''' where a context is required to continue the operation, but the
	''' resolved object is not a context. For example, Context.destroy() requires
	''' that the named object be a context. If it is not, NotContextException
	''' is thrown. Another example is a non-context being encountered during
	''' the resolution phase of the Context methods.
	''' <p>
	''' It is also thrown when a particular subtype of context is required,
	''' such as a DirContext, and the resolved object is a context but not of
	''' the required subtype.
	''' <p>
	''' Synchronization and serialization issues that apply to NamingException
	''' apply directly here. </summary>
	''' <seealso cref= Context#destroySubcontext
	'''  
	''' @author Rosanna Lee
	''' @author Scott Seligman
	''' @since 1.3 </seealso>

	Public Class NotContextException
		Inherits NamingException

		''' <summary>
		''' Constructs a new instance of NotContextException using an
		''' explanation. All other fields default to null.
		''' </summary>
		''' <param name="explanation">     Possibly null additional detail about this exception. </param>
		''' <seealso cref= java.lang.Throwable#getMessage </seealso>
		Public Sub New(ByVal explanation As String)
			MyBase.New(explanation)
		End Sub

		''' <summary>
		''' Constructs a new instance of NotContextException.
		''' All fields default to null.
		''' </summary>
		Public Sub New()
			MyBase.New()
		End Sub

		''' <summary>
		''' Use serialVersionUID from JNDI 1.1.1 for interoperability
		''' </summary>
		Private Const serialVersionUID As Long = 849752551644540417L
	End Class

End Namespace