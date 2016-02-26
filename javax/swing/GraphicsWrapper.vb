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
Namespace javax.swing

	''' <summary>
	''' A private interface to access clip bounds in wrapped Graphics objects.
	''' 
	''' @author Thomas Ball
	''' </summary>


	Friend Interface GraphicsWrapper
		Function subGraphics() As Graphics

		Function isClipIntersecting(ByVal r As Rectangle) As Boolean

		ReadOnly Property clipX As Integer

		ReadOnly Property clipY As Integer

		ReadOnly Property clipWidth As Integer

		ReadOnly Property clipHeight As Integer
	End Interface

End Namespace