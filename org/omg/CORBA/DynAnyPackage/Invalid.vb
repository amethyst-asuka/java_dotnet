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
	''' Invalid is thrown by dynamic any operations when a bad
	''' <code>DynAny</code> or <code>Any</code> is passed as a parameter.
	''' </summary>
	Public NotInheritable Class Invalid
		Inherits org.omg.CORBA.UserException

		''' <summary>
		''' Constructs an <code>Invalid</code> object.
		''' </summary>
		Public Sub New()
			MyBase.New()
		End Sub

		''' <summary>
		''' Constructs an <code>Invalid</code> object. </summary>
		''' <param name="reason"> a <code>String</code> giving more information
		''' regarding the bad parameter passed to a dynamic any operation. </param>
		Public Sub New(ByVal reason As String)
			MyBase.New(reason)
		End Sub
	End Class

End Namespace