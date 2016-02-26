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

Namespace javax.xml.parsers


	''' <summary>
	''' Defines a factory API that enables applications to obtain a
	''' parser that produces DOM object trees from XML documents.
	''' 
	''' @author <a href="mailto:Jeff.Suttor@Sun.com">Jeff Suttor</a>
	''' @author <a href="mailto:Neeraj.Bajaj@sun.com">Neeraj Bajaj</a>
	''' 
	''' @version $Revision: 1.9 $, $Date: 2010/05/25 16:19:44 $
	''' 
	''' </summary>

	Public MustInherit Class DocumentBuilderFactory

		Private validating As Boolean = False
		Private namespaceAware As Boolean = False
		Private whitespace As Boolean = False
		Private expandEntityRef As Boolean = True
		Private ignoreComments As Boolean = False
		Private coalescing As Boolean = False

		''' <summary>
		''' <p>Protected constructor to prevent instantiation.
		''' Use <seealso cref="#newInstance()"/>.</p>
		''' </summary>
		Protected Friend Sub New()
		End Sub

		''' <summary>
		''' Obtain a new instance of a
		''' <code>DocumentBuilderFactory</code>. This static method creates
		''' a new factory instance.
		''' This method uses the following ordered lookup procedure to determine
		''' the <code>DocumentBuilderFactory</code> implementation class to
		''' load:
		''' <ul>
		''' <li>
		''' Use the <code>javax.xml.parsers.DocumentBuilderFactory</code> system
		''' property.
		''' </li>
		''' <li>
		''' Use the properties file "lib/jaxp.properties" in the JRE directory.
		''' This configuration file is in standard <code>java.util.Properties
		''' </code> format and contains the fully qualified name of the
		''' implementation class with the key being the system property defined
		''' above.
		''' 
		''' The jaxp.properties file is read only once by the JAXP implementation
		''' and it's values are then cached for future use.  If the file does not exist
		''' when the first attempt is made to read from it, no further attempts are
		''' made to check for its existence.  It is not possible to change the value
		''' of any property in jaxp.properties after it has been read for the first time.
		''' </li>
		''' <li>
		''' Uses the service-provider loading facilities, defined by the
		''' <seealso cref="java.util.ServiceLoader"/> class, to attempt to locate and load an
		''' implementation of the service using the {@linkplain
		''' java.util.ServiceLoader#load(java.lang.Class) default loading mechanism}:
		''' the service-provider loading facility will use the {@linkplain
		''' java.lang.Thread#getContextClassLoader() current thread's context class loader}
		''' to attempt to load the service. If the context class
		''' loader is null, the {@linkplain
		''' ClassLoader#getSystemClassLoader() system class loader} will be used.
		''' </li>
		''' <li>
		''' Otherwise, the system-default implementation is returned.
		''' </li>
		''' </ul>
		''' 
		''' Once an application has obtained a reference to a
		''' <code>DocumentBuilderFactory</code> it can use the factory to
		''' configure and obtain parser instances.
		''' 
		''' 
		''' <h2>Tip for Trouble-shooting</h2>
		''' <p>Setting the <code>jaxp.debug</code> system property will cause
		''' this method to print a lot of debug messages
		''' to <code>System.err</code> about what it is doing and where it is looking at.</p>
		''' 
		''' <p> If you have problems loading <seealso cref="DocumentBuilder"/>s, try:</p>
		''' <pre>
		''' java -Djaxp.debug=1 YourProgram ....
		''' </pre>
		''' </summary>
		''' <returns> New instance of a <code>DocumentBuilderFactory</code>
		''' </returns>
		''' <exception cref="FactoryConfigurationError"> in case of {@linkplain
		''' java.util.ServiceConfigurationError service configuration error} or if
		''' the implementation is not available or cannot be instantiated. </exception>
		Public Shared Function newInstance() As DocumentBuilderFactory
			Return FactoryFinder.find(GetType(DocumentBuilderFactory), "com.sun.org.apache.xerces.internal.jaxp.DocumentBuilderFactoryImpl") ' "javax.xml.parsers.DocumentBuilderFactory"
					' The default property name according to the JAXP spec 
					' The fallback implementation class name 
		End Function

		''' <summary>
		''' <p>Obtain a new instance of a <code>DocumentBuilderFactory</code> from class name.
		''' This function is useful when there are multiple providers in the classpath.
		''' It gives more control to the application as it can specify which provider
		''' should be loaded.</p>
		''' 
		''' <p>Once an application has obtained a reference to a <code>DocumentBuilderFactory</code>
		''' it can use the factory to configure and obtain parser instances.</p>
		''' 
		''' 
		''' <h2>Tip for Trouble-shooting</h2>
		''' <p>Setting the <code>jaxp.debug</code> system property will cause
		''' this method to print a lot of debug messages
		''' to <code>System.err</code> about what it is doing and where it is looking at.</p>
		''' 
		''' <p> If you have problems try:</p>
		''' <pre>
		''' java -Djaxp.debug=1 YourProgram ....
		''' </pre>
		''' </summary>
		''' <param name="factoryClassName"> fully qualified factory class name that provides implementation of <code>javax.xml.parsers.DocumentBuilderFactory</code>.
		''' </param>
		''' <param name="classLoader"> <code>ClassLoader</code> used to load the factory class. If <code>null</code>
		'''                     current <code>Thread</code>'s context classLoader is used to load the factory class.
		''' </param>
		''' <returns> New instance of a <code>DocumentBuilderFactory</code>
		''' </returns>
		''' <exception cref="FactoryConfigurationError"> if <code>factoryClassName</code> is <code>null</code>, or
		'''                                   the factory class cannot be loaded, instantiated.
		''' </exception>
		''' <seealso cref= #newInstance()
		''' 
		''' @since 1.6 </seealso>
		Public Shared Function newInstance(ByVal factoryClassName As String, ByVal classLoader As ClassLoader) As DocumentBuilderFactory
				'do not fallback if given classloader can't find the class, throw exception
				Return FactoryFinder.newInstance(GetType(DocumentBuilderFactory), factoryClassName, classLoader, False)
		End Function

		''' <summary>
		''' Creates a new instance of a <seealso cref="javax.xml.parsers.DocumentBuilder"/>
		''' using the currently configured parameters.
		''' </summary>
		''' <returns> A new instance of a DocumentBuilder.
		''' </returns>
		''' <exception cref="ParserConfigurationException"> if a DocumentBuilder
		'''   cannot be created which satisfies the configuration requested. </exception>

		Public MustOverride Function newDocumentBuilder() As DocumentBuilder


		''' <summary>
		''' Specifies that the parser produced by this code will
		''' provide support for XML namespaces. By default the value of this is set
		''' to <code>false</code>
		''' </summary>
		''' <param name="awareness"> true if the parser produced will provide support
		'''                  for XML namespaces; false otherwise. </param>

		Public Overridable Property namespaceAware As Boolean
			Set(ByVal awareness As Boolean)
				Me.namespaceAware = awareness
			End Set
			Get
				Return namespaceAware
			End Get
		End Property

		''' <summary>
		''' Specifies that the parser produced by this code will
		''' validate documents as they are parsed. By default the value of this
		''' is set to <code>false</code>.
		''' 
		''' <p>
		''' Note that "the validation" here means
		''' <a href="http://www.w3.org/TR/REC-xml#proc-types">a validating
		''' parser</a> as defined in the XML recommendation.
		''' In other words, it essentially just controls the DTD validation.
		''' (except the legacy two properties defined in JAXP 1.2.)
		''' </p>
		''' 
		''' <p>
		''' To use modern schema languages such as W3C XML Schema or
		''' RELAX NG instead of DTD, you can configure your parser to be
		''' a non-validating parser by leaving the <seealso cref="#setValidating(boolean)"/>
		''' method <code>false</code>, then use the <seealso cref="#setSchema(Schema)"/>
		''' method to associate a schema to a parser.
		''' </p>
		''' </summary>
		''' <param name="validating"> true if the parser produced will validate documents
		'''                   as they are parsed; false otherwise. </param>

		Public Overridable Property validating As Boolean
			Set(ByVal validating As Boolean)
				Me.validating = validating
			End Set
			Get
				Return validating
			End Get
		End Property

		''' <summary>
		''' Specifies that the parsers created by this  factory must eliminate
		''' whitespace in element content (sometimes known loosely as
		''' 'ignorable whitespace') when parsing XML documents (see XML Rec
		''' 2.10). Note that only whitespace which is directly contained within
		''' element content that has an element only content model (see XML
		''' Rec 3.2.1) will be eliminated. Due to reliance on the content model
		''' this setting requires the parser to be in validating mode. By default
		''' the value of this is set to <code>false</code>.
		''' </summary>
		''' <param name="whitespace"> true if the parser created must eliminate whitespace
		'''                   in the element content when parsing XML documents;
		'''                   false otherwise. </param>

		Public Overridable Property ignoringElementContentWhitespace As Boolean
			Set(ByVal whitespace As Boolean)
				Me.whitespace = whitespace
			End Set
			Get
				Return whitespace
			End Get
		End Property

		''' <summary>
		''' Specifies that the parser produced by this code will
		''' expand entity reference nodes. By default the value of this is set to
		''' <code>true</code>
		''' </summary>
		''' <param name="expandEntityRef"> true if the parser produced will expand entity
		'''                        reference nodes; false otherwise. </param>

		Public Overridable Property expandEntityReferences As Boolean
			Set(ByVal expandEntityRef As Boolean)
				Me.expandEntityRef = expandEntityRef
			End Set
			Get
				Return expandEntityRef
			End Get
		End Property

		''' <summary>
		''' <p>Specifies that the parser produced by this code will
		''' ignore comments. By default the value of this is set to <code>false
		''' </code>.</p>
		''' </summary>
		''' <param name="ignoreComments"> <code>boolean</code> value to ignore comments during processing </param>

		Public Overridable Property ignoringComments As Boolean
			Set(ByVal ignoreComments As Boolean)
				Me.ignoreComments = ignoreComments
			End Set
			Get
				Return ignoreComments
			End Get
		End Property

		''' <summary>
		''' Specifies that the parser produced by this code will
		''' convert CDATA nodes to Text nodes and append it to the
		''' adjacent (if any) text node. By default the value of this is set to
		''' <code>false</code>
		''' </summary>
		''' <param name="coalescing">  true if the parser produced will convert CDATA nodes
		'''                    to Text nodes and append it to the adjacent (if any)
		'''                    text node; false otherwise. </param>

		Public Overridable Property coalescing As Boolean
			Set(ByVal coalescing As Boolean)
				Me.coalescing = coalescing
			End Set
			Get
				Return coalescing
			End Get
		End Property













		''' <summary>
		''' Allows the user to set specific attributes on the underlying
		''' implementation.
		''' <p>
		''' All implementations that implement JAXP 1.5 or newer are required to
		''' support the <seealso cref="javax.xml.XMLConstants#ACCESS_EXTERNAL_DTD"/> and
		''' <seealso cref="javax.xml.XMLConstants#ACCESS_EXTERNAL_SCHEMA"/> properties.
		''' </p>
		''' <ul>
		'''   <li>
		'''      <p>
		'''      Setting the <seealso cref="javax.xml.XMLConstants#ACCESS_EXTERNAL_DTD"/> property
		'''      restricts the access to external DTDs, external Entity References to the
		'''      protocols specified by the property.
		'''      If access is denied during parsing due to the restriction of this property,
		'''      <seealso cref="org.xml.sax.SAXException"/> will be thrown by the parse methods defined by
		'''      <seealso cref="javax.xml.parsers.DocumentBuilder"/>.
		'''      </p>
		'''      <p>
		'''      Setting the <seealso cref="javax.xml.XMLConstants#ACCESS_EXTERNAL_SCHEMA"/> property
		'''      restricts the access to external Schema set by the schemaLocation attribute to
		'''      the protocols specified by the property.  If access is denied during parsing
		'''      due to the restriction of this property, <seealso cref="org.xml.sax.SAXException"/>
		'''      will be thrown by the parse methods defined by
		'''      <seealso cref="javax.xml.parsers.DocumentBuilder"/>.
		'''      </p>
		'''   </li>
		''' </ul>
		''' </summary>
		''' <param name="name"> The name of the attribute. </param>
		''' <param name="value"> The value of the attribute.
		''' </param>
		''' <exception cref="IllegalArgumentException"> thrown if the underlying
		'''   implementation doesn't recognize the attribute. </exception>
		Public MustOverride Sub setAttribute(ByVal name As String, ByVal value As Object)

		''' <summary>
		''' Allows the user to retrieve specific attributes on the underlying
		''' implementation.
		''' </summary>
		''' <param name="name"> The name of the attribute.
		''' </param>
		''' <returns> value The value of the attribute.
		''' </returns>
		''' <exception cref="IllegalArgumentException"> thrown if the underlying
		'''   implementation doesn't recognize the attribute. </exception>
		Public MustOverride Function getAttribute(ByVal name As String) As Object

		''' <summary>
		''' <p>Set a feature for this <code>DocumentBuilderFactory</code> and <code>DocumentBuilder</code>s created by this factory.</p>
		''' 
		''' <p>
		''' Feature names are fully qualified <seealso cref="java.net.URI"/>s.
		''' Implementations may define their own features.
		''' A <seealso cref="ParserConfigurationException"/> is thrown if this <code>DocumentBuilderFactory</code> or the
		''' <code>DocumentBuilder</code>s it creates cannot support the feature.
		''' It is possible for a <code>DocumentBuilderFactory</code> to expose a feature value but be unable to change its state.
		''' </p>
		''' 
		''' <p>
		''' All implementations are required to support the <seealso cref="javax.xml.XMLConstants#FEATURE_SECURE_PROCESSING"/> feature.
		''' When the feature is:</p>
		''' <ul>
		'''   <li>
		'''     <code>true</code>: the implementation will limit XML processing to conform to implementation limits.
		'''     Examples include enity expansion limits and XML Schema constructs that would consume large amounts of resources.
		'''     If XML processing is limited for security reasons, it will be reported via a call to the registered
		'''    <seealso cref="org.xml.sax.ErrorHandler#fatalError(SAXParseException exception)"/>.
		'''     See <seealso cref=" DocumentBuilder#setErrorHandler(org.xml.sax.ErrorHandler errorHandler)"/>.
		'''   </li>
		'''   <li>
		'''     <code>false</code>: the implementation will processing XML according to the XML specifications without
		'''     regard to possible implementation limits.
		'''   </li>
		''' </ul>
		''' </summary>
		''' <param name="name"> Feature name. </param>
		''' <param name="value"> Is feature state <code>true</code> or <code>false</code>.
		''' </param>
		''' <exception cref="ParserConfigurationException"> if this <code>DocumentBuilderFactory</code> or the <code>DocumentBuilder</code>s
		'''   it creates cannot support this feature. </exception>
		''' <exception cref="NullPointerException"> If the <code>name</code> parameter is null. </exception>
		Public MustOverride Sub setFeature(ByVal name As String, ByVal value As Boolean)

		''' <summary>
		''' <p>Get the state of the named feature.</p>
		''' 
		''' <p>
		''' Feature names are fully qualified <seealso cref="java.net.URI"/>s.
		''' Implementations may define their own features.
		''' An <seealso cref="ParserConfigurationException"/> is thrown if this <code>DocumentBuilderFactory</code> or the
		''' <code>DocumentBuilder</code>s it creates cannot support the feature.
		''' It is possible for an <code>DocumentBuilderFactory</code> to expose a feature value but be unable to change its state.
		''' </p>
		''' </summary>
		''' <param name="name"> Feature name.
		''' </param>
		''' <returns> State of the named feature.
		''' </returns>
		''' <exception cref="ParserConfigurationException"> if this <code>DocumentBuilderFactory</code>
		'''   or the <code>DocumentBuilder</code>s it creates cannot support this feature. </exception>
		Public MustOverride Function getFeature(ByVal name As String) As Boolean


		''' <summary>
		''' Gets the <seealso cref="Schema"/> object specified through
		''' the <seealso cref="#setSchema(Schema schema)"/> method.
		''' 
		''' @return
		'''      the <seealso cref="Schema"/> object that was last set through
		'''      the <seealso cref="#setSchema(Schema)"/> method, or null
		'''      if the method was not invoked since a <seealso cref="DocumentBuilderFactory"/>
		'''      is created.
		''' </summary>
		''' <exception cref="UnsupportedOperationException"> When implementation does not
		'''   override this method.
		''' 
		''' @since 1.5 </exception>
		Public Overridable Property schema As javax.xml.validation.Schema
			Get
				Throw New System.NotSupportedException("This parser does not support specification """ & Me.GetType().Assembly.specificationTitle & """ version """ & Me.GetType().Assembly.specificationVersion & """")
    
			End Get
			Set(ByVal schema As javax.xml.validation.Schema)
				Throw New System.NotSupportedException("This parser does not support specification """ & Me.GetType().Assembly.specificationTitle & """ version """ & Me.GetType().Assembly.specificationVersion & """")
			End Set
		End Property




		''' <summary>
		''' <p>Set state of XInclude processing.</p>
		''' 
		''' <p>If XInclude markup is found in the document instance, should it be
		''' processed as specified in <a href="http://www.w3.org/TR/xinclude/">
		''' XML Inclusions (XInclude) Version 1.0</a>.</p>
		''' 
		''' <p>XInclude processing defaults to <code>false</code>.</p>
		''' </summary>
		''' <param name="state"> Set XInclude processing to <code>true</code> or
		'''   <code>false</code>
		''' </param>
		''' <exception cref="UnsupportedOperationException"> When implementation does not
		'''   override this method.
		''' 
		''' @since 1.5 </exception>
		Public Overridable Property xIncludeAware As Boolean
			Set(ByVal state As Boolean)
				If state Then Throw New System.NotSupportedException(" setXIncludeAware " & "is not supported on this JAXP" & " implementation or earlier: " & Me.GetType())
			End Set
			Get
				Throw New System.NotSupportedException("This parser does not support specification """ & Me.GetType().Assembly.specificationTitle & """ version """ & Me.GetType().Assembly.specificationVersion & """")
			End Get
		End Property

	End Class

End Namespace