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
	''' This immutable class represents a point on an elliptic curve (EC)
	''' in affine coordinates. Other coordinate systems can
	''' extend this class to represent this point in other
	''' coordinates.
	''' 
	''' @author Valerie Peng
	''' 
	''' @since 1.5
	''' </summary>
	Public Class ECPoint

		Private ReadOnly x As System.Numerics.BigInteger
		Private ReadOnly y As System.Numerics.BigInteger

		''' <summary>
		''' This defines the point at infinity.
		''' </summary>
		Public Shared ReadOnly POINT_INFINITY As New ECPoint

		' private constructor for constructing point at infinity
		Private Sub New()
			Me.x = Nothing
			Me.y = Nothing
		End Sub

		''' <summary>
		''' Creates an ECPoint from the specified affine x-coordinate
		''' {@code x} and affine y-coordinate {@code y}. </summary>
		''' <param name="x"> the affine x-coordinate. </param>
		''' <param name="y"> the affine y-coordinate. </param>
		''' <exception cref="NullPointerException"> if {@code x} or
		''' {@code y} is null. </exception>
		Public Sub New(ByVal x As System.Numerics.BigInteger, ByVal y As System.Numerics.BigInteger)
			If (x Is Nothing) OrElse (y Is Nothing) Then Throw New NullPointerException("affine coordinate x or y is null")
			Me.x = x
			Me.y = y
		End Sub

		''' <summary>
		''' Returns the affine x-coordinate {@code x}.
		''' Note: POINT_INFINITY has a null affine x-coordinate. </summary>
		''' <returns> the affine x-coordinate. </returns>
		Public Overridable Property affineX As System.Numerics.BigInteger
			Get
				Return x
			End Get
		End Property

		''' <summary>
		''' Returns the affine y-coordinate {@code y}.
		''' Note: POINT_INFINITY has a null affine y-coordinate. </summary>
		''' <returns> the affine y-coordinate. </returns>
		Public Overridable Property affineY As System.Numerics.BigInteger
			Get
				Return y
			End Get
		End Property

		''' <summary>
		''' Compares this elliptic curve point for equality with
		''' the specified object. </summary>
		''' <param name="obj"> the object to be compared. </param>
		''' <returns> true if {@code obj} is an instance of
		''' ECPoint and the affine coordinates match, false otherwise. </returns>
		Public Overrides Function Equals(ByVal obj As Object) As Boolean
			If Me Is obj Then Return True
			If Me Is POINT_INFINITY Then Return False
			If TypeOf obj Is ECPoint Then Return ((x.Equals(CType(obj, ECPoint).x)) AndAlso (y.Equals(CType(obj, ECPoint).y)))
			Return False
		End Function

		''' <summary>
		''' Returns a hash code value for this elliptic curve point. </summary>
		''' <returns> a hash code value. </returns>
		Public Overrides Function GetHashCode() As Integer
			If Me Is POINT_INFINITY Then Return 0
			Return x.GetHashCode() << 5 + y.GetHashCode()
		End Function
	End Class

End Namespace