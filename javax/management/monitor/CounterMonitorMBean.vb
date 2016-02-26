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
	''' Exposes the remote management interface of the counter monitor MBean.
	''' 
	''' 
	''' @since 1.5
	''' </summary>
	Public Interface CounterMonitorMBean
		Inherits MonitorMBean

		' GETTERS AND SETTERS
		'--------------------

		''' <summary>
		''' Gets the derived gauge.
		''' </summary>
		''' <returns> The derived gauge. </returns>
		''' @deprecated As of JMX 1.2, replaced by <seealso cref="#getDerivedGauge(ObjectName)"/> 
		<Obsolete("As of JMX 1.2, replaced by <seealso cref="#getDerivedGauge(javax.management.ObjectName)"/>")> _
		ReadOnly Property derivedGauge As Number

		''' <summary>
		''' Gets the derived gauge timestamp.
		''' </summary>
		''' <returns> The derived gauge timestamp. </returns>
		''' @deprecated As of JMX 1.2, replaced by <seealso cref="#getDerivedGaugeTimeStamp(ObjectName)"/> 
		<Obsolete("As of JMX 1.2, replaced by <seealso cref="#getDerivedGaugeTimeStamp(javax.management.ObjectName)"/>")> _
		ReadOnly Property derivedGaugeTimeStamp As Long

		''' <summary>
		''' Gets the threshold value.
		''' </summary>
		''' <returns> The threshold value.
		''' </returns>
		''' <seealso cref= #setThreshold(Number)
		''' </seealso>
		''' @deprecated As of JMX 1.2, replaced by <seealso cref="#getThreshold(ObjectName)"/> 
		<Obsolete("As of JMX 1.2, replaced by <seealso cref="#getThreshold(javax.management.ObjectName)"/>")> _
		Property threshold As Number


		''' <summary>
		''' Gets the derived gauge for the specified MBean.
		''' </summary>
		''' <param name="object"> the MBean for which the derived gauge is to be returned </param>
		''' <returns> The derived gauge for the specified MBean if this MBean is in the
		'''         set of observed MBeans, or <code>null</code> otherwise.
		'''  </returns>
		Function getDerivedGauge(ByVal [object] As javax.management.ObjectName) As Number

		''' <summary>
		''' Gets the derived gauge timestamp for the specified MBean.
		''' </summary>
		''' <param name="object"> the MBean for which the derived gauge timestamp is to be returned </param>
		''' <returns> The derived gauge timestamp for the specified MBean if this MBean
		'''         is in the set of observed MBeans, or <code>null</code> otherwise.
		'''  </returns>
		Function getDerivedGaugeTimeStamp(ByVal [object] As javax.management.ObjectName) As Long

		''' <summary>
		''' Gets the threshold value for the specified MBean.
		''' </summary>
		''' <param name="object"> the MBean for which the threshold value is to be returned </param>
		''' <returns> The threshold value for the specified MBean if this MBean
		'''         is in the set of observed MBeans, or <code>null</code> otherwise.
		''' </returns>
		''' <seealso cref= #setThreshold
		'''  </seealso>
		Function getThreshold(ByVal [object] As javax.management.ObjectName) As Number

		''' <summary>
		''' Gets the initial threshold value common to all observed objects.
		''' </summary>
		''' <returns> The initial threshold value.
		''' </returns>
		''' <seealso cref= #setInitThreshold
		'''  </seealso>
		Property initThreshold As Number


		''' <summary>
		''' Gets the offset value.
		''' </summary>
		''' <seealso cref= #setOffset(Number)
		''' </seealso>
		''' <returns> The offset value. </returns>
		Property offset As Number


		''' <summary>
		''' Gets the modulus value.
		''' </summary>
		''' <returns> The modulus value.
		''' </returns>
		''' <seealso cref= #setModulus </seealso>
		Property modulus As Number


		''' <summary>
		''' Gets the notification's on/off switch value.
		''' </summary>
		''' <returns> <CODE>true</CODE> if the counter monitor notifies when
		''' exceeding the threshold, <CODE>false</CODE> otherwise.
		''' </returns>
		''' <seealso cref= #setNotify </seealso>
		Property notify As Boolean


		''' <summary>
		''' Gets the difference mode flag value.
		''' </summary>
		''' <returns> <CODE>true</CODE> if the difference mode is used,
		''' <CODE>false</CODE> otherwise.
		''' </returns>
		''' <seealso cref= #setDifferenceMode </seealso>
		Property differenceMode As Boolean

	End Interface

End Namespace