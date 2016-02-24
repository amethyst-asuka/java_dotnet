Imports Microsoft.VisualBasic
Imports System

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
' $Id: SAXException.java,v 1.3 2004/11/03 22:55:32 jsuttor Exp $

Namespace org.xml.sax

	''' <summary>
	''' Encapsulate a general SAX error or warning.
	''' 
	''' <blockquote>
	''' <em>This module, both source code and documentation, is in the
	''' Public Domain, and comes with <strong>NO WARRANTY</strong>.</em>
	''' See <a href='http://www.saxproject.org'>http://www.saxproject.org</a>
	''' for further information.
	''' </blockquote>
	''' 
	''' <p>This class can contain basic error or warning information from
	''' either the XML parser or the application: a parser writer or
	''' application writer can subclass it to provide additional
	''' functionality.  SAX handlers may throw this exception or
	''' any exception subclassed from it.</p>
	''' 
	''' <p>If the application needs to pass through other types of
	''' exceptions, it must wrap those exceptions in a SAXException
	''' or an exception derived from a SAXException.</p>
	''' 
	''' <p>If the parser or application needs to include information about a
	''' specific location in an XML document, it should use the
	''' <seealso cref="org.xml.sax.SAXParseException SAXParseException"/> subclass.</p>
	''' 
	''' @since SAX 1.0
	''' @author David Megginson
	''' @version 2.0.1 (sax2r2) </summary>
	''' <seealso cref= org.xml.sax.SAXParseException </seealso>
	Public Class SAXException
		Inherits Exception


		''' <summary>
		''' Create a new SAXException.
		''' </summary>
		Public Sub New()
			MyBase.New()
			Me.exception = Nothing
		End Sub


		''' <summary>
		''' Create a new SAXException.
		''' </summary>
		''' <param name="message"> The error or warning message. </param>
		Public Sub New(ByVal message As String)
			MyBase.New(message)
			Me.exception = Nothing
		End Sub


		''' <summary>
		''' Create a new SAXException wrapping an existing exception.
		''' 
		''' <p>The existing exception will be embedded in the new
		''' one, and its message will become the default message for
		''' the SAXException.</p>
		''' </summary>
		''' <param name="e"> The exception to be wrapped in a SAXException. </param>
		Public Sub New(ByVal e As Exception)
			MyBase.New()
			Me.exception = e
		End Sub


		''' <summary>
		''' Create a new SAXException from an existing exception.
		''' 
		''' <p>The existing exception will be embedded in the new
		''' one, but the new exception will have its own message.</p>
		''' </summary>
		''' <param name="message"> The detail message. </param>
		''' <param name="e"> The exception to be wrapped in a SAXException. </param>
		Public Sub New(ByVal message As String, ByVal e As Exception)
			MyBase.New(message)
			Me.exception = e
		End Sub


		''' <summary>
		''' Return a detail message for this exception.
		''' 
		''' <p>If there is an embedded exception, and if the SAXException
		''' has no detail message of its own, this method will return
		''' the detail message from the embedded exception.</p>
		''' </summary>
		''' <returns> The error or warning message. </returns>
		Public Overridable Property message As String
			Get
				Dim message_Renamed As String = MyBase.message
    
				If message_Renamed Is Nothing AndAlso exception IsNot Nothing Then
					Return exception.Message
				Else
					Return message_Renamed
				End If
			End Get
		End Property


		''' <summary>
		''' Return the embedded exception, if any.
		''' </summary>
		''' <returns> The embedded exception, or null if there is none. </returns>
		Public Overridable Property exception As Exception
			Get
				Return exception
			End Get
		End Property

		''' <summary>
		''' Return the cause of the exception
		''' </summary>
		''' <returns> Return the cause of the exception </returns>
		Public Overridable Property cause As Exception
			Get
				Return exception
			End Get
		End Property

		''' <summary>
		''' Override toString to pick up any embedded exception.
		''' </summary>
		''' <returns> A string representation of this exception. </returns>
		Public Overrides Function ToString() As String
			If exception IsNot Nothing Then
				Return MyBase.ToString() & vbLf & exception.ToString()
			Else
				Return MyBase.ToString()
			End If
		End Function



		'////////////////////////////////////////////////////////////////////
		' Internal state.
		'////////////////////////////////////////////////////////////////////


		''' <summary>
		''' @serial The embedded exception if tunnelling, or null.
		''' </summary>
		Private exception As Exception

		' Added serialVersionUID to preserve binary compatibility
		Friend Const serialVersionUID As Long = 583241635256073760L
	End Class

	' end of SAXException.java

End Namespace