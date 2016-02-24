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
	''' This interface provides a dispatching mechanism for an incoming call.
	''' It is invoked by the ORB to dispatch a request to a servant.
	''' </summary>

	Public Interface InvokeHandler
		''' <summary>
		''' Invoked by the ORB to dispatch a request to the servant.
		''' 
		''' ORB passes the method name, an InputStream containing the
		''' marshalled arguments, and a ResponseHandler which the servant
		''' uses to construct a proper reply.
		''' 
		''' Only CORBA SystemException may be thrown by this method.
		''' 
		''' The method must return an OutputStream created by the
		''' ResponseHandler which contains the marshalled reply.
		''' 
		''' A servant must not retain a reference to the ResponseHandler
		''' beyond the lifetime of a method invocation.
		''' 
		''' Servant behaviour is defined as follows:
		''' <p>1. Determine correct method, and unmarshal parameters from
		'''    InputStream.
		''' <p>2. Invoke method implementation.
		''' <p>3. If no user exception, create a normal reply using
		'''    ResponseHandler.
		''' <p>4. If user exception occurred, create exception reply using
		'''    ResponseHandler.
		''' <p>5. Marshal reply into OutputStream returned by
		'''    ResponseHandler.
		''' <p>6. Return OutputStream to ORB.
		''' <p> </summary>
		''' <param name="method"> The method name. </param>
		''' <param name="input"> The <code>InputStream</code> containing the marshalled arguments. </param>
		''' <param name="handler"> The <code>ResponseHandler</code> which the servant uses
		''' to construct a proper reply </param>
		''' <returns> The <code>OutputStream</code> created by the
		''' ResponseHandler which contains the marshalled reply </returns>
		''' <exception cref="SystemException"> is thrown when invocation fails due to a CORBA system exception. </exception>

		Function _invoke(ByVal method As String, ByVal input As InputStream, ByVal handler As ResponseHandler) As OutputStream
	End Interface

End Namespace