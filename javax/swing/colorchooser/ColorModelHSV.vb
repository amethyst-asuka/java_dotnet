Imports Microsoft.VisualBasic

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

	Friend NotInheritable Class ColorModelHSV
		Inherits ColorModel

		Friend Sub New()
			MyBase.New("hsv", "Hue", "Saturation", "Value", "Transparency") ' NON-NLS: components
		End Sub

		Friend Overrides Sub setColor(ByVal color As Integer, ByVal space As Single())
			MyBase.colorlor(color, space)
			RGBtoHSV(space, space)
			space(3) = 1.0f - space(3)
		End Sub

		Friend Overrides Function getColor(ByVal space As Single()) As Integer
			space(3) = 1.0f - space(3)
			HSVtoRGB(space, space)
			Return MyBase.getColor(space)
		End Function

		Friend Overrides Function getMaximum(ByVal index As Integer) As Integer
			Return If(index = 0, 360, 100)
		End Function

		Friend Overrides Function getDefault(ByVal index As Integer) As Single
			Return If(index = 0, -1.0f, 1.0f)
		End Function

		''' <summary>
		''' Converts HSV components of a color to a set of RGB components.
		''' </summary>
		''' <param name="hsv">  a float array with length equal to
		'''             the number of HSV components </param>
		''' <param name="rgb">  a float array with length of at least 3
		'''             that contains RGB components of a color </param>
		''' <returns> a float array that contains RGB components </returns>
		Private Shared Function HSVtoRGB(ByVal hsv As Single(), ByVal rgb As Single()) As Single()
			If rgb Is Nothing Then rgb = New Single(2){}
			Dim hue As Single = hsv(0)
			Dim saturation As Single = hsv(1)
			Dim value As Single = hsv(2)

			rgb(0) = value
			rgb(1) = value
			rgb(2) = value

			If saturation > 0.0f Then
				hue = If(hue < 1.0f, hue * 6.0f, 0.0f)
				Dim ___integer As Integer = CInt(Fix(hue))
				Dim f As Single = hue - CSng(___integer)
				Select Case ___integer
					Case 0
						rgb(1) *= 1.0f - saturation * (1.0f - f)
						rgb(2) *= 1.0f - saturation
					Case 1
						rgb(0) *= 1.0f - saturation * f
						rgb(2) *= 1.0f - saturation
					Case 2
						rgb(0) *= 1.0f - saturation
						rgb(2) *= 1.0f - saturation * (1.0f - f)
					Case 3
						rgb(0) *= 1.0f - saturation
						rgb(1) *= 1.0f - saturation * f
					Case 4
						rgb(0) *= 1.0f - saturation * (1.0f - f)
						rgb(1) *= 1.0f - saturation
					Case 5
						rgb(1) *= 1.0f - saturation
						rgb(2) *= 1.0f - saturation * f
				End Select
			End If
			Return rgb
		End Function

		''' <summary>
		''' Converts RGB components of a color to a set of HSV components.
		''' </summary>
		''' <param name="rgb">  a float array with length of at least 3
		'''             that contains RGB components of a color </param>
		''' <param name="hsv">  a float array with length equal to
		'''             the number of HSV components </param>
		''' <returns> a float array that contains HSV components </returns>
		Private Shared Function RGBtoHSV(ByVal rgb As Single(), ByVal hsv As Single()) As Single()
			If hsv Is Nothing Then hsv = New Single(2){}
			Dim max As Single = ColorModelHSL.max(rgb(0), rgb(1), rgb(2))
			Dim min As Single = ColorModelHSL.min(rgb(0), rgb(1), rgb(2))

			Dim saturation As Single = max - min
			If saturation > 0.0f Then saturation /= max
			hsv(0) = ColorModelHSL.getHue(rgb(0), rgb(1), rgb(2), max, min)
			hsv(1) = saturation
			hsv(2) = max
			Return hsv
		End Function
	End Class

End Namespace