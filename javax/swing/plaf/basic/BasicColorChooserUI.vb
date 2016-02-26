Imports javax.swing
Imports javax.swing.colorchooser
Imports javax.swing.event
Imports javax.swing.border
Imports javax.swing.plaf

'
' * Copyright (c) 1997, 2012, Oracle and/or its affiliates. All rights reserved.
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
	''' Provides the basic look and feel for a JColorChooser.
	''' <p>
	''' @author Tom Santos
	''' @author Steve Wilson
	''' </summary>

	Public Class BasicColorChooserUI
		Inherits ColorChooserUI

		''' <summary>
		''' JColorChooser this BasicColorChooserUI is installed on.
		''' 
		''' @since 1.5
		''' </summary>
		Protected Friend chooser As JColorChooser

		Friend tabbedPane As JTabbedPane
		Friend singlePanel As JPanel

		Friend previewPanelHolder As JPanel
		Friend previewPanel As JComponent
		Friend isMultiPanel As Boolean = False
		Private Shared defaultTransferHandler As TransferHandler = New ColorTransferHandler

		Protected Friend defaultChoosers As AbstractColorChooserPanel()

		Protected Friend previewListener As ChangeListener
		Protected Friend propertyChangeListener As java.beans.PropertyChangeListener
		Private handler As Handler

		Public Shared Function createUI(ByVal c As JComponent) As ComponentUI
			Return New BasicColorChooserUI
		End Function

		Protected Friend Overridable Function createDefaultChoosers() As AbstractColorChooserPanel()
			Dim panels As AbstractColorChooserPanel() = ColorChooserComponentFactory.defaultChooserPanels
			Return panels
		End Function

		Protected Friend Overridable Sub uninstallDefaultChoosers()
			Dim choosers As AbstractColorChooserPanel() = chooser.chooserPanels
			For i As Integer = 0 To choosers.Length - 1
				chooser.removeChooserPanel(choosers(i))
			Next i
		End Sub

		Public Overridable Sub installUI(ByVal c As JComponent)
			chooser = CType(c, JColorChooser)

			MyBase.installUI(c)

			installDefaults()
			installListeners()

			tabbedPane = New JTabbedPane
			tabbedPane.name = "ColorChooser.tabPane"
			tabbedPane.inheritsPopupMenu = True
			tabbedPane.accessibleContext.accessibleDescription = tabbedPane.name
			singlePanel = New JPanel(New CenterLayout)
			singlePanel.name = "ColorChooser.panel"
			singlePanel.inheritsPopupMenu = True

			chooser.layout = New BorderLayout

			defaultChoosers = createDefaultChoosers()
			chooser.chooserPanels = defaultChoosers

			previewPanelHolder = New JPanel(New CenterLayout)
			previewPanelHolder.name = "ColorChooser.previewPanelHolder"

			If sun.swing.DefaultLookup.getBoolean(chooser, Me, "ColorChooser.showPreviewPanelText", True) Then
				Dim previewString As String = UIManager.getString("ColorChooser.previewText", chooser.locale)
				previewPanelHolder.border = New TitledBorder(previewString)
			End If
			previewPanelHolder.inheritsPopupMenu = True

			installPreviewPanel()
			chooser.applyComponentOrientation(c.componentOrientation)
		End Sub

		Public Overridable Sub uninstallUI(ByVal c As JComponent)
			chooser.remove(tabbedPane)
			chooser.remove(singlePanel)
			chooser.remove(previewPanelHolder)

			uninstallDefaultChoosers()
			uninstallListeners()
			uninstallPreviewPanel()
			uninstallDefaults()

			previewPanelHolder = Nothing
			previewPanel = Nothing
			defaultChoosers = Nothing
			chooser = Nothing
			tabbedPane = Nothing

			handler = Nothing
		End Sub

		Protected Friend Overridable Sub installPreviewPanel()
			Dim previewPanel As JComponent = Me.chooser.previewPanel
			If previewPanel Is Nothing Then
				previewPanel = ColorChooserComponentFactory.previewPanel
			ElseIf GetType(JPanel).Equals(previewPanel.GetType()) AndAlso (0 = previewPanel.componentCount) Then
				previewPanel = Nothing
			End If
			Me.previewPanel = previewPanel
			If previewPanel IsNot Nothing Then
				chooser.add(previewPanelHolder, BorderLayout.SOUTH)
				previewPanel.foreground = chooser.color
				previewPanelHolder.add(previewPanel)
				previewPanel.addMouseListener(handler)
				previewPanel.inheritsPopupMenu = True
			End If
		End Sub

		''' <summary>
		''' Removes installed preview panel from the UI delegate.
		''' 
		''' @since 1.7
		''' </summary>
		Protected Friend Overridable Sub uninstallPreviewPanel()
			If Me.previewPanel IsNot Nothing Then
				Me.previewPanel.removeMouseListener(handler)
				Me.previewPanelHolder.remove(Me.previewPanel)
			End If
			Me.chooser.remove(Me.previewPanelHolder)
		End Sub

		Protected Friend Overridable Sub installDefaults()
			LookAndFeel.installColorsAndFont(chooser, "ColorChooser.background", "ColorChooser.foreground", "ColorChooser.font")
			LookAndFeel.installProperty(chooser, "opaque", Boolean.TRUE)
			Dim th As TransferHandler = chooser.transferHandler
			If th Is Nothing OrElse TypeOf th Is UIResource Then chooser.transferHandler = defaultTransferHandler
		End Sub

		Protected Friend Overridable Sub uninstallDefaults()
			If TypeOf chooser.transferHandler Is UIResource Then chooser.transferHandler = Nothing
		End Sub


		Protected Friend Overridable Sub installListeners()
			propertyChangeListener = createPropertyChangeListener()
			chooser.addPropertyChangeListener(propertyChangeListener)

			previewListener = handler
			chooser.selectionModel.addChangeListener(previewListener)
		End Sub

		Private Property handler As Handler
			Get
				If handler Is Nothing Then handler = New Handler(Me)
				Return handler
			End Get
		End Property

		Protected Friend Overridable Function createPropertyChangeListener() As java.beans.PropertyChangeListener
			Return handler
		End Function

		Protected Friend Overridable Sub uninstallListeners()
			chooser.removePropertyChangeListener(propertyChangeListener)
			chooser.selectionModel.removeChangeListener(previewListener)
			previewListener = Nothing
		End Sub

		Private Sub selectionChanged(ByVal model As ColorSelectionModel)
			Dim previewPanel As JComponent = Me.chooser.previewPanel
			If previewPanel IsNot Nothing Then
				previewPanel.foreground = model.selectedColor
				previewPanel.repaint()
			End If
			Dim panels As AbstractColorChooserPanel() = Me.chooser.chooserPanels
			If panels IsNot Nothing Then
				For Each panel As AbstractColorChooserPanel In panels
					If panel IsNot Nothing Then panel.updateChooser()
				Next panel
			End If
		End Sub

		Private Class Handler
			Implements ChangeListener, MouseListener, java.beans.PropertyChangeListener

			Private ReadOnly outerInstance As BasicColorChooserUI

			Public Sub New(ByVal outerInstance As BasicColorChooserUI)
				Me.outerInstance = outerInstance
			End Sub

			'
			' ChangeListener
			'
			Public Overridable Sub stateChanged(ByVal evt As ChangeEvent) Implements ChangeListener.stateChanged
				outerInstance.selectionChanged(CType(evt.source, ColorSelectionModel))
			End Sub

			'
			' MouseListener
			Public Overridable Sub mousePressed(ByVal evt As MouseEvent)
				If outerInstance.chooser.dragEnabled Then
					Dim th As TransferHandler = outerInstance.chooser.transferHandler
					th.exportAsDrag(outerInstance.chooser, evt, TransferHandler.COPY)
				End If
			End Sub
			Public Overridable Sub mouseReleased(ByVal evt As MouseEvent)
			End Sub
			Public Overridable Sub mouseClicked(ByVal evt As MouseEvent)
			End Sub
			Public Overridable Sub mouseEntered(ByVal evt As MouseEvent)
			End Sub
			Public Overridable Sub mouseExited(ByVal evt As MouseEvent)
			End Sub

			'
			' PropertyChangeListener
			'
			Public Overridable Sub propertyChange(ByVal evt As java.beans.PropertyChangeEvent)
				Dim prop As String = evt.propertyName

				If prop = JColorChooser.CHOOSER_PANELS_PROPERTY Then
					Dim oldPanels As AbstractColorChooserPanel() = CType(evt.oldValue, AbstractColorChooserPanel())
					Dim newPanels As AbstractColorChooserPanel() = CType(evt.newValue, AbstractColorChooserPanel())

					For i As Integer = 0 To oldPanels.Length - 1 ' remove old panels
					   Dim wrapper As Container = oldPanels(i).parent
						If wrapper IsNot Nothing Then
						  Dim parent As Container = wrapper.parent
						  If parent IsNot Nothing Then parent.remove(wrapper) ' remove from hierarchy
						  oldPanels(i).uninstallChooserPanel(outerInstance.chooser) ' uninstall
						End If
					Next i

					Dim numNewPanels As Integer = newPanels.Length
					If numNewPanels = 0 Then ' removed all panels and added none
						outerInstance.chooser.remove(outerInstance.tabbedPane)
						Return
					ElseIf numNewPanels = 1 Then ' one panel case
						outerInstance.chooser.remove(outerInstance.tabbedPane)
						Dim centerWrapper As New JPanel(New CenterLayout)
						centerWrapper.inheritsPopupMenu = True
						centerWrapper.add(newPanels(0))
						outerInstance.singlePanel.add(centerWrapper, BorderLayout.CENTER)
						outerInstance.chooser.add(outerInstance.singlePanel)
					Else ' multi-panel case
						If oldPanels.Length < 2 Then ' moving from single to multiple
							outerInstance.chooser.remove(outerInstance.singlePanel)
							outerInstance.chooser.add(outerInstance.tabbedPane, BorderLayout.CENTER)
						End If

						For i As Integer = 0 To newPanels.Length - 1
							Dim centerWrapper As New JPanel(New CenterLayout)
							centerWrapper.inheritsPopupMenu = True
							Dim name As String = newPanels(i).displayName
							Dim mnemonic As Integer = newPanels(i).mnemonic
							centerWrapper.add(newPanels(i))
							outerInstance.tabbedPane.addTab(name, centerWrapper)
							If mnemonic > 0 Then
								outerInstance.tabbedPane.mnemonicAtcAt(i, mnemonic)
								Dim index As Integer = newPanels(i).displayedMnemonicIndex
								If index >= 0 Then outerInstance.tabbedPane.displayedMnemonicIndexAtxAt(i, index)
							End If
						Next i
					End If
					outerInstance.chooser.applyComponentOrientation(outerInstance.chooser.componentOrientation)
					For i As Integer = 0 To newPanels.Length - 1
						newPanels(i).installChooserPanel(outerInstance.chooser)
					Next i
				ElseIf prop = JColorChooser.PREVIEW_PANEL_PROPERTY Then
					outerInstance.uninstallPreviewPanel()
					outerInstance.installPreviewPanel()
				ElseIf prop = JColorChooser.SELECTION_MODEL_PROPERTY Then
					Dim oldModel As ColorSelectionModel = CType(evt.oldValue, ColorSelectionModel)
					oldModel.removeChangeListener(outerInstance.previewListener)
					Dim newModel As ColorSelectionModel = CType(evt.newValue, ColorSelectionModel)
					newModel.addChangeListener(outerInstance.previewListener)
					outerInstance.selectionChanged(newModel)
				ElseIf prop = "componentOrientation" Then
					Dim o As ComponentOrientation = CType(evt.newValue, ComponentOrientation)
					Dim cc As JColorChooser = CType(evt.source, JColorChooser)
					If o IsNot CType(evt.oldValue, ComponentOrientation) Then
						cc.applyComponentOrientation(o)
						cc.updateUI()
					End If
				End If
			End Sub
		End Class

		''' <summary>
		''' This class should be treated as a &quot;protected&quot; inner class.
		''' Instantiate it only within subclasses of {@code BasicColorChooserUI}.
		''' </summary>
		Public Class PropertyHandler
			Implements java.beans.PropertyChangeListener

			Private ReadOnly outerInstance As BasicColorChooserUI

			Public Sub New(ByVal outerInstance As BasicColorChooserUI)
				Me.outerInstance = outerInstance
			End Sub

			Public Overridable Sub propertyChange(ByVal e As java.beans.PropertyChangeEvent)
				outerInstance.handler.propertyChange(e)
			End Sub
		End Class

		Friend Class ColorTransferHandler
			Inherits TransferHandler
			Implements UIResource

			Friend Sub New()
				MyBase.New("color")
			End Sub
		End Class
	End Class

End Namespace