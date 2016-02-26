'
' * Copyright (c) 2006, Oracle and/or its affiliates. All rights reserved.
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
	''' File manager based on <seealso cref="File java.io.File"/>.  A common way
	''' to obtain an instance of this class is using {@linkplain
	''' JavaCompiler#getStandardFileManager
	''' getStandardFileManager}, for example:
	''' 
	''' <pre>
	'''   JavaCompiler compiler = ToolProvider.getSystemJavaCompiler();
	'''   {@code DiagnosticCollector<JavaFileObject>} diagnostics =
	'''       new {@code DiagnosticCollector<JavaFileObject>()};
	'''   StandardJavaFileManager fm = compiler.getStandardFileManager(diagnostics, null, null);
	''' </pre>
	''' 
	''' This file manager creates file objects representing regular
	''' <seealso cref="File files"/>,
	''' <seealso cref="java.util.zip.ZipEntry zip file entries"/>, or entries in
	''' similar file system based containers.  Any file object returned
	''' from a file manager implementing this interface must observe the
	''' following behavior:
	''' 
	''' <ul>
	'''   <li>
	'''     File names need not be canonical.
	'''   </li>
	'''   <li>
	'''     For file objects representing regular files
	'''     <ul>
	'''       <li>
	'''         the method <code><seealso cref="FileObject#delete()"/></code>
	'''         is equivalent to <code><seealso cref="File#delete()"/></code>,
	'''       </li>
	'''       <li>
	'''         the method <code><seealso cref="FileObject#getLastModified()"/></code>
	'''         is equivalent to <code><seealso cref="File#lastModified()"/></code>,
	'''       </li>
	'''       <li>
	'''         the methods <code><seealso cref="FileObject#getCharContent(boolean)"/></code>,
	'''         <code><seealso cref="FileObject#openInputStream()"/></code>, and
	'''         <code><seealso cref="FileObject#openReader(boolean)"/></code>
	'''         must succeed if the following would succeed (ignoring
	'''         encoding issues):
	'''         <blockquote>
	'''           <pre>new <seealso cref="java.io.FileInputStream#FileInputStream(File) FileInputStream"/>(new <seealso cref="File#File(java.net.URI) File"/>(<seealso cref="FileObject fileObject"/>.{@linkplain FileObject#toUri() toUri}()))</pre>
	'''         </blockquote>
	'''       </li>
	'''       <li>
	'''         and the methods
	'''         <code><seealso cref="FileObject#openOutputStream()"/></code>, and
	'''         <code><seealso cref="FileObject#openWriter()"/></code> must
	'''         succeed if the following would succeed (ignoring encoding
	'''         issues):
	'''         <blockquote>
	'''           <pre>new <seealso cref="java.io.FileOutputStream#FileOutputStream(File) FileOutputStream"/>(new <seealso cref="File#File(java.net.URI) File"/>(<seealso cref="FileObject fileObject"/>.{@linkplain FileObject#toUri() toUri}()))</pre>
	'''         </blockquote>
	'''       </li>
	'''     </ul>
	'''   </li>
	'''   <li>
	'''     The <seealso cref="java.net.URI URI"/> returned from
	'''     <code><seealso cref="FileObject#toUri()"/></code>
	'''     <ul>
	'''       <li>
	'''         must be <seealso cref="java.net.URI#isAbsolute() absolute"/> (have a schema), and
	'''       </li>
	'''       <li>
	'''         must have a <seealso cref="java.net.URI#normalize() normalized"/>
	'''         <seealso cref="java.net.URI#getPath() path component"/> which
	'''         can be resolved without any process-specific context such
	'''         as the current directory (file names must be absolute).
	'''       </li>
	'''     </ul>
	'''   </li>
	''' </ul>
	''' 
	''' According to these rules, the following URIs, for example, are
	''' allowed:
	''' <ul>
	'''   <li>
	'''     <code>file:///C:/Documents%20and%20Settings/UncleBob/BobsApp/Test.java</code>
	'''   </li>
	'''   <li>
	'''     <code>jar:///C:/Documents%20and%20Settings/UncleBob/lib/vendorA.jar!com/vendora/LibraryClass.class</code>
	'''   </li>
	''' </ul>
	''' Whereas these are not (reason in parentheses):
	''' <ul>
	'''   <li>
	'''     <code>file:BobsApp/Test.java</code> (the file name is relative
	'''     and depend on the current directory)
	'''   </li>
	'''   <li>
	'''     <code>jar:lib/vendorA.jar!com/vendora/LibraryClass.class</code>
	'''     (the first half of the path depends on the current directory,
	'''     whereas the component after ! is legal)
	'''   </li>
	'''   <li>
	'''     <code>Test.java</code> (this URI depends on the current
	'''     directory and does not have a schema)
	'''   </li>
	'''   <li>
	'''     <code>jar:///C:/Documents%20and%20Settings/UncleBob/BobsApp/../lib/vendorA.jar!com/vendora/LibraryClass.class</code>
	'''     (the path is not normalized)
	'''   </li>
	''' </ul>
	''' 
	''' @author Peter von der Ah&eacute;
	''' @since 1.6
	''' </summary>
	Public Interface StandardJavaFileManager
		Inherits JavaFileManager

		''' <summary>
		''' Compares two file objects and return true if they represent the
		''' same canonical file, zip file entry, or entry in any file
		''' system based container.
		''' </summary>
		''' <param name="a"> a file object </param>
		''' <param name="b"> a file object </param>
		''' <returns> true if the given file objects represent the same
		''' canonical file or zip file entry; false otherwise
		''' </returns>
		''' <exception cref="IllegalArgumentException"> if either of the arguments
		''' were created with another file manager implementation </exception>
		Function isSameFile(ByVal a As FileObject, ByVal b As FileObject) As Boolean

		''' <summary>
		''' Gets file objects representing the given files.
		''' </summary>
		''' <param name="files"> a list of files </param>
		''' <returns> a list of file objects </returns>
		''' <exception cref="IllegalArgumentException"> if the list of files includes
		''' a directory </exception>
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
		Function getJavaFileObjectsFromFiles(Of T1 As java.io.File)(ByVal files As IEnumerable(Of T1)) As IEnumerable(Of ? As JavaFileObject)

		''' <summary>
		''' Gets file objects representing the given files.
		''' Convenience method equivalent to:
		''' 
		''' <pre>
		'''     getJavaFileObjectsFromFiles(<seealso cref="java.util.Arrays#asList Arrays.asList"/>(files))
		''' </pre>
		''' </summary>
		''' <param name="files"> an array of files </param>
		''' <returns> a list of file objects </returns>
		''' <exception cref="IllegalArgumentException"> if the array of files includes
		''' a directory </exception>
		''' <exception cref="NullPointerException"> if the given array contains null
		''' elements </exception>
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
		Function getJavaFileObjects(ParamArray ByVal files As java.io.File()) As IEnumerable(Of ? As JavaFileObject)

		''' <summary>
		''' Gets file objects representing the given file names.
		''' </summary>
		''' <param name="names"> a list of file names </param>
		''' <returns> a list of file objects </returns>
		''' <exception cref="IllegalArgumentException"> if the list of file names
		''' includes a directory </exception>
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
		Function getJavaFileObjectsFromStrings(ByVal names As IEnumerable(Of String)) As IEnumerable(Of ? As JavaFileObject)

		''' <summary>
		''' Gets file objects representing the given file names.
		''' Convenience method equivalent to:
		''' 
		''' <pre>
		'''     getJavaFileObjectsFromStrings(<seealso cref="java.util.Arrays#asList Arrays.asList"/>(names))
		''' </pre>
		''' </summary>
		''' <param name="names"> a list of file names </param>
		''' <returns> a list of file objects </returns>
		''' <exception cref="IllegalArgumentException"> if the array of file names
		''' includes a directory </exception>
		''' <exception cref="NullPointerException"> if the given array contains null
		''' elements </exception>
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
		Function getJavaFileObjects(ParamArray ByVal names As String()) As IEnumerable(Of ? As JavaFileObject)

		''' <summary>
		''' Associates the given path with the given location.  Any
		''' previous value will be discarded.
		''' </summary>
		''' <param name="location"> a location </param>
		''' <param name="path"> a list of files, if {@code null} use the default
		''' path for this location </param>
		''' <seealso cref= #getLocation </seealso>
		''' <exception cref="IllegalArgumentException"> if location is an output
		''' location and path does not contain exactly one element </exception>
		''' <exception cref="IOException"> if location is an output location and path
		''' does not represent an existing directory </exception>
		Sub setLocation(Of T1 As java.io.File)(ByVal location As Location, ByVal path As IEnumerable(Of T1))

		''' <summary>
		''' Gets the path associated with the given location.
		''' </summary>
		''' <param name="location"> a location </param>
		''' <returns> a list of files or {@code null} if this location has no
		''' associated path </returns>
		''' <seealso cref= #setLocation </seealso>
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
		Function getLocation(ByVal location As Location) As IEnumerable(Of ? As java.io.File)

	End Interface

End Namespace