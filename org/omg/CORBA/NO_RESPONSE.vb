'
' * Copyright (c) 1995, 2006, Oracle and/or its affiliates. All rights reserved.
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
	''' This exception is raised if a client attempts to retrieve the result
	''' of a deferred synchronous call, but the response for the request is
	''' not yet available.<P>
	''' It contains a minor code, which gives more detailed information about
	''' what caused the exception, and a completion status. It may also contain
	''' a string describing the exception.
	''' </summary>
	''' <seealso cref= <A href="../../../../technotes/guides/idl/jidlExceptions.html">documentation on
	''' Java&nbsp;IDL exceptions</A>
	''' @since       JDK1.2 </seealso>

	Public NotInheritable Class NO_RESPONSE
		Inherits SystemException

		''' <summary>
		''' Constructs a <code>NO_RESPONSE</code> exception with a default minor code
		''' of 0, a completion state of CompletionStatus.COMPLETED_NO,
		''' and a null description.
		''' </summary>
		Public Sub New()
			Me.New("")
		End Sub

		''' <summary>
		''' Constructs a <code>NO_RESPONSE</code> exception with the specified description message,
		''' a minor code of 0, and a completion state of COMPLETED_NO. </summary>
		''' <param name="s"> the String containing a description message </param>
		Public Sub New(ByVal s As String)
			Me.New(s, 0, CompletionStatus.COMPLETED_NO)
		End Sub

		''' <summary>
		''' Constructs a <code>NO_RESPONSE</code> exception with the specified
		''' minor code and completion status. </summary>
		''' <param name="minor"> the minor code </param>
		''' <param name="completed"> the completion status </param>
		Public Sub New(ByVal minor As Integer, ByVal completed As CompletionStatus)
			Me.New("", minor, completed)
		End Sub

		''' <summary>
		''' Constructs a <code>NO_RESPONSE</code> exception with the specified description
		''' message, minor code, and completion status. </summary>
		''' <param name="s"> the String containing a description message </param>
		''' <param name="minor"> the minor code </param>
		''' <param name="completed"> the completion status </param>
		Public Sub New(ByVal s As String, ByVal minor As Integer, ByVal completed As CompletionStatus)
			MyBase.New(s, minor, completed)
		End Sub
	End Class

End Namespace