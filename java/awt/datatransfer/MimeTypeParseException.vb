Imports System

'
' * Copyright (c) 1997, 2006, Oracle and/or its affiliates. All rights reserved.
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

Namespace java.awt.datatransfer


	''' <summary>
	'''    A class to encapsulate MimeType parsing related exceptions
	''' 
	''' @serial exclude
	''' @since 1.3
	''' </summary>
	Public Class MimeTypeParseException
		Inherits Exception

		' use serialVersionUID from JDK 1.2.2 for interoperability
		Private Shadows Const serialVersionUID As Long = -5604407764691570741L

		''' <summary>
		''' Constructs a MimeTypeParseException with no specified detail message.
		''' </summary>
		Public Sub New()
			MyBase.New()
		End Sub

		''' <summary>
		''' Constructs a MimeTypeParseException with the specified detail message.
		''' </summary>
		''' <param name="s">   the detail message. </param>
		Public Sub New(ByVal s As String)
			MyBase.New(s)
		End Sub
	End Class ' class MimeTypeParseException

End Namespace