'
' * Copyright (c) 1998, 2008, Oracle and/or its affiliates. All rights reserved.
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
	''' Thrown when the Java Virtual Machine attempts to read a class
	''' file and determines that the major and minor version numbers
	''' in the file are not supported.
	''' 
	''' @since   1.2
	''' </summary>
	Public Class UnsupportedClassVersionError
		Inherits ClassFormatError

		Private Shadows Const serialVersionUID As Long = -7123279212883497373L

		''' <summary>
		''' Constructs a <code>UnsupportedClassVersionError</code>
		''' with no detail message.
		''' </summary>
		Public Sub New()
			MyBase.New()
		End Sub

		''' <summary>
		''' Constructs a <code>UnsupportedClassVersionError</code> with
		''' the specified detail message.
		''' </summary>
		''' <param name="s">   the detail message. </param>
		Public Sub New(ByVal s As String)
			MyBase.New(s)
		End Sub
	End Class

End Namespace