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

	Friend NotInheritable Class ColorModelHSL
		Inherits ColorModel

		Friend Sub New()
			MyBase.New("hsl", "Hue", "Saturation", "Lightness", "Transparency") ' NON-NLS: components
		End Sub

		Friend Overrides Sub setColor(ByVal color As Integer, ByVal space As Single())
			MyBase.colorlor(color, space)
			RGBtoHSL(space, space)
			space(3) = 1.0f - space(3)
		End Sub

		Friend Overrides Function getColor(ByVal space As Single()) As Integer
			space(3) = 1.0f - space(3)
			HSLtoRGB(space, space)
			Return MyBase.getColor(space)
		End Function

		Friend Overrides Function getMaximum(ByVal index As Integer) As Integer
			Return If(index = 0, 360, 100)
		End Function

		Friend Overrides Function getDefault(ByVal index As Integer) As Single
			Return If(index = 0, -1.0f, If(index = 2, 0.5f, 1.0f))
		End Function

		''' <summary>
		''' Converts HSL components of a color to a set of RGB components.
		''' </summary>
		''' <param name="hsl">  a float array with length equal to
		'''             the number of HSL components </param>
		''' <param name="rgb">  a float array with length of at least 3
		'''             that contains RGB components of a color </param>
		''' <returns> a float array that contains RGB components </returns>
		Private Shared Function HSLtoRGB(ByVal hsl As Single(), ByVal rgb As Single()) As Single()
			If rgb Is Nothing Then rgb = New Single(2){}
			Dim ___hue As Single = hsl(0)
			Dim saturation As Single = hsl(1)
			Dim lightness As Single = hsl(2)

			If saturation > 0.0f Then
				___hue = If(___hue < 1.0f, ___hue * 6.0f, 0.0f)
				Dim q As Single = lightness + saturation * (If(lightness > 0.5f, 1.0f - lightness, lightness))
				Dim p As Single = 2.0f * lightness - q
				rgb(0)= normalize(q, p,If(___hue < 4.0f, (___hue + 2.0f), (___hue - 4.0f)))
				rgb(1)= normalize(q, p, ___hue)
				rgb(2)= normalize(q, p,If(___hue < 2.0f, (___hue + 4.0f), (___hue - 2.0f)))
			Else
				rgb(0) = lightness
				rgb(1) = lightness
				rgb(2) = lightness
			End If
			Return rgb
		End Function

		''' <summary>
		''' Converts RGB components of a color to a set of HSL components.
		''' </summary>
		''' <param name="rgb">  a float array with length of at least 3
		'''             that contains RGB components of a color </param>
		''' <param name="hsl">  a float array with length equal to
		'''             the number of HSL components </param>
		''' <returns> a float array that contains HSL components </returns>
		Private Shared Function RGBtoHSL(ByVal rgb As Single(), ByVal hsl As Single()) As Single()
			If hsl Is Nothing Then hsl = New Single(2){}
			Dim max As Single = max(rgb(0), rgb(1), rgb(2))
			Dim min As Single = min(rgb(0), rgb(1), rgb(2))

			Dim summa As Single = max + min
			Dim saturation As Single = max - min
			If saturation > 0.0f Then saturation /= If(summa > 1.0f, 2.0f - summa, summa)
			hsl(0) = getHue(rgb(0), rgb(1), rgb(2), max, min)
			hsl(1) = saturation
			hsl(2) = summa / 2.0f
			Return hsl
		End Function

		''' <summary>
		''' Returns the smaller of three color components.
		''' </summary>
		''' <param name="red">    the red component of the color </param>
		''' <param name="green">  the green component of the color </param>
		''' <param name="blue">   the blue component of the color </param>
		''' <returns> the smaller of {@code red}, {@code green} and {@code blue} </returns>
		Friend Shared Function min(ByVal red As Single, ByVal green As Single, ByVal blue As Single) As Single
			Dim ___min As Single = If(red < green, red, green)
			Return If(___min < blue, ___min, blue)
		End Function

		''' <summary>
		''' Returns the larger of three color components.
		''' </summary>
		''' <param name="red">    the red component of the color </param>
		''' <param name="green">  the green component of the color </param>
		''' <param name="blue">   the blue component of the color </param>
		''' <returns> the larger of {@code red}, {@code green} and {@code blue} </returns>
		Friend Shared Function max(ByVal red As Single, ByVal green As Single, ByVal blue As Single) As Single
			Dim ___max As Single = If(red > green, red, green)
			Return If(___max > blue, ___max, blue)
		End Function

		''' <summary>
		''' Calculates the hue component for HSL and HSV color spaces.
		''' </summary>
		''' <param name="red">    the red component of the color </param>
		''' <param name="green">  the green component of the color </param>
		''' <param name="blue">   the blue component of the color </param>
		''' <param name="max">    the larger of {@code red}, {@code green} and {@code blue} </param>
		''' <param name="min">    the smaller of {@code red}, {@code green} and {@code blue} </param>
		''' <returns> the hue component </returns>
		Friend Shared Function getHue(ByVal red As Single, ByVal green As Single, ByVal blue As Single, ByVal max As Single, ByVal min As Single) As Single
			Dim ___hue As Single = max - min
			If ___hue > 0.0f Then
				If max = red Then
					___hue = (green - blue) / ___hue
					If ___hue < 0.0f Then ___hue += 6.0f
				ElseIf max = green Then
					___hue = 2.0f + (blue - red) / ___hue
				Else 'max == blue
					___hue = 4.0f + (red - green) / ___hue
				End If
				___hue /= 6.0f
			End If
			Return ___hue
		End Function

		Private Shared Function normalize(ByVal q As Single, ByVal p As Single, ByVal color As Single) As Single
			If color < 1.0f Then Return p + (q - p) * color
			If color < 3.0f Then Return q
			If color < 4.0f Then Return p + (q - p) * (4.0f - color)
			Return p
		End Function
	End Class

End Namespace