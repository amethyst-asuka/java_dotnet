Imports System
Imports System.Collections
Imports javax.swing
Imports javax.swing.plaf

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
Namespace javax.swing.plaf.multi

	''' <summary>
	''' <p>A multiplexing look and feel that allows more than one UI
	''' to be associated with a component at the same time.
	''' The primary look and feel is called
	''' the <em>default</em> look and feel,
	''' and the other look and feels are called <em>auxiliary</em>.
	''' <p>
	''' 
	''' For further information, see
	''' <a href="doc-files/multi_tsc.html" target="_top">Using the
	''' Multiplexing Look and Feel.</a>
	''' 
	''' <p>
	''' <strong>Warning:</strong>
	''' Serialized objects of this class will not be compatible with
	''' future Swing releases. The current serialization support is
	''' appropriate for short term storage or RMI between applications running
	''' the same version of Swing.  As of 1.4, support for long term storage
	''' of all JavaBeans&trade;
	''' has been added to the <code>java.beans</code> package.
	''' Please see <seealso cref="java.beans.XMLEncoder"/>.
	''' </summary>
	''' <seealso cref= UIManager#addAuxiliaryLookAndFeel </seealso>
	''' <seealso cref= javax.swing.plaf.multi
	''' 
	''' @author Willie Walker </seealso>
	Public Class MultiLookAndFeel
		Inherits LookAndFeel

	'////////////////////////////
	' LookAndFeel methods
	'////////////////////////////

		''' <summary>
		''' Returns a string, suitable for use in menus,
		''' that identifies this look and feel.
		''' </summary>
		''' <returns> a string such as "Multiplexing Look and Feel" </returns>
		Public Property Overrides name As String
			Get
				Return "Multiplexing Look and Feel"
			End Get
		End Property

		''' <summary>
		''' Returns a string, suitable for use by applications/services,
		''' that identifies this look and feel.
		''' </summary>
		''' <returns> "Multiplex" </returns>
		Public Property Overrides iD As String
			Get
				Return "Multiplex"
			End Get
		End Property

		''' <summary>
		''' Returns a one-line description of this look and feel.
		''' </summary>
		''' <returns> a descriptive string such as "Allows multiple UI instances per component instance" </returns>
		Public Property Overrides description As String
			Get
				Return "Allows multiple UI instances per component instance"
			End Get
		End Property

		''' <summary>
		''' Returns <code>false</code>;
		''' this look and feel is not native to any platform.
		''' </summary>
		''' <returns> <code>false</code> </returns>
		Public Property Overrides nativeLookAndFeel As Boolean
			Get
				Return False
			End Get
		End Property

		''' <summary>
		''' Returns <code>true</code>;
		''' every platform permits this look and feel.
		''' </summary>
		''' <returns> <code>true</code> </returns>
		Public Property Overrides supportedLookAndFeel As Boolean
			Get
				Return True
			End Get
		End Property

		''' <summary>
		''' Creates, initializes, and returns
		''' the look and feel specific defaults.
		''' For this look and feel,
		''' the defaults consist solely of
		''' mappings of UI class IDs
		''' (such as "ButtonUI")
		''' to <code>ComponentUI</code> class names
		''' (such as "javax.swing.plaf.multi.MultiButtonUI").
		''' </summary>
		''' <returns> an initialized <code>UIDefaults</code> object </returns>
		''' <seealso cref= javax.swing.JComponent#getUIClassID </seealso>
		Public Property Overrides defaults As UIDefaults
			Get
				Dim packageName As String = "javax.swing.plaf.multi.Multi"
				Dim uiDefaults As Object() = { "ButtonUI", packageName & "ButtonUI", "CheckBoxMenuItemUI", packageName & "MenuItemUI", "CheckBoxUI", packageName & "ButtonUI", "ColorChooserUI", packageName & "ColorChooserUI", "ComboBoxUI", packageName & "ComboBoxUI", "DesktopIconUI", packageName & "DesktopIconUI", "DesktopPaneUI", packageName & "DesktopPaneUI", "EditorPaneUI", packageName & "TextUI", "FileChooserUI", packageName & "FileChooserUI", "FormattedTextFieldUI", packageName & "TextUI", "InternalFrameUI", packageName & "InternalFrameUI", "LabelUI", packageName & "LabelUI", "ListUI", packageName & "ListUI", "MenuBarUI", packageName & "MenuBarUI", "MenuItemUI", packageName & "MenuItemUI", "MenuUI", packageName & "MenuItemUI", "OptionPaneUI", packageName & "OptionPaneUI", "PanelUI", packageName & "PanelUI", "PasswordFieldUI", packageName & "TextUI", "PopupMenuSeparatorUI", packageName & "SeparatorUI", "PopupMenuUI", packageName & "PopupMenuUI", "ProgressBarUI", packageName & "ProgressBarUI", "RadioButtonMenuItemUI", packageName & "MenuItemUI", "RadioButtonUI", packageName & "ButtonUI", "RootPaneUI", packageName & "RootPaneUI", "ScrollBarUI", packageName & "ScrollBarUI", "ScrollPaneUI", packageName & "ScrollPaneUI", "SeparatorUI", packageName & "SeparatorUI", "SliderUI", packageName & "SliderUI", "SpinnerUI", packageName & "SpinnerUI", "SplitPaneUI", packageName & "SplitPaneUI", "TabbedPaneUI", packageName & "TabbedPaneUI", "TableHeaderUI", packageName & "TableHeaderUI", "TableUI", packageName & "TableUI", "TextAreaUI", packageName & "TextUI", "TextFieldUI", packageName & "TextUI", "TextPaneUI", packageName & "TextUI", "ToggleButtonUI", packageName & "ButtonUI", "ToolBarSeparatorUI", packageName & "SeparatorUI", "ToolBarUI", packageName & "ToolBarUI", "ToolTipUI", packageName & "ToolTipUI", "TreeUI", packageName & "TreeUI", "ViewportUI", packageName & "ViewportUI" }
    
				Dim table As UIDefaults = New MultiUIDefaults(uiDefaults.Length \ 2, 0.75f)
				table.putDefaults(uiDefaults)
				Return table
			End Get
		End Property

	'/////////////////////////////
	' Utility methods for the UI's
	'/////////////////////////////

		''' <summary>
		''' Creates the <code>ComponentUI</code> objects
		''' required to present
		''' the <code>target</code> component,
		''' placing the objects in the <code>uis</code> vector and
		''' returning the
		''' <code>ComponentUI</code> object
		''' that best represents the component's UI.
		''' This method finds the <code>ComponentUI</code> objects
		''' by invoking
		''' <code>getDefaults().getUI(target)</code> on each
		''' default and auxiliary look and feel currently in use.
		''' The first UI object this method adds
		''' to the <code>uis</code> vector
		''' is for the default look and feel.
		''' <p>
		''' This method is invoked by the <code>createUI</code> method
		''' of <code>MultiXxxxUI</code> classes.
		''' </summary>
		''' <param name="mui"> the <code>ComponentUI</code> object
		'''            that represents the complete UI
		'''            for the <code>target</code> component;
		'''            this should be an instance
		'''            of one of the <code>MultiXxxxUI</code> classes </param>
		''' <param name="uis"> a <code>Vector</code>;
		'''            generally this is the <code>uis</code> field
		'''            of the <code>mui</code> argument </param>
		''' <param name="target"> a component whose UI is represented by <code>mui</code>
		''' </param>
		''' <returns> <code>mui</code> if the component has any auxiliary UI objects;
		'''         otherwise, returns the UI object for the default look and feel
		'''         or <code>null</code> if the default UI object couldn't be found
		''' </returns>
		''' <seealso cref= javax.swing.UIManager#getAuxiliaryLookAndFeels </seealso>
		''' <seealso cref= javax.swing.UIDefaults#getUI </seealso>
		''' <seealso cref= MultiButtonUI#uis </seealso>
		''' <seealso cref= MultiButtonUI#createUI </seealso>
		Public Shared Function createUIs(ByVal mui As ComponentUI, ByVal uis As ArrayList, ByVal target As JComponent) As ComponentUI
			Dim ui As ComponentUI

			' Make sure we can at least get the default UI
			'
			ui = UIManager.defaults.getUI(target)
			If ui IsNot Nothing Then
				uis.Add(ui)
				Dim auxiliaryLookAndFeels As LookAndFeel()
				auxiliaryLookAndFeels = UIManager.auxiliaryLookAndFeels
				If auxiliaryLookAndFeels IsNot Nothing Then
					For i As Integer = 0 To auxiliaryLookAndFeels.Length - 1
						ui = auxiliaryLookAndFeels(i).defaults.getUI(target)
						If ui IsNot Nothing Then uis.Add(ui)
					Next i
				End If
			Else
				Return Nothing
			End If

			' Don't bother returning the multiplexing UI if all we did was
			' get a UI from just the default look and feel.
			'
			If uis.Count = 1 Then
				Return CType(uis(0), ComponentUI)
			Else
				Return mui
			End If
		End Function

		''' <summary>
		''' Creates an array,
		''' populates it with UI objects from the passed-in vector,
		''' and returns the array.
		''' If <code>uis</code> is null,
		''' this method returns an array with zero elements.
		''' If <code>uis</code> is an empty vector,
		''' this method returns <code>null</code>.
		''' A run-time error occurs if any objects in the <code>uis</code> vector
		''' are not of type <code>ComponentUI</code>.
		''' </summary>
		''' <param name="uis"> a vector containing <code>ComponentUI</code> objects </param>
		''' <returns> an array equivalent to the passed-in vector
		'''  </returns>
		Protected Friend Shared Function uisToArray(ByVal uis As ArrayList) As ComponentUI()
			If uis Is Nothing Then
				Return New ComponentUI(){}
			Else
				Dim count As Integer = uis.Count
				If count > 0 Then
					Dim u As ComponentUI() = New ComponentUI(count - 1){}
					For i As Integer = 0 To count - 1
						u(i) = CType(uis(i), ComponentUI)
					Next i
					Return u
				Else
					Return Nothing
				End If
			End If
		End Function
	End Class

	''' <summary>
	''' We want the Multiplexing LookAndFeel to be quiet and fallback
	''' gracefully if it cannot find a UI.  This class overrides the
	''' getUIError method of UIDefaults, which is the method that
	''' emits error messages when it cannot find a UI class in the
	''' LAF.
	''' </summary>
	Friend Class MultiUIDefaults
		Inherits UIDefaults

		Friend Sub New(ByVal initialCapacity As Integer, ByVal loadFactor As Single)
			MyBase.New(initialCapacity, loadFactor)
		End Sub
		Protected Friend Overrides Sub getUIError(ByVal msg As String)
			Console.Error.WriteLine("Multiplexing LAF:  " & msg)
		End Sub
	End Class

End Namespace