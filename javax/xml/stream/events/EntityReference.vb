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
	''' An interface for handling Entity events.
	''' 
	''' This event reports entities that have not been resolved
	''' and reports their replacement text unprocessed (if
	''' available).  This event will be reported if javax.xml.stream.isReplacingEntityReferences
	''' is set to false.  If javax.xml.stream.isReplacingEntityReferences is set to true
	''' entity references will be resolved transparently.
	''' 
	''' Entities are handled in two possible ways:
	''' 
	''' (1) If javax.xml.stream.isReplacingEntityReferences is set to true
	''' all entity references are resolved and reported as markup transparently.
	''' (2) If javax.xml.stream.isReplacingEntityReferences is set to false
	''' Entity references are reported as an EntityReference Event.
	''' 
	''' @version 1.0
	''' @author Copyright (c) 2009 by Oracle Corporation. All Rights Reserved.
	''' @since 1.6
	''' </summary>
	Public Interface EntityReference
		Inherits XMLEvent

	  ''' <summary>
	  ''' Return the declaration of this entity.
	  ''' </summary>
	  ReadOnly Property declaration As EntityDeclaration

	  ''' <summary>
	  ''' The name of the entity </summary>
	  ''' <returns> the entity's name, may not be null </returns>
	  ReadOnly Property name As String
	End Interface

End Namespace