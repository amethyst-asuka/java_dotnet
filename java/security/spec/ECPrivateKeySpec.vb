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
	''' This immutable class specifies an elliptic curve private key with
	''' its associated parameters.
	''' </summary>
	''' <seealso cref= KeySpec </seealso>
	''' <seealso cref= ECParameterSpec
	''' 
	''' @author Valerie Peng
	''' 
	''' @since 1.5 </seealso>
	Public Class ECPrivateKeySpec
		Implements KeySpec

		Private s As System.Numerics.BigInteger
		Private params As ECParameterSpec

		''' <summary>
		''' Creates a new ECPrivateKeySpec with the specified
		''' parameter values. </summary>
		''' <param name="s"> the private value. </param>
		''' <param name="params"> the associated elliptic curve domain
		''' parameters. </param>
		''' <exception cref="NullPointerException"> if {@code s}
		''' or {@code params} is null. </exception>
		Public Sub New(  s As System.Numerics.BigInteger,   params As ECParameterSpec)
			If s Is Nothing Then Throw New NullPointerException("s is null")
			If params Is Nothing Then Throw New NullPointerException("params is null")
			Me.s = s
			Me.params = params
		End Sub

		''' <summary>
		''' Returns the private value S. </summary>
		''' <returns> the private value S. </returns>
		Public Overridable Property s As System.Numerics.BigInteger
			Get
				Return s
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