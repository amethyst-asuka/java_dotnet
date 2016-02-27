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

Imports java.io
Imports java.lang

Namespace java.security


    ''' <summary>
    ''' This class is used as an opaque representation of cryptographic parameters.
    ''' 
    ''' <p>An {@code AlgorithmParameters} object for managing the parameters
    ''' for a particular algorithm can be obtained by
    ''' calling one of the {@code getInstance} factory methods
    ''' (static methods that return instances of a given [Class]).
    ''' 
    ''' <p>Once an {@code AlgorithmParameters} object is obtained, it must be
    ''' initialized via a call to {@code init}, using an appropriate parameter
    ''' specification or parameter encoding.
    ''' 
    ''' <p>A transparent parameter specification is obtained from an
    ''' {@code AlgorithmParameters} object via a call to
    ''' {@code getParameterSpec}, and a byte encoding of the parameters is
    ''' obtained via a call to {@code getEncoded}.
    ''' 
    ''' <p> Every implementation of the Java platform is required to support the
    ''' following standard {@code AlgorithmParameters} algorithms:
    ''' <ul>
    ''' <li>{@code AES}</li>
    ''' <li>{@code DES}</li>
    ''' <li>{@code DESede}</li>
    ''' <li>{@code DiffieHellman}</li>
    ''' <li>{@code DSA}</li>
    ''' </ul>
    ''' These algorithms are described in the <a href=
    ''' "{@docRoot}/../technotes/guides/security/StandardNames.html#AlgorithmParameters">
    ''' AlgorithmParameters section</a> of the
    ''' Java Cryptography Architecture Standard Algorithm Name Documentation.
    ''' Consult the release documentation for your implementation to see if any
    ''' other algorithms are supported.
    ''' 
    ''' @author Jan Luehe
    ''' 
    ''' </summary>
    ''' <seealso cref= java.security.spec.AlgorithmParameterSpec </seealso>
    ''' <seealso cref= java.security.spec.DSAParameterSpec </seealso>
    ''' <seealso cref= KeyPairGenerator
    ''' 
    ''' @since 1.2 </seealso>

    Public Class AlgorithmParameters

        ' The provider
        Private _provider As Provider

        ' The provider implementation (delegate)
        Private paramSpi As AlgorithmParametersSpi

        ' The algorithm
        Private _algorithm As String

        ' Has this object been initialized?
        Private initialized As Boolean = False

        ''' <summary>
        ''' Creates an AlgorithmParameters object.
        ''' </summary>
        ''' <param name="paramSpi"> the delegate </param>
        ''' <param name="provider"> the provider </param>
        ''' <param name="algorithm"> the algorithm </param>
        Protected Friend Sub New(ByVal paramSpi As AlgorithmParametersSpi, ByVal provider_Renamed As Provider, ByVal algorithm As String)
            Me.paramSpi = paramSpi
            Me._provider = provider_Renamed
            Me._algorithm = algorithm
        End Sub

        ''' <summary>
        ''' Returns the name of the algorithm associated with this parameter object.
        ''' </summary>
        ''' <returns> the algorithm name. </returns>
        Public ReadOnly Property algorithm As String
            Get
                Return Me._algorithm
            End Get
        End Property

        ''' <summary>
        ''' Returns a parameter object for the specified algorithm.
        ''' 
        ''' <p> This method traverses the list of registered security Providers,
        ''' starting with the most preferred Provider.
        ''' A new AlgorithmParameters object encapsulating the
        ''' AlgorithmParametersSpi implementation from the first
        ''' Provider that supports the specified algorithm is returned.
        ''' 
        ''' <p> Note that the list of registered providers may be retrieved via
        ''' the <seealso cref="Security#getProviders() Security.getProviders()"/> method.
        ''' 
        ''' <p> The returned parameter object must be initialized via a call to
        ''' {@code init}, using an appropriate parameter specification or
        ''' parameter encoding.
        ''' </summary>
        ''' <param name="algorithm"> the name of the algorithm requested.
        ''' See the AlgorithmParameters section in the <a href=
        ''' "{@docRoot}/../technotes/guides/security/StandardNames.html#AlgorithmParameters">
        ''' Java Cryptography Architecture Standard Algorithm Name Documentation</a>
        ''' for information about standard algorithm names.
        ''' </param>
        ''' <returns> the new parameter object.
        ''' </returns>
        ''' <exception cref="NoSuchAlgorithmException"> if no Provider supports an
        '''          AlgorithmParametersSpi implementation for the
        '''          specified algorithm.
        ''' </exception>
        ''' <seealso cref= Provider </seealso>
        Public Shared Function getInstance(ByVal algorithm As String) As AlgorithmParameters
            Try
                Dim objs As Object() = Security.getImpl(algorithm, "AlgorithmParameters", CStr(Nothing))
                Return New AlgorithmParameters(CType(objs(0), AlgorithmParametersSpi), CType(objs(1), Provider), algorithm)
            Catch e As NoSuchProviderException
                Throw New NoSuchAlgorithmException(algorithm & " not found")
            End Try
        End Function

        ''' <summary>
        ''' Returns a parameter object for the specified algorithm.
        ''' 
        ''' <p> A new AlgorithmParameters object encapsulating the
        ''' AlgorithmParametersSpi implementation from the specified provider
        ''' is returned.  The specified provider must be registered
        ''' in the security provider list.
        ''' 
        ''' <p> Note that the list of registered providers may be retrieved via
        ''' the <seealso cref="Security#getProviders() Security.getProviders()"/> method.
        ''' 
        ''' <p>The returned parameter object must be initialized via a call to
        ''' {@code init}, using an appropriate parameter specification or
        ''' parameter encoding.
        ''' </summary>
        ''' <param name="algorithm"> the name of the algorithm requested.
        ''' See the AlgorithmParameters section in the <a href=
        ''' "{@docRoot}/../technotes/guides/security/StandardNames.html#AlgorithmParameters">
        ''' Java Cryptography Architecture Standard Algorithm Name Documentation</a>
        ''' for information about standard algorithm names.
        ''' </param>
        ''' <param name="provider"> the name of the provider.
        ''' </param>
        ''' <returns> the new parameter object.
        ''' </returns>
        ''' <exception cref="NoSuchAlgorithmException"> if an AlgorithmParametersSpi
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
        Public Shared Function getInstance(ByVal algorithm As String, ByVal provider_Renamed As String) As AlgorithmParameters
            If provider_Renamed Is Nothing OrElse provider_Renamed.Length() = 0 Then Throw New IllegalArgumentException("missing provider")
            Dim objs As Object() = Security.getImpl(algorithm, "AlgorithmParameters", provider_Renamed)
            Return New AlgorithmParameters(CType(objs(0), AlgorithmParametersSpi), CType(objs(1), Provider), algorithm)
        End Function

        ''' <summary>
        ''' Returns a parameter object for the specified algorithm.
        ''' 
        ''' <p> A new AlgorithmParameters object encapsulating the
        ''' AlgorithmParametersSpi implementation from the specified Provider
        ''' object is returned.  Note that the specified Provider object
        ''' does not have to be registered in the provider list.
        ''' 
        ''' <p>The returned parameter object must be initialized via a call to
        ''' {@code init}, using an appropriate parameter specification or
        ''' parameter encoding.
        ''' </summary>
        ''' <param name="algorithm"> the name of the algorithm requested.
        ''' See the AlgorithmParameters section in the <a href=
        ''' "{@docRoot}/../technotes/guides/security/StandardNames.html#AlgorithmParameters">
        ''' Java Cryptography Architecture Standard Algorithm Name Documentation</a>
        ''' for information about standard algorithm names.
        ''' </param>
        ''' <param name="provider"> the name of the provider.
        ''' </param>
        ''' <returns> the new parameter object.
        ''' </returns>
        ''' <exception cref="NoSuchAlgorithmException"> if an AlgorithmParameterGeneratorSpi
        '''          implementation for the specified algorithm is not available
        '''          from the specified Provider object.
        ''' </exception>
        ''' <exception cref="IllegalArgumentException"> if the provider is null.
        ''' </exception>
        ''' <seealso cref= Provider
        ''' 
        ''' @since 1.4 </seealso>
        Public Shared Function getInstance(ByVal algorithm As String, ByVal provider_Renamed As Provider) As AlgorithmParameters
            If provider_Renamed Is Nothing Then Throw New IllegalArgumentException("missing provider")
            Dim objs As Object() = Security.getImpl(algorithm, "AlgorithmParameters", provider_Renamed)
            Return New AlgorithmParameters(CType(objs(0), AlgorithmParametersSpi), CType(objs(1), Provider), algorithm)
        End Function

        ''' <summary>
        ''' Returns the provider of this parameter object.
        ''' </summary>
        ''' <returns> the provider of this parameter object </returns>
        Public ReadOnly Property provider As Provider
            Get
                Return Me._provider
            End Get
        End Property

        ''' <summary>
        ''' Initializes this parameter object using the parameters
        ''' specified in {@code paramSpec}.
        ''' </summary>
        ''' <param name="paramSpec"> the parameter specification.
        ''' </param>
        ''' <exception cref="InvalidParameterSpecException"> if the given parameter
        ''' specification is inappropriate for the initialization of this parameter
        ''' object, or if this parameter object has already been initialized. </exception>
        Public Sub init(ByVal paramSpec As java.security.spec.AlgorithmParameterSpec)
            If Me.initialized Then Throw New java.security.spec.InvalidParameterSpecException("already initialized")
            paramSpi.engineInit(paramSpec)
            Me.initialized = True
        End Sub

        ''' <summary>
        ''' Imports the specified parameters and decodes them according to the
        ''' primary decoding format for parameters. The primary decoding
        ''' format for parameters is ASN.1, if an ASN.1 specification for this type
        ''' of parameters exists.
        ''' </summary>
        ''' <param name="params"> the encoded parameters.
        ''' </param>
        ''' <exception cref="IOException"> on decoding errors, or if this parameter object
        ''' has already been initialized. </exception>
        Public Sub init(ByVal params As SByte())
            If Me.initialized Then Throw New IOException("already initialized")
            paramSpi.engineInit(params)
            Me.initialized = True
        End Sub

        ''' <summary>
        ''' Imports the parameters from {@code params} and decodes them
        ''' according to the specified decoding scheme.
        ''' If {@code format} is null, the
        ''' primary decoding format for parameters is used. The primary decoding
        ''' format is ASN.1, if an ASN.1 specification for these parameters
        ''' exists.
        ''' </summary>
        ''' <param name="params"> the encoded parameters.
        ''' </param>
        ''' <param name="format"> the name of the decoding scheme.
        ''' </param>
        ''' <exception cref="IOException"> on decoding errors, or if this parameter object
        ''' has already been initialized. </exception>
        Public Sub init(ByVal params As SByte(), ByVal format As String)
            If Me.initialized Then Throw New IOException("already initialized")
            paramSpi.engineInit(params, format)
            Me.initialized = True
        End Sub

        ''' <summary>
        ''' Returns a (transparent) specification of this parameter object.
        ''' {@code paramSpec} identifies the specification class in which
        ''' the parameters should be returned. It could, for example, be
        ''' {@code DSAParameterSpec.class}, to indicate that the
        ''' parameters should be returned in an instance of the
        ''' {@code DSAParameterSpec} class.
        ''' </summary>
        ''' @param <T> the type of the parameter specification to be returrned </param>
        ''' <param name="paramSpec"> the specification class in which
        ''' the parameters should be returned.
        ''' </param>
        ''' <returns> the parameter specification.
        ''' </returns>
        ''' <exception cref="InvalidParameterSpecException"> if the requested parameter
        ''' specification is inappropriate for this parameter object, or if this
        ''' parameter object has not been initialized. </exception>
        Public Function getParameterSpec(Of T As java.security.spec.AlgorithmParameterSpec)(ByVal paramSpec As [Class]) As T
            If Me.initialized = False Then Throw New java.security.spec.InvalidParameterSpecException("not initialized")
            Return paramSpi.engineGetParameterSpec(paramSpec)
        End Function

        ''' <summary>
        ''' Returns the parameters in their primary encoding format.
        ''' The primary encoding format for parameters is ASN.1, if an ASN.1
        ''' specification for this type of parameters exists.
        ''' </summary>
        ''' <returns> the parameters encoded using their primary encoding format.
        ''' </returns>
        ''' <exception cref="IOException"> on encoding errors, or if this parameter object
        ''' has not been initialized. </exception>
        Public ReadOnly Property encoded As SByte()
            Get
                If Me.initialized = False Then Throw New IOException("not initialized")
                Return paramSpi.engineGetEncoded()
            End Get
        End Property

        ''' <summary>
        ''' Returns the parameters encoded in the specified scheme.
        ''' If {@code format} is null, the
        ''' primary encoding format for parameters is used. The primary encoding
        ''' format is ASN.1, if an ASN.1 specification for these parameters
        ''' exists.
        ''' </summary>
        ''' <param name="format"> the name of the encoding format.
        ''' </param>
        ''' <returns> the parameters encoded using the specified encoding scheme.
        ''' </returns>
        ''' <exception cref="IOException"> on encoding errors, or if this parameter object
        ''' has not been initialized. </exception>
        Public Function getEncoded(ByVal format As String) As SByte()
            If Me.initialized = False Then Throw New IOException("not initialized")
            Return paramSpi.engineGetEncoded(format)
        End Function

        ''' <summary>
        ''' Returns a formatted string describing the parameters.
        ''' </summary>
        ''' <returns> a formatted string describing the parameters, or null if this
        ''' parameter object has not been initialized. </returns>
        Public NotOverridable Overrides Function ToString() As String
            If Me.initialized = False Then Return Nothing
            Return paramSpi.engineToString()
        End Function
    End Class

End Namespace