Imports Microsoft.VisualBasic
Imports System
Imports System.Runtime.CompilerServices
Imports System.Collections.Generic
Imports System.Text
import static com.sun.jmx.defaults.JmxProperties.MLET_LIB_DIR
import static com.sun.jmx.defaults.JmxProperties.MLET_LOGGER

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

Namespace javax.management.loading

	' Java import






	''' <summary>
	''' Allows you to instantiate and register one or several MBeans in the MBean server
	''' coming from a remote URL. M-let is a shortcut for management applet. The m-let service does this
	''' by loading an m-let text file, which specifies information on the MBeans to be obtained.
	''' The information on each MBean is specified in a single instance of a tag, called the MLET tag.
	''' The location of the m-let text file is specified by a URL.
	''' <p>
	''' The <CODE>MLET</CODE> tag has the following syntax:
	''' <p>
	''' &lt;<CODE>MLET</CODE><BR>
	'''      <CODE>CODE = </CODE><VAR>class</VAR><CODE> | OBJECT = </CODE><VAR>serfile</VAR><BR>
	'''      <CODE>ARCHIVE = &quot;</CODE><VAR>archiveList</VAR><CODE>&quot;</CODE><BR>
	'''      <CODE>[CODEBASE = </CODE><VAR>codebaseURL</VAR><CODE>]</CODE><BR>
	'''      <CODE>[NAME = </CODE><VAR>mbeanname</VAR><CODE>]</CODE><BR>
	'''      <CODE>[VERSION = </CODE><VAR>version</VAR><CODE>]</CODE><BR>
	''' &gt;<BR>
	'''      <CODE>[</CODE><VAR>arglist</VAR><CODE>]</CODE><BR>
	''' &lt;<CODE>/MLET</CODE>&gt;
	''' <p>
	''' where:
	''' <DL>
	''' <DT><CODE>CODE = </CODE><VAR>class</VAR></DT>
	''' <DD>
	''' This attribute specifies the full Java class name, including package name, of the MBean to be obtained.
	''' The compiled <CODE>.class</CODE> file of the MBean must be contained in one of the <CODE>.jar</CODE> files specified by the <CODE>ARCHIVE</CODE>
	''' attribute. Either <CODE>CODE</CODE> or <CODE>OBJECT</CODE> must be present.
	''' </DD>
	''' <DT><CODE>OBJECT = </CODE><VAR>serfile</VAR></DT>
	''' <DD>
	''' This attribute specifies the <CODE>.ser</CODE> file that contains a serialized representation of the MBean to be obtained.
	''' This file must be contained in one of the <CODE>.jar</CODE> files specified by the <CODE>ARCHIVE</CODE> attribute. If the <CODE>.jar</CODE> file contains a directory hierarchy, specify the path of the file within this hierarchy. Otherwise  a match will not be found. Either <CODE>CODE</CODE> or <CODE>OBJECT</CODE> must be present.
	''' </DD>
	''' <DT><CODE>ARCHIVE = &quot;</CODE><VAR>archiveList</VAR><CODE>&quot;</CODE></DT>
	''' <DD>
	''' This mandatory attribute specifies one or more <CODE>.jar</CODE> files
	''' containing MBeans or other resources used by
	''' the MBean to be obtained. One of the <CODE>.jar</CODE> files must contain the file specified by the <CODE>CODE</CODE> or <CODE>OBJECT</CODE> attribute.
	''' If archivelist contains more than one file:
	''' <UL>
	''' <LI>Each file must be separated from the one that follows it by a comma (,).
	''' <LI><VAR>archivelist</VAR> must be enclosed in double quote marks.
	''' </UL>
	''' All <CODE>.jar</CODE> files in <VAR>archivelist</VAR> must be stored in the directory specified by the code base URL.
	''' </DD>
	''' <DT><CODE>CODEBASE = </CODE><VAR>codebaseURL</VAR></DT>
	''' <DD>
	''' This optional attribute specifies the code base URL of the MBean to be obtained. It identifies the directory that contains
	''' the <CODE>.jar</CODE> files specified by the <CODE>ARCHIVE</CODE> attribute. Specify this attribute only if the <CODE>.jar</CODE> files are not in the same
	''' directory as the m-let text file. If this attribute is not specified, the base URL of the m-let text file is used.
	''' </DD>
	''' <DT><CODE>NAME = </CODE><VAR>mbeanname</VAR></DT>
	''' <DD>
	''' This optional attribute specifies the object name to be assigned to the
	''' MBean instance when the m-let service registers it. If
	''' <VAR>mbeanname</VAR> starts with the colon character (:), the domain
	''' part of the object name is the default domain of the MBean server,
	''' as returned by <seealso cref="javax.management.MBeanServer#getDefaultDomain()"/>.
	''' </DD>
	''' <DT><CODE>VERSION = </CODE><VAR>version</VAR></DT>
	''' <DD>
	''' This optional attribute specifies the version number of the MBean and
	''' associated <CODE>.jar</CODE> files to be obtained. This version number can
	''' be used to specify that the <CODE>.jar</CODE> files are loaded from the
	''' server to update those stored locally in the cache the next time the m-let
	''' text file is loaded. <VAR>version</VAR> must be a series of non-negative
	''' decimal integers each separated by a period from the one that precedes it.
	''' </DD>
	''' <DT><VAR>arglist</VAR></DT>
	''' <DD>
	''' This optional attribute specifies a list of one or more parameters for the
	''' MBean to be instantiated. This list describes the parameters to be passed the MBean's constructor.
	''' Use the following syntax to specify each item in
	''' <VAR>arglist</VAR>:
	''' <DL>
	''' <DT>&lt;<CODE>ARG TYPE=</CODE><VAR>argumentType</VAR> <CODE>VALUE=</CODE><VAR>value</VAR>&gt;</DT>
	''' <DD>where:
	''' <UL>
	''' <LI><VAR>argumentType</VAR> is the type of the argument that will be passed as parameter to the MBean's constructor.</UL>
	''' </DD>
	''' </DL>
	''' <P>The arguments' type in the argument list should be a Java primitive type or a Java basic type
	''' (<CODE>java.lang.Boolean, java.lang.Byte, java.lang.Short, java.lang.Long, java.lang.Integer, java.lang.Float, java.lang.Double, java.lang.String</CODE>).
	''' </DD>
	''' </DL>
	''' 
	''' When an m-let text file is loaded, an
	''' instance of each MBean specified in the file is created and registered.
	''' <P>
	''' The m-let service extends the <CODE>java.net.URLClassLoader</CODE> and can be used to load remote classes
	''' and jar files in the VM of the agent.
	''' <p><STRONG>Note - </STRONG> The <CODE>MLet</CODE> class loader uses the <seealso cref="javax.management.MBeanServerFactory#getClassLoaderRepository(javax.management.MBeanServer)"/>
	''' to load classes that could not be found in the loaded jar files.
	''' 
	''' @since 1.5
	''' </summary>
	Public Class MLet
		Inherits java.net.URLClassLoader
		Implements MLetMBean, javax.management.MBeanRegistration, java.io.Externalizable

		 Private Const serialVersionUID As Long = 3636148327800330130L

	'     
	'     * ------------------------------------------
	'     *   PRIVATE VARIABLES
	'     * ------------------------------------------
	'     

		 ''' <summary>
		 ''' The reference to the MBean server.
		 ''' @serial
		 ''' </summary>
		 Private server As javax.management.MBeanServer = Nothing


		 ''' <summary>
		 ''' The list of instances of the <CODE>MLetContent</CODE>
		 ''' class found at the specified URL.
		 ''' @serial
		 ''' </summary>
		 Private mletList As IList(Of MLetContent) = New List(Of MLetContent)


		 ''' <summary>
		 ''' The directory used for storing libraries locally before they are loaded.
		 ''' </summary>
		 Private libraryDirectory As String


		 ''' <summary>
		 ''' The object name of the MLet Service.
		 ''' @serial
		 ''' </summary>
		 Private mletObjectName As javax.management.ObjectName = Nothing

		 ''' <summary>
		 ''' The URLs of the MLet Service.
		 ''' @serial
		 ''' </summary>
		 Private myUrls As java.net.URL() = Nothing

		 ''' <summary>
		 ''' What ClassLoaderRepository, if any, to use if this MLet
		 ''' doesn't find an asked-for class.
		 ''' </summary>
		 <NonSerialized> _
		 Private currentClr As ClassLoaderRepository

		 ''' <summary>
		 ''' True if we should consult the <seealso cref="ClassLoaderRepository"/>
		 ''' when we do not find a class ourselves.
		 ''' </summary>
		 <NonSerialized> _
		 Private delegateToCLR As Boolean

		 ''' <summary>
		 ''' objects maps from primitive classes to primitive object classes.
		 ''' </summary>
		 Private primitiveClasses As IDictionary(Of String, Type) = New Dictionary(Of String, Type)(8)
	'	 {
	'		 primitiveClasses.put(Boolean.TYPE.toString(), Boolean.class);
	'		 primitiveClasses.put(Character.TYPE.toString(), Character.class);
	'		 primitiveClasses.put(Byte.TYPE.toString(), Byte.class);
	'		 primitiveClasses.put(Short.TYPE.toString(), Short.class);
	'		 primitiveClasses.put(Integer.TYPE.toString(), Integer.class);
	'		 primitiveClasses.put(Long.TYPE.toString(), Long.class);
	'		 primitiveClasses.put(Float.TYPE.toString(), Float.class);
	'		 primitiveClasses.put(Double.TYPE.toString(), Double.class);
	'
	'	 }


	'     
	'      * ------------------------------------------
	'      *  CONSTRUCTORS
	'      * ------------------------------------------
	'      

	'     
	'      * The constructor stuff would be considerably simplified if our
	'      * parent, URLClassLoader, specified that its one- and
	'      * two-argument constructors were equivalent to its
	'      * three-argument constructor with trailing null arguments.  But
	'      * it doesn't, which prevents us from having all the constructors
	'      * but one call this(...args...).
	'      

		 ''' <summary>
		 ''' Constructs a new MLet using the default delegation parent ClassLoader.
		 ''' </summary>
		 Public Sub New()
			 Me.New(New java.net.URL(){})
		 End Sub

		 ''' <summary>
		 ''' Constructs a new MLet for the specified URLs using the default
		 ''' delegation parent ClassLoader.  The URLs will be searched in
		 ''' the order specified for classes and resources after first
		 ''' searching in the parent class loader.
		 ''' </summary>
		 ''' <param name="urls">  The URLs from which to load classes and resources.
		 '''  </param>
		 Public Sub New(ByVal urls As java.net.URL())
			 Me.New(urls, True)
		 End Sub

		 ''' <summary>
		 ''' Constructs a new MLet for the given URLs. The URLs will be
		 ''' searched in the order specified for classes and resources
		 ''' after first searching in the specified parent class loader.
		 ''' The parent argument will be used as the parent class loader
		 ''' for delegation.
		 ''' </summary>
		 ''' <param name="urls">  The URLs from which to load classes and resources. </param>
		 ''' <param name="parent"> The parent class loader for delegation.
		 '''  </param>
		 Public Sub New(ByVal urls As java.net.URL(), ByVal parent As ClassLoader)
			 Me.New(urls, parent, True)
		 End Sub

		 ''' <summary>
		 ''' Constructs a new MLet for the specified URLs, parent class
		 ''' loader, and URLStreamHandlerFactory. The parent argument will
		 ''' be used as the parent class loader for delegation. The factory
		 ''' argument will be used as the stream handler factory to obtain
		 ''' protocol handlers when creating new URLs.
		 ''' </summary>
		 ''' <param name="urls">  The URLs from which to load classes and resources. </param>
		 ''' <param name="parent"> The parent class loader for delegation. </param>
		 ''' <param name="factory">  The URLStreamHandlerFactory to use when creating URLs.
		 '''  </param>
		 Public Sub New(ByVal urls As java.net.URL(), ByVal parent As ClassLoader, ByVal factory As java.net.URLStreamHandlerFactory)
			 Me.New(urls, parent, factory, True)
		 End Sub

		 ''' <summary>
		 ''' Constructs a new MLet for the specified URLs using the default
		 ''' delegation parent ClassLoader.  The URLs will be searched in
		 ''' the order specified for classes and resources after first
		 ''' searching in the parent class loader.
		 ''' </summary>
		 ''' <param name="urls">  The URLs from which to load classes and resources. </param>
		 ''' <param name="delegateToCLR">  True if, when a class is not found in
		 ''' either the parent ClassLoader or the URLs, the MLet should delegate
		 ''' to its containing MBeanServer's <seealso cref="ClassLoaderRepository"/>.
		 '''  </param>
		 Public Sub New(ByVal urls As java.net.URL(), ByVal delegateToCLR As Boolean)
			 MyBase.New(urls)
			 init(delegateToCLR)
		 End Sub

		 ''' <summary>
		 ''' Constructs a new MLet for the given URLs. The URLs will be
		 ''' searched in the order specified for classes and resources
		 ''' after first searching in the specified parent class loader.
		 ''' The parent argument will be used as the parent class loader
		 ''' for delegation.
		 ''' </summary>
		 ''' <param name="urls">  The URLs from which to load classes and resources. </param>
		 ''' <param name="parent"> The parent class loader for delegation. </param>
		 ''' <param name="delegateToCLR">  True if, when a class is not found in
		 ''' either the parent ClassLoader or the URLs, the MLet should delegate
		 ''' to its containing MBeanServer's <seealso cref="ClassLoaderRepository"/>.
		 '''  </param>
		 Public Sub New(ByVal urls As java.net.URL(), ByVal parent As ClassLoader, ByVal delegateToCLR As Boolean)
			 MyBase.New(urls, parent)
			 init(delegateToCLR)
		 End Sub

		 ''' <summary>
		 ''' Constructs a new MLet for the specified URLs, parent class
		 ''' loader, and URLStreamHandlerFactory. The parent argument will
		 ''' be used as the parent class loader for delegation. The factory
		 ''' argument will be used as the stream handler factory to obtain
		 ''' protocol handlers when creating new URLs.
		 ''' </summary>
		 ''' <param name="urls">  The URLs from which to load classes and resources. </param>
		 ''' <param name="parent"> The parent class loader for delegation. </param>
		 ''' <param name="factory">  The URLStreamHandlerFactory to use when creating URLs. </param>
		 ''' <param name="delegateToCLR">  True if, when a class is not found in
		 ''' either the parent ClassLoader or the URLs, the MLet should delegate
		 ''' to its containing MBeanServer's <seealso cref="ClassLoaderRepository"/>.
		 '''  </param>
		 Public Sub New(ByVal urls As java.net.URL(), ByVal parent As ClassLoader, ByVal factory As java.net.URLStreamHandlerFactory, ByVal delegateToCLR As Boolean)
			 MyBase.New(urls, parent, factory)
			 init(delegateToCLR)
		 End Sub

		 Private Sub init(ByVal delegateToCLR As Boolean)
			 Me.delegateToCLR = delegateToCLR

			 Try
				 libraryDirectory = System.getProperty(MLET_LIB_DIR)
				 If libraryDirectory Is Nothing Then libraryDirectory = tmpDir
			 Catch e As SecurityException
				 ' OK : We don't do AccessController.doPrivileged, but we don't
				 '      stop the user from creating an MLet just because they
				 '      can't read the MLET_LIB_DIR or java.io.tmpdir properties
				 '      either.
			 End Try
		 End Sub


	'     
	'      * ------------------------------------------
	'      *  PUBLIC METHODS
	'      * ------------------------------------------
	'      


		 ''' <summary>
		 ''' Appends the specified URL to the list of URLs to search for classes and
		 ''' resources.
		 ''' </summary>
		 Public Overridable Sub addURL(ByVal url As java.net.URL) Implements MLetMBean.addURL
			 If Not java.util.Arrays.asList(uRLs).contains(url) Then MyBase.addURL(url)
		 End Sub

		 ''' <summary>
		 ''' Appends the specified URL to the list of URLs to search for classes and
		 ''' resources. </summary>
		 ''' <exception cref="ServiceNotFoundException"> The specified URL is malformed. </exception>
		 Public Overridable Sub addURL(ByVal url As String) Implements MLetMBean.addURL
			 Try
				 Dim ur As New java.net.URL(url)
				 If Not java.util.Arrays.asList(uRLs).contains(ur) Then MyBase.addURL(ur)
			 Catch e As java.net.MalformedURLException
				 If MLET_LOGGER.isLoggable(java.util.logging.Level.FINEST) Then MLET_LOGGER.logp(java.util.logging.Level.FINEST, GetType(MLet).name, "addUrl", "Malformed URL: " & url, e)
				 Throw New javax.management.ServiceNotFoundException("The specified URL is malformed")
			 End Try
		 End Sub

		 ''' <summary>
		 ''' Returns the search path of URLs for loading classes and resources.
		 ''' This includes the original list of URLs specified to the constructor,
		 ''' along with any URLs subsequently appended by the addURL() method.
		 ''' </summary>
		 Public Overridable Property uRLs As java.net.URL() Implements MLetMBean.getURLs
			 Get
				 Return MyBase.uRLs
			 End Get
		 End Property

		 ''' <summary>
		 ''' Loads a text file containing MLET tags that define the MBeans to
		 ''' be added to the MBean server. The location of the text file is specified by
		 ''' a URL. The MBeans specified in the MLET file will be instantiated and
		 ''' registered in the MBean server.
		 ''' </summary>
		 ''' <param name="url"> The URL of the text file to be loaded as URL object.
		 ''' </param>
		 ''' <returns>  A set containing one entry per MLET tag in the m-let text file loaded.
		 ''' Each entry specifies either the ObjectInstance for the created MBean, or a throwable object
		 ''' (that is, an error or an exception) if the MBean could not be created.
		 ''' </returns>
		 ''' <exception cref="ServiceNotFoundException"> One of the following errors has occurred: The m-let text file does
		 ''' not contain an MLET tag, the m-let text file is not found, a mandatory
		 ''' attribute of the MLET tag is not specified, the value of url is
		 ''' null. </exception>
		 ''' <exception cref="IllegalStateException"> MLet MBean is not registered with an MBeanServer. </exception>
		 Public Overridable Function getMBeansFromURL(ByVal url As java.net.URL) As java.util.Set(Of Object) Implements MLetMBean.getMBeansFromURL
			 If url Is Nothing Then Throw New javax.management.ServiceNotFoundException("The specified URL is null")
			 Return getMBeansFromURL(url.ToString())
		 End Function

		 ''' <summary>
		 ''' Loads a text file containing MLET tags that define the MBeans to
		 ''' be added to the MBean server. The location of the text file is specified by
		 ''' a URL. The MBeans specified in the MLET file will be instantiated and
		 ''' registered in the MBean server.
		 ''' </summary>
		 ''' <param name="url"> The URL of the text file to be loaded as String object.
		 ''' </param>
		 ''' <returns> A set containing one entry per MLET tag in the m-let
		 ''' text file loaded.  Each entry specifies either the
		 ''' ObjectInstance for the created MBean, or a throwable object
		 ''' (that is, an error or an exception) if the MBean could not be
		 ''' created.
		 ''' </returns>
		 ''' <exception cref="ServiceNotFoundException"> One of the following
		 ''' errors has occurred: The m-let text file does not contain an
		 ''' MLET tag, the m-let text file is not found, a mandatory
		 ''' attribute of the MLET tag is not specified, the url is
		 ''' malformed. </exception>
		 ''' <exception cref="IllegalStateException"> MLet MBean is not registered
		 ''' with an MBeanServer.
		 '''  </exception>
		 Public Overridable Function getMBeansFromURL(ByVal url As String) As java.util.Set(Of Object) Implements MLetMBean.getMBeansFromURL

			 Dim mth As String = "getMBeansFromURL"

			 If server Is Nothing Then Throw New IllegalStateException("This MLet MBean is not " & "registered with an MBeanServer.")
			 ' Parse arguments
			 If url Is Nothing Then
				 MLET_LOGGER.logp(java.util.logging.Level.FINER, GetType(MLet).name, mth, "URL is null")
				 Throw New javax.management.ServiceNotFoundException("The specified URL is null")
			 Else
				 url = url.Replace(System.IO.Path.DirectorySeparatorChar,"/"c)
			 End If
			 If MLET_LOGGER.isLoggable(java.util.logging.Level.FINER) Then MLET_LOGGER.logp(java.util.logging.Level.FINER, GetType(MLet).name, mth, "<URL = " & url & ">")

			 ' Parse URL
			 Try
				 Dim parser As New MLetParser
				 mletList = parser.parseURL(url)
			 Catch e As Exception
				 Dim msg As String = "Problems while parsing URL [" & url & "], got exception [" & e.ToString() & "]"
				 MLET_LOGGER.logp(java.util.logging.Level.FINER, GetType(MLet).name, mth, msg)
				 Throw com.sun.jmx.remote.util.EnvHelp.initCause(New javax.management.ServiceNotFoundException(msg), e)
			 End Try

			 ' Check that the list of MLets is not empty
			 If mletList.Count = 0 Then
				 Dim msg As String = "File " & url & " not found or MLET tag not defined in file"
				 MLET_LOGGER.logp(java.util.logging.Level.FINER, GetType(MLet).name, mth, msg)
				 Throw New javax.management.ServiceNotFoundException(msg)
			 End If

			 ' Walk through the list of MLets
			 Dim mbeans As java.util.Set(Of Object) = New HashSet(Of Object)
			 For Each elmt As MLetContent In mletList
				 ' Initialize local variables
				 Dim code As String = elmt.code
				 If code IsNot Nothing Then
					 If code.EndsWith(".class") Then code = code.Substring(0, code.Length - 6)
				 End If
				 Dim name As String = elmt.name
				 Dim codebase As java.net.URL = elmt.codeBase
				 Dim version As String = elmt.version
				 Dim serName As String = elmt.serializedObject
				 Dim jarFiles As String = elmt.jarFiles
				 Dim documentBase As java.net.URL = elmt.documentBase

				 ' Display debug information
				 If MLET_LOGGER.isLoggable(java.util.logging.Level.FINER) Then
					 Dim strb As (New StringBuilder).Append(vbLf & vbTab & "MLET TAG     = ").append(elmt.attributes).append(vbLf & vbTab & "CODEBASE     = ").append(codebase).append(vbLf & vbTab & "ARCHIVE      = ").append(jarFiles).append(vbLf & vbTab & "CODE         = ").append(code).append(vbLf & vbTab & "OBJECT       = ").append(serName).append(vbLf & vbTab & "NAME         = ").append(name).append(vbLf & vbTab & "VERSION      = ").append(version).append(vbLf & vbTab & "DOCUMENT URL = ").append(documentBase)
					 MLET_LOGGER.logp(java.util.logging.Level.FINER, GetType(MLet).name, mth, strb.ToString())
				 End If

				 ' Load classes from JAR files
				 Dim st As New java.util.StringTokenizer(jarFiles, ",", False)
				 Do While st.hasMoreTokens()
					 Dim tok As String = st.nextToken().Trim()
					 If MLET_LOGGER.isLoggable(java.util.logging.Level.FINER) Then MLET_LOGGER.logp(java.util.logging.Level.FINER, GetType(MLet).name, mth, "Load archive for codebase <" & codebase & ">, file <" & tok & ">")
					 ' Check which is the codebase to be used for loading the jar file.
					 ' If we are using the base MLet implementation then it will be
					 ' always the remote server but if the service has been extended in
					 ' order to support caching and versioning then this method will
					 ' return the appropriate one.
					 '
					 Try
						 codebase = check(version, codebase, tok, elmt)
					 Catch ex As Exception
						 MLET_LOGGER.logp(java.util.logging.Level.FINEST, GetType(MLet).name, mth, "Got unexpected exception", ex)
						 mbeans.add(ex)
						 Continue Do
					 End Try

					 ' Appends the specified JAR file URL to the list of
					 ' URLs to search for classes and resources.
					 Try
						 If Not java.util.Arrays.asList(uRLs).contains(New java.net.URL(codebase.ToString() & tok)) Then addURL(codebase + tok)
					 Catch [me] As java.net.MalformedURLException
						 ' OK : Ignore jar file if its name provokes the
						 ' URL to be an invalid one.
					 End Try

				 Loop
				 ' Instantiate the class specified in the
				 ' CODE or OBJECT section of the MLet tag
				 '
				 Dim o As Object
				 Dim objInst As javax.management.ObjectInstance

				 If code IsNot Nothing AndAlso serName IsNot Nothing Then
					 Dim msg As String = "CODE and OBJECT parameters cannot be specified at the " & "same time in tag MLET"
					 MLET_LOGGER.logp(java.util.logging.Level.FINER, GetType(MLet).name, mth, msg)
					 mbeans.add(New Exception(msg))
					 Continue For
				 End If
				 If code Is Nothing AndAlso serName Is Nothing Then
					 Dim msg As String = "Either CODE or OBJECT parameter must be specified in " & "tag MLET"
					 MLET_LOGGER.logp(java.util.logging.Level.FINER, GetType(MLet).name, mth, msg)
					 mbeans.add(New Exception(msg))
					 Continue For
				 End If
				 Try
					 If code IsNot Nothing Then

						 Dim signat As IList(Of String) = elmt.parameterTypes
						 Dim stringPars As IList(Of String) = elmt.parameterValues
						 Dim objectPars As IList(Of Object) = New List(Of Object)

						 For i As Integer = 0 To signat.Count - 1
							 objectPars.Add(constructParameter(stringPars(i), signat(i)))
						 Next i
						 If signat.Count = 0 Then
							 If name Is Nothing Then
								 objInst = server.createMBean(code, Nothing, mletObjectName)
							 Else
								 objInst = server.createMBean(code, New javax.management.ObjectName(name), mletObjectName)
							 End If
						 Else
							 Dim parms As Object() = objectPars.ToArray()
							 Dim signature As String() = New String(signat.Count - 1){}
							 signat.ToArray(signature)
							 If MLET_LOGGER.isLoggable(java.util.logging.Level.FINEST) Then
								 Dim strb As New StringBuilder
								 For i As Integer = 0 To signature.Length - 1
									 strb.Append(vbLf & vbTab & "Signature     = ").append(signature(i)).append(vbTab & vbLf & "Params        = ").append(parms(i))
								 Next i
								 MLET_LOGGER.logp(java.util.logging.Level.FINEST, GetType(MLet).name, mth, strb.ToString())
							 End If
							 If name Is Nothing Then
								 objInst = server.createMBean(code, Nothing, mletObjectName, parms, signature)
							 Else
								 objInst = server.createMBean(code, New javax.management.ObjectName(name), mletObjectName, parms, signature)
							 End If
						 End If
					 Else
						 o = loadSerializedObject(codebase,serName)
						 If name Is Nothing Then
							 server.registerMBean(o, Nothing)
						 Else
							 server.registerMBean(o, New javax.management.ObjectName(name))
						 End If
						 objInst = New javax.management.ObjectInstance(name, o.GetType().name)
					 End If
				 Catch ex As javax.management.ReflectionException
					 MLET_LOGGER.logp(java.util.logging.Level.FINER, GetType(MLet).name, mth, "ReflectionException", ex)
					 mbeans.add(ex)
					 Continue For
				 Catch ex As javax.management.InstanceAlreadyExistsException
					 MLET_LOGGER.logp(java.util.logging.Level.FINER, GetType(MLet).name, mth, "InstanceAlreadyExistsException", ex)
					 mbeans.add(ex)
					 Continue For
				 Catch ex As javax.management.MBeanRegistrationException
					 MLET_LOGGER.logp(java.util.logging.Level.FINER, GetType(MLet).name, mth, "MBeanRegistrationException", ex)
					 mbeans.add(ex)
					 Continue For
				 Catch ex As javax.management.MBeanException
					 MLET_LOGGER.logp(java.util.logging.Level.FINER, GetType(MLet).name, mth, "MBeanException", ex)
					 mbeans.add(ex)
					 Continue For
				 Catch ex As javax.management.NotCompliantMBeanException
					 MLET_LOGGER.logp(java.util.logging.Level.FINER, GetType(MLet).name, mth, "NotCompliantMBeanException", ex)
					 mbeans.add(ex)
					 Continue For
				 Catch ex As javax.management.InstanceNotFoundException
					 MLET_LOGGER.logp(java.util.logging.Level.FINER, GetType(MLet).name, mth, "InstanceNotFoundException", ex)
					 mbeans.add(ex)
					 Continue For
				 Catch ex As java.io.IOException
					 MLET_LOGGER.logp(java.util.logging.Level.FINER, GetType(MLet).name, mth, "IOException", ex)
					 mbeans.add(ex)
					 Continue For
				 Catch ex As SecurityException
					 MLET_LOGGER.logp(java.util.logging.Level.FINER, GetType(MLet).name, mth, "SecurityException", ex)
					 mbeans.add(ex)
					 Continue For
				 Catch ex As Exception
					 MLET_LOGGER.logp(java.util.logging.Level.FINER, GetType(MLet).name, mth, "Exception", ex)
					 mbeans.add(ex)
					 Continue For
				 Catch ex As Exception
					 MLET_LOGGER.logp(java.util.logging.Level.FINER, GetType(MLet).name, mth, "Error", ex)
					 mbeans.add(ex)
					 Continue For
				 End Try
				 mbeans.add(objInst)
			 Next elmt
			 Return mbeans
		 End Function

		 ''' <summary>
		 ''' Gets the current directory used by the library loader for
		 ''' storing native libraries before they are loaded into memory.
		 ''' </summary>
		 ''' <returns> The current directory used by the library loader.
		 ''' </returns>
		 ''' <seealso cref= #setLibraryDirectory
		 ''' </seealso>
		 ''' <exception cref="UnsupportedOperationException"> if this implementation
		 ''' does not support storing native libraries in this way. </exception>
		 <MethodImpl(MethodImplOptions.Synchronized)> _
		 Public Overridable Property libraryDirectory As String Implements MLetMBean.getLibraryDirectory
			 Get
				 Return libraryDirectory
			 End Get
			 Set(ByVal libdir As String)
				 libraryDirectory = libdir
			 End Set
		 End Property


		 ''' <summary>
		 ''' Allows the m-let to perform any operations it needs before
		 ''' being registered in the MBean server. If the ObjectName is
		 ''' null, the m-let provides a default name for its registration
		 ''' &lt;defaultDomain&gt;:type=MLet
		 ''' </summary>
		 ''' <param name="server"> The MBean server in which the m-let will be registered. </param>
		 ''' <param name="name"> The object name of the m-let.
		 ''' </param>
		 ''' <returns>  The name of the m-let registered.
		 ''' </returns>
		 ''' <exception cref="java.lang.Exception"> This exception should be caught by the MBean server and re-thrown
		 ''' as an MBeanRegistrationException. </exception>
		 Public Overridable Function preRegister(ByVal server As javax.management.MBeanServer, ByVal name As javax.management.ObjectName) As javax.management.ObjectName

			 ' Initialize local pointer to the MBean server
			 mBeanServer = server

			 ' If no name is specified return a default name for the MLet
			 If name Is Nothing Then name = New javax.management.ObjectName(server.defaultDomain & ":" & com.sun.jmx.defaults.ServiceName.MLET)

			Me.mletObjectName = name
			Return Me.mletObjectName
		 End Function

		 ''' <summary>
		 ''' Allows the m-let to perform any operations needed after having been
		 ''' registered in the MBean server or after the registration has failed.
		 ''' </summary>
		 ''' <param name="registrationDone"> Indicates whether or not the m-let has
		 ''' been successfully registered in the MBean server. The value
		 ''' false means that either the registration phase has failed.
		 '''  </param>
		 Public Overridable Sub postRegister(ByVal registrationDone As Boolean?)
		 End Sub

		 ''' <summary>
		 ''' Allows the m-let to perform any operations it needs before being unregistered
		 ''' by the MBean server.
		 ''' </summary>
		 ''' <exception cref="java.lang.Exception"> This exception should be caught
		 ''' by the MBean server and re-thrown as an
		 ''' MBeanRegistrationException. </exception>
		 Public Overridable Sub preDeregister()
		 End Sub


		 ''' <summary>
		 ''' Allows the m-let to perform any operations needed after having been
		 ''' unregistered in the MBean server.
		 ''' </summary>
		 Public Overridable Sub postDeregister()
		 End Sub

		 ''' <summary>
		 ''' <p>Save this MLet's contents to the given <seealso cref="ObjectOutput"/>.
		 ''' Not all implementations support this method.  Those that do not
		 ''' throw <seealso cref="UnsupportedOperationException"/>.  A subclass may
		 ''' override this method to support it or to change the format of
		 ''' the written data.</p>
		 ''' 
		 ''' <p>The format of the written data is not specified, but if
		 ''' an implementation supports <seealso cref="#writeExternal"/> it must
		 ''' also support <seealso cref="#readExternal"/> in such a way that what is
		 ''' written by the former can be read by the latter.</p>
		 ''' </summary>
		 ''' <param name="out"> The object output stream to write to.
		 ''' </param>
		 ''' <exception cref="IOException"> If a problem occurred while writing. </exception>
		 ''' <exception cref="UnsupportedOperationException"> If this
		 ''' implementation does not support this operation. </exception>
		 Public Overridable Sub writeExternal(ByVal out As java.io.ObjectOutput)
			 Throw New System.NotSupportedException("MLet.writeExternal")
		 End Sub

		 ''' <summary>
		 ''' <p>Restore this MLet's contents from the given <seealso cref="ObjectInput"/>.
		 ''' Not all implementations support this method.  Those that do not
		 ''' throw <seealso cref="UnsupportedOperationException"/>.  A subclass may
		 ''' override this method to support it or to change the format of
		 ''' the read data.</p>
		 ''' 
		 ''' <p>The format of the read data is not specified, but if an
		 ''' implementation supports <seealso cref="#readExternal"/> it must also
		 ''' support <seealso cref="#writeExternal"/> in such a way that what is
		 ''' written by the latter can be read by the former.</p>
		 ''' </summary>
		 ''' <param name="in"> The object input stream to read from.
		 ''' </param>
		 ''' <exception cref="IOException"> if a problem occurred while reading. </exception>
		 ''' <exception cref="ClassNotFoundException"> if the class for the object
		 ''' being restored cannot be found. </exception>
		 ''' <exception cref="UnsupportedOperationException"> if this
		 ''' implementation does not support this operation. </exception>
		 Public Overridable Sub readExternal(ByVal [in] As java.io.ObjectInput)
			 Throw New System.NotSupportedException("MLet.readExternal")
		 End Sub

	'     
	'      * ------------------------------------------
	'      *  PACKAGE METHODS
	'      * ------------------------------------------
	'      

		 ''' <summary>
		 ''' <p>Load a class, using the given <seealso cref="ClassLoaderRepository"/> if
		 ''' the class is not found in this MLet's URLs.  The given
		 ''' ClassLoaderRepository can be null, in which case a {@link
		 ''' ClassNotFoundException} occurs immediately if the class is not
		 ''' found in this MLet's URLs.</p>
		 ''' </summary>
		 ''' <param name="name"> The name of the class we want to load. </param>
		 ''' <param name="clr">  The ClassLoaderRepository that will be used to search
		 '''             for the given class, if it is not found in this
		 '''             ClassLoader.  May be null. </param>
		 ''' <returns> The resulting Class object. </returns>
		 ''' <exception cref="ClassNotFoundException"> The specified class could not be
		 '''            found in this ClassLoader nor in the given
		 '''            ClassLoaderRepository.
		 '''  </exception>
		 <MethodImpl(MethodImplOptions.Synchronized)> _
		 Public Overridable Function loadClass(ByVal name As String, ByVal clr As ClassLoaderRepository) As Type
			 Dim before As ClassLoaderRepository=currentClr
			 Try
				 currentClr = clr
				 Return loadClass(name)
			 Finally
				 currentClr = before
			 End Try
		 End Function

	'     
	'      * ------------------------------------------
	'      *  PROTECTED METHODS
	'      * ------------------------------------------
	'      

		 ''' <summary>
		 ''' This is the main method for class loaders that is being redefined.
		 ''' </summary>
		 ''' <param name="name"> The name of the class.
		 ''' </param>
		 ''' <returns> The resulting Class object.
		 ''' </returns>
		 ''' <exception cref="ClassNotFoundException"> The specified class could not be
		 '''            found. </exception>
		 Protected Friend Overridable Function findClass(ByVal name As String) As Type
	'          currentClr is context sensitive - used to avoid recursion
	'            in the class loader repository.  (This is no longer
	'            necessary with the new CLR semantics but is kept for
	'            compatibility with code that might have called the
	'            two-parameter loadClass explicitly.)  
			 Return findClass(name, currentClr)
		 End Function

		 ''' <summary>
		 ''' Called by <seealso cref="MLet#findClass(java.lang.String)"/>.
		 ''' </summary>
		 ''' <param name="name"> The name of the class that we want to load/find. </param>
		 ''' <param name="clr"> The ClassLoaderRepository that can be used to search
		 '''            for the given class. This parameter is
		 '''            <code>null</code> when called from within the
		 '''            <seealso cref="javax.management.MBeanServerFactory#getClassLoaderRepository(javax.management.MBeanServer) Class Loader Repository"/>. </param>
		 ''' <exception cref="ClassNotFoundException"> The specified class could not be
		 '''            found.
		 ''' 
		 '''  </exception>
		 Friend Overridable Function findClass(ByVal name As String, ByVal clr As ClassLoaderRepository) As Type
			 Dim c As Type = Nothing
			 MLET_LOGGER.logp(java.util.logging.Level.FINER, GetType(MLet).name, "findClass", name)
			 ' Try looking in the JAR:
			 Try
				 c = MyBase.findClass(name)
				 If MLET_LOGGER.isLoggable(java.util.logging.Level.FINER) Then MLET_LOGGER.logp(java.util.logging.Level.FINER, GetType(MLet).name, "findClass", "Class " & name & " loaded through MLet classloader")
			 Catch e As ClassNotFoundException
				 ' Drop through
				 If MLET_LOGGER.isLoggable(java.util.logging.Level.FINEST) Then MLET_LOGGER.logp(java.util.logging.Level.FINEST, GetType(MLet).name, "findClass", "Class " & name & " not found locally")
			 End Try
			 ' if we are not called from the ClassLoaderRepository
			 If c Is Nothing AndAlso delegateToCLR AndAlso clr IsNot Nothing Then
				 ' Try the classloader repository:
				 '
				 Try
					 If MLET_LOGGER.isLoggable(java.util.logging.Level.FINEST) Then MLET_LOGGER.logp(java.util.logging.Level.FINEST, GetType(MLet).name, "findClass", "Class " & name & " : looking in CLR")
					 c = clr.loadClassBefore(Me, name)
					 ' The loadClassBefore method never returns null.
					 ' If the class is not found we get an exception.
					 If MLET_LOGGER.isLoggable(java.util.logging.Level.FINER) Then MLET_LOGGER.logp(java.util.logging.Level.FINER, GetType(MLet).name, "findClass", "Class " & name & " loaded through " & "the default classloader repository")
				 Catch e As ClassNotFoundException
					 ' Drop through
					 If MLET_LOGGER.isLoggable(java.util.logging.Level.FINEST) Then MLET_LOGGER.logp(java.util.logging.Level.FINEST, GetType(MLet).name, "findClass", "Class " & name & " not found in CLR")
				 End Try
			 End If
			 If c Is Nothing Then
				 MLET_LOGGER.logp(java.util.logging.Level.FINEST, GetType(MLet).name, "findClass", "Failed to load class " & name)
				 Throw New ClassNotFoundException(name)
			 End If
			 Return c
		 End Function

		 ''' <summary>
		 ''' Returns the absolute path name of a native library. The VM
		 ''' invokes this method to locate the native libraries that belong
		 ''' to classes loaded with this class loader. Libraries are
		 ''' searched in the JAR files using first just the native library
		 ''' name and if not found the native library name together with
		 ''' the architecture-specific path name
		 ''' (<code>OSName/OSArch/OSVersion/lib/nativelibname</code>), i.e.
		 ''' <p>
		 ''' the library stat on Solaris SPARC 5.7 will be searched in the JAR file as:
		 ''' <OL>
		 ''' <LI>libstat.so
		 ''' <LI>SunOS/sparc/5.7/lib/libstat.so
		 ''' </OL>
		 ''' the library stat on Windows NT 4.0 will be searched in the JAR file as:
		 ''' <OL>
		 ''' <LI>stat.dll
		 ''' <LI>WindowsNT/x86/4.0/lib/stat.dll
		 ''' </OL>
		 ''' 
		 ''' <p>More specifically, let <em>{@code nativelibname}</em> be the result of
		 ''' {@link System#mapLibraryName(java.lang.String)
		 ''' System.mapLibraryName}{@code (libname)}.  Then the following names are
		 ''' searched in the JAR files, in order:<br>
		 ''' <em>{@code nativelibname}</em><br>
		 ''' {@code <os.name>/<os.arch>/<os.version>/lib/}<em>{@code nativelibname}</em><br>
		 ''' where {@code <X>} means {@code System.getProperty(X)} with any
		 ''' spaces in the result removed, and {@code /} stands for the
		 ''' file separator character (<seealso cref="File#separator"/>).
		 ''' <p>
		 ''' If this method returns <code>null</code>, i.e. the libraries
		 ''' were not found in any of the JAR files loaded with this class
		 ''' loader, the VM searches the library along the path specified
		 ''' as the <code>java.library.path</code> property.
		 ''' </summary>
		 ''' <param name="libname"> The library name.
		 ''' </param>
		 ''' <returns> The absolute path of the native library. </returns>
		 Protected Friend Overridable Function findLibrary(ByVal libname As String) As String

			 Dim abs_path As String
			 Dim mth As String = "findLibrary"

			 ' Get the platform-specific string representing a native library.
			 '
			 Dim nativelibname As String = System.mapLibraryName(libname)

			 '
			 ' See if the native library is accessible as a resource through the JAR file.
			 '
			 If MLET_LOGGER.isLoggable(java.util.logging.Level.FINER) Then MLET_LOGGER.logp(java.util.logging.Level.FINER, GetType(MLet).name, mth, "Search " & libname & " in all JAR files")

			 ' First try to locate the library in the JAR file using only
			 ' the native library name.  e.g. if user requested a load
			 ' for "foo" on Solaris SPARC 5.7 we try to load "libfoo.so"
			 ' from the JAR file.
			 '
			 If MLET_LOGGER.isLoggable(java.util.logging.Level.FINER) Then MLET_LOGGER.logp(java.util.logging.Level.FINER, GetType(MLet).name, mth, "loadLibraryAsResource(" & nativelibname & ")")
			 abs_path = loadLibraryAsResource(nativelibname)
			 If abs_path IsNot Nothing Then
				 If MLET_LOGGER.isLoggable(java.util.logging.Level.FINER) Then MLET_LOGGER.logp(java.util.logging.Level.FINER, GetType(MLet).name, mth, nativelibname & " loaded, absolute path = " & abs_path)
				 Return abs_path
			 End If

			 ' Next try to locate it using the native library name and
			 ' the architecture-specific path name.  e.g. if user
			 ' requested a load for "foo" on Solaris SPARC 5.7 we try to
			 ' load "SunOS/sparc/5.7/lib/libfoo.so" from the JAR file.
			 '
			 nativelibname = removeSpace(System.getProperty("os.name")) + File.separator + removeSpace(System.getProperty("os.arch")) + File.separator + removeSpace(System.getProperty("os.version")) + File.separator & "lib" & File.separator + nativelibname
			 If MLET_LOGGER.isLoggable(java.util.logging.Level.FINER) Then MLET_LOGGER.logp(java.util.logging.Level.FINER, GetType(MLet).name, mth, "loadLibraryAsResource(" & nativelibname & ")")

			 abs_path = loadLibraryAsResource(nativelibname)
			 If abs_path IsNot Nothing Then
				 If MLET_LOGGER.isLoggable(java.util.logging.Level.FINER) Then MLET_LOGGER.logp(java.util.logging.Level.FINER, GetType(MLet).name, mth, nativelibname & " loaded, absolute path = " & abs_path)
				 Return abs_path
			 End If

			 '
			 ' All paths exhausted, library not found in JAR file.
			 '

			 If MLET_LOGGER.isLoggable(java.util.logging.Level.FINER) Then
				 MLET_LOGGER.logp(java.util.logging.Level.FINER, GetType(MLet).name, mth, libname & " not found in any JAR file")
				 MLET_LOGGER.logp(java.util.logging.Level.FINER, GetType(MLet).name, mth, "Search " & libname & " along the path " & "specified as the java.library.path property")
			 End If

			 ' Let the VM search the library along the path
			 ' specified as the java.library.path property.
			 '
			 Return Nothing
		 End Function


	'     
	'      * ------------------------------------------
	'      *  PRIVATE METHODS
	'      * ------------------------------------------
	'      

		 Private Property tmpDir As String
			 Get
				 ' JDK 1.4
				 Dim ___tmpDir As String = System.getProperty("java.io.tmpdir")
				 If ___tmpDir IsNot Nothing Then Return ___tmpDir
    
				 ' JDK < 1.4
				 Dim tmpFile As File = Nothing
				 Try
					 ' Try to guess the system temporary dir...
					 tmpFile = File.createTempFile("tmp","jmx")
					 If tmpFile Is Nothing Then Return Nothing
					 Dim tmpDirFile As File = tmpFile.parentFile
					 If tmpDirFile Is Nothing Then Return Nothing
					 Return tmpDirFile.absolutePath
				 Catch x As Exception
					 MLET_LOGGER.logp(java.util.logging.Level.FINEST, GetType(MLet).name, "getTmpDir", "Failed to determine system temporary dir")
					 Return Nothing
				 Finally
					 ' Cleanup ...
					 If tmpFile IsNot Nothing Then
						 Try
							 Dim deleted As Boolean = tmpFile.delete()
							 If Not deleted Then MLET_LOGGER.logp(java.util.logging.Level.FINEST, GetType(MLet).name, "getTmpDir", "Failed to delete temp file")
						 Catch x As Exception
							 MLET_LOGGER.logp(java.util.logging.Level.FINEST, GetType(MLet).name, "getTmpDir", "Failed to delete temporary file", x)
						 End Try
					 End If
				 End Try
			 End Get
		 End Property

		 ''' <summary>
		 ''' Search the specified native library in any of the JAR files
		 ''' loaded by this classloader.  If the library is found copy it
		 ''' into the library directory and return the absolute path.  If
		 ''' the library is not found then return null.
		 ''' </summary>
		 <MethodImpl(MethodImplOptions.Synchronized)> _
		 Private Function loadLibraryAsResource(ByVal libname As String) As String
			 Try
				 Dim [is] As java.io.InputStream = getResourceAsStream(libname.Replace(System.IO.Path.DirectorySeparatorChar,"/"c))
				 If [is] IsNot Nothing Then
					 Try
						 Dim directory As New File(libraryDirectory)
						 directory.mkdirs()
						 Dim file As File = java.nio.file.Files.createTempFile(directory.toPath(), libname & ".", Nothing).toFile()
						 file.deleteOnExit()
						 Dim fileOutput As New java.io.FileOutputStream(file)
						 Try
							 Dim buf As SByte() = New SByte(4095){}
							 Dim n As Integer
							 n = [is].read(buf)
							 Do While n >= 0
								fileOutput.write(buf, 0, n)
								 n = [is].read(buf)
							 Loop
						 Finally
							 fileOutput.close()
						 End Try
						 If file.exists() Then Return file.absolutePath
					 Finally
						 [is].close()
					 End Try
				 End If
			 Catch e As Exception
				 MLET_LOGGER.logp(java.util.logging.Level.FINEST, GetType(MLet).name, "loadLibraryAsResource", "Failed to load library : " & libname, e)
				 Return Nothing
			 End Try
			 Return Nothing
		 End Function

	   ''' <summary>
	   ''' Removes any white space from a string. This is used to
	   ''' convert strings such as "Windows NT" to "WindowsNT".
	   ''' </summary>
		 Private Shared Function removeSpace(ByVal s As String) As String
			 Return s.Trim().Replace(" ", "")
		 End Function

		 ''' <summary>
		 ''' <p>This method is to be overridden when extending this service to
		 ''' support caching and versioning.  It is called from {@link
		 ''' #getMBeansFromURL getMBeansFromURL} when the version,
		 ''' codebase, and jarfile have been extracted from the MLet file,
		 ''' and can be used to verify that it is all right to load the
		 ''' given MBean, or to replace the given URL with a different one.</p>
		 ''' 
		 ''' <p>The default implementation of this method returns
		 ''' <code>codebase</code> unchanged.</p>
		 ''' </summary>
		 ''' <param name="version"> The version number of the <CODE>.jar</CODE>
		 ''' file stored locally. </param>
		 ''' <param name="codebase"> The base URL of the remote <CODE>.jar</CODE> file. </param>
		 ''' <param name="jarfile"> The name of the <CODE>.jar</CODE> file to be loaded. </param>
		 ''' <param name="mlet"> The <CODE>MLetContent</CODE> instance that
		 ''' represents the <CODE>MLET</CODE> tag.
		 ''' </param>
		 ''' <returns> the codebase to use for the loaded MBean.  The returned
		 ''' value should not be null.
		 ''' </returns>
		 ''' <exception cref="Exception"> if the MBean is not to be loaded for some
		 ''' reason.  The exception will be added to the set returned by
		 ''' <seealso cref="#getMBeansFromURL getMBeansFromURL"/>.
		 '''  </exception>
		 Protected Friend Overridable Function check(ByVal version As String, ByVal codebase As java.net.URL, ByVal jarfile As String, ByVal mlet As MLetContent) As java.net.URL
			 Return codebase
		 End Function

		''' <summary>
		''' Loads the serialized object specified by the <CODE>OBJECT</CODE>
		''' attribute of the <CODE>MLET</CODE> tag.
		''' </summary>
		''' <param name="codebase"> The <CODE>codebase</CODE>. </param>
		''' <param name="filename"> The name of the file containing the serialized object. </param>
		''' <returns> The serialized object. </returns>
		''' <exception cref="ClassNotFoundException"> The specified serialized
		''' object could not be found. </exception>
		''' <exception cref="IOException"> An I/O error occurred while loading
		''' serialized object. </exception>
		 Private Function loadSerializedObject(ByVal codebase As java.net.URL, ByVal filename As String) As Object
			If filename IsNot Nothing Then filename = filename.Replace(System.IO.Path.DirectorySeparatorChar,"/"c)
			If MLET_LOGGER.isLoggable(java.util.logging.Level.FINER) Then MLET_LOGGER.logp(java.util.logging.Level.FINER, GetType(MLet).name, "loadSerializedObject", codebase.ToString() & filename)
			Dim [is] As java.io.InputStream = getResourceAsStream(filename)
			If [is] IsNot Nothing Then
				Try
					Dim ois As java.io.ObjectInputStream = New MLetObjectInputStream([is], Me)
					Dim serObject As Object = ois.readObject()
					ois.close()
					Return serObject
				Catch e As java.io.IOException
					If MLET_LOGGER.isLoggable(java.util.logging.Level.FINEST) Then MLET_LOGGER.logp(java.util.logging.Level.FINEST, GetType(MLet).name, "loadSerializedObject", "Exception while deserializing " & filename, e)
					Throw e
				Catch e As ClassNotFoundException
					If MLET_LOGGER.isLoggable(java.util.logging.Level.FINEST) Then MLET_LOGGER.logp(java.util.logging.Level.FINEST, GetType(MLet).name, "loadSerializedObject", "Exception while deserializing " & filename, e)
					Throw e
				End Try
			Else
				If MLET_LOGGER.isLoggable(java.util.logging.Level.FINEST) Then MLET_LOGGER.logp(java.util.logging.Level.FINEST, GetType(MLet).name, "loadSerializedObject", "Error: File " & filename & " containing serialized object not found")
				Throw New Exception("File " & filename & " containing serialized object not found")
			End If
		 End Function

		 ''' <summary>
		 ''' Converts the String value of the constructor's parameter to
		 ''' a basic Java object with the type of the parameter.
		 ''' </summary>
		 Private Function constructParameter(ByVal param As String, ByVal type As String) As Object
			 ' check if it is a primitive type
			 Dim c As Type = primitiveClasses(type)
			 If c IsNot Nothing Then
				Try
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
					Dim cons As Constructor(Of ?) = c.GetConstructor(GetType(String))
					Dim oo As Object() = New Object(0){}
					oo(0)=param
					Return (cons.newInstance(oo))

				Catch e As Exception
					MLET_LOGGER.logp(java.util.logging.Level.FINEST, GetType(MLet).name, "constructParameter", "Got unexpected exception", e)
				End Try
			 End If
			If type.CompareTo("java.lang.Boolean") = 0 Then Return Convert.ToBoolean(param)
			If type.CompareTo("java.lang.Byte") = 0 Then Return New SByte?(param)
			If type.CompareTo("java.lang.Short") = 0 Then Return New Short?(param)
			If type.CompareTo("java.lang.Long") = 0 Then Return New Long?(param)
			If type.CompareTo("java.lang.Integer") = 0 Then Return New Integer?(param)
			If type.CompareTo("java.lang.Float") = 0 Then Return New Single?(param)
			If type.CompareTo("java.lang.Double") = 0 Then Return New Double?(param)
			If type.CompareTo("java.lang.String") = 0 Then Return param

			Return param
		 End Function

		<MethodImpl(MethodImplOptions.Synchronized)> _
		Private Property mBeanServer As javax.management.MBeanServer
			Set(ByVal server As javax.management.MBeanServer)
				Me.server = server
	'JAVA TO VB CONVERTER TODO TASK: Anonymous inner classes are not converted to VB if the base type is not defined in the code being converted:
	'			java.security.PrivilegedAction<ClassLoaderRepository> act = New java.security.PrivilegedAction<ClassLoaderRepository>()
		'		{
		'				public ClassLoaderRepository run()
		'				{
		'					Return server.getClassLoaderRepository();
		'				}
		'			};
				currentClr = java.security.AccessController.doPrivileged(act)
			End Set
		End Property

	End Class

End Namespace