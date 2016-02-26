Imports System
Imports System.Collections.Generic

'
' * Copyright (c) 1997, 2008, Oracle and/or its affiliates. All rights reserved.
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
	''' Class used by DebugGraphics for maintaining information about how
	''' to render graphics calls.
	'''  
	''' @author Dave Karlton
	''' </summary>
	Friend Class DebugGraphicsInfo
		Friend flashColor As Color = Color.red
		Friend flashTime As Integer = 100
		Friend flashCount As Integer = 2
		Friend componentToDebug As Dictionary(Of JComponent, Integer?)
		Friend debugFrame As JFrame = Nothing
		Friend stream As java.io.PrintStream = System.out

		Friend Overridable Sub setDebugOptions(ByVal component As JComponent, ByVal debug As Integer)
			If debug = 0 Then Return
			If componentToDebug Is Nothing Then componentToDebug = New Dictionary(Of JComponent, Integer?)
			If debug > 0 Then
				componentToDebug(component) = Convert.ToInt32(debug)
			Else
				componentToDebug.Remove(component)
			End If
		End Sub

		Friend Overridable Function getDebugOptions(ByVal component As JComponent) As Integer
			If componentToDebug Is Nothing Then
				Return 0
			Else
				Dim [integer] As Integer? = componentToDebug(component)

				Return If([integer] Is Nothing, 0, [integer])
			End If
		End Function

		Friend Overridable Sub log(ByVal [string] As String)
			stream.println([string])
		End Sub
	End Class

End Namespace