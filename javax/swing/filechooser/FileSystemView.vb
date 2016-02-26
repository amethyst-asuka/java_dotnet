Imports System
Imports System.Collections.Generic
Imports System.Threading
Imports javax.swing
Imports sun.awt.shell

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

Namespace javax.swing.filechooser



	''' <summary>
	''' FileSystemView is JFileChooser's gateway to the
	''' file system. Since the JDK1.1 File API doesn't allow
	''' access to such information as root partitions, file type
	''' information, or hidden file bits, this class is designed
	''' to intuit as much OS-specific file system information as
	''' possible.
	''' 
	''' <p>
	''' 
	''' Java Licensees may want to provide a different implementation of
	''' FileSystemView to better handle a given operating system.
	''' 
	''' @author Jeff Dinkins
	''' </summary>

	' PENDING(jeff) - need to provide a specification for
	' how Mac/OS2/BeOS/etc file systems can modify FileSystemView
	' to handle their particular type of file system.

	Public MustInherit Class FileSystemView

		Friend Shared windowsFileSystemView As FileSystemView = Nothing
		Friend Shared unixFileSystemView As FileSystemView = Nothing
		'static FileSystemView macFileSystemView = null;
		Friend Shared genericFileSystemView As FileSystemView = Nothing

		Private useSystemExtensionHiding As Boolean = UIManager.defaults.getBoolean("FileChooser.useSystemExtensionHiding")

		Public Property Shared fileSystemView As FileSystemView
			Get
				If System.IO.Path.DirectorySeparatorChar = "\"c Then
					If windowsFileSystemView Is Nothing Then windowsFileSystemView = New WindowsFileSystemView
					Return windowsFileSystemView
				End If
    
				If System.IO.Path.DirectorySeparatorChar = "/"c Then
					If unixFileSystemView Is Nothing Then unixFileSystemView = New UnixFileSystemView
					Return unixFileSystemView
				End If
    
				' if(File.separatorChar == ':') {
				'    if(macFileSystemView == null) {
				'      macFileSystemView = new MacFileSystemView();
				'    }
				'    return macFileSystemView;
				'}
    
				If genericFileSystemView Is Nothing Then genericFileSystemView = New GenericFileSystemView
				Return genericFileSystemView
			End Get
		End Property

		Public Sub New()
			Dim weakReference As New WeakReference(Of FileSystemView)(Me)

'JAVA TO VB CONVERTER TODO TASK: Anonymous inner classes are not converted to VB if the base type is not defined in the code being converted:
'			UIManager.addPropertyChangeListener(New java.beans.PropertyChangeListener()
	'		{
	'			public void propertyChange(PropertyChangeEvent evt)
	'			{
	'				FileSystemView fileSystemView = weakReference.get();
	'
	'				if (fileSystemView == Nothing)
	'				{
	'					' FileSystemView was destroyed
	'					UIManager.removePropertyChangeListener(Me);
	'				}
	'				else
	'				{
	'					if (evt.getPropertyName().equals("lookAndFeel"))
	'					{
	'						fileSystemView.useSystemExtensionHiding = UIManager.getDefaults().getBoolean("FileChooser.useSystemExtensionHiding");
	'					}
	'				}
	'			}
	'		});
		End Sub

		''' <summary>
		''' Determines if the given file is a root in the navigable tree(s).
		''' Examples: Windows 98 has one root, the Desktop folder. DOS has one root
		''' per drive letter, <code>C:\</code>, <code>D:\</code>, etc. Unix has one root,
		''' the <code>"/"</code> directory.
		''' 
		''' The default implementation gets information from the <code>ShellFolder</code> class.
		''' </summary>
		''' <param name="f"> a <code>File</code> object representing a directory </param>
		''' <returns> <code>true</code> if <code>f</code> is a root in the navigable tree. </returns>
		''' <seealso cref= #isFileSystemRoot </seealso>
		Public Overridable Function isRoot(ByVal f As java.io.File) As Boolean
			If f Is Nothing OrElse (Not f.absolute) Then Return False

			Dim ___roots As File() = roots
			For Each ___root As File In ___roots
				If ___root.Equals(f) Then Return True
			Next ___root
			Return False
		End Function

		''' <summary>
		''' Returns true if the file (directory) can be visited.
		''' Returns false if the directory cannot be traversed.
		''' </summary>
		''' <param name="f"> the <code>File</code> </param>
		''' <returns> <code>true</code> if the file/directory can be traversed, otherwise <code>false</code> </returns>
		''' <seealso cref= JFileChooser#isTraversable </seealso>
		''' <seealso cref= FileView#isTraversable
		''' @since 1.4 </seealso>
		Public Overridable Function isTraversable(ByVal f As java.io.File) As Boolean?
			Return Convert.ToBoolean(f.directory)
		End Function

		''' <summary>
		''' Name of a file, directory, or folder as it would be displayed in
		''' a system file browser. Example from Windows: the "M:\" directory
		''' displays as "CD-ROM (M:)"
		''' 
		''' The default implementation gets information from the ShellFolder class.
		''' </summary>
		''' <param name="f"> a <code>File</code> object </param>
		''' <returns> the file name as it would be displayed by a native file chooser </returns>
		''' <seealso cref= JFileChooser#getName
		''' @since 1.4 </seealso>
		Public Overridable Function getSystemDisplayName(ByVal f As java.io.File) As String
			If f Is Nothing Then Return Nothing

			Dim name As String = f.name

			If (Not name.Equals("..")) AndAlso (Not name.Equals(".")) AndAlso (useSystemExtensionHiding OrElse (Not isFileSystem(f)) OrElse isFileSystemRoot(f)) AndAlso (TypeOf f Is ShellFolder OrElse f.exists()) Then

				Try
					name = getShellFolder(f).displayName
				Catch e As java.io.FileNotFoundException
					Return Nothing
				End Try

				If name Is Nothing OrElse name.Length = 0 Then name = f.path ' e.g. "/"
			End If

			Return name
		End Function

		''' <summary>
		''' Type description for a file, directory, or folder as it would be displayed in
		''' a system file browser. Example from Windows: the "Desktop" folder
		''' is described as "Desktop".
		''' 
		''' Override for platforms with native ShellFolder implementations.
		''' </summary>
		''' <param name="f"> a <code>File</code> object </param>
		''' <returns> the file type description as it would be displayed by a native file chooser
		''' or null if no native information is available. </returns>
		''' <seealso cref= JFileChooser#getTypeDescription
		''' @since 1.4 </seealso>
		Public Overridable Function getSystemTypeDescription(ByVal f As java.io.File) As String
			Return Nothing
		End Function

		''' <summary>
		''' Icon for a file, directory, or folder as it would be displayed in
		''' a system file browser. Example from Windows: the "M:\" directory
		''' displays a CD-ROM icon.
		''' 
		''' The default implementation gets information from the ShellFolder class.
		''' </summary>
		''' <param name="f"> a <code>File</code> object </param>
		''' <returns> an icon as it would be displayed by a native file chooser </returns>
		''' <seealso cref= JFileChooser#getIcon
		''' @since 1.4 </seealso>
		Public Overridable Function getSystemIcon(ByVal f As java.io.File) As Icon
			If f Is Nothing Then Return Nothing

			Dim sf As ShellFolder

			Try
				sf = getShellFolder(f)
			Catch e As java.io.FileNotFoundException
				Return Nothing
			End Try

			Dim img As java.awt.Image = sf.getIcon(False)

			If img IsNot Nothing Then
				Return New ImageIcon(img, sf.folderType)
			Else
				Return UIManager.getIcon(If(f.directory, "FileView.directoryIcon", "FileView.fileIcon"))
			End If
		End Function

		''' <summary>
		''' On Windows, a file can appear in multiple folders, other than its
		''' parent directory in the filesystem. Folder could for example be the
		''' "Desktop" folder which is not the same as file.getParentFile().
		''' </summary>
		''' <param name="folder"> a <code>File</code> object representing a directory or special folder </param>
		''' <param name="file"> a <code>File</code> object </param>
		''' <returns> <code>true</code> if <code>folder</code> is a directory or special folder and contains <code>file</code>.
		''' @since 1.4 </returns>
		Public Overridable Function isParent(ByVal folder As java.io.File, ByVal file As java.io.File) As Boolean
			If folder Is Nothing OrElse file Is Nothing Then
				Return False
			ElseIf TypeOf folder Is ShellFolder Then
					Dim ___parent As File = file.parentFile
					If ___parent IsNot Nothing AndAlso ___parent.Equals(folder) Then Return True
				Dim children As File() = getFiles(folder, False)
				For Each ___child As File In children
					If file.Equals(___child) Then Return True
				Next ___child
				Return False
			Else
				Return folder.Equals(file.parentFile)
			End If
		End Function

		''' 
		''' <param name="parent"> a <code>File</code> object representing a directory or special folder </param>
		''' <param name="fileName"> a name of a file or folder which exists in <code>parent</code> </param>
		''' <returns> a File object. This is normally constructed with <code>new
		''' File(parent, fileName)</code> except when parent and child are both
		''' special folders, in which case the <code>File</code> is a wrapper containing
		''' a <code>ShellFolder</code> object.
		''' @since 1.4 </returns>
		Public Overridable Function getChild(ByVal parent As java.io.File, ByVal fileName As String) As java.io.File
			If TypeOf parent Is ShellFolder Then
				Dim children As File() = getFiles(parent, False)
				For Each ___child As File In children
					If ___child.name.Equals(fileName) Then Return ___child
				Next ___child
			End If
			Return createFileObject(parent, fileName)
		End Function


		''' <summary>
		''' Checks if <code>f</code> represents a real directory or file as opposed to a
		''' special folder such as <code>"Desktop"</code>. Used by UI classes to decide if
		''' a folder is selectable when doing directory choosing.
		''' </summary>
		''' <param name="f"> a <code>File</code> object </param>
		''' <returns> <code>true</code> if <code>f</code> is a real file or directory.
		''' @since 1.4 </returns>
		Public Overridable Function isFileSystem(ByVal f As java.io.File) As Boolean
			If TypeOf f Is ShellFolder Then
				Dim sf As ShellFolder = CType(f, ShellFolder)
				' Shortcuts to directories are treated as not being file system objects,
				' so that they are never returned by JFileChooser.
				Return sf.fileSystem AndAlso Not(sf.link AndAlso sf.directory)
			Else
				Return True
			End If
		End Function

		''' <summary>
		''' Creates a new folder with a default folder name.
		''' </summary>
		Public MustOverride Function createNewFolder(ByVal containingDir As java.io.File) As java.io.File

		''' <summary>
		''' Returns whether a file is hidden or not.
		''' </summary>
		Public Overridable Function isHiddenFile(ByVal f As java.io.File) As Boolean
			Return f.hidden
		End Function


		''' <summary>
		''' Is dir the root of a tree in the file system, such as a drive
		''' or partition. Example: Returns true for "C:\" on Windows 98.
		''' </summary>
		''' <param name="dir"> a <code>File</code> object representing a directory </param>
		''' <returns> <code>true</code> if <code>f</code> is a root of a filesystem </returns>
		''' <seealso cref= #isRoot
		''' @since 1.4 </seealso>
		Public Overridable Function isFileSystemRoot(ByVal dir As java.io.File) As Boolean
			Return ShellFolder.isFileSystemRoot(dir)
		End Function

		''' <summary>
		''' Used by UI classes to decide whether to display a special icon
		''' for drives or partitions, e.g. a "hard disk" icon.
		''' 
		''' The default implementation has no way of knowing, so always returns false.
		''' </summary>
		''' <param name="dir"> a directory </param>
		''' <returns> <code>false</code> always
		''' @since 1.4 </returns>
		Public Overridable Function isDrive(ByVal dir As java.io.File) As Boolean
			Return False
		End Function

		''' <summary>
		''' Used by UI classes to decide whether to display a special icon
		''' for a floppy disk. Implies isDrive(dir).
		''' 
		''' The default implementation has no way of knowing, so always returns false.
		''' </summary>
		''' <param name="dir"> a directory </param>
		''' <returns> <code>false</code> always
		''' @since 1.4 </returns>
		Public Overridable Function isFloppyDrive(ByVal dir As java.io.File) As Boolean
			Return False
		End Function

		''' <summary>
		''' Used by UI classes to decide whether to display a special icon
		''' for a computer node, e.g. "My Computer" or a network server.
		''' 
		''' The default implementation has no way of knowing, so always returns false.
		''' </summary>
		''' <param name="dir"> a directory </param>
		''' <returns> <code>false</code> always
		''' @since 1.4 </returns>
		Public Overridable Function isComputerNode(ByVal dir As java.io.File) As Boolean
			Return ShellFolder.isComputerNode(dir)
		End Function


		''' <summary>
		''' Returns all root partitions on this system. For example, on
		''' Windows, this would be the "Desktop" folder, while on DOS this
		''' would be the A: through Z: drives.
		''' </summary>
		Public Overridable Property roots As java.io.File()
			Get
				' Don't cache this array, because filesystem might change
				Dim ___roots As File() = CType(ShellFolder.get("roots"), File())
    
				For i As Integer = 0 To ___roots.Length - 1
					If isFileSystemRoot(___roots(i)) Then ___roots(i) = createFileSystemRoot(___roots(i))
				Next i
				Return ___roots
			End Get
		End Property


		' Providing default implementations for the remaining methods
		' because most OS file systems will likely be able to use this
		' code. If a given OS can't, override these methods in its
		' implementation.

		Public Overridable Property homeDirectory As java.io.File
			Get
				Return createFileObject(System.getProperty("user.home"))
			End Get
		End Property

		''' <summary>
		''' Return the user's default starting directory for the file chooser.
		''' </summary>
		''' <returns> a <code>File</code> object representing the default
		'''         starting folder
		''' @since 1.4 </returns>
		Public Overridable Property defaultDirectory As java.io.File
			Get
				Dim f As File = CType(ShellFolder.get("fileChooserDefaultFolder"), File)
				If isFileSystemRoot(f) Then f = createFileSystemRoot(f)
				Return f
			End Get
		End Property

		''' <summary>
		''' Returns a File object constructed in dir from the given filename.
		''' </summary>
		Public Overridable Function createFileObject(ByVal dir As java.io.File, ByVal filename As String) As java.io.File
			If dir Is Nothing Then
				Return New File(filename)
			Else
				Return New File(dir, filename)
			End If
		End Function

		''' <summary>
		''' Returns a File object constructed from the given path string.
		''' </summary>
		Public Overridable Function createFileObject(ByVal path As String) As java.io.File
			Dim f As New File(path)
			If isFileSystemRoot(f) Then f = createFileSystemRoot(f)
			Return f
		End Function


		''' <summary>
		''' Gets the list of shown (i.e. not hidden) files.
		''' </summary>
		Public Overridable Function getFiles(ByVal dir As java.io.File, ByVal useFileHiding As Boolean) As java.io.File()
			Dim ___files As IList(Of File) = New List(Of File)

			' add all files in dir
			If Not(TypeOf dir Is ShellFolder) Then
				Try
					dir = getShellFolder(dir)
				Catch e As java.io.FileNotFoundException
					Return New File(){}
				End Try
			End If

			Dim names As File() = CType(dir, ShellFolder).listFiles((Not useFileHiding))

			If names Is Nothing Then Return New File(){}

			For Each f As File In names
				If Thread.CurrentThread.interrupted Then Exit For

				If Not(TypeOf f Is ShellFolder) Then
					If isFileSystemRoot(f) Then f = createFileSystemRoot(f)
					Try
						f = ShellFolder.getShellFolder(f)
					Catch e As java.io.FileNotFoundException
						' Not a valid file (wouldn't show in native file chooser)
						' Example: C:\pagefile.sys
						Continue For
					Catch e As InternalError
						' Not a valid file (wouldn't show in native file chooser)
						' Example C:\Winnt\Profiles\joe\history\History.IE5
						Continue For
					End Try
				End If
				If (Not useFileHiding) OrElse (Not isHiddenFile(f)) Then ___files.Add(f)
			Next f

			Return ___files.ToArray()
		End Function



		''' <summary>
		''' Returns the parent directory of <code>dir</code>. </summary>
		''' <param name="dir"> the <code>File</code> being queried </param>
		''' <returns> the parent directory of <code>dir</code>, or
		'''   <code>null</code> if <code>dir</code> is <code>null</code> </returns>
		Public Overridable Function getParentDirectory(ByVal dir As java.io.File) As java.io.File
			If dir Is Nothing OrElse (Not dir.exists()) Then Return Nothing

			Dim sf As ShellFolder

			Try
				sf = getShellFolder(dir)
			Catch e As java.io.FileNotFoundException
				Return Nothing
			End Try

			Dim psf As File = sf.parentFile

			If psf Is Nothing Then Return Nothing

			If isFileSystem(psf) Then
				Dim f As File = psf
				If Not f.exists() Then
					' This could be a node under "Network Neighborhood".
					Dim ppsf As File = psf.parentFile
					If ppsf Is Nothing OrElse (Not isFileSystem(ppsf)) Then f = createFileSystemRoot(f)
				End If
				Return f
			Else
				Return psf
			End If
		End Function

		''' <summary>
		''' Throws {@code FileNotFoundException} if file not found or current thread was interrupted
		''' </summary>
		Friend Overridable Function getShellFolder(ByVal f As java.io.File) As ShellFolder
			If Not(TypeOf f Is ShellFolder) AndAlso Not(TypeOf f Is FileSystemRoot) AndAlso isFileSystemRoot(f) Then f = createFileSystemRoot(f)

			Try
				Return ShellFolder.getShellFolder(f)
			Catch e As InternalError
				Console.Error.WriteLine("FileSystemView.getShellFolder: f=" & f)
				e.printStackTrace()
				Return Nothing
			End Try
		End Function

		''' <summary>
		''' Creates a new <code>File</code> object for <code>f</code> with correct
		''' behavior for a file system root directory.
		''' </summary>
		''' <param name="f"> a <code>File</code> object representing a file system root
		'''          directory, for example "/" on Unix or "C:\" on Windows. </param>
		''' <returns> a new <code>File</code> object
		''' @since 1.4 </returns>
		Protected Friend Overridable Function createFileSystemRoot(ByVal f As java.io.File) As java.io.File
			Return New FileSystemRoot(f)
		End Function




		Friend Class FileSystemRoot
			Inherits java.io.File

			Public Sub New(ByVal f As java.io.File)
				MyBase.New(f,"")
			End Sub

			Public Sub New(ByVal s As String)
				MyBase.New(s)
			End Sub

			Public Overridable Property directory As Boolean
				Get
					Return True
				End Get
			End Property

			Public Overridable Property name As String
				Get
					Return path
				End Get
			End Property
		End Class
	End Class

	''' <summary>
	''' FileSystemView that handles some specific unix-isms.
	''' </summary>
	Friend Class UnixFileSystemView
		Inherits FileSystemView

		Private Shared ReadOnly newFolderString As String = UIManager.getString("FileChooser.other.newFolder")
		Private Shared ReadOnly newFolderNextString As String = UIManager.getString("FileChooser.other.newFolder.subsequent")

		''' <summary>
		''' Creates a new folder with a default folder name.
		''' </summary>
		Public Overrides Function createNewFolder(ByVal containingDir As java.io.File) As java.io.File
			If containingDir Is Nothing Then Throw New java.io.IOException("Containing directory is null:")
			Dim newFolder As File
			' Unix - using OpenWindows' default folder name. Can't find one for Motif/CDE.
			newFolder = createFileObject(containingDir, newFolderString)
			Dim i As Integer = 1
			Do While newFolder.exists() AndAlso i < 100
				newFolder = createFileObject(containingDir, java.text.MessageFormat.format(newFolderNextString, New Integer?(i)))
				i += 1
			Loop

			If newFolder.exists() Then
				Throw New java.io.IOException("Directory already exists:" & newFolder.absolutePath)
			Else
				newFolder.mkdirs()
			End If

			Return newFolder
		End Function

		Public Overrides Function isFileSystemRoot(ByVal dir As java.io.File) As Boolean
			Return dir IsNot Nothing AndAlso dir.absolutePath.Equals("/")
		End Function

		Public Overrides Function isDrive(ByVal dir As java.io.File) As Boolean
			Return isFloppyDrive(dir)
		End Function

		Public Overrides Function isFloppyDrive(ByVal dir As java.io.File) As Boolean
			' Could be looking at the path for Solaris, but wouldn't be reliable.
			' For example:
			' return (dir != null && dir.getAbsolutePath().toLowerCase().startsWith("/floppy"));
			Return False
		End Function

		Public Overrides Function isComputerNode(ByVal dir As java.io.File) As Boolean
			If dir IsNot Nothing Then
				Dim ___parent As String = dir.parent
				If ___parent IsNot Nothing AndAlso ___parent.Equals("/net") Then Return True
			End If
			Return False
		End Function
	End Class


	''' <summary>
	''' FileSystemView that handles some specific windows concepts.
	''' </summary>
	Friend Class WindowsFileSystemView
		Inherits FileSystemView

		Private Shared ReadOnly newFolderString As String = UIManager.getString("FileChooser.win32.newFolder")
		Private Shared ReadOnly newFolderNextString As String = UIManager.getString("FileChooser.win32.newFolder.subsequent")

		Public Overrides Function isTraversable(ByVal f As java.io.File) As Boolean?
			Return Convert.ToBoolean(isFileSystemRoot(f) OrElse isComputerNode(f) OrElse f.directory)
		End Function

		Public Overrides Function getChild(ByVal parent As java.io.File, ByVal fileName As String) As java.io.File
			If fileName.StartsWith("\") AndAlso (Not fileName.StartsWith("\\")) AndAlso isFileSystem(parent) Then

				'Path is relative to the root of parent's drive
				Dim path As String = parent.absolutePath
				If path.Length >= 2 AndAlso path.Chars(1) = ":"c AndAlso Char.IsLetter(path.Chars(0)) Then Return createFileObject(path.Substring(0, 2) + fileName)
			End If
			Return MyBase.getChild(parent, fileName)
		End Function

		''' <summary>
		''' Type description for a file, directory, or folder as it would be displayed in
		''' a system file browser. Example from Windows: the "Desktop" folder
		''' is described as "Desktop".
		''' 
		''' The Windows implementation gets information from the ShellFolder class.
		''' </summary>
		Public Overrides Function getSystemTypeDescription(ByVal f As java.io.File) As String
			If f Is Nothing Then Return Nothing

			Try
				Return getShellFolder(f).folderType
			Catch e As java.io.FileNotFoundException
				Return Nothing
			End Try
		End Function

		''' <returns> the Desktop folder. </returns>
		Public Property Overrides homeDirectory As java.io.File
			Get
				Dim ___roots As File() = roots
				Return If(___roots.Length = 0, Nothing, ___roots(0))
			End Get
		End Property

		''' <summary>
		''' Creates a new folder with a default folder name.
		''' </summary>
		Public Overrides Function createNewFolder(ByVal containingDir As java.io.File) As java.io.File
			If containingDir Is Nothing Then Throw New java.io.IOException("Containing directory is null:")
			' Using NT's default folder name
			Dim newFolder As File = createFileObject(containingDir, newFolderString)
			Dim i As Integer = 2
			Do While newFolder.exists() AndAlso i < 100
				newFolder = createFileObject(containingDir, java.text.MessageFormat.format(newFolderNextString, New Integer?(i)))
				i += 1
			Loop

			If newFolder.exists() Then
				Throw New java.io.IOException("Directory already exists:" & newFolder.absolutePath)
			Else
				newFolder.mkdirs()
			End If

			Return newFolder
		End Function

		Public Overrides Function isDrive(ByVal dir As java.io.File) As Boolean
			Return isFileSystemRoot(dir)
		End Function

		Public Overrides Function isFloppyDrive(ByVal dir As java.io.File) As Boolean
'JAVA TO VB CONVERTER TODO TASK: Anonymous inner classes are not converted to VB if the base type is not defined in the code being converted:
'			String path = java.security.AccessController.doPrivileged(New java.security.PrivilegedAction<String>()
	'		{
	'			public String run()
	'			{
	'				Return dir.getAbsolutePath();
	'			}
	'		});

			Return path IsNot Nothing AndAlso (path.Equals("A:\") OrElse path.Equals("B:\"))
		End Function

		''' <summary>
		''' Returns a File object constructed from the given path string.
		''' </summary>
		Public Overrides Function createFileObject(ByVal path As String) As java.io.File
			' Check for missing backslash after drive letter such as "C:" or "C:filename"
			If path.Length >= 2 AndAlso path.Chars(1) = ":"c AndAlso Char.IsLetter(path.Chars(0)) Then
				If path.Length = 2 Then
					path &= "\"
				ElseIf path.Chars(2) <> "\"c Then
					path = path.Substring(0, 2) & "\" & path.Substring(2)
				End If
			End If
			Return MyBase.createFileObject(path)
		End Function

		Protected Friend Overrides Function createFileSystemRoot(ByVal f As java.io.File) As java.io.File
			' Problem: Removable drives on Windows return false on f.exists()
			' Workaround: Override exists() to always return true.
			Return New FileSystemRootAnonymousInnerClassHelper
		End Function

		Private Class FileSystemRootAnonymousInnerClassHelper
			Inherits FileSystemRoot

			Public Overridable Function exists() As Boolean
				Return True
			End Function
		End Class

	End Class

	''' <summary>
	''' Fallthrough FileSystemView in case we can't determine the OS.
	''' </summary>
	Friend Class GenericFileSystemView
		Inherits FileSystemView

		Private Shared ReadOnly newFolderString As String = UIManager.getString("FileChooser.other.newFolder")

		''' <summary>
		''' Creates a new folder with a default folder name.
		''' </summary>
		Public Overrides Function createNewFolder(ByVal containingDir As java.io.File) As java.io.File
			If containingDir Is Nothing Then Throw New java.io.IOException("Containing directory is null:")
			' Using NT's default folder name
			Dim newFolder As File = createFileObject(containingDir, newFolderString)

			If newFolder.exists() Then
				Throw New java.io.IOException("Directory already exists:" & newFolder.absolutePath)
			Else
				newFolder.mkdirs()
			End If

			Return newFolder
		End Function

	End Class

End Namespace