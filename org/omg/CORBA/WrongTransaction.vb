'
' * Copyright (c) 1997, 2006, Oracle and/or its affiliates. All rights reserved.
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

Namespace org.omg.CORBA

	''' <summary>
	''' The CORBA <code>WrongTransaction</code> user-defined exception.
	''' This exception is thrown only by the methods
	''' <code>Request.get_response</code>
	''' and <code>ORB.get_next_response</code> when they are invoked
	''' from a transaction scope that is different from the one in
	''' which the client originally sent the request.
	''' See the OMG Transaction Service Specification for details.
	''' </summary>
	''' <seealso cref= <A href="../../../../technotes/guides/idl/jidlExceptions.html">documentation on
	''' Java&nbsp;IDL exceptions</A> </seealso>

	Public NotInheritable Class WrongTransaction
		Inherits UserException

		''' <summary>
		''' Constructs a WrongTransaction object with an empty detail message.
		''' </summary>
		Public Sub New()
			MyBase.New(WrongTransactionHelper.id())
		End Sub

		''' <summary>
		''' Constructs a WrongTransaction object with the given detail message. </summary>
		''' <param name="reason"> The detail message explaining what caused this exception to be thrown. </param>
		Public Sub New(ByVal reason As String)
			MyBase.New(WrongTransactionHelper.id() & "  " & reason)
		End Sub
	End Class

End Namespace