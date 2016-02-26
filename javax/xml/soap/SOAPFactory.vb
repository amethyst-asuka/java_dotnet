Imports System

'
' * Copyright (c) 2004, 2013, Oracle and/or its affiliates. All rights reserved.
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
	''' <code>SOAPFactory</code> is a factory for creating various objects
	''' that exist in the SOAP XML tree.
	''' 
	''' <code>SOAPFactory</code> can be
	''' used to create XML fragments that will eventually end up in the
	''' SOAP part. These fragments can be inserted as children of the
	''' <seealso cref="SOAPHeaderElement"/> or <seealso cref="SOAPBodyElement"/> or
	''' <seealso cref="SOAPEnvelope"/> or other <seealso cref="SOAPElement"/> objects.
	''' 
	''' <code>SOAPFactory</code> also has methods to create
	''' <code>javax.xml.soap.Detail</code> objects as well as
	''' <code>java.xml.soap.Name</code> objects.
	''' 
	''' </summary>
	Public MustInherit Class SOAPFactory

		''' <summary>
		''' A constant representing the property used to lookup the name of
		''' a <code>SOAPFactory</code> implementation class.
		''' </summary>
		Private Const SOAP_FACTORY_PROPERTY As String = "javax.xml.soap.SOAPFactory"

		''' <summary>
		''' Class name of default <code>SOAPFactory</code> implementation.
		''' </summary>
		Friend Const DEFAULT_SOAP_FACTORY As String = "com.sun.xml.internal.messaging.saaj.soap.ver1_1.SOAPFactory1_1Impl"

		''' <summary>
		''' Creates a <code>SOAPElement</code> object from an existing DOM
		''' <code>Element</code>. If the DOM <code>Element</code> that is passed in
		''' as an argument is already a <code>SOAPElement</code> then this method
		''' must return it unmodified without any further work. Otherwise, a new
		''' <code>SOAPElement</code> is created and a deep copy is made of the
		''' <code>domElement</code> argument. The concrete type of the return value
		''' will depend on the name of the <code>domElement</code> argument. If any
		''' part of the tree rooted in <code>domElement</code> violates SOAP rules, a
		''' <code>SOAPException</code> will be thrown.
		''' </summary>
		''' <param name="domElement"> - the <code>Element</code> to be copied.
		''' </param>
		''' <returns> a new <code>SOAPElement</code> that is a copy of <code>domElement</code>.
		''' </returns>
		''' <exception cref="SOAPException"> if there is an error in creating the
		'''            <code>SOAPElement</code> object
		''' 
		''' @since SAAJ 1.3 </exception>
		Public Overridable Function createElement(ByVal domElement As org.w3c.dom.Element) As SOAPElement
			Throw New System.NotSupportedException("createElement(org.w3c.dom.Element) must be overridden by all subclasses of SOAPFactory.")
		End Function

		''' <summary>
		''' Creates a <code>SOAPElement</code> object initialized with the
		''' given <code>Name</code> object. The concrete type of the return value
		''' will depend on the name given to the new <code>SOAPElement</code>. For
		''' instance, a new <code>SOAPElement</code> with the name
		''' "{http://www.w3.org/2003/05/soap-envelope}Envelope" would cause a
		''' <code>SOAPEnvelope</code> that supports SOAP 1.2 behavior to be created.
		''' </summary>
		''' <param name="name"> a <code>Name</code> object with the XML name for
		'''             the new element
		''' </param>
		''' <returns> the new <code>SOAPElement</code> object that was
		'''         created
		''' </returns>
		''' <exception cref="SOAPException"> if there is an error in creating the
		'''            <code>SOAPElement</code> object </exception>
		''' <seealso cref= SOAPFactory#createElement(javax.xml.namespace.QName) </seealso>
		Public MustOverride Function createElement(ByVal name As Name) As SOAPElement

		''' <summary>
		''' Creates a <code>SOAPElement</code> object initialized with the
		''' given <code>QName</code> object. The concrete type of the return value
		''' will depend on the name given to the new <code>SOAPElement</code>. For
		''' instance, a new <code>SOAPElement</code> with the name
		''' "{http://www.w3.org/2003/05/soap-envelope}Envelope" would cause a
		''' <code>SOAPEnvelope</code> that supports SOAP 1.2 behavior to be created.
		''' </summary>
		''' <param name="qname"> a <code>QName</code> object with the XML name for
		'''             the new element
		''' </param>
		''' <returns> the new <code>SOAPElement</code> object that was
		'''         created
		''' </returns>
		''' <exception cref="SOAPException"> if there is an error in creating the
		'''            <code>SOAPElement</code> object </exception>
		''' <seealso cref= SOAPFactory#createElement(Name)
		''' @since SAAJ 1.3 </seealso>
		Public Overridable Function createElement(ByVal qname As javax.xml.namespace.QName) As SOAPElement
			Throw New System.NotSupportedException("createElement(QName) must be overridden by all subclasses of SOAPFactory.")
		End Function

		''' <summary>
		''' Creates a <code>SOAPElement</code> object initialized with the
		''' given local name.
		''' </summary>
		''' <param name="localName"> a <code>String</code> giving the local name for
		'''             the new element
		''' </param>
		''' <returns> the new <code>SOAPElement</code> object that was
		'''         created
		''' </returns>
		''' <exception cref="SOAPException"> if there is an error in creating the
		'''            <code>SOAPElement</code> object </exception>
		Public MustOverride Function createElement(ByVal localName As String) As SOAPElement


		''' <summary>
		''' Creates a new <code>SOAPElement</code> object with the given
		''' local name, prefix and uri. The concrete type of the return value
		''' will depend on the name given to the new <code>SOAPElement</code>. For
		''' instance, a new <code>SOAPElement</code> with the name
		''' "{http://www.w3.org/2003/05/soap-envelope}Envelope" would cause a
		''' <code>SOAPEnvelope</code> that supports SOAP 1.2 behavior to be created.
		''' </summary>
		''' <param name="localName"> a <code>String</code> giving the local name
		'''                  for the new element </param>
		''' <param name="prefix"> the prefix for this <code>SOAPElement</code> </param>
		''' <param name="uri"> a <code>String</code> giving the URI of the
		'''            namespace to which the new element belongs
		''' </param>
		''' <exception cref="SOAPException"> if there is an error in creating the
		'''            <code>SOAPElement</code> object </exception>
		Public MustOverride Function createElement(ByVal localName As String, ByVal prefix As String, ByVal uri As String) As SOAPElement

		''' <summary>
		''' Creates a new <code>Detail</code> object which serves as a container
		''' for <code>DetailEntry</code> objects.
		''' <P>
		''' This factory method creates <code>Detail</code> objects for use in
		''' situations where it is not practical to use the <code>SOAPFault</code>
		''' abstraction.
		''' </summary>
		''' <returns> a <code>Detail</code> object </returns>
		''' <exception cref="SOAPException"> if there is a SOAP error </exception>
		''' <exception cref="UnsupportedOperationException"> if the protocol specified
		'''         for the SOAPFactory was <code>DYNAMIC_SOAP_PROTOCOL</code> </exception>
		Public MustOverride Function createDetail() As Detail

		''' <summary>
		''' Creates a new <code>SOAPFault</code> object initialized with the given <code>reasonText</code>
		'''  and <code>faultCode</code> </summary>
		''' <param name="reasonText"> the ReasonText/FaultString for the fault </param>
		''' <param name="faultCode"> the FaultCode for the fault </param>
		''' <returns> a <code>SOAPFault</code> object </returns>
		''' <exception cref="SOAPException"> if there is a SOAP error
		''' @since SAAJ 1.3 </exception>
		Public MustOverride Function createFault(ByVal reasonText As String, ByVal faultCode As javax.xml.namespace.QName) As SOAPFault

		''' <summary>
		''' Creates a new default <code>SOAPFault</code> object </summary>
		''' <returns> a <code>SOAPFault</code> object </returns>
		''' <exception cref="SOAPException"> if there is a SOAP error
		''' @since SAAJ 1.3 </exception>
		Public MustOverride Function createFault() As SOAPFault

		''' <summary>
		''' Creates a new <code>Name</code> object initialized with the
		''' given local name, namespace prefix, and namespace URI.
		''' <P>
		''' This factory method creates <code>Name</code> objects for use in
		''' situations where it is not practical to use the <code>SOAPEnvelope</code>
		''' abstraction.
		''' </summary>
		''' <param name="localName"> a <code>String</code> giving the local name </param>
		''' <param name="prefix"> a <code>String</code> giving the prefix of the namespace </param>
		''' <param name="uri"> a <code>String</code> giving the URI of the namespace </param>
		''' <returns> a <code>Name</code> object initialized with the given
		'''         local name, namespace prefix, and namespace URI </returns>
		''' <exception cref="SOAPException"> if there is a SOAP error </exception>
		Public MustOverride Function createName(ByVal localName As String, ByVal prefix As String, ByVal uri As String) As Name

		''' <summary>
		''' Creates a new <code>Name</code> object initialized with the
		''' given local name.
		''' <P>
		''' This factory method creates <code>Name</code> objects for use in
		''' situations where it is not practical to use the <code>SOAPEnvelope</code>
		''' abstraction.
		''' </summary>
		''' <param name="localName"> a <code>String</code> giving the local name </param>
		''' <returns> a <code>Name</code> object initialized with the given
		'''         local name </returns>
		''' <exception cref="SOAPException"> if there is a SOAP error </exception>
		Public MustOverride Function createName(ByVal localName As String) As Name

		''' <summary>
		''' Creates a new <code>SOAPFactory</code> object that is an instance of
		''' the default implementation (SOAP 1.1),
		''' 
		''' This method uses the following ordered lookup procedure to determine the SOAPFactory implementation class to load:
		''' <UL>
		'''  <LI> Use the javax.xml.soap.SOAPFactory system property.
		'''  <LI> Use the properties file "lib/jaxm.properties" in the JRE directory. This configuration file is in standard
		''' java.util.Properties format and contains the fully qualified name of the implementation class with the key being the
		''' system property defined above.
		'''  <LI> Use the Services API (as detailed in the JAR specification), if available, to determine the classname. The Services API
		''' will look for a classname in the file META-INF/services/javax.xml.soap.SOAPFactory in jars available to the runtime.
		'''  <LI> Use the SAAJMetaFactory instance to locate the SOAPFactory implementation class.
		''' </UL>
		''' </summary>
		''' <returns> a new instance of a <code>SOAPFactory</code>
		''' </returns>
		''' <exception cref="SOAPException"> if there was an error creating the
		'''            default <code>SOAPFactory</code> </exception>
		''' <seealso cref= SAAJMetaFactory </seealso>
		Public Shared Function newInstance() As SOAPFactory
			Try
				Dim factory As SOAPFactory = CType(FactoryFinder.find(SOAP_FACTORY_PROPERTY, DEFAULT_SOAP_FACTORY, False), SOAPFactory)
				If factory IsNot Nothing Then Return factory
				Return newInstance(SOAPConstants.SOAP_1_1_PROTOCOL)
			Catch ex As Exception
				Throw New SOAPException("Unable to create SOAP Factory: " & ex.Message)
			End Try

		End Function

		''' <summary>
		''' Creates a new <code>SOAPFactory</code> object that is an instance of
		''' the specified implementation, this method uses the SAAJMetaFactory to
		''' locate the implementation class and create the SOAPFactory instance.
		''' </summary>
		''' <returns> a new instance of a <code>SOAPFactory</code>
		''' </returns>
		''' <param name="protocol">  a string constant representing the protocol of the
		'''                   specified SOAP factory implementation. May be
		'''                   either <code>DYNAMIC_SOAP_PROTOCOL</code>,
		'''                   <code>DEFAULT_SOAP_PROTOCOL</code> (which is the same
		'''                   as) <code>SOAP_1_1_PROTOCOL</code>, or
		'''                   <code>SOAP_1_2_PROTOCOL</code>.
		''' </param>
		''' <exception cref="SOAPException"> if there was an error creating the
		'''            specified <code>SOAPFactory</code> </exception>
		''' <seealso cref= SAAJMetaFactory
		''' @since SAAJ 1.3 </seealso>
		Public Shared Function newInstance(ByVal protocol As String) As SOAPFactory
				Return SAAJMetaFactory.instance.newSOAPFactory(protocol)
		End Function
	End Class

End Namespace