'
' * Copyright (c) 2005, 2006, Oracle and/or its affiliates. All rights reserved.
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
	''' Thrown when a serious I/O error has occurred.
	''' 
	''' @author  Xueming Shen
	''' @since   1.6
	''' </summary>
	Public Class IOError
		Inherits [Error]

		''' <summary>
		''' Constructs a new instance of IOError with the specified cause. The
		''' IOError is created with the detail message of
		''' <tt>(cause==null ? null : cause.toString())</tt> (which typically
		''' contains the class and detail message of cause).
		''' </summary>
		''' <param name="cause">
		'''         The cause of this error, or <tt>null</tt> if the cause
		'''         is not known </param>
		Public Sub New(  cause As Throwable)
			MyBase.New(cause)
		End Sub

		Private Shadows Const serialVersionUID As Long = 67100927991680413L
	End Class

End Namespace