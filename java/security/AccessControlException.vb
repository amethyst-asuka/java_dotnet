'
' * Copyright (c) 1997, 2013, Oracle and/or its affiliates. All rights reserved.
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

Namespace java.security

	''' <summary>
	''' <p> This exception is thrown by the AccessController to indicate
	''' that a requested access (to a critical system resource such as the
	''' file system or the network) is denied.
	''' 
	''' <p> The reason to deny access can vary.  For example, the requested
	''' permission might be of an incorrect type,  contain an invalid
	''' value, or request access that is not allowed according to the
	''' security policy.  Such information should be given whenever
	''' possible at the time the exception is thrown.
	''' 
	''' @author Li Gong
	''' @author Roland Schemers
	''' </summary>

	Public Class AccessControlException
		Inherits SecurityException

		Private Shadows Const serialVersionUID As Long = 5138225684096988535L

		' the permission that caused the exception to be thrown.
		Private perm As Permission

		''' <summary>
		''' Constructs an {@code AccessControlException} with the
		''' specified, detailed message.
		''' </summary>
		''' <param name="s">   the detail message. </param>
		Public Sub New(  s As String)
			MyBase.New(s)
		End Sub

		''' <summary>
		''' Constructs an {@code AccessControlException} with the
		''' specified, detailed message, and the requested permission that caused
		''' the exception.
		''' </summary>
		''' <param name="s">   the detail message. </param>
		''' <param name="p">   the permission that caused the exception. </param>
		Public Sub New(  s As String,   p As Permission)
			MyBase.New(s)
			perm = p
		End Sub

        ''' <summary>
        ''' Gets the Permission object associated with this exception, or
        ''' null if there was no corresponding Permission object.
        ''' </summary>
        ''' <returns> the Permission object. </returns>
        Public Overridable ReadOnly Property permission As Permission
            Get
                Return perm
            End Get
        End Property
    End Class

End Namespace