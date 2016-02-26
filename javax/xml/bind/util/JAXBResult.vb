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
	''' JAXP <seealso cref="javax.xml.transform.Result"/> implementation
	''' that unmarshals a JAXB object.
	''' 
	''' <p>
	''' This utility class is useful to combine JAXB with
	''' other Java/XML technologies.
	''' 
	''' <p>
	''' The following example shows how to use JAXB to unmarshal a document
	''' resulting from an XSLT transformation.
	''' 
	''' <blockquote>
	'''    <pre>
	'''       JAXBResult result = new JAXBResult(
	'''         JAXBContext.newInstance("org.acme.foo") );
	''' 
	'''       // set up XSLT transformation
	'''       TransformerFactory tf = TransformerFactory.newInstance();
	'''       Transformer t = tf.newTransformer(new StreamSource("test.xsl"));
	''' 
	'''       // run transformation
	'''       t.transform(new StreamSource("document.xml"),result);
	''' 
	'''       // obtain the unmarshalled content tree
	'''       Object o = result.getResult();
	'''    </pre>
	''' </blockquote>
	''' 
	''' <p>
	''' The fact that JAXBResult derives from SAXResult is an implementation
	''' detail. Thus in general applications are strongly discouraged from
	''' accessing methods defined on SAXResult.
	''' 
	''' <p>
	''' In particular it shall never attempt to call the setHandler,
	''' setLexicalHandler, and setSystemId methods.
	''' 
	''' @author
	'''      Kohsuke Kawaguchi (kohsuke.kawaguchi@sun.com)
	''' </summary>
	Public Class JAXBResult
		Inherits javax.xml.transform.sax.SAXResult

		''' <summary>
		''' Creates a new instance that uses the specified
		''' JAXBContext to unmarshal.
		''' </summary>
		''' <param name="context"> The JAXBContext that will be used to create the
		''' necessary Unmarshaller.  This parameter must not be null. </param>
		''' <exception cref="JAXBException"> if an error is encountered while creating the
		''' JAXBResult or if the context parameter is null. </exception>
		Public Sub New(ByVal context As javax.xml.bind.JAXBContext)
			Me.New(If(context Is Nothing, assertionFailed(), context.createUnmarshaller()))
		End Sub

		''' <summary>
		''' Creates a new instance that uses the specified
		''' Unmarshaller to unmarshal an object.
		''' 
		''' <p>
		''' This JAXBResult object will use the specified Unmarshaller
		''' instance. It is the caller's responsibility not to use the
		''' same Unmarshaller for other purposes while it is being
		''' used by this object.
		''' 
		''' <p>
		''' The primary purpose of this method is to allow the client
		''' to configure Unmarshaller. Unless you know what you are doing,
		''' it's easier and safer to pass a JAXBContext.
		''' </summary>
		''' <param name="_unmarshaller"> the unmarshaller.  This parameter must not be null. </param>
		''' <exception cref="JAXBException"> if an error is encountered while creating the
		''' JAXBResult or the Unmarshaller parameter is null. </exception>
		Public Sub New(ByVal _unmarshaller As javax.xml.bind.Unmarshaller)
			If _unmarshaller Is Nothing Then Throw New javax.xml.bind.JAXBException(Messages.format(Messages.RESULT_NULL_UNMARSHALLER))

			Me.unmarshallerHandler = _unmarshaller.unmarshallerHandler

			MyBase.handler = unmarshallerHandler
		End Sub

		''' <summary>
		''' Unmarshaller that will be used to unmarshal
		''' the input documents.
		''' </summary>
		Private ReadOnly unmarshallerHandler As javax.xml.bind.UnmarshallerHandler

		''' <summary>
		''' Gets the unmarshalled object created by the transformation.
		''' 
		''' @return
		'''      Always return a non-null object.
		''' </summary>
		''' <exception cref="IllegalStateException">
		'''  if this method is called before an object is unmarshalled.
		''' </exception>
		''' <exception cref="JAXBException">
		'''      if there is any unmarshalling error.
		'''      Note that the implementation is allowed to throw SAXException
		'''      during the parsing when it finds an error. </exception>
		Public Overridable Property result As Object
			Get
				Return unmarshallerHandler.result
			End Get
		End Property

		''' <summary>
		''' Hook to throw exception from the middle of a contructor chained call
		''' to this
		''' </summary>
		Private Shared Function assertionFailed() As javax.xml.bind.Unmarshaller
			Throw New javax.xml.bind.JAXBException(Messages.format(Messages.RESULT_NULL_CONTEXT))
		End Function
	End Class

End Namespace