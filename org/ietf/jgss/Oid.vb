Imports System

'
' * Copyright (c) 2000, 2011, Oracle and/or its affiliates. All rights reserved.
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

Namespace org.ietf.jgss


	''' <summary>
	''' This class represents Universal Object Identifiers (Oids) and their
	''' associated operations.<p>
	''' 
	''' Oids are hierarchically globally-interpretable identifiers used
	''' within the GSS-API framework to identify mechanisms and name formats.<p>
	''' 
	''' The structure and encoding of Oids is defined in ISOIEC-8824 and
	''' ISOIEC-8825.  For example the Oid representation of Kerberos V5
	''' mechanism is "1.2.840.113554.1.2.2"<p>
	''' 
	''' The GSSName name class contains public static Oid objects
	''' representing the standard name types defined in GSS-API.
	''' 
	''' @author Mayank Upadhyay
	''' @since 1.4
	''' </summary>
	Public Class Oid

		Private oid_Renamed As sun.security.util.ObjectIdentifier
		Private derEncoding As SByte()

		''' <summary>
		''' Constructs an Oid object from a string representation of its
		''' integer components.
		''' </summary>
		''' <param name="strOid"> the dot separated string representation of the oid.
		''' For instance, "1.2.840.113554.1.2.2". </param>
		''' <exception cref="GSSException"> may be thrown when the string is incorrectly
		'''     formatted </exception>
		Public Sub New(ByVal strOid As String)

			Try
				oid_Renamed = New sun.security.util.ObjectIdentifier(strOid)
				derEncoding = Nothing
			Catch e As Exception
				Throw New GSSException(GSSException.FAILURE, "Improperly formatted Object Identifier String - " & strOid)
			End Try
		End Sub

		''' <summary>
		''' Creates an Oid object from its ASN.1 DER encoding.  This refers to
		''' the full encoding including tag and length.  The structure and
		''' encoding of Oids is defined in ISOIEC-8824 and ISOIEC-8825.  This
		''' method is identical in functionality to its byte array counterpart.
		''' </summary>
		''' <param name="derOid"> stream containing the DER encoded oid </param>
		''' <exception cref="GSSException"> may be thrown when the DER encoding does not
		'''  follow the prescribed format. </exception>
		Public Sub New(ByVal derOid As java.io.InputStream)
			Try
				Dim derVal As New sun.security.util.DerValue(derOid)
				derEncoding = derVal.toByteArray()
				oid_Renamed = derVal.oID
			Catch e As java.io.IOException
				Throw New GSSException(GSSException.FAILURE, "Improperly formatted ASN.1 DER encoding for Oid")
			End Try
		End Sub


		''' <summary>
		''' Creates an Oid object from its ASN.1 DER encoding.  This refers to
		''' the full encoding including tag and length.  The structure and
		''' encoding of Oids is defined in ISOIEC-8824 and ISOIEC-8825.  This
		''' method is identical in functionality to its InputStream conterpart.
		''' </summary>
		''' <param name="data"> byte array containing the DER encoded oid </param>
		''' <exception cref="GSSException"> may be thrown when the DER encoding does not
		'''     follow the prescribed format. </exception>
		Public Sub New(ByVal data As SByte ())
			Try
				Dim derVal As New sun.security.util.DerValue(data)
				derEncoding = derVal.toByteArray()
				oid_Renamed = derVal.oID
			Catch e As java.io.IOException
				Throw New GSSException(GSSException.FAILURE, "Improperly formatted ASN.1 DER encoding for Oid")
			End Try
		End Sub

		''' <summary>
		''' Only for calling by initializators used with declarations.
		''' </summary>
		''' <param name="strOid"> </param>
		Shared Function getInstance(ByVal strOid As String) As Oid
			Dim retVal As Oid = Nothing
			Try
				retVal = New Oid(strOid)
			Catch e As GSSException
				' squelch it!
			End Try
			Return retVal
		End Function

		''' <summary>
		''' Returns a string representation of the oid's integer components
		''' in dot separated notation.
		''' </summary>
		''' <returns> string representation in the following format: "1.2.3.4.5" </returns>
		Public Overrides Function ToString() As String
			Return oid_Renamed.ToString()
		End Function

		''' <summary>
		''' Tests if two Oid objects represent the same Object identifier
		''' value.
		''' </summary>
		''' <returns> <code>true</code> if the two Oid objects represent the same
		''' value, <code>false</code> otherwise. </returns>
		''' <param name="other"> the Oid object that has to be compared to this one </param>
		Public Overrides Function Equals(ByVal other As Object) As Boolean

			'check if both reference the same object
			If Me Is other Then Return (True)

			If TypeOf other Is Oid Then
				Return Me.oid_Renamed.Equals(CObj(CType(other, Oid).oid_Renamed))
			ElseIf TypeOf other Is sun.security.util.ObjectIdentifier Then
				Return Me.oid_Renamed.Equals(other)
			Else
				Return False
			End If
		End Function


		''' <summary>
		''' Returns the full ASN.1 DER encoding for this oid object, which
		''' includes the tag and length.
		''' </summary>
		''' <returns> byte array containing the DER encoding of this oid object. </returns>
		''' <exception cref="GSSException"> may be thrown when the oid can't be encoded </exception>
		Public Overridable Property dER As SByte()
			Get
    
				If derEncoding Is Nothing Then
					Dim dout As New sun.security.util.DerOutputStream
					Try
						dout.putOID(oid_Renamed)
					Catch e As java.io.IOException
						Throw New GSSException(GSSException.FAILURE, e.Message)
					End Try
					derEncoding = dout.toByteArray()
				End If
    
				Return derEncoding.clone()
			End Get
		End Property

		''' <summary>
		''' A utility method to test if this Oid value is contained within the
		''' supplied Oid array.
		''' </summary>
		''' <param name="oids"> the array of Oid's to search </param>
		''' <returns> true if the array contains this Oid value, false otherwise </returns>
		Public Overridable Function containedIn(ByVal oids As Oid()) As Boolean

			For i As Integer = 0 To oids.Length - 1
				If oids(i).Equals(Me) Then Return (True)
			Next i

			Return (False)
		End Function


		''' <summary>
		''' Returns a hashcode value for this Oid.
		''' </summary>
		''' <returns> a hashCode value </returns>
		Public Overrides Function GetHashCode() As Integer
			Return oid_Renamed.GetHashCode()
		End Function
	End Class

End Namespace