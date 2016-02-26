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
' * Copyright (c) 2009, 2013, by Oracle Corporation. All Rights Reserved.
' 

Namespace javax.xml.stream


	''' <summary>
	''' Defines an abstract implementation of a factory for
	''' getting XMLEventWriters and XMLStreamWriters.
	''' 
	''' The following table defines the standard properties of this specification.
	''' Each property varies in the level of support required by each implementation.
	''' The level of support required is described in the 'Required' column.
	''' 
	'''     <table border="2" rules="all" cellpadding="4">
	'''     <thead>
	'''      <tr>
	'''        <th align="center" colspan="2">
	'''          Configuration parameters
	'''        </th>
	'''      </tr>
	'''    </thead>
	'''    <tbody>
	'''      <tr>
	'''        <th>Property Name</th>
	'''        <th>Behavior</th>
	'''        <th>Return type</th>
	'''        <th>Default Value</th>
	'''        <th>Required</th>
	'''              </tr>
	'''         <tr><td>javax.xml.stream.isRepairingNamespaces</td><td>defaults prefixes on the output side</td><td>Boolean</td><td>False</td><td>Yes</td></tr>
	'''      </tbody>
	'''   </table>
	''' 
	''' <p>The following paragraphs describe the namespace and prefix repair algorithm:</p>
	''' 
	''' <p>The property can be set with the following code line:
	''' <code>setProperty("javax.xml.stream.isRepairingNamespaces",new Boolean(true|false));</code></p>
	''' 
	''' <p>This property specifies that the writer default namespace prefix declarations.
	''' The default value is false. </p>
	''' 
	''' <p>If a writer isRepairingNamespaces it will create a namespace declaration
	''' on the current StartElement for
	''' any attribute that does not
	''' currently have a namespace declaration in scope.  If the StartElement
	''' has a uri but no prefix specified a prefix will be assigned, if the prefix
	''' has not been declared in a parent of the current StartElement it will be declared
	''' on the current StartElement.  If the defaultNamespace is bound and in scope
	''' and the default namespace matches the URI of the attribute or StartElement
	''' QName no prefix will be assigned.</p>
	''' 
	''' <p>If an element or attribute name has a prefix, but is not
	''' bound to any namespace URI, then the prefix will be removed
	''' during serialization.</p>
	''' 
	''' <p>If element and/or attribute names in the same start or
	''' empty-element tag are bound to different namespace URIs and
	''' are using the same prefix then the element or the first
	''' occurring attribute retains the original prefix and the
	''' following attributes have their prefixes replaced with a
	''' new prefix that is bound to the namespace URIs of those
	''' attributes. </p>
	''' 
	''' <p>If an element or attribute name uses a prefix that is
	''' bound to a different URI than that inherited from the
	''' namespace context of the parent of that element and there
	''' is no namespace declaration in the context of the current
	''' element then such a namespace declaration is added. </p>
	''' 
	''' <p>If an element or attribute name is bound to a prefix and
	''' there is a namespace declaration that binds that prefix
	''' to a different URI then that namespace declaration is
	''' either removed if the correct mapping is inherited from
	''' the parent context of that element, or changed to the
	''' namespace URI of the element or attribute using that prefix.</p>
	''' 
	''' @version 1.2
	''' @author Copyright (c) 2009 by Oracle Corporation. All Rights Reserved. </summary>
	''' <seealso cref= XMLInputFactory </seealso>
	''' <seealso cref= XMLEventWriter </seealso>
	''' <seealso cref= XMLStreamWriter
	''' @since 1.6 </seealso>
	Public MustInherit Class XMLOutputFactory
	  ''' <summary>
	  ''' Property used to set prefix defaulting on the output side
	  ''' </summary>
	  Public Const IS_REPAIRING_NAMESPACES As String= "javax.xml.stream.isRepairingNamespaces"

	  Friend Const DEFAULIMPL As String = "com.sun.xml.internal.stream.XMLOutputFactoryImpl"

	  Protected Friend Sub New()
	  End Sub

	  ''' <summary>
	  ''' Creates a new instance of the factory in exactly the same manner as the
	  ''' <seealso cref="#newFactory()"/> method. </summary>
	  ''' <exception cref="FactoryConfigurationError"> if an instance of this factory cannot be loaded </exception>
	  Public Shared Function newInstance() As XMLOutputFactory
		Return FactoryFinder.find(GetType(XMLOutputFactory), DEFAULIMPL)
	  End Function

	  ''' <summary>
	  ''' Create a new instance of the factory.
	  ''' <p>
	  ''' This static method creates a new factory instance. This method uses the
	  ''' following ordered lookup procedure to determine the XMLOutputFactory
	  ''' implementation class to load:
	  ''' </p>
	  ''' <ul>
	  ''' <li>
	  '''   Use the javax.xml.stream.XMLOutputFactory system property.
	  ''' </li>
	  ''' <li>
	  '''   Use the properties file "lib/stax.properties" in the JRE directory.
	  '''     This configuration file is in standard java.util.Properties format
	  '''     and contains the fully qualified name of the implementation class
	  '''     with the key being the system property defined above.
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
	  ''' <p>
	  ''' Once an application has obtained a reference to a XMLOutputFactory it
	  ''' can use the factory to configure and obtain stream instances.
	  ''' </p>
	  ''' <p>
	  ''' Note that this is a new method that replaces the deprecated newInstance() method.
	  '''   No changes in behavior are defined by this replacement method relative to the
	  '''   deprecated method.
	  ''' </p> </summary>
	  ''' <exception cref="FactoryConfigurationError"> in case of {@linkplain
	  '''   java.util.ServiceConfigurationError service configuration error} or if
	  '''   the implementation is not available or cannot be instantiated. </exception>
	  Public Shared Function newFactory() As XMLOutputFactory
		Return FactoryFinder.find(GetType(XMLOutputFactory), DEFAULIMPL)
	  End Function

	  ''' <summary>
	  ''' Create a new instance of the factory.
	  ''' </summary>
	  ''' <param name="factoryId">             Name of the factory to find, same as
	  '''                              a property name </param>
	  ''' <param name="classLoader">           classLoader to use </param>
	  ''' <returns> the factory implementation </returns>
	  ''' <exception cref="FactoryConfigurationError"> if an instance of this factory cannot be loaded
	  ''' </exception>
	  ''' @deprecated  This method has been deprecated because it returns an
	  '''              instance of XMLInputFactory, which is of the wrong class.
	  '''              Use the new method {@link #newFactory(java.lang.String,
	  '''              java.lang.ClassLoader)} instead. 
	  Public Shared Function newInstance(ByVal factoryId As String, ByVal classLoader As ClassLoader) As XMLInputFactory
		  'do not fallback if given classloader can't find the class, throw exception
		  Return FactoryFinder.find(GetType(XMLInputFactory), factoryId, classLoader, Nothing)
	  End Function

	  ''' <summary>
	  ''' Create a new instance of the factory.
	  ''' If the classLoader argument is null, then the ContextClassLoader is used.
	  ''' <p>
	  ''' This method uses the following ordered lookup procedure to determine
	  ''' the XMLOutputFactory implementation class to load:
	  ''' </p>
	  ''' <ul>
	  ''' <li>
	  '''   Use the value of the system property identified by {@code factoryId}.
	  ''' </li>
	  ''' <li>
	  '''   Use the properties file "lib/stax.properties" in the JRE directory.
	  '''     This configuration file is in standard java.util.Properties format
	  '''     and contains the fully qualified name of the implementation class
	  '''     with the key being the given {@code factoryId}.
	  ''' </li>
	  ''' <li>
	  '''   If {@code factoryId} is "javax.xml.stream.XMLOutputFactory",
	  '''   use the service-provider loading facilities, defined by the
	  '''   <seealso cref="java.util.ServiceLoader"/> class, to attempt to {@linkplain
	  '''   java.util.ServiceLoader#load(java.lang.Class, java.lang.ClassLoader) locate and load}
	  '''   an implementation of the service using the specified {@code ClassLoader}.
	  '''   If {@code classLoader} is null, the {@linkplain
	  '''   java.util.ServiceLoader#load(java.lang.Class) default loading mechanism} will apply:
	  '''   That is, the service-provider loading facility will use the {@linkplain
	  '''   java.lang.Thread#getContextClassLoader() current thread's context class loader}
	  '''   to attempt to load the service. If the context class
	  '''   loader is null, the {@linkplain
	  '''   ClassLoader#getSystemClassLoader() system class loader} will be used.
	  ''' </li>
	  ''' <li>
	  '''   Otherwise, throws a <seealso cref="FactoryConfigurationError"/>.
	  ''' </li>
	  ''' </ul>
	  ''' 
	  ''' @apiNote The parameter factoryId defined here is inconsistent with that
	  ''' of other JAXP factories where the first parameter is fully qualified
	  ''' factory class name that provides implementation of the factory.
	  ''' 
	  ''' <p>
	  '''   Note that this is a new method that replaces the deprecated
	  '''   {@link #newInstance(java.lang.String, java.lang.ClassLoader)
	  '''   newInstance(String factoryId, ClassLoader classLoader)} method.
	  '''   The original method was incorrectly defined to return XMLInputFactory.
	  ''' </p>
	  ''' </summary>
	  ''' <param name="factoryId">             Name of the factory to find, same as
	  '''                              a property name </param>
	  ''' <param name="classLoader">           classLoader to use </param>
	  ''' <returns> the factory implementation </returns>
	  ''' <exception cref="FactoryConfigurationError"> in case of {@linkplain
	  '''   java.util.ServiceConfigurationError service configuration error} or if
	  '''   the implementation is not available or cannot be instantiated. </exception>
	  Public Shared Function newFactory(ByVal factoryId As String, ByVal classLoader As ClassLoader) As XMLOutputFactory
		  'do not fallback if given classloader can't find the class, throw exception
		  Return FactoryFinder.find(GetType(XMLOutputFactory), factoryId, classLoader, Nothing)
	  End Function

	  ''' <summary>
	  ''' Create a new XMLStreamWriter that writes to a writer </summary>
	  ''' <param name="stream"> the writer to write to </param>
	  ''' <exception cref="XMLStreamException"> </exception>
	  Public MustOverride Function createXMLStreamWriter(ByVal stream As java.io.Writer) As XMLStreamWriter

	  ''' <summary>
	  ''' Create a new XMLStreamWriter that writes to a stream </summary>
	  ''' <param name="stream"> the stream to write to </param>
	  ''' <exception cref="XMLStreamException"> </exception>
	  Public MustOverride Function createXMLStreamWriter(ByVal stream As java.io.OutputStream) As XMLStreamWriter

	  ''' <summary>
	  ''' Create a new XMLStreamWriter that writes to a stream </summary>
	  ''' <param name="stream"> the stream to write to </param>
	  ''' <param name="encoding"> the encoding to use </param>
	  ''' <exception cref="XMLStreamException"> </exception>
	  Public MustOverride Function createXMLStreamWriter(ByVal stream As java.io.OutputStream, ByVal encoding As String) As XMLStreamWriter

	  ''' <summary>
	  ''' Create a new XMLStreamWriter that writes to a JAXP result.  This method is optional. </summary>
	  ''' <param name="result"> the result to write to </param>
	  ''' <exception cref="UnsupportedOperationException"> if this method is not
	  ''' supported by this XMLOutputFactory </exception>
	  ''' <exception cref="XMLStreamException"> </exception>
	  Public MustOverride Function createXMLStreamWriter(ByVal result As javax.xml.transform.Result) As XMLStreamWriter


	  ''' <summary>
	  ''' Create a new XMLEventWriter that writes to a JAXP result.  This method is optional. </summary>
	  ''' <param name="result"> the result to write to </param>
	  ''' <exception cref="UnsupportedOperationException"> if this method is not
	  ''' supported by this XMLOutputFactory </exception>
	  ''' <exception cref="XMLStreamException"> </exception>
	  Public MustOverride Function createXMLEventWriter(ByVal result As javax.xml.transform.Result) As XMLEventWriter

	  ''' <summary>
	  ''' Create a new XMLEventWriter that writes to a stream </summary>
	  ''' <param name="stream"> the stream to write to </param>
	  ''' <exception cref="XMLStreamException"> </exception>
	  Public MustOverride Function createXMLEventWriter(ByVal stream As java.io.OutputStream) As XMLEventWriter



	  ''' <summary>
	  ''' Create a new XMLEventWriter that writes to a stream </summary>
	  ''' <param name="stream"> the stream to write to </param>
	  ''' <param name="encoding"> the encoding to use </param>
	  ''' <exception cref="XMLStreamException"> </exception>
	  Public MustOverride Function createXMLEventWriter(ByVal stream As java.io.OutputStream, ByVal encoding As String) As XMLEventWriter

	  ''' <summary>
	  ''' Create a new XMLEventWriter that writes to a writer </summary>
	  ''' <param name="stream"> the stream to write to </param>
	  ''' <exception cref="XMLStreamException"> </exception>
	  Public MustOverride Function createXMLEventWriter(ByVal stream As java.io.Writer) As XMLEventWriter

	  ''' <summary>
	  ''' Allows the user to set specific features/properties on the underlying implementation. </summary>
	  ''' <param name="name"> The name of the property </param>
	  ''' <param name="value"> The value of the property </param>
	  ''' <exception cref="java.lang.IllegalArgumentException"> if the property is not supported </exception>
	  Public MustOverride Sub setProperty(ByVal name As String, ByVal value As Object)

	  ''' <summary>
	  ''' Get a feature/property on the underlying implementation </summary>
	  ''' <param name="name"> The name of the property </param>
	  ''' <returns> The value of the property </returns>
	  ''' <exception cref="java.lang.IllegalArgumentException"> if the property is not supported </exception>
	  Public MustOverride Function getProperty(ByVal name As String) As Object

	  ''' <summary>
	  ''' Query the set of properties that this factory supports.
	  ''' </summary>
	  ''' <param name="name"> The name of the property (may not be null) </param>
	  ''' <returns> true if the property is supported and false otherwise </returns>
	  Public MustOverride Function isPropertySupported(ByVal name As String) As Boolean
	End Class

End Namespace