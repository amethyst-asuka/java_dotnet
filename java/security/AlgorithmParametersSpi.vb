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
	''' for the {@code AlgorithmParameters} class, which is used to manage
	''' algorithm parameters.
	''' 
	''' <p> All the abstract methods in this class must be implemented by each
	''' cryptographic service provider who wishes to supply parameter management
	''' for a particular algorithm.
	''' 
	''' @author Jan Luehe
	''' 
	''' </summary>
	''' <seealso cref= AlgorithmParameters </seealso>
	''' <seealso cref= java.security.spec.AlgorithmParameterSpec </seealso>
	''' <seealso cref= java.security.spec.DSAParameterSpec
	''' 
	''' @since 1.2 </seealso>

	Public MustInherit Class AlgorithmParametersSpi

		''' <summary>
		''' Initializes this parameters object using the parameters
		''' specified in {@code paramSpec}.
		''' </summary>
		''' <param name="paramSpec"> the parameter specification.
		''' </param>
		''' <exception cref="InvalidParameterSpecException"> if the given parameter
		''' specification is inappropriate for the initialization of this parameter
		''' object. </exception>
		Protected Friend MustOverride Sub engineInit(ByVal paramSpec As java.security.spec.AlgorithmParameterSpec)

		''' <summary>
		''' Imports the specified parameters and decodes them
		''' according to the primary decoding format for parameters.
		''' The primary decoding format for parameters is ASN.1, if an ASN.1
		''' specification for this type of parameters exists.
		''' </summary>
		''' <param name="params"> the encoded parameters.
		''' </param>
		''' <exception cref="IOException"> on decoding errors </exception>
		Protected Friend MustOverride Sub engineInit(ByVal params As SByte())

		''' <summary>
		''' Imports the parameters from {@code params} and
		''' decodes them according to the specified decoding format.
		''' If {@code format} is null, the
		''' primary decoding format for parameters is used. The primary decoding
		''' format is ASN.1, if an ASN.1 specification for these parameters
		''' exists.
		''' </summary>
		''' <param name="params"> the encoded parameters.
		''' </param>
		''' <param name="format"> the name of the decoding format.
		''' </param>
		''' <exception cref="IOException"> on decoding errors </exception>
		Protected Friend MustOverride Sub engineInit(ByVal params As SByte(), ByVal format As String)

		''' <summary>
		''' Returns a (transparent) specification of this parameters
		''' object.
		''' {@code paramSpec} identifies the specification class in which
		''' the parameters should be returned. It could, for example, be
		''' {@code DSAParameterSpec.class}, to indicate that the
		''' parameters should be returned in an instance of the
		''' {@code DSAParameterSpec} class.
		''' </summary>
		''' @param <T> the type of the parameter specification to be returned
		''' </param>
		''' <param name="paramSpec"> the specification class in which
		''' the parameters should be returned.
		''' </param>
		''' <returns> the parameter specification.
		''' </returns>
		''' <exception cref="InvalidParameterSpecException"> if the requested parameter
		''' specification is inappropriate for this parameter object. </exception>
		Protected Friend MustOverride Function engineGetParameterSpec(Of T As java.security.spec.AlgorithmParameterSpec)(ByVal paramSpec As Class) As T

		''' <summary>
		''' Returns the parameters in their primary encoding format.
		''' The primary encoding format for parameters is ASN.1, if an ASN.1
		''' specification for this type of parameters exists.
		''' </summary>
		''' <returns> the parameters encoded using their primary encoding format.
		''' </returns>
		''' <exception cref="IOException"> on encoding errors. </exception>
		Protected Friend MustOverride Function engineGetEncoded() As SByte()

		''' <summary>
		''' Returns the parameters encoded in the specified format.
		''' If {@code format} is null, the
		''' primary encoding format for parameters is used. The primary encoding
		''' format is ASN.1, if an ASN.1 specification for these parameters
		''' exists.
		''' </summary>
		''' <param name="format"> the name of the encoding format.
		''' </param>
		''' <returns> the parameters encoded using the specified encoding scheme.
		''' </returns>
		''' <exception cref="IOException"> on encoding errors. </exception>
		Protected Friend MustOverride Function engineGetEncoded(ByVal format As String) As SByte()

		''' <summary>
		''' Returns a formatted string describing the parameters.
		''' </summary>
		''' <returns> a formatted string describing the parameters. </returns>
		Protected Friend MustOverride Function engineToString() As String
	End Class

End Namespace