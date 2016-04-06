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
	''' path validation algorithm.
	''' 
	''' <p>Instances of {@code PKIXCertPathValidatorResult} are returned by the
	''' <seealso cref="CertPathValidator#validate validate"/> method of
	''' {@code CertPathValidator} objects implementing the PKIX algorithm.
	''' 
	''' <p> All {@code PKIXCertPathValidatorResult} objects contain the
	''' valid policy tree and subject public key resulting from the
	''' validation algorithm, as well as a {@code TrustAnchor} describing
	''' the certification authority (CA) that served as a trust anchor for the
	''' certification path.
	''' <p>
	''' <b>Concurrent Access</b>
	''' <p>
	''' Unless otherwise specified, the methods defined in this class are not
	''' thread-safe. Multiple threads that need to access a single
	''' object concurrently should synchronize amongst themselves and
	''' provide the necessary locking. Multiple threads each manipulating
	''' separate objects need not synchronize.
	''' </summary>
	''' <seealso cref= CertPathValidatorResult
	''' 
	''' @since       1.4
	''' @author      Yassir Elley
	''' @author      Sean Mullan </seealso>
	Public Class PKIXCertPathValidatorResult
		Implements CertPathValidatorResult

		Private trustAnchor As TrustAnchor
		Private policyTree As PolicyNode
		Private subjectPublicKey As java.security.PublicKey

		''' <summary>
		''' Creates an instance of {@code PKIXCertPathValidatorResult}
		''' containing the specified parameters.
		''' </summary>
		''' <param name="trustAnchor"> a {@code TrustAnchor} describing the CA that
		''' served as a trust anchor for the certification path </param>
		''' <param name="policyTree"> the immutable valid policy tree, or {@code null}
		''' if there are no valid policies </param>
		''' <param name="subjectPublicKey"> the public key of the subject </param>
		''' <exception cref="NullPointerException"> if the {@code subjectPublicKey} or
		''' {@code trustAnchor} parameters are {@code null} </exception>
		Public Sub New(  trustAnchor As TrustAnchor,   policyTree As PolicyNode,   subjectPublicKey As java.security.PublicKey)
			If subjectPublicKey Is Nothing Then Throw New NullPointerException("subjectPublicKey must be non-null")
			If trustAnchor Is Nothing Then Throw New NullPointerException("trustAnchor must be non-null")
			Me.trustAnchor = trustAnchor
			Me.policyTree = policyTree
			Me.subjectPublicKey = subjectPublicKey
		End Sub

		''' <summary>
		''' Returns the {@code TrustAnchor} describing the CA that served
		''' as a trust anchor for the certification path.
		''' </summary>
		''' <returns> the {@code TrustAnchor} (never {@code null}) </returns>
		Public Overridable Property trustAnchor As TrustAnchor
			Get
				Return trustAnchor
			End Get
		End Property

		''' <summary>
		''' Returns the root node of the valid policy tree resulting from the
		''' PKIX certification path validation algorithm. The
		''' {@code PolicyNode} object that is returned and any objects that
		''' it returns through public methods are immutable.
		''' 
		''' <p>Most applications will not need to examine the valid policy tree.
		''' They can achieve their policy processing goals by setting the
		''' policy-related parameters in {@code PKIXParameters}. However, more
		''' sophisticated applications, especially those that process policy
		''' qualifiers, may need to traverse the valid policy tree using the
		''' <seealso cref="PolicyNode#getParent PolicyNode.getParent"/> and
		''' <seealso cref="PolicyNode#getChildren PolicyNode.getChildren"/> methods.
		''' </summary>
		''' <returns> the root node of the valid policy tree, or {@code null}
		''' if there are no valid policies </returns>
		Public Overridable Property policyTree As PolicyNode
			Get
				Return policyTree
			End Get
		End Property

		''' <summary>
		''' Returns the public key of the subject (target) of the certification
		''' path, including any inherited public key parameters if applicable.
		''' </summary>
		''' <returns> the public key of the subject (never {@code null}) </returns>
		Public Overridable Property publicKey As java.security.PublicKey
			Get
				Return subjectPublicKey
			End Get
		End Property

		''' <summary>
		''' Returns a copy of this object.
		''' </summary>
		''' <returns> the copy </returns>
		Public Overridable Function clone() As Object Implements CertPathValidatorResult.clone
			Try
				Return MyBase.clone()
			Catch e As CloneNotSupportedException
				' Cannot happen 
				Throw New InternalError(e.ToString(), e)
			End Try
		End Function

		''' <summary>
		''' Return a printable representation of this
		''' {@code PKIXCertPathValidatorResult}.
		''' </summary>
		''' <returns> a {@code String} describing the contents of this
		'''         {@code PKIXCertPathValidatorResult} </returns>
		Public Overrides Function ToString() As String
			Dim sb As New StringBuffer
			sb.append("PKIXCertPathValidatorResult: [" & vbLf)
			sb.append("  Trust Anchor: " & trustAnchor.ToString() & vbLf)
			sb.append("  Policy Tree: " & Convert.ToString(policyTree) & vbLf)
			sb.append("  Subject Public Key: " & subjectPublicKey & vbLf)
			sb.append("]")
			Return sb.ToString()
		End Function
	End Class

End Namespace