'
' * Copyright (c) 2000, 2003, Oracle and/or its affiliates. All rights reserved.
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


	''' 
	''' <summary>
	''' Class <code>PrintJobEvent</code> encapsulates common events a print job
	''' reports to let a listener know of progress in the processing of the
	''' <seealso cref="DocPrintJob"/>.
	''' 
	''' </summary>

	Public Class PrintJobEvent
		Inherits PrintEvent

	   Private Const serialVersionUID As Long = -1711656903622072997L

	   Private reason As Integer

	   ''' <summary>
	   ''' The job was canceled by the <seealso cref="javax.print.PrintService PrintService"/>.
	   ''' </summary>
	   Public Const JOB_CANCELED As Integer = 101

	   ''' <summary>
	   ''' The document cis completely printed.
	   ''' </summary>
	   Public Const JOB_COMPLETE As Integer = 102

	   ''' <summary>
	   ''' The print service reports that the job cannot be completed.
	   ''' The application must resubmit the job.
	   ''' </summary>

	   Public Const JOB_FAILED As Integer = 103

	   ''' <summary>
	   ''' The print service indicates that a - possibly transient - problem
	   ''' may require external intervention before the print service can
	   ''' continue.  One example of an event that can
	   ''' generate this message is when the printer runs out of paper.
	   ''' </summary>
	   Public Const REQUIRES_ATTENTION As Integer = 104

	   ''' <summary>
	   ''' Not all print services may be capable of delivering interesting
	   ''' events, or even telling when a job is complete. This message indicates
	   ''' the print job has no further information or communication
	   ''' with the print service. This message should always be delivered
	   ''' if a terminal event (completed/failed/canceled) is not delivered.
	   ''' For example, if messages such as JOB_COMPLETE have NOT been received
	   ''' before receiving this message, the only inference that should be drawn
	   ''' is that the print service does not support delivering such an event.
	   ''' </summary>
	   Public Const NO_MORE_EVENTS As Integer = 105

	   ''' <summary>
	   ''' The job is not necessarily printed yet, but the data has been transferred
	   ''' successfully from the client to the print service. The client may
	   ''' free data resources.
	   ''' </summary>
	   Public Const DATA_TRANSFER_COMPLETE As Integer = 106

	   ''' <summary>
	   ''' Constructs a <code>PrintJobEvent</code> object.
	   ''' </summary>
	   ''' <param name="source">  a <code>DocPrintJob</code> object </param>
	   ''' <param name="reason">  an int specifying the reason. </param>
	   ''' <exception cref="IllegalArgumentException"> if <code>source</code> is
	   '''         <code>null</code>. </exception>

		Public Sub New(ByVal source As javax.print.DocPrintJob, ByVal reason As Integer)

			MyBase.New(source)
			Me.reason = reason
		End Sub

		''' <summary>
		''' Gets the reason for this event. </summary>
		''' <returns>  reason int. </returns>
		Public Overridable Property printEventType As Integer
			Get
				Return reason
			End Get
		End Property

		''' <summary>
		''' Determines the <code>DocPrintJob</code> to which this print job
		''' event pertains.
		''' </summary>
		''' <returns>  the <code>DocPrintJob</code> object that represents the
		'''          print job that reports the events encapsulated by this
		'''          <code>PrintJobEvent</code>.
		'''  </returns>
		Public Overridable Property printJob As javax.print.DocPrintJob
			Get
				Return CType(source, javax.print.DocPrintJob)
			End Get
		End Property


	End Class

End Namespace