'
' * Copyright (c) 2011, Oracle and/or its affiliates. All rights reserved.
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
	''' Helper class to support copying or moving files when the source and target
	''' are associated with different providers.
	''' </summary>

	Friend Class CopyMoveHelper
		Private Sub New()
		End Sub

		''' <summary>
		''' Parses the arguments for a file copy operation.
		''' </summary>
		Private Class CopyOptions
			Friend replaceExisting As Boolean = False
			Friend copyAttributes As Boolean = False
			Friend followLinks As Boolean = True

			Private Sub New()
			End Sub

			Shared Function parse(ParamArray ByVal options As CopyOption()) As CopyOptions
				Dim result As New CopyOptions
				For Each [option] As CopyOption In options
					If [option] = StandardCopyOption.REPLACE_EXISTING Then
						result.replaceExisting = True
						Continue For
					End If
					If [option] = LinkOption.NOFOLLOW_LINKS Then
						result.followLinks = False
						Continue For
					End If
					If [option] = StandardCopyOption.COPY_ATTRIBUTES Then
						result.copyAttributes = True
						Continue For
					End If
					If [option] Is Nothing Then Throw New NullPointerException
					Throw New UnsupportedOperationException("'" & [option] & "' is not a recognized copy option")
				Next [option]
				Return result
			End Function
		End Class

		''' <summary>
		''' Converts the given array of options for moving a file to options suitable
		''' for copying the file when a move is implemented as copy + delete.
		''' </summary>
		Private Shared Function convertMoveToCopyOptions(ParamArray ByVal options As CopyOption()) As CopyOption()
			Dim len As Integer = options.Length
			Dim newOptions As CopyOption() = New CopyOption(len+2 - 1){}
			For i As Integer = 0 To len - 1
				Dim [option] As CopyOption = options(i)
				If [option] = StandardCopyOption.ATOMIC_MOVE Then Throw New AtomicMoveNotSupportedException(Nothing, Nothing, "Atomic move between providers is not supported")
				newOptions(i) = [option]
			Next i
			newOptions(len) = LinkOption.NOFOLLOW_LINKS
			newOptions(len+1) = StandardCopyOption.COPY_ATTRIBUTES
			Return newOptions
		End Function

		''' <summary>
		''' Simple copy for use when source and target are associated with different
		''' providers
		''' </summary>
		Friend Shared Sub copyToForeignTarget(ByVal source As Path, ByVal target As Path, ParamArray ByVal options As CopyOption())
			Dim opts As CopyOptions = CopyOptions.parse(options)
			Dim linkOptions As LinkOption() = If(opts.followLinks, New LinkOption(){}, New LinkOption){ LinkOption.NOFOLLOW_LINKS }

			' attributes of source file
			Dim attrs As BasicFileAttributes = Files.readAttributes(source, GetType(BasicFileAttributes), linkOptions)
			If attrs.symbolicLink Then Throw New java.io.IOException("Copying of symbolic links not supported")

			' delete target if it exists and REPLACE_EXISTING is specified
			If opts.replaceExisting Then
				Files.deleteIfExists(target)
			ElseIf Files.exists(target) Then
				Throw New FileAlreadyExistsException(target.ToString())
			End If

			' create directory or copy file
			If attrs.directory Then
				Files.createDirectory(target)
			Else
				Using [in] As java.io.InputStream = Files.newInputStream(source)
					Files.copy([in], target)
				End Using
			End If

			' copy basic attributes to target
			If opts.copyAttributes Then
				Dim view As BasicFileAttributeView = Files.getFileAttributeView(target, GetType(BasicFileAttributeView))
				Try
					view.timesmes(attrs.lastModifiedTime(), attrs.lastAccessTime(), attrs.creationTime())
				Catch x As Throwable
					' rollback
					Try
						Files.delete(target)
					Catch suppressed As Throwable
						x.addSuppressed(suppressed)
					End Try
					Throw x
				End Try
			End If
		End Sub

		''' <summary>
		''' Simple move implements as copy+delete for use when source and target are
		''' associated with different providers
		''' </summary>
		Friend Shared Sub moveToForeignTarget(ByVal source As Path, ByVal target As Path, ParamArray ByVal options As CopyOption())
			copyToForeignTarget(source, target, convertMoveToCopyOptions(options))
			Files.delete(source)
		End Sub
	End Class

End Namespace