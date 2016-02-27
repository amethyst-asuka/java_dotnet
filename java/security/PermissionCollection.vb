Imports Microsoft.VisualBasic
Imports System

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
    ''' Abstract class representing a collection of Permission objects.
    ''' 
    ''' <p>With a PermissionCollection, you can:
    ''' <UL>
    ''' <LI> add a permission to the collection using the {@code add} method.
    ''' <LI> check to see if a particular permission is implied in the
    '''      collection, using the {@code implies} method.
    ''' <LI> enumerate all the permissions, using the {@code elements} method.
    ''' </UL>
    ''' 
    ''' <p>When it is desirable to group together a number of Permission objects
    ''' of the same type, the {@code newPermissionCollection} method on that
    ''' particular type of Permission object should first be called. The default
    ''' behavior (from the Permission [Class]) is to simply return null.
    ''' Subclasses of class Permission override the method if they need to store
    ''' their permissions in a particular PermissionCollection object in order
    ''' to provide the correct semantics when the
    ''' {@code PermissionCollection.implies} method is called.
    ''' If a non-null value is returned, that PermissionCollection must be used.
    ''' If null is returned, then the caller of {@code newPermissionCollection}
    ''' is free to store permissions of the
    ''' given type in any PermissionCollection they choose
    ''' (one that uses a Hashtable, one that uses a Vector, etc).
    ''' 
    ''' <p>The PermissionCollection returned by the
    ''' {@code Permission.newPermissionCollection}
    ''' method is a homogeneous collection, which stores only Permission objects
    ''' for a given Permission type.  A PermissionCollection may also be
    ''' heterogeneous.  For example, Permissions is a PermissionCollection
    ''' subclass that represents a collection of PermissionCollections.
    ''' That is, its members are each a homogeneous PermissionCollection.
    ''' For example, a Permissions object might have a FilePermissionCollection
    ''' for all the FilePermission objects, a SocketPermissionCollection for all the
    ''' SocketPermission objects, and so on. Its {@code add} method adds a
    ''' permission to the appropriate collection.
    ''' 
    ''' <p>Whenever a permission is added to a heterogeneous PermissionCollection
    ''' such as Permissions, and the PermissionCollection doesn't yet contain a
    ''' PermissionCollection of the specified permission's type, the
    ''' PermissionCollection should call
    ''' the {@code newPermissionCollection} method on the permission's class
    ''' to see if it requires a special PermissionCollection. If
    ''' {@code newPermissionCollection}
    ''' returns null, the PermissionCollection
    ''' is free to store the permission in any type of PermissionCollection it
    ''' desires (one using a Hashtable, one using a Vector, etc.). For example,
    ''' the Permissions object uses a default PermissionCollection implementation
    ''' that stores the permission objects in a Hashtable.
    ''' 
    ''' <p> Subclass implementations of PermissionCollection should assume
    ''' that they may be called simultaneously from multiple threads,
    ''' and therefore should be synchronized properly.  Furthermore,
    ''' Enumerations returned via the {@code elements} method are
    ''' not <em>fail-fast</em>.  Modifications to a collection should not be
    ''' performed while enumerating over that collection.
    ''' </summary>
    ''' <seealso cref= Permission </seealso>
    ''' <seealso cref= Permissions
    ''' 
    ''' 
    ''' @author Roland Schemers </seealso>

    <Serializable>
    Public MustInherit Class PermissionCollection : Inherits java.lang.Object

        Private Const serialVersionUID As Long = -6727011328946861783L

		' when set, add will throw an exception.
'JAVA TO VB CONVERTER TODO TASK: There is no VB equivalent to 'volatile':
		Private [readOnly] As Boolean

		''' <summary>
		''' Adds a permission object to the current collection of permission objects.
		''' </summary>
		''' <param name="permission"> the Permission object to add.
		''' </param>
		''' <exception cref="SecurityException"> -  if this PermissionCollection object
		'''                                 has been marked readonly </exception>
		''' <exception cref="IllegalArgumentException"> - if this PermissionCollection
		'''                object is a homogeneous collection and the permission
		'''                is not of the correct type. </exception>
		Public MustOverride Sub add(ByVal permission As Permission)

		''' <summary>
		''' Checks to see if the specified permission is implied by
		''' the collection of Permission objects held in this PermissionCollection.
		''' </summary>
		''' <param name="permission"> the Permission object to compare.
		''' </param>
		''' <returns> true if "permission" is implied by the  permissions in
		''' the collection, false if not. </returns>
		Public MustOverride Function implies(ByVal permission As Permission) As Boolean

		''' <summary>
		''' Returns an enumeration of all the Permission objects in the collection.
		''' </summary>
		''' <returns> an enumeration of all the Permissions. </returns>
		Public MustOverride Function elements() As Enumeration(Of Permission)

		''' <summary>
		''' Marks this PermissionCollection object as "readonly". After
		''' a PermissionCollection object
		''' is marked as readonly, no new Permission objects can be added to it
		''' using {@code add}.
		''' </summary>
		Public Overridable Sub setReadOnly()
			[readOnly] = True
		End Sub

		''' <summary>
		''' Returns true if this PermissionCollection object is marked as readonly.
		''' If it is readonly, no new Permission objects can be added to it
		''' using {@code add}.
		''' 
		''' <p>By default, the object is <i>not</i> readonly. It can be set to
		''' readonly by a call to {@code setReadOnly}.
		''' </summary>
		''' <returns> true if this PermissionCollection object is marked as readonly,
		''' false otherwise. </returns>
		Public Overridable Property [readOnly] As Boolean
			Get
				Return [readOnly]
			End Get
		End Property

		''' <summary>
		''' Returns a string describing this PermissionCollection object,
		''' providing information about all the permissions it contains.
		''' The format is:
		''' <pre>
		''' super.toString() (
		'''   // enumerate all the Permission
		'''   // objects and call toString() on them,
		'''   // one per line..
		''' )</pre>
		''' 
		''' {@code super.toString} is a call to the {@code toString}
		''' method of this
		''' object's superclass, which is Object. The result is
		''' this PermissionCollection's type name followed by this object's
		''' hashcode, thus enabling clients to differentiate different
		''' PermissionCollections object, even if they contain the same permissions.
		''' </summary>
		''' <returns> information about this PermissionCollection object,
		'''         as described above.
		'''  </returns>
		Public Overrides Function ToString() As String
			Dim enum_ As Enumeration(Of Permission) = elements()
			Dim sb As New StringBuilder
			sb.append(MyBase.ToString() & " (" & vbLf)
			Do While enum_.hasMoreElements()
				Try
					sb.append(" ")
					sb.append(enum_.nextElement().ToString())
					sb.append(vbLf)
				Catch e As NoSuchElementException
					' ignore
				End Try
			Loop
			sb.append(")" & vbLf)
			Return sb.ToString()
		End Function
	End Class

End Namespace