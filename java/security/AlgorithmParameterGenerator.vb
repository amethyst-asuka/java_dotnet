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
	''' The {@code AlgorithmParameterGenerator} class is used to generate a
	''' set of
	''' parameters to be used with a certain algorithm. Parameter generators
	''' are constructed using the {@code getInstance} factory methods
	''' (static methods that return instances of a given [Class]).
	''' 
	''' <P>The object that will generate the parameters can be initialized
	''' in two different ways: in an algorithm-independent manner, or in an
	''' algorithm-specific manner:
	''' 
	''' <ul>
	''' <li>The algorithm-independent approach uses the fact that all parameter
	''' generators share the concept of a "size" and a
	''' source of randomness. The measure of size is universally shared
	''' by all algorithm parameters, though it is interpreted differently
	''' for different algorithms. For example, in the case of parameters for
	''' the <i>DSA</i> algorithm, "size" corresponds to the size
	''' of the prime modulus (in bits).
	''' When using this approach, algorithm-specific parameter generation
	''' values - if any - default to some standard values, unless they can be
	''' derived from the specified size.
	''' 
	''' <li>The other approach initializes a parameter generator object
	''' using algorithm-specific semantics, which are represented by a set of
	''' algorithm-specific parameter generation values. To generate
	''' Diffie-Hellman system parameters, for example, the parameter generation
	''' values usually
	''' consist of the size of the prime modulus and the size of the
	''' random exponent, both specified in number of bits.
	''' </ul>
	''' 
	''' <P>In case the client does not explicitly initialize the
	''' AlgorithmParameterGenerator
	''' (via a call to an {@code init} method), each provider must supply (and
	''' document) a default initialization. For example, the Sun provider uses a
	''' default modulus prime size of 1024 bits for the generation of DSA
	''' parameters.
	''' 
	''' <p> Every implementation of the Java platform is required to support the
	''' following standard {@code AlgorithmParameterGenerator} algorithms and
	''' keysizes in parentheses:
	''' <ul>
	''' <li>{@code DiffieHellman} (1024)</li>
	''' <li>{@code DSA} (1024)</li>
	''' </ul>
	''' These algorithms are described in the <a href=
	''' "{@docRoot}/../technotes/guides/security/StandardNames.html#AlgorithmParameterGenerator">
	''' AlgorithmParameterGenerator section</a> of the
	''' Java Cryptography Architecture Standard Algorithm Name Documentation.
	''' Consult the release documentation for your implementation to see if any
	''' other algorithms are supported.
	''' 
	''' @author Jan Luehe
	''' 
	''' </summary>
	''' <seealso cref= AlgorithmParameters </seealso>
	''' <seealso cref= java.security.spec.AlgorithmParameterSpec
	''' 
	''' @since 1.2 </seealso>

	Public Class AlgorithmParameterGenerator

        ' The provider
        Private _provider As Provider

        ' The provider implementation (delegate)
        Private paramGenSpi As AlgorithmParameterGeneratorSpi

        ' The algorithm
        Private _algorithm As String

        ''' <summary>
        ''' Creates an AlgorithmParameterGenerator object.
        ''' </summary>
        ''' <param name="paramGenSpi"> the delegate </param>
        ''' <param name="provider"> the provider </param>
        ''' <param name="algorithm"> the algorithm </param>
        Protected Friend Sub New(  paramGenSpi As AlgorithmParameterGeneratorSpi,   provider_Renamed As Provider,   algorithm As String)
			Me.paramGenSpi = paramGenSpi
            Me._provider = provider_Renamed
            Me._algorithm = algorithm
        End Sub

        ''' <summary>
        ''' Returns the standard name of the algorithm this parameter
        ''' generator is associated with.
        ''' </summary>
        ''' <returns> the string name of the algorithm. </returns>
        Public ReadOnly Property algorithm As String
            Get
                Return Me._algorithm
            End Get
        End Property

        ''' <summary>
        ''' Returns an AlgorithmParameterGenerator object for generating
        ''' a set of parameters to be used with the specified algorithm.
        ''' 
        ''' <p> This method traverses the list of registered security Providers,
        ''' starting with the most preferred Provider.
        ''' A new AlgorithmParameterGenerator object encapsulating the
        ''' AlgorithmParameterGeneratorSpi implementation from the first
        ''' Provider that supports the specified algorithm is returned.
        ''' 
        ''' <p> Note that the list of registered providers may be retrieved via
        ''' the <seealso cref="Security#getProviders() Security.getProviders()"/> method.
        ''' </summary>
        ''' <param name="algorithm"> the name of the algorithm this
        ''' parameter generator is associated with.
        ''' See the AlgorithmParameterGenerator section in the <a href=
        ''' "{@docRoot}/../technotes/guides/security/StandardNames.html#AlgorithmParameterGenerator">
        ''' Java Cryptography Architecture Standard Algorithm Name Documentation</a>
        ''' for information about standard algorithm names.
        ''' </param>
        ''' <returns> the new AlgorithmParameterGenerator object.
        ''' </returns>
        ''' <exception cref="NoSuchAlgorithmException"> if no Provider supports an
        '''          AlgorithmParameterGeneratorSpi implementation for the
        '''          specified algorithm.
        ''' </exception>
        ''' <seealso cref= Provider </seealso>
        Public Shared Function getInstance(  algorithm As String) As AlgorithmParameterGenerator
				Try
					Dim objs As Object() = Security.getImpl(algorithm, "AlgorithmParameterGenerator", CStr(Nothing))
					Return New AlgorithmParameterGenerator(CType(objs(0), AlgorithmParameterGeneratorSpi), CType(objs(1), Provider), algorithm)
				Catch e As NoSuchProviderException
					Throw New NoSuchAlgorithmException(algorithm & " not found")
				End Try
		End Function

		''' <summary>
		''' Returns an AlgorithmParameterGenerator object for generating
		''' a set of parameters to be used with the specified algorithm.
		''' 
		''' <p> A new AlgorithmParameterGenerator object encapsulating the
		''' AlgorithmParameterGeneratorSpi implementation from the specified provider
		''' is returned.  The specified provider must be registered
		''' in the security provider list.
		''' 
		''' <p> Note that the list of registered providers may be retrieved via
		''' the <seealso cref="Security#getProviders() Security.getProviders()"/> method.
		''' </summary>
		''' <param name="algorithm"> the name of the algorithm this
		''' parameter generator is associated with.
		''' See the AlgorithmParameterGenerator section in the <a href=
		''' "{@docRoot}/../technotes/guides/security/StandardNames.html#AlgorithmParameterGenerator">
		''' Java Cryptography Architecture Standard Algorithm Name Documentation</a>
		''' for information about standard algorithm names.
		''' </param>
		''' <param name="provider"> the string name of the Provider.
		''' </param>
		''' <returns> the new AlgorithmParameterGenerator object.
		''' </returns>
		''' <exception cref="NoSuchAlgorithmException"> if an AlgorithmParameterGeneratorSpi
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
		Public Shared Function getInstance(  algorithm As String,   provider_Renamed As String) As AlgorithmParameterGenerator
			If provider_Renamed Is Nothing OrElse provider_Renamed.length() = 0 Then Throw New IllegalArgumentException("missing provider")
			Dim objs As Object() = Security.getImpl(algorithm, "AlgorithmParameterGenerator", provider_Renamed)
			Return New AlgorithmParameterGenerator(CType(objs(0), AlgorithmParameterGeneratorSpi), CType(objs(1), Provider), algorithm)
		End Function

		''' <summary>
		''' Returns an AlgorithmParameterGenerator object for generating
		''' a set of parameters to be used with the specified algorithm.
		''' 
		''' <p> A new AlgorithmParameterGenerator object encapsulating the
		''' AlgorithmParameterGeneratorSpi implementation from the specified Provider
		''' object is returned.  Note that the specified Provider object
		''' does not have to be registered in the provider list.
		''' </summary>
		''' <param name="algorithm"> the string name of the algorithm this
		''' parameter generator is associated with.
		''' See the AlgorithmParameterGenerator section in the <a href=
		''' "{@docRoot}/../technotes/guides/security/StandardNames.html#AlgorithmParameterGenerator">
		''' Java Cryptography Architecture Standard Algorithm Name Documentation</a>
		''' for information about standard algorithm names.
		''' </param>
		''' <param name="provider"> the Provider object.
		''' </param>
		''' <returns> the new AlgorithmParameterGenerator object.
		''' </returns>
		''' <exception cref="NoSuchAlgorithmException"> if an AlgorithmParameterGeneratorSpi
		'''          implementation for the specified algorithm is not available
		'''          from the specified Provider object.
		''' </exception>
		''' <exception cref="IllegalArgumentException"> if the specified provider is null.
		''' </exception>
		''' <seealso cref= Provider
		''' 
		''' @since 1.4 </seealso>
		Public Shared Function getInstance(  algorithm As String,   provider_Renamed As Provider) As AlgorithmParameterGenerator
			If provider_Renamed Is Nothing Then Throw New IllegalArgumentException("missing provider")
			Dim objs As Object() = Security.getImpl(algorithm, "AlgorithmParameterGenerator", provider_Renamed)
			Return New AlgorithmParameterGenerator(CType(objs(0), AlgorithmParameterGeneratorSpi), CType(objs(1), Provider), algorithm)
		End Function

        ''' <summary>
        ''' Returns the provider of this algorithm parameter generator object.
        ''' </summary>
        ''' <returns> the provider of this algorithm parameter generator object </returns>
        Public ReadOnly Property provider As Provider
            Get
                Return Me._provider
            End Get
        End Property

        ''' <summary>
        ''' Initializes this parameter generator for a certain size.
        ''' To create the parameters, the {@code SecureRandom}
        ''' implementation of the highest-priority installed provider is used as
        ''' the source of randomness.
        ''' (If none of the installed providers supply an implementation of
        ''' {@code SecureRandom}, a system-provided source of randomness is
        ''' used.)
        ''' </summary>
        ''' <param name="size"> the size (number of bits). </param>
        Public Sub init(  size As Integer)
			paramGenSpi.engineInit(size, New SecureRandom)
		End Sub

		''' <summary>
		''' Initializes this parameter generator for a certain size and source
		''' of randomness.
		''' </summary>
		''' <param name="size"> the size (number of bits). </param>
		''' <param name="random"> the source of randomness. </param>
		Public Sub init(  size As Integer,   random As SecureRandom)
			paramGenSpi.engineInit(size, random)
		End Sub

		''' <summary>
		''' Initializes this parameter generator with a set of algorithm-specific
		''' parameter generation values.
		''' To generate the parameters, the {@code SecureRandom}
		''' implementation of the highest-priority installed provider is used as
		''' the source of randomness.
		''' (If none of the installed providers supply an implementation of
		''' {@code SecureRandom}, a system-provided source of randomness is
		''' used.)
		''' </summary>
		''' <param name="genParamSpec"> the set of algorithm-specific parameter generation values.
		''' </param>
		''' <exception cref="InvalidAlgorithmParameterException"> if the given parameter
		''' generation values are inappropriate for this parameter generator. </exception>
		Public Sub init(  genParamSpec As java.security.spec.AlgorithmParameterSpec)
				paramGenSpi.engineInit(genParamSpec, New SecureRandom)
		End Sub

		''' <summary>
		''' Initializes this parameter generator with a set of algorithm-specific
		''' parameter generation values.
		''' </summary>
		''' <param name="genParamSpec"> the set of algorithm-specific parameter generation values. </param>
		''' <param name="random"> the source of randomness.
		''' </param>
		''' <exception cref="InvalidAlgorithmParameterException"> if the given parameter
		''' generation values are inappropriate for this parameter generator. </exception>
		Public Sub init(  genParamSpec As java.security.spec.AlgorithmParameterSpec,   random As SecureRandom)
				paramGenSpi.engineInit(genParamSpec, random)
		End Sub

		''' <summary>
		''' Generates the parameters.
		''' </summary>
		''' <returns> the new AlgorithmParameters object. </returns>
		Public Function generateParameters() As AlgorithmParameters
			Return paramGenSpi.engineGenerateParameters()
		End Function
	End Class

End Namespace