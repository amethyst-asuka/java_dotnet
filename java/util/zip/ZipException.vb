'
' * Copyright (c) 1995, 2010, Oracle and/or its affiliates. All rights reserved.
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
	''' Signals that a Zip exception of some sort has occurred.
	''' 
	''' @author  unascribed </summary>
	''' <seealso cref=     java.io.IOException
	''' @since   JDK1.0 </seealso>

	Public Class ZipException
		Inherits java.io.IOException

		Private Shadows Const serialVersionUID As Long = 8000196834066748623L

		''' <summary>
		''' Constructs a <code>ZipException</code> with <code>null</code>
		''' as its error detail message.
		''' </summary>
		Public Sub New()
			MyBase.New()
		End Sub

		''' <summary>
		''' Constructs a <code>ZipException</code> with the specified detail
		''' message.
		''' </summary>
		''' <param name="s">   the detail message. </param>

		Public Sub New(  s As String)
			MyBase.New(s)
		End Sub
	End Class

End Namespace