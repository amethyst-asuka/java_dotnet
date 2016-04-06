'
' * Copyright (c) 2006, Oracle and/or its affiliates. All rights reserved.
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

Namespace java.util.zip

	''' <summary>
	''' Signals that an unrecoverable error has occurred.
	''' 
	''' @author  Dave Bristor
	''' @since   1.6
	''' </summary>
	Public Class ZipError
		Inherits InternalError

		Private Shadows Const serialVersionUID As Long = 853973422266861979L

		''' <summary>
		''' Constructs a ZipError with the given detail message. </summary>
		''' <param name="s"> the {@code String} containing a detail message </param>
		Public Sub New(  s As String)
			MyBase.New(s)
		End Sub
	End Class

End Namespace