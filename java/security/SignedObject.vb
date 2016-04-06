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
	''' <p> SignedObject is a class for the purpose of creating authentic
	''' runtime objects whose integrity cannot be compromised without being
	''' detected.
	''' 
	''' <p> More specifically, a SignedObject contains another Serializable
	''' object, the (to-be-)signed object and its signature.
	''' 
	''' <p> The signed object is a "deep copy" (in serialized form) of an
	''' original object.  Once the copy is made, further manipulation of
	''' the original object has no side effect on the copy.
	''' 
	''' <p> The underlying signing algorithm is designated by the Signature
	''' object passed to the constructor and the {@code verify} method.
	''' A typical usage for signing is the following:
	''' 
	''' <pre>{@code
	''' Signature signingEngine = Signature.getInstance(algorithm,
	'''                                                 provider);
	''' SignedObject so = new SignedObject(myobject, signingKey,
	'''                                    signingEngine);
	''' }</pre>
	''' 
	''' <p> A typical usage for verification is the following (having
	''' received SignedObject {@code so}):
	''' 
	''' <pre>{@code
	''' Signature verificationEngine =
	'''     Signature.getInstance(algorithm, provider);
	''' if (so.verify(publickey, verificationEngine))
	'''     try {
	'''         Object myobj = so.getObject();
	'''     } catch (java.lang.ClassNotFoundException e) {};
	''' }</pre>
	''' 
	''' <p> Several points are worth noting.  First, there is no need to
	''' initialize the signing or verification engine, as it will be
	''' re-initialized inside the constructor and the {@code verify}
	''' method. Secondly, for verification to succeed, the specified
	''' public key must be the public key corresponding to the private key
	''' used to generate the SignedObject.
	''' 
	''' <p> More importantly, for flexibility reasons, the
	''' constructor and {@code verify} method allow for
	''' customized signature engines, which can implement signature
	''' algorithms that are not installed formally as part of a crypto
	''' provider.  However, it is crucial that the programmer writing the
	''' verifier code be aware what {@code Signature} engine is being
	''' used, as its own implementation of the {@code verify} method
	''' is invoked to verify a signature.  In other words, a malicious
	''' {@code Signature} may choose to always return true on
	''' verification in an attempt to bypass a security check.
	''' 
	''' <p> The signature algorithm can be, among others, the NIST standard
	''' DSA, using DSA and SHA-1.  The algorithm is specified using the
	''' same convention as that for signatures. The DSA algorithm using the
	''' SHA-1 message digest algorithm can be specified, for example, as
	''' "SHA/DSA" or "SHA-1/DSA" (they are equivalent).  In the case of
	''' RSA, there are multiple choices for the message digest algorithm,
	''' so the signing algorithm could be specified as, for example,
	''' "MD2/RSA", "MD5/RSA" or "SHA-1/RSA".  The algorithm name must be
	''' specified, as there is no default.
	''' 
	''' <p> The name of the Cryptography Package Provider is designated
	''' also by the Signature parameter to the constructor and the
	''' {@code verify} method.  If the provider is not
	''' specified, the default provider is used.  Each installation can
	''' be configured to use a particular provider as default.
	''' 
	''' <p> Potential applications of SignedObject include:
	''' <ul>
	''' <li> It can be used
	''' internally to any Java runtime as an unforgeable authorization
	''' token -- one that can be passed around without the fear that the
	''' token can be maliciously modified without being detected.
	''' <li> It
	''' can be used to sign and serialize data/object for storage outside
	''' the Java runtime (e.g., storing critical access control data on
	''' disk).
	''' <li> Nested SignedObjects can be used to construct a logical
	''' sequence of signatures, resembling a chain of authorization and
	''' delegation.
	''' </ul>
	''' </summary>
	''' <seealso cref= Signature
	''' 
	''' @author Li Gong </seealso>

	<Serializable> _
	Public NotInheritable Class SignedObject

		Private Const serialVersionUID As Long = 720502720485447167L

	'    
	'     * The original content is "deep copied" in its serialized format
	'     * and stored in a byte array.  The signature field is also in the
	'     * form of byte array.
	'     

		Private content As SByte()
		Private signature_Renamed As SByte()
		Private thealgorithm As String

		''' <summary>
		''' Constructs a SignedObject from any Serializable object.
		''' The given object is signed with the given signing key, using the
		''' designated signature engine.
		''' </summary>
		''' <param name="object"> the object to be signed. </param>
		''' <param name="signingKey"> the private key for signing. </param>
		''' <param name="signingEngine"> the signature signing engine.
		''' </param>
		''' <exception cref="IOException"> if an error occurs during serialization </exception>
		''' <exception cref="InvalidKeyException"> if the key is invalid. </exception>
		''' <exception cref="SignatureException"> if signing fails. </exception>
		Public Sub New(  [object] As Serializable,   signingKey As PrivateKey,   signingEngine As Signature)
				' creating a stream pipe-line, from a to b
				Dim b As New ByteArrayOutputStream
				Dim a As ObjectOutput = New ObjectOutputStream(b)

				' write and flush the object content to byte array
				a.writeObject(object_Renamed)
				a.flush()
				a.close()
				Me.content = b.toByteArray()
				b.close()

				' now sign the encapsulated object
				Me.sign(signingKey, signingEngine)
		End Sub

		''' <summary>
		''' Retrieves the encapsulated object.
		''' The encapsulated object is de-serialized before it is returned.
		''' </summary>
		''' <returns> the encapsulated object.
		''' </returns>
		''' <exception cref="IOException"> if an error occurs during de-serialization </exception>
		''' <exception cref="ClassNotFoundException"> if an error occurs during
		''' de-serialization </exception>
		Public Property [object] As Object
			Get
				' creating a stream pipe-line, from b to a
				Dim b As New ByteArrayInputStream(Me.content)
				Dim a As ObjectInput = New ObjectInputStream(b)
				Dim obj As Object = a.readObject()
				b.close()
				a.close()
				Return obj
			End Get
		End Property

		''' <summary>
		''' Retrieves the signature on the signed object, in the form of a
		''' byte array.
		''' </summary>
		''' <returns> the signature. Returns a new array each time this
		''' method is called. </returns>
		Public Property signature As SByte()
			Get
				Return Me.signature_Renamed.clone()
			End Get
		End Property

		''' <summary>
		''' Retrieves the name of the signature algorithm.
		''' </summary>
		''' <returns> the signature algorithm name. </returns>
		Public Property algorithm As String
			Get
				Return Me.thealgorithm
			End Get
		End Property

		''' <summary>
		''' Verifies that the signature in this SignedObject is the valid
		''' signature for the object stored inside, with the given
		''' verification key, using the designated verification engine.
		''' </summary>
		''' <param name="verificationKey"> the public key for verification. </param>
		''' <param name="verificationEngine"> the signature verification engine.
		''' </param>
		''' <exception cref="SignatureException"> if signature verification failed. </exception>
		''' <exception cref="InvalidKeyException"> if the verification key is invalid.
		''' </exception>
		''' <returns> {@code true} if the signature
		''' is valid, {@code false} otherwise </returns>
		Public Function verify(  verificationKey As PublicKey,   verificationEngine As Signature) As Boolean
				 verificationEngine.initVerify(verificationKey)
				 verificationEngine.update(Me.content.clone())
				 Return verificationEngine.verify(Me.signature_Renamed.clone())
		End Function

	'    
	'     * Signs the encapsulated object with the given signing key, using the
	'     * designated signature engine.
	'     *
	'     * @param signingKey the private key for signing.
	'     * @param signingEngine the signature signing engine.
	'     *
	'     * @exception InvalidKeyException if the key is invalid.
	'     * @exception SignatureException if signing fails.
	'     
		Private Sub sign(  signingKey As PrivateKey,   signingEngine As Signature)
				' initialize the signing engine
				signingEngine.initSign(signingKey)
				signingEngine.update(Me.content.clone())
				Me.signature_Renamed = signingEngine.sign().clone()
				Me.thealgorithm = signingEngine.algorithm
		End Sub

		''' <summary>
		''' readObject is called to restore the state of the SignedObject from
		''' a stream.
		''' </summary>
		Private Sub readObject(  s As java.io.ObjectInputStream)
				Dim fields As java.io.ObjectInputStream.GetField = s.readFields()
				content = CType(fields.get("content", Nothing), SByte()).clone()
				signature_Renamed = CType(fields.get("signature", Nothing), SByte()).clone()
				thealgorithm = CStr(fields.get("thealgorithm", Nothing))
		End Sub
	End Class

End Namespace