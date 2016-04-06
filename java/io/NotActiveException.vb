'
' * Copyright (c) 1996, 2005, Oracle and/or its affiliates. All rights reserved.
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

Namespace java.io

	''' <summary>
	''' Thrown when serialization or deserialization is not active.
	''' 
	''' @author  unascribed
	''' @since   JDK1.1
	''' </summary>
	Public Class NotActiveException
		Inherits ObjectStreamException

		Private Shadows Const serialVersionUID As Long = -3893467273049808895L

		''' <summary>
		''' Constructor to create a new NotActiveException with the reason given.
		''' </summary>
		''' <param name="reason">  a String describing the reason for the exception. </param>
		Public Sub New(  reason As String)
			MyBase.New(reason)
		End Sub

		''' <summary>
		''' Constructor to create a new NotActiveException without a reason.
		''' </summary>
		Public Sub New()
			MyBase.New()
		End Sub
	End Class

End Namespace