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
	''' Implementations of this interface are attached to a
	''' <seealso cref="javax.print.DocPrintJob DocPrintJob"/> to monitor
	''' the status of attribute changes associated with the print job.
	'''  
	''' </summary>
	Public Interface PrintJobAttributeListener

		''' <summary>
		''' Notifies the listener of a change in some print job attributes.
		''' One example of an occurrence triggering this event is if the
		''' <seealso cref="javax.print.attribute.standard.JobState JobState"/>
		''' attribute changed from
		''' <code>PROCESSING</code> to <code>PROCESSING_STOPPED</code>. </summary>
		''' <param name="pjae"> the event. </param>
		Sub attributeUpdate(ByVal pjae As PrintJobAttributeEvent)

	End Interface

End Namespace