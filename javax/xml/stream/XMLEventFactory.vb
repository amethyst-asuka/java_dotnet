Imports System.Collections
Imports javax.xml.stream.events

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
	''' This interface defines a utility class for creating instances of
	''' XMLEvents
	''' @version 1.2
	''' @author Copyright (c) 2009 by Oracle Corporation. All Rights Reserved. </summary>
	''' <seealso cref= javax.xml.stream.events.StartElement </seealso>
	''' <seealso cref= javax.xml.stream.events.EndElement </seealso>
	''' <seealso cref= javax.xml.stream.events.ProcessingInstruction </seealso>
	''' <seealso cref= javax.xml.stream.events.Comment </seealso>
	''' <seealso cref= javax.xml.stream.events.Characters </seealso>
	''' <seealso cref= javax.xml.stream.events.StartDocument </seealso>
	''' <seealso cref= javax.xml.stream.events.EndDocument </seealso>
	''' <seealso cref= javax.xml.stream.events.DTD
	''' @since 1.6 </seealso>
	Public MustInherit Class XMLEventFactory
	  Protected Friend Sub New()
	  End Sub

		Friend Const JAXPFACTORYID As String = "javax.xml.stream.XMLEventFactory"
		Friend Const DEFAULIMPL As String = "com.sun.xml.internal.stream.events.XMLEventFactoryImpl"


	  ''' <summary>
	  ''' Creates a new instance of the factory in exactly the same manner as the
	  ''' <seealso cref="#newFactory()"/> method. </summary>
	  ''' <exception cref="FactoryConfigurationError"> if an instance of this factory cannot be loaded </exception>
	  Public Shared Function newInstance() As XMLEventFactory
		Return FactoryFinder.find(GetType(XMLEventFactory), DEFAULIMPL)
	  End Function

	  ''' <summary>
	  ''' Create a new instance of the factory.
	  ''' <p>
	  ''' This static method creates a new factory instance.
	  ''' This method uses the following ordered lookup procedure to determine
	  ''' the XMLEventFactory implementation class to load:
	  ''' </p>
	  ''' <ul>
	  ''' <li>
	  '''   Use the javax.xml.stream.XMLEventFactory system property.
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
	  ''' </ul>
	  ''' <p>
	  '''   Once an application has obtained a reference to a XMLEventFactory it
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
	  Public Shared Function newFactory() As XMLEventFactory
		Return FactoryFinder.find(GetType(XMLEventFactory), DEFAULIMPL)
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
	  '''              #newFactory(java.lang.String, java.lang.ClassLoader)}
	  '''              method defines no changes in behavior. 
	  Public Shared Function newInstance(ByVal factoryId As String, ByVal classLoader As ClassLoader) As XMLEventFactory
		  'do not fallback if given classloader can't find the class, throw exception
		  Return FactoryFinder.find(GetType(XMLEventFactory), factoryId, classLoader, Nothing)
	  End Function

	  ''' <summary>
	  ''' Create a new instance of the factory.
	  ''' If the classLoader argument is null, then the ContextClassLoader is used.
	  ''' <p>
	  ''' This method uses the following ordered lookup procedure to determine
	  ''' the XMLEventFactory implementation class to load:
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
	  '''   If {@code factoryId} is "javax.xml.stream.XMLEventFactory",
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
	  Public Shared Function newFactory(ByVal factoryId As String, ByVal classLoader As ClassLoader) As XMLEventFactory
		  'do not fallback if given classloader can't find the class, throw exception
		  Return FactoryFinder.find(GetType(XMLEventFactory), factoryId, classLoader, Nothing)
	  End Function

	 ''' <summary>
	 ''' This method allows setting of the Location on each event that
	 ''' is created by this factory.  The values are copied by value into
	 ''' the events created by this factory.  To reset the location
	 ''' information set the location to null. </summary>
	 ''' <param name="location"> the location to set on each event created </param>
	  Public MustOverride WriteOnly Property location As Location

	  ''' <summary>
	  ''' Create a new Attribute </summary>
	  ''' <param name="prefix"> the prefix of this attribute, may not be null </param>
	  ''' <param name="namespaceURI"> the attribute value is set to this value, may not be null </param>
	  ''' <param name="localName"> the local name of the XML name of the attribute, localName cannot be null </param>
	  ''' <param name="value"> the attribute value to set, may not be null </param>
	  ''' <returns> the Attribute with specified values </returns>
	  Public MustOverride Function createAttribute(ByVal prefix As String, ByVal namespaceURI As String, ByVal localName As String, ByVal value As String) As Attribute

	  ''' <summary>
	  ''' Create a new Attribute </summary>
	  ''' <param name="localName"> the local name of the XML name of the attribute, localName cannot be null </param>
	  ''' <param name="value"> the attribute value to set, may not be null </param>
	  ''' <returns> the Attribute with specified values </returns>
	  Public MustOverride Function createAttribute(ByVal localName As String, ByVal value As String) As Attribute

	  ''' <summary>
	  ''' Create a new Attribute </summary>
	  ''' <param name="name"> the qualified name of the attribute, may not be null </param>
	  ''' <param name="value"> the attribute value to set, may not be null </param>
	  ''' <returns> the Attribute with specified values </returns>
	  Public MustOverride Function createAttribute(ByVal name As javax.xml.namespace.QName, ByVal value As String) As Attribute

	  ''' <summary>
	  ''' Create a new default Namespace </summary>
	  ''' <param name="namespaceURI"> the default namespace uri </param>
	  ''' <returns> the Namespace with the specified value </returns>
	  Public MustOverride Function createNamespace(ByVal namespaceURI As String) As [Namespace]

	  ''' <summary>
	  ''' Create a new Namespace </summary>
	  ''' <param name="prefix"> the prefix of this namespace, may not be null </param>
	  ''' <param name="namespaceUri"> the attribute value is set to this value, may not be null </param>
	  ''' <returns> the Namespace with the specified values </returns>
	  Public MustOverride Function createNamespace(ByVal prefix As String, ByVal namespaceUri As String) As [Namespace]

	  ''' <summary>
	  ''' Create a new StartElement.  Namespaces can be added to this StartElement
	  ''' by passing in an Iterator that walks over a set of Namespace interfaces.
	  ''' Attributes can be added to this StartElement by passing an iterator
	  ''' that walks over a set of Attribute interfaces.
	  ''' </summary>
	  ''' <param name="name"> the qualified name of the attribute, may not be null </param>
	  ''' <param name="attributes"> an optional unordered set of objects that
	  ''' implement Attribute to add to the new StartElement, may be null </param>
	  ''' <param name="namespaces"> an optional unordered set of objects that
	  ''' implement Namespace to add to the new StartElement, may be null </param>
	  ''' <returns> an instance of the requested StartElement </returns>
	  Public MustOverride Function createStartElement(ByVal name As javax.xml.namespace.QName, ByVal attributes As IEnumerator, ByVal namespaces As IEnumerator) As StartElement

	  ''' <summary>
	  ''' Create a new StartElement.  This defaults the NamespaceContext to
	  ''' an empty NamespaceContext.  Querying this event for its namespaces or
	  ''' attributes will result in an empty iterator being returned.
	  ''' </summary>
	  ''' <param name="namespaceUri"> the uri of the QName of the new StartElement </param>
	  ''' <param name="localName"> the local name of the QName of the new StartElement </param>
	  ''' <param name="prefix"> the prefix of the QName of the new StartElement </param>
	  ''' <returns> an instance of the requested StartElement </returns>
	  Public MustOverride Function createStartElement(ByVal prefix As String, ByVal namespaceUri As String, ByVal localName As String) As StartElement
	  ''' <summary>
	  ''' Create a new StartElement.  Namespaces can be added to this StartElement
	  ''' by passing in an Iterator that walks over a set of Namespace interfaces.
	  ''' Attributes can be added to this StartElement by passing an iterator
	  ''' that walks over a set of Attribute interfaces.
	  ''' </summary>
	  ''' <param name="namespaceUri"> the uri of the QName of the new StartElement </param>
	  ''' <param name="localName"> the local name of the QName of the new StartElement </param>
	  ''' <param name="prefix"> the prefix of the QName of the new StartElement </param>
	  ''' <param name="attributes"> an unordered set of objects that implement
	  ''' Attribute to add to the new StartElement </param>
	  ''' <param name="namespaces"> an unordered set of objects that implement
	  ''' Namespace to add to the new StartElement </param>
	  ''' <returns> an instance of the requested StartElement </returns>
	  Public MustOverride Function createStartElement(ByVal prefix As String, ByVal namespaceUri As String, ByVal localName As String, ByVal attributes As IEnumerator, ByVal namespaces As IEnumerator) As StartElement
	  ''' <summary>
	  ''' Create a new StartElement.  Namespaces can be added to this StartElement
	  ''' by passing in an Iterator that walks over a set of Namespace interfaces.
	  ''' Attributes can be added to this StartElement by passing an iterator
	  ''' that walks over a set of Attribute interfaces.
	  ''' </summary>
	  ''' <param name="namespaceUri"> the uri of the QName of the new StartElement </param>
	  ''' <param name="localName"> the local name of the QName of the new StartElement </param>
	  ''' <param name="prefix"> the prefix of the QName of the new StartElement </param>
	  ''' <param name="attributes"> an unordered set of objects that implement
	  ''' Attribute to add to the new StartElement, may be null </param>
	  ''' <param name="namespaces"> an unordered set of objects that implement
	  ''' Namespace to add to the new StartElement, may be null </param>
	  ''' <param name="context"> the namespace context of this element </param>
	  ''' <returns> an instance of the requested StartElement </returns>
	  Public MustOverride Function createStartElement(ByVal prefix As String, ByVal namespaceUri As String, ByVal localName As String, ByVal attributes As IEnumerator, ByVal namespaces As IEnumerator, ByVal context As javax.xml.namespace.NamespaceContext) As StartElement

	  ''' <summary>
	  ''' Create a new EndElement </summary>
	  ''' <param name="name"> the qualified name of the EndElement </param>
	  ''' <param name="namespaces"> an optional unordered set of objects that
	  ''' implement Namespace that have gone out of scope, may be null </param>
	  ''' <returns> an instance of the requested EndElement </returns>
	  Public MustOverride Function createEndElement(ByVal name As javax.xml.namespace.QName, ByVal namespaces As IEnumerator) As EndElement

	  ''' <summary>
	  ''' Create a new EndElement </summary>
	  ''' <param name="namespaceUri"> the uri of the QName of the new StartElement </param>
	  ''' <param name="localName"> the local name of the QName of the new StartElement </param>
	  ''' <param name="prefix"> the prefix of the QName of the new StartElement </param>
	  ''' <returns> an instance of the requested EndElement </returns>
	  Public MustOverride Function createEndElement(ByVal prefix As String, ByVal namespaceUri As String, ByVal localName As String) As EndElement
	  ''' <summary>
	  ''' Create a new EndElement </summary>
	  ''' <param name="namespaceUri"> the uri of the QName of the new StartElement </param>
	  ''' <param name="localName"> the local name of the QName of the new StartElement </param>
	  ''' <param name="prefix"> the prefix of the QName of the new StartElement </param>
	  ''' <param name="namespaces"> an unordered set of objects that implement
	  ''' Namespace that have gone out of scope, may be null </param>
	  ''' <returns> an instance of the requested EndElement </returns>
	  Public MustOverride Function createEndElement(ByVal prefix As String, ByVal namespaceUri As String, ByVal localName As String, ByVal namespaces As IEnumerator) As EndElement

	  ''' <summary>
	  ''' Create a Characters event, this method does not check if the content
	  ''' is all whitespace.  To create a space event use #createSpace(String) </summary>
	  ''' <param name="content"> the string to create </param>
	  ''' <returns> a Characters event </returns>
	  Public MustOverride Function createCharacters(ByVal content As String) As Characters

	  ''' <summary>
	  ''' Create a Characters event with the CData flag set to true </summary>
	  ''' <param name="content"> the string to create </param>
	  ''' <returns> a Characters event </returns>
	  Public MustOverride Function createCData(ByVal content As String) As Characters

	  ''' <summary>
	  ''' Create a Characters event with the isSpace flag set to true </summary>
	  ''' <param name="content"> the content of the space to create </param>
	  ''' <returns> a Characters event </returns>
	  Public MustOverride Function createSpace(ByVal content As String) As Characters
	  ''' <summary>
	  ''' Create an ignorable space </summary>
	  ''' <param name="content"> the space to create </param>
	  ''' <returns> a Characters event </returns>
	  Public MustOverride Function createIgnorableSpace(ByVal content As String) As Characters

	  ''' <summary>
	  ''' Creates a new instance of a StartDocument event </summary>
	  ''' <returns> a StartDocument event </returns>
	  Public MustOverride Function createStartDocument() As StartDocument

	  ''' <summary>
	  ''' Creates a new instance of a StartDocument event
	  ''' </summary>
	  ''' <param name="encoding"> the encoding style </param>
	  ''' <param name="version"> the XML version </param>
	  ''' <param name="standalone"> the status of standalone may be set to "true" or "false" </param>
	  ''' <returns> a StartDocument event </returns>
	  Public MustOverride Function createStartDocument(ByVal encoding As String, ByVal version As String, ByVal standalone As Boolean) As StartDocument

	  ''' <summary>
	  ''' Creates a new instance of a StartDocument event
	  ''' </summary>
	  ''' <param name="encoding"> the encoding style </param>
	  ''' <param name="version"> the XML version </param>
	  ''' <returns> a StartDocument event </returns>
	  Public MustOverride Function createStartDocument(ByVal encoding As String, ByVal version As String) As StartDocument

	  ''' <summary>
	  ''' Creates a new instance of a StartDocument event
	  ''' </summary>
	  ''' <param name="encoding"> the encoding style </param>
	  ''' <returns> a StartDocument event </returns>
	  Public MustOverride Function createStartDocument(ByVal encoding As String) As StartDocument

	  ''' <summary>
	  ''' Creates a new instance of an EndDocument event </summary>
	  ''' <returns> an EndDocument event </returns>
	  Public MustOverride Function createEndDocument() As EndDocument

	  ''' <summary>
	  ''' Creates a new instance of a EntityReference event
	  ''' </summary>
	  ''' <param name="name"> The name of the reference </param>
	  ''' <param name="declaration"> the declaration for the event </param>
	  ''' <returns> an EntityReference event </returns>
	  Public MustOverride Function createEntityReference(ByVal name As String, ByVal declaration As EntityDeclaration) As EntityReference
	  ''' <summary>
	  ''' Create a comment </summary>
	  ''' <param name="text"> The text of the comment
	  ''' a Comment event </param>
	  Public MustOverride Function createComment(ByVal text As String) As Comment

	  ''' <summary>
	  ''' Create a processing instruction </summary>
	  ''' <param name="target"> The target of the processing instruction </param>
	  ''' <param name="data"> The text of the processing instruction </param>
	  ''' <returns> a ProcessingInstruction event </returns>
	  Public MustOverride Function createProcessingInstruction(ByVal target As String, ByVal data As String) As ProcessingInstruction

	  ''' <summary>
	  ''' Create a document type definition event
	  ''' This string contains the entire document type declaration that matches
	  ''' the doctypedecl in the XML 1.0 specification </summary>
	  ''' <param name="dtd"> the text of the document type definition </param>
	  ''' <returns> a DTD event </returns>
	  Public MustOverride Function createDTD(ByVal dtd As String) As DTD
	End Class

End Namespace