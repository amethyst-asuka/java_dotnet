'
' * Copyright (c) 1995, 1999, Oracle and/or its affiliates. All rights reserved.
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
	''' The definition for a CORBA object reference.
	''' <p>
	''' A CORBA object reference is a handle for a particular
	''' CORBA object implemented by a server. A CORBA object reference
	''' identifies the same CORBA object each time the reference is used to invoke
	''' a method on the object.
	''' A CORBA object may have multiple, distinct object references.
	''' <p>
	''' The <code>org.omg.CORBA.Object</code> interface is the root of
	''' the inheritance hierarchy for all CORBA object references in the Java
	''' programming language, analogous to <code>java.rmi.Remote</code>
	''' for RMI remote objects.
	''' <p>
	''' A CORBA object may be either local or remote.
	''' If it is a local object (that is, running in the same
	''' VM as the client), invocations may be directly serviced by
	''' the object instance, and the object reference could point to the actual
	''' instance of the object implementation class.
	''' If a CORBA object is a remote object (that is, running in a different
	''' VM from the client), the object reference points to a stub (proxy) which uses the
	''' ORB machinery to make a remote invocation on the server where the object
	''' implementation resides.
	''' <p>
	''' Default implementations of the methods in the interface
	''' <code>org.omg.CORBA.Object</code>
	''' are provided in the class <code>org.omg.CORBA.portable.ObjectImpl</code>,
	''' which is the base class for stubs and object implementations.
	''' <p> </summary>
	''' <seealso cref= org.omg.CORBA.portable.ObjectImpl </seealso>

	Public Interface [Object]

		''' <summary>
		''' Checks whether this object is an instance of a class that
		''' implements the given interface.
		''' </summary>
		''' <param name="repositoryIdentifier"> the interface to check against </param>
		''' <returns> <code>true</code> if this object reference is an instance
		'''         of a class that implements the interface;
		'''         <code>false</code> otherwise </returns>
		Function _is_a(ByVal repositoryIdentifier As String) As Boolean


		''' <summary>
		''' Determines whether the two object references are equivalent,
		''' so far as the ORB can easily determine. Two object references are equivalent
		''' if they are identical. Two distinct object references which in fact refer to
		''' the same object are also equivalent. However, ORBs are not required
		''' to attempt determination of whether two distinct object references
		''' refer to the same object, since such determination could be impractically
		''' expensive. </summary>
		''' <param name="other"> the other object reference with which to check for equivalence </param>
		''' <returns> <code>true</code> if this object reference is known to be
		'''         equivalent to the given object reference.
		'''         Note that <code>false</code> indicates only that the two
		'''         object references are distinct, not necessarily that
		'''         they reference distinct objects. </returns>
		Function _is_equivalent(ByVal other As org.omg.CORBA.Object) As Boolean


		''' <summary>
		''' Determines whether the server object for this object reference has been
		''' destroyed. </summary>
		''' <returns> <code>true</code> if the ORB knows authoritatively that the
		'''         server object does not exist; <code>false</code> otherwise </returns>
		Function _non_existent() As Boolean


		''' <summary>
		''' Returns an ORB-internal identifier for this object reference.
		''' This is a hash identifier, which does
		''' not change during the lifetime of the object reference, and so
		''' neither will any hash function of that identifier change. The value returned
		''' is not guaranteed to be unique; in other words, another object
		''' reference may have the same hash value.
		''' If two object references hash differently,
		''' then they are distinct object references; however, both may still refer
		''' to the same CORBA object.
		''' </summary>
		''' <param name="maximum"> the upper bound on the hash value returned by the ORB </param>
		''' <returns> the ORB-internal hash identifier for this object reference </returns>
		Function _hash(ByVal maximum As Integer) As Integer


		''' <summary>
		''' Returns a duplicate of this CORBA object reference.
		''' The server object implementation is not involved in creating
		''' the duplicate, and the implementation cannot distinguish whether
		''' the original object reference or a duplicate was used to make a request.
		''' <P>
		''' Note that this method is not very useful in the Java platform,
		''' since memory management is handled by the VM.
		''' It is included for compliance with the CORBA APIs.
		''' <P>
		''' The method <code>_duplicate</code> may return this object reference itself.
		''' </summary>
		''' <returns> a duplicate of this object reference or this object reference
		'''         itself </returns>
		Function _duplicate() As org.omg.CORBA.Object


		''' <summary>
		''' Signals that the caller is done using this object reference, so
		''' internal ORB resources associated with this object reference can be
		''' released. Note that the object implementation is not involved in
		''' this operation, and other references to the same object are not affected.
		''' </summary>
		Sub _release()


		''' <summary>
		''' Obtains an <code>InterfaceDef</code> for the object implementation
		''' referenced by this object reference.
		''' The <code>InterfaceDef</code> object
		''' may be used to introspect on the methods, attributes, and other
		''' type information for the object referred to by this object reference.
		''' </summary>
		''' <returns> the <code>InterfaceDef</code> object in the Interface Repository
		'''         which provides type information about the object referred to by
		'''         this object reference </returns>
		Function _get_interface_def() As org.omg.CORBA.Object



		''' <summary>
		''' Creates a <code>Request</code> instance for use in the
		''' Dynamic Invocation Interface.
		''' </summary>
		''' <param name="operation">  the name of the method to be invoked using the
		'''                        <code>Request</code> instance </param>
		''' <returns> the newly-created <code>Request</code> instance </returns>
		Function _request(ByVal operation As String) As Request



		''' <summary>
		''' Creates a <code>Request</code> instance initialized with the
		''' given context, method name, list of arguments, and container
		''' for the method's return value.
		''' </summary>
		''' <param name="ctx">                       a <code>Context</code> object containing
		'''                     a list of properties </param>
		''' <param name="operation">    the name of the method to be invoked </param>
		''' <param name="arg_list">          an <code>NVList</code> containing the actual arguments
		'''                     to the method being invoked </param>
		''' <param name="result">            a <code>NamedValue</code> object to serve as a
		'''                     container for the method's return value </param>
		''' <returns>                  the newly-created <code>Request</code> object
		''' </returns>
		''' <seealso cref= Request </seealso>
		''' <seealso cref= NVList </seealso>
		''' <seealso cref= NamedValue </seealso>

		Function _create_request(ByVal ctx As Context, ByVal operation As String, ByVal arg_list As NVList, ByVal result As NamedValue) As Request

		''' <summary>
		''' Creates a <code>Request</code> instance initialized with the
		''' given context, method name, list of arguments, container
		''' for the method's return value, list of possible exceptions,
		''' and list of context strings needing to be resolved.
		''' </summary>
		''' <param name="ctx">                       a <code>Context</code> object containing
		'''                     a list of properties </param>
		''' <param name="operation">    the name of the method to be invoked </param>
		''' <param name="arg_list">          an <code>NVList</code> containing the actual arguments
		'''                     to the method being invoked </param>
		''' <param name="result">            a <code>NamedValue</code> object to serve as a
		'''                     container for the method's return value </param>
		''' <param name="exclist">           an <code>ExceptionList</code> object containing a
		'''                     list of possible exceptions the method can throw </param>
		''' <param name="ctxlist">           a <code>ContextList</code> object containing a list of
		'''                     context strings that need to be resolved and sent with the
		'''                          <code>Request</code> instance </param>
		''' <returns>                  the newly-created <code>Request</code> object
		''' </returns>
		''' <seealso cref= Request </seealso>
		''' <seealso cref= NVList </seealso>
		''' <seealso cref= NamedValue </seealso>
		''' <seealso cref= ExceptionList </seealso>
		''' <seealso cref= ContextList </seealso>

		Function _create_request(ByVal ctx As Context, ByVal operation As String, ByVal arg_list As NVList, ByVal result As NamedValue, ByVal exclist As ExceptionList, ByVal ctxlist As ContextList) As Request




		''' <summary>
		''' Returns the <code>Policy</code> object of the specified type
		''' which applies to this object.
		''' </summary>
		''' <param name="policy_type"> the type of policy to be obtained </param>
		''' <returns> A <code>Policy</code> object of the type specified by
		'''         the policy_type parameter </returns>
		''' <exception cref="org.omg.CORBA.BAD_PARAM"> when the value of policy type
		''' is not valid either because the specified type is not supported by this
		''' ORB or because a policy object of that type is not associated with this
		''' Object </exception>
		Function _get_policy(ByVal policy_type As Integer) As Policy


		''' <summary>
		''' Retrieves the <code>DomainManagers</code> of this object.
		''' This allows administration services (and applications) to retrieve the
		''' domain managers, and hence the security and other policies applicable
		''' to individual objects that are members of the domain.
		''' </summary>
		''' <returns> the list of immediately enclosing domain managers of this object.
		'''  At least one domain manager is always returned in the list since by
		''' default each object is associated with at least one domain manager at
		''' creation. </returns>
		Function _get_domain_managers() As DomainManager()


		''' <summary>
		''' Returns a new <code>Object</code> with the given policies
		''' either replacing any existing policies in this
		''' <code>Object</code> or with the given policies added
		''' to the existing ones, depending on the value of the
		''' given <code>SetOverrideType</code> object.
		''' </summary>
		''' <param name="policies"> an array of <code>Policy</code> objects containing
		'''                 the policies to be added or to be used as replacements </param>
		''' <param name="set_add"> either <code>SetOverrideType.SET_OVERRIDE</code>, indicating
		'''                that the given policies will replace any existing ones, or
		'''                <code>SetOverrideType.ADD_OVERRIDE</code>, indicating that
		'''                the given policies should be added to any existing ones </param>
		''' <returns> a new <code>Object</code> with the given policies replacing
		'''         or added to those in this <code>Object</code> </returns>
		Function _set_policy_override(ByVal policies As Policy(), ByVal set_add As SetOverrideType) As org.omg.CORBA.Object


	End Interface

End Namespace