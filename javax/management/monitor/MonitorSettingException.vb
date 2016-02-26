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
	''' Exception thrown by the monitor when a monitor setting becomes invalid while the monitor is running.
	''' <P>
	''' As the monitor attributes may change at runtime, a check is performed before each observation.
	''' If a monitor attribute has become invalid, a monitor setting exception is thrown.
	''' 
	''' 
	''' @since 1.5
	''' </summary>
	Public Class MonitorSettingException
		Inherits javax.management.JMRuntimeException

		' Serial version 
		Private Const serialVersionUID As Long = -8807913418190202007L

		''' <summary>
		''' Default constructor.
		''' </summary>
		Public Sub New()
			MyBase.New()
		End Sub

		''' <summary>
		''' Constructor that allows an error message to be specified.
		''' </summary>
		''' <param name="message"> The specific error message. </param>
		Public Sub New(ByVal message As String)
			MyBase.New(message)
		End Sub
	End Class

End Namespace