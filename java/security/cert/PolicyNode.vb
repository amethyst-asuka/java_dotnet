Imports System.Collections.Generic

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
	''' An immutable valid policy tree node as defined by the PKIX certification
	''' path validation algorithm.
	''' 
	''' <p>One of the outputs of the PKIX certification path validation
	''' algorithm is a valid policy tree, which includes the policies that
	''' were determined to be valid, how this determination was reached,
	''' and any policy qualifiers encountered. This tree is of depth
	''' <i>n</i>, where <i>n</i> is the length of the certification
	''' path that has been validated.
	''' 
	''' <p>Most applications will not need to examine the valid policy tree.
	''' They can achieve their policy processing goals by setting the
	''' policy-related parameters in {@code PKIXParameters}. However,
	''' the valid policy tree is available for more sophisticated applications,
	''' especially those that process policy qualifiers.
	''' 
	''' <p>{@link PKIXCertPathValidatorResult#getPolicyTree()
	''' PKIXCertPathValidatorResult.getPolicyTree} returns the root node of the
	''' valid policy tree. The tree can be traversed using the
	''' <seealso cref="#getChildren getChildren"/> and <seealso cref="#getParent getParent"/> methods.
	''' Data about a particular node can be retrieved using other methods of
	''' {@code PolicyNode}.
	''' 
	''' <p><b>Concurrent Access</b>
	''' <p>All {@code PolicyNode} objects must be immutable and
	''' thread-safe. Multiple threads may concurrently invoke the methods defined
	''' in this class on a single {@code PolicyNode} object (or more than one)
	''' with no ill effects. This stipulation applies to all public fields and
	''' methods of this class and any added or overridden by subclasses.
	''' 
	''' @since       1.4
	''' @author      Sean Mullan
	''' </summary>
	Public Interface PolicyNode

		''' <summary>
		''' Returns the parent of this node, or {@code null} if this is the
		''' root node.
		''' </summary>
		''' <returns> the parent of this node, or {@code null} if this is the
		''' root node </returns>
		ReadOnly Property parent As PolicyNode

		''' <summary>
		''' Returns an iterator over the children of this node. Any attempts to
		''' modify the children of this node through the
		''' {@code Iterator}'s remove method must throw an
		''' {@code UnsupportedOperationException}.
		''' </summary>
		''' <returns> an iterator over the children of this node </returns>
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
		ReadOnly Property children As IEnumerator(Of ? As PolicyNode)

		''' <summary>
		''' Returns the depth of this node in the valid policy tree.
		''' </summary>
		''' <returns> the depth of this node (0 for the root node, 1 for its
		''' children, and so on) </returns>
		ReadOnly Property depth As Integer

		''' <summary>
		''' Returns the valid policy represented by this node.
		''' </summary>
		''' <returns> the {@code String} OID of the valid policy
		''' represented by this node. For the root node, this method always returns
		''' the special anyPolicy OID: "2.5.29.32.0". </returns>
		ReadOnly Property validPolicy As String

		''' <summary>
		''' Returns the set of policy qualifiers associated with the
		''' valid policy represented by this node.
		''' </summary>
		''' <returns> an immutable {@code Set} of
		''' {@code PolicyQualifierInfo}s. For the root node, this
		''' is always an empty {@code Set}. </returns>
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
		ReadOnly Property policyQualifiers As java.util.Set(Of ? As PolicyQualifierInfo)

		''' <summary>
		''' Returns the set of expected policies that would satisfy this
		''' node's valid policy in the next certificate to be processed.
		''' </summary>
		''' <returns> an immutable {@code Set} of expected policy
		''' {@code String} OIDs. For the root node, this method
		''' always returns a {@code Set} with one element, the
		''' special anyPolicy OID: "2.5.29.32.0". </returns>
		ReadOnly Property expectedPolicies As java.util.Set(Of String)

		''' <summary>
		''' Returns the criticality indicator of the certificate policy extension
		''' in the most recently processed certificate.
		''' </summary>
		''' <returns> {@code true} if extension marked critical,
		''' {@code false} otherwise. For the root node, {@code false}
		''' is always returned. </returns>
		ReadOnly Property critical As Boolean
	End Interface

End Namespace