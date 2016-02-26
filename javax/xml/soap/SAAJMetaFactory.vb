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
	''' The access point for the implementation classes of the factories defined in the
	''' SAAJ API. All of the <code>newInstance</code> methods defined on factories in
	''' SAAJ 1.3 defer to instances of this class to do the actual object creation.
	''' The implementations of <code>newInstance()</code> methods (in SOAPFactory and MessageFactory)
	''' that existed in SAAJ 1.2 have been updated to also delegate to the SAAJMetaFactory when the SAAJ 1.2
	''' defined lookup fails to locate the Factory implementation class name.
	''' 
	''' <p>
	''' SAAJMetaFactory is a service provider interface. There are no public methods on this
	''' class.
	''' 
	''' @author SAAJ RI Development Team
	''' @since SAAJ 1.3
	''' </summary>

	Public MustInherit Class SAAJMetaFactory
		Private Const META_FACTORY_CLASS_PROPERTY As String = "javax.xml.soap.MetaFactory"
		Friend Const DEFAULT_META_FACTORY_CLASS As String = "com.sun.xml.internal.messaging.saaj.soap.SAAJMetaFactoryImpl"

		''' <summary>
		''' Creates a new instance of a concrete <code>SAAJMetaFactory</code> object.
		''' The SAAJMetaFactory is an SPI, it pulls the creation of the other factories together into a
		''' single place. Changing out the SAAJMetaFactory has the effect of changing out the entire SAAJ
		''' implementation. Service providers provide the name of their <code>SAAJMetaFactory</code>
		''' implementation.
		''' 
		''' This method uses the following ordered lookup procedure to determine the SAAJMetaFactory implementation class to load:
		''' <UL>
		'''  <LI> Use the javax.xml.soap.MetaFactory system property.
		'''  <LI> Use the properties file "lib/jaxm.properties" in the JRE directory. This configuration file is in standard
		''' java.util.Properties format and contains the fully qualified name of the implementation class with the key being the
		''' system property defined above.
		'''  <LI> Use the Services API (as detailed in the JAR specification), if available, to determine the classname. The Services API
		''' will look for a classname in the file META-INF/services/javax.xml.soap.MetaFactory in jars available to the runtime.
		'''  <LI> Default to com.sun.xml.internal.messaging.saaj.soap.SAAJMetaFactoryImpl.
		''' </UL>
		''' </summary>
		''' <returns> a concrete <code>SAAJMetaFactory</code> object </returns>
		''' <exception cref="SOAPException"> if there is an error in creating the <code>SAAJMetaFactory</code> </exception>
		Shared instance As SAAJMetaFactory
			Get
					Try
						Dim ___instance As SAAJMetaFactory = CType(FactoryFinder.find(META_FACTORY_CLASS_PROPERTY, DEFAULT_META_FACTORY_CLASS), SAAJMetaFactory)
						Return ___instance
					Catch e As Exception
						Throw New SOAPException("Unable to create SAAJ meta-factory" & e.Message)
					End Try
			End Get
		End Property

		Protected Friend Sub New()
		End Sub

		 ''' <summary>
		 ''' Creates a <code>MessageFactory</code> object for
		 ''' the given <code>String</code> protocol.
		 ''' </summary>
		 ''' <param name="protocol"> a <code>String</code> indicating the protocol </param>
		 ''' <exception cref="SOAPException"> if there is an error in creating the
		 '''            MessageFactory </exception>
		 ''' <seealso cref= SOAPConstants#SOAP_1_1_PROTOCOL </seealso>
		 ''' <seealso cref= SOAPConstants#SOAP_1_2_PROTOCOL </seealso>
		 ''' <seealso cref= SOAPConstants#DYNAMIC_SOAP_PROTOCOL </seealso>
		Protected Friend MustOverride Function newMessageFactory(ByVal protocol As String) As MessageFactory

		 ''' <summary>
		 ''' Creates a <code>SOAPFactory</code> object for
		 ''' the given <code>String</code> protocol.
		 ''' </summary>
		 ''' <param name="protocol"> a <code>String</code> indicating the protocol </param>
		 ''' <exception cref="SOAPException"> if there is an error in creating the
		 '''            SOAPFactory </exception>
		 ''' <seealso cref= SOAPConstants#SOAP_1_1_PROTOCOL </seealso>
		 ''' <seealso cref= SOAPConstants#SOAP_1_2_PROTOCOL </seealso>
		 ''' <seealso cref= SOAPConstants#DYNAMIC_SOAP_PROTOCOL </seealso>
		Protected Friend MustOverride Function newSOAPFactory(ByVal protocol As String) As SOAPFactory
	End Class

End Namespace