'
' * Copyright (c) 1999, 2007, Oracle and/or its affiliates. All rights reserved.
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
	''' This class provides definitions of the notifications sent by timer MBeans.
	''' <BR>It defines a timer notification identifier which allows to retrieve a timer notification
	''' from the list of notifications of a timer MBean.
	''' <P>
	''' The timer notifications are created and handled by the timer MBean.
	''' 
	''' @since 1.5
	''' </summary>
	Public Class TimerNotification
		Inherits javax.management.Notification


		' Serial version 
		Private Const serialVersionUID As Long = 1798492029603825750L

	'    
	'     * ------------------------------------------
	'     *  PRIVATE VARIABLES
	'     * ------------------------------------------
	'     

		''' <summary>
		''' @serial Timer notification identifier.
		'''         This identifier is used to retrieve a timer notification from the timer list of notifications.
		''' </summary>
		Private notificationID As Integer?


	'    
	'     * ------------------------------------------
	'     *  CONSTRUCTORS
	'     * ------------------------------------------
	'     

		''' <summary>
		''' Creates a timer notification object.
		''' </summary>
		''' <param name="type"> The notification type. </param>
		''' <param name="source"> The notification producer. </param>
		''' <param name="sequenceNumber"> The notification sequence number within the source object. </param>
		''' <param name="timeStamp"> The notification emission date. </param>
		''' <param name="msg"> The notification message. </param>
		''' <param name="id"> The notification identifier.
		'''  </param>
		Public Sub New(ByVal type As String, ByVal source As Object, ByVal sequenceNumber As Long, ByVal timeStamp As Long, ByVal msg As String, ByVal id As Integer?)

			MyBase.New(type, source, sequenceNumber, timeStamp, msg)
			Me.notificationID = id
		End Sub

	'    
	'     * ------------------------------------------
	'     *  PUBLIC METHODS
	'     * ------------------------------------------
	'     

		' GETTERS AND SETTERS
		'--------------------

		''' <summary>
		''' Gets the identifier of this timer notification.
		''' </summary>
		''' <returns> The identifier. </returns>
		Public Overridable Property notificationID As Integer?
			Get
				Return notificationID
			End Get
		End Property

	'    
	'     * ------------------------------------------
	'     *  PACKAGE METHODS
	'     * ------------------------------------------
	'     

		''' <summary>
		''' Creates and returns a copy of this object.
		''' 
		''' </summary>
		Friend Overridable Function cloneTimerNotification() As Object

			Dim clone As New TimerNotification(Me.type, Me.source, Me.sequenceNumber, Me.timeStamp, Me.message, notificationID)
			clone.userData = Me.userData
			Return clone
		End Function
	End Class

End Namespace