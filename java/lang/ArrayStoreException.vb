'
' * Copyright (c) 1995, 2013, Oracle and/or its affiliates. All rights reserved.
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
	''' Thrown to indicate that an attempt has been made to store the
	''' wrong type of object into an array of objects. For example, the
	''' following code generates an <code>ArrayStoreException</code>:
	''' <blockquote><pre>
	'''     Object x[] = new String[3];
	'''     x[0] = new Integer(0);
	''' </pre></blockquote>
	''' 
	''' @author  unascribed
	''' @since   JDK1.0
	''' </summary>
	Public Class ArrayStoreException
		Inherits RuntimeException

		Private Shadows Const serialVersionUID As Long = -4522193890499838241L

		''' <summary>
		''' Constructs an <code>ArrayStoreException</code> with no detail message.
		''' </summary>
		Public Sub New()
			MyBase.New()
		End Sub

		''' <summary>
		''' Constructs an <code>ArrayStoreException</code> with the specified
		''' detail message.
		''' </summary>
		''' <param name="s">   the detail message. </param>
		Public Sub New(  s As String)
			MyBase.New(s)
		End Sub
	End Class

End Namespace