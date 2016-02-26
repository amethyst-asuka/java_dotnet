Imports System
Imports System.Collections.Generic
Imports System.Text
Imports javax.swing
Imports javax.swing.filechooser
Imports javax.swing.event
Imports javax.swing.plaf
Imports javax.swing.plaf.basic
Imports javax.accessibility
Imports sun.swing

'
' * Copyright (c) 1998, 2015, Oracle and/or its affiliates. All rights reserved.
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
	''' Metal L&amp;F implementation of a FileChooser.
	''' 
	''' @author Jeff Dinkins
	''' </summary>
	Public Class MetalFileChooserUI
		Inherits BasicFileChooserUI

		' Much of the Metal UI for JFilechooser is just a copy of
		' the windows implementation, but using Metal themed buttons, lists,
		' icons, etc. We are planning a complete rewrite, and hence we've
		' made most things in this class private.

		Private lookInLabel As JLabel
		Private directoryComboBox As JComboBox
		Private directoryComboBoxModel As DirectoryComboBoxModel
		Private directoryComboBoxAction As Action = New DirectoryComboBoxAction(Me)

		Private filterComboBoxModel As FilterComboBoxModel

		Private fileNameTextField As JTextField

		Private filePane As FilePane
		Private listViewButton As JToggleButton
		Private detailsViewButton As JToggleButton

		Private approveButton As JButton
		Private cancelButton As JButton

		Private buttonPanel As JPanel
		Private bottomPanel As JPanel

		Private filterComboBox As JComboBox

		Private Shared ReadOnly hstrut5 As New Dimension(5, 1)
		Private Shared ReadOnly hstrut11 As New Dimension(11, 1)

		Private Shared ReadOnly vstrut5 As New Dimension(1, 5)

		Private Shared ReadOnly shrinkwrap As New Insets(0,0,0,0)

		' Preferred and Minimum sizes for the dialog box
		Private Shared PREF_WIDTH As Integer = 500
		Private Shared PREF_HEIGHT As Integer = 326
		Private Shared PREF_SIZE As New Dimension(PREF_WIDTH, PREF_HEIGHT)

		Private Shared MIN_WIDTH As Integer = 500
		Private Shared MIN_HEIGHT As Integer = 326
		Private Shared LIST_PREF_WIDTH As Integer = 405
		Private Shared LIST_PREF_HEIGHT As Integer = 135
		Private Shared LIST_PREF_SIZE As New Dimension(LIST_PREF_WIDTH, LIST_PREF_HEIGHT)

		' Labels, mnemonics, and tooltips (oh my!)
		Private lookInLabelMnemonic As Integer = 0
		Private lookInLabelText As String = Nothing
		Private saveInLabelText As String = Nothing

		Private fileNameLabelMnemonic As Integer = 0
		Private fileNameLabelText As String = Nothing
		Private folderNameLabelMnemonic As Integer = 0
		Private folderNameLabelText As String = Nothing

		Private filesOfTypeLabelMnemonic As Integer = 0
		Private filesOfTypeLabelText As String = Nothing

		Private upFolderToolTipText As String = Nothing
		Private upFolderAccessibleName As String = Nothing

		Private homeFolderToolTipText As String = Nothing
		Private homeFolderAccessibleName As String = Nothing

		Private newFolderToolTipText As String = Nothing
		Private newFolderAccessibleName As String = Nothing

		Private listViewButtonToolTipText As String = Nothing
		Private listViewButtonAccessibleName As String = Nothing

		Private detailsViewButtonToolTipText As String = Nothing
		Private detailsViewButtonAccessibleName As String = Nothing

		Private fileNameLabel As AlignedLabel

		Private Sub populateFileNameLabel()
			If fileChooser.fileSelectionMode = JFileChooser.DIRECTORIES_ONLY Then
				fileNameLabel.text = folderNameLabelText
				fileNameLabel.displayedMnemonic = folderNameLabelMnemonic
			Else
				fileNameLabel.text = fileNameLabelText
				fileNameLabel.displayedMnemonic = fileNameLabelMnemonic
			End If
		End Sub

		'
		' ComponentUI Interface Implementation methods
		'
		Public Shared Function createUI(ByVal c As JComponent) As ComponentUI
			Return New MetalFileChooserUI(CType(c, JFileChooser))
		End Function

		Public Sub New(ByVal filechooser As JFileChooser)
			MyBase.New(filechooser)
		End Sub

		Public Overrides Sub installUI(ByVal c As JComponent)
			MyBase.installUI(c)
		End Sub

		Public Overrides Sub uninstallComponents(ByVal fc As JFileChooser)
			fc.removeAll()
			bottomPanel = Nothing
			buttonPanel = Nothing
		End Sub

		Private Class MetalFileChooserUIAccessor
			Implements FilePane.FileChooserUIAccessor

			Private ReadOnly outerInstance As MetalFileChooserUI

			Public Sub New(ByVal outerInstance As MetalFileChooserUI)
				Me.outerInstance = outerInstance
			End Sub

			Public Overridable Property fileChooser As JFileChooser
				Get
					Return outerInstance.fileChooser
				End Get
			End Property

			Public Overridable Property model As BasicDirectoryModel
				Get
					Return outerInstance.model
				End Get
			End Property

			Public Overridable Function createList() As JPanel
				Return outerInstance.createList(fileChooser)
			End Function

			Public Overridable Function createDetailsView() As JPanel
				Return outerInstance.createDetailsView(fileChooser)
			End Function

			Public Overridable Property directorySelected As Boolean
				Get
					Return outerInstance.directorySelected
				End Get
			End Property

			Public Overridable Property directory As java.io.File
				Get
					Return outerInstance.directory
				End Get
			End Property

			Public Overridable Property changeToParentDirectoryAction As Action
				Get
					Return outerInstance.changeToParentDirectoryAction
				End Get
			End Property

			Public Overridable Property approveSelectionAction As Action
				Get
					Return outerInstance.approveSelectionAction
				End Get
			End Property

			Public Overridable Property newFolderAction As Action
				Get
					Return outerInstance.newFolderAction
				End Get
			End Property

			Public Overridable Function createDoubleClickListener(ByVal list As JList) As MouseListener
				Return outerInstance.createDoubleClickListener(fileChooser, list)
			End Function

			Public Overridable Function createListSelectionListener() As ListSelectionListener
				Return outerInstance.createListSelectionListener(fileChooser)
			End Function
		End Class

		Public Overrides Sub installComponents(ByVal fc As JFileChooser)
			Dim fsv As FileSystemView = fc.fileSystemView

			fc.border = New javax.swing.border.EmptyBorder(12, 12, 11, 11)
			fc.layout = New BorderLayout(0, 11)

			filePane = New FilePane(New MetalFileChooserUIAccessor(Me))
			fc.addPropertyChangeListener(filePane)

			' ********************************* //
			' **** Construct the top panel **** //
			' ********************************* //

			' Directory manipulation buttons
			Dim topPanel As New JPanel(New BorderLayout(11, 0))
			Dim topButtonPanel As New JPanel
			topButtonPanel.layout = New BoxLayout(topButtonPanel, BoxLayout.LINE_AXIS)
			topPanel.add(topButtonPanel, BorderLayout.AFTER_LINE_ENDS)

			' Add the top panel to the fileChooser
			fc.add(topPanel, BorderLayout.NORTH)

			' ComboBox Label
			lookInLabel = New JLabel(lookInLabelText)
			lookInLabel.displayedMnemonic = lookInLabelMnemonic
			topPanel.add(lookInLabel, BorderLayout.BEFORE_LINE_BEGINS)

			' CurrentDir ComboBox
			directoryComboBox = New JComboBoxAnonymousInnerClassHelper(Of E)
			directoryComboBox.putClientProperty(AccessibleContext.ACCESSIBLE_DESCRIPTION_PROPERTY, lookInLabelText)
			directoryComboBox.putClientProperty("JComboBox.isTableCellEditor", Boolean.TRUE)
			lookInLabel.labelFor = directoryComboBox
			directoryComboBoxModel = createDirectoryComboBoxModel(fc)
			directoryComboBox.model = directoryComboBoxModel
			directoryComboBox.addActionListener(directoryComboBoxAction)
			directoryComboBox.renderer = createDirectoryComboBoxRenderer(fc)
			directoryComboBox.alignmentX = JComponent.LEFT_ALIGNMENT
			directoryComboBox.alignmentY = JComponent.TOP_ALIGNMENT
			directoryComboBox.maximumRowCount = 8

			topPanel.add(directoryComboBox, BorderLayout.CENTER)

			' Up Button
			Dim upFolderButton As New JButton(changeToParentDirectoryAction)
			upFolderButton.text = Nothing
			upFolderButton.icon = upFolderIcon
			upFolderButton.toolTipText = upFolderToolTipText
			upFolderButton.putClientProperty(AccessibleContext.ACCESSIBLE_NAME_PROPERTY, upFolderAccessibleName)
			upFolderButton.alignmentX = JComponent.LEFT_ALIGNMENT
			upFolderButton.alignmentY = JComponent.CENTER_ALIGNMENT
			upFolderButton.margin = shrinkwrap

			topButtonPanel.add(upFolderButton)
			topButtonPanel.add(Box.createRigidArea(hstrut5))

			' Home Button
			Dim homeDir As File = fsv.homeDirectory
			Dim toolTipText As String = homeFolderToolTipText
			If fsv.isRoot(homeDir) Then toolTipText = getFileView(fc).getName(homeDir) ' Probably "Desktop".




			Dim b As New JButton(homeFolderIcon)
			b.toolTipText = toolTipText
			b.putClientProperty(AccessibleContext.ACCESSIBLE_NAME_PROPERTY, homeFolderAccessibleName)
			b.alignmentX = JComponent.LEFT_ALIGNMENT
			b.alignmentY = JComponent.CENTER_ALIGNMENT
			b.margin = shrinkwrap

			b.addActionListener(goHomeAction)
			topButtonPanel.add(b)
			topButtonPanel.add(Box.createRigidArea(hstrut5))

			' New Directory Button
			If Not UIManager.getBoolean("FileChooser.readOnly") Then
				b = New JButton(filePane.newFolderAction)
				b.text = Nothing
				b.icon = newFolderIcon
				b.toolTipText = newFolderToolTipText
				b.putClientProperty(AccessibleContext.ACCESSIBLE_NAME_PROPERTY, newFolderAccessibleName)
				b.alignmentX = JComponent.LEFT_ALIGNMENT
				b.alignmentY = JComponent.CENTER_ALIGNMENT
				b.margin = shrinkwrap
			End If
			topButtonPanel.add(b)
			topButtonPanel.add(Box.createRigidArea(hstrut5))

			' View button group
			Dim viewButtonGroup As New ButtonGroup

			' List Button
			listViewButton = New JToggleButton(listViewIcon)
			listViewButton.toolTipText = listViewButtonToolTipText
			listViewButton.putClientProperty(AccessibleContext.ACCESSIBLE_NAME_PROPERTY, listViewButtonAccessibleName)
			listViewButton.selected = True
			listViewButton.alignmentX = JComponent.LEFT_ALIGNMENT
			listViewButton.alignmentY = JComponent.CENTER_ALIGNMENT
			listViewButton.margin = shrinkwrap
			listViewButton.addActionListener(filePane.getViewTypeAction(FilePane.VIEWTYPE_LIST))
			topButtonPanel.add(listViewButton)
			viewButtonGroup.add(listViewButton)

			' Details Button
			detailsViewButton = New JToggleButton(detailsViewIcon)
			detailsViewButton.toolTipText = detailsViewButtonToolTipText
			detailsViewButton.putClientProperty(AccessibleContext.ACCESSIBLE_NAME_PROPERTY, detailsViewButtonAccessibleName)
			detailsViewButton.alignmentX = JComponent.LEFT_ALIGNMENT
			detailsViewButton.alignmentY = JComponent.CENTER_ALIGNMENT
			detailsViewButton.margin = shrinkwrap
			detailsViewButton.addActionListener(filePane.getViewTypeAction(FilePane.VIEWTYPE_DETAILS))
			topButtonPanel.add(detailsViewButton)
			viewButtonGroup.add(detailsViewButton)

'JAVA TO VB CONVERTER TODO TASK: Anonymous inner classes are not converted to VB if the base type is not defined in the code being converted:
'			filePane.addPropertyChangeListener(New PropertyChangeListener()
	'		{
	'			public void propertyChange(PropertyChangeEvent e)
	'			{
	'				if ("viewType".equals(e.getPropertyName()))
	'				{
	'					int viewType = filePane.getViewType();
	'					switch (viewType)
	'					{
	'					  case FilePane.VIEWTYPE_LIST:
	'						listViewButton.setSelected(True);
	'						break;
	'
	'					  case FilePane.VIEWTYPE_DETAILS:
	'						detailsViewButton.setSelected(True);
	'						break;
	'					}
	'				}
	'			}
	'		});

			' ************************************** //
			' ******* Add the directory pane ******* //
			' ************************************** //
			fc.add(accessoryPanel, BorderLayout.AFTER_LINE_ENDS)
			Dim accessory As JComponent = fc.accessory
			If accessory IsNot Nothing Then accessoryPanel.add(accessory)
			filePane.preferredSize = LIST_PREF_SIZE
			fc.add(filePane, BorderLayout.CENTER)

			' ********************************** //
			' **** Construct the bottom panel ** //
			' ********************************** //
			Dim ___bottomPanel As JPanel = bottomPanel
			___bottomPanel.layout = New BoxLayout(___bottomPanel, BoxLayout.Y_AXIS)
			fc.add(___bottomPanel, BorderLayout.SOUTH)

			' FileName label and textfield
			Dim fileNamePanel As New JPanel
			fileNamePanel.layout = New BoxLayout(fileNamePanel, BoxLayout.LINE_AXIS)
			___bottomPanel.add(fileNamePanel)
			___bottomPanel.add(Box.createRigidArea(vstrut5))

			fileNameLabel = New AlignedLabel(Me)
			populateFileNameLabel()
			fileNamePanel.add(fileNameLabel)

			fileNameTextField = New JTextFieldAnonymousInnerClassHelper
			fileNamePanel.add(fileNameTextField)
			fileNameLabel.labelFor = fileNameTextField
'JAVA TO VB CONVERTER TODO TASK: Anonymous inner classes are not converted to VB if the base type is not defined in the code being converted:
'			fileNameTextField.addFocusListener(New FocusAdapter()
	'		{
	'				public void focusGained(FocusEvent e)
	'				{
	'					if (!getFileChooser().isMultiSelectionEnabled())
	'					{
	'						filePane.clearSelection();
	'					}
	'				}
	'			}
		   )
			If fc.multiSelectionEnabled Then
				fileName = fileNameString(fc.selectedFiles)
			Else
				fileName = fileNameString(fc.selectedFile)
			End If


			' Filetype label and combobox
			Dim filesOfTypePanel As New JPanel
			filesOfTypePanel.layout = New BoxLayout(filesOfTypePanel, BoxLayout.LINE_AXIS)
			___bottomPanel.add(filesOfTypePanel)

			Dim filesOfTypeLabel As New AlignedLabel(Me, filesOfTypeLabelText)
			filesOfTypeLabel.displayedMnemonic = filesOfTypeLabelMnemonic
			filesOfTypePanel.add(filesOfTypeLabel)

			filterComboBoxModel = createFilterComboBoxModel()
			fc.addPropertyChangeListener(filterComboBoxModel)
			filterComboBox = New JComboBox(filterComboBoxModel)
			filterComboBox.putClientProperty(AccessibleContext.ACCESSIBLE_DESCRIPTION_PROPERTY, filesOfTypeLabelText)
			filesOfTypeLabel.labelFor = filterComboBox
			filterComboBox.renderer = createFilterComboBoxRenderer()
			filesOfTypePanel.add(filterComboBox)

			' buttons
			buttonPanel.layout = New ButtonAreaLayout

			approveButton = New JButton(getApproveButtonText(fc))
			' Note: Metal does not use mnemonics for approve and cancel
			approveButton.addActionListener(approveSelectionAction)
			approveButton.toolTipText = getApproveButtonToolTipText(fc)
			buttonPanel.add(approveButton)

			cancelButton = New JButton(cancelButtonText)
			cancelButton.toolTipText = cancelButtonToolTipText
			cancelButton.addActionListener(cancelSelectionAction)
			buttonPanel.add(cancelButton)

			If fc.controlButtonsAreShown Then addControlButtons()

			groupLabels(New AlignedLabel() { fileNameLabel, filesOfTypeLabel })
		End Sub

		Private Class JComboBoxAnonymousInnerClassHelper(Of E)
			Inherits JComboBox(Of E)

			Public Overridable Property preferredSize As Dimension
				Get
					Dim d As Dimension = MyBase.preferredSize
					' Must be small enough to not affect total width.
					d.width = 150
					Return d
				End Get
			End Property
		End Class

		Private Class JTextFieldAnonymousInnerClassHelper
			Inherits JTextField

			Public Property Overrides maximumSize As Dimension
				Get
					Return New Dimension(Short.MaxValue, MyBase.preferredSize.height)
				End Get
			End Property
		End Class

		Protected Friend Overridable Property buttonPanel As JPanel
			Get
				If buttonPanel Is Nothing Then buttonPanel = New JPanel
				Return buttonPanel
			End Get
		End Property

		Protected Friend Overridable Property bottomPanel As JPanel
			Get
				If bottomPanel Is Nothing Then bottomPanel = New JPanel
				Return bottomPanel
			End Get
		End Property

		Protected Friend Overrides Sub installStrings(ByVal fc As JFileChooser)
			MyBase.installStrings(fc)

			Dim l As Locale = fc.locale

			lookInLabelMnemonic = getMnemonic("FileChooser.lookInLabelMnemonic", l)
			lookInLabelText = UIManager.getString("FileChooser.lookInLabelText",l)
			saveInLabelText = UIManager.getString("FileChooser.saveInLabelText",l)

			fileNameLabelMnemonic = getMnemonic("FileChooser.fileNameLabelMnemonic", l)
			fileNameLabelText = UIManager.getString("FileChooser.fileNameLabelText",l)
			folderNameLabelMnemonic = getMnemonic("FileChooser.folderNameLabelMnemonic", l)
			folderNameLabelText = UIManager.getString("FileChooser.folderNameLabelText",l)

			filesOfTypeLabelMnemonic = getMnemonic("FileChooser.filesOfTypeLabelMnemonic", l)
			filesOfTypeLabelText = UIManager.getString("FileChooser.filesOfTypeLabelText",l)

			upFolderToolTipText = UIManager.getString("FileChooser.upFolderToolTipText",l)
			upFolderAccessibleName = UIManager.getString("FileChooser.upFolderAccessibleName",l)

			homeFolderToolTipText = UIManager.getString("FileChooser.homeFolderToolTipText",l)
			homeFolderAccessibleName = UIManager.getString("FileChooser.homeFolderAccessibleName",l)

			newFolderToolTipText = UIManager.getString("FileChooser.newFolderToolTipText",l)
			newFolderAccessibleName = UIManager.getString("FileChooser.newFolderAccessibleName",l)

			listViewButtonToolTipText = UIManager.getString("FileChooser.listViewButtonToolTipText",l)
			listViewButtonAccessibleName = UIManager.getString("FileChooser.listViewButtonAccessibleName",l)

			detailsViewButtonToolTipText = UIManager.getString("FileChooser.detailsViewButtonToolTipText",l)
			detailsViewButtonAccessibleName = UIManager.getString("FileChooser.detailsViewButtonAccessibleName",l)
		End Sub

		Private Function getMnemonic(ByVal key As String, ByVal l As Locale) As Integer?
			Return SwingUtilities2.getUIDefaultsInt(key, l)
		End Function

		Protected Friend Overrides Sub installListeners(ByVal fc As JFileChooser)
			MyBase.installListeners(fc)
			Dim ___actionMap As ActionMap = actionMap
			SwingUtilities.replaceUIActionMap(fc, ___actionMap)
		End Sub

		Protected Friend Property Overrides actionMap As ActionMap
			Get
				Return createActionMap()
			End Get
		End Property

		Protected Friend Overrides Function createActionMap() As ActionMap
			Dim map As ActionMap = New ActionMapUIResource
			FilePane.addActionsToMap(map, filePane.actions)
			Return map
		End Function

		Protected Friend Overridable Function createList(ByVal fc As JFileChooser) As JPanel
			Return filePane.createList()
		End Function

		Protected Friend Overridable Function createDetailsView(ByVal fc As JFileChooser) As JPanel
			Return filePane.createDetailsView()
		End Function

		''' <summary>
		''' Creates a selection listener for the list of files and directories.
		''' </summary>
		''' <param name="fc"> a <code>JFileChooser</code> </param>
		''' <returns> a <code>ListSelectionListener</code> </returns>
		Public Overrides Function createListSelectionListener(ByVal fc As JFileChooser) As ListSelectionListener
			Return MyBase.createListSelectionListener(fc)
		End Function

		' Obsolete class, not used in this version.
		Protected Friend Class SingleClickListener
			Inherits MouseAdapter

			Private ReadOnly outerInstance As MetalFileChooserUI

			Public Sub New(ByVal outerInstance As MetalFileChooserUI, ByVal list As JList)
					Me.outerInstance = outerInstance
			End Sub
		End Class

		' Obsolete class, not used in this version.
		Protected Friend Class FileRenderer
			Inherits DefaultListCellRenderer

			Private ReadOnly outerInstance As MetalFileChooserUI

			Public Sub New(ByVal outerInstance As MetalFileChooserUI)
				Me.outerInstance = outerInstance
			End Sub

		End Class

		Public Overrides Sub uninstallUI(ByVal c As JComponent)
			' Remove listeners
			c.removePropertyChangeListener(filterComboBoxModel)
			c.removePropertyChangeListener(filePane)
			cancelButton.removeActionListener(cancelSelectionAction)
			approveButton.removeActionListener(approveSelectionAction)
			fileNameTextField.removeActionListener(approveSelectionAction)

			If filePane IsNot Nothing Then
				filePane.uninstallUI()
				filePane = Nothing
			End If

			MyBase.uninstallUI(c)
		End Sub

		''' <summary>
		''' Returns the preferred size of the specified
		''' <code>JFileChooser</code>.
		''' The preferred size is at least as large,
		''' in both height and width,
		''' as the preferred size recommended
		''' by the file chooser's layout manager.
		''' </summary>
		''' <param name="c">  a <code>JFileChooser</code> </param>
		''' <returns>   a <code>Dimension</code> specifying the preferred
		'''           width and height of the file chooser </returns>
		Public Overrides Function getPreferredSize(ByVal c As JComponent) As Dimension
			Dim prefWidth As Integer = PREF_SIZE.width
			Dim d As Dimension = c.layout.preferredLayoutSize(c)
			If d IsNot Nothing Then
				Return New Dimension(If(d.width < prefWidth, prefWidth, d.width),If(d.height < PREF_SIZE.height, PREF_SIZE.height, d.height))
			Else
				Return New Dimension(prefWidth, PREF_SIZE.height)
			End If
		End Function

		''' <summary>
		''' Returns the minimum size of the <code>JFileChooser</code>.
		''' </summary>
		''' <param name="c">  a <code>JFileChooser</code> </param>
		''' <returns>   a <code>Dimension</code> specifying the minimum
		'''           width and height of the file chooser </returns>
		Public Overrides Function getMinimumSize(ByVal c As JComponent) As Dimension
			Return New Dimension(MIN_WIDTH, MIN_HEIGHT)
		End Function

		''' <summary>
		''' Returns the maximum size of the <code>JFileChooser</code>.
		''' </summary>
		''' <param name="c">  a <code>JFileChooser</code> </param>
		''' <returns>   a <code>Dimension</code> specifying the maximum
		'''           width and height of the file chooser </returns>
		Public Overrides Function getMaximumSize(ByVal c As JComponent) As Dimension
			Return New Dimension(Integer.MaxValue, Integer.MaxValue)
		End Function

		Private Function fileNameString(ByVal file As java.io.File) As String
			If file Is Nothing Then
				Return Nothing
			Else
				Dim fc As JFileChooser = fileChooser
				If (fc.directorySelectionEnabled AndAlso (Not fc.fileSelectionEnabled)) OrElse (fc.directorySelectionEnabled AndAlso fc.fileSelectionEnabled AndAlso fc.fileSystemView.isFileSystemRoot(file)) Then
					Return file.path
				Else
					Return file.name
				End If
			End If
		End Function

		Private Function fileNameString(ByVal files As java.io.File()) As String
			Dim buf As New StringBuilder
			Dim i As Integer = 0
			Do While files IsNot Nothing AndAlso i < files.Length
				If i > 0 Then buf.Append(" ")
				If files.Length > 1 Then buf.Append("""")
				buf.Append(fileNameString(files(i)))
				If files.Length > 1 Then buf.Append("""")
				i += 1
			Loop
			Return buf.ToString()
		End Function

		' The following methods are used by the PropertyChange Listener 

		Private Sub doSelectedFileChanged(ByVal e As PropertyChangeEvent)
			Dim f As File = CType(e.newValue, File)
			Dim fc As JFileChooser = fileChooser
			If f IsNot Nothing AndAlso ((fc.fileSelectionEnabled AndAlso (Not f.directory)) OrElse (f.directory AndAlso fc.directorySelectionEnabled)) Then fileName = fileNameString(f)
		End Sub

		Private Sub doSelectedFilesChanged(ByVal e As PropertyChangeEvent)
			Dim files As File() = CType(e.newValue, File())
			Dim fc As JFileChooser = fileChooser
			If files IsNot Nothing AndAlso files.Length > 0 AndAlso (files.Length > 1 OrElse fc.directorySelectionEnabled OrElse (Not files(0).directory)) Then fileName = fileNameString(files)
		End Sub

		Private Sub doDirectoryChanged(ByVal e As PropertyChangeEvent)
			Dim fc As JFileChooser = fileChooser
			Dim fsv As FileSystemView = fc.fileSystemView

			clearIconCache()
			Dim currentDirectory As File = fc.currentDirectory
			If currentDirectory IsNot Nothing Then
				directoryComboBoxModel.addItem(currentDirectory)

				If fc.directorySelectionEnabled AndAlso (Not fc.fileSelectionEnabled) Then
					If fsv.isFileSystem(currentDirectory) Then
						fileName = currentDirectory.path
					Else
						fileName = Nothing
					End If
				End If
			End If
		End Sub

		Private Sub doFilterChanged(ByVal e As PropertyChangeEvent)
			clearIconCache()
		End Sub

		Private Sub doFileSelectionModeChanged(ByVal e As PropertyChangeEvent)
			If fileNameLabel IsNot Nothing Then populateFileNameLabel()
			clearIconCache()

			Dim fc As JFileChooser = fileChooser
			Dim currentDirectory As File = fc.currentDirectory
			If currentDirectory IsNot Nothing AndAlso fc.directorySelectionEnabled AndAlso (Not fc.fileSelectionEnabled) AndAlso fc.fileSystemView.isFileSystem(currentDirectory) Then

				fileName = currentDirectory.path
			Else
				fileName = Nothing
			End If
		End Sub

		Private Sub doAccessoryChanged(ByVal e As PropertyChangeEvent)
			If accessoryPanel IsNot Nothing Then
				If e.oldValue IsNot Nothing Then accessoryPanel.remove(CType(e.oldValue, JComponent))
				Dim accessory As JComponent = CType(e.newValue, JComponent)
				If accessory IsNot Nothing Then accessoryPanel.add(accessory, BorderLayout.CENTER)
			End If
		End Sub

		Private Sub doApproveButtonTextChanged(ByVal e As PropertyChangeEvent)
			Dim chooser As JFileChooser = fileChooser
			approveButton.text = getApproveButtonText(chooser)
			approveButton.toolTipText = getApproveButtonToolTipText(chooser)
		End Sub

		Private Sub doDialogTypeChanged(ByVal e As PropertyChangeEvent)
			Dim chooser As JFileChooser = fileChooser
			approveButton.text = getApproveButtonText(chooser)
			approveButton.toolTipText = getApproveButtonToolTipText(chooser)
			If chooser.dialogType = JFileChooser.SAVE_DIALOG Then
				lookInLabel.text = saveInLabelText
			Else
				lookInLabel.text = lookInLabelText
			End If
		End Sub

		Private Sub doApproveButtonMnemonicChanged(ByVal e As PropertyChangeEvent)
			' Note: Metal does not use mnemonics for approve and cancel
		End Sub

		Private Sub doControlButtonsChanged(ByVal e As PropertyChangeEvent)
			If fileChooser.controlButtonsAreShown Then
				addControlButtons()
			Else
				removeControlButtons()
			End If
		End Sub

	'    
	'     * Listen for filechooser property changes, such as
	'     * the selected file changing, or the type of the dialog changing.
	'     
		Public Overrides Function createPropertyChangeListener(ByVal fc As JFileChooser) As PropertyChangeListener
'JAVA TO VB CONVERTER TODO TASK: Anonymous inner classes are not converted to VB if the base type is not defined in the code being converted:
'			Return New PropertyChangeListener()
	'		{
	'			public void propertyChange(PropertyChangeEvent e)
	'			{
	'				String s = e.getPropertyName();
	'				if(s.equals(JFileChooser.SELECTED_FILE_CHANGED_PROPERTY))
	'				{
	'					doSelectedFileChanged(e);
	'				}
	'				else if (s.equals(JFileChooser.SELECTED_FILES_CHANGED_PROPERTY))
	'				{
	'					doSelectedFilesChanged(e);
	'				}
	'				else if(s.equals(JFileChooser.DIRECTORY_CHANGED_PROPERTY))
	'				{
	'					doDirectoryChanged(e);
	'				}
	'				else if(s.equals(JFileChooser.FILE_FILTER_CHANGED_PROPERTY))
	'				{
	'					doFilterChanged(e);
	'				}
	'				else if(s.equals(JFileChooser.FILE_SELECTION_MODE_CHANGED_PROPERTY))
	'				{
	'					doFileSelectionModeChanged(e);
	'				}
	'				else if(s.equals(JFileChooser.ACCESSORY_CHANGED_PROPERTY))
	'				{
	'					doAccessoryChanged(e);
	'				}
	'				else if (s.equals(JFileChooser.APPROVE_BUTTON_TEXT_CHANGED_PROPERTY) || s.equals(JFileChooser.APPROVE_BUTTON_TOOL_TIP_TEXT_CHANGED_PROPERTY))
	'				{
	'					doApproveButtonTextChanged(e);
	'				}
	'				else if(s.equals(JFileChooser.DIALOG_TYPE_CHANGED_PROPERTY))
	'				{
	'					doDialogTypeChanged(e);
	'				}
	'				else if(s.equals(JFileChooser.APPROVE_BUTTON_MNEMONIC_CHANGED_PROPERTY))
	'				{
	'					doApproveButtonMnemonicChanged(e);
	'				}
	'				else if(s.equals(JFileChooser.CONTROL_BUTTONS_ARE_SHOWN_CHANGED_PROPERTY))
	'				{
	'					doControlButtonsChanged(e);
	'				}
	'				else if (s.equals("componentOrientation"))
	'				{
	'					ComponentOrientation o = (ComponentOrientation)e.getNewValue();
	'					JFileChooser cc = (JFileChooser)e.getSource();
	'					if (o != e.getOldValue())
	'					{
	'						cc.applyComponentOrientation(o);
	'					}
	'				}
	'				else if (s == "FileChooser.useShellFolder")
	'				{
	'					doDirectoryChanged(e);
	'				}
	'				else if (s.equals("ancestor"))
	'				{
	'					if (e.getOldValue() == Nothing && e.getNewValue() != Nothing)
	'					{
	'						' Ancestor was added, set initial focus
	'						fileNameTextField.selectAll();
	'						fileNameTextField.requestFocus();
	'					}
	'				}
	'			}
	'		};
		End Function


		Protected Friend Overridable Sub removeControlButtons()
			bottomPanel.remove(buttonPanel)
		End Sub

		Protected Friend Overridable Sub addControlButtons()
			bottomPanel.add(buttonPanel)
		End Sub

		Public Overridable Sub ensureFileIsVisible(ByVal fc As JFileChooser, ByVal f As java.io.File)
			filePane.ensureFileIsVisible(fc, f)
		End Sub

		Public Overrides Sub rescanCurrentDirectory(ByVal fc As JFileChooser)
			filePane.rescanCurrentDirectory()
		End Sub

		Public Property Overrides fileName As String
			Get
				If fileNameTextField IsNot Nothing Then
					Return fileNameTextField.text
				Else
					Return Nothing
				End If
			End Get
			Set(ByVal filename As String)
				If fileNameTextField IsNot Nothing Then fileNameTextField.text = filename
			End Set
		End Property


		''' <summary>
		''' Property to remember whether a directory is currently selected in the UI.
		''' This is normally called by the UI on a selection event.
		''' </summary>
		''' <param name="directorySelected"> if a directory is currently selected.
		''' @since 1.4 </param>
		Protected Friend Overrides Property directorySelected As Boolean
			Set(ByVal directorySelected As Boolean)
				MyBase.directorySelected = directorySelected
				Dim chooser As JFileChooser = fileChooser
				If directorySelected Then
					If approveButton IsNot Nothing Then
						approveButton.text = directoryOpenButtonText
						approveButton.toolTipText = directoryOpenButtonToolTipText
					End If
				Else
					If approveButton IsNot Nothing Then
						approveButton.text = getApproveButtonText(chooser)
						approveButton.toolTipText = getApproveButtonToolTipText(chooser)
					End If
				End If
			End Set
		End Property

		Public Property Overrides directoryName As String
			Get
				' PENDING(jeff) - get the name from the directory combobox
				Return Nothing
			End Get
			Set(ByVal dirname As String)
				' PENDING(jeff) - set the name in the directory combobox
			End Set
		End Property


		Protected Friend Overridable Function createDirectoryComboBoxRenderer(ByVal fc As JFileChooser) As DirectoryComboBoxRenderer
			Return New DirectoryComboBoxRenderer(Me)
		End Function

		'
		' Renderer for DirectoryComboBox
		'
		Friend Class DirectoryComboBoxRenderer
			Inherits DefaultListCellRenderer

			Private ReadOnly outerInstance As MetalFileChooserUI

			Public Sub New(ByVal outerInstance As MetalFileChooserUI)
				Me.outerInstance = outerInstance
			End Sub

			Friend ii As New IndentIcon
			Public Overrides Function getListCellRendererComponent(ByVal list As JList, ByVal value As Object, ByVal index As Integer, ByVal isSelected As Boolean, ByVal cellHasFocus As Boolean) As Component

				MyBase.getListCellRendererComponent(list, value, index, isSelected, cellHasFocus)

				If value Is Nothing Then
					text = ""
					Return Me
				End If
				Dim directory As File = CType(value, File)
				text = outerInstance.fileChooser.getName(directory)
				Dim ___icon As Icon = outerInstance.fileChooser.getIcon(directory)
				ii.icon = ___icon
				ii.depth = outerInstance.directoryComboBoxModel.getDepth(index)
				icon = ii

				Return Me
			End Function
		End Class

		Friend Const space As Integer = 10
		Friend Class IndentIcon
			Implements Icon

			Private ReadOnly outerInstance As MetalFileChooserUI

			Public Sub New(ByVal outerInstance As MetalFileChooserUI)
				Me.outerInstance = outerInstance
			End Sub


			Friend icon As Icon = Nothing
			Friend depth As Integer = 0

			Public Overridable Sub paintIcon(ByVal c As Component, ByVal g As Graphics, ByVal x As Integer, ByVal y As Integer)
				If c.componentOrientation.leftToRight Then
					icon.paintIcon(c, g, x+depth*space, y)
				Else
					icon.paintIcon(c, g, x, y)
				End If
			End Sub

			Public Overridable Property iconWidth As Integer Implements Icon.getIconWidth
				Get
					Return icon.iconWidth + depth*space
				End Get
			End Property

			Public Overridable Property iconHeight As Integer Implements Icon.getIconHeight
				Get
					Return icon.iconHeight
				End Get
			End Property

		End Class

		'
		' DataModel for DirectoryComboxbox
		'
		Protected Friend Overridable Function createDirectoryComboBoxModel(ByVal fc As JFileChooser) As DirectoryComboBoxModel
			Return New DirectoryComboBoxModel(Me)
		End Function

		''' <summary>
		''' Data model for a type-face selection combo-box.
		''' </summary>
		Protected Friend Class DirectoryComboBoxModel
			Inherits AbstractListModel(Of Object)
			Implements ComboBoxModel(Of Object)

			Private ReadOnly outerInstance As MetalFileChooserUI

			Friend directories As New List(Of java.io.File)
			Friend depths As Integer() = Nothing
			Friend selectedDirectory As java.io.File = Nothing
			Friend chooser As JFileChooser = outerInstance.fileChooser
			Friend fsv As FileSystemView = chooser.fileSystemView

			Public Sub New(ByVal outerInstance As MetalFileChooserUI)
					Me.outerInstance = outerInstance
				' Add the current directory to the model, and make it the
				' selectedDirectory
				Dim dir As File = outerInstance.fileChooser.currentDirectory
				If dir IsNot Nothing Then addItem(dir)
			End Sub

			''' <summary>
			''' Adds the directory to the model and sets it to be selected,
			''' additionally clears out the previous selected directory and
			''' the paths leading up to it, if any.
			''' </summary>
			Private Sub addItem(ByVal directory As java.io.File)

				If directory Is Nothing Then Return

				Dim useShellFolder As Boolean = FilePane.usesShellFolder(chooser)

				directories.Clear()

				Dim baseFolders As File() = If(useShellFolder, CType(sun.awt.shell.ShellFolder.get("fileChooserComboBoxFolders"), File()), fsv.roots)
				directories.AddRange(baseFolders)

				' Get the canonical (full) path. This has the side
				' benefit of removing extraneous chars from the path,
				' for example /foo/bar/ becomes /foo/bar
				Dim canonical As File
				Try
					canonical = sun.awt.shell.ShellFolder.getNormalizedFile(directory)
				Catch e As java.io.IOException
					' Maybe drive is not ready. Can't abort here.
					canonical = directory
				End Try

				' create File instances of each directory leading up to the top
				Try
					Dim sf As File = If(useShellFolder, sun.awt.shell.ShellFolder.getShellFolder(canonical), canonical)
					Dim f As File = sf
					Dim path As New List(Of File)(10)
					Do
						path.Add(f)
						f = f.parentFile
					Loop While f IsNot Nothing

					Dim pathCount As Integer = path.Count
					' Insert chain at appropriate place in vector
					For i As Integer = 0 To pathCount - 1
						f = path(i)
						If directories.Contains(f) Then
							Dim topIndex As Integer = directories.IndexOf(f)
							For j As Integer = i-1 To 0 Step -1
								directories.Insert(topIndex+i-j, path(j))
							Next j
							Exit For
						End If
					Next i
					calculateDepths()
					selectedItem = sf
				Catch ex As java.io.FileNotFoundException
					calculateDepths()
				End Try
			End Sub

			Private Sub calculateDepths()
				depths = New Integer(directories.Count - 1){}
				For i As Integer = 0 To depths.Length - 1
					Dim dir As File = directories(i)
					Dim parent As File = dir.parentFile
					depths(i) = 0
					If parent IsNot Nothing Then
						For j As Integer = i-1 To 0 Step -1
							If parent.Equals(directories(j)) Then
								depths(i) = depths(j) + 1
								Exit For
							End If
						Next j
					End If
				Next i
			End Sub

			Public Overridable Function getDepth(ByVal i As Integer) As Integer
				Return If(depths IsNot Nothing AndAlso i >= 0 AndAlso i < depths.Length, depths(i), 0)
			End Function

			Public Overridable Property selectedItem As Object
				Set(ByVal selectedDirectory As Object)
					Me.selectedDirectory = CType(selectedDirectory, File)
					fireContentsChanged(Me, -1, -1)
				End Set
				Get
					Return selectedDirectory
				End Get
			End Property


			Public Overridable Property size As Integer
				Get
					Return directories.Count
				End Get
			End Property

			Public Overridable Function getElementAt(ByVal index As Integer) As Object
				Return directories(index)
			End Function
		End Class

		'
		' Renderer for Types ComboBox
		'
		Protected Friend Overridable Function createFilterComboBoxRenderer() As FilterComboBoxRenderer
			Return New FilterComboBoxRenderer(Me)
		End Function

		''' <summary>
		''' Render different type sizes and styles.
		''' </summary>
		Public Class FilterComboBoxRenderer
			Inherits DefaultListCellRenderer

			Private ReadOnly outerInstance As MetalFileChooserUI

			Public Sub New(ByVal outerInstance As MetalFileChooserUI)
				Me.outerInstance = outerInstance
			End Sub

			Public Overrides Function getListCellRendererComponent(ByVal list As JList, ByVal value As Object, ByVal index As Integer, ByVal isSelected As Boolean, ByVal cellHasFocus As Boolean) As Component

				MyBase.getListCellRendererComponent(list, value, index, isSelected, cellHasFocus)

				If value IsNot Nothing AndAlso TypeOf value Is FileFilter Then text = CType(value, FileFilter).description

				Return Me
			End Function
		End Class

		'
		' DataModel for Types Comboxbox
		'
		Protected Friend Overridable Function createFilterComboBoxModel() As FilterComboBoxModel
			Return New FilterComboBoxModel(Me)
		End Function

		''' <summary>
		''' Data model for a type-face selection combo-box.
		''' </summary>
		Protected Friend Class FilterComboBoxModel
			Inherits AbstractListModel(Of Object)
			Implements ComboBoxModel(Of Object), PropertyChangeListener

			Private ReadOnly outerInstance As MetalFileChooserUI

			Protected Friend filters As FileFilter()
			Protected Friend Sub New(ByVal outerInstance As MetalFileChooserUI)
					Me.outerInstance = outerInstance
				MyBase.New()
				filters = outerInstance.fileChooser.choosableFileFilters
			End Sub

			Public Overridable Sub propertyChange(ByVal e As PropertyChangeEvent)
				Dim prop As String = e.propertyName
				If prop = JFileChooser.CHOOSABLE_FILE_FILTER_CHANGED_PROPERTY Then
					filters = CType(e.newValue, FileFilter())
					fireContentsChanged(Me, -1, -1)
				ElseIf prop = JFileChooser.FILE_FILTER_CHANGED_PROPERTY Then
					fireContentsChanged(Me, -1, -1)
				End If
			End Sub

			Public Overridable Property selectedItem As Object
				Set(ByVal filter As Object)
					If filter IsNot Nothing Then
						outerInstance.fileChooser.fileFilter = CType(filter, FileFilter)
						fireContentsChanged(Me, -1, -1)
					End If
				End Set
				Get
					' Ensure that the current filter is in the list.
					' NOTE: we shouldnt' have to do this, since JFileChooser adds
					' the filter to the choosable filters list when the filter
					' is set. Lets be paranoid just in case someone overrides
					' setFileFilter in JFileChooser.
					Dim currentFilter As FileFilter = outerInstance.fileChooser.fileFilter
					Dim found As Boolean = False
					If currentFilter IsNot Nothing Then
						For Each filter As FileFilter In filters
							If filter Is currentFilter Then found = True
						Next filter
						If found = False Then outerInstance.fileChooser.addChoosableFileFilter(currentFilter)
					End If
					Return outerInstance.fileChooser.fileFilter
				End Get
			End Property


			Public Overridable Property size As Integer
				Get
					If filters IsNot Nothing Then
						Return filters.Length
					Else
						Return 0
					End If
				End Get
			End Property

			Public Overridable Function getElementAt(ByVal index As Integer) As Object
				If index > size - 1 Then Return outerInstance.fileChooser.fileFilter
				If filters IsNot Nothing Then
					Return filters(index)
				Else
					Return Nothing
				End If
			End Function
		End Class

		Public Overridable Sub valueChanged(ByVal e As ListSelectionEvent)
			Dim fc As JFileChooser = fileChooser
			Dim f As File = fc.selectedFile
			If (Not e.valueIsAdjusting) AndAlso f IsNot Nothing AndAlso (Not fileChooser.isTraversable(f)) Then fileName = fileNameString(f)
		End Sub

		''' <summary>
		''' Acts when DirectoryComboBox has changed the selected item.
		''' </summary>
		Protected Friend Class DirectoryComboBoxAction
			Inherits AbstractAction

			Private ReadOnly outerInstance As MetalFileChooserUI

			Protected Friend Sub New(ByVal outerInstance As MetalFileChooserUI)
					Me.outerInstance = outerInstance
				MyBase.New("DirectoryComboBoxAction")
			End Sub

			Public Overridable Sub actionPerformed(ByVal e As ActionEvent)
				outerInstance.directoryComboBox.hidePopup()
				Dim f As File = CType(outerInstance.directoryComboBox.selectedItem, File)
				If Not outerInstance.fileChooser.currentDirectory.Equals(f) Then outerInstance.fileChooser.currentDirectory = f
			End Sub
		End Class

		Protected Friend Overrides Function getApproveButton(ByVal fc As JFileChooser) As JButton
			Return approveButton
		End Function


		''' <summary>
		''' <code>ButtonAreaLayout</code> behaves in a similar manner to
		''' <code>FlowLayout</code>. It lays out all components from left to
		''' right, flushed right. The widths of all components will be set
		''' to the largest preferred size width.
		''' </summary>
		Private Class ButtonAreaLayout
			Implements LayoutManager

			Private hGap As Integer = 5
			Private topMargin As Integer = 17

			Public Overridable Sub addLayoutComponent(ByVal [string] As String, ByVal comp As Component)
			End Sub

			Public Overridable Sub layoutContainer(ByVal container As Container)
				Dim children As Component() = container.components

				If children IsNot Nothing AndAlso children.Length > 0 Then
					Dim numChildren As Integer = children.Length
					Dim sizes As Dimension() = New Dimension(numChildren - 1){}
					Dim insets As Insets = container.insets
					Dim yLocation As Integer = insets.top + topMargin
					Dim maxWidth As Integer = 0

					For counter As Integer = 0 To numChildren - 1
						sizes(counter) = children(counter).preferredSize
						maxWidth = Math.Max(maxWidth, sizes(counter).width)
					Next counter
					Dim xLocation, xOffset As Integer
					If container.componentOrientation.leftToRight Then
						xLocation = container.size.width - insets.left - maxWidth
						xOffset = hGap + maxWidth
					Else
						xLocation = insets.left
						xOffset = -(hGap + maxWidth)
					End If
					For counter As Integer = numChildren - 1 To 0 Step -1
						children(counter).boundsnds(xLocation, yLocation, maxWidth, sizes(counter).height)
						xLocation -= xOffset
					Next counter
				End If
			End Sub

			Public Overridable Function minimumLayoutSize(ByVal c As Container) As Dimension
				If c IsNot Nothing Then
					Dim children As Component() = c.components

					If children IsNot Nothing AndAlso children.Length > 0 Then
						Dim numChildren As Integer = children.Length
						Dim height As Integer = 0
						Dim cInsets As Insets = c.insets
						Dim extraHeight As Integer = topMargin + cInsets.top + cInsets.bottom
						Dim extraWidth As Integer = cInsets.left + cInsets.right
						Dim maxWidth As Integer = 0

						For counter As Integer = 0 To numChildren - 1
							Dim aSize As Dimension = children(counter).preferredSize
							height = Math.Max(height, aSize.height)
							maxWidth = Math.Max(maxWidth, aSize.width)
						Next counter
						Return New Dimension(extraWidth + numChildren * maxWidth + (numChildren - 1) * hGap, extraHeight + height)
					End If
				End If
				Return New Dimension(0, 0)
			End Function

			Public Overridable Function preferredLayoutSize(ByVal c As Container) As Dimension
				Return minimumLayoutSize(c)
			End Function

			Public Overridable Sub removeLayoutComponent(ByVal c As Component)
			End Sub
		End Class

		Private Shared Sub groupLabels(ByVal group As AlignedLabel())
			For i As Integer = 0 To group.Length - 1
				group(i).group = group
			Next i
		End Sub

		Private Class AlignedLabel
			Inherits JLabel

			Private ReadOnly outerInstance As MetalFileChooserUI

			Private group As AlignedLabel()
			Private maxWidth As Integer = 0

			Friend Sub New(ByVal outerInstance As MetalFileChooserUI)
					Me.outerInstance = outerInstance
				MyBase.New()
				alignmentX = JComponent.LEFT_ALIGNMENT
			End Sub


			Friend Sub New(ByVal outerInstance As MetalFileChooserUI, ByVal text As String)
					Me.outerInstance = outerInstance
				MyBase.New(text)
				alignmentX = JComponent.LEFT_ALIGNMENT
			End Sub

			Public Property Overrides preferredSize As Dimension
				Get
					Dim d As Dimension = MyBase.preferredSize
					' Align the width with all other labels in group.
					Return New Dimension(maxWidth + 11, d.height)
				End Get
			End Property

			Private Property maxWidth As Integer
				Get
					If maxWidth = 0 AndAlso group IsNot Nothing Then
						Dim max As Integer = 0
						For i As Integer = 0 To group.Length - 1
							max = Math.Max(group(i).superPreferredWidth, max)
						Next i
						For i As Integer = 0 To group.Length - 1
							group(i).maxWidth = max
						Next i
					End If
					Return maxWidth
				End Get
			End Property

			Private Property superPreferredWidth As Integer
				Get
					Return MyBase.preferredSize.width
				End Get
			End Property
		End Class
	End Class

End Namespace