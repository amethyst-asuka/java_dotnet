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
	''' This exception is thrown when an attempt is
	''' made to add to create an attribute with an invalid attribute identifier.
	''' The validity of an attribute identifier is directory-specific.
	''' <p>
	''' Synchronization and serialization issues that apply to NamingException
	''' apply directly here.
	'''  
	''' @author Rosanna Lee
	''' @author Scott Seligman
	''' @since 1.3
	''' </summary>

	Public Class InvalidAttributeIdentifierException
		Inherits javax.naming.NamingException

		''' <summary>
		''' Constructs a new instance of InvalidAttributeIdentifierException using the
		''' explanation supplied. All other fields set to null. </summary>
		''' <param name="explanation">     Possibly null string containing additional detail about this exception. </param>
		''' <seealso cref= java.lang.Throwable#getMessage </seealso>
		Public Sub New(ByVal explanation As String)
			MyBase.New(explanation)
		End Sub

		''' <summary>
		''' Constructs a new instance of InvalidAttributeIdentifierException.
		''' All fields are set to null.
		''' </summary>
		Public Sub New()
			MyBase.New()
		End Sub

		''' <summary>
		''' Use serialVersionUID from JNDI 1.1.1 for interoperability
		''' </summary>
		Private Const serialVersionUID As Long = -9036920266322999923L
	End Class

End Namespace