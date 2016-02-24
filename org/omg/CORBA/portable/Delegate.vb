Imports System

'
' * Copyright (c) 1997, 2002, Oracle and/or its affiliates. All rights reserved.
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
	''' Specifies a portable API for ORB-vendor-specific
	''' implementation of the org.omg.CORBA.Object methods.
	''' 
	''' Each stub (proxy) contains a delegate
	''' object, to which all org.omg.CORBA.Object methods are forwarded.
	''' This allows a stub generated by one vendor's ORB to work with the delegate
	''' from another vendor's ORB.
	''' </summary>
	''' <seealso cref= org.omg.CORBA.Object
	''' @author OMG </seealso>

	Public MustInherit Class [Delegate]

		''' <summary>
		''' Return an InterfaceDef for the object reference provided. </summary>
		''' <param name="self"> The object reference whose InterfaceDef needs to be returned </param>
		''' <returns> the InterfaceDef </returns>
		Public MustOverride Function get_interface_def(ByVal self As org.omg.CORBA.Object) As org.omg.CORBA.Object

		''' <summary>
		''' Returns a duplicate of the object reference provided. </summary>
		''' <param name="obj"> The object reference whose duplicate needs to be returned </param>
		''' <returns> the duplicate object reference </returns>
		Public MustOverride Function duplicate(ByVal obj As org.omg.CORBA.Object) As org.omg.CORBA.Object

		''' <summary>
		''' Releases resources associated with the object reference provided. </summary>
		''' <param name="obj"> The object reference whose resources need to be released </param>
		Public MustOverride Sub release(ByVal obj As org.omg.CORBA.Object)

		''' <summary>
		''' Checks if the object reference is an instance of the given interface. </summary>
		''' <param name="obj"> The object reference to be checked. </param>
		''' <param name="repository_id"> The repository identifier of the interface
		''' to check against. </param>
		''' <returns> true if the object reference supports the interface </returns>
		Public MustOverride Function is_a(ByVal obj As org.omg.CORBA.Object, ByVal repository_id As String) As Boolean

		''' <summary>
		''' Determines whether the server object for the object reference has been
		''' destroyed. </summary>
		''' <param name="obj"> The object reference which delegated to this delegate. </param>
		''' <returns> true if the ORB knows authoritatively that the server object does
		''' not exist, false otherwise </returns>
		Public MustOverride Function non_existent(ByVal obj As org.omg.CORBA.Object) As Boolean

		''' <summary>
		''' Determines if the two object references are equivalent. </summary>
		''' <param name="obj"> The object reference which delegated to this delegate. </param>
		''' <param name="other"> The object reference to check equivalence against. </param>
		''' <returns> true if the objects are CORBA-equivalent. </returns>
		Public MustOverride Function is_equivalent(ByVal obj As org.omg.CORBA.Object, ByVal other As org.omg.CORBA.Object) As Boolean

		''' <summary>
		''' Returns an ORB-internal identifier (hashcode) for this object reference. </summary>
		''' <param name="obj"> The object reference which delegated to this delegate. </param>
		''' <param name="max"> specifies an upper bound on the hash value returned by
		'''            the ORB. </param>
		''' <returns> ORB-internal hash identifier for object reference </returns>
		Public MustOverride Function hash(ByVal obj As org.omg.CORBA.Object, ByVal max As Integer) As Integer

		''' <summary>
		''' Creates a Request instance for use in the Dynamic Invocation Interface. </summary>
		''' <param name="obj"> The object reference which delegated to this delegate. </param>
		''' <param name="operation"> The name of the operation to be invoked using the
		'''                  Request instance. </param>
		''' <returns> the created Request instance </returns>
		Public MustOverride Function request(ByVal obj As org.omg.CORBA.Object, ByVal operation As String) As org.omg.CORBA.Request

		''' <summary>
		''' Creates a Request instance for use in the Dynamic Invocation Interface.
		''' </summary>
		''' <param name="obj"> The object reference which delegated to this delegate. </param>
		''' <param name="ctx">                      The context to be used. </param>
		''' <param name="operation">                The name of the operation to be
		'''                                 invoked. </param>
		''' <param name="arg_list">         The arguments to the operation in the
		'''                                 form of an NVList. </param>
		''' <param name="result">           A container for the result as a NamedValue. </param>
		''' <returns>                 The created Request object.
		'''  </returns>
		Public MustOverride Function create_request(ByVal obj As org.omg.CORBA.Object, ByVal ctx As org.omg.CORBA.Context, ByVal operation As String, ByVal arg_list As org.omg.CORBA.NVList, ByVal result As org.omg.CORBA.NamedValue) As org.omg.CORBA.Request

		''' <summary>
		''' Creates a Request instance for use in the Dynamic Invocation Interface.
		''' </summary>
		''' <param name="obj"> The object reference which delegated to this delegate. </param>
		''' <param name="ctx">                      The context to be used. </param>
		''' <param name="operation">                The name of the operation to be
		'''                                 invoked. </param>
		''' <param name="arg_list">         The arguments to the operation in the
		'''                                 form of an NVList. </param>
		''' <param name="result">           A container for the result as a NamedValue. </param>
		''' <param name="exclist">          A list of possible exceptions the
		'''                                 operation can throw. </param>
		''' <param name="ctxlist">          A list of context strings that need
		'''                                 to be resolved and sent with the
		'''                                 Request. </param>
		''' <returns>                 The created Request object. </returns>
		Public MustOverride Function create_request(ByVal obj As org.omg.CORBA.Object, ByVal ctx As org.omg.CORBA.Context, ByVal operation As String, ByVal arg_list As org.omg.CORBA.NVList, ByVal result As org.omg.CORBA.NamedValue, ByVal exclist As org.omg.CORBA.ExceptionList, ByVal ctxlist As org.omg.CORBA.ContextList) As org.omg.CORBA.Request

		''' <summary>
		''' Provides a reference to the orb associated with its parameter.
		''' </summary>
		''' <param name="obj">  the object reference which delegated to this delegate. </param>
		''' <returns> the associated orb. </returns>
		''' <seealso cref= <a href="package-summary.html#unimpl"><code>portable</code>
		''' package comments for unimplemented features</a> </seealso>
		Public Overridable Function orb(ByVal obj As org.omg.CORBA.Object) As org.omg.CORBA.ORB
			Throw New org.omg.CORBA.NO_IMPLEMENT
		End Function

		''' <summary>
		''' Returns the <code>Policy</code> object of the specified type
		''' which applies to this object.
		''' </summary>
		''' <param name="self"> The object reference which delegated to this delegate. </param>
		''' <param name="policy_type"> The type of policy to be obtained. </param>
		''' <returns> A <code>Policy</code> object of the type specified by
		'''         the policy_type parameter. </returns>
		''' <exception cref="org.omg.CORBA.BAD_PARAM"> raised when the value of policy type
		''' is not valid either because the specified type is not supported by this
		''' ORB or because a policy object of that type is not associated with this
		''' Object. </exception>
		''' <seealso cref= <a href="package-summary.html#unimpl"><code>portable</code>
		''' package comments for unimplemented features</a> </seealso>
		Public Overridable Function get_policy(ByVal self As org.omg.CORBA.Object, ByVal policy_type As Integer) As org.omg.CORBA.Policy
			Throw New org.omg.CORBA.NO_IMPLEMENT
		End Function


		''' <summary>
		''' Retrieves the <code>DomainManagers</code> of this object.
		''' This allows administration services (and applications) to retrieve the
		''' domain managers, and hence the security and other policies applicable
		''' to individual objects that are members of the domain.
		''' </summary>
		''' <param name="self"> The object reference which delegated to this delegate. </param>
		''' <returns> The list of immediately enclosing domain managers of this object.
		'''  At least one domain manager is always returned in the list since by
		''' default each object is associated with at least one domain manager at
		''' creation. </returns>
		''' <seealso cref= <a href="package-summary.html#unimpl"><code>portable</code>
		''' package comments for unimplemented features</a> </seealso>
		Public Overridable Function get_domain_managers(ByVal self As org.omg.CORBA.Object) As org.omg.CORBA.DomainManager()
			Throw New org.omg.CORBA.NO_IMPLEMENT
		End Function


		''' <summary>
		''' Associates the policies passed in
		''' with a newly created object reference that it returns. Only certain
		''' policies that pertain to the invocation of an operation at the client
		''' end can be overridden using this operation. Attempts to override any
		''' other policy will result in the raising of the CORBA::NO_PERMISSION
		''' exception.
		''' </summary>
		''' <param name="self"> The object reference which delegated to this delegate. </param>
		''' <param name="policies"> A sequence of references to Policy objects. </param>
		''' <param name="set_add"> Indicates whether these policies should be added
		''' onto any otheroverrides that already exist (ADD_OVERRIDE) in
		''' the object reference, or they should be added to a clean
		''' override free object reference (SET_OVERRIDE). </param>
		''' <returns>  A new object reference with the new policies associated with it.
		''' </returns>
		''' <seealso cref= <a href="package-summary.html#unimpl"><code>portable</code>
		''' package comments for unimplemented features</a> </seealso>
		Public Overridable Function set_policy_override(ByVal self As org.omg.CORBA.Object, ByVal policies As org.omg.CORBA.Policy(), ByVal set_add As org.omg.CORBA.SetOverrideType) As org.omg.CORBA.Object
			Throw New org.omg.CORBA.NO_IMPLEMENT
		End Function


		''' <summary>
		''' Returns true if this object is implemented by a local servant.
		''' </summary>
		''' <param name="self"> The object reference which delegated to this delegate. </param>
		''' <returns> true only if the servant incarnating this object is located in
		''' this Java VM. Return false if the servant is not local or the ORB
		''' does not support local stubs for this particular servant. The default
		''' behavior of is_local() is to return false. </returns>
		Public Overridable Function is_local(ByVal self As org.omg.CORBA.Object) As Boolean
			Return False
		End Function

		''' <summary>
		''' Returns a Java reference to the servant which should be used for this
		''' request. servant_preinvoke() is invoked by a local stub.
		''' If a ServantObject object is returned, then its servant field
		''' has been set to an object of the expected type (Note: the object may
		''' or may not be the actual servant instance). The local stub may cast
		''' the servant field to the expected type, and then invoke the operation
		''' directly. The ServantRequest object is valid for only one invocation,
		''' and cannot be used for more than one invocation.
		''' </summary>
		''' <param name="self"> The object reference which delegated to this delegate.
		''' </param>
		''' <param name="operation"> a string containing the operation name.
		''' The operation name corresponds to the operation name as it would be
		''' encoded in a GIOP request.
		''' </param>
		''' <param name="expectedType"> a Class object representing the expected type of the servant.
		''' The expected type is the Class object associated with the operations
		''' class of the stub's interface (e.g. A stub for an interface Foo,
		''' would pass the Class object for the FooOperations interface).
		''' </param>
		''' <returns> a ServantObject object.
		''' The method may return a null value if it does not wish to support
		''' this optimization (e.g. due to security, transactions, etc).
		''' The method must return null if the servant is not of the expected type. </returns>
		Public Overridable Function servant_preinvoke(ByVal self As org.omg.CORBA.Object, ByVal operation As String, ByVal expectedType As Type) As ServantObject
			Return Nothing
		End Function

		''' <summary>
		''' servant_postinvoke() is invoked by the local stub after the operation
		''' has been invoked on the local servant.
		''' This method must be called if servant_preinvoke() returned a non-null
		''' value, even if an exception was thrown by the servant's method.
		''' For this reason, the call to servant_postinvoke() should be placed
		''' in a Java finally clause.
		''' </summary>
		''' <param name="self"> The object reference which delegated to this delegate.
		''' </param>
		''' <param name="servant"> the instance of the ServantObject returned from
		'''  the servant_preinvoke() method. </param>
		Public Overridable Sub servant_postinvoke(ByVal self As org.omg.CORBA.Object, ByVal servant As ServantObject)
		End Sub

		''' <summary>
		''' request is called by a stub to obtain an OutputStream for
		''' marshaling arguments. The stub must supply the operation name,
		''' and indicate if a response is expected (i.e is this a oneway
		''' call).
		''' </summary>
		''' <param name="self"> The object reference which delegated to this delegate. </param>
		''' <param name="operation"> a string containing the operation name.
		''' The operation name corresponds to the operation name as it would be
		''' encoded in a GIOP request. </param>
		''' <param name="responseExpected"> false if the operation is a one way operation,
		''' and true otherwise. </param>
		''' <returns> OutputStream the OutputStream into which request arguments
		''' can be marshaled. </returns>
		''' <seealso cref= <a href="package-summary.html#unimpl"><code>portable</code>
		''' package comments for unimplemented features</a> </seealso>
		Public Overridable Function request(ByVal self As org.omg.CORBA.Object, ByVal operation As String, ByVal responseExpected As Boolean) As OutputStream
			Throw New org.omg.CORBA.NO_IMPLEMENT
		End Function

		''' <summary>
		''' invoke is called by a stub to invoke an operation. The stub provides an
		''' OutputStream that was previously returned by a request()
		''' call. invoke returns an InputStream which contains the
		''' marshaled reply. If an exception occurs, invoke may throw an
		''' ApplicationException object which contains an InputStream from
		''' which the user exception state may be unmarshaled.
		''' </summary>
		''' <param name="self"> The object reference which delegated to this delegate. </param>
		''' <param name="output"> the OutputStream which contains marshaled arguments </param>
		''' <returns> input the InputStream from which reply parameters can be
		''' unmarshaled. </returns>
		''' <exception cref="ApplicationException"> thrown when implementation throws
		''' (upon invocation) an exception defined as part of its remote method
		''' definition. </exception>
		''' <exception cref="RemarshalException"> thrown when remarshalling fails. </exception>
		''' <seealso cref= <a href="package-summary.html#unimpl"><code>portable</code>
		''' package comments for unimplemented features</a> </seealso>
		Public Overridable Function invoke(ByVal self As org.omg.CORBA.Object, ByVal output As OutputStream) As InputStream
			Throw New org.omg.CORBA.NO_IMPLEMENT
		End Function

		''' <summary>
		''' releaseReply may optionally be called by a stub to release a
		''' reply stream back to the ORB when the unmarshaling has
		''' completed. The stub passes the InputStream returned by
		''' invoke() or ApplicationException.getInputStream(). A null
		''' value may also be passed to releaseReply, in which case the
		''' method is a noop.
		''' </summary>
		''' <param name="self"> The object reference which delegated to this delegate. </param>
		''' <param name="input"> the InputStream returned from invoke(). </param>
		''' <seealso cref= <a href="package-summary.html#unimpl"><code>portable</code>
		''' package comments for unimplemented features</a> </seealso>
		Public Overridable Sub releaseReply(ByVal self As org.omg.CORBA.Object, ByVal input As InputStream)
			Throw New org.omg.CORBA.NO_IMPLEMENT
		End Sub

		''' <summary>
		''' Provides the implementation to override the toString() method
		''' of the delegating CORBA object.
		''' </summary>
		''' <param name="self"> the object reference that delegated to this delegate </param>
		''' <returns> a <code>String</code> object that represents the object
		'''         reference that delegated to this <code>Delegate</code>
		'''         object </returns>

		Public Overrides Function ToString(ByVal self As org.omg.CORBA.Object) As String
			Return self.GetType().name & ":" & Me.ToString()
		End Function

		''' <summary>
		''' Provides the implementation to override the hashCode() method
		''' of the delegating CORBA object.
		''' </summary>
		''' <param name="self"> the object reference that delegated to this delegate </param>
		''' <returns> an <code>int</code> that represents the hashcode for the
		'''         object reference that delegated to this <code>Delegate</code>
		'''         object </returns>
		Public Overrides Function GetHashCode(ByVal self As org.omg.CORBA.Object) As Integer
			Return System.identityHashCode(self)
		End Function

		''' <summary>
		''' Provides the implementation to override the equals(java.lang.Object obj)
		''' method of the delegating CORBA object.
		''' </summary>
		''' <param name="self"> the object reference that delegated to this delegate </param>
		''' <param name="obj"> the <code>Object</code> with which to compare </param>
		''' <returns> <code>true</code> if <code>obj</code> equals <code>self</code>;
		'''         <code>false</code> otherwise </returns>
		Public Overrides Function Equals(ByVal self As org.omg.CORBA.Object, ByVal obj As Object) As Boolean
			Return (self Is obj)
		End Function
	End Class

End Namespace