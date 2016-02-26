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
	''' the particular flavor of authentication requested is not supported.
	''' For example, if the program
	''' is attempting to use strong authentication but the directory/naming
	''' supports only simple authentication, this exception would be thrown.
	''' Identification of a particular flavor of authentication is
	''' provider- and server-specific. It may be specified using
	''' specific authentication schemes such
	''' those identified using SASL, or a generic authentication specifier
	''' (such as "simple" and "strong").
	''' <p>
	''' If the program wants to handle this exception in particular, it
	''' should catch AuthenticationNotSupportedException explicitly before
	''' attempting to catch NamingException. After catching
	''' <code>AuthenticationNotSupportedException</code>, the program could
	''' reattempt the authentication using a different authentication flavor
	''' by updating the resolved context's environment properties accordingly.
	''' <p>
	''' Synchronization and serialization issues that apply to NamingException
	''' apply directly here.
	'''  
	''' @author Rosanna Lee
	''' @author Scott Seligman
	''' @since 1.3
	''' </summary>

	Public Class AuthenticationNotSupportedException
		Inherits NamingSecurityException

		''' <summary>
		''' Constructs a new instance of AuthenticationNotSupportedException using
		''' an explanation. All other fields default to null.
		''' </summary>
		''' <param name="explanation">     A possibly null string containing additional
		'''                          detail about this exception. </param>
		''' <seealso cref= java.lang.Throwable#getMessage </seealso>
		Public Sub New(ByVal explanation As String)
			MyBase.New(explanation)
		End Sub

		''' <summary>
		''' Constructs a new instance of AuthenticationNotSupportedException
		''' all name resolution fields and explanation initialized to null.
		''' </summary>
		Public Sub New()
			MyBase.New()
		End Sub

		''' <summary>
		''' Use serialVersionUID from JNDI 1.1.1 for interoperability
		''' </summary>
		Private Const serialVersionUID As Long = -7149033933259492300L
	End Class

End Namespace