Imports sun.security.jca

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

Namespace java.security.cert



	''' <summary>
	''' A class for building certification paths (also known as certificate chains).
	''' <p>
	''' This class uses a provider-based architecture.
	''' To create a {@code CertPathBuilder}, call
	''' one of the static {@code getInstance} methods, passing in the
	''' algorithm name of the {@code CertPathBuilder} desired and optionally
	''' the name of the provider desired.
	''' 
	''' <p>Once a {@code CertPathBuilder} object has been created, certification
	''' paths can be constructed by calling the <seealso cref="#build build"/> method and
	''' passing it an algorithm-specific set of parameters. If successful, the
	''' result (including the {@code CertPath} that was built) is returned
	''' in an object that implements the {@code CertPathBuilderResult}
	''' interface.
	''' 
	''' <p>The <seealso cref="#getRevocationChecker"/> method allows an application to specify
	''' additional algorithm-specific parameters and options used by the
	''' {@code CertPathBuilder} when checking the revocation status of certificates.
	''' Here is an example demonstrating how it is used with the PKIX algorithm:
	''' 
	''' <pre>
	''' CertPathBuilder cpb = CertPathBuilder.getInstance("PKIX");
	''' PKIXRevocationChecker rc = (PKIXRevocationChecker)cpb.getRevocationChecker();
	''' rc.setOptions(EnumSet.of(Option.PREFER_CRLS));
	''' params.addCertPathChecker(rc);
	''' CertPathBuilderResult cpbr = cpb.build(params);
	''' </pre>
	''' 
	''' <p>Every implementation of the Java platform is required to support the
	''' following standard {@code CertPathBuilder} algorithm:
	''' <ul>
	''' <li>{@code PKIX}</li>
	''' </ul>
	''' This algorithm is described in the <a href=
	''' "{@docRoot}/../technotes/guides/security/StandardNames.html#CertPathBuilder">
	''' CertPathBuilder section</a> of the
	''' Java Cryptography Architecture Standard Algorithm Name Documentation.
	''' Consult the release documentation for your implementation to see if any
	''' other algorithms are supported.
	''' 
	''' <p>
	''' <b>Concurrent Access</b>
	''' <p>
	''' The static methods of this class are guaranteed to be thread-safe.
	''' Multiple threads may concurrently invoke the static methods defined in
	''' this class with no ill effects.
	''' <p>
	''' However, this is not true for the non-static methods defined by this class.
	''' Unless otherwise documented by a specific provider, threads that need to
	''' access a single {@code CertPathBuilder} instance concurrently should
	''' synchronize amongst themselves and provide the necessary locking. Multiple
	''' threads each manipulating a different {@code CertPathBuilder} instance
	''' need not synchronize.
	''' </summary>
	''' <seealso cref= CertPath
	''' 
	''' @since       1.4
	''' @author      Sean Mullan
	''' @author      Yassir Elley </seealso>
	Public Class CertPathBuilder

	'    
	'     * Constant to lookup in the Security properties file to determine
	'     * the default certpathbuilder type. In the Security properties file,
	'     * the default certpathbuilder type is given as:
	'     * <pre>
	'     * certpathbuilder.type=PKIX
	'     * </pre>
	'     
		Private Const CPB_TYPE As String = "certpathbuilder.type"
		Private ReadOnly builderSpi As CertPathBuilderSpi
		Private ReadOnly provider_Renamed As java.security.Provider
		Private ReadOnly algorithm As String

		''' <summary>
		''' Creates a {@code CertPathBuilder} object of the given algorithm,
		''' and encapsulates the given provider implementation (SPI object) in it.
		''' </summary>
		''' <param name="builderSpi"> the provider implementation </param>
		''' <param name="provider"> the provider </param>
		''' <param name="algorithm"> the algorithm name </param>
		Protected Friend Sub New(  builderSpi As CertPathBuilderSpi,   provider_Renamed As java.security.Provider,   algorithm As String)
			Me.builderSpi = builderSpi
			Me.provider_Renamed = provider_Renamed
			Me.algorithm = algorithm
		End Sub

		''' <summary>
		''' Returns a {@code CertPathBuilder} object that implements the
		''' specified algorithm.
		''' 
		''' <p> This method traverses the list of registered security Providers,
		''' starting with the most preferred Provider.
		''' A new CertPathBuilder object encapsulating the
		''' CertPathBuilderSpi implementation from the first
		''' Provider that supports the specified algorithm is returned.
		''' 
		''' <p> Note that the list of registered providers may be retrieved via
		''' the <seealso cref="Security#getProviders() Security.getProviders()"/> method.
		''' </summary>
		''' <param name="algorithm"> the name of the requested {@code CertPathBuilder}
		'''  algorithm.  See the CertPathBuilder section in the <a href=
		'''  "{@docRoot}/../technotes/guides/security/StandardNames.html#CertPathBuilder">
		''' Java Cryptography Architecture Standard Algorithm Name Documentation</a>
		''' for information about standard algorithm names.
		''' </param>
		''' <returns> a {@code CertPathBuilder} object that implements the
		'''          specified algorithm.
		''' </returns>
		''' <exception cref="NoSuchAlgorithmException"> if no Provider supports a
		'''          CertPathBuilderSpi implementation for the
		'''          specified algorithm.
		''' </exception>
		''' <seealso cref= java.security.Provider </seealso>
		Public Shared Function getInstance(  algorithm As String) As CertPathBuilder
			Dim instance_Renamed As sun.security.jca.GetInstance.Instance = GetInstance.getInstance("CertPathBuilder", GetType(CertPathBuilderSpi), algorithm)
			Return New CertPathBuilder(CType(instance_Renamed.impl, CertPathBuilderSpi), instance_Renamed.provider, algorithm)
		End Function

		''' <summary>
		''' Returns a {@code CertPathBuilder} object that implements the
		''' specified algorithm.
		''' 
		''' <p> A new CertPathBuilder object encapsulating the
		''' CertPathBuilderSpi implementation from the specified provider
		''' is returned.  The specified provider must be registered
		''' in the security provider list.
		''' 
		''' <p> Note that the list of registered providers may be retrieved via
		''' the <seealso cref="Security#getProviders() Security.getProviders()"/> method.
		''' </summary>
		''' <param name="algorithm"> the name of the requested {@code CertPathBuilder}
		'''  algorithm.  See the CertPathBuilder section in the <a href=
		'''  "{@docRoot}/../technotes/guides/security/StandardNames.html#CertPathBuilder">
		''' Java Cryptography Architecture Standard Algorithm Name Documentation</a>
		''' for information about standard algorithm names.
		''' </param>
		''' <param name="provider"> the name of the provider.
		''' </param>
		''' <returns> a {@code CertPathBuilder} object that implements the
		'''          specified algorithm.
		''' </returns>
		''' <exception cref="NoSuchAlgorithmException"> if a CertPathBuilderSpi
		'''          implementation for the specified algorithm is not
		'''          available from the specified provider.
		''' </exception>
		''' <exception cref="NoSuchProviderException"> if the specified provider is not
		'''          registered in the security provider list.
		''' </exception>
		''' <exception cref="IllegalArgumentException"> if the {@code provider} is
		'''          null or empty.
		''' </exception>
		''' <seealso cref= java.security.Provider </seealso>
		Public Shared Function getInstance(  algorithm As String,   provider_Renamed As String) As CertPathBuilder
			Dim instance_Renamed As sun.security.jca.GetInstance.Instance = GetInstance.getInstance("CertPathBuilder", GetType(CertPathBuilderSpi), algorithm, provider_Renamed)
			Return New CertPathBuilder(CType(instance_Renamed.impl, CertPathBuilderSpi), instance_Renamed.provider, algorithm)
		End Function

		''' <summary>
		''' Returns a {@code CertPathBuilder} object that implements the
		''' specified algorithm.
		''' 
		''' <p> A new CertPathBuilder object encapsulating the
		''' CertPathBuilderSpi implementation from the specified Provider
		''' object is returned.  Note that the specified Provider object
		''' does not have to be registered in the provider list.
		''' </summary>
		''' <param name="algorithm"> the name of the requested {@code CertPathBuilder}
		'''  algorithm.  See the CertPathBuilder section in the <a href=
		'''  "{@docRoot}/../technotes/guides/security/StandardNames.html#CertPathBuilder">
		''' Java Cryptography Architecture Standard Algorithm Name Documentation</a>
		''' for information about standard algorithm names.
		''' </param>
		''' <param name="provider"> the provider.
		''' </param>
		''' <returns> a {@code CertPathBuilder} object that implements the
		'''          specified algorithm.
		''' </returns>
		''' <exception cref="NoSuchAlgorithmException"> if a CertPathBuilderSpi
		'''          implementation for the specified algorithm is not available
		'''          from the specified Provider object.
		''' </exception>
		''' <exception cref="IllegalArgumentException"> if the {@code provider} is
		'''          null.
		''' </exception>
		''' <seealso cref= java.security.Provider </seealso>
		Public Shared Function getInstance(  algorithm As String,   provider_Renamed As java.security.Provider) As CertPathBuilder
			Dim instance_Renamed As sun.security.jca.GetInstance.Instance = GetInstance.getInstance("CertPathBuilder", GetType(CertPathBuilderSpi), algorithm, provider_Renamed)
			Return New CertPathBuilder(CType(instance_Renamed.impl, CertPathBuilderSpi), instance_Renamed.provider, algorithm)
		End Function

		''' <summary>
		''' Returns the provider of this {@code CertPathBuilder}.
		''' </summary>
		''' <returns> the provider of this {@code CertPathBuilder} </returns>
		Public Property provider As java.security.Provider
			Get
				Return Me.provider_Renamed
			End Get
		End Property

		''' <summary>
		''' Returns the name of the algorithm of this {@code CertPathBuilder}.
		''' </summary>
		''' <returns> the name of the algorithm of this {@code CertPathBuilder} </returns>
		Public Property algorithm As String
			Get
				Return Me.algorithm
			End Get
		End Property

		''' <summary>
		''' Attempts to build a certification path using the specified algorithm
		''' parameter set.
		''' </summary>
		''' <param name="params"> the algorithm parameters </param>
		''' <returns> the result of the build algorithm </returns>
		''' <exception cref="CertPathBuilderException"> if the builder is unable to construct
		'''  a certification path that satisfies the specified parameters </exception>
		''' <exception cref="InvalidAlgorithmParameterException"> if the specified parameters
		''' are inappropriate for this {@code CertPathBuilder} </exception>
		Public Function build(  params As CertPathParameters) As CertPathBuilderResult
			Return builderSpi.engineBuild(params)
		End Function

		''' <summary>
		''' Returns the default {@code CertPathBuilder} type as specified by
		''' the {@code certpathbuilder.type} security property, or the string
		''' {@literal "PKIX"} if no such property exists.
		''' 
		''' <p>The default {@code CertPathBuilder} type can be used by
		''' applications that do not want to use a hard-coded type when calling one
		''' of the {@code getInstance} methods, and want to provide a default
		''' type in case a user does not specify its own.
		''' 
		''' <p>The default {@code CertPathBuilder} type can be changed by
		''' setting the value of the {@code certpathbuilder.type} security property
		''' to the desired type.
		''' </summary>
		''' <seealso cref= java.security.Security security properties </seealso>
		''' <returns> the default {@code CertPathBuilder} type as specified
		''' by the {@code certpathbuilder.type} security property, or the string
		''' {@literal "PKIX"} if no such property exists. </returns>
		PublicShared ReadOnly PropertydefaultType As String
			Get
				Dim cpbtype As String = java.security.AccessController.doPrivileged(New PrivilegedActionAnonymousInnerClassHelper(Of T)
				Return If(cpbtype Is Nothing, "PKIX", cpbtype)
			End Get
		End Property

		Private Class PrivilegedActionAnonymousInnerClassHelper(Of T)
			Implements java.security.PrivilegedAction(Of T)

			Public Overridable Function run() As String
				Return java.security.Security.getProperty(CPB_TYPE)
			End Function
		End Class

		''' <summary>
		''' Returns a {@code CertPathChecker} that the encapsulated
		''' {@code CertPathBuilderSpi} implementation uses to check the revocation
		''' status of certificates. A PKIX implementation returns objects of
		''' type {@code PKIXRevocationChecker}. Each invocation of this method
		''' returns a new instance of {@code CertPathChecker}.
		''' 
		''' <p>The primary purpose of this method is to allow callers to specify
		''' additional input parameters and options specific to revocation checking.
		''' See the class description for an example.
		''' </summary>
		''' <returns> a {@code CertPathChecker} </returns>
		''' <exception cref="UnsupportedOperationException"> if the service provider does not
		'''         support this method
		''' @since 1.8 </exception>
		Public Property revocationChecker As CertPathChecker
			Get
				Return builderSpi.engineGetRevocationChecker()
			End Get
		End Property
	End Class

End Namespace