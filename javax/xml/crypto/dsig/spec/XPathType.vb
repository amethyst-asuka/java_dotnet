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
' * $Id: XPathType.java,v 1.4 2005/05/10 16:40:17 mullan Exp $
' 
Namespace javax.xml.crypto.dsig.spec


	''' <summary>
	''' The XML Schema Definition of the <code>XPath</code> element as defined in the
	''' <a href="http://www.w3.org/TR/xmldsig-filter2">
	''' W3C Recommendation for XML-Signature XPath Filter 2.0</a>:
	''' <pre><code>
	''' &lt;schema xmlns="http://www.w3.org/2001/XMLSchema"
	'''         xmlns:xf="http://www.w3.org/2002/06/xmldsig-filter2"
	'''         targetNamespace="http://www.w3.org/2002/06/xmldsig-filter2"
	'''         version="0.1" elementFormDefault="qualified"&gt;
	''' 
	''' &lt;element name="XPath"
	'''          type="xf:XPathType"/&gt;
	''' 
	''' &lt;complexType name="XPathType"&gt;
	'''   &lt;simpleContent&gt;
	'''     &lt;extension base="string"&gt;
	'''       &lt;attribute name="Filter"&gt;
	'''         &lt;simpleType&gt;
	'''           &lt;restriction base="string"&gt;
	'''             &lt;enumeration value="intersect"/&gt;
	'''             &lt;enumeration value="subtract"/&gt;
	'''             &lt;enumeration value="union"/&gt;
	'''           &lt;/restriction&gt;
	'''         &lt;/simpleType&gt;
	'''       &lt;/attribute&gt;
	'''     &lt;/extension&gt;
	'''   &lt;/simpleContent&gt;
	''' &lt;/complexType&gt;
	''' </code></pre>
	''' 
	''' @author Sean Mullan
	''' @author JSR 105 Expert Group
	''' @since 1.6 </summary>
	''' <seealso cref= XPathFilter2ParameterSpec </seealso>
	Public Class XPathType

		''' <summary>
		''' Represents the filter set operation.
		''' </summary>
		Public Class Filter
			Private ReadOnly operation As String

			Private Sub New(ByVal operation As String)
				Me.operation = operation
			End Sub

			''' <summary>
			''' Returns the string form of the operation.
			''' </summary>
			''' <returns> the string form of the operation </returns>
			Public Overrides Function ToString() As String
				Return operation
			End Function

			''' <summary>
			''' The intersect filter operation.
			''' </summary>
			Public Shared ReadOnly INTERSECT As New Filter("intersect")

			''' <summary>
			''' The subtract filter operation.
			''' </summary>
			Public Shared ReadOnly SUBTRACT As New Filter("subtract")

			''' <summary>
			''' The union filter operation.
			''' </summary>
			Public Shared ReadOnly UNION As New Filter("union")
		End Class

		Private ReadOnly expression As String
		Private ReadOnly ___filter As Filter
		Private nsMap As IDictionary(Of String, String)

		''' <summary>
		''' Creates an <code>XPathType</code> instance with the specified XPath
		''' expression and filter.
		''' </summary>
		''' <param name="expression"> the XPath expression to be evaluated </param>
		''' <param name="filter"> the filter operation (<seealso cref="Filter#INTERSECT"/>,
		'''    <seealso cref="Filter#SUBTRACT"/>, or <seealso cref="Filter#UNION"/>) </param>
		''' <exception cref="NullPointerException"> if <code>expression</code> or
		'''    <code>filter</code> is <code>null</code> </exception>
		Public Sub New(ByVal expression As String, ByVal ___filter As Filter)
			If expression Is Nothing Then Throw New NullPointerException("expression cannot be null")
			If ___filter Is Nothing Then Throw New NullPointerException("filter cannot be null")
			Me.expression = expression
			Me.___filter = ___filter
			Me.nsMap = java.util.Collections.emptyMap()
		End Sub

		''' <summary>
		''' Creates an <code>XPathType</code> instance with the specified XPath
		''' expression, filter, and namespace map. The map is copied to protect
		''' against subsequent modification.
		''' </summary>
		''' <param name="expression"> the XPath expression to be evaluated </param>
		''' <param name="filter"> the filter operation (<seealso cref="Filter#INTERSECT"/>,
		'''    <seealso cref="Filter#SUBTRACT"/>, or <seealso cref="Filter#UNION"/>) </param>
		''' <param name="namespaceMap"> the map of namespace prefixes. Each key is a
		'''    namespace prefix <code>String</code> that maps to a corresponding
		'''    namespace URI <code>String</code>. </param>
		''' <exception cref="NullPointerException"> if <code>expression</code>,
		'''    <code>filter</code> or <code>namespaceMap</code> are
		'''    <code>null</code> </exception>
		''' <exception cref="ClassCastException"> if any of the map's keys or entries are
		'''    not of type <code>String</code> </exception>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
		Public Sub New(ByVal expression As String, ByVal ___filter As Filter, ByVal namespaceMap As IDictionary)
			Me.New(expression, ___filter)
			If namespaceMap Is Nothing Then Throw New NullPointerException("namespaceMap cannot be null")
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
			Dim copy As IDictionary(Of ?, ?) = New Dictionary(Of ?, ?)(CType(namespaceMap, IDictionary(Of ?, ?)))
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
			Dim entries As IEnumerator(Of ? As KeyValuePair(Of ?, ?)) = copy.GetEnumerator()
			Do While entries.MoveNext()
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
				Dim [me] As KeyValuePair(Of ?, ?) = entries.Current
				If Not(TypeOf [me].Key Is String) OrElse Not(TypeOf [me].Value Is String) Then Throw New ClassCastException("not a String")
			Loop

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
			Dim temp As IDictionary(Of String, String) = CType(copy, IDictionary(Of String, String))

			nsMap = java.util.Collections.unmodifiableMap(temp)
		End Sub

		''' <summary>
		''' Returns the XPath expression to be evaluated.
		''' </summary>
		''' <returns> the XPath expression to be evaluated </returns>
		Public Overridable Property expression As String
			Get
				Return expression
			End Get
		End Property

		''' <summary>
		''' Returns the filter operation.
		''' </summary>
		''' <returns> the filter operation </returns>
		Public Overridable Property filter As Filter
			Get
				Return ___filter
			End Get
		End Property

		''' <summary>
		''' Returns a map of namespace prefixes. Each key is a namespace prefix
		''' <code>String</code> that maps to a corresponding namespace URI
		''' <code>String</code>.
		''' <p>
		''' This implementation returns an {@link Collections#unmodifiableMap
		''' unmodifiable map}.
		''' </summary>
		''' <returns> a <code>Map</code> of namespace prefixes to namespace URIs
		'''    (may be empty, but never <code>null</code>) </returns>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
		Public Overridable Property namespaceMap As IDictionary
			Get
				Return nsMap
			End Get
		End Property
	End Class

End Namespace