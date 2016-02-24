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
	''' A {@code DirectoryStream} that defines operations on files that are located
	''' relative to an open directory. A {@code SecureDirectoryStream} is intended
	''' for use by sophisticated or security sensitive applications requiring to
	''' traverse file trees or otherwise operate on directories in a race-free manner.
	''' Race conditions can arise when a sequence of file operations cannot be
	''' carried out in isolation. Each of the file operations defined by this
	''' interface specify a relative path. All access to the file is relative
	''' to the open directory irrespective of if the directory is moved or replaced
	''' by an attacker while the directory is open. A {@code SecureDirectoryStream}
	''' may also be used as a virtual <em>working directory</em>.
	''' 
	''' <p> A {@code SecureDirectoryStream} requires corresponding support from the
	''' underlying operating system. Where an implementation supports this features
	''' then the {@code DirectoryStream} returned by the {@link Files#newDirectoryStream
	''' newDirectoryStream} method will be a {@code SecureDirectoryStream} and must
	''' be cast to that type in order to invoke the methods defined by this interface.
	''' 
	''' <p> In the case of the default {@link java.nio.file.spi.FileSystemProvider
	''' provider}, and a security manager is set, then the permission checks are
	''' performed using the path obtained by resolving the given relative path
	''' against the <i>original path</i> of the directory (irrespective of if the
	''' directory is moved since it was opened).
	''' 
	''' @since   1.7
	''' </summary>

	Public Interface SecureDirectoryStream(Of T)
		Inherits DirectoryStream(Of T)

		''' <summary>
		''' Opens the directory identified by the given path, returning a {@code
		''' SecureDirectoryStream} to iterate over the entries in the directory.
		''' 
		''' <p> This method works in exactly the manner specified by the {@link
		''' Files#newDirectoryStream(Path) newDirectoryStream} method for the case that
		''' the {@code path} parameter is an <seealso cref="Path#isAbsolute absolute"/> path.
		''' When the parameter is a relative path then the directory to open is
		''' relative to this open directory. The {@link
		''' LinkOption#NOFOLLOW_LINKS NOFOLLOW_LINKS} option may be used to
		''' ensure that this method fails if the file is a symbolic link.
		''' 
		''' <p> The new directory stream, once created, is not dependent upon the
		''' directory stream used to create it. Closing this directory stream has no
		''' effect upon newly created directory stream.
		''' </summary>
		''' <param name="path">
		'''          the path to the directory to open </param>
		''' <param name="options">
		'''          options indicating how symbolic links are handled
		''' </param>
		''' <returns>  a new and open {@code SecureDirectoryStream} object
		''' </returns>
		''' <exception cref="ClosedDirectoryStreamException">
		'''          if the directory stream is closed </exception>
		''' <exception cref="NotDirectoryException">
		'''          if the file could not otherwise be opened because it is not
		'''          a directory <i>(optional specific exception)</i> </exception>
		''' <exception cref="IOException">
		'''          if an I/O error occurs </exception>
		''' <exception cref="SecurityException">
		'''          In the case of the default provider, and a security manager is
		'''          installed, the <seealso cref="SecurityManager#checkRead(String) checkRead"/>
		'''          method is invoked to check read access to the directory. </exception>
		Function newDirectoryStream(ByVal path As T, ParamArray ByVal options As LinkOption()) As SecureDirectoryStream(Of T)

		''' <summary>
		''' Opens or creates a file in this directory, returning a seekable byte
		''' channel to access the file.
		''' 
		''' <p> This method works in exactly the manner specified by the {@link
		''' Files#newByteChannel Files.newByteChannel} method for the
		''' case that the {@code path} parameter is an <seealso cref="Path#isAbsolute absolute"/>
		''' path. When the parameter is a relative path then the file to open or
		''' create is relative to this open directory. In addition to the options
		''' defined by the {@code Files.newByteChannel} method, the {@link
		''' LinkOption#NOFOLLOW_LINKS NOFOLLOW_LINKS} option may be used to
		''' ensure that this method fails if the file is a symbolic link.
		''' 
		''' <p> The channel, once created, is not dependent upon the directory stream
		''' used to create it. Closing this directory stream has no effect upon the
		''' channel.
		''' </summary>
		''' <param name="path">
		'''          the path of the file to open open or create </param>
		''' <param name="options">
		'''          options specifying how the file is opened </param>
		''' <param name="attrs">
		'''          an optional list of attributes to set atomically when creating
		'''          the file
		''' </param>
		''' <returns>  the seekable byte channel
		''' </returns>
		''' <exception cref="ClosedDirectoryStreamException">
		'''          if the directory stream is closed </exception>
		''' <exception cref="IllegalArgumentException">
		'''          if the set contains an invalid combination of options </exception>
		''' <exception cref="UnsupportedOperationException">
		'''          if an unsupported open option is specified or the array contains
		'''          attributes that cannot be set atomically when creating the file </exception>
		''' <exception cref="FileAlreadyExistsException">
		'''          if a file of that name already exists and the {@link
		'''          StandardOpenOption#CREATE_NEW CREATE_NEW} option is specified
		'''          <i>(optional specific exception)</i> </exception>
		''' <exception cref="IOException">
		'''          if an I/O error occurs </exception>
		''' <exception cref="SecurityException">
		'''          In the case of the default provider, and a security manager is
		'''          installed, the <seealso cref="SecurityManager#checkRead(String) checkRead"/>
		'''          method is invoked to check read access to the path if the file
		'''          is opened for reading. The {@link SecurityManager#checkWrite(String)
		'''          checkWrite} method is invoked to check write access to the path
		'''          if the file is opened for writing. </exception>
		Function newByteChannel(Of T1 As OpenOption, T2)(ByVal path As T, ByVal options As java.util.Set(Of T1), ParamArray ByVal attrs As FileAttribute(Of T2)()) As java.nio.channels.SeekableByteChannel

		''' <summary>
		''' Deletes a file.
		''' 
		''' <p> Unlike the <seealso cref="Files#delete delete()"/> method, this method does
		''' not first examine the file to determine if the file is a directory.
		''' Whether a directory is deleted by this method is system dependent and
		''' therefore not specified. If the file is a symbolic link, then the link
		''' itself, not the final target of the link, is deleted. When the
		''' parameter is a relative path then the file to delete is relative to
		''' this open directory.
		''' </summary>
		''' <param name="path">
		'''          the path of the file to delete
		''' </param>
		''' <exception cref="ClosedDirectoryStreamException">
		'''          if the directory stream is closed </exception>
		''' <exception cref="NoSuchFileException">
		'''          if the file does not exist <i>(optional specific exception)</i> </exception>
		''' <exception cref="IOException">
		'''          if an I/O error occurs </exception>
		''' <exception cref="SecurityException">
		'''          In the case of the default provider, and a security manager is
		'''          installed, the <seealso cref="SecurityManager#checkDelete(String) checkDelete"/>
		'''          method is invoked to check delete access to the file </exception>
		Sub deleteFile(ByVal path As T)

		''' <summary>
		''' Deletes a directory.
		''' 
		''' <p> Unlike the <seealso cref="Files#delete delete()"/> method, this method
		''' does not first examine the file to determine if the file is a directory.
		''' Whether non-directories are deleted by this method is system dependent and
		''' therefore not specified. When the parameter is a relative path then the
		''' directory to delete is relative to this open directory.
		''' </summary>
		''' <param name="path">
		'''          the path of the directory to delete
		''' </param>
		''' <exception cref="ClosedDirectoryStreamException">
		'''          if the directory stream is closed </exception>
		''' <exception cref="NoSuchFileException">
		'''          if the directory does not exist <i>(optional specific exception)</i> </exception>
		''' <exception cref="DirectoryNotEmptyException">
		'''          if the directory could not otherwise be deleted because it is
		'''          not empty <i>(optional specific exception)</i> </exception>
		''' <exception cref="IOException">
		'''          if an I/O error occurs </exception>
		''' <exception cref="SecurityException">
		'''          In the case of the default provider, and a security manager is
		'''          installed, the <seealso cref="SecurityManager#checkDelete(String) checkDelete"/>
		'''          method is invoked to check delete access to the directory </exception>
		Sub deleteDirectory(ByVal path As T)

		''' <summary>
		''' Move a file from this directory to another directory.
		''' 
		''' <p> This method works in a similar manner to <seealso cref="Files#move move"/>
		''' method when the <seealso cref="StandardCopyOption#ATOMIC_MOVE ATOMIC_MOVE"/> option
		''' is specified. That is, this method moves a file as an atomic file system
		''' operation. If the {@code srcpath} parameter is an {@link Path#isAbsolute
		''' absolute} path then it locates the source file. If the parameter is a
		''' relative path then it is located relative to this open directory. If
		''' the {@code targetpath} parameter is absolute then it locates the target
		''' file (the {@code targetdir} parameter is ignored). If the parameter is
		''' a relative path it is located relative to the open directory identified
		''' by the {@code targetdir} parameter. In all cases, if the target file
		''' exists then it is implementation specific if it is replaced or this
		''' method fails.
		''' </summary>
		''' <param name="srcpath">
		'''          the name of the file to move </param>
		''' <param name="targetdir">
		'''          the destination directory </param>
		''' <param name="targetpath">
		'''          the name to give the file in the destination directory
		''' </param>
		''' <exception cref="ClosedDirectoryStreamException">
		'''          if this or the target directory stream is closed </exception>
		''' <exception cref="FileAlreadyExistsException">
		'''          if the file already exists in the target directory and cannot
		'''          be replaced <i>(optional specific exception)</i> </exception>
		''' <exception cref="AtomicMoveNotSupportedException">
		'''          if the file cannot be moved as an atomic file system operation </exception>
		''' <exception cref="IOException">
		'''          if an I/O error occurs </exception>
		''' <exception cref="SecurityException">
		'''          In the case of the default provider, and a security manager is
		'''          installed, the <seealso cref="SecurityManager#checkWrite(String) checkWrite"/>
		'''          method is invoked to check write access to both the source and
		'''          target file. </exception>
		Sub move(ByVal srcpath As T, ByVal targetdir As SecureDirectoryStream(Of T), ByVal targetpath As T)

		''' <summary>
		''' Returns a new file attribute view to access the file attributes of this
		''' directory.
		''' 
		''' <p> The resulting file attribute view can be used to read or update the
		''' attributes of this (open) directory. The {@code type} parameter specifies
		''' the type of the attribute view and the method returns an instance of that
		''' type if supported. Invoking this method to obtain a {@link
		''' BasicFileAttributeView} always returns an instance of that class that is
		''' bound to this open directory.
		''' 
		''' <p> The state of resulting file attribute view is intimately connected
		''' to this directory stream. Once the directory stream is <seealso cref="#close closed"/>,
		''' then all methods to read or update attributes will throw {@link
		''' ClosedDirectoryStreamException ClosedDirectoryStreamException}.
		''' </summary>
		''' @param   <V>
		'''          The {@code FileAttributeView} type </param>
		''' <param name="type">
		'''          the {@code Class} object corresponding to the file attribute view
		''' </param>
		''' <returns>  a new file attribute view of the specified type bound to
		'''          this directory stream, or {@code null} if the attribute view
		'''          type is not available </returns>
		 Function getFileAttributeView(Of V As FileAttributeView)(ByVal type As Class) As V

		''' <summary>
		''' Returns a new file attribute view to access the file attributes of a file
		''' in this directory.
		''' 
		''' <p> The resulting file attribute view can be used to read or update the
		''' attributes of file in this directory. The {@code type} parameter specifies
		''' the type of the attribute view and the method returns an instance of that
		''' type if supported. Invoking this method to obtain a {@link
		''' BasicFileAttributeView} always returns an instance of that class that is
		''' bound to the file in the directory.
		''' 
		''' <p> The state of resulting file attribute view is intimately connected
		''' to this directory stream. Once the directory stream <seealso cref="#close closed"/>,
		''' then all methods to read or update attributes will throw {@link
		''' ClosedDirectoryStreamException ClosedDirectoryStreamException}. The
		''' file is not required to exist at the time that the file attribute view
		''' is created but methods to read or update attributes of the file will
		''' fail when invoked and the file does not exist.
		''' </summary>
		''' @param   <V>
		'''          The {@code FileAttributeView} type </param>
		''' <param name="path">
		'''          the path of the file </param>
		''' <param name="type">
		'''          the {@code Class} object corresponding to the file attribute view </param>
		''' <param name="options">
		'''          options indicating how symbolic links are handled
		''' </param>
		''' <returns>  a new file attribute view of the specified type bound to a
		'''          this directory stream, or {@code null} if the attribute view
		'''          type is not available
		'''  </returns>
		 Function getFileAttributeView(Of V As FileAttributeView)(ByVal path As T, ByVal type As Class, ParamArray ByVal options As LinkOption()) As V
	End Interface

End Namespace