Imports Microsoft.VisualBasic
Imports System
Imports System.Runtime.CompilerServices
Imports System.Collections.Generic
Imports System.Text
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


	' jmx imports
	'

	''' 
	''' <summary>
	''' Provides the implementation of the timer MBean.
	''' The timer MBean sends out an alarm at a specified time
	''' that wakes up all the listeners registered to receive timer notifications.
	''' <P>
	''' 
	''' This class manages a list of dated timer notifications.
	''' A method allows users to add/remove as many notifications as required.
	''' When a timer notification is emitted by the timer and becomes obsolete,
	''' it is automatically removed from the list of timer notifications.
	''' <BR>Additional timer notifications can be added into regularly repeating notifications.
	''' <P>
	''' 
	''' Note:
	''' <OL>
	''' <LI>When sending timer notifications, the timer updates the notification sequence number
	''' irrespective of the notification type.
	''' <LI>The timer service relies on the system date of the host where the <CODE>Timer</CODE> class is loaded.
	''' Listeners may receive untimely notifications
	''' if their host has a different system date.
	''' To avoid such problems, synchronize the system date of all host machines where timing is needed.
	''' <LI>The default behavior for periodic notifications is <i>fixed-delay execution</i>, as
	'''     specified in <seealso cref="java.util.Timer"/>. In order to use <i>fixed-rate execution</i>, use the
	'''     overloaded <seealso cref="#addNotification(String, String, Object, Date, long, long, boolean)"/> method.
	''' <LI>Notification listeners are potentially all executed in the same
	''' thread.  Therefore, they should execute rapidly to avoid holding up
	''' other listeners or perturbing the regularity of fixed-delay
	''' executions.  See <seealso cref="NotificationBroadcasterSupport"/>.
	''' </OL>
	''' 
	''' @since 1.5
	''' </summary>
	Public Class Timer
		Inherits javax.management.NotificationBroadcasterSupport
		Implements TimerMBean, javax.management.MBeanRegistration


	'    
	'     * ------------------------------------------
	'     *  PUBLIC VARIABLES
	'     * ------------------------------------------
	'     

		''' <summary>
		''' Number of milliseconds in one second.
		''' Useful constant for the <CODE>addNotification</CODE> method.
		''' </summary>
		Public Const ONE_SECOND As Long = 1000

		''' <summary>
		''' Number of milliseconds in one minute.
		''' Useful constant for the <CODE>addNotification</CODE> method.
		''' </summary>
		Public Shared ReadOnly ONE_MINUTE As Long = 60*ONE_SECOND

		''' <summary>
		''' Number of milliseconds in one hour.
		''' Useful constant for the <CODE>addNotification</CODE> method.
		''' </summary>
		Public Shared ReadOnly ONE_HOUR As Long = 60*ONE_MINUTE

		''' <summary>
		''' Number of milliseconds in one day.
		''' Useful constant for the <CODE>addNotification</CODE> method.
		''' </summary>
		Public Shared ReadOnly ONE_DAY As Long = 24*ONE_HOUR

		''' <summary>
		''' Number of milliseconds in one week.
		''' Useful constant for the <CODE>addNotification</CODE> method.
		''' </summary>
		Public Shared ReadOnly ONE_WEEK As Long = 7*ONE_DAY

	'    
	'     * ------------------------------------------
	'     *  PRIVATE VARIABLES
	'     * ------------------------------------------
	'     

		''' <summary>
		''' Table containing all the timer notifications of this timer,
		''' with the associated date, period and number of occurrences.
		''' </summary>
		Private ReadOnly timerTable As IDictionary(Of Integer?, Object()) = New Dictionary(Of Integer?, Object())

		''' <summary>
		''' Past notifications sending on/off flag value.
		''' This attribute is used to specify if the timer has to send past notifications after start.
		''' <BR>The default value is set to <CODE>false</CODE>.
		''' </summary>
		Private ___sendPastNotifications As Boolean = False

		''' <summary>
		''' Timer state.
		''' The default value is set to <CODE>false</CODE>.
		''' </summary>
		<NonSerialized> _
		Private ___isActive As Boolean = False

		''' <summary>
		''' Timer sequence number.
		''' The default value is set to 0.
		''' </summary>
		<NonSerialized> _
		Private sequenceNumber As Long = 0

		' Flags needed to keep the indexes of the objects in the array.
		'
		Private Const TIMER_NOTIF_INDEX As Integer = 0
		Private Const TIMER_DATE_INDEX As Integer = 1
		Private Const TIMER_PERIOD_INDEX As Integer = 2
		Private Const TIMER_NB_OCCUR_INDEX As Integer = 3
		Private Const ALARM_CLOCK_INDEX As Integer = 4
		Private Const FIXED_RATE_INDEX As Integer = 5

		''' <summary>
		''' The notification counter ID.
		''' Used to keep the max key value inserted into the timer table.
		''' </summary>
'JAVA TO VB CONVERTER TODO TASK: There is no VB equivalent to 'volatile':
		Private counterID As Integer = 0

		Private ___timer As java.util.Timer

	'    
	'     * ------------------------------------------
	'     *  CONSTRUCTORS
	'     * ------------------------------------------
	'     

		''' <summary>
		''' Default constructor.
		''' </summary>
		Public Sub New()
		End Sub

	'    
	'     * ------------------------------------------
	'     *  PUBLIC METHODS
	'     * ------------------------------------------
	'     

		''' <summary>
		''' Allows the timer MBean to perform any operations it needs before being registered
		''' in the MBean server.
		''' <P>
		''' Not used in this context.
		''' </summary>
		''' <param name="server"> The MBean server in which the timer MBean will be registered. </param>
		''' <param name="name"> The object name of the timer MBean.
		''' </param>
		''' <returns> The name of the timer MBean registered.
		''' </returns>
		''' <exception cref="java.lang.Exception"> </exception>
		Public Overridable Function preRegister(ByVal server As javax.management.MBeanServer, ByVal name As javax.management.ObjectName) As javax.management.ObjectName
			Return name
		End Function

		''' <summary>
		''' Allows the timer MBean to perform any operations needed after having been
		''' registered in the MBean server or after the registration has failed.
		''' <P>
		''' Not used in this context.
		''' </summary>
		Public Overridable Sub postRegister(ByVal registrationDone As Boolean?)
		End Sub

		''' <summary>
		''' Allows the timer MBean to perform any operations it needs before being unregistered
		''' by the MBean server.
		''' <P>
		''' Stops the timer.
		''' </summary>
		''' <exception cref="java.lang.Exception"> </exception>
		Public Overridable Sub preDeregister()

			TIMER_LOGGER.logp(java.util.logging.Level.FINER, GetType(Timer).name, "preDeregister", "stop the timer")

			' Stop the timer.
			'
			[stop]()
		End Sub

		''' <summary>
		''' Allows the timer MBean to perform any operations needed after having been
		''' unregistered by the MBean server.
		''' <P>
		''' Not used in this context.
		''' </summary>
		Public Overridable Sub postDeregister()
		End Sub

	'    
	'     * This overrides the method in NotificationBroadcasterSupport.
	'     * Return the MBeanNotificationInfo[] array for this MBean.
	'     * The returned array has one element to indicate that the MBean
	'     * can emit TimerNotification.  The array of type strings
	'     * associated with this entry is a snapshot of the current types
	'     * that were given to addNotification.
	'     
		<MethodImpl(MethodImplOptions.Synchronized)> _
		Public Property Overrides notificationInfo As javax.management.MBeanNotificationInfo()
			Get
				Dim notifTypes As java.util.Set(Of String) = New SortedSet(Of String)
				For Each entry As Object() In timerTable.Values
					Dim notif As TimerNotification = CType(entry(TIMER_NOTIF_INDEX), TimerNotification)
					notifTypes.add(notif.type)
				Next entry
				Dim notifTypesArray As String() = notifTypes.ToArray(New String(){})
				Return New javax.management.MBeanNotificationInfo() { New javax.management.MBeanNotificationInfo(notifTypesArray, GetType(TimerNotification).name, "Notification sent by Timer MBean") }
			End Get
		End Property

		''' <summary>
		''' Starts the timer.
		''' <P>
		''' If there is one or more timer notifications before the time in the list of notifications, the notification
		''' is sent according to the <CODE>sendPastNotifications</CODE> flag and then, updated
		''' according to its period and remaining number of occurrences.
		''' If the timer notification date remains earlier than the current date, this notification is just removed
		''' from the list of notifications.
		''' </summary>
		<MethodImpl(MethodImplOptions.Synchronized)> _
		Public Overridable Sub start() Implements TimerMBean.start

			TIMER_LOGGER.logp(java.util.logging.Level.FINER, GetType(Timer).name, "start", "starting the timer")

			' Start the TimerAlarmClock.
			'
			If ___isActive = False Then

				___timer = New java.util.Timer

				Dim alarmClock As TimerAlarmClock
				Dim ___date As DateTime

				Dim currentDate As DateTime = DateTime.Now

				' Send or not past notifications depending on the flag.
				' Update the date and the number of occurrences of past notifications
				' to make them later than the current date.
				'
				sendPastNotifications(currentDate, ___sendPastNotifications)

				' Update and start all the TimerAlarmClocks.
				' Here, all the notifications in the timer table are later than the current date.
				'
				For Each obj As Object() In timerTable.Values

					' Retrieve the date notification and the TimerAlarmClock.
					'
					___date = CDate(obj(TIMER_DATE_INDEX))

					' Update all the TimerAlarmClock timeouts and start them.
					'
					Dim ___fixedRate As Boolean = CBool(obj(FIXED_RATE_INDEX))
					If ___fixedRate Then
					  alarmClock = New TimerAlarmClock(Me, ___date)
					  obj(ALARM_CLOCK_INDEX) = CObj(alarmClock)
					  ___timer.schedule(alarmClock, alarmClock.next)
					Else
					  alarmClock = New TimerAlarmClock(Me, (___date - currentDate))
					  obj(ALARM_CLOCK_INDEX) = CObj(alarmClock)
					  ___timer.schedule(alarmClock, alarmClock.timeout)
					End If
				Next obj

				' Set the state to ON.
				'
				___isActive = True

				TIMER_LOGGER.logp(java.util.logging.Level.FINER, GetType(Timer).name, "start", "timer started")
			Else
				TIMER_LOGGER.logp(java.util.logging.Level.FINER, GetType(Timer).name, "start", "the timer is already activated")
			End If
		End Sub

		''' <summary>
		''' Stops the timer.
		''' </summary>
		<MethodImpl(MethodImplOptions.Synchronized)> _
		Public Overridable Sub [stop]() Implements TimerMBean.stop

			TIMER_LOGGER.logp(java.util.logging.Level.FINER, GetType(Timer).name, "stop", "stopping the timer")

			' Stop the TimerAlarmClock.
			'
			If ___isActive = True Then

				For Each obj As Object() In timerTable.Values

					' Stop all the TimerAlarmClock.
					'
					Dim alarmClock As TimerAlarmClock = CType(obj(ALARM_CLOCK_INDEX), TimerAlarmClock)
					If alarmClock IsNot Nothing Then alarmClock.cancel()
				Next obj

				___timer.cancel()

				' Set the state to OFF.
				'
				___isActive = False

				TIMER_LOGGER.logp(java.util.logging.Level.FINER, GetType(Timer).name, "stop", "timer stopped")
			Else
				TIMER_LOGGER.logp(java.util.logging.Level.FINER, GetType(Timer).name, "stop", "the timer is already deactivated")
			End If
		End Sub

		''' <summary>
		''' Creates a new timer notification with the specified <CODE>type</CODE>, <CODE>message</CODE>
		''' and <CODE>userData</CODE> and inserts it into the list of notifications with a given date,
		''' period and number of occurrences.
		''' <P>
		''' If the timer notification to be inserted has a date that is before the current date,
		''' the method behaves as if the specified date were the current date. <BR>
		''' For once-off notifications, the notification is delivered immediately. <BR>
		''' For periodic notifications, the first notification is delivered immediately and the
		''' subsequent ones are spaced as specified by the period parameter.
		''' <P>
		''' Note that once the timer notification has been added into the list of notifications,
		''' its associated date, period and number of occurrences cannot be updated.
		''' <P>
		''' In the case of a periodic notification, the value of parameter <i>fixedRate</i> is used to
		''' specify the execution scheme, as specified in <seealso cref="java.util.Timer"/>.
		''' </summary>
		''' <param name="type"> The timer notification type. </param>
		''' <param name="message"> The timer notification detailed message. </param>
		''' <param name="userData"> The timer notification user data object. </param>
		''' <param name="date"> The date when the notification occurs. </param>
		''' <param name="period"> The period of the timer notification (in milliseconds). </param>
		''' <param name="nbOccurences"> The total number the timer notification will be emitted. </param>
		''' <param name="fixedRate"> If <code>true</code> and if the notification is periodic, the notification
		'''                  is scheduled with a <i>fixed-rate</i> execution scheme. If
		'''                  <code>false</code> and if the notification is periodic, the notification
		'''                  is scheduled with a <i>fixed-delay</i> execution scheme. Ignored if the
		'''                  notification is not periodic.
		''' </param>
		''' <returns> The identifier of the new created timer notification.
		''' </returns>
		''' <exception cref="java.lang.IllegalArgumentException"> The date is {@code null} or
		''' the period or the number of occurrences is negative.
		''' </exception>
		''' <seealso cref= #addNotification(String, String, Object, Date, long, long) </seealso>
	' NPCTE fix for bugId 4464388, esc 0,  MR, to be added after modification of jmx spec
	'  public synchronized Integer addNotification(String type, String message, Serializable userData,
	'                                                Date date, long period, long nbOccurences)
	' end of NPCTE fix for bugId 4464388

		<MethodImpl(MethodImplOptions.Synchronized)> _
		Public Overridable Function addNotification(ByVal type As String, ByVal message As String, ByVal userData As Object, ByVal [date] As DateTime, ByVal period As Long, ByVal nbOccurences As Long, ByVal fixedRate As Boolean) As Integer?

			If [date] Is Nothing Then Throw New System.ArgumentException("Timer notification date cannot be null.")

			' Check that all the timer notification attributes are valid.
			'

			' Invalid timer period value exception:
			' Check that the period and the nbOccurences are POSITIVE VALUES.
			'
			If (period < 0) OrElse (nbOccurences < 0) Then Throw New System.ArgumentException("Negative values for the periodicity")

			Dim currentDate As DateTime = DateTime.Now

			' Update the date if it is before the current date.
			'
			If currentDate.after([date]) Then

				[date] = currentDate
				If TIMER_LOGGER.isLoggable(java.util.logging.Level.FINER) Then TIMER_LOGGER.logp(java.util.logging.Level.FINER, GetType(Timer).name, "addNotification", "update timer notification to add with:" & vbLf & vbTab & "Notification date = " & [date])
			End If

			' Create and add the timer notification into the timer table.
			'
			counterID += 1
			Dim notifID As Integer? = Convert.ToInt32(counterID)

			' The sequenceNumber and the timeStamp attributes are updated
			' when the notification is emitted by the timer.
			'
			Dim notif As New TimerNotification(type, Me, 0, 0, message, notifID)
			notif.userData = userData

			Dim obj As Object() = New Object(5){}

			Dim alarmClock As TimerAlarmClock
			If fixedRate Then
			  alarmClock = New TimerAlarmClock(Me, [date])
			Else
			  alarmClock = New TimerAlarmClock(Me, ([date] - currentDate))
			End If

			' Fix bug 00417.B
			' The date registered into the timer is a clone from the date parameter.
			'
			Dim d As New DateTime([date])

			obj(TIMER_NOTIF_INDEX) = CObj(notif)
			obj(TIMER_DATE_INDEX) = CObj(d)
			obj(TIMER_PERIOD_INDEX) = CObj(period)
			obj(TIMER_NB_OCCUR_INDEX) = CObj(nbOccurences)
			obj(ALARM_CLOCK_INDEX) = CObj(alarmClock)
			obj(FIXED_RATE_INDEX) = Convert.ToBoolean(fixedRate)

			If TIMER_LOGGER.isLoggable(java.util.logging.Level.FINER) Then
				Dim strb As (New StringBuilder).Append("adding timer notification:" & vbLf & vbTab).append("Notification source = ").append(notif.source).append(vbLf & vbTab & "Notification type = ").append(notif.type).append(vbLf & vbTab & "Notification ID = ").append(notifID).append(vbLf & vbTab & "Notification date = ").append(d).append(vbLf & vbTab & "Notification period = ").append(period).append(vbLf & vbTab & "Notification nb of occurrences = ").append(nbOccurences).append(vbLf & vbTab & "Notification executes at fixed rate = ").append(fixedRate)
				TIMER_LOGGER.logp(java.util.logging.Level.FINER, GetType(Timer).name, "addNotification", strb.ToString())
			End If

			timerTable(notifID) = obj

			' Update and start the TimerAlarmClock.
			'
			If ___isActive = True Then
			  If fixedRate Then
				___timer.schedule(alarmClock, alarmClock.next)
			  Else
				___timer.schedule(alarmClock, alarmClock.timeout)
			  End If
			End If

			TIMER_LOGGER.logp(java.util.logging.Level.FINER, GetType(Timer).name, "addNotification", "timer notification added")
			Return notifID
		End Function

		''' <summary>
		''' Creates a new timer notification with the specified <CODE>type</CODE>, <CODE>message</CODE>
		''' and <CODE>userData</CODE> and inserts it into the list of notifications with a given date,
		''' period and number of occurrences.
		''' <P>
		''' If the timer notification to be inserted has a date that is before the current date,
		''' the method behaves as if the specified date were the current date. <BR>
		''' For once-off notifications, the notification is delivered immediately. <BR>
		''' For periodic notifications, the first notification is delivered immediately and the
		''' subsequent ones are spaced as specified by the period parameter.
		''' <P>
		''' Note that once the timer notification has been added into the list of notifications,
		''' its associated date, period and number of occurrences cannot be updated.
		''' <P>
		''' In the case of a periodic notification, uses a <i>fixed-delay</i> execution scheme, as specified in
		''' <seealso cref="java.util.Timer"/>. In order to use a <i>fixed-rate</i> execution scheme, use
		''' <seealso cref="#addNotification(String, String, Object, Date, long, long, boolean)"/> instead.
		''' </summary>
		''' <param name="type"> The timer notification type. </param>
		''' <param name="message"> The timer notification detailed message. </param>
		''' <param name="userData"> The timer notification user data object. </param>
		''' <param name="date"> The date when the notification occurs. </param>
		''' <param name="period"> The period of the timer notification (in milliseconds). </param>
		''' <param name="nbOccurences"> The total number the timer notification will be emitted.
		''' </param>
		''' <returns> The identifier of the new created timer notification.
		''' </returns>
		''' <exception cref="java.lang.IllegalArgumentException"> The date is {@code null} or
		''' the period or the number of occurrences is negative.
		''' </exception>
		''' <seealso cref= #addNotification(String, String, Object, Date, long, long, boolean) </seealso>
	' NPCTE fix for bugId 4464388, esc 0,  MR , to be added after modification of jmx spec
	'  public synchronized Integer addNotification(String type, String message, Serializable userData,
	'                                              Date date, long period)
	' end of NPCTE fix for bugId 4464388 */

		<MethodImpl(MethodImplOptions.Synchronized)> _
		Public Overridable Function addNotification(ByVal type As String, ByVal message As String, ByVal userData As Object, ByVal [date] As DateTime, ByVal period As Long, ByVal nbOccurences As Long) As Integer?

		  Return addNotification(type, message, userData, [date], period, nbOccurences, False)
		End Function

		''' <summary>
		''' Creates a new timer notification with the specified <CODE>type</CODE>, <CODE>message</CODE>
		''' and <CODE>userData</CODE> and inserts it into the list of notifications with a given date
		''' and period and a null number of occurrences.
		''' <P>
		''' The timer notification will repeat continuously using the timer period using a <i>fixed-delay</i>
		''' execution scheme, as specified in <seealso cref="java.util.Timer"/>. In order to use a <i>fixed-rate</i>
		''' execution scheme, use {@link #addNotification(String, String, Object, Date, long, long,
		''' boolean)} instead.
		''' <P>
		''' If the timer notification to be inserted has a date that is before the current date,
		''' the method behaves as if the specified date were the current date. The
		''' first notification is delivered immediately and the subsequent ones are
		''' spaced as specified by the period parameter.
		''' </summary>
		''' <param name="type"> The timer notification type. </param>
		''' <param name="message"> The timer notification detailed message. </param>
		''' <param name="userData"> The timer notification user data object. </param>
		''' <param name="date"> The date when the notification occurs. </param>
		''' <param name="period"> The period of the timer notification (in milliseconds).
		''' </param>
		''' <returns> The identifier of the new created timer notification.
		''' </returns>
		''' <exception cref="java.lang.IllegalArgumentException"> The date is {@code null} or
		''' the period is negative. </exception>
	' NPCTE fix for bugId 4464388, esc 0,  MR , to be added after modification of jmx spec
	'  public synchronized Integer addNotification(String type, String message, Serializable userData,
	'                                              Date date, long period)
	' end of NPCTE fix for bugId 4464388 */

		<MethodImpl(MethodImplOptions.Synchronized)> _
		Public Overridable Function addNotification(ByVal type As String, ByVal message As String, ByVal userData As Object, ByVal [date] As DateTime, ByVal period As Long) As Integer?

			Return (addNotification(type, message, userData, [date], period, 0))
		End Function

		''' <summary>
		''' Creates a new timer notification with the specified <CODE>type</CODE>, <CODE>message</CODE>
		''' and <CODE>userData</CODE> and inserts it into the list of notifications with a given date
		''' and a null period and number of occurrences.
		''' <P>
		''' The timer notification will be handled once at the specified date.
		''' <P>
		''' If the timer notification to be inserted has a date that is before the current date,
		''' the method behaves as if the specified date were the current date and the
		''' notification is delivered immediately.
		''' </summary>
		''' <param name="type"> The timer notification type. </param>
		''' <param name="message"> The timer notification detailed message. </param>
		''' <param name="userData"> The timer notification user data object. </param>
		''' <param name="date"> The date when the notification occurs.
		''' </param>
		''' <returns> The identifier of the new created timer notification.
		''' </returns>
		''' <exception cref="java.lang.IllegalArgumentException"> The date is {@code null}. </exception>
	' NPCTE fix for bugId 4464388, esc 0,  MR, to be added after modification of jmx spec
	'  public synchronized Integer addNotification(String type, String message, Serializable userData, Date date)
	'      throws java.lang.IllegalArgumentException {
	' end of NPCTE fix for bugId 4464388

		<MethodImpl(MethodImplOptions.Synchronized)> _
		Public Overridable Function addNotification(ByVal type As String, ByVal message As String, ByVal userData As Object, ByVal [date] As DateTime) As Integer?


			Return (addNotification(type, message, userData, [date], 0, 0))
		End Function

		''' <summary>
		''' Removes the timer notification corresponding to the specified identifier from the list of notifications.
		''' </summary>
		''' <param name="id"> The timer notification identifier.
		''' </param>
		''' <exception cref="InstanceNotFoundException"> The specified identifier does not correspond to any timer notification
		''' in the list of notifications of this timer MBean. </exception>
		<MethodImpl(MethodImplOptions.Synchronized)> _
		Public Overridable Sub removeNotification(ByVal id As Integer?) Implements TimerMBean.removeNotification

			' Check that the notification to remove is effectively in the timer table.
			'
			If timerTable.ContainsKey(id) = False Then Throw New javax.management.InstanceNotFoundException("Timer notification to remove not in the list of notifications")

			' Stop the TimerAlarmClock.
			'
			Dim obj As Object() = timerTable(id)
			Dim alarmClock As TimerAlarmClock = CType(obj(ALARM_CLOCK_INDEX), TimerAlarmClock)
			If alarmClock IsNot Nothing Then alarmClock.cancel()

			' Remove the timer notification from the timer table.
			'
			If TIMER_LOGGER.isLoggable(java.util.logging.Level.FINER) Then
				Dim strb As (New StringBuilder).Append("removing timer notification:").append(vbLf & vbTab & "Notification source = ").append(CType(obj(TIMER_NOTIF_INDEX), TimerNotification).source).append(vbLf & vbTab & "Notification type = ").append(CType(obj(TIMER_NOTIF_INDEX), TimerNotification).type).append(vbLf & vbTab & "Notification ID = ").append(CType(obj(TIMER_NOTIF_INDEX), TimerNotification).notificationID).append(vbLf & vbTab & "Notification date = ").append(obj(TIMER_DATE_INDEX)).append(vbLf & vbTab & "Notification period = ").append(obj(TIMER_PERIOD_INDEX)).append(vbLf & vbTab & "Notification nb of occurrences = ").append(obj(TIMER_NB_OCCUR_INDEX)).append(vbLf & vbTab & "Notification executes at fixed rate = ").append(obj(FIXED_RATE_INDEX))
				TIMER_LOGGER.logp(java.util.logging.Level.FINER, GetType(Timer).name, "removeNotification", strb.ToString())
			End If

			timerTable.Remove(id)

			TIMER_LOGGER.logp(java.util.logging.Level.FINER, GetType(Timer).name, "removeNotification", "timer notification removed")
		End Sub

		''' <summary>
		''' Removes all the timer notifications corresponding to the specified type from the list of notifications.
		''' </summary>
		''' <param name="type"> The timer notification type.
		''' </param>
		''' <exception cref="InstanceNotFoundException"> The specified type does not correspond to any timer notification
		''' in the list of notifications of this timer MBean. </exception>
		<MethodImpl(MethodImplOptions.Synchronized)> _
		Public Overridable Sub removeNotifications(ByVal type As String) Implements TimerMBean.removeNotifications

			Dim v As List(Of Integer?) = getNotificationIDs(type)

			If v.Count = 0 Then Throw New javax.management.InstanceNotFoundException("Timer notifications to remove not in the list of notifications")

			For Each i As Integer? In v
				removeNotification(i)
			Next i
		End Sub

		''' <summary>
		''' Removes all the timer notifications from the list of notifications
		''' and resets the counter used to update the timer notification identifiers.
		''' </summary>
		<MethodImpl(MethodImplOptions.Synchronized)> _
		Public Overridable Sub removeAllNotifications() Implements TimerMBean.removeAllNotifications

			Dim alarmClock As TimerAlarmClock

			For Each obj As Object() In timerTable.Values

				' Stop the TimerAlarmClock.
				'
				alarmClock = CType(obj(ALARM_CLOCK_INDEX), TimerAlarmClock)
	'             if (alarmClock != null) {
	'                 alarmClock.interrupt();
	'                 try {
	'                     // Wait until the thread die.
	'                     //
	'                     alarmClock.join();
	'                 } catch (InterruptedException ex) {
	'                     // Ignore...
	'                 }
					  ' Remove the reference on the TimerAlarmClock.
					  '
	'             }
				alarmClock.cancel()
			Next obj

			' Remove all the timer notifications from the timer table.
			TIMER_LOGGER.logp(java.util.logging.Level.FINER, GetType(Timer).name, "removeAllNotifications", "removing all timer notifications")

			timerTable.Clear()

			TIMER_LOGGER.logp(java.util.logging.Level.FINER, GetType(Timer).name, "removeAllNotifications", "all timer notifications removed")
			' Reset the counterID.
			'
			counterID = 0

			TIMER_LOGGER.logp(java.util.logging.Level.FINER, GetType(Timer).name, "removeAllNotifications", "timer notification counter ID reset")
		End Sub

		' GETTERS AND SETTERS
		'--------------------

		''' <summary>
		''' Gets the number of timer notifications registered into the list of notifications.
		''' </summary>
		''' <returns> The number of timer notifications. </returns>
		<MethodImpl(MethodImplOptions.Synchronized)> _
		Public Overridable Property nbNotifications As Integer Implements TimerMBean.getNbNotifications
			Get
				Return timerTable.Count
			End Get
		End Property

		''' <summary>
		''' Gets all timer notification identifiers registered into the list of notifications.
		''' </summary>
		''' <returns> A vector of <CODE>Integer</CODE> objects containing all the timer notification identifiers.
		''' <BR>The vector is empty if there is no timer notification registered for this timer MBean. </returns>
		<MethodImpl(MethodImplOptions.Synchronized)> _
		Public Overridable Property allNotificationIDs As List(Of Integer?) Implements TimerMBean.getAllNotificationIDs
			Get
				Return New List(Of Integer?)(timerTable.Keys)
			End Get
		End Property

		''' <summary>
		''' Gets all the identifiers of timer notifications corresponding to the specified type.
		''' </summary>
		''' <param name="type"> The timer notification type.
		''' </param>
		''' <returns> A vector of <CODE>Integer</CODE> objects containing all the identifiers of
		''' timer notifications with the specified <CODE>type</CODE>.
		''' <BR>The vector is empty if there is no timer notifications registered for this timer MBean
		''' with the specified <CODE>type</CODE>. </returns>
		<MethodImpl(MethodImplOptions.Synchronized)> _
		Public Overridable Function getNotificationIDs(ByVal type As String) As List(Of Integer?) Implements TimerMBean.getNotificationIDs

			Dim s As String

			Dim v As New List(Of Integer?)

			For Each entry As KeyValuePair(Of Integer?, Object()) In timerTable
				Dim obj As Object() = entry.Value
				s = CType(obj(TIMER_NOTIF_INDEX), TimerNotification).type
				If If(type Is Nothing, s Is Nothing, type.Equals(s)) Then v.Add(entry.Key)
			Next entry
			Return v
		End Function
		' 5089997: return is Vector<Integer> not Vector<TimerNotification>

		''' <summary>
		''' Gets the timer notification type corresponding to the specified identifier.
		''' </summary>
		''' <param name="id"> The timer notification identifier.
		''' </param>
		''' <returns> The timer notification type or null if the identifier is not mapped to any
		''' timer notification registered for this timer MBean. </returns>
		<MethodImpl(MethodImplOptions.Synchronized)> _
		Public Overridable Function getNotificationType(ByVal id As Integer?) As String Implements TimerMBean.getNotificationType

			Dim obj As Object() = timerTable(id)
			If obj IsNot Nothing Then Return (CType(obj(TIMER_NOTIF_INDEX), TimerNotification).type)
			Return Nothing
		End Function

		''' <summary>
		''' Gets the timer notification detailed message corresponding to the specified identifier.
		''' </summary>
		''' <param name="id"> The timer notification identifier.
		''' </param>
		''' <returns> The timer notification detailed message or null if the identifier is not mapped to any
		''' timer notification registered for this timer MBean. </returns>
		<MethodImpl(MethodImplOptions.Synchronized)> _
		Public Overridable Function getNotificationMessage(ByVal id As Integer?) As String Implements TimerMBean.getNotificationMessage

			Dim obj As Object() = timerTable(id)
			If obj IsNot Nothing Then Return (CType(obj(TIMER_NOTIF_INDEX), TimerNotification).message)
			Return Nothing
		End Function

		''' <summary>
		''' Gets the timer notification user data object corresponding to the specified identifier.
		''' </summary>
		''' <param name="id"> The timer notification identifier.
		''' </param>
		''' <returns> The timer notification user data object or null if the identifier is not mapped to any
		''' timer notification registered for this timer MBean. </returns>
		' NPCTE fix for bugId 4464388, esc 0, MR, 03 sept 2001, to be added after modification of jmx spec
		'public Serializable getNotificationUserData(Integer id) {
		' end of NPCTE fix for bugId 4464388

		<MethodImpl(MethodImplOptions.Synchronized)> _
		Public Overridable Function getNotificationUserData(ByVal id As Integer?) As Object Implements TimerMBean.getNotificationUserData
			Dim obj As Object() = timerTable(id)
			If obj IsNot Nothing Then Return (CType(obj(TIMER_NOTIF_INDEX), TimerNotification).userData)
			Return Nothing
		End Function

		''' <summary>
		''' Gets a copy of the date associated to a timer notification.
		''' </summary>
		''' <param name="id"> The timer notification identifier.
		''' </param>
		''' <returns> A copy of the date or null if the identifier is not mapped to any
		''' timer notification registered for this timer MBean. </returns>
		<MethodImpl(MethodImplOptions.Synchronized)> _
		Public Overridable Function getDate(ByVal id As Integer?) As DateTime Implements TimerMBean.getDate

			Dim obj As Object() = timerTable(id)
			If obj IsNot Nothing Then
				Dim ___date As DateTime = CDate(obj(TIMER_DATE_INDEX))
				Return (New DateTime(___date))
			End If
			Return Nothing
		End Function

		''' <summary>
		''' Gets a copy of the period (in milliseconds) associated to a timer notification.
		''' </summary>
		''' <param name="id"> The timer notification identifier.
		''' </param>
		''' <returns> A copy of the period or null if the identifier is not mapped to any
		''' timer notification registered for this timer MBean. </returns>
		<MethodImpl(MethodImplOptions.Synchronized)> _
		Public Overridable Function getPeriod(ByVal id As Integer?) As Long?

			Dim obj As Object() = timerTable(id)
			If obj IsNot Nothing Then Return CLng(Fix(obj(TIMER_PERIOD_INDEX)))
			Return Nothing
		End Function

		''' <summary>
		''' Gets a copy of the remaining number of occurrences associated to a timer notification.
		''' </summary>
		''' <param name="id"> The timer notification identifier.
		''' </param>
		''' <returns> A copy of the remaining number of occurrences or null if the identifier is not mapped to any
		''' timer notification registered for this timer MBean. </returns>
		<MethodImpl(MethodImplOptions.Synchronized)> _
		Public Overridable Function getNbOccurences(ByVal id As Integer?) As Long?

			Dim obj As Object() = timerTable(id)
			If obj IsNot Nothing Then Return CLng(Fix(obj(TIMER_NB_OCCUR_INDEX)))
			Return Nothing
		End Function

		''' <summary>
		''' Gets a copy of the flag indicating whether a periodic notification is
		''' executed at <i>fixed-delay</i> or at <i>fixed-rate</i>.
		''' </summary>
		''' <param name="id"> The timer notification identifier.
		''' </param>
		''' <returns> A copy of the flag indicating whether a periodic notification is
		'''         executed at <i>fixed-delay</i> or at <i>fixed-rate</i>. </returns>
		<MethodImpl(MethodImplOptions.Synchronized)> _
		Public Overridable Function getFixedRate(ByVal id As Integer?) As Boolean?

		  Dim obj As Object() = timerTable(id)
		  If obj IsNot Nothing Then
			Dim ___fixedRate As Boolean? = CBool(obj(FIXED_RATE_INDEX))
			Return (Convert.ToBoolean(___fixedRate))
		  End If
		  Return Nothing
		End Function

		''' <summary>
		''' Gets the flag indicating whether or not the timer sends past notifications.
		''' <BR>The default value of the past notifications sending on/off flag is <CODE>false</CODE>.
		''' </summary>
		''' <returns> The past notifications sending on/off flag value.
		''' </returns>
		''' <seealso cref= #setSendPastNotifications </seealso>
		Public Overridable Property sendPastNotifications As Boolean Implements TimerMBean.getSendPastNotifications
			Get
				Return ___sendPastNotifications
			End Get
			Set(ByVal value As Boolean)
				___sendPastNotifications = value
			End Set
		End Property


		''' <summary>
		''' Tests whether the timer MBean is active.
		''' A timer MBean is marked active when the <seealso cref="#start start"/> method is called.
		''' It becomes inactive when the <seealso cref="#stop stop"/> method is called.
		''' <BR>The default value of the active on/off flag is <CODE>false</CODE>.
		''' </summary>
		''' <returns> <CODE>true</CODE> if the timer MBean is active, <CODE>false</CODE> otherwise. </returns>
		Public Overridable Property active As Boolean Implements TimerMBean.isActive
			Get
				Return ___isActive
			End Get
		End Property

		''' <summary>
		''' Tests whether the list of timer notifications is empty.
		''' </summary>
		''' <returns> <CODE>true</CODE> if the list of timer notifications is empty, <CODE>false</CODE> otherwise. </returns>
		<MethodImpl(MethodImplOptions.Synchronized)> _
		Public Overridable Property empty As Boolean Implements TimerMBean.isEmpty
			Get
				Return (timerTable.Count = 0)
			End Get
		End Property

	'    
	'     * ------------------------------------------
	'     *  PRIVATE METHODS
	'     * ------------------------------------------
	'     

		''' <summary>
		''' Sends or not past notifications depending on the specified flag.
		''' </summary>
		''' <param name="currentDate"> The current date. </param>
		''' <param name="currentFlag"> The flag indicating if past notifications must be sent or not. </param>
		<MethodImpl(MethodImplOptions.Synchronized)> _
		Private Sub sendPastNotifications(ByVal currentDate As DateTime, ByVal currentFlag As Boolean)

			Dim notif As TimerNotification
			Dim notifID As Integer?
			Dim ___date As DateTime

			Dim values As New List(Of Object())(timerTable.Values)

			For Each obj As Object() In values

				' Retrieve the timer notification and the date notification.
				'
				notif = CType(obj(TIMER_NOTIF_INDEX), TimerNotification)
				notifID = notif.notificationID
				___date = CDate(obj(TIMER_DATE_INDEX))

				' Update the timer notification while:
				'  - the timer notification date is earlier than the current date
				'  - the timer notification has not been removed from the timer table.
				'
				Do While (currentDate.after(___date)) AndAlso (timerTable.ContainsKey(notifID))

					If currentFlag = True Then
						If TIMER_LOGGER.isLoggable(java.util.logging.Level.FINER) Then
							Dim strb As (New StringBuilder).Append("sending past timer notification:").append(vbLf & vbTab & "Notification source = ").append(notif.source).append(vbLf & vbTab & "Notification type = ").append(notif.type).append(vbLf & vbTab & "Notification ID = ").append(notif.notificationID).append(vbLf & vbTab & "Notification date = ").append(___date).append(vbLf & vbTab & "Notification period = ").append(obj(TIMER_PERIOD_INDEX)).append(vbLf & vbTab & "Notification nb of occurrences = ").append(obj(TIMER_NB_OCCUR_INDEX)).append(vbLf & vbTab & "Notification executes at fixed rate = ").append(obj(FIXED_RATE_INDEX))
							TIMER_LOGGER.logp(java.util.logging.Level.FINER, GetType(Timer).name, "sendPastNotifications", strb.ToString())
						End If
						sendNotification(___date, notif)

						TIMER_LOGGER.logp(java.util.logging.Level.FINER, GetType(Timer).name, "sendPastNotifications", "past timer notification sent")
					End If

					' Update the date and the number of occurrences of the timer notification.
					'
					updateTimerTable(notif.notificationID)
				Loop
			Next obj
		End Sub

		''' <summary>
		''' If the timer notification is not periodic, it is removed from the list of notifications.
		''' <P>
		''' If the timer period of the timer notification has a non null periodicity,
		''' the date of the timer notification is updated by adding the periodicity.
		''' The associated TimerAlarmClock is updated by setting its timeout to the period value.
		''' <P>
		''' If the timer period has a defined number of occurrences, the timer
		''' notification is updated if the number of occurrences has not yet been reached.
		''' Otherwise it is removed from the list of notifications.
		''' </summary>
		''' <param name="notifID"> The timer notification identifier to update. </param>
		<MethodImpl(MethodImplOptions.Synchronized)> _
		Private Sub updateTimerTable(ByVal notifID As Integer?)

			' Retrieve the timer notification and the TimerAlarmClock.
			'
			Dim obj As Object() = timerTable(notifID)
			Dim ___date As DateTime = CDate(obj(TIMER_DATE_INDEX))
			Dim ___period As Long? = CLng(Fix(obj(TIMER_PERIOD_INDEX)))
			Dim ___nbOccurences As Long? = CLng(Fix(obj(TIMER_NB_OCCUR_INDEX)))
			Dim ___fixedRate As Boolean? = CBool(obj(FIXED_RATE_INDEX))
			Dim alarmClock As TimerAlarmClock = CType(obj(ALARM_CLOCK_INDEX), TimerAlarmClock)

			If ___period <> 0 Then

				' Update the date and the number of occurrences of the timer notification
				' and the TimerAlarmClock time out.
				' NOTES :
				'   nbOccurences = 0 notifies an infinite periodicity.
				'   nbOccurences = 1 notifies a finite periodicity that has reached its end.
				'   nbOccurences > 1 notifies a finite periodicity that has not yet reached its end.
				'
				If (___nbOccurences = 0) OrElse (___nbOccurences > 1) Then

					___date = ___date + ___period
					obj(TIMER_NB_OCCUR_INDEX) = Convert.ToInt64(Math.Max(0L, (___nbOccurences - 1)))
					___nbOccurences = CLng(Fix(obj(TIMER_NB_OCCUR_INDEX)))

					If ___isActive = True Then
					  If ___fixedRate Then
						alarmClock = New TimerAlarmClock(Me, ___date)
						obj(ALARM_CLOCK_INDEX) = CObj(alarmClock)
						___timer.schedule(alarmClock, alarmClock.next)
					  Else
						alarmClock = New TimerAlarmClock(Me, ___period)
						obj(ALARM_CLOCK_INDEX) = CObj(alarmClock)
						___timer.schedule(alarmClock, alarmClock.timeout)
					  End If
					End If
					If TIMER_LOGGER.isLoggable(java.util.logging.Level.FINER) Then
						Dim notif As TimerNotification = CType(obj(TIMER_NOTIF_INDEX), TimerNotification)
						Dim strb As (New StringBuilder).Append("update timer notification with:").append(vbLf & vbTab & "Notification source = ").append(notif.source).append(vbLf & vbTab & "Notification type = ").append(notif.type).append(vbLf & vbTab & "Notification ID = ").append(notifID).append(vbLf & vbTab & "Notification date = ").append(___date).append(vbLf & vbTab & "Notification period = ").append(___period).append(vbLf & vbTab & "Notification nb of occurrences = ").append(___nbOccurences).append(vbLf & vbTab & "Notification executes at fixed rate = ").append(___fixedRate)
						TIMER_LOGGER.logp(java.util.logging.Level.FINER, GetType(Timer).name, "updateTimerTable", strb.ToString())
					End If
				Else
					If alarmClock IsNot Nothing Then alarmClock.cancel()
					timerTable.Remove(notifID)
				End If
			Else
				If alarmClock IsNot Nothing Then alarmClock.cancel()
				timerTable.Remove(notifID)
			End If
		End Sub

	'    
	'     * ------------------------------------------
	'     *  PACKAGE METHODS
	'     * ------------------------------------------
	'     

		''' <summary>
		''' This method is called by the timer each time
		''' the TimerAlarmClock has exceeded its timeout.
		''' </summary>
		''' <param name="notification"> The TimerAlarmClock notification. </param>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
		Friend Overridable Sub notifyAlarmClock(ByVal ___notification As TimerAlarmClockNotification)

			Dim timerNotification As TimerNotification = Nothing
			Dim timerDate As DateTime = Nothing

			' Retrieve the timer notification associated to the alarm-clock.
			'
			Dim alarmClock As TimerAlarmClock = CType(___notification.source, TimerAlarmClock)

			SyncLock Timer.this
				For Each obj As Object() In timerTable.Values
					If obj(ALARM_CLOCK_INDEX) Is alarmClock Then
						timerNotification = CType(obj(TIMER_NOTIF_INDEX), TimerNotification)
						timerDate = CDate(obj(TIMER_DATE_INDEX))
						Exit For
					End If
				Next obj
			End SyncLock

			' Notify the timer.
			'
			sendNotification(timerDate, timerNotification)

			' Update the notification and the TimerAlarmClock timeout.
			'
			updateTimerTable(timerNotification.notificationID)
		End Sub

		''' <summary>
		''' This method is used by the timer MBean to update and send a timer
		''' notification to all the listeners registered for this kind of notification.
		''' </summary>
		''' <param name="timeStamp"> The notification emission date. </param>
		''' <param name="notification"> The timer notification to send. </param>
		Friend Overridable Sub sendNotification(ByVal timeStamp As DateTime, ByVal ___notification As TimerNotification)

			If TIMER_LOGGER.isLoggable(java.util.logging.Level.FINER) Then
				Dim strb As (New StringBuilder).Append("sending timer notification:").append(vbLf & vbTab & "Notification source = ").append(___notification.source).append(vbLf & vbTab & "Notification type = ").append(___notification.type).append(vbLf & vbTab & "Notification ID = ").append(___notification.notificationID).append(vbLf & vbTab & "Notification date = ").append(timeStamp)
				TIMER_LOGGER.logp(java.util.logging.Level.FINER, GetType(Timer).name, "sendNotification", strb.ToString())
			End If
			Dim curSeqNumber As Long
			SyncLock Me
				sequenceNumber = sequenceNumber + 1
				curSeqNumber = sequenceNumber
			End SyncLock
			SyncLock ___notification
				___notification.timeStamp = timeStamp
				___notification.sequenceNumber = curSeqNumber
				Me.sendNotification(CType(___notification.cloneTimerNotification(), TimerNotification))
			End SyncLock

			TIMER_LOGGER.logp(java.util.logging.Level.FINER, GetType(Timer).name, "sendNotification", "timer notification sent")
		End Sub
	End Class

End Namespace