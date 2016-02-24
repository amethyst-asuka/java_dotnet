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
	''' This exception is raised if an operation implementation
	''' throws a non-CORBA exception (such as an exception
	''' specific to the implementation's programming language),
	''' or if an operation raises a user exception that does not
	''' appear in the operation's raises expression. UNKNOWN is
	''' also raised if the server returns a system exception that
	''' is unknown to the client. (This can happen if the server
	''' uses a later version of CORBA than the client and new system
	''' exceptions have been added to the later version.)<P>
	''' It contains a minor code, which gives more detailed information about
	''' what caused the exception, and a completion status. It may also contain
	''' a string describing the exception.
	''' <P>
	''' See the section <A href="../../../../technotes/guides/idl/jidlExceptions.html#minorcodemeanings">Minor
	''' Code Meanings</A> to see the minor codes for this exception.
	''' </summary>
	''' <seealso cref= <A href="../../../../technotes/guides/idl/jidlExceptions.html">documentation on
	''' Java&nbsp;IDL exceptions</A> </seealso>

	Public NotInheritable Class UNKNOWN
		Inherits SystemException

		''' <summary>
		''' Constructs an <code>UNKNOWN</code> exception with a default minor code
		''' of 0, a completion state of CompletionStatus.COMPLETED_NO,
		''' and a null description.
		''' </summary>
		Public Sub New()
			Me.New("")
		End Sub

		''' <summary>
		''' Constructs an <code>UNKNOWN</code> exception with the specified description message,
		''' a minor code of 0, and a completion state of COMPLETED_NO. </summary>
		''' <param name="s"> the String containing a detail message </param>
		Public Sub New(ByVal s As String)
			Me.New(s, 0, CompletionStatus.COMPLETED_NO)
		End Sub

		''' <summary>
		''' Constructs an <code>UNKNOWN</code> exception with the specified
		''' minor code and completion status. </summary>
		''' <param name="minor"> the minor code </param>
		''' <param name="completed"> the completion status </param>
		Public Sub New(ByVal minor As Integer, ByVal completed As CompletionStatus)
			Me.New("", minor, completed)
		End Sub

		''' <summary>
		''' Constructs an <code>UNKNOWN</code> exception with the specified description
		''' message, minor code, and completion status. </summary>
		''' <param name="s"> the String containing a description message </param>
		''' <param name="minor"> the minor code </param>
		''' <param name="completed"> the completion status </param>
		Public Sub New(ByVal s As String, ByVal minor As Integer, ByVal completed As CompletionStatus)
			MyBase.New(s, minor, completed)
		End Sub
	End Class

End Namespace