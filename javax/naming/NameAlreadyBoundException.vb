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
	''' This exception is thrown by methods to indicate that
	''' a binding cannot be added because the name is already bound to
	''' another object.
	''' <p>
	''' Synchronization and serialization issues that apply to NamingException
	''' apply directly here.
	'''  
	''' @author Rosanna Lee
	''' @author Scott Seligman
	''' </summary>
	''' <seealso cref= Context#bind </seealso>
	''' <seealso cref= Context#rebind </seealso>
	''' <seealso cref= Context#createSubcontext </seealso>
	''' <seealso cref= javax.naming.directory.DirContext#bind </seealso>
	''' <seealso cref= javax.naming.directory.DirContext#rebind </seealso>
	''' <seealso cref= javax.naming.directory.DirContext#createSubcontext
	''' @since 1.3 </seealso>

	Public Class NameAlreadyBoundException
		Inherits NamingException

		''' <summary>
		''' Constructs a new instance of NameAlreadyBoundException using the
		''' explanation supplied. All other fields default to null.
		''' 
		''' </summary>
		''' <param name="explanation">     Possibly null additional detail about this exception. </param>
		''' <seealso cref= java.lang.Throwable#getMessage </seealso>
		Public Sub New(ByVal explanation As String)
			MyBase.New(explanation)
		End Sub

		''' <summary>
		''' Constructs a new instance of NameAlreadyBoundException.
		''' All fields are set to null;
		''' </summary>
		Public Sub New()
			MyBase.New()
		End Sub

		''' <summary>
		''' Use serialVersionUID from JNDI 1.1.1 for interoperability
		''' </summary>
		Private Const serialVersionUID As Long = -8491441000356780586L
	End Class

End Namespace