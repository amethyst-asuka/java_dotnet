'
' * Copyright (c) 2003, 2013, Oracle and/or its affiliates. All rights reserved.
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
	''' This immutable class specifies the set of domain parameters
	''' used with elliptic curve cryptography (ECC).
	''' </summary>
	''' <seealso cref= AlgorithmParameterSpec
	''' 
	''' @author Valerie Peng
	''' 
	''' @since 1.5 </seealso>
	Public Class ECParameterSpec
		Implements AlgorithmParameterSpec

		Private ReadOnly curve As EllipticCurve
		Private ReadOnly g As ECPoint
		Private ReadOnly n As System.Numerics.BigInteger
		Private ReadOnly h As Integer

		''' <summary>
		''' Creates elliptic curve domain parameters based on the
		''' specified values. </summary>
		''' <param name="curve"> the elliptic curve which this parameter
		''' defines. </param>
		''' <param name="g"> the generator which is also known as the base point. </param>
		''' <param name="n"> the order of the generator {@code g}. </param>
		''' <param name="h"> the cofactor. </param>
		''' <exception cref="NullPointerException"> if {@code curve},
		''' {@code g}, or {@code n} is null. </exception>
		''' <exception cref="IllegalArgumentException"> if {@code n}
		''' or {@code h} is not positive. </exception>
		Public Sub New(  curve As EllipticCurve,   g As ECPoint,   n As System.Numerics.BigInteger,   h As Integer)
			If curve Is Nothing Then Throw New NullPointerException("curve is null")
			If g Is Nothing Then Throw New NullPointerException("g is null")
			If n Is Nothing Then Throw New NullPointerException("n is null")
			If n.signum() <> 1 Then Throw New IllegalArgumentException("n is not positive")
			If h <= 0 Then Throw New IllegalArgumentException("h is not positive")
			Me.curve = curve
			Me.g = g
			Me.n = n
			Me.h = h
		End Sub

		''' <summary>
		''' Returns the elliptic curve that this parameter defines. </summary>
		''' <returns> the elliptic curve that this parameter defines. </returns>
		Public Overridable Property curve As EllipticCurve
			Get
				Return curve
			End Get
		End Property

		''' <summary>
		''' Returns the generator which is also known as the base point. </summary>
		''' <returns> the generator which is also known as the base point. </returns>
		Public Overridable Property generator As ECPoint
			Get
				Return g
			End Get
		End Property

		''' <summary>
		''' Returns the order of the generator. </summary>
		''' <returns> the order of the generator. </returns>
		Public Overridable Property order As System.Numerics.BigInteger
			Get
				Return n
			End Get
		End Property

		''' <summary>
		''' Returns the cofactor. </summary>
		''' <returns> the cofactor. </returns>
		Public Overridable Property cofactor As Integer
			Get
				Return h
			End Get
		End Property
	End Class

End Namespace