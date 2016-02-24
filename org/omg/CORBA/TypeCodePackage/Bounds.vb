'
' * Copyright (c) 1997, 1999, Oracle and/or its affiliates. All rights reserved.
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

Namespace org.omg.CORBA.TypeCodePackage

	''' <summary>
	''' Provides the <code>TypeCode</code> operations <code>member_name()</code>,
	''' <code>member_type()</code>, and <code>member_label</code>.
	''' These methods
	''' raise <code>Bounds</code> when the index parameter is greater than or equal
	''' to the number of members constituting the type.
	''' 
	''' @since   JDK1.2
	''' </summary>

	Public NotInheritable Class Bounds
		Inherits org.omg.CORBA.UserException

		''' <summary>
		''' Constructs a <code>Bounds</code> exception with no reason message.
		''' </summary>
		Public Sub New()
			MyBase.New()
		End Sub

		''' <summary>
		''' Constructs a <code>Bounds</code> exception with the specified
		''' reason message. </summary>
		''' <param name="reason"> the String containing a reason message </param>
		Public Sub New(ByVal reason As String)
			MyBase.New(reason)
		End Sub
	End Class

End Namespace