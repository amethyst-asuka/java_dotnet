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

Namespace java.security



	''' <summary>
	''' This class represents a heterogeneous collection of Permissions. That is,
	''' it contains different types of Permission objects, organized into
	''' PermissionCollections. For example, if any
	''' {@code java.io.FilePermission} objects are added to an instance of
	''' this class, they are all stored in a single
	''' PermissionCollection. It is the PermissionCollection returned by a call to
	''' the {@code newPermissionCollection} method in the FilePermission class.
	''' Similarly, any {@code java.lang.RuntimePermission} objects are
	''' stored in the PermissionCollection returned by a call to the
	''' {@code newPermissionCollection} method in the
	''' RuntimePermission class. Thus, this class represents a collection of
	''' PermissionCollections.
	''' 
	''' <p>When the {@code add} method is called to add a Permission, the
	''' Permission is stored in the appropriate PermissionCollection. If no such
	''' collection exists yet, the Permission object's class is determined and the
	''' {@code newPermissionCollection} method is called on that class to create
	''' the PermissionCollection and add it to the Permissions object. If
	''' {@code newPermissionCollection} returns null, then a default
	''' PermissionCollection that uses a hashtable will be created and used. Each
	''' hashtable entry stores a Permission object as both the key and the value.
	''' 
	''' <p> Enumerations returned via the {@code elements} method are
	''' not <em>fail-fast</em>.  Modifications to a collection should not be
	''' performed while enumerating over that collection.
	''' </summary>
	''' <seealso cref= Permission </seealso>
	''' <seealso cref= PermissionCollection </seealso>
	''' <seealso cref= AllPermission
	''' 
	''' 
	''' @author Marianne Mueller
	''' @author Roland Schemers
	''' 
	''' @serial exclude </seealso>

	<Serializable> _
	Public NotInheritable Class Permissions
		Inherits PermissionCollection

		''' <summary>
		''' Key is permissions Class, value is PermissionCollection for that class.
		''' Not serialized; see serialization section at end of class.
		''' </summary>
		<NonSerialized> _
		Private permsMap As IDictionary(Of [Class], PermissionCollection)

		' optimization. keep track of whether unresolved permissions need to be
		' checked
		<NonSerialized> _
		Private hasUnresolved As Boolean = False

		' optimization. keep track of the AllPermission collection
		' - package private for ProtectionDomain optimization
		Friend allPermission As PermissionCollection

		''' <summary>
		''' Creates a new Permissions object containing no PermissionCollections.
		''' </summary>
		Public Sub New()
			permsMap = New Dictionary(Of [Class], PermissionCollection)(11)
			allPermission = Nothing
		End Sub

		''' <summary>
		''' Adds a permission object to the PermissionCollection for the class the
		''' permission belongs to. For example, if <i>permission</i> is a
		''' FilePermission, it is added to the FilePermissionCollection stored
		''' in this Permissions object.
		''' 
		''' This method creates
		''' a new PermissionCollection object (and adds the permission to it)
		''' if an appropriate collection does not yet exist. <p>
		''' </summary>
		''' <param name="permission"> the Permission object to add.
		''' </param>
		''' <exception cref="SecurityException"> if this Permissions object is
		''' marked as readonly.
		''' </exception>
		''' <seealso cref= PermissionCollection#isReadOnly() </seealso>

		Public Overrides Sub add(ByVal permission As Permission)
			If [readOnly] Then Throw New SecurityException("attempt to add a Permission to a readonly Permissions object")

			Dim pc As PermissionCollection

			SyncLock Me
				pc = getPermissionCollection(permission, True)
				pc.add(permission)
			End SyncLock

			' No sync; staleness -> optimizations delayed, which is OK
			If TypeOf permission Is AllPermission Then allPermission = pc
			If TypeOf permission Is UnresolvedPermission Then hasUnresolved = True
		End Sub

		''' <summary>
		''' Checks to see if this object's PermissionCollection for permissions of
		''' the specified permission's class implies the permissions
		''' expressed in the <i>permission</i> object. Returns true if the
		''' combination of permissions in the appropriate PermissionCollection
		''' (e.g., a FilePermissionCollection for a FilePermission) together
		''' imply the specified permission.
		''' 
		''' <p>For example, suppose there is a FilePermissionCollection in this
		''' Permissions object, and it contains one FilePermission that specifies
		''' "read" access for  all files in all subdirectories of the "/tmp"
		''' directory, and another FilePermission that specifies "write" access
		''' for all files in the "/tmp/scratch/foo" directory.
		''' Then if the {@code implies} method
		''' is called with a permission specifying both "read" and "write" access
		''' to files in the "/tmp/scratch/foo" directory, {@code true} is
		''' returned.
		''' 
		''' <p>Additionally, if this PermissionCollection contains the
		''' AllPermission, this method will always return true.
		''' <p> </summary>
		''' <param name="permission"> the Permission object to check.
		''' </param>
		''' <returns> true if "permission" is implied by the permissions in the
		''' PermissionCollection it
		''' belongs to, false if not. </returns>

		Public Overrides Function implies(ByVal permission As Permission) As Boolean
			' No sync; staleness -> skip optimization, which is OK
			If allPermission IsNot Nothing Then
				Return True ' AllPermission has already been added
			Else
				SyncLock Me
					Dim pc As PermissionCollection = getPermissionCollection(permission, False)
					If pc IsNot Nothing Then
						Return pc.implies(permission)
					Else
						' none found
						Return False
					End If
				End SyncLock
			End If
		End Function

		''' <summary>
		''' Returns an enumeration of all the Permission objects in all the
		''' PermissionCollections in this Permissions object.
		''' </summary>
		''' <returns> an enumeration of all the Permissions. </returns>

		Public Overrides Function elements() As System.Collections.IEnumerator(Of Permission)
			' go through each Permissions in the hash table
			' and call their elements() function.

			SyncLock Me
				Return New PermissionsEnumerator(permsMap.Values.GetEnumerator())
			End SyncLock
		End Function

		''' <summary>
		''' Gets the PermissionCollection in this Permissions object for
		''' permissions whose type is the same as that of <i>p</i>.
		''' For example, if <i>p</i> is a FilePermission,
		''' the FilePermissionCollection
		''' stored in this Permissions object will be returned.
		''' 
		''' If createEmpty is true,
		''' this method creates a new PermissionCollection object for the specified
		''' type of permission objects if one does not yet exist.
		''' To do so, it first calls the {@code newPermissionCollection} method
		''' on <i>p</i>.  Subclasses of class Permission
		''' override that method if they need to store their permissions in a
		''' particular PermissionCollection object in order to provide the
		''' correct semantics when the {@code PermissionCollection.implies}
		''' method is called.
		''' If the call returns a PermissionCollection, that collection is stored
		''' in this Permissions object. If the call returns null and createEmpty
		''' is true, then
		''' this method instantiates and stores a default PermissionCollection
		''' that uses a hashtable to store its permission objects.
		''' 
		''' createEmpty is ignored when creating empty PermissionCollection
		''' for unresolved permissions because of the overhead of determining the
		''' PermissionCollection to use.
		''' 
		''' createEmpty should be set to false when this method is invoked from
		''' implies() because it incurs the additional overhead of creating and
		''' adding an empty PermissionCollection that will just return false.
		''' It should be set to true when invoked from add().
		''' </summary>
		Private Function getPermissionCollection(ByVal p As Permission, ByVal createEmpty As Boolean) As PermissionCollection
			Dim c As Class = p.GetType()

			Dim pc As PermissionCollection = permsMap(c)

			If (Not hasUnresolved) AndAlso (Not createEmpty) Then
				Return pc
			ElseIf pc Is Nothing Then

				' Check for unresolved permissions
				pc = (If(hasUnresolved, getUnresolvedPermissions(p), Nothing))

				' if still null, create a new collection
				If pc Is Nothing AndAlso createEmpty Then

					pc = p.newPermissionCollection()

					' still no PermissionCollection?
					' We'll give them a PermissionsHash.
					If pc Is Nothing Then pc = New PermissionsHash
				End If

				If pc IsNot Nothing Then permsMap(c) = pc
			End If
			Return pc
		End Function

		''' <summary>
		''' Resolves any unresolved permissions of type p.
		''' </summary>
		''' <param name="p"> the type of unresolved permission to resolve
		''' </param>
		''' <returns> PermissionCollection containing the unresolved permissions,
		'''  or null if there were no unresolved permissions of type p.
		'''  </returns>
		Private Function getUnresolvedPermissions(ByVal p As Permission) As PermissionCollection
			' Called from within synchronized method so permsMap doesn't need lock

			Dim uc As UnresolvedPermissionCollection = CType(permsMap(GetType(UnresolvedPermission)), UnresolvedPermissionCollection)

			' we have no unresolved permissions if uc is null
			If uc Is Nothing Then Return Nothing

			Dim unresolvedPerms As IList(Of UnresolvedPermission) = uc.getUnresolvedPermissions(p)

			' we have no unresolved permissions of this type if unresolvedPerms is null
			If unresolvedPerms Is Nothing Then Return Nothing

			Dim certs As java.security.cert.Certificate() = Nothing

			Dim signers As Object() = p.GetType().signers

			Dim n As Integer = 0
			If signers IsNot Nothing Then
				For j As Integer = 0 To signers.Length - 1
					If TypeOf signers(j) Is java.security.cert.Certificate Then n += 1
				Next j
				certs = New java.security.cert.Certificate(n - 1){}
				n = 0
				For j As Integer = 0 To signers.Length - 1
					If TypeOf signers(j) Is java.security.cert.Certificate Then
						certs(n) = CType(signers(j), java.security.cert.Certificate)
						n += 1
					End If
				Next j
			End If

			Dim pc As PermissionCollection = Nothing
			SyncLock unresolvedPerms
				Dim len As Integer = unresolvedPerms.Count
				For i As Integer = 0 To len - 1
					Dim up As UnresolvedPermission = unresolvedPerms(i)
					Dim perm As Permission = up.resolve(p, certs)
					If perm IsNot Nothing Then
						If pc Is Nothing Then
							pc = p.newPermissionCollection()
							If pc Is Nothing Then pc = New PermissionsHash
						End If
						pc.add(perm)
					End If
				Next i
			End SyncLock
			Return pc
		End Function

		Private Const serialVersionUID As Long = 4858622370623524688L

		' Need to maintain serialization interoperability with earlier releases,
		' which had the serializable field:
		' private Hashtable perms;

		''' <summary>
		''' @serialField perms java.util.Hashtable
		'''     A table of the Permission classes and PermissionCollections.
		''' @serialField allPermission java.security.PermissionCollection
		''' </summary>
		Private Shared ReadOnly serialPersistentFields As java.io.ObjectStreamField() = { New java.io.ObjectStreamField("perms", GetType(Hashtable)), New java.io.ObjectStreamField("allPermission", GetType(PermissionCollection)) }

		''' <summary>
		''' @serialData Default fields.
		''' </summary>
	'    
	'     * Writes the contents of the permsMap field out as a Hashtable for
	'     * serialization compatibility with earlier releases. allPermission
	'     * unchanged.
	'     
		Private Sub writeObject(ByVal out As java.io.ObjectOutputStream)
			' Don't call out.defaultWriteObject()

			' Copy perms into a Hashtable
			Dim perms As New Dictionary(Of [Class], PermissionCollection)(permsMap.Count*2) ' no sync; estimate
			SyncLock Me
'JAVA TO VB CONVERTER TODO TASK: There is no .NET Dictionary equivalent to the Java 'putAll' method:
				perms.putAll(permsMap)
			End SyncLock

			' Write out serializable fields
			Dim pfields As java.io.ObjectOutputStream.PutField = out.putFields()

			pfields.put("allPermission", allPermission) ' no sync; staleness OK
			pfields.put("perms", perms)
			out.writeFields()
		End Sub

	'    
	'     * Reads in a Hashtable of Class/PermissionCollections and saves them in the
	'     * permsMap field. Reads in allPermission.
	'     
		Private Sub readObject(ByVal [in] As java.io.ObjectInputStream)
			' Don't call defaultReadObject()

			' Read in serialized fields
			Dim gfields As java.io.ObjectInputStream.GetField = [in].readFields()

			' Get allPermission
			allPermission = CType(gfields.get("allPermission", Nothing), PermissionCollection)

			' Get permissions
			' writeObject writes a Hashtable<Class<?>, PermissionCollection> for
			' the perms key, so this cast is safe, unless the data is corrupt.
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
			Dim perms As Dictionary(Of [Class], PermissionCollection) = CType(gfields.get("perms", Nothing), Dictionary(Of [Class], PermissionCollection))
			permsMap = New Dictionary(Of [Class], PermissionCollection)(perms.Count*2)
'JAVA TO VB CONVERTER TODO TASK: There is no .NET Dictionary equivalent to the Java 'putAll' method:
			permsMap.putAll(perms)

			' Set hasUnresolved
			Dim uc As UnresolvedPermissionCollection = CType(permsMap(GetType(UnresolvedPermission)), UnresolvedPermissionCollection)
			hasUnresolved = (uc IsNot Nothing AndAlso uc.elements().hasMoreElements())
		End Sub
	End Class

	Friend NotInheritable Class PermissionsEnumerator
		Implements System.Collections.IEnumerator(Of Permission)

		' all the perms
		Private perms As IEnumerator(Of PermissionCollection)
		' the current set
		Private permset As System.Collections.IEnumerator(Of Permission)

		Friend Sub New(ByVal e As IEnumerator(Of PermissionCollection))
			perms = e
			permset = nextEnumWithMore
		End Sub

		' No need to synchronize; caller should sync on object as required
		Public Function hasMoreElements() As Boolean
			' if we enter with permissionimpl null, we know
			' there are no more left.

			If permset Is Nothing Then Return False

			' try to see if there are any left in the current one

'JAVA TO VB CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
			If permset.hasMoreElements() Then Return True

			' get the next one that has something in it...
			permset = nextEnumWithMore

			' if it is null, we are done!
			Return (permset IsNot Nothing)
		End Function

		' No need to synchronize; caller should sync on object as required
		Public Function nextElement() As Permission

			' hasMoreElements will update permset to the next permset
			' with something in it...

			If hasMoreElements() Then
'JAVA TO VB CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
				Return permset.nextElement()
			Else
				Throw New java.util.NoSuchElementException("PermissionsEnumerator")
			End If

		End Function

		Private Property nextEnumWithMore As System.Collections.IEnumerator(Of Permission)
			Get
				Do While perms.MoveNext()
					Dim pc As PermissionCollection = perms.Current
					Dim [next] As System.Collections.IEnumerator(Of Permission) =pc.elements()
	'JAVA TO VB CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
					If [next].hasMoreElements() Then Return [next]
				Loop
				Return Nothing
    
			End Get
		End Property
	End Class

	''' <summary>
	''' A PermissionsHash stores a homogeneous set of permissions in a hashtable.
	''' </summary>
	''' <seealso cref= Permission </seealso>
	''' <seealso cref= Permissions
	''' 
	''' 
	''' @author Roland Schemers
	''' 
	''' @serial include </seealso>

	<Serializable> _
	Friend NotInheritable Class PermissionsHash
		Inherits PermissionCollection

		''' <summary>
		''' Key and value are (same) permissions objects.
		''' Not serialized; see serialization section at end of class.
		''' </summary>
		<NonSerialized> _
		Private permsMap As IDictionary(Of Permission, Permission)

		''' <summary>
		''' Create an empty PermissionsHash object.
		''' </summary>

		Friend Sub New()
			permsMap = New Dictionary(Of Permission, Permission)(11)
		End Sub

		''' <summary>
		''' Adds a permission to the PermissionsHash.
		''' </summary>
		''' <param name="permission"> the Permission object to add. </param>

		Public Overrides Sub add(ByVal permission As Permission)
			SyncLock Me
				permsMap(permission) = permission
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

		Public Overrides Function implies(ByVal permission As Permission) As Boolean
			' attempt a fast lookup and implies. If that fails
			' then enumerate through all the permissions.
			SyncLock Me
				Dim p As Permission = permsMap(permission)

				' If permission is found, then p.equals(permission)
				If p Is Nothing Then
					For Each p_ As Permission In permsMap.Values
						If p_.implies(permission) Then Return True
					Next p_
					Return False
				Else
					Return True
				End If
			End SyncLock
		End Function

		''' <summary>
		''' Returns an enumeration of all the Permission objects in the container.
		''' </summary>
		''' <returns> an enumeration of all the Permissions. </returns>

		Public Overrides Function elements() As System.Collections.IEnumerator(Of Permission)
			' Convert Iterator of Map values into an Enumeration
			SyncLock Me
				Return java.util.Collections.enumeration(permsMap.Values)
			End SyncLock
		End Function

		Private Const serialVersionUID As Long = -8491988220802933440L
		' Need to maintain serialization interoperability with earlier releases,
		' which had the serializable field:
		' private Hashtable perms;
		''' <summary>
		''' @serialField perms java.util.Hashtable
		'''     A table of the Permissions (both key and value are same).
		''' </summary>
		Private Shared ReadOnly serialPersistentFields As java.io.ObjectStreamField() = { New java.io.ObjectStreamField("perms", GetType(Hashtable)) }

		''' <summary>
		''' @serialData Default fields.
		''' </summary>
	'    
	'     * Writes the contents of the permsMap field out as a Hashtable for
	'     * serialization compatibility with earlier releases.
	'     
		Private Sub writeObject(ByVal out As java.io.ObjectOutputStream)
			' Don't call out.defaultWriteObject()

			' Copy perms into a Hashtable
			Dim perms As New Dictionary(Of Permission, Permission)(permsMap.Count*2)
			SyncLock Me
'JAVA TO VB CONVERTER TODO TASK: There is no .NET Dictionary equivalent to the Java 'putAll' method:
				perms.putAll(permsMap)
			End SyncLock

			' Write out serializable fields
			Dim pfields As java.io.ObjectOutputStream.PutField = out.putFields()
			pfields.put("perms", perms)
			out.writeFields()
		End Sub

	'    
	'     * Reads in a Hashtable of Permission/Permission and saves them in the
	'     * permsMap field.
	'     
		Private Sub readObject(ByVal [in] As java.io.ObjectInputStream)
			' Don't call defaultReadObject()

			' Read in serialized fields
			Dim gfields As java.io.ObjectInputStream.GetField = [in].readFields()

			' Get permissions
			' writeObject writes a Hashtable<Class<?>, PermissionCollection> for
			' the perms key, so this cast is safe, unless the data is corrupt.
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
			Dim perms As Dictionary(Of Permission, Permission) = CType(gfields.get("perms", Nothing), Dictionary(Of Permission, Permission))
			permsMap = New Dictionary(Of Permission, Permission)(perms.Count*2)
'JAVA TO VB CONVERTER TODO TASK: There is no .NET Dictionary equivalent to the Java 'putAll' method:
			permsMap.putAll(perms)
		End Sub
	End Class

End Namespace