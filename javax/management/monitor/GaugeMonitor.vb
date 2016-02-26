Imports Microsoft.VisualBasic
Imports System
Imports System.Runtime.CompilerServices
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
	''' Defines a monitor MBean designed to observe the values of a gauge attribute.
	''' 
	''' <P> A gauge monitor observes an attribute that is continuously
	''' variable with time. A gauge monitor sends notifications as
	''' follows:
	''' 
	''' <UL>
	''' 
	''' <LI> if the attribute value is increasing and becomes equal to or
	''' greater than the high threshold value, a {@link
	''' MonitorNotification#THRESHOLD_HIGH_VALUE_EXCEEDED threshold high
	''' notification} is sent. The notify high flag must be set to
	''' <CODE>true</CODE>.
	''' 
	''' <BR>Subsequent crossings of the high threshold value do not cause
	''' further notifications unless the attribute value becomes equal to
	''' or less than the low threshold value.</LI>
	''' 
	''' <LI> if the attribute value is decreasing and becomes equal to or
	''' less than the low threshold value, a {@link
	''' MonitorNotification#THRESHOLD_LOW_VALUE_EXCEEDED threshold low
	''' notification} is sent. The notify low flag must be set to
	''' <CODE>true</CODE>.
	''' 
	''' <BR>Subsequent crossings of the low threshold value do not cause
	''' further notifications unless the attribute value becomes equal to
	''' or greater than the high threshold value.</LI>
	''' 
	''' </UL>
	''' 
	''' This provides a hysteresis mechanism to avoid repeated triggering
	''' of notifications when the attribute value makes small oscillations
	''' around the high or low threshold value.
	''' 
	''' <P> If the gauge difference mode is used, the value of the derived
	''' gauge is calculated as the difference between the observed gauge
	''' values for two successive observations.
	''' 
	''' <BR>The derived gauge value (V[t]) is calculated using the following method:
	''' <UL>
	''' <LI>V[t] = gauge[t] - gauge[t-GP]</LI>
	''' </UL>
	''' 
	''' This implementation of the gauge monitor requires the observed
	''' attribute to be of the type integer or floating-point
	''' (<CODE>Byte</CODE>, <CODE>Integer</CODE>, <CODE>Short</CODE>,
	''' <CODE>Long</CODE>, <CODE>Float</CODE>, <CODE>Double</CODE>).
	''' 
	''' 
	''' @since 1.5
	''' </summary>
	Public Class GaugeMonitor
		Inherits Monitor
		Implements GaugeMonitorMBean

	'    
	'     * ------------------------------------------
	'     *  PACKAGE CLASSES
	'     * ------------------------------------------
	'     

		Friend Class GaugeMonitorObservedObject
			Inherits ObservedObject

			Public Sub New(ByVal observedObject As javax.management.ObjectName)
				MyBase.New(observedObject)
			End Sub

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
			Public Property type As NumericalType
				Get
					Return type
				End Get
				Set(ByVal type As NumericalType)
					Me.type = type
				End Set
			End Property
			<MethodImpl(MethodImplOptions.Synchronized)> _
			Public Property previousScanGauge As Number
				Get
					Return previousScanGauge
				End Get
				Set(ByVal previousScanGauge As Number)
					Me.previousScanGauge = previousScanGauge
				End Set
			End Property
			<MethodImpl(MethodImplOptions.Synchronized)> _
			Public Property status As Integer
				Get
					Return status
				End Get
				Set(ByVal status As Integer)
					Me.status = status
				End Set
			End Property

			Private derivedGaugeValid As Boolean
			Private type As NumericalType
			Private previousScanGauge As Number
			Private status As Integer
		End Class

	'    
	'     * ------------------------------------------
	'     *  PRIVATE VARIABLES
	'     * ------------------------------------------
	'     

		''' <summary>
		''' Gauge high threshold.
		''' 
		''' <BR>The default value is a null Integer object.
		''' </summary>
		Private highThreshold As Number = INTEGER_ZERO

		''' <summary>
		''' Gauge low threshold.
		''' 
		''' <BR>The default value is a null Integer object.
		''' </summary>
		Private lowThreshold As Number = INTEGER_ZERO

		''' <summary>
		''' Flag indicating if the gauge monitor notifies when exceeding
		''' the high threshold.
		''' 
		''' <BR>The default value is <CODE>false</CODE>.
		''' </summary>
		Private notifyHigh As Boolean = False

		''' <summary>
		''' Flag indicating if the gauge monitor notifies when exceeding
		''' the low threshold.
		''' 
		''' <BR>The default value is <CODE>false</CODE>.
		''' </summary>
		Private notifyLow As Boolean = False

		''' <summary>
		''' Flag indicating if the gauge difference mode is used.  If the
		''' gauge difference mode is used, the derived gauge is the
		''' difference between two consecutive observed values.  Otherwise,
		''' the derived gauge is directly the value of the observed
		''' attribute.
		''' 
		''' <BR>The default value is set to <CODE>false</CODE>.
		''' </summary>
		Private differenceMode As Boolean = False

		Private Shared ReadOnly types As String() = { RUNTIME_ERROR, OBSERVED_OBJECT_ERROR, OBSERVED_ATTRIBUTE_ERROR, OBSERVED_ATTRIBUTE_TYPE_ERROR, THRESHOLD_ERROR, THRESHOLD_HIGH_VALUE_EXCEEDED, THRESHOLD_LOW_VALUE_EXCEEDED }

		Private Shared ReadOnly notifsInfo As javax.management.MBeanNotificationInfo() = { New javax.management.MBeanNotificationInfo(types, "javax.management.monitor.MonitorNotification", "Notifications sent by the GaugeMonitor MBean") }

		' Flags needed to implement the hysteresis mechanism.
		'
		Private Const RISING As Integer = 0
		Private Const FALLING As Integer = 1
		Private Const RISING_OR_FALLING As Integer = 2

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
		''' Starts the gauge monitor.
		''' </summary>
		<MethodImpl(MethodImplOptions.Synchronized)> _
		Public Overrides Sub start() Implements MonitorMBean.start
			If active Then
				MONITOR_LOGGER.logp(java.util.logging.Level.FINER, GetType(GaugeMonitor).name, "start", "the monitor is already active")
				Return
			End If
			' Reset values.
			'
			For Each o As ObservedObject In observedObjects
				Dim gmo As GaugeMonitorObservedObject = CType(o, GaugeMonitorObservedObject)
				gmo.status = RISING_OR_FALLING
				gmo.previousScanGauge = Nothing
			Next o
			doStart()
		End Sub

		''' <summary>
		''' Stops the gauge monitor.
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
		''' <param name="object"> the name of the MBean.
		''' </param>
		''' <returns> The derived gauge of the specified object.
		'''  </returns>
		<MethodImpl(MethodImplOptions.Synchronized)> _
		Public Overrides Function getDerivedGauge(ByVal [object] As javax.management.ObjectName) As Number Implements GaugeMonitorMBean.getDerivedGauge
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
		Public Overrides Function getDerivedGaugeTimeStamp(ByVal [object] As javax.management.ObjectName) As Long Implements GaugeMonitorMBean.getDerivedGaugeTimeStamp
			Return MyBase.getDerivedGaugeTimeStamp([object])
		End Function

		''' <summary>
		''' Returns the derived gauge of the first object in the set of
		''' observed MBeans.
		''' </summary>
		''' <returns> The derived gauge.
		''' </returns>
		''' @deprecated As of JMX 1.2, replaced by
		''' <seealso cref="#getDerivedGauge(ObjectName)"/> 
		<Obsolete("As of JMX 1.2, replaced by"), MethodImpl(MethodImplOptions.Synchronized)> _
		Public Overridable Property derivedGauge As Number Implements GaugeMonitorMBean.getDerivedGauge
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
		Public Overridable Property derivedGaugeTimeStamp As Long Implements GaugeMonitorMBean.getDerivedGaugeTimeStamp
			Get
				If observedObjects.Count = 0 Then
					Return 0
				Else
					Return observedObjects(0).derivedGaugeTimeStamp
				End If
			End Get
		End Property

		''' <summary>
		''' Gets the high threshold value common to all observed MBeans.
		''' </summary>
		''' <returns> The high threshold value.
		''' </returns>
		''' <seealso cref= #setThresholds </seealso>
		<MethodImpl(MethodImplOptions.Synchronized)> _
		Public Overridable Property highThreshold As Number Implements GaugeMonitorMBean.getHighThreshold
			Get
				Return highThreshold
			End Get
		End Property

		''' <summary>
		''' Gets the low threshold value common to all observed MBeans.
		''' </summary>
		''' <returns> The low threshold value.
		''' </returns>
		''' <seealso cref= #setThresholds </seealso>
		<MethodImpl(MethodImplOptions.Synchronized)> _
		Public Overridable Property lowThreshold As Number Implements GaugeMonitorMBean.getLowThreshold
			Get
				Return lowThreshold
			End Get
		End Property

		''' <summary>
		''' Sets the high and the low threshold values common to all
		''' observed MBeans.
		''' </summary>
		''' <param name="highValue"> The high threshold value. </param>
		''' <param name="lowValue"> The low threshold value.
		''' </param>
		''' <exception cref="IllegalArgumentException"> The specified high/low
		''' threshold is null or the low threshold is greater than the high
		''' threshold or the high threshold and the low threshold are not
		''' of the same type.
		''' </exception>
		''' <seealso cref= #getHighThreshold </seealso>
		''' <seealso cref= #getLowThreshold </seealso>
		<MethodImpl(MethodImplOptions.Synchronized)> _
		Public Overridable Sub setThresholds(ByVal highValue As Number, ByVal lowValue As Number) Implements GaugeMonitorMBean.setThresholds

			If (highValue Is Nothing) OrElse (lowValue Is Nothing) Then Throw New System.ArgumentException("Null threshold value")

			If highValue.GetType() IsNot lowValue.GetType() Then Throw New System.ArgumentException("Different type " & "threshold values")

			If isFirstStrictlyGreaterThanLast(lowValue, highValue, highValue.GetType().name) Then Throw New System.ArgumentException("High threshold less than " & "low threshold")

			If highThreshold.Equals(highValue) AndAlso lowThreshold.Equals(lowValue) Then Return
			highThreshold = highValue
			lowThreshold = lowValue

			' Reset values.
			'
			Dim index As Integer = 0
			For Each o As ObservedObject In observedObjects
				resetAlreadyNotified(o, index, THRESHOLD_ERROR_NOTIFIED)
				index += 1
				Dim gmo As GaugeMonitorObservedObject = CType(o, GaugeMonitorObservedObject)
				gmo.status = RISING_OR_FALLING
			Next o
		End Sub

		''' <summary>
		''' Gets the high notification's on/off switch value common to all
		''' observed MBeans.
		''' </summary>
		''' <returns> <CODE>true</CODE> if the gauge monitor notifies when
		''' exceeding the high threshold, <CODE>false</CODE> otherwise.
		''' </returns>
		''' <seealso cref= #setNotifyHigh </seealso>
		<MethodImpl(MethodImplOptions.Synchronized)> _
		Public Overridable Property notifyHigh As Boolean Implements GaugeMonitorMBean.getNotifyHigh
			Get
				Return notifyHigh
			End Get
			Set(ByVal value As Boolean)
				If notifyHigh = value Then Return
				notifyHigh = value
			End Set
		End Property


		''' <summary>
		''' Gets the low notification's on/off switch value common to all
		''' observed MBeans.
		''' </summary>
		''' <returns> <CODE>true</CODE> if the gauge monitor notifies when
		''' exceeding the low threshold, <CODE>false</CODE> otherwise.
		''' </returns>
		''' <seealso cref= #setNotifyLow </seealso>
		<MethodImpl(MethodImplOptions.Synchronized)> _
		Public Overridable Property notifyLow As Boolean Implements GaugeMonitorMBean.getNotifyLow
			Get
				Return notifyLow
			End Get
			Set(ByVal value As Boolean)
				If notifyLow = value Then Return
				notifyLow = value
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
		Public Overridable Property differenceMode As Boolean Implements GaugeMonitorMBean.getDifferenceMode
			Get
				Return differenceMode
			End Get
			Set(ByVal value As Boolean)
				If differenceMode = value Then Return
				differenceMode = value
    
				' Reset values.
				'
				For Each o As ObservedObject In observedObjects
					Dim gmo As GaugeMonitorObservedObject = CType(o, GaugeMonitorObservedObject)
					gmo.status = RISING_OR_FALLING
					gmo.previousScanGauge = Nothing
				Next o
			End Set
		End Property


	   ''' <summary>
	   ''' Returns a <CODE>NotificationInfo</CODE> object containing the
	   ''' name of the Java class of the notification and the notification
	   ''' types sent by the gauge monitor.
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
		''' <param name="scanGauge"> The value of the observed attribute. </param>
		''' <param name="o"> The observed object. </param>
		''' <returns> <CODE>true</CODE> if the derived gauge value is valid,
		''' <CODE>false</CODE> otherwise.  The derived gauge value is
		''' invalid when the differenceMode flag is set to
		''' <CODE>true</CODE> and it is the first notification (so we
		''' haven't 2 consecutive values to update the derived gauge). </returns>
		<MethodImpl(MethodImplOptions.Synchronized)> _
		Private Function updateDerivedGauge(ByVal scanGauge As Object, ByVal o As GaugeMonitorObservedObject) As Boolean

			Dim is_derived_gauge_valid As Boolean

			' The gauge difference mode is used.
			'
			If differenceMode Then

				' The previous scan gauge has been initialized.
				'
				If o.previousScanGauge IsNot Nothing Then
					derivedGaugeWithDifferencence(CType(scanGauge, Number), o)
					is_derived_gauge_valid = True
				' The previous scan gauge has not been initialized.
				' We cannot update the derived gauge...
				'
				Else
					is_derived_gauge_valid = False
				End If
				o.previousScanGauge = CType(scanGauge, Number)
			' The gauge difference mode is not used.
			'
			Else
				o.derivedGauge = CType(scanGauge, Number)
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
		Private Function updateNotifications(ByVal o As GaugeMonitorObservedObject) As MonitorNotification

			Dim n As MonitorNotification = Nothing

			' Send high notification if notifyHigh is true.
			' Send low notification if notifyLow is true.
			'
			If o.status Is RISING_OR_FALLING Then
				If isFirstGreaterThanLast(CType(o.derivedGauge, Number), highThreshold, o.type) Then
					If notifyHigh Then n = New MonitorNotification(THRESHOLD_HIGH_VALUE_EXCEEDED, Me, 0, 0, "", Nothing, Nothing, Nothing, highThreshold)
					o.status = FALLING
				ElseIf isFirstGreaterThanLast(lowThreshold, CType(o.derivedGauge, Number), o.type) Then
					If notifyLow Then n = New MonitorNotification(THRESHOLD_LOW_VALUE_EXCEEDED, Me, 0, 0, "", Nothing, Nothing, Nothing, lowThreshold)
					o.status = RISING
				End If
			Else
				If o.status Is RISING Then
					If isFirstGreaterThanLast(CType(o.derivedGauge, Number), highThreshold, o.type) Then
						If notifyHigh Then n = New MonitorNotification(THRESHOLD_HIGH_VALUE_EXCEEDED, Me, 0, 0, "", Nothing, Nothing, Nothing, highThreshold)
						o.status = FALLING
					End If
				ElseIf o.status Is FALLING Then
					If isFirstGreaterThanLast(lowThreshold, CType(o.derivedGauge, Number), o.type) Then
						If notifyLow Then n = New MonitorNotification(THRESHOLD_LOW_VALUE_EXCEEDED, Me, 0, 0, "", Nothing, Nothing, Nothing, lowThreshold)
						o.status = RISING
					End If
				End If
			End If

			Return n
		End Function

		''' <summary>
		''' Sets the derived gauge when the differenceMode flag is set to
		''' <CODE>true</CODE>.  Both integer and floating-point types are
		''' allowed.
		''' </summary>
		''' <param name="scanGauge"> The value of the observed attribute. </param>
		''' <param name="o"> The observed object. </param>
		<MethodImpl(MethodImplOptions.Synchronized)> _
		Private Sub setDerivedGaugeWithDifference(ByVal scanGauge As Number, ByVal o As GaugeMonitorObservedObject)
			Dim prev As Number = o.previousScanGauge
			Dim der As Number
			Select Case o.type
			Case INTEGER
				der = Convert.ToInt32(CInt(Fix(scanGauge)) - CInt(Fix(prev)))
			Case BYTE
				der = Convert.ToByte(CByte(CByte(scanGauge) - CByte(prev)))
			Case SHORT
				der = Convert.ToInt16(CShort(Fix(CShort(Fix(scanGauge)) - CShort(Fix(prev)))))
			Case LONG
				der = Convert.ToInt64(CLng(Fix(scanGauge)) - CLng(Fix(prev)))
			Case FLOAT
				der = Convert.ToSingle(CSng(scanGauge) - CSng(prev))
			Case DOUBLE
				der = Convert.ToDouble(CDbl(scanGauge) - CDbl(prev))
			Case Else
				' Should never occur...
				MONITOR_LOGGER.logp(java.util.logging.Level.FINEST, GetType(GaugeMonitor).name, "setDerivedGaugeWithDifference", "the threshold type is invalid")
				Return
			End Select
			o.derivedGauge = der
		End Sub

		''' <summary>
		''' Tests if the first specified Number is greater than or equal to
		''' the last.  Both integer and floating-point types are allowed.
		''' </summary>
		''' <param name="greater"> The first Number to compare with the second. </param>
		''' <param name="less"> The second Number to compare with the first. </param>
		''' <param name="type"> The number type. </param>
		''' <returns> <CODE>true</CODE> if the first specified Number is
		''' greater than or equal to the last, <CODE>false</CODE>
		''' otherwise. </returns>
		Private Function isFirstGreaterThanLast(ByVal greater As Number, ByVal less As Number, ByVal type As NumericalType) As Boolean

			Select Case type
			Case INTEGER, BYTE, SHORT, LONG
				Return (greater >= less)
			Case FLOAT, DOUBLE
				Return (greater >= less)
			Case Else
				' Should never occur...
				MONITOR_LOGGER.logp(java.util.logging.Level.FINEST, GetType(GaugeMonitor).name, "isFirstGreaterThanLast", "the threshold type is invalid")
				Return False
			End Select
		End Function

		''' <summary>
		''' Tests if the first specified Number is strictly greater than the last.
		''' Both integer and floating-point types are allowed.
		''' </summary>
		''' <param name="greater"> The first Number to compare with the second. </param>
		''' <param name="less"> The second Number to compare with the first. </param>
		''' <param name="className"> The number class name. </param>
		''' <returns> <CODE>true</CODE> if the first specified Number is
		''' strictly greater than the last, <CODE>false</CODE> otherwise. </returns>
		Private Function isFirstStrictlyGreaterThanLast(ByVal greater As Number, ByVal less As Number, ByVal className As String) As Boolean

			If className.Equals("java.lang.Integer") OrElse className.Equals("java.lang.Byte") OrElse className.Equals("java.lang.Short") OrElse className.Equals("java.lang.Long") Then

				Return (greater > less)
			ElseIf className.Equals("java.lang.Float") OrElse className.Equals("java.lang.Double") Then

				Return (greater > less)
			Else
				' Should never occur...
				MONITOR_LOGGER.logp(java.util.logging.Level.FINEST, GetType(GaugeMonitor).name, "isFirstStrictlyGreaterThanLast", "the threshold type is invalid")
				Return False
			End If
		End Function

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
			Dim gmo As New GaugeMonitorObservedObject([object])
			gmo.status = RISING_OR_FALLING
			gmo.previousScanGauge = Nothing
			Return gmo
		End Function

		''' <summary>
		''' This method globally sets the derived gauge type for the given
		''' "object" and "attribute" after checking that the type of the
		''' supplied observed attribute value is one of the value types
		''' supported by this monitor.
		''' </summary>
		<MethodImpl(MethodImplOptions.Synchronized)> _
		Friend Overrides Function isComparableTypeValid(Of T1)(ByVal [object] As javax.management.ObjectName, ByVal attribute As String, ByVal value As IComparable(Of T1)) As Boolean
			Dim o As GaugeMonitorObservedObject = CType(getObservedObject([object]), GaugeMonitorObservedObject)
			If o Is Nothing Then Return False

			' Check that the observed attribute is either of type
			' "Integer" or "Float".
			'
			If TypeOf value Is Integer? Then
				o.type = INTEGER
			ElseIf TypeOf value Is SByte? Then
				o.type = BYTE
			ElseIf TypeOf value Is Short? Then
				o.type = SHORT
			ElseIf TypeOf value Is Long? Then
				o.type = LONG
			ElseIf TypeOf value Is Single? Then
				o.type = FLOAT
			ElseIf TypeOf value Is Double? Then
				o.type = DOUBLE
			Else
				Return False
			End If
			Return True
		End Function

'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
		<MethodImpl(MethodImplOptions.Synchronized)> _
		Friend Overrides Function getDerivedGaugeFromComparable(Of T1)(ByVal [object] As javax.management.ObjectName, ByVal attribute As String, ByVal value As IComparable(Of T1)) As IComparable(Of ?)
			Dim o As GaugeMonitorObservedObject = CType(getObservedObject([object]), GaugeMonitorObservedObject)
			If o Is Nothing Then Return Nothing

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
			Dim o As GaugeMonitorObservedObject = CType(getObservedObject(___notification.observedObject), GaugeMonitorObservedObject)
			If o Is Nothing Then Return

			' Reset values.
			'
			o.status = RISING_OR_FALLING
			o.previousScanGauge = Nothing
		End Sub

		<MethodImpl(MethodImplOptions.Synchronized)> _
		Friend Overrides Function buildAlarmNotification(Of T1)(ByVal [object] As javax.management.ObjectName, ByVal attribute As String, ByVal value As IComparable(Of T1)) As MonitorNotification
			Dim o As GaugeMonitorObservedObject = CType(getObservedObject([object]), GaugeMonitorObservedObject)
			If o Is Nothing Then Return Nothing

			' Notify the listeners if the updated derived
			' gauge value is valid.
			'
			Dim alarm As MonitorNotification
			If o.derivedGaugeValid Then
				alarm = updateNotifications(o)
			Else
				alarm = Nothing
			End If
			Return alarm
		End Function

		''' <summary>
		''' Tests if the threshold high and threshold low are both of the
		''' same type as the gauge.  Both integer and floating-point types
		''' are allowed.
		''' 
		''' Note:
		'''   If the optional lowThreshold or highThreshold have not been
		'''   initialized, their default value is an Integer object with
		'''   a value equal to zero.
		''' </summary>
		''' <param name="object"> The observed object. </param>
		''' <param name="attribute"> The observed attribute. </param>
		''' <param name="value"> The sample value. </param>
		''' <returns> <CODE>true</CODE> if type is the same,
		''' <CODE>false</CODE> otherwise. </returns>
		<MethodImpl(MethodImplOptions.Synchronized)> _
		Friend Overrides Function isThresholdTypeValid(Of T1)(ByVal [object] As javax.management.ObjectName, ByVal attribute As String, ByVal value As IComparable(Of T1)) As Boolean
			Dim o As GaugeMonitorObservedObject = CType(getObservedObject([object]), GaugeMonitorObservedObject)
			If o Is Nothing Then Return False

			Dim c As Type = classForType(o.type)
			Return (isValidForType(highThreshold, c) AndAlso isValidForType(lowThreshold, c))
		End Function
	End Class

End Namespace