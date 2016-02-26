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
	''' create to filter XMLStreamReaders
	''' @version 1.0
	''' @author Copyright (c) 2009 by Oracle Corporation. All Rights Reserved.
	''' @since 1.6
	''' </summary>
	Public Interface StreamFilter

	  ''' <summary>
	  ''' Tests whether the current state is part of this stream.  This method
	  ''' will return true if this filter accepts this event and false otherwise.
	  ''' 
	  ''' The method should not change the state of the reader when accepting
	  ''' a state.
	  ''' </summary>
	  ''' <param name="reader"> the event to test </param>
	  ''' <returns> true if this filter accepts this event, false otherwise </returns>
	  Function accept(ByVal reader As XMLStreamReader) As Boolean
	End Interface

End Namespace