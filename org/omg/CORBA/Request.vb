'
' * Copyright (c) 1996, 1999, Oracle and/or its affiliates. All rights reserved.
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
	''' An object containing the information necessary for
	''' invoking a method.  This class is
	''' the cornerstone of the ORB Dynamic
	''' Invocation Interface (DII), which allows dynamic creation and
	''' invocation of requests.
	''' A server cannot tell the difference between a client
	''' invocation using a client stub and a request using the DII.
	''' <P>
	''' A <code>Request</code> object consists of:
	''' <UL>
	''' <LI>the name of the operation to be invoked
	''' <LI>an <code>NVList</code> containing arguments for the operation.<BR>
	''' Each item in the list is a <code>NamedValue</code> object, which has three
	''' parts:
	'''  <OL>
	'''    <LI>the name of the argument
	'''    <LI>the value of the argument (as an <code>Any</code> object)
	'''    <LI>the argument mode flag indicating whether the argument is
	'''        for input, output, or both
	'''  </OL>
	''' </UL>
	''' <P>
	''' <code>Request</code> objects may also contain additional information,
	''' depending on how an operation was defined in the original IDL
	''' interface definition.  For example, where appropriate, they may contain
	''' a <code>NamedValue</code> object to hold the return value or exception,
	''' a context, a list of possible exceptions, and a list of
	''' context strings that need to be resolved.
	''' <P>
	''' New <code>Request</code> objects are created using one of the
	''' <code>create_request</code> methods in the <code>Object</code> class.
	''' In other words, a <code>create_request</code> method is performed on the
	''' object which is to be invoked.
	''' </summary>
	''' <seealso cref= org.omg.CORBA.NamedValue
	'''  </seealso>

	Public MustInherit Class Request

		''' <summary>
		''' Retrieves the the target object reference.
		''' </summary>
		''' <returns>                  the object reference that points to the
		'''                    object implementation for the method
		'''                    to be invoked </returns>

		Public MustOverride Function target() As org.omg.CORBA.Object

		''' <summary>
		''' Retrieves the name of the method to be invoked.
		''' </summary>
		''' <returns>                  the name of the method to be invoked </returns>

		Public MustOverride Function operation() As String

		''' <summary>
		''' Retrieves the <code>NVList</code> object containing the arguments
		''' to the method being invoked.  The elements in the list are
		''' <code>NamedValue</code> objects, with each one describing an argument
		''' to the method.
		''' </summary>
		''' <returns>  the <code>NVList</code> object containing the arguments
		'''                  for the method
		'''  </returns>

		Public MustOverride Function arguments() As NVList

		''' <summary>
		''' Retrieves the <code>NamedValue</code> object containing the return
		''' value for the method.
		''' </summary>
		''' <returns>          the <code>NamedValue</code> object containing the result
		'''                          of the method </returns>

		Public MustOverride Function result() As NamedValue

		''' <summary>
		''' Retrieves the <code>Environment</code> object for this request.
		''' It contains the exception that the method being invoked has
		''' thrown (after the invocation returns).
		''' 
		''' </summary>
		''' <returns>  the <code>Environment</code> object for this request </returns>

		Public MustOverride Function env() As Environment

		''' <summary>
		''' Retrieves the <code>ExceptionList</code> object for this request.
		''' This list contains <code>TypeCode</code> objects describing the
		''' exceptions that may be thrown by the method being invoked.
		''' </summary>
		''' <returns>  the <code>ExceptionList</code> object describing the exceptions
		'''            that may be thrown by the method being invoked </returns>

		Public MustOverride Function exceptions() As ExceptionList

		''' <summary>
		''' Retrieves the <code>ContextList</code> object for this request.
		''' This list contains context <code>String</code>s that need to
		''' be resolved and sent with the invocation.
		''' 
		''' </summary>
		''' <returns>                  the list of context strings whose values
		'''                          need to be resolved and sent with the
		'''                          invocation. </returns>

		Public MustOverride Function contexts() As ContextList

		''' <summary>
		''' Retrieves the <code>Context</code> object for this request.
		''' This is a list of properties giving information about the
		''' client, the environment, or the circumstances of this request.
		''' </summary>
		''' <returns>          the <code>Context</code> object that is to be used
		'''                          to resolve any context strings whose
		'''                          values need to be sent with the invocation </returns>

		Public MustOverride Function ctx() As Context

		''' <summary>
		''' Sets this request's <code>Context</code> object to the one given.
		''' </summary>
		''' <param name="c">         the new <code>Context</code> object to be used for
		'''                          resolving context strings </param>

		Public MustOverride Sub ctx(ByVal c As Context)


		''' <summary>
		''' Creates an input argument and adds it to this <code>Request</code>
		''' object.
		''' </summary>
		''' <returns>          an <code>Any</code> object that contains the
		'''                value and typecode for the input argument added </returns>

		Public MustOverride Function add_in_arg() As Any

		''' <summary>
		''' Creates an input argument with the given name and adds it to
		''' this <code>Request</code> object.
		''' </summary>
		''' <param name="name">              the name of the argument being added </param>
		''' <returns>          an <code>Any</code> object that contains the
		'''                value and typecode for the input argument added </returns>

		Public MustOverride Function add_named_in_arg(ByVal name As String) As Any

		''' <summary>
		''' Adds an input/output argument to this <code>Request</code> object.
		''' </summary>
		''' <returns>          an <code>Any</code> object that contains the
		'''                value and typecode for the input/output argument added </returns>

		Public MustOverride Function add_inout_arg() As Any

		''' <summary>
		''' Adds an input/output argument with the given name to this
		''' <code>Request</code> object.
		''' </summary>
		''' <param name="name">              the name of the argument being added </param>
		''' <returns>          an <code>Any</code> object that contains the
		'''                value and typecode for the input/output argument added </returns>

		Public MustOverride Function add_named_inout_arg(ByVal name As String) As Any


		''' <summary>
		''' Adds an output argument to this <code>Request</code> object.
		''' </summary>
		''' <returns>          an <code>Any</code> object that contains the
		'''                value and typecode for the output argument added </returns>

		Public MustOverride Function add_out_arg() As Any

		''' <summary>
		''' Adds an output argument with the given name to this
		''' <code>Request</code> object.
		''' </summary>
		''' <param name="name">              the name of the argument being added </param>
		''' <returns>          an <code>Any</code> object that contains the
		'''                value and typecode for the output argument added </returns>

		Public MustOverride Function add_named_out_arg(ByVal name As String) As Any

		''' <summary>
		''' Sets the typecode for the return
		''' value of the method.
		''' </summary>
		''' <param name="tc">                        the <code>TypeCode</code> object containing type information
		'''                   for the return value </param>

		Public MustOverride Sub set_return_type(ByVal tc As TypeCode)

		''' <summary>
		''' Returns the <code>Any</code> object that contains the value for the
		''' result of the method.
		''' </summary>
		''' <returns>                  an <code>Any</code> object containing the value and
		'''                   typecode for the return value </returns>

		Public MustOverride Function return_value() As Any

		''' <summary>
		''' Makes a synchronous invocation using the
		''' information in the <code>Request</code> object. Exception information is
		''' placed into the <code>Request</code> object's environment object.
		''' </summary>

		Public MustOverride Sub invoke()

		''' <summary>
		''' Makes a oneway invocation on the
		''' request. In other words, it does not expect or wait for a
		''' response. Note that this can be used even if the operation was
		''' not declared as oneway in the IDL declaration. No response or
		''' exception information is returned.
		''' </summary>

		Public MustOverride Sub send_oneway()

		''' <summary>
		''' Makes an asynchronous invocation on
		''' the request. In other words, it does not wait for a response before it
		''' returns to the user. The user can then later use the methods
		''' <code>poll_response</code> and <code>get_response</code> to get
		''' the result or exception information for the invocation.
		''' </summary>

		Public MustOverride Sub send_deferred()

		''' <summary>
		''' Allows the user to determine
		''' whether a response has been received for the invocation triggered
		''' earlier with the <code>send_deferred</code> method.
		''' </summary>
		''' <returns>          <code>true</code> if the method response has
		'''                          been received; <code>false</code> otherwise </returns>

		Public MustOverride Function poll_response() As Boolean

		''' <summary>
		''' Allows the user to access the
		''' response for the invocation triggered earlier with the
		''' <code>send_deferred</code> method.
		''' </summary>
		''' <exception cref="WrongTransaction">  if the method <code>get_response</code> was invoked
		''' from a different transaction's scope than the one from which the
		''' request was originally sent. See the OMG Transaction Service specification
		''' for details. </exception>

		Public MustOverride Sub get_response()

	End Class

End Namespace