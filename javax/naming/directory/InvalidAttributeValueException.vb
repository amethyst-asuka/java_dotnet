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
	''' This class is thrown when an attempt is
	''' made to add to an attribute a value that conflicts with the attribute's
	''' schema definition.  This could happen, for example, if attempting
	''' to add an attribute with no value when the attribute is required
	''' to have at least one value, or if attempting to add more than
	''' one value to a single valued-attribute, or if attempting to
	''' add a value that conflicts with the syntax of the attribute.
	''' <p>
	''' Synchronization and serialization issues that apply to NamingException
	''' apply directly here.
	'''  
	''' @author Rosanna Lee
	''' @author Scott Seligman
	''' @since 1.3
	''' </summary>

	Public Class InvalidAttributeValueException
		Inherits javax.naming.NamingException

		''' <summary>
		''' Constructs a new instance of InvalidAttributeValueException using
		''' an explanation. All other fields are set to null. </summary>
		''' <param name="explanation">     Additional detail about this exception. Can be null. </param>
		''' <seealso cref= java.lang.Throwable#getMessage </seealso>
		Public Sub New(ByVal explanation As String)
			MyBase.New(explanation)
		End Sub

		''' <summary>
		''' Constructs a new instance of InvalidAttributeValueException.
		''' All fields are set to null.
		''' </summary>
		Public Sub New()
			MyBase.New()
		End Sub

		''' <summary>
		''' Use serialVersionUID from JNDI 1.1.1 for interoperability
		''' </summary>
		Private Const serialVersionUID As Long = 8720050295499275011L
	End Class

End Namespace