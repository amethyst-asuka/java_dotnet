Imports System

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
	''' This class encapsulates a long term secret key for a Kerberos
	''' principal.<p>
	''' 
	''' All Kerberos JAAS login modules that obtain a principal's password and
	''' generate the secret key from it should use this class.
	''' Sometimes, such as when authenticating a server in
	''' the absence of user-to-user authentication, the login module will store
	''' an instance of this class in the private credential set of a
	''' <seealso cref="javax.security.auth.Subject Subject"/> during the commit phase of the
	''' authentication process.<p>
	''' 
	''' A Kerberos service using a keytab to read secret keys should use
	''' the <seealso cref="KeyTab"/> class, where latest keys can be read when needed.<p>
	''' 
	''' It might be necessary for the application to be granted a
	''' {@link javax.security.auth.PrivateCredentialPermission
	''' PrivateCredentialPermission} if it needs to access the KerberosKey
	''' instance from a Subject. This permission is not needed when the
	''' application depends on the default JGSS Kerberos mechanism to access the
	''' KerberosKey. In that case, however, the application will need an
	''' appropriate
	''' <seealso cref="javax.security.auth.kerberos.ServicePermission ServicePermission"/>.
	''' 
	''' @author Mayank Upadhyay
	''' @since 1.4
	''' </summary>
	Public Class KerberosKey
		Implements javax.crypto.SecretKey, javax.security.auth.Destroyable

		Private Const serialVersionUID As Long = -4625402278148246993L

	   ''' <summary>
	   ''' The principal that this secret key belongs to.
	   '''  
	   ''' @serial
	   ''' </summary>
		Private principal As KerberosPrincipal

	   ''' <summary>
	   ''' the version number of this secret key
	   '''  
	   ''' @serial
	   ''' </summary>
		Private versionNum As Integer

	   ''' <summary>
	   ''' {@code KeyImpl} is serialized by writing out the ASN1 Encoded bytes
	   ''' of the encryption key.
	   ''' The ASN1 encoding is defined in RFC4120 and as  follows:
	   ''' <pre>
	   ''' EncryptionKey   ::= SEQUENCE {
	   '''           keytype   [0] Int32 -- actually encryption type --,
	   '''           keyvalue  [1] OCTET STRING
	   ''' }
	   ''' </pre>
	   ''' 
	   ''' @serial
	   ''' </summary>

		Private key As KeyImpl
		<NonSerialized> _
		Private destroyed As Boolean = False

		''' <summary>
		''' Constructs a KerberosKey from the given bytes when the key type and
		''' key version number are known. This can be used when reading the secret
		''' key information from a Kerberos "keytab".
		''' </summary>
		''' <param name="principal"> the principal that this secret key belongs to </param>
		''' <param name="keyBytes"> the raw bytes for the secret key </param>
		''' <param name="keyType"> the key type for the secret key as defined by the
		''' Kerberos protocol specification. </param>
		''' <param name="versionNum"> the version number of this secret key </param>
		Public Sub New(ByVal principal As KerberosPrincipal, ByVal keyBytes As SByte(), ByVal keyType As Integer, ByVal versionNum As Integer)
			Me.principal = principal
			Me.versionNum = versionNum
			key = New KeyImpl(keyBytes, keyType)
		End Sub

		''' <summary>
		''' Constructs a KerberosKey from a principal's password.
		''' </summary>
		''' <param name="principal"> the principal that this password belongs to </param>
		''' <param name="password"> the password that should be used to compute the key </param>
		''' <param name="algorithm"> the name for the algorithm that this key will be
		''' used for. This parameter may be null in which case the default
		''' algorithm "DES" will be assumed. </param>
		''' <exception cref="IllegalArgumentException"> if the name of the
		''' algorithm passed is unsupported. </exception>
		Public Sub New(ByVal principal As KerberosPrincipal, ByVal password As Char(), ByVal algorithm As String)

			Me.principal = principal
			' Pass principal in for salt
			key = New KeyImpl(principal, password, algorithm)
		End Sub

		''' <summary>
		''' Returns the principal that this key belongs to.
		''' </summary>
		''' <returns> the principal this key belongs to. </returns>
		Public Property principal As KerberosPrincipal
			Get
				If destroyed Then Throw New IllegalStateException("This key is no longer valid")
				Return principal
			End Get
		End Property

		''' <summary>
		''' Returns the key version number.
		''' </summary>
		''' <returns> the key version number. </returns>
		Public Property versionNumber As Integer
			Get
				If destroyed Then Throw New IllegalStateException("This key is no longer valid")
				Return versionNum
			End Get
		End Property

		''' <summary>
		''' Returns the key type for this long-term key.
		''' </summary>
		''' <returns> the key type. </returns>
		Public Property keyType As Integer
			Get
				If destroyed Then Throw New IllegalStateException("This key is no longer valid")
				Return key.keyType
			End Get
		End Property

	'    
	'     * Methods from java.security.Key
	'     

		''' <summary>
		''' Returns the standard algorithm name for this key. For
		''' example, "DES" would indicate that this key is a DES key.
		''' See Appendix A in the <a href=
		''' "../../../../../technotes/guides/security/crypto/CryptoSpec.html#AppA">
		''' Java Cryptography Architecture API Specification &amp; Reference
		''' </a>
		''' for information about standard algorithm names.
		''' </summary>
		''' <returns> the name of the algorithm associated with this key. </returns>
		Public Property algorithm As String
			Get
				If destroyed Then Throw New IllegalStateException("This key is no longer valid")
				Return key.algorithm
			End Get
		End Property

		''' <summary>
		''' Returns the name of the encoding format for this secret key.
		''' </summary>
		''' <returns> the String "RAW" </returns>
		Public Property format As String
			Get
				If destroyed Then Throw New IllegalStateException("This key is no longer valid")
				Return key.format
			End Get
		End Property

		''' <summary>
		''' Returns the key material of this secret key.
		''' </summary>
		''' <returns> the key material </returns>
		Public Property encoded As SByte()
			Get
				If destroyed Then Throw New IllegalStateException("This key is no longer valid")
				Return key.encoded
			End Get
		End Property

		''' <summary>
		''' Destroys this key. A call to any of its other methods after this
		''' will cause an  IllegalStateException to be thrown.
		''' </summary>
		''' <exception cref="DestroyFailedException"> if some error occurs while destorying
		''' this key. </exception>
		Public Overridable Sub destroy()
			If Not destroyed Then
				key.destroy()
				principal = Nothing
				destroyed = True
			End If
		End Sub


		''' <summary>
		''' Determines if this key has been destroyed. </summary>
		Public Overridable Property destroyed As Boolean
			Get
				Return destroyed
			End Get
		End Property

		Public Overrides Function ToString() As String
			If destroyed Then Return "Destroyed Principal"
			Return "Kerberos Principal " & principal.ToString() & "Key Version " & versionNum & "key " & key.ToString()
		End Function

		''' <summary>
		''' Returns a hashcode for this KerberosKey.
		''' </summary>
		''' <returns> a hashCode() for the {@code KerberosKey}
		''' @since 1.6 </returns>
		Public Overrides Function GetHashCode() As Integer
			Dim result As Integer = 17
			If destroyed Then Return result
			result = 37 * result + java.util.Arrays.hashCode(encoded)
			result = 37 * result + keyType
			If principal IsNot Nothing Then result = 37 * result + principal.GetHashCode()
			Return result * 37 + versionNum
		End Function

		''' <summary>
		''' Compares the specified Object with this KerberosKey for equality.
		''' Returns true if the given object is also a
		''' {@code KerberosKey} and the two
		''' {@code KerberosKey} instances are equivalent.
		''' </summary>
		''' <param name="other"> the Object to compare to </param>
		''' <returns> true if the specified object is equal to this KerberosKey,
		''' false otherwise. NOTE: Returns false if either of the KerberosKey
		''' objects has been destroyed.
		''' @since 1.6 </returns>
		Public Overrides Function Equals(ByVal other As Object) As Boolean

			If other Is Me Then Return True

			If Not(TypeOf other Is KerberosKey) Then Return False

			Dim otherKey As KerberosKey = (CType(other, KerberosKey))
			If destroyed OrElse otherKey.destroyed Then Return False

			If versionNum <> otherKey.versionNumber OrElse keyType <> otherKey.keyType OrElse (Not java.util.Arrays.Equals(encoded, otherKey.encoded)) Then Return False

			If principal Is Nothing Then
				If otherKey.principal IsNot Nothing Then Return False
			Else
				If Not principal.Equals(otherKey.principal) Then Return False
			End If

			Return True
		End Function
	End Class

End Namespace