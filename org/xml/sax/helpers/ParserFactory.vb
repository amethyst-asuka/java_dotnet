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

' SAX parser factory.
' http://www.saxproject.org
' No warranty; no copyright -- use this as you will.
' $Id: ParserFactory.java,v 1.2 2004/11/03 22:53:09 jsuttor Exp $

Namespace org.xml.sax.helpers



	''' <summary>
	''' Java-specific class for dynamically loading SAX parsers.
	''' 
	''' <blockquote>
	''' <em>This module, both source code and documentation, is in the
	''' Public Domain, and comes with <strong>NO WARRANTY</strong>.</em>
	''' See <a href='http://www.saxproject.org'>http://www.saxproject.org</a>
	''' for further information.
	''' </blockquote>
	''' 
	''' <p><strong>Note:</strong> This class is designed to work with the now-deprecated
	''' SAX1 <seealso cref="org.xml.sax.Parser Parser"/> class.  SAX2 applications should use
	''' <seealso cref="org.xml.sax.helpers.XMLReaderFactory XMLReaderFactory"/> instead.</p>
	''' 
	''' <p>ParserFactory is not part of the platform-independent definition
	''' of SAX; it is an additional convenience class designed
	''' specifically for Java XML application writers.  SAX applications
	''' can use the static methods in this class to allocate a SAX parser
	''' dynamically at run-time based either on the value of the
	''' `org.xml.sax.parser' system property or on a string containing the class
	''' name.</p>
	''' 
	''' <p>Note that the application still requires an XML parser that
	''' implements SAX1.</p>
	''' </summary>
	''' @deprecated This class works with the deprecated
	'''             <seealso cref="org.xml.sax.Parser Parser"/>
	'''             interface.
	''' @since SAX 1.0
	''' @author David Megginson
	''' @version 2.0.1 (sax2r2) 
	Public Class ParserFactory
		Private Shared ss As New SecuritySupport

		''' <summary>
		''' Private null constructor.
		''' </summary>
		Private Sub New()
		End Sub


		''' <summary>
		''' Create a new SAX parser using the `org.xml.sax.parser' system property.
		''' 
		''' <p>The named class must exist and must implement the
		''' <seealso cref="org.xml.sax.Parser Parser"/> interface.</p>
		''' </summary>
		''' <exception cref="java.lang.NullPointerException"> There is no value
		'''            for the `org.xml.sax.parser' system property. </exception>
		''' <exception cref="java.lang.ClassNotFoundException"> The SAX parser
		'''            class was not found (check your CLASSPATH). </exception>
		''' <exception cref="IllegalAccessException"> The SAX parser class was
		'''            found, but you do not have permission to load
		'''            it. </exception>
		''' <exception cref="InstantiationException"> The SAX parser class was
		'''            found but could not be instantiated. </exception>
		''' <exception cref="java.lang.ClassCastException"> The SAX parser class
		'''            was found and instantiated, but does not implement
		'''            org.xml.sax.Parser. </exception>
		''' <seealso cref= #makeParser(java.lang.String) </seealso>
		''' <seealso cref= org.xml.sax.Parser </seealso>
		Public Shared Function makeParser() As org.xml.sax.Parser
			Dim className As String = ss.getSystemProperty("org.xml.sax.parser")
			If className Is Nothing Then
				Throw New NullPointerException("No value for sax.parser property")
			Else
				Return makeParser(className)
			End If
		End Function


		''' <summary>
		''' Create a new SAX parser object using the class name provided.
		''' 
		''' <p>The named class must exist and must implement the
		''' <seealso cref="org.xml.sax.Parser Parser"/> interface.</p>
		''' </summary>
		''' <param name="className"> A string containing the name of the
		'''                  SAX parser class. </param>
		''' <exception cref="java.lang.ClassNotFoundException"> The SAX parser
		'''            class was not found (check your CLASSPATH). </exception>
		''' <exception cref="IllegalAccessException"> The SAX parser class was
		'''            found, but you do not have permission to load
		'''            it. </exception>
		''' <exception cref="InstantiationException"> The SAX parser class was
		'''            found but could not be instantiated. </exception>
		''' <exception cref="java.lang.ClassCastException"> The SAX parser class
		'''            was found and instantiated, but does not implement
		'''            org.xml.sax.Parser. </exception>
		''' <seealso cref= #makeParser() </seealso>
		''' <seealso cref= org.xml.sax.Parser </seealso>
		Public Shared Function makeParser(ByVal className As String) As org.xml.sax.Parser
			Return CType(NewInstance.newInstance(ss.contextClassLoader, className), org.xml.sax.Parser)
		End Function

	End Class

End Namespace