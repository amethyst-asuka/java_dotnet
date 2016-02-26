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
	''' DerivedColor - A color implementation that is derived from a UIManager
	''' defaults table color and a set of offsets. It can be rederived at any point
	''' by calling rederiveColor(). For example when its parent color changes and it
	''' value will update to reflect the new derived color. Property change events
	''' are fired for the "rgb" property when the derived color changes.
	''' 
	''' @author Jasper Potts
	''' </summary>
	Friend Class DerivedColor
		Inherits java.awt.Color

		Private ReadOnly uiDefaultParentName As String
		Private ReadOnly hOffset, sOffset, bOffset As Single
		Private ReadOnly aOffset As Integer
		Private argbValue As Integer

		Friend Sub New(ByVal uiDefaultParentName As String, ByVal hOffset As Single, ByVal sOffset As Single, ByVal bOffset As Single, ByVal aOffset As Integer)
			MyBase.New(0)
			Me.uiDefaultParentName = uiDefaultParentName
			Me.hOffset = hOffset
			Me.sOffset = sOffset
			Me.bOffset = bOffset
			Me.aOffset = aOffset
		End Sub

		Public Overridable Property uiDefaultParentName As String
			Get
				Return uiDefaultParentName
			End Get
		End Property

		Public Overridable Property hueOffset As Single
			Get
				Return hOffset
			End Get
		End Property

		Public Overridable Property saturationOffset As Single
			Get
				Return sOffset
			End Get
		End Property

		Public Overridable Property brightnessOffset As Single
			Get
				Return bOffset
			End Get
		End Property

		Public Overridable Property alphaOffset As Integer
			Get
				Return aOffset
			End Get
		End Property

		''' <summary>
		''' Recalculate the derived color from the UIManager parent color and offsets
		''' </summary>
		Public Overridable Sub rederiveColor()
			Dim src As java.awt.Color = javax.swing.UIManager.getColor(uiDefaultParentName)
			If src IsNot Nothing Then
				Dim tmp As Single() = java.awt.Color.RGBtoHSB(src.red, src.green, src.blue, Nothing)
				' apply offsets
				tmp(0) = clamp(tmp(0) + hOffset)
				tmp(1) = clamp(tmp(1) + sOffset)
				tmp(2) = clamp(tmp(2) + bOffset)
				Dim alpha As Integer = clamp(src.alpha + aOffset)
				argbValue = (java.awt.Color.HSBtoRGB(tmp(0), tmp(1), tmp(2)) And &HFFFFFF) Or (alpha << 24)
			Else
				Dim tmp As Single() = New Single(2){}
				tmp(0) = clamp(hOffset)
				tmp(1) = clamp(sOffset)
				tmp(2) = clamp(bOffset)
				Dim alpha As Integer = clamp(aOffset)
				argbValue = (java.awt.Color.HSBtoRGB(tmp(0), tmp(1), tmp(2)) And &HFFFFFF) Or (alpha << 24)
			End If
		End Sub

		''' <summary>
		''' Returns the RGB value representing the color in the default sRGB <seealso cref="java.awt.image.ColorModel"/>. (Bits 24-31
		''' are alpha, 16-23 are red, 8-15 are green, 0-7 are blue).
		''' </summary>
		''' <returns> the RGB value of the color in the default sRGB <code>ColorModel</code>. </returns>
		''' <seealso cref= java.awt.image.ColorModel#getRGBdefault </seealso>
		''' <seealso cref= #getRed </seealso>
		''' <seealso cref= #getGreen </seealso>
		''' <seealso cref= #getBlue
		''' @since JDK1.0 </seealso>
		Public Property Overrides rGB As Integer
			Get
				Return argbValue
			End Get
		End Property

		Public Overrides Function Equals(ByVal o As Object) As Boolean
			If Me Is o Then Return True
			If Not(TypeOf o Is DerivedColor) Then Return False
			Dim that As DerivedColor = CType(o, DerivedColor)
			If aOffset <> that.aOffset Then Return False
			If Single.Compare(that.bOffset, bOffset) <> 0 Then Return False
			If Single.Compare(that.hOffset, hOffset) <> 0 Then Return False
			If Single.Compare(that.sOffset, sOffset) <> 0 Then Return False
			If Not uiDefaultParentName.Equals(that.uiDefaultParentName) Then Return False
			Return True
		End Function

		Public Overrides Function GetHashCode() As Integer
			Dim result As Integer = uiDefaultParentName.GetHashCode()
			result = If(31 * result + hOffset <> +0.0f, Single.floatToIntBits(hOffset), 0)
			result = If(31 * result + sOffset <> +0.0f, Single.floatToIntBits(sOffset), 0)
			result = If(31 * result + bOffset <> +0.0f, Single.floatToIntBits(bOffset), 0)
			result = 31 * result + aOffset
			Return result
		End Function

		Private Function clamp(ByVal value As Single) As Single
			If value < 0 Then
				value = 0
			ElseIf value > 1 Then
				value = 1
			End If
			Return value
		End Function

		Private Function clamp(ByVal value As Integer) As Integer
			If value < 0 Then
				value = 0
			ElseIf value > 255 Then
				value = 255
			End If
			Return value
		End Function

		''' <summary>
		''' Returns a string representation of this <code>Color</code>. This method
		''' is intended to be used only for debugging purposes. The content and
		''' format of the returned string might vary between implementations. The
		''' returned string might be empty but cannot be <code>null</code>.
		''' </summary>
		''' <returns> a String representation of this <code>Color</code>. </returns>
		Public Overrides Function ToString() As String
			Dim src As java.awt.Color = javax.swing.UIManager.getColor(uiDefaultParentName)
			Dim s As String = "DerivedColor(color=" & red & "," & green & "," & blue & " parent=" & uiDefaultParentName & " offsets=" & hueOffset & "," & saturationOffset & "," & brightnessOffset & "," & alphaOffset
			Return If(src Is Nothing, s, s & " pColor=" & src.red & "," & src.green & "," & src.blue)
		End Function

		Friend Class UIResource
			Inherits DerivedColor
			Implements javax.swing.plaf.UIResource

			Friend Sub New(ByVal uiDefaultParentName As String, ByVal hOffset As Single, ByVal sOffset As Single, ByVal bOffset As Single, ByVal aOffset As Integer)
				MyBase.New(uiDefaultParentName, hOffset, sOffset, bOffset, aOffset)
			End Sub

			Public Overrides Function Equals(ByVal o As Object) As Boolean
				Return (TypeOf o Is UIResource) AndAlso MyBase.Equals(o)
			End Function

			Public Overrides Function GetHashCode() As Integer
				Return MyBase.GetHashCode() + 7
			End Function
		End Class
	End Class

End Namespace