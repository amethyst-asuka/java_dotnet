'
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
' * Copyright (c) 2009 by Oracle Corporation. All Rights Reserved.
' 

Namespace javax.xml.stream.util


	''' <summary>
	''' This is the base class for deriving an XMLStreamReader filter
	''' 
	''' This class is designed to sit between an XMLStreamReader and an
	''' application's XMLStreamReader.   By default each method
	''' does nothing but call the corresponding method on the
	''' parent interface.
	''' 
	''' @version 1.0
	''' @author Copyright (c) 2009 by Oracle Corporation. All Rights Reserved. </summary>
	''' <seealso cref= javax.xml.stream.XMLStreamReader </seealso>
	''' <seealso cref= EventReaderDelegate
	''' @since 1.6 </seealso>

	Public Class StreamReaderDelegate
		Implements javax.xml.stream.XMLStreamReader

	  Private reader As javax.xml.stream.XMLStreamReader

	  ''' <summary>
	  ''' Construct an empty filter with no parent.
	  ''' </summary>
	  Public Sub New()
	  End Sub

	  ''' <summary>
	  ''' Construct an filter with the specified parent. </summary>
	  ''' <param name="reader"> the parent </param>
	  Public Sub New(ByVal reader As javax.xml.stream.XMLStreamReader)
		Me.reader = reader
	  End Sub

	  ''' <summary>
	  ''' Set the parent of this instance. </summary>
	  ''' <param name="reader"> the new parent </param>
	  Public Overridable Property parent As javax.xml.stream.XMLStreamReader
		  Set(ByVal reader As javax.xml.stream.XMLStreamReader)
			Me.reader = reader
		  End Set
		  Get
			Return reader
		  End Get
	  End Property


	  Public Overridable Function [next]() As Integer
		Return reader.next()
	  End Function

	  Public Overridable Function nextTag() As Integer
		Return reader.nextTag()
	  End Function

	  Public Overridable Property elementText As String
		  Get
			Return reader.elementText
		  End Get
	  End Property

	  Public Overridable Sub require(ByVal type As Integer, ByVal namespaceURI As String, ByVal localName As String)
		reader.require(type,namespaceURI,localName)
	  End Sub

	  Public Overridable Function hasNext() As Boolean
		Return reader.hasNext()
	  End Function

	  Public Overridable Sub close()
		reader.close()
	  End Sub

	  Public Overridable Function getNamespaceURI(ByVal prefix As String) As String
		Return reader.getNamespaceURI(prefix)
	  End Function

	  Public Overridable Property namespaceContext As javax.xml.namespace.NamespaceContext
		  Get
			Return reader.namespaceContext
		  End Get
	  End Property

	  Public Overridable Property startElement As Boolean
		  Get
			Return reader.startElement
		  End Get
	  End Property

	  Public Overridable Property endElement As Boolean
		  Get
			Return reader.endElement
		  End Get
	  End Property

	  Public Overridable Property characters As Boolean
		  Get
			Return reader.characters
		  End Get
	  End Property

	  Public Overridable Property whiteSpace As Boolean
		  Get
			Return reader.whiteSpace
		  End Get
	  End Property

	  Public Overridable Function getAttributeValue(ByVal namespaceUri As String, ByVal localName As String) As String
		Return reader.getAttributeValue(namespaceUri,localName)
	  End Function

	  Public Overridable Property attributeCount As Integer
		  Get
			Return reader.attributeCount
		  End Get
	  End Property

	  Public Overridable Function getAttributeName(ByVal index As Integer) As javax.xml.namespace.QName
		Return reader.getAttributeName(index)
	  End Function

	  Public Overridable Function getAttributePrefix(ByVal index As Integer) As String
		Return reader.getAttributePrefix(index)
	  End Function

	  Public Overridable Function getAttributeNamespace(ByVal index As Integer) As String
		Return reader.getAttributeNamespace(index)
	  End Function

	  Public Overridable Function getAttributeLocalName(ByVal index As Integer) As String
		Return reader.getAttributeLocalName(index)
	  End Function

	  Public Overridable Function getAttributeType(ByVal index As Integer) As String
		Return reader.getAttributeType(index)
	  End Function

	  Public Overridable Function getAttributeValue(ByVal index As Integer) As String
		Return reader.getAttributeValue(index)
	  End Function

	  Public Overridable Function isAttributeSpecified(ByVal index As Integer) As Boolean
		Return reader.isAttributeSpecified(index)
	  End Function

	  Public Overridable Property namespaceCount As Integer
		  Get
			Return reader.namespaceCount
		  End Get
	  End Property

	  Public Overridable Function getNamespacePrefix(ByVal index As Integer) As String
		Return reader.getNamespacePrefix(index)
	  End Function

	  Public Overridable Function getNamespaceURI(ByVal index As Integer) As String
		Return reader.getNamespaceURI(index)
	  End Function

	  Public Overridable Property eventType As Integer
		  Get
			Return reader.eventType
		  End Get
	  End Property

	  Public Overridable Property text As String
		  Get
			Return reader.text
		  End Get
	  End Property

	  Public Overridable Function getTextCharacters(ByVal sourceStart As Integer, ByVal target As Char(), ByVal targetStart As Integer, ByVal length As Integer) As Integer
		Return reader.getTextCharacters(sourceStart, target, targetStart, length)
	  End Function


	  Public Overridable Property textCharacters As Char()
		  Get
			Return reader.textCharacters
		  End Get
	  End Property

	  Public Overridable Property textStart As Integer
		  Get
			Return reader.textStart
		  End Get
	  End Property

	  Public Overridable Property textLength As Integer
		  Get
			Return reader.textLength
		  End Get
	  End Property

	  Public Overridable Property encoding As String
		  Get
			Return reader.encoding
		  End Get
	  End Property

	  Public Overridable Function hasText() As Boolean
		Return reader.hasText()
	  End Function

	  Public Overridable Property location As javax.xml.stream.Location
		  Get
			Return reader.location
		  End Get
	  End Property

	  Public Overridable Property name As javax.xml.namespace.QName
		  Get
			Return reader.name
		  End Get
	  End Property

	  Public Overridable Property localName As String
		  Get
			Return reader.localName
		  End Get
	  End Property

	  Public Overridable Function hasName() As Boolean
		Return reader.hasName()
	  End Function

	  Public Overridable Property namespaceURI As String
		  Get
			Return reader.namespaceURI
		  End Get
	  End Property

	  Public Overridable Property prefix As String
		  Get
			Return reader.prefix
		  End Get
	  End Property

	  Public Overridable Property version As String
		  Get
			Return reader.version
		  End Get
	  End Property

	  Public Overridable Property standalone As Boolean
		  Get
			Return reader.standalone
		  End Get
	  End Property

	  Public Overridable Function standaloneSet() As Boolean
		Return reader.standaloneSet()
	  End Function

	  Public Overridable Property characterEncodingScheme As String
		  Get
			Return reader.characterEncodingScheme
		  End Get
	  End Property

	  Public Overridable Property pITarget As String
		  Get
			Return reader.pITarget
		  End Get
	  End Property

	  Public Overridable Property pIData As String
		  Get
			Return reader.pIData
		  End Get
	  End Property

	  Public Overridable Function getProperty(ByVal name As String) As Object
		Return reader.getProperty(name)
	  End Function
	End Class

End Namespace