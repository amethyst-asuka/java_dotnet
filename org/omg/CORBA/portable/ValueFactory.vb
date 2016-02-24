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

	''' <summary>
	''' The ValueFactory interface is the native mapping for the IDL
	''' type CORBA::ValueFactory. The read_value() method is called by
	''' the ORB runtime while in the process of unmarshaling a value type.
	''' A user shall implement this method as part of implementing a type
	''' specific value factory. In the implementation, the user shall call
	''' is.read_value(java.io.Serializable) with a uninitialized valuetype
	''' to use for unmarshaling. The value returned by the stream is
	''' the same value passed in, with all the data unmarshaled. </summary>
	''' <seealso cref= org.omg.CORBA_2_3.ORB </seealso>

	Public Interface ValueFactory
		''' <summary>
		''' Is called by
		''' the ORB runtime while in the process of unmarshaling a value type.
		''' A user shall implement this method as part of implementing a type
		''' specific value factory. </summary>
		''' <param name="is"> an InputStream object--from which the value will be read. </param>
		''' <returns> a Serializable object--the value read off of "is" Input stream. </returns>
		Function read_value(ByVal [is] As org.omg.CORBA_2_3.portable.InputStream) As java.io.Serializable
	End Interface

End Namespace