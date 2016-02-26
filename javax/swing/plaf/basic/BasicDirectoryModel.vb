Imports System.Runtime.CompilerServices
Imports System.Collections
Imports System.Collections.Generic
Imports javax.swing
Imports javax.swing.filechooser
Imports javax.swing.event

'
' * Copyright (c) 1998, 2011, Oracle and/or its affiliates. All rights reserved.
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
	''' Basic implementation of a file list.
	''' 
	''' @author Jeff Dinkins
	''' </summary>
	Public Class BasicDirectoryModel
		Inherits AbstractListModel(Of Object)
		Implements PropertyChangeListener

		Private filechooser As JFileChooser = Nothing
		' PENDING(jeff) pick the size more sensibly
		Private fileCache As New List(Of java.io.File)(50)
		Private loadThread As LoadFilesThread = Nothing
		Private files As List(Of java.io.File) = Nothing
		Private directories As List(Of java.io.File) = Nothing
		Private fetchID As Integer = 0

		Private changeSupport As PropertyChangeSupport

		Private busy As Boolean = False

		Public Sub New(ByVal filechooser As JFileChooser)
			Me.filechooser = filechooser
			validateFileCache()
		End Sub

		Public Overridable Sub propertyChange(ByVal e As PropertyChangeEvent)
			Dim prop As String = e.propertyName
			If prop = JFileChooser.DIRECTORY_CHANGED_PROPERTY OrElse prop = JFileChooser.FILE_VIEW_CHANGED_PROPERTY OrElse prop = JFileChooser.FILE_FILTER_CHANGED_PROPERTY OrElse prop = JFileChooser.FILE_HIDING_CHANGED_PROPERTY OrElse prop = JFileChooser.FILE_SELECTION_MODE_CHANGED_PROPERTY Then
				validateFileCache()
			ElseIf "UI".Equals(prop) Then
				Dim old As Object = e.oldValue
				If TypeOf old Is BasicFileChooserUI Then
					Dim ui As BasicFileChooserUI = CType(old, BasicFileChooserUI)
					Dim model As BasicDirectoryModel = ui.model
					If model IsNot Nothing Then model.invalidateFileCache()
				End If
			ElseIf "JFileChooserDialogIsClosingProperty".Equals(prop) Then
				invalidateFileCache()
			End If
		End Sub

		''' <summary>
		''' This method is used to interrupt file loading thread.
		''' </summary>
		Public Overridable Sub invalidateFileCache()
			If loadThread IsNot Nothing Then
				loadThread.Interrupt()
				loadThread.cancelRunnables()
				loadThread = Nothing
			End If
		End Sub

		Public Overridable Property directories As List(Of java.io.File)
			Get
				SyncLock fileCache
					If directories IsNot Nothing Then Return directories
					Dim fls As ArrayList = files
					Return directories
				End SyncLock
			End Get
		End Property

		Public Overridable Property files As List(Of java.io.File)
			Get
				SyncLock fileCache
					If files IsNot Nothing Then Return files
					files = New List(Of File)
					directories = New List(Of File)
					directories.Add(filechooser.fileSystemView.createFileObject(filechooser.currentDirectory, ".."))
    
					For i As Integer = 0 To size - 1
						Dim f As File = fileCache(i)
						If filechooser.isTraversable(f) Then
							directories.Add(f)
						Else
							files.Add(f)
						End If
					Next i
					Return files
				End SyncLock
			End Get
		End Property

		Public Overridable Sub validateFileCache()
			Dim currentDirectory As File = filechooser.currentDirectory
			If currentDirectory Is Nothing Then Return
			If loadThread IsNot Nothing Then
				loadThread.Interrupt()
				loadThread.cancelRunnables()
			End If

			fetchID += 1
			busyusy(True, fetchID)

			loadThread = New LoadFilesThread(Me, currentDirectory, fetchID)
			loadThread.Start()
		End Sub

		''' <summary>
		''' Renames a file in the underlying file system.
		''' </summary>
		''' <param name="oldFile"> a <code>File</code> object representing
		'''        the existing file </param>
		''' <param name="newFile"> a <code>File</code> object representing
		'''        the desired new file name </param>
		''' <returns> <code>true</code> if rename succeeded,
		'''        otherwise <code>false</code>
		''' @since 1.4 </returns>
		Public Overridable Function renameFile(ByVal oldFile As java.io.File, ByVal newFile As java.io.File) As Boolean
			SyncLock fileCache
				If oldFile.renameTo(newFile) Then
					validateFileCache()
					Return True
				End If
				Return False
			End SyncLock
		End Function


		Public Overridable Sub fireContentsChanged()
			' System.out.println("BasicDirectoryModel: firecontentschanged");
			fireContentsChanged(Me, 0, size-1)
		End Sub

		Public Overridable Property size As Integer
			Get
				Return fileCache.Count
			End Get
		End Property

		Public Overridable Function contains(ByVal o As Object) As Boolean
			Return fileCache.Contains(o)
		End Function

		Public Overridable Function indexOf(ByVal o As Object) As Integer
			Return fileCache.IndexOf(o)
		End Function

		Public Overridable Function getElementAt(ByVal index As Integer) As Object
			Return fileCache(index)
		End Function

		''' <summary>
		''' Obsolete - not used.
		''' </summary>
		Public Overridable Sub intervalAdded(ByVal e As ListDataEvent)
		End Sub

		''' <summary>
		''' Obsolete - not used.
		''' </summary>
		Public Overridable Sub intervalRemoved(ByVal e As ListDataEvent)
		End Sub

		Protected Friend Overridable Sub sort(Of T1 As java.io.File)(ByVal v As List(Of T1))
			sun.awt.shell.ShellFolder.sort(v)
		End Sub

		' Obsolete - not used
		Protected Friend Overridable Function lt(ByVal a As java.io.File, ByVal b As java.io.File) As Boolean
			' First ignore case when comparing
			Dim diff As Integer = a.name.ToLower().CompareTo(b.name.ToLower())
			If diff <> 0 Then
				Return diff < 0
			Else
				' May differ in case (e.g. "mail" vs. "Mail")
				Return a.name.CompareTo(b.name) < 0
			End If
		End Function


		Friend Class LoadFilesThread
			Inherits System.Threading.Thread

			Private ReadOnly outerInstance As BasicDirectoryModel

			Friend currentDirectory As java.io.File = Nothing
			Friend fid As Integer
			Friend runnables As New List(Of DoChangeContents)(10)

			Public Sub New(ByVal outerInstance As BasicDirectoryModel, ByVal currentDirectory As java.io.File, ByVal fid As Integer)
					Me.outerInstance = outerInstance
				MyBase.New("Basic L&F File Loading Thread")
				Me.currentDirectory = currentDirectory
				Me.fid = fid
			End Sub

			Public Overridable Sub run()
				run0()
				outerInstance.busyusy(False, fid)
			End Sub

			Public Overridable Sub run0()
				Dim fileSystem As FileSystemView = outerInstance.filechooser.fileSystemView

				If interrupted Then Return

				Dim list As File() = fileSystem.getFiles(currentDirectory, outerInstance.filechooser.fileHidingEnabled)

				If interrupted Then Return

				Dim newFileCache As New List(Of File)
				Dim newFiles As New List(Of File)

				' run through the file list, add directories and selectable files to fileCache
				' Note that this block must be OUTSIDE of Invoker thread because of
				' deadlock possibility with custom synchronized FileSystemView
				For Each file As File In list
					If outerInstance.filechooser.accept(file) Then
						Dim isTraversable As Boolean = outerInstance.filechooser.isTraversable(file)

						If isTraversable Then
							newFileCache.Add(file)
						ElseIf outerInstance.filechooser.fileSelectionEnabled Then
							newFiles.Add(file)
						End If

						If interrupted Then Return
					End If
				Next file

				' First sort alphabetically by filename
				outerInstance.sort(newFileCache)
				outerInstance.sort(newFiles)

				newFileCache.AddRange(newFiles)

				' To avoid loads of synchronizations with Invoker and improve performance we
				' execute the whole block on the COM thread
'JAVA TO VB CONVERTER TODO TASK: Anonymous inner classes are not converted to VB if the base type is not defined in the code being converted:
'				DoChangeContents doChangeContents = sun.awt.shell.ShellFolder.invoke(New java.util.concurrent.Callable<DoChangeContents>()
	'			{
	'				public DoChangeContents call()
	'				{
	'					int newSize = newFileCache.size();
	'					int oldSize = fileCache.size();
	'
	'					if (newSize > oldSize)
	'					{
	'						'see if interval is added
	'						int start = oldSize;
	'						int end = newSize;
	'						for (int i = 0; i < oldSize; i += 1)
	'						{
	'							if (!newFileCache.get(i).equals(fileCache.get(i)))
	'							{
	'								start = i;
	'								for (int j = i; j < newSize; j += 1)
	'								{
	'									if (newFileCache.get(j).equals(fileCache.get(i)))
	'									{
	'										end = j;
	'										break;
	'									}
	'								}
	'								break;
	'							}
	'						}
	'						if (start >= 0 && end > start && newFileCache.subList(end, newSize).equals(fileCache.subList(start, oldSize)))
	'						{
	'							if (isInterrupted())
	'							{
	'								Return Nothing;
	'							}
	'							Return New DoChangeContents(newFileCache.subList(start, end), start, Nothing, 0, fid);
	'						}
	'					}
	'					else if (newSize < oldSize)
	'					{
	'						'see if interval is removed
	'						int start = -1;
	'						int end = -1;
	'						for (int i = 0; i < newSize; i += 1)
	'						{
	'							if (!newFileCache.get(i).equals(fileCache.get(i)))
	'							{
	'								start = i;
	'								end = i + oldSize - newSize;
	'								break;
	'							}
	'						}
	'						if (start >= 0 && end > start && fileCache.subList(end, oldSize).equals(newFileCache.subList(start, newSize)))
	'						{
	'							if (isInterrupted())
	'							{
	'								Return Nothing;
	'							}
	'							Return New DoChangeContents(Nothing, 0, New Vector(fileCache.subList(start, end)), start, fid);
	'						}
	'					}
	'					if (!fileCache.equals(newFileCache))
	'					{
	'						if (isInterrupted())
	'						{
	'							cancelRunnables(runnables);
	'						}
	'						Return New DoChangeContents(newFileCache, 0, fileCache, 0, fid);
	'					}
	'					Return Nothing;
	'				}
	'			});

				If doChangeContents IsNot Nothing Then
					runnables.Add(doChangeContents)
					SwingUtilities.invokeLater(doChangeContents)
				End If
			End Sub


			Public Overridable Sub cancelRunnables(ByVal runnables As List(Of DoChangeContents))
				For Each runnable As DoChangeContents In runnables
					runnable.cancel()
				Next runnable
			End Sub

			Public Overridable Sub cancelRunnables()
				cancelRunnables(runnables)
			End Sub
		End Class


		''' <summary>
		''' Adds a PropertyChangeListener to the listener list. The listener is
		''' registered for all bound properties of this class.
		''' <p>
		''' If <code>listener</code> is <code>null</code>,
		''' no exception is thrown and no action is performed.
		''' </summary>
		''' <param name="listener">  the property change listener to be added
		''' </param>
		''' <seealso cref= #removePropertyChangeListener </seealso>
		''' <seealso cref= #getPropertyChangeListeners
		''' 
		''' @since 1.6 </seealso>
		Public Overridable Sub addPropertyChangeListener(ByVal listener As PropertyChangeListener)
			If changeSupport Is Nothing Then changeSupport = New PropertyChangeSupport(Me)
			changeSupport.addPropertyChangeListener(listener)
		End Sub

		''' <summary>
		''' Removes a PropertyChangeListener from the listener list.
		''' <p>
		''' If listener is null, no exception is thrown and no action is performed.
		''' </summary>
		''' <param name="listener"> the PropertyChangeListener to be removed
		''' </param>
		''' <seealso cref= #addPropertyChangeListener </seealso>
		''' <seealso cref= #getPropertyChangeListeners
		''' 
		''' @since 1.6 </seealso>
		Public Overridable Sub removePropertyChangeListener(ByVal listener As PropertyChangeListener)
			If changeSupport IsNot Nothing Then changeSupport.removePropertyChangeListener(listener)
		End Sub

		''' <summary>
		''' Returns an array of all the property change listeners
		''' registered on this component.
		''' </summary>
		''' <returns> all of this component's <code>PropertyChangeListener</code>s
		'''         or an empty array if no property change
		'''         listeners are currently registered
		''' </returns>
		''' <seealso cref=      #addPropertyChangeListener </seealso>
		''' <seealso cref=      #removePropertyChangeListener </seealso>
		''' <seealso cref=      java.beans.PropertyChangeSupport#getPropertyChangeListeners
		''' 
		''' @since 1.6 </seealso>
		Public Overridable Property propertyChangeListeners As PropertyChangeListener()
			Get
				If changeSupport Is Nothing Then Return New PropertyChangeListener(){}
				Return changeSupport.propertyChangeListeners
			End Get
		End Property

		''' <summary>
		''' Support for reporting bound property changes for boolean properties.
		''' This method can be called when a bound property has changed and it will
		''' send the appropriate PropertyChangeEvent to any registered
		''' PropertyChangeListeners.
		''' </summary>
		''' <param name="propertyName"> the property whose value has changed </param>
		''' <param name="oldValue"> the property's previous value </param>
		''' <param name="newValue"> the property's new value
		''' 
		''' @since 1.6 </param>
		Protected Friend Overridable Sub firePropertyChange(ByVal propertyName As String, ByVal oldValue As Object, ByVal newValue As Object)
			If changeSupport IsNot Nothing Then changeSupport.firePropertyChange(propertyName, oldValue, newValue)
		End Sub


		''' <summary>
		''' Set the busy state for the model. The model is considered
		''' busy when it is running a separate (interruptable)
		''' thread in order to load the contents of a directory.
		''' </summary>
		<MethodImpl(MethodImplOptions.Synchronized)> _
		Private Sub setBusy(ByVal busy As Boolean, ByVal fid As Integer)
			If fid = fetchID Then
				Dim oldValue As Boolean = Me.busy
				Me.busy = busy

				If changeSupport IsNot Nothing AndAlso busy <> oldValue Then
'JAVA TO VB CONVERTER TODO TASK: Anonymous inner classes are not converted to VB if the base type is not defined in the code being converted:
'					SwingUtilities.invokeLater(New Runnable()
	'				{
	'					public void run()
	'					{
	'						firePropertyChange("busy", !busy, busy);
	'					}
	'				});
				End If
			End If
		End Sub


		Friend Class DoChangeContents
			Implements Runnable

			Private ReadOnly outerInstance As BasicDirectoryModel

			Private addFiles As IList(Of java.io.File)
			Private remFiles As IList(Of java.io.File)
			Private doFire As Boolean = True
			Private fid As Integer
			Private addStart As Integer = 0
			Private remStart As Integer = 0

			Public Sub New(ByVal outerInstance As BasicDirectoryModel, ByVal addFiles As IList(Of java.io.File), ByVal addStart As Integer, ByVal remFiles As IList(Of java.io.File), ByVal remStart As Integer, ByVal fid As Integer)
					Me.outerInstance = outerInstance
				Me.addFiles = addFiles
				Me.addStart = addStart
				Me.remFiles = remFiles
				Me.remStart = remStart
				Me.fid = fid
			End Sub

			<MethodImpl(MethodImplOptions.Synchronized)> _
			Friend Overridable Sub cancel()
					doFire = False
			End Sub

			<MethodImpl(MethodImplOptions.Synchronized)> _
			Public Overridable Sub run()
				If outerInstance.fetchID = fid AndAlso doFire Then
					Dim remSize As Integer = If(remFiles Is Nothing, 0, remFiles.Count)
					Dim addSize As Integer = If(addFiles Is Nothing, 0, addFiles.Count)
					SyncLock outerInstance.fileCache
						If remSize > 0 Then outerInstance.fileCache.removeAll(remFiles)
						If addSize > 0 Then outerInstance.fileCache.AddRange(addStart, addFiles)
						outerInstance.files = Nothing
						outerInstance.directories = Nothing
					End SyncLock
					If remSize > 0 AndAlso addSize = 0 Then
						outerInstance.fireIntervalRemoved(BasicDirectoryModel.this, remStart, remStart + remSize - 1)
					ElseIf addSize > 0 AndAlso remSize = 0 AndAlso addStart + addSize <= outerInstance.fileCache.Count Then
						outerInstance.fireIntervalAdded(BasicDirectoryModel.this, addStart, addStart + addSize - 1)
					Else
						outerInstance.fireContentsChanged()
					End If
				End If
			End Sub
		End Class
	End Class

End Namespace