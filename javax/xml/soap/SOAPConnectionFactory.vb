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
	''' A factory for creating <code>SOAPConnection</code> objects. Implementation of this class
	''' is optional. If <code>SOAPConnectionFactory.newInstance()</code> throws an
	''' UnsupportedOperationException then the implementation does not support the
	''' SAAJ communication infrastructure. Otherwise <seealso cref="SOAPConnection"/> objects
	''' can be created by calling <code>createConnection()</code> on the newly
	''' created <code>SOAPConnectionFactory</code> object.
	''' </summary>
	Public MustInherit Class SOAPConnectionFactory
		''' <summary>
		''' A constant representing the default value for a <code>SOAPConnection</code>
		''' object. The default is the point-to-point SOAP connection.
		''' </summary>
		Friend Const DEFAULT_SOAP_CONNECTION_FACTORY As String = "com.sun.xml.internal.messaging.saaj.client.p2p.HttpSOAPConnectionFactory"

		''' <summary>
		''' A constant representing the <code>SOAPConnection</code> class.
		''' </summary>
		Private Const SF_PROPERTY As String = "javax.xml.soap.SOAPConnectionFactory"

		''' <summary>
		''' Creates an instance of the default
		''' <code>SOAPConnectionFactory</code> object.
		''' </summary>
		''' <returns> a new instance of a default
		'''         <code>SOAPConnectionFactory</code> object
		''' </returns>
		''' <exception cref="SOAPException"> if there was an error creating the
		'''            <code>SOAPConnectionFactory</code>
		''' </exception>
		''' <exception cref="UnsupportedOperationException"> if newInstance is not
		''' supported. </exception>
		Public Shared Function newInstance() As SOAPConnectionFactory
			Try
			Return CType(FactoryFinder.find(SF_PROPERTY, DEFAULT_SOAP_CONNECTION_FACTORY), SOAPConnectionFactory)
			Catch ex As Exception
				Throw New SOAPException("Unable to create SOAP connection factory: " & ex.Message)
			End Try
		End Function

		''' <summary>
		''' Create a new <code>SOAPConnection</code>.
		''' </summary>
		''' <returns> the new <code>SOAPConnection</code> object.
		''' </returns>
		''' <exception cref="SOAPException"> if there was an exception creating the
		''' <code>SOAPConnection</code> object. </exception>
		Public MustOverride Function createConnection() As SOAPConnection
	End Class

End Namespace