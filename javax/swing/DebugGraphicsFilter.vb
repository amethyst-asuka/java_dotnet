'
' * Copyright (c) 1997, 1998, Oracle and/or its affiliates. All rights reserved.
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
	''' Color filter for DebugGraphics, used for images only.
	'''  
	''' @author Dave Karlton
	''' </summary>
	Friend Class DebugGraphicsFilter
		Inherits RGBImageFilter

		Friend color As Color

		Friend Sub New(ByVal c As Color)
			canFilterIndexColorModel = True
			color = c
		End Sub

		Public Overridable Function filterRGB(ByVal x As Integer, ByVal y As Integer, ByVal rgb As Integer) As Integer
			Return color.rGB Or (rgb And &HFF000000L)
		End Function
	End Class

End Namespace