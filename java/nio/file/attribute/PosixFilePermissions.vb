Imports System.Collections.Generic
Imports java.lang

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

Namespace java.nio.file.attribute


    ''' <summary>
    ''' This class consists exclusively of static methods that operate on sets of
    ''' <seealso cref="PosixFilePermission"/> objects.
    ''' 
    ''' @since 1.7
    ''' </summary>

    Public NotInheritable Class PosixFilePermissions
        Private Sub New()
        End Sub

        ' Write string representation of permission bits to {@code sb}.
        Private Shared Sub writeBits(ByVal sb As StringBuilder, ByVal r As Boolean, ByVal w As Boolean, ByVal x As Boolean)
            If r Then
                sb.append("r"c)
            Else
                sb.append("-"c)
            End If
            If w Then
                sb.append("w"c)
            Else
                sb.append("-"c)
            End If
            If x Then
                sb.append("x"c)
            Else
                sb.append("-"c)
            End If
        End Sub

        ''' <summary>
        ''' Returns the {@code String} representation of a set of permissions. It
        ''' is guaranteed that the returned {@code String} can be parsed by the
        ''' <seealso cref="#fromString"/> method.
        ''' 
        ''' <p> If the set contains {@code null} or elements that are not of type
        ''' {@code PosixFilePermission} then these elements are ignored.
        ''' </summary>
        ''' <param name="perms">
        '''          the set of permissions
        ''' </param>
        ''' <returns>  the string representation of the permission set </returns>
        Public Shared Function ToString(ByVal perms As [Set](Of PosixFilePermission)) As String
            Dim sb As New StringBuilder(9)
            writeBits(sb, perms.contains(OWNER_READ), perms.contains(OWNER_WRITE), perms.contains(OWNER_EXECUTE))
            writeBits(sb, perms.contains(GROUP_READ), perms.contains(GROUP_WRITE), perms.contains(GROUP_EXECUTE))
            writeBits(sb, perms.contains(OTHERS_READ), perms.contains(OTHERS_WRITE), perms.contains(OTHERS_EXECUTE))
            Return sb.ToString()
        End Function

        Private Shared Function isSet(ByVal c As Char, ByVal setValue As Char) As Boolean
            If c = valuelue Then Return True
            If c = "-"c Then Return False
            Throw New IllegalArgumentException("Invalid mode")
        End Function
        Private Shared Function isR(ByVal c As Char) As Boolean
            Return isSet(c, "r"c)
        End Function
        Private Shared Function isW(ByVal c As Char) As Boolean
            Return isSet(c, "w"c)
        End Function
        Private Shared Function isX(ByVal c As Char) As Boolean
            Return isSet(c, "x"c)
        End Function

        ''' <summary>
        ''' Returns the set of permissions corresponding to a given {@code String}
        ''' representation.
        ''' 
        ''' <p> The {@code perms} parameter is a {@code String} representing the
        ''' permissions. It has 9 characters that are interpreted as three sets of
        ''' three. The first set refers to the owner's permissions; the next to the
        ''' group permissions and the last to others. Within each set, the first
        ''' character is {@code 'r'} to indicate permission to read, the second
        ''' character is {@code 'w'} to indicate permission to write, and the third
        ''' character is {@code 'x'} for execute permission. Where a permission is
        ''' not set then the corresponding character is set to {@code '-'}.
        ''' 
        ''' <p> <b>Usage Example:</b>
        ''' Suppose we require the set of permissions that indicate the owner has read,
        ''' write, and execute permissions, the group has read and execute permissions
        ''' and others have none.
        ''' <pre>
        '''   Set&lt;PosixFilePermission&gt; perms = PosixFilePermissions.fromString("rwxr-x---");
        ''' </pre>
        ''' </summary>
        ''' <param name="perms">
        '''          string representing a set of permissions
        ''' </param>
        ''' <returns>  the resulting set of permissions
        ''' </returns>
        ''' <exception cref="IllegalArgumentException">
        '''          if the string cannot be converted to a set of permissions
        ''' </exception>
        ''' <seealso cref= #toString(Set) </seealso>
        Public Shared Function fromString(ByVal perms As String) As [Set](Of PosixFilePermission)
            If perms.Length() <> 9 Then Throw New IllegalArgumentException("Invalid mode")
            Dim result As [Set](Of PosixFilePermission) = EnumSet.noneOf(GetType(PosixFilePermission))
            If isR(perms.Chars(0)) Then result.add(OWNER_READ)
            If isW(perms.Chars(1)) Then result.add(OWNER_WRITE)
            If isX(perms.Chars(2)) Then result.add(OWNER_EXECUTE)
            If isR(perms.Chars(3)) Then result.add(GROUP_READ)
            If isW(perms.Chars(4)) Then result.add(GROUP_WRITE)
            If isX(perms.Chars(5)) Then result.add(GROUP_EXECUTE)
            If isR(perms.Chars(6)) Then result.add(OTHERS_READ)
            If isW(perms.Chars(7)) Then result.add(OTHERS_WRITE)
            If isX(perms.Chars(8)) Then result.add(OTHERS_EXECUTE)
            Return result
        End Function

        ''' <summary>
        ''' Creates a <seealso cref="FileAttribute"/>, encapsulating a copy of the given file
        ''' permissions, suitable for passing to the {@link java.nio.file.Files#createFile
        ''' createFile} or <seealso cref="java.nio.file.Files#createDirectory createDirectory"/>
        ''' methods.
        ''' </summary>
        ''' <param name="perms">
        '''          the set of permissions
        ''' </param>
        ''' <returns>  an attribute encapsulating the given file permissions with
        '''          <seealso cref="FileAttribute#name name"/> {@code "posix:permissions"}
        ''' </returns>
        ''' <exception cref="ClassCastException">
        '''          if the set contains elements that are not of type {@code
        '''          PosixFilePermission} </exception>
        Public Shared Function asFileAttribute(ByVal perms As [Set](Of PosixFilePermission)) As FileAttribute(Of [Set](Of PosixFilePermission))
            ' copy set and check for nulls (CCE will be thrown if an element is not
            ' a PosixFilePermission)
            perms = New HashSet(Of PosixFilePermission)(perms)
            For Each p As PosixFilePermission In perms
                If p Is Nothing Then Throw New NullPointerException
            Next p
            Dim value As [Set](Of PosixFilePermission) = perms
            Return New FileAttributeAnonymousInnerClassHelper(Of T)
        End Function

        Private Class FileAttributeAnonymousInnerClassHelper(Of T)
            Implements FileAttribute(Of T)

            Public Overrides Function name() As String Implements FileAttribute(Of T).name
                Return "posix:permissions"
            End Function
            Public Overrides Function value() As [Set](Of PosixFilePermission)
                Return Collections.unmodifiableSet(value)
            End Function
        End Class
    End Class

End Namespace