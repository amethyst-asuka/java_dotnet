'
' * Copyright (c) 2003, 2013, Oracle and/or its affiliates. All rights reserved.
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

Namespace javax.xml.bind.util



	''' <summary>
	''' JAXP <seealso cref="javax.xml.transform.Source"/> implementation
	''' that marshals a JAXB-generated object.
	''' 
	''' <p>
	''' This utility class is useful to combine JAXB with
	''' other Java/XML technologies.
	''' 
	''' <p>
	''' The following example shows how to use JAXB to marshal a document
	''' for transformation by XSLT.
	''' 
	''' <blockquote>
	'''    <pre>
	'''       MyObject o = // get JAXB content tree
	''' 
	'''       // jaxbContext is a JAXBContext object from which 'o' is created.
	'''       JAXBSource source = new JAXBSource( jaxbContext, o );
	''' 
	'''       // set up XSLT transformation
	'''       TransformerFactory tf = TransformerFactory.newInstance();
	'''       Transformer t = tf.newTransformer(new StreamSource("test.xsl"));
	''' 
	'''       // run transformation
	'''       t.transform(source,new StreamResult(System.out));
	'''    </pre>
	''' </blockquote>
	''' 
	''' <p>
	''' The fact that JAXBSource derives from SAXSource is an implementation
	''' detail. Thus in general applications are strongly discouraged from
	''' accessing methods defined on SAXSource. In particular,
	''' the setXMLReader and setInputSource methods shall never be called.
	''' The XMLReader object obtained by the getXMLReader method shall
	''' be used only for parsing the InputSource object returned by
	''' the getInputSource method.
	''' 
	''' <p>
	''' Similarly the InputSource object obtained by the getInputSource
	''' method shall be used only for being parsed by the XMLReader object
	''' returned by the getXMLReader.
	''' 
	''' @author
	'''      Kohsuke Kawaguchi (kohsuke.kawaguchi@sun.com)
	''' </summary>
	Public Class JAXBSource
		Inherits javax.xml.transform.sax.SAXSource

		''' <summary>
		''' Creates a new <seealso cref="javax.xml.transform.Source"/> for the given content object.
		''' </summary>
		''' <param name="context">
		'''      JAXBContext that was used to create
		'''      <code>contentObject</code>. This context is used
		'''      to create a new instance of marshaller and must not be null. </param>
		''' <param name="contentObject">
		'''      An instance of a JAXB-generated class, which will be
		'''      used as a <seealso cref="javax.xml.transform.Source"/> (by marshalling it into XML).  It must
		'''      not be null. </param>
		''' <exception cref="JAXBException"> if an error is encountered while creating the
		''' JAXBSource or if either of the parameters are null. </exception>
		Public Sub New(ByVal context As javax.xml.bind.JAXBContext, ByVal contentObject As Object)

			Me.New(If(context Is Nothing, assertionFailed(Messages.format(Messages.SOURCE_NULL_CONTEXT)), context.createMarshaller()),If(contentObject Is Nothing, assertionFailed(Messages.format(Messages.SOURCE_NULL_CONTENT)), contentObject))
		End Sub

		''' <summary>
		''' Creates a new <seealso cref="javax.xml.transform.Source"/> for the given content object.
		''' </summary>
		''' <param name="marshaller">
		'''      A marshaller instance that will be used to marshal
		'''      <code>contentObject</code> into XML. This must be
		'''      created from a JAXBContext that was used to build
		'''      <code>contentObject</code> and must not be null. </param>
		''' <param name="contentObject">
		'''      An instance of a JAXB-generated class, which will be
		'''      used as a <seealso cref="javax.xml.transform.Source"/> (by marshalling it into XML).  It must
		'''      not be null. </param>
		''' <exception cref="JAXBException"> if an error is encountered while creating the
		''' JAXBSource or if either of the parameters are null. </exception>
		Public Sub New(ByVal marshaller As javax.xml.bind.Marshaller, ByVal contentObject As Object)

			If marshaller Is Nothing Then Throw New javax.xml.bind.JAXBException(Messages.format(Messages.SOURCE_NULL_MARSHALLER))

			If contentObject Is Nothing Then Throw New javax.xml.bind.JAXBException(Messages.format(Messages.SOURCE_NULL_CONTENT))

			Me.marshaller = marshaller
			Me.contentObject = contentObject

			MyBase.xMLReader = pseudoParser
			' pass a dummy InputSource. We don't care
			MyBase.inputSource = New org.xml.sax.InputSource
		End Sub

		Private ReadOnly marshaller As javax.xml.bind.Marshaller
		Private ReadOnly contentObject As Object

		' this object will pretend as an XMLReader.
		' no matter what parameter is specified to the parse method,
		' it just parse the contentObject.
'JAVA TO VB CONVERTER TODO TASK: Anonymous inner classes are not converted to VB if the base type is not defined in the code being converted:
'		private final org.xml.sax.XMLReader pseudoParser = New org.xml.sax.XMLReader()
	'	{
	'		public boolean getFeature(String name) throws SAXNotRecognizedException
	'		{
	'			if(name.equals("http://xml.org/sax/features/namespaces"))
	'				Return True;
	'			if(name.equals("http://xml.org/sax/features/namespace-prefixes"))
	'				Return False;
	'			throw New SAXNotRecognizedException(name);
	'		}
	'
	'		public void setFeature(String name, boolean value) throws SAXNotRecognizedException
	'		{
	'			if(name.equals("http://xml.org/sax/features/namespaces") && value)
	'				Return;
	'			if(name.equals("http://xml.org/sax/features/namespace-prefixes") && !value)
	'				Return;
	'			throw New SAXNotRecognizedException(name);
	'		}
	'
	'		public Object getProperty(String name) throws SAXNotRecognizedException
	'		{
	'			if("http://xml.org/sax/properties/lexical-handler".equals(name))
	'			{
	'				Return lexicalHandler;
	'			}
	'			throw New SAXNotRecognizedException(name);
	'		}
	'
	'		public void setProperty(String name, Object value) throws SAXNotRecognizedException
	'		{
	'			if("http://xml.org/sax/properties/lexical-handler".equals(name))
	'			{
	'				Me.lexicalHandler = (LexicalHandler)value;
	'				Return;
	'			}
	'			throw New SAXNotRecognizedException(name);
	'		}
	'
	'		private LexicalHandler lexicalHandler;
	'
	'		' we will store this value but never use it by ourselves.
	'		private EntityResolver entityResolver;
	'		public void setEntityResolver(EntityResolver resolver)
	'		{
	'			Me.entityResolver = resolver;
	'		}
	'		public EntityResolver getEntityResolver()
	'		{
	'			Return entityResolver;
	'		}
	'
	'		private DTDHandler dtdHandler;
	'		public void setDTDHandler(DTDHandler handler)
	'		{
	'			Me.dtdHandler = handler;
	'		}
	'		public DTDHandler getDTDHandler()
	'		{
	'			Return dtdHandler;
	'		}
	'
	'		' SAX allows ContentHandler to be changed during the parsing,
	'		' but JAXB doesn't. So this repeater will sit between those
	'		' two components.
	'		private XMLFilter repeater = New XMLFilterImpl();
	'
	'		public void setContentHandler(ContentHandler handler)
	'		{
	'			repeater.setContentHandler(handler);
	'		}
	'		public ContentHandler getContentHandler()
	'		{
	'			Return repeater.getContentHandler();
	'		}
	'
	'		private ErrorHandler errorHandler;
	'		public void setErrorHandler(ErrorHandler handler)
	'		{
	'			Me.errorHandler = handler;
	'		}
	'		public ErrorHandler getErrorHandler()
	'		{
	'			Return errorHandler;
	'		}
	'
	'		public void parse(InputSource input) throws SAXException
	'		{
	'			parse();
	'		}
	'
	'		public void parse(String systemId) throws SAXException
	'		{
	'			parse();
	'		}
	'
	'		public void parse() throws SAXException
	'		{
	'			' parses a content object by using the given marshaller
	'			' SAX events will be sent to the repeater, and the repeater
	'			' will further forward it to an appropriate component.
	'			try
	'			{
	'				marshaller.marshal(contentObject, (XMLFilterImpl)repeater);
	'			}
	'			catch(JAXBException e)
	'			{
	'				' wrap it to a SAXException
	'				SAXParseException se = New SAXParseException(e.getMessage(), Nothing, Nothing, -1, -1, e);
	'
	'				' if the consumer sets an error handler, it is our responsibility
	'				' to notify it.
	'				if(errorHandler!=Nothing)
	'					errorHandler.fatalError(se);
	'
	'				' this is a fatal error. Even if the error handler
	'				' returns, we will abort anyway.
	'				throw se;
	'			}
	'		}
	'	};

		''' <summary>
		''' Hook to throw exception from the middle of a contructor chained call
		''' to this
		''' </summary>
		Private Shared Function assertionFailed(ByVal message As String) As javax.xml.bind.Marshaller

			Throw New javax.xml.bind.JAXBException(message)
		End Function
	End Class

End Namespace