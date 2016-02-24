'
' * Copyright (c) 1998, 1999, Oracle and/or its affiliates. All rights reserved.
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
	''' Defines the code used to represent a truncatable value type in
	''' a typecode. A value type is truncatable if it inherits "safely"
	''' from another value type, which means it can be cast to a more
	''' general inherited type.
	''' This is one of the possible results of the <code>type_modifier</code>
	''' method on the <code>TypeCode</code> interface. </summary>
	''' <seealso cref= org.omg.CORBA.TypeCode </seealso>
	Public Interface VM_TRUNCATABLE
		''' <summary>
		''' The value representing a truncatable value type in
		''' a typecode.
		''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Interfaces cannot contain fields in .NET:
'		final short value = (short)(3L);
	End Interface

End Namespace