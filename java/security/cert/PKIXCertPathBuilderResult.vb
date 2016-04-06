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
	''' This class represents the successful result of the PKIX certification
	''' path builder algorithm. All certification paths that are built and
	''' returned using this algorithm are also validated according to the PKIX
	''' certification path validation algorithm.
	''' 
	''' <p>Instances of {@code PKIXCertPathBuilderResult} are returned by
	''' the {@code build} method of {@code CertPathBuilder}
	''' objects implementing the PKIX algorithm.
	''' 
	''' <p>All {@code PKIXCertPathBuilderResult} objects contain the
	''' certification path constructed by the build algorithm, the
	''' valid policy tree and subject public key resulting from the build
	''' algorithm, and a {@code TrustAnchor} describing the certification
	''' authority (CA) that served as a trust anchor for the certification path.
	''' <p>
	''' <b>Concurrent Access</b>
	''' <p>
	''' Unless otherwise specified, the methods defined in this class are not
	''' thread-safe. Multiple threads that need to access a single
	''' object concurrently should synchronize amongst themselves and
	''' provide the necessary locking. Multiple threads each manipulating
	''' separate objects need not synchronize.
	''' </summary>
	''' <seealso cref= CertPathBuilderResult
	''' 
	''' @since       1.4
	''' @author      Anne Anderson </seealso>
	Public Class PKIXCertPathBuilderResult
		Inherits PKIXCertPathValidatorResult
		Implements CertPathBuilderResult

		Private certPath As CertPath

		''' <summary>
		''' Creates an instance of {@code PKIXCertPathBuilderResult}
		''' containing the specified parameters.
		''' </summary>
		''' <param name="certPath"> the validated {@code CertPath} </param>
		''' <param name="trustAnchor"> a {@code TrustAnchor} describing the CA that
		''' served as a trust anchor for the certification path </param>
		''' <param name="policyTree"> the immutable valid policy tree, or {@code null}
		''' if there are no valid policies </param>
		''' <param name="subjectPublicKey"> the public key of the subject </param>
		''' <exception cref="NullPointerException"> if the {@code certPath},
		''' {@code trustAnchor} or {@code subjectPublicKey} parameters
		''' are {@code null} </exception>
		Public Sub New(  certPath As CertPath,   trustAnchor As TrustAnchor,   policyTree As PolicyNode,   subjectPublicKey As java.security.PublicKey)
			MyBase.New(trustAnchor, policyTree, subjectPublicKey)
			If certPath Is Nothing Then Throw New NullPointerException("certPath must be non-null")
			Me.certPath = certPath
		End Sub

		''' <summary>
		''' Returns the built and validated certification path. The
		''' {@code CertPath} object does not include the trust anchor.
		''' Instead, use the <seealso cref="#getTrustAnchor() getTrustAnchor()"/> method to
		''' obtain the {@code TrustAnchor} that served as the trust anchor
		''' for the certification path.
		''' </summary>
		''' <returns> the built and validated {@code CertPath} (never
		''' {@code null}) </returns>
		Public Overridable Property certPath As CertPath Implements CertPathBuilderResult.getCertPath
			Get
				Return certPath
			End Get
		End Property

		''' <summary>
		''' Return a printable representation of this
		''' {@code PKIXCertPathBuilderResult}.
		''' </summary>
		''' <returns> a {@code String} describing the contents of this
		'''         {@code PKIXCertPathBuilderResult} </returns>
		Public Overrides Function ToString() As String
			Dim sb As New StringBuffer
			sb.append("PKIXCertPathBuilderResult: [" & vbLf)
			sb.append("  Certification Path: " & certPath & vbLf)
			sb.append("  Trust Anchor: " & trustAnchor.ToString() & vbLf)
			sb.append("  Policy Tree: " & Convert.ToString(policyTree) & vbLf)
			sb.append("  Subject Public Key: " & publicKey & vbLf)
			sb.append("]")
			Return sb.ToString()
		End Function
	End Class

End Namespace