'
' * Copyright (c) 2000, 2004, Oracle and/or its affiliates. All rights reserved.
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
	''' Thrown when code that is dependent on a keyboard, display, or mouse
	''' is called in an environment that does not support a keyboard, display,
	''' or mouse.
	''' 
	''' @since 1.4
	''' @author  Michael Martak
	''' </summary>
	Public Class HeadlessException
		Inherits UnsupportedOperationException

	'    
	'     * JDK 1.4 serialVersionUID
	'     
		Private Shadows Const serialVersionUID As Long = 167183644944358563L
		Public Sub New()
		End Sub
		Public Sub New(ByVal msg As String)
			MyBase.New(msg)
		End Sub
		Public Property Overrides message As String
			Get
				Dim superMessage As String = MyBase.message
				Dim headlessMessage As String = GraphicsEnvironment.headlessMessage
    
				If superMessage Is Nothing Then
					Return headlessMessage
				ElseIf headlessMessage Is Nothing Then
					Return superMessage
				Else
					Return superMessage + headlessMessage
				End If
			End Get
		End Property
	End Class

End Namespace