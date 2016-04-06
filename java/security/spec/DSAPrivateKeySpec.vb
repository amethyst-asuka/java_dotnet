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
	''' This class specifies a DSA private key with its associated parameters.
	''' 
	''' @author Jan Luehe
	''' 
	''' </summary>
	''' <seealso cref= java.security.Key </seealso>
	''' <seealso cref= java.security.KeyFactory </seealso>
	''' <seealso cref= KeySpec </seealso>
	''' <seealso cref= DSAPublicKeySpec </seealso>
	''' <seealso cref= PKCS8EncodedKeySpec
	''' 
	''' @since 1.2 </seealso>

	Public Class DSAPrivateKeySpec
		Implements KeySpec

		Private x As System.Numerics.BigInteger
		Private p As System.Numerics.BigInteger
		Private q As System.Numerics.BigInteger
		Private g As System.Numerics.BigInteger

		''' <summary>
		''' Creates a new DSAPrivateKeySpec with the specified parameter values.
		''' </summary>
		''' <param name="x"> the private key.
		''' </param>
		''' <param name="p"> the prime.
		''' </param>
		''' <param name="q"> the sub-prime.
		''' </param>
		''' <param name="g"> the base. </param>
		Public Sub New(  x As System.Numerics.BigInteger,   p As System.Numerics.BigInteger,   q As System.Numerics.BigInteger,   g As System.Numerics.BigInteger)
			Me.x = x
			Me.p = p
			Me.q = q
			Me.g = g
		End Sub

		''' <summary>
		''' Returns the private key {@code x}.
		''' </summary>
		''' <returns> the private key {@code x}. </returns>
		Public Overridable Property x As System.Numerics.BigInteger
			Get
				Return Me.x
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