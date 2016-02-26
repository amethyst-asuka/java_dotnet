'
' * Copyright (c) 2000, 2001, Oracle and/or its affiliates. All rights reserved.
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

Namespace javax.print.event

	''' <summary>
	''' Implementations of this listener interface are attached to a
	''' <seealso cref="javax.print.PrintService PrintService"/> to monitor
	''' the status of the print service.
	''' <p>
	''' To monitor a particular job see <seealso cref="PrintJobListener"/> and
	''' <seealso cref="PrintJobAttributeListener"/>.
	''' </summary>

	Public Interface PrintServiceAttributeListener

		''' <summary>
		''' Called to notify a listener of an event in the print service.
		''' The service will call this method on an event notification thread.
		''' The client should not perform lengthy processing in this callback
		''' or subsequent event notifications may be blocked. </summary>
		''' <param name="psae"> the event being notified </param>
		Sub attributeUpdate(ByVal psae As PrintServiceAttributeEvent)

	End Interface

End Namespace