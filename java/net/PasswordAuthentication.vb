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

Namespace java.net


	''' <summary>
	''' The class PasswordAuthentication is a data holder that is used by
	''' Authenticator.  It is simply a repository for a user name and a password.
	''' </summary>
	''' <seealso cref= java.net.Authenticator </seealso>
	''' <seealso cref= java.net.Authenticator#getPasswordAuthentication()
	''' 
	''' @author  Bill Foote
	''' @since   1.2 </seealso>

	Public NotInheritable Class PasswordAuthentication

		Private userName As String
		Private password As Char()

		''' <summary>
		''' Creates a new {@code PasswordAuthentication} object from the given
		''' user name and password.
		''' 
		''' <p> Note that the given user password is cloned before it is stored in
		''' the new {@code PasswordAuthentication} object.
		''' </summary>
		''' <param name="userName"> the user name </param>
		''' <param name="password"> the user's password </param>
		Public Sub New(  userName As String,   password As Char())
			Me.userName = userName
			Me.password = password.clone()
		End Sub

		''' <summary>
		''' Returns the user name.
		''' </summary>
		''' <returns> the user name </returns>
		Public Property userName As String
			Get
				Return userName
			End Get
		End Property

		''' <summary>
		''' Returns the user password.
		''' 
		''' <p> Note that this method returns a reference to the password. It is
		''' the caller's responsibility to zero out the password information after
		''' it is no longer needed.
		''' </summary>
		''' <returns> the password </returns>
		Public Property password As Char()
			Get
				Return password
			End Get
		End Property
	End Class

End Namespace