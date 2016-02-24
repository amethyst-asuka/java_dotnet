Namespace org.omg.PortableServer


	''' <summary>
	''' org/omg/PortableServer/ServantActivatorOperations.java .
	''' Generated by the IDL-to-Java compiler (portable), version "3.2"
	''' from d:/re/puppet/workspace/8-2-build-windows-amd64-cygwin/jdk8u73/6086/corba/src/share/classes/org/omg/PortableServer/poa.idl
	''' Friday, January 29, 2016 5:40:18 PM PST
	''' </summary>


	''' <summary>
	''' When the POA has the RETAIN policy it uses servant 
	''' managers that are ServantActivators. 
	''' </summary>
	Public Interface ServantActivatorOperations
		Inherits org.omg.PortableServer.ServantManagerOperations

	  ''' <summary>
	  ''' This operation is invoked by the POA whenever the 
	  ''' POA receives a request for an object that is not 
	  ''' currently active, assuming the POA has the 
	  ''' USE_SERVANT_MANAGER and RETAIN policies. </summary>
	  ''' <param name="oid"> object Id associated with the object on 
	  '''            the request was made. </param>
	  ''' <param name="adapter"> object reference for the POA in which
	  '''                the object is being activated. </param>
	  ''' <returns> Servant corresponding to oid is created or 
	  '''         located by the user supplied servant manager. </returns>
	  ''' <exception cref="ForwardRequest"> to indicate to the ORB 
	  '''            that it is responsible for delivering 
	  '''            the current request and subsequent 
	  '''            requests to the object denoted in the 
	  '''            forward_reference member of the exception. </exception>
	  Function incarnate(ByVal oid As SByte(), ByVal adapter As org.omg.PortableServer.POA) As org.omg.PortableServer.Servant

	  ''' <summary>
	  ''' This operation is invoked whenever a servant for 
	  ''' an object is deactivated, assuming the POA has 
	  ''' the USE_SERVANT_MANAGER and RETAIN policies. </summary>
	  ''' <param name="oid"> object Id associated with the object 
	  '''            being deactivated. </param>
	  ''' <param name="adapter"> object reference for the POA in which
	  '''                the object was active. </param>
	  ''' <param name="serv"> contains reference to the servant
	  '''        associated with the object being deactivated. </param>
	  ''' <param name="cleanup_in_progress"> if TRUE indicates that
	  '''        destroy or deactivate is called with 
	  '''        etherealize_objects param of TRUE.  FALSE
	  '''        indicates that etherealize was called due to
	  '''        other reasons. </param>
	  ''' <param name="remaining_activations"> indicates whether the
	  '''        Servant Manager can destroy a servant.  If
	  '''        set to TRUE, the Servant Manager should wait
	  '''        until all invocations in progress have
	  '''        completed. </param>
	  Sub etherealize(ByVal oid As SByte(), ByVal adapter As org.omg.PortableServer.POA, ByVal serv As org.omg.PortableServer.Servant, ByVal cleanup_in_progress As Boolean, ByVal remaining_activations As Boolean)
	End Interface ' interface ServantActivatorOperations

End Namespace