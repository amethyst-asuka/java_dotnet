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

Namespace javax.xml.validation


	''' <summary>
	''' Factory that creates <seealso cref="Schema"/> objects&#x2E; Entry-point to
	''' the validation API.
	''' 
	''' <p>
	''' <seealso cref="SchemaFactory"/> is a schema compiler. It reads external
	''' representations of schemas and prepares them for validation.
	''' 
	''' <p>
	''' The <seealso cref="SchemaFactory"/> class is not thread-safe. In other words,
	''' it is the application's responsibility to ensure that at most
	''' one thread is using a <seealso cref="SchemaFactory"/> object at any
	''' given moment. Implementations are encouraged to mark methods
	''' as <code>synchronized</code> to protect themselves from broken clients.
	''' 
	''' <p>
	''' <seealso cref="SchemaFactory"/> is not re-entrant. While one of the
	''' <code>newSchema</code> methods is being invoked, applications
	''' may not attempt to recursively invoke the <code>newSchema</code> method,
	''' even from the same thread.
	''' 
	''' <h2><a name="schemaLanguage"></a>Schema Language</h2>
	''' <p>
	''' This spec uses a namespace URI to designate a schema language.
	''' The following table shows the values defined by this specification.
	''' <p>
	''' To be compliant with the spec, the implementation
	''' is only required to support W3C XML Schema 1.0. However,
	''' if it chooses to support other schema languages listed here,
	''' it must conform to the relevant behaviors described in this spec.
	''' 
	''' <p>
	''' Schema languages not listed here are expected to
	''' introduce their own URIs to represent themselves.
	''' The <seealso cref="SchemaFactory"/> class is capable of locating other
	''' implementations for other schema languages at run-time.
	''' 
	''' <p>
	''' Note that because the XML DTD is strongly tied to the parsing process
	''' and has a significant effect on the parsing process, it is impossible
	''' to define the DTD validation as a process independent from parsing.
	''' For this reason, this specification does not define the semantics for
	''' the XML DTD. This doesn't prohibit implementors from implementing it
	''' in a way they see fit, but <em>users are warned that any DTD
	''' validation implemented on this interface necessarily deviate from
	''' the XML DTD semantics as defined in the XML 1.0</em>.
	''' 
	''' <table border="1" cellpadding="2">
	'''   <thead>
	'''     <tr>
	'''       <th>value</th>
	'''       <th>language</th>
	'''     </tr>
	'''   </thead>
	'''   <tbody>
	'''     <tr>
	'''       <td><seealso cref="javax.xml.XMLConstants#W3C_XML_SCHEMA_NS_URI"/> ("<code>http://www.w3.org/2001/XMLSchema</code>")</td>
	'''       <td><a href="http://www.w3.org/TR/xmlschema-1">W3C XML Schema 1.0</a></td>
	'''     </tr>
	'''     <tr>
	'''       <td><seealso cref="javax.xml.XMLConstants#RELAXNG_NS_URI"/> ("<code>http://relaxng.org/ns/structure/1.0</code>")</td>
	'''       <td><a href="http://www.relaxng.org/">RELAX NG 1.0</a></td>
	'''     </tr>
	'''   </tbody>
	''' </table>
	''' 
	''' @author  <a href="mailto:Kohsuke.Kawaguchi@Sun.com">Kohsuke Kawaguchi</a>
	''' @author  <a href="mailto:Neeraj.Bajaj@sun.com">Neeraj Bajaj</a>
	''' 
	''' @since 1.5
	''' </summary>
	Public MustInherit Class SchemaFactory

		 Private Shared ss As New SecuritySupport

		''' <summary>
		''' <p>Constructor for derived classes.</p>
		''' 
		''' <p>The constructor does nothing.</p>
		''' 
		''' <p>Derived classes must create <seealso cref="SchemaFactory"/> objects that have
		''' <code>null</code> <seealso cref="ErrorHandler"/> and
		''' <code>null</code> <seealso cref="LSResourceResolver"/>.</p>
		''' </summary>
		Protected Friend Sub New()
		End Sub

		''' <summary>
		''' <p>Lookup an implementation of the <code>SchemaFactory</code> that supports the specified
		''' schema language and return it.</p>
		''' 
		''' <p>To find a <code>SchemaFactory</code> object for a given schema language,
		''' this method looks the following places in the following order
		''' where "the class loader" refers to the context class loader:</p>
		''' <ol>
		'''  <li>
		'''     If the system property
		'''     <code>"javax.xml.validation.SchemaFactory:<i>schemaLanguage</i>"</code>
		'''     is present (where <i>schemaLanguage</i> is the parameter
		'''     to this method), then its value is read
		'''     as a class name. The method will try to
		'''     create a new instance of this class by using the class loader,
		'''     and returns it if it is successfully created.
		'''   </li>
		'''   <li>
		'''     <code>$java.home/lib/jaxp.properties</code> is read and
		'''     the value associated with the key being the system property above
		'''     is looked for. If present, the value is processed just like above.
		'''   </li>
		'''   <li>
		'''   Use the service-provider loading facilities, defined by the
		'''   <seealso cref="java.util.ServiceLoader"/> class, to attempt to locate and load an
		'''   implementation of the service using the {@linkplain
		'''   java.util.ServiceLoader#load(java.lang.Class) default loading mechanism}:
		'''   the service-provider loading facility will use the {@linkplain
		'''   java.lang.Thread#getContextClassLoader() current thread's context class loader}
		'''   to attempt to load the service. If the context class
		'''   loader is null, the {@linkplain
		'''   ClassLoader#getSystemClassLoader() system class loader} will be used.
		'''   <br>
		'''   Each potential service provider is required to implement the method
		'''        <seealso cref="#isSchemaLanguageSupported(String schemaLanguage)"/>.
		'''   <br>
		'''   The first service provider found that supports the specified schema
		'''   language is returned.
		'''   <br>
		'''   In case of <seealso cref="java.util.ServiceConfigurationError"/> a
		'''   <seealso cref="SchemaFactoryConfigurationError"/> will be thrown.
		'''   </li>
		'''   <li>
		'''     Platform default <code>SchemaFactory</code> is located
		'''     in a implementation specific way. There must be a platform default
		'''     <code>SchemaFactory</code> for W3C XML Schema.
		'''   </li>
		''' </ol>
		''' 
		''' <p>If everything fails, <seealso cref="IllegalArgumentException"/> will be thrown.</p>
		''' 
		''' <p><strong>Tip for Trouble-shooting:</strong></p>
		''' <p>See <seealso cref="java.util.Properties#load(java.io.InputStream)"/> for
		''' exactly how a property file is parsed. In particular, colons ':'
		''' need to be escaped in a property file, so make sure schema language
		''' URIs are properly escaped in it. For example:</p>
		''' <pre>
		''' http\://www.w3.org/2001/XMLSchema=org.acme.foo.XSSchemaFactory
		''' </pre>
		''' </summary>
		''' <param name="schemaLanguage">
		'''      Specifies the schema language which the returned
		'''      SchemaFactory will understand. See
		'''      <a href="#schemaLanguage">the list of available
		'''      schema languages</a> for the possible values.
		''' </param>
		''' <returns> New instance of a <code>SchemaFactory</code>
		''' </returns>
		''' <exception cref="IllegalArgumentException">
		'''      If no implementation of the schema language is available. </exception>
		''' <exception cref="NullPointerException">
		'''      If the <code>schemaLanguage</code> parameter is null. </exception>
		''' <exception cref="SchemaFactoryConfigurationError">
		'''      If a configuration error is encountered.
		''' </exception>
		''' <seealso cref= #newInstance(String schemaLanguage, String factoryClassName, ClassLoader classLoader) </seealso>
		Public Shared Function newInstance(ByVal schemaLanguage As String) As SchemaFactory
			Dim cl As ClassLoader
			cl = ss.contextClassLoader

			If cl Is Nothing Then cl = GetType(SchemaFactory).classLoader

			Dim f As SchemaFactory = (New SchemaFactoryFinder(cl)).newFactory(schemaLanguage)
			If f Is Nothing Then Throw New System.ArgumentException("No SchemaFactory" & " that implements the schema language specified by: " & schemaLanguage & " could be loaded")
			Return f
		End Function

		''' <summary>
		''' <p>Obtain a new instance of a <code>SchemaFactory</code> from class name. <code>SchemaFactory</code>
		''' is returned if specified factory class name supports the specified schema language.
		''' This function is useful when there are multiple providers in the classpath.
		''' It gives more control to the application as it can specify which provider
		''' should be loaded.</p>
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
		''' <param name="schemaLanguage"> Specifies the schema language which the returned
		'''                          <code>SchemaFactory</code> will understand. See
		'''                          <a href="#schemaLanguage">the list of available
		'''                          schema languages</a> for the possible values.
		''' </param>
		''' <param name="factoryClassName"> fully qualified factory class name that provides implementation of <code>javax.xml.validation.SchemaFactory</code>.
		''' </param>
		''' <param name="classLoader"> <code>ClassLoader</code> used to load the factory class. If <code>null</code>
		'''                     current <code>Thread</code>'s context classLoader is used to load the factory class.
		''' </param>
		''' <returns> New instance of a <code>SchemaFactory</code>
		''' </returns>
		''' <exception cref="IllegalArgumentException">
		'''                   if <code>factoryClassName</code> is <code>null</code>, or
		'''                   the factory class cannot be loaded, instantiated or doesn't
		'''                   support the schema language specified in <code>schemLanguage</code>
		'''                   parameter.
		''' </exception>
		''' <exception cref="NullPointerException">
		'''      If the <code>schemaLanguage</code> parameter is null.
		''' </exception>
		''' <seealso cref= #newInstance(String schemaLanguage)
		''' 
		''' @since 1.6 </seealso>
		Public Shared Function newInstance(ByVal schemaLanguage As String, ByVal factoryClassName As String, ByVal classLoader As ClassLoader) As SchemaFactory
			Dim cl As ClassLoader = classLoader

			If cl Is Nothing Then cl = ss.contextClassLoader

			Dim f As SchemaFactory = (New SchemaFactoryFinder(cl)).createInstance(factoryClassName)
			If f Is Nothing Then Throw New System.ArgumentException("Factory " & factoryClassName & " could not be loaded to implement the schema language specified by: " & schemaLanguage)
			'if this factory supports the given schemalanguage return this factory else thrown exception
			If f.isSchemaLanguageSupported(schemaLanguage) Then
				Return f
			Else
				Throw New System.ArgumentException("Factory " & f.GetType().name & " does not implement the schema language specified by: " & schemaLanguage)
			End If

		End Function

		''' <summary>
		''' <p>Is specified schema supported by this <code>SchemaFactory</code>?</p>
		''' </summary>
		''' <param name="schemaLanguage"> Specifies the schema language which the returned <code>SchemaFactory</code> will understand.
		'''    <code>schemaLanguage</code> must specify a <a href="#schemaLanguage">valid</a> schema language.
		''' </param>
		''' <returns> <code>true</code> if <code>SchemaFactory</code> supports <code>schemaLanguage</code>, else <code>false</code>.
		''' </returns>
		''' <exception cref="NullPointerException"> If <code>schemaLanguage</code> is <code>null</code>. </exception>
		''' <exception cref="IllegalArgumentException"> If <code>schemaLanguage.length() == 0</code>
		'''   or <code>schemaLanguage</code> does not specify a <a href="#schemaLanguage">valid</a> schema language. </exception>
		Public MustOverride Function isSchemaLanguageSupported(ByVal schemaLanguage As String) As Boolean

		''' <summary>
		''' Look up the value of a feature flag.
		''' 
		''' <p>The feature name is any fully-qualified URI.  It is
		''' possible for a <seealso cref="SchemaFactory"/> to recognize a feature name but
		''' temporarily be unable to return its value.
		''' 
		''' <p>Implementors are free (and encouraged) to invent their own features,
		''' using names built on their own URIs.</p>
		''' </summary>
		''' <param name="name"> The feature name, which is a non-null fully-qualified URI.
		''' </param>
		''' <returns> The current value of the feature (true or false).
		''' </returns>
		''' <exception cref="SAXNotRecognizedException"> If the feature
		'''   value can't be assigned or retrieved. </exception>
		''' <exception cref="SAXNotSupportedException"> When the
		'''   <seealso cref="SchemaFactory"/> recognizes the feature name but
		'''   cannot determine its value at this time. </exception>
		''' <exception cref="NullPointerException"> If <code>name</code> is <code>null</code>.
		''' </exception>
		''' <seealso cref= #setFeature(String, boolean) </seealso>
		Public Overridable Function getFeature(ByVal name As String) As Boolean

			If name Is Nothing Then Throw New NullPointerException("the name parameter is null")
			Throw New org.xml.sax.SAXNotRecognizedException(name)
		End Function

		''' <summary>
		''' <p>Set a feature for this <code>SchemaFactory</code>,
		''' <seealso cref="Schema"/>s created by this factory, and by extension,
		''' <seealso cref="Validator"/>s and <seealso cref="ValidatorHandler"/>s created by
		''' those <seealso cref="Schema"/>s.
		''' </p>
		''' 
		''' <p>Implementors and developers should pay particular attention
		''' to how the special <seealso cref="Schema"/> object returned by {@link
		''' #newSchema()} is processed. In some cases, for example, when the
		''' <code>SchemaFactory</code> and the class actually loading the
		''' schema come from different implementations, it may not be possible
		''' for <code>SchemaFactory</code> features to be inherited automatically.
		''' Developers should
		''' make sure that features, such as secure processing, are explicitly
		''' set in both places.</p>
		''' 
		''' <p>The feature name is any fully-qualified URI. It is
		''' possible for a <seealso cref="SchemaFactory"/> to expose a feature value but
		''' to be unable to change the current value.</p>
		''' 
		''' <p>All implementations are required to support the <seealso cref="javax.xml.XMLConstants#FEATURE_SECURE_PROCESSING"/> feature.
		''' When the feature is:</p>
		''' <ul>
		'''   <li>
		'''     <code>true</code>: the implementation will limit XML processing to conform to implementation limits.
		'''     Examples include enity expansion limits and XML Schema constructs that would consume large amounts of resources.
		'''     If XML processing is limited for security reasons, it will be reported via a call to the registered
		'''    <seealso cref="ErrorHandler#fatalError(SAXParseException exception)"/>.
		'''     See <seealso cref="#setErrorHandler(ErrorHandler errorHandler)"/>.
		'''   </li>
		'''   <li>
		'''     <code>false</code>: the implementation will processing XML according to the XML specifications without
		'''     regard to possible implementation limits.
		'''   </li>
		''' </ul>
		''' </summary>
		''' <param name="name"> The feature name, which is a non-null fully-qualified URI. </param>
		''' <param name="value"> The requested value of the feature (true or false).
		''' </param>
		''' <exception cref="SAXNotRecognizedException"> If the feature
		'''   value can't be assigned or retrieved. </exception>
		''' <exception cref="SAXNotSupportedException"> When the
		'''   <seealso cref="SchemaFactory"/> recognizes the feature name but
		'''   cannot set the requested value. </exception>
		''' <exception cref="NullPointerException"> If <code>name</code> is <code>null</code>.
		''' </exception>
		''' <seealso cref= #getFeature(String) </seealso>
		Public Overridable Sub setFeature(ByVal name As String, ByVal value As Boolean)

			If name Is Nothing Then Throw New NullPointerException("the name parameter is null")
			Throw New org.xml.sax.SAXNotRecognizedException(name)
		End Sub

		''' <summary>
		''' Set the value of a property.
		''' 
		''' <p>The property name is any fully-qualified URI.  It is
		''' possible for a <seealso cref="SchemaFactory"/> to recognize a property name but
		''' to be unable to change the current value.</p>
		''' 
		''' <p>
		''' All implementations that implement JAXP 1.5 or newer are required to
		''' support the <seealso cref="javax.xml.XMLConstants#ACCESS_EXTERNAL_DTD"/> and
		''' <seealso cref="javax.xml.XMLConstants#ACCESS_EXTERNAL_SCHEMA"/> properties.
		''' </p>
		''' <ul>
		'''   <li>
		'''      <p>Access to external DTDs in Schema files is restricted to the protocols
		'''      specified by the <seealso cref="javax.xml.XMLConstants#ACCESS_EXTERNAL_DTD"/> property.
		'''      If access is denied during the creation of new Schema due to the restriction
		'''      of this property, <seealso cref="org.xml.sax.SAXException"/> will be thrown by the
		'''      <seealso cref="#newSchema(Source)"/> or <seealso cref="#newSchema(File)"/>
		'''      or <seealso cref="#newSchema(URL)"/> or  or <seealso cref="#newSchema(Source[])"/> method.</p>
		''' 
		'''      <p>Access to external DTDs in xml source files is restricted to the protocols
		'''      specified by the <seealso cref="javax.xml.XMLConstants#ACCESS_EXTERNAL_DTD"/> property.
		'''      If access is denied during validation due to the restriction
		'''      of this property, <seealso cref="org.xml.sax.SAXException"/> will be thrown by the
		'''      <seealso cref="javax.xml.validation.Validator#validate(Source)"/> or
		'''      <seealso cref="javax.xml.validation.Validator#validate(Source, Result)"/> method.</p>
		''' 
		'''      <p>Access to external reference set by the schemaLocation attribute is
		'''      restricted to the protocols specified by the
		'''      <seealso cref="javax.xml.XMLConstants#ACCESS_EXTERNAL_SCHEMA"/> property.
		'''      If access is denied during validation due to the restriction of this property,
		'''      <seealso cref="org.xml.sax.SAXException"/> will be thrown by the
		'''      <seealso cref="javax.xml.validation.Validator#validate(Source)"/> or
		'''      <seealso cref="javax.xml.validation.Validator#validate(Source, Result)"/> method.</p>
		''' 
		'''      <p>Access to external reference set by the Import
		'''      and Include element is restricted to the protocols specified by the
		'''      <seealso cref="javax.xml.XMLConstants#ACCESS_EXTERNAL_SCHEMA"/> property.
		'''      If access is denied during the creation of new Schema due to the restriction
		'''      of this property, <seealso cref="org.xml.sax.SAXException"/> will be thrown by the
		'''      <seealso cref="#newSchema(Source)"/> or <seealso cref="#newSchema(File)"/>
		'''      or <seealso cref="#newSchema(URL)"/> or <seealso cref="#newSchema(Source[])"/> method.</p>
		'''   </li>
		''' </ul>
		''' </summary>
		''' <param name="name"> The property name, which is a non-null fully-qualified URI. </param>
		''' <param name="object"> The requested value for the property.
		''' </param>
		''' <exception cref="SAXNotRecognizedException"> If the property
		'''   value can't be assigned or retrieved. </exception>
		''' <exception cref="SAXNotSupportedException"> When the
		'''   <seealso cref="SchemaFactory"/> recognizes the property name but
		'''   cannot set the requested value. </exception>
		''' <exception cref="NullPointerException"> If <code>name</code> is <code>null</code>. </exception>
		Public Overridable Sub setProperty(ByVal name As String, ByVal [object] As Object)

			If name Is Nothing Then Throw New NullPointerException("the name parameter is null")
			Throw New org.xml.sax.SAXNotRecognizedException(name)
		End Sub

		''' <summary>
		''' Look up the value of a property.
		''' 
		''' <p>The property name is any fully-qualified URI.  It is
		''' possible for a <seealso cref="SchemaFactory"/> to recognize a property name but
		''' temporarily be unable to return its value.</p>
		''' 
		''' <p><seealso cref="SchemaFactory"/>s are not required to recognize any specific
		''' property names.</p>
		''' 
		''' <p>Implementors are free (and encouraged) to invent their own properties,
		''' using names built on their own URIs.</p>
		''' </summary>
		''' <param name="name"> The property name, which is a non-null fully-qualified URI.
		''' </param>
		''' <returns> The current value of the property.
		''' </returns>
		''' <exception cref="SAXNotRecognizedException"> If the property
		'''   value can't be assigned or retrieved. </exception>
		''' <exception cref="SAXNotSupportedException"> When the
		'''   XMLReader recognizes the property name but
		'''   cannot determine its value at this time. </exception>
		''' <exception cref="NullPointerException"> If <code>name</code> is <code>null</code>.
		''' </exception>
		''' <seealso cref= #setProperty(String, Object) </seealso>
		Public Overridable Function getProperty(ByVal name As String) As Object

			If name Is Nothing Then Throw New NullPointerException("the name parameter is null")
			Throw New org.xml.sax.SAXNotRecognizedException(name)
		End Function

		''' <summary>
		''' Sets the <seealso cref="ErrorHandler"/> to receive errors encountered
		''' during the <code>newSchema</code> method invocation.
		''' 
		''' <p>
		''' Error handler can be used to customize the error handling process
		''' during schema parsing. When an <seealso cref="ErrorHandler"/> is set,
		''' errors found during the parsing of schemas will be first sent
		''' to the <seealso cref="ErrorHandler"/>.
		''' 
		''' <p>
		''' The error handler can abort the parsing of a schema immediately
		''' by throwing <seealso cref="SAXException"/> from the handler. Or for example
		''' it can print an error to the screen and try to continue the
		''' processing by returning normally from the <seealso cref="ErrorHandler"/>
		''' 
		''' <p>
		''' If any <seealso cref="Throwable"/> (or instances of its derived classes)
		''' is thrown from an <seealso cref="ErrorHandler"/>,
		''' the caller of the <code>newSchema</code> method will be thrown
		''' the same <seealso cref="Throwable"/> object.
		''' 
		''' <p>
		''' <seealso cref="SchemaFactory"/> is not allowed to
		''' throw <seealso cref="SAXException"/> without first reporting it to
		''' <seealso cref="ErrorHandler"/>.
		''' 
		''' <p>
		''' Applications can call this method even during a <seealso cref="Schema"/>
		''' is being parsed.
		''' 
		''' <p>
		''' When the <seealso cref="ErrorHandler"/> is null, the implementation will
		''' behave as if the following <seealso cref="ErrorHandler"/> is set:
		''' <pre>
		''' class DraconianErrorHandler implements <seealso cref="ErrorHandler"/> {
		'''     public void fatalError( <seealso cref="org.xml.sax.SAXParseException"/> e ) throws <seealso cref="SAXException"/> {
		'''         throw e;
		'''     }
		'''     public void error( <seealso cref="org.xml.sax.SAXParseException"/> e ) throws <seealso cref="SAXException"/> {
		'''         throw e;
		'''     }
		'''     public void warning( <seealso cref="org.xml.sax.SAXParseException"/> e ) throws <seealso cref="SAXException"/> {
		'''         // noop
		'''     }
		''' }
		''' </pre>
		''' 
		''' <p>
		''' When a new <seealso cref="SchemaFactory"/> object is created, initially
		''' this field is set to null. This field will <em>NOT</em> be
		''' inherited to <seealso cref="Schema"/>s, <seealso cref="Validator"/>s, or
		''' <seealso cref="ValidatorHandler"/>s that are created from this <seealso cref="SchemaFactory"/>.
		''' </summary>
		''' <param name="errorHandler"> A new error handler to be set.
		'''   This parameter can be <code>null</code>. </param>
		Public MustOverride Property errorHandler As org.xml.sax.ErrorHandler


		''' <summary>
		''' Sets the <seealso cref="LSResourceResolver"/> to customize
		''' resource resolution when parsing schemas.
		''' 
		''' <p>
		''' <seealso cref="SchemaFactory"/> uses a <seealso cref="LSResourceResolver"/>
		''' when it needs to locate external resources while parsing schemas,
		''' although exactly what constitutes "locating external resources" is
		''' up to each schema language. For example, for W3C XML Schema,
		''' this includes files <code>&lt;include></code>d or <code>&lt;import></code>ed,
		''' and DTD referenced from schema files, etc.
		''' 
		''' <p>
		''' Applications can call this method even during a <seealso cref="Schema"/>
		''' is being parsed.
		''' 
		''' <p>
		''' When the <seealso cref="LSResourceResolver"/> is null, the implementation will
		''' behave as if the following <seealso cref="LSResourceResolver"/> is set:
		''' <pre>
		''' class DumbDOMResourceResolver implements <seealso cref="LSResourceResolver"/> {
		'''     public <seealso cref="org.w3c.dom.ls.LSInput"/> resolveResource(
		'''         String publicId, String systemId, String baseURI) {
		''' 
		'''         return null; // always return null
		'''     }
		''' }
		''' </pre>
		''' 
		''' <p>
		''' If a <seealso cref="LSResourceResolver"/> throws a <seealso cref="RuntimeException"/>
		'''  (or instances of its derived classes),
		''' then the <seealso cref="SchemaFactory"/> will abort the parsing and
		''' the caller of the <code>newSchema</code> method will receive
		''' the same <seealso cref="RuntimeException"/>.
		''' 
		''' <p>
		''' When a new <seealso cref="SchemaFactory"/> object is created, initially
		''' this field is set to null.  This field will <em>NOT</em> be
		''' inherited to <seealso cref="Schema"/>s, <seealso cref="Validator"/>s, or
		''' <seealso cref="ValidatorHandler"/>s that are created from this <seealso cref="SchemaFactory"/>.
		''' </summary>
		''' <param name="resourceResolver">
		'''      A new resource resolver to be set. This parameter can be null. </param>
		Public MustOverride Property resourceResolver As org.w3c.dom.ls.LSResourceResolver


		''' <summary>
		''' <p>Parses the specified source as a schema and returns it as a schema.</p>
		''' 
		''' <p>This is a convenience method for <seealso cref="#newSchema(Source[] schemas)"/>.</p>
		''' </summary>
		''' <param name="schema"> Source that represents a schema.
		''' </param>
		''' <returns> New <code>Schema</code> from parsing <code>schema</code>.
		''' </returns>
		''' <exception cref="SAXException"> If a SAX error occurs during parsing. </exception>
		''' <exception cref="NullPointerException"> if <code>schema</code> is null. </exception>
		Public Overridable Function newSchema(ByVal schema As javax.xml.transform.Source) As Schema
			Return newSchema(New javax.xml.transform.Source(){schema})
		End Function

		''' <summary>
		''' <p>Parses the specified <code>File</code> as a schema and returns it as a <code>Schema</code>.</p>
		''' 
		''' <p>This is a convenience method for <seealso cref="#newSchema(Source schema)"/>.</p>
		''' </summary>
		''' <param name="schema"> File that represents a schema.
		''' </param>
		''' <returns> New <code>Schema</code> from parsing <code>schema</code>.
		''' </returns>
		''' <exception cref="SAXException"> If a SAX error occurs during parsing. </exception>
		''' <exception cref="NullPointerException"> if <code>schema</code> is null. </exception>
		Public Overridable Function newSchema(ByVal schema As java.io.File) As Schema
			Return newSchema(New javax.xml.transform.stream.StreamSource(schema))
		End Function

		''' <summary>
		''' <p>Parses the specified <code>URL</code> as a schema and returns it as a <code>Schema</code>.</p>
		''' 
		''' <p>This is a convenience method for <seealso cref="#newSchema(Source schema)"/>.</p>
		''' </summary>
		''' <param name="schema"> <code>URL</code> that represents a schema.
		''' </param>
		''' <returns> New <code>Schema</code> from parsing <code>schema</code>.
		''' </returns>
		''' <exception cref="SAXException"> If a SAX error occurs during parsing. </exception>
		''' <exception cref="NullPointerException"> if <code>schema</code> is null. </exception>
		Public Overridable Function newSchema(ByVal schema As java.net.URL) As Schema
			Return newSchema(New javax.xml.transform.stream.StreamSource(schema.toExternalForm()))
		End Function

		''' <summary>
		''' Parses the specified source(s) as a schema and returns it as a schema.
		''' 
		''' <p>
		''' The callee will read all the <seealso cref="Source"/>s and combine them into a
		''' single schema. The exact semantics of the combination depends on the schema
		''' language that this <seealso cref="SchemaFactory"/> object is created for.
		''' 
		''' <p>
		''' When an <seealso cref="ErrorHandler"/> is set, the callee will report all the errors
		''' found in sources to the handler. If the handler throws an exception, it will
		''' abort the schema compilation and the same exception will be thrown from
		''' this method. Also, after an error is reported to a handler, the callee is allowed
		''' to abort the further processing by throwing it. If an error handler is not set,
		''' the callee will throw the first error it finds in the sources.
		''' 
		''' <h2>W3C XML Schema 1.0</h2>
		''' <p>
		''' The resulting schema contains components from the specified sources.
		''' The same result would be achieved if all these sources were
		''' imported, using appropriate values for schemaLocation and namespace,
		''' into a single schema document with a different targetNamespace
		''' and no components of its own, if the import elements were given
		''' in the same order as the sources.  Section 4.2.3 of the XML Schema
		''' recommendation describes the options processors have in this
		''' regard.  While a processor should be consistent in its treatment of
		''' JAXP schema sources and XML Schema imports, the behaviour between
		''' JAXP-compliant parsers may vary; in particular, parsers may choose
		''' to ignore all but the first &lt;import> for a given namespace,
		''' regardless of information provided in schemaLocation.
		''' 
		''' <p>
		''' If the parsed set of schemas includes error(s) as
		''' specified in the section 5.1 of the XML Schema spec, then
		''' the error must be reported to the <seealso cref="ErrorHandler"/>.
		''' 
		''' <h2>RELAX NG</h2>
		''' 
		''' <p>For RELAX NG, this method must throw <seealso cref="UnsupportedOperationException"/>
		''' if <code>schemas.length!=1</code>.
		''' 
		''' </summary>
		''' <param name="schemas">
		'''      inputs to be parsed. <seealso cref="SchemaFactory"/> is required
		'''      to recognize <seealso cref="javax.xml.transform.sax.SAXSource"/>,
		'''      <seealso cref="StreamSource"/>,
		'''      <seealso cref="javax.xml.transform.stax.StAXSource"/>,
		'''      and <seealso cref="javax.xml.transform.dom.DOMSource"/>.
		'''      Input schemas must be XML documents or
		'''      XML elements and must not be null. For backwards compatibility,
		'''      the results of passing anything other than
		'''      a document or element are implementation-dependent.
		'''      Implementations must either recognize and process the input
		'''      or thrown an IllegalArgumentException.
		''' 
		''' @return
		'''      Always return a non-null valid <seealso cref="Schema"/> object.
		'''      Note that when an error has been reported, there is no
		'''      guarantee that the returned <seealso cref="Schema"/> object is
		'''      meaningful.
		''' </param>
		''' <exception cref="SAXException">
		'''      If an error is found during processing the specified inputs.
		'''      When an <seealso cref="ErrorHandler"/> is set, errors are reported to
		'''      there first. See <seealso cref="#setErrorHandler(ErrorHandler)"/>. </exception>
		''' <exception cref="NullPointerException">
		'''      If the <code>schemas</code> parameter itself is null or
		'''      any item in the array is null. </exception>
		''' <exception cref="IllegalArgumentException">
		'''      If any item in the array is not recognized by this method. </exception>
		''' <exception cref="UnsupportedOperationException">
		'''      If the schema language doesn't support this operation. </exception>
		Public MustOverride Function newSchema(ByVal schemas As javax.xml.transform.Source()) As Schema

		''' <summary>
		''' Creates a special <seealso cref="Schema"/> object.
		''' 
		''' <p>The exact semantics of the returned <seealso cref="Schema"/> object
		''' depend on the schema language for which this <seealso cref="SchemaFactory"/>
		''' is created.
		''' 
		''' <p>Also, implementations are allowed to use implementation-specific
		''' property/feature to alter the semantics of this method.</p>
		''' 
		''' <p>Implementors and developers should pay particular attention
		''' to how the features set on this <seealso cref="SchemaFactory"/> are
		''' processed by this special <seealso cref="Schema"/>.
		''' In some cases, for example, when the
		''' <seealso cref="SchemaFactory"/> and the class actually loading the
		''' schema come from different implementations, it may not be possible
		''' for <seealso cref="SchemaFactory"/> features to be inherited automatically.
		''' Developers should
		''' make sure that features, such as secure processing, are explicitly
		''' set in both places.</p>
		''' 
		''' <h2>W3C XML Schema 1.0</h2>
		''' <p>
		''' For XML Schema, this method creates a <seealso cref="Schema"/> object that
		''' performs validation by using location hints specified in documents.
		''' 
		''' <p>
		''' The returned <seealso cref="Schema"/> object assumes that if documents
		''' refer to the same URL in the schema location hints,
		''' they will always resolve to the same schema document. This
		''' asusmption allows implementations to reuse parsed results of
		''' schema documents so that multiple validations against the same
		''' schema will run faster.
		''' 
		''' <p>
		''' Note that the use of schema location hints introduces a
		''' vulnerability to denial-of-service attacks.
		''' 
		''' 
		''' <h2>RELAX NG</h2>
		''' <p>
		''' RELAX NG does not support this operation.
		''' 
		''' @return
		'''      Always return non-null valid <seealso cref="Schema"/> object.
		''' </summary>
		''' <exception cref="UnsupportedOperationException">
		'''      If this operation is not supported by the callee. </exception>
		''' <exception cref="SAXException">
		'''      If this operation is supported but failed for some reason. </exception>
		Public MustOverride Function newSchema() As Schema
	End Class

End Namespace