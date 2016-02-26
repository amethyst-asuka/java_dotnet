Imports System

'
' * Copyright (c) 2005, 2010, Oracle and/or its affiliates. All rights reserved.
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

Namespace javax.xml.ws

	''' <summary>
	''' The <code>ProtocolException</code> class is a
	'''  base class for exceptions related to a specific protocol binding. Subclasses
	'''  are used to communicate protocol level fault information to clients and may
	'''  be used on the server to control the protocol specific fault representation.
	''' 
	'''  @since JAX-WS 2.0
	''' 
	''' </summary>
	Public Class ProtocolException
		Inherits WebServiceException

		''' <summary>
		''' Constructs a new protocol exception with <code>null</code> as its detail message. The
		''' cause is not initialized, and may subsequently be initialized by a call
		''' to <code>Throwable.initCause(java.lang.Throwable)</code>.
		''' </summary>
		Public Sub New()
			MyBase.New()
		End Sub

		''' <summary>
		''' Constructs a new protocol exception with the specified detail message.
		''' The cause is not initialized, and may subsequently be initialized by a
		''' call to <code>Throwable.initCause(java.lang.Throwable)</code>.
		''' </summary>
		''' <param name="message"> the detail message. The detail message is saved for later
		'''   retrieval by the Throwable.getMessage() method. </param>
		Public Sub New(ByVal message As String)
			MyBase.New(message)
		End Sub

		''' <summary>
		''' Constructs a new runtime exception with the specified detail message and
		''' cause.
		''' 
		''' Note that the detail message associated with  cause is not automatically
		''' incorporated in  this runtime exception's detail message.
		''' </summary>
		''' <param name="message"> the detail message (which is saved for later retrieval  by
		'''   the Throwable.getMessage() method). </param>
		''' <param name="cause"> the cause (which is saved for later retrieval by the
		''' <code>Throwable.getCause()</code> method). (A <code>null</code> value is  permitted, and indicates
		''' that the cause is nonexistent or  unknown.) </param>
		Public Sub New(ByVal message As String, ByVal cause As Exception)
			MyBase.New(message, cause)
		End Sub

		''' <summary>
		''' Constructs a new runtime exception with the specified cause and a  detail
		''' message of <code>(cause==null ? null : cause.toString())</code>  (which typically
		''' contains the class and detail message of  cause). This constructor is
		''' useful for runtime exceptions  that are little more than wrappers for
		''' other throwables.
		''' </summary>
		''' <param name="cause"> the cause (which is saved for later retrieval by the
		''' <code>Throwable.getCause()</code> method). (A <code>null</code> value is  permitted, and indicates
		''' that the cause is nonexistent or  unknown.) </param>
		Public Sub New(ByVal cause As Exception)
			MyBase.New(cause)
		End Sub
	End Class

End Namespace