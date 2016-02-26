Imports javax.swing
Imports javax.swing.text
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
	''' Provides the look and feel for a styled text editor.
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
	Public Class BasicTextPaneUI
		Inherits BasicEditorPaneUI

		''' <summary>
		''' Creates a UI for the JTextPane.
		''' </summary>
		''' <param name="c"> the JTextPane object </param>
		''' <returns> the UI </returns>
		Public Shared Function createUI(ByVal c As JComponent) As ComponentUI
			Return New BasicTextPaneUI
		End Function

		''' <summary>
		''' Creates a new BasicTextPaneUI.
		''' </summary>
		Public Sub New()
			MyBase.New()
		End Sub

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

		Public Overrides Sub installUI(ByVal c As JComponent)
			MyBase.installUI(c)
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
		Protected Friend Overrides Sub propertyChange(ByVal evt As PropertyChangeEvent)
			MyBase.propertyChange(evt)
		End Sub
	End Class

End Namespace