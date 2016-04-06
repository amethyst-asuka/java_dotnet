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
	''' This immutable class holds the necessary values needed to represent
	''' an elliptic curve.
	''' </summary>
	''' <seealso cref= ECField </seealso>
	''' <seealso cref= ECFieldFp </seealso>
	''' <seealso cref= ECFieldF2m
	''' 
	''' @author Valerie Peng
	''' 
	''' @since 1.5 </seealso>
	Public Class EllipticCurve

		Private ReadOnly field As ECField
		Private ReadOnly a As System.Numerics.BigInteger
		Private ReadOnly b As System.Numerics.BigInteger
		Private ReadOnly seed As SByte()

		' Check coefficient c is a valid element in ECField field.
		Private Shared Sub checkValidity(  field As ECField,   c As System.Numerics.BigInteger,   cName As String)
			' can only perform check if field is ECFieldFp or ECFieldF2m.
			If TypeOf field Is ECFieldFp Then
				Dim p As System.Numerics.BigInteger = CType(field, ECFieldFp).p
				If p.CompareTo(c) <> 1 Then
					Throw New IllegalArgumentException(cName & " is too large")
				ElseIf c.signum() < 0 Then
					Throw New IllegalArgumentException(cName & " is negative")
				End If
			ElseIf TypeOf field Is ECFieldF2m Then
				Dim m As Integer = CType(field, ECFieldF2m).m
				If c.bitLength() > m Then Throw New IllegalArgumentException(cName & " is too large")
			End If
		End Sub

		''' <summary>
		''' Creates an elliptic curve with the specified elliptic field
		''' {@code field} and the coefficients {@code a} and
		''' {@code b}. </summary>
		''' <param name="field"> the finite field that this elliptic curve is over. </param>
		''' <param name="a"> the first coefficient of this elliptic curve. </param>
		''' <param name="b"> the second coefficient of this elliptic curve. </param>
		''' <exception cref="NullPointerException"> if {@code field},
		''' {@code a}, or {@code b} is null. </exception>
		''' <exception cref="IllegalArgumentException"> if {@code a}
		''' or {@code b} is not null and not in {@code field}. </exception>
		Public Sub New(  field As ECField,   a As System.Numerics.BigInteger,   b As System.Numerics.BigInteger)
			Me.New(field, a, b, Nothing)
		End Sub

		''' <summary>
		''' Creates an elliptic curve with the specified elliptic field
		''' {@code field}, the coefficients {@code a} and
		''' {@code b}, and the {@code seed} used for curve generation. </summary>
		''' <param name="field"> the finite field that this elliptic curve is over. </param>
		''' <param name="a"> the first coefficient of this elliptic curve. </param>
		''' <param name="b"> the second coefficient of this elliptic curve. </param>
		''' <param name="seed"> the bytes used during curve generation for later
		''' validation. Contents of this array are copied to protect against
		''' subsequent modification. </param>
		''' <exception cref="NullPointerException"> if {@code field},
		''' {@code a}, or {@code b} is null. </exception>
		''' <exception cref="IllegalArgumentException"> if {@code a}
		''' or {@code b} is not null and not in {@code field}. </exception>
		Public Sub New(  field As ECField,   a As System.Numerics.BigInteger,   b As System.Numerics.BigInteger,   seed As SByte())
			If field Is Nothing Then Throw New NullPointerException("field is null")
			If a Is Nothing Then Throw New NullPointerException("first coefficient is null")
			If b Is Nothing Then Throw New NullPointerException("second coefficient is null")
			checkValidity(field, a, "first coefficient")
			checkValidity(field, b, "second coefficient")
			Me.field = field
			Me.a = a
			Me.b = b
			If seed IsNot Nothing Then
				Me.seed = seed.clone()
			Else
				Me.seed = Nothing
			End If
		End Sub

		''' <summary>
		''' Returns the finite field {@code field} that this
		''' elliptic curve is over. </summary>
		''' <returns> the field {@code field} that this curve
		''' is over. </returns>
		Public Overridable Property field As ECField
			Get
				Return field
			End Get
		End Property

		''' <summary>
		''' Returns the first coefficient {@code a} of the
		''' elliptic curve. </summary>
		''' <returns> the first coefficient {@code a}. </returns>
		Public Overridable Property a As System.Numerics.BigInteger
			Get
				Return a
			End Get
		End Property

		''' <summary>
		''' Returns the second coefficient {@code b} of the
		''' elliptic curve. </summary>
		''' <returns> the second coefficient {@code b}. </returns>
		Public Overridable Property b As System.Numerics.BigInteger
			Get
				Return b
			End Get
		End Property

		''' <summary>
		''' Returns the seeding bytes {@code seed} used
		''' during curve generation. May be null if not specified. </summary>
		''' <returns> the seeding bytes {@code seed}. A new
		''' array is returned each time this method is called. </returns>
		Public Overridable Property seed As SByte()
			Get
				If seed Is Nothing Then
					Return Nothing
				Else
					Return seed.clone()
				End If
			End Get
		End Property

		''' <summary>
		''' Compares this elliptic curve for equality with the
		''' specified object. </summary>
		''' <param name="obj"> the object to be compared. </param>
		''' <returns> true if {@code obj} is an instance of
		''' EllipticCurve and the field, A, and B match, false otherwise. </returns>
		Public Overrides Function Equals(  obj As Object) As Boolean
			If Me Is obj Then Return True
			If TypeOf obj Is EllipticCurve Then
				Dim curve As EllipticCurve = CType(obj, EllipticCurve)
				If (field.Equals(curve.field)) AndAlso (a.Equals(curve.a)) AndAlso (b.Equals(curve.b)) Then Return True
			End If
			Return False
		End Function

		''' <summary>
		''' Returns a hash code value for this elliptic curve. </summary>
		''' <returns> a hash code value computed from the hash codes of the field, A,
		''' and B, as follows:
		''' <pre>{@code
		'''     (field.hashCode() << 6) + (a.hashCode() << 4) + (b.hashCode() << 2)
		''' }</pre> </returns>
		Public Overrides Function GetHashCode() As Integer
			Return (field.GetHashCode() << 6 + (a.GetHashCode() << 4) + (b.GetHashCode() << 2))
		End Function
	End Class

End Namespace