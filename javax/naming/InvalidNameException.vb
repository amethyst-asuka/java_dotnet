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
	''' This exception indicates that the name being specified does
	''' not conform to the naming syntax of a naming system.
	''' This exception is thrown by any of the methods that does name
	''' parsing (such as those in Context, DirContext, CompositeName and CompoundName).
	''' <p>
	''' Synchronization and serialization issues that apply to NamingException
	''' apply directly here.
	'''  
	''' @author Rosanna Lee
	''' @author Scott Seligman
	''' </summary>
	''' <seealso cref= Context </seealso>
	''' <seealso cref= javax.naming.directory.DirContext </seealso>
	''' <seealso cref= CompositeName </seealso>
	''' <seealso cref= CompoundName </seealso>
	''' <seealso cref= NameParser
	''' @since 1.3 </seealso>

	Public Class InvalidNameException
		Inherits NamingException

		''' <summary>
		''' Constructs an instance of InvalidNameException using an
		''' explanation of the problem.
		''' All other fields are initialized to null. </summary>
		''' <param name="explanation">      A possibly null message explaining the problem. </param>
		''' <seealso cref= java.lang.Throwable#getMessage </seealso>
		Public Sub New(ByVal explanation As String)
			MyBase.New(explanation)
		End Sub

		''' <summary>
		''' Constructs an instance of InvalidNameException with
		''' all fields set to null.
		''' </summary>
		Public Sub New()
			MyBase.New()
		End Sub

		''' <summary>
		''' Use serialVersionUID from JNDI 1.1.1 for interoperability
		''' </summary>
		Private Const serialVersionUID As Long = -8370672380823801105L
	End Class

End Namespace