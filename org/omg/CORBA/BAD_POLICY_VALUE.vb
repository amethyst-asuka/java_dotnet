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
	''' Contains the value used to indicate a policy value that is
	''' incorrect for a valid policy type in a call to the
	''' <code>create_policy</code> method defined in the ORB class.
	''' 
	''' </summary>
	Public Interface BAD_POLICY_VALUE
		''' <summary>
		''' The value used to represent a bad policy value error
		''' in a <code>PolicyError</code> exception. </summary>
		''' <seealso cref= org.omg.CORBA.PolicyError </seealso>
'JAVA TO VB CONVERTER TODO TASK: Interfaces cannot contain fields in .NET:
'		final short value = (short)(3L);
	End Interface

End Namespace