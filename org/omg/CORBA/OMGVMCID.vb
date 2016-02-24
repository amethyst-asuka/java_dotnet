'
' * Copyright (c) 1999, Oracle and/or its affiliates. All rights reserved.
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
	''' The vendor minor code ID reserved for OMG. Minor codes for the standard
	''' exceptions are prefaced by the VMCID assigned to OMG, defined as the
	''' constant OMGVMCID, which, like all VMCIDs, occupies the high order 20 bits.
	''' </summary>

	Public Interface OMGVMCID

		''' <summary>
		''' The vendor minor code ID reserved for OMG. This value is or'd with
		''' the high order 20 bits of the minor code to produce the minor value
		''' in a system exception.
		''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Interfaces cannot contain fields in .NET:
'		static final int value = &H4f4d0000;
	End Interface

End Namespace