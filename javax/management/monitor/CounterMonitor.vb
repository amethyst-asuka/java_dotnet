Imports Microsoft.VisualBasic
Imports System
Imports System.Runtime.CompilerServices
Imports System.Text
import static com.sun.jmx.defaults.JmxProperties.MONITOR_LOGGER
Imports javax.management.monitor.Monitor.NumericalType
Imports javax.management.monitor.MonitorNotification

'
' * Copyright (c) 1999, 2008, Oracle and/or its affiliates. All rights reserved.
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

	''' <summary>
	''' Defines a monitor MBean designed to observe the values of a counter
	''' attribute.
	''' 
	''' <P> A counter monitor sends a {@link
	''' MonitorNotification#THRESHOLD_VALUE_EXCEEDED threshold
	''' notification} when the value of the counter reaches or exceeds a
	''' threshold known as the comparison level.  The notify flag must be
	''' set to <CODE>true</CODE>.
	''' 
	''' <P> In addition, an offset mechanism enables particular counting
	''' intervals to be detected.  If the offset value is not zero,
	''' whenever the threshold is triggered by the counter value reaching a
	''' comparison level, that comparison level is incremented by the
	''' offset value.  This is regarded as taking place instantaneously,
	''' that is, before the count is incremented.  Thus, for each level,
	''' the threshold triggers an event notification every time the count
	''' increases by an interval equal to the offset value.
	''' 
	''' <P> If the counter can wrap around its maximum value, the modulus
	''' needs to be specified.  The modulus is the value at which the
	''' counter is reset to zero.
	''' 
	''' <P> If the counter difference mode is used, the value of the
	''' derived gauge is calculated as the difference between the observed
	''' counter values for two successive observations.  If this difference
	''' is negative, the value of the derived gauge is incremented by the
	''' value of the modulus.  The derived gauge value (V[t]) is calculated
	''' using the following method:
	''' 
	''' <UL>
	''' <LI>if (counter[t] - counter[t-GP]) is positive then
	''' V[t] = counter[t] - counter[t-GP]
	''' <LI>if (counter[t] - counter[t-GP]) is negative then
	''' V[t] = counter[t] - counter[t-GP] + MODULUS
	''' </UL>
	''' 
	''' This implementation of the counter monitor requires the observed
	''' attribute to be of the type integer (<CODE>Byte</CODE>,
	''' <CODE>Integer</CODE>, <CODE>Short</CODE>, <CODE>Long</CODE>).
	''' 
	''' 
	''' @since 1.5
	''' </summary>
	Public Class CounterMonitor
		Inherits Monitor
		Implements CounterMonitorMBean

	'    
	'     * ------------------------------------------
	'     *  PACKAGE CLASSES
	'     * ------------------------------------------
	'     

		Friend Class CounterMonitorObservedObject
			Inherits ObservedObject

			Public Sub New(ByVal observedObject As javax.management.ObjectName)
				MyBase.New(observedObject)
			End Sub

			<MethodImpl(MethodImplOptions.Synchronized)> _
			Public Property threshold As Number
				Get
					Return threshold
				End Get
				Set(ByVal threshold As Number)
					Me.threshold = threshold
				End Set
			End Property
			<MethodImpl(MethodImplOptions.Synchronized)> _
			Public Property previousScanCounter As Number
				Get
					Return previousScanCounter
				End Get
				Set(ByVal previousScanCounter As Number)
					Me.previousScanCounter = previousScanCounter
				End Set
			End Property
			<MethodImpl(MethodImplOptions.Synchronized)> _
			Public Property modulusExceeded As Boolean
				Get
					Return modulusExceeded
				End Get
				Set(ByVal modulusExceeded As Boolean)
					Me.modulusExceeded = modulusExceeded
				End Set
			End Property
			<MethodImpl(MethodImplOptions.Synchronized)> _
			Public Property derivedGaugeExceeded As Number
				Get
					Return derivedGaugeExceeded
				End Get
				Set(ByVal derivedGaugeExceeded As Number)
					Me.derivedGaugeExceeded = derivedGaugeExceeded
				End Set
			End Property
			<MethodImpl(MethodImplOptions.Synchronized)> _
			Public Property derivedGaugeValid As Boolean
				Get
					Return derivedGaugeValid
				End Get
				Set(ByVal derivedGaugeValid As Boolean)
					Me.derivedGaugeValid = derivedGaugeValid
				End Set
			End Property
			<MethodImpl(MethodImplOptions.Synchronized)> _
			Public Property eventAlreadyNotified As Boolean
				Get
					Return eventAlreadyNotified
				End Get
				Set(ByVal eventAlreadyNotified As Boolean)
					Me.eventAlreadyNotified = eventAlreadyNotified
				End Set
			End Property
			<MethodImpl(MethodImplOptions.Synchronized)> _
			Public Property type As NumericalType
				Get
					Return type
				End Get
				Set(ByVal type As NumericalType)
					Me.type = type
				End Set
			End Property

			Private threshold As Number
			Private previousScanCounter As Number
			Private modulusExceeded As Boolean
			Private derivedGaugeExceeded As Number
			Private derivedGaugeValid As Boolean
			Private eventAlreadyNotified As Boolean
			Private type As NumericalType
		End Class

	'    
	'     * ------------------------------------------
	'     *  PRIVATE VARIABLES
	'     * ------------------------------------------
	'     

		''' <summary>
		''' Counter modulus.
		''' <BR>The default value is a null Integer object.
		''' </summary>
		Private modulus As Number = INTEGER_ZERO

		''' <summary>
		''' Counter offset.
		''' <BR>The default value is a null Integer object.
		''' </summary>
		Private offset As Number = INTEGER_ZERO

		''' <summary>
		''' Flag indicating if the counter monitor notifies when exceeding
		''' the threshold.  The default value is set to
		''' <CODE>false</CODE>.
		''' </summary>
		Private notify As Boolean = False

		''' <summary>
		''' Flag indicating if the counter difference mode is used.  If the
		''' counter difference mode is used, the derived gauge is the
		''' difference between two consecutive observed values.  Otherwise,
		''' the derived gauge is directly the value of the observed
		''' attribute.  The default value is set to <CODE>false</CODE>.
		''' </summary>
		Private differenceMode As Boolean = False

		''' <summary>
		''' Initial counter threshold.  This value is used to initialize
		''' the threshold when a new object is added to the list and reset
		''' the threshold to its initial value each time the counter
		''' resets.
		''' </summary>
		Private initThreshold As Number = INTEGER_ZERO

		Private Shared ReadOnly types As String() = { RUNTIME_ERROR, OBSERVED_OBJECT_ERROR, OBSERVED_ATTRIBUTE_ERROR, OBSERVED_ATTRIBUTE_TYPE_ERROR, THRESHOLD_ERROR, THRESHOLD_VALUE_EXCEEDED }

		Private Shared ReadOnly notifsInfo As javax.management.MBeanNotificationInfo() = { New javax.management.MBeanNotificationInfo(types, "javax.management.monitor.MonitorNotification", "Notifications sent by the CounterMonitor MBean") }

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
		''' Starts the counter monitor.
		''' </summary>
		<MethodImpl(MethodImplOptions.Synchronized)> _
		Public Overrides Sub start() Implements MonitorMBean.start
			If active Then
				MONITOR_LOGGER.logp(java.util.logging.Level.FINER, GetType(CounterMonitor).name, "start", "the monitor is already active")
				Return
			End If
			' Reset values.
			'
			For Each o As ObservedObject In observedObjects
				Dim cmo As CounterMonitorObservedObject = CType(o, CounterMonitorObservedObject)
				cmo.threshold = initThreshold
				cmo.modulusExceeded = False
				cmo.eventAlreadyNotified = False
				cmo.previousScanCounter = Nothing
			Next o
			doStart()
		End Sub

		''' <summary>
		''' Stops the counter monitor.
		''' </summary>
		<MethodImpl(MethodImplOptions.Synchronized)> _
		Public Overrides Sub [stop]() Implements MonitorMBean.stop
			doStop()
		End Sub

		' GETTERS AND SETTERS
		'--------------------

		''' <summary>
		''' Gets the derived gauge of the specified object, if this object is
		''' contained in the set of observed MBeans, or <code>null</code> otherwise.
		''' </summary>
		''' <param name="object"> the name of the object whose derived gauge is to
		''' be returned.
		''' </param>
		''' <returns> The derived gauge of the specified object.
		'''  </returns>
		<MethodImpl(MethodImplOptions.Synchronized)> _
		Public Overrides Function getDerivedGauge(ByVal [object] As javax.management.ObjectName) As Number Implements CounterMonitorMBean.getDerivedGauge
			Return CType(MyBase.getDerivedGauge([object]), Number)
		End Function

		''' <summary>
		''' Gets the derived gauge timestamp of the specified object, if
		''' this object is contained in the set of observed MBeans, or
		''' <code>0</code> otherwise.
		''' </summary>
		''' <param name="object"> the name of the object whose derived gauge
		''' timestamp is to be returned.
		''' </param>
		''' <returns> The derived gauge timestamp of the specified object.
		'''  </returns>
		<MethodImpl(MethodImplOptions.Synchronized)> _
		Public Overrides Function getDerivedGaugeTimeStamp(ByVal [object] As javax.management.ObjectName) As Long Implements CounterMonitorMBean.getDerivedGaugeTimeStamp
			Return MyBase.getDerivedGaugeTimeStamp([object])
		End Function

		''' <summary>
		''' Gets the current threshold value of the specified object, if
		''' this object is contained in the set of observed MBeans, or
		''' <code>null</code> otherwise.
		''' </summary>
		''' <param name="object"> the name of the object whose threshold is to be
		''' returned.
		''' </param>
		''' <returns> The threshold value of the specified object.
		'''  </returns>
		<MethodImpl(MethodImplOptions.Synchronized)> _
		Public Overridable Function getThreshold(ByVal [object] As javax.management.ObjectName) As Number Implements CounterMonitorMBean.getThreshold
			Dim o As CounterMonitorObservedObject = CType(getObservedObject([object]), CounterMonitorObservedObject)
			If o Is Nothing Then Return Nothing

			' If the counter that is monitored rolls over when it reaches a
			' maximum value, then the modulus value needs to be set to that
			' maximum value. The threshold will then also roll over whenever
			' it strictly exceeds the modulus value. When the threshold rolls
			' over, it is reset to the value that was specified through the
			' latest call to the monitor's setInitThreshold method, before
			' any offsets were applied.
			'
			If offset > 0L AndAlso modulus > 0L AndAlso o.threshold > modulus Then
				Return initThreshold
			Else
				Return o.threshold
			End If
		End Function

		''' <summary>
		''' Gets the initial threshold value common to all observed objects.
		''' </summary>
		''' <returns> The initial threshold.
		''' </returns>
		''' <seealso cref= #setInitThreshold
		'''  </seealso>
		<MethodImpl(MethodImplOptions.Synchronized)> _
		Public Overridable Property initThreshold As Number Implements CounterMonitorMBean.getInitThreshold
			Get
				Return initThreshold
			End Get
			Set(ByVal value As Number)
    
				If value Is Nothing Then Throw New System.ArgumentException("Null threshold")
				If value < 0L Then Throw New System.ArgumentException("Negative threshold")
    
				If initThreshold.Equals(value) Then Return
				initThreshold = value
    
				' Reset values.
				'
				Dim index As Integer = 0
				For Each o As ObservedObject In observedObjects
					resetAlreadyNotified(o, index, THRESHOLD_ERROR_NOTIFIED)
					index += 1
					Dim cmo As CounterMonitorObservedObject = CType(o, CounterMonitorObservedObject)
					cmo.threshold = value
					cmo.modulusExceeded = False
					cmo.eventAlreadyNotified = False
				Next o
			End Set
		End Property


		''' <summary>
		''' Returns the derived gauge of the first object in the set of
		''' observed MBeans.
		''' </summary>
		''' <returns> The derived gauge.
		''' </returns>
		''' @deprecated As of JMX 1.2, replaced by
		''' <seealso cref="#getDerivedGauge(ObjectName)"/> 
		<Obsolete("As of JMX 1.2, replaced by"), MethodImpl(MethodImplOptions.Synchronized)> _
		Public Overridable Property derivedGauge As Number Implements CounterMonitorMBean.getDerivedGauge
			Get
				If observedObjects.Count = 0 Then
					Return Nothing
				Else
					Return CType(observedObjects(0).derivedGauge, Number)
				End If
			End Get
		End Property

		''' <summary>
		''' Gets the derived gauge timestamp of the first object in the set
		''' of observed MBeans.
		''' </summary>
		''' <returns> The derived gauge timestamp.
		''' </returns>
		''' @deprecated As of JMX 1.2, replaced by
		''' <seealso cref="#getDerivedGaugeTimeStamp(ObjectName)"/> 
		<Obsolete("As of JMX 1.2, replaced by"), MethodImpl(MethodImplOptions.Synchronized)> _
		Public Overridable Property derivedGaugeTimeStamp As Long Implements CounterMonitorMBean.getDerivedGaugeTimeStamp
			Get
				If observedObjects.Count = 0 Then
					Return 0
				Else
					Return observedObjects(0).derivedGaugeTimeStamp
				End If
			End Get
		End Property

		''' <summary>
		''' Gets the threshold value of the first object in the set of
		''' observed MBeans.
		''' </summary>
		''' <returns> The threshold value.
		''' </returns>
		''' <seealso cref= #setThreshold
		''' </seealso>
		''' @deprecated As of JMX 1.2, replaced by <seealso cref="#getThreshold(ObjectName)"/> 
		<Obsolete("As of JMX 1.2, replaced by <seealso cref="#getThreshold(javax.management.ObjectName)"/>"), MethodImpl(MethodImplOptions.Synchronized)> _
		Public Overridable Property threshold As Number Implements CounterMonitorMBean.getThreshold
			Get
				Return getThreshold(observedObject)
			End Get
			Set(ByVal value As Number)
				initThreshold = value
			End Set
		End Property


		''' <summary>
		''' Gets the offset value common to all observed MBeans.
		''' </summary>
		''' <returns> The offset value.
		''' </returns>
		''' <seealso cref= #setOffset </seealso>
		<MethodImpl(MethodImplOptions.Synchronized)> _
		Public Overridable Property offset As Number Implements CounterMonitorMBean.getOffset
			Get
				Return offset
			End Get
			Set(ByVal value As Number)
    
				If value Is Nothing Then Throw New System.ArgumentException("Null offset")
				If value < 0L Then Throw New System.ArgumentException("Negative offset")
    
				If offset.Equals(value) Then Return
				offset = value
    
				Dim index As Integer = 0
				For Each o As ObservedObject In observedObjects
					resetAlreadyNotified(o, index, THRESHOLD_ERROR_NOTIFIED)
					index += 1
				Next o
			End Set
		End Property


		''' <summary>
		''' Gets the modulus value common to all observed MBeans.
		''' </summary>
		''' <seealso cref= #setModulus
		''' </seealso>
		''' <returns> The modulus value. </returns>
		<MethodImpl(MethodImplOptions.Synchronized)> _
		Public Overridable Property modulus As Number Implements CounterMonitorMBean.getModulus
			Get
				Return modulus
			End Get
			Set(ByVal value As Number)
    
				If value Is Nothing Then Throw New System.ArgumentException("Null modulus")
				If value < 0L Then Throw New System.ArgumentException("Negative modulus")
    
				If modulus.Equals(value) Then Return
				modulus = value
    
				' Reset values.
				'
				Dim index As Integer = 0
				For Each o As ObservedObject In observedObjects
					resetAlreadyNotified(o, index, THRESHOLD_ERROR_NOTIFIED)
					index += 1
					Dim cmo As CounterMonitorObservedObject = CType(o, CounterMonitorObservedObject)
					cmo.modulusExceeded = False
				Next o
			End Set
		End Property


		''' <summary>
		''' Gets the notification's on/off switch value common to all
		''' observed MBeans.
		''' </summary>
		''' <returns> <CODE>true</CODE> if the counter monitor notifies when
		''' exceeding the threshold, <CODE>false</CODE> otherwise.
		''' </returns>
		''' <seealso cref= #setNotify </seealso>
		<MethodImpl(MethodImplOptions.Synchronized)> _
		Public Overridable Property notify As Boolean Implements CounterMonitorMBean.getNotify
			Get
				Return notify
			End Get
			Set(ByVal value As Boolean)
				If notify = value Then Return
				notify = value
			End Set
		End Property


		''' <summary>
		''' Gets the difference mode flag value common to all observed MBeans.
		''' </summary>
		''' <returns> <CODE>true</CODE> if the difference mode is used,
		''' <CODE>false</CODE> otherwise.
		''' </returns>
		''' <seealso cref= #setDifferenceMode </seealso>
		<MethodImpl(MethodImplOptions.Synchronized)> _
		Public Overridable Property differenceMode As Boolean Implements CounterMonitorMBean.getDifferenceMode
			Get
				Return differenceMode
			End Get
			Set(ByVal value As Boolean)
				If differenceMode = value Then Return
				differenceMode = value
    
				' Reset values.
				'
				For Each o As ObservedObject In observedObjects
					Dim cmo As CounterMonitorObservedObject = CType(o, CounterMonitorObservedObject)
					cmo.threshold = initThreshold
					cmo.modulusExceeded = False
					cmo.eventAlreadyNotified = False
					cmo.previousScanCounter = Nothing
				Next o
			End Set
		End Property


		''' <summary>
		''' Returns a <CODE>NotificationInfo</CODE> object containing the
		''' name of the Java class of the notification and the notification
		''' types sent by the counter monitor.
		''' </summary>
		Public Property Overrides notificationInfo As javax.management.MBeanNotificationInfo()
			Get
				Return notifsInfo.clone()
			End Get
		End Property

	'    
	'     * ------------------------------------------
	'     *  PRIVATE METHODS
	'     * ------------------------------------------
	'     

		''' <summary>
		''' Updates the derived gauge attribute of the observed object.
		''' </summary>
		''' <param name="scanCounter"> The value of the observed attribute. </param>
		''' <param name="o"> The observed object. </param>
		''' <returns> <CODE>true</CODE> if the derived gauge value is valid,
		''' <CODE>false</CODE> otherwise.  The derived gauge value is
		''' invalid when the differenceMode flag is set to
		''' <CODE>true</CODE> and it is the first notification (so we
		''' haven't 2 consecutive values to update the derived gauge). </returns>
		<MethodImpl(MethodImplOptions.Synchronized)> _
		Private Function updateDerivedGauge(ByVal scanCounter As Object, ByVal o As CounterMonitorObservedObject) As Boolean

			Dim is_derived_gauge_valid As Boolean

			' The counter difference mode is used.
			'
			If differenceMode Then

				' The previous scan counter has been initialized.
				'
				If o.previousScanCounter IsNot Nothing Then
					derivedGaugeWithDifferencence(CType(scanCounter, Number), Nothing, o)

					' If derived gauge is negative it means that the
					' counter has wrapped around and the value of the
					' threshold needs to be reset to its initial value.
					'
					If CType(o.derivedGauge, Number) < 0L Then
						If modulus > 0L Then derivedGaugeWithDifferencence(CType(scanCounter, Number), modulus, o)
						o.threshold = initThreshold
						o.eventAlreadyNotified = False
					End If
					is_derived_gauge_valid = True
				' The previous scan counter has not been initialized.
				' We cannot update the derived gauge...
				'
				Else
					is_derived_gauge_valid = False
				End If
				o.previousScanCounter = CType(scanCounter, Number)
			' The counter difference mode is not used.
			'
			Else
				o.derivedGauge = CType(scanCounter, Number)
				is_derived_gauge_valid = True
			End If
			Return is_derived_gauge_valid
		End Function

		''' <summary>
		''' Updates the notification attribute of the observed object
		''' and notifies the listeners only once if the notify flag
		''' is set to <CODE>true</CODE>. </summary>
		''' <param name="o"> The observed object. </param>
		<MethodImpl(MethodImplOptions.Synchronized)> _
		Private Function updateNotifications(ByVal o As CounterMonitorObservedObject) As MonitorNotification

			Dim n As MonitorNotification = Nothing

			' Send notification if notify is true.
			'
			If Not o.eventAlreadyNotified Then
				If CType(o.derivedGauge, Number) >= o.threshold Then
					If notify Then n = New MonitorNotification(THRESHOLD_VALUE_EXCEEDED, Me, 0, 0, "", Nothing, Nothing, Nothing, o.threshold)
					If Not differenceMode Then o.eventAlreadyNotified = True
				End If
			Else
				If MONITOR_LOGGER.isLoggable(java.util.logging.Level.FINER) Then
					Dim strb As (New StringBuilder).Append("The notification:").append(vbLf & vbTab & "Notification observed object = ").append(o.observedObject).append(vbLf & vbTab & "Notification observed attribute = ").append(observedAttribute).append(vbLf & vbTab & "Notification threshold level = ").append(o.threshold).append(vbLf & vbTab & "Notification derived gauge = ").append(o.derivedGauge).append(vbLf & "has already been sent")
					MONITOR_LOGGER.logp(java.util.logging.Level.FINER, GetType(CounterMonitor).name, "updateNotifications", strb.ToString())
				End If
			End If

			Return n
		End Function

		''' <summary>
		''' Updates the threshold attribute of the observed object. </summary>
		''' <param name="o"> The observed object. </param>
		<MethodImpl(MethodImplOptions.Synchronized)> _
		Private Sub updateThreshold(ByVal o As CounterMonitorObservedObject)

			' Calculate the new threshold value if the threshold has been
			' exceeded and if the offset value is greater than zero.
			'
			If CType(o.derivedGauge, Number) >= o.threshold Then

				If offset > 0L Then

					' Increment the threshold until its value is greater
					' than the one for the current derived gauge.
					'
					Dim threshold_value As Long = o.threshold
					Do While CType(o.derivedGauge, Number) >= threshold_value
						threshold_value += offset
					Loop

					' Set threshold attribute.
					'
					Select Case o.type
						Case INTEGER
							o.threshold = Convert.ToInt32(CInt(threshold_value))
						Case BYTE
							o.threshold = Convert.ToByte(CByte(threshold_value))
						Case SHORT
							o.threshold = Convert.ToInt16(CShort(threshold_value))
						Case LONG
							o.threshold = Convert.ToInt64(threshold_value)
						Case Else
							' Should never occur...
							MONITOR_LOGGER.logp(java.util.logging.Level.FINEST, GetType(CounterMonitor).name, "updateThreshold", "the threshold type is invalid")
					End Select

					' If the counter can wrap around when it reaches
					' its maximum and we are not dealing with counter
					' differences then we need to reset the threshold
					' to its initial value too.
					'
					If Not differenceMode Then
						If modulus > 0L Then
							If o.threshold > modulus Then
								o.modulusExceeded = True
								o.derivedGaugeExceeded = CType(o.derivedGauge, Number)
							End If
						End If
					End If

					' Threshold value has been modified so we can notify again.
					'
					o.eventAlreadyNotified = False
				Else
					o.modulusExceeded = True
					o.derivedGaugeExceeded = CType(o.derivedGauge, Number)
				End If
			End If
		End Sub

		''' <summary>
		''' Sets the derived gauge of the specified observed object when the
		''' differenceMode flag is set to <CODE>true</CODE>.  Integer types
		''' only are allowed.
		''' </summary>
		''' <param name="scanCounter"> The value of the observed attribute. </param>
		''' <param name="mod"> The counter modulus value. </param>
		''' <param name="o"> The observed object. </param>
		<MethodImpl(MethodImplOptions.Synchronized)> _
		Private Sub setDerivedGaugeWithDifference(ByVal scanCounter As Number, ByVal [mod] As Number, ByVal o As CounterMonitorObservedObject)
	'         We do the arithmetic using longs here even though the
	'           result may end up in a smaller type.  Since
	'           l == (byte)l (mod 256) for any long l,
	'           (byte) ((byte)l1 + (byte)l2) == (byte) (l1 + l2),
	'           and likewise for subtraction.  So it's the same as if
	'           we had done the arithmetic in the smaller type.

			Dim derived As Long = scanCounter - o.previousScanCounter
			If [mod] IsNot Nothing Then derived += modulus

			Select Case o.type
			Case INTEGER
				o.derivedGauge = Convert.ToInt32(CInt(derived))
			Case BYTE
				o.derivedGauge = Convert.ToByte(CByte(derived))
			Case SHORT
				o.derivedGauge = Convert.ToInt16(CShort(derived))
			Case LONG
				o.derivedGauge = Convert.ToInt64(derived)
			Case Else
				' Should never occur...
				MONITOR_LOGGER.logp(java.util.logging.Level.FINEST, GetType(CounterMonitor).name, "setDerivedGaugeWithDifference", "the threshold type is invalid")
			End Select
		End Sub

	'    
	'     * ------------------------------------------
	'     *  PACKAGE METHODS
	'     * ------------------------------------------
	'     

		''' <summary>
		''' Factory method for ObservedObject creation.
		''' 
		''' @since 1.6
		''' </summary>
		Friend Overrides Function createObservedObject(ByVal [object] As javax.management.ObjectName) As ObservedObject
			Dim cmo As New CounterMonitorObservedObject([object])
			cmo.threshold = initThreshold
			cmo.modulusExceeded = False
			cmo.eventAlreadyNotified = False
			cmo.previousScanCounter = Nothing
			Return cmo
		End Function

		''' <summary>
		''' This method globally sets the derived gauge type for the given
		''' "object" and "attribute" after checking that the type of the
		''' supplied observed attribute value is one of the value types
		''' supported by this monitor.
		''' </summary>
		<MethodImpl(MethodImplOptions.Synchronized)> _
		Friend Overrides Function isComparableTypeValid(Of T1)(ByVal [object] As javax.management.ObjectName, ByVal attribute As String, ByVal value As IComparable(Of T1)) As Boolean
			Dim o As CounterMonitorObservedObject = CType(getObservedObject([object]), CounterMonitorObservedObject)
			If o Is Nothing Then Return False

			' Check that the observed attribute is of type "Integer".
			'
			If TypeOf value Is Integer? Then
				o.type = INTEGER
			ElseIf TypeOf value Is SByte? Then
				o.type = BYTE
			ElseIf TypeOf value Is Short? Then
				o.type = SHORT
			ElseIf TypeOf value Is Long? Then
				o.type = LONG
			Else
				Return False
			End If
			Return True
		End Function

'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
		<MethodImpl(MethodImplOptions.Synchronized)> _
		Friend Overrides Function getDerivedGaugeFromComparable(Of T1)(ByVal [object] As javax.management.ObjectName, ByVal attribute As String, ByVal value As IComparable(Of T1)) As IComparable(Of ?)
			Dim o As CounterMonitorObservedObject = CType(getObservedObject([object]), CounterMonitorObservedObject)
			If o Is Nothing Then Return Nothing

			' Check if counter has wrapped around.
			'
			If o.modulusExceeded Then
				If CType(o.derivedGauge, Number) < o.derivedGaugeExceeded Then
						o.threshold = initThreshold
						o.modulusExceeded = False
						o.eventAlreadyNotified = False
				End If
			End If

			' Update the derived gauge attributes and check the
			' validity of the new value. The derived gauge value
			' is invalid when the differenceMode flag is set to
			' true and it is the first notification, i.e. we
			' haven't got 2 consecutive values to update the
			' derived gauge.
			'
			o.derivedGaugeValid = updateDerivedGauge(value, o)

'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
			Return CType(o.derivedGauge, IComparable(Of ?))
		End Function

		<MethodImpl(MethodImplOptions.Synchronized)> _
		Friend Overrides Sub onErrorNotification(ByVal ___notification As MonitorNotification)
			Dim o As CounterMonitorObservedObject = CType(getObservedObject(___notification.observedObject), CounterMonitorObservedObject)
			If o Is Nothing Then Return

			' Reset values.
			'
			o.modulusExceeded = False
			o.eventAlreadyNotified = False
			o.previousScanCounter = Nothing
		End Sub

		<MethodImpl(MethodImplOptions.Synchronized)> _
		Friend Overrides Function buildAlarmNotification(Of T1)(ByVal [object] As javax.management.ObjectName, ByVal attribute As String, ByVal value As IComparable(Of T1)) As MonitorNotification
			Dim o As CounterMonitorObservedObject = CType(getObservedObject([object]), CounterMonitorObservedObject)
			If o Is Nothing Then Return Nothing

			' Notify the listeners and update the threshold if
			' the updated derived gauge value is valid.
			'
			Dim alarm As MonitorNotification
			If o.derivedGaugeValid Then
				alarm = updateNotifications(o)
				updateThreshold(o)
			Else
				alarm = Nothing
			End If
			Return alarm
		End Function

		''' <summary>
		''' Tests if the threshold, offset and modulus of the specified observed
		''' object are of the same type as the counter. Only integer types are
		''' allowed.
		''' 
		''' Note:
		'''   If the optional offset or modulus have not been initialized, their
		'''   default value is an Integer object with a value equal to zero.
		''' </summary>
		''' <param name="object"> The observed object. </param>
		''' <param name="attribute"> The observed attribute. </param>
		''' <param name="value"> The sample value. </param>
		''' <returns> <CODE>true</CODE> if type is the same,
		''' <CODE>false</CODE> otherwise. </returns>
		<MethodImpl(MethodImplOptions.Synchronized)> _
		Friend Overrides Function isThresholdTypeValid(Of T1)(ByVal [object] As javax.management.ObjectName, ByVal attribute As String, ByVal value As IComparable(Of T1)) As Boolean
			Dim o As CounterMonitorObservedObject = CType(getObservedObject([object]), CounterMonitorObservedObject)
			If o Is Nothing Then Return False

			Dim c As Type = classForType(o.type)
			Return (c.IsInstanceOfType(o.threshold) AndAlso isValidForType(offset, c) AndAlso isValidForType(modulus, c))
		End Function
	End Class

End Namespace