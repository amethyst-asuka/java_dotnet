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
' * $Id: XPathFilterParameterSpec.java,v 1.4 2005/05/10 16:40:17 mullan Exp $
' 
Namespace javax.xml.crypto.dsig.spec


	''' <summary>
	''' Parameters for the <a href="http://www.w3.org/TR/xmldsig-core/#sec-XPath">
	''' XPath Filtering Transform Algorithm</a>.
	''' The parameters include the XPath expression and an optional <code>Map</code>
	''' of additional namespace prefix mappings. The XML Schema Definition of
	''' the XPath Filtering transform parameters is defined as:
	''' <pre><code>
	''' &lt;element name="XPath" type="string"/&gt;
	''' </code></pre>
	''' 
	''' @author Sean Mullan
	''' @author JSR 105 Expert Group
	''' @since 1.6 </summary>
	''' <seealso cref= Transform </seealso>
	Public NotInheritable Class XPathFilterParameterSpec
		Implements TransformParameterSpec

		Private xPath As String
		Private nsMap As IDictionary(Of String, String)

		''' <summary>
		''' Creates an <code>XPathFilterParameterSpec</code> with the specified
		''' XPath expression.
		''' </summary>
		''' <param name="xPath"> the XPath expression to be evaluated </param>
		''' <exception cref="NullPointerException"> if <code>xPath</code> is <code>null</code> </exception>
		Public Sub New(ByVal xPath As String)
			If xPath Is Nothing Then Throw New NullPointerException
			Me.xPath = xPath
			Me.nsMap = java.util.Collections.emptyMap()
		End Sub

		''' <summary>
		''' Creates an <code>XPathFilterParameterSpec</code> with the specified
		''' XPath expression and namespace map. The map is copied to protect against
		''' subsequent modification.
		''' </summary>
		''' <param name="xPath"> the XPath expression to be evaluated </param>
		''' <param name="namespaceMap"> the map of namespace prefixes. Each key is a
		'''    namespace prefix <code>String</code> that maps to a corresponding
		'''    namespace URI <code>String</code>. </param>
		''' <exception cref="NullPointerException"> if <code>xPath</code> or
		'''    <code>namespaceMap</code> are <code>null</code> </exception>
		''' <exception cref="ClassCastException"> if any of the map's keys or entries are not
		'''    of type <code>String</code> </exception>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
		Public Sub New(ByVal xPath As String, ByVal namespaceMap As IDictionary)
			If xPath Is Nothing OrElse namespaceMap Is Nothing Then Throw New NullPointerException
			Me.xPath = xPath
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
		Public Property xPath As String
			Get
				Return xPath
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
		''' <returns> a <code>Map</code> of namespace prefixes to namespace URIs (may
		'''    be empty, but never <code>null</code>) </returns>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
		Public Property namespaceMap As IDictionary
			Get
				Return nsMap
			End Get
		End Property
	End Class

End Namespace