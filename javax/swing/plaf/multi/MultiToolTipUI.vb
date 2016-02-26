Imports System.Collections

'
' * Copyright (c) 1997, 2001, Oracle and/or its affiliates. All rights reserved.
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
	''' A multiplexing UI used to combine <code>ToolTipUI</code>s.
	''' 
	''' <p>This file was automatically generated by AutoMulti.
	''' 
	''' @author  Otto Multey
	''' </summary>
	Public Class MultiToolTipUI
		Inherits javax.swing.plaf.ToolTipUI

		''' <summary>
		''' The vector containing the real UIs.  This is populated
		''' in the call to <code>createUI</code>, and can be obtained by calling
		''' the <code>getUIs</code> method.  The first element is guaranteed to be the real UI
		''' obtained from the default look and feel.
		''' </summary>
		Protected Friend uis As New ArrayList

	'//////////////////
	' Common UI methods
	'//////////////////

		''' <summary>
		''' Returns the list of UIs associated with this multiplexing UI.  This
		''' allows processing of the UIs by an application aware of multiplexing
		''' UIs on components.
		''' </summary>
		Public Overridable Property uIs As javax.swing.plaf.ComponentUI()
			Get
				Return MultiLookAndFeel.uisToArray(uis)
			End Get
		End Property

	'//////////////////
	' ToolTipUI methods
	'//////////////////

	'//////////////////
	' ComponentUI methods
	'//////////////////

		''' <summary>
		''' Invokes the <code>contains</code> method on each UI handled by this object.
		''' </summary>
		''' <returns> the value obtained from the first UI, which is
		''' the UI obtained from the default <code>LookAndFeel</code> </returns>
		Public Overrides Function contains(ByVal a As javax.swing.JComponent, ByVal b As Integer, ByVal c As Integer) As Boolean
			Dim returnValue As Boolean = CType(uis(0), javax.swing.plaf.ComponentUI).contains(a,b,c)
			For i As Integer = 1 To uis.Count - 1
				CType(uis(i), javax.swing.plaf.ComponentUI).contains(a,b,c)
			Next i
			Return returnValue
		End Function

		''' <summary>
		''' Invokes the <code>update</code> method on each UI handled by this object.
		''' </summary>
		Public Overrides Sub update(ByVal a As java.awt.Graphics, ByVal b As javax.swing.JComponent)
			For i As Integer = 0 To uis.Count - 1
				CType(uis(i), javax.swing.plaf.ComponentUI).update(a,b)
			Next i
		End Sub

		''' <summary>
		''' Returns a multiplexing UI instance if any of the auxiliary
		''' <code>LookAndFeel</code>s supports this UI.  Otherwise, just returns the
		''' UI object obtained from the default <code>LookAndFeel</code>.
		''' </summary>
		Public Shared Function createUI(ByVal a As javax.swing.JComponent) As javax.swing.plaf.ComponentUI
			Dim mui As javax.swing.plaf.ComponentUI = New MultiToolTipUI
			Return MultiLookAndFeel.createUIs(mui, CType(mui, MultiToolTipUI).uis, a)
		End Function

		''' <summary>
		''' Invokes the <code>installUI</code> method on each UI handled by this object.
		''' </summary>
		Public Overrides Sub installUI(ByVal a As javax.swing.JComponent)
			For i As Integer = 0 To uis.Count - 1
				CType(uis(i), javax.swing.plaf.ComponentUI).installUI(a)
			Next i
		End Sub

		''' <summary>
		''' Invokes the <code>uninstallUI</code> method on each UI handled by this object.
		''' </summary>
		Public Overrides Sub uninstallUI(ByVal a As javax.swing.JComponent)
			For i As Integer = 0 To uis.Count - 1
				CType(uis(i), javax.swing.plaf.ComponentUI).uninstallUI(a)
			Next i
		End Sub

		''' <summary>
		''' Invokes the <code>paint</code> method on each UI handled by this object.
		''' </summary>
		Public Overrides Sub paint(ByVal a As java.awt.Graphics, ByVal b As javax.swing.JComponent)
			For i As Integer = 0 To uis.Count - 1
				CType(uis(i), javax.swing.plaf.ComponentUI).paint(a,b)
			Next i
		End Sub

		''' <summary>
		''' Invokes the <code>getPreferredSize</code> method on each UI handled by this object.
		''' </summary>
		''' <returns> the value obtained from the first UI, which is
		''' the UI obtained from the default <code>LookAndFeel</code> </returns>
		Public Overrides Function getPreferredSize(ByVal a As javax.swing.JComponent) As java.awt.Dimension
			Dim returnValue As java.awt.Dimension = CType(uis(0), javax.swing.plaf.ComponentUI).getPreferredSize(a)
			For i As Integer = 1 To uis.Count - 1
				CType(uis(i), javax.swing.plaf.ComponentUI).getPreferredSize(a)
			Next i
			Return returnValue
		End Function

		''' <summary>
		''' Invokes the <code>getMinimumSize</code> method on each UI handled by this object.
		''' </summary>
		''' <returns> the value obtained from the first UI, which is
		''' the UI obtained from the default <code>LookAndFeel</code> </returns>
		Public Overrides Function getMinimumSize(ByVal a As javax.swing.JComponent) As java.awt.Dimension
			Dim returnValue As java.awt.Dimension = CType(uis(0), javax.swing.plaf.ComponentUI).getMinimumSize(a)
			For i As Integer = 1 To uis.Count - 1
				CType(uis(i), javax.swing.plaf.ComponentUI).getMinimumSize(a)
			Next i
			Return returnValue
		End Function

		''' <summary>
		''' Invokes the <code>getMaximumSize</code> method on each UI handled by this object.
		''' </summary>
		''' <returns> the value obtained from the first UI, which is
		''' the UI obtained from the default <code>LookAndFeel</code> </returns>
		Public Overrides Function getMaximumSize(ByVal a As javax.swing.JComponent) As java.awt.Dimension
			Dim returnValue As java.awt.Dimension = CType(uis(0), javax.swing.plaf.ComponentUI).getMaximumSize(a)
			For i As Integer = 1 To uis.Count - 1
				CType(uis(i), javax.swing.plaf.ComponentUI).getMaximumSize(a)
			Next i
			Return returnValue
		End Function

		''' <summary>
		''' Invokes the <code>getAccessibleChildrenCount</code> method on each UI handled by this object.
		''' </summary>
		''' <returns> the value obtained from the first UI, which is
		''' the UI obtained from the default <code>LookAndFeel</code> </returns>
		Public Overrides Function getAccessibleChildrenCount(ByVal a As javax.swing.JComponent) As Integer
			Dim returnValue As Integer = CType(uis(0), javax.swing.plaf.ComponentUI).getAccessibleChildrenCount(a)
			For i As Integer = 1 To uis.Count - 1
				CType(uis(i), javax.swing.plaf.ComponentUI).getAccessibleChildrenCount(a)
			Next i
			Return returnValue
		End Function

		''' <summary>
		''' Invokes the <code>getAccessibleChild</code> method on each UI handled by this object.
		''' </summary>
		''' <returns> the value obtained from the first UI, which is
		''' the UI obtained from the default <code>LookAndFeel</code> </returns>
		Public Overrides Function getAccessibleChild(ByVal a As javax.swing.JComponent, ByVal b As Integer) As javax.accessibility.Accessible
			Dim returnValue As javax.accessibility.Accessible = CType(uis(0), javax.swing.plaf.ComponentUI).getAccessibleChild(a,b)
			For i As Integer = 1 To uis.Count - 1
				CType(uis(i), javax.swing.plaf.ComponentUI).getAccessibleChild(a,b)
			Next i
			Return returnValue
		End Function
	End Class

End Namespace