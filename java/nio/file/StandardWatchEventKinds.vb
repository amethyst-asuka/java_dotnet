'
' * Copyright (c) 2007, 2011, Oracle and/or its affiliates. All rights reserved.
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
	''' Defines the <em>standard</em> event kinds.
	''' 
	''' @since 1.7
	''' </summary>

	Public NotInheritable Class StandardWatchEventKinds
		Private Sub New()
		End Sub

		''' <summary>
		''' A special event to indicate that events may have been lost or
		''' discarded.
		''' 
		''' <p> The <seealso cref="WatchEvent#context context"/> for this event is
		''' implementation specific and may be {@code null}. The event {@link
		''' WatchEvent#count count} may be greater than {@code 1}.
		''' </summary>
		''' <seealso cref= WatchService </seealso>
		Public Shared ReadOnly OVERFLOW As WatchEvent.Kind(Of Object) = New StdWatchEventKind(Of Object)("OVERFLOW", GetType(Object))

		''' <summary>
		''' Directory entry created.
		''' 
		''' <p> When a directory is registered for this event then the <seealso cref="WatchKey"/>
		''' is queued when it is observed that an entry is created in the directory
		''' or renamed into the directory. The event <seealso cref="WatchEvent#count count"/>
		''' for this event is always {@code 1}.
		''' </summary>
		Public Shared ReadOnly ENTRY_CREATE As WatchEvent.Kind(Of Path) = New StdWatchEventKind(Of Path)("ENTRY_CREATE", GetType(Path))

		''' <summary>
		''' Directory entry deleted.
		''' 
		''' <p> When a directory is registered for this event then the <seealso cref="WatchKey"/>
		''' is queued when it is observed that an entry is deleted or renamed out of
		''' the directory. The event <seealso cref="WatchEvent#count count"/> for this event
		''' is always {@code 1}.
		''' </summary>
		Public Shared ReadOnly ENTRY_DELETE As WatchEvent.Kind(Of Path) = New StdWatchEventKind(Of Path)("ENTRY_DELETE", GetType(Path))

		''' <summary>
		''' Directory entry modified.
		''' 
		''' <p> When a directory is registered for this event then the <seealso cref="WatchKey"/>
		''' is queued when it is observed that an entry in the directory has been
		''' modified. The event <seealso cref="WatchEvent#count count"/> for this event is
		''' {@code 1} or greater.
		''' </summary>
		Public Shared ReadOnly ENTRY_MODIFY As WatchEvent.Kind(Of Path) = New StdWatchEventKind(Of Path)("ENTRY_MODIFY", GetType(Path))

		Private Class StdWatchEventKind(Of T)
			Implements WatchEvent.Kind(Of T)

			Private ReadOnly name_Renamed As String
			Private ReadOnly type_Renamed As  [Class]
			Friend Sub New(  name As String,   type As [Class])
				Me.name_Renamed = name
				Me.type_Renamed = type
			End Sub
			Public Overrides Function name() As String
				Return name_Renamed
			End Function
			Public Overrides Function type() As  [Class]
				Return type_Renamed
			End Function
			Public Overrides Function ToString() As String
				Return name_Renamed
			End Function
		End Class
	End Class

End Namespace