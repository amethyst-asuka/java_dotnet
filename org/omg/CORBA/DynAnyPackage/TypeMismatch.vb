'
' * Copyright (c) 1998, 1999, Oracle and/or its affiliates. All rights reserved.
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


Namespace org.omg.CORBA.DynAnyPackage

	''' <summary>
	''' TypeMismatch is thrown by dynamic any accessor methods when
	''' type of the actual contents do not match what is trying to be
	''' accessed.
	''' </summary>
	Public NotInheritable Class TypeMismatch
		Inherits org.omg.CORBA.UserException

		''' <summary>
		''' Constructs a <code>TypeMismatch</code> object.
		''' </summary>
		Public Sub New()
			MyBase.New()
		End Sub

		''' <summary>
		''' Constructs a <code>TypeMismatch</code> object. </summary>
		''' <param name="reason">  a <code>String</code> giving more information
		''' regarding the exception. </param>
		Public Sub New(ByVal reason As String)
			MyBase.New(reason)
		End Sub
	End Class

End Namespace