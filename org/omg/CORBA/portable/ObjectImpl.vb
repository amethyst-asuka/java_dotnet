Imports System

'
' * Copyright (c) 1997, 2006, Oracle and/or its affiliates. All rights reserved.
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
	'''  The common base class for all stub classes; provides default implementations
	'''  of the <code>org.omg.CORBA.Object</code> methods. All method implementations are
	'''  forwarded to a <code>Delegate</code> object stored in the <code>ObjectImpl</code>
	'''  instance.  <code>ObjectImpl</code> allows for portable stubs because the
	'''  <code>Delegate</code> can be implemented by a different vendor-specific ORB.
	''' </summary>

	Public MustInherit Class ObjectImpl
		Implements org.omg.CORBA.Object

			Public MustOverride Function _set_policy_override(ByVal policies As Policy(), ByVal set_add As SetOverrideType) As org.omg.CORBA.Object
			Public MustOverride Function _create_request(ByVal ctx As org.omg.CORBA.Context, ByVal operation As String, ByVal arg_list As org.omg.CORBA.NVList, ByVal result As org.omg.CORBA.NamedValue, ByVal exclist As org.omg.CORBA.ExceptionList, ByVal ctxlist As org.omg.CORBA.ContextList) As org.omg.CORBA.Request
			Public MustOverride Function _create_request(ByVal ctx As org.omg.CORBA.Context, ByVal operation As String, ByVal arg_list As org.omg.CORBA.NVList, ByVal result As org.omg.CORBA.NamedValue) As org.omg.CORBA.Request
			Public MustOverride Function _is_equivalent(ByVal other As org.omg.CORBA.Object) As Boolean
			Public MustOverride Function _is_a(ByVal repositoryIdentifier As String) As Boolean

		''' <summary>
		''' The field that stores the <code>Delegate</code> instance for
		''' this <code>ObjectImpl</code> object. This <code>Delegate</code>
		''' instance can be implemented by a vendor-specific ORB.  Stub classes,
		''' which are derived from this <code>ObjectImpl</code> class, can be
		''' portable because they delegate all of the methods called on them to this
		''' <code>Delegate</code> object.
		''' </summary>
		<NonSerialized> _
		Private __delegate As System.Delegate


		''' <summary>
		''' Retrieves the reference to the vendor-specific <code>Delegate</code>
		''' object to which this <code>ObjectImpl</code> object delegates all
		''' methods invoked on it.
		''' </summary>
		''' <returns> the Delegate contained in this ObjectImpl instance </returns>
		''' <exception cref="BAD_OPERATION"> if the delegate has not been set </exception>
		''' <seealso cref= #_set_delegate </seealso>
		Public Overridable Function _get_delegate() As System.Delegate
			If __delegate Is Nothing Then Throw New org.omg.CORBA.BAD_OPERATION("The delegate has not been set!")
			Return __delegate
		End Function


		''' <summary>
		''' Sets the Delegate for this <code>ObjectImpl</code> instance to the given
		''' <code>Delegate</code> object.  All method invocations on this
		''' <code>ObjectImpl</code> object will be forwarded to this delegate.
		''' </summary>
		''' <param name="delegate"> the <code>Delegate</code> instance to which
		'''        all method calls on this <code>ObjectImpl</code> object
		'''        will be delegated; may be implemented by a third-party ORB </param>
		''' <seealso cref= #_get_delegate </seealso>
		Public Overridable Sub _set_delegate(ByVal [delegate] As System.Delegate)
			__delegate = [delegate]
		End Sub

		''' <summary>
		''' Retrieves a string array containing the repository identifiers
		''' supported by this <code>ObjectImpl</code> object.  For example,
		''' for a stub, this method returns information about all the
		''' interfaces supported by the stub.
		''' </summary>
		''' <returns> the array of all repository identifiers supported by this
		'''         <code>ObjectImpl</code> instance </returns>
		Public MustOverride Function _ids() As String()


		''' <summary>
		''' Returns a duplicate of this <code>ObjectImpl</code> object.
		''' </summary>
		''' <returns> an <code>orb.omg.CORBA.Object</code> object that is
		'''         a duplicate of this object </returns>
		Public Overridable Function _duplicate() As org.omg.CORBA.Object
			Return _get_delegate().duplicate(Me)
		End Function

		''' <summary>
		''' Releases the resources associated with this <code>ObjectImpl</code> object.
		''' </summary>
		Public Overridable Sub _release()
			_get_delegate().release(Me)
		End Sub

		''' <summary>
		''' Checks whether the object identified by the given repository
		''' identifier is an <code>ObjectImpl</code> object.
		''' </summary>
		''' <param name="repository_id"> a <code>String</code> object with the repository
		'''        identifier to check </param>
		''' <returns> <code>true</code> if the object identified by the given
		'''         repository id is an instance of <code>ObjectImpl</code>;
		'''         <code>false</code> otherwise </returns>
		Public Overridable Function _is_a(ByVal repository_id As String) As Boolean
			Return _get_delegate().is_a(Me, repository_id)
		End Function

		''' <summary>
		''' Checks whether the the given <code>ObjectImpl</code> object is
		''' equivalent to this <code>ObjectImpl</code> object.
		''' </summary>
		''' <param name="that"> an instance of <code>ObjectImpl</code> to compare with
		'''        this <code>ObjectImpl</code> object </param>
		''' <returns> <code>true</code> if the given object is equivalent
		'''         to this <code>ObjectImpl</code> object;
		'''         <code>false</code> otherwise </returns>
		Public Overridable Function _is_equivalent(ByVal that As org.omg.CORBA.Object) As Boolean
			Return _get_delegate().is_equivalent(Me, that)
		End Function

		''' <summary>
		''' Checks whether the server object for this <code>ObjectImpl</code>
		''' object has been destroyed.
		''' </summary>
		''' <returns> <code>true</code> if the ORB knows authoritatively that the
		'''         server object does not exist; <code>false</code> otherwise </returns>
		Public Overridable Function _non_existent() As Boolean
			Return _get_delegate().non_existent(Me)
		End Function

		''' <summary>
		''' Retrieves the hash code that serves as an ORB-internal identifier for
		''' this <code>ObjectImpl</code> object.
		''' </summary>
		''' <param name="maximum"> an <code>int</code> indicating the upper bound on the hash
		'''        value returned by the ORB </param>
		''' <returns> an <code>int</code> representing the hash code for this
		'''         <code>ObjectImpl</code> object </returns>
		Public Overridable Function _hash(ByVal maximum As Integer) As Integer
			Return _get_delegate().hash(Me, maximum)
		End Function

		''' <summary>
		''' Creates a <code>Request</code> object containing the given method
		''' that can be used with the Dynamic Invocation Interface.
		''' </summary>
		''' <param name="operation"> the method to be invoked by the new <code>Request</code>
		'''        object </param>
		''' <returns> a new <code>Request</code> object initialized with the
		'''         given method </returns>
		Public Overridable Function _request(ByVal operation As String) As org.omg.CORBA.Request
			Return _get_delegate().request(Me, operation)
		End Function

		''' <summary>
		''' Creates a <code>Request</code> object that contains the given context,
		''' method, argument list, and container for the result.
		''' </summary>
		''' <param name="ctx"> the Context for the request </param>
		''' <param name="operation"> the method that the new <code>Request</code>
		'''        object will invoke </param>
		''' <param name="arg_list"> the arguments for the method; an <code>NVList</code>
		'''        in which each argument is a <code>NamedValue</code> object </param>
		''' <param name="result"> a <code>NamedValue</code> object to be used for
		'''        returning the result of executing the request's method </param>
		''' <returns> a new <code>Request</code> object initialized with the
		'''         given context, method, argument list, and container for the
		'''         return value </returns>
		Public Overridable Function _create_request(ByVal ctx As org.omg.CORBA.Context, ByVal operation As String, ByVal arg_list As org.omg.CORBA.NVList, ByVal result As org.omg.CORBA.NamedValue) As org.omg.CORBA.Request
			Return _get_delegate().create_request(Me, ctx, operation, arg_list, result)
		End Function

		''' <summary>
		''' Creates a <code>Request</code> object that contains the given context,
		''' method, argument list, container for the result, exceptions, and
		''' list of property names to be used in resolving the context strings.
		''' This <code>Request</code> object is for use in the Dynamic
		''' Invocation Interface.
		''' </summary>
		''' <param name="ctx"> the <code>Context</code> object that contains the
		'''        context strings that must be resolved before they are
		'''        sent along with the request </param>
		''' <param name="operation"> the method that the new <code>Request</code>
		'''        object will invoke </param>
		''' <param name="arg_list"> the arguments for the method; an <code>NVList</code>
		'''        in which each argument is a <code>NamedValue</code> object </param>
		''' <param name="result"> a <code>NamedValue</code> object to be used for
		'''        returning the result of executing the request's method </param>
		''' <param name="exceptions"> a list of the exceptions that the given method
		'''        throws </param>
		''' <param name="contexts"> a list of the properties that are needed to
		'''        resolve the contexts in <i>ctx</i>; the strings in
		'''        <i>contexts</i> are used as arguments to the method
		'''        <code>Context.get_values</code>,
		'''        which returns the value associated with the given property </param>
		''' <returns> a new <code>Request</code> object initialized with the
		'''         given context strings to resolve, method, argument list,
		'''         container for the result, exceptions, and list of property
		'''         names to be used in resolving the context strings </returns>
		Public Overridable Function _create_request(ByVal ctx As org.omg.CORBA.Context, ByVal operation As String, ByVal arg_list As org.omg.CORBA.NVList, ByVal result As org.omg.CORBA.NamedValue, ByVal exceptions As org.omg.CORBA.ExceptionList, ByVal contexts As org.omg.CORBA.ContextList) As org.omg.CORBA.Request
			Return _get_delegate().create_request(Me, ctx, operation, arg_list, result, exceptions, contexts)
		End Function

		''' <summary>
		''' Retrieves the interface definition for this <code>ObjectImpl</code>
		''' object.
		''' </summary>
		''' <returns> the <code>org.omg.CORBA.Object</code> instance that is the
		'''         interface definition for this <code>ObjectImpl</code> object </returns>
		Public Overridable Function _get_interface_def() As org.omg.CORBA.Object
			' First try to call the delegate implementation class's
			' "Object get_interface_def(..)" method (will work for JDK1.2 ORBs).
			' Else call the delegate implementation class's
			' "InterfaceDef get_interface(..)" method using reflection
			' (will work for pre-JDK1.2 ORBs).

			Dim [delegate] As org.omg.CORBA.portable.Delegate = _get_delegate()
			Try
				' If the ORB's delegate class does not implement
				' "Object get_interface_def(..)", this will call
				' get_interface_def(..) on portable.Delegate.
				Return [delegate].get_interface_def(Me)
			Catch ex As org.omg.CORBA.NO_IMPLEMENT
				' Call "InterfaceDef get_interface(..)" method using reflection.
				Try
					Dim argc As Type() = { GetType(org.omg.CORBA.Object) }
					Dim meth As System.Reflection.MethodInfo = [delegate].GetType().GetMethod("get_interface", argc)
					Dim argx As Object() = { Me }
					Return CType(meth.invoke([delegate], argx), org.omg.CORBA.Object)
				Catch exs As java.lang.reflect.InvocationTargetException
					Dim t As Exception = exs.targetException
					If TypeOf t Is Exception Then
						Throw CType(t, [Error])
					ElseIf TypeOf t Is Exception Then
						Throw CType(t, Exception)
					Else
						Throw New org.omg.CORBA.NO_IMPLEMENT
					End If
				Catch rex As Exception
					Throw rex
				Catch exr As Exception
					Throw New org.omg.CORBA.NO_IMPLEMENT
				End Try
			End Try
		End Function

		''' <summary>
		''' Returns a reference to the ORB associated with this object and
		''' its delegate.  This is the <code>ORB</code> object that created
		''' the delegate.
		''' </summary>
		''' <returns> the <code>ORB</code> instance that created the
		'''          <code>Delegate</code> object contained in this
		'''          <code>ObjectImpl</code> object </returns>
		Public Overridable Function _orb() As org.omg.CORBA.ORB
			Return _get_delegate().orb(Me)
		End Function


		''' <summary>
		''' Retrieves the <code>Policy</code> object for this
		''' <code>ObjectImpl</code> object that has the given
		''' policy type.
		''' </summary>
		''' <param name="policy_type"> an int indicating the policy type </param>
		''' <returns> the <code>Policy</code> object that is the specified policy type
		'''         and that applies to this <code>ObjectImpl</code> object </returns>
		''' <seealso cref= org.omg.CORBA.PolicyOperations#policy_type </seealso>
		Public Overridable Function _get_policy(ByVal policy_type As Integer) As org.omg.CORBA.Policy
			Return _get_delegate().get_policy(Me, policy_type)
		End Function

		''' <summary>
		''' Retrieves a list of the domain managers for this
		''' <code>ObjectImpl</code> object.
		''' </summary>
		''' <returns> an array containing the <code>DomainManager</code>
		'''         objects for this instance of <code>ObjectImpl</code> </returns>
		Public Overridable Function _get_domain_managers() As org.omg.CORBA.DomainManager()
			Return _get_delegate().get_domain_managers(Me)
		End Function

		''' <summary>
		''' Sets this <code>ObjectImpl</code> object's override type for
		''' the given policies to the given instance of
		''' <code>SetOverrideType</code>.
		''' </summary>
		''' <param name="policies"> an array of <code>Policy</code> objects with the
		'''         policies that will replace the current policies or be
		'''         added to the current policies </param>
		''' <param name="set_add"> either <code>SetOverrideType.SET_OVERRIDE</code>,
		'''         indicating that the given policies will replace any existing
		'''         ones, or <code>SetOverrideType.ADD_OVERRIDE</code>, indicating
		'''         that the given policies should be added to any existing ones </param>
		''' <returns> an <code>Object</code> with the given policies replacing or
		'''         added to its previous policies </returns>
		Public Overridable Function _set_policy_override(ByVal policies As org.omg.CORBA.Policy(), ByVal set_add As org.omg.CORBA.SetOverrideType) As org.omg.CORBA.Object
			Return _get_delegate().set_policy_override(Me, policies, set_add)
		End Function

		''' <summary>
		''' Checks whether this <code>ObjectImpl</code> object is implemented
		''' by a local servant.  If so, local invocation API's may be used.
		''' </summary>
		''' <returns> <code>true</code> if this object is implemented by a local
		'''         servant; <code>false</code> otherwise </returns>
		Public Overridable Function _is_local() As Boolean
			Return _get_delegate().is_local(Me)
		End Function

		''' <summary>
		''' Returns a Java reference to the local servant that should be used for sending
		''' a request for the method specified. If this <code>ObjectImpl</code>
		''' object is a local stub, it will invoke the <code>_servant_preinvoke</code>
		''' method before sending a request in order to obtain the
		''' <code>ServantObject</code> instance to use.
		''' <P>
		''' If a <code>ServantObject</code> object is returned, its <code>servant</code>
		''' field has been set to an object of the expected type (Note: the object may
		''' or may not be the actual servant instance). The local stub may cast
		''' the servant field to the expected type, and then invoke the operation
		''' directly. The <code>ServantRequest</code> object is valid for only one
		''' invocation and cannot be used for more than one invocation.
		''' </summary>
		''' <param name="operation"> a <code>String</code> containing the name of the method
		'''        to be invoked. This name should correspond to the method name as
		'''        it would be encoded in a GIOP request.
		''' </param>
		''' <param name="expectedType"> a <code>Class</code> object representing the
		'''        expected type of the servant that is returned. This expected
		'''        type is the <code>Class</code> object associated with the
		'''        operations class for the stub's interface. For example, a
		'''        stub for an interface <code>Foo</code> would pass the
		'''        <code>Class</code> object for the <code>FooOperations</code>
		'''        interface.
		''' </param>
		''' <returns> (1) a <code>ServantObject</code> object, which may or may
		'''         not be the actual servant instance, or (2) <code>null</code> if
		'''         (a) the servant is not local or (b) the servant has ceased to
		'''         be local due to a ForwardRequest from a POA ServantManager </returns>
		''' <exception cref="org.omg.CORBA.BAD_PARAM"> if the servant is not the expected type </exception>
		Public Overridable Function _servant_preinvoke(ByVal operation As String, ByVal expectedType As Type) As ServantObject
			Return _get_delegate().servant_preinvoke(Me, operation, expectedType)
		End Function

		''' <summary>
		''' Is called by the local stub after it has invoked an operation
		''' on the local servant that was previously retrieved from a
		''' call to the method <code>_servant_preinvoke</code>.
		''' The <code>_servant_postinvoke</code> method must be called
		''' if the <code>_servant_preinvoke</code>
		''' method returned a non-null value, even if an exception was thrown
		''' by the method invoked by the servant. For this reason, the call
		''' to the method <code>_servant_postinvoke</code> should be placed
		''' in a Java <code>finally</code> clause.
		''' </summary>
		''' <param name="servant"> the instance of the <code>ServantObject</code>
		'''        returned by the <code>_servant_preinvoke</code> method </param>
		Public Overridable Sub _servant_postinvoke(ByVal servant As ServantObject)
			_get_delegate().servant_postinvoke(Me, servant)
		End Sub

	'    
	'     * The following methods were added by orbos/98-04-03: Java to IDL
	'     * Mapping. These are used by RMI over IIOP.
	'     

		''' <summary>
		''' Returns an <code>OutputStream</code> object to use for marshalling
		''' the arguments of the given method.  This method is called by a stub,
		''' which must indicate if a response is expected, that is, whether or not
		''' the call is oneway.
		''' </summary>
		''' <param name="operation">         a String giving the name of the method. </param>
		''' <param name="responseExpected">  a boolean -- <code>true</code> if the
		'''         request is not one way, that is, a response is expected </param>
		''' <returns> an <code>OutputStream</code> object for dispatching the request </returns>
		Public Overridable Function _request(ByVal operation As String, ByVal responseExpected As Boolean) As OutputStream
			Return _get_delegate().request(Me, operation, responseExpected)
		End Function

		''' <summary>
		''' Invokes an operation and returns an <code>InputStream</code>
		''' object for reading the response. The stub provides the
		''' <code>OutputStream</code> object that was previously returned by a
		''' call to the <code>_request</code> method. The method specified
		''' as an argument to <code>_request</code> when it was
		''' called previously is the method that this method invokes.
		''' <P>
		''' If an exception occurs, the <code>_invoke</code> method may throw an
		''' <code>ApplicationException</code> object that contains an InputStream from
		''' which the user exception state may be unmarshalled.
		''' </summary>
		''' <param name="output">  an OutputStream object for dispatching the request </param>
		''' <returns> an <code>InputStream</code> object containing the marshalled
		'''         response to the method invoked </returns>
		''' <exception cref="ApplicationException"> if the invocation
		'''         meets application-defined exception </exception>
		''' <exception cref="RemarshalException"> if the invocation leads
		'''         to a remarshalling error </exception>
		''' <seealso cref= #_request </seealso>
		Public Overridable Function _invoke(ByVal output As OutputStream) As InputStream
			Return _get_delegate().invoke(Me, output)
		End Function

		''' <summary>
		''' Releases the given
		''' reply stream back to the ORB when unmarshalling has
		''' completed after a call to the method <code>_invoke</code>.
		''' Calling this method is optional for the stub.
		''' </summary>
		''' <param name="input">  the <code>InputStream</code> object that was returned
		'''        by the <code>_invoke</code> method or the
		'''        <code>ApplicationException.getInputStream</code> method;
		'''        may be <code>null</code>, in which case this method does
		'''        nothing </param>
		''' <seealso cref= #_invoke </seealso>
		Public Overridable Sub _releaseReply(ByVal input As InputStream)
			_get_delegate().releaseReply(Me, input)
		End Sub

		''' <summary>
		''' Returns a <code>String</code> object that represents this
		''' <code>ObjectImpl</code> object.
		''' </summary>
		''' <returns> the <code>String</code> representation of this object </returns>
		Public Overrides Function ToString() As String
			If __delegate IsNot Nothing Then
			   Return __delegate.ToString(Me)
			Else
			   Return Me.GetType().name & ": no delegate set"
			End If
		End Function

		''' <summary>
		''' Returns the hash code for this <code>ObjectImpl</code> object.
		''' </summary>
		''' <returns> the hash code for this object </returns>
		Public Overrides Function GetHashCode() As Integer
			If __delegate IsNot Nothing Then
			   Return __delegate.hashCode(Me)
			Else
				Return MyBase.GetHashCode()
			End If
		End Function

		''' <summary>
		''' Compares this <code>ObjectImpl</code> object with the given one
		''' for equality.
		''' </summary>
		''' <param name="obj"> the object with which to compare this object </param>
		''' <returns> <code>true</code> if the two objects are equal;
		'''        <code>false</code> otherwise </returns>
		Public Overrides Function Equals(ByVal obj As Object) As Boolean
			If __delegate IsNot Nothing Then
			   Return __delegate.Equals(Me, obj)
			Else
			   Return (Me Is obj)
			End If
		End Function
	End Class

End Namespace