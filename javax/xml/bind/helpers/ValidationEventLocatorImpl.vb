Imports System

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

Namespace javax.xml.bind.helpers



	''' <summary>
	''' Default implementation of the ValidationEventLocator interface.
	''' 
	''' <p>
	''' JAXB providers are allowed to use whatever class that implements
	''' the ValidationEventLocator interface. This class is just provided for a
	''' convenience.
	''' 
	''' @author <ul><li>Kohsuke Kawaguchi, Sun Microsystems, Inc.</li></ul> </summary>
	''' <seealso cref= javax.xml.bind.Validator </seealso>
	''' <seealso cref= javax.xml.bind.ValidationEventHandler </seealso>
	''' <seealso cref= javax.xml.bind.ValidationEvent </seealso>
	''' <seealso cref= javax.xml.bind.ValidationEventLocator
	''' @since JAXB1.0 </seealso>
	Public Class ValidationEventLocatorImpl
		Implements javax.xml.bind.ValidationEventLocator

		''' <summary>
		''' Creates an object with all fields unavailable.
		''' </summary>
		Public Sub New()
		End Sub

		''' <summary>
		''' Constructs an object from an org.xml.sax.Locator.
		''' 
		''' The object's ColumnNumber, LineNumber, and URL become available from the
		''' values returned by the locator's getColumnNumber(), getLineNumber(), and
		''' getSystemId() methods respectively. Node, Object, and Offset are not
		''' available.
		''' </summary>
		''' <param name="loc"> the SAX Locator object that will be used to populate this
		''' event locator. </param>
		''' <exception cref="IllegalArgumentException"> if the Locator is null </exception>
		Public Sub New(ByVal loc As org.xml.sax.Locator)
			If loc Is Nothing Then Throw New System.ArgumentException(Messages.format(Messages.MUST_NOT_BE_NULL, "loc"))

			Me.url = toURL(loc.systemId)
			Me.columnNumber = loc.columnNumber
			Me.lineNumber = loc.lineNumber
		End Sub

		''' <summary>
		''' Constructs an object from the location information of a SAXParseException.
		''' 
		''' The object's ColumnNumber, LineNumber, and URL become available from the
		''' values returned by the locator's getColumnNumber(), getLineNumber(), and
		''' getSystemId() methods respectively. Node, Object, and Offset are not
		''' available.
		''' </summary>
		''' <param name="e"> the SAXParseException object that will be used to populate this
		''' event locator. </param>
		''' <exception cref="IllegalArgumentException"> if the SAXParseException is null </exception>
		Public Sub New(ByVal e As org.xml.sax.SAXParseException)
			If e Is Nothing Then Throw New System.ArgumentException(Messages.format(Messages.MUST_NOT_BE_NULL, "e"))

			Me.url = toURL(e.systemId)
			Me.columnNumber = e.columnNumber
			Me.lineNumber = e.lineNumber
		End Sub

		''' <summary>
		''' Constructs an object that points to a DOM Node.
		''' 
		''' The object's Node becomes available.  ColumnNumber, LineNumber, Object,
		''' Offset, and URL are not available.
		''' </summary>
		''' <param name="_node"> the DOM Node object that will be used to populate this
		''' event locator. </param>
		''' <exception cref="IllegalArgumentException"> if the Node is null </exception>
		Public Sub New(ByVal _node As org.w3c.dom.Node)
			If _node Is Nothing Then Throw New System.ArgumentException(Messages.format(Messages.MUST_NOT_BE_NULL, "_node"))

			Me.node = _node
		End Sub

		''' <summary>
		''' Constructs an object that points to a JAXB content object.
		''' 
		''' The object's Object becomes available. ColumnNumber, LineNumber, Node,
		''' Offset, and URL are not available.
		''' </summary>
		''' <param name="_object"> the Object that will be used to populate this
		''' event locator. </param>
		''' <exception cref="IllegalArgumentException"> if the Object is null </exception>
		Public Sub New(ByVal _object As Object)
			If _object Is Nothing Then Throw New System.ArgumentException(Messages.format(Messages.MUST_NOT_BE_NULL, "_object"))

			Me.object = _object
		End Sub

		''' <summary>
		''' Converts a system ID to an URL object. </summary>
		Private Shared Function toURL(ByVal systemId As String) As java.net.URL
			Try
				Return New java.net.URL(systemId)
			Catch e As java.net.MalformedURLException
				' TODO: how should we handle system id here?
				Return Nothing ' for now
			End Try
		End Function

		Private url As java.net.URL = Nothing
		Private offset As Integer = -1
		Private lineNumber As Integer = -1
		Private columnNumber As Integer = -1
		Private [object] As Object = Nothing
		Private node As org.w3c.dom.Node = Nothing


		''' <seealso cref= javax.xml.bind.ValidationEventLocator#getURL() </seealso>
		Public Overridable Property uRL As java.net.URL
			Get
				Return url
			End Get
			Set(ByVal _url As java.net.URL)
				Me.url = _url
			End Set
		End Property


		''' <seealso cref= javax.xml.bind.ValidationEventLocator#getOffset() </seealso>
		Public Overridable Property offset As Integer
			Get
				Return offset
			End Get
			Set(ByVal _offset As Integer)
				Me.offset = _offset
			End Set
		End Property


		''' <seealso cref= javax.xml.bind.ValidationEventLocator#getLineNumber() </seealso>
		Public Overridable Property lineNumber As Integer
			Get
				Return lineNumber
			End Get
			Set(ByVal _lineNumber As Integer)
				Me.lineNumber = _lineNumber
			End Set
		End Property


		''' <seealso cref= javax.xml.bind.ValidationEventLocator#getColumnNumber() </seealso>
		Public Overridable Property columnNumber As Integer
			Get
				Return columnNumber
			End Get
			Set(ByVal _columnNumber As Integer)
				Me.columnNumber = _columnNumber
			End Set
		End Property


		''' <seealso cref= javax.xml.bind.ValidationEventLocator#getObject() </seealso>
		Public Overridable Property [object] As Object
			Get
				Return [object]
			End Get
			Set(ByVal _object As Object)
				Me.object = _object
			End Set
		End Property


		''' <seealso cref= javax.xml.bind.ValidationEventLocator#getNode() </seealso>
		Public Overridable Property node As org.w3c.dom.Node
			Get
				Return node
			End Get
			Set(ByVal _node As org.w3c.dom.Node)
				Me.node = _node
			End Set
		End Property


		''' <summary>
		''' Returns a string representation of this object in a format
		''' helpful to debugging.
		''' </summary>
		''' <seealso cref= Object#equals(Object) </seealso>
		Public Overrides Function ToString() As String
			Return java.text.MessageFormat.format("[node={0},object={1},url={2},line={3},col={4},offset={5}]", node, [object], uRL, Convert.ToString(lineNumber), Convert.ToString(columnNumber), Convert.ToString(offset))
		End Function
	End Class

End Namespace