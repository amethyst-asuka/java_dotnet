Imports System
Imports System.Runtime.CompilerServices
Imports System.Collections.Generic
Imports System.Collections.Concurrent
Imports System.Threading
Imports System.Runtime.InteropServices

'
' * Copyright (c) 1994, 2013, Oracle and/or its affiliates. All rights reserved.
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

Namespace java.lang



	''' <summary>
	''' A <i>thread</i> is a thread of execution in a program. The Java
	''' Virtual Machine allows an application to have multiple threads of
	''' execution running concurrently.
	''' <p>
	''' Every thread has a priority. Threads with higher priority are
	''' executed in preference to threads with lower priority. Each thread
	''' may or may not also be marked as a daemon. When code running in
	''' some thread creates a new <code>Thread</code> object, the new
	''' thread has its priority initially set equal to the priority of the
	''' creating thread, and is a daemon thread if and only if the
	''' creating thread is a daemon.
	''' <p>
	''' When a Java Virtual Machine starts up, there is usually a single
	''' non-daemon thread (which typically calls the method named
	''' <code>main</code> of some designated [Class]). The Java Virtual
	''' Machine continues to execute threads until either of the following
	''' occurs:
	''' <ul>
	''' <li>The <code>exit</code> method of class <code>Runtime</code> has been
	'''     called and the security manager has permitted the exit operation
	'''     to take place.
	''' <li>All threads that are not daemon threads have died, either by
	'''     returning from the call to the <code>run</code> method or by
	'''     throwing an exception that propagates beyond the <code>run</code>
	'''     method.
	''' </ul>
	''' <p>
	''' There are two ways to create a new thread of execution. One is to
	''' declare a class to be a subclass of <code>Thread</code>. This
	''' subclass should override the <code>run</code> method of class
	''' <code>Thread</code>. An instance of the subclass can then be
	''' allocated and started. For example, a thread that computes primes
	''' larger than a stated value could be written as follows:
	''' <hr><blockquote><pre>
	'''     class PrimeThread extends Thread {
	'''         long minPrime;
	'''         PrimeThread(long minPrime) {
	'''             this.minPrime = minPrime;
	'''         }
	''' 
	'''         public  Sub  run() {
	'''             // compute primes larger than minPrime
	'''             &nbsp;.&nbsp;.&nbsp;.
	'''         }
	'''     }
	''' </pre></blockquote><hr>
	''' <p>
	''' The following code would then create a thread and start it running:
	''' <blockquote><pre>
	'''     PrimeThread p = new PrimeThread(143);
	'''     p.start();
	''' </pre></blockquote>
	''' <p>
	''' The other way to create a thread is to declare a class that
	''' implements the <code>Runnable</code> interface. That class then
	''' implements the <code>run</code> method. An instance of the class can
	''' then be allocated, passed as an argument when creating
	''' <code>Thread</code>, and started. The same example in this other
	''' style looks like the following:
	''' <hr><blockquote><pre>
	'''     class PrimeRun implements Runnable {
	'''         long minPrime;
	'''         PrimeRun(long minPrime) {
	'''             this.minPrime = minPrime;
	'''         }
	''' 
	'''         public  Sub  run() {
	'''             // compute primes larger than minPrime
	'''             &nbsp;.&nbsp;.&nbsp;.
	'''         }
	'''     }
	''' </pre></blockquote><hr>
	''' <p>
	''' The following code would then create a thread and start it running:
	''' <blockquote><pre>
	'''     PrimeRun p = new PrimeRun(143);
	'''     new Thread(p).start();
	''' </pre></blockquote>
	''' <p>
	''' Every thread has a name for identification purposes. More than
	''' one thread may have the same name. If a name is not specified when
	''' a thread is created, a new name is generated for it.
	''' <p>
	''' Unless otherwise noted, passing a {@code null} argument to a constructor
	''' or method in this class will cause a <seealso cref="NullPointerException"/> to be
	''' thrown.
	''' 
	''' @author  unascribed </summary>
	''' <seealso cref=     Runnable </seealso>
	''' <seealso cref=     Runtime#exit(int) </seealso>
	''' <seealso cref=     #run() </seealso>
	''' <seealso cref=     #stop()
	''' @since   JDK1.0 </seealso>
	Public Class Thread
		Implements Runnable

		' Make sure registerNatives is the first thing <clinit> does. 
'JAVA TO VB CONVERTER TODO TASK: Replace 'unknown' with the appropriate dll name:
		<DllImport("unknown")> _
		Private Shared Sub registerNatives()
		End Sub
		Shared Sub New()
			registerNatives()
		End Sub

'JAVA TO VB CONVERTER TODO TASK: There is no VB equivalent to 'volatile':
		Private name As Char()
		Private priority As Integer
		Private threadQ As Thread
		Private eetop As Long

		' Whether or not to single_step this thread. 
		Private single_step As Boolean

		' Whether or not the thread is a daemon thread. 
		Private daemon As Boolean = False

		' JVM state 
		Private stillborn As Boolean = False

		' What will be run. 
		Private target As Runnable

		' The group of this thread 
		Private group As ThreadGroup

		' The context ClassLoader for this thread 
		Private contextClassLoader As  ClassLoader

		' The inherited AccessControlContext of this thread 
		Private inheritedAccessControlContext As java.security.AccessControlContext

		' For autonumbering anonymous threads. 
		Private Shared threadInitNumber As Integer
		<MethodImpl(MethodImplOptions.Synchronized)> _
		Private Shared Function nextThreadNum() As Integer
				Dim tempVar As Integer = threadInitNumber
				threadInitNumber += 1
				Return tempVar
		End Function

	'     ThreadLocal values pertaining to this thread. This map is maintained
	'     * by the ThreadLocal class. 
		Friend threadLocals As ThreadLocal.ThreadLocalMap = Nothing

	'    
	'     * InheritableThreadLocal values pertaining to this thread. This map is
	'     * maintained by the InheritableThreadLocal class.
	'     
		Friend inheritableThreadLocals As ThreadLocal.ThreadLocalMap = Nothing

	'    
	'     * The requested stack size for this thread, or 0 if the creator did
	'     * not specify a stack size.  It is up to the VM to do whatever it
	'     * likes with this number; some VMs will ignore it.
	'     
		Private stackSize As Long

	'    
	'     * JVM-private state that persists after native thread termination.
	'     
		Private nativeParkEventPointer As Long

	'    
	'     * Thread ID
	'     
		Private tid As Long

		' For generating thread ID 
		Private Shared threadSeqNumber As Long

	'     Java thread status for tools,
	'     * initialized to indicate thread 'not yet started'
	'     

'JAVA TO VB CONVERTER TODO TASK: There is no VB equivalent to 'volatile':
		Private threadStatus As Integer = 0


		<MethodImpl(MethodImplOptions.Synchronized)> _
		Private Shared Function nextThreadID() As Long
				threadSeqNumber += 1
				Return threadSeqNumber
		End Function

		''' <summary>
		''' The argument supplied to the current call to
		''' java.util.concurrent.locks.LockSupport.park.
		''' Set by (private) java.util.concurrent.locks.LockSupport.setBlocker
		''' Accessed using java.util.concurrent.locks.LockSupport.getBlocker
		''' </summary>
'JAVA TO VB CONVERTER TODO TASK: There is no VB equivalent to 'volatile':
		Friend parkBlocker As Object

	'     The object in which this thread is blocked in an interruptible I/O
	'     * operation, if any.  The blocker's interrupt method should be invoked
	'     * after setting this thread's interrupt status.
	'     
'JAVA TO VB CONVERTER TODO TASK: There is no VB equivalent to 'volatile':
		Private blocker As sun.nio.ch.Interruptible
		Private ReadOnly blockerLock As New Object

	'     Set the blocker field; invoked via sun.misc.SharedSecrets from java.nio code
	'     
		Friend Overridable Sub blockedOn(  b As sun.nio.ch.Interruptible)
			SyncLock blockerLock
				blocker = b
			End SyncLock
		End Sub

		''' <summary>
		''' The minimum priority that a thread can have.
		''' </summary>
		Public Const MIN_PRIORITY As Integer = 1

	   ''' <summary>
	   ''' The default priority that is assigned to a thread.
	   ''' </summary>
		Public Const NORM_PRIORITY As Integer = 5

		''' <summary>
		''' The maximum priority that a thread can have.
		''' </summary>
		Public Const MAX_PRIORITY As Integer = 10

		''' <summary>
		''' Returns a reference to the currently executing thread object.
		''' </summary>
		''' <returns>  the currently executing thread. </returns>
'JAVA TO VB CONVERTER TODO TASK: Replace 'unknown' with the appropriate dll name:
		<DllImport("unknown")> _
		Public Shared Function currentThread() As Thread
		End Function

		''' <summary>
		''' A hint to the scheduler that the current thread is willing to yield
		''' its current use of a processor. The scheduler is free to ignore this
		''' hint.
		''' 
		''' <p> Yield is a heuristic attempt to improve relative progression
		''' between threads that would otherwise over-utilise a CPU. Its use
		''' should be combined with detailed profiling and benchmarking to
		''' ensure that it actually has the desired effect.
		''' 
		''' <p> It is rarely appropriate to use this method. It may be useful
		''' for debugging or testing purposes, where it may help to reproduce
		''' bugs due to race conditions. It may also be useful when designing
		''' concurrency control constructs such as the ones in the
		''' <seealso cref="java.util.concurrent.locks"/> package.
		''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Replace 'unknown' with the appropriate dll name:
		<DllImport("unknown")> _
		Public Shared Sub [yield]()
		End Sub

		''' <summary>
		''' Causes the currently executing thread to sleep (temporarily cease
		''' execution) for the specified number of milliseconds, subject to
		''' the precision and accuracy of system timers and schedulers. The thread
		''' does not lose ownership of any monitors.
		''' </summary>
		''' <param name="millis">
		'''         the length of time to sleep in milliseconds
		''' </param>
		''' <exception cref="IllegalArgumentException">
		'''          if the value of {@code millis} is negative
		''' </exception>
		''' <exception cref="InterruptedException">
		'''          if any thread has interrupted the current thread. The
		'''          <i>interrupted status</i> of the current thread is
		'''          cleared when this exception is thrown. </exception>
'JAVA TO VB CONVERTER TODO TASK: Replace 'unknown' with the appropriate dll name:
		<DllImport("unknown")> _
		Public Shared Sub sleep(  millis As Long)
		End Sub

		''' <summary>
		''' Causes the currently executing thread to sleep (temporarily cease
		''' execution) for the specified number of milliseconds plus the specified
		''' number of nanoseconds, subject to the precision and accuracy of system
		''' timers and schedulers. The thread does not lose ownership of any
		''' monitors.
		''' </summary>
		''' <param name="millis">
		'''         the length of time to sleep in milliseconds
		''' </param>
		''' <param name="nanos">
		'''         {@code 0-999999} additional nanoseconds to sleep
		''' </param>
		''' <exception cref="IllegalArgumentException">
		'''          if the value of {@code millis} is negative, or the value of
		'''          {@code nanos} is not in the range {@code 0-999999}
		''' </exception>
		''' <exception cref="InterruptedException">
		'''          if any thread has interrupted the current thread. The
		'''          <i>interrupted status</i> of the current thread is
		'''          cleared when this exception is thrown. </exception>
		Public Shared Sub sleep(  millis As Long,   nanos As Integer)
			If millis < 0 Then Throw New IllegalArgumentException("timeout value is negative")

			If nanos < 0 OrElse nanos > 999999 Then Throw New IllegalArgumentException("nanosecond timeout value out of range")

			If nanos >= 500000 OrElse (nanos <> 0 AndAlso millis = 0) Then millis += 1

			sleep(millis)
		End Sub

		''' <summary>
		''' Initializes a Thread with the current AccessControlContext. </summary>
		''' <seealso cref= #init(ThreadGroup,Runnable,String,long,AccessControlContext) </seealso>
		Private Sub init(  g As ThreadGroup,   target As Runnable,   name As String,   stackSize As Long)
			init(g, target, name, stackSize, Nothing)
		End Sub

		''' <summary>
		''' Initializes a Thread.
		''' </summary>
		''' <param name="g"> the Thread group </param>
		''' <param name="target"> the object whose run() method gets called </param>
		''' <param name="name"> the name of the new Thread </param>
		''' <param name="stackSize"> the desired stack size for the new thread, or
		'''        zero to indicate that this parameter is to be ignored. </param>
		''' <param name="acc"> the AccessControlContext to inherit, or
		'''            AccessController.getContext() if null </param>
		Private Sub init(  g As ThreadGroup,   target As Runnable,   name As String,   stackSize As Long,   acc As java.security.AccessControlContext)
			If name Is Nothing Then Throw New NullPointerException("name cannot be null")

			Me.name = name.ToCharArray()

			Dim parent As Thread = currentThread()
			Dim security As SecurityManager = System.securityManager
			If g Is Nothing Then
				' Determine if it's an applet or not 

	'             If there is a security manager, ask the security manager
	'               what to do. 
				If security IsNot Nothing Then g = security.threadGroup

	'             If the security doesn't have a strong opinion of the matter
	'               use the parent thread group. 
				If g Is Nothing Then g = parent.threadGroup
			End If

	'         checkAccess regardless of whether or not threadgroup is
	'           explicitly passed in. 
			g.checkAccess()

	'        
	'         * Do we have the required permissions?
	'         
			If security IsNot Nothing Then
				If isCCLOverridden(Me.GetType()) Then security.checkPermission(SUBCLASS_IMPLEMENTATION_PERMISSION)
			End If

			g.addUnstarted()

			Me.group = g
			Me.daemon = parent.daemon
			Me.priority = parent.priority
			If security Is Nothing OrElse isCCLOverridden(parent.GetType()) Then
				Me.contextClassLoader = parent.contextClassLoader
			Else
				Me.contextClassLoader = parent.contextClassLoader
			End If
			Me.inheritedAccessControlContext = If(acc IsNot Nothing, acc, java.security.AccessController.context)
			Me.target = target
			priority = priority
			If parent.inheritableThreadLocals IsNot Nothing Then Me.inheritableThreadLocals = ThreadLocal.createInheritedMap(parent.inheritableThreadLocals)
			' Stash the specified stack size in case the VM cares 
			Me.stackSize = stackSize

			' Set thread ID 
			tid = nextThreadID()
		End Sub

		''' <summary>
		''' Throws CloneNotSupportedException as a Thread can not be meaningfully
		''' cloned. Construct a new Thread instead.
		''' </summary>
		''' <exception cref="CloneNotSupportedException">
		'''          always </exception>
		Protected Friend Overrides Function clone() As Object
			Throw New CloneNotSupportedException
		End Function

		''' <summary>
		''' Allocates a new {@code Thread} object. This constructor has the same
		''' effect as <seealso cref="#Thread(ThreadGroup,Runnable,String) Thread"/>
		''' {@code (null, null, gname)}, where {@code gname} is a newly generated
		''' name. Automatically generated names are of the form
		''' {@code "Thread-"+}<i>n</i>, where <i>n</i> is an  java.lang.[Integer].
		''' </summary>
		Public Sub New()
			init(Nothing, Nothing, "Thread-" & nextThreadNum(), 0)
		End Sub

		''' <summary>
		''' Allocates a new {@code Thread} object. This constructor has the same
		''' effect as <seealso cref="#Thread(ThreadGroup,Runnable,String) Thread"/>
		''' {@code (null, target, gname)}, where {@code gname} is a newly generated
		''' name. Automatically generated names are of the form
		''' {@code "Thread-"+}<i>n</i>, where <i>n</i> is an  java.lang.[Integer].
		''' </summary>
		''' <param name="target">
		'''         the object whose {@code run} method is invoked when this thread
		'''         is started. If {@code null}, this classes {@code run} method does
		'''         nothing. </param>
		Public Sub New(  target As Runnable)
			init(Nothing, target, "Thread-" & nextThreadNum(), 0)
		End Sub

		''' <summary>
		''' Creates a new Thread that inherits the given AccessControlContext.
		''' This is not a public constructor.
		''' </summary>
		Friend Sub New(  target As Runnable,   acc As java.security.AccessControlContext)
			init(Nothing, target, "Thread-" & nextThreadNum(), 0, acc)
		End Sub

		''' <summary>
		''' Allocates a new {@code Thread} object. This constructor has the same
		''' effect as <seealso cref="#Thread(ThreadGroup,Runnable,String) Thread"/>
		''' {@code (group, target, gname)} ,where {@code gname} is a newly generated
		''' name. Automatically generated names are of the form
		''' {@code "Thread-"+}<i>n</i>, where <i>n</i> is an  java.lang.[Integer].
		''' </summary>
		''' <param name="group">
		'''         the thread group. If {@code null} and there is a security
		'''         manager, the group is determined by {@linkplain
		'''         SecurityManager#getThreadGroup SecurityManager.getThreadGroup()}.
		'''         If there is not a security manager or {@code
		'''         SecurityManager.getThreadGroup()} returns {@code null}, the group
		'''         is set to the current thread's thread group.
		''' </param>
		''' <param name="target">
		'''         the object whose {@code run} method is invoked when this thread
		'''         is started. If {@code null}, this thread's run method is invoked.
		''' </param>
		''' <exception cref="SecurityException">
		'''          if the current thread cannot create a thread in the specified
		'''          thread group </exception>
		Public Sub New(  group As ThreadGroup,   target As Runnable)
			init(group, target, "Thread-" & nextThreadNum(), 0)
		End Sub

		''' <summary>
		''' Allocates a new {@code Thread} object. This constructor has the same
		''' effect as <seealso cref="#Thread(ThreadGroup,Runnable,String) Thread"/>
		''' {@code (null, null, name)}.
		''' </summary>
		''' <param name="name">
		'''          the name of the new thread </param>
		Public Sub New(  name As String)
			init(Nothing, Nothing, name, 0)
		End Sub

		''' <summary>
		''' Allocates a new {@code Thread} object. This constructor has the same
		''' effect as <seealso cref="#Thread(ThreadGroup,Runnable,String) Thread"/>
		''' {@code (group, null, name)}.
		''' </summary>
		''' <param name="group">
		'''         the thread group. If {@code null} and there is a security
		'''         manager, the group is determined by {@linkplain
		'''         SecurityManager#getThreadGroup SecurityManager.getThreadGroup()}.
		'''         If there is not a security manager or {@code
		'''         SecurityManager.getThreadGroup()} returns {@code null}, the group
		'''         is set to the current thread's thread group.
		''' </param>
		''' <param name="name">
		'''         the name of the new thread
		''' </param>
		''' <exception cref="SecurityException">
		'''          if the current thread cannot create a thread in the specified
		'''          thread group </exception>
		Public Sub New(  group As ThreadGroup,   name As String)
			init(group, Nothing, name, 0)
		End Sub

		''' <summary>
		''' Allocates a new {@code Thread} object. This constructor has the same
		''' effect as <seealso cref="#Thread(ThreadGroup,Runnable,String) Thread"/>
		''' {@code (null, target, name)}.
		''' </summary>
		''' <param name="target">
		'''         the object whose {@code run} method is invoked when this thread
		'''         is started. If {@code null}, this thread's run method is invoked.
		''' </param>
		''' <param name="name">
		'''         the name of the new thread </param>
		Public Sub New(  target As Runnable,   name As String)
			init(Nothing, target, name, 0)
		End Sub

		''' <summary>
		''' Allocates a new {@code Thread} object so that it has {@code target}
		''' as its run object, has the specified {@code name} as its name,
		''' and belongs to the thread group referred to by {@code group}.
		''' 
		''' <p>If there is a security manager, its
		''' <seealso cref="SecurityManager#checkAccess(ThreadGroup) checkAccess"/>
		''' method is invoked with the ThreadGroup as its argument.
		''' 
		''' <p>In addition, its {@code checkPermission} method is invoked with
		''' the {@code RuntimePermission("enableContextClassLoaderOverride")}
		''' permission when invoked directly or indirectly by the constructor
		''' of a subclass which overrides the {@code getContextClassLoader}
		''' or {@code setContextClassLoader} methods.
		''' 
		''' <p>The priority of the newly created thread is set equal to the
		''' priority of the thread creating it, that is, the currently running
		''' thread. The method <seealso cref="#setPriority setPriority"/> may be
		''' used to change the priority to a new value.
		''' 
		''' <p>The newly created thread is initially marked as being a daemon
		''' thread if and only if the thread creating it is currently marked
		''' as a daemon thread. The method <seealso cref="#setDaemon setDaemon"/>
		''' may be used to change whether or not a thread is a daemon.
		''' </summary>
		''' <param name="group">
		'''         the thread group. If {@code null} and there is a security
		'''         manager, the group is determined by {@linkplain
		'''         SecurityManager#getThreadGroup SecurityManager.getThreadGroup()}.
		'''         If there is not a security manager or {@code
		'''         SecurityManager.getThreadGroup()} returns {@code null}, the group
		'''         is set to the current thread's thread group.
		''' </param>
		''' <param name="target">
		'''         the object whose {@code run} method is invoked when this thread
		'''         is started. If {@code null}, this thread's run method is invoked.
		''' </param>
		''' <param name="name">
		'''         the name of the new thread
		''' </param>
		''' <exception cref="SecurityException">
		'''          if the current thread cannot create a thread in the specified
		'''          thread group or cannot override the context class loader methods. </exception>
		Public Sub New(  group As ThreadGroup,   target As Runnable,   name As String)
			init(group, target, name, 0)
		End Sub

		''' <summary>
		''' Allocates a new {@code Thread} object so that it has {@code target}
		''' as its run object, has the specified {@code name} as its name,
		''' and belongs to the thread group referred to by {@code group}, and has
		''' the specified <i>stack size</i>.
		''' 
		''' <p>This constructor is identical to {@link
		''' #Thread(ThreadGroup,Runnable,String)} with the exception of the fact
		''' that it allows the thread stack size to be specified.  The stack size
		''' is the approximate number of bytes of address space that the virtual
		''' machine is to allocate for this thread's stack.  <b>The effect of the
		''' {@code stackSize} parameter, if any, is highly platform dependent.</b>
		''' 
		''' <p>On some platforms, specifying a higher value for the
		''' {@code stackSize} parameter may allow a thread to achieve greater
		''' recursion depth before throwing a <seealso cref="StackOverflowError"/>.
		''' Similarly, specifying a lower value may allow a greater number of
		''' threads to exist concurrently without throwing an {@link
		''' OutOfMemoryError} (or other internal error).  The details of
		''' the relationship between the value of the <tt>stackSize</tt> parameter
		''' and the maximum recursion depth and concurrency level are
		''' platform-dependent.  <b>On some platforms, the value of the
		''' {@code stackSize} parameter may have no effect whatsoever.</b>
		''' 
		''' <p>The virtual machine is free to treat the {@code stackSize}
		''' parameter as a suggestion.  If the specified value is unreasonably low
		''' for the platform, the virtual machine may instead use some
		''' platform-specific minimum value; if the specified value is unreasonably
		''' high, the virtual machine may instead use some platform-specific
		''' maximum.  Likewise, the virtual machine is free to round the specified
		''' value up or down as it sees fit (or to ignore it completely).
		''' 
		''' <p>Specifying a value of zero for the {@code stackSize} parameter will
		''' cause this constructor to behave exactly like the
		''' {@code Thread(ThreadGroup, Runnable, String)} constructor.
		''' 
		''' <p><i>Due to the platform-dependent nature of the behavior of this
		''' constructor, extreme care should be exercised in its use.
		''' The thread stack size necessary to perform a given computation will
		''' likely vary from one JRE implementation to another.  In light of this
		''' variation, careful tuning of the stack size parameter may be required,
		''' and the tuning may need to be repeated for each JRE implementation on
		''' which an application is to run.</i>
		''' 
		''' <p>Implementation note: Java platform implementers are encouraged to
		''' document their implementation's behavior with respect to the
		''' {@code stackSize} parameter.
		''' 
		''' </summary>
		''' <param name="group">
		'''         the thread group. If {@code null} and there is a security
		'''         manager, the group is determined by {@linkplain
		'''         SecurityManager#getThreadGroup SecurityManager.getThreadGroup()}.
		'''         If there is not a security manager or {@code
		'''         SecurityManager.getThreadGroup()} returns {@code null}, the group
		'''         is set to the current thread's thread group.
		''' </param>
		''' <param name="target">
		'''         the object whose {@code run} method is invoked when this thread
		'''         is started. If {@code null}, this thread's run method is invoked.
		''' </param>
		''' <param name="name">
		'''         the name of the new thread
		''' </param>
		''' <param name="stackSize">
		'''         the desired stack size for the new thread, or zero to indicate
		'''         that this parameter is to be ignored.
		''' </param>
		''' <exception cref="SecurityException">
		'''          if the current thread cannot create a thread in the specified
		'''          thread group
		''' 
		''' @since 1.4 </exception>
		Public Sub New(  group As ThreadGroup,   target As Runnable,   name As String,   stackSize As Long)
			init(group, target, name, stackSize)
		End Sub

		''' <summary>
		''' Causes this thread to begin execution; the Java Virtual Machine
		''' calls the <code>run</code> method of this thread.
		''' <p>
		''' The result is that two threads are running concurrently: the
		''' current thread (which returns from the call to the
		''' <code>start</code> method) and the other thread (which executes its
		''' <code>run</code> method).
		''' <p>
		''' It is never legal to start a thread more than once.
		''' In particular, a thread may not be restarted once it has completed
		''' execution.
		''' </summary>
		''' <exception cref="IllegalThreadStateException">  if the thread was already
		'''               started. </exception>
		''' <seealso cref=        #run() </seealso>
		''' <seealso cref=        #stop() </seealso>
		<MethodImpl(MethodImplOptions.Synchronized)> _
		Public Overridable Sub start()
			''' <summary>
			''' This method is not invoked for the main method thread or "system"
			''' group threads created/set up by the VM. Any new functionality added
			''' to this method in the future may have to also be added to the VM.
			''' 
			''' A zero status value corresponds to state "NEW".
			''' </summary>
			If threadStatus <> 0 Then Throw New IllegalThreadStateException

	'         Notify the group that this thread is about to be started
	'         * so that it can be added to the group's list of threads
	'         * and the group's unstarted count can be decremented. 
			group.add(Me)

			Dim started As Boolean = False
			Try
				start0()
				started = True
			Finally
				Try
					If Not started Then group.threadStartFailed(Me)
				Catch ignore As Throwable
	'                 do nothing. If start0 threw a Throwable then
	'                  it will be passed up the call stack 
				End Try
			End Try
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Replace 'unknown' with the appropriate dll name:
		<DllImport("unknown")> _
		Private Sub start0()
		End Sub

		''' <summary>
		''' If this thread was constructed using a separate
		''' <code>Runnable</code> run object, then that
		''' <code>Runnable</code> object's <code>run</code> method is called;
		''' otherwise, this method does nothing and returns.
		''' <p>
		''' Subclasses of <code>Thread</code> should override this method.
		''' </summary>
		''' <seealso cref=     #start() </seealso>
		''' <seealso cref=     #stop() </seealso>
		''' <seealso cref=     #Thread(ThreadGroup, Runnable, String) </seealso>
		Public Overrides Sub run() Implements Runnable.run
			If target IsNot Nothing Then target.run()
		End Sub

		''' <summary>
		''' This method is called by the system to give a Thread
		''' a chance to clean up before it actually exits.
		''' </summary>
		Private Sub [exit]()
			If group IsNot Nothing Then
				group.threadTerminated(Me)
				group = Nothing
			End If
			' Aggressively null out all reference fields: see bug 4006245 
			target = Nothing
			' Speed the release of some of these resources 
			threadLocals = Nothing
			inheritableThreadLocals = Nothing
			inheritedAccessControlContext = Nothing
			blocker = Nothing
			uncaughtExceptionHandler = Nothing
		End Sub

		''' <summary>
		''' Forces the thread to stop executing.
		''' <p>
		''' If there is a security manager installed, its <code>checkAccess</code>
		''' method is called with <code>this</code>
		''' as its argument. This may result in a
		''' <code>SecurityException</code> being raised (in the current thread).
		''' <p>
		''' If this thread is different from the current thread (that is, the current
		''' thread is trying to stop a thread other than itself), the
		''' security manager's <code>checkPermission</code> method (with a
		''' <code>RuntimePermission("stopThread")</code> argument) is called in
		''' addition.
		''' Again, this may result in throwing a
		''' <code>SecurityException</code> (in the current thread).
		''' <p>
		''' The thread represented by this thread is forced to stop whatever
		''' it is doing abnormally and to throw a newly created
		''' <code>ThreadDeath</code> object as an exception.
		''' <p>
		''' It is permitted to stop a thread that has not yet been started.
		''' If the thread is eventually started, it immediately terminates.
		''' <p>
		''' An application should not normally try to catch
		''' <code>ThreadDeath</code> unless it must do some extraordinary
		''' cleanup operation (note that the throwing of
		''' <code>ThreadDeath</code> causes <code>finally</code> clauses of
		''' <code>try</code> statements to be executed before the thread
		''' officially dies).  If a <code>catch</code> clause catches a
		''' <code>ThreadDeath</code> object, it is important to rethrow the
		''' object so that the thread actually dies.
		''' <p>
		''' The top-level error handler that reacts to otherwise uncaught
		''' exceptions does not print out a message or otherwise notify the
		''' application if the uncaught exception is an instance of
		''' <code>ThreadDeath</code>.
		''' </summary>
		''' <exception cref="SecurityException">  if the current thread cannot
		'''               modify this thread. </exception>
		''' <seealso cref=        #interrupt() </seealso>
		''' <seealso cref=        #checkAccess() </seealso>
		''' <seealso cref=        #run() </seealso>
		''' <seealso cref=        #start() </seealso>
		''' <seealso cref=        ThreadDeath </seealso>
		''' <seealso cref=        ThreadGroup#uncaughtException(Thread,Throwable) </seealso>
		''' <seealso cref=        SecurityManager#checkAccess(Thread) </seealso>
		''' <seealso cref=        SecurityManager#checkPermission </seealso>
		''' @deprecated This method is inherently unsafe.  Stopping a thread with
		'''       Thread.stop causes it to unlock all of the monitors that it
		'''       has locked (as a natural consequence of the unchecked
		'''       <code>ThreadDeath</code> exception propagating up the stack).  If
		'''       any of the objects previously protected by these monitors were in
		'''       an inconsistent state, the damaged objects become visible to
		'''       other threads, potentially resulting in arbitrary behavior.  Many
		'''       uses of <code>stop</code> should be replaced by code that simply
		'''       modifies some variable to indicate that the target thread should
		'''       stop running.  The target thread should check this variable
		'''       regularly, and return from its run method in an orderly fashion
		'''       if the variable indicates that it is to stop running.  If the
		'''       target thread waits for long periods (on a condition variable,
		'''       for example), the <code>interrupt</code> method should be used to
		'''       interrupt the wait.
		'''       For more information, see
		'''       <a href="{@docRoot}/../technotes/guides/concurrency/threadPrimitiveDeprecation.html">Why
		'''       are Thread.stop, Thread.suspend and Thread.resume Deprecated?</a>. 
		<Obsolete("This method is inherently unsafe.  Stopping a thread with")> _
		Public Sub [stop]()
			Dim security As SecurityManager = System.securityManager
			If security IsNot Nothing Then
				checkAccess()
				If Me IsNot Thread.CurrentThread Then security.checkPermission(sun.security.util.SecurityConstants.STOP_THREAD_PERMISSION)
			End If
			' A zero status value corresponds to "NEW", it can't change to
			' not-NEW because we hold the lock.
			If threadStatus <> 0 Then [resume]() ' Wake up thread if it was suspended; no-op otherwise

			' The VM can handle all thread states
			stop0(New ThreadDeath)
		End Sub

		''' <summary>
		''' Throws {@code UnsupportedOperationException}.
		''' </summary>
		''' <param name="obj"> ignored
		''' </param>
		''' @deprecated This method was originally designed to force a thread to stop
		'''        and throw a given {@code Throwable} as an exception. It was
		'''        inherently unsafe (see <seealso cref="#stop()"/> for details), and furthermore
		'''        could be used to generate exceptions that the target thread was
		'''        not prepared to handle.
		'''        For more information, see
		'''        <a href="{@docRoot}/../technotes/guides/concurrency/threadPrimitiveDeprecation.html">Why
		'''        are Thread.stop, Thread.suspend and Thread.resume Deprecated?</a>. 
		<Obsolete("This method was originally designed to force a thread to stop"), MethodImpl(MethodImplOptions.Synchronized)> _
		Public Sub [stop](  obj As Throwable)
			Throw New UnsupportedOperationException
		End Sub

		''' <summary>
		''' Interrupts this thread.
		''' 
		''' <p> Unless the current thread is interrupting itself, which is
		''' always permitted, the <seealso cref="#checkAccess() checkAccess"/> method
		''' of this thread is invoked, which may cause a {@link
		''' SecurityException} to be thrown.
		''' 
		''' <p> If this thread is blocked in an invocation of the {@link
		''' Object#wait() wait()}, <seealso cref="Object#wait(long) wait(long)"/>, or {@link
		''' Object#wait(long, int) wait(long, int)} methods of the <seealso cref="Object"/>
		''' [Class], or of the <seealso cref="#join()"/>, <seealso cref="#join(long)"/>, {@link
		''' #join(long, int)}, <seealso cref="#sleep(long)"/>, or <seealso cref="#sleep(long, int)"/>,
		''' methods of this [Class], then its interrupt status will be cleared and it
		''' will receive an <seealso cref="InterruptedException"/>.
		''' 
		''' <p> If this thread is blocked in an I/O operation upon an {@link
		''' java.nio.channels.InterruptibleChannel InterruptibleChannel}
		''' then the channel will be closed, the thread's interrupt
		''' status will be set, and the thread will receive a {@link
		''' java.nio.channels.ClosedByInterruptException}.
		''' 
		''' <p> If this thread is blocked in a <seealso cref="java.nio.channels.Selector"/>
		''' then the thread's interrupt status will be set and it will return
		''' immediately from the selection operation, possibly with a non-zero
		''' value, just as if the selector's {@link
		''' java.nio.channels.Selector#wakeup wakeup} method were invoked.
		''' 
		''' <p> If none of the previous conditions hold then this thread's interrupt
		''' status will be set. </p>
		''' 
		''' <p> Interrupting a thread that is not alive need not have any effect.
		''' </summary>
		''' <exception cref="SecurityException">
		'''          if the current thread cannot modify this thread
		''' 
		''' @revised 6.0
		''' @spec JSR-51 </exception>
		Public Overridable Sub interrupt()
			If Me IsNot Thread.CurrentThread Then checkAccess()

			SyncLock blockerLock
				Dim b As sun.nio.ch.Interruptible = blocker
				If b IsNot Nothing Then
					interrupt0() ' Just to set the interrupt flag
					b.interrupt(Me)
					Return
				End If
			End SyncLock
			interrupt0()
		End Sub

		''' <summary>
		''' Tests whether the current thread has been interrupted.  The
		''' <i>interrupted status</i> of the thread is cleared by this method.  In
		''' other words, if this method were to be called twice in succession, the
		''' second call would return false (unless the current thread were
		''' interrupted again, after the first call had cleared its interrupted
		''' status and before the second call had examined it).
		''' 
		''' <p>A thread interruption ignored because a thread was not alive
		''' at the time of the interrupt will be reflected by this method
		''' returning false.
		''' </summary>
		''' <returns>  <code>true</code> if the current thread has been interrupted;
		'''          <code>false</code> otherwise. </returns>
		''' <seealso cref= #isInterrupted()
		''' @revised 6.0 </seealso>
		Public Shared Function interrupted() As Boolean
			Return currentThread().isInterrupted(True)
		End Function

		''' <summary>
		''' Tests whether this thread has been interrupted.  The <i>interrupted
		''' status</i> of the thread is unaffected by this method.
		''' 
		''' <p>A thread interruption ignored because a thread was not alive
		''' at the time of the interrupt will be reflected by this method
		''' returning false.
		''' </summary>
		''' <returns>  <code>true</code> if this thread has been interrupted;
		'''          <code>false</code> otherwise. </returns>
		''' <seealso cref=     #interrupted()
		''' @revised 6.0 </seealso>
		Public Overridable Property interrupted As Boolean
			Get
				Return isInterrupted(False)
			End Get
		End Property

		''' <summary>
		''' Tests if some Thread has been interrupted.  The interrupted state
		''' is reset or not based on the value of ClearInterrupted that is
		''' passed.
		''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Replace 'unknown' with the appropriate dll name:
		<DllImport("unknown")> _
		Private Function isInterrupted(  ClearInterrupted As Boolean) As Boolean
		End Function

		''' <summary>
		''' Throws <seealso cref="NoSuchMethodError"/>.
		''' </summary>
		''' @deprecated This method was originally designed to destroy this
		'''     thread without any cleanup. Any monitors it held would have
		'''     remained locked. However, the method was never implemented.
		'''     If if were to be implemented, it would be deadlock-prone in
		'''     much the manner of <seealso cref="#suspend"/>. If the target thread held
		'''     a lock protecting a critical system resource when it was
		'''     destroyed, no thread could ever access this resource again.
		'''     If another thread ever attempted to lock this resource, deadlock
		'''     would result. Such deadlocks typically manifest themselves as
		'''     "frozen" processes. For more information, see
		'''     <a href="{@docRoot}/../technotes/guides/concurrency/threadPrimitiveDeprecation.html">
		'''     Why are Thread.stop, Thread.suspend and Thread.resume Deprecated?</a>. 
		''' <exception cref="NoSuchMethodError"> always </exception>
		<Obsolete("This method was originally designed to destroy this")> _
		Public Overridable Sub destroy()
			Throw New NoSuchMethodError
		End Sub

		''' <summary>
		''' Tests if this thread is alive. A thread is alive if it has
		''' been started and has not yet died.
		''' </summary>
		''' <returns>  <code>true</code> if this thread is alive;
		'''          <code>false</code> otherwise. </returns>
'JAVA TO VB CONVERTER TODO TASK: Replace 'unknown' with the appropriate dll name:
		<DllImport("unknown")> _
		Public final Function isAlive() As Boolean
		End Function

		''' <summary>
		''' Suspends this thread.
		''' <p>
		''' First, the <code>checkAccess</code> method of this thread is called
		''' with no arguments. This may result in throwing a
		''' <code>SecurityException </code>(in the current thread).
		''' <p>
		''' If the thread is alive, it is suspended and makes no further
		''' progress unless and until it is resumed.
		''' </summary>
		''' <exception cref="SecurityException">  if the current thread cannot modify
		'''               this thread. </exception>
		''' <seealso cref= #checkAccess </seealso>
		''' @deprecated   This method has been deprecated, as it is
		'''   inherently deadlock-prone.  If the target thread holds a lock on the
		'''   monitor protecting a critical system resource when it is suspended, no
		'''   thread can access this resource until the target thread is resumed. If
		'''   the thread that would resume the target thread attempts to lock this
		'''   monitor prior to calling <code>resume</code>, deadlock results.  Such
		'''   deadlocks typically manifest themselves as "frozen" processes.
		'''   For more information, see
		'''   <a href="{@docRoot}/../technotes/guides/concurrency/threadPrimitiveDeprecation.html">Why
		'''   are Thread.stop, Thread.suspend and Thread.resume Deprecated?</a>. 
		<Obsolete("  This method has been deprecated, as it is")> _
		Public Sub suspend()
			checkAccess()
			suspend0()
		End Sub

		''' <summary>
		''' Resumes a suspended thread.
		''' <p>
		''' First, the <code>checkAccess</code> method of this thread is called
		''' with no arguments. This may result in throwing a
		''' <code>SecurityException</code> (in the current thread).
		''' <p>
		''' If the thread is alive but suspended, it is resumed and is
		''' permitted to make progress in its execution.
		''' </summary>
		''' <exception cref="SecurityException">  if the current thread cannot modify this
		'''               thread. </exception>
		''' <seealso cref=        #checkAccess </seealso>
		''' <seealso cref=        #suspend() </seealso>
		''' @deprecated This method exists solely for use with <seealso cref="#suspend"/>,
		'''     which has been deprecated because it is deadlock-prone.
		'''     For more information, see
		'''     <a href="{@docRoot}/../technotes/guides/concurrency/threadPrimitiveDeprecation.html">Why
		'''     are Thread.stop, Thread.suspend and Thread.resume Deprecated?</a>. 
		<Obsolete("This method exists solely for use with <seealso cref="#suspend"/>,")> _
		Public Sub [resume]()
			checkAccess()
			resume0()
		End Sub

		''' <summary>
		''' Changes the priority of this thread.
		''' <p>
		''' First the <code>checkAccess</code> method of this thread is called
		''' with no arguments. This may result in throwing a
		''' <code>SecurityException</code>.
		''' <p>
		''' Otherwise, the priority of this thread is set to the smaller of
		''' the specified <code>newPriority</code> and the maximum permitted
		''' priority of the thread's thread group.
		''' </summary>
		''' <param name="newPriority"> priority to set this thread to </param>
		''' <exception cref="IllegalArgumentException">  If the priority is not in the
		'''               range <code>MIN_PRIORITY</code> to
		'''               <code>MAX_PRIORITY</code>. </exception>
		''' <exception cref="SecurityException">  if the current thread cannot modify
		'''               this thread. </exception>
		''' <seealso cref=        #getPriority </seealso>
		''' <seealso cref=        #checkAccess() </seealso>
		''' <seealso cref=        #getThreadGroup() </seealso>
		''' <seealso cref=        #MAX_PRIORITY </seealso>
		''' <seealso cref=        #MIN_PRIORITY </seealso>
		''' <seealso cref=        ThreadGroup#getMaxPriority() </seealso>
		Public Property priority As Integer
			Set(  newPriority As Integer)
				Dim g As ThreadGroup
				checkAccess()
				If newPriority > MAX_PRIORITY OrElse newPriority < MIN_PRIORITY Then Throw New IllegalArgumentException
				g = threadGroup
				If g IsNot Nothing Then
					If newPriority > g.maxPriority Then newPriority = g.maxPriority
	'JAVA TO VB CONVERTER TODO TASK: Assignments within expressions are not supported in VB
					priority0 = priority = newPriority
				End If
			End Set
			Get
				Return priority
			End Get
		End Property


		''' <summary>
		''' Changes the name of this thread to be equal to the argument
		''' <code>name</code>.
		''' <p>
		''' First the <code>checkAccess</code> method of this thread is called
		''' with no arguments. This may result in throwing a
		''' <code>SecurityException</code>.
		''' </summary>
		''' <param name="name">   the new name for this thread. </param>
		''' <exception cref="SecurityException">  if the current thread cannot modify this
		'''               thread. </exception>
		''' <seealso cref=        #getName </seealso>
		''' <seealso cref=        #checkAccess() </seealso>
		<MethodImpl(MethodImplOptions.Synchronized)> _
		Public Property name As String
			Set(  name As String)
				checkAccess()
				Me.name = name.ToCharArray()
				If threadStatus <> 0 Then nativeName = name
			End Set
			Get
				Return New String(name, True)
			End Get
		End Property


		''' <summary>
		''' Returns the thread group to which this thread belongs.
		''' This method returns null if this thread has died
		''' (been stopped).
		''' </summary>
		''' <returns>  this thread's thread group. </returns>
		Public Property threadGroup As ThreadGroup
			Get
				Return group
			End Get
		End Property

		''' <summary>
		''' Returns an estimate of the number of active threads in the current
		''' thread's <seealso cref="java.lang.ThreadGroup thread group"/> and its
		''' subgroups. Recursively iterates over all subgroups in the current
		''' thread's thread group.
		''' 
		''' <p> The value returned is only an estimate because the number of
		''' threads may change dynamically while this method traverses internal
		''' data structures, and might be affected by the presence of certain
		''' system threads. This method is intended primarily for debugging
		''' and monitoring purposes.
		''' </summary>
		''' <returns>  an estimate of the number of active threads in the current
		'''          thread's thread group and in any other thread group that
		'''          has the current thread's thread group as an ancestor </returns>
		Public Shared Function activeCount() As Integer
			Return currentThread().threadGroup.activeCount()
		End Function

		''' <summary>
		''' Copies into the specified array every active thread in the current
		''' thread's thread group and its subgroups. This method simply
		''' invokes the <seealso cref="java.lang.ThreadGroup#enumerate(Thread[])"/>
		''' method of the current thread's thread group.
		''' 
		''' <p> An application might use the <seealso cref="#activeCount activeCount"/>
		''' method to get an estimate of how big the array should be, however
		''' <i>if the array is too short to hold all the threads, the extra threads
		''' are silently ignored.</i>  If it is critical to obtain every active
		''' thread in the current thread's thread group and its subgroups, the
		''' invoker should verify that the returned int value is strictly less
		''' than the length of {@code tarray}.
		''' 
		''' <p> Due to the inherent race condition in this method, it is recommended
		''' that the method only be used for debugging and monitoring purposes.
		''' </summary>
		''' <param name="tarray">
		'''         an array into which to put the list of threads
		''' </param>
		''' <returns>  the number of threads put into the array
		''' </returns>
		''' <exception cref="SecurityException">
		'''          if <seealso cref="java.lang.ThreadGroup#checkAccess"/> determines that
		'''          the current thread cannot access its thread group </exception>
		Public Shared Function enumerate(  tarray As Thread()) As Integer
			Return currentThread().threadGroup.enumerate(tarray)
		End Function

		''' <summary>
		''' Counts the number of stack frames in this thread. The thread must
		''' be suspended.
		''' </summary>
		''' <returns>     the number of stack frames in this thread. </returns>
		''' <exception cref="IllegalThreadStateException">  if this thread is not
		'''             suspended. </exception>
		''' @deprecated The definition of this call depends on <seealso cref="#suspend"/>,
		'''             which is deprecated.  Further, the results of this call
		'''             were never well-defined. 
'JAVA TO VB CONVERTER TODO TASK: Replace 'unknown' with the appropriate dll name:
		<DllImport("unknown")> _
		Public Function countStackFrames() As Integer
		End Function

		''' <summary>
		''' Waits at most {@code millis} milliseconds for this thread to
		''' die. A timeout of {@code 0} means to wait forever.
		''' 
		''' <p> This implementation uses a loop of {@code this.wait} calls
		''' conditioned on {@code this.isAlive}. As a thread terminates the
		''' {@code this.notifyAll} method is invoked. It is recommended that
		''' applications not use {@code wait}, {@code notify}, or
		''' {@code notifyAll} on {@code Thread} instances.
		''' </summary>
		''' <param name="millis">
		'''         the time to wait in milliseconds
		''' </param>
		''' <exception cref="IllegalArgumentException">
		'''          if the value of {@code millis} is negative
		''' </exception>
		''' <exception cref="InterruptedException">
		'''          if any thread has interrupted the current thread. The
		'''          <i>interrupted status</i> of the current thread is
		'''          cleared when this exception is thrown. </exception>
		<MethodImpl(MethodImplOptions.Synchronized)> _
		Public Sub join(  millis As Long)
			Dim base As Long = System.currentTimeMillis()
			Dim now As Long = 0

			If millis < 0 Then Throw New IllegalArgumentException("timeout value is negative")

			If millis = 0 Then
				Do While alive
					wait(0)
				Loop
			Else
				Do While alive
					Dim delay As Long = millis - now
					If delay <= 0 Then Exit Do
					wait(delay)
					now = System.currentTimeMillis() - base
				Loop
			End If
		End Sub

		''' <summary>
		''' Waits at most {@code millis} milliseconds plus
		''' {@code nanos} nanoseconds for this thread to die.
		''' 
		''' <p> This implementation uses a loop of {@code this.wait} calls
		''' conditioned on {@code this.isAlive}. As a thread terminates the
		''' {@code this.notifyAll} method is invoked. It is recommended that
		''' applications not use {@code wait}, {@code notify}, or
		''' {@code notifyAll} on {@code Thread} instances.
		''' </summary>
		''' <param name="millis">
		'''         the time to wait in milliseconds
		''' </param>
		''' <param name="nanos">
		'''         {@code 0-999999} additional nanoseconds to wait
		''' </param>
		''' <exception cref="IllegalArgumentException">
		'''          if the value of {@code millis} is negative, or the value
		'''          of {@code nanos} is not in the range {@code 0-999999}
		''' </exception>
		''' <exception cref="InterruptedException">
		'''          if any thread has interrupted the current thread. The
		'''          <i>interrupted status</i> of the current thread is
		'''          cleared when this exception is thrown. </exception>
		<MethodImpl(MethodImplOptions.Synchronized)> _
		Public Sub join(  millis As Long,   nanos As Integer)

			If millis < 0 Then Throw New IllegalArgumentException("timeout value is negative")

			If nanos < 0 OrElse nanos > 999999 Then Throw New IllegalArgumentException("nanosecond timeout value out of range")

			If nanos >= 500000 OrElse (nanos <> 0 AndAlso millis = 0) Then millis += 1

			join(millis)
		End Sub

		''' <summary>
		''' Waits for this thread to die.
		''' 
		''' <p> An invocation of this method behaves in exactly the same
		''' way as the invocation
		''' 
		''' <blockquote>
		''' <seealso cref="#join(long) join"/>{@code (0)}
		''' </blockquote>
		''' </summary>
		''' <exception cref="InterruptedException">
		'''          if any thread has interrupted the current thread. The
		'''          <i>interrupted status</i> of the current thread is
		'''          cleared when this exception is thrown. </exception>
		Public Sub join()
			join(0)
		End Sub

		''' <summary>
		''' Prints a stack trace of the current thread to the standard error stream.
		''' This method is used only for debugging.
		''' </summary>
		''' <seealso cref=     Throwable#printStackTrace() </seealso>
		Public Shared Sub dumpStack()
			CType(New Exception("Stack trace"), Exception).printStackTrace()
		End Sub

		''' <summary>
		''' Marks this thread as either a <seealso cref="#isDaemon daemon"/> thread
		''' or a user thread. The Java Virtual Machine exits when the only
		''' threads running are all daemon threads.
		''' 
		''' <p> This method must be invoked before the thread is started.
		''' </summary>
		''' <param name="on">
		'''         if {@code true}, marks this thread as a daemon thread
		''' </param>
		''' <exception cref="IllegalThreadStateException">
		'''          if this thread is <seealso cref="#isAlive alive"/>
		''' </exception>
		''' <exception cref="SecurityException">
		'''          if <seealso cref="#checkAccess"/> determines that the current
		'''          thread cannot modify this thread </exception>
		Public Property daemon As Boolean
			Set(  [on] As Boolean)
				checkAccess()
				If alive Then Throw New IllegalThreadStateException
				daemon = [on]
			End Set
			Get
				Return daemon
			End Get
		End Property


		''' <summary>
		''' Determines if the currently running thread has permission to
		''' modify this thread.
		''' <p>
		''' If there is a security manager, its <code>checkAccess</code> method
		''' is called with this thread as its argument. This may result in
		''' throwing a <code>SecurityException</code>.
		''' </summary>
		''' <exception cref="SecurityException">  if the current thread is not allowed to
		'''               access this thread. </exception>
		''' <seealso cref=        SecurityManager#checkAccess(Thread) </seealso>
		Public Sub checkAccess()
			Dim security As SecurityManager = System.securityManager
			If security IsNot Nothing Then security.checkAccess(Me)
		End Sub

		''' <summary>
		''' Returns a string representation of this thread, including the
		''' thread's name, priority, and thread group.
		''' </summary>
		''' <returns>  a string representation of this thread. </returns>
		Public Overrides Function ToString() As String
			Dim group As ThreadGroup = threadGroup
			If group IsNot Nothing Then
				Return "Thread[" & name & "," & priority & "," & group.name & "]"
			Else
				Return "Thread[" & name & "," & priority & "," & "" & "]"
			End If
		End Function

		''' <summary>
		''' Returns the context ClassLoader for this Thread. The context
		''' ClassLoader is provided by the creator of the thread for use
		''' by code running in this thread when loading classes and resources.
		''' If not <seealso cref="#setContextClassLoader set"/>, the default is the
		''' ClassLoader context of the parent Thread. The context ClassLoader of the
		''' primordial thread is typically set to the class loader used to load the
		''' application.
		''' 
		''' <p>If a security manager is present, and the invoker's class loader is not
		''' {@code null} and is not the same as or an ancestor of the context class
		''' loader, then this method invokes the security manager's {@link
		''' SecurityManager#checkPermission(java.security.Permission) checkPermission}
		''' method with a <seealso cref="RuntimePermission RuntimePermission"/>{@code
		''' ("getClassLoader")} permission to verify that retrieval of the context
		''' class loader is permitted.
		''' </summary>
		''' <returns>  the context ClassLoader for this Thread, or {@code null}
		'''          indicating the system class loader (or, failing that, the
		'''          bootstrap class loader)
		''' </returns>
		''' <exception cref="SecurityException">
		'''          if the current thread cannot get the context ClassLoader
		''' 
		''' @since 1.2 </exception>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
		Public Overridable Property contextClassLoader As  ClassLoader
			Get
				If contextClassLoader Is Nothing Then Return Nothing
				Dim sm As SecurityManager = System.securityManager
				If sm IsNot Nothing Then ClassLoader.checkClassLoaderPermission(contextClassLoader, sun.reflect.Reflection.callerClass)
				Return contextClassLoader
			End Get
			Set(  cl As  ClassLoader)
				Dim sm As SecurityManager = System.securityManager
				If sm IsNot Nothing Then sm.checkPermission(New RuntimePermission("setContextClassLoader"))
				contextClassLoader = cl
			End Set
		End Property


		''' <summary>
		''' Returns <tt>true</tt> if and only if the current thread holds the
		''' monitor lock on the specified object.
		''' 
		''' <p>This method is designed to allow a program to assert that
		''' the current thread already holds a specified lock:
		''' <pre>
		'''     assert Thread.holdsLock(obj);
		''' </pre>
		''' </summary>
		''' <param name="obj"> the object on which to test lock ownership </param>
		''' <exception cref="NullPointerException"> if obj is <tt>null</tt> </exception>
		''' <returns> <tt>true</tt> if the current thread holds the monitor lock on
		'''         the specified object.
		''' @since 1.4 </returns>
'JAVA TO VB CONVERTER TODO TASK: Replace 'unknown' with the appropriate dll name:
		<DllImport("unknown")> _
		Public Shared Function holdsLock(  obj As Object) As Boolean
		End Function

		Private Shared ReadOnly EMPTY_STACK_TRACE As StackTraceElement() = New StackTraceElement(){}

		''' <summary>
		''' Returns an array of stack trace elements representing the stack dump
		''' of this thread.  This method will return a zero-length array if
		''' this thread has not started, has started but has not yet been
		''' scheduled to run by the system, or has terminated.
		''' If the returned array is of non-zero length then the first element of
		''' the array represents the top of the stack, which is the most recent
		''' method invocation in the sequence.  The last element of the array
		''' represents the bottom of the stack, which is the least recent method
		''' invocation in the sequence.
		''' 
		''' <p>If there is a security manager, and this thread is not
		''' the current thread, then the security manager's
		''' <tt>checkPermission</tt> method is called with a
		''' <tt>RuntimePermission("getStackTrace")</tt> permission
		''' to see if it's ok to get the stack trace.
		''' 
		''' <p>Some virtual machines may, under some circumstances, omit one
		''' or more stack frames from the stack trace.  In the extreme case,
		''' a virtual machine that has no stack trace information concerning
		''' this thread is permitted to return a zero-length array from this
		''' method.
		''' </summary>
		''' <returns> an array of <tt>StackTraceElement</tt>,
		''' each represents one stack frame.
		''' </returns>
		''' <exception cref="SecurityException">
		'''        if a security manager exists and its
		'''        <tt>checkPermission</tt> method doesn't allow
		'''        getting the stack trace of thread. </exception>
		''' <seealso cref= SecurityManager#checkPermission </seealso>
		''' <seealso cref= RuntimePermission </seealso>
		''' <seealso cref= Throwable#getStackTrace
		''' 
		''' @since 1.5 </seealso>
		Public Overridable Property stackTrace As StackTraceElement()
			Get
				If Me IsNot Thread.CurrentThread Then
					' check for getStackTrace permission
					Dim security As SecurityManager = System.securityManager
					If security IsNot Nothing Then security.checkPermission(sun.security.util.SecurityConstants.GET_STACK_TRACE_PERMISSION)
					' optimization so we do not call into the vm for threads that
					' have not yet started or have terminated
					If Not alive Then Return EMPTY_STACK_TRACE
					Dim stackTraceArray As StackTraceElement()() = dumpThreads(New Thread() { Me })
					Dim stackTrace_Renamed As StackTraceElement() = stackTraceArray(0)
					' a thread that was alive during the previous isAlive call may have
					' since terminated, therefore not having a stacktrace.
					If stackTrace_Renamed Is Nothing Then stackTrace_Renamed = EMPTY_STACK_TRACE
					Return stackTrace_Renamed
				Else
					' Don't need JVM help for current thread
					Return (New Exception).stackTrace
				End If
			End Get
		End Property

		''' <summary>
		''' Returns a map of stack traces for all live threads.
		''' The map keys are threads and each map value is an array of
		''' <tt>StackTraceElement</tt> that represents the stack dump
		''' of the corresponding <tt>Thread</tt>.
		''' The returned stack traces are in the format specified for
		''' the <seealso cref="#getStackTrace getStackTrace"/> method.
		''' 
		''' <p>The threads may be executing while this method is called.
		''' The stack trace of each thread only represents a snapshot and
		''' each stack trace may be obtained at different time.  A zero-length
		''' array will be returned in the map value if the virtual machine has
		''' no stack trace information about a thread.
		''' 
		''' <p>If there is a security manager, then the security manager's
		''' <tt>checkPermission</tt> method is called with a
		''' <tt>RuntimePermission("getStackTrace")</tt> permission as well as
		''' <tt>RuntimePermission("modifyThreadGroup")</tt> permission
		''' to see if it is ok to get the stack trace of all threads.
		''' </summary>
		''' <returns> a <tt>Map</tt> from <tt>Thread</tt> to an array of
		''' <tt>StackTraceElement</tt> that represents the stack trace of
		''' the corresponding thread.
		''' </returns>
		''' <exception cref="SecurityException">
		'''        if a security manager exists and its
		'''        <tt>checkPermission</tt> method doesn't allow
		'''        getting the stack trace of thread. </exception>
		''' <seealso cref= #getStackTrace </seealso>
		''' <seealso cref= SecurityManager#checkPermission </seealso>
		''' <seealso cref= RuntimePermission </seealso>
		''' <seealso cref= Throwable#getStackTrace
		''' 
		''' @since 1.5 </seealso>
		PublicShared ReadOnly PropertyallStackTraces As IDictionary(Of Thread, StackTraceElement())
			Get
				' check for getStackTrace permission
				Dim security As SecurityManager = System.securityManager
				If security IsNot Nothing Then
					security.checkPermission(sun.security.util.SecurityConstants.GET_STACK_TRACE_PERMISSION)
					security.checkPermission(sun.security.util.SecurityConstants.MODIFY_THREADGROUP_PERMISSION)
				End If
    
				' Get a snapshot of the list of all threads
				Dim threads As Thread() = threads
				Dim traces As StackTraceElement()() = dumpThreads(threads)
				Dim m As IDictionary(Of Thread, StackTraceElement()) = New Dictionary(Of Thread, StackTraceElement())(threads.Length)
				For i As Integer = 0 To threads.Length - 1
					Dim stackTrace_Renamed As StackTraceElement() = traces(i)
					If stackTrace_Renamed IsNot Nothing Then m(threads(i)) = stackTrace_Renamed
					' else terminated so we don't put it in the map
				Next i
				Return m
			End Get
		End Property


		Private Shared ReadOnly SUBCLASS_IMPLEMENTATION_PERMISSION As New RuntimePermission("enableContextClassLoaderOverride")

		''' <summary>
		''' cache of subclass security audit results </summary>
	'     Replace with ConcurrentReferenceHashMap when/if it appears in a future
	'     * release 
		Private Class Caches
			''' <summary>
			''' cache of subclass security audit results </summary>
			Friend Shared ReadOnly subclassAudits As java.util.concurrent.ConcurrentMap(Of WeakClassKey, Boolean?) = New ConcurrentDictionary(Of WeakClassKey, Boolean?)

			''' <summary>
			''' queue for WeakReferences to audited subclasses </summary>
			Friend Shared ReadOnly subclassAuditsQueue As New ReferenceQueue(Of [Class])
		End Class

		''' <summary>
		''' Verifies that this (possibly subclass) instance can be constructed
		''' without violating security constraints: the subclass must not override
		''' security-sensitive non-final methods, or else the
		''' "enableContextClassLoaderOverride" RuntimePermission is checked.
		''' </summary>
		Private Shared Function isCCLOverridden(  cl As [Class]) As Boolean
			If cl Is GetType(Thread) Then Return False

			processQueue(Caches.subclassAuditsQueue, Caches.subclassAudits)
			Dim key As New WeakClassKey(cl, Caches.subclassAuditsQueue)
			Dim result As Boolean? = Caches.subclassAudits.get(key)
			If result Is Nothing Then
				result = Convert.ToBoolean(auditSubclass(cl))
				Caches.subclassAudits.putIfAbsent(key, result)
			End If

			Return result
		End Function

		''' <summary>
		''' Performs reflective checks on given subclass to verify that it doesn't
		''' override security-sensitive non-final methods.  Returns true if the
		''' subclass overrides any of the methods, false otherwise.
		''' </summary>
		Private Shared Function auditSubclass(  subcl As [Class]) As Boolean
			Dim result As Boolean? = java.security.AccessController.doPrivileged(New PrivilegedActionAnonymousInnerClassHelper(Of T)
		   )
			Return result
		End Function

		Private Class PrivilegedActionAnonymousInnerClassHelper(Of T)
			Implements java.security.PrivilegedAction(Of T)

			Public Overridable Function run() As Boolean?
				Dim cl As  [Class] = subcl
				Do While cl IsNot GetType(Thread)
					Try
						cl.getDeclaredMethod("getContextClassLoader", New [Class](){})
						Return  java.lang.[Boolean].TRUE
					Catch ex As NoSuchMethodException
					End Try
					Try
						Dim params As  [Class]() = {GetType(ClassLoader)}
						cl.getDeclaredMethod("setContextClassLoader", params)
						Return  java.lang.[Boolean].TRUE
					Catch ex As NoSuchMethodException
					End Try
					cl = cl.BaseType
				Loop
				Return  java.lang.[Boolean].FALSE
			End Function
		End Class

'JAVA TO VB CONVERTER TODO TASK: Replace 'unknown' with the appropriate dll name:
		<DllImport("unknown")> _
		Private Shared Function dumpThreads(  threads As Thread()) As StackTraceElement()()
		End Function
'JAVA TO VB CONVERTER TODO TASK: Replace 'unknown' with the appropriate dll name:
		<DllImport("unknown")> _
		Private Shared Function getThreads() As Thread()
		End Function

		''' <summary>
		''' Returns the identifier of this Thread.  The thread ID is a positive
		''' <tt>long</tt> number generated when this thread was created.
		''' The thread ID is unique and remains unchanged during its lifetime.
		''' When a thread is terminated, this thread ID may be reused.
		''' </summary>
		''' <returns> this thread's ID.
		''' @since 1.5 </returns>
		Public Overridable Property id As Long
			Get
				Return tid
			End Get
		End Property

		''' <summary>
		''' A thread state.  A thread can be in one of the following states:
		''' <ul>
		''' <li><seealso cref="#NEW"/><br>
		'''     A thread that has not yet started is in this state.
		'''     </li>
		''' <li><seealso cref="#RUNNABLE"/><br>
		'''     A thread executing in the Java virtual machine is in this state.
		'''     </li>
		''' <li><seealso cref="#BLOCKED"/><br>
		'''     A thread that is blocked waiting for a monitor lock
		'''     is in this state.
		'''     </li>
		''' <li><seealso cref="#WAITING"/><br>
		'''     A thread that is waiting indefinitely for another thread to
		'''     perform a particular action is in this state.
		'''     </li>
		''' <li><seealso cref="#TIMED_WAITING"/><br>
		'''     A thread that is waiting for another thread to perform an action
		'''     for up to a specified waiting time is in this state.
		'''     </li>
		''' <li><seealso cref="#TERMINATED"/><br>
		'''     A thread that has exited is in this state.
		'''     </li>
		''' </ul>
		''' 
		''' <p>
		''' A thread can be in only one state at a given point in time.
		''' These states are virtual machine states which do not reflect
		''' any operating system thread states.
		''' 
		''' @since   1.5 </summary>
		''' <seealso cref= #getState </seealso>
		Public Enum State
			''' <summary>
			''' Thread state for a thread which has not yet started.
			''' </summary>
			[NEW]

			''' <summary>
			''' Thread state for a runnable thread.  A thread in the runnable
			''' state is executing in the Java virtual machine but it may
			''' be waiting for other resources from the operating system
			''' such as processor.
			''' </summary>
			RUNNABLE

			''' <summary>
			''' Thread state for a thread blocked waiting for a monitor lock.
			''' A thread in the blocked state is waiting for a monitor lock
			''' to enter a synchronized block/method or
			''' reenter a synchronized block/method after calling
			''' <seealso cref="Object#wait() Object.wait"/>.
			''' </summary>
			BLOCKED

			''' <summary>
			''' Thread state for a waiting thread.
			''' A thread is in the waiting state due to calling one of the
			''' following methods:
			''' <ul>
			'''   <li><seealso cref="Object#wait() Object.wait"/> with no timeout</li>
			'''   <li><seealso cref="#join() Thread.join"/> with no timeout</li>
			'''   <li><seealso cref="LockSupport#park() LockSupport.park"/></li>
			''' </ul>
			''' 
			''' <p>A thread in the waiting state is waiting for another thread to
			''' perform a particular action.
			''' 
			''' For example, a thread that has called <tt>Object.wait()</tt>
			''' on an object is waiting for another thread to call
			''' <tt>Object.notify()</tt> or <tt>Object.notifyAll()</tt> on
			''' that object. A thread that has called <tt>Thread.join()</tt>
			''' is waiting for a specified thread to terminate.
			''' </summary>
			WAITING

			''' <summary>
			''' Thread state for a waiting thread with a specified waiting time.
			''' A thread is in the timed waiting state due to calling one of
			''' the following methods with a specified positive waiting time:
			''' <ul>
			'''   <li><seealso cref="#sleep Thread.sleep"/></li>
			'''   <li><seealso cref="Object#wait(long) Object.wait"/> with timeout</li>
			'''   <li><seealso cref="#join(long) Thread.join"/> with timeout</li>
			'''   <li><seealso cref="LockSupport#parkNanos LockSupport.parkNanos"/></li>
			'''   <li><seealso cref="LockSupport#parkUntil LockSupport.parkUntil"/></li>
			''' </ul>
			''' </summary>
			TIMED_WAITING

			''' <summary>
			''' Thread state for a terminated thread.
			''' The thread has completed execution.
			''' </summary>
			TERMINATED
		End Enum

		''' <summary>
		''' Returns the state of this thread.
		''' This method is designed for use in monitoring of the system state,
		''' not for synchronization control.
		''' </summary>
		''' <returns> this thread's state.
		''' @since 1.5 </returns>
		Public Overridable Property state As State
			Get
				' get current thread state
				Return sun.misc.VM.toThreadState(threadStatus)
			End Get
		End Property

		' Added in JSR-166

		''' <summary>
		''' Interface for handlers invoked when a <tt>Thread</tt> abruptly
		''' terminates due to an uncaught exception.
		''' <p>When a thread is about to terminate due to an uncaught exception
		''' the Java Virtual Machine will query the thread for its
		''' <tt>UncaughtExceptionHandler</tt> using
		''' <seealso cref="#getUncaughtExceptionHandler"/> and will invoke the handler's
		''' <tt>uncaughtException</tt> method, passing the thread and the
		''' exception as arguments.
		''' If a thread has not had its <tt>UncaughtExceptionHandler</tt>
		''' explicitly set, then its <tt>ThreadGroup</tt> object acts as its
		''' <tt>UncaughtExceptionHandler</tt>. If the <tt>ThreadGroup</tt> object
		''' has no
		''' special requirements for dealing with the exception, it can forward
		''' the invocation to the {@link #getDefaultUncaughtExceptionHandler
		''' default uncaught exception handler}.
		''' </summary>
		''' <seealso cref= #setDefaultUncaughtExceptionHandler </seealso>
		''' <seealso cref= #setUncaughtExceptionHandler </seealso>
		''' <seealso cref= ThreadGroup#uncaughtException
		''' @since 1.5 </seealso>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
		Public Interface UncaughtExceptionHandler
			''' <summary>
			''' Method invoked when the given thread terminates due to the
			''' given uncaught exception.
			''' <p>Any exception thrown by this method will be ignored by the
			''' Java Virtual Machine. </summary>
			''' <param name="t"> the thread </param>
			''' <param name="e"> the exception </param>
			Sub uncaughtException(  t As Thread,   e As Throwable)
		End Interface

		' null unless explicitly set
'JAVA TO VB CONVERTER TODO TASK: There is no VB equivalent to 'volatile':
		Private uncaughtExceptionHandler As UncaughtExceptionHandler

		' null unless explicitly set
'JAVA TO VB CONVERTER TODO TASK: There is no VB equivalent to 'volatile':
		Private Shared defaultUncaughtExceptionHandler As UncaughtExceptionHandler

		''' <summary>
		''' Set the default handler invoked when a thread abruptly terminates
		''' due to an uncaught exception, and no other handler has been defined
		''' for that thread.
		''' 
		''' <p>Uncaught exception handling is controlled first by the thread, then
		''' by the thread's <seealso cref="ThreadGroup"/> object and finally by the default
		''' uncaught exception handler. If the thread does not have an explicit
		''' uncaught exception handler set, and the thread's thread group
		''' (including parent thread groups)  does not specialize its
		''' <tt>uncaughtException</tt> method, then the default handler's
		''' <tt>uncaughtException</tt> method will be invoked.
		''' <p>By setting the default uncaught exception handler, an application
		''' can change the way in which uncaught exceptions are handled (such as
		''' logging to a specific device, or file) for those threads that would
		''' already accept whatever &quot;default&quot; behavior the system
		''' provided.
		''' 
		''' <p>Note that the default uncaught exception handler should not usually
		''' defer to the thread's <tt>ThreadGroup</tt> object, as that could cause
		''' infinite recursion.
		''' </summary>
		''' <param name="eh"> the object to use as the default uncaught exception handler.
		''' If <tt>null</tt> then there is no default handler.
		''' </param>
		''' <exception cref="SecurityException"> if a security manager is present and it
		'''         denies <tt><seealso cref="RuntimePermission"/>
		'''         (&quot;setDefaultUncaughtExceptionHandler&quot;)</tt>
		''' </exception>
		''' <seealso cref= #setUncaughtExceptionHandler </seealso>
		''' <seealso cref= #getUncaughtExceptionHandler </seealso>
		''' <seealso cref= ThreadGroup#uncaughtException
		''' @since 1.5 </seealso>
		Public Shared Property defaultUncaughtExceptionHandler As UncaughtExceptionHandler
			Set(  eh As UncaughtExceptionHandler)
				Dim sm As SecurityManager = System.securityManager
				If sm IsNot Nothing Then
					sm.checkPermission(New RuntimePermission("setDefaultUncaughtExceptionHandler")
						   )
				End If
    
				 defaultUncaughtExceptionHandler = eh
			End Set
			Get
				Return defaultUncaughtExceptionHandler
			End Get
		End Property


		''' <summary>
		''' Returns the handler invoked when this thread abruptly terminates
		''' due to an uncaught exception. If this thread has not had an
		''' uncaught exception handler explicitly set then this thread's
		''' <tt>ThreadGroup</tt> object is returned, unless this thread
		''' has terminated, in which case <tt>null</tt> is returned.
		''' @since 1.5 </summary>
		''' <returns> the uncaught exception handler for this thread </returns>
		Public Overridable Property uncaughtExceptionHandler As UncaughtExceptionHandler
			Get
				Return If(uncaughtExceptionHandler IsNot Nothing, uncaughtExceptionHandler, group)
			End Get
			Set(  eh As UncaughtExceptionHandler)
				checkAccess()
				uncaughtExceptionHandler = eh
			End Set
		End Property


		''' <summary>
		''' Dispatch an uncaught exception to the handler. This method is
		''' intended to be called only by the JVM.
		''' </summary>
		Private Sub dispatchUncaughtException(  e As Throwable)
			uncaughtExceptionHandler.uncaughtException(Me, e)
		End Sub

		''' <summary>
		''' Removes from the specified map any keys that have been enqueued
		''' on the specified reference queue.
		''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
		Friend Shared Sub processQueue(Of T1 As WeakReference(Of [Class]), ?)(  queue As ReferenceQueue(Of [Class]),   map As java.util.concurrent.ConcurrentMap(Of T1))
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
			Dim ref As Reference(Of ? As [Class])
			ref = queue.poll()
			Do While ref IsNot Nothing
				map.remove(ref)
				ref = queue.poll()
			Loop
		End Sub

		''' <summary>
		'''  Weak key for Class objects.
		''' 
		''' </summary>
		Friend Class WeakClassKey
			Inherits WeakReference(Of [Class])

			''' <summary>
			''' saved value of the referent's identity hash code, to maintain
			''' a consistent hash code after the referent has been cleared
			''' </summary>
			Private ReadOnly hash As Integer

			''' <summary>
			''' Create a new WeakClassKey to the given object, registered
			''' with a queue.
			''' </summary>
			Friend Sub New(  cl As [Class],   refQueue As ReferenceQueue(Of [Class]))
				MyBase.New(cl, refQueue)
				hash = System.identityHashCode(cl)
			End Sub

			''' <summary>
			''' Returns the identity hash code of the original referent.
			''' </summary>
			Public Overrides Function GetHashCode() As Integer
				Return hash
			End Function

			''' <summary>
			''' Returns true if the given object is this identical
			''' WeakClassKey instance, or, if this object's referent has not
			''' been cleared, if the given object is another WeakClassKey
			''' instance with the identical non-null referent as this one.
			''' </summary>
			Public Overrides Function Equals(  obj As Object) As Boolean
				If obj Is Me Then Return True

				If TypeOf obj Is WeakClassKey Then
					Dim referent As Object = get()
					Return (referent IsNot Nothing) AndAlso (referent Is CType(obj, WeakClassKey).get())
				Else
					Return False
				End If
			End Function
		End Class


		' The following three initially uninitialized fields are exclusively
		' managed by class java.util.concurrent.ThreadLocalRandom. These
		' fields are used to build the high-performance PRNGs in the
		' concurrent code, and we can not risk accidental false sharing.
		' Hence, the fields are isolated with @Contended.

		''' <summary>
		''' The current seed for a ThreadLocalRandom </summary>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
		Friend threadLocalRandomSeed As Long

		''' <summary>
		''' Probe hash value; nonzero if threadLocalRandomSeed initialized </summary>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
		Friend threadLocalRandomProbe As Integer

		''' <summary>
		''' Secondary seed isolated from public ThreadLocalRandom sequence </summary>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
		Friend threadLocalRandomSecondarySeed As Integer

		' Some private helper methods 
'JAVA TO VB CONVERTER TODO TASK: Replace 'unknown' with the appropriate dll name:
		<DllImport("unknown")> _
		Private Sub setPriority0(  newPriority As Integer)
		End Sub
'JAVA TO VB CONVERTER TODO TASK: Replace 'unknown' with the appropriate dll name:
		<DllImport("unknown")> _
		Private Sub stop0(  o As Object)
		End Sub
'JAVA TO VB CONVERTER TODO TASK: Replace 'unknown' with the appropriate dll name:
		<DllImport("unknown")> _
		Private Sub suspend0()
		End Sub
'JAVA TO VB CONVERTER TODO TASK: Replace 'unknown' with the appropriate dll name:
		<DllImport("unknown")> _
		Private Sub resume0()
		End Sub
'JAVA TO VB CONVERTER TODO TASK: Replace 'unknown' with the appropriate dll name:
		<DllImport("unknown")> _
		Private Sub interrupt0()
		End Sub
'JAVA TO VB CONVERTER TODO TASK: Replace 'unknown' with the appropriate dll name:
		<DllImport("unknown")> _
		Private Sub setNativeName(  name As String)
		End Sub
	End Class

End Namespace