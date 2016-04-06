'
' * Copyright (c) 1998, 2013, Oracle and/or its affiliates. All rights reserved.
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
	''' Package-private abstract class for the local filesystem abstraction.
	''' </summary>

	Friend MustInherit Class FileSystem

		' -- Normalization and construction -- 

		''' <summary>
		''' Return the local filesystem's name-separator character.
		''' </summary>
		Public MustOverride ReadOnly Property separator As Char

		''' <summary>
		''' Return the local filesystem's path-separator character.
		''' </summary>
		Public MustOverride ReadOnly Property pathSeparator As Char

		''' <summary>
		''' Convert the given pathname string to normal form.  If the string is
		''' already in normal form then it is simply returned.
		''' </summary>
		Public MustOverride Function normalize(  path As String) As String

		''' <summary>
		''' Compute the length of this pathname string's prefix.  The pathname
		''' string must be in normal form.
		''' </summary>
		Public MustOverride Function prefixLength(  path As String) As Integer

		''' <summary>
		''' Resolve the child pathname string against the parent.
		''' Both strings must be in normal form, and the result
		''' will be in normal form.
		''' </summary>
		Public MustOverride Function resolve(  parent As String,   child As String) As String

		''' <summary>
		''' Return the parent pathname string to be used when the parent-directory
		''' argument in one of the two-argument File constructors is the empty
		''' pathname.
		''' </summary>
		Public MustOverride ReadOnly Property defaultParent As String

		''' <summary>
		''' Post-process the given URI path string if necessary.  This is used on
		''' win32, e.g., to transform "/c:/foo" into "c:/foo".  The path string
		''' still has slash separators; code in the File class will translate them
		''' after this method returns.
		''' </summary>
		Public MustOverride Function fromURIPath(  path As String) As String


		' -- Path operations -- 

		''' <summary>
		''' Tell whether or not the given abstract pathname is absolute.
		''' </summary>
		Public MustOverride Function isAbsolute(  f As File) As Boolean

		''' <summary>
		''' Resolve the given abstract pathname into absolute form.  Invoked by the
		''' getAbsolutePath and getCanonicalPath methods in the File class.
		''' </summary>
		Public MustOverride Function resolve(  f As File) As String

		Public MustOverride Function canonicalize(  path As String) As String


		' -- Attribute accessors -- 

		' Constants for simple boolean attributes 
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
		Public Const BA_EXISTS As Integer = &H1
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
		Public Const BA_REGULAR As Integer = &H2
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
		Public Const BA_DIRECTORY As Integer = &H4
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
		Public Const BA_HIDDEN As Integer = &H8

		''' <summary>
		''' Return the simple boolean attributes for the file or directory denoted
		''' by the given abstract pathname, or zero if it does not exist or some
		''' other I/O error occurs.
		''' </summary>
		Public MustOverride Function getBooleanAttributes(  f As File) As Integer

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
		Public Const ACCESS_READ As Integer = &H4
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
		Public Const ACCESS_WRITE As Integer = &H2
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
		Public Const ACCESS_EXECUTE As Integer = &H1

		''' <summary>
		''' Check whether the file or directory denoted by the given abstract
		''' pathname may be accessed by this process.  The second argument specifies
		''' which access, ACCESS_READ, ACCESS_WRITE or ACCESS_EXECUTE, to check.
		''' Return false if access is denied or an I/O error occurs
		''' </summary>
		Public MustOverride Function checkAccess(  f As File,   access As Integer) As Boolean
		''' <summary>
		''' Set on or off the access permission (to owner only or to all) to the file
		''' or directory denoted by the given abstract pathname, based on the parameters
		''' enable, access and oweronly.
		''' </summary>
		Public MustOverride Function setPermission(  f As File,   access As Integer,   enable As Boolean,   owneronly As Boolean) As Boolean

		''' <summary>
		''' Return the time at which the file or directory denoted by the given
		''' abstract pathname was last modified, or zero if it does not exist or
		''' some other I/O error occurs.
		''' </summary>
		Public MustOverride Function getLastModifiedTime(  f As File) As Long

		''' <summary>
		''' Return the length in bytes of the file denoted by the given abstract
		''' pathname, or zero if it does not exist, is a directory, or some other
		''' I/O error occurs.
		''' </summary>
		Public MustOverride Function getLength(  f As File) As Long


		' -- File operations -- 

		''' <summary>
		''' Create a new empty file with the given pathname.  Return
		''' <code>true</code> if the file was created and <code>false</code> if a
		''' file or directory with the given pathname already exists.  Throw an
		''' IOException if an I/O error occurs.
		''' </summary>
		Public MustOverride Function createFileExclusively(  pathname As String) As Boolean

		''' <summary>
		''' Delete the file or directory denoted by the given abstract pathname,
		''' returning <code>true</code> if and only if the operation succeeds.
		''' </summary>
		Public MustOverride Function delete(  f As File) As Boolean

		''' <summary>
		''' List the elements of the directory denoted by the given abstract
		''' pathname.  Return an array of strings naming the elements of the
		''' directory if successful; otherwise, return <code>null</code>.
		''' </summary>
		Public MustOverride Function list(  f As File) As String()

		''' <summary>
		''' Create a new directory denoted by the given abstract pathname,
		''' returning <code>true</code> if and only if the operation succeeds.
		''' </summary>
		Public MustOverride Function createDirectory(  f As File) As Boolean

		''' <summary>
		''' Rename the file or directory denoted by the first abstract pathname to
		''' the second abstract pathname, returning <code>true</code> if and only if
		''' the operation succeeds.
		''' </summary>
		Public MustOverride Function rename(  f1 As File,   f2 As File) As Boolean

		''' <summary>
		''' Set the last-modified time of the file or directory denoted by the
		''' given abstract pathname, returning <code>true</code> if and only if the
		''' operation succeeds.
		''' </summary>
		Public MustOverride Function setLastModifiedTime(  f As File,   time As Long) As Boolean

		''' <summary>
		''' Mark the file or directory denoted by the given abstract pathname as
		''' read-only, returning <code>true</code> if and only if the operation
		''' succeeds.
		''' </summary>
		Public MustOverride Function setReadOnly(  f As File) As Boolean


		' -- Filesystem interface -- 

		''' <summary>
		''' List the available filesystem roots.
		''' </summary>
		Public MustOverride Function listRoots() As File()

		' -- Disk usage -- 
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
		Public Const SPACE_TOTAL As Integer = 0
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
		Public Const SPACE_FREE As Integer = 1
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
		Public Const SPACE_USABLE As Integer = 2

		Public MustOverride Function getSpace(  f As File,   t As Integer) As Long

		' -- Basic infrastructure -- 

		''' <summary>
		''' Compare two abstract pathnames lexicographically.
		''' </summary>
		Public MustOverride Function compare(  f1 As File,   f2 As File) As Integer

		''' <summary>
		''' Compute the hash code of an abstract pathname.
		''' </summary>
		Public MustOverride Function GetHashCode(  f As File) As Integer

		' Flags for enabling/disabling performance optimizations for file
		' name canonicalization
		Friend Shared useCanonCaches As Boolean = True
		Friend Shared useCanonPrefixCache As Boolean = True

		Private Shared Function getBooleanProperty(  prop As String,   defaultVal As Boolean) As Boolean
			Dim val As String = System.getProperty(prop)
			If val Is Nothing Then Return defaultVal
			If val.equalsIgnoreCase("true") Then
				Return True
			Else
				Return False
			End If
		End Function

		Shared Sub New()
			useCanonCaches = getBooleanProperty("sun.io.useCanonCaches", useCanonCaches)
			useCanonPrefixCache = getBooleanProperty("sun.io.useCanonPrefixCache", useCanonPrefixCache)
		End Sub
	End Class

End Namespace