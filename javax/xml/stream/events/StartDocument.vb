'
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

'
' * Copyright (c) 2009 by Oracle Corporation. All Rights Reserved.
' 

Namespace javax.xml.stream.events
	''' <summary>
	''' An interface for the start document event
	''' 
	''' @version 1.0
	''' @author Copyright (c) 2009 by Oracle Corporation. All Rights Reserved.
	''' @since 1.6
	''' </summary>
	Public Interface StartDocument
		Inherits XMLEvent

	  ''' <summary>
	  ''' Returns the system ID of the XML data </summary>
	  ''' <returns> the system ID, defaults to "" </returns>
	  ReadOnly Property systemId As String

	  ''' <summary>
	  ''' Returns the encoding style of the XML data </summary>
	  ''' <returns> the character encoding, defaults to "UTF-8" </returns>
	  ReadOnly Property characterEncodingScheme As String

	  ''' <summary>
	  ''' Returns true if CharacterEncodingScheme was set in
	  ''' the encoding declaration of the document
	  ''' </summary>
	  Function encodingSet() As Boolean

	  ''' <summary>
	  ''' Returns if this XML is standalone </summary>
	  ''' <returns> the standalone state of XML, defaults to "no" </returns>
	  ReadOnly Property standalone As Boolean

	  ''' <summary>
	  ''' Returns true if the standalone attribute was set in
	  ''' the encoding declaration of the document.
	  ''' </summary>
	  Function standaloneSet() As Boolean

	  ''' <summary>
	  ''' Returns the version of XML of this XML stream </summary>
	  ''' <returns> the version of XML, defaults to "1.0" </returns>
	  ReadOnly Property version As String
	End Interface

End Namespace