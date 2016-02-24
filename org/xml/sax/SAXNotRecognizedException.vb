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

' SAXNotRecognizedException.java - unrecognized feature or value.
' http://www.saxproject.org
' Written by David Megginson
' NO WARRANTY!  This class is in the Public Domain.
' $Id: SAXNotRecognizedException.java,v 1.3 2004/11/03 22:55:32 jsuttor Exp $

Namespace org.xml.sax


	''' <summary>
	''' Exception class for an unrecognized identifier.
	''' 
	''' <blockquote>
	''' <em>This module, both source code and documentation, is in the
	''' Public Domain, and comes with <strong>NO WARRANTY</strong>.</em>
	''' See <a href='http://www.saxproject.org'>http://www.saxproject.org</a>
	''' for further information.
	''' </blockquote>
	''' 
	''' <p>An XMLReader will throw this exception when it finds an
	''' unrecognized feature or property identifier; SAX applications and
	''' extensions may use this class for other, similar purposes.</p>
	''' 
	''' @since SAX 2.0
	''' @author David Megginson </summary>
	''' <seealso cref= org.xml.sax.SAXNotSupportedException </seealso>
	Public Class SAXNotRecognizedException
		Inherits SAXException

		''' <summary>
		''' Default constructor.
		''' </summary>
		Public Sub New()
			MyBase.New()
		End Sub


		''' <summary>
		''' Construct a new exception with the given message.
		''' </summary>
		''' <param name="message"> The text message of the exception. </param>
		Public Sub New(ByVal message As String)
			MyBase.New(message)
		End Sub

		' Added serialVersionUID to preserve binary compatibility
		Friend Shadows Const serialVersionUID As Long = 5440506620509557213L
	End Class

	' end of SAXNotRecognizedException.java

End Namespace