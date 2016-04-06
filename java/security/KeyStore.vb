Imports Microsoft.VisualBasic
Imports System
Imports System.Runtime.CompilerServices
Imports System.Collections.Generic
Imports javax.security.auth.callback

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
	''' This class represents a storage facility for cryptographic
	''' keys and certificates.
	''' 
	''' <p> A {@code KeyStore} manages different types of entries.
	''' Each type of entry implements the {@code KeyStore.Entry} interface.
	''' Three basic {@code KeyStore.Entry} implementations are provided:
	''' 
	''' <ul>
	''' <li><b>KeyStore.PrivateKeyEntry</b>
	''' <p> This type of entry holds a cryptographic {@code PrivateKey},
	''' which is optionally stored in a protected format to prevent
	''' unauthorized access.  It is also accompanied by a certificate chain
	''' for the corresponding public key.
	''' 
	''' <p> Private keys and certificate chains are used by a given entity for
	''' self-authentication. Applications for this authentication include software
	''' distribution organizations which sign JAR files as part of releasing
	''' and/or licensing software.
	''' 
	''' <li><b>KeyStore.SecretKeyEntry</b>
	''' <p> This type of entry holds a cryptographic {@code SecretKey},
	''' which is optionally stored in a protected format to prevent
	''' unauthorized access.
	''' 
	''' <li><b>KeyStore.TrustedCertificateEntry</b>
	''' <p> This type of entry contains a single public key {@code Certificate}
	''' belonging to another party. It is called a <i>trusted certificate</i>
	''' because the keystore owner trusts that the public key in the certificate
	''' indeed belongs to the identity identified by the <i>subject</i> (owner)
	''' of the certificate.
	''' 
	''' <p>This type of entry can be used to authenticate other parties.
	''' </ul>
	''' 
	''' <p> Each entry in a keystore is identified by an "alias" string. In the
	''' case of private keys and their associated certificate chains, these strings
	''' distinguish among the different ways in which the entity may authenticate
	''' itself. For example, the entity may authenticate itself using different
	''' certificate authorities, or using different public key algorithms.
	''' 
	''' <p> Whether aliases are case sensitive is implementation dependent. In order
	''' to avoid problems, it is recommended not to use aliases in a KeyStore that
	''' only differ in case.
	''' 
	''' <p> Whether keystores are persistent, and the mechanisms used by the
	''' keystore if it is persistent, are not specified here. This allows
	''' use of a variety of techniques for protecting sensitive (e.g., private or
	''' secret) keys. Smart cards or other integrated cryptographic engines
	''' (SafeKeyper) are one option, and simpler mechanisms such as files may also
	''' be used (in a variety of formats).
	''' 
	''' <p> Typical ways to request a KeyStore object include
	''' relying on the default type and providing a specific keystore type.
	''' 
	''' <ul>
	''' <li>To rely on the default type:
	''' <pre>
	'''    KeyStore ks = KeyStore.getInstance(KeyStore.getDefaultType());
	''' </pre>
	''' The system will return a keystore implementation for the default type.
	''' 
	''' <li>To provide a specific keystore type:
	''' <pre>
	'''      KeyStore ks = KeyStore.getInstance("JKS");
	''' </pre>
	''' The system will return the most preferred implementation of the
	''' specified keystore type available in the environment. <p>
	''' </ul>
	''' 
	''' <p> Before a keystore can be accessed, it must be
	''' <seealso cref="#load(java.io.InputStream, char[]) loaded"/>.
	''' <pre>
	'''    KeyStore ks = KeyStore.getInstance(KeyStore.getDefaultType());
	''' 
	'''    // get user password and file input stream
	'''    char[] password = getPassword();
	''' 
	'''    try (FileInputStream fis = new FileInputStream("keyStoreName")) {
	'''        ks.load(fis, password);
	'''    }
	''' </pre>
	''' 
	''' To create an empty keystore using the above {@code load} method,
	''' pass {@code null} as the {@code InputStream} argument.
	''' 
	''' <p> Once the keystore has been loaded, it is possible
	''' to read existing entries from the keystore, or to write new entries
	''' into the keystore:
	''' <pre>
	'''    KeyStore.ProtectionParameter protParam =
	'''        new KeyStore.PasswordProtection(password);
	''' 
	'''    // get my private key
	'''    KeyStore.PrivateKeyEntry pkEntry = (KeyStore.PrivateKeyEntry)
	'''        ks.getEntry("privateKeyAlias", protParam);
	'''    PrivateKey myPrivateKey = pkEntry.getPrivateKey();
	''' 
	'''    // save my secret key
	'''    javax.crypto.SecretKey mySecretKey;
	'''    KeyStore.SecretKeyEntry skEntry =
	'''        new KeyStore.SecretKeyEntry(mySecretKey);
	'''    ks.setEntry("secretKeyAlias", skEntry, protParam);
	''' 
	'''    // store away the keystore
	'''    try (FileOutputStream fos = new FileOutputStream("newKeyStoreName")) {
	'''        ks.store(fos, password);
	'''    }
	''' </pre>
	''' 
	''' Note that although the same password may be used to
	''' load the keystore, to protect the private key entry,
	''' to protect the secret key entry, and to store the keystore
	''' (as is shown in the sample code above),
	''' different passwords or other protection parameters
	''' may also be used.
	''' 
	''' <p> Every implementation of the Java platform is required to support
	''' the following standard {@code KeyStore} type:
	''' <ul>
	''' <li>{@code PKCS12}</li>
	''' </ul>
	''' This type is described in the <a href=
	''' "{@docRoot}/../technotes/guides/security/StandardNames.html#KeyStore">
	''' KeyStore section</a> of the
	''' Java Cryptography Architecture Standard Algorithm Name Documentation.
	''' Consult the release documentation for your implementation to see if any
	''' other types are supported.
	''' 
	''' @author Jan Luehe
	''' </summary>
	''' <seealso cref= java.security.PrivateKey </seealso>
	''' <seealso cref= javax.crypto.SecretKey </seealso>
	''' <seealso cref= java.security.cert.Certificate
	''' 
	''' @since 1.2 </seealso>

	Public Class KeyStore

		Private Shared ReadOnly pdebug As sun.security.util.Debug = sun.security.util.Debug.getInstance("provider", "Provider")
		Private Shared ReadOnly skipDebug As Boolean = sun.security.util.Debug.isOn("engine=") AndAlso Not sun.security.util.Debug.isOn("keystore")

	'    
	'     * Constant to lookup in the Security properties file to determine
	'     * the default keystore type.
	'     * In the Security properties file, the default keystore type is given as:
	'     * <pre>
	'     * keystore.type=jks
	'     * </pre>
	'     
		Private Const KEYSTORE_TYPE As String = "keystore.type"

		' The keystore type
		Private type As String

		' The provider
		Private provider As Provider

		' The provider implementation
		Private keyStoreSpi As KeyStoreSpi

		' Has this keystore been initialized (loaded)?
		Private initialized As Boolean = False

		''' <summary>
		''' A marker interface for {@code KeyStore}
		''' <seealso cref="#load(KeyStore.LoadStoreParameter) load"/>
		''' and
		''' <seealso cref="#store(KeyStore.LoadStoreParameter) store"/>
		''' parameters.
		''' 
		''' @since 1.5
		''' </summary>
		Public Interface LoadStoreParameter
			''' <summary>
			''' Gets the parameter used to protect keystore data.
			''' </summary>
			''' <returns> the parameter used to protect keystore data, or null </returns>
			ReadOnly Property protectionParameter As ProtectionParameter
		End Interface

		''' <summary>
		''' A marker interface for keystore protection parameters.
		''' 
		''' <p> The information stored in a {@code ProtectionParameter}
		''' object protects the contents of a keystore.
		''' For example, protection parameters may be used to check
		''' the integrity of keystore data, or to protect the
		''' confidentiality of sensitive keystore data
		''' (such as a {@code PrivateKey}).
		''' 
		''' @since 1.5
		''' </summary>
		Public Interface ProtectionParameter
		End Interface

		''' <summary>
		''' A password-based implementation of {@code ProtectionParameter}.
		''' 
		''' @since 1.5
		''' </summary>
		Public Class PasswordProtection
			Implements ProtectionParameter, javax.security.auth.Destroyable

			Private ReadOnly password As Char()
			Private ReadOnly protectionAlgorithm As String
			Private ReadOnly protectionParameters As java.security.spec.AlgorithmParameterSpec
'JAVA TO VB CONVERTER TODO TASK: There is no VB equivalent to 'volatile':
			Private destroyed As Boolean = False

			''' <summary>
			''' Creates a password parameter.
			''' 
			''' <p> The specified {@code password} is cloned before it is stored
			''' in the new {@code PasswordProtection} object.
			''' </summary>
			''' <param name="password"> the password, which may be {@code null} </param>
			Public Sub New(  password As Char())
				Me.password = If(password Is Nothing, Nothing, password.clone())
				Me.protectionAlgorithm = Nothing
				Me.protectionParameters = Nothing
			End Sub

			''' <summary>
			''' Creates a password parameter and specifies the protection algorithm
			''' and associated parameters to use when encrypting a keystore entry.
			''' <p>
			''' The specified {@code password} is cloned before it is stored in the
			''' new {@code PasswordProtection} object.
			''' </summary>
			''' <param name="password"> the password, which may be {@code null} </param>
			''' <param name="protectionAlgorithm"> the encryption algorithm name, for
			'''     example, {@code PBEWithHmacSHA256AndAES_256}.
			'''     See the Cipher section in the <a href=
			''' "{@docRoot}/../technotes/guides/security/StandardNames.html#Cipher">
			''' Java Cryptography Architecture Standard Algorithm Name
			''' Documentation</a>
			'''     for information about standard encryption algorithm names. </param>
			''' <param name="protectionParameters"> the encryption algorithm parameter
			'''     specification, which may be {@code null} </param>
			''' <exception cref="NullPointerException"> if {@code protectionAlgorithm} is
			'''     {@code null}
			''' 
			''' @since 1.8 </exception>
			Public Sub New(  password As Char(),   protectionAlgorithm As String,   protectionParameters As java.security.spec.AlgorithmParameterSpec)
				If protectionAlgorithm Is Nothing Then Throw New NullPointerException("invalid null input")
				Me.password = If(password Is Nothing, Nothing, password.clone())
				Me.protectionAlgorithm = protectionAlgorithm
				Me.protectionParameters = protectionParameters
			End Sub

			''' <summary>
			''' Gets the name of the protection algorithm.
			''' If none was set then the keystore provider will use its default
			''' protection algorithm. The name of the default protection algorithm
			''' for a given keystore type is set using the
			''' {@code 'keystore.<type>.keyProtectionAlgorithm'} security property.
			''' For example, the
			''' {@code keystore.PKCS12.keyProtectionAlgorithm} property stores the
			''' name of the default key protection algorithm used for PKCS12
			''' keystores. If the security property is not set, an
			''' implementation-specific algorithm will be used.
			''' </summary>
			''' <returns> the algorithm name, or {@code null} if none was set
			''' 
			''' @since 1.8 </returns>
			Public Overridable Property protectionAlgorithm As String
				Get
					Return protectionAlgorithm
				End Get
			End Property

			''' <summary>
			''' Gets the parameters supplied for the protection algorithm.
			''' </summary>
			''' <returns> the algorithm parameter specification, or {@code  null},
			'''     if none was set
			''' 
			''' @since 1.8 </returns>
			Public Overridable Property protectionParameters As java.security.spec.AlgorithmParameterSpec
				Get
					Return protectionParameters
				End Get
			End Property

			''' <summary>
			''' Gets the password.
			''' 
			''' <p>Note that this method returns a reference to the password.
			''' If a clone of the array is created it is the caller's
			''' responsibility to zero out the password information
			''' after it is no longer needed.
			''' </summary>
			''' <seealso cref= #destroy() </seealso>
			''' <returns> the password, which may be {@code null} </returns>
			''' <exception cref="IllegalStateException"> if the password has
			'''              been cleared (destroyed) </exception>
			<MethodImpl(MethodImplOptions.Synchronized)> _
			Public Overridable Property password As Char()
				Get
					If destroyed Then Throw New IllegalStateException("password has been cleared")
					Return password
				End Get
			End Property

			''' <summary>
			''' Clears the password.
			''' </summary>
			''' <exception cref="DestroyFailedException"> if this method was unable
			'''      to clear the password </exception>
			<MethodImpl(MethodImplOptions.Synchronized)> _
			Public Overridable Sub destroy()
				destroyed = True
				If password IsNot Nothing Then Arrays.fill(password, " "c)
			End Sub

			''' <summary>
			''' Determines if password has been cleared.
			''' </summary>
			''' <returns> true if the password has been cleared, false otherwise </returns>
			<MethodImpl(MethodImplOptions.Synchronized)> _
			Public Overridable Property destroyed As Boolean
				Get
					Return destroyed
				End Get
			End Property
		End Class

		''' <summary>
		''' A ProtectionParameter encapsulating a CallbackHandler.
		''' 
		''' @since 1.5
		''' </summary>
		Public Class CallbackHandlerProtection
			Implements ProtectionParameter

			Private ReadOnly handler As CallbackHandler

			''' <summary>
			''' Constructs a new CallbackHandlerProtection from a
			''' CallbackHandler.
			''' </summary>
			''' <param name="handler"> the CallbackHandler </param>
			''' <exception cref="NullPointerException"> if handler is null </exception>
			Public Sub New(  handler As CallbackHandler)
				If handler Is Nothing Then Throw New NullPointerException("handler must not be null")
				Me.handler = handler
			End Sub

			''' <summary>
			''' Returns the CallbackHandler.
			''' </summary>
			''' <returns> the CallbackHandler. </returns>
			Public Overridable Property callbackHandler As CallbackHandler
				Get
					Return handler
				End Get
			End Property

		End Class

		''' <summary>
		''' A marker interface for {@code KeyStore} entry types.
		''' 
		''' @since 1.5
		''' </summary>
		Public Interface Entry

			''' <summary>
			''' Retrieves the attributes associated with an entry.
			''' <p>
			''' The default implementation returns an empty {@code Set}.
			''' </summary>
			''' <returns> an unmodifiable {@code Set} of attributes, possibly empty
			''' 
			''' @since 1.8 </returns>
			ReadOnly Property default attributes As [Set](Of Attribute)
				Function Collections.emptySet() As [Return](Of Attribute)

			''' <summary>
			''' An attribute associated with a keystore entry.
			''' It comprises a name and one or more values.
			''' 
			''' @since 1.8
			''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Interface inner types are not converted:
'			public interface Attribute
	'		{
	'			''' <summary>
	'			''' Returns the attribute's name.
	'			''' </summary>
	'			''' <returns> the attribute name </returns>
	'			public String getName();
	'
	'			''' <summary>
	'			''' Returns the attribute's value.
	'			''' Multi-valued attributes encode their values as a single string.
	'			''' </summary>
	'			''' <returns> the attribute value </returns>
	'			public String getValue();
	'		}
		End Interface

		''' <summary>
		''' A {@code KeyStore} entry that holds a {@code PrivateKey}
		''' and corresponding certificate chain.
		''' 
		''' @since 1.5
		''' </summary>
		Public NotInheritable Class PrivateKeyEntry
			Implements Entry

			Private ReadOnly privKey As PrivateKey
			Private ReadOnly chain As java.security.cert.Certificate()
			Private ReadOnly attributes As [Set](Of Attribute)

			''' <summary>
			''' Constructs a {@code PrivateKeyEntry} with a
			''' {@code PrivateKey} and corresponding certificate chain.
			''' 
			''' <p> The specified {@code chain} is cloned before it is stored
			''' in the new {@code PrivateKeyEntry} object.
			''' </summary>
			''' <param name="privateKey"> the {@code PrivateKey} </param>
			''' <param name="chain"> an array of {@code Certificate}s
			'''      representing the certificate chain.
			'''      The chain must be ordered and contain a
			'''      {@code Certificate} at index 0
			'''      corresponding to the private key.
			''' </param>
			''' <exception cref="NullPointerException"> if
			'''      {@code privateKey} or {@code chain}
			'''      is {@code null} </exception>
			''' <exception cref="IllegalArgumentException"> if the specified chain has a
			'''      length of 0, if the specified chain does not contain
			'''      {@code Certificate}s of the same type,
			'''      or if the {@code PrivateKey} algorithm
			'''      does not match the algorithm of the {@code PublicKey}
			'''      in the end entity {@code Certificate} (at index 0) </exception>
			Public Sub New(  privateKey As PrivateKey,   chain As java.security.cert.Certificate())
				Me.New(privateKey, chain, Collections.emptySet(Of Attribute)())
			End Sub

			''' <summary>
			''' Constructs a {@code PrivateKeyEntry} with a {@code PrivateKey} and
			''' corresponding certificate chain and associated entry attributes.
			''' 
			''' <p> The specified {@code chain} and {@code attributes} are cloned
			''' before they are stored in the new {@code PrivateKeyEntry} object.
			''' </summary>
			''' <param name="privateKey"> the {@code PrivateKey} </param>
			''' <param name="chain"> an array of {@code Certificate}s
			'''      representing the certificate chain.
			'''      The chain must be ordered and contain a
			'''      {@code Certificate} at index 0
			'''      corresponding to the private key. </param>
			''' <param name="attributes"> the attributes
			''' </param>
			''' <exception cref="NullPointerException"> if {@code privateKey}, {@code chain}
			'''      or {@code attributes} is {@code null} </exception>
			''' <exception cref="IllegalArgumentException"> if the specified chain has a
			'''      length of 0, if the specified chain does not contain
			'''      {@code Certificate}s of the same type,
			'''      or if the {@code PrivateKey} algorithm
			'''      does not match the algorithm of the {@code PublicKey}
			'''      in the end entity {@code Certificate} (at index 0)
			''' 
			''' @since 1.8 </exception>
			Public Sub New(  privateKey As PrivateKey,   chain As java.security.cert.Certificate(),   attributes As [Set](Of Attribute))

				If privateKey Is Nothing OrElse chain Is Nothing OrElse attributes Is Nothing Then Throw New NullPointerException("invalid null input")
				If chain.Length = 0 Then Throw New IllegalArgumentException("invalid zero-length input chain")

				Dim clonedChain As java.security.cert.Certificate() = chain.clone()
				Dim certType As String = clonedChain(0).type
				For i As Integer = 1 To clonedChain.Length - 1
					If Not certType.Equals(clonedChain(i).type) Then Throw New IllegalArgumentException("chain does not contain certificates " & "of the same type")
				Next i
				If Not privateKey.algorithm.Equals(clonedChain(0).publicKey.algorithm) Then Throw New IllegalArgumentException("private key algorithm does not match " & "algorithm of public key in end entity " & "certificate (at index 0)")
				Me.privKey = privateKey

				If TypeOf clonedChain(0) Is java.security.cert.X509Certificate AndAlso Not(TypeOf clonedChain Is java.security.cert.X509Certificate()) Then

					Me.chain = New java.security.cert.X509Certificate(clonedChain.Length - 1){}
					Array.Copy(clonedChain, 0, Me.chain, 0, clonedChain.Length)
				Else
					Me.chain = clonedChain
				End If

				Me.attributes = Collections.unmodifiableSet(New HashSet(Of )(attributes))
			End Sub

			''' <summary>
			''' Gets the {@code PrivateKey} from this entry.
			''' </summary>
			''' <returns> the {@code PrivateKey} from this entry </returns>
			Public Property privateKey As PrivateKey
				Get
					Return privKey
				End Get
			End Property

			''' <summary>
			''' Gets the {@code Certificate} chain from this entry.
			''' 
			''' <p> The stored chain is cloned before being returned.
			''' </summary>
			''' <returns> an array of {@code Certificate}s corresponding
			'''      to the certificate chain for the public key.
			'''      If the certificates are of type X.509,
			'''      the runtime type of the returned array is
			'''      {@code X509Certificate[]}. </returns>
			Public Property certificateChain As java.security.cert.Certificate()
				Get
					Return chain.clone()
				End Get
			End Property

			''' <summary>
			''' Gets the end entity {@code Certificate}
			''' from the certificate chain in this entry.
			''' </summary>
			''' <returns> the end entity {@code Certificate} (at index 0)
			'''      from the certificate chain in this entry.
			'''      If the certificate is of type X.509,
			'''      the runtime type of the returned certificate is
			'''      {@code X509Certificate}. </returns>
			Public Property certificate As java.security.cert.Certificate
				Get
					Return chain(0)
				End Get
			End Property

			''' <summary>
			''' Retrieves the attributes associated with an entry.
			''' <p>
			''' </summary>
			''' <returns> an unmodifiable {@code Set} of attributes, possibly empty
			''' 
			''' @since 1.8 </returns>
			Public  Overrides ReadOnly Property  attributes As [Set](Of Attribute)
				Get
					Return attributes
				End Get
			End Property

			''' <summary>
			''' Returns a string representation of this PrivateKeyEntry. </summary>
			''' <returns> a string representation of this PrivateKeyEntry. </returns>
			Public Overrides Function ToString() As String
				Dim sb As New StringBuilder
				sb.append("Private key entry and certificate chain with " & chain.Length & " elements:" & vbCrLf)
				For Each cert As java.security.cert.Certificate In chain
					sb.append(cert)
					sb.append(vbCrLf)
				Next cert
				Return sb.ToString()
			End Function

		End Class

		''' <summary>
		''' A {@code KeyStore} entry that holds a {@code SecretKey}.
		''' 
		''' @since 1.5
		''' </summary>
		Public NotInheritable Class SecretKeyEntry
			Implements Entry

			Private ReadOnly sKey As javax.crypto.SecretKey
			Private ReadOnly attributes As [Set](Of Attribute)

			''' <summary>
			''' Constructs a {@code SecretKeyEntry} with a
			''' {@code SecretKey}.
			''' </summary>
			''' <param name="secretKey"> the {@code SecretKey}
			''' </param>
			''' <exception cref="NullPointerException"> if {@code secretKey}
			'''      is {@code null} </exception>
			Public Sub New(  secretKey As javax.crypto.SecretKey)
				If secretKey Is Nothing Then Throw New NullPointerException("invalid null input")
				Me.sKey = secretKey
				Me.attributes = Collections.emptySet(Of Attribute)()
			End Sub

			''' <summary>
			''' Constructs a {@code SecretKeyEntry} with a {@code SecretKey} and
			''' associated entry attributes.
			''' 
			''' <p> The specified {@code attributes} is cloned before it is stored
			''' in the new {@code SecretKeyEntry} object.
			''' </summary>
			''' <param name="secretKey"> the {@code SecretKey} </param>
			''' <param name="attributes"> the attributes
			''' </param>
			''' <exception cref="NullPointerException"> if {@code secretKey} or
			'''     {@code attributes} is {@code null}
			''' 
			''' @since 1.8 </exception>
			Public Sub New(  secretKey As javax.crypto.SecretKey,   attributes As [Set](Of Attribute))

				If secretKey Is Nothing OrElse attributes Is Nothing Then Throw New NullPointerException("invalid null input")
				Me.sKey = secretKey
				Me.attributes = Collections.unmodifiableSet(New HashSet(Of )(attributes))
			End Sub

			''' <summary>
			''' Gets the {@code SecretKey} from this entry.
			''' </summary>
			''' <returns> the {@code SecretKey} from this entry </returns>
			Public Property secretKey As javax.crypto.SecretKey
				Get
					Return sKey
				End Get
			End Property

			''' <summary>
			''' Retrieves the attributes associated with an entry.
			''' <p>
			''' </summary>
			''' <returns> an unmodifiable {@code Set} of attributes, possibly empty
			''' 
			''' @since 1.8 </returns>
			Public  Overrides ReadOnly Property  attributes As [Set](Of Attribute)
				Get
					Return attributes
				End Get
			End Property

			''' <summary>
			''' Returns a string representation of this SecretKeyEntry. </summary>
			''' <returns> a string representation of this SecretKeyEntry. </returns>
			Public Overrides Function ToString() As String
				Return "Secret key entry with algorithm " & sKey.algorithm
			End Function
		End Class

		''' <summary>
		''' A {@code KeyStore} entry that holds a trusted
		''' {@code Certificate}.
		''' 
		''' @since 1.5
		''' </summary>
		Public NotInheritable Class TrustedCertificateEntry
			Implements Entry

			Private ReadOnly cert As java.security.cert.Certificate
			Private ReadOnly attributes As [Set](Of Attribute)

			''' <summary>
			''' Constructs a {@code TrustedCertificateEntry} with a
			''' trusted {@code Certificate}.
			''' </summary>
			''' <param name="trustedCert"> the trusted {@code Certificate}
			''' </param>
			''' <exception cref="NullPointerException"> if
			'''      {@code trustedCert} is {@code null} </exception>
			Public Sub New(  trustedCert As java.security.cert.Certificate)
				If trustedCert Is Nothing Then Throw New NullPointerException("invalid null input")
				Me.cert = trustedCert
				Me.attributes = Collections.emptySet(Of Attribute)()
			End Sub

			''' <summary>
			''' Constructs a {@code TrustedCertificateEntry} with a
			''' trusted {@code Certificate} and associated entry attributes.
			''' 
			''' <p> The specified {@code attributes} is cloned before it is stored
			''' in the new {@code TrustedCertificateEntry} object.
			''' </summary>
			''' <param name="trustedCert"> the trusted {@code Certificate} </param>
			''' <param name="attributes"> the attributes
			''' </param>
			''' <exception cref="NullPointerException"> if {@code trustedCert} or
			'''     {@code attributes} is {@code null}
			''' 
			''' @since 1.8 </exception>
			Public Sub New(  trustedCert As java.security.cert.Certificate,   attributes As [Set](Of Attribute))
				If trustedCert Is Nothing OrElse attributes Is Nothing Then Throw New NullPointerException("invalid null input")
				Me.cert = trustedCert
				Me.attributes = Collections.unmodifiableSet(New HashSet(Of )(attributes))
			End Sub

			''' <summary>
			''' Gets the trusted {@code Certficate} from this entry.
			''' </summary>
			''' <returns> the trusted {@code Certificate} from this entry </returns>
			Public Property trustedCertificate As java.security.cert.Certificate
				Get
					Return cert
				End Get
			End Property

			''' <summary>
			''' Retrieves the attributes associated with an entry.
			''' <p>
			''' </summary>
			''' <returns> an unmodifiable {@code Set} of attributes, possibly empty
			''' 
			''' @since 1.8 </returns>
			Public  Overrides ReadOnly Property  attributes As [Set](Of Attribute)
				Get
					Return attributes
				End Get
			End Property

			''' <summary>
			''' Returns a string representation of this TrustedCertificateEntry. </summary>
			''' <returns> a string representation of this TrustedCertificateEntry. </returns>
			Public Overrides Function ToString() As String
				Return "Trusted certificate entry:" & vbCrLf & cert.ToString()
			End Function
		End Class

		''' <summary>
		''' Creates a KeyStore object of the given type, and encapsulates the given
		''' provider implementation (SPI object) in it.
		''' </summary>
		''' <param name="keyStoreSpi"> the provider implementation. </param>
		''' <param name="provider"> the provider. </param>
		''' <param name="type"> the keystore type. </param>
		Protected Friend Sub New(  keyStoreSpi As KeyStoreSpi,   provider_Renamed As Provider,   type As String)
			Me.keyStoreSpi = keyStoreSpi
			Me.provider = provider_Renamed
			Me.type = type

			If (Not skipDebug) AndAlso pdebug IsNot Nothing Then pdebug.println("KeyStore." & type.ToUpper() & " type from: " & Me.provider.name)
		End Sub

		''' <summary>
		''' Returns a keystore object of the specified type.
		''' 
		''' <p> This method traverses the list of registered security Providers,
		''' starting with the most preferred Provider.
		''' A new KeyStore object encapsulating the
		''' KeyStoreSpi implementation from the first
		''' Provider that supports the specified type is returned.
		''' 
		''' <p> Note that the list of registered providers may be retrieved via
		''' the <seealso cref="Security#getProviders() Security.getProviders()"/> method.
		''' </summary>
		''' <param name="type"> the type of keystore.
		''' See the KeyStore section in the <a href=
		''' "{@docRoot}/../technotes/guides/security/StandardNames.html#KeyStore">
		''' Java Cryptography Architecture Standard Algorithm Name Documentation</a>
		''' for information about standard keystore types.
		''' </param>
		''' <returns> a keystore object of the specified type.
		''' </returns>
		''' <exception cref="KeyStoreException"> if no Provider supports a
		'''          KeyStoreSpi implementation for the
		'''          specified type.
		''' </exception>
		''' <seealso cref= Provider </seealso>
		Public Shared Function getInstance(  type As String) As KeyStore
			Try
				Dim objs As Object() = Security.getImpl(type, "KeyStore", CStr(Nothing))
				Return New KeyStore(CType(objs(0), KeyStoreSpi), CType(objs(1), Provider), type)
			Catch nsae As NoSuchAlgorithmException
				Throw New KeyStoreException(type & " not found", nsae)
			Catch nspe As NoSuchProviderException
				Throw New KeyStoreException(type & " not found", nspe)
			End Try
		End Function

		''' <summary>
		''' Returns a keystore object of the specified type.
		''' 
		''' <p> A new KeyStore object encapsulating the
		''' KeyStoreSpi implementation from the specified provider
		''' is returned.  The specified provider must be registered
		''' in the security provider list.
		''' 
		''' <p> Note that the list of registered providers may be retrieved via
		''' the <seealso cref="Security#getProviders() Security.getProviders()"/> method.
		''' </summary>
		''' <param name="type"> the type of keystore.
		''' See the KeyStore section in the <a href=
		''' "{@docRoot}/../technotes/guides/security/StandardNames.html#KeyStore">
		''' Java Cryptography Architecture Standard Algorithm Name Documentation</a>
		''' for information about standard keystore types.
		''' </param>
		''' <param name="provider"> the name of the provider.
		''' </param>
		''' <returns> a keystore object of the specified type.
		''' </returns>
		''' <exception cref="KeyStoreException"> if a KeyStoreSpi
		'''          implementation for the specified type is not
		'''          available from the specified provider.
		''' </exception>
		''' <exception cref="NoSuchProviderException"> if the specified provider is not
		'''          registered in the security provider list.
		''' </exception>
		''' <exception cref="IllegalArgumentException"> if the provider name is null
		'''          or empty.
		''' </exception>
		''' <seealso cref= Provider </seealso>
		Public Shared Function getInstance(  type As String,   provider_Renamed As String) As KeyStore
			If provider_Renamed Is Nothing OrElse provider_Renamed.length() = 0 Then Throw New IllegalArgumentException("missing provider")
			Try
				Dim objs As Object() = Security.getImpl(type, "KeyStore", provider_Renamed)
				Return New KeyStore(CType(objs(0), KeyStoreSpi), CType(objs(1), Provider), type)
			Catch nsae As NoSuchAlgorithmException
				Throw New KeyStoreException(type & " not found", nsae)
			End Try
		End Function

		''' <summary>
		''' Returns a keystore object of the specified type.
		''' 
		''' <p> A new KeyStore object encapsulating the
		''' KeyStoreSpi implementation from the specified Provider
		''' object is returned.  Note that the specified Provider object
		''' does not have to be registered in the provider list.
		''' </summary>
		''' <param name="type"> the type of keystore.
		''' See the KeyStore section in the <a href=
		''' "{@docRoot}/../technotes/guides/security/StandardNames.html#KeyStore">
		''' Java Cryptography Architecture Standard Algorithm Name Documentation</a>
		''' for information about standard keystore types.
		''' </param>
		''' <param name="provider"> the provider.
		''' </param>
		''' <returns> a keystore object of the specified type.
		''' </returns>
		''' <exception cref="KeyStoreException"> if KeyStoreSpi
		'''          implementation for the specified type is not available
		'''          from the specified Provider object.
		''' </exception>
		''' <exception cref="IllegalArgumentException"> if the specified provider is null.
		''' </exception>
		''' <seealso cref= Provider
		''' 
		''' @since 1.4 </seealso>
		Public Shared Function getInstance(  type As String,   provider_Renamed As Provider) As KeyStore
			If provider_Renamed Is Nothing Then Throw New IllegalArgumentException("missing provider")
			Try
				Dim objs As Object() = Security.getImpl(type, "KeyStore", provider_Renamed)
				Return New KeyStore(CType(objs(0), KeyStoreSpi), CType(objs(1), Provider), type)
			Catch nsae As NoSuchAlgorithmException
				Throw New KeyStoreException(type & " not found", nsae)
			End Try
		End Function

		''' <summary>
		''' Returns the default keystore type as specified by the
		''' {@code keystore.type} security property, or the string
		''' {@literal "jks"} (acronym for {@literal "Java keystore"})
		''' if no such property exists.
		''' 
		''' <p>The default keystore type can be used by applications that do not
		''' want to use a hard-coded keystore type when calling one of the
		''' {@code getInstance} methods, and want to provide a default keystore
		''' type in case a user does not specify its own.
		''' 
		''' <p>The default keystore type can be changed by setting the value of the
		''' {@code keystore.type} security property to the desired keystore type.
		''' </summary>
		''' <returns> the default keystore type as specified by the
		''' {@code keystore.type} security property, or the string {@literal "jks"}
		''' if no such property exists. </returns>
		''' <seealso cref= java.security.Security security properties </seealso>
		PublicShared ReadOnly PropertydefaultType As String
			Get
				Dim kstype As String
				kstype = AccessController.doPrivileged(New PrivilegedActionAnonymousInnerClassHelper(Of T)
				If kstype Is Nothing Then kstype = "jks"
				Return kstype
			End Get
		End Property

		Private Class PrivilegedActionAnonymousInnerClassHelper(Of T)
			Implements PrivilegedAction(Of T)

			Public Overridable Function run() As String
				Return Security.getProperty(KEYSTORE_TYPE)
			End Function
		End Class

		''' <summary>
		''' Returns the provider of this keystore.
		''' </summary>
		''' <returns> the provider of this keystore. </returns>
		Public Property provider As Provider
			Get
				Return Me.provider
			End Get
		End Property

		''' <summary>
		''' Returns the type of this keystore.
		''' </summary>
		''' <returns> the type of this keystore. </returns>
		Public Property type As String
			Get
				Return Me.type
			End Get
		End Property

		''' <summary>
		''' Returns the key associated with the given alias, using the given
		''' password to recover it.  The key must have been associated with
		''' the alias by a call to {@code setKeyEntry},
		''' or by a call to {@code setEntry} with a
		''' {@code PrivateKeyEntry} or {@code SecretKeyEntry}.
		''' </summary>
		''' <param name="alias"> the alias name </param>
		''' <param name="password"> the password for recovering the key
		''' </param>
		''' <returns> the requested key, or null if the given alias does not exist
		''' or does not identify a key-related entry.
		''' </returns>
		''' <exception cref="KeyStoreException"> if the keystore has not been initialized
		''' (loaded). </exception>
		''' <exception cref="NoSuchAlgorithmException"> if the algorithm for recovering the
		''' key cannot be found </exception>
		''' <exception cref="UnrecoverableKeyException"> if the key cannot be recovered
		''' (e.g., the given password is wrong). </exception>
		Public Function getKey(  [alias] As String,   password As Char()) As Key
			If Not initialized Then Throw New KeyStoreException("Uninitialized keystore")
			Return keyStoreSpi.engineGetKey([alias], password)
		End Function

		''' <summary>
		''' Returns the certificate chain associated with the given alias.
		''' The certificate chain must have been associated with the alias
		''' by a call to {@code setKeyEntry},
		''' or by a call to {@code setEntry} with a
		''' {@code PrivateKeyEntry}.
		''' </summary>
		''' <param name="alias"> the alias name
		''' </param>
		''' <returns> the certificate chain (ordered with the user's certificate first
		''' followed by zero or more certificate authorities), or null if the given alias
		''' does not exist or does not contain a certificate chain
		''' </returns>
		''' <exception cref="KeyStoreException"> if the keystore has not been initialized
		''' (loaded). </exception>
		Public Function getCertificateChain(  [alias] As String) As java.security.cert.Certificate()
			If Not initialized Then Throw New KeyStoreException("Uninitialized keystore")
			Return keyStoreSpi.engineGetCertificateChain([alias])
		End Function

		''' <summary>
		''' Returns the certificate associated with the given alias.
		''' 
		''' <p> If the given alias name identifies an entry
		''' created by a call to {@code setCertificateEntry},
		''' or created by a call to {@code setEntry} with a
		''' {@code TrustedCertificateEntry},
		''' then the trusted certificate contained in that entry is returned.
		''' 
		''' <p> If the given alias name identifies an entry
		''' created by a call to {@code setKeyEntry},
		''' or created by a call to {@code setEntry} with a
		''' {@code PrivateKeyEntry},
		''' then the first element of the certificate chain in that entry
		''' is returned.
		''' </summary>
		''' <param name="alias"> the alias name
		''' </param>
		''' <returns> the certificate, or null if the given alias does not exist or
		''' does not contain a certificate.
		''' </returns>
		''' <exception cref="KeyStoreException"> if the keystore has not been initialized
		''' (loaded). </exception>
		Public Function getCertificate(  [alias] As String) As java.security.cert.Certificate
			If Not initialized Then Throw New KeyStoreException("Uninitialized keystore")
			Return keyStoreSpi.engineGetCertificate([alias])
		End Function

		''' <summary>
		''' Returns the creation date of the entry identified by the given alias.
		''' </summary>
		''' <param name="alias"> the alias name
		''' </param>
		''' <returns> the creation date of this entry, or null if the given alias does
		''' not exist
		''' </returns>
		''' <exception cref="KeyStoreException"> if the keystore has not been initialized
		''' (loaded). </exception>
		Public Function getCreationDate(  [alias] As String) As Date
			If Not initialized Then Throw New KeyStoreException("Uninitialized keystore")
			Return keyStoreSpi.engineGetCreationDate([alias])
		End Function

		''' <summary>
		''' Assigns the given key to the given alias, protecting it with the given
		''' password.
		''' 
		''' <p>If the given key is of type {@code java.security.PrivateKey},
		''' it must be accompanied by a certificate chain certifying the
		''' corresponding public key.
		''' 
		''' <p>If the given alias already exists, the keystore information
		''' associated with it is overridden by the given key (and possibly
		''' certificate chain).
		''' </summary>
		''' <param name="alias"> the alias name </param>
		''' <param name="key"> the key to be associated with the alias </param>
		''' <param name="password"> the password to protect the key </param>
		''' <param name="chain"> the certificate chain for the corresponding public
		''' key (only required if the given key is of type
		''' {@code java.security.PrivateKey}).
		''' </param>
		''' <exception cref="KeyStoreException"> if the keystore has not been initialized
		''' (loaded), the given key cannot be protected, or this operation fails
		''' for some other reason </exception>
		Public Sub setKeyEntry(  [alias] As String,   key As Key,   password As Char(),   chain As java.security.cert.Certificate())
			If Not initialized Then Throw New KeyStoreException("Uninitialized keystore")
			If (TypeOf key Is PrivateKey) AndAlso (chain Is Nothing OrElse chain.Length = 0) Then Throw New IllegalArgumentException("Private key must be " & "accompanied by certificate " & "chain")
			keyStoreSpi.engineSetKeyEntry([alias], key, password, chain)
		End Sub

		''' <summary>
		''' Assigns the given key (that has already been protected) to the given
		''' alias.
		''' 
		''' <p>If the protected key is of type
		''' {@code java.security.PrivateKey}, it must be accompanied by a
		''' certificate chain certifying the corresponding public key. If the
		''' underlying keystore implementation is of type {@code jks},
		''' {@code key} must be encoded as an
		''' {@code EncryptedPrivateKeyInfo} as defined in the PKCS #8 standard.
		''' 
		''' <p>If the given alias already exists, the keystore information
		''' associated with it is overridden by the given key (and possibly
		''' certificate chain).
		''' </summary>
		''' <param name="alias"> the alias name </param>
		''' <param name="key"> the key (in protected format) to be associated with the alias </param>
		''' <param name="chain"> the certificate chain for the corresponding public
		'''          key (only useful if the protected key is of type
		'''          {@code java.security.PrivateKey}).
		''' </param>
		''' <exception cref="KeyStoreException"> if the keystore has not been initialized
		''' (loaded), or if this operation fails for some other reason. </exception>
		Public Sub setKeyEntry(  [alias] As String,   key As SByte(),   chain As java.security.cert.Certificate())
			If Not initialized Then Throw New KeyStoreException("Uninitialized keystore")
			keyStoreSpi.engineSetKeyEntry([alias], key, chain)
		End Sub

		''' <summary>
		''' Assigns the given trusted certificate to the given alias.
		''' 
		''' <p> If the given alias identifies an existing entry
		''' created by a call to {@code setCertificateEntry},
		''' or created by a call to {@code setEntry} with a
		''' {@code TrustedCertificateEntry},
		''' the trusted certificate in the existing entry
		''' is overridden by the given certificate.
		''' </summary>
		''' <param name="alias"> the alias name </param>
		''' <param name="cert"> the certificate
		''' </param>
		''' <exception cref="KeyStoreException"> if the keystore has not been initialized,
		''' or the given alias already exists and does not identify an
		''' entry containing a trusted certificate,
		''' or this operation fails for some other reason. </exception>
		Public Sub setCertificateEntry(  [alias] As String,   cert As java.security.cert.Certificate)
			If Not initialized Then Throw New KeyStoreException("Uninitialized keystore")
			keyStoreSpi.engineSetCertificateEntry([alias], cert)
		End Sub

		''' <summary>
		''' Deletes the entry identified by the given alias from this keystore.
		''' </summary>
		''' <param name="alias"> the alias name
		''' </param>
		''' <exception cref="KeyStoreException"> if the keystore has not been initialized,
		''' or if the entry cannot be removed. </exception>
		Public Sub deleteEntry(  [alias] As String)
			If Not initialized Then Throw New KeyStoreException("Uninitialized keystore")
			keyStoreSpi.engineDeleteEntry([alias])
		End Sub

		''' <summary>
		''' Lists all the alias names of this keystore.
		''' </summary>
		''' <returns> enumeration of the alias names
		''' </returns>
		''' <exception cref="KeyStoreException"> if the keystore has not been initialized
		''' (loaded). </exception>
		Public Function aliases() As Enumeration(Of String)
			If Not initialized Then Throw New KeyStoreException("Uninitialized keystore")
			Return keyStoreSpi.engineAliases()
		End Function

		''' <summary>
		''' Checks if the given alias exists in this keystore.
		''' </summary>
		''' <param name="alias"> the alias name
		''' </param>
		''' <returns> true if the alias exists, false otherwise
		''' </returns>
		''' <exception cref="KeyStoreException"> if the keystore has not been initialized
		''' (loaded). </exception>
		Public Function containsAlias(  [alias] As String) As Boolean
			If Not initialized Then Throw New KeyStoreException("Uninitialized keystore")
			Return keyStoreSpi.engineContainsAlias([alias])
		End Function

		''' <summary>
		''' Retrieves the number of entries in this keystore.
		''' </summary>
		''' <returns> the number of entries in this keystore
		''' </returns>
		''' <exception cref="KeyStoreException"> if the keystore has not been initialized
		''' (loaded). </exception>
		Public Function size() As Integer
			If Not initialized Then Throw New KeyStoreException("Uninitialized keystore")
			Return keyStoreSpi.engineSize()
		End Function

		''' <summary>
		''' Returns true if the entry identified by the given alias
		''' was created by a call to {@code setKeyEntry},
		''' or created by a call to {@code setEntry} with a
		''' {@code PrivateKeyEntry} or a {@code SecretKeyEntry}.
		''' </summary>
		''' <param name="alias"> the alias for the keystore entry to be checked
		''' </param>
		''' <returns> true if the entry identified by the given alias is a
		''' key-related entry, false otherwise.
		''' </returns>
		''' <exception cref="KeyStoreException"> if the keystore has not been initialized
		''' (loaded). </exception>
		Public Function isKeyEntry(  [alias] As String) As Boolean
			If Not initialized Then Throw New KeyStoreException("Uninitialized keystore")
			Return keyStoreSpi.engineIsKeyEntry([alias])
		End Function

		''' <summary>
		''' Returns true if the entry identified by the given alias
		''' was created by a call to {@code setCertificateEntry},
		''' or created by a call to {@code setEntry} with a
		''' {@code TrustedCertificateEntry}.
		''' </summary>
		''' <param name="alias"> the alias for the keystore entry to be checked
		''' </param>
		''' <returns> true if the entry identified by the given alias contains a
		''' trusted certificate, false otherwise.
		''' </returns>
		''' <exception cref="KeyStoreException"> if the keystore has not been initialized
		''' (loaded). </exception>
		Public Function isCertificateEntry(  [alias] As String) As Boolean
			If Not initialized Then Throw New KeyStoreException("Uninitialized keystore")
			Return keyStoreSpi.engineIsCertificateEntry([alias])
		End Function

		''' <summary>
		''' Returns the (alias) name of the first keystore entry whose certificate
		''' matches the given certificate.
		''' 
		''' <p> This method attempts to match the given certificate with each
		''' keystore entry. If the entry being considered was
		''' created by a call to {@code setCertificateEntry},
		''' or created by a call to {@code setEntry} with a
		''' {@code TrustedCertificateEntry},
		''' then the given certificate is compared to that entry's certificate.
		''' 
		''' <p> If the entry being considered was
		''' created by a call to {@code setKeyEntry},
		''' or created by a call to {@code setEntry} with a
		''' {@code PrivateKeyEntry},
		''' then the given certificate is compared to the first
		''' element of that entry's certificate chain.
		''' </summary>
		''' <param name="cert"> the certificate to match with.
		''' </param>
		''' <returns> the alias name of the first entry with a matching certificate,
		''' or null if no such entry exists in this keystore.
		''' </returns>
		''' <exception cref="KeyStoreException"> if the keystore has not been initialized
		''' (loaded). </exception>
		Public Function getCertificateAlias(  cert As java.security.cert.Certificate) As String
			If Not initialized Then Throw New KeyStoreException("Uninitialized keystore")
			Return keyStoreSpi.engineGetCertificateAlias(cert)
		End Function

		''' <summary>
		''' Stores this keystore to the given output stream, and protects its
		''' integrity with the given password.
		''' </summary>
		''' <param name="stream"> the output stream to which this keystore is written. </param>
		''' <param name="password"> the password to generate the keystore integrity check
		''' </param>
		''' <exception cref="KeyStoreException"> if the keystore has not been initialized
		''' (loaded). </exception>
		''' <exception cref="IOException"> if there was an I/O problem with data </exception>
		''' <exception cref="NoSuchAlgorithmException"> if the appropriate data integrity
		''' algorithm could not be found </exception>
		''' <exception cref="CertificateException"> if any of the certificates included in
		''' the keystore data could not be stored </exception>
		Public Sub store(  stream As OutputStream,   password As Char())
			If Not initialized Then Throw New KeyStoreException("Uninitialized keystore")
			keyStoreSpi.engineStore(stream, password)
		End Sub

		''' <summary>
		''' Stores this keystore using the given {@code LoadStoreParameter}.
		''' </summary>
		''' <param name="param"> the {@code LoadStoreParameter}
		'''          that specifies how to store the keystore,
		'''          which may be {@code null}
		''' </param>
		''' <exception cref="IllegalArgumentException"> if the given
		'''          {@code LoadStoreParameter}
		'''          input is not recognized </exception>
		''' <exception cref="KeyStoreException"> if the keystore has not been initialized
		'''          (loaded) </exception>
		''' <exception cref="IOException"> if there was an I/O problem with data </exception>
		''' <exception cref="NoSuchAlgorithmException"> if the appropriate data integrity
		'''          algorithm could not be found </exception>
		''' <exception cref="CertificateException"> if any of the certificates included in
		'''          the keystore data could not be stored
		''' 
		''' @since 1.5 </exception>
		Public Sub store(  param As LoadStoreParameter)
			If Not initialized Then Throw New KeyStoreException("Uninitialized keystore")
			keyStoreSpi.engineStore(param)
		End Sub

		''' <summary>
		''' Loads this KeyStore from the given input stream.
		''' 
		''' <p>A password may be given to unlock the keystore
		''' (e.g. the keystore resides on a hardware token device),
		''' or to check the integrity of the keystore data.
		''' If a password is not given for integrity checking,
		''' then integrity checking is not performed.
		''' 
		''' <p>In order to create an empty keystore, or if the keystore cannot
		''' be initialized from a stream, pass {@code null}
		''' as the {@code stream} argument.
		''' 
		''' <p> Note that if this keystore has already been loaded, it is
		''' reinitialized and loaded again from the given input stream.
		''' </summary>
		''' <param name="stream"> the input stream from which the keystore is loaded,
		''' or {@code null} </param>
		''' <param name="password"> the password used to check the integrity of
		''' the keystore, the password used to unlock the keystore,
		''' or {@code null}
		''' </param>
		''' <exception cref="IOException"> if there is an I/O or format problem with the
		''' keystore data, if a password is required but not given,
		''' or if the given password was incorrect. If the error is due to a
		''' wrong password, the <seealso cref="Throwable#getCause cause"/> of the
		''' {@code IOException} should be an
		''' {@code UnrecoverableKeyException} </exception>
		''' <exception cref="NoSuchAlgorithmException"> if the algorithm used to check
		''' the integrity of the keystore cannot be found </exception>
		''' <exception cref="CertificateException"> if any of the certificates in the
		''' keystore could not be loaded </exception>
		Public Sub load(  stream As InputStream,   password As Char())
			keyStoreSpi.engineLoad(stream, password)
			initialized = True
		End Sub

		''' <summary>
		''' Loads this keystore using the given {@code LoadStoreParameter}.
		''' 
		''' <p> Note that if this KeyStore has already been loaded, it is
		''' reinitialized and loaded again from the given parameter.
		''' </summary>
		''' <param name="param"> the {@code LoadStoreParameter}
		'''          that specifies how to load the keystore,
		'''          which may be {@code null}
		''' </param>
		''' <exception cref="IllegalArgumentException"> if the given
		'''          {@code LoadStoreParameter}
		'''          input is not recognized </exception>
		''' <exception cref="IOException"> if there is an I/O or format problem with the
		'''          keystore data. If the error is due to an incorrect
		'''         {@code ProtectionParameter} (e.g. wrong password)
		'''         the <seealso cref="Throwable#getCause cause"/> of the
		'''         {@code IOException} should be an
		'''         {@code UnrecoverableKeyException} </exception>
		''' <exception cref="NoSuchAlgorithmException"> if the algorithm used to check
		'''          the integrity of the keystore cannot be found </exception>
		''' <exception cref="CertificateException"> if any of the certificates in the
		'''          keystore could not be loaded
		''' 
		''' @since 1.5 </exception>
		Public Sub load(  param As LoadStoreParameter)

			keyStoreSpi.engineLoad(param)
			initialized = True
		End Sub

		''' <summary>
		''' Gets a keystore {@code Entry} for the specified alias
		''' with the specified protection parameter.
		''' </summary>
		''' <param name="alias"> get the keystore {@code Entry} for this alias </param>
		''' <param name="protParam"> the {@code ProtectionParameter}
		'''          used to protect the {@code Entry},
		'''          which may be {@code null}
		''' </param>
		''' <returns> the keystore {@code Entry} for the specified alias,
		'''          or {@code null} if there is no such entry
		''' </returns>
		''' <exception cref="NullPointerException"> if
		'''          {@code alias} is {@code null} </exception>
		''' <exception cref="NoSuchAlgorithmException"> if the algorithm for recovering the
		'''          entry cannot be found </exception>
		''' <exception cref="UnrecoverableEntryException"> if the specified
		'''          {@code protParam} were insufficient or invalid </exception>
		''' <exception cref="UnrecoverableKeyException"> if the entry is a
		'''          {@code PrivateKeyEntry} or {@code SecretKeyEntry}
		'''          and the specified {@code protParam} does not contain
		'''          the information needed to recover the key (e.g. wrong password) </exception>
		''' <exception cref="KeyStoreException"> if the keystore has not been initialized
		'''          (loaded). </exception>
		''' <seealso cref= #setEntry(String, KeyStore.Entry, KeyStore.ProtectionParameter)
		''' 
		''' @since 1.5 </seealso>
		Public Function getEntry(  [alias] As String,   protParam As ProtectionParameter) As Entry

			If [alias] Is Nothing Then Throw New NullPointerException("invalid null input")
			If Not initialized Then Throw New KeyStoreException("Uninitialized keystore")
			Return keyStoreSpi.engineGetEntry([alias], protParam)
		End Function

		''' <summary>
		''' Saves a keystore {@code Entry} under the specified alias.
		''' The protection parameter is used to protect the
		''' {@code Entry}.
		''' 
		''' <p> If an entry already exists for the specified alias,
		''' it is overridden.
		''' </summary>
		''' <param name="alias"> save the keystore {@code Entry} under this alias </param>
		''' <param name="entry"> the {@code Entry} to save </param>
		''' <param name="protParam"> the {@code ProtectionParameter}
		'''          used to protect the {@code Entry},
		'''          which may be {@code null}
		''' </param>
		''' <exception cref="NullPointerException"> if
		'''          {@code alias} or {@code entry}
		'''          is {@code null} </exception>
		''' <exception cref="KeyStoreException"> if the keystore has not been initialized
		'''          (loaded), or if this operation fails for some other reason
		''' </exception>
		''' <seealso cref= #getEntry(String, KeyStore.ProtectionParameter)
		''' 
		''' @since 1.5 </seealso>
		Public Sub setEntry(  [alias] As String,   entry As Entry,   protParam As ProtectionParameter)
			If [alias] Is Nothing OrElse entry Is Nothing Then Throw New NullPointerException("invalid null input")
			If Not initialized Then Throw New KeyStoreException("Uninitialized keystore")
			keyStoreSpi.engineSetEntry([alias], entry, protParam)
		End Sub

		''' <summary>
		''' Determines if the keystore {@code Entry} for the specified
		''' {@code alias} is an instance or subclass of the specified
		''' {@code entryClass}.
		''' </summary>
		''' <param name="alias"> the alias name </param>
		''' <param name="entryClass"> the entry class
		''' </param>
		''' <returns> true if the keystore {@code Entry} for the specified
		'''          {@code alias} is an instance or subclass of the
		'''          specified {@code entryClass}, false otherwise
		''' </returns>
		''' <exception cref="NullPointerException"> if
		'''          {@code alias} or {@code entryClass}
		'''          is {@code null} </exception>
		''' <exception cref="KeyStoreException"> if the keystore has not been
		'''          initialized (loaded)
		''' 
		''' @since 1.5 </exception>
		Public Function entryInstanceOf(  [alias] As String,   entryClass As [Class]) As Boolean

			If [alias] Is Nothing OrElse entryClass Is Nothing Then Throw New NullPointerException("invalid null input")
			If Not initialized Then Throw New KeyStoreException("Uninitialized keystore")
			Return keyStoreSpi.engineEntryInstanceOf([alias], entryClass)
		End Function

		''' <summary>
		''' A description of a to-be-instantiated KeyStore object.
		''' 
		''' <p>An instance of this class encapsulates the information needed to
		''' instantiate and initialize a KeyStore object. That process is
		''' triggered when the <seealso cref="#getKeyStore"/> method is called.
		''' 
		''' <p>This makes it possible to decouple configuration from KeyStore
		''' object creation and e.g. delay a password prompt until it is
		''' needed.
		''' </summary>
		''' <seealso cref= KeyStore </seealso>
		''' <seealso cref= javax.net.ssl.KeyStoreBuilderParameters
		''' @since 1.5 </seealso>
		Public MustInherit Class Builder

			' maximum times to try the callbackhandler if the password is wrong
			Friend Const MAX_CALLBACK_TRIES As Integer = 3

			''' <summary>
			''' Construct a new Builder.
			''' </summary>
			Protected Friend Sub New()
				' empty
			End Sub

			''' <summary>
			''' Returns the KeyStore described by this object.
			''' </summary>
			''' <returns> the {@code KeyStore} described by this object </returns>
			''' <exception cref="KeyStoreException"> if an error occurred during the
			'''   operation, for example if the KeyStore could not be
			'''   instantiated or loaded </exception>
			Public MustOverride ReadOnly Property keyStore As KeyStore

			''' <summary>
			''' Returns the ProtectionParameters that should be used to obtain
			''' the <seealso cref="KeyStore.Entry Entry"/> with the given alias.
			''' The {@code getKeyStore} method must be invoked before this
			''' method may be called.
			''' </summary>
			''' <returns> the ProtectionParameters that should be used to obtain
			'''   the <seealso cref="KeyStore.Entry Entry"/> with the given alias. </returns>
			''' <param name="alias"> the alias of the KeyStore entry </param>
			''' <exception cref="NullPointerException"> if alias is null </exception>
			''' <exception cref="KeyStoreException"> if an error occurred during the
			'''   operation </exception>
			''' <exception cref="IllegalStateException"> if the getKeyStore method has
			'''   not been invoked prior to calling this method </exception>
			Public MustOverride Function getProtectionParameter(  [alias] As String) As ProtectionParameter

			''' <summary>
			''' Returns a new Builder that encapsulates the given KeyStore.
			''' The <seealso cref="#getKeyStore"/> method of the returned object
			''' will return {@code keyStore}, the {@linkplain
			''' #getProtectionParameter getProtectionParameter()} method will
			''' return {@code protectionParameters}.
			''' 
			''' <p> This is useful if an existing KeyStore object needs to be
			''' used with Builder-based APIs.
			''' </summary>
			''' <returns> a new Builder object </returns>
			''' <param name="keyStore"> the KeyStore to be encapsulated </param>
			''' <param name="protectionParameter"> the ProtectionParameter used to
			'''   protect the KeyStore entries </param>
			''' <exception cref="NullPointerException"> if keyStore or
			'''   protectionParameters is null </exception>
			''' <exception cref="IllegalArgumentException"> if the keyStore has not been
			'''   initialized </exception>
			Public Shared Function newInstance(  keyStore_Renamed As KeyStore,   protectionParameter As ProtectionParameter) As Builder
				If (keyStore_Renamed Is Nothing) OrElse (protectionParameter Is Nothing) Then Throw New NullPointerException
				If keyStore_Renamed.initialized = False Then Throw New IllegalArgumentException("KeyStore not initialized")
				Return New BuilderAnonymousInnerClassHelper
			End Function

			Private Class BuilderAnonymousInnerClassHelper
				Inherits Builder

'JAVA TO VB CONVERTER TODO TASK: There is no VB equivalent to 'volatile':
				Private getCalled As Boolean

				Public  Overrides ReadOnly Property  keyStore As KeyStore
					Get
						getCalled = True
						Return keyStore
					End Get
				End Property

				Public Overrides Function getProtectionParameter(  [alias] As String) As ProtectionParameter
					If [alias] Is Nothing Then Throw New NullPointerException
					If getCalled = False Then Throw New IllegalStateException("getKeyStore() must be called first")
					Return protectionParameter
				End Function
			End Class

			''' <summary>
			''' Returns a new Builder object.
			''' 
			''' <p>The first call to the <seealso cref="#getKeyStore"/> method on the returned
			''' builder will create a KeyStore of type {@code type} and call
			''' its <seealso cref="KeyStore#load load()"/> method.
			''' The {@code inputStream} argument is constructed from
			''' {@code file}.
			''' If {@code protection} is a
			''' {@code PasswordProtection}, the password is obtained by
			''' calling the {@code getPassword} method.
			''' Otherwise, if {@code protection} is a
			''' {@code CallbackHandlerProtection}, the password is obtained
			''' by invoking the CallbackHandler.
			''' 
			''' <p>Subsequent calls to <seealso cref="#getKeyStore"/> return the same object
			''' as the initial call. If the initial call to failed with a
			''' KeyStoreException, subsequent calls also throw a
			''' KeyStoreException.
			''' 
			''' <p>The KeyStore is instantiated from {@code provider} if
			''' non-null. Otherwise, all installed providers are searched.
			''' 
			''' <p>Calls to <seealso cref="#getProtectionParameter getProtectionParameter()"/>
			''' will return a <seealso cref="KeyStore.PasswordProtection PasswordProtection"/>
			''' object encapsulating the password that was used to invoke the
			''' {@code load} method.
			''' 
			''' <p><em>Note</em> that the <seealso cref="#getKeyStore"/> method is executed
			''' within the <seealso cref="AccessControlContext"/> of the code invoking this
			''' method.
			''' </summary>
			''' <returns> a new Builder object </returns>
			''' <param name="type"> the type of KeyStore to be constructed </param>
			''' <param name="provider"> the provider from which the KeyStore is to
			'''   be instantiated (or null) </param>
			''' <param name="file"> the File that contains the KeyStore data </param>
			''' <param name="protection"> the ProtectionParameter securing the KeyStore data </param>
			''' <exception cref="NullPointerException"> if type, file or protection is null </exception>
			''' <exception cref="IllegalArgumentException"> if protection is not an instance
			'''   of either PasswordProtection or CallbackHandlerProtection; or
			'''   if file does not exist or does not refer to a normal file </exception>
			Public Shared Function newInstance(  type As String,   provider_Renamed As Provider,   file_Renamed As File,   protection As ProtectionParameter) As Builder
				If (type Is Nothing) OrElse (file_Renamed Is Nothing) OrElse (protection Is Nothing) Then Throw New NullPointerException
				If (TypeOf protection Is PasswordProtection = False) AndAlso (TypeOf protection Is CallbackHandlerProtection = False) Then Throw New IllegalArgumentException("Protection must be PasswordProtection or " & "CallbackHandlerProtection")
				If file_Renamed.file = False Then Throw New IllegalArgumentException("File does not exist or it does not refer " & "to a normal file: " & file_Renamed)
				Return New FileBuilder(type, provider_Renamed, file_Renamed, protection, AccessController.context)
			End Function

			Private NotInheritable Class FileBuilder
				Inherits Builder

				Private ReadOnly type As String
				Private ReadOnly provider As Provider
				Private ReadOnly file_Renamed As File
				Private protection As ProtectionParameter
				Private keyProtection As ProtectionParameter
				Private ReadOnly context As AccessControlContext

				Private keyStore_Renamed As KeyStore

				Private oldException As Throwable

				Friend Sub New(  type As String,   provider_Renamed As Provider,   file_Renamed As File,   protection As ProtectionParameter,   context As AccessControlContext)
					Me.type = type
					Me.provider = provider_Renamed
					Me.file_Renamed = file_Renamed
					Me.protection = protection
					Me.context = context
				End Sub

				<MethodImpl(MethodImplOptions.Synchronized)> _
				Public  Overrides ReadOnly Property  keyStore As KeyStore
					Get
						If keyStore_Renamed IsNot Nothing Then Return keyStore_Renamed
						If oldException IsNot Nothing Then Throw New KeyStoreException("Previous KeyStore instantiation failed", oldException)
						Dim action As PrivilegedExceptionAction(Of KeyStore) = New PrivilegedExceptionActionAnonymousInnerClassHelper(Of T)
						Try
							keyStore_Renamed = AccessController.doPrivileged(action, context)
							Return keyStore_Renamed
						Catch e As PrivilegedActionException
							oldException = e.InnerException
							Throw New KeyStoreException("KeyStore instantiation failed", oldException)
						End Try
					End Get
				End Property

				Private Class PrivilegedExceptionActionAnonymousInnerClassHelper(Of T)
					Implements PrivilegedExceptionAction(Of T)

					Public Overridable Function run() As KeyStore
						If TypeOf outerInstance.protection Is CallbackHandlerProtection = False Then Return run0()
						' when using a CallbackHandler,
						' reprompt if the password is wrong
						Dim tries As Integer = 0
						Do
							tries += 1
							Try
								Return run0()
							Catch e As IOException
								If (tries < MAX_CALLBACK_TRIES) AndAlso (TypeOf e.InnerException Is UnrecoverableKeyException) Then Continue Do
								Throw e
							End Try
						Loop
					End Function
					Public Overridable Function run0() As KeyStore
						Dim ks As KeyStore
						If outerInstance.provider Is Nothing Then
							ks = KeyStore.getInstance(outerInstance.type)
						Else
							ks = KeyStore.getInstance(outerInstance.type, outerInstance.provider)
						End If
						Dim [in] As InputStream = Nothing
						Dim password As Char() = Nothing
						Try
							[in] = New FileInputStream(outerInstance.file_Renamed)
							If TypeOf outerInstance.protection Is PasswordProtection Then
								password = CType(outerInstance.protection, PasswordProtection).password
								outerInstance.keyProtection = outerInstance.protection
							Else
								Dim handler As CallbackHandler = CType(outerInstance.protection, CallbackHandlerProtection).callbackHandler
								Dim callback As New PasswordCallback("Password for keystore " & outerInstance.file_Renamed.name, False)
								handler.handle(New Callback() {callback})
								password = callback.password
								If password Is Nothing Then Throw New KeyStoreException("No password" & " provided")
								callback.clearPassword()
								outerInstance.keyProtection = New PasswordProtection(password)
							End If
							ks.load([in], password)
							Return ks
						Finally
							If [in] IsNot Nothing Then [in].close()
						End Try
					End Function
				End Class

				<MethodImpl(MethodImplOptions.Synchronized)> _
				Public Overrides Function getProtectionParameter(  [alias] As String) As ProtectionParameter
					If [alias] Is Nothing Then Throw New NullPointerException
					If keyStore_Renamed Is Nothing Then Throw New IllegalStateException("getKeyStore() must be called first")
					Return keyProtection
				End Function
			End Class

			''' <summary>
			''' Returns a new Builder object.
			''' 
			''' <p>Each call to the <seealso cref="#getKeyStore"/> method on the returned
			''' builder will return a new KeyStore object of type {@code type}.
			''' Its <seealso cref="KeyStore#load(KeyStore.LoadStoreParameter) load()"/>
			''' method is invoked using a
			''' {@code LoadStoreParameter} that encapsulates
			''' {@code protection}.
			''' 
			''' <p>The KeyStore is instantiated from {@code provider} if
			''' non-null. Otherwise, all installed providers are searched.
			''' 
			''' <p>Calls to <seealso cref="#getProtectionParameter getProtectionParameter()"/>
			''' will return {@code protection}.
			''' 
			''' <p><em>Note</em> that the <seealso cref="#getKeyStore"/> method is executed
			''' within the <seealso cref="AccessControlContext"/> of the code invoking this
			''' method.
			''' </summary>
			''' <returns> a new Builder object </returns>
			''' <param name="type"> the type of KeyStore to be constructed </param>
			''' <param name="provider"> the provider from which the KeyStore is to
			'''   be instantiated (or null) </param>
			''' <param name="protection"> the ProtectionParameter securing the Keystore </param>
			''' <exception cref="NullPointerException"> if type or protection is null </exception>
			Public Shared Function newInstance(  type As String,   provider_Renamed As Provider,   protection As ProtectionParameter) As Builder
				If (type Is Nothing) OrElse (protection Is Nothing) Then Throw New NullPointerException
				Dim context As AccessControlContext = AccessController.context
				Return New BuilderAnonymousInnerClassHelper
			End Function

			Private Class BuilderAnonymousInnerClassHelper
				Inherits Builder

'JAVA TO VB CONVERTER TODO TASK: There is no VB equivalent to 'volatile':
				Private getCalled As Boolean
				Private oldException As IOException

				Private ReadOnly action As PrivilegedExceptionAction(Of KeyStore) = New PrivilegedExceptionActionAnonymousInnerClassHelper(Of T)

				Private Class PrivilegedExceptionActionAnonymousInnerClassHelper(Of T)
					Implements PrivilegedExceptionAction(Of T)

					Public Overridable Function run() As KeyStore
						Dim ks As KeyStore
						If provider Is Nothing Then
							ks = KeyStore.getInstance(type)
						Else
							ks = KeyStore.getInstance(type, provider)
						End If
						Dim param As LoadStoreParameter = New SimpleLoadStoreParameter(protection)
						If TypeOf protection Is CallbackHandlerProtection = False Then
							ks.load(param)
						Else
							' when using a CallbackHandler,
							' reprompt if the password is wrong
							Dim tries As Integer = 0
							Do
								tries += 1
								Try
									ks.load(param)
									Exit Do
								Catch e As IOException
									If TypeOf e.InnerException Is UnrecoverableKeyException Then
										If tries < MAX_CALLBACK_TRIES Then
											Continue Do
										Else
											oldException = e
										End If
									End If
									Throw e
								End Try
							Loop
						End If
						getCalled = True
						Return ks
					End Function
				End Class

				<MethodImpl(MethodImplOptions.Synchronized)> _
				Public  Overrides ReadOnly Property  keyStore As KeyStore
					Get
						If oldException IsNot Nothing Then Throw New KeyStoreException("Previous KeyStore instantiation failed", oldException)
						Try
							Return AccessController.doPrivileged(action, context)
						Catch e As PrivilegedActionException
							Dim cause As Throwable = e.InnerException
							Throw New KeyStoreException("KeyStore instantiation failed", cause)
						End Try
					End Get
				End Property

				Public Overrides Function getProtectionParameter(  [alias] As String) As ProtectionParameter
					If [alias] Is Nothing Then Throw New NullPointerException
					If getCalled = False Then Throw New IllegalStateException("getKeyStore() must be called first")
					Return protection
				End Function
			End Class

		End Class

		Friend Class SimpleLoadStoreParameter
			Implements LoadStoreParameter

			Private ReadOnly protection As ProtectionParameter

			Friend Sub New(  protection As ProtectionParameter)
				Me.protection = protection
			End Sub

			Public Overridable Property protectionParameter As ProtectionParameter
				Get
					Return protection
				End Get
			End Property
		End Class

	End Class

End Namespace