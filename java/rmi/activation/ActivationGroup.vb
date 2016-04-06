Imports System
Imports System.Runtime.CompilerServices

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
	''' An <code>ActivationGroup</code> is responsible for creating new
	''' instances of "activatable" objects in its group, informing its
	''' <code>ActivationMonitor</code> when either: its object's become
	''' active or inactive, or the group as a whole becomes inactive. <p>
	''' 
	''' An <code>ActivationGroup</code> is <i>initially</i> created in one
	''' of several ways: <ul>
	''' <li>as a side-effect of creating an <code>ActivationDesc</code>
	'''     without an explicit <code>ActivationGroupID</code> for the
	'''     first activatable object in the group, or
	''' <li>via the <code>ActivationGroup.createGroup</code> method
	''' <li>as a side-effect of activating the first object in a group
	'''     whose <code>ActivationGroupDesc</code> was only registered.</ul><p>
	''' 
	''' Only the activator can <i>recreate</i> an
	''' <code>ActivationGroup</code>.  The activator spawns, as needed, a
	''' separate VM (as a child process, for example) for each registered
	''' activation group and directs activation requests to the appropriate
	''' group. It is implementation specific how VMs are spawned. An
	''' activation group is created via the
	''' <code>ActivationGroup.createGroup</code> static method. The
	''' <code>createGroup</code> method has two requirements on the group
	''' to be created: 1) the group must be a concrete subclass of
	''' <code>ActivationGroup</code>, and 2) the group must have a
	''' constructor that takes two arguments:
	''' 
	''' <ul>
	''' <li> the group's <code>ActivationGroupID</code>, and
	''' <li> the group's initialization data (in a
	'''      <code>java.rmi.MarshalledObject</code>)</ul><p>
	''' 
	''' When created, the default implementation of
	''' <code>ActivationGroup</code> will override the system properties
	''' with the properties requested when its
	''' <code>ActivationGroupDesc</code> was created, and will set a
	''' <seealso cref="SecurityManager"/> as the default system
	''' security manager.  If your application requires specific properties
	''' to be set when objects are activated in the group, the application
	''' should create a special <code>Properties</code> object containing
	''' these properties, then create an <code>ActivationGroupDesc</code>
	''' with the <code>Properties</code> object, and use
	''' <code>ActivationGroup.createGroup</code> before creating any
	''' <code>ActivationDesc</code>s (before the default
	''' <code>ActivationGroupDesc</code> is created).  If your application
	''' requires the use of a security manager other than
	''' <seealso cref="SecurityManager"/>, in the
	''' ActivativationGroupDescriptor properties list you can set
	''' <code>java.security.manager</code> property to the name of the security
	''' manager you would like to install.
	''' 
	''' @author      Ann Wollrath </summary>
	''' <seealso cref=         ActivationInstantiator </seealso>
	''' <seealso cref=         ActivationGroupDesc </seealso>
	''' <seealso cref=         ActivationGroupID
	''' @since       1.2 </seealso>
	Public MustInherit Class ActivationGroup
		Inherits java.rmi.server.UnicastRemoteObject
		Implements ActivationInstantiator

'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
			Public MustOverride Function newInstance(  id As ActivationID,   desc As ActivationDesc) As java.rmi.MarshalledObject<? As java.rmi.Remote>
		''' <summary>
		''' @serial the group's identifier
		''' </summary>
		Private groupID As ActivationGroupID

		''' <summary>
		''' @serial the group's monitor
		''' </summary>
		Private monitor As ActivationMonitor

		''' <summary>
		''' @serial the group's incarnation number
		''' </summary>
		Private incarnation As Long

		''' <summary>
		''' the current activation group for this VM </summary>
		Private Shared currGroup As ActivationGroup
		''' <summary>
		''' the current group's identifier </summary>
		Private Shared currGroupID As ActivationGroupID
		''' <summary>
		''' the current group's activation system </summary>
		Private Shared currSystem As ActivationSystem
		''' <summary>
		''' used to control a group being created only once </summary>
		Private Shared canCreate As Boolean = True

		''' <summary>
		''' indicate compatibility with the Java 2 SDK v1.2 version of class </summary>
		Private Const serialVersionUID As Long = -7696947875314805420L

		''' <summary>
		''' Constructs an activation group with the given activation group
		''' identifier.  The group is exported as a
		''' <code>java.rmi.server.UnicastRemoteObject</code>.
		''' </summary>
		''' <param name="groupID"> the group's identifier </param>
		''' <exception cref="RemoteException"> if this group could not be exported </exception>
		''' <exception cref="UnsupportedOperationException"> if and only if activation is
		'''          not supported by this implementation
		''' @since   1.2 </exception>
		Protected Friend Sub New(  groupID As ActivationGroupID)
			' call super constructor to export the object
			MyBase.New()
			Me.groupID = groupID
		End Sub

		''' <summary>
		''' The group's <code>inactiveObject</code> method is called
		''' indirectly via a call to the <code>Activatable.inactive</code>
		''' method. A remote object implementation must call
		''' <code>Activatable</code>'s <code>inactive</code> method when
		''' that object deactivates (the object deems that it is no longer
		''' active). If the object does not call
		''' <code>Activatable.inactive</code> when it deactivates, the
		''' object will never be garbage collected since the group keeps
		''' strong references to the objects it creates.
		''' 
		''' <p>The group's <code>inactiveObject</code> method unexports the
		''' remote object from the RMI runtime so that the object can no
		''' longer receive incoming RMI calls. An object will only be unexported
		''' if the object has no pending or executing calls.
		''' The subclass of <code>ActivationGroup</code> must override this
		''' method and unexport the object.
		''' 
		''' <p>After removing the object from the RMI runtime, the group
		''' must inform its <code>ActivationMonitor</code> (via the monitor's
		''' <code>inactiveObject</code> method) that the remote object is
		''' not currently active so that the remote object will be
		''' re-activated by the activator upon a subsequent activation
		''' request.
		''' 
		''' <p>This method simply informs the group's monitor that the object
		''' is inactive.  It is up to the concrete subclass of ActivationGroup
		''' to fulfill the additional requirement of unexporting the object. <p>
		''' </summary>
		''' <param name="id"> the object's activation identifier </param>
		''' <returns> true if the object was successfully deactivated; otherwise
		'''         returns false. </returns>
		''' <exception cref="UnknownObjectException"> if object is unknown (may already
		''' be inactive) </exception>
		''' <exception cref="RemoteException"> if call informing monitor fails </exception>
		''' <exception cref="ActivationException"> if group is inactive
		''' @since 1.2 </exception>
		Public Overridable Function inactiveObject(  id As ActivationID) As Boolean
			monitor.inactiveObject(id)
			Return True
		End Function

		''' <summary>
		''' The group's <code>activeObject</code> method is called when an
		''' object is exported (either by <code>Activatable</code> object
		''' construction or an explicit call to
		''' <code>Activatable.exportObject</code>. The group must inform its
		''' <code>ActivationMonitor</code> that the object is active (via
		''' the monitor's <code>activeObject</code> method) if the group
		''' hasn't already done so.
		''' </summary>
		''' <param name="id"> the object's identifier </param>
		''' <param name="obj"> the remote object implementation </param>
		''' <exception cref="UnknownObjectException"> if object is not registered </exception>
		''' <exception cref="RemoteException"> if call informing monitor fails </exception>
		''' <exception cref="ActivationException"> if group is inactive
		''' @since 1.2 </exception>
		Public MustOverride Sub activeObject(  id As ActivationID,   obj As java.rmi.Remote)

		''' <summary>
		''' Create and set the activation group for the current VM.  The
		''' activation group can only be set if it is not currently set.
		''' An activation group is set using the <code>createGroup</code>
		''' method when the <code>Activator</code> initiates the
		''' re-creation of an activation group in order to carry out
		''' incoming <code>activate</code> requests. A group must first be
		''' registered with the <code>ActivationSystem</code> before it can
		''' be created via this method.
		''' 
		''' <p>The group class specified by the
		''' <code>ActivationGroupDesc</code> must be a concrete subclass of
		''' <code>ActivationGroup</code> and have a public constructor that
		''' takes two arguments: the <code>ActivationGroupID</code> for the
		''' group and the <code>MarshalledObject</code> containing the
		''' group's initialization data (obtained from the
		''' <code>ActivationGroupDesc</code>.
		''' 
		''' <p>If the group class name specified in the
		''' <code>ActivationGroupDesc</code> is <code>null</code>, then
		''' this method will behave as if the group descriptor contained
		''' the name of the default activation group implementation class.
		''' 
		''' <p>Note that if your application creates its own custom
		''' activation group, a security manager must be set for that
		''' group.  Otherwise objects cannot be activated in the group.
		''' <seealso cref="SecurityManager"/> is set by default.
		''' 
		''' <p>If a security manager is already set in the group VM, this
		''' method first calls the security manager's
		''' <code>checkSetFactory</code> method.  This could result in a
		''' <code>SecurityException</code>. If your application needs to
		''' set a different security manager, you must ensure that the
		''' policy file specified by the group's
		''' <code>ActivationGroupDesc</code> grants the group the necessary
		''' permissions to set a new security manager.  (Note: This will be
		''' necessary if your group downloads and sets a security manager).
		''' 
		''' <p>After the group is created, the
		''' <code>ActivationSystem</code> is informed that the group is
		''' active by calling the <code>activeGroup</code> method which
		''' returns the <code>ActivationMonitor</code> for the group. The
		''' application need not call <code>activeGroup</code>
		''' independently since it is taken care of by this method.
		''' 
		''' <p>Once a group is created, subsequent calls to the
		''' <code>currentGroupID</code> method will return the identifier
		''' for this group until the group becomes inactive.
		''' </summary>
		''' <param name="id"> the activation group's identifier </param>
		''' <param name="desc"> the activation group's descriptor </param>
		''' <param name="incarnation"> the group's incarnation number (zero on group's
		''' initial creation) </param>
		''' <returns> the activation group for the VM </returns>
		''' <exception cref="ActivationException"> if group already exists or if error
		''' occurs during group creation </exception>
		''' <exception cref="SecurityException"> if permission to create group is denied.
		''' (Note: The default implementation of the security manager
		''' <code>checkSetFactory</code>
		''' method requires the RuntimePermission "setFactory") </exception>
		''' <exception cref="UnsupportedOperationException"> if and only if activation is
		''' not supported by this implementation </exception>
		''' <seealso cref= SecurityManager#checkSetFactory
		''' @since 1.2 </seealso>
		<MethodImpl(MethodImplOptions.Synchronized)> _
		Public Shared Function createGroup(  id As ActivationGroupID,   desc As ActivationGroupDesc,   incarnation As Long) As ActivationGroup
			Dim security As SecurityManager = System.securityManager
			If security IsNot Nothing Then security.checkSetFactory()

			If currGroup IsNot Nothing Then Throw New ActivationException("group already exists")

			If canCreate = False Then Throw New ActivationException("group deactivated and " & "cannot be recreated")

			Try
				' load group's class
				Dim groupClassName As String = desc.className
				Dim cl As  [Class]
				Dim defaultGroupClass As  [Class] = GetType(sun.rmi.server.ActivationGroupImpl)
				If groupClassName Is Nothing OrElse groupClassName.Equals(defaultGroupClass.name) Then ' see 4252236
					cl = defaultGroupClass
				Else
					Dim cl0 As  [Class]
					Try
						cl0 = java.rmi.server.RMIClassLoader.loadClass(desc.location, groupClassName)
					Catch ex As Exception
						Throw New ActivationException("Could not load group implementation class", ex)
					End Try
					If cl0.IsSubclassOf(GetType(ActivationGroup)) Then
						cl = cl0.asSubclass(GetType(ActivationGroup))
					Else
						Throw New ActivationException("group not correct class: " & cl0.name)
					End If
				End If

				' create group
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
				Dim constructor As Constructor(Of ? As ActivationGroup) = cl.getConstructor(GetType(ActivationGroupID), GetType(java.rmi.MarshalledObject))
				Dim newGroup As ActivationGroup = constructor.newInstance(id, desc.data)
				currSystem = id.system
				newGroup.incarnation = incarnation
				newGroup.monitor = currSystem.activeGroup(id, newGroup, incarnation)
				currGroup = newGroup
				currGroupID = id
				canCreate = False
			Catch e As InvocationTargetException
					e.targetException.printStackTrace()
					Throw New ActivationException("exception in group constructor", e.targetException)

			Catch e As ActivationException
				Throw e

			Catch e As Exception
				Throw New ActivationException("exception creating group", e)
			End Try

			Return currGroup
		End Function

		''' <summary>
		''' Returns the current activation group's identifier.  Returns null
		''' if no group is currently active for this VM. </summary>
		''' <exception cref="UnsupportedOperationException"> if and only if activation is
		''' not supported by this implementation </exception>
		''' <returns> the activation group's identifier
		''' @since 1.2 </returns>
		<MethodImpl(MethodImplOptions.Synchronized)> _
		Public Shared Function currentGroupID() As ActivationGroupID
			Return currGroupID
		End Function

		''' <summary>
		''' Returns the activation group identifier for the VM.  If an
		''' activation group does not exist for this VM, a default
		''' activation group is created. A group can be created only once,
		''' so if a group has already become active and deactivated.
		''' </summary>
		''' <returns> the activation group identifier </returns>
		''' <exception cref="ActivationException"> if error occurs during group
		''' creation, if security manager is not set, or if the group
		''' has already been created and deactivated. </exception>
		<MethodImpl(MethodImplOptions.Synchronized)> _
		Friend Shared Function internalCurrentGroupID() As ActivationGroupID
			If currGroupID Is Nothing Then Throw New ActivationException("nonexistent group")

			Return currGroupID
		End Function

		''' <summary>
		''' Set the activation system for the VM.  The activation system can
		''' only be set it if no group is currently active. If the activation
		''' system is not set via this call, then the <code>getSystem</code>
		''' method attempts to obtain a reference to the
		''' <code>ActivationSystem</code> by looking up the name
		''' "java.rmi.activation.ActivationSystem" in the Activator's
		''' registry. By default, the port number used to look up the
		''' activation system is defined by
		''' <code>ActivationSystem.SYSTEM_PORT</code>. This port can be overridden
		''' by setting the property <code>java.rmi.activation.port</code>.
		''' 
		''' <p>If there is a security manager, this method first
		''' calls the security manager's <code>checkSetFactory</code> method.
		''' This could result in a SecurityException.
		''' </summary>
		''' <param name="system"> remote reference to the <code>ActivationSystem</code> </param>
		''' <exception cref="ActivationException"> if activation system is already set </exception>
		''' <exception cref="SecurityException"> if permission to set the activation system is denied.
		''' (Note: The default implementation of the security manager
		''' <code>checkSetFactory</code>
		''' method requires the RuntimePermission "setFactory") </exception>
		''' <exception cref="UnsupportedOperationException"> if and only if activation is
		''' not supported by this implementation </exception>
		''' <seealso cref= #getSystem </seealso>
		''' <seealso cref= SecurityManager#checkSetFactory
		''' @since 1.2 </seealso>
		<MethodImpl(MethodImplOptions.Synchronized)> _
		Public Shared Property system As ActivationSystem
			Set(  system_Renamed As ActivationSystem)
				Dim security As SecurityManager = System.securityManager
				If security IsNot Nothing Then security.checkSetFactory()
    
				If currSystem IsNot Nothing Then Throw New ActivationException("activation system already set")
    
				currSystem = system_Renamed
			End Set
			Get
				If currSystem Is Nothing Then
					Try
						Dim port As Integer = java.security.AccessController.doPrivileged(New sun.security.action.GetIntegerAction("java.rmi.activation.port", ActivationSystem.SYSTEM_PORT))
						currSystem = CType(java.rmi.Naming.lookup("//:" & port & "/java.rmi.activation.ActivationSystem"), ActivationSystem)
					Catch e As Exception
						Throw New ActivationException("unable to obtain ActivationSystem", e)
					End Try
				End If
				Return currSystem
			End Get
		End Property


		''' <summary>
		''' This protected method is necessary for subclasses to
		''' make the <code>activeObject</code> callback to the group's
		''' monitor. The call is simply forwarded to the group's
		''' <code>ActivationMonitor</code>.
		''' </summary>
		''' <param name="id"> the object's identifier </param>
		''' <param name="mobj"> a marshalled object containing the remote object's stub </param>
		''' <exception cref="UnknownObjectException"> if object is not registered </exception>
		''' <exception cref="RemoteException"> if call informing monitor fails </exception>
		''' <exception cref="ActivationException"> if an activation error occurs
		''' @since 1.2 </exception>
		Protected Friend Overridable Sub activeObject(Of T1 As java.rmi.Remote)(  id As ActivationID,   mobj As java.rmi.MarshalledObject(Of T1))
			monitor.activeObject(id, mobj)
		End Sub

		''' <summary>
		''' This protected method is necessary for subclasses to
		''' make the <code>inactiveGroup</code> callback to the group's
		''' monitor. The call is simply forwarded to the group's
		''' <code>ActivationMonitor</code>. Also, the current group
		''' for the VM is set to null.
		''' </summary>
		''' <exception cref="UnknownGroupException"> if group is not registered </exception>
		''' <exception cref="RemoteException"> if call informing monitor fails
		''' @since 1.2 </exception>
		Protected Friend Overridable Sub inactiveGroup()
			Try
				monitor.inactiveGroup(groupID, incarnation)
			Finally
				destroyGroup()
			End Try
		End Sub

		''' <summary>
		''' Returns the monitor for the activation group.
		''' </summary>
		Private Property monitor As ActivationMonitor
			Get
				SyncLock GetType(ActivationGroup)
					If monitor IsNot Nothing Then Return monitor
				End SyncLock
				Throw New java.rmi.RemoteException("monitor not received")
			End Get
		End Property

		''' <summary>
		''' Destroys the current group.
		''' </summary>
		<MethodImpl(MethodImplOptions.Synchronized)> _
		Private Shared Sub destroyGroup()
			currGroup = Nothing
			currGroupID = Nothing
			' NOTE: don't set currSystem to null since it may be needed
		End Sub

		''' <summary>
		''' Returns the current group for the VM. </summary>
		''' <exception cref="ActivationException"> if current group is null (not active) </exception>
		<MethodImpl(MethodImplOptions.Synchronized)> _
		Shared Function currentGroup() As ActivationGroup
			If currGroup Is Nothing Then Throw New ActivationException("group is not active")
			Return currGroup
		End Function

	End Class

End Namespace