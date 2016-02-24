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
	''' This exception indicates that even though the operation that
	''' was invoked exists (it has an IDL definition), no implementation
	''' for that operation exists. <tt>NO_IMPLEMENT</tt> can, for
	''' example, be raised by an ORB if a client asks for an object's
	''' type definition from the interface repository, but no interface
	''' repository is provided by the ORB.<P>
	''' It contains a minor code, which gives more detailed information about
	''' what caused the exception, and a completion status. It may also contain
	''' a string describing the exception.
	''' <P>
	''' See the section <A href="../../../../technotes/guides/idl/jidlExceptions.html#minorcodemeanings">Minor
	''' Code Meanings</A> to see the minor codes for this exception.
	''' 
	''' @since       JDK1.2
	''' </summary>

	Public NotInheritable Class NO_IMPLEMENT
		Inherits SystemException

		''' <summary>
		''' Constructs a <code>NO_IMPLEMENT</code> exception with a default minor code
		''' of 0, a completion state of CompletionStatus.COMPLETED_NO,
		''' and a null description.
		''' </summary>
		Public Sub New()
			Me.New("")
		End Sub

		''' <summary>
		''' Constructs a <code>NO_IMPLEMENT</code> exception with the specified description message,
		''' a minor code of 0, and a completion state of COMPLETED_NO. </summary>
		''' <param name="s"> the String containing a description of the exception </param>
		Public Sub New(ByVal s As String)
			Me.New(s, 0, CompletionStatus.COMPLETED_NO)
		End Sub

		''' <summary>
		''' Constructs a <code>NO_IMPLEMENT</code> exception with the specified
		''' minor code and completion status. </summary>
		''' <param name="minor"> an <code>int</code> specifying the minor code </param>
		''' <param name="completed"> a <code>CompletionStatus</code> instance indicating
		'''                  the completion status </param>
		Public Sub New(ByVal minor As Integer, ByVal completed As CompletionStatus)
			Me.New("", minor, completed)
		End Sub

		''' <summary>
		''' Constructs a <code>NO_IMPLEMENT</code> exception with the specified description
		''' message, minor code, and completion status. </summary>
		''' <param name="s"> the String containing a description message </param>
		''' <param name="minor"> an <code>int</code> specifying the minor code </param>
		''' <param name="completed"> a <code>CompletionStatus</code> instance indicating
		'''                  the completion status </param>
		Public Sub New(ByVal s As String, ByVal minor As Integer, ByVal completed As CompletionStatus)
			MyBase.New(s, minor, completed)
		End Sub
	End Class

End Namespace