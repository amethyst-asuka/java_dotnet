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
	''' Exception  thrown
	''' when a parameter passed to a call is out of range or
	''' otherwise considered illegal. An ORB may raise this exception
	''' if null values or null pointers are passed to an operation (for
	''' language mappings where the concept of a null pointers or null
	''' values applies). BAD_PARAM can also be raised as a result of a
	''' client generating requests with incorrect parameters using the DII. <P>
	''' It contains a minor code, which gives more detailed information about
	''' what caused the exception, and a completion status. It may also contain
	''' a string describing the exception.
	''' </summary>
	''' <seealso cref= <A href="../../../../technotes/guides/idl/jidlExceptions.html">documentation on
	''' Java&nbsp;IDL exceptions</A> </seealso>
	''' <seealso cref= <A href="../../../../technotes/guides/idl/jidlExceptions.html#minorcodemeanings">meaning of
	''' minor codes</A>
	''' @since       JDK1.2 </seealso>

	Public NotInheritable Class BAD_PARAM
		Inherits SystemException

		''' <summary>
		''' Constructs a <code>BAD_PARAM</code> exception with a default
		''' minor code of 0 and a completion state of COMPLETED_NO.
		''' </summary>
		Public Sub New()
			Me.New("")
		End Sub

		''' <summary>
		''' Constructs a <code>BAD_PARAM</code> exception with the specified detail
		''' message, a minor code of 0, and a completion state of COMPLETED_NO.
		''' </summary>
		''' <param name="s"> the String containing a detail message describing this
		'''          exception </param>
		Public Sub New(ByVal s As String)
			Me.New(s, 0, CompletionStatus.COMPLETED_NO)
		End Sub

		''' <summary>
		''' Constructs a <code>BAD_PARAM</code> exception with the specified
		''' minor code and completion status. </summary>
		''' <param name="minor"> the minor code </param>
		''' <param name="completed"> the completion status </param>
		Public Sub New(ByVal minor As Integer, ByVal completed As CompletionStatus)
			Me.New("", minor, completed)
		End Sub

		''' <summary>
		''' Constructs a <code>BAD_PARAM</code> exception with the specified detail
		''' message, minor code, and completion status.
		''' A detail message is a <code>String</code> that describes
		''' this particular exception.
		''' </summary>
		''' <param name="s"> the <code>String</code> containing a detail message </param>
		''' <param name="minor"> the minor code </param>
		''' <param name="completed"> the completion status </param>
		Public Sub New(ByVal s As String, ByVal minor As Integer, ByVal completed As CompletionStatus)
			MyBase.New(s, minor, completed)
		End Sub
	End Class

End Namespace