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
	''' This exception is raised if communication is lost while an operation
	''' is in progress, after the request was sent by the client, but before
	''' the reply from the server has been returned to the client.<P>
	''' It contains a minor code, which gives more detailed information about
	''' what caused the exception, and a completion status. It may also contain
	''' a string describing the exception.
	''' <P>
	''' See the section <A href="../../../../technotes/guides/idl/jidlExceptions.html#minorcodemeanings">meaning
	''' of minor codes</A> to see the minor codes for this exception.
	''' </summary>
	''' <seealso cref= <A href="../../../../technotes/guides/idl/jidlExceptions.html#minorcodemeanings">meaning of
	''' minor codes</A>
	''' @since       JDK1.2 </seealso>

	Public NotInheritable Class COMM_FAILURE
		Inherits SystemException

		''' <summary>
		''' Constructs a <code>COMM_FAILURE</code> exception with
		''' a default minor code of 0 and a completion state of COMPLETED_NO.
		''' </summary>
		Public Sub New()
			Me.New("")
		End Sub

		''' <summary>
		''' Constructs a <code>COMM_FAILURE</code> exception with the specified detail
		''' message, a minor code of 0, and a completion state of COMPLETED_NO.
		''' </summary>
		''' <param name="s"> the <code>String</code> containing a detail message describing
		'''          this exception </param>
		Public Sub New(ByVal s As String)
			Me.New(s, 0, CompletionStatus.COMPLETED_NO)
		End Sub

		''' <summary>
		''' Constructs a <code>COMM_FAILURE</code> exception with the specified
		''' minor code and completion status. </summary>
		''' <param name="minor"> the minor code </param>
		''' <param name="completed"> the completion status, which must be one of
		'''                  <code>COMPLETED_YES</code>, <code>COMPLETED_NO</code>, or
		'''                  <code>COMPLETED_MAYBE</code>. </param>
		Public Sub New(ByVal minor As Integer, ByVal completed As CompletionStatus)
			Me.New("", minor, completed)
		End Sub

		''' <summary>
		''' Constructs a <code>COMM_FAILURE</code> exception with the specified detail
		''' message, minor code, and completion status.
		''' A detail message is a String that describes this particular exception. </summary>
		''' <param name="s"> the String containing a detail message </param>
		''' <param name="minor"> the minor code </param>
		''' <param name="completed"> the completion status, which must be one of
		'''                  <code>COMPLETED_YES</code>, <code>COMPLETED_NO</code>, or
		'''                  <code>COMPLETED_MAYBE</code>. </param>
		Public Sub New(ByVal s As String, ByVal minor As Integer, ByVal completed As CompletionStatus)
			MyBase.New(s, minor, completed)
		End Sub
	End Class

End Namespace