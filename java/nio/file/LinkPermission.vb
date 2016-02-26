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

Imports java.lang

Namespace java.nio.file


    ''' <summary>
    ''' The {@code Permission} class for link creation operations.
    ''' 
    ''' <p> The following table provides a summary description of what the permission
    ''' allows, and discusses the risks of granting code the permission.
    ''' 
    ''' <table border=1 cellpadding=5
    '''        summary="Table shows permission target name, what the permission allows, and associated risks">
    ''' <tr>
    ''' <th>Permission Target Name</th>
    ''' <th>What the Permission Allows</th>
    ''' <th>Risks of Allowing this Permission</th>
    ''' </tr>
    ''' <tr>
    '''   <td>hard</td>
    '''   <td> Ability to add an existing file to a directory. This is sometimes
    '''   known as creating a link, or hard link. </td>
    '''   <td> Extreme care should be taken when granting this permission. It allows
    '''   linking to any file or directory in the file system thus allowing the
    '''   attacker access to all files. </td>
    ''' </tr>
    ''' <tr>
    '''   <td>symbolic</td>
    '''   <td> Ability to create symbolic links. </td>
    '''   <td> Extreme care should be taken when granting this permission. It allows
    '''   linking to any file or directory in the file system thus allowing the
    '''   attacker to access to all files. </td>
    ''' </tr>
    ''' </table>
    ''' 
    ''' @since 1.7
    ''' </summary>
    ''' <seealso cref= Files#createLink </seealso>
    ''' <seealso cref= Files#createSymbolicLink </seealso>
    Public NotInheritable Class LinkPermission
        Inherits java.security.BasicPermission

        Friend Const serialVersionUID As Long = -1441492453772213220L

        Private Sub checkName(ByVal name As String)
            If (Not name.Equals("hard")) AndAlso (Not name.Equals("symbolic")) Then Throw New IllegalArgumentException("name: " & name)
        End Sub

        ''' <summary>
        ''' Constructs a {@code LinkPermission} with the specified name.
        ''' </summary>
        ''' <param name="name">
        '''          the name of the permission. It must be "hard" or "symbolic".
        ''' </param>
        ''' <exception cref="IllegalArgumentException">
        '''          if name is empty or invalid </exception>
        Public Sub New(ByVal name As String)
            MyBase.New(name)
            checkName(name)
        End Sub

        ''' <summary>
        ''' Constructs a {@code LinkPermission} with the specified name.
        ''' </summary>
        ''' <param name="name">
        '''          the name of the permission; must be "hard" or "symbolic". </param>
        ''' <param name="actions">
        '''          the actions for the permission; must be the empty string or
        '''          {@code null}
        ''' </param>
        ''' <exception cref="IllegalArgumentException">
        '''          if name is empty or invalid, or actions is a non-empty string </exception>
        Public Sub New(ByVal name As String, ByVal actions As String)
            MyBase.New(name)
            checkName(name)
            If actions IsNot Nothing AndAlso actions.Length() > 0 Then Throw New IllegalArgumentException("actions: " & actions)
        End Sub
    End Class

End Namespace