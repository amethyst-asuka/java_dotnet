Imports System
Imports System.Text

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

' SAX exception class.
' http://www.saxproject.org
' No warranty; no copyright -- use this as you will.
' $Id: SAXParseException.java,v 1.2 2004/11/03 22:55:32 jsuttor Exp $

Namespace org.xml.sax

	''' <summary>
	''' Encapsulate an XML parse error or warning.
	''' 
	''' <blockquote>
	''' <em>This module, both source code and documentation, is in the
	''' Public Domain, and comes with <strong>NO WARRANTY</strong>.</em>
	''' See <a href='http://www.saxproject.org'>http://www.saxproject.org</a>
	''' for further information.
	''' </blockquote>
	''' 
	''' <p>This exception may include information for locating the error
	''' in the original XML document, as if it came from a <seealso cref="Locator"/>
	''' object.  Note that although the application
	''' will receive a SAXParseException as the argument to the handlers
	''' in the <seealso cref="org.xml.sax.ErrorHandler ErrorHandler"/> interface,
	''' the application is not actually required to throw the exception;
	''' instead, it can simply read the information in it and take a
	''' different action.</p>
	''' 
	''' <p>Since this exception is a subclass of {@link org.xml.sax.SAXException
	''' SAXException}, it inherits the ability to wrap another exception.</p>
	''' 
	''' @since SAX 1.0
	''' @author David Megginson
	''' @version 2.0.1 (sax2r2) </summary>
	''' <seealso cref= org.xml.sax.SAXException </seealso>
	''' <seealso cref= org.xml.sax.Locator </seealso>
	''' <seealso cref= org.xml.sax.ErrorHandler </seealso>
	Public Class SAXParseException
		Inherits SAXException


		'////////////////////////////////////////////////////////////////////
		' Constructors.
		'////////////////////////////////////////////////////////////////////


		''' <summary>
		''' Create a new SAXParseException from a message and a Locator.
		''' 
		''' <p>This constructor is especially useful when an application is
		''' creating its own exception from within a {@link org.xml.sax.ContentHandler
		''' ContentHandler} callback.</p>
		''' </summary>
		''' <param name="message"> The error or warning message. </param>
		''' <param name="locator"> The locator object for the error or warning (may be
		'''        null). </param>
		''' <seealso cref= org.xml.sax.Locator </seealso>
		Public Sub New(ByVal message As String, ByVal locator As Locator)
			MyBase.New(message)
			If locator IsNot Nothing Then
				init(locator.publicId, locator.systemId, locator.lineNumber, locator.columnNumber)
			Else
				init(Nothing, Nothing, -1, -1)
			End If
		End Sub


		''' <summary>
		''' Wrap an existing exception in a SAXParseException.
		''' 
		''' <p>This constructor is especially useful when an application is
		''' creating its own exception from within a {@link org.xml.sax.ContentHandler
		''' ContentHandler} callback, and needs to wrap an existing exception that is not a
		''' subclass of <seealso cref="org.xml.sax.SAXException SAXException"/>.</p>
		''' </summary>
		''' <param name="message"> The error or warning message, or null to
		'''                use the message from the embedded exception. </param>
		''' <param name="locator"> The locator object for the error or warning (may be
		'''        null). </param>
		''' <param name="e"> Any exception. </param>
		''' <seealso cref= org.xml.sax.Locator </seealso>
		Public Sub New(ByVal message As String, ByVal locator As Locator, ByVal e As Exception)
			MyBase.New(message, e)
			If locator IsNot Nothing Then
				init(locator.publicId, locator.systemId, locator.lineNumber, locator.columnNumber)
			Else
				init(Nothing, Nothing, -1, -1)
			End If
		End Sub


		''' <summary>
		''' Create a new SAXParseException.
		''' 
		''' <p>This constructor is most useful for parser writers.</p>
		''' 
		''' <p>All parameters except the message are as if
		''' they were provided by a <seealso cref="Locator"/>.  For example, if the
		''' system identifier is a URL (including relative filename), the
		''' caller must resolve it fully before creating the exception.</p>
		''' 
		''' </summary>
		''' <param name="message"> The error or warning message. </param>
		''' <param name="publicId"> The public identifier of the entity that generated
		'''                 the error or warning. </param>
		''' <param name="systemId"> The system identifier of the entity that generated
		'''                 the error or warning. </param>
		''' <param name="lineNumber"> The line number of the end of the text that
		'''                   caused the error or warning. </param>
		''' <param name="columnNumber"> The column number of the end of the text that
		'''                     cause the error or warning. </param>
		Public Sub New(ByVal message As String, ByVal publicId As String, ByVal systemId As String, ByVal lineNumber As Integer, ByVal columnNumber As Integer)
			MyBase.New(message)
			init(publicId, systemId, lineNumber, columnNumber)
		End Sub


		''' <summary>
		''' Create a new SAXParseException with an embedded exception.
		''' 
		''' <p>This constructor is most useful for parser writers who
		''' need to wrap an exception that is not a subclass of
		''' <seealso cref="org.xml.sax.SAXException SAXException"/>.</p>
		''' 
		''' <p>All parameters except the message and exception are as if
		''' they were provided by a <seealso cref="Locator"/>.  For example, if the
		''' system identifier is a URL (including relative filename), the
		''' caller must resolve it fully before creating the exception.</p>
		''' </summary>
		''' <param name="message"> The error or warning message, or null to use
		'''                the message from the embedded exception. </param>
		''' <param name="publicId"> The public identifier of the entity that generated
		'''                 the error or warning. </param>
		''' <param name="systemId"> The system identifier of the entity that generated
		'''                 the error or warning. </param>
		''' <param name="lineNumber"> The line number of the end of the text that
		'''                   caused the error or warning. </param>
		''' <param name="columnNumber"> The column number of the end of the text that
		'''                     cause the error or warning. </param>
		''' <param name="e"> Another exception to embed in this one. </param>
		Public Sub New(ByVal message As String, ByVal publicId As String, ByVal systemId As String, ByVal lineNumber As Integer, ByVal columnNumber As Integer, ByVal e As Exception)
			MyBase.New(message, e)
			init(publicId, systemId, lineNumber, columnNumber)
		End Sub


		''' <summary>
		''' Internal initialization method.
		''' </summary>
		''' <param name="publicId"> The public identifier of the entity which generated the exception,
		'''        or null. </param>
		''' <param name="systemId"> The system identifier of the entity which generated the exception,
		'''        or null. </param>
		''' <param name="lineNumber"> The line number of the error, or -1. </param>
		''' <param name="columnNumber"> The column number of the error, or -1. </param>
		Private Sub init(ByVal publicId As String, ByVal systemId As String, ByVal lineNumber As Integer, ByVal columnNumber As Integer)
			Me.publicId = publicId
			Me.systemId = systemId
			Me.lineNumber = lineNumber
			Me.columnNumber = columnNumber
		End Sub


		''' <summary>
		''' Get the public identifier of the entity where the exception occurred.
		''' </summary>
		''' <returns> A string containing the public identifier, or null
		'''         if none is available. </returns>
		''' <seealso cref= org.xml.sax.Locator#getPublicId </seealso>
		Public Overridable Property publicId As String
			Get
				Return Me.publicId
			End Get
		End Property


		''' <summary>
		''' Get the system identifier of the entity where the exception occurred.
		''' 
		''' <p>If the system identifier is a URL, it will have been resolved
		''' fully.</p>
		''' </summary>
		''' <returns> A string containing the system identifier, or null
		'''         if none is available. </returns>
		''' <seealso cref= org.xml.sax.Locator#getSystemId </seealso>
		Public Overridable Property systemId As String
			Get
				Return Me.systemId
			End Get
		End Property


		''' <summary>
		''' The line number of the end of the text where the exception occurred.
		''' 
		''' <p>The first line is line 1.</p>
		''' </summary>
		''' <returns> An integer representing the line number, or -1
		'''         if none is available. </returns>
		''' <seealso cref= org.xml.sax.Locator#getLineNumber </seealso>
		Public Overridable Property lineNumber As Integer
			Get
				Return Me.lineNumber
			End Get
		End Property


		''' <summary>
		''' The column number of the end of the text where the exception occurred.
		''' 
		''' <p>The first column in a line is position 1.</p>
		''' </summary>
		''' <returns> An integer representing the column number, or -1
		'''         if none is available. </returns>
		''' <seealso cref= org.xml.sax.Locator#getColumnNumber </seealso>
		Public Overridable Property columnNumber As Integer
			Get
				Return Me.columnNumber
			End Get
		End Property

		''' <summary>
		''' Override toString to provide more detailed error message.
		''' </summary>
		''' <returns> A string representation of this exception. </returns>
		Public Overrides Function ToString() As String
			Dim buf As New StringBuilder(Me.GetType().name)
			Dim message_Renamed As String = localizedMessage
			If publicId IsNot Nothing Then buf.Append("publicId: ").append(publicId)
			If systemId IsNot Nothing Then buf.Append("; systemId: ").append(systemId)
			If lineNumber<>-1 Then buf.Append("; lineNumber: ").append(lineNumber)
			If columnNumber<>-1 Then buf.Append("; columnNumber: ").append(columnNumber)

		   'append the exception message at the end
			If message_Renamed IsNot Nothing Then buf.Append("; ").append(message_Renamed)
			Return buf.ToString()
		End Function

		'////////////////////////////////////////////////////////////////////
		' Internal state.
		'////////////////////////////////////////////////////////////////////


		''' <summary>
		''' @serial The public identifier, or null. </summary>
		''' <seealso cref= #getPublicId </seealso>
		Private publicId As String


		''' <summary>
		''' @serial The system identifier, or null. </summary>
		''' <seealso cref= #getSystemId </seealso>
		Private systemId As String


		''' <summary>
		''' @serial The line number, or -1. </summary>
		''' <seealso cref= #getLineNumber </seealso>
		Private lineNumber As Integer


		''' <summary>
		''' @serial The column number, or -1. </summary>
		''' <seealso cref= #getColumnNumber </seealso>
		Private columnNumber As Integer

		' Added serialVersionUID to preserve binary compatibility
		Friend Shadows Const serialVersionUID As Long = -5651165872476709336L
	End Class

	' end of SAXParseException.java

End Namespace