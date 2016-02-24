Imports System.Diagnostics
Imports System.Collections.Generic

'
' * Copyright (c) 2007, 2013, Oracle and/or its affiliates. All rights reserved.
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

Namespace java.nio.file


	''' <summary>
	''' Walks a file tree, generating a sequence of events corresponding to the files
	''' in the tree.
	''' 
	''' <pre>{@code
	'''     Path top = ...
	'''     Set<FileVisitOption> options = ...
	'''     int maxDepth = ...
	''' 
	'''     try (FileTreeWalker walker = new FileTreeWalker(options, maxDepth)) {
	'''         FileTreeWalker.Event ev = walker.walk(top);
	'''         do {
	'''             process(ev);
	'''             ev = walker.next();
	'''         } while (ev != null);
	'''     }
	''' }</pre>
	''' </summary>
	''' <seealso cref= Files#walkFileTree </seealso>

	Friend Class FileTreeWalker
		Implements java.io.Closeable

		Private ReadOnly followLinks As Boolean
		Private ReadOnly linkOptions As LinkOption()
		Private ReadOnly maxDepth As Integer
		Private ReadOnly stack As New java.util.ArrayDeque(Of DirectoryNode)
		Private closed As Boolean

		''' <summary>
		''' The element on the walking stack corresponding to a directory node.
		''' </summary>
		Private Class DirectoryNode
			Private ReadOnly dir As Path
			Private ReadOnly key_Renamed As Object
			Private ReadOnly stream_Renamed As DirectoryStream(Of Path)
			Private ReadOnly iterator_Renamed As IEnumerator(Of Path)
			Private skipped_Renamed As Boolean

			Friend Sub New(ByVal dir As Path, ByVal key As Object, ByVal stream As DirectoryStream(Of Path))
				Me.dir = dir
				Me.key_Renamed = key
				Me.stream_Renamed = stream
				Me.iterator_Renamed = stream.GetEnumerator()
			End Sub

			Friend Overridable Function directory() As Path
				Return dir
			End Function

			Friend Overridable Function key() As Object
				Return key_Renamed
			End Function

			Friend Overridable Function stream() As DirectoryStream(Of Path)
				Return stream_Renamed
			End Function

			Friend Overridable Function [iterator]() As IEnumerator(Of Path)
				Return iterator_Renamed
			End Function

			Friend Overridable Sub skip()
				skipped_Renamed = True
			End Sub

			Friend Overridable Function skipped() As Boolean
				Return skipped_Renamed
			End Function
		End Class

		''' <summary>
		''' The event types.
		''' </summary>
		Friend Enum EventType
			''' <summary>
			''' Start of a directory
			''' </summary>
			START_DIRECTORY
			''' <summary>
			''' End of a directory
			''' </summary>
			END_DIRECTORY
			''' <summary>
			''' An entry in a directory
			''' </summary>
			ENTRY
		End Enum

		''' <summary>
		''' Events returned by the <seealso cref="#walk"/> and <seealso cref="#next"/> methods.
		''' </summary>
		Friend Class [Event]
			Private ReadOnly type_Renamed As EventType
			Private ReadOnly file_Renamed As Path
			Private ReadOnly attrs As java.nio.file.attribute.BasicFileAttributes
			Private ReadOnly ioe As java.io.IOException

			Private Sub New(ByVal type As EventType, ByVal file As Path, ByVal attrs As java.nio.file.attribute.BasicFileAttributes, ByVal ioe As java.io.IOException)
				Me.type_Renamed = type
				Me.file_Renamed = file
				Me.attrs = attrs
				Me.ioe = ioe
			End Sub

			Friend Sub New(ByVal type As EventType, ByVal file As Path, ByVal attrs As java.nio.file.attribute.BasicFileAttributes)
				Me.New(type, file, attrs, Nothing)
			End Sub

			Friend Sub New(ByVal type As EventType, ByVal file As Path, ByVal ioe As java.io.IOException)
				Me.New(type, file, Nothing, ioe)
			End Sub

			Friend Overridable Function type() As EventType
				Return type_Renamed
			End Function

			Friend Overridable Function file() As Path
				Return file_Renamed
			End Function

			Friend Overridable Function attributes() As java.nio.file.attribute.BasicFileAttributes
				Return attrs
			End Function

			Friend Overridable Function ioeException() As java.io.IOException
				Return ioe
			End Function
		End Class

		''' <summary>
		''' Creates a {@code FileTreeWalker}.
		''' </summary>
		''' <exception cref="IllegalArgumentException">
		'''          if {@code maxDepth} is negative </exception>
		''' <exception cref="ClassCastException">
		'''          if (@code options} contains an element that is not a
		'''          {@code FileVisitOption} </exception>
		''' <exception cref="NullPointerException">
		'''          if {@code options} is {@ocde null} or the options
		'''          array contains a {@code null} element </exception>
		Friend Sub New(ByVal options As ICollection(Of FileVisitOption), ByVal maxDepth As Integer)
			Dim fl As Boolean = False
			For Each [option] As FileVisitOption In options
				' will throw NPE if options contains null
				Select Case [option]
					Case FileVisitOption.FOLLOW_LINKS
						fl = True
					Case Else
						Throw New AssertionError("Should not get here")
				End Select
			Next [option]
			If maxDepth < 0 Then Throw New IllegalArgumentException("'maxDepth' is negative")

			Me.followLinks = fl
			Me.linkOptions = If(fl, New LinkOption(){}, New LinkOption){ LinkOption.NOFOLLOW_LINKS }
			Me.maxDepth = maxDepth
		End Sub

		''' <summary>
		''' Returns the attributes of the given file, taking into account whether
		''' the walk is following sym links is not. The {@code canUseCached}
		''' argument determines whether this method can use cached attributes.
		''' </summary>
		Private Function getAttributes(ByVal file As Path, ByVal canUseCached As Boolean) As java.nio.file.attribute.BasicFileAttributes
			' if attributes are cached then use them if possible
			If canUseCached AndAlso (TypeOf file Is sun.nio.fs.BasicFileAttributesHolder) AndAlso (System.securityManager Is Nothing) Then
				Dim cached As java.nio.file.attribute.BasicFileAttributes = CType(file, sun.nio.fs.BasicFileAttributesHolder).get()
				If cached IsNot Nothing AndAlso ((Not followLinks) OrElse (Not cached.symbolicLink)) Then Return cached
			End If

			' attempt to get attributes of file. If fails and we are following
			' links then a link target might not exist so get attributes of link
			Dim attrs As java.nio.file.attribute.BasicFileAttributes
			Try
				attrs = Files.readAttributes(file, GetType(java.nio.file.attribute.BasicFileAttributes), linkOptions)
			Catch ioe As java.io.IOException
				If Not followLinks Then Throw ioe

				' attempt to get attrmptes without following links
				attrs = Files.readAttributes(file, GetType(java.nio.file.attribute.BasicFileAttributes), LinkOption.NOFOLLOW_LINKS)
			End Try
			Return attrs
		End Function

		''' <summary>
		''' Returns true if walking into the given directory would result in a
		''' file system loop/cycle.
		''' </summary>
		Private Function wouldLoop(ByVal dir As Path, ByVal key As Object) As Boolean
			' if this directory and ancestor has a file key then we compare
			' them; otherwise we use less efficient isSameFile test.
			For Each ancestor As DirectoryNode In stack
				Dim ancestorKey As Object = ancestor.key()
				If key IsNot Nothing AndAlso ancestorKey IsNot Nothing Then
					If key.Equals(ancestorKey) Then Return True
				Else
					Try
						If Files.isSameFile(dir, ancestor.directory()) Then Return True
'JAVA TO VB CONVERTER TODO TASK: There is no equivalent in VB to Java 'multi-catch' syntax:
					Catch java.io.IOException Or SecurityException x
						' ignore
					End Try
				End If
			Next ancestor
			Return False
		End Function

		''' <summary>
		''' Visits the given file, returning the {@code Event} corresponding to that
		''' visit.
		''' 
		''' The {@code ignoreSecurityException} parameter determines whether
		''' any SecurityException should be ignored or not. If a SecurityException
		''' is thrown, and is ignored, then this method returns {@code null} to
		''' mean that there is no event corresponding to a visit to the file.
		''' 
		''' The {@code canUseCached} parameter determines whether cached attributes
		''' for the file can be used or not.
		''' </summary>
		Private Function visit(ByVal entry As Path, ByVal ignoreSecurityException As Boolean, ByVal canUseCached As Boolean) As [Event]
			' need the file attributes
			Dim attrs As java.nio.file.attribute.BasicFileAttributes
			Try
				attrs = getAttributes(entry, canUseCached)
			Catch ioe As java.io.IOException
				Return New [Event](EventType.ENTRY, entry, ioe)
			Catch se As SecurityException
				If ignoreSecurityException Then Return Nothing
				Throw se
			End Try

			' at maximum depth or file is not a directory
			Dim depth As Integer = stack.size()
			If depth >= maxDepth OrElse (Not attrs.directory) Then Return New [Event](EventType.ENTRY, entry, attrs)

			' check for cycles when following links
			If followLinks AndAlso wouldLoop(entry, attrs.fileKey()) Then Return New [Event](EventType.ENTRY, entry, New FileSystemLoopException(entry.ToString()))

			' file is a directory, attempt to open it
			Dim stream As DirectoryStream(Of Path) = Nothing
			Try
				stream = Files.newDirectoryStream(entry)
			Catch ioe As java.io.IOException
				Return New [Event](EventType.ENTRY, entry, ioe)
			Catch se As SecurityException
				If ignoreSecurityException Then Return Nothing
				Throw se
			End Try

			' push a directory node to the stack and return an event
			stack.push(New DirectoryNode(entry, attrs.fileKey(), stream))
			Return New [Event](EventType.START_DIRECTORY, entry, attrs)
		End Function


		''' <summary>
		''' Start walking from the given file.
		''' </summary>
		Friend Overridable Function walk(ByVal file As Path) As [Event]
			If closed Then Throw New IllegalStateException("Closed")

			Dim ev As [Event] = visit(file, False, False) ' canUseCached -  ignoreSecurityException
			Debug.Assert(ev IsNot Nothing)
			Return ev
		End Function

		''' <summary>
		''' Returns the next Event or {@code null} if there are no more events or
		''' the walker is closed.
		''' </summary>
		Friend Overridable Function [next]() As [Event]
			Dim top As DirectoryNode = stack.peek()
			If top Is Nothing Then Return Nothing ' stack is empty, we are done

			' continue iteration of the directory at the top of the stack
			Dim ev As [Event]
			Do
				Dim entry As Path = Nothing
				Dim ioe As java.io.IOException = Nothing

				' get next entry in the directory
				If Not top.skipped() Then
					Dim [iterator] As IEnumerator(Of Path) = top.GetEnumerator()
					Try
'JAVA TO VB CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
						If [iterator].hasNext() Then entry = [iterator].next()
					Catch x As DirectoryIteratorException
						ioe = x.InnerException
					End Try
				End If

				' no next entry so close and pop directory, creating corresponding event
				If entry Is Nothing Then
					Try
						top.stream().close()
					Catch e As java.io.IOException
						If ioe IsNot Nothing Then
							ioe = e
						Else
							ioe.addSuppressed(e)
						End If
					End Try
					stack.pop()
					Return New [Event](EventType.END_DIRECTORY, top.directory(), ioe)
				End If

				' visit the entry
				ev = visit(entry, True, True) ' canUseCached -  ignoreSecurityException

			Loop While ev Is Nothing

			Return ev
		End Function

		''' <summary>
		''' Pops the directory node that is the current top of the stack so that
		''' there are no more events for the directory (including no END_DIRECTORY)
		''' event. This method is a no-op if the stack is empty or the walker is
		''' closed.
		''' </summary>
		Friend Overridable Sub pop()
			If Not stack.empty Then
				Dim node As DirectoryNode = stack.pop()
				Try
					node.stream().close()
				Catch ignore As java.io.IOException
				End Try
			End If
		End Sub

		''' <summary>
		''' Skips the remaining entries in the directory at the top of the stack.
		''' This method is a no-op if the stack is empty or the walker is closed.
		''' </summary>
		Friend Overridable Sub skipRemainingSiblings()
			If Not stack.empty Then stack.peek().skip()
		End Sub

		''' <summary>
		''' Returns {@code true} if the walker is open.
		''' </summary>
		Friend Overridable Property open As Boolean
			Get
				Return Not closed
			End Get
		End Property

		''' <summary>
		''' Closes/pops all directories on the stack.
		''' </summary>
		Public Overrides Sub close()
			If Not closed Then
				Do While Not stack.empty
					pop()
				Loop
				closed = True
			End If
		End Sub
	End Class

End Namespace