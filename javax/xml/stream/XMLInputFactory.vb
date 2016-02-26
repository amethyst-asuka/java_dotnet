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
	''' Defines an abstract implementation of a factory for getting streams.
	''' 
	''' The following table defines the standard properties of this specification.
	''' Each property varies in the level of support required by each implementation.
	''' The level of support required is described in the 'Required' column.
	''' 
	'''   <table border="2" rules="all" cellpadding="4">
	'''    <thead>
	'''      <tr>
	'''        <th align="center" colspan="5">
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
	'''      </tr>
	''' <tr><td>javax.xml.stream.isValidating</td><td>Turns on/off implementation specific DTD validation</td><td>Boolean</td><td>False</td><td>No</td></tr>
	''' <tr><td>javax.xml.stream.isNamespaceAware</td><td>Turns on/off namespace processing for XML 1.0 support</td><td>Boolean</td><td>True</td><td>True (required) / False (optional)</td></tr>
	''' <tr><td>javax.xml.stream.isCoalescing</td><td>Requires the processor to coalesce adjacent character data</td><td>Boolean</td><td>False</td><td>Yes</td></tr>
	''' <tr><td>javax.xml.stream.isReplacingEntityReferences</td><td>replace internal entity references with their replacement text and report them as characters</td><td>Boolean</td><td>True</td><td>Yes</td></tr>
	''' <tr><td>javax.xml.stream.isSupportingExternalEntities</td><td>Resolve external parsed entities</td><td>Boolean</td><td>Unspecified</td><td>Yes</td></tr>
	''' <tr><td>javax.xml.stream.supportDTD</td><td>Use this property to request processors that do not support DTDs</td><td>Boolean</td><td>True</td><td>Yes</td></tr>
	''' <tr><td>javax.xml.stream.reporter</td><td>sets/gets the impl of the XMLReporter </td><td>javax.xml.stream.XMLReporter</td><td>Null</td><td>Yes</td></tr>
	''' <tr><td>javax.xml.stream.resolver</td><td>sets/gets the impl of the XMLResolver interface</td><td>javax.xml.stream.XMLResolver</td><td>Null</td><td>Yes</td></tr>
	''' <tr><td>javax.xml.stream.allocator</td><td>sets/gets the impl of the XMLEventAllocator interface</td><td>javax.xml.stream.util.XMLEventAllocator</td><td>Null</td><td>Yes</td></tr>
	'''    </tbody>
	'''  </table>
	''' 
	''' 
	''' @version 1.2
	''' @author Copyright (c) 2009 by Oracle Corporation. All Rights Reserved. </summary>
	''' <seealso cref= XMLOutputFactory </seealso>
	''' <seealso cref= XMLEventReader </seealso>
	''' <seealso cref= XMLStreamReader </seealso>
	''' <seealso cref= EventFilter </seealso>
	''' <seealso cref= XMLReporter </seealso>
	''' <seealso cref= XMLResolver </seealso>
	''' <seealso cref= javax.xml.stream.util.XMLEventAllocator
	''' @since 1.6 </seealso>

	Public MustInherit Class XMLInputFactory
	  ''' <summary>
	  ''' The property used to turn on/off namespace support,
	  ''' this is to support XML 1.0 documents,
	  ''' only the true setting must be supported
	  ''' </summary>
	  Public Const IS_NAMESPACE_AWARE As String= "javax.xml.stream.isNamespaceAware"

	  ''' <summary>
	  ''' The property used to turn on/off implementation specific validation
	  ''' </summary>
	  Public Const IS_VALIDATING As String= "javax.xml.stream.isValidating"

	  ''' <summary>
	  ''' The property that requires the parser to coalesce adjacent character data sections
	  ''' </summary>
	  Public Const IS_COALESCING As String= "javax.xml.stream.isCoalescing"

	  ''' <summary>
	  ''' Requires the parser to replace internal
	  ''' entity references with their replacement
	  ''' text and report them as characters
	  ''' </summary>
	  Public Const IS_REPLACING_ENTITY_REFERENCES As String= "javax.xml.stream.isReplacingEntityReferences"

	  ''' <summary>
	  '''  The property that requires the parser to resolve external parsed entities
	  ''' </summary>
	  Public Const IS_SUPPORTING_EXTERNAL_ENTITIES As String= "javax.xml.stream.isSupportingExternalEntities"

	  ''' <summary>
	  '''  The property that requires the parser to support DTDs
	  ''' </summary>
	  Public Const SUPPORT_DTD As String= "javax.xml.stream.supportDTD"

	  ''' <summary>
	  ''' The property used to
	  ''' set/get the implementation of the XMLReporter interface
	  ''' </summary>
	  Public Const REPORTER As String= "javax.xml.stream.reporter"

	  ''' <summary>
	  ''' The property used to set/get the implementation of the XMLResolver
	  ''' </summary>
	  Public Const RESOLVER As String= "javax.xml.stream.resolver"

	  ''' <summary>
	  ''' The property used to set/get the implementation of the allocator
	  ''' </summary>
	  Public Const ALLOCATOR As String= "javax.xml.stream.allocator"

	  Friend Const DEFAULIMPL As String = "com.sun.xml.internal.stream.XMLInputFactoryImpl"

	  Protected Friend Sub New()
	  End Sub

	  ''' <summary>
	  ''' Creates a new instance of the factory in exactly the same manner as the
	  ''' <seealso cref="#newFactory()"/> method. </summary>
	  ''' <exception cref="FactoryConfigurationError"> if an instance of this factory cannot be loaded </exception>
	  Public Shared Function newInstance() As XMLInputFactory
		Return FactoryFinder.find(GetType(XMLInputFactory), DEFAULIMPL)
	  End Function

	  ''' <summary>
	  ''' Create a new instance of the factory.
	  ''' <p>
	  ''' This static method creates a new factory instance.
	  ''' This method uses the following ordered lookup procedure to determine
	  ''' the XMLInputFactory implementation class to load:
	  ''' </p>
	  ''' <ul>
	  ''' <li>
	  '''   Use the javax.xml.stream.XMLInputFactory system property.
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
	  ''' Otherwise, the system-default implementation is returned.
	  ''' </li>
	  ''' </ul>
	  ''' <p>
	  '''   Once an application has obtained a reference to a XMLInputFactory it
	  '''   can use the factory to configure and obtain stream instances.
	  ''' </p>
	  ''' <p>
	  '''   Note that this is a new method that replaces the deprecated newInstance() method.
	  '''     No changes in behavior are defined by this replacement method relative to
	  '''     the deprecated method.
	  ''' </p> </summary>
	  ''' <exception cref="FactoryConfigurationError"> in case of {@linkplain
	  '''   java.util.ServiceConfigurationError service configuration error} or if
	  '''   the implementation is not available or cannot be instantiated. </exception>
	  Public Shared Function newFactory() As XMLInputFactory
		Return FactoryFinder.find(GetType(XMLInputFactory), DEFAULIMPL)
	  End Function

	  ''' <summary>
	  ''' Create a new instance of the factory
	  ''' </summary>
	  ''' <param name="factoryId">             Name of the factory to find, same as
	  '''                              a property name </param>
	  ''' <param name="classLoader">           classLoader to use </param>
	  ''' <returns> the factory implementation </returns>
	  ''' <exception cref="FactoryConfigurationError"> if an instance of this factory cannot be loaded
	  ''' </exception>
	  ''' @deprecated  This method has been deprecated to maintain API consistency.
	  '''              All newInstance methods have been replaced with corresponding
	  '''              newFactory methods. The replacement {@link
	  '''              #newFactory(java.lang.String, java.lang.ClassLoader)} method
	  '''              defines no changes in behavior. 
	  Public Shared Function newInstance(ByVal factoryId As String, ByVal classLoader As ClassLoader) As XMLInputFactory
		  'do not fallback if given classloader can't find the class, throw exception
		  Return FactoryFinder.find(GetType(XMLInputFactory), factoryId, classLoader, Nothing)
	  End Function

	  ''' <summary>
	  ''' Create a new instance of the factory.
	  ''' If the classLoader argument is null, then the ContextClassLoader is used.
	  ''' <p>
	  ''' This method uses the following ordered lookup procedure to determine
	  ''' the XMLInputFactory implementation class to load:
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
	  '''   If {@code factoryId} is "javax.xml.stream.XMLInputFactory",
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
	  ''' <p>
	  ''' Note that this is a new method that replaces the deprecated
	  '''   {@link #newInstance(java.lang.String, java.lang.ClassLoader)
	  '''   newInstance(String factoryId, ClassLoader classLoader)} method.
	  ''' No changes in behavior are defined by this replacement method relative
	  ''' to the deprecated method.
	  ''' </p>
	  ''' 
	  ''' @apiNote The parameter factoryId defined here is inconsistent with that
	  ''' of other JAXP factories where the first parameter is fully qualified
	  ''' factory class name that provides implementation of the factory.
	  ''' </summary>
	  ''' <param name="factoryId">             Name of the factory to find, same as
	  '''                              a property name </param>
	  ''' <param name="classLoader">           classLoader to use </param>
	  ''' <returns> the factory implementation </returns>
	  ''' <exception cref="FactoryConfigurationError"> in case of {@linkplain
	  '''   java.util.ServiceConfigurationError service configuration error} or if
	  '''   the implementation is not available or cannot be instantiated. </exception>
	  ''' <exception cref="FactoryConfigurationError"> if an instance of this factory cannot be loaded </exception>
	  Public Shared Function newFactory(ByVal factoryId As String, ByVal classLoader As ClassLoader) As XMLInputFactory
		  'do not fallback if given classloader can't find the class, throw exception
		  Return FactoryFinder.find(GetType(XMLInputFactory), factoryId, classLoader, Nothing)
	  End Function

	  ''' <summary>
	  ''' Create a new XMLStreamReader from a reader </summary>
	  ''' <param name="reader"> the XML data to read from </param>
	  ''' <exception cref="XMLStreamException"> </exception>
	  Public MustOverride Function createXMLStreamReader(ByVal reader As java.io.Reader) As XMLStreamReader

	  ''' <summary>
	  ''' Create a new XMLStreamReader from a JAXP source.  This method is optional. </summary>
	  ''' <param name="source"> the source to read from </param>
	  ''' <exception cref="UnsupportedOperationException"> if this method is not
	  ''' supported by this XMLInputFactory </exception>
	  ''' <exception cref="XMLStreamException"> </exception>
	  Public MustOverride Function createXMLStreamReader(ByVal source As javax.xml.transform.Source) As XMLStreamReader

	  ''' <summary>
	  ''' Create a new XMLStreamReader from a java.io.InputStream </summary>
	  ''' <param name="stream"> the InputStream to read from </param>
	  ''' <exception cref="XMLStreamException"> </exception>
	  Public MustOverride Function createXMLStreamReader(ByVal stream As java.io.InputStream) As XMLStreamReader

	  ''' <summary>
	  ''' Create a new XMLStreamReader from a java.io.InputStream </summary>
	  ''' <param name="stream"> the InputStream to read from </param>
	  ''' <param name="encoding"> the character encoding of the stream </param>
	  ''' <exception cref="XMLStreamException"> </exception>
	  Public MustOverride Function createXMLStreamReader(ByVal stream As java.io.InputStream, ByVal encoding As String) As XMLStreamReader

	  ''' <summary>
	  ''' Create a new XMLStreamReader from a java.io.InputStream </summary>
	  ''' <param name="systemId"> the system ID of the stream </param>
	  ''' <param name="stream"> the InputStream to read from </param>
	  Public MustOverride Function createXMLStreamReader(ByVal systemId As String, ByVal stream As java.io.InputStream) As XMLStreamReader

	  ''' <summary>
	  ''' Create a new XMLStreamReader from a java.io.InputStream </summary>
	  ''' <param name="systemId"> the system ID of the stream </param>
	  ''' <param name="reader"> the InputStream to read from </param>
	  Public MustOverride Function createXMLStreamReader(ByVal systemId As String, ByVal reader As java.io.Reader) As XMLStreamReader

	  ''' <summary>
	  ''' Create a new XMLEventReader from a reader </summary>
	  ''' <param name="reader"> the XML data to read from </param>
	  ''' <exception cref="XMLStreamException"> </exception>
	  Public MustOverride Function createXMLEventReader(ByVal reader As java.io.Reader) As XMLEventReader

	  ''' <summary>
	  ''' Create a new XMLEventReader from a reader </summary>
	  ''' <param name="systemId"> the system ID of the input </param>
	  ''' <param name="reader"> the XML data to read from </param>
	  ''' <exception cref="XMLStreamException"> </exception>
	  Public MustOverride Function createXMLEventReader(ByVal systemId As String, ByVal reader As java.io.Reader) As XMLEventReader

	  ''' <summary>
	  ''' Create a new XMLEventReader from an XMLStreamReader.  After being used
	  ''' to construct the XMLEventReader instance returned from this method
	  ''' the XMLStreamReader must not be used. </summary>
	  ''' <param name="reader"> the XMLStreamReader to read from (may not be modified) </param>
	  ''' <returns> a new XMLEventReader </returns>
	  ''' <exception cref="XMLStreamException"> </exception>
	  Public MustOverride Function createXMLEventReader(ByVal reader As XMLStreamReader) As XMLEventReader

	  ''' <summary>
	  ''' Create a new XMLEventReader from a JAXP source.
	  ''' Support of this method is optional. </summary>
	  ''' <param name="source"> the source to read from </param>
	  ''' <exception cref="UnsupportedOperationException"> if this method is not
	  ''' supported by this XMLInputFactory </exception>
	  Public MustOverride Function createXMLEventReader(ByVal source As javax.xml.transform.Source) As XMLEventReader

	  ''' <summary>
	  ''' Create a new XMLEventReader from a java.io.InputStream </summary>
	  ''' <param name="stream"> the InputStream to read from </param>
	  ''' <exception cref="XMLStreamException"> </exception>
	  Public MustOverride Function createXMLEventReader(ByVal stream As java.io.InputStream) As XMLEventReader

	  ''' <summary>
	  ''' Create a new XMLEventReader from a java.io.InputStream </summary>
	  ''' <param name="stream"> the InputStream to read from </param>
	  ''' <param name="encoding"> the character encoding of the stream </param>
	  ''' <exception cref="XMLStreamException"> </exception>
	  Public MustOverride Function createXMLEventReader(ByVal stream As java.io.InputStream, ByVal encoding As String) As XMLEventReader

	  ''' <summary>
	  ''' Create a new XMLEventReader from a java.io.InputStream </summary>
	  ''' <param name="systemId"> the system ID of the stream </param>
	  ''' <param name="stream"> the InputStream to read from </param>
	  ''' <exception cref="XMLStreamException"> </exception>
	  Public MustOverride Function createXMLEventReader(ByVal systemId As String, ByVal stream As java.io.InputStream) As XMLEventReader

	  ''' <summary>
	  ''' Create a filtered reader that wraps the filter around the reader </summary>
	  ''' <param name="reader"> the reader to filter </param>
	  ''' <param name="filter"> the filter to apply to the reader </param>
	  ''' <exception cref="XMLStreamException"> </exception>
	  Public MustOverride Function createFilteredReader(ByVal reader As XMLStreamReader, ByVal filter As StreamFilter) As XMLStreamReader

	  ''' <summary>
	  ''' Create a filtered event reader that wraps the filter around the event reader </summary>
	  ''' <param name="reader"> the event reader to wrap </param>
	  ''' <param name="filter"> the filter to apply to the event reader </param>
	  ''' <exception cref="XMLStreamException"> </exception>
	  Public MustOverride Function createFilteredReader(ByVal reader As XMLEventReader, ByVal filter As EventFilter) As XMLEventReader

	  ''' <summary>
	  ''' The resolver that will be set on any XMLStreamReader or XMLEventReader created
	  ''' by this factory instance.
	  ''' </summary>
	  Public MustOverride Property xMLResolver As XMLResolver


	  ''' <summary>
	  ''' The reporter that will be set on any XMLStreamReader or XMLEventReader created
	  ''' by this factory instance.
	  ''' </summary>
	  Public MustOverride Property xMLReporter As XMLReporter


	  ''' <summary>
	  ''' Allows the user to set specific feature/property on the underlying
	  ''' implementation. The underlying implementation is not required to support
	  ''' every setting of every property in the specification and may use
	  ''' IllegalArgumentException to signal that an unsupported property may not be
	  ''' set with the specified value.
	  ''' <p>
	  ''' All implementations that implement JAXP 1.5 or newer are required to
	  ''' support the <seealso cref="javax.xml.XMLConstants#ACCESS_EXTERNAL_DTD"/> property.
	  ''' </p>
	  ''' <ul>
	  '''   <li>
	  '''        <p>
	  '''        Access to external DTDs, external Entity References is restricted to the
	  '''        protocols specified by the property. If access is denied during parsing
	  '''        due to the restriction of this property, <seealso cref="javax.xml.stream.XMLStreamException"/>
	  '''        will be thrown by the <seealso cref="javax.xml.stream.XMLStreamReader#next()"/> or
	  '''        <seealso cref="javax.xml.stream.XMLEventReader#nextEvent()"/> method.
	  '''        </p>
	  '''   </li>
	  ''' </ul> </summary>
	  ''' <param name="name"> The name of the property (may not be null) </param>
	  ''' <param name="value"> The value of the property </param>
	  ''' <exception cref="java.lang.IllegalArgumentException"> if the property is not supported </exception>
	  Public MustOverride Sub setProperty(ByVal name As String, ByVal value As Object)

	  ''' <summary>
	  ''' Get the value of a feature/property from the underlying implementation </summary>
	  ''' <param name="name"> The name of the property (may not be null) </param>
	  ''' <returns> The value of the property </returns>
	  ''' <exception cref="IllegalArgumentException"> if the property is not supported </exception>
	  Public MustOverride Function getProperty(ByVal name As String) As Object


	  ''' <summary>
	  ''' Query the set of properties that this factory supports.
	  ''' </summary>
	  ''' <param name="name"> The name of the property (may not be null) </param>
	  ''' <returns> true if the property is supported and false otherwise </returns>
	  Public MustOverride Function isPropertySupported(ByVal name As String) As Boolean

	  ''' <summary>
	  ''' Set a user defined event allocator for events </summary>
	  ''' <param name="allocator"> the user defined allocator </param>
	  Public MustOverride Property eventAllocator As javax.xml.stream.util.XMLEventAllocator


	End Class

End Namespace