'
' * Copyright (c) 2002, 2006, Oracle and/or its affiliates. All rights reserved.
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
	''' <p>Definitions of the notifications sent by TimerAlarmClock
	''' MBeans.</p>
	''' </summary>
	Friend Class TimerAlarmClockNotification
		Inherits javax.management.Notification

		' Serial version 
		Private Const serialVersionUID As Long = -4841061275673620641L

	'    
	'     * ------------------------------------------
	'     *  CONSTRUCTORS
	'     * ------------------------------------------
	'     

		''' <summary>
		''' Constructor.
		''' </summary>
		''' <param name="source"> the source. </param>
		Public Sub New(ByVal source As TimerAlarmClock)
			MyBase.New("", source, 0)
		End Sub
	End Class

End Namespace