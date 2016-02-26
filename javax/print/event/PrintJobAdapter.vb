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
	''' An abstract adapter class for receiving print job events.
	''' The methods in this class are empty.
	''' This class exists as a convenience for creating listener objects.
	''' Extend this class to create a <seealso cref="PrintJobEvent"/> listener and override
	''' the methods for the events of interest.  Unlike the
	''' <seealso cref="java.awt.event.ComponentListener ComponentListener"/>
	''' interface, this abstract interface provides null methods so that you
	''' only need to define the methods you need, rather than all of the methods.
	'''  
	''' </summary>

	Public MustInherit Class PrintJobAdapter
		Implements PrintJobListener

		''' <summary>
		''' Called to notify the client that data has been successfully
		''' transferred to the print service, and the client may free
		''' local resources allocated for that data.  The client should
		''' not assume that the data has been completely printed after
		''' receiving this event.
		''' </summary>
		''' <param name="pje"> the event being notified </param>
		Public Overridable Sub printDataTransferCompleted(ByVal pje As PrintJobEvent) Implements PrintJobListener.printDataTransferCompleted
		End Sub

		''' <summary>
		''' Called to notify the client that the job completed successfully.
		''' </summary>
		''' <param name="pje"> the event being notified </param>
		Public Overridable Sub printJobCompleted(ByVal pje As PrintJobEvent) Implements PrintJobListener.printJobCompleted
		End Sub


		''' <summary>
		''' Called to notify the client that the job failed to complete
		''' successfully and will have to be resubmitted.
		''' </summary>
		''' <param name="pje"> the event being notified </param>
		Public Overridable Sub printJobFailed(ByVal pje As PrintJobEvent) Implements PrintJobListener.printJobFailed
		End Sub

		''' <summary>
		''' Called to notify the client that the job was canceled
		''' by user or program.
		''' </summary>
		''' <param name="pje"> the event being notified </param>
		Public Overridable Sub printJobCanceled(ByVal pje As PrintJobEvent) Implements PrintJobListener.printJobCanceled
		End Sub


		''' <summary>
		''' Called to notify the client that no more events will be delivered.
		''' One cause of this event being generated is if the job
		''' has successfully completed, but the printing system
		''' is limited in capability and cannot verify this.
		''' This event is required to be delivered if none of the other
		''' terminal events (completed/failed/canceled) are delivered.
		''' </summary>
		''' <param name="pje"> the event being notified </param>
		Public Overridable Sub printJobNoMoreEvents(ByVal pje As PrintJobEvent) Implements PrintJobListener.printJobNoMoreEvents
		End Sub


		''' <summary>
		''' Called to notify the client that some possibly user rectifiable
		''' problem occurs (eg printer out of paper).
		''' </summary>
		''' <param name="pje"> the event being notified </param>
		Public Overridable Sub printJobRequiresAttention(ByVal pje As PrintJobEvent) Implements PrintJobListener.printJobRequiresAttention
		End Sub

	End Class

End Namespace