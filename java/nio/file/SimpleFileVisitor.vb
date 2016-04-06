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
	''' A simple visitor of files with default behavior to visit all files and to
	''' re-throw I/O errors.
	''' 
	''' <p> Methods in this class may be overridden subject to their general contract.
	''' </summary>
	''' @param   <T>     The type of reference to the files
	''' 
	''' @since 1.7 </param>

	Public Class SimpleFileVisitor(Of T)
		Implements FileVisitor(Of T)

		''' <summary>
		''' Initializes a new instance of this class.
		''' </summary>
		Protected Friend Sub New()
		End Sub

		''' <summary>
		''' Invoked for a directory before entries in the directory are visited.
		''' 
		''' <p> Unless overridden, this method returns {@link FileVisitResult#CONTINUE
		''' CONTINUE}.
		''' </summary>
		Public Overrides Function preVisitDirectory(  dir As T,   attrs As java.nio.file.attribute.BasicFileAttributes) As FileVisitResult Implements FileVisitor(Of T).preVisitDirectory
			java.util.Objects.requireNonNull(dir)
			java.util.Objects.requireNonNull(attrs)
			Return FileVisitResult.CONTINUE
		End Function

		''' <summary>
		''' Invoked for a file in a directory.
		''' 
		''' <p> Unless overridden, this method returns {@link FileVisitResult#CONTINUE
		''' CONTINUE}.
		''' </summary>
		Public Overrides Function visitFile(  file As T,   attrs As java.nio.file.attribute.BasicFileAttributes) As FileVisitResult Implements FileVisitor(Of T).visitFile
			java.util.Objects.requireNonNull(file)
			java.util.Objects.requireNonNull(attrs)
			Return FileVisitResult.CONTINUE
		End Function

		''' <summary>
		''' Invoked for a file that could not be visited.
		''' 
		''' <p> Unless overridden, this method re-throws the I/O exception that prevented
		''' the file from being visited.
		''' </summary>
		Public Overrides Function visitFileFailed(  file As T,   exc As java.io.IOException) As FileVisitResult Implements FileVisitor(Of T).visitFileFailed
			java.util.Objects.requireNonNull(file)
			Throw exc
		End Function

		''' <summary>
		''' Invoked for a directory after entries in the directory, and all of their
		''' descendants, have been visited.
		''' 
		''' <p> Unless overridden, this method returns {@link FileVisitResult#CONTINUE
		''' CONTINUE} if the directory iteration completes without an I/O exception;
		''' otherwise this method re-throws the I/O exception that caused the iteration
		''' of the directory to terminate prematurely.
		''' </summary>
		Public Overrides Function postVisitDirectory(  dir As T,   exc As java.io.IOException) As FileVisitResult Implements FileVisitor(Of T).postVisitDirectory
			java.util.Objects.requireNonNull(dir)
			If exc IsNot Nothing Then Throw exc
			Return FileVisitResult.CONTINUE
		End Function
	End Class

End Namespace