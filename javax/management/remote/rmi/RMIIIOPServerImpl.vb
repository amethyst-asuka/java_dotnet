Imports System.Collections.Generic

'
' * Copyright (c) 2003, 2007, Oracle and/or its affiliates. All rights reserved.
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

Namespace javax.management.remote.rmi



	''' <summary>
	''' <p>An <seealso cref="RMIServerImpl"/> that is exported through IIOP and that
	''' creates client connections as RMI objects exported through IIOP.
	''' User code does not usually reference this class directly.</p>
	''' </summary>
	''' <seealso cref= RMIServerImpl
	''' 
	''' @since 1.5 </seealso>
	Public Class RMIIIOPServerImpl
		Inherits RMIServerImpl

		''' <summary>
		''' <p>Creates a new <seealso cref="RMIServerImpl"/>.</p>
		''' </summary>
		''' <param name="env"> the environment containing attributes for the new
		''' <code>RMIServerImpl</code>.  Can be null, which is equivalent
		''' to an empty Map.
		''' </param>
		''' <exception cref="IOException"> if the RMI object cannot be created. </exception>
		Public Sub New(Of T1)(ByVal env As IDictionary(Of T1))
			MyBase.New(env)

			Me.env = If(env Is Nothing, java.util.Collections.emptyMap(Of String, Object)(), env)

			callerACC = java.security.AccessController.context
		End Sub

		Protected Friend Overrides Sub export()
			com.sun.jmx.remote.internal.IIOPHelper.exportObject(Me)
		End Sub

		Protected Friend Property Overrides protocol As String
			Get
				Return "iiop"
			End Get
		End Property

		''' <summary>
		''' <p>Returns an IIOP stub.</p>
		''' The stub might not yet be connected to the ORB. The stub will
		''' be serializable only if it is connected to the ORB. </summary>
		''' <returns> an IIOP stub. </returns>
		''' <exception cref="IOException"> if the stub cannot be created - e.g the
		'''            RMIIIOPServerImpl has not been exported yet.
		'''  </exception>
		Public Overrides Function toStub() As java.rmi.Remote
			' javax.rmi.CORBA.Stub stub =
			'    (javax.rmi.CORBA.Stub) PortableRemoteObject.toStub(this);
			Dim stub As java.rmi.Remote = com.sun.jmx.remote.internal.IIOPHelper.toStub(Me)
			' java.lang.System.out.println("NON CONNECTED STUB " + stub);
			' org.omg.CORBA.ORB orb =
			'    org.omg.CORBA.ORB.init((String[])null, (Properties)null);
			' stub.connect(orb);
			' java.lang.System.out.println("CONNECTED STUB " + stub);
			Return stub
		End Function

		''' <summary>
		''' <p>Creates a new client connection as an RMI object exported
		''' through IIOP.
		''' </summary>
		''' <param name="connectionId"> the ID of the new connection.  Every
		''' connection opened by this connector server will have a
		''' different ID.  The behavior is unspecified if this parameter is
		''' null.
		''' </param>
		''' <param name="subject"> the authenticated subject.  Can be null.
		''' </param>
		''' <returns> the newly-created <code>RMIConnection</code>.
		''' </returns>
		''' <exception cref="IOException"> if the new client object cannot be
		''' created or exported. </exception>
		Protected Friend Overrides Function makeClient(ByVal connectionId As String, ByVal subject As javax.security.auth.Subject) As RMIConnection

			If connectionId Is Nothing Then Throw New NullPointerException("Null connectionId")

			Dim client As RMIConnection = New RMIConnectionImpl(Me, connectionId, defaultClassLoader, subject, env)
			com.sun.jmx.remote.internal.IIOPHelper.exportObject(client)
			Return client
		End Function

		Protected Friend Overrides Sub closeClient(ByVal client As RMIConnection)
			com.sun.jmx.remote.internal.IIOPHelper.unexportObject(client)
		End Sub

		''' <summary>
		''' <p>Called by <seealso cref="#close()"/> to close the connector server by
		''' unexporting this object.  After returning from this method, the
		''' connector server must not accept any new connections.</p>
		''' </summary>
		''' <exception cref="IOException"> if the attempt to close the connector
		''' server failed. </exception>
		Protected Friend Overrides Sub closeServer()
			com.sun.jmx.remote.internal.IIOPHelper.unexportObject(Me)
		End Sub

		Friend Overrides Function doNewClient(ByVal credentials As Object) As RMIConnection
			If callerACC Is Nothing Then Throw New SecurityException("AccessControlContext cannot be null")
			Try
'JAVA TO VB CONVERTER TODO TASK: Anonymous inner classes are not converted to VB if the base type is not defined in the code being converted:
'				Return java.security.AccessController.doPrivileged(New java.security.PrivilegedExceptionAction<RMIConnection>()
	'			{
	'					public RMIConnection run() throws IOException
	'					{
	'						Return superDoNewClient(credentials);
	'					}
	'			}, callerACC);
			Catch pae As java.security.PrivilegedActionException
				Throw CType(pae.InnerException, java.io.IOException)
			End Try
		End Function

		Friend Overridable Function superDoNewClient(ByVal credentials As Object) As RMIConnection
			Return MyBase.doNewClient(credentials)
		End Function

'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
		Private ReadOnly env As IDictionary(Of String, ?)
		Private ReadOnly callerACC As java.security.AccessControlContext
	End Class

End Namespace