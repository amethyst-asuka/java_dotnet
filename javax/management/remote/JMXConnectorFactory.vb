Imports System
Imports System.Collections.Generic
Imports System.Threading

'
' * Copyright (c) 2002, 2013, Oracle and/or its affiliates. All rights reserved.
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
	''' <p>Factory to create JMX API connector clients.  There
	''' are no instances of this class.</p>
	''' 
	''' <p>Connections are usually made using the {@link
	''' #connect(JMXServiceURL) connect} method of this class.  More
	''' advanced applications can separate the creation of the connector
	''' client, using {@link #newJMXConnector(JMXServiceURL, Map)
	''' newJMXConnector} and the establishment of the connection itself, using
	''' <seealso cref="JMXConnector#connect(Map)"/>.</p>
	''' 
	''' <p>Each client is created by an instance of {@link
	''' JMXConnectorProvider}.  This instance is found as follows.  Suppose
	''' the given <seealso cref="JMXServiceURL"/> looks like
	''' <code>"service:jmx:<em>protocol</em>:<em>remainder</em>"</code>.
	''' Then the factory will attempt to find the appropriate {@link
	''' JMXConnectorProvider} for <code><em>protocol</em></code>.  Each
	''' occurrence of the character <code>+</code> or <code>-</code> in
	''' <code><em>protocol</em></code> is replaced by <code>.</code> or
	''' <code>_</code>, respectively.</p>
	''' 
	''' <p>A <em>provider package list</em> is searched for as follows:</p>
	''' 
	''' <ol>
	''' 
	''' <li>If the <code>environment</code> parameter to {@link
	''' #newJMXConnector(JMXServiceURL, Map) newJMXConnector} contains the
	''' key <code>jmx.remote.protocol.provider.pkgs</code> then the
	''' associated value is the provider package list.
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
	''' <code><em>pkg</em>.<em>protocol</em>.ClientProvider</code>
	''' </blockquote>
	''' 
	''' <p>If the <code>environment</code> parameter to {@link
	''' #newJMXConnector(JMXServiceURL, Map) newJMXConnector} contains the
	''' key <code>jmx.remote.protocol.provider.class.loader</code> then the
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
	''' JMXProviderException#getCause() <em>cause</em>} indicates the underlying
	''' exception, as follows:</p>
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
	''' interface is <code>JMXConnectorProvider</code>.</p>
	''' 
	''' <p>Every implementation must support the RMI connector protocol with
	''' the default RMI transport, specified with string <code>rmi</code>.
	''' An implementation may optionally support the RMI connector protocol
	''' with the RMI/IIOP transport, specified with the string
	''' <code>iiop</code>.</p>
	''' 
	''' <p>Once a provider is found, the result of the
	''' <code>newJMXConnector</code> method is the result of calling {@link
	''' JMXConnectorProvider#newJMXConnector(JMXServiceURL,Map) newJMXConnector}
	''' on the provider.</p>
	''' 
	''' <p>The <code>Map</code> parameter passed to the
	''' <code>JMXConnectorProvider</code> is a new read-only
	''' <code>Map</code> that contains all the entries that were in the
	''' <code>environment</code> parameter to {@link
	''' #newJMXConnector(JMXServiceURL,Map)
	''' JMXConnectorFactory.newJMXConnector}, if there was one.
	''' Additionally, if the
	''' <code>jmx.remote.protocol.provider.class.loader</code> key is not
	''' present in the <code>environment</code> parameter, it is added to
	''' the new read-only <code>Map</code>.  The associated value is the
	''' calling thread's context class loader.</p>
	''' 
	''' @since 1.5
	''' </summary>
	Public Class JMXConnectorFactory

		''' <summary>
		''' <p>Name of the attribute that specifies the default class
		''' loader. This class loader is used to deserialize return values and
		''' exceptions from remote <code>MBeanServerConnection</code>
		''' calls.  The value associated with this attribute is an instance
		''' of <seealso cref="ClassLoader"/>.</p>
		''' </summary>
		Public Const DEFAULT_CLASS_LOADER As String = "jmx.remote.default.class.loader"

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

		Private Shared ReadOnly logger As New com.sun.jmx.remote.util.ClassLogger("javax.management.remote.misc", "JMXConnectorFactory")

		''' <summary>
		''' There are no instances of this class. </summary>
		Private Sub New()
		End Sub

		''' <summary>
		''' <p>Creates a connection to the connector server at the given
		''' address.</p>
		''' 
		''' <p>This method is equivalent to {@link
		''' #connect(JMXServiceURL,Map) connect(serviceURL, null)}.</p>
		''' </summary>
		''' <param name="serviceURL"> the address of the connector server to
		''' connect to.
		''' </param>
		''' <returns> a <code>JMXConnector</code> whose {@link
		''' JMXConnector#connect connect} method has been called.
		''' </returns>
		''' <exception cref="NullPointerException"> if <code>serviceURL</code> is null.
		''' </exception>
		''' <exception cref="IOException"> if the connector client or the
		''' connection cannot be made because of a communication problem.
		''' </exception>
		''' <exception cref="SecurityException"> if the connection cannot be made
		''' for security reasons. </exception>
		Public Shared Function connect(ByVal serviceURL As JMXServiceURL) As JMXConnector
			Return connect(serviceURL, Nothing)
		End Function

		''' <summary>
		''' <p>Creates a connection to the connector server at the given
		''' address.</p>
		''' 
		''' <p>This method is equivalent to:</p>
		''' 
		''' <pre>
		''' JMXConnector conn = JMXConnectorFactory.newJMXConnector(serviceURL,
		'''                                                         environment);
		''' conn.connect(environment);
		''' </pre>
		''' </summary>
		''' <param name="serviceURL"> the address of the connector server to connect to.
		''' </param>
		''' <param name="environment"> a set of attributes to determine how the
		''' connection is made.  This parameter can be null.  Keys in this
		''' map must be Strings.  The appropriate type of each associated
		''' value depends on the attribute.  The contents of
		''' <code>environment</code> are not changed by this call.
		''' </param>
		''' <returns> a <code>JMXConnector</code> representing the newly-made
		''' connection.  Each successful call to this method produces a
		''' different object.
		''' </returns>
		''' <exception cref="NullPointerException"> if <code>serviceURL</code> is null.
		''' </exception>
		''' <exception cref="IOException"> if the connector client or the
		''' connection cannot be made because of a communication problem.
		''' </exception>
		''' <exception cref="SecurityException"> if the connection cannot be made
		''' for security reasons. </exception>
		Public Shared Function connect(Of T1)(ByVal serviceURL As JMXServiceURL, ByVal environment As IDictionary(Of T1)) As JMXConnector
			If serviceURL Is Nothing Then Throw New NullPointerException("Null JMXServiceURL")
			Dim conn As JMXConnector = newJMXConnector(serviceURL, environment)
			conn.connect(environment)
			Return conn
		End Function

		Private Shared Function newHashMap(Of K, V)() As IDictionary(Of K, V)
			Return New Dictionary(Of K, V)
		End Function

		Private Shared Function newHashMap(Of K, T1)(ByVal map As IDictionary(Of T1)) As IDictionary(Of K, Object)
			Return New Dictionary(Of K, Object)(map)
		End Function

		''' <summary>
		''' <p>Creates a connector client for the connector server at the
		''' given address.  The resultant client is not connected until its
		''' <seealso cref="JMXConnector#connect(Map) connect"/> method is called.</p>
		''' </summary>
		''' <param name="serviceURL"> the address of the connector server to connect to.
		''' </param>
		''' <param name="environment"> a set of attributes to determine how the
		''' connection is made.  This parameter can be null.  Keys in this
		''' map must be Strings.  The appropriate type of each associated
		''' value depends on the attribute.  The contents of
		''' <code>environment</code> are not changed by this call.
		''' </param>
		''' <returns> a <code>JMXConnector</code> representing the new
		''' connector client.  Each successful call to this method produces
		''' a different object.
		''' </returns>
		''' <exception cref="NullPointerException"> if <code>serviceURL</code> is null.
		''' </exception>
		''' <exception cref="IOException"> if the connector client cannot be made
		''' because of a communication problem.
		''' </exception>
		''' <exception cref="MalformedURLException"> if there is no provider for the
		''' protocol in <code>serviceURL</code>.
		''' </exception>
		''' <exception cref="JMXProviderException"> if there is a provider for the
		''' protocol in <code>serviceURL</code> but it cannot be used for
		''' some reason. </exception>
		Public Shared Function newJMXConnector(Of T1)(ByVal serviceURL As JMXServiceURL, ByVal environment As IDictionary(Of T1)) As JMXConnector

			Dim envcopy As IDictionary(Of String, Object)
			If environment Is Nothing Then
				envcopy = newHashMap()
			Else
				com.sun.jmx.remote.util.EnvHelp.checkAttributes(environment)
				envcopy = newHashMap(environment)
			End If

			Dim loader As ClassLoader = resolveClassLoader(envcopy)
			Dim targetInterface As Type = GetType(JMXConnectorProvider)
			Dim protocol As String = serviceURL.protocol
			Const providerClassName As String = "ClientProvider"
			Dim providerURL As JMXServiceURL = serviceURL

			Dim ___provider As JMXConnectorProvider = getProvider(providerURL, envcopy, providerClassName, targetInterface, loader)

			Dim exception As java.io.IOException = Nothing
			If ___provider Is Nothing Then
				' Loader is null when context class loader is set to null
				' and no loader has been provided in map.
				' com.sun.jmx.remote.util.Service class extracted from j2se
				' provider search algorithm doesn't handle well null classloader.
				If loader IsNot Nothing Then
					Try
						Dim connection As JMXConnector = getConnectorAsService(loader, providerURL, envcopy)
						If connection IsNot Nothing Then Return connection
					Catch e As JMXProviderException
						Throw e
					Catch e As java.io.IOException
						exception = e
					End Try
				End If
				___provider = getProvider(protocol, PROTOCOL_PROVIDER_DEFAULT_PACKAGE, GetType(JMXConnectorFactory).classLoader, providerClassName, targetInterface)
			End If

			If ___provider Is Nothing Then
				Dim e As New java.net.MalformedURLException("Unsupported protocol: " & protocol)
				If exception Is Nothing Then
					Throw e
				Else
					Throw com.sun.jmx.remote.util.EnvHelp.initCause(e, exception)
				End If
			End If

			Dim fixedenv As IDictionary(Of String, Object) = java.util.Collections.unmodifiableMap(envcopy)

			Return ___provider.newJMXConnector(serviceURL, fixedenv)
		End Function

		Private Shared Function resolvePkgs(Of T1)(ByVal env As IDictionary(Of T1)) As String

			Dim pkgsObject As Object = Nothing

			If env IsNot Nothing Then pkgsObject = env(PROTOCOL_PROVIDER_PACKAGES)

			If pkgsObject Is Nothing Then
'JAVA TO VB CONVERTER TODO TASK: Anonymous inner classes are not converted to VB if the base type is not defined in the code being converted:
'				pkgsObject = java.security.AccessController.doPrivileged(New java.security.PrivilegedAction<String>()
	'			{
	'					public String run()
	'					{
	'						Return System.getProperty(PROTOCOL_PROVIDER_PACKAGES);
	'					}
	'				});

			If pkgsObject Is Nothing Then Return Nothing
			End If

			If Not(TypeOf pkgsObject Is String) Then
				Dim msg As String = "Value of " & PROTOCOL_PROVIDER_PACKAGES & " parameter is not a String: " & pkgsObject.GetType().name
				Throw New JMXProviderException(msg)
			End If

			Dim pkgs As String = CStr(pkgsObject)
			If pkgs.Trim().Equals("") Then Return Nothing

			' pkgs may not contain an empty element
			If pkgs.StartsWith("|") OrElse pkgs.EndsWith("|") OrElse pkgs.IndexOf("||") >= 0 Then
				Dim msg As String = "Value of " & PROTOCOL_PROVIDER_PACKAGES & " contains an empty element: " & pkgs
				Throw New JMXProviderException(msg)
			End If

			Return pkgs
		End Function

		Friend Shared Function getProvider(Of T)(ByVal serviceURL As JMXServiceURL, ByVal environment As IDictionary(Of String, Object), ByVal providerClassName As String, ByVal targetInterface As Type, ByVal loader As ClassLoader) As T

			Dim protocol As String = serviceURL.protocol

			Dim pkgs As String = resolvePkgs(environment)

			Dim instance As T = Nothing

			If pkgs IsNot Nothing Then
				instance = getProvider(protocol, pkgs, loader, providerClassName, targetInterface)

				If instance IsNot Nothing Then
					Dim needsWrap As Boolean = (loader IsNot instance.GetType().classLoader)
					environment(PROTOCOL_PROVIDER_CLASS_LOADER) = If(needsWrap, wrap(loader), loader)
				End If
			End If

			Return instance
		End Function

		Friend Shared Function getProviderIterator(Of T)(ByVal providerClass As Type, ByVal loader As ClassLoader) As IEnumerator(Of T)
		   Dim serviceLoader As java.util.ServiceLoader(Of T) = java.util.ServiceLoader.load(providerClass, loader)
		   Return serviceLoader.GetEnumerator()
		End Function

		Private Shared Function wrap(ByVal parent As ClassLoader) As ClassLoader
'JAVA TO VB CONVERTER TODO TASK: Anonymous inner classes are not converted to VB if the base type is not defined in the code being converted:
'			Return parent != Nothing ? java.security.AccessController.doPrivileged(New java.security.PrivilegedAction<ClassLoader>()
	'		{
	'			@Override public ClassLoader run()
	'			{
	'				Return New ClassLoader(parent)
	'				{
	'					@Override protected Class loadClass(String name, boolean resolve) throws ClassNotFoundException
	'					{
	'						ReflectUtil.checkPackageAccess(name);
	'						Return MyBase.loadClass(name, resolve);
	'					}
	'				};
	'			}
	'		}) : Nothing;
		End Function

		Private Shared Function getConnectorAsService(Of T1)(ByVal loader As ClassLoader, ByVal url As JMXServiceURL, ByVal map As IDictionary(Of T1)) As JMXConnector

			Dim providers As IEnumerator(Of JMXConnectorProvider) = getProviderIterator(GetType(JMXConnectorProvider), loader)
			Dim connection As JMXConnector
			Dim exception As java.io.IOException = Nothing
			Do While providers.MoveNext()
				Dim ___provider As JMXConnectorProvider = providers.Current
				Try
					connection = ___provider.newJMXConnector(url, map)
					Return connection
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

		Friend Shared Function getProvider(Of T)(ByVal protocol As String, ByVal pkgs As String, ByVal loader As ClassLoader, ByVal providerClassName As String, ByVal targetInterface As Type) As T

			Dim tokenizer As New java.util.StringTokenizer(pkgs, "|")

			Do While tokenizer.hasMoreTokens()
				Dim pkg As String = tokenizer.nextToken()
				Dim className As String = (pkg & "." & protocol2package(protocol) & "." & providerClassName)
				Dim providerClass As Type
				Try
					providerClass = Type.GetType(className, True, loader)
				Catch e As ClassNotFoundException
					'Add trace.
					Continue Do
				End Try

				If Not targetInterface.IsAssignableFrom(providerClass) Then
					Dim msg As String = "Provider class does not implement " & targetInterface.name & ": " & providerClass.name
					Throw New JMXProviderException(msg)
				End If

				' We have just proved that this cast is correct
				Dim providerClassT As Type = com.sun.jmx.mbeanserver.Util.cast(providerClass)
				Try
					Return providerClassT.newInstance()
				Catch e As Exception
					Dim msg As String = "Exception when instantiating provider [" & className & "]"
					Throw New JMXProviderException(msg, e)
				End Try
			Loop

			Return Nothing
		End Function

		Friend Shared Function resolveClassLoader(Of T1)(ByVal environment As IDictionary(Of T1)) As ClassLoader
			Dim loader As ClassLoader = Nothing

			If environment IsNot Nothing Then
				Try
					loader = CType(environment(PROTOCOL_PROVIDER_CLASS_LOADER), ClassLoader)
				Catch e As ClassCastException
					Dim msg As String = "The ClassLoader supplied in the environment map using " & "the " & PROTOCOL_PROVIDER_CLASS_LOADER & " attribute is not an instance of java.lang.ClassLoader"
					Throw New System.ArgumentException(msg)
				End Try
			End If

			If loader Is Nothing Then loader = Thread.CurrentThread.contextClassLoader

			Return loader
		End Function

		Private Shared Function protocol2package(ByVal protocol As String) As String
			Return protocol.Replace("+"c, "."c).Replace("-"c, "_"c)
		End Function
	End Class

End Namespace