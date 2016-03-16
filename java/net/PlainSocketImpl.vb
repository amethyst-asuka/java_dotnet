Imports System
Imports System.Runtime.CompilerServices
Imports System.Diagnostics

'
' * Copyright (c) 2007, 2013, Oracle and/or its affiliates. All rights reserved.
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
Namespace java.net


	'
	' * This class PlainSocketImpl simply delegates to the appropriate real
	' * SocketImpl. We do this because PlainSocketImpl is already extended
	' * by SocksSocketImpl.
	' * <p>
	' * There are two possibilities for the real SocketImpl,
	' * TwoStacksPlainSocketImpl or DualStackPlainSocketImpl. We use
	' * DualStackPlainSocketImpl on systems that have a dual stack
	' * TCP implementation. Otherwise we create an instance of
	' * TwoStacksPlainSocketImpl and delegate to it.
	' *
	' * @author Chris Hegarty
	' 

	Friend Class PlainSocketImpl
		Inherits AbstractPlainSocketImpl

		Private impl As AbstractPlainSocketImpl

		' the windows version. 
		Private Shared version As Single

		' java.net.preferIPv4Stack 
		Private Shared preferIPv4Stack As Boolean = False

		' If the version supports a dual stack TCP implementation 
		Private Shared useDualStackImpl As Boolean = False

		' sun.net.useExclusiveBind 
		Private Shared exclBindProp As String

		' True if exclusive binding is on for Windows 
		Private Shared exclusiveBind As Boolean = True

		Shared Sub New()
			java.security.AccessController.doPrivileged(New PrivilegedActionAnonymousInnerClassHelper(Of T)

			' (version >= 6.0) implies Vista or greater.
			If version >= 6.0 AndAlso (Not preferIPv4Stack) Then useDualStackImpl = True

			If exclBindProp IsNot Nothing Then
				' sun.net.useExclusiveBind is true
				exclusiveBind = If(exclBindProp.length() = 0, True, Convert.ToBoolean(exclBindProp))
			ElseIf version < 6.0 Then
				exclusiveBind = False
			End If
		End Sub

		Private Class PrivilegedActionAnonymousInnerClassHelper(Of T)
			Implements java.security.PrivilegedAction(Of T)

			Public Overridable Function run() As Object
				version = 0
				Try
					version = Convert.ToSingle(System.properties.getProperty("os.version"))
					preferIPv4Stack = Convert.ToBoolean(System.properties.getProperty("java.net.preferIPv4Stack"))
					exclBindProp = System.getProperty("sun.net.useExclusiveBind")
				Catch e As NumberFormatException
					Debug.Assert(False, e)
				End Try
				Return Nothing ' nothing to return
			End Function
		End Class

		''' <summary>
		''' Constructs an empty instance.
		''' </summary>
		Friend Sub New()
			If useDualStackImpl Then
				impl = New DualStackPlainSocketImpl(exclusiveBind)
			Else
				impl = New TwoStacksPlainSocketImpl(exclusiveBind)
			End If
		End Sub

		''' <summary>
		''' Constructs an instance with the given file descriptor.
		''' </summary>
		Friend Sub New(ByVal fd As FileDescriptor)
			If useDualStackImpl Then
				impl = New DualStackPlainSocketImpl(fd, exclusiveBind)
			Else
				impl = New TwoStacksPlainSocketImpl(fd, exclusiveBind)
			End If
		End Sub

		' Override methods in SocketImpl that access impl's fields.

		Protected Friend Property Overrides fileDescriptor As FileDescriptor
			Get
				Return impl.fileDescriptor
			End Get
		End Property

		Protected Friend Property Overrides inetAddress As InetAddress
			Get
				Return impl.inetAddress
			End Get
		End Property

		Protected Friend Property Overrides port As Integer
			Get
				Return impl.port
			End Get
		End Property

		Protected Friend Property Overrides localPort As Integer
			Get
				Return impl.localPort
			End Get
		End Property

		Friend Overrides Property socket As Socket
			Set(ByVal soc As Socket)
				impl.socket = soc
			End Set
			Get
				Return impl.socket
			End Get
		End Property


		Friend Overrides Property serverSocket As ServerSocket
			Set(ByVal soc As ServerSocket)
				impl.serverSocket = soc
			End Set
			Get
				Return impl.serverSocket
			End Get
		End Property


		Public Overrides Function ToString() As String
			Return impl.ToString()
		End Function

		' Override methods in AbstractPlainSocketImpl that access impl's fields.

		<MethodImpl(MethodImplOptions.Synchronized)> _
		Protected Friend Overrides Sub create(ByVal stream As Boolean)
			impl.create(stream)

			' set fd to delegate's fd to be compatible with older releases
			Me.fd = impl.fd
		End Sub

		Protected Friend Overrides Sub connect(ByVal host As String, ByVal port As Integer)
			impl.connect(host, port)
		End Sub

		Protected Friend Overrides Sub connect(ByVal address As InetAddress, ByVal port As Integer)
			impl.connect(address, port)
		End Sub

		Protected Friend Overrides Sub connect(ByVal address As SocketAddress, ByVal timeout As Integer)
			impl.connect(address, timeout)
		End Sub

		Public Overrides Sub setOption(ByVal opt As Integer, ByVal val As Object)
			impl.optionion(opt, val)
		End Sub

		Public Overrides Function getOption(ByVal opt As Integer) As Object
			Return impl.getOption(opt)
		End Function

		SyncLock  Sub  doConnect InetAddress address, Integer port, Integer timeout
			Dim IOException As throws
			impl.doConnect(address, port, timeout)
		End SyncLock

		protected synchronized  Sub  bind(InetAddress address, Integer lport) throws IOException
			impl.bind(address, lport)

		protected synchronized  Sub  accept(SocketImpl s) throws IOException
			If TypeOf s Is PlainSocketImpl Then
				' pass in the real impl not the wrapper.
				Dim [delegate] As SocketImpl = CType(s, PlainSocketImpl).impl
				[delegate].address = New InetAddress
				[delegate].fd = New FileDescriptor
				impl.accept([delegate])
				' set fd to delegate's fd to be compatible with older releases
				s.fd = [delegate].fd
			Else
				impl.accept(s)
			End If

		void fileDescriptortor(FileDescriptor fd)
			impl.fileDescriptor = fd

		void addressess(InetAddress address)
			impl.address = address

		void portort(Integer port)
			impl.port = port

		void localPortort(Integer localPort)
			impl.localPort = localPort

		protected synchronized InputStream inputStream throws IOException
			Return impl.inputStream

		void inputStreameam(SocketInputStream in)
			impl.inputStream = in

		protected synchronized OutputStream outputStream throws IOException
			Return impl.outputStream

		protected  Sub  close() throws IOException
			Try
				impl.close()
			Finally
				' set fd to delegate's fd to be compatible with older releases
				Me.fd = Nothing
			End Try

		void reset() throws IOException
			Try
				impl.reset()
			Finally
				' set fd to delegate's fd to be compatible with older releases
				Me.fd = Nothing
			End Try

		protected  Sub  shutdownInput() throws IOException
			impl.shutdownInput()

		protected  Sub  shutdownOutput() throws IOException
			impl.shutdownOutput()

		protected  Sub  sendUrgentData(Integer data) throws IOException
			impl.sendUrgentData(data)

		FileDescriptor acquireFD()
			Return impl.acquireFD()

		void releaseFD()
			impl.releaseFD()

		public Boolean connectionReset
			Return impl.connectionReset

		public Boolean connectionResetPending
			Return impl.connectionResetPending

		public  Sub  connectionResetset()
			impl.connectionResetset()

		public  Sub  connectionResetPendinging()
			impl.connectionResetPendinging()

		public Boolean closedOrPending
			Return impl.closedOrPending

		public Integer timeout
			Return impl.timeout

		' Override methods in AbstractPlainSocketImpl that need to be implemented.

		void socketCreate(Boolean isServer) throws IOException
			impl.socketCreate(isServer)

		void socketConnect(InetAddress address, Integer port, Integer timeout) throws IOException
			impl.socketConnect(address, port, timeout)

		void socketBind(InetAddress address, Integer port) throws IOException
			impl.socketBind(address, port)

		void socketListen(Integer count) throws IOException
			impl.socketListen(count)

		void socketAccept(SocketImpl s) throws IOException
			impl.socketAccept(s)

		Integer socketAvailable() throws IOException
			Return impl.socketAvailable()

		void socketClose0(Boolean useDeferredClose) throws IOException
			impl.socketClose0(useDeferredClose)

		void socketShutdown(Integer howto) throws IOException
			impl.socketShutdown(howto)

		void socketSetOption(Integer cmd, Boolean on, Object value) throws SocketException
			impl.socketSetOption(cmd, on, value)

		Integer socketGetOption(Integer opt, Object iaContainerObj) throws SocketException
			Return impl.socketGetOption(opt, iaContainerObj)

		void socketSendUrgentData(Integer data) throws IOException
			impl.socketSendUrgentData(data)
	End Class

End Namespace