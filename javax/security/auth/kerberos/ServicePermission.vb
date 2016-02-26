Imports Microsoft.VisualBasic
Imports System
Imports System.Collections
Imports System.Collections.Generic
Imports System.Text

'
' * Copyright (c) 2000, 2013, Oracle and/or its affiliates. All rights reserved.
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

Namespace javax.security.auth.kerberos


	''' <summary>
	''' This class is used to protect Kerberos services and the
	''' credentials necessary to access those services. There is a one to
	''' one mapping of a service principal and the credentials necessary
	''' to access the service. Therefore granting access to a service
	''' principal implicitly grants access to the credential necessary to
	''' establish a security context with the service principal. This
	''' applies regardless of whether the credentials are in a cache
	''' or acquired via an exchange with the KDC. The credential can
	''' be either a ticket granting ticket, a service ticket or a secret
	''' key from a key table.
	''' <p>
	''' A ServicePermission contains a service principal name and
	''' a list of actions which specify the context the credential can be
	''' used within.
	''' <p>
	''' The service principal name is the canonical name of the
	''' {@code KerberosPrincipal} supplying the service, that is
	''' the KerberosPrincipal represents a Kerberos service
	''' principal. This name is treated in a case sensitive manner.
	''' An asterisk may appear by itself, to signify any service principal.
	''' <p>
	''' Granting this permission implies that the caller can use a cached
	''' credential (TGT, service ticket or secret key) within the context
	''' designated by the action. In the case of the TGT, granting this
	''' permission also implies that the TGT can be obtained by an
	''' Authentication Service exchange.
	''' <p>
	''' The possible actions are:
	''' 
	''' <pre>
	'''    initiate -              allow the caller to use the credential to
	'''                            initiate a security context with a service
	'''                            principal.
	''' 
	'''    accept -                allow the caller to use the credential to
	'''                            accept security context as a particular
	'''                            principal.
	''' </pre>
	''' 
	''' For example, to specify the permission to access to the TGT to
	''' initiate a security context the permission is constructed as follows:
	''' 
	''' <pre>
	'''     ServicePermission("krbtgt/EXAMPLE.COM@EXAMPLE.COM", "initiate");
	''' </pre>
	''' <p>
	''' To obtain a service ticket to initiate a context with the "host"
	''' service the permission is constructed as follows:
	''' <pre>
	'''     ServicePermission("host/foo.example.com@EXAMPLE.COM", "initiate");
	''' </pre>
	''' <p>
	''' For a Kerberized server the action is "accept". For example, the permission
	''' necessary to access and use the secret key of the  Kerberized "host"
	''' service (telnet and the likes)  would be constructed as follows:
	''' 
	''' <pre>
	'''     ServicePermission("host/foo.example.com@EXAMPLE.COM", "accept");
	''' </pre>
	''' 
	''' @since 1.4
	''' </summary>

	<Serializable> _
	Public NotInheritable Class ServicePermission
		Inherits java.security.Permission

		Private Const serialVersionUID As Long = -1227585031618624935L

		''' <summary>
		''' Initiate a security context to the specified service
		''' </summary>
		Private Const INITIATE As Integer = &H1

		''' <summary>
		''' Accept a security context
		''' </summary>
		Private Const ACCEPT As Integer = &H2

		''' <summary>
		''' All actions
		''' </summary>
		Private Shared ReadOnly ALL As Integer = INITIATE Or ACCEPT

		''' <summary>
		''' No actions.
		''' </summary>
		Private Const NONE As Integer = &H0

		' the actions mask
		<NonSerialized> _
		Private mask As Integer

		''' <summary>
		''' the actions string.
		''' 
		''' @serial
		''' </summary>

		Private actions As String ' Left null as long as possible, then
								' created and re-used in the getAction function.

		''' <summary>
		''' Create a new {@code ServicePermission}
		''' with the specified {@code servicePrincipal}
		''' and {@code action}.
		''' </summary>
		''' <param name="servicePrincipal"> the name of the service principal.
		''' An asterisk may appear by itself, to signify any service principal.
		''' <p> </param>
		''' <param name="action"> the action string </param>
		Public Sub New(ByVal servicePrincipal As String, ByVal action As String)
			' Note: servicePrincipal can be "@REALM" which means any principal in
			' this realm implies it. action can be "-" which means any
			' action implies it.
			MyBase.New(servicePrincipal)
			init(servicePrincipal, getMask(action))
		End Sub


		''' <summary>
		''' Initialize the ServicePermission object.
		''' </summary>
		Private Sub init(ByVal servicePrincipal As String, ByVal mask As Integer)

			If servicePrincipal Is Nothing Then Throw New NullPointerException("service principal can't be null")

			If (mask And ALL) <> mask Then Throw New System.ArgumentException("invalid actions mask")

			Me.mask = mask
		End Sub


		''' <summary>
		''' Checks if this Kerberos service permission object "implies" the
		''' specified permission.
		''' <P>
		''' If none of the above are true, {@code implies} returns false. </summary>
		''' <param name="p"> the permission to check against.
		''' </param>
		''' <returns> true if the specified permission is implied by this object,
		''' false if not. </returns>
		Public Function implies(ByVal p As java.security.Permission) As Boolean
			If Not(TypeOf p Is ServicePermission) Then Return False

			Dim that As ServicePermission = CType(p, ServicePermission)

			Return ((Me.mask And that.mask) = that.mask) AndAlso impliesIgnoreMask(that)
		End Function


		Friend Function impliesIgnoreMask(ByVal p As ServicePermission) As Boolean
			Return ((Me.name.Equals("*")) OrElse Me.name.Equals(p.name) OrElse (p.name.StartsWith("@") AndAlso Me.name.EndsWith(p.name)))
		End Function

		''' <summary>
		''' Checks two ServicePermission objects for equality.
		''' <P> </summary>
		''' <param name="obj"> the object to test for equality with this object.
		''' </param>
		''' <returns> true if <i>obj</i> is a ServicePermission, and has the
		'''  same service principal, and actions as this
		''' ServicePermission object. </returns>
		Public Overrides Function Equals(ByVal obj As Object) As Boolean
			If obj Is Me Then Return True

			If Not(TypeOf obj Is ServicePermission) Then Return False

			Dim that As ServicePermission = CType(obj, ServicePermission)
			Return ((Me.mask And that.mask) = that.mask) AndAlso Me.name.Equals(that.name)


		End Function

		''' <summary>
		''' Returns the hash code value for this object.
		''' </summary>
		''' <returns> a hash code value for this object. </returns>

		Public Overrides Function GetHashCode() As Integer
			Return (name.GetHashCode() Xor mask)
		End Function


		''' <summary>
		''' Returns the "canonical string representation" of the actions in the
		''' specified mask.
		''' Always returns present actions in the following order:
		''' initiate, accept.
		''' </summary>
		''' <param name="mask"> a specific integer action mask to translate into a string </param>
		''' <returns> the canonical string representation of the actions </returns>
		Private Shared Function getActions(ByVal mask As Integer) As String
			Dim sb As New StringBuilder
			Dim comma As Boolean = False

			If (mask And INITIATE) = INITIATE Then
				If comma Then
					sb.Append(","c)
				Else
					comma = True
				End If
				sb.Append("initiate")
			End If

			If (mask And ACCEPT) = ACCEPT Then
				If comma Then
					sb.Append(","c)
				Else
					comma = True
				End If
				sb.Append("accept")
			End If

			Return sb.ToString()
		End Function

		''' <summary>
		''' Returns the canonical string representation of the actions.
		''' Always returns present actions in the following order:
		''' initiate, accept.
		''' </summary>
		Public Property actions As String
			Get
				If actions Is Nothing Then actions = getActions(Me.mask)
    
				Return actions
			End Get
		End Property


		''' <summary>
		''' Returns a PermissionCollection object for storing
		''' ServicePermission objects.
		''' <br>
		''' ServicePermission objects must be stored in a manner that
		''' allows them to be inserted into the collection in any order, but
		''' that also enables the PermissionCollection implies method to
		''' be implemented in an efficient (and consistent) manner.
		''' </summary>
		''' <returns> a new PermissionCollection object suitable for storing
		''' ServicePermissions. </returns>
		Public Function newPermissionCollection() As java.security.PermissionCollection
			Return New KrbServicePermissionCollection
		End Function

		''' <summary>
		''' Return the current action mask.
		''' </summary>
		''' <returns> the actions mask. </returns>
		Friend Property mask As Integer
			Get
				Return mask
			End Get
		End Property

		''' <summary>
		''' Convert an action string to an integer actions mask.
		''' 
		''' Note: if action is "-", action will be NONE, which means any
		''' action implies it.
		''' </summary>
		''' <param name="action"> the action string. </param>
		''' <returns> the action mask </returns>
		Private Shared Function getMask(ByVal action As String) As Integer

			If action Is Nothing Then Throw New NullPointerException("action can't be null")

			If action.Equals("") Then Throw New System.ArgumentException("action can't be empty")

			Dim ___mask As Integer = NONE

			Dim a As Char() = action.ToCharArray()

			If a.Length = 1 AndAlso a(0) = "-"c Then Return ___mask

			Dim i As Integer = a.Length - 1

			Do While i <> -1
				Dim c As Char

				' skip whitespace
				c = a(i)
				Do While (i<>-1) AndAlso (c = " "c OrElse c = ControlChars.Cr OrElse c = ControlChars.Lf OrElse c = ControlChars.FormFeed OrElse c = ControlChars.Tab)
					i -= 1
					c = a(i)
				Loop

				' check for the known strings
				Dim matchlen As Integer

				If i >= 7 AndAlso (a(i-7) = "i"c OrElse a(i-7) = "I"c) AndAlso (a(i-6) = "n"c OrElse a(i-6) = "N"c) AndAlso (a(i-5) = "i"c OrElse a(i-5) = "I"c) AndAlso (a(i-4) = "t"c OrElse a(i-4) = "T"c) AndAlso (a(i-3) = "i"c OrElse a(i-3) = "I"c) AndAlso (a(i-2) = "a"c OrElse a(i-2) = "A"c) AndAlso (a(i-1) = "t"c OrElse a(i-1) = "T"c) AndAlso (a(i) = "e"c OrElse a(i) = "E"c) Then
					matchlen = 8
					___mask = ___mask Or INITIATE

				ElseIf i >= 5 AndAlso (a(i-5) = "a"c OrElse a(i-5) = "A"c) AndAlso (a(i-4) = "c"c OrElse a(i-4) = "C"c) AndAlso (a(i-3) = "c"c OrElse a(i-3) = "C"c) AndAlso (a(i-2) = "e"c OrElse a(i-2) = "E"c) AndAlso (a(i-1) = "p"c OrElse a(i-1) = "P"c) AndAlso (a(i) = "t"c OrElse a(i) = "T"c) Then
					matchlen = 6
					___mask = ___mask Or ACCEPT

				Else
					' parse error
					Throw New System.ArgumentException("invalid permission: " & action)
				End If

				' make sure we didn't just match the tail of a word
				' like "ackbarfaccept".  Also, skip to the comma.
				Dim seencomma As Boolean = False
				Do While i >= matchlen AndAlso Not seencomma
					Select Case a(i-matchlen)
					Case ","c
						seencomma = True
					Case " "c, ControlChars.Cr, ControlChars.Lf, ControlChars.FormFeed, ControlChars.Tab
					Case Else
						Throw New System.ArgumentException("invalid permission: " & action)
					End Select
					i -= 1
				Loop

				' point i at the location of the comma minus one (or -1).
				i -= matchlen
			Loop

			Return ___mask
		End Function


		''' <summary>
		''' WriteObject is called to save the state of the ServicePermission
		''' to a stream. The actions are serialized, and the superclass
		''' takes care of the name.
		''' </summary>
		Private Sub writeObject(ByVal s As java.io.ObjectOutputStream)
			' Write out the actions. The superclass takes care of the name
			' call getActions to make sure actions field is initialized
			If actions Is Nothing Then actions
			s.defaultWriteObject()
		End Sub

		''' <summary>
		''' readObject is called to restore the state of the
		''' ServicePermission from a stream.
		''' </summary>
		Private Sub readObject(ByVal s As java.io.ObjectInputStream)
			' Read in the action, then initialize the rest
			s.defaultReadObject()
			init(name,getMask(actions))
		End Sub


	'    
	'      public static void main(String args[]) throws Exception {
	'      ServicePermission this_ =
	'      new ServicePermission(args[0], "accept");
	'      ServicePermission that_ =
	'      new ServicePermission(args[1], "accept,initiate");
	'      System.out.println("-----\n");
	'      System.out.println("this.implies(that) = " + this_.implies(that_));
	'      System.out.println("-----\n");
	'      System.out.println("this = "+this_);
	'      System.out.println("-----\n");
	'      System.out.println("that = "+that_);
	'      System.out.println("-----\n");
	'
	'      KrbServicePermissionCollection nps =
	'      new KrbServicePermissionCollection();
	'      nps.add(this_);
	'      nps.add(new ServicePermission("nfs/example.com@EXAMPLE.COM",
	'      "accept"));
	'      nps.add(new ServicePermission("host/example.com@EXAMPLE.COM",
	'      "initiate"));
	'      System.out.println("nps.implies(that) = " + nps.implies(that_));
	'      System.out.println("-----\n");
	'
	'      Enumeration e = nps.elements();
	'
	'      while (e.hasMoreElements()) {
	'      ServicePermission x =
	'      (ServicePermission) e.nextElement();
	'      System.out.println("nps.e = " + x);
	'      }
	'
	'      }
	'    

	End Class


	<Serializable> _
	Friend NotInheritable Class KrbServicePermissionCollection
		Inherits java.security.PermissionCollection

		' Not serialized; see serialization section at end of class
		<NonSerialized> _
		Private perms As IList(Of java.security.Permission)

		Public Sub New()
			perms = New List(Of java.security.Permission)
		End Sub

		''' <summary>
		''' Check and see if this collection of permissions implies the permissions
		''' expressed in "permission".
		''' </summary>
		''' <param name="permission"> the Permission object to compare
		''' </param>
		''' <returns> true if "permission" is a proper subset of a permission in
		''' the collection, false if not. </returns>
		Public Function implies(ByVal permission As java.security.Permission) As Boolean
			If Not(TypeOf permission Is ServicePermission) Then Return False

			Dim np As ServicePermission = CType(permission, ServicePermission)
			Dim desired As Integer = np.mask

			If desired = 0 Then
				For Each p As java.security.Permission In perms
					Dim sp As ServicePermission = CType(p, ServicePermission)
					If sp.impliesIgnoreMask(np) Then Return True
				Next p
				Return False
			End If

			Dim effective As Integer = 0
			Dim needed As Integer = desired

			SyncLock Me
				Dim len As Integer = perms.Count

				' need to deal with the case where the needed permission has
				' more than one action and the collection has individual permissions
				' that sum up to the needed.

				For i As Integer = 0 To len - 1
					Dim x As ServicePermission = CType(perms(i), ServicePermission)

					'System.out.println("  trying "+x);
					If ((needed And x.mask) <> 0) AndAlso x.impliesIgnoreMask(np) Then
						effective = effective Or x.mask
						If (effective And desired) = desired Then Return True
						needed = (desired Xor effective)
					End If
				Next i
			End SyncLock
			Return False
		End Function

		''' <summary>
		''' Adds a permission to the ServicePermissions. The key for
		''' the hash is the name.
		''' </summary>
		''' <param name="permission"> the Permission object to add.
		''' </param>
		''' <exception cref="IllegalArgumentException"> - if the permission is not a
		'''                                       ServicePermission
		''' </exception>
		''' <exception cref="SecurityException"> - if this PermissionCollection object
		'''                                has been marked readonly </exception>
		Public Sub add(ByVal permission As java.security.Permission)
			If Not(TypeOf permission Is ServicePermission) Then Throw New System.ArgumentException("invalid permission: " & permission)
			If [readOnly] Then Throw New SecurityException("attempt to add a Permission to a readonly PermissionCollection")

			SyncLock Me
				perms.Insert(0, permission)
			End SyncLock
		End Sub

		''' <summary>
		''' Returns an enumeration of all the ServicePermission objects
		''' in the container.
		''' </summary>
		''' <returns> an enumeration of all the ServicePermission objects. </returns>

		Public Function elements() As System.Collections.IEnumerator(Of java.security.Permission)
			' Convert Iterator into Enumeration
			SyncLock Me
				Return Collections.enumeration(perms)
			End SyncLock
		End Function

		Private Const serialVersionUID As Long = -4118834211490102011L

		' Need to maintain serialization interoperability with earlier releases,
		' which had the serializable field:
		' private Vector permissions;

		''' <summary>
		''' @serialField permissions java.util.Vector
		'''     A list of ServicePermission objects.
		''' </summary>
		Private Shared ReadOnly serialPersistentFields As java.io.ObjectStreamField() = { New java.io.ObjectStreamField("permissions", GetType(ArrayList)) }

		''' <summary>
		''' @serialData "permissions" field (a Vector containing the ServicePermissions).
		''' </summary>
	'    
	'     * Writes the contents of the perms field out as a Vector for
	'     * serialization compatibility with earlier releases.
	'     
		Private Sub writeObject(ByVal out As java.io.ObjectOutputStream)
			' Don't call out.defaultWriteObject()

			' Write out Vector
			Dim permissions As New List(Of java.security.Permission)(perms.Count)

			SyncLock Me
				permissions.AddRange(perms)
			End SyncLock

			Dim pfields As java.io.ObjectOutputStream.PutField = out.putFields()
			pfields.put("permissions", permissions)
			out.writeFields()
		End Sub

	'    
	'     * Reads in a Vector of ServicePermissions and saves them in the perms field.
	'     
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
		Private Sub readObject(ByVal [in] As java.io.ObjectInputStream)
			' Don't call defaultReadObject()

			' Read in serialized fields
			Dim gfields As java.io.ObjectInputStream.GetField = [in].readFields()

			' Get the one we want
			Dim permissions As List(Of java.security.Permission) = CType(gfields.get("permissions", Nothing), List(Of java.security.Permission))
			perms = New List(Of java.security.Permission)(permissions.Count)
			perms.AddRange(permissions)
		End Sub
	End Class

End Namespace