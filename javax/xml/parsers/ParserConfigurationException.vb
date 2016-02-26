Imports System

'
' * Copyright (c) 2000, 2005, Oracle and/or its affiliates. All rights reserved.
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

Namespace javax.xml.parsers

	''' <summary>
	''' Indicates a serious configuration error.
	''' 
	''' @author <a href="mailto:Jeff.Suttor@Sun.com">Jeff Suttor</a>
	''' </summary>

	Public Class ParserConfigurationException
		Inherits Exception

		''' <summary>
		''' Create a new <code>ParserConfigurationException</code> with no
		''' detail mesage.
		''' </summary>

		Public Sub New()
			MyBase.New()
		End Sub

		''' <summary>
		''' Create a new <code>ParserConfigurationException</code> with
		''' the <code>String</code> specified as an error message.
		''' </summary>
		''' <param name="msg"> The error message for the exception. </param>

		Public Sub New(ByVal msg As String)
			MyBase.New(msg)
		End Sub

	End Class

End Namespace