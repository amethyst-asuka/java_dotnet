Imports System

'
' * Copyright (c) 2005, 2013, Oracle and/or its affiliates. All rights reserved.
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

Namespace javax.xml.bind



	''' <summary>
	''' Enable synchronization between XML infoset nodes and JAXB objects
	''' representing same XML document.
	''' 
	''' <p>
	''' An instance of this class maintains the association between XML nodes of
	''' an infoset preserving view and a JAXB representation of an XML document.
	''' Navigation between the two views is provided by the methods
	''' <seealso cref="#getXMLNode(Object)"/> and <seealso cref="#getJAXBNode(Object)"/>.
	''' 
	''' <p>
	''' Modifications can be made to either the infoset preserving view or the
	''' JAXB representation of the document while the other view remains
	''' unmodified. The binder is able to synchronize the changes made in the
	''' modified view back into the other view using the appropriate
	''' Binder update methods, <seealso cref="#updateXML(Object, Object)"/> or
	''' <seealso cref="#updateJAXB(Object)"/>.
	''' 
	''' <p>
	''' A typical usage scenario is the following:
	''' <ul>
	'''   <li>load XML document into an XML infoset representation</li>
	'''   <li><seealso cref="#unmarshal(Object)"/> XML infoset view to JAXB view.
	'''       (Note to conserve resources, it is possible to only unmarshal a
	'''       subtree of the XML infoset view to the JAXB view.)</li>
	'''   <li>application access/updates JAXB view of XML document.</li>
	'''   <li><seealso cref="#updateXML(Object)"/> synchronizes modifications to JAXB view
	'''       back into the XML infoset view. Update operation preserves as
	'''       much of original XML infoset as possible (i.e. comments, PI, ...)</li>
	''' </ul>
	''' 
	''' <p>
	''' A Binder instance is created using the factory method
	''' <seealso cref="JAXBContext#createBinder()"/> or <seealso cref="JAXBContext#createBinder(Class)"/>.
	''' 
	''' <p>
	''' The template parameter, <code>XmlNode</code>, is the
	''' root interface/class for the XML infoset preserving representation.
	''' A Binder implementation is required to minimally support
	''' an <code>XmlNode</code> value of <code>org.w3c.dom.Node.class</code>.
	''' A Binder implementation can support alternative XML infoset
	''' preserving representations.
	''' 
	''' @author
	'''     Kohsuke Kawaguchi (kohsuke.kawaguchi@sun.com)
	'''     Joseph Fialli
	''' 
	''' @since JAXB 2.0
	''' </summary>
	Public MustInherit Class Binder(Of XmlNode)
		''' <summary>
		''' Unmarshal XML infoset view to a JAXB object tree.
		''' 
		''' <p>
		''' This method is similar to <seealso cref="Unmarshaller#unmarshal(Node)"/>
		''' with the addition of maintaining the association between XML nodes
		''' and the produced JAXB objects, enabling future update operations,
		''' <seealso cref="#updateXML(Object, Object)"/> or <seealso cref="#updateJAXB(Object)"/>.
		''' 
		''' <p>
		''' When <seealso cref="#getSchema()"/> is non-null, <code>xmlNode</code>
		''' and its descendants is validated during this operation.
		''' 
		''' <p>
		''' This method throws <seealso cref="UnmarshalException"/> when the Binder's
		''' <seealso cref="JAXBContext"/> does not have a mapping for the XML element name
		''' or the type, specifiable via <tt>@xsi:type</tt>, of <tt>xmlNode</tt>
		''' to a JAXB mapped class. The method <seealso cref="#unmarshal(Object, Class)"/>
		''' enables an application to specify the JAXB mapped class that
		''' the <tt>xmlNode</tt> should be mapped to.
		''' </summary>
		''' <param name="xmlNode">
		'''      the document/element to unmarshal XML data from.
		''' 
		''' @return
		'''      the newly created root object of the JAXB object tree.
		''' </param>
		''' <exception cref="JAXBException">
		'''      If any unexpected errors occur while unmarshalling </exception>
		''' <exception cref="UnmarshalException">
		'''     If the <seealso cref="ValidationEventHandler ValidationEventHandler"/>
		'''     returns false from its <tt>handleEvent</tt> method or the
		'''     <tt>Binder</tt> is unable to perform the XML to Java
		'''     binding. </exception>
		''' <exception cref="IllegalArgumentException">
		'''      If the node parameter is null </exception>
		Public MustOverride Function unmarshal(ByVal xmlNode As XmlNode) As Object

		''' <summary>
		''' Unmarshal XML root element by provided <tt>declaredType</tt>
		''' to a JAXB object tree.
		''' 
		''' <p>
		''' Implements <a href="Unmarshaller.html#unmarshalByDeclaredType">Unmarshal by Declared Type</a>
		''' 
		''' <p>
		''' This method is similar to <seealso cref="Unmarshaller#unmarshal(Node, Class)"/>
		''' with the addition of maintaining the association between XML nodes
		''' and the produced JAXB objects, enabling future update operations,
		''' <seealso cref="#updateXML(Object, Object)"/> or <seealso cref="#updateJAXB(Object)"/>.
		''' 
		''' <p>
		''' When <seealso cref="#getSchema()"/> is non-null, <code>xmlNode</code>
		''' and its descendants is validated during this operation.
		''' </summary>
		''' <param name="xmlNode">
		'''      the document/element to unmarshal XML data from. </param>
		''' <param name="declaredType">
		'''      appropriate JAXB mapped class to hold <tt>node</tt>'s XML data.
		''' 
		''' @return
		''' <a href="JAXBElement.html">JAXB Element</a> representation
		''' of <tt>node</tt>
		''' </param>
		''' <exception cref="JAXBException">
		'''      If any unexpected errors occur while unmarshalling </exception>
		''' <exception cref="UnmarshalException">
		'''     If the <seealso cref="ValidationEventHandler ValidationEventHandler"/>
		'''     returns false from its <tt>handleEvent</tt> method or the
		'''     <tt>Binder</tt> is unable to perform the XML to Java
		'''     binding. </exception>
		''' <exception cref="IllegalArgumentException">
		'''      If any of the input parameters are null
		''' @since JAXB2.0 </exception>
		Public MustOverride Function unmarshal(Of T)(ByVal xmlNode As XmlNode, ByVal declaredType As Type) As JAXBElement(Of T)

		''' <summary>
		''' Marshal a JAXB object tree to a new XML document.
		''' 
		''' <p>
		''' This method is similar to <seealso cref="Marshaller#marshal(Object, Node)"/>
		''' with the addition of maintaining the association between JAXB objects
		''' and the produced XML nodes,
		''' enabling future update operations such as
		''' <seealso cref="#updateXML(Object, Object)"/> or <seealso cref="#updateJAXB(Object)"/>.
		''' 
		''' <p>
		''' When <seealso cref="#getSchema()"/> is non-null, the marshalled
		''' xml content is validated during this operation.
		''' </summary>
		''' <param name="jaxbObject">
		'''      The content tree to be marshalled. </param>
		''' <param name="xmlNode">
		'''      The parameter must be a Node that accepts children.
		''' </param>
		''' <exception cref="JAXBException">
		'''      If any unexpected problem occurs during the marshalling. </exception>
		''' <exception cref="MarshalException">
		'''      If the <seealso cref="ValidationEventHandler ValidationEventHandler"/>
		'''      returns false from its <tt>handleEvent</tt> method or the
		'''      <tt>Binder</tt> is unable to marshal <tt>jaxbObject</tt> (or any
		'''      object reachable from <tt>jaxbObject</tt>).
		''' </exception>
		''' <exception cref="IllegalArgumentException">
		'''      If any of the method parameters are null </exception>
		Public MustOverride Sub marshal(ByVal jaxbObject As Object, ByVal xmlNode As XmlNode)

		''' <summary>
		''' Gets the XML element associated with the given JAXB object.
		''' 
		''' <p>
		''' Once a JAXB object tree is associated with an XML fragment,
		''' this method enables navigation between the two trees.
		''' 
		''' <p>
		''' An association between an XML element and a JAXB object is
		''' established by the bind methods and the update methods.
		''' Note that this association is partial; not all XML elements
		''' have associated JAXB objects, and not all JAXB objects have
		''' associated XML elements.
		''' </summary>
		''' <param name="jaxbObject"> An instance that is reachable from a prior
		'''                   call to a bind or update method that returned
		'''                   a JAXB object tree.
		''' 
		''' @return
		'''      null if the specified JAXB object is not known to this
		'''      <seealso cref="Binder"/>, or if it is not associated with an
		'''      XML element.
		''' </param>
		''' <exception cref="IllegalArgumentException">
		'''      If the jaxbObject parameter is null </exception>
		Public MustOverride Function getXMLNode(ByVal jaxbObject As Object) As XmlNode

		''' <summary>
		''' Gets the JAXB object associated with the given XML element.
		''' 
		''' <p>
		''' Once a JAXB object tree is associated with an XML fragment,
		''' this method enables navigation between the two trees.
		''' 
		''' <p>
		''' An association between an XML element and a JAXB object is
		''' established by the unmarshal, marshal and update methods.
		''' Note that this association is partial; not all XML elements
		''' have associated JAXB objects, and not all JAXB objects have
		''' associated XML elements.
		''' 
		''' @return
		'''      null if the specified XML node is not known to this
		'''      <seealso cref="Binder"/>, or if it is not associated with a
		'''      JAXB object.
		''' </summary>
		''' <exception cref="IllegalArgumentException">
		'''      If the node parameter is null </exception>
		Public MustOverride Function getJAXBNode(ByVal xmlNode As XmlNode) As Object

		''' <summary>
		''' Takes an JAXB object and updates
		''' its associated XML node and its descendants.
		''' 
		''' <p>
		''' This is a convenience method of:
		''' <pre>
		''' updateXML( jaxbObject, getXMLNode(jaxbObject));
		''' </pre>
		''' </summary>
		''' <exception cref="JAXBException">
		'''      If any unexpected problem occurs updating corresponding XML content. </exception>
		''' <exception cref="IllegalArgumentException">
		'''      If the jaxbObject parameter is null </exception>
		Public MustOverride Function updateXML(ByVal jaxbObject As Object) As XmlNode

		''' <summary>
		''' Changes in JAXB object tree are updated in its associated XML parse tree.
		''' 
		''' <p>
		''' This operation can be thought of as an "in-place" marshalling.
		''' The difference is that instead of creating a whole new XML tree,
		''' this operation updates an existing tree while trying to preserve
		''' the XML as much as possible.
		''' 
		''' <p>
		''' For example, unknown elements/attributes in XML that were not bound
		''' to JAXB will be left untouched (whereas a marshalling operation
		''' would create a new tree that doesn't contain any of those.)
		''' 
		''' <p>
		''' As a side-effect, this operation updates the association between
		''' XML nodes and JAXB objects.
		''' </summary>
		''' <param name="jaxbObject"> root of potentially modified JAXB object tree </param>
		''' <param name="xmlNode">    root of update target XML parse tree
		''' 
		''' @return
		'''      Returns the updated XML node. Typically, this is the same
		'''      node you passed in as <i>xmlNode</i>, but it maybe
		'''      a different object, for example when the tag name of the object
		'''      has changed.
		''' </param>
		''' <exception cref="JAXBException">
		'''      If any unexpected problem occurs updating corresponding XML content. </exception>
		''' <exception cref="IllegalArgumentException">
		'''      If any of the input parameters are null </exception>
		Public MustOverride Function updateXML(ByVal jaxbObject As Object, ByVal xmlNode As XmlNode) As XmlNode

		''' <summary>
		''' Takes an XML node and updates its associated JAXB object and its descendants.
		''' 
		''' <p>
		''' This operation can be thought of as an "in-place" unmarshalling.
		''' The difference is that instead of creating a whole new JAXB tree,
		''' this operation updates an existing tree, reusing as much JAXB objects
		''' as possible.
		''' 
		''' <p>
		''' As a side-effect, this operation updates the association between
		''' XML nodes and JAXB objects.
		''' 
		''' @return
		'''      Returns the updated JAXB object. Typically, this is the same
		'''      object that was returned from earlier
		'''      <seealso cref="#marshal(Object,Object)"/> or
		'''      <seealso cref="#updateJAXB(Object)"/> method invocation,
		'''      but it maybe
		'''      a different object, for example when the name of the XML
		'''      element has changed.
		''' </summary>
		''' <exception cref="JAXBException">
		'''      If any unexpected problem occurs updating corresponding JAXB mapped content. </exception>
		''' <exception cref="IllegalArgumentException">
		'''      If node parameter is null </exception>
		Public MustOverride Function updateJAXB(ByVal xmlNode As XmlNode) As Object


		''' <summary>
		''' Specifies whether marshal, unmarshal and update methods
		''' performs validation on their XML content.
		''' </summary>
		''' <param name="schema"> set to null to disable validation.
		''' </param>
		''' <seealso cref= Unmarshaller#setSchema(Schema) </seealso>
		Public MustOverride Property schema As javax.xml.validation.Schema


		''' <summary>
		''' Allow an application to register a <tt>ValidationEventHandler</tt>.
		''' <p>
		''' The <tt>ValidationEventHandler</tt> will be called by the JAXB Provider
		''' if any validation errors are encountered during calls to any of the
		''' Binder unmarshal, marshal and update methods.
		''' 
		''' <p>
		''' Calling this method with a null parameter will cause the Binder
		''' to revert back to the default default event handler.
		''' </summary>
		''' <param name="handler"> the validation event handler </param>
		''' <exception cref="JAXBException"> if an error was encountered while setting the
		'''         event handler </exception>
		Public MustOverride Property eventHandler As ValidationEventHandler


		''' 
		''' <summary>
		''' Set the particular property in the underlying implementation of
		''' <tt>Binder</tt>.  This method can only be used to set one of
		''' the standard JAXB defined unmarshal/marshal properties
		''' or a provider specific property for binder, unmarshal or marshal.
		''' Attempting to set an undefined property will result in
		''' a PropertyException being thrown.  See
		''' <a href="Unmarshaller.html#supportedProps">Supported Unmarshal Properties</a>
		''' and
		''' <a href="Marshaller.html#supportedProps">Supported Marshal Properties</a>.
		''' </summary>
		''' <param name="name"> the name of the property to be set. This value can either
		'''              be specified using one of the constant fields or a user
		'''              supplied string. </param>
		''' <param name="value"> the value of the property to be set
		''' </param>
		''' <exception cref="PropertyException"> when there is an error processing the given
		'''                            property or value </exception>
		''' <exception cref="IllegalArgumentException">
		'''      If the name parameter is null </exception>
		Public MustOverride Sub setProperty(ByVal name As String, ByVal value As Object)


		''' <summary>
		''' Get the particular property in the underlying implementation of
		''' <tt>Binder</tt>.  This method can only
		''' be used to get one of
		''' the standard JAXB defined unmarshal/marshal properties
		''' or a provider specific property for binder, unmarshal or marshal.
		''' Attempting to get an undefined property will result in
		''' a PropertyException being thrown.  See
		''' <a href="Unmarshaller.html#supportedProps">Supported Unmarshal Properties</a>
		''' and
		''' <a href="Marshaller.html#supportedProps">Supported Marshal Properties</a>.
		''' </summary>
		''' <param name="name"> the name of the property to retrieve </param>
		''' <returns> the value of the requested property
		''' </returns>
		''' <exception cref="PropertyException">
		'''      when there is an error retrieving the given property or value
		'''      property name </exception>
		''' <exception cref="IllegalArgumentException">
		'''      If the name parameter is null </exception>
		Public MustOverride Function getProperty(ByVal name As String) As Object

	End Class

End Namespace