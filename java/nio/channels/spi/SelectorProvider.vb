Imports System
Imports System.Collections.Generic

'
' * Copyright (c) 2000, 2013, Oracle and/or its affiliates. All rights reserved.
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

Namespace java.nio.channels.spi



	''' <summary>
	''' Service-provider class for selectors and selectable channels.
	''' 
	''' <p> A selector provider is a concrete subclass of this class that has a
	''' zero-argument constructor and implements the abstract methods specified
	''' below.  A given invocation of the Java virtual machine maintains a single
	''' system-wide default provider instance, which is returned by the {@link
	''' #provider() provider} method.  The first invocation of that method will locate
	''' the default provider as specified below.
	''' 
	''' <p> The system-wide default provider is used by the static <tt>open</tt>
	''' methods of the {@link java.nio.channels.DatagramChannel#open
	''' DatagramChannel}, <seealso cref="java.nio.channels.Pipe#open Pipe"/>, {@link
	''' java.nio.channels.Selector#open Selector}, {@link
	''' java.nio.channels.ServerSocketChannel#open ServerSocketChannel}, and {@link
	''' java.nio.channels.SocketChannel#open SocketChannel} classes.  It is also
	''' used by the <seealso cref="java.lang.System#inheritedChannel System.inheritedChannel()"/>
	''' method. A program may make use of a provider other than the default provider
	''' by instantiating that provider and then directly invoking the <tt>open</tt>
	''' methods defined in this class.
	''' 
	''' <p> All of the methods in this class are safe for use by multiple concurrent
	''' threads.  </p>
	''' 
	''' 
	''' @author Mark Reinhold
	''' @author JSR-51 Expert Group
	''' @since 1.4
	''' </summary>

	Public MustInherit Class SelectorProvider

		Private Shared ReadOnly lock As New Object
		Private Shared provider_Renamed As SelectorProvider = Nothing

		''' <summary>
		''' Initializes a new instance of this class.
		''' </summary>
		''' <exception cref="SecurityException">
		'''          If a security manager has been installed and it denies
		'''          <seealso cref="RuntimePermission"/><tt>("selectorProvider")</tt> </exception>
		Protected Friend Sub New()
			Dim sm As SecurityManager = System.securityManager
			If sm IsNot Nothing Then sm.checkPermission(New RuntimePermission("selectorProvider"))
		End Sub

		Private Shared Function loadProviderFromProperty() As Boolean
			Dim cn As String = System.getProperty("java.nio.channels.spi.SelectorProvider")
			If cn Is Nothing Then Return False
			Try
				Dim c As  [Class] = Type.GetType(cn, True, ClassLoader.systemClassLoader)
				provider_Renamed = CType(c.newInstance(), SelectorProvider)
				Return True
			Catch x As  [Class]NotFoundException
				Throw New java.util.ServiceConfigurationError(Nothing, x)
			Catch x As IllegalAccessException
				Throw New java.util.ServiceConfigurationError(Nothing, x)
			Catch x As InstantiationException
				Throw New java.util.ServiceConfigurationError(Nothing, x)
			Catch x As SecurityException
				Throw New java.util.ServiceConfigurationError(Nothing, x)
			End Try
		End Function

		Private Shared Function loadProviderAsService() As Boolean

			Dim sl As java.util.ServiceLoader(Of SelectorProvider) = java.util.ServiceLoader.load(GetType(SelectorProvider), ClassLoader.systemClassLoader)
			Dim i As IEnumerator(Of SelectorProvider) = sl.GetEnumerator()
			Do
				Try
'JAVA TO VB CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
					If Not i.hasNext() Then Return False
'JAVA TO VB CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
					provider_Renamed = i.next()
					Return True
				Catch sce As java.util.ServiceConfigurationError
					If TypeOf sce.cause Is SecurityException Then Continue Do
					Throw sce
				End Try
			Loop
		End Function

		''' <summary>
		''' Returns the system-wide default selector provider for this invocation of
		''' the Java virtual machine.
		''' 
		''' <p> The first invocation of this method locates the default provider
		''' object as follows: </p>
		''' 
		''' <ol>
		''' 
		'''   <li><p> If the system property
		'''   <tt>java.nio.channels.spi.SelectorProvider</tt> is defined then it is
		'''   taken to be the fully-qualified name of a concrete provider class.
		'''   The class is loaded and instantiated; if this process fails then an
		'''   unspecified error is thrown.  </p></li>
		''' 
		'''   <li><p> If a provider class has been installed in a jar file that is
		'''   visible to the system class loader, and that jar file contains a
		'''   provider-configuration file named
		'''   <tt>java.nio.channels.spi.SelectorProvider</tt> in the resource
		'''   directory <tt>META-INF/services</tt>, then the first class name
		'''   specified in that file is taken.  The class is loaded and
		'''   instantiated; if this process fails then an unspecified error is
		'''   thrown.  </p></li>
		''' 
		'''   <li><p> Finally, if no provider has been specified by any of the above
		'''   means then the system-default provider class is instantiated and the
		'''   result is returned.  </p></li>
		''' 
		''' </ol>
		''' 
		''' <p> Subsequent invocations of this method return the provider that was
		''' returned by the first invocation.  </p>
		''' </summary>
		''' <returns>  The system-wide default selector provider </returns>
		Public Shared Function provider() As SelectorProvider
			SyncLock lock
				If provider_Renamed IsNot Nothing Then Return provider_Renamed
				Return java.security.AccessController.doPrivileged(New PrivilegedActionAnonymousInnerClassHelper(Of T)
			End SyncLock
		End Function

		Private Class PrivilegedActionAnonymousInnerClassHelper(Of T)
			Implements java.security.PrivilegedAction(Of T)

			Public Overridable Function run() As SelectorProvider
					If loadProviderFromProperty() Then Return provider_Renamed
					If loadProviderAsService() Then Return provider_Renamed
					provider_Renamed = sun.nio.ch.DefaultSelectorProvider.create()
					Return provider_Renamed
			End Function
		End Class

		''' <summary>
		''' Opens a datagram channel.
		''' </summary>
		''' <returns>  The new channel
		''' </returns>
		''' <exception cref="IOException">
		'''          If an I/O error occurs </exception>
		Public MustOverride Function openDatagramChannel() As DatagramChannel

		''' <summary>
		''' Opens a datagram channel.
		''' </summary>
		''' <param name="family">
		'''          The protocol family
		''' </param>
		''' <returns>  A new datagram channel
		''' </returns>
		''' <exception cref="UnsupportedOperationException">
		'''          If the specified protocol family is not supported </exception>
		''' <exception cref="IOException">
		'''          If an I/O error occurs
		''' 
		''' @since 1.7 </exception>
		Public MustOverride Function openDatagramChannel(ByVal family As java.net.ProtocolFamily) As DatagramChannel

		''' <summary>
		''' Opens a pipe.
		''' </summary>
		''' <returns>  The new pipe
		''' </returns>
		''' <exception cref="IOException">
		'''          If an I/O error occurs </exception>
		Public MustOverride Function openPipe() As Pipe

		''' <summary>
		''' Opens a selector.
		''' </summary>
		''' <returns>  The new selector
		''' </returns>
		''' <exception cref="IOException">
		'''          If an I/O error occurs </exception>
		Public MustOverride Function openSelector() As AbstractSelector

		''' <summary>
		''' Opens a server-socket channel.
		''' </summary>
		''' <returns>  The new channel
		''' </returns>
		''' <exception cref="IOException">
		'''          If an I/O error occurs </exception>
		Public MustOverride Function openServerSocketChannel() As ServerSocketChannel

		''' <summary>
		''' Opens a socket channel.
		''' </summary>
		''' <returns>  The new channel
		''' </returns>
		''' <exception cref="IOException">
		'''          If an I/O error occurs </exception>
		Public MustOverride Function openSocketChannel() As SocketChannel

		''' <summary>
		''' Returns the channel inherited from the entity that created this
		''' Java virtual machine.
		''' 
		''' <p> On many operating systems a process, such as a Java virtual
		''' machine, can be started in a manner that allows the process to
		''' inherit a channel from the entity that created the process. The
		''' manner in which this is done is system dependent, as are the
		''' possible entities to which the channel may be connected. For example,
		''' on UNIX systems, the Internet services daemon (<i>inetd</i>) is used to
		''' start programs to service requests when a request arrives on an
		''' associated network port. In this example, the process that is started,
		''' inherits a channel representing a network socket.
		''' 
		''' <p> In cases where the inherited channel represents a network socket
		''' then the <seealso cref="java.nio.channels.Channel Channel"/> type returned
		''' by this method is determined as follows:
		''' 
		''' <ul>
		''' 
		'''  <li><p> If the inherited channel represents a stream-oriented connected
		'''  socket then a <seealso cref="java.nio.channels.SocketChannel SocketChannel"/> is
		'''  returned. The socket channel is, at least initially, in blocking
		'''  mode, bound to a socket address, and connected to a peer.
		'''  </p></li>
		''' 
		'''  <li><p> If the inherited channel represents a stream-oriented listening
		'''  socket then a {@link java.nio.channels.ServerSocketChannel
		'''  ServerSocketChannel} is returned. The server-socket channel is, at
		'''  least initially, in blocking mode, and bound to a socket address.
		'''  </p></li>
		''' 
		'''  <li><p> If the inherited channel is a datagram-oriented socket
		'''  then a <seealso cref="java.nio.channels.DatagramChannel DatagramChannel"/> is
		'''  returned. The datagram channel is, at least initially, in blocking
		'''  mode, and bound to a socket address.
		'''  </p></li>
		''' 
		''' </ul>
		''' 
		''' <p> In addition to the network-oriented channels described, this method
		''' may return other kinds of channels in the future.
		''' 
		''' <p> The first invocation of this method creates the channel that is
		''' returned. Subsequent invocations of this method return the same
		''' channel. </p>
		''' </summary>
		''' <returns>  The inherited channel, if any, otherwise <tt>null</tt>.
		''' </returns>
		''' <exception cref="IOException">
		'''          If an I/O error occurs
		''' </exception>
		''' <exception cref="SecurityException">
		'''          If a security manager has been installed and it denies
		'''          <seealso cref="RuntimePermission"/><tt>("inheritedChannel")</tt>
		''' 
		''' @since 1.5 </exception>
	   Public Overridable Function inheritedChannel() As Channel
			Return Nothing
	   End Function

	End Class

End Namespace