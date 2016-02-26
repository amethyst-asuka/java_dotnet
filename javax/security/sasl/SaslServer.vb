'
' * Copyright (c) 2000, 2013, Oracle and/or its affiliates. All rights reserved.
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

Namespace javax.security.sasl

	''' <summary>
	''' Performs SASL authentication as a server.
	''' <p>
	''' A server such an LDAP server gets an instance of this
	''' class in order to perform authentication defined by a specific SASL
	''' mechanism. Invoking methods on the {@code SaslServer} instance
	''' generates challenges according to the SASL
	''' mechanism implemented by the {@code SaslServer}.
	''' As the authentication proceeds, the instance
	''' encapsulates the state of a SASL server's authentication exchange.
	''' <p>
	''' Here's an example of how an LDAP server might use a {@code SaslServer}.
	''' It first gets an instance of a {@code SaslServer} for the SASL mechanism
	''' requested by the client:
	''' <blockquote><pre>
	''' SaslServer ss = Sasl.createSaslServer(mechanism,
	'''     "ldap", myFQDN, props, callbackHandler);
	''' </pre></blockquote>
	''' It can then proceed to use the server for authentication.
	''' For example, suppose the LDAP server received an LDAP BIND request
	''' containing the name of the SASL mechanism and an (optional) initial
	''' response. It then might use the server as follows:
	''' <blockquote><pre>{@code
	''' while (!ss.isComplete()) {
	'''     try {
	'''         byte[] challenge = ss.evaluateResponse(response);
	'''         if (ss.isComplete()) {
	'''             status = ldap.sendBindResponse(mechanism, challenge, SUCCESS);
	'''         } else {
	'''             status = ldap.sendBindResponse(mechanism, challenge,
	'''                   SASL_BIND_IN_PROGRESS);
	'''             response = ldap.readBindRequest();
	'''         }
	'''     } catch (SaslException e) {
	'''          status = ldap.sendErrorResponse(e);
	'''          break;
	'''     }
	''' }
	''' if (ss.isComplete() && status == SUCCESS) {
	'''    String qop = (String) sc.getNegotiatedProperty(Sasl.QOP);
	'''    if (qop != null
	'''        && (qop.equalsIgnoreCase("auth-int")
	'''            || qop.equalsIgnoreCase("auth-conf"))) {
	''' 
	'''      // Use SaslServer.wrap() and SaslServer.unwrap() for future
	'''      // communication with client
	'''      ldap.in = new SecureInputStream(ss, ldap.in);
	'''      ldap.out = new SecureOutputStream(ss, ldap.out);
	'''    }
	''' }
	''' }</pre></blockquote>
	''' 
	''' @since 1.5
	''' </summary>
	''' <seealso cref= Sasl </seealso>
	''' <seealso cref= SaslServerFactory
	''' 
	''' @author Rosanna Lee
	''' @author Rob Weltman </seealso>
	Public abstract Interface SaslServer

		''' <summary>
		''' Returns the IANA-registered mechanism name of this SASL server.
		''' (e.g. "CRAM-MD5", "GSSAPI"). </summary>
		''' <returns> A non-null string representing the IANA-registered mechanism name. </returns>
		ReadOnly Property mechanismName As String

		''' <summary>
		''' Evaluates the response data and generates a challenge.
		''' 
		''' If a response is received from the client during the authentication
		''' process, this method is called to prepare an appropriate next
		''' challenge to submit to the client. The challenge is null if the
		''' authentication has succeeded and no more challenge data is to be sent
		''' to the client. It is non-null if the authentication must be continued
		''' by sending a challenge to the client, or if the authentication has
		''' succeeded but challenge data needs to be processed by the client.
		''' {@code isComplete()} should be called
		''' after each call to {@code evaluateResponse()},to determine if any further
		''' response is needed from the client.
		''' </summary>
		''' <param name="response"> The non-null (but possibly empty) response sent
		''' by the client.
		''' </param>
		''' <returns> The possibly null challenge to send to the client.
		''' It is null if the authentication has succeeded and there is
		''' no more challenge data to be sent to the client. </returns>
		''' <exception cref="SaslException"> If an error occurred while processing
		''' the response or generating a challenge. </exception>
		Function evaluateResponse(ByVal response As SByte()) As SByte()

		''' <summary>
		''' Determines whether the authentication exchange has completed.
		''' This method is typically called after each invocation of
		''' {@code evaluateResponse()} to determine whether the
		''' authentication has completed successfully or should be continued. </summary>
		''' <returns> true if the authentication exchange has completed; false otherwise. </returns>
		ReadOnly Property complete As Boolean

		''' <summary>
		''' Reports the authorization ID in effect for the client of this
		''' session.
		''' This method can only be called if isComplete() returns true. </summary>
		''' <returns> The authorization ID of the client. </returns>
		''' <exception cref="IllegalStateException"> if this authentication session has not completed </exception>
		ReadOnly Property authorizationID As String

		''' <summary>
		''' Unwraps a byte array received from the client.
		''' This method can be called only after the authentication exchange has
		''' completed (i.e., when {@code isComplete()} returns true) and only if
		''' the authentication exchange has negotiated integrity and/or privacy
		''' as the quality of protection; otherwise,
		''' an {@code IllegalStateException} is thrown.
		''' <p>
		''' {@code incoming} is the contents of the SASL buffer as defined in RFC 2222
		''' without the leading four octet field that represents the length.
		''' {@code offset} and {@code len} specify the portion of {@code incoming}
		''' to use.
		''' </summary>
		''' <param name="incoming"> A non-null byte array containing the encoded bytes
		'''                from the client. </param>
		''' <param name="offset"> The starting position at {@code incoming} of the bytes to use. </param>
		''' <param name="len"> The number of bytes from {@code incoming} to use. </param>
		''' <returns> A non-null byte array containing the decoded bytes. </returns>
		''' <exception cref="SaslException"> if {@code incoming} cannot be successfully
		''' unwrapped. </exception>
		''' <exception cref="IllegalStateException"> if the authentication exchange has
		''' not completed, or if the negotiated quality of protection
		''' has neither integrity nor privacy </exception>
		Function unwrap(ByVal incoming As SByte(), ByVal offset As Integer, ByVal len As Integer) As SByte()

		''' <summary>
		''' Wraps a byte array to be sent to the client.
		''' This method can be called only after the authentication exchange has
		''' completed (i.e., when {@code isComplete()} returns true) and only if
		''' the authentication exchange has negotiated integrity and/or privacy
		''' as the quality of protection; otherwise, a {@code SaslException} is thrown.
		''' <p>
		''' The result of this method
		''' will make up the contents of the SASL buffer as defined in RFC 2222
		''' without the leading four octet field that represents the length.
		''' {@code offset} and {@code len} specify the portion of {@code outgoing}
		''' to use.
		''' </summary>
		''' <param name="outgoing"> A non-null byte array containing the bytes to encode. </param>
		''' <param name="offset"> The starting position at {@code outgoing} of the bytes to use. </param>
		''' <param name="len"> The number of bytes from {@code outgoing} to use. </param>
		''' <returns> A non-null byte array containing the encoded bytes. </returns>
		''' <exception cref="SaslException"> if {@code outgoing} cannot be successfully
		''' wrapped. </exception>
		''' <exception cref="IllegalStateException"> if the authentication exchange has
		''' not completed, or if the negotiated quality of protection has
		''' neither integrity nor privacy. </exception>
		Function wrap(ByVal outgoing As SByte(), ByVal offset As Integer, ByVal len As Integer) As SByte()

		''' <summary>
		''' Retrieves the negotiated property.
		''' This method can be called only after the authentication exchange has
		''' completed (i.e., when {@code isComplete()} returns true); otherwise, an
		''' {@code IllegalStateException} is thrown.
		''' </summary>
		''' <param name="propName"> the property </param>
		''' <returns> The value of the negotiated property. If null, the property was
		''' not negotiated or is not applicable to this mechanism. </returns>
		''' <exception cref="IllegalStateException"> if this authentication exchange has not completed </exception>

		Function getNegotiatedProperty(ByVal propName As String) As Object

		 ''' <summary>
		 ''' Disposes of any system resources or security-sensitive information
		 ''' the SaslServer might be using. Invoking this method invalidates
		 ''' the SaslServer instance. This method is idempotent. </summary>
		 ''' <exception cref="SaslException"> If a problem was encountered while disposing
		 ''' the resources. </exception>
		Sub dispose()
	End Interface

End Namespace