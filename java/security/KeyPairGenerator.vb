Imports System
Imports System.Collections.Generic
Imports sun.security.jca

'
' * Copyright (c) 1997, 2014, Oracle and/or its affiliates. All rights reserved.
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
	''' The KeyPairGenerator class is used to generate pairs of
	''' public and private keys. Key pair generators are constructed using the
	''' {@code getInstance} factory methods (static methods that
	''' return instances of a given [Class]).
	''' 
	''' <p>A Key pair generator for a particular algorithm creates a public/private
	''' key pair that can be used with this algorithm. It also associates
	''' algorithm-specific parameters with each of the generated keys.
	''' 
	''' <p>There are two ways to generate a key pair: in an algorithm-independent
	''' manner, and in an algorithm-specific manner.
	''' The only difference between the two is the initialization of the object:
	''' 
	''' <ul>
	''' <li><b>Algorithm-Independent Initialization</b>
	''' <p>All key pair generators share the concepts of a keysize and a
	''' source of randomness. The keysize is interpreted differently for different
	''' algorithms (e.g., in the case of the <i>DSA</i> algorithm, the keysize
	''' corresponds to the length of the modulus).
	''' There is an
	''' <seealso cref="#initialize(int, java.security.SecureRandom) initialize"/>
	''' method in this KeyPairGenerator class that takes these two universally
	''' shared types of arguments. There is also one that takes just a
	''' {@code keysize} argument, and uses the {@code SecureRandom}
	''' implementation of the highest-priority installed provider as the source
	''' of randomness. (If none of the installed providers supply an implementation
	''' of {@code SecureRandom}, a system-provided source of randomness is
	''' used.)
	''' 
	''' <p>Since no other parameters are specified when you call the above
	''' algorithm-independent {@code initialize} methods, it is up to the
	''' provider what to do about the algorithm-specific parameters (if any) to be
	''' associated with each of the keys.
	''' 
	''' <p>If the algorithm is the <i>DSA</i> algorithm, and the keysize (modulus
	''' size) is 512, 768, or 1024, then the <i>Sun</i> provider uses a set of
	''' precomputed values for the {@code p}, {@code q}, and
	''' {@code g} parameters. If the modulus size is not one of the above
	''' values, the <i>Sun</i> provider creates a new set of parameters. Other
	''' providers might have precomputed parameter sets for more than just the
	''' three modulus sizes mentioned above. Still others might not have a list of
	''' precomputed parameters at all and instead always create new parameter sets.
	''' 
	''' <li><b>Algorithm-Specific Initialization</b>
	''' <p>For situations where a set of algorithm-specific parameters already
	''' exists (e.g., so-called <i>community parameters</i> in DSA), there are two
	''' {@link #initialize(java.security.spec.AlgorithmParameterSpec)
	''' initialize} methods that have an {@code AlgorithmParameterSpec}
	''' argument. One also has a {@code SecureRandom} argument, while the
	''' the other uses the {@code SecureRandom}
	''' implementation of the highest-priority installed provider as the source
	''' of randomness. (If none of the installed providers supply an implementation
	''' of {@code SecureRandom}, a system-provided source of randomness is
	''' used.)
	''' </ul>
	''' 
	''' <p>In case the client does not explicitly initialize the KeyPairGenerator
	''' (via a call to an {@code initialize} method), each provider must
	''' supply (and document) a default initialization.
	''' For example, the <i>Sun</i> provider uses a default modulus size (keysize)
	''' of 1024 bits.
	''' 
	''' <p>Note that this class is abstract and extends from
	''' {@code KeyPairGeneratorSpi} for historical reasons.
	''' Application developers should only take notice of the methods defined in
	''' this {@code KeyPairGenerator} class; all the methods in
	''' the superclass are intended for cryptographic service providers who wish to
	''' supply their own implementations of key pair generators.
	''' 
	''' <p> Every implementation of the Java platform is required to support the
	''' following standard {@code KeyPairGenerator} algorithms and keysizes in
	''' parentheses:
	''' <ul>
	''' <li>{@code DiffieHellman} (1024)</li>
	''' <li>{@code DSA} (1024)</li>
	''' <li>{@code RSA} (1024, 2048)</li>
	''' </ul>
	''' These algorithms are described in the <a href=
	''' "{@docRoot}/../technotes/guides/security/StandardNames.html#KeyPairGenerator">
	''' KeyPairGenerator section</a> of the
	''' Java Cryptography Architecture Standard Algorithm Name Documentation.
	''' Consult the release documentation for your implementation to see if any
	''' other algorithms are supported.
	''' 
	''' @author Benjamin Renaud
	''' </summary>
	''' <seealso cref= java.security.spec.AlgorithmParameterSpec </seealso>

	Public MustInherit Class KeyPairGenerator
		Inherits KeyPairGeneratorSpi

		Private Shared ReadOnly pdebug As sun.security.util.Debug = sun.security.util.Debug.getInstance("provider", "Provider")
		Private Shared ReadOnly skipDebug As Boolean = sun.security.util.Debug.isOn("engine=") AndAlso Not sun.security.util.Debug.isOn("keypairgenerator")

		Private ReadOnly algorithm As String

		' The provider
		Friend provider As Provider

		''' <summary>
		''' Creates a KeyPairGenerator object for the specified algorithm.
		''' </summary>
		''' <param name="algorithm"> the standard string name of the algorithm.
		''' See the KeyPairGenerator section in the <a href=
		''' "{@docRoot}/../technotes/guides/security/StandardNames.html#KeyPairGenerator">
		''' Java Cryptography Architecture Standard Algorithm Name Documentation</a>
		''' for information about standard algorithm names. </param>
		Protected Friend Sub New(  algorithm As String)
			Me.algorithm = algorithm
		End Sub

		''' <summary>
		''' Returns the standard name of the algorithm for this key pair generator.
		''' See the KeyPairGenerator section in the <a href=
		''' "{@docRoot}/../technotes/guides/security/StandardNames.html#KeyPairGenerator">
		''' Java Cryptography Architecture Standard Algorithm Name Documentation</a>
		''' for information about standard algorithm names.
		''' </summary>
		''' <returns> the standard string name of the algorithm. </returns>
		Public Overridable Property algorithm As String
			Get
				Return Me.algorithm
			End Get
		End Property

		Private Shared Function getInstance(  instance As sun.security.jca.GetInstance.Instance,   algorithm As String) As KeyPairGenerator
			Dim kpg As KeyPairGenerator
			If TypeOf instance.impl Is KeyPairGenerator Then
				kpg = CType(instance.impl, KeyPairGenerator)
			Else
				Dim spi As KeyPairGeneratorSpi = CType(instance.impl, KeyPairGeneratorSpi)
				kpg = New [Delegate](spi, algorithm)
			End If
			kpg.provider = instance.provider

			If (Not skipDebug) AndAlso pdebug IsNot Nothing Then pdebug.println("KeyPairGenerator." & algorithm & " algorithm from: " & kpg.provider.name)

			Return kpg
		End Function

		''' <summary>
		''' Returns a KeyPairGenerator object that generates public/private
		''' key pairs for the specified algorithm.
		''' 
		''' <p> This method traverses the list of registered security Providers,
		''' starting with the most preferred Provider.
		''' A new KeyPairGenerator object encapsulating the
		''' KeyPairGeneratorSpi implementation from the first
		''' Provider that supports the specified algorithm is returned.
		''' 
		''' <p> Note that the list of registered providers may be retrieved via
		''' the <seealso cref="Security#getProviders() Security.getProviders()"/> method.
		''' </summary>
		''' <param name="algorithm"> the standard string name of the algorithm.
		''' See the KeyPairGenerator section in the <a href=
		''' "{@docRoot}/../technotes/guides/security/StandardNames.html#KeyPairGenerator">
		''' Java Cryptography Architecture Standard Algorithm Name Documentation</a>
		''' for information about standard algorithm names.
		''' </param>
		''' <returns> the new KeyPairGenerator object.
		''' </returns>
		''' <exception cref="NoSuchAlgorithmException"> if no Provider supports a
		'''          KeyPairGeneratorSpi implementation for the
		'''          specified algorithm.
		''' </exception>
		''' <seealso cref= Provider </seealso>
		Public Shared Function getInstance(  algorithm As String) As KeyPairGenerator
			Dim list As List(Of java.security.Provider.Service) = GetInstance.getServices("KeyPairGenerator", algorithm)
			Dim t As [Iterator](Of java.security.Provider.Service) = list.GetEnumerator()
			If t.hasNext() = False Then Throw New NoSuchAlgorithmException(algorithm & " KeyPairGenerator not available")
			' find a working Spi or KeyPairGenerator subclass
			Dim failure As NoSuchAlgorithmException = Nothing
			Do
				Dim s As java.security.Provider.Service = t.next()
				Try
					Dim instance_Renamed As sun.security.jca.GetInstance.Instance = GetInstance.getInstance(s, GetType(KeyPairGeneratorSpi))
					If TypeOf instance_Renamed.impl Is KeyPairGenerator Then
						Return getInstance(instance_Renamed, algorithm)
					Else
						Return New [Delegate](instance_Renamed, t, algorithm)
					End If
				Catch e As NoSuchAlgorithmException
					If failure Is Nothing Then failure = e
				End Try
			Loop While t.hasNext()
			Throw failure
		End Function

		''' <summary>
		''' Returns a KeyPairGenerator object that generates public/private
		''' key pairs for the specified algorithm.
		''' 
		''' <p> A new KeyPairGenerator object encapsulating the
		''' KeyPairGeneratorSpi implementation from the specified provider
		''' is returned.  The specified provider must be registered
		''' in the security provider list.
		''' 
		''' <p> Note that the list of registered providers may be retrieved via
		''' the <seealso cref="Security#getProviders() Security.getProviders()"/> method.
		''' </summary>
		''' <param name="algorithm"> the standard string name of the algorithm.
		''' See the KeyPairGenerator section in the <a href=
		''' "{@docRoot}/../technotes/guides/security/StandardNames.html#KeyPairGenerator">
		''' Java Cryptography Architecture Standard Algorithm Name Documentation</a>
		''' for information about standard algorithm names.
		''' </param>
		''' <param name="provider"> the string name of the provider.
		''' </param>
		''' <returns> the new KeyPairGenerator object.
		''' </returns>
		''' <exception cref="NoSuchAlgorithmException"> if a KeyPairGeneratorSpi
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
		Public Shared Function getInstance(  algorithm As String,   provider_Renamed As String) As KeyPairGenerator
			Dim instance_Renamed As sun.security.jca.GetInstance.Instance = GetInstance.getInstance("KeyPairGenerator", GetType(KeyPairGeneratorSpi), algorithm, provider_Renamed)
			Return getInstance(instance_Renamed, algorithm)
		End Function

		''' <summary>
		''' Returns a KeyPairGenerator object that generates public/private
		''' key pairs for the specified algorithm.
		''' 
		''' <p> A new KeyPairGenerator object encapsulating the
		''' KeyPairGeneratorSpi implementation from the specified Provider
		''' object is returned.  Note that the specified Provider object
		''' does not have to be registered in the provider list.
		''' </summary>
		''' <param name="algorithm"> the standard string name of the algorithm.
		''' See the KeyPairGenerator section in the <a href=
		''' "{@docRoot}/../technotes/guides/security/StandardNames.html#KeyPairGenerator">
		''' Java Cryptography Architecture Standard Algorithm Name Documentation</a>
		''' for information about standard algorithm names.
		''' </param>
		''' <param name="provider"> the provider.
		''' </param>
		''' <returns> the new KeyPairGenerator object.
		''' </returns>
		''' <exception cref="NoSuchAlgorithmException"> if a KeyPairGeneratorSpi
		'''          implementation for the specified algorithm is not available
		'''          from the specified Provider object.
		''' </exception>
		''' <exception cref="IllegalArgumentException"> if the specified provider is null.
		''' </exception>
		''' <seealso cref= Provider
		''' 
		''' @since 1.4 </seealso>
		Public Shared Function getInstance(  algorithm As String,   provider_Renamed As Provider) As KeyPairGenerator
			Dim instance_Renamed As sun.security.jca.GetInstance.Instance = GetInstance.getInstance("KeyPairGenerator", GetType(KeyPairGeneratorSpi), algorithm, provider_Renamed)
			Return getInstance(instance_Renamed, algorithm)
		End Function

		''' <summary>
		''' Returns the provider of this key pair generator object.
		''' </summary>
		''' <returns> the provider of this key pair generator object </returns>
		Public Property provider As Provider
			Get
				disableFailover()
				Return Me.provider
			End Get
		End Property

		Friend Overridable Sub disableFailover()
			' empty, overridden in Delegate
		End Sub

		''' <summary>
		''' Initializes the key pair generator for a certain keysize using
		''' a default parameter set and the {@code SecureRandom}
		''' implementation of the highest-priority installed provider as the source
		''' of randomness.
		''' (If none of the installed providers supply an implementation of
		''' {@code SecureRandom}, a system-provided source of randomness is
		''' used.)
		''' </summary>
		''' <param name="keysize"> the keysize. This is an
		''' algorithm-specific metric, such as modulus length, specified in
		''' number of bits.
		''' </param>
		''' <exception cref="InvalidParameterException"> if the {@code keysize} is not
		''' supported by this KeyPairGenerator object. </exception>
		Public Overridable Sub initialize(  keysize As Integer)
			initialize(keysize, JCAUtil.secureRandom)
		End Sub

		''' <summary>
		''' Initializes the key pair generator for a certain keysize with
		''' the given source of randomness (and a default parameter set).
		''' </summary>
		''' <param name="keysize"> the keysize. This is an
		''' algorithm-specific metric, such as modulus length, specified in
		''' number of bits. </param>
		''' <param name="random"> the source of randomness.
		''' </param>
		''' <exception cref="InvalidParameterException"> if the {@code keysize} is not
		''' supported by this KeyPairGenerator object.
		''' 
		''' @since 1.2 </exception>
		Public Overrides Sub initialize(  keysize As Integer,   random_Renamed As SecureRandom)
			' This does nothing, because either
			' 1. the implementation object returned by getInstance() is an
			'    instance of KeyPairGenerator which has its own
			'    initialize(keysize, random) method, so the application would
			'    be calling that method directly, or
			' 2. the implementation returned by getInstance() is an instance
			'    of Delegate, in which case initialize(keysize, random) is
			'    overridden to call the corresponding SPI method.
			' (This is a special case, because the API and SPI method have the
			' same name.)
		End Sub

		''' <summary>
		''' Initializes the key pair generator using the specified parameter
		''' set and the {@code SecureRandom}
		''' implementation of the highest-priority installed provider as the source
		''' of randomness.
		''' (If none of the installed providers supply an implementation of
		''' {@code SecureRandom}, a system-provided source of randomness is
		''' used.).
		''' 
		''' <p>This concrete method has been added to this previously-defined
		''' abstract class.
		''' This method calls the KeyPairGeneratorSpi
		''' {@link KeyPairGeneratorSpi#initialize(
		''' java.security.spec.AlgorithmParameterSpec,
		''' java.security.SecureRandom) initialize} method,
		''' passing it {@code params} and a source of randomness (obtained
		''' from the highest-priority installed provider or system-provided if none
		''' of the installed providers supply one).
		''' That {@code initialize} method always throws an
		''' UnsupportedOperationException if it is not overridden by the provider.
		''' </summary>
		''' <param name="params"> the parameter set used to generate the keys.
		''' </param>
		''' <exception cref="InvalidAlgorithmParameterException"> if the given parameters
		''' are inappropriate for this key pair generator.
		''' 
		''' @since 1.2 </exception>
		Public Overridable Sub initialize(  params As java.security.spec.AlgorithmParameterSpec)
			initialize(params, JCAUtil.secureRandom)
		End Sub

		''' <summary>
		''' Initializes the key pair generator with the given parameter
		''' set and source of randomness.
		''' 
		''' <p>This concrete method has been added to this previously-defined
		''' abstract class.
		''' This method calls the KeyPairGeneratorSpi {@link
		''' KeyPairGeneratorSpi#initialize(
		''' java.security.spec.AlgorithmParameterSpec,
		''' java.security.SecureRandom) initialize} method,
		''' passing it {@code params} and {@code random}.
		''' That {@code initialize}
		''' method always throws an
		''' UnsupportedOperationException if it is not overridden by the provider.
		''' </summary>
		''' <param name="params"> the parameter set used to generate the keys. </param>
		''' <param name="random"> the source of randomness.
		''' </param>
		''' <exception cref="InvalidAlgorithmParameterException"> if the given parameters
		''' are inappropriate for this key pair generator.
		''' 
		''' @since 1.2 </exception>
		Public Overrides Sub initialize(  params As java.security.spec.AlgorithmParameterSpec,   random_Renamed As SecureRandom)
			' This does nothing, because either
			' 1. the implementation object returned by getInstance() is an
			'    instance of KeyPairGenerator which has its own
			'    initialize(params, random) method, so the application would
			'    be calling that method directly, or
			' 2. the implementation returned by getInstance() is an instance
			'    of Delegate, in which case initialize(params, random) is
			'    overridden to call the corresponding SPI method.
			' (This is a special case, because the API and SPI method have the
			' same name.)
		End Sub

		''' <summary>
		''' Generates a key pair.
		''' 
		''' <p>If this KeyPairGenerator has not been initialized explicitly,
		''' provider-specific defaults will be used for the size and other
		''' (algorithm-specific) values of the generated keys.
		''' 
		''' <p>This will generate a new key pair every time it is called.
		''' 
		''' <p>This method is functionally equivalent to
		''' <seealso cref="#generateKeyPair() generateKeyPair"/>.
		''' </summary>
		''' <returns> the generated key pair
		''' 
		''' @since 1.2 </returns>
		Public Function genKeyPair() As KeyPair
			Return generateKeyPair()
		End Function

		''' <summary>
		''' Generates a key pair.
		''' 
		''' <p>If this KeyPairGenerator has not been initialized explicitly,
		''' provider-specific defaults will be used for the size and other
		''' (algorithm-specific) values of the generated keys.
		''' 
		''' <p>This will generate a new key pair every time it is called.
		''' 
		''' <p>This method is functionally equivalent to
		''' <seealso cref="#genKeyPair() genKeyPair"/>.
		''' </summary>
		''' <returns> the generated key pair </returns>
		Public Overrides Function generateKeyPair() As KeyPair
			' This does nothing (except returning null), because either:
			'
			' 1. the implementation object returned by getInstance() is an
			'    instance of KeyPairGenerator which has its own implementation
			'    of generateKeyPair (overriding this one), so the application
			'    would be calling that method directly, or
			'
			' 2. the implementation returned by getInstance() is an instance
			'    of Delegate, in which case generateKeyPair is
			'    overridden to invoke the corresponding SPI method.
			'
			' (This is a special case, because in JDK 1.1.x the generateKeyPair
			' method was used both as an API and a SPI method.)
			Return Nothing
		End Function


	'    
	'     * The following class allows providers to extend from KeyPairGeneratorSpi
	'     * rather than from KeyPairGenerator. It represents a KeyPairGenerator
	'     * with an encapsulated, provider-supplied SPI object (of type
	'     * KeyPairGeneratorSpi).
	'     * If the provider implementation is an instance of KeyPairGeneratorSpi,
	'     * the getInstance() methods above return an instance of this [Class], with
	'     * the SPI object encapsulated.
	'     *
	'     * Note: All SPI methods from the original KeyPairGenerator class have been
	'     * moved up the hierarchy into a new class (KeyPairGeneratorSpi), which has
	'     * been interposed in the hierarchy between the API (KeyPairGenerator)
	'     * and its original parent (Object).
	'     

		'
		' error failover notes:
		'
		'  . we failover if the implementation throws an error during init
		'    by retrying the init on other providers
		'
		'  . we also failover if the init succeeded but the subsequent call
		'    to generateKeyPair() fails. In order for this to work, we need
		'    to remember the parameters to the last successful call to init
		'    and initialize() the next spi using them.
		'
		'  . although not specified, KeyPairGenerators could be thread safe,
		'    so we make sure we do not interfere with that
		'
		'  . failover is not available, if:
		'    . getInstance(algorithm, provider) was used
		'    . a provider extends KeyPairGenerator rather than
		'      KeyPairGeneratorSpi (JDK 1.1 style)
		'    . once getProvider() is called
		'

		Private NotInheritable Class [Delegate]
			Inherits KeyPairGenerator

			' The provider implementation (delegate)
'JAVA TO VB CONVERTER TODO TASK: There is no VB equivalent to 'volatile':
			Private spi As KeyPairGeneratorSpi

			Private ReadOnly lock As New Object

			Private serviceIterator As [Iterator](Of java.security.Provider.Service)

			Private Const I_NONE As Integer = 1
			Private Const I_SIZE As Integer = 2
			Private Const I_PARAMS As Integer = 3

			Private initType As Integer
			Private initKeySize As Integer
			Private initParams As java.security.spec.AlgorithmParameterSpec
			Private initRandom As SecureRandom

			' constructor
			Friend Sub New(  spi As KeyPairGeneratorSpi,   algorithm As String)
				MyBase.New(algorithm)
				Me.spi = spi
			End Sub

			Friend Sub New(  instance As sun.security.jca.GetInstance.Instance,   serviceIterator As [Iterator](Of java.security.Provider.Service),   algorithm As String)
				MyBase.New(algorithm)
				spi = CType(instance.impl, KeyPairGeneratorSpi)
				provider = instance.provider
				Me.serviceIterator = serviceIterator
				initType = I_NONE

				If (Not skipDebug) AndAlso pdebug IsNot Nothing Then pdebug.println("KeyPairGenerator." & algorithm & " algorithm from: " & provider.name)
			End Sub

			''' <summary>
			''' Update the active spi of this class and return the next
			''' implementation for failover. If no more implemenations are
			''' available, this method returns null. However, the active spi of
			''' this class is never set to null.
			''' </summary>
			Private Function nextSpi(  oldSpi As KeyPairGeneratorSpi,   reinit As Boolean) As KeyPairGeneratorSpi
				SyncLock lock
					' somebody else did a failover concurrently
					' try that spi now
					If (oldSpi IsNot Nothing) AndAlso (oldSpi IsNot spi) Then Return spi
					If serviceIterator Is Nothing Then Return Nothing
					Do While serviceIterator.MoveNext()
						Dim s As java.security.Provider.Service = serviceIterator.Current
						Try
							Dim inst As Object = s.newInstance(Nothing)
							' ignore non-spis
							If TypeOf inst Is KeyPairGeneratorSpi = False Then Continue Do
							If TypeOf inst Is KeyPairGenerator Then Continue Do
							Dim spi As KeyPairGeneratorSpi = CType(inst, KeyPairGeneratorSpi)
							If reinit Then
								If initType = I_SIZE Then
									spi.initialize(initKeySize, initRandom)
								ElseIf initType = I_PARAMS Then
									spi.initialize(initParams, initRandom)
								ElseIf initType <> I_NONE Then
									Throw New AssertionError("KeyPairGenerator initType: " & initType)
								End If
							End If
							provider = s.provider
							Me.spi = spi
							Return spi
						Catch e As Exception
							' ignore
						End Try
					Loop
					disableFailover()
					Return Nothing
				End SyncLock
			End Function

			Friend Overrides Sub disableFailover()
				serviceIterator = Nothing
				initType = 0
				initParams = Nothing
				initRandom = Nothing
			End Sub

			' engine method
			Public Overrides Sub initialize(  keysize As Integer,   random_Renamed As SecureRandom)
				If serviceIterator Is Nothing Then
					spi.initialize(keysize, random_Renamed)
					Return
				End If
				Dim failure As RuntimeException = Nothing
				Dim mySpi As KeyPairGeneratorSpi = spi
				Do
					Try
						mySpi.initialize(keysize, random_Renamed)
						initType = I_SIZE
						initKeySize = keysize
						initParams = Nothing
						initRandom = random_Renamed
						Return
					Catch e As RuntimeException
						If failure Is Nothing Then failure = e
						mySpi = nextSpi(mySpi, False)
					End Try
				Loop While mySpi IsNot Nothing
				Throw failure
			End Sub

			' engine method
			Public Overrides Sub initialize(  params As java.security.spec.AlgorithmParameterSpec,   random_Renamed As SecureRandom)
				If serviceIterator Is Nothing Then
					spi.initialize(params, random_Renamed)
					Return
				End If
				Dim failure As Exception = Nothing
				Dim mySpi As KeyPairGeneratorSpi = spi
				Do
					Try
						mySpi.initialize(params, random_Renamed)
						initType = I_PARAMS
						initKeySize = 0
						initParams = params
						initRandom = random_Renamed
						Return
					Catch e As Exception
						If failure Is Nothing Then failure = e
						mySpi = nextSpi(mySpi, False)
					End Try
				Loop While mySpi IsNot Nothing
				If TypeOf failure Is RuntimeException Then Throw CType(failure, RuntimeException)
				' must be an InvalidAlgorithmParameterException
				Throw CType(failure, InvalidAlgorithmParameterException)
			End Sub

			' engine method
			Public Overrides Function generateKeyPair() As KeyPair
				If serviceIterator Is Nothing Then Return spi.generateKeyPair()
				Dim failure As RuntimeException = Nothing
				Dim mySpi As KeyPairGeneratorSpi = spi
				Do
					Try
						Return mySpi.generateKeyPair()
					Catch e As RuntimeException
						If failure Is Nothing Then failure = e
						mySpi = nextSpi(mySpi, True)
					End Try
				Loop While mySpi IsNot Nothing
				Throw failure
			End Function
		End Class

	End Class

End Namespace