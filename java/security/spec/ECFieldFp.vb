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
	''' This immutable class defines an elliptic curve (EC) prime
	''' finite field.
	''' </summary>
	''' <seealso cref= ECField
	''' 
	''' @author Valerie Peng
	''' 
	''' @since 1.5 </seealso>
	Public Class ECFieldFp
		Implements ECField

		Private p As System.Numerics.BigInteger

		''' <summary>
		''' Creates an elliptic curve prime finite field
		''' with the specified prime {@code p}. </summary>
		''' <param name="p"> the prime. </param>
		''' <exception cref="NullPointerException"> if {@code p} is null. </exception>
		''' <exception cref="IllegalArgumentException"> if {@code p}
		''' is not positive. </exception>
		Public Sub New(ByVal p As System.Numerics.BigInteger)
			If p.signum() <> 1 Then Throw New IllegalArgumentException("p is not positive")
			Me.p = p
		End Sub

		''' <summary>
		''' Returns the field size in bits which is size of prime p
		''' for this prime finite field. </summary>
		''' <returns> the field size in bits. </returns>
		Public Overridable Property fieldSize As Integer Implements ECField.getFieldSize
			Get
				Return p.bitLength()
			End Get
		End Property

		''' <summary>
		''' Returns the prime {@code p} of this prime finite field. </summary>
		''' <returns> the prime. </returns>
		Public Overridable Property p As System.Numerics.BigInteger
			Get
				Return p
			End Get
		End Property

		''' <summary>
		''' Compares this prime finite field for equality with the
		''' specified object. </summary>
		''' <param name="obj"> the object to be compared. </param>
		''' <returns> true if {@code obj} is an instance
		''' of ECFieldFp and the prime value match, false otherwise. </returns>
		Public Overrides Function Equals(ByVal obj As Object) As Boolean
			If Me Is obj Then Return True
			If TypeOf obj Is ECFieldFp Then Return (p.Equals(CType(obj, ECFieldFp).p))
			Return False
		End Function

		''' <summary>
		''' Returns a hash code value for this prime finite field. </summary>
		''' <returns> a hash code value. </returns>
		Public Overrides Function GetHashCode() As Integer
			Return p.GetHashCode()
		End Function
	End Class

End Namespace