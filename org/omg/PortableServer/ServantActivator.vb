Namespace org.omg.PortableServer


	''' <summary>
	''' org/omg/PortableServer/ServantActivator.java .
	''' Generated by the IDL-to-Java compiler (portable), version "3.2"
	''' from d:/re/puppet/workspace/8-2-build-windows-amd64-cygwin/jdk8u73/6086/corba/src/share/classes/org/omg/PortableServer/poa.idl
	''' Friday, January 29, 2016 5:40:18 PM PST
	''' </summary>


	''' <summary>
	''' When the POA has the RETAIN policy it uses servant 
	''' managers that are ServantActivators. 
	''' </summary>
	Public Interface ServantActivator
		Inherits ServantActivatorOperations, org.omg.PortableServer.ServantManager, org.omg.CORBA.portable.IDLEntity

	End Interface ' interface ServantActivator

End Namespace