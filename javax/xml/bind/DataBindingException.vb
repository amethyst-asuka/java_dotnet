Imports System

'
' * Copyright (c) 2006, 2013, Oracle and/or its affiliates. All rights reserved.
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

Namespace javax.xml.bind

	''' <summary>
	''' Exception that represents a failure in a JAXB operation.
	''' 
	''' <p>
	''' This exception differs from <seealso cref="JAXBException"/> in that
	''' this is an unchecked exception, while <tt>JAXBException</tt>
	''' is a checked exception.
	''' </summary>
	''' <seealso cref= JAXB
	''' @since JAXB2.1 </seealso>
	Public Class DataBindingException
		Inherits Exception

		Public Sub New(ByVal message As String, ByVal cause As Exception)
			MyBase.New(message, cause)
		End Sub

		Public Sub New(ByVal cause As Exception)
			MyBase.New(cause)
		End Sub
	End Class

End Namespace