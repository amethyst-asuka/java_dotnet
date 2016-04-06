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

Namespace java.security.spec


	''' <summary>
	''' This class specifies a DSA public key with its associated parameters.
	''' 
	''' @author Jan Luehe
	''' 
	''' </summary>
	''' <seealso cref= java.security.Key </seealso>
	''' <seealso cref= java.security.KeyFactory </seealso>
	''' <seealso cref= KeySpec </seealso>
	''' <seealso cref= DSAPrivateKeySpec </seealso>
	''' <seealso cref= X509EncodedKeySpec
	''' 
	''' @since 1.2 </seealso>

	Public Class DSAPublicKeySpec
		Implements KeySpec

		Private y As System.Numerics.BigInteger
		Private p As System.Numerics.BigInteger
		Private q As System.Numerics.BigInteger
		Private g As System.Numerics.BigInteger

		''' <summary>
		''' Creates a new DSAPublicKeySpec with the specified parameter values.
		''' </summary>
		''' <param name="y"> the public key.
		''' </param>
		''' <param name="p"> the prime.
		''' </param>
		''' <param name="q"> the sub-prime.
		''' </param>
		''' <param name="g"> the base. </param>
		Public Sub New(  y As System.Numerics.BigInteger,   p As System.Numerics.BigInteger,   q As System.Numerics.BigInteger,   g As System.Numerics.BigInteger)
			Me.y = y
			Me.p = p
			Me.q = q
			Me.g = g
		End Sub

		''' <summary>
		''' Returns the public key {@code y}.
		''' </summary>
		''' <returns> the public key {@code y}. </returns>
		Public Overridable Property y As System.Numerics.BigInteger
			Get
				Return Me.y
			End Get
		End Property

		''' <summary>
		''' Returns the prime {@code p}.
		''' </summary>
		''' <returns> the prime {@code p}. </returns>
		Public Overridable Property p As System.Numerics.BigInteger
			Get
				Return Me.p
			End Get
		End Property

		''' <summary>
		''' Returns the sub-prime {@code q}.
		''' </summary>
		''' <returns> the sub-prime {@code q}. </returns>
		Public Overridable Property q As System.Numerics.BigInteger
			Get
				Return Me.q
			End Get
		End Property

		''' <summary>
		''' Returns the base {@code g}.
		''' </summary>
		''' <returns> the base {@code g}. </returns>
		Public Overridable Property g As System.Numerics.BigInteger
			Get
				Return Me.g
			End Get
		End Property
	End Class

End Namespace