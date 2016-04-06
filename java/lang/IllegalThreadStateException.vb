'
' * Copyright (c) 1994, 2008, Oracle and/or its affiliates. All rights reserved.
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
	''' Thrown to indicate that a thread is not in an appropriate state
	''' for the requested operation. See, for example, the
	''' <code>suspend</code> and <code>resume</code> methods in class
	''' <code>Thread</code>.
	''' 
	''' @author  unascribed </summary>
	''' <seealso cref=     java.lang.Thread#resume() </seealso>
	''' <seealso cref=     java.lang.Thread#suspend()
	''' @since   JDK1.0 </seealso>
	Public Class IllegalThreadStateException
		Inherits IllegalArgumentException

		Private Shadows Const serialVersionUID As Long = -7626246362397460174L

		''' <summary>
		''' Constructs an <code>IllegalThreadStateException</code> with no
		''' detail message.
		''' </summary>
		Public Sub New()
			MyBase.New()
		End Sub

		''' <summary>
		''' Constructs an <code>IllegalThreadStateException</code> with the
		''' specified detail message.
		''' </summary>
		''' <param name="s">   the detail message. </param>
		Public Sub New(  s As String)
			MyBase.New(s)
		End Sub
	End Class

End Namespace