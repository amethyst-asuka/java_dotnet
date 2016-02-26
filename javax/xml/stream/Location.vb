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

Namespace javax.xml.stream

	''' <summary>
	''' Provides information on the location of an event.
	''' 
	''' All the information provided by a Location is optional.  For example
	''' an application may only report line numbers.
	''' 
	''' @version 1.0
	''' @author Copyright (c) 2009 by Oracle Corporation. All Rights Reserved.
	''' @since 1.6
	''' </summary>
	Public Interface Location
	  ''' <summary>
	  ''' Return the line number where the current event ends,
	  ''' returns -1 if none is available. </summary>
	  ''' <returns> the current line number </returns>
	  ReadOnly Property lineNumber As Integer

	  ''' <summary>
	  ''' Return the column number where the current event ends,
	  ''' returns -1 if none is available. </summary>
	  ''' <returns> the current column number </returns>
	  ReadOnly Property columnNumber As Integer

	  ''' <summary>
	  ''' Return the byte or character offset into the input source this location
	  ''' is pointing to. If the input source is a file or a byte stream then
	  ''' this is the byte offset into that stream, but if the input source is
	  ''' a character media then the offset is the character offset.
	  ''' Returns -1 if there is no offset available. </summary>
	  ''' <returns> the current offset </returns>
	  ReadOnly Property characterOffset As Integer

	  ''' <summary>
	  ''' Returns the public ID of the XML </summary>
	  ''' <returns> the public ID, or null if not available </returns>
	  ReadOnly Property publicId As String

	  ''' <summary>
	  ''' Returns the system ID of the XML </summary>
	  ''' <returns> the system ID, or null if not available </returns>
	  ReadOnly Property systemId As String
	End Interface

End Namespace