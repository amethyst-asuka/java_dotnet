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
	''' This exception is thrown when a method
	''' in some ways violates the schema. An example of schema violation
	''' is modifying attributes of an object that violates the object's
	''' schema definition. Another example is renaming or moving an object
	''' to a part of the namespace that violates the namespace's
	''' schema definition.
	''' <p>
	''' Synchronization and serialization issues that apply to NamingException
	''' apply directly here.
	'''  
	''' @author Rosanna Lee
	''' @author Scott Seligman
	''' </summary>
	''' <seealso cref= javax.naming.Context#bind </seealso>
	''' <seealso cref= DirContext#bind </seealso>
	''' <seealso cref= javax.naming.Context#rebind </seealso>
	''' <seealso cref= DirContext#rebind </seealso>
	''' <seealso cref= DirContext#createSubcontext </seealso>
	''' <seealso cref= javax.naming.Context#createSubcontext </seealso>
	''' <seealso cref= DirContext#modifyAttributes
	''' @since 1.3 </seealso>
	Public Class SchemaViolationException
		Inherits javax.naming.NamingException

		''' <summary>
		''' Constructs a new instance of SchemaViolationException.
		''' All fields are set to null.
		''' </summary>
		Public Sub New()
			MyBase.New()
		End Sub

		''' <summary>
		''' Constructs a new instance of SchemaViolationException
		''' using the explanation supplied. All other fields are set to null. </summary>
		''' <param name="explanation"> Detail about this exception. Can be null. </param>
		''' <seealso cref= java.lang.Throwable#getMessage </seealso>
		Public Sub New(ByVal explanation As String)
			MyBase.New(explanation)
		End Sub

		''' <summary>
		''' Use serialVersionUID from JNDI 1.1.1 for interoperability
		''' </summary>
		Private Const serialVersionUID As Long = -3041762429525049663L
	End Class

End Namespace