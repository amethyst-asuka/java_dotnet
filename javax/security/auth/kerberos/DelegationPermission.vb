Imports System
Imports System.Runtime.CompilerServices
Imports System.Collections
Imports System.Collections.Generic

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
	''' This class is used to restrict the usage of the Kerberos
	''' delegation model, ie: forwardable and proxiable tickets.
	''' <p>
	''' The target name of this {@code Permission} specifies a pair of
	''' kerberos service principals. The first is the subordinate service principal
	''' being entrusted to use the TGT. The second service principal designates
	''' the target service the subordinate service principal is to
	''' interact with on behalf of the initiating KerberosPrincipal. This
	''' latter service principal is specified to restrict the use of a
	''' proxiable ticket.
	''' <p>
	''' For example, to specify the "host" service use of a forwardable TGT the
	''' target permission is specified as follows:
	''' 
	''' <pre>
	'''  DelegationPermission("\"host/foo.example.com@EXAMPLE.COM\" \"krbtgt/EXAMPLE.COM@EXAMPLE.COM\"");
	''' </pre>
	''' <p>
	''' To give the "backup" service a proxiable nfs service ticket the target permission
	''' might be specified:
	''' 
	''' <pre>
	'''  DelegationPermission("\"backup/bar.example.com@EXAMPLE.COM\" \"nfs/home.EXAMPLE.COM@EXAMPLE.COM\"");
	''' </pre>
	''' 
	''' @since 1.4
	''' </summary>

	<Serializable> _
	Public NotInheritable Class DelegationPermission
		Inherits java.security.BasicPermission

		Private Const serialVersionUID As Long = 883133252142523922L

		<NonSerialized> _
		Private subordinate, service As String

		''' <summary>
		''' Create a new {@code DelegationPermission}
		''' with the specified subordinate and target principals.
		''' 
		''' <p>
		''' </summary>
		''' <param name="principals"> the name of the subordinate and target principals
		''' </param>
		''' <exception cref="NullPointerException"> if {@code principals} is {@code null}. </exception>
		''' <exception cref="IllegalArgumentException"> if {@code principals} is empty. </exception>
		Public Sub New(ByVal principals As String)
			MyBase.New(principals)
			init(principals)
		End Sub

		''' <summary>
		''' Create a new {@code DelegationPermission}
		''' with the specified subordinate and target principals.
		''' <p>
		''' </summary>
		''' <param name="principals"> the name of the subordinate and target principals
		''' <p> </param>
		''' <param name="actions"> should be null.
		''' </param>
		''' <exception cref="NullPointerException"> if {@code principals} is {@code null}. </exception>
		''' <exception cref="IllegalArgumentException"> if {@code principals} is empty. </exception>
		Public Sub New(ByVal principals As String, ByVal actions As String)
			MyBase.New(principals, actions)
			init(principals)
		End Sub


		''' <summary>
		''' Initialize the DelegationPermission object.
		''' </summary>
		Private Sub init(ByVal target As String)

			Dim t As StringTokenizer = Nothing
			If Not target.StartsWith("""") Then
				Throw New System.ArgumentException("service principal [" & target & "] syntax invalid: " & "improperly quoted")
			Else
				t = New StringTokenizer(target, """", False)
				subordinate = t.nextToken()
				If t.countTokens() = 2 Then
					t.nextToken() ' bypass whitespace
					service = t.nextToken()
				ElseIf t.countTokens() > 0 Then
					Throw New System.ArgumentException("service principal [" & t.nextToken() & "] syntax invalid: " & "improperly quoted")
				End If
			End If
		End Sub

		''' <summary>
		''' Checks if this Kerberos delegation permission object "implies" the
		''' specified permission.
		''' <P>
		''' If none of the above are true, {@code implies} returns false. </summary>
		''' <param name="p"> the permission to check against.
		''' </param>
		''' <returns> true if the specified permission is implied by this object,
		''' false if not. </returns>
		Public Function implies(ByVal p As java.security.Permission) As Boolean
			If Not(TypeOf p Is DelegationPermission) Then Return False

			Dim that As DelegationPermission = CType(p, DelegationPermission)
			If Me.subordinate.Equals(that.subordinate) AndAlso Me.service.Equals(that.service) Then Return True

			Return False
		End Function


		''' <summary>
		''' Checks two DelegationPermission objects for equality.
		''' <P> </summary>
		''' <param name="obj"> the object to test for equality with this object.
		''' </param>
		''' <returns> true if <i>obj</i> is a DelegationPermission, and
		'''  has the same subordinate and service principal as this.
		'''  DelegationPermission object. </returns>
		Public Overrides Function Equals(ByVal obj As Object) As Boolean
			If obj Is Me Then Return True

			If Not(TypeOf obj Is DelegationPermission) Then Return False

			Dim that As DelegationPermission = CType(obj, DelegationPermission)
			Return implies(that)
		End Function

		''' <summary>
		''' Returns the hash code value for this object.
		''' </summary>
		''' <returns> a hash code value for this object. </returns>
		Public Overrides Function GetHashCode() As Integer
			Return name.GetHashCode()
		End Function


		''' <summary>
		''' Returns a PermissionCollection object for storing
		''' DelegationPermission objects.
		''' <br>
		''' DelegationPermission objects must be stored in a manner that
		''' allows them to be inserted into the collection in any order, but
		''' that also enables the PermissionCollection implies method to
		''' be implemented in an efficient (and consistent) manner.
		''' </summary>
		''' <returns> a new PermissionCollection object suitable for storing
		''' DelegationPermissions. </returns>

		Public Function newPermissionCollection() As java.security.PermissionCollection
			Return New KrbDelegationPermissionCollection
		End Function

		''' <summary>
		''' WriteObject is called to save the state of the DelegationPermission
		''' to a stream. The actions are serialized, and the superclass
		''' takes care of the name.
		''' </summary>
		<MethodImpl(MethodImplOptions.Synchronized)> _
		Private Sub writeObject(ByVal s As java.io.ObjectOutputStream)
			s.defaultWriteObject()
		End Sub

		''' <summary>
		''' readObject is called to restore the state of the
		''' DelegationPermission from a stream.
		''' </summary>
		<MethodImpl(MethodImplOptions.Synchronized)> _
		Private Sub readObject(ByVal s As java.io.ObjectInputStream)
			' Read in the action, then initialize the rest
			s.defaultReadObject()
			init(name)
		End Sub

	'    
	'      public static void main(String args[]) throws Exception {
	'      DelegationPermission this_ =
	'      new DelegationPermission(args[0]);
	'      DelegationPermission that_ =
	'      new DelegationPermission(args[1]);
	'      System.out.println("-----\n");
	'      System.out.println("this.implies(that) = " + this_.implies(that_));
	'      System.out.println("-----\n");
	'      System.out.println("this = "+this_);
	'      System.out.println("-----\n");
	'      System.out.println("that = "+that_);
	'      System.out.println("-----\n");
	'
	'      KrbDelegationPermissionCollection nps =
	'      new KrbDelegationPermissionCollection();
	'      nps.add(this_);
	'      nps.add(new DelegationPermission("\"host/foo.example.com@EXAMPLE.COM\" \"CN=Gary Ellison/OU=JSN/O=SUNW/L=Palo Alto/ST=CA/C=US\""));
	'      try {
	'      nps.add(new DelegationPermission("host/foo.example.com@EXAMPLE.COM \"CN=Gary Ellison/OU=JSN/O=SUNW/L=Palo Alto/ST=CA/C=US\""));
	'      } catch (Exception e) {
	'      System.err.println(e);
	'      }
	'
	'      System.out.println("nps.implies(that) = " + nps.implies(that_));
	'      System.out.println("-----\n");
	'
	'      Enumeration e = nps.elements();
	'
	'      while (e.hasMoreElements()) {
	'      DelegationPermission x =
	'      (DelegationPermission) e.nextElement();
	'      System.out.println("nps.e = " + x);
	'      }
	'      }
	'    
	End Class


	<Serializable> _
	Friend NotInheritable Class KrbDelegationPermissionCollection
		Inherits java.security.PermissionCollection

		' Not serialized; see serialization section at end of class.
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
			If Not(TypeOf permission Is DelegationPermission) Then Return False

			SyncLock Me
				For Each x As java.security.Permission In perms
					If x.implies(permission) Then Return True
				Next x
			End SyncLock
			Return False

		End Function

		''' <summary>
		''' Adds a permission to the DelegationPermissions. The key for
		''' the hash is the name.
		''' </summary>
		''' <param name="permission"> the Permission object to add.
		''' </param>
		''' <exception cref="IllegalArgumentException"> - if the permission is not a
		'''                                       DelegationPermission
		''' </exception>
		''' <exception cref="SecurityException"> - if this PermissionCollection object
		'''                                has been marked readonly </exception>
		Public Sub add(ByVal permission As java.security.Permission)
			If Not(TypeOf permission Is DelegationPermission) Then Throw New System.ArgumentException("invalid permission: " & permission)
			If [readOnly] Then Throw New SecurityException("attempt to add a Permission to a readonly PermissionCollection")

			SyncLock Me
				perms.Insert(0, permission)
			End SyncLock
		End Sub

		''' <summary>
		''' Returns an enumeration of all the DelegationPermission objects
		''' in the container.
		''' </summary>
		''' <returns> an enumeration of all the DelegationPermission objects. </returns>
		Public Function elements() As System.Collections.IEnumerator(Of java.security.Permission)
			' Convert Iterator into Enumeration
			SyncLock Me
				Return Collections.enumeration(perms)
			End SyncLock
		End Function

		Private Const serialVersionUID As Long = -3383936936589966948L

		' Need to maintain serialization interoperability with earlier releases,
		' which had the serializable field:
		'    private Vector permissions;
		''' <summary>
		''' @serialField permissions java.util.Vector
		'''     A list of DelegationPermission objects.
		''' </summary>
		Private Shared ReadOnly serialPersistentFields As java.io.ObjectStreamField() = { New java.io.ObjectStreamField("permissions", GetType(ArrayList)) }

		''' <summary>
		''' @serialData "permissions" field (a Vector containing the DelegationPermissions).
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
	'     * Reads in a Vector of DelegationPermissions and saves them in the perms field.
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