Imports System

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
Namespace org.omg.CORBA.portable

	''' <summary>
	''' This class is used for reporting locate forward exceptions and object forward
	''' GIOP messages back to the ORB. In this case the ORB must remarshal the request
	''' before trying again.
	''' Stubs which use the stream-based model shall catch the <code>RemarshalException</code>
	''' which is potentially thrown from the <code>_invoke()</code> method of <code>ObjectImpl</code>.
	''' Upon catching the exception, the stub shall immediately remarshal the request by calling
	''' <code>_request()</code>, marshalling the arguments (if any), and then calling
	''' <code>_invoke()</code>. The stub shall repeat this process until <code>_invoke()</code>
	''' returns normally or raises some exception other than <code>RemarshalException</code>.
	''' </summary>

	Public NotInheritable Class RemarshalException
		Inherits Exception

		''' <summary>
		''' Constructs a RemarshalException.
		''' </summary>
		Public Sub New()
			MyBase.New()
		End Sub
	End Class

End Namespace