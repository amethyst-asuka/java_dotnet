'
' * Copyright (c) 1998, 2001, Oracle and/or its affiliates. All rights reserved.
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

Namespace org.omg.CORBA

	''' <summary>
	''' An object that represents an ORB service: its <code>service_detail_type</code>
	''' field contains the type of the ORB service, and its <code>service_detail</code>
	''' field contains a description of the ORB service.
	''' 
	''' 
	''' @author RIP Team
	''' </summary>
	Public NotInheritable Class ServiceDetail
		Implements org.omg.CORBA.portable.IDLEntity

		''' <summary>
		''' The type of the ORB service that this <code>ServiceDetail</code>
		''' object represents.
		''' </summary>
		Public service_detail_type As Integer

		''' <summary>
		''' The data describing the ORB service that this <code>ServiceDetail</code>
		''' object represents.
		''' </summary>
		Public service_detail As SByte()

		''' <summary>
		''' Constructs a <code>ServiceDetail</code> object with 0 for the type of
		''' ORB service and an empty description.
		''' </summary>
		Public Sub New()
		End Sub

		''' <summary>
		''' Constructs a <code>ServiceDetail</code> object with the given
		''' ORB service type and the given description.
		''' </summary>
		''' <param name="service_detail_type"> an <code>int</code> specifying the type of
		'''                            ORB service </param>
		''' <param name="service_detail"> a <code>byte</code> array describing the ORB service </param>
		Public Sub New(ByVal service_detail_type As Integer, ByVal service_detail As SByte())
			Me.service_detail_type = service_detail_type
			Me.service_detail = service_detail
		End Sub
	End Class

End Namespace