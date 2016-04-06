'
' * Copyright (c) 2000, 2003, Oracle and/or its affiliates. All rights reserved.
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


Namespace java.util.logging


	''' <summary>
	''' The permission which the SecurityManager will check when code
	''' that is running with a SecurityManager calls one of the logging
	''' control methods (such as Logger.setLevel).
	''' <p>
	''' Currently there is only one named LoggingPermission.  This is "control"
	''' and it grants the ability to control the logging configuration, for
	''' example by adding or removing Handlers, by adding or removing Filters,
	''' or by changing logging levels.
	''' <p>
	''' Programmers do not normally create LoggingPermission objects directly.
	''' Instead they are created by the security policy code based on reading
	''' the security policy file.
	''' 
	''' 
	''' @since 1.4 </summary>
	''' <seealso cref= java.security.BasicPermission </seealso>
	''' <seealso cref= java.security.Permission </seealso>
	''' <seealso cref= java.security.Permissions </seealso>
	''' <seealso cref= java.security.PermissionCollection </seealso>
	''' <seealso cref= java.lang.SecurityManager
	'''  </seealso>

	Public NotInheritable Class LoggingPermission
		Inherits java.security.BasicPermission

		Private Const serialVersionUID As Long = 63564341580231582L

		''' <summary>
		''' Creates a new LoggingPermission object.
		''' </summary>
		''' <param name="name"> Permission name.  Must be "control". </param>
		''' <param name="actions"> Must be either null or the empty string.
		''' </param>
		''' <exception cref="NullPointerException"> if <code>name</code> is <code>null</code>. </exception>
		''' <exception cref="IllegalArgumentException"> if <code>name</code> is empty or if
		''' arguments are invalid. </exception>
		Public Sub New(  name As String,   actions As String)
			MyBase.New(name)
			If Not name.Equals("control") Then Throw New IllegalArgumentException("name: " & name)
			If actions IsNot Nothing AndAlso actions.length() > 0 Then Throw New IllegalArgumentException("actions: " & actions)
		End Sub
	End Class

End Namespace