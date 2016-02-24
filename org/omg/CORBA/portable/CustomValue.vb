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

''' <summary>
''' Defines the base interface for all custom value types
''' generated from IDL.
''' 
''' All value types implement ValueBase either directly
''' or indirectly by implementing either the StreamableValue
''' or CustomValue interface.
''' @author OMG
''' </summary>

Namespace org.omg.CORBA.portable

	''' <summary>
	''' An extension of <code>ValueBase</code> that is implemented by custom value
	''' types.
	''' </summary>
	Public Interface CustomValue
		Inherits ValueBase, org.omg.CORBA.CustomMarshal

	End Interface

End Namespace