'
' * Copyright (c) 1997, 2000, Oracle and/or its affiliates. All rights reserved.
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
	''' The <code>InvalidName</code> exception is raised when
	''' <code>ORB.resolve_initial_references</code> is passed a name
	''' for which there is no initial reference.
	''' </summary>
	''' <seealso cref= org.omg.CORBA.ORB#resolve_initial_references(String)
	''' @since   JDK1.2 </seealso>

	Public NotInheritable Class InvalidName
		Inherits org.omg.CORBA.UserException

		''' <summary>
		''' Constructs an <code>InvalidName</code> exception with no reason message.
		''' </summary>
		Public Sub New()
			MyBase.New()
		End Sub

		''' <summary>
		''' Constructs an <code>InvalidName</code> exception with the specified
		''' reason message. </summary>
		''' <param name="reason"> the String containing a reason message </param>
		Public Sub New(ByVal reason As String)
			MyBase.New(reason)
		End Sub
	End Class

End Namespace