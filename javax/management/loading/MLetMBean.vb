Imports System.Collections.Generic
Imports javax.management

'
' * Copyright (c) 1999, 2006, Oracle and/or its affiliates. All rights reserved.
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




	''' <summary>
	''' Exposes the remote management interface of the MLet
	''' MBean.
	''' 
	''' @since 1.5
	''' </summary>
	Public Interface MLetMBean


		''' <summary>
		''' Loads a text file containing MLET tags that define the MBeans
		''' to be added to the MBean server. The location of the text file is
		''' specified by a URL. The text file is read using the UTF-8
		''' encoding. The MBeans specified in the MLET file will be
		''' instantiated and registered in the MBean server.
		''' </summary>
		''' <param name="url"> The URL of the text file to be loaded as String object.
		''' </param>
		''' <returns> A set containing one entry per MLET tag in the m-let
		''' text file loaded.  Each entry specifies either the
		''' ObjectInstance for the created MBean, or a throwable object
		''' (that is, an error or an exception) if the MBean could not be
		''' created.
		''' </returns>
		''' <exception cref="ServiceNotFoundException"> One of the following errors
		''' has occurred: The m-let text file does not contain an MLET tag,
		''' the m-let text file is not found, a mandatory attribute of the
		''' MLET tag is not specified, the value of url is malformed. </exception>
		Function getMBeansFromURL(ByVal url As String) As java.util.Set(Of Object)

		''' <summary>
		''' Loads a text file containing MLET tags that define the MBeans
		''' to be added to the MBean server. The location of the text file is
		''' specified by a URL. The text file is read using the UTF-8
		''' encoding. The MBeans specified in the MLET file will be
		''' instantiated and registered in the MBean server.
		''' </summary>
		''' <param name="url"> The URL of the text file to be loaded as URL object.
		''' </param>
		''' <returns> A set containing one entry per MLET tag in the m-let
		''' text file loaded.  Each entry specifies either the
		''' ObjectInstance for the created MBean, or a throwable object
		''' (that is, an error or an exception) if the MBean could not be
		''' created.
		''' </returns>
		''' <exception cref="ServiceNotFoundException"> One of the following errors
		''' has occurred: The m-let text file does not contain an MLET tag,
		''' the m-let text file is not found, a mandatory attribute of the
		''' MLET tag is not specified, the value of url is null. </exception>
		Function getMBeansFromURL(ByVal url As java.net.URL) As java.util.Set(Of Object)

		''' <summary>
		''' Appends the specified URL to the list of URLs to search for classes and
		''' resources.
		''' </summary>
		''' <param name="url"> the URL to add. </param>
		Sub addURL(ByVal url As java.net.URL)

		''' <summary>
		''' Appends the specified URL to the list of URLs to search for classes and
		''' resources.
		''' </summary>
		''' <param name="url"> the URL to add.
		''' </param>
		''' <exception cref="ServiceNotFoundException"> The specified URL is malformed. </exception>
		Sub addURL(ByVal url As String)

		''' <summary>
		''' Returns the search path of URLs for loading classes and resources.
		''' This includes the original list of URLs specified to the constructor,
		''' along with any URLs subsequently appended by the addURL() method.
		''' </summary>
		''' <returns> the list of URLs. </returns>
		ReadOnly Property uRLs As java.net.URL()

		''' <summary>
		''' Finds the resource with the given name.
		''' A resource is some data (images, audio, text, etc) that can be accessed by class code in a way that is
		'''   independent of the location of the code.
		'''   The name of a resource is a "/"-separated path name that identifies the resource.
		''' </summary>
		''' <param name="name"> The resource name
		''' </param>
		''' <returns>  An URL for reading the resource, or null if the resource could not be found or the caller doesn't have adequate privileges to get the
		''' resource. </returns>
		Function getResource(ByVal name As String) As java.net.URL

		''' <summary>
		''' Returns an input stream for reading the specified resource. The search order is described in the documentation for
		'''  getResource(String).
		''' </summary>
		''' <param name="name">  The resource name
		''' </param>
		''' <returns> An input stream for reading the resource, or null if the resource could not be found
		'''  </returns>
		Function getResourceAsStream(ByVal name As String) As java.io.InputStream

		''' <summary>
		''' Finds all the resources with the given name. A resource is some
		''' data (images, audio, text, etc) that can be accessed by class
		''' code in a way that is independent of the location of the code.
		''' The name of a resource is a "/"-separated path name that
		''' identifies the resource.
		''' </summary>
		''' <param name="name"> The  resource name.
		''' </param>
		''' <returns> An enumeration of URL to the resource. If no resources
		''' could be found, the enumeration will be empty. Resources that
		''' cannot be accessed will not be in the enumeration.
		''' </returns>
		''' <exception cref="IOException"> if an I/O exception occurs when
		''' searching for resources. </exception>
		Function getResources(ByVal name As String) As System.Collections.IEnumerator(Of java.net.URL)

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
		Property libraryDirectory As String


	End Interface

End Namespace