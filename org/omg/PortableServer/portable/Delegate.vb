'
' * Copyright (c) 1999, 2003, Oracle and/or its affiliates. All rights reserved.
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
Namespace org.omg.PortableServer.portable


	''' <summary>
	''' The portability package contains interfaces and classes
	''' that are designed for and intended to be used by ORB
	''' implementor. It exposes the publicly defined APIs that
	''' are used to connect stubs and skeletons to the ORB.
	''' The Delegate interface provides the ORB vendor specific
	''' implementation of PortableServer::Servant.
	''' Conformant to spec CORBA V2.3.1, ptc/00-01-08.pdf
	''' </summary>
	Public Interface [Delegate]
	''' <summary>
	''' Convenience method that returns the instance of the ORB
	''' currently associated with the Servant. </summary>
	''' <param name="Self"> the servant. </param>
	''' <returns> ORB associated with the Servant. </returns>
		Function orb(ByVal Self As org.omg.PortableServer.Servant) As org.omg.CORBA.ORB

	''' <summary>
	''' This allows the servant to obtain the object reference for
	''' the target CORBA Object it is incarnating for that request. </summary>
	''' <param name="Self"> the servant. </param>
	''' <returns> Object reference associated with the request. </returns>
		Function this_object(ByVal Self As org.omg.PortableServer.Servant) As org.omg.CORBA.Object

	''' <summary>
	''' The method _poa() is equivalent to
	''' calling PortableServer::Current:get_POA. </summary>
	''' <param name="Self"> the servant. </param>
	''' <returns> POA associated with the servant. </returns>
		Function poa(ByVal Self As org.omg.PortableServer.Servant) As org.omg.PortableServer.POA

	''' <summary>
	''' The method _object_id() is equivalent
	''' to calling PortableServer::Current::get_object_id. </summary>
	''' <param name="Self"> the servant. </param>
	''' <returns> ObjectId associated with this servant. </returns>
		Function object_id(ByVal Self As org.omg.PortableServer.Servant) As SByte()

	''' <summary>
	''' The default behavior of this function is to return the
	''' root POA from the ORB instance associated with the servant. </summary>
	''' <param name="Self"> the servant. </param>
	''' <returns> POA associated with the servant class. </returns>
		Function default_POA(ByVal Self As org.omg.PortableServer.Servant) As org.omg.PortableServer.POA

	''' <summary>
	''' This method checks to see if the specified repid is present
	''' on the list returned by _all_interfaces() or is the
	''' repository id for the generic CORBA Object. </summary>
	''' <param name="Self"> the servant. </param>
	''' <param name="Repository_Id"> the repository_id to be checked in the
	'''            repository list or against the id of generic CORBA
	'''            object. </param>
	''' <returns> boolean indicating whether the specified repid is
	'''         in the list or is same as that got generic CORBA
	'''         object. </returns>
		Function is_a(ByVal Self As org.omg.PortableServer.Servant, ByVal Repository_Id As String) As Boolean

	''' <summary>
	''' This operation is used to check for the existence of the
	''' Object. </summary>
	''' <param name="Self"> the servant. </param>
	''' <returns> boolean true to indicate that object does not exist,
	'''                 and false otherwise. </returns>
		Function non_existent(ByVal Self As org.omg.PortableServer.Servant) As Boolean
		'Simon And Ken Will Ask About Editorial Changes
		'In Idl To Java For The Following Signature.

	''' <summary>
	''' This operation returns an object in the Interface Repository
	''' which provides type information that may be useful to a program. </summary>
	''' <param name="self"> the servant. </param>
	''' <returns> type information corresponding to the object. </returns>
		' The get_interface() method has been replaced by get_interface_def()
		'org.omg.CORBA.Object get_interface(Servant Self);

		Function get_interface_def(ByVal self As org.omg.PortableServer.Servant) As org.omg.CORBA.Object
	End Interface

End Namespace