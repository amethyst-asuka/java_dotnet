Imports System

'
' * Copyright (c) 2009, 2013, Oracle and/or its affiliates. All rights reserved.
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
	''' Helper class to support creation of temporary files and directories with
	''' initial attributes.
	''' </summary>

	Friend Class TempFileHelper
		Private Sub New()
		End Sub

		' temporary directory location
		Private Shared ReadOnly tmpdir As Path = Paths.get(doPrivileged(New sun.security.action.GetPropertyAction("java.io.tmpdir")))

		Private Shared ReadOnly isPosix As Boolean = FileSystems.default.supportedFileAttributeViews().contains("posix")

		' file name generation, same as java.io.File for now
		Private Shared ReadOnly random As New java.security.SecureRandom
		Private Shared Function generatePath(ByVal prefix As String, ByVal suffix As String, ByVal dir As Path) As Path
			Dim n As Long = random.nextLong()
			n = If(n = Long.MinValue, 0, Math.Abs(n))
			Dim name As Path = dir.fileSystem.getPath(prefix + Convert.ToString(n) + suffix)
			' the generated name should be a simple file name
			If name.parent IsNot Nothing Then Throw New IllegalArgumentException("Invalid prefix or suffix")
			Return dir.resolve(name)
		End Function

		' default file and directory permissions (lazily initialized)
		Private Class PosixPermissions
			Friend Shared ReadOnly filePermissions As java.nio.file.attribute.FileAttribute(Of java.util.Set(Of java.nio.file.attribute.PosixFilePermission)) = java.nio.file.attribute.PosixFilePermissions.asFileAttribute(java.util.EnumSet.of(OWNER_READ, OWNER_WRITE))
			Friend Shared ReadOnly dirPermissions As java.nio.file.attribute.FileAttribute(Of java.util.Set(Of java.nio.file.attribute.PosixFilePermission)) = java.nio.file.attribute.PosixFilePermissions.asFileAttribute(java.util.EnumSet.of(OWNER_READ, OWNER_WRITE, OWNER_EXECUTE))
		End Class

		''' <summary>
		''' Creates a file or directory in in the given given directory (or in the
		''' temporary directory if dir is {@code null}).
		''' </summary>
		Private Shared Function create(Of T1)(ByVal dir As Path, ByVal prefix As String, ByVal suffix As String, ByVal createDirectory As Boolean, ByVal attrs As java.nio.file.attribute.FileAttribute(Of T1)()) As Path
			If prefix Is Nothing Then prefix = ""
			If suffix Is Nothing Then suffix = If(createDirectory, "", ".tmp")
			If dir Is Nothing Then dir = tmpdir

			' in POSIX environments use default file and directory permissions
			' if initial permissions not given by caller.
			If isPosix AndAlso (dir.fileSystem Is FileSystems.default) Then
				If attrs.Length = 0 Then
					' no attributes so use default permissions
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
					attrs = New java.nio.file.attribute.FileAttribute(Of ?)(0){}
					attrs(0) = If(createDirectory, PosixPermissions.dirPermissions, PosixPermissions.filePermissions)
				Else
					' check if posix permissions given; if not use default
					Dim hasPermissions As Boolean = False
					For i As Integer = 0 To attrs.Length - 1
						If attrs(i).name().Equals("posix:permissions") Then
							hasPermissions = True
							Exit For
						End If
					Next i
					If Not hasPermissions Then
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
						Dim copy As java.nio.file.attribute.FileAttribute(Of ?)() = New java.nio.file.attribute.FileAttribute(Of ?)(attrs.Length){}
						Array.Copy(attrs, 0, copy, 0, attrs.Length)
						attrs = copy
						attrs(attrs.Length-1) = If(createDirectory, PosixPermissions.dirPermissions, PosixPermissions.filePermissions)
					End If
				End If
			End If

			' loop generating random names until file or directory can be created
			Dim sm As SecurityManager = System.securityManager
			Do
				Dim f As Path
				Try
					f = generatePath(prefix, suffix, dir)
				Catch e As InvalidPathException
					' don't reveal temporary directory location
					If sm IsNot Nothing Then Throw New IllegalArgumentException("Invalid prefix or suffix")
					Throw e
				End Try
				Try
					If createDirectory Then
						Return Files.createDirectory(f, attrs)
					Else
						Return Files.createFile(f, attrs)
					End If
				Catch e As SecurityException
					' don't reveal temporary directory location
					If dir Is tmpdir AndAlso sm IsNot Nothing Then Throw New SecurityException("Unable to create temporary file or directory")
					Throw e
				Catch e As FileAlreadyExistsException
					' ignore
				End Try
			Loop
		End Function

		''' <summary>
		''' Creates a temporary file in the given directory, or in in the
		''' temporary directory if dir is {@code null}.
		''' </summary>
		Friend Shared Function createTempFile(Of T1)(ByVal dir As Path, ByVal prefix As String, ByVal suffix As String, ByVal attrs As java.nio.file.attribute.FileAttribute(Of T1)()) As Path
			Return create(dir, prefix, suffix, False, attrs)
		End Function

		''' <summary>
		''' Creates a temporary directory in the given directory, or in in the
		''' temporary directory if dir is {@code null}.
		''' </summary>
		Friend Shared Function createTempDirectory(Of T1)(ByVal dir As Path, ByVal prefix As String, ByVal attrs As java.nio.file.attribute.FileAttribute(Of T1)()) As Path
			Return create(dir, prefix, Nothing, True, attrs)
		End Function
	End Class

End Namespace