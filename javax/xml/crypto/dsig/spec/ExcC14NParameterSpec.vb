Imports System.Collections
Imports System.Collections.Generic

'
' * Copyright (c) 2005, 2011, Oracle and/or its affiliates. All rights reserved.
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
'
' * $Id: ExcC14NParameterSpec.java,v 1.7 2005/05/13 18:45:42 mullan Exp $
' 
Namespace javax.xml.crypto.dsig.spec


	''' <summary>
	''' Parameters for the W3C Recommendation:
	''' <a href="http://www.w3.org/TR/xml-exc-c14n/">
	''' Exclusive XML Canonicalization (C14N) algorithm</a>. The
	''' parameters include an optional inclusive namespace prefix list. The XML
	''' Schema Definition of the Exclusive XML Canonicalization parameters is
	''' defined as:
	''' <pre><code>
	''' &lt;schema xmlns="http://www.w3.org/2001/XMLSchema"
	'''         xmlns:ec="http://www.w3.org/2001/10/xml-exc-c14n#"
	'''         targetNamespace="http://www.w3.org/2001/10/xml-exc-c14n#"
	'''         version="0.1" elementFormDefault="qualified"&gt;
	''' 
	''' &lt;element name="InclusiveNamespaces" type="ec:InclusiveNamespaces"/&gt;
	''' &lt;complexType name="InclusiveNamespaces"&gt;
	'''   &lt;attribute name="PrefixList" type="xsd:string"/&gt;
	''' &lt;/complexType&gt;
	''' &lt;/schema&gt;
	''' </code></pre>
	''' 
	''' @author Sean Mullan
	''' @author JSR 105 Expert Group
	''' @since 1.6 </summary>
	''' <seealso cref= CanonicalizationMethod </seealso>
	Public NotInheritable Class ExcC14NParameterSpec
		Implements C14NMethodParameterSpec

		Private preList As IList(Of String)

		''' <summary>
		''' Indicates the default namespace ("#default").
		''' </summary>
		Public Const [DEFAULT] As String = "#default"

		''' <summary>
		''' Creates a <code>ExcC14NParameterSpec</code> with an empty prefix
		''' list.
		''' </summary>
		Public Sub New()
			preList = java.util.Collections.emptyList()
		End Sub

		''' <summary>
		''' Creates a <code>ExcC14NParameterSpec</code> with the specified list
		''' of prefixes. The list is copied to protect against subsequent
		''' modification.
		''' </summary>
		''' <param name="prefixList"> the inclusive namespace prefix list. Each entry in
		'''    the list is a <code>String</code> that represents a namespace prefix. </param>
		''' <exception cref="NullPointerException"> if <code>prefixList</code> is
		'''    <code>null</code> </exception>
		''' <exception cref="ClassCastException"> if any of the entries in the list are not
		'''    of type <code>String</code> </exception>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
		Public Sub New(ByVal prefixList As IList)
			If prefixList Is Nothing Then Throw New NullPointerException("prefixList cannot be null")
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
			Dim copy As IList(Of ?) = New List(Of ?)(CType(prefixList, IList(Of ?)))
			Dim i As Integer = 0
			Dim size As Integer = copy.Count
			Do While i < size
				If Not(TypeOf copy(i) Is String) Then Throw New ClassCastException("not a String")
				i += 1
			Loop

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
			Dim temp As IList(Of String) = CType(copy, IList(Of String))

			preList = java.util.Collections.unmodifiableList(temp)
		End Sub

		''' <summary>
		''' Returns the inclusive namespace prefix list. Each entry in the list
		''' is a <code>String</code> that represents a namespace prefix.
		''' 
		''' <p>This implementation returns an {@link
		''' java.util.Collections#unmodifiableList unmodifiable list}.
		''' </summary>
		''' <returns> the inclusive namespace prefix list (may be empty but never
		'''    <code>null</code>) </returns>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
		Public Property prefixList As IList
			Get
				Return preList
			End Get
		End Property
	End Class

End Namespace