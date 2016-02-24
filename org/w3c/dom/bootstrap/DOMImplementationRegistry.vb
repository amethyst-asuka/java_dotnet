Imports System
Imports System.Collections

'
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

'
' *
' *
' *
' *
' *
' * Copyright (c) 2004 World Wide Web Consortium,
' *
' * (Massachusetts Institute of Technology, European Research Consortium for
' * Informatics and Mathematics, Keio University). All Rights Reserved. This
' * work is distributed under the W3C(r) Software License [1] in the hope that
' * it will be useful, but WITHOUT ANY WARRANTY; without even the implied
' * warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.
' *
' * [1] http://www.w3.org/Consortium/Legal/2002/copyright-software-20021231
' 


Namespace org.w3c.dom.bootstrap


	''' <summary>
	''' A factory that enables applications to obtain instances of
	''' <code>DOMImplementation</code>.
	''' 
	''' <p>
	''' Example:
	''' </p>
	''' 
	''' <pre class='example'>
	'''  // get an instance of the DOMImplementation registry
	'''  DOMImplementationRegistry registry =
	'''       DOMImplementationRegistry.newInstance();
	'''  // get a DOM implementation the Level 3 XML module
	'''  DOMImplementation domImpl =
	'''       registry.getDOMImplementation("XML 3.0");
	''' </pre>
	''' 
	''' <p>
	''' This provides an application with an implementation-independent starting
	''' point. DOM implementations may modify this class to meet new security
	''' standards or to provide *additional* fallbacks for the list of
	''' DOMImplementationSources.
	''' </p>
	''' </summary>
	''' <seealso cref= DOMImplementation </seealso>
	''' <seealso cref= DOMImplementationSource
	''' @since DOM Level 3 </seealso>
	Public NotInheritable Class DOMImplementationRegistry
		''' <summary>
		''' The system property to specify the
		''' DOMImplementationSource class names.
		''' </summary>
		Public Const [PROPERTY] As String = "org.w3c.dom.DOMImplementationSourceList"

		''' <summary>
		''' Default columns per line.
		''' </summary>
		Private Const DEFAULT_LINE_LENGTH As Integer = 80

		''' <summary>
		''' The list of DOMImplementationSources.
		''' </summary>
		Private sources As ArrayList

		''' <summary>
		''' Default class name.
		''' </summary>
		Private Const FALLBACK_CLASS As String = "com.sun.org.apache.xerces.internal.dom.DOMXSImplementationSourceImpl"
		Private Const DEFAULT_PACKAGE As String = "com.sun.org.apache.xerces.internal.dom"
		''' <summary>
		''' Private constructor. </summary>
		''' <param name="srcs"> Vector List of DOMImplementationSources </param>
		Private Sub New(ByVal srcs As ArrayList)
			sources = srcs
		End Sub

		''' <summary>
		''' Obtain a new instance of a <code>DOMImplementationRegistry</code>.
		''' 
		''' 
		''' The <code>DOMImplementationRegistry</code> is initialized by the
		''' application or the implementation, depending on the context, by
		''' first checking the value of the Java system property
		''' <code>org.w3c.dom.DOMImplementationSourceList</code> and
		''' the service provider whose contents are at
		''' "<code>META_INF/services/org.w3c.dom.DOMImplementationSourceList</code>".
		''' The value of this property is a white-space separated list of
		''' names of availables classes implementing the
		''' <code>DOMImplementationSource</code> interface. Each class listed
		''' in the class name list is instantiated and any exceptions
		''' encountered are thrown to the application.
		''' </summary>
		''' <returns> an initialized instance of DOMImplementationRegistry </returns>
		''' <exception cref="ClassNotFoundException">
		'''     If any specified class can not be found </exception>
		''' <exception cref="InstantiationException">
		'''     If any specified class is an interface or abstract class </exception>
		''' <exception cref="IllegalAccessException">
		'''     If the default constructor of a specified class is not accessible </exception>
		''' <exception cref="ClassCastException">
		'''     If any specified class does not implement
		''' <code>DOMImplementationSource</code> </exception>
		Public Shared Function newInstance() As DOMImplementationRegistry
			Dim sources As New ArrayList

			Dim classLoader_Renamed As ClassLoader = classLoader
			' fetch system property:
			Dim p As String = getSystemProperty([PROPERTY])

			'
			' if property is not specified then use contents of
			' META_INF/org.w3c.dom.DOMImplementationSourceList from classpath
			If p Is Nothing Then p = getServiceValue(classLoader_Renamed)
			If p Is Nothing Then p = FALLBACK_CLASS
			If p IsNot Nothing Then
				Dim st As New java.util.StringTokenizer(p)
				Do While st.hasMoreTokens()
					Dim sourceName As String = st.nextToken()
					' make sure we have access to restricted packages
					Dim internal As Boolean = False
					If System.securityManager IsNot Nothing Then
						If sourceName IsNot Nothing AndAlso sourceName.StartsWith(DEFAULT_PACKAGE) Then internal = True
					End If
					Dim sourceClass As Type = Nothing
					If classLoader_Renamed IsNot Nothing AndAlso (Not internal) Then
						sourceClass = classLoader_Renamed.loadClass(sourceName)
					Else
						sourceClass = Type.GetType(sourceName)
					End If
					Dim source As org.w3c.dom.DOMImplementationSource = CType(sourceClass.newInstance(), org.w3c.dom.DOMImplementationSource)
					sources.Add(source)
				Loop
			End If
			Return New DOMImplementationRegistry(sources)
		End Function

		''' <summary>
		''' Return the first implementation that has the desired
		''' features, or <code>null</code> if none is found.
		''' </summary>
		''' <param name="features">
		'''            A string that specifies which features are required. This is
		'''            a space separated list in which each feature is specified by
		'''            its name optionally followed by a space and a version number.
		'''            This is something like: "XML 1.0 Traversal +Events 2.0" </param>
		''' <returns> An implementation that has the desired features,
		'''         or <code>null</code> if none found. </returns>
		Public Function getDOMImplementation(ByVal features As String) As org.w3c.dom.DOMImplementation
			Dim size As Integer = sources.Count
			Dim name As String = Nothing
			For i As Integer = 0 To size - 1
				Dim source As org.w3c.dom.DOMImplementationSource = CType(sources(i), org.w3c.dom.DOMImplementationSource)
				Dim impl As org.w3c.dom.DOMImplementation = source.getDOMImplementation(features)
				If impl IsNot Nothing Then Return impl
			Next i
			Return Nothing
		End Function

		''' <summary>
		''' Return a list of implementations that support the
		''' desired features.
		''' </summary>
		''' <param name="features">
		'''            A string that specifies which features are required. This is
		'''            a space separated list in which each feature is specified by
		'''            its name optionally followed by a space and a version number.
		'''            This is something like: "XML 1.0 Traversal +Events 2.0" </param>
		''' <returns> A list of DOMImplementations that support the desired features. </returns>
		Public Function getDOMImplementationList(ByVal features As String) As org.w3c.dom.DOMImplementationList
			Dim implementations As New ArrayList
			Dim size As Integer = sources.Count
			For i As Integer = 0 To size - 1
				Dim source As org.w3c.dom.DOMImplementationSource = CType(sources(i), org.w3c.dom.DOMImplementationSource)
				Dim impls As org.w3c.dom.DOMImplementationList = source.getDOMImplementationList(features)
				For j As Integer = 0 To impls.length - 1
					Dim impl As org.w3c.dom.DOMImplementation = impls.item(j)
					implementations.Add(impl)
				Next j
			Next i
			Return New DOMImplementationListAnonymousInnerClassHelper
		End Function

		Private Class DOMImplementationListAnonymousInnerClassHelper
			Implements org.w3c.dom.DOMImplementationList

			Public Overridable Function item(ByVal index As Integer) As org.w3c.dom.DOMImplementation
				If index >= 0 AndAlso index < implementations.size() Then
					Try
						Return CType(implementations.elementAt(index), org.w3c.dom.DOMImplementation)
					Catch e As System.IndexOutOfRangeException
						Return Nothing
					End Try
				End If
				Return Nothing
			End Function

			Public Overridable Property length As Integer
				Get
					Return implementations.size()
				End Get
			End Property
		End Class

		''' <summary>
		''' Register an implementation.
		''' </summary>
		''' <param name="s"> The source to be registered, may not be <code>null</code> </param>
		Public Sub addSource(ByVal s As org.w3c.dom.DOMImplementationSource)
			If s Is Nothing Then Throw New NullPointerException
			If Not sources.Contains(s) Then sources.Add(s)
		End Sub

		''' 
		''' <summary>
		''' Gets a class loader.
		''' </summary>
		''' <returns> A class loader, possibly <code>null</code> </returns>
		Private Property Shared classLoader As ClassLoader
			Get
				Try
					Dim contextClassLoader_Renamed As ClassLoader = contextClassLoader
    
					If contextClassLoader_Renamed IsNot Nothing Then Return contextClassLoader_Renamed
				Catch e As Exception
					' Assume that the DOM application is in a JRE 1.1, use the
					' current ClassLoader
					Return GetType(DOMImplementationRegistry).classLoader
				End Try
				Return GetType(DOMImplementationRegistry).classLoader
			End Get
		End Property

		''' <summary>
		''' This method attempts to return the first line of the resource
		''' META_INF/services/org.w3c.dom.DOMImplementationSourceList
		''' from the provided ClassLoader.
		''' </summary>
		''' <param name="classLoader"> classLoader, may not be <code>null</code>. </param>
		''' <returns> first line of resource, or <code>null</code> </returns>
		Private Shared Function getServiceValue(ByVal classLoader As ClassLoader) As String
			Dim serviceId As String = "META-INF/services/" & [PROPERTY]
			' try to find services in CLASSPATH
			Try
				Dim [is] As java.io.InputStream = getResourceAsStream(classLoader, serviceId)

				If [is] IsNot Nothing Then
					Dim rd As java.io.BufferedReader
					Try
						rd = New java.io.BufferedReader(New java.io.InputStreamReader([is], "UTF-8"), DEFAULT_LINE_LENGTH)
					Catch e As java.io.UnsupportedEncodingException
						rd = New java.io.BufferedReader(New java.io.InputStreamReader([is]), DEFAULT_LINE_LENGTH)
					End Try
					Dim serviceValue_Renamed As String = rd.readLine()
					rd.close()
					If serviceValue_Renamed IsNot Nothing AndAlso serviceValue_Renamed.Length > 0 Then Return serviceValue_Renamed
				End If
			Catch ex As Exception
				Return Nothing
			End Try
			Return Nothing
		End Function

		''' <summary>
		''' A simple JRE (Java Runtime Environment) 1.1 test
		''' </summary>
		''' <returns> <code>true</code> if JRE 1.1 </returns>
		Private Property Shared jRE11 As Boolean
			Get
				Try
					Dim c As Type = Type.GetType("java.security.AccessController")
					' java.security.AccessController existed since 1.2 so, if no
					' exception was thrown, the DOM application is running in a JRE
					' 1.2 or higher
					Return False
				Catch ex As Exception
					' ignore
				End Try
				Return True
			End Get
		End Property

		''' <summary>
		''' This method returns the ContextClassLoader or <code>null</code> if
		''' running in a JRE 1.1
		''' </summary>
		''' <returns> The Context Classloader </returns>
		Private Property Shared contextClassLoader As ClassLoader
			Get
	'JAVA TO VB CONVERTER TODO TASK: Anonymous inner classes are not converted to VB if the base type is not defined in the code being converted:
	'			Return isJRE11() ? Nothing : (ClassLoader) java.security.AccessController.doPrivileged(New java.security.PrivilegedAction()
		'		{
		'					public Object run()
		'					{
		'						ClassLoader classLoader = Nothing;
		'						try
		'						{
		'							classLoader = Thread.currentThread().getContextClassLoader();
		'						}
		'						catch (SecurityException ex)
		'						{
		'						}
		'						Return classLoader;
		'					}
		'				});
			End Get
		End Property

		''' <summary>
		''' This method returns the system property indicated by the specified name
		''' after checking access control privileges. For a JRE 1.1, this check is
		''' not done.
		''' </summary>
		''' <param name="name"> the name of the system property </param>
		''' <returns> the system property </returns>
		Private Shared Function getSystemProperty(ByVal name As String) As String
'JAVA TO VB CONVERTER TODO TASK: Anonymous inner classes are not converted to VB if the base type is not defined in the code being converted:
'			Return isJRE11() ? (String) System.getProperty(name) : (String) java.security.AccessController.doPrivileged(New java.security.PrivilegedAction()
	'		{
	'					public Object run()
	'					{
	'						Return System.getProperty(name);
	'					}
	'				});
		End Function

		''' <summary>
		''' This method returns an Inputstream for the reading resource
		''' META_INF/services/org.w3c.dom.DOMImplementationSourceList after checking
		''' access control privileges. For a JRE 1.1, this check is not done.
		''' </summary>
		''' <param name="classLoader"> classLoader </param>
		''' <param name="name"> the resource </param>
		''' <returns> an Inputstream for the resource specified </returns>
		Private Shared Function getResourceAsStream(ByVal classLoader As ClassLoader, ByVal name As String) As java.io.InputStream
			If jRE11 Then
				Dim ris As java.io.InputStream
				If classLoader Is Nothing Then
					ris = ClassLoader.getSystemResourceAsStream(name)
				Else
					ris = classLoader.getResourceAsStream(name)
				End If
				Return ris
			Else
'JAVA TO VB CONVERTER TODO TASK: Anonymous inner classes are not converted to VB if the base type is not defined in the code being converted:
'				Return (java.io.InputStream) java.security.AccessController.doPrivileged(New java.security.PrivilegedAction()
	'			{
	'						public Object run()
	'						{
	'							InputStream ris;
	'							if (classLoader == Nothing)
	'							{
	'								ris = ClassLoader.getSystemResourceAsStream(name);
	'							}
	'							else
	'							{
	'								ris = classLoader.getResourceAsStream(name);
	'							}
	'							Return ris;
	'						}
	'					});
			End If
		End Function
	End Class

End Namespace