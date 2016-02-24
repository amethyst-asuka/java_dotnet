Imports System

'
' * Copyright (c) 1997, 2013, Oracle and/or its affiliates. All rights reserved.
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
	''' The <code>Activatable</code> class provides support for remote
	''' objects that require persistent access over time and that
	''' can be activated by the system.
	''' 
	''' <p>For the constructors and static <code>exportObject</code> methods,
	''' the stub for a remote object being exported is obtained as described in
	''' <seealso cref="java.rmi.server.UnicastRemoteObject"/>.
	''' 
	''' <p>An attempt to serialize explicitly an instance of this class will
	''' fail.
	''' 
	''' @author      Ann Wollrath
	''' @since       1.2
	''' @serial      exclude
	''' </summary>
	Public MustInherit Class Activatable
		Inherits java.rmi.server.RemoteServer

		Private id As ActivationID
		''' <summary>
		''' indicate compatibility with the Java 2 SDK v1.2 version of class </summary>
		Private Const serialVersionUID As Long = -3120617863591563455L

		''' <summary>
		''' Constructs an activatable remote object by registering
		''' an activation descriptor (with the specified location, data, and
		''' restart mode) for this object, and exporting the object with the
		''' specified port.
		''' 
		''' <p><strong>Note:</strong> Using the <code>Activatable</code>
		''' constructors that both register and export an activatable remote
		''' object is strongly discouraged because the actions of registering
		''' and exporting the remote object are <i>not</i> guaranteed to be
		''' atomic.  Instead, an application should register an activation
		''' descriptor and export a remote object separately, so that exceptions
		''' can be handled properly.
		''' 
		''' <p>This method invokes the {@link
		''' #exportObject(Remote,String,MarshalledObject,boolean,int)
		''' exportObject} method with this object, and the specified location,
		''' data, restart mode, and port.  Subsequent calls to <seealso cref="#getID"/>
		''' will return the activation identifier returned from the call to
		''' <code>exportObject</code>.
		''' </summary>
		''' <param name="location"> the location for classes for this object </param>
		''' <param name="data"> the object's initialization data </param>
		''' <param name="port"> the port on which the object is exported (an anonymous
		''' port is used if port=0) </param>
		''' <param name="restart"> if true, the object is restarted (reactivated) when
		''' either the activator is restarted or the object's activation group
		''' is restarted after an unexpected crash; if false, the object is only
		''' activated on demand.  Specifying <code>restart</code> to be
		''' <code>true</code> does not force an initial immediate activation of
		''' a newly registered object;  initial activation is lazy. </param>
		''' <exception cref="ActivationException"> if object registration fails. </exception>
		''' <exception cref="RemoteException"> if either of the following fails:
		''' a) registering the object with the activation system or b) exporting
		''' the object to the RMI runtime. </exception>
		''' <exception cref="UnsupportedOperationException"> if and only if activation is
		''' not supported by this implementation.
		''' @since 1.2
		'''  </exception>
		Protected Friend Sub New(Of T1)(ByVal location As String, ByVal data As java.rmi.MarshalledObject(Of T1), ByVal restart As Boolean, ByVal port As Integer)
			MyBase.New()
			id = exportObject(Me, location, data, restart, port)
		End Sub

		''' <summary>
		''' Constructs an activatable remote object by registering
		''' an activation descriptor (with the specified location, data, and
		''' restart mode) for this object, and exporting the object with the
		''' specified port, and specified client and server socket factories.
		''' 
		''' <p><strong>Note:</strong> Using the <code>Activatable</code>
		''' constructors that both register and export an activatable remote
		''' object is strongly discouraged because the actions of registering
		''' and exporting the remote object are <i>not</i> guaranteed to be
		''' atomic.  Instead, an application should register an activation
		''' descriptor and export a remote object separately, so that exceptions
		''' can be handled properly.
		''' 
		''' <p>This method invokes the {@link
		''' #exportObject(Remote,String,MarshalledObject,boolean,int,RMIClientSocketFactory,RMIServerSocketFactory)
		''' exportObject} method with this object, and the specified location,
		''' data, restart mode, port, and client and server socket factories.
		''' Subsequent calls to <seealso cref="#getID"/> will return the activation
		''' identifier returned from the call to <code>exportObject</code>.
		''' </summary>
		''' <param name="location"> the location for classes for this object </param>
		''' <param name="data"> the object's initialization data </param>
		''' <param name="restart"> if true, the object is restarted (reactivated) when
		''' either the activator is restarted or the object's activation group
		''' is restarted after an unexpected crash; if false, the object is only
		''' activated on demand.  Specifying <code>restart</code> to be
		''' <code>true</code> does not force an initial immediate activation of
		''' a newly registered object;  initial activation is lazy. </param>
		''' <param name="port"> the port on which the object is exported (an anonymous
		''' port is used if port=0) </param>
		''' <param name="csf"> the client-side socket factory for making calls to the
		''' remote object </param>
		''' <param name="ssf"> the server-side socket factory for receiving remote calls </param>
		''' <exception cref="ActivationException"> if object registration fails. </exception>
		''' <exception cref="RemoteException"> if either of the following fails:
		''' a) registering the object with the activation system or b) exporting
		''' the object to the RMI runtime. </exception>
		''' <exception cref="UnsupportedOperationException"> if and only if activation is
		''' not supported by this implementation.
		''' @since 1.2
		'''  </exception>
		Protected Friend Sub New(Of T1)(ByVal location As String, ByVal data As java.rmi.MarshalledObject(Of T1), ByVal restart As Boolean, ByVal port As Integer, ByVal csf As java.rmi.server.RMIClientSocketFactory, ByVal ssf As java.rmi.server.RMIServerSocketFactory)
			MyBase.New()
			id = exportObject(Me, location, data, restart, port, csf, ssf)
		End Sub

		''' <summary>
		''' Constructor used to activate/export the object on a specified
		''' port. An "activatable" remote object must have a constructor that
		''' takes two arguments: <ul>
		''' <li>the object's activation identifier (<code>ActivationID</code>), and
		''' <li>the object's initialization data (a <code>MarshalledObject</code>).
		''' </ul><p>
		''' 
		''' A concrete subclass of this class must call this constructor when it is
		''' <i>activated</i> via the two parameter constructor described above. As
		''' a side-effect of construction, the remote object is "exported"
		''' to the RMI runtime (on the specified <code>port</code>) and is
		''' available to accept incoming calls from clients.
		''' </summary>
		''' <param name="id"> activation identifier for the object </param>
		''' <param name="port"> the port number on which the object is exported </param>
		''' <exception cref="RemoteException"> if exporting the object to the RMI
		''' runtime fails </exception>
		''' <exception cref="UnsupportedOperationException"> if and only if activation is
		''' not supported by this implementation
		''' @since 1.2 </exception>
		Protected Friend Sub New(ByVal id As ActivationID, ByVal port As Integer)
			MyBase.New()
			Me.id = id
			exportObject(Me, id, port)
		End Sub

		''' <summary>
		''' Constructor used to activate/export the object on a specified
		''' port. An "activatable" remote object must have a constructor that
		''' takes two arguments: <ul>
		''' <li>the object's activation identifier (<code>ActivationID</code>), and
		''' <li>the object's initialization data (a <code>MarshalledObject</code>).
		''' </ul><p>
		''' 
		''' A concrete subclass of this class must call this constructor when it is
		''' <i>activated</i> via the two parameter constructor described above. As
		''' a side-effect of construction, the remote object is "exported"
		''' to the RMI runtime (on the specified <code>port</code>) and is
		''' available to accept incoming calls from clients.
		''' </summary>
		''' <param name="id"> activation identifier for the object </param>
		''' <param name="port"> the port number on which the object is exported </param>
		''' <param name="csf"> the client-side socket factory for making calls to the
		''' remote object </param>
		''' <param name="ssf"> the server-side socket factory for receiving remote calls </param>
		''' <exception cref="RemoteException"> if exporting the object to the RMI
		''' runtime fails </exception>
		''' <exception cref="UnsupportedOperationException"> if and only if activation is
		''' not supported by this implementation
		''' @since 1.2 </exception>
		Protected Friend Sub New(ByVal id As ActivationID, ByVal port As Integer, ByVal csf As java.rmi.server.RMIClientSocketFactory, ByVal ssf As java.rmi.server.RMIServerSocketFactory)
			MyBase.New()
			Me.id = id
			exportObject(Me, id, port, csf, ssf)
		End Sub

		''' <summary>
		''' Returns the object's activation identifier.  The method is
		''' protected so that only subclasses can obtain an object's
		''' identifier. </summary>
		''' <returns> the object's activation identifier
		''' @since 1.2 </returns>
		Protected Friend Overridable Property iD As ActivationID
			Get
				Return id
			End Get
		End Property

		''' <summary>
		''' Register an object descriptor for an activatable remote
		''' object so that is can be activated on demand.
		''' </summary>
		''' <param name="desc">  the object's descriptor </param>
		''' <returns> the stub for the activatable remote object </returns>
		''' <exception cref="UnknownGroupException"> if group id in <code>desc</code>
		''' is not registered with the activation system </exception>
		''' <exception cref="ActivationException"> if activation system is not running </exception>
		''' <exception cref="RemoteException"> if remote call fails </exception>
		''' <exception cref="UnsupportedOperationException"> if and only if activation is
		''' not supported by this implementation
		''' @since 1.2 </exception>
		Public Shared Function register(ByVal desc As ActivationDesc) As java.rmi.Remote
			' register object with activator.
			Dim id_Renamed As ActivationID = ActivationGroup.system.registerObject(desc)
			Return sun.rmi.server.ActivatableRef.getStub(desc, id_Renamed)
		End Function

		''' <summary>
		''' Informs the system that the object with the corresponding activation
		''' <code>id</code> is currently inactive. If the object is currently
		''' active, the object is "unexported" from the RMI runtime (only if
		''' there are no pending or in-progress calls)
		''' so the that it can no longer receive incoming calls. This call
		''' informs this VM's ActivationGroup that the object is inactive,
		''' that, in turn, informs its ActivationMonitor. If this call
		''' completes successfully, a subsequent activate request to the activator
		''' will cause the object to reactivate. The operation may still
		''' succeed if the object is considered active but has already
		''' unexported itself.
		''' </summary>
		''' <param name="id"> the object's activation identifier </param>
		''' <returns> true if the operation succeeds (the operation will
		''' succeed if the object in currently known to be active and is
		''' either already unexported or is currently exported and has no
		''' pending/executing calls); false is returned if the object has
		''' pending/executing calls in which case it cannot be deactivated </returns>
		''' <exception cref="UnknownObjectException"> if object is not known (it may
		''' already be inactive) </exception>
		''' <exception cref="ActivationException"> if group is not active </exception>
		''' <exception cref="RemoteException"> if call informing monitor fails </exception>
		''' <exception cref="UnsupportedOperationException"> if and only if activation is
		''' not supported by this implementation
		''' @since 1.2 </exception>
		Public Shared Function inactive(ByVal id As ActivationID) As Boolean
			Return ActivationGroup.currentGroup().inactiveObject(id)
		End Function

		''' <summary>
		''' Revokes previous registration for the activation descriptor
		''' associated with <code>id</code>. An object can no longer be
		''' activated via that <code>id</code>.
		''' </summary>
		''' <param name="id"> the object's activation identifier </param>
		''' <exception cref="UnknownObjectException"> if object (<code>id</code>) is unknown </exception>
		''' <exception cref="ActivationException"> if activation system is not running </exception>
		''' <exception cref="RemoteException"> if remote call to activation system fails </exception>
		''' <exception cref="UnsupportedOperationException"> if and only if activation is
		''' not supported by this implementation
		''' @since 1.2 </exception>
		Public Shared Sub unregister(ByVal id As ActivationID)
			ActivationGroup.system.unregisterObject(id)
		End Sub

		''' <summary>
		''' Registers an activation descriptor (with the specified location,
		''' data, and restart mode) for the specified object, and exports that
		''' object with the specified port.
		''' 
		''' <p><strong>Note:</strong> Using this method (as well as the
		''' <code>Activatable</code> constructors that both register and export
		''' an activatable remote object) is strongly discouraged because the
		''' actions of registering and exporting the remote object are
		''' <i>not</i> guaranteed to be atomic.  Instead, an application should
		''' register an activation descriptor and export a remote object
		''' separately, so that exceptions can be handled properly.
		''' 
		''' <p>This method invokes the {@link
		''' #exportObject(Remote,String,MarshalledObject,boolean,int,RMIClientSocketFactory,RMIServerSocketFactory)
		''' exportObject} method with the specified object, location, data,
		''' restart mode, and port, and <code>null</code> for both client and
		''' server socket factories, and then returns the resulting activation
		''' identifier.
		''' </summary>
		''' <param name="obj"> the object being exported </param>
		''' <param name="location"> the object's code location </param>
		''' <param name="data"> the object's bootstrapping data </param>
		''' <param name="restart"> if true, the object is restarted (reactivated) when
		''' either the activator is restarted or the object's activation group
		''' is restarted after an unexpected crash; if false, the object is only
		''' activated on demand.  Specifying <code>restart</code> to be
		''' <code>true</code> does not force an initial immediate activation of
		''' a newly registered object;  initial activation is lazy. </param>
		''' <param name="port"> the port on which the object is exported (an anonymous
		''' port is used if port=0) </param>
		''' <returns> the activation identifier obtained from registering the
		''' descriptor, <code>desc</code>, with the activation system
		''' the wrong group </returns>
		''' <exception cref="ActivationException"> if activation group is not active </exception>
		''' <exception cref="RemoteException"> if object registration or export fails </exception>
		''' <exception cref="UnsupportedOperationException"> if and only if activation is
		''' not supported by this implementation
		''' @since 1.2
		'''  </exception>
		Public Shared Function exportObject(Of T1)(ByVal obj As java.rmi.Remote, ByVal location As String, ByVal data As java.rmi.MarshalledObject(Of T1), ByVal restart As Boolean, ByVal port As Integer) As ActivationID
			Return exportObject(obj, location, data, restart, port, Nothing, Nothing)
		End Function

		''' <summary>
		''' Registers an activation descriptor (with the specified location,
		''' data, and restart mode) for the specified object, and exports that
		''' object with the specified port, and the specified client and server
		''' socket factories.
		''' 
		''' <p><strong>Note:</strong> Using this method (as well as the
		''' <code>Activatable</code> constructors that both register and export
		''' an activatable remote object) is strongly discouraged because the
		''' actions of registering and exporting the remote object are
		''' <i>not</i> guaranteed to be atomic.  Instead, an application should
		''' register an activation descriptor and export a remote object
		''' separately, so that exceptions can be handled properly.
		''' 
		''' <p>This method first registers an activation descriptor for the
		''' specified object as follows. It obtains the activation system by
		''' invoking the method {@link ActivationGroup#getSystem
		''' ActivationGroup.getSystem}.  This method then obtains an {@link
		''' ActivationID} for the object by invoking the activation system's
		''' <seealso cref="ActivationSystem#registerObject registerObject"/> method with
		''' an <seealso cref="ActivationDesc"/> constructed with the specified object's
		''' class name, and the specified location, data, and restart mode.  If
		''' an exception occurs obtaining the activation system or registering
		''' the activation descriptor, that exception is thrown to the caller.
		''' 
		''' <p>Next, this method exports the object by invoking the {@link
		''' #exportObject(Remote,ActivationID,int,RMIClientSocketFactory,RMIServerSocketFactory)
		''' exportObject} method with the specified remote object, the
		''' activation identifier obtained from registration, the specified
		''' port, and the specified client and server socket factories.  If an
		''' exception occurs exporting the object, this method attempts to
		''' unregister the activation identifier (obtained from registration) by
		''' invoking the activation system's {@link
		''' ActivationSystem#unregisterObject unregisterObject} method with the
		''' activation identifier.  If an exception occurs unregistering the
		''' identifier, that exception is ignored, and the original exception
		''' that occurred exporting the object is thrown to the caller.
		''' 
		''' <p>Finally, this method invokes the {@link
		''' ActivationGroup#activeObject activeObject} method on the activation
		''' group in this VM with the activation identifier and the specified
		''' remote object, and returns the activation identifier to the caller.
		''' </summary>
		''' <param name="obj"> the object being exported </param>
		''' <param name="location"> the object's code location </param>
		''' <param name="data"> the object's bootstrapping data </param>
		''' <param name="restart"> if true, the object is restarted (reactivated) when
		''' either the activator is restarted or the object's activation group
		''' is restarted after an unexpected crash; if false, the object is only
		''' activated on demand.  Specifying <code>restart</code> to be
		''' <code>true</code> does not force an initial immediate activation of
		''' a newly registered object;  initial activation is lazy. </param>
		''' <param name="port"> the port on which the object is exported (an anonymous
		''' port is used if port=0) </param>
		''' <param name="csf"> the client-side socket factory for making calls to the
		''' remote object </param>
		''' <param name="ssf"> the server-side socket factory for receiving remote calls </param>
		''' <returns> the activation identifier obtained from registering the
		''' descriptor with the activation system </returns>
		''' <exception cref="ActivationException"> if activation group is not active </exception>
		''' <exception cref="RemoteException"> if object registration or export fails </exception>
		''' <exception cref="UnsupportedOperationException"> if and only if activation is
		''' not supported by this implementation
		''' @since 1.2
		'''  </exception>
		Public Shared Function exportObject(Of T1)(ByVal obj As java.rmi.Remote, ByVal location As String, ByVal data As java.rmi.MarshalledObject(Of T1), ByVal restart As Boolean, ByVal port As Integer, ByVal csf As java.rmi.server.RMIClientSocketFactory, ByVal ssf As java.rmi.server.RMIServerSocketFactory) As ActivationID
			Dim desc As New ActivationDesc(obj.GetType().name, location, data, restart)
	'        
	'         * Register descriptor.
	'         
			Dim system_Renamed As ActivationSystem = ActivationGroup.system
			Dim id_Renamed As ActivationID = system_Renamed.registerObject(desc)

	'        
	'         * Export object.
	'         
			Try
				exportObject(obj, id_Renamed, port, csf, ssf)
			Catch e As java.rmi.RemoteException
	'            
	'             * Attempt to unregister activation descriptor because export
	'             * failed and register/export should be atomic (see 4323621).
	'             
				Try
					system_Renamed.unregisterObject(id_Renamed)
				Catch ex As Exception
				End Try
	'            
	'             * Report original exception.
	'             
				Throw e
			End Try

	'        
	'         * This call can't fail (it is a local call, and the only possible
	'         * exception, thrown if the group is inactive, will not be thrown
	'         * because the group is not inactive).
	'         
			ActivationGroup.currentGroup().activeObject(id_Renamed, obj)

			Return id_Renamed
		End Function

		''' <summary>
		''' Export the activatable remote object to the RMI runtime to make
		''' the object available to receive incoming calls. The object is
		''' exported on an anonymous port, if <code>port</code> is zero. <p>
		''' 
		''' During activation, this <code>exportObject</code> method should
		''' be invoked explicitly by an "activatable" object, that does not
		''' extend the <code>Activatable</code> class. There is no need for objects
		''' that do extend the <code>Activatable</code> class to invoke this
		''' method directly because the object is exported during construction.
		''' </summary>
		''' <returns> the stub for the activatable remote object </returns>
		''' <param name="obj"> the remote object implementation </param>
		''' <param name="id"> the object's  activation identifier </param>
		''' <param name="port"> the port on which the object is exported (an anonymous
		''' port is used if port=0) </param>
		''' <exception cref="RemoteException"> if object export fails </exception>
		''' <exception cref="UnsupportedOperationException"> if and only if activation is
		''' not supported by this implementation
		''' @since 1.2 </exception>
		Public Shared Function exportObject(ByVal obj As java.rmi.Remote, ByVal id As ActivationID, ByVal port As Integer) As java.rmi.Remote
			Return exportObject(obj, New sun.rmi.server.ActivatableServerRef(id, port))
		End Function

		''' <summary>
		''' Export the activatable remote object to the RMI runtime to make
		''' the object available to receive incoming calls. The object is
		''' exported on an anonymous port, if <code>port</code> is zero. <p>
		''' 
		''' During activation, this <code>exportObject</code> method should
		''' be invoked explicitly by an "activatable" object, that does not
		''' extend the <code>Activatable</code> class. There is no need for objects
		''' that do extend the <code>Activatable</code> class to invoke this
		''' method directly because the object is exported during construction.
		''' </summary>
		''' <returns> the stub for the activatable remote object </returns>
		''' <param name="obj"> the remote object implementation </param>
		''' <param name="id"> the object's  activation identifier </param>
		''' <param name="port"> the port on which the object is exported (an anonymous
		''' port is used if port=0) </param>
		''' <param name="csf"> the client-side socket factory for making calls to the
		''' remote object </param>
		''' <param name="ssf"> the server-side socket factory for receiving remote calls </param>
		''' <exception cref="RemoteException"> if object export fails </exception>
		''' <exception cref="UnsupportedOperationException"> if and only if activation is
		''' not supported by this implementation
		''' @since 1.2 </exception>
		Public Shared Function exportObject(ByVal obj As java.rmi.Remote, ByVal id As ActivationID, ByVal port As Integer, ByVal csf As java.rmi.server.RMIClientSocketFactory, ByVal ssf As java.rmi.server.RMIServerSocketFactory) As java.rmi.Remote
			Return exportObject(obj, New sun.rmi.server.ActivatableServerRef(id, port, csf, ssf))
		End Function

		''' <summary>
		''' Remove the remote object, obj, from the RMI runtime. If
		''' successful, the object can no longer accept incoming RMI calls.
		''' If the force parameter is true, the object is forcibly unexported
		''' even if there are pending calls to the remote object or the
		''' remote object still has calls in progress.  If the force
		''' parameter is false, the object is only unexported if there are
		''' no pending or in progress calls to the object.
		''' </summary>
		''' <param name="obj"> the remote object to be unexported </param>
		''' <param name="force"> if true, unexports the object even if there are
		''' pending or in-progress calls; if false, only unexports the object
		''' if there are no pending or in-progress calls </param>
		''' <returns> true if operation is successful, false otherwise </returns>
		''' <exception cref="NoSuchObjectException"> if the remote object is not
		''' currently exported </exception>
		''' <exception cref="UnsupportedOperationException"> if and only if activation is
		''' not supported by this implementation
		''' @since 1.2 </exception>
		Public Shared Function unexportObject(ByVal obj As java.rmi.Remote, ByVal force As Boolean) As Boolean
			Return sun.rmi.transport.ObjectTable.unexportObject(obj, force)
		End Function

		''' <summary>
		''' Exports the specified object using the specified server ref.
		''' </summary>
		Private Shared Function exportObject(ByVal obj As java.rmi.Remote, ByVal sref As sun.rmi.server.ActivatableServerRef) As java.rmi.Remote
			' if obj extends Activatable, set its ref.
			If TypeOf obj Is Activatable Then CType(obj, Activatable).ref = sref
			Return sref.exportObject(obj, Nothing, False)
		End Function
	End Class

End Namespace