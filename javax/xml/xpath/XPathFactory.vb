Imports System

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

Namespace javax.xml.xpath

	''' <summary>
	''' <p>An <code>XPathFactory</code> instance can be used to create
	''' <seealso cref="javax.xml.xpath.XPath"/> objects.</p>
	''' 
	''' <p>See <seealso cref="#newInstance(String uri)"/> for lookup mechanism.</p>
	''' 
	''' <p>The <seealso cref="XPathFactory"/> class is not thread-safe. In other words,
	''' it is the application's responsibility to ensure that at most
	''' one thread is using a <seealso cref="XPathFactory"/> object at any
	''' given moment. Implementations are encouraged to mark methods
	''' as <code>synchronized</code> to protect themselves from broken clients.
	''' 
	''' <p><seealso cref="XPathFactory"/> is not re-entrant. While one of the
	''' <code>newInstance</code> methods is being invoked, applications
	''' may not attempt to recursively invoke a <code>newInstance</code> method,
	''' even from the same thread.
	''' 
	''' @author  <a href="mailto:Norman.Walsh@Sun.com">Norman Walsh</a>
	''' @author  <a href="mailto:Jeff.Suttor@Sun.com">Jeff Suttor</a>
	''' 
	''' @since 1.5
	''' </summary>
	Public MustInherit Class XPathFactory


		''' <summary>
		''' <p>The default property name according to the JAXP spec.</p>
		''' </summary>
		Public Const DEFAULT_PROPERTY_NAME As String = "javax.xml.xpath.XPathFactory"

		''' <summary>
		''' <p>Default Object Model URI.</p>
		''' </summary>
		Public Const DEFAULT_OBJECT_MODEL_URI As String = "http://java.sun.com/jaxp/xpath/dom"

		''' <summary>
		''' <p> Take care of restrictions imposed by java security model </p>
		''' </summary>
		Private Shared ss As New SecuritySupport

		''' <summary>
		''' <p>Protected constructor as <seealso cref="#newInstance()"/> or <seealso cref="#newInstance(String uri)"/>
		''' or <seealso cref="#newInstance(String uri, String factoryClassName, ClassLoader classLoader)"/>
		''' should be used to create a new instance of an <code>XPathFactory</code>.</p>
		''' </summary>
		Protected Friend Sub New()
		End Sub

		''' <summary>
		''' <p>Get a new <code>XPathFactory</code> instance using the default object model,
		''' <seealso cref="#DEFAULT_OBJECT_MODEL_URI"/>,
		''' the W3C DOM.</p>
		''' 
		''' <p>This method is functionally equivalent to:</p>
		''' <pre>
		'''   newInstance(DEFAULT_OBJECT_MODEL_URI)
		''' </pre>
		''' 
		''' <p>Since the implementation for the W3C DOM is always available, this method will never fail.</p>
		''' </summary>
		''' <returns> Instance of an <code>XPathFactory</code>.
		''' </returns>
		''' <exception cref="RuntimeException"> When there is a failure in creating an
		'''   <code>XPathFactory</code> for the default object model. </exception>
		Public Shared Function newInstance() As XPathFactory

			Try
					Return newInstance(DEFAULT_OBJECT_MODEL_URI)
			Catch xpathFactoryConfigurationException As XPathFactoryConfigurationException
					Throw New Exception("XPathFactory#newInstance() failed to create an XPathFactory for the default object model: " & DEFAULT_OBJECT_MODEL_URI & " with the XPathFactoryConfigurationException: " & xpathFactoryConfigurationException.ToString())
			End Try
		End Function

		''' <summary>
		''' <p>Get a new <code>XPathFactory</code> instance using the specified object model.</p>
		''' 
		''' <p>To find a <code>XPathFactory</code> object,
		''' this method looks the following places in the following order where "the class loader" refers to the context class loader:</p>
		''' <ol>
		'''   <li>
		'''     If the system property <seealso cref="#DEFAULT_PROPERTY_NAME"/> + ":uri" is present,
		'''     where uri is the parameter to this method, then its value is read as a class name.
		'''     The method will try to create a new instance of this class by using the class loader,
		'''     and returns it if it is successfully created.
		'''   </li>
		'''   <li>
		'''     ${java.home}/lib/jaxp.properties is read and the value associated with the key being the system property above is looked for.
		'''     If present, the value is processed just like above.
		'''   </li>
		'''   <li>
		'''     Use the service-provider loading facilities, defined by the
		'''     <seealso cref="java.util.ServiceLoader"/> class, to attempt to locate and load an
		'''     implementation of the service using the {@linkplain
		'''     java.util.ServiceLoader#load(java.lang.Class) default loading mechanism}:
		'''     the service-provider loading facility will use the {@linkplain
		'''     java.lang.Thread#getContextClassLoader() current thread's context class loader}
		'''     to attempt to load the service. If the context class
		'''     loader is null, the {@linkplain
		'''     ClassLoader#getSystemClassLoader() system class loader} will be used.
		'''     <br>
		'''     Each potential service provider is required to implement the method
		'''     <seealso cref="#isObjectModelSupported(String objectModel)"/>.
		'''     The first service provider found that supports the specified object
		'''     model is returned.
		'''     <br>
		'''     In case of <seealso cref="java.util.ServiceConfigurationError"/> an
		'''     <seealso cref="XPathFactoryConfigurationException"/> will be thrown.
		'''   </li>
		'''   <li>
		'''     Platform default <code>XPathFactory</code> is located in a platform specific way.
		'''     There must be a platform default XPathFactory for the W3C DOM, i.e. <seealso cref="#DEFAULT_OBJECT_MODEL_URI"/>.
		'''   </li>
		''' </ol>
		''' <p>If everything fails, an <code>XPathFactoryConfigurationException</code> will be thrown.</p>
		''' 
		''' <p>Tip for Trouble-shooting:</p>
		''' <p>See <seealso cref="java.util.Properties#load(java.io.InputStream)"/> for exactly how a property file is parsed.
		''' In particular, colons ':' need to be escaped in a property file, so make sure the URIs are properly escaped in it.
		''' For example:</p>
		''' <pre>
		'''   http\://java.sun.com/jaxp/xpath/dom=org.acme.DomXPathFactory
		''' </pre>
		''' </summary>
		''' <param name="uri"> Identifies the underlying object model.
		'''   The specification only defines the URI <seealso cref="#DEFAULT_OBJECT_MODEL_URI"/>,
		'''   <code>http://java.sun.com/jaxp/xpath/dom</code> for the W3C DOM,
		'''   the org.w3c.dom package, and implementations are free to introduce other URIs for other object models.
		''' </param>
		''' <returns> Instance of an <code>XPathFactory</code>.
		''' </returns>
		''' <exception cref="XPathFactoryConfigurationException"> If the specified object model
		'''      is unavailable, or if there is a configuration error. </exception>
		''' <exception cref="NullPointerException"> If <code>uri</code> is <code>null</code>. </exception>
		''' <exception cref="IllegalArgumentException"> If <code>uri</code> is <code>null</code>
		'''   or <code>uri.length() == 0</code>. </exception>
		Public Shared Function newInstance(ByVal uri As String) As XPathFactory

			If uri Is Nothing Then Throw New NullPointerException("XPathFactory#newInstance(String uri) cannot be called with uri == null")

			If uri.Length = 0 Then Throw New System.ArgumentException("XPathFactory#newInstance(String uri) cannot be called with uri == """"")

			Dim classLoader As ClassLoader = ss.contextClassLoader

			If classLoader Is Nothing Then classLoader = GetType(XPathFactory).classLoader

			Dim xpathFactory As XPathFactory = (New XPathFactoryFinder(classLoader)).newFactory(uri)

			If xpathFactory Is Nothing Then Throw New XPathFactoryConfigurationException("No XPathFactory implementation found for the object model: " & uri)

			Return xpathFactory
		End Function

		''' <summary>
		''' <p>Obtain a new instance of a <code>XPathFactory</code> from a factory class name. <code>XPathFactory</code>
		''' is returned if specified factory class supports the specified object model.
		''' This function is useful when there are multiple providers in the classpath.
		''' It gives more control to the application as it can specify which provider
		''' should be loaded.</p>
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
		''' <param name="uri">         Identifies the underlying object model. The specification only defines the URI
		'''                    <seealso cref="#DEFAULT_OBJECT_MODEL_URI"/>,<code>http://java.sun.com/jaxp/xpath/dom</code>
		'''                    for the W3C DOM, the org.w3c.dom package, and implementations are free to introduce
		'''                    other URIs for other object models.
		''' </param>
		''' <param name="factoryClassName"> fully qualified factory class name that provides implementation of <code>javax.xml.xpath.XPathFactory</code>.
		''' </param>
		''' <param name="classLoader"> <code>ClassLoader</code> used to load the factory class. If <code>null</code>
		'''                     current <code>Thread</code>'s context classLoader is used to load the factory class.
		''' 
		''' </param>
		''' <returns> New instance of a <code>XPathFactory</code>
		''' </returns>
		''' <exception cref="XPathFactoryConfigurationException">
		'''                   if <code>factoryClassName</code> is <code>null</code>, or
		'''                   the factory class cannot be loaded, instantiated
		'''                   or the factory class does not support the object model specified
		'''                   in the <code>uri</code> parameter.
		''' </exception>
		''' <exception cref="NullPointerException"> If <code>uri</code> is <code>null</code>. </exception>
		''' <exception cref="IllegalArgumentException"> If <code>uri</code> is <code>null</code>
		'''          or <code>uri.length() == 0</code>.
		''' </exception>
		''' <seealso cref= #newInstance() </seealso>
		''' <seealso cref= #newInstance(String uri)
		''' 
		''' @since 1.6 </seealso>
		Public Shared Function newInstance(ByVal uri As String, ByVal factoryClassName As String, ByVal classLoader As ClassLoader) As XPathFactory
			Dim cl As ClassLoader = classLoader

			If uri Is Nothing Then Throw New NullPointerException("XPathFactory#newInstance(String uri) cannot be called with uri == null")

			If uri.Length = 0 Then Throw New System.ArgumentException("XPathFactory#newInstance(String uri) cannot be called with uri == """"")

			If cl Is Nothing Then cl = ss.contextClassLoader

			Dim f As XPathFactory = (New XPathFactoryFinder(cl)).createInstance(factoryClassName)

			If f Is Nothing Then Throw New XPathFactoryConfigurationException("No XPathFactory implementation found for the object model: " & uri)
			'if this factory supports the given schemalanguage return this factory else thrown exception
			If f.isObjectModelSupported(uri) Then
				Return f
			Else
				Throw New XPathFactoryConfigurationException("Factory " & factoryClassName & " doesn't support given " & uri & " object model")
			End If

		End Function

		''' <summary>
		''' <p>Is specified object model supported by this <code>XPathFactory</code>?</p>
		''' </summary>
		''' <param name="objectModel"> Specifies the object model which the returned <code>XPathFactory</code> will understand.
		''' </param>
		''' <returns> <code>true</code> if <code>XPathFactory</code> supports <code>objectModel</code>, else <code>false</code>.
		''' </returns>
		''' <exception cref="NullPointerException"> If <code>objectModel</code> is <code>null</code>. </exception>
		''' <exception cref="IllegalArgumentException"> If <code>objectModel.length() == 0</code>. </exception>
		Public MustOverride Function isObjectModelSupported(ByVal objectModel As String) As Boolean

		''' <summary>
		''' <p>Set a feature for this <code>XPathFactory</code> and
		''' <code>XPath</code>s created by this factory.</p>
		''' 
		''' <p>
		''' Feature names are fully qualified <seealso cref="java.net.URI"/>s.
		''' Implementations may define their own features.
		''' An <seealso cref="XPathFactoryConfigurationException"/> is thrown if this
		''' <code>XPathFactory</code> or the <code>XPath</code>s
		''' it creates cannot support the feature.
		''' It is possible for an <code>XPathFactory</code> to expose a feature value
		''' but be unable to change its state.
		''' </p>
		''' 
		''' <p>
		''' All implementations are required to support the <seealso cref="javax.xml.XMLConstants#FEATURE_SECURE_PROCESSING"/> feature.
		''' When the feature is <code>true</code>, any reference to  an external function is an error.
		''' Under these conditions, the implementation must not call the <seealso cref="XPathFunctionResolver"/>
		''' and must throw an <seealso cref="XPathFunctionException"/>.
		''' </p>
		''' </summary>
		''' <param name="name"> Feature name. </param>
		''' <param name="value"> Is feature state <code>true</code> or <code>false</code>.
		''' </param>
		''' <exception cref="XPathFactoryConfigurationException"> if this <code>XPathFactory</code> or the <code>XPath</code>s
		'''   it creates cannot support this feature. </exception>
		''' <exception cref="NullPointerException"> if <code>name</code> is <code>null</code>. </exception>
		Public MustOverride Sub setFeature(ByVal name As String, ByVal value As Boolean)

		''' <summary>
		''' <p>Get the state of the named feature.</p>
		''' 
		''' <p>
		''' Feature names are fully qualified <seealso cref="java.net.URI"/>s.
		''' Implementations may define their own features.
		''' An <seealso cref="XPathFactoryConfigurationException"/> is thrown if this
		''' <code>XPathFactory</code> or the <code>XPath</code>s
		''' it creates cannot support the feature.
		''' It is possible for an <code>XPathFactory</code> to expose a feature value
		''' but be unable to change its state.
		''' </p>
		''' </summary>
		''' <param name="name"> Feature name.
		''' </param>
		''' <returns> State of the named feature.
		''' </returns>
		''' <exception cref="XPathFactoryConfigurationException"> if this
		'''   <code>XPathFactory</code> or the <code>XPath</code>s
		'''   it creates cannot support this feature. </exception>
		''' <exception cref="NullPointerException"> if <code>name</code> is <code>null</code>. </exception>
		Public MustOverride Function getFeature(ByVal name As String) As Boolean

		''' <summary>
		''' <p>Establish a default variable resolver.</p>
		''' 
		''' <p>Any <code>XPath</code> objects constructed from this factory will use
		''' the specified resolver by default.</p>
		''' 
		''' <p>A <code>NullPointerException</code> is thrown if <code>resolver</code>
		''' is <code>null</code>.</p>
		''' </summary>
		''' <param name="resolver"> Variable resolver.
		''' </param>
		''' <exception cref="NullPointerException"> If <code>resolver</code> is
		'''   <code>null</code>. </exception>
		Public MustOverride WriteOnly Property xPathVariableResolver As XPathVariableResolver

		''' <summary>
		''' <p>Establish a default function resolver.</p>
		''' 
		''' <p>Any <code>XPath</code> objects constructed from this factory will
		''' use the specified resolver by default.</p>
		''' 
		''' <p>A <code>NullPointerException</code> is thrown if
		''' <code>resolver</code> is <code>null</code>.</p>
		''' </summary>
		''' <param name="resolver"> XPath function resolver.
		''' </param>
		''' <exception cref="NullPointerException"> If <code>resolver</code> is
		'''   <code>null</code>. </exception>
		Public MustOverride WriteOnly Property xPathFunctionResolver As XPathFunctionResolver

		''' <summary>
		''' <p>Return a new <code>XPath</code> using the underlying object
		''' model determined when the <code>XPathFactory</code> was instantiated.</p>
		''' </summary>
		''' <returns> New instance of an <code>XPath</code>. </returns>
		Public MustOverride Function newXPath() As XPath
	End Class

End Namespace