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


Namespace javax.naming.directory


	''' <summary>
	''' This exception is thrown when the specification of
	''' a search filter is invalid.  The expression of the filter may
	''' be invalid, or there may be a problem with one of the parameters
	''' passed to the filter.
	''' <p>
	''' Synchronization and serialization issues that apply to NamingException
	''' apply directly here.
	'''  
	''' @author Rosanna Lee
	''' @author Scott Seligman
	''' @since 1.3
	''' </summary>
	Public Class InvalidSearchFilterException
		Inherits javax.naming.NamingException

		''' <summary>
		''' Constructs a new instance of InvalidSearchFilterException.
		''' All fields are set to null.
		''' </summary>
		Public Sub New()
			MyBase.New()
		End Sub

		''' <summary>
		''' Constructs a new instance of InvalidSearchFilterException
		''' with an explanation. All other fields are set to null. </summary>
		''' <param name="msg"> Detail about this exception. Can be null. </param>
		''' <seealso cref= java.lang.Throwable#getMessage </seealso>
		Public Sub New(ByVal msg As String)
			MyBase.New(msg)
		End Sub

		''' <summary>
		''' Use serialVersionUID from JNDI 1.1.1 for interoperability
		''' </summary>
		Private Const serialVersionUID As Long = 2902700940682875441L
	End Class

End Namespace