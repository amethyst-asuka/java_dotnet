Imports System.Runtime.CompilerServices
Imports System.Runtime.InteropServices
Imports java.io

'
' * Copyright (c) 1995, 2013, Oracle and/or its affiliates. All rights reserved.
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
Namespace java.awt


    ''' <summary>
    ''' The <code>FileDialog</code> class displays a dialog window
    ''' from which the user can select a file.
    ''' <p>
    ''' Since it is a modal dialog, when the application calls
    ''' its <code>show</code> method to display the dialog,
    ''' it blocks the rest of the application until the user has
    ''' chosen a file.
    ''' </summary>
    ''' <seealso cref= Window#show
    ''' 
    ''' @author      Sami Shaio
    ''' @author      Arthur van Hoff
    ''' @since       JDK1.0 </seealso>
    Public Class FileDialog
        Inherits Dialog

        ''' <summary>
        ''' This constant value indicates that the purpose of the file
        ''' dialog window is to locate a file from which to read.
        ''' </summary>
        Public Const LOAD As Integer = 0

        ''' <summary>
        ''' This constant value indicates that the purpose of the file
        ''' dialog window is to locate a file to which to write.
        ''' </summary>
        Public Const SAVE As Integer = 1

        '    
        '     * There are two <code>FileDialog</code> modes: <code>LOAD</code> and
        '     * <code>SAVE</code>.
        '     * This integer will represent one or the other.
        '     * If the mode is not specified it will default to <code>LOAD</code>.
        '     *
        '     * @serial
        '     * @see getMode()
        '     * @see setMode()
        '     * @see java.awt.FileDialog#LOAD
        '     * @see java.awt.FileDialog#SAVE
        '     
        Friend _mode As Integer

        '    
        '     * The string specifying the directory to display
        '     * in the file dialog.  This variable may be <code>null</code>.
        '     *
        '     * @serial
        '     * @see getDirectory()
        '     * @see setDirectory()
        '     
        Friend _dir As String

        '    
        '     * The string specifying the initial value of the
        '     * filename text field in the file dialog.
        '     * This variable may be <code>null</code>.
        '     *
        '     * @serial
        '     * @see getFile()
        '     * @see setFile()
        '     
        Friend _file As String

        ''' <summary>
        ''' Contains the File instances for all the files that the user selects.
        ''' 
        ''' @serial </summary>
        ''' <seealso cref= #getFiles
        ''' @since 1.7 </seealso>
        Private _files As java.io.File()

        ''' <summary>
        ''' Represents whether the file dialog allows the multiple file selection.
        ''' 
        ''' @serial </summary>
        ''' <seealso cref= #setMultipleMode </seealso>
        ''' <seealso cref= #isMultipleMode
        ''' @since 1.7 </seealso>
        Private _multipleMode As Boolean = False

        '    
        '     * The filter used as the file dialog's filename filter.
        '     * The file dialog will only be displaying files whose
        '     * names are accepted by this filter.
        '     * This variable may be <code>null</code>.
        '     *
        '     * @serial
        '     * @see #getFilenameFilter()
        '     * @see #setFilenameFilter()
        '     * @see FileNameFilter
        '     
        Friend filter As java.io.FilenameFilter

        Private Const base As String = "filedlg"
        Private Shared nameCounter As Integer = 0

        '    
        '     * JDK 1.1 serialVersionUID
        '     
        Private Const serialVersionUID As Long = 5035145889651310422L


        Shared Sub New()
            ' ensure that the necessary native libraries are loaded 
            Toolkit.loadLibraries()
            If Not GraphicsEnvironment.headless Then initIDs()
            'JAVA TO VB CONVERTER TODO TASK: Anonymous inner classes are not converted to VB if the base type is not defined in the code being converted:
            '			sun.awt.AWTAccessor.setFileDialogAccessor(New sun.awt.AWTAccessor.FileDialogAccessor()
            '		{
            '				public  Sub  setFiles(FileDialog fileDialog, File files[])
            '				{
            '					fileDialog.setFiles(files);
            '				}
            '				public  Sub  setFile(FileDialog fileDialog, String file)
            '				{
            '					fileDialog.file = ("".equals(file)) ? Nothing : file;
            '				}
            '				public  Sub  setDirectory(FileDialog fileDialog, String directory)
            '				{
            '					fileDialog.dir = ("".equals(directory)) ? Nothing : directory;
            '				}
            '				public boolean isMultipleMode(FileDialog fileDialog)
            '				{
            '					synchronized(fileDialog.getObjectLock())
            '					{
            '						Return fileDialog.multipleMode;
            '					}
            '				}
            '			});
        End Sub


        ''' <summary>
        ''' Initialize JNI field and method IDs for fields that may be
        '''   accessed from C.
        ''' </summary>
        'JAVA TO VB CONVERTER TODO TASK: Replace 'unknown' with the appropriate dll name:
        <DllImport("unknown")>
        Private Shared Sub initIDs()
        End Sub

        ''' <summary>
        ''' Creates a file dialog for loading a file.  The title of the
        ''' file dialog is initially empty.  This is a convenience method for
        ''' <code>FileDialog(parent, "", LOAD)</code>.
        ''' </summary>
        ''' <param name="parent"> the owner of the dialog
        ''' @since JDK1.1 </param>
        Public Sub New(ByVal parent As Frame)
            Me.New(parent, "", LOAD)
        End Sub

        ''' <summary>
        ''' Creates a file dialog window with the specified title for loading
        ''' a file. The files shown are those in the current directory.
        ''' This is a convenience method for
        ''' <code>FileDialog(parent, title, LOAD)</code>.
        ''' </summary>
        ''' <param name="parent">   the owner of the dialog </param>
        ''' <param name="title">    the title of the dialog </param>
        Public Sub New(ByVal parent As Frame, ByVal title As String)
            Me.New(parent, title, LOAD)
        End Sub

        ''' <summary>
        ''' Creates a file dialog window with the specified title for loading
        ''' or saving a file.
        ''' <p>
        ''' If the value of <code>mode</code> is <code>LOAD</code>, then the
        ''' file dialog is finding a file to read, and the files shown are those
        ''' in the current directory.   If the value of
        ''' <code>mode</code> is <code>SAVE</code>, the file dialog is finding
        ''' a place to write a file.
        ''' </summary>
        ''' <param name="parent">   the owner of the dialog </param>
        ''' <param name="title">   the title of the dialog </param>
        ''' <param name="mode">   the mode of the dialog; either
        '''          <code>FileDialog.LOAD</code> or <code>FileDialog.SAVE</code> </param>
        ''' <exception cref="IllegalArgumentException"> if an illegal file
        '''                 dialog mode is supplied </exception>
        ''' <seealso cref=       java.awt.FileDialog#LOAD </seealso>
        ''' <seealso cref=       java.awt.FileDialog#SAVE </seealso>
        Public Sub New(ByVal parent As Frame, ByVal title As String, ByVal mode As Integer)
            MyBase.New(parent, title, True)
            Me.mode = mode
            layout = Nothing
        End Sub

        ''' <summary>
        ''' Creates a file dialog for loading a file.  The title of the
        ''' file dialog is initially empty.  This is a convenience method for
        ''' <code>FileDialog(parent, "", LOAD)</code>.
        ''' </summary>
        ''' <param name="parent">   the owner of the dialog </param>
        ''' <exception cref="java.lang.IllegalArgumentException"> if the <code>parent</code>'s
        '''            <code>GraphicsConfiguration</code>
        '''            is not from a screen device; </exception>
        ''' <exception cref="java.lang.IllegalArgumentException"> if <code>parent</code>
        '''            is <code>null</code>; this exception is always thrown when
        '''            <code>GraphicsEnvironment.isHeadless</code>
        '''            returns <code>true</code> </exception>
        ''' <seealso cref= java.awt.GraphicsEnvironment#isHeadless
        ''' @since 1.5 </seealso>
        Public Sub New(ByVal parent As Dialog)
            Me.New(parent, "", LOAD)
        End Sub

        ''' <summary>
        ''' Creates a file dialog window with the specified title for loading
        ''' a file. The files shown are those in the current directory.
        ''' This is a convenience method for
        ''' <code>FileDialog(parent, title, LOAD)</code>.
        ''' </summary>
        ''' <param name="parent">   the owner of the dialog </param>
        ''' <param name="title">    the title of the dialog; a <code>null</code> value
        '''                     will be accepted without causing a
        '''                     <code>NullPointerException</code> to be thrown </param>
        ''' <exception cref="java.lang.IllegalArgumentException"> if the <code>parent</code>'s
        '''            <code>GraphicsConfiguration</code>
        '''            is not from a screen device; </exception>
        ''' <exception cref="java.lang.IllegalArgumentException"> if <code>parent</code>
        '''            is <code>null</code>; this exception is always thrown when
        '''            <code>GraphicsEnvironment.isHeadless</code>
        '''            returns <code>true</code> </exception>
        ''' <seealso cref= java.awt.GraphicsEnvironment#isHeadless
        ''' @since     1.5 </seealso>
        Public Sub New(ByVal parent As Dialog, ByVal title As String)
            Me.New(parent, title, LOAD)
        End Sub

        ''' <summary>
        ''' Creates a file dialog window with the specified title for loading
        ''' or saving a file.
        ''' <p>
        ''' If the value of <code>mode</code> is <code>LOAD</code>, then the
        ''' file dialog is finding a file to read, and the files shown are those
        ''' in the current directory.   If the value of
        ''' <code>mode</code> is <code>SAVE</code>, the file dialog is finding
        ''' a place to write a file.
        ''' </summary>
        ''' <param name="parent">   the owner of the dialog </param>
        ''' <param name="title">    the title of the dialog; a <code>null</code> value
        '''                     will be accepted without causing a
        '''                     <code>NullPointerException</code> to be thrown </param>
        ''' <param name="mode">     the mode of the dialog; either
        '''                     <code>FileDialog.LOAD</code> or <code>FileDialog.SAVE</code> </param>
        ''' <exception cref="java.lang.IllegalArgumentException"> if an illegal
        '''            file dialog mode is supplied; </exception>
        ''' <exception cref="java.lang.IllegalArgumentException"> if the <code>parent</code>'s
        '''            <code>GraphicsConfiguration</code>
        '''            is not from a screen device; </exception>
        ''' <exception cref="java.lang.IllegalArgumentException"> if <code>parent</code>
        '''            is <code>null</code>; this exception is always thrown when
        '''            <code>GraphicsEnvironment.isHeadless</code>
        '''            returns <code>true</code> </exception>
        ''' <seealso cref=       java.awt.GraphicsEnvironment#isHeadless </seealso>
        ''' <seealso cref=       java.awt.FileDialog#LOAD </seealso>
        ''' <seealso cref=       java.awt.FileDialog#SAVE
        ''' @since     1.5 </seealso>
        Public Sub New(ByVal parent As Dialog, ByVal title As String, ByVal mode As Integer)
            MyBase.New(parent, title, True)
            Me.mode = mode
            layout = Nothing
        End Sub

        ''' <summary>
        ''' Constructs a name for this component. Called by <code>getName()</code>
        ''' when the name is <code>null</code>.
        ''' </summary>
        Friend Overrides Function constructComponentName() As String
            SyncLock GetType(FileDialog)
                Dim tempVar As Integer = nameCounter
                nameCounter += 1
                Return base + tempVar
            End SyncLock
        End Function

        ''' <summary>
        ''' Creates the file dialog's peer.  The peer allows us to change the look
        ''' of the file dialog without changing its functionality.
        ''' </summary>
        Public Overrides Sub addNotify()
            SyncLock treeLock
                If parent IsNot Nothing AndAlso parent.peer Is Nothing Then parent.addNotify()
                If peer Is Nothing Then peer = toolkit.createFileDialog(Me)
                MyBase.addNotify()
            End SyncLock
        End Sub

        ''' <summary>
        ''' Indicates whether this file dialog box is for loading from a file
        ''' or for saving to a file.
        ''' </summary>
        ''' <returns>   the mode of this file dialog window, either
        '''               <code>FileDialog.LOAD</code> or
        '''               <code>FileDialog.SAVE</code> </returns>
        ''' <seealso cref=      java.awt.FileDialog#LOAD </seealso>
        ''' <seealso cref=      java.awt.FileDialog#SAVE </seealso>
        ''' <seealso cref=      java.awt.FileDialog#setMode </seealso>
        Public Overridable Property mode As Integer
            Get
                Return mode
            End Get
            Set(ByVal mode As Integer)
                Select Case mode
                    Case LOAD, SAVE
                        Me.mode = mode
                    Case Else
                        Throw New IllegalArgumentException("illegal file dialog mode")
                End Select
            End Set
        End Property


        ''' <summary>
        ''' Gets the directory of this file dialog.
        ''' </summary>
        ''' <returns>  the (potentially <code>null</code> or invalid)
        '''          directory of this <code>FileDialog</code> </returns>
        ''' <seealso cref=       java.awt.FileDialog#setDirectory </seealso>
        Public Overridable Property directory As String
            Get
                Return _dir
            End Get
            Set(ByVal dir As String)
                Me._dir = If(dir IsNot Nothing AndAlso dir.Equals(""), Nothing, dir)
                Dim peer_Renamed As java.awt.peer.FileDialogPeer = CType(Me.peer, java.awt.peer.FileDialogPeer)
                If peer_Renamed IsNot Nothing Then peer_Renamed.directory = Me.dir
            End Set
        End Property


        ''' <summary>
        ''' Gets the selected file of this file dialog.  If the user
        ''' selected <code>CANCEL</code>, the returned file is <code>null</code>.
        ''' </summary>
        ''' <returns>    the currently selected file of this file dialog window,
        '''                or <code>null</code> if none is selected </returns>
        ''' <seealso cref=       java.awt.FileDialog#setFile </seealso>
        Public Overridable Property file As String
            Get
                Return _file
            End Get
            Set(ByVal file As String)
                Me._file = If(file IsNot Nothing AndAlso file.Equals(""), Nothing, file)
                Dim peer_Renamed As java.awt.peer.FileDialogPeer = CType(Me.peer, java.awt.peer.FileDialogPeer)
                If peer_Renamed IsNot Nothing Then peer_Renamed.file = Me.file
            End Set
        End Property

        ''' <summary>
        ''' Returns files that the user selects.
        ''' <p>
        ''' If the user cancels the file dialog,
        ''' then the method returns an empty array.
        ''' </summary>
        ''' <returns>    files that the user selects or an empty array
        '''            if the user cancels the file dialog. </returns>
        ''' <seealso cref=       #setFile(String) </seealso>
        ''' <seealso cref=       #getFile
        ''' @since 1.7 </seealso>
        Public Overridable Property files As java.io.File()
            Get
                SyncLock objectLock
                    If _files IsNot Nothing Then
                        Return _files.Clone()
                    Else
                        Return New File() {}
                    End If
                End SyncLock
            End Get
            Set(ByVal files As java.io.File())
                SyncLock objectLock
                    Me._files = files
                End SyncLock
            End Set
        End Property

        ''' <summary>
        ''' Enables or disables multiple file selection for the file dialog.
        ''' </summary>
        ''' <param name="enable">    if {@code true}, multiple file selection is enabled;
        '''                  {@code false} - disabled. </param>
        ''' <seealso cref= #isMultipleMode
        ''' @since 1.7 </seealso>
        Public Overridable Property multipleMode As Boolean
            Set(ByVal enable As Boolean)
                SyncLock objectLock
                    Me._multipleMode = enable
                End SyncLock
            End Set
            Get
                SyncLock objectLock
                    Return _multipleMode
                End SyncLock
            End Get
        End Property


        ''' <summary>
        ''' Determines this file dialog's filename filter. A filename filter
        ''' allows the user to specify which files appear in the file dialog
        ''' window.  Filename filters do not function in Sun's reference
        ''' implementation for Microsoft Windows.
        ''' </summary>
        ''' <returns>    this file dialog's filename filter </returns>
        ''' <seealso cref=       java.io.FilenameFilter </seealso>
        ''' <seealso cref=       java.awt.FileDialog#setFilenameFilter </seealso>
        Public Overridable Property filenameFilter As java.io.FilenameFilter
            Get
                Return filter
            End Get
            Set(ByVal filter As java.io.FilenameFilter)
                Me.filter = filter
                Dim peer_Renamed As java.awt.peer.FileDialogPeer = CType(Me.peer, java.awt.peer.FileDialogPeer)
                If peer_Renamed IsNot Nothing Then peer_Renamed.filenameFilter = filter
            End Set
        End Property


        ''' <summary>
        ''' Reads the <code>ObjectInputStream</code> and performs
        ''' a backwards compatibility check by converting
        ''' either a <code>dir</code> or a <code>file</code>
        ''' equal to an empty string to <code>null</code>.
        ''' </summary>
        ''' <param name="s"> the <code>ObjectInputStream</code> to read </param>
        Private Sub readObject(ByVal s As java.io.ObjectInputStream)
            s.defaultReadObject()

            ' 1.1 Compatibility: "" is not converted to null in 1.1
            If Dir() IsNot Nothing AndAlso Dir.Equals("") Then Dir = Nothing
            If file IsNot Nothing AndAlso file.Equals("") Then file = Nothing
        End Sub

        ''' <summary>
        ''' Returns a string representing the state of this <code>FileDialog</code>
        ''' window. This method is intended to be used only for debugging purposes,
        ''' and the content and format of the returned string may vary between
        ''' implementations. The returned string may be empty but may not be
        ''' <code>null</code>.
        ''' </summary>
        ''' <returns>  the parameter string of this file dialog window </returns>
        Protected Friend Overrides Function paramString() As String
            Dim str As String = MyBase.paramString()
            str &= ",dir= " & Dir()
            str &= ",file= " & file
            Return str + (If(mode = LOAD, ",load", ",save"))
        End Function

        Friend Overrides Function postsOldMouseEvents() As Boolean
            Return False
        End Function
    End Class

End Namespace