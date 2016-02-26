Imports System.Collections

'
' * Copyright (c) 2004, 2012, Oracle and/or its affiliates. All rights reserved.
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

Namespace javax.xml.soap



	''' <summary>
	''' An object representing an element of a SOAP message that is allowed but not
	''' specifically prescribed by a SOAP specification. This interface serves as the
	''' base interface for those objects that are specifically prescribed by a SOAP
	''' specification.
	''' <p>
	''' Methods in this interface that are required to return SAAJ specific objects
	''' may "silently" replace nodes in the tree as required to successfully return
	''' objects of the correct type. See <seealso cref="#getChildElements()"/> and
	''' <seealso cref="<a HREF="package-summary.html#package_description">javax.xml.soap<a>"/>
	''' for details.
	''' </summary>
	Public Interface SOAPElement
		Inherits Node, org.w3c.dom.Element

		''' <summary>
		''' Creates a new <code>SOAPElement</code> object initialized with the
		''' given <code>Name</code> object and adds the new element to this
		''' <code>SOAPElement</code> object.
		''' <P>
		''' This method may be deprecated in a future release of SAAJ in favor of
		''' addChildElement(javax.xml.namespace.QName)
		''' </summary>
		''' <param name="name"> a <code>Name</code> object with the XML name for the
		'''        new element
		''' </param>
		''' <returns> the new <code>SOAPElement</code> object that was created </returns>
		''' <exception cref="SOAPException"> if there is an error in creating the
		'''                          <code>SOAPElement</code> object </exception>
		''' <seealso cref= SOAPElement#addChildElement(javax.xml.namespace.QName) </seealso>
		Function addChildElement(ByVal name As Name) As SOAPElement

		''' <summary>
		''' Creates a new <code>SOAPElement</code> object initialized with the given
		''' <code>QName</code> object and adds the new element to this <code>SOAPElement</code>
		'''  object. The  <i>namespace</i>, <i>localname</i> and <i>prefix</i> of the new
		''' <code>SOAPElement</code> are all taken  from the <code>qname</code> argument.
		''' </summary>
		''' <param name="qname"> a <code>QName</code> object with the XML name for the
		'''        new element
		''' </param>
		''' <returns> the new <code>SOAPElement</code> object that was created </returns>
		''' <exception cref="SOAPException"> if there is an error in creating the
		'''                          <code>SOAPElement</code> object </exception>
		''' <seealso cref= SOAPElement#addChildElement(Name)
		''' @since SAAJ 1.3 </seealso>
		Function addChildElement(ByVal qname As javax.xml.namespace.QName) As SOAPElement

		''' <summary>
		''' Creates a new <code>SOAPElement</code> object initialized with the
		''' specified local name and adds the new element to this
		''' <code>SOAPElement</code> object.
		''' The new  <code>SOAPElement</code> inherits any in-scope default namespace.
		''' </summary>
		''' <param name="localName"> a <code>String</code> giving the local name for
		'''          the element </param>
		''' <returns> the new <code>SOAPElement</code> object that was created </returns>
		''' <exception cref="SOAPException"> if there is an error in creating the
		'''                          <code>SOAPElement</code> object </exception>
		Function addChildElement(ByVal localName As String) As SOAPElement

		''' <summary>
		''' Creates a new <code>SOAPElement</code> object initialized with the
		''' specified local name and prefix and adds the new element to this
		''' <code>SOAPElement</code> object.
		''' </summary>
		''' <param name="localName"> a <code>String</code> giving the local name for
		'''        the new element </param>
		''' <param name="prefix"> a <code>String</code> giving the namespace prefix for
		'''        the new element
		''' </param>
		''' <returns> the new <code>SOAPElement</code> object that was created </returns>
		''' <exception cref="SOAPException"> if the <code>prefix</code> is not valid in the
		'''         context of this <code>SOAPElement</code> or  if there is an error in creating the
		'''                          <code>SOAPElement</code> object </exception>
		Function addChildElement(ByVal localName As String, ByVal prefix As String) As SOAPElement

		''' <summary>
		''' Creates a new <code>SOAPElement</code> object initialized with the
		''' specified local name, prefix, and URI and adds the new element to this
		''' <code>SOAPElement</code> object.
		''' </summary>
		''' <param name="localName"> a <code>String</code> giving the local name for
		'''        the new element </param>
		''' <param name="prefix"> a <code>String</code> giving the namespace prefix for
		'''        the new element </param>
		''' <param name="uri"> a <code>String</code> giving the URI of the namespace
		'''        to which the new element belongs
		''' </param>
		''' <returns> the new <code>SOAPElement</code> object that was created </returns>
		''' <exception cref="SOAPException"> if there is an error in creating the
		'''                          <code>SOAPElement</code> object </exception>
		Function addChildElement(ByVal localName As String, ByVal prefix As String, ByVal uri As String) As SOAPElement

		''' <summary>
		''' Add a <code>SOAPElement</code> as a child of this
		''' <code>SOAPElement</code> instance. The <code>SOAPElement</code>
		''' is expected to be created by a
		''' <code>SOAPFactory</code>. Callers should not rely on the
		''' element instance being added as is into the XML
		''' tree. Implementations could end up copying the content
		''' of the <code>SOAPElement</code> passed into an instance of
		''' a different <code>SOAPElement</code> implementation. For
		''' instance if <code>addChildElement()</code> is called on a
		''' <code>SOAPHeader</code>, <code>element</code> will be copied
		''' into an instance of a <code>SOAPHeaderElement</code>.
		''' 
		''' <P>The fragment rooted in <code>element</code> is either added
		''' as a whole or not at all, if there was an error.
		''' 
		''' <P>The fragment rooted in <code>element</code> cannot contain
		''' elements named "Envelope", "Header" or "Body" and in the SOAP
		''' namespace. Any namespace prefixes present in the fragment
		''' should be fully resolved using appropriate namespace
		''' declarations within the fragment itself.
		''' </summary>
		''' <param name="element"> the <code>SOAPElement</code> to be added as a
		'''                new child
		''' </param>
		''' <exception cref="SOAPException"> if there was an error in adding this
		'''                          element as a child
		''' </exception>
		''' <returns> an instance representing the new SOAP element that was
		'''         actually added to the tree. </returns>
		Function addChildElement(ByVal element As SOAPElement) As SOAPElement

		''' <summary>
		''' Detaches all children of this <code>SOAPElement</code>.
		''' <p>
		''' This method is useful for rolling back the construction of partially
		''' completed <code>SOAPHeaders</code> and <code>SOAPBodys</code> in
		''' preparation for sending a fault when an error condition is detected. It
		''' is also useful for recycling portions of a document within a SOAP
		''' message.
		''' 
		''' @since SAAJ 1.2
		''' </summary>
		Sub removeContents()

		''' <summary>
		''' Creates a new <code>Text</code> object initialized with the given
		''' <code>String</code> and adds it to this <code>SOAPElement</code> object.
		''' </summary>
		''' <param name="text"> a <code>String</code> object with the textual content to be added
		''' </param>
		''' <returns> the <code>SOAPElement</code> object into which
		'''         the new <code>Text</code> object was inserted </returns>
		''' <exception cref="SOAPException"> if there is an error in creating the
		'''                    new <code>Text</code> object or if it is not legal to
		'''                      attach it as a child to this
		'''                      <code>SOAPElement</code> </exception>
		Function addTextNode(ByVal text As String) As SOAPElement

		''' <summary>
		''' Adds an attribute with the specified name and value to this
		''' <code>SOAPElement</code> object.
		''' </summary>
		''' <param name="name"> a <code>Name</code> object with the name of the attribute </param>
		''' <param name="value"> a <code>String</code> giving the value of the attribute </param>
		''' <returns> the <code>SOAPElement</code> object into which the attribute was
		'''         inserted
		''' </returns>
		''' <exception cref="SOAPException"> if there is an error in creating the
		'''                          Attribute, or it is invalid to set
		'''                            an attribute with <code>Name</code>
		'''                             <code>name</code> on this SOAPElement. </exception>
		''' <seealso cref= SOAPElement#addAttribute(javax.xml.namespace.QName, String) </seealso>
		Function addAttribute(ByVal name As Name, ByVal value As String) As SOAPElement

		''' <summary>
		''' Adds an attribute with the specified name and value to this
		''' <code>SOAPElement</code> object.
		''' </summary>
		''' <param name="qname"> a <code>QName</code> object with the name of the attribute </param>
		''' <param name="value"> a <code>String</code> giving the value of the attribute </param>
		''' <returns> the <code>SOAPElement</code> object into which the attribute was
		'''         inserted
		''' </returns>
		''' <exception cref="SOAPException"> if there is an error in creating the
		'''                          Attribute, or it is invalid to set
		'''                            an attribute with <code>QName</code>
		'''                            <code>qname</code> on this SOAPElement. </exception>
		''' <seealso cref= SOAPElement#addAttribute(Name, String)
		''' @since SAAJ 1.3 </seealso>
		Function addAttribute(ByVal qname As javax.xml.namespace.QName, ByVal value As String) As SOAPElement

		''' <summary>
		''' Adds a namespace declaration with the specified prefix and URI to this
		''' <code>SOAPElement</code> object.
		''' </summary>
		''' <param name="prefix"> a <code>String</code> giving the prefix of the namespace </param>
		''' <param name="uri"> a <code>String</code> giving the uri of the namespace </param>
		''' <returns> the <code>SOAPElement</code> object into which this
		'''          namespace declaration was inserted.
		''' </returns>
		''' <exception cref="SOAPException"> if there is an error in creating the
		'''                          namespace </exception>
		Function addNamespaceDeclaration(ByVal prefix As String, ByVal uri As String) As SOAPElement

		''' <summary>
		''' Returns the value of the attribute with the specified name.
		''' </summary>
		''' <param name="name"> a <code>Name</code> object with the name of the attribute </param>
		''' <returns> a <code>String</code> giving the value of the specified
		'''         attribute, Null if there is no such attribute </returns>
		''' <seealso cref= SOAPElement#getAttributeValue(javax.xml.namespace.QName) </seealso>
		Function getAttributeValue(ByVal name As Name) As String

		''' <summary>
		''' Returns the value of the attribute with the specified qname.
		''' </summary>
		''' <param name="qname"> a <code>QName</code> object with the qname of the attribute </param>
		''' <returns> a <code>String</code> giving the value of the specified
		'''         attribute, Null if there is no such attribute </returns>
		''' <seealso cref= SOAPElement#getAttributeValue(Name)
		''' @since SAAJ 1.3 </seealso>
		Function getAttributeValue(ByVal qname As javax.xml.namespace.QName) As String

		''' <summary>
		''' Returns an <code>Iterator</code> over all of the attribute
		''' <code>Name</code> objects in this
		''' <code>SOAPElement</code> object. The iterator can be used to get
		''' the attribute names, which can then be passed to the method
		''' <code>getAttributeValue</code> to retrieve the value of each
		''' attribute.
		''' </summary>
		''' <seealso cref= SOAPElement#getAllAttributesAsQNames() </seealso>
		''' <returns> an iterator over the names of the attributes </returns>
		ReadOnly Property allAttributes As IEnumerator

		''' <summary>
		''' Returns an <code>Iterator</code> over all of the attributes
		''' in this <code>SOAPElement</code>  as <code>QName</code> objects.
		''' The iterator can be used to get the attribute QName, which can then
		''' be passed to the method <code>getAttributeValue</code> to retrieve
		''' the value of each attribute.
		''' </summary>
		''' <returns> an iterator over the QNames of the attributes </returns>
		''' <seealso cref= SOAPElement#getAllAttributes()
		''' @since SAAJ 1.3 </seealso>
		ReadOnly Property allAttributesAsQNames As IEnumerator


		''' <summary>
		''' Returns the URI of the namespace that has the given prefix.
		''' </summary>
		''' <param name="prefix"> a <code>String</code> giving the prefix of the namespace
		'''        for which to search </param>
		''' <returns> a <code>String</code> with the uri of the namespace that has
		'''        the given prefix </returns>
		Function getNamespaceURI(ByVal prefix As String) As String

		''' <summary>
		''' Returns an <code>Iterator</code> over the namespace prefix
		''' <code>String</code>s declared by this element. The prefixes returned by
		''' this iterator can be passed to the method
		''' <code>getNamespaceURI</code> to retrieve the URI of each namespace.
		''' </summary>
		''' <returns> an iterator over the namespace prefixes in this
		'''         <code>SOAPElement</code> object </returns>
		ReadOnly Property namespacePrefixes As IEnumerator

		''' <summary>
		''' Returns an <code>Iterator</code> over the namespace prefix
		''' <code>String</code>s visible to this element. The prefixes returned by
		''' this iterator can be passed to the method
		''' <code>getNamespaceURI</code> to retrieve the URI of each namespace.
		''' </summary>
		''' <returns> an iterator over the namespace prefixes are within scope of this
		'''         <code>SOAPElement</code> object
		''' 
		''' @since SAAJ 1.2 </returns>
		ReadOnly Property visibleNamespacePrefixes As IEnumerator

		''' <summary>
		''' Creates a <code>QName</code> whose namespace URI is the one associated
		''' with the parameter, <code>prefix</code>, in the context of this
		''' <code>SOAPElement</code>. The remaining elements of the new
		''' <code>QName</code> are taken directly from the parameters,
		''' <code>localName</code> and <code>prefix</code>.
		''' </summary>
		''' <param name="localName">
		'''          a <code>String</code> containing the local part of the name. </param>
		''' <param name="prefix">
		'''          a <code>String</code> containing the prefix for the name.
		''' </param>
		''' <returns> a <code>QName</code> with the specified <code>localName</code>
		'''          and <code>prefix</code>, and with a namespace that is associated
		'''          with the <code>prefix</code> in the context of this
		'''          <code>SOAPElement</code>. This namespace will be the same as
		'''          the one that would be returned by
		'''          <code><seealso cref="#getNamespaceURI(String)"/></code> if it were given
		'''          <code>prefix</code> as it's parameter.
		''' </returns>
		''' <exception cref="SOAPException"> if the <code>QName</code> cannot be created.
		''' 
		''' @since SAAJ 1.3 </exception>
		Function createQName(ByVal localName As String, ByVal prefix As String) As javax.xml.namespace.QName
		''' <summary>
		''' Returns the name of this <code>SOAPElement</code> object.
		''' </summary>
		''' <returns> a <code>Name</code> object with the name of this
		'''         <code>SOAPElement</code> object </returns>
		ReadOnly Property elementName As Name

		''' <summary>
		''' Returns the qname of this <code>SOAPElement</code> object.
		''' </summary>
		''' <returns> a <code>QName</code> object with the qname of this
		'''         <code>SOAPElement</code> object </returns>
		''' <seealso cref= SOAPElement#getElementName()
		''' @since SAAJ 1.3 </seealso>
		ReadOnly Property elementQName As javax.xml.namespace.QName

		''' <summary>
		''' Changes the name of this <code>Element</code> to <code>newName</code> if
		''' possible. SOAP Defined elements such as SOAPEnvelope, SOAPHeader, SOAPBody
		''' etc. cannot have their names changed using this method. Any attempt to do
		''' so will result in a  SOAPException being thrown.
		''' <P>
		''' Callers should not rely on the element instance being renamed as is.
		''' Implementations could end up copying the content of the
		''' <code>SOAPElement</code> to a renamed instance.
		''' </summary>
		''' <param name="newName"> the new name for the <code>Element</code>.
		''' </param>
		''' <exception cref="SOAPException"> if changing the name of this <code>Element</code>
		'''                          is not allowed. </exception>
		''' <returns> The renamed Node
		''' 
		''' @since SAAJ 1.3 </returns>
	   Function setElementQName(ByVal newName As javax.xml.namespace.QName) As SOAPElement

	   ''' <summary>
	   ''' Removes the attribute with the specified name.
	   ''' </summary>
	   ''' <param name="name"> the <code>Name</code> object with the name of the
	   '''        attribute to be removed </param>
	   ''' <returns> <code>true</code> if the attribute was
	   '''         removed successfully; <code>false</code> if it was not </returns>
	   ''' <seealso cref= SOAPElement#removeAttribute(javax.xml.namespace.QName) </seealso>
		Function removeAttribute(ByVal name As Name) As Boolean

		''' <summary>
		''' Removes the attribute with the specified qname.
		''' </summary>
		''' <param name="qname"> the <code>QName</code> object with the qname of the
		'''        attribute to be removed </param>
		''' <returns> <code>true</code> if the attribute was
		'''         removed successfully; <code>false</code> if it was not </returns>
		''' <seealso cref= SOAPElement#removeAttribute(Name)
		''' @since SAAJ 1.3 </seealso>
		Function removeAttribute(ByVal qname As javax.xml.namespace.QName) As Boolean

		''' <summary>
		''' Removes the namespace declaration corresponding to the given prefix.
		''' </summary>
		''' <param name="prefix"> a <code>String</code> giving the prefix for which
		'''        to search </param>
		''' <returns> <code>true</code> if the namespace declaration was
		'''         removed successfully; <code>false</code> if it was not </returns>
		Function removeNamespaceDeclaration(ByVal prefix As String) As Boolean

		''' <summary>
		''' Returns an <code>Iterator</code> over all the immediate child
		''' <seealso cref="Node"/>s of this element. This includes <code>javax.xml.soap.Text</code>
		''' objects as well as <code>SOAPElement</code> objects.
		''' <p>
		''' Calling this method may cause child <code>Element</code>,
		''' <code>SOAPElement</code> and <code>org.w3c.dom.Text</code> nodes to be
		''' replaced by <code>SOAPElement</code>, <code>SOAPHeaderElement</code>,
		''' <code>SOAPBodyElement</code> or <code>javax.xml.soap.Text</code> nodes as
		''' appropriate for the type of this parent node. As a result the calling
		''' application must treat any existing references to these child nodes that
		''' have been obtained through DOM APIs as invalid and either discard them or
		''' refresh them with the values returned by this <code>Iterator</code>. This
		''' behavior can be avoided by calling the equivalent DOM APIs. See
		''' <seealso cref="<a HREF="package-summary.html#package_description">javax.xml.soap<a>"/>
		''' for more details.
		''' </summary>
		''' <returns> an iterator with the content of this <code>SOAPElement</code>
		'''         object </returns>
		ReadOnly Property childElements As IEnumerator

		''' <summary>
		''' Returns an <code>Iterator</code> over all the immediate child
		''' <seealso cref="Node"/>s of this element with the specified name. All of these
		''' children will be <code>SOAPElement</code> nodes.
		''' <p>
		''' Calling this method may cause child <code>Element</code>,
		''' <code>SOAPElement</code> and <code>org.w3c.dom.Text</code> nodes to be
		''' replaced by <code>SOAPElement</code>, <code>SOAPHeaderElement</code>,
		''' <code>SOAPBodyElement</code> or <code>javax.xml.soap.Text</code> nodes as
		''' appropriate for the type of this parent node. As a result the calling
		''' application must treat any existing references to these child nodes that
		''' have been obtained through DOM APIs as invalid and either discard them or
		''' refresh them with the values returned by this <code>Iterator</code>. This
		''' behavior can be avoided by calling the equivalent DOM APIs. See
		''' <seealso cref="<a HREF="package-summary.html#package_description">javax.xml.soap<a>"/>
		''' for more details.
		''' </summary>
		''' <param name="name"> a <code>Name</code> object with the name of the child
		'''        elements to be returned
		''' </param>
		''' <returns> an <code>Iterator</code> object over all the elements
		'''         in this <code>SOAPElement</code> object with the
		'''         specified name </returns>
		''' <seealso cref= SOAPElement#getChildElements(javax.xml.namespace.QName) </seealso>
		Function getChildElements(ByVal name As Name) As IEnumerator

		''' <summary>
		''' Returns an <code>Iterator</code> over all the immediate child
		''' <seealso cref="Node"/>s of this element with the specified qname. All of these
		''' children will be <code>SOAPElement</code> nodes.
		''' <p>
		''' Calling this method may cause child <code>Element</code>,
		''' <code>SOAPElement</code> and <code>org.w3c.dom.Text</code> nodes to be
		''' replaced by <code>SOAPElement</code>, <code>SOAPHeaderElement</code>,
		''' <code>SOAPBodyElement</code> or <code>javax.xml.soap.Text</code> nodes as
		''' appropriate for the type of this parent node. As a result the calling
		''' application must treat any existing references to these child nodes that
		''' have been obtained through DOM APIs as invalid and either discard them or
		''' refresh them with the values returned by this <code>Iterator</code>. This
		''' behavior can be avoided by calling the equivalent DOM APIs. See
		''' <seealso cref="<a HREF="package-summary.html#package_description">javax.xml.soap<a>"/>
		''' for more details.
		''' </summary>
		''' <param name="qname"> a <code>QName</code> object with the qname of the child
		'''        elements to be returned
		''' </param>
		''' <returns> an <code>Iterator</code> object over all the elements
		'''         in this <code>SOAPElement</code> object with the
		'''         specified qname </returns>
		''' <seealso cref= SOAPElement#getChildElements(Name)
		''' @since SAAJ 1.3 </seealso>
		Function getChildElements(ByVal qname As javax.xml.namespace.QName) As IEnumerator

		''' <summary>
		''' Sets the encoding style for this <code>SOAPElement</code> object
		''' to one specified.
		''' </summary>
		''' <param name="encodingStyle"> a <code>String</code> giving the encoding style
		''' </param>
		''' <exception cref="IllegalArgumentException"> if there was a problem in the
		'''            encoding style being set. </exception>
		''' <exception cref="SOAPException"> if setting the encodingStyle is invalid for this SOAPElement. </exception>
		''' <seealso cref= #getEncodingStyle </seealso>
		Property encodingStyle As String
	End Interface

End Namespace