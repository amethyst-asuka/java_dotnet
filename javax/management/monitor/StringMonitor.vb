Imports System
Imports System.Runtime.CompilerServices
import static com.sun.jmx.defaults.JmxProperties.MONITOR_LOGGER
Imports javax.management.monitor.MonitorNotification

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

	''' <summary>
	''' Defines a monitor MBean designed to observe the values of a string
	''' attribute.
	''' <P>
	''' A string monitor sends notifications as follows:
	''' <UL>
	''' <LI> if the attribute value matches the string to compare value,
	'''      a {@link MonitorNotification#STRING_TO_COMPARE_VALUE_MATCHED
	'''      match notification} is sent.
	'''      The notify match flag must be set to <CODE>true</CODE>.
	'''      <BR>Subsequent matchings of the string to compare values do not
	'''      cause further notifications unless
	'''      the attribute value differs from the string to compare value.
	''' <LI> if the attribute value differs from the string to compare value,
	'''      a {@link MonitorNotification#STRING_TO_COMPARE_VALUE_DIFFERED
	'''      differ notification} is sent.
	'''      The notify differ flag must be set to <CODE>true</CODE>.
	'''      <BR>Subsequent differences from the string to compare value do
	'''      not cause further notifications unless
	'''      the attribute value matches the string to compare value.
	''' </UL>
	''' 
	''' 
	''' @since 1.5
	''' </summary>
	Public Class StringMonitor
		Inherits Monitor
		Implements StringMonitorMBean

	'    
	'     * ------------------------------------------
	'     *  PACKAGE CLASSES
	'     * ------------------------------------------
	'     

		Friend Class StringMonitorObservedObject
			Inherits ObservedObject

			Public Sub New(ByVal observedObject As javax.management.ObjectName)
				MyBase.New(observedObject)
			End Sub

			<MethodImpl(MethodImplOptions.Synchronized)> _
			Public Property status As Integer
				Get
					Return status
				End Get
				Set(ByVal status As Integer)
					Me.status = status
				End Set
			End Property

			Private status As Integer
		End Class

	'    
	'     * ------------------------------------------
	'     *  PRIVATE VARIABLES
	'     * ------------------------------------------
	'     

		''' <summary>
		''' String to compare with the observed attribute.
		''' <BR>The default value is an empty character sequence.
		''' </summary>
		Private stringToCompare As String = ""

		''' <summary>
		''' Flag indicating if the string monitor notifies when matching
		''' the string to compare.
		''' <BR>The default value is set to <CODE>false</CODE>.
		''' </summary>
		Private notifyMatch As Boolean = False

		''' <summary>
		''' Flag indicating if the string monitor notifies when differing
		''' from the string to compare.
		''' <BR>The default value is set to <CODE>false</CODE>.
		''' </summary>
		Private notifyDiffer As Boolean = False

		Private Shared ReadOnly types As String() = { RUNTIME_ERROR, OBSERVED_OBJECT_ERROR, OBSERVED_ATTRIBUTE_ERROR, OBSERVED_ATTRIBUTE_TYPE_ERROR, STRING_TO_COMPARE_VALUE_MATCHED, STRING_TO_COMPARE_VALUE_DIFFERED }

		Private Shared ReadOnly notifsInfo As javax.management.MBeanNotificationInfo() = { New javax.management.MBeanNotificationInfo(types, "javax.management.monitor.MonitorNotification", "Notifications sent by the StringMonitor MBean") }

		' Flags needed to implement the matching/differing mechanism.
		'
		Private Const MATCHING As Integer = 0
		Private Const DIFFERING As Integer = 1
		Private Const MATCHING_OR_DIFFERING As Integer = 2

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
		''' Starts the string monitor.
		''' </summary>
		<MethodImpl(MethodImplOptions.Synchronized)> _
		Public Overrides Sub start() Implements MonitorMBean.start
			If active Then
				MONITOR_LOGGER.logp(java.util.logging.Level.FINER, GetType(StringMonitor).name, "start", "the monitor is already active")
				Return
			End If
			' Reset values.
			'
			For Each o As ObservedObject In observedObjects
				Dim smo As StringMonitorObservedObject = CType(o, StringMonitorObservedObject)
				smo.status = MATCHING_OR_DIFFERING
			Next o
			doStart()
		End Sub

		''' <summary>
		''' Stops the string monitor.
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
		''' <param name="object"> the name of the MBean whose derived gauge is required.
		''' </param>
		''' <returns> The derived gauge of the specified object.
		'''  </returns>
		<MethodImpl(MethodImplOptions.Synchronized)> _
		Public Overrides Function getDerivedGauge(ByVal [object] As javax.management.ObjectName) As String Implements StringMonitorMBean.getDerivedGauge
			Return CStr(MyBase.getDerivedGauge([object]))
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
		Public Overrides Function getDerivedGaugeTimeStamp(ByVal [object] As javax.management.ObjectName) As Long Implements StringMonitorMBean.getDerivedGaugeTimeStamp
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
		Public Overridable Property derivedGauge As String Implements StringMonitorMBean.getDerivedGauge
			Get
				If observedObjects.Count = 0 Then
					Return Nothing
				Else
					Return CStr(observedObjects(0).derivedGauge)
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
		Public Overridable Property derivedGaugeTimeStamp As Long Implements StringMonitorMBean.getDerivedGaugeTimeStamp
			Get
				If observedObjects.Count = 0 Then
					Return 0
				Else
					Return observedObjects(0).derivedGaugeTimeStamp
				End If
			End Get
		End Property

		''' <summary>
		''' Gets the string to compare with the observed attribute common
		''' to all observed MBeans.
		''' </summary>
		''' <returns> The string value.
		''' </returns>
		''' <seealso cref= #setStringToCompare </seealso>
		<MethodImpl(MethodImplOptions.Synchronized)> _
		Public Overridable Property stringToCompare As String Implements StringMonitorMBean.getStringToCompare
			Get
				Return stringToCompare
			End Get
			Set(ByVal value As String)
    
				If value Is Nothing Then Throw New System.ArgumentException("Null string to compare")
    
				If stringToCompare.Equals(value) Then Return
				stringToCompare = value
    
				' Reset values.
				'
				For Each o As ObservedObject In observedObjects
					Dim smo As StringMonitorObservedObject = CType(o, StringMonitorObservedObject)
					smo.status = MATCHING_OR_DIFFERING
				Next o
			End Set
		End Property


		''' <summary>
		''' Gets the matching notification's on/off switch value common to
		''' all observed MBeans.
		''' </summary>
		''' <returns> <CODE>true</CODE> if the string monitor notifies when
		''' matching the string to compare, <CODE>false</CODE> otherwise.
		''' </returns>
		''' <seealso cref= #setNotifyMatch </seealso>
		<MethodImpl(MethodImplOptions.Synchronized)> _
		Public Overridable Property notifyMatch As Boolean Implements StringMonitorMBean.getNotifyMatch
			Get
				Return notifyMatch
			End Get
			Set(ByVal value As Boolean)
				If notifyMatch = value Then Return
				notifyMatch = value
			End Set
		End Property


		''' <summary>
		''' Gets the differing notification's on/off switch value common to
		''' all observed MBeans.
		''' </summary>
		''' <returns> <CODE>true</CODE> if the string monitor notifies when
		''' differing from the string to compare, <CODE>false</CODE> otherwise.
		''' </returns>
		''' <seealso cref= #setNotifyDiffer </seealso>
		<MethodImpl(MethodImplOptions.Synchronized)> _
		Public Overridable Property notifyDiffer As Boolean Implements StringMonitorMBean.getNotifyDiffer
			Get
				Return notifyDiffer
			End Get
			Set(ByVal value As Boolean)
				If notifyDiffer = value Then Return
				notifyDiffer = value
			End Set
		End Property


		''' <summary>
		''' Returns a <CODE>NotificationInfo</CODE> object containing the name of
		''' the Java class of the notification and the notification types sent by
		''' the string monitor.
		''' </summary>
		Public Property Overrides notificationInfo As javax.management.MBeanNotificationInfo()
			Get
				Return notifsInfo.clone()
			End Get
		End Property

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
			Dim smo As New StringMonitorObservedObject([object])
			smo.status = MATCHING_OR_DIFFERING
			Return smo
		End Function

		''' <summary>
		''' Check that the type of the supplied observed attribute
		''' value is one of the value types supported by this monitor.
		''' </summary>
		<MethodImpl(MethodImplOptions.Synchronized)> _
		Friend Overrides Function isComparableTypeValid(Of T1)(ByVal [object] As javax.management.ObjectName, ByVal attribute As String, ByVal value As IComparable(Of T1)) As Boolean
			' Check that the observed attribute is of type "String".
			'
			If TypeOf value Is String Then Return True
			Return False
		End Function

		<MethodImpl(MethodImplOptions.Synchronized)> _
		Friend Overrides Sub onErrorNotification(ByVal ___notification As MonitorNotification)
			Dim o As StringMonitorObservedObject = CType(getObservedObject(___notification.observedObject), StringMonitorObservedObject)
			If o Is Nothing Then Return

			' Reset values.
			'
			o.status = MATCHING_OR_DIFFERING
		End Sub

		<MethodImpl(MethodImplOptions.Synchronized)> _
		Friend Overrides Function buildAlarmNotification(Of T1)(ByVal [object] As javax.management.ObjectName, ByVal attribute As String, ByVal value As IComparable(Of T1)) As MonitorNotification
			Dim type As String = Nothing
			Dim msg As String = Nothing
			Dim trigger As Object = Nothing

			Dim o As StringMonitorObservedObject = CType(getObservedObject([object]), StringMonitorObservedObject)
			If o Is Nothing Then Return Nothing

			' Send matching notification if notifyMatch is true.
			' Send differing notification if notifyDiffer is true.
			'
			If o.status Is MATCHING_OR_DIFFERING Then
				If o.derivedGauge.Equals(stringToCompare) Then
					If notifyMatch Then
						type = STRING_TO_COMPARE_VALUE_MATCHED
						msg = ""
						trigger = stringToCompare
					End If
					o.status = DIFFERING
				Else
					If notifyDiffer Then
						type = STRING_TO_COMPARE_VALUE_DIFFERED
						msg = ""
						trigger = stringToCompare
					End If
					o.status = MATCHING
				End If
			Else
				If o.status Is MATCHING Then
					If o.derivedGauge.Equals(stringToCompare) Then
						If notifyMatch Then
							type = STRING_TO_COMPARE_VALUE_MATCHED
							msg = ""
							trigger = stringToCompare
						End If
						o.status = DIFFERING
					End If
				ElseIf o.status Is DIFFERING Then
					If Not o.derivedGauge.Equals(stringToCompare) Then
						If notifyDiffer Then
							type = STRING_TO_COMPARE_VALUE_DIFFERED
							msg = ""
							trigger = stringToCompare
						End If
						o.status = MATCHING
					End If
				End If
			End If

			Return New MonitorNotification(type, Me, 0, 0, msg, Nothing, Nothing, Nothing, trigger)
		End Function
	End Class

End Namespace