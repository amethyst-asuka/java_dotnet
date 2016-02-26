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

Namespace javax.management.monitor


	' jmx imports
	'

	''' <summary>
	''' Provides definitions of the notifications sent by monitor MBeans.
	''' <P>
	''' The notification source and a set of parameters concerning the monitor MBean's state
	''' need to be specified when creating a new object of this class.
	''' 
	''' The list of notifications fired by the monitor MBeans is the following:
	''' 
	''' <UL>
	''' <LI>Common to all kind of monitors:
	'''     <UL>
	'''     <LI>The observed object is not registered in the MBean server.
	'''     <LI>The observed attribute is not contained in the observed object.
	'''     <LI>The type of the observed attribute is not correct.
	'''     <LI>Any exception (except the cases described above) occurs when trying to get the value of the observed attribute.
	'''     </UL>
	''' <LI>Common to the counter and the gauge monitors:
	'''     <UL>
	'''     <LI>The threshold high or threshold low are not of the same type as the gauge (gauge monitors).
	'''     <LI>The threshold or the offset or the modulus are not of the same type as the counter (counter monitors).
	'''     </UL>
	''' <LI>Counter monitors only:
	'''     <UL>
	'''     <LI>The observed attribute has reached the threshold value.
	'''     </UL>
	''' <LI>Gauge monitors only:
	'''     <UL>
	'''     <LI>The observed attribute has exceeded the threshold high value.
	'''     <LI>The observed attribute has exceeded the threshold low value.
	'''     </UL>
	''' <LI>String monitors only:
	'''     <UL>
	'''     <LI>The observed attribute has matched the "string to compare" value.
	'''     <LI>The observed attribute has differed from the "string to compare" value.
	'''     </UL>
	''' </UL>
	''' 
	''' 
	''' @since 1.5
	''' </summary>
	Public Class MonitorNotification
		Inherits javax.management.Notification


	'    
	'     * ------------------------------------------
	'     *  PUBLIC VARIABLES
	'     * ------------------------------------------
	'     

		''' <summary>
		''' Notification type denoting that the observed object is not registered in the MBean server.
		''' This notification is fired by all kinds of monitors.
		''' <BR>The value of this notification type is <CODE>jmx.monitor.error.mbean</CODE>.
		''' </summary>
		Public Const OBSERVED_OBJECT_ERROR As String = "jmx.monitor.error.mbean"

		''' <summary>
		''' Notification type denoting that the observed attribute is not contained in the observed object.
		''' This notification is fired by all kinds of monitors.
		''' <BR>The value of this notification type is <CODE>jmx.monitor.error.attribute</CODE>.
		''' </summary>
		Public Const OBSERVED_ATTRIBUTE_ERROR As String = "jmx.monitor.error.attribute"

		''' <summary>
		''' Notification type denoting that the type of the observed attribute is not correct.
		''' This notification is fired by all kinds of monitors.
		''' <BR>The value of this notification type is <CODE>jmx.monitor.error.type</CODE>.
		''' </summary>
		Public Const OBSERVED_ATTRIBUTE_TYPE_ERROR As String = "jmx.monitor.error.type"

		''' <summary>
		''' Notification type denoting that the type of the thresholds, offset or modulus is not correct.
		''' This notification is fired by counter and gauge monitors.
		''' <BR>The value of this notification type is <CODE>jmx.monitor.error.threshold</CODE>.
		''' </summary>
		Public Const THRESHOLD_ERROR As String = "jmx.monitor.error.threshold"

		''' <summary>
		''' Notification type denoting that a non-predefined error type has occurred when trying to get the value of the observed attribute.
		''' This notification is fired by all kinds of monitors.
		''' <BR>The value of this notification type is <CODE>jmx.monitor.error.runtime</CODE>.
		''' </summary>
		Public Const RUNTIME_ERROR As String = "jmx.monitor.error.runtime"

		''' <summary>
		''' Notification type denoting that the observed attribute has reached the threshold value.
		''' This notification is only fired by counter monitors.
		''' <BR>The value of this notification type is <CODE>jmx.monitor.counter.threshold</CODE>.
		''' </summary>
		Public Const THRESHOLD_VALUE_EXCEEDED As String = "jmx.monitor.counter.threshold"

		''' <summary>
		''' Notification type denoting that the observed attribute has exceeded the threshold high value.
		''' This notification is only fired by gauge monitors.
		''' <BR>The value of this notification type is <CODE>jmx.monitor.gauge.high</CODE>.
		''' </summary>
		Public Const THRESHOLD_HIGH_VALUE_EXCEEDED As String = "jmx.monitor.gauge.high"

		''' <summary>
		''' Notification type denoting that the observed attribute has exceeded the threshold low value.
		''' This notification is only fired by gauge monitors.
		''' <BR>The value of this notification type is <CODE>jmx.monitor.gauge.low</CODE>.
		''' </summary>
		Public Const THRESHOLD_LOW_VALUE_EXCEEDED As String = "jmx.monitor.gauge.low"

		''' <summary>
		''' Notification type denoting that the observed attribute has matched the "string to compare" value.
		''' This notification is only fired by string monitors.
		''' <BR>The value of this notification type is <CODE>jmx.monitor.string.matches</CODE>.
		''' </summary>
		Public Const STRING_TO_COMPARE_VALUE_MATCHED As String = "jmx.monitor.string.matches"

		''' <summary>
		''' Notification type denoting that the observed attribute has differed from the "string to compare" value.
		''' This notification is only fired by string monitors.
		''' <BR>The value of this notification type is <CODE>jmx.monitor.string.differs</CODE>.
		''' </summary>
		Public Const STRING_TO_COMPARE_VALUE_DIFFERED As String = "jmx.monitor.string.differs"


	'    
	'     * ------------------------------------------
	'     *  PRIVATE VARIABLES
	'     * ------------------------------------------
	'     

		' Serial version 
		Private Const serialVersionUID As Long = -4608189663661929204L

		''' <summary>
		''' @serial Monitor notification observed object.
		''' </summary>
		Private observedObject As javax.management.ObjectName = Nothing

		''' <summary>
		''' @serial Monitor notification observed attribute.
		''' </summary>
		Private observedAttribute As String = Nothing

		''' <summary>
		''' @serial Monitor notification derived gauge.
		''' </summary>
		Private derivedGauge As Object = Nothing

		''' <summary>
		''' @serial Monitor notification release mechanism.
		'''         This value is used to keep the threshold/string (depending on the
		'''         monitor type) that triggered off this notification.
		''' </summary>
		Private trigger As Object = Nothing


	'    
	'     * ------------------------------------------
	'     *  CONSTRUCTORS
	'     * ------------------------------------------
	'     

		''' <summary>
		''' Creates a monitor notification object.
		''' </summary>
		''' <param name="type"> The notification type. </param>
		''' <param name="source"> The notification producer. </param>
		''' <param name="sequenceNumber"> The notification sequence number within the source object. </param>
		''' <param name="timeStamp"> The notification emission date. </param>
		''' <param name="msg"> The notification message. </param>
		''' <param name="obsObj"> The object observed by the producer of this notification. </param>
		''' <param name="obsAtt"> The attribute observed by the producer of this notification. </param>
		''' <param name="derGauge"> The derived gauge. </param>
		''' <param name="trigger"> The threshold/string (depending on the monitor type) that triggered the notification. </param>
		Friend Sub New(ByVal type As String, ByVal source As Object, ByVal sequenceNumber As Long, ByVal timeStamp As Long, ByVal msg As String, ByVal obsObj As javax.management.ObjectName, ByVal obsAtt As String, ByVal derGauge As Object, ByVal trigger As Object)

			MyBase.New(type, source, sequenceNumber, timeStamp, msg)
			Me.observedObject = obsObj
			Me.observedAttribute = obsAtt
			Me.derivedGauge = derGauge
			Me.trigger = trigger
		End Sub

	'    
	'     * ------------------------------------------
	'     *  PUBLIC METHODS
	'     * ------------------------------------------
	'     

		' GETTERS AND SETTERS
		'--------------------

		''' <summary>
		''' Gets the observed object of this monitor notification.
		''' </summary>
		''' <returns> The observed object. </returns>
		Public Overridable Property observedObject As javax.management.ObjectName
			Get
				Return observedObject
			End Get
		End Property

		''' <summary>
		''' Gets the observed attribute of this monitor notification.
		''' </summary>
		''' <returns> The observed attribute. </returns>
		Public Overridable Property observedAttribute As String
			Get
				Return observedAttribute
			End Get
		End Property

		''' <summary>
		''' Gets the derived gauge of this monitor notification.
		''' </summary>
		''' <returns> The derived gauge. </returns>
		Public Overridable Property derivedGauge As Object
			Get
				Return derivedGauge
			End Get
		End Property

		''' <summary>
		''' Gets the threshold/string (depending on the monitor type) that triggered off this monitor notification.
		''' </summary>
		''' <returns> The trigger. </returns>
		Public Overridable Property trigger As Object
			Get
				Return trigger
			End Get
		End Property

	End Class

End Namespace