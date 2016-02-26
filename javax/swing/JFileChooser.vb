Imports System
Imports System.Diagnostics
Imports System.Collections.Generic
Imports System.Windows.Forms
Imports javax.swing.event
Imports javax.swing.filechooser
Imports javax.accessibility

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

Namespace javax.swing




	''' <summary>
	''' <code>JFileChooser</code> provides a simple mechanism for the user to
	''' choose a file.
	''' For information about using <code>JFileChooser</code>, see
	''' <a
	''' href="https://docs.oracle.com/javase/tutorial/uiswing/components/filechooser.html">How to Use File Choosers</a>,
	''' a section in <em>The Java Tutorial</em>.
	''' 
	''' <p>
	''' 
	''' The following code pops up a file chooser for the user's home directory that
	''' sees only .jpg and .gif images:
	''' <pre>
	'''    JFileChooser chooser = new JFileChooser();
	'''    FileNameExtensionFilter filter = new FileNameExtensionFilter(
	'''        "JPG &amp; GIF Images", "jpg", "gif");
	'''    chooser.setFileFilter(filter);
	'''    int returnVal = chooser.showOpenDialog(parent);
	'''    if(returnVal == JFileChooser.APPROVE_OPTION) {
	'''       System.out.println("You chose to open this file: " +
	'''            chooser.getSelectedFile().getName());
	'''    }
	''' </pre>
	''' <p>
	''' <strong>Warning:</strong> Swing is not thread safe. For more
	''' information see <a
	''' href="package-summary.html#threading">Swing's Threading
	''' Policy</a>.
	''' 
	''' @beaninfo
	'''   attribute: isContainer false
	''' description: A component which allows for the interactive selection of a file.
	''' 
	''' @author Jeff Dinkins
	''' 
	''' </summary>
	Public Class JFileChooser
		Inherits JComponent
		Implements Accessible

		''' <seealso cref= #getUIClassID </seealso>
		''' <seealso cref= #readObject </seealso>
		Private Const uiClassID As String = "FileChooserUI"

		' ************************
		' ***** Dialog Types *****
		' ************************

		''' <summary>
		''' Type value indicating that the <code>JFileChooser</code> supports an
		''' "Open" file operation.
		''' </summary>
		Public Const OPEN_DIALOG As Integer = 0

		''' <summary>
		''' Type value indicating that the <code>JFileChooser</code> supports a
		''' "Save" file operation.
		''' </summary>
		Public Const SAVE_DIALOG As Integer = 1

		''' <summary>
		''' Type value indicating that the <code>JFileChooser</code> supports a
		''' developer-specified file operation.
		''' </summary>
		Public Const CUSTOM_DIALOG As Integer = 2


		' ********************************
		' ***** Dialog Return Values *****
		' ********************************

		''' <summary>
		''' Return value if cancel is chosen.
		''' </summary>
		Public Const CANCEL_OPTION As Integer = 1

		''' <summary>
		''' Return value if approve (yes, ok) is chosen.
		''' </summary>
		Public Const APPROVE_OPTION As Integer = 0

		''' <summary>
		''' Return value if an error occurred.
		''' </summary>
		Public Const ERROR_OPTION As Integer = -1


		' **********************************
		' ***** JFileChooser properties *****
		' **********************************


		''' <summary>
		''' Instruction to display only files. </summary>
		Public Const FILES_ONLY As Integer = 0

		''' <summary>
		''' Instruction to display only directories. </summary>
		Public Const DIRECTORIES_ONLY As Integer = 1

		''' <summary>
		''' Instruction to display both files and directories. </summary>
		Public Const FILES_AND_DIRECTORIES As Integer = 2

		''' <summary>
		''' Instruction to cancel the current selection. </summary>
		Public Const CANCEL_SELECTION As String = "CancelSelection"

		''' <summary>
		''' Instruction to approve the current selection
		''' (same as pressing yes or ok).
		''' </summary>
		Public Const APPROVE_SELECTION As String = "ApproveSelection"

		''' <summary>
		''' Identifies change in the text on the approve (yes, ok) button. </summary>
		Public Const APPROVE_BUTTON_TEXT_CHANGED_PROPERTY As String = "ApproveButtonTextChangedProperty"

		''' <summary>
		''' Identifies change in the tooltip text for the approve (yes, ok)
		''' button.
		''' </summary>
		Public Const APPROVE_BUTTON_TOOL_TIP_TEXT_CHANGED_PROPERTY As String = "ApproveButtonToolTipTextChangedProperty"

		''' <summary>
		''' Identifies change in the mnemonic for the approve (yes, ok) button. </summary>
		Public Const APPROVE_BUTTON_MNEMONIC_CHANGED_PROPERTY As String = "ApproveButtonMnemonicChangedProperty"

		''' <summary>
		''' Instruction to display the control buttons. </summary>
		Public Const CONTROL_BUTTONS_ARE_SHOWN_CHANGED_PROPERTY As String = "ControlButtonsAreShownChangedProperty"

		''' <summary>
		''' Identifies user's directory change. </summary>
		Public Const DIRECTORY_CHANGED_PROPERTY As String = "directoryChanged"

		''' <summary>
		''' Identifies change in user's single-file selection. </summary>
		Public Const SELECTED_FILE_CHANGED_PROPERTY As String = "SelectedFileChangedProperty"

		''' <summary>
		''' Identifies change in user's multiple-file selection. </summary>
		Public Const SELECTED_FILES_CHANGED_PROPERTY As String = "SelectedFilesChangedProperty"

		''' <summary>
		''' Enables multiple-file selections. </summary>
		Public Const MULTI_SELECTION_ENABLED_CHANGED_PROPERTY As String = "MultiSelectionEnabledChangedProperty"

		''' <summary>
		''' Says that a different object is being used to find available drives
		''' on the system.
		''' </summary>
		Public Const FILE_SYSTEM_VIEW_CHANGED_PROPERTY As String = "FileSystemViewChanged"

		''' <summary>
		''' Says that a different object is being used to retrieve file
		''' information.
		''' </summary>
		Public Const FILE_VIEW_CHANGED_PROPERTY As String = "fileViewChanged"

		''' <summary>
		''' Identifies a change in the display-hidden-files property. </summary>
		Public Const FILE_HIDING_CHANGED_PROPERTY As String = "FileHidingChanged"

		''' <summary>
		''' User changed the kind of files to display. </summary>
		Public Const FILE_FILTER_CHANGED_PROPERTY As String = "fileFilterChanged"

		''' <summary>
		''' Identifies a change in the kind of selection (single,
		''' multiple, etc.).
		''' </summary>
		Public Const FILE_SELECTION_MODE_CHANGED_PROPERTY As String = "fileSelectionChanged"

		''' <summary>
		''' Says that a different accessory component is in use
		''' (for example, to preview files).
		''' </summary>
		Public Const ACCESSORY_CHANGED_PROPERTY As String = "AccessoryChangedProperty"

		''' <summary>
		''' Identifies whether a the AcceptAllFileFilter is used or not.
		''' </summary>
		Public Const ACCEPT_ALL_FILE_FILTER_USED_CHANGED_PROPERTY As String = "acceptAllFileFilterUsedChanged"

		''' <summary>
		''' Identifies a change in the dialog title. </summary>
		Public Const DIALOG_TITLE_CHANGED_PROPERTY As String = "DialogTitleChangedProperty"

		''' <summary>
		''' Identifies a change in the type of files displayed (files only,
		''' directories only, or both files and directories).
		''' </summary>
		Public Const DIALOG_TYPE_CHANGED_PROPERTY As String = "DialogTypeChangedProperty"

		''' <summary>
		''' Identifies a change in the list of predefined file filters
		''' the user can choose from.
		''' </summary>
		Public Const CHOOSABLE_FILE_FILTER_CHANGED_PROPERTY As String = "ChoosableFileFilterChangedProperty"

		' ******************************
		' ***** instance variables *****
		' ******************************

		Private dialogTitle As String = Nothing
		Private approveButtonText As String = Nothing
		Private approveButtonToolTipText As String = Nothing
		Private approveButtonMnemonic As Integer = 0

		Private filters As New List(Of FileFilter)(5)
		Private dialog As JDialog = Nothing
		Private dialogType As Integer = OPEN_DIALOG
		Private returnValue As Integer = ERROR_OPTION
		Private accessory As JComponent = Nothing

		Private fileView As FileView = Nothing

		Private controlsShown As Boolean = True

		Private useFileHiding As Boolean = True
		Private Const SHOW_HIDDEN_PROP As String = "awt.file.showHiddenFiles"

		' Listens to changes in the native setting for showing hidden files.
		' The Listener is removed and the native setting is ignored if
		' setFileHidingEnabled() is ever called.
		<NonSerialized> _
		Private showFilesListener As java.beans.PropertyChangeListener = Nothing

		Private fileSelectionMode As Integer = FILES_ONLY

		Private multiSelectionEnabled As Boolean = False

		Private useAcceptAllFileFilter As Boolean = True

		Private dragEnabled As Boolean = False

		Private fileFilter As FileFilter = Nothing

		Private fileSystemView As FileSystemView = Nothing

		Private currentDirectory As java.io.File = Nothing
		Private selectedFile As java.io.File = Nothing
		Private selectedFiles As java.io.File()

		' *************************************
		' ***** JFileChooser Constructors *****
		' *************************************

		''' <summary>
		''' Constructs a <code>JFileChooser</code> pointing to the user's
		''' default directory. This default depends on the operating system.
		''' It is typically the "My Documents" folder on Windows, and the
		''' user's home directory on Unix.
		''' </summary>
		Public Sub New()
			Me.New(CType(Nothing, File), CType(Nothing, FileSystemView))
		End Sub

		''' <summary>
		''' Constructs a <code>JFileChooser</code> using the given path.
		''' Passing in a <code>null</code>
		''' string causes the file chooser to point to the user's default directory.
		''' This default depends on the operating system. It is
		''' typically the "My Documents" folder on Windows, and the user's
		''' home directory on Unix.
		''' </summary>
		''' <param name="currentDirectoryPath">  a <code>String</code> giving the path
		'''                          to a file or directory </param>
		Public Sub New(ByVal currentDirectoryPath As String)
			Me.New(currentDirectoryPath, CType(Nothing, FileSystemView))
		End Sub

		''' <summary>
		''' Constructs a <code>JFileChooser</code> using the given <code>File</code>
		''' as the path. Passing in a <code>null</code> file
		''' causes the file chooser to point to the user's default directory.
		''' This default depends on the operating system. It is
		''' typically the "My Documents" folder on Windows, and the user's
		''' home directory on Unix.
		''' </summary>
		''' <param name="currentDirectory">  a <code>File</code> object specifying
		'''                          the path to a file or directory </param>
		Public Sub New(ByVal currentDirectory As java.io.File)
			Me.New(currentDirectory, CType(Nothing, FileSystemView))
		End Sub

		''' <summary>
		''' Constructs a <code>JFileChooser</code> using the given
		''' <code>FileSystemView</code>.
		''' </summary>
		Public Sub New(ByVal fsv As FileSystemView)
			Me.New(CType(Nothing, File), fsv)
		End Sub


		''' <summary>
		''' Constructs a <code>JFileChooser</code> using the given current directory
		''' and <code>FileSystemView</code>.
		''' </summary>
		Public Sub New(ByVal currentDirectory As java.io.File, ByVal fsv As FileSystemView)
			setup(fsv)
			currentDirectory = currentDirectory
		End Sub

		''' <summary>
		''' Constructs a <code>JFileChooser</code> using the given current directory
		''' path and <code>FileSystemView</code>.
		''' </summary>
		Public Sub New(ByVal currentDirectoryPath As String, ByVal fsv As FileSystemView)
			setup(fsv)
			If currentDirectoryPath Is Nothing Then
				currentDirectory = Nothing
			Else
				currentDirectory = fileSystemView.createFileObject(currentDirectoryPath)
			End If
		End Sub

		''' <summary>
		''' Performs common constructor initialization and setup.
		''' </summary>
		Protected Friend Overridable Sub setup(ByVal view As FileSystemView)
			installShowFilesListener()
			installHierarchyListener()

			If view Is Nothing Then view = FileSystemView.fileSystemView
			fileSystemView = view
			updateUI()
			If acceptAllFileFilterUsed Then fileFilter = acceptAllFileFilter
			enableEvents(java.awt.AWTEvent.MOUSE_EVENT_MASK)
		End Sub

		Private Sub installHierarchyListener()
'JAVA TO VB CONVERTER TODO TASK: Anonymous inner classes are not converted to VB if the base type is not defined in the code being converted:
'			addHierarchyListener(New HierarchyListener()
	'		{
	'			@Override public void hierarchyChanged(HierarchyEvent e)
	'			{
	'				if ((e.getChangeFlags() & HierarchyEvent.PARENT_CHANGED) == HierarchyEvent.PARENT_CHANGED)
	'				{
	'					JFileChooser fc = JFileChooser.this;
	'					JRootPane rootPane = SwingUtilities.getRootPane(fc);
	'					if (rootPane != Nothing)
	'					{
	'						rootPane.setDefaultButton(fc.getUI().getDefaultButton(fc));
	'					}
	'				}
	'			}
	'		});
		End Sub

		Private Sub installShowFilesListener()
			' Track native setting for showing hidden files
			Dim tk As java.awt.Toolkit = java.awt.Toolkit.defaultToolkit
			Dim showHiddenProperty As Object = tk.getDesktopProperty(SHOW_HIDDEN_PROP)
			If TypeOf showHiddenProperty Is Boolean? Then
				useFileHiding = Not CBool(showHiddenProperty)
				showFilesListener = New WeakPCL(Me)
				tk.addPropertyChangeListener(SHOW_HIDDEN_PROP, showFilesListener)
			End If
		End Sub

		''' <summary>
		''' Sets the <code>dragEnabled</code> property,
		''' which must be <code>true</code> to enable
		''' automatic drag handling (the first part of drag and drop)
		''' on this component.
		''' The <code>transferHandler</code> property needs to be set
		''' to a non-<code>null</code> value for the drag to do
		''' anything.  The default value of the <code>dragEnabled</code>
		''' property
		''' is <code>false</code>.
		''' 
		''' <p>
		''' 
		''' When automatic drag handling is enabled,
		''' most look and feels begin a drag-and-drop operation
		''' whenever the user presses the mouse button over an item
		''' and then moves the mouse a few pixels.
		''' Setting this property to <code>true</code>
		''' can therefore have a subtle effect on
		''' how selections behave.
		''' 
		''' <p>
		''' 
		''' Some look and feels might not support automatic drag and drop;
		''' they will ignore this property.  You can work around such
		''' look and feels by modifying the component
		''' to directly call the <code>exportAsDrag</code> method of a
		''' <code>TransferHandler</code>.
		''' </summary>
		''' <param name="b"> the value to set the <code>dragEnabled</code> property to </param>
		''' <exception cref="HeadlessException"> if
		'''            <code>b</code> is <code>true</code> and
		'''            <code>GraphicsEnvironment.isHeadless()</code>
		'''            returns <code>true</code> </exception>
		''' <seealso cref= java.awt.GraphicsEnvironment#isHeadless </seealso>
		''' <seealso cref= #getDragEnabled </seealso>
		''' <seealso cref= #setTransferHandler </seealso>
		''' <seealso cref= TransferHandler
		''' @since 1.4
		''' 
		''' @beaninfo
		'''  description: determines whether automatic drag handling is enabled
		'''        bound: false </seealso>
		Public Overridable Property dragEnabled As Boolean
			Set(ByVal b As Boolean)
				If b AndAlso java.awt.GraphicsEnvironment.headless Then Throw New java.awt.HeadlessException
				dragEnabled = b
			End Set
			Get
				Return dragEnabled
			End Get
		End Property


		' *****************************
		' ****** File Operations ******
		' *****************************

		''' <summary>
		''' Returns the selected file. This can be set either by the
		''' programmer via <code>setSelectedFile</code> or by a user action, such as
		''' either typing the filename into the UI or selecting the
		''' file from a list in the UI.
		''' </summary>
		''' <seealso cref= #setSelectedFile </seealso>
		''' <returns> the selected file </returns>
		Public Overridable Property selectedFile As java.io.File
			Get
				Return selectedFile
			End Get
			Set(ByVal file As java.io.File)
				Dim oldValue As File = selectedFile
				selectedFile = file
				If selectedFile IsNot Nothing Then
					If file.absolute AndAlso (Not fileSystemView.isParent(currentDirectory, selectedFile)) Then currentDirectory = selectedFile.parentFile
					If (Not multiSelectionEnabled) OrElse selectedFiles Is Nothing OrElse selectedFiles.Length = 1 Then ensureFileIsVisible(selectedFile)
				End If
				firePropertyChange(SELECTED_FILE_CHANGED_PROPERTY, oldValue, selectedFile)
			End Set
		End Property


		''' <summary>
		''' Returns a list of selected files if the file chooser is
		''' set to allow multiple selection.
		''' </summary>
		Public Overridable Property selectedFiles As java.io.File()
			Get
				If selectedFiles Is Nothing Then
					Return New File(){}
				Else
					Return selectedFiles.clone()
				End If
			End Get
			Set(ByVal selectedFiles As java.io.File())
				Dim oldValue As File() = Me.selectedFiles
				If selectedFiles Is Nothing OrElse selectedFiles.Length = 0 Then
					selectedFiles = Nothing
					Me.selectedFiles = Nothing
					selectedFile = Nothing
				Else
					Me.selectedFiles = selectedFiles.clone()
					selectedFile = Me.selectedFiles(0)
				End If
				firePropertyChange(SELECTED_FILES_CHANGED_PROPERTY, oldValue, selectedFiles)
			End Set
		End Property


		''' <summary>
		''' Returns the current directory.
		''' </summary>
		''' <returns> the current directory </returns>
		''' <seealso cref= #setCurrentDirectory </seealso>
		Public Overridable Property currentDirectory As java.io.File
			Get
				Return currentDirectory
			End Get
			Set(ByVal dir As java.io.File)
				Dim oldValue As File = currentDirectory
    
				If dir IsNot Nothing AndAlso (Not dir.exists()) Then dir = currentDirectory
				If dir Is Nothing Then dir = fileSystemView.defaultDirectory
				If currentDirectory IsNot Nothing Then
					' Verify the toString of object 
					If Me.currentDirectory.Equals(dir) Then Return
				End If
    
				Dim prev As File = Nothing
				Do While (Not isTraversable(dir)) AndAlso prev IsNot dir
					prev = dir
					dir = fileSystemView.getParentDirectory(dir)
				Loop
				currentDirectory = dir
    
				firePropertyChange(DIRECTORY_CHANGED_PROPERTY, oldValue, currentDirectory)
			End Set
		End Property


		''' <summary>
		''' Changes the directory to be set to the parent of the
		''' current directory.
		''' </summary>
		''' <seealso cref= #getCurrentDirectory </seealso>
		Public Overridable Sub changeToParentDirectory()
			selectedFile = Nothing
			Dim oldValue As File = currentDirectory
			currentDirectory = fileSystemView.getParentDirectory(oldValue)
		End Sub

		''' <summary>
		''' Tells the UI to rescan its files list from the current directory.
		''' </summary>
		Public Overridable Sub rescanCurrentDirectory()
			uI.rescanCurrentDirectory(Me)
		End Sub

		''' <summary>
		''' Makes sure that the specified file is viewable, and
		''' not hidden.
		''' </summary>
		''' <param name="f">  a File object </param>
		Public Overridable Sub ensureFileIsVisible(ByVal f As java.io.File)
			uI.ensureFileIsVisible(Me, f)
		End Sub

		' **************************************
		' ***** JFileChooser Dialog methods *****
		' **************************************

		''' <summary>
		''' Pops up an "Open File" file chooser dialog. Note that the
		''' text that appears in the approve button is determined by
		''' the L&amp;F.
		''' </summary>
		''' <param name="parent">  the parent component of the dialog,
		'''                  can be <code>null</code>;
		'''                  see <code>showDialog</code> for details </param>
		''' <returns>   the return state of the file chooser on popdown:
		''' <ul>
		''' <li>JFileChooser.CANCEL_OPTION
		''' <li>JFileChooser.APPROVE_OPTION
		''' <li>JFileChooser.ERROR_OPTION if an error occurs or the
		'''                  dialog is dismissed
		''' </ul> </returns>
		''' <exception cref="HeadlessException"> if GraphicsEnvironment.isHeadless()
		''' returns true. </exception>
		''' <seealso cref= java.awt.GraphicsEnvironment#isHeadless </seealso>
		''' <seealso cref= #showDialog </seealso>
		Public Overridable Function showOpenDialog(ByVal parent As java.awt.Component) As Integer
			dialogType = OPEN_DIALOG
			Return showDialog(parent, Nothing)
		End Function

		''' <summary>
		''' Pops up a "Save File" file chooser dialog. Note that the
		''' text that appears in the approve button is determined by
		''' the L&amp;F.
		''' </summary>
		''' <param name="parent">  the parent component of the dialog,
		'''                  can be <code>null</code>;
		'''                  see <code>showDialog</code> for details </param>
		''' <returns>   the return state of the file chooser on popdown:
		''' <ul>
		''' <li>JFileChooser.CANCEL_OPTION
		''' <li>JFileChooser.APPROVE_OPTION
		''' <li>JFileChooser.ERROR_OPTION if an error occurs or the
		'''                  dialog is dismissed
		''' </ul> </returns>
		''' <exception cref="HeadlessException"> if GraphicsEnvironment.isHeadless()
		''' returns true. </exception>
		''' <seealso cref= java.awt.GraphicsEnvironment#isHeadless </seealso>
		''' <seealso cref= #showDialog </seealso>
		Public Overridable Function showSaveDialog(ByVal parent As java.awt.Component) As Integer
			dialogType = SAVE_DIALOG
			Return showDialog(parent, Nothing)
		End Function

		''' <summary>
		''' Pops a custom file chooser dialog with a custom approve button.
		''' For example, the following code
		''' pops up a file chooser with a "Run Application" button
		''' (instead of the normal "Save" or "Open" button):
		''' <pre>
		''' filechooser.showDialog(parentFrame, "Run Application");
		''' </pre>
		''' 
		''' Alternatively, the following code does the same thing:
		''' <pre>
		'''    JFileChooser chooser = new JFileChooser(null);
		'''    chooser.setApproveButtonText("Run Application");
		'''    chooser.showDialog(parentFrame, null);
		''' </pre>
		''' 
		''' <!--PENDING(jeff) - the following method should be added to the api:
		'''      showDialog(Component parent);-->
		''' <!--PENDING(kwalrath) - should specify modality and what
		'''      "depends" means.-->
		''' 
		''' <p>
		''' 
		''' The <code>parent</code> argument determines two things:
		''' the frame on which the open dialog depends and
		''' the component whose position the look and feel
		''' should consider when placing the dialog.  If the parent
		''' is a <code>Frame</code> object (such as a <code>JFrame</code>)
		''' then the dialog depends on the frame and
		''' the look and feel positions the dialog
		''' relative to the frame (for example, centered over the frame).
		''' If the parent is a component, then the dialog
		''' depends on the frame containing the component,
		''' and is positioned relative to the component
		''' (for example, centered over the component).
		''' If the parent is <code>null</code>, then the dialog depends on
		''' no visible window, and it's placed in a
		''' look-and-feel-dependent position
		''' such as the center of the screen.
		''' </summary>
		''' <param name="parent">  the parent component of the dialog;
		'''                  can be <code>null</code> </param>
		''' <param name="approveButtonText"> the text of the <code>ApproveButton</code> </param>
		''' <returns>  the return state of the file chooser on popdown:
		''' <ul>
		''' <li>JFileChooser.CANCEL_OPTION
		''' <li>JFileChooser.APPROVE_OPTION
		''' <li>JFileChooser.ERROR_OPTION if an error occurs or the
		'''                  dialog is dismissed
		''' </ul> </returns>
		''' <exception cref="HeadlessException"> if GraphicsEnvironment.isHeadless()
		''' returns true. </exception>
		''' <seealso cref= java.awt.GraphicsEnvironment#isHeadless </seealso>
		Public Overridable Function showDialog(ByVal parent As java.awt.Component, ByVal approveButtonText As String) As Integer
			If dialog IsNot Nothing Then Return JFileChooser.ERROR_OPTION

			If approveButtonText IsNot Nothing Then
				approveButtonText = approveButtonText
				dialogType = CUSTOM_DIALOG
			End If
			dialog = createDialog(parent)
'JAVA TO VB CONVERTER TODO TASK: Anonymous inner classes are not converted to VB if the base type is not defined in the code being converted:
'			dialog.addWindowListener(New WindowAdapter()
	'		{
	'			public void windowClosing(WindowEvent e)
	'			{
	'				returnValue = CANCEL_OPTION;
	'			}
	'		});
			returnValue = ERROR_OPTION
			rescanCurrentDirectory()

			dialog.show()
			firePropertyChange("JFileChooserDialogIsClosingProperty", dialog, Nothing)

			' Remove all components from dialog. The MetalFileChooserUI.installUI() method (and other LAFs)
			' registers AWT listener for dialogs and produces memory leaks. It happens when
			' installUI invoked after the showDialog method.
			dialog.contentPane.removeAll()
			dialog.Dispose()
			dialog = Nothing
			Return returnValue
		End Function

		''' <summary>
		''' Creates and returns a new <code>JDialog</code> wrapping
		''' <code>this</code> centered on the <code>parent</code>
		''' in the <code>parent</code>'s frame.
		''' This method can be overriden to further manipulate the dialog,
		''' to disable resizing, set the location, etc. Example:
		''' <pre>
		'''     class MyFileChooser extends JFileChooser {
		'''         protected JDialog createDialog(Component parent) throws HeadlessException {
		'''             JDialog dialog = super.createDialog(parent);
		'''             dialog.setLocation(300, 200);
		'''             dialog.setResizable(false);
		'''             return dialog;
		'''         }
		'''     }
		''' </pre>
		''' </summary>
		''' <param name="parent">  the parent component of the dialog;
		'''                  can be <code>null</code> </param>
		''' <returns> a new <code>JDialog</code> containing this instance </returns>
		''' <exception cref="HeadlessException"> if GraphicsEnvironment.isHeadless()
		''' returns true. </exception>
		''' <seealso cref= java.awt.GraphicsEnvironment#isHeadless
		''' @since 1.4 </seealso>
		Protected Friend Overridable Function createDialog(ByVal parent As java.awt.Component) As JDialog
			Dim ___ui As javax.swing.plaf.FileChooserUI = uI
			Dim title As String = ___ui.getDialogTitle(Me)
			putClientProperty(AccessibleContext.ACCESSIBLE_DESCRIPTION_PROPERTY, title)

			Dim dialog As JDialog
			Dim window As java.awt.Window = JOptionPane.getWindowForComponent(parent)
			If TypeOf window Is java.awt.Frame Then
				dialog = New JDialog(CType(window, java.awt.Frame), title, True)
			Else
				dialog = New JDialog(CType(window, java.awt.Dialog), title, True)
			End If
			dialog.componentOrientation = Me.componentOrientation

			Dim contentPane As java.awt.Container = dialog.contentPane
			contentPane.layout = New java.awt.BorderLayout
			contentPane.add(Me, java.awt.BorderLayout.CENTER)

			If JDialog.defaultLookAndFeelDecorated Then
				Dim supportsWindowDecorations As Boolean = UIManager.lookAndFeel.supportsWindowDecorations
				If supportsWindowDecorations Then dialog.rootPane.windowDecorationStyle = JRootPane.FILE_CHOOSER_DIALOG
			End If
			dialog.pack()
			dialog.locationRelativeTo = parent

			Return dialog
		End Function

		' **************************
		' ***** Dialog Options *****
		' **************************

		''' <summary>
		''' Returns the value of the <code>controlButtonsAreShown</code>
		''' property.
		''' </summary>
		''' <returns>   the value of the <code>controlButtonsAreShown</code>
		'''     property
		''' </returns>
		''' <seealso cref= #setControlButtonsAreShown
		''' @since 1.3 </seealso>
		Public Overridable Property controlButtonsAreShown As Boolean
			Get
				Return controlsShown
			End Get
			Set(ByVal b As Boolean)
				If controlsShown = b Then Return
				Dim oldValue As Boolean = controlsShown
				controlsShown = b
				firePropertyChange(CONTROL_BUTTONS_ARE_SHOWN_CHANGED_PROPERTY, oldValue, controlsShown)
			End Set
		End Property



		''' <summary>
		''' Returns the type of this dialog.  The default is
		''' <code>JFileChooser.OPEN_DIALOG</code>.
		''' </summary>
		''' <returns>   the type of dialog to be displayed:
		''' <ul>
		''' <li>JFileChooser.OPEN_DIALOG
		''' <li>JFileChooser.SAVE_DIALOG
		''' <li>JFileChooser.CUSTOM_DIALOG
		''' </ul>
		''' </returns>
		''' <seealso cref= #setDialogType </seealso>
		Public Overridable Property dialogType As Integer
			Get
				Return dialogType
			End Get
			Set(ByVal dialogType As Integer)
				If Me.dialogType = dialogType Then Return
				If Not(dialogType = OPEN_DIALOG OrElse dialogType = SAVE_DIALOG OrElse dialogType = CUSTOM_DIALOG) Then Throw New System.ArgumentException("Incorrect Dialog Type: " & dialogType)
				Dim oldValue As Integer = Me.dialogType
				Me.dialogType = dialogType
				If dialogType = OPEN_DIALOG OrElse dialogType = SAVE_DIALOG Then approveButtonText = Nothing
				firePropertyChange(DIALOG_TYPE_CHANGED_PROPERTY, oldValue, dialogType)
			End Set
		End Property

		''' <summary>
		''' Sets the type of this dialog. Use <code>OPEN_DIALOG</code> when you
		''' want to bring up a file chooser that the user can use to open a file.
		''' Likewise, use <code>SAVE_DIALOG</code> for letting the user choose
		''' a file for saving.
		''' Use <code>CUSTOM_DIALOG</code> when you want to use the file
		''' chooser in a context other than "Open" or "Save".
		''' For instance, you might want to bring up a file chooser that allows
		''' the user to choose a file to execute. Note that you normally would not
		''' need to set the <code>JFileChooser</code> to use
		''' <code>CUSTOM_DIALOG</code>
		''' since a call to <code>setApproveButtonText</code> does this for you.
		''' The default dialog type is <code>JFileChooser.OPEN_DIALOG</code>.
		''' </summary>
		''' <param name="dialogType"> the type of dialog to be displayed:
		''' <ul>
		''' <li>JFileChooser.OPEN_DIALOG
		''' <li>JFileChooser.SAVE_DIALOG
		''' <li>JFileChooser.CUSTOM_DIALOG
		''' </ul>
		''' </param>
		''' <exception cref="IllegalArgumentException"> if <code>dialogType</code> is
		'''                          not legal
		''' @beaninfo
		'''   preferred: true
		'''       bound: true
		''' description: The type (open, save, custom) of the JFileChooser.
		'''        enum:
		'''              OPEN_DIALOG JFileChooser.OPEN_DIALOG
		'''              SAVE_DIALOG JFileChooser.SAVE_DIALOG
		'''              CUSTOM_DIALOG JFileChooser.CUSTOM_DIALOG
		''' </exception>
		''' <seealso cref= #getDialogType </seealso>
		''' <seealso cref= #setApproveButtonText </seealso>
		' PENDING(jeff) - fire button text change property

		''' <summary>
		''' Sets the string that goes in the <code>JFileChooser</code> window's
		''' title bar.
		''' </summary>
		''' <param name="dialogTitle"> the new <code>String</code> for the title bar
		''' 
		''' @beaninfo
		'''   preferred: true
		'''       bound: true
		''' description: The title of the JFileChooser dialog window.
		''' </param>
		''' <seealso cref= #getDialogTitle
		'''  </seealso>
		Public Overridable Property dialogTitle As String
			Set(ByVal dialogTitle As String)
				Dim oldValue As String = Me.dialogTitle
				Me.dialogTitle = dialogTitle
				If dialog IsNot Nothing Then dialog.title = dialogTitle
				firePropertyChange(DIALOG_TITLE_CHANGED_PROPERTY, oldValue, dialogTitle)
			End Set
			Get
				Return dialogTitle
			End Get
		End Property


		' ************************************
		' ***** JFileChooser View Options *****
		' ************************************



		''' <summary>
		''' Sets the tooltip text used in the <code>ApproveButton</code>.
		''' If <code>null</code>, the UI object will determine the button's text.
		''' 
		''' @beaninfo
		'''   preferred: true
		'''       bound: true
		''' description: The tooltip text for the ApproveButton.
		''' </summary>
		''' <param name="toolTipText"> the tooltip text for the approve button </param>
		''' <seealso cref= #setApproveButtonText </seealso>
		''' <seealso cref= #setDialogType </seealso>
		''' <seealso cref= #showDialog </seealso>
		Public Overridable Property approveButtonToolTipText As String
			Set(ByVal toolTipText As String)
				If approveButtonToolTipText = toolTipText Then Return
				Dim oldValue As String = approveButtonToolTipText
				approveButtonToolTipText = toolTipText
				firePropertyChange(APPROVE_BUTTON_TOOL_TIP_TEXT_CHANGED_PROPERTY, oldValue, approveButtonToolTipText)
			End Set
			Get
				Return approveButtonToolTipText
			End Get
		End Property



		''' <summary>
		''' Returns the approve button's mnemonic. </summary>
		''' <returns> an integer value for the mnemonic key
		''' </returns>
		''' <seealso cref= #setApproveButtonMnemonic </seealso>
		Public Overridable Property approveButtonMnemonic As Integer
			Get
				Return approveButtonMnemonic
			End Get
			Set(ByVal mnemonic As Integer)
				If approveButtonMnemonic = mnemonic Then Return
				Dim oldValue As Integer = approveButtonMnemonic
				approveButtonMnemonic = mnemonic
				firePropertyChange(APPROVE_BUTTON_MNEMONIC_CHANGED_PROPERTY, oldValue, approveButtonMnemonic)
			End Set
		End Property


		''' <summary>
		''' Sets the approve button's mnemonic using a character. </summary>
		''' <param name="mnemonic">  a character value for the mnemonic key
		''' </param>
		''' <seealso cref= #getApproveButtonMnemonic </seealso>
		Public Overridable Property approveButtonMnemonic As Char
			Set(ByVal mnemonic As Char)
				Dim vk As Integer = AscW(mnemonic)
				If vk >= "a"c AndAlso vk <="z"c Then vk -= (AscW("a"c) - AscW("A"c))
				approveButtonMnemonic = vk
			End Set
		End Property


		''' <summary>
		''' Sets the text used in the <code>ApproveButton</code> in the
		''' <code>FileChooserUI</code>.
		''' 
		''' @beaninfo
		'''   preferred: true
		'''       bound: true
		''' description: The text that goes in the ApproveButton.
		''' </summary>
		''' <param name="approveButtonText"> the text used in the <code>ApproveButton</code>
		''' </param>
		''' <seealso cref= #getApproveButtonText </seealso>
		''' <seealso cref= #setDialogType </seealso>
		''' <seealso cref= #showDialog </seealso>
		' PENDING(jeff) - have ui set this on dialog type change
		Public Overridable Property approveButtonText As String
			Set(ByVal approveButtonText As String)
				If Me.approveButtonText = approveButtonText Then Return
				Dim oldValue As String = Me.approveButtonText
				Me.approveButtonText = approveButtonText
				firePropertyChange(APPROVE_BUTTON_TEXT_CHANGED_PROPERTY, oldValue, approveButtonText)
			End Set
			Get
				Return approveButtonText
			End Get
		End Property


		''' <summary>
		''' Gets the list of user choosable file filters.
		''' </summary>
		''' <returns> a <code>FileFilter</code> array containing all the choosable
		'''         file filters
		''' </returns>
		''' <seealso cref= #addChoosableFileFilter </seealso>
		''' <seealso cref= #removeChoosableFileFilter </seealso>
		''' <seealso cref= #resetChoosableFileFilters </seealso>
		Public Overridable Property choosableFileFilters As FileFilter()
			Get
				Dim filterArray As FileFilter() = New FileFilter(filters.Count - 1){}
				filters.CopyTo(filterArray)
				Return filterArray
			End Get
		End Property

		''' <summary>
		''' Adds a filter to the list of user choosable file filters.
		''' For information on setting the file selection mode, see
		''' <seealso cref="#setFileSelectionMode setFileSelectionMode"/>.
		''' </summary>
		''' <param name="filter"> the <code>FileFilter</code> to add to the choosable file
		'''               filter list
		''' 
		''' @beaninfo
		'''   preferred: true
		'''       bound: true
		''' description: Adds a filter to the list of user choosable file filters.
		''' </param>
		''' <seealso cref= #getChoosableFileFilters </seealso>
		''' <seealso cref= #removeChoosableFileFilter </seealso>
		''' <seealso cref= #resetChoosableFileFilters </seealso>
		''' <seealso cref= #setFileSelectionMode </seealso>
		Public Overridable Sub addChoosableFileFilter(ByVal filter As FileFilter)
			If filter IsNot Nothing AndAlso (Not filters.Contains(filter)) Then
				Dim oldValue As FileFilter() = choosableFileFilters
				filters.Add(filter)
				firePropertyChange(CHOOSABLE_FILE_FILTER_CHANGED_PROPERTY, oldValue, choosableFileFilters)
				If fileFilter Is Nothing AndAlso filters.Count = 1 Then fileFilter = filter
			End If
		End Sub

		''' <summary>
		''' Removes a filter from the list of user choosable file filters. Returns
		''' true if the file filter was removed.
		''' </summary>
		''' <seealso cref= #addChoosableFileFilter </seealso>
		''' <seealso cref= #getChoosableFileFilters </seealso>
		''' <seealso cref= #resetChoosableFileFilters </seealso>
		Public Overridable Function removeChoosableFileFilter(ByVal f As FileFilter) As Boolean
			Dim index As Integer = filters.IndexOf(f)
			If index >= 0 Then
				If fileFilter Is f Then
					Dim aaff As FileFilter = acceptAllFileFilter
					If acceptAllFileFilterUsed AndAlso (aaff IsNot f) Then
						' choose default filter if it is used
						fileFilter = aaff
					ElseIf index > 0 Then
						' choose the first filter, because it is not removed
						fileFilter = filters(0)
					ElseIf filters.Count > 1 Then
						' choose the second filter, because the first one is removed
						fileFilter = filters(1)
					Else
						' no more filters
						fileFilter = Nothing
					End If
				End If
				Dim oldValue As FileFilter() = choosableFileFilters
				filters.Remove(f)
				firePropertyChange(CHOOSABLE_FILE_FILTER_CHANGED_PROPERTY, oldValue, choosableFileFilters)
				Return True
			Else
				Return False
			End If
		End Function

		''' <summary>
		''' Resets the choosable file filter list to its starting state. Normally,
		''' this removes all added file filters while leaving the
		''' <code>AcceptAll</code> file filter.
		''' </summary>
		''' <seealso cref= #addChoosableFileFilter </seealso>
		''' <seealso cref= #getChoosableFileFilters </seealso>
		''' <seealso cref= #removeChoosableFileFilter </seealso>
		Public Overridable Sub resetChoosableFileFilters()
			Dim oldValue As FileFilter() = choosableFileFilters
			fileFilter = Nothing
			filters.Clear()
			If acceptAllFileFilterUsed Then addChoosableFileFilter(acceptAllFileFilter)
			firePropertyChange(CHOOSABLE_FILE_FILTER_CHANGED_PROPERTY, oldValue, choosableFileFilters)
		End Sub

		''' <summary>
		''' Returns the <code>AcceptAll</code> file filter.
		''' For example, on Microsoft Windows this would be All Files (*.*).
		''' </summary>
		Public Overridable Property acceptAllFileFilter As FileFilter
			Get
				Dim filter As FileFilter = Nothing
				If uI IsNot Nothing Then filter = uI.getAcceptAllFileFilter(Me)
				Return filter
			End Get
		End Property

	   ''' <summary>
	   ''' Returns whether the <code>AcceptAll FileFilter</code> is used. </summary>
	   ''' <returns> true if the <code>AcceptAll FileFilter</code> is used </returns>
	   ''' <seealso cref= #setAcceptAllFileFilterUsed
	   ''' @since 1.3 </seealso>
		Public Overridable Property acceptAllFileFilterUsed As Boolean
			Get
				Return useAcceptAllFileFilter
			End Get
			Set(ByVal b As Boolean)
				Dim oldValue As Boolean = useAcceptAllFileFilter
				useAcceptAllFileFilter = b
				If Not b Then
					removeChoosableFileFilter(acceptAllFileFilter)
				Else
					removeChoosableFileFilter(acceptAllFileFilter)
					addChoosableFileFilter(acceptAllFileFilter)
				End If
				firePropertyChange(ACCEPT_ALL_FILE_FILTER_USED_CHANGED_PROPERTY, oldValue, useAcceptAllFileFilter)
			End Set
		End Property


		''' <summary>
		''' Returns the accessory component.
		''' </summary>
		''' <returns> this JFileChooser's accessory component, or null </returns>
		''' <seealso cref= #setAccessory </seealso>
		Public Overridable Property accessory As JComponent
			Get
				Return accessory
			End Get
			Set(ByVal newAccessory As JComponent)
				Dim oldValue As JComponent = accessory
				accessory = newAccessory
				firePropertyChange(ACCESSORY_CHANGED_PROPERTY, oldValue, accessory)
			End Set
		End Property


		''' <summary>
		''' Sets the <code>JFileChooser</code> to allow the user to just
		''' select files, just select
		''' directories, or select both files and directories.  The default is
		''' <code>JFilesChooser.FILES_ONLY</code>.
		''' </summary>
		''' <param name="mode"> the type of files to be displayed:
		''' <ul>
		''' <li>JFileChooser.FILES_ONLY
		''' <li>JFileChooser.DIRECTORIES_ONLY
		''' <li>JFileChooser.FILES_AND_DIRECTORIES
		''' </ul>
		''' </param>
		''' <exception cref="IllegalArgumentException">  if <code>mode</code> is an
		'''                          illegal file selection mode
		''' @beaninfo
		'''   preferred: true
		'''       bound: true
		''' description: Sets the types of files that the JFileChooser can choose.
		'''        enum: FILES_ONLY JFileChooser.FILES_ONLY
		'''              DIRECTORIES_ONLY JFileChooser.DIRECTORIES_ONLY
		'''              FILES_AND_DIRECTORIES JFileChooser.FILES_AND_DIRECTORIES
		''' 
		''' </exception>
		''' <seealso cref= #getFileSelectionMode </seealso>
		Public Overridable Property fileSelectionMode As Integer
			Set(ByVal mode As Integer)
				If fileSelectionMode = mode Then Return
    
				If (mode = FILES_ONLY) OrElse (mode = DIRECTORIES_ONLY) OrElse (mode = FILES_AND_DIRECTORIES) Then
				   Dim oldValue As Integer = fileSelectionMode
				   fileSelectionMode = mode
				   firePropertyChange(FILE_SELECTION_MODE_CHANGED_PROPERTY, oldValue, fileSelectionMode)
				Else
				   Throw New System.ArgumentException("Incorrect Mode for file selection: " & mode)
				End If
			End Set
			Get
				Return fileSelectionMode
			End Get
		End Property


		''' <summary>
		''' Convenience call that determines if files are selectable based on the
		''' current file selection mode.
		''' </summary>
		''' <seealso cref= #setFileSelectionMode </seealso>
		''' <seealso cref= #getFileSelectionMode </seealso>
		Public Overridable Property fileSelectionEnabled As Boolean
			Get
				Return ((fileSelectionMode = FILES_ONLY) OrElse (fileSelectionMode = FILES_AND_DIRECTORIES))
			End Get
		End Property

		''' <summary>
		''' Convenience call that determines if directories are selectable based
		''' on the current file selection mode.
		''' </summary>
		''' <seealso cref= #setFileSelectionMode </seealso>
		''' <seealso cref= #getFileSelectionMode </seealso>
		Public Overridable Property directorySelectionEnabled As Boolean
			Get
				Return ((fileSelectionMode = DIRECTORIES_ONLY) OrElse (fileSelectionMode = FILES_AND_DIRECTORIES))
			End Get
		End Property

		''' <summary>
		''' Sets the file chooser to allow multiple file selections.
		''' </summary>
		''' <param name="b"> true if multiple files may be selected
		''' @beaninfo
		'''       bound: true
		''' description: Sets multiple file selection mode.
		''' </param>
		''' <seealso cref= #isMultiSelectionEnabled </seealso>
		Public Overridable Property multiSelectionEnabled As Boolean
			Set(ByVal b As Boolean)
				If multiSelectionEnabled = b Then Return
				Dim oldValue As Boolean = multiSelectionEnabled
				multiSelectionEnabled = b
				firePropertyChange(MULTI_SELECTION_ENABLED_CHANGED_PROPERTY, oldValue, multiSelectionEnabled)
			End Set
			Get
				Return multiSelectionEnabled
			End Get
		End Property



		''' <summary>
		''' Returns true if hidden files are not shown in the file chooser;
		''' otherwise, returns false.
		''' </summary>
		''' <returns> the status of the file hiding property </returns>
		''' <seealso cref= #setFileHidingEnabled </seealso>
		Public Overridable Property fileHidingEnabled As Boolean
			Get
				Return useFileHiding
			End Get
			Set(ByVal b As Boolean)
				' Dump showFilesListener since we'll ignore it from now on
				If showFilesListener IsNot Nothing Then
					java.awt.Toolkit.defaultToolkit.removePropertyChangeListener(SHOW_HIDDEN_PROP, showFilesListener)
					showFilesListener = Nothing
				End If
				Dim oldValue As Boolean = useFileHiding
				useFileHiding = b
				firePropertyChange(FILE_HIDING_CHANGED_PROPERTY, oldValue, useFileHiding)
			End Set
		End Property


		''' <summary>
		''' Sets the current file filter. The file filter is used by the
		''' file chooser to filter out files from the user's view.
		''' 
		''' @beaninfo
		'''   preferred: true
		'''       bound: true
		''' description: Sets the File Filter used to filter out files of type.
		''' </summary>
		''' <param name="filter"> the new current file filter to use </param>
		''' <seealso cref= #getFileFilter </seealso>
		Public Overridable Property fileFilter As FileFilter
			Set(ByVal filter As FileFilter)
				Dim oldValue As FileFilter = fileFilter
				fileFilter = filter
				If filter IsNot Nothing Then
					If multiSelectionEnabled AndAlso selectedFiles IsNot Nothing AndAlso selectedFiles.Length > 0 Then
						Dim fList As New List(Of File)
						Dim failed As Boolean = False
						For Each file As File In selectedFiles
							If filter.accept(file) Then
								fList.Add(file)
							Else
								failed = True
							End If
						Next file
						If failed Then selectedFiles = If(fList.Count = 0, Nothing, fList.ToArray())
					ElseIf selectedFile IsNot Nothing AndAlso (Not filter.accept(selectedFile)) Then
						selectedFile = Nothing
					End If
				End If
				firePropertyChange(FILE_FILTER_CHANGED_PROPERTY, oldValue, fileFilter)
			End Set
			Get
				Return fileFilter
			End Get
		End Property



		''' <summary>
		''' Sets the file view to used to retrieve UI information, such as
		''' the icon that represents a file or the type description of a file.
		''' 
		''' @beaninfo
		'''   preferred: true
		'''       bound: true
		''' description: Sets the File View used to get file type information.
		''' </summary>
		''' <seealso cref= #getFileView </seealso>
		Public Overridable Property fileView As FileView
			Set(ByVal fileView As FileView)
				Dim oldValue As FileView = Me.fileView
				Me.fileView = fileView
				firePropertyChange(FILE_VIEW_CHANGED_PROPERTY, oldValue, fileView)
			End Set
			Get
				Return fileView
			End Get
		End Property


		' ******************************
		' *****FileView delegation *****
		' ******************************

		' NOTE: all of the following methods attempt to delegate
		' first to the client set fileView, and if <code>null</code> is returned
		' (or there is now client defined fileView) then calls the
		' UI's default fileView.

		''' <summary>
		''' Returns the filename. </summary>
		''' <param name="f"> the <code>File</code> </param>
		''' <returns> the <code>String</code> containing the filename for
		'''          <code>f</code> </returns>
		''' <seealso cref= FileView#getName </seealso>
		Public Overridable Function getName(ByVal f As java.io.File) As String
			Dim filename As String = Nothing
			If f IsNot Nothing Then
				If fileView IsNot Nothing Then filename = fileView.getName(f)

				Dim uiFileView As FileView = uI.getFileView(Me)

				If filename Is Nothing AndAlso uiFileView IsNot Nothing Then filename = uiFileView.getName(f)
			End If
			Return filename
		End Function

		''' <summary>
		''' Returns the file description. </summary>
		''' <param name="f"> the <code>File</code> </param>
		''' <returns> the <code>String</code> containing the file description for
		'''          <code>f</code> </returns>
		''' <seealso cref= FileView#getDescription </seealso>
		Public Overridable Function getDescription(ByVal f As java.io.File) As String
			Dim ___description As String = Nothing
			If f IsNot Nothing Then
				If fileView IsNot Nothing Then ___description = fileView.getDescription(f)

				Dim uiFileView As FileView = uI.getFileView(Me)

				If ___description Is Nothing AndAlso uiFileView IsNot Nothing Then ___description = uiFileView.getDescription(f)
			End If
			Return ___description
		End Function

		''' <summary>
		''' Returns the file type. </summary>
		''' <param name="f"> the <code>File</code> </param>
		''' <returns> the <code>String</code> containing the file type description for
		'''          <code>f</code> </returns>
		''' <seealso cref= FileView#getTypeDescription </seealso>
		Public Overridable Function getTypeDescription(ByVal f As java.io.File) As String
			Dim ___typeDescription As String = Nothing
			If f IsNot Nothing Then
				If fileView IsNot Nothing Then ___typeDescription = fileView.getTypeDescription(f)

				Dim uiFileView As FileView = uI.getFileView(Me)

				If ___typeDescription Is Nothing AndAlso uiFileView IsNot Nothing Then ___typeDescription = uiFileView.getTypeDescription(f)
			End If
			Return ___typeDescription
		End Function

		''' <summary>
		''' Returns the icon for this file or type of file, depending
		''' on the system. </summary>
		''' <param name="f"> the <code>File</code> </param>
		''' <returns> the <code>Icon</code> for this file, or type of file </returns>
		''' <seealso cref= FileView#getIcon </seealso>
		Public Overridable Function getIcon(ByVal f As java.io.File) As Icon
			Dim ___icon As Icon = Nothing
			If f IsNot Nothing Then
				If fileView IsNot Nothing Then ___icon = fileView.getIcon(f)

				Dim uiFileView As FileView = uI.getFileView(Me)

				If ___icon Is Nothing AndAlso uiFileView IsNot Nothing Then ___icon = uiFileView.getIcon(f)
			End If
			Return ___icon
		End Function

		''' <summary>
		''' Returns true if the file (directory) can be visited.
		''' Returns false if the directory cannot be traversed. </summary>
		''' <param name="f"> the <code>File</code> </param>
		''' <returns> true if the file/directory can be traversed, otherwise false </returns>
		''' <seealso cref= FileView#isTraversable </seealso>
		Public Overridable Function isTraversable(ByVal f As java.io.File) As Boolean
			Dim ___traversable As Boolean? = Nothing
			If f IsNot Nothing Then
				If fileView IsNot Nothing Then ___traversable = fileView.isTraversable(f)

				Dim uiFileView As FileView = uI.getFileView(Me)

				If ___traversable Is Nothing AndAlso uiFileView IsNot Nothing Then ___traversable = uiFileView.isTraversable(f)
				If ___traversable Is Nothing Then ___traversable = fileSystemView.isTraversable(f)
			End If
			Return (___traversable IsNot Nothing AndAlso ___traversable)
		End Function

		''' <summary>
		''' Returns true if the file should be displayed. </summary>
		''' <param name="f"> the <code>File</code> </param>
		''' <returns> true if the file should be displayed, otherwise false </returns>
		''' <seealso cref= FileFilter#accept </seealso>
		Public Overridable Function accept(ByVal f As java.io.File) As Boolean
			Dim shown As Boolean = True
			If f IsNot Nothing AndAlso fileFilter IsNot Nothing Then shown = fileFilter.accept(f)
			Return shown
		End Function

		''' <summary>
		''' Sets the file system view that the <code>JFileChooser</code> uses for
		''' accessing and creating file system resources, such as finding
		''' the floppy drive and getting a list of root drives. </summary>
		''' <param name="fsv">  the new <code>FileSystemView</code>
		''' 
		''' @beaninfo
		'''      expert: true
		'''       bound: true
		''' description: Sets the FileSytemView used to get filesystem information.
		''' </param>
		''' <seealso cref= FileSystemView </seealso>
		Public Overridable Property fileSystemView As FileSystemView
			Set(ByVal fsv As FileSystemView)
				Dim oldValue As FileSystemView = fileSystemView
				fileSystemView = fsv
				firePropertyChange(FILE_SYSTEM_VIEW_CHANGED_PROPERTY, oldValue, fileSystemView)
			End Set
			Get
				Return fileSystemView
			End Get
		End Property


		' **************************
		' ***** Event Handling *****
		' **************************

		''' <summary>
		''' Called by the UI when the user hits the Approve button
		''' (labeled "Open" or "Save", by default). This can also be
		''' called by the programmer.
		''' This method causes an action event to fire
		''' with the command string equal to
		''' <code>APPROVE_SELECTION</code>.
		''' </summary>
		''' <seealso cref= #APPROVE_SELECTION </seealso>
		Public Overridable Sub approveSelection()
			returnValue = APPROVE_OPTION
			If dialog IsNot Nothing Then dialog.visible = False
			fireActionPerformed(APPROVE_SELECTION)
		End Sub

		''' <summary>
		''' Called by the UI when the user chooses the Cancel button.
		''' This can also be called by the programmer.
		''' This method causes an action event to fire
		''' with the command string equal to
		''' <code>CANCEL_SELECTION</code>.
		''' </summary>
		''' <seealso cref= #CANCEL_SELECTION </seealso>
		Public Overridable Sub cancelSelection()
			returnValue = DialogResult.Cancel
			If dialog IsNot Nothing Then dialog.visible = False
			fireActionPerformed(CANCEL_SELECTION)
		End Sub

		''' <summary>
		''' Adds an <code>ActionListener</code> to the file chooser.
		''' </summary>
		''' <param name="l">  the listener to be added
		''' </param>
		''' <seealso cref= #approveSelection </seealso>
		''' <seealso cref= #cancelSelection </seealso>
		Public Overridable Sub addActionListener(ByVal l As ActionListener)
			listenerList.add(GetType(ActionListener), l)
		End Sub

		''' <summary>
		''' Removes an <code>ActionListener</code> from the file chooser.
		''' </summary>
		''' <param name="l">  the listener to be removed
		''' </param>
		''' <seealso cref= #addActionListener </seealso>
		Public Overridable Sub removeActionListener(ByVal l As ActionListener)
			listenerList.remove(GetType(ActionListener), l)
		End Sub

		''' <summary>
		''' Returns an array of all the action listeners
		''' registered on this file chooser.
		''' </summary>
		''' <returns> all of this file chooser's <code>ActionListener</code>s
		'''         or an empty
		'''         array if no action listeners are currently registered
		''' </returns>
		''' <seealso cref= #addActionListener </seealso>
		''' <seealso cref= #removeActionListener
		''' 
		''' @since 1.4 </seealso>
		Public Overridable Property actionListeners As ActionListener()
			Get
				Return listenerList.getListeners(GetType(ActionListener))
			End Get
		End Property

		''' <summary>
		''' Notifies all listeners that have registered interest for
		''' notification on this event type. The event instance
		''' is lazily created using the <code>command</code> parameter.
		''' </summary>
		''' <seealso cref= EventListenerList </seealso>
		Protected Friend Overridable Sub fireActionPerformed(ByVal command As String)
			' Guaranteed to return a non-null array
			Dim ___listeners As Object() = listenerList.listenerList
			Dim mostRecentEventTime As Long = java.awt.EventQueue.mostRecentEventTime
			Dim modifiers As Integer = 0
			Dim currentEvent As java.awt.AWTEvent = java.awt.EventQueue.currentEvent
			If TypeOf currentEvent Is InputEvent Then
				modifiers = CType(currentEvent, InputEvent).modifiers
			ElseIf TypeOf currentEvent Is ActionEvent Then
				modifiers = CType(currentEvent, ActionEvent).modifiers
			End If
			Dim e As ActionEvent = Nothing
			' Process the listeners last to first, notifying
			' those that are interested in this event
			For i As Integer = ___listeners.Length-2 To 0 Step -2
				If ___listeners(i) Is GetType(ActionListener) Then
					' Lazily create the event:
					If e Is Nothing Then e = New ActionEvent(Me, ActionEvent.ACTION_PERFORMED, command, mostRecentEventTime, modifiers)
					CType(___listeners(i+1), ActionListener).actionPerformed(e)
				End If
			Next i
		End Sub

		Private Class WeakPCL
			Implements java.beans.PropertyChangeListener

			Friend jfcRef As WeakReference(Of JFileChooser)

			Public Sub New(ByVal jfc As JFileChooser)
				jfcRef = New WeakReference(Of JFileChooser)(jfc)
			End Sub
			Public Overridable Sub propertyChange(ByVal ev As java.beans.PropertyChangeEvent)
				Debug.Assert(ev.propertyName.Equals(SHOW_HIDDEN_PROP))
				Dim jfc As JFileChooser = jfcRef.get()
				If jfc Is Nothing Then
					' Our JFileChooser is no longer around, so we no longer need to
					' listen for PropertyChangeEvents.
					java.awt.Toolkit.defaultToolkit.removePropertyChangeListener(SHOW_HIDDEN_PROP, Me)
				Else
					Dim oldValue As Boolean = jfc.useFileHiding
					jfc.useFileHiding = Not CBool(ev.newValue)
					jfc.firePropertyChange(FILE_HIDING_CHANGED_PROPERTY, oldValue, jfc.useFileHiding)
				End If
			End Sub
		End Class

		' *********************************
		' ***** Pluggable L&F methods *****
		' *********************************

		''' <summary>
		''' Resets the UI property to a value from the current look and feel.
		''' </summary>
		''' <seealso cref= JComponent#updateUI </seealso>
		Public Overrides Sub updateUI()
			If acceptAllFileFilterUsed Then removeChoosableFileFilter(acceptAllFileFilter)
			Dim ___ui As javax.swing.plaf.FileChooserUI = (CType(UIManager.getUI(Me), javax.swing.plaf.FileChooserUI))
			If fileSystemView Is Nothing Then fileSystemView = FileSystemView.fileSystemView
			uI = ___ui

			If acceptAllFileFilterUsed Then addChoosableFileFilter(acceptAllFileFilter)
		End Sub

		''' <summary>
		''' Returns a string that specifies the name of the L&amp;F class
		''' that renders this component.
		''' </summary>
		''' <returns> the string "FileChooserUI" </returns>
		''' <seealso cref= JComponent#getUIClassID </seealso>
		''' <seealso cref= UIDefaults#getUI
		''' @beaninfo
		'''        expert: true
		'''   description: A string that specifies the name of the L&amp;F class. </seealso>
		Public Property Overrides uIClassID As String
			Get
				Return uiClassID
			End Get
		End Property

		''' <summary>
		''' Gets the UI object which implements the L&amp;F for this component.
		''' </summary>
		''' <returns> the FileChooserUI object that implements the FileChooserUI L&amp;F </returns>
		Public Overridable Property uI As javax.swing.plaf.FileChooserUI
			Get
				Return CType(ui, javax.swing.plaf.FileChooserUI)
			End Get
		End Property

		''' <summary>
		''' See <code>readObject</code> and <code>writeObject</code> in
		''' <code>JComponent</code> for more
		''' information about serialization in Swing.
		''' </summary>
		Private Sub readObject(ByVal [in] As java.io.ObjectInputStream)
			[in].defaultReadObject()
			installShowFilesListener()
		End Sub

		''' <summary>
		''' See <code>readObject</code> and <code>writeObject</code> in
		''' <code>JComponent</code> for more
		''' information about serialization in Swing.
		''' </summary>
		Private Sub writeObject(ByVal s As java.io.ObjectOutputStream)
			Dim fsv As FileSystemView = Nothing

			If acceptAllFileFilterUsed Then removeChoosableFileFilter(acceptAllFileFilter)
			If fileSystemView.Equals(FileSystemView.fileSystemView) Then
				'The default FileSystemView is platform specific, it will be
				'reset by updateUI() after deserialization
				fsv = fileSystemView
				fileSystemView = Nothing
			End If
			s.defaultWriteObject()
			If fsv IsNot Nothing Then fileSystemView = fsv
			If acceptAllFileFilterUsed Then addChoosableFileFilter(acceptAllFileFilter)
			If uIClassID.Equals(uiClassID) Then
				Dim count As SByte = JComponent.getWriteObjCounter(Me)
				count -= 1
				JComponent.writeObjCounterter(Me, count)
				If count = 0 AndAlso ui IsNot Nothing Then ui.installUI(Me)
			End If
		End Sub


		''' <summary>
		''' Returns a string representation of this <code>JFileChooser</code>.
		''' This method
		''' is intended to be used only for debugging purposes, and the
		''' content and format of the returned string may vary between
		''' implementations. The returned string may be empty but may not
		''' be <code>null</code>.
		''' </summary>
		''' <returns>  a string representation of this <code>JFileChooser</code> </returns>
		Protected Friend Overrides Function paramString() As String
			Dim approveButtonTextString As String = (If(approveButtonText IsNot Nothing, approveButtonText, ""))
			Dim dialogTitleString As String = (If(dialogTitle IsNot Nothing, dialogTitle, ""))
			Dim dialogTypeString As String
			If dialogType = OPEN_DIALOG Then
				dialogTypeString = "OPEN_DIALOG"
			ElseIf dialogType = SAVE_DIALOG Then
				dialogTypeString = "SAVE_DIALOG"
			ElseIf dialogType = CUSTOM_DIALOG Then
				dialogTypeString = "CUSTOM_DIALOG"
			Else
				dialogTypeString = ""
			End If
			Dim returnValueString As String
			If returnValue = DialogResult.Cancel Then
				returnValueString = "CANCEL_OPTION"
			ElseIf returnValue = APPROVE_OPTION Then
				returnValueString = "APPROVE_OPTION"
			ElseIf returnValue = ERROR_OPTION Then
				returnValueString = "ERROR_OPTION"
			Else
				returnValueString = ""
			End If
			Dim useFileHidingString As String = (If(useFileHiding, "true", "false"))
			Dim fileSelectionModeString As String
			If fileSelectionMode = FILES_ONLY Then
				fileSelectionModeString = "FILES_ONLY"
			ElseIf fileSelectionMode = DIRECTORIES_ONLY Then
				fileSelectionModeString = "DIRECTORIES_ONLY"
			ElseIf fileSelectionMode = FILES_AND_DIRECTORIES Then
				fileSelectionModeString = "FILES_AND_DIRECTORIES"
			Else
				fileSelectionModeString = ""
			End If
			Dim currentDirectoryString As String = (If(currentDirectory IsNot Nothing, currentDirectory.ToString(), ""))
			Dim selectedFileString As String = (If(selectedFile IsNot Nothing, selectedFile.ToString(), ""))

			Return MyBase.paramString() & ",approveButtonText=" & approveButtonTextString & ",currentDirectory=" & currentDirectoryString & ",dialogTitle=" & dialogTitleString & ",dialogType=" & dialogTypeString & ",fileSelectionMode=" & fileSelectionModeString & ",returnValue=" & returnValueString & ",selectedFile=" & selectedFileString & ",useFileHiding=" & useFileHidingString
		End Function

	'///////////////
	' Accessibility support
	'//////////////

		Protected Friend ___accessibleContext As AccessibleContext = Nothing

		''' <summary>
		''' Gets the AccessibleContext associated with this JFileChooser.
		''' For file choosers, the AccessibleContext takes the form of an
		''' AccessibleJFileChooser.
		''' A new AccessibleJFileChooser instance is created if necessary.
		''' </summary>
		''' <returns> an AccessibleJFileChooser that serves as the
		'''         AccessibleContext of this JFileChooser </returns>
		Public Overridable Property accessibleContext As AccessibleContext Implements Accessible.getAccessibleContext
			Get
				If ___accessibleContext Is Nothing Then ___accessibleContext = New AccessibleJFileChooser(Me)
				Return ___accessibleContext
			End Get
		End Property

		''' <summary>
		''' This class implements accessibility support for the
		''' <code>JFileChooser</code> class.  It provides an implementation of the
		''' Java Accessibility API appropriate to file chooser user-interface
		''' elements.
		''' </summary>
		Protected Friend Class AccessibleJFileChooser
			Inherits AccessibleJComponent

			Private ReadOnly outerInstance As JFileChooser

			Public Sub New(ByVal outerInstance As JFileChooser)
				Me.outerInstance = outerInstance
			End Sub


			''' <summary>
			''' Gets the role of this object.
			''' </summary>
			''' <returns> an instance of AccessibleRole describing the role of the
			''' object </returns>
			''' <seealso cref= AccessibleRole </seealso>
			Public Overridable Property accessibleRole As AccessibleRole
				Get
					Return AccessibleRole.FILE_CHOOSER
				End Get
			End Property

		End Class ' inner class AccessibleJFileChooser

	End Class

End Namespace