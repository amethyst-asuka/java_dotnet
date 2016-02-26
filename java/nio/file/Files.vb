Imports System
Imports System.Diagnostics
Imports System.Collections.Generic
Imports java.lang

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
    ''' This class consists exclusively of static methods that operate on files,
    ''' directories, or other types of files.
    ''' 
    ''' <p> In most cases, the methods defined here will delegate to the associated
    ''' file system provider to perform the file operations.
    ''' 
    ''' @since 1.7
    ''' </summary>

    Public NotInheritable Class Files
        Private Sub New()
        End Sub

        ''' <summary>
        ''' Returns the {@code FileSystemProvider} to delegate to.
        ''' </summary>
        Private Shared Function provider(ByVal path As Path) As java.nio.file.spi.FileSystemProvider
            Return path.fileSystem.provider()
        End Function

        ''' <summary>
        ''' Convert a Closeable to a Runnable by converting checked IOException
        ''' to UncheckedIOException
        ''' </summary>
        Private Shared Function asUncheckedRunnable(ByVal c As java.io.Closeable) As Runnable
            Return () ->
				Try
					c.close()
            Catch e As java.io.IOException
            Throw New java.io.UncheckedIOException(e)
            End Try
        End Function

        ' -- File contents --

        ''' <summary>
        ''' Opens a file, returning an input stream to read from the file. The stream
        ''' will not be buffered, and is not required to support the {@link
        ''' InputStream#mark mark} or <seealso cref="InputStream#reset reset"/> methods. The
        ''' stream will be safe for access by multiple concurrent threads. Reading
        ''' commences at the beginning of the file. Whether the returned stream is
        ''' <i>asynchronously closeable</i> and/or <i>interruptible</i> is highly
        ''' file system provider specific and therefore not specified.
        ''' 
        ''' <p> The {@code options} parameter determines how the file is opened.
        ''' If no options are present then it is equivalent to opening the file with
        ''' the <seealso cref="StandardOpenOption#READ READ"/> option. In addition to the {@code
        ''' READ} option, an implementation may also support additional implementation
        ''' specific options.
        ''' </summary>
        ''' <param name="path">
        '''          the path to the file to open </param>
        ''' <param name="options">
        '''          options specifying how the file is opened
        ''' </param>
        ''' <returns>  a new input stream
        ''' </returns>
        ''' <exception cref="IllegalArgumentException">
        '''          if an invalid combination of options is specified </exception>
        ''' <exception cref="UnsupportedOperationException">
        '''          if an unsupported option is specified </exception>
        ''' <exception cref="IOException">
        '''          if an I/O error occurs </exception>
        ''' <exception cref="SecurityException">
        '''          In the case of the default provider, and a security manager is
        '''          installed, the <seealso cref="SecurityManager#checkRead(String) checkRead"/>
        '''          method is invoked to check read access to the file. </exception>
        Public Shared Function newInputStream(ByVal path As Path, ParamArray ByVal options As OpenOption()) As java.io.InputStream
            Return provider(path).newInputStream(path, options)
        End Function

        ''' <summary>
        ''' Opens or creates a file, returning an output stream that may be used to
        ''' write bytes to the file. The resulting stream will not be buffered. The
        ''' stream will be safe for access by multiple concurrent threads. Whether
        ''' the returned stream is <i>asynchronously closeable</i> and/or
        ''' <i>interruptible</i> is highly file system provider specific and
        ''' therefore not specified.
        ''' 
        ''' <p> This method opens or creates a file in exactly the manner specified
        ''' by the <seealso cref="#newByteChannel(Path,Set,FileAttribute[]) newByteChannel"/>
        ''' method with the exception that the <seealso cref="StandardOpenOption#READ READ"/>
        ''' option may not be present in the array of options. If no options are
        ''' present then this method works as if the {@link StandardOpenOption#CREATE
        ''' CREATE}, <seealso cref="StandardOpenOption#TRUNCATE_EXISTING TRUNCATE_EXISTING"/>,
        ''' and <seealso cref="StandardOpenOption#WRITE WRITE"/> options are present. In other
        ''' words, it opens the file for writing, creating the file if it doesn't
        ''' exist, or initially truncating an existing {@link #isRegularFile
        ''' regular-file} to a size of {@code 0} if it exists.
        ''' 
        ''' <p> <b>Usage Examples:</b>
        ''' <pre>
        '''     Path path = ...
        ''' 
        '''     // truncate and overwrite an existing file, or create the file if
        '''     // it doesn't initially exist
        '''     OutputStream out = Files.newOutputStream(path);
        ''' 
        '''     // append to an existing file, fail if the file does not exist
        '''     out = Files.newOutputStream(path, APPEND);
        ''' 
        '''     // append to an existing file, create file if it doesn't initially exist
        '''     out = Files.newOutputStream(path, CREATE, APPEND);
        ''' 
        '''     // always create new file, failing if it already exists
        '''     out = Files.newOutputStream(path, CREATE_NEW);
        ''' </pre>
        ''' </summary>
        ''' <param name="path">
        '''          the path to the file to open or create </param>
        ''' <param name="options">
        '''          options specifying how the file is opened
        ''' </param>
        ''' <returns>  a new output stream
        ''' </returns>
        ''' <exception cref="IllegalArgumentException">
        '''          if {@code options} contains an invalid combination of options </exception>
        ''' <exception cref="UnsupportedOperationException">
        '''          if an unsupported option is specified </exception>
        ''' <exception cref="IOException">
        '''          if an I/O error occurs </exception>
        ''' <exception cref="SecurityException">
        '''          In the case of the default provider, and a security manager is
        '''          installed, the <seealso cref="SecurityManager#checkWrite(String) checkWrite"/>
        '''          method is invoked to check write access to the file. The {@link
        '''          SecurityManager#checkDelete(String) checkDelete} method is
        '''          invoked to check delete access if the file is opened with the
        '''          {@code DELETE_ON_CLOSE} option. </exception>
        Public Shared Function newOutputStream(ByVal path As Path, ParamArray ByVal options As OpenOption()) As java.io.OutputStream
            Return provider(path).newOutputStream(path, options)
        End Function

        ''' <summary>
        ''' Opens or creates a file, returning a seekable byte channel to access the
        ''' file.
        ''' 
        ''' <p> The {@code options} parameter determines how the file is opened.
        ''' The <seealso cref="StandardOpenOption#READ READ"/> and {@link
        ''' StandardOpenOption#WRITE WRITE} options determine if the file should be
        ''' opened for reading and/or writing. If neither option (or the {@link
        ''' StandardOpenOption#APPEND APPEND} option) is present then the file is
        ''' opened for reading. By default reading or writing commence at the
        ''' beginning of the file.
        ''' 
        ''' <p> In the addition to {@code READ} and {@code WRITE}, the following
        ''' options may be present:
        ''' 
        ''' <table border=1 cellpadding=5 summary="Options">
        ''' <tr> <th>Option</th> <th>Description</th> </tr>
        ''' <tr>
        '''   <td> <seealso cref="StandardOpenOption#APPEND APPEND"/> </td>
        '''   <td> If this option is present then the file is opened for writing and
        '''     each invocation of the channel's {@code write} method first advances
        '''     the position to the end of the file and then writes the requested
        '''     data. Whether the advancement of the position and the writing of the
        '''     data are done in a single atomic operation is system-dependent and
        '''     therefore unspecified. This option may not be used in conjunction
        '''     with the {@code READ} or {@code TRUNCATE_EXISTING} options. </td>
        ''' </tr>
        ''' <tr>
        '''   <td> <seealso cref="StandardOpenOption#TRUNCATE_EXISTING TRUNCATE_EXISTING"/> </td>
        '''   <td> If this option is present then the existing file is truncated to
        '''   a size of 0 bytes. This option is ignored when the file is opened only
        '''   for reading. </td>
        ''' </tr>
        ''' <tr>
        '''   <td> <seealso cref="StandardOpenOption#CREATE_NEW CREATE_NEW"/> </td>
        '''   <td> If this option is present then a new file is created, failing if
        '''   the file already exists or is a symbolic link. When creating a file the
        '''   check for the existence of the file and the creation of the file if it
        '''   does not exist is atomic with respect to other file system operations.
        '''   This option is ignored when the file is opened only for reading. </td>
        ''' </tr>
        ''' <tr>
        '''   <td > <seealso cref="StandardOpenOption#CREATE CREATE"/> </td>
        '''   <td> If this option is present then an existing file is opened if it
        '''   exists, otherwise a new file is created. This option is ignored if the
        '''   {@code CREATE_NEW} option is also present or the file is opened only
        '''   for reading. </td>
        ''' </tr>
        ''' <tr>
        '''   <td > <seealso cref="StandardOpenOption#DELETE_ON_CLOSE DELETE_ON_CLOSE"/> </td>
        '''   <td> When this option is present then the implementation makes a
        '''   <em>best effort</em> attempt to delete the file when closed by the
        '''   <seealso cref="SeekableByteChannel#close close"/> method. If the {@code close}
        '''   method is not invoked then a <em>best effort</em> attempt is made to
        '''   delete the file when the Java virtual machine terminates. </td>
        ''' </tr>
        ''' <tr>
        '''   <td><seealso cref="StandardOpenOption#SPARSE SPARSE"/> </td>
        '''   <td> When creating a new file this option is a <em>hint</em> that the
        '''   new file will be sparse. This option is ignored when not creating
        '''   a new file. </td>
        ''' </tr>
        ''' <tr>
        '''   <td> <seealso cref="StandardOpenOption#SYNC SYNC"/> </td>
        '''   <td> Requires that every update to the file's content or metadata be
        '''   written synchronously to the underlying storage device. (see <a
        '''   href="package-summary.html#integrity"> Synchronized I/O file
        '''   integrity</a>). </td>
        ''' </tr>
        ''' <tr>
        '''   <td> <seealso cref="StandardOpenOption#DSYNC DSYNC"/> </td>
        '''   <td> Requires that every update to the file's content be written
        '''   synchronously to the underlying storage device. (see <a
        '''   href="package-summary.html#integrity"> Synchronized I/O file
        '''   integrity</a>). </td>
        ''' </tr>
        ''' </table>
        ''' 
        ''' <p> An implementation may also support additional implementation specific
        ''' options.
        ''' 
        ''' <p> The {@code attrs} parameter is optional {@link FileAttribute
        ''' file-attributes} to set atomically when a new file is created.
        ''' 
        ''' <p> In the case of the default provider, the returned seekable byte channel
        ''' is a <seealso cref="java.nio.channels.FileChannel"/>.
        ''' 
        ''' <p> <b>Usage Examples:</b>
        ''' <pre>
        '''     Path path = ...
        ''' 
        '''     // open file for reading
        '''     ReadableByteChannel rbc = Files.newByteChannel(path, EnumSet.of(READ)));
        ''' 
        '''     // open file for writing to the end of an existing file, creating
        '''     // the file if it doesn't already exist
        '''     WritableByteChannel wbc = Files.newByteChannel(path, EnumSet.of(CREATE,APPEND));
        ''' 
        '''     // create file with initial permissions, opening it for both reading and writing
        '''     {@code FileAttribute<Set<PosixFilePermission>> perms = ...}
        '''     SeekableByteChannel sbc = Files.newByteChannel(path, EnumSet.of(CREATE_NEW,READ,WRITE), perms);
        ''' </pre>
        ''' </summary>
        ''' <param name="path">
        '''          the path to the file to open or create </param>
        ''' <param name="options">
        '''          options specifying how the file is opened </param>
        ''' <param name="attrs">
        '''          an optional list of file attributes to set atomically when
        '''          creating the file
        ''' </param>
        ''' <returns>  a new seekable byte channel
        ''' </returns>
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
        '''          method is invoked to check read access to the path if the file is
        '''          opened for reading. The {@link SecurityManager#checkWrite(String)
        '''          checkWrite} method is invoked to check write access to the path
        '''          if the file is opened for writing. The {@link
        '''          SecurityManager#checkDelete(String) checkDelete} method is
        '''          invoked to check delete access if the file is opened with the
        '''          {@code DELETE_ON_CLOSE} option.
        ''' </exception>
        ''' <seealso cref= java.nio.channels.FileChannel#open(Path,Set,FileAttribute[]) </seealso>
        Public Shared Function newByteChannel(Of T1 As OpenOption, T2)(ByVal path As Path, ByVal options As java.util.Set(Of T1), ParamArray ByVal attrs As java.nio.file.attribute.FileAttribute(Of T2)()) As java.nio.channels.SeekableByteChannel
            Return provider(path).newByteChannel(path, options, attrs)
        End Function

        ''' <summary>
        ''' Opens or creates a file, returning a seekable byte channel to access the
        ''' file.
        ''' 
        ''' <p> This method opens or creates a file in exactly the manner specified
        ''' by the <seealso cref="#newByteChannel(Path,Set,FileAttribute[]) newByteChannel"/>
        ''' method.
        ''' </summary>
        ''' <param name="path">
        '''          the path to the file to open or create </param>
        ''' <param name="options">
        '''          options specifying how the file is opened
        ''' </param>
        ''' <returns>  a new seekable byte channel
        ''' </returns>
        ''' <exception cref="IllegalArgumentException">
        '''          if the set contains an invalid combination of options </exception>
        ''' <exception cref="UnsupportedOperationException">
        '''          if an unsupported open option is specified </exception>
        ''' <exception cref="FileAlreadyExistsException">
        '''          if a file of that name already exists and the {@link
        '''          StandardOpenOption#CREATE_NEW CREATE_NEW} option is specified
        '''          <i>(optional specific exception)</i> </exception>
        ''' <exception cref="IOException">
        '''          if an I/O error occurs </exception>
        ''' <exception cref="SecurityException">
        '''          In the case of the default provider, and a security manager is
        '''          installed, the <seealso cref="SecurityManager#checkRead(String) checkRead"/>
        '''          method is invoked to check read access to the path if the file is
        '''          opened for reading. The {@link SecurityManager#checkWrite(String)
        '''          checkWrite} method is invoked to check write access to the path
        '''          if the file is opened for writing. The {@link
        '''          SecurityManager#checkDelete(String) checkDelete} method is
        '''          invoked to check delete access if the file is opened with the
        '''          {@code DELETE_ON_CLOSE} option.
        ''' </exception>
        ''' <seealso cref= java.nio.channels.FileChannel#open(Path,OpenOption[]) </seealso>
        Public Shared Function newByteChannel(ByVal path As Path, ParamArray ByVal options As OpenOption()) As java.nio.channels.SeekableByteChannel
            Dim [set] As java.util.Set(Of OpenOption) = New HashSet(Of OpenOption)(options.Length)
            java.util.Collections.addAll([set], options)
            Return newByteChannel(path, [set])
        End Function

        ' -- Directories --

        Private Class AcceptAllFilter
            Implements DirectoryStream.Filter(Of Path)

            Private Sub New()
            End Sub

            Public Overrides Function accept(ByVal entry As Path) As Boolean
                Return True
            End Function

            Friend Shared ReadOnly FILTER As New AcceptAllFilter
        End Class

        ''' <summary>
        ''' Opens a directory, returning a <seealso cref="DirectoryStream"/> to iterate over
        ''' all entries in the directory. The elements returned by the directory
        ''' stream's <seealso cref="DirectoryStream#iterator iterator"/> are of type {@code
        ''' Path}, each one representing an entry in the directory. The {@code Path}
        ''' objects are obtained as if by <seealso cref="Path#resolve(Path) resolving"/> the
        ''' name of the directory entry against {@code dir}.
        ''' 
        ''' <p> When not using the try-with-resources construct, then directory
        ''' stream's {@code close} method should be invoked after iteration is
        ''' completed so as to free any resources held for the open directory.
        ''' 
        ''' <p> When an implementation supports operations on entries in the
        ''' directory that execute in a race-free manner then the returned directory
        ''' stream is a <seealso cref="SecureDirectoryStream"/>.
        ''' </summary>
        ''' <param name="dir">
        '''          the path to the directory
        ''' </param>
        ''' <returns>  a new and open {@code DirectoryStream} object
        ''' </returns>
        ''' <exception cref="NotDirectoryException">
        '''          if the file could not otherwise be opened because it is not
        '''          a directory <i>(optional specific exception)</i> </exception>
        ''' <exception cref="IOException">
        '''          if an I/O error occurs </exception>
        ''' <exception cref="SecurityException">
        '''          In the case of the default provider, and a security manager is
        '''          installed, the <seealso cref="SecurityManager#checkRead(String) checkRead"/>
        '''          method is invoked to check read access to the directory. </exception>
        Public Shared Function newDirectoryStream(ByVal dir As Path) As DirectoryStream(Of Path)
            Return provider(dir).newDirectoryStream(dir, AcceptAllFilter.FILTER)
        End Function

        ''' <summary>
        ''' Opens a directory, returning a <seealso cref="DirectoryStream"/> to iterate over
        ''' the entries in the directory. The elements returned by the directory
        ''' stream's <seealso cref="DirectoryStream#iterator iterator"/> are of type {@code
        ''' Path}, each one representing an entry in the directory. The {@code Path}
        ''' objects are obtained as if by <seealso cref="Path#resolve(Path) resolving"/> the
        ''' name of the directory entry against {@code dir}. The entries returned by
        ''' the iterator are filtered by matching the {@code String} representation
        ''' of their file names against the given <em>globbing</em> pattern.
        ''' 
        ''' <p> For example, suppose we want to iterate over the files ending with
        ''' ".java" in a directory:
        ''' <pre>
        '''     Path dir = ...
        '''     try (DirectoryStream&lt;Path&gt; stream = Files.newDirectoryStream(dir, "*.java")) {
        '''         :
        '''     }
        ''' </pre>
        ''' 
        ''' <p> The globbing pattern is specified by the {@link
        ''' FileSystem#getPathMatcher getPathMatcher} method.
        ''' 
        ''' <p> When not using the try-with-resources construct, then directory
        ''' stream's {@code close} method should be invoked after iteration is
        ''' completed so as to free any resources held for the open directory.
        ''' 
        ''' <p> When an implementation supports operations on entries in the
        ''' directory that execute in a race-free manner then the returned directory
        ''' stream is a <seealso cref="SecureDirectoryStream"/>.
        ''' </summary>
        ''' <param name="dir">
        '''          the path to the directory </param>
        ''' <param name="glob">
        '''          the glob pattern
        ''' </param>
        ''' <returns>  a new and open {@code DirectoryStream} object
        ''' </returns>
        ''' <exception cref="java.util.regex.PatternSyntaxException">
        '''          if the pattern is invalid </exception>
        ''' <exception cref="NotDirectoryException">
        '''          if the file could not otherwise be opened because it is not
        '''          a directory <i>(optional specific exception)</i> </exception>
        ''' <exception cref="IOException">
        '''          if an I/O error occurs </exception>
        ''' <exception cref="SecurityException">
        '''          In the case of the default provider, and a security manager is
        '''          installed, the <seealso cref="SecurityManager#checkRead(String) checkRead"/>
        '''          method is invoked to check read access to the directory. </exception>
        Public Shared Function newDirectoryStream(ByVal dir As Path, ByVal glob As String) As DirectoryStream(Of Path)
            ' avoid creating a matcher if all entries are required.
            If glob.Equals("*") Then Return newDirectoryStream(dir)

            ' create a matcher and return a filter that uses it.
            Dim fs As FileSystem = dir.fileSystem
            Dim matcher As PathMatcher = fs.getPathMatcher("glob:" & glob)
            'JAVA TO VB CONVERTER TODO TASK: Anonymous inner classes are not converted to VB if the base type is not defined in the code being converted:
            '			DirectoryStream.Filter<Path> filter = New DirectoryStream.Filter<Path>()
            '		{
            '			@Override public boolean accept(Path entry)
            '			{
            '				Return matcher.matches(entry.getFileName());
            '			}
            '		};
            Return fs.provider().newDirectoryStream(dir, Filter)
        End Function

        ''' <summary>
        ''' Opens a directory, returning a <seealso cref="DirectoryStream"/> to iterate over
        ''' the entries in the directory. The elements returned by the directory
        ''' stream's <seealso cref="DirectoryStream#iterator iterator"/> are of type {@code
        ''' Path}, each one representing an entry in the directory. The {@code Path}
        ''' objects are obtained as if by <seealso cref="Path#resolve(Path) resolving"/> the
        ''' name of the directory entry against {@code dir}. The entries returned by
        ''' the iterator are filtered by the given {@link DirectoryStream.Filter
        ''' filter}.
        ''' 
        ''' <p> When not using the try-with-resources construct, then directory
        ''' stream's {@code close} method should be invoked after iteration is
        ''' completed so as to free any resources held for the open directory.
        ''' 
        ''' <p> Where the filter terminates due to an uncaught error or runtime
        ''' exception then it is propagated to the {@link Iterator#hasNext()
        ''' hasNext} or <seealso cref="Iterator#next() next"/> method. Where an {@code
        ''' IOException} is thrown, it results in the {@code hasNext} or {@code
        ''' next} method throwing a <seealso cref="DirectoryIteratorException"/> with the
        ''' {@code IOException} as the cause.
        ''' 
        ''' <p> When an implementation supports operations on entries in the
        ''' directory that execute in a race-free manner then the returned directory
        ''' stream is a <seealso cref="SecureDirectoryStream"/>.
        ''' 
        ''' <p> <b>Usage Example:</b>
        ''' Suppose we want to iterate over the files in a directory that are
        ''' larger than 8K.
        ''' <pre>
        '''     DirectoryStream.Filter&lt;Path&gt; filter = new DirectoryStream.Filter&lt;Path&gt;() {
        '''         public boolean accept(Path file) throws IOException {
        '''             return (Files.size(file) &gt; 8192L);
        '''         }
        '''     };
        '''     Path dir = ...
        '''     try (DirectoryStream&lt;Path&gt; stream = Files.newDirectoryStream(dir, filter)) {
        '''         :
        '''     }
        ''' </pre>
        ''' </summary>
        ''' <param name="dir">
        '''          the path to the directory </param>
        ''' <param name="filter">
        '''          the directory stream filter
        ''' </param>
        ''' <returns>  a new and open {@code DirectoryStream} object
        ''' </returns>
        ''' <exception cref="NotDirectoryException">
        '''          if the file could not otherwise be opened because it is not
        '''          a directory <i>(optional specific exception)</i> </exception>
        ''' <exception cref="IOException">
        '''          if an I/O error occurs </exception>
        ''' <exception cref="SecurityException">
        '''          In the case of the default provider, and a security manager is
        '''          installed, the <seealso cref="SecurityManager#checkRead(String) checkRead"/>
        '''          method is invoked to check read access to the directory. </exception>
        'JAVA TO VB CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
        Public Shared Function newDirectoryStream(Of T1)(ByVal dir As Path, ByVal filter As DirectoryStream.Filter(Of T1)) As DirectoryStream(Of Path)
            Return provider(dir).newDirectoryStream(dir, filter)
        End Function

        ' -- Creation and deletion --

        ''' <summary>
        ''' Creates a new and empty file, failing if the file already exists. The
        ''' check for the existence of the file and the creation of the new file if
        ''' it does not exist are a single operation that is atomic with respect to
        ''' all other filesystem activities that might affect the directory.
        ''' 
        ''' <p> The {@code attrs} parameter is optional {@link FileAttribute
        ''' file-attributes} to set atomically when creating the file. Each attribute
        ''' is identified by its <seealso cref="FileAttribute#name name"/>. If more than one
        ''' attribute of the same name is included in the array then all but the last
        ''' occurrence is ignored.
        ''' </summary>
        ''' <param name="path">
        '''          the path to the file to create </param>
        ''' <param name="attrs">
        '''          an optional list of file attributes to set atomically when
        '''          creating the file
        ''' </param>
        ''' <returns>  the file
        ''' </returns>
        ''' <exception cref="UnsupportedOperationException">
        '''          if the array contains an attribute that cannot be set atomically
        '''          when creating the file </exception>
        ''' <exception cref="FileAlreadyExistsException">
        '''          if a file of that name already exists
        '''          <i>(optional specific exception)</i> </exception>
        ''' <exception cref="IOException">
        '''          if an I/O error occurs or the parent directory does not exist </exception>
        ''' <exception cref="SecurityException">
        '''          In the case of the default provider, and a security manager is
        '''          installed, the <seealso cref="SecurityManager#checkWrite(String) checkWrite"/>
        '''          method is invoked to check write access to the new file. </exception>
        Public Shared Function createFile(Of T1)(ByVal path As Path, ParamArray ByVal attrs As java.nio.file.attribute.FileAttribute(Of T1)()) As Path
            Dim options As java.util.EnumSet(Of StandardOpenOption) = java.util.EnumSet.of(Of StandardOpenOption)(StandardOpenOption.CREATE_NEW, StandardOpenOption.WRITE)
            newByteChannel(path, options, attrs).close()
            Return path
        End Function

        ''' <summary>
        ''' Creates a new directory. The check for the existence of the file and the
        ''' creation of the directory if it does not exist are a single operation
        ''' that is atomic with respect to all other filesystem activities that might
        ''' affect the directory. The <seealso cref="#createDirectories createDirectories"/>
        ''' method should be used where it is required to create all nonexistent
        ''' parent directories first.
        ''' 
        ''' <p> The {@code attrs} parameter is optional {@link FileAttribute
        ''' file-attributes} to set atomically when creating the directory. Each
        ''' attribute is identified by its <seealso cref="FileAttribute#name name"/>. If more
        ''' than one attribute of the same name is included in the array then all but
        ''' the last occurrence is ignored.
        ''' </summary>
        ''' <param name="dir">
        '''          the directory to create </param>
        ''' <param name="attrs">
        '''          an optional list of file attributes to set atomically when
        '''          creating the directory
        ''' </param>
        ''' <returns>  the directory
        ''' </returns>
        ''' <exception cref="UnsupportedOperationException">
        '''          if the array contains an attribute that cannot be set atomically
        '''          when creating the directory </exception>
        ''' <exception cref="FileAlreadyExistsException">
        '''          if a directory could not otherwise be created because a file of
        '''          that name already exists <i>(optional specific exception)</i> </exception>
        ''' <exception cref="IOException">
        '''          if an I/O error occurs or the parent directory does not exist </exception>
        ''' <exception cref="SecurityException">
        '''          In the case of the default provider, and a security manager is
        '''          installed, the <seealso cref="SecurityManager#checkWrite(String) checkWrite"/>
        '''          method is invoked to check write access to the new directory. </exception>
        Public Shared Function createDirectory(Of T1)(ByVal dir As Path, ParamArray ByVal attrs As java.nio.file.attribute.FileAttribute(Of T1)()) As Path
            provider(dir).createDirectory(dir, attrs)
            Return dir
        End Function

        ''' <summary>
        ''' Creates a directory by creating all nonexistent parent directories first.
        ''' Unlike the <seealso cref="#createDirectory createDirectory"/> method, an exception
        ''' is not thrown if the directory could not be created because it already
        ''' exists.
        ''' 
        ''' <p> The {@code attrs} parameter is optional {@link FileAttribute
        ''' file-attributes} to set atomically when creating the nonexistent
        ''' directories. Each file attribute is identified by its {@link
        ''' FileAttribute#name name}. If more than one attribute of the same name is
        ''' included in the array then all but the last occurrence is ignored.
        ''' 
        ''' <p> If this method fails, then it may do so after creating some, but not
        ''' all, of the parent directories.
        ''' </summary>
        ''' <param name="dir">
        '''          the directory to create
        ''' </param>
        ''' <param name="attrs">
        '''          an optional list of file attributes to set atomically when
        '''          creating the directory
        ''' </param>
        ''' <returns>  the directory
        ''' </returns>
        ''' <exception cref="UnsupportedOperationException">
        '''          if the array contains an attribute that cannot be set atomically
        '''          when creating the directory </exception>
        ''' <exception cref="FileAlreadyExistsException">
        '''          if {@code dir} exists but is not a directory <i>(optional specific
        '''          exception)</i> </exception>
        ''' <exception cref="IOException">
        '''          if an I/O error occurs </exception>
        ''' <exception cref="SecurityException">
        '''          in the case of the default provider, and a security manager is
        '''          installed, the <seealso cref="SecurityManager#checkWrite(String) checkWrite"/>
        '''          method is invoked prior to attempting to create a directory and
        '''          its <seealso cref="SecurityManager#checkRead(String) checkRead"/> is
        '''          invoked for each parent directory that is checked. If {@code
        '''          dir} is not an absolute path then its {@link Path#toAbsolutePath
        '''          toAbsolutePath} may need to be invoked to get its absolute path.
        '''          This may invoke the security manager's {@link
        '''          SecurityManager#checkPropertyAccess(String) checkPropertyAccess}
        '''          method to check access to the system property {@code user.dir} </exception>
        Public Shared Function createDirectories(Of T1)(ByVal dir As Path, ParamArray ByVal attrs As java.nio.file.attribute.FileAttribute(Of T1)()) As Path
            ' attempt to create the directory
            Try
                createAndCheckIsDirectory(dir, attrs)
                Return dir
            Catch x As FileAlreadyExistsException
                ' file exists and is not a directory
                Throw x
            Catch x As java.io.IOException
                ' parent may not exist or other reason
            End Try
            Dim se As SecurityException = Nothing
            Try
                dir = dir.toAbsolutePath()
            Catch x As SecurityException
                ' don't have permission to get absolute path
                se = x
            End Try
            ' find a decendent that exists
            Dim parent As Path = dir.parent
            Do While parent IsNot Nothing
                Try
                    provider(parent).checkAccess(parent)
                    Exit Do
                Catch x As NoSuchFileException
                    ' does not exist
                End Try
                parent = parent.parent
            Loop
            If parent Is Nothing Then
                ' unable to find existing parent
                If se Is Nothing Then
                    Throw New FileSystemException(dir.ToString(), Nothing, "Unable to determine if root directory exists")
                Else
                    Throw se
                End If
            End If

            ' create directories
            Dim child As Path = parent
            For Each name As Path In parent.relativize(dir)
                child = child.resolve(name)
                createAndCheckIsDirectory(child, attrs)
            Next name
            Return dir
        End Function

        ''' <summary>
        ''' Used by createDirectories to attempt to create a directory. A no-op
        ''' if the directory already exists.
        ''' </summary>
        Private Shared Sub createAndCheckIsDirectory(Of T1)(ByVal dir As Path, ParamArray ByVal attrs As java.nio.file.attribute.FileAttribute(Of T1)())
            Try
                createDirectory(dir, attrs)
            Catch x As FileAlreadyExistsException
                If Not isDirectory(dir, LinkOption.NOFOLLOW_LINKS) Then Throw x
            End Try
        End Sub

        ''' <summary>
        ''' Creates a new empty file in the specified directory, using the given
        ''' prefix and suffix strings to generate its name. The resulting
        ''' {@code Path} is associated with the same {@code FileSystem} as the given
        ''' directory.
        ''' 
        ''' <p> The details as to how the name of the file is constructed is
        ''' implementation dependent and therefore not specified. Where possible
        ''' the {@code prefix} and {@code suffix} are used to construct candidate
        ''' names in the same manner as the {@link
        ''' java.io.File#createTempFile(String,String,File)} method.
        ''' 
        ''' <p> As with the {@code File.createTempFile} methods, this method is only
        ''' part of a temporary-file facility. Where used as a <em>work files</em>,
        ''' the resulting file may be opened using the {@link
        ''' StandardOpenOption#DELETE_ON_CLOSE DELETE_ON_CLOSE} option so that the
        ''' file is deleted when the appropriate {@code close} method is invoked.
        ''' Alternatively, a <seealso cref="Runtime#addShutdownHook shutdown-hook"/>, or the
        ''' <seealso cref="java.io.File#deleteOnExit"/> mechanism may be used to delete the
        ''' file automatically.
        ''' 
        ''' <p> The {@code attrs} parameter is optional {@link FileAttribute
        ''' file-attributes} to set atomically when creating the file. Each attribute
        ''' is identified by its <seealso cref="FileAttribute#name name"/>. If more than one
        ''' attribute of the same name is included in the array then all but the last
        ''' occurrence is ignored. When no file attributes are specified, then the
        ''' resulting file may have more restrictive access permissions to files
        ''' created by the <seealso cref="java.io.File#createTempFile(String,String,File)"/>
        ''' method.
        ''' </summary>
        ''' <param name="dir">
        '''          the path to directory in which to create the file </param>
        ''' <param name="prefix">
        '''          the prefix string to be used in generating the file's name;
        '''          may be {@code null} </param>
        ''' <param name="suffix">
        '''          the suffix string to be used in generating the file's name;
        '''          may be {@code null}, in which case "{@code .tmp}" is used </param>
        ''' <param name="attrs">
        '''          an optional list of file attributes to set atomically when
        '''          creating the file
        ''' </param>
        ''' <returns>  the path to the newly created file that did not exist before
        '''          this method was invoked
        ''' </returns>
        ''' <exception cref="IllegalArgumentException">
        '''          if the prefix or suffix parameters cannot be used to generate
        '''          a candidate file name </exception>
        ''' <exception cref="UnsupportedOperationException">
        '''          if the array contains an attribute that cannot be set atomically
        '''          when creating the directory </exception>
        ''' <exception cref="IOException">
        '''          if an I/O error occurs or {@code dir} does not exist </exception>
        ''' <exception cref="SecurityException">
        '''          In the case of the default provider, and a security manager is
        '''          installed, the <seealso cref="SecurityManager#checkWrite(String) checkWrite"/>
        '''          method is invoked to check write access to the file. </exception>
        Public Shared Function createTempFile(Of T1)(ByVal dir As Path, ByVal prefix As String, ByVal suffix As String, ParamArray ByVal attrs As java.nio.file.attribute.FileAttribute(Of T1)()) As Path
            Return TempFileHelper.createTempFile(java.util.Objects.requireNonNull(dir), prefix, suffix, attrs)
        End Function

        ''' <summary>
        ''' Creates an empty file in the default temporary-file directory, using
        ''' the given prefix and suffix to generate its name. The resulting {@code
        ''' Path} is associated with the default {@code FileSystem}.
        ''' 
        ''' <p> This method works in exactly the manner specified by the
        ''' <seealso cref="#createTempFile(Path,String,String,FileAttribute[])"/> method for
        ''' the case that the {@code dir} parameter is the temporary-file directory.
        ''' </summary>
        ''' <param name="prefix">
        '''          the prefix string to be used in generating the file's name;
        '''          may be {@code null} </param>
        ''' <param name="suffix">
        '''          the suffix string to be used in generating the file's name;
        '''          may be {@code null}, in which case "{@code .tmp}" is used </param>
        ''' <param name="attrs">
        '''          an optional list of file attributes to set atomically when
        '''          creating the file
        ''' </param>
        ''' <returns>  the path to the newly created file that did not exist before
        '''          this method was invoked
        ''' </returns>
        ''' <exception cref="IllegalArgumentException">
        '''          if the prefix or suffix parameters cannot be used to generate
        '''          a candidate file name </exception>
        ''' <exception cref="UnsupportedOperationException">
        '''          if the array contains an attribute that cannot be set atomically
        '''          when creating the directory </exception>
        ''' <exception cref="IOException">
        '''          if an I/O error occurs or the temporary-file directory does not
        '''          exist </exception>
        ''' <exception cref="SecurityException">
        '''          In the case of the default provider, and a security manager is
        '''          installed, the <seealso cref="SecurityManager#checkWrite(String) checkWrite"/>
        '''          method is invoked to check write access to the file. </exception>
        Public Shared Function createTempFile(Of T1)(ByVal prefix As String, ByVal suffix As String, ParamArray ByVal attrs As java.nio.file.attribute.FileAttribute(Of T1)()) As Path
            Return TempFileHelper.createTempFile(Nothing, prefix, suffix, attrs)
        End Function

        ''' <summary>
        ''' Creates a new directory in the specified directory, using the given
        ''' prefix to generate its name.  The resulting {@code Path} is associated
        ''' with the same {@code FileSystem} as the given directory.
        ''' 
        ''' <p> The details as to how the name of the directory is constructed is
        ''' implementation dependent and therefore not specified. Where possible
        ''' the {@code prefix} is used to construct candidate names.
        ''' 
        ''' <p> As with the {@code createTempFile} methods, this method is only
        ''' part of a temporary-file facility. A {@link Runtime#addShutdownHook
        ''' shutdown-hook}, or the <seealso cref="java.io.File#deleteOnExit"/> mechanism may be
        ''' used to delete the directory automatically.
        ''' 
        ''' <p> The {@code attrs} parameter is optional {@link FileAttribute
        ''' file-attributes} to set atomically when creating the directory. Each
        ''' attribute is identified by its <seealso cref="FileAttribute#name name"/>. If more
        ''' than one attribute of the same name is included in the array then all but
        ''' the last occurrence is ignored.
        ''' </summary>
        ''' <param name="dir">
        '''          the path to directory in which to create the directory </param>
        ''' <param name="prefix">
        '''          the prefix string to be used in generating the directory's name;
        '''          may be {@code null} </param>
        ''' <param name="attrs">
        '''          an optional list of file attributes to set atomically when
        '''          creating the directory
        ''' </param>
        ''' <returns>  the path to the newly created directory that did not exist before
        '''          this method was invoked
        ''' </returns>
        ''' <exception cref="IllegalArgumentException">
        '''          if the prefix cannot be used to generate a candidate directory name </exception>
        ''' <exception cref="UnsupportedOperationException">
        '''          if the array contains an attribute that cannot be set atomically
        '''          when creating the directory </exception>
        ''' <exception cref="IOException">
        '''          if an I/O error occurs or {@code dir} does not exist </exception>
        ''' <exception cref="SecurityException">
        '''          In the case of the default provider, and a security manager is
        '''          installed, the <seealso cref="SecurityManager#checkWrite(String) checkWrite"/>
        '''          method is invoked to check write access when creating the
        '''          directory. </exception>
        Public Shared Function createTempDirectory(Of T1)(ByVal dir As Path, ByVal prefix As String, ParamArray ByVal attrs As java.nio.file.attribute.FileAttribute(Of T1)()) As Path
            Return TempFileHelper.createTempDirectory(java.util.Objects.requireNonNull(dir), prefix, attrs)
        End Function

        ''' <summary>
        ''' Creates a new directory in the default temporary-file directory, using
        ''' the given prefix to generate its name. The resulting {@code Path} is
        ''' associated with the default {@code FileSystem}.
        ''' 
        ''' <p> This method works in exactly the manner specified by {@link
        ''' #createTempDirectory(Path,String,FileAttribute[])} method for the case
        ''' that the {@code dir} parameter is the temporary-file directory.
        ''' </summary>
        ''' <param name="prefix">
        '''          the prefix string to be used in generating the directory's name;
        '''          may be {@code null} </param>
        ''' <param name="attrs">
        '''          an optional list of file attributes to set atomically when
        '''          creating the directory
        ''' </param>
        ''' <returns>  the path to the newly created directory that did not exist before
        '''          this method was invoked
        ''' </returns>
        ''' <exception cref="IllegalArgumentException">
        '''          if the prefix cannot be used to generate a candidate directory name </exception>
        ''' <exception cref="UnsupportedOperationException">
        '''          if the array contains an attribute that cannot be set atomically
        '''          when creating the directory </exception>
        ''' <exception cref="IOException">
        '''          if an I/O error occurs or the temporary-file directory does not
        '''          exist </exception>
        ''' <exception cref="SecurityException">
        '''          In the case of the default provider, and a security manager is
        '''          installed, the <seealso cref="SecurityManager#checkWrite(String) checkWrite"/>
        '''          method is invoked to check write access when creating the
        '''          directory. </exception>
        Public Shared Function createTempDirectory(Of T1)(ByVal prefix As String, ParamArray ByVal attrs As java.nio.file.attribute.FileAttribute(Of T1)()) As Path
            Return TempFileHelper.createTempDirectory(Nothing, prefix, attrs)
        End Function

        ''' <summary>
        ''' Creates a symbolic link to a target <i>(optional operation)</i>.
        ''' 
        ''' <p> The {@code target} parameter is the target of the link. It may be an
        ''' <seealso cref="Path#isAbsolute absolute"/> or relative path and may not exist. When
        ''' the target is a relative path then file system operations on the resulting
        ''' link are relative to the path of the link.
        ''' 
        ''' <p> The {@code attrs} parameter is optional {@link FileAttribute
        ''' attributes} to set atomically when creating the link. Each attribute is
        ''' identified by its <seealso cref="FileAttribute#name name"/>. If more than one attribute
        ''' of the same name is included in the array then all but the last occurrence
        ''' is ignored.
        ''' 
        ''' <p> Where symbolic links are supported, but the underlying <seealso cref="FileStore"/>
        ''' does not support symbolic links, then this may fail with an {@link
        ''' IOException}. Additionally, some operating systems may require that the
        ''' Java virtual machine be started with implementation specific privileges to
        ''' create symbolic links, in which case this method may throw {@code IOException}.
        ''' </summary>
        ''' <param name="link">
        '''          the path of the symbolic link to create </param>
        ''' <param name="target">
        '''          the target of the symbolic link </param>
        ''' <param name="attrs">
        '''          the array of attributes to set atomically when creating the
        '''          symbolic link
        ''' </param>
        ''' <returns>  the path to the symbolic link
        ''' </returns>
        ''' <exception cref="UnsupportedOperationException">
        '''          if the implementation does not support symbolic links or the
        '''          array contains an attribute that cannot be set atomically when
        '''          creating the symbolic link </exception>
        ''' <exception cref="FileAlreadyExistsException">
        '''          if a file with the name already exists <i>(optional specific
        '''          exception)</i> </exception>
        ''' <exception cref="IOException">
        '''          if an I/O error occurs </exception>
        ''' <exception cref="SecurityException">
        '''          In the case of the default provider, and a security manager
        '''          is installed, it denies <seealso cref="LinkPermission"/><tt>("symbolic")</tt>
        '''          or its <seealso cref="SecurityManager#checkWrite(String) checkWrite"/>
        '''          method denies write access to the path of the symbolic link. </exception>
        Public Shared Function createSymbolicLink(Of T1)(ByVal link As Path, ByVal target As Path, ParamArray ByVal attrs As java.nio.file.attribute.FileAttribute(Of T1)()) As Path
            provider(link).createSymbolicLink(link, target, attrs)
            Return link
        End Function

        ''' <summary>
        ''' Creates a new link (directory entry) for an existing file <i>(optional
        ''' operation)</i>.
        ''' 
        ''' <p> The {@code link} parameter locates the directory entry to create.
        ''' The {@code existing} parameter is the path to an existing file. This
        ''' method creates a new directory entry for the file so that it can be
        ''' accessed using {@code link} as the path. On some file systems this is
        ''' known as creating a "hard link". Whether the file attributes are
        ''' maintained for the file or for each directory entry is file system
        ''' specific and therefore not specified. Typically, a file system requires
        ''' that all links (directory entries) for a file be on the same file system.
        ''' Furthermore, on some platforms, the Java virtual machine may require to
        ''' be started with implementation specific privileges to create hard links
        ''' or to create links to directories.
        ''' </summary>
        ''' <param name="link">
        '''          the link (directory entry) to create </param>
        ''' <param name="existing">
        '''          a path to an existing file
        ''' </param>
        ''' <returns>  the path to the link (directory entry)
        ''' </returns>
        ''' <exception cref="UnsupportedOperationException">
        '''          if the implementation does not support adding an existing file
        '''          to a directory </exception>
        ''' <exception cref="FileAlreadyExistsException">
        '''          if the entry could not otherwise be created because a file of
        '''          that name already exists <i>(optional specific exception)</i> </exception>
        ''' <exception cref="IOException">
        '''          if an I/O error occurs </exception>
        ''' <exception cref="SecurityException">
        '''          In the case of the default provider, and a security manager
        '''          is installed, it denies <seealso cref="LinkPermission"/><tt>("hard")</tt>
        '''          or its <seealso cref="SecurityManager#checkWrite(String) checkWrite"/>
        '''          method denies write access to either the link or the
        '''          existing file. </exception>
        Public Shared Function createLink(ByVal link As Path, ByVal existing As Path) As Path
            provider(link).createLink(link, existing)
            Return link
        End Function

        ''' <summary>
        ''' Deletes a file.
        ''' 
        ''' <p> An implementation may require to examine the file to determine if the
        ''' file is a directory. Consequently this method may not be atomic with respect
        ''' to other file system operations.  If the file is a symbolic link then the
        ''' symbolic link itself, not the final target of the link, is deleted.
        ''' 
        ''' <p> If the file is a directory then the directory must be empty. In some
        ''' implementations a directory has entries for special files or links that
        ''' are created when the directory is created. In such implementations a
        ''' directory is considered empty when only the special entries exist.
        ''' This method can be used with the <seealso cref="#walkFileTree walkFileTree"/>
        ''' method to delete a directory and all entries in the directory, or an
        ''' entire <i>file-tree</i> where required.
        ''' 
        ''' <p> On some operating systems it may not be possible to remove a file when
        ''' it is open and in use by this Java virtual machine or other programs.
        ''' </summary>
        ''' <param name="path">
        '''          the path to the file to delete
        ''' </param>
        ''' <exception cref="NoSuchFileException">
        '''          if the file does not exist <i>(optional specific exception)</i> </exception>
        ''' <exception cref="DirectoryNotEmptyException">
        '''          if the file is a directory and could not otherwise be deleted
        '''          because the directory is not empty <i>(optional specific
        '''          exception)</i> </exception>
        ''' <exception cref="IOException">
        '''          if an I/O error occurs </exception>
        ''' <exception cref="SecurityException">
        '''          In the case of the default provider, and a security manager is
        '''          installed, the <seealso cref="SecurityManager#checkDelete(String)"/> method
        '''          is invoked to check delete access to the file </exception>
        Public Shared Sub delete(ByVal path As Path)
            provider(path).delete(path)
        End Sub

        ''' <summary>
        ''' Deletes a file if it exists.
        ''' 
        ''' <p> As with the <seealso cref="#delete(Path) delete(Path)"/> method, an
        ''' implementation may need to examine the file to determine if the file is a
        ''' directory. Consequently this method may not be atomic with respect to
        ''' other file system operations.  If the file is a symbolic link, then the
        ''' symbolic link itself, not the final target of the link, is deleted.
        ''' 
        ''' <p> If the file is a directory then the directory must be empty. In some
        ''' implementations a directory has entries for special files or links that
        ''' are created when the directory is created. In such implementations a
        ''' directory is considered empty when only the special entries exist.
        ''' 
        ''' <p> On some operating systems it may not be possible to remove a file when
        ''' it is open and in use by this Java virtual machine or other programs.
        ''' </summary>
        ''' <param name="path">
        '''          the path to the file to delete
        ''' </param>
        ''' <returns>  {@code true} if the file was deleted by this method; {@code
        '''          false} if the file could not be deleted because it did not
        '''          exist
        ''' </returns>
        ''' <exception cref="DirectoryNotEmptyException">
        '''          if the file is a directory and could not otherwise be deleted
        '''          because the directory is not empty <i>(optional specific
        '''          exception)</i> </exception>
        ''' <exception cref="IOException">
        '''          if an I/O error occurs </exception>
        ''' <exception cref="SecurityException">
        '''          In the case of the default provider, and a security manager is
        '''          installed, the <seealso cref="SecurityManager#checkDelete(String)"/> method
        '''          is invoked to check delete access to the file. </exception>
        Public Shared Function deleteIfExists(ByVal path As Path) As Boolean
            Return provider(path).deleteIfExists(path)
        End Function

        ' -- Copying and moving files --

        ''' <summary>
        ''' Copy a file to a target file.
        ''' 
        ''' <p> This method copies a file to the target file with the {@code
        ''' options} parameter specifying how the copy is performed. By default, the
        ''' copy fails if the target file already exists or is a symbolic link,
        ''' except if the source and target are the <seealso cref="#isSameFile same"/> file, in
        ''' which case the method completes without copying the file. File attributes
        ''' are not required to be copied to the target file. If symbolic links are
        ''' supported, and the file is a symbolic link, then the final target of the
        ''' link is copied. If the file is a directory then it creates an empty
        ''' directory in the target location (entries in the directory are not
        ''' copied). This method can be used with the {@link #walkFileTree
        ''' walkFileTree} method to copy a directory and all entries in the directory,
        ''' or an entire <i>file-tree</i> where required.
        ''' 
        ''' <p> The {@code options} parameter may include any of the following:
        ''' 
        ''' <table border=1 cellpadding=5 summary="">
        ''' <tr> <th>Option</th> <th>Description</th> </tr>
        ''' <tr>
        '''   <td> <seealso cref="StandardCopyOption#REPLACE_EXISTING REPLACE_EXISTING"/> </td>
        '''   <td> If the target file exists, then the target file is replaced if it
        '''     is not a non-empty directory. If the target file exists and is a
        '''     symbolic link, then the symbolic link itself, not the target of
        '''     the link, is replaced. </td>
        ''' </tr>
        ''' <tr>
        '''   <td> <seealso cref="StandardCopyOption#COPY_ATTRIBUTES COPY_ATTRIBUTES"/> </td>
        '''   <td> Attempts to copy the file attributes associated with this file to
        '''     the target file. The exact file attributes that are copied is platform
        '''     and file system dependent and therefore unspecified. Minimally, the
        '''     <seealso cref="BasicFileAttributes#lastModifiedTime last-modified-time"/> is
        '''     copied to the target file if supported by both the source and target
        '''     file stores. Copying of file timestamps may result in precision
        '''     loss. </td>
        ''' </tr>
        ''' <tr>
        '''   <td> <seealso cref="LinkOption#NOFOLLOW_LINKS NOFOLLOW_LINKS"/> </td>
        '''   <td> Symbolic links are not followed. If the file is a symbolic link,
        '''     then the symbolic link itself, not the target of the link, is copied.
        '''     It is implementation specific if file attributes can be copied to the
        '''     new link. In other words, the {@code COPY_ATTRIBUTES} option may be
        '''     ignored when copying a symbolic link. </td>
        ''' </tr>
        ''' </table>
        ''' 
        ''' <p> An implementation of this interface may support additional
        ''' implementation specific options.
        ''' 
        ''' <p> Copying a file is not an atomic operation. If an <seealso cref="IOException"/>
        ''' is thrown, then it is possible that the target file is incomplete or some
        ''' of its file attributes have not been copied from the source file. When
        ''' the {@code REPLACE_EXISTING} option is specified and the target file
        ''' exists, then the target file is replaced. The check for the existence of
        ''' the file and the creation of the new file may not be atomic with respect
        ''' to other file system activities.
        ''' 
        ''' <p> <b>Usage Example:</b>
        ''' Suppose we want to copy a file into a directory, giving it the same file
        ''' name as the source file:
        ''' <pre>
        '''     Path source = ...
        '''     Path newdir = ...
        '''     Files.copy(source, newdir.resolve(source.getFileName());
        ''' </pre>
        ''' </summary>
        ''' <param name="source">
        '''          the path to the file to copy </param>
        ''' <param name="target">
        '''          the path to the target file (may be associated with a different
        '''          provider to the source path) </param>
        ''' <param name="options">
        '''          options specifying how the copy should be done
        ''' </param>
        ''' <returns>  the path to the target file
        ''' </returns>
        ''' <exception cref="UnsupportedOperationException">
        '''          if the array contains a copy option that is not supported </exception>
        ''' <exception cref="FileAlreadyExistsException">
        '''          if the target file exists but cannot be replaced because the
        '''          {@code REPLACE_EXISTING} option is not specified <i>(optional
        '''          specific exception)</i> </exception>
        ''' <exception cref="DirectoryNotEmptyException">
        '''          the {@code REPLACE_EXISTING} option is specified but the file
        '''          cannot be replaced because it is a non-empty directory
        '''          <i>(optional specific exception)</i> </exception>
        ''' <exception cref="IOException">
        '''          if an I/O error occurs </exception>
        ''' <exception cref="SecurityException">
        '''          In the case of the default provider, and a security manager is
        '''          installed, the <seealso cref="SecurityManager#checkRead(String) checkRead"/>
        '''          method is invoked to check read access to the source file, the
        '''          <seealso cref="SecurityManager#checkWrite(String) checkWrite"/> is invoked
        '''          to check write access to the target file. If a symbolic link is
        '''          copied the security manager is invoked to check {@link
        '''          LinkPermission}{@code ("symbolic")}. </exception>
        Public Shared Function copy(ByVal source As Path, ByVal target As Path, ParamArray ByVal options As CopyOption()) As Path
            Dim provider As java.nio.file.spi.FileSystemProvider = provider(source)
            If provider(target) Is provider Then
                ' same provider
                provider.copy(source, target, options)
            Else
                ' different providers
                CopyMoveHelper.copyToForeignTarget(source, target, options)
            End If
            Return target
        End Function

        ''' <summary>
        ''' Move or rename a file to a target file.
        ''' 
        ''' <p> By default, this method attempts to move the file to the target
        ''' file, failing if the target file exists except if the source and
        ''' target are the <seealso cref="#isSameFile same"/> file, in which case this method
        ''' has no effect. If the file is a symbolic link then the symbolic link
        ''' itself, not the target of the link, is moved. This method may be
        ''' invoked to move an empty directory. In some implementations a directory
        ''' has entries for special files or links that are created when the
        ''' directory is created. In such implementations a directory is considered
        ''' empty when only the special entries exist. When invoked to move a
        ''' directory that is not empty then the directory is moved if it does not
        ''' require moving the entries in the directory.  For example, renaming a
        ''' directory on the same <seealso cref="FileStore"/> will usually not require moving
        ''' the entries in the directory. When moving a directory requires that its
        ''' entries be moved then this method fails (by throwing an {@code
        ''' IOException}). To move a <i>file tree</i> may involve copying rather
        ''' than moving directories and this can be done using the {@link
        ''' #copy copy} method in conjunction with the {@link
        ''' #walkFileTree Files.walkFileTree} utility method.
        ''' 
        ''' <p> The {@code options} parameter may include any of the following:
        ''' 
        ''' <table border=1 cellpadding=5 summary="">
        ''' <tr> <th>Option</th> <th>Description</th> </tr>
        ''' <tr>
        '''   <td> <seealso cref="StandardCopyOption#REPLACE_EXISTING REPLACE_EXISTING"/> </td>
        '''   <td> If the target file exists, then the target file is replaced if it
        '''     is not a non-empty directory. If the target file exists and is a
        '''     symbolic link, then the symbolic link itself, not the target of
        '''     the link, is replaced. </td>
        ''' </tr>
        ''' <tr>
        '''   <td> <seealso cref="StandardCopyOption#ATOMIC_MOVE ATOMIC_MOVE"/> </td>
        '''   <td> The move is performed as an atomic file system operation and all
        '''     other options are ignored. If the target file exists then it is
        '''     implementation specific if the existing file is replaced or this method
        '''     fails by throwing an <seealso cref="IOException"/>. If the move cannot be
        '''     performed as an atomic file system operation then {@link
        '''     AtomicMoveNotSupportedException} is thrown. This can arise, for
        '''     example, when the target location is on a different {@code FileStore}
        '''     and would require that the file be copied, or target location is
        '''     associated with a different provider to this object. </td>
        ''' </table>
        ''' 
        ''' <p> An implementation of this interface may support additional
        ''' implementation specific options.
        ''' 
        ''' <p> Moving a file will copy the {@link
        ''' BasicFileAttributes#lastModifiedTime last-modified-time} to the target
        ''' file if supported by both source and target file stores. Copying of file
        ''' timestamps may result in precision loss. An implementation may also
        ''' attempt to copy other file attributes but is not required to fail if the
        ''' file attributes cannot be copied. When the move is performed as
        ''' a non-atomic operation, and an {@code IOException} is thrown, then the
        ''' state of the files is not defined. The original file and the target file
        ''' may both exist, the target file may be incomplete or some of its file
        ''' attributes may not been copied from the original file.
        ''' 
        ''' <p> <b>Usage Examples:</b>
        ''' Suppose we want to rename a file to "newname", keeping the file in the
        ''' same directory:
        ''' <pre>
        '''     Path source = ...
        '''     Files.move(source, source.resolveSibling("newname"));
        ''' </pre>
        ''' Alternatively, suppose we want to move a file to new directory, keeping
        ''' the same file name, and replacing any existing file of that name in the
        ''' directory:
        ''' <pre>
        '''     Path source = ...
        '''     Path newdir = ...
        '''     Files.move(source, newdir.resolve(source.getFileName()), REPLACE_EXISTING);
        ''' </pre>
        ''' </summary>
        ''' <param name="source">
        '''          the path to the file to move </param>
        ''' <param name="target">
        '''          the path to the target file (may be associated with a different
        '''          provider to the source path) </param>
        ''' <param name="options">
        '''          options specifying how the move should be done
        ''' </param>
        ''' <returns>  the path to the target file
        ''' </returns>
        ''' <exception cref="UnsupportedOperationException">
        '''          if the array contains a copy option that is not supported </exception>
        ''' <exception cref="FileAlreadyExistsException">
        '''          if the target file exists but cannot be replaced because the
        '''          {@code REPLACE_EXISTING} option is not specified <i>(optional
        '''          specific exception)</i> </exception>
        ''' <exception cref="DirectoryNotEmptyException">
        '''          the {@code REPLACE_EXISTING} option is specified but the file
        '''          cannot be replaced because it is a non-empty directory
        '''          <i>(optional specific exception)</i> </exception>
        ''' <exception cref="AtomicMoveNotSupportedException">
        '''          if the options array contains the {@code ATOMIC_MOVE} option but
        '''          the file cannot be moved as an atomic file system operation. </exception>
        ''' <exception cref="IOException">
        '''          if an I/O error occurs </exception>
        ''' <exception cref="SecurityException">
        '''          In the case of the default provider, and a security manager is
        '''          installed, the <seealso cref="SecurityManager#checkWrite(String) checkWrite"/>
        '''          method is invoked to check write access to both the source and
        '''          target file. </exception>
        Public Shared Function move(ByVal source As Path, ByVal target As Path, ParamArray ByVal options As CopyOption()) As Path
            Dim provider As java.nio.file.spi.FileSystemProvider = provider(source)
            If provider(target) Is provider Then
                ' same provider
                provider.move(source, target, options)
            Else
                ' different providers
                CopyMoveHelper.moveToForeignTarget(source, target, options)
            End If
            Return target
        End Function

        ' -- Miscellenous --

        ''' <summary>
        ''' Reads the target of a symbolic link <i>(optional operation)</i>.
        ''' 
        ''' <p> If the file system supports <a href="package-summary.html#links">symbolic
        ''' links</a> then this method is used to read the target of the link, failing
        ''' if the file is not a symbolic link. The target of the link need not exist.
        ''' The returned {@code Path} object will be associated with the same file
        ''' system as {@code link}.
        ''' </summary>
        ''' <param name="link">
        '''          the path to the symbolic link
        ''' </param>
        ''' <returns>  a {@code Path} object representing the target of the link
        ''' </returns>
        ''' <exception cref="UnsupportedOperationException">
        '''          if the implementation does not support symbolic links </exception>
        ''' <exception cref="NotLinkException">
        '''          if the target could otherwise not be read because the file
        '''          is not a symbolic link <i>(optional specific exception)</i> </exception>
        ''' <exception cref="IOException">
        '''          if an I/O error occurs </exception>
        ''' <exception cref="SecurityException">
        '''          In the case of the default provider, and a security manager
        '''          is installed, it checks that {@code FilePermission} has been
        '''          granted with the "{@code readlink}" action to read the link. </exception>
        Public Shared Function readSymbolicLink(ByVal link As Path) As Path
            Return provider(link).readSymbolicLink(link)
        End Function

        ''' <summary>
        ''' Returns the <seealso cref="FileStore"/> representing the file store where a file
        ''' is located.
        ''' 
        ''' <p> Once a reference to the {@code FileStore} is obtained it is
        ''' implementation specific if operations on the returned {@code FileStore},
        ''' or <seealso cref="FileStoreAttributeView"/> objects obtained from it, continue
        ''' to depend on the existence of the file. In particular the behavior is not
        ''' defined for the case that the file is deleted or moved to a different
        ''' file store.
        ''' </summary>
        ''' <param name="path">
        '''          the path to the file
        ''' </param>
        ''' <returns>  the file store where the file is stored
        ''' </returns>
        ''' <exception cref="IOException">
        '''          if an I/O error occurs </exception>
        ''' <exception cref="SecurityException">
        '''          In the case of the default provider, and a security manager is
        '''          installed, the <seealso cref="SecurityManager#checkRead(String) checkRead"/>
        '''          method is invoked to check read access to the file, and in
        '''          addition it checks <seealso cref="RuntimePermission"/><tt>
        '''          ("getFileStoreAttributes")</tt> </exception>
        Public Shared Function getFileStore(ByVal path As Path) As FileStore
            Return provider(path).getFileStore(path)
        End Function

        ''' <summary>
        ''' Tests if two paths locate the same file.
        ''' 
        ''' <p> If both {@code Path} objects are <seealso cref="Path#equals(Object) equal"/>
        ''' then this method returns {@code true} without checking if the file exists.
        ''' If the two {@code Path} objects are associated with different providers
        ''' then this method returns {@code false}. Otherwise, this method checks if
        ''' both {@code Path} objects locate the same file, and depending on the
        ''' implementation, may require to open or access both files.
        ''' 
        ''' <p> If the file system and files remain static, then this method implements
        ''' an equivalence relation for non-null {@code Paths}.
        ''' <ul>
        ''' <li>It is <i>reflexive</i>: for {@code Path} {@code f},
        '''     {@code isSameFile(f,f)} should return {@code true}.
        ''' <li>It is <i>symmetric</i>: for two {@code Paths} {@code f} and {@code g},
        '''     {@code isSameFile(f,g)} will equal {@code isSameFile(g,f)}.
        ''' <li>It is <i>transitive</i>: for three {@code Paths}
        '''     {@code f}, {@code g}, and {@code h}, if {@code isSameFile(f,g)} returns
        '''     {@code true} and {@code isSameFile(g,h)} returns {@code true}, then
        '''     {@code isSameFile(f,h)} will return return {@code true}.
        ''' </ul>
        ''' </summary>
        ''' <param name="path">
        '''          one path to the file </param>
        ''' <param name="path2">
        '''          the other path
        ''' </param>
        ''' <returns>  {@code true} if, and only if, the two paths locate the same file
        ''' </returns>
        ''' <exception cref="IOException">
        '''          if an I/O error occurs </exception>
        ''' <exception cref="SecurityException">
        '''          In the case of the default provider, and a security manager is
        '''          installed, the <seealso cref="SecurityManager#checkRead(String) checkRead"/>
        '''          method is invoked to check read access to both files.
        ''' </exception>
        ''' <seealso cref= java.nio.file.attribute.BasicFileAttributes#fileKey </seealso>
        Public Shared Function isSameFile(ByVal path As Path, ByVal path2 As Path) As Boolean
            Return provider(path).isSameFile(path, path2)
        End Function

        ''' <summary>
        ''' Tells whether or not a file is considered <em>hidden</em>. The exact
        ''' definition of hidden is platform or provider dependent. On UNIX for
        ''' example a file is considered to be hidden if its name begins with a
        ''' period character ('.'). On Windows a file is considered hidden if it
        ''' isn't a directory and the DOS <seealso cref="DosFileAttributes#isHidden hidden"/>
        ''' attribute is set.
        ''' 
        ''' <p> Depending on the implementation this method may require to access
        ''' the file system to determine if the file is considered hidden.
        ''' </summary>
        ''' <param name="path">
        '''          the path to the file to test
        ''' </param>
        ''' <returns>  {@code true} if the file is considered hidden
        ''' </returns>
        ''' <exception cref="IOException">
        '''          if an I/O error occurs </exception>
        ''' <exception cref="SecurityException">
        '''          In the case of the default provider, and a security manager is
        '''          installed, the <seealso cref="SecurityManager#checkRead(String) checkRead"/>
        '''          method is invoked to check read access to the file. </exception>
        Public Shared Function isHidden(ByVal path As Path) As Boolean
            Return provider(path).isHidden(path)
        End Function

        ' lazy loading of default and installed file type detectors
        Private Class FileTypeDetectors
            Friend Shared ReadOnly defaultFileTypeDetector As java.nio.file.spi.FileTypeDetector = createDefaultFileTypeDetector()
            Friend Shared ReadOnly installeDetectors As IList(Of java.nio.file.spi.FileTypeDetector) = loadInstalledDetectors()

            ' creates the default file type detector
            Private Shared Function createDefaultFileTypeDetector() As java.nio.file.spi.FileTypeDetector
                Return java.security.AccessController.doPrivileged(New PrivilegedActionAnonymousInnerClassHelper(Of T)
            End Function

            Private Class PrivilegedActionAnonymousInnerClassHelper(Of T)
                Implements java.security.PrivilegedAction(Of T)

                Public Overrides Function run() As java.nio.file.spi.FileTypeDetector
                    Return sun.nio.fs.DefaultFileTypeDetector.create()
                End Function
            End Class

            ' loads all installed file type detectors
            Private Shared Function loadInstalledDetectors() As IList(Of java.nio.file.spi.FileTypeDetector)
                Return java.security.AccessController.doPrivileged(New PrivilegedActionAnonymousInnerClassHelper2(Of T)
            End Function

            Private Class PrivilegedActionAnonymousInnerClassHelper2(Of T)
                Implements java.security.PrivilegedAction(Of T)

                Public Overrides Function run() As IList(Of java.nio.file.spi.FileTypeDetector)
                    Dim list As IList(Of java.nio.file.spi.FileTypeDetector) = New List(Of java.nio.file.spi.FileTypeDetector)
                    Dim loader As java.util.ServiceLoader(Of java.nio.file.spi.FileTypeDetector) = java.util.ServiceLoader.load(GetType(java.nio.file.spi.FileTypeDetector), ClassLoader.systemClassLoader)
                    For Each detector As java.nio.file.spi.FileTypeDetector In loader
                        list.Add(detector)
                    Next detector
                    Return list
                End Function
            End Class
        End Class

        ''' <summary>
        ''' Probes the content type of a file.
        ''' 
        ''' <p> This method uses the installed <seealso cref="FileTypeDetector"/> implementations
        ''' to probe the given file to determine its content type. Each file type
        ''' detector's <seealso cref="FileTypeDetector#probeContentType probeContentType"/> is
        ''' invoked, in turn, to probe the file type. If the file is recognized then
        ''' the content type is returned. If the file is not recognized by any of the
        ''' installed file type detectors then a system-default file type detector is
        ''' invoked to guess the content type.
        ''' 
        ''' <p> A given invocation of the Java virtual machine maintains a system-wide
        ''' list of file type detectors. Installed file type detectors are loaded
        ''' using the service-provider loading facility defined by the <seealso cref="ServiceLoader"/>
        ''' class. Installed file type detectors are loaded using the system class
        ''' loader. If the system class loader cannot be found then the extension class
        ''' loader is used; If the extension class loader cannot be found then the
        ''' bootstrap class loader is used. File type detectors are typically installed
        ''' by placing them in a JAR file on the application class path or in the
        ''' extension directory, the JAR file contains a provider-configuration file
        ''' named {@code java.nio.file.spi.FileTypeDetector} in the resource directory
        ''' {@code META-INF/services}, and the file lists one or more fully-qualified
        ''' names of concrete subclass of {@code FileTypeDetector } that have a zero
        ''' argument constructor. If the process of locating or instantiating the
        ''' installed file type detectors fails then an unspecified error is thrown.
        ''' The ordering that installed providers are located is implementation
        ''' specific.
        ''' 
        ''' <p> The return value of this method is the string form of the value of a
        ''' Multipurpose Internet Mail Extension (MIME) content type as
        ''' defined by <a href="http://www.ietf.org/rfc/rfc2045.txt"><i>RFC&nbsp;2045:
        ''' Multipurpose Internet Mail Extensions (MIME) Part One: Format of Internet
        ''' Message Bodies</i></a>. The string is guaranteed to be parsable according
        ''' to the grammar in the RFC.
        ''' </summary>
        ''' <param name="path">
        '''          the path to the file to probe
        ''' </param>
        ''' <returns>  The content type of the file, or {@code null} if the content
        '''          type cannot be determined
        ''' </returns>
        ''' <exception cref="IOException">
        '''          if an I/O error occurs </exception>
        ''' <exception cref="SecurityException">
        '''          If a security manager is installed and it denies an unspecified
        '''          permission required by a file type detector implementation. </exception>
        Public Shared Function probeContentType(ByVal path As Path) As String
            ' try installed file type detectors
            For Each detector As java.nio.file.spi.FileTypeDetector In FileTypeDetectors.installeDetectors
                Dim result As String = detector.probeContentType(path)
                If result IsNot Nothing Then Return result
            Next detector

            ' fallback to default
            Return FileTypeDetectors.defaultFileTypeDetector.probeContentType(path)
        End Function

        ' -- File Attributes --

        ''' <summary>
        ''' Returns a file attribute view of a given type.
        ''' 
        ''' <p> A file attribute view provides a read-only or updatable view of a
        ''' set of file attributes. This method is intended to be used where the file
        ''' attribute view defines type-safe methods to read or update the file
        ''' attributes. The {@code type} parameter is the type of the attribute view
        ''' required and the method returns an instance of that type if supported.
        ''' The <seealso cref="BasicFileAttributeView"/> type supports access to the basic
        ''' attributes of a file. Invoking this method to select a file attribute
        ''' view of that type will always return an instance of that class.
        ''' 
        ''' <p> The {@code options} array may be used to indicate how symbolic links
        ''' are handled by the resulting file attribute view for the case that the
        ''' file is a symbolic link. By default, symbolic links are followed. If the
        ''' option <seealso cref="LinkOption#NOFOLLOW_LINKS NOFOLLOW_LINKS"/> is present then
        ''' symbolic links are not followed. This option is ignored by implementations
        ''' that do not support symbolic links.
        ''' 
        ''' <p> <b>Usage Example:</b>
        ''' Suppose we want read or set a file's ACL, if supported:
        ''' <pre>
        '''     Path path = ...
        '''     AclFileAttributeView view = Files.getFileAttributeView(path, AclFileAttributeView.class);
        '''     if (view != null) {
        '''         List&lt;AclEntry&gt; acl = view.getAcl();
        '''         :
        '''     }
        ''' </pre>
        ''' </summary>
        ''' @param   <V>
        '''          The {@code FileAttributeView} type </param>
        ''' <param name="path">
        '''          the path to the file </param>
        ''' <param name="type">
        '''          the {@code Class} object corresponding to the file attribute view </param>
        ''' <param name="options">
        '''          options indicating how symbolic links are handled
        ''' </param>
        ''' <returns>  a file attribute view of the specified type, or {@code null} if
        '''          the attribute view type is not available </returns>
        Public Shared Function getFileAttributeView(Of V As java.nio.file.attribute.FileAttributeView)(ByVal path As Path, ByVal type As [Class], ParamArray ByVal options As LinkOption()) As V
            Return provider(path).getFileAttributeView(path, type, options)
        End Function

        ''' <summary>
        ''' Reads a file's attributes as a bulk operation.
        ''' 
        ''' <p> The {@code type} parameter is the type of the attributes required
        ''' and this method returns an instance of that type if supported. All
        ''' implementations support a basic set of file attributes and so invoking
        ''' this method with a  {@code type} parameter of {@code
        ''' BasicFileAttributes.class} will not throw {@code
        ''' UnsupportedOperationException}.
        ''' 
        ''' <p> The {@code options} array may be used to indicate how symbolic links
        ''' are handled for the case that the file is a symbolic link. By default,
        ''' symbolic links are followed and the file attribute of the final target
        ''' of the link is read. If the option {@link LinkOption#NOFOLLOW_LINKS
        ''' NOFOLLOW_LINKS} is present then symbolic links are not followed.
        ''' 
        ''' <p> It is implementation specific if all file attributes are read as an
        ''' atomic operation with respect to other file system operations.
        ''' 
        ''' <p> <b>Usage Example:</b>
        ''' Suppose we want to read a file's attributes in bulk:
        ''' <pre>
        '''    Path path = ...
        '''    BasicFileAttributes attrs = Files.readAttributes(path, BasicFileAttributes.class);
        ''' </pre>
        ''' Alternatively, suppose we want to read file's POSIX attributes without
        ''' following symbolic links:
        ''' <pre>
        '''    PosixFileAttributes attrs = Files.readAttributes(path, PosixFileAttributes.class, NOFOLLOW_LINKS);
        ''' </pre>
        ''' </summary>
        ''' @param   <A>
        '''          The {@code BasicFileAttributes} type </param>
        ''' <param name="path">
        '''          the path to the file </param>
        ''' <param name="type">
        '''          the {@code Class} of the file attributes required
        '''          to read </param>
        ''' <param name="options">
        '''          options indicating how symbolic links are handled
        ''' </param>
        ''' <returns>  the file attributes
        ''' </returns>
        ''' <exception cref="UnsupportedOperationException">
        '''          if an attributes of the given type are not supported </exception>
        ''' <exception cref="IOException">
        '''          if an I/O error occurs </exception>
        ''' <exception cref="SecurityException">
        '''          In the case of the default provider, a security manager is
        '''          installed, its <seealso cref="SecurityManager#checkRead(String) checkRead"/>
        '''          method is invoked to check read access to the file. If this
        '''          method is invoked to read security sensitive attributes then the
        '''          security manager may be invoke to check for additional permissions. </exception>
        Public Shared Function readAttributes(Of A As java.nio.file.attribute.BasicFileAttributes)(ByVal path As Path, ByVal type As [Class], ParamArray ByVal options As LinkOption()) As A
            Return provider(path).readAttributes(path, type, options)
        End Function

        ''' <summary>
        ''' Sets the value of a file attribute.
        ''' 
        ''' <p> The {@code attribute} parameter identifies the attribute to be set
        ''' and takes the form:
        ''' <blockquote>
        ''' [<i>view-name</i><b>:</b>]<i>attribute-name</i>
        ''' </blockquote>
        ''' where square brackets [...] delineate an optional component and the
        ''' character {@code ':'} stands for itself.
        ''' 
        ''' <p> <i>view-name</i> is the <seealso cref="FileAttributeView#name name"/> of a {@link
        ''' FileAttributeView} that identifies a set of file attributes. If not
        ''' specified then it defaults to {@code "basic"}, the name of the file
        ''' attribute view that identifies the basic set of file attributes common to
        ''' many file systems. <i>attribute-name</i> is the name of the attribute
        ''' within the set.
        ''' 
        ''' <p> The {@code options} array may be used to indicate how symbolic links
        ''' are handled for the case that the file is a symbolic link. By default,
        ''' symbolic links are followed and the file attribute of the final target
        ''' of the link is set. If the option {@link LinkOption#NOFOLLOW_LINKS
        ''' NOFOLLOW_LINKS} is present then symbolic links are not followed.
        ''' 
        ''' <p> <b>Usage Example:</b>
        ''' Suppose we want to set the DOS "hidden" attribute:
        ''' <pre>
        '''    Path path = ...
        '''    Files.setAttribute(path, "dos:hidden", true);
        ''' </pre>
        ''' </summary>
        ''' <param name="path">
        '''          the path to the file </param>
        ''' <param name="attribute">
        '''          the attribute to set </param>
        ''' <param name="value">
        '''          the attribute value </param>
        ''' <param name="options">
        '''          options indicating how symbolic links are handled
        ''' </param>
        ''' <returns>  the {@code path} parameter
        ''' </returns>
        ''' <exception cref="UnsupportedOperationException">
        '''          if the attribute view is not available </exception>
        ''' <exception cref="IllegalArgumentException">
        '''          if the attribute name is not specified, or is not recognized, or
        '''          the attribute value is of the correct type but has an
        '''          inappropriate value </exception>
        ''' <exception cref="ClassCastException">
        '''          if the attribute value is not of the expected type or is a
        '''          collection containing elements that are not of the expected
        '''          type </exception>
        ''' <exception cref="IOException">
        '''          if an I/O error occurs </exception>
        ''' <exception cref="SecurityException">
        '''          In the case of the default provider, and a security manager is
        '''          installed, its <seealso cref="SecurityManager#checkWrite(String) checkWrite"/>
        '''          method denies write access to the file. If this method is invoked
        '''          to set security sensitive attributes then the security manager
        '''          may be invoked to check for additional permissions. </exception>
        Public Shared Function setAttribute(ByVal path As Path, ByVal attribute As String, ByVal value As Object, ParamArray ByVal options As LinkOption()) As Path
            provider(path).attributeute(path, attribute, value, options)
            Return path
        End Function

        ''' <summary>
        ''' Reads the value of a file attribute.
        ''' 
        ''' <p> The {@code attribute} parameter identifies the attribute to be read
        ''' and takes the form:
        ''' <blockquote>
        ''' [<i>view-name</i><b>:</b>]<i>attribute-name</i>
        ''' </blockquote>
        ''' where square brackets [...] delineate an optional component and the
        ''' character {@code ':'} stands for itself.
        ''' 
        ''' <p> <i>view-name</i> is the <seealso cref="FileAttributeView#name name"/> of a {@link
        ''' FileAttributeView} that identifies a set of file attributes. If not
        ''' specified then it defaults to {@code "basic"}, the name of the file
        ''' attribute view that identifies the basic set of file attributes common to
        ''' many file systems. <i>attribute-name</i> is the name of the attribute.
        ''' 
        ''' <p> The {@code options} array may be used to indicate how symbolic links
        ''' are handled for the case that the file is a symbolic link. By default,
        ''' symbolic links are followed and the file attribute of the final target
        ''' of the link is read. If the option {@link LinkOption#NOFOLLOW_LINKS
        ''' NOFOLLOW_LINKS} is present then symbolic links are not followed.
        ''' 
        ''' <p> <b>Usage Example:</b>
        ''' Suppose we require the user ID of the file owner on a system that
        ''' supports a "{@code unix}" view:
        ''' <pre>
        '''    Path path = ...
        '''    int uid = (Integer)Files.getAttribute(path, "unix:uid");
        ''' </pre>
        ''' </summary>
        ''' <param name="path">
        '''          the path to the file </param>
        ''' <param name="attribute">
        '''          the attribute to read </param>
        ''' <param name="options">
        '''          options indicating how symbolic links are handled
        ''' </param>
        ''' <returns>  the attribute value
        ''' </returns>
        ''' <exception cref="UnsupportedOperationException">
        '''          if the attribute view is not available </exception>
        ''' <exception cref="IllegalArgumentException">
        '''          if the attribute name is not specified or is not recognized </exception>
        ''' <exception cref="IOException">
        '''          if an I/O error occurs </exception>
        ''' <exception cref="SecurityException">
        '''          In the case of the default provider, and a security manager is
        '''          installed, its <seealso cref="SecurityManager#checkRead(String) checkRead"/>
        '''          method denies read access to the file. If this method is invoked
        '''          to read security sensitive attributes then the security manager
        '''          may be invoked to check for additional permissions. </exception>
        Public Shared Function getAttribute(ByVal path As Path, ByVal attribute As String, ParamArray ByVal options As LinkOption()) As Object
            ' only one attribute should be read
            If attribute.IndexOf("*"c) >= 0 OrElse attribute.IndexOf(","c) >= 0 Then Throw New IllegalArgumentException(attribute)
            Dim map As IDictionary(Of String, Object) = readAttributes(path, attribute, options)
            Debug.Assert(map.Count = 1)
            Dim name As String
            Dim pos As Integer = attribute.IndexOf(":"c)
            If pos = -1 Then
                name = attribute
            Else
                name = If(pos = attribute.Length(), "", attribute.Substring(pos + 1))
            End If
            Return map(name)
        End Function

        ''' <summary>
        ''' Reads a set of file attributes as a bulk operation.
        ''' 
        ''' <p> The {@code attributes} parameter identifies the attributes to be read
        ''' and takes the form:
        ''' <blockquote>
        ''' [<i>view-name</i><b>:</b>]<i>attribute-list</i>
        ''' </blockquote>
        ''' where square brackets [...] delineate an optional component and the
        ''' character {@code ':'} stands for itself.
        ''' 
        ''' <p> <i>view-name</i> is the <seealso cref="FileAttributeView#name name"/> of a {@link
        ''' FileAttributeView} that identifies a set of file attributes. If not
        ''' specified then it defaults to {@code "basic"}, the name of the file
        ''' attribute view that identifies the basic set of file attributes common to
        ''' many file systems.
        ''' 
        ''' <p> The <i>attribute-list</i> component is a comma separated list of
        ''' zero or more names of attributes to read. If the list contains the value
        ''' {@code "*"} then all attributes are read. Attributes that are not supported
        ''' are ignored and will not be present in the returned map. It is
        ''' implementation specific if all attributes are read as an atomic operation
        ''' with respect to other file system operations.
        ''' 
        ''' <p> The following examples demonstrate possible values for the {@code
        ''' attributes} parameter:
        ''' 
        ''' <blockquote>
        ''' <table border="0" summary="Possible values">
        ''' <tr>
        '''   <td> {@code "*"} </td>
        '''   <td> Read all <seealso cref="BasicFileAttributes basic-file-attributes"/>. </td>
        ''' </tr>
        ''' <tr>
        '''   <td> {@code "size,lastModifiedTime,lastAccessTime"} </td>
        '''   <td> Reads the file size, last modified time, and last access time
        '''     attributes. </td>
        ''' </tr>
        ''' <tr>
        '''   <td> {@code "posix:*"} </td>
        '''   <td> Read all <seealso cref="PosixFileAttributes POSIX-file-attributes"/>. </td>
        ''' </tr>
        ''' <tr>
        '''   <td> {@code "posix:permissions,owner,size"} </td>
        '''   <td> Reads the POSX file permissions, owner, and file size. </td>
        ''' </tr>
        ''' </table>
        ''' </blockquote>
        ''' 
        ''' <p> The {@code options} array may be used to indicate how symbolic links
        ''' are handled for the case that the file is a symbolic link. By default,
        ''' symbolic links are followed and the file attribute of the final target
        ''' of the link is read. If the option {@link LinkOption#NOFOLLOW_LINKS
        ''' NOFOLLOW_LINKS} is present then symbolic links are not followed.
        ''' </summary>
        ''' <param name="path">
        '''          the path to the file </param>
        ''' <param name="attributes">
        '''          the attributes to read </param>
        ''' <param name="options">
        '''          options indicating how symbolic links are handled
        ''' </param>
        ''' <returns>  a map of the attributes returned; The map's keys are the
        '''          attribute names, its values are the attribute values
        ''' </returns>
        ''' <exception cref="UnsupportedOperationException">
        '''          if the attribute view is not available </exception>
        ''' <exception cref="IllegalArgumentException">
        '''          if no attributes are specified or an unrecognized attributes is
        '''          specified </exception>
        ''' <exception cref="IOException">
        '''          if an I/O error occurs </exception>
        ''' <exception cref="SecurityException">
        '''          In the case of the default provider, and a security manager is
        '''          installed, its <seealso cref="SecurityManager#checkRead(String) checkRead"/>
        '''          method denies read access to the file. If this method is invoked
        '''          to read security sensitive attributes then the security manager
        '''          may be invoke to check for additional permissions. </exception>
        Public Shared Function readAttributes(ByVal path As Path, ByVal attributes As String, ParamArray ByVal options As LinkOption()) As IDictionary(Of String, Object)
            Return provider(path).readAttributes(path, attributes, options)
        End Function

        ''' <summary>
        ''' Returns a file's POSIX file permissions.
        ''' 
        ''' <p> The {@code path} parameter is associated with a {@code FileSystem}
        ''' that supports the <seealso cref="PosixFileAttributeView"/>. This attribute view
        ''' provides access to file attributes commonly associated with files on file
        ''' systems used by operating systems that implement the Portable Operating
        ''' System Interface (POSIX) family of standards.
        ''' 
        ''' <p> The {@code options} array may be used to indicate how symbolic links
        ''' are handled for the case that the file is a symbolic link. By default,
        ''' symbolic links are followed and the file attribute of the final target
        ''' of the link is read. If the option {@link LinkOption#NOFOLLOW_LINKS
        ''' NOFOLLOW_LINKS} is present then symbolic links are not followed.
        ''' </summary>
        ''' <param name="path">
        '''          the path to the file </param>
        ''' <param name="options">
        '''          options indicating how symbolic links are handled
        ''' </param>
        ''' <returns>  the file permissions
        ''' </returns>
        ''' <exception cref="UnsupportedOperationException">
        '''          if the associated file system does not support the {@code
        '''          PosixFileAttributeView} </exception>
        ''' <exception cref="IOException">
        '''          if an I/O error occurs </exception>
        ''' <exception cref="SecurityException">
        '''          In the case of the default provider, a security manager is
        '''          installed, and it denies <seealso cref="RuntimePermission"/><tt>("accessUserInformation")</tt>
        '''          or its <seealso cref="SecurityManager#checkRead(String) checkRead"/> method
        '''          denies read access to the file. </exception>
        Public Shared Function getPosixFilePermissions(ByVal path As Path, ParamArray ByVal options As LinkOption()) As java.util.Set(Of java.nio.file.attribute.PosixFilePermission)
            Return readAttributes(path, GetType(java.nio.file.attribute.PosixFileAttributes), options).permissions()
        End Function

        ''' <summary>
        ''' Sets a file's POSIX permissions.
        ''' 
        ''' <p> The {@code path} parameter is associated with a {@code FileSystem}
        ''' that supports the <seealso cref="PosixFileAttributeView"/>. This attribute view
        ''' provides access to file attributes commonly associated with files on file
        ''' systems used by operating systems that implement the Portable Operating
        ''' System Interface (POSIX) family of standards.
        ''' </summary>
        ''' <param name="path">
        '''          The path to the file </param>
        ''' <param name="perms">
        '''          The new set of permissions
        ''' </param>
        ''' <returns>  The path
        ''' </returns>
        ''' <exception cref="UnsupportedOperationException">
        '''          if the associated file system does not support the {@code
        '''          PosixFileAttributeView} </exception>
        ''' <exception cref="ClassCastException">
        '''          if the sets contains elements that are not of type {@code
        '''          PosixFilePermission} </exception>
        ''' <exception cref="IOException">
        '''          if an I/O error occurs </exception>
        ''' <exception cref="SecurityException">
        '''          In the case of the default provider, and a security manager is
        '''          installed, it denies <seealso cref="RuntimePermission"/><tt>("accessUserInformation")</tt>
        '''          or its <seealso cref="SecurityManager#checkWrite(String) checkWrite"/>
        '''          method denies write access to the file. </exception>
        Public Shared Function setPosixFilePermissions(ByVal path As Path, ByVal perms As java.util.Set(Of java.nio.file.attribute.PosixFilePermission)) As Path
            Dim view As java.nio.file.attribute.PosixFileAttributeView = getFileAttributeView(path, GetType(java.nio.file.attribute.PosixFileAttributeView))
            If view Is Nothing Then Throw New UnsupportedOperationException
            view.permissions = perms
            Return path
        End Function

        ''' <summary>
        ''' Returns the owner of a file.
        ''' 
        ''' <p> The {@code path} parameter is associated with a file system that
        ''' supports <seealso cref="FileOwnerAttributeView"/>. This file attribute view provides
        ''' access to a file attribute that is the owner of the file.
        ''' </summary>
        ''' <param name="path">
        '''          The path to the file </param>
        ''' <param name="options">
        '''          options indicating how symbolic links are handled
        ''' </param>
        ''' <returns>  A user principal representing the owner of the file
        ''' </returns>
        ''' <exception cref="UnsupportedOperationException">
        '''          if the associated file system does not support the {@code
        '''          FileOwnerAttributeView} </exception>
        ''' <exception cref="IOException">
        '''          if an I/O error occurs </exception>
        ''' <exception cref="SecurityException">
        '''          In the case of the default provider, and a security manager is
        '''          installed, it denies <seealso cref="RuntimePermission"/><tt>("accessUserInformation")</tt>
        '''          or its <seealso cref="SecurityManager#checkRead(String) checkRead"/> method
        '''          denies read access to the file. </exception>
        Public Shared Function getOwner(ByVal path As Path, ParamArray ByVal options As LinkOption()) As java.nio.file.attribute.UserPrincipal
            Dim view As java.nio.file.attribute.FileOwnerAttributeView = getFileAttributeView(path, GetType(java.nio.file.attribute.FileOwnerAttributeView), options)
            If view Is Nothing Then Throw New UnsupportedOperationException
            Return view.owner
        End Function

        ''' <summary>
        ''' Updates the file owner.
        ''' 
        ''' <p> The {@code path} parameter is associated with a file system that
        ''' supports <seealso cref="FileOwnerAttributeView"/>. This file attribute view provides
        ''' access to a file attribute that is the owner of the file.
        ''' 
        ''' <p> <b>Usage Example:</b>
        ''' Suppose we want to make "joe" the owner of a file:
        ''' <pre>
        '''     Path path = ...
        '''     UserPrincipalLookupService lookupService =
        '''         provider(path).getUserPrincipalLookupService();
        '''     UserPrincipal joe = lookupService.lookupPrincipalByName("joe");
        '''     Files.setOwner(path, joe);
        ''' </pre>
        ''' </summary>
        ''' <param name="path">
        '''          The path to the file </param>
        ''' <param name="owner">
        '''          The new file owner
        ''' </param>
        ''' <returns>  The path
        ''' </returns>
        ''' <exception cref="UnsupportedOperationException">
        '''          if the associated file system does not support the {@code
        '''          FileOwnerAttributeView} </exception>
        ''' <exception cref="IOException">
        '''          if an I/O error occurs </exception>
        ''' <exception cref="SecurityException">
        '''          In the case of the default provider, and a security manager is
        '''          installed, it denies <seealso cref="RuntimePermission"/><tt>("accessUserInformation")</tt>
        '''          or its <seealso cref="SecurityManager#checkWrite(String) checkWrite"/>
        '''          method denies write access to the file.
        ''' </exception>
        ''' <seealso cref= FileSystem#getUserPrincipalLookupService </seealso>
        ''' <seealso cref= java.nio.file.attribute.UserPrincipalLookupService </seealso>
        Public Shared Function setOwner(ByVal path As Path, ByVal owner As java.nio.file.attribute.UserPrincipal) As Path
            Dim view As java.nio.file.attribute.FileOwnerAttributeView = getFileAttributeView(path, GetType(java.nio.file.attribute.FileOwnerAttributeView))
            If view Is Nothing Then Throw New UnsupportedOperationException
            view.owner = owner
            Return path
        End Function

        ''' <summary>
        ''' Tests whether a file is a symbolic link.
        ''' 
        ''' <p> Where it is required to distinguish an I/O exception from the case
        ''' that the file is not a symbolic link then the file attributes can be
        ''' read with the {@link #readAttributes(Path,Class,LinkOption[])
        ''' readAttributes} method and the file type tested with the {@link
        ''' BasicFileAttributes#isSymbolicLink} method.
        ''' </summary>
        ''' <param name="path">  The path to the file
        ''' </param>
        ''' <returns>  {@code true} if the file is a symbolic link; {@code false} if
        '''          the file does not exist, is not a symbolic link, or it cannot
        '''          be determined if the file is a symbolic link or not.
        ''' </returns>
        ''' <exception cref="SecurityException">
        '''          In the case of the default provider, and a security manager is
        '''          installed, its <seealso cref="SecurityManager#checkRead(String) checkRead"/>
        '''          method denies read access to the file. </exception>
        Public Shared Function isSymbolicLink(ByVal path As Path) As Boolean
            Try
                Return readAttributes(path, GetType(java.nio.file.attribute.BasicFileAttributes), LinkOption.NOFOLLOW_LINKS).symbolicLink
            Catch ioe As java.io.IOException
                Return False
            End Try
        End Function

        ''' <summary>
        ''' Tests whether a file is a directory.
        ''' 
        ''' <p> The {@code options} array may be used to indicate how symbolic links
        ''' are handled for the case that the file is a symbolic link. By default,
        ''' symbolic links are followed and the file attribute of the final target
        ''' of the link is read. If the option {@link LinkOption#NOFOLLOW_LINKS
        ''' NOFOLLOW_LINKS} is present then symbolic links are not followed.
        ''' 
        ''' <p> Where it is required to distinguish an I/O exception from the case
        ''' that the file is not a directory then the file attributes can be
        ''' read with the {@link #readAttributes(Path,Class,LinkOption[])
        ''' readAttributes} method and the file type tested with the {@link
        ''' BasicFileAttributes#isDirectory} method.
        ''' </summary>
        ''' <param name="path">
        '''          the path to the file to test </param>
        ''' <param name="options">
        '''          options indicating how symbolic links are handled
        ''' </param>
        ''' <returns>  {@code true} if the file is a directory; {@code false} if
        '''          the file does not exist, is not a directory, or it cannot
        '''          be determined if the file is a directory or not.
        ''' </returns>
        ''' <exception cref="SecurityException">
        '''          In the case of the default provider, and a security manager is
        '''          installed, its <seealso cref="SecurityManager#checkRead(String) checkRead"/>
        '''          method denies read access to the file. </exception>
        Public Shared Function isDirectory(ByVal path As Path, ParamArray ByVal options As LinkOption()) As Boolean
            Try
                Return readAttributes(path, GetType(java.nio.file.attribute.BasicFileAttributes), options).directory
            Catch ioe As java.io.IOException
                Return False
            End Try
        End Function

        ''' <summary>
        ''' Tests whether a file is a regular file with opaque content.
        ''' 
        ''' <p> The {@code options} array may be used to indicate how symbolic links
        ''' are handled for the case that the file is a symbolic link. By default,
        ''' symbolic links are followed and the file attribute of the final target
        ''' of the link is read. If the option {@link LinkOption#NOFOLLOW_LINKS
        ''' NOFOLLOW_LINKS} is present then symbolic links are not followed.
        ''' 
        ''' <p> Where it is required to distinguish an I/O exception from the case
        ''' that the file is not a regular file then the file attributes can be
        ''' read with the {@link #readAttributes(Path,Class,LinkOption[])
        ''' readAttributes} method and the file type tested with the {@link
        ''' BasicFileAttributes#isRegularFile} method.
        ''' </summary>
        ''' <param name="path">
        '''          the path to the file </param>
        ''' <param name="options">
        '''          options indicating how symbolic links are handled
        ''' </param>
        ''' <returns>  {@code true} if the file is a regular file; {@code false} if
        '''          the file does not exist, is not a regular file, or it
        '''          cannot be determined if the file is a regular file or not.
        ''' </returns>
        ''' <exception cref="SecurityException">
        '''          In the case of the default provider, and a security manager is
        '''          installed, its <seealso cref="SecurityManager#checkRead(String) checkRead"/>
        '''          method denies read access to the file. </exception>
        Public Shared Function isRegularFile(ByVal path As Path, ParamArray ByVal options As LinkOption()) As Boolean
            Try
                Return readAttributes(path, GetType(java.nio.file.attribute.BasicFileAttributes), options).regularFile
            Catch ioe As java.io.IOException
                Return False
            End Try
        End Function

        ''' <summary>
        ''' Returns a file's last modified time.
        ''' 
        ''' <p> The {@code options} array may be used to indicate how symbolic links
        ''' are handled for the case that the file is a symbolic link. By default,
        ''' symbolic links are followed and the file attribute of the final target
        ''' of the link is read. If the option {@link LinkOption#NOFOLLOW_LINKS
        ''' NOFOLLOW_LINKS} is present then symbolic links are not followed.
        ''' </summary>
        ''' <param name="path">
        '''          the path to the file </param>
        ''' <param name="options">
        '''          options indicating how symbolic links are handled
        ''' </param>
        ''' <returns>  a {@code FileTime} representing the time the file was last
        '''          modified, or an implementation specific default when a time
        '''          stamp to indicate the time of last modification is not supported
        '''          by the file system
        ''' </returns>
        ''' <exception cref="IOException">
        '''          if an I/O error occurs </exception>
        ''' <exception cref="SecurityException">
        '''          In the case of the default provider, and a security manager is
        '''          installed, its <seealso cref="SecurityManager#checkRead(String) checkRead"/>
        '''          method denies read access to the file.
        ''' </exception>
        ''' <seealso cref= BasicFileAttributes#lastModifiedTime </seealso>
        Public Shared Function getLastModifiedTime(ByVal path As Path, ParamArray ByVal options As LinkOption()) As java.nio.file.attribute.FileTime
            Return readAttributes(path, GetType(java.nio.file.attribute.BasicFileAttributes), options).lastModifiedTime()
        End Function

        ''' <summary>
        ''' Updates a file's last modified time attribute. The file time is converted
        ''' to the epoch and precision supported by the file system. Converting from
        ''' finer to coarser granularities result in precision loss. The behavior of
        ''' this method when attempting to set the last modified time when it is not
        ''' supported by the file system or is outside the range supported by the
        ''' underlying file store is not defined. It may or not fail by throwing an
        ''' {@code IOException}.
        ''' 
        ''' <p> <b>Usage Example:</b>
        ''' Suppose we want to set the last modified time to the current time:
        ''' <pre>
        '''    Path path = ...
        '''    FileTime now = FileTime.fromMillis(System.currentTimeMillis());
        '''    Files.setLastModifiedTime(path, now);
        ''' </pre>
        ''' </summary>
        ''' <param name="path">
        '''          the path to the file </param>
        ''' <param name="time">
        '''          the new last modified time
        ''' </param>
        ''' <returns>  the path
        ''' </returns>
        ''' <exception cref="IOException">
        '''          if an I/O error occurs </exception>
        ''' <exception cref="SecurityException">
        '''          In the case of the default provider, the security manager's {@link
        '''          SecurityManager#checkWrite(String) checkWrite} method is invoked
        '''          to check write access to file
        ''' </exception>
        ''' <seealso cref= BasicFileAttributeView#setTimes </seealso>
        Public Shared Function setLastModifiedTime(ByVal path As Path, ByVal time As java.nio.file.attribute.FileTime) As Path
            getFileAttributeView(path, GetType(java.nio.file.attribute.BasicFileAttributeView)).timesmes(time, Nothing, Nothing)
            Return path
        End Function

        ''' <summary>
        ''' Returns the size of a file (in bytes). The size may differ from the
        ''' actual size on the file system due to compression, support for sparse
        ''' files, or other reasons. The size of files that are not {@link
        ''' #isRegularFile regular} files is implementation specific and
        ''' therefore unspecified.
        ''' </summary>
        ''' <param name="path">
        '''          the path to the file
        ''' </param>
        ''' <returns>  the file size, in bytes
        ''' </returns>
        ''' <exception cref="IOException">
        '''          if an I/O error occurs </exception>
        ''' <exception cref="SecurityException">
        '''          In the case of the default provider, and a security manager is
        '''          installed, its <seealso cref="SecurityManager#checkRead(String) checkRead"/>
        '''          method denies read access to the file.
        ''' </exception>
        ''' <seealso cref= BasicFileAttributes#size </seealso>
        Public Shared Function size(ByVal path As Path) As Long
            Return readAttributes(path, GetType(java.nio.file.attribute.BasicFileAttributes)).size()
        End Function

        ' -- Accessibility --

        ''' <summary>
        ''' Returns {@code false} if NOFOLLOW_LINKS is present.
        ''' </summary>
        Private Shared Function followLinks(ParamArray ByVal options As LinkOption()) As Boolean
            Dim followLinks_Renamed As Boolean = True
            For Each opt As LinkOption In options
                If opt Is LinkOption.NOFOLLOW_LINKS Then
                    followLinks_Renamed = False
                    Continue For
                End If
                If opt Is Nothing Then Throw New NullPointerException
                Throw New AssertionError("Should not get here")
            Next opt
            Return followLinks_Renamed
        End Function

        ''' <summary>
        ''' Tests whether a file exists.
        ''' 
        ''' <p> The {@code options} parameter may be used to indicate how symbolic links
        ''' are handled for the case that the file is a symbolic link. By default,
        ''' symbolic links are followed. If the option {@link LinkOption#NOFOLLOW_LINKS
        ''' NOFOLLOW_LINKS} is present then symbolic links are not followed.
        ''' 
        ''' <p> Note that the result of this method is immediately outdated. If this
        ''' method indicates the file exists then there is no guarantee that a
        ''' subsequence access will succeed. Care should be taken when using this
        ''' method in security sensitive applications.
        ''' </summary>
        ''' <param name="path">
        '''          the path to the file to test </param>
        ''' <param name="options">
        '''          options indicating how symbolic links are handled
        ''' . </param>
        ''' <returns>  {@code true} if the file exists; {@code false} if the file does
        '''          not exist or its existence cannot be determined.
        ''' </returns>
        ''' <exception cref="SecurityException">
        '''          In the case of the default provider, the {@link
        '''          SecurityManager#checkRead(String)} is invoked to check
        '''          read access to the file.
        ''' </exception>
        ''' <seealso cref= #notExists </seealso>
        Public Shared Function exists(ByVal path As Path, ParamArray ByVal options As LinkOption()) As Boolean
            Try
                If followLinks(options) Then
                    provider(path).checkAccess(path)
                Else
                    ' attempt to read attributes without following links
                    readAttributes(path, GetType(java.nio.file.attribute.BasicFileAttributes), LinkOption.NOFOLLOW_LINKS)
                End If
                ' file exists
                Return True
            Catch x As java.io.IOException
                ' does not exist or unable to determine if file exists
                Return False
            End Try

        End Function

        ''' <summary>
        ''' Tests whether the file located by this path does not exist. This method
        ''' is intended for cases where it is required to take action when it can be
        ''' confirmed that a file does not exist.
        ''' 
        ''' <p> The {@code options} parameter may be used to indicate how symbolic links
        ''' are handled for the case that the file is a symbolic link. By default,
        ''' symbolic links are followed. If the option {@link LinkOption#NOFOLLOW_LINKS
        ''' NOFOLLOW_LINKS} is present then symbolic links are not followed.
        ''' 
        ''' <p> Note that this method is not the complement of the {@link #exists
        ''' exists} method. Where it is not possible to determine if a file exists
        ''' or not then both methods return {@code false}. As with the {@code exists}
        ''' method, the result of this method is immediately outdated. If this
        ''' method indicates the file does exist then there is no guarantee that a
        ''' subsequence attempt to create the file will succeed. Care should be taken
        ''' when using this method in security sensitive applications.
        ''' </summary>
        ''' <param name="path">
        '''          the path to the file to test </param>
        ''' <param name="options">
        '''          options indicating how symbolic links are handled
        ''' </param>
        ''' <returns>  {@code true} if the file does not exist; {@code false} if the
        '''          file exists or its existence cannot be determined
        ''' </returns>
        ''' <exception cref="SecurityException">
        '''          In the case of the default provider, the {@link
        '''          SecurityManager#checkRead(String)} is invoked to check
        '''          read access to the file. </exception>
        Public Shared Function notExists(ByVal path As Path, ParamArray ByVal options As LinkOption()) As Boolean
            Try
                If followLinks(options) Then
                    provider(path).checkAccess(path)
                Else
                    ' attempt to read attributes without following links
                    readAttributes(path, GetType(java.nio.file.attribute.BasicFileAttributes), LinkOption.NOFOLLOW_LINKS)
                End If
                ' file exists
                Return False
            Catch x As NoSuchFileException
                ' file confirmed not to exist
                Return True
            Catch x As java.io.IOException
                Return False
            End Try
        End Function

        ''' <summary>
        ''' Used by isReadbale, isWritable, isExecutable to test access to a file.
        ''' </summary>
        Private Shared Function isAccessible(ByVal path As Path, ParamArray ByVal modes As AccessMode()) As Boolean
            Try
                provider(path).checkAccess(path, modes)
                Return True
            Catch x As java.io.IOException
                Return False
            End Try
        End Function

        ''' <summary>
        ''' Tests whether a file is readable. This method checks that a file exists
        ''' and that this Java virtual machine has appropriate privileges that would
        ''' allow it open the file for reading. Depending on the implementation, this
        ''' method may require to read file permissions, access control lists, or
        ''' other file attributes in order to check the effective access to the file.
        ''' Consequently, this method may not be atomic with respect to other file
        ''' system operations.
        ''' 
        ''' <p> Note that the result of this method is immediately outdated, there is
        ''' no guarantee that a subsequent attempt to open the file for reading will
        ''' succeed (or even that it will access the same file). Care should be taken
        ''' when using this method in security sensitive applications.
        ''' </summary>
        ''' <param name="path">
        '''          the path to the file to check
        ''' </param>
        ''' <returns>  {@code true} if the file exists and is readable; {@code false}
        '''          if the file does not exist, read access would be denied because
        '''          the Java virtual machine has insufficient privileges, or access
        '''          cannot be determined
        ''' </returns>
        ''' <exception cref="SecurityException">
        '''          In the case of the default provider, and a security manager is
        '''          installed, the <seealso cref="SecurityManager#checkRead(String) checkRead"/>
        '''          is invoked to check read access to the file. </exception>
        Public Shared Function isReadable(ByVal path As Path) As Boolean
            Return isAccessible(path, AccessMode.READ)
        End Function

        ''' <summary>
        ''' Tests whether a file is writable. This method checks that a file exists
        ''' and that this Java virtual machine has appropriate privileges that would
        ''' allow it open the file for writing. Depending on the implementation, this
        ''' method may require to read file permissions, access control lists, or
        ''' other file attributes in order to check the effective access to the file.
        ''' Consequently, this method may not be atomic with respect to other file
        ''' system operations.
        ''' 
        ''' <p> Note that result of this method is immediately outdated, there is no
        ''' guarantee that a subsequent attempt to open the file for writing will
        ''' succeed (or even that it will access the same file). Care should be taken
        ''' when using this method in security sensitive applications.
        ''' </summary>
        ''' <param name="path">
        '''          the path to the file to check
        ''' </param>
        ''' <returns>  {@code true} if the file exists and is writable; {@code false}
        '''          if the file does not exist, write access would be denied because
        '''          the Java virtual machine has insufficient privileges, or access
        '''          cannot be determined
        ''' </returns>
        ''' <exception cref="SecurityException">
        '''          In the case of the default provider, and a security manager is
        '''          installed, the <seealso cref="SecurityManager#checkWrite(String) checkWrite"/>
        '''          is invoked to check write access to the file. </exception>
        Public Shared Function isWritable(ByVal path As Path) As Boolean
            Return isAccessible(path, AccessMode.WRITE)
        End Function

        ''' <summary>
        ''' Tests whether a file is executable. This method checks that a file exists
        ''' and that this Java virtual machine has appropriate privileges to {@link
        ''' Runtime#exec execute} the file. The semantics may differ when checking
        ''' access to a directory. For example, on UNIX systems, checking for
        ''' execute access checks that the Java virtual machine has permission to
        ''' search the directory in order to access file or subdirectories.
        ''' 
        ''' <p> Depending on the implementation, this method may require to read file
        ''' permissions, access control lists, or other file attributes in order to
        ''' check the effective access to the file. Consequently, this method may not
        ''' be atomic with respect to other file system operations.
        ''' 
        ''' <p> Note that the result of this method is immediately outdated, there is
        ''' no guarantee that a subsequent attempt to execute the file will succeed
        ''' (or even that it will access the same file). Care should be taken when
        ''' using this method in security sensitive applications.
        ''' </summary>
        ''' <param name="path">
        '''          the path to the file to check
        ''' </param>
        ''' <returns>  {@code true} if the file exists and is executable; {@code false}
        '''          if the file does not exist, execute access would be denied because
        '''          the Java virtual machine has insufficient privileges, or access
        '''          cannot be determined
        ''' </returns>
        ''' <exception cref="SecurityException">
        '''          In the case of the default provider, and a security manager is
        '''          installed, the {@link SecurityManager#checkExec(String)
        '''          checkExec} is invoked to check execute access to the file. </exception>
        Public Shared Function isExecutable(ByVal path As Path) As Boolean
            Return isAccessible(path, AccessMode.EXECUTE)
        End Function

        ' -- Recursive operations --

        ''' <summary>
        ''' Walks a file tree.
        ''' 
        ''' <p> This method walks a file tree rooted at a given starting file. The
        ''' file tree traversal is <em>depth-first</em> with the given {@link
        ''' FileVisitor} invoked for each file encountered. File tree traversal
        ''' completes when all accessible files in the tree have been visited, or a
        ''' visit method returns a result of {@link FileVisitResult#TERMINATE
        ''' TERMINATE}. Where a visit method terminates due an {@code IOException},
        ''' an uncaught error, or runtime exception, then the traversal is terminated
        ''' and the error or exception is propagated to the caller of this method.
        ''' 
        ''' <p> For each file encountered this method attempts to read its {@link
        ''' java.nio.file.attribute.BasicFileAttributes}. If the file is not a
        ''' directory then the <seealso cref="FileVisitor#visitFile visitFile"/> method is
        ''' invoked with the file attributes. If the file attributes cannot be read,
        ''' due to an I/O exception, then the {@link FileVisitor#visitFileFailed
        ''' visitFileFailed} method is invoked with the I/O exception.
        ''' 
        ''' <p> Where the file is a directory, and the directory could not be opened,
        ''' then the {@code visitFileFailed} method is invoked with the I/O exception,
        ''' after which, the file tree walk continues, by default, at the next
        ''' <em>sibling</em> of the directory.
        ''' 
        ''' <p> Where the directory is opened successfully, then the entries in the
        ''' directory, and their <em>descendants</em> are visited. When all entries
        ''' have been visited, or an I/O error occurs during iteration of the
        ''' directory, then the directory is closed and the visitor's {@link
        ''' FileVisitor#postVisitDirectory postVisitDirectory} method is invoked.
        ''' The file tree walk then continues, by default, at the next <em>sibling</em>
        ''' of the directory.
        ''' 
        ''' <p> By default, symbolic links are not automatically followed by this
        ''' method. If the {@code options} parameter contains the {@link
        ''' FileVisitOption#FOLLOW_LINKS FOLLOW_LINKS} option then symbolic links are
        ''' followed. When following links, and the attributes of the target cannot
        ''' be read, then this method attempts to get the {@code BasicFileAttributes}
        ''' of the link. If they can be read then the {@code visitFile} method is
        ''' invoked with the attributes of the link (otherwise the {@code visitFileFailed}
        ''' method is invoked as specified above).
        ''' 
        ''' <p> If the {@code options} parameter contains the {@link
        ''' FileVisitOption#FOLLOW_LINKS FOLLOW_LINKS} option then this method keeps
        ''' track of directories visited so that cycles can be detected. A cycle
        ''' arises when there is an entry in a directory that is an ancestor of the
        ''' directory. Cycle detection is done by recording the {@link
        ''' java.nio.file.attribute.BasicFileAttributes#fileKey file-key} of directories,
        ''' or if file keys are not available, by invoking the {@link #isSameFile
        ''' isSameFile} method to test if a directory is the same file as an
        ''' ancestor. When a cycle is detected it is treated as an I/O error, and the
        ''' <seealso cref="FileVisitor#visitFileFailed visitFileFailed"/> method is invoked with
        ''' an instance of <seealso cref="FileSystemLoopException"/>.
        ''' 
        ''' <p> The {@code maxDepth} parameter is the maximum number of levels of
        ''' directories to visit. A value of {@code 0} means that only the starting
        ''' file is visited, unless denied by the security manager. A value of
        ''' <seealso cref="Integer#MAX_VALUE MAX_VALUE"/> may be used to indicate that all
        ''' levels should be visited. The {@code visitFile} method is invoked for all
        ''' files, including directories, encountered at {@code maxDepth}, unless the
        ''' basic file attributes cannot be read, in which case the {@code
        ''' visitFileFailed} method is invoked.
        ''' 
        ''' <p> If a visitor returns a result of {@code null} then {@code
        ''' NullPointerException} is thrown.
        ''' 
        ''' <p> When a security manager is installed and it denies access to a file
        ''' (or directory), then it is ignored and the visitor is not invoked for
        ''' that file (or directory).
        ''' </summary>
        ''' <param name="start">
        '''          the starting file </param>
        ''' <param name="options">
        '''          options to configure the traversal </param>
        ''' <param name="maxDepth">
        '''          the maximum number of directory levels to visit </param>
        ''' <param name="visitor">
        '''          the file visitor to invoke for each file
        ''' </param>
        ''' <returns>  the starting file
        ''' </returns>
        ''' <exception cref="IllegalArgumentException">
        '''          if the {@code maxDepth} parameter is negative </exception>
        ''' <exception cref="SecurityException">
        '''          If the security manager denies access to the starting file.
        '''          In the case of the default provider, the {@link
        '''          SecurityManager#checkRead(String) checkRead} method is invoked
        '''          to check read access to the directory. </exception>
        ''' <exception cref="IOException">
        '''          if an I/O error is thrown by a visitor method </exception>
        'JAVA TO VB CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
        Public Shared Function walkFileTree(Of T1)(ByVal start As Path, ByVal options As java.util.Set(Of FileVisitOption), ByVal maxDepth As Integer, ByVal visitor As FileVisitor(Of T1)) As Path
            ''' <summary>
            ''' Create a FileTreeWalker to walk the file tree, invoking the visitor
            ''' for each event.
            ''' </summary>
            Using walker As New FileTreeWalker(options, maxDepth)
                Dim ev As FileTreeWalker.Event = walker.walk(start)
                Do
                    Dim result As FileVisitResult
                    Select Case ev.type()
                        Case ENTRY
                            Dim ioe As java.io.IOException = ev.ioeException()
                            If ioe Is Nothing Then
                                Debug.Assert(ev.attributes() IsNot Nothing)
                                result = visitor.visitFile(ev.file(), ev.attributes())
                            Else
                                result = visitor.visitFileFailed(ev.file(), ioe)
                            End If

                        Case START_DIRECTORY
                            result = visitor.preVisitDirectory(ev.file(), ev.attributes())

                            ' if SKIP_SIBLINGS and SKIP_SUBTREE is returned then
                            ' there shouldn't be any more events for the current
                            ' directory.
                            If result = FileVisitResult.SKIP_SUBTREE OrElse result = FileVisitResult.SKIP_SIBLINGS Then walker.pop()

                        Case END_DIRECTORY
                            result = visitor.postVisitDirectory(ev.file(), ev.ioeException())

                            ' SKIP_SIBLINGS is a no-op for postVisitDirectory
                            If result = FileVisitResult.SKIP_SIBLINGS Then result = FileVisitResult.CONTINUE

                        Case Else
                            Throw New AssertionError("Should not get here")
                    End Select

                    If java.util.Objects.requireNonNull(result) <> FileVisitResult.CONTINUE Then
                        If result = FileVisitResult.TERMINATE Then
                            Exit Do
                        ElseIf result = FileVisitResult.SKIP_SIBLINGS Then
                            walker.skipRemainingSiblings()
                        End If
                    End If
                    ev = walker.next()
                Loop While ev IsNot Nothing
            End Using

            Return start
        End Function

        ''' <summary>
        ''' Walks a file tree.
        ''' 
        ''' <p> This method works as if invoking it were equivalent to evaluating the
        ''' expression:
        ''' <blockquote><pre>
        ''' walkFileTree(start, EnumSet.noneOf(FileVisitOption.class),  [Integer].MAX_VALUE, visitor)
        ''' </pre></blockquote>
        ''' In other words, it does not follow symbolic links, and visits all levels
        ''' of the file tree.
        ''' </summary>
        ''' <param name="start">
        '''          the starting file </param>
        ''' <param name="visitor">
        '''          the file visitor to invoke for each file
        ''' </param>
        ''' <returns>  the starting file
        ''' </returns>
        ''' <exception cref="SecurityException">
        '''          If the security manager denies access to the starting file.
        '''          In the case of the default provider, the {@link
        '''          SecurityManager#checkRead(String) checkRead} method is invoked
        '''          to check read access to the directory. </exception>
        ''' <exception cref="IOException">
        '''          if an I/O error is thrown by a visitor method </exception>
        'JAVA TO VB CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
        Public Shared Function walkFileTree(Of T1)(ByVal start As Path, ByVal visitor As FileVisitor(Of T1)) As Path
            Return walkFileTree(start, java.util.EnumSet.noneOf(GetType(FileVisitOption)), Integer.MaxValue, visitor)
        End Function


        ' -- Utility methods for simple usages --

        ' buffer size used for reading and writing
        Private Const BUFFER_SIZE As Integer = 8192

        ''' <summary>
        ''' Opens a file for reading, returning a {@code BufferedReader} that may be
        ''' used to read text from the file in an efficient manner. Bytes from the
        ''' file are decoded into characters using the specified charset. Reading
        ''' commences at the beginning of the file.
        ''' 
        ''' <p> The {@code Reader} methods that read from the file throw {@code
        ''' IOException} if a malformed or unmappable byte sequence is read.
        ''' </summary>
        ''' <param name="path">
        '''          the path to the file </param>
        ''' <param name="cs">
        '''          the charset to use for decoding
        ''' </param>
        ''' <returns>  a new buffered reader, with default buffer size, to read text
        '''          from the file
        ''' </returns>
        ''' <exception cref="IOException">
        '''          if an I/O error occurs opening the file </exception>
        ''' <exception cref="SecurityException">
        '''          In the case of the default provider, and a security manager is
        '''          installed, the <seealso cref="SecurityManager#checkRead(String) checkRead"/>
        '''          method is invoked to check read access to the file.
        ''' </exception>
        ''' <seealso cref= #readAllLines </seealso>
        Public Shared Function newBufferedReader(ByVal path As Path, ByVal cs As java.nio.charset.Charset) As java.io.BufferedReader
            Dim decoder As java.nio.charset.CharsetDecoder = cs.newDecoder()
            Dim reader As java.io.Reader = New java.io.InputStreamReader(newInputStream(path), decoder)
            Return New java.io.BufferedReader(reader)
        End Function

        ''' <summary>
        ''' Opens a file for reading, returning a {@code BufferedReader} to read text
        ''' from the file in an efficient manner. Bytes from the file are decoded into
        ''' characters using the <seealso cref="StandardCharsets#UTF_8 UTF-8"/> {@link Charset
        ''' charset}.
        ''' 
        ''' <p> This method works as if invoking it were equivalent to evaluating the
        ''' expression:
        ''' <pre>{@code
        ''' Files.newBufferedReader(path, StandardCharsets.UTF_8)
        ''' }</pre>
        ''' </summary>
        ''' <param name="path">
        '''          the path to the file
        ''' </param>
        ''' <returns>  a new buffered reader, with default buffer size, to read text
        '''          from the file
        ''' </returns>
        ''' <exception cref="IOException">
        '''          if an I/O error occurs opening the file </exception>
        ''' <exception cref="SecurityException">
        '''          In the case of the default provider, and a security manager is
        '''          installed, the <seealso cref="SecurityManager#checkRead(String) checkRead"/>
        '''          method is invoked to check read access to the file.
        ''' 
        ''' @since 1.8 </exception>
        Public Shared Function newBufferedReader(ByVal path As Path) As java.io.BufferedReader
            Return newBufferedReader(path, java.nio.charset.StandardCharsets.UTF_8)
        End Function

        ''' <summary>
        ''' Opens or creates a file for writing, returning a {@code BufferedWriter}
        ''' that may be used to write text to the file in an efficient manner.
        ''' The {@code options} parameter specifies how the the file is created or
        ''' opened. If no options are present then this method works as if the {@link
        ''' StandardOpenOption#CREATE CREATE}, {@link
        ''' StandardOpenOption#TRUNCATE_EXISTING TRUNCATE_EXISTING}, and {@link
        ''' StandardOpenOption#WRITE WRITE} options are present. In other words, it
        ''' opens the file for writing, creating the file if it doesn't exist, or
        ''' initially truncating an existing <seealso cref="#isRegularFile regular-file"/> to
        ''' a size of {@code 0} if it exists.
        ''' 
        ''' <p> The {@code Writer} methods to write text throw {@code IOException}
        ''' if the text cannot be encoded using the specified charset.
        ''' </summary>
        ''' <param name="path">
        '''          the path to the file </param>
        ''' <param name="cs">
        '''          the charset to use for encoding </param>
        ''' <param name="options">
        '''          options specifying how the file is opened
        ''' </param>
        ''' <returns>  a new buffered writer, with default buffer size, to write text
        '''          to the file
        ''' </returns>
        ''' <exception cref="IOException">
        '''          if an I/O error occurs opening or creating the file </exception>
        ''' <exception cref="UnsupportedOperationException">
        '''          if an unsupported option is specified </exception>
        ''' <exception cref="SecurityException">
        '''          In the case of the default provider, and a security manager is
        '''          installed, the <seealso cref="SecurityManager#checkWrite(String) checkWrite"/>
        '''          method is invoked to check write access to the file.
        ''' </exception>
        ''' <seealso cref= #write(Path,Iterable,Charset,OpenOption[]) </seealso>
        Public Shared Function newBufferedWriter(ByVal path As Path, ByVal cs As java.nio.charset.Charset, ParamArray ByVal options As OpenOption()) As java.io.BufferedWriter
            Dim encoder As java.nio.charset.CharsetEncoder = cs.newEncoder()
            Dim writer As java.io.Writer = New java.io.OutputStreamWriter(newOutputStream(path, options), encoder)
            Return New java.io.BufferedWriter(writer)
        End Function

        ''' <summary>
        ''' Opens or creates a file for writing, returning a {@code BufferedWriter}
        ''' to write text to the file in an efficient manner. The text is encoded
        ''' into bytes for writing using the <seealso cref="StandardCharsets#UTF_8 UTF-8"/>
        ''' <seealso cref="Charset charset"/>.
        ''' 
        ''' <p> This method works as if invoking it were equivalent to evaluating the
        ''' expression:
        ''' <pre>{@code
        ''' Files.newBufferedWriter(path, StandardCharsets.UTF_8, options)
        ''' }</pre>
        ''' </summary>
        ''' <param name="path">
        '''          the path to the file </param>
        ''' <param name="options">
        '''          options specifying how the file is opened
        ''' </param>
        ''' <returns>  a new buffered writer, with default buffer size, to write text
        '''          to the file
        ''' </returns>
        ''' <exception cref="IOException">
        '''          if an I/O error occurs opening or creating the file </exception>
        ''' <exception cref="UnsupportedOperationException">
        '''          if an unsupported option is specified </exception>
        ''' <exception cref="SecurityException">
        '''          In the case of the default provider, and a security manager is
        '''          installed, the <seealso cref="SecurityManager#checkWrite(String) checkWrite"/>
        '''          method is invoked to check write access to the file.
        ''' 
        ''' @since 1.8 </exception>
        Public Shared Function newBufferedWriter(ByVal path As Path, ParamArray ByVal options As OpenOption()) As java.io.BufferedWriter
            Return newBufferedWriter(path, java.nio.charset.StandardCharsets.UTF_8, options)
        End Function

        ''' <summary>
        ''' Reads all bytes from an input stream and writes them to an output stream.
        ''' </summary>
        Private Shared Function copy(ByVal source As java.io.InputStream, ByVal sink As java.io.OutputStream) As Long
            Dim nread As Long = 0L
            Dim buf As SByte() = New SByte(BUFFER_SIZE - 1) {}
            Dim n As Integer
            n = source.read(buf)
            Do While n > 0
                sink.write(buf, 0, n)
                nread += n
                n = source.read(buf)
            Loop
            Return nread
        End Function

        ''' <summary>
        ''' Copies all bytes from an input stream to a file. On return, the input
        ''' stream will be at end of stream.
        ''' 
        ''' <p> By default, the copy fails if the target file already exists or is a
        ''' symbolic link. If the {@link StandardCopyOption#REPLACE_EXISTING
        ''' REPLACE_EXISTING} option is specified, and the target file already exists,
        ''' then it is replaced if it is not a non-empty directory. If the target
        ''' file exists and is a symbolic link, then the symbolic link is replaced.
        ''' In this release, the {@code REPLACE_EXISTING} option is the only option
        ''' required to be supported by this method. Additional options may be
        ''' supported in future releases.
        ''' 
        ''' <p>  If an I/O error occurs reading from the input stream or writing to
        ''' the file, then it may do so after the target file has been created and
        ''' after some bytes have been read or written. Consequently the input
        ''' stream may not be at end of stream and may be in an inconsistent state.
        ''' It is strongly recommended that the input stream be promptly closed if an
        ''' I/O error occurs.
        ''' 
        ''' <p> This method may block indefinitely reading from the input stream (or
        ''' writing to the file). The behavior for the case that the input stream is
        ''' <i>asynchronously closed</i> or the thread interrupted during the copy is
        ''' highly input stream and file system provider specific and therefore not
        ''' specified.
        ''' 
        ''' <p> <b>Usage example</b>: Suppose we want to capture a web page and save
        ''' it to a file:
        ''' <pre>
        '''     Path path = ...
        '''     URI u = URI.create("http://java.sun.com/");
        '''     try (InputStream in = u.toURL().openStream()) {
        '''         Files.copy(in, path);
        '''     }
        ''' </pre>
        ''' </summary>
        ''' <param name="in">
        '''          the input stream to read from </param>
        ''' <param name="target">
        '''          the path to the file </param>
        ''' <param name="options">
        '''          options specifying how the copy should be done
        ''' </param>
        ''' <returns>  the number of bytes read or written
        ''' </returns>
        ''' <exception cref="IOException">
        '''          if an I/O error occurs when reading or writing </exception>
        ''' <exception cref="FileAlreadyExistsException">
        '''          if the target file exists but cannot be replaced because the
        '''          {@code REPLACE_EXISTING} option is not specified <i>(optional
        '''          specific exception)</i> </exception>
        ''' <exception cref="DirectoryNotEmptyException">
        '''          the {@code REPLACE_EXISTING} option is specified but the file
        '''          cannot be replaced because it is a non-empty directory
        '''          <i>(optional specific exception)</i>     * </exception>
        ''' <exception cref="UnsupportedOperationException">
        '''          if {@code options} contains a copy option that is not supported </exception>
        ''' <exception cref="SecurityException">
        '''          In the case of the default provider, and a security manager is
        '''          installed, the <seealso cref="SecurityManager#checkWrite(String) checkWrite"/>
        '''          method is invoked to check write access to the file. Where the
        '''          {@code REPLACE_EXISTING} option is specified, the security
        '''          manager's <seealso cref="SecurityManager#checkDelete(String) checkDelete"/>
        '''          method is invoked to check that an existing file can be deleted. </exception>
        Public Shared Function copy(ByVal [in] As java.io.InputStream, ByVal target As Path, ParamArray ByVal options As CopyOption()) As Long
            ' ensure not null before opening file
            java.util.Objects.requireNonNull([in])

            ' check for REPLACE_EXISTING
            Dim replaceExisting As Boolean = False
            For Each opt As CopyOption In options
                If opt = StandardCopyOption.REPLACE_EXISTING Then
                    replaceExisting = True
                Else
                    If opt Is Nothing Then
                        Throw New NullPointerException("options contains 'null'")
                    Else
                        Throw New UnsupportedOperationException(opt & " not supported")
                    End If
                End If
            Next opt

            ' attempt to delete an existing file
            Dim se As SecurityException = Nothing
            If replaceExisting Then
                Try
                    deleteIfExists(target)
                Catch x As SecurityException
                    se = x
                End Try
            End If

            ' attempt to create target file. If it fails with
            ' FileAlreadyExistsException then it may be because the security
            ' manager prevented us from deleting the file, in which case we just
            ' throw the SecurityException.
            Dim ostream As java.io.OutputStream
            Try
                ostream = newOutputStream(target, StandardOpenOption.CREATE_NEW, StandardOpenOption.WRITE)
            Catch x As FileAlreadyExistsException
                If se IsNot Nothing Then Throw se
                ' someone else won the race and created the file
                Throw x
            End Try

            ' do the copy
            Using out As java.io.OutputStream = ostream
                Return copy([in], out)
            End Using
        End Function

        ''' <summary>
        ''' Copies all bytes from a file to an output stream.
        ''' 
        ''' <p> If an I/O error occurs reading from the file or writing to the output
        ''' stream, then it may do so after some bytes have been read or written.
        ''' Consequently the output stream may be in an inconsistent state. It is
        ''' strongly recommended that the output stream be promptly closed if an I/O
        ''' error occurs.
        ''' 
        ''' <p> This method may block indefinitely writing to the output stream (or
        ''' reading from the file). The behavior for the case that the output stream
        ''' is <i>asynchronously closed</i> or the thread interrupted during the copy
        ''' is highly output stream and file system provider specific and therefore
        ''' not specified.
        ''' 
        ''' <p> Note that if the given output stream is <seealso cref="java.io.Flushable"/>
        ''' then its <seealso cref="java.io.Flushable#flush flush"/> method may need to invoked
        ''' after this method completes so as to flush any buffered output.
        ''' </summary>
        ''' <param name="source">
        '''          the  path to the file </param>
        ''' <param name="out">
        '''          the output stream to write to
        ''' </param>
        ''' <returns>  the number of bytes read or written
        ''' </returns>
        ''' <exception cref="IOException">
        '''          if an I/O error occurs when reading or writing </exception>
        ''' <exception cref="SecurityException">
        '''          In the case of the default provider, and a security manager is
        '''          installed, the <seealso cref="SecurityManager#checkRead(String) checkRead"/>
        '''          method is invoked to check read access to the file. </exception>
        Public Shared Function copy(ByVal source As Path, ByVal out As java.io.OutputStream) As Long
            ' ensure not null before opening file
            java.util.Objects.requireNonNull(out)

            Using [in] As java.io.InputStream = newInputStream(source)
                Return copy([in], out)
            End Using
        End Function

        ''' <summary>
        ''' The maximum size of array to allocate.
        ''' Some VMs reserve some header words in an array.
        ''' Attempts to allocate larger arrays may result in
        ''' OutOfMemoryError: Requested array size exceeds VM limit
        ''' </summary>
        Private Shared ReadOnly MAX_BUFFER_SIZE As Integer = [Integer].MAX_VALUE - 8

        ''' <summary>
        ''' Reads all the bytes from an input stream. Uses {@code initialSize} as a hint
        ''' about how many bytes the stream will have.
        ''' </summary>
        ''' <param name="source">
        '''          the input stream to read from </param>
        ''' <param name="initialSize">
        '''          the initial size of the byte array to allocate
        ''' </param>
        ''' <returns>  a byte array containing the bytes read from the file
        ''' </returns>
        ''' <exception cref="IOException">
        '''          if an I/O error occurs reading from the stream </exception>
        ''' <exception cref="OutOfMemoryError">
        '''          if an array of the required size cannot be allocated </exception>
        Private Shared Function read(ByVal source As java.io.InputStream, ByVal initialSize As Integer) As SByte()
            Dim capacity As Integer = initialSize
            Dim buf As SByte() = New SByte(capacity - 1) {}
            Dim nread As Integer = 0
            Dim n As Integer
            Do
                ' read to EOF which may read more or less than initialSize (eg: file
                ' is truncated while we are reading)
                n = source.read(buf, nread, capacity - nread)
                Do While n > 0
                    nread += n
                    n = source.read(buf, nread, capacity - nread)
                Loop

                ' if last call to source.read() returned -1, we are done
                ' otherwise, try to read one more byte; if that failed we're done too
                n = source.read()
                If n < 0 OrElse n < 0 Then Exit Do

                ' one more byte was read; need to allocate a larger buffer
                If capacity <= MAX_BUFFER_SIZE - capacity Then
                    capacity = math.Max(capacity << 1, BUFFER_SIZE)
                Else
                    If capacity = MAX_BUFFER_SIZE Then Throw New OutOfMemoryError("Required array size too large")
                    capacity = MAX_BUFFER_SIZE
                End If
                buf = java.util.Arrays.copyOf(buf, capacity)
                buf(nread) = CByte(n)
                nread += 1
            Loop
            Return If(capacity = nread, buf, java.util.Arrays.copyOf(buf, nread))
        End Function

        ''' <summary>
        ''' Reads all the bytes from a file. The method ensures that the file is
        ''' closed when all bytes have been read or an I/O error, or other runtime
        ''' exception, is thrown.
        ''' 
        ''' <p> Note that this method is intended for simple cases where it is
        ''' convenient to read all bytes into a byte array. It is not intended for
        ''' reading in large files.
        ''' </summary>
        ''' <param name="path">
        '''          the path to the file
        ''' </param>
        ''' <returns>  a byte array containing the bytes read from the file
        ''' </returns>
        ''' <exception cref="IOException">
        '''          if an I/O error occurs reading from the stream </exception>
        ''' <exception cref="OutOfMemoryError">
        '''          if an array of the required size cannot be allocated, for
        '''          example the file is larger that {@code 2GB} </exception>
        ''' <exception cref="SecurityException">
        '''          In the case of the default provider, and a security manager is
        '''          installed, the <seealso cref="SecurityManager#checkRead(String) checkRead"/>
        '''          method is invoked to check read access to the file. </exception>
        Public Shared Function readAllBytes(ByVal path As Path) As SByte()
            Using sbc As java.nio.channels.SeekableByteChannel = Files.newByteChannel(path), [in] As java.io.InputStream = java.nio.channels.Channels.newInputStream(sbc)
                Dim size As Long = sbc.size()
                If size > (Long)MAX_BUFFER_SIZE Then Throw New OutOfMemoryError("Required array size too large")

				Return read([in], CInt(size))
            End Using
        End Function

        ''' <summary>
        ''' Read all lines from a file. This method ensures that the file is
        ''' closed when all bytes have been read or an I/O error, or other runtime
        ''' exception, is thrown. Bytes from the file are decoded into characters
        ''' using the specified charset.
        ''' 
        ''' <p> This method recognizes the following as line terminators:
        ''' <ul>
        '''   <li> <code>&#92;u000D</code> followed by <code>&#92;u000A</code>,
        '''     CARRIAGE RETURN followed by LINE FEED </li>
        '''   <li> <code>&#92;u000A</code>, LINE FEED </li>
        '''   <li> <code>&#92;u000D</code>, CARRIAGE RETURN </li>
        ''' </ul>
        ''' <p> Additional Unicode line terminators may be recognized in future
        ''' releases.
        ''' 
        ''' <p> Note that this method is intended for simple cases where it is
        ''' convenient to read all lines in a single operation. It is not intended
        ''' for reading in large files.
        ''' </summary>
        ''' <param name="path">
        '''          the path to the file </param>
        ''' <param name="cs">
        '''          the charset to use for decoding
        ''' </param>
        ''' <returns>  the lines from the file as a {@code List}; whether the {@code
        '''          List} is modifiable or not is implementation dependent and
        '''          therefore not specified
        ''' </returns>
        ''' <exception cref="IOException">
        '''          if an I/O error occurs reading from the file or a malformed or
        '''          unmappable byte sequence is read </exception>
        ''' <exception cref="SecurityException">
        '''          In the case of the default provider, and a security manager is
        '''          installed, the <seealso cref="SecurityManager#checkRead(String) checkRead"/>
        '''          method is invoked to check read access to the file.
        ''' </exception>
        ''' <seealso cref= #newBufferedReader </seealso>
        Public Shared Function readAllLines(ByVal path As Path, ByVal cs As java.nio.charset.Charset) As IList(Of String)
            Using reader As java.io.BufferedReader = newBufferedReader(path, cs)
                Dim result As IList(Of String) = New List(Of String)
                Do
                    Dim line As String = reader.readLine()
                    If line Is Nothing Then Exit Do
                    result.Add(line)
                Loop
                Return result
            End Using
        End Function

        ''' <summary>
        ''' Read all lines from a file. Bytes from the file are decoded into characters
        ''' using the <seealso cref="StandardCharsets#UTF_8 UTF-8"/> <seealso cref="Charset charset"/>.
        ''' 
        ''' <p> This method works as if invoking it were equivalent to evaluating the
        ''' expression:
        ''' <pre>{@code
        ''' Files.readAllLines(path, StandardCharsets.UTF_8)
        ''' }</pre>
        ''' </summary>
        ''' <param name="path">
        '''          the path to the file
        ''' </param>
        ''' <returns>  the lines from the file as a {@code List}; whether the {@code
        '''          List} is modifiable or not is implementation dependent and
        '''          therefore not specified
        ''' </returns>
        ''' <exception cref="IOException">
        '''          if an I/O error occurs reading from the file or a malformed or
        '''          unmappable byte sequence is read </exception>
        ''' <exception cref="SecurityException">
        '''          In the case of the default provider, and a security manager is
        '''          installed, the <seealso cref="SecurityManager#checkRead(String) checkRead"/>
        '''          method is invoked to check read access to the file.
        ''' 
        ''' @since 1.8 </exception>
        Public Shared Function readAllLines(ByVal path As Path) As IList(Of String)
            Return readAllLines(path, java.nio.charset.StandardCharsets.UTF_8)
        End Function

        ''' <summary>
        ''' Writes bytes to a file. The {@code options} parameter specifies how the
        ''' the file is created or opened. If no options are present then this method
        ''' works as if the <seealso cref="StandardOpenOption#CREATE CREATE"/>, {@link
        ''' StandardOpenOption#TRUNCATE_EXISTING TRUNCATE_EXISTING}, and {@link
        ''' StandardOpenOption#WRITE WRITE} options are present. In other words, it
        ''' opens the file for writing, creating the file if it doesn't exist, or
        ''' initially truncating an existing <seealso cref="#isRegularFile regular-file"/> to
        ''' a size of {@code 0}. All bytes in the byte array are written to the file.
        ''' The method ensures that the file is closed when all bytes have been
        ''' written (or an I/O error or other runtime exception is thrown). If an I/O
        ''' error occurs then it may do so after the file has created or truncated,
        ''' or after some bytes have been written to the file.
        ''' 
        ''' <p> <b>Usage example</b>: By default the method creates a new file or
        ''' overwrites an existing file. Suppose you instead want to append bytes
        ''' to an existing file:
        ''' <pre>
        '''     Path path = ...
        '''     byte[] bytes = ...
        '''     Files.write(path, bytes, StandardOpenOption.APPEND);
        ''' </pre>
        ''' </summary>
        ''' <param name="path">
        '''          the path to the file </param>
        ''' <param name="bytes">
        '''          the byte array with the bytes to write </param>
        ''' <param name="options">
        '''          options specifying how the file is opened
        ''' </param>
        ''' <returns>  the path
        ''' </returns>
        ''' <exception cref="IOException">
        '''          if an I/O error occurs writing to or creating the file </exception>
        ''' <exception cref="UnsupportedOperationException">
        '''          if an unsupported option is specified </exception>
        ''' <exception cref="SecurityException">
        '''          In the case of the default provider, and a security manager is
        '''          installed, the <seealso cref="SecurityManager#checkWrite(String) checkWrite"/>
        '''          method is invoked to check write access to the file. </exception>
        Public Shared Function write(ByVal path As Path, ByVal bytes As SByte(), ParamArray ByVal options As OpenOption()) As Path
            ' ensure bytes is not null before opening file
            java.util.Objects.requireNonNull(bytes)

            Using out As java.io.OutputStream = Files.newOutputStream(path, options)
                Dim len As Integer = bytes.Length
                Dim [rem] As Integer = len
                Do While [rem] > 0
                    Dim n As Integer = math.Min([rem], BUFFER_SIZE)
                    out.write(bytes, (len - [rem]), n)
                    [rem] -= n
                Loop
            End Using
            Return path
        End Function

        ''' <summary>
        ''' Write lines of text to a file. Each line is a char sequence and is
        ''' written to the file in sequence with each line terminated by the
        ''' platform's line separator, as defined by the system property {@code
        ''' line.separator}. Characters are encoded into bytes using the specified
        ''' charset.
        ''' 
        ''' <p> The {@code options} parameter specifies how the the file is created
        ''' or opened. If no options are present then this method works as if the
        ''' <seealso cref="StandardOpenOption#CREATE CREATE"/>, {@link
        ''' StandardOpenOption#TRUNCATE_EXISTING TRUNCATE_EXISTING}, and {@link
        ''' StandardOpenOption#WRITE WRITE} options are present. In other words, it
        ''' opens the file for writing, creating the file if it doesn't exist, or
        ''' initially truncating an existing <seealso cref="#isRegularFile regular-file"/> to
        ''' a size of {@code 0}. The method ensures that the file is closed when all
        ''' lines have been written (or an I/O error or other runtime exception is
        ''' thrown). If an I/O error occurs then it may do so after the file has
        ''' created or truncated, or after some bytes have been written to the file.
        ''' </summary>
        ''' <param name="path">
        '''          the path to the file </param>
        ''' <param name="lines">
        '''          an object to iterate over the char sequences </param>
        ''' <param name="cs">
        '''          the charset to use for encoding </param>
        ''' <param name="options">
        '''          options specifying how the file is opened
        ''' </param>
        ''' <returns>  the path
        ''' </returns>
        ''' <exception cref="IOException">
        '''          if an I/O error occurs writing to or creating the file, or the
        '''          text cannot be encoded using the specified charset </exception>
        ''' <exception cref="UnsupportedOperationException">
        '''          if an unsupported option is specified </exception>
        ''' <exception cref="SecurityException">
        '''          In the case of the default provider, and a security manager is
        '''          installed, the <seealso cref="SecurityManager#checkWrite(String) checkWrite"/>
        '''          method is invoked to check write access to the file. </exception>
        Public Shared Function write(Of T1 As CharSequence)(ByVal path As Path, ByVal lines As Iterable(Of T1), ByVal cs As java.nio.charset.Charset, ParamArray ByVal options As OpenOption()) As Path
            ' ensure lines is not null before opening file
            java.util.Objects.requireNonNull(lines)
            Dim encoder As java.nio.charset.CharsetEncoder = cs.newEncoder()
            Dim out As java.io.OutputStream = newOutputStream(path, options)
            Using writer As New java.io.BufferedWriter(New java.io.OutputStreamWriter(out, encoder))
                For Each line As CharSequence In lines
                    writer.append(line)
                    writer.newLine()
                Next line
            End Using
            Return path
        End Function

        ''' <summary>
        ''' Write lines of text to a file. Characters are encoded into bytes using
        ''' the <seealso cref="StandardCharsets#UTF_8 UTF-8"/> <seealso cref="Charset charset"/>.
        ''' 
        ''' <p> This method works as if invoking it were equivalent to evaluating the
        ''' expression:
        ''' <pre>{@code
        ''' Files.write(path, lines, StandardCharsets.UTF_8, options);
        ''' }</pre>
        ''' </summary>
        ''' <param name="path">
        '''          the path to the file </param>
        ''' <param name="lines">
        '''          an object to iterate over the char sequences </param>
        ''' <param name="options">
        '''          options specifying how the file is opened
        ''' </param>
        ''' <returns>  the path
        ''' </returns>
        ''' <exception cref="IOException">
        '''          if an I/O error occurs writing to or creating the file, or the
        '''          text cannot be encoded as {@code UTF-8} </exception>
        ''' <exception cref="UnsupportedOperationException">
        '''          if an unsupported option is specified </exception>
        ''' <exception cref="SecurityException">
        '''          In the case of the default provider, and a security manager is
        '''          installed, the <seealso cref="SecurityManager#checkWrite(String) checkWrite"/>
        '''          method is invoked to check write access to the file.
        ''' 
        ''' @since 1.8 </exception>
        Public Shared Function write(Of T1 As CharSequence)(ByVal path As Path, ByVal lines As Iterable(Of T1), ParamArray ByVal options As OpenOption()) As Path
            Return write(path, lines, java.nio.charset.StandardCharsets.UTF_8, options)
        End Function

        ' -- Stream APIs --

        ''' <summary>
        ''' Return a lazily populated {@code Stream}, the elements of
        ''' which are the entries in the directory.  The listing is not recursive.
        ''' 
        ''' <p> The elements of the stream are <seealso cref="Path"/> objects that are
        ''' obtained as if by <seealso cref="Path#resolve(Path) resolving"/> the name of the
        ''' directory entry against {@code dir}. Some file systems maintain special
        ''' links to the directory itself and the directory's parent directory.
        ''' Entries representing these links are not included.
        ''' 
        ''' <p> The stream is <i>weakly consistent</i>. It is thread safe but does
        ''' not freeze the directory while iterating, so it may (or may not)
        ''' reflect updates to the directory that occur after returning from this
        ''' method.
        ''' 
        ''' <p> The returned stream encapsulates a <seealso cref="DirectoryStream"/>.
        ''' If timely disposal of file system resources is required, the
        ''' {@code try}-with-resources construct should be used to ensure that the
        ''' stream's <seealso cref="Stream#close close"/> method is invoked after the stream
        ''' operations are completed.
        ''' 
        ''' <p> Operating on a closed stream behaves as if the end of stream
        ''' has been reached. Due to read-ahead, one or more elements may be
        ''' returned after the stream has been closed.
        ''' 
        ''' <p> If an <seealso cref="IOException"/> is thrown when accessing the directory
        ''' after this method has returned, it is wrapped in an {@link
        ''' UncheckedIOException} which will be thrown from the method that caused
        ''' the access to take place.
        ''' </summary>
        ''' <param name="dir">  The path to the directory
        ''' </param>
        ''' <returns>  The {@code Stream} describing the content of the
        '''          directory
        ''' </returns>
        ''' <exception cref="NotDirectoryException">
        '''          if the file could not otherwise be opened because it is not
        '''          a directory <i>(optional specific exception)</i> </exception>
        ''' <exception cref="IOException">
        '''          if an I/O error occurs when opening the directory </exception>
        ''' <exception cref="SecurityException">
        '''          In the case of the default provider, and a security manager is
        '''          installed, the <seealso cref="SecurityManager#checkRead(String) checkRead"/>
        '''          method is invoked to check read access to the directory.
        ''' </exception>
        ''' <seealso cref=     #newDirectoryStream(Path)
        ''' @since   1.8 </seealso>
        Public Shared Function list(ByVal dir As Path) As java.util.stream.Stream(Of Path)
            Dim ds As DirectoryStream(Of Path) = Files.newDirectoryStream(dir)
            Try
                Dim [delegate] As IEnumerator(Of Path) = ds.GetEnumerator()

                ' Re-wrap DirectoryIteratorException to UncheckedIOException
                Dim it As IEnumerator(Of Path) = New IteratorAnonymousInnerClassHelper(Of E)

                Return java.util.stream.StreamSupport.stream(java.util.Spliterators.spliteratorUnknownSize(it, java.util.Spliterator.DISTINCT), False).onClose(asUncheckedRunnable(ds))
                'JAVA TO VB CONVERTER TODO TASK: There is no equivalent in VB to Java 'multi-catch' syntax:
            Catch Error Or RuntimeException e
				Try
                    ds.close()
                Catch ex As java.io.IOException
                    Try
                        e.addSuppressed(ex)
                    Catch ignore As Throwable
                    End Try
                End Try
                Throw e
            End Try
        End Function

        Private Class IteratorAnonymousInnerClassHelper(Of E)
            Implements IEnumerator(Of E)

            Public Overrides Function hasNext() As Boolean
                Try
                    Return delegate.hasNext()
				Catch e As DirectoryIteratorException
                    Throw New java.io.UncheckedIOException(e.InnerException)
                End Try
            End Function
            Public Overrides Function [next]() As Path
                Try
                    Return delegate.next()
				Catch e As DirectoryIteratorException
                    Throw New java.io.UncheckedIOException(e.InnerException)
                End Try
            End Function
        End Class

        ''' <summary>
        ''' Return a {@code Stream} that is lazily populated with {@code
        ''' Path} by walking the file tree rooted at a given starting file.  The
        ''' file tree is traversed <em>depth-first</em>, the elements in the stream
        ''' are <seealso cref="Path"/> objects that are obtained as if by {@link
        ''' Path#resolve(Path) resolving} the relative path against {@code start}.
        ''' 
        ''' <p> The {@code stream} walks the file tree as elements are consumed.
        ''' The {@code Stream} returned is guaranteed to have at least one
        ''' element, the starting file itself. For each file visited, the stream
        ''' attempts to read its <seealso cref="BasicFileAttributes"/>. If the file is a
        ''' directory and can be opened successfully, entries in the directory, and
        ''' their <em>descendants</em> will follow the directory in the stream as
        ''' they are encountered. When all entries have been visited, then the
        ''' directory is closed. The file tree walk then continues at the next
        ''' <em>sibling</em> of the directory.
        ''' 
        ''' <p> The stream is <i>weakly consistent</i>. It does not freeze the
        ''' file tree while iterating, so it may (or may not) reflect updates to
        ''' the file tree that occur after returned from this method.
        ''' 
        ''' <p> By default, symbolic links are not automatically followed by this
        ''' method. If the {@code options} parameter contains the {@link
        ''' FileVisitOption#FOLLOW_LINKS FOLLOW_LINKS} option then symbolic links are
        ''' followed. When following links, and the attributes of the target cannot
        ''' be read, then this method attempts to get the {@code BasicFileAttributes}
        ''' of the link.
        ''' 
        ''' <p> If the {@code options} parameter contains the {@link
        ''' FileVisitOption#FOLLOW_LINKS FOLLOW_LINKS} option then the stream keeps
        ''' track of directories visited so that cycles can be detected. A cycle
        ''' arises when there is an entry in a directory that is an ancestor of the
        ''' directory. Cycle detection is done by recording the {@link
        ''' java.nio.file.attribute.BasicFileAttributes#fileKey file-key} of directories,
        ''' or if file keys are not available, by invoking the {@link #isSameFile
        ''' isSameFile} method to test if a directory is the same file as an
        ''' ancestor. When a cycle is detected it is treated as an I/O error with
        ''' an instance of <seealso cref="FileSystemLoopException"/>.
        ''' 
        ''' <p> The {@code maxDepth} parameter is the maximum number of levels of
        ''' directories to visit. A value of {@code 0} means that only the starting
        ''' file is visited, unless denied by the security manager. A value of
        ''' <seealso cref="Integer#MAX_VALUE MAX_VALUE"/> may be used to indicate that all
        ''' levels should be visited.
        ''' 
        ''' <p> When a security manager is installed and it denies access to a file
        ''' (or directory), then it is ignored and not included in the stream.
        ''' 
        ''' <p> The returned stream encapsulates one or more <seealso cref="DirectoryStream"/>s.
        ''' If timely disposal of file system resources is required, the
        ''' {@code try}-with-resources construct should be used to ensure that the
        ''' stream's <seealso cref="Stream#close close"/> method is invoked after the stream
        ''' operations are completed.  Operating on a closed stream will result in an
        ''' <seealso cref="java.lang.IllegalStateException"/>.
        ''' 
        ''' <p> If an <seealso cref="IOException"/> is thrown when accessing the directory
        ''' after this method has returned, it is wrapped in an {@link
        ''' UncheckedIOException} which will be thrown from the method that caused
        ''' the access to take place.
        ''' </summary>
        ''' <param name="start">
        '''          the starting file </param>
        ''' <param name="maxDepth">
        '''          the maximum number of directory levels to visit </param>
        ''' <param name="options">
        '''          options to configure the traversal
        ''' </param>
        ''' <returns>  the <seealso cref="Stream"/> of <seealso cref="Path"/>
        ''' </returns>
        ''' <exception cref="IllegalArgumentException">
        '''          if the {@code maxDepth} parameter is negative </exception>
        ''' <exception cref="SecurityException">
        '''          If the security manager denies access to the starting file.
        '''          In the case of the default provider, the {@link
        '''          SecurityManager#checkRead(String) checkRead} method is invoked
        '''          to check read access to the directory. </exception>
        ''' <exception cref="IOException">
        '''          if an I/O error is thrown when accessing the starting file.
        ''' @since   1.8 </exception>
        Public Shared Function walk(ByVal start As Path, ByVal maxDepth As Integer, ParamArray ByVal options As FileVisitOption()) As java.util.stream.Stream(Of Path)
            Dim [iterator] As New FileTreeIterator(start, maxDepth, options)
            Try
                Return java.util.stream.StreamSupport.stream(java.util.Spliterators.spliteratorUnknownSize([iterator], java.util.Spliterator.DISTINCT), False).onClose([iterator]: close).map(entry -> entry.file())
'JAVA TO VB CONVERTER TODO TASK: There is no equivalent in VB to Java 'multi-catch' syntax:
			Catch Error Or RuntimeException e
				[iterator].close()
                Throw e
            End Try
        End Function

        ''' <summary>
        ''' Return a {@code Stream} that is lazily populated with {@code
        ''' Path} by walking the file tree rooted at a given starting file.  The
        ''' file tree is traversed <em>depth-first</em>, the elements in the stream
        ''' are <seealso cref="Path"/> objects that are obtained as if by {@link
        ''' Path#resolve(Path) resolving} the relative path against {@code start}.
        ''' 
        ''' <p> This method works as if invoking it were equivalent to evaluating the
        ''' expression:
        ''' <blockquote><pre>
        ''' walk(start,  [Integer].MAX_VALUE, options)
        ''' </pre></blockquote>
        ''' In other words, it visits all levels of the file tree.
        ''' 
        ''' <p> The returned stream encapsulates one or more <seealso cref="DirectoryStream"/>s.
        ''' If timely disposal of file system resources is required, the
        ''' {@code try}-with-resources construct should be used to ensure that the
        ''' stream's <seealso cref="Stream#close close"/> method is invoked after the stream
        ''' operations are completed.  Operating on a closed stream will result in an
        ''' <seealso cref="java.lang.IllegalStateException"/>.
        ''' </summary>
        ''' <param name="start">
        '''          the starting file </param>
        ''' <param name="options">
        '''          options to configure the traversal
        ''' </param>
        ''' <returns>  the <seealso cref="Stream"/> of <seealso cref="Path"/>
        ''' </returns>
        ''' <exception cref="SecurityException">
        '''          If the security manager denies access to the starting file.
        '''          In the case of the default provider, the {@link
        '''          SecurityManager#checkRead(String) checkRead} method is invoked
        '''          to check read access to the directory. </exception>
        ''' <exception cref="IOException">
        '''          if an I/O error is thrown when accessing the starting file.
        ''' </exception>
        ''' <seealso cref=     #walk(Path, int, FileVisitOption...)
        ''' @since   1.8 </seealso>
        Public Shared Function walk(ByVal start As Path, ParamArray ByVal options As FileVisitOption()) As java.util.stream.Stream(Of Path)
            Return walk(start, Integer.MaxValue, options)
        End Function

        ''' <summary>
        ''' Return a {@code Stream} that is lazily populated with {@code
        ''' Path} by searching for files in a file tree rooted at a given starting
        ''' file.
        ''' 
        ''' <p> This method walks the file tree in exactly the manner specified by
        ''' the <seealso cref="#walk walk"/> method. For each file encountered, the given
        ''' <seealso cref="BiPredicate"/> is invoked with its <seealso cref="Path"/> and {@link
        ''' BasicFileAttributes}. The {@code Path} object is obtained as if by
        ''' <seealso cref="Path#resolve(Path) resolving"/> the relative path against {@code
        ''' start} and is only included in the returned <seealso cref="Stream"/> if
        ''' the {@code BiPredicate} returns true. Compare to calling {@link
        ''' java.util.stream.Stream#filter filter} on the {@code Stream}
        ''' returned by {@code walk} method, this method may be more efficient by
        ''' avoiding redundant retrieval of the {@code BasicFileAttributes}.
        ''' 
        ''' <p> The returned stream encapsulates one or more <seealso cref="DirectoryStream"/>s.
        ''' If timely disposal of file system resources is required, the
        ''' {@code try}-with-resources construct should be used to ensure that the
        ''' stream's <seealso cref="Stream#close close"/> method is invoked after the stream
        ''' operations are completed.  Operating on a closed stream will result in an
        ''' <seealso cref="java.lang.IllegalStateException"/>.
        ''' 
        ''' <p> If an <seealso cref="IOException"/> is thrown when accessing the directory
        ''' after returned from this method, it is wrapped in an {@link
        ''' UncheckedIOException} which will be thrown from the method that caused
        ''' the access to take place.
        ''' </summary>
        ''' <param name="start">
        '''          the starting file </param>
        ''' <param name="maxDepth">
        '''          the maximum number of directory levels to search </param>
        ''' <param name="matcher">
        '''          the function used to decide whether a file should be included
        '''          in the returned stream </param>
        ''' <param name="options">
        '''          options to configure the traversal
        ''' </param>
        ''' <returns>  the <seealso cref="Stream"/> of <seealso cref="Path"/>
        ''' </returns>
        ''' <exception cref="IllegalArgumentException">
        '''          if the {@code maxDepth} parameter is negative </exception>
        ''' <exception cref="SecurityException">
        '''          If the security manager denies access to the starting file.
        '''          In the case of the default provider, the {@link
        '''          SecurityManager#checkRead(String) checkRead} method is invoked
        '''          to check read access to the directory. </exception>
        ''' <exception cref="IOException">
        '''          if an I/O error is thrown when accessing the starting file.
        ''' </exception>
        ''' <seealso cref=     #walk(Path, int, FileVisitOption...)
        ''' @since   1.8 </seealso>
        Public Shared Function find(ByVal start As Path, ByVal maxDepth As Integer, ByVal matcher As java.util.function.BiPredicate(Of Path, java.nio.file.attribute.BasicFileAttributes), ParamArray ByVal options As FileVisitOption()) As java.util.stream.Stream(Of Path)
            Dim [iterator] As New FileTreeIterator(start, maxDepth, options)
            Try
                Return java.util.stream.StreamSupport.stream(java.util.Spliterators.spliteratorUnknownSize([iterator], java.util.Spliterator.DISTINCT), False).onClose([iterator]: close).filter(entry -> matcher.test(entry.file(), entry.attributes())).map(entry -> entry.file())
'JAVA TO VB CONVERTER TODO TASK: There is no equivalent in VB to Java 'multi-catch' syntax:
			Catch Error Or RuntimeException e
				[iterator].close()
                Throw e
            End Try
        End Function

        ''' <summary>
        ''' Read all lines from a file as a {@code Stream}. Unlike {@link
        ''' #readAllLines(Path, Charset) readAllLines}, this method does not read
        ''' all lines into a {@code List}, but instead populates lazily as the stream
        ''' is consumed.
        ''' 
        ''' <p> Bytes from the file are decoded into characters using the specified
        ''' charset and the same line terminators as specified by {@code
        ''' readAllLines} are supported.
        ''' 
        ''' <p> After this method returns, then any subsequent I/O exception that
        ''' occurs while reading from the file or when a malformed or unmappable byte
        ''' sequence is read, is wrapped in an <seealso cref="UncheckedIOException"/> that will
        ''' be thrown from the
        ''' <seealso cref="java.util.stream.Stream"/> method that caused the read to take
        ''' place. In case an {@code IOException} is thrown when closing the file,
        ''' it is also wrapped as an {@code UncheckedIOException}.
        ''' 
        ''' <p> The returned stream encapsulates a <seealso cref="Reader"/>.  If timely
        ''' disposal of file system resources is required, the try-with-resources
        ''' construct should be used to ensure that the stream's
        ''' <seealso cref="Stream#close close"/> method is invoked after the stream operations
        ''' are completed.
        ''' 
        ''' </summary>
        ''' <param name="path">
        '''          the path to the file </param>
        ''' <param name="cs">
        '''          the charset to use for decoding
        ''' </param>
        ''' <returns>  the lines from the file as a {@code Stream}
        ''' </returns>
        ''' <exception cref="IOException">
        '''          if an I/O error occurs opening the file </exception>
        ''' <exception cref="SecurityException">
        '''          In the case of the default provider, and a security manager is
        '''          installed, the <seealso cref="SecurityManager#checkRead(String) checkRead"/>
        '''          method is invoked to check read access to the file.
        ''' </exception>
        ''' <seealso cref=     #readAllLines(Path, Charset) </seealso>
        ''' <seealso cref=     #newBufferedReader(Path, Charset) </seealso>
        ''' <seealso cref=     java.io.BufferedReader#lines()
        ''' @since   1.8 </seealso>
        Public Shared Function lines(ByVal path As Path, ByVal cs As java.nio.charset.Charset) As java.util.stream.Stream(Of String)
            Dim br As java.io.BufferedReader = Files.newBufferedReader(path, cs)
            Try
                Return br.lines().onClose(asUncheckedRunnable(br))
                'JAVA TO VB CONVERTER TODO TASK: There is no equivalent in VB to Java 'multi-catch' syntax:
            Catch Error Or RuntimeException e
				Try
                    br.close()
                Catch ex As java.io.IOException
                    Try
                        e.addSuppressed(ex)
                    Catch ignore As Throwable
                    End Try
                End Try
                Throw e
            End Try
        End Function

        ''' <summary>
        ''' Read all lines from a file as a {@code Stream}. Bytes from the file are
        ''' decoded into characters using the <seealso cref="StandardCharsets#UTF_8 UTF-8"/>
        ''' <seealso cref="Charset charset"/>.
        ''' 
        ''' <p> This method works as if invoking it were equivalent to evaluating the
        ''' expression:
        ''' <pre>{@code
        ''' Files.lines(path, StandardCharsets.UTF_8)
        ''' }</pre>
        ''' </summary>
        ''' <param name="path">
        '''          the path to the file
        ''' </param>
        ''' <returns>  the lines from the file as a {@code Stream}
        ''' </returns>
        ''' <exception cref="IOException">
        '''          if an I/O error occurs opening the file </exception>
        ''' <exception cref="SecurityException">
        '''          In the case of the default provider, and a security manager is
        '''          installed, the <seealso cref="SecurityManager#checkRead(String) checkRead"/>
        '''          method is invoked to check read access to the file.
        ''' 
        ''' @since 1.8 </exception>
        Public Shared Function lines(ByVal path As Path) As java.util.stream.Stream(Of String)
            Return lines(path, java.nio.charset.StandardCharsets.UTF_8)
        End Function
    End Class

End Namespace