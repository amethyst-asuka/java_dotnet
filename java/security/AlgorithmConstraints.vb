'
' * Copyright (c) 2010, Oracle and/or its affiliates. All rights reserved.
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
	''' This interface specifies constraints for cryptographic algorithms,
	''' keys (key sizes), and other algorithm parameters.
	''' <p>
	''' {@code AlgorithmConstraints} objects are immutable.  An implementation
	''' of this interface should not provide methods that can change the state
	''' of an instance once it has been created.
	''' <p>
	''' Note that {@code AlgorithmConstraints} can be used to represent the
	''' restrictions described by the security properties
	''' {@code jdk.certpath.disabledAlgorithms} and
	''' {@code jdk.tls.disabledAlgorithms}, or could be used by a
	''' concrete {@code PKIXCertPathChecker} to check whether a specified
	''' certificate in the certification path contains the required algorithm
	''' constraints.
	''' </summary>
	''' <seealso cref= javax.net.ssl.SSLParameters#getAlgorithmConstraints </seealso>
	''' <seealso cref= javax.net.ssl.SSLParameters#setAlgorithmConstraints(AlgorithmConstraints)
	''' 
	''' @since 1.7 </seealso>

	Public Interface AlgorithmConstraints

		''' <summary>
		''' Determines whether an algorithm is granted permission for the
		''' specified cryptographic primitives.
		''' </summary>
		''' <param name="primitives"> a set of cryptographic primitives </param>
		''' <param name="algorithm"> the algorithm name </param>
		''' <param name="parameters"> the algorithm parameters, or null if no additional
		'''     parameters
		''' </param>
		''' <returns> true if the algorithm is permitted and can be used for all
		'''     of the specified cryptographic primitives
		''' </returns>
		''' <exception cref="IllegalArgumentException"> if primitives or algorithm is null
		'''     or empty </exception>
		Function permits(ByVal primitives As java.util.Set(Of CryptoPrimitive), ByVal algorithm As String, ByVal parameters As AlgorithmParameters) As Boolean

		''' <summary>
		''' Determines whether a key is granted permission for the specified
		''' cryptographic primitives.
		''' <p>
		''' This method is usually used to check key size and key usage.
		''' </summary>
		''' <param name="primitives"> a set of cryptographic primitives </param>
		''' <param name="key"> the key
		''' </param>
		''' <returns> true if the key can be used for all of the specified
		'''     cryptographic primitives
		''' </returns>
		''' <exception cref="IllegalArgumentException"> if primitives is null or empty,
		'''     or the key is null </exception>
		Function permits(ByVal primitives As java.util.Set(Of CryptoPrimitive), ByVal key As Key) As Boolean

		''' <summary>
		''' Determines whether an algorithm and the corresponding key are granted
		''' permission for the specified cryptographic primitives.
		''' </summary>
		''' <param name="primitives"> a set of cryptographic primitives </param>
		''' <param name="algorithm"> the algorithm name </param>
		''' <param name="key"> the key </param>
		''' <param name="parameters"> the algorithm parameters, or null if no additional
		'''     parameters
		''' </param>
		''' <returns> true if the key and the algorithm can be used for all of the
		'''     specified cryptographic primitives
		''' </returns>
		''' <exception cref="IllegalArgumentException"> if primitives or algorithm is null
		'''     or empty, or the key is null </exception>
		Function permits(ByVal primitives As java.util.Set(Of CryptoPrimitive), ByVal algorithm As String, ByVal key As Key, ByVal parameters As AlgorithmParameters) As Boolean

	End Interface

End Namespace