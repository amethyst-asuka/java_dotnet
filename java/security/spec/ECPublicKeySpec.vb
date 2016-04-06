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
	''' This immutable class specifies an elliptic curve public key with
	''' its associated parameters.
	''' </summary>
	''' <seealso cref= KeySpec </seealso>
	''' <seealso cref= ECPoint </seealso>
	''' <seealso cref= ECParameterSpec
	''' 
	''' @author Valerie Peng
	''' 
	''' @since 1.5 </seealso>
	Public Class ECPublicKeySpec
		Implements KeySpec

		Private w As ECPoint
		Private params As ECParameterSpec

		''' <summary>
		''' Creates a new ECPublicKeySpec with the specified
		''' parameter values. </summary>
		''' <param name="w"> the public point. </param>
		''' <param name="params"> the associated elliptic curve domain
		''' parameters. </param>
		''' <exception cref="NullPointerException"> if {@code w}
		''' or {@code params} is null. </exception>
		''' <exception cref="IllegalArgumentException"> if {@code w}
		''' is point at infinity, i.e. ECPoint.POINT_INFINITY </exception>
		Public Sub New(  w As ECPoint,   params As ECParameterSpec)
			If w Is Nothing Then Throw New NullPointerException("w is null")
			If params Is Nothing Then Throw New NullPointerException("params is null")
			If w Is ECPoint.POINT_INFINITY Then Throw New IllegalArgumentException("w is ECPoint.POINT_INFINITY")
			Me.w = w
			Me.params = params
		End Sub

		''' <summary>
		''' Returns the public point W. </summary>
		''' <returns> the public point W. </returns>
		Public Overridable Property w As ECPoint
			Get
				Return w
			End Get
		End Property

		''' <summary>
		''' Returns the associated elliptic curve domain
		''' parameters. </summary>
		''' <returns> the EC domain parameters. </returns>
		Public Overridable Property params As ECParameterSpec
			Get
				Return params
			End Get
		End Property
	End Class

End Namespace