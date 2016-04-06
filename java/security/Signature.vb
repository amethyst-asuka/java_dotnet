Imports System
Imports System.Collections.Generic
Imports System.Collections.Concurrent
Imports sun.security.jca

'
' * Copyright (c) 1996, 2015, Oracle and/or its affiliates. All rights reserved.
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
	''' The Signature class is used to provide applications the functionality
	''' of a digital signature algorithm. Digital signatures are used for
	''' authentication and integrity assurance of digital data.
	''' 
	''' <p> The signature algorithm can be, among others, the NIST standard
	''' DSA, using DSA and SHA-1. The DSA algorithm using the
	''' SHA-1 message digest algorithm can be specified as {@code SHA1withDSA}.
	''' In the case of RSA, there are multiple choices for the message digest
	''' algorithm, so the signing algorithm could be specified as, for example,
	''' {@code MD2withRSA}, {@code MD5withRSA}, or {@code SHA1withRSA}.
	''' The algorithm name must be specified, as there is no default.
	''' 
	''' <p> A Signature object can be used to generate and verify digital
	''' signatures.
	''' 
	''' <p> There are three phases to the use of a Signature object for
	''' either signing data or verifying a signature:<ol>
	''' 
	''' <li>Initialization, with either
	''' 
	'''     <ul>
	''' 
	'''     <li>a public key, which initializes the signature for
	'''     verification (see <seealso cref="#initVerify(PublicKey) initVerify"/>), or
	''' 
	'''     <li>a private key (and optionally a Secure Random Number Generator),
	'''     which initializes the signature for signing
	'''     (see <seealso cref="#initSign(PrivateKey)"/>
	'''     and <seealso cref="#initSign(PrivateKey, SecureRandom)"/>).
	''' 
	'''     </ul>
	''' 
	''' <li>Updating
	''' 
	''' <p>Depending on the type of initialization, this will update the
	''' bytes to be signed or verified. See the
	''' <seealso cref="#update(byte) update"/> methods.
	''' 
	''' <li>Signing or Verifying a signature on all updated bytes. See the
	''' <seealso cref="#sign() sign"/> methods and the <seealso cref="#verify(byte[]) verify"/>
	''' method.
	''' 
	''' </ol>
	''' 
	''' <p>Note that this class is abstract and extends from
	''' {@code SignatureSpi} for historical reasons.
	''' Application developers should only take notice of the methods defined in
	''' this {@code Signature} class; all the methods in
	''' the superclass are intended for cryptographic service providers who wish to
	''' supply their own implementations of digital signature algorithms.
	''' 
	''' <p> Every implementation of the Java platform is required to support the
	''' following standard {@code Signature} algorithms:
	''' <ul>
	''' <li>{@code SHA1withDSA}</li>
	''' <li>{@code SHA1withRSA}</li>
	''' <li>{@code SHA256withRSA}</li>
	''' </ul>
	''' These algorithms are described in the <a href=
	''' "{@docRoot}/../technotes/guides/security/StandardNames.html#Signature">
	''' Signature section</a> of the
	''' Java Cryptography Architecture Standard Algorithm Name Documentation.
	''' Consult the release documentation for your implementation to see if any
	''' other algorithms are supported.
	''' 
	''' @author Benjamin Renaud
	''' 
	''' </summary>

	Public MustInherit Class Signature
		Inherits SignatureSpi

		Private Shared ReadOnly debug As sun.security.util.Debug = sun.security.util.Debug.getInstance("jca", "Signature")

		Private Shared ReadOnly pdebug As sun.security.util.Debug = sun.security.util.Debug.getInstance("provider", "Provider")
		Private Shared ReadOnly skipDebug As Boolean = sun.security.util.Debug.isOn("engine=") AndAlso Not sun.security.util.Debug.isOn("signature")

	'    
	'     * The algorithm for this signature object.
	'     * This value is used to map an OID to the particular algorithm.
	'     * The mapping is done in AlgorithmObject.algOID(String algorithm)
	'     
		Private algorithm As String

		' The provider
		Friend provider_Renamed As Provider

		''' <summary>
		''' Possible <seealso cref="#state"/> value, signifying that
		''' this signature object has not yet been initialized.
		''' </summary>
		Protected Friend Const UNINITIALIZED As Integer = 0

		''' <summary>
		''' Possible <seealso cref="#state"/> value, signifying that
		''' this signature object has been initialized for signing.
		''' </summary>
		Protected Friend Const SIGN_Renamed As Integer = 2

		''' <summary>
		''' Possible <seealso cref="#state"/> value, signifying that
		''' this signature object has been initialized for verification.
		''' </summary>
		Protected Friend Const VERIFY_Renamed As Integer = 3

		''' <summary>
		''' Current state of this signature object.
		''' </summary>
		Protected Friend state As Integer = UNINITIALIZED

		''' <summary>
		''' Creates a Signature object for the specified algorithm.
		''' </summary>
		''' <param name="algorithm"> the standard string name of the algorithm.
		''' See the Signature section in the <a href=
		''' "{@docRoot}/../technotes/guides/security/StandardNames.html#Signature">
		''' Java Cryptography Architecture Standard Algorithm Name Documentation</a>
		''' for information about standard algorithm names. </param>
		Protected Friend Sub New(  algorithm As String)
			Me.algorithm = algorithm
		End Sub

		' name of the special signature alg
		Private Const RSA_SIGNATURE As String = "NONEwithRSA"

		' name of the equivalent cipher alg
		Private Const RSA_CIPHER As String = "RSA/ECB/PKCS1Padding"

		' all the services we need to lookup for compatibility with Cipher
		Private Shared ReadOnly rsaIds As List(Of ServiceId) = Arrays.asList(New ServiceId() { New ServiceId("Signature", "NONEwithRSA"), New ServiceId("Cipher", "RSA/ECB/PKCS1Padding"), New ServiceId("Cipher", "RSA/ECB"), New ServiceId("Cipher", "RSA//PKCS1Padding"), New ServiceId("Cipher", "RSA") })

		''' <summary>
		''' Returns a Signature object that implements the specified signature
		''' algorithm.
		''' 
		''' <p> This method traverses the list of registered security Providers,
		''' starting with the most preferred Provider.
		''' A new Signature object encapsulating the
		''' SignatureSpi implementation from the first
		''' Provider that supports the specified algorithm is returned.
		''' 
		''' <p> Note that the list of registered providers may be retrieved via
		''' the <seealso cref="Security#getProviders() Security.getProviders()"/> method.
		''' </summary>
		''' <param name="algorithm"> the standard name of the algorithm requested.
		''' See the Signature section in the <a href=
		''' "{@docRoot}/../technotes/guides/security/StandardNames.html#Signature">
		''' Java Cryptography Architecture Standard Algorithm Name Documentation</a>
		''' for information about standard algorithm names.
		''' </param>
		''' <returns> the new Signature object.
		''' </returns>
		''' <exception cref="NoSuchAlgorithmException"> if no Provider supports a
		'''          Signature implementation for the
		'''          specified algorithm.
		''' </exception>
		''' <seealso cref= Provider </seealso>
		Public Shared Function getInstance(  algorithm As String) As Signature
			Dim list As List(Of java.security.Provider.Service)
			If algorithm.equalsIgnoreCase(RSA_SIGNATURE) Then
				list = GetInstance.getServices(rsaIds)
			Else
				list = GetInstance.getServices("Signature", algorithm)
			End If
			Dim t As [Iterator](Of java.security.Provider.Service) = list.GetEnumerator()
			If t.hasNext() = False Then Throw New NoSuchAlgorithmException(algorithm & " Signature not available")
			' try services until we find an Spi or a working Signature subclass
			Dim failure As NoSuchAlgorithmException
			Do
				Dim s As java.security.Provider.Service = t.next()
				If isSpi(s) Then
					Return New [Delegate](s, t, algorithm)
				Else
					' must be a subclass of Signature, disable dynamic selection
					Try
						Dim instance_Renamed As sun.security.jca.GetInstance.Instance = GetInstance.getInstance(s, GetType(SignatureSpi))
						Return getInstance(instance_Renamed, algorithm)
					Catch e As NoSuchAlgorithmException
						failure = e
					End Try
				End If
			Loop While t.hasNext()
			Throw failure
		End Function

		Private Shared Function getInstance(  instance As sun.security.jca.GetInstance.Instance,   algorithm As String) As Signature
			Dim sig As Signature
			If TypeOf instance.impl Is Signature Then
				sig = CType(instance.impl, Signature)
				sig.algorithm = algorithm
			Else
				Dim spi_Renamed As SignatureSpi = CType(instance.impl, SignatureSpi)
				sig = New [Delegate](spi_Renamed, algorithm)
			End If
			sig.provider_Renamed = instance.provider
			Return sig
		End Function

		Private Shared ReadOnly signatureInfo As Map(Of String, Boolean?)

		Shared Sub New()
			signatureInfo = New ConcurrentDictionary(Of String, Boolean?)
			Dim [TRUE] As Boolean? =  java.lang.[Boolean].TRUE
			' pre-initialize with values for our SignatureSpi implementations
			signatureInfo.put("sun.security.provider.DSA$RawDSA", [TRUE])
			signatureInfo.put("sun.security.provider.DSA$SHA1withDSA", [TRUE])
			signatureInfo.put("sun.security.rsa.RSASignature$MD2withRSA", [TRUE])
			signatureInfo.put("sun.security.rsa.RSASignature$MD5withRSA", [TRUE])
			signatureInfo.put("sun.security.rsa.RSASignature$SHA1withRSA", [TRUE])
			signatureInfo.put("sun.security.rsa.RSASignature$SHA256withRSA", [TRUE])
			signatureInfo.put("sun.security.rsa.RSASignature$SHA384withRSA", [TRUE])
			signatureInfo.put("sun.security.rsa.RSASignature$SHA512withRSA", [TRUE])
			signatureInfo.put("com.sun.net.ssl.internal.ssl.RSASignature", [TRUE])
			signatureInfo.put("sun.security.pkcs11.P11Signature", [TRUE])
		End Sub

		Private Shared Function isSpi(  s As java.security.Provider.Service) As Boolean
			If s.type.Equals("Cipher") Then Return True
			Dim className As String = s.className
			Dim result As Boolean? = signatureInfo.get(className)
			If result Is Nothing Then
				Try
					Dim instance_Renamed As Object = s.newInstance(Nothing)
					' Signature extends SignatureSpi
					' so it is a "real" Spi if it is an
					' instance of SignatureSpi but not Signature
					Dim r As Boolean = (TypeOf instance_Renamed Is SignatureSpi) AndAlso (TypeOf instance_Renamed Is Signature = False)
					If (debug IsNot Nothing) AndAlso (r = False) Then
						debug.println("Not a SignatureSpi " & className)
						debug.println("Delayed provider selection may not be " & "available for algorithm " & s.algorithm)
					End If
					result = Convert.ToBoolean(r)
					signatureInfo.put(className, result)
				Catch e As Exception
					' something is wrong, assume not an SPI
					Return False
				End Try
			End If
			Return result
		End Function

		''' <summary>
		''' Returns a Signature object that implements the specified signature
		''' algorithm.
		''' 
		''' <p> A new Signature object encapsulating the
		''' SignatureSpi implementation from the specified provider
		''' is returned.  The specified provider must be registered
		''' in the security provider list.
		''' 
		''' <p> Note that the list of registered providers may be retrieved via
		''' the <seealso cref="Security#getProviders() Security.getProviders()"/> method.
		''' </summary>
		''' <param name="algorithm"> the name of the algorithm requested.
		''' See the Signature section in the <a href=
		''' "{@docRoot}/../technotes/guides/security/StandardNames.html#Signature">
		''' Java Cryptography Architecture Standard Algorithm Name Documentation</a>
		''' for information about standard algorithm names.
		''' </param>
		''' <param name="provider"> the name of the provider.
		''' </param>
		''' <returns> the new Signature object.
		''' </returns>
		''' <exception cref="NoSuchAlgorithmException"> if a SignatureSpi
		'''          implementation for the specified algorithm is not
		'''          available from the specified provider.
		''' </exception>
		''' <exception cref="NoSuchProviderException"> if the specified provider is not
		'''          registered in the security provider list.
		''' </exception>
		''' <exception cref="IllegalArgumentException"> if the provider name is null
		'''          or empty.
		''' </exception>
		''' <seealso cref= Provider </seealso>
		Public Shared Function getInstance(  algorithm As String,   provider_Renamed As String) As Signature
			If algorithm.equalsIgnoreCase(RSA_SIGNATURE) Then
				' exception compatibility with existing code
				If (provider_Renamed Is Nothing) OrElse (provider_Renamed.length() = 0) Then Throw New IllegalArgumentException("missing provider")
				Dim p As Provider = Security.getProvider(provider_Renamed)
				If p Is Nothing Then Throw New NoSuchProviderException("no such provider: " & provider_Renamed)
				Return getInstanceRSA(p)
			End If
			Dim instance_Renamed As sun.security.jca.GetInstance.Instance = GetInstance.getInstance("Signature", GetType(SignatureSpi), algorithm, provider_Renamed)
			Return getInstance(instance_Renamed, algorithm)
		End Function

		''' <summary>
		''' Returns a Signature object that implements the specified
		''' signature algorithm.
		''' 
		''' <p> A new Signature object encapsulating the
		''' SignatureSpi implementation from the specified Provider
		''' object is returned.  Note that the specified Provider object
		''' does not have to be registered in the provider list.
		''' </summary>
		''' <param name="algorithm"> the name of the algorithm requested.
		''' See the Signature section in the <a href=
		''' "{@docRoot}/../technotes/guides/security/StandardNames.html#Signature">
		''' Java Cryptography Architecture Standard Algorithm Name Documentation</a>
		''' for information about standard algorithm names.
		''' </param>
		''' <param name="provider"> the provider.
		''' </param>
		''' <returns> the new Signature object.
		''' </returns>
		''' <exception cref="NoSuchAlgorithmException"> if a SignatureSpi
		'''          implementation for the specified algorithm is not available
		'''          from the specified Provider object.
		''' </exception>
		''' <exception cref="IllegalArgumentException"> if the provider is null.
		''' </exception>
		''' <seealso cref= Provider
		''' 
		''' @since 1.4 </seealso>
		Public Shared Function getInstance(  algorithm As String,   provider_Renamed As Provider) As Signature
			If algorithm.equalsIgnoreCase(RSA_SIGNATURE) Then
				' exception compatibility with existing code
				If provider_Renamed Is Nothing Then Throw New IllegalArgumentException("missing provider")
				Return getInstanceRSA(provider_Renamed)
			End If
			Dim instance_Renamed As sun.security.jca.GetInstance.Instance = GetInstance.getInstance("Signature", GetType(SignatureSpi), algorithm, provider_Renamed)
			Return getInstance(instance_Renamed, algorithm)
		End Function

		' return an implementation for NONEwithRSA, which is a special case
		' because of the Cipher.RSA/ECB/PKCS1Padding compatibility wrapper
		Private Shared Function getInstanceRSA(  p As Provider) As Signature
			' try Signature first
			Dim s As java.security.Provider.Service = p.getService("Signature", RSA_SIGNATURE)
			If s IsNot Nothing Then
				Dim instance_Renamed As sun.security.jca.GetInstance.Instance = GetInstance.getInstance(s, GetType(SignatureSpi))
				Return getInstance(instance_Renamed, RSA_SIGNATURE)
			End If
			' check Cipher
			Try
				Dim c As javax.crypto.Cipher = javax.crypto.Cipher.getInstance(RSA_CIPHER, p)
				Return New [Delegate](New CipherAdapter(c), RSA_SIGNATURE)
			Catch e As GeneralSecurityException
				' throw Signature style exception message to avoid confusion,
				' but append Cipher exception as cause
				Throw New NoSuchAlgorithmException("no such algorithm: " & RSA_SIGNATURE & " for provider " & p.name, e)
			End Try
		End Function

		''' <summary>
		''' Returns the provider of this signature object.
		''' </summary>
		''' <returns> the provider of this signature object </returns>
		Public Property provider As Provider
			Get
				chooseFirstProvider()
				Return Me.provider_Renamed
			End Get
		End Property

		Friend Overridable Sub chooseFirstProvider()
			' empty, overridden in Delegate
		End Sub

		''' <summary>
		''' Initializes this object for verification. If this method is called
		''' again with a different argument, it negates the effect
		''' of this call.
		''' </summary>
		''' <param name="publicKey"> the public key of the identity whose signature is
		''' going to be verified.
		''' </param>
		''' <exception cref="InvalidKeyException"> if the key is invalid. </exception>
		Public Sub initVerify(  publicKey As PublicKey)
			engineInitVerify(publicKey)
			state = VERIFY_Renamed

			If (Not skipDebug) AndAlso pdebug IsNot Nothing Then pdebug.println("Signature." & algorithm & " verification algorithm from: " & Me.provider_Renamed.name)
		End Sub

		''' <summary>
		''' Initializes this object for verification, using the public key from
		''' the given certificate.
		''' <p>If the certificate is of type X.509 and has a <i>key usage</i>
		''' extension field marked as critical, and the value of the <i>key usage</i>
		''' extension field implies that the public key in
		''' the certificate and its corresponding private key are not
		''' supposed to be used for digital signatures, an
		''' {@code InvalidKeyException} is thrown.
		''' </summary>
		''' <param name="certificate"> the certificate of the identity whose signature is
		''' going to be verified.
		''' </param>
		''' <exception cref="InvalidKeyException">  if the public key in the certificate
		''' is not encoded properly or does not include required  parameter
		''' information or cannot be used for digital signature purposes.
		''' @since 1.3 </exception>
		Public Sub initVerify(  certificate As java.security.cert.Certificate)
			' If the certificate is of type X509Certificate,
			' we should check whether it has a Key Usage
			' extension marked as critical.
			If TypeOf certificate Is java.security.cert.X509Certificate Then
				' Check whether the cert has a key usage extension
				' marked as a critical extension.
				' The OID for KeyUsage extension is 2.5.29.15.
				Dim cert As java.security.cert.X509Certificate = CType(certificate, java.security.cert.X509Certificate)
				Dim critSet As [Set](Of String) = cert.criticalExtensionOIDs

				If critSet IsNot Nothing AndAlso (Not critSet.empty) AndAlso critSet.contains("2.5.29.15") Then
					Dim keyUsageInfo As Boolean() = cert.keyUsage
					' keyUsageInfo[0] is for digitalSignature.
					If (keyUsageInfo IsNot Nothing) AndAlso (keyUsageInfo(0) = False) Then Throw New InvalidKeyException("Wrong key usage")
				End If
			End If

			Dim publicKey As PublicKey = certificate.publicKey
			engineInitVerify(publicKey)
			state = VERIFY_Renamed

			If (Not skipDebug) AndAlso pdebug IsNot Nothing Then pdebug.println("Signature." & algorithm & " verification algorithm from: " & Me.provider_Renamed.name)
		End Sub

		''' <summary>
		''' Initialize this object for signing. If this method is called
		''' again with a different argument, it negates the effect
		''' of this call.
		''' </summary>
		''' <param name="privateKey"> the private key of the identity whose signature
		''' is going to be generated.
		''' </param>
		''' <exception cref="InvalidKeyException"> if the key is invalid. </exception>
		Public Sub initSign(  privateKey As PrivateKey)
			engineInitSign(privateKey)
			state = SIGN_Renamed

			If (Not skipDebug) AndAlso pdebug IsNot Nothing Then pdebug.println("Signature." & algorithm & " signing algorithm from: " & Me.provider_Renamed.name)
		End Sub

		''' <summary>
		''' Initialize this object for signing. If this method is called
		''' again with a different argument, it negates the effect
		''' of this call.
		''' </summary>
		''' <param name="privateKey"> the private key of the identity whose signature
		''' is going to be generated.
		''' </param>
		''' <param name="random"> the source of randomness for this signature.
		''' </param>
		''' <exception cref="InvalidKeyException"> if the key is invalid. </exception>
		Public Sub initSign(  privateKey As PrivateKey,   random_Renamed As SecureRandom)
			engineInitSign(privateKey, random_Renamed)
			state = SIGN_Renamed

			If (Not skipDebug) AndAlso pdebug IsNot Nothing Then pdebug.println("Signature." & algorithm & " signing algorithm from: " & Me.provider_Renamed.name)
		End Sub

		''' <summary>
		''' Returns the signature bytes of all the data updated.
		''' The format of the signature depends on the underlying
		''' signature scheme.
		''' 
		''' <p>A call to this method resets this signature object to the state
		''' it was in when previously initialized for signing via a
		''' call to {@code initSign(PrivateKey)}. That is, the object is
		''' reset and available to generate another signature from the same
		''' signer, if desired, via new calls to {@code update} and
		''' {@code sign}.
		''' </summary>
		''' <returns> the signature bytes of the signing operation's result.
		''' </returns>
		''' <exception cref="SignatureException"> if this signature object is not
		''' initialized properly or if this signature algorithm is unable to
		''' process the input data provided. </exception>
		Public Function sign() As SByte()
			If state = SIGN_Renamed Then Return engineSign()
			Throw New SignatureException("object not initialized for " & "signing")
		End Function

		''' <summary>
		''' Finishes the signature operation and stores the resulting signature
		''' bytes in the provided buffer {@code outbuf}, starting at
		''' {@code offset}.
		''' The format of the signature depends on the underlying
		''' signature scheme.
		''' 
		''' <p>This signature object is reset to its initial state (the state it
		''' was in after a call to one of the {@code initSign} methods) and
		''' can be reused to generate further signatures with the same private key.
		''' </summary>
		''' <param name="outbuf"> buffer for the signature result.
		''' </param>
		''' <param name="offset"> offset into {@code outbuf} where the signature is
		''' stored.
		''' </param>
		''' <param name="len"> number of bytes within {@code outbuf} allotted for the
		''' signature.
		''' </param>
		''' <returns> the number of bytes placed into {@code outbuf}.
		''' </returns>
		''' <exception cref="SignatureException"> if this signature object is not
		''' initialized properly, if this signature algorithm is unable to
		''' process the input data provided, or if {@code len} is less
		''' than the actual signature length.
		''' 
		''' @since 1.2 </exception>
		Public Function sign(  outbuf As SByte(),   offset As Integer,   len As Integer) As Integer
			If outbuf Is Nothing Then Throw New IllegalArgumentException("No output buffer given")
			If offset < 0 OrElse len < 0 Then Throw New IllegalArgumentException("offset or len is less than 0")
			If outbuf.Length - offset < len Then Throw New IllegalArgumentException("Output buffer too small for specified offset and length")
			If state <> SIGN_Renamed Then Throw New SignatureException("object not initialized for " & "signing")
			Return engineSign(outbuf, offset, len)
		End Function

		''' <summary>
		''' Verifies the passed-in signature.
		''' 
		''' <p>A call to this method resets this signature object to the state
		''' it was in when previously initialized for verification via a
		''' call to {@code initVerify(PublicKey)}. That is, the object is
		''' reset and available to verify another signature from the identity
		''' whose public key was specified in the call to {@code initVerify}.
		''' </summary>
		''' <param name="signature"> the signature bytes to be verified.
		''' </param>
		''' <returns> true if the signature was verified, false if not.
		''' </returns>
		''' <exception cref="SignatureException"> if this signature object is not
		''' initialized properly, the passed-in signature is improperly
		''' encoded or of the wrong type, if this signature algorithm is unable to
		''' process the input data provided, etc. </exception>
		Public Function verify(  signature_Renamed As SByte()) As Boolean
			If state = VERIFY_Renamed Then Return engineVerify(signature_Renamed)
			Throw New SignatureException("object not initialized for " & "verification")
		End Function

		''' <summary>
		''' Verifies the passed-in signature in the specified array
		''' of bytes, starting at the specified offset.
		''' 
		''' <p>A call to this method resets this signature object to the state
		''' it was in when previously initialized for verification via a
		''' call to {@code initVerify(PublicKey)}. That is, the object is
		''' reset and available to verify another signature from the identity
		''' whose public key was specified in the call to {@code initVerify}.
		''' 
		''' </summary>
		''' <param name="signature"> the signature bytes to be verified. </param>
		''' <param name="offset"> the offset to start from in the array of bytes. </param>
		''' <param name="length"> the number of bytes to use, starting at offset.
		''' </param>
		''' <returns> true if the signature was verified, false if not.
		''' </returns>
		''' <exception cref="SignatureException"> if this signature object is not
		''' initialized properly, the passed-in signature is improperly
		''' encoded or of the wrong type, if this signature algorithm is unable to
		''' process the input data provided, etc. </exception>
		''' <exception cref="IllegalArgumentException"> if the {@code signature}
		''' byte array is null, or the {@code offset} or {@code length}
		''' is less than 0, or the sum of the {@code offset} and
		''' {@code length} is greater than the length of the
		''' {@code signature} byte array.
		''' @since 1.4 </exception>
		Public Function verify(  signature_Renamed As SByte(),   offset As Integer,   length As Integer) As Boolean
			If state = VERIFY_Renamed Then
				If signature_Renamed Is Nothing Then Throw New IllegalArgumentException("signature is null")
				If offset < 0 OrElse length < 0 Then Throw New IllegalArgumentException("offset or length is less than 0")
				If signature_Renamed.Length - offset < length Then Throw New IllegalArgumentException("signature too small for specified offset and length")

				Return engineVerify(signature_Renamed, offset, length)
			End If
			Throw New SignatureException("object not initialized for " & "verification")
		End Function

		''' <summary>
		''' Updates the data to be signed or verified by a java.lang.[Byte].
		''' </summary>
		''' <param name="b"> the byte to use for the update.
		''' </param>
		''' <exception cref="SignatureException"> if this signature object is not
		''' initialized properly. </exception>
		Public Sub update(  b As SByte)
			If state = VERIFY_Renamed OrElse state = SIGN_Renamed Then
				engineUpdate(b)
			Else
				Throw New SignatureException("object not initialized for " & "signature or verification")
			End If
		End Sub

		''' <summary>
		''' Updates the data to be signed or verified, using the specified
		''' array of bytes.
		''' </summary>
		''' <param name="data"> the byte array to use for the update.
		''' </param>
		''' <exception cref="SignatureException"> if this signature object is not
		''' initialized properly. </exception>
		Public Sub update(  data As SByte())
			update(data, 0, data.Length)
		End Sub

		''' <summary>
		''' Updates the data to be signed or verified, using the specified
		''' array of bytes, starting at the specified offset.
		''' </summary>
		''' <param name="data"> the array of bytes. </param>
		''' <param name="off"> the offset to start from in the array of bytes. </param>
		''' <param name="len"> the number of bytes to use, starting at offset.
		''' </param>
		''' <exception cref="SignatureException"> if this signature object is not
		''' initialized properly. </exception>
		Public Sub update(  data As SByte(),   [off] As Integer,   len As Integer)
			If state = SIGN_Renamed OrElse state = VERIFY_Renamed Then
				If data Is Nothing Then Throw New IllegalArgumentException("data is null")
				If [off] < 0 OrElse len < 0 Then Throw New IllegalArgumentException("off or len is less than 0")
				If data.Length - [off] < len Then Throw New IllegalArgumentException("data too small for specified offset and length")
				engineUpdate(data, [off], len)
			Else
				Throw New SignatureException("object not initialized for " & "signature or verification")
			End If
		End Sub

		''' <summary>
		''' Updates the data to be signed or verified using the specified
		''' ByteBuffer. Processes the {@code data.remaining()} bytes
		''' starting at at {@code data.position()}.
		''' Upon return, the buffer's position will be equal to its limit;
		''' its limit will not have changed.
		''' </summary>
		''' <param name="data"> the ByteBuffer
		''' </param>
		''' <exception cref="SignatureException"> if this signature object is not
		''' initialized properly.
		''' @since 1.5 </exception>
		Public Sub update(  data As java.nio.ByteBuffer)
			If (state <> SIGN_Renamed) AndAlso (state <> VERIFY_Renamed) Then Throw New SignatureException("object not initialized for " & "signature or verification")
			If data Is Nothing Then Throw New NullPointerException
			engineUpdate(data)
		End Sub

		''' <summary>
		''' Returns the name of the algorithm for this signature object.
		''' </summary>
		''' <returns> the name of the algorithm for this signature object. </returns>
		Public Property algorithm As String
			Get
				Return Me.algorithm
			End Get
		End Property

		''' <summary>
		''' Returns a string representation of this signature object,
		''' providing information that includes the state of the object
		''' and the name of the algorithm used.
		''' </summary>
		''' <returns> a string representation of this signature object. </returns>
		Public Overrides Function ToString() As String
			Dim initState As String = ""
			Select Case state
			Case UNINITIALIZED
				initState = "<not initialized>"
			Case VERIFY_Renamed
				initState = "<initialized for verifying>"
			Case SIGN_Renamed
				initState = "<initialized for signing>"
			End Select
			Return "Signature object: " & algorithm + initState
		End Function

		''' <summary>
		''' Sets the specified algorithm parameter to the specified value.
		''' This method supplies a general-purpose mechanism through
		''' which it is possible to set the various parameters of this object.
		''' A parameter may be any settable parameter for the algorithm, such as
		''' a parameter size, or a source of random bits for signature generation
		''' (if appropriate), or an indication of whether or not to perform
		''' a specific but optional computation. A uniform algorithm-specific
		''' naming scheme for each parameter is desirable but left unspecified
		''' at this time.
		''' </summary>
		''' <param name="param"> the string identifier of the parameter. </param>
		''' <param name="value"> the parameter value.
		''' </param>
		''' <exception cref="InvalidParameterException"> if {@code param} is an
		''' invalid parameter for this signature algorithm engine,
		''' the parameter is already set
		''' and cannot be set again, a security exception occurs, and so on.
		''' </exception>
		''' <seealso cref= #getParameter
		''' </seealso>
		''' @deprecated Use
		''' {@link #setParameter(java.security.spec.AlgorithmParameterSpec)
		''' setParameter}. 
		<Obsolete("Use")> _
		Public Sub setParameter(  param As String,   value As Object)
			engineSetParameter(param, value)
		End Sub

		''' <summary>
		''' Initializes this signature engine with the specified parameter set.
		''' </summary>
		''' <param name="params"> the parameters
		''' </param>
		''' <exception cref="InvalidAlgorithmParameterException"> if the given parameters
		''' are inappropriate for this signature engine
		''' </exception>
		''' <seealso cref= #getParameters </seealso>
		Public Property parameter As java.security.spec.AlgorithmParameterSpec
			Set(  params As java.security.spec.AlgorithmParameterSpec)
				engineSetParameter(params)
			End Set
		End Property

		''' <summary>
		''' Returns the parameters used with this signature object.
		''' 
		''' <p>The returned parameters may be the same that were used to initialize
		''' this signature, or may contain a combination of default and randomly
		''' generated parameter values used by the underlying signature
		''' implementation if this signature requires algorithm parameters but
		''' was not initialized with any.
		''' </summary>
		''' <returns> the parameters used with this signature, or null if this
		''' signature does not use any parameters.
		''' </returns>
		''' <seealso cref= #setParameter(AlgorithmParameterSpec)
		''' @since 1.4 </seealso>
		Public Property parameters As AlgorithmParameters
			Get
				Return engineGetParameters()
			End Get
		End Property

		''' <summary>
		''' Gets the value of the specified algorithm parameter. This method
		''' supplies a general-purpose mechanism through which it is possible to
		''' get the various parameters of this object. A parameter may be any
		''' settable parameter for the algorithm, such as a parameter size, or
		''' a source of random bits for signature generation (if appropriate),
		''' or an indication of whether or not to perform a specific but optional
		''' computation. A uniform algorithm-specific naming scheme for each
		''' parameter is desirable but left unspecified at this time.
		''' </summary>
		''' <param name="param"> the string name of the parameter.
		''' </param>
		''' <returns> the object that represents the parameter value, or null if
		''' there is none.
		''' </returns>
		''' <exception cref="InvalidParameterException"> if {@code param} is an invalid
		''' parameter for this engine, or another exception occurs while
		''' trying to get this parameter.
		''' </exception>
		''' <seealso cref= #setParameter(String, Object)
		''' 
		''' @deprecated </seealso>
		<Obsolete> _
		Public Function getParameter(  param As String) As Object
			Return engineGetParameter(param)
		End Function

		''' <summary>
		''' Returns a clone if the implementation is cloneable.
		''' </summary>
		''' <returns> a clone if the implementation is cloneable.
		''' </returns>
		''' <exception cref="CloneNotSupportedException"> if this is called
		''' on an implementation that does not support {@code Cloneable}. </exception>
		Public Overrides Function clone() As Object
			If TypeOf Me Is Cloneable Then
				Return MyBase.clone()
			Else
				Throw New CloneNotSupportedException
			End If
		End Function

	'    
	'     * The following class allows providers to extend from SignatureSpi
	'     * rather than from Signature. It represents a Signature with an
	'     * encapsulated, provider-supplied SPI object (of type SignatureSpi).
	'     * If the provider implementation is an instance of SignatureSpi, the
	'     * getInstance() methods above return an instance of this [Class], with
	'     * the SPI object encapsulated.
	'     *
	'     * Note: All SPI methods from the original Signature class have been
	'     * moved up the hierarchy into a new class (SignatureSpi), which has
	'     * been interposed in the hierarchy between the API (Signature)
	'     * and its original parent (Object).
	'     

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
		Private Class [Delegate]
			Inherits Signature

			' The provider implementation (delegate)
			' filled in once the provider is selected
			Private sigSpi As SignatureSpi

			' lock for mutex during provider selection
			Private ReadOnly lock As Object

			' next service to try in provider selection
			' null once provider is selected
			Private firstService As java.security.Provider.Service

			' remaining services to try in provider selection
			' null once provider is selected
			Private serviceIterator As [Iterator](Of java.security.Provider.Service)

			' constructor
			Friend Sub New(  sigSpi As SignatureSpi,   algorithm As String)
				MyBase.New(algorithm)
				Me.sigSpi = sigSpi
				Me.lock = Nothing ' no lock needed
			End Sub

			' used with delayed provider selection
			Friend Sub New(  service As java.security.Provider.Service,   [iterator] As [Iterator](Of java.security.Provider.Service),   algorithm As String)
				MyBase.New(algorithm)
				Me.firstService = service
				Me.serviceIterator = [iterator]
				Me.lock = New Object
			End Sub

			''' <summary>
			''' Returns a clone if the delegate is cloneable.
			''' </summary>
			''' <returns> a clone if the delegate is cloneable.
			''' </returns>
			''' <exception cref="CloneNotSupportedException"> if this is called on a
			''' delegate that does not support {@code Cloneable}. </exception>
			Public Overrides Function clone() As Object
				chooseFirstProvider()
				If TypeOf sigSpi Is Cloneable Then
					Dim sigSpiClone As SignatureSpi = CType(sigSpi.clone(), SignatureSpi)
					' Because 'algorithm' and 'provider' are private
					' members of our supertype, we must perform a cast to
					' access them.
					Dim that As Signature = New [Delegate](sigSpiClone, CType(Me, Signature).algorithm)
					that.provider_Renamed = CType(Me, Signature).provider_Renamed
					Return that
				Else
					Throw New CloneNotSupportedException
				End If
			End Function

			Private Shared Function newInstance(  s As java.security.Provider.Service) As SignatureSpi
				If s.type.Equals("Cipher") Then
					' must be NONEwithRSA
					Try
						Dim c As javax.crypto.Cipher = javax.crypto.Cipher.getInstance(RSA_CIPHER, s.provider)
						Return New CipherAdapter(c)
					Catch e As javax.crypto.NoSuchPaddingException
						Throw New NoSuchAlgorithmException(e)
					End Try
				Else
					Dim o As Object = s.newInstance(Nothing)
					If TypeOf o Is SignatureSpi = False Then Throw New NoSuchAlgorithmException("Not a SignatureSpi: " & o.GetType().name)
					Return CType(o, SignatureSpi)
				End If
			End Function

			' max number of debug warnings to print from chooseFirstProvider()
			Private Shared warnCount As Integer = 10

			''' <summary>
			''' Choose the Spi from the first provider available. Used if
			''' delayed provider selection is not possible because initSign()/
			''' initVerify() is not the first method called.
			''' </summary>
			Friend Overrides Sub chooseFirstProvider()
				If sigSpi IsNot Nothing Then Return
				SyncLock lock
					If sigSpi IsNot Nothing Then Return
					If debug IsNot Nothing Then
						warnCount -= 1
						Dim w As Integer = warnCount
						If w >= 0 Then
							debug.println("Signature.init() not first method " & "called, disabling delayed provider selection")
							If w = 0 Then debug.println("Further warnings of this type will " & "be suppressed")
							CType(New Exception("Call trace"), Exception).printStackTrace()
						End If
					End If
					Dim lastException As Exception = Nothing
					Do While (firstService IsNot Nothing) OrElse serviceIterator.MoveNext()
						Dim s As java.security.Provider.Service
						If firstService IsNot Nothing Then
							s = firstService
							firstService = Nothing
						Else
							s = serviceIterator.Current
						End If
						If isSpi(s) = False Then Continue Do
						Try
							sigSpi = newInstance(s)
							provider_Renamed = s.provider
							' not needed any more
							firstService = Nothing
							serviceIterator = Nothing
							Return
						Catch e As NoSuchAlgorithmException
							lastException = e
						End Try
					Loop
					Dim e As New ProviderException("Could not construct SignatureSpi instance")
					If lastException IsNot Nothing Then e.initCause(lastException)
					Throw e
				End SyncLock
			End Sub

			Private Sub chooseProvider(  type As Integer,   key As Key,   random_Renamed As SecureRandom)
				SyncLock lock
					If sigSpi IsNot Nothing Then
						init(sigSpi, type, key, random_Renamed)
						Return
					End If
					Dim lastException As Exception = Nothing
					Do While (firstService IsNot Nothing) OrElse serviceIterator.MoveNext()
						Dim s As java.security.Provider.Service
						If firstService IsNot Nothing Then
							s = firstService
							firstService = Nothing
						Else
							s = serviceIterator.Current
						End If
						' if provider says it does not support this key, ignore it
						If s.supportsParameter(key) = False Then Continue Do
						' if instance is not a SignatureSpi, ignore it
						If isSpi(s) = False Then Continue Do
						Try
							Dim spi As SignatureSpi = newInstance(s)
							init(spi, type, key, random_Renamed)
							provider_Renamed = s.provider
							sigSpi = spi
							firstService = Nothing
							serviceIterator = Nothing
							Return
						Catch e As Exception
							' NoSuchAlgorithmException from newInstance()
							' InvalidKeyException from init()
							' RuntimeException (ProviderException) from init()
							If lastException Is Nothing Then lastException = e
						End Try
					Loop
					' no working provider found, fail
					If TypeOf lastException Is InvalidKeyException Then Throw CType(lastException, InvalidKeyException)
					If TypeOf lastException Is RuntimeException Then Throw CType(lastException, RuntimeException)
					Dim k As String = If(key IsNot Nothing, key.GetType().name, "(null)")
					Throw New InvalidKeyException("No installed provider supports this key: " & k, lastException)
				End SyncLock
			End Sub

			Private Const I_PUB As Integer = 1
			Private Const I_PRIV As Integer = 2
			Private Const I_PRIV_SR As Integer = 3

			Private Sub init(  spi As SignatureSpi,   type As Integer,   key As Key,   random_Renamed As SecureRandom)
				Select Case type
				Case I_PUB
					spi.engineInitVerify(CType(key, PublicKey))
				Case I_PRIV
					spi.engineInitSign(CType(key, PrivateKey))
				Case I_PRIV_SR
					spi.engineInitSign(CType(key, PrivateKey), random_Renamed)
				Case Else
					Throw New AssertionError("Internal error: " & type)
				End Select
			End Sub

			Protected Friend Overrides Sub engineInitVerify(  publicKey As PublicKey)
				If sigSpi IsNot Nothing Then
					sigSpi.engineInitVerify(publicKey)
				Else
					chooseProvider(I_PUB, publicKey, Nothing)
				End If
			End Sub

			Protected Friend Overrides Sub engineInitSign(  privateKey As PrivateKey)
				If sigSpi IsNot Nothing Then
					sigSpi.engineInitSign(privateKey)
				Else
					chooseProvider(I_PRIV, privateKey, Nothing)
				End If
			End Sub

			Protected Friend Overrides Sub engineInitSign(  privateKey As PrivateKey,   sr As SecureRandom)
				If sigSpi IsNot Nothing Then
					sigSpi.engineInitSign(privateKey, sr)
				Else
					chooseProvider(I_PRIV_SR, privateKey, sr)
				End If
			End Sub

			Protected Friend Overrides Sub engineUpdate(  b As SByte)
				chooseFirstProvider()
				sigSpi.engineUpdate(b)
			End Sub

			Protected Friend Overrides Sub engineUpdate(  b As SByte(),   [off] As Integer,   len As Integer)
				chooseFirstProvider()
				sigSpi.engineUpdate(b, [off], len)
			End Sub

			Protected Friend Overrides Sub engineUpdate(  data As java.nio.ByteBuffer)
				chooseFirstProvider()
				sigSpi.engineUpdate(data)
			End Sub

			Protected Friend Overrides Function engineSign() As SByte()
				chooseFirstProvider()
				Return sigSpi.engineSign()
			End Function

			Protected Friend Overrides Function engineSign(  outbuf As SByte(),   offset As Integer,   len As Integer) As Integer
				chooseFirstProvider()
				Return sigSpi.engineSign(outbuf, offset, len)
			End Function

			Protected Friend Overrides Function engineVerify(  sigBytes As SByte()) As Boolean
				chooseFirstProvider()
				Return sigSpi.engineVerify(sigBytes)
			End Function

			Protected Friend Overrides Function engineVerify(  sigBytes As SByte(),   offset As Integer,   length As Integer) As Boolean
				chooseFirstProvider()
				Return sigSpi.engineVerify(sigBytes, offset, length)
			End Function

			Protected Friend Overrides Sub engineSetParameter(  param As String,   value As Object)
				chooseFirstProvider()
				sigSpi.engineSetParameter(param, value)
			End Sub

			Protected Friend Overrides Sub engineSetParameter(  params As java.security.spec.AlgorithmParameterSpec)
				chooseFirstProvider()
				sigSpi.engineSetParameter(params)
			End Sub

			Protected Friend Overrides Function engineGetParameter(  param As String) As Object
				chooseFirstProvider()
				Return sigSpi.engineGetParameter(param)
			End Function

			Protected Friend Overrides Function engineGetParameters() As AlgorithmParameters
				chooseFirstProvider()
				Return sigSpi.engineGetParameters()
			End Function
		End Class

		' adapter for RSA/ECB/PKCS1Padding ciphers
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
		Private Class CipherAdapter
			Inherits SignatureSpi

			Private ReadOnly cipher As javax.crypto.Cipher

			Private data As ByteArrayOutputStream

			Friend Sub New(  cipher As javax.crypto.Cipher)
				Me.cipher = cipher
			End Sub

			Protected Friend Overrides Sub engineInitVerify(  publicKey As PublicKey)
				cipher.init(javax.crypto.Cipher.DECRYPT_MODE, publicKey)
				If data Is Nothing Then
					data = New ByteArrayOutputStream(128)
				Else
					data.reset()
				End If
			End Sub

			Protected Friend Overrides Sub engineInitSign(  privateKey As PrivateKey)
				cipher.init(javax.crypto.Cipher.ENCRYPT_MODE, privateKey)
				data = Nothing
			End Sub

			Protected Friend Overrides Sub engineInitSign(  privateKey As PrivateKey,   random_Renamed As SecureRandom)
				cipher.init(javax.crypto.Cipher.ENCRYPT_MODE, privateKey, random_Renamed)
				data = Nothing
			End Sub

			Protected Friend Overrides Sub engineUpdate(  b As SByte)
				engineUpdate(New SByte() {b}, 0, 1)
			End Sub

			Protected Friend Overrides Sub engineUpdate(  b As SByte(),   [off] As Integer,   len As Integer)
				If data IsNot Nothing Then
					data.write(b, [off], len)
					Return
				End If
				Dim out As SByte() = cipher.update(b, [off], len)
				If (out IsNot Nothing) AndAlso (out.Length <> 0) Then Throw New SignatureException("Cipher unexpectedly returned data")
			End Sub

			Protected Friend Overrides Function engineSign() As SByte()
				Try
					Return cipher.doFinal()
				Catch e As javax.crypto.IllegalBlockSizeException
					Throw New SignatureException("doFinal() failed", e)
				Catch e As javax.crypto.BadPaddingException
					Throw New SignatureException("doFinal() failed", e)
				End Try
			End Function

			Protected Friend Overrides Function engineVerify(  sigBytes As SByte()) As Boolean
				Try
					Dim out As SByte() = cipher.doFinal(sigBytes)
					Dim dataBytes As SByte() = data.toByteArray()
					data.reset()
					Return MessageDigest.isEqual(out, dataBytes)
				Catch e As javax.crypto.BadPaddingException
					' e.g. wrong public key used
					' return false rather than throwing exception
					Return False
				Catch e As javax.crypto.IllegalBlockSizeException
					Throw New SignatureException("doFinal() failed", e)
				End Try
			End Function

			Protected Friend Overrides Sub engineSetParameter(  param As String,   value As Object)
				Throw New InvalidParameterException("Parameters not supported")
			End Sub

			Protected Friend Overrides Function engineGetParameter(  param As String) As Object
				Throw New InvalidParameterException("Parameters not supported")
			End Function

		End Class

	End Class

End Namespace