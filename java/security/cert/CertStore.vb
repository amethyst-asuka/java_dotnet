Imports System.Collections.Generic
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
	''' A class for retrieving {@code Certificate}s and {@code CRL}s
	''' from a repository.
	''' <p>
	''' This class uses a provider-based architecture.
	''' To create a {@code CertStore}, call one of the static
	''' {@code getInstance} methods, passing in the type of
	''' {@code CertStore} desired, any applicable initialization parameters
	''' and optionally the name of the provider desired.
	''' <p>
	''' Once the {@code CertStore} has been created, it can be used to
	''' retrieve {@code Certificate}s and {@code CRL}s by calling its
	''' <seealso cref="#getCertificates(CertSelector selector) getCertificates"/> and
	''' <seealso cref="#getCRLs(CRLSelector selector) getCRLs"/> methods.
	''' <p>
	''' Unlike a <seealso cref="java.security.KeyStore KeyStore"/>, which provides access
	''' to a cache of private keys and trusted certificates, a
	''' {@code CertStore} is designed to provide access to a potentially
	''' vast repository of untrusted certificates and CRLs. For example, an LDAP
	''' implementation of {@code CertStore} provides access to certificates
	''' and CRLs stored in one or more directories using the LDAP protocol and the
	''' schema as defined in the RFC service attribute.
	''' 
	''' <p> Every implementation of the Java platform is required to support the
	''' following standard {@code CertStore} type:
	''' <ul>
	''' <li>{@code Collection}</li>
	''' </ul>
	''' This type is described in the <a href=
	''' "{@docRoot}/../technotes/guides/security/StandardNames.html#CertStore">
	''' CertStore section</a> of the
	''' Java Cryptography Architecture Standard Algorithm Name Documentation.
	''' Consult the release documentation for your implementation to see if any
	''' other types are supported.
	''' 
	''' <p>
	''' <b>Concurrent Access</b>
	''' <p>
	''' All public methods of {@code CertStore} objects must be thread-safe.
	''' That is, multiple threads may concurrently invoke these methods on a
	''' single {@code CertStore} object (or more than one) with no
	''' ill effects. This allows a {@code CertPathBuilder} to search for a
	''' CRL while simultaneously searching for further certificates, for instance.
	''' <p>
	''' The static methods of this class are also guaranteed to be thread-safe.
	''' Multiple threads may concurrently invoke the static methods defined in
	''' this class with no ill effects.
	''' 
	''' @since       1.4
	''' @author      Sean Mullan, Steve Hanna
	''' </summary>
	Public Class CertStore
	'    
	'     * Constant to lookup in the Security properties file to determine
	'     * the default certstore type. In the Security properties file, the
	'     * default certstore type is given as:
	'     * <pre>
	'     * certstore.type=LDAP
	'     * </pre>
	'     
		Private Const CERTSTORE_TYPE As String = "certstore.type"
		Private storeSpi As CertStoreSpi
		Private provider_Renamed As java.security.Provider
		Private type As String
		Private params As CertStoreParameters

		''' <summary>
		''' Creates a {@code CertStore} object of the given type, and
		''' encapsulates the given provider implementation (SPI object) in it.
		''' </summary>
		''' <param name="storeSpi"> the provider implementation </param>
		''' <param name="provider"> the provider </param>
		''' <param name="type"> the type </param>
		''' <param name="params"> the initialization parameters (may be {@code null}) </param>
		Protected Friend Sub New(ByVal storeSpi As CertStoreSpi, ByVal provider_Renamed As java.security.Provider, ByVal type As String, ByVal params As CertStoreParameters)
			Me.storeSpi = storeSpi
			Me.provider_Renamed = provider_Renamed
			Me.type = type
			If params IsNot Nothing Then Me.params = CType(params.clone(), CertStoreParameters)
		End Sub

		''' <summary>
		''' Returns a {@code Collection} of {@code Certificate}s that
		''' match the specified selector. If no {@code Certificate}s
		''' match the selector, an empty {@code Collection} will be returned.
		''' <p>
		''' For some {@code CertStore} types, the resulting
		''' {@code Collection} may not contain <b>all</b> of the
		''' {@code Certificate}s that match the selector. For instance,
		''' an LDAP {@code CertStore} may not search all entries in the
		''' directory. Instead, it may just search entries that are likely to
		''' contain the {@code Certificate}s it is looking for.
		''' <p>
		''' Some {@code CertStore} implementations (especially LDAP
		''' {@code CertStore}s) may throw a {@code CertStoreException}
		''' unless a non-null {@code CertSelector} is provided that
		''' includes specific criteria that can be used to find the certificates.
		''' Issuer and/or subject names are especially useful criteria.
		''' </summary>
		''' <param name="selector"> A {@code CertSelector} used to select which
		'''  {@code Certificate}s should be returned. Specify {@code null}
		'''  to return all {@code Certificate}s (if supported). </param>
		''' <returns> A {@code Collection} of {@code Certificate}s that
		'''         match the specified selector (never {@code null}) </returns>
		''' <exception cref="CertStoreException"> if an exception occurs </exception>
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
		Public Function getCertificates(ByVal selector As CertSelector) As ICollection(Of ? As Certificate)
			Return storeSpi.engineGetCertificates(selector)
		End Function

		''' <summary>
		''' Returns a {@code Collection} of {@code CRL}s that
		''' match the specified selector. If no {@code CRL}s
		''' match the selector, an empty {@code Collection} will be returned.
		''' <p>
		''' For some {@code CertStore} types, the resulting
		''' {@code Collection} may not contain <b>all</b> of the
		''' {@code CRL}s that match the selector. For instance,
		''' an LDAP {@code CertStore} may not search all entries in the
		''' directory. Instead, it may just search entries that are likely to
		''' contain the {@code CRL}s it is looking for.
		''' <p>
		''' Some {@code CertStore} implementations (especially LDAP
		''' {@code CertStore}s) may throw a {@code CertStoreException}
		''' unless a non-null {@code CRLSelector} is provided that
		''' includes specific criteria that can be used to find the CRLs.
		''' Issuer names and/or the certificate to be checked are especially useful.
		''' </summary>
		''' <param name="selector"> A {@code CRLSelector} used to select which
		'''  {@code CRL}s should be returned. Specify {@code null}
		'''  to return all {@code CRL}s (if supported). </param>
		''' <returns> A {@code Collection} of {@code CRL}s that
		'''         match the specified selector (never {@code null}) </returns>
		''' <exception cref="CertStoreException"> if an exception occurs </exception>
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
		Public Function getCRLs(ByVal selector As CRLSelector) As ICollection(Of ? As CRL)
			Return storeSpi.engineGetCRLs(selector)
		End Function

		''' <summary>
		''' Returns a {@code CertStore} object that implements the specified
		''' {@code CertStore} type and is initialized with the specified
		''' parameters.
		''' 
		''' <p> This method traverses the list of registered security Providers,
		''' starting with the most preferred Provider.
		''' A new CertStore object encapsulating the
		''' CertStoreSpi implementation from the first
		''' Provider that supports the specified type is returned.
		''' 
		''' <p> Note that the list of registered providers may be retrieved via
		''' the <seealso cref="Security#getProviders() Security.getProviders()"/> method.
		''' 
		''' <p>The {@code CertStore} that is returned is initialized with the
		''' specified {@code CertStoreParameters}. The type of parameters
		''' needed may vary between different types of {@code CertStore}s.
		''' Note that the specified {@code CertStoreParameters} object is
		''' cloned.
		''' </summary>
		''' <param name="type"> the name of the requested {@code CertStore} type.
		''' See the CertStore section in the <a href=
		''' "{@docRoot}/../technotes/guides/security/StandardNames.html#CertStore">
		''' Java Cryptography Architecture Standard Algorithm Name Documentation</a>
		''' for information about standard types.
		''' </param>
		''' <param name="params"> the initialization parameters (may be {@code null}).
		''' </param>
		''' <returns> a {@code CertStore} object that implements the specified
		'''          {@code CertStore} type.
		''' </returns>
		''' <exception cref="NoSuchAlgorithmException"> if no Provider supports a
		'''          CertStoreSpi implementation for the specified type.
		''' </exception>
		''' <exception cref="InvalidAlgorithmParameterException"> if the specified
		'''          initialization parameters are inappropriate for this
		'''          {@code CertStore}.
		''' </exception>
		''' <seealso cref= java.security.Provider </seealso>
		Public Shared Function getInstance(ByVal type As String, ByVal params As CertStoreParameters) As CertStore
			Try
				Dim instance_Renamed As sun.security.jca.GetInstance.Instance = GetInstance.getInstance("CertStore", GetType(CertStoreSpi), type, params)
				Return New CertStore(CType(instance_Renamed.impl, CertStoreSpi), instance_Renamed.provider, type, params)
			Catch e As java.security.NoSuchAlgorithmException
				Return handleException(e)
			End Try
		End Function

		Private Shared Function handleException(ByVal e As java.security.NoSuchAlgorithmException) As CertStore
			Dim cause As Throwable = e.InnerException
			If TypeOf cause Is java.security.InvalidAlgorithmParameterException Then Throw CType(cause, java.security.InvalidAlgorithmParameterException)
			Throw e
		End Function

		''' <summary>
		''' Returns a {@code CertStore} object that implements the specified
		''' {@code CertStore} type.
		''' 
		''' <p> A new CertStore object encapsulating the
		''' CertStoreSpi implementation from the specified provider
		''' is returned.  The specified provider must be registered
		''' in the security provider list.
		''' 
		''' <p> Note that the list of registered providers may be retrieved via
		''' the <seealso cref="Security#getProviders() Security.getProviders()"/> method.
		''' 
		''' <p>The {@code CertStore} that is returned is initialized with the
		''' specified {@code CertStoreParameters}. The type of parameters
		''' needed may vary between different types of {@code CertStore}s.
		''' Note that the specified {@code CertStoreParameters} object is
		''' cloned.
		''' </summary>
		''' <param name="type"> the requested {@code CertStore} type.
		''' See the CertStore section in the <a href=
		''' "{@docRoot}/../technotes/guides/security/StandardNames.html#CertStore">
		''' Java Cryptography Architecture Standard Algorithm Name Documentation</a>
		''' for information about standard types.
		''' </param>
		''' <param name="params"> the initialization parameters (may be {@code null}).
		''' </param>
		''' <param name="provider"> the name of the provider.
		''' </param>
		''' <returns> a {@code CertStore} object that implements the
		'''          specified type.
		''' </returns>
		''' <exception cref="NoSuchAlgorithmException"> if a CertStoreSpi
		'''          implementation for the specified type is not
		'''          available from the specified provider.
		''' </exception>
		''' <exception cref="InvalidAlgorithmParameterException"> if the specified
		'''          initialization parameters are inappropriate for this
		'''          {@code CertStore}.
		''' </exception>
		''' <exception cref="NoSuchProviderException"> if the specified provider is not
		'''          registered in the security provider list.
		''' </exception>
		''' <exception cref="IllegalArgumentException"> if the {@code provider} is
		'''          null or empty.
		''' </exception>
		''' <seealso cref= java.security.Provider </seealso>
		Public Shared Function getInstance(ByVal type As String, ByVal params As CertStoreParameters, ByVal provider_Renamed As String) As CertStore
			Try
				Dim instance_Renamed As sun.security.jca.GetInstance.Instance = GetInstance.getInstance("CertStore", GetType(CertStoreSpi), type, params, provider_Renamed)
				Return New CertStore(CType(instance_Renamed.impl, CertStoreSpi), instance_Renamed.provider, type, params)
			Catch e As java.security.NoSuchAlgorithmException
				Return handleException(e)
			End Try
		End Function

		''' <summary>
		''' Returns a {@code CertStore} object that implements the specified
		''' {@code CertStore} type.
		''' 
		''' <p> A new CertStore object encapsulating the
		''' CertStoreSpi implementation from the specified Provider
		''' object is returned.  Note that the specified Provider object
		''' does not have to be registered in the provider list.
		''' 
		''' <p>The {@code CertStore} that is returned is initialized with the
		''' specified {@code CertStoreParameters}. The type of parameters
		''' needed may vary between different types of {@code CertStore}s.
		''' Note that the specified {@code CertStoreParameters} object is
		''' cloned.
		''' </summary>
		''' <param name="type"> the requested {@code CertStore} type.
		''' See the CertStore section in the <a href=
		''' "{@docRoot}/../technotes/guides/security/StandardNames.html#CertStore">
		''' Java Cryptography Architecture Standard Algorithm Name Documentation</a>
		''' for information about standard types.
		''' </param>
		''' <param name="params"> the initialization parameters (may be {@code null}).
		''' </param>
		''' <param name="provider"> the provider.
		''' </param>
		''' <returns> a {@code CertStore} object that implements the
		'''          specified type.
		''' </returns>
		''' <exception cref="NoSuchAlgorithmException"> if a CertStoreSpi
		'''          implementation for the specified type is not available
		'''          from the specified Provider object.
		''' </exception>
		''' <exception cref="InvalidAlgorithmParameterException"> if the specified
		'''          initialization parameters are inappropriate for this
		'''          {@code CertStore}
		''' </exception>
		''' <exception cref="IllegalArgumentException"> if the {@code provider} is
		'''          null.
		''' </exception>
		''' <seealso cref= java.security.Provider </seealso>
		Public Shared Function getInstance(ByVal type As String, ByVal params As CertStoreParameters, ByVal provider_Renamed As java.security.Provider) As CertStore
			Try
				Dim instance_Renamed As sun.security.jca.GetInstance.Instance = GetInstance.getInstance("CertStore", GetType(CertStoreSpi), type, params, provider_Renamed)
				Return New CertStore(CType(instance_Renamed.impl, CertStoreSpi), instance_Renamed.provider, type, params)
			Catch e As java.security.NoSuchAlgorithmException
				Return handleException(e)
			End Try
		End Function

		''' <summary>
		''' Returns the parameters used to initialize this {@code CertStore}.
		''' Note that the {@code CertStoreParameters} object is cloned before
		''' it is returned.
		''' </summary>
		''' <returns> the parameters used to initialize this {@code CertStore}
		''' (may be {@code null}) </returns>
		Public Property certStoreParameters As CertStoreParameters
			Get
				Return (If(params Is Nothing, Nothing, CType(params.clone(), CertStoreParameters)))
			End Get
		End Property

		''' <summary>
		''' Returns the type of this {@code CertStore}.
		''' </summary>
		''' <returns> the type of this {@code CertStore} </returns>
		Public Property type As String
			Get
				Return Me.type
			End Get
		End Property

		''' <summary>
		''' Returns the provider of this {@code CertStore}.
		''' </summary>
		''' <returns> the provider of this {@code CertStore} </returns>
		Public Property provider As java.security.Provider
			Get
				Return Me.provider_Renamed
			End Get
		End Property

		''' <summary>
		''' Returns the default {@code CertStore} type as specified by the
		''' {@code certstore.type} security property, or the string
		''' {@literal "LDAP"} if no such property exists.
		''' 
		''' <p>The default {@code CertStore} type can be used by applications
		''' that do not want to use a hard-coded type when calling one of the
		''' {@code getInstance} methods, and want to provide a default
		''' {@code CertStore} type in case a user does not specify its own.
		''' 
		''' <p>The default {@code CertStore} type can be changed by setting
		''' the value of the {@code certstore.type} security property to the
		''' desired type.
		''' </summary>
		''' <seealso cref= java.security.Security security properties </seealso>
		''' <returns> the default {@code CertStore} type as specified by the
		''' {@code certstore.type} security property, or the string
		''' {@literal "LDAP"} if no such property exists. </returns>
		Public Property Shared defaultType As String
			Get
				Dim cstype As String
				cstype = java.security.AccessController.doPrivileged(New PrivilegedActionAnonymousInnerClassHelper(Of T)
				If cstype Is Nothing Then cstype = "LDAP"
				Return cstype
			End Get
		End Property

		Private Class PrivilegedActionAnonymousInnerClassHelper(Of T)
			Implements java.security.PrivilegedAction(Of T)

			Public Overridable Function run() As String
				Return java.security.Security.getProperty(CERTSTORE_TYPE)
			End Function
		End Class
	End Class

End Namespace