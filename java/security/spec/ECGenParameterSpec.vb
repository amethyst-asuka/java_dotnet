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
	''' This immutable class specifies the set of parameters used for
	''' generating elliptic curve (EC) domain parameters.
	''' </summary>
	''' <seealso cref= AlgorithmParameterSpec
	''' 
	''' @author Valerie Peng
	''' 
	''' @since 1.5 </seealso>
	Public Class ECGenParameterSpec
		Implements AlgorithmParameterSpec

		Private name As String

		''' <summary>
		''' Creates a parameter specification for EC parameter
		''' generation using a standard (or predefined) name
		''' {@code stdName} in order to generate the corresponding
		''' (precomputed) elliptic curve domain parameters. For the
		''' list of supported names, please consult the documentation
		''' of provider whose implementation will be used. </summary>
		''' <param name="stdName"> the standard name of the to-be-generated EC
		''' domain parameters. </param>
		''' <exception cref="NullPointerException"> if {@code stdName}
		''' is null. </exception>
		Public Sub New(  stdName As String)
			If stdName Is Nothing Then Throw New NullPointerException("stdName is null")
			Me.name = stdName
		End Sub

		''' <summary>
		''' Returns the standard or predefined name of the
		''' to-be-generated EC domain parameters. </summary>
		''' <returns> the standard or predefined name. </returns>
		Public Overridable Property name As String
			Get
				Return name
			End Get
		End Property
	End Class

End Namespace