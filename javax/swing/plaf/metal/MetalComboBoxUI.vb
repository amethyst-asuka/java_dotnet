Imports System
Imports javax.swing
Imports javax.swing.plaf
Imports javax.swing.border
Imports javax.swing.plaf.basic

'
' * Copyright (c) 1998, 2013, Oracle and/or its affiliates. All rights reserved.
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

Namespace javax.swing.plaf.metal



	''' <summary>
	''' Metal UI for JComboBox
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
	''' <seealso cref= MetalComboBoxEditor </seealso>
	''' <seealso cref= MetalComboBoxButton
	''' @author Tom Santos </seealso>
	Public Class MetalComboBoxUI
		Inherits BasicComboBoxUI

		Public Shared Function createUI(ByVal c As JComponent) As ComponentUI
			Return New MetalComboBoxUI
		End Function

		Public Overrides Sub paint(ByVal g As Graphics, ByVal c As JComponent)
			If MetalLookAndFeel.usingOcean() Then MyBase.paint(g, c)
		End Sub

		''' <summary>
		''' If necessary paints the currently selected item.
		''' </summary>
		''' <param name="g"> Graphics to paint to </param>
		''' <param name="bounds"> Region to paint current value to </param>
		''' <param name="hasFocus"> whether or not the JComboBox has focus </param>
		''' <exception cref="NullPointerException"> if any of the arguments are null.
		''' @since 1.5 </exception>
		Public Overrides Sub paintCurrentValue(ByVal g As Graphics, ByVal bounds As Rectangle, ByVal hasFocus As Boolean)
			' This is really only called if we're using ocean.
			If MetalLookAndFeel.usingOcean() Then
				bounds.x += 2
				bounds.width -= 3
				If arrowButton IsNot Nothing Then
					Dim buttonInsets As Insets = arrowButton.insets
					bounds.y += buttonInsets.top
					bounds.height -= (buttonInsets.top + buttonInsets.bottom)
				Else
					bounds.y += 2
					bounds.height -= 4
				End If
				MyBase.paintCurrentValue(g, bounds, hasFocus)
			ElseIf g Is Nothing OrElse bounds Is Nothing Then
				Throw New NullPointerException("Must supply a non-null Graphics and Rectangle")
			End If
		End Sub

		''' <summary>
		''' If necessary paints the background of the currently selected item.
		''' </summary>
		''' <param name="g"> Graphics to paint to </param>
		''' <param name="bounds"> Region to paint background to </param>
		''' <param name="hasFocus"> whether or not the JComboBox has focus </param>
		''' <exception cref="NullPointerException"> if any of the arguments are null.
		''' @since 1.5 </exception>
		Public Overrides Sub paintCurrentValueBackground(ByVal g As Graphics, ByVal bounds As Rectangle, ByVal hasFocus As Boolean)
			' This is really only called if we're using ocean.
			If MetalLookAndFeel.usingOcean() Then
				g.color = MetalLookAndFeel.controlDarkShadow
				g.drawRect(bounds.x, bounds.y, bounds.width, bounds.height - 1)
				g.color = MetalLookAndFeel.controlShadow
				g.drawRect(bounds.x + 1, bounds.y + 1, bounds.width - 2, bounds.height - 3)
				If hasFocus AndAlso (Not isPopupVisible(comboBox)) AndAlso arrowButton IsNot Nothing Then
					g.color = listBox.selectionBackground
					Dim buttonInsets As Insets = arrowButton.insets
					If buttonInsets.top > 2 Then g.fillRect(bounds.x + 2, bounds.y + 2, bounds.width - 3, buttonInsets.top - 2)
					If buttonInsets.bottom > 2 Then g.fillRect(bounds.x + 2, bounds.y + bounds.height - buttonInsets.bottom, bounds.width - 3, buttonInsets.bottom - 2)
				End If
			ElseIf g Is Nothing OrElse bounds Is Nothing Then
				Throw New NullPointerException("Must supply a non-null Graphics and Rectangle")
			End If
		End Sub

		''' <summary>
		''' Returns the baseline.
		''' </summary>
		''' <exception cref="NullPointerException"> {@inheritDoc} </exception>
		''' <exception cref="IllegalArgumentException"> {@inheritDoc} </exception>
		''' <seealso cref= javax.swing.JComponent#getBaseline(int, int)
		''' @since 1.6 </seealso>
		Public Overrides Function getBaseline(ByVal c As JComponent, ByVal width As Integer, ByVal height As Integer) As Integer
			Dim ___baseline As Integer
			If MetalLookAndFeel.usingOcean() AndAlso height >= 4 Then
				height -= 4
				___baseline = MyBase.getBaseline(c, width, height)
				If ___baseline >= 0 Then ___baseline += 2
			Else
				___baseline = MyBase.getBaseline(c, width, height)
			End If
			Return ___baseline
		End Function

		Protected Friend Overrides Function createEditor() As ComboBoxEditor
			Return New MetalComboBoxEditor.UIResource
		End Function

		Protected Friend Overrides Function createPopup() As ComboPopup
			Return MyBase.createPopup()
		End Function

		Protected Friend Overrides Function createArrowButton() As JButton
			Dim iconOnly As Boolean = (comboBox.editable OrElse MetalLookAndFeel.usingOcean())
			Dim button As JButton = New MetalComboBoxButton(comboBox, New MetalComboBoxIcon, iconOnly, currentValuePane, listBox)
			button.margin = New Insets(0, 1, 1, 3)
			If MetalLookAndFeel.usingOcean() Then button.putClientProperty(MetalBorders.NO_BUTTON_ROLLOVER, Boolean.TRUE)
			updateButtonForOcean(button)
			Return button
		End Function

		''' <summary>
		''' Resets the necessary state on the ComboBoxButton for ocean.
		''' </summary>
		Private Sub updateButtonForOcean(ByVal button As JButton)
			If MetalLookAndFeel.usingOcean() Then button.focusPainted = comboBox.editable
		End Sub

		Public Overrides Function createPropertyChangeListener() As PropertyChangeListener
			Return New MetalPropertyChangeListener(Me)
		End Function

		''' <summary>
		''' This class should be treated as a &quot;protected&quot; inner class.
		''' Instantiate it only within subclasses of {@code MetalComboBoxUI}.
		''' </summary>
		Public Class MetalPropertyChangeListener
			Inherits BasicComboBoxUI.PropertyChangeHandler

			Private ReadOnly outerInstance As MetalComboBoxUI

			Public Sub New(ByVal outerInstance As MetalComboBoxUI)
				Me.outerInstance = outerInstance
			End Sub

			Public Overridable Sub propertyChange(ByVal e As PropertyChangeEvent)
				MyBase.propertyChange(e)
				Dim propertyName As String = e.propertyName

				If propertyName = "editable" Then
					If TypeOf outerInstance.arrowButton Is MetalComboBoxButton Then
								Dim button As MetalComboBoxButton = CType(outerInstance.arrowButton, MetalComboBoxButton)
								button.iconOnly = outerInstance.comboBox.editable OrElse MetalLookAndFeel.usingOcean()
					End If
							outerInstance.comboBox.repaint()
					outerInstance.updateButtonForOcean(outerInstance.arrowButton)
				ElseIf propertyName = "background" Then
					Dim color As Color = CType(e.newValue, Color)
					outerInstance.arrowButton.background = color
					outerInstance.listBox.background = color

				ElseIf propertyName = "foreground" Then
					Dim color As Color = CType(e.newValue, Color)
					outerInstance.arrowButton.foreground = color
					outerInstance.listBox.foreground = color
				End If
			End Sub
		End Class

		''' <summary>
		''' As of Java 2 platform v1.4 this method is no longer used. Do not call or
		''' override. All the functionality of this method is in the
		''' MetalPropertyChangeListener.
		''' </summary>
		''' @deprecated As of Java 2 platform v1.4. 
		<Obsolete("As of Java 2 platform v1.4.")> _
		Protected Friend Overridable Sub editablePropertyChanged(ByVal e As PropertyChangeEvent)
		End Sub

		Protected Friend Overrides Function createLayoutManager() As LayoutManager
			Return New MetalComboBoxLayoutManager(Me)
		End Function

		''' <summary>
		''' This class should be treated as a &quot;protected&quot; inner class.
		''' Instantiate it only within subclasses of {@code MetalComboBoxUI}.
		''' </summary>
		Public Class MetalComboBoxLayoutManager
			Inherits BasicComboBoxUI.ComboBoxLayoutManager

			Private ReadOnly outerInstance As MetalComboBoxUI

			Public Sub New(ByVal outerInstance As MetalComboBoxUI)
				Me.outerInstance = outerInstance
			End Sub

			Public Overridable Sub layoutContainer(ByVal parent As Container)
				outerInstance.layoutComboBox(parent, Me)
			End Sub
			Public Overridable Sub superLayout(ByVal parent As Container)
				MyBase.layoutContainer(parent)
			End Sub
		End Class

		' This is here because of a bug in the compiler.
		' When a protected-inner-class-savvy compiler comes out we
		' should move this into MetalComboBoxLayoutManager.
		Public Overridable Sub layoutComboBox(ByVal parent As Container, ByVal manager As MetalComboBoxLayoutManager)
			If comboBox.editable AndAlso (Not MetalLookAndFeel.usingOcean()) Then
				manager.superLayout(parent)
				Return
			End If

			If arrowButton IsNot Nothing Then
				If MetalLookAndFeel.usingOcean() Then
					Dim ___insets As Insets = comboBox.insets
					Dim buttonWidth As Integer = arrowButton.minimumSize.width
					arrowButton.boundsnds(If(MetalUtils.isLeftToRight(comboBox), (comboBox.width - ___insets.right - buttonWidth), ___insets.left), ___insets.top, buttonWidth, comboBox.height - ___insets.top - ___insets.bottom)
				Else
					Dim ___insets As Insets = comboBox.insets
					Dim width As Integer = comboBox.width
					Dim height As Integer = comboBox.height
					arrowButton.boundsnds(___insets.left, ___insets.top, width - (___insets.left + ___insets.right), height - (___insets.top + ___insets.bottom))
				End If
			End If

			If editor IsNot Nothing AndAlso MetalLookAndFeel.usingOcean() Then
				Dim cvb As Rectangle = rectangleForCurrentValue()
				editor.bounds = cvb
			End If
		End Sub

		''' <summary>
		''' As of Java 2 platform v1.4 this method is no
		''' longer used.
		''' </summary>
		''' @deprecated As of Java 2 platform v1.4. 
		<Obsolete("As of Java 2 platform v1.4.")> _
		Protected Friend Overridable Sub removeListeners()
			If propertyChangeListener IsNot Nothing Then comboBox.removePropertyChangeListener(propertyChangeListener)
		End Sub

		' These two methods were overloaded and made public. This was probably a
		' mistake in the implementation. The functionality that they used to
		' provide is no longer necessary and should be removed. However,
		' removing them will create an uncompatible API change.

		Public Overrides Sub configureEditor()
			MyBase.configureEditor()
		End Sub

		Public Overrides Sub unconfigureEditor()
			MyBase.unconfigureEditor()
		End Sub

		Public Overrides Function getMinimumSize(ByVal c As JComponent) As Dimension
			If Not isMinimumSizeDirty Then Return New Dimension(cachedMinimumSize)

			Dim size As Dimension = Nothing

			If (Not comboBox.editable) AndAlso arrowButton IsNot Nothing Then
				Dim buttonInsets As Insets = arrowButton.insets
				Dim ___insets As Insets = comboBox.insets

				size = displaySize
				size.width += ___insets.left + ___insets.right
				size.width += buttonInsets.right
				size.width += arrowButton.minimumSize.width
				size.height += ___insets.top + ___insets.bottom
				size.height += buttonInsets.top + buttonInsets.bottom
			ElseIf comboBox.editable AndAlso arrowButton IsNot Nothing AndAlso editor IsNot Nothing Then
				size = MyBase.getMinimumSize(c)
				Dim margin As Insets = arrowButton.margin
				size.height += margin.top + margin.bottom
				size.width += margin.left + margin.right
			Else
				size = MyBase.getMinimumSize(c)
			End If

			cachedMinimumSize.sizeize(size.width, size.height)
			isMinimumSizeDirty = False

			Return New Dimension(cachedMinimumSize)
		End Function

		''' <summary>
		''' This class should be treated as a &quot;protected&quot; inner class.
		''' Instantiate it only within subclasses of {@code MetalComboBoxUI}.
		''' 
		''' This class is now obsolete and doesn't do anything and
		''' is only included for backwards API compatibility. Do not call or
		''' override.
		''' </summary>
		''' @deprecated As of Java 2 platform v1.4. 
		<Obsolete("As of Java 2 platform v1.4.")> _
		Public Class MetalComboPopup
			Inherits BasicComboPopup

			Private ReadOnly outerInstance As MetalComboBoxUI


			Public Sub New(ByVal outerInstance As MetalComboBoxUI, ByVal cBox As JComboBox)
					Me.outerInstance = outerInstance
				MyBase.New(cBox)
			End Sub

			' This method was overloaded and made public. This was probably
			' mistake in the implementation. The functionality that they used to
			' provide is no longer necessary and should be removed. However,
			' removing them will create an uncompatible API change.

			Public Overrides Sub delegateFocus(ByVal e As MouseEvent)
				MyBase.delegateFocus(e)
			End Sub
		End Class
	End Class

End Namespace