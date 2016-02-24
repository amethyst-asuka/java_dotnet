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
	''' This exception indicates that an object reference is internally
	''' malformed. For example, the repository ID may have incorrect
	''' syntax or the addressing information may be invalid. This
	''' exception is raised by ORB::string_to_object if the passed
	''' string does not decode correctly. An ORB may choose to detect
	''' calls via nil references (but is not obliged to do detect them).
	''' <tt>INV_OBJREF</tt> is used to indicate this.<P>
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

	Public NotInheritable Class INV_OBJREF
		Inherits SystemException

		''' <summary>
		''' Constructs an <code>INV_OBJREF</code> exception with a default
		''' minor code of 0 and a completion state of COMPLETED_NO.
		''' </summary>
		Public Sub New()
			Me.New("")
		End Sub

		''' <summary>
		''' Constructs an <code>INV_OBJREF</code> exception with the specified detail
		''' message, a minor code of 0, and a completion state of COMPLETED_NO. </summary>
		''' <param name="s"> the String containing a detail message </param>
		Public Sub New(ByVal s As String)
			Me.New(s, 0, CompletionStatus.COMPLETED_NO)
		End Sub

		''' <summary>
		''' Constructs an <code>INV_OBJREF</code> exception with the specified
		''' minor code and completion status. </summary>
		''' <param name="minor"> the minor code </param>
		''' <param name="completed"> a <code>CompletionStatus</code> instance indicating
		'''                  the completion status </param>
		Public Sub New(ByVal minor As Integer, ByVal completed As CompletionStatus)
			Me.New("", minor, completed)
		End Sub

		''' <summary>
		''' Constructs an <code>INV_OBJREF</code> exception with the specified detail
		''' message, minor code, and completion status.
		''' A detail message is a String that describes this particular exception. </summary>
		''' <param name="s"> the String containing a detail message </param>
		''' <param name="minor"> the minor code </param>
		''' <param name="completed"> a <code>CompletionStatus</code> instance indicating
		'''                  the completion status </param>
		Public Sub New(ByVal s As String, ByVal minor As Integer, ByVal completed As CompletionStatus)
			MyBase.New(s, minor, completed)
		End Sub
	End Class

End Namespace