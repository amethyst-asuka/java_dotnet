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
	''' This exception is thrown when a method
	''' produces a result that exceeds a size-related limit.
	''' This can happen, for example, if the result contains
	''' more objects than the user requested, or when the size
	''' of the result exceeds some implementation-specific limit.
	''' <p>
	''' Synchronization and serialization issues that apply to NamingException
	''' apply directly here.
	'''  
	''' @author Rosanna Lee
	''' @author Scott Seligman
	'''  
	''' @since 1.3
	''' </summary>
	Public Class SizeLimitExceededException
		Inherits LimitExceededException

		''' <summary>
		''' Constructs a new instance of SizeLimitExceededException.
		''' All fields default to null.
		''' </summary>
		Public Sub New()
			MyBase.New()
		End Sub

		''' <summary>
		''' Constructs a new instance of SizeLimitExceededException using an
		''' explanation. All other fields default to null.
		''' </summary>
		''' <param name="explanation"> Possibly null detail about this exception. </param>
		Public Sub New(ByVal explanation As String)
			MyBase.New(explanation)
		End Sub

		''' <summary>
		''' Use serialVersionUID from JNDI 1.1.1 for interoperability
		''' </summary>
		Private Const serialVersionUID As Long = 7129289564879168579L
	End Class

End Namespace