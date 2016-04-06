Imports System

'
' * Copyright (c) 1996, 2004, Oracle and/or its affiliates. All rights reserved.
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

Namespace java.rmi.server


	''' <summary>
	''' <code>LoaderHandler</code> is an interface used internally by the RMI
	''' runtime in previous implementation versions.  It should never be accessed
	''' by application code.
	''' 
	''' @author  Ann Wollrath
	''' @since   JDK1.1
	''' </summary>
	''' @deprecated no replacement 
	<Obsolete("no replacement")> _
	Public Interface LoaderHandler

		''' <summary>
		''' package of system <code>LoaderHandler</code> implementation. </summary>
'JAVA TO VB CONVERTER TODO TASK: Interfaces cannot contain fields in .NET:
'		final static String packagePrefix = "sun.rmi.server";

		''' <summary>
		''' Loads a class from the location specified by the
		''' <code>java.rmi.server.codebase</code> property.
		''' </summary>
		''' <param name="name"> the name of the class to load </param>
		''' <returns> the <code>Class</code> object representing the loaded class </returns>
		''' <exception cref="MalformedURLException">
		'''            if the system property <b>java.rmi.server.codebase</b>
		'''            contains an invalid URL </exception>
		''' <exception cref="ClassNotFoundException">
		'''            if a definition for the class could not
		'''            be found at the codebase location.
		''' @since JDK1.1 </exception>
		''' @deprecated no replacement 
		<Obsolete("no replacement")> _
		Function loadClass(  name As String) As  [Class]

		''' <summary>
		''' Loads a class from a URL.
		''' </summary>
		''' <param name="codebase">  the URL from which to load the class </param>
		''' <param name="name">      the name of the class to load </param>
		''' <returns> the <code>Class</code> object representing the loaded class </returns>
		''' <exception cref="MalformedURLException">
		'''            if the <code>codebase</code> paramater
		'''            contains an invalid URL </exception>
		''' <exception cref="ClassNotFoundException">
		'''            if a definition for the class could not
		'''            be found at the specified URL
		''' @since JDK1.1 </exception>
		''' @deprecated no replacement 
		<Obsolete("no replacement")> _
		Function loadClass(  codebase As java.net.URL,   name As String) As  [Class]

		''' <summary>
		''' Returns the security context of the given class loader.
		''' </summary>
		''' <param name="loader">  a class loader from which to get the security context </param>
		''' <returns> the security context
		''' @since JDK1.1 </returns>
		''' @deprecated no replacement 
		<Obsolete("no replacement")> _
		Function getSecurityContext(  loader As  ClassLoader) As Object
	End Interface

End Namespace