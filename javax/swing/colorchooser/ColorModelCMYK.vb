'
' * Copyright (c) 2008, Oracle and/or its affiliates. All rights reserved.
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

Namespace javax.swing.colorchooser

	Friend NotInheritable Class ColorModelCMYK
		Inherits ColorModel

		Friend Sub New()
			MyBase.New("cmyk", "Cyan", "Magenta", "Yellow", "Black", "Alpha") ' NON-NLS: components
		End Sub

		Friend Overrides Sub setColor(ByVal color As Integer, ByVal space As Single())
			MyBase.colorlor(color, space)
			space(4) = space(3)
			RGBtoCMYK(space, space)
		End Sub

		Friend Overrides Function getColor(ByVal space As Single()) As Integer
			CMYKtoRGB(space, space)
			space(3) = space(4)
			Return MyBase.getColor(space)
		End Function

		''' <summary>
		''' Converts CMYK components of a color to a set of RGB components.
		''' </summary>
		''' <param name="cmyk">  a float array with length equal to
		'''              the number of CMYK components </param>
		''' <param name="rgb">   a float array with length of at least 3
		'''              that contains RGB components of a color </param>
		''' <returns> a float array that contains RGB components </returns>
		Private Shared Function CMYKtoRGB(ByVal cmyk As Single(), ByVal rgb As Single()) As Single()
			If rgb Is Nothing Then rgb = New Single(2){}
			rgb(0) = 1.0f + cmyk(0) * cmyk(3) - cmyk(3) - cmyk(0)
			rgb(1) = 1.0f + cmyk(1) * cmyk(3) - cmyk(3) - cmyk(1)
			rgb(2) = 1.0f + cmyk(2) * cmyk(3) - cmyk(3) - cmyk(2)
			Return rgb
		End Function

		''' <summary>
		''' Converts RGB components of a color to a set of CMYK components.
		''' </summary>
		''' <param name="rgb">   a float array with length of at least 3
		'''              that contains RGB components of a color </param>
		''' <param name="cmyk">  a float array with length equal to
		'''              the number of CMYK components </param>
		''' <returns> a float array that contains CMYK components </returns>
		Private Shared Function RGBtoCMYK(ByVal rgb As Single(), ByVal cmyk As Single()) As Single()
			If cmyk Is Nothing Then cmyk = New Single(3){}
			Dim max As Single = ColorModelHSL.max(rgb(0), rgb(1), rgb(2))
			If max > 0.0f Then
				cmyk(0) = 1.0f - rgb(0) / max
				cmyk(1) = 1.0f - rgb(1) / max
				cmyk(2) = 1.0f - rgb(2) / max
			Else
				cmyk(0) = 0.0f
				cmyk(1) = 0.0f
				cmyk(2) = 0.0f
			End If
			cmyk(3) = 1.0f - max
			Return cmyk
		End Function
	End Class

End Namespace