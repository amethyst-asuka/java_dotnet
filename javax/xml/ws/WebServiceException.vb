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
	''' The <code>WebServiceException</code> class is the base
	'''  exception class for all JAX-WS API runtime exceptions.
	''' 
	'''  @since JAX-WS 2.0
	''' 
	''' </summary>

	Public Class WebServiceException
		Inherits Exception

	  ''' <summary>
	  ''' Constructs a new exception with <code>null</code> as its
	  '''  detail message. The cause is not initialized.
	  ''' 
	  ''' </summary>
	  Public Sub New()
		MyBase.New()
	  End Sub

	  ''' <summary>
	  ''' Constructs a new exception with the specified detail
	  '''  message.  The cause is not initialized. </summary>
	  '''  <param name="message"> The detail message which is later
	  '''                 retrieved using the getMessage method
	  '''  </param>
	  Public Sub New(ByVal message As String)
		MyBase.New(message)
	  End Sub

	  ''' <summary>
	  ''' Constructs a new exception with the specified detail
	  '''  message and cause.
	  ''' </summary>
	  '''  <param name="message"> The detail message which is later retrieved
	  '''                 using the getMessage method </param>
	  '''  <param name="cause">   The cause which is saved for the later
	  '''                 retrieval throw by the getCause method
	  '''  </param>
	  Public Sub New(ByVal message As String, ByVal cause As Exception)
		MyBase.New(message,cause)
	  End Sub

	  ''' <summary>
	  ''' Constructs a new WebServiceException with the specified cause
	  '''  and a detail message of <tt>(cause==null ? null :
	  '''  cause.toString())</tt> (which typically contains the
	  '''  class and detail message of <tt>cause</tt>).
	  ''' </summary>
	  '''  <param name="cause">   The cause which is saved for the later
	  '''                 retrieval throw by the getCause method.
	  '''                 (A <tt>null</tt> value is permitted, and
	  '''                 indicates that the cause is nonexistent or
	  '''               unknown.)
	  '''  </param>
	  Public Sub New(ByVal cause As Exception)
		MyBase.New(cause)
	  End Sub

	End Class

End Namespace