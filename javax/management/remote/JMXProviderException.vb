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


	''' <summary>
	''' <p>Exception thrown by <seealso cref="JMXConnectorFactory"/> and
	''' <seealso cref="JMXConnectorServerFactory"/> when a provider exists for
	''' the required protocol but cannot be used for some reason.</p>
	''' </summary>
	''' <seealso cref= JMXConnectorFactory#newJMXConnector </seealso>
	''' <seealso cref= JMXConnectorServerFactory#newJMXConnectorServer
	''' @since 1.5 </seealso>
	Public Class JMXProviderException
		Inherits java.io.IOException

		Private Const serialVersionUID As Long = -3166703627550447198L

		''' <summary>
		''' <p>Constructs a <code>JMXProviderException</code> with no
		''' specified detail message.</p>
		''' </summary>
		Public Sub New()
		End Sub

		''' <summary>
		''' <p>Constructs a <code>JMXProviderException</code> with the
		''' specified detail message.</p>
		''' </summary>
		''' <param name="message"> the detail message </param>
		Public Sub New(ByVal message As String)
			MyBase.New(message)
		End Sub

		''' <summary>
		''' <p>Constructs a <code>JMXProviderException</code> with the
		''' specified detail message and nested exception.</p>
		''' </summary>
		''' <param name="message"> the detail message </param>
		''' <param name="cause"> the nested exception </param>
		Public Sub New(ByVal message As String, ByVal cause As Exception)
			MyBase.New(message)
			Me.cause = cause
		End Sub

		Public Overridable Property cause As Exception
			Get
				Return cause
			End Get
		End Property

		''' <summary>
		''' @serial An exception that caused this exception to be thrown.
		'''         This field may be null. </summary>
		''' <seealso cref= #getCause()
		'''  </seealso>
		Private cause As Exception = Nothing
	End Class

End Namespace