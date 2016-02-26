Imports System.Diagnostics
Imports javax.swing
Imports javax.swing.border

'
' * Copyright (c) 2002, 2006, Oracle and/or its affiliates. All rights reserved.
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
	''' SynthBorder is a border that delegates to a Painter. The Insets
	''' are determined at construction time.
	''' 
	''' @author Scott Violet
	''' </summary>
	Friend Class SynthBorder
		Inherits AbstractBorder
		Implements javax.swing.plaf.UIResource

		Private ui As SynthUI
		Private insets As Insets

		Friend Sub New(ByVal ui As SynthUI, ByVal insets As Insets)
			Me.ui = ui
			Me.insets = insets
		End Sub

		Friend Sub New(ByVal ui As SynthUI)
			Me.New(ui, Nothing)
		End Sub

		Public Overridable Sub paintBorder(ByVal c As Component, ByVal g As Graphics, ByVal x As Integer, ByVal y As Integer, ByVal width As Integer, ByVal height As Integer)
			Dim jc As JComponent = CType(c, JComponent)
			Dim context As SynthContext = ui.getContext(jc)
			Dim style As SynthStyle = context.style
			If style Is Nothing Then
				Debug.Assert(False, "SynthBorder is being used outside after the UI " & "has been uninstalled")
				Return
			End If
			ui.paintBorder(context, g, x, y, width, height)
			context.Dispose()
		End Sub

		''' <summary>
		''' Reinitializes the insets parameter with this Border's current Insets. </summary>
		''' <param name="c"> the component for which this border insets value applies </param>
		''' <param name="insets"> the object to be reinitialized </param>
		''' <returns> the <code>insets</code> object </returns>
		Public Overridable Function getBorderInsets(ByVal c As Component, ByVal insets As Insets) As Insets
			If Me.insets IsNot Nothing Then
				If insets Is Nothing Then
					insets = New Insets(Me.insets.top, Me.insets.left, Me.insets.bottom, Me.insets.right)
				Else
					insets.top = Me.insets.top
					insets.bottom = Me.insets.bottom
					insets.left = Me.insets.left
					insets.right = Me.insets.right
				End If
			ElseIf insets Is Nothing Then
				insets = New Insets(0, 0, 0, 0)
			Else
					insets.right = 0
						insets.left = insets.right
							insets.bottom = insets.left
							insets.top = insets.bottom
			End If
			If TypeOf c Is JComponent Then
				Dim ___region As Region = Region.getRegion(CType(c, JComponent))
				Dim margin As Insets = Nothing
				If (___region Is Region.ARROW_BUTTON OrElse ___region Is Region.BUTTON OrElse ___region Is Region.CHECK_BOX OrElse ___region Is Region.CHECK_BOX_MENU_ITEM OrElse ___region Is Region.MENU OrElse ___region Is Region.MENU_ITEM OrElse ___region Is Region.RADIO_BUTTON OrElse ___region Is Region.RADIO_BUTTON_MENU_ITEM OrElse ___region Is Region.TOGGLE_BUTTON) AndAlso (TypeOf c Is AbstractButton) Then
					margin = CType(c, AbstractButton).margin
				ElseIf (___region Is Region.EDITOR_PANE OrElse ___region Is Region.FORMATTED_TEXT_FIELD OrElse ___region Is Region.PASSWORD_FIELD OrElse ___region Is Region.TEXT_AREA OrElse ___region Is Region.TEXT_FIELD OrElse ___region Is Region.TEXT_PANE) AndAlso (TypeOf c Is javax.swing.text.JTextComponent) Then
					margin = CType(c, javax.swing.text.JTextComponent).margin
				ElseIf ___region Is Region.TOOL_BAR AndAlso (TypeOf c Is JToolBar) Then
					margin = CType(c, JToolBar).margin
				ElseIf ___region Is Region.MENU_BAR AndAlso (TypeOf c Is JMenuBar) Then
					margin = CType(c, JMenuBar).margin
				End If
				If margin IsNot Nothing Then
					insets.top += margin.top
					insets.bottom += margin.bottom
					insets.left += margin.left
					insets.right += margin.right
				End If
			End If
			Return insets
		End Function

		''' <summary>
		''' This default implementation returns false. </summary>
		''' <returns> false </returns>
		Public Property Overrides borderOpaque As Boolean
			Get
				Return False
			End Get
		End Property
	End Class

End Namespace