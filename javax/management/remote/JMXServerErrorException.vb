Imports System

'
' * Copyright (c) 2002, 2007, Oracle and/or its affiliates. All rights reserved.
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


Namespace javax.management.remote


	' imports for javadoc

	''' <summary>
	''' Exception thrown as the result of a remote <seealso cref="MBeanServer"/>
	''' method invocation when an <code>Error</code> is thrown while
	''' processing the invocation in the remote MBean server.  A
	''' <code>JMXServerErrorException</code> instance contains the original
	''' <code>Error</code> that occurred as its cause.
	''' </summary>
	''' <seealso cref= java.rmi.ServerError
	''' @since 1.5 </seealso>
	Public Class JMXServerErrorException
		Inherits java.io.IOException

		Private Const serialVersionUID As Long = 3996732239558744666L

		''' <summary>
		''' Constructs a <code>JMXServerErrorException</code> with the specified
		''' detail message and nested error.
		''' </summary>
		''' <param name="s"> the detail message. </param>
		''' <param name="err"> the nested error.  An instance of this class can be
		''' constructed where this parameter is null, but the standard
		''' connectors will never do so. </param>
		Public Sub New(ByVal s As String, ByVal err As Exception)
			MyBase.New(s)
			cause = err
		End Sub

		Public Overridable Property cause As Exception
			Get
				Return cause
			End Get
		End Property

		''' <summary>
		''' @serial An <seealso cref="Error"/> that caused this exception to be thrown. </summary>
		''' <seealso cref= #getCause()
		'''  </seealso>
		Private ReadOnly cause As Exception
	End Class

End Namespace