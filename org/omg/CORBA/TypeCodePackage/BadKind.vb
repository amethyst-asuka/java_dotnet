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
	''' The exception <code>BadKind</code> is thrown when
	''' an inappropriate operation is invoked on a <code>TypeCode</code> object. For example,
	''' invoking the method <code>discriminator_type()</code> on an instance of
	''' <code>TypeCode</code> that does not represent an IDL union will cause the
	''' exception <code>BadKind</code> to be thrown.
	''' </summary>
	''' <seealso cref= org.omg.CORBA.TypeCode
	''' @since   JDK1.2 </seealso>

	Public NotInheritable Class BadKind
		Inherits org.omg.CORBA.UserException

		''' <summary>
		''' Constructs a <code>BadKind</code> exception with no reason message.
		''' </summary>
		Public Sub New()
			MyBase.New()
		End Sub

		''' <summary>
		''' Constructs a <code>BadKind</code> exception with the specified
		''' reason message. </summary>
		''' <param name="reason"> the String containing a reason message </param>
		Public Sub New(ByVal reason As String)
			MyBase.New(reason)
		End Sub
	End Class

End Namespace