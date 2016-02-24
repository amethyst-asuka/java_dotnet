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
	''' A user exception thrown when a parameter is not within
	''' the legal bounds for the object that a method is trying
	''' to access.
	''' </summary>
	''' <seealso cref= <A href="../../../../technotes/guides/idl/jidlExceptions.html">documentation on
	''' Java&nbsp;IDL exceptions</A> </seealso>

	Public NotInheritable Class Bounds
		Inherits org.omg.CORBA.UserException

		''' <summary>
		''' Constructs an <code>Bounds</code> with no specified detail message.
		''' </summary>
		Public Sub New()
			MyBase.New()
		End Sub

		''' <summary>
		''' Constructs an <code>Bounds</code> with the specified detail message.
		''' </summary>
		''' <param name="reason">   the detail message. </param>
		Public Sub New(ByVal reason As String)
			MyBase.New(reason)
		End Sub
	End Class

End Namespace