Imports System
Imports System.Collections.Generic

'
' * Copyright (c) 1999, 2013, Oracle and/or its affiliates. All rights reserved.
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
	''' A static class for creating SASL clients and servers.
	''' <p>
	''' This class defines the policy of how to locate, load, and instantiate
	''' SASL clients and servers.
	''' <p>
	''' For example, an application or library gets a SASL client by doing
	''' something like:
	''' <blockquote><pre>
	''' SaslClient sc = Sasl.createSaslClient(mechanisms,
	'''     authorizationId, protocol, serverName, props, callbackHandler);
	''' </pre></blockquote>
	''' It can then proceed to use the instance to create an authentication connection.
	''' <p>
	''' Similarly, a server gets a SASL server by using code that looks as follows:
	''' <blockquote><pre>
	''' SaslServer ss = Sasl.createSaslServer(mechanism,
	'''     protocol, serverName, props, callbackHandler);
	''' </pre></blockquote>
	''' 
	''' @since 1.5
	''' 
	''' @author Rosanna Lee
	''' @author Rob Weltman
	''' </summary>
	Public Class Sasl
		' Cannot create one of these
		Private Sub New()
		End Sub

		''' <summary>
		''' The name of a property that specifies the quality-of-protection to use.
		''' The property contains a comma-separated, ordered list
		''' of quality-of-protection values that the
		''' client or server is willing to support.  A qop value is one of
		''' <ul>
		''' <li>{@code "auth"} - authentication only</li>
		''' <li>{@code "auth-int"} - authentication plus integrity protection</li>
		''' <li>{@code "auth-conf"} - authentication plus integrity and confidentiality
		''' protection</li>
		''' </ul>
		''' 
		''' The order of the list specifies the preference order of the client or
		''' server. If this property is absent, the default qop is {@code "auth"}.
		''' The value of this constant is {@code "javax.security.sasl.qop"}.
		''' </summary>
		Public Const QOP As String = "javax.security.sasl.qop"

		''' <summary>
		''' The name of a property that specifies the cipher strength to use.
		''' The property contains a comma-separated, ordered list
		''' of cipher strength values that
		''' the client or server is willing to support. A strength value is one of
		''' <ul>
		''' <li>{@code "low"}</li>
		''' <li>{@code "medium"}</li>
		''' <li>{@code "high"}</li>
		''' </ul>
		''' The order of the list specifies the preference order of the client or
		''' server.  An implementation should allow configuration of the meaning
		''' of these values.  An application may use the Java Cryptography
		''' Extension (JCE) with JCE-aware mechanisms to control the selection of
		''' cipher suites that match the strength values.
		''' <BR>
		''' If this property is absent, the default strength is
		''' {@code "high,medium,low"}.
		''' The value of this constant is {@code "javax.security.sasl.strength"}.
		''' </summary>
		Public Const STRENGTH As String = "javax.security.sasl.strength"

		''' <summary>
		''' The name of a property that specifies whether the
		''' server must authenticate to the client. The property contains
		''' {@code "true"} if the server must
		''' authenticate the to client; {@code "false"} otherwise.
		''' The default is {@code "false"}.
		''' <br>The value of this constant is
		''' {@code "javax.security.sasl.server.authentication"}.
		''' </summary>
		Public Const SERVER_AUTH As String = "javax.security.sasl.server.authentication"

		''' <summary>
		''' The name of a property that specifies the bound server name for
		''' an unbound server. A server is created as an unbound server by setting
		''' the {@code serverName} argument in <seealso cref="#createSaslServer"/> as null.
		''' The property contains the bound host name after the authentication
		''' exchange has completed. It is only available on the server side.
		''' <br>The value of this constant is
		''' {@code "javax.security.sasl.bound.server.name"}.
		''' </summary>
		Public Const BOUND_SERVER_NAME As String = "javax.security.sasl.bound.server.name"

		''' <summary>
		''' The name of a property that specifies the maximum size of the receive
		''' buffer in bytes of {@code SaslClient}/{@code SaslServer}.
		''' The property contains the string representation of an integer.
		''' <br>If this property is absent, the default size
		''' is defined by the mechanism.
		''' <br>The value of this constant is {@code "javax.security.sasl.maxbuffer"}.
		''' </summary>
		Public Const MAX_BUFFER As String = "javax.security.sasl.maxbuffer"

		''' <summary>
		''' The name of a property that specifies the maximum size of the raw send
		''' buffer in bytes of {@code SaslClient}/{@code SaslServer}.
		''' The property contains the string representation of an integer.
		''' The value of this property is negotiated between the client and server
		''' during the authentication exchange.
		''' <br>The value of this constant is {@code "javax.security.sasl.rawsendsize"}.
		''' </summary>
		Public Const RAW_SEND_SIZE As String = "javax.security.sasl.rawsendsize"

		''' <summary>
		''' The name of a property that specifies whether to reuse previously
		''' authenticated session information. The property contains "true" if the
		''' mechanism implementation may attempt to reuse previously authenticated
		''' session information; it contains "false" if the implementation must
		''' not reuse previously authenticated session information.  A setting of
		''' "true" serves only as a hint: it does not necessarily entail actual
		''' reuse because reuse might not be possible due to a number of reasons,
		''' including, but not limited to, lack of mechanism support for reuse,
		''' expiration of reusable information, and the peer's refusal to support
		''' reuse.
		''' 
		''' The property's default value is "false".  The value of this constant
		''' is "javax.security.sasl.reuse".
		''' 
		''' Note that all other parameters and properties required to create a
		''' SASL client/server instance must be provided regardless of whether
		''' this property has been supplied. That is, you cannot supply any less
		''' information in anticipation of reuse.
		''' 
		''' Mechanism implementations that support reuse might allow customization
		''' of its implementation, for factors such as cache size, timeouts, and
		''' criteria for reusability. Such customizations are
		''' implementation-dependent.
		''' </summary>
		 Public Const REUSE As String = "javax.security.sasl.reuse"

		''' <summary>
		''' The name of a property that specifies
		''' whether mechanisms susceptible to simple plain passive attacks (e.g.,
		''' "PLAIN") are not permitted. The property
		''' contains {@code "true"} if such mechanisms are not permitted;
		''' {@code "false"} if such mechanisms are permitted.
		''' The default is {@code "false"}.
		''' <br>The value of this constant is
		''' {@code "javax.security.sasl.policy.noplaintext"}.
		''' </summary>
		Public Const POLICY_NOPLAINTEXT As String = "javax.security.sasl.policy.noplaintext"

		''' <summary>
		''' The name of a property that specifies whether
		''' mechanisms susceptible to active (non-dictionary) attacks
		''' are not permitted.
		''' The property contains {@code "true"}
		''' if mechanisms susceptible to active attacks
		''' are not permitted; {@code "false"} if such mechanisms are permitted.
		''' The default is {@code "false"}.
		''' <br>The value of this constant is
		''' {@code "javax.security.sasl.policy.noactive"}.
		''' </summary>
		Public Const POLICY_NOACTIVE As String = "javax.security.sasl.policy.noactive"

		''' <summary>
		''' The name of a property that specifies whether
		''' mechanisms susceptible to passive dictionary attacks are not permitted.
		''' The property contains {@code "true"}
		''' if mechanisms susceptible to dictionary attacks are not permitted;
		''' {@code "false"} if such mechanisms are permitted.
		''' The default is {@code "false"}.
		''' <br>
		''' The value of this constant is
		''' {@code "javax.security.sasl.policy.nodictionary"}.
		''' </summary>
		Public Const POLICY_NODICTIONARY As String = "javax.security.sasl.policy.nodictionary"

		''' <summary>
		''' The name of a property that specifies whether mechanisms that accept
		''' anonymous login are not permitted. The property contains {@code "true"}
		''' if mechanisms that accept anonymous login are not permitted;
		''' {@code "false"}
		''' if such mechanisms are permitted. The default is {@code "false"}.
		''' <br>
		''' The value of this constant is
		''' {@code "javax.security.sasl.policy.noanonymous"}.
		''' </summary>
		Public Const POLICY_NOANONYMOUS As String = "javax.security.sasl.policy.noanonymous"

		 ''' <summary>
		 ''' The name of a property that specifies whether mechanisms that implement
		 ''' forward secrecy between sessions are required. Forward secrecy
		 ''' means that breaking into one session will not automatically
		 ''' provide information for breaking into future sessions.
		 ''' The property
		 ''' contains {@code "true"} if mechanisms that implement forward secrecy
		 ''' between sessions are required; {@code "false"} if such mechanisms
		 ''' are not required. The default is {@code "false"}.
		 ''' <br>
		 ''' The value of this constant is
		 ''' {@code "javax.security.sasl.policy.forward"}.
		 ''' </summary>
		Public Const POLICY_FORWARD_SECRECY As String = "javax.security.sasl.policy.forward"

		''' <summary>
		''' The name of a property that specifies whether
		''' mechanisms that pass client credentials are required. The property
		''' contains {@code "true"} if mechanisms that pass
		''' client credentials are required; {@code "false"}
		''' if such mechanisms are not required. The default is {@code "false"}.
		''' <br>
		''' The value of this constant is
		''' {@code "javax.security.sasl.policy.credentials"}.
		''' </summary>
		Public Const POLICY_PASS_CREDENTIALS As String = "javax.security.sasl.policy.credentials"

		''' <summary>
		''' The name of a property that specifies the credentials to use.
		''' The property contains a mechanism-specific Java credential object.
		''' Mechanism implementations may examine the value of this property
		''' to determine whether it is a class that they support.
		''' The property may be used to supply credentials to a mechanism that
		''' supports delegated authentication.
		''' <br>
		''' The value of this constant is
		''' {@code "javax.security.sasl.credentials"}.
		''' </summary>
		Public Const CREDENTIALS As String = "javax.security.sasl.credentials"

		''' <summary>
		''' Creates a {@code SaslClient} using the parameters supplied.
		''' 
		''' This method uses the
		''' <a href="{@docRoot}/../technotes/guides/security/crypto/CryptoSpec.html#Provider">JCA Security Provider Framework</a>, described in the
		''' "Java Cryptography Architecture API Specification &amp; Reference", for
		''' locating and selecting a {@code SaslClient} implementation.
		''' 
		''' First, it
		''' obtains an ordered list of {@code SaslClientFactory} instances from
		''' the registered security providers for the "SaslClientFactory" service
		''' and the specified SASL mechanism(s). It then invokes
		''' {@code createSaslClient()} on each factory instance on the list
		''' until one produces a non-null {@code SaslClient} instance. It returns
		''' the non-null {@code SaslClient} instance, or null if the search fails
		''' to produce a non-null {@code SaslClient} instance.
		''' <p>
		''' A security provider for SaslClientFactory registers with the
		''' JCA Security Provider Framework keys of the form <br>
		''' {@code SaslClientFactory.}<em>{@code mechanism_name}</em>
		''' <br>
		''' and values that are class names of implementations of
		''' {@code javax.security.sasl.SaslClientFactory}.
		''' 
		''' For example, a provider that contains a factory class,
		''' {@code com.wiz.sasl.digest.ClientFactory}, that supports the
		''' "DIGEST-MD5" mechanism would register the following entry with the JCA:
		''' {@code SaslClientFactory.DIGEST-MD5 com.wiz.sasl.digest.ClientFactory}
		''' <p>
		''' See the
		''' "Java Cryptography Architecture API Specification &amp; Reference"
		''' for information about how to install and configure security service
		'''  providers.
		''' </summary>
		''' <param name="mechanisms"> The non-null list of mechanism names to try. Each is the
		''' IANA-registered name of a SASL mechanism. (e.g. "GSSAPI", "CRAM-MD5"). </param>
		''' <param name="authorizationId"> The possibly null protocol-dependent
		''' identification to be used for authorization.
		''' If null or empty, the server derives an authorization
		''' ID from the client's authentication credentials.
		''' When the SASL authentication completes successfully,
		''' the specified entity is granted access.
		''' </param>
		''' <param name="protocol"> The non-null string name of the protocol for which
		''' the authentication is being performed (e.g., "ldap").
		''' </param>
		''' <param name="serverName"> The non-null fully-qualified host name of the server
		''' to authenticate to.
		''' </param>
		''' <param name="props"> The possibly null set of properties used to
		''' select the SASL mechanism and to configure the authentication
		''' exchange of the selected mechanism.
		''' For example, if {@code props} contains the
		''' {@code Sasl.POLICY_NOPLAINTEXT} property with the value
		''' {@code "true"}, then the selected
		''' SASL mechanism must not be susceptible to simple plain passive attacks.
		''' In addition to the standard properties declared in this class,
		''' other, possibly mechanism-specific, properties can be included.
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
		''' <returns> A possibly null {@code SaslClient} created using the parameters
		''' supplied. If null, cannot find a {@code SaslClientFactory}
		''' that will produce one. </returns>
		''' <exception cref="SaslException"> If cannot create a {@code SaslClient} because
		''' of an error. </exception>
		Public Shared Function createSaslClient(Of T1)(ByVal mechanisms As String(), ByVal authorizationId As String, ByVal protocol As String, ByVal serverName As String, ByVal props As IDictionary(Of T1), ByVal cbh As javax.security.auth.callback.CallbackHandler) As SaslClient

			Dim mech As SaslClient = Nothing
			Dim fac As SaslClientFactory
			Dim className As String
			Dim mechName As String

			For i As Integer = 0 To mechanisms.Length - 1
				mechName=mechanisms(i)
				If mechName Is Nothing Then
					Throw New NullPointerException("Mechanism name cannot be null")
				ElseIf mechName.Length = 0 Then
					Continue For
				End If
				Dim mechFilter As String = "SaslClientFactory." & mechName
				Dim provs As java.security.Provider() = java.security.Security.getProviders(mechFilter)
				Dim j As Integer = 0
				Do While provs IsNot Nothing AndAlso j < provs.Length
					className = provs(j).getProperty(mechFilter)
					If className Is Nothing Then
						' Case is ignored
						j += 1
						Continue Do
					End If

					fac = CType(loadFactory(provs(j), className), SaslClientFactory)
					If fac IsNot Nothing Then
						mech = fac.createSaslClient(New String(){mechanisms(i)}, authorizationId, protocol, serverName, props, cbh)
						If mech IsNot Nothing Then Return mech
					End If
					j += 1
				Loop
			Next i

			Return Nothing
		End Function

		Private Shared Function loadFactory(ByVal p As java.security.Provider, ByVal className As String) As Object
			Try
	'            
	'             * Load the implementation class with the same class loader
	'             * that was used to load the provider.
	'             * In order to get the class loader of a class, the
	'             * caller's class loader must be the same as or an ancestor of
	'             * the class loader being returned. Otherwise, the caller must
	'             * have "getClassLoader" permission, or a SecurityException
	'             * will be thrown.
	'             
				Dim cl As ClassLoader = p.GetType().classLoader
				Dim implClass As Type
				implClass = Type.GetType(className, True, cl)
				Return implClass.newInstance()
			Catch e As ClassNotFoundException
				Throw New SaslException("Cannot load class " & className, e)
			Catch e As InstantiationException
				Throw New SaslException("Cannot instantiate class " & className, e)
			Catch e As IllegalAccessException
				Throw New SaslException("Cannot access class " & className, e)
			Catch e As SecurityException
				Throw New SaslException("Cannot access class " & className, e)
			End Try
		End Function


		''' <summary>
		''' Creates a {@code SaslServer} for the specified mechanism.
		''' 
		''' This method uses the
		''' <a href="{@docRoot}/../technotes/guides/security/crypto/CryptoSpec.html#Provider">JCA Security Provider Framework</a>,
		''' described in the
		''' "Java Cryptography Architecture API Specification &amp; Reference", for
		''' locating and selecting a {@code SaslServer} implementation.
		''' 
		''' First, it
		''' obtains an ordered list of {@code SaslServerFactory} instances from
		''' the registered security providers for the "SaslServerFactory" service
		''' and the specified mechanism. It then invokes
		''' {@code createSaslServer()} on each factory instance on the list
		''' until one produces a non-null {@code SaslServer} instance. It returns
		''' the non-null {@code SaslServer} instance, or null if the search fails
		''' to produce a non-null {@code SaslServer} instance.
		''' <p>
		''' A security provider for SaslServerFactory registers with the
		''' JCA Security Provider Framework keys of the form <br>
		''' {@code SaslServerFactory.}<em>{@code mechanism_name}</em>
		''' <br>
		''' and values that are class names of implementations of
		''' {@code javax.security.sasl.SaslServerFactory}.
		''' 
		''' For example, a provider that contains a factory class,
		''' {@code com.wiz.sasl.digest.ServerFactory}, that supports the
		''' "DIGEST-MD5" mechanism would register the following entry with the JCA:
		''' {@code SaslServerFactory.DIGEST-MD5  com.wiz.sasl.digest.ServerFactory}
		''' <p>
		''' See the
		''' "Java Cryptography Architecture API Specification &amp; Reference"
		''' for information about how to install and configure security
		''' service providers.
		''' </summary>
		''' <param name="mechanism"> The non-null mechanism name. It must be an
		''' IANA-registered name of a SASL mechanism. (e.g. "GSSAPI", "CRAM-MD5"). </param>
		''' <param name="protocol"> The non-null string name of the protocol for which
		''' the authentication is being performed (e.g., "ldap"). </param>
		''' <param name="serverName"> The fully qualified host name of the server, or null
		''' if the server is not bound to any specific host name. If the mechanism
		''' does not allow an unbound server, a {@code SaslException} will
		''' be thrown. </param>
		''' <param name="props"> The possibly null set of properties used to
		''' select the SASL mechanism and to configure the authentication
		''' exchange of the selected mechanism.
		''' For example, if {@code props} contains the
		''' {@code Sasl.POLICY_NOPLAINTEXT} property with the value
		''' {@code "true"}, then the selected
		''' SASL mechanism must not be susceptible to simple plain passive attacks.
		''' In addition to the standard properties declared in this class,
		''' other, possibly mechanism-specific, properties can be included.
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
		''' supplied. If null, cannot find a {@code SaslServerFactory}
		''' that will produce one. </returns>
		''' <exception cref="SaslException"> If cannot create a {@code SaslServer} because
		''' of an error.
		'''  </exception>
		Public Shared Function createSaslServer(Of T1)(ByVal mechanism As String, ByVal protocol As String, ByVal serverName As String, ByVal props As IDictionary(Of T1), ByVal cbh As javax.security.auth.callback.CallbackHandler) As SaslServer

			Dim mech As SaslServer = Nothing
			Dim fac As SaslServerFactory
			Dim className As String

			If mechanism Is Nothing Then
				Throw New NullPointerException("Mechanism name cannot be null")
			ElseIf mechanism.Length = 0 Then
				Return Nothing
			End If

			Dim mechFilter As String = "SaslServerFactory." & mechanism
			Dim provs As java.security.Provider() = java.security.Security.getProviders(mechFilter)
			Dim j As Integer = 0
			Do While provs IsNot Nothing AndAlso j < provs.Length
				className = provs(j).getProperty(mechFilter)
				If className Is Nothing Then Throw New SaslException("Provider does not support " & mechFilter)
				fac = CType(loadFactory(provs(j), className), SaslServerFactory)
				If fac IsNot Nothing Then
					mech = fac.createSaslServer(mechanism, protocol, serverName, props, cbh)
					If mech IsNot Nothing Then Return mech
				End If
				j += 1
			Loop

			Return Nothing
		End Function

		''' <summary>
		''' Gets an enumeration of known factories for producing {@code SaslClient}.
		''' This method uses the same algorithm for locating factories as
		''' {@code createSaslClient()}. </summary>
		''' <returns> A non-null enumeration of known factories for producing
		''' {@code SaslClient}. </returns>
		''' <seealso cref= #createSaslClient </seealso>
		Public Property Shared saslClientFactories As System.Collections.IEnumerator(Of SaslClientFactory)
			Get
				Dim facs As java.util.Set(Of Object) = getFactories("SaslClientFactory")
				Dim iter As IEnumerator(Of Object) = facs.GetEnumerator()
	'JAVA TO VB CONVERTER TODO TASK: Anonymous inner classes are not converted to VB if the base type is not defined in the code being converted:
	'			Return New java.util.Enumeration<SaslClientFactory>()
		'		{
		'			public boolean hasMoreElements()
		'			{
		'				Return iter.hasNext();
		'			}
		'			public SaslClientFactory nextElement()
		'			{
		'				Return (SaslClientFactory)iter.next();
		'			}
		'		};
			End Get
		End Property

		''' <summary>
		''' Gets an enumeration of known factories for producing {@code SaslServer}.
		''' This method uses the same algorithm for locating factories as
		''' {@code createSaslServer()}. </summary>
		''' <returns> A non-null enumeration of known factories for producing
		''' {@code SaslServer}. </returns>
		''' <seealso cref= #createSaslServer </seealso>
		Public Property Shared saslServerFactories As System.Collections.IEnumerator(Of SaslServerFactory)
			Get
				Dim facs As java.util.Set(Of Object) = getFactories("SaslServerFactory")
				Dim iter As IEnumerator(Of Object) = facs.GetEnumerator()
	'JAVA TO VB CONVERTER TODO TASK: Anonymous inner classes are not converted to VB if the base type is not defined in the code being converted:
	'			Return New java.util.Enumeration<SaslServerFactory>()
		'		{
		'			public boolean hasMoreElements()
		'			{
		'				Return iter.hasNext();
		'			}
		'			public SaslServerFactory nextElement()
		'			{
		'				Return (SaslServerFactory)iter.next();
		'			}
		'		};
			End Get
		End Property

		Private Shared Function getFactories(ByVal serviceName As String) As java.util.Set(Of Object)
			Dim result As New HashSet(Of Object)

			If (serviceName Is Nothing) OrElse (serviceName.Length = 0) OrElse (serviceName.EndsWith(".")) Then Return result


			Dim providers As java.security.Provider() = java.security.Security.providers
			Dim classes As New HashSet(Of String)
			Dim fac As Object

			For i As Integer = 0 To providers.Length - 1
				classes.Clear()

				' Check the keys for each provider.
				Dim e As System.Collections.IEnumerator(Of Object) = providers(i).keys()
'JAVA TO VB CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
				Do While e.hasMoreElements()
'JAVA TO VB CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
					Dim currentKey As String = CStr(e.nextElement())
					If currentKey.StartsWith(serviceName) Then
						' We should skip the currentKey if it contains a
						' whitespace. The reason is: such an entry in the
						' provider property contains attributes for the
						' implementation of an algorithm. We are only interested
						' in entries which lead to the implementation
						' classes.
						If currentKey.IndexOf(" ") < 0 Then
							Dim className As String = providers(i).getProperty(currentKey)
							If Not classes.Contains(className) Then
								classes.Add(className)
								Try
									fac = loadFactory(providers(i), className)
									If fac IsNot Nothing Then result.Add(fac)
								Catch ignore As Exception
								End Try
							End If
						End If
					End If
				Loop
			Next i
			Return java.util.Collections.unmodifiableSet(result)
		End Function
	End Class

End Namespace