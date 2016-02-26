Imports System
Imports System.Collections.Generic
Imports javax.swing
Imports javax.swing.filechooser
Imports javax.swing.event
Imports javax.swing.plaf
Imports sun.swing

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

Namespace javax.swing.plaf.basic


	''' <summary>
	''' Basic L&amp;F implementation of a FileChooser.
	''' 
	''' @author Jeff Dinkins
	''' </summary>
	Public Class BasicFileChooserUI
		Inherits FileChooserUI

		' FileView icons 
		Protected Friend directoryIcon As Icon = Nothing
		Protected Friend fileIcon As Icon = Nothing
		Protected Friend computerIcon As Icon = Nothing
		Protected Friend hardDriveIcon As Icon = Nothing
		Protected Friend floppyDriveIcon As Icon = Nothing

		Protected Friend newFolderIcon As Icon = Nothing
		Protected Friend upFolderIcon As Icon = Nothing
		Protected Friend homeFolderIcon As Icon = Nothing
		Protected Friend listViewIcon As Icon = Nothing
		Protected Friend detailsViewIcon As Icon = Nothing
		Protected Friend viewMenuIcon As Icon = Nothing

		Protected Friend saveButtonMnemonic As Integer = 0
		Protected Friend openButtonMnemonic As Integer = 0
		Protected Friend cancelButtonMnemonic As Integer = 0
		Protected Friend updateButtonMnemonic As Integer = 0
		Protected Friend helpButtonMnemonic As Integer = 0

		''' <summary>
		''' The mnemonic keycode used for the approve button when a directory
		''' is selected and the current selection mode is FILES_ONLY.
		''' 
		''' @since 1.4
		''' </summary>
		Protected Friend directoryOpenButtonMnemonic As Integer = 0

		Protected Friend saveButtonText As String = Nothing
		Protected Friend openButtonText As String = Nothing
		Protected Friend cancelButtonText As String = Nothing
		Protected Friend updateButtonText As String = Nothing
		Protected Friend helpButtonText As String = Nothing

		''' <summary>
		''' The label text displayed on the approve button when a directory
		''' is selected and the current selection mode is FILES_ONLY.
		''' 
		''' @since 1.4
		''' </summary>
		Protected Friend directoryOpenButtonText As String = Nothing

		Private openDialogTitleText As String = Nothing
		Private saveDialogTitleText As String = Nothing

		Protected Friend saveButtonToolTipText As String = Nothing
		Protected Friend openButtonToolTipText As String = Nothing
		Protected Friend cancelButtonToolTipText As String = Nothing
		Protected Friend updateButtonToolTipText As String = Nothing
		Protected Friend helpButtonToolTipText As String = Nothing

		''' <summary>
		''' The tooltip text displayed on the approve button when a directory
		''' is selected and the current selection mode is FILES_ONLY.
		''' 
		''' @since 1.4
		''' </summary>
		Protected Friend directoryOpenButtonToolTipText As String = Nothing

		' Some generic FileChooser functions
		Private approveSelectionAction As Action = New ApproveSelectionAction(Me)
		Private cancelSelectionAction As Action = New CancelSelectionAction
		Private updateAction As Action = New UpdateAction
		Private newFolderAction As Action
		Private goHomeAction As Action = New GoHomeAction(Me)
		Private changeToParentDirectoryAction As Action = New ChangeToParentDirectoryAction(Me)

		Private newFolderErrorSeparator As String = Nothing
		Private newFolderErrorText As String = Nothing
		Private newFolderParentDoesntExistTitleText As String = Nothing
		Private newFolderParentDoesntExistText As String = Nothing
		Private fileDescriptionText As String = Nothing
		Private directoryDescriptionText As String = Nothing

		Private filechooser As JFileChooser = Nothing

		Private directorySelected As Boolean = False
		Private directory As File = Nothing

		Private propertyChangeListener As PropertyChangeListener = Nothing
		Private acceptAllFileFilter As New AcceptAllFileFilter
		Private actualFileFilter As javax.swing.filechooser.FileFilter = Nothing
		Private globFilter As GlobFilter = Nothing
		Private model As BasicDirectoryModel = Nothing
		Private fileView As New BasicFileView
		Private usesSingleFilePane As Boolean
		Private [readOnly] As Boolean

		' The accessoryPanel is a container to place the JFileChooser accessory component
		Private accessoryPanel As JPanel = Nothing
		Private handler As Handler

		''' <summary>
		''' Creates a {@code BasicFileChooserUI} implementation
		''' for the specified component. By default
		''' the {@code BasicLookAndFeel} class uses
		''' {@code createUI} methods of all basic UIs classes
		''' to instantiate UIs.
		''' </summary>
		''' <param name="c"> the {@code JFileChooser} which needs a UI </param>
		''' <returns> the {@code BasicFileChooserUI} object
		''' </returns>
		''' <seealso cref= UIDefaults#getUI(JComponent)
		''' @since 1.7 </seealso>
		Public Shared Function createUI(ByVal c As JComponent) As ComponentUI
			Return New BasicFileChooserUI(CType(c, JFileChooser))
		End Function

		Public Sub New(ByVal b As JFileChooser)
		End Sub

		Public Overridable Sub installUI(ByVal c As JComponent)
			accessoryPanel = New JPanel(New BorderLayout)
			filechooser = CType(c, JFileChooser)

			createModel()

			clearIconCache()

			installDefaults(filechooser)
			installComponents(filechooser)
			installListeners(filechooser)
			filechooser.applyComponentOrientation(filechooser.componentOrientation)
		End Sub

		Public Overridable Sub uninstallUI(ByVal c As JComponent)
			uninstallListeners(filechooser)
			uninstallComponents(filechooser)
			uninstallDefaults(filechooser)

			If accessoryPanel IsNot Nothing Then accessoryPanel.removeAll()

			accessoryPanel = Nothing
			fileChooser.removeAll()

			handler = Nothing
		End Sub

		Public Overridable Sub installComponents(ByVal fc As JFileChooser)
		End Sub

		Public Overridable Sub uninstallComponents(ByVal fc As JFileChooser)
		End Sub

		Protected Friend Overridable Sub installListeners(ByVal fc As JFileChooser)
			propertyChangeListener = createPropertyChangeListener(fc)
			If propertyChangeListener IsNot Nothing Then fc.addPropertyChangeListener(propertyChangeListener)
			fc.addPropertyChangeListener(model)

			Dim ___inputMap As InputMap = getInputMap(JComponent.WHEN_ANCESTOR_OF_FOCUSED_COMPONENT)
			SwingUtilities.replaceUIInputMap(fc, JComponent.WHEN_ANCESTOR_OF_FOCUSED_COMPONENT, ___inputMap)
			Dim ___actionMap As ActionMap = actionMap
			SwingUtilities.replaceUIActionMap(fc, ___actionMap)
		End Sub

		Friend Overridable Function getInputMap(ByVal condition As Integer) As InputMap
			If condition = JComponent.WHEN_ANCESTOR_OF_FOCUSED_COMPONENT Then Return CType(DefaultLookup.get(fileChooser, Me, "FileChooser.ancestorInputMap"), InputMap)
			Return Nothing
		End Function

		Friend Overridable Property actionMap As ActionMap
			Get
				Return createActionMap()
			End Get
		End Property

		Friend Overridable Function createActionMap() As ActionMap
			Dim map As ActionMap = New ActionMapUIResource

'JAVA TO VB CONVERTER TODO TASK: Anonymous inner classes are not converted to VB if the base type is not defined in the code being converted:
'			Action refreshAction = New UIAction(FilePane.ACTION_REFRESH)
	'		{
	'			public void actionPerformed(ActionEvent evt)
	'			{
	'				getFileChooser().rescanCurrentDirectory();
	'			}
	'		};

			map.put(FilePane.ACTION_APPROVE_SELECTION, approveSelectionAction)
			map.put(FilePane.ACTION_CANCEL, cancelSelectionAction)
			map.put(FilePane.ACTION_REFRESH, refreshAction)
			map.put(FilePane.ACTION_CHANGE_TO_PARENT_DIRECTORY, changeToParentDirectoryAction)
			Return map
		End Function


		Protected Friend Overridable Sub uninstallListeners(ByVal fc As JFileChooser)
			If propertyChangeListener IsNot Nothing Then fc.removePropertyChangeListener(propertyChangeListener)
			fc.removePropertyChangeListener(model)
			SwingUtilities.replaceUIInputMap(fc, JComponent.WHEN_ANCESTOR_OF_FOCUSED_COMPONENT, Nothing)
			SwingUtilities.replaceUIActionMap(fc, Nothing)
		End Sub


		Protected Friend Overridable Sub installDefaults(ByVal fc As JFileChooser)
			installIcons(fc)
			installStrings(fc)
			usesSingleFilePane = UIManager.getBoolean("FileChooser.usesSingleFilePane")
			[readOnly] = UIManager.getBoolean("FileChooser.readOnly")
			Dim th As TransferHandler = fc.transferHandler
			If th Is Nothing OrElse TypeOf th Is UIResource Then fc.transferHandler = defaultTransferHandler
			LookAndFeel.installProperty(fc, "opaque", Boolean.FALSE)
		End Sub

		Protected Friend Overridable Sub installIcons(ByVal fc As JFileChooser)
			directoryIcon = UIManager.getIcon("FileView.directoryIcon")
			fileIcon = UIManager.getIcon("FileView.fileIcon")
			computerIcon = UIManager.getIcon("FileView.computerIcon")
			hardDriveIcon = UIManager.getIcon("FileView.hardDriveIcon")
			floppyDriveIcon = UIManager.getIcon("FileView.floppyDriveIcon")

			newFolderIcon = UIManager.getIcon("FileChooser.newFolderIcon")
			upFolderIcon = UIManager.getIcon("FileChooser.upFolderIcon")
			homeFolderIcon = UIManager.getIcon("FileChooser.homeFolderIcon")
			detailsViewIcon = UIManager.getIcon("FileChooser.detailsViewIcon")
			listViewIcon = UIManager.getIcon("FileChooser.listViewIcon")
			viewMenuIcon = UIManager.getIcon("FileChooser.viewMenuIcon")
		End Sub

		Protected Friend Overridable Sub installStrings(ByVal fc As JFileChooser)

			Dim l As Locale = fc.locale
			newFolderErrorText = UIManager.getString("FileChooser.newFolderErrorText",l)
			newFolderErrorSeparator = UIManager.getString("FileChooser.newFolderErrorSeparator",l)

			newFolderParentDoesntExistTitleText = UIManager.getString("FileChooser.newFolderParentDoesntExistTitleText", l)
			newFolderParentDoesntExistText = UIManager.getString("FileChooser.newFolderParentDoesntExistText", l)

			fileDescriptionText = UIManager.getString("FileChooser.fileDescriptionText",l)
			directoryDescriptionText = UIManager.getString("FileChooser.directoryDescriptionText",l)

			saveButtonText = UIManager.getString("FileChooser.saveButtonText",l)
			openButtonText = UIManager.getString("FileChooser.openButtonText",l)
			saveDialogTitleText = UIManager.getString("FileChooser.saveDialogTitleText",l)
			openDialogTitleText = UIManager.getString("FileChooser.openDialogTitleText",l)
			cancelButtonText = UIManager.getString("FileChooser.cancelButtonText",l)
			updateButtonText = UIManager.getString("FileChooser.updateButtonText",l)
			helpButtonText = UIManager.getString("FileChooser.helpButtonText",l)
			directoryOpenButtonText = UIManager.getString("FileChooser.directoryOpenButtonText",l)

			saveButtonMnemonic = getMnemonic("FileChooser.saveButtonMnemonic", l)
			openButtonMnemonic = getMnemonic("FileChooser.openButtonMnemonic", l)
			cancelButtonMnemonic = getMnemonic("FileChooser.cancelButtonMnemonic", l)
			updateButtonMnemonic = getMnemonic("FileChooser.updateButtonMnemonic", l)
			helpButtonMnemonic = getMnemonic("FileChooser.helpButtonMnemonic", l)
			directoryOpenButtonMnemonic = getMnemonic("FileChooser.directoryOpenButtonMnemonic", l)

			saveButtonToolTipText = UIManager.getString("FileChooser.saveButtonToolTipText",l)
			openButtonToolTipText = UIManager.getString("FileChooser.openButtonToolTipText",l)
			cancelButtonToolTipText = UIManager.getString("FileChooser.cancelButtonToolTipText",l)
			updateButtonToolTipText = UIManager.getString("FileChooser.updateButtonToolTipText",l)
			helpButtonToolTipText = UIManager.getString("FileChooser.helpButtonToolTipText",l)
			directoryOpenButtonToolTipText = UIManager.getString("FileChooser.directoryOpenButtonToolTipText",l)
		End Sub

		Protected Friend Overridable Sub uninstallDefaults(ByVal fc As JFileChooser)
			uninstallIcons(fc)
			uninstallStrings(fc)
			If TypeOf fc.transferHandler Is UIResource Then fc.transferHandler = Nothing
		End Sub

		Protected Friend Overridable Sub uninstallIcons(ByVal fc As JFileChooser)
			directoryIcon = Nothing
			fileIcon = Nothing
			computerIcon = Nothing
			hardDriveIcon = Nothing
			floppyDriveIcon = Nothing

			newFolderIcon = Nothing
			upFolderIcon = Nothing
			homeFolderIcon = Nothing
			detailsViewIcon = Nothing
			listViewIcon = Nothing
			viewMenuIcon = Nothing
		End Sub

		Protected Friend Overridable Sub uninstallStrings(ByVal fc As JFileChooser)
			saveButtonText = Nothing
			openButtonText = Nothing
			cancelButtonText = Nothing
			updateButtonText = Nothing
			helpButtonText = Nothing
			directoryOpenButtonText = Nothing

			saveButtonToolTipText = Nothing
			openButtonToolTipText = Nothing
			cancelButtonToolTipText = Nothing
			updateButtonToolTipText = Nothing
			helpButtonToolTipText = Nothing
			directoryOpenButtonToolTipText = Nothing
		End Sub

		Protected Friend Overridable Sub createModel()
			If model IsNot Nothing Then model.invalidateFileCache()
			model = New BasicDirectoryModel(fileChooser)
		End Sub

		Public Overridable Property model As BasicDirectoryModel
			Get
				Return model
			End Get
		End Property

		Public Overridable Function createPropertyChangeListener(ByVal fc As JFileChooser) As PropertyChangeListener
			Return Nothing
		End Function

		Public Overridable Property fileName As String
			Get
				Return Nothing
			End Get
			Set(ByVal filename As String)
			End Set
		End Property

		Public Overridable Property directoryName As String
			Get
				Return Nothing
			End Get
			Set(ByVal dirname As String)
			End Set
		End Property



		Public Overrides Sub rescanCurrentDirectory(ByVal fc As JFileChooser)
		End Sub

		Public Overridable Sub ensureFileIsVisible(ByVal fc As JFileChooser, ByVal f As File)
		End Sub

		Public Overridable Property fileChooser As JFileChooser
			Get
				Return filechooser
			End Get
		End Property

		Public Overridable Property accessoryPanel As JPanel
			Get
				Return accessoryPanel
			End Get
		End Property

		Protected Friend Overridable Function getApproveButton(ByVal fc As JFileChooser) As JButton
			Return Nothing
		End Function

		Public Overrides Function getDefaultButton(ByVal fc As JFileChooser) As JButton
			Return getApproveButton(fc)
		End Function

		Public Overridable Function getApproveButtonToolTipText(ByVal fc As JFileChooser) As String
			Dim tooltipText As String = fc.approveButtonToolTipText
			If tooltipText IsNot Nothing Then Return tooltipText

			If fc.dialogType = JFileChooser.OPEN_DIALOG Then
				Return openButtonToolTipText
			ElseIf fc.dialogType = JFileChooser.SAVE_DIALOG Then
				Return saveButtonToolTipText
			End If
			Return Nothing
		End Function

		Public Overridable Sub clearIconCache()
			fileView.clearIconCache()
		End Sub


		' ********************************************
		' ************ Create Listeners **************
		' ********************************************

		Private Property handler As Handler
			Get
				If handler Is Nothing Then handler = New Handler(Me)
				Return handler
			End Get
		End Property

		Protected Friend Overridable Function createDoubleClickListener(ByVal fc As JFileChooser, ByVal list As JList) As MouseListener
			Return New Handler(Me, list)
		End Function

		Public Overridable Function createListSelectionListener(ByVal fc As JFileChooser) As ListSelectionListener
			Return handler
		End Function

		Private Class Handler
			Implements MouseListener, ListSelectionListener

			Private ReadOnly outerInstance As BasicFileChooserUI

			Friend list As JList

			Friend Sub New(ByVal outerInstance As BasicFileChooserUI)
					Me.outerInstance = outerInstance
			End Sub

			Friend Sub New(ByVal outerInstance As BasicFileChooserUI, ByVal list As JList)
					Me.outerInstance = outerInstance
				Me.list = list
			End Sub

			Public Overridable Sub mouseClicked(ByVal evt As MouseEvent)
				' Note: we can't depend on evt.getSource() because of backward
				' compatibility
				If list IsNot Nothing AndAlso SwingUtilities.isLeftMouseButton(evt) AndAlso (evt.clickCount Mod 2 = 0) Then

					Dim index As Integer = sun.swing.SwingUtilities2.loc2IndexFileList(list, evt.point)
					If index >= 0 Then
						Dim f As File = CType(list.model.getElementAt(index), File)
						Try
							' Strip trailing ".."
							f = sun.awt.shell.ShellFolder.getNormalizedFile(f)
						Catch ex As IOException
							' That's ok, we'll use f as is
						End Try
						If outerInstance.fileChooser.isTraversable(f) Then
							list.clearSelection()
							changeDirectory(f)
						Else
							outerInstance.fileChooser.approveSelection()
						End If
					End If
				End If
			End Sub

			Public Overridable Sub mouseEntered(ByVal evt As MouseEvent)
				If list IsNot Nothing Then
					Dim th1 As TransferHandler = outerInstance.fileChooser.transferHandler
					Dim th2 As TransferHandler = list.transferHandler
					If th1 IsNot th2 Then list.transferHandler = th1
					If outerInstance.fileChooser.dragEnabled <> list.dragEnabled Then list.dragEnabled = outerInstance.fileChooser.dragEnabled
				End If
			End Sub

			Public Overridable Sub mouseExited(ByVal evt As MouseEvent)
			End Sub

			Public Overridable Sub mousePressed(ByVal evt As MouseEvent)
			End Sub

			Public Overridable Sub mouseReleased(ByVal evt As MouseEvent)
			End Sub

			Public Overridable Sub valueChanged(ByVal evt As ListSelectionEvent) Implements ListSelectionListener.valueChanged
				If Not evt.valueIsAdjusting Then
					Dim chooser As JFileChooser = outerInstance.fileChooser
					Dim fsv As FileSystemView = chooser.fileSystemView
					Dim list As JList = CType(evt.source, JList)

					Dim fsm As Integer = chooser.fileSelectionMode
					Dim useSetDirectory As Boolean = outerInstance.usesSingleFilePane AndAlso (fsm = JFileChooser.FILES_ONLY)

					If chooser.multiSelectionEnabled Then
						Dim files As File() = Nothing
						Dim objects As Object() = list.selectedValues
						If objects IsNot Nothing Then
							If objects.Length = 1 AndAlso CType(objects(0), File).directory AndAlso chooser.isTraversable((CType(objects(0), File))) AndAlso (useSetDirectory OrElse (Not fsv.isFileSystem((CType(objects(0), File))))) Then
								outerInstance.directorySelected = True
								outerInstance.directory = (CType(objects(0), File))
							Else
								Dim fList As New List(Of File)(objects.Length)
								For Each [object] As Object In objects
									Dim f As File = CType([object], File)
									Dim isDir As Boolean = f.directory
									If (chooser.fileSelectionEnabled AndAlso (Not isDir)) OrElse (chooser.directorySelectionEnabled AndAlso fsv.isFileSystem(f) AndAlso isDir) Then fList.Add(f)
								Next [object]
								If fList.Count > 0 Then files = fList.ToArray()
								outerInstance.directorySelected = False
							End If
						End If
						chooser.selectedFiles = files
					Else
						Dim file As File = CType(list.selectedValue, File)
						If file IsNot Nothing AndAlso file.directory AndAlso chooser.isTraversable(file) AndAlso (useSetDirectory OrElse (Not fsv.isFileSystem(file))) Then

							outerInstance.directorySelected = True
							outerInstance.directory = file
							If outerInstance.usesSingleFilePane Then chooser.selectedFile = Nothing
						Else
							outerInstance.directorySelected = False
							If file IsNot Nothing Then chooser.selectedFile = file
						End If
					End If
				End If
			End Sub
		End Class

		Protected Friend Class DoubleClickListener
			Inherits MouseAdapter

			Private ReadOnly outerInstance As BasicFileChooserUI

			' NOTE: This class exists only for backward compatibility. All
			' its functionality has been moved into Handler. If you need to add
			' new functionality add it to the Handler, but make sure this
			' class calls into the Handler.
			Friend handler As Handler
			Public Sub New(ByVal outerInstance As BasicFileChooserUI, ByVal list As JList)
					Me.outerInstance = outerInstance
				handler = New Handler(list)
			End Sub

			''' <summary>
			''' The JList used for representing the files is created by subclasses, but the
			''' selection is monitored in this class.  The TransferHandler installed in the
			''' JFileChooser is also installed in the file list as it is used as the actual
			''' transfer source.  The list is updated on a mouse enter to reflect the current
			''' data transfer state of the file chooser.
			''' </summary>
			Public Overridable Sub mouseEntered(ByVal e As MouseEvent)
				handler.mouseEntered(e)
			End Sub

			Public Overridable Sub mouseClicked(ByVal e As MouseEvent)
				handler.mouseClicked(e)
			End Sub
		End Class

		Protected Friend Class SelectionListener
			Implements ListSelectionListener

			Private ReadOnly outerInstance As BasicFileChooserUI

			Public Sub New(ByVal outerInstance As BasicFileChooserUI)
				Me.outerInstance = outerInstance
			End Sub

			' NOTE: This class exists only for backward compatibility. All
			' its functionality has been moved into Handler. If you need to add
			' new functionality add it to the Handler, but make sure this
			' class calls into the Handler.
			Public Overridable Sub valueChanged(ByVal e As ListSelectionEvent) Implements ListSelectionListener.valueChanged
				outerInstance.handler.valueChanged(e)
			End Sub
		End Class

		''' <summary>
		''' Property to remember whether a directory is currently selected in the UI.
		''' </summary>
		''' <returns> <code>true</code> iff a directory is currently selected.
		''' @since 1.4 </returns>
		Protected Friend Overridable Property directorySelected As Boolean
			Get
				Return directorySelected
			End Get
			Set(ByVal b As Boolean)
				directorySelected = b
			End Set
		End Property


		''' <summary>
		''' Property to remember the directory that is currently selected in the UI.
		''' </summary>
		''' <returns> the value of the <code>directory</code> property </returns>
		''' <seealso cref= #setDirectory
		''' @since 1.4 </seealso>
		Protected Friend Overridable Property directory As File
			Get
				Return directory
			End Get
			Set(ByVal f As File)
				directory = f
			End Set
		End Property


		''' <summary>
		''' Returns the mnemonic for the given key.
		''' </summary>
		Private Function getMnemonic(ByVal key As String, ByVal l As Locale) As Integer
			Return sun.swing.SwingUtilities2.getUIDefaultsInt(key, l)
		End Function

		' *******************************************************
		' ************ FileChooser UI PLAF methods **************
		' *******************************************************

		''' <summary>
		''' Returns the default accept all file filter
		''' </summary>
		Public Overrides Function getAcceptAllFileFilter(ByVal fc As JFileChooser) As javax.swing.filechooser.FileFilter
			Return acceptAllFileFilter
		End Function


		Public Overrides Function getFileView(ByVal fc As JFileChooser) As FileView
			Return fileView
		End Function


		''' <summary>
		''' Returns the title of this dialog
		''' </summary>
		Public Overrides Function getDialogTitle(ByVal fc As JFileChooser) As String
			Dim ___dialogTitle As String = fc.dialogTitle
			If ___dialogTitle IsNot Nothing Then
				Return ___dialogTitle
			ElseIf fc.dialogType = JFileChooser.OPEN_DIALOG Then
				Return openDialogTitleText
			ElseIf fc.dialogType = JFileChooser.SAVE_DIALOG Then
				Return saveDialogTitleText
			Else
				Return getApproveButtonText(fc)
			End If
		End Function


		Public Overridable Function getApproveButtonMnemonic(ByVal fc As JFileChooser) As Integer
			Dim ___mnemonic As Integer = fc.approveButtonMnemonic
			If ___mnemonic > 0 Then
				Return ___mnemonic
			ElseIf fc.dialogType = JFileChooser.OPEN_DIALOG Then
				Return openButtonMnemonic
			ElseIf fc.dialogType = JFileChooser.SAVE_DIALOG Then
				Return saveButtonMnemonic
			Else
				Return ___mnemonic
			End If
		End Function

		Public Overrides Function getApproveButtonText(ByVal fc As JFileChooser) As String
			Dim buttonText As String = fc.approveButtonText
			If buttonText IsNot Nothing Then
				Return buttonText
			ElseIf fc.dialogType = JFileChooser.OPEN_DIALOG Then
				Return openButtonText
			ElseIf fc.dialogType = JFileChooser.SAVE_DIALOG Then
				Return saveButtonText
			Else
				Return Nothing
			End If
		End Function


		' *****************************
		' ***** Directory Actions *****
		' *****************************

		Public Overridable Property newFolderAction As Action
			Get
				If newFolderAction Is Nothing Then
					newFolderAction = New NewFolderAction(Me)
					' Note: Don't return null for readOnly, it might
					' break older apps.
					If [readOnly] Then newFolderAction.enabled = False
				End If
				Return newFolderAction
			End Get
		End Property

		Public Overridable Property goHomeAction As Action
			Get
				Return goHomeAction
			End Get
		End Property

		Public Overridable Property changeToParentDirectoryAction As Action
			Get
				Return changeToParentDirectoryAction
			End Get
		End Property

		Public Overridable Property approveSelectionAction As Action
			Get
				Return approveSelectionAction
			End Get
		End Property

		Public Overridable Property cancelSelectionAction As Action
			Get
				Return cancelSelectionAction
			End Get
		End Property

		Public Overridable Property updateAction As Action
			Get
				Return updateAction
			End Get
		End Property


		''' <summary>
		''' Creates a new folder.
		''' </summary>
		Protected Friend Class NewFolderAction
			Inherits AbstractAction

			Private ReadOnly outerInstance As BasicFileChooserUI

			Protected Friend Sub New(ByVal outerInstance As BasicFileChooserUI)
					Me.outerInstance = outerInstance
				MyBase.New(FilePane.ACTION_NEW_FOLDER)
			End Sub
			Public Overridable Sub actionPerformed(ByVal e As ActionEvent)
				If outerInstance.readOnly Then Return
				Dim fc As JFileChooser = outerInstance.fileChooser
				Dim currentDirectory As File = fc.currentDirectory

				If Not currentDirectory.exists() Then
					JOptionPane.showMessageDialog(fc, outerInstance.newFolderParentDoesntExistText, outerInstance.newFolderParentDoesntExistTitleText, JOptionPane.WARNING_MESSAGE)
					Return
				End If

				Dim newFolder As File
				Try
					newFolder = fc.fileSystemView.createNewFolder(currentDirectory)
					If fc.multiSelectionEnabled Then
						fc.selectedFiles = New File() { newFolder }
					Else
						fc.selectedFile = newFolder
					End If
				Catch exc As IOException
					JOptionPane.showMessageDialog(fc, outerInstance.newFolderErrorText + outerInstance.newFolderErrorSeparator + exc, outerInstance.newFolderErrorText, JOptionPane.ERROR_MESSAGE)
					Return
				End Try

				fc.rescanCurrentDirectory()
			End Sub
		End Class

		''' <summary>
		''' Acts on the "home" key event or equivalent event.
		''' </summary>
		Protected Friend Class GoHomeAction
			Inherits AbstractAction

			Private ReadOnly outerInstance As BasicFileChooserUI

			Protected Friend Sub New(ByVal outerInstance As BasicFileChooserUI)
					Me.outerInstance = outerInstance
				MyBase.New("Go Home")
			End Sub
			Public Overridable Sub actionPerformed(ByVal e As ActionEvent)
				Dim fc As JFileChooser = outerInstance.fileChooser
				changeDirectory(fc.fileSystemView.homeDirectory)
			End Sub
		End Class

		Protected Friend Class ChangeToParentDirectoryAction
			Inherits AbstractAction

			Private ReadOnly outerInstance As BasicFileChooserUI

			Protected Friend Sub New(ByVal outerInstance As BasicFileChooserUI)
					Me.outerInstance = outerInstance
				MyBase.New("Go Up")
				putValue(Action.ACTION_COMMAND_KEY, FilePane.ACTION_CHANGE_TO_PARENT_DIRECTORY)
			End Sub
			Public Overridable Sub actionPerformed(ByVal e As ActionEvent)
				outerInstance.fileChooser.changeToParentDirectory()
			End Sub
		End Class

		''' <summary>
		''' Responds to an Open or Save request
		''' </summary>
		Protected Friend Class ApproveSelectionAction
			Inherits AbstractAction

			Private ReadOnly outerInstance As BasicFileChooserUI

			Protected Friend Sub New(ByVal outerInstance As BasicFileChooserUI)
					Me.outerInstance = outerInstance
				MyBase.New(FilePane.ACTION_APPROVE_SELECTION)
			End Sub
			Public Overridable Sub actionPerformed(ByVal e As ActionEvent)
				If outerInstance.directorySelected Then
					Dim dir As File = outerInstance.directory
					If dir IsNot Nothing Then
						Try
							' Strip trailing ".."
							dir = sun.awt.shell.ShellFolder.getNormalizedFile(dir)
						Catch ex As IOException
							' Ok, use f as is
						End Try
						changeDirectory(dir)
						Return
					End If
				End If

				Dim chooser As JFileChooser = outerInstance.fileChooser

				Dim filename As String = outerInstance.fileName
				Dim fs As FileSystemView = chooser.fileSystemView
				Dim dir As File = chooser.currentDirectory

				If filename IsNot Nothing Then
					' Remove whitespaces from end of filename
					Dim i As Integer = filename.Length - 1

					Do While i >=0 AndAlso filename.Chars(i) <= " "c
						i -= 1
					Loop

					filename = filename.Substring(0, i + 1)
				End If

				If filename Is Nothing OrElse filename.Length = 0 Then
					' no file selected, multiple selection off, therefore cancel the approve action
					outerInstance.resetGlobFilter()
					Return
				End If

				Dim selectedFile As File = Nothing
				Dim selectedFiles As File() = Nothing

				' Unix: Resolve '~' to user's home directory
				If System.IO.Path.DirectorySeparatorChar = "/"c Then
					If filename.StartsWith("~/") Then
						filename = System.getProperty("user.home") + filename.Substring(1)
					ElseIf filename.Equals("~") Then
						filename = System.getProperty("user.home")
					End If
				End If

				If chooser.multiSelectionEnabled AndAlso filename.Length > 1 AndAlso filename.Chars(0) = """"c AndAlso filename.Chars(filename.Length - 1) = """"c Then
					Dim fList As IList(Of File) = New List(Of File)

					Dim files As String() = filename.Substring(1, filename.Length - 1 - 1).Split(""" """)
					' Optimize searching files by names in "children" array
					Array.Sort(files)

					Dim children As File() = Nothing
					Dim childIndex As Integer = 0

					For Each str As String In files
						Dim file As File = fs.createFileObject(str)
						If Not file.absolute Then
							If children Is Nothing Then
								children = fs.getFiles(dir, False)
								Array.Sort(children)
							End If
							For k As Integer = 0 To children.Length - 1
								Dim l As Integer = (childIndex + k) Mod children.Length
								If children(l).name.Equals(str) Then
									file = children(l)
									childIndex = l + 1
									Exit For
								End If
							Next k
						End If
						fList.Add(file)
					Next str

					If fList.Count > 0 Then selectedFiles = fList.ToArray()
					outerInstance.resetGlobFilter()
				Else
					selectedFile = fs.createFileObject(filename)
					If Not selectedFile.absolute Then selectedFile = fs.getChild(dir, filename)
					' check for wildcard pattern
					Dim currentFilter As javax.swing.filechooser.FileFilter = chooser.fileFilter
					If (Not selectedFile.exists()) AndAlso isGlobPattern(filename) Then
						changeDirectory(selectedFile.parentFile)
						If outerInstance.globFilter Is Nothing Then outerInstance.globFilter = New GlobFilter
						Try
							outerInstance.globFilter.pattern = selectedFile.name
							If Not(TypeOf currentFilter Is GlobFilter) Then outerInstance.actualFileFilter = currentFilter
							chooser.fileFilter = Nothing
							chooser.fileFilter = outerInstance.globFilter
							Return
						Catch pse As PatternSyntaxException
							' Not a valid glob pattern. Abandon filter.
						End Try
					End If

					outerInstance.resetGlobFilter()

					' Check for directory change action
					Dim isDir As Boolean = (selectedFile IsNot Nothing AndAlso selectedFile.directory)
					Dim isTrav As Boolean = (selectedFile IsNot Nothing AndAlso chooser.isTraversable(selectedFile))
					Dim isDirSelEnabled As Boolean = chooser.directorySelectionEnabled
					Dim isFileSelEnabled As Boolean = chooser.fileSelectionEnabled
					Dim isCtrl As Boolean = (e IsNot Nothing AndAlso (e.modifiers And Toolkit.defaultToolkit.menuShortcutKeyMask) <> 0)

					If isDir AndAlso isTrav AndAlso (isCtrl OrElse (Not isDirSelEnabled)) Then
						changeDirectory(selectedFile)
						Return
					ElseIf (isDir OrElse (Not isFileSelEnabled)) AndAlso ((Not isDir) OrElse (Not isDirSelEnabled)) AndAlso ((Not isDirSelEnabled) OrElse selectedFile.exists()) Then
						selectedFile = Nothing
					End If
				End If

				If selectedFiles IsNot Nothing OrElse selectedFile IsNot Nothing Then
					If selectedFiles IsNot Nothing OrElse chooser.multiSelectionEnabled Then
						If selectedFiles Is Nothing Then selectedFiles = New File() { selectedFile }
						chooser.selectedFiles = selectedFiles
						' Do it again. This is a fix for bug 4949273 to force the
						' selected value in case the ListSelectionModel clears it
						' for non-existing file names.
						chooser.selectedFiles = selectedFiles
					Else
						chooser.selectedFile = selectedFile
					End If
					chooser.approveSelection()
				Else
					If chooser.multiSelectionEnabled Then
						chooser.selectedFiles = Nothing
					Else
						chooser.selectedFile = Nothing
					End If
					chooser.cancelSelection()
				End If
			End Sub
		End Class


		Private Sub resetGlobFilter()
			If actualFileFilter IsNot Nothing Then
				Dim chooser As JFileChooser = fileChooser
				Dim currentFilter As javax.swing.filechooser.FileFilter = chooser.fileFilter
				If currentFilter IsNot Nothing AndAlso currentFilter.Equals(globFilter) Then
					chooser.fileFilter = actualFileFilter
					chooser.removeChoosableFileFilter(globFilter)
				End If
				actualFileFilter = Nothing
			End If
		End Sub

		Private Shared Function isGlobPattern(ByVal filename As String) As Boolean
			Return ((System.IO.Path.DirectorySeparatorChar = "\"c AndAlso (filename.IndexOf("*"c) >= 0 OrElse filename.IndexOf("?"c) >= 0)) OrElse (System.IO.Path.DirectorySeparatorChar = "/"c AndAlso (filename.IndexOf("*"c) >= 0 OrElse filename.IndexOf("?"c) >= 0 OrElse filename.IndexOf("["c) >= 0)))
		End Function


	'     A file filter which accepts file patterns containing
	'     * the special wildcards *? on Windows and *?[] on Unix.
	'     
		Friend Class GlobFilter
			Inherits javax.swing.filechooser.FileFilter

			Private ReadOnly outerInstance As BasicFileChooserUI

			Public Sub New(ByVal outerInstance As BasicFileChooserUI)
				Me.outerInstance = outerInstance
			End Sub

			Friend pattern As Pattern
			Friend globPattern As String

			Public Overridable Sub setPattern(ByVal globPattern As String)
				Dim gPat As Char() = globPattern.ToCharArray()
				Dim rPat As Char() = New Char(gPat.Length * 2 - 1){}
				Dim isWin32 As Boolean = (System.IO.Path.DirectorySeparatorChar = "\"c)
				Dim inBrackets As Boolean = False
				Dim j As Integer = 0

				Me.globPattern = globPattern

				If isWin32 Then
					' On windows, a pattern ending with *.* is equal to ending with *
					Dim len As Integer = gPat.Length
					If globPattern.EndsWith("*.*") Then len -= 2
					For i As Integer = 0 To len - 1
						Select Case gPat(i)
						  Case "*"c
							rPat(j) = "."c
							j += 1
							rPat(j) = "*"c
							j += 1

						  Case "?"c
							rPat(j) = "."c
							j += 1

						  Case "\"c
							rPat(j) = """c; j += 1; rPat[j++] = "\"c; break; default: if (" & () Xor $.
							() ".indexOf(gPat[i]) >= 0) { rPat[j++] = "\"c; } rPat[j++] = gPat[i]; break; } } } else { for (int i = 0; i < gPat.length; i++) { switch(gPat[i]) { case "*"c: if (!inBrackets) { rPat[j++] = "."c; } rPat[j++] = "*"c; break; case "?"c: rPat[j++] = inBrackets ? "?"c : "."c; break; case ".Chars("c: inBrackets = true; rPat[j++] = gPat[i]; if (i < gPat.length - 1) { switch (gPat[i+1]) { case ".Chars(Not "c: case ") Xor "c: rPat[j++] = " Xor "c; i++; break; case ")"c: rPat[j++] = gPat[++i]; break; } } break; case ")"c: rPat[j++] = gPat[i]; inBrackets = false; break; case "\"c: if (i == 0 && gPat.length > 1 && gPat[1] == ".Chars(Not "c) { rPat[j++] = gPat[++i]; } else { rPat[j++] = ")\"c; if (i < gPat.length - 1 && "*?()".indexOf(gPat[i+1]) >= 0) { rPat[j++] = gPat[++i]; } else { rPat[j++] = "\"c; } } break; default: if (!Character.isLetterOrDigit(gPat[i])) { rPat[j++] = "\"c; } rPat[j++] = gPat[i]; break; } } } this.pattern = Pattern.compile(new String(rPat, 0, j), Pattern.CASE_INSENSITIVE); } public boolean accept(File f) { if (f == null) { return false; } if (f.isDirectory()) { return true; } return pattern.matcher(f.getName()).matches(); } public String getDescription() { return globPattern; } } protected class CancelSelectionAction extends AbstractAction { public void actionPerformed(ActionEvent e) { getFileChooser().cancelSelection(); } } protected class UpdateAction extends AbstractAction { public void actionPerformed(ActionEvent e) { JFileChooser fc = getFileChooser(); fc.setCurrentDirectory(fc.getFileSystemView().createFileObject(getDirectoryName())); fc.rescanCurrentDirectory(); } } private void changeDirectory(File dir) { JFileChooser fc = getFileChooser(); if (dir != null && FilePane.usesShellFolder(fc)) { try { ShellFolder shellFolder = ShellFolder.getShellFolder(dir); if (shellFolder.isLink()) { File linkedTo = shellFolder.getLinkLocation(); if (linkedTo != null) { if (fc.isTraversable(linkedTo)) { dir = linkedTo; } else { return; } } else { dir = shellFolder; } } } catch (FileNotFoundException ex) { return; } } fc.setCurrentDirectory(dir); if (fc.getFileSelectionMode() == JFileChooser.FILES_AND_DIRECTORIES && fc.getFileSystemView().isFileSystem(dir)) { setFileName(dir.getAbsolutePath()); } } protected class AcceptAllFileFilter extends FileFilter { public AcceptAllFileFilter() { } public boolean accept(File f) { return true; } public String getDescription() { return UIManager.getString("FileChooser.acceptAllFileFilterText"); } } protected class BasicFileView extends FileView { protected Hashtable<File,Icon> iconCache = new Hashtable<File,Icon>(); public BasicFileView() { } public void clearIconCache() { iconCache = new Hashtable<File,Icon>(); } public String getName(File f) { String fileName = null; if(f != null) { fileName = getFileChooser().getFileSystemView().getSystemDisplayName(f); } return fileName; } public String getDescription(File f) { return f.getName(); } public String getTypeDescription(File f) { String type = getFileChooser().getFileSystemView().getSystemTypeDescription(f); if (type == null) { if (f.isDirectory()) { type = directoryDescriptionText; } else { type = fileDescriptionText; } } return type; } public Icon getCachedIcon(File f) { return iconCache.get(f); } public void cacheIcon(File f, Icon i) { if(f == null || i == null) { return; } iconCache.put(f, i); } public Icon getIcon(File f) { Icon icon = getCachedIcon(f); if(icon != null) { return icon; } icon = fileIcon; if (f != null) { FileSystemView fsv = getFileChooser().getFileSystemView(); if (fsv.isFloppyDrive(f)) { icon = floppyDriveIcon; } else if (fsv.isDrive(f)) { icon = hardDriveIcon; } else if (fsv.isComputerNode(f)) { icon = computerIcon; } else if (f.isDirectory()) { icon = directoryIcon; } } cacheIcon(f, icon); return icon; } public Boolean isHidden(File f) { String name = f.getName(); if(name != null && name.charAt(0) == "."c) { return Boolean.TRUE; } else { return Boolean.FALSE; } } } private static final TransferHandler defaultTransferHandler = new FileTransferHandler(); static class FileTransferHandler extends TransferHandler implements UIResource { protected Transferable createTransferable(JComponent c) { Object[] values = null; if (c instanceof JList) { values = ((JList)c).getSelectedValues(); } else if (c instanceof JTable) { JTable table = (JTable)c; int[] rows = table.getSelectedRows(); if (rows != null) { values = new Object[rows.length]; for (int i=0; i<rows.length; i++) { values[i] = table.getValueAt(rows[i], 0); } } } if (values == null || values.length == 0) { return null; } StringBuffer plainBuf = new StringBuffer(); StringBuffer htmlBuf = new StringBuffer(); htmlBuf.append(".Chars(Of html)" + vbLf + ".Chars(Of body)" + vbLf + ".Chars(Of ul)" + vbLf); for (Object obj : values) { String val = ((obj == null) ? "" : obj.toString()); plainBuf.append(val + vbLf); htmlBuf.append(" (Of li)" + val + vbLf); } plainBuf.deleteCharAt(plainBuf.length() - 1); htmlBuf.append("</ul>" + vbLf + "</body>" + vbLf + "</html>"); return new FileTransferable(plainBuf.toString(), htmlBuf.toString(), values); } public int getSourceActions(JComponent c) { return COPY; } static class FileTransferable extends BasicTransferable { Object[] fileData; FileTransferable(String plainData, String htmlData, Object[] fileData) { super(plainData, htmlData); this.fileData = fileData; } protected DataFlavor[] getRicherFlavors() { DataFlavor[] flavors = new DataFlavor[1]; flavors[0] = DataFlavor.javaFileListFlavor; return flavors; } protected Object getRicherData(DataFlavor flavor) { if (DataFlavor.javaFileListFlavor.equals(flavor)) { ArrayList<Object> files = new ArrayList<Object>(); for (Object file : this.fileData) { files.add(file); } return files; } return null; } } } }
							'if ("+()|^$.{}<>".indexOf(gPat[i]) >= 0) {
		''' <summary>
		''' Responds to a cancel request.
		''' </summary>
		''' <summary>
		''' Rescans the files in the current directory
		''' </summary>
			' Traverse shortcuts on Windows
						' If linkedTo is null we try to use dir
		' *****************************************
		' ***** default AcceptAll file filter *****
		' *****************************************
		' ***********************
		' * FileView operations *
		' ***********************
			' FileView type descriptions 
			' PENDING(jeff) - pass in the icon cache size
				' Note: Returns display name rather than file name
		''' <summary>
		''' Data transfer support for the file chooser.  Since files are currently presented
		''' as a list, the list support is reused with the added flavor of DataFlavor.javaFileListFlavor
		''' </summary>
			''' <summary>
			''' Create a Transferable to use as the source for a data transfer.
			''' </summary>
			''' <param name="c">  The component holding the data to be transfered.  This
			'''  argument is provided to enable sharing of TransferHandlers by
			'''  multiple components. </param>
			''' <returns>  The representation of the data to be transfered.
			'''  </returns>
				' remove the last newline
				''' <summary>
				''' Best format of the file chooser is DataFlavor.javaFileListFlavor.
				''' </summary>
				''' <summary>
				''' The only richer format supported is the file list flavor
				''' </summary>

End Namespace