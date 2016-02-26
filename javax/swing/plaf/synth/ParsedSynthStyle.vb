Imports System
Imports System.Collections.Generic
Imports System.Text

'
' * Copyright (c) 2003, 2005, Oracle and/or its affiliates. All rights reserved.
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
Namespace javax.swing.plaf.synth



	''' <summary>
	''' ParsedSynthStyle are the SynthStyle's that SynthParser creates.
	''' 
	''' @author Scott Violet
	''' </summary>
	Friend Class ParsedSynthStyle
		Inherits sun.swing.plaf.synth.DefaultSynthStyle

		Private Shared DELEGATING_PAINTER_INSTANCE As SynthPainter = New DelegatingPainter
		Private _painters As PainterInfo()

		Private Shared Function mergePainterInfo(ByVal old As PainterInfo(), ByVal newPI As PainterInfo()) As PainterInfo()
			If old Is Nothing Then Return newPI
			If newPI Is Nothing Then Return old
			Dim oldLength As Integer = old.Length
			Dim newLength As Integer = newPI.Length
			Dim dups As Integer = 0
			Dim merged As PainterInfo() = New PainterInfo(oldLength + newLength - 1){}
			Array.Copy(old, 0, merged, 0, oldLength)
			For newCounter As Integer = 0 To newLength - 1
				Dim found As Boolean = False
				For oldCounter As Integer = 0 To oldLength - dups - 1
					If newPI(newCounter).equalsPainter(old(oldCounter)) Then
						merged(oldCounter) = newPI(newCounter)
						dups += 1
						found = True
						Exit For
					End If
				Next oldCounter
				If Not found Then merged(oldLength + newCounter - dups) = newPI(newCounter)
			Next newCounter
			If dups > 0 Then
				Dim tmp As PainterInfo() = merged
				merged = New PainterInfo(merged.Length - dups - 1){}
				Array.Copy(tmp, 0, merged, 0, merged.Length)
			End If
			Return merged
		End Function


		Public Sub New()
		End Sub

		Public Sub New(ByVal style As sun.swing.plaf.synth.DefaultSynthStyle)
			MyBase.New(style)
			If TypeOf style Is ParsedSynthStyle Then
				Dim pStyle As ParsedSynthStyle = CType(style, ParsedSynthStyle)

				If pStyle._painters IsNot Nothing Then _painters = pStyle._painters
			End If
		End Sub

		Public Overridable Function getPainter(ByVal ss As SynthContext) As SynthPainter
			Return DELEGATING_PAINTER_INSTANCE
		End Function

'JAVA TO VB CONVERTER TODO TASK: The following line could not be converted:
        Public Overridable Sub setPainters(ByVal info As PainterInfo[]) 'JavaToDotNetTempPropertySetpainters
		Public Overridable Property painters As PainterInfo()
			Set(ByVal info As PainterInfo())
				_painters = info
			End Set
			Get
		End Property

		Public Overridable Function addTo(ByVal style As sun.swing.plaf.synth.DefaultSynthStyle) As sun.swing.plaf.synth.DefaultSynthStyle
			If Not(TypeOf style Is ParsedSynthStyle) Then style = New ParsedSynthStyle(style)
			Dim pStyle As ParsedSynthStyle = CType(MyBase.addTo(style), ParsedSynthStyle)
			pStyle._painters = mergePainterInfo(pStyle._painters, _painters)
			Return pStyle
		End Function

		Private Function getBestPainter(ByVal context As SynthContext, ByVal method As String, ByVal direction As Integer) As SynthPainter
			' Check the state info first
			Dim info As StateInfo = CType(getStateInfo(context.componentState), StateInfo)
			Dim ___painter As SynthPainter
			If info IsNot Nothing Then
				___painter = getBestPainter(info.painters, method, direction)
				If ___painter IsNot Nothing Then Return ___painter
			End If
			___painter = getBestPainter(_painters, method, direction)
			If ___painter IsNot Nothing Then Return ___painter
			Return SynthPainter.NULL_PAINTER
		End Function

		Private Function getBestPainter(ByVal info As PainterInfo(), ByVal method As String, ByVal direction As Integer) As SynthPainter
			If info IsNot Nothing Then
				' Painter specified with no method
				Dim nullPainter As SynthPainter = Nothing
				' Painter specified for this method
				Dim methodPainter As SynthPainter = Nothing

				For counter As Integer = info.Length - 1 To 0 Step -1
					Dim pi As PainterInfo = info(counter)

					If pi.method = method Then
						If pi.direction = direction Then
							Return pi.painter
						ElseIf methodPainter Is Nothing AndAlso pi.direction = -1 Then
							methodPainter = pi.painter
						End If
					ElseIf nullPainter Is Nothing AndAlso pi.method Is Nothing Then
						nullPainter = pi.painter
					End If
				Next counter
				If methodPainter IsNot Nothing Then Return methodPainter
				Return nullPainter
			End If
			Return Nothing
		End Function

		Public Overrides Function ToString() As String
			Dim text As New StringBuilder(MyBase.ToString())
			If _painters IsNot Nothing Then
				text.Append(",painters=[")
				For i As Integer = 0 To +_painters.Length - 1
					text.Append(_painters(i).ToString())
				Next i
				text.Append("]")
			End If
			Return text.ToString()
		End Function


		Friend Class StateInfo
			Inherits sun.swing.plaf.synth.DefaultSynthStyle.StateInfo

			Private _painterInfo As PainterInfo()

			Public Sub New()
			End Sub

			Public Sub New(ByVal info As sun.swing.plaf.synth.DefaultSynthStyle.StateInfo)
				MyBase.New(info)
				If TypeOf info Is StateInfo Then _painterInfo = CType(info, StateInfo)._painterInfo
			End Sub

			Public Overridable Property painters As PainterInfo()
				Set(ByVal painterInfo As PainterInfo())
					_painterInfo = painterInfo
				End Set
			End Property

				Return _painterInfo
			End Function

			Public Overridable Function clone() As Object
				Return New StateInfo(Me)
			End Function

			Public Overridable Function addTo(ByVal info As sun.swing.plaf.synth.DefaultSynthStyle.StateInfo) As sun.swing.plaf.synth.DefaultSynthStyle.StateInfo
				If Not(TypeOf info Is StateInfo) Then
					info = New StateInfo(info)
				Else
					info = MyBase.addTo(info)
					Dim si As StateInfo = CType(info, StateInfo)
					si._painterInfo = mergePainterInfo(si._painterInfo, _painterInfo)
				End If
				Return info
			End Function

			Public Overrides Function ToString() As String
				Dim text As New StringBuilder(MyBase.ToString())
				text.Append(",painters=[")
				If _painterInfo IsNot Nothing Then
					For i As Integer = 0 To +_painterInfo.Length - 1
						text.Append("    ").append(_painterInfo(i).ToString())
					Next i
				End If
				text.Append("]")
				Return text.ToString()
			End Function
		End Class


		Friend Class PainterInfo
			Private _method As String
			Private _painter As SynthPainter
			Private _direction As Integer

			Friend Sub New(ByVal method As String, ByVal painter As SynthPainter, ByVal direction As Integer)
				If method IsNot Nothing Then _method = method.intern()
				_painter = painter
				_direction = direction
			End Sub

			Friend Overridable Sub addPainter(ByVal painter As SynthPainter)
				If Not(TypeOf _painter Is AggregatePainter) Then _painter = New AggregatePainter(_painter)

				CType(_painter, AggregatePainter).addPainter(painter)
			End Sub

			Friend Overridable Property method As String
				Get
					Return _method
				End Get
			End Property

			Friend Overridable Property painter As SynthPainter
				Get
					Return _painter
				End Get
			End Property

			Friend Overridable Property direction As Integer
				Get
					Return _direction
				End Get
			End Property

			Friend Overridable Function equalsPainter(ByVal info As PainterInfo) As Boolean
				Return (_method = info._method AndAlso _direction = info._direction)
			End Function

			Public Overrides Function ToString() As String
				Return "PainterInfo {method=" & _method & ",direction=" & _direction & ",painter=" & _painter & "}"
			End Function
		End Class

		Private Class AggregatePainter
			Inherits SynthPainter

			Private painters As IList(Of SynthPainter)

			Friend Sub New(ByVal painter As SynthPainter)
				painters = New LinkedList(Of SynthPainter)
				painters.Add(painter)
			End Sub

			Friend Overridable Sub addPainter(ByVal painter As SynthPainter)
				If painter IsNot Nothing Then painters.Add(painter)
			End Sub

			Public Overridable Sub paintArrowButtonBackground(ByVal context As SynthContext, ByVal g As java.awt.Graphics, ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer)
				For Each painter As SynthPainter In painters
					painter.paintArrowButtonBackground(context, g, x, y, w, h)
				Next painter
			End Sub

			Public Overridable Sub paintArrowButtonBorder(ByVal context As SynthContext, ByVal g As java.awt.Graphics, ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer)
				For Each painter As SynthPainter In painters
					painter.paintArrowButtonBorder(context, g, x, y, w, h)
				Next painter
			End Sub

			Public Overridable Sub paintArrowButtonForeground(ByVal context As SynthContext, ByVal g As java.awt.Graphics, ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer, ByVal direction As Integer)
				For Each painter As SynthPainter In painters
					painter.paintArrowButtonForeground(context, g, x, y, w, h, direction)
				Next painter
			End Sub

			Public Overridable Sub paintButtonBackground(ByVal context As SynthContext, ByVal g As java.awt.Graphics, ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer)
				For Each painter As SynthPainter In painters
					painter.paintButtonBackground(context, g, x, y, w, h)
				Next painter
			End Sub

			Public Overridable Sub paintButtonBorder(ByVal context As SynthContext, ByVal g As java.awt.Graphics, ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer)
				For Each painter As SynthPainter In painters
					painter.paintButtonBorder(context, g, x, y, w, h)
				Next painter
			End Sub

			Public Overridable Sub paintCheckBoxMenuItemBackground(ByVal context As SynthContext, ByVal g As java.awt.Graphics, ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer)
				For Each painter As SynthPainter In painters
					painter.paintCheckBoxMenuItemBackground(context, g, x, y, w, h)
				Next painter
			End Sub

			Public Overridable Sub paintCheckBoxMenuItemBorder(ByVal context As SynthContext, ByVal g As java.awt.Graphics, ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer)
				For Each painter As SynthPainter In painters
					painter.paintCheckBoxMenuItemBorder(context, g, x, y, w, h)
				Next painter
			End Sub

			Public Overridable Sub paintCheckBoxBackground(ByVal context As SynthContext, ByVal g As java.awt.Graphics, ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer)
				For Each painter As SynthPainter In painters
					painter.paintCheckBoxBackground(context, g, x, y, w, h)
				Next painter
			End Sub

			Public Overridable Sub paintCheckBoxBorder(ByVal context As SynthContext, ByVal g As java.awt.Graphics, ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer)
				For Each painter As SynthPainter In painters
					painter.paintCheckBoxBorder(context, g, x, y, w, h)
				Next painter
			End Sub

			Public Overridable Sub paintColorChooserBackground(ByVal context As SynthContext, ByVal g As java.awt.Graphics, ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer)
				For Each painter As SynthPainter In painters
					painter.paintColorChooserBackground(context, g, x, y, w, h)
				Next painter
			End Sub

			Public Overridable Sub paintColorChooserBorder(ByVal context As SynthContext, ByVal g As java.awt.Graphics, ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer)
				For Each painter As SynthPainter In painters
					painter.paintColorChooserBorder(context, g, x, y, w, h)
				Next painter
			End Sub

			Public Overridable Sub paintComboBoxBackground(ByVal context As SynthContext, ByVal g As java.awt.Graphics, ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer)
				For Each painter As SynthPainter In painters
					painter.paintComboBoxBackground(context, g, x, y, w, h)
				Next painter
			End Sub

			Public Overridable Sub paintComboBoxBorder(ByVal context As SynthContext, ByVal g As java.awt.Graphics, ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer)
				For Each painter As SynthPainter In painters
					painter.paintComboBoxBorder(context, g, x, y, w, h)
				Next painter
			End Sub

			Public Overridable Sub paintDesktopIconBackground(ByVal context As SynthContext, ByVal g As java.awt.Graphics, ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer)
				For Each painter As SynthPainter In painters
					painter.paintDesktopIconBackground(context, g, x, y, w, h)
				Next painter
			End Sub

			Public Overridable Sub paintDesktopIconBorder(ByVal context As SynthContext, ByVal g As java.awt.Graphics, ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer)
				For Each painter As SynthPainter In painters
					painter.paintDesktopIconBorder(context, g, x, y, w, h)
				Next painter
			End Sub

			Public Overridable Sub paintDesktopPaneBackground(ByVal context As SynthContext, ByVal g As java.awt.Graphics, ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer)
				For Each painter As SynthPainter In painters
					painter.paintDesktopPaneBackground(context, g, x, y, w, h)
				Next painter
			End Sub

			Public Overridable Sub paintDesktopPaneBorder(ByVal context As SynthContext, ByVal g As java.awt.Graphics, ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer)
				For Each painter As SynthPainter In painters
					painter.paintDesktopPaneBorder(context, g, x, y, w, h)
				Next painter
			End Sub

			Public Overridable Sub paintEditorPaneBackground(ByVal context As SynthContext, ByVal g As java.awt.Graphics, ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer)
				For Each painter As SynthPainter In painters
					painter.paintEditorPaneBackground(context, g, x, y, w, h)
				Next painter
			End Sub

			Public Overridable Sub paintEditorPaneBorder(ByVal context As SynthContext, ByVal g As java.awt.Graphics, ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer)
				For Each painter As SynthPainter In painters
					painter.paintEditorPaneBorder(context, g, x, y, w, h)
				Next painter
			End Sub

			Public Overridable Sub paintFileChooserBackground(ByVal context As SynthContext, ByVal g As java.awt.Graphics, ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer)
				For Each painter As SynthPainter In painters
					painter.paintFileChooserBackground(context, g, x, y, w, h)
				Next painter
			End Sub

			Public Overridable Sub paintFileChooserBorder(ByVal context As SynthContext, ByVal g As java.awt.Graphics, ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer)
				For Each painter As SynthPainter In painters
					painter.paintFileChooserBorder(context, g, x, y, w, h)
				Next painter
			End Sub

			Public Overridable Sub paintFormattedTextFieldBackground(ByVal context As SynthContext, ByVal g As java.awt.Graphics, ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer)
				For Each painter As SynthPainter In painters
					painter.paintFormattedTextFieldBackground(context, g, x, y, w, h)
				Next painter
			End Sub

			Public Overridable Sub paintFormattedTextFieldBorder(ByVal context As SynthContext, ByVal g As java.awt.Graphics, ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer)
				For Each painter As SynthPainter In painters
					painter.paintFormattedTextFieldBorder(context, g, x, y, w, h)
				Next painter
			End Sub

			Public Overridable Sub paintInternalFrameTitlePaneBackground(ByVal context As SynthContext, ByVal g As java.awt.Graphics, ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer)
				For Each painter As SynthPainter In painters
					painter.paintInternalFrameTitlePaneBackground(context, g, x, y, w, h)
				Next painter
			End Sub

			Public Overridable Sub paintInternalFrameTitlePaneBorder(ByVal context As SynthContext, ByVal g As java.awt.Graphics, ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer)
				For Each painter As SynthPainter In painters
					painter.paintInternalFrameTitlePaneBorder(context, g, x, y, w, h)
				Next painter
			End Sub

			Public Overridable Sub paintInternalFrameBackground(ByVal context As SynthContext, ByVal g As java.awt.Graphics, ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer)
				For Each painter As SynthPainter In painters
					painter.paintInternalFrameBackground(context, g, x, y, w, h)
				Next painter
			End Sub

			Public Overridable Sub paintInternalFrameBorder(ByVal context As SynthContext, ByVal g As java.awt.Graphics, ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer)
				For Each painter As SynthPainter In painters
					painter.paintInternalFrameBorder(context, g, x, y, w, h)
				Next painter
			End Sub

			Public Overridable Sub paintLabelBackground(ByVal context As SynthContext, ByVal g As java.awt.Graphics, ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer)
				For Each painter As SynthPainter In painters
					painter.paintLabelBackground(context, g, x, y, w, h)
				Next painter
			End Sub

			Public Overridable Sub paintLabelBorder(ByVal context As SynthContext, ByVal g As java.awt.Graphics, ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer)
				For Each painter As SynthPainter In painters
					painter.paintLabelBorder(context, g, x, y, w, h)
				Next painter
			End Sub

			Public Overridable Sub paintListBackground(ByVal context As SynthContext, ByVal g As java.awt.Graphics, ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer)
				For Each painter As SynthPainter In painters
					painter.paintListBackground(context, g, x, y, w, h)
				Next painter
			End Sub

			Public Overridable Sub paintListBorder(ByVal context As SynthContext, ByVal g As java.awt.Graphics, ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer)
				For Each painter As SynthPainter In painters
					painter.paintListBorder(context, g, x, y, w, h)
				Next painter
			End Sub

			Public Overridable Sub paintMenuBarBackground(ByVal context As SynthContext, ByVal g As java.awt.Graphics, ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer)
				For Each painter As SynthPainter In painters
					painter.paintMenuBarBackground(context, g, x, y, w, h)
				Next painter
			End Sub

			Public Overridable Sub paintMenuBarBorder(ByVal context As SynthContext, ByVal g As java.awt.Graphics, ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer)
				For Each painter As SynthPainter In painters
					painter.paintMenuBarBorder(context, g, x, y, w, h)
				Next painter
			End Sub

			Public Overridable Sub paintMenuItemBackground(ByVal context As SynthContext, ByVal g As java.awt.Graphics, ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer)
				For Each painter As SynthPainter In painters
					painter.paintMenuItemBackground(context, g, x, y, w, h)
				Next painter
			End Sub

			Public Overridable Sub paintMenuItemBorder(ByVal context As SynthContext, ByVal g As java.awt.Graphics, ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer)
				For Each painter As SynthPainter In painters
					painter.paintMenuItemBorder(context, g, x, y, w, h)
				Next painter
			End Sub

			Public Overridable Sub paintMenuBackground(ByVal context As SynthContext, ByVal g As java.awt.Graphics, ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer)
				For Each painter As SynthPainter In painters
					painter.paintMenuBackground(context, g, x, y, w, h)
				Next painter
			End Sub

			Public Overridable Sub paintMenuBorder(ByVal context As SynthContext, ByVal g As java.awt.Graphics, ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer)
				For Each painter As SynthPainter In painters
					painter.paintMenuBorder(context, g, x, y, w, h)
				Next painter
			End Sub

			Public Overridable Sub paintOptionPaneBackground(ByVal context As SynthContext, ByVal g As java.awt.Graphics, ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer)
				For Each painter As SynthPainter In painters
					painter.paintOptionPaneBackground(context, g, x, y, w, h)
				Next painter
			End Sub

			Public Overridable Sub paintOptionPaneBorder(ByVal context As SynthContext, ByVal g As java.awt.Graphics, ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer)
				For Each painter As SynthPainter In painters
					painter.paintOptionPaneBorder(context, g, x, y, w, h)
				Next painter
			End Sub

			Public Overridable Sub paintPanelBackground(ByVal context As SynthContext, ByVal g As java.awt.Graphics, ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer)
				For Each painter As SynthPainter In painters
					painter.paintPanelBackground(context, g, x, y, w, h)
				Next painter
			End Sub

			Public Overridable Sub paintPanelBorder(ByVal context As SynthContext, ByVal g As java.awt.Graphics, ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer)
				For Each painter As SynthPainter In painters
					painter.paintPanelBorder(context, g, x, y, w, h)
				Next painter
			End Sub

			Public Overridable Sub paintPasswordFieldBackground(ByVal context As SynthContext, ByVal g As java.awt.Graphics, ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer)
				For Each painter As SynthPainter In painters
					painter.paintPasswordFieldBackground(context, g, x, y, w, h)
				Next painter
			End Sub

			Public Overridable Sub paintPasswordFieldBorder(ByVal context As SynthContext, ByVal g As java.awt.Graphics, ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer)
				For Each painter As SynthPainter In painters
					painter.paintPasswordFieldBorder(context, g, x, y, w, h)
				Next painter
			End Sub

			Public Overridable Sub paintPopupMenuBackground(ByVal context As SynthContext, ByVal g As java.awt.Graphics, ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer)
				For Each painter As SynthPainter In painters
					painter.paintPopupMenuBackground(context, g, x, y, w, h)
				Next painter
			End Sub

			Public Overridable Sub paintPopupMenuBorder(ByVal context As SynthContext, ByVal g As java.awt.Graphics, ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer)
				For Each painter As SynthPainter In painters
					painter.paintPopupMenuBorder(context, g, x, y, w, h)
				Next painter
			End Sub

			Public Overridable Sub paintProgressBarBackground(ByVal context As SynthContext, ByVal g As java.awt.Graphics, ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer)
				For Each painter As SynthPainter In painters
					painter.paintProgressBarBackground(context, g, x, y, w, h)
				Next painter
			End Sub

			Public Overridable Sub paintProgressBarBackground(ByVal context As SynthContext, ByVal g As java.awt.Graphics, ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer, ByVal orientation As Integer)
				For Each painter As SynthPainter In painters
					painter.paintProgressBarBackground(context, g, x, y, w, h, orientation)
				Next painter
			End Sub

			Public Overridable Sub paintProgressBarBorder(ByVal context As SynthContext, ByVal g As java.awt.Graphics, ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer)
				For Each painter As SynthPainter In painters
					painter.paintProgressBarBorder(context, g, x, y, w, h)
				Next painter
			End Sub

			Public Overridable Sub paintProgressBarBorder(ByVal context As SynthContext, ByVal g As java.awt.Graphics, ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer, ByVal orientation As Integer)
				For Each painter As SynthPainter In painters
					painter.paintProgressBarBorder(context, g, x, y, w, h, orientation)
				Next painter
			End Sub

			Public Overridable Sub paintProgressBarForeground(ByVal context As SynthContext, ByVal g As java.awt.Graphics, ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer, ByVal orientation As Integer)
				For Each painter As SynthPainter In painters
					painter.paintProgressBarForeground(context, g, x, y, w, h, orientation)
				Next painter
			End Sub

			Public Overridable Sub paintRadioButtonMenuItemBackground(ByVal context As SynthContext, ByVal g As java.awt.Graphics, ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer)
				For Each painter As SynthPainter In painters
					painter.paintRadioButtonMenuItemBackground(context, g, x, y, w, h)
				Next painter
			End Sub

			Public Overridable Sub paintRadioButtonMenuItemBorder(ByVal context As SynthContext, ByVal g As java.awt.Graphics, ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer)
				For Each painter As SynthPainter In painters
					painter.paintRadioButtonMenuItemBorder(context, g, x, y, w, h)
				Next painter
			End Sub

			Public Overridable Sub paintRadioButtonBackground(ByVal context As SynthContext, ByVal g As java.awt.Graphics, ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer)
				For Each painter As SynthPainter In painters
					painter.paintRadioButtonBackground(context, g, x, y, w, h)
				Next painter
			End Sub

			Public Overridable Sub paintRadioButtonBorder(ByVal context As SynthContext, ByVal g As java.awt.Graphics, ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer)
				For Each painter As SynthPainter In painters
					painter.paintRadioButtonBorder(context, g, x, y, w, h)
				Next painter
			End Sub

			Public Overridable Sub paintRootPaneBackground(ByVal context As SynthContext, ByVal g As java.awt.Graphics, ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer)
				For Each painter As SynthPainter In painters
					painter.paintRootPaneBackground(context, g, x, y, w, h)
				Next painter
			End Sub

			Public Overridable Sub paintRootPaneBorder(ByVal context As SynthContext, ByVal g As java.awt.Graphics, ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer)
				For Each painter As SynthPainter In painters
					painter.paintRootPaneBorder(context, g, x, y, w, h)
				Next painter
			End Sub

			Public Overridable Sub paintScrollBarBackground(ByVal context As SynthContext, ByVal g As java.awt.Graphics, ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer)
				For Each painter As SynthPainter In painters
					painter.paintScrollBarBackground(context, g, x, y, w, h)
				Next painter
			End Sub

			Public Overridable Sub paintScrollBarBackground(ByVal context As SynthContext, ByVal g As java.awt.Graphics, ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer, ByVal orientation As Integer)
				For Each painter As SynthPainter In painters
					painter.paintScrollBarBackground(context, g, x, y, w, h, orientation)
				Next painter
			End Sub

			Public Overridable Sub paintScrollBarBorder(ByVal context As SynthContext, ByVal g As java.awt.Graphics, ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer)
				For Each painter As SynthPainter In painters
					painter.paintScrollBarBorder(context, g, x, y, w, h)
				Next painter
			End Sub

			Public Overridable Sub paintScrollBarBorder(ByVal context As SynthContext, ByVal g As java.awt.Graphics, ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer, ByVal orientation As Integer)
				For Each painter As SynthPainter In painters
					painter.paintScrollBarBorder(context, g, x, y, w, h, orientation)
				Next painter
			End Sub

			Public Overridable Sub paintScrollBarThumbBackground(ByVal context As SynthContext, ByVal g As java.awt.Graphics, ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer, ByVal orientation As Integer)
				For Each painter As SynthPainter In painters
					painter.paintScrollBarThumbBackground(context, g, x, y, w, h, orientation)
				Next painter
			End Sub

			Public Overridable Sub paintScrollBarThumbBorder(ByVal context As SynthContext, ByVal g As java.awt.Graphics, ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer, ByVal orientation As Integer)
				For Each painter As SynthPainter In painters
					painter.paintScrollBarThumbBorder(context, g, x, y, w, h, orientation)
				Next painter
			End Sub

			Public Overridable Sub paintScrollBarTrackBackground(ByVal context As SynthContext, ByVal g As java.awt.Graphics, ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer)
				For Each painter As SynthPainter In painters
					painter.paintScrollBarTrackBackground(context, g, x, y, w, h)
				Next painter
			End Sub

			Public Overridable Sub paintScrollBarTrackBackground(ByVal context As SynthContext, ByVal g As java.awt.Graphics, ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer, ByVal orientation As Integer)
				For Each painter As SynthPainter In painters
					painter.paintScrollBarTrackBackground(context, g, x, y, w, h, orientation)
				Next painter
			End Sub

			Public Overridable Sub paintScrollBarTrackBorder(ByVal context As SynthContext, ByVal g As java.awt.Graphics, ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer)
				For Each painter As SynthPainter In painters
					painter.paintScrollBarTrackBorder(context, g, x, y, w, h)
				Next painter
			End Sub

			Public Overridable Sub paintScrollBarTrackBorder(ByVal context As SynthContext, ByVal g As java.awt.Graphics, ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer, ByVal orientation As Integer)
				For Each painter As SynthPainter In painters
					painter.paintScrollBarTrackBorder(context, g, x, y, w, h, orientation)
				Next painter
			End Sub

			Public Overridable Sub paintScrollPaneBackground(ByVal context As SynthContext, ByVal g As java.awt.Graphics, ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer)
				For Each painter As SynthPainter In painters
					painter.paintScrollPaneBackground(context, g, x, y, w, h)
				Next painter
			End Sub

			Public Overridable Sub paintScrollPaneBorder(ByVal context As SynthContext, ByVal g As java.awt.Graphics, ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer)
				For Each painter As SynthPainter In painters
					painter.paintScrollPaneBorder(context, g, x, y, w, h)
				Next painter
			End Sub

			Public Overridable Sub paintSeparatorBackground(ByVal context As SynthContext, ByVal g As java.awt.Graphics, ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer)
				For Each painter As SynthPainter In painters
					painter.paintSeparatorBackground(context, g, x, y, w, h)
				Next painter
			End Sub

			Public Overridable Sub paintSeparatorBackground(ByVal context As SynthContext, ByVal g As java.awt.Graphics, ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer, ByVal orientation As Integer)
				For Each painter As SynthPainter In painters
					painter.paintSeparatorBackground(context, g, x, y, w, h, orientation)
				Next painter
			End Sub

			Public Overridable Sub paintSeparatorBorder(ByVal context As SynthContext, ByVal g As java.awt.Graphics, ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer)
				For Each painter As SynthPainter In painters
					painter.paintSeparatorBorder(context, g, x, y, w, h)
				Next painter
			End Sub

			Public Overridable Sub paintSeparatorBorder(ByVal context As SynthContext, ByVal g As java.awt.Graphics, ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer, ByVal orientation As Integer)
				For Each painter As SynthPainter In painters
					painter.paintSeparatorBorder(context, g, x, y, w, h, orientation)
				Next painter
			End Sub

			Public Overridable Sub paintSeparatorForeground(ByVal context As SynthContext, ByVal g As java.awt.Graphics, ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer, ByVal orientation As Integer)
				For Each painter As SynthPainter In painters
					painter.paintSeparatorForeground(context, g, x, y, w, h, orientation)
				Next painter
			End Sub

			Public Overridable Sub paintSliderBackground(ByVal context As SynthContext, ByVal g As java.awt.Graphics, ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer)
				For Each painter As SynthPainter In painters
					painter.paintSliderBackground(context, g, x, y, w, h)
				Next painter
			End Sub

			Public Overridable Sub paintSliderBackground(ByVal context As SynthContext, ByVal g As java.awt.Graphics, ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer, ByVal orientation As Integer)
				For Each painter As SynthPainter In painters
					painter.paintSliderBackground(context, g, x, y, w, h, orientation)
				Next painter
			End Sub

			Public Overridable Sub paintSliderBorder(ByVal context As SynthContext, ByVal g As java.awt.Graphics, ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer)
				For Each painter As SynthPainter In painters
					painter.paintSliderBorder(context, g, x, y, w, h)
				Next painter
			End Sub

			Public Overridable Sub paintSliderBorder(ByVal context As SynthContext, ByVal g As java.awt.Graphics, ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer, ByVal orientation As Integer)
				For Each painter As SynthPainter In painters
					painter.paintSliderBorder(context, g, x, y, w, h, orientation)
				Next painter
			End Sub

			Public Overridable Sub paintSliderThumbBackground(ByVal context As SynthContext, ByVal g As java.awt.Graphics, ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer, ByVal orientation As Integer)
				For Each painter As SynthPainter In painters
					painter.paintSliderThumbBackground(context, g, x, y, w, h, orientation)
				Next painter
			End Sub

			Public Overridable Sub paintSliderThumbBorder(ByVal context As SynthContext, ByVal g As java.awt.Graphics, ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer, ByVal orientation As Integer)
				For Each painter As SynthPainter In painters
					painter.paintSliderThumbBorder(context, g, x, y, w, h, orientation)
				Next painter
			End Sub

			Public Overridable Sub paintSliderTrackBackground(ByVal context As SynthContext, ByVal g As java.awt.Graphics, ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer)
				For Each painter As SynthPainter In painters
					painter.paintSliderTrackBackground(context, g, x, y, w, h)
				Next painter
			End Sub

			Public Overridable Sub paintSliderTrackBackground(ByVal context As SynthContext, ByVal g As java.awt.Graphics, ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer, ByVal orientation As Integer)
				For Each painter As SynthPainter In painters
					painter.paintSliderTrackBackground(context, g, x, y, w, h, orientation)
				Next painter
			End Sub

			Public Overridable Sub paintSliderTrackBorder(ByVal context As SynthContext, ByVal g As java.awt.Graphics, ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer)
				For Each painter As SynthPainter In painters
					painter.paintSliderTrackBorder(context, g, x, y, w, h)
				Next painter
			End Sub

			Public Overridable Sub paintSliderTrackBorder(ByVal context As SynthContext, ByVal g As java.awt.Graphics, ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer, ByVal orientation As Integer)
				For Each painter As SynthPainter In painters
					painter.paintSliderTrackBorder(context, g, x, y, w, h, orientation)
				Next painter
			End Sub

			Public Overridable Sub paintSpinnerBackground(ByVal context As SynthContext, ByVal g As java.awt.Graphics, ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer)
				For Each painter As SynthPainter In painters
					painter.paintSpinnerBackground(context, g, x, y, w, h)
				Next painter
			End Sub

			Public Overridable Sub paintSpinnerBorder(ByVal context As SynthContext, ByVal g As java.awt.Graphics, ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer)
				For Each painter As SynthPainter In painters
					painter.paintSpinnerBorder(context, g, x, y, w, h)
				Next painter
			End Sub

			Public Overridable Sub paintSplitPaneDividerBackground(ByVal context As SynthContext, ByVal g As java.awt.Graphics, ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer)
				For Each painter As SynthPainter In painters
					painter.paintSplitPaneDividerBackground(context, g, x, y, w, h)
				Next painter
			End Sub

			Public Overridable Sub paintSplitPaneDividerBackground(ByVal context As SynthContext, ByVal g As java.awt.Graphics, ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer, ByVal orientation As Integer)
				For Each painter As SynthPainter In painters
					painter.paintSplitPaneDividerBackground(context, g, x, y, w, h, orientation)
				Next painter
			End Sub

			Public Overridable Sub paintSplitPaneDividerForeground(ByVal context As SynthContext, ByVal g As java.awt.Graphics, ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer, ByVal orientation As Integer)
				For Each painter As SynthPainter In painters
					painter.paintSplitPaneDividerForeground(context, g, x, y, w, h, orientation)
				Next painter
			End Sub

			Public Overridable Sub paintSplitPaneDragDivider(ByVal context As SynthContext, ByVal g As java.awt.Graphics, ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer, ByVal orientation As Integer)
				For Each painter As SynthPainter In painters
					painter.paintSplitPaneDragDivider(context, g, x, y, w, h, orientation)
				Next painter
			End Sub

			Public Overridable Sub paintSplitPaneBackground(ByVal context As SynthContext, ByVal g As java.awt.Graphics, ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer)
				For Each painter As SynthPainter In painters
					painter.paintSplitPaneBackground(context, g, x, y, w, h)
				Next painter
			End Sub

			Public Overridable Sub paintSplitPaneBorder(ByVal context As SynthContext, ByVal g As java.awt.Graphics, ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer)
				For Each painter As SynthPainter In painters
					painter.paintSplitPaneBorder(context, g, x, y, w, h)
				Next painter
			End Sub

			Public Overridable Sub paintTabbedPaneBackground(ByVal context As SynthContext, ByVal g As java.awt.Graphics, ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer)
				For Each painter As SynthPainter In painters
					painter.paintTabbedPaneBackground(context, g, x, y, w, h)
				Next painter
			End Sub

			Public Overridable Sub paintTabbedPaneBorder(ByVal context As SynthContext, ByVal g As java.awt.Graphics, ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer)
				For Each painter As SynthPainter In painters
					painter.paintTabbedPaneBorder(context, g, x, y, w, h)
				Next painter
			End Sub

			Public Overridable Sub paintTabbedPaneTabAreaBackground(ByVal context As SynthContext, ByVal g As java.awt.Graphics, ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer)
				For Each painter As SynthPainter In painters
					painter.paintTabbedPaneTabAreaBackground(context, g, x, y, w, h)
				Next painter
			End Sub

			Public Overridable Sub paintTabbedPaneTabAreaBackground(ByVal context As SynthContext, ByVal g As java.awt.Graphics, ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer, ByVal orientation As Integer)
				For Each painter As SynthPainter In painters
					painter.paintTabbedPaneTabAreaBackground(context, g, x, y, w, h, orientation)
				Next painter
			End Sub

			Public Overridable Sub paintTabbedPaneTabAreaBorder(ByVal context As SynthContext, ByVal g As java.awt.Graphics, ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer)
				For Each painter As SynthPainter In painters
					painter.paintTabbedPaneTabAreaBorder(context, g, x, y, w, h)
				Next painter
			End Sub

			Public Overridable Sub paintTabbedPaneTabAreaBorder(ByVal context As SynthContext, ByVal g As java.awt.Graphics, ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer, ByVal orientation As Integer)
				For Each painter As SynthPainter In painters
					painter.paintTabbedPaneTabAreaBorder(context, g, x, y, w, h, orientation)
				Next painter
			End Sub

			Public Overridable Sub paintTabbedPaneTabBackground(ByVal context As SynthContext, ByVal g As java.awt.Graphics, ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer, ByVal tabIndex As Integer)
				For Each painter As SynthPainter In painters
					painter.paintTabbedPaneTabBackground(context, g, x, y, w, h, tabIndex)
				Next painter
			End Sub

			Public Overridable Sub paintTabbedPaneTabBackground(ByVal context As SynthContext, ByVal g As java.awt.Graphics, ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer, ByVal tabIndex As Integer, ByVal orientation As Integer)
				For Each painter As SynthPainter In painters
					painter.paintTabbedPaneTabBackground(context, g, x, y, w, h, tabIndex, orientation)
				Next painter
			End Sub

			Public Overridable Sub paintTabbedPaneTabBorder(ByVal context As SynthContext, ByVal g As java.awt.Graphics, ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer, ByVal tabIndex As Integer)
				For Each painter As SynthPainter In painters
					painter.paintTabbedPaneTabBorder(context, g, x, y, w, h, tabIndex)
				Next painter
			End Sub

			Public Overridable Sub paintTabbedPaneTabBorder(ByVal context As SynthContext, ByVal g As java.awt.Graphics, ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer, ByVal tabIndex As Integer, ByVal orientation As Integer)
				For Each painter As SynthPainter In painters
					painter.paintTabbedPaneTabBorder(context, g, x, y, w, h, tabIndex, orientation)
				Next painter
			End Sub

			Public Overridable Sub paintTabbedPaneContentBackground(ByVal context As SynthContext, ByVal g As java.awt.Graphics, ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer)
				For Each painter As SynthPainter In painters
					painter.paintTabbedPaneContentBackground(context, g, x, y, w, h)
				Next painter
			End Sub

			Public Overridable Sub paintTabbedPaneContentBorder(ByVal context As SynthContext, ByVal g As java.awt.Graphics, ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer)
				For Each painter As SynthPainter In painters
					painter.paintTabbedPaneContentBorder(context, g, x, y, w, h)
				Next painter
			End Sub

			Public Overridable Sub paintTableHeaderBackground(ByVal context As SynthContext, ByVal g As java.awt.Graphics, ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer)
				For Each painter As SynthPainter In painters
					painter.paintTableHeaderBackground(context, g, x, y, w, h)
				Next painter
			End Sub

			Public Overridable Sub paintTableHeaderBorder(ByVal context As SynthContext, ByVal g As java.awt.Graphics, ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer)
				For Each painter As SynthPainter In painters
					painter.paintTableHeaderBorder(context, g, x, y, w, h)
				Next painter
			End Sub

			Public Overridable Sub paintTableBackground(ByVal context As SynthContext, ByVal g As java.awt.Graphics, ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer)
				For Each painter As SynthPainter In painters
					painter.paintTableBackground(context, g, x, y, w, h)
				Next painter
			End Sub

			Public Overridable Sub paintTableBorder(ByVal context As SynthContext, ByVal g As java.awt.Graphics, ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer)
				For Each painter As SynthPainter In painters
					painter.paintTableBorder(context, g, x, y, w, h)
				Next painter
			End Sub

			Public Overridable Sub paintTextAreaBackground(ByVal context As SynthContext, ByVal g As java.awt.Graphics, ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer)
				For Each painter As SynthPainter In painters
					painter.paintTextAreaBackground(context, g, x, y, w, h)
				Next painter
			End Sub

			Public Overridable Sub paintTextAreaBorder(ByVal context As SynthContext, ByVal g As java.awt.Graphics, ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer)
				For Each painter As SynthPainter In painters
					painter.paintTextAreaBorder(context, g, x, y, w, h)
				Next painter
			End Sub

			Public Overridable Sub paintTextPaneBackground(ByVal context As SynthContext, ByVal g As java.awt.Graphics, ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer)
				For Each painter As SynthPainter In painters
					painter.paintTextPaneBackground(context, g, x, y, w, h)
				Next painter
			End Sub

			Public Overridable Sub paintTextPaneBorder(ByVal context As SynthContext, ByVal g As java.awt.Graphics, ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer)
				For Each painter As SynthPainter In painters
					painter.paintTextPaneBorder(context, g, x, y, w, h)
				Next painter
			End Sub

			Public Overridable Sub paintTextFieldBackground(ByVal context As SynthContext, ByVal g As java.awt.Graphics, ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer)
				For Each painter As SynthPainter In painters
					painter.paintTextFieldBackground(context, g, x, y, w, h)
				Next painter
			End Sub

			Public Overridable Sub paintTextFieldBorder(ByVal context As SynthContext, ByVal g As java.awt.Graphics, ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer)
				For Each painter As SynthPainter In painters
					painter.paintTextFieldBorder(context, g, x, y, w, h)
				Next painter
			End Sub

			Public Overridable Sub paintToggleButtonBackground(ByVal context As SynthContext, ByVal g As java.awt.Graphics, ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer)
				For Each painter As SynthPainter In painters
					painter.paintToggleButtonBackground(context, g, x, y, w, h)
				Next painter
			End Sub

			Public Overridable Sub paintToggleButtonBorder(ByVal context As SynthContext, ByVal g As java.awt.Graphics, ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer)
				For Each painter As SynthPainter In painters
					painter.paintToggleButtonBorder(context, g, x, y, w, h)
				Next painter
			End Sub

			Public Overridable Sub paintToolBarBackground(ByVal context As SynthContext, ByVal g As java.awt.Graphics, ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer)
				For Each painter As SynthPainter In painters
					painter.paintToolBarBackground(context, g, x, y, w, h)
				Next painter
			End Sub

			Public Overridable Sub paintToolBarBackground(ByVal context As SynthContext, ByVal g As java.awt.Graphics, ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer, ByVal orientation As Integer)
				For Each painter As SynthPainter In painters
					painter.paintToolBarBackground(context, g, x, y, w, h, orientation)
				Next painter
			End Sub

			Public Overridable Sub paintToolBarBorder(ByVal context As SynthContext, ByVal g As java.awt.Graphics, ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer)
				For Each painter As SynthPainter In painters
					painter.paintToolBarBorder(context, g, x, y, w, h)
				Next painter
			End Sub

			Public Overridable Sub paintToolBarBorder(ByVal context As SynthContext, ByVal g As java.awt.Graphics, ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer, ByVal orientation As Integer)
				For Each painter As SynthPainter In painters
					painter.paintToolBarBorder(context, g, x, y, w, h, orientation)
				Next painter
			End Sub

			Public Overridable Sub paintToolBarContentBackground(ByVal context As SynthContext, ByVal g As java.awt.Graphics, ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer)
				For Each painter As SynthPainter In painters
					painter.paintToolBarContentBackground(context, g, x, y, w, h)
				Next painter
			End Sub

			Public Overridable Sub paintToolBarContentBackground(ByVal context As SynthContext, ByVal g As java.awt.Graphics, ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer, ByVal orientation As Integer)
				For Each painter As SynthPainter In painters
					painter.paintToolBarContentBackground(context, g, x, y, w, h, orientation)
				Next painter
			End Sub

			Public Overridable Sub paintToolBarContentBorder(ByVal context As SynthContext, ByVal g As java.awt.Graphics, ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer)
				For Each painter As SynthPainter In painters
					painter.paintToolBarContentBorder(context, g, x, y, w, h)
				Next painter
			End Sub

			Public Overridable Sub paintToolBarContentBorder(ByVal context As SynthContext, ByVal g As java.awt.Graphics, ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer, ByVal orientation As Integer)
				For Each painter As SynthPainter In painters
					painter.paintToolBarContentBorder(context, g, x, y, w, h, orientation)
				Next painter
			End Sub

			Public Overridable Sub paintToolBarDragWindowBackground(ByVal context As SynthContext, ByVal g As java.awt.Graphics, ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer)
				For Each painter As SynthPainter In painters
					painter.paintToolBarDragWindowBackground(context, g, x, y, w, h)
				Next painter
			End Sub

			Public Overridable Sub paintToolBarDragWindowBackground(ByVal context As SynthContext, ByVal g As java.awt.Graphics, ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer, ByVal orientation As Integer)
				For Each painter As SynthPainter In painters
					painter.paintToolBarDragWindowBackground(context, g, x, y, w, h, orientation)
				Next painter
			End Sub

			Public Overridable Sub paintToolBarDragWindowBorder(ByVal context As SynthContext, ByVal g As java.awt.Graphics, ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer)
				For Each painter As SynthPainter In painters
					painter.paintToolBarDragWindowBorder(context, g, x, y, w, h)
				Next painter
			End Sub

			Public Overridable Sub paintToolBarDragWindowBorder(ByVal context As SynthContext, ByVal g As java.awt.Graphics, ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer, ByVal orientation As Integer)
				For Each painter As SynthPainter In painters
					painter.paintToolBarDragWindowBorder(context, g, x, y, w, h, orientation)
				Next painter
			End Sub

			Public Overridable Sub paintToolTipBackground(ByVal context As SynthContext, ByVal g As java.awt.Graphics, ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer)
				For Each painter As SynthPainter In painters
					painter.paintToolTipBackground(context, g, x, y, w, h)
				Next painter
			End Sub

			Public Overridable Sub paintToolTipBorder(ByVal context As SynthContext, ByVal g As java.awt.Graphics, ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer)
				For Each painter As SynthPainter In painters
					painter.paintToolTipBorder(context, g, x, y, w, h)
				Next painter
			End Sub

			Public Overridable Sub paintTreeBackground(ByVal context As SynthContext, ByVal g As java.awt.Graphics, ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer)
				For Each painter As SynthPainter In painters
					painter.paintTreeBackground(context, g, x, y, w, h)
				Next painter
			End Sub

			Public Overridable Sub paintTreeBorder(ByVal context As SynthContext, ByVal g As java.awt.Graphics, ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer)
				For Each painter As SynthPainter In painters
					painter.paintTreeBorder(context, g, x, y, w, h)
				Next painter
			End Sub

			Public Overridable Sub paintTreeCellBackground(ByVal context As SynthContext, ByVal g As java.awt.Graphics, ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer)
				For Each painter As SynthPainter In painters
					painter.paintTreeCellBackground(context, g, x, y, w, h)
				Next painter
			End Sub

			Public Overridable Sub paintTreeCellBorder(ByVal context As SynthContext, ByVal g As java.awt.Graphics, ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer)
				For Each painter As SynthPainter In painters
					painter.paintTreeCellBorder(context, g, x, y, w, h)
				Next painter
			End Sub

			Public Overridable Sub paintTreeCellFocus(ByVal context As SynthContext, ByVal g As java.awt.Graphics, ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer)
				For Each painter As SynthPainter In painters
					painter.paintTreeCellFocus(context, g, x, y, w, h)
				Next painter
			End Sub

			Public Overridable Sub paintViewportBackground(ByVal context As SynthContext, ByVal g As java.awt.Graphics, ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer)
				For Each painter As SynthPainter In painters
					painter.paintViewportBackground(context, g, x, y, w, h)
				Next painter
			End Sub

			Public Overridable Sub paintViewportBorder(ByVal context As SynthContext, ByVal g As java.awt.Graphics, ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer)
				For Each painter As SynthPainter In painters
					painter.paintViewportBorder(context, g, x, y, w, h)
				Next painter
			End Sub
		End Class

		Private Class DelegatingPainter
			Inherits SynthPainter

			Private Shared Function getPainter(ByVal context As SynthContext, ByVal method As String, ByVal direction As Integer) As SynthPainter
				Return CType(context.style, ParsedSynthStyle).getBestPainter(context, method, direction)
			End Function

			Public Overridable Sub paintArrowButtonBackground(ByVal context As SynthContext, ByVal g As java.awt.Graphics, ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer)
				getPainter(context, "arrowbuttonbackground", -1).paintArrowButtonBackground(context, g, x, y, w, h)
			End Sub

			Public Overridable Sub paintArrowButtonBorder(ByVal context As SynthContext, ByVal g As java.awt.Graphics, ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer)
				getPainter(context, "arrowbuttonborder", -1).paintArrowButtonBorder(context, g, x, y, w, h)
			End Sub

			Public Overridable Sub paintArrowButtonForeground(ByVal context As SynthContext, ByVal g As java.awt.Graphics, ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer, ByVal direction As Integer)
				getPainter(context, "arrowbuttonforeground", direction).paintArrowButtonForeground(context, g, x, y, w, h, direction)
			End Sub

			Public Overridable Sub paintButtonBackground(ByVal context As SynthContext, ByVal g As java.awt.Graphics, ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer)
				getPainter(context, "buttonbackground", -1).paintButtonBackground(context, g, x, y, w, h)
			End Sub

			Public Overridable Sub paintButtonBorder(ByVal context As SynthContext, ByVal g As java.awt.Graphics, ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer)
				getPainter(context, "buttonborder", -1).paintButtonBorder(context, g, x, y, w, h)
			End Sub

			Public Overridable Sub paintCheckBoxMenuItemBackground(ByVal context As SynthContext, ByVal g As java.awt.Graphics, ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer)
				getPainter(context, "checkboxmenuitembackground", -1).paintCheckBoxMenuItemBackground(context, g, x, y, w, h)
			End Sub

			Public Overridable Sub paintCheckBoxMenuItemBorder(ByVal context As SynthContext, ByVal g As java.awt.Graphics, ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer)
				getPainter(context, "checkboxmenuitemborder", -1).paintCheckBoxMenuItemBorder(context, g, x, y, w, h)
			End Sub

			Public Overridable Sub paintCheckBoxBackground(ByVal context As SynthContext, ByVal g As java.awt.Graphics, ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer)
				getPainter(context, "checkboxbackground", -1).paintCheckBoxBackground(context, g, x, y, w, h)
			End Sub

			Public Overridable Sub paintCheckBoxBorder(ByVal context As SynthContext, ByVal g As java.awt.Graphics, ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer)
				getPainter(context, "checkboxborder", -1).paintCheckBoxBorder(context, g, x, y, w, h)
			End Sub

			Public Overridable Sub paintColorChooserBackground(ByVal context As SynthContext, ByVal g As java.awt.Graphics, ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer)
				getPainter(context, "colorchooserbackground", -1).paintColorChooserBackground(context, g, x, y, w, h)
			End Sub

			Public Overridable Sub paintColorChooserBorder(ByVal context As SynthContext, ByVal g As java.awt.Graphics, ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer)
				getPainter(context, "colorchooserborder", -1).paintColorChooserBorder(context, g, x, y, w, h)
			End Sub

			Public Overridable Sub paintComboBoxBackground(ByVal context As SynthContext, ByVal g As java.awt.Graphics, ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer)
				getPainter(context, "comboboxbackground", -1).paintComboBoxBackground(context, g, x, y, w, h)
			End Sub

			Public Overridable Sub paintComboBoxBorder(ByVal context As SynthContext, ByVal g As java.awt.Graphics, ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer)
				getPainter(context, "comboboxborder", -1).paintComboBoxBorder(context, g, x, y, w, h)
			End Sub

			Public Overridable Sub paintDesktopIconBackground(ByVal context As SynthContext, ByVal g As java.awt.Graphics, ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer)
				getPainter(context, "desktopiconbackground", -1).paintDesktopIconBackground(context, g, x, y, w, h)
			End Sub

			Public Overridable Sub paintDesktopIconBorder(ByVal context As SynthContext, ByVal g As java.awt.Graphics, ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer)
				getPainter(context, "desktopiconborder", -1).paintDesktopIconBorder(context, g, x, y, w, h)
			End Sub

			Public Overridable Sub paintDesktopPaneBackground(ByVal context As SynthContext, ByVal g As java.awt.Graphics, ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer)
				getPainter(context, "desktoppanebackground", -1).paintDesktopPaneBackground(context, g, x, y, w, h)
			End Sub

			Public Overridable Sub paintDesktopPaneBorder(ByVal context As SynthContext, ByVal g As java.awt.Graphics, ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer)
				getPainter(context, "desktoppaneborder", -1).paintDesktopPaneBorder(context, g, x, y, w, h)
			End Sub

			Public Overridable Sub paintEditorPaneBackground(ByVal context As SynthContext, ByVal g As java.awt.Graphics, ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer)
				getPainter(context, "editorpanebackground", -1).paintEditorPaneBackground(context, g, x, y, w, h)
			End Sub

			Public Overridable Sub paintEditorPaneBorder(ByVal context As SynthContext, ByVal g As java.awt.Graphics, ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer)
				getPainter(context, "editorpaneborder", -1).paintEditorPaneBorder(context, g, x, y, w, h)
			End Sub

			Public Overridable Sub paintFileChooserBackground(ByVal context As SynthContext, ByVal g As java.awt.Graphics, ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer)
				getPainter(context, "filechooserbackground", -1).paintFileChooserBackground(context, g, x, y, w, h)
			End Sub

			Public Overridable Sub paintFileChooserBorder(ByVal context As SynthContext, ByVal g As java.awt.Graphics, ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer)
				getPainter(context, "filechooserborder", -1).paintFileChooserBorder(context, g, x, y, w, h)
			End Sub

			Public Overridable Sub paintFormattedTextFieldBackground(ByVal context As SynthContext, ByVal g As java.awt.Graphics, ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer)
				getPainter(context, "formattedtextfieldbackground", -1).paintFormattedTextFieldBackground(context, g, x, y, w, h)
			End Sub

			Public Overridable Sub paintFormattedTextFieldBorder(ByVal context As SynthContext, ByVal g As java.awt.Graphics, ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer)
				getPainter(context, "formattedtextfieldborder", -1).paintFormattedTextFieldBorder(context, g, x, y, w, h)
			End Sub

			Public Overridable Sub paintInternalFrameTitlePaneBackground(ByVal context As SynthContext, ByVal g As java.awt.Graphics, ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer)
				getPainter(context, "internalframetitlepanebackground", -1).paintInternalFrameTitlePaneBackground(context, g, x, y, w, h)
			End Sub

			Public Overridable Sub paintInternalFrameTitlePaneBorder(ByVal context As SynthContext, ByVal g As java.awt.Graphics, ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer)
				getPainter(context, "internalframetitlepaneborder", -1).paintInternalFrameTitlePaneBorder(context, g, x, y, w, h)
			End Sub

			Public Overridable Sub paintInternalFrameBackground(ByVal context As SynthContext, ByVal g As java.awt.Graphics, ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer)
				getPainter(context, "internalframebackground", -1).paintInternalFrameBackground(context, g, x, y, w, h)
			End Sub

			Public Overridable Sub paintInternalFrameBorder(ByVal context As SynthContext, ByVal g As java.awt.Graphics, ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer)
				getPainter(context, "internalframeborder", -1).paintInternalFrameBorder(context, g, x, y, w, h)
			End Sub

			Public Overridable Sub paintLabelBackground(ByVal context As SynthContext, ByVal g As java.awt.Graphics, ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer)
				getPainter(context, "labelbackground", -1).paintLabelBackground(context, g, x, y, w, h)
			End Sub

			Public Overridable Sub paintLabelBorder(ByVal context As SynthContext, ByVal g As java.awt.Graphics, ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer)
				getPainter(context, "labelborder", -1).paintLabelBorder(context, g, x, y, w, h)
			End Sub

			Public Overridable Sub paintListBackground(ByVal context As SynthContext, ByVal g As java.awt.Graphics, ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer)
				getPainter(context, "listbackground", -1).paintListBackground(context, g, x, y, w, h)
			End Sub

			Public Overridable Sub paintListBorder(ByVal context As SynthContext, ByVal g As java.awt.Graphics, ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer)
				getPainter(context, "listborder", -1).paintListBorder(context, g, x, y, w, h)
			End Sub

			Public Overridable Sub paintMenuBarBackground(ByVal context As SynthContext, ByVal g As java.awt.Graphics, ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer)
				getPainter(context, "menubarbackground", -1).paintMenuBarBackground(context, g, x, y, w, h)
			End Sub

			Public Overridable Sub paintMenuBarBorder(ByVal context As SynthContext, ByVal g As java.awt.Graphics, ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer)
				getPainter(context, "menubarborder", -1).paintMenuBarBorder(context, g, x, y, w, h)
			End Sub

			Public Overridable Sub paintMenuItemBackground(ByVal context As SynthContext, ByVal g As java.awt.Graphics, ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer)
				getPainter(context, "menuitembackground", -1).paintMenuItemBackground(context, g, x, y, w, h)
			End Sub

			Public Overridable Sub paintMenuItemBorder(ByVal context As SynthContext, ByVal g As java.awt.Graphics, ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer)
				getPainter(context, "menuitemborder", -1).paintMenuItemBorder(context, g, x, y, w, h)
			End Sub

			Public Overridable Sub paintMenuBackground(ByVal context As SynthContext, ByVal g As java.awt.Graphics, ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer)
				getPainter(context, "menubackground", -1).paintMenuBackground(context, g, x, y, w, h)
			End Sub

			Public Overridable Sub paintMenuBorder(ByVal context As SynthContext, ByVal g As java.awt.Graphics, ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer)
				getPainter(context, "menuborder", -1).paintMenuBorder(context, g, x, y, w, h)
			End Sub

			Public Overridable Sub paintOptionPaneBackground(ByVal context As SynthContext, ByVal g As java.awt.Graphics, ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer)
				getPainter(context, "optionpanebackground", -1).paintOptionPaneBackground(context, g, x, y, w, h)
			End Sub

			Public Overridable Sub paintOptionPaneBorder(ByVal context As SynthContext, ByVal g As java.awt.Graphics, ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer)
				getPainter(context, "optionpaneborder", -1).paintOptionPaneBorder(context, g, x, y, w, h)
			End Sub

			Public Overridable Sub paintPanelBackground(ByVal context As SynthContext, ByVal g As java.awt.Graphics, ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer)
				getPainter(context, "panelbackground", -1).paintPanelBackground(context, g, x, y, w, h)
			End Sub

			Public Overridable Sub paintPanelBorder(ByVal context As SynthContext, ByVal g As java.awt.Graphics, ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer)
				getPainter(context, "panelborder", -1).paintPanelBorder(context, g, x, y, w, h)
			End Sub

			Public Overridable Sub paintPasswordFieldBackground(ByVal context As SynthContext, ByVal g As java.awt.Graphics, ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer)
				getPainter(context, "passwordfieldbackground", -1).paintPasswordFieldBackground(context, g, x, y, w, h)
			End Sub

			Public Overridable Sub paintPasswordFieldBorder(ByVal context As SynthContext, ByVal g As java.awt.Graphics, ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer)
				getPainter(context, "passwordfieldborder", -1).paintPasswordFieldBorder(context, g, x, y, w, h)
			End Sub

			Public Overridable Sub paintPopupMenuBackground(ByVal context As SynthContext, ByVal g As java.awt.Graphics, ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer)
				getPainter(context, "popupmenubackground", -1).paintPopupMenuBackground(context, g, x, y, w, h)
			End Sub

			Public Overridable Sub paintPopupMenuBorder(ByVal context As SynthContext, ByVal g As java.awt.Graphics, ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer)
				getPainter(context, "popupmenuborder", -1).paintPopupMenuBorder(context, g, x, y, w, h)
			End Sub

			Public Overridable Sub paintProgressBarBackground(ByVal context As SynthContext, ByVal g As java.awt.Graphics, ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer)
				getPainter(context, "progressbarbackground", -1).paintProgressBarBackground(context, g, x, y, w, h)
			End Sub

			Public Overridable Sub paintProgressBarBackground(ByVal context As SynthContext, ByVal g As java.awt.Graphics, ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer, ByVal direction As Integer)
				getPainter(context, "progressbarbackground", direction).paintProgressBarBackground(context, g, x, y, w, h, direction)
			End Sub

			Public Overridable Sub paintProgressBarBorder(ByVal context As SynthContext, ByVal g As java.awt.Graphics, ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer)
				getPainter(context, "progressbarborder", -1).paintProgressBarBorder(context, g, x, y, w, h)
			End Sub

			Public Overridable Sub paintProgressBarBorder(ByVal context As SynthContext, ByVal g As java.awt.Graphics, ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer, ByVal direction As Integer)
				getPainter(context, "progressbarborder", direction).paintProgressBarBorder(context, g, x, y, w, h, direction)
			End Sub

			Public Overridable Sub paintProgressBarForeground(ByVal context As SynthContext, ByVal g As java.awt.Graphics, ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer, ByVal direction As Integer)
				getPainter(context, "progressbarforeground", direction).paintProgressBarForeground(context, g, x, y, w, h, direction)
			End Sub

			Public Overridable Sub paintRadioButtonMenuItemBackground(ByVal context As SynthContext, ByVal g As java.awt.Graphics, ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer)
				getPainter(context, "radiobuttonmenuitembackground", -1).paintRadioButtonMenuItemBackground(context, g, x, y, w, h)
			End Sub

			Public Overridable Sub paintRadioButtonMenuItemBorder(ByVal context As SynthContext, ByVal g As java.awt.Graphics, ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer)
				getPainter(context, "radiobuttonmenuitemborder", -1).paintRadioButtonMenuItemBorder(context, g, x, y, w, h)
			End Sub

			Public Overridable Sub paintRadioButtonBackground(ByVal context As SynthContext, ByVal g As java.awt.Graphics, ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer)
				getPainter(context, "radiobuttonbackground", -1).paintRadioButtonBackground(context, g, x, y, w, h)
			End Sub

			Public Overridable Sub paintRadioButtonBorder(ByVal context As SynthContext, ByVal g As java.awt.Graphics, ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer)
				getPainter(context, "radiobuttonborder", -1).paintRadioButtonBorder(context, g, x, y, w, h)
			End Sub

			Public Overridable Sub paintRootPaneBackground(ByVal context As SynthContext, ByVal g As java.awt.Graphics, ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer)
				getPainter(context, "rootpanebackground", -1).paintRootPaneBackground(context, g, x, y, w, h)
			End Sub

			Public Overridable Sub paintRootPaneBorder(ByVal context As SynthContext, ByVal g As java.awt.Graphics, ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer)
				getPainter(context, "rootpaneborder", -1).paintRootPaneBorder(context, g, x, y, w, h)
			End Sub

			Public Overridable Sub paintScrollBarBackground(ByVal context As SynthContext, ByVal g As java.awt.Graphics, ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer)
				getPainter(context, "scrollbarbackground", -1).paintScrollBarBackground(context, g, x, y, w, h)
			End Sub

			Public Overridable Sub paintScrollBarBackground(ByVal context As SynthContext, ByVal g As java.awt.Graphics, ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer, ByVal direction As Integer)
				getPainter(context, "scrollbarbackground", direction).paintScrollBarBackground(context, g, x, y, w, h, direction)
			End Sub


			Public Overridable Sub paintScrollBarBorder(ByVal context As SynthContext, ByVal g As java.awt.Graphics, ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer)
				getPainter(context, "scrollbarborder", -1).paintScrollBarBorder(context, g, x, y, w, h)
			End Sub

			Public Overridable Sub paintScrollBarBorder(ByVal context As SynthContext, ByVal g As java.awt.Graphics, ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer, ByVal orientation As Integer)
				getPainter(context, "scrollbarborder", orientation).paintScrollBarBorder(context, g, x, y, w, h, orientation)
			End Sub

			Public Overridable Sub paintScrollBarThumbBackground(ByVal context As SynthContext, ByVal g As java.awt.Graphics, ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer, ByVal direction As Integer)
				getPainter(context, "scrollbarthumbbackground", direction).paintScrollBarThumbBackground(context, g, x, y, w, h, direction)
			End Sub

			Public Overridable Sub paintScrollBarThumbBorder(ByVal context As SynthContext, ByVal g As java.awt.Graphics, ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer, ByVal direction As Integer)
				getPainter(context, "scrollbarthumbborder", direction).paintScrollBarThumbBorder(context, g, x, y, w, h, direction)
			End Sub

			Public Overridable Sub paintScrollBarTrackBackground(ByVal context As SynthContext, ByVal g As java.awt.Graphics, ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer)
				getPainter(context, "scrollbartrackbackground", -1).paintScrollBarTrackBackground(context, g, x, y, w, h)
			End Sub

			Public Overridable Sub paintScrollBarTrackBackground(ByVal context As SynthContext, ByVal g As java.awt.Graphics, ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer, ByVal direction As Integer)
				 getPainter(context, "scrollbartrackbackground", direction).paintScrollBarTrackBackground(context, g, x, y, w, h, direction)
			End Sub

			Public Overridable Sub paintScrollBarTrackBorder(ByVal context As SynthContext, ByVal g As java.awt.Graphics, ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer)
				getPainter(context, "scrollbartrackborder", -1).paintScrollBarTrackBorder(context, g, x, y, w, h)
			End Sub

			Public Overridable Sub paintScrollBarTrackBorder(ByVal context As SynthContext, ByVal g As java.awt.Graphics, ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer, ByVal orientation As Integer)
				getPainter(context, "scrollbartrackborder", orientation).paintScrollBarTrackBorder(context, g, x, y, w, h, orientation)
			End Sub

			Public Overridable Sub paintScrollPaneBackground(ByVal context As SynthContext, ByVal g As java.awt.Graphics, ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer)
				getPainter(context, "scrollpanebackground", -1).paintScrollPaneBackground(context, g, x, y, w, h)
			End Sub

			Public Overridable Sub paintScrollPaneBorder(ByVal context As SynthContext, ByVal g As java.awt.Graphics, ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer)
				getPainter(context, "scrollpaneborder", -1).paintScrollPaneBorder(context, g, x, y, w, h)
			End Sub

			Public Overridable Sub paintSeparatorBackground(ByVal context As SynthContext, ByVal g As java.awt.Graphics, ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer)
				getPainter(context, "separatorbackground", -1).paintSeparatorBackground(context, g, x, y, w, h)
			End Sub

			Public Overridable Sub paintSeparatorBackground(ByVal context As SynthContext, ByVal g As java.awt.Graphics, ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer, ByVal orientation As Integer)
				getPainter(context, "separatorbackground", orientation).paintSeparatorBackground(context, g, x, y, w, h, orientation)
			End Sub

			Public Overridable Sub paintSeparatorBorder(ByVal context As SynthContext, ByVal g As java.awt.Graphics, ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer)
				getPainter(context, "separatorborder", -1).paintSeparatorBorder(context, g, x, y, w, h)
			End Sub

			Public Overridable Sub paintSeparatorBorder(ByVal context As SynthContext, ByVal g As java.awt.Graphics, ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer, ByVal orientation As Integer)
				getPainter(context, "separatorborder", orientation).paintSeparatorBorder(context, g, x, y, w, h, orientation)
			End Sub

			Public Overridable Sub paintSeparatorForeground(ByVal context As SynthContext, ByVal g As java.awt.Graphics, ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer, ByVal direction As Integer)
				getPainter(context, "separatorforeground", direction).paintSeparatorForeground(context, g, x, y, w, h, direction)
			End Sub

			Public Overridable Sub paintSliderBackground(ByVal context As SynthContext, ByVal g As java.awt.Graphics, ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer)
				getPainter(context, "sliderbackground", -1).paintSliderBackground(context, g, x, y, w, h)
			End Sub

			Public Overridable Sub paintSliderBackground(ByVal context As SynthContext, ByVal g As java.awt.Graphics, ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer, ByVal direction As Integer)
				getPainter(context, "sliderbackground", direction).paintSliderBackground(context, g, x, y, w, h, direction)
			End Sub

			Public Overridable Sub paintSliderBorder(ByVal context As SynthContext, ByVal g As java.awt.Graphics, ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer)
				getPainter(context, "sliderborder", -1).paintSliderBorder(context, g, x, y, w, h)
			End Sub

			Public Overridable Sub paintSliderBorder(ByVal context As SynthContext, ByVal g As java.awt.Graphics, ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer, ByVal direction As Integer)
				getPainter(context, "sliderborder", direction).paintSliderBorder(context, g, x, y, w, h, direction)
			End Sub

			Public Overridable Sub paintSliderThumbBackground(ByVal context As SynthContext, ByVal g As java.awt.Graphics, ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer, ByVal direction As Integer)
				getPainter(context, "sliderthumbbackground", direction).paintSliderThumbBackground(context, g, x, y, w, h, direction)
			End Sub

			Public Overridable Sub paintSliderThumbBorder(ByVal context As SynthContext, ByVal g As java.awt.Graphics, ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer, ByVal direction As Integer)
				getPainter(context, "sliderthumbborder", direction).paintSliderThumbBorder(context, g, x, y, w, h, direction)
			End Sub

			Public Overridable Sub paintSliderTrackBackground(ByVal context As SynthContext, ByVal g As java.awt.Graphics, ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer)
				getPainter(context, "slidertrackbackground", -1).paintSliderTrackBackground(context, g, x, y, w, h)
			End Sub

			Public Overridable Sub paintSliderTrackBackground(ByVal context As SynthContext, ByVal g As java.awt.Graphics, ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer, ByVal direction As Integer)
				getPainter(context, "slidertrackbackground", direction).paintSliderTrackBackground(context, g, x, y, w, h, direction)
			End Sub

			Public Overridable Sub paintSliderTrackBorder(ByVal context As SynthContext, ByVal g As java.awt.Graphics, ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer)
				getPainter(context, "slidertrackborder", -1).paintSliderTrackBorder(context, g, x, y, w, h)
			End Sub

			Public Overridable Sub paintSliderTrackBorder(ByVal context As SynthContext, ByVal g As java.awt.Graphics, ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer, ByVal direction As Integer)
				getPainter(context, "slidertrackborder", direction).paintSliderTrackBorder(context, g, x, y, w, h, direction)
			End Sub

			Public Overridable Sub paintSpinnerBackground(ByVal context As SynthContext, ByVal g As java.awt.Graphics, ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer)
				getPainter(context, "spinnerbackground", -1).paintSpinnerBackground(context, g, x, y, w, h)
			End Sub

			Public Overridable Sub paintSpinnerBorder(ByVal context As SynthContext, ByVal g As java.awt.Graphics, ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer)
				getPainter(context, "spinnerborder", -1).paintSpinnerBorder(context, g, x, y, w, h)
			End Sub

			Public Overridable Sub paintSplitPaneDividerBackground(ByVal context As SynthContext, ByVal g As java.awt.Graphics, ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer)
				getPainter(context, "splitpanedividerbackground", -1).paintSplitPaneDividerBackground(context, g, x, y, w, h)
			End Sub

			Public Overridable Sub paintSplitPaneDividerBackground(ByVal context As SynthContext, ByVal g As java.awt.Graphics, ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer, ByVal orientation As Integer)
				getPainter(context, "splitpanedividerbackground", orientation).paintSplitPaneDividerBackground(context, g, x, y, w, h, orientation)
			End Sub

			Public Overridable Sub paintSplitPaneDividerForeground(ByVal context As SynthContext, ByVal g As java.awt.Graphics, ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer, ByVal direction As Integer)
				getPainter(context, "splitpanedividerforeground", direction).paintSplitPaneDividerForeground(context, g, x, y, w, h, direction)
			End Sub

			Public Overridable Sub paintSplitPaneDragDivider(ByVal context As SynthContext, ByVal g As java.awt.Graphics, ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer, ByVal direction As Integer)
				getPainter(context, "splitpanedragdivider", direction).paintSplitPaneDragDivider(context, g, x, y, w, h, direction)
			End Sub

			Public Overridable Sub paintSplitPaneBackground(ByVal context As SynthContext, ByVal g As java.awt.Graphics, ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer)
				getPainter(context, "splitpanebackground", -1).paintSplitPaneBackground(context, g, x, y, w, h)
			End Sub

			Public Overridable Sub paintSplitPaneBorder(ByVal context As SynthContext, ByVal g As java.awt.Graphics, ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer)
				getPainter(context, "splitpaneborder", -1).paintSplitPaneBorder(context, g, x, y, w, h)
			End Sub

			Public Overridable Sub paintTabbedPaneBackground(ByVal context As SynthContext, ByVal g As java.awt.Graphics, ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer)
				getPainter(context, "tabbedpanebackground", -1).paintTabbedPaneBackground(context, g, x, y, w, h)
			End Sub

			Public Overridable Sub paintTabbedPaneBorder(ByVal context As SynthContext, ByVal g As java.awt.Graphics, ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer)
				getPainter(context, "tabbedpaneborder", -1).paintTabbedPaneBorder(context, g, x, y, w, h)
			End Sub

			Public Overridable Sub paintTabbedPaneTabAreaBackground(ByVal context As SynthContext, ByVal g As java.awt.Graphics, ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer)
				getPainter(context, "tabbedpanetabareabackground", -1).paintTabbedPaneTabAreaBackground(context, g, x, y, w, h)
			End Sub

			Public Overridable Sub paintTabbedPaneTabAreaBackground(ByVal context As SynthContext, ByVal g As java.awt.Graphics, ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer, ByVal orientation As Integer)
				getPainter(context, "tabbedpanetabareabackground", orientation).paintTabbedPaneTabAreaBackground(context, g, x, y, w, h, orientation)
			End Sub

			Public Overridable Sub paintTabbedPaneTabAreaBorder(ByVal context As SynthContext, ByVal g As java.awt.Graphics, ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer)
				getPainter(context, "tabbedpanetabareaborder", -1).paintTabbedPaneTabAreaBorder(context, g, x, y, w, h)
			End Sub

			Public Overridable Sub paintTabbedPaneTabAreaBorder(ByVal context As SynthContext, ByVal g As java.awt.Graphics, ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer, ByVal orientation As Integer)
				getPainter(context, "tabbedpanetabareaborder", orientation).paintTabbedPaneTabAreaBorder(context, g, x, y, w, h, orientation)
			End Sub

			Public Overridable Sub paintTabbedPaneTabBackground(ByVal context As SynthContext, ByVal g As java.awt.Graphics, ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer, ByVal direction As Integer)
				getPainter(context, "tabbedpanetabbackground", -1).paintTabbedPaneTabBackground(context, g, x, y, w, h, direction)
			End Sub

			Public Overridable Sub paintTabbedPaneTabBackground(ByVal context As SynthContext, ByVal g As java.awt.Graphics, ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer, ByVal tabIndex As Integer, ByVal direction As Integer)
				getPainter(context, "tabbedpanetabbackground", direction).paintTabbedPaneTabBackground(context, g, x, y, w, h, tabIndex, direction)
			End Sub

			Public Overridable Sub paintTabbedPaneTabBorder(ByVal context As SynthContext, ByVal g As java.awt.Graphics, ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer, ByVal direction As Integer)
				getPainter(context, "tabbedpanetabborder", -1).paintTabbedPaneTabBorder(context, g, x, y, w, h, direction)
			End Sub

			Public Overridable Sub paintTabbedPaneTabBorder(ByVal context As SynthContext, ByVal g As java.awt.Graphics, ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer, ByVal tabIndex As Integer, ByVal direction As Integer)
				getPainter(context, "tabbedpanetabborder", direction).paintTabbedPaneTabBorder(context, g, x, y, w, h, tabIndex, direction)
			End Sub

			Public Overridable Sub paintTabbedPaneContentBackground(ByVal context As SynthContext, ByVal g As java.awt.Graphics, ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer)
				getPainter(context, "tabbedpanecontentbackground", -1).paintTabbedPaneContentBackground(context, g, x, y, w, h)
			End Sub

			Public Overridable Sub paintTabbedPaneContentBorder(ByVal context As SynthContext, ByVal g As java.awt.Graphics, ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer)
				getPainter(context, "tabbedpanecontentborder", -1).paintTabbedPaneContentBorder(context, g, x, y, w, h)
			End Sub

			Public Overridable Sub paintTableHeaderBackground(ByVal context As SynthContext, ByVal g As java.awt.Graphics, ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer)
				getPainter(context, "tableheaderbackground", -1).paintTableHeaderBackground(context, g, x, y, w, h)
			End Sub

			Public Overridable Sub paintTableHeaderBorder(ByVal context As SynthContext, ByVal g As java.awt.Graphics, ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer)
				getPainter(context, "tableheaderborder", -1).paintTableHeaderBorder(context, g, x, y, w, h)
			End Sub

			Public Overridable Sub paintTableBackground(ByVal context As SynthContext, ByVal g As java.awt.Graphics, ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer)
				getPainter(context, "tablebackground", -1).paintTableBackground(context, g, x, y, w, h)
			End Sub

			Public Overridable Sub paintTableBorder(ByVal context As SynthContext, ByVal g As java.awt.Graphics, ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer)
				getPainter(context, "tableborder", -1).paintTableBorder(context, g, x, y, w, h)
			End Sub

			Public Overridable Sub paintTextAreaBackground(ByVal context As SynthContext, ByVal g As java.awt.Graphics, ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer)
				getPainter(context, "textareabackground", -1).paintTextAreaBackground(context, g, x, y, w, h)
			End Sub

			Public Overridable Sub paintTextAreaBorder(ByVal context As SynthContext, ByVal g As java.awt.Graphics, ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer)
				getPainter(context, "textareaborder", -1).paintTextAreaBorder(context, g, x, y, w, h)
			End Sub

			Public Overridable Sub paintTextPaneBackground(ByVal context As SynthContext, ByVal g As java.awt.Graphics, ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer)
				getPainter(context, "textpanebackground", -1).paintTextPaneBackground(context, g, x, y, w, h)
			End Sub

			Public Overridable Sub paintTextPaneBorder(ByVal context As SynthContext, ByVal g As java.awt.Graphics, ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer)
				getPainter(context, "textpaneborder", -1).paintTextPaneBorder(context, g, x, y, w, h)
			End Sub

			Public Overridable Sub paintTextFieldBackground(ByVal context As SynthContext, ByVal g As java.awt.Graphics, ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer)
				getPainter(context, "textfieldbackground", -1).paintTextFieldBackground(context, g, x, y, w, h)
			End Sub

			Public Overridable Sub paintTextFieldBorder(ByVal context As SynthContext, ByVal g As java.awt.Graphics, ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer)
				getPainter(context, "textfieldborder", -1).paintTextFieldBorder(context, g, x, y, w, h)
			End Sub

			Public Overridable Sub paintToggleButtonBackground(ByVal context As SynthContext, ByVal g As java.awt.Graphics, ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer)
				getPainter(context, "togglebuttonbackground", -1).paintToggleButtonBackground(context, g, x, y, w, h)
			End Sub

			Public Overridable Sub paintToggleButtonBorder(ByVal context As SynthContext, ByVal g As java.awt.Graphics, ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer)
				getPainter(context, "togglebuttonborder", -1).paintToggleButtonBorder(context, g, x, y, w, h)
			End Sub

			Public Overridable Sub paintToolBarBackground(ByVal context As SynthContext, ByVal g As java.awt.Graphics, ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer)
				getPainter(context, "toolbarbackground", -1).paintToolBarBackground(context, g, x, y, w, h)
			End Sub

			Public Overridable Sub paintToolBarBackground(ByVal context As SynthContext, ByVal g As java.awt.Graphics, ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer, ByVal orientation As Integer)
				getPainter(context, "toolbarbackground", orientation).paintToolBarBackground(context, g, x, y, w, h, orientation)
			End Sub

			Public Overridable Sub paintToolBarBorder(ByVal context As SynthContext, ByVal g As java.awt.Graphics, ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer)
				getPainter(context, "toolbarborder", -1).paintToolBarBorder(context, g, x, y, w, h)
			End Sub

			Public Overridable Sub paintToolBarBorder(ByVal context As SynthContext, ByVal g As java.awt.Graphics, ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer, ByVal orientation As Integer)
				getPainter(context, "toolbarborder", orientation).paintToolBarBorder(context, g, x, y, w, h, orientation)
			End Sub

			Public Overridable Sub paintToolBarContentBackground(ByVal context As SynthContext, ByVal g As java.awt.Graphics, ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer)
				getPainter(context, "toolbarcontentbackground", -1).paintToolBarContentBackground(context, g, x, y, w, h)
			End Sub

			Public Overridable Sub paintToolBarContentBackground(ByVal context As SynthContext, ByVal g As java.awt.Graphics, ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer, ByVal orientation As Integer)
				getPainter(context, "toolbarcontentbackground", orientation).paintToolBarContentBackground(context, g, x, y, w, h, orientation)
			End Sub

			Public Overridable Sub paintToolBarContentBorder(ByVal context As SynthContext, ByVal g As java.awt.Graphics, ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer)
				getPainter(context, "toolbarcontentborder", -1).paintToolBarContentBorder(context, g, x, y, w, h)
			End Sub

			Public Overridable Sub paintToolBarContentBorder(ByVal context As SynthContext, ByVal g As java.awt.Graphics, ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer, ByVal orientation As Integer)
				getPainter(context, "toolbarcontentborder", orientation).paintToolBarContentBorder(context, g, x, y, w, h, orientation)
			End Sub

			Public Overridable Sub paintToolBarDragWindowBackground(ByVal context As SynthContext, ByVal g As java.awt.Graphics, ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer)
				getPainter(context, "toolbardragwindowbackground", -1).paintToolBarDragWindowBackground(context, g, x, y, w, h)
			End Sub

			Public Overridable Sub paintToolBarDragWindowBackground(ByVal context As SynthContext, ByVal g As java.awt.Graphics, ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer, ByVal orientation As Integer)
				getPainter(context, "toolbardragwindowbackground", orientation).paintToolBarDragWindowBackground(context, g, x, y, w, h, orientation)
			End Sub

			Public Overridable Sub paintToolBarDragWindowBorder(ByVal context As SynthContext, ByVal g As java.awt.Graphics, ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer)
				getPainter(context, "toolbardragwindowborder", -1).paintToolBarDragWindowBorder(context, g, x, y, w, h)
			End Sub

			Public Overridable Sub paintToolBarDragWindowBorder(ByVal context As SynthContext, ByVal g As java.awt.Graphics, ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer, ByVal orientation As Integer)
				getPainter(context, "toolbardragwindowborder", orientation).paintToolBarDragWindowBorder(context, g, x, y, w, h, orientation)
			End Sub

			Public Overridable Sub paintToolTipBackground(ByVal context As SynthContext, ByVal g As java.awt.Graphics, ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer)
				getPainter(context, "tooltipbackground", -1).paintToolTipBackground(context, g, x, y, w, h)
			End Sub

			Public Overridable Sub paintToolTipBorder(ByVal context As SynthContext, ByVal g As java.awt.Graphics, ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer)
				getPainter(context, "tooltipborder", -1).paintToolTipBorder(context, g, x, y, w, h)
			End Sub

			Public Overridable Sub paintTreeBackground(ByVal context As SynthContext, ByVal g As java.awt.Graphics, ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer)
				getPainter(context, "treebackground", -1).paintTreeBackground(context, g, x, y, w, h)
			End Sub

			Public Overridable Sub paintTreeBorder(ByVal context As SynthContext, ByVal g As java.awt.Graphics, ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer)
				getPainter(context, "treeborder", -1).paintTreeBorder(context, g, x, y, w, h)
			End Sub

			Public Overridable Sub paintTreeCellBackground(ByVal context As SynthContext, ByVal g As java.awt.Graphics, ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer)
				getPainter(context, "treecellbackground", -1).paintTreeCellBackground(context, g, x, y, w, h)
			End Sub

			Public Overridable Sub paintTreeCellBorder(ByVal context As SynthContext, ByVal g As java.awt.Graphics, ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer)
				getPainter(context, "treecellborder", -1).paintTreeCellBorder(context, g, x, y, w, h)
			End Sub

			Public Overridable Sub paintTreeCellFocus(ByVal context As SynthContext, ByVal g As java.awt.Graphics, ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer)
				getPainter(context, "treecellfocus", -1).paintTreeCellFocus(context, g, x, y, w, h)
			End Sub

			Public Overridable Sub paintViewportBackground(ByVal context As SynthContext, ByVal g As java.awt.Graphics, ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer)
				getPainter(context, "viewportbackground", -1).paintViewportBackground(context, g, x, y, w, h)
			End Sub

			Public Overridable Sub paintViewportBorder(ByVal context As SynthContext, ByVal g As java.awt.Graphics, ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer)
				getPainter(context, "viewportborder", -1).paintViewportBorder(context, g, x, y, w, h)
			End Sub
		End Class
	End Class

End Namespace