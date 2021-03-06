Namespace org.omg.PortableServer


	''' <summary>
	''' org/omg/PortableServer/CurrentOperations.java .
	''' Generated by the IDL-to-Java compiler (portable), version "3.2"
	''' from d:/re/puppet/workspace/8-2-build-windows-amd64-cygwin/jdk8u73/6086/corba/src/share/classes/org/omg/PortableServer/poa.idl
	''' Friday, January 29, 2016 5:40:18 PM PST
	''' </summary>


	''' <summary>
	''' The PortableServer::Current interface, derived from 
	''' CORBA::Current, provides method implementations with 
	''' access to the identity of the object on which the 
	''' method was invoked. The Current interface is provided 
	''' to support servants that implement multiple objects, 
	''' but can be used within the context of POA-dispatched 
	''' method invocations on any servant. To provide location 
	''' transparency, ORBs are required to support use of 
	''' Current in the context of both locally and remotely 
	''' invoked operations. An instance of Current can be 
	''' obtained by the application by issuing the 
	''' CORBA::ORB::resolve_initial_references("POACurrent") 
	''' operation. Thereafter, it can be used within the 
	''' context of a method dispatched by the POA to obtain 
	''' the POA and ObjectId that identify the object on 
	''' which that operation was invoked.
	''' </summary>
	Public Interface CurrentOperations
		Inherits org.omg.CORBA.CurrentOperations

	  ''' <summary>
	  ''' Returns reference to the POA implementing the 
	  ''' object in whose context it is called. 
	  ''' </summary>
	  ''' <returns> The poa implementing the object
	  ''' </returns>
	  ''' <exception cref="NoContext"> is raised when the operation is
	  '''            outside the context of a POA-dispatched 
	  '''            operation </exception>
	  Function get_POA() As org.omg.PortableServer.POA

	  ''' <summary>
	  ''' Returns the ObjectId identifying the object in 
	  ''' whose context it is called. 
	  ''' </summary>
	  ''' <returns> the ObjectId of the object
	  ''' </returns>
	  ''' <exception cref="NoContext"> is raised when the operation
	  ''' is called outside the context of a POA-dispatched 
	  ''' operation. </exception>
	  Function get_object_id() As SByte()
	End Interface ' interface CurrentOperations

End Namespace