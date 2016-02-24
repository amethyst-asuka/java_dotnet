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


Namespace org.omg.CORBA.ORBPackage

	''' <summary>
	''' InconsistentTypeCode is thrown when an attempt is made to create a
	''' dynamic any with a type code that does not match the particular
	''' subclass of <code>DynAny</code>.
	''' </summary>
	Public NotInheritable Class InconsistentTypeCode
		Inherits org.omg.CORBA.UserException

		''' <summary>
		''' Constructs an <code>InconsistentTypeCode</code> user exception
		''' with no reason message.
		''' </summary>
		Public Sub New()
			MyBase.New()
		End Sub

		''' <summary>
		''' Constructs an <code>InconsistentTypeCode</code> user exception
		''' with the specified reason message. </summary>
		''' <param name="reason"> The String containing a reason message </param>
		Public Sub New(ByVal reason As String)
			MyBase.New(reason)
		End Sub
	End Class

End Namespace