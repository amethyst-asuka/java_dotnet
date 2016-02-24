'
' * Copyright (c) 2002, Oracle and/or its affiliates. All rights reserved.
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

Namespace org.omg.CORBA.portable

	''' <summary>
	''' Java to IDL ptc 02-01-12 1.5.1.3
	''' 
	''' ValueOutputStream is used for implementing RMI-IIOP
	''' stream format version 2.
	''' </summary>
	Public Interface ValueOutputStream
		''' <summary>
		''' The start_value method ends any currently open chunk,
		''' writes a valuetype header for a nested custom valuetype
		''' (with a null codebase and the specified repository ID),
		''' and increments the valuetype nesting depth.
		''' </summary>
		Sub start_value(ByVal rep_id As String)

		''' <summary>
		''' The end_value method ends any currently open chunk,
		''' writes the end tag for the nested custom valuetype,
		''' and decrements the valuetype nesting depth.
		''' </summary>
		Sub end_value()
	End Interface

End Namespace