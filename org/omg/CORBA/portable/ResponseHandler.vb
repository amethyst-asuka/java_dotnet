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
	''' This interface is supplied by an ORB to a servant at invocation time and allows
	''' the servant to later retrieve an OutputStream for returning the invocation results.
	''' </summary>

	Public Interface ResponseHandler
		''' <summary>
		''' Called by the servant during a method invocation. The servant
		''' should call this method to create a reply marshal buffer if no
		''' exception occurred.
		''' </summary>
		''' <returns> an OutputStream suitable for marshalling the reply.
		''' </returns>
		''' <seealso cref= <a href="package-summary.html#unimpl"><code>portable</code>
		''' package comments for unimplemented features</a> </seealso>
		Function createReply() As OutputStream

		''' <summary>
		''' Called by the servant during a method invocation. The servant
		''' should call this method to create a reply marshal buffer if a
		''' user exception occurred.
		''' </summary>
		''' <returns> an OutputStream suitable for marshalling the exception
		''' ID and the user exception body. </returns>
		Function createExceptionReply() As OutputStream
	End Interface

End Namespace