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
	''' Exception  thrown
	''' when the request carried an invalid transaction context.
	''' For example, this exception could be raised if an error
	''' occurred when trying to register a resource.<P>
	''' It contains a minor code, which gives more detailed information about
	''' what caused the exception, and a completion status. It may also contain
	''' a string describing the exception.
	''' </summary>
	''' <seealso cref= <A href="../../../../technotes/guides/idl/jidlExceptions.html">documentation on
	''' Java&nbsp;IDL exceptions</A>
	''' @since   JDK1.2
	'''  </seealso>

	Public NotInheritable Class INVALID_TRANSACTION
		Inherits SystemException

		''' <summary>
		''' Constructs an <code>INVALID_TRANSACTION</code> exception with a default minor code
		''' of 0 and a completion state of COMPLETED_NO.
		''' </summary>
		Public Sub New()
			Me.New("")
		End Sub

		''' <summary>
		''' Constructs an <code>INVALID_TRANSACTION</code> exception
		''' with the specified detail message. </summary>
		''' <param name="s"> the String containing a detail message </param>
		Public Sub New(ByVal s As String)
			Me.New(s, 0, CompletionStatus.COMPLETED_NO)
		End Sub

		''' <summary>
		''' Constructs an <code>INVALID_TRANSACTION</code> exception with the specified
		''' minor code and completion status. </summary>
		''' <param name="minor"> the minor code </param>
		''' <param name="completed"> the completion status </param>
		Public Sub New(ByVal minor As Integer, ByVal completed As CompletionStatus)
			Me.New("", minor, completed)
		End Sub

		''' <summary>
		''' Constructs an <code>INVALID_TRANSACTION</code> exception with the specified detail
		''' message, minor code, and completion status.
		''' A detail message is a String that describes this particular exception. </summary>
		''' <param name="s"> the String containing a detail message </param>
		''' <param name="minor"> the minor code </param>
		''' <param name="completed"> the completion status </param>
		Public Sub New(ByVal s As String, ByVal minor As Integer, ByVal completed As CompletionStatus)
			MyBase.New(s, minor, completed)
		End Sub
	End Class

End Namespace