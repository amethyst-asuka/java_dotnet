'
' * Copyright (c) 1999, 2003, Oracle and/or its affiliates. All rights reserved.
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
Namespace org.omg.PortableServer

	''' <summary>
	''' Allows dynamic handling of object invocations.  POA-based DSI
	''' servants inherit from the
	''' standard <code>DynamicImplementation</code> class, this class inherits
	''' from the <code>Servant</code> class. Based on IDL to Java spec.
	''' CORBA V 2.3.1 ptc/00-01-08.pdf.
	''' </summary>
	Public MustInherit Class DynamicImplementation
		Inherits Servant

	''' <summary>
	''' Receives requests issued to any CORBA object
	''' incarnated by the DSI servant and performs the processing
	''' necessary to execute the request. </summary>
	''' <param name="request"> the request issued to the CORBA object. </param>
		Public MustOverride Sub invoke(ByVal request As org.omg.CORBA.ServerRequest)
	End Class

End Namespace