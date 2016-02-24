Imports System

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

' XMLReaderFactory.java - factory for creating a new reader.
' http://www.saxproject.org
' Written by David Megginson
' and by David Brownell
' NO WARRANTY!  This class is in the Public Domain.
' $Id: XMLReaderFactory.java,v 1.2.2.1 2005/07/31 22:48:08 jeffsuttor Exp $

Namespace org.xml.sax.helpers


	''' <summary>
	''' Factory for creating an XML reader.
	''' 
	''' <blockquote>
	''' <em>This module, both source code and documentation, is in the
	''' Public Domain, and comes with <strong>NO WARRANTY</strong>.</em>
	''' See <a href='http://www.saxproject.org'>http://www.saxproject.org</a>
	''' for further information.
	''' </blockquote>
	''' 
	''' <p>This class contains static methods for creating an XML reader
	''' from an explicit class name, or based on runtime defaults:</p>
	''' 
	''' <pre>
	''' try {
	'''   XMLReader myReader = XMLReaderFactory.createXMLReader();
	''' } catch (SAXException e) {
	'''   System.err.println(e.getMessage());
	''' }
	''' </pre>
	''' 
	''' <p><strong>Note to Distributions bundled with parsers:</strong>
	''' You should modify the implementation of the no-arguments
	''' <em>createXMLReader</em> to handle cases where the external
	''' configuration mechanisms aren't set up.  That method should do its
	''' best to return a parser when one is in the class path, even when
	''' nothing bound its class name to <code>org.xml.sax.driver</code> so
	''' those configuration mechanisms would see it.</p>
	''' 
	''' @since SAX 2.0
	''' @author David Megginson, David Brownell
	''' @version 2.0.1 (sax2r2)
	''' </summary>
	Public NotInheritable Class XMLReaderFactory
		''' <summary>
		''' Private constructor.
		''' 
		''' <p>This constructor prevents the class from being instantiated.</p>
		''' </summary>
		Private Sub New()
		End Sub

		Private Const [property] As String = "org.xml.sax.driver"
		Private Shared ss As New SecuritySupport

		Private Shared _clsFromJar As String = Nothing
		Private Shared _jarread As Boolean = False
		''' <summary>
		''' Attempt to create an XMLReader from system defaults.
		''' In environments which can support it, the name of the XMLReader
		''' class is determined by trying each these options in order, and
		''' using the first one which succeeds:</p> <ul>
		''' 
		''' <li>If the system property <code>org.xml.sax.driver</code>
		''' has a value, that is used as an XMLReader class name. </li>
		''' 
		''' <li>The JAR "Services API" is used to look for a class name
		''' in the <em>META-INF/services/org.xml.sax.driver</em> file in
		''' jarfiles available to the runtime.</li>
		''' 
		''' <li> SAX parser distributions are strongly encouraged to provide
		''' a default XMLReader class name that will take effect only when
		''' previous options (on this list) are not successful.</li>
		''' 
		''' <li>Finally, if <seealso cref="ParserFactory#makeParser()"/> can
		''' return a system default SAX1 parser, that parser is wrapped in
		''' a <seealso cref="ParserAdapter"/>.  (This is a migration aid for SAX1
		''' environments, where the <code>org.xml.sax.parser</code> system
		''' property will often be usable.) </li>
		''' 
		''' </ul>
		''' 
		''' <p> In environments such as small embedded systems, which can not
		''' support that flexibility, other mechanisms to determine the default
		''' may be used. </p>
		''' 
		''' <p>Note that many Java environments allow system properties to be
		''' initialized on a command line.  This means that <em>in most cases</em>
		''' setting a good value for that property ensures that calls to this
		''' method will succeed, except when security policies intervene.
		''' This will also maximize application portability to older SAX
		''' environments, with less robust implementations of this method.
		''' </p>
		''' </summary>
		''' <returns> A new XMLReader. </returns>
		''' <exception cref="org.xml.sax.SAXException"> If no default XMLReader class
		'''            can be identified and instantiated. </exception>
		''' <seealso cref= #createXMLReader(java.lang.String) </seealso>
		Public Shared Function createXMLReader() As org.xml.sax.XMLReader
			Dim className As String = Nothing
			Dim cl As ClassLoader = ss.contextClassLoader

			' 1. try the JVM-instance-wide system property
			Try
				className = ss.getSystemProperty([property])
			Catch e As Exception ' continue searching
			End Try

			' 2. if that fails, try META-INF/services/
			If className Is Nothing Then
				If Not _jarread Then
					_jarread = True
					Dim service As String = "META-INF/services/" & [property]
					Dim [in] As java.io.InputStream
					Dim reader As java.io.BufferedReader

					Try
						If cl IsNot Nothing Then
							[in] = ss.getResourceAsStream(cl, service)

							' If no provider found then try the current ClassLoader
							If [in] Is Nothing Then
								cl = Nothing
								[in] = ss.getResourceAsStream(cl, service)
							End If
						Else
							' No Context ClassLoader, try the current ClassLoader
							[in] = ss.getResourceAsStream(cl, service)
						End If

						If [in] IsNot Nothing Then
							reader = New java.io.BufferedReader(New java.io.InputStreamReader([in], "UTF8"))
							_clsFromJar = reader.readLine()
							[in].close()
						End If
					Catch e As Exception
					End Try
				End If
				className = _clsFromJar
			End If

			' 3. Distro-specific fallback
			If className Is Nothing Then className = "com.sun.org.apache.xerces.internal.parsers.SAXParser"

			' do we know the XMLReader implementation class yet?
			If className IsNot Nothing Then Return loadClass(cl, className)

			' 4. panic -- adapt any SAX1 parser
			Try
				Return New ParserAdapter(ParserFactory.makeParser())
			Catch e As Exception
				Throw New org.xml.sax.SAXException("Can't create default XMLReader; " & "is system property org.xml.sax.driver set?")
			End Try
		End Function


		''' <summary>
		''' Attempt to create an XML reader from a class name.
		''' 
		''' <p>Given a class name, this method attempts to load
		''' and instantiate the class as an XML reader.</p>
		''' 
		''' <p>Note that this method will not be usable in environments where
		''' the caller (perhaps an applet) is not permitted to load classes
		''' dynamically.</p>
		''' </summary>
		''' <returns> A new XML reader. </returns>
		''' <exception cref="org.xml.sax.SAXException"> If the class cannot be
		'''            loaded, instantiated, and cast to XMLReader. </exception>
		''' <seealso cref= #createXMLReader() </seealso>
		Public Shared Function createXMLReader(ByVal className As String) As org.xml.sax.XMLReader
			Return loadClass(ss.contextClassLoader, className)
		End Function

		Private Shared Function loadClass(ByVal loader As ClassLoader, ByVal className As String) As org.xml.sax.XMLReader
			Try
				Return CType(NewInstance.newInstance(loader, className), org.xml.sax.XMLReader)
			Catch e1 As ClassNotFoundException
				Throw New org.xml.sax.SAXException("SAX2 driver class " & className & " not found", e1)
			Catch e2 As IllegalAccessException
				Throw New org.xml.sax.SAXException("SAX2 driver class " & className & " found but cannot be loaded", e2)
			Catch e3 As InstantiationException
				Throw New org.xml.sax.SAXException("SAX2 driver class " & className & " loaded but cannot be instantiated (no empty public constructor?)", e3)
			Catch e4 As ClassCastException
				Throw New org.xml.sax.SAXException("SAX2 driver class " & className & " does not implement XMLReader", e4)
			End Try
		End Function
	End Class

End Namespace