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
	''' This interface declares a simple filter interface that one can
	''' create to filter XMLEventReaders
	''' @version 1.0
	''' @author Copyright (c) 2009 by Oracle Corporation. All Rights Reserved.
	''' @since 1.6
	''' </summary>
	Public Interface EventFilter
	  ''' <summary>
	  ''' Tests whether this event is part of this stream.  This method
	  ''' will return true if this filter accepts this event and false
	  ''' otherwise.
	  ''' </summary>
	  ''' <param name="event"> the event to test </param>
	  ''' <returns> true if this filter accepts this event, false otherwise </returns>
	  Function accept(ByVal [event] As javax.xml.stream.events.XMLEvent) As Boolean
	End Interface

End Namespace