Imports System
Imports System.Collections.Generic

'
' * Copyright (c) 2003, 2011, Oracle and/or its affiliates. All rights reserved.
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
    ''' This class encapsulates information about a signed timestamp.
    ''' It is immutable.
    ''' It includes the timestamp's date and time as well as information about the
    ''' Timestamping Authority (TSA) which generated and signed the timestamp.
    ''' 
    ''' @since 1.5
    ''' @author Vincent Ryan
    ''' </summary>

    <Serializable>
    Public NotInheritable Class Timestamp : Inherits java.lang.Object

        Private Const serialVersionUID As Long = -5502683707821851294L

		''' <summary>
		''' The timestamp's date and time
		''' 
		''' @serial
		''' </summary>
		Private timestamp_Renamed As DateTime?

		''' <summary>
		''' The TSA's certificate path.
		''' 
		''' @serial
		''' </summary>
		Private signerCertPath As java.security.cert.CertPath

	'    
	'     * Hash code for this timestamp.
	'     
		<NonSerialized> _
		Private myhash As Integer = -1

		''' <summary>
		''' Constructs a Timestamp.
		''' </summary>
		''' <param name="timestamp"> is the timestamp's date and time. It must not be null. </param>
		''' <param name="signerCertPath"> is the TSA's certificate path. It must not be null. </param>
		''' <exception cref="NullPointerException"> if timestamp or signerCertPath is null. </exception>
		Public Sub New(ByVal timestamp As DateTime?, ByVal signerCertPath As java.security.cert.CertPath)
			If timestamp Is Nothing OrElse signerCertPath Is Nothing Then Throw New NullPointerException
			Me.timestamp_Renamed = New DateTime?(timestamp.Value.time) ' clone
			Me.signerCertPath = signerCertPath
		End Sub

		''' <summary>
		''' Returns the date and time when the timestamp was generated.
		''' </summary>
		''' <returns> The timestamp's date and time. </returns>
		Public Property timestamp As DateTime?
			Get
				Return New DateTime?(timestamp_Renamed.Value.time) ' clone
			End Get
		End Property

		''' <summary>
		''' Returns the certificate path for the Timestamping Authority.
		''' </summary>
		''' <returns> The TSA's certificate path. </returns>
		Public Property signerCertPath As java.security.cert.CertPath
			Get
				Return signerCertPath
			End Get
		End Property

		''' <summary>
		''' Returns the hash code value for this timestamp.
		''' The hash code is generated using the date and time of the timestamp
		''' and the TSA's certificate path.
		''' </summary>
		''' <returns> a hash code value for this timestamp. </returns>
		Public Overrides Function GetHashCode() As Integer
			If myhash = -1 Then myhash = timestamp_Renamed.Value.GetHashCode() + signerCertPath.GetHashCode()
			Return myhash
		End Function

		''' <summary>
		''' Tests for equality between the specified object and this
		''' timestamp. Two timestamps are considered equal if the date and time of
		''' their timestamp's and their signer's certificate paths are equal.
		''' </summary>
		''' <param name="obj"> the object to test for equality with this timestamp.
		''' </param>
		''' <returns> true if the timestamp are considered equal, false otherwise. </returns>
		Public Overrides Function Equals(ByVal obj As Object) As Boolean
			If obj Is Nothing OrElse (Not(TypeOf obj Is Timestamp)) Then Return False
			Dim that As Timestamp = CType(obj, Timestamp)

			If Me Is that Then Return True
			Return (timestamp_Renamed.Value.Equals(that.timestamp) AndAlso signerCertPath.Equals(that.signerCertPath))
		End Function

		''' <summary>
		''' Returns a string describing this timestamp.
		''' </summary>
		''' <returns> A string comprising the date and time of the timestamp and
		'''         its signer's certificate. </returns>
		Public Overrides Function ToString() As String
			Dim sb As New StringBuffer
			sb.append("(")
			sb.append("timestamp: " & timestamp_Renamed)
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
			Dim certs As IList(Of ? As java.security.cert.Certificate) = signerCertPath.certificates
			If certs.Count > 0 Then
				sb.append("TSA: " & certs(0))
			Else
				sb.append("TSA: <empty>")
			End If
			sb.append(")")
			Return sb.ToString()
		End Function

		' Explicitly reset hash code value to -1
		Private Sub readObject(ByVal ois As ObjectInputStream)
			ois.defaultReadObject()
			myhash = -1
			timestamp_Renamed = New DateTime?(timestamp_Renamed.Value.time)
		End Sub
	End Class

End Namespace