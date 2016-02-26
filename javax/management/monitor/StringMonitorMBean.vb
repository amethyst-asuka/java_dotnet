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
	''' Exposes the remote management interface of the string monitor MBean.
	''' 
	''' 
	''' @since 1.5
	''' </summary>
	Public Interface StringMonitorMBean
		Inherits MonitorMBean

		' GETTERS AND SETTERS
		'--------------------

		''' <summary>
		''' Gets the derived gauge.
		''' </summary>
		''' <returns> The derived gauge. </returns>
		''' @deprecated As of JMX 1.2, replaced by <seealso cref="#getDerivedGauge(ObjectName)"/> 
		<Obsolete("As of JMX 1.2, replaced by <seealso cref="#getDerivedGauge(javax.management.ObjectName)"/>")> _
		ReadOnly Property derivedGauge As String

		''' <summary>
		''' Gets the derived gauge timestamp.
		''' </summary>
		''' <returns> The derived gauge timestamp. </returns>
		''' @deprecated As of JMX 1.2, replaced by <seealso cref="#getDerivedGaugeTimeStamp(ObjectName)"/> 
		<Obsolete("As of JMX 1.2, replaced by <seealso cref="#getDerivedGaugeTimeStamp(javax.management.ObjectName)"/>")> _
		ReadOnly Property derivedGaugeTimeStamp As Long

		''' <summary>
		''' Gets the derived gauge for the specified MBean.
		''' </summary>
		''' <param name="object"> the MBean for which the derived gauge is to be returned </param>
		''' <returns> The derived gauge for the specified MBean if this MBean is in the
		'''         set of observed MBeans, or <code>null</code> otherwise.
		'''  </returns>
		Function getDerivedGauge(ByVal [object] As javax.management.ObjectName) As String

		''' <summary>
		''' Gets the derived gauge timestamp for the specified MBean.
		''' </summary>
		''' <param name="object"> the MBean for which the derived gauge timestamp is to be returned </param>
		''' <returns> The derived gauge timestamp for the specified MBean if this MBean
		'''         is in the set of observed MBeans, or <code>null</code> otherwise.
		'''  </returns>
		Function getDerivedGaugeTimeStamp(ByVal [object] As javax.management.ObjectName) As Long

		''' <summary>
		''' Gets the string to compare with the observed attribute.
		''' </summary>
		''' <returns> The string value.
		''' </returns>
		''' <seealso cref= #setStringToCompare </seealso>
		Property stringToCompare As String


		''' <summary>
		''' Gets the matching notification's on/off switch value.
		''' </summary>
		''' <returns> <CODE>true</CODE> if the string monitor notifies when
		''' matching, <CODE>false</CODE> otherwise.
		''' </returns>
		''' <seealso cref= #setNotifyMatch </seealso>
		Property notifyMatch As Boolean


		''' <summary>
		''' Gets the differing notification's on/off switch value.
		''' </summary>
		''' <returns> <CODE>true</CODE> if the string monitor notifies when
		''' differing, <CODE>false</CODE> otherwise.
		''' </returns>
		''' <seealso cref= #setNotifyDiffer </seealso>
		Property notifyDiffer As Boolean

	End Interface

End Namespace