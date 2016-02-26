Imports System
Imports System.Runtime.CompilerServices
Imports System.Collections.Generic
Imports System.Text

'
' * Copyright (c) 2001, 2007, Oracle and/or its affiliates. All rights reserved.
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

Namespace javax.management


	''' <summary>
	''' A Permission to perform actions related to MBeanServers.
	'''    The <em>name</em> of the permission specifies the operation requested
	'''    or granted by the permission.  For a granted permission, it can be
	'''    <code>*</code> to allow all of the MBeanServer operations specified below.
	'''    Otherwise, for a granted or requested permission, it must be one of the
	'''    following:
	'''    <dl>
	'''    <dt>createMBeanServer</dt>
	'''    <dd>Create a new MBeanServer object using the method
	'''    <seealso cref="MBeanServerFactory#createMBeanServer()"/> or
	'''    <seealso cref="MBeanServerFactory#createMBeanServer(java.lang.String)"/>.
	'''    <dt>findMBeanServer</dt>
	'''    <dd>Find an MBeanServer with a given name, or all MBeanServers in this
	'''    JVM, using the method <seealso cref="MBeanServerFactory#findMBeanServer"/>.
	'''    <dt>newMBeanServer</dt>
	'''    <dd>Create a new MBeanServer object without keeping a reference to it,
	'''    using the method <seealso cref="MBeanServerFactory#newMBeanServer()"/> or
	'''    <seealso cref="MBeanServerFactory#newMBeanServer(java.lang.String)"/>.
	'''    <dt>releaseMBeanServer</dt>
	'''    <dd>Remove the MBeanServerFactory's reference to an MBeanServer,
	'''    using the method <seealso cref="MBeanServerFactory#releaseMBeanServer"/>.
	'''    </dl>
	'''    The <em>name</em> of the permission can also denote a list of one or more
	'''    comma-separated operations.  Spaces are allowed at the beginning and
	'''    end of the <em>name</em> and before and after commas.
	'''    <p>
	'''    <code>MBeanServerPermission("createMBeanServer")</code> implies
	'''    <code>MBeanServerPermission("newMBeanServer")</code>.
	''' 
	''' @since 1.5
	''' </summary>
	Public Class MBeanServerPermission
		Inherits java.security.BasicPermission

		Private Const serialVersionUID As Long = -5661980843569388590L

		Private Const CREATE As Integer = 0, FIND As Integer = 1, [NEW] As Integer = 2, RELEASE As Integer = 3, N_NAMES As Integer = 4

		Private Shared ReadOnly names As String() = { "createMBeanServer", "findMBeanServer", "newMBeanServer", "releaseMBeanServer" }

		Private Shared ReadOnly CREATE_MASK As Integer = 1<<CREATE, FIND_MASK As Integer = 1<<FIND, NEW_MASK As Integer = 1<<NEW, RELEASE_MASK As Integer = 1<<RELEASE, ALL_MASK As Integer = CREATE_MASK Or FIND_MASK Or NEW_MASK Or RELEASE_MASK

	'    
	'     * Map from permission masks to canonical names.  This array is
	'     * filled in on demand.
	'     *
	'     * This isn't very scalable.  If we have more than five or six
	'     * permissions, we should consider doing this differently,
	'     * e.g. with a Map.
	'     
		Private Shared ReadOnly canonicalNames As String() = New String(1 << N_NAMES - 1){}

	'    
	'     * The target names mask.  This is not private to avoid having to
	'     * generate accessor methods for accesses from the collection class.
	'     *
	'     * This mask includes implied bits.  So if it has CREATE_MASK then
	'     * it necessarily has NEW_MASK too.
	'     
		<NonSerialized> _
		Friend mask As Integer

		''' <summary>
		''' <p>Create a new MBeanServerPermission with the given name.</p>
		'''    <p>This constructor is equivalent to
		'''    <code>MBeanServerPermission(name,null)</code>.</p> </summary>
		'''    <param name="name"> the name of the granted permission.  It must
		'''    respect the constraints spelt out in the description of the
		'''    <seealso cref="MBeanServerPermission"/> class. </param>
		'''    <exception cref="NullPointerException"> if the name is null. </exception>
		'''    <exception cref="IllegalArgumentException"> if the name is not
		'''    <code>*</code> or one of the allowed names or a comma-separated
		'''    list of the allowed names. </exception>
		Public Sub New(ByVal name As String)
			Me.New(name, Nothing)
		End Sub

		''' <summary>
		''' <p>Create a new MBeanServerPermission with the given name.</p> </summary>
		'''    <param name="name"> the name of the granted permission.  It must
		'''    respect the constraints spelt out in the description of the
		'''    <seealso cref="MBeanServerPermission"/> class. </param>
		'''    <param name="actions"> the associated actions.  This parameter is not
		'''    currently used and must be null or the empty string. </param>
		'''    <exception cref="NullPointerException"> if the name is null. </exception>
		'''    <exception cref="IllegalArgumentException"> if the name is not
		'''    <code>*</code> or one of the allowed names or a comma-separated
		'''    list of the allowed names, or if <code>actions</code> is a non-null
		'''    non-empty string.
		''' </exception>
		''' <exception cref="NullPointerException"> if <code>name</code> is <code>null</code>. </exception>
		''' <exception cref="IllegalArgumentException"> if <code>name</code> is empty or
		''' if arguments are invalid. </exception>
		Public Sub New(ByVal name As String, ByVal actions As String)
			MyBase.New(getCanonicalName(parseMask(name)), actions)

	'         It's annoying to have to parse the name twice, but since
	'           Permission.getName() is final and since we can't access "this"
	'           until after the call to the superclass constructor, there
	'           isn't any very clean way to do this.  MBeanServerPermission
	'           objects aren't constructed very often, luckily.  
			mask = parseMask(name)

			' Check that actions is a null empty string 
			If actions IsNot Nothing AndAlso actions.Length > 0 Then Throw New System.ArgumentException("MBeanServerPermission " & "actions must be null: " & actions)
		End Sub

		Friend Sub New(ByVal mask As Integer)
			MyBase.New(getCanonicalName(mask))
			Me.mask = impliedMask(mask)
		End Sub

		Private Sub readObject(ByVal [in] As java.io.ObjectInputStream)
			[in].defaultReadObject()
			mask = parseMask(name)
		End Sub

		Friend Shared Function simplifyMask(ByVal mask As Integer) As Integer
			If (mask And CREATE_MASK) <> 0 Then mask = mask And Not NEW_MASK
			Return mask
		End Function

		Friend Shared Function impliedMask(ByVal mask As Integer) As Integer
			If (mask And CREATE_MASK) <> 0 Then mask = mask Or NEW_MASK
			Return mask
		End Function

		Friend Shared Function getCanonicalName(ByVal mask As Integer) As String
			If mask = ALL_MASK Then Return "*"

			mask = simplifyMask(mask)

			SyncLock canonicalNames
				If canonicalNames(mask) Is Nothing Then canonicalNames(mask) = makeCanonicalName(mask)
			End SyncLock

			Return canonicalNames(mask)
		End Function

		Private Shared Function makeCanonicalName(ByVal mask As Integer) As String
			Dim buf As New StringBuilder
			For i As Integer = 0 To N_NAMES - 1
				If (mask And (1<<i)) <> 0 Then
					If buf.Length > 0 Then buf.Append(","c)
					buf.Append(names(i))
				End If
			Next i
			Return buf.ToString().intern()
	'         intern() avoids duplication when the mask has only
	'           one bit, so is equivalent to the string constants
	'           we have for the names[] array.  
		End Function

	'     Convert the string into a bitmask, including bits that
	'       are implied by the permissions in the string.  
		Private Shared Function parseMask(ByVal name As String) As Integer
			' Check that target name is a non-null non-empty string 
			If name Is Nothing Then Throw New NullPointerException("MBeanServerPermission: " & "target name can't be null")

			name = name.Trim()
			If name.Equals("*") Then Return ALL_MASK

			' If the name is empty, nameIndex will barf. 
			If name.IndexOf(","c) < 0 Then Return impliedMask(1 << nameIndex(name.Trim()))

			Dim mask As Integer = 0

			Dim tok As New java.util.StringTokenizer(name, ",")
			Do While tok.hasMoreTokens()
				Dim action As String = tok.nextToken()
				Dim i As Integer = nameIndex(action.Trim())
				mask = mask Or (1 << i)
			Loop

			Return impliedMask(mask)
		End Function

		Private Shared Function nameIndex(ByVal name As String) As Integer
			For i As Integer = 0 To N_NAMES - 1
				If names(i).Equals(name) Then Return i
			Next i
			Dim msg As String = "Invalid MBeanServerPermission name: """ & name & """"
			Throw New System.ArgumentException(msg)
		End Function

		Public Overrides Function GetHashCode() As Integer
			Return mask
		End Function

		''' <summary>
		''' <p>Checks if this MBeanServerPermission object "implies" the specified
		''' permission.</p>
		''' 
		''' <p>More specifically, this method returns true if:</p>
		''' 
		''' <ul>
		''' <li> <i>p</i> is an instance of MBeanServerPermission,</li>
		''' <li> <i>p</i>'s target names are a subset of this object's target
		''' names</li>
		''' </ul>
		''' 
		''' <p>The <code>createMBeanServer</code> permission implies the
		''' <code>newMBeanServer</code> permission.</p>
		''' </summary>
		''' <param name="p"> the permission to check against. </param>
		''' <returns> true if the specified permission is implied by this object,
		''' false if not. </returns>
		Public Overridable Function implies(ByVal p As java.security.Permission) As Boolean
			If Not(TypeOf p Is MBeanServerPermission) Then Return False

			Dim that As MBeanServerPermission = CType(p, MBeanServerPermission)

			Return ((Me.mask And that.mask) = that.mask)
		End Function

		''' <summary>
		''' Checks two MBeanServerPermission objects for equality. Checks that
		''' <i>obj</i> is an MBeanServerPermission, and represents the same
		''' list of allowable actions as this object.
		''' <P> </summary>
		''' <param name="obj"> the object we are testing for equality with this object. </param>
		''' <returns> true if the objects are equal. </returns>
		Public Overrides Function Equals(ByVal obj As Object) As Boolean
			If obj Is Me Then Return True

			If Not(TypeOf obj Is MBeanServerPermission) Then Return False

			Dim that As MBeanServerPermission = CType(obj, MBeanServerPermission)

			Return (Me.mask = that.mask)
		End Function

		Public Overridable Function newPermissionCollection() As java.security.PermissionCollection
			Return New MBeanServerPermissionCollection
		End Function
	End Class

	''' <summary>
	''' Class returned by <seealso cref="MBeanServerPermission#newPermissionCollection()"/>.
	''' 
	''' @serial include
	''' </summary>

	'
	' * Since every collection of MBSP can be represented by a single MBSP,
	' * that is what our PermissionCollection does.  We need to define a
	' * PermissionCollection because the one inherited from BasicPermission
	' * doesn't know that createMBeanServer implies newMBeanServer.
	' *
	' * Though the serial form is defined, the TCK does not check it.  We do
	' * not require independent implementations to duplicate it.  Even though
	' * PermissionCollection is Serializable, instances of this class will
	' * hardly ever be serialized, and different implementations do not
	' * typically exchange serialized permission collections.
	' *
	' * If we did require that a particular form be respected here, we would
	' * logically also have to require it for
	' * MBeanPermission.newPermissionCollection, which would preclude an
	' * implementation from defining a PermissionCollection there with an
	' * optimized "implies" method.
	' 
	Friend Class MBeanServerPermissionCollection
		Inherits java.security.PermissionCollection

		''' <summary>
		''' @serial Null if no permissions in collection, otherwise a
		'''    single permission that is the union of all permissions that
		'''    have been added.  
		''' </summary>
		Private collectionPermission As MBeanServerPermission

		Private Const serialVersionUID As Long = -5661980843569388590L

		<MethodImpl(MethodImplOptions.Synchronized)> _
		Public Overridable Sub add(ByVal permission As java.security.Permission)
			If Not(TypeOf permission Is MBeanServerPermission) Then
				Dim msg As String = "Permission not an MBeanServerPermission: " & permission
				Throw New System.ArgumentException(msg)
			End If
			If [readOnly] Then Throw New SecurityException("Read-only permission collection")
			Dim mbsp As MBeanServerPermission = CType(permission, MBeanServerPermission)
			If collectionPermission Is Nothing Then
				collectionPermission = mbsp
			ElseIf Not collectionPermission.implies(permission) Then
				Dim newmask As Integer = collectionPermission.mask Or mbsp.mask
				collectionPermission = New MBeanServerPermission(newmask)
			End If
		End Sub

		<MethodImpl(MethodImplOptions.Synchronized)> _
		Public Overridable Function implies(ByVal permission As java.security.Permission) As Boolean
			Return (collectionPermission IsNot Nothing AndAlso collectionPermission.implies(permission))
		End Function

		<MethodImpl(MethodImplOptions.Synchronized)> _
		Public Overridable Function elements() As System.Collections.IEnumerator(Of java.security.Permission)
			Dim [set] As java.util.Set(Of java.security.Permission)
			If collectionPermission Is Nothing Then
				[set] = java.util.Collections.emptySet()
			Else
				[set] = java.util.Collections.singleton(CType(collectionPermission, java.security.Permission))
			End If
			Return java.util.Collections.enumeration([set])
		End Function
	End Class

End Namespace