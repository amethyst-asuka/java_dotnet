'
' * Copyright (c) 1997, 1999, Oracle and/or its affiliates. All rights reserved.
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
	''' A flag that can be used as the second parameter to the method
	''' <code>Context.get_values</code> to restrict the search scope.
	''' When this flag is used, it restricts the search for
	''' context values to this particular <code>Context</code> object
	''' or to the scope specified in the first parameter to
	''' <code>Context.get_values</code>.
	''' <P>
	''' Usage:
	''' <PRE>
	'''     NVList props = myContext.get_values("_USER",
	'''                     CTX_RESTRICT_SCOPE.value, "id*");
	''' </PRE>
	''' </summary>
	''' <seealso cref= org.omg.CORBA.Context#get_values(String, int, String)
	''' @since   JDK1.2 </seealso>
	Public Interface CTX_RESTRICT_SCOPE

	''' <summary>
	''' The field containing the <code>int</code> value of a
	''' <code>CTX_RESTRICT_SCOPE</code> flag.
	''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Interfaces cannot contain fields in .NET:
'	  int value = 15;
	End Interface

End Namespace