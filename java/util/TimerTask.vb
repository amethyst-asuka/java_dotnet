'
' * Copyright (c) 1999, 2013, Oracle and/or its affiliates. All rights reserved.
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

Namespace java.util

	''' <summary>
	''' A task that can be scheduled for one-time or repeated execution by a Timer.
	''' 
	''' @author  Josh Bloch </summary>
	''' <seealso cref=     Timer
	''' @since   1.3 </seealso>

	Public MustInherit Class TimerTask
		Implements Runnable

		''' <summary>
		''' This object is used to control access to the TimerTask internals.
		''' </summary>
		Friend ReadOnly lock As New Object

		''' <summary>
		''' The state of this task, chosen from the constants below.
		''' </summary>
		Friend state As Integer = VIRGIN

		''' <summary>
		''' This task has not yet been scheduled.
		''' </summary>
		Friend Const VIRGIN As Integer = 0

		''' <summary>
		''' This task is scheduled for execution.  If it is a non-repeating task,
		''' it has not yet been executed.
		''' </summary>
		Friend Const SCHEDULED As Integer = 1

		''' <summary>
		''' This non-repeating task has already executed (or is currently
		''' executing) and has not been cancelled.
		''' </summary>
		Friend Const EXECUTED As Integer = 2

		''' <summary>
		''' This task has been cancelled (with a call to TimerTask.cancel).
		''' </summary>
		Friend Const CANCELLED As Integer = 3

		''' <summary>
		''' Next execution time for this task in the format returned by
		''' System.currentTimeMillis, assuming this task is scheduled for execution.
		''' For repeating tasks, this field is updated prior to each task execution.
		''' </summary>
		Friend nextExecutionTime As Long

		''' <summary>
		''' Period in milliseconds for repeating tasks.  A positive value indicates
		''' fixed-rate execution.  A negative value indicates fixed-delay execution.
		''' A value of 0 indicates a non-repeating task.
		''' </summary>
		Friend period As Long = 0

		''' <summary>
		''' Creates a new timer task.
		''' </summary>
		Protected Friend Sub New()
		End Sub

		''' <summary>
		''' The action to be performed by this timer task.
		''' </summary>
		Public MustOverride Sub run() Implements Runnable.run

		''' <summary>
		''' Cancels this timer task.  If the task has been scheduled for one-time
		''' execution and has not yet run, or has not yet been scheduled, it will
		''' never run.  If the task has been scheduled for repeated execution, it
		''' will never run again.  (If the task is running when this call occurs,
		''' the task will run to completion, but will never run again.)
		''' 
		''' <p>Note that calling this method from within the <tt>run</tt> method of
		''' a repeating timer task absolutely guarantees that the timer task will
		''' not run again.
		''' 
		''' <p>This method may be called repeatedly; the second and subsequent
		''' calls have no effect.
		''' </summary>
		''' <returns> true if this task is scheduled for one-time execution and has
		'''         not yet run, or this task is scheduled for repeated execution.
		'''         Returns false if the task was scheduled for one-time execution
		'''         and has already run, or if the task was never scheduled, or if
		'''         the task was already cancelled.  (Loosely speaking, this method
		'''         returns <tt>true</tt> if it prevents one or more scheduled
		'''         executions from taking place.) </returns>
		Public Overridable Function cancel() As Boolean
			SyncLock lock
				Dim result As Boolean = (state = SCHEDULED)
				state = CANCELLED
				Return result
			End SyncLock
		End Function

		''' <summary>
		''' Returns the <i>scheduled</i> execution time of the most recent
		''' <i>actual</i> execution of this task.  (If this method is invoked
		''' while task execution is in progress, the return value is the scheduled
		''' execution time of the ongoing task execution.)
		''' 
		''' <p>This method is typically invoked from within a task's run method, to
		''' determine whether the current execution of the task is sufficiently
		''' timely to warrant performing the scheduled activity:
		''' <pre>{@code
		'''   public  Sub  run() {
		'''       if (System.currentTimeMillis() - scheduledExecutionTime() >=
		'''           MAX_TARDINESS)
		'''               return;  // Too late; skip this execution.
		'''       // Perform the task
		'''   }
		''' }</pre>
		''' This method is typically <i>not</i> used in conjunction with
		''' <i>fixed-delay execution</i> repeating tasks, as their scheduled
		''' execution times are allowed to drift over time, and so are not terribly
		''' significant.
		''' </summary>
		''' <returns> the time at which the most recent execution of this task was
		'''         scheduled to occur, in the format returned by Date.getTime().
		'''         The return value is undefined if the task has yet to commence
		'''         its first execution. </returns>
		''' <seealso cref= Date#getTime() </seealso>
		Public Overridable Function scheduledExecutionTime() As Long
			SyncLock lock
				Return (If(period < 0, nextExecutionTime + period, nextExecutionTime - period))
			End SyncLock
		End Function
	End Class

End Namespace