Imports System

'
' * Copyright (c) 1995, 2008, Oracle and/or its affiliates. All rights reserved.
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

Namespace java.lang

	''' <summary>
	''' Thrown to indicate that the <code>clone</code> method in class
	''' <code>Object</code> has been called to clone an object, but that
	''' the object's class does not implement the <code>Cloneable</code>
	''' interface.
	''' <p>
	''' Applications that override the <code>clone</code> method can also
	''' throw this exception to indicate that an object could not or
	''' should not be cloned.
	''' 
	''' @author  unascribed </summary>
	''' <seealso cref=     java.lang.Cloneable </seealso>
	''' <seealso cref=     java.lang.Object#clone()
	''' @since   JDK1.0 </seealso>

	Public Class CloneNotSupportedException
		Inherits Exception

		Private Shadows Const serialVersionUID As Long = 5195511250079656443L

		''' <summary>
		''' Constructs a <code>CloneNotSupportedException</code> with no
		''' detail message.
		''' </summary>
		Public Sub New()
			MyBase.New()
		End Sub

		''' <summary>
		''' Constructs a <code>CloneNotSupportedException</code> with the
		''' specified detail message.
		''' </summary>
		''' <param name="s">   the detail message. </param>
		Public Sub New(ByVal s As String)
			MyBase.New(s)
		End Sub
	End Class

End Namespace