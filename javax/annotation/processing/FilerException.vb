'
' * Copyright (c) 2005, 2012, Oracle and/or its affiliates. All rights reserved.
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

Namespace javax.annotation.processing


	''' <summary>
	''' Indicates a <seealso cref="Filer"/> detected an attempt to open a file that
	''' would violate the guarantees provided by the {@code Filer}.  Those
	''' guarantees include not creating the same file more than once, not
	''' creating multiple files corresponding to the same type, and not
	''' creating files for types with invalid names.
	''' 
	''' @author Joseph D. Darcy
	''' @author Scott Seligman
	''' @author Peter von der Ah&eacute;
	''' @since 1.6
	''' </summary>
	Public Class FilerException
		Inherits java.io.IOException

		Friend Const serialVersionUID As Long = 8426423106453163293L
		''' <summary>
		''' Constructs an exception with the specified detail message. </summary>
		''' <param name="s"> the detail message, which should include the name of
		''' the file attempting to be opened; may be {@code null} </param>
		Public Sub New(ByVal s As String)
			MyBase.New(s)
		End Sub
	End Class

End Namespace