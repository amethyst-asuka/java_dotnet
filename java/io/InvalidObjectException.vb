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
	''' Indicates that one or more deserialized objects failed validation
	''' tests.  The argument should provide the reason for the failure.
	''' </summary>
	''' <seealso cref= ObjectInputValidation
	''' @since JDK1.1
	''' 
	''' @author  unascribed
	''' @since   JDK1.1 </seealso>
	Public Class InvalidObjectException
		Inherits ObjectStreamException

		Private Shadows Const serialVersionUID As Long = 3233174318281839583L

		''' <summary>
		''' Constructs an <code>InvalidObjectException</code>. </summary>
		''' <param name="reason"> Detailed message explaining the reason for the failure.
		''' </param>
		''' <seealso cref= ObjectInputValidation </seealso>
		Public Sub New(  reason As String)
			MyBase.New(reason)
		End Sub
	End Class

End Namespace