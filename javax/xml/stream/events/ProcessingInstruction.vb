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
	''' An interface that describes the data found in processing instructions
	''' 
	''' @version 1.0
	''' @author Copyright (c) 2009 by Oracle Corporation. All Rights Reserved.
	''' @since 1.6
	''' </summary>
	Public Interface ProcessingInstruction
		Inherits XMLEvent

	  ''' <summary>
	  ''' The target section of the processing instruction
	  ''' </summary>
	  ''' <returns> the String value of the PI or null </returns>
	  ReadOnly Property target As String

	  ''' <summary>
	  ''' The data section of the processing instruction
	  ''' </summary>
	  ''' <returns> the String value of the PI's data or null </returns>
	  ReadOnly Property data As String
	End Interface

End Namespace