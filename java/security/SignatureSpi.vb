Imports System

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
	''' for the {@code Signature} [Class], which is used to provide the
	''' functionality of a digital signature algorithm. Digital signatures are used
	''' for authentication and integrity assurance of digital data.
	''' .
	''' <p> All the abstract methods in this class must be implemented by each
	''' cryptographic service provider who wishes to supply the implementation
	''' of a particular signature algorithm.
	''' 
	''' @author Benjamin Renaud
	''' 
	''' </summary>
	''' <seealso cref= Signature </seealso>

	Public MustInherit Class SignatureSpi

		''' <summary>
		''' Application-specified source of randomness.
		''' </summary>
		Protected Friend appRandom As SecureRandom = Nothing

		''' <summary>
		''' Initializes this signature object with the specified
		''' public key for verification operations.
		''' </summary>
		''' <param name="publicKey"> the public key of the identity whose signature is
		''' going to be verified.
		''' </param>
		''' <exception cref="InvalidKeyException"> if the key is improperly
		''' encoded, parameters are missing, and so on. </exception>
		Protected Friend MustOverride Sub engineInitVerify(ByVal publicKey As PublicKey)

		''' <summary>
		''' Initializes this signature object with the specified
		''' private key for signing operations.
		''' </summary>
		''' <param name="privateKey"> the private key of the identity whose signature
		''' will be generated.
		''' </param>
		''' <exception cref="InvalidKeyException"> if the key is improperly
		''' encoded, parameters are missing, and so on. </exception>
		Protected Friend MustOverride Sub engineInitSign(ByVal privateKey As PrivateKey)

		''' <summary>
		''' Initializes this signature object with the specified
		''' private key and source of randomness for signing operations.
		''' 
		''' <p>This concrete method has been added to this previously-defined
		''' abstract class. (For backwards compatibility, it cannot be abstract.)
		''' </summary>
		''' <param name="privateKey"> the private key of the identity whose signature
		''' will be generated. </param>
		''' <param name="random"> the source of randomness
		''' </param>
		''' <exception cref="InvalidKeyException"> if the key is improperly
		''' encoded, parameters are missing, and so on. </exception>
		Protected Friend Overridable Sub engineInitSign(ByVal privateKey As PrivateKey, ByVal random_Renamed As SecureRandom)
				Me.appRandom = random_Renamed
				engineInitSign(privateKey)
		End Sub

		''' <summary>
		''' Updates the data to be signed or verified
		''' using the specified java.lang.[Byte].
		''' </summary>
		''' <param name="b"> the byte to use for the update.
		''' </param>
		''' <exception cref="SignatureException"> if the engine is not initialized
		''' properly. </exception>
		Protected Friend MustOverride Sub engineUpdate(ByVal b As SByte)

		''' <summary>
		''' Updates the data to be signed or verified, using the
		''' specified array of bytes, starting at the specified offset.
		''' </summary>
		''' <param name="b"> the array of bytes </param>
		''' <param name="off"> the offset to start from in the array of bytes </param>
		''' <param name="len"> the number of bytes to use, starting at offset
		''' </param>
		''' <exception cref="SignatureException"> if the engine is not initialized
		''' properly </exception>
		Protected Friend MustOverride Sub engineUpdate(ByVal b As SByte(), ByVal [off] As Integer, ByVal len As Integer)

		''' <summary>
		''' Updates the data to be signed or verified using the specified
		''' ByteBuffer. Processes the {@code data.remaining()} bytes
		''' starting at at {@code data.position()}.
		''' Upon return, the buffer's position will be equal to its limit;
		''' its limit will not have changed.
		''' </summary>
		''' <param name="input"> the ByteBuffer
		''' @since 1.5 </param>
		Protected Friend Overridable Sub engineUpdate(ByVal input As java.nio.ByteBuffer)
			If input.hasRemaining() = False Then Return
			Try
				If input.hasArray() Then
					Dim b As SByte() = input.array()
					Dim ofs As Integer = input.arrayOffset()
					Dim pos As Integer = input.position()
					Dim lim As Integer = input.limit()
					engineUpdate(b, ofs + pos, lim - pos)
					input.position(lim)
				Else
					Dim len As Integer = input.remaining()
					Dim b As SByte() = New SByte(sun.security.jca.JCAUtil.getTempArraySize(len) - 1){}
					Do While len > 0
						Dim chunk As Integer = System.Math.Min(len, b.Length)
						input.get(b, 0, chunk)
						engineUpdate(b, 0, chunk)
						len -= chunk
					Loop
				End If
			Catch e As SignatureException
				' is specified to only occur when the engine is not initialized
				' this case should never occur as it is caught in Signature.java
				Throw New ProviderException("update() failed", e)
			End Try
		End Sub

		''' <summary>
		''' Returns the signature bytes of all the data
		''' updated so far.
		''' The format of the signature depends on the underlying
		''' signature scheme.
		''' </summary>
		''' <returns> the signature bytes of the signing operation's result.
		''' </returns>
		''' <exception cref="SignatureException"> if the engine is not
		''' initialized properly or if this signature algorithm is unable to
		''' process the input data provided. </exception>
		Protected Friend MustOverride Function engineSign() As SByte()

		''' <summary>
		''' Finishes this signature operation and stores the resulting signature
		''' bytes in the provided buffer {@code outbuf}, starting at
		''' {@code offset}.
		''' The format of the signature depends on the underlying
		''' signature scheme.
		''' 
		''' <p>The signature implementation is reset to its initial state
		''' (the state it was in after a call to one of the
		''' {@code engineInitSign} methods)
		''' and can be reused to generate further signatures with the same private
		''' key.
		''' 
		''' This method should be abstract, but we leave it concrete for
		''' binary compatibility.  Knowledgeable providers should override this
		''' method.
		''' </summary>
		''' <param name="outbuf"> buffer for the signature result.
		''' </param>
		''' <param name="offset"> offset into {@code outbuf} where the signature is
		''' stored.
		''' </param>
		''' <param name="len"> number of bytes within {@code outbuf} allotted for the
		''' signature.
		''' Both this default implementation and the SUN provider do not
		''' return partial digests. If the value of this parameter is less
		''' than the actual signature length, this method will throw a
		''' SignatureException.
		''' This parameter is ignored if its value is greater than or equal to
		''' the actual signature length.
		''' </param>
		''' <returns> the number of bytes placed into {@code outbuf}
		''' </returns>
		''' <exception cref="SignatureException"> if the engine is not
		''' initialized properly, if this signature algorithm is unable to
		''' process the input data provided, or if {@code len} is less
		''' than the actual signature length.
		''' 
		''' @since 1.2 </exception>
		Protected Friend Overridable Function engineSign(ByVal outbuf As SByte(), ByVal offset As Integer, ByVal len As Integer) As Integer
			Dim sig As SByte() = engineSign()
			If len < sig.Length Then Throw New SignatureException("partial signatures not returned")
			If outbuf.Length - offset < sig.Length Then Throw New SignatureException("insufficient space in the output buffer to store the " & "signature")
			Array.Copy(sig, 0, outbuf, offset, sig.Length)
			Return sig.Length
		End Function

		''' <summary>
		''' Verifies the passed-in signature.
		''' </summary>
		''' <param name="sigBytes"> the signature bytes to be verified.
		''' </param>
		''' <returns> true if the signature was verified, false if not.
		''' </returns>
		''' <exception cref="SignatureException"> if the engine is not
		''' initialized properly, the passed-in signature is improperly
		''' encoded or of the wrong type, if this signature algorithm is unable to
		''' process the input data provided, etc. </exception>
		Protected Friend MustOverride Function engineVerify(ByVal sigBytes As SByte()) As Boolean

		''' <summary>
		''' Verifies the passed-in signature in the specified array
		''' of bytes, starting at the specified offset.
		''' 
		''' <p> Note: Subclasses should overwrite the default implementation.
		''' 
		''' </summary>
		''' <param name="sigBytes"> the signature bytes to be verified. </param>
		''' <param name="offset"> the offset to start from in the array of bytes. </param>
		''' <param name="length"> the number of bytes to use, starting at offset.
		''' </param>
		''' <returns> true if the signature was verified, false if not.
		''' </returns>
		''' <exception cref="SignatureException"> if the engine is not
		''' initialized properly, the passed-in signature is improperly
		''' encoded or of the wrong type, if this signature algorithm is unable to
		''' process the input data provided, etc.
		''' @since 1.4 </exception>
		Protected Friend Overridable Function engineVerify(ByVal sigBytes As SByte(), ByVal offset As Integer, ByVal length As Integer) As Boolean
			Dim sigBytesCopy As SByte() = New SByte(length - 1){}
			Array.Copy(sigBytes, offset, sigBytesCopy, 0, length)
			Return engineVerify(sigBytesCopy)
		End Function

		''' <summary>
		''' Sets the specified algorithm parameter to the specified
		''' value. This method supplies a general-purpose mechanism through
		''' which it is possible to set the various parameters of this object.
		''' A parameter may be any settable parameter for the algorithm, such as
		''' a parameter size, or a source of random bits for signature generation
		''' (if appropriate), or an indication of whether or not to perform
		''' a specific but optional computation. A uniform algorithm-specific
		''' naming scheme for each parameter is desirable but left unspecified
		''' at this time.
		''' </summary>
		''' <param name="param"> the string identifier of the parameter.
		''' </param>
		''' <param name="value"> the parameter value.
		''' </param>
		''' <exception cref="InvalidParameterException"> if {@code param} is an
		''' invalid parameter for this signature algorithm engine,
		''' the parameter is already set
		''' and cannot be set again, a security exception occurs, and so on.
		''' </exception>
		''' @deprecated Replaced by {@link
		''' #engineSetParameter(java.security.spec.AlgorithmParameterSpec)
		''' engineSetParameter}. 
		<Obsolete("Replaced by {@link")> _
		Protected Friend MustOverride Sub engineSetParameter(ByVal param As String, ByVal value As Object)

		''' <summary>
		''' <p>This method is overridden by providers to initialize
		''' this signature engine with the specified parameter set.
		''' </summary>
		''' <param name="params"> the parameters
		''' </param>
		''' <exception cref="UnsupportedOperationException"> if this method is not
		''' overridden by a provider
		''' </exception>
		''' <exception cref="InvalidAlgorithmParameterException"> if this method is
		''' overridden by a provider and the given parameters
		''' are inappropriate for this signature engine </exception>
		Protected Friend Overridable Sub engineSetParameter(ByVal params As java.security.spec.AlgorithmParameterSpec)
				Throw New UnsupportedOperationException
		End Sub

		''' <summary>
		''' <p>This method is overridden by providers to return the
		''' parameters used with this signature engine, or null
		''' if this signature engine does not use any parameters.
		''' 
		''' <p>The returned parameters may be the same that were used to initialize
		''' this signature engine, or may contain a combination of default and
		''' randomly generated parameter values used by the underlying signature
		''' implementation if this signature engine requires algorithm parameters
		''' but was not initialized with any.
		''' </summary>
		''' <returns> the parameters used with this signature engine, or null if this
		''' signature engine does not use any parameters
		''' </returns>
		''' <exception cref="UnsupportedOperationException"> if this method is
		''' not overridden by a provider
		''' @since 1.4 </exception>
		Protected Friend Overridable Function engineGetParameters() As AlgorithmParameters
			Throw New UnsupportedOperationException
		End Function

		''' <summary>
		''' Gets the value of the specified algorithm parameter.
		''' This method supplies a general-purpose mechanism through which it
		''' is possible to get the various parameters of this object. A parameter
		''' may be any settable parameter for the algorithm, such as a parameter
		''' size, or  a source of random bits for signature generation (if
		''' appropriate), or an indication of whether or not to perform a
		''' specific but optional computation. A uniform algorithm-specific
		''' naming scheme for each parameter is desirable but left unspecified
		''' at this time.
		''' </summary>
		''' <param name="param"> the string name of the parameter.
		''' </param>
		''' <returns> the object that represents the parameter value, or null if
		''' there is none.
		''' </returns>
		''' <exception cref="InvalidParameterException"> if {@code param} is an
		''' invalid parameter for this engine, or another exception occurs while
		''' trying to get this parameter.
		''' 
		''' @deprecated </exception>
		<Obsolete> _
		Protected Friend MustOverride Function engineGetParameter(ByVal param As String) As Object

		''' <summary>
		''' Returns a clone if the implementation is cloneable.
		''' </summary>
		''' <returns> a clone if the implementation is cloneable.
		''' </returns>
		''' <exception cref="CloneNotSupportedException"> if this is called
		''' on an implementation that does not support {@code Cloneable}. </exception>
		Public Overridable Function clone() As Object
			If TypeOf Me Is Cloneable Then
				Return MyBase.clone()
			Else
				Throw New CloneNotSupportedException
			End If
		End Function
	End Class

End Namespace