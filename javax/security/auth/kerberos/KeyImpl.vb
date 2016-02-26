Imports Microsoft.VisualBasic
Imports System

'
' * Copyright (c) 2000, 2013, Oracle and/or its affiliates. All rights reserved.
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

Namespace javax.security.auth.kerberos


	''' <summary>
	''' This class encapsulates a Kerberos encryption key. It is not associated
	''' with a principal and may represent an ephemeral session key.
	''' 
	''' @author Mayank Upadhyay
	''' @since 1.4
	''' 
	''' @serial include
	''' </summary>
	<Serializable> _
	Friend Class KeyImpl
		Implements javax.crypto.SecretKey, javax.security.auth.Destroyable

		Private Const serialVersionUID As Long = -7889313790214321193L

		<NonSerialized> _
		Private keyBytes As SByte()
		<NonSerialized> _
		Private keyType As Integer
'JAVA TO VB CONVERTER TODO TASK: There is no VB equivalent to 'volatile':
		<NonSerialized> _
		Private destroyed As Boolean = False


		''' <summary>
		''' Constructs a KeyImpl from the given bytes.
		''' </summary>
		''' <param name="keyBytes"> the raw bytes for the secret key </param>
		''' <param name="keyType"> the key type for the secret key as defined by the
		''' Kerberos protocol specification. </param>
		Public Sub New(ByVal keyBytes As SByte(), ByVal keyType As Integer)
			Me.keyBytes = keyBytes.clone()
			Me.keyType = keyType
		End Sub

		''' <summary>
		''' Constructs a KeyImpl from a password.
		''' </summary>
		''' <param name="principal"> the principal from which to derive the salt </param>
		''' <param name="password"> the password that should be used to compute the
		''' key. </param>
		''' <param name="algorithm"> the name for the algorithm that this key wil be
		''' used for. This parameter may be null in which case "DES" will be
		''' assumed. </param>
		Public Sub New(ByVal principal As KerberosPrincipal, ByVal password As Char(), ByVal algorithm As String)

			Try
				Dim princ As New sun.security.krb5.PrincipalName(principal.name)
				Dim key As New sun.security.krb5.EncryptionKey(password, princ.salt, algorithm)
				Me.keyBytes = key.bytes
				Me.keyType = key.eType
			Catch e As sun.security.krb5.KrbException
				Throw New System.ArgumentException(e.Message)
			End Try
		End Sub

		''' <summary>
		''' Returns the keyType for this key as defined in the Kerberos Spec.
		''' </summary>
		Public Property keyType As Integer
			Get
				If destroyed Then Throw New IllegalStateException("This key is no longer valid")
				Return keyType
			End Get
		End Property

	'    
	'     * Methods from java.security.Key
	'     

		Public Property algorithm As String
			Get
				Return getAlgorithmName(keyType)
			End Get
		End Property

		Private Function getAlgorithmName(ByVal eType As Integer) As String
			If destroyed Then Throw New IllegalStateException("This key is no longer valid")

			Select Case eType
			Case sun.security.krb5.EncryptedData.ETYPE_DES_CBC_CRC, EncryptedData.ETYPE_DES_CBC_MD5
				Return "DES"

			Case sun.security.krb5.EncryptedData.ETYPE_DES3_CBC_HMAC_SHA1_KD
				Return "DESede"

			Case sun.security.krb5.EncryptedData.ETYPE_ARCFOUR_HMAC
				Return "ArcFourHmac"

			Case sun.security.krb5.EncryptedData.ETYPE_AES128_CTS_HMAC_SHA1_96
				Return "AES128"

			Case sun.security.krb5.EncryptedData.ETYPE_AES256_CTS_HMAC_SHA1_96
				Return "AES256"

			Case sun.security.krb5.EncryptedData.ETYPE_NULL
				Return "NULL"

			Case Else
				Throw New System.ArgumentException("Unsupported encryption type: " & eType)
			End Select
		End Function

		Public Property format As String
			Get
				If destroyed Then Throw New IllegalStateException("This key is no longer valid")
				Return "RAW"
			End Get
		End Property

		Public Property encoded As SByte()
			Get
				If destroyed Then Throw New IllegalStateException("This key is no longer valid")
				Return keyBytes.clone()
			End Get
		End Property

		Public Overridable Sub destroy()
			If Not destroyed Then
				destroyed = True
				java.util.Arrays.fill(keyBytes, CByte(0))
			End If
		End Sub

		Public Overridable Property destroyed As Boolean
			Get
				Return destroyed
			End Get
		End Property

		''' <summary>
		''' @serialData this {@code KeyImpl} is serialized by
		''' writing out the ASN1 Encoded bytes of the encryption key.
		''' The ASN1 encoding is defined in RFC4120 and as  follows:
		''' EncryptionKey   ::= SEQUENCE {
		'''          keytype    [0] Int32 -- actually encryption type --,
		'''          keyvalue   [1] OCTET STRING
		''' }
		''' </summary>
		Private Sub writeObject(ByVal ois As ObjectOutputStream)
			If destroyed Then Throw New IOException("This key is no longer valid")

			Try
			   ois.writeObject((New sun.security.krb5.EncryptionKey(keyType, keyBytes)).asn1Encode())
			Catch ae As sun.security.krb5.Asn1Exception
			   Throw New IOException(ae.Message)
			End Try
		End Sub

		Private Sub readObject(ByVal ois As ObjectInputStream)
			Try
				Dim encKey As New sun.security.krb5.EncryptionKey(New sun.security.util.DerValue(CType(ois.readObject(), SByte())))
				keyType = encKey.eType
				keyBytes = encKey.bytes
			Catch ae As sun.security.krb5.Asn1Exception
				Throw New IOException(ae.Message)
			End Try
		End Sub

		Public Overrides Function ToString() As String
			Dim hd As New sun.misc.HexDumpEncoder
			Return "EncryptionKey: keyType=" & keyType & " keyBytes (hex dump)=" & (If(keyBytes Is Nothing OrElse keyBytes.Length = 0, " Empty Key", ControlChars.Lf & hd.encodeBuffer(keyBytes) & ControlChars.Lf))


		End Function

		Public Overrides Function GetHashCode() As Integer
			Dim result As Integer = 17
			If destroyed Then Return result
			result = 37 * result + java.util.Arrays.hashCode(keyBytes)
			Return 37 * result + keyType
		End Function

		Public Overrides Function Equals(ByVal other As Object) As Boolean

			If other Is Me Then Return True

			If Not(TypeOf other Is KeyImpl) Then Return False

			Dim otherKey As KeyImpl = (CType(other, KeyImpl))
			If destroyed OrElse otherKey.destroyed Then Return False

			If keyType <> otherKey.keyType OrElse (Not java.util.Arrays.Equals(keyBytes, otherKey.encoded)) Then Return False

			Return True
		End Function
	End Class

End Namespace