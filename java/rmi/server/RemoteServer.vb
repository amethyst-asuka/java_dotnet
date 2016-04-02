'
' * Copyright (c) 1996, 2002, Oracle and/or its affiliates. All rights reserved.
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
Namespace java.rmi.server


	''' <summary>
	''' The <code>RemoteServer</code> class is the common superclass to server
	''' implementations and provides the framework to support a wide range
	''' of remote reference semantics.  Specifically, the functions needed
	''' to create and export remote objects (i.e. to make them remotely
	''' available) are provided abstractly by <code>RemoteServer</code> and
	''' concretely by its subclass(es).
	''' 
	''' @author  Ann Wollrath
	''' @since   JDK1.1
	''' </summary>
	Public MustInherit Class RemoteServer
		Inherits RemoteObject

		' indicate compatibility with JDK 1.1.x version of class 
		Private Const serialVersionUID As Long = -4100238210092549637L

		''' <summary>
		''' Constructs a <code>RemoteServer</code>.
		''' @since JDK1.1
		''' </summary>
		Protected Friend Sub New()
			MyBase.New()
		End Sub

		''' <summary>
		''' Constructs a <code>RemoteServer</code> with the given reference type.
		''' </summary>
		''' <param name="ref"> the remote reference
		''' @since JDK1.1 </param>
		Protected Friend Sub New(ByVal ref As RemoteRef)
			MyBase.New(ref)
		End Sub

		''' <summary>
		''' Returns a string representation of the client host for the
		''' remote method invocation being processed in the current thread.
		''' </summary>
		''' <returns>  a string representation of the client host
		''' </returns>
		''' <exception cref="ServerNotActiveException"> if no remote method invocation
		''' is being processed in the current thread
		''' 
		''' @since   JDK1.1 </exception>
		PublicShared ReadOnly PropertyclientHost As String
			Get
				Return sun.rmi.transport.tcp.TCPTransport.clientHost
			End Get
		End Property

		''' <summary>
		''' Log RMI calls to the output stream <code>out</code>. If
		''' <code>out</code> is <code>null</code>, call logging is turned off.
		''' 
		''' <p>If there is a security manager, its
		''' <code>checkPermission</code> method will be invoked with a
		''' <code>java.util.logging.LoggingPermission("control")</code>
		''' permission; this could result in a <code>SecurityException</code>.
		''' </summary>
		''' <param name="out"> the output stream to which RMI calls should be logged </param>
		''' <exception cref="SecurityException">  if there is a security manager and
		'''          the invocation of its <code>checkPermission</code> method
		'''          fails </exception>
		''' <seealso cref= #getLog
		''' @since JDK1.1 </seealso>
		Public Shared Property log As java.io.OutputStream
			Set(ByVal out As java.io.OutputStream)
				logNull = (out Is Nothing)
				sun.rmi.server.UnicastServerRef.callLog.outputStream = out
			End Set
			Get
				Return (If(logNull, Nothing, sun.rmi.server.UnicastServerRef.callLog.printStream))
			End Get
		End Property


		' initialize log status
		Private Shared logNull As Boolean = Not sun.rmi.server.UnicastServerRef.logCalls
	End Class

End Namespace