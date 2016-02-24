Imports System

'
' * Copyright (c) 2003, 2008, Oracle and/or its affiliates. All rights reserved.
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

Namespace java.lang.instrument

	'
	' * Copyright 2003 Wily Technology, Inc.
	' 

	''' <summary>
	''' Thrown by an implementation of
	''' <seealso cref="java.lang.instrument.ClassFileTransformer#transform ClassFileTransformer.transform"/>
	''' when its input parameters are invalid.
	''' This may occur either because the initial class file bytes were
	''' invalid or a previously applied transform corrupted the bytes.
	''' </summary>
	''' <seealso cref=     java.lang.instrument.ClassFileTransformer#transform
	''' @since   1.5 </seealso>
	Public Class IllegalClassFormatException
		Inherits Exception

		Private Shadows Const serialVersionUID As Long = -3841736710924794009L

		''' <summary>
		''' Constructs an <code>IllegalClassFormatException</code> with no
		''' detail message.
		''' </summary>
		Public Sub New()
			MyBase.New()
		End Sub

		''' <summary>
		''' Constructs an <code>IllegalClassFormatException</code> with the
		''' specified detail message.
		''' </summary>
		''' <param name="s">   the detail message. </param>
		Public Sub New(ByVal s As String)
			MyBase.New(s)
		End Sub
	End Class

End Namespace