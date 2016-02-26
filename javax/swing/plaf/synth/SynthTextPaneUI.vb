Imports javax.swing
Imports javax.swing.text
Imports javax.swing.plaf

'
' * Copyright (c) 2002, 2013, Oracle and/or its affiliates. All rights reserved.
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
	''' Provides the look and feel for a styled text editor in the
	''' Synth look and feel.
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
	''' @author  Shannon Hickey
	''' @since 1.7
	''' </summary>
	Public Class SynthTextPaneUI
		Inherits SynthEditorPaneUI

		''' <summary>
		''' Creates a UI for the JTextPane.
		''' </summary>
		''' <param name="c"> the JTextPane object </param>
		''' <returns> the UI object </returns>
		Public Shared Function createUI(ByVal c As JComponent) As ComponentUI
			Return New SynthTextPaneUI
		End Function

		''' <summary>
		''' Fetches the name used as a key to lookup properties through the
		''' UIManager.  This is used as a prefix to all the standard
		''' text properties.
		''' </summary>
		''' <returns> the name ("TextPane") </returns>
		Protected Friend Property Overrides propertyPrefix As String
			Get
				Return "TextPane"
			End Get
		End Property

		''' <summary>
		''' Installs the UI for a component.  This does the following
		''' things.
		''' <ol>
		''' <li>
		''' Sets opaqueness of the associated component according to its style,
		''' if the opaque property has not already been set by the client program.
		''' <li>
		''' Installs the default caret and highlighter into the
		''' associated component. These properties are only set if their
		''' current value is either {@code null} or an instance of
		''' <seealso cref="UIResource"/>.
		''' <li>
		''' Attaches to the editor and model.  If there is no
		''' model, a default one is created.
		''' <li>
		''' Creates the view factory and the view hierarchy used
		''' to represent the model.
		''' </ol>
		''' </summary>
		''' <param name="c"> the editor component </param>
		''' <seealso cref= javax.swing.plaf.basic.BasicTextUI#installUI </seealso>
		''' <seealso cref= ComponentUI#installUI </seealso>
		Public Overrides Sub installUI(ByVal c As JComponent)
			MyBase.installUI(c)
			updateForeground(c.foreground)
			updateFont(c.font)
		End Sub

		''' <summary>
		''' This method gets called when a bound property is changed
		''' on the associated JTextComponent.  This is a hook
		''' which UI implementations may change to reflect how the
		''' UI displays bound properties of JTextComponent subclasses.
		''' If the font, foreground or document has changed, the
		''' the appropriate property is set in the default style of
		''' the document.
		''' </summary>
		''' <param name="evt"> the property change event </param>
		Protected Friend Overrides Sub propertyChange(ByVal evt As java.beans.PropertyChangeEvent)
			MyBase.propertyChange(evt)

			Dim name As String = evt.propertyName

			If name.Equals("foreground") Then
				updateForeground(CType(evt.newValue, Color))
			ElseIf name.Equals("font") Then
				updateFont(CType(evt.newValue, Font))
			ElseIf name.Equals("document") Then
				Dim comp As JComponent = component
				updateForeground(comp.foreground)
				updateFont(comp.font)
			End If
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
				style.removeAttribute(StyleConstants.Foreground)
			Else
				StyleConstants.foregroundund(style, color)
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

			If font Is Nothing Then
				style.removeAttribute(StyleConstants.FontFamily)
				style.removeAttribute(StyleConstants.FontSize)
				style.removeAttribute(StyleConstants.Bold)
				style.removeAttribute(StyleConstants.Italic)
			Else
				StyleConstants.fontFamilyily(style, font.name)
				StyleConstants.fontSizeize(style, font.size)
				StyleConstants.boldold(style, font.bold)
				StyleConstants.italiclic(style, font.italic)
			End If
		End Sub

		Friend Overrides Sub paintBackground(ByVal context As SynthContext, ByVal g As Graphics, ByVal c As JComponent)
			context.painter.paintTextPaneBackground(context, g, 0, 0, c.width, c.height)
		End Sub

		''' <summary>
		''' {@inheritDoc}
		''' </summary>
		Public Overrides Sub paintBorder(ByVal context As SynthContext, ByVal g As Graphics, ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer)
			context.painter.paintTextPaneBorder(context, g, x, y, w, h)
		End Sub
	End Class

End Namespace