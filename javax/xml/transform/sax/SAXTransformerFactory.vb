Imports javax.xml.transform

'
' * Copyright (c) 2000, 2005, Oracle and/or its affiliates. All rights reserved.
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

Namespace javax.xml.transform.sax


	''' <summary>
	''' This class extends TransformerFactory to provide SAX-specific
	''' factory methods.  It provides two types of ContentHandlers,
	''' one for creating Transformers, the other for creating Templates
	''' objects.
	''' 
	''' <p>If an application wants to set the ErrorHandler or EntityResolver
	''' for an XMLReader used during a transformation, it should use a URIResolver
	''' to return the SAXSource which provides (with getXMLReader) a reference to
	''' the XMLReader.</p>
	''' </summary>
	Public MustInherit Class SAXTransformerFactory
		Inherits TransformerFactory

		''' <summary>
		''' If <seealso cref="javax.xml.transform.TransformerFactory#getFeature"/>
		''' returns true when passed this value as an argument,
		''' the TransformerFactory returned from
		''' <seealso cref="javax.xml.transform.TransformerFactory#newInstance"/> may
		''' be safely cast to a SAXTransformerFactory.
		''' </summary>
		Public Const FEATURE As String = "http://javax.xml.transform.sax.SAXTransformerFactory/feature"

		''' <summary>
		''' If <seealso cref="javax.xml.transform.TransformerFactory#getFeature"/>
		''' returns true when passed this value as an argument,
		''' the <seealso cref="#newXMLFilter(Source src)"/>
		''' and <seealso cref="#newXMLFilter(Templates templates)"/> methods are supported.
		''' </summary>
		Public Const FEATURE_XMLFILTER As String = "http://javax.xml.transform.sax.SAXTransformerFactory/feature/xmlfilter"

		''' <summary>
		''' The default constructor is protected on purpose.
		''' </summary>
		Protected Friend Sub New()
		End Sub

		''' <summary>
		''' Get a TransformerHandler object that can process SAX
		''' ContentHandler events into a Result, based on the transformation
		''' instructions specified by the argument.
		''' </summary>
		''' <param name="src"> The Source of the transformation instructions.
		''' </param>
		''' <returns> TransformerHandler ready to transform SAX events.
		''' </returns>
		''' <exception cref="TransformerConfigurationException"> If for some reason the
		''' TransformerHandler can not be created. </exception>
		Public MustOverride Function newTransformerHandler(ByVal src As Source) As TransformerHandler

		''' <summary>
		''' Get a TransformerHandler object that can process SAX
		''' ContentHandler events into a Result, based on the Templates argument.
		''' </summary>
		''' <param name="templates"> The compiled transformation instructions.
		''' </param>
		''' <returns> TransformerHandler ready to transform SAX events.
		''' </returns>
		''' <exception cref="TransformerConfigurationException"> If for some reason the
		''' TransformerHandler can not be created. </exception>
		Public MustOverride Function newTransformerHandler(ByVal templates As Templates) As TransformerHandler

		''' <summary>
		''' Get a TransformerHandler object that can process SAX
		''' ContentHandler events into a Result. The transformation
		''' is defined as an identity (or copy) transformation, for example
		''' to copy a series of SAX parse events into a DOM tree.
		''' </summary>
		''' <returns> A non-null reference to a TransformerHandler, that may
		''' be used as a ContentHandler for SAX parse events.
		''' </returns>
		''' <exception cref="TransformerConfigurationException"> If for some reason the
		''' TransformerHandler cannot be created. </exception>
		Public MustOverride Function newTransformerHandler() As TransformerHandler

		''' <summary>
		''' Get a TemplatesHandler object that can process SAX
		''' ContentHandler events into a Templates object.
		''' </summary>
		''' <returns> A non-null reference to a TransformerHandler, that may
		''' be used as a ContentHandler for SAX parse events.
		''' </returns>
		''' <exception cref="TransformerConfigurationException"> If for some reason the
		''' TemplatesHandler cannot be created. </exception>
		Public MustOverride Function newTemplatesHandler() As TemplatesHandler

		''' <summary>
		''' Create an XMLFilter that uses the given Source as the
		''' transformation instructions.
		''' </summary>
		''' <param name="src"> The Source of the transformation instructions.
		''' </param>
		''' <returns> An XMLFilter object, or null if this feature is not supported.
		''' </returns>
		''' <exception cref="TransformerConfigurationException"> If for some reason the
		''' TemplatesHandler cannot be created. </exception>
		Public MustOverride Function newXMLFilter(ByVal src As Source) As org.xml.sax.XMLFilter

		''' <summary>
		''' Create an XMLFilter, based on the Templates argument..
		''' </summary>
		''' <param name="templates"> The compiled transformation instructions.
		''' </param>
		''' <returns> An XMLFilter object, or null if this feature is not supported.
		''' </returns>
		''' <exception cref="TransformerConfigurationException"> If for some reason the
		''' TemplatesHandler cannot be created. </exception>
		Public MustOverride Function newXMLFilter(ByVal templates As Templates) As org.xml.sax.XMLFilter
	End Class

End Namespace