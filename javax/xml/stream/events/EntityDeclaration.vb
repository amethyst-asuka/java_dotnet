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
	''' An interface for handling Entity Declarations
	''' 
	''' This interface is used to record and report unparsed entity declarations.
	''' 
	''' @version 1.0
	''' @author Copyright (c) 2009 by Oracle Corporation. All Rights Reserved.
	''' @since 1.6
	''' </summary>
	Public Interface EntityDeclaration
		Inherits XMLEvent

	  ''' <summary>
	  ''' The entity's public identifier, or null if none was given </summary>
	  ''' <returns> the public ID for this declaration or null </returns>
	  ReadOnly Property publicId As String

	  ''' <summary>
	  ''' The entity's system identifier. </summary>
	  ''' <returns> the system ID for this declaration or null </returns>
	  ReadOnly Property systemId As String

	  ''' <summary>
	  ''' The entity's name </summary>
	  ''' <returns> the name, may not be null </returns>
	  ReadOnly Property name As String

	  ''' <summary>
	  ''' The name of the associated notation. </summary>
	  ''' <returns> the notation name </returns>
	  ReadOnly Property notationName As String

	  ''' <summary>
	  ''' The replacement text of the entity.
	  ''' This method will only return non-null
	  ''' if this is an internal entity. </summary>
	  ''' <returns> null or the replacment text </returns>
	  ReadOnly Property replacementText As String

	  ''' <summary>
	  ''' Get the base URI for this reference
	  ''' or null if this information is not available </summary>
	  ''' <returns> the base URI or null </returns>
	  ReadOnly Property baseURI As String

	End Interface

End Namespace