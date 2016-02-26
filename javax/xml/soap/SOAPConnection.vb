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
	''' A point-to-point connection that a client can use for sending messages
	''' directly to a remote party (represented by a URL, for instance).
	''' <p>
	''' The SOAPConnection class is optional. Some implementations may
	''' not implement this interface in which case the call to
	''' <code>SOAPConnectionFactory.newInstance()</code> (see below) will
	''' throw an <code>UnsupportedOperationException</code>.
	''' <p>
	''' A client can obtain a <code>SOAPConnection</code> object using a
	''' <seealso cref="SOAPConnectionFactory"/> object as in the following example:
	''' <PRE>
	'''      SOAPConnectionFactory factory = SOAPConnectionFactory.newInstance();
	'''      SOAPConnection con = factory.createConnection();
	''' </PRE>
	''' A <code>SOAPConnection</code> object can be used to send messages
	''' directly to a URL following the request/response paradigm.  That is,
	''' messages are sent using the method <code>call</code>, which sends the
	''' message and then waits until it gets a reply.
	''' </summary>
	Public MustInherit Class SOAPConnection

		''' <summary>
		''' Sends the given message to the specified endpoint and blocks until
		''' it has returned the response.
		''' </summary>
		''' <param name="request"> the <code>SOAPMessage</code> object to be sent </param>
		''' <param name="to"> an <code>Object</code> that identifies
		'''         where the message should be sent. It is required to
		'''         support Objects of type
		'''         <code>java.lang.String</code>,
		'''         <code>java.net.URL</code>, and when JAXM is present
		'''         <code>javax.xml.messaging.URLEndpoint</code>
		''' </param>
		''' <returns> the <code>SOAPMessage</code> object that is the response to the
		'''         message that was sent </returns>
		''' <exception cref="SOAPException"> if there is a SOAP error </exception>
		Public MustOverride Function [call](ByVal request As SOAPMessage, ByVal [to] As Object) As SOAPMessage

		''' <summary>
		''' Gets a message from a specific endpoint and blocks until it receives,
		''' </summary>
		''' <param name="to"> an <code>Object</code> that identifies where
		'''                  the request should be sent. Objects of type
		'''                 <code>java.lang.String</code> and
		'''                 <code>java.net.URL</code> must be supported.
		''' </param>
		''' <returns> the <code>SOAPMessage</code> object that is the response to the
		'''                  get message request </returns>
		''' <exception cref="SOAPException"> if there is a SOAP error
		''' @since SAAJ 1.3 </exception>
		Public Overridable Function [get](ByVal [to] As Object) As SOAPMessage
			Throw New System.NotSupportedException("All subclasses of SOAPConnection must override get()")
		End Function

		''' <summary>
		''' Closes this <code>SOAPConnection</code> object.
		''' </summary>
		''' <exception cref="SOAPException"> if there is a SOAP error </exception>
		Public MustOverride Sub close()
	End Class

End Namespace