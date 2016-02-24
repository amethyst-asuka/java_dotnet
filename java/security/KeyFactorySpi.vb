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
	''' for the {@code KeyFactory} class.
	''' All the abstract methods in this class must be implemented by each
	''' cryptographic service provider who wishes to supply the implementation
	''' of a key factory for a particular algorithm.
	''' 
	''' <P> Key factories are used to convert <I>keys</I> (opaque
	''' cryptographic keys of type {@code Key}) into <I>key specifications</I>
	''' (transparent representations of the underlying key material), and vice
	''' versa.
	''' 
	''' <P> Key factories are bi-directional. That is, they allow you to build an
	''' opaque key object from a given key specification (key material), or to
	''' retrieve the underlying key material of a key object in a suitable format.
	''' 
	''' <P> Multiple compatible key specifications may exist for the same key.
	''' For example, a DSA public key may be specified using
	''' {@code DSAPublicKeySpec} or
	''' {@code X509EncodedKeySpec}. A key factory can be used to translate
	''' between compatible key specifications.
	''' 
	''' <P> A provider should document all the key specifications supported by its
	''' key factory.
	''' 
	''' @author Jan Luehe
	''' 
	''' </summary>
	''' <seealso cref= KeyFactory </seealso>
	''' <seealso cref= Key </seealso>
	''' <seealso cref= PublicKey </seealso>
	''' <seealso cref= PrivateKey </seealso>
	''' <seealso cref= java.security.spec.KeySpec </seealso>
	''' <seealso cref= java.security.spec.DSAPublicKeySpec </seealso>
	''' <seealso cref= java.security.spec.X509EncodedKeySpec
	''' 
	''' @since 1.2 </seealso>

	Public MustInherit Class KeyFactorySpi

		''' <summary>
		''' Generates a public key object from the provided key
		''' specification (key material).
		''' </summary>
		''' <param name="keySpec"> the specification (key material) of the public key.
		''' </param>
		''' <returns> the public key.
		''' </returns>
		''' <exception cref="InvalidKeySpecException"> if the given key specification
		''' is inappropriate for this key factory to produce a public key. </exception>
		Protected Friend MustOverride Function engineGeneratePublic(ByVal keySpec As java.security.spec.KeySpec) As PublicKey

		''' <summary>
		''' Generates a private key object from the provided key
		''' specification (key material).
		''' </summary>
		''' <param name="keySpec"> the specification (key material) of the private key.
		''' </param>
		''' <returns> the private key.
		''' </returns>
		''' <exception cref="InvalidKeySpecException"> if the given key specification
		''' is inappropriate for this key factory to produce a private key. </exception>
		Protected Friend MustOverride Function engineGeneratePrivate(ByVal keySpec As java.security.spec.KeySpec) As PrivateKey

		''' <summary>
		''' Returns a specification (key material) of the given key
		''' object.
		''' {@code keySpec} identifies the specification class in which
		''' the key material should be returned. It could, for example, be
		''' {@code DSAPublicKeySpec.class}, to indicate that the
		''' key material should be returned in an instance of the
		''' {@code DSAPublicKeySpec} class.
		''' </summary>
		''' @param <T> the type of the key specification to be returned
		''' </param>
		''' <param name="key"> the key.
		''' </param>
		''' <param name="keySpec"> the specification class in which
		''' the key material should be returned.
		''' </param>
		''' <returns> the underlying key specification (key material) in an instance
		''' of the requested specification class.
		''' </returns>
		''' <exception cref="InvalidKeySpecException"> if the requested key specification is
		''' inappropriate for the given key, or the given key cannot be dealt with
		''' (e.g., the given key has an unrecognized format). </exception>
		Protected Friend MustOverride Function engineGetKeySpec(Of T As java.security.spec.KeySpec)(ByVal key As Key, ByVal keySpec As Class) As T

		''' <summary>
		''' Translates a key object, whose provider may be unknown or
		''' potentially untrusted, into a corresponding key object of this key
		''' factory.
		''' </summary>
		''' <param name="key"> the key whose provider is unknown or untrusted.
		''' </param>
		''' <returns> the translated key.
		''' </returns>
		''' <exception cref="InvalidKeyException"> if the given key cannot be processed
		''' by this key factory. </exception>
		Protected Friend MustOverride Function engineTranslateKey(ByVal key As Key) As Key

	End Class

End Namespace