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
'
' * Licensed Materials - Property of IBM
' * RMI-IIOP v1.0
' * Copyright IBM Corp. 1998 1999  All Rights Reserved
' *
' 

Namespace org.omg.CORBA.portable

	Public Interface BoxedValueHelper
		Function read_value(ByVal [is] As InputStream) As java.io.Serializable
		Sub write_value(ByVal os As OutputStream, ByVal value As java.io.Serializable)
		Function get_id() As String
	End Interface

End Namespace