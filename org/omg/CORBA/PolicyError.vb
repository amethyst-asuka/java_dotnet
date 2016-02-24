'
' * Copyright (c) 1998, 2000, Oracle and/or its affiliates. All rights reserved.
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
	''' A user exception thrown when a policy error occurs.  A <code>PolicyError</code>
	''' exception may include one of the following policy error reason codes
	''' defined in the org.omg.CORBA package: BAD_POLICY, BAD_POLICY_TYPE,
	''' BAD_POLICY_VALUE, UNSUPPORTED_POLICY, UNSUPPORTED_POLICY_VALUE.
	''' </summary>

	Public NotInheritable Class PolicyError
		Inherits org.omg.CORBA.UserException

		''' <summary>
		''' The reason for the <code>PolicyError</code> exception being thrown.
		''' @serial
		''' </summary>
		Public reason As Short

		''' <summary>
		''' Constructs a default <code>PolicyError</code> user exception
		''' with no reason code and an empty reason detail message.
		''' </summary>
		Public Sub New()
			MyBase.New()
		End Sub

		''' <summary>
		''' Constructs a <code>PolicyError</code> user exception
		''' initialized with the given reason code and an empty reason detail message. </summary>
		''' <param name="__reason"> the reason code. </param>
		Public Sub New(ByVal __reason As Short)
			MyBase.New()
			reason = __reason
		End Sub

		''' <summary>
		''' Constructs a <code>PolicyError</code> user exception
		''' initialized with the given reason detail message and reason code. </summary>
		''' <param name="reason_string"> the reason detail message. </param>
		''' <param name="__reason"> the reason code. </param>
		Public Sub New(ByVal reason_string As String, ByVal __reason As Short)
			MyBase.New(reason_string)
			reason = __reason
		End Sub
	End Class

End Namespace