Namespace org.omg.PortableServer


	''' <summary>
	''' org/omg/PortableServer/ImplicitActivationPolicyOperations.java .
	''' Generated by the IDL-to-Java compiler (portable), version "3.2"
	''' from d:/re/puppet/workspace/8-2-build-windows-amd64-cygwin/jdk8u73/6086/corba/src/share/classes/org/omg/PortableServer/poa.idl
	''' Friday, January 29, 2016 5:40:18 PM PST
	''' </summary>


	''' <summary>
	''' This policy specifies whether implicit activation 
	''' of servants is supported in the created POA.
	''' </summary>
	Public Interface ImplicitActivationPolicyOperations
		Inherits org.omg.CORBA.PolicyOperations

	  ''' <summary>
	  ''' specifies the policy value
	  ''' </summary>
	  Function value() As org.omg.PortableServer.ImplicitActivationPolicyValue
	End Interface ' interface ImplicitActivationPolicyOperations

End Namespace