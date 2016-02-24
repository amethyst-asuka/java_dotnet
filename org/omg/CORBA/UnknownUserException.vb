'
' * Copyright (c) 1997, 2010, Oracle and/or its affiliates. All rights reserved.
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
	''' A class that contains user exceptions returned by the server.
	''' When the client uses the DII to make an invocation, any user exception
	''' returned from the server is enclosed in an <code>Any</code> object contained in the
	''' <code>UnknownUserException</code> object. This is available from the
	''' <code>Environment</code> object returned by the method <code>Request.env</code>.
	''' </summary>
	''' <seealso cref= <A href="../../../../technotes/guides/idl/jidlExceptions.html">documentation on
	''' Java&nbsp;IDL exceptions</A> </seealso>
	''' <seealso cref= Request </seealso>

	Public NotInheritable Class UnknownUserException
		Inherits UserException

		''' <summary>
		''' The <code>Any</code> instance that contains the actual user exception thrown
		'''  by the server.
		''' @serial
		''' </summary>
		Public except As Any

		''' <summary>
		''' Constructs an <code>UnknownUserException</code> object.
		''' </summary>
		Public Sub New()
			MyBase.New()
		End Sub

		''' <summary>
		''' Constructs an <code>UnknownUserException</code> object that contains the given
		''' <code>Any</code> object.
		''' </summary>
		''' <param name="a"> an <code>Any</code> object that contains a user exception returned
		'''         by the server </param>
		Public Sub New(ByVal a As Any)
			MyBase.New()
			except = a
		End Sub
	End Class

End Namespace