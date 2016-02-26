'
' * Copyright (c) 1999, Oracle and/or its affiliates. All rights reserved.
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

Namespace javax.naming.event


	''' <summary>
	''' This class represents an event fired when the procedures/processes
	''' used to collect information for notifying listeners of
	''' <tt>NamingEvent</tt>s threw a <tt>NamingException</tt>.
	''' This can happen, for example, if the server which the listener is using
	''' aborts subsequent to the <tt>addNamingListener()</tt> call.
	'''  
	''' @author Rosanna Lee
	''' @author Scott Seligman
	''' </summary>
	''' <seealso cref= NamingListener#namingExceptionThrown </seealso>
	''' <seealso cref= EventContext
	''' @since 1.3 </seealso>

	Public Class NamingExceptionEvent
		Inherits java.util.EventObject

		''' <summary>
		''' Contains the exception that was thrown
		''' @serial
		''' </summary>
		Private exception As javax.naming.NamingException

		''' <summary>
		''' Constructs an instance of <tt>NamingExceptionEvent</tt> using
		''' the context in which the <tt>NamingException</tt> was thrown and the exception
		''' that was thrown.
		''' </summary>
		''' <param name="source"> The non-null context in which the exception was thrown. </param>
		''' <param name="exc">    The non-null <tt>NamingException</tt> that was thrown.
		'''  </param>
		Public Sub New(ByVal source As EventContext, ByVal exc As javax.naming.NamingException)
			MyBase.New(source)
			exception = exc
		End Sub

		''' <summary>
		''' Retrieves the exception that was thrown. </summary>
		''' <returns> The exception that was thrown. </returns>
	ReadOnly	Public Overridable Property exception As javax.naming.NamingException
			Get
				Return exception
			End Get
		End Property

		''' <summary>
		''' Retrieves the <tt>EventContext</tt> that fired this event.
		''' This returns the same object as <tt>EventObject.getSource()</tt>. </summary>
		''' <returns> The non-null <tt>EventContext</tt> that fired this event. </returns>
	ReadOnly	Public Overridable Property eventContext As EventContext
			Get
				Return CType(source, EventContext)
			End Get
		End Property

		''' <summary>
		''' Invokes the <tt>namingExceptionThrown()</tt> method on
		''' a listener using this event. </summary>
		''' <param name="listener"> The non-null naming listener on which to invoke
		''' the method. </param>
		Public Overridable Sub dispatch(ByVal listener As NamingListener)
			listener.namingExceptionThrown(Me)
		End Sub

		Private Const serialVersionUID As Long = -4877678086134736336L
	End Class

End Namespace