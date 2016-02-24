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
	''' for the <seealso cref="CertPathBuilder CertPathBuilder"/> class. All
	''' {@code CertPathBuilder} implementations must include a class (the
	''' SPI class) that extends this class ({@code CertPathBuilderSpi}) and
	''' implements all of its methods. In general, instances of this class should
	''' only be accessed through the {@code CertPathBuilder} class. For
	''' details, see the Java Cryptography Architecture.
	''' <p>
	''' <b>Concurrent Access</b>
	''' <p>
	''' Instances of this class need not be protected against concurrent
	''' access from multiple threads. Threads that need to access a single
	''' {@code CertPathBuilderSpi} instance concurrently should synchronize
	''' amongst themselves and provide the necessary locking before calling the
	''' wrapping {@code CertPathBuilder} object.
	''' <p>
	''' However, implementations of {@code CertPathBuilderSpi} may still
	''' encounter concurrency issues, since multiple threads each
	''' manipulating a different {@code CertPathBuilderSpi} instance need not
	''' synchronize.
	''' 
	''' @since       1.4
	''' @author      Sean Mullan
	''' </summary>
	Public MustInherit Class CertPathBuilderSpi

		''' <summary>
		''' The default constructor.
		''' </summary>
		Public Sub New()
		End Sub

		''' <summary>
		''' Attempts to build a certification path using the specified
		''' algorithm parameter set.
		''' </summary>
		''' <param name="params"> the algorithm parameters </param>
		''' <returns> the result of the build algorithm </returns>
		''' <exception cref="CertPathBuilderException"> if the builder is unable to construct
		''' a certification path that satisfies the specified parameters </exception>
		''' <exception cref="InvalidAlgorithmParameterException"> if the specified parameters
		''' are inappropriate for this {@code CertPathBuilder} </exception>
		Public MustOverride Function engineBuild(ByVal params As CertPathParameters) As CertPathBuilderResult

		''' <summary>
		''' Returns a {@code CertPathChecker} that this implementation uses to
		''' check the revocation status of certificates. A PKIX implementation
		''' returns objects of type {@code PKIXRevocationChecker}.
		''' 
		''' <p>The primary purpose of this method is to allow callers to specify
		''' additional input parameters and options specific to revocation checking.
		''' See the class description of {@code CertPathBuilder} for an example.
		''' 
		''' <p>This method was added to version 1.8 of the Java Platform Standard
		''' Edition. In order to maintain backwards compatibility with existing
		''' service providers, this method cannot be abstract and by default throws
		''' an {@code UnsupportedOperationException}.
		''' </summary>
		''' <returns> a {@code CertPathChecker} that this implementation uses to
		''' check the revocation status of certificates </returns>
		''' <exception cref="UnsupportedOperationException"> if this method is not supported
		''' @since 1.8 </exception>
		Public Overridable Function engineGetRevocationChecker() As CertPathChecker
			Throw New UnsupportedOperationException
		End Function
	End Class

End Namespace