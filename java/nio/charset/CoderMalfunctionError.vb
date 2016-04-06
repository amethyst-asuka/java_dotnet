Imports System

'
' * Copyright (c) 2001, 2007, Oracle and/or its affiliates. All rights reserved.
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

Namespace java.nio.charset


	''' <summary>
	''' Error thrown when the <seealso cref="CharsetDecoder#decodeLoop decodeLoop"/> method of
	''' a <seealso cref="CharsetDecoder"/>, or the {@link CharsetEncoder#encodeLoop
	''' encodeLoop} method of a <seealso cref="CharsetEncoder"/>, throws an unexpected
	''' exception.
	''' 
	''' @since 1.4
	''' </summary>

	Public Class CoderMalfunctionError
		Inherits [Error]

		Private Shadows Const serialVersionUID As Long = -1151412348057794301L

		''' <summary>
		''' Initializes an instance of this class.
		''' </summary>
		''' <param name="cause">
		'''         The unexpected exception that was thrown </param>
		Public Sub New(  cause As Exception)
			MyBase.New(cause)
		End Sub

	End Class

End Namespace