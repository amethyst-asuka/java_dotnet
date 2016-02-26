Imports System.Collections.Generic
import static javax.tools.JavaFileObject.Kind

'
' * Copyright (c) 2005, 2006, Oracle and/or its affiliates. All rights reserved.
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

Namespace javax.tools

	''' <summary>
	''' File manager for tools operating on Java&trade; programming language
	''' source and class files.  In this context, <em>file</em> means an
	''' abstraction of regular files and other sources of data.
	''' 
	''' <p>When constructing new JavaFileObjects, the file manager must
	''' determine where to create them.  For example, if a file manager
	''' manages regular files on a file system, it would most likely have a
	''' current/working directory to use as default location when creating
	''' or finding files.  A number of hints can be provided to a file
	''' manager as to where to create files.  Any file manager might choose
	''' to ignore these hints.
	''' 
	''' <p>Some methods in this interface use class names.  Such class
	''' names must be given in the Java Virtual Machine internal form of
	''' fully qualified class and interface names.  For convenience '.'
	''' and '/' are interchangeable.  The internal form is defined in
	''' chapter four of
	''' <cite>The Java&trade; Virtual Machine Specification</cite>.
	''' 
	''' <blockquote><p>
	'''   <i>Discussion:</i> this means that the names
	'''   "java/lang.package-info", "java/lang/package-info",
	'''   "java.lang.package-info", are valid and equivalent.  Compare to
	'''   binary name as defined in
	'''   <cite>The Java&trade; Language Specification</cite>,
	'''   section 13.1 "The Form of a Binary".
	''' </p></blockquote>
	''' 
	''' <p>The case of names is significant.  All names should be treated
	''' as case-sensitive.  For example, some file systems have
	''' case-insensitive, case-aware file names.  File objects representing
	''' such files should take care to preserve case by using {@link
	''' java.io.File#getCanonicalFile} or similar means.  If the system is
	''' not case-aware, file objects must use other means to preserve case.
	''' 
	''' <p><em><a name="relative_name">Relative names</a>:</em> some
	''' methods in this interface use relative names.  A relative name is a
	''' non-null, non-empty sequence of path segments separated by '/'.
	''' '.' or '..'  are invalid path segments.  A valid relative name must
	''' match the "path-rootless" rule of <a
	''' href="http://www.ietf.org/rfc/rfc3986.txt">RFC&nbsp;3986</a>,
	''' section&nbsp;3.3.  Informally, this should be true:
	''' 
	''' <!-- URI.create(relativeName).normalize().getPath().equals(relativeName) -->
	''' <pre>  URI.{@linkplain java.net.URI#create create}(relativeName).{@linkplain java.net.URI#normalize normalize}().{@linkplain java.net.URI#getPath getPath}().equals(relativeName)</pre>
	''' 
	''' <p>All methods in this interface might throw a SecurityException.
	''' 
	''' <p>An object of this interface is not required to support
	''' multi-threaded access, that is, be synchronized.  However, it must
	''' support concurrent access to different file objects created by this
	''' object.
	''' 
	''' <p><em>Implementation note:</em> a consequence of this requirement
	''' is that a trivial implementation of output to a {@linkplain
	''' java.util.jar.JarOutputStream} is not a sufficient implementation.
	''' That is, rather than creating a JavaFileObject that returns the
	''' JarOutputStream directly, the contents must be cached until closed
	''' and then written to the JarOutputStream.
	''' 
	''' <p>Unless explicitly allowed, all methods in this interface might
	''' throw a NullPointerException if given a {@code null} argument.
	''' 
	''' @author Peter von der Ah&eacute;
	''' @author Jonathan Gibbons </summary>
	''' <seealso cref= JavaFileObject </seealso>
	''' <seealso cref= FileObject
	''' @since 1.6 </seealso>
	Public Interface JavaFileManager
		Inherits java.io.Closeable, java.io.Flushable, OptionChecker

		''' <summary>
		''' Interface for locations of file objects.  Used by file managers
		''' to determine where to place or search for file objects.
		''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Interface inner types are not converted:
'		interface Location
	'	{
	'		''' <summary>
	'		''' Gets the name of this location.
	'		''' </summary>
	'		''' <returns> a name </returns>
	'		String getName();
	'
	'		''' <summary>
	'		''' Determines if this is an output location.  An output
	'		''' location is a location that is conventionally used for
	'		''' output.
	'		''' </summary>
	'		''' <returns> true if this is an output location, false otherwise </returns>
	'		boolean isOutputLocation();
	'	}

		''' <summary>
		''' Gets a class loader for loading plug-ins from the given
		''' location.  For example, to load annotation processors, a
		''' compiler will request a class loader for the {@link
		''' StandardLocation#ANNOTATION_PROCESSOR_PATH
		''' ANNOTATION_PROCESSOR_PATH} location.
		''' </summary>
		''' <param name="location"> a location </param>
		''' <returns> a class loader for the given location; or {@code null}
		''' if loading plug-ins from the given location is disabled or if
		''' the location is not known </returns>
		''' <exception cref="SecurityException"> if a class loader can not be created
		''' in the current security context </exception>
		''' <exception cref="IllegalStateException"> if <seealso cref="#close"/> has been called
		''' and this file manager cannot be reopened </exception>
		Function getClassLoader(ByVal location As Location) As ClassLoader

		''' <summary>
		''' Lists all file objects matching the given criteria in the given
		''' location.  List file objects in "subpackages" if recurse is
		''' true.
		''' 
		''' <p>Note: even if the given location is unknown to this file
		''' manager, it may not return {@code null}.  Also, an unknown
		''' location may not cause an exception.
		''' </summary>
		''' <param name="location">     a location </param>
		''' <param name="packageName">  a package name </param>
		''' <param name="kinds">        return objects only of these kinds </param>
		''' <param name="recurse">      if true include "subpackages" </param>
		''' <returns> an Iterable of file objects matching the given criteria </returns>
		''' <exception cref="IOException"> if an I/O error occurred, or if {@link
		''' #close} has been called and this file manager cannot be
		''' reopened </exception>
		''' <exception cref="IllegalStateException"> if <seealso cref="#close"/> has been called
		''' and this file manager cannot be reopened </exception>
		Function list(ByVal location As Location, ByVal packageName As String, ByVal kinds As java.util.Set(Of Kind), ByVal recurse As Boolean) As IEnumerable(Of JavaFileObject)

		''' <summary>
		''' Infers a binary name of a file object based on a location.  The
		''' binary name returned might not be a valid binary name according to
		''' <cite>The Java&trade; Language Specification</cite>.
		''' </summary>
		''' <param name="location"> a location </param>
		''' <param name="file"> a file object </param>
		''' <returns> a binary name or {@code null} the file object is not
		''' found in the given location </returns>
		''' <exception cref="IllegalStateException"> if <seealso cref="#close"/> has been called
		''' and this file manager cannot be reopened </exception>
		Function inferBinaryName(ByVal location As Location, ByVal file As JavaFileObject) As String

		''' <summary>
		''' Compares two file objects and return true if they represent the
		''' same underlying object.
		''' </summary>
		''' <param name="a"> a file object </param>
		''' <param name="b"> a file object </param>
		''' <returns> true if the given file objects represent the same
		''' underlying object
		''' </returns>
		''' <exception cref="IllegalArgumentException"> if either of the arguments
		''' were created with another file manager and this file manager
		''' does not support foreign file objects </exception>
		Function isSameFile(ByVal a As FileObject, ByVal b As FileObject) As Boolean

		''' <summary>
		''' Handles one option.  If {@code current} is an option to this
		''' file manager it will consume any arguments to that option from
		''' {@code remaining} and return true, otherwise return false.
		''' </summary>
		''' <param name="current"> current option </param>
		''' <param name="remaining"> remaining options </param>
		''' <returns> true if this option was handled by this file manager,
		''' false otherwise </returns>
		''' <exception cref="IllegalArgumentException"> if this option to this file
		''' manager is used incorrectly </exception>
		''' <exception cref="IllegalStateException"> if <seealso cref="#close"/> has been called
		''' and this file manager cannot be reopened </exception>
		Function handleOption(ByVal current As String, ByVal remaining As IEnumerator(Of String)) As Boolean

		''' <summary>
		''' Determines if a location is known to this file manager.
		''' </summary>
		''' <param name="location"> a location </param>
		''' <returns> true if the location is known </returns>
		Function hasLocation(ByVal location As Location) As Boolean

		''' <summary>
		''' Gets a <seealso cref="JavaFileObject file object"/> for input
		''' representing the specified class of the specified kind in the
		''' given location.
		''' </summary>
		''' <param name="location"> a location </param>
		''' <param name="className"> the name of a class </param>
		''' <param name="kind"> the kind of file, must be one of {@link
		''' JavaFileObject.Kind#SOURCE SOURCE} or {@link
		''' JavaFileObject.Kind#CLASS CLASS} </param>
		''' <returns> a file object, might return {@code null} if the
		''' file does not exist </returns>
		''' <exception cref="IllegalArgumentException"> if the location is not known
		''' to this file manager and the file manager does not support
		''' unknown locations, or if the kind is not valid </exception>
		''' <exception cref="IOException"> if an I/O error occurred, or if {@link
		''' #close} has been called and this file manager cannot be
		''' reopened </exception>
		''' <exception cref="IllegalStateException"> if <seealso cref="#close"/> has been called
		''' and this file manager cannot be reopened </exception>
		Function getJavaFileForInput(ByVal location As Location, ByVal className As String, ByVal kind As Kind) As JavaFileObject

		''' <summary>
		''' Gets a <seealso cref="JavaFileObject file object"/> for output
		''' representing the specified class of the specified kind in the
		''' given location.
		''' 
		''' <p>Optionally, this file manager might consider the sibling as
		''' a hint for where to place the output.  The exact semantics of
		''' this hint is unspecified.  The JDK compiler, javac, for
		''' example, will place class files in the same directories as
		''' originating source files unless a class file output directory
		''' is provided.  To facilitate this behavior, javac might provide
		''' the originating source file as sibling when calling this
		''' method.
		''' </summary>
		''' <param name="location"> a location </param>
		''' <param name="className"> the name of a class </param>
		''' <param name="kind"> the kind of file, must be one of {@link
		''' JavaFileObject.Kind#SOURCE SOURCE} or {@link
		''' JavaFileObject.Kind#CLASS CLASS} </param>
		''' <param name="sibling"> a file object to be used as hint for placement;
		''' might be {@code null} </param>
		''' <returns> a file object for output </returns>
		''' <exception cref="IllegalArgumentException"> if sibling is not known to
		''' this file manager, or if the location is not known to this file
		''' manager and the file manager does not support unknown
		''' locations, or if the kind is not valid </exception>
		''' <exception cref="IOException"> if an I/O error occurred, or if {@link
		''' #close} has been called and this file manager cannot be
		''' reopened </exception>
		''' <exception cref="IllegalStateException"> <seealso cref="#close"/> has been called
		''' and this file manager cannot be reopened </exception>
		Function getJavaFileForOutput(ByVal location As Location, ByVal className As String, ByVal kind As Kind, ByVal sibling As FileObject) As JavaFileObject

		''' <summary>
		''' Gets a <seealso cref="FileObject file object"/> for input
		''' representing the specified <a href="JavaFileManager.html#relative_name">relative
		''' name</a> in the specified package in the given location.
		''' 
		''' <p>If the returned object represents a {@linkplain
		''' JavaFileObject.Kind#SOURCE source} or {@linkplain
		''' JavaFileObject.Kind#CLASS class} file, it must be an instance
		''' of <seealso cref="JavaFileObject"/>.
		''' 
		''' <p>Informally, the file object returned by this method is
		''' located in the concatenation of the location, package name, and
		''' relative name.  For example, to locate the properties file
		''' "resources/compiler.properties" in the package
		''' "com.sun.tools.javac" in the {@linkplain
		''' StandardLocation#SOURCE_PATH SOURCE_PATH} location, this method
		''' might be called like so:
		''' 
		''' <pre>getFileForInput(SOURCE_PATH, "com.sun.tools.javac", "resources/compiler.properties");</pre>
		''' 
		''' <p>If the call was executed on Windows, with SOURCE_PATH set to
		''' <code>"C:\Documents&nbsp;and&nbsp;Settings\UncleBob\src\share\classes"</code>,
		''' a valid result would be a file object representing the file
		''' <code>"C:\Documents&nbsp;and&nbsp;Settings\UncleBob\src\share\classes\com\sun\tools\javac\resources\compiler.properties"</code>.
		''' </summary>
		''' <param name="location"> a location </param>
		''' <param name="packageName"> a package name </param>
		''' <param name="relativeName"> a relative name </param>
		''' <returns> a file object, might return {@code null} if the file
		''' does not exist </returns>
		''' <exception cref="IllegalArgumentException"> if the location is not known
		''' to this file manager and the file manager does not support
		''' unknown locations, or if {@code relativeName} is not valid </exception>
		''' <exception cref="IOException"> if an I/O error occurred, or if {@link
		''' #close} has been called and this file manager cannot be
		''' reopened </exception>
		''' <exception cref="IllegalStateException"> if <seealso cref="#close"/> has been called
		''' and this file manager cannot be reopened </exception>
		Function getFileForInput(ByVal location As Location, ByVal packageName As String, ByVal relativeName As String) As FileObject

		''' <summary>
		''' Gets a <seealso cref="FileObject file object"/> for output
		''' representing the specified <a href="JavaFileManager.html#relative_name">relative
		''' name</a> in the specified package in the given location.
		''' 
		''' <p>Optionally, this file manager might consider the sibling as
		''' a hint for where to place the output.  The exact semantics of
		''' this hint is unspecified.  The JDK compiler, javac, for
		''' example, will place class files in the same directories as
		''' originating source files unless a class file output directory
		''' is provided.  To facilitate this behavior, javac might provide
		''' the originating source file as sibling when calling this
		''' method.
		''' 
		''' <p>If the returned object represents a {@linkplain
		''' JavaFileObject.Kind#SOURCE source} or {@linkplain
		''' JavaFileObject.Kind#CLASS class} file, it must be an instance
		''' of <seealso cref="JavaFileObject"/>.
		''' 
		''' <p>Informally, the file object returned by this method is
		''' located in the concatenation of the location, package name, and
		''' relative name or next to the sibling argument.  See {@link
		''' #getFileForInput getFileForInput} for an example.
		''' </summary>
		''' <param name="location"> a location </param>
		''' <param name="packageName"> a package name </param>
		''' <param name="relativeName"> a relative name </param>
		''' <param name="sibling"> a file object to be used as hint for placement;
		''' might be {@code null} </param>
		''' <returns> a file object </returns>
		''' <exception cref="IllegalArgumentException"> if sibling is not known to
		''' this file manager, or if the location is not known to this file
		''' manager and the file manager does not support unknown
		''' locations, or if {@code relativeName} is not valid </exception>
		''' <exception cref="IOException"> if an I/O error occurred, or if {@link
		''' #close} has been called and this file manager cannot be
		''' reopened </exception>
		''' <exception cref="IllegalStateException"> if <seealso cref="#close"/> has been called
		''' and this file manager cannot be reopened </exception>
		Function getFileForOutput(ByVal location As Location, ByVal packageName As String, ByVal relativeName As String, ByVal sibling As FileObject) As FileObject

		''' <summary>
		''' Flushes any resources opened for output by this file manager
		''' directly or indirectly.  Flushing a closed file manager has no
		''' effect.
		''' </summary>
		''' <exception cref="IOException"> if an I/O error occurred </exception>
		''' <seealso cref= #close </seealso>
		Sub flush()

		''' <summary>
		''' Releases any resources opened by this file manager directly or
		''' indirectly.  This might render this file manager useless and
		''' the effect of subsequent calls to methods on this object or any
		''' objects obtained through this object is undefined unless
		''' explicitly allowed.  However, closing a file manager which has
		''' already been closed has no effect.
		''' </summary>
		''' <exception cref="IOException"> if an I/O error occurred </exception>
		''' <seealso cref= #flush </seealso>
		Sub close()
	End Interface

End Namespace