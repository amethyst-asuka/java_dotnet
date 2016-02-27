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
	''' for the {@code MessageDigest} [Class], which provides the functionality
	''' of a message digest algorithm, such as MD5 or SHA. Message digests are
	''' secure one-way hash functions that take arbitrary-sized data and output a
	''' fixed-length hash value.
	''' 
	''' <p> All the abstract methods in this class must be implemented by a
	''' cryptographic service provider who wishes to supply the implementation
	''' of a particular message digest algorithm.
	''' 
	''' <p> Implementations are free to implement the Cloneable interface.
	''' 
	''' @author Benjamin Renaud
	''' 
	''' </summary>
	''' <seealso cref= MessageDigest </seealso>

	Public MustInherit Class MessageDigestSpi

		' for re-use in engineUpdate(ByteBuffer input)
		Private tempArray As SByte()

		''' <summary>
		''' Returns the digest length in bytes.
		''' 
		''' <p>This concrete method has been added to this previously-defined
		''' abstract class. (For backwards compatibility, it cannot be abstract.)
		''' 
		''' <p>The default behavior is to return 0.
		''' 
		''' <p>This method may be overridden by a provider to return the digest
		''' length.
		''' </summary>
		''' <returns> the digest length in bytes.
		''' 
		''' @since 1.2 </returns>
		Protected Friend Overridable Function engineGetDigestLength() As Integer
			Return 0
		End Function

		''' <summary>
		''' Updates the digest using the specified java.lang.[Byte].
		''' </summary>
		''' <param name="input"> the byte to use for the update. </param>
		Protected Friend MustOverride Sub engineUpdate(ByVal input As SByte)

		''' <summary>
		''' Updates the digest using the specified array of bytes,
		''' starting at the specified offset.
		''' </summary>
		''' <param name="input"> the array of bytes to use for the update.
		''' </param>
		''' <param name="offset"> the offset to start from in the array of bytes.
		''' </param>
		''' <param name="len"> the number of bytes to use, starting at
		''' {@code offset}. </param>
		Protected Friend MustOverride Sub engineUpdate(ByVal input As SByte(), ByVal offset As Integer, ByVal len As Integer)

		''' <summary>
		''' Update the digest using the specified ByteBuffer. The digest is
		''' updated using the {@code input.remaining()} bytes starting
		''' at {@code input.position()}.
		''' Upon return, the buffer's position will be equal to its limit;
		''' its limit will not have changed.
		''' </summary>
		''' <param name="input"> the ByteBuffer
		''' @since 1.5 </param>
		Protected Friend Overridable Sub engineUpdate(ByVal input As java.nio.ByteBuffer)
			If input.hasRemaining() = False Then Return
			If input.hasArray() Then
				Dim b As SByte() = input.array()
				Dim ofs As Integer = input.arrayOffset()
				Dim pos As Integer = input.position()
				Dim lim As Integer = input.limit()
				engineUpdate(b, ofs + pos, lim - pos)
				input.position(lim)
			Else
				Dim len As Integer = input.remaining()
				Dim n As Integer = sun.security.jca.JCAUtil.getTempArraySize(len)
				If (tempArray Is Nothing) OrElse (n > tempArray.Length) Then tempArray = New SByte(n - 1){}
				Do While len > 0
					Dim chunk As Integer = System.Math.Min(len, tempArray.Length)
					input.get(tempArray, 0, chunk)
					engineUpdate(tempArray, 0, chunk)
					len -= chunk
				Loop
			End If
		End Sub

		''' <summary>
		''' Completes the hash computation by performing final
		''' operations such as padding. Once {@code engineDigest} has
		''' been called, the engine should be reset (see
		''' <seealso cref="#engineReset() engineReset"/>).
		''' Resetting is the responsibility of the
		''' engine implementor.
		''' </summary>
		''' <returns> the array of bytes for the resulting hash value. </returns>
		Protected Friend MustOverride Function engineDigest() As SByte()

		''' <summary>
		''' Completes the hash computation by performing final
		''' operations such as padding. Once {@code engineDigest} has
		''' been called, the engine should be reset (see
		''' <seealso cref="#engineReset() engineReset"/>).
		''' Resetting is the responsibility of the
		''' engine implementor.
		''' 
		''' This method should be abstract, but we leave it concrete for
		''' binary compatibility.  Knowledgeable providers should override this
		''' method.
		''' </summary>
		''' <param name="buf"> the output buffer in which to store the digest
		''' </param>
		''' <param name="offset"> offset to start from in the output buffer
		''' </param>
		''' <param name="len"> number of bytes within buf allotted for the digest.
		''' Both this default implementation and the SUN provider do not
		''' return partial digests.  The presence of this parameter is solely
		''' for consistency in our API's.  If the value of this parameter is less
		''' than the actual digest length, the method will throw a DigestException.
		''' This parameter is ignored if its value is greater than or equal to
		''' the actual digest length.
		''' </param>
		''' <returns> the length of the digest stored in the output buffer.
		''' </returns>
		''' <exception cref="DigestException"> if an error occurs.
		''' 
		''' @since 1.2 </exception>
		Protected Friend Overridable Function engineDigest(ByVal buf As SByte(), ByVal offset As Integer, ByVal len As Integer) As Integer

			Dim digest As SByte() = engineDigest()
			If len < digest.Length Then Throw New DigestException("partial digests not returned")
			If buf.Length - offset < digest.Length Then Throw New DigestException("insufficient space in the output " & "buffer to store the digest")
			Array.Copy(digest, 0, buf, offset, digest.Length)
			Return digest.Length
		End Function

		''' <summary>
		''' Resets the digest for further use.
		''' </summary>
		Protected Friend MustOverride Sub engineReset()

		''' <summary>
		''' Returns a clone if the implementation is cloneable.
		''' </summary>
		''' <returns> a clone if the implementation is cloneable.
		''' </returns>
		''' <exception cref="CloneNotSupportedException"> if this is called on an
		''' implementation that does not support {@code Cloneable}. </exception>
		Public Overridable Function clone() As Object
			If TypeOf Me Is Cloneable Then
				Return MyBase.clone()
			Else
				Throw New CloneNotSupportedException
			End If
		End Function
	End Class

End Namespace