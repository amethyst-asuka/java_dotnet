'
' * Copyright (c) 2001, 2013, Oracle and/or its affiliates. All rights reserved.
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

Namespace java.security.spec


	''' <summary>
	''' This class represents the triplet (prime, exponent, and coefficient)
	''' inside RSA's OtherPrimeInfo structure, as defined in the PKCS#1 v2.1.
	''' The ASN.1 syntax of RSA's OtherPrimeInfo is as follows:
	''' 
	''' <pre>
	''' OtherPrimeInfo ::= SEQUENCE {
	'''   prime INTEGER,
	'''   exponent INTEGER,
	'''   coefficient INTEGER
	'''   }
	''' 
	''' </pre>
	''' 
	''' @author Valerie Peng
	''' 
	''' </summary>
	''' <seealso cref= RSAPrivateCrtKeySpec </seealso>
	''' <seealso cref= java.security.interfaces.RSAMultiPrimePrivateCrtKey
	''' 
	''' @since 1.4 </seealso>

	Public Class RSAOtherPrimeInfo

		Private prime As System.Numerics.BigInteger
		Private primeExponent As System.Numerics.BigInteger
		Private crtCoefficient As System.Numerics.BigInteger


	   ''' <summary>
	   ''' Creates a new {@code RSAOtherPrimeInfo}
	   ''' given the prime, primeExponent, and
	   ''' crtCoefficient as defined in PKCS#1.
	   ''' </summary>
	   ''' <param name="prime"> the prime factor of n. </param>
	   ''' <param name="primeExponent"> the exponent. </param>
	   ''' <param name="crtCoefficient"> the Chinese Remainder Theorem
	   ''' coefficient. </param>
	   ''' <exception cref="NullPointerException"> if any of the parameters, i.e.
	   ''' {@code prime}, {@code primeExponent},
	   ''' {@code crtCoefficient}, is null.
	   '''  </exception>
		Public Sub New(  prime As System.Numerics.BigInteger,   primeExponent As System.Numerics.BigInteger,   crtCoefficient As System.Numerics.BigInteger)
			If prime Is Nothing Then Throw New NullPointerException("the prime parameter must be " & "non-null")
			If primeExponent Is Nothing Then Throw New NullPointerException("the primeExponent parameter " & "must be non-null")
			If crtCoefficient Is Nothing Then Throw New NullPointerException("the crtCoefficient parameter " & "must be non-null")
			Me.prime = prime
			Me.primeExponent = primeExponent
			Me.crtCoefficient = crtCoefficient
		End Sub

		''' <summary>
		''' Returns the prime.
		''' </summary>
		''' <returns> the prime. </returns>
		Public Property prime As System.Numerics.BigInteger
			Get
				Return Me.prime
			End Get
		End Property

		''' <summary>
		''' Returns the prime's exponent.
		''' </summary>
		''' <returns> the primeExponent. </returns>
		Public Property exponent As System.Numerics.BigInteger
			Get
				Return Me.primeExponent
			End Get
		End Property

		''' <summary>
		''' Returns the prime's crtCoefficient.
		''' </summary>
		''' <returns> the crtCoefficient. </returns>
		Public Property crtCoefficient As System.Numerics.BigInteger
			Get
				Return Me.crtCoefficient
			End Get
		End Property
	End Class

End Namespace