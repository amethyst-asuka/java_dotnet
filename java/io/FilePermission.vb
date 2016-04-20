Imports Microsoft.VisualBasic
Imports System
Imports System.Collections
Imports System.Collections.Generic

'
' * Copyright (c) 1997, 2013, Oracle and/or its affiliates. All rights reserved.
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
	''' This class represents access to a file or directory.  A FilePermission consists
	''' of a pathname and a set of actions valid for that pathname.
	''' <P>
	''' Pathname is the pathname of the file or directory granted the specified
	''' actions. A pathname that ends in "/*" (where "/" is
	''' the file separator character, <code>File.separatorChar</code>) indicates
	''' all the files and directories contained in that directory. A pathname
	''' that ends with "/-" indicates (recursively) all files
	''' and subdirectories contained in that directory. A pathname consisting of
	''' the special token "&lt;&lt;ALL FILES&gt;&gt;" matches <b>any</b> file.
	''' <P>
	''' Note: A pathname consisting of a single "*" indicates all the files
	''' in the current directory, while a pathname consisting of a single "-"
	''' indicates all the files in the current directory and
	''' (recursively) all files and subdirectories contained in the current
	''' directory.
	''' <P>
	''' The actions to be granted are passed to the constructor in a string containing
	''' a list of one or more comma-separated keywords. The possible keywords are
	''' "read", "write", "execute", "delete", and "readlink". Their meaning is
	''' defined as follows:
	''' 
	''' <DL>
	'''    <DT> read <DD> read permission
	'''    <DT> write <DD> write permission
	'''    <DT> execute
	'''    <DD> execute permission. Allows <code>Runtime.exec</code> to
	'''         be called. Corresponds to <code>SecurityManager.checkExec</code>.
	'''    <DT> delete
	'''    <DD> delete permission. Allows <code>File.delete</code> to
	'''         be called. Corresponds to <code>SecurityManager.checkDelete</code>.
	'''    <DT> readlink
	'''    <DD> read link permission. Allows the target of a
	'''         <a href="../nio/file/package-summary.html#links">symbolic link</a>
	'''         to be read by invoking the {@link java.nio.file.Files#readSymbolicLink
	'''         readSymbolicLink } method.
	''' </DL>
	''' <P>
	''' The actions string is converted to lowercase before processing.
	''' <P>
	''' Be careful when granting FilePermissions. Think about the implications
	''' of granting read and especially write access to various files and
	''' directories. The "&lt;&lt;ALL FILES&gt;&gt;" permission with write action is
	''' especially dangerous. This grants permission to write to the entire
	''' file system. One thing this effectively allows is replacement of the
	''' system binary, including the JVM runtime environment.
	''' 
	''' <p>Please note: Code can always read a file from the same
	''' directory it's in (or a subdirectory of that directory); it does not
	''' need explicit permission to do so.
	''' </summary>
	''' <seealso cref= java.security.Permission </seealso>
	''' <seealso cref= java.security.Permissions </seealso>
	''' <seealso cref= java.security.PermissionCollection
	''' 
	''' 
	''' @author Marianne Mueller
	''' @author Roland Schemers
	''' @since 1.2
	''' 
	''' @serial exclude </seealso>

	<Serializable> _
	Public NotInheritable Class FilePermission
		Inherits Permission

		''' <summary>
		''' Execute action.
		''' </summary>
		Private Const EXECUTE As Integer = &H1
		''' <summary>
		''' Write action.
		''' </summary>
		Private Const WRITE As Integer = &H2
		''' <summary>
		''' Read action.
		''' </summary>
		Private Const READ As Integer = &H4
		''' <summary>
		''' Delete action.
		''' </summary>
		Private Const DELETE As Integer = &H8
		''' <summary>
		''' Read link action.
		''' </summary>
		Private Const READLINK As Integer = &H10

		''' <summary>
		''' All actions (read,write,execute,delete,readlink)
		''' </summary>
		Private Shared ReadOnly ALL As Integer = READ Or WRITE Or EXECUTE Or DELETE Or READLINK
		''' <summary>
		''' No actions.
		''' </summary>
		Private Const NONE As Integer = &H0

		' the actions mask
		<NonSerialized> _
		Private mask As Integer

		' does path indicate a directory? (wildcard or recursive)
		<NonSerialized> _
		Private directory As Boolean

		' is it a recursive directory specification?
		<NonSerialized> _
		Private recursive As Boolean

		''' <summary>
		''' the actions string.
		''' 
		''' @serial
		''' </summary>
		Private actions As String ' Left null as long as possible, then
								' created and re-used in the getAction function.

		' canonicalized dir path. In the case of
		' directories, it is the name "/blah/*" or "/blah/-" without
		' the last character (the "*" or "-").

		<NonSerialized> _
		Private cpath As String

		' static Strings used by init(int mask)
		Private Const RECURSIVE_CHAR As Char = "-"c
		Private Const WILD_CHAR As Char = "*"c

	'
	'    public String toString()
	'    {
	'        StringBuffer sb = new StringBuffer();
	'        sb.append("***\n");
	'        sb.append("cpath = "+cpath+"\n");
	'        sb.append("mask = "+mask+"\n");
	'        sb.append("actions = "+getActions()+"\n");
	'        sb.append("directory = "+directory+"\n");
	'        sb.append("recursive = "+recursive+"\n");
	'        sb.append("***\n");
	'        return sb.toString();
	'    }
	'

		Private Const serialVersionUID As Long = 7930732926638008763L

		''' <summary>
		''' initialize a FilePermission object. Common to all constructors.
		''' Also called during de-serialization.
		''' </summary>
		''' <param name="mask"> the actions mask to use.
		'''  </param>
		Private Sub init(  mask As Integer)
            'If (mask And ALL) <> mask Then Throw New IllegalArgumentException("invalid actions mask")

            'If mask = NONE Then Throw New IllegalArgumentException("invalid actions mask")

            'cpath = name
            'If cpath Is Nothing Then Throw New NullPointerException("name can't be null")

            'Me.mask = mask

            'If cpath.Equals("<<ALL FILES>>") Then
            '	directory = True
            '	recursive = True
            '	cpath = ""
            '	Return
            'End If

            '         ' store only the canonical cpath if possible
            '         cpath = AccessController.doPrivileged(New PrivilegedActionAnonymousInnerClassHelper(Of T))

            '         Dim len As Integer = cpath.length()
            'Dim last As Char = (If(len > 0, cpath.Chars(len - 1), 0))

            'If last = RECURSIVE_CHAR AndAlso cpath.Chars(len - 2) = System.IO.Path.DirectorySeparatorChar Then
            '	directory = True
            '	recursive = True
            '	len -= 1
            '	cpath = cpath.Substring(0, len)
            'ElseIf last = WILD_CHAR AndAlso cpath.Chars(len - 2) = System.IO.Path.DirectorySeparatorChar Then
            '	directory = True
            '	'recursive = false;
            '	len -= 1
            '	cpath = cpath.Substring(0, len)
            'Else
            '	' overkill since they are initialized to false, but
            '	' commented out here to remind us...
            '	'directory = false;
            '	'recursive = false;
            'End If

            '' XXX: at this point the path should be absolute. die if it isn't?
        End Sub

		Private Class PrivilegedActionAnonymousInnerClassHelper(Of T)
			Implements PrivilegedAction(Of T)

			Public Overridable Function run() As String
				Try
					Dim path As String = outerInstance.cpath
					If outerInstance.cpath.EndsWith("*") Then
						' call getCanonicalPath with a path with wildcard character
						' replaced to avoid calling it with paths that are
						' intended to match all entries in a directory
						path = path.Substring(0, path.length()-1) & "-"
						path = (New File(path)).canonicalPath
						Return path.Substring(0, path.length()-1) & "*"
					Else
						Return (New File(path)).canonicalPath
					End If
				Catch ioe As IOException
					Return outerInstance.cpath
				End Try
			End Function
		End Class

		''' <summary>
		''' Creates a new FilePermission object with the specified actions.
		''' <i>path</i> is the pathname of a file or directory, and <i>actions</i>
		''' contains a comma-separated list of the desired actions granted on the
		''' file or directory. Possible actions are
		''' "read", "write", "execute", "delete", and "readlink".
		''' 
		''' <p>A pathname that ends in "/*" (where "/" is
		''' the file separator character, <code>File.separatorChar</code>)
		''' indicates all the files and directories contained in that directory.
		''' A pathname that ends with "/-" indicates (recursively) all files and
		''' subdirectories contained in that directory. The special pathname
		''' "&lt;&lt;ALL FILES&gt;&gt;" matches any file.
		''' 
		''' <p>A pathname consisting of a single "*" indicates all the files
		''' in the current directory, while a pathname consisting of a single "-"
		''' indicates all the files in the current directory and
		''' (recursively) all files and subdirectories contained in the current
		''' directory.
		''' 
		''' <p>A pathname containing an empty string represents an empty path.
		''' </summary>
		''' <param name="path"> the pathname of the file/directory. </param>
		''' <param name="actions"> the action string.
		''' </param>
		''' <exception cref="IllegalArgumentException">
		'''          If actions is <code>null</code>, empty or contains an action
		'''          other than the specified possible actions. </exception>
		Public Sub New(  path As String,   actions As String)
			MyBase.New(path)
			init(getMask(actions))
		End Sub

		''' <summary>
		''' Creates a new FilePermission object using an action mask.
		''' More efficient than the FilePermission(String, String) constructor.
		''' Can be used from within
		''' code that needs to create a FilePermission object to pass into the
		''' <code>implies</code> method.
		''' </summary>
		''' <param name="path"> the pathname of the file/directory. </param>
		''' <param name="mask"> the action mask to use. </param>

		' package private for use by the FilePermissionCollection add method
		Friend Sub New(  path As String,   mask As Integer)
			MyBase.New(path)
			init(mask)
		End Sub

		''' <summary>
		''' Checks if this FilePermission object "implies" the specified permission.
		''' <P>
		''' More specifically, this method returns true if:
		''' <ul>
		''' <li> <i>p</i> is an instanceof FilePermission,
		''' <li> <i>p</i>'s actions are a proper subset of this
		''' object's actions, and
		''' <li> <i>p</i>'s pathname is implied by this object's
		'''      pathname. For example, "/tmp/*" implies "/tmp/foo", since
		'''      "/tmp/*" encompasses all files in the "/tmp" directory,
		'''      including the one named "foo".
		''' </ul>
		''' </summary>
		''' <param name="p"> the permission to check against.
		''' </param>
		''' <returns> <code>true</code> if the specified permission is not
		'''                  <code>null</code> and is implied by this object,
		'''                  <code>false</code> otherwise. </returns>
		Public Overrides Function implies(  p As Permission) As Boolean
			If Not(TypeOf p Is FilePermission) Then Return False

			Dim that As FilePermission = CType(p, FilePermission)

			' we get the effective mask. i.e., the "and" of this and that.
			' They must be equal to that.mask for implies to return true.

			Return ((Me.mask And that.mask) = that.mask) AndAlso impliesIgnoreMask(that)
		End Function

		''' <summary>
		''' Checks if the Permission's actions are a proper subset of the
		''' this object's actions. Returns the effective mask iff the
		''' this FilePermission's path also implies that FilePermission's path.
		''' </summary>
		''' <param name="that"> the FilePermission to check against. </param>
		''' <returns> the effective mask </returns>
		Friend Function impliesIgnoreMask(  that As FilePermission) As Boolean
			If Me.directory Then
				If Me.recursive Then
					' make sure that.path is longer then path so
					' something like /foo/- does not imply /foo
					If that.directory Then
						Return (that.cpath.length() >= Me.cpath.length()) AndAlso that.cpath.StartsWith(Me.cpath)
					Else
						Return ((that.cpath.length() > Me.cpath.length()) AndAlso that.cpath.StartsWith(Me.cpath))
					End If
				Else
					If that.directory Then
						' if the permission passed in is a directory
						' specification, make sure that a non-recursive
						' permission (i.e., this object) can't imply a recursive
						' permission.
						If that.recursive Then
							Return False
						Else
							Return (Me.cpath.Equals(that.cpath))
						End If
					Else
						Dim last As Integer = that.cpath.LastIndexOf(System.IO.Path.DirectorySeparatorChar)
						If last = -1 Then
							Return False
						Else
							' this.cpath.equals(that.cpath.substring(0, last+1));
							' Use regionMatches to avoid creating new string
							Return (Me.cpath.length() = (last + 1)) AndAlso Me.cpath.regionMatches(0, that.cpath, 0, last+1)
						End If
					End If
				End If
			ElseIf that.directory Then
				' if this is NOT recursive/wildcarded,
				' do not let it imply a recursive/wildcarded permission
				Return False
			Else
				Return (Me.cpath.Equals(that.cpath))
			End If
		End Function

		''' <summary>
		''' Checks two FilePermission objects for equality. Checks that <i>obj</i> is
		''' a FilePermission, and has the same pathname and actions as this object.
		''' </summary>
		''' <param name="obj"> the object we are testing for equality with this object. </param>
		''' <returns> <code>true</code> if obj is a FilePermission, and has the same
		'''          pathname and actions as this FilePermission object,
		'''          <code>false</code> otherwise. </returns>
		Public Overrides Function Equals(  obj As Object) As Boolean
			If obj Is Me Then Return True

			If Not(TypeOf obj Is FilePermission) Then Return False

			Dim that As FilePermission = CType(obj, FilePermission)

			Return (Me.mask = that.mask) AndAlso Me.cpath.Equals(that.cpath) AndAlso (Me.directory = that.directory) AndAlso (Me.recursive = that.recursive)
		End Function

		''' <summary>
		''' Returns the hash code value for this object.
		''' </summary>
		''' <returns> a hash code value for this object. </returns>
		Public Overrides Function GetHashCode() As Integer
			Return 0
		End Function

		''' <summary>
		''' Converts an actions String to an actions mask.
		''' </summary>
		''' <param name="actions"> the action string. </param>
		''' <returns> the actions mask. </returns>
		Private Shared Function getMask(  actions As String) As Integer
			Dim mask_Renamed As Integer = NONE

			' Null action valid?
			If actions Is Nothing Then Return mask_Renamed

			' Use object identity comparison against known-interned strings for
			' performance benefit (these values are used heavily within the JDK).
			If actions = sun.security.util.SecurityConstants.FILE_READ_ACTION Then
				Return READ
			ElseIf actions = sun.security.util.SecurityConstants.FILE_WRITE_ACTION Then
				Return WRITE
			ElseIf actions = sun.security.util.SecurityConstants.FILE_EXECUTE_ACTION Then
				Return EXECUTE
			ElseIf actions = sun.security.util.SecurityConstants.FILE_DELETE_ACTION Then
				Return DELETE
			ElseIf actions = sun.security.util.SecurityConstants.FILE_READLINK_ACTION Then
				Return READLINK
			End If

			Dim a As Char() = actions.ToCharArray()

			Dim i As Integer = a.Length - 1
			If i < 0 Then Return mask_Renamed

			Do While i <> -1
				Dim c As Char

				' skip whitespace
				c = a(i)
				Do While (i<>-1) AndAlso (c = " "c OrElse c = ControlChars.Cr OrElse c = ControlChars.Lf OrElse c = ControlChars.FormFeed OrElse c = ControlChars.Tab)
					i -= 1
					c = a(i)
				Loop

				' check for the known strings
				Dim matchlen As Integer

				If i >= 3 AndAlso (a(i-3) = "r"c OrElse a(i-3) = "R"c) AndAlso (a(i-2) = "e"c OrElse a(i-2) = "E"c) AndAlso (a(i-1) = "a"c OrElse a(i-1) = "A"c) AndAlso (a(i) = "d"c OrElse a(i) = "D"c) Then
					matchlen = 4
					mask_Renamed = mask_Renamed Or READ

				ElseIf i >= 4 AndAlso (a(i-4) = "w"c OrElse a(i-4) = "W"c) AndAlso (a(i-3) = "r"c OrElse a(i-3) = "R"c) AndAlso (a(i-2) = "i"c OrElse a(i-2) = "I"c) AndAlso (a(i-1) = "t"c OrElse a(i-1) = "T"c) AndAlso (a(i) = "e"c OrElse a(i) = "E"c) Then
					matchlen = 5
					mask_Renamed = mask_Renamed Or WRITE

				ElseIf i >= 6 AndAlso (a(i-6) = "e"c OrElse a(i-6) = "E"c) AndAlso (a(i-5) = "x"c OrElse a(i-5) = "X"c) AndAlso (a(i-4) = "e"c OrElse a(i-4) = "E"c) AndAlso (a(i-3) = "c"c OrElse a(i-3) = "C"c) AndAlso (a(i-2) = "u"c OrElse a(i-2) = "U"c) AndAlso (a(i-1) = "t"c OrElse a(i-1) = "T"c) AndAlso (a(i) = "e"c OrElse a(i) = "E"c) Then
					matchlen = 7
					mask_Renamed = mask_Renamed Or EXECUTE

				ElseIf i >= 5 AndAlso (a(i-5) = "d"c OrElse a(i-5) = "D"c) AndAlso (a(i-4) = "e"c OrElse a(i-4) = "E"c) AndAlso (a(i-3) = "l"c OrElse a(i-3) = "L"c) AndAlso (a(i-2) = "e"c OrElse a(i-2) = "E"c) AndAlso (a(i-1) = "t"c OrElse a(i-1) = "T"c) AndAlso (a(i) = "e"c OrElse a(i) = "E"c) Then
					matchlen = 6
					mask_Renamed = mask_Renamed Or DELETE

				ElseIf i >= 7 AndAlso (a(i-7) = "r"c OrElse a(i-7) = "R"c) AndAlso (a(i-6) = "e"c OrElse a(i-6) = "E"c) AndAlso (a(i-5) = "a"c OrElse a(i-5) = "A"c) AndAlso (a(i-4) = "d"c OrElse a(i-4) = "D"c) AndAlso (a(i-3) = "l"c OrElse a(i-3) = "L"c) AndAlso (a(i-2) = "i"c OrElse a(i-2) = "I"c) AndAlso (a(i-1) = "n"c OrElse a(i-1) = "N"c) AndAlso (a(i) = "k"c OrElse a(i) = "K"c) Then
					matchlen = 8
					mask_Renamed = mask_Renamed Or READLINK

				Else
					' parse error
					Throw New IllegalArgumentException("invalid permission: " & actions)
				End If

				' make sure we didn't just match the tail of a word
				' like "ackbarfaccept".  Also, skip to the comma.
				Dim seencomma As Boolean = False
				Do While i >= matchlen AndAlso Not seencomma
					Select Case a(i-matchlen)
					Case ","c
						seencomma = True
					Case " "c, ControlChars.Cr, ControlChars.Lf, ControlChars.FormFeed, ControlChars.Tab
					Case Else
						Throw New IllegalArgumentException("invalid permission: " & actions)
					End Select
					i -= 1
				Loop

				' point i at the location of the comma minus one (or -1).
				i -= matchlen
			Loop

			Return mask_Renamed
		End Function

		''' <summary>
		''' Return the current action mask. Used by the FilePermissionCollection.
		''' </summary>
		''' <returns> the actions mask. </returns>
		Friend Property mask As Integer
			Get
				Return mask
			End Get
		End Property

		''' <summary>
		''' Return the canonical string representation of the actions.
		''' Always returns present actions in the following order:
		''' read, write, execute, delete, readlink.
		''' </summary>
		''' <returns> the canonical string representation of the actions. </returns>
		Private Shared Function getActions(  mask As Integer) As String
			Dim sb As New StringBuilder
			Dim comma As Boolean = False

			If (mask And READ) = READ Then
				comma = True
				sb.append("read")
			End If

			If (mask And WRITE) = WRITE Then
				If comma Then
					sb.append(","c)
				Else
					comma = True
				End If
				sb.append("write")
			End If

			If (mask And EXECUTE) = EXECUTE Then
				If comma Then
					sb.append(","c)
				Else
					comma = True
				End If
				sb.append("execute")
			End If

			If (mask And DELETE) = DELETE Then
				If comma Then
					sb.append(","c)
				Else
					comma = True
				End If
				sb.append("delete")
			End If

			If (mask And READLINK) = READLINK Then
				If comma Then
					sb.append(","c)
				Else
					comma = True
				End If
				sb.append("readlink")
			End If

			Return sb.ToString()
		End Function

		''' <summary>
		''' Returns the "canonical string representation" of the actions.
		''' That is, this method always returns present actions in the following order:
		''' read, write, execute, delete, readlink. For example, if this FilePermission
		''' object allows both write and read actions, a call to <code>getActions</code>
		''' will return the string "read,write".
		''' </summary>
		''' <returns> the canonical string representation of the actions. </returns>
		Public  Overrides ReadOnly Property  actions As String
			Get
				If actions Is Nothing Then actions = getActions(Me.mask)
    
				Return actions
			End Get
		End Property

		''' <summary>
		''' Returns a new PermissionCollection object for storing FilePermission
		''' objects.
		''' <p>
		''' FilePermission objects must be stored in a manner that allows them
		''' to be inserted into the collection in any order, but that also enables the
		''' PermissionCollection <code>implies</code>
		''' method to be implemented in an efficient (and consistent) manner.
		''' 
		''' <p>For example, if you have two FilePermissions:
		''' <OL>
		''' <LI>  <code>"/tmp/-", "read"</code>
		''' <LI>  <code>"/tmp/scratch/foo", "write"</code>
		''' </OL>
		''' 
		''' <p>and you are calling the <code>implies</code> method with the FilePermission:
		''' 
		''' <pre>
		'''   "/tmp/scratch/foo", "read,write",
		''' </pre>
		''' 
		''' then the <code>implies</code> function must
		''' take into account both the "/tmp/-" and "/tmp/scratch/foo"
		''' permissions, so the effective permission is "read,write",
		''' and <code>implies</code> returns true. The "implies" semantics for
		''' FilePermissions are handled properly by the PermissionCollection object
		''' returned by this <code>newPermissionCollection</code> method.
		''' </summary>
		''' <returns> a new PermissionCollection object suitable for storing
		''' FilePermissions. </returns>
		Public Overrides Function newPermissionCollection() As PermissionCollection
			Return New FilePermissionCollection
		End Function

		''' <summary>
		''' WriteObject is called to save the state of the FilePermission
		''' to a stream. The actions are serialized, and the superclass
		''' takes care of the name.
		''' </summary>
		Private Sub writeObject(  s As ObjectOutputStream)
			' Write out the actions. The superclass takes care of the name
			' call getActions to make sure actions field is initialized
			If actions Is Nothing Then actions
			s.defaultWriteObject()
		End Sub

		''' <summary>
		''' readObject is called to restore the state of the FilePermission from
		''' a stream.
		''' </summary>
		Private Sub readObject(  s As ObjectInputStream)
			' Read in the actions, then restore everything else by calling init.
			s.defaultReadObject()
			init(getMask(actions))
		End Sub
	End Class

	''' <summary>
	''' A FilePermissionCollection stores a set of FilePermission permissions.
	''' FilePermission objects
	''' must be stored in a manner that allows them to be inserted in any
	''' order, but enable the implies function to evaluate the implies
	''' method.
	''' For example, if you have two FilePermissions:
	''' <OL>
	''' <LI> "/tmp/-", "read"
	''' <LI> "/tmp/scratch/foo", "write"
	''' </OL>
	''' And you are calling the implies function with the FilePermission:
	''' "/tmp/scratch/foo", "read,write", then the implies function must
	''' take into account both the /tmp/- and /tmp/scratch/foo
	''' permissions, so the effective permission is "read,write".
	''' </summary>
	''' <seealso cref= java.security.Permission </seealso>
	''' <seealso cref= java.security.Permissions </seealso>
	''' <seealso cref= java.security.PermissionCollection
	''' 
	''' 
	''' @author Marianne Mueller
	''' @author Roland Schemers
	''' 
	''' @serial include
	'''  </seealso>

	<Serializable> _
	Friend NotInheritable Class FilePermissionCollection
		Inherits PermissionCollection

		' Not serialized; see serialization section at end of class
		<NonSerialized> _
		Private perms As IList(Of Permission)

		''' <summary>
		''' Create an empty FilePermissionCollection object.
		''' </summary>
		Public Sub New()
			perms = New List(Of )
		End Sub

		''' <summary>
		''' Adds a permission to the FilePermissionCollection. The key for the hash is
		''' permission.path.
		''' </summary>
		''' <param name="permission"> the Permission object to add.
		''' </param>
		''' <exception cref="IllegalArgumentException"> - if the permission is not a
		'''                                       FilePermission
		''' </exception>
		''' <exception cref="SecurityException"> - if this FilePermissionCollection object
		'''                                has been marked readonly </exception>
		Public Overrides Sub add(  permission As Permission)
			If Not(TypeOf permission Is FilePermission) Then Throw New IllegalArgumentException("invalid permission: " & permission)
			If [readOnly] Then Throw New SecurityException("attempt to add a Permission to a readonly PermissionCollection")

			SyncLock Me
				perms.Add(permission)
			End SyncLock
		End Sub

		''' <summary>
		''' Check and see if this set of permissions implies the permissions
		''' expressed in "permission".
		''' </summary>
		''' <param name="permission"> the Permission object to compare
		''' </param>
		''' <returns> true if "permission" is a proper subset of a permission in
		''' the set, false if not. </returns>
		Public Overrides Function implies(  permission As Permission) As Boolean
			If Not(TypeOf permission Is FilePermission) Then Return False

			Dim fp As FilePermission = CType(permission, FilePermission)

			Dim desired As Integer = fp.mask
			Dim effective As Integer = 0
			Dim needed As Integer = desired

			SyncLock Me
				Dim len As Integer = perms.Count
				For i As Integer = 0 To len - 1
					Dim x As FilePermission = CType(perms(i), FilePermission)
					If ((needed And x.mask) <> 0) AndAlso x.impliesIgnoreMask(fp) Then
						effective = effective Or x.mask
						If (effective And desired) = desired Then Return True
						needed = (desired Xor effective)
					End If
				Next i
			End SyncLock
			Return False
		End Function

		''' <summary>
		''' Returns an enumeration of all the FilePermission objects in the
		''' container.
		''' </summary>
		''' <returns> an enumeration of all the FilePermission objects. </returns>
		Public Overrides Function elements() As System.Collections.IEnumerator(Of Permission)
			' Convert Iterator into Enumeration
			SyncLock Me
				Return java.util.Collections.enumeration(perms)
			End SyncLock
		End Function

		Private Const serialVersionUID As Long = 2202956749081564585L

		' Need to maintain serialization interoperability with earlier releases,
		' which had the serializable field:
		'    private Vector permissions;

		''' <summary>
		''' @serialField permissions java.util.Vector
		'''     A list of FilePermission objects.
		''' </summary>
		Private Shared ReadOnly serialPersistentFields As ObjectStreamField() = { New ObjectStreamField("permissions", GetType(ArrayList)) }

		''' <summary>
		''' @serialData "permissions" field (a Vector containing the FilePermissions).
		''' </summary>
	'    
	'     * Writes the contents of the perms field out as a Vector for
	'     * serialization compatibility with earlier releases.
	'     
		Private Sub writeObject(  out As ObjectOutputStream)
			' Don't call out.defaultWriteObject()

			' Write out Vector
			Dim permissions As New List(Of Permission)(perms.Count)
			SyncLock Me
				permissions.AddRange(perms)
			End SyncLock

			Dim pfields As ObjectOutputStream.PutField = out.putFields()
			pfields.put("permissions", permissions)
			out.writeFields()
		End Sub

	'    
	'     * Reads in a Vector of FilePermissions and saves them in the perms field.
	'     
		Private Sub readObject(  [in] As ObjectInputStream)
			' Don't call defaultReadObject()

			' Read in serialized fields
			Dim gfields As ObjectInputStream.GetField = [in].readFields()

			' Get the one we want
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
			Dim permissions As List(Of Permission) = CType(gfields.get("permissions", Nothing), List(Of Permission))
			perms = New List(Of )(permissions.Count)
			perms.AddRange(permissions)
		End Sub
	End Class

End Namespace