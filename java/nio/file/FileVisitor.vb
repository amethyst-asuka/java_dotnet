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
	''' A visitor of files. An implementation of this interface is provided to the
	''' <seealso cref="Files#walkFileTree Files.walkFileTree"/> methods to visit each file in
	''' a file tree.
	''' 
	''' <p> <b>Usage Examples:</b>
	''' Suppose we want to delete a file tree. In that case, each directory should
	''' be deleted after the entries in the directory are deleted.
	''' <pre>
	'''     Path start = ...
	'''     Files.walkFileTree(start, new SimpleFileVisitor&lt;Path&gt;() {
	'''         &#64;Override
	'''         public FileVisitResult visitFile(Path file, BasicFileAttributes attrs)
	'''             throws IOException
	'''         {
	'''             Files.delete(file);
	'''             return FileVisitResult.CONTINUE;
	'''         }
	'''         &#64;Override
	'''         public FileVisitResult postVisitDirectory(Path dir, IOException e)
	'''             throws IOException
	'''         {
	'''             if (e == null) {
	'''                 Files.delete(dir);
	'''                 return FileVisitResult.CONTINUE;
	'''             } else {
	'''                 // directory iteration failed
	'''                 throw e;
	'''             }
	'''         }
	'''     });
	''' </pre>
	''' <p> Furthermore, suppose we want to copy a file tree to a target location.
	''' In that case, symbolic links should be followed and the target directory
	''' should be created before the entries in the directory are copied.
	''' <pre>
	'''     final Path source = ...
	'''     final Path target = ...
	''' 
	'''     Files.walkFileTree(source, EnumSet.of(FileVisitOption.FOLLOW_LINKS),  java.lang.[Integer].MAX_VALUE,
	'''         new SimpleFileVisitor&lt;Path&gt;() {
	'''             &#64;Override
	'''             public FileVisitResult preVisitDirectory(Path dir, BasicFileAttributes attrs)
	'''                 throws IOException
	'''             {
	'''                 Path targetdir = target.resolve(source.relativize(dir));
	'''                 try {
	'''                     Files.copy(dir, targetdir);
	'''                 } catch (FileAlreadyExistsException e) {
	'''                      if (!Files.isDirectory(targetdir))
	'''                          throw e;
	'''                 }
	'''                 return CONTINUE;
	'''             }
	'''             &#64;Override
	'''             public FileVisitResult visitFile(Path file, BasicFileAttributes attrs)
	'''                 throws IOException
	'''             {
	'''                 Files.copy(file, target.resolve(source.relativize(file)));
	'''                 return CONTINUE;
	'''             }
	'''         });
	''' </pre>
	''' 
	''' @since 1.7
	''' </summary>

	Public Interface FileVisitor(Of T)

		''' <summary>
		''' Invoked for a directory before entries in the directory are visited.
		''' 
		''' <p> If this method returns <seealso cref="FileVisitResult#CONTINUE CONTINUE"/>,
		''' then entries in the directory are visited. If this method returns {@link
		''' FileVisitResult#SKIP_SUBTREE SKIP_SUBTREE} or {@link
		''' FileVisitResult#SKIP_SIBLINGS SKIP_SIBLINGS} then entries in the
		''' directory (and any descendants) will not be visited.
		''' </summary>
		''' <param name="dir">
		'''          a reference to the directory </param>
		''' <param name="attrs">
		'''          the directory's basic attributes
		''' </param>
		''' <returns>  the visit result
		''' </returns>
		''' <exception cref="IOException">
		'''          if an I/O error occurs </exception>
		Function preVisitDirectory(  dir As T,   attrs As java.nio.file.attribute.BasicFileAttributes) As FileVisitResult

		''' <summary>
		''' Invoked for a file in a directory.
		''' </summary>
		''' <param name="file">
		'''          a reference to the file </param>
		''' <param name="attrs">
		'''          the file's basic attributes
		''' </param>
		''' <returns>  the visit result
		''' </returns>
		''' <exception cref="IOException">
		'''          if an I/O error occurs </exception>
		Function visitFile(  file As T,   attrs As java.nio.file.attribute.BasicFileAttributes) As FileVisitResult

		''' <summary>
		''' Invoked for a file that could not be visited. This method is invoked
		''' if the file's attributes could not be read, the file is a directory
		''' that could not be opened, and other reasons.
		''' </summary>
		''' <param name="file">
		'''          a reference to the file </param>
		''' <param name="exc">
		'''          the I/O exception that prevented the file from being visited
		''' </param>
		''' <returns>  the visit result
		''' </returns>
		''' <exception cref="IOException">
		'''          if an I/O error occurs </exception>
		Function visitFileFailed(  file As T,   exc As java.io.IOException) As FileVisitResult

		''' <summary>
		''' Invoked for a directory after entries in the directory, and all of their
		''' descendants, have been visited. This method is also invoked when iteration
		''' of the directory completes prematurely (by a <seealso cref="#visitFile visitFile"/>
		''' method returning <seealso cref="FileVisitResult#SKIP_SIBLINGS SKIP_SIBLINGS"/>,
		''' or an I/O error when iterating over the directory).
		''' </summary>
		''' <param name="dir">
		'''          a reference to the directory </param>
		''' <param name="exc">
		'''          {@code null} if the iteration of the directory completes without
		'''          an error; otherwise the I/O exception that caused the iteration
		'''          of the directory to complete prematurely
		''' </param>
		''' <returns>  the visit result
		''' </returns>
		''' <exception cref="IOException">
		'''          if an I/O error occurs </exception>
		Function postVisitDirectory(  dir As T,   exc As java.io.IOException) As FileVisitResult
	End Interface

End Namespace