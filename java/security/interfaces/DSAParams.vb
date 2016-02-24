'
' * Copyright (c) 1996, 2013, Oracle and/or its affiliates. All rights reserved.
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

Namespace java.security.interfaces


	''' <summary>
	''' Interface to a DSA-specific set of key parameters, which defines a
	''' DSA <em>key family</em>. DSA (Digital Signature Algorithm) is defined
	''' in NIST's FIPS-186.
	''' </summary>
	''' <seealso cref= DSAKey </seealso>
	''' <seealso cref= java.security.Key </seealso>
	''' <seealso cref= java.security.Signature
	''' 
	''' @author Benjamin Renaud
	''' @author Josh Bloch </seealso>
	Public Interface DSAParams

		''' <summary>
		''' Returns the prime, {@code p}.
		''' </summary>
		''' <returns> the prime, {@code p}. </returns>
		ReadOnly Property p As System.Numerics.BigInteger

		''' <summary>
		''' Returns the subprime, {@code q}.
		''' </summary>
		''' <returns> the subprime, {@code q}. </returns>
		ReadOnly Property q As System.Numerics.BigInteger

		''' <summary>
		''' Returns the base, {@code g}.
		''' </summary>
		''' <returns> the base, {@code g}. </returns>
		ReadOnly Property g As System.Numerics.BigInteger
	End Interface

End Namespace