Imports Microsoft.VisualBasic
Imports System
Imports System.Runtime.CompilerServices
Imports System.Collections.Generic
Imports System.Threading
import static com.sun.jmx.defaults.JmxProperties.MONITOR_LOGGER
Imports javax.management.monitor.MonitorNotification

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

Namespace javax.management.monitor

	''' <summary>
	''' Defines the part common to all monitor MBeans.
	''' A monitor MBean monitors values of an attribute common to a set of observed
	''' MBeans. The observed attribute is monitored at intervals specified by the
	''' granularity period. A gauge value (derived gauge) is derived from the values
	''' of the observed attribute.
	''' 
	''' 
	''' @since 1.5
	''' </summary>
	Public MustInherit Class Monitor
		Inherits javax.management.NotificationBroadcasterSupport
		Implements MonitorMBean, javax.management.MBeanRegistration

	'    
	'     * ------------------------------------------
	'     *  PACKAGE CLASSES
	'     * ------------------------------------------
	'     

		Friend Class ObservedObject

			Public Sub New(ByVal observedObject As javax.management.ObjectName)
				Me.observedObject = observedObject
			End Sub

			Public Property observedObject As javax.management.ObjectName
				Get
					Return observedObject
				End Get
			End Property
			<MethodImpl(MethodImplOptions.Synchronized)> _
			Public Property alreadyNotified As Integer
				Get
					Return alreadyNotified
				End Get
				Set(ByVal alreadyNotified As Integer)
					Me.alreadyNotified = alreadyNotified
				End Set
			End Property
			<MethodImpl(MethodImplOptions.Synchronized)> _
			Public Property derivedGauge As Object
				Get
					Return derivedGauge
				End Get
				Set(ByVal derivedGauge As Object)
					Me.derivedGauge = derivedGauge
				End Set
			End Property
			<MethodImpl(MethodImplOptions.Synchronized)> _
			Public Property derivedGaugeTimeStamp As Long
				Get
					Return derivedGaugeTimeStamp
				End Get
				Set(ByVal derivedGaugeTimeStamp As Long)
					Me.derivedGaugeTimeStamp = derivedGaugeTimeStamp
				End Set
			End Property

			Private ReadOnly observedObject As javax.management.ObjectName
			Private alreadyNotified As Integer
			Private derivedGauge As Object
			Private derivedGaugeTimeStamp As Long
		End Class

	'    
	'     * ------------------------------------------
	'     *  PRIVATE VARIABLES
	'     * ------------------------------------------
	'     

		''' <summary>
		''' Attribute to observe.
		''' </summary>
		Private observedAttribute As String

		''' <summary>
		''' Monitor granularity period (in milliseconds).
		''' The default value is set to 10 seconds.
		''' </summary>
		Private granularityPeriod As Long = 10000

		''' <summary>
		''' Monitor state.
		''' The default value is set to <CODE>false</CODE>.
		''' </summary>
		Private ___isActive As Boolean = False

		''' <summary>
		''' Monitor sequence number.
		''' The default value is set to 0.
		''' </summary>
		Private ReadOnly sequenceNumber As New java.util.concurrent.atomic.AtomicLong

		''' <summary>
		''' Complex type attribute flag.
		''' The default value is set to <CODE>false</CODE>.
		''' </summary>
		Private isComplexTypeAttribute As Boolean = False

		''' <summary>
		''' First attribute name extracted from complex type attribute name.
		''' </summary>
		Private firstAttribute As String

		''' <summary>
		''' Remaining attribute names extracted from complex type attribute name.
		''' </summary>
		Private ReadOnly remainingAttributes As IList(Of String) = New java.util.concurrent.CopyOnWriteArrayList(Of String)

		''' <summary>
		''' AccessControlContext of the Monitor.start() caller.
		''' </summary>
		Private Shared ReadOnly noPermissionsACC As New java.security.AccessControlContext(New java.security.ProtectionDomain() {New java.security.ProtectionDomain(Nothing, Nothing)})
'JAVA TO VB CONVERTER TODO TASK: There is no VB equivalent to 'volatile':
		Private acc As java.security.AccessControlContext = noPermissionsACC

		''' <summary>
		''' Scheduler Service.
		''' </summary>
		Private Shared ReadOnly scheduler As java.util.concurrent.ScheduledExecutorService = java.util.concurrent.Executors.newSingleThreadScheduledExecutor(New DaemonThreadFactory("Scheduler"))

		''' <summary>
		''' Map containing the thread pool executor per thread group.
		''' </summary>
		Private Shared ReadOnly executors As IDictionary(Of java.util.concurrent.ThreadPoolExecutor, Void) = New java.util.WeakHashMap(Of java.util.concurrent.ThreadPoolExecutor, Void)

		''' <summary>
		''' Lock for executors map.
		''' </summary>
		Private Shared ReadOnly executorsLock As New Object

		''' <summary>
		''' Maximum Pool Size
		''' </summary>
		Private Shared ReadOnly maximumPoolSize As Integer
		Shared Sub New()
			Const maximumPoolSizeSysProp As String = "jmx.x.monitor.maximum.pool.size"
			Dim maximumPoolSizeStr As String = java.security.AccessController.doPrivileged(New com.sun.jmx.mbeanserver.GetPropertyAction(maximumPoolSizeSysProp))
			If maximumPoolSizeStr Is Nothing OrElse maximumPoolSizeStr.Trim().Length = 0 Then
				maximumPoolSize = 10
			Else
				Dim maximumPoolSizeTmp As Integer = 10
				Try
					maximumPoolSizeTmp = Convert.ToInt32(maximumPoolSizeStr)
				Catch e As NumberFormatException
					If MONITOR_LOGGER.isLoggable(java.util.logging.Level.FINER) Then
						MONITOR_LOGGER.logp(java.util.logging.Level.FINER, GetType(Monitor).name, "<static initializer>", "Wrong value for " & maximumPoolSizeSysProp & " system property", e)
						MONITOR_LOGGER.logp(java.util.logging.Level.FINER, GetType(Monitor).name, "<static initializer>", maximumPoolSizeSysProp & " defaults to 10")
					End If
					maximumPoolSizeTmp = 10
				End Try
				If maximumPoolSizeTmp < 1 Then
					maximumPoolSize = 1
				Else
					maximumPoolSize = maximumPoolSizeTmp
				End If
			End If
		End Sub

		''' <summary>
		''' Future associated to the current monitor task.
		''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
		Private monitorFuture As java.util.concurrent.Future(Of ?)

		''' <summary>
		''' Scheduler task to be executed by the Scheduler Service.
		''' </summary>
		Private ReadOnly schedulerTask As New SchedulerTask(Me)

		''' <summary>
		''' ScheduledFuture associated to the current scheduler task.
		''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
		Private schedulerFuture As java.util.concurrent.ScheduledFuture(Of ?)

	'    
	'     * ------------------------------------------
	'     *  PROTECTED VARIABLES
	'     * ------------------------------------------
	'     

		''' <summary>
		''' The amount by which the capacity of the monitor arrays are
		''' automatically incremented when their size becomes greater than
		''' their capacity.
		''' </summary>
		Protected Friend Const capacityIncrement As Integer = 16

		''' <summary>
		''' The number of valid components in the vector of observed objects.
		''' 
		''' </summary>
		Protected Friend elementCount As Integer = 0

		''' <summary>
		''' Monitor errors that have already been notified. </summary>
		''' @deprecated equivalent to <seealso cref="#alreadyNotifieds"/>[0]. 
		<Obsolete("equivalent to <seealso cref="#alreadyNotifieds"/>[0].")> _
		Protected Friend alreadyNotified As Integer = 0

		''' <summary>
		''' <p>Selected monitor errors that have already been notified.</p>
		''' 
		''' <p>Each element in this array corresponds to an observed object
		''' in the vector.  It contains a bit mask of the flags {@link
		''' #OBSERVED_OBJECT_ERROR_NOTIFIED} etc, indicating whether the
		''' corresponding notification has already been sent for the MBean
		''' being monitored.</p>
		''' 
		''' </summary>
		Protected Friend alreadyNotifieds As Integer() = New Integer(capacityIncrement - 1){}

		''' <summary>
		''' Reference to the MBean server.  This reference is null when the
		''' monitor MBean is not registered in an MBean server.  This
		''' reference is initialized before the monitor MBean is registered
		''' in the MBean server. </summary>
		''' <seealso cref= #preRegister(MBeanServer server, ObjectName name) </seealso>
		Protected Friend server As javax.management.MBeanServer

		' Flags defining possible monitor errors.
		'

		''' <summary>
		''' This flag is used to reset the {@link #alreadyNotifieds
		''' alreadyNotifieds} monitor attribute.
		''' </summary>
		Protected Friend Const RESET_FLAGS_ALREADY_NOTIFIED As Integer = 0

		''' <summary>
		''' Flag denoting that a notification has occurred after changing
		''' the observed object.  This flag is used to check that the new
		''' observed object is registered in the MBean server at the time
		''' of the first notification.
		''' </summary>
		Protected Friend Const OBSERVED_OBJECT_ERROR_NOTIFIED As Integer = 1

		''' <summary>
		''' Flag denoting that a notification has occurred after changing
		''' the observed attribute.  This flag is used to check that the
		''' new observed attribute belongs to the observed object at the
		''' time of the first notification.
		''' </summary>
		Protected Friend Const OBSERVED_ATTRIBUTE_ERROR_NOTIFIED As Integer = 2

		''' <summary>
		''' Flag denoting that a notification has occurred after changing
		''' the observed object or the observed attribute.  This flag is
		''' used to check that the observed attribute type is correct
		''' (depending on the monitor in use) at the time of the first
		''' notification.
		''' </summary>
		Protected Friend Const OBSERVED_ATTRIBUTE_TYPE_ERROR_NOTIFIED As Integer = 4

		''' <summary>
		''' Flag denoting that a notification has occurred after changing
		''' the observed object or the observed attribute.  This flag is
		''' used to notify any exception (except the cases described above)
		''' when trying to get the value of the observed attribute at the
		''' time of the first notification.
		''' </summary>
		Protected Friend Const RUNTIME_ERROR_NOTIFIED As Integer = 8

		''' <summary>
		''' This field is retained for compatibility but should not be referenced.
		''' </summary>
		''' @deprecated No replacement. 
		<Obsolete("No replacement.")> _
		Protected Friend dbgTag As String = GetType(Monitor).name

	'    
	'     * ------------------------------------------
	'     *  PACKAGE VARIABLES
	'     * ------------------------------------------
	'     

		''' <summary>
		''' List of ObservedObjects to which the attribute to observe belongs.
		''' </summary>
		Friend ReadOnly observedObjects As IList(Of ObservedObject) = New java.util.concurrent.CopyOnWriteArrayList(Of ObservedObject)

		''' <summary>
		''' Flag denoting that a notification has occurred after changing
		''' the threshold. This flag is used to notify any exception
		''' related to invalid thresholds settings.
		''' </summary>
		Friend Const THRESHOLD_ERROR_NOTIFIED As Integer = 16

		''' <summary>
		''' Enumeration used to keep trace of the derived gauge type
		''' in counter and gauge monitors.
		''' </summary>
		Friend Enum NumericalType
			[BYTE]
			[SHORT]
			[INTEGER]
			[LONG]
			FLOAT
			[DOUBLE]
		End Enum

		''' <summary>
		''' Constant used to initialize all the numeric values.
		''' </summary>
		Friend Const INTEGER_ZERO As Integer? = 0


	'    
	'     * ------------------------------------------
	'     *  PUBLIC METHODS
	'     * ------------------------------------------
	'     

		''' <summary>
		''' Allows the monitor MBean to perform any operations it needs
		''' before being registered in the MBean server.
		''' <P>
		''' Initializes the reference to the MBean server.
		''' </summary>
		''' <param name="server"> The MBean server in which the monitor MBean will
		''' be registered. </param>
		''' <param name="name"> The object name of the monitor MBean.
		''' </param>
		''' <returns> The name of the monitor MBean registered.
		''' </returns>
		''' <exception cref="Exception"> </exception>
		Public Overridable Function preRegister(ByVal server As javax.management.MBeanServer, ByVal name As javax.management.ObjectName) As javax.management.ObjectName

			MONITOR_LOGGER.logp(java.util.logging.Level.FINER, GetType(Monitor).name, "preRegister(MBeanServer, ObjectName)", "initialize the reference on the MBean server")

			Me.server = server
			Return name
		End Function

		''' <summary>
		''' Allows the monitor MBean to perform any operations needed after
		''' having been registered in the MBean server or after the
		''' registration has failed.
		''' <P>
		''' Not used in this context.
		''' </summary>
		Public Overridable Sub postRegister(ByVal registrationDone As Boolean?)
		End Sub

		''' <summary>
		''' Allows the monitor MBean to perform any operations it needs
		''' before being unregistered by the MBean server.
		''' <P>
		''' Stops the monitor.
		''' </summary>
		''' <exception cref="Exception"> </exception>
		Public Overridable Sub preDeregister()

			MONITOR_LOGGER.logp(java.util.logging.Level.FINER, GetType(Monitor).name, "preDeregister()", "stop the monitor")

			' Stop the Monitor.
			'
			[stop]()
		End Sub

		''' <summary>
		''' Allows the monitor MBean to perform any operations needed after
		''' having been unregistered by the MBean server.
		''' <P>
		''' Not used in this context.
		''' </summary>
		Public Overridable Sub postDeregister()
		End Sub

		''' <summary>
		''' Starts the monitor.
		''' </summary>
		Public MustOverride Sub start() Implements MonitorMBean.start

		''' <summary>
		''' Stops the monitor.
		''' </summary>
		Public MustOverride Sub [stop]() Implements MonitorMBean.stop

		' GETTERS AND SETTERS
		'--------------------

		''' <summary>
		''' Returns the object name of the first object in the set of observed
		''' MBeans, or <code>null</code> if there is no such object.
		''' </summary>
		''' <returns> The object being observed.
		''' </returns>
		''' <seealso cref= #setObservedObject(ObjectName)
		''' </seealso>
		''' @deprecated As of JMX 1.2, replaced by <seealso cref="#getObservedObjects"/> 
		<Obsolete("As of JMX 1.2, replaced by <seealso cref="#getObservedObjects"/>"), MethodImpl(MethodImplOptions.Synchronized)> _
		Public Overridable Property observedObject As javax.management.ObjectName Implements MonitorMBean.getObservedObject
			Get
				If observedObjects.Count = 0 Then
					Return Nothing
				Else
					Return observedObjects(0).observedObject
				End If
			End Get
			Set(ByVal [object] As javax.management.ObjectName)
				If [object] Is Nothing Then Throw New System.ArgumentException("Null observed object")
				If observedObjects.Count = 1 AndAlso containsObservedObject([object]) Then Return
				observedObjects.Clear()
				addObservedObject([object])
			End Set
		End Property


		''' <summary>
		''' Adds the specified object in the set of observed MBeans, if this object
		''' is not already present.
		''' </summary>
		''' <param name="object"> The object to observe. </param>
		''' <exception cref="IllegalArgumentException"> The specified object is null.
		'''  </exception>
		<MethodImpl(MethodImplOptions.Synchronized)> _
		Public Overridable Sub addObservedObject(ByVal [object] As javax.management.ObjectName) Implements MonitorMBean.addObservedObject

			If [object] Is Nothing Then Throw New System.ArgumentException("Null observed object")

			' Check that the specified object is not already contained.
			'
			If containsObservedObject([object]) Then Return

			' Add the specified object in the list.
			'
			Dim o As ObservedObject = createObservedObject([object])
			o.alreadyNotified = RESET_FLAGS_ALREADY_NOTIFIED
			o.derivedGauge = INTEGER_ZERO
			o.derivedGaugeTimeStamp = System.currentTimeMillis()
			observedObjects.Add(o)

			' Update legacy protected stuff.
			'
			createAlreadyNotified()
		End Sub

		''' <summary>
		''' Removes the specified object from the set of observed MBeans.
		''' </summary>
		''' <param name="object"> The object to remove.
		'''  </param>
		<MethodImpl(MethodImplOptions.Synchronized)> _
		Public Overridable Sub removeObservedObject(ByVal [object] As javax.management.ObjectName) Implements MonitorMBean.removeObservedObject
			' Check for null object.
			'
			If [object] Is Nothing Then Return

			Dim o As ObservedObject = getObservedObject([object])
			If o IsNot Nothing Then
				' Remove the specified object from the list.
				'
				observedObjects.Remove(o)
				' Update legacy protected stuff.
				'
				createAlreadyNotified()
			End If
		End Sub

		''' <summary>
		''' Tests whether the specified object is in the set of observed MBeans.
		''' </summary>
		''' <param name="object"> The object to check. </param>
		''' <returns> <CODE>true</CODE> if the specified object is present,
		''' <CODE>false</CODE> otherwise.
		'''  </returns>
		<MethodImpl(MethodImplOptions.Synchronized)> _
		Public Overridable Function containsObservedObject(ByVal [object] As javax.management.ObjectName) As Boolean Implements MonitorMBean.containsObservedObject
			Return getObservedObject([object]) IsNot Nothing
		End Function

		''' <summary>
		''' Returns an array containing the objects being observed.
		''' </summary>
		''' <returns> The objects being observed.
		'''  </returns>
		<MethodImpl(MethodImplOptions.Synchronized)> _
		Public Overridable Property observedObjects As javax.management.ObjectName() Implements MonitorMBean.getObservedObjects
			Get
				Dim names As javax.management.ObjectName() = New javax.management.ObjectName(observedObjects.Count - 1){}
				For i As Integer = 0 To names.Length - 1
					names(i) = observedObjects(i).observedObject
				Next i
				Return names
			End Get
		End Property

		''' <summary>
		''' Gets the attribute being observed.
		''' <BR>The observed attribute is not initialized by default (set to null).
		''' </summary>
		''' <returns> The attribute being observed.
		''' </returns>
		''' <seealso cref= #setObservedAttribute </seealso>
		<MethodImpl(MethodImplOptions.Synchronized)> _
		Public Overridable Property observedAttribute As String Implements MonitorMBean.getObservedAttribute
			Get
				Return observedAttribute
			End Get
			Set(ByVal attribute As String)
    
				If attribute Is Nothing Then Throw New System.ArgumentException("Null observed attribute")
    
				' Update alreadyNotified array.
				'
				SyncLock Me
					If observedAttribute IsNot Nothing AndAlso observedAttribute.Equals(attribute) Then Return
					observedAttribute = attribute
    
					' Reset the complex type attribute information
					' such that it is recalculated again.
					'
					cleanupIsComplexTypeAttribute()
    
					Dim index As Integer = 0
					For Each o As ObservedObject In observedObjects
						resetAlreadyNotified(o, index, OBSERVED_ATTRIBUTE_ERROR_NOTIFIED Or OBSERVED_ATTRIBUTE_TYPE_ERROR_NOTIFIED)
						index += 1
					Next o
				End SyncLock
			End Set
		End Property


		''' <summary>
		''' Gets the granularity period (in milliseconds).
		''' <BR>The default value of the granularity period is 10 seconds.
		''' </summary>
		''' <returns> The granularity period value.
		''' </returns>
		''' <seealso cref= #setGranularityPeriod </seealso>
		<MethodImpl(MethodImplOptions.Synchronized)> _
		Public Overridable Property granularityPeriod As Long Implements MonitorMBean.getGranularityPeriod
			Get
				Return granularityPeriod
			End Get
			Set(ByVal period As Long)
    
				If period <= 0 Then Throw New System.ArgumentException("Nonpositive granularity " & "period")
    
				If granularityPeriod = period Then Return
				granularityPeriod = period
    
				' Reschedule the scheduler task if the monitor is active.
				'
				If active Then
					cleanupFutures()
					schedulerFuture = scheduler.schedule(schedulerTask, period, java.util.concurrent.TimeUnit.MILLISECONDS)
				End If
			End Set
		End Property


		''' <summary>
		''' Tests whether the monitor MBean is active.  A monitor MBean is
		''' marked active when the <seealso cref="#start start"/> method is called.
		''' It becomes inactive when the <seealso cref="#stop stop"/> method is
		''' called.
		''' </summary>
		''' <returns> <CODE>true</CODE> if the monitor MBean is active,
		''' <CODE>false</CODE> otherwise. </returns>
	'     This method must be synchronized so that the monitoring thread will
	'       correctly see modifications to the isActive variable. See the MonitorTask
	'       action executed by the Scheduled Executor Service. 
		<MethodImpl(MethodImplOptions.Synchronized)> _
		Public Overridable Property active As Boolean Implements MonitorMBean.isActive
			Get
				Return ___isActive
			End Get
		End Property

	'    
	'     * ------------------------------------------
	'     *  PACKAGE METHODS
	'     * ------------------------------------------
	'     

		''' <summary>
		''' Starts the monitor.
		''' </summary>
		Friend Overridable Sub doStart()
				MONITOR_LOGGER.logp(java.util.logging.Level.FINER, GetType(Monitor).name, "doStart()", "start the monitor")

			SyncLock Me
				If active Then
					MONITOR_LOGGER.logp(java.util.logging.Level.FINER, GetType(Monitor).name, "doStart()", "the monitor is already active")
					Return
				End If

				___isActive = True

				' Reset the complex type attribute information
				' such that it is recalculated again.
				'
				cleanupIsComplexTypeAttribute()

				' Cache the AccessControlContext of the Monitor.start() caller.
				' The monitor tasks will be executed within this context.
				'
				acc = java.security.AccessController.context

				' Start the scheduler.
				'
				cleanupFutures()
				schedulerTask.monitorTask = New MonitorTask(Me)
				schedulerFuture = scheduler.schedule(schedulerTask, granularityPeriod, java.util.concurrent.TimeUnit.MILLISECONDS)
			End SyncLock
		End Sub

		''' <summary>
		''' Stops the monitor.
		''' </summary>
		Friend Overridable Sub doStop()
			MONITOR_LOGGER.logp(java.util.logging.Level.FINER, GetType(Monitor).name, "doStop()", "stop the monitor")

			SyncLock Me
				If Not active Then
					MONITOR_LOGGER.logp(java.util.logging.Level.FINER, GetType(Monitor).name, "doStop()", "the monitor is not active")
					Return
				End If

				___isActive = False

				' Cancel the scheduler task associated with the
				' scheduler and its associated monitor task.
				'
				cleanupFutures()

				' Reset the AccessControlContext.
				'
				acc = noPermissionsACC

				' Reset the complex type attribute information
				' such that it is recalculated again.
				'
				cleanupIsComplexTypeAttribute()
			End SyncLock
		End Sub

		''' <summary>
		''' Gets the derived gauge of the specified object, if this object is
		''' contained in the set of observed MBeans, or <code>null</code> otherwise.
		''' </summary>
		''' <param name="object"> the name of the object whose derived gauge is to
		''' be returned.
		''' </param>
		''' <returns> The derived gauge of the specified object.
		''' 
		''' @since 1.6 </returns>
		<MethodImpl(MethodImplOptions.Synchronized)> _
		Friend Overridable Function getDerivedGauge(ByVal [object] As javax.management.ObjectName) As Object
			Dim o As ObservedObject = getObservedObject([object])
			Return If(o Is Nothing, Nothing, o.derivedGauge)
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
		Friend Overridable Function getDerivedGaugeTimeStamp(ByVal [object] As javax.management.ObjectName) As Long
			Dim o As ObservedObject = getObservedObject([object])
			Return If(o Is Nothing, 0, o.derivedGaugeTimeStamp)
		End Function

		Friend Overridable Function getAttribute(ByVal mbsc As javax.management.MBeanServerConnection, ByVal [object] As javax.management.ObjectName, ByVal attribute As String) As Object
			' Check for "ObservedAttribute" replacement.
			' This could happen if a thread A called setObservedAttribute()
			' while other thread B was in the middle of the monitor() method
			' and received the old observed attribute value.
			'
			Dim lookupMBeanInfo As Boolean
			SyncLock Me
				If Not active Then Throw New System.ArgumentException("The monitor has been stopped")
				If Not attribute.Equals(observedAttribute) Then Throw New System.ArgumentException("The observed attribute has been changed")
				lookupMBeanInfo = (firstAttribute Is Nothing AndAlso attribute.IndexOf("."c) <> -1)
			End SyncLock

			' Look up MBeanInfo if needed
			'
			Dim mbi As javax.management.MBeanInfo
			If lookupMBeanInfo Then
				Try
					mbi = mbsc.getMBeanInfo([object])
				Catch e As javax.management.IntrospectionException
					Throw New System.ArgumentException(e)
				End Try
			Else
				mbi = Nothing
			End If

			' Check for complex type attribute
			'
			Dim fa As String
			SyncLock Me
				If Not active Then Throw New System.ArgumentException("The monitor has been stopped")
				If Not attribute.Equals(observedAttribute) Then Throw New System.ArgumentException("The observed attribute has been changed")
				If firstAttribute Is Nothing Then
					If attribute.IndexOf("."c) <> -1 Then
						Dim mbaiArray As javax.management.MBeanAttributeInfo() = mbi.attributes
						For Each mbai As javax.management.MBeanAttributeInfo In mbaiArray
							If attribute.Equals(mbai.name) Then
								firstAttribute = attribute
								Exit For
							End If
						Next mbai
						If firstAttribute Is Nothing Then
							Dim tokens As String() = StringHelperClass.StringSplit(attribute, "\.", False)
							firstAttribute = tokens(0)
							For i As Integer = 1 To tokens.Length - 1
								remainingAttributes.Add(tokens(i))
							Next i
							isComplexTypeAttribute = True
						End If
					Else
						firstAttribute = attribute
					End If
				End If
				fa = firstAttribute
			End SyncLock
			Return mbsc.getAttribute([object], fa)
		End Function

'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
		Friend Overridable Function getComparableFromAttribute(ByVal [object] As javax.management.ObjectName, ByVal attribute As String, ByVal value As Object) As IComparable(Of ?)
			If isComplexTypeAttribute Then
				Dim v As Object = value
				For Each attr As String In remainingAttributes
					v = com.sun.jmx.mbeanserver.Introspector.elementFromComplex(v, attr)
				Next attr
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
				Return CType(v, IComparable(Of ?))
			Else
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
				Return CType(value, IComparable(Of ?))
			End If
		End Function

		Friend Overridable Function isComparableTypeValid(Of T1)(ByVal [object] As javax.management.ObjectName, ByVal attribute As String, ByVal value As IComparable(Of T1)) As Boolean
			Return True
		End Function

		Friend Overridable Function buildErrorNotification(Of T1)(ByVal [object] As javax.management.ObjectName, ByVal attribute As String, ByVal value As IComparable(Of T1)) As String
			Return Nothing
		End Function

		Friend Overridable Sub onErrorNotification(ByVal ___notification As MonitorNotification)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
		Friend Overridable Function getDerivedGaugeFromComparable(Of T1)(ByVal [object] As javax.management.ObjectName, ByVal attribute As String, ByVal value As IComparable(Of T1)) As IComparable(Of ?)
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
			Return CType(value, IComparable(Of ?))
		End Function

		Friend Overridable Function buildAlarmNotification(Of T1)(ByVal [object] As javax.management.ObjectName, ByVal attribute As String, ByVal value As IComparable(Of T1)) As MonitorNotification
			Return Nothing
		End Function

		Friend Overridable Function isThresholdTypeValid(Of T1)(ByVal [object] As javax.management.ObjectName, ByVal attribute As String, ByVal value As IComparable(Of T1)) As Boolean
			Return True
		End Function

		Friend Shared Function classForType(ByVal type As NumericalType) As Type
			Select Case type
				Case NumericalType.BYTE
					Return GetType(SByte?)
				Case NumericalType.SHORT
					Return GetType(Short)
				Case NumericalType.INTEGER
					Return GetType(Integer)
				Case NumericalType.LONG
					Return GetType(Long)
				Case NumericalType.FLOAT
					Return GetType(Single?)
				Case NumericalType.DOUBLE
					Return GetType(Double)
				Case Else
					Throw New System.ArgumentException("Unsupported numerical type")
			End Select
		End Function

		Friend Shared Function isValidForType(ByVal value As Object, ByVal c As Type) As Boolean
			Return ((value Is INTEGER_ZERO) OrElse c.IsInstanceOfType(value))
		End Function

		''' <summary>
		''' Get the specified {@code ObservedObject} if this object is
		''' contained in the set of observed MBeans, or {@code null}
		''' otherwise.
		''' </summary>
		''' <param name="object"> the name of the {@code ObservedObject} to retrieve.
		''' </param>
		''' <returns> The {@code ObservedObject} associated to the supplied
		''' {@code ObjectName}.
		''' 
		''' @since 1.6 </returns>
		<MethodImpl(MethodImplOptions.Synchronized)> _
		Friend Overridable Function getObservedObject(ByVal [object] As javax.management.ObjectName) As ObservedObject
			For Each o As ObservedObject In observedObjects
				If o.observedObject.Equals([object]) Then Return o
			Next o
			Return Nothing
		End Function

		''' <summary>
		''' Factory method for ObservedObject creation.
		''' 
		''' @since 1.6
		''' </summary>
		Friend Overridable Function createObservedObject(ByVal [object] As javax.management.ObjectName) As ObservedObject
			Return New ObservedObject([object])
		End Function

		''' <summary>
		''' Create the <seealso cref="#alreadyNotified"/> array from
		''' the {@code ObservedObject} array list.
		''' </summary>
		<MethodImpl(MethodImplOptions.Synchronized)> _
		Friend Overridable Sub createAlreadyNotified()
			' Update elementCount.
			'
			elementCount = observedObjects.Count

			' Update arrays.
			'
			alreadyNotifieds = New Integer(elementCount - 1){}
			For i As Integer = 0 To elementCount - 1
				alreadyNotifieds(i) = observedObjects(i).alreadyNotified
			Next i
			updateDeprecatedAlreadyNotified()
		End Sub

		''' <summary>
		''' Update the deprecated <seealso cref="#alreadyNotified"/> field.
		''' </summary>
		<MethodImpl(MethodImplOptions.Synchronized)> _
		Friend Overridable Sub updateDeprecatedAlreadyNotified()
			If elementCount > 0 Then
				alreadyNotified = alreadyNotifieds(0)
			Else
				alreadyNotified = 0
			End If
		End Sub

		''' <summary>
		''' Update the <seealso cref="#alreadyNotifieds"/> array element at the given index
		''' with the already notified flag in the given {@code ObservedObject}.
		''' Ensure the deprecated <seealso cref="#alreadyNotified"/> field is updated
		''' if appropriate.
		''' </summary>
		<MethodImpl(MethodImplOptions.Synchronized)> _
		Friend Overridable Sub updateAlreadyNotified(ByVal o As ObservedObject, ByVal index As Integer)
			alreadyNotifieds(index) = o.alreadyNotified
			If index = 0 Then updateDeprecatedAlreadyNotified()
		End Sub

		''' <summary>
		''' Check if the given bits in the given element of <seealso cref="#alreadyNotifieds"/>
		''' are set.
		''' </summary>
		<MethodImpl(MethodImplOptions.Synchronized)> _
		Friend Overridable Function isAlreadyNotified(ByVal o As ObservedObject, ByVal mask As Integer) As Boolean
			Return ((o.alreadyNotified And mask) <> 0)
		End Function

		''' <summary>
		''' Set the given bits in the given element of <seealso cref="#alreadyNotifieds"/>.
		''' Ensure the deprecated <seealso cref="#alreadyNotified"/> field is updated
		''' if appropriate.
		''' </summary>
		<MethodImpl(MethodImplOptions.Synchronized)> _
		Friend Overridable Sub setAlreadyNotified(ByVal o As ObservedObject, ByVal index As Integer, ByVal mask As Integer, ByVal an As Integer())
			Dim i As Integer = computeAlreadyNotifiedIndex(o, index, an)
			If i = -1 Then Return
			o.alreadyNotified = o.alreadyNotified Or mask
			updateAlreadyNotified(o, i)
		End Sub

		''' <summary>
		''' Reset the given bits in the given element of <seealso cref="#alreadyNotifieds"/>.
		''' Ensure the deprecated <seealso cref="#alreadyNotified"/> field is updated
		''' if appropriate.
		''' </summary>
		<MethodImpl(MethodImplOptions.Synchronized)> _
		Friend Overridable Sub resetAlreadyNotified(ByVal o As ObservedObject, ByVal index As Integer, ByVal mask As Integer)
			o.alreadyNotified = o.alreadyNotified And (Not mask)
			updateAlreadyNotified(o, index)
		End Sub

		''' <summary>
		''' Reset all bits in the given element of <seealso cref="#alreadyNotifieds"/>.
		''' Ensure the deprecated <seealso cref="#alreadyNotified"/> field is updated
		''' if appropriate.
		''' </summary>
		<MethodImpl(MethodImplOptions.Synchronized)> _
		Friend Overridable Sub resetAllAlreadyNotified(ByVal o As ObservedObject, ByVal index As Integer, ByVal an As Integer())
			Dim i As Integer = computeAlreadyNotifiedIndex(o, index, an)
			If i = -1 Then Return
			o.alreadyNotified = RESET_FLAGS_ALREADY_NOTIFIED
			updateAlreadyNotified(o, index)
		End Sub

		''' <summary>
		''' Check if the <seealso cref="#alreadyNotifieds"/> array has been modified.
		''' If true recompute the index for the given observed object.
		''' </summary>
		<MethodImpl(MethodImplOptions.Synchronized)> _
		Friend Overridable Function computeAlreadyNotifiedIndex(ByVal o As ObservedObject, ByVal index As Integer, ByVal an As Integer()) As Integer
			If an = alreadyNotifieds Then
				Return index
			Else
				Return observedObjects.IndexOf(o)
			End If
		End Function

	'    
	'     * ------------------------------------------
	'     *  PRIVATE METHODS
	'     * ------------------------------------------
	'     

		''' <summary>
		''' This method is used by the monitor MBean to create and send a
		''' monitor notification to all the listeners registered for this
		''' kind of notification.
		''' </summary>
		''' <param name="type"> The notification type. </param>
		''' <param name="timeStamp"> The notification emission date. </param>
		''' <param name="msg"> The notification message. </param>
		''' <param name="derGauge"> The derived gauge. </param>
		''' <param name="trigger"> The threshold/string (depending on the monitor
		''' type) that triggered off the notification. </param>
		''' <param name="object"> The ObjectName of the observed object that triggered
		''' off the notification. </param>
		''' <param name="onError"> Flag indicating if this monitor notification is
		''' an error notification or an alarm notification. </param>
		Private Sub sendNotification(ByVal type As String, ByVal timeStamp As Long, ByVal msg As String, ByVal derGauge As Object, ByVal trigger As Object, ByVal [object] As javax.management.ObjectName, ByVal onError As Boolean)
			If Not active Then Return

			If MONITOR_LOGGER.isLoggable(java.util.logging.Level.FINER) Then MONITOR_LOGGER.logp(java.util.logging.Level.FINER, GetType(Monitor).name, "sendNotification", "send notification: " & vbLf & vbTab & "Notification observed object = " & [object] & vbLf & vbTab & "Notification observed attribute = " & observedAttribute & vbLf & vbTab & "Notification derived gauge = " & derGauge)

			Dim seqno As Long = sequenceNumber.andIncrement

			Dim mn As New MonitorNotification(type, Me, seqno, timeStamp, msg, [object], observedAttribute, derGauge, trigger)
			If onError Then onErrorNotification(mn)
			sendNotification(mn)
		End Sub

		''' <summary>
		''' This method is called by the monitor each time
		''' the granularity period has been exceeded. </summary>
		''' <param name="o"> The observed object. </param>
		Private Sub monitor(ByVal o As ObservedObject, ByVal index As Integer, ByVal an As Integer())

			Dim ___attribute As String
			Dim notifType As String = Nothing
			Dim msg As String = Nothing
			Dim derGauge As Object = Nothing
			Dim trigger As Object = Nothing
			Dim [object] As javax.management.ObjectName
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
			Dim value As IComparable(Of ?) = Nothing
			Dim alarm As MonitorNotification = Nothing

			If Not active Then Return

			' Check that neither the observed object nor the
			' observed attribute are null.  If the observed
			' object or observed attribute is null, this means
			' that the monitor started before a complete
			' initialization and nothing is done.
			'
			SyncLock Me
				[object] = o.observedObject
				___attribute = observedAttribute
				If [object] Is Nothing OrElse ___attribute Is Nothing Then Return
			End SyncLock

			' Check that the observed object is registered in the
			' MBean server and that the observed attribute
			' belongs to the observed object.
			'
			Dim attributeValue As Object = Nothing
			Try
				attributeValue = getAttribute(server, [object], ___attribute)
				If attributeValue Is Nothing Then
					If isAlreadyNotified(o, OBSERVED_ATTRIBUTE_TYPE_ERROR_NOTIFIED) Then
						Return
					Else
						notifType = OBSERVED_ATTRIBUTE_TYPE_ERROR
						alreadyNotifiedied(o, index, OBSERVED_ATTRIBUTE_TYPE_ERROR_NOTIFIED, an)
						msg = "The observed attribute value is null."
						MONITOR_LOGGER.logp(java.util.logging.Level.FINEST, GetType(Monitor).name, "monitor", msg)
					End If
				End If
			Catch np_ex As NullPointerException
				If isAlreadyNotified(o, RUNTIME_ERROR_NOTIFIED) Then
					Return
				Else
					notifType = RUNTIME_ERROR
					alreadyNotifiedied(o, index, RUNTIME_ERROR_NOTIFIED, an)
					msg = "The monitor must be registered in the MBean " & "server or an MBeanServerConnection must be " & "explicitly supplied."
					MONITOR_LOGGER.logp(java.util.logging.Level.FINEST, GetType(Monitor).name, "monitor", msg)
					MONITOR_LOGGER.logp(java.util.logging.Level.FINEST, GetType(Monitor).name, "monitor", np_ex.ToString())
				End If
			Catch inf_ex As javax.management.InstanceNotFoundException
				If isAlreadyNotified(o, OBSERVED_OBJECT_ERROR_NOTIFIED) Then
					Return
				Else
					notifType = OBSERVED_OBJECT_ERROR
					alreadyNotifiedied(o, index, OBSERVED_OBJECT_ERROR_NOTIFIED, an)
					msg = "The observed object must be accessible in " & "the MBeanServerConnection."
					MONITOR_LOGGER.logp(java.util.logging.Level.FINEST, GetType(Monitor).name, "monitor", msg)
					MONITOR_LOGGER.logp(java.util.logging.Level.FINEST, GetType(Monitor).name, "monitor", inf_ex.ToString())
				End If
			Catch anf_ex As javax.management.AttributeNotFoundException
				If isAlreadyNotified(o, OBSERVED_ATTRIBUTE_ERROR_NOTIFIED) Then
					Return
				Else
					notifType = OBSERVED_ATTRIBUTE_ERROR
					alreadyNotifiedied(o, index, OBSERVED_ATTRIBUTE_ERROR_NOTIFIED, an)
					msg = "The observed attribute must be accessible in " & "the observed object."
					MONITOR_LOGGER.logp(java.util.logging.Level.FINEST, GetType(Monitor).name, "monitor", msg)
					MONITOR_LOGGER.logp(java.util.logging.Level.FINEST, GetType(Monitor).name, "monitor", anf_ex.ToString())
				End If
			Catch mb_ex As javax.management.MBeanException
				If isAlreadyNotified(o, RUNTIME_ERROR_NOTIFIED) Then
					Return
				Else
					notifType = RUNTIME_ERROR
					alreadyNotifiedied(o, index, RUNTIME_ERROR_NOTIFIED, an)
					msg = If(mb_ex.Message Is Nothing, "", mb_ex.Message)
					MONITOR_LOGGER.logp(java.util.logging.Level.FINEST, GetType(Monitor).name, "monitor", msg)
					MONITOR_LOGGER.logp(java.util.logging.Level.FINEST, GetType(Monitor).name, "monitor", mb_ex.ToString())
				End If
			Catch ref_ex As javax.management.ReflectionException
				If isAlreadyNotified(o, RUNTIME_ERROR_NOTIFIED) Then
					Return
				Else
					notifType = RUNTIME_ERROR
					alreadyNotifiedied(o, index, RUNTIME_ERROR_NOTIFIED, an)
					msg = If(ref_ex.Message Is Nothing, "", ref_ex.Message)
					MONITOR_LOGGER.logp(java.util.logging.Level.FINEST, GetType(Monitor).name, "monitor", msg)
					MONITOR_LOGGER.logp(java.util.logging.Level.FINEST, GetType(Monitor).name, "monitor", ref_ex.ToString())
				End If
			Catch io_ex As java.io.IOException
				If isAlreadyNotified(o, RUNTIME_ERROR_NOTIFIED) Then
					Return
				Else
					notifType = RUNTIME_ERROR
					alreadyNotifiedied(o, index, RUNTIME_ERROR_NOTIFIED, an)
					msg = If(io_ex.Message Is Nothing, "", io_ex.Message)
					MONITOR_LOGGER.logp(java.util.logging.Level.FINEST, GetType(Monitor).name, "monitor", msg)
					MONITOR_LOGGER.logp(java.util.logging.Level.FINEST, GetType(Monitor).name, "monitor", io_ex.ToString())
				End If
			Catch rt_ex As Exception
				If isAlreadyNotified(o, RUNTIME_ERROR_NOTIFIED) Then
					Return
				Else
					notifType = RUNTIME_ERROR
					alreadyNotifiedied(o, index, RUNTIME_ERROR_NOTIFIED, an)
					msg = If(rt_ex.Message Is Nothing, "", rt_ex.Message)
					MONITOR_LOGGER.logp(java.util.logging.Level.FINEST, GetType(Monitor).name, "monitor", msg)
					MONITOR_LOGGER.logp(java.util.logging.Level.FINEST, GetType(Monitor).name, "monitor", rt_ex.ToString())
				End If
			End Try

			SyncLock Me

				' Check if the monitor has been stopped.
				'
				If Not active Then Return

				' Check if the observed attribute has been changed.
				'
				' Avoid race condition where mbs.getAttribute() succeeded but
				' another thread replaced the observed attribute meanwhile.
				'
				' Avoid setting computed derived gauge on erroneous attribute.
				'
				If Not ___attribute.Equals(observedAttribute) Then Return

				' Derive a Comparable object from the ObservedAttribute value
				' if the type of the ObservedAttribute value is a complex type.
				'
				If msg Is Nothing Then
					Try
						value = getComparableFromAttribute([object], ___attribute, attributeValue)
					Catch e As ClassCastException
						If isAlreadyNotified(o, OBSERVED_ATTRIBUTE_TYPE_ERROR_NOTIFIED) Then
							Return
						Else
							notifType = OBSERVED_ATTRIBUTE_TYPE_ERROR
							alreadyNotifiedied(o, index, OBSERVED_ATTRIBUTE_TYPE_ERROR_NOTIFIED, an)
							msg = "The observed attribute value does not " & "implement the Comparable interface."
							MONITOR_LOGGER.logp(java.util.logging.Level.FINEST, GetType(Monitor).name, "monitor", msg)
							MONITOR_LOGGER.logp(java.util.logging.Level.FINEST, GetType(Monitor).name, "monitor", e.ToString())
						End If
					Catch e As javax.management.AttributeNotFoundException
						If isAlreadyNotified(o, OBSERVED_ATTRIBUTE_ERROR_NOTIFIED) Then
							Return
						Else
							notifType = OBSERVED_ATTRIBUTE_ERROR
							alreadyNotifiedied(o, index, OBSERVED_ATTRIBUTE_ERROR_NOTIFIED, an)
							msg = "The observed attribute must be accessible in " & "the observed object."
							MONITOR_LOGGER.logp(java.util.logging.Level.FINEST, GetType(Monitor).name, "monitor", msg)
							MONITOR_LOGGER.logp(java.util.logging.Level.FINEST, GetType(Monitor).name, "monitor", e.ToString())
						End If
					Catch e As Exception
						If isAlreadyNotified(o, RUNTIME_ERROR_NOTIFIED) Then
							Return
						Else
							notifType = RUNTIME_ERROR
							alreadyNotifiedied(o, index, RUNTIME_ERROR_NOTIFIED, an)
							msg = If(e.Message Is Nothing, "", e.Message)
							MONITOR_LOGGER.logp(java.util.logging.Level.FINEST, GetType(Monitor).name, "monitor", msg)
							MONITOR_LOGGER.logp(java.util.logging.Level.FINEST, GetType(Monitor).name, "monitor", e.ToString())
						End If
					End Try
				End If

				' Check that the observed attribute type is supported by this
				' monitor.
				'
				If msg Is Nothing Then
					If Not isComparableTypeValid([object], ___attribute, value) Then
						If isAlreadyNotified(o, OBSERVED_ATTRIBUTE_TYPE_ERROR_NOTIFIED) Then
							Return
						Else
							notifType = OBSERVED_ATTRIBUTE_TYPE_ERROR
							alreadyNotifiedied(o, index, OBSERVED_ATTRIBUTE_TYPE_ERROR_NOTIFIED, an)
							msg = "The observed attribute type is not valid."
							MONITOR_LOGGER.logp(java.util.logging.Level.FINEST, GetType(Monitor).name, "monitor", msg)
						End If
					End If
				End If

				' Check that threshold type is supported by this monitor.
				'
				If msg Is Nothing Then
					If Not isThresholdTypeValid([object], ___attribute, value) Then
						If isAlreadyNotified(o, THRESHOLD_ERROR_NOTIFIED) Then
							Return
						Else
							notifType = THRESHOLD_ERROR
							alreadyNotifiedied(o, index, THRESHOLD_ERROR_NOTIFIED, an)
							msg = "The threshold type is not valid."
							MONITOR_LOGGER.logp(java.util.logging.Level.FINEST, GetType(Monitor).name, "monitor", msg)
						End If
					End If
				End If

				' Let someone subclassing the monitor to perform additional
				' monitor consistency checks and report errors if necessary.
				'
				If msg Is Nothing Then
					msg = buildErrorNotification([object], ___attribute, value)
					If msg IsNot Nothing Then
						If isAlreadyNotified(o, RUNTIME_ERROR_NOTIFIED) Then
							Return
						Else
							notifType = RUNTIME_ERROR
							alreadyNotifiedied(o, index, RUNTIME_ERROR_NOTIFIED, an)
							MONITOR_LOGGER.logp(java.util.logging.Level.FINEST, GetType(Monitor).name, "monitor", msg)
						End If
					End If
				End If

				' If no errors were found then clear all error flags and
				' let the monitor decide if a notification must be sent.
				'
				If msg Is Nothing Then
					' Clear all already notified flags.
					'
					resetAllAlreadyNotified(o, index, an)

					' Get derived gauge from comparable value.
					'
					derGauge = getDerivedGaugeFromComparable([object], ___attribute, value)

					o.derivedGauge = derGauge
					o.derivedGaugeTimeStamp = System.currentTimeMillis()

					' Check if an alarm must be fired.
					'
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
					alarm = buildAlarmNotification([object], ___attribute, CType(derGauge, IComparable(Of ?)))
				End If

			End SyncLock

			' Notify monitor errors
			'
			If msg IsNot Nothing Then sendNotification(notifType, System.currentTimeMillis(), msg, derGauge, trigger, [object], True)

			' Notify monitor alarms
			'
			If alarm IsNot Nothing AndAlso alarm.type IsNot Nothing Then sendNotification(alarm.type, System.currentTimeMillis(), alarm.message, derGauge, alarm.trigger, [object], False)
		End Sub

		''' <summary>
		''' Cleanup the scheduler and monitor tasks futures.
		''' </summary>
		<MethodImpl(MethodImplOptions.Synchronized)> _
		Private Sub cleanupFutures()
			If schedulerFuture IsNot Nothing Then
				schedulerFuture.cancel(False)
				schedulerFuture = Nothing
			End If
			If monitorFuture IsNot Nothing Then
				monitorFuture.cancel(False)
				monitorFuture = Nothing
			End If
		End Sub

		''' <summary>
		''' Cleanup the "is complex type attribute" info.
		''' </summary>
		<MethodImpl(MethodImplOptions.Synchronized)> _
		Private Sub cleanupIsComplexTypeAttribute()
			firstAttribute = Nothing
			remainingAttributes.Clear()
			isComplexTypeAttribute = False
		End Sub

		''' <summary>
		''' SchedulerTask nested class: This class implements the Runnable interface.
		''' 
		''' The SchedulerTask is executed periodically with a given fixed delay by
		''' the Scheduled Executor Service.
		''' </summary>
		Private Class SchedulerTask
			Implements Runnable

			Private ReadOnly outerInstance As Monitor


			Private task As MonitorTask

	'        
	'         * ------------------------------------------
	'         *  CONSTRUCTORS
	'         * ------------------------------------------
	'         

			Public Sub New(ByVal outerInstance As Monitor)
					Me.outerInstance = outerInstance
			End Sub

	'        
	'         * ------------------------------------------
	'         *  GETTERS/SETTERS
	'         * ------------------------------------------
	'         

			Public Overridable Property monitorTask As MonitorTask
				Set(ByVal task As MonitorTask)
					Me.task = task
				End Set
			End Property

	'        
	'         * ------------------------------------------
	'         *  PUBLIC METHODS
	'         * ------------------------------------------
	'         

			Public Overridable Sub run()
				SyncLock Monitor.this
					outerInstance.monitorFuture = task.submit()
				End SyncLock
			End Sub
		End Class

		''' <summary>
		''' MonitorTask nested class: This class implements the Runnable interface.
		''' 
		''' The MonitorTask is executed periodically with a given fixed delay by the
		''' Scheduled Executor Service.
		''' </summary>
		Private Class MonitorTask
			Implements Runnable

			Private ReadOnly outerInstance As Monitor


			Private executor As java.util.concurrent.ThreadPoolExecutor

	'        
	'         * ------------------------------------------
	'         *  CONSTRUCTORS
	'         * ------------------------------------------
	'         

			Public Sub New(ByVal outerInstance As Monitor)
					Me.outerInstance = outerInstance
				' Find out if there's already an existing executor for the calling
				' thread and reuse it. Otherwise, create a new one and store it in
				' the executors map. If there is a SecurityManager, the group of
				' System.getSecurityManager() is used, else the group of the thread
				' instantiating this MonitorTask, i.e. the group of the thread that
				' calls "Monitor.start()".
				Dim s As SecurityManager = System.securityManager
				Dim group As ThreadGroup = If(s IsNot Nothing, s.threadGroup, Thread.CurrentThread.threadGroup)
				SyncLock executorsLock
					For Each e As java.util.concurrent.ThreadPoolExecutor In executors.Keys
						Dim tf As DaemonThreadFactory = CType(e.threadFactory, DaemonThreadFactory)
						Dim tg As ThreadGroup = tf.threadGroup
						If tg Is group Then
							executor = e
							Exit For
						End If
					Next e
					If executor Is Nothing Then
						executor = New java.util.concurrent.ThreadPoolExecutor(maximumPoolSize, maximumPoolSize, 60L, java.util.concurrent.TimeUnit.SECONDS, New java.util.concurrent.LinkedBlockingQueue(Of Runnable), New DaemonThreadFactory("ThreadGroup<" & group.name & "> Executor", group))
						executor.allowCoreThreadTimeOut(True)
						executors(executor) = Nothing
					End If
				End SyncLock
			End Sub

	'        
	'         * ------------------------------------------
	'         *  PUBLIC METHODS
	'         * ------------------------------------------
	'         

'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
			Public Overridable Function submit() As java.util.concurrent.Future(Of ?)
				Return executor.submit(Me)
			End Function

			Public Overridable Sub run()
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
				Dim sf As java.util.concurrent.ScheduledFuture(Of ?)
				Dim ac As java.security.AccessControlContext
				SyncLock Monitor.this
					sf = outerInstance.schedulerFuture
					ac = outerInstance.acc
				End SyncLock
'JAVA TO VB CONVERTER TODO TASK: Anonymous inner classes are not converted to VB if the base type is not defined in the code being converted:
'				java.security.PrivilegedAction<Void> action = New java.security.PrivilegedAction<Void>()
	'			{
	'				public Void run()
	'				{
	'					if (outerInstance.isActive())
	'					{
	'						final int an[] = alreadyNotifieds;
	'						int index = 0;
	'						for (ObservedObject o : outerInstance.observedObjects)
	'						{
	'							if (outerInstance.isActive())
	'							{
	'								outerInstance.monitor(o, index, an);
	'								index += 1;
	'							}
	'						}
	'					}
	'					Return Nothing;
	'				}
	'			};
				If ac Is Nothing Then Throw New SecurityException("AccessControlContext cannot be null")
				java.security.AccessController.doPrivileged(action, ac)
				SyncLock Monitor.this
					If outerInstance.active AndAlso outerInstance.schedulerFuture Is sf Then
						outerInstance.monitorFuture = Nothing
						outerInstance.schedulerFuture = scheduler.schedule(outerInstance.schedulerTask, outerInstance.granularityPeriod, java.util.concurrent.TimeUnit.MILLISECONDS)
					End If
				End SyncLock
			End Sub
		End Class

		''' <summary>
		''' Daemon thread factory used by the monitor executors.
		''' <P>
		''' This factory creates all new threads used by an Executor in
		''' the same ThreadGroup. If there is a SecurityManager, it uses
		''' the group of System.getSecurityManager(), else the group of
		''' the thread instantiating this DaemonThreadFactory. Each new
		''' thread is created as a daemon thread with priority
		''' Thread.NORM_PRIORITY. New threads have names accessible via
		''' Thread.getName() of "{@literal JMX Monitor <pool-name> Pool [Thread-M]}",
		''' where M is the sequence number of the thread created by this
		''' factory.
		''' </summary>
		Private Class DaemonThreadFactory
			Implements java.util.concurrent.ThreadFactory

			Friend ReadOnly group As ThreadGroup
			Friend ReadOnly threadNumber As New java.util.concurrent.atomic.AtomicInteger(1)
			Friend ReadOnly namePrefix As String
			Friend Const nameSuffix As String = "]"

			Public Sub New(ByVal poolName As String)
				Dim s As SecurityManager = System.securityManager
				group = If(s IsNot Nothing, s.threadGroup, Thread.CurrentThread.threadGroup)
				namePrefix = "JMX Monitor " & poolName & " Pool [Thread-"
			End Sub

			Public Sub New(ByVal poolName As String, ByVal threadGroup As ThreadGroup)
				group = threadGroup
				namePrefix = "JMX Monitor " & poolName & " Pool [Thread-"
			End Sub

			Public Overridable Property threadGroup As ThreadGroup
				Get
					Return group
				End Get
			End Property

			Public Overridable Function newThread(ByVal r As Runnable) As Thread
				Dim t As New Thread(group, r, namePrefix + threadNumber.andIncrement + nameSuffix, 0)
				t.daemon = True
				If t.priority <> Thread.NORM_PRIORITY Then t.priority = Thread.NORM_PRIORITY
				Return t
			End Function
		End Class
	End Class

End Namespace