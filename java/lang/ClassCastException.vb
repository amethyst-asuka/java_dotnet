'
' * Copyright (c) 1994, 2013, Oracle and/or its affiliates. All rights reserved.
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
	''' Thrown to indicate that the code has attempted to cast an object
	''' to a subclass of which it is not an instance. For example, the
	''' following code generates a <code>ClassCastException</code>:
	''' <blockquote><pre>
	'''     Object x = new Integer(0);
	'''     System.out.println((String)x);
	''' </pre></blockquote>
	''' 
	''' @author  unascribed
	''' @since   JDK1.0
	''' </summary>
	Public Class ClassCastException
		Inherits RuntimeException

		Private Shadows Const serialVersionUID As Long = -9223365651070458532L

		''' <summary>
		''' Constructs a <code>ClassCastException</code> with no detail message.
		''' </summary>
		Public Sub New()
			MyBase.New()
		End Sub

		''' <summary>
		''' Constructs a <code>ClassCastException</code> with the specified
		''' detail message.
		''' </summary>
		''' <param name="s">   the detail message. </param>
		Public Sub New(  s As String)
			MyBase.New(s)
		End Sub
	End Class

End Namespace