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
	''' An IDL struct in the CORBA module that
	'''  stores information about a CORBA service available in the
	'''  ORB implementation and is obtained from the <tt>ORB.get_service_information</tt>
	'''  method.
	''' </summary>
	Public NotInheritable Class ServiceInformation
		Implements org.omg.CORBA.portable.IDLEntity

		''' <summary>
		''' Array of ints representing service options.
		''' </summary>
		Public service_options As Integer()

		''' <summary>
		''' Array of ServiceDetails giving more details about the service.
		''' </summary>
		Public service_details As org.omg.CORBA.ServiceDetail()

		''' <summary>
		''' Constructs a ServiceInformation object with empty service_options
		''' and service_details.
		''' </summary>
		Public Sub New()
		End Sub

		''' <summary>
		''' Constructs a ServiceInformation object with the given service_options
		''' and service_details. </summary>
		''' <param name="__service_options"> An array of ints describing the service options. </param>
		''' <param name="__service_details"> An array of ServiceDetails describing the service
		''' details. </param>
		Public Sub New(ByVal __service_options As Integer(), ByVal __service_details As org.omg.CORBA.ServiceDetail())
			service_options = __service_options
			service_details = __service_details
		End Sub
	End Class

End Namespace