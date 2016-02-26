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

Namespace javax.xml.transform

	''' <summary>
	''' <p>A TransformerFactory instance can be used to create
	''' <seealso cref="javax.xml.transform.Transformer"/> and
	''' <seealso cref="javax.xml.transform.Templates"/> objects.</p>
	''' 
	''' <p>The system property that determines which Factory implementation
	''' to create is named <code>"javax.xml.transform.TransformerFactory"</code>.
	''' This property names a concrete subclass of the
	''' <code>TransformerFactory</code> abstract class. If the property is not
	''' defined, a platform default is be used.</p>
	''' 
	''' @author <a href="mailto:Jeff.Suttor@Sun.com">Jeff Suttor</a>
	''' @author <a href="mailto:Neeraj.Bajaj@sun.com">Neeraj Bajaj</a>
	''' 
	''' @since 1.5
	''' </summary>
	Public MustInherit Class TransformerFactory

		''' <summary>
		''' Default constructor is protected on purpose.
		''' </summary>
		Protected Friend Sub New()
		End Sub



		''' <summary>
		''' <p>Obtain a new instance of a <code>TransformerFactory</code>.
		''' This static method creates a new factory instance.</p>
		''' <p>This method uses the following ordered lookup procedure to determine
		''' the <code>TransformerFactory</code> implementation class to
		''' load:</p>
		''' <ul>
		''' <li>
		''' Use the <code>javax.xml.transform.TransformerFactory</code> system
		''' property.
		''' </li>
		''' <li>
		''' Use the properties file "lib/jaxp.properties" in the JRE directory.
		''' This configuration file is in standard <code>java.util.Properties
		''' </code> format and contains the fully qualified name of the
		''' implementation class with the key being the system property defined
		''' above.
		''' <br>
		''' The jaxp.properties file is read only once by the JAXP implementation
		''' and it's values are then cached for future use.  If the file does not exist
		''' when the first attempt is made to read from it, no further attempts are
		''' made to check for its existence.  It is not possible to change the value
		''' of any property in jaxp.properties after it has been read for the first time.
		''' </li>
		''' <li>
		'''   Use the service-provider loading facilities, defined by the
		'''   <seealso cref="java.util.ServiceLoader"/> class, to attempt to locate and load an
		'''   implementation of the service using the {@linkplain
		'''   java.util.ServiceLoader#load(java.lang.Class) default loading mechanism}:
		'''   the service-provider loading facility will use the {@linkplain
		'''   java.lang.Thread#getContextClassLoader() current thread's context class loader}
		'''   to attempt to load the service. If the context class
		'''   loader is null, the {@linkplain
		'''   ClassLoader#getSystemClassLoader() system class loader} will be used.
		''' </li>
		''' <li>
		'''   Otherwise, the system-default implementation is returned.
		''' </li>
		''' </ul>
		''' 
		''' <p>Once an application has obtained a reference to a <code>
		''' TransformerFactory</code> it can use the factory to configure
		''' and obtain transformer instances.</p>
		''' </summary>
		''' <returns> new TransformerFactory instance, never null.
		''' </returns>
		''' <exception cref="TransformerFactoryConfigurationError"> Thrown in case of {@linkplain
		''' java.util.ServiceConfigurationError service configuration error} or if
		''' the implementation is not available or cannot be instantiated. </exception>
		Public Shared Function newInstance() As TransformerFactory

			Return FactoryFinder.find(GetType(TransformerFactory), "com.sun.org.apache.xalan.internal.xsltc.trax.TransformerFactoryImpl")
				' The default property name according to the JAXP spec 
				' The fallback implementation class name, XSLTC 
		End Function

		''' <summary>
		''' <p>Obtain a new instance of a <code>TransformerFactory</code> from factory class name.
		''' This function is useful when there are multiple providers in the classpath.
		''' It gives more control to the application as it can specify which provider
		''' should be loaded.</p>
		''' 
		''' <p>Once an application has obtained a reference to a <code>
		''' TransformerFactory</code> it can use the factory to configure
		''' and obtain transformer instances.</p>
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
		''' <param name="factoryClassName"> fully qualified factory class name that provides implementation of <code>javax.xml.transform.TransformerFactory</code>.
		''' </param>
		''' <param name="classLoader"> <code>ClassLoader</code> used to load the factory class. If <code>null</code>
		'''                     current <code>Thread</code>'s context classLoader is used to load the factory class.
		''' </param>
		''' <returns> new TransformerFactory instance, never null.
		''' </returns>
		''' <exception cref="TransformerFactoryConfigurationError">
		'''                    if <code>factoryClassName</code> is <code>null</code>, or
		'''                   the factory class cannot be loaded, instantiated.
		''' </exception>
		''' <seealso cref= #newInstance()
		''' 
		''' @since 1.6 </seealso>
		Public Shared Function newInstance(ByVal factoryClassName As String, ByVal classLoader As ClassLoader) As TransformerFactory

			'do not fallback if given classloader can't find the class, throw exception
			Return FactoryFinder.newInstance(GetType(TransformerFactory), factoryClassName, classLoader, False, False)
		End Function
		''' <summary>
		''' <p>Process the <code>Source</code> into a <code>Transformer</code>
		''' <code>Object</code>.  The <code>Source</code> is an XSLT document that
		''' conforms to <a href="http://www.w3.org/TR/xslt">
		''' XSL Transformations (XSLT) Version 1.0</a>.  Care must
		''' be taken not to use this <code>Transformer</code> in multiple
		''' <code>Thread</code>s running concurrently.
		''' Different <code>TransformerFactories</code> can be used concurrently by
		''' different <code>Thread</code>s.</p>
		''' </summary>
		''' <param name="source"> <code>Source </code> of XSLT document used to create
		'''   <code>Transformer</code>.
		'''   Examples of XML <code>Source</code>s include
		'''   <seealso cref="javax.xml.transform.dom.DOMSource DOMSource"/>,
		'''   <seealso cref="javax.xml.transform.sax.SAXSource SAXSource"/>, and
		'''   <seealso cref="javax.xml.transform.stream.StreamSource StreamSource"/>.
		''' </param>
		''' <returns> A <code>Transformer</code> object that may be used to perform
		'''   a transformation in a single <code>Thread</code>, never
		'''   <code>null</code>.
		''' </returns>
		''' <exception cref="TransformerConfigurationException"> Thrown if there are errors when
		'''    parsing the <code>Source</code> or it is not possible to create a
		'''   <code>Transformer</code> instance.
		''' </exception>
		''' <seealso cref= <a href="http://www.w3.org/TR/xslt">
		'''   XSL Transformations (XSLT) Version 1.0</a> </seealso>
		Public MustOverride Function newTransformer(ByVal source As Source) As Transformer

		''' <summary>
		''' <p>Create a new <code>Transformer</code> that performs a copy
		''' of the <code>Source</code> to the <code>Result</code>.
		''' i.e. the "<em>identity transform</em>".</p>
		''' </summary>
		''' <returns> A Transformer object that may be used to perform a transformation
		''' in a single thread, never null.
		''' </returns>
		''' <exception cref="TransformerConfigurationException"> When it is not
		'''   possible to create a <code>Transformer</code> instance. </exception>
		Public MustOverride Function newTransformer() As Transformer

		''' <summary>
		''' Process the Source into a Templates object, which is a
		''' a compiled representation of the source. This Templates object
		''' may then be used concurrently across multiple threads.  Creating
		''' a Templates object allows the TransformerFactory to do detailed
		''' performance optimization of transformation instructions, without
		''' penalizing runtime transformation.
		''' </summary>
		''' <param name="source"> An object that holds a URL, input stream, etc.
		''' </param>
		''' <returns> A Templates object capable of being used for transformation
		'''   purposes, never <code>null</code>.
		''' </returns>
		''' <exception cref="TransformerConfigurationException"> When parsing to
		'''   construct the Templates object fails. </exception>
		Public MustOverride Function newTemplates(ByVal source As Source) As Templates

		''' <summary>
		''' <p>Get the stylesheet specification(s) associated with the
		''' XML <code>Source</code> document via the
		''' <a href="http://www.w3.org/TR/xml-stylesheet/">
		''' xml-stylesheet processing instruction</a> that match the given criteria.
		''' Note that it is possible to return several stylesheets, in which case
		''' they are applied as if they were a list of imports or cascades in a
		''' single stylesheet.</p>
		''' </summary>
		''' <param name="source"> The XML source document. </param>
		''' <param name="media"> The media attribute to be matched.  May be null, in which
		'''      case the prefered templates will be used (i.e. alternate = no). </param>
		''' <param name="title"> The value of the title attribute to match.  May be null. </param>
		''' <param name="charset"> The value of the charset attribute to match.  May be null.
		''' </param>
		''' <returns> A <code>Source</code> <code>Object</code> suitable for passing
		'''   to the <code>TransformerFactory</code>.
		''' </returns>
		''' <exception cref="TransformerConfigurationException"> An <code>Exception</code>
		'''   is thrown if an error occurings during parsing of the
		'''   <code>source</code>.
		''' </exception>
		''' <seealso cref= <a href="http://www.w3.org/TR/xml-stylesheet/">
		'''   Associating Style Sheets with XML documents Version 1.0</a> </seealso>
		Public MustOverride Function getAssociatedStylesheet(ByVal source As Source, ByVal media As String, ByVal title As String, ByVal charset As String) As Source

		''' <summary>
		''' Set an object that is used by default during the transformation
		''' to resolve URIs used in document(), xsl:import, or xsl:include.
		''' </summary>
		''' <param name="resolver"> An object that implements the URIResolver interface,
		''' or null. </param>
		Public MustOverride Property uRIResolver As URIResolver


		'======= CONFIGURATION METHODS =======

			''' <summary>
			''' <p>Set a feature for this <code>TransformerFactory</code> and <code>Transformer</code>s
			''' or <code>Template</code>s created by this factory.</p>
			''' 
			''' <p>
			''' Feature names are fully qualified <seealso cref="java.net.URI"/>s.
			''' Implementations may define their own features.
			''' An <seealso cref="TransformerConfigurationException"/> is thrown if this <code>TransformerFactory</code> or the
			''' <code>Transformer</code>s or <code>Template</code>s it creates cannot support the feature.
			''' It is possible for an <code>TransformerFactory</code> to expose a feature value but be unable to change its state.
			''' </p>
			''' 
			''' <p>All implementations are required to support the <seealso cref="javax.xml.XMLConstants#FEATURE_SECURE_PROCESSING"/> feature.
			''' When the feature is:</p>
			''' <ul>
			'''   <li>
			'''     <code>true</code>: the implementation will limit XML processing to conform to implementation limits
			'''     and behave in a secure fashion as defined by the implementation.
			'''     Examples include resolving user defined style sheets and functions.
			'''     If XML processing is limited for security reasons, it will be reported via a call to the registered
			'''     <seealso cref="ErrorListener#fatalError(TransformerException exception)"/>.
			'''     See <seealso cref=" #setErrorListener(ErrorListener listener)"/>.
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
			''' <exception cref="TransformerConfigurationException"> if this <code>TransformerFactory</code>
			'''   or the <code>Transformer</code>s or <code>Template</code>s it creates cannot support this feature. </exception>
			''' <exception cref="NullPointerException"> If the <code>name</code> parameter is null. </exception>
			Public MustOverride Sub setFeature(ByVal name As String, ByVal value As Boolean)

		''' <summary>
		''' Look up the value of a feature.
		''' 
		''' <p>
		''' Feature names are fully qualified <seealso cref="java.net.URI"/>s.
		''' Implementations may define their own features.
		''' <code>false</code> is returned if this <code>TransformerFactory</code> or the
		''' <code>Transformer</code>s or <code>Template</code>s it creates cannot support the feature.
		''' It is possible for an <code>TransformerFactory</code> to expose a feature value but be unable to change its state.
		''' </p>
		''' </summary>
		''' <param name="name"> Feature name.
		''' </param>
		''' <returns> The current state of the feature, <code>true</code> or <code>false</code>.
		''' </returns>
		''' <exception cref="NullPointerException"> If the <code>name</code> parameter is null. </exception>
		Public MustOverride Function getFeature(ByVal name As String) As Boolean

		''' <summary>
		''' Allows the user to set specific attributes on the underlying
		''' implementation.  An attribute in this context is defined to
		''' be an option that the implementation provides.
		''' An <code>IllegalArgumentException</code> is thrown if the underlying
		''' implementation doesn't recognize the attribute.
		''' <p>
		''' All implementations that implement JAXP 1.5 or newer are required to
		''' support the <seealso cref="javax.xml.XMLConstants#ACCESS_EXTERNAL_DTD"/>  and
		''' <seealso cref="javax.xml.XMLConstants#ACCESS_EXTERNAL_STYLESHEET"/> properties.
		''' </p>
		''' <ul>
		'''   <li>
		'''      <p>
		'''      Access to external DTDs in the source file is restricted to the protocols
		'''      specified by the <seealso cref="javax.xml.XMLConstants#ACCESS_EXTERNAL_DTD"/> property.
		'''      If access is denied during transformation due to the restriction of this property,
		'''      <seealso cref="javax.xml.transform.TransformerException"/> will be thrown by
		'''      <seealso cref="javax.xml.transform.Transformer#transform(Source, Result)"/>.
		'''      </p>
		'''      <p>
		'''      Access to external DTDs in the stylesheet is restricted to the protocols
		'''      specified by the <seealso cref="javax.xml.XMLConstants#ACCESS_EXTERNAL_DTD"/> property.
		'''      If access is denied during the creation of a new transformer due to the
		'''      restriction of this property,
		'''      <seealso cref="javax.xml.transform.TransformerConfigurationException"/> will be thrown
		'''      by the <seealso cref="#newTransformer(Source)"/> method.
		'''      </p>
		'''      <p>
		'''      Access to external reference set by the stylesheet processing instruction,
		'''      Import and Include element is restricted to the protocols specified by the
		'''      <seealso cref="javax.xml.XMLConstants#ACCESS_EXTERNAL_STYLESHEET"/> property.
		'''      If access is denied during the creation of a new transformer due to the
		'''      restriction of this property,
		'''      <seealso cref="javax.xml.transform.TransformerConfigurationException"/> will be thrown
		'''      by the <seealso cref="#newTransformer(Source)"/> method.
		'''      </p>
		'''      <p>
		'''      Access to external document through XSLT document function is restricted
		'''      to the protocols specified by the property. If access is denied during
		'''      the transformation due to the restriction of this property,
		'''      <seealso cref="javax.xml.transform.TransformerException"/> will be thrown by the
		'''      <seealso cref="javax.xml.transform.Transformer#transform(Source, Result)"/> method.
		'''      </p>
		'''   </li>
		''' </ul>
		''' </summary>
		''' <param name="name"> The name of the attribute. </param>
		''' <param name="value"> The value of the attribute.
		''' </param>
		''' <exception cref="IllegalArgumentException"> When implementation does not
		'''   recognize the attribute. </exception>
		Public MustOverride Sub setAttribute(ByVal name As String, ByVal value As Object)

		''' <summary>
		''' Allows the user to retrieve specific attributes on the underlying
		''' implementation.
		''' An <code>IllegalArgumentException</code> is thrown if the underlying
		''' implementation doesn't recognize the attribute.
		''' </summary>
		''' <param name="name"> The name of the attribute.
		''' </param>
		''' <returns> value The value of the attribute.
		''' </returns>
		''' <exception cref="IllegalArgumentException"> When implementation does not
		'''   recognize the attribute. </exception>
		Public MustOverride Function getAttribute(ByVal name As String) As Object

		''' <summary>
		''' Set the error event listener for the TransformerFactory, which
		''' is used for the processing of transformation instructions,
		''' and not for the transformation itself.
		''' An <code>IllegalArgumentException</code> is thrown if the
		''' <code>ErrorListener</code> listener is <code>null</code>.
		''' </summary>
		''' <param name="listener"> The new error listener.
		''' </param>
		''' <exception cref="IllegalArgumentException"> When <code>listener</code> is
		'''   <code>null</code> </exception>
		Public MustOverride Property errorListener As ErrorListener


	End Class

End Namespace