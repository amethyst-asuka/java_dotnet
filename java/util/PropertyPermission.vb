Imports Microsoft.VisualBasic
Imports System
Imports System.Runtime.CompilerServices
Imports System.Collections
Imports System.Collections.Generic
Imports java.security

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

Namespace java.util


    ''' <summary>
    ''' This class is for property permissions.
    ''' 
    ''' <P>
    ''' The name is the name of the property ("java.home",
    ''' "os.name", etc). The naming
    ''' convention follows the  hierarchical property naming convention.
    ''' Also, an asterisk
    ''' may appear at the end of the name, following a ".", or by itself, to
    ''' signify a wildcard match. For example: "java.*" and "*" signify a wildcard
    ''' match, while "*java" and "a*b" do not.
    ''' <P>
    ''' The actions to be granted are passed to the constructor in a string containing
    ''' a list of one or more comma-separated keywords. The possible keywords are
    ''' "read" and "write". Their meaning is defined as follows:
    ''' 
    ''' <DL>
    '''    <DT> read
    '''    <DD> read permission. Allows <code>System.getProperty</code> to
    '''         be called.
    '''    <DT> write
    '''    <DD> write permission. Allows <code>System.setProperty</code> to
    '''         be called.
    ''' </DL>
    ''' <P>
    ''' The actions string is converted to lowercase before processing.
    ''' <P>
    ''' Care should be taken before granting code permission to access
    ''' certain system properties.  For example, granting permission to
    ''' access the "java.home" system property gives potentially malevolent
    ''' code sensitive information about the system environment (the Java
    ''' installation directory).  Also, granting permission to access
    ''' the "user.name" and "user.home" system properties gives potentially
    ''' malevolent code sensitive information about the user environment
    ''' (the user's account name and home directory).
    ''' </summary>
    ''' <seealso cref= java.security.BasicPermission </seealso>
    ''' <seealso cref= java.security.Permission </seealso>
    ''' <seealso cref= java.security.Permissions </seealso>
    ''' <seealso cref= java.security.PermissionCollection </seealso>
    ''' <seealso cref= java.lang.SecurityManager
    ''' 
    ''' 
    ''' @author Roland Schemers
    ''' @since 1.2
    ''' 
    ''' @serial exclude </seealso>

    Public NotInheritable Class PropertyPermission
        Inherits BasicPermission

        ''' <summary>
        ''' Read action.
        ''' </summary>
        Private Const READ As Integer = &H1

        ''' <summary>
        ''' Write action.
        ''' </summary>
        Private Const WRITE As Integer = &H2
        ''' <summary>
        ''' All actions (read,write);
        ''' </summary>
        Private Shared ReadOnly ALL As Integer = READ Or WRITE
        ''' <summary>
        ''' No actions.
        ''' </summary>
        Private Const NONE As Integer = &H0

        ''' <summary>
        ''' The actions string.
        ''' 
        ''' @serial
        ''' </summary>
        Private _actions As String ' Left null as long as possible, then
        ' created and re-used in the getAction function.

        ''' <summary>
        ''' initialize a PropertyPermission object. Common to all constructors.
        ''' Also called during de-serialization.
        ''' </summary>
        ''' <param name="mask"> the actions mask to use.
        '''  </param>
        Private Sub init(ByVal mask As Integer)
            If (mask And ALL) <> mask Then Throw New IllegalArgumentException("invalid actions mask")

            If mask = NONE Then Throw New IllegalArgumentException("invalid actions mask")

            If name Is Nothing Then Throw New NullPointerException("name can't be null")

            Me.mask = mask
        End Sub

        ''' <summary>
        ''' Creates a new PropertyPermission object with the specified name.
        ''' The name is the name of the system property, and
        ''' <i>actions</i> contains a comma-separated list of the
        ''' desired actions granted on the property. Possible actions are
        ''' "read" and "write".
        ''' </summary>
        ''' <param name="name"> the name of the PropertyPermission. </param>
        ''' <param name="actions"> the actions string.
        ''' </param>
        ''' <exception cref="NullPointerException"> if <code>name</code> is <code>null</code>. </exception>
        ''' <exception cref="IllegalArgumentException"> if <code>name</code> is empty or if
        ''' <code>actions</code> is invalid. </exception>
        Public Sub New(ByVal name As String, ByVal actions As String)
            MyBase.New(name, actions)
            init(getMask(actions))
        End Sub

        ''' <summary>
        ''' Checks if this PropertyPermission object "implies" the specified
        ''' permission.
        ''' <P>
        ''' More specifically, this method returns true if:
        ''' <ul>
        ''' <li> <i>p</i> is an instanceof PropertyPermission,
        ''' <li> <i>p</i>'s actions are a subset of this
        ''' object's actions, and
        ''' <li> <i>p</i>'s name is implied by this object's
        '''      name. For example, "java.*" implies "java.home".
        ''' </ul> </summary>
        ''' <param name="p"> the permission to check against.
        ''' </param>
        ''' <returns> true if the specified permission is implied by this object,
        ''' false if not. </returns>
        Public Overrides Function implies(ByVal p As Permission) As Boolean
            If Not (TypeOf p Is PropertyPermission) Then Return False

            Dim that As PropertyPermission = CType(p, PropertyPermission)

            ' we get the effective mask. i.e., the "and" of this and that.
            ' They must be equal to that.mask for implies to return true.

            Return ((Me.mask And that.mask) = that.mask) AndAlso MyBase.implies(that)
        End Function

        ''' <summary>
        ''' Checks two PropertyPermission objects for equality. Checks that <i>obj</i> is
        ''' a PropertyPermission, and has the same name and actions as this object.
        ''' <P> </summary>
        ''' <param name="obj"> the object we are testing for equality with this object. </param>
        ''' <returns> true if obj is a PropertyPermission, and has the same name and
        ''' actions as this PropertyPermission object. </returns>
        Public Overrides Function Equals(ByVal obj As Object) As Boolean
            If obj Is Me Then Return True

            If Not (TypeOf obj Is PropertyPermission) Then Return False

            Dim that As PropertyPermission = CType(obj, PropertyPermission)

            Return (Me.mask = that.mask) AndAlso (Me.name.Equals(that.name))
        End Function

        ''' <summary>
        ''' Returns the hash code value for this object.
        ''' The hash code used is the hash code of this permissions name, that is,
        ''' <code>getName().hashCode()</code>, where <code>getName</code> is
        ''' from the Permission superclass.
        ''' </summary>
        ''' <returns> a hash code value for this object. </returns>
        Public Overrides Function GetHashCode() As Integer
            Return Me.name.GetHashCode()
        End Function

        ''' <summary>
        ''' Converts an actions String to an actions mask.
        ''' </summary>
        ''' <param name="actions"> the action string. </param>
        ''' <returns> the actions mask. </returns>
        Private Shared Function getMask(ByVal actions As String) As Integer

            Dim mask_Renamed As Integer = NONE

            If actions Is Nothing Then Return mask_Renamed

            ' Use object identity comparison against known-interned strings for
            ' performance benefit (these values are used heavily within the JDK).
            If actions = sun.security.util.SecurityConstants.PROPERTY_READ_ACTION Then Return READ
            If actions = sun.security.util.SecurityConstants.PROPERTY_WRITE_ACTION Then
                Return WRITE
            ElseIf actions = sun.security.util.SecurityConstants.PROPERTY_RW_ACTION Then
                Return READ Or WRITE
            End If

            Dim a As Char() = actions.ToCharArray()

            Dim i As Integer = a.Length - 1
            If i < 0 Then Return mask_Renamed

            Do While i <> -1
                Dim c As Char

                ' skip whitespace
                c = a(i)
                Do While (i <> -1) AndAlso (c = " "c OrElse c = ControlChars.Cr OrElse c = ControlChars.Lf OrElse c = ControlChars.FormFeed OrElse c = ControlChars.Tab)
                    i -= 1
                    c = a(i)
                Loop

                ' check for the known strings
                Dim matchlen As Integer

                If i >= 3 AndAlso (a(i - 3) = "r"c OrElse a(i - 3) = "R"c) AndAlso (a(i - 2) = "e"c OrElse a(i - 2) = "E"c) AndAlso (a(i - 1) = "a"c OrElse a(i - 1) = "A"c) AndAlso (a(i) = "d"c OrElse a(i) = "D"c) Then
                    matchlen = 4
                    mask_Renamed = mask_Renamed Or READ

                ElseIf i >= 4 AndAlso (a(i - 4) = "w"c OrElse a(i - 4) = "W"c) AndAlso (a(i - 3) = "r"c OrElse a(i - 3) = "R"c) AndAlso (a(i - 2) = "i"c OrElse a(i - 2) = "I"c) AndAlso (a(i - 1) = "t"c OrElse a(i - 1) = "T"c) AndAlso (a(i) = "e"c OrElse a(i) = "E"c) Then
                    matchlen = 5
                    mask_Renamed = mask_Renamed Or WRITE

                Else
                    ' parse error
                    Throw New IllegalArgumentException("invalid permission: " & actions)
                End If

                ' make sure we didn't just match the tail of a word
                ' like "ackbarfaccept".  Also, skip to the comma.
                Dim seencomma As Boolean = False
                Do While i >= matchlen AndAlso Not seencomma
                    Select Case a(i - matchlen)
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
        ''' Return the canonical string representation of the actions.
        ''' Always returns present actions in the following order:
        ''' read, write.
        ''' </summary>
        ''' <returns> the canonical string representation of the actions. </returns>
        Friend Shared Function getActions(ByVal mask As Integer) As String
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
            Return sb.ToString()
        End Function

        ''' <summary>
        ''' Returns the "canonical string representation" of the actions.
        ''' That is, this method always returns present actions in the following order:
        ''' read, write. For example, if this PropertyPermission object
        ''' allows both write and read actions, a call to <code>getActions</code>
        ''' will return the string "read,write".
        ''' </summary>
        ''' <returns> the canonical string representation of the actions. </returns>
        Public Overrides ReadOnly Property actions As String
            Get
                If _actions Is Nothing Then _actions = getActions(Me.mask)
                Return _actions
            End Get
        End Property

        ''' <summary>
        ''' Return the current action mask.
        ''' Used by the PropertyPermissionCollection
        ''' </summary>
        ''' <returns> the actions mask. </returns>
        Friend Property mask As Integer

        ''' <summary>
        ''' Returns a new PermissionCollection object for storing
        ''' PropertyPermission objects.
        ''' <p>
        ''' </summary>
        ''' <returns> a new PermissionCollection object suitable for storing
        ''' PropertyPermissions. </returns>
        Public Overrides Function newPermissionCollection() As PermissionCollection
            Return New PropertyPermissionCollection
        End Function


        Private Const serialVersionUID As Long = 885438825399942851L

        ''' <summary>
        ''' WriteObject is called to save the state of the PropertyPermission
        ''' to a stream. The actions are serialized, and the superclass
        ''' takes care of the name.
        ''' </summary>
        <MethodImpl(MethodImplOptions.Synchronized)>
        Private Sub writeObject(ByVal s As java.io.ObjectOutputStream)
            ' Write out the actions. The superclass takes care of the name
            ' call getActions to make sure actions field is initialized
            If actions Is Nothing Then actions
            s.defaultWriteObject()
        End Sub

        ''' <summary>
        ''' readObject is called to restore the state of the PropertyPermission from
        ''' a stream.
        ''' </summary>
        <MethodImpl(MethodImplOptions.Synchronized)>
        Private Sub readObject(ByVal s As java.io.ObjectInputStream)
            ' Read in the action, then initialize the rest
            s.defaultReadObject()
            init(getMask(actions))
        End Sub
    End Class

    ''' <summary>
    ''' A PropertyPermissionCollection stores a set of PropertyPermission
    ''' permissions.
    ''' </summary>
    ''' <seealso cref= java.security.Permission </seealso>
    ''' <seealso cref= java.security.Permissions </seealso>
    ''' <seealso cref= java.security.PermissionCollection
    ''' 
    ''' 
    ''' @author Roland Schemers
    ''' 
    ''' @serial include </seealso>
    <Serializable>
    Friend NotInheritable Class PropertyPermissionCollection
        Inherits PermissionCollection

        ''' <summary>
        ''' Key is property name; value is PropertyPermission.
        ''' Not serialized; see serialization section at end of class.
        ''' </summary>
        <NonSerialized>
        Private perms As IDictionary(Of String, PropertyPermission)

        ''' <summary>
        ''' Boolean saying if "*" is in the collection.
        ''' </summary>
        ''' <seealso cref= #serialPersistentFields </seealso>
        ' No sync access; OK for this to be stale.
        Private all_allowed As Boolean

        ''' <summary>
        ''' Create an empty PropertyPermissionCollection object.
        ''' </summary>
        Public Sub New()
            perms = New Dictionary(Of )(32) ' Capacity for default policy
            all_allowed = False
        End Sub

        ''' <summary>
        ''' Adds a permission to the PropertyPermissions. The key for the hash is
        ''' the name.
        ''' </summary>
        ''' <param name="permission"> the Permission object to add.
        ''' </param>
        ''' <exception cref="IllegalArgumentException"> - if the permission is not a
        '''                                       PropertyPermission
        ''' </exception>
        ''' <exception cref="SecurityException"> - if this PropertyPermissionCollection
        '''                                object has been marked readonly </exception>
        Public Overrides Sub add(ByVal permission As Permission)
            If Not (TypeOf permission Is PropertyPermission) Then Throw New IllegalArgumentException("invalid permission: " & permission)
            If [readOnly] Then Throw New SecurityException("attempt to add a Permission to a readonly PermissionCollection")

            Dim pp As PropertyPermission = CType(permission, PropertyPermission)
            Dim propName As String = pp.name

            SyncLock Me
                Dim existing As PropertyPermission = perms(propName)

                If existing IsNot Nothing Then
                    Dim oldMask As Integer = existing.mask
                    Dim newMask As Integer = pp.mask
                    If oldMask <> newMask Then
                        Dim effective As Integer = oldMask Or newMask
                        Dim actions As String = PropertyPermission.getActions(effective)
                        perms(propName) = New PropertyPermission(propName, actions)
                    End If
                Else
                    perms(propName) = pp
                End If
            End SyncLock

            If Not all_allowed Then
                If propName.Equals("*") Then all_allowed = True
            End If
        End Sub

        ''' <summary>
        ''' Check and see if this set of permissions implies the permissions
        ''' expressed in "permission".
        ''' </summary>
        ''' <param name="permission"> the Permission object to compare
        ''' </param>
        ''' <returns> true if "permission" is a proper subset of a permission in
        ''' the set, false if not. </returns>
        Public Overrides Function implies(ByVal permission As Permission) As Boolean
            If Not (TypeOf permission Is PropertyPermission) Then Return False

            Dim pp As PropertyPermission = CType(permission, PropertyPermission)
            Dim x As PropertyPermission

            Dim desired As Integer = pp.mask
            Dim effective As Integer = 0

            ' short circuit if the "*" Permission was added
            If all_allowed Then
                SyncLock Me
                    x = perms("*")
                End SyncLock
                If x IsNot Nothing Then
                    effective = effective Or x.mask
                    If (effective And desired) = desired Then Return True
                End If
            End If

            ' strategy:
            ' Check for full match first. Then work our way up the
            ' name looking for matches on a.b.*

            Dim name As String = pp.name
            'System.out.println("check "+name);

            SyncLock Me
                x = perms(name)
            End SyncLock

            If x IsNot Nothing Then
                ' we have a direct hit!
                effective = effective Or x.mask
                If (effective And desired) = desired Then Return True
            End If

            ' work our way up the tree...
            Dim last, offset As Integer

            offset = name.Length() - 1

            last = name.LastIndexOf(".", offset)
            Do While last <> -1

                name = name.Substring(0, last + 1) & "*"
                'System.out.println("check "+name);
                SyncLock Me
                    x = perms(name)
                End SyncLock

                If x IsNot Nothing Then
                    effective = effective Or x.mask
                    If (effective And desired) = desired Then Return True
                End If
                offset = last - 1
                last = name.LastIndexOf(".", offset)
            Loop

            ' we don't have to check for "*" as it was already checked
            ' at the top (all_allowed), so we just return false
            Return False
        End Function

        ''' <summary>
        ''' Returns an enumeration of all the PropertyPermission objects in the
        ''' container.
        ''' </summary>
        ''' <returns> an enumeration of all the PropertyPermission objects. </returns>
        'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
        Public Overrides Function elements() As IEnumerator(Of Permission)
            ' Convert Iterator of Map values into an Enumeration
            SyncLock Me
                ''' <summary>
                ''' Casting to rawtype since Enumeration<PropertyPermission>
                ''' cannot be directly cast to Enumeration<Permission>
                ''' </summary>
                Return CType(java.util.Collections.enumeration(perms.Values), System.Collections.IEnumerator)
            End SyncLock
        End Function

        Private Const serialVersionUID As Long = 7015263904581634791L

        ' Need to maintain serialization interoperability with earlier releases,
        ' which had the serializable field:
        '
        ' Table of permissions.
        '
        ' @serial
        '
        ' private Hashtable permissions;
        ''' <summary>
        ''' @serialField permissions java.util.Hashtable
        '''     A table of the PropertyPermissions.
        ''' @serialField all_allowed boolean
        '''     boolean saying if "*" is in the collection.
        ''' </summary>
        Private Shared ReadOnly serialPersistentFields As java.io.ObjectStreamField() = {New java.io.ObjectStreamField("permissions", GetType(Hashtable)), New java.io.ObjectStreamField("all_allowed", java.lang.[Boolean].TYPE)}

        ''' <summary>
        ''' @serialData Default fields.
        ''' </summary>
        '    
        '     * Writes the contents of the perms field out as a Hashtable for
        '     * serialization compatibility with earlier releases. all_allowed
        '     * unchanged.
        '     
        Private Sub writeObject(ByVal out As java.io.ObjectOutputStream)
            ' Don't call out.defaultWriteObject()

            ' Copy perms into a Hashtable
            Dim permissions As New Dictionary(Of String, Permission)(perms.Count * 2)
            SyncLock Me
                permissions.putAll(perms)
            End SyncLock

            ' Write out serializable fields
            Dim pfields As java.io.ObjectOutputStream.PutField = out.putFields()
            pfields.put("all_allowed", all_allowed)
            pfields.put("permissions", permissions)
            out.writeFields()
        End Sub

        '    
        '     * Reads in a Hashtable of PropertyPermissions and saves them in the
        '     * perms field. Reads in all_allowed.
        '     
        Private Sub readObject(ByVal [in] As java.io.ObjectInputStream)
            ' Don't call defaultReadObject()

            ' Read in serialized fields
            Dim gfields As java.io.ObjectInputStream.GetField = [in].readFields()

            ' Get all_allowed
            all_allowed = gfields.get("all_allowed", False)

            ' Get permissions
            'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
            Dim permissions As Dictionary(Of String, PropertyPermission) = CType(gfields.get("permissions", Nothing), Dictionary(Of String, PropertyPermission))
            perms = New Dictionary(Of )(permissions.size() * 2)
            'JAVA TO VB CONVERTER TODO TASK: There is no .NET Dictionary equivalent to the Java 'putAll' method:
            perms.putAll(permissions)
        End Sub
    End Class

End Namespace