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
	''' Exposes the remote management interface of the gauge monitor MBean.
	''' 
	''' 
	''' @since 1.5
	''' </summary>
	Public Interface GaugeMonitorMBean
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
		''' Gets the high threshold value.
		''' </summary>
		''' <returns> The high threshold value. </returns>
		ReadOnly Property highThreshold As Number

		''' <summary>
		''' Gets the low threshold value.
		''' </summary>
		''' <returns> The low threshold value. </returns>
		ReadOnly Property lowThreshold As Number

		''' <summary>
		''' Sets the high and the low threshold values.
		''' </summary>
		''' <param name="highValue"> The high threshold value. </param>
		''' <param name="lowValue"> The low threshold value. </param>
		''' <exception cref="java.lang.IllegalArgumentException"> The specified high/low threshold is null
		''' or the low threshold is greater than the high threshold
		''' or the high threshold and the low threshold are not of the same type. </exception>
		Sub setThresholds(ByVal highValue As Number, ByVal lowValue As Number)

		''' <summary>
		''' Gets the high notification's on/off switch value.
		''' </summary>
		''' <returns> <CODE>true</CODE> if the gauge monitor notifies when
		''' exceeding the high threshold, <CODE>false</CODE> otherwise.
		''' </returns>
		''' <seealso cref= #setNotifyHigh </seealso>
		Property notifyHigh As Boolean


		''' <summary>
		''' Gets the low notification's on/off switch value.
		''' </summary>
		''' <returns> <CODE>true</CODE> if the gauge monitor notifies when
		''' exceeding the low threshold, <CODE>false</CODE> otherwise.
		''' </returns>
		''' <seealso cref= #setNotifyLow </seealso>
		Property notifyLow As Boolean


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