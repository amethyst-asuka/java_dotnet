Imports System.Collections.Generic

'
' * Copyright (c) 2002, 2007, Oracle and/or its affiliates. All rights reserved.
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
	''' <p>An <seealso cref="RMIServer"/> object that is exported through JRMP and that
	''' creates client connections as RMI objects exported through JRMP.
	''' User code does not usually reference this class directly.</p>
	''' </summary>
	''' <seealso cref= RMIServerImpl
	''' 
	''' @since 1.5 </seealso>
	Public Class RMIJRMPServerImpl
		Inherits RMIServerImpl

		''' <summary>
		''' <p>Creates a new <seealso cref="RMIServer"/> object that will be exported
		''' on the given port using the given socket factories.</p>
		''' </summary>
		''' <param name="port"> the port on which this object and the {@link
		''' RMIConnectionImpl} objects it creates will be exported.  Can be
		''' zero, to indicate any available port.
		''' </param>
		''' <param name="csf"> the client socket factory for the created RMI
		''' objects.  Can be null.
		''' </param>
		''' <param name="ssf"> the server socket factory for the created RMI
		''' objects.  Can be null.
		''' </param>
		''' <param name="env"> the environment map.  Can be null.
		''' </param>
		''' <exception cref="IOException"> if the <seealso cref="RMIServer"/> object
		''' cannot be created.
		''' </exception>
		''' <exception cref="IllegalArgumentException"> if <code>port</code> is
		''' negative. </exception>
		Public Sub New(Of T1)(ByVal port As Integer, ByVal csf As java.rmi.server.RMIClientSocketFactory, ByVal ssf As java.rmi.server.RMIServerSocketFactory, ByVal env As IDictionary(Of T1))

			MyBase.New(env)

			If port < 0 Then Throw New System.ArgumentException("Negative port: " & port)

			Me.port = port
			Me.csf = csf
			Me.ssf = ssf
			Me.env = If(env Is Nothing, java.util.Collections.emptyMap(Of String, Object)(), env)
		End Sub

		Protected Friend Overrides Sub export()
			export(Me)
		End Sub

		Private Sub export(ByVal obj As java.rmi.Remote)
			Dim exporter As com.sun.jmx.remote.internal.RMIExporter = CType(env(com.sun.jmx.remote.internal.RMIExporter.EXPORTER_ATTRIBUTE), com.sun.jmx.remote.internal.RMIExporter)
			Dim daemon As Boolean = com.sun.jmx.remote.util.EnvHelp.isServerDaemon(env)

			If daemon AndAlso exporter IsNot Nothing Then Throw New System.ArgumentException("If " & com.sun.jmx.remote.util.EnvHelp.JMX_SERVER_DAEMON & " is specified as true, " & com.sun.jmx.remote.internal.RMIExporter.EXPORTER_ATTRIBUTE & " cannot be used to specify an exporter!")

			If daemon Then
				If csf Is Nothing AndAlso ssf Is Nothing Then
					CType(New sun.rmi.server.UnicastServerRef(port), sun.rmi.server.UnicastServerRef).exportObject(obj, Nothing, True)
				Else
					CType(New sun.rmi.server.UnicastServerRef2(port, csf, ssf), sun.rmi.server.UnicastServerRef2).exportObject(obj, Nothing, True)
				End If
			ElseIf exporter IsNot Nothing Then
				exporter.exportObject(obj, port, csf, ssf)
			Else
				java.rmi.server.UnicastRemoteObject.exportObject(obj, port, csf, ssf)
			End If
		End Sub

		Private Sub unexport(ByVal obj As java.rmi.Remote, ByVal force As Boolean)
			Dim exporter As com.sun.jmx.remote.internal.RMIExporter = CType(env(com.sun.jmx.remote.internal.RMIExporter.EXPORTER_ATTRIBUTE), com.sun.jmx.remote.internal.RMIExporter)
			If exporter Is Nothing Then
				java.rmi.server.UnicastRemoteObject.unexportObject(obj, force)
			Else
				exporter.unexportObject(obj, force)
			End If
		End Sub

		Protected Friend Property Overrides protocol As String
			Get
				Return "rmi"
			End Get
		End Property

		''' <summary>
		''' <p>Returns a serializable stub for this <seealso cref="RMIServer"/> object.</p>
		''' </summary>
		''' <returns> a serializable stub.
		''' </returns>
		''' <exception cref="IOException"> if the stub cannot be obtained - e.g the
		'''            RMIJRMPServerImpl has not been exported yet. </exception>
		Public Overrides Function toStub() As java.rmi.Remote
			Return java.rmi.server.RemoteObject.toStub(Me)
		End Function

		''' <summary>
		''' <p>Creates a new client connection as an RMI object exported
		''' through JRMP. The port and socket factories for the new
		''' <seealso cref="RMIConnection"/> object are the ones supplied
		''' to the <code>RMIJRMPServerImpl</code> constructor.</p>
		''' </summary>
		''' <param name="connectionId"> the ID of the new connection. Every
		''' connection opened by this connector server will have a
		''' different id.  The behavior is unspecified if this parameter is
		''' null.
		''' </param>
		''' <param name="subject"> the authenticated subject.  Can be null.
		''' </param>
		''' <returns> the newly-created <code>RMIConnection</code>.
		''' </returns>
		''' <exception cref="IOException"> if the new <seealso cref="RMIConnection"/>
		''' object cannot be created or exported. </exception>
		Protected Friend Overrides Function makeClient(ByVal connectionId As String, ByVal subject As javax.security.auth.Subject) As RMIConnection

			If connectionId Is Nothing Then Throw New NullPointerException("Null connectionId")

			Dim client As RMIConnection = New RMIConnectionImpl(Me, connectionId, defaultClassLoader, subject, env)
			export(client)
			Return client
		End Function

		Protected Friend Overrides Sub closeClient(ByVal client As RMIConnection)
			unexport(client, True)
		End Sub

		''' <summary>
		''' <p>Called by <seealso cref="#close()"/> to close the connector server by
		''' unexporting this object.  After returning from this method, the
		''' connector server must not accept any new connections.</p>
		''' </summary>
		''' <exception cref="IOException"> if the attempt to close the connector
		''' server failed. </exception>
		Protected Friend Overrides Sub closeServer()
			unexport(Me, True)
		End Sub

		Private ReadOnly port As Integer
		Private ReadOnly csf As java.rmi.server.RMIClientSocketFactory
		Private ReadOnly ssf As java.rmi.server.RMIServerSocketFactory
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
		Private ReadOnly env As IDictionary(Of String, ?)
	End Class

End Namespace