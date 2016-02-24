Namespace org.omg.PortableInterceptor


	''' <summary>
	''' org/omg/PortableInterceptor/PolicyFactoryOperations.java .
	''' Generated by the IDL-to-Java compiler (portable), version "3.2"
	''' from d:/re/puppet/workspace/8-2-build-windows-amd64-cygwin/jdk8u73/6086/corba/src/share/classes/org/omg/PortableInterceptor/Interceptors.idl
	''' Friday, January 29, 2016 5:40:18 PM PST
	''' </summary>


	''' <summary>
	''' Enables policy types to be constructed using 
	''' <code>CORBA.ORB.create_policy</code>.
	''' <p>
	''' A portable ORB service implementation registers an instance of the 
	''' <code>PolicyFactory</code> interface during ORB initialization in order 
	''' to enable its policy types to be constructed using 
	''' <code>CORBA.ORB.create_policy</code>. The POA is required to preserve 
	''' any policy which is registered with <code>ORBInitInfo</code> in this 
	''' manner.
	''' </summary>
	''' <seealso cref= ORBInitInfo#register_policy_factory </seealso>
	Public Interface PolicyFactoryOperations

	  ''' <summary>
	  ''' Returns an instance of the appropriate interface derived from 
	  ''' <code>CORBA.Policy</code> whose value corresponds to the 
	  ''' specified any. 
	  ''' <p>
	  ''' The ORB calls <code>create_policy</code> on a registered 
	  ''' <code>PolicyFactory</code> instance when 
	  ''' <code>CORBA.ORB.create_policy</code> is called for the 
	  ''' <code>PolicyType</code> under which the <code>PolicyFactory</code> has 
	  ''' been registered. The <code>create_policy</code> operation then 
	  ''' returns an instance of the appropriate interface derived from 
	  ''' <code>CORBA.Policy</code> whose value corresponds to the specified 
	  ''' any. If it cannot, it shall throw an exception as described for 
	  ''' <code>CORBA.ORB.create_policy</code>. 
	  ''' </summary>
	  ''' <param name="type"> An int specifying the type of policy being created. </param>
	  ''' <param name="value"> An any containing data with which to construct the 
	  '''     <code>CORBA.Policy</code>. </param>
	  ''' <returns> A <code>CORBA.Policy<code> object of the specified type and 
	  '''     value. </returns>
	  Function create_policy(ByVal type As Integer, ByVal value As org.omg.CORBA.Any) As org.omg.CORBA.Policy
	End Interface ' interface PolicyFactoryOperations

End Namespace