Imports System

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
	''' The root class for all CORBA standard exceptions. These exceptions
	''' may be thrown as a result of any CORBA operation invocation and may
	''' also be returned by many standard CORBA API methods. The standard
	''' exceptions contain a minor code, allowing more detailed specification, and a
	''' completion status. This class is subclassed to
	''' generate each one of the set of standard ORB exceptions.
	''' <code>SystemException</code> extends
	''' <code>java.lang.RuntimeException</code>; thus none of the
	''' <code>SystemException</code> exceptions need to be
	''' declared in signatures of the Java methods mapped from operations in
	''' IDL interfaces.
	''' </summary>
	''' <seealso cref= <A href="../../../../technotes/guides/idl/jidlExceptions.html">documentation on
	''' Java&nbsp;IDL exceptions</A> </seealso>

	Public MustInherit Class SystemException
		Inherits Exception

		''' <summary>
		''' The CORBA Exception minor code.
		''' @serial
		''' </summary>
		Public minor As Integer

		''' <summary>
		''' The status of the operation that threw this exception.
		''' @serial
		''' </summary>
		Public completed As CompletionStatus

		''' <summary>
		''' Constructs a <code>SystemException</code> exception with the specified detail
		''' message, minor code, and completion status.
		''' A detail message is a String that describes this particular exception. </summary>
		''' <param name="reason"> the String containing a detail message </param>
		''' <param name="minor"> the minor code </param>
		''' <param name="completed"> the completion status </param>
		Protected Friend Sub New(ByVal reason As String, ByVal minor As Integer, ByVal completed As CompletionStatus)
			MyBase.New(reason)
			Me.minor = minor
			Me.completed = completed
		End Sub

		''' <summary>
		''' Converts this exception to a representative string.
		''' </summary>
		Public Overrides Function ToString() As String
			' The fully qualified exception class name
			Dim result As String = MyBase.ToString()

			' The vmcid part
			Dim vmcid As Integer = minor And &HFFFFF000L
			Select Case vmcid
				Case org.omg.CORBA.OMGVMCID.value
					result &= "  vmcid: OMG"
				Case com.sun.corba.se.impl.util.SUNVMCID.value
					result &= "  vmcid: SUN"
				Case Else
					result &= "  vmcid: 0x" & Integer.toHexString(vmcid)
			End Select

			' The minor code part
			Dim mc As Integer = minor And &HFFF
			result &= "  minor code: " & mc

			' The completion status part
			Select Case completed.value()
				Case CompletionStatus._COMPLETED_YES
					result &= "  completed: Yes"
				Case CompletionStatus._COMPLETED_NO
					result &= "  completed: No"
				Case Else
					result &= " completed: Maybe"
			End Select
			Return result
		End Function
	End Class

End Namespace