Imports Microsoft.VisualBasic
Imports javax.swing
Imports javax.swing.text
Imports javax.swing.text.html
Imports javax.swing.plaf
Imports javax.swing.border

'
' * Copyright (c) 1997, 2013, Oracle and/or its affiliates. All rights reserved.
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
Namespace javax.swing.plaf.basic


	''' <summary>
	''' Provides the look and feel for a JEditorPane.
	''' <p>
	''' <strong>Warning:</strong>
	''' Serialized objects of this class will not be compatible with
	''' future Swing releases. The current serialization support is
	''' appropriate for short term storage or RMI between applications running
	''' the same version of Swing.  As of 1.4, support for long term storage
	''' of all JavaBeans&trade;
	''' has been added to the <code>java.beans</code> package.
	''' Please see <seealso cref="java.beans.XMLEncoder"/>.
	''' 
	''' @author  Timothy Prinzing
	''' </summary>
	Public Class BasicEditorPaneUI
		Inherits BasicTextUI

		''' <summary>
		''' Creates a UI for the JTextPane.
		''' </summary>
		''' <param name="c"> the JTextPane component </param>
		''' <returns> the UI </returns>
		Public Shared Function createUI(ByVal c As JComponent) As ComponentUI
			Return New BasicEditorPaneUI
		End Function

		''' <summary>
		''' Creates a new BasicEditorPaneUI.
		''' </summary>
		Public Sub New()
			MyBase.New()
		End Sub

		''' <summary>
		''' Fetches the name used as a key to lookup properties through the
		''' UIManager.  This is used as a prefix to all the standard
		''' text properties.
		''' </summary>
		''' <returns> the name ("EditorPane") </returns>
		Protected Friend Property Overrides propertyPrefix As String
			Get
				Return "EditorPane"
			End Get
		End Property

		''' <summary>
		''' {@inheritDoc}
		''' 
		''' @since 1.5
		''' </summary>
		Public Overrides Sub installUI(ByVal c As JComponent)
			MyBase.installUI(c)
			updateDisplayProperties(c.font, c.foreground)
		End Sub

		''' <summary>
		''' {@inheritDoc}
		''' 
		''' @since 1.5
		''' </summary>
		Public Overrides Sub uninstallUI(ByVal c As JComponent)
			cleanDisplayProperties()
			MyBase.uninstallUI(c)
		End Sub

		''' <summary>
		''' Fetches the EditorKit for the UI.  This is whatever is
		''' currently set in the associated JEditorPane.
		''' </summary>
		''' <returns> the editor capabilities </returns>
		''' <seealso cref= TextUI#getEditorKit </seealso>
		Public Overrides Function getEditorKit(ByVal tc As JTextComponent) As EditorKit
			Dim pane As JEditorPane = CType(component, JEditorPane)
			Return pane.editorKit
		End Function

		''' <summary>
		''' Fetch an action map to use.  The map for a JEditorPane
		''' is not shared because it changes with the EditorKit.
		''' </summary>
		Friend Property Overrides actionMap As ActionMap
			Get
				Dim am As ActionMap = New ActionMapUIResource
				am.put("requestFocus", New FocusAction)
				Dim ___editorKit As EditorKit = getEditorKit(component)
				If ___editorKit IsNot Nothing Then
					Dim actions As Action() = ___editorKit.actions
					If actions IsNot Nothing Then addActions(am, actions)
				End If
				am.put(TransferHandler.cutAction.getValue(Action.NAME), TransferHandler.cutAction)
				am.put(TransferHandler.copyAction.getValue(Action.NAME), TransferHandler.copyAction)
				am.put(TransferHandler.pasteAction.getValue(Action.NAME), TransferHandler.pasteAction)
				Return am
			End Get
		End Property

		''' <summary>
		''' This method gets called when a bound property is changed
		''' on the associated JTextComponent.  This is a hook
		''' which UI implementations may change to reflect how the
		''' UI displays bound properties of JTextComponent subclasses.
		''' This is implemented to rebuild the ActionMap based upon an
		''' EditorKit change.
		''' </summary>
		''' <param name="evt"> the property change event </param>
		Protected Friend Overrides Sub propertyChange(ByVal evt As PropertyChangeEvent)
			MyBase.propertyChange(evt)
			Dim name As String = evt.propertyName
			If "editorKit".Equals(name) Then
				Dim ___map As ActionMap = SwingUtilities.getUIActionMap(component)
				If ___map IsNot Nothing Then
					Dim oldValue As Object = evt.oldValue
					If TypeOf oldValue Is EditorKit Then
						Dim actions As Action() = CType(oldValue, EditorKit).actions
						If actions IsNot Nothing Then removeActions(___map, actions)
					End If
					Dim newValue As Object = evt.newValue
					If TypeOf newValue Is EditorKit Then
						Dim actions As Action() = CType(newValue, EditorKit).actions
						If actions IsNot Nothing Then addActions(___map, actions)
					End If
				End If
				updateFocusTraversalKeys()
			ElseIf "editable".Equals(name) Then
				updateFocusTraversalKeys()
			ElseIf "foreground".Equals(name) OrElse "font".Equals(name) OrElse "document".Equals(name) OrElse JEditorPane.W3C_LENGTH_UNITS.Equals(name) OrElse JEditorPane.HONOR_DISPLAY_PROPERTIES.Equals(name) Then
				Dim c As JComponent = component
				updateDisplayProperties(c.font, c.foreground)
				If JEditorPane.W3C_LENGTH_UNITS.Equals(name) OrElse JEditorPane.HONOR_DISPLAY_PROPERTIES.Equals(name) Then modelChanged()
				If "foreground".Equals(name) Then
					Dim honorDisplayPropertiesObject As Object = c.getClientProperty(JEditorPane.HONOR_DISPLAY_PROPERTIES)
					Dim honorDisplayProperties As Boolean = False
					If TypeOf honorDisplayPropertiesObject Is Boolean? Then honorDisplayProperties = CBool(honorDisplayPropertiesObject)
					If honorDisplayProperties Then modelChanged()
				End If


			End If
		End Sub

		Friend Overridable Sub removeActions(ByVal ___map As ActionMap, ByVal actions As Action())
			Dim n As Integer = actions.Length
			For i As Integer = 0 To n - 1
				Dim a As Action = actions(i)
				___map.remove(a.getValue(Action.NAME))
			Next i
		End Sub

		Friend Overridable Sub addActions(ByVal ___map As ActionMap, ByVal actions As Action())
			Dim n As Integer = actions.Length
			For i As Integer = 0 To n - 1
				Dim a As Action = actions(i)
				___map.put(a.getValue(Action.NAME), a)
			Next i
		End Sub

		Friend Overridable Sub updateDisplayProperties(ByVal font As Font, ByVal fg As Color)
			Dim c As JComponent = component
			Dim honorDisplayPropertiesObject As Object = c.getClientProperty(JEditorPane.HONOR_DISPLAY_PROPERTIES)
			Dim honorDisplayProperties As Boolean = False
			Dim w3cLengthUnitsObject As Object = c.getClientProperty(JEditorPane.W3C_LENGTH_UNITS)
			Dim w3cLengthUnits As Boolean = False
			If TypeOf honorDisplayPropertiesObject Is Boolean? Then honorDisplayProperties = CBool(honorDisplayPropertiesObject)
			If TypeOf w3cLengthUnitsObject Is Boolean? Then w3cLengthUnits = CBool(w3cLengthUnitsObject)
			If TypeOf Me Is BasicTextPaneUI OrElse honorDisplayProperties Then
				 'using equals because can not use UIResource for Boolean
				Dim doc As Document = component.document
				If TypeOf doc Is StyledDocument Then
					If TypeOf doc Is HTMLDocument AndAlso honorDisplayProperties Then
						updateCSS(font, fg)
					Else
						updateStyle(font, fg)
					End If
				End If
			Else
				cleanDisplayProperties()
			End If
			If w3cLengthUnits Then
				Dim doc As Document = component.document
				If TypeOf doc Is HTMLDocument Then
					Dim documentStyleSheet As StyleSheet = CType(doc, HTMLDocument).styleSheet
					documentStyleSheet.addRule("W3C_LENGTH_UNITS_ENABLE")
				End If
			Else
				Dim doc As Document = component.document
				If TypeOf doc Is HTMLDocument Then
					Dim documentStyleSheet As StyleSheet = CType(doc, HTMLDocument).styleSheet
					documentStyleSheet.addRule("W3C_LENGTH_UNITS_DISABLE")
				End If

			End If
		End Sub

		''' <summary>
		''' Attribute key to reference the default font.
		''' used in javax.swing.text.StyleContext.getFont
		''' to resolve the default font.
		''' </summary>
		Private Const FONT_ATTRIBUTE_KEY As String = "FONT_ATTRIBUTE_KEY"

		Friend Overridable Sub cleanDisplayProperties()
			Dim document As Document = component.document
			If TypeOf document Is HTMLDocument Then
				Dim documentStyleSheet As StyleSheet = CType(document, HTMLDocument).styleSheet
				Dim styleSheets As StyleSheet() = documentStyleSheet.styleSheets
				If styleSheets IsNot Nothing Then
					For Each s As StyleSheet In styleSheets
						If TypeOf s Is StyleSheetUIResource Then
							documentStyleSheet.removeStyleSheet(s)
							documentStyleSheet.addRule("BASE_SIZE_DISABLE")
							Exit For
						End If
					Next s
				End If
				Dim style As Style = CType(document, StyledDocument).getStyle(StyleContext.DEFAULT_STYLE)
				If style.getAttribute(FONT_ATTRIBUTE_KEY) IsNot Nothing Then style.removeAttribute(FONT_ATTRIBUTE_KEY)
			End If
		End Sub

		Friend Class StyleSheetUIResource
			Inherits StyleSheet
			Implements UIResource

		End Class

		Private Sub updateCSS(ByVal font As Font, ByVal fg As Color)
			Dim ___component As JTextComponent = component
			Dim document As Document = ___component.document
			If TypeOf document Is HTMLDocument Then
				Dim ___styleSheet As StyleSheet = New StyleSheetUIResource
				Dim documentStyleSheet As StyleSheet = CType(document, HTMLDocument).styleSheet
				Dim styleSheets As StyleSheet() = documentStyleSheet.styleSheets
				If styleSheets IsNot Nothing Then
					For Each s As StyleSheet In styleSheets
						If TypeOf s Is StyleSheetUIResource Then documentStyleSheet.removeStyleSheet(s)
					Next s
				End If
				Dim cssRule As String = sun.swing.SwingUtilities2.displayPropertiesToCSS(font, fg)
				___styleSheet.addRule(cssRule)
				documentStyleSheet.addStyleSheet(___styleSheet)
				documentStyleSheet.addRule("BASE_SIZE " & ___component.font.size)
				Dim style As Style = CType(document, StyledDocument).getStyle(StyleContext.DEFAULT_STYLE)
				If Not font.Equals(style.getAttribute(FONT_ATTRIBUTE_KEY)) Then style.addAttribute(FONT_ATTRIBUTE_KEY, font)
			End If
		End Sub

		Private Sub updateStyle(ByVal font As Font, ByVal fg As Color)
			updateFont(font)
			updateForeground(fg)
		End Sub

		''' <summary>
		''' Update the color in the default style of the document.
		''' </summary>
		''' <param name="color"> the new color to use or null to remove the color attribute
		'''              from the document's style </param>
		Private Sub updateForeground(ByVal color As Color)
			Dim doc As StyledDocument = CType(component.document, StyledDocument)
			Dim style As Style = doc.getStyle(StyleContext.DEFAULT_STYLE)

			If style Is Nothing Then Return

			If color Is Nothing Then
				If style.getAttribute(StyleConstants.Foreground) IsNot Nothing Then style.removeAttribute(StyleConstants.Foreground)
			Else
				If Not color.Equals(StyleConstants.getForeground(style)) Then StyleConstants.foregroundund(style, color)
			End If
		End Sub

		''' <summary>
		''' Update the font in the default style of the document.
		''' </summary>
		''' <param name="font"> the new font to use or null to remove the font attribute
		'''             from the document's style </param>
		Private Sub updateFont(ByVal font As Font)
			Dim doc As StyledDocument = CType(component.document, StyledDocument)
			Dim style As Style = doc.getStyle(StyleContext.DEFAULT_STYLE)

			If style Is Nothing Then Return

			Dim fontFamily As String = CStr(style.getAttribute(StyleConstants.FontFamily))
			Dim fontSize As Integer? = CInt(Fix(style.getAttribute(StyleConstants.FontSize)))
			Dim isBold As Boolean? = CBool(style.getAttribute(StyleConstants.Bold))
			Dim isItalic As Boolean? = CBool(style.getAttribute(StyleConstants.Italic))
			Dim fontAttribute As Font = CType(style.getAttribute(FONT_ATTRIBUTE_KEY), Font)
			If font Is Nothing Then
				If fontFamily IsNot Nothing Then style.removeAttribute(StyleConstants.FontFamily)
				If fontSize IsNot Nothing Then style.removeAttribute(StyleConstants.FontSize)
				If isBold IsNot Nothing Then style.removeAttribute(StyleConstants.Bold)
				If isItalic IsNot Nothing Then style.removeAttribute(StyleConstants.Italic)
				If fontAttribute IsNot Nothing Then style.removeAttribute(FONT_ATTRIBUTE_KEY)
			Else
				If Not font.name.Equals(fontFamily) Then StyleConstants.fontFamilyily(style, font.name)
				If fontSize Is Nothing OrElse fontSize <> font.size Then StyleConstants.fontSizeize(style, font.size)
				If isBold Is Nothing OrElse isBold <> font.bold Then StyleConstants.boldold(style, font.bold)
				If isItalic Is Nothing OrElse isItalic <> font.italic Then StyleConstants.italiclic(style, font.italic)
				If Not font.Equals(fontAttribute) Then style.addAttribute(FONT_ATTRIBUTE_KEY, font)
			End If
		End Sub
	End Class

End Namespace