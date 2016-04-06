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
	''' This class specifies the set of parameters used with the DSA algorithm.
	''' 
	''' @author Jan Luehe
	''' 
	''' </summary>
	''' <seealso cref= AlgorithmParameterSpec
	''' 
	''' @since 1.2 </seealso>

	Public Class DSAParameterSpec
		Implements AlgorithmParameterSpec, java.security.interfaces.DSAParams

		Friend p As System.Numerics.BigInteger
		Friend q As System.Numerics.BigInteger
		Friend g As System.Numerics.BigInteger

		''' <summary>
		''' Creates a new DSAParameterSpec with the specified parameter values.
		''' </summary>
		''' <param name="p"> the prime.
		''' </param>
		''' <param name="q"> the sub-prime.
		''' </param>
		''' <param name="g"> the base. </param>
		Public Sub New(  p As System.Numerics.BigInteger,   q As System.Numerics.BigInteger,   g As System.Numerics.BigInteger)
			Me.p = p
			Me.q = q
			Me.g = g
		End Sub

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