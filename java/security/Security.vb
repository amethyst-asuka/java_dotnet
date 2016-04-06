Imports System
Imports System.Runtime.CompilerServices
Imports System.Collections.Generic
Imports System.Collections.Concurrent
Imports sun.security.jca

'
' * Copyright (c) 1996, 2013, Oracle and/or its affiliates. All rights reserved.
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

Namespace java.security


	''' <summary>
	''' <p>This class centralizes all security properties and common security
	''' methods. One of its primary uses is to manage providers.
	''' 
	''' <p>The default values of security properties are read from an
	''' implementation-specific location, which is typically the properties file
	''' {@code lib/security/java.security} in the Java installation directory.
	''' 
	''' @author Benjamin Renaud
	''' </summary>

	Public NotInheritable Class Security

		' Are we debugging? -- for developers 
		Private Shared ReadOnly sdebug As sun.security.util.Debug = sun.security.util.Debug.getInstance("properties")

		' The java.security properties 
		Private Shared props As Properties

		' An element in the cache
		Private Class ProviderProperty
			Friend className As String
			Friend provider_Renamed As Provider
		End Class

		Shared Sub New()
			' doPrivileged here because there are multiple
			' things in initialize that might require privs.
			' (the FileInputStream call and the File.exists call,
			' the securityPropFile call, etc)
			AccessController.doPrivileged(New PrivilegedActionAnonymousInnerClassHelper(Of T)
		End Sub

		Private Class PrivilegedActionAnonymousInnerClassHelper(Of T)
			Implements PrivilegedAction(Of T)

			Public Overridable Function run() As Void
				initialize()
				Return Nothing
			End Function
		End Class

		Private Shared Sub initialize()
			props = New Properties
			Dim loadedProps As Boolean = False
			Dim overrideAll As Boolean = False

			' first load the system properties file
			' to determine the value of security.overridePropertiesFile
			Dim propFile As File = securityPropFile("java.security")
			If propFile.exists() Then
				Dim [is] As InputStream = Nothing
				Try
					Dim fis As New FileInputStream(propFile)
					[is] = New BufferedInputStream(fis)
					props.load([is])
					loadedProps = True

					If sdebug IsNot Nothing Then sdebug.println("reading security properties file: " & propFile)
				Catch e As IOException
					If sdebug IsNot Nothing Then
						sdebug.println("unable to load security properties from " & propFile)
						Console.WriteLine(e.ToString())
						Console.Write(e.StackTrace)
					End If
				Finally
					If [is] IsNot Nothing Then
						Try
							[is].close()
						Catch ioe As IOException
							If sdebug IsNot Nothing Then sdebug.println("unable to close input stream")
						End Try
					End If
				End Try
			End If

			If "true".equalsIgnoreCase(props.getProperty("security.overridePropertiesFile")) Then

				Dim extraPropFile As String = System.getProperty("java.security.properties")
				If extraPropFile IsNot Nothing AndAlso extraPropFile.StartsWith("=") Then
					overrideAll = True
					extraPropFile = extraPropFile.Substring(1)
				End If

				If overrideAll Then
					props = New Properties
					If sdebug IsNot Nothing Then sdebug.println("overriding other security properties files!")
				End If

				' now load the user-specified file so its values
				' will win if they conflict with the earlier values
				If extraPropFile IsNot Nothing Then
					Dim bis As BufferedInputStream = Nothing
					Try
						Dim propURL As java.net.URL

						extraPropFile = sun.security.util.PropertyExpander.expand(extraPropFile)
						propFile = New File(extraPropFile)
						If propFile.exists() Then
							propURL = New java.net.URL("file:" & propFile.canonicalPath)
						Else
							propURL = New java.net.URL(extraPropFile)
						End If
						bis = New BufferedInputStream(propURL.openStream())
						props.load(bis)
						loadedProps = True

						If sdebug IsNot Nothing Then
							sdebug.println("reading security properties file: " & propURL)
							If overrideAll Then sdebug.println("overriding other security properties files!")
						End If
					Catch e As Exception
						If sdebug IsNot Nothing Then
							sdebug.println("unable to load security properties from " & extraPropFile)
							e.printStackTrace()
						End If
					Finally
						If bis IsNot Nothing Then
							Try
								bis.close()
							Catch ioe As IOException
								If sdebug IsNot Nothing Then sdebug.println("unable to close input stream")
							End Try
						End If
					End Try
				End If
			End If

			If Not loadedProps Then
				initializeStatic()
				If sdebug IsNot Nothing Then sdebug.println("unable to load security properties " & "-- using defaults")
			End If

		End Sub

	'    
	'     * Initialize to default values, if <java.home>/lib/java.security
	'     * is not found.
	'     
		Private Shared Sub initializeStatic()
			props("security.provider.1") = "sun.security.provider.Sun"
			props("security.provider.2") = "sun.security.rsa.SunRsaSign"
			props("security.provider.3") = "com.sun.net.ssl.internal.ssl.Provider"
			props("security.provider.4") = "com.sun.crypto.provider.SunJCE"
			props("security.provider.5") = "sun.security.jgss.SunProvider"
			props("security.provider.6") = "com.sun.security.sasl.Provider"
		End Sub

		''' <summary>
		''' Don't let anyone instantiate this.
		''' </summary>
		Private Sub New()
		End Sub

		Private Shared Function securityPropFile(  filename As String) As File
			' maybe check for a system property which will specify where to
			' look. Someday.
			Dim sep As String = File.separator
			Return New File(System.getProperty("java.home") + sep & "lib" & sep & "security" & sep + filename)
		End Function

		''' <summary>
		''' Looks up providers, and returns the property (and its associated
		''' provider) mapping the key, if any.
		''' The order in which the providers are looked up is the
		''' provider-preference order, as specificed in the security
		''' properties file.
		''' </summary>
		Private Shared Function getProviderProperty(  key As String) As ProviderProperty
			Dim entry As ProviderProperty = Nothing

			Dim providers_Renamed As List(Of Provider) = Providers.providerList.providers()
			For i As Integer = 0 To providers_Renamed.size() - 1

				Dim matchKey As String = Nothing
				Dim prov As Provider = providers_Renamed.get(i)
				Dim prop As String = prov.getProperty(key)

				If prop Is Nothing Then
					' Is there a match if we do a case-insensitive property name
					' comparison? Let's try ...
					Dim e As Enumeration(Of Object) = prov.keys()
					Do While e.hasMoreElements() AndAlso prop Is Nothing
						matchKey = CStr(e.nextElement())
						If key.equalsIgnoreCase(matchKey) Then
							prop = prov.getProperty(matchKey)
							Exit Do
						End If
					Loop
				End If

				If prop IsNot Nothing Then
					Dim newEntry As New ProviderProperty
					newEntry.className = prop
					newEntry.provider_Renamed = prov
					Return newEntry
				End If
			Next i

			Return entry
		End Function

		''' <summary>
		''' Returns the property (if any) mapping the key for the given provider.
		''' </summary>
		Private Shared Function getProviderProperty(  key As String,   provider_Renamed As Provider) As String
			Dim prop As String = provider_Renamed.getProperty(key)
			If prop Is Nothing Then
				' Is there a match if we do a case-insensitive property name
				' comparison? Let's try ...
				Dim e As Enumeration(Of Object) = provider_Renamed.keys()
				Do While e.hasMoreElements() AndAlso prop Is Nothing
					Dim matchKey As String = CStr(e.nextElement())
					If key.equalsIgnoreCase(matchKey) Then
						prop = provider_Renamed.getProperty(matchKey)
						Exit Do
					End If
				Loop
			End If
			Return prop
		End Function

		''' <summary>
		''' Gets a specified property for an algorithm. The algorithm name
		''' should be a standard name. See the <a href=
		''' "{@docRoot}/../technotes/guides/security/StandardNames.html">
		''' Java Cryptography Architecture Standard Algorithm Name Documentation</a>
		''' for information about standard algorithm names.
		''' 
		''' One possible use is by specialized algorithm parsers, which may map
		''' classes to algorithms which they understand (much like Key parsers
		''' do).
		''' </summary>
		''' <param name="algName"> the algorithm name.
		''' </param>
		''' <param name="propName"> the name of the property to get.
		''' </param>
		''' <returns> the value of the specified property.
		''' </returns>
		''' @deprecated This method used to return the value of a proprietary
		''' property in the master file of the "SUN" Cryptographic Service
		''' Provider in order to determine how to parse algorithm-specific
		''' parameters. Use the new provider-based and algorithm-independent
		''' {@code AlgorithmParameters} and {@code KeyFactory} engine
		''' classes (introduced in the J2SE version 1.2 platform) instead. 
		<Obsolete("This method used to return the value of a proprietary")> _
		Public Shared Function getAlgorithmProperty(  algName As String,   propName As String) As String
			Dim entry As ProviderProperty = getProviderProperty("Alg." & propName & "." & algName)
			If entry IsNot Nothing Then
				Return entry.className
			Else
				Return Nothing
			End If
		End Function

		''' <summary>
		''' Adds a new provider, at a specified position. The position is
		''' the preference order in which providers are searched for
		''' requested algorithms.  The position is 1-based, that is,
		''' 1 is most preferred, followed by 2, and so on.
		''' 
		''' <p>If the given provider is installed at the requested position,
		''' the provider that used to be at that position, and all providers
		''' with a position greater than {@code position}, are shifted up
		''' one position (towards the end of the list of installed providers).
		''' 
		''' <p>A provider cannot be added if it is already installed.
		''' 
		''' <p>If there is a security manager, the
		''' <seealso cref="java.lang.SecurityManager#checkSecurityAccess"/> method is called
		''' with the {@code "insertProvider"} permission target name to see if
		''' it's ok to add a new provider. If this permission check is denied,
		''' {@code checkSecurityAccess} is called again with the
		''' {@code "insertProvider."+provider.getName()} permission target name. If
		''' both checks are denied, a {@code SecurityException} is thrown.
		''' </summary>
		''' <param name="provider"> the provider to be added.
		''' </param>
		''' <param name="position"> the preference position that the caller would
		''' like for this provider.
		''' </param>
		''' <returns> the actual preference position in which the provider was
		''' added, or -1 if the provider was not added because it is
		''' already installed.
		''' </returns>
		''' <exception cref="NullPointerException"> if provider is null </exception>
		''' <exception cref="SecurityException">
		'''          if a security manager exists and its {@link
		'''          java.lang.SecurityManager#checkSecurityAccess} method
		'''          denies access to add a new provider
		''' </exception>
		''' <seealso cref= #getProvider </seealso>
		''' <seealso cref= #removeProvider </seealso>
		''' <seealso cref= java.security.SecurityPermission </seealso>
		<MethodImpl(MethodImplOptions.Synchronized)> _
		Public Shared Function insertProviderAt(  provider_Renamed As Provider,   position As Integer) As Integer
			Dim providerName As String = provider_Renamed.name
			checkInsertProvider(providerName)
			Dim list As ProviderList = Providers.fullProviderList
			Dim newList As ProviderList = ProviderList.insertAt(list, provider_Renamed, position - 1)
			If list Is newList Then Return -1
			Providers.providerList = newList
			Return newList.getIndex(providerName) + 1
		End Function

		''' <summary>
		''' Adds a provider to the next position available.
		''' 
		''' <p>If there is a security manager, the
		''' <seealso cref="java.lang.SecurityManager#checkSecurityAccess"/> method is called
		''' with the {@code "insertProvider"} permission target name to see if
		''' it's ok to add a new provider. If this permission check is denied,
		''' {@code checkSecurityAccess} is called again with the
		''' {@code "insertProvider."+provider.getName()} permission target name. If
		''' both checks are denied, a {@code SecurityException} is thrown.
		''' </summary>
		''' <param name="provider"> the provider to be added.
		''' </param>
		''' <returns> the preference position in which the provider was
		''' added, or -1 if the provider was not added because it is
		''' already installed.
		''' </returns>
		''' <exception cref="NullPointerException"> if provider is null </exception>
		''' <exception cref="SecurityException">
		'''          if a security manager exists and its {@link
		'''          java.lang.SecurityManager#checkSecurityAccess} method
		'''          denies access to add a new provider
		''' </exception>
		''' <seealso cref= #getProvider </seealso>
		''' <seealso cref= #removeProvider </seealso>
		''' <seealso cref= java.security.SecurityPermission </seealso>
		Public Shared Function addProvider(  provider_Renamed As Provider) As Integer
	'        
	'         * We can't assign a position here because the statically
	'         * registered providers may not have been installed yet.
	'         * insertProviderAt() will fix that value after it has
	'         * loaded the static providers.
	'         
			Return insertProviderAt(provider_Renamed, 0)
		End Function

		''' <summary>
		''' Removes the provider with the specified name.
		''' 
		''' <p>When the specified provider is removed, all providers located
		''' at a position greater than where the specified provider was are shifted
		''' down one position (towards the head of the list of installed
		''' providers).
		''' 
		''' <p>This method returns silently if the provider is not installed or
		''' if name is null.
		''' 
		''' <p>First, if there is a security manager, its
		''' {@code checkSecurityAccess}
		''' method is called with the string {@code "removeProvider."+name}
		''' to see if it's ok to remove the provider.
		''' If the default implementation of {@code checkSecurityAccess}
		''' is used (i.e., that method is not overriden), then this will result in
		''' a call to the security manager's {@code checkPermission} method
		''' with a {@code SecurityPermission("removeProvider."+name)}
		''' permission.
		''' </summary>
		''' <param name="name"> the name of the provider to remove.
		''' </param>
		''' <exception cref="SecurityException">
		'''          if a security manager exists and its {@link
		'''          java.lang.SecurityManager#checkSecurityAccess} method
		'''          denies
		'''          access to remove the provider
		''' </exception>
		''' <seealso cref= #getProvider </seealso>
		''' <seealso cref= #addProvider </seealso>
		<MethodImpl(MethodImplOptions.Synchronized)> _
		Public Shared Sub removeProvider(  name As String)
			check("removeProvider." & name)
			Dim list As ProviderList = Providers.fullProviderList
			Dim newList As ProviderList = ProviderList.remove(list, name)
			Providers.providerList = newList
		End Sub

		''' <summary>
		''' Returns an array containing all the installed providers. The order of
		''' the providers in the array is their preference order.
		''' </summary>
		''' <returns> an array of all the installed providers. </returns>
		PublicShared ReadOnly Propertyproviders As Provider()
			Get
				Return Providers.fullProviderList.ToArray()
			End Get
		End Property

		''' <summary>
		''' Returns the provider installed with the specified name, if
		''' any. Returns null if no provider with the specified name is
		''' installed or if name is null.
		''' </summary>
		''' <param name="name"> the name of the provider to get.
		''' </param>
		''' <returns> the provider of the specified name.
		''' </returns>
		''' <seealso cref= #removeProvider </seealso>
		''' <seealso cref= #addProvider </seealso>
		Public Shared Function getProvider(  name As String) As Provider
			Return Providers.providerList.getProvider(name)
		End Function

		''' <summary>
		''' Returns an array containing all installed providers that satisfy the
		''' specified selection criterion, or null if no such providers have been
		''' installed. The returned providers are ordered
		''' according to their
		''' <seealso cref="#insertProviderAt(java.security.Provider, int) preference order"/>.
		''' 
		''' <p> A cryptographic service is always associated with a particular
		''' algorithm or type. For example, a digital signature service is
		''' always associated with a particular algorithm (e.g., DSA),
		''' and a CertificateFactory service is always associated with
		''' a particular certificate type (e.g., X.509).
		''' 
		''' <p>The selection criterion must be specified in one of the following two
		''' formats:
		''' <ul>
		''' <li> <i>{@literal <crypto_service>.<algorithm_or_type>}</i>
		''' <p> The cryptographic service name must not contain any dots.
		''' <p> A
		''' provider satisfies the specified selection criterion iff the provider
		''' implements the
		''' specified algorithm or type for the specified cryptographic service.
		''' <p> For example, "CertificateFactory.X.509"
		''' would be satisfied by any provider that supplied
		''' a CertificateFactory implementation for X.509 certificates.
		''' <li> <i>{@literal <crypto_service>.<algorithm_or_type>
		''' <attribute_name>:<attribute_value>}</i>
		''' <p> The cryptographic service name must not contain any dots. There
		''' must be one or more space characters between the
		''' <i>{@literal <algorithm_or_type>}</i> and the
		''' <i>{@literal <attribute_name>}</i>.
		'''  <p> A provider satisfies this selection criterion iff the
		''' provider implements the specified algorithm or type for the specified
		''' cryptographic service and its implementation meets the
		''' constraint expressed by the specified attribute name/value pair.
		''' <p> For example, "Signature.SHA1withDSA KeySize:1024" would be
		''' satisfied by any provider that implemented
		''' the SHA1withDSA signature algorithm with a keysize of 1024 (or larger).
		''' 
		''' </ul>
		''' 
		''' <p> See the <a href=
		''' "{@docRoot}/../technotes/guides/security/StandardNames.html">
		''' Java Cryptography Architecture Standard Algorithm Name Documentation</a>
		''' for information about standard cryptographic service names, standard
		''' algorithm names and standard attribute names.
		''' </summary>
		''' <param name="filter"> the criterion for selecting
		''' providers. The filter is case-insensitive.
		''' </param>
		''' <returns> all the installed providers that satisfy the selection
		''' criterion, or null if no such providers have been installed.
		''' </returns>
		''' <exception cref="InvalidParameterException">
		'''         if the filter is not in the required format </exception>
		''' <exception cref="NullPointerException"> if filter is null
		''' </exception>
		''' <seealso cref= #getProviders(java.util.Map)
		''' @since 1.3 </seealso>
		Public Shared Function getProviders(  filter As String) As Provider()
			Dim key As String = Nothing
			Dim value As String = Nothing
			Dim index As Integer = filter.IndexOf(":"c)

			If index = -1 Then
				key = filter
				value = ""
			Else
				key = filter.Substring(0, index)
				value = filter.Substring(index + 1)
			End If

			Dim hashtableFilter As New Dictionary(Of String, String)(1)
			hashtableFilter.put(key, value)

			Return (getProviders(hashtableFilter))
		End Function

		''' <summary>
		''' Returns an array containing all installed providers that satisfy the
		''' specified* selection criteria, or null if no such providers have been
		''' installed. The returned providers are ordered
		''' according to their
		''' {@link #insertProviderAt(java.security.Provider, int)
		''' preference order}.
		''' 
		''' <p>The selection criteria are represented by a map.
		''' Each map entry represents a selection criterion.
		''' A provider is selected iff it satisfies all selection
		''' criteria. The key for any entry in such a map must be in one of the
		''' following two formats:
		''' <ul>
		''' <li> <i>{@literal <crypto_service>.<algorithm_or_type>}</i>
		''' <p> The cryptographic service name must not contain any dots.
		''' <p> The value associated with the key must be an empty string.
		''' <p> A provider
		''' satisfies this selection criterion iff the provider implements the
		''' specified algorithm or type for the specified cryptographic service.
		''' <li>  <i>{@literal <crypto_service>}.
		''' {@literal <algorithm_or_type> <attribute_name>}</i>
		''' <p> The cryptographic service name must not contain any dots. There
		''' must be one or more space characters between the
		''' <i>{@literal <algorithm_or_type>}</i>
		''' and the <i>{@literal <attribute_name>}</i>.
		''' <p> The value associated with the key must be a non-empty string.
		''' A provider satisfies this selection criterion iff the
		''' provider implements the specified algorithm or type for the specified
		''' cryptographic service and its implementation meets the
		''' constraint expressed by the specified attribute name/value pair.
		''' </ul>
		''' 
		''' <p> See the <a href=
		''' "../../../technotes/guides/security/StandardNames.html">
		''' Java Cryptography Architecture Standard Algorithm Name Documentation</a>
		''' for information about standard cryptographic service names, standard
		''' algorithm names and standard attribute names.
		''' </summary>
		''' <param name="filter"> the criteria for selecting
		''' providers. The filter is case-insensitive.
		''' </param>
		''' <returns> all the installed providers that satisfy the selection
		''' criteria, or null if no such providers have been installed.
		''' </returns>
		''' <exception cref="InvalidParameterException">
		'''         if the filter is not in the required format </exception>
		''' <exception cref="NullPointerException"> if filter is null
		''' </exception>
		''' <seealso cref= #getProviders(java.lang.String)
		''' @since 1.3 </seealso>
		Public Shared Function getProviders(  filter As Map(Of String, String)) As Provider()
			' Get all installed providers first.
			' Then only return those providers who satisfy the selection criteria.
			Dim allProviders As Provider() = Security.providers
			Dim keySet As IDictionary(Of String, String).KeyCollection = filter.Keys
			Dim candidates As New LinkedHashSet(Of Provider)(5)

			' Returns all installed providers
			' if the selection criteria is null.
			If (keySet Is Nothing) OrElse (allProviders Is Nothing) Then Return allProviders

			Dim firstSearch As Boolean = True

			' For each selection criterion, remove providers
			' which don't satisfy the criterion from the candidate set.
			Dim ite As [Iterator](Of String) = keySet.GetEnumerator()
			Do While ite.MoveNext()
				Dim key As String = ite.Current
				Dim value As String = filter.get(key)

				Dim newCandidates As LinkedHashSet(Of Provider) = getAllQualifyingCandidates(key, value, allProviders)
				If firstSearch Then
					candidates = newCandidates
					firstSearch = False
				End If

				If (newCandidates IsNot Nothing) AndAlso newCandidates.Count > 0 Then
					' For each provider in the candidates set, if it
					' isn't in the newCandidate set, we should remove
					' it from the candidate set.
					Dim cansIte As [Iterator](Of Provider) = candidates.GetEnumerator()
					Do While cansIte.MoveNext()
						Dim prov As Provider = cansIte.Current
						If Not newCandidates.Contains(prov) Then cansIte.remove()
					Loop
				Else
					candidates = Nothing
					Exit Do
				End If
			Loop

			If (candidates Is Nothing) OrElse (candidates.Count = 0) Then Return Nothing

			Dim candidatesArray As Object() = candidates.ToArray()
			Dim result As Provider() = New Provider(candidatesArray.Length - 1){}

			For i As Integer = 0 To result.Length - 1
				result(i) = CType(candidatesArray(i), Provider)
			Next i

			Return result
		End Function

		' Map containing cached Spi Class objects of the specified type
		Private Shared ReadOnly spiMap As Map(Of String, [Class]) = New ConcurrentDictionary(Of String, [Class])

		''' <summary>
		''' Return the Class object for the given engine type
		''' (e.g. "MessageDigest"). Works for Spis in the java.security package
		''' only.
		''' </summary>
		Private Shared Function getSpiClass(  type As String) As  [Class]
			Dim clazz As  [Class] = spiMap.get(type)
			If clazz IsNot Nothing Then Return clazz
			Try
				clazz = Type.GetType("java.security." & type & "Spi")
				spiMap.put(type, clazz)
				Return clazz
			Catch e As  ClassNotFoundException
				Throw New AssertionError("Spi class not found", e)
			End Try
		End Function

	'    
	'     * Returns an array of objects: the first object in the array is
	'     * an instance of an implementation of the requested algorithm
	'     * and type, and the second object in the array identifies the provider
	'     * of that implementation.
	'     * The {@code provider} argument can be null, in which case all
	'     * configured providers will be searched in order of preference.
	'     
		Friend Shared Function getImpl(  algorithm As String,   type As String,   provider_Renamed As String) As Object()
			If provider_Renamed Is Nothing Then
				Return GetInstance.getInstance(type, getSpiClass(type), algorithm).ToArray()
			Else
				Return GetInstance.getInstance(type, getSpiClass(type), algorithm, provider_Renamed).ToArray()
			End If
		End Function

		Friend Shared Function getImpl(  algorithm As String,   type As String,   provider_Renamed As String,   params As Object) As Object()
			If provider_Renamed Is Nothing Then
				Return GetInstance.getInstance(type, getSpiClass(type), algorithm, params).ToArray()
			Else
				Return GetInstance.getInstance(type, getSpiClass(type), algorithm, params, provider_Renamed).ToArray()
			End If
		End Function

	'    
	'     * Returns an array of objects: the first object in the array is
	'     * an instance of an implementation of the requested algorithm
	'     * and type, and the second object in the array identifies the provider
	'     * of that implementation.
	'     * The {@code provider} argument cannot be null.
	'     
		Friend Shared Function getImpl(  algorithm As String,   type As String,   provider_Renamed As Provider) As Object()
			Return GetInstance.getInstance(type, getSpiClass(type), algorithm, provider_Renamed).ToArray()
		End Function

		Friend Shared Function getImpl(  algorithm As String,   type As String,   provider_Renamed As Provider,   params As Object) As Object()
			Return GetInstance.getInstance(type, getSpiClass(type), algorithm, params, provider_Renamed).ToArray()
		End Function

		''' <summary>
		''' Gets a security property value.
		''' 
		''' <p>First, if there is a security manager, its
		''' {@code checkPermission}  method is called with a
		''' {@code java.security.SecurityPermission("getProperty."+key)}
		''' permission to see if it's ok to retrieve the specified
		''' security property value..
		''' </summary>
		''' <param name="key"> the key of the property being retrieved.
		''' </param>
		''' <returns> the value of the security property corresponding to key.
		''' </returns>
		''' <exception cref="SecurityException">
		'''          if a security manager exists and its {@link
		'''          java.lang.SecurityManager#checkPermission} method
		'''          denies
		'''          access to retrieve the specified security property value </exception>
		''' <exception cref="NullPointerException"> is key is null
		''' </exception>
		''' <seealso cref= #setProperty </seealso>
		''' <seealso cref= java.security.SecurityPermission </seealso>
		Public Shared Function getProperty(  key As String) As String
			Dim sm As SecurityManager = System.securityManager
			If sm IsNot Nothing Then sm.checkPermission(New SecurityPermission("getProperty." & key))
			Dim name As String = props.getProperty(key)
			If name IsNot Nothing Then name = name.Trim() ' could be a class name with trailing ws
			Return name
		End Function

		''' <summary>
		''' Sets a security property value.
		''' 
		''' <p>First, if there is a security manager, its
		''' {@code checkPermission} method is called with a
		''' {@code java.security.SecurityPermission("setProperty."+key)}
		''' permission to see if it's ok to set the specified
		''' security property value.
		''' </summary>
		''' <param name="key"> the name of the property to be set.
		''' </param>
		''' <param name="datum"> the value of the property to be set.
		''' </param>
		''' <exception cref="SecurityException">
		'''          if a security manager exists and its {@link
		'''          java.lang.SecurityManager#checkPermission} method
		'''          denies access to set the specified security property value </exception>
		''' <exception cref="NullPointerException"> if key or datum is null
		''' </exception>
		''' <seealso cref= #getProperty </seealso>
		''' <seealso cref= java.security.SecurityPermission </seealso>
		Public Shared Sub setProperty(  key As String,   datum As String)
			check("setProperty." & key)
			props(key) = datum
			invalidateSMCache(key) ' See below.
		End Sub

	'    
	'     * Implementation detail:  If the property we just set in
	'     * setProperty() was either "package.access" or
	'     * "package.definition", we need to signal to the SecurityManager
	'     * class that the value has just changed, and that it should
	'     * invalidate it's local cache values.
	'     *
	'     * Rather than create a new API entry for this function,
	'     * we use reflection to set a private variable.
	'     
		Private Shared Sub invalidateSMCache(  key As String)

			Dim pa As Boolean = key.Equals("package.access")
			Dim pd As Boolean = key.Equals("package.definition")

			If pa OrElse pd Then
				AccessController.doPrivileged(New PrivilegedActionAnonymousInnerClassHelper2(Of T)
			End If ' if
		End Sub

		Private Class PrivilegedActionAnonymousInnerClassHelper2(Of T)
			Implements PrivilegedAction(Of T)

			Public Overridable Function run() As Void
				Try
					' Get the class via the bootstrap class loader. 
					Dim cl As  [Class] = Type.GetType("java.lang.SecurityManager", False, Nothing)
					Dim f As Field = Nothing
					Dim accessible As Boolean = False

					If pa Then
						f = cl.getDeclaredField("packageAccessValid")
						accessible = f.accessible
						f.accessible = True
					Else
						f = cl.getDeclaredField("packageDefinitionValid")
						accessible = f.accessible
						f.accessible = True
					End If
					f.booleanean(f, False)
					f.accessible = accessible
				Catch e1 As Exception
		'                         If we couldn't get the [Class], it hasn't
		'                         * been loaded yet.  If there is no such
		'                         * field, we shouldn't try to set it.  There
		'                         * shouldn't be a security execption, as we
		'                         * are loaded by boot class loader, and we
		'                         * are inside a doPrivileged() here.
		'                         *
		'                         * NOOP: don't do anything...
		'                         
				End Try
				Return Nothing
			End Function ' run
		End Class

		Private Shared Sub check(  directive As String)
			Dim security_Renamed As SecurityManager = System.securityManager
			If security_Renamed IsNot Nothing Then security_Renamed.checkSecurityAccess(directive)
		End Sub

		Private Shared Sub checkInsertProvider(  name As String)
			Dim security_Renamed As SecurityManager = System.securityManager
			If security_Renamed IsNot Nothing Then
				Try
					security_Renamed.checkSecurityAccess("insertProvider")
				Catch se1 As SecurityException
					Try
						security_Renamed.checkSecurityAccess("insertProvider." & name)
					Catch se2 As SecurityException
						' throw first exception, but add second to suppressed
						se1.addSuppressed(se2)
						Throw se1
					End Try
				End Try
			End If
		End Sub

	'    
	'    * Returns all providers who satisfy the specified
	'    * criterion.
	'    
		Private Shared Function getAllQualifyingCandidates(  filterKey As String,   filterValue As String,   allProviders As Provider()) As LinkedHashSet(Of Provider)
			Dim filterComponents_Renamed As String() = getFilterComponents(filterKey, filterValue)

			' The first component is the service name.
			' The second is the algorithm name.
			' If the third isn't null, that is the attrinute name.
			Dim serviceName As String = filterComponents_Renamed(0)
			Dim algName As String = filterComponents_Renamed(1)
			Dim attrName As String = filterComponents_Renamed(2)

			Return getProvidersNotUsingCache(serviceName, algName, attrName, filterValue, allProviders)
		End Function

		Private Shared Function getProvidersNotUsingCache(  serviceName As String,   algName As String,   attrName As String,   filterValue As String,   allProviders As Provider()) As LinkedHashSet(Of Provider)
			Dim candidates As New LinkedHashSet(Of Provider)(5)
			For i As Integer = 0 To allProviders.Length - 1
				If isCriterionSatisfied(allProviders(i), serviceName, algName, attrName, filterValue) Then candidates.Add(allProviders(i))
			Next i
			Return candidates
		End Function

	'    
	'     * Returns true if the given provider satisfies
	'     * the selection criterion key:value.
	'     
		Private Shared Function isCriterionSatisfied(  prov As Provider,   serviceName As String,   algName As String,   attrName As String,   filterValue As String) As Boolean
			Dim key As String = serviceName + AscW("."c) + algName

			If attrName IsNot Nothing Then key += AscW(" "c) + attrName
			' Check whether the provider has a property
			' whose key is the same as the given key.
			Dim propValue As String = getProviderProperty(key, prov)

			If propValue Is Nothing Then
				' Check whether we have an alias instead
				' of a standard name in the key.
				Dim standardName As String = getProviderProperty("Alg.Alias." & serviceName & "." & algName, prov)
				If standardName IsNot Nothing Then
					key = serviceName & "." & standardName

					If attrName IsNot Nothing Then key += AscW(" "c) + attrName

					propValue = getProviderProperty(key, prov)
				End If

				If propValue Is Nothing Then Return False
			End If

			' If the key is in the format of:
			' <crypto_service>.<algorithm_or_type>,
			' there is no need to check the value.

			If attrName Is Nothing Then Return True

			' If we get here, the key must be in the
			' format of <crypto_service>.<algorithm_or_provider> <attribute_name>.
			If isStandardAttr(attrName) Then
				Return isConstraintSatisfied(attrName, filterValue, propValue)
			Else
				Return filterValue.equalsIgnoreCase(propValue)
			End If
		End Function

	'    
	'     * Returns true if the attribute is a standard attribute;
	'     * otherwise, returns false.
	'     
		Private Shared Function isStandardAttr(  attribute As String) As Boolean
			' For now, we just have two standard attributes:
			' KeySize and ImplementedIn.
			If attribute.equalsIgnoreCase("KeySize") Then Return True

			If attribute.equalsIgnoreCase("ImplementedIn") Then Return True

			Return False
		End Function

	'    
	'     * Returns true if the requested attribute value is supported;
	'     * otherwise, returns false.
	'     
		Private Shared Function isConstraintSatisfied(  attribute As String,   value As String,   prop As String) As Boolean
			' For KeySize, prop is the max key size the
			' provider supports for a specific <crypto_service>.<algorithm>.
			If attribute.equalsIgnoreCase("KeySize") Then
				Dim requestedSize As Integer = Convert.ToInt32(value)
				Dim maxSize As Integer = Convert.ToInt32(prop)
				If requestedSize <= maxSize Then
					Return True
				Else
					Return False
				End If
			End If

			' For Type, prop is the type of the implementation
			' for a specific <crypto service>.<algorithm>.
			If attribute.equalsIgnoreCase("ImplementedIn") Then Return value.equalsIgnoreCase(prop)

			Return False
		End Function

		Friend Shared Function getFilterComponents(  filterKey As String,   filterValue As String) As String()
			Dim algIndex As Integer = filterKey.IndexOf("."c)

			If algIndex < 0 Then Throw New InvalidParameterException("Invalid filter")

			Dim serviceName As String = filterKey.Substring(0, algIndex)
			Dim algName As String = Nothing
			Dim attrName As String = Nothing

			If filterValue.length() = 0 Then
				' The filterValue is an empty string. So the filterKey
				' should be in the format of <crypto_service>.<algorithm_or_type>.
				algName = filterKey.Substring(algIndex + 1).Trim()
				If algName.length() = 0 Then Throw New InvalidParameterException("Invalid filter")
			Else
				' The filterValue is a non-empty string. So the filterKey must be
				' in the format of
				' <crypto_service>.<algorithm_or_type> <attribute_name>
				Dim attrIndex As Integer = filterKey.IndexOf(" "c)

				If attrIndex = -1 Then
					' There is no attribute name in the filter.
					Throw New InvalidParameterException("Invalid filter")
				Else
					attrName = filterKey.Substring(attrIndex + 1).Trim()
					If attrName.length() = 0 Then Throw New InvalidParameterException("Invalid filter")
				End If

				' There must be an algorithm name in the filter.
				If (attrIndex < algIndex) OrElse (algIndex = attrIndex - 1) Then
					Throw New InvalidParameterException("Invalid filter")
				Else
					algName = filterKey.Substring(algIndex + 1, attrIndex - (algIndex + 1))
				End If
			End If

			Dim result As String() = New String(2){}
			result(0) = serviceName
			result(1) = algName
			result(2) = attrName

			Return result
		End Function

		''' <summary>
		''' Returns a Set of Strings containing the names of all available
		''' algorithms or types for the specified Java cryptographic service
		''' (e.g., Signature, MessageDigest, Cipher, Mac, KeyStore). Returns
		''' an empty Set if there is no provider that supports the
		''' specified service or if serviceName is null. For a complete list
		''' of Java cryptographic services, please see the
		''' <a href="../../../technotes/guides/security/crypto/CryptoSpec.html">Java
		''' Cryptography Architecture API Specification &amp; Reference</a>.
		''' Note: the returned set is immutable.
		''' </summary>
		''' <param name="serviceName"> the name of the Java cryptographic
		''' service (e.g., Signature, MessageDigest, Cipher, Mac, KeyStore).
		''' Note: this parameter is case-insensitive.
		''' </param>
		''' <returns> a Set of Strings containing the names of all available
		''' algorithms or types for the specified Java cryptographic service
		''' or an empty set if no provider supports the specified service.
		''' 
		''' @since 1.4
		'''  </returns>
		Public Shared Function getAlgorithms(  serviceName As String) As [Set](Of String)

			If (serviceName Is Nothing) OrElse (serviceName.length() = 0) OrElse (serviceName.EndsWith(".")) Then Return Collections.emptySet()

			Dim result As New HashSet(Of String)
			Dim providers_Renamed As Provider() = Security.providers

			For i As Integer = 0 To providers_Renamed.Length - 1
				' Check the keys for each provider.
				Dim e As Enumeration(Of Object) = providers_Renamed(i).keys()
				Do While e.hasMoreElements()
					Dim currentKey As String = CStr(e.nextElement()).ToUpper(Locale.ENGLISH)
					If currentKey.StartsWith(serviceName.ToUpper(Locale.ENGLISH)) Then
						' We should skip the currentKey if it contains a
						' whitespace. The reason is: such an entry in the
						' provider property contains attributes for the
						' implementation of an algorithm. We are only interested
						' in entries which lead to the implementation
						' classes.
						If currentKey.IndexOf(" ") < 0 Then result.add(currentKey.Substring(serviceName.length() + 1))
					End If
				Loop
			Next i
			Return Collections.unmodifiableSet(result)
		End Function
	End Class

End Namespace