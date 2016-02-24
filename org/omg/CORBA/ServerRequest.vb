Imports System

'
' * Copyright (c) 1996, 2004, Oracle and/or its affiliates. All rights reserved.
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
	''' An object that captures the explicit state of a request
	''' for the Dynamic Skeleton Interface (DSI).  This class, the
	''' cornerstone of the DSI, is analogous to the <code>Request</code>
	''' object in the DII.
	''' <P>
	''' The ORB is responsible for creating this embodiment of a request,
	''' and delivering it to a Dynamic Implementation Routine (DIR).
	''' A dynamic servant (a DIR) is created by implementing the
	''' <code>DynamicImplementation</code> class,
	''' which has a single <code>invoke</code> method.  This method accepts a
	''' <code>ServerRequest</code> object.
	''' 
	''' The abstract class <code>ServerRequest</code> defines
	''' methods for accessing the
	''' method name, the arguments and the context of the request, as
	''' well as methods for setting the result of the request either as a
	''' return value or an exception. <p>
	''' 
	''' A subtlety with accessing the arguments of the request is that the
	''' DIR needs to provide type information about the
	''' expected arguments, since there is no compiled information about
	''' these. This information is provided through an <code>NVList</code>,
	''' which is a list of <code>NamedValue</code> objects.
	''' Each <code>NamedValue</code> object
	''' contains an <code>Any</code> object, which in turn
	''' has a <code>TypeCode</code> object representing the type
	''' of the argument. <p>
	''' 
	''' Similarly, type information needs to be provided for the response,
	''' for either the expected result or for an exception, so the methods
	''' <code>result</code> and <code>except</code> take an <code>Any</code>
	''' object as a parameter. <p>
	''' </summary>
	''' <seealso cref= org.omg.CORBA.DynamicImplementation </seealso>
	''' <seealso cref= org.omg.CORBA.NVList </seealso>
	''' <seealso cref= org.omg.CORBA.NamedValue
	'''  </seealso>

	Public MustInherit Class ServerRequest

		''' <summary>
		''' Retrieves the name of the operation being
		''' invoked. According to OMG IDL's rules, these names must be unique
		''' among all operations supported by this object's "most-derived"
		''' interface. Note that the operation names for getting and setting
		''' attributes are <code>_get_&lt;attribute_name&gt;</code>
		''' and <code>_set_&lt;attribute_name&gt;</code>,
		''' respectively.
		''' </summary>
		''' <returns>     the name of the operation to be invoked </returns>
		''' @deprecated use operation() 
		<Obsolete("use operation()")> _
		Public Overridable Function op_name() As String
			Return operation()
		End Function


		''' <summary>
		''' Throws an <code>org.omg.CORBA.NO_IMPLEMENT</code> exception.
		''' <P>
		''' Retrieves the name of the operation being
		''' invoked. According to OMG IDL's rules, these names must be unique
		''' among all operations supported by this object's "most-derived"
		''' interface. Note that the operation names for getting and setting
		''' attributes are <code>_get_&lt;attribute_name&gt;</code>
		''' and <code>_set_&lt;attribute_name&gt;</code>,
		''' respectively.
		''' </summary>
		''' <returns>     the name of the operation to be invoked </returns>
		''' <seealso cref= <a href="package-summary.html#unimpl"><code>CORBA</code>
		'''      package comments for unimplemented features</a> </seealso>
		Public Overridable Function operation() As String
			Throw New org.omg.CORBA.NO_IMPLEMENT
		End Function


		''' <summary>
		''' Specifies method parameter types and retrieves "in" and "inout"
		''' argument values.
		''' <P>
		''' Note that this method is deprecated; use the method
		''' <code>arguments</code> in its place.
		''' <P>
		''' Unless it calls the method <code>set_exception</code>,
		''' the DIR must call this method exactly once, even if the
		''' method signature contains no parameters. Once the method <code>
		''' arguments</code> or <code>set_exception</code>
		''' has been called, calling <code>arguments</code> on the same
		''' <code>ServerRequest</code> object
		''' will result in a <code>BAD_INV_ORDER</code> system exception.
		''' The DIR must pass in to the method <code>arguments</code>
		''' an NVList initialized with TypeCodes and Flags
		''' describing the parameter types for the operation, in the order in which
		''' they appear in the IDL specification (left to right). A
		''' potentially-different NVList will be returned from
		''' <code>arguments</code>, with the
		''' "in" and "inout" argument values supplied. If it does not call
		''' the method <code>set_exception</code>,
		''' the DIR must supply the returned NVList with return
		''' values for any "out" arguments before returning, and may also change
		''' the return values for any "inout" arguments.
		''' </summary>
		''' <param name="params">            the arguments of the method, in the
		'''                          form of an <code>NVList</code> object </param>
		''' @deprecated use the method <code>arguments</code> 
		<Obsolete("use the method <code>arguments</code>")> _
		Public Overridable Sub params(ByVal params As NVList)
			arguments(params)
		End Sub

		''' <summary>
		''' Specifies method parameter types and retrieves "in" and "inout"
		''' argument values.
		''' Unless it calls the method <code>set_exception</code>,
		''' the DIR must call this method exactly once, even if the
		''' method signature contains no parameters. Once the method <code>
		''' arguments</code> or <code>set_exception</code>
		''' has been called, calling <code>arguments</code> on the same
		''' <code>ServerRequest</code> object
		''' will result in a <code>BAD_INV_ORDER</code> system exception.
		''' The DIR must pass in to the method <code>arguments</code>
		''' an NVList initialized with TypeCodes and Flags
		''' describing the parameter types for the operation, in the order in which
		''' they appear in the IDL specification (left to right). A
		''' potentially-different NVList will be returned from
		''' <code>arguments</code>, with the
		''' "in" and "inout" argument values supplied. If it does not call
		''' the method <code>set_exception</code>,
		''' the DIR must supply the returned NVList with return
		''' values for any "out" arguments before returning, and it may also change
		''' the return values for any "inout" arguments.
		''' </summary>
		''' <param name="args">              the arguments of the method, in the
		'''                            form of an NVList </param>
		''' <seealso cref= <a href="package-summary.html#unimpl"><code>CORBA</code>
		'''      package comments for unimplemented features</a> </seealso>
		Public Overridable Sub arguments(ByVal args As org.omg.CORBA.NVList)
			Throw New org.omg.CORBA.NO_IMPLEMENT
		End Sub



		''' <summary>
		''' Specifies any return value for the call.
		''' <P>
		''' Note that this method is deprecated; use the method
		''' <code>set_result</code> in its place.
		''' <P>
		''' Unless the method
		''' <code>set_exception</code> is called, if the invoked method
		''' has a non-void result type, the method <code>set_result</code>
		''' must be called exactly once before the DIR returns.
		''' If the operation has a void result type, the method
		''' <code>set_result</code> may optionally be
		''' called once with an <code>Any</code> object whose type is
		''' <code>tk_void</code>. Calling the method <code>set_result</code> before
		''' the method <code>arguments</code> has been called or after
		''' the method <code>set_result</code> or <code>set_exception</code> has been
		''' called will result in a BAD_INV_ORDER exception. Calling the method
		''' <code>set_result</code> without having previously called
		''' the method <code>ctx</code> when the IDL operation contains a
		''' context expression, or when the NVList passed to arguments did not
		''' describe all parameters passed by the client, may result in a MARSHAL
		''' system exception.
		''' </summary>
		''' <param name="any"> an <code>Any</code> object containing the return value to be set </param>
		''' @deprecated use the method <code>set_result</code> 
		<Obsolete("use the method <code>set_result</code>")> _
		Public Overridable Sub result(ByVal any As Any)
			set_result(any)
		End Sub


		''' <summary>
		''' Throws an <code>org.omg.CORBA.NO_IMPLEMENT</code> exception.
		''' <P>
		''' Specifies any return value for the call. Unless the method
		''' <code>set_exception</code> is called, if the invoked method
		''' has a non-void result type, the method <code>set_result</code>
		''' must be called exactly once before the DIR returns.
		''' If the operation has a void result type, the method
		''' <code>set_result</code> may optionally be
		''' called once with an <code>Any</code> object whose type is
		''' <code>tk_void</code>. Calling the method <code>set_result</code> before
		''' the method <code>arguments</code> has been called or after
		''' the method <code>set_result</code> or <code>set_exception</code> has been
		''' called will result in a BAD_INV_ORDER exception. Calling the method
		''' <code>set_result</code> without having previously called
		''' the method <code>ctx</code> when the IDL operation contains a
		''' context expression, or when the NVList passed to arguments did not
		''' describe all parameters passed by the client, may result in a MARSHAL
		''' system exception.
		''' </summary>
		''' <param name="any"> an <code>Any</code> object containing the return value to be set </param>
		''' <seealso cref= <a href="package-summary.html#unimpl"><code>CORBA</code>
		'''      package comments for unimplemented features</a> </seealso>
		Public Overridable Sub set_result(ByVal any As org.omg.CORBA.Any)
			Throw New org.omg.CORBA.NO_IMPLEMENT
		End Sub


		''' <summary>
		''' The DIR may call set_exception at any time to return an exception to the
		''' client. The Any passed to set_exception must contain either a system
		''' exception or a user exception specified in the raises expression
		''' of the invoked operation's IDL definition. Passing in an Any that does
		''' not
		''' contain an exception will result in a BAD_PARAM system exception. Passing
		''' in an unlisted user exception will result in either the DIR receiving a
		''' BAD_PARAM system exception or in the client receiving an
		''' UNKNOWN_EXCEPTION system exception.
		''' </summary>
		''' <param name="any">       the <code>Any</code> object containing the exception </param>
		''' @deprecated use set_exception() 
		<Obsolete("use set_exception()")> _
		Public Overridable Sub except(ByVal any As Any)
			set_exception(any)
		End Sub

		''' <summary>
		''' Throws an <code>org.omg.CORBA.NO_IMPLEMENT</code> exception.
		''' <P>
		''' Returns the given exception to the client.  This method
		''' is invoked by the DIR, which may call it at any time.
		''' The <code>Any</code> object  passed to this method must
		''' contain either a system
		''' exception or one of the user exceptions specified in the
		''' invoked operation's IDL definition. Passing in an
		''' <code>Any</code> object that does not contain an exception
		''' will cause a BAD_PARAM system exception to be thrown. Passing
		''' in an unlisted user exception will result in either the DIR receiving a
		''' BAD_PARAM system exception or in the client receiving an
		''' UNKNOWN_EXCEPTION system exception.
		''' </summary>
		''' <param name="any">       the <code>Any</code> object containing the exception </param>
		''' <exception cref="BAD_PARAM"> if the given <code>Any</code> object does not
		'''                      contain an exception or the exception is an
		'''                      unlisted user exception </exception>
		''' <exception cref="UNKNOWN_EXCEPTION"> if the given exception is an unlisted
		'''                              user exception and the DIR did not
		'''                              receive a BAD_PARAM exception </exception>
		''' <seealso cref= <a href="package-summary.html#unimpl"><code>CORBA</code>
		'''      package comments for unimplemented features</a> </seealso>
		Public Overridable Sub set_exception(ByVal any As Any)
			Throw New org.omg.CORBA.NO_IMPLEMENT
		End Sub

		''' <summary>
		''' Returns the context information specified in IDL for the operation
		''' when the operation is not an attribute access and the operation's IDL
		''' definition contains a context expression; otherwise it returns
		''' a nil <code>Context</code> reference. Calling the method
		''' <code>ctx</code> before the method <code>arguments</code> has
		''' been called or after the method <code>ctx</code>,
		''' <code>set_result</code>, or <code>set_exception</code>
		''' has been called will result in a
		''' BAD_INV_ORDER system exception.
		''' </summary>
		''' <returns>                  the context object that is to be used
		'''                          to resolve any context strings whose
		'''                          values need to be sent with the invocation. </returns>
		''' <exception cref="BAD_INV_ORDER"> if (1) the method <code>ctx</code> is called
		'''                          before the method <code>arguments</code> or
		'''                          (2) the method <code>ctx</code> is called
		'''                          after calling <code>set_result</code> or
		'''                          <code>set_exception</code> </exception>
		Public MustOverride Function ctx() As Context

	End Class

End Namespace