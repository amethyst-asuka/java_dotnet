'
' * Copyright (c) 1997, 2003, Oracle and/or its affiliates. All rights reserved.
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

Namespace java.security.spec


	''' <summary>
	''' This is the exception for invalid parameter specifications.
	''' 
	''' @author Jan Luehe
	''' 
	''' </summary>
	''' <seealso cref= java.security.AlgorithmParameters </seealso>
	''' <seealso cref= AlgorithmParameterSpec </seealso>
	''' <seealso cref= DSAParameterSpec
	''' 
	''' @since 1.2 </seealso>

	Public Class InvalidParameterSpecException
		Inherits java.security.GeneralSecurityException

		Private Shadows Const serialVersionUID As Long = -970468769593399342L

		''' <summary>
		''' Constructs an InvalidParameterSpecException with no detail message. A
		''' detail message is a String that describes this particular
		''' exception.
		''' </summary>
		Public Sub New()
			MyBase.New()
		End Sub

		''' <summary>
		''' Constructs an InvalidParameterSpecException with the specified detail
		''' message. A detail message is a String that describes this
		''' particular exception.
		''' </summary>
		''' <param name="msg"> the detail message. </param>
		Public Sub New(  msg As String)
			MyBase.New(msg)
		End Sub
	End Class

End Namespace