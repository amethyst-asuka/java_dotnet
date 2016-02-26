'
' * Copyright (c) 1999, 2003, Oracle and/or its affiliates. All rights reserved.
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

Namespace javax.management



	''' <summary>
	''' Represents exceptions thrown in the MBean server when performing operations
	''' on MBeans.
	''' 
	''' @since 1.5
	''' </summary>
	Public Class OperationsException
		Inherits JMException

		' Serial version 
		Private Const serialVersionUID As Long = -4967597595580536216L

		''' <summary>
		''' Default constructor.
		''' </summary>
		Public Sub New()
			MyBase.New()
		End Sub

		''' <summary>
		''' Constructor that allows a specific error message to be specified.
		''' </summary>
		''' <param name="message"> the detail message. </param>
		Public Sub New(ByVal message As String)
			MyBase.New(message)
		End Sub

	End Class

End Namespace