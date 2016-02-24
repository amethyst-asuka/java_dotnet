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
	''' Exception raised whenever an invocation on a deleted object was
	''' performed. It is an authoritative  "hard"  fault report. Anyone
	''' receiving it is allowed (even expected) to delete all copies of
	''' this object reference and to perform other appropriate  "final
	''' recovery"  style procedures. Bridges forward this exception to
	''' clients, also destroying any records they may hold (for example,
	''' proxy objects used in reference translation). The clients could
	''' in turn purge any of their own data structures.
	''' <P>
	''' It contains a minor code, which gives more detailed information about
	''' what caused the exception, and a completion status. It may also contain
	''' a string describing the exception.
	''' <P>
	''' See the section <A href="../../../../technotes/guides/idl/jidlExceptions.html#minorcodemeanings">Minor
	''' Code Meanings</A> to see the minor codes for this exception.
	''' </summary>
	''' <seealso cref= <A href="../../../../technotes/guides/idl/jidlExceptions.html">documentation on
	''' Java&nbsp;IDL exceptions</A>
	''' @since       JDK1.2 </seealso>

	Public NotInheritable Class OBJECT_NOT_EXIST
		Inherits SystemException

		''' <summary>
		''' Constructs an <code>OBJECT_NOT_EXIST</code> exception with a default minor code
		''' of 0, a completion state of CompletionStatus.COMPLETED_NO,
		''' and a null description.
		''' </summary>
		Public Sub New()
			Me.New("")
		End Sub

		''' <summary>
		''' Constructs an <code>OBJECT_NOT_EXIST</code> exception with the specified description,
		''' a minor code of 0, and a completion state of COMPLETED_NO. </summary>
		''' <param name="s"> the String containing a description message </param>
		Public Sub New(ByVal s As String)
			Me.New(s, 0, CompletionStatus.COMPLETED_NO)
		End Sub

		''' <summary>
		''' Constructs an <code>OBJECT_NOT_EXIST</code> exception with the specified
		''' minor code and completion status. </summary>
		''' <param name="minor"> the minor code </param>
		''' <param name="completed"> the completion status </param>
		Public Sub New(ByVal minor As Integer, ByVal completed As CompletionStatus)
			Me.New("", minor, completed)
		End Sub

		''' <summary>
		''' Constructs an <code>OBJECT_NOT_EXIST</code> exception with the specified description
		''' message, minor code, and completion status. </summary>
		''' <param name="s"> the String containing a description message </param>
		''' <param name="minor"> the minor code </param>
		''' <param name="completed"> the completion status </param>
		Public Sub New(ByVal s As String, ByVal minor As Integer, ByVal completed As CompletionStatus)
			MyBase.New(s, minor, completed)
		End Sub
	End Class

End Namespace