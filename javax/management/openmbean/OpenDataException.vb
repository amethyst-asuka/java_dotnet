'
' * Copyright (c) 2000, 2007, Oracle and/or its affiliates. All rights reserved.
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

Namespace javax.management.openmbean

	' jmx import
	'

	''' <summary>
	''' This checked exception is thrown when an <i>open type</i>, an <i>open data</i>  or an <i>open MBean metadata info</i> instance
	''' could not be constructed because one or more validity constraints were not met.
	''' 
	''' 
	''' @since 1.5
	''' </summary>
	Public Class OpenDataException
		Inherits javax.management.JMException

		Private Const serialVersionUID As Long = 8346311255433349870L

		''' <summary>
		''' An OpenDataException with no detail message.
		''' </summary>
		Public Sub New()
			MyBase.New()
		End Sub

		''' <summary>
		''' An OpenDataException with a detail message.
		''' </summary>
		''' <param name="msg"> the detail message. </param>
		Public Sub New(ByVal msg As String)
			MyBase.New(msg)
		End Sub

	End Class

End Namespace