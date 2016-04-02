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
	''' The BasicPermission class extends the Permission [Class], and
	''' can be used as the base class for permissions that want to
	''' follow the same naming convention as BasicPermission.
	''' <P>
	''' The name for a BasicPermission is the name of the given permission
	''' (for example, "exit",
	''' "setFactory", "print.queueJob", etc). The naming
	''' convention follows the  hierarchical property naming convention.
	''' An asterisk may appear by itself, or if immediately preceded by a "."
	''' may appear at the end of the name, to signify a wildcard match.
	''' For example, "*" and "java.*" signify a wildcard match, while "*java", "a*b",
	''' and "java*" do not.
	''' <P>
	''' The action string (inherited from Permission) is unused.
	''' Thus, BasicPermission is commonly used as the base class for
	''' "named" permissions
	''' (ones that contain a name but no actions list; you either have the
	''' named permission or you don't.)
	''' Subclasses may implement actions on top of BasicPermission,
	''' if desired.
	''' <p> </summary>
	''' <seealso cref= java.security.Permission </seealso>
	''' <seealso cref= java.security.Permissions </seealso>
	''' <seealso cref= java.security.PermissionCollection </seealso>
	''' <seealso cref= java.lang.SecurityManager
	''' 
	''' @author Marianne Mueller
	''' @author Roland Schemers </seealso>

	<Serializable> _
	Public MustInherit Class BasicPermission
		Inherits Permission

		Private Const serialVersionUID As Long = 6279438298436773498L

		' does this permission have a wildcard at the end?
		<NonSerialized> _
		Private wildcard As Boolean

		' the name without the wildcard on the end
		<NonSerialized> _
		Private path As String

		' is this permission the old-style exitVM permission (pre JDK 1.6)?
		<NonSerialized> _
		Private exitVM As Boolean

		''' <summary>
		''' initialize a BasicPermission object. Common to all constructors.
		''' </summary>
		Private Sub init(ByVal name As String)
			If name Is Nothing Then Throw New NullPointerException("name can't be null")

			Dim len As Integer = name.length()

			If len = 0 Then Throw New IllegalArgumentException("name can't be empty")

			Dim last As Char = name.Chars(len - 1)

			' Is wildcard or ends with ".*"?
			If last = "*"c AndAlso (len = 1 OrElse name.Chars(len - 2) = "."c) Then
				wildcard = True
				If len = 1 Then
					path = ""
				Else
					path = name.Substring(0, len - 1)
				End If
			Else
				If name.Equals("exitVM") Then
					wildcard = True
					path = "exitVM."
					exitVM = True
				Else
					path = name
				End If
			End If
		End Sub

		''' <summary>
		''' Creates a new BasicPermission with the specified name.
		''' Name is the symbolic name of the permission, such as
		''' "setFactory",
		''' "print.queueJob", or "topLevelWindow", etc.
		''' </summary>
		''' <param name="name"> the name of the BasicPermission.
		''' </param>
		''' <exception cref="NullPointerException"> if {@code name} is {@code null}. </exception>
		''' <exception cref="IllegalArgumentException"> if {@code name} is empty. </exception>
		Public Sub New(ByVal name As String)
			MyBase.New(name)
			init(name)
		End Sub


		''' <summary>
		''' Creates a new BasicPermission object with the specified name.
		''' The name is the symbolic name of the BasicPermission, and the
		''' actions String is currently unused.
		''' </summary>
		''' <param name="name"> the name of the BasicPermission. </param>
		''' <param name="actions"> ignored.
		''' </param>
		''' <exception cref="NullPointerException"> if {@code name} is {@code null}. </exception>
		''' <exception cref="IllegalArgumentException"> if {@code name} is empty. </exception>
		Public Sub New(ByVal name As String, ByVal actions As String)
			MyBase.New(name)
			init(name)
		End Sub

		''' <summary>
		''' Checks if the specified permission is "implied" by
		''' this object.
		''' <P>
		''' More specifically, this method returns true if:
		''' <ul>
		''' <li> <i>p</i>'s class is the same as this object's [Class], and
		''' <li> <i>p</i>'s name equals or (in the case of wildcards)
		'''      is implied by this object's
		'''      name. For example, "a.b.*" implies "a.b.c".
		''' </ul>
		''' </summary>
		''' <param name="p"> the permission to check against.
		''' </param>
		''' <returns> true if the passed permission is equal to or
		''' implied by this permission, false otherwise. </returns>
		Public Overrides Function implies(ByVal p As Permission) As Boolean
			If (p Is Nothing) OrElse (p.GetType() IsNot Me.GetType()) Then Return False

			Dim that As BasicPermission = CType(p, BasicPermission)

			If Me.wildcard Then
				If that.wildcard Then
					' one wildcard can imply another
					Return that.path.StartsWith(path)
				Else
					' make sure ap.path is longer so a.b.* doesn't imply a.b
					Return (that.path.length() > Me.path.length()) AndAlso that.path.StartsWith(Me.path)
				End If
			Else
				If that.wildcard Then
					' a non-wildcard can't imply a wildcard
					Return False
				Else
					Return Me.path.Equals(that.path)
				End If
			End If
		End Function

		''' <summary>
		''' Checks two BasicPermission objects for equality.
		''' Checks that <i>obj</i>'s class is the same as this object's class
		''' and has the same name as this object.
		''' <P> </summary>
		''' <param name="obj"> the object we are testing for equality with this object. </param>
		''' <returns> true if <i>obj</i>'s class is the same as this object's class
		'''  and has the same name as this BasicPermission object, false otherwise. </returns>
		Public Overrides Function Equals(ByVal obj As Object) As Boolean
			If obj Is Me Then Return True

			If (obj Is Nothing) OrElse (obj.GetType() IsNot Me.GetType()) Then Return False

			Dim bp As BasicPermission = CType(obj, BasicPermission)

			Return name.Equals(bp.name)
		End Function


		''' <summary>
		''' Returns the hash code value for this object.
		''' The hash code used is the hash code of the name, that is,
		''' {@code getName().hashCode()}, where {@code getName} is
		''' from the Permission superclass.
		''' </summary>
		''' <returns> a hash code value for this object. </returns>
		Public Overrides Function GetHashCode() As Integer
			Return Me.name.GetHashCode()
		End Function

		''' <summary>
		''' Returns the canonical string representation of the actions,
		''' which currently is the empty string "", since there are no actions for
		''' a BasicPermission.
		''' </summary>
		''' <returns> the empty string "". </returns>
		Public  Overrides ReadOnly Property  actions As String
			Get
				Return ""
			End Get
		End Property

		''' <summary>
		''' Returns a new PermissionCollection object for storing BasicPermission
		''' objects.
		''' 
		''' <p>BasicPermission objects must be stored in a manner that allows them
		''' to be inserted in any order, but that also enables the
		''' PermissionCollection {@code implies} method
		''' to be implemented in an efficient (and consistent) manner.
		''' </summary>
		''' <returns> a new PermissionCollection object suitable for
		''' storing BasicPermissions. </returns>
		Public Overrides Function newPermissionCollection() As PermissionCollection
			Return New BasicPermissionCollection(Me.GetType())
		End Function

		''' <summary>
		''' readObject is called to restore the state of the BasicPermission from
		''' a stream.
		''' </summary>
		Private Sub readObject(ByVal s As java.io.ObjectInputStream)
			s.defaultReadObject()
			' init is called to initialize the rest of the values.
			init(name)
		End Sub

		''' <summary>
		''' Returns the canonical name of this BasicPermission.
		''' All internal invocations of getName should invoke this method, so
		''' that the pre-JDK 1.6 "exitVM" and current "exitVM.*" permission are
		''' equivalent in equals/hashCode methods.
		''' </summary>
		''' <returns> the canonical name of this BasicPermission. </returns>
		Friend Property canonicalName As String
			Get
				Return If(exitVM, "exitVM.*", name)
			End Get
		End Property
	End Class

	''' <summary>
	''' A BasicPermissionCollection stores a collection
	''' of BasicPermission permissions. BasicPermission objects
	''' must be stored in a manner that allows them to be inserted in any
	''' order, but enable the implies function to evaluate the implies
	''' method in an efficient (and consistent) manner.
	''' 
	''' A BasicPermissionCollection handles comparing a permission like "a.b.c.d.e"
	''' with a Permission such as "a.b.*", or "*".
	''' </summary>
	''' <seealso cref= java.security.Permission </seealso>
	''' <seealso cref= java.security.Permissions
	''' 
	''' 
	''' @author Roland Schemers
	''' 
	''' @serial include </seealso>

	<Serializable> _
	Friend NotInheritable Class BasicPermissionCollection
		Inherits PermissionCollection

		Private Const serialVersionUID As Long = 739301742472979399L

		''' <summary>
		''' Key is name, value is permission. All permission objects in
		''' collection must be of the same type.
		''' Not serialized; see serialization section at end of class.
		''' </summary>
		<NonSerialized> _
		Private perms As IDictionary(Of String, Permission)

		''' <summary>
		''' This is set to {@code true} if this BasicPermissionCollection
		''' contains a BasicPermission with '*' as its permission name.
		''' </summary>
		''' <seealso cref= #serialPersistentFields </seealso>
		Private all_allowed As Boolean

		''' <summary>
		''' The class to which all BasicPermissions in this
		''' BasicPermissionCollection belongs.
		''' </summary>
		''' <seealso cref= #serialPersistentFields </seealso>
		Private permClass As  [Class]

		''' <summary>
		''' Create an empty BasicPermissionCollection object.
		''' 
		''' </summary>

		Public Sub New(ByVal clazz As [Class])
			perms = New Dictionary(Of String, Permission)(11)
			all_allowed = False
			permClass = clazz
		End Sub

		''' <summary>
		''' Adds a permission to the BasicPermissions. The key for the hash is
		''' permission.path.
		''' </summary>
		''' <param name="permission"> the Permission object to add.
		''' </param>
		''' <exception cref="IllegalArgumentException"> - if the permission is not a
		'''                                       BasicPermission, or if
		'''                                       the permission is not of the
		'''                                       same Class as the other
		'''                                       permissions in this collection.
		''' </exception>
		''' <exception cref="SecurityException"> - if this BasicPermissionCollection object
		'''                                has been marked readonly </exception>
		Public Overrides Sub add(ByVal permission As Permission)
			If Not(TypeOf permission Is BasicPermission) Then Throw New IllegalArgumentException("invalid permission: " & permission)
			If [readOnly] Then Throw New SecurityException("attempt to add a Permission to a readonly PermissionCollection")

			Dim bp As BasicPermission = CType(permission, BasicPermission)

			' make sure we only add new BasicPermissions of the same class
			' Also check null for compatibility with deserialized form from
			' previous versions.
			If permClass Is Nothing Then
				' adding first permission
				permClass = bp.GetType()
			Else
				If bp.GetType() IsNot permClass Then Throw New IllegalArgumentException("invalid permission: " & permission)
			End If

			SyncLock Me
				perms(bp.canonicalName) = permission
			End SyncLock

			' No sync on all_allowed; staleness OK
			If Not all_allowed Then
				If bp.canonicalName.Equals("*") Then all_allowed = True
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
			If Not(TypeOf permission Is BasicPermission) Then Return False

			Dim bp As BasicPermission = CType(permission, BasicPermission)

			' random subclasses of BasicPermission do not imply each other
			If bp.GetType() IsNot permClass Then Return False

			' short circuit if the "*" Permission was added
			If all_allowed Then Return True

			' strategy:
			' Check for full match first. Then work our way up the
			' path looking for matches on a.b..*

			Dim path As String = bp.canonicalName
			'System.out.println("check "+path);

			Dim x As Permission

			SyncLock Me
				x = perms(path)
			End SyncLock

			If x IsNot Nothing Then Return x.implies(permission)

			' work our way up the tree...
			Dim last, offset As Integer

			offset = path.length()-1

			last = path.LastIndexOf(".", offset)
			Do While last <> -1

				path = path.Substring(0, last+1) & "*"
				'System.out.println("check "+path);

				SyncLock Me
					x = perms(path)
				End SyncLock

				If x IsNot Nothing Then Return x.implies(permission)
				offset = last -1
				last = path.LastIndexOf(".", offset)
			Loop

			' we don't have to check for "*" as it was already checked
			' at the top (all_allowed), so we just return false
			Return False
		End Function

		''' <summary>
		''' Returns an enumeration of all the BasicPermission objects in the
		''' container.
		''' </summary>
		''' <returns> an enumeration of all the BasicPermission objects. </returns>
		Public Overrides Function elements() As System.Collections.IEnumerator(Of Permission)
			' Convert Iterator of Map values into an Enumeration
			SyncLock Me
				Return java.util.Collections.enumeration(perms.Values)
			End SyncLock
		End Function

		' Need to maintain serialization interoperability with earlier releases,
		' which had the serializable field:
		'
		' @serial the Hashtable is indexed by the BasicPermission name
		'
		' private Hashtable permissions;
		''' <summary>
		''' @serialField permissions java.util.Hashtable
		'''    The BasicPermissions in this BasicPermissionCollection.
		'''    All BasicPermissions in the collection must belong to the same class.
		'''    The Hashtable is indexed by the BasicPermission name; the value
		'''    of the Hashtable entry is the permission.
		''' @serialField all_allowed boolean
		'''   This is set to {@code true} if this BasicPermissionCollection
		'''   contains a BasicPermission with '*' as its permission name.
		''' @serialField permClass java.lang.Class
		'''   The class to which all BasicPermissions in this
		'''   BasicPermissionCollection belongs.
		''' </summary>
		Private Shared ReadOnly serialPersistentFields As java.io.ObjectStreamField() = { New java.io.ObjectStreamField("permissions", GetType(Hashtable)), New java.io.ObjectStreamField("all_allowed",  java.lang.[Boolean].TYPE), New java.io.ObjectStreamField("permClass", GetType(Class)) }

		''' <summary>
		''' @serialData Default fields.
		''' </summary>
	'    
	'     * Writes the contents of the perms field out as a Hashtable for
	'     * serialization compatibility with earlier releases. all_allowed
	'     * and permClass unchanged.
	'     
		Private Sub writeObject(ByVal out As java.io.ObjectOutputStream)
			' Don't call out.defaultWriteObject()

			' Copy perms into a Hashtable
			Dim permissions As New Dictionary(Of String, Permission)(perms.Count*2)

			SyncLock Me
'JAVA TO VB CONVERTER TODO TASK: There is no .NET Dictionary equivalent to the Java 'putAll' method:
				permissions.putAll(perms)
			End SyncLock

			' Write out serializable fields
			Dim pfields As java.io.ObjectOutputStream.PutField = out.putFields()
			pfields.put("all_allowed", all_allowed)
			pfields.put("permissions", permissions)
			pfields.put("permClass", permClass)
			out.writeFields()
		End Sub

		''' <summary>
		''' readObject is called to restore the state of the
		''' BasicPermissionCollection from a stream.
		''' </summary>
		Private Sub readObject(ByVal [in] As java.io.ObjectInputStream)
			' Don't call defaultReadObject()

			' Read in serialized fields
			Dim gfields As java.io.ObjectInputStream.GetField = [in].readFields()

			' Get permissions
			' writeObject writes a Hashtable<String, Permission> for the
			' permissions key, so this cast is safe, unless the data is corrupt.
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
			Dim permissions As Dictionary(Of String, Permission) = CType(gfields.get("permissions", Nothing), Dictionary(Of String, Permission))
			perms = New Dictionary(Of String, Permission)(permissions.Count*2)
'JAVA TO VB CONVERTER TODO TASK: There is no .NET Dictionary equivalent to the Java 'putAll' method:
			perms.putAll(permissions)

			' Get all_allowed
			all_allowed = gfields.get("all_allowed", False)

			' Get permClass
			permClass = CType(gfields.get("permClass", Nothing), [Class])

			If permClass Is Nothing Then
				' set permClass
				Dim e As System.Collections.IEnumerator(Of Permission) = permissions.Values.GetEnumerator()
'JAVA TO VB CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
				If e.hasMoreElements() Then
'JAVA TO VB CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
					Dim p As Permission = e.nextElement()
					permClass = p.GetType()
				End If
			End If
		End Sub
	End Class

End Namespace