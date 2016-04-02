'
' * Copyright (c) 1997, 2003, Oracle and/or its affiliates. All rights reserved.
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

Namespace java.rmi.activation


	''' <summary>
	''' The <code>ActivationSystem</code> provides a means for registering
	''' groups and "activatable" objects to be activated within those groups.
	''' The <code>ActivationSystem</code> works closely with the
	''' <code>Activator</code>, which activates objects registered via the
	''' <code>ActivationSystem</code>, and the <code>ActivationMonitor</code>,
	''' which obtains information about active and inactive objects,
	''' and inactive groups.
	''' 
	''' @author      Ann Wollrath </summary>
	''' <seealso cref=         Activator </seealso>
	''' <seealso cref=         ActivationMonitor
	''' @since       1.2 </seealso>
	Public Interface ActivationSystem
		Inherits java.rmi.Remote

		''' <summary>
		''' The port to lookup the activation system. </summary>
'JAVA TO VB CONVERTER TODO TASK: Interfaces cannot contain fields in .NET:
'		Public Shared final int SYSTEM_PORT = 1098;

		''' <summary>
		''' The <code>registerObject</code> method is used to register an
		''' activation descriptor, <code>desc</code>, and obtain an
		''' activation identifier for a activatable remote object. The
		''' <code>ActivationSystem</code> creates an
		''' <code>ActivationID</code> (a activation identifier) for the
		''' object specified by the descriptor, <code>desc</code>, and
		''' records, in stable storage, the activation descriptor and its
		''' associated identifier for later use. When the <code>Activator</code>
		''' receives an <code>activate</code> request for a specific identifier, it
		''' looks up the activation descriptor (registered previously) for
		''' the specified identifier and uses that information to activate
		''' the object. <p>
		''' </summary>
		''' <param name="desc"> the object's activation descriptor </param>
		''' <returns> the activation id that can be used to activate the object </returns>
		''' <exception cref="ActivationException"> if registration fails (e.g., database
		''' update failure, etc). </exception>
		''' <exception cref="UnknownGroupException"> if group referred to in
		''' <code>desc</code> is not registered with this system </exception>
		''' <exception cref="RemoteException"> if remote call fails
		''' @since 1.2 </exception>
		Function registerObject(ByVal desc As ActivationDesc) As ActivationID

		''' <summary>
		''' Remove the activation id and associated descriptor previously
		''' registered with the <code>ActivationSystem</code>; the object
		''' can no longer be activated via the object's activation id.
		''' </summary>
		''' <param name="id"> the object's activation id (from previous registration) </param>
		''' <exception cref="ActivationException"> if unregister fails (e.g., database
		''' update failure, etc). </exception>
		''' <exception cref="UnknownObjectException"> if object is unknown (not registered) </exception>
		''' <exception cref="RemoteException"> if remote call fails
		''' @since 1.2 </exception>
		Sub unregisterObject(ByVal id As ActivationID)

		''' <summary>
		''' Register the activation group. An activation group must be
		''' registered with the <code>ActivationSystem</code> before objects
		''' can be registered within that group.
		''' </summary>
		''' <param name="desc"> the group's descriptor </param>
		''' <returns> an identifier for the group </returns>
		''' <exception cref="ActivationException"> if group registration fails </exception>
		''' <exception cref="RemoteException"> if remote call fails
		''' @since 1.2 </exception>
		Function registerGroup(ByVal desc As ActivationGroupDesc) As ActivationGroupID

		''' <summary>
		''' Callback to inform activation system that group is now
		''' active. This call is made internally by the
		''' <code>ActivationGroup.createGroup</code> method to inform
		''' the <code>ActivationSystem</code> that the group is now
		''' active.
		''' </summary>
		''' <param name="id"> the activation group's identifier </param>
		''' <param name="group"> the group's instantiator </param>
		''' <param name="incarnation"> the group's incarnation number </param>
		''' <returns> monitor for activation group </returns>
		''' <exception cref="UnknownGroupException"> if group is not registered </exception>
		''' <exception cref="ActivationException"> if a group for the specified
		''' <code>id</code> is already active and that group is not equal
		''' to the specified <code>group</code> or that group has a different
		''' <code>incarnation</code> than the specified <code>group</code> </exception>
		''' <exception cref="RemoteException"> if remote call fails
		''' @since 1.2 </exception>
		Function activeGroup(ByVal id As ActivationGroupID, ByVal group As ActivationInstantiator, ByVal incarnation As Long) As ActivationMonitor

		''' <summary>
		''' Remove the activation group. An activation group makes this call back
		''' to inform the activator that the group should be removed (destroyed).
		''' If this call completes successfully, objects can no longer be
		''' registered or activated within the group. All information of the
		''' group and its associated objects is removed from the system.
		''' </summary>
		''' <param name="id"> the activation group's identifier </param>
		''' <exception cref="ActivationException"> if unregister fails (e.g., database
		''' update failure, etc). </exception>
		''' <exception cref="UnknownGroupException"> if group is not registered </exception>
		''' <exception cref="RemoteException"> if remote call fails
		''' @since 1.2 </exception>
		Sub unregisterGroup(ByVal id As ActivationGroupID)

		''' <summary>
		''' Shutdown the activation system. Destroys all groups spawned by
		''' the activation daemon and exits the activation daemon. </summary>
		''' <exception cref="RemoteException"> if failed to contact/shutdown the activation
		''' daemon
		''' @since 1.2 </exception>
		Sub shutdown()

		''' <summary>
		''' Set the activation descriptor, <code>desc</code> for the object with
		''' the activation identifier, <code>id</code>. The change will take
		''' effect upon subsequent activation of the object.
		''' </summary>
		''' <param name="id"> the activation identifier for the activatable object </param>
		''' <param name="desc"> the activation descriptor for the activatable object </param>
		''' <exception cref="UnknownGroupException"> the group associated with
		''' <code>desc</code> is not a registered group </exception>
		''' <exception cref="UnknownObjectException"> the activation <code>id</code>
		''' is not registered </exception>
		''' <exception cref="ActivationException"> for general failure (e.g., unable
		''' to update log) </exception>
		''' <exception cref="RemoteException"> if remote call fails </exception>
		''' <returns> the previous value of the activation descriptor </returns>
		''' <seealso cref= #getActivationDesc
		''' @since 1.2 </seealso>
		Function setActivationDesc(ByVal id As ActivationID, ByVal desc As ActivationDesc) As ActivationDesc

		''' <summary>
		''' Set the activation group descriptor, <code>desc</code> for the object
		''' with the activation group identifier, <code>id</code>. The change will
		''' take effect upon subsequent activation of the group.
		''' </summary>
		''' <param name="id"> the activation group identifier for the activation group </param>
		''' <param name="desc"> the activation group descriptor for the activation group </param>
		''' <exception cref="UnknownGroupException"> the group associated with
		''' <code>id</code> is not a registered group </exception>
		''' <exception cref="ActivationException"> for general failure (e.g., unable
		''' to update log) </exception>
		''' <exception cref="RemoteException"> if remote call fails </exception>
		''' <returns> the previous value of the activation group descriptor </returns>
		''' <seealso cref= #getActivationGroupDesc
		''' @since 1.2 </seealso>
		Function setActivationGroupDesc(ByVal id As ActivationGroupID, ByVal desc As ActivationGroupDesc) As ActivationGroupDesc

		''' <summary>
		''' Returns the activation descriptor, for the object with the activation
		''' identifier, <code>id</code>.
		''' </summary>
		''' <param name="id"> the activation identifier for the activatable object </param>
		''' <exception cref="UnknownObjectException"> if <code>id</code> is not registered </exception>
		''' <exception cref="ActivationException"> for general failure </exception>
		''' <exception cref="RemoteException"> if remote call fails </exception>
		''' <returns> the activation descriptor </returns>
		''' <seealso cref= #setActivationDesc
		''' @since 1.2 </seealso>
		Function getActivationDesc(ByVal id As ActivationID) As ActivationDesc

		''' <summary>
		''' Returns the activation group descriptor, for the group
		''' with the activation group identifier, <code>id</code>.
		''' </summary>
		''' <param name="id"> the activation group identifier for the group </param>
		''' <exception cref="UnknownGroupException"> if <code>id</code> is not registered </exception>
		''' <exception cref="ActivationException"> for general failure </exception>
		''' <exception cref="RemoteException"> if remote call fails </exception>
		''' <returns> the activation group descriptor </returns>
		''' <seealso cref= #setActivationGroupDesc
		''' @since 1.2 </seealso>
		Function getActivationGroupDesc(ByVal id As ActivationGroupID) As ActivationGroupDesc
	End Interface

End Namespace