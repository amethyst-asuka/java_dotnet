Namespace org.omg.PortableServer


	''' <summary>
	''' org/omg/PortableServer/ServantRetentionPolicyOperations.java .
	''' Generated by the IDL-to-Java compiler (portable), version "3.2"
	''' from d:/re/puppet/workspace/8-2-build-windows-amd64-cygwin/jdk8u73/6086/corba/src/share/classes/org/omg/PortableServer/poa.idl
	''' Friday, January 29, 2016 5:40:18 PM PST
	''' </summary>


	''' <summary>
	''' This policy specifies whether the created POA retains 
	''' active servants in an Active Object Map. 
	''' </summary>
	Public Interface ServantRetentionPolicyOperations
		Inherits org.omg.CORBA.PolicyOperations

	  ''' <summary>
	  ''' specifies the policy value
	  ''' </summary>
	  Function value() As org.omg.PortableServer.ServantRetentionPolicyValue
	End Interface ' interface ServantRetentionPolicyOperations

End Namespace