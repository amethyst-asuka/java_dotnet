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
	''' This exception is thrown when no initial context implementation
	''' can be created.  The policy of how an initial context implementation
	''' is selected is described in the documentation of the InitialContext class.
	''' <p>
	''' This exception can be thrown during any interaction with the
	''' InitialContext, not only when the InitialContext is constructed.
	''' For example, the implementation of the initial context might lazily
	''' retrieve the context only when actual methods are invoked on it.
	''' The application should not have any dependency on when the existence
	''' of an initial context is determined.
	''' <p>
	''' Synchronization and serialization issues that apply to NamingException
	''' apply directly here.
	'''  
	''' @author Rosanna Lee
	''' @author Scott Seligman
	''' </summary>
	''' <seealso cref= InitialContext </seealso>
	''' <seealso cref= javax.naming.directory.InitialDirContext </seealso>
	''' <seealso cref= javax.naming.spi.NamingManager#getInitialContext </seealso>
	''' <seealso cref= javax.naming.spi.NamingManager#setInitialContextFactoryBuilder
	''' @since 1.3 </seealso>
	Public Class NoInitialContextException
		Inherits NamingException

		''' <summary>
		''' Constructs an instance of NoInitialContextException.
		''' All fields are initialized to null.
		''' </summary>
		Public Sub New()
			MyBase.New()
		End Sub

		''' <summary>
		''' Constructs an instance of NoInitialContextException with an
		''' explanation. All other fields are initialized to null. </summary>
		''' <param name="explanation">     Possibly null additional detail about this exception. </param>
		''' <seealso cref= java.lang.Throwable#getMessage </seealso>
		Public Sub New(ByVal explanation As String)
			MyBase.New(explanation)
		End Sub

		''' <summary>
		''' Use serialVersionUID from JNDI 1.1.1 for interoperability
		''' </summary>
		Private Const serialVersionUID As Long = -3413733186901258623L
	End Class

End Namespace