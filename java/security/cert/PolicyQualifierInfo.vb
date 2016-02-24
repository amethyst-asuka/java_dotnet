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

Namespace java.security.cert



	''' <summary>
	''' An immutable policy qualifier represented by the ASN.1 PolicyQualifierInfo
	''' structure.
	''' 
	''' <p>The ASN.1 definition is as follows:
	''' <pre>
	'''   PolicyQualifierInfo ::= SEQUENCE {
	'''        policyQualifierId       PolicyQualifierId,
	'''        qualifier               ANY DEFINED BY policyQualifierId }
	''' </pre>
	''' <p>
	''' A certificate policies extension, if present in an X.509 version 3
	''' certificate, contains a sequence of one or more policy information terms,
	''' each of which consists of an object identifier (OID) and optional
	''' qualifiers. In an end-entity certificate, these policy information terms
	''' indicate the policy under which the certificate has been issued and the
	''' purposes for which the certificate may be used. In a CA certificate, these
	''' policy information terms limit the set of policies for certification paths
	''' which include this certificate.
	''' <p>
	''' A {@code Set} of {@code PolicyQualifierInfo} objects are returned
	''' by the <seealso cref="PolicyNode#getPolicyQualifiers PolicyNode.getPolicyQualifiers"/>
	''' method. This allows applications with specific policy requirements to
	''' process and validate each policy qualifier. Applications that need to
	''' process policy qualifiers should explicitly set the
	''' {@code policyQualifiersRejected} flag to false (by calling the
	''' {@link PKIXParameters#setPolicyQualifiersRejected
	''' PKIXParameters.setPolicyQualifiersRejected} method) before validating
	''' a certification path.
	''' 
	''' <p>Note that the PKIX certification path validation algorithm specifies
	''' that any policy qualifier in a certificate policies extension that is
	''' marked critical must be processed and validated. Otherwise the
	''' certification path must be rejected. If the
	''' {@code policyQualifiersRejected} flag is set to false, it is up to
	''' the application to validate all policy qualifiers in this manner in order
	''' to be PKIX compliant.
	''' 
	''' <p><b>Concurrent Access</b>
	''' 
	''' <p>All {@code PolicyQualifierInfo} objects must be immutable and
	''' thread-safe. That is, multiple threads may concurrently invoke the
	''' methods defined in this class on a single {@code PolicyQualifierInfo}
	''' object (or more than one) with no ill effects. Requiring
	''' {@code PolicyQualifierInfo} objects to be immutable and thread-safe
	''' allows them to be passed around to various pieces of code without
	''' worrying about coordinating access.
	''' 
	''' @author      seth proctor
	''' @author      Sean Mullan
	''' @since       1.4
	''' </summary>
	Public Class PolicyQualifierInfo

		Private mEncoded As SByte()
		Private mId As String
		Private mData As SByte()
		Private pqiString As String

		''' <summary>
		''' Creates an instance of {@code PolicyQualifierInfo} from the
		''' encoded bytes. The encoded byte array is copied on construction.
		''' </summary>
		''' <param name="encoded"> a byte array containing the qualifier in DER encoding </param>
		''' <exception cref="IOException"> thrown if the byte array does not represent a
		''' valid and parsable policy qualifier </exception>
		Public Sub New(ByVal encoded As SByte())
			mEncoded = encoded.clone()

			Dim val As New sun.security.util.DerValue(mEncoded)
			If val.tag <> sun.security.util.DerValue.tag_Sequence Then Throw New java.io.IOException("Invalid encoding for PolicyQualifierInfo")

			mId = (val.data.derValue).oID.ToString()
			Dim tmp As SByte() = val.data.toByteArray()
			If tmp Is Nothing Then
				mData = Nothing
			Else
				mData = New SByte(tmp.Length - 1){}
				Array.Copy(tmp, 0, mData, 0, tmp.Length)
			End If
		End Sub

		''' <summary>
		''' Returns the {@code policyQualifierId} field of this
		''' {@code PolicyQualifierInfo}. The {@code policyQualifierId}
		''' is an Object Identifier (OID) represented by a set of nonnegative
		''' integers separated by periods.
		''' </summary>
		''' <returns> the OID (never {@code null}) </returns>
		Public Property policyQualifierId As String
			Get
				Return mId
			End Get
		End Property

		''' <summary>
		''' Returns the ASN.1 DER encoded form of this
		''' {@code PolicyQualifierInfo}.
		''' </summary>
		''' <returns> the ASN.1 DER encoded bytes (never {@code null}).
		''' Note that a copy is returned, so the data is cloned each time
		''' this method is called. </returns>
		Public Property encoded As SByte()
			Get
				Return mEncoded.clone()
			End Get
		End Property

		''' <summary>
		''' Returns the ASN.1 DER encoded form of the {@code qualifier}
		''' field of this {@code PolicyQualifierInfo}.
		''' </summary>
		''' <returns> the ASN.1 DER encoded bytes of the {@code qualifier}
		''' field. Note that a copy is returned, so the data is cloned each
		''' time this method is called. </returns>
		Public Property policyQualifier As SByte()
			Get
				Return (If(mData Is Nothing, Nothing, mData.clone()))
			End Get
		End Property

		''' <summary>
		''' Return a printable representation of this
		''' {@code PolicyQualifierInfo}.
		''' </summary>
		''' <returns> a {@code String} describing the contents of this
		'''         {@code PolicyQualifierInfo} </returns>
		Public Overrides Function ToString() As String
			If pqiString IsNot Nothing Then Return pqiString
			Dim enc As New sun.misc.HexDumpEncoder
			Dim sb As New StringBuffer
			sb.append("PolicyQualifierInfo: [" & vbLf)
			sb.append("  qualifierID: " & mId & vbLf)
			sb.append("  qualifier: " & (If(mData Is Nothing, "null", enc.encodeBuffer(mData))) & vbLf)
			sb.append("]")
			pqiString = sb.ToString()
			Return pqiString
		End Function
	End Class

End Namespace