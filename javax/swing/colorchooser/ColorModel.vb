Imports Microsoft.VisualBasic
Imports System

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


	Friend Class ColorModel

		Private ReadOnly prefix As String
		Private ReadOnly labels As String()

		Friend Sub New(ByVal name As String, ParamArray ByVal labels As String())
			Me.prefix = "ColorChooser." & name ' NON-NLS: default prefix
			Me.labels = labels
		End Sub

		Friend Sub New()
			Me.New("rgb", "Red", "Green", "Blue", "Alpha") ' NON-NLS: components
		End Sub

		Friend Overridable Sub setColor(ByVal color As Integer, ByVal model As Single())
			model(0) = normalize(color >> 16)
			model(1) = normalize(color >> 8)
			model(2) = normalize(color)
			model(3) = normalize(color >> 24)
		End Sub

		Friend Overridable Function getColor(ByVal model As Single()) As Integer
			Return to8bit(model(2)) Or (to8bit(model(1)) << 8) Or (to8bit(model(0)) << 16) Or (to8bit(model(3)) << 24)
		End Function

		Friend Overridable Property count As Integer
			Get
				Return Me.labels.Length
			End Get
		End Property

		Friend Overridable Function getMinimum(ByVal index As Integer) As Integer
			Return 0
		End Function

		Friend Overridable Function getMaximum(ByVal index As Integer) As Integer
			Return 255
		End Function

		Friend Overridable Function getDefault(ByVal index As Integer) As Single
			Return 0.0f
		End Function

		Friend Function getLabel(ByVal component As java.awt.Component, ByVal index As Integer) As String
			Return getText(component, Me.labels(index))
		End Function

		Private Shared Function normalize(ByVal value As Integer) As Single
			Return CSng(value And &HFF) / 255.0f
		End Function

		Private Shared Function to8bit(ByVal value As Single) As Integer
			Return CInt(Fix(255.0f * value))
		End Function

		Friend Function getText(ByVal component As java.awt.Component, ByVal suffix As String) As String
			Return javax.swing.UIManager.getString(Me.prefix + suffix & "Text", component.locale) ' NON-NLS: default postfix
		End Function

		Friend Function getInteger(ByVal component As java.awt.Component, ByVal suffix As String) As Integer
			Dim value As Object = javax.swing.UIManager.get(Me.prefix + suffix, component.locale)
			If TypeOf value Is Integer? Then Return CInt(Fix(value))
			If TypeOf value Is String Then
				Try
					Return Convert.ToInt32(CStr(value))
				Catch exception As NumberFormatException
				End Try
			End If
			Return -1
		End Function
	End Class

End Namespace