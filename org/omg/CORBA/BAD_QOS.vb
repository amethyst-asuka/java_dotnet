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
	''' The <code>BAD_QOS</code> exception is raised whenever an object cannot
	''' support the quality of service required by an invocation parameter that
	''' has a quality of service semantics associated with it.
	''' </summary>
	''' <seealso cref= <A href="../../../../technotes/guides/idl/jidlExceptions.html">documentation on
	'''      Java&nbsp;IDL exceptions</A>
	''' @since   J2SE 1.5 </seealso>

	Public NotInheritable Class BAD_QOS
		Inherits SystemException

		''' <summary>
		''' Constructs an <code>BAD_QOS</code> exception with
		''' minor code set to 0 and CompletionStatus set to COMPLETED_NO.
		''' </summary>
		Public Sub New()
			Me.New("")
		End Sub

		''' <summary>
		''' Constructs an <code>BAD_QOS</code> exception with the
		''' specified message.
		''' </summary>
		''' <param name="detailMessage"> string containing a detailed message. </param>
		Public Sub New(ByVal detailMessage As String)
			Me.New(detailMessage, 0, CompletionStatus.COMPLETED_NO)
		End Sub

		''' <summary>
		''' Constructs an <code>BAD_QOS</code> exception with the
		''' specified minor code and completion status.
		''' </summary>
		''' <param name="minorCode"> minor code. </param>
		''' <param name="completionStatus"> completion status. </param>
		Public Sub New(ByVal minorCode As Integer, ByVal completionStatus_Renamed As CompletionStatus)
			Me.New("", minorCode, completionStatus_Renamed)
		End Sub

		''' <summary>
		''' Constructs an <code>BAD_QOS</code> exception with the
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