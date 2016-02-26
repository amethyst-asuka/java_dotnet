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
	''' This exception is thrown when there is a configuration problem.
	''' This can arise when installation of a provider was
	''' not done correctly, or if there are configuration problems with the
	''' server, or if configuration information required to access
	''' the provider or service is malformed or missing.
	''' For example, a request to use SSL as the security protocol when
	''' the service provider software was not configured with the SSL
	''' component would cause such an exception. Another example is
	''' if the provider requires that a URL be specified as one of the
	''' environment properties but the client failed to provide it.
	''' <p>
	''' Synchronization and serialization issues that apply to NamingException
	''' apply directly here.
	'''  
	''' @author Rosanna Lee
	''' @author Scott Seligman
	''' @since 1.3
	''' </summary>
	Public Class ConfigurationException
		Inherits NamingException

		''' <summary>
		''' Constructs a new instance of ConfigurationException using an
		''' explanation. All other fields default to null.
		''' </summary>
		''' <param name="explanation">     A possibly null string containing
		'''                          additional detail about this exception. </param>
		''' <seealso cref= java.lang.Throwable#getMessage </seealso>
		Public Sub New(ByVal explanation As String)
			MyBase.New(explanation)
		End Sub

		''' <summary>
		''' Constructs a new instance of ConfigurationException with
		''' all name resolution fields and explanation initialized to null.
		''' </summary>
		Public Sub New()
			MyBase.New()
		End Sub

		''' <summary>
		''' Use serialVersionUID from JNDI 1.1.1 for interoperability
		''' </summary>
		Private Const serialVersionUID As Long = -2535156726228855704L
	End Class

End Namespace