Imports System
Imports System.Collections.Generic

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



	' java imports
	'
	' NPCTE fix for bugId 4464388, esc 0,  MR , to be added after modification of jmx spec
	'import java.io.Serializable;
	' end of NPCTE fix for bugId 4464388

	' jmx imports
	'

	''' <summary>
	''' Exposes the management interface of the timer MBean.
	''' 
	''' @since 1.5
	''' </summary>
	Public Interface TimerMBean

		''' <summary>
		''' Starts the timer.
		''' <P>
		''' If there is one or more timer notifications before the time in the list of notifications, the notification
		''' is sent according to the <CODE>sendPastNotifications</CODE> flag and then, updated
		''' according to its period and remaining number of occurrences.
		''' If the timer notification date remains earlier than the current date, this notification is just removed
		''' from the list of notifications.
		''' </summary>
		Sub start()

		''' <summary>
		''' Stops the timer.
		''' </summary>
		Sub [stop]()

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

		Function addNotification(ByVal type As String, ByVal message As String, ByVal userData As Object, ByVal [date] As DateTime, ByVal period As Long, ByVal nbOccurences As Long, ByVal fixedRate As Boolean) As Integer?

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

		Function addNotification(ByVal type As String, ByVal message As String, ByVal userData As Object, ByVal [date] As DateTime, ByVal period As Long, ByVal nbOccurences As Long) As Integer?

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

		Function addNotification(ByVal type As String, ByVal message As String, ByVal userData As Object, ByVal [date] As DateTime, ByVal period As Long) As Integer?

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

		Function addNotification(ByVal type As String, ByVal message As String, ByVal userData As Object, ByVal [date] As DateTime) As Integer?

		''' <summary>
		''' Removes the timer notification corresponding to the specified identifier from the list of notifications.
		''' </summary>
		''' <param name="id"> The timer notification identifier.
		''' </param>
		''' <exception cref="InstanceNotFoundException"> The specified identifier does not correspond to any timer notification
		''' in the list of notifications of this timer MBean. </exception>
		Sub removeNotification(ByVal id As Integer?)

		''' <summary>
		''' Removes all the timer notifications corresponding to the specified type from the list of notifications.
		''' </summary>
		''' <param name="type"> The timer notification type.
		''' </param>
		''' <exception cref="InstanceNotFoundException"> The specified type does not correspond to any timer notification
		''' in the list of notifications of this timer MBean. </exception>
		Sub removeNotifications(ByVal type As String)

		''' <summary>
		''' Removes all the timer notifications from the list of notifications
		''' and resets the counter used to update the timer notification identifiers.
		''' </summary>
		Sub removeAllNotifications()

		' GETTERS AND SETTERS
		'--------------------

		''' <summary>
		''' Gets the number of timer notifications registered into the list of notifications.
		''' </summary>
		''' <returns> The number of timer notifications. </returns>
		ReadOnly Property nbNotifications As Integer

		''' <summary>
		''' Gets all timer notification identifiers registered into the list of notifications.
		''' </summary>
		''' <returns> A vector of <CODE>Integer</CODE> objects containing all the timer notification identifiers.
		''' <BR>The vector is empty if there is no timer notification registered for this timer MBean. </returns>
		ReadOnly Property allNotificationIDs As List(Of Integer?)

		''' <summary>
		''' Gets all the identifiers of timer notifications corresponding to the specified type.
		''' </summary>
		''' <param name="type"> The timer notification type.
		''' </param>
		''' <returns> A vector of <CODE>Integer</CODE> objects containing all the identifiers of
		''' timer notifications with the specified <CODE>type</CODE>.
		''' <BR>The vector is empty if there is no timer notifications registered for this timer MBean
		''' with the specified <CODE>type</CODE>. </returns>
		Function getNotificationIDs(ByVal type As String) As List(Of Integer?)

		''' <summary>
		''' Gets the timer notification type corresponding to the specified identifier.
		''' </summary>
		''' <param name="id"> The timer notification identifier.
		''' </param>
		''' <returns> The timer notification type or null if the identifier is not mapped to any
		''' timer notification registered for this timer MBean. </returns>
		Function getNotificationType(ByVal id As Integer?) As String

		''' <summary>
		''' Gets the timer notification detailed message corresponding to the specified identifier.
		''' </summary>
		''' <param name="id"> The timer notification identifier.
		''' </param>
		''' <returns> The timer notification detailed message or null if the identifier is not mapped to any
		''' timer notification registered for this timer MBean. </returns>
		Function getNotificationMessage(ByVal id As Integer?) As String

		''' <summary>
		''' Gets the timer notification user data object corresponding to the specified identifier.
		''' </summary>
		''' <param name="id"> The timer notification identifier.
		''' </param>
		''' <returns> The timer notification user data object or null if the identifier is not mapped to any
		''' timer notification registered for this timer MBean. </returns>
		' NPCTE fix for bugId 4464388, esc 0 , MR , 03 sept 2001 , to be added after modification of jmx spec
		'public Serializable getNotificationUserData(Integer id);
		' end of NPCTE fix for bugId 4464388
		Function getNotificationUserData(ByVal id As Integer?) As Object
		''' <summary>
		''' Gets a copy of the date associated to a timer notification.
		''' </summary>
		''' <param name="id"> The timer notification identifier.
		''' </param>
		''' <returns> A copy of the date or null if the identifier is not mapped to any
		''' timer notification registered for this timer MBean. </returns>
		Function getDate(ByVal id As Integer?) As DateTime

		''' <summary>
		''' Gets a copy of the period (in milliseconds) associated to a timer notification.
		''' </summary>
		''' <param name="id"> The timer notification identifier.
		''' </param>
		''' <returns> A copy of the period or null if the identifier is not mapped to any
		''' timer notification registered for this timer MBean. </returns>
		Function getPeriod(ByVal id As Integer?) As Long?

		''' <summary>
		''' Gets a copy of the remaining number of occurrences associated to a timer notification.
		''' </summary>
		''' <param name="id"> The timer notification identifier.
		''' </param>
		''' <returns> A copy of the remaining number of occurrences or null if the identifier is not mapped to any
		''' timer notification registered for this timer MBean. </returns>
		Function getNbOccurences(ByVal id As Integer?) As Long?

		''' <summary>
		''' Gets a copy of the flag indicating whether a periodic notification is
		''' executed at <i>fixed-delay</i> or at <i>fixed-rate</i>.
		''' </summary>
		''' <param name="id"> The timer notification identifier.
		''' </param>
		''' <returns> A copy of the flag indicating whether a periodic notification is
		'''         executed at <i>fixed-delay</i> or at <i>fixed-rate</i>. </returns>
		Function getFixedRate(ByVal id As Integer?) As Boolean?

		''' <summary>
		''' Gets the flag indicating whether or not the timer sends past notifications.
		''' </summary>
		''' <returns> The past notifications sending on/off flag value.
		''' </returns>
		''' <seealso cref= #setSendPastNotifications </seealso>
		Property sendPastNotifications As Boolean


		''' <summary>
		''' Tests whether the timer MBean is active.
		''' A timer MBean is marked active when the <seealso cref="#start start"/> method is called.
		''' It becomes inactive when the <seealso cref="#stop stop"/> method is called.
		''' </summary>
		''' <returns> <CODE>true</CODE> if the timer MBean is active, <CODE>false</CODE> otherwise. </returns>
		ReadOnly Property active As Boolean

		''' <summary>
		''' Tests whether the list of timer notifications is empty.
		''' </summary>
		''' <returns> <CODE>true</CODE> if the list of timer notifications is empty, <CODE>false</CODE> otherwise. </returns>
		ReadOnly Property empty As Boolean
	End Interface

End Namespace