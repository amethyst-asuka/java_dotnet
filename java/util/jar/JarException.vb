'
' * Copyright (c) 1997, 2008, Oracle and/or its affiliates. All rights reserved.
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

Namespace java.util.jar

	''' <summary>
	''' Signals that an error of some sort has occurred while reading from
	''' or writing to a JAR file.
	''' 
	''' @author  David Connelly
	''' @since   1.2
	''' </summary>
	Public Class JarException
		Inherits java.util.zip.ZipException

		Private Shadows Const serialVersionUID As Long = 7159778400963954473L

		''' <summary>
		''' Constructs a JarException with no detail message.
		''' </summary>
		Public Sub New()
		End Sub

		''' <summary>
		''' Constructs a JarException with the specified detail message. </summary>
		''' <param name="s"> the detail message </param>
		Public Sub New(  s As String)
			MyBase.New(s)
		End Sub
	End Class

End Namespace