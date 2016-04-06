Imports System
Imports System.Collections.Generic

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

Namespace java.nio.channels.spi


	''' <summary>
	''' Service-provider class for asynchronous channels.
	''' 
	''' <p> An asynchronous channel provider is a concrete subclass of this class that
	''' has a zero-argument constructor and implements the abstract methods specified
	''' below.  A given invocation of the Java virtual machine maintains a single
	''' system-wide default provider instance, which is returned by the {@link
	''' #provider() provider} method.  The first invocation of that method will locate
	''' the default provider as specified below.
	''' 
	''' <p> All of the methods in this class are safe for use by multiple concurrent
	''' threads.  </p>
	''' 
	''' @since 1.7
	''' </summary>

	Public MustInherit Class AsynchronousChannelProvider
		Private Shared Function checkPermission() As Void
			Dim sm As SecurityManager = System.securityManager
			If sm IsNot Nothing Then sm.checkPermission(New RuntimePermission("asynchronousChannelProvider"))
			Return Nothing
		End Function
		Private Sub New(  ignore As Void)
		End Sub

		''' <summary>
		''' Initializes a new instance of this class.
		''' </summary>
		''' <exception cref="SecurityException">
		'''          If a security manager has been installed and it denies
		'''          <seealso cref="RuntimePermission"/><tt>("asynchronousChannelProvider")</tt> </exception>
		Protected Friend Sub New()
			Me.New(checkPermission())
		End Sub

		' lazy initialization of default provider
		Private Class ProviderHolder
			Friend Shared ReadOnly provider As AsynchronousChannelProvider = load()

			Private Shared Function load() As AsynchronousChannelProvider
				Return java.security.AccessController.doPrivileged(New PrivilegedActionAnonymousInnerClassHelper(Of T)
			End Function

			Private Class PrivilegedActionAnonymousInnerClassHelper(Of T)
				Implements java.security.PrivilegedAction(Of T)

				Public Overridable Function run() As AsynchronousChannelProvider
					Dim p As AsynchronousChannelProvider
					p = loadProviderFromProperty()
					If p IsNot Nothing Then Return p
					p = loadProviderAsService()
					If p IsNot Nothing Then Return p
					Return sun.nio.ch.DefaultAsynchronousChannelProvider.create()
				End Function
			End Class

			Private Shared Function loadProviderFromProperty() As AsynchronousChannelProvider
				Dim cn As String = System.getProperty("java.nio.channels.spi.AsynchronousChannelProvider")
				If cn Is Nothing Then Return Nothing
				Try
					Dim c As  [Class] = Type.GetType(cn, True, ClassLoader.systemClassLoader)
					Return CType(c.newInstance(), AsynchronousChannelProvider)
				Catch x As  ClassNotFoundException
					Throw New java.util.ServiceConfigurationError(Nothing, x)
				Catch x As IllegalAccessException
					Throw New java.util.ServiceConfigurationError(Nothing, x)
				Catch x As InstantiationException
					Throw New java.util.ServiceConfigurationError(Nothing, x)
				Catch x As SecurityException
					Throw New java.util.ServiceConfigurationError(Nothing, x)
				End Try
			End Function

			Private Shared Function loadProviderAsService() As AsynchronousChannelProvider
				Dim sl As java.util.ServiceLoader(Of AsynchronousChannelProvider) = java.util.ServiceLoader.load(GetType(AsynchronousChannelProvider), ClassLoader.systemClassLoader)
				Dim i As IEnumerator(Of AsynchronousChannelProvider) = sl.GetEnumerator()
				Do
					Try
'JAVA TO VB CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
						Return If(i.hasNext(), i.next(), Nothing)
					Catch sce As java.util.ServiceConfigurationError
						If TypeOf sce.cause Is SecurityException Then Continue Do
						Throw sce
					End Try
				Loop
			End Function
		End Class

		''' <summary>
		''' Returns the system-wide default asynchronous channel provider for this
		''' invocation of the Java virtual machine.
		''' 
		''' <p> The first invocation of this method locates the default provider
		''' object as follows: </p>
		''' 
		''' <ol>
		''' 
		'''   <li><p> If the system property
		'''   <tt>java.nio.channels.spi.AsynchronousChannelProvider</tt> is defined
		'''   then it is taken to be the fully-qualified name of a concrete provider class.
		'''   The class is loaded and instantiated; if this process fails then an
		'''   unspecified error is thrown.  </p></li>
		''' 
		'''   <li><p> If a provider class has been installed in a jar file that is
		'''   visible to the system class loader, and that jar file contains a
		'''   provider-configuration file named
		'''   <tt>java.nio.channels.spi.AsynchronousChannelProvider</tt> in the resource
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
		''' <returns>  The system-wide default AsynchronousChannel provider </returns>
		Public Shared Function provider() As AsynchronousChannelProvider
			Return ProviderHolder.provider
		End Function

		''' <summary>
		''' Constructs a new asynchronous channel group with a fixed thread pool.
		''' </summary>
		''' <param name="nThreads">
		'''          The number of threads in the pool </param>
		''' <param name="threadFactory">
		'''          The factory to use when creating new threads
		''' </param>
		''' <returns>  A new asynchronous channel group
		''' </returns>
		''' <exception cref="IllegalArgumentException">
		'''          If {@code nThreads <= 0} </exception>
		''' <exception cref="IOException">
		'''          If an I/O error occurs
		''' </exception>
		''' <seealso cref= AsynchronousChannelGroup#withFixedThreadPool </seealso>
		Public MustOverride Function openAsynchronousChannelGroup(  nThreads As Integer,   threadFactory As ThreadFactory) As AsynchronousChannelGroup

		''' <summary>
		''' Constructs a new asynchronous channel group with the given thread pool.
		''' </summary>
		''' <param name="executor">
		'''          The thread pool </param>
		''' <param name="initialSize">
		'''          A value {@code >=0} or a negative value for implementation
		'''          specific default
		''' </param>
		''' <returns>  A new asynchronous channel group
		''' </returns>
		''' <exception cref="IOException">
		'''          If an I/O error occurs
		''' </exception>
		''' <seealso cref= AsynchronousChannelGroup#withCachedThreadPool </seealso>
		Public MustOverride Function openAsynchronousChannelGroup(  executor As ExecutorService,   initialSize As Integer) As AsynchronousChannelGroup

		''' <summary>
		''' Opens an asynchronous server-socket channel.
		''' </summary>
		''' <param name="group">
		'''          The group to which the channel is bound, or {@code null} to
		'''          bind to the default group
		''' </param>
		''' <returns>  The new channel
		''' </returns>
		''' <exception cref="IllegalChannelGroupException">
		'''          If the provider that created the group differs from this provider </exception>
		''' <exception cref="ShutdownChannelGroupException">
		'''          The group is shutdown </exception>
		''' <exception cref="IOException">
		'''          If an I/O error occurs </exception>
		Public MustOverride Function openAsynchronousServerSocketChannel(  group As AsynchronousChannelGroup) As AsynchronousServerSocketChannel

		''' <summary>
		''' Opens an asynchronous socket channel.
		''' </summary>
		''' <param name="group">
		'''          The group to which the channel is bound, or {@code null} to
		'''          bind to the default group
		''' </param>
		''' <returns>  The new channel
		''' </returns>
		''' <exception cref="IllegalChannelGroupException">
		'''          If the provider that created the group differs from this provider </exception>
		''' <exception cref="ShutdownChannelGroupException">
		'''          The group is shutdown </exception>
		''' <exception cref="IOException">
		'''          If an I/O error occurs </exception>
		Public MustOverride Function openAsynchronousSocketChannel(  group As AsynchronousChannelGroup) As AsynchronousSocketChannel
	End Class

End Namespace