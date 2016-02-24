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
	''' Thrown if the Java Virtual Machine cannot find an appropriate
	''' native-language definition of a method declared <code>native</code>.
	''' 
	''' @author unascribed </summary>
	''' <seealso cref=     java.lang.Runtime
	''' @since   JDK1.0 </seealso>
	Public Class UnsatisfiedLinkError
		Inherits LinkageError

		Private Shadows Const serialVersionUID As Long = -4019343241616879428L

		''' <summary>
		''' Constructs an <code>UnsatisfiedLinkError</code> with no detail message.
		''' </summary>
		Public Sub New()
			MyBase.New()
		End Sub

		''' <summary>
		''' Constructs an <code>UnsatisfiedLinkError</code> with the
		''' specified detail message.
		''' </summary>
		''' <param name="s">   the detail message. </param>
		Public Sub New(ByVal s As String)
			MyBase.New(s)
		End Sub
	End Class

End Namespace