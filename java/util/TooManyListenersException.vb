Imports System

'
' * Copyright (c) 1996, 2008, Oracle and/or its affiliates. All rights reserved.
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

Namespace java.util

	''' <summary>
	''' <p>
	''' The <code> TooManyListenersException </code> Exception is used as part of
	''' the Java Event model to annotate and implement a unicast special case of
	''' a multicast Event Source.
	''' </p>
	''' <p>
	''' The presence of a "throws TooManyListenersException" clause on any given
	''' concrete implementation of the normally multicast "void addXyzEventListener"
	''' event listener registration pattern is used to annotate that interface as
	''' implementing a unicast Listener special case, that is, that one and only
	''' one Listener may be registered on the particular event listener source
	''' concurrently.
	''' </p>
	''' </summary>
	''' <seealso cref= java.util.EventObject </seealso>
	''' <seealso cref= java.util.EventListener
	''' 
	''' @author Laurence P. G. Cable
	''' @since  JDK1.1 </seealso>

	Public Class TooManyListenersException
		Inherits Exception

		Private Shadows Const serialVersionUID As Long = 5074640544770687831L

		''' <summary>
		''' Constructs a TooManyListenersException with no detail message.
		''' A detail message is a String that describes this particular exception.
		''' </summary>

		Public Sub New()
			MyBase.New()
		End Sub

		''' <summary>
		''' Constructs a TooManyListenersException with the specified detail message.
		''' A detail message is a String that describes this particular exception. </summary>
		''' <param name="s"> the detail message </param>

		Public Sub New(ByVal s As String)
			MyBase.New(s)
		End Sub
	End Class

End Namespace