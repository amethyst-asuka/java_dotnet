'
' * Copyright (c) 2003, 2005, Oracle and/or its affiliates. All rights reserved.
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

Namespace javax.xml.xpath


	''' <summary>
	''' <p>XPath constants.</p>
	''' 
	''' @author <a href="mailto:Norman.Walsh@Sun.COM">Norman Walsh</a>
	''' @author <a href="mailto:Jeff.Suttor@Sun.COM">Jeff Suttor</a> </summary>
	''' <seealso cref= <a href="http://www.w3.org/TR/xpath">XML Path Language (XPath) Version 1.0</a>
	''' @since 1.5 </seealso>
	Public Class XPathConstants

		''' <summary>
		''' <p>Private constructor to prevent instantiation.</p>
		''' </summary>
		Private Sub New()
		End Sub

		''' <summary>
		''' <p>The XPath 1.0 number data type.</p>
		''' 
		''' <p>Maps to Java <seealso cref="Double"/>.</p>
		''' </summary>
		Public Shared ReadOnly NUMBER As New javax.xml.namespace.QName("http://www.w3.org/1999/XSL/Transform", "NUMBER")

		''' <summary>
		''' <p>The XPath 1.0 string data type.</p>
		''' 
		''' <p>Maps to Java <seealso cref="String"/>.</p>
		''' </summary>
		Public Shared ReadOnly [STRING] As New javax.xml.namespace.QName("http://www.w3.org/1999/XSL/Transform", "STRING")

		''' <summary>
		''' <p>The XPath 1.0 boolean data type.</p>
		''' 
		''' <p>Maps to Java <seealso cref="Boolean"/>.</p>
		''' </summary>
		Public Shared ReadOnly [BOOLEAN] As New javax.xml.namespace.QName("http://www.w3.org/1999/XSL/Transform", "BOOLEAN")

		''' <summary>
		''' <p>The XPath 1.0 NodeSet data type.</p>
		''' 
		''' <p>Maps to Java <seealso cref="org.w3c.dom.NodeList"/>.</p>
		''' </summary>
		Public Shared ReadOnly NODESET As New javax.xml.namespace.QName("http://www.w3.org/1999/XSL/Transform", "NODESET")

		''' <summary>
		''' <p>The XPath 1.0 NodeSet data type.
		''' 
		''' <p>Maps to Java <seealso cref="org.w3c.dom.Node"/>.</p>
		''' </summary>
		Public Shared ReadOnly NODE As New javax.xml.namespace.QName("http://www.w3.org/1999/XSL/Transform", "NODE")

		''' <summary>
		''' <p>The URI for the DOM object model, "http://java.sun.com/jaxp/xpath/dom".</p>
		''' </summary>
		Public Const DOM_OBJECT_MODEL As String = "http://java.sun.com/jaxp/xpath/dom"
	End Class

End Namespace