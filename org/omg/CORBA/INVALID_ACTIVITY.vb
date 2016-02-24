'
' * Copyright (c) 2004, 2006, Oracle and/or its affiliates. All rights reserved.
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
	''' The <code>INVALID_ACTIVITY</code> system exception may be raised on the
	''' Activity or Transaction services' resume methods if a transaction or
	''' Activity is resumed in a context different to that from which it was
	''' suspended. It is also raised when an attempted invocation is made that
	''' is incompatible with the Activity's current state.
	''' </summary>
	''' <seealso cref= <A href="../../../../technotes/guides/idl/jidlExceptions.html">documentation on
	'''      Java&nbsp;IDL exceptions</A>
	''' @since   J2SE 1.5 </seealso>

	Public NotInheritable Class INVALID_ACTIVITY
		Inherits SystemException

		''' <summary>
		''' Constructs an <code>INVALID_ACTIVITY</code> exception with
		''' minor code set to 0 and CompletionStatus set to COMPLETED_NO.
		''' </summary>
		Public Sub New()
			Me.New("")
		End Sub

		''' <summary>
		''' Constructs an <code>INVALID_ACTIVITY</code> exception with the
		''' specified message.
		''' </summary>
		''' <param name="detailMessage"> string containing a detailed message. </param>
		Public Sub New(ByVal detailMessage As String)
			Me.New(detailMessage, 0, CompletionStatus.COMPLETED_NO)
		End Sub

		''' <summary>
		''' Constructs an <code>INVALID_ACTIVITY</code> exception with the
		''' specified minor code and completion status.
		''' </summary>
		''' <param name="minorCode"> minor code. </param>
		''' <param name="completionStatus"> completion status. </param>
		Public Sub New(ByVal minorCode As Integer, ByVal completionStatus_Renamed As CompletionStatus)
			Me.New("", minorCode, completionStatus_Renamed)
		End Sub

		''' <summary>
		''' Constructs an <code>INVALID_ACTIVITY</code> exception with the
		''' specified message, minor code, and completion status.
		''' </summary>
		''' <param name="detailMessage"> string containing a detailed message. </param>
		''' <param name="minorCode"> minor code. </param>
		''' <param name="completionStatus"> completion status. </param>
		Public Sub New(ByVal detailMessage As String, ByVal minorCode As Integer, ByVal completionStatus_Renamed As CompletionStatus)
			MyBase.New(detailMessage, minorCode, completionStatus_Renamed)
		End Sub
	End Class

End Namespace