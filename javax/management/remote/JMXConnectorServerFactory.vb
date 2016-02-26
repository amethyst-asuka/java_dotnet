Imports System
Imports System.Collections.Generic

'
' * Copyright (c) 2003, 2012, Oracle and/or its affiliates. All rights reserved.
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

Namespace javax.management.remote





	''' <summary>
	''' <p>Factory to create JMX API connector servers.  There
	''' are no instances of this class.</p>
	''' 
	''' <p>Each connector server is created by an instance of {@link
	''' JMXConnectorServerProvider}.  This instance is found as follows.  Suppose
	''' the given <seealso cref="JMXServiceURL"/> looks like
	''' <code>"service:jmx:<em>protocol</em>:<em>remainder</em>"</code>.
	''' Then the factory will attempt to find the appropriate {@link
	''' JMXConnectorServerProvider} for <code><em>protocol</em></code>.  Each
	''' occurrence of the character <code>+</code> or <code>-</code> in
	''' <code><em>protocol</em></code> is replaced by <code>.</code> or
	''' <code>_</code>, respectively.</p>
	''' 
	''' <p>A <em>provider package list</em> is searched for as follows:</p>
	''' 
	''' <ol>
	''' 
	''' <li>If the <code>environment</code> parameter to {@link
	''' #newJMXConnectorServer(JMXServiceURL,Map,MBeanServer)
	''' newJMXConnectorServer} contains the key
	''' <code>jmx.remote.protocol.provider.pkgs</code> then the associated
	''' value is the provider package list.
	''' 
	''' <li>Otherwise, if the system property
	''' <code>jmx.remote.protocol.provider.pkgs</code> exists, then its value
	''' is the provider package list.
	''' 
	''' <li>Otherwise, there is no provider package list.
	''' 
	''' </ol>
	''' 
	''' <p>The provider package list is a string that is interpreted as a
	''' list of non-empty Java package names separated by vertical bars
	''' (<code>|</code>).  If the string is empty, then so is the provider
	''' package list.  If the provider package list is not a String, or if
	''' it contains an element that is an empty string, a {@link
	''' JMXProviderException} is thrown.</p>
	''' 
	''' <p>If the provider package list exists and is not empty, then for
	''' each element <code><em>pkg</em></code> of the list, the factory
	''' will attempt to load the class
	''' 
	''' <blockquote>
	''' <code><em>pkg</em>.<em>protocol</em>.ServerProvider</code>
	''' </blockquote>
	''' 
	''' <p>If the <code>environment</code> parameter to {@link
	''' #newJMXConnectorServer(JMXServiceURL, Map, MBeanServer)
	''' newJMXConnectorServer} contains the key
	''' <code>jmx.remote.protocol.provider.class.loader</code> then the
	''' associated value is the class loader to use to load the provider.
	''' If the associated value is not an instance of {@link
	''' java.lang.ClassLoader}, an {@link
	''' java.lang.IllegalArgumentException} is thrown.</p>
	''' 
	''' <p>If the <code>jmx.remote.protocol.provider.class.loader</code>
	''' key is not present in the <code>environment</code> parameter, the
	''' calling thread's context class loader is used.</p>
	''' 
	''' <p>If the attempt to load this class produces a {@link
	''' ClassNotFoundException}, the search for a handler continues with
	''' the next element of the list.</p>
	''' 
	''' <p>Otherwise, a problem with the provider found is signalled by a
	''' <seealso cref="JMXProviderException"/> whose {@link
	''' JMXProviderException#getCause() <em>cause</em>} indicates the
	''' underlying exception, as follows:</p>
	''' 
	''' <ul>
	''' 
	''' <li>if the attempt to load the class produces an exception other
	''' than <code>ClassNotFoundException</code>, that is the
	''' <em>cause</em>;
	''' 
	''' <li>if <seealso cref="Class#newInstance()"/> for the class produces an
	''' exception, that is the <em>cause</em>.
	''' 
	''' </ul>
	''' 
	''' <p>If no provider is found by the above steps, including the
	''' default case where there is no provider package list, then the
	''' implementation will use its own provider for
	''' <code><em>protocol</em></code>, or it will throw a
	''' <code>MalformedURLException</code> if there is none.  An
	''' implementation may choose to find providers by other means.  For
	''' example, it may support the <a
	''' href="{@docRoot}/../technotes/guides/jar/jar.html#Service Provider">
	''' JAR conventions for service providers</a>, where the service
	''' interface is <code>JMXConnectorServerProvider</code>.</p>
	''' 
	''' <p>Every implementation must support the RMI connector protocol with
	''' the default RMI transport, specified with string <code>rmi</code>.
	''' An implementation may optionally support the RMI connector protocol
	''' with the RMI/IIOP transport, specified with the string
	''' <code>iiop</code>.</p>
	''' 
	''' <p>Once a provider is found, the result of the
	''' <code>newJMXConnectorServer</code> method is the result of calling
	''' {@link
	''' JMXConnectorServerProvider#newJMXConnectorServer(JMXServiceURL,
	''' Map, MBeanServer) newJMXConnectorServer} on the provider.</p>
	''' 
	''' <p>The <code>Map</code> parameter passed to the
	''' <code>JMXConnectorServerProvider</code> is a new read-only
	''' <code>Map</code> that contains all the entries that were in the
	''' <code>environment</code> parameter to {@link
	''' #newJMXConnectorServer(JMXServiceURL,Map,MBeanServer)
	''' JMXConnectorServerFactory.newJMXConnectorServer}, if there was one.
	''' Additionally, if the
	''' <code>jmx.remote.protocol.provider.class.loader</code> key is not
	''' present in the <code>environment</code> parameter, it is added to
	''' the new read-only <code>Map</code>. The associated value is the
	''' calling thread's context class loader.</p>
	''' 
	''' @since 1.5
	''' </summary>
	Public Class JMXConnectorServerFactory

		''' <summary>
		''' <p>Name of the attribute that specifies the default class
		''' loader.  This class loader is used to deserialize objects in
		''' requests received from the client, possibly after consulting an
		''' MBean-specific class loader.  The value associated with this
		''' attribute is an instance of <seealso cref="ClassLoader"/>.</p>
		''' </summary>
		Public Const DEFAULT_CLASS_LOADER As String = JMXConnectorFactory.DEFAULT_CLASS_LOADER

		''' <summary>
		''' <p>Name of the attribute that specifies the default class
		''' loader MBean name.  This class loader is used to deserialize objects in
		''' requests received from the client, possibly after consulting an
		''' MBean-specific class loader.  The value associated with this
		''' attribute is an instance of {@link javax.management.ObjectName
		''' ObjectName}.</p>
		''' </summary>
		Public Const DEFAULT_CLASS_LOADER_NAME As String = "jmx.remote.default.class.loader.name"

		''' <summary>
		''' <p>Name of the attribute that specifies the provider packages
		''' that are consulted when looking for the handler for a protocol.
		''' The value associated with this attribute is a string with
		''' package names separated by vertical bars (<code>|</code>).</p>
		''' </summary>
		Public Const PROTOCOL_PROVIDER_PACKAGES As String = "jmx.remote.protocol.provider.pkgs"

		''' <summary>
		''' <p>Name of the attribute that specifies the class
		''' loader for loading protocol providers.
		''' The value associated with this attribute is an instance
		''' of <seealso cref="ClassLoader"/>.</p>
		''' </summary>
		Public Const PROTOCOL_PROVIDER_CLASS_LOADER As String = "jmx.remote.protocol.provider.class.loader"

		Private Const PROTOCOL_PROVIDER_DEFAULT_PACKAGE As String = "com.sun.jmx.remote.protocol"

		Private Shared ReadOnly logger As New com.sun.jmx.remote.util.ClassLogger("javax.management.remote.misc","JMXConnectorServerFactory")

		''' <summary>
		''' There are no instances of this class. </summary>
		Private Sub New()
		End Sub

		Private Shared Function getConnectorServerAsService(Of T1)(ByVal loader As ClassLoader, ByVal url As JMXServiceURL, ByVal map As IDictionary(Of T1), ByVal mbs As javax.management.MBeanServer) As JMXConnectorServer
			Dim providers As IEnumerator(Of JMXConnectorServerProvider) = JMXConnectorFactory.getProviderIterator(GetType(JMXConnectorServerProvider), loader)

			Dim exception As java.io.IOException = Nothing
			Do While providers.MoveNext()
				Try
					Return providers.Current.newJMXConnectorServer(url, map, mbs)
				Catch e As JMXProviderException
					Throw e
				Catch e As Exception
					If logger.traceOn() Then logger.trace("getConnectorAsService", "URL[" & url & "] Service provider exception: " & e)
					If Not(TypeOf e Is java.net.MalformedURLException) Then
						If exception Is Nothing Then
							If TypeOf e Is java.io.IOException Then
								exception = CType(e, java.io.IOException)
							Else
								exception = com.sun.jmx.remote.util.EnvHelp.initCause(New java.io.IOException(e.Message), e)
							End If
						End If
					End If
					Continue Do
				End Try
			Loop
			If exception Is Nothing Then
				Return Nothing
			Else
				Throw exception
			End If
		End Function

		''' <summary>
		''' <p>Creates a connector server at the given address.  The
		''' resultant server is not started until its {@link
		''' JMXConnectorServer#start() start} method is called.</p>
		''' </summary>
		''' <param name="serviceURL"> the address of the new connector server.  The
		''' actual address of the new connector server, as returned by its
		''' <seealso cref="JMXConnectorServer#getAddress() getAddress"/> method, will
		''' not necessarily be exactly the same.  For example, it might
		''' include a port number if the original address did not.
		''' </param>
		''' <param name="environment"> a set of attributes to control the new
		''' connector server's behavior.  This parameter can be null.
		''' Keys in this map must be Strings.  The appropriate type of each
		''' associated value depends on the attribute.  The contents of
		''' <code>environment</code> are not changed by this call.
		''' </param>
		''' <param name="mbeanServer"> the MBean server that this connector server
		''' is attached to.  Null if this connector server will be attached
		''' to an MBean server by being registered in it.
		''' </param>
		''' <returns> a <code>JMXConnectorServer</code> representing the new
		''' connector server.  Each successful call to this method produces
		''' a different object.
		''' </returns>
		''' <exception cref="NullPointerException"> if <code>serviceURL</code> is null.
		''' </exception>
		''' <exception cref="IOException"> if the connector server cannot be made
		''' because of a communication problem.
		''' </exception>
		''' <exception cref="MalformedURLException"> if there is no provider for the
		''' protocol in <code>serviceURL</code>.
		''' </exception>
		''' <exception cref="JMXProviderException"> if there is a provider for the
		''' protocol in <code>serviceURL</code> but it cannot be used for
		''' some reason. </exception>
		Public Shared Function newJMXConnectorServer(Of T1)(ByVal serviceURL As JMXServiceURL, ByVal environment As IDictionary(Of T1), ByVal mbeanServer As javax.management.MBeanServer) As JMXConnectorServer
			Dim envcopy As IDictionary(Of String, Object)
			If environment Is Nothing Then
				envcopy = New Dictionary(Of String, Object)
			Else
				com.sun.jmx.remote.util.EnvHelp.checkAttributes(environment)
				envcopy = New Dictionary(Of String, Object)(environment)
			End If

			Dim targetInterface As Type = GetType(JMXConnectorServerProvider)
			Dim loader As ClassLoader = JMXConnectorFactory.resolveClassLoader(envcopy)
			Dim protocol As String = serviceURL.protocol
			Const providerClassName As String = "ServerProvider"

			Dim provider As JMXConnectorServerProvider = JMXConnectorFactory.getProvider(serviceURL, envcopy, providerClassName, targetInterface, loader)

			Dim exception As java.io.IOException = Nothing
			If provider Is Nothing Then
				' Loader is null when context class loader is set to null
				' and no loader has been provided in map.
				' com.sun.jmx.remote.util.Service class extracted from j2se
				' provider search algorithm doesn't handle well null classloader.
				If loader IsNot Nothing Then
					Try
						Dim connection As JMXConnectorServer = getConnectorServerAsService(loader, serviceURL, envcopy, mbeanServer)
						If connection IsNot Nothing Then Return connection
					Catch e As JMXProviderException
						Throw e
					Catch e As java.io.IOException
						exception = e
					End Try
				End If
				provider = JMXConnectorFactory.getProvider(protocol, PROTOCOL_PROVIDER_DEFAULT_PACKAGE, GetType(JMXConnectorFactory).classLoader, providerClassName, targetInterface)
			End If

			If provider Is Nothing Then
				Dim e As New java.net.MalformedURLException("Unsupported protocol: " & protocol)
				If exception Is Nothing Then
					Throw e
				Else
					Throw com.sun.jmx.remote.util.EnvHelp.initCause(e, exception)
				End If
			End If

			envcopy = java.util.Collections.unmodifiableMap(envcopy)

			Return provider.newJMXConnectorServer(serviceURL, envcopy, mbeanServer)
		End Function
	End Class

End Namespace