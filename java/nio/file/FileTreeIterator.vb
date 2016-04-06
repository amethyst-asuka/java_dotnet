Imports System.Diagnostics

'
' * Copyright (c) 2013, Oracle and/or its affiliates. All rights reserved.
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
	''' An {@code Iterator to iterate over the nodes of a file tree.
	''' 
	''' <pre>{@code
	'''     try (FileTreeIterator iterator = new FileTreeIterator(start, maxDepth, options)) {
	'''         while (iterator.hasNext()) {
	'''             Event ev = iterator.next();
	'''             Path path = ev.file();
	'''             BasicFileAttributes attrs = ev.attributes();
	'''         }
	'''     }
	''' }</pre>
	''' </summary>

	Friend Class FileTreeIterator
		Implements IEnumerator(Of java.nio.file.FileTreeWalker.Event), java.io.Closeable

		Private ReadOnly walker As FileTreeWalker
		Private next_Renamed As java.nio.file.FileTreeWalker.Event

		''' <summary>
		''' Creates a new iterator to walk the file tree starting at the given file.
		''' </summary>
		''' <exception cref="IllegalArgumentException">
		'''          if {@code maxDepth} is negative </exception>
		''' <exception cref="IOException">
		'''          if an I/O errors occurs opening the starting file </exception>
		''' <exception cref="SecurityException">
		'''          if the security manager denies access to the starting file </exception>
		''' <exception cref="NullPointerException">
		'''          if {@code start} or {@code options} is {@ocde null} or
		'''          the options array contains a {@code null} element </exception>
		Friend Sub New(  start As Path,   maxDepth As Integer, ParamArray   options As FileVisitOption())
			Me.walker = New FileTreeWalker(java.util.Arrays.asList(options), maxDepth)
			Me.next_Renamed = walker.walk(start)
			Debug.Assert(next_Renamed.type() = FileTreeWalker.EventType.ENTRY OrElse next_Renamed.type() = FileTreeWalker.EventType.START_DIRECTORY)

			' IOException if there a problem accessing the starting file
			Dim ioe As java.io.IOException = next_Renamed.ioeException()
			If ioe IsNot Nothing Then Throw ioe
		End Sub

		Private Sub fetchNextIfNeeded()
			If next_Renamed Is Nothing Then
				Dim ev As FileTreeWalker.Event = walker.next()
				Do While ev IsNot Nothing
					Dim ioe As java.io.IOException = ev.ioeException()
					If ioe IsNot Nothing Then Throw New java.io.UncheckedIOException(ioe)

					' END_DIRECTORY events are ignored
					If ev.type() <> FileTreeWalker.EventType.END_DIRECTORY Then
						next_Renamed = ev
						Return
					End If
					ev = walker.next()
				Loop
			End If
		End Sub

		Public Overrides Function hasNext() As Boolean
			If Not walker.open Then Throw New IllegalStateException
			fetchNextIfNeeded()
			Return next_Renamed IsNot Nothing
		End Function

		Public Overrides Function [next]() As java.nio.file.FileTreeWalker.Event
			If Not walker.open Then Throw New IllegalStateException
			fetchNextIfNeeded()
			If next_Renamed Is Nothing Then Throw New java.util.NoSuchElementException
			Dim result As java.nio.file.FileTreeWalker.Event = next_Renamed
			next_Renamed = Nothing
			Return result
		End Function

		Public Overrides Sub close()
			walker.close()
		End Sub
	End Class

End Namespace