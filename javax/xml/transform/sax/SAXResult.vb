'
' * Copyright (c) 2000, 2005, Oracle and/or its affiliates. All rights reserved.
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

Namespace javax.xml.transform.sax



	''' <summary>
	''' <p>Acts as an holder for a transformation Result.</p>
	''' 
	''' @author <a href="Jeff.Suttor@Sun.com">Jeff Suttor</a>
	''' </summary>
	Public Class SAXResult
		Implements javax.xml.transform.Result

		''' <summary>
		''' If <seealso cref="javax.xml.transform.TransformerFactory#getFeature"/>
		''' returns true when passed this value as an argument,
		''' the Transformer supports Result output of this type.
		''' </summary>
		Public Const FEATURE As String = "http://javax.xml.transform.sax.SAXResult/feature"

		''' <summary>
		''' Zero-argument default constructor.
		''' </summary>
		Public Sub New()
		End Sub

		''' <summary>
		''' Create a SAXResult that targets a SAX2 <seealso cref="org.xml.sax.ContentHandler"/>.
		''' </summary>
		''' <param name="handler"> Must be a non-null ContentHandler reference. </param>
		Public Sub New(ByVal handler As org.xml.sax.ContentHandler)
			handler = handler
		End Sub

		''' <summary>
		''' Set the target to be a SAX2 <seealso cref="org.xml.sax.ContentHandler"/>.
		''' </summary>
		''' <param name="handler"> Must be a non-null ContentHandler reference. </param>
		Public Overridable Property handler As org.xml.sax.ContentHandler
			Set(ByVal handler As org.xml.sax.ContentHandler)
				Me.handler = handler
			End Set
			Get
				Return handler
			End Get
		End Property


		''' <summary>
		''' Set the SAX2 <seealso cref="org.xml.sax.ext.LexicalHandler"/> for the output.
		''' 
		''' <p>This is needed to handle XML comments and the like.  If the
		''' lexical handler is not set, an attempt should be made by the
		''' transformer to cast the <seealso cref="org.xml.sax.ContentHandler"/> to a
		''' <code>LexicalHandler</code>.</p>
		''' </summary>
		''' <param name="handler"> A non-null <code>LexicalHandler</code> for
		''' handling lexical parse events. </param>
		Public Overridable Property lexicalHandler As org.xml.sax.ext.LexicalHandler
			Set(ByVal handler As org.xml.sax.ext.LexicalHandler)
				Me.lexhandler = handler
			End Set
			Get
				Return lexhandler
			End Get
		End Property


		''' <summary>
		''' Method setSystemId Set the systemID that may be used in association
		''' with the <seealso cref="org.xml.sax.ContentHandler"/>.
		''' </summary>
		''' <param name="systemId"> The system identifier as a URI string. </param>
		Public Overridable Property systemId As String
			Set(ByVal systemId As String)
				Me.systemId = systemId
			End Set
			Get
				Return systemId
			End Get
		End Property


		'////////////////////////////////////////////////////////////////////
		' Internal state.
		'////////////////////////////////////////////////////////////////////

		''' <summary>
		''' The handler for parse events.
		''' </summary>
		Private handler As org.xml.sax.ContentHandler

		''' <summary>
		''' The handler for lexical events.
		''' </summary>
		Private lexhandler As org.xml.sax.ext.LexicalHandler

		''' <summary>
		''' The systemID that may be used in association
		''' with the node.
		''' </summary>
		Private systemId As String
	End Class

End Namespace