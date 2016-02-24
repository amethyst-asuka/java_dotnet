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
	''' This class encapsulates information about a code signer.
	''' It is immutable.
	''' 
	''' @since 1.5
	''' @author Vincent Ryan
	''' </summary>

	<Serializable> _
	Public NotInheritable Class CodeSigner

		Private Const serialVersionUID As Long = 6819288105193937581L

		''' <summary>
		''' The signer's certificate path.
		''' 
		''' @serial
		''' </summary>
		Private signerCertPath As java.security.cert.CertPath

	'    
	'     * The signature timestamp.
	'     *
	'     * @serial
	'     
		Private timestamp As Timestamp

	'    
	'     * Hash code for this code signer.
	'     
		<NonSerialized> _
		Private myhash As Integer = -1

		''' <summary>
		''' Constructs a CodeSigner object.
		''' </summary>
		''' <param name="signerCertPath"> The signer's certificate path.
		'''                       It must not be {@code null}. </param>
		''' <param name="timestamp"> A signature timestamp.
		'''                  If {@code null} then no timestamp was generated
		'''                  for the signature. </param>
		''' <exception cref="NullPointerException"> if {@code signerCertPath} is
		'''                              {@code null}. </exception>
		Public Sub New(ByVal signerCertPath As java.security.cert.CertPath, ByVal timestamp As Timestamp)
			If signerCertPath Is Nothing Then Throw New NullPointerException
			Me.signerCertPath = signerCertPath
			Me.timestamp = timestamp
		End Sub

		''' <summary>
		''' Returns the signer's certificate path.
		''' </summary>
		''' <returns> A certificate path. </returns>
		Public Property signerCertPath As java.security.cert.CertPath
			Get
				Return signerCertPath
			End Get
		End Property

		''' <summary>
		''' Returns the signature timestamp.
		''' </summary>
		''' <returns> The timestamp or {@code null} if none is present. </returns>
		Public Property timestamp As Timestamp
			Get
				Return timestamp
			End Get
		End Property

		''' <summary>
		''' Returns the hash code value for this code signer.
		''' The hash code is generated using the signer's certificate path and the
		''' timestamp, if present.
		''' </summary>
		''' <returns> a hash code value for this code signer. </returns>
		Public Overrides Function GetHashCode() As Integer
			If myhash = -1 Then
				If timestamp Is Nothing Then
					myhash = signerCertPath.GetHashCode()
				Else
					myhash = signerCertPath.GetHashCode() + timestamp.GetHashCode()
				End If
			End If
			Return myhash
		End Function

		''' <summary>
		''' Tests for equality between the specified object and this
		''' code signer. Two code signers are considered equal if their
		''' signer certificate paths are equal and if their timestamps are equal,
		''' if present in both.
		''' </summary>
		''' <param name="obj"> the object to test for equality with this object.
		''' </param>
		''' <returns> true if the objects are considered equal, false otherwise. </returns>
		Public Overrides Function Equals(ByVal obj As Object) As Boolean
			If obj Is Nothing OrElse (Not(TypeOf obj Is CodeSigner)) Then Return False
			Dim that As CodeSigner = CType(obj, CodeSigner)

			If Me Is that Then Return True
			Dim thatTimestamp As Timestamp = that.timestamp
			If timestamp Is Nothing Then
				If thatTimestamp IsNot Nothing Then Return False
			Else
				If thatTimestamp Is Nothing OrElse ((Not timestamp.Equals(thatTimestamp))) Then Return False
			End If
			Return signerCertPath.Equals(that.signerCertPath)
		End Function

		''' <summary>
		''' Returns a string describing this code signer.
		''' </summary>
		''' <returns> A string comprising the signer's certificate and a timestamp,
		'''         if present. </returns>
		Public Overrides Function ToString() As String
			Dim sb As New StringBuffer
			sb.append("(")
			sb.append("Signer: " & signerCertPath.certificates(0))
			If timestamp IsNot Nothing Then sb.append("timestamp: " & timestamp)
			sb.append(")")
			Return sb.ToString()
		End Function

		' Explicitly reset hash code value to -1
		Private Sub readObject(ByVal ois As ObjectInputStream)
		 ois.defaultReadObject()
		 myhash = -1
		End Sub
	End Class

End Namespace