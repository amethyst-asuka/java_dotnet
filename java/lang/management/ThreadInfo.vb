Imports Microsoft.VisualBasic
Imports System
Imports System.Diagnostics
Imports System.Collections.Generic
Imports Thread.State

'
' * Copyright (c) 2003, 2013, Oracle and/or its affiliates. All rights reserved.
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

Namespace java.lang.management

	''' <summary>
	''' Thread information. <tt>ThreadInfo</tt> contains the information
	''' about a thread including:
	''' <h3>General thread information</h3>
	''' <ul>
	'''   <li>Thread ID.</li>
	'''   <li>Name of the thread.</li>
	''' </ul>
	''' 
	''' <h3>Execution information</h3>
	''' <ul>
	'''   <li>Thread state.</li>
	'''   <li>The object upon which the thread is blocked due to:
	'''       <ul>
	'''       <li>waiting to enter a synchronization block/method, or</li>
	'''       <li>waiting to be notified in a <seealso cref="Object#wait Object.wait"/> method,
	'''           or</li>
	'''       <li>parking due to a {@link java.util.concurrent.locks.LockSupport#park
	'''           LockSupport.park} call.</li>
	'''       </ul>
	'''   </li>
	'''   <li>The ID of the thread that owns the object
	'''       that the thread is blocked.</li>
	'''   <li>Stack trace of the thread.</li>
	'''   <li>List of object monitors locked by the thread.</li>
	'''   <li>List of <a href="LockInfo.html#OwnableSynchronizer">
	'''       ownable synchronizers</a> locked by the thread.</li>
	''' </ul>
	''' 
	''' <h4><a name="SyncStats">Synchronization Statistics</a></h4>
	''' <ul>
	'''   <li>The number of times that the thread has blocked for
	'''       synchronization or waited for notification.</li>
	'''   <li>The accumulated elapsed time that the thread has blocked
	'''       for synchronization or waited for notification
	'''       since {@link ThreadMXBean#setThreadContentionMonitoringEnabled
	'''       thread contention monitoring}
	'''       was enabled. Some Java virtual machine implementation
	'''       may not support this.  The
	'''       <seealso cref="ThreadMXBean#isThreadContentionMonitoringSupported()"/>
	'''       method can be used to determine if a Java virtual machine
	'''       supports this.</li>
	''' </ul>
	''' 
	''' <p>This thread information class is designed for use in monitoring of
	''' the system, not for synchronization control.
	''' 
	''' <h4>MXBean Mapping</h4>
	''' <tt>ThreadInfo</tt> is mapped to a <seealso cref="CompositeData CompositeData"/>
	''' with attributes as specified in
	''' the <seealso cref="#from from"/> method.
	''' </summary>
	''' <seealso cref= ThreadMXBean#getThreadInfo </seealso>
	''' <seealso cref= ThreadMXBean#dumpAllThreads
	''' 
	''' @author  Mandy Chung
	''' @since   1.5 </seealso>

	Public Class ThreadInfo
		Private threadName As String

		Private Class MXBeanFetcherAnonymousInnerClassHelper(Of T)
			Implements MXBeanFetcher(Of T)

			Public Overridable Property mXBeans As IList(Of ClassLoadingMXBean)
				Get
					Return Collections.singletonList(sun.management.ManagementFactoryHelper.classLoadingMXBean)
				End Get
			End Property
		End Class

		Private Class MXBeanFetcherAnonymousInnerClassHelper2(Of T)
			Implements MXBeanFetcher(Of T)

			Public Overridable Property mXBeans As IList(Of CompilationMXBean)
				Get
					Dim m As CompilationMXBean = sun.management.ManagementFactoryHelper.compilationMXBean
					If m Is Nothing Then
					   Return Collections.emptyList()
					Else
					   Return Collections.singletonList(m)
					End If
				End Get
			End Property
		End Class

		Private Class MXBeanFetcherAnonymousInnerClassHelper3(Of T)
			Implements MXBeanFetcher(Of T)

			Public Overridable Property mXBeans As IList(Of MemoryMXBean)
				Get
					Return Collections.singletonList(sun.management.ManagementFactoryHelper.memoryMXBean)
				End Get
			End Property
		End Class

		Private Class MXBeanFetcherAnonymousInnerClassHelper4(Of T)
			Implements MXBeanFetcher(Of T)

			Public Overridable Property mXBeans As IList(Of GarbageCollectorMXBean)
				Get
					Return sun.management.ManagementFactoryHelper.garbageCollectorMXBeans
				End Get
			End Property
		End Class

		Private Class MXBeanFetcherAnonymousInnerClassHelper5(Of T)
			Implements MXBeanFetcher(Of T)

			Public Overridable Property mXBeans As IList(Of MemoryManagerMXBean)
				Get
					Return sun.management.ManagementFactoryHelper.memoryManagerMXBeans
				End Get
			End Property
		End Class

		Private Class MXBeanFetcherAnonymousInnerClassHelper6(Of T)
			Implements MXBeanFetcher(Of T)

			''' <summary>
			''' Memory pool in the Java virtual machine.
			''' </summary>
			Public Overridable Property mXBeans As IList(Of MemoryPoolMXBean)
				Get
					Return sun.management.ManagementFactoryHelper.memoryPoolMXBeans
				End Get
			End Property
		End Class

		Private Class MXBeanFetcherAnonymousInnerClassHelper7(Of T)
			Implements MXBeanFetcher(Of T)

			Public Overridable Property mXBeans As IList(Of OperatingSystemMXBean)
				Get
					Return Collections.singletonList(sun.management.ManagementFactoryHelper.operatingSystemMXBean)
				End Get
			End Property
		End Class

		Private Class MXBeanFetcherAnonymousInnerClassHelper8(Of T)
			Implements MXBeanFetcher(Of T)

			Public Overridable Property mXBeans As IList(Of RuntimeMXBean)
				Get
					Return Collections.singletonList(sun.management.ManagementFactoryHelper.runtimeMXBean)
				End Get
			End Property
		End Class

		Private Class MXBeanFetcherAnonymousInnerClassHelper9(Of T)
			Implements MXBeanFetcher(Of T)

			Public Overridable Property mXBeans As IList(Of ThreadMXBean)
				Get
					Return Collections.singletonList(sun.management.ManagementFactoryHelper.threadMXBean)
				End Get
			End Property
		End Class

		Private Class MXBeanFetcherAnonymousInnerClassHelper10(Of T)
			Implements MXBeanFetcher(Of T)

			Public Overridable Property mXBeans As IList(Of PlatformLoggingMXBean)
				Get
					Dim m As PlatformLoggingMXBean = sun.management.ManagementFactoryHelper.platformLoggingMXBean
					If m Is Nothing Then
					   Return Collections.emptyList()
					Else
					   Return Collections.singletonList(m)
					End If
				End Get
			End Property
		End Class

		Private Class MXBeanFetcherAnonymousInnerClassHelper11(Of T)
			Implements MXBeanFetcher(Of T)

			Public Overridable Property mXBeans As IList(Of BufferPoolMXBean)
				Get
					Return sun.management.ManagementFactoryHelper.bufferPoolMXBeans
				End Get
			End Property
		End Class

		Private Class MXBeanFetcherAnonymousInnerClassHelper12(Of T)
			Implements MXBeanFetcher(Of T)

			Public Overridable Property mXBeans As IList(Of com.sun.management.GarbageCollectorMXBean)
				Get
					Return getGcMXBeanList(GetType(com.sun.management.GarbageCollectorMXBean))
				End Get
			End Property
		End Class

		Private Class MXBeanFetcherAnonymousInnerClassHelper13(Of T)
			Implements MXBeanFetcher(Of T)

			Public Overridable Property mXBeans As IList(Of com.sun.management.OperatingSystemMXBean)
				Get
					Return getOSMXBeanList(GetType(com.sun.management.OperatingSystemMXBean))
				End Get
			End Property
		End Class

		Private Class MXBeanFetcherAnonymousInnerClassHelper14(Of T)
			Implements MXBeanFetcher(Of T)

			Public Overridable Property mXBeans As IList(Of UnixOperatingSystemMXBean)
				Get
					Return getOSMXBeanList(GetType(com.sun.management.UnixOperatingSystemMXBean))
				End Get
			End Property
		End Class

		Private Class MXBeanFetcherAnonymousInnerClassHelper15(Of T)
			Implements MXBeanFetcher(Of T)

			Public Overridable Property mXBeans As IList(Of HotSpotDiagnosticMXBean)
				Get
					Return Collections.singletonList(sun.management.ManagementFactoryHelper.diagnosticMXBean)
				End Get
			End Property
		End Class
		Private threadId As Long
		Private blockedTime As Long
		Private blockedCount As Long
		Private waitedTime As Long
		Private waitedCount As Long
		Private lock As LockInfo
		Private lockName As String
		Private lockOwnerId As Long
		Private lockOwnerName As String
		Private inNative As Boolean
		Private suspended As Boolean
		Private threadState As Thread.State
		Private stackTrace As StackTraceElement()
		Private lockedMonitors As MonitorInfo()
		Private lockedSynchronizers As LockInfo()

		Private Shared EMPTY_MONITORS As MonitorInfo() = New MonitorInfo(){}
		Private Shared EMPTY_SYNCS As LockInfo() = New LockInfo(){}

		''' <summary>
		''' Constructor of ThreadInfo created by the JVM
		''' </summary>
		''' <param name="t">             Thread </param>
		''' <param name="state">         Thread state </param>
		''' <param name="lockObj">       Object on which the thread is blocked </param>
		''' <param name="lockOwner">     the thread holding the lock </param>
		''' <param name="blockedCount">  Number of times blocked to enter a lock </param>
		''' <param name="blockedTime">   Approx time blocked to enter a lock </param>
		''' <param name="waitedCount">   Number of times waited on a lock </param>
		''' <param name="waitedTime">    Approx time waited on a lock </param>
		''' <param name="stackTrace">    Thread stack trace </param>
		Private Sub New(  t As Thread,   state As Integer,   lockObj As Object,   lockOwner As Thread,   blockedCount As Long,   blockedTime As Long,   waitedCount As Long,   waitedTime As Long,   stackTrace As StackTraceElement())
			initialize(t, state, lockObj, lockOwner, blockedCount, blockedTime, waitedCount, waitedTime, stackTrace, EMPTY_MONITORS, EMPTY_SYNCS)
		End Sub

		''' <summary>
		''' Constructor of ThreadInfo created by the JVM
		''' for <seealso cref="ThreadMXBean#getThreadInfo(long[],boolean,boolean)"/>
		''' and <seealso cref="ThreadMXBean#dumpAllThreads"/>
		''' </summary>
		''' <param name="t">             Thread </param>
		''' <param name="state">         Thread state </param>
		''' <param name="lockObj">       Object on which the thread is blocked </param>
		''' <param name="lockOwner">     the thread holding the lock </param>
		''' <param name="blockedCount">  Number of times blocked to enter a lock </param>
		''' <param name="blockedTime">   Approx time blocked to enter a lock </param>
		''' <param name="waitedCount">   Number of times waited on a lock </param>
		''' <param name="waitedTime">    Approx time waited on a lock </param>
		''' <param name="stackTrace">    Thread stack trace </param>
		''' <param name="monitors">      List of locked monitors </param>
		''' <param name="stackDepths">   List of stack depths </param>
		''' <param name="synchronizers"> List of locked synchronizers </param>
		Private Sub New(  t As Thread,   state As Integer,   lockObj As Object,   lockOwner As Thread,   blockedCount As Long,   blockedTime As Long,   waitedCount As Long,   waitedTime As Long,   stackTrace As StackTraceElement(),   monitors As Object(),   stackDepths As Integer(),   synchronizers As Object())
			Dim numMonitors As Integer = (If(monitors Is Nothing, 0, monitors.Length))
			Dim lockedMonitors_Renamed As MonitorInfo()
			If numMonitors = 0 Then
				lockedMonitors_Renamed = EMPTY_MONITORS
			Else
				lockedMonitors_Renamed = New MonitorInfo(numMonitors - 1){}
				For i As Integer = 0 To numMonitors - 1
					Dim lock As Object = monitors(i)
					Dim className As String = lock.GetType().name
					Dim identityHashCode As Integer = System.identityHashCode(lock)
					Dim depth As Integer = stackDepths(i)
					Dim ste As StackTraceElement = (If(depth >= 0, stackTrace(depth), Nothing))
					lockedMonitors_Renamed(i) = New MonitorInfo(className, identityHashCode, depth, ste)
				Next i
			End If

			Dim numSyncs As Integer = (If(synchronizers Is Nothing, 0, synchronizers.Length))
			Dim lockedSynchronizers_Renamed As LockInfo()
			If numSyncs = 0 Then
				lockedSynchronizers_Renamed = EMPTY_SYNCS
			Else
				lockedSynchronizers_Renamed = New LockInfo(numSyncs - 1){}
				For i As Integer = 0 To numSyncs - 1
					Dim lock As Object = synchronizers(i)
					Dim className As String = lock.GetType().name
					Dim identityHashCode As Integer = System.identityHashCode(lock)
					lockedSynchronizers_Renamed(i) = New LockInfo(className, identityHashCode)
				Next i
			End If

			initialize(t, state, lockObj, lockOwner, blockedCount, blockedTime, waitedCount, waitedTime, stackTrace, lockedMonitors_Renamed, lockedSynchronizers_Renamed)
		End Sub

		''' <summary>
		''' Initialize ThreadInfo object
		''' </summary>
		''' <param name="t">             Thread </param>
		''' <param name="state">         Thread state </param>
		''' <param name="lockObj">       Object on which the thread is blocked </param>
		''' <param name="lockOwner">     the thread holding the lock </param>
		''' <param name="blockedCount">  Number of times blocked to enter a lock </param>
		''' <param name="blockedTime">   Approx time blocked to enter a lock </param>
		''' <param name="waitedCount">   Number of times waited on a lock </param>
		''' <param name="waitedTime">    Approx time waited on a lock </param>
		''' <param name="stackTrace">    Thread stack trace </param>
		''' <param name="lockedMonitors"> List of locked monitors </param>
		''' <param name="lockedSynchronizers"> List of locked synchronizers </param>
		Private Sub initialize(  t As Thread,   state As Integer,   lockObj As Object,   lockOwner As Thread,   blockedCount As Long,   blockedTime As Long,   waitedCount As Long,   waitedTime As Long,   stackTrace As StackTraceElement(),   lockedMonitors As MonitorInfo(),   lockedSynchronizers As LockInfo())
			Me.threadId = t.id
			Me.threadName = t.name
			Me.threadState = sun.management.ManagementFactoryHelper.toThreadState(state)
			Me.suspended = sun.management.ManagementFactoryHelper.isThreadSuspended(state)
			Me.inNative = sun.management.ManagementFactoryHelper.isThreadRunningNative(state)
			Me.blockedCount = blockedCount
			Me.blockedTime = blockedTime
			Me.waitedCount = waitedCount
			Me.waitedTime = waitedTime

			If lockObj Is Nothing Then
				Me.lock = Nothing
				Me.lockName = Nothing
			Else
				Me.lock = New LockInfo(lockObj)
				Me.lockName = lock.className + AscW("@"c) +  java.lang.[Integer].toHexString(lock.identityHashCode)
			End If
			If lockOwner Is Nothing Then
				Me.lockOwnerId = -1
				Me.lockOwnerName = Nothing
			Else
				Me.lockOwnerId = lockOwner.id
				Me.lockOwnerName = lockOwner.name
			End If
			If stackTrace Is Nothing Then
				Me.stackTrace = NO_STACK_TRACE
			Else
				Me.stackTrace = stackTrace
			End If
			Me.lockedMonitors = lockedMonitors
			Me.lockedSynchronizers = lockedSynchronizers
		End Sub

	'    
	'     * Constructs a <tt>ThreadInfo</tt> object from a
	'     * {@link CompositeData CompositeData}.
	'     
		Private Sub New(  cd As javax.management.openmbean.CompositeData)
			Dim ticd As sun.management.ThreadInfoCompositeData = sun.management.ThreadInfoCompositeData.getInstance(cd)

			threadId = ticd.threadId()
			threadName = ticd.threadName()
			blockedTime = ticd.blockedTime()
			blockedCount = ticd.blockedCount()
			waitedTime = ticd.waitedTime()
			waitedCount = ticd.waitedCount()
			lockName = ticd.lockName()
			lockOwnerId = ticd.lockOwnerId()
			lockOwnerName = ticd.lockOwnerName()
			threadState = ticd.threadState()
			suspended = ticd.suspended()
			inNative = ticd.inNative()
			stackTrace = ticd.stackTrace()

			' 6.0 attributes
			If ticd.currentVersion Then
				lock = ticd.lockInfo()
				lockedMonitors = ticd.lockedMonitors()
				lockedSynchronizers = ticd.lockedSynchronizers()
			Else
				' lockInfo is a new attribute added in 1.6 ThreadInfo
				' If cd is a 5.0 version, construct the LockInfo object
				'  from the lockName value.
				If lockName IsNot Nothing Then
					Dim result As String() = lockName.Split("@")
					If result.Length = 2 Then
						Dim identityHashCode As Integer = Convert.ToInt32(result(1), 16)
						lock = New LockInfo(result(0), identityHashCode)
					Else
						Debug.Assert(result.Length = 2)
						lock = Nothing
					End If
				Else
					lock = Nothing
				End If
				lockedMonitors = EMPTY_MONITORS
				lockedSynchronizers = EMPTY_SYNCS
			End If
		End Sub

		''' <summary>
		''' Returns the ID of the thread associated with this <tt>ThreadInfo</tt>.
		''' </summary>
		''' <returns> the ID of the associated thread. </returns>
		Public Overridable Property threadId As Long
			Get
				Return threadId
			End Get
		End Property

		''' <summary>
		''' Returns the name of the thread associated with this <tt>ThreadInfo</tt>.
		''' </summary>
		''' <returns> the name of the associated thread. </returns>
		Public Overridable Property threadName As String
			Get
				Return threadName
			End Get
		End Property

		''' <summary>
		''' Returns the state of the thread associated with this <tt>ThreadInfo</tt>.
		''' </summary>
		''' <returns> <tt>Thread.State</tt> of the associated thread. </returns>
		Public Overridable Property threadState As Thread.State
			Get
				 Return threadState
			End Get
		End Property

		''' <summary>
		''' Returns the approximate accumulated elapsed time (in milliseconds)
		''' that the thread associated with this <tt>ThreadInfo</tt>
		''' has blocked to enter or reenter a monitor
		''' since thread contention monitoring is enabled.
		''' I.e. the total accumulated time the thread has been in the
		''' <seealso cref="java.lang.Thread.State#BLOCKED BLOCKED"/> state since thread
		''' contention monitoring was last enabled.
		''' This method returns <tt>-1</tt> if thread contention monitoring
		''' is disabled.
		''' 
		''' <p>The Java virtual machine may measure the time with a high
		''' resolution timer.  This statistic is reset when
		''' the thread contention monitoring is reenabled.
		''' </summary>
		''' <returns> the approximate accumulated elapsed time in milliseconds
		''' that a thread entered the <tt>BLOCKED</tt> state;
		''' <tt>-1</tt> if thread contention monitoring is disabled.
		''' </returns>
		''' <exception cref="java.lang.UnsupportedOperationException"> if the Java
		''' virtual machine does not support this operation.
		''' </exception>
		''' <seealso cref= ThreadMXBean#isThreadContentionMonitoringSupported </seealso>
		''' <seealso cref= ThreadMXBean#setThreadContentionMonitoringEnabled </seealso>
		Public Overridable Property blockedTime As Long
			Get
				Return blockedTime
			End Get
		End Property

		''' <summary>
		''' Returns the total number of times that
		''' the thread associated with this <tt>ThreadInfo</tt>
		''' blocked to enter or reenter a monitor.
		''' I.e. the number of times a thread has been in the
		''' <seealso cref="java.lang.Thread.State#BLOCKED BLOCKED"/> state.
		''' </summary>
		''' <returns> the total number of times that the thread
		''' entered the <tt>BLOCKED</tt> state. </returns>
		Public Overridable Property blockedCount As Long
			Get
				Return blockedCount
			End Get
		End Property

		''' <summary>
		''' Returns the approximate accumulated elapsed time (in milliseconds)
		''' that the thread associated with this <tt>ThreadInfo</tt>
		''' has waited for notification
		''' since thread contention monitoring is enabled.
		''' I.e. the total accumulated time the thread has been in the
		''' <seealso cref="java.lang.Thread.State#WAITING WAITING"/>
		''' or <seealso cref="java.lang.Thread.State#TIMED_WAITING TIMED_WAITING"/> state
		''' since thread contention monitoring is enabled.
		''' This method returns <tt>-1</tt> if thread contention monitoring
		''' is disabled.
		''' 
		''' <p>The Java virtual machine may measure the time with a high
		''' resolution timer.  This statistic is reset when
		''' the thread contention monitoring is reenabled.
		''' </summary>
		''' <returns> the approximate accumulated elapsed time in milliseconds
		''' that a thread has been in the <tt>WAITING</tt> or
		''' <tt>TIMED_WAITING</tt> state;
		''' <tt>-1</tt> if thread contention monitoring is disabled.
		''' </returns>
		''' <exception cref="java.lang.UnsupportedOperationException"> if the Java
		''' virtual machine does not support this operation.
		''' </exception>
		''' <seealso cref= ThreadMXBean#isThreadContentionMonitoringSupported </seealso>
		''' <seealso cref= ThreadMXBean#setThreadContentionMonitoringEnabled </seealso>
		Public Overridable Property waitedTime As Long
			Get
				Return waitedTime
			End Get
		End Property

		''' <summary>
		''' Returns the total number of times that
		''' the thread associated with this <tt>ThreadInfo</tt>
		''' waited for notification.
		''' I.e. the number of times that a thread has been
		''' in the <seealso cref="java.lang.Thread.State#WAITING WAITING"/>
		''' or <seealso cref="java.lang.Thread.State#TIMED_WAITING TIMED_WAITING"/> state.
		''' </summary>
		''' <returns> the total number of times that the thread
		''' was in the <tt>WAITING</tt> or <tt>TIMED_WAITING</tt> state. </returns>
		Public Overridable Property waitedCount As Long
			Get
				Return waitedCount
			End Get
		End Property

		''' <summary>
		''' Returns the <tt>LockInfo</tt> of an object for which
		''' the thread associated with this <tt>ThreadInfo</tt>
		''' is blocked waiting.
		''' A thread can be blocked waiting for one of the following:
		''' <ul>
		''' <li>an object monitor to be acquired for entering or reentering
		'''     a synchronization block/method.
		'''     <br>The thread is in the <seealso cref="java.lang.Thread.State#BLOCKED BLOCKED"/>
		'''     state waiting to enter the <tt>synchronized</tt> statement
		'''     or method.
		'''     <p></li>
		''' <li>an object monitor to be notified by another thread.
		'''     <br>The thread is in the <seealso cref="java.lang.Thread.State#WAITING WAITING"/>
		'''     or <seealso cref="java.lang.Thread.State#TIMED_WAITING TIMED_WAITING"/> state
		'''     due to a call to the <seealso cref="Object#wait Object.wait"/> method.
		'''     <p></li>
		''' <li>a synchronization object responsible for the thread parking.
		'''     <br>The thread is in the <seealso cref="java.lang.Thread.State#WAITING WAITING"/>
		'''     or <seealso cref="java.lang.Thread.State#TIMED_WAITING TIMED_WAITING"/> state
		'''     due to a call to the
		'''     {@link java.util.concurrent.locks.LockSupport#park(Object)
		'''     LockSupport.park} method.  The synchronization object
		'''     is the object returned from
		'''     {@link java.util.concurrent.locks.LockSupport#getBlocker
		'''     LockSupport.getBlocker} method. Typically it is an
		'''     <a href="LockInfo.html#OwnableSynchronizer"> ownable synchronizer</a>
		'''     or a <seealso cref="java.util.concurrent.locks.Condition Condition"/>.</li>
		''' </ul>
		''' 
		''' <p>This method returns <tt>null</tt> if the thread is not in any of
		''' the above conditions.
		''' </summary>
		''' <returns> <tt>LockInfo</tt> of an object for which the thread
		'''         is blocked waiting if any; <tt>null</tt> otherwise.
		''' @since 1.6 </returns>
		Public Overridable Property lockInfo As LockInfo
			Get
				Return lock
			End Get
		End Property

		''' <summary>
		''' Returns the <seealso cref="LockInfo#toString string representation"/>
		''' of an object for which the thread associated with this
		''' <tt>ThreadInfo</tt> is blocked waiting.
		''' This method is equivalent to calling:
		''' <blockquote>
		''' <pre>
		''' getLockInfo().toString()
		''' </pre></blockquote>
		''' 
		''' <p>This method will return <tt>null</tt> if this thread is not blocked
		''' waiting for any object or if the object is not owned by any thread.
		''' </summary>
		''' <returns> the string representation of the object on which
		''' the thread is blocked if any;
		''' <tt>null</tt> otherwise.
		''' </returns>
		''' <seealso cref= #getLockInfo </seealso>
		Public Overridable Property lockName As String
			Get
				Return lockName
			End Get
		End Property

		''' <summary>
		''' Returns the ID of the thread which owns the object
		''' for which the thread associated with this <tt>ThreadInfo</tt>
		''' is blocked waiting.
		''' This method will return <tt>-1</tt> if this thread is not blocked
		''' waiting for any object or if the object is not owned by any thread.
		''' </summary>
		''' <returns> the thread ID of the owner thread of the object
		''' this thread is blocked on;
		''' <tt>-1</tt> if this thread is not blocked
		''' or if the object is not owned by any thread.
		''' </returns>
		''' <seealso cref= #getLockInfo </seealso>
		Public Overridable Property lockOwnerId As Long
			Get
				Return lockOwnerId
			End Get
		End Property

		''' <summary>
		''' Returns the name of the thread which owns the object
		''' for which the thread associated with this <tt>ThreadInfo</tt>
		''' is blocked waiting.
		''' This method will return <tt>null</tt> if this thread is not blocked
		''' waiting for any object or if the object is not owned by any thread.
		''' </summary>
		''' <returns> the name of the thread that owns the object
		''' this thread is blocked on;
		''' <tt>null</tt> if this thread is not blocked
		''' or if the object is not owned by any thread.
		''' </returns>
		''' <seealso cref= #getLockInfo </seealso>
		Public Overridable Property lockOwnerName As String
			Get
				Return lockOwnerName
			End Get
		End Property

		''' <summary>
		''' Returns the stack trace of the thread
		''' associated with this <tt>ThreadInfo</tt>.
		''' If no stack trace was requested for this thread info, this method
		''' will return a zero-length array.
		''' If the returned array is of non-zero length then the first element of
		''' the array represents the top of the stack, which is the most recent
		''' method invocation in the sequence.  The last element of the array
		''' represents the bottom of the stack, which is the least recent method
		''' invocation in the sequence.
		''' 
		''' <p>Some Java virtual machines may, under some circumstances, omit one
		''' or more stack frames from the stack trace.  In the extreme case,
		''' a virtual machine that has no stack trace information concerning
		''' the thread associated with this <tt>ThreadInfo</tt>
		''' is permitted to return a zero-length array from this method.
		''' </summary>
		''' <returns> an array of <tt>StackTraceElement</tt> objects of the thread. </returns>
		Public Overridable Property stackTrace As StackTraceElement()
			Get
				Return stackTrace
			End Get
		End Property

		''' <summary>
		''' Tests if the thread associated with this <tt>ThreadInfo</tt>
		''' is suspended.  This method returns <tt>true</tt> if
		''' <seealso cref="Thread#suspend"/> has been called.
		''' </summary>
		''' <returns> <tt>true</tt> if the thread is suspended;
		'''         <tt>false</tt> otherwise. </returns>
		Public Overridable Property suspended As Boolean
			Get
				 Return suspended
			End Get
		End Property

		''' <summary>
		''' Tests if the thread associated with this <tt>ThreadInfo</tt>
		''' is executing native code via the Java Native Interface (JNI).
		''' The JNI native code does not include
		''' the virtual machine support code or the compiled native
		''' code generated by the virtual machine.
		''' </summary>
		''' <returns> <tt>true</tt> if the thread is executing native code;
		'''         <tt>false</tt> otherwise. </returns>
		Public Overridable Property inNative As Boolean
			Get
				 Return inNative
			End Get
		End Property

		''' <summary>
		''' Returns a string representation of this thread info.
		''' The format of this string depends on the implementation.
		''' The returned string will typically include
		''' the <seealso cref="#getThreadName thread name"/>,
		''' the <seealso cref="#getThreadId thread ID"/>,
		''' its <seealso cref="#getThreadState state"/>,
		''' and a <seealso cref="#getStackTrace stack trace"/> if any.
		''' </summary>
		''' <returns> a string representation of this thread info. </returns>
		Public Overrides Function ToString() As String
			Dim sb As New StringBuilder("""" & threadName & """" & " Id=" & threadId & " " & threadState)
			If lockName IsNot Nothing Then sb.append(" on " & lockName)
			If lockOwnerName IsNot Nothing Then sb.append(" owned by """ & lockOwnerName & """ Id=" & lockOwnerId)
			If suspended Then sb.append(" (suspended)")
			If inNative Then sb.append(" (in native)")
			sb.append(ControlChars.Lf)
			Dim i As Integer = 0
			Do While i < stackTrace.Length AndAlso i < MAX_FRAMES
				Dim ste As StackTraceElement = stackTrace(i)
				sb.append(vbTab & "at " & ste.ToString())
				sb.append(ControlChars.Lf)
				If i = 0 AndAlso lockInfo IsNot Nothing Then
					Dim ts As Thread.State = threadState
					Select Case ts
						Case Thread.State.BLOCKED
							sb.append(vbTab & "-  blocked on " & lockInfo)
							sb.append(ControlChars.Lf)
						Case Thread.State.WAITING
							sb.append(vbTab & "-  waiting on " & lockInfo)
							sb.append(ControlChars.Lf)
						Case Thread.State.TIMED_WAITING
							sb.append(vbTab & "-  waiting on " & lockInfo)
							sb.append(ControlChars.Lf)
						Case Else
					End Select
				End If

				For Each mi As MonitorInfo In lockedMonitors
					If mi.lockedStackDepth = i Then
						sb.append(vbTab & "-  locked " & mi)
						sb.append(ControlChars.Lf)
					End If
				Next mi
				i += 1
			Loop
		   If i < stackTrace.Length Then
			   sb.append(vbTab & "...")
			   sb.append(ControlChars.Lf)
		   End If

		   Dim locks As LockInfo() = lockedSynchronizers
		   If locks.Length > 0 Then
			   sb.append(vbLf & vbTab & "Number of locked synchronizers = " & locks.Length)
			   sb.append(ControlChars.Lf)
			   For Each li As LockInfo In locks
				   sb.append(vbTab & "- " & li)
				   sb.append(ControlChars.Lf)
			   Next li
		   End If
		   sb.append(ControlChars.Lf)
		   Return sb.ToString()
		End Function
		Private Const MAX_FRAMES As Integer = 8

		''' <summary>
		''' Returns a <tt>ThreadInfo</tt> object represented by the
		''' given <tt>CompositeData</tt>.
		''' The given <tt>CompositeData</tt> must contain the following attributes
		''' unless otherwise specified below:
		''' <blockquote>
		''' <table border summary="The attributes and their types the given CompositeData contains">
		''' <tr>
		'''   <th align=left>Attribute Name</th>
		'''   <th align=left>Type</th>
		''' </tr>
		''' <tr>
		'''   <td>threadId</td>
		'''   <td><tt>java.lang.Long</tt></td>
		''' </tr>
		''' <tr>
		'''   <td>threadName</td>
		'''   <td><tt>java.lang.String</tt></td>
		''' </tr>
		''' <tr>
		'''   <td>threadState</td>
		'''   <td><tt>java.lang.String</tt></td>
		''' </tr>
		''' <tr>
		'''   <td>suspended</td>
		'''   <td><tt>java.lang.Boolean</tt></td>
		''' </tr>
		''' <tr>
		'''   <td>inNative</td>
		'''   <td><tt>java.lang.Boolean</tt></td>
		''' </tr>
		''' <tr>
		'''   <td>blockedCount</td>
		'''   <td><tt>java.lang.Long</tt></td>
		''' </tr>
		''' <tr>
		'''   <td>blockedTime</td>
		'''   <td><tt>java.lang.Long</tt></td>
		''' </tr>
		''' <tr>
		'''   <td>waitedCount</td>
		'''   <td><tt>java.lang.Long</tt></td>
		''' </tr>
		''' <tr>
		'''   <td>waitedTime</td>
		'''   <td><tt>java.lang.Long</tt></td>
		''' </tr>
		''' <tr>
		'''   <td>lockInfo</td>
		'''   <td><tt>javax.management.openmbean.CompositeData</tt>
		'''       - the mapped type for <seealso cref="LockInfo"/> as specified in the
		'''         <seealso cref="LockInfo#from"/> method.
		'''       <p>
		'''       If <tt>cd</tt> does not contain this attribute,
		'''       the <tt>LockInfo</tt> object will be constructed from
		'''       the value of the <tt>lockName</tt> attribute. </td>
		''' </tr>
		''' <tr>
		'''   <td>lockName</td>
		'''   <td><tt>java.lang.String</tt></td>
		''' </tr>
		''' <tr>
		'''   <td>lockOwnerId</td>
		'''   <td><tt>java.lang.Long</tt></td>
		''' </tr>
		''' <tr>
		'''   <td>lockOwnerName</td>
		'''   <td><tt>java.lang.String</tt></td>
		''' </tr>
		''' <tr>
		'''   <td><a name="StackTrace">stackTrace</a></td>
		'''   <td><tt>javax.management.openmbean.CompositeData[]</tt>
		'''       <p>
		'''       Each element is a <tt>CompositeData</tt> representing
		'''       StackTraceElement containing the following attributes:
		'''       <blockquote>
		'''       <table cellspacing=1 cellpadding=0 summary="The attributes and their types the given CompositeData contains">
		'''       <tr>
		'''         <th align=left>Attribute Name</th>
		'''         <th align=left>Type</th>
		'''       </tr>
		'''       <tr>
		'''         <td>className</td>
		'''         <td><tt>java.lang.String</tt></td>
		'''       </tr>
		'''       <tr>
		'''         <td>methodName</td>
		'''         <td><tt>java.lang.String</tt></td>
		'''       </tr>
		'''       <tr>
		'''         <td>fileName</td>
		'''         <td><tt>java.lang.String</tt></td>
		'''       </tr>
		'''       <tr>
		'''         <td>lineNumber</td>
		'''         <td><tt>java.lang.Integer</tt></td>
		'''       </tr>
		'''       <tr>
		'''         <td>nativeMethod</td>
		'''         <td><tt>java.lang.Boolean</tt></td>
		'''       </tr>
		'''       </table>
		'''       </blockquote>
		'''   </td>
		''' </tr>
		''' <tr>
		'''   <td>lockedMonitors</td>
		'''   <td><tt>javax.management.openmbean.CompositeData[]</tt>
		'''       whose element type is the mapped type for
		'''       <seealso cref="MonitorInfo"/> as specified in the
		'''       <seealso cref="MonitorInfo#from Monitor.from"/> method.
		'''       <p>
		'''       If <tt>cd</tt> does not contain this attribute,
		'''       this attribute will be set to an empty array. </td>
		''' </tr>
		''' <tr>
		'''   <td>lockedSynchronizers</td>
		'''   <td><tt>javax.management.openmbean.CompositeData[]</tt>
		'''       whose element type is the mapped type for
		'''       <seealso cref="LockInfo"/> as specified in the <seealso cref="LockInfo#from"/> method.
		'''       <p>
		'''       If <tt>cd</tt> does not contain this attribute,
		'''       this attribute will be set to an empty array. </td>
		''' </tr>
		''' </table>
		''' </blockquote>
		''' </summary>
		''' <param name="cd"> <tt>CompositeData</tt> representing a <tt>ThreadInfo</tt>
		''' </param>
		''' <exception cref="IllegalArgumentException"> if <tt>cd</tt> does not
		'''   represent a <tt>ThreadInfo</tt> with the attributes described
		'''   above.
		''' </exception>
		''' <returns> a <tt>ThreadInfo</tt> object represented
		'''         by <tt>cd</tt> if <tt>cd</tt> is not <tt>null</tt>;
		'''         <tt>null</tt> otherwise. </returns>
		Public Shared Function [from](  cd As javax.management.openmbean.CompositeData) As ThreadInfo
			If cd Is Nothing Then Return Nothing

			If TypeOf cd Is sun.management.ThreadInfoCompositeData Then
				Return CType(cd, sun.management.ThreadInfoCompositeData).threadInfo
			Else
				Return New ThreadInfo(cd)
			End If
		End Function

		''' <summary>
		''' Returns an array of <seealso cref="MonitorInfo"/> objects, each of which
		''' represents an object monitor currently locked by the thread
		''' associated with this <tt>ThreadInfo</tt>.
		''' If no locked monitor was requested for this thread info or
		''' no monitor is locked by the thread, this method
		''' will return a zero-length array.
		''' </summary>
		''' <returns> an array of <tt>MonitorInfo</tt> objects representing
		'''         the object monitors locked by the thread.
		''' 
		''' @since 1.6 </returns>
		Public Overridable Property lockedMonitors As MonitorInfo()
			Get
				Return lockedMonitors
			End Get
		End Property

		''' <summary>
		''' Returns an array of <seealso cref="LockInfo"/> objects, each of which
		''' represents an <a href="LockInfo.html#OwnableSynchronizer">ownable
		''' synchronizer</a> currently locked by the thread associated with
		''' this <tt>ThreadInfo</tt>.  If no locked synchronizer was
		''' requested for this thread info or no synchronizer is locked by
		''' the thread, this method will return a zero-length array.
		''' </summary>
		''' <returns> an array of <tt>LockInfo</tt> objects representing
		'''         the ownable synchronizers locked by the thread.
		''' 
		''' @since 1.6 </returns>
		Public Overridable Property lockedSynchronizers As LockInfo()
			Get
				Return lockedSynchronizers
			End Get
		End Property

		Private Shared ReadOnly NO_STACK_TRACE As StackTraceElement() = New StackTraceElement(){}
	End Class

End Namespace