Imports System
Imports org.omg.CORBA.portable

'
' * Copyright (c) 2000, 2001, Oracle and/or its affiliates. All rights reserved.
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
	''' <P>Used as a base class for implementation of a local IDL interface in the
	''' Java language mapping.  It is a class which implements all the operations
	''' in the <tt>org.omg.CORBA.Object</tt> interface.
	''' <P>Local interfaces are implemented by using CORBA::LocalObject
	'''  to provide implementations of <code>Object</code> pseudo
	'''  operations and any other ORB-specific support mechanisms that are
	'''  appropriate for such objects.  Object implementation techniques are
	'''  inherently language-mapping specific.  Therefore, the
	'''  <code>LocalObject</code> type is not defined in IDL, but is specified
	'''  in each language mapping.
	'''  <P>Methods that do not apply to local objects throw
	'''  an <code>org.omg.CORBA.NO_IMPLEMENT</code> exception with the message,
	'''  "This is a locally contrained object."  Attempting to use a
	'''  <TT>LocalObject</TT> to create a DII request results in NO_IMPLEMENT
	'''  system exception.  Attempting to marshal or stringify a
	'''  <TT>LocalObject</TT> results in a MARSHAL system exception.  Narrowing
	'''  and widening references to <TT>LocalObjects</TT> must work as for regular
	'''  object references.
	'''  <P><code>LocalObject</code> is to be used as the base class of locally
	'''  constrained objects, such as those in the PortableServer module.
	'''  The specification here is based on the CORBA Components
	'''  Volume I - orbos/99-07-01<P> </summary>
	''' <seealso cref= <a href="package-summary.html#unimpl"><code>CORBA</code> package
	'''      comments for unimplemented features</a> </seealso>

	Public Class LocalObject
		Implements org.omg.CORBA.Object

		Private Shared reason As String = "This is a locally constrained object."

		''' <summary>
		''' Constructs a default <code>LocalObject</code> instance.
		''' </summary>
		Public Sub New()
		End Sub

		''' <summary>
		''' <P>Determines whether the two object references are equivalent,
		''' so far as the ORB can easily determine. Two object references are equivalent
		''' if they are identical. Two distinct object references which in fact refer to
		''' the same object are also equivalent. However, ORBs are not required
		''' to attempt determination of whether two distinct object references
		''' refer to the same object, since such determination could be impractically
		''' expensive.
		''' <P>Default implementation of the org.omg.CORBA.Object method. <P>
		''' </summary>
		''' <param name="that"> the object reference with which to check for equivalence </param>
		''' <returns> <code>true</code> if this object reference is known to be
		'''         equivalent to the given object reference.
		'''         Note that <code>false</code> indicates only that the two
		'''         object references are distinct, not necessarily that
		'''         they reference distinct objects. </returns>
		Public Overridable Function _is_equivalent(ByVal that As org.omg.CORBA.Object) As Boolean
			Return Equals(that)
		End Function

		''' <summary>
		''' Always returns <code>false</code>.
		''' This method is the default implementation of the
		''' <code>org.omg.CORBA.Object</code> method.<P>
		''' </summary>
		''' <returns> <code>false</code> </returns>
		Public Overridable Function _non_existent() As Boolean
			Return False
		End Function

		''' <summary>
		''' Returns a hash value that is consistent for the
		''' lifetime of the object, using the given number as the maximum.
		''' This method is the default implementation of the
		''' <code>org.omg.CORBA.Object</code> method.<P> </summary>
		''' <param name="maximum"> an <code>int</code> identifying maximum value of
		'''                  the hashcode </param>
		''' <returns> this instance's hashcode </returns>
		Public Overridable Function _hash(ByVal maximum As Integer) As Integer
			Return GetHashCode()
		End Function

		''' <summary>
		''' Throws an <code>org.omg.CORBA.NO_IMPLEMENT</code> exception with
		''' the message "This is a locally constrained object."  This method
		''' does not apply to local objects and is therefore not implemented.
		''' This method is the default implementation of the
		''' <code>org.omg.CORBA.Object</code> method.<P>
		''' </summary>
		''' <param name="repository_id"> a <code>String</code> </param>
		''' <returns> NO_IMPLEMENT because this is a locally constrained object
		'''      and this method does not apply to local objects </returns>
		''' <exception cref="NO_IMPLEMENT"> because this is a locally constrained object
		'''      and this method does not apply to local objects </exception>
		''' <seealso cref= <a href="package-summary.html#unimpl"><code>CORBA</code> package
		'''      comments for unimplemented features</a> </seealso>
		Public Overridable Function _is_a(ByVal repository_id As String) As Boolean
			Throw New org.omg.CORBA.NO_IMPLEMENT(reason)
		End Function

		''' <summary>
		''' Throws an <code>org.omg.CORBA.NO_IMPLEMENT</code> exception with
		''' the message "This is a locally constrained object."
		''' This method is the default implementation of the
		''' <code>org.omg.CORBA.Object</code> method.<P> </summary>
		''' <returns> a duplicate of this <code>LocalObject</code> instance. </returns>
		''' <exception cref="NO_IMPLEMENT"> </exception>
		''' <seealso cref= <a href="package-summary.html#unimpl"><code>CORBA</code> package
		'''      comments for unimplemented features</a> </seealso>
		Public Overridable Function _duplicate() As org.omg.CORBA.Object
			Throw New org.omg.CORBA.NO_IMPLEMENT(reason)
		End Function

		''' <summary>
		''' Throws an <code>org.omg.CORBA.NO_IMPLEMENT</code> exception with
		''' the message "This is a locally constrained object."
		''' This method is the default implementation of the
		''' <code>org.omg.CORBA.Object</code> method.<P> </summary>
		''' <exception cref="NO_IMPLEMENT"> </exception>
		''' <seealso cref= <a href="package-summary.html#unimpl"><code>CORBA</code> package
		'''      comments for unimplemented features</a> </seealso>
		Public Overridable Sub _release()
			Throw New org.omg.CORBA.NO_IMPLEMENT(reason)
		End Sub

		''' <summary>
		''' Throws an <code>org.omg.CORBA.NO_IMPLEMENT</code> exception with
		''' the message "This is a locally constrained object."
		''' This method is the default implementation of the
		''' <code>org.omg.CORBA.Object</code> method.<P>
		''' </summary>
		''' <param name="operation"> a <code>String</code> giving the name of an operation
		'''        to be performed by the request that is returned </param>
		''' <returns> a <code>Request</code> object with the given operation </returns>
		''' <exception cref="NO_IMPLEMENT"> </exception>
		''' <seealso cref= <a href="package-summary.html#unimpl"><code>CORBA</code> package
		'''      comments for unimplemented features</a> </seealso>
		Public Overridable Function _request(ByVal operation As String) As Request
			Throw New org.omg.CORBA.NO_IMPLEMENT(reason)
		End Function

		''' <summary>
		''' Throws an <code>org.omg.CORBA.NO_IMPLEMENT</code> exception with
		''' the message "This is a locally constrained object."
		''' This method is the default implementation of the
		''' <code>org.omg.CORBA.Object</code> method.<P>
		''' </summary>
		''' <param name="ctx">          a <code>Context</code> object containing
		'''                     a list of properties </param>
		''' <param name="operation">    the <code>String</code> representing the name of the
		'''                     method to be invoked </param>
		''' <param name="arg_list">     an <code>NVList</code> containing the actual arguments
		'''                     to the method being invoked </param>
		''' <param name="result">       a <code>NamedValue</code> object to serve as a
		'''                     container for the method's return value </param>
		''' <returns> a new <code>Request</code> object initialized with the given
		''' arguments </returns>
		''' <exception cref="NO_IMPLEMENT"> </exception>
		''' <seealso cref= <a href="package-summary.html#unimpl"><code>CORBA</code> package
		'''      comments for unimplemented features</a> </seealso>
		Public Overridable Function _create_request(ByVal ctx As Context, ByVal operation As String, ByVal arg_list As NVList, ByVal result As NamedValue) As Request
			Throw New org.omg.CORBA.NO_IMPLEMENT(reason)
		End Function

		''' <summary>
		''' Throws an <code>org.omg.CORBA.NO_IMPLEMENT</code> exception with
		''' the message "This is a locally constrained object."
		''' This method is the default implementation of the
		''' <code>org.omg.CORBA.Object</code> method.<P>
		''' </summary>
		''' <param name="ctx">          a <code>Context</code> object containing
		'''                     a list of properties </param>
		''' <param name="operation">    the name of the method to be invoked </param>
		''' <param name="arg_list">     an <code>NVList</code> containing the actual arguments
		'''                     to the method being invoked </param>
		''' <param name="result">       a <code>NamedValue</code> object to serve as a
		'''                     container for the method's return value </param>
		''' <param name="exceptions">   an <code>ExceptionList</code> object containing a
		'''                     list of possible exceptions the method can throw </param>
		''' <param name="contexts">     a <code>ContextList</code> object containing a list of
		'''                     context strings that need to be resolved and sent
		'''                     with the
		'''                     <code>Request</code> instance </param>
		''' <returns> the new <code>Request</code> object initialized with the given
		''' arguments </returns>
		''' <exception cref="NO_IMPLEMENT"> </exception>
		''' <seealso cref= <a href="package-summary.html#unimpl"><code>CORBA</code> package
		'''      comments for unimplemented features</a> </seealso>
		Public Overridable Function _create_request(ByVal ctx As Context, ByVal operation As String, ByVal arg_list As NVList, ByVal result As NamedValue, ByVal exceptions As ExceptionList, ByVal contexts As ContextList) As Request
			Throw New org.omg.CORBA.NO_IMPLEMENT(reason)
		End Function

		''' <summary>
		''' Throws an <code>org.omg.CORBA.NO_IMPLEMENT</code> exception with
		''' the message "This is a locally constrained object." This method
		''' does not apply to local objects and is therefore not implemented.
		''' This method is the default implementation of the
		''' <code>org.omg.CORBA.Object</code> method.<P> </summary>
		''' <returns> NO_IMPLEMENT because this is a locally constrained object
		'''      and this method does not apply to local objects </returns>
		''' <exception cref="NO_IMPLEMENT"> because this is a locally constrained object
		'''      and this method does not apply to local objects </exception>
		''' <seealso cref= <a href="package-summary.html#unimpl"><code>CORBA</code> package
		'''      comments for unimplemented features</a> </seealso>
		Public Overridable Function _get_interface() As org.omg.CORBA.Object
			Throw New org.omg.CORBA.NO_IMPLEMENT(reason)
		End Function

		''' <summary>
		''' Throws an <code>org.omg.CORBA.NO_IMPLEMENT</code> exception with
		''' the message "This is a locally constrained object."
		''' This method is the default implementation of the
		''' <code>org.omg.CORBA.Object</code> method.<P> </summary>
		''' <exception cref="NO_IMPLEMENT"> </exception>
		''' <seealso cref= <a href="package-summary.html#unimpl"><code>CORBA</code> package
		'''      comments for unimplemented features</a> </seealso>
		Public Overridable Function _get_interface_def() As org.omg.CORBA.Object
			' First try to call the delegate implementation class's
			' "Object get_interface_def(..)" method (will work for JDK1.2
			' ORBs).
			' Else call the delegate implementation class's
			' "InterfaceDef get_interface(..)" method using reflection
			' (will work for pre-JDK1.2 ORBs).

			Throw New org.omg.CORBA.NO_IMPLEMENT(reason)
		End Function

		''' <summary>
		''' Throws an <code>org.omg.CORBA.NO_IMPLEMENT</code> exception with
		''' the message "This is a locally constrained object."
		''' This method is the default implementation of the
		''' <code>org.omg.CORBA.Object</code> method.<P> </summary>
		''' <returns> the ORB instance that created the Delegate contained in this
		''' <code>ObjectImpl</code> </returns>
		''' <exception cref="NO_IMPLEMENT"> </exception>
		''' <seealso cref= <a href="package-summary.html#unimpl"><code>CORBA</code> package
		'''      comments for unimplemented features</a> </seealso>
		Public Overridable Function _orb() As org.omg.CORBA.ORB
			Throw New org.omg.CORBA.NO_IMPLEMENT(reason)
		End Function

		''' <summary>
		''' Throws an <code>org.omg.CORBA.NO_IMPLEMENT</code> exception with
		''' the message "This is a locally constrained object." This method
		''' does not apply to local objects and is therefore not implemented.
		''' This method is the default implementation of the
		''' <code>org.omg.CORBA.Object</code> method.<P> </summary>
		''' <param name="policy_type">  an <code>int</code> </param>
		''' <returns> NO_IMPLEMENT because this is a locally constrained object
		'''      and this method does not apply to local objects </returns>
		''' <exception cref="NO_IMPLEMENT"> because this is a locally constrained object
		'''      and this method does not apply to local objects </exception>
		''' <seealso cref= <a href="package-summary.html#unimpl"><code>CORBA</code> package
		'''      comments for unimplemented features</a> </seealso>
		Public Overridable Function _get_policy(ByVal policy_type As Integer) As org.omg.CORBA.Policy
			Throw New org.omg.CORBA.NO_IMPLEMENT(reason)
		End Function


		''' <summary>
		''' Throws an <code>org.omg.CORBA.NO_IMPLEMENT</code> exception with
		''' the message "This is a locally constrained object." This method
		''' does not apply to local objects and is therefore not implemented.
		''' This method is the default implementation of the
		''' <code>org.omg.CORBA.Object</code> method.<P> </summary>
		''' <exception cref="NO_IMPLEMENT"> </exception>
		''' <seealso cref= <a href="package-summary.html#unimpl"><code>CORBA</code> package
		'''      comments for unimplemented features</a> </seealso>
		Public Overridable Function _get_domain_managers() As org.omg.CORBA.DomainManager()
			Throw New org.omg.CORBA.NO_IMPLEMENT(reason)
		End Function

		''' <summary>
		''' Throws an <code>org.omg.CORBA.NO_IMPLEMENT</code> exception with
		''' the message "This is a locally constrained object." This method
		''' does not apply to local objects and is therefore not implemented.
		''' This method is the default implementation of the
		''' <code>org.omg.CORBA.Object</code> method.
		''' </summary>
		''' <param name="policies"> an array </param>
		''' <param name="set_add"> a flag </param>
		''' <returns> NO_IMPLEMENT because this is a locally constrained object
		'''      and this method does not apply to local objects </returns>
		''' <exception cref="NO_IMPLEMENT"> because this is a locally constrained object
		'''      and this method does not apply to local objects </exception>
		''' <seealso cref= <a href="package-summary.html#unimpl"><code>CORBA</code> package
		'''      comments for unimplemented features</a> </seealso>
		Public Overridable Function _set_policy_override(ByVal policies As org.omg.CORBA.Policy(), ByVal set_add As org.omg.CORBA.SetOverrideType) As org.omg.CORBA.Object
			Throw New org.omg.CORBA.NO_IMPLEMENT(reason)
		End Function


		''' <summary>
		''' Throws an <code>org.omg.CORBA.NO_IMPLEMENT</code> exception with
		''' the message "This is a locally constrained object."
		''' This method is the default implementation of the
		''' <code>org.omg.CORBA.Object</code> method.<P>
		''' Returns <code>true</code> for this <code>LocalObject</code> instance.<P> </summary>
		''' <returns> <code>true</code> always </returns>
		''' <exception cref="NO_IMPLEMENT"> </exception>
		''' <seealso cref= <a href="package-summary.html#unimpl"><code>CORBA</code> package
		'''      comments for unimplemented features</a> </seealso>
		Public Overridable Function _is_local() As Boolean
			Throw New org.omg.CORBA.NO_IMPLEMENT(reason)
		End Function

		''' <summary>
		''' Throws an <code>org.omg.CORBA.NO_IMPLEMENT</code> exception with
		''' the message "This is a locally constrained object."
		''' This method is the default implementation of the
		''' <code>org.omg.CORBA.Object</code> method.<P> </summary>
		''' <param name="operation"> a <code>String</code> indicating which operation
		'''                  to preinvoke </param>
		''' <param name="expectedType"> the class of the type of operation mentioned above </param>
		''' <returns> NO_IMPLEMENT because this is a locally constrained object
		'''      and this method does not apply to local objects </returns>
		''' <exception cref="NO_IMPLEMENT"> because this is a locally constrained object
		'''      and this method does not apply to local object </exception>
		''' <seealso cref= <a href="package-summary.html#unimpl"><code>CORBA</code> package
		'''      comments for unimplemented features</a> </seealso>
		Public Overridable Function _servant_preinvoke(ByVal operation As String, ByVal expectedType As Type) As ServantObject
			Throw New org.omg.CORBA.NO_IMPLEMENT(reason)
		End Function

		''' <summary>
		''' Throws an <code>org.omg.CORBA.NO_IMPLEMENT</code> exception with
		''' the message "This is a locally constrained object."
		''' This method is the default implementation of the
		''' <code>org.omg.CORBA.Object</code> method.<P> </summary>
		''' <param name="servant"> the servant object on which to post-invoke </param>
		''' <exception cref="NO_IMPLEMENT"> </exception>
		''' <seealso cref= <a href="package-summary.html#unimpl"><code>CORBA</code> package
		'''      comments for unimplemented features</a> </seealso>
		Public Overridable Sub _servant_postinvoke(ByVal servant As ServantObject)
			Throw New org.omg.CORBA.NO_IMPLEMENT(reason)
		End Sub

	'    
	'     * The following methods were added by orbos/98-04-03: Java to IDL
	'     * Mapping. These are used by RMI over IIOP.
	'     

		''' <summary>
		''' Throws an <code>org.omg.CORBA.NO_IMPLEMENT</code> exception with
		''' the message "This is a locally constrained object."
		''' This method is the default implementation of the
		''' <code>org.omg.CORBA.Object</code> method.
		''' <P>Called by a stub to obtain an OutputStream for
		''' marshaling arguments. The stub must supply the operation name,
		''' and indicate if a response is expected (i.e is this a oneway
		''' call).<P> </summary>
		''' <param name="operation"> the name of the operation being requested </param>
		''' <param name="responseExpected"> <code>true</code> if a response is expected,
		'''                         <code>false</code> if it is a one-way call </param>
		''' <returns> NO_IMPLEMENT because this is a locally constrained object
		'''      and this method does not apply to local objects </returns>
		''' <exception cref="NO_IMPLEMENT"> because this is a locally constrained object
		'''      and this method does not apply to local objects </exception>
		''' <seealso cref= <a href="package-summary.html#unimpl"><code>CORBA</code> package
		'''      comments for unimplemented features</a> </seealso>
		Public Overridable Function _request(ByVal operation As String, ByVal responseExpected As Boolean) As OutputStream
			Throw New org.omg.CORBA.NO_IMPLEMENT(reason)
		End Function

		''' <summary>
		''' Throws an <code>org.omg.CORBA.NO_IMPLEMENT</code> exception with
		''' the message "This is a locally constrained object."
		''' This method is the default implementation of the
		''' <code>org.omg.CORBA.Object</code> method.
		''' <P>Called to invoke an operation. The stub provides an
		''' <code>OutputStream</code> that was previously returned by a
		''' <code>_request()</code>
		''' call. <code>_invoke</code> returns an <code>InputStream</code> which
		''' contains the
		''' marshaled reply. If an exception occurs, <code>_invoke</code> may throw an
		''' <code>ApplicationException</code> object which contains an
		''' <code>InputStream</code> from
		''' which the user exception state may be unmarshaled.<P> </summary>
		''' <param name="output"> the <code>OutputStream</code> to invoke </param>
		''' <returns> NO_IMPLEMENT because this is a locally constrained object
		'''      and this method does not apply to local objects </returns>
		''' <exception cref="ApplicationException"> If an exception occurs,
		''' <code>_invoke</code> may throw an
		''' <code>ApplicationException</code> object which contains
		''' an <code>InputStream</code> from
		''' which the user exception state may be unmarshaled. </exception>
		''' <exception cref="RemarshalException"> If an exception occurs,
		''' <code>_invoke</code> may throw an
		''' <code>ApplicationException</code> object which contains
		''' an <code>InputStream</code> from
		''' which the user exception state may be unmarshaled. </exception>
		''' <exception cref="NO_IMPLEMENT"> because this is a locally constrained object
		'''      and this method does not apply to local objects </exception>
		''' <seealso cref= <a href="package-summary.html#unimpl"><code>CORBA</code> package
		'''      comments for unimplemented features</a> </seealso>
		Public Overridable Function _invoke(ByVal output As OutputStream) As InputStream
			Throw New org.omg.CORBA.NO_IMPLEMENT(reason)
		End Function

		''' <summary>
		''' Throws an <code>org.omg.CORBA.NO_IMPLEMENT</code> exception with
		''' the message "This is a locally constrained object."
		''' This method is the default implementation of the
		''' <code>org.omg.CORBA.Object</code> method.
		''' <P>May optionally be called by a stub to release a
		''' reply stream back to the ORB when the unmarshaling has
		''' completed. The stub passes the <code>InputStream</code> returned by
		''' <code>_invoke()</code> or
		''' <code>ApplicationException.getInputStream()</code>.
		''' A null
		''' value may also be passed to <code>_releaseReply</code>, in which case the
		''' method is a no-op.<P> </summary>
		''' <param name="input"> the reply stream back to the ORB or null </param>
		''' <exception cref="NO_IMPLEMENT"> </exception>
		''' <seealso cref= <a href="package-summary.html#unimpl"><code>CORBA</code> package
		'''      comments for unimplemented features</a> </seealso>
		Public Overridable Sub _releaseReply(ByVal input As InputStream)
			Throw New org.omg.CORBA.NO_IMPLEMENT(reason)
		End Sub

		''' <summary>
		''' Throws an <code>org.omg.CORBA.NO_IMPLEMENT</code> exception with
		''' the message "This is a locally constrained object." This method
		''' does not apply to local objects and is therefore not implemented.
		''' This method is the default implementation of the
		''' <code>org.omg.CORBA.Object</code> method.<P> </summary>
		''' <returns> NO_IMPLEMENT because this is a locally constrained object
		'''      and this method does not apply to local objects </returns>
		''' <exception cref="NO_IMPLEMENT"> because this is a locally constrained object
		'''      and this method does not apply to local objects </exception>
		''' <seealso cref= <a href="package-summary.html#unimpl"><code>CORBA</code> package
		'''      comments for unimplemented features</a> </seealso>

		Public Overridable Function validate_connection() As Boolean
			Throw New org.omg.CORBA.NO_IMPLEMENT(reason)
		End Function
	End Class

End Namespace