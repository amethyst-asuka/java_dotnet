'
' * Copyright (c) 2003, 2013, Oracle and/or its affiliates. All rights reserved.
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

Namespace javax.xml.bind

	''' <summary>
	''' Encapsulate the location of a ValidationEvent.
	''' 
	''' <p>
	''' The <tt>ValidationEventLocator</tt> indicates where the <tt>ValidationEvent
	''' </tt> occurred.  Different fields will be set depending on the type of
	''' validation that was being performed when the error or warning was detected.
	''' For example, on-demand validation would produce locators that contained
	''' references to objects in the Java content tree while unmarshal-time
	''' validation would produce locators containing information appropriate to the
	''' source of the XML data (file, url, Node, etc).
	''' 
	''' @author <ul><li>Ryan Shoemaker, Sun Microsystems, Inc.</li><li>Kohsuke Kawaguchi, Sun Microsystems, Inc.</li><li>Joe Fialli, Sun Microsystems, Inc.</li></ul> </summary>
	''' <seealso cref= Validator </seealso>
	''' <seealso cref= ValidationEvent
	''' @since JAXB1.0 </seealso>
	Public Interface ValidationEventLocator

		''' <summary>
		''' Return the name of the XML source as a URL if available
		''' </summary>
		''' <returns> the name of the XML source as a URL or null if unavailable </returns>
		ReadOnly Property uRL As java.net.URL

		''' <summary>
		''' Return the byte offset if available
		''' </summary>
		''' <returns> the byte offset into the input source or -1 if unavailable </returns>
		ReadOnly Property offset As Integer

		''' <summary>
		''' Return the line number if available
		''' </summary>
		''' <returns> the line number or -1 if unavailable </returns>
		ReadOnly Property lineNumber As Integer

		''' <summary>
		''' Return the column number if available
		''' </summary>
		''' <returns> the column number or -1 if unavailable </returns>
		ReadOnly Property columnNumber As Integer

		''' <summary>
		''' Return a reference to the object in the Java content tree if available
		''' </summary>
		''' <returns> a reference to the object in the Java content tree or null if
		'''         unavailable </returns>
		ReadOnly Property [object] As Object

		''' <summary>
		''' Return a reference to the DOM Node if available
		''' </summary>
		''' <returns> a reference to the DOM Node or null if unavailable </returns>
		ReadOnly Property node As org.w3c.dom.Node

	End Interface

End Namespace