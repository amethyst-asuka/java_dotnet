Imports System.Collections.Generic

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
	''' The <i>Service Provider Interface</i> (<b>SPI</b>)
	''' for the <seealso cref="CertStore CertStore"/> class. All {@code CertStore}
	''' implementations must include a class (the SPI [Class]) that extends
	''' this class ({@code CertStoreSpi}), provides a constructor with
	''' a single argument of type {@code CertStoreParameters}, and implements
	''' all of its methods. In general, instances of this class should only be
	''' accessed through the {@code CertStore} class.
	''' For details, see the Java Cryptography Architecture.
	''' <p>
	''' <b>Concurrent Access</b>
	''' <p>
	''' The public methods of all {@code CertStoreSpi} objects must be
	''' thread-safe. That is, multiple threads may concurrently invoke these
	''' methods on a single {@code CertStoreSpi} object (or more than one)
	''' with no ill effects. This allows a {@code CertPathBuilder} to search
	''' for a CRL while simultaneously searching for further certificates, for
	''' instance.
	''' <p>
	''' Simple {@code CertStoreSpi} implementations will probably ensure
	''' thread safety by adding a {@code synchronized} keyword to their
	''' {@code engineGetCertificates} and {@code engineGetCRLs} methods.
	''' More sophisticated ones may allow truly concurrent access.
	''' 
	''' @since       1.4
	''' @author      Steve Hanna
	''' </summary>
	Public MustInherit Class CertStoreSpi

		''' <summary>
		''' The sole constructor.
		''' </summary>
		''' <param name="params"> the initialization parameters (may be {@code null}) </param>
		''' <exception cref="InvalidAlgorithmParameterException"> if the initialization
		''' parameters are inappropriate for this {@code CertStoreSpi} </exception>
		Public Sub New(  params As CertStoreParameters)
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
		''' unless a non-null {@code CertSelector} is provided that includes
		''' specific criteria that can be used to find the certificates. Issuer
		''' and/or subject names are especially useful criteria.
		''' </summary>
		''' <param name="selector"> A {@code CertSelector} used to select which
		'''  {@code Certificate}s should be returned. Specify {@code null}
		'''  to return all {@code Certificate}s (if supported). </param>
		''' <returns> A {@code Collection} of {@code Certificate}s that
		'''         match the specified selector (never {@code null}) </returns>
		''' <exception cref="CertStoreException"> if an exception occurs </exception>
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
		Public MustOverride Function engineGetCertificates(  selector As CertSelector) As ICollection(Of ? As Certificate)

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
		''' unless a non-null {@code CRLSelector} is provided that includes
		''' specific criteria that can be used to find the CRLs. Issuer names
		''' and/or the certificate to be checked are especially useful.
		''' </summary>
		''' <param name="selector"> A {@code CRLSelector} used to select which
		'''  {@code CRL}s should be returned. Specify {@code null}
		'''  to return all {@code CRL}s (if supported). </param>
		''' <returns> A {@code Collection} of {@code CRL}s that
		'''         match the specified selector (never {@code null}) </returns>
		''' <exception cref="CertStoreException"> if an exception occurs </exception>
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
		Public MustOverride Function engineGetCRLs(  selector As CRLSelector) As ICollection(Of ? As CRL)
	End Class

End Namespace