'
' * Copyright (c) 2007, 2009, Oracle and/or its affiliates. All rights reserved.
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

Namespace java.nio.file.attribute


	''' <summary>
	''' Checked exception thrown when a lookup of <seealso cref="UserPrincipal"/> fails because
	''' the principal does not exist.
	''' 
	''' @since 1.7
	''' </summary>

	Public Class UserPrincipalNotFoundException
		Inherits java.io.IOException

		Friend Shadows Const serialVersionUID As Long = -5369283889045833024L

		Private ReadOnly name As String

		''' <summary>
		''' Constructs an instance of this class.
		''' </summary>
		''' <param name="name">
		'''          the principal name; may be {@code null} </param>
		Public Sub New(  name As String)
			MyBase.New()
			Me.name = name
		End Sub

		''' <summary>
		''' Returns the user principal name if this exception was created with the
		''' user principal name that was not found, otherwise <tt>null</tt>.
		''' </summary>
		''' <returns>  the user principal name or {@code null} </returns>
		Public Overridable Property name As String
			Get
				Return name
			End Get
		End Property
	End Class

End Namespace