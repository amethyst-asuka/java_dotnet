Imports Microsoft.VisualBasic
Imports System
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
	''' The UnresolvedPermission class is used to hold Permissions that
	''' were "unresolved" when the Policy was initialized.
	''' An unresolved permission is one whose actual Permission class
	''' does not yet exist at the time the Policy is initialized (see below).
	''' 
	''' <p>The policy for a Java runtime (specifying
	''' which permissions are available for code from various principals)
	''' is represented by a Policy object.
	''' Whenever a Policy is initialized or refreshed, Permission objects of
	''' appropriate classes are created for all permissions
	''' allowed by the Policy.
	''' 
	''' <p>Many permission class types
	''' referenced by the policy configuration are ones that exist
	''' locally (i.e., ones that can be found on CLASSPATH).
	''' Objects for such permissions can be instantiated during
	''' Policy initialization. For example, it is always possible
	''' to instantiate a java.io.FilePermission, since the
	''' FilePermission class is found on the CLASSPATH.
	''' 
	''' <p>Other permission classes may not yet exist during Policy
	''' initialization. For example, a referenced permission class may
	''' be in a JAR file that will later be loaded.
	''' For each such class, an UnresolvedPermission is instantiated.
	''' Thus, an UnresolvedPermission is essentially a "placeholder"
	''' containing information about the permission.
	''' 
	''' <p>Later, when code calls AccessController.checkPermission
	''' on a permission of a type that was previously unresolved,
	''' but whose class has since been loaded, previously-unresolved
	''' permissions of that type are "resolved". That is,
	''' for each such UnresolvedPermission, a new object of
	''' the appropriate class type is instantiated, based on the
	''' information in the UnresolvedPermission.
	''' 
	''' <p> To instantiate the new class, UnresolvedPermission assumes
	''' the class provides a zero, one, and/or two-argument constructor.
	''' The zero-argument constructor would be used to instantiate
	''' a permission without a name and without actions.
	''' A one-arg constructor is assumed to take a {@code String}
	''' name as input, and a two-arg constructor is assumed to take a
	''' {@code String} name and {@code String} actions
	''' as input.  UnresolvedPermission may invoke a
	''' constructor with a {@code null} name and/or actions.
	''' If an appropriate permission constructor is not available,
	''' the UnresolvedPermission is ignored and the relevant permission
	''' will not be granted to executing code.
	''' 
	''' <p> The newly created permission object replaces the
	''' UnresolvedPermission, which is removed.
	''' 
	''' <p> Note that the {@code getName} method for an
	''' {@code UnresolvedPermission} returns the
	''' {@code type} (class name) for the underlying permission
	''' that has not been resolved.
	''' </summary>
	''' <seealso cref= java.security.Permission </seealso>
	''' <seealso cref= java.security.Permissions </seealso>
	''' <seealso cref= java.security.PermissionCollection </seealso>
	''' <seealso cref= java.security.Policy
	''' 
	''' 
	''' @author Roland Schemers </seealso>

	<Serializable> _
	Public NotInheritable Class UnresolvedPermission
		Inherits Permission

		Private Const serialVersionUID As Long = -4821973115467008846L

		Private Shared ReadOnly debug As sun.security.util.Debug = sun.security.util.Debug.getInstance("policy,access", "UnresolvedPermission")

		''' <summary>
		''' The class name of the Permission class that will be
		''' created when this unresolved permission is resolved.
		''' 
		''' @serial
		''' </summary>
		Private type As String

		''' <summary>
		''' The permission name.
		''' 
		''' @serial
		''' </summary>
		Private name As String

		''' <summary>
		''' The actions of the permission.
		''' 
		''' @serial
		''' </summary>
		Private actions As String

		<NonSerialized> _
		Private certs As java.security.cert.Certificate()

		''' <summary>
		''' Creates a new UnresolvedPermission containing the permission
		''' information needed later to actually create a Permission of the
		''' specified class, when the permission is resolved.
		''' </summary>
		''' <param name="type"> the class name of the Permission class that will be
		''' created when this unresolved permission is resolved. </param>
		''' <param name="name"> the name of the permission. </param>
		''' <param name="actions"> the actions of the permission. </param>
		''' <param name="certs"> the certificates the permission's class was signed with.
		''' This is a list of certificate chains, where each chain is composed of a
		''' signer certificate and optionally its supporting certificate chain.
		''' Each chain is ordered bottom-to-top (i.e., with the signer certificate
		''' first and the (root) certificate authority last). The signer
		''' certificates are copied from the array. Subsequent changes to
		''' the array will not affect this UnsolvedPermission. </param>
		Public Sub New(ByVal type As String, ByVal name As String, ByVal actions As String, ByVal certs As java.security.cert.Certificate())
			MyBase.New(type)

			If type Is Nothing Then Throw New NullPointerException("type can't be null")

			Me.type = type
			Me.name = name
			Me.actions = actions
			If certs IsNot Nothing Then
				' Extract the signer certs from the list of certificates.
				For i As Integer = 0 To certs.Length - 1
					If Not(TypeOf certs(i) Is X509Certificate) Then
						' there is no concept of signer certs, so we store the
						' entire cert array
						Me.certs = certs.clone()
						Exit For
					End If
				Next i

				If Me.certs Is Nothing Then
					' Go through the list of certs and see if all the certs are
					' signer certs.
					Dim i As Integer = 0
					Dim count As Integer = 0
					Do While i < certs.Length
						count += 1
						Do While ((i+1) < certs.Length) AndAlso CType(certs(i), X509Certificate).issuerDN.Equals(CType(certs(i+1), X509Certificate).subjectDN)
							i += 1
						Loop
						i += 1
					Loop
					If count = certs.Length Then Me.certs = certs.clone()

					If Me.certs Is Nothing Then
						' extract the signer certs
						Dim signerCerts As New List(Of java.security.cert.Certificate)
						i = 0
						Do While i < certs.Length
							signerCerts.Add(certs(i))
							Do While ((i+1) < certs.Length) AndAlso CType(certs(i), X509Certificate).issuerDN.Equals(CType(certs(i+1), X509Certificate).subjectDN)
								i += 1
							Loop
							i += 1
						Loop
						Me.certs = New java.security.cert.Certificate(signerCerts.Count - 1){}
						signerCerts.ToArray(Me.certs)
					End If
				End If
			End If
		End Sub


		Private Shared ReadOnly PARAMS0 As Class() = { }
		Private Shared ReadOnly PARAMS1 As Class() = { GetType(String) }
		Private Shared ReadOnly PARAMS2 As Class() = { GetType(String), GetType(String) }

		''' <summary>
		''' try and resolve this permission using the class loader of the permission
		''' that was passed in.
		''' </summary>
		Friend Function resolve(ByVal p As Permission, ByVal certs As java.security.cert.Certificate()) As Permission
			If Me.certs IsNot Nothing Then
				' if p wasn't signed, we don't have a match
				If certs Is Nothing Then Return Nothing

				' all certs in this.certs must be present in certs
				Dim match As Boolean
				For i As Integer = 0 To Me.certs.Length - 1
					match = False
					For j As Integer = 0 To certs.Length - 1
						If Me.certs(i).Equals(certs(j)) Then
							match = True
							Exit For
						End If
					Next j
					If Not match Then Return Nothing
				Next i
			End If
			Try
				Dim pc As Class = p.GetType()

				If name Is Nothing AndAlso actions Is Nothing Then
					Try
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
						Dim c As Constructor(Of ?) = pc.getConstructor(PARAMS0)
						Return CType(c.newInstance(New Object() {}), Permission)
					Catch ne As NoSuchMethodException
						Try
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
							Dim c As Constructor(Of ?) = pc.getConstructor(PARAMS1)
							Return CType(c.newInstance(New Object() { name}), Permission)
						Catch ne1 As NoSuchMethodException
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
							Dim c As Constructor(Of ?) = pc.getConstructor(PARAMS2)
							Return CType(c.newInstance(New Object() { name, actions }), Permission)
						End Try
					End Try
				Else
					If name IsNot Nothing AndAlso actions Is Nothing Then
						Try
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
							Dim c As Constructor(Of ?) = pc.getConstructor(PARAMS1)
							Return CType(c.newInstance(New Object() { name}), Permission)
						Catch ne As NoSuchMethodException
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
							Dim c As Constructor(Of ?) = pc.getConstructor(PARAMS2)
							Return CType(c.newInstance(New Object() { name, actions }), Permission)
						End Try
					Else
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
						Dim c As Constructor(Of ?) = pc.getConstructor(PARAMS2)
						Return CType(c.newInstance(New Object() { name, actions }), Permission)
					End If
				End If
			Catch nsme As NoSuchMethodException
				If debug IsNot Nothing Then
					debug.println("NoSuchMethodException:" & vbLf & "  could not find " & "proper constructor for " & type)
					Console.WriteLine(nsme.ToString())
					Console.Write(nsme.StackTrace)
				End If
				Return Nothing
			Catch e As Exception
				If debug IsNot Nothing Then
					debug.println("unable to instantiate " & name)
					e.printStackTrace()
				End If
				Return Nothing
			End Try
		End Function

		''' <summary>
		''' This method always returns false for unresolved permissions.
		''' That is, an UnresolvedPermission is never considered to
		''' imply another permission.
		''' </summary>
		''' <param name="p"> the permission to check against.
		''' </param>
		''' <returns> false. </returns>
		Public Overrides Function implies(ByVal p As Permission) As Boolean
			Return False
		End Function

		''' <summary>
		''' Checks two UnresolvedPermission objects for equality.
		''' Checks that <i>obj</i> is an UnresolvedPermission, and has
		''' the same type (class) name, permission name, actions, and
		''' certificates as this object.
		''' 
		''' <p> To determine certificate equality, this method only compares
		''' actual signer certificates.  Supporting certificate chains
		''' are not taken into consideration by this method.
		''' </summary>
		''' <param name="obj"> the object we are testing for equality with this object.
		''' </param>
		''' <returns> true if obj is an UnresolvedPermission, and has the same
		''' type (class) name, permission name, actions, and
		''' certificates as this object. </returns>
		Public Overrides Function Equals(ByVal obj As Object) As Boolean
			If obj Is Me Then Return True

			If Not(TypeOf obj Is UnresolvedPermission) Then Return False
			Dim that As UnresolvedPermission = CType(obj, UnresolvedPermission)

			' check type
			If Not Me.type.Equals(that.type) Then Return False

			' check name
			If Me.name Is Nothing Then
				If that.name IsNot Nothing Then Return False
			ElseIf Not Me.name.Equals(that.name) Then
				Return False
			End If

			' check actions
			If Me.actions Is Nothing Then
				If that.actions IsNot Nothing Then Return False
			Else
				If Not Me.actions.Equals(that.actions) Then Return False
			End If

			' check certs
			If (Me.certs Is Nothing AndAlso that.certs IsNot Nothing) OrElse (Me.certs IsNot Nothing AndAlso that.certs Is Nothing) OrElse (Me.certs IsNot Nothing AndAlso that.certs IsNot Nothing AndAlso Me.certs.Length <> that.certs.Length) Then Return False

			Dim i, j As Integer
			Dim match As Boolean

			i = 0
			Do While Me.certs IsNot Nothing AndAlso i < Me.certs.Length
				match = False
				For j = 0 To that.certs.Length - 1
					If Me.certs(i).Equals(that.certs(j)) Then
						match = True
						Exit For
					End If
				Next j
				If Not match Then Return False
				i += 1
			Loop

			i = 0
			Do While that.certs IsNot Nothing AndAlso i < that.certs.Length
				match = False
				For j = 0 To Me.certs.Length - 1
					If that.certs(i).Equals(Me.certs(j)) Then
						match = True
						Exit For
					End If
				Next j
				If Not match Then Return False
				i += 1
			Loop
			Return True
		End Function

		''' <summary>
		''' Returns the hash code value for this object.
		''' </summary>
		''' <returns> a hash code value for this object. </returns>

		Public Overrides Function GetHashCode() As Integer
			Dim hash As Integer = type.GetHashCode()
			If name IsNot Nothing Then hash = hash Xor name.GetHashCode()
			If actions IsNot Nothing Then hash = hash Xor actions.GetHashCode()
			Return hash
		End Function

		''' <summary>
		''' Returns the canonical string representation of the actions,
		''' which currently is the empty string "", since there are no actions for
		''' an UnresolvedPermission. That is, the actions for the
		''' permission that will be created when this UnresolvedPermission
		''' is resolved may be non-null, but an UnresolvedPermission
		''' itself is never considered to have any actions.
		''' </summary>
		''' <returns> the empty string "". </returns>
		Public Property Overrides actions As String
			Get
				Return ""
			End Get
		End Property

		''' <summary>
		''' Get the type (class name) of the underlying permission that
		''' has not been resolved.
		''' </summary>
		''' <returns> the type (class name) of the underlying permission that
		'''  has not been resolved
		''' 
		''' @since 1.5 </returns>
		Public Property unresolvedType As String
			Get
				Return type
			End Get
		End Property

		''' <summary>
		''' Get the target name of the underlying permission that
		''' has not been resolved.
		''' </summary>
		''' <returns> the target name of the underlying permission that
		'''          has not been resolved, or {@code null},
		'''          if there is no target name
		''' 
		''' @since 1.5 </returns>
		Public Property unresolvedName As String
			Get
				Return name
			End Get
		End Property

		''' <summary>
		''' Get the actions for the underlying permission that
		''' has not been resolved.
		''' </summary>
		''' <returns> the actions for the underlying permission that
		'''          has not been resolved, or {@code null}
		'''          if there are no actions
		''' 
		''' @since 1.5 </returns>
		Public Property unresolvedActions As String
			Get
				Return actions
			End Get
		End Property

		''' <summary>
		''' Get the signer certificates (without any supporting chain)
		''' for the underlying permission that has not been resolved.
		''' </summary>
		''' <returns> the signer certificates for the underlying permission that
		''' has not been resolved, or null, if there are no signer certificates.
		''' Returns a new array each time this method is called.
		''' 
		''' @since 1.5 </returns>
		Public Property unresolvedCerts As java.security.cert.Certificate()
			Get
				Return If(certs Is Nothing, Nothing, certs.clone())
			End Get
		End Property

		''' <summary>
		''' Returns a string describing this UnresolvedPermission.  The convention
		''' is to specify the class name, the permission name, and the actions, in
		''' the following format: '(unresolved "ClassName" "name" "actions")'.
		''' </summary>
		''' <returns> information about this UnresolvedPermission. </returns>
		Public Overrides Function ToString() As String
			Return "(unresolved " & type & " " & name & " " & actions & ")"
		End Function

		''' <summary>
		''' Returns a new PermissionCollection object for storing
		''' UnresolvedPermission  objects.
		''' <p> </summary>
		''' <returns> a new PermissionCollection object suitable for
		''' storing UnresolvedPermissions. </returns>

		Public Overrides Function newPermissionCollection() As PermissionCollection
			Return New UnresolvedPermissionCollection
		End Function

		''' <summary>
		''' Writes this object out to a stream (i.e., serializes it).
		''' 
		''' @serialData An initial {@code String} denoting the
		''' {@code type} is followed by a {@code String} denoting the
		''' {@code name} is followed by a {@code String} denoting the
		''' {@code actions} is followed by an {@code int} indicating the
		''' number of certificates to follow
		''' (a value of "zero" denotes that there are no certificates associated
		''' with this object).
		''' Each certificate is written out starting with a {@code String}
		''' denoting the certificate type, followed by an
		''' {@code int} specifying the length of the certificate encoding,
		''' followed by the certificate encoding itself which is written out as an
		''' array of bytes.
		''' </summary>
		Private Sub writeObject(ByVal oos As java.io.ObjectOutputStream)
			oos.defaultWriteObject()

			If certs Is Nothing OrElse certs.Length=0 Then
				oos.writeInt(0)
			Else
				' write out the total number of certs
				oos.writeInt(certs.Length)
				' write out each cert, including its type
				For i As Integer = 0 To certs.Length - 1
					Dim cert As java.security.cert.Certificate = certs(i)
					Try
						oos.writeUTF(cert.type)
						Dim encoded As SByte() = cert.encoded
						oos.writeInt(encoded.Length)
						oos.write(encoded)
					Catch cee As CertificateEncodingException
						Throw New java.io.IOException(cee.Message)
					End Try
				Next i
			End If
		End Sub

		''' <summary>
		''' Restores this object from a stream (i.e., deserializes it).
		''' </summary>
		Private Sub readObject(ByVal ois As java.io.ObjectInputStream)
			Dim cf As CertificateFactory
			Dim cfs As Dictionary(Of String, CertificateFactory) = Nothing

			ois.defaultReadObject()

			If type Is Nothing Then Throw New NullPointerException("type can't be null")

			' process any new-style certs in the stream (if present)
			Dim size As Integer = ois.readInt()
			If size > 0 Then
				' we know of 3 different cert types: X.509, PGP, SDSI, which
				' could all be present in the stream at the same time
				cfs = New Dictionary(Of String, CertificateFactory)(3)
				Me.certs = New java.security.cert.Certificate(size - 1){}
			End If

			For i As Integer = 0 To size - 1
				' read the certificate type, and instantiate a certificate
				' factory of that type (reuse existing factory if possible)
				Dim certType As String = ois.readUTF()
				If cfs.ContainsKey(certType) Then
					' reuse certificate factory
					cf = cfs(certType)
				Else
					' create new certificate factory
					Try
						cf = CertificateFactory.getInstance(certType)
					Catch ce As CertificateException
						Throw New ClassNotFoundException("Certificate factory for " & certType & " not found")
					End Try
					' store the certificate factory so we can reuse it later
					cfs(certType) = cf
				End If
				' parse the certificate
				Dim encoded As SByte()=Nothing
				Try
					encoded = New SByte(ois.readInt() - 1){}
				Catch oome As OutOfMemoryError
					Throw New java.io.IOException("Certificate too big")
				End Try
				ois.readFully(encoded)
				Dim bais As New java.io.ByteArrayInputStream(encoded)
				Try
					Me.certs(i) = cf.generateCertificate(bais)
				Catch ce As CertificateException
					Throw New java.io.IOException(ce.Message)
				End Try
				bais.close()
			Next i
		End Sub
	End Class

End Namespace