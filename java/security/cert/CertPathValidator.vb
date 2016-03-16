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
	''' A class for validating certification paths (also known as certificate
	''' chains).
	''' <p>
	''' This class uses a provider-based architecture.
	''' To create a {@code CertPathValidator},
	''' call one of the static {@code getInstance} methods, passing in the
	''' algorithm name of the {@code CertPathValidator} desired and
	''' optionally the name of the provider desired.
	''' 
	''' <p>Once a {@code CertPathValidator} object has been created, it can
	''' be used to validate certification paths by calling the {@link #validate
	''' validate} method and passing it the {@code CertPath} to be validated
	''' and an algorithm-specific set of parameters. If successful, the result is
	''' returned in an object that implements the
	''' {@code CertPathValidatorResult} interface.
	''' 
	''' <p>The <seealso cref="#getRevocationChecker"/> method allows an application to specify
	''' additional algorithm-specific parameters and options used by the
	''' {@code CertPathValidator} when checking the revocation status of
	''' certificates. Here is an example demonstrating how it is used with the PKIX
	''' algorithm:
	''' 
	''' <pre>
	''' CertPathValidator cpv = CertPathValidator.getInstance("PKIX");
	''' PKIXRevocationChecker rc = (PKIXRevocationChecker)cpv.getRevocationChecker();
	''' rc.setOptions(EnumSet.of(Option.SOFT_FAIL));
	''' params.addCertPathChecker(rc);
	''' CertPathValidatorResult cpvr = cpv.validate(path, params);
	''' </pre>
	''' 
	''' <p>Every implementation of the Java platform is required to support the
	''' following standard {@code CertPathValidator} algorithm:
	''' <ul>
	''' <li>{@code PKIX}</li>
	''' </ul>
	''' This algorithm is described in the <a href=
	''' "{@docRoot}/../technotes/guides/security/StandardNames.html#CertPathValidator">
	''' CertPathValidator section</a> of the
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
	''' access a single {@code CertPathValidator} instance concurrently should
	''' synchronize amongst themselves and provide the necessary locking. Multiple
	''' threads each manipulating a different {@code CertPathValidator}
	''' instance need not synchronize.
	''' </summary>
	''' <seealso cref= CertPath
	''' 
	''' @since       1.4
	''' @author      Yassir Elley </seealso>
	Public Class CertPathValidator

	'    
	'     * Constant to lookup in the Security properties file to determine
	'     * the default certpathvalidator type. In the Security properties file,
	'     * the default certpathvalidator type is given as:
	'     * <pre>
	'     * certpathvalidator.type=PKIX
	'     * </pre>
	'     
		Private Const CPV_TYPE As String = "certpathvalidator.type"
		Private ReadOnly validatorSpi As CertPathValidatorSpi
		Private ReadOnly provider_Renamed As java.security.Provider
		Private ReadOnly algorithm As String

		''' <summary>
		''' Creates a {@code CertPathValidator} object of the given algorithm,
		''' and encapsulates the given provider implementation (SPI object) in it.
		''' </summary>
		''' <param name="validatorSpi"> the provider implementation </param>
		''' <param name="provider"> the provider </param>
		''' <param name="algorithm"> the algorithm name </param>
		Protected Friend Sub New(ByVal validatorSpi As CertPathValidatorSpi, ByVal provider_Renamed As java.security.Provider, ByVal algorithm As String)
			Me.validatorSpi = validatorSpi
			Me.provider_Renamed = provider_Renamed
			Me.algorithm = algorithm
		End Sub

		''' <summary>
		''' Returns a {@code CertPathValidator} object that implements the
		''' specified algorithm.
		''' 
		''' <p> This method traverses the list of registered security Providers,
		''' starting with the most preferred Provider.
		''' A new CertPathValidator object encapsulating the
		''' CertPathValidatorSpi implementation from the first
		''' Provider that supports the specified algorithm is returned.
		''' 
		''' <p> Note that the list of registered providers may be retrieved via
		''' the <seealso cref="Security#getProviders() Security.getProviders()"/> method.
		''' </summary>
		''' <param name="algorithm"> the name of the requested {@code CertPathValidator}
		'''  algorithm. See the CertPathValidator section in the <a href=
		'''  "{@docRoot}/../technotes/guides/security/StandardNames.html#CertPathValidator">
		''' Java Cryptography Architecture Standard Algorithm Name Documentation</a>
		''' for information about standard algorithm names.
		''' </param>
		''' <returns> a {@code CertPathValidator} object that implements the
		'''          specified algorithm.
		''' </returns>
		''' <exception cref="NoSuchAlgorithmException"> if no Provider supports a
		'''          CertPathValidatorSpi implementation for the
		'''          specified algorithm.
		''' </exception>
		''' <seealso cref= java.security.Provider </seealso>
		Public Shared Function getInstance(ByVal algorithm As String) As CertPathValidator
			Dim instance_Renamed As sun.security.jca.GetInstance.Instance = GetInstance.getInstance("CertPathValidator", GetType(CertPathValidatorSpi), algorithm)
			Return New CertPathValidator(CType(instance_Renamed.impl, CertPathValidatorSpi), instance_Renamed.provider, algorithm)
		End Function

		''' <summary>
		''' Returns a {@code CertPathValidator} object that implements the
		''' specified algorithm.
		''' 
		''' <p> A new CertPathValidator object encapsulating the
		''' CertPathValidatorSpi implementation from the specified provider
		''' is returned.  The specified provider must be registered
		''' in the security provider list.
		''' 
		''' <p> Note that the list of registered providers may be retrieved via
		''' the <seealso cref="Security#getProviders() Security.getProviders()"/> method.
		''' </summary>
		''' <param name="algorithm"> the name of the requested {@code CertPathValidator}
		'''  algorithm. See the CertPathValidator section in the <a href=
		'''  "{@docRoot}/../technotes/guides/security/StandardNames.html#CertPathValidator">
		''' Java Cryptography Architecture Standard Algorithm Name Documentation</a>
		''' for information about standard algorithm names.
		''' </param>
		''' <param name="provider"> the name of the provider.
		''' </param>
		''' <returns> a {@code CertPathValidator} object that implements the
		'''          specified algorithm.
		''' </returns>
		''' <exception cref="NoSuchAlgorithmException"> if a CertPathValidatorSpi
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
		Public Shared Function getInstance(ByVal algorithm As String, ByVal provider_Renamed As String) As CertPathValidator
			Dim instance_Renamed As sun.security.jca.GetInstance.Instance = GetInstance.getInstance("CertPathValidator", GetType(CertPathValidatorSpi), algorithm, provider_Renamed)
			Return New CertPathValidator(CType(instance_Renamed.impl, CertPathValidatorSpi), instance_Renamed.provider, algorithm)
		End Function

		''' <summary>
		''' Returns a {@code CertPathValidator} object that implements the
		''' specified algorithm.
		''' 
		''' <p> A new CertPathValidator object encapsulating the
		''' CertPathValidatorSpi implementation from the specified Provider
		''' object is returned.  Note that the specified Provider object
		''' does not have to be registered in the provider list.
		''' </summary>
		''' <param name="algorithm"> the name of the requested {@code CertPathValidator}
		''' algorithm. See the CertPathValidator section in the <a href=
		''' "{@docRoot}/../technotes/guides/security/StandardNames.html#CertPathValidator">
		''' Java Cryptography Architecture Standard Algorithm Name Documentation</a>
		''' for information about standard algorithm names.
		''' </param>
		''' <param name="provider"> the provider.
		''' </param>
		''' <returns> a {@code CertPathValidator} object that implements the
		'''          specified algorithm.
		''' </returns>
		''' <exception cref="NoSuchAlgorithmException"> if a CertPathValidatorSpi
		'''          implementation for the specified algorithm is not available
		'''          from the specified Provider object.
		''' </exception>
		''' <exception cref="IllegalArgumentException"> if the {@code provider} is
		'''          null.
		''' </exception>
		''' <seealso cref= java.security.Provider </seealso>
		Public Shared Function getInstance(ByVal algorithm As String, ByVal provider_Renamed As java.security.Provider) As CertPathValidator
			Dim instance_Renamed As sun.security.jca.GetInstance.Instance = GetInstance.getInstance("CertPathValidator", GetType(CertPathValidatorSpi), algorithm, provider_Renamed)
			Return New CertPathValidator(CType(instance_Renamed.impl, CertPathValidatorSpi), instance_Renamed.provider, algorithm)
		End Function

		''' <summary>
		''' Returns the {@code Provider} of this
		''' {@code CertPathValidator}.
		''' </summary>
		''' <returns> the {@code Provider} of this {@code CertPathValidator} </returns>
		Public Property provider As java.security.Provider
			Get
				Return Me.provider_Renamed
			End Get
		End Property

		''' <summary>
		''' Returns the algorithm name of this {@code CertPathValidator}.
		''' </summary>
		''' <returns> the algorithm name of this {@code CertPathValidator} </returns>
		Public Property algorithm As String
			Get
				Return Me.algorithm
			End Get
		End Property

		''' <summary>
		''' Validates the specified certification path using the specified
		''' algorithm parameter set.
		''' <p>
		''' The {@code CertPath} specified must be of a type that is
		''' supported by the validation algorithm, otherwise an
		''' {@code InvalidAlgorithmParameterException} will be thrown. For
		''' example, a {@code CertPathValidator} that implements the PKIX
		''' algorithm validates {@code CertPath} objects of type X.509.
		''' </summary>
		''' <param name="certPath"> the {@code CertPath} to be validated </param>
		''' <param name="params"> the algorithm parameters </param>
		''' <returns> the result of the validation algorithm </returns>
		''' <exception cref="CertPathValidatorException"> if the {@code CertPath}
		''' does not validate </exception>
		''' <exception cref="InvalidAlgorithmParameterException"> if the specified
		''' parameters or the type of the specified {@code CertPath} are
		''' inappropriate for this {@code CertPathValidator} </exception>
		Public Function validate(ByVal certPath As CertPath, ByVal params As CertPathParameters) As CertPathValidatorResult
			Return validatorSpi.engineValidate(certPath, params)
		End Function

        ''' <summary>
        ''' Returns the default {@code CertPathValidator} type as specified by
        ''' the {@code certpathvalidator.type} security property, or the string
        ''' {@literal "PKIX"} if no such property exists.
        ''' 
        ''' <p>The default {@code CertPathValidator} type can be used by
        ''' applications that do not want to use a hard-coded type when calling one
        ''' of the {@code getInstance} methods, and want to provide a default
        ''' type in case a user does not specify its own.
        ''' 
        ''' <p>The default {@code CertPathValidator} type can be changed by
        ''' setting the value of the {@code certpathvalidator.type} security
        ''' property to the desired type.
        ''' </summary>
        ''' <seealso cref= java.security.Security security properties </seealso>
        ''' <returns> the default {@code CertPathValidator} type as specified
        ''' by the {@code certpathvalidator.type} security property, or the string
        ''' {@literal "PKIX"} if no such property exists. </returns>
        Public Shared ReadOnly Property defaultType As String
            Get
                Dim cpvtype As String = java.security.AccessController.doPrivileged(New PrivilegedActionAnonymousInnerClassHelper(Of T))
                Return If(cpvtype Is Nothing, "PKIX", cpvtype)
            End Get
        End Property

        Private Class PrivilegedActionAnonymousInnerClassHelper(Of T)
			Implements java.security.PrivilegedAction(Of T)

			Public Overridable Function run() As String
				Return java.security.Security.getProperty(CPV_TYPE)
			End Function
		End Class

		''' <summary>
		''' Returns a {@code CertPathChecker} that the encapsulated
		''' {@code CertPathValidatorSpi} implementation uses to check the revocation
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
				Return validatorSpi.engineGetRevocationChecker()
			End Get
		End Property
	End Class

End Namespace