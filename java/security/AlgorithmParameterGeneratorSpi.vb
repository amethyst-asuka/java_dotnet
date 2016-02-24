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

Namespace java.security


	''' <summary>
	''' This class defines the <i>Service Provider Interface</i> (<b>SPI</b>)
	''' for the {@code AlgorithmParameterGenerator} class, which
	''' is used to generate a set of parameters to be used with a certain algorithm.
	''' 
	''' <p> All the abstract methods in this class must be implemented by each
	''' cryptographic service provider who wishes to supply the implementation
	''' of a parameter generator for a particular algorithm.
	''' 
	''' <p> In case the client does not explicitly initialize the
	''' AlgorithmParameterGenerator (via a call to an {@code engineInit}
	''' method), each provider must supply (and document) a default initialization.
	''' For example, the Sun provider uses a default modulus prime size of 1024
	''' bits for the generation of DSA parameters.
	''' 
	''' @author Jan Luehe
	''' 
	''' </summary>
	''' <seealso cref= AlgorithmParameterGenerator </seealso>
	''' <seealso cref= AlgorithmParameters </seealso>
	''' <seealso cref= java.security.spec.AlgorithmParameterSpec
	''' 
	''' @since 1.2 </seealso>

	Public MustInherit Class AlgorithmParameterGeneratorSpi

		''' <summary>
		''' Initializes this parameter generator for a certain size
		''' and source of randomness.
		''' </summary>
		''' <param name="size"> the size (number of bits). </param>
		''' <param name="random"> the source of randomness. </param>
		Protected Friend MustOverride Sub engineInit(ByVal size As Integer, ByVal random As SecureRandom)

		''' <summary>
		''' Initializes this parameter generator with a set of
		''' algorithm-specific parameter generation values.
		''' </summary>
		''' <param name="genParamSpec"> the set of algorithm-specific parameter generation values. </param>
		''' <param name="random"> the source of randomness.
		''' </param>
		''' <exception cref="InvalidAlgorithmParameterException"> if the given parameter
		''' generation values are inappropriate for this parameter generator. </exception>
		Protected Friend MustOverride Sub engineInit(ByVal genParamSpec As java.security.spec.AlgorithmParameterSpec, ByVal random As SecureRandom)

		''' <summary>
		''' Generates the parameters.
		''' </summary>
		''' <returns> the new AlgorithmParameters object. </returns>
		Protected Friend MustOverride Function engineGenerateParameters() As AlgorithmParameters
	End Class

End Namespace