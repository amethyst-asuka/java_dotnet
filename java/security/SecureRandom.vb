Imports System
Imports System.Runtime.CompilerServices
Imports sun.security.jca

'
' * Copyright (c) 1996, 2013, Oracle and/or its affiliates. All rights reserved.
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
	''' This class provides a cryptographically strong random number
	''' generator (RNG).
	''' 
	''' <p>A cryptographically strong random number
	''' minimally complies with the statistical random number generator tests
	''' specified in <a href="http://csrc.nist.gov/cryptval/140-2.htm">
	''' <i>FIPS 140-2, Security Requirements for Cryptographic Modules</i></a>,
	''' section 4.9.1.
	''' Additionally, SecureRandom must produce non-deterministic output.
	''' Therefore any seed material passed to a SecureRandom object must be
	''' unpredictable, and all SecureRandom output sequences must be
	''' cryptographically strong, as described in
	''' <a href="http://www.ietf.org/rfc/rfc1750.txt">
	''' <i>RFC 1750: Randomness Recommendations for Security</i></a>.
	''' 
	''' <p>A caller obtains a SecureRandom instance via the
	''' no-argument constructor or one of the {@code getInstance} methods:
	''' 
	''' <pre>
	'''      SecureRandom random = new SecureRandom();
	''' </pre>
	''' 
	''' <p> Many SecureRandom implementations are in the form of a pseudo-random
	''' number generator (PRNG), which means they use a deterministic algorithm
	''' to produce a pseudo-random sequence from a true random seed.
	''' Other implementations may produce true random numbers,
	''' and yet others may use a combination of both techniques.
	''' 
	''' <p> Typical callers of SecureRandom invoke the following methods
	''' to retrieve random bytes:
	''' 
	''' <pre>
	'''      SecureRandom random = new SecureRandom();
	'''      byte bytes[] = new byte[20];
	'''      random.nextBytes(bytes);
	''' </pre>
	''' 
	''' <p> Callers may also invoke the {@code generateSeed} method
	''' to generate a given number of seed bytes (to seed other random number
	''' generators, for example):
	''' <pre>
	'''      byte seed[] = random.generateSeed(20);
	''' </pre>
	''' 
	''' Note: Depending on the implementation, the {@code generateSeed} and
	''' {@code nextBytes} methods may block as entropy is being gathered,
	''' for example, if they need to read from /dev/random on various Unix-like
	''' operating systems.
	''' </summary>
	''' <seealso cref= java.security.SecureRandomSpi </seealso>
	''' <seealso cref= java.util.Random
	''' 
	''' @author Benjamin Renaud
	''' @author Josh Bloch </seealso>

	Public Class SecureRandom
		Inherits Random

		Private Shared ReadOnly pdebug As sun.security.util.Debug = sun.security.util.Debug.getInstance("provider", "Provider")
		Private Shared ReadOnly skipDebug As Boolean = sun.security.util.Debug.isOn("engine=") AndAlso Not sun.security.util.Debug.isOn("securerandom")

		''' <summary>
		''' The provider.
		''' 
		''' @serial
		''' @since 1.2
		''' </summary>
		Private provider_Renamed As Provider = Nothing

		''' <summary>
		''' The provider implementation.
		''' 
		''' @serial
		''' @since 1.2
		''' </summary>
		Private secureRandomSpi As SecureRandomSpi = Nothing

	'    
	'     * The algorithm name of null if unknown.
	'     *
	'     * @serial
	'     * @since 1.5
	'     
		Private algorithm As String

		' Seed Generator
'JAVA TO VB CONVERTER TODO TASK: There is no VB equivalent to 'volatile':
		Private Shared seedGenerator As SecureRandom = Nothing

		''' <summary>
		''' Constructs a secure random number generator (RNG) implementing the
		''' default random number algorithm.
		''' 
		''' <p> This constructor traverses the list of registered security Providers,
		''' starting with the most preferred Provider.
		''' A new SecureRandom object encapsulating the
		''' SecureRandomSpi implementation from the first
		''' Provider that supports a SecureRandom (RNG) algorithm is returned.
		''' If none of the Providers support a RNG algorithm,
		''' then an implementation-specific default is returned.
		''' 
		''' <p> Note that the list of registered providers may be retrieved via
		''' the <seealso cref="Security#getProviders() Security.getProviders()"/> method.
		''' 
		''' <p> See the SecureRandom section in the <a href=
		''' "{@docRoot}/../technotes/guides/security/StandardNames.html#SecureRandom">
		''' Java Cryptography Architecture Standard Algorithm Name Documentation</a>
		''' for information about standard RNG algorithm names.
		''' 
		''' <p> The returned SecureRandom object has not been seeded.  To seed the
		''' returned object, call the {@code setSeed} method.
		''' If {@code setSeed} is not called, the first call to
		''' {@code nextBytes} will force the SecureRandom object to seed itself.
		''' This self-seeding will not occur if {@code setSeed} was
		''' previously called.
		''' </summary>
		Public Sub New()
	'        
	'         * This call to our superclass constructor will result in a call
	'         * to our own {@code setSeed} method, which will return
	'         * immediately when it is passed zero.
	'         
			MyBase.New(0)
			getDefaultPRNG(False, Nothing)
		End Sub

		''' <summary>
		''' Constructs a secure random number generator (RNG) implementing the
		''' default random number algorithm.
		''' The SecureRandom instance is seeded with the specified seed bytes.
		''' 
		''' <p> This constructor traverses the list of registered security Providers,
		''' starting with the most preferred Provider.
		''' A new SecureRandom object encapsulating the
		''' SecureRandomSpi implementation from the first
		''' Provider that supports a SecureRandom (RNG) algorithm is returned.
		''' If none of the Providers support a RNG algorithm,
		''' then an implementation-specific default is returned.
		''' 
		''' <p> Note that the list of registered providers may be retrieved via
		''' the <seealso cref="Security#getProviders() Security.getProviders()"/> method.
		''' 
		''' <p> See the SecureRandom section in the <a href=
		''' "{@docRoot}/../technotes/guides/security/StandardNames.html#SecureRandom">
		''' Java Cryptography Architecture Standard Algorithm Name Documentation</a>
		''' for information about standard RNG algorithm names.
		''' </summary>
		''' <param name="seed"> the seed. </param>
		Public Sub New(  seed As SByte())
			MyBase.New(0)
			getDefaultPRNG(True, seed)
		End Sub

		Private Sub getDefaultPRNG(  setSeed As Boolean,   seed As SByte())
			Dim prng As String = prngAlgorithm
			If prng Is Nothing Then
				' bummer, get the SUN implementation
				prng = "SHA1PRNG"
				Me.secureRandomSpi = New sun.security.provider.SecureRandom
				Me.provider_Renamed = Providers.sunProvider
				If seedeed Then Me.secureRandomSpi.engineSetSeed(seed)
			Else
				Try
					Dim random_Renamed As SecureRandom = SecureRandom.getInstance(prng)
					Me.secureRandomSpi = random_Renamed.secureRandomSpi
					Me.provider_Renamed = random_Renamed.provider
					If seedeed Then Me.secureRandomSpi.engineSetSeed(seed)
				Catch nsae As NoSuchAlgorithmException
					' never happens, because we made sure the algorithm exists
					Throw New RuntimeException(nsae)
				End Try
			End If
			' JDK 1.1 based implementations subclass SecureRandom instead of
			' SecureRandomSpi. They will also go through this code path because
			' they must call a SecureRandom constructor as it is their superclass.
			' If we are dealing with such an implementation, do not set the
			' algorithm value as it would be inaccurate.
			If Me.GetType() = GetType(SecureRandom) Then Me.algorithm = prng
		End Sub

		''' <summary>
		''' Creates a SecureRandom object.
		''' </summary>
		''' <param name="secureRandomSpi"> the SecureRandom implementation. </param>
		''' <param name="provider"> the provider. </param>
		Protected Friend Sub New(  secureRandomSpi As SecureRandomSpi,   provider_Renamed As Provider)
			Me.New(secureRandomSpi, provider_Renamed, Nothing)
		End Sub

		Private Sub New(  secureRandomSpi As SecureRandomSpi,   provider_Renamed As Provider,   algorithm As String)
			MyBase.New(0)
			Me.secureRandomSpi = secureRandomSpi
			Me.provider_Renamed = provider_Renamed
			Me.algorithm = algorithm

			If (Not skipDebug) AndAlso pdebug IsNot Nothing Then pdebug.println("SecureRandom." & algorithm & " algorithm from: " & Me.provider_Renamed.name)
		End Sub

		''' <summary>
		''' Returns a SecureRandom object that implements the specified
		''' Random Number Generator (RNG) algorithm.
		''' 
		''' <p> This method traverses the list of registered security Providers,
		''' starting with the most preferred Provider.
		''' A new SecureRandom object encapsulating the
		''' SecureRandomSpi implementation from the first
		''' Provider that supports the specified algorithm is returned.
		''' 
		''' <p> Note that the list of registered providers may be retrieved via
		''' the <seealso cref="Security#getProviders() Security.getProviders()"/> method.
		''' 
		''' <p> The returned SecureRandom object has not been seeded.  To seed the
		''' returned object, call the {@code setSeed} method.
		''' If {@code setSeed} is not called, the first call to
		''' {@code nextBytes} will force the SecureRandom object to seed itself.
		''' This self-seeding will not occur if {@code setSeed} was
		''' previously called.
		''' </summary>
		''' <param name="algorithm"> the name of the RNG algorithm.
		''' See the SecureRandom section in the <a href=
		''' "{@docRoot}/../technotes/guides/security/StandardNames.html#SecureRandom">
		''' Java Cryptography Architecture Standard Algorithm Name Documentation</a>
		''' for information about standard RNG algorithm names.
		''' </param>
		''' <returns> the new SecureRandom object.
		''' </returns>
		''' <exception cref="NoSuchAlgorithmException"> if no Provider supports a
		'''          SecureRandomSpi implementation for the
		'''          specified algorithm.
		''' </exception>
		''' <seealso cref= Provider
		''' 
		''' @since 1.2 </seealso>
		Public Shared Function getInstance(  algorithm As String) As SecureRandom
			Dim instance_Renamed As sun.security.jca.GetInstance.Instance = GetInstance.getInstance("SecureRandom", GetType(SecureRandomSpi), algorithm)
			Return New SecureRandom(CType(instance_Renamed.impl, SecureRandomSpi), instance_Renamed.provider, algorithm)
		End Function

		''' <summary>
		''' Returns a SecureRandom object that implements the specified
		''' Random Number Generator (RNG) algorithm.
		''' 
		''' <p> A new SecureRandom object encapsulating the
		''' SecureRandomSpi implementation from the specified provider
		''' is returned.  The specified provider must be registered
		''' in the security provider list.
		''' 
		''' <p> Note that the list of registered providers may be retrieved via
		''' the <seealso cref="Security#getProviders() Security.getProviders()"/> method.
		''' 
		''' <p> The returned SecureRandom object has not been seeded.  To seed the
		''' returned object, call the {@code setSeed} method.
		''' If {@code setSeed} is not called, the first call to
		''' {@code nextBytes} will force the SecureRandom object to seed itself.
		''' This self-seeding will not occur if {@code setSeed} was
		''' previously called.
		''' </summary>
		''' <param name="algorithm"> the name of the RNG algorithm.
		''' See the SecureRandom section in the <a href=
		''' "{@docRoot}/../technotes/guides/security/StandardNames.html#SecureRandom">
		''' Java Cryptography Architecture Standard Algorithm Name Documentation</a>
		''' for information about standard RNG algorithm names.
		''' </param>
		''' <param name="provider"> the name of the provider.
		''' </param>
		''' <returns> the new SecureRandom object.
		''' </returns>
		''' <exception cref="NoSuchAlgorithmException"> if a SecureRandomSpi
		'''          implementation for the specified algorithm is not
		'''          available from the specified provider.
		''' </exception>
		''' <exception cref="NoSuchProviderException"> if the specified provider is not
		'''          registered in the security provider list.
		''' </exception>
		''' <exception cref="IllegalArgumentException"> if the provider name is null
		'''          or empty.
		''' </exception>
		''' <seealso cref= Provider
		''' 
		''' @since 1.2 </seealso>
		Public Shared Function getInstance(  algorithm As String,   provider_Renamed As String) As SecureRandom
			Dim instance_Renamed As sun.security.jca.GetInstance.Instance = GetInstance.getInstance("SecureRandom", GetType(SecureRandomSpi), algorithm, provider_Renamed)
			Return New SecureRandom(CType(instance_Renamed.impl, SecureRandomSpi), instance_Renamed.provider, algorithm)
		End Function

		''' <summary>
		''' Returns a SecureRandom object that implements the specified
		''' Random Number Generator (RNG) algorithm.
		''' 
		''' <p> A new SecureRandom object encapsulating the
		''' SecureRandomSpi implementation from the specified Provider
		''' object is returned.  Note that the specified Provider object
		''' does not have to be registered in the provider list.
		''' 
		''' <p> The returned SecureRandom object has not been seeded.  To seed the
		''' returned object, call the {@code setSeed} method.
		''' If {@code setSeed} is not called, the first call to
		''' {@code nextBytes} will force the SecureRandom object to seed itself.
		''' This self-seeding will not occur if {@code setSeed} was
		''' previously called.
		''' </summary>
		''' <param name="algorithm"> the name of the RNG algorithm.
		''' See the SecureRandom section in the <a href=
		''' "{@docRoot}/../technotes/guides/security/StandardNames.html#SecureRandom">
		''' Java Cryptography Architecture Standard Algorithm Name Documentation</a>
		''' for information about standard RNG algorithm names.
		''' </param>
		''' <param name="provider"> the provider.
		''' </param>
		''' <returns> the new SecureRandom object.
		''' </returns>
		''' <exception cref="NoSuchAlgorithmException"> if a SecureRandomSpi
		'''          implementation for the specified algorithm is not available
		'''          from the specified Provider object.
		''' </exception>
		''' <exception cref="IllegalArgumentException"> if the specified provider is null.
		''' </exception>
		''' <seealso cref= Provider
		''' 
		''' @since 1.4 </seealso>
		Public Shared Function getInstance(  algorithm As String,   provider_Renamed As Provider) As SecureRandom
			Dim instance_Renamed As sun.security.jca.GetInstance.Instance = GetInstance.getInstance("SecureRandom", GetType(SecureRandomSpi), algorithm, provider_Renamed)
			Return New SecureRandom(CType(instance_Renamed.impl, SecureRandomSpi), instance_Renamed.provider, algorithm)
		End Function

		''' <summary>
		''' Returns the SecureRandomSpi of this SecureRandom object.
		''' </summary>
		Friend Overridable Property secureRandomSpi As SecureRandomSpi
			Get
				Return secureRandomSpi
			End Get
		End Property

		''' <summary>
		''' Returns the provider of this SecureRandom object.
		''' </summary>
		''' <returns> the provider of this SecureRandom object. </returns>
		Public Property provider As Provider
			Get
				Return provider_Renamed
			End Get
		End Property

		''' <summary>
		''' Returns the name of the algorithm implemented by this SecureRandom
		''' object.
		''' </summary>
		''' <returns> the name of the algorithm or {@code unknown}
		'''          if the algorithm name cannot be determined.
		''' @since 1.5 </returns>
		Public Overridable Property algorithm As String
			Get
				Return If(algorithm IsNot Nothing, algorithm, "unknown")
			End Get
		End Property

		''' <summary>
		''' Reseeds this random object. The given seed supplements, rather than
		''' replaces, the existing seed. Thus, repeated calls are guaranteed
		''' never to reduce randomness.
		''' </summary>
		''' <param name="seed"> the seed.
		''' </param>
		''' <seealso cref= #getSeed </seealso>
		<MethodImpl(MethodImplOptions.Synchronized)> _
		Public Overridable Property seed As SByte()
			Set(  seed As SByte())
				secureRandomSpi.engineSetSeed(seed)
			End Set
		End Property

		''' <summary>
		''' Reseeds this random object, using the eight bytes contained
		''' in the given {@code long seed}. The given seed supplements,
		''' rather than replaces, the existing seed. Thus, repeated calls
		''' are guaranteed never to reduce randomness.
		''' 
		''' <p>This method is defined for compatibility with
		''' {@code java.util.Random}.
		''' </summary>
		''' <param name="seed"> the seed.
		''' </param>
		''' <seealso cref= #getSeed </seealso>
		Public Overrides Property seed As Long
			Set(  seed As Long)
		'        
		'         * Ignore call from super constructor (as well as any other calls
		'         * unfortunate enough to be passing 0).  It's critical that we
		'         * ignore call from superclass constructor, as digest has not
		'         * yet been initialized at that point.
		'         
				If seed <> 0 Then secureRandomSpi.engineSetSeed(longToByteArray(seed))
			End Set
		End Property

		''' <summary>
		''' Generates a user-specified number of random bytes.
		''' 
		''' <p> If a call to {@code setSeed} had not occurred previously,
		''' the first call to this method forces this SecureRandom object
		''' to seed itself.  This self-seeding will not occur if
		''' {@code setSeed} was previously called.
		''' </summary>
		''' <param name="bytes"> the array to be filled in with random bytes. </param>
		<MethodImpl(MethodImplOptions.Synchronized)> _
		Public Overrides Sub nextBytes(  bytes As SByte())
			secureRandomSpi.engineNextBytes(bytes)
		End Sub

		''' <summary>
		''' Generates an integer containing the user-specified number of
		''' pseudo-random bits (right justified, with leading zeros).  This
		''' method overrides a {@code java.util.Random} method, and serves
		''' to provide a source of random bits to all of the methods inherited
		''' from that class (for example, {@code nextInt},
		''' {@code nextLong}, and {@code nextFloat}).
		''' </summary>
		''' <param name="numBits"> number of pseudo-random bits to be generated, where
		''' {@code 0 <= numBits <= 32}.
		''' </param>
		''' <returns> an {@code int} containing the user-specified number
		''' of pseudo-random bits (right justified, with leading zeros). </returns>
		Protected Friend NotOverridable Overrides Function [next](  numBits As Integer) As Integer
			Dim numBytes As Integer = (numBits+7)\8
			Dim b As SByte() = New SByte(numBytes - 1){}
			Dim next_Renamed As Integer = 0

			nextBytes(b)
			For i As Integer = 0 To numBytes - 1
				next_Renamed = (next_Renamed << 8) + (b(i) And &HFF)
			Next i

			Return CInt(CUInt(next_Renamed) >> (numBytes*8 - numBits))
		End Function

		''' <summary>
		''' Returns the given number of seed bytes, computed using the seed
		''' generation algorithm that this class uses to seed itself.  This
		''' call may be used to seed other random number generators.
		''' 
		''' <p>This method is only included for backwards compatibility.
		''' The caller is encouraged to use one of the alternative
		''' {@code getInstance} methods to obtain a SecureRandom object, and
		''' then call the {@code generateSeed} method to obtain seed bytes
		''' from that object.
		''' </summary>
		''' <param name="numBytes"> the number of seed bytes to generate.
		''' </param>
		''' <returns> the seed bytes.
		''' </returns>
		''' <seealso cref= #setSeed </seealso>
		Public Shared Function getSeed(  numBytes As Integer) As SByte()
			If seedGenerator Is Nothing Then seedGenerator = New SecureRandom
			Return seedGenerator.generateSeed(numBytes)
		End Function

		''' <summary>
		''' Returns the given number of seed bytes, computed using the seed
		''' generation algorithm that this class uses to seed itself.  This
		''' call may be used to seed other random number generators.
		''' </summary>
		''' <param name="numBytes"> the number of seed bytes to generate.
		''' </param>
		''' <returns> the seed bytes. </returns>
		Public Overridable Function generateSeed(  numBytes As Integer) As SByte()
			Return secureRandomSpi.engineGenerateSeed(numBytes)
		End Function

		''' <summary>
		''' Helper function to convert a long into a byte array (least significant
		''' byte first).
		''' </summary>
		Private Shared Function longToByteArray(  l As Long) As SByte()
			Dim retVal As SByte() = New SByte(7){}

			For i As Integer = 0 To 7
				retVal(i) = CByte(l)
				l >>= 8
			Next i

			Return retVal
		End Function

		''' <summary>
		''' Gets a default PRNG algorithm by looking through all registered
		''' providers. Returns the first PRNG algorithm of the first provider that
		''' has registered a SecureRandom implementation, or null if none of the
		''' registered providers supplies a SecureRandom implementation.
		''' </summary>
		PrivateShared ReadOnly PropertyprngAlgorithm As String
			Get
				For Each p As Provider In Providers.providerList.providers()
					For Each s As java.security.Provider.Service In p.services
						If s.type.Equals("SecureRandom") Then Return s.algorithm
					Next s
				Next p
				Return Nothing
			End Get
		End Property

	'    
	'     * Lazily initialize since Pattern.compile() is heavy.
	'     * Effective Java (2nd Edition), Item 71.
	'     
		Private NotInheritable Class StrongPatternHolder
	'        
	'         * Entries are alg:prov separated by ,
	'         * Allow for prepended/appended whitespace between entries.
	'         *
	'         * Capture groups:
	'         *     1 - alg
	'         *     2 - :prov (optional)
	'         *     3 - prov (optional)
	'         *     4 - ,nextEntry (optional)
	'         *     5 - nextEntry (optional)
	'         
			Private Shared pattern As Pattern = Pattern.compile("\s*([\S&&[^:,]]*)(\:([\S&&[^,]]*))?\s*(\,(.*))?")
		End Class

		''' <summary>
		''' Returns a {@code SecureRandom} object that was selected by using
		''' the algorithms/providers specified in the {@code
		''' securerandom.strongAlgorithms} <seealso cref="Security"/> property.
		''' <p>
		''' Some situations require strong random values, such as when
		''' creating high-value/long-lived secrets like RSA public/private
		''' keys.  To help guide applications in selecting a suitable strong
		''' {@code SecureRandom} implementation, Java distributions
		''' include a list of known strong {@code SecureRandom}
		''' implementations in the {@code securerandom.strongAlgorithms}
		''' Security property.
		''' <p>
		''' Every implementation of the Java platform is required to
		''' support at least one strong {@code SecureRandom} implementation.
		''' </summary>
		''' <returns> a strong {@code SecureRandom} implementation as indicated
		''' by the {@code securerandom.strongAlgorithms} Security property
		''' </returns>
		''' <exception cref="NoSuchAlgorithmException"> if no algorithm is available
		''' </exception>
		''' <seealso cref= Security#getProperty(String)
		''' 
		''' @since 1.8 </seealso>
		PublicShared ReadOnly PropertyinstanceStrong As SecureRandom
			Get
    
				Dim [property] As String = AccessController.doPrivileged(New PrivilegedActionAnonymousInnerClassHelper(Of T)
    
				If ([property] Is Nothing) OrElse ([property].length() = 0) Then Throw New NoSuchAlgorithmException("Null/empty securerandom.strongAlgorithms Security Property")
    
				Dim remainder As String = [property]
				Do While remainder IsNot Nothing
					Dim m As Matcher
					m = StrongPatternHolder.pattern.matcher(remainder)
					If m .matches() Then
    
						Dim alg As String = m.group(1)
						Dim prov As String = m.group(3)
    
						Try
							If prov Is Nothing Then
								Return SecureRandom.getInstance(alg)
							Else
								Return SecureRandom.getInstance(alg, prov)
							End If
	'JAVA TO VB CONVERTER TODO TASK: There is no equivalent in VB to Java 'multi-catch' syntax:
						Catch NoSuchAlgorithmException Or NoSuchProviderException e
						End Try
						remainder = m.group(5)
					Else
						remainder = Nothing
					End If
				Loop
    
				Throw New NoSuchAlgorithmException("No strong SecureRandom impls available: " & [property])
			End Get
		End Property

		Private Class PrivilegedActionAnonymousInnerClassHelper(Of T)
			Implements PrivilegedAction(Of T)

			Public Overrides Function run() As String
				Return Security.getProperty("securerandom.strongAlgorithms")
			End Function
		End Class

		' Declare serialVersionUID to be compatible with JDK1.1
		Friend Shadows Const serialVersionUID As Long = 4940670005562187L

		' Retain unused values serialized from JDK1.1
		''' <summary>
		''' @serial
		''' </summary>
		Private state As SByte()
		''' <summary>
		''' @serial
		''' </summary>
		Private digest As MessageDigest = Nothing
		''' <summary>
		''' @serial
		''' 
		''' We know that the MessageDigest class does not implement
		''' java.io.Serializable.  However, since this field is no longer
		''' used, it will always be NULL and won't affect the serialization
		''' of the SecureRandom class itself.
		''' </summary>
		Private randomBytes As SByte()
		''' <summary>
		''' @serial
		''' </summary>
		Private randomBytesUsed As Integer
		''' <summary>
		''' @serial
		''' </summary>
		Private counter As Long
	End Class

End Namespace