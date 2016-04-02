Imports System.Runtime.InteropServices

'
' * Copyright (c) 2001, 2012, Oracle and/or its affiliates. All rights reserved.
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
	''' Unicode-aware FileSystem for Windows NT/2000.
	''' 
	''' @author Konstantin Kladko
	''' @since 1.4
	''' </summary>
	Friend Class WinNTFileSystem
		Inherits FileSystem

		Private ReadOnly slash As Char
		Private ReadOnly altSlash As Char
		Private ReadOnly semicolon As Char

		Public Sub New()
			slash = java.security.AccessController.doPrivileged(New sun.security.action.GetPropertyAction("file.separator")).Chars(0)
			semicolon = java.security.AccessController.doPrivileged(New sun.security.action.GetPropertyAction("path.separator")).Chars(0)
			altSlash = If(Me.slash = "\"c, "/"c, "\"c)
		End Sub

		Private Function isSlash(ByVal c As Char) As Boolean
			Return (c = "\"c) OrElse (c = "/"c)
		End Function

		Private Function isLetter(ByVal c As Char) As Boolean
			Return ((c >= "a"c) AndAlso (c <= "z"c)) OrElse ((c >= "A"c) AndAlso (c <= "Z"c))
		End Function

		Private Function slashify(ByVal p As String) As String
			If (p.length() > 0) AndAlso (p.Chars(0) <> slash) Then
				Return AscW(slash) + p
			Else
				Return p
			End If
		End Function

		' -- Normalization and construction -- 

		Public  Overrides ReadOnly Property  separator As Char
			Get
				Return slash
			End Get
		End Property

		Public  Overrides ReadOnly Property  pathSeparator As Char
			Get
				Return semicolon
			End Get
		End Property

	'     Check that the given pathname is normal.  If not, invoke the real
	'       normalizer on the part of the pathname that requires normalization.
	'       This way we iterate through the whole pathname string only once. 
		Public Overrides Function normalize(ByVal path As String) As String
			Dim n As Integer = path.length()
			Dim slash_Renamed As Char = Me.slash
			Dim altSlash As Char = Me.altSlash
			Dim prev As Char = 0
			For i As Integer = 0 To n - 1
				Dim c As Char = path.Chars(i)
				If c = altSlash Then Return normalize(path, n,If(prev = slash_Renamed, i - 1, i))
				If (c = slash_Renamed) AndAlso (prev = slash_Renamed) AndAlso (i > 1) Then Return normalize(path, n, i - 1)
				If (c = ":"c) AndAlso (i > 1) Then Return normalize(path, n, 0)
				prev = c
			Next i
			If prev = slash_Renamed Then Return normalize(path, n, n - 1)
			Return path
		End Function

	'     Normalize the given pathname, whose length is len, starting at the given
	'       offset; everything before this offset is already normal. 
		Private Function normalize(ByVal path As String, ByVal len As Integer, ByVal [off] As Integer) As String
			If len = 0 Then Return path
			If [off] < 3 Then ' Avoid fencepost cases with UNC pathnames [off] = 0
			Dim src As Integer
			Dim slash_Renamed As Char = Me.slash
			Dim sb As New StringBuffer(len)

			If [off] = 0 Then
				' Complete normalization, including prefix 
				src = normalizePrefix(path, len, sb)
			Else
				' Partial normalization 
				src = [off]
				sb.append(path.Substring(0, [off]))
			End If

	'         Remove redundant slashes from the remainder of the path, forcing all
	'           slashes into the preferred slash 
			Do While src < len
				Dim c As Char = path.Chars(src)
				src += 1
				If isSlash(c) Then
					Do While (src < len) AndAlso isSlash(path.Chars(src))
						src += 1
					Loop
					If src = len Then
						' Check for trailing separator 
						Dim sn As Integer = sb.length()
						If (sn = 2) AndAlso (sb.Chars(1) Is ":"c) Then
							' "z:\\" 
							sb.append(slash_Renamed)
							Exit Do
						End If
						If sn = 0 Then
							' "\\" 
							sb.append(slash_Renamed)
							Exit Do
						End If
						If (sn = 1) AndAlso (isSlash(sb.Chars(0))) Then
	'                         "\\\\" is not collapsed to "\\" because "\\\\" marks
	'                           the beginning of a UNC pathname.  Even though it is
	'                           not, by itself, a valid UNC pathname, we leave it as
	'                           is in order to be consistent with the win32 APIs,
	'                           which treat this case as an invalid UNC pathname
	'                           rather than as an alias for the root directory of
	'                           the current drive. 
							sb.append(slash_Renamed)
							Exit Do
						End If
	'                     Path does not denote a root directory, so do not append
	'                       trailing slash 
						Exit Do
					Else
						sb.append(slash_Renamed)
					End If
				Else
					sb.append(c)
				End If
			Loop

			Dim rv As String = sb.ToString()
			Return rv
		End Function

	'     A normal Win32 pathname contains no duplicate slashes, except possibly
	'       for a UNC prefix, and does not end with a slash.  It may be the empty
	'       string.  Normalized Win32 pathnames have the convenient property that
	'       the length of the prefix almost uniquely identifies the type of the path
	'       and whether it is absolute or relative:
	'
	'           0  relative to both drive and directory
	'           1  drive-relative (begins with '\\')
	'           2  absolute UNC (if first char is '\\'),
	'                else directory-relative (has form "z:foo")
	'           3  absolute local pathname (begins with "z:\\")
	'     
		Private Function normalizePrefix(ByVal path As String, ByVal len As Integer, ByVal sb As StringBuffer) As Integer
			Dim src As Integer = 0
			Do While (src < len) AndAlso isSlash(path.Chars(src))
				src += 1
			Loop
			Dim c As Char
			c = path.Chars(src)
			If (len - src >= 2) AndAlso isLetterc AndAlso path.Chars(src + 1) = ":"c Then
	'             Remove leading slashes if followed by drive specifier.
	'               This hack is necessary to support file URLs containing drive
	'               specifiers (e.g., "file://c:/path").  As a side effect,
	'               "/c:/path" can be used as an alternative to "c:/path". 
				sb.append(c)
				sb.append(":"c)
				src += 2
			Else
				src = 0
				If (len >= 2) AndAlso isSlash(path.Chars(0)) AndAlso isSlash(path.Chars(1)) Then
	'                 UNC pathname: Retain first slash; leave src pointed at
	'                   second slash so that further slashes will be collapsed
	'                   into the second slash.  The result will be a pathname
	'                   beginning with "\\\\" followed (most likely) by a host
	'                   name. 
					src = 1
					sb.append(slash)
				End If
			End If
			Return src
		End Function

		Public Overrides Function prefixLength(ByVal path As String) As Integer
			Dim slash_Renamed As Char = Me.slash
			Dim n As Integer = path.length()
			If n = 0 Then Return 0
			Dim c0 As Char = path.Chars(0)
			Dim c1 As Char = If(n > 1, path.Chars(1), 0)
			If c0 = slash_Renamed Then
				If c1 = slash_Renamed Then ' Absolute UNC pathname "\\\\foo" Return 2
				Return 1 ' Drive-relative "\\foo"
			End If
			If isLetter(c0) AndAlso (c1 = ":"c) Then
				If (n > 2) AndAlso (path.Chars(2) = slash_Renamed) Then Return 3 ' Absolute local pathname "z:\\foo"
				Return 2 ' Directory-relative "z:foo"
			End If
			Return 0 ' Completely relative
		End Function

		Public Overrides Function resolve(ByVal parent As String, ByVal child As String) As String
			Dim pn As Integer = parent.length()
			If pn = 0 Then Return child
			Dim cn As Integer = child.length()
			If cn = 0 Then Return parent

			Dim c As String = child
			Dim childStart As Integer = 0
			Dim parentEnd As Integer = pn

			If (cn > 1) AndAlso (c.Chars(0) = slash) Then
				If c.Chars(1) = slash Then
					' Drop prefix when child is a UNC pathname 
					childStart = 2
				Else
					' Drop prefix when child is drive-relative 
					childStart = 1

				End If
				If cn = childStart Then ' Child is double slash
					If parent.Chars(pn - 1) = slash Then Return parent.Substring(0, pn - 1)
					Return parent
				End If
			End If

			If parent.Chars(pn - 1) = slash Then parentEnd -= 1

			Dim strlen As Integer = parentEnd + cn - childStart
			Dim theChars As Char() = Nothing
			If child.Chars(childStart) = slash Then
				theChars = New Char(strlen - 1){}
				parent.getChars(0, parentEnd, theChars, 0)
				child.getChars(childStart, cn, theChars, parentEnd)
			Else
				theChars = New Char(strlen){}
				parent.getChars(0, parentEnd, theChars, 0)
				theChars(parentEnd) = slash
				child.getChars(childStart, cn, theChars, parentEnd + 1)
			End If
			Return New String(theChars)
		End Function

		Public  Overrides ReadOnly Property  defaultParent As String
			Get
				Return ("" & AscW(slash))
			End Get
		End Property

		Public Overrides Function fromURIPath(ByVal path As String) As String
			Dim p As String = path
			If (p.length() > 2) AndAlso (p.Chars(2) = ":"c) Then
				' "/c:/foo" --> "c:/foo"
				p = p.Substring(1)
				' "c:/foo/" --> "c:/foo", but "c:/" --> "c:/"
				If (p.length() > 3) AndAlso p.EndsWith("/") Then p = p.Substring(0, p.length() - 1)
			ElseIf (p.length() > 1) AndAlso p.EndsWith("/") Then
				' "/foo/" --> "/foo"
				p = p.Substring(0, p.length() - 1)
			End If
			Return p
		End Function

		' -- Path operations -- 

		Public Overrides Function isAbsolute(ByVal f As File) As Boolean
			Dim pl As Integer = f.prefixLength
			Return (((pl = 2) AndAlso (f.path.Chars(0) = slash)) OrElse (pl = 3))
		End Function

		Public Overrides Function resolve(ByVal f As File) As String
			Dim path As String = f.path
			Dim pl As Integer = f.prefixLength
			If (pl = 2) AndAlso (path.Chars(0) = slash) Then Return path ' UNC
			If pl = 3 Then Return path ' Absolute local
			If pl = 0 Then Return userPath + slashify(path) ' Completely relative
			If pl = 1 Then ' Drive-relative
				Dim up As String = userPath
				Dim ud As String = getDrive(up)
				If ud IsNot Nothing Then Return ud + path
				Return up + path ' User dir is a UNC path
			End If
			If pl = 2 Then ' Directory-relative
				Dim up As String = userPath
				Dim ud As String = getDrive(up)
				If (ud IsNot Nothing) AndAlso path.StartsWith(ud) Then Return up + slashify(path.Substring(2))
				Dim drive_Renamed As Char = path.Chars(0)
				Dim dir As String = getDriveDirectory(drive_Renamed)
				Dim np As String
				If dir IsNot Nothing Then
	'                 When resolving a directory-relative path that refers to a
	'                   drive other than the current drive, insist that the caller
	'                   have read permission on the result 
					Dim p As String = AscW(drive_Renamed) + (AscW(":"c) + dir + slashify(path.Substring(2)))
					Dim security As SecurityManager = System.securityManager
					Try
						If security IsNot Nothing Then security.checkRead(p)
					Catch x As SecurityException
						' Don't disclose the drive's directory in the exception 
						Throw New SecurityException("Cannot resolve path " & path)
					End Try
					Return p
				End If
				Return AscW(drive_Renamed) & ":" & slashify(path.Substring(2)) ' fake it
			End If
			Throw New InternalError("Unresolvable path: " & path)
		End Function

		Private Property userPath As String
			Get
		'         For both compatibility and security,
		'           we must look this up every time 
				Return normalize(System.getProperty("user.dir"))
			End Get
		End Property

		Private Function getDrive(ByVal path As String) As String
			Dim pl As Integer = prefixLength(path)
			Return If(pl = 3, path.Substring(0, 2), Nothing)
		End Function

		Private Shared driveDirCache As String() = New String(25){}

		Private Shared Function driveIndex(ByVal d As Char) As Integer
			If (d >= "a"c) AndAlso (d <= "z"c) Then Return AscW(d) - AscW("a"c)
			If (d >= "A"c) AndAlso (d <= "Z"c) Then Return AscW(d) - AscW("A"c)
			Return -1
		End Function

'JAVA TO VB CONVERTER TODO TASK: Replace 'unknown' with the appropriate dll name:
		<DllImport("unknown")> _
		Private Function getDriveDirectory(ByVal drive As Integer) As String
		End Function

		Private Function getDriveDirectory(ByVal drive As Char) As String
			Dim i As Integer = driveIndex(drive)
			If i < 0 Then Return Nothing
			Dim s As String = driveDirCache(i)
			If s IsNot Nothing Then Return s
			s = getDriveDirectory(i + 1)
			driveDirCache(i) = s
			Return s
		End Function

		' Caches for canonicalization results to improve startup performance.
		' The first cache handles repeated canonicalizations of the same path
		' name. The prefix cache handles repeated canonicalizations within the
		' same directory, and must not create results differing from the true
		' canonicalization algorithm in canonicalize_md.c. For this reason the
		' prefix cache is conservative and is not used for complex path names.
		Private cache As New ExpiringCache
		Private prefixCache As New ExpiringCache

		Public Overrides Function canonicalize(ByVal path As String) As String
			' If path is a drive letter only then skip canonicalization
			Dim len As Integer = path.length()
			If (len = 2) AndAlso (isLetter(path.Chars(0))) AndAlso (path.Chars(1) = ":"c) Then
				Dim c As Char = path.Chars(0)
				If (c >= "A"c) AndAlso (c <= "Z"c) Then Return path
				Return "" & (CChar(AscW(c)-32)) + AscW(":"c)
			ElseIf (len = 3) AndAlso (isLetter(path.Chars(0))) AndAlso (path.Chars(1) = ":"c) AndAlso (path.Chars(2) = "\"c) Then
				Dim c As Char = path.Chars(0)
				If (c >= "A"c) AndAlso (c <= "Z"c) Then Return path
				Return "" & (CChar(AscW(c)-32)) + AscW(":"c) + AscW("\"c)
			End If
			If Not useCanonCaches Then
				Return canonicalize0(path)
			Else
				Dim res As String = cache.get(path)
				If res Is Nothing Then
					Dim dir As String = Nothing
					Dim resDir As String = Nothing
					If useCanonPrefixCache Then
						dir = parentOrNull(path)
						If dir IsNot Nothing Then
							resDir = prefixCache.get(dir)
							If resDir IsNot Nothing Then
	'                            
	'                             * Hit only in prefix cache; full path is canonical,
	'                             * but we need to get the canonical name of the file
	'                             * in this directory to get the appropriate
	'                             * capitalization
	'                             
								Dim filename As String = path.Substring(1 + dir.length())
								res = canonicalizeWithPrefix(resDir, filename)
								cache.put(dir + System.IO.Path.DirectorySeparatorChar + filename, res)
							End If
						End If
					End If
					If res Is Nothing Then
						res = canonicalize0(path)
						cache.put(path, res)
						If useCanonPrefixCache AndAlso dir IsNot Nothing Then
							resDir = parentOrNull(res)
							If resDir IsNot Nothing Then
								Dim f As New File(res)
								If f.exists() AndAlso (Not f.directory) Then prefixCache.put(dir, resDir)
							End If
						End If
					End If
				End If
				Return res
			End If
		End Function

'JAVA TO VB CONVERTER TODO TASK: Replace 'unknown' with the appropriate dll name:
		<DllImport("unknown")> _
		Private Function canonicalize0(ByVal path As String) As String
		End Function

		Private Function canonicalizeWithPrefix(ByVal canonicalPrefix As String, ByVal filename As String) As String
			Return canonicalizeWithPrefix0(canonicalPrefix, canonicalPrefix + System.IO.Path.DirectorySeparatorChar + filename)
		End Function

		' Run the canonicalization operation assuming that the prefix
		' (everything up to the last filename) is canonical; just gets
		' the canonical name of the last element of the path
'JAVA TO VB CONVERTER TODO TASK: Replace 'unknown' with the appropriate dll name:
		<DllImport("unknown")> _
		Private Function canonicalizeWithPrefix0(ByVal canonicalPrefix As String, ByVal pathWithCanonicalPrefix As String) As String
		End Function

		' Best-effort attempt to get parent of this path; used for
		' optimization of filename canonicalization. This must return null for
		' any cases where the code in canonicalize_md.c would throw an
		' exception or otherwise deal with non-simple pathnames like handling
		' of "." and "..". It may conservatively return null in other
		' situations as well. Returning null will cause the underlying
		' (expensive) canonicalization routine to be called.
		Private Shared Function parentOrNull(ByVal path As String) As String
			If path Is Nothing Then Return Nothing
			Dim sep As Char = System.IO.Path.DirectorySeparatorChar
			Dim altSep As Char = "/"c
			Dim last As Integer = path.length() - 1
			Dim idx As Integer = last
			Dim adjacentDots As Integer = 0
			Dim nonDotCount As Integer = 0
			Do While idx > 0
				Dim c As Char = path.Chars(idx)
				If c = "."c Then
					adjacentDots += 1
					If adjacentDots >= 2 Then Return Nothing
					If nonDotCount = 0 Then Return Nothing
				ElseIf c = sep Then
					If adjacentDots = 1 AndAlso nonDotCount = 0 Then Return Nothing
					If idx = 0 OrElse idx >= last - 1 OrElse path.Chars(idx - 1) = sep OrElse path.Chars(idx - 1) = altSep Then Return Nothing
					Return path.Substring(0, idx)
				ElseIf c = altSep Then
					' Punt on pathnames containing both backward and
					' forward slashes
					Return Nothing
				ElseIf c = "*"c OrElse c = "?"c Then
					' Punt on pathnames containing wildcards
					Return Nothing
				Else
					nonDotCount += 1
					adjacentDots = 0
				End If
				idx -= 1
			Loop
			Return Nothing
		End Function

		' -- Attribute accessors -- 

'JAVA TO VB CONVERTER TODO TASK: Replace 'unknown' with the appropriate dll name:
		<DllImport("unknown")> _
		Public Overrides Function getBooleanAttributes(ByVal f As File) As Integer
		End Function

'JAVA TO VB CONVERTER TODO TASK: Replace 'unknown' with the appropriate dll name:
		<DllImport("unknown")> _
		Public Overrides Function checkAccess(ByVal f As File, ByVal access As Integer) As Boolean
		End Function

'JAVA TO VB CONVERTER TODO TASK: Replace 'unknown' with the appropriate dll name:
		<DllImport("unknown")> _
		Public Overrides Function getLastModifiedTime(ByVal f As File) As Long
		End Function

'JAVA TO VB CONVERTER TODO TASK: Replace 'unknown' with the appropriate dll name:
		<DllImport("unknown")> _
		Public Overrides Function getLength(ByVal f As File) As Long
		End Function

'JAVA TO VB CONVERTER TODO TASK: Replace 'unknown' with the appropriate dll name:
		<DllImport("unknown")> _
		Public Overrides Function setPermission(ByVal f As File, ByVal access As Integer, ByVal enable As Boolean, ByVal owneronly As Boolean) As Boolean
		End Function

		' -- File operations -- 

'JAVA TO VB CONVERTER TODO TASK: Replace 'unknown' with the appropriate dll name:
		<DllImport("unknown")> _
		Public Overrides Function createFileExclusively(ByVal path As String) As Boolean
		End Function

'JAVA TO VB CONVERTER TODO TASK: Replace 'unknown' with the appropriate dll name:
		<DllImport("unknown")> _
		Public Overrides Function list(ByVal f As File) As String()
		End Function

'JAVA TO VB CONVERTER TODO TASK: Replace 'unknown' with the appropriate dll name:
		<DllImport("unknown")> _
		Public Overrides Function createDirectory(ByVal f As File) As Boolean
		End Function

'JAVA TO VB CONVERTER TODO TASK: Replace 'unknown' with the appropriate dll name:
		<DllImport("unknown")> _
		Public Overrides Function setLastModifiedTime(ByVal f As File, ByVal time As Long) As Boolean
		End Function

'JAVA TO VB CONVERTER TODO TASK: Replace 'unknown' with the appropriate dll name:
		<DllImport("unknown")> _
		Public Overrides Function setReadOnly(ByVal f As File) As Boolean
		End Function

		Public Overrides Function delete(ByVal f As File) As Boolean
			' Keep canonicalization caches in sync after file deletion
			' and renaming operations. Could be more clever than this
			' (i.e., only remove/update affected entries) but probably
			' not worth it since these entries expire after 30 seconds
			' anyway.
			cache.clear()
			prefixCache.clear()
			Return delete0(f)
		End Function

'JAVA TO VB CONVERTER TODO TASK: Replace 'unknown' with the appropriate dll name:
		<DllImport("unknown")> _
		Private Function delete0(ByVal f As File) As Boolean
		End Function

		Public Overrides Function rename(ByVal f1 As File, ByVal f2 As File) As Boolean
			' Keep canonicalization caches in sync after file deletion
			' and renaming operations. Could be more clever than this
			' (i.e., only remove/update affected entries) but probably
			' not worth it since these entries expire after 30 seconds
			' anyway.
			cache.clear()
			prefixCache.clear()
			Return rename0(f1, f2)
		End Function

'JAVA TO VB CONVERTER TODO TASK: Replace 'unknown' with the appropriate dll name:
		<DllImport("unknown")> _
		Private Function rename0(ByVal f1 As File, ByVal f2 As File) As Boolean
		End Function

		' -- Filesystem interface -- 

		Public Overrides Function listRoots() As File()
			Dim ds As Integer = listRoots0()
			Dim n As Integer = 0
			For i As Integer = 0 To 25
				If ((ds >> i) And 1) <> 0 Then
					If Not access(ChrW(AscW("A"c) + i) & ":" & AscW(slash)) Then
						ds = ds And Not(1 << i)
					Else
						n += 1
					End If
				End If
			Next i
			Dim fs As File() = New File(n - 1){}
			Dim j As Integer = 0
			Dim slash_Renamed As Char = Me.slash
			For i As Integer = 0 To 25
				If ((ds >> i) And 1) <> 0 Then
					fs(j) = New File(ChrW(AscW("A"c) + i) & ":" & AscW(slash_Renamed))
					j += 1
				End If
			Next i
			Return fs
		End Function

'JAVA TO VB CONVERTER TODO TASK: Replace 'unknown' with the appropriate dll name:
		<DllImport("unknown")> _
		Private Shared Function listRoots0() As Integer
		End Function

		Private Function access(ByVal path As String) As Boolean
			Try
				Dim security As SecurityManager = System.securityManager
				If security IsNot Nothing Then security.checkRead(path)
				Return True
			Catch x As SecurityException
				Return False
			End Try
		End Function

		' -- Disk usage -- 

		Public Overrides Function getSpace(ByVal f As File, ByVal t As Integer) As Long
			If f.exists() Then Return getSpace0(f, t)
			Return 0
		End Function

'JAVA TO VB CONVERTER TODO TASK: Replace 'unknown' with the appropriate dll name:
		<DllImport("unknown")> _
		Private Function getSpace0(ByVal f As File, ByVal t As Integer) As Long
		End Function

		' -- Basic infrastructure -- 

		Public Overrides Function compare(ByVal f1 As File, ByVal f2 As File) As Integer
			Return f1.path.compareToIgnoreCase(f2.path)
		End Function

		Public Overrides Function GetHashCode(ByVal f As File) As Integer
			' Could make this more efficient: String.hashCodeIgnoreCase 
			Return f.path.ToLower(java.util.Locale.ENGLISH).GetHashCode() Xor 1234321
		End Function

'JAVA TO VB CONVERTER TODO TASK: Replace 'unknown' with the appropriate dll name:
		<DllImport("unknown")> _
		Private Shared Sub initIDs()
		End Sub

		Shared Sub New()
			initIDs()
		End Sub
	End Class

End Namespace