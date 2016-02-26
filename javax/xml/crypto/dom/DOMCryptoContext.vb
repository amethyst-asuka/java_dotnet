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
' * $Id: DOMCryptoContext.java,v 1.3 2005/05/09 18:33:26 mullan Exp $
' 
Namespace javax.xml.crypto.dom


	''' <summary>
	''' This class provides a DOM-specific implementation of the
	''' <seealso cref="XMLCryptoContext"/> interface. It also includes additional
	''' methods that are specific to a DOM-based implementation for registering
	''' and retrieving elements that contain attributes of type ID.
	''' 
	''' @author Sean Mullan
	''' @author JSR 105 Expert Group
	''' @since 1.6
	''' </summary>
	Public Class DOMCryptoContext
		Implements javax.xml.crypto.XMLCryptoContext

		Private nsMap As New Dictionary(Of String, String)
		Private idMap As New Dictionary(Of String, org.w3c.dom.Element)
		Private objMap As New Dictionary(Of Object, Object)
		Private baseURI As String
		Private ks As javax.xml.crypto.KeySelector
		Private dereferencer As javax.xml.crypto.URIDereferencer
		Private propMap As New Dictionary(Of String, Object)
		Private defaultPrefix As String

		''' <summary>
		''' Default constructor. (For invocation by subclass constructors).
		''' </summary>
		Protected Friend Sub New()
		End Sub

		''' <summary>
		''' This implementation uses an internal <seealso cref="HashMap"/> to get the prefix
		''' that the specified URI maps to. It returns the <code>defaultPrefix</code>
		''' if it maps to <code>null</code>.
		''' </summary>
		''' <exception cref="NullPointerException"> {@inheritDoc} </exception>
		Public Overridable Function getNamespacePrefix(ByVal namespaceURI As String, ByVal defaultPrefix As String) As String
			If namespaceURI Is Nothing Then Throw New NullPointerException("namespaceURI cannot be null")
			Dim prefix As String = nsMap(namespaceURI)
			Return (If(prefix IsNot Nothing, prefix, defaultPrefix))
		End Function

		''' <summary>
		''' This implementation uses an internal <seealso cref="HashMap"/> to map the URI
		''' to the specified prefix.
		''' </summary>
		''' <exception cref="NullPointerException"> {@inheritDoc} </exception>
		Public Overridable Function putNamespacePrefix(ByVal namespaceURI As String, ByVal prefix As String) As String
			If namespaceURI Is Nothing Then Throw New NullPointerException("namespaceURI is null")
				nsMap(namespaceURI) = prefix
				Return nsMap(namespaceURI)
		End Function

		Public Overridable Property defaultNamespacePrefix As String
			Get
				Return defaultPrefix
			End Get
			Set(ByVal defaultPrefix As String)
				Me.defaultPrefix = defaultPrefix
			End Set
		End Property


		Public Overridable Property baseURI As String
			Get
				Return baseURI
			End Get
			Set(ByVal baseURI As String)
				If baseURI IsNot Nothing Then java.net.URI.create(baseURI)
				Me.baseURI = baseURI
			End Set
		End Property


		Public Overridable Property uRIDereferencer As javax.xml.crypto.URIDereferencer
			Get
				Return dereferencer
			End Get
			Set(ByVal dereferencer As javax.xml.crypto.URIDereferencer)
				Me.dereferencer = dereferencer
			End Set
		End Property


		''' <summary>
		''' This implementation uses an internal <seealso cref="HashMap"/> to get the object
		''' that the specified name maps to.
		''' </summary>
		''' <exception cref="NullPointerException"> {@inheritDoc} </exception>
		Public Overridable Function getProperty(ByVal name As String) As Object
			If name Is Nothing Then Throw New NullPointerException("name is null")
			Return propMap(name)
		End Function

		''' <summary>
		''' This implementation uses an internal <seealso cref="HashMap"/> to map the name
		''' to the specified object.
		''' </summary>
		''' <exception cref="NullPointerException"> {@inheritDoc} </exception>
		Public Overridable Function setProperty(ByVal name As String, ByVal value As Object) As Object
			If name Is Nothing Then Throw New NullPointerException("name is null")
				propMap(name) = value
				Return propMap(name)
		End Function

		Public Overridable Property keySelector As javax.xml.crypto.KeySelector
			Get
				Return ks
			End Get
			Set(ByVal ks As javax.xml.crypto.KeySelector)
				Me.ks = ks
			End Set
		End Property


		''' <summary>
		''' Returns the <code>Element</code> with the specified ID attribute value.
		''' 
		''' <p>This implementation uses an internal <seealso cref="HashMap"/> to get the
		''' element that the specified attribute value maps to.
		''' </summary>
		''' <param name="idValue"> the value of the ID </param>
		''' <returns> the <code>Element</code> with the specified ID attribute value,
		'''    or <code>null</code> if none. </returns>
		''' <exception cref="NullPointerException"> if <code>idValue</code> is <code>null</code> </exception>
		''' <seealso cref= #setIdAttributeNS </seealso>
		Public Overridable Function getElementById(ByVal idValue As String) As org.w3c.dom.Element
			If idValue Is Nothing Then Throw New NullPointerException("idValue is null")
			Return idMap(idValue)
		End Function

		''' <summary>
		''' Registers the element's attribute specified by the namespace URI and
		''' local name to be of type ID. The attribute must have a non-empty value.
		''' 
		''' <p>This implementation uses an internal <seealso cref="HashMap"/> to map the
		''' attribute's value to the specified element.
		''' </summary>
		''' <param name="element"> the element </param>
		''' <param name="namespaceURI"> the namespace URI of the attribute (specify
		'''    <code>null</code> if not applicable) </param>
		''' <param name="localName"> the local name of the attribute </param>
		''' <exception cref="IllegalArgumentException"> if <code>localName</code> is not an
		'''    attribute of the specified element or it does not contain a specific
		'''    value </exception>
		''' <exception cref="NullPointerException"> if <code>element</code> or
		'''    <code>localName</code> is <code>null</code> </exception>
		''' <seealso cref= #getElementById </seealso>
		Public Overridable Sub setIdAttributeNS(ByVal element As org.w3c.dom.Element, ByVal namespaceURI As String, ByVal localName As String)
			If element Is Nothing Then Throw New NullPointerException("element is null")
			If localName Is Nothing Then Throw New NullPointerException("localName is null")
			Dim idValue As String = element.getAttributeNS(namespaceURI, localName)
			If idValue Is Nothing OrElse idValue.Length = 0 Then Throw New System.ArgumentException(localName & " is not an " & "attribute")
			idMap(idValue) = element
		End Sub

		''' <summary>
		''' Returns a read-only iterator over the set of Id/Element mappings of
		''' this <code>DOMCryptoContext</code>. Attempts to modify the set via the
		''' <seealso cref="Iterator#remove"/> method throw an
		''' <code>UnsupportedOperationException</code>. The mappings are returned
		''' in no particular order. Each element in the iteration is represented as a
		''' <seealso cref="java.util.Map.Entry"/>. If the <code>DOMCryptoContext</code> is
		''' modified while an iteration is in progress, the results of the
		''' iteration are undefined.
		''' </summary>
		''' <returns> a read-only iterator over the set of mappings </returns>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
		Public Overridable Function [iterator]() As IEnumerator
			Return java.util.Collections.unmodifiableMap(idMap).entrySet().GetEnumerator()
		End Function

		''' <summary>
		''' This implementation uses an internal <seealso cref="HashMap"/> to get the object
		''' that the specified key maps to.
		''' </summary>
		Public Overridable Function [get](ByVal key As Object) As Object
			Return objMap(key)
		End Function

		''' <summary>
		''' This implementation uses an internal <seealso cref="HashMap"/> to map the key
		''' to the specified object.
		''' </summary>
		''' <exception cref="IllegalArgumentException"> {@inheritDoc} </exception>
		Public Overridable Function put(ByVal key As Object, ByVal value As Object) As Object
				objMap(key) = value
				Return objMap(key)
		End Function
	End Class

End Namespace