'
' * Copyright (c) 1996, 1997, Oracle and/or its affiliates. All rights reserved.
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

Namespace java.awt

	''' <summary>
	''' Signals that an AWT component is not in an appropriate state for
	''' the requested operation.
	''' 
	''' @author      Jonni Kanerva
	''' </summary>
	Public Class IllegalComponentStateException
		Inherits IllegalStateException

	'    
	'     * JDK 1.1 serialVersionUID
	'     
		 Private Shadows Const serialVersionUID As Long = -1889339587208144238L

		''' <summary>
		''' Constructs an IllegalComponentStateException with no detail message.
		''' A detail message is a String that describes this particular exception.
		''' </summary>
		Public Sub New()
			MyBase.New()
		End Sub

		''' <summary>
		''' Constructs an IllegalComponentStateException with the specified detail
		''' message.  A detail message is a String that describes this particular
		''' exception. </summary>
		''' <param name="s"> the String that contains a detailed message </param>
		Public Sub New(  s As String)
			MyBase.New(s)
		End Sub
	End Class

End Namespace