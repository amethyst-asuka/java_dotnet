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
Namespace javax.swing.plaf.nimbus


	''' <summary>
	''' ShadowEffect - base class with all the standard properties for shadow effects
	''' 
	''' @author Created by Jasper Potts (Jun 18, 2007)
	''' </summary>
	Friend MustInherit Class ShadowEffect
		Inherits Effect

		Protected Friend color As java.awt.Color = java.awt.Color.BLACK
		''' <summary>
		''' Opacity a float 0-1 for percentage </summary>
		Protected Friend opacity As Single = 0.75f
		''' <summary>
		''' Angle in degrees between 0-360 </summary>
		Protected Friend angle As Integer = 135
		''' <summary>
		''' Distance in pixels </summary>
		Protected Friend distance As Integer = 5
		''' <summary>
		''' The shadow spread between 0-100 % </summary>
		Protected Friend spread As Integer = 0
		''' <summary>
		''' Size in pixels </summary>
		Protected Friend size As Integer = 5

		' =================================================================================================================
		' Bean methods

		Friend Overridable Property color As java.awt.Color
			Get
				Return color
			End Get
			Set(ByVal color As java.awt.Color)
				Dim old As java.awt.Color = color
				Me.color = color
			End Set
		End Property


		Friend Property Overrides opacity As Single
			Get
				Return opacity
			End Get
			Set(ByVal opacity As Single)
				Dim old As Single = opacity
				Me.opacity = opacity
			End Set
		End Property


		Friend Overridable Property angle As Integer
			Get
				Return angle
			End Get
			Set(ByVal angle As Integer)
				Dim old As Integer = angle
				Me.angle = angle
			End Set
		End Property


		Friend Overridable Property distance As Integer
			Get
				Return distance
			End Get
			Set(ByVal distance As Integer)
				Dim old As Integer = distance
				Me.distance = distance
			End Set
		End Property


		Friend Overridable Property spread As Integer
			Get
				Return spread
			End Get
			Set(ByVal spread As Integer)
				Dim old As Integer = spread
				Me.spread = spread
			End Set
		End Property


		Friend Overridable Property size As Integer
			Get
				Return size
			End Get
			Set(ByVal size As Integer)
				Dim old As Integer = size
				Me.size = size
			End Set
		End Property

	End Class

End Namespace