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
	''' This exception is thrown when attempting to access
	''' an attribute that does not exist.
	''' <p>
	''' Synchronization and serialization issues that apply to NamingException
	''' apply directly here.
	'''  
	''' @author Rosanna Lee
	''' @author Scott Seligman
	''' @since 1.3
	''' </summary>

	Public Class NoSuchAttributeException
		Inherits javax.naming.NamingException

		''' <summary>
		''' Constructs a new instance of NoSuchAttributeException using
		''' an explanation. All other fields are set to null. </summary>
		''' <param name="explanation">     Additional detail about this exception. Can be null. </param>
		''' <seealso cref= java.lang.Throwable#getMessage </seealso>
		Public Sub New(ByVal explanation As String)
			MyBase.New(explanation)
		End Sub


		''' <summary>
		''' Constructs a new instance of NoSuchAttributeException.
		''' All fields are initialized to null.
		''' </summary>
		Public Sub New()
			MyBase.New()
		End Sub

		''' <summary>
		''' Use serialVersionUID from JNDI 1.1.1 for interoperability
		''' </summary>
		Private Const serialVersionUID As Long = 4836415647935888137L
	End Class

End Namespace