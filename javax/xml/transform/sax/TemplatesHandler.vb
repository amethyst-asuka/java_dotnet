Imports javax.xml.transform

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
	''' A SAX ContentHandler that may be used to process SAX
	''' parse events (parsing transformation instructions) into a Templates object.
	''' 
	''' <p>Note that TemplatesHandler does not need to implement LexicalHandler.</p>
	''' </summary>
	Public Interface TemplatesHandler
		Inherits org.xml.sax.ContentHandler

		''' <summary>
		''' When a TemplatesHandler object is used as a ContentHandler
		''' for the parsing of transformation instructions, it creates a Templates object,
		''' which the caller can get once the SAX events have been completed.
		''' </summary>
		''' <returns> The Templates object that was created during
		''' the SAX event process, or null if no Templates object has
		''' been created.
		'''  </returns>
		ReadOnly Property templates As Templates

		''' <summary>
		''' Set the base ID (URI or system ID) for the Templates object
		''' created by this builder.  This must be set in order to
		''' resolve relative URIs in the stylesheet.  This must be
		''' called before the startDocument event.
		''' </summary>
		''' <param name="systemID"> Base URI for this stylesheet. </param>
		Property systemId As String

	End Interface

End Namespace