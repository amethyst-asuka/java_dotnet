Imports System
Imports System.Collections.Generic
Imports System.Runtime.InteropServices

'
' * Copyright (c) 2003, 2011, Oracle and/or its affiliates. All rights reserved.
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

' We use APIs that access a so-called Windows "Environment Block",
' * which looks like an array of jchars like this:
' *
' * FOO=BAR\u0000 ... GORP=QUUX\u0000\u0000
' *
' * This data structure has a number of peculiarities we must contend with:
' * (see: http://windowssdk.msdn.microsoft.com/en-us/library/ms682009.aspx)
' * - The NUL jchar separators, and a double NUL jchar terminator.
' *   It appears that the Windows implementation requires double NUL
' *   termination even if the environment is empty.  We should always
' *   generate environments with double NUL termination, while accepting
' *   empty environments consisting of a single NUL.
' * - on Windows9x, this is actually an array of 8-bit chars, not jchars,
' *   encoded in the system default encoding.
' * - The block must be sorted by Unicode value, case-insensitively,
' *   as if folded to upper case.
' * - There are magic environment variables maintained by Windows
' *   that start with a `=' (!) character.  These are used for
' *   Windows drive current directory (e.g. "=C:=C:\WINNT") or the
' *   exit code of the last command (e.g. "=ExitCode=0000001").
' *
' * Since Java and non-9x Windows speak the same character set, and
' * even the same encoding, we don't have to deal with unreliable
' * conversion to byte streams.  Just add a few NUL terminators.
' *
' * System.getenv(String) is case-insensitive, while System.getenv()
' * returns a map that is case-sensitive, which is consistent with
' * native Windows APIs.
' *
' * The non-private methods in this class are not for general use even
' * within this package.  Instead, they are the system-dependent parts
' * of the system-independent method of the same name.  Don't even
' * think of using this class unless your method's name appears below.
' *
' * @author Martin Buchholz
' * @since 1.5
' 

Namespace java.lang


	Friend NotInheritable Class ProcessEnvironment
		Inherits HashMap(Of String, String)

		Private Const serialVersionUID As Long = -8017839552603542824L

		Private Shared Function validateName(ByVal name As String) As String
			' An initial `=' indicates a magic Windows variable name -- OK
			If name.IndexOf("="c, 1) <> -1 OrElse name.IndexOf(ChrW(&H0000)) <> -1 Then Throw New IllegalArgumentException("Invalid environment variable name: """ & name & """")
			Return name
		End Function

		Private Shared Function validateValue(ByVal value As String) As String
			If value.IndexOf(ChrW(&H0000)) <> -1 Then Throw New IllegalArgumentException("Invalid environment variable value: """ & value & """")
			Return value
		End Function

		Private Shared Function nonNullString(ByVal o As Object) As String
			If o Is Nothing Then Throw New NullPointerException
			Return CStr(o)
		End Function

		Public Function put(ByVal key As String, ByVal value As String) As String
			Return MyBase.put(validateName(key), validateValue(value))
		End Function

		Public Function [get](ByVal key As Object) As String
			Return MyBase.get(nonNullString(key))
		End Function

		Public Function containsKey(ByVal key As Object) As Boolean
			Return MyBase.containsKey(nonNullString(key))
		End Function

		Public Function containsValue(ByVal value As Object) As Boolean
			Return MyBase.containsValue(nonNullString(value))
		End Function

		Public Function remove(ByVal key As Object) As String
			Return MyBase.remove(nonNullString(key))
		End Function

		Private Class CheckedEntry
			Implements KeyValuePair(Of String, String)

			Private ReadOnly e As KeyValuePair(Of String, String)
			Public Sub New(ByVal e As KeyValuePair(Of String, String))
				Me.e = e
			End Sub
			Public Overridable Property key As String
				Get
					Return e.Key
				End Get
			End Property
			Public Overridable Property value As String
				Get
					Return e.Value
				End Get
			End Property
			Public Overridable Function setValue(ByVal value As String) As String
				Return e.valuelue(validateValue(value))
			End Function
			Public Overrides Function ToString() As String
				Return key & "=" & value
			End Function
			Public Overrides Function Equals(ByVal o As Object) As Boolean
				Return e.Equals(o)
			End Function
			Public Overrides Function GetHashCode() As Integer
				Return e.GetHashCode()
			End Function
		End Class

		Private Class CheckedEntrySet
			Inherits AbstractSet(Of KeyValuePair(Of String, String))

			Private ReadOnly s As [Set](Of KeyValuePair(Of String, String))
			Public Sub New(ByVal s As [Set](Of KeyValuePair(Of String, String)))
				Me.s = s
			End Sub
			Public Overridable Function size() As Integer
				Return s.size()
			End Function
			Public Overridable Property empty As Boolean
				Get
					Return s.empty
				End Get
			End Property
			Public Overridable Sub clear()
				s.clear()
			End Sub
			Public Overridable Function [iterator]() As [Iterator](Of KeyValuePair(Of String, String))
				Return New IteratorAnonymousInnerClassHelper(Of E)
			End Function

			Private Class IteratorAnonymousInnerClassHelper(Of E)
				Implements Iterator(Of E)

				Friend i As [Iterator](Of KeyValuePair(Of String, String)) = outerInstance.s.GetEnumerator()
				Public Overridable Function hasNext() As Boolean Implements Iterator(Of E).hasNext
					Return i.hasNext()
				End Function
				Public Overridable Function [next]() As KeyValuePair(Of String, String)
					Return New CheckedEntry(i.next())
				End Function
				Public Overridable Sub remove() Implements Iterator(Of E).remove
					i.remove()
				End Sub
			End Class
			Private Shared Function checkedEntry(ByVal o As Object) As KeyValuePair(Of String, String)
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
				Dim e As KeyValuePair(Of String, String) = CType(o, KeyValuePair(Of String, String))
				nonNullString(e.Key)
				nonNullString(e.Value)
				Return e
			End Function
			Public Overridable Function contains(ByVal o As Object) As Boolean
				Return s.contains(checkedEntry(o))
			End Function
			Public Overridable Function remove(ByVal o As Object) As Boolean
				Return s.remove(checkedEntry(o))
			End Function
		End Class

		Private Class CheckedValues
			Inherits AbstractCollection(Of String)

			Private ReadOnly c As Collection(Of String)
			Public Sub New(ByVal c As Collection(Of String))
				Me.c = c
			End Sub
			Public Overridable Function size() As Integer
				Return c.size()
			End Function
			Public Overridable Property empty As Boolean
				Get
					Return c.empty
				End Get
			End Property
			Public Overridable Sub clear()
				c.clear()
			End Sub
			Public Overridable Function [iterator]() As [Iterator](Of String)
				Return c.GetEnumerator()
			End Function
			Public Overridable Function contains(ByVal o As Object) As Boolean
				Return c.contains(nonNullString(o))
			End Function
			Public Overridable Function remove(ByVal o As Object) As Boolean
				Return c.remove(nonNullString(o))
			End Function
		End Class

		Private Class CheckedKeySet
			Inherits AbstractSet(Of String)

			Private ReadOnly s As [Set](Of String)
			Public Sub New(ByVal s As [Set](Of String))
				Me.s = s
			End Sub
			Public Overridable Function size() As Integer
				Return s.size()
			End Function
			Public Overridable Property empty As Boolean
				Get
					Return s.empty
				End Get
			End Property
			Public Overridable Sub clear()
				s.clear()
			End Sub
			Public Overridable Function [iterator]() As [Iterator](Of String)
				Return s.GetEnumerator()
			End Function
			Public Overridable Function contains(ByVal o As Object) As Boolean
				Return s.contains(nonNullString(o))
			End Function
			Public Overridable Function remove(ByVal o As Object) As Boolean
				Return s.remove(nonNullString(o))
			End Function
		End Class

		Public Function keySet() As [Set](Of String)
			Return New CheckedKeySet(MyBase.Keys)
		End Function

		Public Function values() As Collection(Of String)
			Return New CheckedValues(MyBase.values())
		End Function

		Public Function entrySet() As [Set](Of KeyValuePair(Of String, String))
			Return New CheckedEntrySet(MyBase.entrySet())
		End Function


		Private NotInheritable Class NameComparator
			Implements Comparator(Of String)

			Public Function compare(ByVal s1 As String, ByVal s2 As String) As Integer Implements Comparator(Of String).compare
				' We can't use String.compareToIgnoreCase since it
				' canonicalizes to lower case, while Windows
				' canonicalizes to upper case!  For example, "_" should
				' sort *after* "Z", not before.
				Dim n1 As Integer = s1.length()
				Dim n2 As Integer = s2.length()
				Dim min As Integer = Math.Min(n1, n2)
				For i As Integer = 0 To min - 1
					Dim c1 As Char = s1.Chars(i)
					Dim c2 As Char = s2.Chars(i)
					If c1 <> c2 Then
						c1 = Char.ToUpper(c1)
						c2 = Char.ToUpper(c2)
						If c1 <> c2 Then Return AscW(c1) - AscW(c2)
					End If
				Next i
				Return n1 - n2
			End Function
		End Class

		Private NotInheritable Class EntryComparator
			Implements Comparator(Of KeyValuePair(Of String, String))

			Public Function compare(ByVal e1 As KeyValuePair(Of String, String), ByVal e2 As KeyValuePair(Of String, String)) As Integer
				Return nameComparator.Compare(e1.Key, e2.Key)
			End Function
		End Class

		' Allow `=' as first char in name, e.g. =C:=C:\DIR
		Friend Const MIN_NAME_LENGTH As Integer = 1

		Private Shared ReadOnly nameComparator As NameComparator
		Private Shared ReadOnly entryComparator As EntryComparator
		Private Shared ReadOnly theEnvironment As ProcessEnvironment
		Private Shared ReadOnly theUnmodifiableEnvironment As Map(Of String, String)
		Private Shared ReadOnly theCaseInsensitiveEnvironment As Map(Of String, String)

		Shared Sub New()
			nameComparator = New NameComparator
			entryComparator = New EntryComparator
			theEnvironment = New ProcessEnvironment
			theUnmodifiableEnvironment = Collections.unmodifiableMap(theEnvironment)

			Dim envblock As String = environmentBlock()
			Dim beg, [end], eql As Integer
			beg = 0
'JAVA TO VB CONVERTER TODO TASK: Assignments within expressions are not supported in VB
			Do While (([end] = envblock.IndexOf(ChrW(&H0000), beg)) <> -1 AndAlso (eql = envblock.IndexOf("="c, beg+1)) <> -1)
				  ' An initial `=' indicates a magic Windows variable name -- OK
				' Ignore corrupted environment strings.
				If eql < [end] Then theEnvironment.put(envblock.Substring(beg, eql - beg), envblock.Substring(eql+1, [end] - (eql+1)))
				beg = [end] + 1
			Loop

			theCaseInsensitiveEnvironment = New TreeMap(Of )(nameComparator)
			theCaseInsensitiveEnvironment.putAll(theEnvironment)
		End Sub

		Private Sub New()
			MyBase.New()
		End Sub

		Private Sub New(ByVal capacity As Integer)
			MyBase.New(capacity)
		End Sub

		' Only for use by System.getenv(String)
		Friend Shared Function getenv(ByVal name As String) As String
			' The original implementation used a native call to _wgetenv,
			' but it turns out that _wgetenv is only consistent with
			' GetEnvironmentStringsW (for non-ASCII) if `wmain' is used
			' instead of `main', even in a process created using
			' CREATE_UNICODE_ENVIRONMENT.  Instead we perform the
			' case-insensitive comparison ourselves.  At least this
			' guarantees that System.getenv().get(String) will be
			' consistent with System.getenv(String).
			Return theCaseInsensitiveEnvironment.get(name)
		End Function

		' Only for use by System.getenv()
		Friend Shared Function getenv() As Map(Of String, String)
			Return theUnmodifiableEnvironment
		End Function

		' Only for use by ProcessBuilder.environment()
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
		Friend Shared Function environment() As Map(Of String, String)
			Return CType(theEnvironment.clone(), Map(Of String, String))
		End Function

		' Only for use by ProcessBuilder.environment(String[] envp)
		Friend Shared Function emptyEnvironment(ByVal capacity As Integer) As Map(Of String, String)
			Return New ProcessEnvironment(capacity)
		End Function

'JAVA TO VB CONVERTER TODO TASK: Replace 'unknown' with the appropriate dll name:
		<DllImport("unknown")> _
		Private Shared Function environmentBlock() As String
		End Function

		' Only for use by ProcessImpl.start()
		Friend Function toEnvironmentBlock() As String
			' Sort Unicode-case-insensitively by name
			Dim list As List(Of KeyValuePair(Of String, String)) = New List(Of KeyValuePair(Of String, String))(entrySet())
			Collections.sort(list, entryComparator)

			Dim sb As New StringBuilder(size()*30)
			Dim cmp As Integer = -1

			' Some versions of MSVCRT.DLL require SystemRoot to be set.
			' So, we make sure that it is always set, even if not provided
			' by the caller.
			Const SYSTEMROOT As String = "SystemRoot"

			For Each e As KeyValuePair(Of String, String) In list
				Dim key As String = e.Key
				Dim value As String = e.Value
				cmp = nameComparator.Compare(key, SYSTEMROOT)
				If cmp < 0 AndAlso cmp > 0 Then addToEnvIfSet(sb, SYSTEMROOT)
				addToEnv(sb, key, value)
			Next e
			If cmp < 0 Then addToEnvIfSet(sb, SYSTEMROOT)
			If sb.length() = 0 Then sb.append(ChrW(&H0000))
			' Block is double NUL terminated
			sb.append(ChrW(&H0000))
			Return sb.ToString()
		End Function

		' add the environment variable to the child, if it exists in parent
		Private Shared Sub addToEnvIfSet(ByVal sb As StringBuilder, ByVal name As String)
			Dim s As String = getenv(name)
			If s IsNot Nothing Then addToEnv(sb, name, s)
		End Sub

		Private Shared Sub addToEnv(ByVal sb As StringBuilder, ByVal name As String, ByVal val As String)
			sb.append(name).append("="c).append(val).append(ChrW(&H0000))
		End Sub

		Friend Shared Function toEnvironmentBlock(ByVal map As Map(Of String, String)) As String
			Return If(map Is Nothing, Nothing, CType(map, ProcessEnvironment).toEnvironmentBlock())
		End Function
	End Class

End Namespace