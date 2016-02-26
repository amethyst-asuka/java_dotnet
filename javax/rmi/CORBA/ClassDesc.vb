Imports System

'
' * Copyright (c) 1999, 2000, Oracle and/or its affiliates. All rights reserved.
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

Namespace javax.rmi.CORBA

	''' <summary>
	''' This class is used to marshal java.lang.Class objects over IIOP.
	''' </summary>
	<Serializable> _
	Public Class ClassDesc

		''' <summary>
		''' @serial The class's RepositoryId.
		''' </summary>
		Private repid As String

		''' <summary>
		''' @serial A space-separated list of codebase URLs.
		''' </summary>
		Private codebase As String
	End Class

End Namespace