Imports System
Imports System.Runtime.CompilerServices
Imports System.Diagnostics
Imports System.Collections.Generic

'
' * Copyright (c) 1994, 2013, Oracle and/or its affiliates. All rights reserved.
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

Namespace java.io


	''' <summary>
	''' An abstract representation of file and directory pathnames.
	''' 
	''' <p> User interfaces and operating systems use system-dependent <em>pathname
	''' strings</em> to name files and directories.  This class presents an
	''' abstract, system-independent view of hierarchical pathnames.  An
	''' <em>abstract pathname</em> has two components:
	''' 
	''' <ol>
	''' <li> An optional system-dependent <em>prefix</em> string,
	'''      such as a disk-drive specifier, <code>"/"</code>&nbsp;for the UNIX root
	'''      directory, or <code>"\\\\"</code>&nbsp;for a Microsoft Windows UNC pathname, and
	''' <li> A sequence of zero or more string <em>names</em>.
	''' </ol>
	''' 
	''' The first name in an abstract pathname may be a directory name or, in the
	''' case of Microsoft Windows UNC pathnames, a hostname.  Each subsequent name
	''' in an abstract pathname denotes a directory; the last name may denote
	''' either a directory or a file.  The <em>empty</em> abstract pathname has no
	''' prefix and an empty name sequence.
	''' 
	''' <p> The conversion of a pathname string to or from an abstract pathname is
	''' inherently system-dependent.  When an abstract pathname is converted into a
	''' pathname string, each name is separated from the next by a single copy of
	''' the default <em>separator character</em>.  The default name-separator
	''' character is defined by the system property <code>file.separator</code>, and
	''' is made available in the Public Shared fields <code>{@link
	''' #separator}</code> and <code><seealso cref="#separatorChar"/></code> of this class.
	''' When a pathname string is converted into an abstract pathname, the names
	''' within it may be separated by the default name-separator character or by any
	''' other name-separator character that is supported by the underlying system.
	''' 
	''' <p> A pathname, whether abstract or in string form, may be either
	''' <em>absolute</em> or <em>relative</em>.  An absolute pathname is complete in
	''' that no other information is required in order to locate the file that it
	''' denotes.  A relative pathname, in contrast, must be interpreted in terms of
	''' information taken from some other pathname.  By default the classes in the
	''' <code>java.io</code> package always resolve relative pathnames against the
	''' current user directory.  This directory is named by the system property
	''' <code>user.dir</code>, and is typically the directory in which the Java
	''' virtual machine was invoked.
	''' 
	''' <p> The <em>parent</em> of an abstract pathname may be obtained by invoking
	''' the <seealso cref="#getParent"/> method of this class and consists of the pathname's
	''' prefix and each name in the pathname's name sequence except for the last.
	''' Each directory's absolute pathname is an ancestor of any <tt>File</tt>
	''' object with an absolute abstract pathname which begins with the directory's
	''' absolute pathname.  For example, the directory denoted by the abstract
	''' pathname <tt>"/usr"</tt> is an ancestor of the directory denoted by the
	''' pathname <tt>"/usr/local/bin"</tt>.
	''' 
	''' <p> The prefix concept is used to handle root directories on UNIX platforms,
	''' and drive specifiers, root directories and UNC pathnames on Microsoft Windows platforms,
	''' as follows:
	''' 
	''' <ul>
	''' 
	''' <li> For UNIX platforms, the prefix of an absolute pathname is always
	''' <code>"/"</code>.  Relative pathnames have no prefix.  The abstract pathname
	''' denoting the root directory has the prefix <code>"/"</code> and an empty
	''' name sequence.
	''' 
	''' <li> For Microsoft Windows platforms, the prefix of a pathname that contains a drive
	''' specifier consists of the drive letter followed by <code>":"</code> and
	''' possibly followed by <code>"\\"</code> if the pathname is absolute.  The
	''' prefix of a UNC pathname is <code>"\\\\"</code>; the hostname and the share
	''' name are the first two names in the name sequence.  A relative pathname that
	''' does not specify a drive has no prefix.
	''' 
	''' </ul>
	''' 
	''' <p> Instances of this class may or may not denote an actual file-system
	''' object such as a file or a directory.  If it does denote such an object
	''' then that object resides in a <i>partition</i>.  A partition is an
	''' operating system-specific portion of storage for a file system.  A single
	''' storage device (e.g. a physical disk-drive, flash memory, CD-ROM) may
	''' contain multiple partitions.  The object, if any, will reside on the
	''' partition <a name="partName">named</a> by some ancestor of the absolute
	''' form of this pathname.
	''' 
	''' <p> A file system may implement restrictions to certain operations on the
	''' actual file-system object, such as reading, writing, and executing.  These
	''' restrictions are collectively known as <i>access permissions</i>.  The file
	''' system may have multiple sets of access permissions on a single object.
	''' For example, one set may apply to the object's <i>owner</i>, and another
	''' may apply to all other users.  The access permissions on an object may
	''' cause some methods in this class to fail.
	''' 
	''' <p> Instances of the <code>File</code> class are immutable; that is, once
	''' created, the abstract pathname represented by a <code>File</code> object
	''' will never change.
	''' 
	''' <h3>Interoperability with {@code java.nio.file} package</h3>
	''' 
	''' <p> The <a href="../../java/nio/file/package-summary.html">{@code java.nio.file}</a>
	''' package defines interfaces and classes for the Java virtual machine to access
	''' files, file attributes, and file systems. This API may be used to overcome
	''' many of the limitations of the {@code java.io.File} class.
	''' The <seealso cref="#toPath toPath"/> method may be used to obtain a {@link
	''' Path} that uses the abstract path represented by a {@code File} object to
	''' locate a file. The resulting {@code Path} may be used with the {@link
	''' java.nio.file.Files} class to provide more efficient and extensive access to
	''' additional file operations, file attributes, and I/O exceptions to help
	''' diagnose errors when an operation on a file fails.
	''' 
	''' @author  unascribed
	''' @since   JDK1.0
	''' </summary>

	<Serializable> _
	Public Class File
		Implements Comparable(Of File)

		''' <summary>
		''' The FileSystem object representing the platform's local file system.
		''' </summary>
		Private Shared ReadOnly fs As FileSystem = DefaultFileSystem.fileSystem

		''' <summary>
		''' This abstract pathname's normalized pathname string. A normalized
		''' pathname string uses the default name-separator character and does not
		''' contain any duplicate or redundant separators.
		''' 
		''' @serial
		''' </summary>
		Private ReadOnly path As String

		''' <summary>
		''' Enum type that indicates the status of a file path.
		''' </summary>
		Private Enum PathStatus
			INVALID
			CHECKED
		End Enum

		''' <summary>
		''' The flag indicating whether the file path is invalid.
		''' </summary>
		<NonSerialized> _
		Private status As PathStatus = Nothing

		''' <summary>
		''' Check if the file has an invalid path. Currently, the inspection of
		''' a file path is very limited, and it only covers Nul character check.
		''' Returning true means the path is definitely invalid/garbage. But
		''' returning false does not guarantee that the path is valid.
		''' </summary>
		''' <returns> true if the file path is invalid. </returns>
		Friend Property invalid As Boolean
			Get
				If status Is Nothing Then status = If(Me.path.IndexOf(ChrW(&H0000)) < 0, PathStatus.CHECKED, PathStatus.INVALID)
				Return status = PathStatus.INVALID
			End Get
		End Property

		''' <summary>
		''' The length of this abstract pathname's prefix, or zero if it has no
		''' prefix.
		''' </summary>
		<NonSerialized> _
		Private ReadOnly prefixLength As Integer

		''' <summary>
		''' Returns the length of this abstract pathname's prefix.
		''' For use by FileSystem classes.
		''' </summary>
		Friend Overridable Property prefixLength As Integer
			Get
				Return prefixLength
			End Get
		End Property

		''' <summary>
		''' The system-dependent default name-separator character.  This field is
		''' initialized to contain the first character of the value of the system
		''' property <code>file.separator</code>.  On UNIX systems the value of this
		''' field is <code>'/'</code>; on Microsoft Windows systems it is <code>'\\'</code>.
		''' </summary>
		''' <seealso cref=     java.lang.System#getProperty(java.lang.String) </seealso>
		Public Shared ReadOnly separatorChar As Char = fs.separator

		''' <summary>
		''' The system-dependent default name-separator character, represented as a
		''' string for convenience.  This string contains a single character, namely
		''' <code><seealso cref="#separatorChar"/></code>.
		''' </summary>
		Public Shared ReadOnly separator As String = "" & AscW(separatorChar)

		''' <summary>
		''' The system-dependent path-separator character.  This field is
		''' initialized to contain the first character of the value of the system
		''' property <code>path.separator</code>.  This character is used to
		''' separate filenames in a sequence of files given as a <em>path list</em>.
		''' On UNIX systems, this character is <code>':'</code>; on Microsoft Windows systems it
		''' is <code>';'</code>.
		''' </summary>
		''' <seealso cref=     java.lang.System#getProperty(java.lang.String) </seealso>
		Public Shared ReadOnly pathSeparatorChar As Char = fs.pathSeparator

		''' <summary>
		''' The system-dependent path-separator character, represented as a string
		''' for convenience.  This string contains a single character, namely
		''' <code><seealso cref="#pathSeparatorChar"/></code>.
		''' </summary>
		Public Shared ReadOnly pathSeparator As String = "" & AscW(pathSeparatorChar)


		' -- Constructors -- 

		''' <summary>
		''' Internal constructor for already-normalized pathname strings.
		''' </summary>
		Private Sub New(ByVal pathname As String, ByVal prefixLength As Integer)
			Me.path = pathname
			Me.prefixLength = prefixLength
		End Sub

		''' <summary>
		''' Internal constructor for already-normalized pathname strings.
		''' The parameter order is used to disambiguate this method from the
		''' public(File, String) constructor.
		''' </summary>
		Private Sub New(ByVal child As String, ByVal parent As File)
			Debug.Assert(parent.path IsNot Nothing)
			assert((Not parent.path.Equals("")))
			Me.path = fs.resolve(parent.path, child)
			Me.prefixLength = parent.prefixLength
		End Sub

		''' <summary>
		''' Creates a new <code>File</code> instance by converting the given
		''' pathname string into an abstract pathname.  If the given string is
		''' the empty string, then the result is the empty abstract pathname.
		''' </summary>
		''' <param name="pathname">  A pathname string </param>
		''' <exception cref="NullPointerException">
		'''          If the <code>pathname</code> argument is <code>null</code> </exception>
		Public Sub New(ByVal pathname As String)
			If pathname Is Nothing Then Throw New NullPointerException
			Me.path = fs.normalize(pathname)
			Me.prefixLength = fs.prefixLength(Me.path)
		End Sub

	'     Note: The two-argument File constructors do not interpret an empty
	'       parent abstract pathname as the current user directory.  An empty parent
	'       instead causes the child to be resolved against the system-dependent
	'       directory defined by the FileSystem.getDefaultParent method.  On Unix
	'       this default is "/", while on Microsoft Windows it is "\\".  This is required for
	'       compatibility with the original behavior of this class. 

		''' <summary>
		''' Creates a new <code>File</code> instance from a parent pathname string
		''' and a child pathname string.
		''' 
		''' <p> If <code>parent</code> is <code>null</code> then the new
		''' <code>File</code> instance is created as if by invoking the
		''' single-argument <code>File</code> constructor on the given
		''' <code>child</code> pathname string.
		''' 
		''' <p> Otherwise the <code>parent</code> pathname string is taken to denote
		''' a directory, and the <code>child</code> pathname string is taken to
		''' denote either a directory or a file.  If the <code>child</code> pathname
		''' string is absolute then it is converted into a relative pathname in a
		''' system-dependent way.  If <code>parent</code> is the empty string then
		''' the new <code>File</code> instance is created by converting
		''' <code>child</code> into an abstract pathname and resolving the result
		''' against a system-dependent default directory.  Otherwise each pathname
		''' string is converted into an abstract pathname and the child abstract
		''' pathname is resolved against the parent.
		''' </summary>
		''' <param name="parent">  The parent pathname string </param>
		''' <param name="child">   The child pathname string </param>
		''' <exception cref="NullPointerException">
		'''          If <code>child</code> is <code>null</code> </exception>
		Public Sub New(ByVal parent As String, ByVal child As String)
			If child Is Nothing Then Throw New NullPointerException
			If parent IsNot Nothing Then
				If parent.Equals("") Then
					Me.path = fs.resolve(fs.defaultParent, fs.normalize(child))
				Else
					Me.path = fs.resolve(fs.normalize(parent), fs.normalize(child))
				End If
			Else
				Me.path = fs.normalize(child)
			End If
			Me.prefixLength = fs.prefixLength(Me.path)
		End Sub

		''' <summary>
		''' Creates a new <code>File</code> instance from a parent abstract
		''' pathname and a child pathname string.
		''' 
		''' <p> If <code>parent</code> is <code>null</code> then the new
		''' <code>File</code> instance is created as if by invoking the
		''' single-argument <code>File</code> constructor on the given
		''' <code>child</code> pathname string.
		''' 
		''' <p> Otherwise the <code>parent</code> abstract pathname is taken to
		''' denote a directory, and the <code>child</code> pathname string is taken
		''' to denote either a directory or a file.  If the <code>child</code>
		''' pathname string is absolute then it is converted into a relative
		''' pathname in a system-dependent way.  If <code>parent</code> is the empty
		''' abstract pathname then the new <code>File</code> instance is created by
		''' converting <code>child</code> into an abstract pathname and resolving
		''' the result against a system-dependent default directory.  Otherwise each
		''' pathname string is converted into an abstract pathname and the child
		''' abstract pathname is resolved against the parent.
		''' </summary>
		''' <param name="parent">  The parent abstract pathname </param>
		''' <param name="child">   The child pathname string </param>
		''' <exception cref="NullPointerException">
		'''          If <code>child</code> is <code>null</code> </exception>
		Public Sub New(ByVal parent As File, ByVal child As String)
			If child Is Nothing Then Throw New NullPointerException
			If parent IsNot Nothing Then
				If parent.path.Equals("") Then
					Me.path = fs.resolve(fs.defaultParent, fs.normalize(child))
				Else
					Me.path = fs.resolve(parent.path, fs.normalize(child))
				End If
			Else
				Me.path = fs.normalize(child)
			End If
			Me.prefixLength = fs.prefixLength(Me.path)
		End Sub

		''' <summary>
		''' Creates a new <tt>File</tt> instance by converting the given
		''' <tt>file:</tt> URI into an abstract pathname.
		''' 
		''' <p> The exact form of a <tt>file:</tt> URI is system-dependent, hence
		''' the transformation performed by this constructor is also
		''' system-dependent.
		''' 
		''' <p> For a given abstract pathname <i>f</i> it is guaranteed that
		''' 
		''' <blockquote><tt>
		''' new File(</tt><i>&nbsp;f</i><tt>.<seealso cref="#toURI() toURI"/>()).equals(</tt><i>&nbsp;f</i><tt>.<seealso cref="#getAbsoluteFile() getAbsoluteFile"/>())
		''' </tt></blockquote>
		''' 
		''' so long as the original abstract pathname, the URI, and the new abstract
		''' pathname are all created in (possibly different invocations of) the same
		''' Java virtual machine.  This relationship typically does not hold,
		''' however, when a <tt>file:</tt> URI that is created in a virtual machine
		''' on one operating system is converted into an abstract pathname in a
		''' virtual machine on a different operating system.
		''' </summary>
		''' <param name="uri">
		'''         An absolute, hierarchical URI with a scheme equal to
		'''         <tt>"file"</tt>, a non-empty path component, and undefined
		'''         authority, query, and fragment components
		''' </param>
		''' <exception cref="NullPointerException">
		'''          If <tt>uri</tt> is <tt>null</tt>
		''' </exception>
		''' <exception cref="IllegalArgumentException">
		'''          If the preconditions on the parameter do not hold
		''' </exception>
		''' <seealso cref= #toURI() </seealso>
		''' <seealso cref= java.net.URI
		''' @since 1.4 </seealso>
		Public Sub New(ByVal uri As java.net.URI)

			' Check our many preconditions
			If Not uri.absolute Then Throw New IllegalArgumentException("URI is not absolute")
			If uri.opaque Then Throw New IllegalArgumentException("URI is not hierarchical")
			Dim scheme As String = uri.scheme
			If (scheme Is Nothing) OrElse (Not scheme.equalsIgnoreCase("file")) Then Throw New IllegalArgumentException("URI scheme is not ""file""")
			If uri.authority IsNot Nothing Then Throw New IllegalArgumentException("URI has an authority component")
			If uri.fragment IsNot Nothing Then Throw New IllegalArgumentException("URI has a fragment component")
			If uri.query IsNot Nothing Then Throw New IllegalArgumentException("URI has a query component")
			Dim p As String = uri.path
			If p.Equals("") Then Throw New IllegalArgumentException("URI path component is empty")

			' Okay, now initialize
			p = fs.fromURIPath(p)
			If System.IO.Path.DirectorySeparatorChar <> "/"c Then p = p.replace("/"c, System.IO.Path.DirectorySeparatorChar)
			Me.path = fs.normalize(p)
			Me.prefixLength = fs.prefixLength(Me.path)
		End Sub


		' -- Path-component accessors -- 

		''' <summary>
		''' Returns the name of the file or directory denoted by this abstract
		''' pathname.  This is just the last name in the pathname's name
		''' sequence.  If the pathname's name sequence is empty, then the empty
		''' string is returned.
		''' </summary>
		''' <returns>  The name of the file or directory denoted by this abstract
		'''          pathname, or the empty string if this pathname's name sequence
		'''          is empty </returns>
		Public Overridable Property name As String
			Get
				Dim index As Integer = path.LastIndexOf(separatorChar)
				If index < prefixLength Then Return path.Substring(prefixLength)
				Return path.Substring(index + 1)
			End Get
		End Property

		''' <summary>
		''' Returns the pathname string of this abstract pathname's parent, or
		''' <code>null</code> if this pathname does not name a parent directory.
		''' 
		''' <p> The <em>parent</em> of an abstract pathname consists of the
		''' pathname's prefix, if any, and each name in the pathname's name
		''' sequence except for the last.  If the name sequence is empty then
		''' the pathname does not name a parent directory.
		''' </summary>
		''' <returns>  The pathname string of the parent directory named by this
		'''          abstract pathname, or <code>null</code> if this pathname
		'''          does not name a parent </returns>
		Public Overridable Property parent As String
			Get
				Dim index As Integer = path.LastIndexOf(separatorChar)
				If index < prefixLength Then
					If (prefixLength > 0) AndAlso (path.length() > prefixLength) Then Return path.Substring(0, prefixLength)
					Return Nothing
				End If
				Return path.Substring(0, index)
			End Get
		End Property

		''' <summary>
		''' Returns the abstract pathname of this abstract pathname's parent,
		''' or <code>null</code> if this pathname does not name a parent
		''' directory.
		''' 
		''' <p> The <em>parent</em> of an abstract pathname consists of the
		''' pathname's prefix, if any, and each name in the pathname's name
		''' sequence except for the last.  If the name sequence is empty then
		''' the pathname does not name a parent directory.
		''' </summary>
		''' <returns>  The abstract pathname of the parent directory named by this
		'''          abstract pathname, or <code>null</code> if this pathname
		'''          does not name a parent
		''' 
		''' @since 1.2 </returns>
		Public Overridable Property parentFile As File
			Get
				Dim p As String = Me.parent
				If p Is Nothing Then Return Nothing
				Return New File(p, Me.prefixLength)
			End Get
		End Property

		''' <summary>
		''' Converts this abstract pathname into a pathname string.  The resulting
		''' string uses the <seealso cref="#separator default name-separator character"/> to
		''' separate the names in the name sequence.
		''' </summary>
		''' <returns>  The string form of this abstract pathname </returns>
		Public Overridable Property path As String
			Get
				Return path
			End Get
		End Property


		' -- Path operations -- 

		''' <summary>
		''' Tests whether this abstract pathname is absolute.  The definition of
		''' absolute pathname is system dependent.  On UNIX systems, a pathname is
		''' absolute if its prefix is <code>"/"</code>.  On Microsoft Windows systems, a
		''' pathname is absolute if its prefix is a drive specifier followed by
		''' <code>"\\"</code>, or if its prefix is <code>"\\\\"</code>.
		''' </summary>
		''' <returns>  <code>true</code> if this abstract pathname is absolute,
		'''          <code>false</code> otherwise </returns>
		Public Overridable Property absolute As Boolean
			Get
				Return fs.isAbsolute(Me)
			End Get
		End Property

		''' <summary>
		''' Returns the absolute pathname string of this abstract pathname.
		''' 
		''' <p> If this abstract pathname is already absolute, then the pathname
		''' string is simply returned as if by the <code><seealso cref="#getPath"/></code>
		''' method.  If this abstract pathname is the empty abstract pathname then
		''' the pathname string of the current user directory, which is named by the
		''' system property <code>user.dir</code>, is returned.  Otherwise this
		''' pathname is resolved in a system-dependent way.  On UNIX systems, a
		''' relative pathname is made absolute by resolving it against the current
		''' user directory.  On Microsoft Windows systems, a relative pathname is made absolute
		''' by resolving it against the current directory of the drive named by the
		''' pathname, if any; if not, it is resolved against the current user
		''' directory.
		''' </summary>
		''' <returns>  The absolute pathname string denoting the same file or
		'''          directory as this abstract pathname
		''' </returns>
		''' <exception cref="SecurityException">
		'''          If a required system property value cannot be accessed.
		''' </exception>
		''' <seealso cref=     java.io.File#isAbsolute() </seealso>
		Public Overridable Property absolutePath As String
			Get
				Return fs.resolve(Me)
			End Get
		End Property

		''' <summary>
		''' Returns the absolute form of this abstract pathname.  Equivalent to
		''' <code>new&nbsp;File(this.<seealso cref="#getAbsolutePath"/>)</code>.
		''' </summary>
		''' <returns>  The absolute abstract pathname denoting the same file or
		'''          directory as this abstract pathname
		''' </returns>
		''' <exception cref="SecurityException">
		'''          If a required system property value cannot be accessed.
		''' 
		''' @since 1.2 </exception>
		Public Overridable Property absoluteFile As File
			Get
				Dim absPath As String = absolutePath
				Return New File(absPath, fs.prefixLength(absPath))
			End Get
		End Property

		''' <summary>
		''' Returns the canonical pathname string of this abstract pathname.
		''' 
		''' <p> A canonical pathname is both absolute and unique.  The precise
		''' definition of canonical form is system-dependent.  This method first
		''' converts this pathname to absolute form if necessary, as if by invoking the
		''' <seealso cref="#getAbsolutePath"/> method, and then maps it to its unique form in a
		''' system-dependent way.  This typically involves removing redundant names
		''' such as <tt>"."</tt> and <tt>".."</tt> from the pathname, resolving
		''' symbolic links (on UNIX platforms), and converting drive letters to a
		''' standard case (on Microsoft Windows platforms).
		''' 
		''' <p> Every pathname that denotes an existing file or directory has a
		''' unique canonical form.  Every pathname that denotes a nonexistent file
		''' or directory also has a unique canonical form.  The canonical form of
		''' the pathname of a nonexistent file or directory may be different from
		''' the canonical form of the same pathname after the file or directory is
		''' created.  Similarly, the canonical form of the pathname of an existing
		''' file or directory may be different from the canonical form of the same
		''' pathname after the file or directory is deleted.
		''' </summary>
		''' <returns>  The canonical pathname string denoting the same file or
		'''          directory as this abstract pathname
		''' </returns>
		''' <exception cref="IOException">
		'''          If an I/O error occurs, which is possible because the
		'''          construction of the canonical pathname may require
		'''          filesystem queries
		''' </exception>
		''' <exception cref="SecurityException">
		'''          If a required system property value cannot be accessed, or
		'''          if a security manager exists and its <code>{@link
		'''          java.lang.SecurityManager#checkRead}</code> method denies
		'''          read access to the file
		''' 
		''' @since   JDK1.1 </exception>
		''' <seealso cref=     Path#toRealPath </seealso>
		Public Overridable Property canonicalPath As String
			Get
				If invalid Then Throw New IOException("Invalid file path")
				Return fs.canonicalize(fs.resolve(Me))
			End Get
		End Property

		''' <summary>
		''' Returns the canonical form of this abstract pathname.  Equivalent to
		''' <code>new&nbsp;File(this.<seealso cref="#getCanonicalPath"/>)</code>.
		''' </summary>
		''' <returns>  The canonical pathname string denoting the same file or
		'''          directory as this abstract pathname
		''' </returns>
		''' <exception cref="IOException">
		'''          If an I/O error occurs, which is possible because the
		'''          construction of the canonical pathname may require
		'''          filesystem queries
		''' </exception>
		''' <exception cref="SecurityException">
		'''          If a required system property value cannot be accessed, or
		'''          if a security manager exists and its <code>{@link
		'''          java.lang.SecurityManager#checkRead}</code> method denies
		'''          read access to the file
		''' 
		''' @since 1.2 </exception>
		''' <seealso cref=     Path#toRealPath </seealso>
		Public Overridable Property canonicalFile As File
			Get
				Dim canonPath As String = canonicalPath
				Return New File(canonPath, fs.prefixLength(canonPath))
			End Get
		End Property

		Private Shared Function slashify(ByVal path As String, ByVal isDirectory As Boolean) As String
			Dim p As String = path
			If System.IO.Path.DirectorySeparatorChar <> "/"c Then p = p.replace(System.IO.Path.DirectorySeparatorChar, "/"c)
			If Not p.StartsWith("/") Then p = "/" & p
			If (Not p.EndsWith("/")) AndAlso isDirectory Then p = p & "/"
			Return p
		End Function

		''' <summary>
		''' Converts this abstract pathname into a <code>file:</code> URL.  The
		''' exact form of the URL is system-dependent.  If it can be determined that
		''' the file denoted by this abstract pathname is a directory, then the
		''' resulting URL will end with a slash.
		''' </summary>
		''' <returns>  A URL object representing the equivalent file URL
		''' </returns>
		''' <exception cref="MalformedURLException">
		'''          If the path cannot be parsed as a URL
		''' </exception>
		''' <seealso cref=     #toURI() </seealso>
		''' <seealso cref=     java.net.URI </seealso>
		''' <seealso cref=     java.net.URI#toURL() </seealso>
		''' <seealso cref=     java.net.URL
		''' @since   1.2
		''' </seealso>
		''' @deprecated This method does not automatically escape characters that
		''' are illegal in URLs.  It is recommended that new code convert an
		''' abstract pathname into a URL by first converting it into a URI, via the
		''' <seealso cref="#toURI() toURI"/> method, and then converting the URI into a URL
		''' via the <seealso cref="java.net.URI#toURL() URI.toURL"/> method. 
		<Obsolete("This method does not automatically escape characters that")> _
		Public Overridable Function toURL() As java.net.URL
			If invalid Then Throw New java.net.MalformedURLException("Invalid file path")
			Return New java.net.URL("file", "", slashify(absolutePath, directory))
		End Function

		''' <summary>
		''' Constructs a <tt>file:</tt> URI that represents this abstract pathname.
		''' 
		''' <p> The exact form of the URI is system-dependent.  If it can be
		''' determined that the file denoted by this abstract pathname is a
		''' directory, then the resulting URI will end with a slash.
		''' 
		''' <p> For a given abstract pathname <i>f</i>, it is guaranteed that
		''' 
		''' <blockquote><tt>
		''' new <seealso cref="#File(java.net.URI) File"/>(</tt><i>&nbsp;f</i><tt>.toURI()).equals(</tt><i>&nbsp;f</i><tt>.<seealso cref="#getAbsoluteFile() getAbsoluteFile"/>())
		''' </tt></blockquote>
		''' 
		''' so long as the original abstract pathname, the URI, and the new abstract
		''' pathname are all created in (possibly different invocations of) the same
		''' Java virtual machine.  Due to the system-dependent nature of abstract
		''' pathnames, however, this relationship typically does not hold when a
		''' <tt>file:</tt> URI that is created in a virtual machine on one operating
		''' system is converted into an abstract pathname in a virtual machine on a
		''' different operating system.
		''' 
		''' <p> Note that when this abstract pathname represents a UNC pathname then
		''' all components of the UNC (including the server name component) are encoded
		''' in the {@code URI} path. The authority component is undefined, meaning
		''' that it is represented as {@code null}. The <seealso cref="Path"/> class defines the
		''' <seealso cref="Path#toUri toUri"/> method to encode the server name in the authority
		''' component of the resulting {@code URI}. The <seealso cref="#toPath toPath"/> method
		''' may be used to obtain a {@code Path} representing this abstract pathname.
		''' </summary>
		''' <returns>  An absolute, hierarchical URI with a scheme equal to
		'''          <tt>"file"</tt>, a path representing this abstract pathname,
		'''          and undefined authority, query, and fragment components </returns>
		''' <exception cref="SecurityException"> If a required system property value cannot
		''' be accessed.
		''' </exception>
		''' <seealso cref= #File(java.net.URI) </seealso>
		''' <seealso cref= java.net.URI </seealso>
		''' <seealso cref= java.net.URI#toURL()
		''' @since 1.4 </seealso>
		Public Overridable Function toURI() As java.net.URI
			Try
				Dim f As File = absoluteFile
				Dim sp As String = slashify(f.path, f.directory)
				If sp.StartsWith("//") Then sp = "//" & sp
				Return New java.net.URI("file", Nothing, sp, Nothing)
			Catch x As java.net.URISyntaxException
				Throw New [Error](x) ' Can't happen
			End Try
		End Function


		' -- Attribute accessors -- 

		''' <summary>
		''' Tests whether the application can read the file denoted by this
		''' abstract pathname. On some platforms it may be possible to start the
		''' Java virtual machine with special privileges that allow it to read
		''' files that are marked as unreadable. Consequently this method may return
		''' {@code true} even though the file does not have read permissions.
		''' </summary>
		''' <returns>  <code>true</code> if and only if the file specified by this
		'''          abstract pathname exists <em>and</em> can be read by the
		'''          application; <code>false</code> otherwise
		''' </returns>
		''' <exception cref="SecurityException">
		'''          If a security manager exists and its <code>{@link
		'''          java.lang.SecurityManager#checkRead(java.lang.String)}</code>
		'''          method denies read access to the file </exception>
		Public Overridable Function canRead() As Boolean
			Dim security As SecurityManager = System.securityManager
			If security IsNot Nothing Then security.checkRead(path)
			If invalid Then Return False
			Return fs.checkAccess(Me, FileSystem.ACCESS_READ)
		End Function

		''' <summary>
		''' Tests whether the application can modify the file denoted by this
		''' abstract pathname. On some platforms it may be possible to start the
		''' Java virtual machine with special privileges that allow it to modify
		''' files that are marked read-only. Consequently this method may return
		''' {@code true} even though the file is marked read-only.
		''' </summary>
		''' <returns>  <code>true</code> if and only if the file system actually
		'''          contains a file denoted by this abstract pathname <em>and</em>
		'''          the application is allowed to write to the file;
		'''          <code>false</code> otherwise.
		''' </returns>
		''' <exception cref="SecurityException">
		'''          If a security manager exists and its <code>{@link
		'''          java.lang.SecurityManager#checkWrite(java.lang.String)}</code>
		'''          method denies write access to the file </exception>
		Public Overridable Function canWrite() As Boolean
			Dim security As SecurityManager = System.securityManager
			If security IsNot Nothing Then security.checkWrite(path)
			If invalid Then Return False
			Return fs.checkAccess(Me, FileSystem.ACCESS_WRITE)
		End Function

		''' <summary>
		''' Tests whether the file or directory denoted by this abstract pathname
		''' exists.
		''' </summary>
		''' <returns>  <code>true</code> if and only if the file or directory denoted
		'''          by this abstract pathname exists; <code>false</code> otherwise
		''' </returns>
		''' <exception cref="SecurityException">
		'''          If a security manager exists and its <code>{@link
		'''          java.lang.SecurityManager#checkRead(java.lang.String)}</code>
		'''          method denies read access to the file or directory </exception>
		Public Overridable Function exists() As Boolean
			Dim security As SecurityManager = System.securityManager
			If security IsNot Nothing Then security.checkRead(path)
			If invalid Then Return False
			Return ((fs.getBooleanAttributes(Me) And FileSystem.BA_EXISTS) <> 0)
		End Function

		''' <summary>
		''' Tests whether the file denoted by this abstract pathname is a
		''' directory.
		''' 
		''' <p> Where it is required to distinguish an I/O exception from the case
		''' that the file is not a directory, or where several attributes of the
		''' same file are required at the same time, then the {@link
		''' java.nio.file.Files#readAttributes(Path,Class,LinkOption[])
		''' Files.readAttributes} method may be used.
		''' </summary>
		''' <returns> <code>true</code> if and only if the file denoted by this
		'''          abstract pathname exists <em>and</em> is a directory;
		'''          <code>false</code> otherwise
		''' </returns>
		''' <exception cref="SecurityException">
		'''          If a security manager exists and its <code>{@link
		'''          java.lang.SecurityManager#checkRead(java.lang.String)}</code>
		'''          method denies read access to the file </exception>
		Public Overridable Property directory As Boolean
			Get
				Dim security As SecurityManager = System.securityManager
				If security IsNot Nothing Then security.checkRead(path)
				If invalid Then Return False
				Return ((fs.getBooleanAttributes(Me) And FileSystem.BA_DIRECTORY) <> 0)
			End Get
		End Property

		''' <summary>
		''' Tests whether the file denoted by this abstract pathname is a normal
		''' file.  A file is <em>normal</em> if it is not a directory and, in
		''' addition, satisfies other system-dependent criteria.  Any non-directory
		''' file created by a Java application is guaranteed to be a normal file.
		''' 
		''' <p> Where it is required to distinguish an I/O exception from the case
		''' that the file is not a normal file, or where several attributes of the
		''' same file are required at the same time, then the {@link
		''' java.nio.file.Files#readAttributes(Path,Class,LinkOption[])
		''' Files.readAttributes} method may be used.
		''' </summary>
		''' <returns>  <code>true</code> if and only if the file denoted by this
		'''          abstract pathname exists <em>and</em> is a normal file;
		'''          <code>false</code> otherwise
		''' </returns>
		''' <exception cref="SecurityException">
		'''          If a security manager exists and its <code>{@link
		'''          java.lang.SecurityManager#checkRead(java.lang.String)}</code>
		'''          method denies read access to the file </exception>
		Public Overridable Property file As Boolean
			Get
				Dim security As SecurityManager = System.securityManager
				If security IsNot Nothing Then security.checkRead(path)
				If invalid Then Return False
				Return ((fs.getBooleanAttributes(Me) And FileSystem.BA_REGULAR) <> 0)
			End Get
		End Property

		''' <summary>
		''' Tests whether the file named by this abstract pathname is a hidden
		''' file.  The exact definition of <em>hidden</em> is system-dependent.  On
		''' UNIX systems, a file is considered to be hidden if its name begins with
		''' a period character (<code>'.'</code>).  On Microsoft Windows systems, a file is
		''' considered to be hidden if it has been marked as such in the filesystem.
		''' </summary>
		''' <returns>  <code>true</code> if and only if the file denoted by this
		'''          abstract pathname is hidden according to the conventions of the
		'''          underlying platform
		''' </returns>
		''' <exception cref="SecurityException">
		'''          If a security manager exists and its <code>{@link
		'''          java.lang.SecurityManager#checkRead(java.lang.String)}</code>
		'''          method denies read access to the file
		''' 
		''' @since 1.2 </exception>
		Public Overridable Property hidden As Boolean
			Get
				Dim security As SecurityManager = System.securityManager
				If security IsNot Nothing Then security.checkRead(path)
				If invalid Then Return False
				Return ((fs.getBooleanAttributes(Me) And FileSystem.BA_HIDDEN) <> 0)
			End Get
		End Property

		''' <summary>
		''' Returns the time that the file denoted by this abstract pathname was
		''' last modified.
		''' 
		''' <p> Where it is required to distinguish an I/O exception from the case
		''' where {@code 0L} is returned, or where several attributes of the
		''' same file are required at the same time, or where the time of last
		''' access or the creation time are required, then the {@link
		''' java.nio.file.Files#readAttributes(Path,Class,LinkOption[])
		''' Files.readAttributes} method may be used.
		''' </summary>
		''' <returns>  A <code>long</code> value representing the time the file was
		'''          last modified, measured in milliseconds since the epoch
		'''          (00:00:00 GMT, January 1, 1970), or <code>0L</code> if the
		'''          file does not exist or if an I/O error occurs
		''' </returns>
		''' <exception cref="SecurityException">
		'''          If a security manager exists and its <code>{@link
		'''          java.lang.SecurityManager#checkRead(java.lang.String)}</code>
		'''          method denies read access to the file </exception>
		Public Overridable Function lastModified() As Long
			Dim security As SecurityManager = System.securityManager
			If security IsNot Nothing Then security.checkRead(path)
			If invalid Then Return 0L
			Return fs.getLastModifiedTime(Me)
		End Function

		''' <summary>
		''' Returns the length of the file denoted by this abstract pathname.
		''' The return value is unspecified if this pathname denotes a directory.
		''' 
		''' <p> Where it is required to distinguish an I/O exception from the case
		''' that {@code 0L} is returned, or where several attributes of the same file
		''' are required at the same time, then the {@link
		''' java.nio.file.Files#readAttributes(Path,Class,LinkOption[])
		''' Files.readAttributes} method may be used.
		''' </summary>
		''' <returns>  The length, in bytes, of the file denoted by this abstract
		'''          pathname, or <code>0L</code> if the file does not exist.  Some
		'''          operating systems may return <code>0L</code> for pathnames
		'''          denoting system-dependent entities such as devices or pipes.
		''' </returns>
		''' <exception cref="SecurityException">
		'''          If a security manager exists and its <code>{@link
		'''          java.lang.SecurityManager#checkRead(java.lang.String)}</code>
		'''          method denies read access to the file </exception>
		Public Overridable Function length() As Long
			Dim security As SecurityManager = System.securityManager
			If security IsNot Nothing Then security.checkRead(path)
			If invalid Then Return 0L
			Return fs.getLength(Me)
		End Function


		' -- File operations -- 

		''' <summary>
		''' Atomically creates a new, empty file named by this abstract pathname if
		''' and only if a file with this name does not yet exist.  The check for the
		''' existence of the file and the creation of the file if it does not exist
		''' are a single operation that is atomic with respect to all other
		''' filesystem activities that might affect the file.
		''' <P>
		''' Note: this method should <i>not</i> be used for file-locking, as
		''' the resulting protocol cannot be made to work reliably. The
		''' <seealso cref="java.nio.channels.FileLock FileLock"/>
		''' facility should be used instead.
		''' </summary>
		''' <returns>  <code>true</code> if the named file does not exist and was
		'''          successfully created; <code>false</code> if the named file
		'''          already exists
		''' </returns>
		''' <exception cref="IOException">
		'''          If an I/O error occurred
		''' </exception>
		''' <exception cref="SecurityException">
		'''          If a security manager exists and its <code>{@link
		'''          java.lang.SecurityManager#checkWrite(java.lang.String)}</code>
		'''          method denies write access to the file
		''' 
		''' @since 1.2 </exception>
		Public Overridable Function createNewFile() As Boolean
			Dim security As SecurityManager = System.securityManager
			If security IsNot Nothing Then security.checkWrite(path)
			If invalid Then Throw New IOException("Invalid file path")
			Return fs.createFileExclusively(path)
		End Function

		''' <summary>
		''' Deletes the file or directory denoted by this abstract pathname.  If
		''' this pathname denotes a directory, then the directory must be empty in
		''' order to be deleted.
		''' 
		''' <p> Note that the <seealso cref="java.nio.file.Files"/> class defines the {@link
		''' java.nio.file.Files#delete(Path) delete} method to throw an <seealso cref="IOException"/>
		''' when a file cannot be deleted. This is useful for error reporting and to
		''' diagnose why a file cannot be deleted.
		''' </summary>
		''' <returns>  <code>true</code> if and only if the file or directory is
		'''          successfully deleted; <code>false</code> otherwise
		''' </returns>
		''' <exception cref="SecurityException">
		'''          If a security manager exists and its <code>{@link
		'''          java.lang.SecurityManager#checkDelete}</code> method denies
		'''          delete access to the file </exception>
		Public Overridable Function delete() As Boolean
			Dim security As SecurityManager = System.securityManager
			If security IsNot Nothing Then security.checkDelete(path)
			If invalid Then Return False
			Return fs.delete(Me)
		End Function

		''' <summary>
		''' Requests that the file or directory denoted by this abstract
		''' pathname be deleted when the virtual machine terminates.
		''' Files (or directories) are deleted in the reverse order that
		''' they are registered. Invoking this method to delete a file or
		''' directory that is already registered for deletion has no effect.
		''' Deletion will be attempted only for normal termination of the
		''' virtual machine, as defined by the Java Language Specification.
		''' 
		''' <p> Once deletion has been requested, it is not possible to cancel the
		''' request.  This method should therefore be used with care.
		''' 
		''' <P>
		''' Note: this method should <i>not</i> be used for file-locking, as
		''' the resulting protocol cannot be made to work reliably. The
		''' <seealso cref="java.nio.channels.FileLock FileLock"/>
		''' facility should be used instead.
		''' </summary>
		''' <exception cref="SecurityException">
		'''          If a security manager exists and its <code>{@link
		'''          java.lang.SecurityManager#checkDelete}</code> method denies
		'''          delete access to the file
		''' </exception>
		''' <seealso cref= #delete
		''' 
		''' @since 1.2 </seealso>
		Public Overridable Sub deleteOnExit()
			Dim security As SecurityManager = System.securityManager
			If security IsNot Nothing Then security.checkDelete(path)
			If invalid Then Return
			DeleteOnExitHook.add(path)
		End Sub

		''' <summary>
		''' Returns an array of strings naming the files and directories in the
		''' directory denoted by this abstract pathname.
		''' 
		''' <p> If this abstract pathname does not denote a directory, then this
		''' method returns {@code null}.  Otherwise an array of strings is
		''' returned, one for each file or directory in the directory.  Names
		''' denoting the directory itself and the directory's parent directory are
		''' not included in the result.  Each string is a file name rather than a
		''' complete path.
		''' 
		''' <p> There is no guarantee that the name strings in the resulting array
		''' will appear in any specific order; they are not, in particular,
		''' guaranteed to appear in alphabetical order.
		''' 
		''' <p> Note that the <seealso cref="java.nio.file.Files"/> class defines the {@link
		''' java.nio.file.Files#newDirectoryStream(Path) newDirectoryStream} method to
		''' open a directory and iterate over the names of the files in the directory.
		''' This may use less resources when working with very large directories, and
		''' may be more responsive when working with remote directories.
		''' </summary>
		''' <returns>  An array of strings naming the files and directories in the
		'''          directory denoted by this abstract pathname.  The array will be
		'''          empty if the directory is empty.  Returns {@code null} if
		'''          this abstract pathname does not denote a directory, or if an
		'''          I/O error occurs.
		''' </returns>
		''' <exception cref="SecurityException">
		'''          If a security manager exists and its {@link
		'''          SecurityManager#checkRead(String)} method denies read access to
		'''          the directory </exception>
		Public Overridable Function list() As String()
			Dim security As SecurityManager = System.securityManager
			If security IsNot Nothing Then security.checkRead(path)
			If invalid Then Return Nothing
			Return fs.list(Me)
		End Function

		''' <summary>
		''' Returns an array of strings naming the files and directories in the
		''' directory denoted by this abstract pathname that satisfy the specified
		''' filter.  The behavior of this method is the same as that of the
		''' <seealso cref="#list()"/> method, except that the strings in the returned array
		''' must satisfy the filter.  If the given {@code filter} is {@code null}
		''' then all names are accepted.  Otherwise, a name satisfies the filter if
		''' and only if the value {@code true} results when the {@link
		''' FilenameFilter#accept FilenameFilter.accept(File,&nbsp;String)} method
		''' of the filter is invoked on this abstract pathname and the name of a
		''' file or directory in the directory that it denotes.
		''' </summary>
		''' <param name="filter">
		'''         A filename filter
		''' </param>
		''' <returns>  An array of strings naming the files and directories in the
		'''          directory denoted by this abstract pathname that were accepted
		'''          by the given {@code filter}.  The array will be empty if the
		'''          directory is empty or if no names were accepted by the filter.
		'''          Returns {@code null} if this abstract pathname does not denote
		'''          a directory, or if an I/O error occurs.
		''' </returns>
		''' <exception cref="SecurityException">
		'''          If a security manager exists and its {@link
		'''          SecurityManager#checkRead(String)} method denies read access to
		'''          the directory
		''' </exception>
		''' <seealso cref= java.nio.file.Files#newDirectoryStream(Path,String) </seealso>
		Public Overridable Function list(ByVal filter As FilenameFilter) As String()
			Dim names As String() = list()
			If (names Is Nothing) OrElse (filter Is Nothing) Then Return names
			Dim v As IList(Of String) = New List(Of String)
			For i As Integer = 0 To names.Length - 1
				If filter.accept(Me, names(i)) Then v.Add(names(i))
			Next i
			Return v.ToArray()
		End Function

		''' <summary>
		''' Returns an array of abstract pathnames denoting the files in the
		''' directory denoted by this abstract pathname.
		''' 
		''' <p> If this abstract pathname does not denote a directory, then this
		''' method returns {@code null}.  Otherwise an array of {@code File} objects
		''' is returned, one for each file or directory in the directory.  Pathnames
		''' denoting the directory itself and the directory's parent directory are
		''' not included in the result.  Each resulting abstract pathname is
		''' constructed from this abstract pathname using the {@link #File(File,
		''' String) File(File,&nbsp;String)} constructor.  Therefore if this
		''' pathname is absolute then each resulting pathname is absolute; if this
		''' pathname is relative then each resulting pathname will be relative to
		''' the same directory.
		''' 
		''' <p> There is no guarantee that the name strings in the resulting array
		''' will appear in any specific order; they are not, in particular,
		''' guaranteed to appear in alphabetical order.
		''' 
		''' <p> Note that the <seealso cref="java.nio.file.Files"/> class defines the {@link
		''' java.nio.file.Files#newDirectoryStream(Path) newDirectoryStream} method
		''' to open a directory and iterate over the names of the files in the
		''' directory. This may use less resources when working with very large
		''' directories.
		''' </summary>
		''' <returns>  An array of abstract pathnames denoting the files and
		'''          directories in the directory denoted by this abstract pathname.
		'''          The array will be empty if the directory is empty.  Returns
		'''          {@code null} if this abstract pathname does not denote a
		'''          directory, or if an I/O error occurs.
		''' </returns>
		''' <exception cref="SecurityException">
		'''          If a security manager exists and its {@link
		'''          SecurityManager#checkRead(String)} method denies read access to
		'''          the directory
		''' 
		''' @since  1.2 </exception>
		Public Overridable Function listFiles() As File()
			Dim ss As String() = list()
			If ss Is Nothing Then Return Nothing
			Dim n As Integer = ss.Length
			Dim fs As File() = New File(n - 1){}
			For i As Integer = 0 To n - 1
				fs(i) = New File(ss(i), Me)
			Next i
			Return fs
		End Function

		''' <summary>
		''' Returns an array of abstract pathnames denoting the files and
		''' directories in the directory denoted by this abstract pathname that
		''' satisfy the specified filter.  The behavior of this method is the same
		''' as that of the <seealso cref="#listFiles()"/> method, except that the pathnames in
		''' the returned array must satisfy the filter.  If the given {@code filter}
		''' is {@code null} then all pathnames are accepted.  Otherwise, a pathname
		''' satisfies the filter if and only if the value {@code true} results when
		''' the {@link FilenameFilter#accept
		''' FilenameFilter.accept(File,&nbsp;String)} method of the filter is
		''' invoked on this abstract pathname and the name of a file or directory in
		''' the directory that it denotes.
		''' </summary>
		''' <param name="filter">
		'''         A filename filter
		''' </param>
		''' <returns>  An array of abstract pathnames denoting the files and
		'''          directories in the directory denoted by this abstract pathname.
		'''          The array will be empty if the directory is empty.  Returns
		'''          {@code null} if this abstract pathname does not denote a
		'''          directory, or if an I/O error occurs.
		''' </returns>
		''' <exception cref="SecurityException">
		'''          If a security manager exists and its {@link
		'''          SecurityManager#checkRead(String)} method denies read access to
		'''          the directory
		''' 
		''' @since  1.2 </exception>
		''' <seealso cref= java.nio.file.Files#newDirectoryStream(Path,String) </seealso>
		Public Overridable Function listFiles(ByVal filter As FilenameFilter) As File()
			Dim ss As String() = list()
			If ss Is Nothing Then Return Nothing
			Dim files As New List(Of File)
			For Each s As String In ss
				If (filter Is Nothing) OrElse filter.accept(Me, s) Then files.Add(New File(s, Me))
			Next s
			Return files.ToArray()
		End Function

		''' <summary>
		''' Returns an array of abstract pathnames denoting the files and
		''' directories in the directory denoted by this abstract pathname that
		''' satisfy the specified filter.  The behavior of this method is the same
		''' as that of the <seealso cref="#listFiles()"/> method, except that the pathnames in
		''' the returned array must satisfy the filter.  If the given {@code filter}
		''' is {@code null} then all pathnames are accepted.  Otherwise, a pathname
		''' satisfies the filter if and only if the value {@code true} results when
		''' the <seealso cref="FileFilter#accept FileFilter.accept(File)"/> method of the
		''' filter is invoked on the pathname.
		''' </summary>
		''' <param name="filter">
		'''         A file filter
		''' </param>
		''' <returns>  An array of abstract pathnames denoting the files and
		'''          directories in the directory denoted by this abstract pathname.
		'''          The array will be empty if the directory is empty.  Returns
		'''          {@code null} if this abstract pathname does not denote a
		'''          directory, or if an I/O error occurs.
		''' </returns>
		''' <exception cref="SecurityException">
		'''          If a security manager exists and its {@link
		'''          SecurityManager#checkRead(String)} method denies read access to
		'''          the directory
		''' 
		''' @since  1.2 </exception>
		''' <seealso cref= java.nio.file.Files#newDirectoryStream(Path,java.nio.file.DirectoryStream.Filter) </seealso>
		Public Overridable Function listFiles(ByVal filter As FileFilter) As File()
			Dim ss As String() = list()
			If ss Is Nothing Then Return Nothing
			Dim files As New List(Of File)
			For Each s As String In ss
				Dim f As New File(s, Me)
				If (filter Is Nothing) OrElse filter.accept(f) Then files.Add(f)
			Next s
			Return files.ToArray()
		End Function

		''' <summary>
		''' Creates the directory named by this abstract pathname.
		''' </summary>
		''' <returns>  <code>true</code> if and only if the directory was
		'''          created; <code>false</code> otherwise
		''' </returns>
		''' <exception cref="SecurityException">
		'''          If a security manager exists and its <code>{@link
		'''          java.lang.SecurityManager#checkWrite(java.lang.String)}</code>
		'''          method does not permit the named directory to be created </exception>
		Public Overridable Function mkdir() As Boolean
			Dim security As SecurityManager = System.securityManager
			If security IsNot Nothing Then security.checkWrite(path)
			If invalid Then Return False
			Return fs.createDirectory(Me)
		End Function

		''' <summary>
		''' Creates the directory named by this abstract pathname, including any
		''' necessary but nonexistent parent directories.  Note that if this
		''' operation fails it may have succeeded in creating some of the necessary
		''' parent directories.
		''' </summary>
		''' <returns>  <code>true</code> if and only if the directory was created,
		'''          along with all necessary parent directories; <code>false</code>
		'''          otherwise
		''' </returns>
		''' <exception cref="SecurityException">
		'''          If a security manager exists and its <code>{@link
		'''          java.lang.SecurityManager#checkRead(java.lang.String)}</code>
		'''          method does not permit verification of the existence of the
		'''          named directory and all necessary parent directories; or if
		'''          the <code>{@link
		'''          java.lang.SecurityManager#checkWrite(java.lang.String)}</code>
		'''          method does not permit the named directory and all necessary
		'''          parent directories to be created </exception>
		Public Overridable Function mkdirs() As Boolean
			If exists() Then Return False
			If mkdir() Then Return True
			Dim canonFile As File = Nothing
			Try
				canonFile = canonicalFile
			Catch e As IOException
				Return False
			End Try

			Dim parent_Renamed As File = canonFile.parentFile
			Return (parent_Renamed IsNot Nothing AndAlso (parent_Renamed.mkdirs() OrElse parent_Renamed.exists()) AndAlso canonFile.mkdir())
		End Function

		''' <summary>
		''' Renames the file denoted by this abstract pathname.
		''' 
		''' <p> Many aspects of the behavior of this method are inherently
		''' platform-dependent: The rename operation might not be able to move a
		''' file from one filesystem to another, it might not be atomic, and it
		''' might not succeed if a file with the destination abstract pathname
		''' already exists.  The return value should always be checked to make sure
		''' that the rename operation was successful.
		''' 
		''' <p> Note that the <seealso cref="java.nio.file.Files"/> class defines the {@link
		''' java.nio.file.Files#move move} method to move or rename a file in a
		''' platform independent manner.
		''' </summary>
		''' <param name="dest">  The new abstract pathname for the named file
		''' </param>
		''' <returns>  <code>true</code> if and only if the renaming succeeded;
		'''          <code>false</code> otherwise
		''' </returns>
		''' <exception cref="SecurityException">
		'''          If a security manager exists and its <code>{@link
		'''          java.lang.SecurityManager#checkWrite(java.lang.String)}</code>
		'''          method denies write access to either the old or new pathnames
		''' </exception>
		''' <exception cref="NullPointerException">
		'''          If parameter <code>dest</code> is <code>null</code> </exception>
		Public Overridable Function renameTo(ByVal dest As File) As Boolean
			Dim security As SecurityManager = System.securityManager
			If security IsNot Nothing Then
				security.checkWrite(path)
				security.checkWrite(dest.path)
			End If
			If dest Is Nothing Then Throw New NullPointerException
			If Me.invalid OrElse dest.invalid Then Return False
			Return fs.rename(Me, dest)
		End Function

		''' <summary>
		''' Sets the last-modified time of the file or directory named by this
		''' abstract pathname.
		''' 
		''' <p> All platforms support file-modification times to the nearest second,
		''' but some provide more precision.  The argument will be truncated to fit
		''' the supported precision.  If the operation succeeds and no intervening
		''' operations on the file take place, then the next invocation of the
		''' <code><seealso cref="#lastModified"/></code> method will return the (possibly
		''' truncated) <code>time</code> argument that was passed to this method.
		''' </summary>
		''' <param name="time">  The new last-modified time, measured in milliseconds since
		'''               the epoch (00:00:00 GMT, January 1, 1970)
		''' </param>
		''' <returns> <code>true</code> if and only if the operation succeeded;
		'''          <code>false</code> otherwise
		''' </returns>
		''' <exception cref="IllegalArgumentException">  If the argument is negative
		''' </exception>
		''' <exception cref="SecurityException">
		'''          If a security manager exists and its <code>{@link
		'''          java.lang.SecurityManager#checkWrite(java.lang.String)}</code>
		'''          method denies write access to the named file
		''' 
		''' @since 1.2 </exception>
		Public Overridable Function setLastModified(ByVal time As Long) As Boolean
			If time < 0 Then Throw New IllegalArgumentException("Negative time")
			Dim security As SecurityManager = System.securityManager
			If security IsNot Nothing Then security.checkWrite(path)
			If invalid Then Return False
			Return fs.lastModifiedTimeime(Me, time)
		End Function

		''' <summary>
		''' Marks the file or directory named by this abstract pathname so that
		''' only read operations are allowed. After invoking this method the file
		''' or directory will not change until it is either deleted or marked
		''' to allow write access. On some platforms it may be possible to start the
		''' Java virtual machine with special privileges that allow it to modify
		''' files that are marked read-only. Whether or not a read-only file or
		''' directory may be deleted depends upon the underlying system.
		''' </summary>
		''' <returns> <code>true</code> if and only if the operation succeeded;
		'''          <code>false</code> otherwise
		''' </returns>
		''' <exception cref="SecurityException">
		'''          If a security manager exists and its <code>{@link
		'''          java.lang.SecurityManager#checkWrite(java.lang.String)}</code>
		'''          method denies write access to the named file
		''' 
		''' @since 1.2 </exception>
		Public Overridable Function setReadOnly() As Boolean
			Dim security As SecurityManager = System.securityManager
			If security IsNot Nothing Then security.checkWrite(path)
			If invalid Then Return False
			Return fs.readOnlynly(Me)
		End Function

		''' <summary>
		''' Sets the owner's or everybody's write permission for this abstract
		''' pathname. On some platforms it may be possible to start the Java virtual
		''' machine with special privileges that allow it to modify files that
		''' disallow write operations.
		''' 
		''' <p> The <seealso cref="java.nio.file.Files"/> class defines methods that operate on
		''' file attributes including file permissions. This may be used when finer
		''' manipulation of file permissions is required.
		''' </summary>
		''' <param name="writable">
		'''          If <code>true</code>, sets the access permission to allow write
		'''          operations; if <code>false</code> to disallow write operations
		''' </param>
		''' <param name="ownerOnly">
		'''          If <code>true</code>, the write permission applies only to the
		'''          owner's write permission; otherwise, it applies to everybody.  If
		'''          the underlying file system can not distinguish the owner's write
		'''          permission from that of others, then the permission will apply to
		'''          everybody, regardless of this value.
		''' </param>
		''' <returns>  <code>true</code> if and only if the operation succeeded. The
		'''          operation will fail if the user does not have permission to change
		'''          the access permissions of this abstract pathname.
		''' </returns>
		''' <exception cref="SecurityException">
		'''          If a security manager exists and its <code>{@link
		'''          java.lang.SecurityManager#checkWrite(java.lang.String)}</code>
		'''          method denies write access to the named file
		''' 
		''' @since 1.6 </exception>
		Public Overridable Function setWritable(ByVal writable As Boolean, ByVal ownerOnly As Boolean) As Boolean
			Dim security As SecurityManager = System.securityManager
			If security IsNot Nothing Then security.checkWrite(path)
			If invalid Then Return False
			Return fs.permissionion(Me, FileSystem.ACCESS_WRITE, writable, ownerOnly)
		End Function

		''' <summary>
		''' A convenience method to set the owner's write permission for this abstract
		''' pathname. On some platforms it may be possible to start the Java virtual
		''' machine with special privileges that allow it to modify files that
		''' disallow write operations.
		''' 
		''' <p> An invocation of this method of the form <tt>file.setWritable(arg)</tt>
		''' behaves in exactly the same way as the invocation
		''' 
		''' <pre>
		'''     file.setWritable(arg, true) </pre>
		''' </summary>
		''' <param name="writable">
		'''          If <code>true</code>, sets the access permission to allow write
		'''          operations; if <code>false</code> to disallow write operations
		''' </param>
		''' <returns>  <code>true</code> if and only if the operation succeeded.  The
		'''          operation will fail if the user does not have permission to
		'''          change the access permissions of this abstract pathname.
		''' </returns>
		''' <exception cref="SecurityException">
		'''          If a security manager exists and its <code>{@link
		'''          java.lang.SecurityManager#checkWrite(java.lang.String)}</code>
		'''          method denies write access to the file
		''' 
		''' @since 1.6 </exception>
		Public Overridable Function setWritable(ByVal writable As Boolean) As Boolean
			Return writableble(writable, True)
		End Function

		''' <summary>
		''' Sets the owner's or everybody's read permission for this abstract
		''' pathname. On some platforms it may be possible to start the Java virtual
		''' machine with special privileges that allow it to read files that are
		''' marked as unreadable.
		''' 
		''' <p> The <seealso cref="java.nio.file.Files"/> class defines methods that operate on
		''' file attributes including file permissions. This may be used when finer
		''' manipulation of file permissions is required.
		''' </summary>
		''' <param name="readable">
		'''          If <code>true</code>, sets the access permission to allow read
		'''          operations; if <code>false</code> to disallow read operations
		''' </param>
		''' <param name="ownerOnly">
		'''          If <code>true</code>, the read permission applies only to the
		'''          owner's read permission; otherwise, it applies to everybody.  If
		'''          the underlying file system can not distinguish the owner's read
		'''          permission from that of others, then the permission will apply to
		'''          everybody, regardless of this value.
		''' </param>
		''' <returns>  <code>true</code> if and only if the operation succeeded.  The
		'''          operation will fail if the user does not have permission to
		'''          change the access permissions of this abstract pathname.  If
		'''          <code>readable</code> is <code>false</code> and the underlying
		'''          file system does not implement a read permission, then the
		'''          operation will fail.
		''' </returns>
		''' <exception cref="SecurityException">
		'''          If a security manager exists and its <code>{@link
		'''          java.lang.SecurityManager#checkWrite(java.lang.String)}</code>
		'''          method denies write access to the file
		''' 
		''' @since 1.6 </exception>
		Public Overridable Function setReadable(ByVal readable As Boolean, ByVal ownerOnly As Boolean) As Boolean
			Dim security As SecurityManager = System.securityManager
			If security IsNot Nothing Then security.checkWrite(path)
			If invalid Then Return False
			Return fs.permissionion(Me, FileSystem.ACCESS_READ, readable, ownerOnly)
		End Function

		''' <summary>
		''' A convenience method to set the owner's read permission for this abstract
		''' pathname. On some platforms it may be possible to start the Java virtual
		''' machine with special privileges that allow it to read files that that are
		''' marked as unreadable.
		''' 
		''' <p>An invocation of this method of the form <tt>file.setReadable(arg)</tt>
		''' behaves in exactly the same way as the invocation
		''' 
		''' <pre>
		'''     file.setReadable(arg, true) </pre>
		''' </summary>
		''' <param name="readable">
		'''          If <code>true</code>, sets the access permission to allow read
		'''          operations; if <code>false</code> to disallow read operations
		''' </param>
		''' <returns>  <code>true</code> if and only if the operation succeeded.  The
		'''          operation will fail if the user does not have permission to
		'''          change the access permissions of this abstract pathname.  If
		'''          <code>readable</code> is <code>false</code> and the underlying
		'''          file system does not implement a read permission, then the
		'''          operation will fail.
		''' </returns>
		''' <exception cref="SecurityException">
		'''          If a security manager exists and its <code>{@link
		'''          java.lang.SecurityManager#checkWrite(java.lang.String)}</code>
		'''          method denies write access to the file
		''' 
		''' @since 1.6 </exception>
		Public Overridable Function setReadable(ByVal readable As Boolean) As Boolean
			Return readableble(readable, True)
		End Function

		''' <summary>
		''' Sets the owner's or everybody's execute permission for this abstract
		''' pathname. On some platforms it may be possible to start the Java virtual
		''' machine with special privileges that allow it to execute files that are
		''' not marked executable.
		''' 
		''' <p> The <seealso cref="java.nio.file.Files"/> class defines methods that operate on
		''' file attributes including file permissions. This may be used when finer
		''' manipulation of file permissions is required.
		''' </summary>
		''' <param name="executable">
		'''          If <code>true</code>, sets the access permission to allow execute
		'''          operations; if <code>false</code> to disallow execute operations
		''' </param>
		''' <param name="ownerOnly">
		'''          If <code>true</code>, the execute permission applies only to the
		'''          owner's execute permission; otherwise, it applies to everybody.
		'''          If the underlying file system can not distinguish the owner's
		'''          execute permission from that of others, then the permission will
		'''          apply to everybody, regardless of this value.
		''' </param>
		''' <returns>  <code>true</code> if and only if the operation succeeded.  The
		'''          operation will fail if the user does not have permission to
		'''          change the access permissions of this abstract pathname.  If
		'''          <code>executable</code> is <code>false</code> and the underlying
		'''          file system does not implement an execute permission, then the
		'''          operation will fail.
		''' </returns>
		''' <exception cref="SecurityException">
		'''          If a security manager exists and its <code>{@link
		'''          java.lang.SecurityManager#checkWrite(java.lang.String)}</code>
		'''          method denies write access to the file
		''' 
		''' @since 1.6 </exception>
		Public Overridable Function setExecutable(ByVal executable As Boolean, ByVal ownerOnly As Boolean) As Boolean
			Dim security As SecurityManager = System.securityManager
			If security IsNot Nothing Then security.checkWrite(path)
			If invalid Then Return False
			Return fs.permissionion(Me, FileSystem.ACCESS_EXECUTE, executable, ownerOnly)
		End Function

		''' <summary>
		''' A convenience method to set the owner's execute permission for this
		''' abstract pathname. On some platforms it may be possible to start the Java
		''' virtual machine with special privileges that allow it to execute files
		''' that are not marked executable.
		''' 
		''' <p>An invocation of this method of the form <tt>file.setExcutable(arg)</tt>
		''' behaves in exactly the same way as the invocation
		''' 
		''' <pre>
		'''     file.setExecutable(arg, true) </pre>
		''' </summary>
		''' <param name="executable">
		'''          If <code>true</code>, sets the access permission to allow execute
		'''          operations; if <code>false</code> to disallow execute operations
		''' </param>
		''' <returns>   <code>true</code> if and only if the operation succeeded.  The
		'''           operation will fail if the user does not have permission to
		'''           change the access permissions of this abstract pathname.  If
		'''           <code>executable</code> is <code>false</code> and the underlying
		'''           file system does not implement an execute permission, then the
		'''           operation will fail.
		''' </returns>
		''' <exception cref="SecurityException">
		'''          If a security manager exists and its <code>{@link
		'''          java.lang.SecurityManager#checkWrite(java.lang.String)}</code>
		'''          method denies write access to the file
		''' 
		''' @since 1.6 </exception>
		Public Overridable Function setExecutable(ByVal executable As Boolean) As Boolean
			Return executableble(executable, True)
		End Function

		''' <summary>
		''' Tests whether the application can execute the file denoted by this
		''' abstract pathname. On some platforms it may be possible to start the
		''' Java virtual machine with special privileges that allow it to execute
		''' files that are not marked executable. Consequently this method may return
		''' {@code true} even though the file does not have execute permissions.
		''' </summary>
		''' <returns>  <code>true</code> if and only if the abstract pathname exists
		'''          <em>and</em> the application is allowed to execute the file
		''' </returns>
		''' <exception cref="SecurityException">
		'''          If a security manager exists and its <code>{@link
		'''          java.lang.SecurityManager#checkExec(java.lang.String)}</code>
		'''          method denies execute access to the file
		''' 
		''' @since 1.6 </exception>
		Public Overridable Function canExecute() As Boolean
			Dim security As SecurityManager = System.securityManager
			If security IsNot Nothing Then security.checkExec(path)
			If invalid Then Return False
			Return fs.checkAccess(Me, FileSystem.ACCESS_EXECUTE)
		End Function


		' -- Filesystem interface -- 

		''' <summary>
		''' List the available filesystem roots.
		''' 
		''' <p> A particular Java platform may support zero or more
		''' hierarchically-organized file systems.  Each file system has a
		''' {@code root} directory from which all other files in that file system
		''' can be reached.  Windows platforms, for example, have a root directory
		''' for each active drive; UNIX platforms have a single root directory,
		''' namely {@code "/"}.  The set of available filesystem roots is affected
		''' by various system-level operations such as the insertion or ejection of
		''' removable media and the disconnecting or unmounting of physical or
		''' virtual disk drives.
		''' 
		''' <p> This method returns an array of {@code File} objects that denote the
		''' root directories of the available filesystem roots.  It is guaranteed
		''' that the canonical pathname of any file physically present on the local
		''' machine will begin with one of the roots returned by this method.
		''' 
		''' <p> The canonical pathname of a file that resides on some other machine
		''' and is accessed via a remote-filesystem protocol such as SMB or NFS may
		''' or may not begin with one of the roots returned by this method.  If the
		''' pathname of a remote file is syntactically indistinguishable from the
		''' pathname of a local file then it will begin with one of the roots
		''' returned by this method.  Thus, for example, {@code File} objects
		''' denoting the root directories of the mapped network drives of a Windows
		''' platform will be returned by this method, while {@code File} objects
		''' containing UNC pathnames will not be returned by this method.
		''' 
		''' <p> Unlike most methods in this [Class], this method does not throw
		''' security exceptions.  If a security manager exists and its {@link
		''' SecurityManager#checkRead(String)} method denies read access to a
		''' particular root directory, then that directory will not appear in the
		''' result.
		''' </summary>
		''' <returns>  An array of {@code File} objects denoting the available
		'''          filesystem roots, or {@code null} if the set of roots could not
		'''          be determined.  The array will be empty if there are no
		'''          filesystem roots.
		''' 
		''' @since  1.2 </returns>
		''' <seealso cref= java.nio.file.FileStore </seealso>
		Public Shared Function listRoots() As File()
			Return fs.listRoots()
		End Function


		' -- Disk usage -- 

		''' <summary>
		''' Returns the size of the partition <a href="#partName">named</a> by this
		''' abstract pathname.
		''' </summary>
		''' <returns>  The size, in bytes, of the partition or <tt>0L</tt> if this
		'''          abstract pathname does not name a partition
		''' </returns>
		''' <exception cref="SecurityException">
		'''          If a security manager has been installed and it denies
		'''          <seealso cref="RuntimePermission"/><tt>("getFileSystemAttributes")</tt>
		'''          or its <seealso cref="SecurityManager#checkRead(String)"/> method denies
		'''          read access to the file named by this abstract pathname
		''' 
		''' @since  1.6 </exception>
		Public Overridable Property totalSpace As Long
			Get
				Dim sm As SecurityManager = System.securityManager
				If sm IsNot Nothing Then
					sm.checkPermission(New RuntimePermission("getFileSystemAttributes"))
					sm.checkRead(path)
				End If
				If invalid Then Return 0L
				Return fs.getSpace(Me, FileSystem.SPACE_TOTAL)
			End Get
		End Property

		''' <summary>
		''' Returns the number of unallocated bytes in the partition <a
		''' href="#partName">named</a> by this abstract path name.
		''' 
		''' <p> The returned number of unallocated bytes is a hint, but not
		''' a guarantee, that it is possible to use most or any of these
		''' bytes.  The number of unallocated bytes is most likely to be
		''' accurate immediately after this call.  It is likely to be made
		''' inaccurate by any external I/O operations including those made
		''' on the system outside of this virtual machine.  This method
		''' makes no guarantee that write operations to this file system
		''' will succeed.
		''' </summary>
		''' <returns>  The number of unallocated bytes on the partition or <tt>0L</tt>
		'''          if the abstract pathname does not name a partition.  This
		'''          value will be less than or equal to the total file system size
		'''          returned by <seealso cref="#getTotalSpace"/>.
		''' </returns>
		''' <exception cref="SecurityException">
		'''          If a security manager has been installed and it denies
		'''          <seealso cref="RuntimePermission"/><tt>("getFileSystemAttributes")</tt>
		'''          or its <seealso cref="SecurityManager#checkRead(String)"/> method denies
		'''          read access to the file named by this abstract pathname
		''' 
		''' @since  1.6 </exception>
		Public Overridable Property freeSpace As Long
			Get
				Dim sm As SecurityManager = System.securityManager
				If sm IsNot Nothing Then
					sm.checkPermission(New RuntimePermission("getFileSystemAttributes"))
					sm.checkRead(path)
				End If
				If invalid Then Return 0L
				Return fs.getSpace(Me, FileSystem.SPACE_FREE)
			End Get
		End Property

		''' <summary>
		''' Returns the number of bytes available to this virtual machine on the
		''' partition <a href="#partName">named</a> by this abstract pathname.  When
		''' possible, this method checks for write permissions and other operating
		''' system restrictions and will therefore usually provide a more accurate
		''' estimate of how much new data can actually be written than {@link
		''' #getFreeSpace}.
		''' 
		''' <p> The returned number of available bytes is a hint, but not a
		''' guarantee, that it is possible to use most or any of these bytes.  The
		''' number of unallocated bytes is most likely to be accurate immediately
		''' after this call.  It is likely to be made inaccurate by any external
		''' I/O operations including those made on the system outside of this
		''' virtual machine.  This method makes no guarantee that write operations
		''' to this file system will succeed.
		''' </summary>
		''' <returns>  The number of available bytes on the partition or <tt>0L</tt>
		'''          if the abstract pathname does not name a partition.  On
		'''          systems where this information is not available, this method
		'''          will be equivalent to a call to <seealso cref="#getFreeSpace"/>.
		''' </returns>
		''' <exception cref="SecurityException">
		'''          If a security manager has been installed and it denies
		'''          <seealso cref="RuntimePermission"/><tt>("getFileSystemAttributes")</tt>
		'''          or its <seealso cref="SecurityManager#checkRead(String)"/> method denies
		'''          read access to the file named by this abstract pathname
		''' 
		''' @since  1.6 </exception>
		Public Overridable Property usableSpace As Long
			Get
				Dim sm As SecurityManager = System.securityManager
				If sm IsNot Nothing Then
					sm.checkPermission(New RuntimePermission("getFileSystemAttributes"))
					sm.checkRead(path)
				End If
				If invalid Then Return 0L
				Return fs.getSpace(Me, FileSystem.SPACE_USABLE)
			End Get
		End Property

		' -- Temporary files -- 

		Private Class TempDirectory
			Private Sub New()
			End Sub

			' temporary directory location
			Private Shared ReadOnly tmpdir As New File(java.security.AccessController.doPrivileged(New sun.security.action.GetPropertyAction("java.io.tmpdir")))
			Friend Shared Function location() As File
				Return tmpdir
			End Function

			' file name generation
			Private Shared ReadOnly random As New java.security.SecureRandom
			Friend Shared Function generateFile(ByVal prefix As String, ByVal suffix As String, ByVal dir As File) As File
				Dim n As Long = random.nextLong()
				If n = java.lang.[Long].MIN_VALUE Then
					n = 0 ' corner case
				Else
					n = System.Math.Abs(n)
				End If

				' Use only the file name from the supplied prefix
				prefix = (New System.IO.FileInfo(prefix)).Name

				Dim name As String = prefix + Convert.ToString(n) + suffix
				Dim f As New File(dir, name)
				If (Not name.Equals(f.name)) OrElse f.invalid Then
					If System.securityManager IsNot Nothing Then
						Throw New IOException("Unable to create temporary file")
					Else
						Throw New IOException("Unable to create temporary file, " & f)
					End If
				End If
				Return f
			End Function
		End Class

		''' <summary>
		''' <p> Creates a new empty file in the specified directory, using the
		''' given prefix and suffix strings to generate its name.  If this method
		''' returns successfully then it is guaranteed that:
		''' 
		''' <ol>
		''' <li> The file denoted by the returned abstract pathname did not exist
		'''      before this method was invoked, and
		''' <li> Neither this method nor any of its variants will return the same
		'''      abstract pathname again in the current invocation of the virtual
		'''      machine.
		''' </ol>
		''' 
		''' This method provides only part of a temporary-file facility.  To arrange
		''' for a file created by this method to be deleted automatically, use the
		''' <code><seealso cref="#deleteOnExit"/></code> method.
		''' 
		''' <p> The <code>prefix</code> argument must be at least three characters
		''' java.lang.[Long].  It is recommended that the prefix be a short, meaningful string
		''' such as <code>"hjb"</code> or <code>"mail"</code>.  The
		''' <code>suffix</code> argument may be <code>null</code>, in which case the
		''' suffix <code>".tmp"</code> will be used.
		''' 
		''' <p> To create the new file, the prefix and the suffix may first be
		''' adjusted to fit the limitations of the underlying platform.  If the
		''' prefix is too long then it will be truncated, but its first three
		''' characters will always be preserved.  If the suffix is too long then it
		''' too will be truncated, but if it begins with a period character
		''' (<code>'.'</code>) then the period and the first three characters
		''' following it will always be preserved.  Once these adjustments have been
		''' made the name of the new file will be generated by concatenating the
		''' prefix, five or more internally-generated characters, and the suffix.
		''' 
		''' <p> If the <code>directory</code> argument is <code>null</code> then the
		''' system-dependent default temporary-file directory will be used.  The
		''' default temporary-file directory is specified by the system property
		''' <code>java.io.tmpdir</code>.  On UNIX systems the default value of this
		''' property is typically <code>"/tmp"</code> or <code>"/var/tmp"</code>; on
		''' Microsoft Windows systems it is typically <code>"C:\\WINNT\\TEMP"</code>.  A different
		''' value may be given to this system property when the Java virtual machine
		''' is invoked, but programmatic changes to this property are not guaranteed
		''' to have any effect upon the temporary directory used by this method.
		''' </summary>
		''' <param name="prefix">     The prefix string to be used in generating the file's
		'''                    name; must be at least three characters long
		''' </param>
		''' <param name="suffix">     The suffix string to be used in generating the file's
		'''                    name; may be <code>null</code>, in which case the
		'''                    suffix <code>".tmp"</code> will be used
		''' </param>
		''' <param name="directory">  The directory in which the file is to be created, or
		'''                    <code>null</code> if the default temporary-file
		'''                    directory is to be used
		''' </param>
		''' <returns>  An abstract pathname denoting a newly-created empty file
		''' </returns>
		''' <exception cref="IllegalArgumentException">
		'''          If the <code>prefix</code> argument contains fewer than three
		'''          characters
		''' </exception>
		''' <exception cref="IOException">  If a file could not be created
		''' </exception>
		''' <exception cref="SecurityException">
		'''          If a security manager exists and its <code>{@link
		'''          java.lang.SecurityManager#checkWrite(java.lang.String)}</code>
		'''          method does not allow a file to be created
		''' 
		''' @since 1.2 </exception>
		Public Shared Function createTempFile(ByVal prefix As String, ByVal suffix As String, ByVal directory As File) As File
			If prefix.length() < 3 Then Throw New IllegalArgumentException("Prefix string too short")
			If suffix Is Nothing Then suffix = ".tmp"

			Dim tmpdir As File = If(directory IsNot Nothing, directory, TempDirectory.location())
			Dim sm As SecurityManager = System.securityManager
			Dim f As File
			Do
				f = TempDirectory.generateFile(prefix, suffix, tmpdir)

				If sm IsNot Nothing Then
					Try
						sm.checkWrite(f.path)
					Catch se As SecurityException
						' don't reveal temporary directory location
						If directory Is Nothing Then Throw New SecurityException("Unable to create temporary file")
						Throw se
					End Try
				End If
			Loop While (fs.getBooleanAttributes(f) And FileSystem.BA_EXISTS) <> 0

			If Not fs.createFileExclusively(f.path) Then Throw New IOException("Unable to create temporary file")

			Return f
		End Function

		''' <summary>
		''' Creates an empty file in the default temporary-file directory, using
		''' the given prefix and suffix to generate its name. Invoking this method
		''' is equivalent to invoking <code>{@link #createTempFile(java.lang.String,
		''' java.lang.String, java.io.File)
		''' createTempFile(prefix,&nbsp;suffix,&nbsp;null)}</code>.
		''' 
		''' <p> The {@link
		''' java.nio.file.Files#createTempFile(String,String,java.nio.file.attribute.FileAttribute[])
		''' Files.createTempFile} method provides an alternative method to create an
		''' empty file in the temporary-file directory. Files created by that method
		''' may have more restrictive access permissions to files created by this
		''' method and so may be more suited to security-sensitive applications.
		''' </summary>
		''' <param name="prefix">     The prefix string to be used in generating the file's
		'''                    name; must be at least three characters long
		''' </param>
		''' <param name="suffix">     The suffix string to be used in generating the file's
		'''                    name; may be <code>null</code>, in which case the
		'''                    suffix <code>".tmp"</code> will be used
		''' </param>
		''' <returns>  An abstract pathname denoting a newly-created empty file
		''' </returns>
		''' <exception cref="IllegalArgumentException">
		'''          If the <code>prefix</code> argument contains fewer than three
		'''          characters
		''' </exception>
		''' <exception cref="IOException">  If a file could not be created
		''' </exception>
		''' <exception cref="SecurityException">
		'''          If a security manager exists and its <code>{@link
		'''          java.lang.SecurityManager#checkWrite(java.lang.String)}</code>
		'''          method does not allow a file to be created
		''' 
		''' @since 1.2 </exception>
		''' <seealso cref= java.nio.file.Files#createTempDirectory(String,FileAttribute[]) </seealso>
		Public Shared Function createTempFile(ByVal prefix As String, ByVal suffix As String) As File
			Return createTempFile(prefix, suffix, Nothing)
		End Function

		' -- Basic infrastructure -- 

		''' <summary>
		''' Compares two abstract pathnames lexicographically.  The ordering
		''' defined by this method depends upon the underlying system.  On UNIX
		''' systems, alphabetic case is significant in comparing pathnames; on Microsoft Windows
		''' systems it is not.
		''' </summary>
		''' <param name="pathname">  The abstract pathname to be compared to this abstract
		'''                    pathname
		''' </param>
		''' <returns>  Zero if the argument is equal to this abstract pathname, a
		'''          value less than zero if this abstract pathname is
		'''          lexicographically less than the argument, or a value greater
		'''          than zero if this abstract pathname is lexicographically
		'''          greater than the argument
		''' 
		''' @since   1.2 </returns>
		Public Overridable Function compareTo(ByVal pathname As File) As Integer Implements Comparable(Of File).compareTo
			Return fs.Compare(Me, pathname)
		End Function

		''' <summary>
		''' Tests this abstract pathname for equality with the given object.
		''' Returns <code>true</code> if and only if the argument is not
		''' <code>null</code> and is an abstract pathname that denotes the same file
		''' or directory as this abstract pathname.  Whether or not two abstract
		''' pathnames are equal depends upon the underlying system.  On UNIX
		''' systems, alphabetic case is significant in comparing pathnames; on Microsoft Windows
		''' systems it is not.
		''' </summary>
		''' <param name="obj">   The object to be compared with this abstract pathname
		''' </param>
		''' <returns>  <code>true</code> if and only if the objects are the same;
		'''          <code>false</code> otherwise </returns>
		Public Overrides Function Equals(ByVal obj As Object) As Boolean
			If (obj IsNot Nothing) AndAlso (TypeOf obj Is File) Then Return compareTo(CType(obj, File)) = 0
			Return False
		End Function

		''' <summary>
		''' Computes a hash code for this abstract pathname.  Because equality of
		''' abstract pathnames is inherently system-dependent, so is the computation
		''' of their hash codes.  On UNIX systems, the hash code of an abstract
		''' pathname is equal to the exclusive <em>or</em> of the hash code
		''' of its pathname string and the decimal value
		''' <code>1234321</code>.  On Microsoft Windows systems, the hash
		''' code is equal to the exclusive <em>or</em> of the hash code of
		''' its pathname string converted to lower case and the decimal
		''' value <code>1234321</code>.  Locale is not taken into account on
		''' lowercasing the pathname string.
		''' </summary>
		''' <returns>  A hash code for this abstract pathname </returns>
		Public Overrides Function GetHashCode() As Integer
			Return fs.hashCode(Me)
		End Function

		''' <summary>
		''' Returns the pathname string of this abstract pathname.  This is just the
		''' string returned by the <code><seealso cref="#getPath"/></code> method.
		''' </summary>
		''' <returns>  The string form of this abstract pathname </returns>
		Public Overrides Function ToString() As String
			Return path
		End Function

		''' <summary>
		''' WriteObject is called to save this filename.
		''' The separator character is saved also so it can be replaced
		''' in case the path is reconstituted on a different host type.
		''' <p>
		''' @serialData  Default fields followed by separator character.
		''' </summary>
		<MethodImpl(MethodImplOptions.Synchronized)> _
		Private Sub writeObject(ByVal s As java.io.ObjectOutputStream)
			s.defaultWriteObject()
			s.writeChar(separatorChar) ' Add the separator character
		End Sub

		''' <summary>
		''' readObject is called to restore this filename.
		''' The original separator character is read.  If it is different
		''' than the separator character on this system, then the old separator
		''' is replaced by the local separator.
		''' </summary>
		<MethodImpl(MethodImplOptions.Synchronized)> _
		Private Sub readObject(ByVal s As java.io.ObjectInputStream)
			Dim fields As ObjectInputStream.GetField = s.readFields()
			Dim pathField As String = CStr(fields.get("path", Nothing))
			Dim sep As Char = s.readChar() ' read the previous separator char
			If sep <> separatorChar Then pathField = pathField.replace(sep, separatorChar)
			Dim path_Renamed As String = fs.normalize(pathField)
			UNSAFE.putObject(Me, PATH_OFFSET, path_Renamed)
			UNSAFE.putIntVolatile(Me, PREFIX_LENGTH_OFFSET, fs.prefixLength(path_Renamed))
		End Sub

		Private Shared ReadOnly PATH_OFFSET As Long
		Private Shared ReadOnly PREFIX_LENGTH_OFFSET As Long
		Private Shared ReadOnly UNSAFE As sun.misc.Unsafe
		Shared Sub New()
			Try
				Dim unsafe_Renamed As sun.misc.Unsafe = sun.misc.Unsafe.unsafe
				PATH_OFFSET = unsafe_Renamed.objectFieldOffset(GetType(File).getDeclaredField("path"))
				PREFIX_LENGTH_OFFSET = unsafe_Renamed.objectFieldOffset(GetType(File).getDeclaredField("prefixLength"))
				UNSAFE = unsafe_Renamed
			Catch e As ReflectiveOperationException
				Throw New [Error](e)
			End Try
		End Sub


		''' <summary>
		''' use serialVersionUID from JDK 1.0.2 for interoperability </summary>
		Private Const serialVersionUID As Long = 301077366599181567L

		' -- Integration with java.nio.file --

'JAVA TO VB CONVERTER TODO TASK: There is no VB equivalent to 'volatile':
		<NonSerialized> _
		Private filePath As java.nio.file.Path

		''' <summary>
		''' Returns a <seealso cref="Path java.nio.file.Path"/> object constructed from the
		''' this abstract path. The resulting {@code Path} is associated with the
		''' <seealso cref="java.nio.file.FileSystems#getDefault default-filesystem"/>.
		''' 
		''' <p> The first invocation of this method works as if invoking it were
		''' equivalent to evaluating the expression:
		''' <blockquote><pre>
		''' <seealso cref="java.nio.file.FileSystems#getDefault FileSystems.getDefault"/>().{@link
		''' java.nio.file.FileSystem#getPath getPath}(this.<seealso cref="#getPath getPath"/>());
		''' </pre></blockquote>
		''' Subsequent invocations of this method return the same {@code Path}.
		''' 
		''' <p> If this abstract pathname is the empty abstract pathname then this
		''' method returns a {@code Path} that may be used to access the current
		''' user directory.
		''' </summary>
		''' <returns>  a {@code Path} constructed from this abstract path
		''' </returns>
		''' <exception cref="java.nio.file.InvalidPathException">
		'''          if a {@code Path} object cannot be constructed from the abstract
		'''          path (see <seealso cref="java.nio.file.FileSystem#getPath FileSystem.getPath"/>)
		''' 
		''' @since   1.7 </exception>
		''' <seealso cref= Path#toFile </seealso>
		Public Overridable Function toPath() As java.nio.file.Path
			Dim result As java.nio.file.Path = filePath
			If result Is Nothing Then
				SyncLock Me
					result = filePath
					If result Is Nothing Then
						result = java.nio.file.FileSystems.default.getPath(path)
						filePath = result
					End If
				End SyncLock
			End If
			Return result
		End Function
	End Class

End Namespace