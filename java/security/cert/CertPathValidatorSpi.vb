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


	''' 
	''' <summary>
	''' The <i>Service Provider Interface</i> (<b>SPI</b>)
	''' for the <seealso cref="CertPathValidator CertPathValidator"/> class. All
	''' {@code CertPathValidator} implementations must include a class (the
	''' SPI [Class]) that extends this class ({@code CertPathValidatorSpi})
	''' and implements all of its methods. In general, instances of this class
	''' should only be accessed through the {@code CertPathValidator} class.
	''' For details, see the Java Cryptography Architecture.
	''' <p>
	''' <b>Concurrent Access</b>
	''' <p>
	''' Instances of this class need not be protected against concurrent
	''' access from multiple threads. Threads that need to access a single
	''' {@code CertPathValidatorSpi} instance concurrently should synchronize
	''' amongst themselves and provide the necessary locking before calling the
	''' wrapping {@code CertPathValidator} object.
	''' <p>
	''' However, implementations of {@code CertPathValidatorSpi} may still
	''' encounter concurrency issues, since multiple threads each
	''' manipulating a different {@code CertPathValidatorSpi} instance need not
	''' synchronize.
	''' 
	''' @since       1.4
	''' @author      Yassir Elley
	''' </summary>
	Public MustInherit Class CertPathValidatorSpi

		''' <summary>
		''' The default constructor.
		''' </summary>
		Public Sub New()
		End Sub

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
		Public MustOverride Function engineValidate(  certPath As CertPath,   params As CertPathParameters) As CertPathValidatorResult

		''' <summary>
		''' Returns a {@code CertPathChecker} that this implementation uses to
		''' check the revocation status of certificates. A PKIX implementation
		''' returns objects of type {@code PKIXRevocationChecker}.
		''' 
		''' <p>The primary purpose of this method is to allow callers to specify
		''' additional input parameters and options specific to revocation checking.
		''' See the class description of {@code CertPathValidator} for an example.
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