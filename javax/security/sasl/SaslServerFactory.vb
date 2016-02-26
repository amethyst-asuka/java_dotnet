Imports System.Collections.Generic

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
	''' An interface for creating instances of {@code SaslServer}.
	''' A class that implements this interface
	''' must be thread-safe and handle multiple simultaneous
	''' requests. It must also have a public constructor that accepts no
	''' argument.
	''' <p>
	''' This interface is not normally accessed directly by a server, which will use the
	''' {@code Sasl} static methods
	''' instead. However, a particular environment may provide and install a
	''' new or different {@code SaslServerFactory}.
	''' 
	''' @since 1.5
	''' </summary>
	''' <seealso cref= SaslServer </seealso>
	''' <seealso cref= Sasl
	''' 
	''' @author Rosanna Lee
	''' @author Rob Weltman </seealso>
	Public abstract Interface SaslServerFactory
		''' <summary>
		''' Creates a {@code SaslServer} using the parameters supplied.
		''' It returns null
		''' if no {@code SaslServer} can be created using the parameters supplied.
		''' Throws {@code SaslException} if it cannot create a {@code SaslServer}
		''' because of an error.
		''' </summary>
		''' <param name="mechanism"> The non-null
		''' IANA-registered name of a SASL mechanism. (e.g. "GSSAPI", "CRAM-MD5"). </param>
		''' <param name="protocol"> The non-null string name of the protocol for which
		''' the authentication is being performed (e.g., "ldap"). </param>
		''' <param name="serverName"> The fully qualified host name of the server to
		''' authenticate to, or null if the server is not bound to any specific host
		''' name. If the mechanism does not allow an unbound server, a
		''' {@code SaslException} will be thrown. </param>
		''' <param name="props"> The possibly null set of properties used to select the SASL
		''' mechanism and to configure the authentication exchange of the selected
		''' mechanism. See the {@code Sasl} class for a list of standard properties.
		''' Other, possibly mechanism-specific, properties can be included.
		''' Properties not relevant to the selected mechanism are ignored,
		''' including any map entries with non-String keys.
		''' </param>
		''' <param name="cbh"> The possibly null callback handler to used by the SASL
		''' mechanisms to get further information from the application/library
		''' to complete the authentication. For example, a SASL mechanism might
		''' require the authentication ID, password and realm from the caller.
		''' The authentication ID is requested by using a {@code NameCallback}.
		''' The password is requested by using a {@code PasswordCallback}.
		''' The realm is requested by using a {@code RealmChoiceCallback} if there is a list
		''' of realms to choose from, and by using a {@code RealmCallback} if
		''' the realm must be entered.
		''' </param>
		''' <returns> A possibly null {@code SaslServer} created using the parameters
		''' supplied. If null, this factory cannot produce a {@code SaslServer}
		''' using the parameters supplied. </returns>
		''' <exception cref="SaslException"> If cannot create a {@code SaslServer} because
		''' of an error. </exception>
		Function createSaslServer(Of T1)(ByVal mechanism As String, ByVal protocol As String, ByVal serverName As String, ByVal props As IDictionary(Of T1), ByVal cbh As javax.security.auth.callback.CallbackHandler) As SaslServer

		''' <summary>
		''' Returns an array of names of mechanisms that match the specified
		''' mechanism selection policies. </summary>
		''' <param name="props"> The possibly null set of properties used to specify the
		''' security policy of the SASL mechanisms. For example, if {@code props}
		''' contains the {@code Sasl.POLICY_NOPLAINTEXT} property with the value
		''' {@code "true"}, then the factory must not return any SASL mechanisms
		''' that are susceptible to simple plain passive attacks.
		''' See the {@code Sasl} class for a complete list of policy properties.
		''' Non-policy related properties, if present in {@code props}, are ignored,
		''' including any map entries with non-String keys. </param>
		''' <returns> A non-null array containing a IANA-registered SASL mechanism names. </returns>
		Function getMechanismNames(Of T1)(ByVal props As IDictionary(Of T1)) As String()
	End Interface

End Namespace