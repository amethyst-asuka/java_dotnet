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
	''' Superclass of all exceptions specific to Object Stream classes.
	''' 
	''' @author  unascribed
	''' @since   JDK1.1
	''' </summary>
	Public MustInherit Class ObjectStreamException
		Inherits IOException

		Private Shadows Const serialVersionUID As Long = 7260898174833392607L

		''' <summary>
		''' Create an ObjectStreamException with the specified argument.
		''' </summary>
		''' <param name="classname"> the detailed message for the exception </param>
		Protected Friend Sub New(  classname As String)
			MyBase.New(classname)
		End Sub

		''' <summary>
		''' Create an ObjectStreamException.
		''' </summary>
		Protected Friend Sub New()
			MyBase.New()
		End Sub
	End Class

End Namespace