Imports System
Imports System.Collections.Generic
Imports sun.security.jca

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
	''' Key factories are used to convert <I>keys</I> (opaque
	''' cryptographic keys of type {@code Key}) into <I>key specifications</I>
	''' (transparent representations of the underlying key material), and vice
	''' versa.
	''' 
	''' <P> Key factories are bi-directional. That is, they allow you to build an
	''' opaque key object from a given key specification (key material), or to
	''' retrieve the underlying key material of a key object in a suitable format.
	''' 
	''' <P> Multiple compatible key specifications may exist for the same key.
	''' For example, a DSA public key may be specified using
	''' {@code DSAPublicKeySpec} or
	''' {@code X509EncodedKeySpec}. A key factory can be used to translate
	''' between compatible key specifications.
	''' 
	''' <P> The following is an example of how to use a key factory in order to
	''' instantiate a DSA public key from its encoding.
	''' Assume Alice has received a digital signature from Bob.
	''' Bob also sent her his public key (in encoded format) to verify
	''' his signature. Alice then performs the following actions:
	''' 
	''' <pre>
	''' X509EncodedKeySpec bobPubKeySpec = new X509EncodedKeySpec(bobEncodedPubKey);
	''' KeyFactory keyFactory = KeyFactory.getInstance("DSA");
	''' PublicKey bobPubKey = keyFactory.generatePublic(bobPubKeySpec);
	''' Signature sig = Signature.getInstance("DSA");
	''' sig.initVerify(bobPubKey);
	''' sig.update(data);
	''' sig.verify(signature);
	''' </pre>
	''' 
	''' <p> Every implementation of the Java platform is required to support the
	''' following standard {@code KeyFactory} algorithms:
	''' <ul>
	''' <li>{@code DiffieHellman}</li>
	''' <li>{@code DSA}</li>
	''' <li>{@code RSA}</li>
	''' </ul>
	''' These algorithms are described in the <a href=
	''' "{@docRoot}/../technotes/guides/security/StandardNames.html#KeyFactory">
	''' KeyFactory section</a> of the
	''' Java Cryptography Architecture Standard Algorithm Name Documentation.
	''' Consult the release documentation for your implementation to see if any
	''' other algorithms are supported.
	''' 
	''' @author Jan Luehe
	''' </summary>
	''' <seealso cref= Key </seealso>
	''' <seealso cref= PublicKey </seealso>
	''' <seealso cref= PrivateKey </seealso>
	''' <seealso cref= java.security.spec.KeySpec </seealso>
	''' <seealso cref= java.security.spec.DSAPublicKeySpec </seealso>
	''' <seealso cref= java.security.spec.X509EncodedKeySpec
	''' 
	''' @since 1.2 </seealso>

	Public Class KeyFactory

		Private Shared ReadOnly debug As sun.security.util.Debug = sun.security.util.Debug.getInstance("jca", "KeyFactory")

		' The algorithm associated with this key factory
		Private ReadOnly algorithm As String

		' The provider
		Private provider As Provider

		' The provider implementation (delegate)
'JAVA TO VB CONVERTER TODO TASK: There is no VB equivalent to 'volatile':
		Private spi As KeyFactorySpi

		' lock for mutex during provider selection
		Private ReadOnly lock As New Object

		' remaining services to try in provider selection
		' null once provider is selected
		Private serviceIterator As [Iterator](Of java.security.Provider.Service)

		''' <summary>
		''' Creates a KeyFactory object.
		''' </summary>
		''' <param name="keyFacSpi"> the delegate </param>
		''' <param name="provider"> the provider </param>
		''' <param name="algorithm"> the name of the algorithm
		''' to associate with this {@code KeyFactory} </param>
		Protected Friend Sub New(ByVal keyFacSpi As KeyFactorySpi, ByVal provider_Renamed As Provider, ByVal algorithm As String)
			Me.spi = keyFacSpi
			Me.provider = provider_Renamed
			Me.algorithm = algorithm
		End Sub

		Private Sub New(ByVal algorithm As String)
			Me.algorithm = algorithm
			Dim list As List(Of java.security.Provider.Service) = GetInstance.getServices("KeyFactory", algorithm)
			serviceIterator = list.GetEnumerator()
			' fetch and instantiate initial spi
			If nextSpi(Nothing) Is Nothing Then Throw New NoSuchAlgorithmException(algorithm & " KeyFactory not available")
		End Sub

		''' <summary>
		''' Returns a KeyFactory object that converts
		''' public/private keys of the specified algorithm.
		''' 
		''' <p> This method traverses the list of registered security Providers,
		''' starting with the most preferred Provider.
		''' A new KeyFactory object encapsulating the
		''' KeyFactorySpi implementation from the first
		''' Provider that supports the specified algorithm is returned.
		''' 
		''' <p> Note that the list of registered providers may be retrieved via
		''' the <seealso cref="Security#getProviders() Security.getProviders()"/> method.
		''' </summary>
		''' <param name="algorithm"> the name of the requested key algorithm.
		''' See the KeyFactory section in the <a href=
		''' "{@docRoot}/../technotes/guides/security/StandardNames.html#KeyFactory">
		''' Java Cryptography Architecture Standard Algorithm Name Documentation</a>
		''' for information about standard algorithm names.
		''' </param>
		''' <returns> the new KeyFactory object.
		''' </returns>
		''' <exception cref="NoSuchAlgorithmException"> if no Provider supports a
		'''          KeyFactorySpi implementation for the
		'''          specified algorithm.
		''' </exception>
		''' <seealso cref= Provider </seealso>
		Public Shared Function getInstance(ByVal algorithm As String) As KeyFactory
			Return New KeyFactory(algorithm)
		End Function

		''' <summary>
		''' Returns a KeyFactory object that converts
		''' public/private keys of the specified algorithm.
		''' 
		''' <p> A new KeyFactory object encapsulating the
		''' KeyFactorySpi implementation from the specified provider
		''' is returned.  The specified provider must be registered
		''' in the security provider list.
		''' 
		''' <p> Note that the list of registered providers may be retrieved via
		''' the <seealso cref="Security#getProviders() Security.getProviders()"/> method.
		''' </summary>
		''' <param name="algorithm"> the name of the requested key algorithm.
		''' See the KeyFactory section in the <a href=
		''' "{@docRoot}/../technotes/guides/security/StandardNames.html#KeyFactory">
		''' Java Cryptography Architecture Standard Algorithm Name Documentation</a>
		''' for information about standard algorithm names.
		''' </param>
		''' <param name="provider"> the name of the provider.
		''' </param>
		''' <returns> the new KeyFactory object.
		''' </returns>
		''' <exception cref="NoSuchAlgorithmException"> if a KeyFactorySpi
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
		Public Shared Function getInstance(ByVal algorithm As String, ByVal provider_Renamed As String) As KeyFactory
			Dim instance_Renamed As sun.security.jca.GetInstance.Instance = GetInstance.getInstance("KeyFactory", GetType(KeyFactorySpi), algorithm, provider_Renamed)
			Return New KeyFactory(CType(instance_Renamed.impl, KeyFactorySpi), instance_Renamed.provider, algorithm)
		End Function

		''' <summary>
		''' Returns a KeyFactory object that converts
		''' public/private keys of the specified algorithm.
		''' 
		''' <p> A new KeyFactory object encapsulating the
		''' KeyFactorySpi implementation from the specified Provider
		''' object is returned.  Note that the specified Provider object
		''' does not have to be registered in the provider list.
		''' </summary>
		''' <param name="algorithm"> the name of the requested key algorithm.
		''' See the KeyFactory section in the <a href=
		''' "{@docRoot}/../technotes/guides/security/StandardNames.html#KeyFactory">
		''' Java Cryptography Architecture Standard Algorithm Name Documentation</a>
		''' for information about standard algorithm names.
		''' </param>
		''' <param name="provider"> the provider.
		''' </param>
		''' <returns> the new KeyFactory object.
		''' </returns>
		''' <exception cref="NoSuchAlgorithmException"> if a KeyFactorySpi
		'''          implementation for the specified algorithm is not available
		'''          from the specified Provider object.
		''' </exception>
		''' <exception cref="IllegalArgumentException"> if the specified provider is null.
		''' </exception>
		''' <seealso cref= Provider
		''' 
		''' @since 1.4 </seealso>
		Public Shared Function getInstance(ByVal algorithm As String, ByVal provider_Renamed As Provider) As KeyFactory
			Dim instance_Renamed As sun.security.jca.GetInstance.Instance = GetInstance.getInstance("KeyFactory", GetType(KeyFactorySpi), algorithm, provider_Renamed)
			Return New KeyFactory(CType(instance_Renamed.impl, KeyFactorySpi), instance_Renamed.provider, algorithm)
		End Function

		''' <summary>
		''' Returns the provider of this key factory object.
		''' </summary>
		''' <returns> the provider of this key factory object </returns>
		Public Property provider As Provider
			Get
				SyncLock lock
					' disable further failover after this call
					serviceIterator = Nothing
					Return provider
				End SyncLock
			End Get
		End Property

		''' <summary>
		''' Gets the name of the algorithm
		''' associated with this {@code KeyFactory}.
		''' </summary>
		''' <returns> the name of the algorithm associated with this
		''' {@code KeyFactory} </returns>
		Public Property algorithm As String
			Get
				Return Me.algorithm
			End Get
		End Property

		''' <summary>
		''' Update the active KeyFactorySpi of this class and return the next
		''' implementation for failover. If no more implemenations are
		''' available, this method returns null. However, the active spi of
		''' this class is never set to null.
		''' </summary>
		Private Function nextSpi(ByVal oldSpi As KeyFactorySpi) As KeyFactorySpi
			SyncLock lock
				' somebody else did a failover concurrently
				' try that spi now
				If (oldSpi IsNot Nothing) AndAlso (oldSpi IsNot spi) Then Return spi
				If serviceIterator Is Nothing Then Return Nothing
				Do While serviceIterator.MoveNext()
					Dim s As java.security.Provider.Service = serviceIterator.Current
					Try
						Dim obj As Object = s.newInstance(Nothing)
						If TypeOf obj Is KeyFactorySpi = False Then Continue Do
						Dim spi As KeyFactorySpi = CType(obj, KeyFactorySpi)
						provider = s.provider
						Me.spi = spi
						Return spi
					Catch e As NoSuchAlgorithmException
						' ignore
					End Try
				Loop
				serviceIterator = Nothing
				Return Nothing
			End SyncLock
		End Function

		''' <summary>
		''' Generates a public key object from the provided key specification
		''' (key material).
		''' </summary>
		''' <param name="keySpec"> the specification (key material) of the public key.
		''' </param>
		''' <returns> the public key.
		''' </returns>
		''' <exception cref="InvalidKeySpecException"> if the given key specification
		''' is inappropriate for this key factory to produce a public key. </exception>
		Public Function generatePublic(ByVal keySpec As java.security.spec.KeySpec) As PublicKey
			If serviceIterator Is Nothing Then Return spi.engineGeneratePublic(keySpec)
			Dim failure As Exception = Nothing
			Dim mySpi As KeyFactorySpi = spi
			Do
				Try
					Return mySpi.engineGeneratePublic(keySpec)
				Catch e As Exception
					If failure Is Nothing Then failure = e
					mySpi = nextSpi(mySpi)
				End Try
			Loop While mySpi IsNot Nothing
			If TypeOf failure Is RuntimeException Then Throw CType(failure, RuntimeException)
			If TypeOf failure Is java.security.spec.InvalidKeySpecException Then Throw CType(failure, java.security.spec.InvalidKeySpecException)
			Throw New java.security.spec.InvalidKeySpecException("Could not generate public key", failure)
		End Function

		''' <summary>
		''' Generates a private key object from the provided key specification
		''' (key material).
		''' </summary>
		''' <param name="keySpec"> the specification (key material) of the private key.
		''' </param>
		''' <returns> the private key.
		''' </returns>
		''' <exception cref="InvalidKeySpecException"> if the given key specification
		''' is inappropriate for this key factory to produce a private key. </exception>
		Public Function generatePrivate(ByVal keySpec As java.security.spec.KeySpec) As PrivateKey
			If serviceIterator Is Nothing Then Return spi.engineGeneratePrivate(keySpec)
			Dim failure As Exception = Nothing
			Dim mySpi As KeyFactorySpi = spi
			Do
				Try
					Return mySpi.engineGeneratePrivate(keySpec)
				Catch e As Exception
					If failure Is Nothing Then failure = e
					mySpi = nextSpi(mySpi)
				End Try
			Loop While mySpi IsNot Nothing
			If TypeOf failure Is RuntimeException Then Throw CType(failure, RuntimeException)
			If TypeOf failure Is java.security.spec.InvalidKeySpecException Then Throw CType(failure, java.security.spec.InvalidKeySpecException)
			Throw New java.security.spec.InvalidKeySpecException("Could not generate private key", failure)
		End Function

		''' <summary>
		''' Returns a specification (key material) of the given key object.
		''' {@code keySpec} identifies the specification class in which
		''' the key material should be returned. It could, for example, be
		''' {@code DSAPublicKeySpec.class}, to indicate that the
		''' key material should be returned in an instance of the
		''' {@code DSAPublicKeySpec} class.
		''' </summary>
		''' @param <T> the type of the key specification to be returned
		''' </param>
		''' <param name="key"> the key.
		''' </param>
		''' <param name="keySpec"> the specification class in which
		''' the key material should be returned.
		''' </param>
		''' <returns> the underlying key specification (key material) in an instance
		''' of the requested specification class.
		''' </returns>
		''' <exception cref="InvalidKeySpecException"> if the requested key specification is
		''' inappropriate for the given key, or the given key cannot be processed
		''' (e.g., the given key has an unrecognized algorithm or format). </exception>
		Public Function getKeySpec(Of T As java.security.spec.KeySpec)(ByVal key As Key, ByVal keySpec As Class) As T
			If serviceIterator Is Nothing Then Return spi.engineGetKeySpec(key, keySpec)
			Dim failure As Exception = Nothing
			Dim mySpi As KeyFactorySpi = spi
			Do
				Try
					Return mySpi.engineGetKeySpec(key, keySpec)
				Catch e As Exception
					If failure Is Nothing Then failure = e
					mySpi = nextSpi(mySpi)
				End Try
			Loop While mySpi IsNot Nothing
			If TypeOf failure Is RuntimeException Then Throw CType(failure, RuntimeException)
			If TypeOf failure Is java.security.spec.InvalidKeySpecException Then Throw CType(failure, java.security.spec.InvalidKeySpecException)
			Throw New java.security.spec.InvalidKeySpecException("Could not get key spec", failure)
		End Function

		''' <summary>
		''' Translates a key object, whose provider may be unknown or potentially
		''' untrusted, into a corresponding key object of this key factory.
		''' </summary>
		''' <param name="key"> the key whose provider is unknown or untrusted.
		''' </param>
		''' <returns> the translated key.
		''' </returns>
		''' <exception cref="InvalidKeyException"> if the given key cannot be processed
		''' by this key factory. </exception>
		Public Function translateKey(ByVal key As Key) As Key
			If serviceIterator Is Nothing Then Return spi.engineTranslateKey(key)
			Dim failure As Exception = Nothing
			Dim mySpi As KeyFactorySpi = spi
			Do
				Try
					Return mySpi.engineTranslateKey(key)
				Catch e As Exception
					If failure Is Nothing Then failure = e
					mySpi = nextSpi(mySpi)
				End Try
			Loop While mySpi IsNot Nothing
			If TypeOf failure Is RuntimeException Then Throw CType(failure, RuntimeException)
			If TypeOf failure Is InvalidKeyException Then Throw CType(failure, InvalidKeyException)
			Throw New InvalidKeyException("Could not translate key", failure)
		End Function

	End Class

End Namespace