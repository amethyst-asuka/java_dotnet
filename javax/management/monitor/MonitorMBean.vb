Imports System

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
	''' Exposes the remote management interface of monitor MBeans.
	''' 
	''' 
	''' @since 1.5
	''' </summary>
	Public Interface MonitorMBean

		''' <summary>
		''' Starts the monitor.
		''' </summary>
		Sub start()

		''' <summary>
		''' Stops the monitor.
		''' </summary>
		Sub [stop]()

		' GETTERS AND SETTERS
		'--------------------

		''' <summary>
		''' Adds the specified object in the set of observed MBeans.
		''' </summary>
		''' <param name="object"> The object to observe. </param>
		''' <exception cref="java.lang.IllegalArgumentException"> the specified object is null.
		'''  </exception>
		Sub addObservedObject(ByVal [object] As javax.management.ObjectName)

		''' <summary>
		''' Removes the specified object from the set of observed MBeans.
		''' </summary>
		''' <param name="object"> The object to remove.
		'''  </param>
		Sub removeObservedObject(ByVal [object] As javax.management.ObjectName)

		''' <summary>
		''' Tests whether the specified object is in the set of observed MBeans.
		''' </summary>
		''' <param name="object"> The object to check. </param>
		''' <returns> <CODE>true</CODE> if the specified object is in the set, <CODE>false</CODE> otherwise.
		'''  </returns>
		Function containsObservedObject(ByVal [object] As javax.management.ObjectName) As Boolean

		''' <summary>
		''' Returns an array containing the objects being observed.
		''' </summary>
		''' <returns> The objects being observed.
		'''  </returns>
		ReadOnly Property observedObjects As javax.management.ObjectName()

		''' <summary>
		''' Gets the object name of the object being observed.
		''' </summary>
		''' <returns> The object being observed.
		''' </returns>
		''' <seealso cref= #setObservedObject
		''' </seealso>
		''' @deprecated As of JMX 1.2, replaced by <seealso cref="#getObservedObjects"/> 
		<Obsolete("As of JMX 1.2, replaced by <seealso cref="#getObservedObjects"/>")> _
		Property observedObject As javax.management.ObjectName


		''' <summary>
		''' Gets the attribute being observed.
		''' </summary>
		''' <returns> The attribute being observed.
		''' </returns>
		''' <seealso cref= #setObservedAttribute </seealso>
		Property observedAttribute As String


		''' <summary>
		''' Gets the granularity period (in milliseconds).
		''' </summary>
		''' <returns> The granularity period.
		''' </returns>
		''' <seealso cref= #setGranularityPeriod </seealso>
		Property granularityPeriod As Long


		''' <summary>
		''' Tests if the monitor MBean is active.
		''' A monitor MBean is marked active when the <seealso cref="#start start"/> method is called.
		''' It becomes inactive when the <seealso cref="#stop stop"/> method is called.
		''' </summary>
		''' <returns> <CODE>true</CODE> if the monitor MBean is active, <CODE>false</CODE> otherwise. </returns>
		ReadOnly Property active As Boolean
	End Interface

End Namespace