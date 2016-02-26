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
	''' made to add or modify an attribute set that has been specified
	''' incompletely or incorrectly. This could happen, for example,
	''' when attempting to add or modify a binding, or to create a new
	''' subcontext without specifying all the mandatory attributes
	''' required for creation of the object.  Another situation in
	''' which this exception is thrown is by specification of incompatible
	''' attributes within the same attribute set, or attributes in conflict
	''' with that specified by the object's schema.
	''' <p>
	''' Synchronization and serialization issues that apply to NamingException
	''' apply directly here.
	'''  
	''' @author Rosanna Lee
	''' @author Scott Seligman
	''' @since 1.3
	''' </summary>

	Public Class InvalidAttributesException
		Inherits javax.naming.NamingException

		''' <summary>
		''' Constructs a new instance of InvalidAttributesException using an
		''' explanation. All other fields are set to null. </summary>
		''' <param name="explanation">     Additional detail about this exception. Can be null. </param>
		''' <seealso cref= java.lang.Throwable#getMessage </seealso>
		Public Sub New(ByVal explanation As String)
			MyBase.New(explanation)
		End Sub

		''' <summary>
		''' Constructs a new instance of InvalidAttributesException.
		''' All fields are set to null.
		''' </summary>
		Public Sub New()
			MyBase.New()
		End Sub

		''' <summary>
		''' Use serialVersionUID from JNDI 1.1.1 for interoperability
		''' </summary>
		Private Const serialVersionUID As Long = 2607612850539889765L
	End Class

End Namespace