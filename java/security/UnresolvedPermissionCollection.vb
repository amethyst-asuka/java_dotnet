Imports System
Imports System.Collections
Imports System.Collections.Generic

'
' * Copyright (c) 1997, 2011, Oracle and/or its affiliates. All rights reserved.
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

Namespace java.security


	''' <summary>
	''' A UnresolvedPermissionCollection stores a collection
	''' of UnresolvedPermission permissions.
	''' </summary>
	''' <seealso cref= java.security.Permission </seealso>
	''' <seealso cref= java.security.Permissions </seealso>
	''' <seealso cref= java.security.UnresolvedPermission
	''' 
	''' 
	''' @author Roland Schemers
	''' 
	''' @serial include </seealso>

	<Serializable> _
	Friend NotInheritable Class UnresolvedPermissionCollection
		Inherits PermissionCollection

		''' <summary>
		''' Key is permission type, value is a list of the UnresolvedPermissions
		''' of the same type.
		''' Not serialized; see serialization section at end of class.
		''' </summary>
		<NonSerialized> _
		Private perms As Map(Of String, List(Of UnresolvedPermission))

		''' <summary>
		''' Create an empty UnresolvedPermissionCollection object.
		''' 
		''' </summary>
		Public Sub New()
			perms = New HashMap(Of String, List(Of UnresolvedPermission))(11)
		End Sub

		''' <summary>
		''' Adds a permission to this UnresolvedPermissionCollection.
		''' The key for the hash is the unresolved permission's type (class) name.
		''' </summary>
		''' <param name="permission"> the Permission object to add. </param>

		Public Overrides Sub add(  permission As Permission)
			If Not(TypeOf permission Is UnresolvedPermission) Then Throw New IllegalArgumentException("invalid permission: " & permission)
			Dim up As UnresolvedPermission = CType(permission, UnresolvedPermission)

			Dim v As List(Of UnresolvedPermission)
			SyncLock Me
				v = perms.get(up.name)
				If v Is Nothing Then
					v = New List(Of UnresolvedPermission)
					perms.put(up.name, v)
				End If
			End SyncLock
			SyncLock v
				v.add(up)
			End SyncLock
		End Sub

		''' <summary>
		''' get any unresolved permissions of the same type as p,
		''' and return the List containing them.
		''' </summary>
		Friend Function getUnresolvedPermissions(  p As Permission) As List(Of UnresolvedPermission)
			SyncLock Me
				Return perms.get(p.GetType().name)
			End SyncLock
		End Function

		''' <summary>
		''' always returns false for unresolved permissions
		''' 
		''' </summary>
		Public Overrides Function implies(  permission As Permission) As Boolean
			Return False
		End Function

		''' <summary>
		''' Returns an enumeration of all the UnresolvedPermission lists in the
		''' container.
		''' </summary>
		''' <returns> an enumeration of all the UnresolvedPermission objects. </returns>

		Public Overrides Function elements() As Enumeration(Of Permission)
			Dim results As List(Of Permission) = New List(Of Permission) ' where results are stored

			' Get iterator of Map values (which are lists of permissions)
			SyncLock Me
				For Each l As List(Of UnresolvedPermission) In perms.values()
					SyncLock l
						results.addAll(l)
					End SyncLock
				Next l
			End SyncLock

			Return Collections.enumeration(results)
		End Function

		Private Const serialVersionUID As Long = -7176153071733132400L

		' Need to maintain serialization interoperability with earlier releases,
		' which had the serializable field:
		' private Hashtable permissions; // keyed on type

		''' <summary>
		''' @serialField permissions java.util.Hashtable
		'''     A table of the UnresolvedPermissions keyed on type, value is Vector
		'''     of permissions
		''' </summary>
		Private Shared ReadOnly serialPersistentFields As java.io.ObjectStreamField() = { New java.io.ObjectStreamField("permissions", GetType(Hashtable)) }

		''' <summary>
		''' @serialData Default field.
		''' </summary>
	'    
	'     * Writes the contents of the perms field out as a Hashtable
	'     * in which the values are Vectors for
	'     * serialization compatibility with earlier releases.
	'     
		Private Sub writeObject(  out As java.io.ObjectOutputStream)
			' Don't call out.defaultWriteObject()

			' Copy perms into a Hashtable
			Dim permissions As New Dictionary(Of String, Vector(Of UnresolvedPermission))(perms.size()*2)

			' Convert each entry (List) into a Vector
			SyncLock Me
				Dim [set] As [Set](Of KeyValuePair(Of String, List(Of UnresolvedPermission))) = perms.entrySet()
				For Each e As KeyValuePair(Of String, List(Of UnresolvedPermission)) In [set]
					' Convert list into Vector
					Dim list As List(Of UnresolvedPermission) = e.Value
					Dim vec As New Vector(Of UnresolvedPermission)(list.size())
					SyncLock list
						vec.addAll(list)
					End SyncLock

					' Add to Hashtable being serialized
					permissions.put(e.Key, vec)
				Next e
			End SyncLock

			' Write out serializable fields
			Dim pfields As java.io.ObjectOutputStream.PutField = out.putFields()
			pfields.put("permissions", permissions)
			out.writeFields()
		End Sub

	'    
	'     * Reads in a Hashtable in which the values are Vectors of
	'     * UnresolvedPermissions and saves them in the perms field.
	'     
		Private Sub readObject(  [in] As java.io.ObjectInputStream)
			' Don't call defaultReadObject()

			' Read in serialized fields
			Dim gfields As java.io.ObjectInputStream.GetField = [in].readFields()

			' Get permissions
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
			Dim permissions As Dictionary(Of String, Vector(Of UnresolvedPermission)) = CType(gfields.get("permissions", Nothing), Dictionary(Of String, Vector(Of UnresolvedPermission)))
			' writeObject writes a Hashtable<String, Vector<UnresolvedPermission>>
			' for the permissions key, so this cast is safe, unless the data is corrupt.
			perms = New HashMap(Of String, List(Of UnresolvedPermission))(permissions.size()*2)

			' Convert each entry (Vector) into a List
			Dim [set] As [Set](Of KeyValuePair(Of String, Vector(Of UnresolvedPermission))) = permissions.entrySet()
			For Each e As KeyValuePair(Of String, Vector(Of UnresolvedPermission)) In [set]
				' Convert Vector into ArrayList
				Dim vec As Vector(Of UnresolvedPermission) = e.Value
				Dim list As List(Of UnresolvedPermission) = New List(Of UnresolvedPermission)(vec.size())
				list.addAll(vec)

				' Add to Hashtable being serialized
				perms.put(e.Key, list)
			Next e
		End Sub
	End Class

End Namespace