'
' * Copyright (c) 1998, Oracle and/or its affiliates. All rights reserved.
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
Namespace javax.swing.text


	''' <summary>
	''' ChangedCharSetException as the name indicates is an exception
	''' thrown when the charset is changed.
	''' 
	''' @author Sunita Mani
	''' </summary>
	Public Class ChangedCharSetException
		Inherits java.io.IOException

		Friend charSetSpec As String
		Friend charSetKey As Boolean

		Public Sub New(ByVal charSetSpec As String, ByVal charSetKey As Boolean)
			Me.charSetSpec = charSetSpec
			Me.charSetKey = charSetKey
		End Sub

		Public Overridable Property charSetSpec As String
			Get
				Return charSetSpec
			End Get
		End Property

		Public Overridable Function keyEqualsCharSet() As Boolean
			Return charSetKey
		End Function

	End Class

End Namespace