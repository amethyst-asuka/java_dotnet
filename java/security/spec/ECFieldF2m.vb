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
	''' This immutable class defines an elliptic curve (EC)
	''' characteristic 2 finite field.
	''' </summary>
	''' <seealso cref= ECField
	''' 
	''' @author Valerie Peng
	''' 
	''' @since 1.5 </seealso>
	Public Class ECFieldF2m
		Implements ECField

		Private m As Integer
		Private ks As Integer()
		Private rp As System.Numerics.BigInteger

		''' <summary>
		''' Creates an elliptic curve characteristic 2 finite
		''' field which has 2^{@code m} elements with normal basis. </summary>
		''' <param name="m"> with 2^{@code m} being the number of elements. </param>
		''' <exception cref="IllegalArgumentException"> if {@code m}
		''' is not positive. </exception>
		Public Sub New(ByVal m As Integer)
			If m <= 0 Then Throw New IllegalArgumentException("m is not positive")
			Me.m = m
			Me.ks = Nothing
			Me.rp = Nothing
		End Sub

		''' <summary>
		''' Creates an elliptic curve characteristic 2 finite
		''' field which has 2^{@code m} elements with
		''' polynomial basis.
		''' The reduction polynomial for this field is based
		''' on {@code rp} whose i-th bit corresponds to
		''' the i-th coefficient of the reduction polynomial.<p>
		''' Note: A valid reduction polynomial is either a
		''' trinomial (X^{@code m} + X^{@code k} + 1
		''' with {@code m} &gt; {@code k} &gt;= 1) or a
		''' pentanomial (X^{@code m} + X^{@code k3}
		''' + X^{@code k2} + X^{@code k1} + 1 with
		''' {@code m} &gt; {@code k3} &gt; {@code k2}
		''' &gt; {@code k1} &gt;= 1). </summary>
		''' <param name="m"> with 2^{@code m} being the number of elements. </param>
		''' <param name="rp"> the BigInteger whose i-th bit corresponds to
		''' the i-th coefficient of the reduction polynomial. </param>
		''' <exception cref="NullPointerException"> if {@code rp} is null. </exception>
		''' <exception cref="IllegalArgumentException"> if {@code m}
		''' is not positive, or {@code rp} does not represent
		''' a valid reduction polynomial. </exception>
		Public Sub New(ByVal m As Integer, ByVal rp As System.Numerics.BigInteger)
			' check m and rp
			Me.m = m
			Me.rp = rp
			If m <= 0 Then Throw New IllegalArgumentException("m is not positive")
			Dim bitCount As Integer = Me.rp.bitCount()
			If (Not Me.rp.testBit(0)) OrElse (Not Me.rp.testBit(m)) OrElse ((bitCount <> 3) AndAlso (bitCount <> 5)) Then Throw New IllegalArgumentException("rp does not represent a valid reduction polynomial")
			' convert rp into ks
			Dim temp As System.Numerics.BigInteger = Me.rp.clearBit(0).clearBit(m)
			Me.ks = New Integer(bitCount-2 - 1){}
			For i As Integer = Me.ks.Length-1 To 0 Step -1
				Dim index As Integer = temp.lowestSetBit
				Me.ks(i) = index
				temp = temp.clearBit(index)
			Next i
		End Sub

		''' <summary>
		''' Creates an elliptic curve characteristic 2 finite
		''' field which has 2^{@code m} elements with
		''' polynomial basis. The reduction polynomial for this
		''' field is based on {@code ks} whose content
		''' contains the order of the middle term(s) of the
		''' reduction polynomial.
		''' Note: A valid reduction polynomial is either a
		''' trinomial (X^{@code m} + X^{@code k} + 1
		''' with {@code m} &gt; {@code k} &gt;= 1) or a
		''' pentanomial (X^{@code m} + X^{@code k3}
		''' + X^{@code k2} + X^{@code k1} + 1 with
		''' {@code m} &gt; {@code k3} &gt; {@code k2}
		''' &gt; {@code k1} &gt;= 1), so {@code ks} should
		''' have length 1 or 3. </summary>
		''' <param name="m"> with 2^{@code m} being the number of elements. </param>
		''' <param name="ks"> the order of the middle term(s) of the
		''' reduction polynomial. Contents of this array are copied
		''' to protect against subsequent modification. </param>
		''' <exception cref="NullPointerException"> if {@code ks} is null. </exception>
		''' <exception cref="IllegalArgumentException"> if{@code m}
		''' is not positive, or the length of {@code ks}
		''' is neither 1 nor 3, or values in {@code ks}
		''' are not between {@code m}-1 and 1 (inclusive)
		''' and in descending order. </exception>
		Public Sub New(ByVal m As Integer, ByVal ks As Integer())
			' check m and ks
			Me.m = m
			Me.ks = ks.clone()
			If m <= 0 Then Throw New IllegalArgumentException("m is not positive")
			If (Me.ks.Length <> 1) AndAlso (Me.ks.Length <> 3) Then Throw New IllegalArgumentException("length of ks is neither 1 nor 3")
			For i As Integer = 0 To Me.ks.Length - 1
				If (Me.ks(i) < 1) OrElse (Me.ks(i) > m-1) Then Throw New IllegalArgumentException("ks[" & i & "] is out of range")
				If (i <> 0) AndAlso (Me.ks(i) >= Me.ks(i-1)) Then Throw New IllegalArgumentException("values in ks are not in descending order")
			Next i
			' convert ks into rp
			Me.rp = System.Numerics.BigInteger.ONE
			Me.rp = rp.bitBit(m)
			For j As Integer = 0 To Me.ks.Length - 1
				rp = rp.bitBit(Me.ks(j))
			Next j
		End Sub

		''' <summary>
		''' Returns the field size in bits which is {@code m}
		''' for this characteristic 2 finite field. </summary>
		''' <returns> the field size in bits. </returns>
		Public Overridable Property fieldSize As Integer Implements ECField.getFieldSize
			Get
				Return m
			End Get
		End Property

		''' <summary>
		''' Returns the value {@code m} of this characteristic
		''' 2 finite field. </summary>
		''' <returns> {@code m} with 2^{@code m} being the
		''' number of elements. </returns>
		Public Overridable Property m As Integer
			Get
				Return m
			End Get
		End Property

		''' <summary>
		''' Returns a BigInteger whose i-th bit corresponds to the
		''' i-th coefficient of the reduction polynomial for polynomial
		''' basis or null for normal basis. </summary>
		''' <returns> a BigInteger whose i-th bit corresponds to the
		''' i-th coefficient of the reduction polynomial for polynomial
		''' basis or null for normal basis. </returns>
		Public Overridable Property reductionPolynomial As System.Numerics.BigInteger
			Get
				Return rp
			End Get
		End Property

		''' <summary>
		''' Returns an integer array which contains the order of the
		''' middle term(s) of the reduction polynomial for polynomial
		''' basis or null for normal basis. </summary>
		''' <returns> an integer array which contains the order of the
		''' middle term(s) of the reduction polynomial for polynomial
		''' basis or null for normal basis. A new array is returned
		''' each time this method is called. </returns>
		Public Overridable Property midTermsOfReductionPolynomial As Integer()
			Get
				If ks Is Nothing Then
					Return Nothing
				Else
					Return ks.clone()
				End If
			End Get
		End Property

		''' <summary>
		''' Compares this finite field for equality with the
		''' specified object. </summary>
		''' <param name="obj"> the object to be compared. </param>
		''' <returns> true if {@code obj} is an instance
		''' of ECFieldF2m and both {@code m} and the reduction
		''' polynomial match, false otherwise. </returns>
		Public Overrides Function Equals(ByVal obj As Object) As Boolean
			If Me Is obj Then Return True
			If TypeOf obj Is ECFieldF2m Then Return ((m = CType(obj, ECFieldF2m).m) AndAlso (java.util.Arrays.Equals(ks, CType(obj, ECFieldF2m).ks)))
			Return False
		End Function

		''' <summary>
		''' Returns a hash code value for this characteristic 2
		''' finite field. </summary>
		''' <returns> a hash code value. </returns>
		Public Overrides Function GetHashCode() As Integer
			Dim value As Integer = m << 5
			value += (If(rp Is Nothing, 0, rp.GetHashCode()))
			' no need to involve ks here since ks and rp
			' should be equivalent.
			Return value
		End Function
	End Class

End Namespace