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
	''' An interface for comment events
	''' 
	''' @version 1.0
	''' @author Copyright (c) 2009 by Oracle Corporation. All Rights Reserved.
	''' @since 1.6
	''' </summary>
	Public Interface Comment
		Inherits XMLEvent

	  ''' <summary>
	  ''' Return the string data of the comment, returns empty string if it
	  ''' does not exist
	  ''' </summary>
	  ReadOnly Property text As String
	End Interface

End Namespace