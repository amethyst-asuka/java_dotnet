'
' * Copyright (c) 1996, 2015, Oracle and/or its affiliates. All rights reserved.
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
	''' This MessageDigest class provides applications the functionality of a
	''' message digest algorithm, such as SHA-1 or SHA-256.
	''' Message digests are secure one-way hash functions that take arbitrary-sized
	''' data and output a fixed-length hash value.
	''' 
	''' <p>A MessageDigest object starts out initialized. The data is
	''' processed through it using the <seealso cref="#update(byte) update"/>
	''' methods. At any point <seealso cref="#reset() reset"/> can be called
	''' to reset the digest. Once all the data to be updated has been
	''' updated, one of the <seealso cref="#digest() digest"/> methods should
	''' be called to complete the hash computation.
	''' 
	''' <p>The {@code digest} method can be called once for a given number
	''' of updates. After {@code digest} has been called, the MessageDigest
	''' object is reset to its initialized state.
	''' 
	''' <p>Implementations are free to implement the Cloneable interface.
	''' Client applications can test cloneability by attempting cloning
	''' and catching the CloneNotSupportedException:
	''' 
	''' <pre>{@code
	''' MessageDigest md = MessageDigest.getInstance("SHA");
	''' 
	''' try {
	'''     md.update(toChapter1);
	'''     MessageDigest tc1 = md.clone();
	'''     byte[] toChapter1Digest = tc1.digest();
	'''     md.update(toChapter2);
	'''     ...etc.
	''' } catch (CloneNotSupportedException cnse) {
	'''     throw new DigestException("couldn't make digest of partial content");
	''' }
	''' }</pre>
	''' 
	''' <p>Note that if a given implementation is not cloneable, it is
	''' still possible to compute intermediate digests by instantiating
	''' several instances, if the number of digests is known in advance.
	''' 
	''' <p>Note that this class is abstract and extends from
	''' {@code MessageDigestSpi} for historical reasons.
	''' Application developers should only take notice of the methods defined in
	''' this {@code MessageDigest} class; all the methods in
	''' the superclass are intended for cryptographic service providers who wish to
	''' supply their own implementations of message digest algorithms.
	''' 
	''' <p> Every implementation of the Java platform is required to support
	''' the following standard {@code MessageDigest} algorithms:
	''' <ul>
	''' <li>{@code MD5}</li>
	''' <li>{@code SHA-1}</li>
	''' <li>{@code SHA-256}</li>
	''' </ul>
	''' These algorithms are described in the <a href=
	''' "{@docRoot}/../technotes/guides/security/StandardNames.html#MessageDigest">
	''' MessageDigest section</a> of the
	''' Java Cryptography Architecture Standard Algorithm Name Documentation.
	''' Consult the release documentation for your implementation to see if any
	''' other algorithms are supported.
	''' 
	''' @author Benjamin Renaud
	''' </summary>
	''' <seealso cref= DigestInputStream </seealso>
	''' <seealso cref= DigestOutputStream </seealso>

	Public MustInherit Class MessageDigest
		Inherits MessageDigestSpi

		Private Shared ReadOnly pdebug As sun.security.util.Debug = sun.security.util.Debug.getInstance("provider", "Provider")
		Private Shared ReadOnly skipDebug As Boolean = sun.security.util.Debug.isOn("engine=") AndAlso Not sun.security.util.Debug.isOn("messagedigest")

		Private algorithm As String

		' The state of this digest
		Private Const INITIAL As Integer = 0
		Private Const IN_PROGRESS As Integer = 1
		Private state As Integer = INITIAL

		' The provider
		Private provider As Provider

		''' <summary>
		''' Creates a message digest with the specified algorithm name.
		''' </summary>
		''' <param name="algorithm"> the standard name of the digest algorithm.
		''' See the MessageDigest section in the <a href=
		''' "{@docRoot}/../technotes/guides/security/StandardNames.html#MessageDigest">
		''' Java Cryptography Architecture Standard Algorithm Name Documentation</a>
		''' for information about standard algorithm names. </param>
		Protected Friend Sub New(  algorithm As String)
			Me.algorithm = algorithm
		End Sub

		''' <summary>
		''' Returns a MessageDigest object that implements the specified digest
		''' algorithm.
		''' 
		''' <p> This method traverses the list of registered security Providers,
		''' starting with the most preferred Provider.
		''' A new MessageDigest object encapsulating the
		''' MessageDigestSpi implementation from the first
		''' Provider that supports the specified algorithm is returned.
		''' 
		''' <p> Note that the list of registered providers may be retrieved via
		''' the <seealso cref="Security#getProviders() Security.getProviders()"/> method.
		''' </summary>
		''' <param name="algorithm"> the name of the algorithm requested.
		''' See the MessageDigest section in the <a href=
		''' "{@docRoot}/../technotes/guides/security/StandardNames.html#MessageDigest">
		''' Java Cryptography Architecture Standard Algorithm Name Documentation</a>
		''' for information about standard algorithm names.
		''' </param>
		''' <returns> a Message Digest object that implements the specified algorithm.
		''' </returns>
		''' <exception cref="NoSuchAlgorithmException"> if no Provider supports a
		'''          MessageDigestSpi implementation for the
		'''          specified algorithm.
		''' </exception>
		''' <seealso cref= Provider </seealso>
		Public Shared Function getInstance(  algorithm As String) As MessageDigest
			Try
				Dim md As MessageDigest
				Dim objs As Object() = Security.getImpl(algorithm, "MessageDigest", CStr(Nothing))
				If TypeOf objs(0) Is MessageDigest Then
					md = CType(objs(0), MessageDigest)
				Else
					md = New [Delegate](CType(objs(0), MessageDigestSpi), algorithm)
				End If
				md.provider = CType(objs(1), Provider)

				If (Not skipDebug) AndAlso pdebug IsNot Nothing Then pdebug.println("MessageDigest." & algorithm & " algorithm from: " & md.provider.name)

				Return md

			Catch e As NoSuchProviderException
				Throw New NoSuchAlgorithmException(algorithm & " not found")
			End Try
		End Function

		''' <summary>
		''' Returns a MessageDigest object that implements the specified digest
		''' algorithm.
		''' 
		''' <p> A new MessageDigest object encapsulating the
		''' MessageDigestSpi implementation from the specified provider
		''' is returned.  The specified provider must be registered
		''' in the security provider list.
		''' 
		''' <p> Note that the list of registered providers may be retrieved via
		''' the <seealso cref="Security#getProviders() Security.getProviders()"/> method.
		''' </summary>
		''' <param name="algorithm"> the name of the algorithm requested.
		''' See the MessageDigest section in the <a href=
		''' "{@docRoot}/../technotes/guides/security/StandardNames.html#MessageDigest">
		''' Java Cryptography Architecture Standard Algorithm Name Documentation</a>
		''' for information about standard algorithm names.
		''' </param>
		''' <param name="provider"> the name of the provider.
		''' </param>
		''' <returns> a MessageDigest object that implements the specified algorithm.
		''' </returns>
		''' <exception cref="NoSuchAlgorithmException"> if a MessageDigestSpi
		'''          implementation for the specified algorithm is not
		'''          available from the specified provider.
		''' </exception>
		''' <exception cref="NoSuchProviderException"> if the specified provider is not
		'''          registered in the security provider list.
		''' </exception>
		''' <exception cref="IllegalArgumentException"> if the provider name is null
		'''          or empty.
		''' </exception>
		''' <seealso cref= Provider </seealso>
		Public Shared Function getInstance(  algorithm As String,   provider_Renamed As String) As MessageDigest
			If provider_Renamed Is Nothing OrElse provider_Renamed.length() = 0 Then Throw New IllegalArgumentException("missing provider")
			Dim objs As Object() = Security.getImpl(algorithm, "MessageDigest", provider_Renamed)
			If TypeOf objs(0) Is MessageDigest Then
				Dim md As MessageDigest = CType(objs(0), MessageDigest)
				md.provider = CType(objs(1), Provider)
				Return md
			Else
				Dim [delegate] As MessageDigest = New [Delegate](CType(objs(0), MessageDigestSpi), algorithm)
				[delegate].provider = CType(objs(1), Provider)
				Return [delegate]
			End If
		End Function

		''' <summary>
		''' Returns a MessageDigest object that implements the specified digest
		''' algorithm.
		''' 
		''' <p> A new MessageDigest object encapsulating the
		''' MessageDigestSpi implementation from the specified Provider
		''' object is returned.  Note that the specified Provider object
		''' does not have to be registered in the provider list.
		''' </summary>
		''' <param name="algorithm"> the name of the algorithm requested.
		''' See the MessageDigest section in the <a href=
		''' "{@docRoot}/../technotes/guides/security/StandardNames.html#MessageDigest">
		''' Java Cryptography Architecture Standard Algorithm Name Documentation</a>
		''' for information about standard algorithm names.
		''' </param>
		''' <param name="provider"> the provider.
		''' </param>
		''' <returns> a MessageDigest object that implements the specified algorithm.
		''' </returns>
		''' <exception cref="NoSuchAlgorithmException"> if a MessageDigestSpi
		'''          implementation for the specified algorithm is not available
		'''          from the specified Provider object.
		''' </exception>
		''' <exception cref="IllegalArgumentException"> if the specified provider is null.
		''' </exception>
		''' <seealso cref= Provider
		''' 
		''' @since 1.4 </seealso>
		Public Shared Function getInstance(  algorithm As String,   provider_Renamed As Provider) As MessageDigest
			If provider_Renamed Is Nothing Then Throw New IllegalArgumentException("missing provider")
			Dim objs As Object() = Security.getImpl(algorithm, "MessageDigest", provider_Renamed)
			If TypeOf objs(0) Is MessageDigest Then
				Dim md As MessageDigest = CType(objs(0), MessageDigest)
				md.provider = CType(objs(1), Provider)
				Return md
			Else
				Dim [delegate] As MessageDigest = New [Delegate](CType(objs(0), MessageDigestSpi), algorithm)
				[delegate].provider = CType(objs(1), Provider)
				Return [delegate]
			End If
		End Function

		''' <summary>
		''' Returns the provider of this message digest object.
		''' </summary>
		''' <returns> the provider of this message digest object </returns>
		Public Property provider As Provider
			Get
				Return Me.provider
			End Get
		End Property

		''' <summary>
		''' Updates the digest using the specified java.lang.[Byte].
		''' </summary>
		''' <param name="input"> the byte with which to update the digest. </param>
		Public Overridable Sub update(  input As SByte)
			engineUpdate(input)
			state = IN_PROGRESS
		End Sub

		''' <summary>
		''' Updates the digest using the specified array of bytes, starting
		''' at the specified offset.
		''' </summary>
		''' <param name="input"> the array of bytes.
		''' </param>
		''' <param name="offset"> the offset to start from in the array of bytes.
		''' </param>
		''' <param name="len"> the number of bytes to use, starting at
		''' {@code offset}. </param>
		Public Overridable Sub update(  input As SByte(),   offset As Integer,   len As Integer)
			If input Is Nothing Then Throw New IllegalArgumentException("No input buffer given")
			If input.Length - offset < len Then Throw New IllegalArgumentException("Input buffer too short")
			engineUpdate(input, offset, len)
			state = IN_PROGRESS
		End Sub

		''' <summary>
		''' Updates the digest using the specified array of bytes.
		''' </summary>
		''' <param name="input"> the array of bytes. </param>
		Public Overridable Sub update(  input As SByte())
			engineUpdate(input, 0, input.Length)
			state = IN_PROGRESS
		End Sub

		''' <summary>
		''' Update the digest using the specified ByteBuffer. The digest is
		''' updated using the {@code input.remaining()} bytes starting
		''' at {@code input.position()}.
		''' Upon return, the buffer's position will be equal to its limit;
		''' its limit will not have changed.
		''' </summary>
		''' <param name="input"> the ByteBuffer
		''' @since 1.5 </param>
		Public Sub update(  input As java.nio.ByteBuffer)
			If input Is Nothing Then Throw New NullPointerException
			engineUpdate(input)
			state = IN_PROGRESS
		End Sub

		''' <summary>
		''' Completes the hash computation by performing final operations
		''' such as padding. The digest is reset after this call is made.
		''' </summary>
		''' <returns> the array of bytes for the resulting hash value. </returns>
		Public Overridable Function digest() As SByte()
			' Resetting is the responsibility of implementors. 
			Dim result As SByte() = engineDigest()
			state = INITIAL
			Return result
		End Function

		''' <summary>
		''' Completes the hash computation by performing final operations
		''' such as padding. The digest is reset after this call is made.
		''' </summary>
		''' <param name="buf"> output buffer for the computed digest
		''' </param>
		''' <param name="offset"> offset into the output buffer to begin storing the digest
		''' </param>
		''' <param name="len"> number of bytes within buf allotted for the digest
		''' </param>
		''' <returns> the number of bytes placed into {@code buf}
		''' </returns>
		''' <exception cref="DigestException"> if an error occurs. </exception>
		Public Overridable Function digest(  buf As SByte(),   offset As Integer,   len As Integer) As Integer
			If buf Is Nothing Then Throw New IllegalArgumentException("No output buffer given")
			If buf.Length - offset < len Then Throw New IllegalArgumentException("Output buffer too small for specified offset and length")
			Dim numBytes As Integer = engineDigest(buf, offset, len)
			state = INITIAL
			Return numBytes
		End Function

		''' <summary>
		''' Performs a final update on the digest using the specified array
		''' of bytes, then completes the digest computation. That is, this
		''' method first calls <seealso cref="#update(byte[]) update(input)"/>,
		''' passing the <i>input</i> array to the {@code update} method,
		''' then calls <seealso cref="#digest() digest()"/>.
		''' </summary>
		''' <param name="input"> the input to be updated before the digest is
		''' completed.
		''' </param>
		''' <returns> the array of bytes for the resulting hash value. </returns>
		Public Overridable Function digest(  input As SByte()) As SByte()
			update(input)
			Return digest()
		End Function

		''' <summary>
		''' Returns a string representation of this message digest object.
		''' </summary>
		Public Overrides Function ToString() As String
			Dim baos As New java.io.ByteArrayOutputStream
			Dim p As New java.io.PrintStream(baos)
			p.print(algorithm & " Message Digest from " & provider.name & ", ")
			Select Case state
			Case INITIAL
				p.print("<initialized>")
			Case IN_PROGRESS
				p.print("<in progress>")
			End Select
			p.println()
			Return (baos.ToString())
		End Function

		''' <summary>
		''' Compares two digests for equality. Does a simple byte compare.
		''' </summary>
		''' <param name="digesta"> one of the digests to compare.
		''' </param>
		''' <param name="digestb"> the other digest to compare.
		''' </param>
		''' <returns> true if the digests are equal, false otherwise. </returns>
		Public Shared Function isEqual(  digesta As SByte(),   digestb As SByte()) As Boolean
			If digesta = digestb Then Return True
			If digesta Is Nothing OrElse digestb Is Nothing Then Return False
			If digesta.Length <> digestb.Length Then Return False

			Dim result As Integer = 0
			' time-constant comparison
			For i As Integer = 0 To digesta.Length - 1
				result = result Or digesta(i) Xor digestb(i)
			Next i
			Return result = 0
		End Function

		''' <summary>
		''' Resets the digest for further use.
		''' </summary>
		Public Overridable Sub reset()
			engineReset()
			state = INITIAL
		End Sub

		''' <summary>
		''' Returns a string that identifies the algorithm, independent of
		''' implementation details. The name should be a standard
		''' Java Security name (such as "SHA", "MD5", and so on).
		''' See the MessageDigest section in the <a href=
		''' "{@docRoot}/../technotes/guides/security/StandardNames.html#MessageDigest">
		''' Java Cryptography Architecture Standard Algorithm Name Documentation</a>
		''' for information about standard algorithm names.
		''' </summary>
		''' <returns> the name of the algorithm </returns>
		Public Property algorithm As String
			Get
				Return Me.algorithm
			End Get
		End Property

		''' <summary>
		''' Returns the length of the digest in bytes, or 0 if this operation is
		''' not supported by the provider and the implementation is not cloneable.
		''' </summary>
		''' <returns> the digest length in bytes, or 0 if this operation is not
		''' supported by the provider and the implementation is not cloneable.
		''' 
		''' @since 1.2 </returns>
		Public Property digestLength As Integer
			Get
				Dim digestLen As Integer = engineGetDigestLength()
				If digestLen = 0 Then
					Try
						Dim md As MessageDigest = CType(clone(), MessageDigest)
						Dim digest As SByte() = md.digest()
						Return digest.Length
					Catch e As CloneNotSupportedException
						Return digestLen
					End Try
				End If
				Return digestLen
			End Get
		End Property

		''' <summary>
		''' Returns a clone if the implementation is cloneable.
		''' </summary>
		''' <returns> a clone if the implementation is cloneable.
		''' </returns>
		''' <exception cref="CloneNotSupportedException"> if this is called on an
		''' implementation that does not support {@code Cloneable}. </exception>
		Public Overrides Function clone() As Object
			If TypeOf Me Is Cloneable Then
				Return MyBase.clone()
			Else
				Throw New CloneNotSupportedException
			End If
		End Function




	'    
	'     * The following class allows providers to extend from MessageDigestSpi
	'     * rather than from MessageDigest. It represents a MessageDigest with an
	'     * encapsulated, provider-supplied SPI object (of type MessageDigestSpi).
	'     * If the provider implementation is an instance of MessageDigestSpi,
	'     * the getInstance() methods above return an instance of this [Class], with
	'     * the SPI object encapsulated.
	'     *
	'     * Note: All SPI methods from the original MessageDigest class have been
	'     * moved up the hierarchy into a new class (MessageDigestSpi), which has
	'     * been interposed in the hierarchy between the API (MessageDigest)
	'     * and its original parent (Object).
	'     

		Friend Class [Delegate]
			Inherits MessageDigest

			' The provider implementation (delegate)
			Private digestSpi As MessageDigestSpi

			' constructor
			Public Sub New(  digestSpi As MessageDigestSpi,   algorithm As String)
				MyBase.New(algorithm)
				Me.digestSpi = digestSpi
			End Sub

			''' <summary>
			''' Returns a clone if the delegate is cloneable.
			''' </summary>
			''' <returns> a clone if the delegate is cloneable.
			''' </returns>
			''' <exception cref="CloneNotSupportedException"> if this is called on a
			''' delegate that does not support {@code Cloneable}. </exception>
			Public Overrides Function clone() As Object
				If TypeOf digestSpi Is Cloneable Then
					Dim digestSpiClone As MessageDigestSpi = CType(digestSpi.clone(), MessageDigestSpi)
					' Because 'algorithm', 'provider', and 'state' are private
					' members of our supertype, we must perform a cast to
					' access them.
					Dim that As MessageDigest = New [Delegate](digestSpiClone, CType(Me, MessageDigest).algorithm)
					that.provider = CType(Me, MessageDigest).provider
					that.state = CType(Me, MessageDigest).state
					Return that
				Else
					Throw New CloneNotSupportedException
				End If
			End Function

			Protected Friend Overrides Function engineGetDigestLength() As Integer
				Return digestSpi.engineGetDigestLength()
			End Function

			Protected Friend Overrides Sub engineUpdate(  input As SByte)
				digestSpi.engineUpdate(input)
			End Sub

			Protected Friend Overrides Sub engineUpdate(  input As SByte(),   offset As Integer,   len As Integer)
				digestSpi.engineUpdate(input, offset, len)
			End Sub

			Protected Friend Overrides Sub engineUpdate(  input As java.nio.ByteBuffer)
				digestSpi.engineUpdate(input)
			End Sub

			Protected Friend Overrides Function engineDigest() As SByte()
				Return digestSpi.engineDigest()
			End Function

			Protected Friend Overrides Function engineDigest(  buf As SByte(),   offset As Integer,   len As Integer) As Integer
					Return digestSpi.engineDigest(buf, offset, len)
			End Function

			Protected Friend Overrides Sub engineReset()
				digestSpi.engineReset()
			End Sub
		End Class
	End Class

End Namespace