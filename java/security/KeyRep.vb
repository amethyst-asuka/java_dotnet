Imports System

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

Namespace java.security




	''' <summary>
	''' Standardized representation for serialized Key objects.
	''' 
	''' <p>
	''' 
	''' Note that a serialized Key may contain sensitive information
	''' which should not be exposed in untrusted environments.  See the
	''' <a href="../../../platform/serialization/spec/security.html">
	''' Security Appendix</a>
	''' of the Serialization Specification for more information.
	''' </summary>
	''' <seealso cref= Key </seealso>
	''' <seealso cref= KeyFactory </seealso>
	''' <seealso cref= javax.crypto.spec.SecretKeySpec </seealso>
	''' <seealso cref= java.security.spec.X509EncodedKeySpec </seealso>
	''' <seealso cref= java.security.spec.PKCS8EncodedKeySpec
	''' 
	''' @since 1.5 </seealso>

	<Serializable> _
	Public Class KeyRep

		Private Const serialVersionUID As Long = -4757683898830641853L

		''' <summary>
		''' Key type.
		''' 
		''' @since 1.5
		''' </summary>
		Public Enum Type

			''' <summary>
			''' Type for secret keys. </summary>
			SECRET

			''' <summary>
			''' Type for public keys. </summary>
			[PUBLIC]

			''' <summary>
			''' Type for private keys. </summary>
			[PRIVATE]

		End Enum

		Private Const PKCS8 As String = "PKCS#8"
		Private Const X509 As String = "X.509"
		Private Const RAW As String = "RAW"

		''' <summary>
		''' Either one of Type.SECRET, Type.PUBLIC, or Type.PRIVATE
		''' 
		''' @serial
		''' </summary>
		Private type As Type

		''' <summary>
		''' The Key algorithm
		''' 
		''' @serial
		''' </summary>
		Private algorithm As String

		''' <summary>
		''' The Key encoding format
		''' 
		''' @serial
		''' </summary>
		Private format As String

		''' <summary>
		''' The encoded Key bytes
		''' 
		''' @serial
		''' </summary>
		Private encoded As SByte()

		''' <summary>
		''' Construct the alternate Key class.
		''' 
		''' <p>
		''' </summary>
		''' <param name="type"> either one of Type.SECRET, Type.PUBLIC, or Type.PRIVATE </param>
		''' <param name="algorithm"> the algorithm returned from
		'''          {@code Key.getAlgorithm()} </param>
		''' <param name="format"> the encoding format returned from
		'''          {@code Key.getFormat()} </param>
		''' <param name="encoded"> the encoded bytes returned from
		'''          {@code Key.getEncoded()}
		''' </param>
		''' <exception cref="NullPointerException">
		'''          if type is {@code null},
		'''          if algorithm is {@code null},
		'''          if format is {@code null},
		'''          or if encoded is {@code null} </exception>
		Public Sub New(ByVal type As Type, ByVal algorithm As String, ByVal format As String, ByVal encoded As SByte())

			If type Is Nothing OrElse algorithm Is Nothing OrElse format Is Nothing OrElse encoded Is Nothing Then Throw New NullPointerException("invalid null input(s)")

			Me.type = type
			Me.algorithm = algorithm
			Me.format = format.ToUpper(java.util.Locale.ENGLISH)
			Me.encoded = encoded.clone()
		End Sub

		''' <summary>
		''' Resolve the Key object.
		''' 
		''' <p> This method supports three Type/format combinations:
		''' <ul>
		''' <li> Type.SECRET/"RAW" - returns a SecretKeySpec object
		''' constructed using encoded key bytes and algorithm
		''' <li> Type.PUBLIC/"X.509" - gets a KeyFactory instance for
		''' the key algorithm, constructs an X509EncodedKeySpec with the
		''' encoded key bytes, and generates a public key from the spec
		''' <li> Type.PRIVATE/"PKCS#8" - gets a KeyFactory instance for
		''' the key algorithm, constructs a PKCS8EncodedKeySpec with the
		''' encoded key bytes, and generates a private key from the spec
		''' </ul>
		''' 
		''' <p>
		''' </summary>
		''' <returns> the resolved Key object
		''' </returns>
		''' <exception cref="ObjectStreamException"> if the Type/format
		'''  combination is unrecognized, if the algorithm, key format, or
		'''  encoded key bytes are unrecognized/invalid, of if the
		'''  resolution of the key fails for any reason </exception>
		Protected Friend Overridable Function readResolve() As Object
			Try
				If type = Type.SECRET AndAlso RAW.Equals(format) Then
					Return New javax.crypto.spec.SecretKeySpec(encoded, algorithm)
				ElseIf type = Type.PUBLIC AndAlso X509.Equals(format) Then
					Dim f As KeyFactory = KeyFactory.getInstance(algorithm)
					Return f.generatePublic(New java.security.spec.X509EncodedKeySpec(encoded))
				ElseIf type = Type.PRIVATE AndAlso PKCS8.Equals(format) Then
					Dim f As KeyFactory = KeyFactory.getInstance(algorithm)
					Return f.generatePrivate(New java.security.spec.PKCS8EncodedKeySpec(encoded))
				Else
					Throw New NotSerializableException("unrecognized type/format combination: " & type & "/" & format)
				End If
			Catch nse As NotSerializableException
				Throw nse
			Catch e As Exception
				Dim nse As New NotSerializableException("java.security.Key: " & "[" & type & "] " & "[" & algorithm & "] " & "[" & format & "]")
				nse.initCause(e)
				Throw nse
			End Try
		End Function
	End Class

End Namespace