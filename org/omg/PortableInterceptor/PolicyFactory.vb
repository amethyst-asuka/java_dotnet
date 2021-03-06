Namespace org.omg.PortableInterceptor


	''' <summary>
	''' org/omg/PortableInterceptor/PolicyFactory.java .
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
	Public Interface PolicyFactory
		Inherits PolicyFactoryOperations, org.omg.CORBA.Object, org.omg.CORBA.portable.IDLEntity

	End Interface ' interface PolicyFactory

End Namespace