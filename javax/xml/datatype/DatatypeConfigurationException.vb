Imports System

'
' * Copyright (c) 2004, 2005, Oracle and/or its affiliates. All rights reserved.
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

Namespace javax.xml.datatype

	''' <summary>
	''' <p>Indicates a serious configuration error.</p>
	''' 
	''' @author <a href="mailto:Jeff.Suttor@Sun.com">Jeff Suttor</a>
	''' @since 1.5
	''' </summary>

	Public Class DatatypeConfigurationException
		Inherits Exception

		''' <summary>
		''' <p>Create a new <code>DatatypeConfigurationException</code> with
		''' no specified detail mesage and cause.</p>
		''' </summary>

		Public Sub New()
			MyBase.New()
		End Sub

		''' <summary>
		''' <p>Create a new <code>DatatypeConfigurationException</code> with
		''' the specified detail message.</p>
		''' </summary>
		''' <param name="message"> The detail message. </param>

		Public Sub New(ByVal message As String)
			MyBase.New(message)
		End Sub

			''' <summary>
			''' <p>Create a new <code>DatatypeConfigurationException</code> with
			''' the specified detail message and cause.</p>
			''' </summary>
			''' <param name="message"> The detail message. </param>
			''' <param name="cause"> The cause.  A <code>null</code> value is permitted, and indicates that the cause is nonexistent or unknown. </param>

			Public Sub New(ByVal message As String, ByVal cause As Exception)
					MyBase.New(message, cause)
			End Sub

			''' <summary>
			''' <p>Create a new <code>DatatypeConfigurationException</code> with
			''' the specified cause.</p>
			''' </summary>
			''' <param name="cause"> The cause.  A <code>null</code> value is permitted, and indicates that the cause is nonexistent or unknown. </param>

			Public Sub New(ByVal cause As Exception)
					MyBase.New(cause)
			End Sub
	End Class

End Namespace