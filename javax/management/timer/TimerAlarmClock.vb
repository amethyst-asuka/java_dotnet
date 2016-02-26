Imports System
import static com.sun.jmx.defaults.JmxProperties.TIMER_LOGGER

'
' * Copyright (c) 1999, 2012, Oracle and/or its affiliates. All rights reserved.
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

Namespace javax.management.timer

	''' <summary>
	''' This class provides a simple implementation of an alarm clock MBean.
	''' The aim of this MBean is to set up an alarm which wakes up the timer every timeout (fixed-delay)
	''' or at the specified date (fixed-rate).
	''' </summary>

	Friend Class TimerAlarmClock
		Inherits java.util.TimerTask

		Friend listener As Timer = Nothing
		Friend timeout As Long = 10000
		Friend [next] As DateTime = Nothing

	'    
	'     * ------------------------------------------
	'     *  CONSTRUCTORS
	'     * ------------------------------------------
	'     

		Public Sub New(ByVal listener As Timer, ByVal timeout As Long)
			Me.listener = listener
			Me.timeout = Math.Max(0L, timeout)
		End Sub

		Public Sub New(ByVal listener As Timer, ByVal [next] As DateTime)
			Me.listener = listener
			Me.next = [next]
		End Sub

	'    
	'     * ------------------------------------------
	'     *  PUBLIC METHODS
	'     * ------------------------------------------
	'     

		''' <summary>
		''' This method is called by the timer when it is started.
		''' </summary>
		Public Overridable Sub run()

			Try
				'this.sleep(timeout);
				Dim notif As New TimerAlarmClockNotification(Me)
				listener.notifyAlarmClock(notif)
			Catch e As Exception
				TIMER_LOGGER.logp(java.util.logging.Level.FINEST, GetType(Timer).name, "run", "Got unexpected exception when sending a notification", e)
			End Try
		End Sub
	End Class

End Namespace