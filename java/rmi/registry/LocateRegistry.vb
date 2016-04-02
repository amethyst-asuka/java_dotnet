Imports System

'
' * Copyright (c) 1996, 2003, Oracle and/or its affiliates. All rights reserved.
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

Namespace java.rmi.registry


	''' <summary>
	''' <code>LocateRegistry</code> is used to obtain a reference to a bootstrap
	''' remote object registry on a particular host (including the local host), or
	''' to create a remote object registry that accepts calls on a specific port.
	''' 
	''' <p> Note that a <code>getRegistry</code> call does not actually make a
	''' connection to the remote host.  It simply creates a local reference to
	''' the remote registry and will succeed even if no registry is running on
	''' the remote host.  Therefore, a subsequent method invocation to a remote
	''' registry returned as a result of this method may fail.
	''' 
	''' @author  Ann Wollrath
	''' @author  Peter Jones
	''' @since   JDK1.1 </summary>
	''' <seealso cref=     java.rmi.registry.Registry </seealso>
	Public NotInheritable Class LocateRegistry

		''' <summary>
		''' Private constructor to disable public construction.
		''' </summary>
		Private Sub New()
		End Sub

		''' <summary>
		''' Returns a reference to the the remote object <code>Registry</code> for
		''' the local host on the default registry port of 1099.
		''' </summary>
		''' <returns> reference (a stub) to the remote object registry </returns>
		''' <exception cref="RemoteException"> if the reference could not be created
		''' @since JDK1.1 </exception>
		PublicShared ReadOnly Propertyregistry As Registry
			Get
				Return getRegistry(Nothing, Registry.REGISTRY_PORT)
			End Get
		End Property

		''' <summary>
		''' Returns a reference to the the remote object <code>Registry</code> for
		''' the local host on the specified <code>port</code>.
		''' </summary>
		''' <param name="port"> port on which the registry accepts requests </param>
		''' <returns> reference (a stub) to the remote object registry </returns>
		''' <exception cref="RemoteException"> if the reference could not be created
		''' @since JDK1.1 </exception>
		Public Shared Function getRegistry(ByVal port As Integer) As Registry
			Return getRegistry(Nothing, port)
		End Function

		''' <summary>
		''' Returns a reference to the remote object <code>Registry</code> on the
		''' specified <code>host</code> on the default registry port of 1099.  If
		''' <code>host</code> is <code>null</code>, the local host is used.
		''' </summary>
		''' <param name="host"> host for the remote registry </param>
		''' <returns> reference (a stub) to the remote object registry </returns>
		''' <exception cref="RemoteException"> if the reference could not be created
		''' @since JDK1.1 </exception>
		Public Shared Function getRegistry(ByVal host As String) As Registry
			Return getRegistry(host, Registry.REGISTRY_PORT)
		End Function

		''' <summary>
		''' Returns a reference to the remote object <code>Registry</code> on the
		''' specified <code>host</code> and <code>port</code>. If <code>host</code>
		''' is <code>null</code>, the local host is used.
		''' </summary>
		''' <param name="host"> host for the remote registry </param>
		''' <param name="port"> port on which the registry accepts requests </param>
		''' <returns> reference (a stub) to the remote object registry </returns>
		''' <exception cref="RemoteException"> if the reference could not be created
		''' @since JDK1.1 </exception>
		Public Shared Function getRegistry(ByVal host As String, ByVal port As Integer) As Registry
			Return getRegistry(host, port, Nothing)
		End Function

		''' <summary>
		''' Returns a locally created remote reference to the remote object
		''' <code>Registry</code> on the specified <code>host</code> and
		''' <code>port</code>.  Communication with this remote registry will
		''' use the supplied <code>RMIClientSocketFactory</code> <code>csf</code>
		''' to create <code>Socket</code> connections to the registry on the
		''' remote <code>host</code> and <code>port</code>.
		''' </summary>
		''' <param name="host"> host for the remote registry </param>
		''' <param name="port"> port on which the registry accepts requests </param>
		''' <param name="csf">  client-side <code>Socket</code> factory used to
		'''      make connections to the registry.  If <code>csf</code>
		'''      is null, then the default client-side <code>Socket</code>
		'''      factory will be used in the registry stub. </param>
		''' <returns> reference (a stub) to the remote registry </returns>
		''' <exception cref="RemoteException"> if the reference could not be created
		''' @since 1.2 </exception>
		Public Shared Function getRegistry(ByVal host As String, ByVal port As Integer, ByVal csf As java.rmi.server.RMIClientSocketFactory) As Registry
			Dim registry_Renamed As Registry = Nothing

			If port <= 0 Then port = Registry.REGISTRY_PORT

			If host Is Nothing OrElse host.length() = 0 Then
				' If host is blank (as returned by "file:" URL in 1.0.2 used in
				' java.rmi.Naming), try to convert to real local host name so
				' that the RegistryImpl's checkAccess will not fail.
				Try
					host = java.net.InetAddress.localHost.hostAddress
				Catch e As Exception
					' If that failed, at least try "" (localhost) anyway...
					host = ""
				End Try
			End If

	'        
	'         * Create a proxy for the registry with the given host, port, and
	'         * client socket factory.  If the supplied client socket factory is
	'         * null, then the ref type is a UnicastRef, otherwise the ref type
	'         * is a UnicastRef2.  If the property
	'         * java.rmi.server.ignoreStubClasses is true, then the proxy
	'         * returned is an instance of a dynamic proxy class that implements
	'         * the Registry interface; otherwise the proxy returned is an
	'         * instance of the pregenerated stub class for RegistryImpl.
	'         *
			Dim liveRef As New sun.rmi.transport.LiveRef(New java.rmi.server.ObjID(java.rmi.server.ObjID.REGISTRY_ID), New sun.rmi.transport.tcp.TCPEndpoint(host, port, csf, Nothing), False)
			Dim ref As java.rmi.server.RemoteRef = If(csf Is Nothing, New sun.rmi.server.UnicastRef(liveRef), New sun.rmi.server.UnicastRef2(liveRef))

			Return CType(sun.rmi.server.Util.createProxy(GetType(sun.rmi.registry.RegistryImpl), ref, False), Registry)
		End Function

		''' <summary>
		''' Creates and exports a <code>Registry</code> instance on the local
		''' host that accepts requests on the specified <code>port</code>.
		''' 
		''' <p>The <code>Registry</code> instance is exported as if the static
		''' {@link UnicastRemoteObject#exportObject(Remote,int)
		''' UnicastRemoteObject.exportObject} method is invoked, passing the
		''' <code>Registry</code> instance and the specified <code>port</code> as
		''' arguments, except that the <code>Registry</code> instance is
		''' exported with a well-known object identifier, an <seealso cref="ObjID"/>
		''' instance constructed with the value <seealso cref="ObjID#REGISTRY_ID"/>.
		''' </summary>
		''' <param name="port"> the port on which the registry accepts requests </param>
		''' <returns> the registry </returns>
		''' <exception cref="RemoteException"> if the registry could not be exported
		''' @since JDK1.1
		'''  </exception>
		Public Shared Function createRegistry(ByVal port As Integer) As Registry
			Return New sun.rmi.registry.RegistryImpl(port)
		End Function

		''' <summary>
		''' Creates and exports a <code>Registry</code> instance on the local
		''' host that uses custom socket factories for communication with that
		''' instance.  The registry that is created listens for incoming
		''' requests on the given <code>port</code> using a
		''' <code>ServerSocket</code> created from the supplied
		''' <code>RMIServerSocketFactory</code>.
		''' 
		''' <p>The <code>Registry</code> instance is exported as if
		''' the static {@link
		''' UnicastRemoteObject#exportObject(Remote,int,RMIClientSocketFactory,RMIServerSocketFactory)
		''' UnicastRemoteObject.exportObject} method is invoked, passing the
		''' <code>Registry</code> instance, the specified <code>port</code>, the
		''' specified <code>RMIClientSocketFactory</code>, and the specified
		''' <code>RMIServerSocketFactory</code> as arguments, except that the
		''' <code>Registry</code> instance is exported with a well-known object
		''' identifier, an <seealso cref="ObjID"/> instance constructed with the value
		''' <seealso cref="ObjID#REGISTRY_ID"/>.
		''' </summary>
		''' <param name="port"> port on which the registry accepts requests </param>
		''' <param name="csf">  client-side <code>Socket</code> factory used to
		'''      make connections to the registry </param>
		''' <param name="ssf">  server-side <code>ServerSocket</code> factory
		'''      used to accept connections to the registry </param>
		''' <returns> the registry </returns>
		''' <exception cref="RemoteException"> if the registry could not be exported
		''' @since 1.2
		'''  </exception>
		Public Shared Function createRegistry(ByVal port As Integer, ByVal csf As java.rmi.server.RMIClientSocketFactory, ByVal ssf As java.rmi.server.RMIServerSocketFactory) As Registry
			Return New sun.rmi.registry.RegistryImpl(port, csf, ssf)
		End Function
	End Class

End Namespace