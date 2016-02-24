'
' * Copyright (c) 1998, 2010, Oracle and/or its affiliates. All rights reserved.
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
	''' This class is part of the local stub API, the purpose of which is to provide
	''' high performance calls for collocated clients and servers
	''' (i.e. clients and servers residing in the same Java VM).
	''' The local stub API is supported via three additional methods on
	''' <code>ObjectImpl</code> and <code>Delegate</code>.
	''' ORB vendors may subclass this class to return additional
	''' request state that may be required by their implementations. </summary>
	''' <seealso cref= ObjectImpl </seealso>
	''' <seealso cref= Delegate </seealso>

	Public Class ServantObject
		''' <summary>
		''' The real servant. The local stub may cast this field to the expected type, and then
		''' invoke the operation directly. Note, the object may or may not be the actual servant
		''' instance.
		''' </summary>
		Public servant As Object
	End Class

End Namespace