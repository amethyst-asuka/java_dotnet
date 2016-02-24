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
	''' Signifies an "input" argument to an invocation,
	''' meaning that the argument is being passed from the client to
	''' the server.
	''' <code>ARG_IN.value</code> is one of the possible values used to
	''' indicate the direction in
	''' which a parameter is being passed during an invocation performed
	''' using the Dynamic Invocation Interface (DII).
	''' <P>
	''' The code fragment below shows a typical usage:
	''' <PRE>
	'''    ORB orb = ORB.init(args, null);
	'''    org.omg.CORBA.NamedValue nv = orb.create_named_value(
	'''         "IDLArgumentIdentifier", myAny, org.omg.CORBA.ARG_IN.value);
	''' </PRE>
	''' </summary>
	''' <seealso cref=     org.omg.CORBA.NamedValue
	''' @since   JDK1.2 </seealso>
	Public Interface ARG_IN

		''' <summary>
		''' The value indicating an input argument.
		''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Interfaces cannot contain fields in .NET:
'		int value = 1;
	End Interface

End Namespace