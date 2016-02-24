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
	''' The InvalidSeq exception is thrown by all operations on dynamic
	''' anys that take a sequence (Java array) as an argument, when that
	''' sequence is invalid.
	''' </summary>
	Public NotInheritable Class InvalidSeq
		Inherits org.omg.CORBA.UserException

		''' <summary>
		''' Constructs an <code>InvalidSeq</code> object.
		''' </summary>
		Public Sub New()
			MyBase.New()
		End Sub

		''' <summary>
		''' Constructs an <code>InvalidSeq</code> object. </summary>
		''' <param name="reason">  a <code>String</code> giving more information
		''' regarding the exception. </param>
		Public Sub New(ByVal reason As String)
			MyBase.New(reason)
		End Sub
	End Class

End Namespace